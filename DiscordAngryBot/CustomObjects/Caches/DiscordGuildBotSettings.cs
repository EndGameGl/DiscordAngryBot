using System.Collections.Generic;

namespace DiscordAngryBot.CustomObjects.Caches
{
    /// <summary>
    /// Set of settings used in cache
    /// </summary>
    public class DiscordGuildBotSettings
    {
        /// <summary>
        /// Prefix for calling commands of this guild, if any
        /// </summary>
        public char? CommandPrefix { get; private set; } = '_';

        /// <summary>
        /// List of bot admins in this guild
        /// </summary>
        public List<ulong> AdminsID { get; }

        /// <summary>
        /// ID of the ban role, if any
        /// </summary>
        public ulong? BanRoleID { get; private set; } = null;

        /// <summary>
        /// ID of the news channel, if any
        /// </summary>
        public ulong? NewsChannelID { get; private set; } = null;

        /// <summary>
        /// API Token for identifyng external input
        /// </summary>
        public string APIToken { get; } = null;

        /// <summary>
        /// Whether swear filter is enabled in this guild
        /// </summary>
        public bool? IsSwearFilterEnabled { get; private set; } = null;


        /// <summary>
        /// Class constructor
        /// </summary>
        public DiscordGuildBotSettings() 
        {
            AdminsID = new List<ulong>() { 261497385274966026 };
        }

        /// <summary>
        /// Whether this user is admin
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns></returns>
        public bool IsAdmin(ulong id) => AdminsID.Contains(id);
        /// <summary>
        /// Change ID of the ban role
        /// </summary>
        /// <param name="id">New ID</param>
        public void ChangeBanRoleID(ulong id)
        {
            BanRoleID = id;
        }

        public void ChangeCommandPrefix(char newPrefix)
        {
            CommandPrefix = newPrefix;
        }

        public void ChangeNewsChannelID(ulong newID)
        {
            NewsChannelID = newID;
        }

        public void EnableSwearFilter()
        {
            IsSwearFilterEnabled = true;
        }
        public void DisableSwearFilter()
        {
            IsSwearFilterEnabled = false;
        }
    }
}
 