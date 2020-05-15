using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.ConsoleOutput
{
    /// <summary>
    /// Класс, предназаченный для форматирования текста в консоль
    /// </summary>
    public static class ConsoleWriter
    {
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
            Chat
        }
        /// <summary>
        /// Вывод текста в консоль
        /// </summary>
        /// <param name="obj">Информация для вывода в консоль</param>
        /// <param name="type">Тип информации</param>
        /// <returns></returns>
        public static async Task Write(object obj, InfoType type)
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
                    }
                    Console.WriteLine($"[{DateTime.Now,5}] [{type.ToString(),5}]: {obj.ToString()}");
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
