using Newtonsoft.Json;

namespace ObjectDiscordAPI.Resources.GuildResources
{
    public class GuildPreview
    {
        [JsonProperty("id")]
        public ulong ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("icon")]
        public string IconHash { get; set; }

        [JsonProperty("splash")]
        public string SplashHash { get; set; }

        [JsonProperty("discovery_splash")]
        public string DiscoverySplashHash { get; set; }

        [JsonProperty("emojis")]
        public Emoji[] Emojis { get; set; }

        [JsonProperty("features")]
        public string[] Features { get; set; }

        [JsonProperty("approzimate_member_count")]
        public int ApproximateMemberCount { get; set; }

        [JsonProperty("approximate_presence_count")]
        public int ApproximatePresenceCount { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonConstructor]
        public GuildPreview() { }
    }
}
