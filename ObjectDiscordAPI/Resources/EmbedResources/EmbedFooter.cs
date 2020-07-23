using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Resources.EmbedResources
{
    public class EmbedFooter
    {
        /// <summary>
        /// Footer text
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }
        /// <summary>
        /// URL of footer icon (only supports http(s) and attachments)
        /// </summary>
        [JsonProperty("icon_url")]
        public string IconURL { get; set; }
        /// <summary>
        /// A proxied url of footer icon
        /// </summary>
        [JsonProperty("proxy_icon_url")]
        public string ProxyIconURL { get; set; }

        [JsonConstructor]
        public EmbedFooter() { }
    }
}
