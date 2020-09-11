using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
using DiscordAngryBot.CustomObjects.Logs;
using System;
using System.Reflection;
using System.Threading;

namespace DiscordAngryBot.CustomObjects.DiscordCommands
{
    /// <summary>
    /// Discord command class
    /// </summary>
    public class DiscordCommand
    {
        /// <summary>
        /// Additional command metadata
        /// </summary>
        public CustomCommandAttribute CommandMetadata { get; set; }

        /// <summary>
        /// Method that will be invoked for this command
        /// </summary>
        public MethodInfo Method { get; set; }

        /// <summary>
        /// Class constructor
        /// </summary>
        public DiscordCommand() { }

        /// <summary>
        /// Runs this command with given parameters
        /// </summary>
        /// <param name="parameterSet">Command parameters</param>
        public void Run(DiscordCommandParameterSet parameterSet)
        {
            Thread thread = new Thread(async () =>
            {
                try
                {
                    await Debug.Log($"Invoking command {CommandMetadata.CommandName}", LogInfoType.CommandInfo);
                    var methodParameters = parameterSet.GetParameters(Method.GetParameters().Length);
                    switch (CommandMetadata.Type) 
                    {
                        case CommandType.StringCommand:
                            BotCore.GetDataLogs().Add(new DataLog(null, CommandMetadata.CommandName, $"{Method.Name} was called to execute task for {((SocketMessage)methodParameters[0]).Author.Username}"));
                            break;
                        case CommandType.EmojiCommand:
                            BotCore.GetDataLogs().Add(new DataLog(null, CommandMetadata.CommandName, $"{Method.Name} was called to execute task for {((SocketReaction)methodParameters[0]).User.Value}"));
                            break;
                    }               
                    Method.Invoke(null, methodParameters);
                }
                catch (Exception ex)
                {
                    await Debug.Log($"{ex.Message} : {ex.InnerException?.Message}", LogInfoType.Error);
                    BotCore.GetDataLogs().Add(new DataLog(ex, CommandMetadata.CommandName, $"{Method.Name} failed to execute task."));
                }
            });
            thread.Start();
        }

        /// <summary>
        /// Get command hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return CommandMetadata.CommandName.GetHashCode();
        }

        /// <summary>
        /// Check if this command equals other
        /// </summary>
        /// <param name="obj">Other command</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return CommandMetadata.CommandName.Equals(((DiscordCommand)obj).CommandMetadata.CommandName);
        }
    }
}
