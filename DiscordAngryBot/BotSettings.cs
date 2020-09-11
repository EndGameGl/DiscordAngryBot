using DiscordAngryBot.CustomObjects.DiscordCommands;
using System.Collections.Generic;

namespace DiscordAngryBot
{
    /// <summary>
    /// Global bot settings class
    /// </summary>
    public class BotSettings
    {
        /// <summary>
        /// Default commands prefix
        /// </summary>
        public char defaultCommandPrefix { get; set; } = '_';

        /// <summary>
        /// Shiro forbidden commands
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
        /// List of filtered words
        /// </summary>
        public List<string> swearFilterWords { get; set; } = new List<string>()
        {
             "NAHOOU", "PIDOR", "ХУЙ", "ХУЕ", "ХУЁ", "БЛЯ", "ЕБИ", "ЕБА", "ПИЗД", "ДОЛБОЕБ", "ДОЛБОЁБ", "ШЛЮХ", "СОСИ", "СОСА", "СОСОВ", "ПИДОР", "ПИДР", "ПОРН", "СПЕРМ", "ПИДАР",
             "ШАЛАВ", "ДАУН", "ХУЛЕ", "ХУЛИ", "УБЛЮД", "DICK", "BASTARD", "АНАЛ"
        };

        /// <summary>
        /// Registered bot commands
        /// </summary>
        public HashSet<DiscordCommand> Commands = new HashSet<DiscordCommand>(new DiscordCommandComparer());

        /// <summary>
        /// Class constructor
        /// </summary>
        public BotSettings() { }

        /// <summary>
        /// Register command
        /// </summary>
        /// <param name="command">Command</param>
        public void RegisterCommand(DiscordCommand command)
        {
            Commands.Add(command);
        }
    }
}
