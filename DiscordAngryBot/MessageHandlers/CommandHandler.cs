using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.Groups;
using System;
using System.Collections.Generic;
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
            public async static Task TempBanUser(DiscordServerObject serverObject, List<Timer> timers, SocketMessage message, Tuple<ISocketMessageChannel, string, string[]> mainArgs)
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
            public static async Task CreateParty(List<Group> groups, SocketMessage message, string[] destination)
            {
                Party party = await GroupBuilder.BuildParty(message, destination);
                groups.Add(party);
                await party.SendMessage();
                await party.SaveToDB();
            }
            public static async Task CreateRaid(List<Group> groups, SocketMessage message, string[] destination)
            {
                Raid raid = await GroupBuilder.BuildRaid(message, destination);
                groups.Add(raid);
                await raid.SendMessage();
            }
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
