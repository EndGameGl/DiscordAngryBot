using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Data.SQLite;
using System.Data;
using DiscordAngryBot.CustomObjects.SQLIteHandler;
using DiscordAngryBot.CustomObjects.ConsoleOutput;

namespace DiscordAngryBot.CustomObjects.Groups
{
    /// <summary>
    /// Класс, содержащий методы расширения Group
    /// </summary>
    public static class GroupHandler
    {
        /// <summary>
        /// Добавить пользователя в группу
        /// </summary>
        /// <param name="group">Группа</param>
        /// <param name="user">Пользователь, добавляемый в группу</param>
        public static void AddUser(this Group group, SocketUser user)
        {
            if (group.users.Contains(user))
            {
                return;
            }
            else
            {
                group.users.Add(user);
            }
        }
        /// <summary>
        /// Удалить пользователя из группы
        /// </summary>
        /// <param name="group">Группа</param>
        /// <param name="user">Пользователь, удаляемый из группы</param>
        public static void RemoveUser(this Group group, SocketUser user)
        {
            if (group.users.Contains(user))
            {
                group.users.Remove(user);
            }
            else
            {
                return;
            }
        }
        /// <summary>
        /// Отправить первичное сообщение о сборе группы
        /// </summary>
        /// <param name="group">Группа</param>
        /// <returns></returns>
        public static async Task SendMessage(this Group group)
        {
            StringBuilder messageBuilder = new StringBuilder();
            if (group.IsParty())
            {
                messageBuilder.Append($"Собирается пати пользователем {group.author.Mention}: {group.destination}\nОсталось {group.userLimit - group.users.Count()} мест.\nСостав группы:\n");
            }
            else if (group.IsRaid())
            {
                messageBuilder.Append($"Собирается рейд пользователем {group.author.Mention}: {group.destination}\nОсталось {group.userLimit - group.users.Count()} мест.\nСостав группы:\n");
            }
            else if (group.IsGuildFight())
            {
                messageBuilder.Append($"Собирается группа на битвы БШ: {group.destination}\nОсталось {group.userLimit - group.users.Count()} мест.\nСостав группы:\n");
            }
            for (int i = 0; i < group.users.Where(x => x != null).Count(); i++)
            {
                messageBuilder.AppendLine($"{i + 1}: {group.users[i].Mention}\n");
            }
            group.targetMessage = await group.channel.SendMessageAsync(messageBuilder.ToString());
            await group.targetMessage.AddReactionAsync(new Emoji("✅"));
            await group.targetMessage.AddReactionAsync(new Emoji("\u274C"));
            await group.targetMessage.AddReactionAsync(new Emoji("\u2757"));
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
                messageBuilder.Append($"Собирается пати пользователем {group.author.Mention}: {group.destination}\nОсталось {group.userLimit - group.users.Count()} мест.\nСостав группы:\n");
            }
            else if (group is Raid)
            {
                messageBuilder.Append($"Собирается рейд пользователем {group.author.Mention}: {group.destination}\nОсталось {group.userLimit - group.users.Count()} мест.\nСостав группы:\n");
            }
            else if (group.IsGuildFight())
            {
                messageBuilder.Append($"Собирается группа на битвы БШ: {group.destination}\nОсталось {group.userLimit - group.users.Count()} мест.\nСостав группы:\n");
            }
            for (int i = 0; i < group.users.Where(x => x != null).Count(); i++)
            {
                messageBuilder.AppendLine($"{i + 1}: {group.users[i].Mention}");
            }
            await group.targetMessage.ModifyAsync(m => { m.Content = messageBuilder.ToString(); });
        }
        public static async Task RewriteMessageOnCancel(this Group group)
        {
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.Append($"Сбор группы {group.author.Mention} ({group.destination}) завершен.");                    
            await group.targetMessage.ModifyAsync(m => { m.Content = messageBuilder.ToString(); });
            await group.targetMessage.RemoveAllReactionsAsync();
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
        public static async Task<Group> DeserializeFromJson(string jsonText, DiscordSocketClient client)
        {
            await ConsoleWriter.Write($"Deserializing Group info object from JSON", ConsoleWriter.InfoType.Notice);
            using (MemoryStream serializationStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonText)))
            {
                var groupDataObject = await JsonSerializer.DeserializeAsync<GroupJSONObject>(serializationStream);
                var group = await groupDataObject.ConvertToGroup(client.GetGuild(636208919114547212));
                return group;
            }
        }
        /// <summary>
        /// Метод, определяющий, является ли группа рейдом
        /// </summary>
        /// <param name="group">Группа</param>
        /// <returns></returns>
        public static bool IsRaid(this Group group)
        {
            if (group is Raid)
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// Метод, определяющий, является ли группа простой группой
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public static bool IsParty(this Group group)
        {
            if (group is Party)
            {
                return true;
            }
            else
                return false;
        }
        public static bool IsGuildFight(this Group group)
        {
            return group is GuildFight;
        }

        /// <summary>
        /// Сохранение группы в базу данных
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public static async Task SaveToDB(this Group group)
        {
            var jsonString = await group.SerializeToJson();
            int isActive = 0;
            if (group.isActive == true)
                isActive = 1;
            string query = $"INSERT INTO Groups (GUID, JSON, isActive) VALUES('{group.GUID}', '{jsonString}', {isActive})";
            await SQLiteDataManager.PushToDB(configs.groups_dbPath, query);
        }
        /// <summary>
        /// Обновление записи группы в базе данных
        /// </summary>
        /// <param name="group">Группа</param>
        /// <returns></returns>
        public static async Task UpdateAtDB(this Group group)
        {
            var jsonString = await group.SerializeToJson();
            int isActive = 0;
            if (group.isActive == true)
                isActive = 1;
            string query = $"UPDATE Groups SET JSON = '{jsonString}' WHERE GUID = '{group.GUID}'";
            await SQLiteDataManager.PushToDB(configs.groups_dbPath, query);
        }
        /// <summary>
        /// Обновление записи в случае, если группа заполнилась
        /// </summary>
        /// <param name="group">Группа</param>
        /// <returns></returns>
        public static async Task UpdateAtDBIfFull(this Group group)
        {
            var jsonString = await group.SerializeToJson();
            int isActive = 0;
            if (group.isActive == true)
                isActive = 1;
            string query = $"UPDATE Groups SET JSON = '{jsonString}', isActive = {isActive} WHERE GUID = '{group.GUID}'";
            await SQLiteDataManager.PushToDB(configs.groups_dbPath, query);
        }
        /// <summary>
        /// Удаление группы из базы данных
        /// </summary>
        /// <param name="group">Группа</param>
        /// <returns></returns>
        public static async Task RemoveFromDB(this Group group)
        {
            var jsonString = await group.SerializeToJson();
            int isActive = 0;
            if (group.isActive == true)
                isActive = 1;
            string query = $"DELETE FROM Groups WHERE GUID = '{group.GUID}'";
            await SQLiteDataManager.PushToDB(configs.groups_dbPath, query);
        }
        /// <summary>
        /// Загрузка всех активных групп из базы данных
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task<List<Group>> LoadAllGroupsFromDB(DiscordSocketClient client)
        {
            await ConsoleWriter.Write($"Loading all groups from database", ConsoleWriter.InfoType.Notice);
            string query = "SELECT * FROM Groups WHERE isActive = 1";
            DataTable data = await SQLiteDataManager.GetDataFromDB(configs.groups_dbPath, query);
            await ConsoleWriter.Write($"Created empty Group list", ConsoleWriter.InfoType.Notice);
            List<Group> groups = new List<Group>();
            await ConsoleWriter.Write($"Filling list with groups", ConsoleWriter.InfoType.Notice);
            foreach (DataRow row in data.AsEnumerable())
            {
                groups.Add(await GroupBuilder.BuildLoadedGroup(client, row["GUID"].ToString(), row["JSON"].ToString(), (int)row["isActive"]));
            }
            return groups;
        }
        /// <summary>
        /// Загрузка группы из рейда по GUID'у
        /// </summary>
        /// <param name="client">Клиент бота</param>
        /// <param name="GUID">Уникальный идентификатор группы</param>
        /// <returns></returns>
        public static async Task<Group> LoadGroupFromDBByGUID(DiscordSocketClient client, string GUID)
        {
            string query = $"SELECT * FROM Groups WHERE GUID = '{GUID}'";
            DataTable data = await SQLiteDataManager.GetDataFromDB(configs.groups_dbPath, query);
            DataRow row = data.Rows[0];
            return await GroupBuilder.BuildLoadedGroup(client, row["GUID"].ToString(), row["JSON"].ToString(), (int)row["isActive"]);
        }

        public static async Task ActualizeReactionsOnGroups(List<Group> groups, DiscordSocketClient client)
        {
            await ConsoleWriter.Write($"Actualizing group members", ConsoleWriter.InfoType.Notice);
            foreach (var group in groups)
            {
                await ConsoleWriter.Write($"Checking group {group.GUID}", ConsoleWriter.InfoType.Notice);
                var usersReacted = group.targetMessage.GetReactionUsersAsync(new Emoji("\u2705"), 13).ToEnumerable().FirstOrDefault();

                foreach (var user in usersReacted)
                {
                    if (group.users.Where(x => x.Id == user.Id).Count() == 0 && !user.IsBot)
                    {
                        await ConsoleWriter.Write($"Adding user {user.Username}", ConsoleWriter.InfoType.Notice);
                        group.AddUser(client.GetUser(user.Id));
                        await group.UpdateAtDB();
                        await group.RewriteMessage();
                    }
                }
                await group.RewriteMessage();
            }
        }
    }

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
            var groupDestination = string.Empty;
            foreach (var word in args)
            {
                groupDestination += (" " + word);
            }
            Party party = new Party()
            {
                author = sourceMessage.Author,
                channel = sourceMessage.Channel,
                createdAt = DateTime.Now,
                destination = groupDestination,
                GUID = Guid.NewGuid().ToString(),
                users = new List<SocketUser>(),
                isActive = true,
                userLimit = 6
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
            var groupDestination = string.Empty;
            foreach (var word in args)
            {
                groupDestination += (" " + word);
            }
            Raid raid = new Raid()
            {
                author = sourceMessage.Author,
                channel = sourceMessage.Channel,
                createdAt = DateTime.Now,
                destination = groupDestination,
                GUID = Guid.NewGuid().ToString(),
                users = new List<SocketUser>(),
                isActive = true,
                userLimit = 12
            };
            await sourceMessage.DeleteAsync();
            return raid;
        }

        public async static Task<GuildFight> BuildGuildFight(SocketMessage sourceMessage, string[] args)
        {
            var groupDestination = string.Empty;
            foreach (var word in args)
            {
                groupDestination += (" " + word);
            }
            GuildFight guildFight = new GuildFight()
            {
                author = sourceMessage.Author,
                channel = sourceMessage.Channel,
                createdAt = DateTime.Now,
                destination = groupDestination,
                GUID = Guid.NewGuid().ToString(),
                users = new List<SocketUser>(),
                isActive = true,
                userLimit = 6,
                isGuildFight = true
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
        public static async Task<Group> BuildLoadedGroup(DiscordSocketClient client, string GUID, string json, int isActive)
        {
            await ConsoleWriter.Write($"Building group {GUID}", ConsoleWriter.InfoType.Notice);
            Group group = await GroupHandler.DeserializeFromJson(json, client);
            group.GUID = GUID;
            group.isActive = true;
            if (group.userLimit == 6)
            {
                //if (group.isGuildFight)
                //    return (GuildFight)group;
                //else
                    return (Party)group;
            }
            else
            {
                return (Raid)group;
            }
        }
       
    }
   
}
