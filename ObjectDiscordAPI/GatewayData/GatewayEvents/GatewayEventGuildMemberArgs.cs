using Newtonsoft.Json;
using ObjectDiscordAPI.Resources.GuildResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayData.GatewayEvents
{
    /// <summary>
    /// Sent when a new user joins a guild.
    /// </summary>
    public class GatewayEventGuildMemberArgs : GuildMember
    {
        /// <summary>
        /// ID of the guild
        /// </summary>
        [JsonProperty("guild_id")]
        public ulong GuildID { get; set; }

        [JsonConstructor]
        public GatewayEventGuildMemberArgs() { }
    }
}
