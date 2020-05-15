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
        [JsonProperty("url")]
        public string URL { get; set; }

        [JsonProperty("proxy_url")]
        public string ProxyURL { get; set; }

        [JsonProperty("height")]
        public int? Height { get; set; }

        [JsonProperty("width")]
        public int? Width { get; set; }

        [JsonConstructor]
        public EmbedThumbnail() { }
    }
}
