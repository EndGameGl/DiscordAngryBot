using DiscordAngryBot.CustomObjects.Groups;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAngryBot.Models
{
    /// <summary>
    /// Класс, предоставляющий упрощенную сводку данных класса UserList
    /// </summary>
    public class UserListReference
    {
        /// <summary>
        /// Название списка
        /// </summary>
        public string ListName { get; set; }
        /// <summary>
        /// Эмодзи для попадания в список
        /// </summary>
        public string Emoji { get; set; }
        /// <summary>
        /// Идентификаторы пользователей в списке
        /// </summary>
        public List<ulong> UserIDs { get; set; }
        /// <summary>
        /// Лимит пользователей в списке
        /// </summary>
        public int? UserLimit { get; set; }
        [JsonConstructor]
        public UserListReference() { }
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="userList"></param>
        public UserListReference(UserList userList)
        {
            ListName = userList.ListName;
            Emoji = userList.ListEmoji.Name;
            UserIDs = new List<ulong>();
            foreach (var user in userList.Users)
            {
                UserIDs.Add(user.Id);
            }
            UserLimit = userList.UserLimit;
        }
    }
}
