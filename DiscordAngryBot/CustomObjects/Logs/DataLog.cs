using DiscordAngryBot.CustomObjects.DiscordCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Logs
{
    public class DataLog
    {
        public string LogType { get; set; }
        public Exception Exception { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;

        public string GetMessage()
        {
            if (Exception != null)
            {
                return Exception.Message;
            }
            else
                return Message;
        }
    }
}
