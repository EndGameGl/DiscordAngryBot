using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ObjectDiscordAPI.Extensions;
using ObjectDiscordAPI.GatewayData;
using ObjectDiscordAPI.Resources;
using ObjectDiscordAPI.Resources.GuildResources;

namespace ObjectDiscordAPI
{
    class Program
    {
        static DiscordClient discordClient = new DiscordClient();
        static void Main(string[] args)
        {
            try
            {
                var result = DoStuff().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}: {ex.InnerException.Message}"); 
            }

            Console.ReadLine();
        }

        public async static Task<object> DoStuff()
        {
            discordClient.SetSettings("NjM1MDEyNzg1MTkyOTYwMDEx.Xnj0TQ.B38NBN5KmbLE89hwUWIjSKk2aII");
            await discordClient.ConnectAsync();
            discordClient.Ready += RunOnReady;

            return await discordClient.GetGuildChannelsAsync(636208919114547212);
        }

        public async static Task RunOnReady()
        {
            Console.WriteLine("Ready handler is running");
        }
    }
}
