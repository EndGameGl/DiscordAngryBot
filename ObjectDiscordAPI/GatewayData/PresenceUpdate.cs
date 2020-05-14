using Newtonsoft.Json;
using ObjectDiscordAPI.GatewayOperations;
using ObjectDiscordAPI.Resources;
using System;

namespace ObjectDiscordAPI.GatewayData
{
    public class PresenceUpdate
    {
        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("roles")]
        public ulong[] Roles { get; set; }

        [JsonProperty("game")]
        public Activity Game { get; set; }

        [JsonProperty("guild_id")]
        public ulong GuildID { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("activities")]
        public Activity[] Activities { get; set; }

        [JsonProperty("client_status")]
        public ClientStatus ClientStatus { get; set; }

        [JsonProperty("premium_since")]
        public DateTime? PremiumSince { get; set; }

        [JsonProperty("nick")]
        public string Nickname { get; set; }

        [JsonConstructor]
        public PresenceUpdate() { }
    }
}
