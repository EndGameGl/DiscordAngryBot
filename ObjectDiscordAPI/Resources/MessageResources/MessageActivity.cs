using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Resources.MessageResources
{
    public enum MessageActivityType
    {
        Join = 1,
        Spectate = 2,
        Listen = 3,
        JoinRequest = 5
    }
    public class MessageActivity
    {
        [JsonProperty("type")]
        public MessageActivityType MessageActivityType { get; set; }

        [JsonProperty("party_id")]
        public string PartyID { get; set; }

        [JsonConstructor]
        public MessageActivity() { }
    }
}
