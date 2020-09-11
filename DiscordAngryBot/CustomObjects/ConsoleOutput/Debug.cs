using System;
using System.IO;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.ConsoleOutput
{
    /// <summary>
    /// Class for debugging messages to console
    /// </summary>
    public static class Debug
    {
        /// <summary>
        /// Whether debug mode is on
        /// </summary>
        private static readonly bool IsDegugEnabled = true;
        /// <summary>
        /// Whether should write logs to file
        /// </summary>
        private static readonly bool WriteToFile = false;

        /// <summary>
        /// Outputs data to console
        /// </summary>
        /// <param name="obj">Data</param>
        /// <param name="type">Information type</param>
        /// <returns></returns>
        public static async Task Log(object obj, LogInfoType type = LogInfoType.Debug)
        {               
                await Task.Run( () => 
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    switch (type)
                    {
                        case LogInfoType.Info:
                            Console.ForegroundColor = ConsoleColor.Black;
                            break;
                        case LogInfoType.Chat:
                            Console.ForegroundColor = ConsoleColor.Black;
                            break;
                        case LogInfoType.Error:
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            break;
                        case LogInfoType.Notice:
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                            break;
                        case LogInfoType.CommandInfo:
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            break;
                        case LogInfoType.Debug:
                            if (IsDegugEnabled == false)
                                return;                           
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            break;
                    }
                    var text = $"[{DateTime.Now,5}] [{type, 8}]: {obj}";
                    Console.WriteLine(text);
                    if (WriteToFile)
                        File.AppendAllText("Logs.log", "\n" + text);
                });
        }

        /// <summary>
        /// Writes a divide line to console
        /// </summary>
        /// <returns></returns>
        public static async Task WriteDivideLine()
        {
            await Task.Run(() =>
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine($"[{DateTime.Now,5}]: #{new string('-', 20)}#");
            });
        }

        /// <summary>
        /// Writes a divide message to console
        /// </summary>
        /// <param name="text">Message text</param>
        /// <returns></returns>
        public static async Task WriteDivideMessage(object text)
        {
            await Task.Run(() => 
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine($"\n[{DateTime.Now,5}]: ..//{text}/\n"); 
            });
        }
    }
}
