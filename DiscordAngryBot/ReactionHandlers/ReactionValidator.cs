using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.ReactionHandlers
{
    public static class ReactionValidator
    {
        public static bool ValidateReaction(this SocketReaction reaction, Emoji emoji)
        {
            if (reaction.Emote.Name == emoji.Name && !reaction.User.Value.IsBot)
                return true;
            else 
                return false;
        }
    }
}
