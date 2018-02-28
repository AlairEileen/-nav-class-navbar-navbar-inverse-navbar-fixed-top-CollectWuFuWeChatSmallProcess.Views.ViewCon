using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools.Models;
using Tools.Response.Json;

namespace CollectWuFuWeChatSmallProcess.Models
{
    public class AccountModel : BaseAccount
    {
        public string OpenID { get; set; }
        /// <summary>
        /// 微擎专用
        /// </summary>
        public string uniacid { get; set; }

        public AccountInfo Info { get; set; }


        private JackCollect[] collect = 
        {
        new JackCollect{JackType= JackType.喜神, HasCount=0},
        new JackCollect{JackType= JackType.寿星, HasCount=0},
        new JackCollect{JackType= JackType.禄星, HasCount=0},
        new JackCollect{JackType= JackType.福星, HasCount=0},
        new JackCollect{JackType= JackType.财神, HasCount=0}
        };

        private int canOpenJackTimes = 3;

        private int canShareTimes = 5;
        public int CanOpenJackTimes { get => canOpenJackTimes; set => canOpenJackTimes = value; }
        public int CanShareTimes { get => canShareTimes; set => canShareTimes = value; }

        [JsonConverter(typeof(Tools.Response.Json.DateConverterEndMinute))]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [JsonIgnore]
        public DateTime LastRefreshTime { get => lastRefreshTime; set => lastRefreshTime = value; }
        public JackCollect[] Collect { get => collect; set => collect = value; }
      

        private DateTime lastRefreshTime = DateTime.Now;
    }

    public class AccountInfo
    {
        public string Address { get; set; }
        public string Brief { get; set; }
    }


    public enum JackType
    {
        福星 = 1, 禄星 = 2, 寿星 = 3, 喜神 = 4, 财神 = 5
    }
    public class JackCollect
    {
        public JackType JackType { get; set; }
        public int HasCount { get; set; }
    }
}
