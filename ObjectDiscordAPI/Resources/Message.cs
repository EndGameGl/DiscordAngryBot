﻿using Newtonsoft.Json;
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
    /// <summary>
    /// Represents a message sent in a channel within Discord.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// ID of the message
        /// </summary>
        [JsonProperty("id")]
        public ulong ID { get; set; }
        /// <summary>
        /// ID of the channel the message was sent in
        /// </summary>
        [JsonProperty("channel_id")]
        public ulong ChannelID { get; set; }
        /// <summary>
        /// ID of the guild the message was sent in
        /// </summary>
        [JsonProperty("guild_id")]
        public ulong? GuildID { get; set; }
        /// <summary>
        /// The author of this message (not guaranteed to be a valid user, see webhooks info)
        /// </summary>
        [JsonProperty("author")]
        public User Author { get; set; }
        /// <summary>
        /// Member properties for this message's author
        /// </summary>
        [JsonProperty("member")]
        public GuildMember Member { get; set; }
        /// <summary>
        /// Contents of the message
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }
        /// <summary>
        /// When this message was sent
        /// </summary>
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// When this message was edited (or null if never)
        /// </summary>
        [JsonProperty("edited_timestamp")]
        public DateTime? EditedTimestamp { get; set; }
        /// <summary>
        /// whether this was a Text-To-Speech message
        /// </summary>
        [JsonProperty("tts")]
        public bool IsTTS { get; set; }
        /// <summary>
        /// Whether this message mentions everyone
        /// </summary>
        [JsonProperty("mention_everyone")]
        public bool IsMentioningEveryone { get; set; }
        /// <summary>
        /// Users specifically mentioned in the message
        /// </summary>
        [JsonProperty("mentions")]
        public List<object> Mentions { get; set; }
        /// <summary>
        /// Roles specifically mentioned in this message
        /// </summary>
        [JsonProperty("mention_roles")]
        public List<ulong> MentionedRoles { get; set; }
        /// <summary>
        /// Channels specifically mentioned in this message
        /// </summary>
        [JsonProperty("mention_channels")]
        public List<Channel> MentionedChannels { get; set; }
        /// <summary>
        /// Any attached files
        /// </summary>
        [JsonProperty("attachments")]
        public List<MessageAttachment> Attachments { get; set; }
        /// <summary>
        /// Any embedded content
        /// </summary>
        [JsonProperty("embeds")]
        public List<Embed> Embeds { get; set; }
        /// <summary>
        /// Reactions to the message
        /// </summary>
        [JsonProperty("reactions")]
        public List<Reaction> Reactions { get; set; }
        /// <summary>
        /// Used for validating a message was sent
        /// </summary>
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
        /// <summary>
        /// Whether this message is pinned
        /// </summary>
        [JsonProperty("pinned")]
        public bool IsPinned { get; set; }
        /// <summary>
        /// If the message is generated by a webhook, this is the webhook's id
        /// </summary>
        [JsonProperty("webhook_id")]
        public ulong? WebhookID { get; set; }
        /// <summary>
        /// Type of message
        /// </summary>
        [JsonProperty("type")]
        public MessageType Type { get; set; }
        /// <summary>
        /// Sent with Rich Presence-related chat embeds
        /// </summary>
        [JsonProperty("activity")]
        public MessageActivity MessageActivity { get; set; }
        /// <summary>
        /// Sent with Rich Presence-related chat embeds
        /// </summary>
        [JsonProperty("application")]
        public MessageApplication MessageApplication { get; set; }
        /// <summary>
        /// Reference data sent with crossposted messages
        /// </summary>
        [JsonProperty("message_reference")]
        public MessageReference MessageReference { get; set; }
        /// <summary>
        /// Message flags OR d together, describes extra features of the message
        /// </summary>
        [JsonProperty("flags")] 
        public int? Flags { get; set; }

        [JsonConstructor]
        public Message() { }
    }
}
