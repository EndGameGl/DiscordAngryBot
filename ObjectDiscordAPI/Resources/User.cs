using Newtonsoft.Json;

namespace ObjectDiscordAPI.Resources
{
    public enum UserPremiumType
    {
        None,
        NitroClassic,
        Nitro
    }
    /// <summary>
    /// User Object
    /// </summary>
    public class User
    {
        /// <summary>
        /// The user's ID
        /// </summary>
        [JsonProperty("id")]
        public ulong ID { get; set; }
        /// <summary>
        /// The user's username, not unique across the platform
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }
        /// <summary>
        /// The user's 4-digit discord-tag
        /// </summary>
        [JsonProperty("discriminator")]
        public string Discriminator { get; set; }
        /// <summary>
        /// The user's avatar hash
        /// </summary>
        [JsonProperty("avatar")]
        public string AvatarHash { get; set; }
        /// <summary>
        /// Whether the user belongs to an OAuth2 application
        /// </summary>
        [JsonProperty("bot")]
        public bool? IsBot { get; set; }
        /// <summary>
        /// Whether the user is an Official Discord System user (part of the urgent message system)
        /// </summary>
        [JsonProperty("system")]
        public bool? IsSystem { get; set; }
        /// <summary>
        /// Whether the user has two factor enabled on their account
        /// </summary>
        [JsonProperty("mfa_enabled")]
        public bool? IsMFAEnabled { get; set; }
        /// <summary>
        /// The user's chosen language option
        /// </summary>
        [JsonProperty("locale")]
        public string Locale { get; set; }
        /// <summary>
        /// Whether the email on this account has been verified
        /// </summary>
        [JsonProperty("verified")]
        public bool? IsVerified { get; set; }
        /// <summary>
        /// The user's email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }
        /// <summary>
        /// The flags on a user's account
        /// </summary>
        [JsonProperty("flags")]
        public int? Flags { get; set; }
        /// <summary>
        /// The type of Nitro subscription on a user's account
        /// </summary>
        [JsonProperty("premium_type")]
        public UserPremiumType? PremiumType { get; set; }
        /// <summary>
        /// The public flags on a user's account
        /// </summary>
        [JsonProperty("public_flags")]
        public int? PublicFlags { get; set; }

        [JsonConstructor]
        public User() { }
    }
}
