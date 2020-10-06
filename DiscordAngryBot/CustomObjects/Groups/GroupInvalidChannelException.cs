using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Groups
{
    class GroupInvalidChannelException : Exception
    {
        public ulong ChannelID { get; }
        public string GroupGUID { get; }
        public GroupInvalidChannelException(string GUID, ulong channelID) : base(string.Format("Invalid group channel {0} : {1}", GUID, channelID))
        {
            GroupGUID = GUID;
            ChannelID = channelID;
        }
    }
}
