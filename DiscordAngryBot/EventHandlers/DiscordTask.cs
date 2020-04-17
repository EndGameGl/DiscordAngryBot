using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.EventHandlers
{
    public class DiscordTask : IDisposable
    {
        private bool disposed = false;
        private Task targetTask { get; set; }
        private DateTime launchTime { get; set; }

        public DiscordTask(Task task, DateTime time)
        {
            targetTask = task;
            
            launchTime = time;
        }

        public async Task<bool> Run()
        {
            await targetTask;
            return targetTask.IsCompleted;
        }

        public DateTime GetDueTime()
        {
            return launchTime;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    targetTask = null;
                }
                disposed = true;
            }
        }
    }
}
