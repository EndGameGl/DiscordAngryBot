using Discord;
using Discord.Rest;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects;
using DiscordAngryBot.CustomObjects.Groups;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DiscordAngryBot.Models
{
    /// <summary>
    /// Refence object for Group
    /// </summary>
    public class GroupReference : ILoadableInto<Group>
    {
        /// <summary>
        /// Group GUID
        /// </summary>
        [JsonProperty("GUID")]
        public string GUID { get; set; }

        /// <summary>
        /// Group author ID
        /// </summary>
        [JsonProperty("authorID")]
        public ulong authorID { get; set; }

        /// <summary>
        /// Group server ID
        /// </summary>
        [JsonProperty("serverID")]
        public ulong serverID { get; set; }

        /// <summary>
        /// Group channel ID
        /// </summary>
        [JsonProperty("channelID")]
        public ulong channelID { get; set; }

        /// <summary>
        /// Target message ID
        /// </summary>
        [JsonProperty("targetMessageID")]
        public ulong targetMessageID { get; set; }

        /// <summary>
        /// Group creation date
        /// </summary>
        [JsonProperty("createdAt")]
        public DateTime createdAt { get; set; }

        /// <summary>
        /// Group goal
        /// </summary>
        [JsonProperty("destination")]
        public string destination { get; set; }

        /// <summary>
        /// Group type
        /// </summary>
        [JsonProperty("groupType")]
        public GroupType groupType { get; set; }

        /// <summary>
        /// User lists references
        /// </summary>
        [JsonProperty("userListReferences")]
        public List<UserListReference> userListReferences { get; set; }

        /// <summary>
        /// Guild fight type, if any
        /// </summary>
        [JsonProperty("guildFightType")]
        public GuildFightType? guildFightType { get; set; }

        /// <summary>
        /// Class constructor
        /// </summary>
        [JsonConstructor]
        public GroupReference() { }

        /// <summary>
        /// Second class constructor
        /// </summary>
        /// <param name="group">Group object</param>
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

        /// <summary>
        /// Loads into origin object
        /// </summary>
        /// <returns></returns>
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
