using Discord;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.Bans;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Filters
{
    /// <summary>
    /// Class for handling swear counters
    /// </summary>
    public static class SwearCounterHandler
    {
        /// <summary>
        /// Create new swear handler
        /// </summary>
        /// <param name="message">Initial message</param>
        /// <returns></returns>
        public static async Task<SwearCounter> CreateSwearCounter(this SocketMessage message)
        {
            if (message.Channel is SocketGuildChannel)
            {
                SwearCounter swearCounter = null;
                await Task.Run(() =>
                {
                    swearCounter = new SwearCounter()
                    {
                        Author = (SocketGuildUser)message.Author,
                        Reasons = new List<SocketMessage>() { message },
                    };
                });
                return swearCounter;
            }
            else return null;
        }

        /// <summary>
        /// Add new reason to swear counter
        /// </summary>
        /// <param name="counter">Counter to add to</param>
        /// <param name="reason">New swear message</param>
        /// <returns></returns>
        public async static Task AddReason(this SwearCounter counter, SocketMessage reason)
        {
            counter.Reasons.Add(reason);
            if (counter.Reasons.Count() == 5)
            {
                if (BotCore.TryGetDiscordGuildSettings(counter.Author.Guild.Id, out var settings))
                {
                    await ((SocketGuildUser)reason.Author).Ban(
                        BanSettings.Default.AutoBanLength,
                        ((SocketGuildUser)reason.Author).Guild.GetRole(settings.BanRoleID.Value),
                        (SocketTextChannel)reason.Channel,
                        true);
                    await BotCore.GetClient().GetUser(261497385274966026).SendMessageAsync(
                        $"Выдан автоматический бан юзеру {reason.Author.Username}\n" +
                        $"Причины:\n" +
                        $"1.{counter.Reasons[0].Content}\n" +
                        $"2.{counter.Reasons[1].Content}\n" +
                        $"3.{counter.Reasons[2].Content}\n" +
                        $"4.{counter.Reasons[3].Content}\n" +
                        $"5.{counter.Reasons[4].Content}");
                    counter.Reasons.Clear();
                }
            }
        }

        /// <summary>
        /// Runs swear filter for this message
        /// </summary>
        /// <param name="message">Target message</param>
        /// <returns></returns>
        public static async Task RunSwearFilter(this SocketMessage message)
        {
                var userSwearCounters = BotCore.GetDiscordGuildSwearCounters(((SocketTextChannel)message.Channel).Guild.Id).Where(x => x.Author == message.Author);
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
