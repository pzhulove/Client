using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class AuctionNewFirstLayerTabItem : MonoBehaviour
    {

        private bool _isSelected = false;
        private AuctionNewMenuTabDataModel _firstLayerMenuTabDataModel = null;
        private bool _isOwnerChildren = false;     //不存在子节点
        private bool _isInitChildrenLayer = false;  //子层级是否进行了初始化

        private OnFirstLayerTabToggleClick _onFirstLayerTabToggleClick;
        private OnSecondLayerTabToggleClick _onSecondLayerTabToggleClick;
        private int _defaultSecondLayerTabId;

        private List<AuctionNewMenuTabDataModel> _secondLayerTabDataModelList;
        private List<AuctionNewSecondLayerTabItem> _secondLayerTabItemList = new List<AuctionNewSecondLayerTabItem>();


        [Space(10)]
        [HeaderAttribute("FirstLayerTab")]
        [Space(5)]
        [SerializeField] private Text tabName;

        [SerializeField] private Text selectedTabName;
        [SerializeField] private Toggle tabToggle;
        [SerializeField] private GameObject arrowRoot;
        [SerializeField] private Image arrowUp;
        [SerializeField] private Image arrowDown;

        [Space(10)] [HeaderAttribute("SecondLayerInfo")]
        [Space(5)]
        [SerializeField] private GameObject secondMenuRoot;
        [SerializeField] private GameObject secondLayerTabItemGo;

        private void Awake()
        {
            if (tabToggle != null)
            {
                tabToggle.onValueChanged.RemoveAllListeners();
                tabToggle.onValueChanged.AddListener(OnTabClicked);
            }
        }

        private void OnDestroy()
        {
            if (tabToggle != null)
                tabToggle.onValueChanged.RemoveAllListeners();

            ResetData();
        }

        private void ResetData()
        {
            _isSelected = false;
            _isOwnerChildren = false;
            _isInitChildrenLayer = false;
            _firstLayerMenuTabDataModel = null;
            _onFirstLayerTabToggleClick = null;
            _onSecondLayerTabToggleClick = null;
            _secondLayerTabItemList.Clear();
            _defaultSecondLayerTabId = 0;
            _secondLayerTabDataModelList = null;
        }

        public void InitTabItem(AuctionNewMenuTabDataModel firstLayerMenuTabDataModel,
            List<AuctionNewMenuTabDataModel> secondLayerTabDataModelList,
            bool isSelected = false,
            int defaultSecondLayerTabId = 0,
            OnFirstLayerTabToggleClick onFirstLayerTabToggleClick = null,
            OnSecondLayerTabToggleClick onSecondLayerTabToggleClick = null
            )
        {

            //InitData
            //首先数据的重置
            ResetData();

            _firstLayerMenuTabDataModel = firstLayerMenuTabDataModel;
            _secondLayerTabDataModelList = secondLayerTabDataModelList;
            _defaultSecondLayerTabId = defaultSecondLayerTabId;

            _onFirstLayerTabToggleClick = onFirstLayerTabToggleClick;
            _onSecondLayerTabToggleClick = onSecondLayerTabToggleClick;

            if(_firstLayerMenuTabDataModel == null)
                return;
            if(_firstLayerMenuTabDataModel.AuctionNewFrameTable == null)
                return;

            _isOwnerChildren = false;
            //存在二级页签的列表
            if (_secondLayerTabDataModelList != null && _secondLayerTabDataModelList.Count > 0)
            {
                _isOwnerChildren = true;
            }

            //InitView
            if (tabName != null)
                tabName.text = _firstLayerMenuTabDataModel.AuctionNewFrameTable.Name;

            if (selectedTabName != null)
                selectedTabName.text = _firstLayerMenuTabDataModel.AuctionNewFrameTable.Name;

            InitArrowUp();
            
            if (isSelected == true)
            {
                if (tabToggle != null)
                {
                    tabToggle.isOn = true;
                }
            }
        }
        

        private void OnTabClicked(bool value)
        {
            if (_firstLayerMenuTabDataModel == null)
                return;

            //避免重复点击时，View重复更新
            if (_isSelected == value)
                return;
            _isSelected = value;

            if (value == true)
            {
                if (_onFirstLayerTabToggleClick != null)
                {
                    _onFirstLayerTabToggleClick(_firstLayerMenuTabDataModel);
                }

                if (_isOwnerChildren == true)
                {
                    SetArrowUp(false);
                    SetArrowDown(true);

                    if (secondMenuRoot != null)
                    {
                        if (secondMenuRoot.activeSelf == false)
                            secondMenuRoot.CustomActive(true);
                    }

                    //第一次进行初始化
                    if (_isInitChildrenLayer == false)
                    {
                        InitChildrenLayer();
                        _isInitChildrenLayer = true;                        
                    }
                    else
                    {
                        //再次打开的时候，将二级页签进行点击操作
                        //按钮对应的值进行刷新
                        if (_secondLayerTabItemList != null && _secondLayerTabItemList.Count > 0)
                        {
                            for (var i = 0; i < _secondLayerTabItemList.Count; i++)
                            {
                                var curSecondLayerTabItem = _secondLayerTabItemList[i];
                                if(curSecondLayerTabItem != null)
                                    curSecondLayerTabItem.OnEnabelTabItem();
                            }
                        }
                    }
                }
            }
            else
            {
                //取消选择
                if (_isOwnerChildren == true)
                {
                    SetArrowUp(true);
                    SetArrowDown(false);

                    if (secondMenuRoot != null)
                    {
                        secondMenuRoot.CustomActive(false);
                    }
                }
            }
        }

        #region TabArrow
        private void InitArrowUp()
        {

            if (arrowRoot != null)
            {
                if (_isOwnerChildren == false)
                {
                    arrowRoot.CustomActive(false);
                }
                else
                {
                    arrowRoot.CustomActive(true);
                }
            }

            if (_isOwnerChildren == true)
            {
                SetArrowUp(true);
                SetArrowDown(false);
            }
        }

        private void SetArrowUp(bool flag)
        {
            if (arrowUp != null)
                arrowUp.gameObject.CustomActive(flag);
        }

        private void SetArrowDown(bool flag)
        {
            if (arrowDown != null)
                arrowDown.gameObject.CustomActive(flag);
        }
        #endregion

        #region SecondLayerInfo

        private void InitChildrenLayer()
        {
            _secondLayerTabItemList.Clear();

            if (_secondLayerTabDataModelList == null || _secondLayerTabDataModelList.Count <= 0)
            {
                Logger.LogErrorFormat("SecondLayerTabDataModelList is null and Init is Error");
                return;
            }

            if (secondMenuRoot == null || secondLayerTabItemGo == null)
            {
                Logger.LogErrorFormat("SecondMenuRoot is null or secondLayerTabItem is null");
                return;
            }

            var defaultIndex = 0;
            defaultIndex = GetSecondDefaultIndex(_secondLayerTabDataModelList);

            for (var i = 0; i < _secondLayerTabDataModelList.Count; i++)
            {
                var curChildrenTabDataModel = _secondLayerTabDataModelList[i];
                if(curChildrenTabDataModel == null)
                    continue;

                var isSelected = i == defaultIndex;

                var curChildrenTabItem = Instantiate(secondLayerTabItemGo) as GameObject;
                if (curChildrenTabItem != null)
                {
                    curChildrenTabItem.CustomActive(true);
                    Utility.AttachTo(curChildrenTabItem, secondMenuRoot);

                    var secondLayerTabItem = curChildrenTabItem.GetComponent<AuctionNewSecondLayerTabItem>();
                    if (secondLayerTabItem != null)
                    {
                        secondLayerTabItem.InitTabItem(_firstLayerMenuTabDataModel,
                            curChildrenTabDataModel,
                            isSelected,
                            _onSecondLayerTabToggleClick);
                        _secondLayerTabItemList.Add(secondLayerTabItem);
                    }
                }
            }
        }
        #endregion

        //按照职业类型，默认选中对应职业的页签
        private int GetSecondDefaultIndex(List<AuctionNewMenuTabDataModel> childrenTabDataModelList)
        {
            //首先查找从外界传过来的数值
            if (_defaultSecondLayerTabId > 0)
            {
                for (var i = 0; i < childrenTabDataModelList.Count; i++)
                {
                    var curChildrenTabDataModel = childrenTabDataModelList[i];
                    if(curChildrenTabDataModel == null || curChildrenTabDataModel.AuctionNewFrameTable == null)
                        continue;
                    //找到，第一次打开默认选中某个二级页签
                    if (curChildrenTabDataModel.Id == _defaultSecondLayerTabId)
                    {
                        _defaultSecondLayerTabId = 0;
                        return i;
                    }
                }
            }

            _defaultSecondLayerTabId = 0;
            var defaultIndex = 0;

            //按照职业进行默认选中
            if (_firstLayerMenuTabDataModel.AuctionNewFrameTable.ChooseLogic == 2)
            {
                var curBaseJobId = Utility.GetBaseJobID(PlayerBaseData.GetInstance().JobTableID);
                for (var i = 0; i < childrenTabDataModelList.Count; i++)
                {
                    var curChildrenTabDataModel = childrenTabDataModelList[i];
                    if (curChildrenTabDataModel == null || curChildrenTabDataModel.AuctionNewFrameTable == null)
                        continue;
                    if (curChildrenTabDataModel.AuctionNewFrameTable.JobBaseId == curBaseJobId)
                    {
                        defaultIndex = i;
                        break;
                    }
                }
            }

            return defaultIndex;
        }


    }
}
