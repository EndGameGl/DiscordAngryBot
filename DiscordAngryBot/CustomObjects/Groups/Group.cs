using Discord.Rest;
using Discord.WebSocket;
using DiscordAngryBot.Models;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DiscordAngryBot.CustomObjects.Groups
{
    /// <summary>
    /// Class for creating groups
    /// </summary>
    public class Group : IDisposable, IReferableTo<GroupReference>
    {
        /// <summary>
        /// Whether object was disposed
        /// </summary>
        bool disposed = false;
        /// <summary>
        /// SafeFileHandle class object
        /// </summary>
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        /// <summary>
        /// Group GUID
        /// </summary>
        public string GUID { get; set; }

        /// <summary>
        /// Group author
        /// </summary>
        public SocketGuildUser Author { get; set; }

        /// <summary>
        /// Channel where this group is being held
        /// </summary>
        public SocketTextChannel Channel { get; set; }

        /// <summary>
        /// User lists
        /// </summary>
        public List<UserList> UserLists { get; set; }

        /// <summary>
        /// Message that represents this group
        /// </summary>
        public RestUserMessage TargetMessage { get; set; }

        /// <summary>
        /// Group creation date timestamp
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Group creation goal
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// Group type
        /// </summary>
        public GroupType Type { get; set; }

        /// <summary>
        /// Memory collection
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets this group reference
        /// </summary>
        /// <returns></returns>
        public GroupReference GetReference()
        {
            return new GroupReference(this);
        }

        /// <summary>
        /// Memory collection
        /// </summary>
        /// <param name="disposing">Wheter already disposed</param>
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
