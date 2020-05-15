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
        [JsonProperty("id")]
        public ulong ID { get; set; }

        [JsonProperty("filename")]
        public string FileName { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("url")]
        public string URL { get; set; }

        [JsonProperty("proxy_url")]
        public string ProxyURL { get; set; }

        [JsonProperty("height")]
        public int? Height { get; set; }

        [JsonProperty("width")]
        public int? Width { get; set; }

        [JsonConstructor]
        public MessageAttachment() { }
    }
}
