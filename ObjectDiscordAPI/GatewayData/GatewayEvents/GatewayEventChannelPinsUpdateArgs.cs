using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayData.GatewayEvents
{
    public class GatewayEventChannelPinsUpdateArgs
    {
        [JsonProperty("guild_id")]
        public ulong? GuildID { get; set; }

        [JsonProperty("channel_id")]
        public ulong ChannelID { get; set; }

        [JsonProperty("last_pin_timestamp")]
        public DateTime? LastPinTimestamp { get; set; }

        [JsonConstructor]
        public GatewayEventChannelPinsUpdateArgs() { }
    }
}
