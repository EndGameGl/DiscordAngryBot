using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordAngryBot.CustomObjects.Parsers
{
    /// <summary>
    /// Class for parsing bot commands
    /// </summary>
    public class CommandParser
    {
        /// <summary>
        /// Message that is being parsed
        /// </summary>
        private SocketMessage _message { get; set; }
        /// <summary>
        /// Command prefix
        /// </summary>
        private char? _prefix { get; set; }
        /// <summary>
        /// Command text
        /// </summary>
        private string Command { get; set; }
        /// <summary>
        /// Parameters array
        /// </summary>
        private string[] Parameters { get; set; }
        /// <summary>
        /// Arguments array
        /// </summary>
        private string[] Arguments { get; set; }

        /// <summary>
        /// Class contructor
        /// </summary>
        /// <param name="message">Message to parse</param>
        /// <param name="prefix">Command prefix</param>
        public CommandParser(SocketMessage message, char? prefix)
        {
            _prefix = prefix;
            _message = message;
            ParseCommand(message);
        }

        /// <summary>
        /// Parse command and get data
        /// </summary>
        /// <param name="message">Message to parse</param>
        private void ParseCommand(SocketMessage message)
        {
            if (_prefix != null && _message.Content[0] == _prefix)
            {
                var commandText = _message.Content.Remove(0, 1);
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
        /// Get command from parser
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
        /// Get arguments from parser
        /// </summary>
        /// <returns></returns>
        public string[] GetCommandArgs()
        {
            if (Arguments.Length == 0)
                return null;
            return Arguments;
        }

        /// <summary>
        /// Get parameters from parser
        /// </summary>
        /// <returns></returns>
        public string[] GetCommandParams()
        {
            return Parameters;
        }
    }
}
