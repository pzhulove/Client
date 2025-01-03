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

    //限时商城View
    public class MallNewLimitTimeMallView : MallNewBaseView
    {

        //对应商城表中的ID
        private const int MallTypeTableId = 9;

        private int _curMallType = 0;
        private bool _isAlreadyInit = false;

        private List<MallItemInfo> mTotalMallItemModelList = new List<MallItemInfo>();
        private int mCurSelectSubType = 1;//1为节日 2为限时

        [SerializeField] private ComUIListScript limitTimeMallElementList;
        //限时页签 只有在有限时商品时才会限时
        [SerializeField] private GameObject mObjToggleLimit;
        [SerializeField] private CommonTabToggleGroup mToggleGroup;
        
        private void Awake()
        {
            _isAlreadyInit = false;
            BindUiEventSystem();
        }

        private void BindUiEventSystem()
        {
            
            if (limitTimeMallElementList != null)
            {
                limitTimeMallElementList.Initialize();
                limitTimeMallElementList.onItemVisiable += OnElementItemVisible;
                limitTimeMallElementList.OnItemRecycle += OnItemRecycle;
                limitTimeMallElementList.onItemSelected += OnItemSelect;
            }
        }

        private void OnDestroy()
        {
            UnBindUiEventSystem();
            if (mTotalMallItemModelList != null)
                mTotalMallItemModelList.Clear();
            _curMallType = 0;
            ResetLimitTimeMallElementDataModelList();
        }

        private void ResetLimitTimeMallElementDataModelList()
        {
            if(mTotalMallItemModelList != null)
                mTotalMallItemModelList.Clear();
        }

        private void UnBindUiEventSystem()
        {
            if (limitTimeMallElementList != null)
            {
                limitTimeMallElementList.onItemVisiable -= OnElementItemVisible;
                limitTimeMallElementList.OnItemRecycle -= OnItemRecycle;
                limitTimeMallElementList.onItemSelected -= OnItemSelect;
            }
        }

        private void OnEnable()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSyncWorldMallQueryItems, OnSyncWorldMallQueryItem);
            if (_isAlreadyInit == true)
            {
                UpdateLimitTimeMallElementListByOnEnable();
            }
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSyncWorldMallQueryItems, OnSyncWorldMallQueryItem);
        }

        private int mDefaultItemId = 0;
        public override void InitData(int index, int secondIndex = 0, int thirdIndex = 0)
        {
            if (limitTimeMallElementList == null)
                return;

            if(_isAlreadyInit == true)
                return;
            _isAlreadyInit = true;
            mDefaultItemId = thirdIndex;

            mToggleGroup.InitComTab(_ToggleClick);
            mToggleGroup.CustomActive(2, false);
            _curMallType = GetMallType();
            mCurSelectSubType = 1;
            //初始化，发送协议
            //获取限时商品
            MallNewDataManager.GetInstance().SendWorldMallQueryItemReq(MallTypeTableId, 1);
            //获取节日商品
            MallNewDataManager.GetInstance().SendWorldMallQueryItemReq(MallTypeTableId, 2);
        }

        private void OnSyncWorldMallQueryItem(UIEvent uiEvent)
        {
            if(uiEvent == null)
                return;

            if(uiEvent.Param1 == null)
                return;

            var mallType = (int) uiEvent.Param1;
            if(mallType != _curMallType)
                return;
            //有该类型商品才会显示这个页签
            var list = MallNewDataManager.GetInstance().GetMallItemInfoList(_curMallType, 2);
            mToggleGroup.CustomActive(2, null != list && list.Count > 0);

            mTotalMallItemModelList = MallNewDataManager.GetInstance().GetMallItemInfoList(_curMallType, mCurSelectSubType);
            if (mTotalMallItemModelList != null)
            {
                mTotalMallItemModelList.Sort(_SortList);
            }
            //默认选中
            mCurSelectIndex = 0;
            if (0 != mDefaultItemId)
            {
                for (int index = 0; index < mTotalMallItemModelList.Count; ++index)
                {
                    if (mTotalMallItemModelList[index].id == mDefaultItemId)
                    {
                        mCurSelectIndex = index;
                        break;
                    }
                }
                mDefaultItemId = 0;
            }
            ShowLimitTimeMallElementList();
        }

        private void ShowLimitTimeMallElementList()
        {
            if(mTotalMallItemModelList == null
               || mTotalMallItemModelList.Count <= 0)
            {
                limitTimeMallElementList.SetElementAmount(0);
                InitMallContent(null);
                return;
            }
            
            if (limitTimeMallElementList != null)
            {
                limitTimeMallElementList.SetElementAmount(mTotalMallItemModelList.Count);
                limitTimeMallElementList.MoveElementInScrollArea(mCurSelectIndex, true);
                limitTimeMallElementList.SelectElement(mCurSelectIndex);
            }
        }

        //点击刷新对应列表
        //点击tab事件
        private void _ToggleClick(CommonTabData data)
        {
            mCurSelectSubType = data.id;
            UpdateLimitTimeMallElementListByOnEnable();
        }
        //选中节日礼包
        public void OnClickToggle1(bool value)
        {
            if (value)
            {
                mCurSelectSubType = 1;
                UpdateLimitTimeMallElementListByOnEnable();
            }
        }
        //选中限时礼包
        public void OnClickToggle2(bool value)
        {
            if (value)
            {
                mCurSelectSubType = 2;
                UpdateLimitTimeMallElementListByOnEnable();
            }
        }

        //更新列表
        private void UpdateLimitTimeMallElementListByOnEnable()
        {
            mTotalMallItemModelList = MallNewDataManager.GetInstance().GetMallItemInfoList(_curMallType, mCurSelectSubType);
            if(mTotalMallItemModelList != null)
            {
                mTotalMallItemModelList.Sort(_SortList);
            }
            ShowLimitTimeMallElementList();
        }
        
        private int _SortList(MallItemInfo a, MallItemInfo b)
        {
            if(a.sortIdx > b.sortIdx)
            {
                return 1;
            }
            else if(a.sortIdx < b.sortIdx)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        //限时商城中的Element元素
        private int mCurSelectIndex = 0;
        private void OnElementItemVisible(ComUIListElementScript item)
        {
            if(item == null)
                return;

            if(limitTimeMallElementList == null)
                return;

            if(mTotalMallItemModelList == null
               || mTotalMallItemModelList.Count <= 0)
                return;
            
            if(item.m_index < 0 || item.m_index >= mTotalMallItemModelList.Count)
                return;

            var limitTimeMallElementItem = item.GetComponent<MallNewLimitTimeMallElementItem>();
            var limitTimeMallElementDataModel = mTotalMallItemModelList[item.m_index];

            if (limitTimeMallElementItem != null
                && limitTimeMallElementDataModel != null)
            {
                //初始化限时礼包的数值
                limitTimeMallElementItem.Init(limitTimeMallElementDataModel);
                limitTimeMallElementItem.SetSelect(item.m_index == mCurSelectIndex);
            }
        }

        private void OnItemSelect(ComUIListElementScript item)
        {
            var elementItem = item.GetComponent<MallNewLimitTimeMallElementItem>();
            if (elementItem != null && item.m_index >= 0 && item.m_index < mTotalMallItemModelList.Count)
            {
                var selectItem = limitTimeMallElementList.GetElemenet(mCurSelectIndex);
                if (null != selectItem)
                {
                    var selectElementItem = selectItem.GetComponent<MallNewLimitTimeMallElementItem>();
                    if (null != selectElementItem)
                        selectElementItem.SetSelect(false);
                }
                elementItem.SetSelect(true);
                InitMallContent(mTotalMallItemModelList[item.m_index]);
                mCurSelectIndex = item.m_index;
            }
        }
        [SerializeField] private MallNewItemContent mContent;
        private void InitMallContent(MallItemInfo itemData)
        {
            mContent.OnInitMallItem(itemData);
        }

        private void OnItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (limitTimeMallElementList == null)
                return;
            
            var limitTimeMallElementItem = item.GetComponent<MallNewLimitTimeMallElementItem>();
            if(limitTimeMallElementItem != null)
                limitTimeMallElementItem.Reset();
        }

        private int GetMallType()
        {
            var mallData = TableManager.GetInstance().GetTableItem<MallTypeTable>(MallTypeTableId);
            if (mallData == null)
                return 0;

            return (int)mallData.MallType;
        }

        private int GetSelfBaseJobId()
        {
            var selfBaseId = PlayerBaseData.GetInstance().JobTableID;

            var data = TableManager.GetInstance().GetTableItem<JobTable>(selfBaseId);
            if (data == null)
                return selfBaseId;
            if (data.JobType == 1)
                selfBaseId = data.prejob;

            return selfBaseId;
        }


    }
}