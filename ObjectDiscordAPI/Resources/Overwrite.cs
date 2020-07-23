using Newtonsoft.Json;

namespace ObjectDiscordAPI.Resources
{
    public class Overwrite
    {
        [JsonProperty("id")]
        public ulong ID { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("allow")]
        public int Allow { get; set; }

        [JsonProperty("deny")]
        public int Deny { get; set; }

        [JsonConstructor]
        public Overwrite() { }
    }
}
