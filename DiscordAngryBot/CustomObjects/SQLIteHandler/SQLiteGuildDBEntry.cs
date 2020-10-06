using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.Caches;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.SQLIteHandler
{
    public class SQLiteGuildDBEntry
    {
        public ExtendedDiscordGuildData GuildData { get; }
        public string DBPath { get; }

        public SQLiteGuildDBEntry(ExtendedDiscordGuildData guildData)
        {
            GuildData = guildData;
            DBPath = $"locals/Databases/{guildData.Guild.Id}/Data.sqlite";
        }

        public async Task PushToDB(string sqlQuery)
        {
            SQLiteConnection connection = new SQLiteConnection($"Data Source={DBPath};Version=3;");
            SQLiteCommand command = new SQLiteCommand(sqlQuery, connection);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            connection.Close();
        }
    }
}
