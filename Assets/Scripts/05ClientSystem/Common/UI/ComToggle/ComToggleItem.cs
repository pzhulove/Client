using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public delegate void OnComToggleClick(ComControlData comToggleData);

    public class ComToggleItem : MonoBehaviour
    {

        protected ComControlData _comToggleData = null;
        private OnComToggleClick _onComToggleClick = null;

        [Space(10)]
        [HeaderAttribute("ComToggleItem")]
        [Space(5)]
        [SerializeField] private Text normalName;
        [SerializeField] private Text selectedName;

        [SerializeField] private Toggle toggle;

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
            _comToggleData = null;
            _onComToggleClick = null;
        }

        public virtual void InitItem(ComControlData comToggleData,
            OnComToggleClick onComToggleClick = null)
        {

            //InitData
            //首先数据的重置
            ResetData();

            _comToggleData = comToggleData;
            _onComToggleClick = onComToggleClick;

            if (_comToggleData == null)
            {
                Logger.LogErrorFormat("ComToggleItem InitItem comToggleData is null");
                return;
            }

            InitItemView();
        }

        protected virtual void InitItemView()
        {
            if (normalName != null)
                normalName.text = _comToggleData.Name;

            if (selectedName != null)
                selectedName.text = _comToggleData.Name;

            if (toggle != null)
            {
                if (_comToggleData.IsSelected == true)
                {
                    if (toggle.isOn == true)
                        toggle.isOn = false;

                    toggle.isOn = true;
                }
                else
                {
                    toggle.isOn = false;
                }
            }
        }


        private void OnToggleClick(bool value)
        {

            if (_comToggleData == null)
            {
                return;
            }

            _comToggleData.IsSelected = value;

            if (value == true)
            {
                if (_onComToggleClick != null)
                {
                    _onComToggleClick(_comToggleData);
                }
            }
        }

        public void OnEnableComToggleItem()
        {
            if (_onComToggleClick != null && _comToggleData.IsSelected)
            {
                if (_onComToggleClick != null)
                    _onComToggleClick(_comToggleData);
            }
        }

        public void OnItemRecycle()
        {
            _comToggleData = null;
        }
        
    }
}
