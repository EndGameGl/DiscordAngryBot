using Discord;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
using DiscordAngryBot.Models;
using System.Collections.Generic;
using System.Linq;

namespace DiscordAngryBot.CustomObjects.Groups
{
    /// <summary>
    /// Class for specifying which list user attends for
    /// </summary>
    public class UserList : IReferableTo<UserListReference>
    {
        /// <summary>
        /// List name
        /// </summary>
        public string ListName { get; set; }
        /// <summary>
        /// List emoji
        /// </summary>
        public Emoji ListEmoji { get; set; }
        /// <summary>
        /// List of joined users
        /// </summary>
        public List<SocketGuildUser> Users { get; set; }
        /// <summary>
        /// List user limit, if any
        /// </summary>
        public int? UserLimit { get; set; }
        /// <summary>
        /// Get user list reference
        /// </summary>
        /// <returns></returns>
        public UserListReference GetReference()
        {
            return new UserListReference(this);
        }

        /// <summary>
        /// Remove user from list if found any
        /// </summary>
        /// <param name="ID">User ID</param>
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
        /// Try to join the list if not yet
        /// </summary>
        /// <param name="user">User object</param>
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
