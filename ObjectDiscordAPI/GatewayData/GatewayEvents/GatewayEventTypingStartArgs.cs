using Newtonsoft.Json;
using ObjectDiscordAPI.Resources.GuildResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayData.GatewayEvents
{
    public class GatewayEventTypingStartArgs
    {
        /// <summary>
        /// ID of the channel
        /// </summary>
        [JsonProperty("channel_id")]
        public ulong ChannelID { get; set; }
        /// <summary>
        /// ID of the guild
        /// </summary>
        [JsonProperty("guild_id")]
        public ulong? GuildID { get; set; }
        /// <summary>
        /// ID of the user
        /// </summary>
        [JsonProperty("user_id")]
        public ulong UserID { get; set; }
        /// <summary>
        /// Unix time (in seconds) of when the user started typing
        /// </summary>
        [JsonProperty("timestamp")]
        public ulong Timestamp { get; set; }
        /// <summary>
        /// The member who started typing if this happened in a guild
        /// </summary>
        [JsonProperty("member")]
        public GuildMember Member { get; set; }
        [JsonConstructor]
        public GatewayEventTypingStartArgs() { }
    }
}
