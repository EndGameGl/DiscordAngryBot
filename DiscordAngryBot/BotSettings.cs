using System.Collections.Generic;

namespace DiscordAngryBot
{
    /// <summary>
    /// Класс, представляющий настройки бота
    /// </summary>
    public class BotSettings
    {
        /// <summary>
        /// Префикс команд бота
        /// </summary>
        public char defaultCommandPrefix { get; set; } = '_';
        /// <summary>
        /// Список запрещенных команд
        /// </summary>
        public List<string> forbiddenCommands { get; set; } = new List<string>() 
        { 
            ".br all",
            ".$",
            ".timely",
            ".profile",
            ".xplb"
        };
        /// <summary>
        /// Список команд администраторов
        /// </summary>
        public List<string> systemCommands { get; set;} = new List<string>()
        { 
            "EMBEDTEST",
            "BAN",
            "CLEAR",
            "UNBAN",
            "SETPREFIX",
            "NEWS",
            "BANROLE",
            "ADMIN",
            "DEADMIN",
            "FILTERENABLE",
            "FILTERDISABLE"
        };
        /// <summary>
        /// Список пользовательских команд
        /// </summary>
        public List<string> userCommands { get; set; } = new List<string>()
        { 
            "PARTY", 
            "RAID", 
            "LIST", 
            "GVGEV",
            "GVGPR",
            "HELP",
            "SELFBAN",
            "ROLL"
        };
        public List<string> musicCommands { get; set; } = new List<string>()
        {
            "SUMMON"
        };
        public List<string> otherCommands { get; set; } = new List<string>()
        { 
            "КУСЬ", 
            "БАН" 
        };
        public List<string> swearFilterWords { get; set; } = new List<string>()
        {
             "NAHOOU", "PIDOR", "ХУЙ", "ХУЕ", "ХУЁ", "БЛЯ", "ЕБИ", "ЕБА", "ПИЗД", "ДОЛБОЕБ", "ДОЛБОЁБ", "ШЛЮХ", "СОСИ", "СОСА", "СОСОВ", "ПИДОР", "ПИДР", "ПОРН", "СПЕРМ", "ПИДАР",
             "ШАЛАВ", "ДАУН", "ХУЛЕ", "ХУЛИ", "УБЛЮД", "DICK", "BASTARD", "АНАЛ"
        };
        /// <summary>
        /// Список администраторов бота
        /// </summary>
        public List<ulong> admins { get; set; } = new List<ulong>()
        { 
            261497385274966026, 
            261496784336060417, 
            279716516595892235,
            395211796354433025
        };
        /// <summary>
        /// Пустой конструктор класса
        /// </summary>
        public BotSettings() { }
    }
}
