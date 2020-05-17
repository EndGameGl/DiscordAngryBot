using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        public char commandPrefix { get; set; } = '_';
        /// <summary>
        /// Список запрещенных команд
        /// </summary>
        public string[] forbiddenCommands { get; set; } = new string[] 
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
        public string[] systemCommands { get; set;} = new string[] 
        { 
            "BAN",
            "CLEAR"
        };
        /// <summary>
        /// Список пользовательских команд
        /// </summary>
        public string[] userCommands { get; set; } = new string[] 
        { 
            "PARTY", 
            "RAID", 
            "LIST", 
            "GVG", 
            "HELP" 
        };
        public string[] musicCommands { get; set; } = new string[] 
        {
            "SUMMON"
        };
        public string[] otherCommands { get; set; } = new string[] 
        { 
            "КУСЬ", 
            "БАН" 
        };
        public string[] swearFilterWords { get; set; } = new string[] 
        {
             "NAHOOU", "PIDOR", "ХУЙ", "ХУЕ", "ХУЁ", "БЛЯ", "ЕБИ", "ЕБА", "ПИЗД", "ДОЛБОЕБ", "ДОЛБОЁБ", "ШЛЮХ", "СОСИ", "СОСА", "СОСОВ", "ПИДОР", "ПИДР", "ПОРН", "СПЕРМ", "ПИДАР",
             "ШАЛАВ", "ДАУН", "ХУЛЕ", "ХУЛИ", "УБЛЮД"
        };
        /// <summary>
        /// Список администраторов бота
        /// </summary>
        public ulong[] admins { get; set; } = new ulong[] 
        { 
            261497385274966026, 
            261496784336060417, 
            279716516595892235 
        };
        public bool needsDeepLog { get; set; } = true;
        /// <summary>
        /// Пустой конструктор класса
        /// </summary>
        public BotSettings() { }
    }
}
