using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Data;
using DiscordAngryBot.CustomObjects.SQLIteHandler;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
using System.Threading;
using System.Diagnostics;

namespace DiscordAngryBot.CustomObjects.Groups
{
    /// <summary>
    /// Класс, содержащий методы расширения Group
    /// </summary>
    public static class GroupHandler
    {
        /// <summary>
        /// Отправить первичное сообщение о сборе группы
        /// </summary>
        /// <param name="group">Группа</param>
        /// <returns></returns>
        public static async Task SendMessage(this Group group)
        {
            StringBuilder messageBuilder = new StringBuilder();
            if (group is Party)
            {
                messageBuilder.Append($"Собирается пати пользователем {group.Author.Mention}: {group.Destination}\n__**Осталось {group.UserLimit - group.Users.Count()} мест**__\nСостав группы:\n");
            }
            else if (group is Raid)
            {
                messageBuilder.Append($"Собирается рейд пользователем {group.Author.Mention}: {group.Destination}\n__**Осталось {group.UserLimit - group.Users.Count()} мест**__\nСостав группы:\n");
            }
            else if (group is GuildFight && ((GuildFight)group).GuildFightType == GuildFightType.PR)
            {
                messageBuilder.Append($"Собирается группа на битвы БШ: {group.Destination}.\nСостав группы:\n");
            }
            else if (group is GuildFight && ((GuildFight)group).GuildFightType == GuildFightType.EV)
            {
                messageBuilder.Append($"Опрос готовности участия в битве БШ: {group.Destination}.\nОтмечаемся!\n");
            } 
            group.TargetMessage = await group.Channel.SendMessageAsync(messageBuilder.ToString());

            await group.TargetMessage.AddReactionAsync(new Emoji("✅")); // галочка           
            if (!(group is GuildFight))
                await group.TargetMessage.AddReactionAsync(new Emoji("\u2757")); // восклицательный знак
            if (group is GuildFight && ((GuildFight)group).GuildFightType == GuildFightType.PR)
            {
                await group.TargetMessage.AddReactionAsync(new Emoji("🐾"));
                await group.TargetMessage.AddReactionAsync(new Emoji("🐷"));
                await group.TargetMessage.AddReactionAsync(new Emoji("❓"));
            }
            else if (group is GuildFight && ((GuildFight)group).GuildFightType == GuildFightType.EV)
            {
                await group.TargetMessage.AddReactionAsync(new Emoji("❎"));
                await group.TargetMessage.AddReactionAsync(new Emoji("☑️"));
                await group.TargetMessage.AddReactionAsync(new Emoji("🇽"));
            }
            await group.TargetMessage.AddReactionAsync(new Emoji("\u274C")); // крестик
        }

        /// <summary>
        /// Отредактировать сообщение в соотвествии с последней версией группы
        /// </summary>
        /// <param name="group">Группа</param>
        /// <returns></returns>
        public static async Task RewriteMessage(this Group group)
        {
            StringBuilder messageBuilder = new StringBuilder();
            if (group is Party)
            {
                messageBuilder.Append($"Собирается пати пользователем {group.Author.Mention}: {group.Destination}\n__**Осталось {group.UserLimit - group.Users.Count()} мест**__\nСостав группы:\n");
            }
            else if (group is Raid)
            {
                messageBuilder.Append($"Собирается рейд пользователем {group.Author.Mention}: {group.Destination}\n__**Осталось {group.UserLimit - group.Users.Count()} мест**__\nСостав группы:\n");
            }
            else if (group is GuildFight && ((GuildFight)group).GuildFightType == GuildFightType.PR)
            {
                messageBuilder.Append($"__**Собирается группа на битвы БШ: {group.Destination} **__\nОтмечаемся!\n");
            }
            else if (group is GuildFight && ((GuildFight)group).GuildFightType == GuildFightType.EV)
            {
                messageBuilder.Append($"Опрос готовности участия в битве БШ: {group.Destination}.\nОтмечаемся!\n");
            }
            if (!(group is GuildFight))
            {
                for (int i = 0; i < group.Users.Where(x => x != null).Count(); i++)
                {
                    messageBuilder.AppendLine($"{i + 1}: {group.Users[i].Mention}");
                }
            }
            else if (group is GuildFight && ((GuildFight)group).GuildFightType == GuildFightType.PR)
            {
                messageBuilder.Append($"**Иду, гир есть**: \u2705\n");
                for (int i = 0; i < ((GuildFight)group).Users.Where(x => x != null).Count(); i++)
                {
                    messageBuilder.AppendLine($"> {i + 1}: {group.Users[i].Mention}");
                }
                messageBuilder.Append($"\n**Иду, но гир слабый/нет вообще**: 🐾\n");
                for (int i = 0; i < ((GuildFight)group).noGearUsers.Where(x => x != null).Count(); i++)
                {
                    messageBuilder.AppendLine($"> {i + 1}: {((GuildFight)group).noGearUsers[i].Mention}");
                }
                messageBuilder.Append($"\n**Могу пойти, если людей не хватит**: 🐷\n");
                for (int i = 0; i < ((GuildFight)group).unwillingUsers.Where(x => x != null).Count(); i++)
                {
                    messageBuilder.AppendLine($"> {i + 1}: {((GuildFight)group).unwillingUsers[i].Mention}");
                }
                messageBuilder.Append($"\n**Пока не уверен**: ❓\n");
                for (int i = 0; i < ((GuildFight)group).unsureUsers.Where(x => x != null).Count(); i++)
                {
                    messageBuilder.AppendLine($"> {i + 1}: {((GuildFight)group).unsureUsers[i].Mention}");
                }
                messageBuilder.Append($"\n**Не могу пойти**: \u274C");
            }
            else if (group is GuildFight && ((GuildFight)group).GuildFightType == GuildFightType.EV)
            {
                messageBuilder.Append($"Основной состав буду: ✅\n");
                for (int i = 0; i < ((GuildFight)group).Users.Where(x => x != null).Count(); i++)
                {
                    messageBuilder.AppendLine($"> {i + 1}: {group.Users[i].Mention}");
                }
                messageBuilder.Append($"\nОсновной состав не смогу: ❎\n");
                for (int i = 0; i < ((GuildFight)group).noGearUsers.Where(x => x != null).Count(); i++)
                {
                    messageBuilder.AppendLine($"> {i + 1}: {((GuildFight)group).noGearUsers[i].Mention}");
                }
                messageBuilder.Append($"\nРезерв готов помочь: ☑️\n");
                for (int i = 0; i < ((GuildFight)group).unwillingUsers.Where(x => x != null).Count(); i++)
                {
                    messageBuilder.AppendLine($"> {i + 1}: {((GuildFight)group).unwillingUsers[i].Mention}");
                }
                messageBuilder.Append($"\nРезерв не смогу: 🇽\n");
                for (int i = 0; i < ((GuildFight)group).unsureUsers.Where(x => x != null).Count(); i++)
                {
                    messageBuilder.AppendLine($"> {i + 1}: {((GuildFight)group).unsureUsers[i].Mention}");
                }
            }
            await group.TargetMessage.ModifyAsync(m => { m.Content = messageBuilder.ToString(); });
        }

        /// <summary>
        /// Редактирование сообщения про отмене сбора группы
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public static async Task RewriteMessageOnCancel(this Group group)
        {
            StringBuilder messageBuilder = new StringBuilder();
            if (group is Party)
            {
                messageBuilder.Append($"Сбор группы {group.Author.Mention} ({group.Destination}) завершен.");
            }
            else if (group is Raid)
            {
                messageBuilder.Append($"Сбор рейда {group.Author.Mention} ({group.Destination}) завершен.");
            }
            else if (group is GuildFight)
            {
                messageBuilder.Append($"Сбор битв БШ ({group.Destination}) завершен.");
            }
            await group.TargetMessage.ModifyAsync(m => { m.Content = messageBuilder.ToString(); });
            await group.TargetMessage.RemoveAllReactionsAsync();
        }

        /// <summary>
        /// Конвертация группы в формат JSON
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public static async Task<string> SerializeToJson(this Group group)
        {
            using (MemoryStream serializationStream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync<GroupJSONObject>(serializationStream, new GroupJSONObject(group));
                var jsonResult = Encoding.UTF8.GetString(serializationStream.ToArray());
                return jsonResult;
            }
        }

        /// <summary>
        /// Создание группы из формата JSON
        /// </summary>
        /// <param name="jsonText">Строка, представляющая файл</param>
        /// <param name="client">Клиент бота</param>
        /// <returns></returns>
        public static async Task<Group> DeserializeFromJson(SocketGuild guild, string jsonText)
        {
            using (MemoryStream serializationStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonText)))
            {
                var groupDataObject = await JsonSerializer.DeserializeAsync<GroupJSONObject>(serializationStream);
                var group = await groupDataObject.ConvertToGroup(guild);
                return group;
            }
        }

        /// <summary>
        /// Сохранение группы в базу данных
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public static async Task SaveToDB(this Group group)
        {
            var jsonString = await group.SerializeToJson();
            string query = $"INSERT INTO Groups (GUID, GroupJSON) VALUES('{group.GUID}', '{jsonString}')";
            await SQLiteDataManager.PushToDB($"locals/Databases/{group.Channel.Guild.Id}/Groups.sqlite", query);
        }

        /// <summary>
        /// Обновление записи группы в базе данных
        /// </summary>
        /// <param name="group">Группа</param>
        /// <returns></returns>
        public static async Task UpdateAtDB(this Group group)
        {
            var jsonString = await group.SerializeToJson();
            string query = $"UPDATE Groups SET GroupJSON = '{jsonString}' WHERE GUID = '{group.GUID}'";
            await SQLiteDataManager.PushToDB($"locals/Databases/{group.Channel.Guild.Id}/Groups.sqlite", query);
        }

        /// <summary>
        /// Удаление группы из базы данных
        /// </summary>
        /// <param name="group">Группа</param>
        /// <returns></returns>
        public static async Task RemoveFromDB(this Group group)
        {
            var jsonString = await group.SerializeToJson();
            string query = $"DELETE FROM Groups WHERE GUID = '{group.GUID}'";
            await SQLiteDataManager.PushToDB($"locals/Databases/{group.Channel.Guild.Id}/Groups.sqlite", query);
        }

        /// <summary>
        /// Загрузка всех активных групп из базы данных
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task<List<Group>> LoadAllGroupsFromDB(SocketGuild guild)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string query = "SELECT * FROM Groups";
            DataTable data = await SQLiteDataManager.GetDataFromDB($"locals/Databases/{guild.Id}/Groups.sqlite", query);
            List<Group> groups = new List<Group>();
            List<Thread> threads = new List<Thread>();
            foreach (DataRow row in data.AsEnumerable())
            {
                Thread loadThread = new Thread(() =>
                {
                    groups.Add(GroupBuilder.BuildLoadedGroup(guild, row["GUID"].ToString(), row["GroupJSON"].ToString()).Result);
                });
                loadThread.Start();
                threads.Add(loadThread);
            }
            foreach (var thread in threads)
            {
                thread.Join();
            }
            sw.Stop();
            await ConsoleWriter.Write($"Group loading took {sw.ElapsedMilliseconds} ms.", ConsoleWriter.InfoType.Notice);
            return groups;
        }

        /// <summary>
        /// Актуализация групп пользователей по реакциям
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task ActualizeReactionsOnGroups(SocketGuild guild)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            await ConsoleWriter.Write($"Actualizing group members", ConsoleWriter.InfoType.Notice);
            List<Thread> threadList = new List<Thread>();
            foreach (var group in BotCore.GetDiscordGuildGroups(guild.Id))
            {
                Thread actThread = new Thread(() =>
                {
                    if (group.TargetMessage != null)
                    {
                        if (!(group is GuildFight))
                        {
                            var usersReacted = group.TargetMessage.GetReactionUsersAsync(new Emoji("\u2705"), (group.UserLimit + 1).Value).ToEnumerable().FirstOrDefault();
                            foreach (var user in usersReacted)
                            {
                                try
                                {
                                    if (user != null && group.Users.Where(x => x.Id == user.Id).Count() == 0 && !user.IsBot)
                                    {
                                        if (group.Users.Count < group.UserLimit)
                                        {
                                            ConsoleWriter.Write($"Adding user {user.Username} at group {group.GUID} in channel {group.Channel.Name}", ConsoleWriter.InfoType.Notice).GetAwaiter().GetResult();
                                            var guildUser = ((SocketGuildChannel)group.Channel).Guild.Users.Where(x => x.Id == user.Id)?.SingleOrDefault();
                                            if (guildUser != null)
                                            {                                                
                                                group.Users.Add(guildUser);
                                                group.UpdateAtDB().GetAwaiter().GetResult();
                                            }            
                                            else
                                                ConsoleWriter.Write($"User {user.Username} at message {group.TargetMessage.Id} in channel {group.Channel.Name} is no longer a server member, check reactions", ConsoleWriter.InfoType.Notice).GetAwaiter().GetResult();
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ConsoleWriter.Write($"{ex.Message}", ConsoleWriter.InfoType.Error).GetAwaiter().GetResult();
                                    continue;
                                }
                            }
                            group.RewriteMessage().GetAwaiter().GetResult();
                        }
                        else
                        {
                            var usersReacted = group.TargetMessage.GetReactionUsersAsync(new Emoji("\u2705"), 30).ToEnumerable().FirstOrDefault();
                            IReadOnlyCollection<IUser> noGearUsersReacted = null;
                            IReadOnlyCollection<IUser> unwillingUsersReacted = null;
                            IReadOnlyCollection<IUser> unsureUsersReacted = null;
                            if (((GuildFight)group).GuildFightType == GuildFightType.PR)
                            {
                                noGearUsersReacted = group.TargetMessage.GetReactionUsersAsync(new Emoji("🐾"), 30).ToEnumerable().FirstOrDefault();
                                unwillingUsersReacted = group.TargetMessage.GetReactionUsersAsync(new Emoji("🐷"), 30).ToEnumerable().FirstOrDefault();
                                unsureUsersReacted = group.TargetMessage.GetReactionUsersAsync(new Emoji("❓"), 30).ToEnumerable().FirstOrDefault();
                            }
                            else
                            {
                                noGearUsersReacted = group.TargetMessage.GetReactionUsersAsync(new Emoji("❎"), 30).ToEnumerable().FirstOrDefault();
                                unwillingUsersReacted = group.TargetMessage.GetReactionUsersAsync(new Emoji("☑️"), 30).ToEnumerable().FirstOrDefault();
                                unsureUsersReacted = group.TargetMessage.GetReactionUsersAsync(new Emoji("🇽"), 30).ToEnumerable().FirstOrDefault();
                            }
                            foreach (var user in unsureUsersReacted)
                            {
                                if (!user.IsBot &&
                                    (group.Users.Where(x => x.Id == user.Id).Count() == 0 &&
                                    ((GuildFight)group).noGearUsers.Where(x => x.Id == user.Id).Count() == 0 &&
                                    ((GuildFight)group).unwillingUsers.Where(x => x.Id == user.Id).Count() == 0) &&
                                    ((GuildFight)group).unsureUsers.Where(x => x.Id == user.Id).Count() == 0)
                                {
                                    ConsoleWriter.Write($"Adding user {user.Username}", ConsoleWriter.InfoType.Notice).GetAwaiter().GetResult();
                                    ((GuildFight)group).unsureUsers.Add(group.Channel.Guild.GetUser(user.Id));
                                    group.UpdateAtDB().GetAwaiter().GetResult();
                                }
                            }
                            foreach (var user in unwillingUsersReacted)
                            {
                                if (!user.IsBot &&
                                    (group.Users.Where(x => x.Id == user.Id).Count() == 0 &&
                                    ((GuildFight)group).noGearUsers.Where(x => x.Id == user.Id).Count() == 0 &&
                                    ((GuildFight)group).unwillingUsers.Where(x => x.Id == user.Id).Count() == 0) &&
                                    ((GuildFight)group).unsureUsers.Where(x => x.Id == user.Id).Count() == 0)
                                {
                                    ConsoleWriter.Write($"Adding user {user.Username}", ConsoleWriter.InfoType.Notice).GetAwaiter().GetResult();
                                    ((GuildFight)group).unwillingUsers.Add(group.Channel.Guild.GetUser(user.Id));
                                    group.UpdateAtDB().GetAwaiter().GetResult();
                                }
                            }
                            foreach (var user in usersReacted)
                            {
                                if (!user.IsBot &&
                                    (group.Users.Where(x => x.Id == user.Id).Count() == 0 &&
                                    ((GuildFight)group).noGearUsers.Where(x => x.Id == user.Id).Count() == 0 &&
                                    ((GuildFight)group).unwillingUsers.Where(x => x.Id == user.Id).Count() == 0) &&
                                    ((GuildFight)group).unsureUsers.Where(x => x.Id == user.Id).Count() == 0)
                                {
                                    ConsoleWriter.Write($"Adding user {user.Username}", ConsoleWriter.InfoType.Notice).GetAwaiter().GetResult();
                                    group.Users.Add(group.Channel.Guild.GetUser(user.Id));
                                    group.UpdateAtDB().GetAwaiter().GetResult();
                                }
                            }
                            foreach (var user in noGearUsersReacted)
                            {
                                if (!user.IsBot &&
                                    (group.Users.Where(x => x.Id == user.Id).Count() == 0 &&
                                    ((GuildFight)group).noGearUsers.Where(x => x.Id == user.Id).Count() == 0 &&
                                    ((GuildFight)group).unwillingUsers.Where(x => x.Id == user.Id).Count() == 0) &&
                                    ((GuildFight)group).unsureUsers.Where(x => x.Id == user.Id).Count() == 0)
                                {
                                    ConsoleWriter.Write($"Adding user {user.Username}", ConsoleWriter.InfoType.Notice).GetAwaiter().GetResult();
                                    ((GuildFight)group).noGearUsers.Add(group.Channel.Guild.GetUser(user.Id));
                                    group.UpdateAtDB().GetAwaiter().GetResult();
                                }
                            }
                            group.RewriteMessage().GetAwaiter().GetResult();
                        }
                    }
                    else
                    {
                        ConsoleWriter.Write("Found broken party, deleting entry...", ConsoleWriter.InfoType.Error).GetAwaiter().GetResult();
                        SQLiteDataManager.PushToDB($"locals/Databases/{group.Channel.Guild.Id}/Groups.sqlite", $"DELETE FROM Groups WHERE GUID = '{group.GUID}'").GetAwaiter().GetResult();
                    }
                });
                actThread.Start();
                threadList.Add(actThread);
            }
            foreach (var thread in threadList)
            {
                thread.Join();
            }
            sw.Stop();
            await ConsoleWriter.Write($"Actualizing group members took {sw.Elapsed.Milliseconds} ms", ConsoleWriter.InfoType.Notice);
        }
    }   
}
