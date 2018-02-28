using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using CollectWuFuWeChatSmallProcess.AppData;
using CollectWuFuWeChatSmallProcess.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tools.Models;
using Tools.Response;
using Tools.Response.Json;
using We7Tools;
using We7Tools.Extend;
using System.Net.Http.Headers;
using ConfigData;

namespace CollectWuFuWeChatSmallProcess.Controllers
{
    public class MerchantController : BaseController<MerchantData, CompanyModel>
    {
        private IHostingEnvironment hostingEnvironment;

        public MerchantController(IHostingEnvironment environment) : base(true)
        {
            hostingEnvironment = environment;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            //var thList = GetOrderViewList();
            return View();
        }

        //private object GetOrderViewList()
        //{
        //    return thisData.GetOrderViewList(HttpContext.Session.GetUniacID());
        //}

        public IActionResult Settings()
        {
            var companyModel = thisData.GetCompanyModel(HttpContext.Session.GetUniacID());
            if (companyModel == null)
            {
                companyModel = new CompanyModel { ProcessMiniInfo = new ProcessMiniInfo() };
            }
            return View(new ManageViewModel
            {
                ProcessMiniInfo = companyModel.ProcessMiniInfo,
                Company = companyModel
            });
        }
        public IActionResult AccountInfo()
        {
            var fblist = thisData.GetAccountList(HttpContext.Session.GetUniacID());
            return View(fblist);
        }
        public IActionResult ProcessMiniZipDownload()
        {
            if (!HttpContext.Session.HasWe7Data())
            {
                return RedirectToAction(nameof(Index), "Error", new { errorType = ErrorType.ErrorNoUserOrTimeOut });
            }
            try
            {
                string fileUrl;
                ProcessMiniTool.GetProcessMiniZipPath(HttpContext.Session, out fileUrl);
                var fileByteArray = System.IO.File.ReadAllBytes(fileUrl);
                var fileName = Path.GetFileName(fileUrl);
                System.IO.File.Delete(fileUrl);
                return File(fileByteArray, "application/vnd.android.package-archive", fileName);
            }
            catch (Exception e)
            {
                e.Save();
                return RedirectToAction("Index", "Error");
                //throw;
            }
        }

        public IActionResult SetCompanyInfo()
        {
            try
            {
                using (var streamReader = new StreamReader(Request.Body))
                {
                    var json = streamReader.ReadToEnd();
                    var company = JsonConvert.DeserializeObject<CompanyModel>(json);
                    company.uniacid = HttpContext.Session.GetUniacID();
                    thisData.SaveCompanyInfo(company);
                }
                return this.JsonSuccessStatus();
            }
            catch (Exception e)
            {
                e.Save();
                return this.JsonErrorStatus();
            }
        }
        public IActionResult SetQiNiu()
        {
            try
            {
                string json = new StreamReader(Request.Body).ReadToEnd();
                QiNiuModel qiNiuModel = JsonConvert.DeserializeObject<QiNiuModel>(json);
                thisData.SetQiNiu(HttpContext.Session.GetUniacID(), qiNiuModel);
                return this.JsonSuccessStatus();
            }
            catch (Exception)
            {
                return this.JsonErrorStatus();

            }
        }

        public IActionResult SavePic(PicType picType)
        {
            try
            {
                long size = 0;
                var files = Request.Form.Files;
                var file = files[0];
                var filename = ContentDispositionHeaderValue
                                      .Parse(file.ContentDisposition)
                                      .FileName
                                      .Trim('"');
                var uniacid = HttpContext.Session.GetUniacID();
                string saveDir = $@"{MainConfig.BaseDir}{MainConfig.TempDir}{uniacid}/";
                if (!Directory.Exists(saveDir))
                {
                    Directory.CreateDirectory(saveDir);
                }
                string exString = filename.Substring(filename.LastIndexOf("."));
                string saveName = Guid.NewGuid().ToString("N");
                filename = $@"{saveDir}{saveName}{exString}";

                size += file.Length;
                using (FileStream fs = System.IO.File.Create(filename))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
                thisData.SavePic(picType, uniacid, filename);
                return this.JsonSuccessStatus();
            }
            catch (Exception)
            {
                return this.JsonErrorStatus();
            }
        }
    }
}
