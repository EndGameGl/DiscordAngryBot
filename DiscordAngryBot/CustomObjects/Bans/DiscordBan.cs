using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Bans
{
    public class DiscordBan
    {
        public ISocketMessageChannel channel { get; set; }
        public SocketGuildUser banTarget { get; set; }
        public SocketRole roleToBan { get; set; }
        public Timer timer { get; set; }
        public int? length { get; set; }
        public bool isInfinite { get; set; }
        public string GUID { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? endsAt { get; set; }
    }
}
