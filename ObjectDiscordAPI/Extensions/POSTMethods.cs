using Newtonsoft.Json;
using ObjectDiscordAPI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Extensions
{
    public static class POSTMethods
    {
        public static async Task SendMessage(this Channel channel, string messageText)
        {
            await DiscordClient.POST($"channels/{channel.ID}/messages", JsonConvert.SerializeObject(new { content = messageText, tts = false, embed = "" }));
        }
    }
}
