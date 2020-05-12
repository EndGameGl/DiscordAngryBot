using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Filters
{
    public class SwearCounter
    {
        public SocketUser author { get; set; }
        public List<SocketMessage> reasons { get; set; }
    }
}
