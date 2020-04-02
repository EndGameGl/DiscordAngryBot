using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordAngryBot
{
    public class BotSettings
    {
        public char commandPrefix { get; set; } = '_';
        public string[] forbiddenCommands { get; set; } = new string[] { ".br all", ".$", ".timely", ".profile", ".xplb" };
        public string[] systemCommands { get; set;} = new string[] { "BAN" };
        public string[] userCommands { get; set; } = new string[] { "PARTY", "RAID", "LIST"};
        public ulong[] admins { get; set; } = new ulong[] { 261497385274966026, 261496784336060417, 279716516595892235 };
        public BotSettings() { }
    }
}
