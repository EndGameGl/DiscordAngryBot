using Discord.WebSocket;
using DiscordAngryBot.CustomObjects;
using DiscordAngryBot.CustomObjects.Bans;
using System;

namespace DiscordAngryBot.Models
{
    /// <summary>
    /// Reference object for DiscordBan
    /// </summary>
    public class BanReference : ILoadableInto<DiscordBan>
    {
        /// <summary>
        /// Guild ID
        /// </summary>
        public ulong GuildID { get; set; }
        /// <summary>
        /// Channel ID
        /// </summary>
        public ulong ChannelID { get; set; }
        /// <summary>
        /// Banned user ID
        /// </summary>
        public ulong BanTargetID { get; set; }
        /// <summary>
        /// Ban start date
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Ban end date, if any
        /// </summary>
        public DateTime? EndsAt { get; set; }

        /// <summary>
        /// Class constructor
        /// </summary>
        public BanReference() { }

        /// <summary>
        /// Second class constructor
        /// </summary>
        /// <param name="ban">DiscordBan object</param>
        public BanReference(DiscordBan ban)
        {
            GuildID = ban.Channel.Guild.Id;
            ChannelID = ban.Channel.Id;
            BanTargetID = ban.BanTarget.Id;
            CreatedAt = ban.CreatedAt;
            EndsAt = ban.EndsAt;
        }

        /// <summary>
        /// Loads origin of this reference
        /// </summary>
        /// <returns></returns>
        public DiscordBan LoadOrigin()
        {
            if (BotCore.TryGetExtendedDiscordGuildBotData(GuildID, out var cache))
            {
                DiscordBan ban = new DiscordBan()
                {
                    BanTarget = cache.Guild.GetUser(BanTargetID),
                    Channel = (SocketTextChannel)cache.Guild.GetChannel(ChannelID),
                    CreatedAt = CreatedAt,
                    EndsAt = EndsAt
                };
                return ban;
            }
            else
                return null;
        }
    }
}
