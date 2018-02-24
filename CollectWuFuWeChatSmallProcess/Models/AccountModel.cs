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

    }

    public class AccountInfo
    {
        public string Address { get; set; }
        public string Brief { get; set; }
    }
}
