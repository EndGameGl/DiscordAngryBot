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
        /// <summary>
        /// ID of the application
        /// </summary>
        [JsonProperty("id")]
        public ulong ID { get; set; }
        /// <summary>
        /// ID of the embed's image asset
        /// </summary>
        [JsonProperty("cover_image")]
        public string ImageAssetID { get; set; }
        /// <summary>
        /// Application's description
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// ID of the application's icon
        /// </summary>
        [JsonProperty("icon")]
        public string IconID { get; set; }
        /// <summary>
        /// Name of the application
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonConstructor]
        public MessageApplication() { }
    }
}
