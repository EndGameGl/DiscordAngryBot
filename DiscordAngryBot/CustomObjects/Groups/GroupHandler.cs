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

namespace DiscordAngryBot.CustomObjects.Groups
{
    public static class GroupHandler
    {
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
        public static async Task SendMessage(this Group group)
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
            for (int i = 0; i < group.users.Where(x => x != null).Count(); i++)
            {
                messageBuilder.AppendLine($"{i + 1}: {group.users[i].Mention}\n");
            }
            group.targetMessage = await group.channel.SendMessageAsync(messageBuilder.ToString());
            await group.targetMessage.AddReactionAsync(new Emoji("✅"));
            await group.targetMessage.AddReactionAsync(new Emoji("\u274C"));
            await group.targetMessage.AddReactionAsync(new Emoji("\u2757"));
        }
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
            for (int i = 0; i < group.users.Where(x => x != null).Count(); i++)
            {
                messageBuilder.AppendLine($"{i + 1}: {group.users[i].Mention}");
            }
            await group.targetMessage.ModifyAsync(m => { m.Content = messageBuilder.ToString(); });
        }
        public static async Task<string> SerializeToJson(this Group group) 
        {
            using (MemoryStream serializationStream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync<GroupJSONObject>(serializationStream, new GroupJSONObject(group));
                var jsonResult = Encoding.UTF8.GetString(serializationStream.ToArray());
                return jsonResult;
            }
        }
        public static async Task<Group> DeserializeFromJson(string jsonText, DiscordSocketClient client)
        {
            using (MemoryStream serializationStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonText)))
            {
                var groupDataObject = await JsonSerializer.DeserializeAsync<GroupJSONObject>(serializationStream);
                var group = await groupDataObject.ConvertToGroup(client.GetGuild(636208919114547212));
                return group;
            }
        }
        public static bool IsRaid(this Group group)
        {
            if (group is Raid)
            {
                return true;
            }
            else
                return false;
        }
        public static bool IsParty(this Group group)
        {
            if (group is Party)
            {
                return true;
            }
            else
                return false;
        }
        public static async Task SaveToDB(this Group group)
        {
            var jsonString = await group.SerializeToJson();
            int isActive = 0;
            if (group.isActive == true)
                isActive = 1;
            string query = $"INSERT INTO Groups (GUID, JSON, isActive) VALUES('{group.GUID}', '{jsonString}', {isActive})";
            await SQLiteDataManager.PushToDB(configs.groups_dbPath, query);
        }
        public static async Task UpdateAtDB(this Group group)
        {
            var jsonString = await group.SerializeToJson();
            int isActive = 0;
            if (group.isActive == true)
                isActive = 1;
            string query = $"UPDATE Groups SET JSON = '{jsonString}' WHERE GUID = '{group.GUID}'";
            await SQLiteDataManager.PushToDB(configs.groups_dbPath, query);
        }
        public static async Task UpdateAtDBIfFull(this Group group)
        {
            var jsonString = await group.SerializeToJson();
            int isActive = 0;
            if (group.isActive == true)
                isActive = 1;
            string query = $"UPDATE Groups SET JSON = '{jsonString}', isActive = {isActive} WHERE GUID = '{group.GUID}'";
            await SQLiteDataManager.PushToDB(configs.groups_dbPath, query);
        }
        public static async Task RemoveFromDB(this Group group)
        {
            var jsonString = await group.SerializeToJson();
            int isActive = 0;
            if (group.isActive == true)
                isActive = 1;
            string query = $"DELETE FROM Groups WHERE GUID = '{group.GUID}'";
            await SQLiteDataManager.PushToDB(configs.groups_dbPath, query);
        }
        public static async Task<List<Group>> LoadAllGroupsFromDB(DiscordSocketClient client)
        {
            string query = "SELECT * FROM Groups WHERE isActive = 1";
            DataTable data = await SQLiteDataManager.GetDataFromDB(configs.groups_dbPath, query);
            List<Group> groups = new List<Group>();
            foreach (DataRow row in data.AsEnumerable())
            {
                groups.Add(await GroupBuilder.BuildLoadedGroup(client, row["GUID"].ToString(), row["JSON"].ToString(), (int)row["isActive"]));
            }
            return groups;
        }
        public static async Task<Group> LoadGroupFromDBByGUID(DiscordSocketClient client, string GUID)
        {
            string query = $"SELECT * FROM Groups WHERE GUID = '{GUID}'";
            DataTable data = await SQLiteDataManager.GetDataFromDB(configs.groups_dbPath, query);
            DataRow row = data.Rows[0];
            return await GroupBuilder.BuildLoadedGroup(client, row["GUID"].ToString(), row["JSON"].ToString(), (int)row["isActive"]);
        }
    }

    public static class GroupBuilder
    {
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
        public static async Task<Group> BuildLoadedGroup(DiscordSocketClient client, string GUID, string json, int isActive)
        {
            Group group = await GroupHandler.DeserializeFromJson(json, client);
            group.GUID = GUID;
            group.isActive = true;
            if (group.userLimit == 6)
            {
                return (Party)group;
            }
            else
            {
                return (Raid)group;
            }
        }
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
    }

    public static class SQLiteDataManager
    {
        public static async Task PushToDB(string dbPath, string sqlQuery)
        {
            SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            SQLiteCommand command = new SQLiteCommand(sqlQuery, connection);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            connection.Close();
        }
        public static async Task<DataTable> GetDataFromDB(string dbPath, string sqlQuery)
        {
            SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            SQLiteCommand command = new SQLiteCommand(sqlQuery, connection);

            await connection.OpenAsync();
            var reader = await command.ExecuteReaderAsync();

            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            reader.Close();

            connection.Close();

            return dataTable;
        }
    }
}
