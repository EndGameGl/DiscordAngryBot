using Newtonsoft.Json;
using ObjectDiscordAPI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayData.GatewayEvents
{
    /// <summary>
    /// Sent when a bot removes all instances of a given emoji from the reactions of a message.
    /// </summary>
    public class GatewayEventMessageReactionEmojiRemovedArgs
    {
        /// <summary>
        /// The id of the channel
        /// </summary>
        [JsonProperty("channel_id")]
        public ulong ChannelID { get; set; }
        /// <summary>
        /// The id of the message
        /// </summary>
        [JsonProperty("message_id")]
        public ulong MessageID { get; set; }
        /// <summary>
        /// The id of the guild
        /// </summary>
        [JsonProperty("guild_id")]
        public ulong? GuildID { get; set; }
        /// <summary>
        /// The emoji that was removed
        /// </summary>
        [JsonProperty("emoji")]
        public Emoji Emoji { get; set; }

        [JsonConstructor]
        public GatewayEventMessageReactionEmojiRemovedArgs() { }
    }
}
