using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Protocol;
using System;
using ProtoTable;
using Network;

namespace GameClient
{
    public class MallTipView : MonoBehaviour, IDisposable
    {
        [SerializeField] private RectTransform mItemRoot;
        [SerializeField] private Text mTextDesp;
        [SerializeField] private Text mTextCount;
        [SerializeField] private Text mTextCost;
        [SerializeField] private Image mImgCost;
        [SerializeField] private Button mBtnAdd;
        [SerializeField] private Button mBtnReduce;
        [SerializeField] private UIGray mGrayAdd;
        [SerializeField] private UIGray mGrayReduce;
        private List<ComItem> mComItems = new List<ComItem>();
        private MallItemInfo MallItemdata = null;
        private int CurNum;
        private int MaxNum;
        private int mPrice;
        //实际需要花费的价格（可能是全价或者打折价格）
        int mTempPrice = 0;
        bool limit;
        int limitnum = 0;
        void Awake()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChangeNum, OnChangeNum);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSyncWorldMallBuySucceed, OnSyncWorldMallBuySucceed);
        }

        public void OnInit(MallItemInfo mallInfo)
        {
            MallItemdata = mallInfo;
            CurNum = 1;
            if (MallItemdata == null) 
                return;
            InitItemInfo();
            SetCountInfo();
        }

        private void SetCountInfo()
        {
            mPrice = Utility.GetMallRealPrice(MallItemdata);
            limit = MallItemdata.limit > 0;
            bool bIsDailyLimit = false;
            limitnum = Utility.GetLeftLimitNum(MallItemdata, ref bIsDailyLimit);
            var costItemTable = MallNewDataManager.GetInstance().GetCostItemTableByCostType(MallItemdata.moneytype);
            if (costItemTable != null)
            {
                if (mImgCost != null)
                {
                    ETCImageLoader.LoadSprite(ref mImgCost, costItemTable.Icon);
                }
                var ownerCostNumber = ItemDataManager.GetInstance().GetOwnedItemCount(costItemTable.ID);
            }
            MaxNum = GetMaxCount();
            if (MaxNum < 1)
            {
                MaxNum = 1;
            }
            OnCountChanged(1);
        }
        private int GetMaxCount()
        {
            var costItemTable = MallNewDataManager.GetInstance().GetCostItemTableByCostType(MallItemdata.moneytype);
            var ownerCostNumber = ItemDataManager.GetInstance().GetOwnedItemCount(costItemTable.ID);

            //五一礼包标记并且全民团购活动开启
            if (MallItemdata.tagType == 2 && ActivityDataManager.GetInstance().CheckGroupPurchaseActivityIsOpen())
            {
                var voucherTicketData = TableManager.GetInstance().GetTableItem<ItemTable>((int)MallItemdata.deductionCouponId);
                var discountTicketData = TableManager.GetInstance().GetTableItem<ItemTable>((int)MallItemdata.discountCouponId);

                int voucherTicketCount = 0;
                if (voucherTicketData != null)
                {
                    voucherTicketCount = ItemDataManager.GetInstance().GetOwnedItemCount(voucherTicketData.ID);
                }

                int discountTicketCount = 0;
                if (discountTicketData != null)
                {
                    discountTicketCount = ItemDataManager.GetInstance().GetOwnedItemCount(discountTicketData.ID);
                }


                int index = 0;
                int totalPrice = 0;
                while (true)
                { 
                    //乘以最终折扣系数
                    int price = GetMayDayDiscountPrice(mPrice, ActivityDataManager.LimitTimeGroupBuyDiscount);
                    
                    if (discountTicketData != null)
                    {  //计算折扣券
                        if (index < discountTicketCount)
                        {
                            price = GetMayDayDiscountPrice(price, discountTicketData.DiscountCouponProp);
                        }
                    }

                    if (voucherTicketData != null)
                    {
                        //计算抵扣券
                        if (index < voucherTicketCount)
                        {
                            price = price - voucherTicketData.DiscountCouponProp;
                        }
                    }

                    totalPrice += price;

                    //总价格大于已有点券 
                    if (totalPrice > ownerCostNumber)
                    {
                        return index;
                    }

                    index++;
                }
            }
            else
            {
                if (!ActivityDataManager.GetInstance().IsShowFirstDiscountDes(MallItemdata.id))
                {
                    if (costItemTable != null)
                    {
                        var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>((int)MallItemdata.discountCouponId);
                        if (itemTableData != null && itemTableData.DiscountCouponProp != 0)
                        {
                            //折扣卷的数量
                            int itemCount = ItemDataManager.GetInstance().GetOwnedItemCount((int)MallItemdata.discountCouponId);
                            //折扣之后的价格
                            int tempDisPrice = GetDiscountPrice(mPrice, itemTableData.DiscountCouponProp);

                            int canBuyCount;
                            if (tempDisPrice != 0)
                            {
                                canBuyCount = ownerCostNumber / tempDisPrice;//全用打折券可以买多少个
                            }
                            else
                            {
                                Logger.LogErrorFormat("tempDisPrice = 0为分母 其中mPrice = {0},itemID = {1}", mPrice, itemTableData.ID);
                                canBuyCount = 1;
                            }

                            if (canBuyCount <= itemCount)
                            {
                                MaxNum = canBuyCount;
                            }
                            else
                            {
                                if (mPrice != 0)
                                {
                                    //打折的数量+不打折的数量
                                    MaxNum = itemCount + (ownerCostNumber - (tempDisPrice * itemCount)) / mPrice;
                                }
                                else
                                {
                                    Logger.LogErrorFormat("price = 0 其中itemID = {0}", itemTableData.ID);
                                    MaxNum = 1;
                                }
                            }
                        }
                        else
                        {
                            if (mPrice > 0)
                                MaxNum = ownerCostNumber / mPrice;
                        }
                        return MaxNum;
                    }
                }
                else
                {
                    var activityData = ActivityDataManager.GetInstance().GetActiveDataFromType(ActivityLimitTimeFactory.EActivityType.OAT_LIMITTIMEFIRSTDISCOUNTACTIIVITY);
                    if (activityData != null)
                    {
                        //折扣之后的价格
                        int tempDisPrice = GetDiscountPrice(mPrice, (int)activityData.parm);
                        if (ownerCostNumber >= tempDisPrice * 1)
                        {   //可以折扣+不能折扣的价格
                            MaxNum = 1 + (ownerCostNumber - tempDisPrice * 1) / mPrice;
                        }
                        else
                        {
                            MaxNum = 0;
                        }

                    }
                    return MaxNum;
                }
            }
            return 0;
        }
        private int GetMayDayDiscountPrice(float price,float discount)
        {
            return (int)Math.Floor(Convert.ToDecimal(price * discount * 1.0f / 100));
        }
        private int GetDiscountPrice(int price, int discount)
        {
            return (int)Math.Floor(Convert.ToDecimal(price * discount * 1.0f / 100));
        }
        //数量变化
        public void OnCountChanged(int count)
        {
            if (count > MaxNum)
            {
                OnCountChanged(MaxNum);
                return;
            }
            if (count < 1)
            {
                OnCountChanged(1);
                return;
            }
            CurNum = count;
            UpdateNumButtonState();
            UpdateText();
        }

        private void UpdateText()
        {
            mTempPrice = CurNum * mPrice;
            mTextCount.text = CurNum.ToString();
            mTextCost.text = mTempPrice.ToString();
        }
        //更新按钮状态
        void UpdateNumButtonState()
        {
            if (MaxNum < 1)
            {
                mGrayReduce.enabled = true;
                mBtnReduce.interactable = false;

                if (mGrayAdd != null)
                {
                    mGrayAdd.enabled = true;
                }
                
                mBtnAdd.interactable = false;
            }
            else
            {
                if ((limit && CurNum >= limitnum) || CurNum >= MaxNum)
                {
                    if (mGrayAdd != null)
                    {
                        mGrayAdd.enabled = true;
                    }
                    mBtnAdd.interactable = false;
                }
                else
                {
                    if (mGrayAdd != null)
                    {
                        mGrayAdd.enabled = false;
                    }
                    mBtnAdd.interactable = true;
                }

                if (CurNum <= 1)
                {
                    mGrayReduce.enabled = true;
                    mBtnReduce.interactable = false;
                }
                else
                {
                    mGrayReduce.enabled = false;
                    mBtnReduce.interactable = true;
                }
            }
        }
        //+号
        public void OnClickAdd()
        {
            OnCountChanged(CurNum + 1);
        }
        //-号
        public void OnClickReduce()
        {
            OnCountChanged(CurNum - 1);
        }
        //打开小键盘
        public void OnClickOpenInputFrame()
        {
            ClientSystemManager.GetInstance().OpenFrame<VirtualKeyboardFrame>();
        }
        //小键盘输入
        private void OnChangeNum(UIEvent uiEvent)
        {
            int count = CurNum;
            ChangeNumType changeNumType = (ChangeNumType)uiEvent.Param1;
            if (changeNumType == ChangeNumType.BackSpace)
            {
                if (count < 10)
                {
                    count = 1;
                }
                else
                {
                    count = count / 10;
                }
            }
            else if (changeNumType == ChangeNumType.Add)
            {
                int addNum = (int)uiEvent.Param2;
                if (addNum < 0 || addNum > 9)
                {
                    Logger.LogErrorFormat("传入数字不合法，请控制在0-9之间");
                    return;
                }
                count = count * 10 + addNum;
                if (count < 1)
                {
                    Logger.LogErrorFormat("tempCurNum is error");
                }

                if (MaxNum < count)
                {
                    count = MaxNum;
                    // SystemNotifyManager.SysNotifyTextAnimation("超出可购买数量");
                }
            }
            OnCountChanged(count);
        }
        //点击购买
        public void OnClickBuy()
        {
            if(MallItemdata!=null)
            {
                if (CurNum <= 0)
                {
                    if (MaxNum >= 1)
                        SystemNotifyManager.SysNotifyTextAnimation("请选择购买数量");
                    else
                        SystemNotifyManager.SystemNotify(1086);
                    return;
                }
                CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();
                costInfo.nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType((ItemTable.eSubType)MallItemdata.moneytype);
                costInfo.nCount = mTempPrice;
                OnBuyItemClick(costInfo);
            }
        }
        void OnBuyItemClick(CostItemManager.CostInfo costInfo)
        {
            CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
            {
                if (MallItemdata.multiple > 0)
                {
                    string content = string.Empty;

                    //积分商城积分等于上限值
                    if ((int)PlayerBaseData.GetInstance().IntergralMallTicket == MallNewUtility.GetIntergralMallTicketUpper() &&
                         MallNewDataManager.GetInstance().bItemMallIntergralMallScoreIsEqual == false)
                    {
                        content = TR.Value("mall_buy_intergral_mall_score_equal_upper_value_desc");
                        MallNewUtility.CommonIntergralMallPopupWindow(content, MallNewUtility.ItemMallIntergralMallScoreIsEqual, () => { OnSendWorldMallBuy(MallItemdata, CurNum); });
                    }
                    else
                    {
                        int ticketConvertScoreNumber = MallNewUtility.GetTicketConvertIntergalNumnber(mTempPrice) * MallItemdata.multiple;
                        int allIntergralScore = (int)PlayerBaseData.GetInstance().IntergralMallTicket + ticketConvertScoreNumber;
                        //购买道具后商城积分超出上限值
                        if (allIntergralScore > MallNewUtility.GetIntergralMallTicketUpper() &&
                           (int)PlayerBaseData.GetInstance().IntergralMallTicket != MallNewUtility.GetIntergralMallTicketUpper() &&
                            MallNewDataManager.GetInstance().bItemMallIntergralMallScoreIsExceed == false)
                        {
                            content = TR.Value("mall_buy_intergral_mall_score_exceed_upper_value_desc",
                                               (int)PlayerBaseData.GetInstance().IntergralMallTicket,
                                               MallNewUtility.GetIntergralMallTicketUpper(),
                                               MallNewUtility.GetIntergralMallTicketUpper() - (int)PlayerBaseData.GetInstance().IntergralMallTicket);
                            MallNewUtility.CommonIntergralMallPopupWindow(content, MallNewUtility.ItemMallIntergralMallScoreIsExceed, () => { OnSendWorldMallBuy(MallItemdata, CurNum); });
                        }
                        else
                        {//未超出
                            OnSendWorldMallBuy(MallItemdata, CurNum);
                        }
                    }
                }
                else
                {
                    OnSendWorldMallBuy(MallItemdata, CurNum);
                }
            });
        }
        void OnSendWorldMallBuy(MallItemInfo MallItemdata, int CurNum)
        {
            WorldMallBuy req = new WorldMallBuy();
            req.itemId = MallItemdata.id;
            req.num = (UInt16)CurNum;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        public void Dispose()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ChangeNum, OnChangeNum);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSyncWorldMallBuySucceed, OnSyncWorldMallBuySucceed);
            for (int index = this.mComItems.Count - 1; index >= 0; --index)
            {
                ComItemManager.Destroy(mComItems[index]);
            }
            mComItems.Clear();
        }

        //初始化道具信息
        private void InitItemInfo()
        {
            //描述
            if (MallItemdata.itemid == 0)
                mTextDesp.SafeSetText(MallItemdata.giftDesc);
            else
            {
                ItemTable ItemTableData = TableManager.GetInstance().GetTableItem<ItemTable>((int)MallItemdata.itemid);
                mTextDesp.SafeSetText(ItemTableData.Description);
            }
            //道具
            var awards = MallItemdata.giftItems;
            if (awards == null || awards.Length == 0)
            {
                return;
            }

            for (int index = 0; index < awards.Length; ++index)
            {
                if (mComItems.Count > index)
                {
                    var comItem = mComItems[index];
                    comItem.CustomActive(true);
                    if (comItem != null)
                    {
                        ItemData data = ItemDataManager.CreateItemDataFromTable((int)awards[index].id);
                        if (data == null)
                        {
                            Logger.LogError("道具表找补到id为" + awards[index].id + "的道具");
                            continue;
                        }
                        data.Count = (int)awards[index].num;
                        data.StrengthenLevel = awards[index].strength;
                        comItem.Setup(data, (obj, itemData) =>
                        {
                            Utility.OnItemClicked(gameObject, data);
                        });
                        comItem.NeedShowName(true);
                    }
                }
                else
                {
                    var comItem = ComItemManager.Create(this.mItemRoot.gameObject);
                    if (comItem != null)
                    {
                        ItemData data = ItemDataManager.CreateItemDataFromTable((int)awards[index].id);
                        if (data == null)
                        {
                            Logger.LogError("道具表找补到id为" + awards[index].id + "的道具");
                            continue;
                        }
                        data.Count = (int)awards[index].num;
                        data.StrengthenLevel = awards[index].strength;
                        comItem.Setup(data, (obj, itemData) =>
                        {
                            Utility.OnItemClicked(gameObject, data);
                        });
                        comItem.NeedShowName(true);
                        mComItems.Add(comItem);
                    }
                }
            }
            for (int index = awards.Length; index < mComItems.Count; ++index)
            {
                mComItems[index].CustomActive(false);
            }
        }

        private void OnSyncWorldMallBuySucceed(UIEvent uiEvent)
        {
            if (uiEvent == null)
                return;

            if (uiEvent.Param1 == null || uiEvent.Param2 == null)
                return;
            //购买完成 刷新次数
            var itemId = (UInt32)uiEvent.Param1;
            if (itemId == MallItemdata.id)
            {
                SetCountInfo();
            }
        }
    }
}
