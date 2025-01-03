using System;
using System.Collections.Generic;
using ActivityLimitTime;
using GameClient;
using LimitTimeGift;
using Protocol;
using ProtoTable;
using UnityEngine;

namespace GameClient
{

    public struct LimitTimeGiftPackModel : ILimitTimeNote
    {

        public uint Id { get; private set; } //活动id

        public uint StartTime { get; private set; }

        public uint EndTime { get; private set; }

        public string RuleDesc { get; private set; }//规则描述

        public string LogoDesc { get; private set; }//logo描述

        public string Desc { get; private set; }//活动描述

        public string LogoPath { get; private set; }//logo资源路径

        public string NoteBgPath { get; private set; }//上部宣传图

        public string NotePrefabPath { get; private set; }//上部预制体路径

        public string ItemPath { get; private set; }//item prefab路径

        public OpActivityState State { get; private set; }//活动状态

        public string Name { get; private set; }//活动名

        public List<LimitTimeGiftPackDetailModel> DetailDatas { get; private set; }

        public MallTypeTable.eMallType MallType{get; private set;}

        public LimitTimeGiftPackModel(OpActivityData msg, List<MallItemInfo> itemInfos, MallTypeTable.eMallType mallType, string itemPath, string logoPath = null, string noteBgPath = null, string notePrefabPath = null) : this()
        {
            Id = msg.dataId;
            Name = msg.name;
            RuleDesc = msg.ruleDesc;
            LogoDesc = msg.logoDesc;
            State = (OpActivityState)msg.state;
            Desc = msg.desc;
            LogoPath = logoPath;
            ItemPath = itemPath;
            DetailDatas = new List<LimitTimeGiftPackDetailModel>();
            if (itemInfos != null)
            {
                for (int i = 0; i < itemInfos.Count; ++i)
                {
                    DetailDatas.Add(new LimitTimeGiftPackDetailModel(itemInfos[i]));
                }
            }

            StartTime = msg.startTime;
            EndTime = msg.endTime;
            NoteBgPath = noteBgPath;
            NotePrefabPath = notePrefabPath;
            MallType = mallType;
        }

        /// <summary>
        /// 更新商品.注意 这个是结构体
        /// </summary>
        /// <param name="giftId"></param>
        public void UpdateItem(int itemId)
        {
            bool isContain = false;
            var itemInfo = ActivityDataManager.GetInstance().GetGiftPackData(MallType, (uint)itemId);

            for (int i = 0; i < DetailDatas.Count; ++i)
            {
                if (DetailDatas[i].Id == itemId)
                {
                    //如果数据层没有 说明要任务已经没有了
                    if (itemInfo != null)
                    {
                        DetailDatas[i] = new LimitTimeGiftPackDetailModel(itemInfo);
                    }
                    else
                    {
                        DetailDatas.RemoveAt(i);
                    }
                    isContain = true;
                    break;
                }
            }

            //如果是新数据 则加入
            if (itemInfo != null && !isContain && DetailDatas != null)
            {
                DetailDatas.Add(new LimitTimeGiftPackDetailModel(itemInfo));
            }
        }
    }

    public struct LimitTimeGiftPackDetailModel
    {
        public uint Id { get; private set; }//商品id
        public int ItemId { get; private set; }//道具表id
        public int ItemNum { get; private set; }//数量
        public string Name { get; private set; }//礼包名字
        public ItemReward[] mRewards { get; private set; } //礼包内容
        public MallGoodsType GiftType { get; private set; }
        public int GiftPrice{ get; private set;}
        public LimitTimeGiftPriceType PriceType { get; private set; }
        public int LimitTotalNum { get; private set; }
        public bool NeedTimeCountDown
        {
            get
            {
                return GiftIntraDay == LimitTimeGiftIntraDay.IntraDay ? true : false;
            }
        }
        public MallBuyGotInfo[] buyGotInfos { get; private set; }
        //一天内（限时）
        //private LimitTimeGiftIntraDay giftIntraDay;
        public LimitTimeGiftIntraDay GiftIntraDay
        {
            get
            {
                if (GiftState == LimitTimeGiftState.SoldOut)
                    return LimitTimeGiftIntraDay.None;
                return RemainingTimeSec > 3600 * 24 ? LimitTimeGiftIntraDay.MoreThanOneDay : LimitTimeGiftIntraDay.IntraDay;
            }
            //set { giftIntraDay = value; }
        }
        public LimitTimeGiftState GiftState
        {
            get
            {
                return (LimitPurchaseNum > 0 || this.LimitType == ELimitiTimeGiftDataLimitType.None) ? LimitTimeGiftState.OnSale : LimitTimeGiftState.SoldOut;
            }
            //set { giftState = value; }
        }
        public uint RemainingTimeSec { get; private set; }
        public int LimitPurchaseNum { get; private set; }
        public ELimitiTimeGiftDataLimitType LimitType { get; private set; }
        public uint GiftEndTime { get; private set; }
        public uint GiftStartTime { get; private set; }
        public uint LimitNum { get; private set; }

        public byte Limit { get; private set; }
        public uint DiscountCouponId { get; private set; }

        public uint AccountLimitBuyNum { get; private set; }

        public uint AccountRestBuyNum { get; private set; }

        public uint AccountRefreshType { get; private set; }

        public LimitTimeGiftPackDetailModel(MallItemInfo msgData) : this()
        {
            Id = msgData.id;
            Name = msgData.giftName;
            ItemId = (int) msgData.itemid;
            ItemNum = (int) msgData.itemnum;
            mRewards = msgData.giftItems;
            GiftType = (MallGoodsType)msgData.gift;
            GiftPrice = Utility.GetMallRealPrice(msgData);
            PriceType = (LimitTimeGiftPriceType) msgData.moneytype;
            LimitTotalNum = msgData.limittotalnum;
            RemainingTimeSec = msgData.endtime - TimeManager.GetInstance().GetServerTime();
            if (msgData.gift == (int)MallGoodsType.GIFT_DAILY_REFRESH || msgData.gift == (int)MallGoodsType.GIFT_COMMON_DAILY_REFRESH)
            {
                LimitPurchaseNum = msgData.limitnum;
            }
            else
            {
                LimitPurchaseNum = msgData.limittotalnum;
            }
            LimitType = (ELimitiTimeGiftDataLimitType)msgData.limit;
            GiftEndTime = msgData.endtime;
            GiftStartTime = msgData.starttime;
            LimitNum = msgData.limitnum;
            buyGotInfos = msgData.buyGotInfos;
            Limit = msgData.limit;
            DiscountCouponId = msgData.discountCouponId;
            AccountLimitBuyNum = msgData.accountLimitBuyNum;
            AccountRestBuyNum = msgData.accountRestBuyNum;
            AccountRefreshType = msgData.accountRefreshType;
        }
    }
    
}