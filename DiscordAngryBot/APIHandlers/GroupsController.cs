using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
using DiscordAngryBot.CustomObjects.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <summary>
        /// Получение всех групп
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [HttpGet]
        public List<object> GetGroups()
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
        [Route("{id}")]
        [HttpGet]
        public object GetGroup(string id)
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
        [Route("")]
        [HttpGet]
        public List<object> GetUsers()
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
}
