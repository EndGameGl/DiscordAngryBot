using Newtonsoft.Json;
using ObjectDiscordAPI.Resources.GuildResources;
using ObjectDiscordAPI.Resources.MessageResouces;
using ObjectDiscordAPI.Resources.MessageResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Resources
{
    public enum MessageType
    {
        Default,
        RecipientAdd,
        RecipientRemove,
        Call,
        ChannelNameChange,
        ChannelIconChange,
        ChannelPinnedMessage,
        GuildMemberJoin,
        UserPremiumGuildSubscription,
        UserPremiumGuildSubscriptionTier1,
        UserPremiumGuildSubscriptionTier2,
        UserPremiumGuildSubscriptionTier3,
        ChannelFollowAdd,
        GuildDiscoveryDisqualified,
        GuildDiscoveryRequalified
    }
    public class Message
    {
        [JsonProperty("id")]
        public ulong ID { get; set; }

        [JsonProperty("channel_id")]
        public ulong ChannelID { get; set; }

        [JsonProperty("guild_id")]
        public ulong? GuildID { get; set; }

        [JsonProperty("author")]
        public User Author { get; set; }

        [JsonProperty("member")]
        public GuildMember Member { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("edited_timestamp")]
        public DateTime? EditedTimestamp { get; set; }

        [JsonProperty("tts")]
        public bool IsTTS { get; set; }

        [JsonProperty("mention_everyone")]
        public bool IsMentioningEveryone { get; set; }

        [JsonProperty("mentions")]
        public object[] Mentions { get; set; }

        [JsonProperty("mention_roles")]
        public ulong[] MentionedRoles { get; set; }

        [JsonProperty("mention_channels")]
        public Channel[] MentionedChannels { get; set; }

        [JsonProperty("attachments")]
        public MessageAttachment[] Attachments { get; set; }

        [JsonProperty("embeds")]
        public Embed[] Embeds { get; set; }

        [JsonProperty("reactions")]
        public Reaction[] Reactions { get; set; }

        [JsonProperty("nonce")]
        public object Nonce { get; set; }

        [JsonProperty("pinned")]
        public bool IsPinned { get; set; }

        [JsonProperty("webhook_id")]
        public ulong? WebhookID { get; set; }

        [JsonProperty("type")]
        public MessageType Type { get; set; }

        [JsonProperty("activity")]
        public MessageActivity MessageActivity { get; set; }

        [JsonProperty("application")]
        public MessageApplication MessageApplication { get; set; }

        [JsonProperty("message_reference")]
        public MessageReference MessageReference { get; set; }

        [JsonProperty("flags")] 
        public int? Flags { get; set; }

        [JsonConstructor]
        public Message() { }
    }
}
