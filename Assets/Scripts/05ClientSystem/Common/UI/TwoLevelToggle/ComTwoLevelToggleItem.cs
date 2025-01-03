using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComTwoLevelToggleItem : MonoBehaviour
    {

        private ComTwoLevelToggleData _comTwoLevelToggleData = null;
        protected ComControlData _comFirstLevelToggleData = null;

        private bool _isOwnerSecondLevel = false;      //是否存在第二层级

        private OnComToggleClick _onFirstLevelToggleClick;
        private OnComToggleClick _onSecondLevelToggleClick;

        [Space(10)]
        [HeaderAttribute("FirstLevelToggle")]
        [Space(5)]
        [SerializeField] private Toggle toggle;
        [SerializeField] private Text normalName;
        [SerializeField] private Text selectedName;

        [Space(10)]
        [HeaderAttribute("Arrow")]
        [Space(5)]
        [SerializeField] private GameObject arrowRoot;
        [SerializeField] private Image normalArrow;
        [SerializeField] private Image selectedArrow;

        [Space(10)]
        [HeaderAttribute("SecondLevelToggleView")]
        [Space(5)]
        [SerializeField]
        private ComSecondLevelToggleView secondLevelToggleView;

        private void Awake()
        {
            if (toggle != null)
            {
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener(OnFirstLevelToggleClick);
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
            _comTwoLevelToggleData = null;
            _comFirstLevelToggleData = null;

            _isOwnerSecondLevel = false;

            _onFirstLevelToggleClick = null;
            _onSecondLevelToggleClick = null;
        }

        public void InitItem(ComTwoLevelToggleData comTwoLevelToggleData,
            OnComToggleClick onFirstLevelToggleClick = null,
            OnComToggleClick onSecondLevelToggleClick = null)
        {

            //数据重置
            ResetData();

            _comTwoLevelToggleData = comTwoLevelToggleData;
            if (_comTwoLevelToggleData == null || _comTwoLevelToggleData.FirstLevelToggleData == null)
            {
                Logger.LogErrorFormat("ComFirstLevelToggleItem InitItem comFirstLevelToggleData is null");
                return;
            }

            _comFirstLevelToggleData = _comTwoLevelToggleData.FirstLevelToggleData;

            _onFirstLevelToggleClick = onFirstLevelToggleClick;
            _onSecondLevelToggleClick = onSecondLevelToggleClick;

            //初始化二级toggle数据
            InitSecondLevelToggleData();

            InitItemView();
        }

        private void InitSecondLevelToggleData()
        {
            //默认拥有二级toggle
            _isOwnerSecondLevel = true;
            if (_comTwoLevelToggleData.SecondLevelToggleDataList == null
                || _comTwoLevelToggleData.SecondLevelToggleDataList.Count <= 0)
                _isOwnerSecondLevel = false;

            //二级toggle存在
            if (_isOwnerSecondLevel == true)
            {
                if (secondLevelToggleView != null)
                {
                    secondLevelToggleView.InitSecondLevelToggleViewData(_comTwoLevelToggleData,
                        _onSecondLevelToggleClick);
                }
            }
        }

        protected virtual void InitItemView()
        {
            //名字
            InitItemName();
            //箭头
            InitArrowRoot();
            //Toggle
            InitItemToggle();
        }

        private void InitItemName()
        {
            if (normalName != null)
                normalName.text = _comFirstLevelToggleData.Name;

            if (selectedName != null)
                selectedName.text = _comFirstLevelToggleData.Name;
        }

        private void InitItemToggle()
        {
            if (toggle != null)
            {
                if (_comFirstLevelToggleData.IsSelected == true)
                {
                    toggle.isOn = true;
                }
                else
                {
                    toggle.isOn = false;
                }
            }
        }


        private void OnFirstLevelToggleClick(bool value)
        {
            if (_comFirstLevelToggleData == null)
                return;

            //更新选择的标志
            _comFirstLevelToggleData.IsSelected = value;

            if (value == true)
            {
                if (_onFirstLevelToggleClick != null)
                    _onFirstLevelToggleClick(_comFirstLevelToggleData);

                //拥有子节点
                if (_isOwnerSecondLevel == true)
                {
                    UpdateNormalArrow(false);

                    UpdateSecondLevelToggleViewVisible(true);
                    //二级菜单的view展示
                    if (secondLevelToggleView != null)
                        secondLevelToggleView.ShowSecondLevelToggleView();
                }
            }
            else
            {
                if (_isOwnerSecondLevel == true)
                {
                    UpdateNormalArrow(true);

                    UpdateSecondLevelToggleViewVisible(false);
                }
            }
        }

        #region Arrow
        private void InitArrowRoot()
        {
            CommonUtility.UpdateGameObjectVisible(arrowRoot, _isOwnerSecondLevel);
            UpdateNormalArrow(_isOwnerSecondLevel);
        }

        //箭头显示与否
        private void UpdateNormalArrow(bool flag)
        {
            CommonUtility.UpdateImageVisible(normalArrow, flag);
            CommonUtility.UpdateImageVisible(selectedArrow, !flag);
        }

        #endregion

        //二级toggle是否显示
        private void UpdateSecondLevelToggleViewVisible(bool flag)
        {
            if (secondLevelToggleView != null)
            {
                CommonUtility.UpdateGameObjectVisible(secondLevelToggleView.gameObject, flag);
            }
        }


    }
}
