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
    public class Guild
    {
        [JsonProperty("id")]
        public ulong ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("icon")]
        public string IconHash { get; set; }

        [JsonProperty("splash")]
        public string SplashHash { get; set; }

        [JsonProperty("discovery_splash")]
        public string DiscoverySplashHash { get; set; }

        [JsonProperty("owner")]
        public bool? IsOwner { get; set; }

        [JsonProperty("owner_id")]
        public ulong OwnerID { get; set; }

        [JsonProperty("permissions")]
        public int? Permissions { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("afk_channel_id")]
        public ulong? AFKChannelID { get; set; }

        [JsonProperty("afk_timeout")]
        public int AFKTimeout { get; set; }

        [JsonProperty("embed_enabled")]
        public bool? IsEmbedEnabled { get; set; }

        [JsonProperty("embed_channel_id")]
        public ulong? EmbedChannelID { get; set; }

        [JsonProperty("verification_level")]
        public VerificationLevel VerificationLevel { get; set; }

        [JsonProperty("default_message_notifications")]
        public DefaultMessageNotificationLevel DefaultMessageNotificationsLevel { get; set; }

        [JsonProperty("explicit_content_filter")]
        public ExplicitContentFilterLevel ExplicitContentFilterLevel { get; set; }

        [JsonProperty("roles")]
        public GuildRole[] Roles { get; set; }

        [JsonProperty("emojis")]
        public Emoji[] Emojis { get; set; }

        [JsonProperty("features")]
        public string[] Features { get; set; }

        [JsonProperty("mfa_level")]
        public MFALevel MFALevel { get; set; }

        [JsonProperty("application_id")]
        public ulong? ApplicationID { get; set; }

        [JsonProperty("widget_enabled")]
        public bool? IsWidgetEnabled { get; set; }

        [JsonProperty("widget_channel_id")]
        public ulong? WidgetChannelID { get; set; }

        [JsonProperty("systen_channel_id")]
        public ulong? SystemChannelID { get; set; }

        [JsonProperty("system_channel_flags")]
        public SystemChannelFlags SystemChannelFlags { get; set; }

        [JsonProperty("rules_channel_id")]
        public int? RulesChannelID { get; set; }

        [JsonProperty("max_presences")]
        public int? MaxPresences { get; set; }

        [JsonProperty("max_members")]
        public int? MaxMembers { get; set; }

        [JsonProperty("vanity_url_code")]
        public string VanityUrlCode { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("banner")]
        public string BannerHash { get; set; }

        [JsonProperty("premium_tier")]
        public PremiumTierLevel PremiumTierLevel { get; set; }

        [JsonProperty("premium_subscription_count")]
        public int? PremiumSubscriptionCount { get; set; }

        [JsonProperty("preferred_locale")]
        public string PreferredLocale { get; set; }

        [JsonProperty("public_updates_channel_id")]
        public ulong? PublicUpdatesChannelID { get; set; }

        [JsonProperty("max_video_channel_users")]
        public int? MaxVideoChannelUsers { get; set; }

        [JsonProperty("approximate_member_count")]
        public int? ApproximateMemberCount { get; set; }

        [JsonProperty("approximate_presence_count")]
        public int? ApproximatePresenceCount { get; set; }

        [JsonConstructor]
        public Guild() { }
    }
}
