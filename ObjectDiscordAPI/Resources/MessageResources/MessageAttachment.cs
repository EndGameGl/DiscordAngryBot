using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Resources.MessageResouces
{
    public class MessageAttachment
    {
        /// <summary>
        /// Attachment id
        /// </summary>
        [JsonProperty("id")]
        public ulong ID { get; set; }
        /// <summary>
        /// Name of file attached
        /// </summary>
        [JsonProperty("filename")]
        public string FileName { get; set; }
        /// <summary>
        /// Size of file in bytes
        /// </summary>
        [JsonProperty("size")]
        public int Size { get; set; }
        /// <summary>
        /// Source url of file
        /// </summary>
        [JsonProperty("url")]
        public string URL { get; set; }
        /// <summary>
        /// A proxied url of file
        /// </summary>
        [JsonProperty("proxy_url")]
        public string ProxyURL { get; set; }
        /// <summary>
        /// Height of file (if image)
        /// </summary>
        [JsonProperty("height")]
        public int? Height { get; set; }
        /// <summary>
        /// Width of file (if image)
        /// </summary>
        [JsonProperty("width")]
        public int? Width { get; set; }

        [JsonConstructor]
        public MessageAttachment() { }
    }
}
