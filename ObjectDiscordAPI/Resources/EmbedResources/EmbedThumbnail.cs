using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Resources.EmbedResources
{
    public class EmbedThumbnail
    {
        /// <summary>
        /// Source url of thumbnail (only supports http(s) and attachments)
        /// </summary>
        [JsonProperty("url")]
        public string URL { get; set; }
        /// <summary>
        /// A proxied url of the thumbnail
        /// </summary>
        [JsonProperty("proxy_url")]
        public string ProxyURL { get; set; }
        /// <summary>
        /// Height of thumbnail
        /// </summary>
        [JsonProperty("height")]
        public int? Height { get; set; }
        /// <summary>
        /// Width of thumbnail
        /// </summary>
        [JsonProperty("width")]
        public int? Width { get; set; }

        [JsonConstructor]
        public EmbedThumbnail() { }
    }
}
