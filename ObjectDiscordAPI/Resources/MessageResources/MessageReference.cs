using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Resources.MessageResources
{
    public class MessageReference
    {
        /// <summary>
        /// ID of the originating message
        /// </summary>
        [JsonProperty("message_id")]
        public ulong? MessageID { get; set; }
        /// <summary>
        /// ID of the originating message's channel
        /// </summary>
        [JsonProperty("channel_id")]
        public ulong? ChannelID { get; set; }
        /// <summary>
        /// ID of the originating message's guild
        /// </summary>
        [JsonProperty("guild_id")]
        public ulong? GuildID { get; set; }

        [JsonConstructor]
        public MessageReference() { }
    }
}
