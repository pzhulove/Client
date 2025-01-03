using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class MallNewRecommendMallView : MallNewBaseView
    {
        [SerializeField] private ComUIListScript mLeftListScript;
        [SerializeField] private ComUIListScript mRightListScript;
        [SerializeField] private List<RecommendLeftItem> mRightBottomItems;
        [SerializeField] private Button mButtonNext;
        [SerializeField] private Button mButtonPrevious;
        [SerializeField] private List<GameObject> mObjMaskRootList = new List<GameObject>();
        [SerializeField] private List<GameObject> mObjMaskList = new List<GameObject>();
        [SerializeField] private float mListMoveTime = 0.5f;
        private bool mIsInitShopItem = false;
        private bool mIsInitAccountShopItem = false;
        private bool mIsInitMallItem = false;
        private int mCurDisplayId = 0;
        private List<MallRecommendPageInfo> mLeftPageInfoList = new List<MallRecommendPageInfo>();
        private List<MallRecommendPageInfo> mRightPageInfoList = new List<MallRecommendPageInfo>();
        private List<MallRecommendPageInfo> mBottomPageInfoList = new List<MallRecommendPageInfo>();
        
        private void Awake()
        {
            BindUIEventSystem();
        }

        private void BindUIEventSystem()
        {
            if (null != mLeftListScript)
            {
                mLeftListScript.Initialize();
                mLeftListScript.onItemVisiable += onLeftItemShow;
                mLeftListScript.onItemSelected += onLeftItemSelect;
                mLeftListScript.OnItemEndDrag += onLeftItemEndDrag;
            }
            if (null != mRightListScript)
            {
                mRightListScript.Initialize();
                mRightListScript.onItemVisiable += onRightItemShow;
                mRightListScript.OnItemUpdate += onRightItemShow;
                mRightListScript.onItemSelected += onRightItemSelect;
            }
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GetCommendShopItemSucc, OnSyncShopItemSucc);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GetCommendAccountShopItemSucc, OnSyncAccountShopItemSucc);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GetCommendMallItemSucc, OnSyncWorldMallQueryItem);
        }

        private void OnDestroy()
        {
            ClearData();
        }

        private void ClearData()
        {
            if (null != mLeftListScript)
                mLeftListScript.UnInitialize();
            if (null != mRightListScript)
                mRightListScript.UnInitialize();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GetCommendShopItemSucc, OnSyncShopItemSucc);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GetCommendAccountShopItemSucc, OnSyncAccountShopItemSucc);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GetCommendMallItemSucc, OnSyncWorldMallQueryItem);
        }

        public sealed override void InitData(int index, int secondIndex = 0, int thirdIndex = 0)
        {
            ShopNewDataManager.GetInstance().ReqWorldMallRecommendPageList();
        }

        //展示列表道具
        private void _ShowListItem()
        {
            //有一个没有准备好就不显示
            if (!mIsInitMallItem || !mIsInitShopItem || !mIsInitAccountShopItem)
                return;
            mLeftPageInfoList = ShopNewDataManager.GetInstance().GetRecommendPageInfoList(MallRecommendPageTable.eRecommendType.RECOMMEND_TYPE_GIFT1);
            mRightPageInfoList = ShopNewDataManager.GetInstance().GetRecommendPageInfoList(MallRecommendPageTable.eRecommendType.RECOMMEND_TYPE_ITEM);
            mBottomPageInfoList = ShopNewDataManager.GetInstance().GetRecommendPageInfoList(MallRecommendPageTable.eRecommendType.RECOMMEND_TYPE_GIFT2);
            mBottomPageInfoList.AddRange(ShopNewDataManager.GetInstance().GetRecommendPageInfoList(MallRecommendPageTable.eRecommendType.RECOMMEND_TYPE_GIFT3));
            mLeftListScript.SetElementAmount(mLeftPageInfoList.Count);
            _CheackRightItemList();
            mRightListScript.SetElementAmount(mRightPageInfoList.Count);
            if (mRightBottomItems.Count >= 2)
            {
                mRightBottomItems[0].CustomActive(mBottomPageInfoList.Count > 0);
                if (mBottomPageInfoList.Count > 0)
                    mRightBottomItems[0].OnInit(mBottomPageInfoList[0]);
                mRightBottomItems[1].CustomActive(mBottomPageInfoList.Count > 1);
                if (mBottomPageInfoList.Count > 1)
                    mRightBottomItems[1].OnInit(mBottomPageInfoList[1]);
            }
            for (int index = 0; index < mObjMaskRootList.Count; ++index)
            {
                mObjMaskRootList[index].CustomActive(index < mLeftPageInfoList.Count);
            }
        }

        //删除可购买次数为0的道具
        private void _CheackRightItemList()
        {
            for (int index = mRightPageInfoList.Count - 1; index >= 0; --index)
            {
                var info = mRightPageInfoList[index];
                var itemData = ShopNewDataManager.GetInstance().GetRecommendItemInfo(info);
                int mAccountBuyCount = 0;
                int mAccountLimitCount = 0;
                int mRoleLimitCount = 0;
                int mRoleBuyCount = 0;
                if (info.itemBelongMallType == (byte)MallRecommendPageTable.eItemBelongMallType.ITEM_BELONG_MALL_TYPE_MALL)
                {
                    MallItemInfo item = itemData as MallItemInfo;
                    mAccountBuyCount = (int)item.accountRestBuyNum;
                    mAccountLimitCount = (int)item.accountLimitBuyNum;
                    mRoleLimitCount = item.limitnum > item.limittotalnum ? item.limitnum : item.limittotalnum;
                    mRoleBuyCount = mRoleLimitCount - CountDataManager.GetInstance().GetCount(item.id.ToString());
                }
                //普通商店
                else if (info.itemBelongMallType == (byte)MallRecommendPageTable.eItemBelongMallType.ITEM_BELONG_MALL_TYPE_NORMAL_SHOP)
                {
                    ProtoShopItem item = itemData as ProtoShopItem;
                    mAccountBuyCount = 0;
                    mAccountLimitCount = 0;
                    mRoleBuyCount = item.restNum;
                    var shopTable = TableManager.GetInstance().GetTableItem<ShopItemTable>((int)item.shopItemId);
                    mRoleLimitCount = shopTable.NumLimite;
                }
                //账号商店
                else if (info.itemBelongMallType == (byte)MallRecommendPageTable.eItemBelongMallType.ITEM_BELONG_MALL_TYPE_ACCOUNT_SHOP)
                {
                    AccountShopItemInfo item = itemData as AccountShopItemInfo;
                    mAccountBuyCount = (int)item.accountRestBuyNum;
                    mAccountLimitCount = (int)item.accountLimitBuyNum;
                    mRoleBuyCount = (int)item.roleRestBuyNum;
                    mRoleLimitCount = (int)item.roleLimitBuyNum;
                }
                if (mAccountLimitCount > 0 && (int)mAccountBuyCount <= 0 ||
                    mRoleLimitCount > 0 && (int)mRoleBuyCount <= 0)
                {
                    mRightPageInfoList.Remove(info);
                    continue;
                }
            }
        }

        //设置标记
        private void _SetMaskStatus()
        {
            mButtonNext.CustomActive(mLeftPageInfoList.Count > 1 && mCurDisplayId != mLeftPageInfoList.Count - 1);
            mButtonPrevious.CustomActive(mLeftPageInfoList.Count > 1 && mCurDisplayId != 0);
            for (int index = 0; index < mObjMaskList.Count; ++index)
                mObjMaskList[index].CustomActive(mCurDisplayId == index);
        }

        //左推荐页的页签
        private void onLeftItemShow(ComUIListElementScript item)
        {
            if (item == null || item.m_index >= mLeftPageInfoList.Count)
                return;
            var script = item.GetComponent<RecommendLeftItem>();
            if (null != script)
            {
                script.OnInit(mLeftPageInfoList[item.m_index]);
            }
            mCurDisplayId = item.m_index;
            _SetMaskStatus();
        }

        private void onLeftItemSelect(ComUIListElementScript item)
        {
            if (item == null || item.m_index >= mLeftPageInfoList.Count)
                return;
            var script = item.GetComponent<RecommendLeftItem>();
            if (null != script)
            {
                script.OnClick();
            }
        }

        private void onLeftItemEndDrag()
        {
            if (mCurDisplayId >= mLeftPageInfoList.Count || mCurDisplayId < 0)
                return;
            // mLeftListScript.MoveElementInScrollArea(mCurDisplayId, true);
            mLeftListScript.ScrollToItem(mCurDisplayId, ComUIListScript.ParkingDirection.ParkingLeftOrTop, true, mListMoveTime);
        }

        //右侧推荐的页签
        private void onRightItemShow(ComUIListElementScript item)
        {
            if (item == null || item.m_index >= mRightPageInfoList.Count)
                return;
            var script = item.GetComponent<RecommendRightItem>();
            if (null != script)
            {
                script.OnInit(mRightPageInfoList[item.m_index]);
            }
        }

        private void onRightItemSelect(ComUIListElementScript item)
        {
            if (item == null || item.m_index >= mRightPageInfoList.Count)
                return;
            var script = item.GetComponent<RecommendRightItem>();
            if (null != script)
            {
                script.OnClick();
            }
        }

        //同步商城道具
        private void OnSyncWorldMallQueryItem(UIEvent uiEvent)
        {
            mIsInitMallItem = true;
            _ShowListItem();
        }

        //同步账号商店道具
        private void OnSyncAccountShopItemSucc(UIEvent uiEvent)
        {
            mIsInitAccountShopItem = true;
            _ShowListItem();
        }

        //同步商店道具
        private void OnSyncShopItemSucc(UIEvent uiEvent)
        {
            mIsInitShopItem = true;
            _ShowListItem();
        }
    
        //向左移动
        public void OnButtonPreviousClick()
        {
            // mLeftListScript.MoveElementInScrollArea(mCurDisplayId - 1, true);
            mLeftListScript.ScrollToItem(mCurDisplayId - 1, ComUIListScript.ParkingDirection.ParkingLeftOrTop, true, mListMoveTime);
        }

        //向右移动
        public void OnButtonNextClick()
        {
            // mLeftListScript.MoveElementInScrollArea(mCurDisplayId + 1, true);
            mLeftListScript.ScrollToItem(mCurDisplayId + 1, ComUIListScript.ParkingDirection.ParkingLeftOrTop, true, mListMoveTime);
        }
    
    }
}
