using Newtonsoft.Json;

namespace ObjectDiscordAPI.Resources.GuildResources
{
    public class GuildRole
    {
        [JsonProperty("id")]
        public ulong ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("color")]
        public int Color { get; set; }

        [JsonProperty("hoist")]
        public bool IsHoist { get; set; }

        [JsonProperty("position")]
        public int Position { get; set; }

        [JsonProperty("permissions")]
        public int Permissions { get; set; }

        [JsonProperty("managed")]
        public bool IsManaged { get; set; }

        [JsonProperty("mentionable")]
        public bool IsMentionable { get; set; }

        [JsonConstructor]
        public GuildRole() { }
    }
}
