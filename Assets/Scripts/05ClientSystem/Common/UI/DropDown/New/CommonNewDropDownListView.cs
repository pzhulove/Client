using System;
using System.Collections.Generic;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class CommonNewDropDownListView : MonoBehaviour
    {

        //数据和回调
        private OnCommonNewDropDownItemButtonClick _onItemButtonClick = null;
        private List<ComControlData> _commonNewDropDownDataList = null;

        [SerializeField] private ComUIListScriptEx dropDownItemList;

        private void Awake()
        {
            BindUiEventSystem();
        }

        private void BindUiEventSystem()
        {
            if (dropDownItemList != null)
            {
                dropDownItemList.Initialize();
                dropDownItemList.onItemVisiable += OnItemVisible;
                dropDownItemList.OnItemUpdate += OnItemUpdate;
            }
        }

        private void OnDestroy()
        {
            UnBindUiEventSystem();
            ClearData();
        }

        private void UnBindUiEventSystem()
        {
            if (dropDownItemList != null)
            {
                dropDownItemList.onItemVisiable -= OnItemVisible;
                dropDownItemList.OnItemUpdate -= OnItemUpdate;
            }
        }

        private void ClearData()
        {
            _onItemButtonClick = null;
            _commonNewDropDownDataList = null;
        }

        public virtual void InitCommonNewDropDownListView(ComControlData defaultControlData,
            List<ComControlData> comControlDataList,
            OnCommonNewDropDownItemButtonClick onItemButtonClick = null)
        {
            _commonNewDropDownDataList = comControlDataList;
            _onItemButtonClick = onItemButtonClick;

            UpdateCommonNewDropDownDataList(defaultControlData);

            InitDropDownList();
        }

        private void InitDropDownList()
        {
            if (dropDownItemList != null)
                dropDownItemList.SetElementAmount(_commonNewDropDownDataList.Count);

            UpdateSelectedItemPosition();
        }


        public virtual void UpdateSelectedItemPosition()
        {
            if(dropDownItemList != null)
                dropDownItemList.ResetComUiListScriptEx();

            var index = GetSelectedItemIndex();
            if (dropDownItemList != null)
                dropDownItemList.MoveItemToMiddlePosition(index);
        }

        private void OnItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (dropDownItemList == null)
                return;

            if (_commonNewDropDownDataList == null)
                return;

            if (_commonNewDropDownDataList.Count <= 0 || item.m_index >= _commonNewDropDownDataList.Count)
                return;

            var commonNewDropDownData = _commonNewDropDownDataList[item.m_index];
            var commonNewDropDownItem = item.GetComponent<CommonNewDropDownItem>();

            if (commonNewDropDownItem != null && commonNewDropDownData != null)
            {
                commonNewDropDownItem.InitItem(commonNewDropDownData,
                    OnItemButtonClick);
            }
        }

        private void OnItemUpdate(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var commonNewDropDownItem = item.GetComponent<CommonNewDropDownItem>();
            if (commonNewDropDownItem != null)
                commonNewDropDownItem.UpdateComDropDownItemSelectedFlag();
        }

        private void OnItemButtonClick(ComControlData comControlData)
        {
            //更新数据和展示
            UpdateCommonNewDropDownDataList(comControlData);
            UpdateCommonNewDropDownItemList();

            //回调
            if (_onItemButtonClick != null)
                _onItemButtonClick(comControlData);
        }

        //更新数据
        public void UpdateCommonNewDropDownDataList(ComControlData comControlData)
        {
            if (_commonNewDropDownDataList == null)
                return;

            for (var i = 0; i < _commonNewDropDownDataList.Count; i++)
            {
                if (_commonNewDropDownDataList[i].Id == comControlData.Id)
                {
                    _commonNewDropDownDataList[i].IsSelected = true;
                }
                else
                {
                    _commonNewDropDownDataList[i].IsSelected = false;
                }
            }
        }

        public void UpdateCommonNewDropDownItemList()
        {
            if(dropDownItemList != null)
                dropDownItemList.UpdateElement();
        }

        private int GetSelectedItemIndex()
        {
            for (var i = 0; i < _commonNewDropDownDataList.Count; i++)
            {
                var comDropDownData = _commonNewDropDownDataList[i];
                if (comDropDownData != null && comDropDownData.IsSelected == true)
                    return i;
            }
            return 0;
        }

    }
}
