using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayOperations
{
    public class Heartbeat
    {
        [JsonProperty("op")]
        public int Operation { get; set; }

        [JsonProperty("d")]
        public int? LastSequence { get; set; }

        [JsonConstructor]
        public Heartbeat() { }
    }
}
