using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Bans
{
    /// <summary>
    /// Объект для десериализации бана дискорда
    /// </summary>
    public class BanJSONobject
    {
        /// <summary>
        /// Идентификатор канала, где был выдан бан
        /// </summary>
        public ulong _ChannelID { get; set; }
        /// <summary>
        /// Юзер, у которого забрана роль
        /// </summary>
        public ulong _BanTargetID { get; set; }
        /// <summary>
        /// Дата выдачи бана
        /// </summary>
        public DateTime _CreatedAt { get; set; }
        /// <summary>
        /// Дата окончания бана
        /// </summary>
        public DateTime? _EndsAt { get; set; }

        /// <summary>
        /// Пустой конструктор бана
        /// </summary>
        public BanJSONobject() { }

        /// <summary>
        /// Конструктор бана на основе объекта DiscordBan
        /// </summary>
        /// <param name="ban"></param>
        public BanJSONobject(DiscordBan ban)
        {
            _ChannelID = ban.Channel.Id;
            _BanTargetID = ban.BanTarget.Id;
            _CreatedAt = ban.CreatedAt;
            _EndsAt = ban.EndsAt;
        }

        /// <summary>
        /// Конвертация JSON-бана в DiscordBan
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        public async Task<DiscordBan> ConvertToDiscordBan(SocketGuild guild)
        {
            return await Task.Run(() =>
            {
                DiscordBan ban = new DiscordBan()
                {
                    BanTarget = guild.GetUser(_BanTargetID),
                    Channel = (SocketTextChannel)guild.GetChannel(_ChannelID),
                    CreatedAt = _CreatedAt,
                    EndsAt = _EndsAt              
                };
                return ban;
            });
        }
    }
}
