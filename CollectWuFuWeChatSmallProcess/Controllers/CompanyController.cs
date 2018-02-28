using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CollectWuFuWeChatSmallProcess.AppData;
using CollectWuFuWeChatSmallProcess.Managers;
using CollectWuFuWeChatSmallProcess.Models;
using Microsoft.AspNetCore.Mvc;
using Tools.Models;
using Tools.Response;
using Tools.Response.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CollectWuFuWeChatSmallProcess.Controllers
{
    public class CompanyController : BaseController<CompanyData, CompanyModel>
    {
        /// <summary>
        /// 获取公司信息
        /// </summary>
        /// <param name="uniacid">商户识别ID</param>
        /// <returns></returns>
        public IActionResult GetCompanyInfo(string uniacid)
        {
            try
            {
                var company = thisData.GetCompanyInfo(uniacid);
                return company.ToJsonSuccessWithLimit(this, new string[] {"JsonData", "Phone", "Address", "About" });

            }
            catch (Exception e)
            {
                e.Save();
                return this.JsonErrorStatus();
            }
        }
        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="uniacid">商户识别ID</param>
        /// <param name="picType">图片类型编码</param>
        /// <returns></returns>
        public async Task<IActionResult> GetPics(string uniacid,PicType picType)
        {
            try
            {
                var company = thisData.GetCompanyInfo(uniacid);
                var url = "";
                foreach (var item in company.ProjPics)
                {
                    if (item.Type == picType)
                    {
                        url = item.Url;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(url))
                {
                    throw new Exception();
                }
                return await GetQiniuPic(uniacid,url);
            }
            catch (Exception)
            {
                return this.JsonErrorStatus();
            }
        }

        private async Task<IActionResult> GetQiniuPic(string uniacid,string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }
            string fileUrl = await FileManager.Exerciser(uniacid, null, url).GetFile();
            var stream = System.IO.File.OpenRead(fileUrl);
            return File(stream, "application/vnd.android.package-archive", Path.GetFileName(fileUrl));
        }
      
    }

    
}
