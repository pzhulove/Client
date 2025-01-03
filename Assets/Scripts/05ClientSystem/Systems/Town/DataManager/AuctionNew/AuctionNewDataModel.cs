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

    public class AuctionNewBuyItemDataModel
    {
        public UInt64 Guid;         // 唯一ID
        public int ItemTypeId;      // 物品TypeID
        public int MoneyTypeId;     // 货币类型ID
        public int Number;             // 道具数量(小于1表示商品本身不限制购买数量,购买上限取决于自身拥有的货币数额)
        public Int64 TotalPrice;    // 总价
        public Int64 SinglePrice;   // 单价
        public int StrengthLevel;   // 强化等级
        public byte EquipType;      //装备类型
        public byte EnhanceType;    //增幅类型
        public int EnhanceNum;      //增幅数量
        public UInt32 PublicEndTime; //结束的时间戳（主要是针对公示中的珍品)
        public byte IsTreasure;         //是否为珍品
        public byte IsAgainOnSale;      //是否为再次上架的商品
        public UInt32 ItemTradeNumber;      //道具已经交易的次数

        public void SetBuyItemDataModel(AuctionBaseInfo auctionBaseInfo)
        {
            Guid = auctionBaseInfo.guid;
            ItemTypeId = (int)auctionBaseInfo.itemTypeId;
            MoneyTypeId = ItemDataManager.GetInstance().GetMoneyTypeID(auctionBaseInfo.pricetype);
            Number = (int)auctionBaseInfo.num;
            TotalPrice = auctionBaseInfo.price;
            SinglePrice = (Number > 0) ? (TotalPrice / Number) : TotalPrice;
            StrengthLevel = (int)auctionBaseInfo.strengthed;
            EquipType = auctionBaseInfo.equipType;
            EnhanceType = auctionBaseInfo.enhanceType;
            EnhanceNum = (int)auctionBaseInfo.enhanceNum;
            PublicEndTime = auctionBaseInfo.publicEndTime;
            IsTreasure = auctionBaseInfo.isTreas;
            IsAgainOnSale = auctionBaseInfo.isAgainOnsale;
            ItemTradeNumber = auctionBaseInfo.itemTransNum;
        }
    }

    public class AuctionSellItemData : System.IComparable<AuctionSellItemData>
    {
        public ulong uId { get; set; }
        public int Quality { get; set; }
        public int Level { get; set; }

        public int IsTreasure { get; set; }     //1 表示珍品， 0表示非珍品

        public AuctionSellItemData(ulong uid, int quality, int level, bool isTreasure = false)
        {
            this.uId = uid;
            this.Quality = quality;
            this.Level = level;
            this.IsTreasure = isTreasure == true ? 1 : 0;
        }

        public int CompareTo(AuctionSellItemData other)
        {
            if (Quality == other.Quality)
            {
                return other.Level - Level;
            }

            return other.Quality - Quality;
        }
    }

}
