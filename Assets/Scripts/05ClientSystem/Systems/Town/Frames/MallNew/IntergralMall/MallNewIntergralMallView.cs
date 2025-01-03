using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.UI;
using Protocol;
using System;

namespace GameClient
{
    public class MallNewIntergralMallTabData
    {
        public int index;
        public int mallTypeTableId;
    }
    //积分商城
    public class MallNewIntergralMallView : MallNewBaseView
    {
        [SerializeField]private ComUIListScript mIntergralMallTabList;
        [SerializeField]private ComUIListScript mIntergralMallElmentList;

        private List<MallItemInfo> mIntergralMallElementDataModelList = null;
        private List<MallNewIntergralMallTabData> mMallNewIntergralMallTabDataList = null;
        private int mSeletedIndex = 0;
        private int mMallTypeTableId = 0;
        private ProtoTable.MallTypeTable mMallTypeTable = null;

        private void Awake()
        {
            BindUIEventSystem();
        }

        private void OnDestroy()
        {
            ClearData();
        }

        private int mDefaultItemId = 0;
        public sealed override void InitData(int index, int secondIndex = 0, int thirdIndex = 0)
        {
            mMallNewIntergralMallTabDataList = MallNewUtility.GetIntergralMallTabDataModelList();
            if (mMallNewIntergralMallTabDataList == null)
            {
                return;
            }

            mSeletedIndex = index;
            mDefaultItemId = thirdIndex;

            if (mMallNewIntergralMallTabDataList != null && mMallNewIntergralMallTabDataList.Count > 0)
            {
                if (mSeletedIndex >= mMallNewIntergralMallTabDataList.Count)
                {
                    mSeletedIndex = 0;
                }
            }

            mIntergralMallTabList.SetElementAmount(mMallNewIntergralMallTabDataList.Count);
        }

        public sealed override void OnEnableView()
        {
            base.OnEnableView();
        }

        private void ClearData()
        {
            mSeletedIndex = 0;
            mMallTypeTableId = 0;
            mMallNewIntergralMallTabDataList = null;
            mIntergralMallElementDataModelList = null;
            mMallTypeTable = null;

            UnBindUIEventSystem();
        }

        private void BindUIEventSystem()
        {
            if (mIntergralMallTabList != null)
            {
                mIntergralMallTabList.Initialize();
                mIntergralMallTabList.onBindItem += OnBindItemDelegate;
                mIntergralMallTabList.onItemVisiable += OnItemVisiableDelegate;
            }

            if (mIntergralMallElmentList != null)
            {
                mIntergralMallElmentList.Initialize();
                mIntergralMallElmentList.onBindItem += OnBindIntergralMallElementItemDelegate;
                mIntergralMallElmentList.onItemVisiable += OnIntergralMallElementItemVisiableDelegate;
                mIntergralMallElmentList.onItemSelected += OnIntergralMallElementItemSelect;
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSyncWorldMallQueryItems, OnSyncWorldMallQueryItem);
        }

        private void UnBindUIEventSystem()
        {
            if (mIntergralMallTabList != null)
            {
                mIntergralMallTabList.onBindItem -= OnBindItemDelegate;
                mIntergralMallTabList.onItemVisiable -= OnItemVisiableDelegate;
            }

            if (mIntergralMallElmentList != null)
            {
                mIntergralMallElmentList.onBindItem -= OnBindIntergralMallElementItemDelegate;
                mIntergralMallElmentList.onItemVisiable -= OnIntergralMallElementItemVisiableDelegate;
                mIntergralMallElmentList.onItemSelected -= OnIntergralMallElementItemSelect;
            }

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSyncWorldMallQueryItems, OnSyncWorldMallQueryItem);
        }

        private MallNewIntergralMallTabItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<MallNewIntergralMallTabItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var tabItem = item.gameObjectBindScript as MallNewIntergralMallTabItem;
            if (tabItem != null && item.m_index >= 0 && item.m_index < mMallNewIntergralMallTabDataList.Count)
            {
                bool seleted = mSeletedIndex == mMallNewIntergralMallTabDataList[item.m_index].index;
                tabItem.InitData(mMallNewIntergralMallTabDataList[item.m_index], OnTabClick, seleted);
            }
        }

        private void OnTabClick(int mallTypeTableId)
        {
            mMallTypeTableId = mallTypeTableId;

            UpdateIntergralMallElementItemList();
        }

        private void UpdateIntergralMallElementItemList()
        {
            mMallTypeTable = TableManager.GetInstance().GetTableItem<ProtoTable.MallTypeTable>(mMallTypeTableId);
            if (mMallTypeTable == null)
            {
                return;
            }

            mIntergralMallElementDataModelList = MallNewDataManager.GetInstance().GetMallItemInfoList((int)mMallTypeTable.MallType);

            if (mIntergralMallElementDataModelList == null)
            {
                MallNewDataManager.GetInstance().SendWorldMallQueryItemReq(mMallTypeTable.ID);
            }
            else
            {
                SetIntergralMallElementList();
            }
        }

        private void SetIntergralMallElementList()
        {
            mCurSelectIndex = 0;
            if (mIntergralMallElementDataModelList == null)
            {
                return;
            }
            mIntergralMallElmentList.SetElementAmount(mIntergralMallElementDataModelList.Count);
            mIntergralMallElmentList.MoveElementInScrollArea(mCurSelectIndex,true);
            mIntergralMallElmentList.SelectElement(mCurSelectIndex);
        }

        private MallNewPropertyMallElementItem OnBindIntergralMallElementItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<MallNewPropertyMallElementItem>();
        }

        private int mCurSelectIndex = 0;
        private void OnIntergralMallElementItemVisiableDelegate(ComUIListElementScript item)
        {
            MallNewPropertyMallElementItem elementItem = item.gameObjectBindScript as MallNewPropertyMallElementItem;
            if (elementItem != null && item.m_index >= 0 && item.m_index < mIntergralMallElementDataModelList.Count)
            {
                elementItem.InitData(mIntergralMallElementDataModelList[item.m_index]);
                elementItem.SetSelect(item.m_index == mCurSelectIndex);
            }
        }

        private void OnIntergralMallElementItemSelect(ComUIListElementScript item)
        {
            MallNewPropertyMallElementItem elementItem = item.gameObjectBindScript as MallNewPropertyMallElementItem;
            if (elementItem != null && item.m_index >= 0 && item.m_index < mIntergralMallElementDataModelList.Count)
            {
                var selectItem = mIntergralMallElmentList.GetElemenet(mCurSelectIndex);
                if (null != selectItem)
                {
                    var selectElementItem = selectItem.gameObjectBindScript as MallNewPropertyMallElementItem;
                    if (null != selectElementItem)
                        selectElementItem.SetSelect(false);
                }
                elementItem.SetSelect(true);
                InitMallContent(mIntergralMallElementDataModelList[item.m_index]);
                mCurSelectIndex = item.m_index;
            }
        }

        [SerializeField] private MallNewItemContent mContent;
        private void InitMallContent(MallItemInfo itemData)
        {
            mContent.OnInitMallItem(itemData);
        }

        private void OnSyncWorldMallQueryItem(UIEvent uiEvent)
        {
            if (mMallTypeTable == null)
                return;

            if (uiEvent == null)
                return;

            if (uiEvent.Param1 == null)
                return;

            var mallType = (int)uiEvent.Param1;
            if (mallType != (int)mMallTypeTable.MallType)
                return;

            mIntergralMallElementDataModelList =
                MallNewDataManager.GetInstance().GetMallItemInfoList((int)mMallTypeTable.MallType);
            //默认选中
            if (0 != mDefaultItemId)
            {
                for (int index = 0; index < mIntergralMallElementDataModelList.Count; ++index)
                {
                    if (mIntergralMallElementDataModelList[index].id == mDefaultItemId)
                    {
                        mCurSelectIndex = index;
                        break;
                    }
                }
                mDefaultItemId = 0;
            }
            SetIntergralMallElementList();
        }
    }
}