namespace DiscordAngryBot.APIHandlers
{
    /// <summary>
    /// Class that represents a web page
    /// </summary>
    public class WebPage
    {
        /// <summary>
        /// HTML Doctype
        /// </summary>
        public string Doctype { get; set; }
        /// <summary>
        /// Head content
        /// </summary>
        public string HeadContent { get; set; }
        /// <summary>
        /// Body content
        /// </summary>
        public string BodyContent { get; set; }
        /// <summary>
        /// Build an HTML page with given content
        /// </summary>
        /// <returns></returns>
        public string Build()
        {
            return $"{Doctype}<html><head>{HeadContent}</head><body>{BodyContent}</body></html>";
        }
    }
}
