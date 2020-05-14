using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayOperations
{
    public class UnavailableGuild
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("unavailable")]
        public bool Unavailable { get; set; }

        [JsonConstructor]
        public UnavailableGuild() { }
    }
}
