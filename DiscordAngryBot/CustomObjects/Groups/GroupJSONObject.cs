using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Groups
{
    public class GroupJSONObject
    {
        public ulong author_id { get; set; }
        public ulong channel_id { get; set; }
        public List<ulong> users_id { get; set; } = new List<ulong>();
        public int userLimit { get; set; }
        public ulong targetMessage_id { get; set; }
        public string destination { get; set; }
        public DateTime createdAt { get; set; }
        public GroupJSONObject() { }
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
