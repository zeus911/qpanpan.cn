using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using log4net;
using Utility;

namespace AliYunServer
{
    class ImageOperate
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ImageOperate));
        public UploadResult Result { get; private set; }

        public ImageOperate(UploadResult result)
        {
            this.Result = result;
        }

        private string UpYunPath { get; set; }

        public ImageOperate(UploadResult result, string upYunPath)
        {
            this.Result = result;
            this.UpYunPath = upYunPath;
        }
        public object UpLoad(string imageName, Stream imageStream)
        {
            object model;
            try
            {
                byte[] imageBytes = UpYun.StreamToBytes(imageStream);

                string upYunFilePath = string.Format("/images/qrcode/{0}", imageName);
                UpYun upYun = new UpYun("panpan", "panpan", "panpan88")
                {
                    ContentMd5 = UpYun.BytesMd5(imageBytes),
                };
                bool success = upYun.WriteFile(upYunFilePath, imageBytes, true);
                if (success)
                {
                    // 返回图片在UpYun中的http访问地址
                    Result.Url = "http://static.qpanpan.cn" + upYunFilePath;
                    Result.State = UploadState.Success;
                }
                else
                {
                    Result.State = UploadState.FileAccessError;
                    Result.ErrorMessage = "上传图片到又拍云失败";
                    Logger.Error(Result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                Result.State = UploadState.FileAccessError;
                Result.ErrorMessage = ex.Message;
                Logger.Error(Result.ErrorMessage);
            }
            finally
            {
                model = new
                {
                    state = GetStateMessage(Result.State),
                    url = Result.Url,
                    title = Result.OriginFileName,
                    original = Result.OriginFileName,
                    error = Result.ErrorMessage
                };
            }
            return model;
        }

        private string GetStateMessage(UploadState state)
        {
            switch (state)
            {
                case UploadState.Success:
                    return "SUCCESS";
                case UploadState.FileAccessError:
                    return "文件访问出错，请检查写入权限";
                case UploadState.SizeLimitExceed:
                    return "文件大小超出服务器限制";
                case UploadState.TypeNotAllow:
                    return "不允许的文件格式";
                case UploadState.NetworkError:
                    return "网络错误";
            }
            return "未知错误";
        }
    }

    public static class SecurityCodeHelper
    {
        static readonly Random Rand = new Random();

        /// <summary>
        /// 生成一个验证码图片
        /// </summary>
        /// <param name="securityCode">生成的验证码明文</param>
        /// <returns>图片的JPEG格式的二进制数据</returns>
        public static byte[] CreateSecurityCode(out string securityCode)
        {
            using (Bitmap bmp = new Bitmap(112, 28))
            using (Graphics g = Graphics.FromImage(bmp))
            using (Font font = new Font(FontFamily.GenericSansSerif, 18, FontStyle.Bold))
            using (MemoryStream ms = new MemoryStream())
            {
                g.Clear(Color.Yellow); // 图片背景黄色
                securityCode = GenerateSecurityCode(); // 验证码明文
                g.DrawString(securityCode, font, Brushes.Red, new PointF(0, 0));
                for (int i = 0; i < (64 + 32); i++)
                {
                    int width = Rand.Next(0, 101);
                    int height = Rand.Next(0, 26);
                    g.DrawRectangle(Pens.Red, width, height, 1, 1);
                }
                bmp.Save(ms, ImageFormat.Jpeg);
                ms.Position = 0;
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 生成一个随机的验证码
        /// </summary>
        /// <param name="length">字符串长度</param>
        /// <returns></returns>
        private static string GenerateSecurityCode(int length = 4)
        {
            //常用汉字
            const string cyhz = "一乙二十丁厂七卜人入八九几儿了力乃刀又三于干亏士工土才寸下大丈与万上小口巾山千乞川亿个勺久凡及夕丸么" +
                                "广亡门义之尸弓己已子卫也女飞刃习叉马乡丰王井开夫天无元专云扎艺木五支厅不太犬区历尤友匹车巨牙屯比互切" +
                                "瓦止少日中冈贝内水见午牛手毛气升长仁什片仆化仇币仍仅斤爪反介父从今凶分乏公仓月氏勿欠风丹匀乌凤勾文六" +
                                "方火为斗忆订计户认心尺引丑巴孔队办以允予劝双书幻玉刊示末未击打巧正扑扒功扔去甘世古节本术可丙左厉右石" +
                                "布龙平灭轧东卡北占业旧帅归且旦目叶甲申叮电号田由史只央兄叼叫另叨叹四生失禾丘付仗代仙们仪白仔他斥瓜乎" +
                                "丛令用甩印乐句匆册犯外处冬鸟务包饥主市立闪兰半汁汇头汉宁穴它讨写让礼训必议讯记永司尼民出辽奶奴加召皮" +
                                "边发孕圣对台矛纠母幼丝式刑动扛寺吉扣考托老执巩圾扩扫地扬场耳共芒亚芝朽朴机权过臣再协西压厌在有百存而" +
                                "页匠夸夺灰达列死成夹轨邪划迈毕至此贞师尘尖劣光当早吐吓虫曲团同吊吃因吸吗屿帆岁回岂刚则肉网年朱先丢舌" +
                                "竹迁乔伟传乒乓休伍伏优伐延件任伤价份华仰仿伙伪自血向似后行舟全会杀合兆企众爷伞创肌朵杂危旬旨负各名多";

            List<char> code = new List<char>();
            for (int i = 0; i < length; i++)
            {
                int index = Rand.Next(0, cyhz.Length);
                code.Add(cyhz[index]);
            }
            return new string(code.ToArray());
        }
    }

    public static class CommonHelper
    {
        /// <summary>
        /// 计算字符串的MD5值
        /// </summary>
        public static string StringMd5(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return BytesMd5(bytes);
        }

        /// <summary>
        /// 计算字节数组的MD5值
        /// </summary>
        public static string BytesMd5(byte[] buffer)
        {
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                byte[] hash = md5.ComputeHash(buffer);

                //以下计算是把二进制数转换成十六进制数
                char[] chArray = new char[hash.Length << 1];
                int index = 0;
                for (int i = 0; i < chArray.Length; i += 2)
                {
                    byte @byte = hash[index++];
                    //byte可以表示二进制0000 0000 到 1111 1111 之间的数
                    //比如二进制数 0101 1111，对应的十六进制数为 5f
                    chArray[i] = GetHexValue(@byte >> 4);       //获取高四位 0101
                    chArray[i + 1] = GetHexValue(@byte & 15);   //获取低四位 1111
                }

                return new string(chArray, 0, chArray.Length);
            }
        }

        /// <summary>
        /// 获取对应的16进制数（小写）
        /// </summary>
        private static char GetHexValue(int i)
        {
            if (i < 10)
            {
                return (char)(i + '0');
            }
            i -= 10;
            return (char)(i + 'a');    // 改为'A'，获取大写
        }
    }

    public static class SessionHelper
    {
        public static void StoreInSession<T>(HttpContextBase ctx, string key, T value)
        {
            if (ctx.Session != null)
            {
                ctx.Session[key] = value;
            }
        }

        public static T TakeFromSession<T>(HttpContextBase ctx, string key)
        {
            return (T)ctx.Session[key];
        }
    }

    public static class KeyHelper
    {
        /// <summary>
        /// 验证码
        /// </summary>
        public static string AccountLoginSecurityCode = "26223709-9CD7-4E84-AF17-0181B6CCC825";
        
        public static string AccountReturnUrl = "3F3D2434-0B76-455F-834A-4CAE860B0369";
    }
}