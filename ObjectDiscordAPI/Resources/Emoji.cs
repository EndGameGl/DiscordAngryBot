using Newtonsoft.Json;
using ObjectDiscordAPI.Resources.GuildResources;
using System.Collections.Generic;

namespace ObjectDiscordAPI.Resources
{
    /// <summary>
    /// Emoji Object
    /// </summary>
    public class Emoji
    {
        /// <summary>
        /// Emoji id
        /// </summary>
        [JsonProperty("id")]
        public ulong? ID { get; set; }
        /// <summary>
        /// Emoji name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// Roles this emoji is whitelisted to
        /// </summary>
        [JsonProperty("roles")]
        public List<GuildRole> Roles { get; set; }
        /// <summary>
        /// User that created this emoji
        /// </summary>
        [JsonProperty("user")]
        public User User { get; set; }
        /// <summary>
        /// Whether this emoji must be wrapped in colons
        /// </summary>
        [JsonProperty("require_colons")]
        public bool? DoRequireColons { get; set; }
        /// <summary>
        /// Whether this emoji is managed
        /// </summary>
        [JsonProperty("managed")]
        public bool? IsManaged { get; set; }
        /// <summary>
        /// Whether this emoji is animated
        /// </summary>
        [JsonProperty("animated")]
        public bool? IsAnimated { get; set; }
        /// <summary>
        /// Whether this emoji can be used, may be false due to loss of Server Boosts
        /// </summary>
        [JsonProperty("available")]
        public bool? IsAvailable { get; set; }

        [JsonConstructor]
        public Emoji() { }
    }
}
