using System;
using System.Collections.Generic;
///////删除linq
using System.Text;

namespace GameClient
{
    static class CustomDecoder
    {
        // 大坑！！！！服务器发送的数据类型是 ItemReward，客户端解析用的 RewardItem
        // 这两个结构体必须保证字段类型和顺序一致，不然出bug！！！！
        public struct RewardItem 
        {
            public UInt32 ID;
            public UInt32 Num;
            public byte qualityLv;
            public byte strength;
			public UInt32 auctionCoolTimeStamp;
            public byte equipType;
        }

        public class StrengthenRet
        {
            public UInt32 code;
            public List<RewardItem> BrokenItems;
        }

        public static List<RewardItem> DecodeGetRewards(byte[] buffer, ref int pos, int length, ref UInt32 itemSource, ref byte notify)
        {
            BaseDLL.decode_uint32(buffer, ref pos, ref itemSource);
            BaseDLL.decode_int8(buffer, ref pos, ref notify);

            List<RewardItem> rewardItems = new List<RewardItem>();
            if (rewardItems == null)
            {
                Logger.LogError("new List<RewardItem>() failed");
                return null;
            }

            UInt16 size = 0;
            BaseDLL.decode_uint16(buffer, ref pos, ref size);
            for (int i = 0; i < size; ++i)
            {
                RewardItem rewardItem = new RewardItem();
                BaseDLL.decode_uint32(buffer, ref pos, ref rewardItem.ID);                
                BaseDLL.decode_uint32(buffer, ref pos, ref rewardItem.Num);
                BaseDLL.decode_int8(buffer, ref pos, ref rewardItem.qualityLv);
                BaseDLL.decode_int8(buffer, ref pos, ref rewardItem.strength);
				BaseDLL.decode_uint32(buffer, ref pos, ref rewardItem.auctionCoolTimeStamp);
                BaseDLL.decode_int8(buffer, ref pos, ref rewardItem.equipType);
                rewardItems.Add(rewardItem);
            }

            return rewardItems;
        }

        public static bool DecodeStrengthenResult(out StrengthenRet ret, byte[] buffer, ref int pos, int length)
        {
            ret = new StrengthenRet();
            BaseDLL.decode_uint32(buffer, ref pos, ref ret.code);
            ret.BrokenItems = new List<RewardItem>();
            UInt16 size = 0;
            BaseDLL.decode_uint16(buffer, ref pos, ref size);
            for (int i = 0; i < size; ++i)
            {
                RewardItem brokenItem = new RewardItem();
                BaseDLL.decode_uint32(buffer, ref pos, ref brokenItem.ID);
                BaseDLL.decode_uint32(buffer, ref pos, ref brokenItem.Num);
                BaseDLL.decode_int8(buffer, ref pos, ref brokenItem.qualityLv);
                BaseDLL.decode_int8(buffer, ref pos, ref brokenItem.strength);
				BaseDLL.decode_uint32(buffer, ref pos, ref brokenItem.auctionCoolTimeStamp);
                BaseDLL.decode_int8(buffer, ref pos, ref brokenItem.equipType);
                ret.BrokenItems.Add(brokenItem);
            }

            return true;
        }

        public class ProtoShopItem
        {
            //商店id
            public UInt64 shopId;
            //商店道具id
            public UInt32 shopItemId;
            //格子
            public byte grid;
            //剩余数量
            public Int16 restNum;
			//次数限制
			public Int16 limiteNum;
			//vip等级
			public byte vipLv;
            //vip折扣
            public byte vipDiscount;
            //折扣
            public UInt32 discount;
			//租赁结束时间戳
			public UInt32 leaseEndTimeStamp;
        }

        public class ProtoShop
        {
            public UInt32 shopID;
            // 刷新次数
            public UInt16 refreshCost;
            // 普通商店列表
            public List<ProtoShopItem> shopItemList;
            // 每日刷新剩余时间
            public UInt32 restRefreshTime;
            // 还能刷新的次数
            public byte refreshTimes;
            // 商店刷新总次数
            public byte refreshAllTimes;
            // 每周刷新剩余时间
            public UInt32 WeekRestRefreshTime;
            // 每月刷新剩余时间
            public UInt32 MonthRefreshTime;
        }

        public static bool DecodeShop(out ProtoShop ret, byte[] buffer, ref int pos, int length)
        {
            ret = new ProtoShop();
            BaseDLL.decode_uint32(buffer, ref pos, ref ret.shopID);
            BaseDLL.decode_uint16(buffer, ref pos, ref ret.refreshCost);
            ret.shopItemList = new List<ProtoShopItem>();
            UInt16 size = 0;
            BaseDLL.decode_uint16(buffer, ref pos, ref size);
            for (int i = 0; i < size; ++i)
            {
                ProtoShopItem shopItem = new ProtoShopItem();
                BaseDLL.decode_uint32(buffer, ref pos, ref shopItem.shopItemId);
                BaseDLL.decode_int8(buffer, ref pos, ref shopItem.grid);
                BaseDLL.decode_int16(buffer, ref pos, ref shopItem.restNum);
				BaseDLL.decode_int16(buffer, ref pos, ref shopItem.limiteNum);
				BaseDLL.decode_int8(buffer, ref pos, ref shopItem.vipLv);
                BaseDLL.decode_int8(buffer, ref pos, ref shopItem.vipDiscount);
                BaseDLL.decode_uint32(buffer, ref pos, ref shopItem.discount);
				BaseDLL.decode_uint32(buffer, ref pos, ref shopItem.leaseEndTimeStamp);
                ret.shopItemList.Add(shopItem);
            }
            BaseDLL.decode_uint32(buffer, ref pos, ref ret.restRefreshTime);
            BaseDLL.decode_int8(buffer, ref pos, ref ret.refreshTimes);
            BaseDLL.decode_int8(buffer, ref pos, ref ret.refreshAllTimes);
            BaseDLL.decode_uint32(buffer, ref pos, ref ret.WeekRestRefreshTime);
            BaseDLL.decode_uint32(buffer, ref pos, ref ret.MonthRefreshTime);

            return true;
        }
    }
}
