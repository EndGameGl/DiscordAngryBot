using Discord;
using System;

namespace DiscordAngryBot.ReactionHandlers
{
    /// <summary>
    /// Class for simple emoji fetching
    /// </summary>
    public static class EmojiGetter
    {
        /// <summary>
        /// Get emoji by enum value
        /// </summary>
        /// <param name="emoji">Emoji</param>
        /// <returns></returns>
        public static Emoji GetEmoji(Emojis emoji)
        {
            return emoji switch
            {
                Emojis.WhiteCheckMark => new Emoji("\u2705"),
                Emojis.CrossMark => new Emoji("\u274C"),
                Emojis.ExclamationMark => new Emoji("\u2757"),
                Emojis.Feet => new Emoji("🐾"),
                Emojis.Pig => new Emoji("🐷"),
                Emojis.QuestionMark => new Emoji("❓"),
                Emojis.NegativeSquaredCrossMark => new Emoji("❎"),
                Emojis.BallotBoxWithCheck => new Emoji("☑️"),
                Emojis.RegionalIndicatorX => new Emoji("🇽"),
                _ => throw new Exception("Wrong emoji"),
            };
        }
    }
}
