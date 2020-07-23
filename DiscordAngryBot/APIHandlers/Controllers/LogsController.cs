using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace DiscordAngryBot.APIHandlers.Controllers
{
    /// <summary>
    /// API контроллер для логов
    /// </summary>
    [RoutePrefix("api/web/Logs")]
    public class LogsController : ApiController
    {
        /// <summary>
        /// Генерация HTML страницы для вывода пользователю
        /// </summary>
        /// <returns></returns>
        [Route("")]
        public HttpResponseMessage GetLogsPage()
        {
            var logs = BotCore.GetDataLogs();

            var response = new HttpResponseMessage();

            string htmlCode =
                new WebPage()
                .SetDoctype()
                .AddHeaderLink("stylesheet", @"https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css", "sha384-9aIt2nRpC12Uk9gS9baDl411NQApFmC26EwAOH8WgZl5MYYxFfc+NcPb1dKGj7Sk", "anonymous")
                .AddStyle("div {padding: 20px;}")
                .AddOpenDiv("container p-3 my-3 bg-dark text-white")
                .AddBodyHeader("Логи", 1)
                .AddCloseDiv()
                .AddOpenDiv("container")
                .AddTable(
                    new string[] { "Событие", "Сообщение", "Время" },
                    logs.Select(x =>
                    new {
                        Type = x.LogType,
                        Message = x.GetMessage(),
                        Time = x.Time.ToString()
                    }).ToList(),
                    "table table-stripped")
                .AddCloseDiv()
                .Build();

            response.Content = new StringContent(htmlCode);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            response.Content.Headers.ContentType.CharSet = Encoding.UTF8.HeaderName;
            response.Content.Headers.Add("CodePage", Encoding.UTF8.CodePage.ToString());

            return response;
        }
    }
}
