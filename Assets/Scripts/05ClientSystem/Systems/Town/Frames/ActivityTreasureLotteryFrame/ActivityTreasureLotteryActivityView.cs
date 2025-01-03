using DataModel;
using Protocol;
using ProtoTable;
using Scripts.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    namespace ActivityTreasureLottery
    {
        /// <summary>
        /// 夺宝活动模板
        /// </summary>
        public sealed class ActivityTreasureLotteryActivityView : ActivityTreasureLotteryActivityViewBase<IActivityTreasureLotteryModel>
        {
            public uint BuyCount { get { return mBuyCount; } }

            public uint LeftCount
            {
                get { return this.mModel == null ? 0 : this.mModel.LeftNum; }
            }

            public UnityAction OnButtonBuyAllCallBack { set; private get; }
            
            public UnityAction OnButtonBuyCallBack
            {
                set
                {
                    if (value == null)
                    {
                        mButtonBuy.SafeRemoveOnClickListener(mOnButtonBuyCallBack);
                    }
                    else
                    {
                        mOnButtonBuyCallBack = value;
                        mButtonBuy.onClick.AddListener(value);
                    }
                }
            }

            private UnityAction mOnButtonBuyCallBack;

            #region serialize field

            [SerializeField]
            private Button mButtonBuy = null;

            [SerializeField]
            private Slider mSliderProgressValue = null;

            [SerializeField]
            private Button mButtonBuyCurrency = null;

            [SerializeField]
            private Text mTextOwnedNum = null;

            [SerializeField]
            private Image mImageTotalCurrency = null;

            [SerializeField]
            private Image mImageOwnCurrency = null;

            [SerializeField]
            private Text mTextTotalPrice = null;

            [SerializeField]
            private Image mImageUnitCurrency = null;

            [SerializeField]
            private Text mTextUnitPrice = null;

            [SerializeField]
            private Transform mComItemRoot;

            //[SerializeField]
            //private Button mButtonMax = null;

            [SerializeField]
            private Button mButtonAdd = null;

            [SerializeField]
            private Button mButtonMinus = null;

            [SerializeField]
            private Text mTextItemName = null;

            [SerializeField]
            private Text mTextInfo = null;

            [SerializeField]
            private Text mTextSelling = null;

            [SerializeField]
            private Text mTextBought = null;

            [SerializeField]
            private Text mTextBuyCount = null;

            [SerializeField]
            private Text mTextLeftNum = null;

            [SerializeField]
            private Button mButtonCountInput = null;

            [SerializeField]
            private Button mButtonBuyAll = null;

            [SerializeField]
            private Toggle mToggelBuyAll = null;

            [SerializeField]
            float mKeyBoardOffetX = -150;

            [SerializeField]
            private Text mTextTip = null;

            [SerializeField] private Text mTextLoadingData;
            [SerializeField] private GameObject mRightPanel;
            [Header("是否切换物品后清空默认勾选购买剩余")]
            [SerializeField]
            private bool mIsClearTollgeAll = true;
            #endregion

            uint mBuyCount = 1;
            //判断是否打开输入框界面后第一次输入，如果是 则会清除掉当前的数字
            bool mIsFirstNum = false;
            ComItem mComItem = null;
            IActivityTreasureLotteryModel mModel = null;
            SetButtonCD mButtonBuyCd = null;
            SetButtonCD mButtonBuyAllCd = null;
            UIGray mButtonBuyGray = null;
            UIGray mButtonBuyAllGray = null;

            protected sealed override void OnSelectItem(IActivityTreasureLotteryModel model)
            {
                mModel = model;
                InitCommon();
                switch (model.State)
                {
                    case GambingItemStatus.GIS_PREPARE:
                        InitPrepare();
                        break;
                    case GambingItemStatus.GIS_SELLING:
                        InitOpen();
                        break;
                    case GambingItemStatus.GIS_SOLD_OUE:
                    case GambingItemStatus.GIS_LOTTERY:
                        InitClose();
                        break;
                }
                //mTextOwnedNum.SafeSetText(PlayerBaseData.GetInstance().


                UpdateMoneyBinder();
                UpdateCostData();
                UpdateNumButtonState();
            }

            public sealed override void UpdateData()
            {
                base.UpdateData();
                CheckData();
            }

            protected sealed override void OnInit()
            {
                base.OnInit();
                CheckData();
            }

            void CheckData()
            {
                if (mDataManager == null || !mDataManager.IsHadData())
                {
                    this.mRightPanel.CustomActive(false);
                    this.mTextLoadingData.CustomActive(true);
                }
                else
                {
                    this.mRightPanel.CustomActive(true);
                    this.mTextLoadingData.CustomActive(false);
                }
            }

            void InitCommon()
            {
                if (mButtonMinus != null && mButtonMinus.GetComponent<UIGray>() != null)
                {
                    mButtonMinus.GetComponent<UIGray>().enabled = false;
                }

                if (mButtonAdd != null && mButtonAdd.GetComponent<UIGray>() != null)
                {
                    mButtonAdd.GetComponent<UIGray>().enabled = false;
                }

                if (mButtonBuyCd == null)
                {
                    mButtonBuyCd = mButtonBuy.GetComponent<SetButtonCD>();
                }

                if (mButtonBuyAllCd == null)
                {
                    mButtonBuyAllCd = mButtonBuyAll.GetComponent<SetButtonCD>();
                }

                if (mButtonBuyGray == null)
                {
                    mButtonBuyGray = mButtonBuy.GetComponent<UIGray>();
                }

                if (mButtonBuyAllGray == null)
                {
                    mButtonBuyAllGray = mButtonBuyAll.GetComponent<UIGray>();
                }
                //mButtonMax.GetComponent<UIGray>().enabled = false;
                ItemTable itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(mModel.ItemId);

                if (mComItem == null)
                {
                    mComItem = ComItemManager.Create(mComItemRoot.gameObject);
                }

                var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mModel.ItemId);
                mComItem.Setup(itemData, ShowItemTip);
                if (itemTableData != null)
                {
                    var qualityInfo = ItemData.GetQualityInfo(itemTableData.Color);
                    if (qualityInfo != null)
                    {
                        mTextItemName.SafeSetText(string.Format("<color={0}>{1}*{2}</color>", qualityInfo.ColStr, itemTableData.Name, mModel.ItemNum));
                    }
                    else
                    {
                        mTextItemName.SafeSetText(string.Format("{1}*{2}", qualityInfo.ColStr, itemTableData.Name, mModel.ItemNum));
                    }
                }

                if (mModel != null)
                {
                    mTextUnitPrice.SafeSetText(mModel.UnitPrice.ToString());
                    mSliderProgressValue.value = mModel.TotalNum == 0 ? 0 : (float)mModel.LeftNum / mModel.TotalNum;
                }
                mTextTip.SafeSetText(TR.Value("activity_treasure_lottery_activity_tip"));
            }

            protected override void OnDispose()
            {
                base.OnDispose();
                ComItemManager.Destroy(this.mComItem);
                this.mComItem = null;
            }

            void ShowItemTip(GameObject go, ItemData itemData)
            {
                if (null != itemData)
                {
                    ItemTipManager.GetInstance().ShowTip(itemData);
                }
            }

            void InitPrepare()
            {
	            mTextBought.gameObject.CustomActive(true);
                if (mModel != null)
                {
                    mTextBought.SafeSetText(string.Format(TR.Value("activity_treasure_lottery_item_prepare"), mModel.TimeRemainStr, mModel.LeftGroupNum, mModel.TotalGroup));
                    mTextSelling.SafeSetText(string.Empty);
                    mTextLeftNum.SafeSetText(string.Format("{0}/{1}", mModel.LeftNum, mModel.TotalNum));
                    //补偿信息
                    string compensation = "";
                    if (mModel.Compensation != null)
                    {

                        for (int i = 0; i < mModel.Compensation.Length; ++i)
                        {
                            ItemTable itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>((int)mModel.Compensation[i].id);
                            var qualityInfo = ItemData.GetQualityInfo(itemTableData.Color);
                            compensation += string.Format("<color={0}>{1}*{2}</color>", qualityInfo.ColStr, itemTableData.Name, mModel.Compensation[i].num);
                        }

                        mTextInfo.CustomActive(true);
                        if (mModel.TotalNum != 1)
                        {
                            mTextInfo.SafeSetText(string.Format(TR.Value("activity_treasure_lottery_compensation"), compensation));
                        }
                        else
                        {
                            mTextInfo.SafeSetText(string.Format(TR.Value("activity_treasure_lottery_compensation_mustReward"), compensation));

                        }
                    }
                    else
                    {
                        mTextInfo.CustomActive(false);
                    }

                }
                SetBuyOperateInteractable(false);
                SetGraySelectableInteractable(this.mButtonBuyAll, this.mToggelBuyAll.isOn);
                if (!mToggelBuyAll.isOn)
                    this.mButtonBuyAll.interactable = true;
            }

            void InitOpen()
            {
                mTextBought.gameObject.CustomActive(true);
                //补偿信息
                string compensation = "";

                mTextSelling.SafeSetText(string.Format(TR.Value("activity_treasure_lottery_selling_num"), mModel.GroupId, mModel.TotalGroup));
                mTextBought.SafeSetText(string.Format(TR.Value("activity_treasure_lottery_bought_num"), mModel.BoughtNum));
                mTextLeftNum.SafeSetText(string.Format("{0}/{1}", mModel.LeftNum, mModel.TotalNum));
                SetBuyOperateInteractable(true);
                SetGraySelectableInteractable(mButtonBuyAll, this.mToggelBuyAll.isOn);
                if (!this.mToggelBuyAll.isOn)
                {
                    this.mButtonBuyAll.interactable = true;
                }
                if(mModel.Compensation!=null)//配置了补偿奖励
                {
                    for (int i = 0; i < mModel.Compensation.Length; ++i)
                    {
                        ItemTable itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>((int)mModel.Compensation[i].id);
                        var qualityInfo = ItemData.GetQualityInfo(itemTableData.Color);
                        compensation += string.Format("<color={0}>{1}*{2}</color>", qualityInfo.ColStr, itemTableData.Name, mModel.Compensation[i].num);
                    }
                    mTextInfo.CustomActive(true);
                    if (mModel.TotalNum != 1)
                    {
                        mTextInfo.SafeSetText(string.Format(TR.Value("activity_treasure_lottery_compensation"), compensation));
                    }
                    else
                    {
                        mTextInfo.SafeSetText(string.Format(TR.Value("activity_treasure_lottery_compensation_mustReward"), compensation));
                        
                    }
                   
                }
                else//没有配置补偿奖励
                {
                    mTextInfo.CustomActive(false);
                }
                
            }

            void InitClose()
            {
                mTextBought.gameObject.CustomActive(false);
                mTextInfo.SafeSetText(string.Format(TR.Value("activity_treasure_lottery_sell_out"), mModel.TotalGroup));
                mTextSelling.SafeSetText(TR.Value("activity_treasure_lottery_end"));
                mTextLeftNum.SafeSetText(string.Format("{0}/{1}", mModel.LeftNum, mModel.TotalNum));
                SetBuyOperateInteractable(false);
                SetGraySelectableInteractable(mButtonBuyAll, false);
                if (!this.mToggelBuyAll.isOn)
                {
                    this.mButtonBuyAll.interactable = true;
                }
            }

            void SetBuyOperateInteractable(bool enable)
            {
                SetGraySelectableInteractable(mButtonMinus, enable);
                SetGraySelectableInteractable(mButtonAdd, enable);
                //SetGraySelectableInteractable(mButtonMax, enable);
                SetGraySelectableInteractable(mButtonCountInput, enable);
            }

            void SetGraySelectableInteractable(Selectable element, bool enable)
            {
                if (element != null)
                {
                    element.interactable = enable;
                    var gray = element.GetComponent<UIGray>();
                    if (gray != null)
                    {
                        gray.enabled = !enable;
                    }
                }
            }

            void Update()
            {
                if (mModel != null && mModel.State == GambingItemStatus.GIS_PREPARE)
                {
                    //mTextSelling.SafeSetText(string.Empty);
                    mTextBought.SafeSetText(string.Format(TR.Value("activity_treasure_lottery_item_prepare"), mModel.TimeRemainStr, mModel.LeftGroupNum, mModel.TotalGroup));
                }

                if (mButtonBuyCd != null && mButtonBuy != null)
                {
                    if (!this.mToggelBuyAll.isOn)
                    { 
                        if (mButtonBuyCd.BtIsWork && !mButtonBuy.interactable)
                        {
                            mButtonBuy.interactable = true;
                            mButtonBuyGray.enabled = false;
                        }
                        else if (!mButtonBuyCd.BtIsWork && mButtonBuy.interactable)
                        {
                            mButtonBuy.interactable = false;
                            mButtonBuyGray.enabled = true;
                        }
                    }
                }

                if (mButtonBuyAllCd != null && mButtonBuyAll != null)
                {
                    if (this.mToggelBuyAll.isOn)
                    {
                        if (mButtonBuyAllCd.BtIsWork && !mButtonBuyAll.interactable)
                        {
                            mButtonBuyAll.interactable = true;
                            mButtonBuyAllGray.enabled = false;
                        }
                        else if (!mButtonBuyAllCd.BtIsWork && mButtonBuyAll.interactable)
                        {
                            mButtonBuyAll.interactable = false;
                            mButtonBuyAllGray.enabled = true;
                        }
                    }
                }
            }

            void OnChangeNum(UIEvent uiEvent)
            {
                ChangeNumType changeNumType = (ChangeNumType)uiEvent.Param1;
                if (changeNumType == ChangeNumType.BackSpace)
                {
                    if (mBuyCount < 10)
                    {
                        mBuyCount = 1;
	                    mIsFirstNum = true;
                    }
                    else
                    {
                        uint temp_buyCount = mBuyCount / 10;
                        mBuyCount = temp_buyCount;
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
                    int temp_buyCount = 0;
                    if (mIsFirstNum)
                    {
                        if (addNum != 0)
                        {
                            temp_buyCount = addNum;
                            mIsFirstNum = false;
                        }
                        else
                        {
                            temp_buyCount = 1;
                        }
                    }
                    else
                    {
                        temp_buyCount = (int)mBuyCount * 10 + addNum;
                    }
                    if (temp_buyCount < 1)
                    {
                        Logger.LogErrorFormat("temp_buyCount is error");
                    }

                    if (mModel.LeftNum < temp_buyCount)
                    {
                        temp_buyCount = (int)mModel.LeftNum;
                    }
                    mBuyCount = (uint)temp_buyCount;
                }
                UpdateCostData();
                UpdateNumButtonState();
            }

            void UpdateMoneyBinder()
            {
                MoneyBinder.Create(mTextOwnedNum.gameObject, null, mTextOwnedNum, null, mModel.MoneyId, MoneyBinder.MoneyShowType.MST_NORMAL);
                var money = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(mModel.MoneyId);
                // image.sprite = AssetLoader.instance.LoadRes(item.Icon, typeof(Sprite)).obj as Sprite;
                if (money != null)
                {
                    ETCImageLoader.LoadSprite(ref mImageTotalCurrency, money.Icon);
                    ETCImageLoader.LoadSprite(ref mImageUnitCurrency, money.Icon);
                    ETCImageLoader.LoadSprite(ref mImageOwnCurrency, money.Icon);
                }
            }

            void UpdateCostData()
            {
                mTextBuyCount.SafeSetText(mBuyCount.ToString());
                if (mModel != null)
                {
                    mTextTotalPrice.SafeSetText((mBuyCount * mModel.UnitPrice).ToString());
                }
            }
            void UpdateNumButtonState()
            {
                if (mModel.State != GambingItemStatus.GIS_SELLING || mModel.LeftNum < 1 || mToggelBuyAll.isOn)
                {
                    SetBuyOperateInteractable(false);
                }
                else
                {
                    SetBuyOperateInteractable(true);
                    if (mBuyCount >= mModel.LeftNum)
                    {
                        SetGraySelectableInteractable(mButtonAdd, false);
                    }

                    if (mBuyCount <= 1)
                    {
                        SetGraySelectableInteractable(mButtonMinus, false);
                    }
                }
            }

            protected sealed override void BindEvents()
            {
                base.BindEvents();
                mButtonBuyCurrency.SafeAddOnClickListener(OnButtonBuyCurrencyButtonClick);
                mButtonAdd.SafeAddOnClickListener(OnButtonAddButtonClick);
                //mButtonMax.onClick.AddListener(OnButtonMaxButtonClick);
                mButtonMinus.SafeAddOnClickListener(OnButtonMinusButtonClick);
                mButtonCountInput.SafeAddOnClickListener(OnButtonCountInputButtonClick);
                mButtonBuyAll.SafeAddOnClickListener(OnButtonBuyAllClick);
                mToggelBuyAll.SafeAddOnValueChangedListener(OnToggleBuyAll);
                mButtonBuy.SafeAddOnClickListener(OnButtonBuyClick);
                UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChangeNum, OnChangeNum);
            }

            protected sealed override void UnBindEvents()
            {
                base.UnBindEvents();
                mButtonBuyCurrency.SafeRemoveOnClickListener(OnButtonBuyCurrencyButtonClick);
                //mButtonMax.SafeRemoveOnClickListener(OnButtonMaxButtonClick);
                mButtonAdd.SafeRemoveOnClickListener(OnButtonAddButtonClick);
                mButtonMinus.SafeRemoveOnClickListener(OnButtonMinusButtonClick);
                mButtonCountInput.SafeRemoveOnClickListener(OnButtonCountInputButtonClick);
                mButtonBuyAll.SafeRemoveOnClickListener(OnButtonBuyAllClick);
                mToggelBuyAll.SafeRemoveOnValueChangedListener(OnToggleBuyAll);
                mButtonBuy.SafeRemoveOnClickListener(OnButtonBuyClick);
                UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ChangeNum, OnChangeNum);
            }

            #region Callback

            protected sealed override void OnItemSelected(ComUIListElementScript item)
            {
                if (mSelectedItem != null && mSelectId != item.m_index)
                {
                    mToggelBuyAll.isOn = false;
                    mBuyCount = 1;
                    mTextBuyCount.SafeSetText("1");
                    UpdateCostData();
                }
                base.OnItemSelected(item);
            }

            private void OnToggleBuyAll(bool value)
            {
                if (value)
                {
                    SystemNotifyManager.SystemNotify(9862);
                    SetGraySelectableInteractable(mButtonBuyAll, true);
                    SetGraySelectableInteractable(mButtonBuy, false);

                    mBuyCount = mModel.LeftNum;
                }
                else
                {
                    SetGraySelectableInteractable(mButtonBuy, true);
                    SetGraySelectableInteractable(mButtonBuyAll, false);
                    this.mButtonBuyAll.interactable = true;

                    mBuyCount = 1;
                }

                mTextBuyCount.SafeSetText("1");
                UpdateCostData();

                UpdateNumButtonState();
            }

            private void OnButtonBuyClick()
            {
                if (mButtonBuyCd != null)
                {
                    mButtonBuyCd.BtIsWork = false;
                }
            }

            private void OnButtonBuyAllClick()
            {
                if (this.mToggelBuyAll.isOn)
                {
                    if (mButtonBuyAllCd != null)
                    {
                        mButtonBuyAllCd.BtIsWork = false;
                    }

                    if (OnButtonBuyAllCallBack != null)
                    {
                        OnButtonBuyAllCallBack();
                    }
                }
                else
                {
                    SystemNotifyManager.SystemNotify(9861);
                }
            }

            private void OnButtonCountInputButtonClick()
            {
                ClientSystemManager.GetInstance().OpenFrame<VirtualKeyboardFrame>(FrameLayer.Middle, mKeyBoardOffetX);
	            mIsFirstNum = true;
            }
            private void OnButtonBuyCurrencyButtonClick()
            {
#if !UNITY_EDITOR
                if (mModel == null)
                {
                    return;
                }
#endif
                /* put your code in here */
                int iLinkItemID = mModel.MoneyId;
                uint totalCount = (uint)(mModel.UnitPrice * mBuyCount);
                uint ownedCount = (uint)ItemDataManager.GetInstance().GetOwnedItemCount(iLinkItemID, false);
                bool bNotEnough = totalCount > ownedCount;
                ItemComeLink.OnLink(iLinkItemID, (int)(totalCount - ownedCount), bNotEnough);
            }
            private void OnButtonMaxButtonClick()
            {
#if !UNITY_EDITOR
                if (mModel == null)
                {
                    return;
                }
#endif
                mBuyCount = mModel.LeftNum;
                UpdateCostData();
                UpdateNumButtonState();
            }

            private void OnButtonAddButtonClick()
            {
#if !UNITY_EDITOR
                if (mModel == null)
                {
                    return;
                }
#endif
                if (mBuyCount >= mModel.LeftNum)
                {
                    return;
                }

                mBuyCount++;

                UpdateCostData();
                UpdateNumButtonState();
            }
            private void OnButtonMinusButtonClick()
            {
                if (mBuyCount <= 1)
                {
                    return;
                }

                mBuyCount--;

                UpdateCostData();
                UpdateNumButtonState();
            }

            #endregion
        }
    }
}