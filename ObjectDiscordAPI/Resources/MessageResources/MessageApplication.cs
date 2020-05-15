using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Resources.MessageResources
{
    public class MessageApplication
    {
        [JsonProperty("id")]
        public ulong ID { get; set; }

        [JsonProperty("cover_image")]
        public string ImageAssetID { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("icon")]
        public string IconID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonConstructor]
        public MessageApplication() { }
    }
}
