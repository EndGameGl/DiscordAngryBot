using System;

namespace DiscordAngryBot.CustomObjects.DiscordCommands
{
    /// <summary>
    /// Attribute for a method that will be called as a command
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CustomCommandAttribute : Attribute
    {
        /// <summary>
        /// Name for command
        /// </summary>
        public string CommandName { get; private set; }

        /// <summary>
        /// Command description
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Command category
        /// </summary>
        public CommandCategory Category { get; private set; }

        /// <summary>
        /// Command type
        /// </summary>
        public CommandType Type { get; private set; }

        /// <summary>
        /// Command user usage scope
        /// </summary>
        public CommandScope Scope { get; private set; }

        /// <summary>
        /// Command execution environment scope
        /// </summary>
        public CommandExecutionScope CommandExecutionScope { get; private set; }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="commandName">Name for command</param>
        /// <param name="category">Command category</param>
        /// <param name="type">Command type</param>
        /// <param name="description">Command description</param>
        /// <param name="scope">Command user usage scope</param>
        /// <param name="commandExecutionScope">Command execution environment scope</param>
        public CustomCommandAttribute(string commandName, CommandCategory category, CommandType type, string description, CommandScope scope, CommandExecutionScope commandExecutionScope)
        {
            CommandName = commandName;
            Category = category;
            Type = type;
            Description = description;
            Scope = scope;
            CommandExecutionScope = commandExecutionScope;
        }
    }
}
