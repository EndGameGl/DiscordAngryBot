using Discord;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.Bans;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Filters
{
    /// <summary>
    /// Обработчик событий, связанных с счетчиками матов
    /// </summary>
    public static class SwearCounterHandler
    {
        public static async Task<SwearCounter> CreateSwearCounter(this SocketMessage message)
        {
            if (message.Channel is SocketGuildChannel)
            {
                SwearCounter swearCounter = null;
                await Task.Run(() =>
                {
                    swearCounter = new SwearCounter()
                    {
                        author = (SocketGuildUser)message.Author,
                        reasons = new List<SocketMessage>() { message },
                    };
                });
                return swearCounter;
            }
            else return null;
        }

        public async static Task AddReason(this SwearCounter counter, SocketMessage reason)
        {
            counter.reasons.Add(reason);
            if (counter.reasons.Count() == 5)
            {
                await ((SocketGuildUser)reason.Author).Ban(
                    1800000, 
                    ((SocketGuildUser)reason.Author).Guild.GetRole(BotCore.GetDiscordGuildSettings(counter.author.Guild.Id).BanRoleID.Value), 
                    (SocketTextChannel)reason.Channel, 
                    true);
                await BotCore.GetClient().GetUser(261497385274966026).SendMessageAsync($"Выдан автоматический бан юзеру {reason.Author.Username}\n" +
                    $"Причины:\n1. {counter.reasons[0].Content}\n2.{counter.reasons[1].Content}\n3.{counter.reasons[2].Content}\n4.{counter.reasons[3].Content}\n5.{counter.reasons[4].Content}");
                counter.reasons = new List<SocketMessage>();
            }
        }

        public static async Task RunSwearFilter(this SocketMessage message)
        {
                var userSwearCounters = BotCore.GetDiscordGuildSwearCounters(((SocketTextChannel)message.Channel).Guild.Id).Where(x => x.author == message.Author);
                if (await message.CheckPhrase() && userSwearCounters.Count() == 0)
                {
                    BotCore.GetDiscordGuildSwearCounters(((SocketTextChannel)message.Channel).Guild.Id).Add(await message.CreateSwearCounter());

                }
                else if (await message.CheckPhrase() && userSwearCounters.Count() == 1)
                {
                    await userSwearCounters.SingleOrDefault().AddReason(message);
                }
            
        }
    }
}
