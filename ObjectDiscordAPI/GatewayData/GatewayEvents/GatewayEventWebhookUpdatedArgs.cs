using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayData.GatewayEvents
{
    /// <summary>
    /// Sent when a guild channel's webhook is created, updated, or deleted.
    /// </summary>
    public class GatewayEventWebhookUpdatedArgs
    {
        /// <summary>
        /// ID of the guild
        /// </summary>
        [JsonProperty("guild_id")]
        public ulong GuildID { get; set; }

        /// <summary>
        /// ID of the channel
        /// </summary>
        [JsonProperty("channel_id")]
        public ulong ChannelID { get; set; }

        [JsonConstructor]
        public GatewayEventWebhookUpdatedArgs() { }
    }
}
