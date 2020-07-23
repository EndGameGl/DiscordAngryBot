using DiscordAngryBot.CustomObjects.ConsoleOutput;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace DiscordAngryBot.APIHandlers
{
    /// <summary>
    /// Класс-обертка для класса HttpSelfHostServer
    /// </summary>
    public class APIServer
    {
        /// <summary>
        /// Конфигурация сервера
        /// </summary>
        private HttpSelfHostConfiguration serverConfig { get; set; }
        /// <summary>
        /// Сервер, поддерживающий работу API
        /// </summary>
        private HttpSelfHostServer server { get; set; }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="serverAddress"></param>
        /// <param name="mediaTypeHeaderValue"></param>
        public APIServer(string serverAddress, MediaTypeHeaderValue mediaTypeHeaderValue) 
        {
            ConsoleWriter.Write($"Setting up API configs to {serverAddress} | {mediaTypeHeaderValue.MediaType}", ConsoleWriter.InfoType.Notice).GetAwaiter().GetResult();
            this.serverConfig = new HttpSelfHostConfiguration(serverAddress);
            this.serverConfig.MapHttpAttributeRoutes();
            this.serverConfig.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            this.serverConfig.Formatters.JsonFormatter.SupportedMediaTypes.Add(mediaTypeHeaderValue);
            this.serverConfig.ReceiveTimeout = new TimeSpan(100);
            this.server = new HttpSelfHostServer(serverConfig);
        }
        /// <summary>
        /// Запуск сервера API
        /// </summary>
        /// <returns></returns>
        public async Task RunAPIServer()
        {            
            await this.server.OpenAsync();
        }
    }
}
