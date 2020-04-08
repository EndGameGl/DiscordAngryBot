using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDBManager
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateBanTable();
        }

        public static void CreatePartyDB()
        {
            SQLiteConnection.CreateFile("GroupDataBase.sqlite");
            SQLiteConnection connection = new SQLiteConnection("Data Source=GroupDataBase.sqlite;Version=3;");
            connection.Open();

            string commandText = $"CREATE TABLE Groups (GUID TEXT PRIMARY KEY NOT NULL, JSON TEXT NOT NULL, isActive INT NOT NULL)";

            SQLiteCommand command = new SQLiteCommand(commandText, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }

        public static void AddEntry()
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source=GroupDataBase.sqlite;Version=3;");
            string query = $"INSERT INTO Groups (GUID, JSON, isActive) VALUES('abc', 'json', 1)";

            connection.Open();

            SQLiteCommand command = new SQLiteCommand(query, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }

        public static void ToConsole()
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source=GroupDataBase.sqlite;Version=3;");
            string query = $"SELECT * FROM Groups";

            connection.Open();

            SQLiteCommand command = new SQLiteCommand(query, connection);
            var reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            connection.Close();

            foreach (DataRow row in dataTable.Rows)
            {
                Console.WriteLine($"{row[0]} - {row[1]} - {row[2]}");
            }
        }

        public static void CreateBanTable()
        {
            SQLiteConnection.CreateFile("SystemDataBase.sqlite");
            SQLiteConnection connection = new SQLiteConnection("Data Source=SystemDataBase.sqlite;Version=3;");
            connection.Open();

            string commandText = $"CREATE TABLE Bans (GUID TEXT PRIMARY KEY NOT NULL, JSON TEXT NOT NULL, isInfinite INT NOT NULL)";

            SQLiteCommand command = new SQLiteCommand(commandText, connection);
            command.ExecuteNonQuery();

            commandText = $"CREATE TABLE Groups (GUID TEXT PRIMARY KEY NOT NULL, JSON TEXT NOT NULL, isActive INT NOT NULL)";

            command = new SQLiteCommand(commandText, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }
    }
}
