using Discord;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
using DiscordAngryBot.Models;
using System.Collections.Generic;
using System.Linq;

namespace DiscordAngryBot.CustomObjects.Groups
{
    /// <summary>
    /// Класс, представляющий пользователей, привязанных к конкретному списку в группе
    /// </summary>
    public class UserList : IReferableTo<UserListReference>
    {
        /// <summary>
        /// Название списка
        /// </summary>
        public string ListName { get; set; }
        /// <summary>
        /// Эмодзи, отвечающее за вступление в список
        /// </summary>
        public Emoji ListEmoji { get; set; }
        /// <summary>
        /// Список пользователей
        /// </summary>
        public List<SocketGuildUser> Users { get; set; }
        /// <summary>
        /// Лимит пользователей списка
        /// </summary>
        public int? UserLimit { get; set; }
        public UserListReference GetReference()
        {
            return new UserListReference(this);
        }

        /// <summary>
        /// Убирает пользователя из списка, если он в нем находится
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool RemoveUserIfExists(ulong ID)
        {
            var user = Users.FirstOrDefault(x => x.Id == ID);
            if (user != null)
            {
                Users.Remove(user);
                return true;
            }
            else return false;
        }
        /// <summary>
        /// Попытка попасть в список, если юзера там еще нет
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool TryJoin(SocketGuildUser user)
        {

            if (Users.FirstOrDefault(x => x.Id == user.Id) == null)
            {
                Debug.Log($"User is not list: {Users.Count} users in list. Adding to the list.").GetAwaiter().GetResult();
                Users.Add(user);
                Debug.Log($"User added: {Users.Count} users in list.").GetAwaiter().GetResult();
                return true;
            }
            else
            {
                Debug.Log($"User is already in list.").GetAwaiter().GetResult();
                return false;
            }
        }
    }
}
