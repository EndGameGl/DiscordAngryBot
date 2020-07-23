using DiscordAngryBot.CustomObjects.Groups;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DiscordAngryBot.APIHandlers.Controllers
{

    /// <summary>
    /// Контроллер API для объектов Group
    /// </summary>
    [RoutePrefix("api/Guilds")]
    public class GroupController : ApiController
    {
        /// <summary>
        /// Получение всех групп сервера в формате JSON
        /// </summary>
        /// <param name="guildID"></param>
        /// <returns></returns>
        [HttpGet, Route("{guildID}/Groups")]
        public List<GroupJSONObject> GetGroups(string guildID)
        {
            var groups = BotCore.GetDiscordGuildGroups(ulong.Parse(guildID));
            List<GroupJSONObject> returnData = new List<GroupJSONObject>();
            foreach (var group in groups)
            {
                returnData.Add(new GroupJSONObject(group));
            }
            return returnData;
        }
        /// <summary>
        /// Получение конкретной группы сервера
        /// </summary>
        /// <param name="guildID"></param>
        /// <param name="GUID"></param>
        /// <returns></returns>
        [HttpGet, Route("{guildID}/Groups/{GUID}")]
        public object GetGroupByGUID(string guildID, string GUID)
        {
            Group group = BotCore.GetDiscordGuildGroups(ulong.Parse(guildID)).Where(x => x.GUID == GUID).SingleOrDefault();
            if (group != null)
                return new GroupJSONObject(group);
            else
                return "No such group was found.";
        }
    }
}
