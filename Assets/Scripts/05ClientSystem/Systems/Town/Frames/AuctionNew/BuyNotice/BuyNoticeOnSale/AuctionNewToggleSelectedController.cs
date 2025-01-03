using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;


namespace GameClient
{

    public delegate void OnToggleSelectedClicked(bool isSelected);

    //Toggle选中的控制器
    public class AuctionNewToggleSelectedController : MonoBehaviour
    {
        
        private bool _isToggleSelected = false;
        private OnToggleSelectedClicked _onToggleSelectedClick = null;
        private string _labelStr;

        private float _curInterval;

        [SerializeField] private Text toggleLabel;
        [SerializeField] private Image selectedFlag;
        [SerializeField] private Button toggleButton;
        [SerializeField] private float buttonClickInterval = 0.5f;

        private void Awake()
        {
            BindEvents();
        }

        private void BindEvents()
        {
            if (toggleButton != null)
            {
                toggleButton.onClick.RemoveAllListeners();
                toggleButton.onClick.AddListener(OnToggleButtonClick);
            }
        }
        private void OnDestroy()
        {
            ResetData();
            UnBindEvents();
        }

        private void UnBindEvents()
        {
            if (toggleButton != null)
                toggleButton.onClick.RemoveAllListeners();
        }

        private void ResetData()
        {
            _isToggleSelected = false;
            _onToggleSelectedClick = null;
            _curInterval = 0.0f;
        }

        //初始化
        public void InitToggleSelectedController(bool isToggleSelected,
            string labelStr,
            OnToggleSelectedClicked onToggleClicked)
        {
            _isToggleSelected = isToggleSelected;
            _onToggleSelectedClick = onToggleClicked;

            _labelStr = labelStr;

            InitControllerView();
        }

        private void InitControllerView()
        {
            if (toggleLabel != null)
                toggleLabel.text = _labelStr;

            UpdateSelectedFlag();
        }

        private void UpdateSelectedFlag()
        {
            if (selectedFlag != null)
                selectedFlag.gameObject.CustomActive(_isToggleSelected);
        }

        private void OnToggleButtonClick()
        {
            //更新状态
            _isToggleSelected = !_isToggleSelected;
            UpdateSelectedFlag();

            //执行回调
            if (_onToggleSelectedClick != null)
                _onToggleSelectedClick(_isToggleSelected);

            SetButtonTimeLimit();
        }

        private void SetButtonTimeLimit()
        {
            if (buttonClickInterval > 0.0f)
            {
                //重置为不可点击
                UpdateButtonState(false);
                _curInterval = buttonClickInterval;
                //开启协程
                StartCoroutine(StartCountDown());
            }
        }
        
        //开启倒计时
        private IEnumerator StartCountDown()
        {
            while (_curInterval > 0.0f)
            {
                _curInterval -= 0.1f;
                yield return new WaitForSeconds(0.1f);
            }

            UpdateButtonState(true);
            yield break;
        }


        private void UpdateButtonState(bool flag)
        {
            if (toggleButton != null)
                toggleButton.interactable = flag;
        }

        public void ResetAuctionNewToggleSelectedController()
        {
            StopCountDown();
            UpdateButtonState(true);
        }

        public void StopCountDown()
        {
            StopCoroutine(StartCountDown());
        }

    }
}