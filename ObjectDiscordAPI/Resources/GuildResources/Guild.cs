using Newtonsoft.Json;

namespace ObjectDiscordAPI.Resources.GuildResources
{
    public enum DefaultMessageNotificationLevel
    {
        AllMessages,
        OnlyMentions
    }
    public enum VerificationLevel
    {
        None,
        Low,
        Medium,
        High,
        VeryHigh
    }
    public enum ExplicitContentFilterLevel
    {
        Disabled,
        MembersWithoutRoles,
        AllMembers
    }
    public enum MFALevel
    {
        None,
        Elevated
    }
    public enum SystemChannelFlags
    {
        SupressJoinNotifications = 1 << 0,
        SupressPremiumSubscriptions = 1 << 1
    }
    public enum PremiumTierLevel
    {
        None,
        Tier1,
        Tier2,
        Tier3
    }

    /// <summary>
    /// Guild object
    /// </summary>
    public class Guild
    {
        /// <summary>
        /// Guild ID
        /// </summary>
        [JsonProperty("id")]
        public ulong ID { get; set; }

        /// <summary>
        /// Guild name (2-100 characters, excluding trailing and leading whitespace)
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Icon hash
        /// </summary>
        [JsonProperty("icon")]
        public string IconHash { get; set; }

        /// <summary>
        /// Splash hash
        /// </summary>
        [JsonProperty("splash")]
        public string SplashHash { get; set; }

        /// <summary>
        /// Discovery splash hash; only present for guilds with the "DISCOVERABLE" feature
        /// </summary>
        [JsonProperty("discovery_splash")]
        public string DiscoverySplashHash { get; set; }

        /// <summary>
        /// True if the user is the owner of the guild
        /// </summary>
        [JsonProperty("owner")]
        public bool? IsOwner { get; set; }

        /// <summary>
        /// ID of owner
        /// </summary>
        [JsonProperty("owner_id")]
        public ulong OwnerID { get; set; }

        /// <summary>
        /// Total permissions for the user in the guild (excludes overrides)
        /// </summary>
        [JsonProperty("permissions")]
        public int? Permissions { get; set; }

        /// <summary>
        /// Voice region id for the guild
        /// </summary>
        [JsonProperty("region")]
        public string Region { get; set; }

        /// <summary>
        /// ID of afk channel
        /// </summary>
        [JsonProperty("afk_channel_id")]
        public ulong? AFKChannelID { get; set; }

        /// <summary>
        /// AFK timeout in seconds
        /// </summary>
        [JsonProperty("afk_timeout")]
        public int AFKTimeout { get; set; }

        /// <summary>
        /// True if the server widget is enabled (deprecated, replaced with widget_enabled)
        /// </summary>
        [JsonProperty("embed_enabled")]
        public bool? IsEmbedEnabled { get; set; }

        /// <summary>
        /// The channel id that the widget will generate an invite to, or null if set to no invite (deprecated, replaced with widget_channel_id)
        /// </summary>
        [JsonProperty("embed_channel_id")]
        public ulong? EmbedChannelID { get; set; }

        /// <summary>
        /// Verification level required for the guild
        /// </summary>
        [JsonProperty("verification_level")]
        public VerificationLevel VerificationLevel { get; set; }

        /// <summary>
        /// Default message notifications level
        /// </summary>
        [JsonProperty("default_message_notifications")]
        public DefaultMessageNotificationLevel DefaultMessageNotificationsLevel { get; set; }

        /// <summary>
        /// Explicit content filter level
        /// </summary>
        [JsonProperty("explicit_content_filter")]
        public ExplicitContentFilterLevel ExplicitContentFilterLevel { get; set; }

        /// <summary>
        /// Roles in the guild
        /// </summary>
        [JsonProperty("roles")]
        public GuildRole[] Roles { get; set; }

        /// <summary>
        /// Custom guild emojis
        /// </summary>
        [JsonProperty("emojis")]
        public Emoji[] Emojis { get; set; }

        /// <summary>
        /// Enabled guild features
        /// </summary>
        [JsonProperty("features")]
        public string[] Features { get; set; }

        /// <summary>
        /// Required MFA level for the guild
        /// </summary>
        [JsonProperty("mfa_level")]
        public MFALevel MFALevel { get; set; }

        /// <summary>
        /// Application id of the guild creator if it is bot-created
        /// </summary>
        [JsonProperty("application_id")]
        public ulong? ApplicationID { get; set; }

        /// <summary>
        /// True if the server widget is enabled
        /// </summary>
        [JsonProperty("widget_enabled")]
        public bool? IsWidgetEnabled { get; set; }

        /// <summary>
        /// The channel id that the widget will generate an invite to, or null if set to no invite
        /// </summary>
        [JsonProperty("widget_channel_id")]
        public ulong? WidgetChannelID { get; set; }

        /// <summary>
        /// The id of the channel where guild notices such as welcome messages and boost events are posted
        /// </summary>
        [JsonProperty("systen_channel_id")]
        public ulong? SystemChannelID { get; set; }

        /// <summary>
        /// System channel flags
        /// </summary>
        [JsonProperty("system_channel_flags")]
        public SystemChannelFlags SystemChannelFlags { get; set; }

        /// <summary>
        /// The id of the channel where guilds with the "PUBLIC" feature can display rules and/or guidelines
        /// </summary>
        [JsonProperty("rules_channel_id")]
        public int? RulesChannelID { get; set; }

        /// <summary>
        /// The maximum number of presences for the guild (the default value, currently 25000, is in effect when null is returned)
        /// </summary>
        [JsonProperty("max_presences")]
        public int? MaxPresences { get; set; }

        /// <summary>
        /// The maximum number of members for the guild
        /// </summary>
        [JsonProperty("max_members")]
        public int? MaxMembers { get; set; }

        /// <summary>
        /// The vanity url code for the guild
        /// </summary>
        [JsonProperty("vanity_url_code")]
        public string VanityUrlCode { get; set; }

        /// <summary>
        /// The description for the guild, if the guild is discoverable
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Banner hash
        /// </summary>
        [JsonProperty("banner")]
        public string BannerHash { get; set; }

        /// <summary>
        /// Premium tier (Server Boost level)
        /// </summary>
        [JsonProperty("premium_tier")]
        public PremiumTierLevel PremiumTierLevel { get; set; }

        /// <summary>
        /// The number of boosts this guild currently has
        /// </summary>
        [JsonProperty("premium_subscription_count")]
        public int? PremiumSubscriptionCount { get; set; }

        /// <summary>
        /// The preferred locale of a guild with the "PUBLIC" feature; used in server discovery and notices from Discord; defaults to "en-US"
        /// </summary>
        [JsonProperty("preferred_locale")]
        public string PreferredLocale { get; set; }

        /// <summary>
        /// The id of the channel where admins and moderators of guilds with the "PUBLIC" feature receive notices from Discord
        /// </summary>
        [JsonProperty("public_updates_channel_id")]
        public ulong? PublicUpdatesChannelID { get; set; }

        /// <summary>
        /// The maximum amount of users in a video channel
        /// </summary>
        [JsonProperty("max_video_channel_users")]
        public int? MaxVideoChannelUsers { get; set; }

        /// <summary>
        /// Approximate number of members in this guild, returned from the GET /guild/<id> endpoint when with_counts is true
        /// </summary>
        [JsonProperty("approximate_member_count")]
        public int? ApproximateMemberCount { get; set; }

        /// <summary>
        /// Approximate number of non-offline members in this guild, returned from the GET /guild/<id> endpoint when with_counts is true
        /// </summary>
        [JsonProperty("approximate_presence_count")]
        public int? ApproximatePresenceCount { get; set; }

        [JsonConstructor]
        public Guild() { }
    }
}
