using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DiscordAngryBot.APIHandlers.Controllers
{
    [RoutePrefix("api/Birthday")]
    public class UtilsController : ApiController
    {
        [HttpGet, Route("")]
        public string GetTimeUntilBirtday()
        {
            var timeSpan = DateTime.Parse("31.05.2020") - DateTime.Now;
            return $"Осталось {timeSpan.Days} дней, {timeSpan.Hours} часов, {timeSpan.Minutes} минут, {timeSpan.Seconds} секунд";
        }

        [HttpGet, Route("Web")]
        public HttpResponseMessage GetTimeUntilBirtdayAsWebpage()
        {
            var timeSpan = DateTime.Parse("31.05.2020") - DateTime.Now;
            string output = $"Осталось {timeSpan.Days} дней, {timeSpan.Hours} часов, {timeSpan.Minutes} минут, {timeSpan.Seconds} секунд";
            var response = new HttpResponseMessage();
            response.Content = new StringContent($"<div style=\"font-size: 40px; padding: 20px;\">{output}</div>");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            response.Content.Headers.ContentType.CharSet = Encoding.UTF8.HeaderName;
            response.Content.Headers.Add("CodePage", Encoding.UTF8.CodePage.ToString());

            return response;
        }
    }
}
