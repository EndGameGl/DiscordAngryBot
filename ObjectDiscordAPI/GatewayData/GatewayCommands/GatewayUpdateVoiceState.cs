using Newtonsoft.Json;

namespace ObjectDiscordAPI.GatewayData.GatewayCommands
{
    public class GatewayUpdateVoiceState
    {
        [JsonProperty("guild_id")]
        public ulong GuildID { get; set; }

        [JsonProperty("channel_id")]
        public ulong? ChannelID { get; set; }

        [JsonProperty("self_mute")]
        public bool IsMuted { get; set; }

        [JsonProperty("self_deaf")]
        public bool IsDeaf { get; set; }

        [JsonConstructor]
        public GatewayUpdateVoiceState() { }
    }
}
