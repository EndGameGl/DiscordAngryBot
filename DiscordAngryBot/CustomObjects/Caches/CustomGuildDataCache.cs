using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.Bans;
using DiscordAngryBot.CustomObjects.Filters;
using DiscordAngryBot.CustomObjects.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Caches
{
    /// <summary>
    /// Кэш гильдии с кастомными объектами
    /// </summary>
    public class CustomGuildDataCache
    {
        /// <summary>
        /// Кэш гильдии
        /// </summary>
        public SocketGuild Guild { get; set; }
        /// <summary>
        /// Группы, собирыемые в гильдии
        /// </summary>
        public List<Group> Groups { get; set; }
        /// <summary>
        /// Активные баны гильдии
        /// </summary>
        public List<DiscordBan> Bans { get; set; }
        /// <summary>
        /// Счетчики мата гильдии
        /// </summary>
        public List<SwearCounter> SwearCounters { get; set; }
        /// <summary>
        /// Настройка бота в пределах гильдии
        /// </summary>
        public DiscordGuildSettings Settings { get; set; }
        /// <summary>
        /// Признак доступности гильдии
        /// </summary>
        public bool IsAvailable { get; set; }

    }
}
