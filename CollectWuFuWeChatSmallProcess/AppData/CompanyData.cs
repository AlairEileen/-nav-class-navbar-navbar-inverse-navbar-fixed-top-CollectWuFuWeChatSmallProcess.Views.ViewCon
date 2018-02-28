using CollectWuFuWeChatSmallProcess.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectWuFuWeChatSmallProcess.AppData
{
    public class CompanyData : BaseData<CompanyModel>
    {
        internal CompanyModel GetCompanyInfo(string uniacid)
        {
            var company = collection.Find(x => x.uniacid.Equals(uniacid)).FirstOrDefault();
            return company;
        }
    }
}
