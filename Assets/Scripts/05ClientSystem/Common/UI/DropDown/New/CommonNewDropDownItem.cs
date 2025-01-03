using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class CommonNewDropDownItem : MonoBehaviour
    {

        //数据和回调
        private OnCommonNewDropDownItemButtonClick _onItemButtonClick = null;
        protected ComControlData _commonNewControlData = null;

        [SerializeField] private Text nameText;
        [SerializeField] private Image selectedFlag;
        [SerializeField] private Button button;

        private void Awake()
        {
            _commonNewControlData = null;
            BindUiEventSystem();
        }

        private void BindUiEventSystem()
        {
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnItemButtonClick);
            }
        }

        private void OnDestroy()
        {
            UnBindUiEventSystem();
        }

        private void UnBindUiEventSystem()
        {
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
            }
        }

        private void ClearData()
        {
            _commonNewControlData = null;
            _onItemButtonClick = null;
        }

        public virtual void InitItem(ComControlData comControlData,
            OnCommonNewDropDownItemButtonClick onItemButtonClick = null)
        {
            _commonNewControlData = comControlData;
            if (_commonNewControlData == null)
                return;

            _onItemButtonClick = onItemButtonClick;

            //名字和选中标志
            UpdateCommonNewDropDownItemName();
            UpdateComDropDownItemSelectedFlag();
        }

        private void UpdateCommonNewDropDownItemName()
        {
            if (nameText != null)
            {
                if (string.IsNullOrEmpty(_commonNewControlData.Name) == true)
                {
                    nameText.text = "ERROR";
                }
                else
                {
                    nameText.text = _commonNewControlData.Name;
                }
            }
        }

        public void UpdateComDropDownItemSelectedFlag()
        {
            if (_commonNewControlData == null)
                return;

            if (selectedFlag == null)
                return;

            selectedFlag.gameObject.CustomActive(_commonNewControlData.IsSelected);
        }

        private void OnItemButtonClick()
        {
            _commonNewControlData.IsSelected = true;
            UpdateComDropDownItemSelectedFlag();

            //执行回调，更新所有的Item选中状态
            if (_onItemButtonClick != null)
                _onItemButtonClick(_commonNewControlData);
        }

    }
}
