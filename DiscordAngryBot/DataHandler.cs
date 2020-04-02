using DiscordAngryBot.CustomObjects.Parties;
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
        public List<Party> parties { get; set; }
        public List<Raid> raids { get; set; }
        public DataHandler()
        {
            timers = new List<Timer>();
            parties = new List<Party>();
            raids = new List<Raid>();
        }
    }
}
