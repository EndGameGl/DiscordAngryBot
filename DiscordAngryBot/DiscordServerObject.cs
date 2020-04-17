using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
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
            ConsoleWriter.Write($"Fetching server object...", ConsoleWriter.InfoType.Notice);
            this.server = client.GetGuild(serverID);
            ConsoleWriter.Write($"Server: {server.Name}", ConsoleWriter.InfoType.Notice);
            this.channels = this.server.Channels;
            ConsoleWriter.Write($"Collected {channels.Count} channels", ConsoleWriter.InfoType.Notice);
            this.users = this.server.Users;
            ConsoleWriter.Write($"Collected {users.Count} users", ConsoleWriter.InfoType.Notice);
        }
    }
}
