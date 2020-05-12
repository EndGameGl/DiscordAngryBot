using ObjectDiscordAPI.Resources.GuildResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Resources
{
    public class Invite
    {
        public string code { get; set; }
        public Guild guild { get; set; }
        public Channel channel { get; set; }
        public User inviter { get; set; }
        public User target_user { get; set; }
        public int? target_user_type { get; set; }
        public int? approximate_presence_count { get; set; }
        public int? approximate_member_count { get; set; }
        public Invite() { }
    }
}
