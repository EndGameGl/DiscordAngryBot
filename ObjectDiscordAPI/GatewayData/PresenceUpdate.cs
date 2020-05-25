using Newtonsoft.Json;
using ObjectDiscordAPI.GatewayOperations;
using ObjectDiscordAPI.Resources;
using System;

namespace ObjectDiscordAPI.GatewayData
{
    /// <summary>
    /// A user's presence is their current state on a guild. This event is sent when a user's presence or info, such as name or avatar, is updated.
    /// </summary>
    public class PresenceUpdate
    {
        /// <summary>
        /// The user presence is being updated for
        /// </summary>
        [JsonProperty("user")]
        public User User { get; set; }
        /// <summary>
        /// Roles this user is in
        /// </summary>
        [JsonProperty("roles")]
        public ulong[] Roles { get; set; }
        /// <summary>
        /// Null, or the user's current activity
        /// </summary>
        [JsonProperty("game")]
        public Activity Game { get; set; }
        /// <summary>
        /// ID of the guild
        /// </summary>
        [JsonProperty("guild_id")]
        public ulong GuildID { get; set; }
        /// <summary>
        /// Either "idle", "dnd", "online", or "offline"
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }
        /// <summary>
        /// User's current activities
        /// </summary>
        [JsonProperty("activities")]
        public Activity[] Activities { get; set; }
        /// <summary>
        /// User's platform-dependent status
        /// </summary>
        [JsonProperty("client_status")]
        public ClientStatus ClientStatus { get; set; }
        /// <summary>
        /// When the user started boosting the guild
        /// </summary>
        [JsonProperty("premium_since")]
        public DateTime? PremiumSince { get; set; }
        /// <summary>
        /// This users guild nickname (if one is set)
        /// </summary>
        [JsonProperty("nick")]
        public string Nickname { get; set; }

        [JsonConstructor]
        public PresenceUpdate() { }
    }
}
