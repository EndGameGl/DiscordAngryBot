using DiscordAngryBot.CustomObjects.ConsoleOutput;
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
            await ConsoleWriter.Write($"Creating new connection...", ConsoleWriter.InfoType.Notice);
            SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            SQLiteCommand command = new SQLiteCommand(sqlQuery, connection);

            await connection.OpenAsync();
            await ConsoleWriter.Write($"Opened new async connection: {connection.DataSource}", ConsoleWriter.InfoType.Notice);
            await ConsoleWriter.Write($"Executing async reader", ConsoleWriter.InfoType.Notice);
            var reader = await command.ExecuteReaderAsync();

            DataTable dataTable = new DataTable();
            await ConsoleWriter.Write($"Loading new datatable", ConsoleWriter.InfoType.Notice);            
            dataTable.Load(reader);
            await ConsoleWriter.Write($"Loaded datatable: {dataTable.Rows.Count} rows", ConsoleWriter.InfoType.Notice);
            reader.Close();
            await ConsoleWriter.Write($"Closed reader", ConsoleWriter.InfoType.Notice);
            connection.Close();
            await ConsoleWriter.Write($"Closed connection", ConsoleWriter.InfoType.Notice);

            return dataTable;
        }
    }
}
