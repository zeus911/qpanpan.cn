using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Models;

namespace AliYunServer.Controllers
{
    [Authorize]
    public class NoteController : BaseController
    {
        [ValidateInput(false)]
        [HttpPost]
        public JsonResult Index(string title, string note, long categoryId)
        {
            News news = new News()
            {
                //ID = 
                Title = title,
                Article = note,
                PostDateTime = DateTime.Now,
                NewsCategoryID = categoryId,
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


            foreach (IGrouping<long, News> news in Db.News.GroupBy(n => n.NewsCategoryID))
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                string directory = Path.Combine(Path.GetDirectoryName(template), "heart", news.Key.ToString());
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                IEnumerable<News> groupNews = news.Select(n => n);
                int index = 0;
                int count = groupNews.Count();
                foreach (News @new in groupNews)
                {
                    //long last = Math.Max(groupNews.ElementAt(0).ID, groupNews.ElementAt().ID);
                    //if (last == index)
                    //{
                    //    last = groupNews.Last().ID;
                    //}
                    //long next = Math.Min(count, index + 1);
                    //if (next == index)
                    //{
                    //    next = groupNews.First().ID;
                    //}
                    long last, next;
                    if (index == 0)
                    {
                        last = groupNews.Last().ID;
                        next = groupNews.ElementAt(index + 1).ID;
                    }
                    else if (index == count - 1)
                    {
                        last = groupNews.ElementAt(index - 1).ID;
                        next = groupNews.First().ID;
                    }
                    else
                    {
                        last = groupNews.ElementAt(index - 1).ID;
                        next = groupNews.ElementAt(index + 1).ID;
                    }
                    string temp = html.Replace("@title", @new.Title)
                                        .Replace("@postdate", @new.PostDateTime.ToString())
                                        .Replace("@article", @new.Article)
                                        .Replace("@category", news.Key.ToString())
                                        .Replace("@last", last.ToString()).Replace("@next", next.ToString());

                    string file = Path.Combine(directory, @new.ID + ".shtml");

                    if (System.IO.File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                    }

                    using (Stream stream = System.IO.File.OpenWrite(file))
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(temp);
                        stream.Write(buffer, 0, buffer.Length);
                    }
                    index++;
                }
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
