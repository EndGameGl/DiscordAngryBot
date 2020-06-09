using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.News
{
    public class NewsData
    {
        [JsonProperty("image")]
        public string ImageURL { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("url")]
        public string SourceURL { get; set; }

        [JsonConstructor]
        public NewsData() { }
    }
}
