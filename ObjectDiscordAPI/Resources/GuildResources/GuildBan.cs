using Newtonsoft.Json;

namespace ObjectDiscordAPI.Resources.GuildResources
{
    public class GuildBan
    {
        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonConstructor]
        public GuildBan() { }
    }
}
