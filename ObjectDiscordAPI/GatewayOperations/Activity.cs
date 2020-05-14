using Newtonsoft.Json;
using ObjectDiscordAPI.GatewayOperations.ActivityData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayOperations
{
    public enum ActivityType
    {
        Game,
        Streaming,
        Listening,
        Custom
    }
    public class Activity
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public ActivityType ActivityType { get; set; }

        [JsonProperty("url")]
        public string StreamURL { get; set; }

        [JsonProperty("created_at")]
        public int CreatedAt { get; set; }

        [JsonProperty("timestamps")]
        public ActivityTimestamps ActivityTimestamp { get; set; }

        [JsonProperty("application_id")]
        public ulong ApplicationID { get; set; }

        [JsonProperty("details")]
        public string Details { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("emoji")]
        public ActivityEmoji ActivityEmoji { get; set; }

        [JsonProperty("party")]
        public ActivityParty ActivityParty { get; set; }

        [JsonProperty("assets")]
        public ActivityAssets ActivityAssets { get; set; }

        [JsonProperty("secrets")]
        public ActivitySecrets ActivitySecrets { get; set; }

        [JsonProperty("instance")]
        public bool? IsInstanced { get; set; }

        [JsonProperty("flags")]
        public int? Flags { get; set; }

        [JsonConstructor]
        public Activity() { }
    }
}
