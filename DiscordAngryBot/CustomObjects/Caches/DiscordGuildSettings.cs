using System.Collections.Generic;

namespace DiscordAngryBot.CustomObjects.Caches
{
    public class DiscordGuildSettings
    {
        public char? CommandPrefix { get; set; }
        public List<ulong> adminsID { get; set; }
        public ulong? BanRoleID { get; set; }
        public ulong? NewsChannelID { get; set; }
        public string APIToken { get; set; }
        public bool? IsSwearFilterEnabled { get; set; }
    }
}
 