using System;

namespace DiscordAngryBot.CustomObjects.DiscordCommands
{
    /// <summary>
    /// Command user usage scopes
    /// </summary>
    [Flags]
    public enum CommandScope
    {
        /// <summary>
        /// Users can use this
        /// </summary>
        User = 1,
        /// <summary>
        /// Only-admin use
        /// </summary>
        Admin = 2
    }
}
