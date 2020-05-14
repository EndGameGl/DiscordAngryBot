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
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("size")]
        public int[] Size { get; set; }

        [JsonConstructor]
        public ActivityParty() { }
    }
}
