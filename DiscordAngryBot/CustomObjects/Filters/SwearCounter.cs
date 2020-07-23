using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Filters
{
    /// <summary>
    /// Счетчик матов
    /// </summary>
    public class SwearCounter
    { 
        /// <summary>
        /// Цель проверок
        /// </summary>
        public SocketGuildUser author { get; set; }
        /// <summary>
        /// Найденные сообщения с матами
        /// </summary>
        public List<SocketMessage> reasons { get; set; }
    }
}
