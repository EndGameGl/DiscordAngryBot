using DiscordAngryBot.CustomObjects.Groups;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAngryBot.Models
{
    /// <summary>
    /// Reference object for UserList
    /// </summary>
    public class UserListReference
    {
        /// <summary>
        /// List name
        /// </summary>
        public string ListName { get; set; }

        /// <summary>
        /// List emoji
        /// </summary>
        public string Emoji { get; set; }

        /// <summary>
        /// List of user IDs
        /// </summary>
        public List<ulong> UserIDs { get; set; }

        /// <summary>
        /// User limit, if any
        /// </summary>
        public int? UserLimit { get; set; }

        /// <summary>
        /// Class constructor
        /// </summary>
        [JsonConstructor]
        public UserListReference() { }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="userList">UserList object</param>
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
