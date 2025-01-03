using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class ComToggleControl : MonoBehaviour
    {

        [Space(5)] [HeaderAttribute("ComToggleControl")] [Space(5)] [SerializeField]
        private ComUIListScript comToggleItemList;

        private List<ComControlData> _comToggleDataList;
        private OnComToggleClick _onComToggleClick;

        private void Awake()
        {
            if (comToggleItemList != null)
            {
                comToggleItemList.Initialize();
                comToggleItemList.onItemVisiable += OnComToggleItemVisible;
                comToggleItemList.OnItemRecycle += OnComToggleItemRecycle;
            }
        }

        private void OnDestroy()
        {
            if (comToggleItemList != null)
            {
                comToggleItemList.onItemVisiable -= OnComToggleItemVisible;
                comToggleItemList.OnItemRecycle -= OnComToggleItemRecycle;
            }

            ClearData();
        }

        private void ClearData()
        {
            if (_comToggleDataList != null)
                _comToggleDataList = null;
            _onComToggleClick = null;
        }

        //初始化：设置数据和回调
        public void InitComToggleControl(List<ComControlData> comToggleDataList,
            OnComToggleClick onComToggleClick = null)
        {
            if (comToggleItemList == null )
            {
                Logger.LogErrorFormat("comToggleItemList is null");
                return;
            }

            if (comToggleDataList == null || comToggleDataList.Count <= 0)
            {
                Logger.LogErrorFormat("comToggleControl comToggleDataList is null");
                return;
            }

            _comToggleDataList = comToggleDataList;
            _onComToggleClick = onComToggleClick;

            if (comToggleItemList != null)
                comToggleItemList.SetElementAmount(_comToggleDataList.Count);
        }

        private void OnComToggleItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (_comToggleDataList == null || item.m_index >= _comToggleDataList.Count)
                return;

            var comToggleData = _comToggleDataList[item.m_index];
            var comToggleItem = item.GetComponent<ComToggleItem>();

            if (comToggleItem != null && comToggleData != null)
            {
                comToggleItem.InitItem(comToggleData, _onComToggleClick);
            }
        }

        private void OnComToggleItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var comToggleItem = item.GetComponent<ComToggleItem>();

            if(comToggleItem != null)
                comToggleItem.OnItemRecycle();
        }

    }
}