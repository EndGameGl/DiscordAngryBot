using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayData
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
