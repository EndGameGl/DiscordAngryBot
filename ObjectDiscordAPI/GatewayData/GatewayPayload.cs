using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayData
{
    public class GatewayPayload
    {
        [JsonProperty("op")]
        public int OperationCode { get; set; }

        [JsonProperty("d")]
        public object JSONEventData { get; set; }

        [JsonProperty("s")]
        public int? SequenceNumber { get; set; }

        [JsonProperty("t")]
        public string EventName { get; set; }

        [JsonConstructor]
        public GatewayPayload() { }
    }
}
