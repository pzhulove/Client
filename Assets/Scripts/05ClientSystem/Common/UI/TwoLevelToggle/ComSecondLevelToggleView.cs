using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComSecondLevelToggleView : MonoBehaviour
    {

        private List<ComControlData> _comSecondLevelToggleDataModelList = null;
        private List<ComToggleItem> _comSecondLevelToggleItemList = new List<ComToggleItem>();

        private bool _isInitSecondLevelToggleView = false;       //是否对第二层级进行了初始化

        private OnComToggleClick _onSecondLevelToggleClick;


        [Space(10)]
        [HeaderAttribute("SecondLevelToggle")]
        [Space(5)]
        [SerializeField] private GameObject secondLevelToggleRoot;
        [SerializeField] private GameObject secondLevelToggleItemPrefab;

        private void Awake()
        {
        }

        private void OnDestroy()
        {
            ResetData();
        }

        private void ResetData()
        {
            _comSecondLevelToggleDataModelList = null;
            _comSecondLevelToggleItemList.Clear();

            _isInitSecondLevelToggleView = false;
            _onSecondLevelToggleClick = null;
        }

        //初始化第二级toggle的数据
        public void InitSecondLevelToggleViewData(ComTwoLevelToggleData comTwoLevelToggleData,
            OnComToggleClick onSecondLevelToggleClick = null)
        {
            //数据重置
            ResetData();

            if (comTwoLevelToggleData == null)
                return;

            _comSecondLevelToggleDataModelList = comTwoLevelToggleData.SecondLevelToggleDataList;
            _onSecondLevelToggleClick = onSecondLevelToggleClick;
        }

        //初始化，或者展示
        public void ShowSecondLevelToggleView()
        {
            if (_comSecondLevelToggleDataModelList == null || _comSecondLevelToggleDataModelList.Count <= 0)
                return;

            //第一次初始化，创建二级菜单
            if (_isInitSecondLevelToggleView == false)
            {
                InitSecondLevelToggleView();
                _isInitSecondLevelToggleView = true;
            }
            else
            {
                //已经初始化，直接展示
                EnableSecondLevelToggleView();
            }
        }

        #region SecondLayerInfo
        private void InitSecondLevelToggleView()
        {
            _comSecondLevelToggleItemList.Clear();

            if (_comSecondLevelToggleDataModelList == null
                || _comSecondLevelToggleDataModelList.Count <= 0)
                return;

            if (secondLevelToggleRoot == null || secondLevelToggleItemPrefab == null)
            {
                Logger.LogErrorFormat(
                    "ComFirstLevelToggleItem InitSecondLevel secondLevelToggleRoot is null or secondLevelToggleItem is null");
                return;
            }

            for (var i = 0; i < _comSecondLevelToggleDataModelList.Count; i++)
            {
                var secondLevelToggleData = _comSecondLevelToggleDataModelList[i];
                if (secondLevelToggleData == null)
                {
                    Logger.LogErrorFormat("InitSecondLevel index is {0}, secondLevelToggleData is null", i);
                    continue;
                }

                var secondLevelToggleItemGo = GameObject.Instantiate(secondLevelToggleItemPrefab) as GameObject;
                if (secondLevelToggleItemGo != null)
                {
                    secondLevelToggleItemGo.CustomActive(true);
                    Utility.AttachTo(secondLevelToggleItemGo, secondLevelToggleRoot);
                    secondLevelToggleItemGo.transform.name = secondLevelToggleItemGo.transform.name + "_" + (i + 1);

                    var secondLevelToggleItem = secondLevelToggleItemGo.GetComponent<ComToggleItem>();

                    if (secondLevelToggleItem != null)
                    {
                        secondLevelToggleItem.InitItem(secondLevelToggleData, _onSecondLevelToggleClick);
                        _comSecondLevelToggleItemList.Add(secondLevelToggleItem);
                    }
                }
            }
        }

        private void EnableSecondLevelToggleView()
        {
            if (_comSecondLevelToggleItemList != null && _comSecondLevelToggleItemList.Count > 0)
            {
                for (var i = 0; i < _comSecondLevelToggleItemList.Count; i++)
                {
                    var secondLevelToggleItem = _comSecondLevelToggleItemList[i];
                    if (secondLevelToggleItem != null)
                    {
                        secondLevelToggleItem.OnEnableComToggleItem();
                    }
                }
            }
        }

        #endregion
    }
}
