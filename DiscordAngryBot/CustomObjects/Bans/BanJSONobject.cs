using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Bans
{
    public class BanJSONobject
    {
        public ulong channel_id { get; set; }
        public ulong banTarget_id { get; set; }
        public ulong role_id { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? endsAt { get; set; }

        public BanJSONobject() { }
        public BanJSONobject(DiscordBan ban)
        {
            channel_id = ban.channel.Id;
            banTarget_id = ban.banTarget.Id;
            role_id = ban.roleToBan.Id;
            createdAt = ban.createdAt;
            endsAt = ban.endsAt;
        }

        public async Task<DiscordBan> ConvertToDiscordBan(SocketGuild guild)
        {
            await ConsoleWriter.Write($"Converting into DiscordBan", ConsoleWriter.InfoType.Notice);
            DiscordBan ban = new DiscordBan()
            {
                banTarget = guild.GetUser(banTarget_id),
                channel = (ISocketMessageChannel)guild.GetChannel(channel_id),
                createdAt = createdAt,
                endsAt = endsAt,
                roleToBan = guild.GetRole(role_id)
            };
            return ban;
        }
    }
}
