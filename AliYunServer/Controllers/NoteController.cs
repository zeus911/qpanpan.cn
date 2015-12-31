using System;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Models;

namespace AliYunServer.Controllers
{
    public class NoteController : BaseController
    {
        [ValidateInput(false)]
        [HttpPost]
        public JsonResult Index(string title, string note)
        {
            News news = new News()
            {
                //ID = 
                Title = title,
                Article = note,
                PostDateTime = DateTime.Now,
                NewsCategoryID = 2,
            };
            Db.News.Add(news);
            Db.Entry(news).State = EntityState.Added;
            int row = Db.SaveChanges();

            return Json(new
            {
                Msg = (row > 0) ? "ok" : "error",
            });
        }

        [HttpPost]
        public JsonResult StaticPages()
        {
            string template = Server.MapPath("~/shtml/template.shtml");

            string html;
            using (Stream stream = System.IO.File.OpenRead(template))
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                html = reader.ReadToEnd();
            }

            int count = Db.News.Count();
            int index = 1;
            foreach (News @new in Db.News)
            {
                int last = Math.Max(1, index - 1);
                int next = Math.Min(count, index + 1);
                string temp = html.Replace("@title", @new.Title)
                            .Replace("@postdate", @new.PostDateTime.ToString())
                            .Replace("@article", @new.Article)
                            .Replace("@last", last.ToString()).Replace("@next", next.ToString());
                // ReSharper disable once AssignNullToNotNullAttribute
                string file = Path.Combine(Path.GetDirectoryName(template), "heart", index + ".shtml");
                using (Stream stream = System.IO.File.OpenWrite(file))
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(temp);
                    stream.Write(buffer, 0, buffer.Length);
                }
                index++;
            }

            return Json(new { Msg = "搞定！" });
        }

        [HttpGet]
        public ActionResult Qpanpan(int id)
        {
            News news = Db.News.SingleOrDefault(ns => ns.ID == id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        [ValidateInput(false)]
        [HttpPost]
        public JsonResult Update(int id, string title, string note)
        {
            News news = Db.News.SingleOrDefault(ns => ns.ID == id);
            if (news != null)
            {
                news.Title = title;
                news.Article = note;

                Db.Entry(news).State = EntityState.Modified;
            }

            int row = Db.SaveChanges();
            return Json(new
            {
                Msg = (row > 0) ? "ok" : "error",
            });
        }
    }
}
