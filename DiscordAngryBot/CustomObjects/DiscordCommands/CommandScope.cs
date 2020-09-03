using System;

namespace DiscordAngryBot.CustomObjects.DiscordCommands
{
    [Flags]
    public enum CommandScope
    {
        User = 1,
        Admin = 2
    }
}
