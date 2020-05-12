using Newtonsoft.Json;

namespace ObjectDiscordAPI.Resources
{
    public class VoiceRegion
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("vip")]
        public bool IsVIP { get; set; }

        [JsonProperty("optimal")]
        public bool IsOptimal { get; set; }

        [JsonProperty("deprecated")]
        public bool IsDeprecated { get; set; }

        [JsonProperty("custom")]
        public bool IsCustom { get; set; }

        [JsonConstructor]
        public VoiceRegion() { }
    }
}
