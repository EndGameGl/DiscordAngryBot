using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayOperations
{
    public class OperationHello
    {
        [JsonProperty("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }

        [JsonConstructor]
        public OperationHello() { }
    }
}
