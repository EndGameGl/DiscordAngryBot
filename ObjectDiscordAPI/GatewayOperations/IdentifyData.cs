using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayOperations
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
        public UpdateStatus UpdatePresenceStatus { get; set; }

        [JsonProperty("guild_subscriptions")]
        public bool DoGuildSubscriptions { get; set; } = true;

        [JsonProperty("intents")]
        public int Intents { get; set; } = 1 << 14;

        [JsonConstructor]
        public IdentifyData() { }
    }
}
