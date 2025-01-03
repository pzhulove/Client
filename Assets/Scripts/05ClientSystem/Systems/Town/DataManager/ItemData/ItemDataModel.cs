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

    //ItemTip的类型
    public enum ItemTipShowAvatarType
    {
        None = 0,                  //不展示
        CompareTipType = 1,         //对比的tip
        SingleTipType = 2,          //单独的一个Tip
    }

    //针对比较属性
    public enum ItemTipCompareItemType
    {
        None = 0,
        CompareAttribute = 1,       //比较属性
        ShowAvatar = 2,              //展示模型
    }

    //礼包中展示ModelAvatar的模型
    public class GiftPackModelAvatarDataModel
    {
        public GiftPackTable.eShowAvatarModelType GiftPackShowModelAvatarType;
        public List<ItemTable> GiftPackShowItemTableList;
        public bool IsOwnerCompleteShowType;                    //包含整套预览的类型
    }

    //礼包中展示ModelAvatar中分页预览的模型
    public class ItemTipModelAvatarEnumerationDataModel
    {
        public int Index;
        public ItemTable ItemTable;
        public bool IsSelectedFlag;

        //职业相关
        public bool IsPlayerProfessionType = false;
        public int ProfessionId = 0;
    }


}
