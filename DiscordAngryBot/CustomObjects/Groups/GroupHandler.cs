using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Data;
using DiscordAngryBot.CustomObjects.SQLIteHandler;
using System.Threading;
using System.Diagnostics;
using DiscordAngryBot.ReactionHandlers;
using System;
using DiscordAngryBot.Models;
using Debug = DiscordAngryBot.CustomObjects.ConsoleOutput.Debug;

namespace DiscordAngryBot.CustomObjects.Groups
{
    /// <summary>
    /// Класс, содержащий методы расширения Group
    /// </summary>
    public static class GroupHandler
    {
        private static string FormMessage(Group group)
        {
            Debug.Log($"Forming message...").GetAwaiter().GetResult();
            StringBuilder messageBuilder = new StringBuilder();
            Debug.Log($"Checking group type: {group.Type}").GetAwaiter().GetResult();
            switch (group.Type)
            {
                case GroupType.Simple:
                    messageBuilder.Append(
                        $"Собирается группа пользователем {group.Author.Mention}: {group.Destination}\n" +
                        $"__**Осталось {group.UserLists[0].UserLimit - group.UserLists[0].Users.Count()} мест**__\n");
                    break;
                case GroupType.GuildFight:
                    messageBuilder.Append($"Собирается группа на битвы БШ пользователем {group.Author.Mention}: {group.Destination}\n");
                    break;
                case GroupType.Poll:
                    messageBuilder.Append($"Пользователь {group.Author.Mention} предлагает проголосовать:\n");
                    break;
            }
            Debug.Log($"Iterating user lists...").GetAwaiter().GetResult();
            foreach (var list in group.UserLists)
            {
                Debug.Log($"Writing list: {list.ListName}").GetAwaiter().GetResult();
                messageBuilder.Append($"{list.ListName} ({list.ListEmoji.Name}):\n");
                Debug.Log($"Iteraring users in list...").GetAwaiter().GetResult();
                foreach (var user in list.Users)
                {
                    Debug.Log($"Writing user: {user.Mention}").GetAwaiter().GetResult();
                    messageBuilder.Append($"> {user.Mention}\n");
                }
            }
            Debug.Log($"Finished forming message.").GetAwaiter().GetResult();
            return messageBuilder.ToString();
        }
        /// <summary>
        /// Отправить первичное сообщение о сборе группы
        /// </summary>
        /// <param name="group">Группа</param>
        /// <returns></returns>
        public static async Task SendMessage(this Group group)
        {
            group.TargetMessage = await group.Channel.SendMessageAsync(FormMessage(group));
            var emojis = group.UserLists.Select(x => x.ListEmoji).ToList();
            emojis.Add(EmojiGetter.GetEmoji(Emojis.ExclamationMark));
            emojis.Add(EmojiGetter.GetEmoji(Emojis.CrossMark));
            await group.TargetMessage.AddReactionsAsync(emojis.ToArray());
        }

        /// <summary>
        /// Отредактировать сообщение в соотвествии с последней версией группы
        /// </summary>
        /// <param name="group">Группа</param>
        /// <returns></returns>
        public static async Task RewriteMessage(this Group group)
        {
            await Debug.Log($"Preparing to modify message: {group.TargetMessage.Id}");
            await group.TargetMessage.ModifyAsync(m => { m.Content = FormMessage(group); });
            await Debug.Log($"Message modifying finished.");
        }

        /// <summary>
        /// Редактирование сообщения про отмене сбора группы
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public static async Task RewriteMessageOnCancel(this Group group)
        {
            switch (group.Type)
            {
                case GroupType.Simple:
                    await group.TargetMessage.ModifyAsync(m => 
                    {
                        m.Content = $"Сбор группы {group.Author.Mention} ({group.Destination}) завершен."; 
                    });
                    break;
                case GroupType.GuildFight:
                    await group.TargetMessage.ModifyAsync(m => 
                    { 
                        m.Content = $"Сбор битв БШ ({group.Destination}) завершен."; 
                    });
                    break;
                case GroupType.Poll:
                    break;
            }
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
                await JsonSerializer.SerializeAsync(serializationStream, group.GetReference());
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
        public static async Task<Group> DeserializeFromJson(string jsonText)
        {
            using (MemoryStream serializationStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonText)))
            {
                var groupDataObject = await JsonSerializer.DeserializeAsync<GroupReference>(serializationStream);
                var group = groupDataObject.LoadOrigin();
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
            await ConsoleOutput.Debug.Log($"Group loading took {sw.ElapsedMilliseconds} ms.", ConsoleOutput.Debug.InfoType.Notice);
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
            await ConsoleOutput.Debug.Log($"Actualizing group members", ConsoleOutput.Debug.InfoType.Notice);
            List<Thread> threadList = new List<Thread>();
            if (BotCore.TryGetDiscordGuildGroups(guild.Id, out List<Group> groups))
            {
                foreach (var group in groups)
                {
                    Thread actThread = new Thread(() =>
                    {
                        if (group.TargetMessage != null)
                        {
                            bool groupHasChanges = false;
                            foreach (var list in group.UserLists)
                            {
                                var usersReacted = group.TargetMessage.GetReactionUsersAsync(list.ListEmoji, 20).ToEnumerable().FirstOrDefault();
                                foreach (var user in usersReacted)
                                {
                                    if (group.UserLists.Where(x => x.Users.Where(q => q.Id == user.Id).Count() == 0).Count() == 0)
                                    {
                                        if (list.UserLimit.HasValue)
                                        {
                                            if (list.Users.Count() < list.UserLimit)
                                            {
                                                list.Users.Add(guild.GetUser(user.Id));
                                                groupHasChanges = true;
                                            }
                                        }
                                        else
                                        {
                                            list.Users.Add(guild.GetUser(user.Id));
                                            groupHasChanges = true;
                                        }
                                    }
                                }
                            }
                            if (groupHasChanges)
                                group.RewriteMessage().GetAwaiter().GetResult();
                        }
                        else
                        {
                            ConsoleOutput.Debug.Log("Found broken party, deleting entry...", ConsoleOutput.Debug.InfoType.Error).GetAwaiter().GetResult();
                            SQLiteDataManager.PushToDB($"locals/Databases/{group.Channel.Guild.Id}/Groups.sqlite", $"DELETE FROM Groups WHERE GUID = '{group.GUID}'").GetAwaiter().GetResult();
                        }
                    });
                    actThread.Start();
                    threadList.Add(actThread);
                }
            }
            foreach (var thread in threadList)
            {
                thread.Join();
            }
            sw.Stop();
            await ConsoleOutput.Debug.Log($"Actualizing group members took {sw.Elapsed.Milliseconds} ms", ConsoleOutput.Debug.InfoType.Notice);
        }

        public static bool TryGetUserList(this Group group, Emoji emoji, out UserList userList)
        {
            userList = group.UserLists.FirstOrDefault(x => x.ListEmoji.Name == emoji.Name);           
            if (userList != null)
                return true;
            else
                return false;
        }

        public static bool TryFindGroup(this List<Group> groups, ulong groupID, out Group group)
        {
            group = groups.FirstOrDefault(x => x.TargetMessage.Id == groupID);
            if (group != null)
                return true;
            else
                return false;
        }
    }   
}
