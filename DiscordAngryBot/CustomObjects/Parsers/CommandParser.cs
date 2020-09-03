using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordAngryBot.CustomObjects.Parsers
{
    /// <summary>
    /// Класс для парса команд бота
    /// </summary>
    public class CommandParser
    {
        /// <summary>
        /// Сообщение, текст которого парсится
        /// </summary>
        private SocketMessage _Message { get; set; }
        /// <summary>
        /// Префикс команды
        /// </summary>
        private char? _Prefix { get; set; }
        /// <summary>
        /// Текст команды
        /// </summary>
        private string Command { get; set; }
        /// <summary>
        /// Параметры команды
        /// </summary>
        private string[] Parameters { get; set; }
        /// <summary>
        /// Аргументы команды
        /// </summary>
        private string[] Arguments { get; set; }
        /// <summary>
        /// Конструктор парсера
        /// </summary>
        /// <param name="message"></param>
        /// <param name="prefix"></param>
        public CommandParser(SocketMessage message, char? prefix)
        {
            _Prefix = prefix;
            _Message = message;
            ParseCommand(message);
        }
        /// <summary>
        /// Парс команды
        /// </summary>
        /// <param name="message"></param>
        private void ParseCommand(SocketMessage message)
        {
            if (_Prefix != null && _Message.Content[0] == _Prefix)
            {
                var commandText = _Message.Content.Remove(0, 1);
                var commandWords = commandText.Split(new char[] { ' ' });
                Command = commandWords[0];
                Arguments = new List<string>(commandWords).GetRange(1, commandWords.Length - 1).ToArray();

                if (Command.Contains('.'))
                {
                    var commandParams = Command.Split(new char[] { '.' });
                    Parameters = new List<string>(commandParams).GetRange(1, commandParams.Length - 1).ToArray();
                }
            }
        }
        /// <summary>
        /// Получение команды
        /// </summary>
        /// <returns></returns>
        public string GetCommand()
        {
            if ((Command != null) && (Command?.Length > 0))
            {
                return Command.ToUpper();
            }
            else
            {
                throw new Exception("Encountered an empty command.");
            }
        }
        /// <summary>
        /// Получение аргументов команды
        /// </summary>
        /// <returns></returns>
        public string[] GetCommandArgs()
        {
            if (Arguments.Length == 0)
                return null;
            return Arguments;
        }
        /// <summary>
        /// Получение параметров команды
        /// </summary>
        /// <returns></returns>
        public string[] GetCommandParams()
        {
            return Parameters;
        }
    }
}
