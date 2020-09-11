using DiscordAngryBot.CustomObjects.ConsoleOutput;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace DiscordAngryBot.APIHandlers
{
    /// <summary>
    /// API server for this bot
    /// </summary>
    public class APIServer
    {
        /// <summary>
        /// Server configuration
        /// </summary>
        private HttpSelfHostConfiguration serverConfig;

        /// <summary>
        /// Server that is responsible for API
        /// </summary>
        private HttpSelfHostServer server;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="serverAddress">Server address</param>
        /// <param name="mediaTypeHeaderValue">Media type header</param>
        public APIServer(string serverAddress, MediaTypeHeaderValue mediaTypeHeaderValue) 
        {
            Debug.Log($"Setting up API configs to {serverAddress} | {mediaTypeHeaderValue.MediaType}", LogInfoType.Notice).GetAwaiter().GetResult();
            serverConfig = new HttpSelfHostConfiguration(serverAddress);
            serverConfig.MapHttpAttributeRoutes();
            serverConfig.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            serverConfig.Formatters.JsonFormatter.SupportedMediaTypes.Add(mediaTypeHeaderValue);
            serverConfig.ReceiveTimeout = new TimeSpan(100);
            server = new HttpSelfHostServer(serverConfig);
        }
        /// <summary>
        /// Start API server
        /// </summary>
        /// <returns></returns>
        public async Task RunAPIServer()
        {            
            await server.OpenAsync();
        }
    }
}
