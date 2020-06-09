using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Parsers
{
    public class CommandParser
    {
        private SocketMessage _Message { get; set; }
        private char? _Prefix { get; set; }
        private string Command { get; set; }
        private string[] Parameters { get; set; }

        public CommandParser(SocketMessage message, char? prefix)
        {
            _Message = message;
            _Prefix = prefix;
            ParseCommand();
        }
        private void ParseCommand()
        {
            if (_Prefix != null && _Message.Content[0] == _Prefix)
            {
                var commandText = _Message.Content.Remove(0, 1);
                var words = commandText.Split(new char[] { ' ' });
                Command = words[0];
                Parameters = new List<string>(words).GetRange(1, words.Length - 1).ToArray();
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

        public string[] GetCommandParameters()
        {
            return Parameters;
        }
    }
}
