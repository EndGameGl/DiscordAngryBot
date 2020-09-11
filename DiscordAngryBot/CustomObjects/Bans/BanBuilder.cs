using Discord.WebSocket;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Bans
{
    /// <summary>
    /// Class for building ban objects
    /// </summary>
    public static class BanBuilder
    {
        /// <summary>
        /// Builds new discord ban
        /// </summary>
        /// <param name="target">User to be banned</param>
        /// <param name="time">Time to ban for</param>
        /// <param name="channel">Channel on which ban has occured</param>
        /// <returns></returns>
        public static async Task<DiscordBan> BuildDiscordBan(SocketGuildUser target, int? time, SocketTextChannel channel)
        {
            return await Task.Run(() =>
            {
                DateTime? endTime = null;
                if (time != null)
                {
                    endTime = DateTime.Now.AddMilliseconds((double)time);
                }
                DiscordBan ban = new DiscordBan()
                {
                    Channel = channel,
                    BanTarget = target,
                    GUID = Guid.NewGuid().ToString(),
                    BanTimer = null,
                    CreatedAt = DateTime.Now,
                    EndsAt = endTime
                };
                return ban;
            });
        }

        /// <summary>
        /// Builds discord ban based on DB data
        /// </summary>
        /// <param name="GUID">Ban GUID</param>
        /// <param name="JSON">Ban reference data</param>
        /// <returns></returns>
        public static async Task<DiscordBan> BuildLoadedDiscordBan(string GUID, string JSON)
        {
            return await Task.Run(async () =>
            {
                DiscordBan ban = await BanHandler.DeserializeFromJson(JSON);
                ban.GUID = GUID;
                DateTime currentTime = DateTime.Now;

                if (ban.EndsAt.Value > currentTime)
                {
                    TimeSpan timeLeft = ban.EndsAt.Value - currentTime;
                    ban.BanTimer = new Timer(BanHandler.BanTimerCallBack, ban, (int)timeLeft.TotalMilliseconds, Timeout.Infinite);
                }
                else
                {
                    ban.BanTimer = new Timer(BanHandler.BanTimerCallBack, ban, 0, Timeout.Infinite);
                }
                return ban;
            });
        }
    }
}
