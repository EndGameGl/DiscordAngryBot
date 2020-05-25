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
        /// <summary>
        /// Unix time (in milliseconds) of when the activity started
        /// </summary>
        [JsonProperty("start")]
        public ulong? ActivityStarted { get; set; }
        /// <summary>
        /// Unix time (in milliseconds) of when the activity ends
        /// </summary>
        [JsonProperty("end")]
        public ulong? ActivityEnded { get; set; }

        [JsonConstructor]
        public ActivityTimestamps() { }
    }
}
