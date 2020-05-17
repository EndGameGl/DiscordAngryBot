using Discord;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.Bans;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
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
            if (counterReasons.Count() == 5)
            {
                var ban = await ((SocketGuildUser)reason.Author).Ban(1800000, Program.FetchServerObject().server.GetRole(682277138455330821), reason.Channel, true);
                Program.FetchData().bans.Add(ban);
                await ban.SaveBanToDB();
                await Program.FetchServerObject().server.Users.Where(x => x.Id == 261497385274966026).SingleOrDefault().SendMessageAsync($"Выдан автоматический бан юзеру {reason.Author.Username}\n" +
                    $"Причины:\n1. {counterReasons[0].Content}\n2.{counterReasons[1].Content}\n3.{counterReasons[2].Content}\n4.{counterReasons[3].Content}\n5.{counterReasons[4].Content}");
                counterReasons = new List<SocketMessage>();
            }
        }
    }
}
