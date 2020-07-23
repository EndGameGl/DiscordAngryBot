using ObjectDiscordAPI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.CacheData
{
    /// <summary>
    /// Represents cached messages within a channel
    /// </summary>
    public class MessagesCache
    {
        /// <summary>
        /// ID of the guild if channel in a guild
        /// </summary>
        public ulong? GuildID { get; set; }
        /// <summary>
        /// ID of the channel
        /// </summary>
        public ulong ChannelID { get; set; }
        /// <summary>
        /// List of cached messages
        /// </summary>
        public List<Message> cachedData { get; set; }
    }
}
