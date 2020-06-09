using DiscordAngryBot.CustomObjects.Bans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DiscordAngryBot.APIHandlers.Controllers
{
    [RoutePrefix("api/Bans")]
    public class BansController : ApiController
    {
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
