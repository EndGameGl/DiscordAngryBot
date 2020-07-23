using Newtonsoft.Json;
using ObjectDiscordAPI.Resources.GuildResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayData
{
    public class GuildMembersChunk
    {
        [JsonProperty("guild_id")]
        public ulong GuildID { get; set; }

        [JsonProperty("members")]
        public GuildMember[] members { get; set; }

        [JsonProperty("chunk_index")]
        public int ChunkIndex { get; set; }

        [JsonProperty("chunk_count")]
        public int ChunkCount { get; set; }

        [JsonProperty("not_found")]
        public object[] NotFound { get; set; }

        [JsonProperty("presences")]
        public PresenceUpdate[] Presences { get; set; }

        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        [JsonConstructor]
        public GuildMembersChunk() { }
    }
}
