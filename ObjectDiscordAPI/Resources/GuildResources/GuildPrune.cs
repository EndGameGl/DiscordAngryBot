using Newtonsoft.Json;

namespace ObjectDiscordAPI.Resources.GuildResources
{
    public class GuildPrune
    {
        [JsonProperty("pruned")]
        public int Pruned { get; set; }

        [JsonConstructor]
        public GuildPrune() { }
    }
}
