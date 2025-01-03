using System;
using System.Collections;
using System.Collections.Generic;
using ActivityLimitTime;
using LimitTimeGift;
using Network;
using Protocol;
using ProtoTable;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class MallGiftPackItem : MonoBehaviour, IDisposable
    {
        [SerializeField] private Text mTextDescription;
        [SerializeField] private Image mImageBg;
        [SerializeField] private RectTransform mItemRoot;
        [SerializeField] Vector2 mComItemSize;
        [SerializeField] private Text mTextDetail;
        [SerializeField] private SimpleTimer mSimpleTimer;
        [SerializeField] private Image mImageCurrency;
        [SerializeField] private Text mTextCost;
        [SerializeField] private Color mNotEnoughColor = Color.red;
        [SerializeField]
        private GameObject presentationGo;
        [SerializeField]
        private Image presentationIcon;
        [SerializeField]
        private Text presentationNum;
        [SerializeField]
        private Button presentationBtn;
        [SerializeField]
        private GameObject discountGo;
        [SerializeField]
        private Text mAccountTextDetail;

        private List<ComItem> mComItems = new List<ComItem>();
        private Color mCostOriginalCorlor;
        private LimitTimeGiftPackDetailModel mData;
        private LimitTimeMallGiftPackActivityView.OnActivityMallItemSelectEvent<int> mOnItemSelect;
        
        private int mIndex;
        private int accountLimitNum = -1;

        private int mActivityId = 0;

        //首套折扣相关
        [SerializeField]
        private Text mFirstDiscountDesTxt;
        [SerializeField]
        private SimpleTimer mFirstDiscountTimer;
        [SerializeField]
        private Text mFirstDiscountPriceTxt;
        [SerializeField]
        private Image mFirstDiscountPriceIconImg;
        [SerializeField]
        private GameObject mFirstDiscountRootGo;
        [SerializeField]
        private GameObject mFirstDiscountPriceRootGo;

        [SerializeField] private GameObject mMayDayGiftDiscountRoot;
        [SerializeField] private Text mMayDayDiscount;
        [SerializeField] private Toggle mToggle;

        private OpActivityData mFirstDiscountActivityData = null;//打折活动
        private bool mIsCanShowDiscount = false;
        void Awake()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSyncWorldMallBuySucceed, OnSyncWorldMallBuySucceed);
            this.mCostOriginalCorlor = this.mTextCost.color;
        }
        void OnDestroy()
        {
            Dispose();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSyncWorldMallBuySucceed, OnSyncWorldMallBuySucceed);
        }
        private void OnSyncWorldMallBuySucceed(UIEvent uiEvent)
        {
            if (uiEvent == null)
                return;

            if (uiEvent.Param1 == null || uiEvent.Param2 == null)
                return;

            var itemId = (UInt32)uiEvent.Param1;
            //var leftLimitNumber = (int)uiEvent.Param2;
            //限购商品：对剩余次数和按钮状态进行更新
            if (itemId == mData.Id)
            {
                accountLimitNum = (int)uiEvent.Param3;
                UpdateData(mData);
            }
        }

        public void Init(int index, LimitTimeGiftPackDetailModel data, LimitTimeMallGiftPackActivityView.OnActivityMallItemSelectEvent<int> onItemSelect, int activityId, ToggleGroup group)
        {
            if (null != mToggle)
                mToggle.group = group;
            mData = data;
            mOnItemSelect = onItemSelect;
            mIndex = index;
            UpdateData(data);
            InitItems(data.mRewards);
            InitElementPresentationItem(data);
            PlayerBaseData.GetInstance().onMoneyChanged += this.OnMoneyChanged;
            mActivityId = activityId;

            InitFirstDiscountActivtiyParams(data);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.AccountSpecialItemUpdate, _OnCountValueChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityLimitTimeDataUpdate, _OnLimitTimeDataUpdate);
        }
        /// <summary>
        /// 初始化打折活动相关内容
        /// </summary>
        private void InitFirstDiscountActivtiyParams(LimitTimeGiftPackDetailModel data)
        {
            mFirstDiscountActivityData = ActivityDataManager.GetInstance().GetActiveDataFromType(ActivityLimitTimeFactory.EActivityType.OAT_LIMITTIMEFIRSTDISCOUNTACTIIVITY);
            if (ActivityDataManager.GetInstance().IsShowFirstDiscountDes(data.Id))
            {
                mIsCanShowDiscount = true;
                //设置打折时间
                if (mFirstDiscountTimer != null)
                {
                    uint endTime = mFirstDiscountActivityData.endTime;
                    uint curTime= TimeManager.GetInstance().GetServerTime();
                    uint remainTime = 0;
                    if (endTime>=curTime)
                    {
                        remainTime = endTime - curTime;
                    }
                    else
                    {
                        remainTime = 0;
                    }
                    if (remainTime <= 0)
                    {
                        remainTime = 0;
                    }
                    if(remainTime>=24*3600)
                    {
                        mFirstDiscountTimer.useSystemUpdate = false;
                    }
                    else
                    {
                        mFirstDiscountTimer.useSystemUpdate = true;
                    }
                    mFirstDiscountTimer.SetCountdown((int)remainTime);
                    mFirstDiscountTimer.StartTimer();
                }
                //折率
                mFirstDiscountDesTxt.SafeSetText(string.Format(TR.Value("DiscountActivity_DiscountDes", mFirstDiscountActivityData.parm*1.0/10)));
            }
            else
            {
                mIsCanShowDiscount = false;
            }

            HideOrShowFirstDiscountDes(mIsCanShowDiscount);




         
        }



        public uint GetGiftId()
        {
            return mData.Id;
        }

        public void UpdateData(LimitTimeGiftPackDetailModel data)
        {
            mData = data;

            if (data.GiftType != MallGoodsType.GIFT_COMMON || data.LimitTotalNum > 0 || data.Limit == (int)ELimitiTimeGiftDataLimitType.Week)
            {
                UpdateBuyCountNew(data);
            }
            else
            {
                this.mTextDetail.text = string.Format(TR.Value("activity_mall_gift_pack_item_detail").Replace("\\n", "\n"), "", Function.GetLeftDay((int)TimeManager.GetInstance().GetServerTime(), (int)data.GiftEndTime));
            }
            if (mSimpleTimer)
            {
                if (data.NeedTimeCountDown)
                    mSimpleTimer.useSystemUpdate = true;
                else
                    mSimpleTimer.useSystemUpdate = false;

                uint remainTime = data.GiftEndTime - TimeManager.GetInstance().GetServerTime();
                if (remainTime <= 0)
                {
                    remainTime = 0;
                }

                mSimpleTimer.SetCountdown((int)remainTime);
                mSimpleTimer.StartTimer();
            }

            UpdateCostText();
            if (data.DiscountCouponId > 0)
            {
                discountGo.CustomActive(true);
            }
            else
            {
                discountGo.CustomActive(false);
            }
            //账号限购
            if (data.AccountLimitBuyNum > 0)
            {
                mAccountTextDetail.CustomActive(true);
                int totalNum = (int)data.AccountLimitBuyNum;
                int curNum;
                if (accountLimitNum == -1 || accountLimitNum == 2147483647)
                {
                    curNum = (int)data.AccountRestBuyNum;
                }
                else
                {
                    curNum = accountLimitNum;
                }

                switch (data.AccountRefreshType)
                {
                    case (int)RefreshType.REFRESH_TYPE_PER_DAY:
                        mAccountTextDetail.text = string.Format(TR.Value("count_limittime_mall_limit_number_today"), curNum, totalNum);
                        break;
                    case (int)RefreshType.REFRESH_TYPE_PER_WEEK:
                        mAccountTextDetail.text = string.Format(TR.Value("count_limittime_mall_limit_number_week"), curNum, totalNum);
                        break;
                    case (int)RefreshType.REFRESH_TYPE_PER_MONTH:
                        mAccountTextDetail.text = string.Format(TR.Value("count_limittime_mall_limit_number_month"), curNum, totalNum);
                        break;
                    case (int)RefreshType.REFRESH_TYPE_NONE:
                        mAccountTextDetail.text = string.Format(TR.Value("count_limittime_mall_limit_number_everyday"), curNum, totalNum);
                        break;
                }
            }
            else
            {
                mAccountTextDetail.CustomActive(false);
            }
        }

        void UpdateCostText()
        {
            this.mTextCost.text = this.mData.GiftPrice.ToString();
            ulong ownNum = 0;
            switch (mData.PriceType)
            {
                case LimitTimeGiftPriceType.Point:
                    ownNum = PlayerBaseData.GetInstance().Ticket;
                    break;
                case LimitTimeGiftPriceType.BindPint:
                    ownNum = PlayerBaseData.GetInstance().BindTicket;
                    break;
                case LimitTimeGiftPriceType.Gold:
                    ownNum = PlayerBaseData.GetInstance().Gold;
                    break;
                case LimitTimeGiftPriceType.BindGOLD:
                    ownNum = PlayerBaseData.GetInstance().BindGold;
                    break;
            }
            var costItemTable = MallNewDataManager.GetInstance().GetCostItemTableByCostType((byte)mData.PriceType);
            if (mImageCurrency != null)
            {
                if (costItemTable != null)
                {
                    ETCImageLoader.LoadSprite(ref mImageCurrency, costItemTable.Icon);
                }

            }
            //设置9折活动打折价格的icon和价格
            if (mIsCanShowDiscount)
            {
                if (mFirstDiscountPriceIconImg != null)
                {
                    if (costItemTable != null)
                    {
                        ETCImageLoader.LoadSprite(ref mFirstDiscountPriceIconImg, costItemTable.Icon);
                    }
                }
                int discountPrice = 0;
                if (mFirstDiscountPriceTxt != null)
                {
                    if(mFirstDiscountActivityData!=null)
                    {
                        discountPrice = (int)Math.Floor(Convert.ToDecimal(mData.GiftPrice * mFirstDiscountActivityData.parm * 1.0f / 100));
                        mFirstDiscountPriceTxt.SafeSetText(discountPrice.ToString());
                    }
                   
                }
                if (ownNum >= (ulong)discountPrice)
                {
                    mFirstDiscountPriceTxt.color = this.mCostOriginalCorlor;
                }
                else
                {
                    mFirstDiscountPriceTxt.color = mNotEnoughColor;
                }

            }

            //五一礼包价格显示
            var mallItemTable = TableManager.GetInstance().GetTableItem<MallItemTable>((int)mData.Id);
            if (mallItemTable != null)
            {
                if (mallItemTable.tagtype == 2 && ActivityDataManager.GetInstance().CheckGroupPurchaseActivityIsOpen())
                {
                    if (mFirstDiscountPriceIconImg != null)
                    {
                        if (costItemTable != null)
                        {
                            ETCImageLoader.LoadSprite(ref mFirstDiscountPriceIconImg, costItemTable.Icon);
                        }
                    }

                    int discountPrice = (int)(mData.GiftPrice * ActivityDataManager.LimitTimeGroupBuyDiscount * 1.0f / 100);
                    if (mFirstDiscountPriceTxt != null)
                        mFirstDiscountPriceTxt.text = discountPrice.ToString();

                    if (ownNum >= (ulong)discountPrice)
                    {
                        mFirstDiscountPriceTxt.color = this.mCostOriginalCorlor;
                    }
                    else
                    {
                        mFirstDiscountPriceTxt.color = mNotEnoughColor;
                    }

                    if (mMayDayDiscount != null)
                    {
                        mMayDayDiscount.text = string.Format("{0}折", ActivityDataManager.LimitTimeGroupBuyDiscount / 10f);
                    }

                    mFirstDiscountPriceRootGo.CustomActive(true);
                    mMayDayGiftDiscountRoot.CustomActive(true);
                }
                else
                {
                    mFirstDiscountPriceRootGo.CustomActive(false);
                    mMayDayGiftDiscountRoot.CustomActive(false);
                }
            }

            if (ownNum >= (ulong)mData.GiftPrice)
            {
                mTextCost.color = this.mCostOriginalCorlor;
            }
            else
            {
                this.mTextCost.color = mNotEnoughColor;
            }


        }
        void InitItems(ItemReward[] awards)
        {
            if (awards == null || awards.Length == 0)
            {
                return;
            }

            for (int i = 0; i < awards.Length; ++i)
            {
                var comItem = ComItemManager.Create(this.mItemRoot.gameObject);
                if (comItem != null)
                {
                    ItemData data = ItemDataManager.CreateItemDataFromTable((int)awards[i].id);
                    if (data == null)
                    {
                        Logger.LogError("道具表找补到id为" + awards[i].id + "的道具");
                        continue;
                    }
                    data.Count = (int)awards[i].num;
                    data.StrengthenLevel = awards[i].strength;
                    comItem.Setup(data, (obj, itemData) =>
                    {
                        // if (mActivityId == 5000)
                        // {
                        //     //添加埋点
                        //     Utility.DoStartFrameOperation("MallGiftPackActivity", string.Format("ItemID/{0}", data.TableID));
                        // }
                        // Utility.OnItemClicked(gameObject, data);
                    });
                    this.mComItems.Add(comItem);
                    mTextDescription.text = data.GetColorName();
                }

                (comItem.transform as RectTransform).sizeDelta = this.mComItemSize;
            }
        }
        /// <summary>
        /// 礼包可能会赠送的额外货币
        /// </summary>
        private void InitElementPresentationItem(LimitTimeGiftPackDetailModel limitTimeGiftData)
        {
            presentationGo.CustomActive(false);
            MallItemInfo ItemInfo = new MallItemInfo();
            ItemInfo.id = limitTimeGiftData.Id;
            ItemInfo.limitnum = (ushort)limitTimeGiftData.LimitNum;
            ItemInfo.limittotalnum = (ushort)limitTimeGiftData.LimitTotalNum;
            ItemInfo.gift = (byte)limitTimeGiftData.GiftType;
            ItemInfo.buyGotInfos = limitTimeGiftData.buyGotInfos;
            var _mallItemInfo = ItemInfo;
            if (_mallItemInfo.buyGotInfos != null && _mallItemInfo.buyGotInfos.Length != 0)
            {
                presentationGo.CustomActive(true);
                presentationNum.text = string.Format("*{0}", _mallItemInfo.buyGotInfos[0].buyGotNum.ToString());
                var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>((int)_mallItemInfo.buyGotInfos[0].itemDataId);
                if (itemTableData != null)
                {
                    ETCImageLoader.LoadSprite(ref presentationIcon, itemTableData.Icon);
                }
                presentationBtn.onClick.RemoveAllListeners();
                ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable((int)_mallItemInfo.buyGotInfos[0].itemDataId);
                presentationBtn.onClick.AddListener(() => { _OnShowTips(ItemDetailData); });
            }
        }
        void _OnShowTips(ItemData result)
        {
            if (result == null)
            {
                return;
            }
            if (mActivityId == 5000)
            {
                //添加埋点
                Utility.DoStartFrameOperation("MallGiftPackActivity", string.Format("ItemID/{0}", result.TableID));
            }
            ItemTipManager.GetInstance().ShowTip(result);
        }
        void UpdateBuyCountNew(LimitTimeGiftPackDetailModel limitTimeGiftData)
        {
            bool bIsDailyLimit = false;
            MallItemInfo _mallItemInfo = new MallItemInfo();
            _mallItemInfo.id = limitTimeGiftData.Id;
            _mallItemInfo.limitnum = (ushort)limitTimeGiftData.LimitNum;
            _mallItemInfo.limittotalnum = (ushort)limitTimeGiftData.LimitTotalNum;
            _mallItemInfo.gift = (byte)limitTimeGiftData.GiftType;
            _mallItemInfo.limit = limitTimeGiftData.Limit;
            _mallItemInfo.accountLimitBuyNum = limitTimeGiftData.AccountLimitBuyNum;

            int LeftLimitNum = Utility.GetLeftLimitNum(_mallItemInfo, ref bIsDailyLimit);
            string info = "";
            bool _isLimitPurchaseItem = false;
            if (_mallItemInfo.limitnum > 0
                || _mallItemInfo.limittotalnum > 0
                || _mallItemInfo.accountLimitBuyNum > 0)
                _isLimitPurchaseItem = true;

            if (_isLimitPurchaseItem == false)
            {
                mTextDetail.CustomActive(false);
                mAccountTextDetail.CustomActive(false);
                return;
            }
            //角色购买限制
            mTextDetail.CustomActive(true);
            var totalLimitNumber = _mallItemInfo.limittotalnum > _mallItemInfo.limitnum
                ? _mallItemInfo.limittotalnum
                : _mallItemInfo.limitnum;
            bool _isDailyLimit = false;
            var _leftLimitPurchaseTimes = Utility.GetLeftLimitNum(_mallItemInfo, ref _isDailyLimit);
            if (_mallItemInfo.limit == (int)ELimitiTimeGiftDataLimitType.Week)
            {
                info = TR.Value("limittime_mall_limit_role_week",
                    _leftLimitPurchaseTimes, totalLimitNumber);
            }
            else if (_mallItemInfo.limit == (int)ELimitiTimeGiftDataLimitType.Refresh)//每天刷新
            {
                info = TR.Value("limittime_mall_limit_role_everyday",
                   _leftLimitPurchaseTimes, totalLimitNumber);
            }
            else if (_mallItemInfo.limit == (int)ELimitiTimeGiftDataLimitType.NotRefresh)//限购不刷新
            {
                info = TR.Value("limittime_mall_limit_role",
                   _leftLimitPurchaseTimes, totalLimitNumber);
            }

            this.mTextDetail.text = string.Format(TR.Value("activity_mall_gift_pack_item_detail").Replace("\\n", "\n"), info, Function.GetLeftDay((int)TimeManager.GetInstance().GetServerTime(), (int)limitTimeGiftData.GiftEndTime)); ;
        }

        void UpdateBuyCount(LimitTimeGiftPackDetailModel limitTimeGiftData)
        {
            bool bIsDailyLimit = false;
            MallItemInfo ItemInfo = new MallItemInfo();
            ItemInfo.id = limitTimeGiftData.Id;
            ItemInfo.limitnum = (ushort)limitTimeGiftData.LimitNum;
            ItemInfo.limittotalnum = (ushort)limitTimeGiftData.LimitTotalNum;
            ItemInfo.gift = (byte)limitTimeGiftData.GiftType;
            ItemInfo.limit = limitTimeGiftData.Limit;

            int LeftLimitNum = Utility.GetLeftLimitNum(ItemInfo, ref bIsDailyLimit);
            string info = "";
            if (ItemInfo.limit == (int)ELimitiTimeGiftDataLimitType.Week)
            {
                info = string.Format(TR.Value("limittime_mall_limit_number_week"), LeftLimitNum);
            }
            else
            {
                info = bIsDailyLimit ? string.Format(TR.Value("mall_gift_daily_buy"), LeftLimitNum) : string.Format(TR.Value("mall_gift_buy"), LeftLimitNum);
            }
            if (LeftLimitNum <= 0)
            {
                if (ItemInfo.limit == (int)ELimitiTimeGiftDataLimitType.Week)
                {
                    info = string.Format(TR.Value("limittime_mall_limit_number_finished_week"));
                }
                else
                {
                    info = bIsDailyLimit ? TR.Value("mall_gift_daily_empty") : TR.Value("mall_gift_empty");
                }
            }
            this.mTextDetail.text = string.Format(TR.Value("activity_mall_gift_pack_item_detail").Replace("\\n", "\n"), info, Function.GetLeftDay((int)TimeManager.GetInstance().GetServerTime(), (int)limitTimeGiftData.GiftEndTime)); ;
        }

        void OnMoneyChanged(PlayerBaseData.MoneyBinderType eTarget)
        {
            UpdateCostText();
        }

        private void _OnCountValueChanged(UIEvent uiEvent)
        {
            //已经购买过9折礼包
            if (AccountShopDataManager.GetInstance().GetAccountSpecialItemNum(AccountCounterType.ACC_NEW_SERVER_GIFT_DISCOUNT) > 0)
            {
                HideOrShowFirstDiscountDes(false);
            }
        }

        /// <summary>
        /// 监听活动状态改变  
        /// </summary>
        /// <param name="uiEvent"></param>
        private void _OnLimitTimeDataUpdate(UIEvent uiEvent)
        {
            uint activityId = (uint)uiEvent.Param1;
            var firstDiscountActivityData =ActivityDataManager.GetInstance().GetActiveDataFromType(ActivityLimitTimeFactory.EActivityType.OAT_LIMITTIMEFIRSTDISCOUNTACTIIVITY);
            if (firstDiscountActivityData != null)
            {
                if(firstDiscountActivityData.dataId==activityId)
                {
                    if(firstDiscountActivityData.state==(int)ActivityState.End)
                    {
                        HideOrShowFirstDiscountDes(false);
                    }
                }
            }
        }

        private void HideOrShowFirstDiscountDes(bool isShow)
        {
            mFirstDiscountRootGo.CustomActive(isShow);
            mFirstDiscountPriceRootGo.CustomActive(isShow);
        }

        public void Destroy()
        {
            Dispose();
            Destroy(gameObject);
        }

        public void Dispose()
        {
            for (int i = this.mComItems.Count - 1; i >= 0; --i)
            {
                ComItemManager.Destroy(mComItems[i]);
            }
            mComItems.Clear();
            if (mSimpleTimer)
                mSimpleTimer.StopTimer();
            PlayerBaseData.GetInstance().onMoneyChanged -= OnMoneyChanged;

            mIsCanShowDiscount = false;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.AccountSpecialItemUpdate, _OnCountValueChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityLimitTimeDataUpdate, _OnLimitTimeDataUpdate);
            mFirstDiscountActivityData = null;
            if (mFirstDiscountTimer != null)
            {
                mFirstDiscountTimer.StopTimer();

            }
        }

        public void SetSelect(bool value)
        {
            if (null != mToggle)
            {
                if (value)
                {
                    if (mToggle.isOn)
                        OnToggleChanged(true);
                    else
                        mToggle.isOn = true;
                }
                else
                    mToggle.isOn = false;
            }
        }

        public void OnToggleChanged(bool value)
        {
            if (value && null != mOnItemSelect)
            {
                mOnItemSelect(mIndex);
            }
        }
    }
}
