namespace DiscordAngryBot.CustomObjects.Groups
{
    /// <summary>
    /// Тип группы
    /// </summary>
    public enum GroupType
    {
        /// <summary>
        /// Простая группа (сбор людей в пати)
        /// </summary>
        Simple,
        /// <summary>
        /// Битва БШ (Пресет, но требует типа для создания)
        /// </summary>
        GuildFight,
        /// <summary>
        /// Голосование
        /// </summary>
        Poll
    }
}
