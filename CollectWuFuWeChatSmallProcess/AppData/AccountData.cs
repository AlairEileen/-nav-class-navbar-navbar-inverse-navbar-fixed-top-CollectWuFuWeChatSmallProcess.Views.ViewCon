using CollectWuFuWeChatSmallProcess.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WXSmallAppCommon.Models;

namespace CollectWuFuWeChatSmallProcess.AppData
{
    public class AccountData : BaseData<AccountModel>
    {
        /// <summary>
        /// 调取微信用户，更新或者保存本地用户
        /// </summary>
        /// <param name="uniacid">商户识别ID</param>
        /// <param name="wXAccount">微信用户</param>
        /// <returns></returns>
        internal AccountModel SaveOrdUpdateAccount(string uniacid, WXAccountInfo wXAccount)
        {
            Console.WriteLine("在SaveOrdUpdateAccount");
            AccountModel accountCard = null;
            if (wXAccount.OpenId != null)
            {
                var filter = Builders<AccountModel>.Filter.Eq(x => x.OpenID, wXAccount.OpenId) &
                   Builders<AccountModel>.Filter.Eq(x => x.uniacid, uniacid);
                var update = Builders<AccountModel>.Update.Set(x => x.LastChangeTime, DateTime.Now);
                accountCard = collection.FindOneAndUpdate<AccountModel>(filter, update);
                Console.WriteLine($"在SaveOrdUpdateAccount{accountCard == null}");

                if (accountCard == null)
                {
                    //string avatarUrl = DownloadAvatar(wXAccount.AvatarUrl, wXAccount.OpenId);
                    var avatarUrl = wXAccount.AvatarUrl;
                    accountCard = new AccountModel() { uniacid = uniacid, OpenID = wXAccount.OpenId, AccountName = wXAccount.NickName, Gender = wXAccount.GetGender, AccountAvatar = avatarUrl, CreateTime = DateTime.Now, LastChangeTime = DateTime.Now };
                    collection.InsertOne(accountCard);
                }
            }
            return accountCard;
        }

        internal AccountModel GetAccountInfo(string uniacid, ObjectId accountID)
        {
            return GetModelByIDAndUniacID(accountID, uniacid);
        }

        internal void SaveAccountInfo(AccountModel account)
        {
            collection.UpdateOne(
                x=>x.AccountID.Equals(account.AccountID)
                &&x.uniacid.Equals(account.uniacid),
                Builders<AccountModel>.Update
                .Set(x=>x.Info,account.Info)
                .Set(x=>x.Gender,account.Gender)
                .Set(x=>x.AccountName,account.AccountName)
                .Set(x=>x.AccountPhoneNumber,account.AccountPhoneNumber));
        }
    }
}
