using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace DiscordAngryBot.APIHandlers.Controllers
{
    /// <summary>
    /// API контроллер для генерации веб страниц
    /// </summary>
    [RoutePrefix("Web")]
    public class WebController : ApiController
    {
        /// <summary>
        /// Получение списка групп сервера в виде веб-страницы
        /// </summary>
        /// <param name="guildID"></param>
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
                            Username = x.Author.Username,
                            Destination = x.Destination,
                            Channel = x.Channel.Name,
                            Users = x.UserLists.Select(x => x.Users.Select(x => x.Username)).ToList(),
                            DateCreated = x.CreatedAt
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
            else
                return null;
        }
        /// <summary>
        /// Получение списка пользователей сервера в виде веб-страницы
        /// </summary>
        /// <param name="guildID"></param>
        /// <returns></returns>
        [HttpGet, Route("{guildID}/Users/Test")]
        public HttpResponseMessage GetUsersWebTest(string guildID)
        {
            if (BotCore.TryGetGuildDataCache(ulong.Parse(guildID), out var cache))
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
                            Username = x.Username,
                            Nickname = x.Nickname,
                            Roles = x.Roles.Select(x => new { Name = x.Name }).ToList(),
                            JoinedAt = x.JoinedAt
                        }).ToList(),
                        "table table - stripped")
                    .AddCloseDiv()
                    .Build();

                response.Content = new StringContent(htmlCode);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                response.Content.Headers.ContentType.CharSet = Encoding.UTF8.HeaderName;
                response.Content.Headers.Add("CodePage", Encoding.UTF8.CodePage.ToString());

                return response;
            }
            else
                return null;
        }
    }
}
