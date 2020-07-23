using Newtonsoft.Json;
using System;

namespace ObjectDiscordAPI.Resources
{
    public enum ExpireBehavior
    {
        RemoveRole,
        Kick
    }
    public class Integration
    {
        [JsonProperty("id")]
        public ulong ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("enabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("syncing")]
        public bool IsSyncing { get; set; }

        [JsonProperty("role_id")]
        public ulong RoleID { get; set; }

        [JsonProperty("enable_emoticons")]
        public bool? DoEnableEmoticons { get; set; }

        [JsonProperty("expire_behavior")]
        public ExpireBehavior ExpireBehavior { get; set; }

        [JsonProperty("expire_grace_period")]
        public int ExpireGracePeriod { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("account")]
        public IntegrationAccount IntegrationAccount { get; set; }

        [JsonProperty("synced_at")]
        public DateTime SyncedAt { get; set; }

        [JsonConstructor]
        public Integration() { }
    }
}
