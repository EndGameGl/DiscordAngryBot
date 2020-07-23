using Newtonsoft.Json;

namespace ObjectDiscordAPI.GatewayData
{
    public class SessionStartLimit
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("remaining")]
        public int Remaining { get; set; }

        [JsonProperty("reset_after")]
        public int ResetAfter { get; set; }

        [JsonConstructor]
        public SessionStartLimit() { }
    }
}
