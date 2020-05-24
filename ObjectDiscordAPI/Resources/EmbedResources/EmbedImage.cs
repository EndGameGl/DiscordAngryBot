using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Resources.EmbedResources
{
    public class EmbedImage
    {
        /// <summary>
        /// Source URL of image (only supports http(s) and attachments)
        /// </summary>
        [JsonProperty("url")]
        public string URL { get; set; }
        /// <summary>
        /// A proxied URL of the image
        /// </summary>
        [JsonProperty("proxy_url")]
        public string ProxyURL { get; set; }
        /// <summary>
        /// Height of image
        /// </summary>
        [JsonProperty("height")]
        public int? Height { get; set; }
        /// <summary>
        /// Width of image
        /// </summary>
        [JsonProperty("width")]
        public int? Width { get; set; }

        [JsonConstructor]
        public EmbedImage() { }
    }
}
