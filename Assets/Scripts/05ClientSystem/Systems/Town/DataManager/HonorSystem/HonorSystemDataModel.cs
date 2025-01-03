using System;
using System.Collections.Generic;
using Protocol;
using ProtoTable;

namespace GameClient
{

    //pvp次数统计
    public class PvpNumberStatistics
    {
        public uint PvpType;
        public uint PvpCount;
        public string PvpName;
        public string PvpIconPath;
        public int PvpSort;
        public bool IsNewFlag;          //只有当当前次数为1的时候，才有可能为true，其他情况均为false
    }

    //历史荣誉
    public class PlayerHistoryHonorInfo
    {
        public HONOR_DATE_TYPE HonorDateType;
        public uint HonorTotalNumber;
        public List<PvpNumberStatistics> PvpNumberStatisticsList = new List<PvpNumberStatistics>();
    }
    
    //预览Item
    public class PreviewLevelItemDataModel
    {
        public int HonorSystemLevel;                //等级
        public int HonorSystemTotalNeedExpValue;        //总共需要的经验值
        public int HonorSystemNeedExpValue;         //从6级到7级需要的经验值（7级经验值-6级经验值)
        public int TitleId;                         //对应头衔表的Id
        public string HonorLevelName;               //经验名字
        public string HonorLevelFlagPath;           //等级对应Icon的路径
        public List<string> ShopDiscountList = new List<string>();
        public List<int> UnLockShopItemList = new List<int>();     //当前等级一共解锁的商店道具
    }

    public class ReceiveItemDataModel
    {
        public int ItemId;
        public int MinNumber;
        public int MaxNumber;
    }

}
