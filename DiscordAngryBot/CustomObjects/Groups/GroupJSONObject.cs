using Discord.Rest;
using Discord.WebSocket;
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
            Group group = null;
            var author = guild.GetUser(author_id);
            List<SocketUser> users = new List<SocketUser>();
            var channel = guild.GetChannel(channel_id);
            var targetMessage = await ((ISocketMessageChannel)channel).GetMessageAsync(targetMessage_id);
            foreach (var userId in users_id)
            {
                users.Add(guild.GetUser(userId));
            }
            if (userLimit == 6)
            {
                group = new Party()
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
            else if (userLimit == 12)
            {
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
            return group;
        }
    }
}
