namespace DiscordAngryBot.CustomObjects.Groups
{
    /// <summary>
    /// Extended group for guild fights
    /// </summary>
    public class GuildFight : Group
    {
        /// <summary>
        /// Guiyld fight type
        /// </summary>
        public GuildFightType GuildFightType { get; set; }

        /// <summary>
        /// Class constructor
        /// </summary>
        public GuildFight() { }
    }
}
