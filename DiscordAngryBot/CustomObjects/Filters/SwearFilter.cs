using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Filters
{
    /// <summary>
    /// Класс для фильтрации матов
    /// </summary>
    public static class SwearFilter
    {
        /// <summary>
        /// Проверка на мат
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task<bool> CheckPhrase(this SocketMessage message)
        {
            bool doesContainSwear = false;
            if (message.Content.Count() != 0)
            {
                foreach (var word in BotCore.Swears())
                {
                    if (message.Content.ToUpper().Replace(" ", "").Contains(word))
                    {
                        doesContainSwear = true;
                    }
                }
            }
            return doesContainSwear;
        }
    }
}
