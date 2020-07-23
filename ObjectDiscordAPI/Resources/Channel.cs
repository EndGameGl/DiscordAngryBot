using Newtonsoft.Json;
using System;

namespace ObjectDiscordAPI.Resources
{
    public enum ChannelType
    {
        GuildText,
        DM,
        GuildVoice,
        GroupDM,
        GuildCategory,
        GuildNews,
        GuildStore
    }
    /// <summary>
    /// Represents a guild or DM channel within Discord
    /// </summary>
    public class Channel
    {
        /// <summary>
        /// The id of this channel
        /// </summary>
        [JsonProperty("id")]
        public ulong ID { get; set; }
        /// <summary>
        /// The type of channel
        /// </summary>
        [JsonProperty("type")]
        public ChannelType ChannelType { get; set; }
        /// <summary>
        /// The id of the guild
        /// </summary>
        [JsonProperty("guild_id")]
        public ulong? GuildID { get; set; }
        /// <summary>
        /// Sorting position of the channel
        /// </summary>
        [JsonProperty("position")]
        public int? Position { get; set; }
        /// <summary>
        /// Explicit permission overwrites for members and roles
        /// </summary>
        [JsonProperty("permission_overwrites")]
        public Overwrite[] PermissionOverwrites { get; set; }
        /// <summary>
        /// The name of the channel (2-100 characters)
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// The channel topic (0-1024 characters)
        /// </summary>
        [JsonProperty("topic")]
        public string Topic { get; set; }
        /// <summary>
        /// Whether the channel is nsfw
        /// </summary>
        [JsonProperty("nsfw")]
        public bool? IsNSFW { get; set; }
        /// <summary>
        /// The id of the last message sent in this channel (may not point to an existing or valid message)
        /// </summary>
        [JsonProperty("last_message_id")]
        public ulong? LastMessageID { get; set; }
        /// <summary>
        /// The bitrate (in bits) of the voice channel
        /// </summary>
        [JsonProperty("bitrate")]
        public int? Bitrate { get; set; }
        /// <summary>
        /// The user limit of the voice channel
        /// </summary>
        [JsonProperty("user_limit")]
        public int? UserLimit { get; set; }
        /// <summary>
        /// Amount of seconds a user has to wait before sending another message (0-21600); bots, as well as users with the permission manage_messages or manage_channel, are unaffected
        /// </summary>
        [JsonProperty("rate_limit_per_user")]
        public int? RateLimitPerUser { get; set; }
        /// <summary>
        /// The recipients of the DM
        /// </summary>
        [JsonProperty("recipients")]
        public User[] Recipients { get; set; }
        /// <summary>
        /// Icon hash
        /// </summary>
        [JsonProperty("icon")]
        public string IconHash { get; set; }
        /// <summary>
        /// ID of the DM creator
        /// </summary>
        [JsonProperty("owner_id")]
        public ulong? OwnerID { get; set; }
        /// <summary>
        /// Application id of the group DM creator if it is bot-created
        /// </summary>
        [JsonProperty("application_id")]
        public ulong? ApplicationID { get; set; }
        /// <summary>
        /// ID of the parent category for a channel (each parent category can contain up to 50 channels)
        /// </summary>
        [JsonProperty("parent_id")]
        public ulong? ParentID { get; set; }
        /// <summary>
        /// When the last pinned message was pinned
        /// </summary>
        [JsonProperty("last_pin_timestamp")]
        public DateTime? LastPinTimestamp { get; set; }

        [JsonConstructor]
        public Channel() { }
    }
}
