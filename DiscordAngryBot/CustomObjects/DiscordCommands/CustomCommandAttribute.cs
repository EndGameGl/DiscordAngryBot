using System;

namespace DiscordAngryBot.CustomObjects.DiscordCommands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CustomCommandAttribute : Attribute
    {
        public string CommandName { get; private set; }
        public string Description { get; private set; }
        public CommandCategory Category { get; private set; }
        public CommandType Type { get; private set; }
        public CommandScope Scope { get; private set; }
        public CommandExecutionScope CommandExecutionScope { get; private set; }
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
