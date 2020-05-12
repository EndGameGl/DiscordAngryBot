using Newtonsoft.Json;

namespace ObjectDiscordAPI.Resources
{
    public class IntegrationAccount
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonConstructor]
        public IntegrationAccount() { }
    }
}
