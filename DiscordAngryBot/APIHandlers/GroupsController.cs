using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
using DiscordAngryBot.CustomObjects.Groups;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DiscordAngryBot.APIHandlers
{
    /// <summary>
    /// Контроллер API для объектов Group
    /// </summary>
    [RoutePrefix("api/Groups")]
    public class GroupsController : ApiController
    {
        [HttpGet, Route("")]
        public List<GroupJSONObject> GetGroups()
        {
            List<Group> groups = Program.FetchData().groups;
            List<GroupJSONObject> returnData = new List<GroupJSONObject>();
            foreach (var group in groups)
            {
                returnData.Add(new GroupJSONObject(group));
            }
            return returnData;
        }

        /// <summary>
        /// Получение всех групп
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("SimpleGroups")]
        public List<object> GetSimpleGroups()
        {
            List<Group> groups = Program.FetchData().groups;
            List<object> returnData = new List<object>();

            foreach (var group in groups)
            {

                if (!group.IsGuildFight())
                {
                    returnData.Add(new
                    {
                        GUID = group.GUID,
                        Author = group.author.Username,
                        Channel = group.channel.Name,
                        Users = group.users.Select(x => new { Name = x.Username }),
                        UserLimit = group.userLimit,
                        TargetMessageID = group.targetMessage.Id,
                        CreatedAt = group.createdAt,
                        Goal = group.destination
                    });
                }
                else
                {
                    returnData.Add(new
                    {
                        GUID = group.GUID,
                        Author = group.author.Username,
                        Channel = group.channel.Name,
                        ReadyUsers = group.users.Select(x => new { Name = x.Username }),
                        NoGearUsers = ((GuildFight)group).noGearUsers.Select(x => new { Name = x.Username }),
                        UnwillingUsers = ((GuildFight)group).unwillingUsers.Select(x => new { Name = x.Username }),
                        UnsureUsers = ((GuildFight)group).unsureUsers.Select(x => new { Name = x.Username }),
                        UserLimit = group.userLimit,
                        TargetMessageID = group.targetMessage.Id,
                        CreatedAt = group.createdAt,
                        Goal = group.destination
                    });
                }
            }
            ConsoleWriter.Write($"[GET]: fetching groups list for {((RemoteEndpointMessageProperty)Request.Properties[RemoteEndpointMessageProperty.Name]).Address}", ConsoleWriter.InfoType.Notice);
            return returnData;
        }

        /// <summary>
        /// Поиск группы по GUID'у
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("SimpleGroup/{id}")]
        public object GetSimpleGroup(string id)
        {
            Group target = Program.FetchData().groups.Where(x => x.GUID == id).SingleOrDefault();
            if (target != null)
            {
                ConsoleWriter.Write($"[GET]: fetching group {target.GUID} for {((RemoteEndpointMessageProperty)Request.Properties[RemoteEndpointMessageProperty.Name]).Address}", ConsoleWriter.InfoType.Notice);
                List<string> usersList = new List<string>();
                foreach (var user in target.users)
                {
                    usersList.Add(user.Username);
                }
                return new
                {
                    GUID = target.GUID,
                    Author = target.author.Username,
                    Channel = target.channel.Name,
                    Users = usersList,
                    UserLimit = target.userLimit,
                    TargetMessageID = target.targetMessage.Id,
                    CreatedAt = target.createdAt,
                    Goal = target.destination
                };
            }
            else
                return "Такой группы не найдено";
        }
    }

    /// <summary>
    /// Контроллер API для юзеров 
    /// </summary>
    [RoutePrefix("api/Users")]
    public class UsersController : ApiController
    {
        /// <summary>
        /// Получение всех юзеров
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("SimpleUsers")]
        public List<object> GetSimpleUsers()
        {
            var userList = Program.FetchServerObject().users;
            List<object> userReturnList = new List<object>();
            foreach (var user in userList)
            {
                var guildUser = (SocketGuildUser)user;
                userReturnList.Add(new 
                { 
                    Nickname = guildUser.Nickname,
                    Username = guildUser.Username,
                    Mention = guildUser.Mention,
                    Roles = guildUser.Roles.Select(x => new {RoleName  = x.Name, RoleID = x.Id })
                });
            }
            return userReturnList;
        }
    }

    [RoutePrefix("api/Birthday")]
    public class MiscController : ApiController
    {
        [HttpGet, Route("")]
        public string GetTimeUntilBirtday()
        {
            var timeSpan = DateTime.Parse("31.05.2020") - DateTime.Now;
            return $"Осталось {timeSpan.Days} дней, {timeSpan.Hours} часов, {timeSpan.Minutes} минут, {timeSpan.Seconds} секунд";
        }

        [HttpGet, Route("Web")]
        public HttpResponseMessage GetTimeUntilBirtdayAsWebpage()
        {
            var timeSpan = DateTime.Parse("31.05.2020") - DateTime.Now;
            string output = $"Осталось {timeSpan.Days} дней, {timeSpan.Hours} часов, {timeSpan.Minutes} минут, {timeSpan.Seconds} секунд";
            var response = new HttpResponseMessage();
            response.Content = new StringContent($"<div style=\"font-size: 40px; padding: 20px;\">{output}</div>");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            response.Content.Headers.ContentType.CharSet = Encoding.UTF8.HeaderName;
            response.Content.Headers.Add("CodePage", Encoding.UTF8.CodePage.ToString());

            return response;
        }
    }

}
