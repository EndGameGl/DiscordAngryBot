using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayOperations
{
    public class IdentityConnectionProperties
    {
        [JsonProperty("$os")]
        public string OS { get; set; }
        [JsonProperty("$browser")]
        public string Browser { get; set; }
        [JsonProperty("$device")]
        public string Device { get; set; }

        [JsonConstructor]
        public IdentityConnectionProperties() { }
    }
}
