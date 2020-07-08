using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Groups
{
    /// <summary>
    /// Класс, предназначенный для инициализации групп
    /// </summary>
    public static class GroupBuilder
    {
        /// <summary>
        /// Конструктор простой группы
        /// </summary>
        /// <param name="sourceMessage">Сообщение, запустившее конструктор</param>
        /// <param name="args">Параметры</param>
        /// <returns></returns>
        public static async Task<Party> BuildParty(SocketMessage sourceMessage, string[] args)
        {
            Party party = new Party()
            {
                Author = (SocketGuildUser)sourceMessage.Author,
                Channel = (SocketTextChannel)sourceMessage.Channel,
                CreatedAt = DateTime.Now,
                Destination = string.Join(" ", args),
                GUID = Guid.NewGuid().ToString(),
                Users = new List<SocketGuildUser>(),
                UserLimit = 6
            };
            await sourceMessage.DeleteAsync();
            return party;
        }

        /// <summary>
        /// Конструктор рейда
        /// </summary>
        /// <param name="sourceMessage">Сообщение, запустившее конструктор</param>
        /// <param name="args">Параметры</param>
        /// <returns></returns>
        public async static Task<Raid> BuildRaid(SocketMessage sourceMessage, string[] args)
        {
            Raid raid = new Raid()
            {
                Author = (SocketGuildUser)sourceMessage.Author,
                Channel = (SocketTextChannel)sourceMessage.Channel,
                CreatedAt = DateTime.Now,
                Destination = string.Join(" ", args),
                GUID = Guid.NewGuid().ToString(),
                Users = new List<SocketGuildUser>(),
                UserLimit = 12
            };
            await sourceMessage.DeleteAsync();
            return raid;
        }

        /// <summary>
        /// Конструктор битвы БШ
        /// </summary>
        /// <param name="sourceMessage"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async static Task<GuildFight> BuildGuildFight(SocketMessage sourceMessage, string[] args, GuildFightType type)
        {
            GuildFight guildFight = new GuildFight()
            {
                Author = (SocketGuildUser)sourceMessage.Author,
                Channel = (SocketTextChannel)sourceMessage.Channel,
                CreatedAt = DateTime.Now,
                Destination = string.Join(" ", args),
                GUID = Guid.NewGuid().ToString(),
                UserLimit = null,
                Users = new List<SocketGuildUser>(),
                noGearUsers = new List<SocketGuildUser>(),
                unwillingUsers = new List<SocketGuildUser>(),
                unsureUsers = new List<SocketGuildUser>(),
                GuildFightType = type
            };
            await sourceMessage.DeleteAsync();
            return guildFight;
        }

        /// <summary>
        /// Конструктор группы, на основе данных, полученных из базы данных
        /// </summary>
        /// <param name="client">Клиент бота</param>
        /// <param name="GUID">Уникальный идентификатор</param>
        /// <param name="json">JSON-данные группы</param>
        /// <param name="isActive">Признак активности группы</param>
        /// <returns></returns>
        public async static Task<Group> BuildLoadedGroup(SocketGuild guild, string GUID, string json)
        {
            await ConsoleWriter.Write($"Building group {GUID}", ConsoleWriter.InfoType.Notice);
            Group group = await GroupHandler.DeserializeFromJson(guild, json);
            group.GUID = GUID;
            if (group is Party)
            {
                return group as Party;
            }
            else if (group is Raid)
            {
                return group as Raid;
            }
            else
            {
                return group as GuildFight;
            }
        }
    }
}
