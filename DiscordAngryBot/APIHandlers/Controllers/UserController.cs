using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DiscordAngryBot.APIHandlers.Controllers
{
    /// <summary>
    /// API controller for users
    /// </summary>
    [RoutePrefix("api/Guilds")]
    public class UserController : ApiController
    {
        /// <summary>
        /// Gets all users from guild
        /// </summary>
        /// <param name="guildID">Guild ID</param>
        /// <returns></returns>
        [HttpGet, Route("{guildID}/Users")]
        public List<object> GetSimpleUsers(string guildID)
        {
            if (BotCore.TryGetExtendedDiscordGuildBotData(ulong.Parse(guildID), out var cache))
            {
                var userList = cache.Guild.Users;
                List<object> userReturnList = new List<object>();
                foreach (var user in userList)
                {
                    var guildUser = (SocketGuildUser)user;
                    userReturnList.Add(new
                    {
                        guildUser.Nickname,
                        guildUser.Username,
                        Roles = guildUser.Roles.Select(x => x.Name)
                    });
                }
                return userReturnList;
            }
            else 
                return null;
        }
    }
}
