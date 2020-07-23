using DiscordAngryBot.CustomObjects.Bans;
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
        public List<BanJSONobject> GetBans(string guildID)
        {
            List<DiscordBan> bans = BotCore.GetDiscordGuildBans(ulong.Parse(guildID));
            List<BanJSONobject> returnData = new List<BanJSONobject>();
            foreach (var ban in bans)
            {
                returnData.Add(new BanJSONobject(ban));
            }
            return returnData;
        }
    }
}
