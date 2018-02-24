using ConfigData;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using CollectWuFuWeChatSmallProcess.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CollectWuFuWeChatSmallProcess.AppData
{
    public class MerchantData : BaseData<CompanyModel>
    {
     


        internal List<AccountModel> GetAccountList(string uniacid)
        {
            var list = mongo.GetMongoCollection<AccountModel>().Find(x => x.uniacid.Equals(uniacid)).ToList();
            return list;
        }

    

        internal CompanyModel GetCompanyModel(string uniacid)
        {
            var companyModel = mongo.GetMongoCollection<CompanyModel>().Find(x => x.uniacid.Equals(uniacid)).FirstOrDefault();
            return companyModel;
        }

    

      }
}
