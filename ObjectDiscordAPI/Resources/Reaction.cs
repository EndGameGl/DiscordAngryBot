using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Resources
{
    public class Reaction
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("me")]
        public bool IsMe { get; set; }

        [JsonProperty("emoji")]
        public Emoji Emoji { get; set; }

        [JsonConstructor]
        public Reaction() { }
    }
}
