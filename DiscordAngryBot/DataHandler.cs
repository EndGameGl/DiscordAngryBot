using DiscordAngryBot.CustomObjects.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordAngryBot
{
    public class DataHandler
    {
        public List<Timer> timers { get; set; }
        public List<Group> groups { get; set; }
        public DataHandler()
        {
            timers = new List<Timer>();
            groups = new List<Group>();
        }
    }
}
