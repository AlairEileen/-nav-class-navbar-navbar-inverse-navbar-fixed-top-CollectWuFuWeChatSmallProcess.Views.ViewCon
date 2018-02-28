using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CollectWuFuWeChatSmallProcess.AppData;
using CollectWuFuWeChatSmallProcess.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using Tools.Models;
using Tools.Response;
using Tools.Response.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CollectWuFuWeChatSmallProcess.Controllers
{
    public class AccountController : BaseController<AccountData, AccountModel>
    {
        /// <summary>
        /// 请求登录
        /// </summary>
        /// <param name="uniacid">商户识别ID</param>
        /// <param name="code"></param>
        /// <param name="iv"></param>
        /// <param name="encryptedData"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAccountID(string uniacid, string code, string iv, string encryptedData)
        {
            try
            {
                var responseModel = new JsonResponse1<AccountModel>();

                //WXSmallAppCommon.Models.WXAccountInfo wXAccount = WXSmallAppCommon.WXInteractions.WXLoginAction.ProcessRequest(code, iv, encryptedData);
                ///微擎方式
                var wXAccount = We7Tools.We7Tools.GetWeChatUserInfo(uniacid, code, iv, encryptedData);
                var accountCard = thisData.SaveOrdUpdateAccount(uniacid, wXAccount);
                var stautsCode = ResponseStatus.请求失败;
                if (accountCard != null)
                {
                    responseModel.JsonData = accountCard;
                    stautsCode = ResponseStatus.请求成功;
                }
                responseModel.StatusCode = stautsCode;
                var param = new string[] { "JsonData", "AccountID" };
                return this.JsonSuccessWithLimit(responseModel, param);
            }
            catch (Exception e)
            {
                e.Save();
                return this.JsonErrorStatus();
            }
        }

        /// <summary>
        /// 查询个人信息
        /// </summary>
        /// <param name="uniacid">商户识别ID</param>
        /// <param name="accountID">用户ID</param>
        /// <returns></returns>
        public IActionResult GetAccountInfo(string uniacid, string accountID)
        {
            try
            {
                var account = thisData.GetAccountInfo(uniacid, new ObjectId(accountID));
                var response = new JsonResponse1<AccountModel> { JsonData = account };
                var param = new string[] {
                    "JsonData",
                    "AccountName",
                    "AccountPhoneNumber",
                    nameof(Gender),
                    "AccountAvatar" ,
                    "Info",
                    "Address",
                    "Brief" };
                return this.JsonSuccessWithLimit(response, param);
            }
            catch (Exception e)
            {
                e.Save();
                return this.JsonErrorStatus();
            }
        }

        /// <summary>
        /// 获取我的收集列表
        /// </summary>
        /// <param name="uniacid">商户识别ID</param>
        /// <param name="accountID">用户ID</param>
        /// <returns></returns>
        public IActionResult GetAccountCollect(string uniacid, string accountID)
        {
            try
            {
                var account = thisData.GetAccountInfo(uniacid, new ObjectId(accountID));
                var response = new JsonResponse1<AccountModel> { JsonData = account };
                var param = new string[] {
                    "JsonData",
                    "CanOpenJackTimes",
                    "JackType",
                    "HasCount",
                    "CanShareTimes",
                    "Collect" };
                return this.JsonSuccessWithLimit(response, param);
            }
            catch (Exception e)
            {
                e.Save();
                return this.JsonErrorStatus();
            }
        }
        /// <summary>
        /// 分享成功接口
        /// </summary>
        ///<param name="uniacid">商户识别ID</param>
        ///<param name="accountID">用户ID</param>
        /// <returns></returns>
        public IActionResult ShareSuccess(string uniacid, string accountID)
        {
            try
            {
                thisData.ShareSuccess(uniacid, new ObjectId(accountID));
                return this.JsonSuccessStatus();
            }
            catch (Exception e)
            {
                e.Save();
                return this.JsonErrorStatus();
            }
        }
        /// <summary>
        /// 开奖
        /// </summary>
        /// <param name="uniacid"></param>
        /// <param name="accountID"></param>
        /// <returns></returns>
        public IActionResult OpenJack(string uniacid, string accountID)
        {
            try
            {
                var jack = thisData.OpenJack(uniacid, new ObjectId(accountID));
                return jack.ToJsonSuccess(this);
            }
            catch (Exception e)
            {
                e.Save();
                return this.JsonErrorStatus();
            }
        }


        /// <summary>
        /// 保存个人信息
        /// json:
        /// {
        /// "uniacid":"",
        /// "AccountID":"",
        /// "AccountName":"",
        /// "AccountPhoneNumber":"",
        /// "Gender":"",
        /// "Info":{
        /// "Address":"",
        /// "Brief":""
        /// }
        /// }
        /// </summary>
        /// <returns></returns>
        public IActionResult SaveAccountInfo()
        {
            try
            {
                using (var streamReader = new StreamReader(Request.Body))
                {
                    var json = streamReader.ReadToEnd();
                    var account = JsonConvert.DeserializeObject<AccountModel>(json);
                    thisData.SaveAccountInfo(account);
                }
                return this.JsonSuccessStatus();
            }
            catch (Exception e)
            {
                e.Save();
                return this.JsonErrorStatus();
            }
        }
    }
}
