using Discord.WebSocket;
using System.Collections.Generic;

namespace DiscordAngryBot.CustomObjects.Filters
{
    /// <summary>
    /// Class for counting swears made by user
    /// </summary>
    public class SwearCounter
    { 
        /// <summary>
        /// User
        /// </summary>
        public SocketGuildUser Author { get; set; }
        /// <summary>
        /// Messages with swears
        /// </summary>
        public List<SocketMessage> Reasons { get; set; }
    }
}
