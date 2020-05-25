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
        /// <summary>
        /// The name of the emoji
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// The id of the emoji
        /// </summary>
        [JsonProperty("id")]
        public ulong? ID { get; set; }
        /// <summary>
        /// Whether this emoji is animated
        /// </summary>
        [JsonProperty("animated")]
        public bool? IsAnimated { get; set; }

        public ActivityEmoji() { }
    }
}
