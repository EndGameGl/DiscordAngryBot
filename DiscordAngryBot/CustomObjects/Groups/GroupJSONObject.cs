using Discord.Rest;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Groups
{
    /// <summary>
    /// Вспомогательный класс для сохранения данных группы
    /// </summary>
    public class GroupJSONObject
    {
        /// <summary>
        /// Идентификатор автора
        /// </summary>
        public ulong author_id { get; set; }
        /// <summary>
        /// Идентификатор канала
        /// </summary>
        public ulong channel_id { get; set; }
        /// <summary>
        /// Идентификаторы всех пользователей в группе
        /// </summary>
        public List<ulong> users_id { get; set; } = new List<ulong>();
        /// <summary>
        /// Идентификаторы пользователей без гира (ТОЛЬКО ДЛЯ БИТВ БШ)
        /// </summary>
        public List<ulong> noGearUsers_id { get; set; } = new List<ulong>();
        /// <summary>
        /// Идентификаторы пользователей на замену (ТОЛЬКО ДЛЯ БИТВ БШ)
        /// </summary>
        public List<ulong> unwillingUsers_id { get; set; } = new List<ulong>();
        /// <summary>
        /// Идентификаторы пользоваелей, не уверенных в возможности быть (ТОЛЬКО ДЛЯ БИТВ БШ)
        /// </summary>
        public List<ulong> unsureUsers_id { get; set; } = new List<ulong>();
        /// <summary>
        /// Лимит пользователей в группе
        /// </summary>
        public int userLimit { get; set; }
        /// <summary>
        /// Идентификатор целевого сообщения
        /// </summary>
        public ulong targetMessage_id { get; set; }
        /// <summary>
        /// Цель сбора группы
        /// </summary>
        public string destination { get; set; }
        /// <summary>
        /// Дата создания группы
        /// </summary>
        public DateTime createdAt { get; set; }
        public bool isGuildFight { get; set; }
        /// <summary>
        /// Пустой конструктор класса
        /// </summary>
        public GroupJSONObject() { }
        /// <summary>
        /// Конструктор классе на основе объекта Group
        /// </summary>
        /// <param name="group"></param>
        public GroupJSONObject(Group group)
        {
            author_id = group.author.Id;
            channel_id = group.channel.Id;
            userLimit = group.userLimit;
            foreach (var user in group.users)
            {
                users_id.Add(user.Id);
            }
            if (group is GuildFight)
            {
                foreach (var user in ((GuildFight)group).noGearUsers)
                {
                    noGearUsers_id.Add(user.Id);
                }
                foreach (var user in ((GuildFight)group).unwillingUsers)
                {
                    unwillingUsers_id.Add(user.Id);
                }
                foreach (var user in ((GuildFight)group).unsureUsers)
                {
                    unsureUsers_id.Add(user.Id);
                }
            }
            targetMessage_id = group.targetMessage.Id;
            destination = group.destination;
            createdAt = group.createdAt;
        }
        /// <summary>
        /// Ковертация объекта данных в группу
        /// </summary>
        /// <param name="guild">Сервер, к которому принадлежит группа</param>
        /// <returns></returns>
        public async Task<Group> ConvertToGroup(SocketGuild guild)
        {
            await ConsoleWriter.Write($"Converting Group info object to Group", ConsoleWriter.InfoType.Notice);
            Group group = null;            
            var author = guild.GetUser(author_id);
            await ConsoleWriter.Write($"Getting group author: {author.Username}", ConsoleWriter.InfoType.Notice);
            List<SocketUser> users = new List<SocketUser>();
            var channel = guild.GetChannel(channel_id);
            await ConsoleWriter.Write($"Getting group channel: {channel.Name}", ConsoleWriter.InfoType.Notice);
            await ConsoleWriter.Write($"Getting target message...", ConsoleWriter.InfoType.Notice);
            var targetMessage = await ((ISocketMessageChannel)channel).GetMessageAsync(targetMessage_id);
            await ConsoleWriter.Write($"Adding users to group user list", ConsoleWriter.InfoType.Notice);
            foreach (var userId in users_id)
            {
                users.Add(guild.GetUser(userId));
            }
            await ConsoleWriter.Write($"Added {users.Count} users to group", ConsoleWriter.InfoType.Notice);
            if (userLimit == 6)
            {
                await ConsoleWriter.Write($"Group object is party, creating party", ConsoleWriter.InfoType.Notice);
                group = new Party()
                    {
                        author = author,
                        users = users,
                        userLimit = userLimit,
                        channel = (ISocketMessageChannel)channel,
                        createdAt = createdAt,
                        destination = destination,
                        targetMessage = (RestUserMessage)targetMessage,
                    };
            }
            else if (userLimit == 12)
            {
                await ConsoleWriter.Write($"Group object is raid, creating raid", ConsoleWriter.InfoType.Notice);
                group = new Raid() 
                {
                    author = author,
                    users = users,
                    userLimit = userLimit,
                    channel = (ISocketMessageChannel)channel,
                    createdAt = createdAt,
                    destination = destination,
                    targetMessage = (RestUserMessage)targetMessage
                };
            }
            else if (userLimit == 100)
            {
                await ConsoleWriter.Write($"Group object is guild fight, creating guild fight", ConsoleWriter.InfoType.Notice);
                List<SocketUser> noGearUsers = new List<SocketUser>();
                foreach (var userId in noGearUsers_id)
                {
                    noGearUsers.Add(guild.GetUser(userId));
                }

                List<SocketUser> unwillingUsers = new List<SocketUser>();
                foreach (var userId in unwillingUsers_id)
                {
                    unwillingUsers.Add(guild.GetUser(userId));
                }

                List<SocketUser> unsureUsers = new List<SocketUser>();
                foreach (var userId in unsureUsers_id)
                {
                    unsureUsers.Add(guild.GetUser(userId));
                }

                group = new GuildFight()
                {
                    author = author,
                    users = users,
                    noGearUsers = noGearUsers,
                    unwillingUsers = unwillingUsers,
                    unsureUsers = unsureUsers,
                    userLimit = userLimit,
                    channel = (ISocketMessageChannel)channel,
                    createdAt = createdAt,
                    destination = destination,
                    targetMessage = (RestUserMessage)targetMessage
                };
            }
            return group;
        }
    }
}
