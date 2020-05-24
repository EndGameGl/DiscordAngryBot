using Newtonsoft.Json;

namespace ObjectDiscordAPI.Resources.GuildResources
{
    /// <summary>
    /// Roles represent a set of permissions attached to a group of users.
    /// </summary>
    public class GuildRole
    {
        /// <summary>
        /// Role ID
        /// </summary>
        [JsonProperty("id")]
        public ulong ID { get; set; }
        /// <summary>
        /// Role name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// Integer representation of hexadecimal color code
        /// </summary>
        [JsonProperty("color")]
        public int Color { get; set; }
        /// <summary>
        /// If this role is pinned in the user listing
        /// </summary>
        [JsonProperty("hoist")]
        public bool IsHoist { get; set; }
        /// <summary>
        /// Position of this role
        /// </summary>
        [JsonProperty("position")]
        public int Position { get; set; }
        /// <summary>
        /// Permission bit set
        /// </summary>
        [JsonProperty("permissions")]
        public int Permissions { get; set; }
        /// <summary>
        /// Whether this role is managed by an integration
        /// </summary>
        [JsonProperty("managed")]
        public bool IsManaged { get; set; }
        /// <summary>
        /// Whether this role is mentionable
        /// </summary>
        [JsonProperty("mentionable")]
        public bool IsMentionable { get; set; }

        [JsonConstructor]
        public GuildRole() { }
    }
}
