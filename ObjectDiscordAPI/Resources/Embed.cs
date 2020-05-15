using Newtonsoft.Json;
using ObjectDiscordAPI.Resources.EmbedResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Resources
{
    public class Embed
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("url")]
        public string URL { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("color")]
        public int? Color { get; set; }

        [JsonProperty("footer")]
        public EmbedFooter EmbedFooter { get; set; }

        [JsonProperty("image")]
        public EmbedImage EmbedImage { get; set; }

        [JsonProperty("thumbnail")]
        public EmbedThumbnail EmbedThumbnail { get; set; }

        [JsonProperty("video")]
        public EmbedVideo EmbedVideo { get; set; }

        [JsonProperty("provider")]
        public EmbedProvider EmbedProvider { get; set; }

        [JsonProperty("author")]
        public EmbedAuthor EmbedAuthor { get; set; }

        [JsonProperty("fields")]
        public EmbedField[] EmbedFields { get; set; }

        [JsonConstructor]
        public Embed() { }
    }
}
