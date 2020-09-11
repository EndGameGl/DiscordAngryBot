using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace DiscordAngryBot.APIHandlers.Controllers
{
    /// <summary>
    /// API controller for logs
    /// </summary>
    [RoutePrefix("api/Logs")]
    public class LogsController : ApiController
    {       
        /// <summary>
        /// Gets list of logs
        /// </summary>
        /// <returns></returns>
        [Route("")]
        public object GetLogs()
        {
            return BotCore.GetDataLogs().Select(x => new { LogType = x.LogInfo, x.Message, x.Time });
        }
    }
}
