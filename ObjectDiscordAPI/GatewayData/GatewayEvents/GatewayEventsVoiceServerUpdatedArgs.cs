using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayData.GatewayEvents
{
    /// <summary>
    /// Sent when a guild's voice server is updated. This is sent when initially connecting to voice, and when the current voice instance fails over to a new server.
    /// </summary>
    public class GatewayEventsVoiceServerUpdatedArgs
    {
        /// <summary>
        /// Voice connection token
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }
        /// <summary>
        /// The guild this voice server update is for
        /// </summary>
        [JsonProperty("guild_id")]
        public ulong GuildID { get; set; }
        /// <summary>
        /// The voice server host
        /// </summary>
        [JsonProperty("endpoint")]
        public string Endpoint { get; set; }

        [JsonConstructor]
        public GatewayEventsVoiceServerUpdatedArgs() { }
    }
}
