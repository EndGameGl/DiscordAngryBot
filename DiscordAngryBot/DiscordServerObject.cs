using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot
{
    public class DiscordServerObject
    {
        public SocketGuild server { get; set; }
        public IReadOnlyCollection<SocketGuildChannel> channels { get; set; }
        public IReadOnlyCollection<SocketGuildUser> users { get; set; }
        public DiscordServerObject(DiscordSocketClient client, ulong serverID)
        {
            this.server = client.GetGuild(serverID);
            this.channels = this.server.Channels;
            this.users = this.server.Users;
        }
    }
}
