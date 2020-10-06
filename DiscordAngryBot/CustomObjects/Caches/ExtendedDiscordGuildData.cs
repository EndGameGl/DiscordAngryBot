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
    public class ExtendedDiscordGuildData
    {
        /// <summary>
        /// Guild itself
        /// </summary>
        public SocketGuild Guild { get; }

        /// <summary>
        /// Guild groups
        /// </summary>
        public List<Group> Groups { get; private set; }

        /// <summary>
        /// Guild bans
        /// </summary>
        public List<DiscordBan> Bans { get; private set; }

        /// <summary>
        /// Guild swear counters
        /// </summary>
        public List<SwearCounter> SwearCounters { get; }

        /// <summary>
        /// Guild settings
        /// </summary>
        public DiscordGuildBotSettings Settings { get; private set; }

        /// <summary>
        /// Whether this guild is available
        /// </summary>
        public bool IsAvailable { get; private set; }

        /// <summary>
        /// Default class constructor
        /// </summary>
        /// <param name="guild">Guild to be extended</param>
        /// <param name="isAvailable">Whether it is available</param>
        public ExtendedDiscordGuildData(SocketGuild guild, bool isAvailable)
        {
            Guild = guild;
            Groups = new List<Group>();
            Bans = new List<DiscordBan>();
            SwearCounters = new List<SwearCounter>();
            Settings = new DiscordGuildBotSettings();
            IsAvailable = isAvailable;
        }     

        /// <summary>
        /// Apply different set of settings to this guild
        /// </summary>
        /// <param name="settings">New settings</param>
        public void UseSettings(DiscordGuildBotSettings settings)
        {
            Settings = settings;
        }
        /// <summary>
        /// Apply different set of bans to this guild
        /// </summary>
        /// <param name="discordBans">List of bans</param>
        public void UseExistingListOfBans(List<DiscordBan> discordBans)
        {
            Bans = discordBans;
        }
        /// <summary>
        /// Apply different set of groups to this guild
        /// </summary>
        /// <param name="groups">List of groups</param>
        public void UseExistingListOfGroups(List<Group> groups)
        {
            Groups = groups;
        }

        public void SetUnavailable()
        {
            IsAvailable = false;
        }
        public void SetAvailable()
        {
            IsAvailable = true;
        }
    }
}
