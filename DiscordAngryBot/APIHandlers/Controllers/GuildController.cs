﻿using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DiscordAngryBot.APIHandlers.Controllers
{
    [RoutePrefix("api/Guilds")]
    public class GuildController : ApiController
    {
        [HttpGet, Route("")]
        public List<object> GetGuildsForAPI()
        {
            var guilds = BotCore.GetClient().Guilds;
            List<object> returnList = new List<object>();
            foreach (SocketGuild guild in guilds)
            {
                returnList.Add(new { GuildName = guild.Name, GuildID = guild.Id, Channels = guild.Channels.Select(x => new { Name = x.Name, ID = x.Id }).ToList()});
            }
            return returnList;
        }

        [HttpGet, Route("{guildID}")]
        public object GetGuildForAPI(string guildID)
        {
            var guild = BotCore.GetGuildDataCache(ulong.Parse(guildID)).Guild;
            return new { GuildName = guild.Name, GuildID = guild.Id, Channels = guild.Channels.Select(x => new { Name = x.Name, ID = x.Id }).ToList() };
        }
    }
}
