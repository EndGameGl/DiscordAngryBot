using Discord;
using Discord.Rest;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects;
using DiscordAngryBot.CustomObjects.Groups;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordAngryBot.Models
{
    /// <summary>
    /// Референс для объекта Group
    /// </summary>
    public class GroupReference : ILoadableInto<Group>
    {
        /// <summary>
        /// Идентификатор группы
        /// </summary>
        [JsonProperty("GUID")]
        public string GUID { get; set; }
        /// <summary>
        /// Идентификатор автора
        /// </summary>
        [JsonProperty("authorID")]
        public ulong authorID { get; set; }
        /// <summary>
        /// Идентификатор сервера дискорда
        /// </summary>
        [JsonProperty("serverID")]
        public ulong serverID { get; set; }
        /// <summary>
        /// Идентификатор канала дискорда
        /// </summary>
        [JsonProperty("channelID")]
        public ulong channelID { get; set; }
        /// <summary>
        /// Идентификатор сообщения, представляющего группу
        /// </summary>
        [JsonProperty("targetMessageID")]
        public ulong targetMessageID { get; set; }
        /// <summary>
        /// Дата создания группы
        /// </summary>
        [JsonProperty("createdAt")]
        public DateTime createdAt { get; set; }
        /// <summary>
        /// Цель создания группы
        /// </summary>
        [JsonProperty("destination")]
        public string destination { get; set; }
        /// <summary>
        /// Тип группы
        /// </summary>
        [JsonProperty("groupType")]
        public GroupType groupType { get; set; }
        /// <summary>
        /// Референс на списки в группе
        /// </summary>
        [JsonProperty("userListReferences")]
        public List<UserListReference> userListReferences { get; set; }
        /// <summary>
        /// Тип битвы БШ (Если такой есть)
        /// </summary>
        [JsonProperty("guildFightType")]
        public GuildFightType? guildFightType { get; set; }
        [JsonConstructor]
        public GroupReference() { }
        /// <summary>
        /// Конструктор референса группы
        /// </summary>
        /// <param name="group"></param>
        public GroupReference(Group group)
        {
            GUID = group.GUID;
            authorID = group.Author.Id;
            serverID = group.Channel.Guild.Id;
            channelID = group.Channel.Id;
            targetMessageID = group.TargetMessage.Id;
            createdAt = group.CreatedAt;
            destination = group.Destination;
            groupType = group.Type;
            userListReferences = new List<UserListReference>();
            foreach (var userList in group.UserLists)
            {
                userListReferences.Add(new UserListReference(userList));
            }
            if (groupType == GroupType.GuildFight)
            {
                guildFightType = ((GuildFight)group).GuildFightType;
            }
        }
        public Group LoadOrigin()
        {
            Group loadedGroup = null;
            if (BotCore.TryGetGuildDataCache(serverID, out var customGuildDataCache))
            {
                var guild = customGuildDataCache.Guild;
                var channel = guild.GetTextChannel(channelID);
                var message = (RestUserMessage)channel.GetMessageAsync(targetMessageID).GetAwaiter().GetResult();
                var author = guild.GetUser(authorID);
                List<UserList> userLists = new List<UserList>();
                foreach (var listReference in userListReferences)
                {
                    List<SocketGuildUser> users = new List<SocketGuildUser>();
                    foreach (var id in listReference.UserIDs)
                    {
                        users.Add(guild.GetUser(id));
                    }
                    userLists.Add(new UserList()
                    {
                        ListName = listReference.ListName,
                        ListEmoji = new Emoji(listReference.Emoji),
                        UserLimit = listReference.UserLimit,
                        Users = users
                    });
                }
                loadedGroup = new Group()
                {
                    GUID = this.GUID,
                    Author = author,
                    Channel = channel,
                    TargetMessage = message,
                    CreatedAt = createdAt,
                    Destination = destination,
                    Type = groupType,
                    UserLists = userLists
                };
                if (guildFightType != null)
                {
                    (loadedGroup as GuildFight).GuildFightType = guildFightType.Value;
                }
            }
            return loadedGroup;
        }
    }
}
