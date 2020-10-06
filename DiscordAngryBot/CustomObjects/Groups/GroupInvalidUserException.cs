using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Groups
{
    class GroupInvalidUserException : Exception
    {
        public ulong UserID { get; }
        public string GroupGUID { get; }
        public GroupInvalidUserException(string GUID, ulong userID) : base(string.Format("Invalid user in group {0} : {1}", GUID, userID))
        {
            GroupGUID = GUID;
            UserID = userID;
        }
    }
}
