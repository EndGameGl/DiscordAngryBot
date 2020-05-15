using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Resources.MessageResources
{
    public class MessageReference
    {
        [JsonProperty("message_id")]
        public ulong? MessageID { get; set; }

        [JsonProperty("channel_id")]
        public ulong? ChannelID { get; set; }

        [JsonProperty("guild_id")]
        public ulong? GuildID { get; set; }

        [JsonConstructor]
        public MessageReference() { }
    }
}
