using DiscordAngryBot.CustomObjects.ConsoleOutput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.DiscordCommands
{
    public class DiscordCommand
    {
        private Task TaskToRun { get; set; }

        public DiscordCommand(Task task)
        {
            TaskToRun = task;
        }

        public void RunCommand()
        {
            if (TaskToRun != null)
            {
                
                Thread thread = new Thread(async () =>
                {
                    try
                    {
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

        private static string GetMethodName([CallerMemberName]string methodName = "")
        {
            var method = new StackTrace()
                .GetFrames()
                .Select(frame => frame.GetMethod())
                .FirstOrDefault(item => item.Name == methodName);

            return method.Name;
        }
    }
}
