using Newtonsoft.Json;

namespace ObjectDiscordAPI.GatewayData.GatewayCommands
{
    public class GatewayRequestGuildMembers
    {
        [JsonProperty("guild_id")]
        public ulong GuildID { get; set; }

        [JsonProperty("query")]
        public string QueryString { get; set; }

        [JsonProperty("limit")]
        public int UserLimit { get; set; }

        [JsonProperty("presences")]
        public bool? NeedPresences { get; set; }

        [JsonProperty("user_ids")]
        public ulong[] UserIDs { get; set; }

        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        [JsonConstructor]
        public GatewayRequestGuildMembers() { }
    }
}
