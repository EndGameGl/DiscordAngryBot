using Discord;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
using DiscordAngryBot.CustomObjects.News;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DiscordAngryBot.APIHandlers.Controllers
{
    /// <summary>
    /// API controller for news
    /// </summary>
    [RoutePrefix("api/News")]
    public class NewsController : ApiController
    {
        /// <summary>
        /// Process posted data and output it to connected guilds if needed
        /// </summary>
        /// <param name="data">Posted data</param>
        /// <returns></returns>
        [HttpPost, Route("")]
        public string PostNews(object data)
        {
            try
            {
                var newsInfo = JsonConvert.DeserializeObject<NewsData>(data.ToString());
                Dictionary<string, SocketTextChannel> channelsList = new Dictionary<string, SocketTextChannel>();
                foreach (var guild in BotCore.GetClient().Guilds.Select(x => x.Id).ToList())
                {
                    if (BotCore.TryGetDiscordGuildSettings(guild, out var guildSettings))
                    {
                        var newsID = guildSettings.NewsChannelID;
                        var apiToken = guildSettings.APIToken;
                        if (newsID != null && apiToken != null)
                        {
                            if (BotCore.TryGetExtendedDiscordGuildBotData(guild, out var cache)) 
                            {
                                channelsList.Add(apiToken, cache.Guild.GetTextChannel(newsID.Value)); 
                            }
                        }
                    }
                }
                if (channelsList.Count() != 0)
                {
                    foreach (var item in channelsList)
                    {
                        if (newsInfo.Token == item.Key)
                        {
                            item.Value.SendMessageAsync($"{newsInfo.Text}\n{newsInfo.SourceURL}", false, new EmbedBuilder().WithImageUrl(newsInfo.ImageURL).Build());
                        }
                    }
                    return "Received all data";
                }
                else 
                {
                    return "No news channels set currently";
                }
            }
            catch (Exception e)
            {
                Debug.Log($"{e.Message}", LogInfoType.Error).GetAwaiter().GetResult();
                return $"Error: {e.Message} [{e.InnerException?.Message}]";
            }
        }
    }
}
