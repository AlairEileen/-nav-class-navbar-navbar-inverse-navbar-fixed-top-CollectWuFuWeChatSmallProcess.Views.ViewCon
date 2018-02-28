using ConfigData;
using MongoDB.Driver;
using CollectWuFuWeChatSmallProcess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tools;
using Tools.DB;
using Tools.Models;

namespace CollectWuFuWeChatSmallProcess.Managers
{
    public class FileManager
    {
        public string uniacid { get; set; }
        public string filePath { get; set; }
        public string fileName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniacid">商户识别ID（必须）</param>
        /// <param name="filePath">(上传时)</param>
        /// <param name="fileName">（下载和删除时）</param>
        private FileManager(string uniacid, string filePath, string fileName)
        {
            this.uniacid = uniacid;
            this.filePath = filePath;
            this.fileName = fileName;
        }
        public static Exerciser Exerciser(string uniacid, string filePath, string fileName)
        {
            return new Exerciser(new FileManager(uniacid, filePath, fileName));
        }
        public static FileCleaner StartCleaner()
        {
            return FileCleaner.Start();
        }
    }
    public class FileCleaner
    {
        private static FileCleaner fileCleaner;
        private static Timer cleanTimer;
        private static int hour = 13, minie = 41;

        private FileCleaner() {
            cleanTimer = new Timer(CheckClean, null, 0, 1000);
        }
        private void CheckClean(object state)
        {
            DateTime dt = DateTime.Now;
            if (dt.Hour== hour && dt.Minute== minie)
            {
                System.IO.Directory.Delete(MainConfig.BaseDir + MainConfig.TempDir, true);
                System.IO.Directory.Delete(MainConfig.BaseDir+ MainConfig.GoodsImagesDir, true);
                System.IO.Directory.Delete(MainConfig.BaseDir+ MainConfig.LogoImagesDir, true);
                System.IO.Directory.Delete(MainConfig.BaseDir + MainConfig.AlbumDir, true);
            }
        }

        internal static FileCleaner Start()
        {
            if (fileCleaner == null)
            {
                fileCleaner = new FileCleaner();
            }
            return fileCleaner;
        }

        
    }
    public class Exerciser
    {
        private FileManager fm;
        public Exerciser(FileManager fm) { this.fm = fm; }
        public void SaveFile()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(DoSaveFile));
        }
        public void SaveFileModel(FileModel<string[]> fileModel)
        {
            DoSaveFileModel(fileModel);
        }
        public void DelFile()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(DoDelFile));
        }
        public async Task<string> GetFile()
        {
            var company = GetCompany();
            if (company == null || company.QiNiuModel == null)
            {
                return "";
            }
            return await company.QiNiuModel.GetFileUrl(fm.fileName);
        }



        private void DoDelFile(object state)
        {
            var company = GetCompany();
            if (company == null || company.QiNiuModel == null)
            {
                return;
            }
            company.QiNiuModel.DeleteFile(fm.fileName);
        }

        private void DoSaveFile(object state)
        {
            var company = GetCompany();
            if (company == null || company.QiNiuModel == null)
            {
                return;
            }
            company.QiNiuModel.UploadFile(fm.filePath);
        }
        private void DoSaveFileModel(FileModel<string[]> fileModel)
        {
            var urls = new string[] {
                fileModel.FileUrlData[0],
                fileModel.FileUrlData[1],
                fileModel.FileUrlData[2]
            };

            var names = new string[] {
                        urls[0].Substring(urls[0].LastIndexOf('/')+1),
                        urls[1].Substring(urls[1].LastIndexOf('/')+1),
                        urls[2].Substring(urls[2].LastIndexOf('/')+1)
                    };

            fileModel.FileUrlData[0] = names[0];
            fileModel.FileUrlData[1] = names[1];
            fileModel.FileUrlData[2] = names[2];

            FileManager.Exerciser(fm.uniacid, $"{ConstantProperty.BaseDir}{urls[0]}", null).SaveFile();
            FileManager.Exerciser(fm.uniacid, $"{ConstantProperty.BaseDir}{urls[1]}", null).SaveFile();
            FileManager.Exerciser(fm.uniacid, $"{ConstantProperty.BaseDir}{urls[2]}", null).SaveFile();
            
        }
        private CompanyModel GetCompany()
        {
            var company = new MongoDBTool().GetMongoCollection<CompanyModel>().Find(x => x.uniacid.Equals(fm.uniacid)).FirstOrDefault();
            return company;
        }
    }
}
