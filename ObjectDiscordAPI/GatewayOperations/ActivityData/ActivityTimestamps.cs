using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayOperations.ActivityData
{
    public class ActivityTimestamps
    {
        [JsonProperty("start")]
        public ulong? ActivityStarted { get; set; }

        [JsonProperty("end")]
        public ulong? ActivityEnded { get; set; }

        [JsonConstructor]
        public ActivityTimestamps() { }
    }
}
