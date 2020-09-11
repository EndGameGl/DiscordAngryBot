namespace DiscordAngryBot.CustomObjects.DiscordCommands
{
    /// <summary>
    /// Command usage type
    /// </summary>
    public enum CommandType
    {
        /// <summary>
        /// Text command
        /// </summary>
        StringCommand,
        /// <summary>
        /// Command tied to message reaction
        /// </summary>
        EmojiCommand
    }
}
