using System;
using System.IO;
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
}