using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Caches
{
    public class DiscordGuildSettings
    {
        public char? CommandPrefix { get; set; }
        public List<ulong> adminsID { get; set; }
        public ulong? BanRoleID { get; set; }
        public ulong? NewsChannelID { get; set; }
        public string APIToken { get; set; }
    }
}
 