using Newtonsoft.Json;
using ObjectDiscordAPI.GatewayOperations.ActivityData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.GatewayOperations
{
    public enum ActivityType
    {
        Game,
        Streaming,
        Listening,
        Custom
    }
    /// <summary>
    /// Activity Object
    /// </summary>
    public class Activity
    {
        /// <summary>
        /// The activity's name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// Activity type
        /// </summary>
        [JsonProperty("type")]
        public ActivityType ActivityType { get; set; }
        /// <summary>
        /// Stream URL, is validated when type is 1
        /// </summary>
        [JsonProperty("url")]
        public string StreamURL { get; set; }
        /// <summary>
        /// Unix timestamp of when the activity was added to the user's session
        /// </summary>
        [JsonProperty("created_at")]
        public ulong CreatedAt { get; set; }
        /// <summary>
        /// Unix timestamps for start and/or end of the game
        /// </summary>
        [JsonProperty("timestamps")]
        public ActivityTimestamps ActivityTimestamp { get; set; }
        /// <summary>
        /// Application id for the game
        /// </summary>
        [JsonProperty("application_id")]
        public ulong ApplicationID { get; set; }
        /// <summary>
        /// What the player is currently doing
        /// </summary>
        [JsonProperty("details")]
        public string Details { get; set; }
        /// <summary>
        /// The user's current party status
        /// </summary>
        [JsonProperty("state")]
        public string State { get; set; }
        /// <summary>
        /// The emoji used for a custom status
        /// </summary>
        [JsonProperty("emoji")]
        public ActivityEmoji ActivityEmoji { get; set; }
        /// <summary>
        /// Information for the current party of the player
        /// </summary>
        [JsonProperty("party")]
        public ActivityParty ActivityParty { get; set; }
        /// <summary>
        /// Images for the presence and their hover texts
        /// </summary>
        [JsonProperty("assets")]
        public ActivityAssets ActivityAssets { get; set; }
        /// <summary>
        /// Secrets for Rich Presence joining and spectating
        /// </summary>
        [JsonProperty("secrets")]
        public ActivitySecrets ActivitySecrets { get; set; }
        /// <summary>
        /// Whether or not the activity is an instanced game session
        /// </summary>
        [JsonProperty("instance")]
        public bool? IsInstanced { get; set; }
        /// <summary>
        /// Activity flags OR'd together, describes what the payload includes
        /// </summary>
        [JsonProperty("flags")]
        public int? Flags { get; set; }

        [JsonConstructor]
        public Activity() { }
    }
}
