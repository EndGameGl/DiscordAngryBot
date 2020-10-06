using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Groups
{
    public class GroupInvalidTargetException : Exception
    {
        public ulong MessageID { get; }
        public string GroupGUID { get; }
        public GroupInvalidTargetException(string GUID, ulong messageID) : base(string.Format("Invalid message ID for group {0} : {1}", GUID, messageID))
        {
            GroupGUID = GUID;
            MessageID = messageID;
        }
    }
}
