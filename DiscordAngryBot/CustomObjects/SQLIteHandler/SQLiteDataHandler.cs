using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.SQLIteHandler
{
    public static class SQLiteDataHandler
    {
        public static List<SQLiteGuildDBEntry> Databases { get; } = new List<SQLiteGuildDBEntry>();

        public static void Add(SQLiteGuildDBEntry entry)
        {
            Databases.Add(entry);
        }
    }
}
