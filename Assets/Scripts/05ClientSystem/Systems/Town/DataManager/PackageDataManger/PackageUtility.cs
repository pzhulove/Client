using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;

namespace GameClient
{
    //背包助手类
    public static class PackageUtility
    {
        #region PackageNumberFull
        //检测某种类型的背包是否为满
        public static bool IsPackageFullByType(EPackageType type)
        {
            var itemGuidList = ItemDataManager.GetInstance().GetItemsByPackageType(type);
            var iCount = 0;
            if (itemGuidList != null)
                iCount = itemGuidList.Count;

            if (PlayerBaseData.GetInstance().PackTotalSize.Count > (int)type)
            {
                return PlayerBaseData.GetInstance().PackTotalSize[(int)type] <= iCount;
            }

            return false;
        }

        #endregion

    }
}
