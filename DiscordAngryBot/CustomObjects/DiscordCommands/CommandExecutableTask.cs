using Discord.WebSocket;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.DiscordCommands
{
    public class CommandExecutableTask
    {
        private Task TaskToRun { get; set; }
        public SocketUser CommandCaller { get; set; }

        public CommandExecutableTask(Task task, SocketUser user)
        {
            TaskToRun = task;
            CommandCaller = user;
        }
        public async Task Run()
        {
            await TaskToRun;
        }
    }
}
