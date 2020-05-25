using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayData.GatewayEvents
{
    public class GatewayEventMessageAllReactionRemovedArgs
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

        public GatewayEventMessageAllReactionRemovedArgs() { }
    }
}
