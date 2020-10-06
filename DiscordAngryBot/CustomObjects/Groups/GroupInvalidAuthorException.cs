using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Groups
{
    class GroupInvalidAuthorException : Exception
    {
        public ulong ChannelID { get; }
        public ulong UserID { get; }
        public ulong GroupMessageID { get; }
        public string GroupGUID { get; }
        public GroupInvalidAuthorException(string GUID, ulong userID, ulong messageID, ulong channelID) : base(string.Format("Invalid author in group {0} : {1}", GUID, userID))
        {
            GroupGUID = GUID;
            UserID = userID;
            GroupMessageID = messageID;
            ChannelID = channelID;
        }
    }
}
