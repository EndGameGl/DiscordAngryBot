using System.Collections.Generic;

namespace DiscordAngryBot.CustomObjects.DiscordCommands
{
    /// <summary>
    /// IEqualityComparer implementation for discord commands
    /// </summary>
    class DiscordCommandComparer : IEqualityComparer<DiscordCommand>
    {
        /// <summary>
        /// Whether one command equals another
        /// </summary>
        /// <param name="x">First command</param>
        /// <param name="y">Second command</param>
        /// <returns></returns>
        public bool Equals(DiscordCommand x, DiscordCommand y)
        {
            return x.CommandMetadata.CommandName.Equals(y.CommandMetadata.CommandName);
        }

        /// <summary>
        /// Gets discord command hashcode
        /// </summary>
        /// <param name="obj">Command to get hashcode from</param>
        /// <returns></returns>
        public int GetHashCode(DiscordCommand obj)
        {
            return obj.CommandMetadata.CommandName.GetHashCode();
        }
    }
}
