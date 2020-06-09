using DiscordAngryBot.CustomObjects.ConsoleOutput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace DiscordAngryBot.APIHandlers
{
    public class APIServer
    {
        private HttpSelfHostConfiguration serverConfig { get; set; }
        public HttpSelfHostServer server { get; set; }

        public APIServer() { }

        public void ConfigServer(string serverAddress, MediaTypeHeaderValue mediaTypeHeaderValue, bool needIndent)
        {
            ConsoleWriter.Write($"Setting up API configs to {serverAddress} | {mediaTypeHeaderValue.MediaType} | indent: {needIndent}", ConsoleWriter.InfoType.Notice).GetAwaiter().GetResult();
            this.serverConfig = new HttpSelfHostConfiguration(serverAddress);
            this.serverConfig.MapHttpAttributeRoutes();
            this.serverConfig.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            this.serverConfig.Formatters.JsonFormatter.SupportedMediaTypes.Add(mediaTypeHeaderValue);
            this.serverConfig.ReceiveTimeout = new TimeSpan(100);
            
        }

        public async Task RunAPIServer()
        {
            server = new HttpSelfHostServer(serverConfig);
            await server.OpenAsync();
        }
    }
}
