using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class ComDropDownItem : MonoBehaviour
    {

        //数据和回调
        private OnComDropDownItemButtonClick _onItemButtonClick = null;
        protected ComControlData _comControlData = null;

        [SerializeField] private Text nameText;
        [SerializeField] private Image selectedFlag;
        [SerializeField] private Button button;
        [SerializeField] private bool mNeedChangeColor = false;
        [SerializeField] private Color mColorUnselect;
        [SerializeField] private Color mColorSelect;


        private void Awake()
        {
            _comControlData = null;
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

        public virtual void InitItem(ComControlData comControlData,
            OnComDropDownItemButtonClick onItemButtonClick = null)
        {
            _comControlData = comControlData;
            if(_comControlData == null)
                return;

            if (nameText != null)
            {
                if (string.IsNullOrEmpty(_comControlData.Name) == true)
                {
                    nameText.text = "ERROR";
                }
                else
                {
                    nameText.text = _comControlData.Name;
                }
            }

            _onItemButtonClick = onItemButtonClick;
            
            UpdateComDropDownItem();
        }

        private void OnItemButtonClick()
        {
            _comControlData.IsSelected = true;

            //执行回调，更新所有的Item选中状态
            if (_onItemButtonClick != null)
                _onItemButtonClick(_comControlData);

            UpdateComDropDownItem();
        }

        public void UpdateComDropDownItem()
        {
            if (_comControlData == null)
                return;

            if (selectedFlag == null)
                return;
            if (mNeedChangeColor)
            {
                nameText.color = _comControlData.IsSelected ? mColorSelect : mColorUnselect;
            }
            selectedFlag.gameObject.CustomActive(_comControlData.IsSelected);
        }
    }
}
