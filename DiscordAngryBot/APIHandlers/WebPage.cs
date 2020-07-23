namespace DiscordAngryBot.APIHandlers
{
    /// <summary>
    /// Класс для построения веб-страницы
    /// </summary>
    public class WebPage
    {
        /// <summary>
        /// Тип документа HTML
        /// </summary>
        public string Doctype { get; set; }
        /// <summary>
        /// Содержимое тега head
        /// </summary>
        public string HeadContent { get; set; }
        /// <summary>
        /// Содержимое тега body
        /// </summary>
        public string BodyContent { get; set; }
        /// <summary>
        /// Сборка веб-страницы из данных
        /// </summary>
        /// <returns></returns>
        public string Build()
        {
            return $"{Doctype}<html><head>{HeadContent}</head><body>{BodyContent}</body></html>";
        }
    }
}
