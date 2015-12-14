using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Utility
{
    public class UpYun
    {
        #region 类初始化属性
        private readonly string _bucketName;
        private readonly string _userName;
        private readonly string _password;

        /// <summary>
        /// 初始化 UpYun 存储接口
        /// </summary>
        /// <param name="bucketName">空间名称</param>
        /// <param name="userName">操作员名称</param>
        /// <param name="password">密码</param>
        public UpYun(string bucketName, string userName, string password)
        {
            _bucketName = bucketName;
            _userName = userName;
            _password = password;
        }
        #endregion

        private string _apiDomain;
        /// <summary>
        /// 可切换 API 接口的域名
        /// 默认 v0.api.upyun.com 自动识别, 
        /// v1.api.upyun.com 电信, v2.api.upyun.com 联通, v3.api.upyun.com 移动
        /// </summary>
        public string ApiDomain
        {
            get
            {
                return _apiDomain ?? "v0.api.upyun.com";
            }
            set
            {
                _apiDomain = value;
            }
        }

        /// <summary>
        /// 是否启用 又拍签名认证
        /// 默认 false 不启用(直接使用basic auth)，true 启用又拍签名认证
        /// </summary>
        public bool UpAuth { get; set; }

        private bool AutoMkDir { get; set; }

        /// 设置待上传文件的 访问密钥
        /// （注意：仅支持图片空！，设置密钥后，无法根据原文件URL直接访问，需带 URL 后面加上 （缩略图间隔标志符+密钥） 进行访问）
        /// 例如缩略图间隔标志符为 ! ，密钥为 bac，上传文件路径为 /folder/test.jpg ，
        /// 那么该图片的对外访问地址为： http://空间域名/folder/test.jpg!bac
        public string FileSecretKey { get; set; }

        /// <summary>
        /// 设置待上传文件的 Content-Md5 值
        /// （若又拍云服务端收到的文件MD5值与用户设置的不一致，将回报 406 Not Acceptable 错误）</summary>
        public string ContentMd5 { get; set; }

        public string Version()
        {
            return "1.0.1";
        }

        private const string Dl = "/";
        private Hashtable _hashTable;



        private void UpYunAuth(Hashtable headers, string method, string uri, HttpWebRequest request)
        {
            DateTime now = DateTime.UtcNow;
            request.Date = now;
            string date = now.ToString("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'", CultureInfo.CreateSpecificCulture("en-US"));
            if (request.ContentLength < 0)
            {
                request.ContentLength = 0;
            }

            //  auth = md5(method + '&' + uri + '&' + date + '&' + request.ContentLength + '&' + md5(this.password));
            string auth = string.Format("{0}&{1}&{2}&{3}&{4}", method, uri, date, request.ContentLength, StringMd5(_password));
            headers.Add("Authorization", "UpYun " + _userName + ':' + auth);
        }

        private bool Delete(string path, Hashtable headers)
        {
            using (HttpWebResponse resp =
                NewWorker("DELETE", Dl + _bucketName + path, null, headers))
            {
                return resp.StatusCode == HttpStatusCode.OK;
            }
        }

        private HttpWebResponse NewWorker(string method, string url, byte[] postData, Hashtable headers)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://" + ApiDomain + url);
            request.Method = method;

            if (AutoMkDir)
            {
                headers.Add("mkdir", "true");
                AutoMkDir = false;
            }

            if (postData != null)
            {
                request.ContentLength = postData.Length;
                request.KeepAlive = true;
                if (ContentMd5 != null)
                {
                    request.Headers.Add("Content-Md5", ContentMd5);
                    ContentMd5 = null;
                }
                if (FileSecretKey != null)
                {
                    request.Headers.Add("Content-Secret", FileSecretKey);
                    FileSecretKey = null;
                }
            }

            if (UpAuth)
            {
                UpYunAuth(headers, method, url, request);
            }
            else
            {
                request.Headers.Add("Authorization", "Basic " +
                    Convert.ToBase64String(new ASCIIEncoding().GetBytes(_userName + ":" + _password)));
            }

            foreach (DictionaryEntry de in headers)
            {
                request.Headers.Add(de.Key.ToString(), de.Value.ToString());
            }

            if (postData != null)
            {
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(postData, 0, postData.Length);
                    dataStream.Close();
                }
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            _hashTable = new Hashtable();
            foreach (string name in response.Headers)
            {
                if (name.Length > 7 && name.Substring(0, 7) == "x-upyun")
                {
                    _hashTable.Add(name, response.Headers[name]);
                }
            }
            return response;
        }


        /// <summary>
        /// 获取总体空间的占用信息
        /// </summary>
        /// <returns>返回空间占用量，失败返回 null</returns>
        public int GetFolderUsage(string url)
        {
            using (HttpWebResponse resp = NewWorker("GET", Dl + _bucketName + url + "?usage", null, new Hashtable()))
            {
                int size;
                try
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    using (StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                    {
                        string strhtml = sr.ReadToEnd();
                        size = int.Parse(strhtml);
                    }
                }
                catch
                {
                    size = 0;
                }
                return size;
            }
        }

        /// <summary>
        /// 获取某个子目录的占用信息
        /// </summary>
        /// <returns>返回 空间占用量，失败返回 null</returns>
        public int GetBucketUsage()
        {
            return GetFolderUsage(string.Empty);
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="path">目录路径</param>
        /// <param name="autoMkDir"></param>
        public bool MkDir(string path, bool autoMkDir)
        {
            AutoMkDir = autoMkDir;
            Hashtable headers = new Hashtable
            {
                {"folder", "create"},
            };
            using (HttpWebResponse resp = NewWorker("POST", Dl + _bucketName + path, new byte[0], headers))
            {
                return resp.StatusCode == HttpStatusCode.OK;
            }
        }


        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="path">目录路径</param>
        public bool RmDir(string path)
        {
            return Delete(path, new Hashtable());
        }

        /// <summary>
        /// 读取目录列表
        /// </summary>
        /// <param name="url">目录路径</param>
        /// <returns>array 数组 或 null</returns>

        public List<FolderItem> ReadDir(string url)
        {
            using (HttpWebResponse resp = NewWorker("GET", Dl + _bucketName + url, null, new Hashtable()))
            // ReSharper disable once AssignNullToNotNullAttribute
            using (StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
            {
                string[] temp = sr.ReadToEnd().Split('\t', '\n');
                int i = 0;
                List<FolderItem> fis = new List<FolderItem>();
                while (i < temp.Length)
                {
                    FolderItem fi = new FolderItem(temp[i], temp[i + 1], int.Parse(temp[i + 2]), int.Parse(temp[i + 3]));
                    fis.Add(fi);
                    i += 4;
                }
                return fis;
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="path">文件路径（包含文件名）</param>
        /// <param name="postData">文件内容 或 文件IO数据流</param>
        /// <param name="autoMkDir"></param>
        public bool WriteFile(string path, byte[] postData, bool autoMkDir)
        {
            AutoMkDir = autoMkDir;
            Hashtable headers = new Hashtable();
            using (HttpWebResponse resp =
                NewWorker("POST", Dl + _bucketName + path, postData, headers))
            {
                return (resp.StatusCode == HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path">文件路径（包含文件名）</param>
        public bool DeleteFile(string path)
        {
            return Delete(path, new Hashtable());
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="path">文件路径（包含文件名）</param>
        /// <returns>返回文件内容 或 null</returns>
        public byte[] ReadFile(string path)
        {
            Hashtable headers = new Hashtable();

            using (HttpWebResponse resp = NewWorker("GET", Dl + _bucketName + path, null, headers))
            // ReSharper disable once AssignNullToNotNullAttribute
            using (StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
            using (BinaryReader br = new BinaryReader(sr.BaseStream))
            {
                // 又拍云存储最大文件限制 100Mb，对于普通用户可以改写该值，以减少内存消耗
                return br.ReadBytes(1024 * 1024 * 100);
            }
        }


        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="file">文件路径（包含文件名）</param>
        /// <returns>array('type'=> file | folder, 'size'=> file size, 'date'=> unix time) 或 null</returns>
        public Hashtable GetFileInfo(string file)
        {
            Hashtable headers = new Hashtable();
            using (NewWorker("HEAD", Dl + _bucketName + file, null, headers))
            {
            }

            Hashtable ht;
            try
            {
                ht = new Hashtable
                {
                    {"type", _hashTable["x-upyun-file-type"]},
                    {"size", _hashTable["x-upyun-file-size"]},
                    {"date", _hashTable["x-upyun-file-date"]}
                };
            }
            catch
            {
                ht = new Hashtable();
            }
            return ht;
        }

        /// <summary>
        /// 获取上传后的图片信息（仅图片空间有返回数据）
        /// </summary>
        public object GetWritedFileInfo(string key)
        {
            //if (Equals(_hashTable, new Hashtable()))
            //{
            //    return "";
            //}
            //return _hashTable[key];
            return _hashTable[key] ?? string.Empty;
        }

        /// <summary>
        /// 计算文件的MD5码
        /// </summary>
        public static string FileMd5(string pathName)
        {
            using (Stream stream = new FileStream(pathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                // 设置当前流的位置为流的开始   
                return BytesMd5(bytes);
            }
        }

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

                char[] chArray = new char[hash.Length << 1];
                int index = 0;
                for (int i = 0; i < chArray.Length; i += 2)
                {
                    byte @byte = hash[index++];
                    chArray[i] = GetHexValue(@byte >> 4);
                    chArray[i + 1] = GetHexValue(@byte & 15);
                }

                return new string(chArray, 0, chArray.Length);
            }
        }

        private static char GetHexValue(int i)
        {
            if (i < 10)
            {
                return (char)(i + '0');
            }
            i -= 10;
            return (char)(i + 'a');
        }
    }

    /// <summary>
    /// 目录条目类
    /// </summary>
    public class FolderItem
    {
        public string FileName;
        public string FileType;
        public int Size;
        public int Number;

        public FolderItem(string fileName, string fileType, int size, int number)
        {
            FileName = fileName;
            FileType = fileType;
            Size = size;
            Number = number;
        }
    }

    public class UploadConfig
    {
        /// <summary>
        /// 文件命名规则
        /// </summary>
        public string PathFormat { get; set; }

        /// <summary>
        /// 上传表单域名称
        /// </summary>
        public string UploadFieldName { get; set; }

        /// <summary>
        /// 上传大小限制
        /// </summary>
        public int SizeLimit { get; set; }

        /// <summary>
        /// 上传允许的文件格式
        /// </summary>
        public string[] AllowExtensions { get; set; }

        /// <summary>
        /// 文件是否以 Base64 的形式上传
        /// </summary>
        public bool Base64 { get; set; }

        /// <summary>
        /// Base64 字符串所表示的文件名
        /// </summary>
        public string Base64FileName { get; set; }
    }

    public class UploadResult
    {
        public UploadState State { get; set; }
        public string Url { get; set; }
        public string OriginFileName { get; set; }
        public string ErrorMessage { get; set; }
    }

    public enum UploadState
    {
        NetworkError = -4,
        FileAccessError = -3,
        TypeNotAllow = -2,
        SizeLimitExceed = -1,

        Success = 0,
        Unknown = 1,
    }
}