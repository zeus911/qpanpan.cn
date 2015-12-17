using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace AliYunServer.Controllers
{
    public class CameraController : BaseController
    {
        [HttpGet]
        public ViewResult TakePicture()
        {
            return View();
        }

        static Random _random = new Random();
        [HttpPost]
        public JsonResult SaveImage(string image)
        {
            try
            {
                // 获取base64字符串 
                byte[] imgBytes = Convert.FromBase64String(image);
                string md5 = UpYun.BytesMd5(imgBytes);
                UploadResult ur = new UploadResult()
                {
                    State = UploadState.Unknown,
                    OriginFileName = string.Format("{0}.jpg", _random.Next(100000, 10000000)),
                };

                UpYun upYun = new UpYun("panpan", "panpan", "panpan88")
                {
                    ContentMd5 = md5,
                };

                string file = string.Format("/12310720112/{0}.jpg", md5);
                upYun.WriteFile(file, imgBytes, true);

                return Json(new
                {
                    status = "ok",
                    msg = "http://static.qpanpan.cn " + file,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = "error",
                    msg = ex.Message,
                });
            }
        }
    }
}
