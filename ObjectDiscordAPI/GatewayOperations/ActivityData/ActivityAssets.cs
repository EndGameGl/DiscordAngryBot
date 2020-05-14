using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayOperations.ActivityData
{
    public class ActivityAssets
    {
        [JsonProperty("large_image")]
        public string LargeImage { get; set; }

        [JsonProperty("large_text")]
        public string LargeText { get; set; }

        [JsonProperty("small_image")]
        public string SmallImage { get; set; }

        [JsonProperty("small_text")]
        public string SmallText { get; set; }

        [JsonConstructor]
        public ActivityAssets() { }
    }
}
