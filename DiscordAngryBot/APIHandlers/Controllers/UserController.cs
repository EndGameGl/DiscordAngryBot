﻿using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DiscordAngryBot.APIHandlers.Controllers
{
    /// <summary>
    /// Контроллер API для юзеров 
    /// </summary>
    [RoutePrefix("api/Users")]
    public class UserController : ApiController
    {
        /// <summary>
        /// Получение всех юзеров
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("{guildID}/SimpleUsers")]
        public List<object> GetSimpleUsers(string guildID)
        {
            if (BotCore.TryGetGuildDataCache(ulong.Parse(guildID), out var cache))
            {
                var userList = cache.Guild.Users;
                List<object> userReturnList = new List<object>();
                foreach (var user in userList)
                {
                    var guildUser = (SocketGuildUser)user;
                    userReturnList.Add(new
                    {
                        Nickname = guildUser.Nickname,
                        Username = guildUser.Username,
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
