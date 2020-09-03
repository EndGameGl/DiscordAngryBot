using DiscordAngryBot.CustomObjects.ConsoleOutput;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace DiscordAngryBot.CustomObjects.DiscordCommands
{
    /// <summary>
    /// Класс, содержащий в себе команду для бота
    /// </summary>
    public class DiscordCommand
    {
        /// <summary>
        /// Метадата к методу, вызываемому командой
        /// </summary>
        public CustomCommandAttribute CommandMetadata { get; set; }
        /// <summary>
        /// Метод, вызываемый командой
        /// </summary>
        public MethodInfo Method { get; set; }
        /// <summary>
        /// Конструктор команды
        /// </summary>
        public DiscordCommand() { }
        /// <summary>
        /// Запуск команды с указанным набором параметров
        /// </summary>
        /// <param name="parameterSet"></param>
        public void Run(DiscordCommandParameterSet parameterSet)
        {
            Thread thread = new Thread(async () =>
            {
                try
                {
                    await Debug.Log($"Invoking command {CommandMetadata.CommandName}", Debug.InfoType.CommandInfo);
                    Method.Invoke(null, parameterSet.GetParameters(Method.GetParameters().Length));
                }
                catch (Exception ex)
                {
                    await Debug.Log($"{ex.Message} : {ex.InnerException?.Message}", Debug.InfoType.Error);
                }
            });
            thread.Start();
        }
        public override int GetHashCode()
        {
            return CommandMetadata.CommandName.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return CommandMetadata.CommandName.Equals(((DiscordCommand)obj).CommandMetadata.CommandName);
        }
    }
}
