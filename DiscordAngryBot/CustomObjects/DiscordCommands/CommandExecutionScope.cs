using System;

namespace DiscordAngryBot.CustomObjects.DiscordCommands
{
    [Flags]
    public enum CommandExecutionScope
    {
        Server = 1,
        DM = 2,
        All = 4
    }
}
