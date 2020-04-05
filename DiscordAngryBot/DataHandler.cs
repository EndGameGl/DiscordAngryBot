using DiscordAngryBot.CustomObjects.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordAngryBot
{
    /// <summary>
    /// Класс, хранящий в себе основные необходимые данные бота
    /// </summary>
    public class DataHandler
    {
        /// <summary>
        /// Список таймеров
        /// </summary>
        public List<Timer> timers { get; set; }
        /// <summary>
        /// Список групп
        /// </summary>
        public List<Group> groups { get; set; }
        /// <summary>
        /// Конструктор класса
        /// </summary>
        public DataHandler()
        {
            timers = new List<Timer>();
            groups = new List<Group>();
        }
    }
}
