using Newtonsoft.Json;
using ObjectDiscordAPI.Resources.EmbedResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Resources
{
    /// <summary>
    /// Embed Object
    /// </summary>
    public class Embed
    {
        /// <summary>
        /// Title of embed
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
        /// <summary>
        /// Type of embed (always "rich" for webhook embeds)
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
        /// <summary>
        /// Description of embed
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// URL of embed
        /// </summary>
        [JsonProperty("url")]
        public string URL { get; set; }
        /// <summary>
        /// Timestamp of embed content
        /// </summary>
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Color code of the embed
        /// </summary>
        [JsonProperty("color")]
        public int? Color { get; set; }
        /// <summary>
        /// Footer information
        /// </summary>
        [JsonProperty("footer")]
        public EmbedFooter EmbedFooter { get; set; }
        /// <summary>
        /// Image information
        /// </summary>
        [JsonProperty("image")]
        public EmbedImage EmbedImage { get; set; }
        /// <summary>
        /// Thumbnail information
        /// </summary>
        [JsonProperty("thumbnail")]
        public EmbedThumbnail EmbedThumbnail { get; set; }
        /// <summary>
        /// Video information
        /// </summary>
        [JsonProperty("video")]
        public EmbedVideo EmbedVideo { get; set; }
        /// <summary>
        /// Provider information
        /// </summary>
        [JsonProperty("provider")]
        public EmbedProvider EmbedProvider { get; set; }
        /// <summary>
        /// Author information
        /// </summary>
        [JsonProperty("author")]
        public EmbedAuthor EmbedAuthor { get; set; }
        /// <summary>
        /// Fields information
        /// </summary>
        [JsonProperty("fields")]
        public List<EmbedField> EmbedFields { get; set; }

        [JsonConstructor]
        public Embed() { }
    }
}
