using System;
using System.Collections.Generic;
using LimitTimeGift;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using ActivityLimitTime;

namespace GameClient
{

    public class MallNewLimitTimeMallElementItem : MonoBehaviour
    {
        private MallItemInfo _mallItemInfo = null;

        private ELimitiTimeGiftDataLimitType _playerLimitType = ELimitiTimeGiftDataLimitType.None;
        private string _playerLimitStr = "";        //角色限购的描述
        private int _playerLimitTotalNumber = 0;     //角色限购的总数量
        private int _playerLimitLeftNumber = 0;     //角色限购剩余次数

        private int _remainTimeSeconds = 0;          //剩余时间

        private bool _isLimitPurchaseItem = false;   //是否为限制购买的商品

        [HeaderAttribute("Text")]
        [SerializeField] private Text titleNameText;
        [SerializeField] private Text limitBuyCountText;
        [SerializeField] private Text giftPriceText;
        [SerializeField] private GameObject mObjCost;
        [SerializeField] private GameObject mObjNoCount;

        [SerializeField] private Image mImgItem;
        [SerializeField] private Text mTextName;
        [SerializeField] private Image mImgCostItemIcon;

        [SerializeField]
        private SimpleTimer timer;

        // [SerializeField]
        // private GameObject presentationGo;
        // [SerializeField]
        // private Image presentationIcon;
        // [SerializeField]
        // private Text presentationNum;
        // [SerializeField]
        // private Button presentationBtn;
        // [SerializeField]
        // private Text discountText;
        // [SerializeField]
        // private Image discountIcon;
        //[SerializeField]
        //private Text discountCount;
        // [SerializeField]
        // private GameObject priceDiscountImage;
        // [SerializeField]
        // private GameObject priceNomalImage;

        //打折活动相关
        // [SerializeField]
        // private Text mFirstDiscountDesTxt;
        // [SerializeField]
        // private SimpleTimer mFirstDiscountTimer;
        // [SerializeField]
        // private Text mPrePriceTxt;
        // [SerializeField]
        // private GameObject mFirstDiscountRoot;

        // [SerializeField] private GameObject mMayDayDiscountRoot;
        // [SerializeField] private Text mMayDayPrice;
        private void Awake()
        {
        }

        private void OnDestroy()
        {
            UnBindUiEventSystem();
        }

        private void UnBindUiEventSystem()
        {
            ClearData();
        }

        private void ClearData()
        {
            _mallItemInfo = null;
            _remainTimeSeconds = 0;
            _isLimitPurchaseItem = false;

            _playerLimitType = ELimitiTimeGiftDataLimitType.None;
            _playerLimitStr = "";
            _playerLimitLeftNumber = 0;
            _playerLimitTotalNumber = 0;
        }

        private void OnEnable()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSyncWorldMallBuySucceed,OnSyncWorldMallBuySucceed);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.AccountSpecialItemUpdate, _OnCountValueChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityLimitTimeDataUpdate, _OnLimitTimeDataUpdate);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSyncWorldMallBuySucceed, OnSyncWorldMallBuySucceed);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.AccountSpecialItemUpdate, _OnCountValueChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityLimitTimeDataUpdate, _OnLimitTimeDataUpdate);
        }

        public void Init(MallItemInfo mallItemInfo)
        {
            //首先清空数据
            ClearData();

            _mallItemInfo = mallItemInfo;

            InitElementView();
        }

        private void InitElementView()
        {
            if (_mallItemInfo == null)
                return;
            //初始化限制购买的数据
            InitLeftLimitBuyData();

            InitElementRewards();
            InitElementLeftTimes();
            InitElementPresentationItem();
            InitElementLimitBuyButtonContent();
            UpdateElementDiscountInformation();
            // UpdateElementMayDayDiscountInfomation();
            UpdateElementLimitPurchaseInfo();

           
           InitFirstDiscountInfo(ActivityDataManager.GetInstance().IsShowFirstDiscountDes(_mallItemInfo.id));

        }
        //初始化折扣活动相关数据
        private void InitFirstDiscountInfo(bool isShow)
        {
            // mFirstDiscountRoot.CustomActive(isShow);
            // priceNomalImage.CustomActive(!isShow);
            if(isShow)
            {
                // var activityData= ActivityDataManager.GetInstance().GetActiveDataFromType(ActivityLimitTimeFactory.EActivityType.OAT_LIMITTIMEFIRSTDISCOUNTACTIIVITY);
                // if(activityData!=null)
                // {
                //     mPrePriceTxt.SafeSetText(string.Format(TR.Value("DiscountActivity_PrePriceDes"), Utility.GetMallRealPrice(_mallItemInfo)));
                //     mFirstDiscountDesTxt.SafeSetText(string.Format(TR.Value("DiscountActivity_DiscountDes", activityData.parm * 1.0 / 10)));
                //     //设置打折时间
                //     if (mFirstDiscountTimer != null)
                //     {
                       
                //         uint endTime = activityData.endTime;
                //         uint curTime = TimeManager.GetInstance().GetServerTime();
                //         uint remainTime = 0;
                //         if (endTime >= curTime)
                //         {
                //             remainTime = endTime - curTime;
                //         }
                //         else
                //         {
                //             remainTime = 0;
                //         }
                //         if (remainTime <= 0)
                //         {
                //             remainTime = 0;
                //         }
                //         if (remainTime<=24*3600)
                //         {
                //             mFirstDiscountTimer.useSystemUpdate = true;
                //         }
                //         else
                //         {
                //             mFirstDiscountTimer.useSystemUpdate = false;
                //         }

                //         mFirstDiscountTimer.SetCountdown((int)remainTime);
                //         mFirstDiscountTimer.StartTimer();
                //     }
                // }
            }

           
        }

        [SerializeField] private GameObject mObjSelect;
        public void SetSelect(bool isSelect)
        {
            if (null != mObjSelect)
            {
                mObjSelect.CustomActive(isSelect);
            }
        }

        private void _OnCountValueChanged(UIEvent uiEvent)
        {
            //已经购买过9折礼包
            if (AccountShopDataManager.GetInstance().GetAccountSpecialItemNum(AccountCounterType.ACC_NEW_SERVER_GIFT_DISCOUNT) > 0)
            {
                OnRecover();
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
                        OnRecover();
                    }
                }
            }
        }
        private void OnRecover()
        {
            // mFirstDiscountRoot.CustomActive(false);
            // priceNomalImage.CustomActive(true);
            if(_mallItemInfo!=null)
            {
                int giftPrice = Utility.GetMallRealPrice(_mallItemInfo);
                if (giftPriceText.text != null)
                    giftPriceText.text = giftPrice.ToString();
            }
          
        }

        private void InitLeftLimitBuyData()
        {
            //角色限购或者账号限购
            if ((ELimitiTimeGiftDataLimitType) _mallItemInfo.limit != ELimitiTimeGiftDataLimitType.None
                || _mallItemInfo.accountLimitBuyNum > 0)
            {
                _isLimitPurchaseItem = true;
            }

            //角色限购的类型，数量和描述0.
            _playerLimitType = (ELimitiTimeGiftDataLimitType) _mallItemInfo.limit;
            if (_playerLimitType == ELimitiTimeGiftDataLimitType.Refresh)
            {
                //每天刷新
                _playerLimitStr = TR.Value("limittime_mall_limit_role_everyday");
                _playerLimitTotalNumber = _mallItemInfo.limitnum;
                _playerLimitLeftNumber = _mallItemInfo.limitnum -
                                         CountDataManager.GetInstance().GetCount(_mallItemInfo.id.ToString());

            }
            else if (_playerLimitType == ELimitiTimeGiftDataLimitType.Week)
            {
                //每周刷新
                _playerLimitStr = TR.Value("limittime_mall_limit_role_week");
                _playerLimitTotalNumber = _mallItemInfo.limitnum;
                _playerLimitLeftNumber = _mallItemInfo.limitnum -
                                         CountDataManager.GetInstance().GetCount(_mallItemInfo.id.ToString());
            }
            else if (_playerLimitType == ELimitiTimeGiftDataLimitType.NotRefresh)
            {
                //限购 不刷新
                _playerLimitStr = TR.Value("limittime_mall_limit_role");
                _playerLimitTotalNumber = _mallItemInfo.limittotalnum;
                _playerLimitLeftNumber = _mallItemInfo.limittotalnum -
                                         CountDataManager.GetInstance().GetCount(_mallItemInfo.id.ToString());
            }
        }

        //奖励列表
        private void InitElementRewards()
        {
            //只显示第一个道具
            var rewardCount = _mallItemInfo.giftItems.Length;
            //只显示第一个道具
            var itemData = ItemDataManager.CreateItemDataFromTable((int)_mallItemInfo.giftItems[0].id);
            if (itemData == null)
                return;
            var comItem = mImgItem.GetComponentInChildren<ComItem>();
            if (comItem == null)
            {
                comItem = ComItemManager.Create(mImgItem.gameObject);
            }
            itemData.Count = (int)_mallItemInfo.giftItems[0].num;
            itemData.StrengthenLevel = _mallItemInfo.giftItems[0].strength;
            comItem.Setup(itemData, ShowItemTip);
            //临时改名
            itemData.Name = _mallItemInfo.giftName;
            mTextName.SafeSetText(itemData.GetColorName());
        }

        //剩余时间
        private void InitElementLeftTimes()
        {
            if (timer == null)
                return;
            _remainTimeSeconds = (int)(_mallItemInfo.endtime - TimeManager.GetInstance().GetServerTime());
            //判断是否超过1天 小于1天需要更新时间
            var giftState = LimitTimeGiftState.OnSale;
            var giftIntraData = LimitTimeGiftIntraDay.None;
            if (giftState == LimitTimeGiftState.OnSale)
            {
                giftIntraData = _remainTimeSeconds > 3600 * 24
                    ? LimitTimeGiftIntraDay.MoreThanOneDay
                    : LimitTimeGiftIntraDay.IntraDay;
            }
            //是否需要更新时间
            var needTimeCount = (giftIntraData == LimitTimeGiftIntraDay.IntraDay) ? true : false;
            if (needTimeCount)
                timer.useSystemUpdate = true;
            else
                timer.useSystemUpdate = false;
            timer.SetCountdown(_remainTimeSeconds);
            timer.StartTimer();
        }
        /// <summary>
        /// 礼包可能会赠送的额外货币
        /// </summary>
        private void InitElementPresentationItem()
        {
            // presentationGo.CustomActive(false);
            // if (_mallItemInfo.buyGotInfos != null && _mallItemInfo.buyGotInfos.Length != 0)
            // {
            //     presentationGo.CustomActive(true);
            //     presentationNum.text = string.Format("x{0}",_mallItemInfo.buyGotInfos[0].buyGotNum.ToString());
            //     var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>((int)_mallItemInfo.buyGotInfos[0].itemDataId);
            //     if (itemTableData != null)
            //     {
            //         ETCImageLoader.LoadSprite(ref presentationIcon, itemTableData.Icon);
            //     }
            //     presentationBtn.onClick.RemoveAllListeners();
            //     ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable((int)_mallItemInfo.buyGotInfos[0].itemDataId);
            //     presentationBtn.onClick.AddListener(() => { _OnShowTips(ItemDetailData); });
            // }
        }
        void _OnShowTips(ItemData result)
        {
            if (result == null)
            {
                return;
            }
            ItemTipManager.GetInstance().ShowTip(result);
        }
        //按钮的内容
        private void InitElementLimitBuyButtonContent()
        {
            int giftPrice  = Utility.GetMallRealPrice(_mallItemInfo);
            if (ActivityDataManager.GetInstance().IsShowFirstDiscountDes(_mallItemInfo.id))
            {
                var activityData = ActivityDataManager.GetInstance().GetActiveDataFromType(ActivityLimitTimeFactory.EActivityType.OAT_LIMITTIMEFIRSTDISCOUNTACTIIVITY);
                if(activityData!=null)
                {
                    giftPrice =(int)(giftPrice * activityData.parm * 1.0 / 100);
                }
            }

            //代表是五一礼包 全民团购活动开启
            if (_mallItemInfo.tagType == 2 && ActivityDataManager.GetInstance().CheckGroupPurchaseActivityIsOpen())
            {
                giftPrice = (int)(giftPrice * ActivityDataManager.LimitTimeGroupBuyDiscount * 1.0f / 100);
            }

            if (giftPriceText.text != null)
                giftPriceText.text = giftPrice.ToString();

            if (mImgCostItemIcon != null)
            {
                var costItemTable = MallNewDataManager.GetInstance().GetCostItemTableByCostType(_mallItemInfo.moneytype);
                if (costItemTable != null)
                {
                    ETCImageLoader.LoadSprite(ref mImgCostItemIcon, costItemTable.Icon);
                }
                else
                {
                    Logger.LogErrorFormat("CostItemTable is null and moneyType is {0}", _mallItemInfo.moneytype);
                }
            }
        }

        /// <summary>
        /// 更新打折商品的相关信息,是打折商品，且有打折券的时候，显示打折信息
        /// </summary>
        private void UpdateElementDiscountInformation()
        {
            // var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>((int)_mallItemInfo.discountCouponId);
            // if(itemTableData != null && itemTableData.DiscountCouponProp != 0)
            // {
            //     int itemCount = ItemDataManager.GetInstance().GetOwnedItemCount((int)_mallItemInfo.discountCouponId);
            //     //显示打折卷数量，和折扣价格
            //     // discountText.text = string.Format(TR.Value("mall_new_discount_text"), itemCount);
            //     // ETCImageLoader.LoadSprite(ref discountIcon, itemTableData.Icon);
            //     priceDiscountImage.CustomActive(true);
            //     //priceNomalImage.CustomActive(false);
            //     var giftPrice = Utility.GetMallRealPrice(_mallItemInfo);
            //     //giftPriceText.color = Color.green;
            //     //giftPriceText.text = Math.Floor(Convert.ToDecimal(giftPrice * itemTableData.DiscountCouponProp / 100)).ToString();//向下取整

            // }
            // else
            // {
            //     //giftPriceText.color = Color.white;
            //     priceDiscountImage.CustomActive(false);
            //     //priceNomalImage.CustomActive(true);
            // }
        }

        private void UpdateElementMayDayDiscountInfomation()
        {
            // if (_mallItemInfo.tagType == 2 && ActivityDataManager.GetInstance().CheckGroupPurchaseActivityIsOpen())
            // {
            //     mMayDayDiscountRoot.CustomActive(true);

            //     if(mMayDayDiscount != null)
            //     {
            //         ETCImageLoader.LoadSprite(ref mMayDayDiscount, TR.Value("Discount_LimitTimeGroupBuy_Path", ActivityDataManager.LimitTimeGroupBuyDiscount));
            //     }
               
            //     mMayDayPrice.text = string.Format("原价：{0}", Utility.GetMallRealPrice(_mallItemInfo));
            // }
            // else
            // {
            //     mMayDayDiscountRoot.CustomActive(false);
            // }
        }

        //更新商品限购信息：限购次数，按钮情况
        private void UpdateElementLimitPurchaseInfo(int accountLimitNum = -1)
        {
            //购买按钮重置
            // UpdateBuyButton(true);

            //非限购
            if (!_isLimitPurchaseItem)
            {
                limitBuyCountText.CustomActive(false);
                return;
            }
            limitBuyCountText.CustomActive(true);
            //角色购买限制
            if (_playerLimitType != ELimitiTimeGiftDataLimitType.None)
            {
                if (limitBuyCountText != null)
                    limitBuyCountText.text = string.Format(_playerLimitStr,
                        _playerLimitLeftNumber, _playerLimitTotalNumber);
                
                //不存在剩余次数
                mObjCost.CustomActive(_playerLimitLeftNumber > 0);
                mObjNoCount.CustomActive(_playerLimitLeftNumber <= 0);
            }
            //账号限购
            else if (_mallItemInfo.accountLimitBuyNum > 0)
            {
                int totalNum = (int)_mallItemInfo.accountLimitBuyNum;
                int curNum;
                if (accountLimitNum == -1)
                    curNum = (int)_mallItemInfo.accountRestBuyNum;
                else
                    curNum = accountLimitNum;
                //不存在剩余次数
                mObjCost.CustomActive(curNum > 0);
                mObjNoCount.CustomActive(curNum <= 0);
                // if (curNum <= 0)
                // {
                //     UpdateBuyButton(false);
                // }
                switch (_mallItemInfo.accountRefreshType)
                {
                    case (int)RefreshType.REFRESH_TYPE_PER_DAY:
                        limitBuyCountText.text = string.Format(TR.Value("count_limittime_mall_limit_number_today"), curNum, totalNum);
                        break;
                    case (int)RefreshType.REFRESH_TYPE_PER_WEEK:
                        limitBuyCountText.text = string.Format(TR.Value("count_limittime_mall_limit_number_week"), curNum, totalNum);
                        break;
                    case (int)RefreshType.REFRESH_TYPE_PER_MONTH:
                        limitBuyCountText.text = string.Format(TR.Value("count_limittime_mall_limit_number_month"), curNum, totalNum);
                        break;
                    case (int)RefreshType.REFRESH_TYPE_NONE:
                        limitBuyCountText.text = string.Format(TR.Value("count_limittime_mall_limit_number_everyday"), curNum, totalNum);
                        break;
                }
            }
        }

        #region UIEvent

        private void OnSyncWorldMallBuySucceed(UIEvent uiEvent)
        {
            if (uiEvent == null)
                return;
            if (uiEvent.Param1 == null || uiEvent.Param2 == null)
                return;
            var itemId = (UInt32)uiEvent.Param1;
            var leftLimitNumber = (int)uiEvent.Param2;
            var accountLimitNum = (int)uiEvent.Param3;

            if (_mallItemInfo == null)
                return;
            UpdateElementDiscountInformation();
            if (_mallItemInfo.id != itemId)
                return;
            //非限购商品，直接返回
            if (_isLimitPurchaseItem == false)
                return;
            //限购商品
            _playerLimitLeftNumber = leftLimitNumber;
            //限购商品：对剩余次数和按钮状态进行更新
            UpdateElementLimitPurchaseInfo(accountLimitNum);
        }
        #endregion


        // private void OnButtonClickCallBack()
        // {
        //     if(_mallItemInfo == null)
        //         return;

        //     if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
        //     {
        //         return;
        //     }
        //     OpenMallBuyFrame();
        // }

        // private void UpdateBuyButton(bool flag)
        // {
        //     if (buyButtonGray != null)
        //         buyButtonGray.enabled = !flag;

        //     if (buyButton != null)
        //         buyButton.interactable = flag;
        // }

        private void ShowItemTip(GameObject go, ItemData itemData)
        { 
            //添加埋点
            Utility.DoStartFrameOperation("LimitTimeMallView", string.Format("ItemID/{0}", itemData.PackID));
            ItemTipManager.GetInstance().ShowTip(itemData);
        }

        //打开购买界面
        // private void OpenMallBuyFrame()
        // {
        //     if (ClientSystemManager.GetInstance().IsFrameOpen<MallBuyFrame>() == true)
        //         ClientSystemManager.GetInstance().CloseFrame<MallBuyFrame>();

        //     ClientSystemManager.GetInstance().OpenFrame<MallBuyFrame>(FrameLayer.Middle, _mallItemInfo);
        // }

        public void Reset()
        {
            ClearData();
        }

    }
}
