using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Resources
{
    public enum ExpireBehavior
    {
        RemoveRole,
        Kick
    }
    public class Integration
    {
        public ulong id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public bool enabled { get; set; }
        public bool syncing { get; set; }
        public ulong role_id { get; set; }
        public bool? enable_emoticons { get; set; }
        public ExpireBehavior expire_behavior { get; set; }
        public int expire_grace_period { get; set; }
        public User user { get; set; }
        public IntegrationAccount account { get; set; }
        public DateTime synced_at { get; set; }
        public Integration() { }
    }
}
