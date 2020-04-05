﻿using Discord.Rest;
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
    /// <summary>
    /// Абстрактный класс, предстравляющий группу пользователей
    /// </summary>
    public abstract class Group : IDisposable
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
        public SocketUser author { get; set; }
        /// <summary>
        /// Канал, в котором была создана группа
        /// </summary>
        public ISocketMessageChannel channel { get; set; }
        /// <summary>
        /// Список пользователей, состоящих в группе
        /// </summary>
        public List<SocketUser> users { get; set; }
        /// <summary>
        /// Ограничение на количество пользователей в группе
        /// </summary>
        public int userLimit { get; set; } 
        /// <summary>
        /// Сообщение, представляющее группу в дискорде
        /// </summary>
        public RestUserMessage targetMessage { get; set; }
        /// <summary>
        /// Дата создания группы
        /// </summary>
        public DateTime createdAt { get; set; }
        /// <summary>
        /// Описание цели сбора группы
        /// </summary>
        public string destination { get; set; }
        /// <summary>
        /// Признак того, ведется ли набор в группе
        /// </summary>
        public bool isActive { get; set; }
        /// <summary>
        /// Метод вызова сбора памяти
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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