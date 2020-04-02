using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.Parties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordAngryBot.MessageHandlers
{
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
        public static class SystemCommands
        {
            /// <summary>
            /// Метод временного бана юзера
            /// </summary>
            /// <param name="serverObject"></param>
            /// <param name="timers"></param>
            /// <param name="message"></param>
            /// <param name="channel"></param>
            /// <param name="command"></param>
            /// <param name="args"></param>
            public async static void TempBanUser(DiscordServerObject serverObject, List<Timer> timers, SocketMessage message, Tuple<ISocketMessageChannel, string, string[]> mainArgs)
            {
                Program.Write($"Called command: {mainArgs.Item2}\n    Arguments: {mainArgs.Item3[0]}, {mainArgs.Item3[1]}");
                var targetID = mainArgs.Item3[0].Substring(3, mainArgs.Item3[0].Length - 4);
                var targetUser = serverObject.users.Where(x => x.Id.ToString() == targetID).Single();
                var time = Convert.ToInt32(mainArgs.Item3[1]) * 60 * 1000;
                Program.Write(serverObject.server.GetRole(682277138455330821).Name);
                Program.Write($"Removing role {serverObject.server.GetRole(682277138455330821)} from {targetUser.Username}");
                await targetUser.RemoveRoleAsync(serverObject.server.GetRole(682277138455330821));
                await mainArgs.Item1.SendMessageAsync($"Выдан бан пользователю {targetUser.Username} на {mainArgs.Item3[1]} минут");

                timers.Add(new Timer(TimerCallBackMethod, timers.Count, time, Timeout.Infinite));

                async void TimerCallBackMethod(object timerID)
                {
                    Program.Write($"Adding role back to {targetUser.Username}");
                    await targetUser.AddRoleAsync(serverObject.server.GetRole(682277138455330821));
                    await mainArgs.Item1.SendMessageAsync($"Возвращена роль {serverObject.server.GetRole(682277138455330821).Name} пользователю {targetUser.Username}");
                    timers[(int)timerID].Change(Timeout.Infinite, Timeout.Infinite);
                    timers[(int)timerID].Dispose();
                    timers[(int)timerID] = null;
                }
            }
        }
        public static class UserCommands
        {
            public static async void CreateParty(SocketMessage message, Tuple<ISocketMessageChannel, string, string[]> mainArgs)
            {
                Party createdParty = new Party(message, mainArgs.Item3);
                createdParty.SendMessage();
                Program.systemData.parties.Add(createdParty);
                await message.DeleteAsync();

            }
            public static async void CreateRaid(SocketMessage message, Tuple<ISocketMessageChannel, string, string[]> mainArgs)
            {
                Raid createdRaid = new Raid(message, mainArgs.Item3);
                createdRaid.SendMessage();
                Program.systemData.raids.Add(createdRaid);
                await message.DeleteAsync();
            }
            public static async void ListGroups(SocketMessage message)
            {
                StringBuilder text = new StringBuilder(); if (Program.systemData.parties.Count() == 0 && Program.systemData.raids.Count() == 0)
                {
                    text.AppendLine("В данный момент нет никаких групп или рейдов.");
                }                            
                else
                {
                    text.AppendLine("Список собираемых в данный момент составов:");
                    if (Program.systemData.parties.Count() > 0)
                        text.AppendLine("Группы:");
                    foreach (var party in Program.systemData.parties)
                    {
                        text.AppendLine($"    {party.author.Username}: {party.partyDestination} ({party.users.Count} участников)");
                    }
                    if (Program.systemData.raids.Count() > 0)
                        text.AppendLine("Рейды:");
                    foreach (var raid in Program.systemData.raids)
                    {
                        text.AppendLine($"    {raid.author.Username}: {raid.partyDestination} ({raid.users.Count} участников)");
                    }
                }
                await message.Channel.SendMessageAsync(text.ToString());
                await message.DeleteAsync();
            }                   
        }
    }
}
