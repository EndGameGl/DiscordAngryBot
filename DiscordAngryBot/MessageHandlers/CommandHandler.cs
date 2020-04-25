﻿using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.Bans;
using DiscordAngryBot.CustomObjects.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordAngryBot.MessageHandlers
{
    /// <summary>
    /// Класс для обработки команд
    /// </summary>
    public static class CommandHandler
    {
        /// <summary>
        /// Метод для разбора параметров команды
        /// </summary>
        /// <param name="message">Команда</param>
        /// <returns></returns>
        public static Tuple<ISocketMessageChannel, string, string[]> ProcessCommandMessage(SocketMessage message)
        {
            var channel = message.Channel;
            string command = message.Content.Substring(1);
            var splitMes = command.Split(' ');
            command = splitMes[0];
            string[] args = new string[10];
            if ((splitMes.Length - 1) > 0)
            {
                args = new string[splitMes.Length - 1];
                for (int i = 0; i < (splitMes.Length - 1); i++)
                {
                    args[i] = splitMes[i + 1];
                }
            }           
            return Tuple.Create(channel, command, args);
        }

        /// <summary>
        /// Класс для обработки администраторских команд
        /// </summary>
        public static class SystemCommands
        {
            public async static Task<DiscordBan> BanUser(DiscordServerObject serverObject, SocketMessage message, Tuple<ISocketMessageChannel, string, string[]> mainArgs)
            {
                var targetID = mainArgs.Item3[0].Substring(3, mainArgs.Item3[0].Length - 4);
                var targetUser = serverObject.users.Where(x => x.Id.ToString() == targetID).Single();
                int? time = null;
                if (mainArgs.Item3.Count() > 1)
                {
                    time = Convert.ToInt32(mainArgs.Item3[1]) * 60 * 1000;
                }
                var ban = await targetUser.Ban(time, serverObject.server.GetRole(682277138455330821), message.Channel);
                await ban.SaveBanToDB();
                return ban;
            }
        }

        /// <summary>
        /// Класс для обработки пользовательских команд
        /// </summary>
        public static class UserCommands
        {
            /// <summary>
            /// Операция создания группы
            /// </summary>
            /// <param name="groups">Список всех групп</param>
            /// <param name="message">Сообщение, инициировавшее создание</param>
            /// <param name="destination">Описание группы</param>
            /// <returns></returns>
            public static async Task CreateParty(List<Group> groups, SocketMessage message, string[] destination)
            {
                Party party = await GroupBuilder.BuildParty(message, destination);
                groups.Add(party);
                await party.SendMessage();
                await party.SaveToDB();
            }

            /// <summary>
            /// Операция создания рейда
            /// </summary>
            /// <param name="groups">Список всех групп</param>
            /// <param name="message">Сообщение, инициировавшее создание</param>
            /// <param name="destination">Описание рейда</param>
            /// <returns></returns>
            public static async Task CreateRaid(List<Group> groups, SocketMessage message, string[] destination)
            {
                Raid raid = await GroupBuilder.BuildRaid(message, destination);
                groups.Add(raid);
                await raid.SendMessage();
                await raid.SaveToDB();
            }

            /// <summary>
            /// Операция создания битвы БШ
            /// </summary>
            /// <param name="groups"></param>
            /// <param name="message"></param>
            /// <param name="destination"></param>
            /// <returns></returns>
            public static async Task CreateGuildFight(List<Group> groups, SocketMessage message, string[] destination)
            {
                GuildFight guildFight = await GroupBuilder.BuildGuildFight(message, destination);
                groups.Add(guildFight);
                await guildFight.SendMessage();
                await guildFight.SaveToDB();
                await guildFight.RewriteMessage();
            }

            /// <summary>
            /// Операция вывода списка групп
            /// </summary>
            /// <param name="message">Сообщение, инициировавшее вызов списка</param>
            /// <param name="groups">Список всех групп</param>
            /// <returns></returns>
            public static async Task ListGroups(SocketMessage message, List<Group> groups)
            {
                StringBuilder text = new StringBuilder();
                
                if (groups.Count() == 0)
                {
                    text.AppendLine("В данный момент нет никаких групп или рейдов.");
                }                            
                else
                {
                    List<Party> parties = new List<Party>();
                    List<Raid> raids = new List<Raid>();
                    foreach (var group in groups)
                    {
                        if (group is Party)
                        {
                            parties.Add((Party)group);
                        }
                        else if (group is Raid)
                        {
                            raids.Add((Raid)group);
                        }
                    }
                    text.AppendLine("Список собираемых в данный момент составов:");
                    if (parties.Count() > 0)
                        text.AppendLine("Группы:");
                    foreach (var party in parties)
                    {
                        text.AppendLine($"    {party.author.Username}: {party.destination} ({party.users.Count} участников)");
                    }
                    if (raids.Count() > 0)
                        text.AppendLine("Рейды:");
                    foreach (var raid in raids)
                    {
                        text.AppendLine($"    {raid.author.Username}: {raid.destination} ({raid.users.Count} участников)");
                    }
                }
                await message.Channel.SendMessageAsync(text.ToString());
                await message.DeleteAsync();
            }                   
        }
    }
}
