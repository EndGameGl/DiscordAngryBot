using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayOperations.ActivityData
{
    public class ActivityParty
    {
        /// <summary>
        /// The id of the party
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }
        /// <summary>
        /// Used to show the party's current and maximum size
        /// </summary>
        [JsonProperty("size")]
        public int[] Size { get; set; }

        [JsonConstructor]
        public ActivityParty() { }
    }
}
