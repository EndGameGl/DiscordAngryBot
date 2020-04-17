using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordAngryBot.EventHandlers
{
    public class EventHandler
    {
        private List<DiscordTask> queuedTasks { get; set; }
        private Timer timer { get; set; }
        public EventHandler() 
        {
            timer = new Timer(TimerCallbackListCheck, null, 0, 500);
        }

        public void AddToQueue(DiscordTask newTask)
        {            
            queuedTasks.Add(newTask);
        }

        private async void TimerCallbackListCheck(object state)
        {
            if (queuedTasks.Count > 0)
            {
                foreach (var queuedTask in queuedTasks)
                {
                    if (queuedTask.GetDueTime() <= DateTime.Now)
                    {
                        if (await queuedTask.Run())
                        {
                            queuedTask.Dispose();
                        }

                    }
                }
            }
        }
    }
}
