using Discord.WebSocket;
using DiscordAngryBot.CustomObjects;
using DiscordAngryBot.CustomObjects.Bans;
using System;

namespace DiscordAngryBot.Models
{
    /// <summary>
    /// Объект для десериализации бана дискорда
    /// </summary>
    public class BanReference : ILoadableInto<DiscordBan>
    {
        public ulong GuildID { get; set; }
        /// <summary>
        /// Идентификатор канала, где был выдан бан
        /// </summary>
        public ulong ChannelID { get; set; }
        /// <summary>
        /// Юзер, у которого забрана роль
        /// </summary>
        public ulong BanTargetID { get; set; }
        /// <summary>
        /// Дата выдачи бана
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Дата окончания бана
        /// </summary>
        public DateTime? EndsAt { get; set; }

        /// <summary>
        /// Пустой конструктор бана
        /// </summary>
        public BanReference() { }

        /// <summary>
        /// Конструктор бана на основе объекта DiscordBan
        /// </summary>
        /// <param name="ban"></param>
        public BanReference(DiscordBan ban)
        {
            GuildID = ban.Channel.Guild.Id;
            ChannelID = ban.Channel.Id;
            BanTargetID = ban.BanTarget.Id;
            CreatedAt = ban.CreatedAt;
            EndsAt = ban.EndsAt;
        }

        public DiscordBan LoadOrigin()
        {
            if (BotCore.TryGetGuildDataCache(GuildID, out var cache))
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
