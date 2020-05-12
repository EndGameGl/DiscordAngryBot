using Newtonsoft.Json;

namespace ObjectDiscordAPI.Resources
{
    public enum UserPremiumType
    {
        None,
        NitroClassic,
        Nitro
    }
    public class User
    {
        [JsonProperty("id")]
        public ulong ID { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("discriminator")]
        public string Discriminator { get; set; }

        [JsonProperty("avatar")]
        public string AvatarHash { get; set; }

        [JsonProperty("bot")]
        public bool? IsBot { get; set; }

        [JsonProperty("system")]
        public bool? IsSystem { get; set; }

        [JsonProperty("mfa_enabled")]
        public bool? IsMFAEnabled { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("verified")]
        public bool? IsVerified { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("flags")]
        public int? Flags { get; set; }

        [JsonProperty("premium_type")]
        public UserPremiumType? PremiumType { get; set; }

        [JsonProperty("public_flags")]
        public int? PublicFlags { get; set; }

        [JsonConstructor]
        public User() { }
    }
}
