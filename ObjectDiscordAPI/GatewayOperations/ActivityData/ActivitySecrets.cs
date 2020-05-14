using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayOperations.ActivityData
{
    public class ActivitySecrets
    {
        [JsonProperty("join")]
        public string JoinSecret { get; set; }

        [JsonProperty("spectate")]
        public string SpectateSecret { get; set; }

        [JsonProperty("match")]
        public string MatchSecret { get; set; }

        [JsonConstructor]
        public ActivitySecrets() { }
    }
}
