using DiscordAngryBot.CustomObjects.DiscordCommands;
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
        public List<string> swearFilterWords { get; set; } = new List<string>()
        {
             "NAHOOU", "PIDOR", "ХУЙ", "ХУЕ", "ХУЁ", "БЛЯ", "ЕБИ", "ЕБА", "ПИЗД", "ДОЛБОЕБ", "ДОЛБОЁБ", "ШЛЮХ", "СОСИ", "СОСА", "СОСОВ", "ПИДОР", "ПИДР", "ПОРН", "СПЕРМ", "ПИДАР",
             "ШАЛАВ", "ДАУН", "ХУЛЕ", "ХУЛИ", "УБЛЮД", "DICK", "BASTARD", "АНАЛ"
        };
        /// <summary>
        /// Список команд, зарегистрированных в боте
        /// </summary>
        public HashSet<DiscordCommand> Commands = new HashSet<DiscordCommand>(new DiscordCommandComparer());

        /// <summary>
        /// Пустой конструктор класса
        /// </summary>
        public BotSettings() { }
        /// <summary>
        /// Регистрация новой команды
        /// </summary>
        /// <param name="command"></param>
        public void RegisterCommand(DiscordCommand command)
        {
            Commands.Add(command);
        }
    }
}
