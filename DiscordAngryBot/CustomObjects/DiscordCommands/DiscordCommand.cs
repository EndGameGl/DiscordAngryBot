using DiscordAngryBot.CustomObjects.ConsoleOutput;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.DiscordCommands
{
    public enum CommandType
    {
        Ban,
        Unban,
        Clear,
        SetPrefix,
        News,
        BanRole,
        Admin,
        Deadmin,
        Party,
        Raid,
        List,
        GvgEV,
        GvgPR,
        Help,
        Selfban,
        Bite,
        NotBan,
        TestingPlaceholder,
        JoinGroup,
        LeaveGroup,
        CallGroup,
        TerminateGroup,
        EnableSwear,
        DisableSwear
    }
    public class DiscordCommand
    {
        private Task TaskToRun { get; set; }
        private CommandType Type { get; set; }

        public DiscordCommand(Task task, CommandType commandType)
        {
            TaskToRun = task;
            Type = commandType;
        }

        public void RunCommand()
        {
            if (TaskToRun != null)
            {
                
                Thread thread = new Thread(async () =>
                {
                    try
                    {
                        await ConsoleWriter.Write($"Executing command {Type}", ConsoleWriter.InfoType.CommandInfo);
                        await TaskToRun;
                    }
                    catch (Exception e)
                    {
                        await ConsoleWriter.Write($"Task got an error: [{e.Message}: {e.InnerException?.Message}]", ConsoleWriter.InfoType.Error);
                    }
                });
                thread.Start();                
            }
        }
    }
}
