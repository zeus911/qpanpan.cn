using System.Linq;
using System.Web.Mvc;

namespace AliYunServer.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            var admins = from admin in Db.AdminUser
                         select admin;

            return View(admins.ToList());
        }
    }
}
