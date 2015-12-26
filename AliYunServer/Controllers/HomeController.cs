using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using log4net;
using Utility;

namespace AliYunServer.Controllers
{
    public class HomeController : BaseController
    {
        ILog logger = LogManager.GetLogger(typeof(HomeController));
        //
        // GET: /Home/
        [HttpGet]
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        public ViewResult Attribution()
        {
            var regions = from r in Db.Region
                          orderby r.GeographicalId ascending, r.Id ascending
                          select r;

            return View(regions.ToList());
        }

        [HttpPost]
        public JsonResult GetAttribution(int identity)
        {
            var attribution =
                Db.IdentityCardNumberAttribution.SingleOrDefault(icna => icna.IdentityCardNumber == identity);
            if (attribution == null)
            {
                logger.Error(UserInformation(Request, identity));
            }

            return Json(new
            {
                Message = (attribution == null) ? "不存在该地区" : attribution.Attribution,
            });
        }

        private string UserInformation(HttpRequestBase request, object identity)
        {
            return string.Format("{0}UserAgent: {1}{0}UserHostAddress：{2}{0}UserHostName：{3}{0}数据库不存在：{4}{0}",
                Environment.NewLine,
                request.UserAgent, request.UserHostAddress, request.UserHostName, identity);
        }


        [HttpPost]
        public JsonResult GetCode(int regionId)
        {
            var codes = from lp in Db.LicensePlate
                        where lp.RegionId == regionId && lp.Area != null
                        select lp.Code;

            return Json(new
            {
                Codes = codes.ToList(),
            });
        }

        [HttpPost]
        public JsonResult GetArea(string regionName, string code)
        {
            try
            {
                var result = from a in Db.LicensePlate
                             join b in Db.Region on new { RegionId = a.RegionId } equals new { RegionId = b.Id } into bJoin
                             from b in bJoin.DefaultIfEmpty()
                             where a.Code == code && b.Name == regionName && a.Area != null
                             select new
                             {
                                 Area = b.FullName + a.Area,
                             };

                return Json(new
                {
                    Area = result.First().Area,
                });
            }
            catch
            {
                logger.Error(UserInformation(Request, regionName + code));

                return Json(new
                {
                    Area = @"不存在这个车牌",
                });
            }
        }


        /// <summary>
        /// 生成二维码，返回连接
        /// </summary>
        [HttpPost]
        public JsonResult CreateQrCode(string content)
        {
            string url = null;
            try
            {
                QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                QrCode qrCode;
                bool ok = qrEncoder.TryEncode(content, out qrCode);
                if (ok)
                {
                    GraphicsRenderer renderer = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), Brushes.Black, Brushes.White);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, stream);

                        UploadResult ur = new UploadResult()
                        {
                            State = UploadState.Unknown,
                            OriginFileName = string.Format("{0}.png", UpYun.StreamMd5(stream)),
                        };
                        ImageOperate imgOpe = new ImageOperate(ur);
                        imgOpe.UpLoad(ur.OriginFileName, stream);
                        url = ur.Url;
                    }
                    //string md5 = UpYun.
                }
            }
            catch (Exception ex)
            {
                url = null;
                logger.Error(ex);
            }
            return Json(new
            {
                Status = (url == null) ? "error" : "ok",
                Url = url,
            });
        }
    }
}