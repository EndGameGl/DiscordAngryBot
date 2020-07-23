using Newtonsoft.Json;
using ObjectDiscordAPI.Resources.GuildResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Resources
{
    public class VoiceState
    {
        [JsonProperty("guild_id")]
        public ulong? GuildID { get; set; }

        [JsonProperty("channel_id")]
        public ulong? ChannelID { get; set; }

        [JsonProperty("user_id")]
        public ulong UserID { get; set; }

        [JsonProperty("member")]
        public GuildMember Member { get; set; }

        [JsonProperty("session_id")]
        public string SessionID { get; set; }

        [JsonProperty("deaf")]
        public bool IsDeaf { get; set; }

        [JsonProperty("mute")]
        public bool IsMuted { get; set; }

        [JsonProperty("self_deaf")]
        public bool SelfDeaf { get; set; }

        [JsonProperty("self_mute")]
        public bool SelfMuted { get; set; }

        [JsonProperty("self_stream")]
        public bool? SelfSteaming { get; set; }

        [JsonProperty("supress")]
        public bool? IsSupressed { get; set; }

        [JsonConstructor]
        public VoiceState() { }
    }
}
