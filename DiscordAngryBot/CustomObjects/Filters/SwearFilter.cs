using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Filters
{
    /// <summary>
    /// Extension class for filtering swears
    /// </summary>
    public static class SwearFilter
    {
        /// <summary>
        /// Swear check for the message
        /// </summary>
        /// <param name="message">Checked socket message</param>
        /// <returns>Whether this message contains swears</returns>
        public static async Task<bool> CheckPhrase(this SocketMessage message)
        {
            return await Task.Run(() =>
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
            });
        }
    }
}
