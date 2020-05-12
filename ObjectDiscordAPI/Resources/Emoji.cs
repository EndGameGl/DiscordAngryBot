using Newtonsoft.Json;
using ObjectDiscordAPI.Resources.GuildResources;

namespace ObjectDiscordAPI.Resources
{
    public class Emoji
    {
        [JsonProperty("id")]
        public ulong? ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("roles")]
        public GuildRole[] Roles { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("require_colons")]
        public bool? DoRequireColons { get; set; }

        [JsonProperty("managed")]
        public bool? IsManaged { get; set; }

        [JsonProperty("animated")]
        public bool? IsAnimated { get; set; }

        [JsonProperty("available")]
        public bool? IsAvailable { get; set; }

        [JsonConstructor]
        public Emoji() { }
    }
}
