using DiscordAngryBot.CustomObjects.Bans;
using DiscordAngryBot.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace DiscordAngryBot.APIHandlers.Controllers
{
    /// <summary>
    /// API controller for guild bans
    /// </summary>
    [RoutePrefix("api/Guilds")]
    public class BansController : ApiController
    {
        /// <summary>
        /// Gets list of bans for the guild
        /// </summary>
        /// <param name="guildID">Guild ID</param>
        /// <returns></returns>
        [HttpGet, Route("{guildID}/Bans")]
        public List<BanReference> GetBans(string guildID)
        {
            List<DiscordBan> bans = BotCore.GetDiscordGuildBans(ulong.Parse(guildID));
            List<BanReference> returnData = new List<BanReference>();
            foreach (var ban in bans)
            {
                returnData.Add(ban.GetReference());
            }
            return returnData;
        }
    }
}
