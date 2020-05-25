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
        /// <summary>
        /// The id for a large asset of the activity, usually a snowflake
        /// </summary>
        [JsonProperty("large_image")]
        public string LargeImage { get; set; }
        /// <summary>
        /// Text displayed when hovering over the large image of the activity
        /// </summary>
        [JsonProperty("large_text")]
        public string LargeText { get; set; }
        /// <summary>
        /// The id for a small asset of the activity, usually a snowflake
        /// </summary>
        [JsonProperty("small_image")]
        public string SmallImage { get; set; }
        /// <summary>
        /// Text displayed when hovering over the small image of the activity
        /// </summary>
        [JsonProperty("small_text")]
        public string SmallText { get; set; }

        [JsonConstructor]
        public ActivityAssets() { }
    }
}
