using Newtonsoft.Json;
using ObjectDiscordAPI.GatewayOperations;

namespace ObjectDiscordAPI.GatewayData.GatewayCommands
{
    public class IdentifyData
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("properties")]
        public IdentityConnectionProperties IdentityConnectionProperties { get; set; }

        [JsonProperty("compress")]
        public bool? IsSendingCompressed { get; set; } = false;

        [JsonProperty("large_threshold")]
        public int? LargeThreshold { get; set; } = 50;

        [JsonProperty("shard")]
        public int[] Shard { get; set; }

        [JsonProperty("presence")]
        public GatewayUpdateStatus UpdatePresenceStatus { get; set; }

        [JsonProperty("guild_subscriptions")]
        public bool DoGuildSubscriptions { get; set; } = true;

        [JsonProperty("intents")]
        public int? Intents { get; set; }

        [JsonConstructor]
        public IdentifyData() { }
    }
}
