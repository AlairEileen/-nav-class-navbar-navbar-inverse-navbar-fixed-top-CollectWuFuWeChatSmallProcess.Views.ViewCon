using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ConfigData;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tools.DB;
using Tools.Response;
using Tools.Response.Json;
using We7Tools.Extend;
using We7Tools.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CollectWuFuWeChatSmallProcess.Controllers
{
    public class ExternalController : Controller
    {

        private IHostingEnvironment hostingEnvironment;
        public ExternalController(IHostingEnvironment environment)
        {
            this.hostingEnvironment = environment;
        }
        // GET: /<controller>/
        public IActionResult Index(string key)
        {
            ViewData["key"] = key;
            var db = new MongoDBTool().GetMongoCollection<We7Temp>();
            We7Temp data = null;

            if (MainConfig.IsDev)
#pragma warning disable CS0162 // Unreachable code detected
                data = db.Find(x => x.We7TempID.Equals(new ObjectId(key))).FirstOrDefault();
#pragma warning restore CS0162 // Unreachable code detected
            else
                data = db.FindOneAndDelete(x => x.We7TempID.Equals(new ObjectId(key)));

            if (data == null)
            {
                return RedirectToAction("Index", "Error");
            }
            ViewData["we7Data"] = data.Data;
            var jObject = (JObject)JsonConvert.DeserializeObject(data.Data);
            var uniacid = (string)jObject["uniacid"];
            if (!string.IsNullOrEmpty(uniacid))
            {
                HttpContext.Session.PushWe7Data(data.Data);
            }
            //hasIdentity = true;
            return RedirectToAction("Index", "Merchant");
        }
        public IActionResult ReceiveWe7Data()
        {
            try
            {
                using (var streamReader = new StreamReader(Request.Body))
                {
                    var json = streamReader.ReadToEnd();
                    var db = new MongoDBTool().GetMongoCollection<We7Temp>();
                    var we7Temp = new We7Temp() { Data = json };
                    db.InsertOne(we7Temp);
                    return new JsonResponse1<string>() { JsonData = we7Temp.We7TempID.ToString() }.ToJsonResult(this);
                }
            }
            catch (Exception)
            {
                return this.JsonErrorStatus();
            }
        }
    }
}
