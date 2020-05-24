using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Resources.EmbedResources
{
    public class EmbedProvider
    {
        /// <summary>
        /// Name of provider
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// URL of provider
        /// </summary>
        [JsonProperty("url")]
        public string URL { get; set; }
        [JsonConstructor]
        public EmbedProvider() { }
    }
}
