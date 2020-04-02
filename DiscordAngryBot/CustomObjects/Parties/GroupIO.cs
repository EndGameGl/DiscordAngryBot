using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Parties
{
    public class GroupIO
    {
        public List<ulong> userIDs { get; set; }
        public int userLimit { get; set; }
        public DateTimeOffset sourceMessage_CreatedAt { get; set; }
        public ulong sourceMessage_AuthorID{ get; set; }
        public DateTime created { get; set; }
        public string partyDestination { get; set; }
        public ulong targetMessage_ID { get; set; }
        public ulong targetMessage_ChannelID { get; set; }
        public GroupIO() { }
        public GroupIO(IGroup group)
        {
            userIDs = new List<ulong>();
            foreach (var user in group.users)
            {
                userIDs.Add(user.Id);
            }
            userLimit = group.userLimit;
            if (group.isLoadedFromFile == true)
            {
                sourceMessage_CreatedAt = group.loadedInfo.sourceMessage_CreatedAt;
                sourceMessage_AuthorID = group.loadedInfo.sourceMessage_AuthorID;
            }
            else
            {
                sourceMessage_CreatedAt = group.sourceMessage.CreatedAt;
                sourceMessage_AuthorID = group.sourceMessage.Author.Id;
            }
            created = group.created;
            partyDestination = group.partyDestination;
            targetMessage_ID = group.targetMessage.Id;
            targetMessage_ChannelID = group.targetMessage.Channel.Id;
        }

    }
}
