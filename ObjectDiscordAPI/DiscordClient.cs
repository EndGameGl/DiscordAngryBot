using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI
{
    public class DiscordClient
    {
        WebClient client { get; set; }
        bool isConfigured { get; set; } = false;

        public void SetSettings(string Token)
        {
            client = new WebClient();
            client.BaseAddress = apiPath.DiscordAPIPath;
            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            client.Headers.Add(HttpRequestHeader.Authorization, $"Bot {Token}");
            isConfigured = true;
        }

        public async Task<string> GET(string parameters)
        {
            if (isConfigured)
            {
                try
                {
                    return await client.DownloadStringTaskAsync(new Uri(client.BaseAddress + parameters));
                }
                catch (Exception ex)
                {
                    throw new Exception("GET string operation error", ex);
                }
            }
            else
                return null;
        }

        public async Task<byte[]> GETFile(string parameters)
        {
            if (isConfigured)
            {
                try
                {
                    return await client.DownloadDataTaskAsync(new Uri(client.BaseAddress + parameters));
                }
                catch (Exception ex)
                {
                    throw new Exception("GET File operation error", ex);
                }
            }
            else
                return null;
        }
    }
}
