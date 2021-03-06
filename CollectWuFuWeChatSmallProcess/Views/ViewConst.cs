﻿using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectWuFuWeChatSmallProcess.Views
{
    public class ViewConst
    {
        public static bool CheckRout(ViewDataDictionary<dynamic> viewData, RoutType routType)
        {
            return (RoutType)viewData[nameof(routType)] == routType;
        }
        public static void SetRoutType<T>(ViewDataDictionary<T> viewData, RoutType routType)
        {
            viewData[nameof(routType)] = routType;
            viewData["Title"] = routType.ToString();
        }

    }

    public enum RoutType
    {
        登录 = -1, 中奖记录 = 0, 系统管理 = 1, 错误 = 2, 客户信息 = 3, 反馈记录 = 4
    }
}
