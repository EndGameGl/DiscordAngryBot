using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.DiscordCommands
{
    class DiscordCommandComparer : IEqualityComparer<DiscordCommand>
    {
        public bool Equals(DiscordCommand x, DiscordCommand y)
        {
            return x.CommandMetadata.CommandName.Equals(y.CommandMetadata.CommandName);
        }

        public int GetHashCode(DiscordCommand obj)
        {
            return obj.CommandMetadata.CommandName.GetHashCode();
        }
    }
}
