using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Groups
{
    public abstract class Group : IDisposable
    {

        bool disposed = false;
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        public string GUID { get; set; }
        public SocketUser author { get; set; }
        public ISocketMessageChannel channel { get; set; }
        public List<SocketUser> users { get; set; }
        public int userLimit { get; set; }       
        public RestUserMessage targetMessage { get; set; }
        public DateTime createdAt { get; set; }
        public string destination { get; set; }
        public bool isActive { get; set; }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
            }

            disposed = true;
        }
    }
}
