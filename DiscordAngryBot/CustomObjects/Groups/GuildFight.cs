using Discord.WebSocket;
using System.Collections.Generic;

namespace DiscordAngryBot.CustomObjects.Groups
{
    public enum GuildFightType
    {
        PR,
        EV
    }
    /// <summary>
    /// Класс, представляющий собой битву БШ
    /// </summary>
    public class GuildFight : Group
    {
        /// <summary>
        /// Список пользователей, которые хотят на гвг, но нет гира
        /// </summary>
        public List<SocketGuildUser> noGearUsers;
        /// <summary>
        /// Список тех, у кого есть гир, но нет желания идти на гвг
        /// </summary>
        public List<SocketGuildUser> unwillingUsers;
        /// <summary>
        /// Список пользователей, не знающих, попадут ли они на гвг
        /// </summary>
        public List<SocketGuildUser> unsureUsers;
        /// <summary>
        /// Тип битвы БШ
        /// </summary>
        public GuildFightType GuildFightType { get; set; }


        /// <summary>
        /// Пустой конструктор битв БШ
        /// </summary>
        public GuildFight() { }
    }
}
