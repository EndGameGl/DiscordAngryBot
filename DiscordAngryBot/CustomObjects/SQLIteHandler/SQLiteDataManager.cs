using DiscordAngryBot.CustomObjects.ConsoleOutput;
using System.Data;
using System.Data.SQLite;
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
            await Debug.Log($"Creating new connection...", Debug.InfoType.Notice);
            SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            SQLiteCommand command = new SQLiteCommand(sqlQuery, connection);

            await connection.OpenAsync();
            await Debug.Log($"Opened new async connection: {connection.DataSource}", Debug.InfoType.Notice);
            await Debug.Log($"Executing async reader", Debug.InfoType.Notice);
            var reader = await command.ExecuteReaderAsync();

            DataTable dataTable = new DataTable();
            await Debug.Log($"Loading new datatable", Debug.InfoType.Notice);            
            dataTable.Load(reader);
            await Debug.Log($"Loaded datatable: {dataTable.Rows.Count} rows", Debug.InfoType.Notice);
            reader.Close();
            await Debug.Log($"Closed reader", Debug.InfoType.Notice);
            connection.Close();
            await Debug.Log($"Closed connection", Debug.InfoType.Notice);

            return dataTable;
        }
        /// <summary>
        /// Создание базы данных
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public static async Task CreateDataBase(string Path)
        {
            await Task.Run(
                () =>
            SQLiteConnection.CreateFile($"{Path}")
            );
        }
    }
}
