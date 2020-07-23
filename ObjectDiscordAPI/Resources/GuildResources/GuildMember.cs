using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ObjectDiscordAPI.Resources.GuildResources
{
    /// <summary>
    /// Guild Member Object
    /// </summary>
    public class GuildMember
    {
        /// <summary>
        /// The user this guild member represents
        /// </summary>
        [JsonProperty("user")]
        public User User { get; set; }
        /// <summary>
        /// This users guild nickname
        /// </summary>
        [JsonProperty("nick")]
        public string Nickname { get; set; }
        /// <summary>
        /// Array of role object ids
        /// </summary>
        [JsonProperty("roles")]
        public List<ulong> RolesIDs { get; set; }
        /// <summary>
        /// When the user joined the guild
        /// </summary>
        [JsonProperty("joined_at")]
        public DateTime JoinedAt { get; set; }
        /// <summary>
        /// When the user started boosting the guild
        /// </summary>
        [JsonProperty("premium_since")]
        public DateTime? PremiumSince { get; set; }
        /// <summary>
        /// Whether the user is deafened in voice channels
        /// </summary>
        [JsonProperty("deaf")]
        public bool IsDeaf { get; set; }
        /// <summary>
        /// Whether the user is muted in voice channels
        /// </summary>
        [JsonProperty("mute")]
        public bool IsMuted { get; set; }

        [JsonConstructor]
        public GuildMember() { }
    }
}
