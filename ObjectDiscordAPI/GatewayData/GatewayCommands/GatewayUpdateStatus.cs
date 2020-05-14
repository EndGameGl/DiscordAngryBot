using Newtonsoft.Json;
using ObjectDiscordAPI.GatewayOperations;

namespace ObjectDiscordAPI.GatewayData.GatewayCommands
{
    public class GatewayUpdateStatus
    {
        [JsonProperty("since")]
        public int? Since { get; set; }

        [JsonProperty("game")]
        public Activity Game { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("afk")]
        public bool AFK { get; set; }

        [JsonConstructor]
        public GatewayUpdateStatus() { }
    }
}
