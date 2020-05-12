using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Resources
{
    public class VoiceRegion
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool vip { get; set; }
        public bool optimal { get; set; }
        public bool deprecated { get; set; }
        public bool custom { get; set; }
        public VoiceRegion() { }
    }
}
