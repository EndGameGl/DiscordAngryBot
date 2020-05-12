using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Resources
{
    public class Overwrite
    {
        public ulong id { get; set; }
        public string type { get; set; }
        public int allow { get; set; }
        public int deny { get; set; }
        public Overwrite() { }
    }
}
