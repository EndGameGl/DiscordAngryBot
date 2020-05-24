using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Resources.EmbedResources
{
    public class EmbedAuthor
    {
        /// <summary>
        /// Name of author
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// url of author
        /// </summary>
        [JsonProperty("url")]
        public string URL { get; set; }
        /// <summary>
        /// url of author icon (only supports http(s) and attachments)
        /// </summary>
        [JsonProperty("icon_url")]
        public string IconURL { get; set; }
        /// <summary>
        /// A proxied url of author icon
        /// </summary>
        [JsonProperty("proxy_icon_url")]
        public string ProxyIconURL { get; set; }

        [JsonConstructor]
        public EmbedAuthor() { }
    }
}
