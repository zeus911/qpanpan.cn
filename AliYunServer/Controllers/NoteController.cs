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

            string html = null;
            using (Stream stream = System.IO.File.OpenRead(template))
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                html = reader.ReadToEnd();
            }

            var news = from ns in Db.News
                       select ns;
            int index = 1;
            foreach (News @new in news)
            {
                html = html.Replace("@title", @new.Title)
                            .Replace("@postdate", @new.PostDateTime.ToString(CultureInfo.InvariantCulture))
                            .Replace("@article", @new.Article);
                string file = Path.Combine(Path.GetDirectoryName(template), "heart", index + ".shtml");
                using (Stream stream = System.IO.File.OpenWrite(file))
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(html);
                    stream.Write(buffer, 0, buffer.Length);
                }
                index++;
            }

            return Json(new { Msg = "搞定！" });
        }

    }
}
