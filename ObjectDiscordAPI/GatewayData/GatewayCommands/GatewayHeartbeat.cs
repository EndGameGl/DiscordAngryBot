using Newtonsoft.Json;

namespace ObjectDiscordAPI.GatewayData.GatewayCommands
{
    public class GatewayHeartbeat
    {
        [JsonProperty("op")]
        public int Operation { get; set; }

        [JsonProperty("d")]
        public int? LastSequence { get; set; }

        [JsonConstructor]
        public GatewayHeartbeat() { }
    }
}
