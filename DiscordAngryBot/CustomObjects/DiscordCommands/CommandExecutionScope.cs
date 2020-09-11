using System;

namespace DiscordAngryBot.CustomObjects.DiscordCommands
{
    /// <summary>
    /// Command execution scopes
    /// </summary>
    [Flags]
    public enum CommandExecutionScope
    {
        /// <summary>
        /// Server-only command
        /// </summary>
        Server = 1,
        /// <summary>
        /// DM command
        /// </summary>
        DM = 2,
        /// <summary>
        /// General command
        /// </summary>
        All = 4
    }
}
