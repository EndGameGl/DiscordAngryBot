using Discord;
using Discord.Rest;
using Discord.WebSocket;
using DiscordAngryBot.Models;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace DiscordAngryBot.CustomObjects.Groups
{
    /// <summary>
    /// Абстрактный класс, предстравляющий группу пользователей
    /// </summary>
    public class Group : IDisposable, IReferableTo<GroupReference>
    {
        /// <summary>
        /// Признак того, был ли удален объект
        /// </summary>
        bool disposed = false;
        /// <summary>
        /// Объект SafeFileHandle
        /// </summary>
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        /// <summary>
        /// Уникальный идентификатор группы
        /// </summary>
        public string GUID { get; set; }
        /// <summary>
        /// Пользователь, создавший группу
        /// </summary>
        public SocketGuildUser Author { get; set; }
        /// <summary>
        /// Канал, в котором была создана группа
        /// </summary>
        public SocketTextChannel Channel { get; set; }
        /// <summary>
        /// Собираемые группы пользователей
        /// </summary>
        public List<UserList> UserLists { get; set; }
        /// <summary>
        /// Сообщение, представляющее группу в дискорде
        /// </summary>
        public RestUserMessage TargetMessage { get; set; }
        /// <summary>
        /// Дата создания группы
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Описание цели сбора группы
        /// </summary>
        public string Destination { get; set; }
        /// <summary>
        /// Тип группы
        /// </summary>
        public GroupType Type { get; set; }

        /// <summary>
        /// Метод вызова сбора памяти
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public GroupReference GetReference()
        {
            return new GroupReference(this);
        }

        /// <summary>
        /// Метод сбора памяти
        /// </summary>
        /// <param name="disposing"></param>
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
