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
        /// <summary>
        /// The secret for joining a party
        /// </summary>
        [JsonProperty("join")]
        public string JoinSecret { get; set; }
        /// <summary>
        /// The secret for spectating a game
        /// </summary>
        [JsonProperty("spectate")]
        public string SpectateSecret { get; set; }
        /// <summary>
        /// The secret for a specific instanced match
        /// </summary>
        [JsonProperty("match")]
        public string MatchSecret { get; set; }

        [JsonConstructor]
        public ActivitySecrets() { }
    }
}
