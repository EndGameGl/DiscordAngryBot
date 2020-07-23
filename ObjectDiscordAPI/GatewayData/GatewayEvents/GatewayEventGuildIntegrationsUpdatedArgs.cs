using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayData.GatewayEvents
{
    /// <summary>
    /// Sent when a guild integration is updated.
    /// </summary>
    public class GatewayEventGuildIntegrationsUpdatedArgs
    {
        /// <summary>
        /// ID of the guild whose integrations were updated
        /// </summary>
        [JsonProperty("guild_id")]
        public ulong GuildID { get; set; }

        [JsonConstructor]
        public GatewayEventGuildIntegrationsUpdatedArgs() { }
    }
}
