﻿using Newtonsoft.Json;
using ObjectDiscordAPI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayData.GatewayEvents
{
    public class GatewayEventMessageReactionRemovedArgs
    {
        /// <summary>
        /// The id of the user
        /// </summary>
        [JsonProperty("user_id")]
        public ulong UserID { get; set; }
        /// <summary>
        /// The id of the channel
        /// </summary>
        [JsonProperty("channel_id")]
        public ulong ChannelID { get; set; }
        /// <summary>
        /// The id of the message
        /// </summary>
        [JsonProperty("message_id")]
        public ulong MessageID { get; set; }
        /// <summary>
        /// The id of the guild
        /// </summary>
        [JsonProperty("guild_id")]
        public ulong? GuildID { get; set; }
        /// <summary>
        /// The emoji used to react
        /// </summary>
        [JsonProperty("emoji")]
        public Emoji Emoji { get; set; }

        [JsonConstructor]
        public GatewayEventMessageReactionRemovedArgs() { }
    }
}
