using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComFirstLevelToggleItem : MonoBehaviour
    {

        private ComFirstLevelToggleData _comFirstLevelToggleData = null;

        private bool _isOwnerSecondLevel = false;      //是否存在第二层级
        private bool _isInitSecondLevel = false;       //是否对第二层级进行了初始化

        private OnComToggleClick _onFirstLevelToggleClick;
        private OnComToggleClick _onSecondLevelToggleClick;

        private bool _isSelected = false;

        private List<ComToggleItem> _secondLevelToggleItemList = new List<ComToggleItem>();

        [Space(10)]
        [HeaderAttribute("FirstLevelToggle")]
        [Space(5)]
        [SerializeField] private Text normalName;
        [SerializeField] private Text selectedTabName;
        [SerializeField] private Toggle toggle;

        [Space(10)]
        [HeaderAttribute("Arrow")]
        [Space(5)]
        [SerializeField] private GameObject arrowRoot;
        [SerializeField] private Image arrowUp;
        [SerializeField] private Image arrowDown;

        [Space(10)]
        [HeaderAttribute("SecondLevelToggle")]
        [Space(5)]
        [SerializeField] private GameObject secondLevelToggleRoot;
        [SerializeField] private GameObject secondLevelToggleItemPrefab;

        private void Awake()
        {
            if (toggle != null)
            {
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener(OnToggleClick);
            }
        }

        private void OnDestroy()
        {
            if (toggle != null)
                toggle.onValueChanged.RemoveAllListeners();

            ResetData();
        }

        private void ResetData()
        {

            _comFirstLevelToggleData = null;
            _isOwnerSecondLevel = false;
            _isInitSecondLevel = false;
            _onFirstLevelToggleClick = null;
            _onSecondLevelToggleClick = null;
            _isSelected = false;
            _secondLevelToggleItemList.Clear();
        }

        public void InitItem(ComFirstLevelToggleData comFirstLevelToggleData,
            OnComToggleClick onFirstLevelToggleClick = null,
            OnComToggleClick onSecondLevelToggleClick = null)
        {

            //InitData
            //首先数据的重置
            ResetData();
            _comFirstLevelToggleData = comFirstLevelToggleData;

            _onFirstLevelToggleClick = onFirstLevelToggleClick;
            _onSecondLevelToggleClick = onSecondLevelToggleClick;

            if (_comFirstLevelToggleData == null || _comFirstLevelToggleData.FirstLevelToggleData == null)
            {
                Logger.LogErrorFormat("ComFirstLevelToggleItem InitItem comFirstLevelToggleData is null");
                return;
            }

            _isOwnerSecondLevel = true;
            if (_comFirstLevelToggleData.SecondLevelToggleDataList == null
                || _comFirstLevelToggleData.SecondLevelToggleDataList.Count <= 0)
                _isOwnerSecondLevel = false;
            

            InitItemView();
        }

        private void InitItemView()
        {
            if (normalName != null)
                normalName.text = _comFirstLevelToggleData.FirstLevelToggleData.Name;

            if (selectedTabName != null)
                selectedTabName.text = _comFirstLevelToggleData.FirstLevelToggleData.Name;

            InitArrowUp();

            if (_comFirstLevelToggleData.FirstLevelToggleData.IsSelected == true)
            {
                if (toggle != null)
                    toggle.isOn = true;
            }
        }


        private void OnToggleClick(bool value)
        {
            if(_comFirstLevelToggleData == null || _comFirstLevelToggleData.FirstLevelToggleData == null)
                return;

            //避免重复点击时，View重复更新
            if (_isSelected == value)
                return;
            _isSelected = value;

            if (value == true)
            {
                if (_onFirstLevelToggleClick != null)
                    _onFirstLevelToggleClick(_comFirstLevelToggleData.FirstLevelToggleData);

                //拥有子节点
                if (_isOwnerSecondLevel == true)
                {
                    SetArrowUp(false);
                    SetArrowDown(true);

                    if (secondLevelToggleRoot != null)
                    {
                        if (secondLevelToggleRoot.activeSelf == false)
                            secondLevelToggleRoot.CustomActive(true);
                    }

                    //第一次进行初始化操作
                    if (_isInitSecondLevel == false)
                    {
                        InitSecondLevel();
                        _isInitSecondLevel = true;
                    }
                    else
                    {
                        if (_secondLevelToggleItemList != null && _secondLevelToggleItemList.Count > 0)
                        {
                            for (var i = 0; i < _secondLevelToggleItemList.Count; i++)
                            {
                                var secondLevelToggleItem = _secondLevelToggleItemList[i];
                                if(secondLevelToggleItem != null)
                                    secondLevelToggleItem.OnEnableComToggleItem();
                            }
                        }
                    }
                }
            }
            else
            {
                if (_isOwnerSecondLevel == true)
                {
                    SetArrowUp(true);
                    SetArrowDown(false);

                    if (secondLevelToggleRoot != null)
                        secondLevelToggleRoot.CustomActive(false);
                }
            }
        }

        #region SecondLayerInfo
        private void InitSecondLevel()
        {
            _secondLevelToggleItemList.Clear();

            if (_comFirstLevelToggleData.SecondLevelToggleDataList == null
                || _comFirstLevelToggleData.SecondLevelToggleDataList.Count <= 0)
            {
                Logger.LogErrorFormat("ComFirstLevelToggleItem InitSecondLevel secondLevelToggleDataList is null");
                return;
            }

            if (secondLevelToggleRoot == null || secondLevelToggleItemPrefab == null)
            {
                Logger.LogErrorFormat(
                    "ComFirstLevelToggleItem InitSecondLevel secondLevelToggleRoot is null or secondLevelToggleItem is null");
                return;
            }

            for (var i = 0; i < _comFirstLevelToggleData.SecondLevelToggleDataList.Count; i++)
            {
                var secondLevelToggleData = _comFirstLevelToggleData.SecondLevelToggleDataList[i];
                if (secondLevelToggleData == null)
                {
                    Logger.LogErrorFormat("InitSecondLevel index is {0}, secondLevelToggleData is null", i);
                    continue;
                }

                var secondLevelToggleGo = Instantiate(secondLevelToggleItemPrefab) as GameObject;
                if (secondLevelToggleGo != null)
                {
                    secondLevelToggleGo.CustomActive(true);
                    Utility.AttachTo(secondLevelToggleGo, secondLevelToggleRoot);

                    var secondLevelToggleItem = secondLevelToggleGo.GetComponent<ComToggleItem>();

                    if (secondLevelToggleItem != null)
                    {
                        secondLevelToggleItem.InitItem(secondLevelToggleData, _onSecondLevelToggleClick);
                        _secondLevelToggleItemList.Add(secondLevelToggleItem);
                    }
                }
            }
        }
        #endregion

        #region Arrow
        private void InitArrowUp()
        {
            if (arrowRoot != null)
            {
                if (_isOwnerSecondLevel == false)
                {
                    arrowRoot.CustomActive(false);
                }
                else
                {
                    arrowRoot.CustomActive(true);
                }
            }

            if (_isOwnerSecondLevel == true)
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

    }
}
