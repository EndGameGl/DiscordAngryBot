using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace DiscordAngryBot.APIHandlers.Controllers
{
    /// <summary>
    /// API controllers for web pages
    /// </summary>
    [RoutePrefix("Web")]
    public class WebController : ApiController
    {
        /// <summary>
        /// Gets all guild groups as a web page
        /// </summary>
        /// <param name="guildID">Guild ID</param>
        /// <returns></returns>
        [HttpGet, Route("{guildID}/Groups")]
        public HttpResponseMessage GetFullGroupsWeb(string guildID)
        {
            if (BotCore.TryGetDiscordGuildGroups(ulong.Parse(guildID), out var groups))
            {

                var response = new HttpResponseMessage();

                string htmlCode =
                    new WebPage()
                    .SetDoctype()
                    .AddHeaderLink("stylesheet", @"https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css", "sha384-9aIt2nRpC12Uk9gS9baDl411NQApFmC26EwAOH8WgZl5MYYxFfc+NcPb1dKGj7Sk", "anonymous")
                    .AddStyle("div {padding: 20px;}")
                    .AddOpenDiv("container p-3 my-3 bg-dark text-white")
                    .AddBodyHeader("Список полных групп", 1)
                    .AddCloseDiv()
                    .AddOpenDiv("container")
                    .AddTable(
                        new string[] { "Тип группы", "Создатель группы", "Цель создания", "Канал группы", "Список участников", "Время создания" },
                        groups.Select(x =>
                        new
                        {
                            Type = x.GetType().ToString(),
                            x.Author.Username,
                            x.Destination,
                            Channel = x.Channel.Name,
                            Users = x.UserLists.Select(x => x.Users.Select(x => x.Username)).ToList(),
                            DateCreated = x.CreatedAt
                        }).ToList(),
                        "table table-stripped")
                    .AddCloseDiv()
                    .Build();

                response.Content = new StringContent(htmlCode);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html")
                {
                    CharSet = Encoding.UTF8.HeaderName
                };
                response.Content.Headers.Add("CodePage", Encoding.UTF8.CodePage.ToString());

                return response;
            }
            else
                return null;
        }

        /// <summary>
        /// Gets all guild users as a web page
        /// </summary>
        /// <param name="guildID">Guild ID</param>
        /// <returns></returns>
        [HttpGet, Route("{guildID}/Users")]
        public HttpResponseMessage GetUsersWebTest(string guildID)
        {
            if (BotCore.TryGetExtendedDiscordGuildBotData(ulong.Parse(guildID), out var cache))
            {
                var users = cache.Guild.Users;

                var response = new HttpResponseMessage();

                string htmlCode =
                    new WebPage()
                    .SetDoctype()
                    .AddHeaderLink("stylesheet", @"https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css", "sha384-9aIt2nRpC12Uk9gS9baDl411NQApFmC26EwAOH8WgZl5MYYxFfc+NcPb1dKGj7Sk", "anonymous")
                    .AddStyle("div {padding: 20px;}")
                    .AddOpenDiv("container p-3 my-3 bg-dark text-white")
                    .AddBodyHeader("Список юзеров сервера", 1)
                    .AddCloseDiv()
                    .AddOpenDiv("container")
                    .AddTable(
                        new string[]
                        {
                        "Имя юзера",
                        "Никнейм",
                        "Роли",
                        "Дата вступления"
                        },
                        users.Select(x => new
                        {
                            x.Username,
                            x.Nickname,
                            Roles = x.Roles.Select(x => new { x.Name }).ToList(),
                            x.JoinedAt
                        }).ToList(),
                        "table table - stripped")
                    .AddCloseDiv()
                    .Build();

                response.Content = new StringContent(htmlCode);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html")
                {
                    CharSet = Encoding.UTF8.HeaderName
                };
                response.Content.Headers.Add("CodePage", Encoding.UTF8.CodePage.ToString());

                return response;
            }
            else
                return null;
        }

        /// <summary>
        /// Generated an HTML page with logs
        /// </summary>
        /// <returns></returns>
        [Route("Logs")]
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
                        Type = x.LogInfo,
                        x.Message,
                        Time = x.Time.ToString()
                    }).ToList(),
                    "table table-stripped")
                .AddCloseDiv()
                .Build();

            response.Content = new StringContent(htmlCode);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html")
            {
                CharSet = Encoding.UTF8.HeaderName
            };
            response.Content.Headers.Add("CodePage", Encoding.UTF8.CodePage.ToString());

            return response;
        }
    }
}
