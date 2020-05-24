using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Resources
{
    /// <summary>
    /// Reaction Object
    /// </summary>
    public class Reaction
    {
        /// <summary>
        /// Times this emoji has been used to react
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; set; }
        /// <summary>
        /// Whether the current user reacted using this emoji
        /// </summary>
        [JsonProperty("me")]
        public bool IsMe { get; set; }
        /// <summary>
        /// Emoji information
        /// </summary>
        [JsonProperty("emoji")]
        public Emoji Emoji { get; set; }

        [JsonConstructor]
        public Reaction() { }
    }
}
