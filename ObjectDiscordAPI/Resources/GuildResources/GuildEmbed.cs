using Newtonsoft.Json;

namespace ObjectDiscordAPI.Resources.GuildResources
{
    public class GuildEmbed
    {
        [JsonProperty("enabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("channel_id")]
        public ulong? ChannelID { get; set; }

        [JsonConstructor]
        public GuildEmbed() { }
    }
}
