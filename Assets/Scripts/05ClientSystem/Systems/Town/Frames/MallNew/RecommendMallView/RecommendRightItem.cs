using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Protocol;
using ProtoTable;
using LimitTimeGift;
using System;

namespace GameClient
{
    public class RecommendRightItem : MonoBehaviour
    {
        [SerializeField] private ComItem mItem;
        [SerializeField] private Image mImgCost;
        [SerializeField] private Text mTextCostCount;
        [SerializeField] private Text mTextName;
        [SerializeField] private Text mTextLimitBuyCount;
        [SerializeField] private Text mTextLimitTimeType;
        [SerializeField] private GameObject mObjEndTime;
        [SerializeField] private SimpleTimer mEndTimer;
        private MallRecommendPageInfo mInfo;
        private System.Object mItemData;
        //显示的道具id
        private int mItemId;
        //消耗的道具id
        private int mCostItemId;
        //消耗的道具数量
        private int mCostItemCount;
        //角色限购次数
        private int mRoleLimitCount;
        //角色剩余购买次数
        private int mRoleBuyCount;
        //账号限购次数
        private int mAccountLimitCount;
        //账号剩余购买次数
        private int mAccountBuyCount;
        //限购周期类型
        private Utility.EItemLimitType mLimitType;
        //下架时间
        private int mEndTime;

        public void OnInit(MallRecommendPageInfo info)
        {
            mInfo = info;
            mItemData = ShopNewDataManager.GetInstance().GetRecommendItemInfo(mInfo);
            //商城
            if (info.itemBelongMallType == (byte)MallRecommendPageTable.eItemBelongMallType.ITEM_BELONG_MALL_TYPE_MALL)
                _InitMallItem();
            //普通商店
            else if (info.itemBelongMallType == (byte)MallRecommendPageTable.eItemBelongMallType.ITEM_BELONG_MALL_TYPE_NORMAL_SHOP)
                _InitShopItem();
            //账号商店
            else if (info.itemBelongMallType == (byte)MallRecommendPageTable.eItemBelongMallType.ITEM_BELONG_MALL_TYPE_ACCOUNT_SHOP)
                _InitAccountShopItem();
            _InitItem();
        }

        private void _InitMallItem()
        {
            MallItemInfo item = mItemData as MallItemInfo;
            if (null == item)
                return;
            mItemId = (int)item.itemid;
            if (0 == mItemId && null != item.giftItems && item.giftItems.Length > 0)
                mItemId = (int)item.giftItems[0].id;
            mCostItemId = ItemDataManager.GetInstance().GetMoneyIDByType((ItemTable.eSubType)item.moneytype);
            mCostItemCount = (int)item.discountprice;
            mAccountBuyCount = (int)item.accountRestBuyNum;
            mAccountLimitCount = (int)item.accountLimitBuyNum;
            mRoleLimitCount = item.limitnum > item.limittotalnum ? item.limitnum : item.limittotalnum;
            mRoleBuyCount = mRoleLimitCount - CountDataManager.GetInstance().GetCount(item.id.ToString());
            mLimitType = Utility.GetMallItemLimitType(item);
            mEndTime = (int)item.endtime;
        }

        private void _InitShopItem()
        {
            ProtoShopItem item = mItemData as ProtoShopItem;
            if (null == item)
                return;
            int shopItemId = (int)item.shopItemId;
            var shopTable = TableManager.GetInstance().GetTableItem<ShopItemTable>(shopItemId);
            if (null == shopTable)
                return;
            mItemId = shopTable.ItemID;
            mCostItemId = shopTable.CostItemID;
            mCostItemCount = shopTable.CostNum;
            mAccountBuyCount = 0;
            mAccountLimitCount = 0;
            mRoleBuyCount = item.restNum;
            mRoleLimitCount = shopTable.NumLimite;
            mLimitType = Utility.GetShopItemLimitType(shopItemId);
            mEndTime = 0;
        }

        private void _InitAccountShopItem()
        {
            AccountShopItemInfo item = mItemData as AccountShopItemInfo;
            if (null == item)
                return;
            mItemId = (int)item.itemDataId;
            if (null != item.costItems && item.costItems.Length > 0)
            {
                mCostItemId = (int)item.costItems[0].id;
                mCostItemCount = (int)item.costItems[0].num;
            }
            mAccountBuyCount = (int)item.accountRestBuyNum;
            mAccountLimitCount = (int)item.accountLimitBuyNum;
            mRoleBuyCount = (int)item.roleRestBuyNum;
            mRoleLimitCount = (int)item.roleLimitBuyNum;
            mLimitType = Utility.GetAccountShopItemLimitType(item);
            mEndTime = 0;
        }

        private void _InitItem()
        {
            var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mItemId);
            if (null != itemData)
                mItem.Setup(itemData, null);
            mTextName.SafeSetText(itemData.GetColorName());
            //限购提示
            _SetDesp();
            var costItemTable = TableManager.GetInstance().GetTableItem<ItemTable>(mCostItemId);
            if (null != costItemTable)
            {
                mImgCost.SafeSetImage(costItemTable.Icon);
                mTextCostCount.SafeSetText(mCostItemCount.ToString());
            }
            
        }
        //限购提示
        private void _SetDesp()
        {
            //限购次数
            if (0 < mAccountLimitCount)
            {
                mTextLimitBuyCount.CustomActive(true);
                mTextLimitBuyCount.SafeSetText(string.Format(TR.Value("shop_recommend_limit_count_account"), mAccountBuyCount));
            }
            else if (0 < mRoleLimitCount)
            {
                mTextLimitBuyCount.CustomActive(true);
                mTextLimitBuyCount.SafeSetText(string.Format(TR.Value("shop_recommend_limit_count_role"), mRoleBuyCount));
            }
            else
                mTextLimitBuyCount.CustomActive(false);
            //限购类型
            switch(mLimitType)
            {
                case Utility.EItemLimitType.Daily:
                    mTextLimitTimeType.CustomActive(true);
                    mTextLimitTimeType.SafeSetText(TR.Value("shop_recommend_limit_type_dayly"));
                    break;
                case Utility.EItemLimitType.Weekly:
                    mTextLimitTimeType.CustomActive(true);
                    mTextLimitTimeType.SafeSetText(TR.Value("shop_recommend_limit_type_weekly"));
                    break;
                case Utility.EItemLimitType.Monthly:
                    mTextLimitTimeType.CustomActive(true);
                    mTextLimitTimeType.SafeSetText(TR.Value("shop_recommend_limit_type_monthly"));
                    break;
                case Utility.EItemLimitType.Season:
                    mTextLimitTimeType.CustomActive(true);
                    mTextLimitTimeType.SafeSetText(TR.Value("shop_recommend_limit_type_season"));
                    break;
                case Utility.EItemLimitType.Activityly:
                    mTextLimitTimeType.CustomActive(true);
                    mTextLimitTimeType.SafeSetText(TR.Value("shop_recommend_limit_type_activity"));
                    break;
                default:
                    mTextLimitTimeType.CustomActive(false);
                    break;
            }
            //限购时间
            if (!mTextLimitTimeType.gameObject.activeSelf && 0 != mEndTime)
            {
                mObjEndTime.CustomActive(true);
                mEndTimer.SetCountdown((int)(mEndTime - TimeManager.GetInstance().GetServerTime()));
            }
            else
                mObjEndTime.CustomActive(false);
        }

        public void OnClick()
        {
            //如果是链接
            if (mInfo.linkFunctionType == (byte)MallRecommendPageTable.eLinkFunctionType.LINK_FUNCTION_TYPE_LINK)
            {
                ActiveManager.GetInstance().OnClickLinkInfo(mInfo.linkPath);
            }
            //购买
            else if (mInfo.linkFunctionType == (byte)MallRecommendPageTable.eLinkFunctionType.LINK_FUNCTION_TYPE_BUY)
            {
                // 走统一接口
                ShopNewDataManager.GetInstance().OpenBuyFrame(mInfo);
            }
        }
    }
}
