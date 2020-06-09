using Discord;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
using DiscordAngryBot.CustomObjects.News;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DiscordAngryBot.APIHandlers.Controllers
{
    [RoutePrefix("api/News")]
    public class NewsController : ApiController
    {
        [HttpPost, Route("")]
        public string PostNews(object data)
        {
            try
            {
                var newsInfo = JsonConvert.DeserializeObject<NewsData>(data.ToString());
                Dictionary<string, SocketTextChannel> channelsList = new Dictionary<string, SocketTextChannel>();
                foreach (var guild in BotCore.GetClient().Guilds.Select(x => x.Id).ToList())
                {
                    var newsID = BotCore.GetDiscordGuildSettings(guild).NewsChannelID;
                    var apiToken = BotCore.GetDiscordGuildSettings(guild).APIToken;
                    if (newsID != null && apiToken != null)
                    {
                        channelsList.Add(apiToken, BotCore.GetGuildDataCache(guild).Guild.GetTextChannel(newsID.Value));
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
                ConsoleWriter.Write($"{e.Message}", ConsoleWriter.InfoType.Error).GetAwaiter().GetResult();
                return $"Error: {e.Message} [{e.InnerException?.Message}]";
            }
        }
    }
}
