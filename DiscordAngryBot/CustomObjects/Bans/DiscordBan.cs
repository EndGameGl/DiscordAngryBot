using Discord.WebSocket;
using DiscordAngryBot.Models;
using System;
using System.Threading;

namespace DiscordAngryBot.CustomObjects.Bans
{
    /// <summary>
    /// Object representing custom discord ban
    /// </summary>
    public class DiscordBan : IReferableTo<BanReference>
    {
        /// <summary>
        /// Ban GUID
        /// </summary>
        public string GUID { get; set; }

        /// <summary>
        /// Ban originating channel
        /// </summary>
        public SocketTextChannel Channel { get; set; }

        /// <summary>
        /// Banned user
        /// </summary>
        public SocketGuildUser BanTarget { get; set; }

        /// <summary>
        /// Ban timer, if any
        /// </summary>
        public Timer BanTimer { get; set; }
        
        /// <summary>
        /// Date timestamp
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date when this ban is supposedly ends, if any
        /// </summary>
        public DateTime? EndsAt { get; set; }

        /// <summary>
        /// Gets this ban reference
        /// </summary>
        /// <returns></returns>
        public BanReference GetReference()
        {
            return new BanReference(this);
        }
    }
}
