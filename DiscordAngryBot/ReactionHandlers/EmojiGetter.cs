using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.ReactionHandlers
{
    public static class EmojiGetter
    {
        public static Emoji GetEmoji(Emojis emoji)
        {
            switch (emoji)
            {
                case Emojis.WhiteCheckMark:
                    return new Emoji("\u2705");
                case Emojis.CrossMark:
                    return new Emoji("\u274C");
                case Emojis.ExclamationMark:
                    return new Emoji("\u2757");
                case Emojis.Feet:
                    return new Emoji("🐾");
                case Emojis.Pig:
                    return new Emoji("🐷");
                case Emojis.QuestionMark:
                    return new Emoji("❓");
                case Emojis.NegativeSquaredCrossMark:
                    return new Emoji("❎");
                case Emojis.BallotBoxWithCheck:
                    return new Emoji("☑️");
                case Emojis.RegionalIndicatorX:
                    return new Emoji("🇽");
                default:
                    throw new Exception("Wrong emoji");
            }
        }
    }
}
