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
        public ulong channel_id { get; set; }
        /// <summary>
        /// Юзер, у которого забрана роль
        /// </summary>
        public ulong banTarget_id { get; set; }
        /// <summary>
        /// Идентификатор роли, которую забрали
        /// </summary>
        public ulong role_id { get; set; }
        /// <summary>
        /// Дата выдачи бана
        /// </summary>
        public DateTime createdAt { get; set; }
        /// <summary>
        /// Дата окончания бана
        /// </summary>
        public DateTime? endsAt { get; set; }

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
            channel_id = ban.channel.Id;
            banTarget_id = ban.banTarget.Id;
            role_id = ban.roleToBan.Id;
            createdAt = ban.createdAt;
            endsAt = ban.endsAt;
        }

        /// <summary>
        /// Конвертация JSON-бана в DiscordBan
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        public async Task<DiscordBan> ConvertToDiscordBan(SocketGuild guild)
        {
            await ConsoleWriter.Write($"Converting into DiscordBan", ConsoleWriter.InfoType.Notice);
            DiscordBan ban = new DiscordBan()
            {
                banTarget = guild.GetUser(banTarget_id),
                channel = (ISocketMessageChannel)guild.GetChannel(channel_id),
                createdAt = createdAt,
                endsAt = endsAt,
                roleToBan = guild.GetRole(role_id)
            };
            return ban;
        }
    }
}
