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
        /// Канал, в котором выдан бан
        /// </summary>
        public ISocketMessageChannel channel { get; set; }
        /// <summary>
        /// Цель бана
        /// </summary>
        public SocketGuildUser banTarget { get; set; }
        /// <summary>
        /// Роль бана
        /// </summary>
        public SocketRole roleToBan { get; set; }
        /// <summary>
        /// Объект System.Threading.Timer бана
        /// </summary>
        public Timer timer { get; set; }
        /// <summary>
        /// Длина бана
        /// </summary>
        public int? length { get; set; }
        /// <summary>
        /// Признак конечности бана
        /// </summary>
        public bool isInfinite { get; set; }
        /// <summary>
        /// Уникальный идентификатор бана
        /// </summary>
        public string GUID { get; set; }
        /// <summary>
        /// Дата создания бана
        /// </summary>
        public DateTime createdAt { get; set; }
        /// <summary>
        /// Дата окончания бана
        /// </summary>
        public DateTime? endsAt { get; set; }
    }
}
