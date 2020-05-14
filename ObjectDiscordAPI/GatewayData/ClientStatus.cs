using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayData
{
    public class ClientStatus
    {
        [JsonProperty("desktop")]
        public string Desktop { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("web")]
        public string Web { get; set; }

        [JsonConstructor]
        public ClientStatus() { }
    }
}
