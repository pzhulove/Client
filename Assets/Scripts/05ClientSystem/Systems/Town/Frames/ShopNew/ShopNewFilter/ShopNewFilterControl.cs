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
    public class ShopNewFilterControl : MonoBehaviour
    {
        private ShopNewFilterData _shopNewFilterData = null;
        private List<ShopNewFilterData> _shopNewFilterDataList = new List<ShopNewFilterData>();

        private int _defaultSelectedIndex = 0;

        private OnShopNewFilterElementItemTabValueChanged _filterElementItemTabValueChanged;

        private Action _onResetFilterListAction;

        private bool _isShowFilterTitle = false;         //是否显示过滤器的描述

        [SerializeField] private Text filterName;
        [SerializeField] private Button filterButton;
        [SerializeField] private Button closeButton;

        [SerializeField] private ComUIListScript filterElementItemList;
        [SerializeField] private GameObject filterElementItemListRoot;

        [Space(10)] [HeaderAttribute("typeRoot")] [SerializeField]
        private GameObject typeRoot;

        [SerializeField] private Text typeName;
        
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
            if(filterButton != null)
                filterButton.onClick.RemoveAllListeners();

            if(closeButton != null)
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
            _shopNewFilterData = null;
            _shopNewFilterDataList = null;

            _onResetFilterListAction = null;

            _isShowFilterTitle = false;

            if (filterElementItemList != null)
                filterElementItemList.SetElementAmount(0);
        }

        public void InitFilterControl(ShopNewFilterData shopNewFilterData,
            OnShopNewFilterElementItemTabValueChanged filterElementItemTabValueChanged,
            Action onResetFilterListAction = null,
            bool isShowFilterTitle = false)
        {

            ClearData();
            _onResetFilterListAction = onResetFilterListAction;

            _shopNewFilterData = shopNewFilterData;
            _filterElementItemTabValueChanged = filterElementItemTabValueChanged;
            _isShowFilterTitle = isShowFilterTitle;

            if(_shopNewFilterData == null)
                return;

            _shopNewFilterDataList = ShopNewDataManager.GetInstance().GetShopNewFilterDataList(_shopNewFilterData.FilterType);

            if (_shopNewFilterDataList != null && _shopNewFilterDataList.Count > 0)
            {
                for (var i = 0; i < _shopNewFilterDataList.Count; i++)
                {
                    if (_shopNewFilterDataList[i].Index == _shopNewFilterData.Index)
                    {
                        _shopNewFilterDataList[i].IsSelected = true;
                    }
                    else
                    {
                        _shopNewFilterDataList[i].IsSelected = false;
                    }
                }
            }

            InitFilterView();
        }

        private void InitFilterView()
        {
            if (filterName != null)
            {
                filterName.text = _shopNewFilterData.Name;
            }

            if (filterElementItemListRoot != null)
            {
                filterElementItemListRoot.gameObject.CustomActive(false);
            }

            if (filterElementItemList != null)
            {
                if (_shopNewFilterDataList != null)
                {
                    filterElementItemList.SetElementAmount(_shopNewFilterDataList.Count);
                }
            }

            InitFilterTitle();
        }

        private void InitFilterTitle()
        {
            if (typeRoot != null)
            {
                typeRoot.gameObject.CustomActive(_isShowFilterTitle == true);
            }

            if (typeName != null)
            {
                typeName.text = TR.Value("shop_new_occu_label");
                if (_shopNewFilterData.FilterType == ShopTable.eFilter.SF_OCCU)
                {
                    typeName.text = TR.Value("shop_new_occu_label");
                }
                else if (_shopNewFilterData.FilterType == ShopTable.eFilter.SF_ARMOR)
                {
                    typeName.text = TR.Value("shop_new_armor_label");
                }
                else
                {
                    //只有职业和防具显示页签，其他的不显示页签
                    if (typeRoot != null)
                        typeRoot.gameObject.CustomActive(false);
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
            if(filterElementItemList == null)
                return;

            var index = GetSelectedFilterDataIndex();
            if (index <= 1)
            {
                filterElementItemList.MoveElementInScrollArea(0, true);
            }
            else if (index >= _shopNewFilterDataList.Count - 2)
            {
                filterElementItemList.MoveElementInScrollArea(_shopNewFilterDataList.Count - 1, true);
            }
            else
            {
                filterElementItemList.MoveElementInScrollArea(index + 1, true);
            }
        }

        private void OnFilterElementItemVisible(ComUIListElementScript item)
        {
            if(item == null)
                return;

            if(filterElementItemList == null)
                return;

            if(_shopNewFilterDataList == null)
                return;

            if (item.m_index < 0 || item.m_index >= _shopNewFilterDataList.Count)
                return;

            var shopNewFilterData = _shopNewFilterDataList[item.m_index];
            var shopNewFilterElementItem = item.GetComponent<ShopNewFilterElementItem>();
            
            if (shopNewFilterElementItem != null && shopNewFilterData != null)
            {
                shopNewFilterElementItem.InitData(shopNewFilterData,
                    OnFilterElementItemToggleClick);
            }
        }

        private void OnFilterElementItemUpdate(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var shopNewFilterElementItem = item.GetComponent<ShopNewFilterElementItem>();
            if (shopNewFilterElementItem != null)
            {
                shopNewFilterElementItem.UpdateFilterElementItem();
            }
        }

        private void OnFilterElementItemToggleClick(ShopNewFilterData shopNewFilterData)
        {
            if(shopNewFilterData == null)
                return;

            if(shopNewFilterData.Index == _shopNewFilterData.Index)
                return;

            _shopNewFilterData = shopNewFilterData;
            
            UpdateFilterName();

            DisableFilterListRoot();

            if (_filterElementItemTabValueChanged != null)
            {
                _filterElementItemTabValueChanged(_shopNewFilterData);
            }

            UpdateShopNewFilterDataSelectedInfo();
        }

        private void UpdateShopNewFilterDataSelectedInfo()
        {
            if(_shopNewFilterData == null)
                return;

            if(_shopNewFilterDataList == null || _shopNewFilterDataList.Count <= 0)
                return;

            for (var i = 0; i < _shopNewFilterDataList.Count; i++)
            {
                if (_shopNewFilterDataList[i].FilterType == _shopNewFilterData.FilterType
                    && _shopNewFilterDataList[i].Index == _shopNewFilterData.Index)
                {
                    _shopNewFilterDataList[i].IsSelected = true;
                }
                else
                {
                    _shopNewFilterDataList[i].IsSelected = false;
                }
            }

            if(filterElementItemList != null)
                filterElementItemList.UpdateElement();
        }

        private void UpdateFilterName()
        {
            if (filterName != null)
                filterName.text = _shopNewFilterData.Name;
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

            if (_shopNewFilterDataList == null || _shopNewFilterDataList.Count <= 0)
                return 0;

            if (_shopNewFilterData == null)
                return 0;

            for (var i = 0; i < _shopNewFilterDataList.Count; i++)
            {
                var curShopNewFilterData = _shopNewFilterDataList[i];
                if(curShopNewFilterData == null)
                    continue;

                if (curShopNewFilterData.FilterType == _shopNewFilterData.FilterType
                    && curShopNewFilterData.Index == _shopNewFilterData.Index)
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