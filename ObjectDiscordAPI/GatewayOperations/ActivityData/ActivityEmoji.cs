using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayOperations.ActivityData
{
    public class ActivityEmoji
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public ulong? ID { get; set; }

        [JsonProperty("animated")]
        public bool? IsAnimated { get; set; }

        public ActivityEmoji() { }
    }
}
