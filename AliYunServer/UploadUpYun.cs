using System;
using System.IO;
using log4net;
using Utility;

namespace AliYunServer
{
    public class UploadUpYun
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UploadHandler));

        public UploadResult Result { get; private set; }

        public UploadUpYun(UploadResult result)
        {
            this.Result = result;
        }

        private string UpYunPath { get; set; }

        /// <param name="upYunPath">格式：/upload/2015   最深10级目录</param>
        public UploadUpYun(UploadResult result, string upYunPath)
        {
            this.Result = result;
            this.UpYunPath = upYunPath;
        }

        public object Upload(byte[] uploadFileBytes, string uploadFileName)
        {
            object model;
            try
            {
                //string fileMd5 = CommonHelper.CalcMd5(uploadFileBytes);                 // 上传图片的md5值
                string fileMd5 = UpYun.BytesMd5(uploadFileBytes);
                //string bucket = ConfigurationManager.AppSettings["UpYun_bucket"];       // 空间名称
                //string username = ConfigurationManager.AppSettings["UpYun_username"];   // 操作员
                //string password = ConfigurationManager.AppSettings["UpYun_password"];   // 密码
                string bucket = "panpan";
                string username = "panpan";
                string password = "panpan88";


                UpYun upyun = new UpYun(bucket, username, password);
                // 设置待上传文件的 Content-MD5 值（如又拍云服务端收到的文件MD5值与用户设置的不一致，将回报 406 Not Acceptable 错误）
                //upyun.setContentMD5(fileMd5);
                upyun.ContentMd5 = fileMd5;

                // 上传文件时可使用 upyun.writeFile("/a/test.jpg",postArray, true); 
                // 进行父级目录的自动创建（最深10级目录）
                string upYunFilePath = string.Format("{0}/{1}{2}", this.UpYunPath, fileMd5, Path.GetExtension(uploadFileName));
                bool success = upyun.WriteFile(upYunFilePath, uploadFileBytes, true);
                if (success)
                {
                    // 返回图片在UpYun中的http访问地址
                    Result.Url = string.Format("{0}{1}", "http://static.qpanpan.cn", upYunFilePath);
                    Result.State = UploadState.Success;
                }
                else
                {
                    Result.State = UploadState.FileAccessError;
                    Result.ErrorMessage = "上传图片到又拍云失败";
                    Logger.Error(Result.ErrorMessage);
                }
            }
            catch (Exception err)
            {
                Result.State = UploadState.FileAccessError;
                Result.ErrorMessage = err.Message;
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
}