using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using Protocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class TriggerMallItem : MonoBehaviour
    {
        [SerializeField] private GameObject mItemParent;
        [SerializeField] private Text mName;
        [SerializeField] private Text mDiscountPrice;
        [SerializeField] private Text mLimitBuyCount;
        [SerializeField] private UIGray mBuyGray;
        [SerializeField] private SimpleTimer mTimer;
        private MallItemInfo mMallItem;
        public void OnInit(MallItemInfo mallItemInfo)
        {
            mMallItem = mallItemInfo;
            ComItem comItem = ComItemManager.Create(mItemParent);
            int id = (int)mMallItem.itemid;
            int num = (int)mMallItem.itemnum;
            if (0 == id)
            {
                id = (int)mMallItem.giftItems[0].id;
                num = (int)mMallItem.giftItems[0].num;
            }
            ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(id);
            itemData.Count = num;
            comItem.Setup(itemData, Utility.OnItemClicked);
            //名字
            mName.text = itemData.GetColorName();
            //折扣价
            mDiscountPrice.text = mMallItem.discountprice.ToString();
            
            var roleLimitCount = this.mMallItem.limitnum > this.mMallItem.limittotalnum ? this.mMallItem.limitnum : this.mMallItem.limittotalnum;
            var roleBuyCount = CountDataManager.GetInstance().GetCount(this.mMallItem.id.ToString());
            //剩余次数
            int iResidueDegree = roleLimitCount - roleBuyCount;
            //限购次数
            mLimitBuyCount.text = string.Format("限购次数:{0}/{1}", iResidueDegree, roleLimitCount);
            mBuyGray.enabled = iResidueDegree <= 0;
            mTimer.SetCountdown((int)((int)mMallItem.endtime - TimeManager.GetInstance().GetServerTime()));
            mTimer.StartTimer();
        }

        //点击购买道具
        public void OnClickBuyItem()
        {
            if (null == mMallItem)
                return;
            var costInfo = new CostItemManager.CostInfo()
            {
                nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType((ProtoTable.ItemTable.eSubType)mMallItem.moneytype),
                nCount = (int)mMallItem.discountprice,
            };
            CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, _ReqBuyMallItem);
        }
        private void _ReqBuyMallItem()
        {
            WorldMallBuy req = new WorldMallBuy();
            req.itemId = mMallItem.id;
            req.num = (UInt16)1;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }
    }
}
