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
    public class Channel
    {
        [JsonProperty("id")]
        public ulong ID { get; set; }

        [JsonProperty("type")]
        public ChannelType ChannelType { get; set; }
        [JsonProperty("guild_id")]
        public ulong? GuildID { get; set; }

        [JsonProperty("position")]
        public int? Position { get; set; }

        [JsonProperty("permission_overwrites")]
        public Overwrite[] PermissionOverwrites { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("topic")]
        public string Topic { get; set; }

        [JsonProperty("nsfw")]
        public bool? IsNSFW { get; set; }

        [JsonProperty("last_message_id")]
        public ulong? LastMessageID { get; set; }

        [JsonProperty("bitrate")]
        public int? Bitrate { get; set; }

        [JsonProperty("user_limit")]
        public int? UserLimit { get; set; }

        [JsonProperty("rate_limit_per_user")]
        public int? RateLimitPerUser { get; set; }

        [JsonProperty("recipients")]
        public User[] Recipients { get; set; }

        [JsonProperty("icon")]
        public string IconHash { get; set; }

        [JsonProperty("owner_id")]
        public ulong? OwnerID { get; set; }

        [JsonProperty("application_id")]
        public ulong? ApplicationID { get; set; }

        [JsonProperty("parent_id")]
        public ulong? ParentID { get; set; }

        [JsonProperty("last_pin_timestamp")]
        public DateTime? last_pin_timestamp { get; set; }

        [JsonConstructor]
        public Channel() { }
    }
}
