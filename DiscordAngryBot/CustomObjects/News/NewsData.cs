using Newtonsoft.Json;

namespace DiscordAngryBot.CustomObjects.News
{
    /// <summary>
    /// Class for posting external data to news channels
    /// </summary>
    public class NewsData
    {
        /// <summary>
        /// Image URL
        /// </summary>
        [JsonProperty("image")]
        public string ImageURL { get; set; }

        /// <summary>
        /// Message text
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// Identity token
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        /// <summary>
        /// Source URL
        /// </summary>
        [JsonProperty("url")]
        public string SourceURL { get; set; }

        /// <summary>
        /// Class constructor
        /// </summary>
        [JsonConstructor]
        public NewsData() { }
    }
}
