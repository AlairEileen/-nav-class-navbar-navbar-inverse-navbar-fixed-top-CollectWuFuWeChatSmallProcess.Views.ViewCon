using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigData
{
    public class MainConfig
    {
        /// <summary>
        /// 是否为开发版
        /// </summary>
        public const bool IsDev = false;

        /// <summary>
        /// 项目相关参数
        /// </summary>
        public const string
            ProjDevName = "CollectWuFu_Dev",
            ProjLineName = "CollectWuFu",
            BaseDir = "/home/project_data/" + (IsDev ? ProjDevName : ProjLineName) + "/",
            AvatarDir = "avatar/",
            AlbumDir= "album/",
            GoodsImagesDir = "goods/",
            LogoImagesDir = "logo/",
            TempDir = "temp/",
            CertsDir = "certs/";

        /// <summary>
        /// 数据库参数
        /// </summary>
        public const string
            MongoDBLineConn = "mongodb://47.94.208.29:27027",
            MongoDBLocalConn = "mongodb://localhost:27027",
            MongoDBName = IsDev ? ProjDevName : ProjLineName;

    }
}
