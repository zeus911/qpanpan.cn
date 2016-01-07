using Models;
using System.Web.Mvc;

namespace AliYunServer.Controllers
{
    public class BaseController : Controller
    {
        protected qds173643455_db Db = new qds173643455_db();

        /// <summary>
        /// JSON状态码， error
        /// </summary>
        protected static string Error = "error";

        /// <summary>
        /// JSON状态码，ok
        /// </summary>
        protected static string Ok = "ok";
    }
}
