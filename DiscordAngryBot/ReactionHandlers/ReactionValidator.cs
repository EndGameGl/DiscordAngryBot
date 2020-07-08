using Discord;
using Discord.WebSocket;

namespace DiscordAngryBot.ReactionHandlers
{
    /// <summary>
    /// Класс для валидации реакций
    /// </summary>
    public static class ReactionValidator
    {
        /// <summary>
        /// Провести валидацию реакции
        /// </summary>
        /// <param name="reaction"></param>
        /// <param name="emoji"></param>
        /// <returns></returns>
        public static bool ValidateReaction(this SocketReaction reaction, Emoji emoji)
        {
            if (reaction.Emote.Name == emoji.Name && !reaction.User.Value.IsBot)
                return true;
            else 
                return false;
        }
    }
}
