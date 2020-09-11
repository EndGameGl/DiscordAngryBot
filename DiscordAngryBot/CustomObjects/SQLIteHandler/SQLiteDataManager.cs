using DiscordAngryBot.CustomObjects.ConsoleOutput;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.SQLIteHandler
{
    /// <summary>
    /// Class for handling SQLite DB operations
    /// </summary>
    public static class SQLiteDataManager
    {
        /// <summary>
        /// Push query to DB
        /// </summary>
        /// <param name="dbPath">DB path</param>
        /// <param name="sqlQuery">DB SQL Query</param>
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
        /// Load query result to DataTable object
        /// </summary>
        /// <param name="dbPath">DB path</param>
        /// <param name="sqlQuery">DB SQL query</param>
        /// <returns></returns>
        public static async Task<DataTable> GetDataFromDB(string dbPath, string sqlQuery)
        {
            await Debug.Log($"Creating new connection...", LogInfoType.Notice);
            SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            SQLiteCommand command = new SQLiteCommand(sqlQuery, connection);

            await connection.OpenAsync();
            await Debug.Log($"Opened new async connection: {connection.DataSource}", LogInfoType.Notice);
            await Debug.Log($"Executing async reader", LogInfoType.Notice);
            var reader = await command.ExecuteReaderAsync();

            DataTable dataTable = new DataTable();
            await Debug.Log($"Loading new datatable", LogInfoType.Notice);            
            dataTable.Load(reader);
            await Debug.Log($"Loaded datatable: {dataTable.Rows.Count} rows", LogInfoType.Notice);
            reader.Close();
            await Debug.Log($"Closed reader", LogInfoType.Notice);
            connection.Close();
            await Debug.Log($"Closed connection", LogInfoType.Notice);

            return dataTable;
        }

        /// <summary>
        /// Create new DB
        /// </summary>
        /// <param name="Path">Creation path</param>
        /// <returns></returns>
        public static async Task CreateDataBase(string Path)
        {
            await Task.Run(() => SQLiteConnection.CreateFile($"{Path}"));
        }
    }
}
