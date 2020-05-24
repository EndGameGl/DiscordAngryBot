using Newtonsoft.Json;
using ObjectDiscordAPI.Resources;
using ObjectDiscordAPI.Resources.GuildResources;
using System;
using System.Collections.Generic;

namespace ObjectDiscordAPI.GatewayData.GatewayEvents
{
    public class GatewayEventGuildCreateArgs : Guild
    {
        /// <summary>
        /// When this guild was joined at
        /// </summary>
        [JsonProperty("joined_at")]
        public DateTime? JoinedAt { get; set; }

        /// <summary>
        /// True if this is considered a large guild
        /// </summary>
        [JsonProperty("large")]
        public bool? IsLarge { get; set; }

        /// <summary>
        /// True if this guild is unavailable due to an outage
        /// </summary>
        [JsonProperty("unavailable")]
        public bool? IsUnavailable { get; set; }

        /// <summary>
        /// Total number of members in this guild
        /// </summary>
        [JsonProperty("member_count")]
        public int MemberCount { get; set; }

        /// <summary>
        /// States of members currently in voice channels; lacks the guild_id key
        /// </summary>
        [JsonProperty("voice_states")]
        public List<VoiceState> VoiceState { get; set; }

        /// <summary>
        /// Users in the guild
        /// </summary>
        [JsonProperty("members")]
        public List<GuildMember> Members { get; set; }

        /// <summary>
        /// Channels in the guild
        /// </summary>
        [JsonProperty("channels")]
        public List<Channel> Channels { get; set; }

        /// <summary>
        /// Presences of the members in the guild, will only include non-offline members if the size is greater than large threshold
        /// </summary>
        [JsonProperty("presences")]
        public List<PresenceUpdate> Presences { get; set; }

        [JsonConstructor]
        public GatewayEventGuildCreateArgs() { }
    }
}
