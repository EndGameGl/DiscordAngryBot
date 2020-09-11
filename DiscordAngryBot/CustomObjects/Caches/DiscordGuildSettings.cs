using System.Collections.Generic;

namespace DiscordAngryBot.CustomObjects.Caches
{
    /// <summary>
    /// Set of settings used in cache
    /// </summary>
    public class DiscordGuildSettings
    {
        /// <summary>
        /// Prefix for calling commands of this guild, if any
        /// </summary>
        public char? CommandPrefix { get; set; }

        /// <summary>
        /// List of bot admins in this guild
        /// </summary>
        public List<ulong> AdminsID { get; set; }

        /// <summary>
        /// ID of the ban role, if any
        /// </summary>
        public ulong? BanRoleID { get; set; }

        /// <summary>
        /// ID of the news channel, if any
        /// </summary>
        public ulong? NewsChannelID { get; set; }

        /// <summary>
        /// API Token for identifyng external input
        /// </summary>
        public string APIToken { get; set; }

        /// <summary>
        /// Whether swear filter is enabled in this guild
        /// </summary>
        public bool? IsSwearFilterEnabled { get; set; }

        /// <summary>
        /// Class constructor
        /// </summary>
        public DiscordGuildSettings() { }

        /// <summary>
        /// Whether this user is admin
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns></returns>
        public bool IsAdmin(ulong id) => AdminsID.Contains(id);

    }
}
 