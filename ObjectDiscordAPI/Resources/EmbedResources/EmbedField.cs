using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Resources.EmbedResources
{
    public class EmbedField
    {
        /// <summary>
        /// Name of the field
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// Value of the field
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
        /// <summary>
        /// Whether or not this field should display inline
        /// </summary>
        [JsonProperty("inline")]
        public bool? IsInline { get; set; }

        [JsonConstructor]
        public EmbedField() { }
    }
}
