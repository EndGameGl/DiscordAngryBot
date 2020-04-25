using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Groups
{
    /// <summary>
    /// Класс, представляющий собой битву БШ
    /// </summary>
    public class GuildFight : Group
    {
        /// <summary>
        /// Список пользователей, которые хотят на гвг, но нет гира
        /// </summary>
        public List<SocketUser> noGearUsers;
        /// <summary>
        /// Список тех, у кого есть гир, но нет желания идти на гвг
        /// </summary>
        public List<SocketUser> unwillingUsers;
        /// <summary>
        /// Список пользователей, не знающих, попадут ли они на гвг
        /// </summary>
        public List<SocketUser> unsureUsers;

        /// <summary>
        /// Пустой конструктор битв БШ
        /// </summary>
        public GuildFight() { }
    }
}
