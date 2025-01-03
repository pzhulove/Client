using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;
using Scripts.UI;

namespace GameClient
{
    public class AuctionNewFilterControl : MonoBehaviour
    {
        private AuctionNewFilterData _auctionNewFilterData = null;
        private List<AuctionNewFilterData> _auctionNewFilterDataList = new List<AuctionNewFilterData>();

        private int _defaultSelectedIndex = 0;

        private OnAuctionNewFilterElementItemButtonClick _onAuctionNewfilterElementItemButtonClick;

        private Action _onResetFilterListAction;

        [SerializeField] private Text filterName;
        [SerializeField] private Button filterButton;
        [SerializeField] private Button closeButton;        //关闭掉下拉单

        [SerializeField] private ComUIListScript filterElementItemList;
        [SerializeField] private GameObject filterElementItemListRoot;

        private void Awake()
        {
            BindUiEventSystem();
        }

        private void BindUiEventSystem()
        {
            if (filterButton != null)
            {
                filterButton.onClick.RemoveAllListeners();
                filterButton.onClick.AddListener(OnFilterButtonClick);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseButtonClick);
            }

            if (filterElementItemList != null)
            {
                filterElementItemList.Initialize();
                filterElementItemList.onItemVisiable += OnFilterElementItemVisible;
                filterElementItemList.OnItemUpdate += OnFilterElementItemUpdate;
            }
        }

        private void OnDestroy()
        {
            UnBindUiEventSystem();
            ClearData();
        }

        private void UnBindUiEventSystem()
        {
            if (filterButton != null)
                filterButton.onClick.RemoveAllListeners();

            if (closeButton != null)
                closeButton.onClick.RemoveAllListeners();

            if (filterElementItemList != null)
            {
                filterElementItemList.onItemVisiable -= OnFilterElementItemVisible;
                filterElementItemList.OnItemUpdate -= OnFilterElementItemUpdate;
            }

        }

        private void ClearData()
        {
            _defaultSelectedIndex = 0;
            _auctionNewFilterData = null;
            _auctionNewFilterDataList = null;

            _onResetFilterListAction = null;
        }

        public void InitFilterControl(AuctionNewFilterData auctionNewFilterData,
            OnAuctionNewFilterElementItemButtonClick filterElementItemButtonClick,
            Action onResetFilterListAction = null)
        {

            ClearData();
            _onResetFilterListAction = onResetFilterListAction;

            _auctionNewFilterData = auctionNewFilterData;
            _onAuctionNewfilterElementItemButtonClick = filterElementItemButtonClick;

            if (_auctionNewFilterData == null)
                return;

            _auctionNewFilterDataList = AuctionNewDataManager.GetInstance()
                .GetAuctionNewFilterDataList(_auctionNewFilterData.FilterItemType);

            if (_auctionNewFilterDataList != null && _auctionNewFilterDataList.Count > 0)
            {
                for (var i = 0; i < _auctionNewFilterDataList.Count; i++)
                {
                    if (_auctionNewFilterDataList[i].Id == _auctionNewFilterData.Id)
                    {
                        _auctionNewFilterDataList[i].IsSelected = true;
                    }
                    else
                    {
                        _auctionNewFilterDataList[i].IsSelected = false;
                    }
                }
            }

            InitFilterView();
        }

        private void InitFilterView()
        {
            if (filterName != null)
            {
                filterName.text = _auctionNewFilterData.Name;
            }

            if (filterElementItemListRoot != null)
            {
                filterElementItemListRoot.gameObject.CustomActive(false);
            }

            if (filterElementItemList != null)
            {
                if (_auctionNewFilterDataList != null)
                {
                    filterElementItemList.SetElementAmount(_auctionNewFilterDataList.Count);
                }
            }
        }
        
        private void OnFilterButtonClick()
        {
            if (filterElementItemListRoot != null)
            {
                if (filterElementItemListRoot.gameObject.activeSelf == true)
                {
                    UpdateFilterElementItemListVisible(false);
                }
                else
                {
                    UpdateFilterElementItemListVisible(true);
                    UpdateElementItemListContent();
                }
            }
        }

        //重置filterElementItmeList 的展示位置
        private void UpdateElementItemListContent()
        {
            if (filterElementItemList != null)
            {
                filterElementItemList.ResetContentPosition();
            }

            MoveScrollListToShowSelectedFilter();
        }

        private void MoveScrollListToShowSelectedFilter()
        {
            if (filterElementItemList == null)
                return;

            var index = GetSelectedFilterDataIndex();
            if (index <= 1)
            {
                filterElementItemList.MoveElementInScrollArea(0, true);
            }
            else if (index >= _auctionNewFilterDataList.Count - 2)
            {
                filterElementItemList.MoveElementInScrollArea(_auctionNewFilterDataList.Count - 1, true);
            }
            else
            {
                filterElementItemList.MoveElementInScrollArea(index + 1, true);
            }
        }

        private void OnFilterElementItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (filterElementItemList == null)
                return;

            if (_auctionNewFilterDataList == null)
                return;

            if (item.m_index < 0 || item.m_index >= _auctionNewFilterDataList.Count)
                return;

            var auctionNewFilterData = _auctionNewFilterDataList[item.m_index];
            var auctionNewFilterElementItem = item.GetComponent<AuctionNewFilterElementItem>();

            if (auctionNewFilterElementItem != null && auctionNewFilterData != null)
            {
                auctionNewFilterElementItem.InitData(auctionNewFilterData,
                    OnFilterElementItemButtonClick);
            }
        }

        private void OnFilterElementItemUpdate(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var auctionNewFilterElementItem = item.GetComponent<AuctionNewFilterElementItem>();
            if (auctionNewFilterElementItem != null)
            {
                auctionNewFilterElementItem.UpdateFilterElementItem();
            }
        }

        private void OnFilterElementItemButtonClick(AuctionNewFilterData auctionNewFilterData)
        {
            if (auctionNewFilterData == null)
                return;

            //if (auctionNewFilterData.Id == _auctionNewFilterData.Id)
            //    return;

            _auctionNewFilterData = auctionNewFilterData;

            UpdateFilterName();

            DisableFilterListRoot();

            if (_onAuctionNewfilterElementItemButtonClick != null)
            {
                _onAuctionNewfilterElementItemButtonClick(_auctionNewFilterData);
            }

            UpdateAuctionNewFilterDataSelectedInfo();
        }

        private void UpdateAuctionNewFilterDataSelectedInfo()
        {
            if (_auctionNewFilterData == null)
                return;

            if (_auctionNewFilterDataList == null || _auctionNewFilterDataList.Count <= 0)
                return;

            for (var i = 0; i < _auctionNewFilterDataList.Count; i++)
            {
                if (_auctionNewFilterDataList[i].FilterItemType == _auctionNewFilterData.FilterItemType
                    && _auctionNewFilterDataList[i].Id == _auctionNewFilterData.Id)
                {
                    _auctionNewFilterDataList[i].IsSelected = true;
                }
                else
                {
                    _auctionNewFilterDataList[i].IsSelected = false;
                }
            }

            if (filterElementItemList != null)
                filterElementItemList.UpdateElement();
        }

        private void UpdateFilterName()
        {
            if (filterName != null)
                filterName.text = _auctionNewFilterData.Name;
        }

        private void DisableFilterListRoot()
        {
            UpdateFilterElementItemListVisible(false);
        }

        private void UpdateFilterElementItemListVisible(bool flag)
        {
            //首先重置FilterButton
            if (_onResetFilterListAction != null)
                _onResetFilterListAction();

            if (filterElementItemListRoot != null)
            {
                filterElementItemListRoot.gameObject.CustomActive(flag);
            }

            if (closeButton != null)
            {
                closeButton.gameObject.CustomActive(flag);
            }
        }

        private int GetSelectedFilterDataIndex()
        {
            var index = 0;

            if (_auctionNewFilterDataList == null || _auctionNewFilterDataList.Count <= 0)
                return 0;

            if (_auctionNewFilterData == null)
                return 0;

            for (var i = 0; i < _auctionNewFilterDataList.Count; i++)
            {
                var curAuctionNewFilterData = _auctionNewFilterDataList[i];
                if (curAuctionNewFilterData == null)
                    continue;

                if (curAuctionNewFilterData.FilterItemType == _auctionNewFilterData.FilterItemType
                    && curAuctionNewFilterData.Id == _auctionNewFilterData.Id)
                {
                    index = i;
                    return index;
                }
            }

            return index;
        }

        private void OnCloseButtonClick()
        {
            UpdateFilterElementItemListVisible(false);
        }

        public void ResetFilterList()
        {
            if (filterElementItemListRoot != null)
                filterElementItemListRoot.gameObject.CustomActive(false);

            if (closeButton != null)
                closeButton.gameObject.CustomActive(false);
        }

    }
}