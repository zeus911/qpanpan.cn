using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Models;

namespace AliYunServer.Controllers
{
    public class AccountController : BaseController
    {
        /// <summary>
        /// 密码盐
        /// </summary>
        private static string _salt = "4IbIMoKKSyMGjm8wnHapOMSX7g4N5J5wknWZpxex4krUBOA8BvNy75TlwYK1uDxDUT5sBFtn7Yp63mGhetj8YtuIWdJUFddcXaJtUURUn066EPskyLtGXt0Do3iD8Hslyqlan3Q7Q3or7S31RWkOvswCeaG48Kc2iv6GonKv6dcps9Dp4VQCEj3p4TrtH45w3OZljt";

        [HttpGet]
        public ViewResult Login()
        {
            object a = HttpContext.User;
            //进入登录页面，首先删除用户验证。
            FormsAuthentication.SignOut();

            ViewBag.userName = string.Empty;
            if (Request.Cookies["username"] != null)
            {
                ViewBag.userName = Request.Cookies["username"].Value;
            }
            SessionHelper.StoreInSession(HttpContext, KeyHelper.AccountReturnUrl, Request.QueryString["ReturnUrl"]);
            return View();
        }

        /// <summary>
        /// 执行登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="security">验证码</param>
        /// <param name="remember">是否记住用户名</param>
        /// <returns>登录结果</returns>
        [HttpPost]
        public JsonResult Login(string username, string password, string security, bool remember)
        {
            string server = SessionHelper.TakeFromSession<string>(HttpContext, KeyHelper.AccountLoginSecurityCode);
            SessionHelper.StoreInSession(HttpContext, KeyHelper.AccountLoginSecurityCode, string.Empty);
            if (security != server)
            {
                return Json(new
                {
                    status = Error,
                    msg = "验证码错误",
                });
            }

            string hashPwd = CommonHelper.StringMd5(password + _salt);
            AdminUser admin = Db.AdminUser.Where(a => a.IsEnabled)
                    .SingleOrDefault(a => a.UserName == username && a.Password == hashPwd);
            if (admin == null)
            {
                return Json(new
                {
                    status = Error,
                    msg = "用户名或密码错误",
                });
            }

            //创建用户身份验证，与[Authorize]特性配合使用，
            //所有控制器的action如果打了Authorize的特性 则参与用户身份验证
            //验证失败的跳转URL,验证的超时时间 都由web.config里面的<authentication>标签决定
            FormsAuthentication.SetAuthCookie(username, true);
            if (remember)
            {
                HttpCookie cookie = new HttpCookie("username", username)
                {
                    Expires = DateTime.Now.AddDays(7),
                };
                Response.Cookies.Add(cookie);
            }

            return Json(new
            {
                status = Ok,
                nextUrl = SessionHelper.TakeFromSession<string>(HttpContext, KeyHelper.AccountReturnUrl),
            });
        }

        [HttpGet]
        public FileContentResult SecurityCode()
        {
            string security;
            byte[] buffer = SecurityCodeHelper.CreateSecurityCode(out security);
            SessionHelper.StoreInSession(HttpContext, KeyHelper.AccountLoginSecurityCode, security);
            return File(buffer, "image/jpeg");
        }
    }
}
