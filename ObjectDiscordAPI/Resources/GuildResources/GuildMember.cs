using Newtonsoft.Json;
using System;

namespace ObjectDiscordAPI.Resources.GuildResources
{
    public class GuildMember
    {
        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("nick")]
        public string Nickname { get; set; }

        [JsonProperty("roles")]
        public ulong[] RolesIDs { get; set; }

        [JsonProperty("joined_at")]
        public DateTime JoinedAt { get; set; }

        [JsonProperty("premium_since")]
        public DateTime? PremiumSince { get; set; }
        [JsonProperty("deaf")]
        public bool IsDeaf { get; set; }

        [JsonProperty("mute")]
        public bool IsMuted { get; set; }

        [JsonConstructor]
        public GuildMember() { }
    }
}
