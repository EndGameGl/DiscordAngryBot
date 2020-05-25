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
    /// Sent when a guild's emojis have been updated.
    /// </summary>
    public class GatewayEventGuildEmojisUpdateArgs
    {
        /// <summary>
        /// ID of the guild
        /// </summary>
        [JsonProperty("guild_id")]
        public ulong GuildID { get; set; }
        /// <summary>
        /// Array of emojis
        /// </summary>
        [JsonProperty("emojis")]
        public List<Emoji> Emojis { get; set; }

        [JsonConstructor]
        public GatewayEventGuildEmojisUpdateArgs() { }
    }
}
