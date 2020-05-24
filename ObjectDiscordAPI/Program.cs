using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ObjectDiscordAPI.Extensions;
using ObjectDiscordAPI.GatewayData;
using ObjectDiscordAPI.GatewayData.GatewayEvents;
using ObjectDiscordAPI.Resources;
using ObjectDiscordAPI.Resources.GuildResources;

namespace ObjectDiscordAPI
{
    class Program
    {
        static DiscordClient discordClient = new DiscordClient();
        public static List<GatewayEventGuildCreateArgs> guilds = new List<GatewayEventGuildCreateArgs>();
        static void Main(string[] args)
        {
            try
            {
                StartBot().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}: {ex.InnerException.Message}"); 
            }           
        }

        public async static Task StartBot()
        {
            discordClient.SetSettings("NjM1MDEyNzg1MTkyOTYwMDEx.Xnj0TQ.B38NBN5KmbLE89hwUWIjSKk2aII");
            await discordClient.ConnectAsync();
            discordClient.MessageCreated += DisplayNewMessage;
            discordClient.MessageUpdated += DisplayUpdatedMessage;
            discordClient.MessageDeleted += DisplayDeletedMessage;
            await Task.Delay(Timeout.Infinite);
        }

        public async static Task DisplayNewMessage(Message e)
        {
            var guild = await discordClient.GetGuildByIDAsync(e.GuildID.GetValueOrDefault());
            var channel = guild.Channels.Where(x => x.ID == e.ChannelID).SingleOrDefault();
            var memberSent = guild.Members.Where(x => x.User.ID == e.Author.ID).SingleOrDefault();
            if (memberSent.Nickname != null && memberSent.Nickname != "")
                await Task.Run(() => Console.WriteLine($"[{DateTime.Now}] [{guild.Name}] [{channel.Name}] [{memberSent.Nickname}]: {e.Content}"));
            else
                await Task.Run(() => Console.WriteLine($"[{DateTime.Now}] [{guild.Name}] [{channel.Name}] [{memberSent.User.Username}]: {e.Content}"));
        }

        public async static Task DisplayUpdatedMessage(Message e)
        {
            var guild = await discordClient.GetGuildByIDAsync(e.GuildID.GetValueOrDefault());
            var channel = guild.Channels.Where(x => x.ID == e.ChannelID).SingleOrDefault();
            await Task.Run(() => Console.WriteLine($"[{DateTime.Now}] [{guild.Name}] [{channel.Name}] [{e.Author.Username}]: [UPDATED] {e.Content}"));
        }

        public async static Task DisplayDeletedMessage(GatewayEventMessageDeleteArgs e)
        {
            var guild = await discordClient.GetGuildByIDAsync(e.GuildID.GetValueOrDefault());
            var channel = guild.Channels.Where(x => x.ID == e.ChannelID).SingleOrDefault();
            await Task.Run(() => Console.WriteLine($"[{DateTime.Now}] [{guild.Name}] [{channel.Name}]: Message {e.ID} was deleted"));
        }
    }
}
