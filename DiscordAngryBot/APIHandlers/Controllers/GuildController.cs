using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DiscordAngryBot.APIHandlers.Controllers
{
    /// <summary>
    /// API controller for guilds
    /// </summary>
    [RoutePrefix("api/Guilds")]
    public class GuildController : ApiController
    {
        /// <summary>
        /// Gets all guilds currently connected to this bot
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("")]
        public List<object> GetGuildsForAPI()
        {
            var guilds = BotCore.GetClient().Guilds;
            List<object> returnList = new List<object>();
            foreach (SocketGuild guild in guilds)
            {
                returnList.Add(new { GuildName = guild.Name, GuildID = guild.Id, Channels = guild.Channels.Select(x => new { x.Name, ID = x.Id }).ToList()});
            }
            return returnList;
        }
        /// <summary>
        /// Gets a guild summary info by ID
        /// </summary>
        /// <param name="guildID">Guild ID</param>
        /// <returns></returns>
        [HttpGet, Route("{guildID}")]
        public object GetGuildForAPI(string guildID)
        {
            if (BotCore.TryGetExtendedDiscordGuildBotData(ulong.Parse(guildID), out var cache))
            {
                var guild = cache.Guild;
                return new { GuildName = guild.Name, GuildID = guild.Id, Channels = guild.Channels.Select(x => new { x.Name, ID = x.Id }).ToList() };
            }
            else
                return null;
        }
    }
}
