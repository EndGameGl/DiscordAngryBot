using DiscordAngryBot.CustomObjects.Bans;
using DiscordAngryBot.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace DiscordAngryBot.APIHandlers.Controllers
{
    /// <summary>
    /// API контроллер банов
    /// </summary>
    [RoutePrefix("api/Bans")]
    public class BansController : ApiController
    {
        /// <summary>
        /// Возврат списка банов в формате JSON
        /// </summary>
        /// <param name="guildID"></param>
        /// <returns></returns>
        [HttpGet, Route("{guildID}")]
        public List<BanReference> GetBans(string guildID)
        {
            List<DiscordBan> bans = BotCore.GetDiscordGuildBans(ulong.Parse(guildID));
            List<BanReference> returnData = new List<BanReference>();
            foreach (var ban in bans)
            {
                returnData.Add(new BanReference(ban));
            }
            return returnData;
        }
    }
}
