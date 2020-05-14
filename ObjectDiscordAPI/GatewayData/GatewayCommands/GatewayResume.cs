using Newtonsoft.Json;

namespace ObjectDiscordAPI.GatewayData.GatewayCommands
{
    public class GatewayResume
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("session_id")]
        public string SessionID { get; set; }

        [JsonProperty("seq")]
        public int? SequenceNumber { get; set; }

        [JsonConstructor]
        public GatewayResume() { }
    }
}
