using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.SQLIteHandler
{
    /// <summary>
    /// Класс, предназначенный для взаимодействия с базой данных
    /// </summary>
    public static class SQLiteDataManager
    {
        /// <summary>
        /// Запрос к базе данных
        /// </summary>
        /// <param name="dbPath">Путь к БД</param>
        /// <param name="sqlQuery">Запрос к БД</param>
        /// <returns></returns>
        public static async Task PushToDB(string dbPath, string sqlQuery)
        {
            SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            SQLiteCommand command = new SQLiteCommand(sqlQuery, connection);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            connection.Close();
        }
        /// <summary>
        /// Загрузка объекта DataTable из базы данных по запросу
        /// </summary>
        /// <param name="dbPath">Путь к БД</param>
        /// <param name="sqlQuery">Запрос к БД</param>
        /// <returns></returns>
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
