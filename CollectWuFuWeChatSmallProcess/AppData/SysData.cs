using CollectWuFuWeChatSmallProcess.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tools.DB;

namespace CollectWuFuWeChatSmallProcess.AppData
{
    public class SysData
    {
        private static Timer resetTimer;


        public static void ResetShareAndOpenTimes()
        {
            resetTimer = new Timer(DoResetSAOT, null, 0, 1000 * 60 * 10);
        }

        private static void DoResetSAOT(object state)
        {
            var collection = new MongoDBTool().GetMongoCollection<AccountModel>();
            var accountList = collection.Find(Builders<AccountModel>.Filter.Empty).ToList();
            accountList.ForEach(x =>
            {
                if (x.LastRefreshTime.DayOfYear <= DateTime.Now.DayOfYear - 1)
                {
                    collection.UpdateOne(y => y.uniacid.Equals(x.uniacid) && y.AccountID.Equals(x.AccountID),
                        Builders<AccountModel>.Update.Set(y => y.CanShareTimes, 5).Set(y => y.CanOpenJackTimes, 2));
                }

            });
        }
    }
}
