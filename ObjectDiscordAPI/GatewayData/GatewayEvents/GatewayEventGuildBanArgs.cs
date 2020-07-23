using Newtonsoft.Json;
using ObjectDiscordAPI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayData.GatewayEvents
{
    public class GatewayEventGuildBanArgs
    {
        /// <summary>
        /// ID of the guild
        /// </summary>
        [JsonProperty("guild_id")]
        public ulong GuildID { get; set; }
        /// <summary>
        /// The banned user
        /// </summary>
        [JsonProperty("user")]
        public User User { get; set; }

        [JsonConstructor]
        public GatewayEventGuildBanArgs() { }
    }
}
