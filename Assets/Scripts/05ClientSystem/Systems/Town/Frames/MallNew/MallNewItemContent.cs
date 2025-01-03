using System;
using System.Collections;
using System.Collections.Generic;
using LimitTimeGift;
using Network;
using Protocol;
using ProtoTable;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    //商城右侧的tip
    public class MallNewItemContent : MonoBehaviour
    {
        public enum EItemLimitType
        {
            None,//无(永久
            Monthly,//每月
            Weekly,//每周
            Daily,//每日
        }
        [SerializeField] private ComItemNew mItem;
        [SerializeField] private Image mImgCost;
        [SerializeField] private Text mTextDesp;
        [SerializeField] private Text mTextName;
        [SerializeField] private Text mTextBuyCount;
        [SerializeField] private Text mTextLimitBuyType;
        [SerializeField] private Text mTextLimitBuy;
        [SerializeField] private Transform mTrShort;
        [SerializeField] private Transform mTrLong;
        [SerializeField] private SimpleTimer mEndTimer;
        [SerializeField] private Text mTextTotalCost;
        [SerializeField] private Button mBtnAdd;
        [SerializeField] private Button mBtnReduce;
        [SerializeField] private UIGray mGrayAdd;
        [SerializeField] private UIGray mGrayReduce;
        [SerializeField] private Image mImgExtraGotItemIcon;
        [SerializeField] private Text mTextExtraGotItemCount;
        [SerializeField] private ComUIListScript mGiftItemListScript;
        [SerializeField] private GameObject mObjCoupon;
        [SerializeField] private Text mTextCouponCount;
        [SerializeField] private UIGray mGray;
        [SerializeField] private GameObject mObjShow;
        [SerializeField] private GameObject mObjHide;
        //商城道具数据
        private MallItemInfo mMallItem;
        //商店道具数据
        private ShopNewShopItemInfo mShopItem;
        //账号商店道具数据
        private AccountShopItemInfo mAccountShopItem;
        //当前商品所属商店种类（商城/商店/账号商店）
        private ShopItemType mType = ShopItemType.None;
        //显示的道具id
        private int mItemId;
        //消耗的道具id
        private int mCostItemId; 
        //描述
        private string mDesp; 
        //消耗的道具数量
        private int mCostItemCount;
        //角色限购次数
        private int mRoleLimitCount; 
        //角色可购买次数
        private int mRoleBuyCount;
        //账号限购次数
        private int mAccountLimitCount;
        //账号可购买次数
        private int mAccountBuyCount;
        //可以购买的最大数量
        private int mCanBuyMaxNum;
        //购买商品的数量
        private int mBuyCount;
        //使用折扣券的数量
        private int mCostCouponCount;
        //折扣券id
        private int mCouponItemId;
        //折扣券数量
        private int mCouponItemCount;
        //购买额外获得道具
        private int mExtraGotItemId;
        //购买额外获得道具数量
        private int mExtraGotItemCount;
        //下架时间
        private int mEndTime;
        private EItemLimitType mLimitTimeType = EItemLimitType.None;
        //是否接受礼包信息
        private bool mNeedGetGiftData = false;
        //礼包内容
        private List<GiftSyncInfo> mGiftItemList = new List<GiftSyncInfo>();

        private void Awake()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChangeNum, OnChangeNum);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GetGiftData, OnGetGiftData);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSyncWorldMallBuySucceed, UpdateMallContent);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.AccountShopItemUpdata, UpdateAccShopContent);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ShopNewBuyGoodsSuccess, UpdateShopContent);
            
        }

        public void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ChangeNum, OnChangeNum);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GetGiftData, OnGetGiftData);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSyncWorldMallBuySucceed, UpdateMallContent);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.AccountShopItemUpdata, UpdateAccShopContent);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ShopNewBuyGoodsSuccess, UpdateShopContent);
            if (mGiftItemListScript.IsInitialised())
                mGiftItemListScript.UnInitialize();
        }

        //购买商店商品返回
        private void UpdateShopContent(UIEvent uiEvent)
        {
            if (null == mShopItem)
                return;
            if (null == uiEvent.Param2 || !(uiEvent.Param2 is ushort) ||
                null == uiEvent.Param1 || !(uiEvent.Param1 is int))
                    return;
            if (mShopItem.ShopItemId == (int)uiEvent.Param1)
            {
                mShopItem.LimitBuyTimes = mShopItem.LimitBuyTimes >= 0 ? (ushort)uiEvent.Param2 : -1;
                OnInitShopItem(mShopItem);
            }
        }

        //购买账号商店商品返回
        private void UpdateAccShopContent(UIEvent uiEvent)
        {
            if (null == mAccountShopItem)
                return;
            if (null == uiEvent.Param2 || !(uiEvent.Param2 is uint) ||
                null == uiEvent.Param3 || !(uiEvent.Param3 is uint) ||
                null == uiEvent.Param4 || !(uiEvent.Param4 is uint))
                    return;
            if (mAccountShopItem.shopItemId == (uint)uiEvent.Param2)
            {
                mAccountShopItem.accountRestBuyNum = (uint)uiEvent.Param3;
                mAccountShopItem.roleRestBuyNum = (uint)uiEvent.Param4;
                OnInitAccountShopItem(mAccountShopItem);
            }
        }

        //购买商城商品返回
        private void UpdateMallContent(UIEvent uiEvent)
        {
            if (null == mMallItem)
                return;
            if (mMallItem.id == (uint)uiEvent.Param1)
            {
                int count = (int)uiEvent.Param3;
                mMallItem.accountRestBuyNum = (uint)count;
                OnInitMallItem(mMallItem);
            }
        }

        //礼包道具显示
        private void OnGiftItemVisiable(ComUIListElementScript item)
        {
            if (null == item || mGiftItemList.Count <= item.m_index)
                return;
            var script = item.GetComponent<ComItemNew>();
            if(null == script)
                return;
            var itemInfo = mGiftItemList[item.m_index];
            var itemData = ItemDataManager.CreateItemDataFromTable((int)itemInfo.itemId);
            if (null == itemData)
            {
                Logger.LogErrorFormat("The ItemData is null and itemId is {0}", itemInfo.itemId);
                return;
            }
            itemData.Count = (int)itemInfo.itemNum;
            script.Setup(itemData, (go, curItemData) =>
            {
                Utility.DoStartFrameOperation("MallNewPropertyMallElementItem", string.Format("DetailItem/{0}", itemData.PackID));
                ItemTipManager.GetInstance().ShowTip((ItemData)curItemData);
            });
            script.SetFashionMaskShow(false);
        }

        //清空数据
        private void _ClearData()
        {
            mMallItem = null;
            mShopItem = null;
            mAccountShopItem = null;
            mType = ShopItemType.None;
            mItemId = 0;
            mCostItemId = 0;
            mCostItemCount = 0;
            mRoleLimitCount = 0;
            mRoleBuyCount = 0;
            mAccountLimitCount = 0;
            mAccountBuyCount = 0;
            mCanBuyMaxNum = 0;
            mBuyCount = 0;
            mCostCouponCount = 0;
            mCouponItemId = 0;
            mCouponItemCount = 0;
            mDesp = "";
            mEndTime = 0;
            mExtraGotItemId = 0;
            mExtraGotItemCount = 0;
            mIsFristNum = false;
        }

#region 商城道具
        public void OnInitMallItem(MallItemInfo itemData)
        {
            mObjHide.CustomActive(null == itemData);
            mObjShow.CustomActive(null != itemData);
            if (null == itemData)
                return;
            _ClearData();
            mMallItem = itemData;
            if (0 != mMallItem.itemid)
                mItemId = (int)mMallItem.itemid;
            else
                mItemId = (int)mMallItem.giftItems[0].id;
            mDesp = mMallItem.giftDesc;
            mCostItemId = ItemDataManager.GetInstance().GetMoneyIDByType((ItemTable.eSubType)mMallItem.moneytype);
            mCostItemCount = (int)mMallItem.discountprice;
            mRoleLimitCount = mMallItem.limitnum > mMallItem.limittotalnum ? mMallItem.limitnum : mMallItem.limittotalnum;
            mRoleBuyCount = mRoleLimitCount - CountDataManager.GetInstance().GetCount(mMallItem.id.ToString());
            mType = ShopItemType.Mall;
            mMallItem = itemData;
            mAccountBuyCount = (int)mMallItem.accountRestBuyNum;
            mAccountLimitCount = (int)mMallItem.accountLimitBuyNum;
            mCouponItemId = (int)itemData.discountCouponId;
            mLimitTimeType = (EItemLimitType)mMallItem.accountRefreshType;
            //如果账号限购为0 可能是角色限购
            if (mLimitTimeType == EItemLimitType.None && 0 != mMallItem.limit)
            {
                if (mMallItem.limit == (byte)ELimitiTimeGiftDataLimitType.Refresh)
                    mLimitTimeType = EItemLimitType.Daily;
                else if (mMallItem.limit == (byte)ELimitiTimeGiftDataLimitType.Week)
                    mLimitTimeType = EItemLimitType.Weekly;
            }
            if (0 != mCouponItemId)
                mCouponItemCount = ItemDataManager.GetInstance().GetOwnedItemCount(mCouponItemId);
            mEndTime = (int)mMallItem.endtime;
            if (mMallItem.buyGotInfos != null && mMallItem.buyGotInfos.Length != 0)
            {
                mExtraGotItemId = (int)mMallItem.buyGotInfos[0].itemDataId;
                mExtraGotItemCount = (int)mMallItem.buyGotInfos[0].buyGotNum;
            }
            _InitItem();
        }

        //购买商城道具
        private void _BuyMallItem()
        {
            if (null == mMallItem)
                return;
            var costInfo = new CostItemManager.CostInfo()
            {
                nMoneyID = mCostItemId,
                nCount = mBuyCount * mCostItemCount,
            };
            CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, _ReqBuyMallItem);
        }
        private void _ReqBuyMallItem()
        {
            WorldMallBuy req = new WorldMallBuy();
            req.itemId = mMallItem.id;
            req.num = (UInt16)mBuyCount;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }
#endregion

#region 商店道具
        public void OnInitShopItem(ShopNewShopItemInfo itemData)
        {
            mObjHide.CustomActive(null == itemData);
            mObjShow.CustomActive(null != itemData);
            if (null == itemData)
                return;
            _ClearData();
            mType = ShopItemType.Shop;
            mShopItem = itemData;
            var shopTable = TableManager.GetInstance().GetTableItem<ShopItemTable>((int)mShopItem.ShopItemId);
            if (null != shopTable)
            {
                mItemId = shopTable.ItemID;
                mCostItemId = shopTable.CostItemID;
                mCostItemCount = shopTable.CostNum;
                var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(shopTable.ItemID);
                if (null != itemTable)
                    mDesp = itemTable.Description;
            }
            int shopItemId = (int)itemData.ShopItemId;
            if (itemData.LimitBuyTimes >= 0)
            {
                mRoleBuyCount = itemData.LimitBuyTimes;
                mRoleLimitCount = shopTable.NumLimite;
            }
            _InitItem();
        }
        //购买商品
        private void _BuyShopItem()
        {
            if (null == mShopItem)
                return;
            var costInfo = new CostItemManager.CostInfo()
            {
                nMoneyID = mCostItemId,
                nCount = mBuyCount * mCostItemCount,
            };
            CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, _ReqBuyShopItem);
        }
        private void _ReqBuyShopItem()
        {
            ShopNewDataManager.GetInstance().BuyGoods(mShopItem.ShopId,
                mShopItem.ShopItemId,
                mBuyCount,
                new List<ItemInfo>());
        }
#endregion

#region 账号商店道具
        public void OnInitAccountShopItem(AccountShopItemInfo itemData)
        {
            mObjHide.CustomActive(null == itemData);
            mObjShow.CustomActive(null != itemData);
            if (null == itemData)
                return;
            _ClearData();
            mType = ShopItemType.AccountShop;
            mAccountShopItem = itemData;
            mItemId = (int)mAccountShopItem.itemDataId;
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(mItemId);
            if (null != itemTable)
                mDesp = itemTable.Description;
            mCostItemId = (int)mAccountShopItem.costItems[0].id;
            mCostItemCount = (int)mAccountShopItem.costItems[0].num;
            mRoleLimitCount = (int)mAccountShopItem.roleLimitBuyNum;
            mAccountBuyCount = (int)itemData.accountRestBuyNum;
            mAccountLimitCount = (int)itemData.accountLimitBuyNum;
            mRoleBuyCount = (int)itemData.roleRestBuyNum;
            _InitItem();
        }
        //购买账号商店城道具
        private void _BuyAccountShopItem()
        {
            if (null == mAccountShopItem)
                return;
            var costInfo = new CostItemManager.CostInfo()
            {
                nMoneyID = mCostItemId,
                nCount = mBuyCount * mCostItemCount,
            };
            CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, _ReqBuyAccountShopItem);
        }
        private void _ReqBuyAccountShopItem()
        {
            AccountShopQueryIndex queryIndex = new AccountShopQueryIndex();
            queryIndex.shopId = mAccountShopItem.shopId;
            queryIndex.tabType = (byte)mAccountShopItem.tabType;
            queryIndex.jobType = (byte)mAccountShopItem.jobType;
            AccountShopDataManager.GetInstance().SendWorldAccountShopItemBuyReq(queryIndex, (uint)mAccountShopItem.shopItemId, (uint)mBuyCount);
        
        }
#endregion
        
        //初始界面
        private void _InitItem()
        {
            //comitem
            var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mItemId);
            if (null != itemData)
            {
                mItem.Setup(itemData, null);
                mItem.SetFashionMaskShow(false);
            }
            //名称
            mTextName.SafeSetText(itemData.GetColorName());
            //限购提示
            string buyLimitTip = "";
            int buyLimit = -1;
            if (0 != mAccountLimitCount)
            {
                buyLimit = mAccountBuyCount;
                buyLimitTip = string.Format("{0}/{1}", mAccountBuyCount, mAccountLimitCount);
            }
            if (0 != mRoleLimitCount)
            {
                if (buyLimit <= 0 || mRoleBuyCount < mAccountBuyCount)
                    buyLimit = mRoleBuyCount;
                buyLimitTip = string.Format("{0}/{1}", mRoleBuyCount, mRoleLimitCount);
            }
            mTextLimitBuyType.CustomActive(-1 != buyLimit);
            if (-1 != buyLimit)
            {
                string tip = string.Format(TR.Value(buyLimit != mAccountBuyCount ? "shop_item_role_limit_count_tip" 
                            : "shop_item_account_limit_count_tip"), _GetLimitTimeType());
                if (string.IsNullOrEmpty(_GetLimitTimeType()))
                    mTextLimitBuy.transform.position = mTrShort.position;
                else
                    mTextLimitBuy.transform.position = mTrLong.position;
                mTextLimitBuyType.SafeSetText(tip);
                mTextLimitBuy.SafeSetText(buyLimitTip);
            }
            //购买数量显示
            var costItemTable = TableManager.GetInstance().GetTableItem<ItemTable>(mCostItemId);
            if (null != costItemTable)
            {
                mImgCost.SafeSetImage(costItemTable.Icon);
            }
            //描述
            mTextDesp.SafeSetText(mDesp);
            //可购买数量
            mCanBuyMaxNum = 0;
            int curCoinCount = ItemDataManager.GetInstance().GetOwnedItemCount(mCostItemId);
            //折扣价
            int tempDisPrice = 0;
            //可用折扣券数量
            if (mCouponItemCount > 0)
            {
                var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(mCouponItemId);
                tempDisPrice = (int)Math.Floor(Convert.ToDecimal(mCostItemCount * itemTableData.DiscountCouponProp * 1.0f / 100));
            }
            //判断价格上可购买
            while (curCoinCount >= mCostItemCount && (buyLimit < 0 || mCanBuyMaxNum < buyLimit))
            {
                ++mCanBuyMaxNum;
                if (mCanBuyMaxNum <= mCouponItemCount)
                    curCoinCount -= tempDisPrice;
                else
                    curCoinCount -= mCostItemCount;
            }
            //神秘匣子类道具特殊判断
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(mItemId);
            if (null != itemTable)
            {
                if (itemTable.GetLimitNum > 0)
                {
                    var curCount = ItemDataManager.GetInstance().GetOwnedItemCount(mItemId);
                    int limitBuyCount = itemTable.GetLimitNum - curCount;
                    if (mCanBuyMaxNum > limitBuyCount)
                        mCanBuyMaxNum = limitBuyCount;
                }
            }
            if (mCanBuyMaxNum <= 0)
                mCanBuyMaxNum = 1;
            //结束时间
            mEndTimer.CustomActive(0 != mEndTime);
            if (0 != mEndTime)
            {
                mEndTimer.SetCountdown((int)(mEndTime - TimeManager.GetInstance().GetServerTime()));
            }
            //礼包需要显示内容
            mGiftItemList.Clear();
            mGiftItemListScript.CustomActive(false);
            if (itemData.TableData.SubType == ItemTable.eSubType.GiftPackage)
            {
                mNeedGetGiftData = true;
                GiftPackDataManager.GetInstance().GetGiftPackItem(mItemId);
            }
            //额外获得道具
            mImgExtraGotItemIcon.CustomActive(0 != mExtraGotItemId);
            if (0 != mExtraGotItemId)
            {
                var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(mExtraGotItemId);
                if (itemTableData != null)
                {
                    ETCImageLoader.LoadSprite(ref mImgExtraGotItemIcon, itemTableData.Icon);
                }
                mTextExtraGotItemCount.SafeSetText(string.Format("*{0}", mExtraGotItemCount));
            }
            //折扣券
            mObjCoupon.CustomActive(0 != mCouponItemCount);
            if (0 != mCouponItemCount)
            {
                mTextCouponCount.SafeSetText(string.Format("{0}/{1}", 0, mCouponItemCount));
            }
            if (mAccountLimitCount > 0 && (int)mAccountBuyCount <= 0 ||
                mRoleLimitCount > 0 && (int)mRoleBuyCount <= 0)
            {
                mGray.enabled = true;
                mGray.Refresh();
            }
            else
            {
                mGray.enabled = false;
            }
            OnCountChanged(1);
        }
        //获取礼包信息
        private void OnGetGiftData(UIEvent param)
        {
            if (!mNeedGetGiftData)
                return;
            mNeedGetGiftData = false;
            if (param == null || param.Param1 == null)
            {
                Logger.LogError("礼包数据为空");
                return;
            }
            GiftPackSyncInfo data = param.Param1 as GiftPackSyncInfo;
            if ((int)data.id != mItemId)
                return;
            foreach (var item in data.gifts)
            {
                var itemData = ItemDataManager.CreateItemDataFromTable((int)item.itemId);
                if (itemData.IsLevelFit() && itemData.IsOccupationFit())
                    mGiftItemList.Add(item);
            }
            mGiftItemListScript.CustomActive(true);
            if (!mGiftItemListScript.IsInitialised())
            {
                mGiftItemListScript.Initialize();
                mGiftItemListScript.onItemVisiable = OnGiftItemVisiable;
            }
            mGiftItemListScript.SetElementAmount(mGiftItemList.Count);
        }
        private string _GetLimitTimeType()
        {
            switch(mLimitTimeType)
            {
                case EItemLimitType.None:
                    return TR.Value("shop_item_limit_count_forever");
                case EItemLimitType.Monthly:
                    return TR.Value("shop_item_limit_count_monthly");
                case EItemLimitType.Weekly:
                    return TR.Value("shop_item_limit_count_weekly");
                case EItemLimitType.Daily:
                    return TR.Value("shop_item_limit_count_daily");
                default:
                    return "";
            }
        }


        private bool mIsFristNum = false;
        //小键盘输入
        private void OnChangeNum(UIEvent uiEvent)
        {
            int count = mBuyCount;
            ChangeNumType changeNumType = (ChangeNumType)uiEvent.Param1;
            if (changeNumType == ChangeNumType.BackSpace)
            {
                //个位数
                if (count < 10)
                {
                    count = 1;
                    mIsFristNum = true;
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
                if (mIsFristNum)
                {
                    mIsFristNum = false;
                    count = addNum;
                }
                else
                    count = count * 10 + addNum;
                if (count < 1)
                {
                    Logger.LogErrorFormat("tempCurNum is error");
                }

                if (mCanBuyMaxNum < count)
                {
                    count = mCanBuyMaxNum;
                    // SystemNotifyManager.SysNotifyTextAnimation("超出可购买数量");
                }
            }
            else if (changeNumType == ChangeNumType.EnSure)
            {
                //
                if (count < 1)
                {
                    count = 1;
                }
                else if (count > mCanBuyMaxNum)
                {
                    count = mCanBuyMaxNum;
                }
            }

            OnCountChanged(count);
        }
        //数量变化
        public void OnCountChanged(int count)
        {
            mBuyCount = count;
            UpdateCountChanged();
        }
        //数量变化后界面调整
        private void UpdateCountChanged()
        {
            mTextBuyCount.text = mBuyCount.ToString();
            //计算价格
            var totalCost = mBuyCount * mCostItemCount;
            if (0 != mCouponItemCount)
            {
                var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(mCouponItemId);
                int tempDisPrice = (int)Math.Floor(Convert.ToDecimal(mCostItemCount * itemTableData.DiscountCouponProp * 1.0f / 100));
                if (mCouponItemCount >= mBuyCount)
                    totalCost = tempDisPrice * mBuyCount;
                else
                    totalCost = tempDisPrice * mCouponItemCount + (mBuyCount - mCouponItemCount) * mCostItemCount;
                mTextCouponCount.SafeSetText(string.Format("{0}/{1}", mBuyCount, mCouponItemCount));
            }
            mTextTotalCost.text = totalCost.ToString();
            if (mCanBuyMaxNum < 1)
            {
                mGrayReduce.enabled = true;
                mBtnReduce.interactable = false;

                mGrayAdd.enabled = true;
                mBtnAdd.interactable = false;
            }
            else
            {
                if ((mCanBuyMaxNum < 0) || mBuyCount >= mCanBuyMaxNum)
                {
                    mGrayAdd.enabled = true;
                    mBtnAdd.interactable = false;
                }
                else
                {
                    mGrayAdd.enabled = false;
                    mBtnAdd.interactable = true;
                }

                if (mBuyCount <= 1)
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
            if (mBuyCount >= mCanBuyMaxNum)
                return;

            OnCountChanged(mBuyCount + 1);
        }
        //-号
        public void OnClickReduce()
        {
            if (mBuyCount <= 1)
                return;

            OnCountChanged(mBuyCount - 1);
        }
        //点击打开小键盘
        public void OnClickOpenInputFrame()
        {
            mIsFristNum = true;
            ClientSystemManager.GetInstance().OpenFrame<VirtualKeyboardFrame>();
        }
        //点击购买
        public void OnClickBuy()
        {
            switch(mType)
            {
                case ShopItemType.Mall:
                    _BuyMallItem();
                    return;
                case ShopItemType.Shop:
                    _BuyShopItem();
                    return;
                case ShopItemType.AccountShop:
                    _BuyAccountShopItem();
                    return;
                default:
                    Logger.LogErrorFormat("商城对应信息错误，ShopItemType = {0}", mType);
                    return;
            }
        }
    }

    //商品类型
    public enum ShopItemType
    {
        None,
        //商城
        Mall,
        //商店
        Shop,
        //账号商店
        AccountShop,
    }
}
