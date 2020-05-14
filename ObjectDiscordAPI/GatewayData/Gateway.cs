using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayData
{
    public class Gateway
    {
        [JsonProperty("url")]
        public string URL { get; set; }

        [JsonProperty("shards")]
        public int Shards { get; set; }

        [JsonProperty("session_start_limit")]
        public SessionStartLimit SessionStartLimit { get; set; }

        [JsonConstructor]
        public Gateway() { }
    }
}
