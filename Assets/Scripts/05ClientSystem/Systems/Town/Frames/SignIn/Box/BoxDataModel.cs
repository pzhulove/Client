using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class BoxDataModel
    {
        //罐子动画的路径
        public string BoxModelPath;
        //随机道具的名字
        public List<CommonNewItemDataModel> CommonNewItemDataModelList = new List<CommonNewItemDataModel>();
        //奖励的数据
        public CommonNewItemDataModel AwardItemData;
        //是否为大奖的标志
        public bool IsSpecialAward;         
    }
}
