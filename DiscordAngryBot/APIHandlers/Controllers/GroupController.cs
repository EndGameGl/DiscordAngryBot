using Discord;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
using DiscordAngryBot.CustomObjects.Groups;
using DiscordAngryBot.CustomObjects.News;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DiscordAngryBot.APIHandlers.Controllers
{

    /// <summary>
    /// Контроллер API для объектов Group
    /// </summary>
    [RoutePrefix("api/Guilds")]
    public class GroupController : ApiController
    {
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
