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
                if (BotCore.TryGetDiscordGuildSettings(counter.author.Guild.Id, out var settings))
                {
                    await ((SocketGuildUser)reason.Author).Ban(
                        BanSettings.Default.AutoBanLength,
                        ((SocketGuildUser)reason.Author).Guild.GetRole(settings.BanRoleID.Value),
                        (SocketTextChannel)reason.Channel,
                        true);
                    await BotCore.GetClient().GetUser(261497385274966026).SendMessageAsync(
                        $"Выдан автоматический бан юзеру {reason.Author.Username}\n" +
                        $"Причины:\n" +
                        $"1.{counter.reasons[0].Content}\n" +
                        $"2.{counter.reasons[1].Content}\n" +
                        $"3.{counter.reasons[2].Content}\n" +
                        $"4.{counter.reasons[3].Content}\n" +
                        $"5.{counter.reasons[4].Content}");
                    counter.reasons.Clear();
                }
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
