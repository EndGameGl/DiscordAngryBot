using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Bans
{
    /// <summary>
    /// Класс, представляющий бан в дискорде
    /// </summary>
    public class DiscordBan
    {
        /// <summary>
        /// Уникальный идентификатор бана
        /// </summary>
        public string GUID { get; set; }

        /// <summary>
        /// Канал гильдии, в котором был выдан бан
        /// </summary>
        public SocketTextChannel Channel { get; set; }

        /// <summary>
        /// Цель бана
        /// </summary>
        public SocketGuildUser BanTarget { get; set; }

        /// <summary>
        /// Объект System.Threading.Timer бана
        /// </summary>
        public Timer BanTimer { get; set; }
        
        /// <summary>
        /// Дата создания бана
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Дата окончания бана
        /// </summary>
        public DateTime? EndsAt { get; set; }
    }
}
