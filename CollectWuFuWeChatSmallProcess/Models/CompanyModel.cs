using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools.Models;
using Tools.Response.Json;

namespace CollectWuFuWeChatSmallProcess.Models
{
    public class CompanyModel
    {
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId CompanyID { get; set; }
        public string uniacid { get; set; }
        public ProcessMiniInfo ProcessMiniInfo { get; set; }

        public string CertFileName { get; set; }

        public string Phone { get; set; }
        public string About { get; set; }
        public string Address { get; set; }
        public List<ProjPic> ProjPics { get; set; }
        public QiNiuModel QiNiuModel { get; set; }

    }

    public class QiNiuModel
    {
        public QiNiuDAL.Exerciser exerciser = new QiNiuDAL.Exerciser();
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string Bucket { get; set; }
        public string DoMain { get; set; }
        public void UploadFile(string filePath)
        {
            exerciser.UploadFile(filePath, AccessKey, SecretKey, Bucket);
        }
        public async Task<string> GetFileUrl(string fileName)
        {
            return await exerciser.CreateDownloadUrl(DoMain, fileName);
        }
        public void DeleteFile(string fileName)
        {
            exerciser.DeleteFile(fileName, AccessKey, SecretKey, Bucket);
        }
    }
    public class ProjPic
    {
        public PicType Type { get; set; }
        public string Url { get; set; }
    }


    public enum PicType
    {
        福星 = 1, 禄星 = 2, 寿星 = 3, 喜神 = 4, 财神 = 5,
        福星_黑 = -1, 禄星_黑 = -2, 寿星_黑 = -3, 喜神_黑 = -4, 财神_黑 = -5,
        小程序分享 = 10, 分享成功 = 20, 头部 = 30, 二维码 = 40

    }


    //public class QiNiuModel
    //{
    //    public QiNiuDAL.Exerciser exerciser = new QiNiuDAL.Exerciser();
    //    public string AccessKey { get; set; }
    //    public string SecretKey { get; set; }
    //    public string Bucket { get; set; }
    //    public string DoMain { get; set; }
    //    public void UploadFile(string filePath)
    //    {
    //        exerciser.UploadFile(filePath, AccessKey, SecretKey, Bucket);
    //    }
    //    public async Task<string> GetFileUrl(string fileName)
    //    {
    //        return await exerciser.CreateDownloadUrl(DoMain, fileName);
    //    }
    //    public void DeleteFile(string fileName)
    //    {
    //        exerciser.DeleteFile(fileName, AccessKey, SecretKey, Bucket);
    //    }
    //}
    public class ProcessMiniInfo
    {
        public string Detail { get; set; }
        public string Name { get; set; }
        public FileModel<string[]> Logo { get; set; }
    }
}
