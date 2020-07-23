using Newtonsoft.Json;


namespace ObjectDiscordAPI.GatewayData.GatewayEvents
{
    public class GatewayEventHelloArgs
    {
        [JsonProperty("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }

        [JsonConstructor]
        public GatewayEventHelloArgs() { }
    }
}
