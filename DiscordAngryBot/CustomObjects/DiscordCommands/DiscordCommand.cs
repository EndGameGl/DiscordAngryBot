using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.System;

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
        DisableSwear,
        Roll
    }
    public class DiscordCommand
    {
        private Task TaskToRun { get; set; }
        private CommandType Type { get; set; }
        private SocketUser User { get; set; }

        public DiscordCommand(Task task, CommandType commandType, SocketUser user)
        {
            TaskToRun = task;
            Type = commandType;
            User = user;
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
                        BotCore.GetDataLogs().Add(new Logs.DataLog() { LogType = "Command", Message = $"Executed command {Type} for {User.Username} ({User.Id})" });
                        await TaskToRun;
                    }
                    catch (Exception e)
                    {
                        BotCore.GetDataLogs().Add(new CustomObjects.Logs.DataLog() { Exception = e, LogType = "Error" });
                        await ConsoleWriter.Write($"Task got an error: [{e.Message}: {e.InnerException?.Message}]", ConsoleWriter.InfoType.Error);
                    }
                });
                thread.Start();                
            }
        }
    }
}
