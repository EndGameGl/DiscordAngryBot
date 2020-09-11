using DiscordAngryBot.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DiscordAngryBot.APIHandlers.Controllers
{

    /// <summary>
    /// API controller for guild groups
    /// </summary>
    [RoutePrefix("api/Guilds")]
    public class GroupController : ApiController
    {
        /// <summary>
        /// Gets list of groups for the guild
        /// </summary>
        /// <param name="guildID">Guild ID</param>
        /// <returns></returns>
        [HttpGet, Route("{guildID}/Groups")]
        public List<GroupReference> GetGroups(string guildID)
        {
            if (BotCore.TryGetDiscordGuildGroups(ulong.Parse(guildID), out var groups))
            {
                List<GroupReference> returnData = new List<GroupReference>();
                foreach (var group in groups)
                {
                    returnData.Add(new GroupReference(group));
                }
                return returnData;
            }
            else
                return null;
        }
        /// <summary>
        /// Gets guild group by ID
        /// </summary>
        /// <param name="guildID">Guild ID</param>
        /// <param name="GUID">Group GUID</param>
        /// <returns></returns>
        [HttpGet, Route("{guildID}/Groups/{GUID}")]
        public object GetGroupByGUID(string guildID, string GUID)
        {
            if (BotCore.TryGetDiscordGuildGroups(ulong.Parse(guildID), out var groups))
            {
                var group = groups.Where(x => x.GUID == GUID).SingleOrDefault();
                if (group != null)
                    return new GroupReference(group);
                else
                    return "No such group was found.";
            }
            else
                return "No such guild was found.";
        }
    }
}
