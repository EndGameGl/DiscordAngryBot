using Newtonsoft.Json;
using ObjectDiscordAPI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayOperations
{
    public class ReadyData
    {
        [JsonProperty("v")]
        public int Version { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("private_channels")]
        public object[] PrivateChannels { get; set; }

        [JsonProperty("guilds")]
        public UnavailableGuild[] Guilds { get; set; }

        [JsonProperty("session_id")]
        public string SessionID { get; set; }

        [JsonProperty("shard")]
        public int[] Shard { get; set; }

        [JsonConstructor]
        public ReadyData() { }
    }
}
