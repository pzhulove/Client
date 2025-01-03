using System;
using Protocol;
using UnityEngine.UI;
using ProtoTable;
using UnityEngine;
using Network;
using System.Collections.Generic;
using System.Globalization;
using Scripts.UI;
using ActivityLimitTime;

namespace GameClient
{
    class MallBuyFrame : ClientFrame
    {
        MallItemInfo mMallItemData = null;
        ItemTable.eSubType moneyType = ItemTable.eSubType.BindPOINT;

        int CurNum = 1;
        int MaxNum = 1;
        //真正价格
        int iPrice = 0;
        float fBaseSliderValue = 0.0f;
        //是否有购买限制
        bool mHaveBuyLimit;
        int limitnum = 0;
        int tipsID = -1;

        //这个值是判断数字键盘上此时的数字是否是真的1（假的1的时候再输入数字会把1抹去换成输入数字）
        bool isTrueNum = false;
        bool needDiscountTips = false;

        //实际需要花费的价格（可能是全价或者打折价格）
        int mTempPrice = 0;
        //商品内包含的道具
        List<ItemReward> mMallItemList = new List<ItemReward>();

        private bool _isUpdate = false; //积分时间戳更新
        //首次购买原本颜色
        private Color mOriginalColor;
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Mall/MallBuyFrame";
        }

        protected override void _OnOpenFrame()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChangeNum, OnChangeNum);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSendQueryMallItemInfo, ReqQueryMallItemInfo);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnQueryMallItenInfoSuccess, OnQueryMallItenInfoSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.AccountSpecialItemUpdate, _OnCountValueChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityLimitTimeDataUpdate, _OnLimitTimeDataUpdate);
            mMallItemData = (MallItemInfo)userData;
            if (mMallItemData == null)
                return;
            _InitView();
            _InitData();
        }

        protected override void _OnCloseFrame()
        {
            needDiscountTips = false;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ChangeNum, OnChangeNum);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSendQueryMallItemInfo, ReqQueryMallItemInfo);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnQueryMallItenInfoSuccess, OnQueryMallItenInfoSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.AccountSpecialItemUpdate, _OnCountValueChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityLimitTimeDataUpdate, _OnLimitTimeDataUpdate);
            ClearData();
            if (ClientSystemManager.GetInstance().IsFrameOpen<VirtualKeyboardFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<VirtualKeyboardFrame>();
            }
        }

        public override bool IsNeedUpdate()
        {
            return _isUpdate;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            if (mMallItemData == null)
            {
                return;
            }
            int time = (int)(mMallItemData.multipleEndTime - TimeManager.GetInstance().GetServerTime());
            if (time <= 0)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSendQueryMallItemInfo, mMallItemData.itemid);
                _isUpdate = false;
            }
        }

        void ClearData()
        {
            mMallItemData = null;
            moneyType = ItemTable.eSubType.BindPOINT;

            CurNum = 1;
            MaxNum = 1;
            iPrice = 0;
            fBaseSliderValue = 0.0f;

            if (kSlider != null)
            {
                kSlider.onValueChanged.RemoveAllListeners();
                kSlider = null;
            }

            _isUpdate = false;
        }

        [UIEventHandle("title/closeicon")]
        void OnClose()
        {
            ClearData();
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("Middle/minus")]
        void OnLeft()
        {
            if (CurNum <= 1)
            {
                return;
            }
            CurNum--;
            isTrueNum = true;
            ShowDataText();
            UpdateNumButtonState();
            if (kSlider.value <= CurNum * fBaseSliderValue || kSlider.value > (CurNum + 1) * fBaseSliderValue)
            {
                kSlider.value = CurNum * fBaseSliderValue;
            }
        }

        [UIEventHandle("Middle/add")]
        void OnRight()
        {
            if (mHaveBuyLimit)
            {
                if (limitnum >= 1 && CurNum >= limitnum)
                {
                    return;
                }
            }
            if (CurNum >= MaxNum)
            {
                return;
            }
            CurNum++;
            isTrueNum = true;
            ShowDataText();
            UpdateNumButtonState();
            if (kSlider.value <= CurNum * fBaseSliderValue || kSlider.value > (CurNum + 1) * fBaseSliderValue)
            {
                kSlider.value = CurNum * fBaseSliderValue;
            }
        }

        [UIEventHandle("Middle/max")]
        void OnMax()
        {
            if (mHaveBuyLimit)
            {
                if (MaxNum < limitnum)
                {
                    CurNum = MaxNum;
                }
                else
                {
                    CurNum = limitnum;
                }
            }
            else
            {
                CurNum = MaxNum;
            }
            isTrueNum = true;
            ShowDataText();
            UpdateNumButtonState();

            kSlider.value = 1.0f;
        }

        void OnChangeNum(UIEvent uiEvent)
        {
            ChangeNumType changeNumType = (ChangeNumType)uiEvent.Param1;
            if (changeNumType == ChangeNumType.BackSpace)
            {
                if (isTrueNum == false)
                {
                    return;
                }
                if (CurNum < 10)
                {
                    CurNum = 1;
                    isTrueNum = false;
                }
                else
                {
                    int tempCurNum = CurNum / 10;
                    CurNum = tempCurNum;
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
                int tempCurNum = -1;
                if (isTrueNum == false)
                {
                    if (addNum != 0)
                    {
                        tempCurNum = addNum;
                        isTrueNum = true;
                    }
                    else
                    {
                        tempCurNum = 1;
                        isTrueNum = false;
                    }
                }
                else
                {
                    tempCurNum = CurNum * 10 + addNum;
                }
                if (tempCurNum < 1)
                {
                    Logger.LogErrorFormat("tempCurNum is error");
                }

                if (MaxNum < tempCurNum)
                {

                    tempCurNum = MaxNum;
                    SystemNotifyManager.SystemNotify(tipsID);
                }
                CurNum = tempCurNum;
            }
            ShowDataText();
            UpdateNumButtonState();
        }

        void _OnSliderValueChanged(float fValue)
        {
            int num = (int)((fValue / fBaseSliderValue) + 0.5f);

            if (num <= 1)
            {
                CurNum = 1;
            }
            else if (num >= MaxNum)
            {
                CurNum = MaxNum;
            }
            else
            {
                CurNum = num;
            }

            ShowDataText();
            UpdateNumButtonState();
        }

        [UIEventHandle("Bottom/ok")]
        void OnBuy()
        {
            if (mMallItemData != null)
            {
                if (!ActivityDataManager.GetInstance().IsShowFirstDiscountDes(mMallItemData.id))
                {
                    Buy();
                }
                else
                {
                    CommonMsgBoxOkCancelNewParamData paraData = new CommonMsgBoxOkCancelNewParamData()
                    {
                        ContentLabel = string.Format(TR.Value("DiscountActivity_BuyContent")),
                        IsShowNotify = false,
                        LeftButtonText = TR.Value("DiscountActivity_DiscountCancel"),
                        RightButtonText = TR.Value("DiscountActivity_DiscountOK"),
                        OnRightButtonClickCallBack = () => { Buy(); }
                    };
                    SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(paraData);
                }
            }

        }
        private void Buy()
        {
            if (CurNum <= 0)
            {
                if (MaxNum >= 1)
                    SystemNotifyManager.SysNotifyTextAnimation("请选择购买数量");
                else
                    SystemNotifyManager.SystemNotify(1086);
                return;
            }
            ItemTable itemTableData = null;
            if (mMallItemData != null)
            {
                itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>((int)mMallItemList[0].id);
                if (itemTableData == null)
                    Logger.LogErrorFormat("can not find ItemTableData when id is {0}", mMallItemList[0].id);
                if (itemTableData != null)
                {
                    if (itemTableData.GetLimitNum != 0)
                    {
                        if (itemTableData.GetLimitNum < ItemDataManager.GetInstance().GetItemCount((int)mMallItemList[0].id) + CurNum)
                        {
                            string name = itemTableData.Name;

                            object[] args = new object[1];
                            args[0] = name;
                            SystemNotifyManager.SystemNotify(9102, null, null, 0f, args);
                            return;
                        }
                    }
                }
            }

            if ((moneyType == ItemTable.eSubType.BindPOINT || moneyType == ItemTable.eSubType.POINT) && SecurityLockDataManager.GetInstance().CheckSecurityLock())
            {
                return;
            }


            if (mSetButtonCD == null || !mSetButtonCD.BtIsWork)
            {
                return;
            }
            mSetButtonCD.BtIsWork = false;
            CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();
            costInfo.nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(moneyType);
            costInfo.nCount = mTempPrice;
            if (mMallItemData != null)
            {
                //商城类型为飞升礼包
                if (mMallItemData.type == (int)MallTypeTable.eMallType.SN_ASCEND_GIFT)
                {
                    if (itemTableData != null)
                    {
                        //判断账号下是否符合购买道具
                        if (!ClientApplication.playerinfo.CheckRoleIsBuyGift(itemTableData.NeedLevel, itemTableData.MaxLevel))
                        {
                            SystemNotifyManager.SysNotifyMsgBoxCancelOk("账号下没有符合礼包使用条件的角色，是否继续购买?",
                                null,
                                () => { OnBuyItemClick(costInfo); });

                            return;
                        }
                    }
                }
            }
            else
            {
#if MG_TEST
                        Logger.LogErrorFormat("[MallBuyFrame OnBuy()] MallItemdata is Null");
#endif
            }

            OnBuyItemClick(costInfo);
        }
        /// <summary>
        /// 购买道具流程
        /// </summary>
        void OnBuyItemClick(CostItemManager.CostInfo costInfo)
        {
            CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
            {
                if (mMallItemData.multiple > 0)
                {
                    string content = string.Empty;

                    //积分商城积分等于上限值
                    if ((int)PlayerBaseData.GetInstance().IntergralMallTicket == MallNewUtility.GetIntergralMallTicketUpper() &&
                         MallNewDataManager.GetInstance().bItemMallIntergralMallScoreIsEqual == false)
                    {
                        content = TR.Value("mall_buy_intergral_mall_score_equal_upper_value_desc");

                        MallNewUtility.CommonIntergralMallPopupWindow(content, MallNewUtility.ItemMallIntergralMallScoreIsEqual, () => { OnSendWorldMallBuy(mMallItemData, CurNum); });
                    }
                    else
                    {
                        int ticketConvertScoreNumber = MallNewUtility.GetTicketConvertIntergalNumnber(mTempPrice) * mMallItemData.multiple;

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

                            MallNewUtility.CommonIntergralMallPopupWindow(content, MallNewUtility.ItemMallIntergralMallScoreIsExceed, () => { OnSendWorldMallBuy(mMallItemData, CurNum); });
                        }
                        else
                        {//未超出
                            OnSendWorldMallBuy(mMallItemData, CurNum);
                        }
                    }
                }
                else
                {
                    OnSendWorldMallBuy(mMallItemData, CurNum);
                }

            });
        }
        void OnSendWorldMallBuy(MallItemInfo MallItemdata, int CurNum)
        {
            if (needDiscountTips)
            {
                int tempNum = 0;
                int itemCount = ItemDataManager.GetInstance().GetOwnedItemCount((int)MallItemdata.discountCouponId);
                if (CurNum > itemCount)
                {
                    tempNum = itemCount;
                }
                else
                {
                    tempNum = CurNum;
                }
                SystemNotifyManager.SysNotifyMsgBoxCancelOk(string.Format(TR.Value("mallbuyFrame_discount_tips"), tempNum), null, () =>
                {
                    WorldMallBuy req = new WorldMallBuy();

                    req.itemId = MallItemdata.id;
                    req.num = (UInt16)CurNum;

                    NetManager netMgr = NetManager.Instance();
                    netMgr.SendCommand(ServerType.GATE_SERVER, req);
                    OnClose();
                });
            }
            else
            {
                WorldMallBuy req = new WorldMallBuy();

                req.itemId = MallItemdata.id;
                req.num = (UInt16)CurNum;

                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, req);
                OnClose();
            }
        }

        private void _InitView()
        {
            _InitComUIList();
            needDiscountTips = false;
            if (mFirstDiscountTxt != null)
                mOriginalColor = mFirstDiscountTxt.color;
        }

        void _InitData()
        {
            mHaveBuyLimit = mMallItemData.limit > 0;
            mMallItemList.Clear();
            if (mMallItemData.itemid != 0)
                mMallItemList.Add(new ItemReward() { id = mMallItemData.itemid, num = mMallItemData.itemnum });
            for (int i = 0; i < mMallItemData.giftItems.Length; i++)
            {
                mMallItemList.Add(mMallItemData.giftItems[i]);
            }
            mRewardScrollView.SetElementAmount(mMallItemList.Count);
            mGiftName.text = mMallItemData.giftName;
            bool bIsDailyLimit = false;
            limitnum = Utility.GetLeftLimitNum(mMallItemData, ref bIsDailyLimit);
            iPrice = Utility.GetMallRealPrice(mMallItemData);

            ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable((int)mMallItemList[0].id);
            ItemTable ItemTableData = TableManager.GetInstance().GetTableItem<ItemTable>((int)mMallItemList[0].id);
            if (ItemTableData == null)
                return;
            if (ItemDetailData.SubType == (int)ItemTable.eSubType.GOLD || ItemDetailData.SubType == (int)ItemTable.eSubType.BindGOLD)
            {
                mGiftName.text = Utility.GetShowPrice((UInt64)mMallItemData.itemnum, true) + PetDataManager.GetInstance().GetColorName(ItemTableData.Name, (PetTable.eQuality)ItemTableData.Color);
            }
            moneyType = (ItemTable.eSubType)mMallItemData.moneytype;

            var costItemTable = MallNewDataManager.GetInstance().GetCostItemTableByCostType(mMallItemData.moneytype);
            if (costItemTable != null)
            {
                if (ticketicon != null)
                {
                    ETCImageLoader.LoadSprite(ref ticketicon, costItemTable.Icon);
                    ETCImageLoader.LoadSprite(ref mTicketiconDiscount1, costItemTable.Icon);
                    ETCImageLoader.LoadSprite(ref mTicketiconDiscount2, costItemTable.Icon);
                }

                var ownerCostNumber = ItemDataManager.GetInstance().GetOwnedItemCount(costItemTable.ID);
                MaxNum = GetMaxNum();
            }
            tipsID = 9207;

            if (mMallItemData.type == (int)MallTypeTable.eMallType.SN_MALL_POINT_ITEM)
            {
                tipsID = 10053;
            }
            if (MaxNum == 0)
                MaxNum = 1;
            if (mHaveBuyLimit && MaxNum > limitnum)
            {
                MaxNum = limitnum;
                if (MaxNum == 0)
                    MaxNum = 1;
                tipsID = 9210;
            }
            //账号限购
            if (mMallItemData != null && mMallItemData.accountLimitBuyNum > 0)
            {
                if (MaxNum > mMallItemData.accountRestBuyNum)
                    MaxNum = (int)mMallItemData.accountRestBuyNum;
                if (MaxNum == 0)
                    MaxNum = 1;
            }
            //叠加数
            if (MaxNum > ItemTableData.MaxNum && ItemTableData.MaxNum != 0)
            {
                MaxNum = ItemTableData.MaxNum;
                if (MaxNum == 0)
                    MaxNum = 1;
                tipsID = 9208;
            }
            //最大可拥有数量
            if (ItemTableData.GetLimitNum != 0 && MaxNum > ItemTableData.GetLimitNum - ItemDataManager.GetInstance().GetItemCountInPackage(ItemTableData.ID))
            {
                MaxNum = ItemTableData.GetLimitNum - ItemDataManager.GetInstance().GetItemCountInPackage(ItemTableData.ID);
                if (MaxNum == 0)
                    MaxNum = 1;
                tipsID = 9209;
            }

            if (MaxNum < 1)
            {
                totalprice.color = new Color(1f, 0f, 0f);
            }
            CurNum = 1;

            btLeft.gameObject.AddComponent<UIGray>();
            btLeft.GetComponent<UIGray>().enabled = false;
            btRight.gameObject.AddComponent<UIGray>();
            btRight.GetComponent<UIGray>().enabled = false;
            btMax.gameObject.AddComponent<UIGray>();
            btMax.GetComponent<UIGray>().enabled = false;

            if (mMallItemData.itemid == 0)
            {
                des.text = mMallItemData.giftDesc;
            }
            else
            {
                des.text = ItemTableData.Description;

            }

            fBaseSliderValue = 1.0f / MaxNum;
            kSlider.onValueChanged.AddListener(_OnSliderValueChanged);
            kSlider.value = fBaseSliderValue * CurNum;

            if (mMallItemData != null && mMallItemData.multipleEndTime > 0)
            {
                _isUpdate = true;
            }

            InitIntergralInfoRoot();

            ShowDataText();
            UpdateNumButtonState();

            InitBuyNumberInfo(ItemDetailData);
        }
        private int GetDiscountPrice(int price, int discount)
        {
            return (int)Math.Floor(Convert.ToDecimal(price * discount * 1.0f / 100));
        }

        private int GetMayDayDiscountPrice(float price, float discount)
        {
            return (int)Math.Floor(Convert.ToDecimal(price * discount * 1.0f / 100));
        }

        private int GetMaxNum()
        {
            var costItemTable = MallNewDataManager.GetInstance().GetCostItemTableByCostType(mMallItemData.moneytype);
            var ownerCostNumber = ItemDataManager.GetInstance().GetOwnedItemCount(costItemTable.ID);

            //五一礼包标记并且全民团购活动开启
            if (mMallItemData.tagType == 2 && ActivityDataManager.GetInstance().CheckGroupPurchaseActivityIsOpen())
            {
                var voucherTicketData = TableManager.GetInstance().GetTableItem<ItemTable>((int)mMallItemData.deductionCouponId);
                var discountTicketData = TableManager.GetInstance().GetTableItem<ItemTable>((int)mMallItemData.discountCouponId);

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
                    int price = GetMayDayDiscountPrice(iPrice, ActivityDataManager.LimitTimeGroupBuyDiscount);

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
                if (!ActivityDataManager.GetInstance().IsShowFirstDiscountDes(mMallItemData.id))
                {
                    if (costItemTable != null)
                    {
                        var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>((int)mMallItemData.discountCouponId);
                        if (itemTableData != null && itemTableData.DiscountCouponProp != 0)
                        {
                            //折扣卷的数量
                            int itemCount = ItemDataManager.GetInstance().GetOwnedItemCount((int)mMallItemData.discountCouponId);
                            //折扣之后的价格
                            int tempDisPrice = GetDiscountPrice(iPrice, itemTableData.DiscountCouponProp);

                            int canBuyCount;
                            if (tempDisPrice != 0)
                            {
                                canBuyCount = ownerCostNumber / tempDisPrice;//全用打折券可以买多少个
                            }
                            else
                            {
                                Logger.LogErrorFormat("tempDisPrice = 0为分母 其中iPrice = {0},itemID = {1}", iPrice, itemTableData.ID);
                                canBuyCount = 1;
                            }

                            if (canBuyCount <= itemCount)
                            {
                                MaxNum = canBuyCount;
                            }
                            else
                            {
                                if (iPrice != 0)
                                {
                                    //打折的数量+不打折的数量
                                    MaxNum = itemCount + (ownerCostNumber - (tempDisPrice * itemCount)) / iPrice;
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
                            if (iPrice > 0)
                                MaxNum = ownerCostNumber / iPrice;
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
                        int tempDisPrice = GetDiscountPrice(iPrice, (int)activityData.parm);
                        if (ownerCostNumber >= tempDisPrice * 1)
                        {   //可以折扣+不能折扣的价格
                            MaxNum = 1 + (ownerCostNumber - tempDisPrice * 1) / iPrice;
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

        private void InitBuyNumberInfo(ItemData itemData)
        {
            if (itemData == null)
                return;

            mBuyNumberRoot.CustomActive(false);
            if (itemData.SubType == (int)ItemTable.eSubType.GOLD
                || itemData.SubType == (int)ItemTable.eSubType.BindGOLD)
            {
                if (mBuyNumberRoot != null)
                {
                    mBuyNumberRoot.CustomActive(true);
                }

                if (mBuyNumberText != null)
                {
                    mBuyNumberText.text = TR.Value("shop_new_buy_text");
                }

                if (itemData.SubType == (int)ItemTable.eSubType.GOLD)
                {
                    if (mBindGoldIcon != null)
                        mBindGoldIcon.gameObject.CustomActive(false);
                    if (mGoldIcon != null)
                        mGoldIcon.gameObject.CustomActive(true);
                }
                else
                {
                    if (mBindGoldIcon != null)
                        mBindGoldIcon.gameObject.CustomActive(true);
                    if (mGoldIcon != null)
                        mGoldIcon.gameObject.CustomActive(false);
                }

                UpdateBuyNumberValue();
            }
        }
        /// <summary>
        /// 更新打折商品的相关信息,是打折商品，且有打折券的时候，显示打折信息 
        /// </summary>
        private void UpdateElementDiscountInformation()
        {
            //五一礼包标记并且全民团购活动开启
            if (mMallItemData.tagType == 2 && ActivityDataManager.GetInstance().CheckGroupPurchaseActivityIsOpen())
            {
                mDiscountGO.CustomActive(true);
                mMayDayDisCountRoot.CustomActive(true);
                mNormalCostItem.CustomActive(false);

                //全民团购活动开启
                //if (ActivityDataManager.GetInstance().CheckGroupPurchaseActivityIsOpen())
                {
                    mDisCount.text = string.Format("{0}折", ActivityDataManager.LimitTimeGroupBuyDiscount / 10f);

                    var voucherTicketData = TableManager.GetInstance().GetTableItem<ItemTable>((int)mMallItemData.deductionCouponId);
                    var discountTicketData = TableManager.GetInstance().GetTableItem<ItemTable>((int)mMallItemData.discountCouponId);
                    int voucherTicketCount = 0;
                    if (voucherTicketData != null)
                    {
                        voucherTicketCount = ItemDataManager.GetInstance().GetOwnedItemCount(voucherTicketData.ID);

                        ETCImageLoader.LoadSprite(ref mUseVoucherTicketIcon, voucherTicketData.Icon);

                        if (voucherTicketCount > 0)
                        {
                            mUseVoucherTicketRoot.CustomActive(true);

                            if (voucherTicketCount >= CurNum)
                            {
                                mUseVoucherTickeCount.color = Color.white;
                                mUseVoucherTickeCount.text = string.Format("{0}/{1}", CurNum, voucherTicketCount);
                            }
                            else
                            {
                                mUseVoucherTickeCount.color = Color.red;
                                mUseVoucherTickeCount.text = string.Format("{0}/{1}", voucherTicketCount, voucherTicketCount);
                            }
                        }
                        else
                        {
                            mUseVoucherTicketRoot.CustomActive(false);
                        }
                    }
                    else
                    {
                        mUseVoucherTicketRoot.CustomActive(false);
                    }

                    int discountTicketCount = 0;
                    if (discountTicketData != null)
                    {
                        discountTicketCount = ItemDataManager.GetInstance().GetOwnedItemCount(discountTicketData.ID);

                        ETCImageLoader.LoadSprite(ref mUseDisCountTicketIcon, discountTicketData.Icon);

                        if (discountTicketCount > 0)
                        {
                            mUseDiscountTicketRoot.CustomActive(true);

                            if (discountTicketCount >= CurNum)
                            {
                                mUseDisCountTicketCount.color = Color.white;
                                mUseDisCountTicketCount.text = string.Format("{0}/{1}", CurNum, discountTicketCount);
                            }
                            else
                            {
                                mUseDisCountTicketCount.color = Color.red;
                                mUseDisCountTicketCount.text = string.Format("{0}/{1}", discountTicketCount, discountTicketCount);
                            }
                        }
                        else
                        {
                            mUseDiscountTicketRoot.CustomActive(false);
                        }
                    }
                    else
                    {
                        mUseDiscountTicketRoot.CustomActive(false);
                    }

                    mTempPrice = 0;

                    for (int i = 0; i < CurNum; i++)
                    {
                        //乘以最终折扣系数
                        int price = GetMayDayDiscountPrice(iPrice, ActivityDataManager.LimitTimeGroupBuyDiscount);

                        if (discountTicketData != null)
                        {  //计算折扣券
                            if (i < discountTicketCount)
                            {
                                price = GetMayDayDiscountPrice(price, discountTicketData.DiscountCouponProp);
                            }
                        }

                        if (voucherTicketData != null)
                        {
                            //计算抵扣券
                            if (i < voucherTicketCount)
                            {
                                price = price - voucherTicketData.DiscountCouponProp;
                            }
                        }

                        mTempPrice += price;
                    }

                    mCostnumDiscount.text = mTempPrice.ToString();
                }
            }
            else
            {
                var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>((int)mMallItemData.discountCouponId);
                if (!ActivityDataManager.GetInstance().IsShowFirstDiscountDes(mMallItemData.id))
                {
                    if (itemTableData != null && itemTableData.DiscountCouponProp != 0)
                    {
                        int itemCount = ItemDataManager.GetInstance().GetOwnedItemCount((int)mMallItemData.discountCouponId);
                        //显示打折卷数量，和折扣价格
                        if (itemCount > 0)
                        {
                            needDiscountTips = true;
                            mDiscountText.text = string.Format(TR.Value("mallbuyframe_new_discount_text"));
                            ETCImageLoader.LoadSprite(ref mDiscountIcon, itemTableData.Icon);
                            mDiscountGO.CustomActive(true);
                            mNormalCostItem.CustomActive(false);

                            mDiscountCount.CustomActive(true);
                            mDiscountText.CustomActive(true);
                            mDiscountIcon.CustomActive(true);
                            mFirstDiscountTxt.CustomActive(false);


                            var giftPrice = Utility.GetMallRealPrice(mMallItemData);

                            int tempDisPrice = GetDiscountPrice(iPrice, itemTableData.DiscountCouponProp);
                            if (itemCount >= CurNum)
                            {
                                //所有的都打折
                                mDiscountCount.color = Color.white;
                                mDiscountCount.text = string.Format("{0}/{1}", CurNum, itemCount);
                                mTempPrice = (int)(tempDisPrice * CurNum);
                                mCostnumDiscount.text = mTempPrice.ToString();
                            }
                            else
                            {
                                //打折券不够，部分打折
                                mDiscountCount.color = Color.red;
                                mDiscountCount.text = string.Format("{0}/{1}", itemCount, itemCount);
                                mTempPrice = (int)(tempDisPrice * itemCount + iPrice * (CurNum - itemCount));
                                mCostnumDiscount.text = mTempPrice.ToString();
                            }
                        }
                        else
                        {
                            needDiscountTips = false;
                            mNormalCostItem.CustomActive(true);
                            mDiscountGO.CustomActive(false);
                        }
                    }
                    else
                    {
                        needDiscountTips = false;
                        mNormalCostItem.CustomActive(true);
                        mDiscountGO.CustomActive(false);
                    }
                }
                else
                {
                    //显示打折的价格 
                    mNormalCostItem.CustomActive(false);
                    mDiscountGO.CustomActive(true);

                    //隐藏打折卷的打折信息
                    mDiscountCount.CustomActive(false);
                    mDiscountText.CustomActive(false);
                    mDiscountIcon.CustomActive(false);
                    mFirstDiscountTxt.CustomActive(true);

                    var activityData = ActivityDataManager.GetInstance().GetActiveDataFromType(ActivityLimitTimeFactory.EActivityType.OAT_LIMITTIMEFIRSTDISCOUNTACTIIVITY);

                    if (activityData != null)
                    {
                        mFirstDiscountTxt.SafeSetText(string.Format(TR.Value("DiscountActivity_DiscountTip", activityData.parm * 1.0f / 10)));
                        ETCImageLoader.LoadSprite(ref mDiscountIcon, itemTableData.Icon);
                        int tempDisPrice = GetDiscountPrice(iPrice, (int)activityData.parm);

                        //买多个就这商品的时候，就一个打折，其他的都是原价
                        mTempPrice = (int)(tempDisPrice * 1 + iPrice * (CurNum - 1));
                        mCostnumDiscount.text = mTempPrice.ToString();
                        if (CurNum > 1)
                        {
                            mFirstDiscountTxt.color = Color.red;
                        }
                        else
                        {
                            mFirstDiscountTxt.color = mOriginalColor;

                        }
                    }
                }
            }
        }
        private void UpdateBuyNumberValue()
        {
            if (mMallItemData == null)
                return;

            if (mBuyNumberValue == null)
                return;

            if (mBuyNumberValue.gameObject.activeSelf == false)
                return;

            //GetShowPrice
            var buyNumber = (ulong)CurNum * (ulong)mMallItemData.itemnum;
            var buyNumberValueStr = GetNumberStr(buyNumber, true);

            mBuyNumberValue.text = buyNumberValueStr;
        }

        void _InitComUIList()
        {
            mRewardScrollView.Initialize();
            mRewardScrollView.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0)
                {
                    _UpdateRewardBind(item);
                }
            };
            // mRewardScrollView.OnItemRecycle = (item) =>
            // {
            //     ComCommonBind combind = item.GetComponent<ComCommonBind>();
            //     if (combind == null)
            //     {
            //         return;
            //     }
            // };
        }

        void _UpdateRewardBind(ComUIListElementScript item)
        {
            ComCommonBind mBind = item.GetComponent<ComCommonBind>();
            if (mBind == null || item.m_index < 0)
            {
                return;
            }
            var mRewardRoot = mBind.GetGameObject("rewardRoot");
            ComItem comitem = mRewardRoot.GetComponentInChildren<ComItem>();
            if (comitem == null)
                comitem = CreateComItem(mRewardRoot);
            ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable((int)mMallItemList[item.m_index].id);
            if (null == ItemDetailData)
            {
                return;
            }
            ItemDetailData.Count = (int)mMallItemList[item.m_index].num;
            comitem.Setup(ItemDetailData, (GameObject Obj, ItemData sitem) =>
            {
                if (ItemDetailData != null)
                    ItemTipManager.GetInstance().ShowTip(ItemDetailData);
            });
        }

        void UpdateNumButtonState()
        {
            if (MaxNum < 1)
            {
                btLeft.GetComponent<UIGray>().enabled = true;
                btLeft.interactable = false;

                btRight.GetComponent<UIGray>().enabled = true;
                btRight.interactable = false;

                btMax.GetComponent<UIGray>().enabled = true;
                btMax.interactable = false;
            }
            else
            {
                if ((mHaveBuyLimit && CurNum >= limitnum) || CurNum >= MaxNum)
                {
                    btRight.GetComponent<UIGray>().enabled = true;
                    btRight.interactable = false;
                }
                else
                {
                    btRight.GetComponent<UIGray>().enabled = false;
                    btRight.interactable = true;
                }

                if (CurNum <= 1)
                {
                    btLeft.GetComponent<UIGray>().enabled = true;
                    btLeft.interactable = false;
                }
                else
                {
                    btLeft.GetComponent<UIGray>().enabled = false;
                    btLeft.interactable = true;
                }

                btMax.GetComponent<UIGray>().enabled = false;
                btMax.interactable = true;
            }
        }

        void ShowDataText()
        {
            mTempPrice = CurNum * iPrice;
            buynum.text = CurNum.ToString();
            totalprice.text = (CurNum * iPrice).ToString();
            mCostnumNormal.text = (CurNum * iPrice).ToString();
            UpdateBuyNumberValue();
            UpdateElementDiscountInformation();
            UpdataIntergralInfo();
        }

        /// <summary>
        /// 初始化积分信息是否显示
        /// </summary>
        private void InitIntergralInfoRoot()
        {
            if (mMallItemData == null)
            {
                return;
            }

            mIntergralMallInfoRoot.CustomActive(mMallItemData.multiple > 0);
        }

        /// <summary>
        /// 更新积分信息
        /// </summary>
        private void UpdataIntergralInfo()
        {
            if (mMallItemData == null)
            {
                return;
            }

            int price = MallNewUtility.GetTicketConvertIntergalNumnber(mTempPrice) * mMallItemData.multiple;
            string mContent = string.Empty;
            if (mMallItemData.multiple <= 1)
            {
                mContent = TR.Value("mall_buy_intergral_single_multiple_desc", price);
            }
            else
            {
                mContent = TR.Value("mall_buy_intergral_many_multiple_desc", price, mMallItemData.multiple);
            }

            mIntergralInfoText.text = mContent;
        }

        [UIControl("Middle/slider")]
        Slider kSlider;

        [UIControl("Middle/count/Text")]
        protected Text buynum;

        [UIControl("CostItem/item/costnum")]
        protected Text totalprice;

        [UIControl("Middle/minus")]
        protected Button btLeft;

        [UIControl("Middle/add")]
        protected Button btRight;

        [UIControl("Middle/max")]
        protected Button btMax;

        [UIControl("Middle/ScrollView/Viewport/Content/Desc")]
        protected Text des;

        [UIControl("CostItem/item/ticketicon")]
        protected Image ticketicon;

        #region ExtraUIBind
        private Button mOpenKeyBoard = null;
        private SetButtonCD mSetButtonCD = null;
        //商品名称
        private Text mGiftName = null;
        private ComUIListScript mRewardScrollView = null;

        private GameObject mBuyNumberRoot = null;
        private Text mBuyNumberText = null;
        private GameObject mBuyItemRoot = null;
        private Text mBuyNumberValue = null;
        private Image mBindGoldIcon = null;
        private Image mGoldIcon = null;
        private GameObject mDiscountGO = null;
        private Text mDiscountText = null;
        private Image mDiscountIcon = null;
        private Text mDiscountCount = null;
        private GameObject mNormalCostItem = null;
        //消耗道具图标
        private Image mTicketiconDiscount1 = null;
        //消耗道具图标
        private Image mTicketiconDiscount2 = null;
        private Text mCostnumNormal = null;
        private Text mCostnumDiscount = null;
        private GameObject mIntergralMallInfoRoot = null;
        private Text mIntergralInfoText = null;
        //首次购买折扣文本
        private Text mFirstDiscountTxt;
        private Text mUseDisCountTicketCount = null;
        private Image mUseDisCountTicketIcon = null;
        private Text mUseVoucherTickeCount = null;
        private Image mUseVoucherTicketIcon = null;
        private Text mDisCount = null;
        private GameObject mMayDayDisCountRoot = null;
        private GameObject mUseVoucherTicketRoot = null;
        private GameObject mUseDiscountTicketRoot = null;

        protected override void _bindExUI()
        {
            mOpenKeyBoard = mBind.GetCom<Button>("OpenKeyBoard");
            if (null != mOpenKeyBoard)
            {
                mOpenKeyBoard.onClick.AddListener(_onOpenKeyBoardButtonClick);
            }
            mSetButtonCD = mBind.GetCom<SetButtonCD>("SetButtonCD");
            mGiftName = mBind.GetCom<Text>("GiftName");
            mRewardScrollView = mBind.GetCom<ComUIListScript>("RewardScrollView");

            mBuyNumberRoot = mBind.GetGameObject("buyNumberRoot");
            mBuyNumberText = mBind.GetCom<Text>("buyNumberText");
            mBuyItemRoot = mBind.GetGameObject("buyItemRoot");
            mBuyNumberValue = mBind.GetCom<Text>("buyNumberValue");
            mGoldIcon = mBind.GetCom<Image>("ticketicon");
            mDiscountGO = mBind.GetGameObject("discountGO");
            mDiscountText = mBind.GetCom<Text>("discountText");
            mDiscountIcon = mBind.GetCom<Image>("discountIcon");
            mDiscountCount = mBind.GetCom<Text>("discountCount");
            mNormalCostItem = mBind.GetGameObject("NormalCostItem");
            mTicketiconDiscount1 = mBind.GetCom<Image>("ticketiconDiscount1");
            mTicketiconDiscount2 = mBind.GetCom<Image>("ticketiconDiscount2");
            mCostnumNormal = mBind.GetCom<Text>("costnumNormal");
            mCostnumDiscount = mBind.GetCom<Text>("costnumDiscount");
            mIntergralMallInfoRoot = mBind.GetGameObject("IntergralMallInfoRoot");
            mIntergralInfoText = mBind.GetCom<Text>("IntergralInfoText");
            mFirstDiscountTxt = mBind.GetCom<Text>("FirstDiscountTxt");
            mUseDisCountTicketCount = mBind.GetCom<Text>("UseDisCountTicketCount");
            mUseDisCountTicketIcon = mBind.GetCom<Image>("UseDisCountTicketIcon");
            mUseVoucherTickeCount = mBind.GetCom<Text>("UseVoucherTickeCount");
            mUseVoucherTicketIcon = mBind.GetCom<Image>("UseVoucherTicketIcon");
            mDisCount = mBind.GetCom<Text>("DisCount");
            mMayDayDisCountRoot = mBind.GetGameObject("MayDayDisCountRoot");
            mUseVoucherTicketRoot = mBind.GetGameObject("UseVoucherTicketRoot");
            mUseDiscountTicketRoot = mBind.GetGameObject("UseDiscountTicketRoot");
        }

        protected override void _unbindExUI()
        {
            if (null != mOpenKeyBoard)
            {
                mOpenKeyBoard.onClick.RemoveListener(_onOpenKeyBoardButtonClick);
            }
            mOpenKeyBoard = null;
            mSetButtonCD = null;
            mGiftName = null;
            mRewardScrollView = null;

            mBuyNumberRoot = null;
            mBuyNumberText = null;
            mBuyItemRoot = null;
            mBuyNumberValue = null;
            mBindGoldIcon = null;
            mGoldIcon = null;
            mDiscountGO = null;
            mDiscountText = null;
            mDiscountIcon = null;
            mDiscountCount = null;
            mNormalCostItem = null;
            mTicketiconDiscount1 = null;
            mTicketiconDiscount2 = null;
            mCostnumNormal = null;
            mCostnumDiscount = null;
            mIntergralMallInfoRoot = null;
            mIntergralInfoText = null;
            mFirstDiscountTxt = null;
            mUseDisCountTicketCount = null;
            mUseDisCountTicketIcon = null;
            mUseVoucherTickeCount = null;
            mUseVoucherTicketIcon = null;
            mDisCount = null;
            mMayDayDisCountRoot = null;
            mUseVoucherTicketRoot = null;
            mUseDiscountTicketRoot = null;
        }
        #endregion

        #region Callback
        private void _onOpenKeyBoardButtonClick()
        {
            /* put your code in here */
            ClientSystemManager.GetInstance().OpenFrame<VirtualKeyboardFrame>();
        }
        #endregion

        //购买数量的格式
        public static string GetNumberStr(UInt64 uPrice, bool bUseToMillion = false)
        {
            if (uPrice < 10000)
            {
                return uPrice.ToString();
            }
            else if (uPrice >= 10000 && uPrice < 100000000)
            {
                if (bUseToMillion)
                {
                    return string.Format("{0}万", uPrice / 10000.0f);
                }
                else
                {
                    return uPrice.ToString();
                }
            }
            else
            {
                float finalPrice = uPrice / 100000000.0f;
                string sfinalPrice = finalPrice.ToString("F3");

                return string.Format("{0}亿", sfinalPrice);
            }
        }

        private void ReqQueryMallItemInfo(UIEvent uiEvent)
        {
            //是否为同一个商品
            if (mMallItemData == null)
                return;

            var itemId = (UInt32)uiEvent.Param1;

            if (mMallItemData.itemid != itemId)
                return;

            MallNewDataManager.GetInstance().ReqQueryMallItemInfo((int)mMallItemData.itemid);
        }

        private void OnQueryMallItenInfoSuccess(UIEvent uiEvent)
        {
            mMallItemData = MallNewDataManager.GetInstance().QueryMallItemInfo;
            InitIntergralInfoRoot();
            UpdataIntergralInfo();
        }


        private void _OnCountValueChanged(UIEvent uiEvent)
        {
            //已经购买过9折礼包
            if (AccountShopDataManager.GetInstance().GetAccountSpecialItemNum(AccountCounterType.ACC_NEW_SERVER_GIFT_DISCOUNT) > 0)
            {
                Recover();
            }
        }
        private void _OnLimitTimeDataUpdate(UIEvent uiEvent)
        {
            uint activityId = (uint)uiEvent.Param1;
            var activityData = ActivityDataManager.GetInstance().GetActiveDataFromType(ActivityLimitTimeFactory.EActivityType.OAT_LIMITTIMEFIRSTDISCOUNTACTIIVITY);
            if (activityData != null)
            {
                if (activityData.dataId == activityId)
                {
                    if (activityData.state == (int)ActivityState.End)
                    {
                        Recover();
                    }
                }
            }
        }

        private void Recover()
        {
            if (mMallItemData != null)
            {
                MaxNum = GetMaxNum();
                ShowDataText();
                UpdateNumButtonState();
            }
        }

    }
}
