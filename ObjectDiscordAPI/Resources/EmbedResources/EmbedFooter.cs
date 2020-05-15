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
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("icon_url")]
        public string IconURL { get; set; }

        [JsonProperty("proxy_icon_url")]
        public string ProxyIconURL { get; set; }

        [JsonConstructor]
        public EmbedFooter() { }
    }
}
