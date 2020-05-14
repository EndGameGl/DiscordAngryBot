using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayOperations
{
    public class UpdateStatus
    {
        [JsonProperty("since")]
        public int? Since { get; set; }

        [JsonProperty("game")]
        public Activity Game { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("afk")]
        public bool AFK { get; set; }

        [JsonConstructor]
        public UpdateStatus() { }
    }
}
