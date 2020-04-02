using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Parties
{
    /// <summary>
    /// Интерфейс для обобщения групп
    /// </summary>
    public interface IGroup
    {
        /// <summary>
        /// Список юзеров
        /// </summary>
        List<SocketUser> users { get; set; }
        /// <summary>
        /// Лимит юзеров в пределах одной группы
        /// </summary>
        int userLimit { get; set; }
        /// <summary>
        /// Сообщение-команда, запустившее пати
        /// </summary>
        SocketMessage sourceMessage { get; set; }
        /// <summary>
        /// Сообщение, с которым идет работа
        /// </summary>
        RestUserMessage targetMessage { get; set; }
        /// <summary>
        /// Дата создания сообщения
        /// </summary>
        DateTime created { get; set; }
        /// <summary>
        /// Описание группы
        /// </summary>
        string partyDestination { get; set; }
        /// <summary>
        /// Путь до файла группы
        /// </summary>
        string localPath { get; set; }
        /// <summary>
        /// Объект, загруженный из файла
        /// </summary>
        GroupIO loadedInfo { get; set; }
        /// <summary>
        /// Признак того, была ли загруженна группа из файла
        /// </summary>
        bool isLoadedFromFile { get; set; }
        /// <summary>
        /// Инициатор группы
        /// </summary>
        SocketUser author { get; set; }
        /// <summary>
        /// Метод для определения типа группы
        /// </summary>
        /// <returns></returns>
        public bool IsParty();
        /// <summary>
        /// Метод для определения типа группы
        /// </summary>
        /// <returns></returns>
        public bool IsRaid();
        /// <summary>
        /// Задача для сохранения группы
        /// </summary>
        /// <returns></returns>
        public Task Save();
        /// <summary>
        /// Задача для загрузки группы
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public Task<IGroup> Load(string text);
    }
}
