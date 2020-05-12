using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.Bans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Filters
{
    public static class SwearCounterHandler
    {
        public static async Task<SwearCounter> CreateSwearCounter(this SocketMessage message)
        {
            SwearCounter swearCounter = new SwearCounter()
            {
                author = message.Author,
                reasons = new List<SocketMessage>() { message }
            };
            return swearCounter;
        }

        public async static Task AddReason(this List<SocketMessage> counterReasons, SocketMessage reason)
        {
            counterReasons.Add(reason);
            if (counterReasons.Count() == 3)
            {
                var ban = await ((SocketGuildUser)reason.Author).Ban(300000, Program.FetchServerObject().server.GetRole(682277138455330821), reason.Channel);
                Program.FetchData().bans.Add(ban);
                await ban.SaveBanToDB();                
            }
        }
    }
}
