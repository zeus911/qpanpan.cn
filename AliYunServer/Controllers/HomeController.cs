using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;

namespace AliYunServer.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/
        [HttpGet]
        public ViewResult Index()
        {
            var admins = from admin in Db.AdminUser
                         select admin;

            return View(admins.ToList());
        }

        [HttpGet]
        public ViewResult Attribution()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetAttribution(int identity)
        {
            var attribution = Db.IdentityCardNumberAttribution.SingleOrDefault(icna => icna.IdentityCardNumber == identity);
            if (attribution == null)
            {
                ILog logger = LogManager.GetLogger(typeof(HomeController));
                logger.Error(UserInformation(Request, identity));
            }

            return Json(new
            {
                Message = (attribution == null) ? "不存在该地区" : attribution.Attribution,
            });
        }

        private string UserInformation(HttpRequestBase request, int identity)
        {
            return string.Format("{0}UserAgent: {1}{0}UserHostAddress：{2}{0}UserHostName：{3}{0}数据库不存在地区代码：{4}{0}", Environment.NewLine,
                request.UserAgent, request.UserHostAddress, request.UserHostName, identity);
        }
    }
}
