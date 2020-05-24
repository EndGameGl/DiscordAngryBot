using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayData
{
    public class MessageDeleteBulk
    {
        /// <summary>
        /// The ids of the messages
        /// </summary>
        [JsonProperty("ids")]
        public ulong[] IDs { get; set; }
        /// <summary>
        /// The id of the channel
        /// </summary>
        [JsonProperty("channel_id")]
        public ulong ChannelID { get; set; }
        /// <summary>
        /// The id of the guild
        /// </summary>
        [JsonProperty("guild_id")]
        public ulong? GuildID { get; set; }

        [JsonConstructor]
        public MessageDeleteBulk() { }
    }
}
