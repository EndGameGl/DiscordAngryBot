using System;
using System.IO;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.ConsoleOutput
{
    /// <summary>
    /// Класс, предназаченный для форматирования текста в консоль
    /// </summary>
    public static class Debug
    {
        private static readonly bool IsDegugEnabled = true;
        private static readonly bool WriteToFile = false;
        /// <summary>
        /// Тип информации, выводимой в консоль
        /// </summary>
        public enum InfoType
        {
            /// <summary>
            /// Обычная информация
            /// </summary>
            Info,
            /// <summary>
            /// События внутрия бота
            /// </summary>
            Notice,
            /// <summary>
            /// Ошибки внутри бота
            /// </summary>
            Error,
            /// <summary>
            /// Инфо о коммандах
            /// </summary>
            CommandInfo,
            /// <summary>
            /// Чат дискорда
            /// </summary>
            Chat,
            Debug
        }
        /// <summary>
        /// Вывод текста в консоль
        /// </summary>
        /// <param name="obj">Информация для вывода в консоль</param>
        /// <param name="type">Тип информации</param>
        /// <returns></returns>
        public static async Task Log(object obj, InfoType type = InfoType.Debug)
        {               
                await Task.Run( () => 
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
                        case InfoType.Debug:
                            if (IsDegugEnabled == false)
                                return;
                            
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            break;
                    }
                    var text = $"[{DateTime.Now,5}] [{type.ToString(),8}]: {obj.ToString()}";
                    Console.WriteLine(text);
                    if (WriteToFile)
                        File.AppendAllText("Logs.log", "\n" + text);
                });
        }

        /// <summary>
        /// Разделительная строка в консоли
        /// </summary>
        /// <returns></returns>
        public static async Task WriteDivideLine()
        {
            await Task.Run(() =>
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine($"[{DateTime.Now,5}]: #-------------------------------------------------#");
            });
        }

        /// <summary>
        /// Разделительное сообщение в консоли
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static async Task WriteDivideMessage(object text)
        {
            await Task.Run(() => 
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine($"\n[{DateTime.Now,5}]: ..//{text.ToString()}/\n"); 
            });
        }
    }
}
