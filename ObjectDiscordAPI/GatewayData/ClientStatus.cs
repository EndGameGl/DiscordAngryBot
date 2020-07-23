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
        /// <summary>
        /// The user's status set for an active desktop (Windows, Linux, Mac) application session
        /// </summary>
        [JsonProperty("desktop")]
        public string Desktop { get; set; }
        /// <summary>
        /// The user's status set for an active mobile (iOS, Android) application session
        /// </summary>
        [JsonProperty("mobile")]
        public string Mobile { get; set; }
        /// <summary>
        /// The user's status set for an active web (browser, bot account) application session
        /// </summary>
        [JsonProperty("web")]
        public string Web { get; set; }

        [JsonConstructor]
        public ClientStatus() { }
    }
}
