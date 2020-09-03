namespace DiscordAngryBot.CustomObjects.Groups
{
    /// <summary>
    /// Класс, представляющий собой битву БШ
    /// </summary>
    public class GuildFight : Group
    {
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
