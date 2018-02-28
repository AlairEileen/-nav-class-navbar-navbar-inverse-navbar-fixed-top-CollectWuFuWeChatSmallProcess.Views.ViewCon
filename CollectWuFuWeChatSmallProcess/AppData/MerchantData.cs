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

        internal void SaveCompanyInfo(CompanyModel company)
        {
            var companyOld = GetCompanyModel(company.uniacid);
            if (companyOld == null)
            {
                collection.InsertOne(company);
            }
            else
            {
                collection.UpdateOne(x => x.uniacid.Equals(company.uniacid), Builders<CompanyModel>
                    .Update.Set(x => x.Phone, company.Phone)
                    .Set(x => x.About, company.About)
                    .Set(x => x.Address, company.Address));
            }
        }

        internal void SetQiNiu(string uniacid, QiNiuModel qiNiuModel)
        {
            var companyCollection = mongo.GetMongoCollection<CompanyModel>();
            var company = companyCollection.Find(x => x.uniacid.Equals(uniacid)).FirstOrDefault();
            if (company == null)
            {
                companyCollection.InsertOne(new CompanyModel() { QiNiuModel = qiNiuModel, uniacid = uniacid });
            }
            companyCollection.UpdateOne(x => x.uniacid.Equals(uniacid), Builders<CompanyModel>.Update.Set(x => x.QiNiuModel, qiNiuModel));
        }

        internal void SavePic(PicType picType, string uniacid, string filename)
        {
            var company = GetCompanyModel(uniacid);
            if (company.QiNiuModel == null)
            {
                throw new Exception("error");
            }
            company.QiNiuModel.UploadFile(filename);
            if (company.ProjPics==null)
            {
                company.ProjPics = new List<ProjPic>();
            }
            if (company.ProjPics.Exists(x => x.Type == picType))
            {
                company.QiNiuModel.DeleteFile(company.ProjPics.Find(x => x.Type == picType).Url);
                company.ProjPics.Find(x => x.Type == picType).Url = filename;
            }
            else
            {
                company.ProjPics.Add(new ProjPic
                {
                    Type = picType,
                    Url = filename
                });
            }
            collection.UpdateOne(x => x.uniacid.Equals(uniacid),
                Builders<CompanyModel>
                .Update
                .Set(x => x.ProjPics, company.ProjPics));
        }
    }
}
