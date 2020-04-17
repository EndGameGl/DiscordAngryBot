using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.ConsoleOutput
{
    public static class ConsoleWriter
    {
        public enum InfoType
        {
            Info,
            Notice,
            Error,
            CommandInfo,
            Chat
        }
        public static async Task Write(object obj, InfoType type)
        {
                Console.BackgroundColor = ConsoleColor.White;
                switch (type)
                {
                    case InfoType.Info:
                        Console.ForegroundColor = ConsoleColor.Black;
                        break;
                    case InfoType.Chat:
                        Console.ForegroundColor = ConsoleColor.Black;
                        break;
                    case InfoType.Error:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;
                    case InfoType.Notice:
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        break;
                    case InfoType.CommandInfo:
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        break;
                }
                Console.WriteLine($"[{DateTime.Now,5}] [{type.ToString(),5}]: {obj.ToString()}");
        }

        public static async Task WriteDivideLine()
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine($"[{DateTime.Now,5}]: #-------------------------------------------------#");
        }

        public static async Task WriteDivideMessage(object text)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine($"\n[{DateTime.Now,5}]: ..//{text.ToString()}/\n");
        }
    }
}
