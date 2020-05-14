using Newtonsoft.Json;
using ObjectDiscordAPI.Resources;
using ObjectDiscordAPI.Resources.GuildResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayData.GatewayEvents
{
    public class GatewayEventGuildCreateArgs : Guild
    {
        [JsonProperty("joined_at")]
        public DateTime? JoinedAt { get; set; }

        [JsonProperty("large")]
        public bool? IsLarge { get; set; }

        [JsonProperty("unavailable")]
        public bool? IsUnavailable { get; set; }

        [JsonProperty("member_count")]
        public int MemberCount { get; set; }

        [JsonProperty("voice_states")]
        public VoiceState[] VoiceState { get; set; }

        [JsonProperty("members")]
        public GuildMember[] Members { get; set; }

        [JsonProperty("channels")]
        public Channel[] Channels { get; set; }

        [JsonProperty("presences")]
        public PresenceUpdate[] Presences { get; set; }

        [JsonConstructor]
        public GatewayEventGuildCreateArgs() { }
    }
}
