using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    //道具商城View
    public class MallNewPropertyMallView : MallNewBaseView
    {
        private bool mIsAlreadyInit = false;

        private MallTypeTable mMallTypeTable = null;

        private int _mallTypeTableId = 0;
        
        private List<MallItemInfo> _propertyMallElementDataModelList = new List<MallItemInfo>();

        [HeaderAttribute("TabDataList")]
        [SerializeField] private CommonTabToggleGroup mToggleGroup;
        [SerializeField]
        private List<CommonTabData> propertyMallTabDataModelList = new List<CommonTabData>();

        [Space(20)]
        [SerializeField] private ComUIListScript propertyMallElementList;

        #region Awake InitData
        private void Awake()
        {
            mIsAlreadyInit = false;
            mMallTypeTable = null;
            BindUiEventSystem();
        }

        private void BindUiEventSystem()
        {

            if (propertyMallElementList != null)
            {
                propertyMallElementList.Initialize();
                propertyMallElementList.onItemVisiable += OnElementItemVisible;
                propertyMallElementList.onItemSelected += OnElementItemSelect;
            }
        }

        private void OnDestroy()
        {
            UnBindUiEventSystem();
            ClearData();
        }

        private void ClearData()
        {
            _mallTypeTableId = 0;

            mMallTypeTable = null;

            _propertyMallElementDataModelList = null;
        }

        private void UnBindUiEventSystem()
        {

            if (propertyMallElementList != null)
            {
                propertyMallElementList.onItemVisiable -= OnElementItemVisible;
                propertyMallElementList.onItemSelected -= OnElementItemSelect;
            }
        }
        #endregion


        #region OnEnable Sync
        private void OnEnable()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSyncWorldMallQueryItems, OnSyncWorldMallQueryItem);
            if (mIsAlreadyInit == true)
                UpdatePropertyMallElementListByOnEnable();
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSyncWorldMallQueryItems, OnSyncWorldMallQueryItem);
        }

        private void OnSyncWorldMallQueryItem(UIEvent uiEvent)
        {
            if(mMallTypeTable == null || uiEvent == null || uiEvent.Param1 == null)
                return;
            var mallType = (int)uiEvent.Param1;
            if(mallType != (int)mMallTypeTable.MallType)
                return;

            _propertyMallElementDataModelList =
                MallNewDataManager.GetInstance().GetMallItemInfoList((int) mMallTypeTable.MallType);
            //默认选中
            mCurSelectIndex = 0;
            if (0 != mDefaultItemId)
            {
                for (int index = 0; index < _propertyMallElementDataModelList.Count; ++index)
                {
                    if (_propertyMallElementDataModelList[index].id == mDefaultItemId)
                    {
                        mCurSelectIndex = index;
                        break;
                    }
                }
                mDefaultItemId = 0;
            }
            ShowPropertyMallElementList();
            if (mCurSelectIndex >= 0 && mCurSelectIndex < _propertyMallElementDataModelList.Count)
                InitMallContent(_propertyMallElementDataModelList[mCurSelectIndex]);
        }

        private void UpdatePropertyMallElementListByOnEnable()
        {
            mMallTypeTable = GetMallTypeTableByType(_mallTypeTableId);
            if (mMallTypeTable == null)
            {
                return;
            }

            _propertyMallElementDataModelList = MallNewDataManager.GetInstance().GetMallItemInfoList((int)mMallTypeTable.MallType);
            mCurSelectIndex = 0;
            ShowPropertyMallElementList();
        }
        
        #endregion

        private int mDefaultItemId = 0;
        public override void InitData(int index, int secondIndex = 0, int thirdIndex = 0)
        {
            if(mIsAlreadyInit == true)
                return;
            mIsAlreadyInit = true;
            if (null != mToggleGroup)
            {
                mToggleGroup.InitComTab(_ToggleClick, -1, propertyMallTabDataModelList);
            }
            int selectedIndex = index;
            mDefaultItemId = thirdIndex;
            if (propertyMallTabDataModelList != null
                && propertyMallTabDataModelList.Count > 0)
            {
                if (selectedIndex >= propertyMallTabDataModelList.Count)
                {
                    selectedIndex = 0;
                }
                mToggleGroup.SelectTabIndex(selectedIndex);
            }
        }

        #region Tab
        //道具商城的tab页签
        private void _ToggleClick(CommonTabData data)
        {
            OnTabClicked(data.id);
        }

        //道具商城tab 选中回调
        private void OnTabClicked(int mallTypeTableId)
        {
            _mallTypeTableId = mallTypeTableId;
            Utility.DoStartFrameOperation("MallNewPropertyMallView",string.Format("MallTypeTableId/{0}", mallTypeTableId));
            mCurSelectIndex = 0;
            UpdatePropertyMallElementItemList();
        }
        #endregion

        #region PropertyMallElement
        private void UpdatePropertyMallElementItemList()
        {

            ResetPropertyMallElementList();
            mMallTypeTable = GetMallTypeTableByType(_mallTypeTableId);
            if (mMallTypeTable == null)
            {
                return;
            }

            // bugfix: 不为零某些情况下_propertyMallElementDataModelList为null，则不会默认选中，这里强制发送请求使默认选中一定执行
            if (mDefaultItemId != 0)
            {
                MallNewDataManager.GetInstance().SendWorldMallQueryItemReq(mMallTypeTable.ID);
            }
            else
            {
                _propertyMallElementDataModelList = MallNewDataManager.GetInstance().GetMallItemInfoList((int)mMallTypeTable.MallType);

                if (_propertyMallElementDataModelList == null)
                {
                    MallNewDataManager.GetInstance().SendWorldMallQueryItemReq(mMallTypeTable.ID);
                }
                else
                {
                    ShowPropertyMallElementList();
                }
            }
        }

        private void ShowPropertyMallElementList()
        {
            if(propertyMallElementList == null
               || _propertyMallElementDataModelList == null)
                return;

            propertyMallElementList.SetElementAmount(_propertyMallElementDataModelList.Count);
            propertyMallElementList.MoveElementInScrollArea(mCurSelectIndex, true);
            propertyMallElementList.SelectElement(mCurSelectIndex);
        }

        private void ResetPropertyMallElementList()
        {
            propertyMallElementList.SetElementAmount(0);
        }
        
        //道具商城的Element元素
        private int mCurSelectIndex = 0;
        private void OnElementItemVisible(ComUIListElementScript item)
        {
            if(item == null)
                return;

            if(propertyMallElementList == null)
                return;

            if(_propertyMallElementDataModelList == null
               || _propertyMallElementDataModelList.Count <= 0)
                return;

            if (item.m_index < 0 || item.m_index >= _propertyMallElementDataModelList.Count)
                return;

            var propertyMallElementItem = item.GetComponent<MallNewPropertyMallElementItem>();
            var propertyMallElementDataModel = _propertyMallElementDataModelList[item.m_index];
            if (propertyMallElementItem != null
                && propertyMallElementDataModel != null)
            {
                propertyMallElementItem.InitData(propertyMallElementDataModel);
                propertyMallElementItem.SetSelect(item.m_index == mCurSelectIndex);
            }
        }
        
        //道具商城选中
        private void OnElementItemSelect(ComUIListElementScript item)
        {
            var elementItem = item.GetComponent<MallNewPropertyMallElementItem>();
            if (elementItem != null && item.m_index >= 0 && item.m_index < _propertyMallElementDataModelList.Count)
            {
                var selectItem = propertyMallElementList.GetElemenet(mCurSelectIndex);
                if (null != selectItem)
                {
                    var selectElementItem = selectItem.GetComponent<MallNewPropertyMallElementItem>();
                    if (null != selectElementItem)
                        selectElementItem.SetSelect(false);
                }
                elementItem.SetSelect(true);
                InitMallContent(_propertyMallElementDataModelList[item.m_index]);
                mCurSelectIndex = item.m_index;
            }
        }
        [SerializeField] private MallNewItemContent mContent;
        private void InitMallContent(MallItemInfo itemData)
        {
            mContent.OnInitMallItem(itemData);
        }
        #endregion


        //对应商城表中的Data
        private MallTypeTable GetMallTypeTableByType(int mallTypeTableId)
        {
            var mallTypeTable = TableManager.GetInstance().GetTableItem<MallTypeTable>(mallTypeTableId);
            return mallTypeTable;
        }

    }
}