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
                x => x.AccountID.Equals(account.AccountID)
                && x.uniacid.Equals(account.uniacid),
                Builders<AccountModel>.Update
                .Set(x => x.Info, account.Info)
                .Set(x => x.Gender, account.Gender)
                .Set(x => x.AccountName, account.AccountName)
                .Set(x => x.AccountPhoneNumber, account.AccountPhoneNumber));
        }

        internal void ShareSuccess(string uniacid, ObjectId accountID)
        {
            var account = GetModelByIDAndUniacID(accountID, uniacid);
            if (account != null && account.CanShareTimes > 0)
            {
                account.CanShareTimes--;
                account.CanOpenJackTimes++;
                collection.UpdateOne(
                    x => x.AccountID.Equals(account.AccountID) && x.uniacid.Equals(account.uniacid),
                    Builders<AccountModel>.Update.Set(x => x.CanOpenJackTimes, account.CanOpenJackTimes)
                    .Set(x => x.CanShareTimes, account.CanShareTimes));
            }
            else
            {
                throw new Exception("error");
            }
        }

        Dictionary<JackType, int> collect = new Dictionary<JackType, int>{
        { JackType.喜神,10},
        { JackType.寿星,20},
        { JackType.禄星,40},
        { JackType.福星,80},
        { JackType.财神,160}
        };


        public JackType RandomJack()
        {
            var random = new Random();
            JackType jack = JackType.喜神;
            while (true)
            {
                var n1 = random.Next(1, collect[JackType.喜神]);
                var n2 = random.Next(1, collect[JackType.寿星]);
                var n3 = random.Next(1, collect[JackType.禄星]);
                var n4 = random.Next(1, collect[JackType.福星]);
                var n5 = random.Next(1, collect[JackType.财神]);

                if (n1 == (int)JackType.喜神)
                {
                    jack = JackType.喜神;
                    break;
                }
                if (n2 == (int)JackType.寿星)
                {
                    jack = JackType.寿星;
                    break;
                }
                if (n3 == (int)JackType.禄星)
                {
                    jack = JackType.禄星;
                    break;
                }
                if (n4 == (int)JackType.福星)
                {
                    jack = JackType.福星;
                    break;
                }
                if (n5 == (int)JackType.财神)
                {
                    jack = JackType.财神;
                    break;
                }
            }
            return jack;
        }

        internal JackType OpenJack(string uniacid, ObjectId objectId)
        {

            var account = GetAccountInfo(uniacid, objectId);
            if (account.CanOpenJackTimes<=0)
            {
                throw new Exception("抽奖失败");
            }
            var jack = RandomJack();
            foreach (var item in account.Collect)
            {
                if (item.JackType==jack)
                {
                    item.HasCount++;
                }
            }
            collection.UpdateOne(x => x.AccountID.Equals(account.AccountID) && x.uniacid.Equals(account.uniacid),
                    Builders<AccountModel>.Update
                    .Set(x=>x.Collect,account.Collect)
                    .Set(x=>x.CanOpenJackTimes,account.CanOpenJackTimes-1));

            return jack;
        }
    }
}
