using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Resources.EmbedResources
{
    public class EmbedVideo
    {
        /// <summary>
        /// Source url of video
        /// </summary>
        [JsonProperty("url")]
        public string URL { get; set; }
        /// <summary>
        /// Height of video
        /// </summary>
        [JsonProperty("height")]
        public int? Height { get; set; }
        /// <summary>
        /// Width of video
        /// </summary>
        [JsonProperty("width")]
        public int? Width { get; set; }

        [JsonConstructor]
        public EmbedVideo() { }
    }
}
