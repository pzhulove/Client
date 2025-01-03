using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Scripts.UI;

namespace GameClient
{
    [Serializable]
    public class HeadPortraitTabDataModel
    {
        public int index;
        public HeadPortraitTabType tabType;
        public string tabName;
    }

    public class HeadPortraitFrameView : MonoBehaviour,IDisposable
    {
        [SerializeField]private List<HeadPortraitTabDataModel> mMainTabList;
        [SerializeField]private ComUIListScript mMainTabUIList;
        [SerializeField]private ComUIListScript mHeadPortraitItemUIList;
        [SerializeField]private GameObject mGoHeadPortraitInfomation;

        private void Awake()
        {
            InitHeadPortraitItemUIList();

            InitHeadPortraitInfoMation();
        }

        private List<HeadPortraitItemData> mSelfItemDataList = new List<HeadPortraitItemData>();
        private OnHeadPortraitTabItemClick mOnHeadPortraitTabItemClick;
        private HeadPortraitInfoMationView mHeadPortraitInfoMationView = null;
        public void InitView(OnHeadPortraitTabItemClick onHeadPortraitTabItemClick)
        {
            mOnHeadPortraitTabItemClick = onHeadPortraitTabItemClick;
            InitMainTabUIList();
        }

        #region  HeadPortraitItemUIList

        private int mCurSelectHeadPortraitItemID = 0;
        private int mCurSelectHeadPortraitItemIndex = 0; // 当前选择头像框索引
        private void InitHeadPortraitItemUIList()
        {
            if (mHeadPortraitItemUIList != null)
            {
                mHeadPortraitItemUIList.Initialize();
                mHeadPortraitItemUIList.onBindItem += OnBindItemDelegate;
                mHeadPortraitItemUIList.onItemVisiable += OnItemVisiableDelegate;
                mHeadPortraitItemUIList.OnItemUpdate += OnItemVisiableDelegate;
                mHeadPortraitItemUIList.onItemSelected += OnItemSelectedDelegate;
                mHeadPortraitItemUIList.onItemChageDisplay += OnItemChangeDisplayDelegate;

                mHeadPortraitItemUIList.SetElementAmount(0);
            }
        }

        private void UnInitHeadPortraitItemUIList()
        {
            if (mHeadPortraitItemUIList != null)
            {
                mHeadPortraitItemUIList.onBindItem -= OnBindItemDelegate;
                mHeadPortraitItemUIList.onItemVisiable -= OnItemVisiableDelegate;
                mHeadPortraitItemUIList.OnItemUpdate -= OnItemVisiableDelegate;
                mHeadPortraitItemUIList.onItemSelected -= OnItemSelectedDelegate;
                mHeadPortraitItemUIList.onItemChageDisplay -= OnItemChangeDisplayDelegate;
            }

            mHeadPortraitItemUIList = null;
        }

        private HeadPortraitItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<HeadPortraitItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var headPortraitItem = item.gameObjectBindScript as HeadPortraitItem;
            if (headPortraitItem == null)
            {
                return;
            }

            if (item.m_index >= 0 && item.m_index < mSelfItemDataList.Count)
            {
                headPortraitItem.OnItemVisiable(mSelfItemDataList[item.m_index]);
            }
        }

        private void OnItemSelectedDelegate(ComUIListElementScript item)
        {
            var headPortraitItem = item.gameObjectBindScript as HeadPortraitItem;
            if (headPortraitItem == null)
            {
                return;
            }

            if (headPortraitItem.HeadPortraitItemData.itemID == mCurSelectHeadPortraitItemID)
            {
                return;
            }

            mCurSelectHeadPortraitItemIndex = item.m_index;
            mCurSelectHeadPortraitItemID = headPortraitItem.HeadPortraitItemData.itemID;

            RefreshHeadPortraitInfoMation(headPortraitItem.HeadPortraitItemData);
        }

        private void OnItemChangeDisplayDelegate(ComUIListElementScript item,bool bSelected)
        {
            var headPortraitItem = item.gameObjectBindScript as HeadPortraitItem;
            if (headPortraitItem == null)
            {
                return;
            }

            headPortraitItem.OnItemChangeDisplay(bSelected);
        }

        #endregion

        #region  MainTabUIList

        private void InitMainTabUIList()
        {
            if (mMainTabUIList != null)
            {
                mMainTabUIList.Initialize();
                mMainTabUIList.onBindItem += OnMainTabBindItemDelegate;
                mMainTabUIList.onItemVisiable += OnMainTabItemVisiableDelegate;

                mMainTabUIList.SetElementAmount(mMainTabList.Count);
            }
        }

        private void UnInitMainTabUIList()
        {
            if (mMainTabUIList != null)
            {
                mMainTabUIList.onBindItem -= OnMainTabBindItemDelegate;
                mMainTabUIList.onItemVisiable -= OnMainTabItemVisiableDelegate;
            }

            mMainTabUIList = null;
        }

        private HeadPortraitTabItem OnMainTabBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<HeadPortraitTabItem>();
        }

        private void OnMainTabItemVisiableDelegate(ComUIListElementScript item)
        {
            var headPortraitTabItem = item.gameObjectBindScript as HeadPortraitTabItem;
            if (headPortraitTabItem == null)
            {
                return;
            }

            if (item.m_index >= 0 && item.m_index < mMainTabList.Count)
            {
                if (mMainTabList[item.m_index].index == 0)
                {
                    headPortraitTabItem.InitTabItem(mMainTabList[item.m_index], mOnHeadPortraitTabItemClick, true);
                }
                else
                {
                    headPortraitTabItem.InitTabItem(mMainTabList[item.m_index], mOnHeadPortraitTabItemClick, false);
                }
            }
        }

        #endregion

        private void InitHeadPortraitInfoMation()
        {
            if (mGoHeadPortraitInfomation != null)
            {
                var uiPrefabWrapper = mGoHeadPortraitInfomation.GetComponent<UIPrefabWrapper>();
                if (uiPrefabWrapper != null)
                {
                    var go = uiPrefabWrapper.LoadUIPrefab();

                    Utility.AttachTo(go, mGoHeadPortraitInfomation);

                    if (mHeadPortraitInfoMationView == null)
                    {
                        mHeadPortraitInfoMationView = go.GetComponent<HeadPortraitInfoMationView>();
                    }
                }
            }
        }

        public void UpdateHeadProtraitItem(List<HeadPortraitItemData> list,bool isResetIndex = false)
        {
            mCurSelectHeadPortraitItemID = 0;
            if (isResetIndex)
            {
                mCurSelectHeadPortraitItemIndex = 0;
            }
           
            mSelfItemDataList = new List<HeadPortraitItemData>();
            mSelfItemDataList = list;
            mSelfItemDataList.Sort();

            if (mHeadPortraitItemUIList != null)
            {
                mHeadPortraitItemUIList.ResetSelectedElementIndex();

                mHeadPortraitItemUIList.SetElementAmount(mSelfItemDataList.Count);

                mHeadPortraitItemUIList.SelectElement(mCurSelectHeadPortraitItemIndex);

                if (mHeadPortraitInfoMationView != null)
                {
                    mHeadPortraitInfoMationView.SetGameobjectRoot(mSelfItemDataList.Count > 0);
                }
            }
        }

        private void RefreshHeadPortraitInfoMation(HeadPortraitItemData itemData)
        {
            if (mHeadPortraitInfoMationView != null)
            {
                mHeadPortraitInfoMationView.RefreshHeadPortraitInfoMation(itemData);
            }
        }

        public void Dispose()
        {
            mOnHeadPortraitTabItemClick = null;
            mHeadPortraitInfoMationView = null;
            mCurSelectHeadPortraitItemIndex = 0;
            mCurSelectHeadPortraitItemID = 0;
            UnInitHeadPortraitItemUIList();
            UnInitMainTabUIList();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}

