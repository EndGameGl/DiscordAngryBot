using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.Bans;
using DiscordAngryBot.CustomObjects.Filters;
using DiscordAngryBot.CustomObjects.Groups;
using System.Collections.Generic;

namespace DiscordAngryBot.CustomObjects.Caches
{
    /// <summary>
    /// Guild cache with all the custom objects
    /// </summary>
    public class CustomGuildDataCache
    {
        /// <summary>
        /// Guild itself
        /// </summary>
        public SocketGuild Guild { get; set; }

        /// <summary>
        /// Guild groups
        /// </summary>
        public List<Group> Groups { get; set; }

        /// <summary>
        /// Guild bans
        /// </summary>
        public List<DiscordBan> Bans { get; set; }

        /// <summary>
        /// Guild swear counters
        /// </summary>
        public List<SwearCounter> SwearCounters { get; set; }

        /// <summary>
        /// Guild settings
        /// </summary>
        public DiscordGuildSettings Settings { get; set; }

        /// <summary>
        /// Whether this guild is available
        /// </summary>
        public bool IsAvailable { get; set; }
    }
}
