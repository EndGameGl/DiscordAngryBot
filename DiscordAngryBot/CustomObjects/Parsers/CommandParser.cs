using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordAngryBot.CustomObjects.Parsers
{
    public class CommandParser
    {
        private SocketMessage _Message { get; set; }
        private char? _Prefix { get; set; }
        private string Command { get; set; }
        private string[] Parameters { get; set; }
        private string[] Arguments { get; set; }

        public CommandParser(SocketMessage message, char? prefix)
        {
            _Prefix = prefix;
            _Message = message;
            ParseCommand(message);
        }
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

        public string[] GetCommandArgs()
        {
            return Arguments;
        }

        public string[] GetCommandParams()
        {
            return Parameters;
        }
    }
}
