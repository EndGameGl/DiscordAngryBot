using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Filters
{
    public static class SwearFilter
    {
        public static async Task<bool> CheckPhrase(this SocketMessage message)
        {
            bool doesContainSwear = false;
            if (message.Content.Count() != 0)
            {
                foreach (var word in Program.FetchSettings().swearFilterWords)
                {
                    if (message.Content.Contains(word))
                    {
                        doesContainSwear = true;
                    }
                }
            }
            return doesContainSwear;
        }
    }
}
