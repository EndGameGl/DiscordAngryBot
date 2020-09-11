using DiscordAngryBot.Models;
using Newtonsoft.Json.Schema.Generation;
using System.Web.Http;

namespace DiscordAngryBot.APIHandlers.Controllers
{
    /// <summary>
    /// API controller for models definition
    /// </summary>
    [RoutePrefix("api/Models")]
    public class ModelsController : ApiController
    {
        /// <summary>
        /// Gets a ban model definition
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("Ban")]
        public object GetBanModel()
        {
            return new JSchemaGenerator().Generate(typeof(BanReference)).ToString();
        }

        /// <summary>
        /// Gets a group model definition
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("Group")]
        public object GetGroupModel()
        {
            return new JSchemaGenerator().Generate(typeof(GroupReference)).ToString();
        }

        /// <summary>
        /// Gets a group user list model definition
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("UserList")]
        public object GetUserListModel()
        {
            return new JSchemaGenerator().Generate(typeof(UserListReference)).ToString();
        }
    }
}
