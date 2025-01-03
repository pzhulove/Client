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

    public delegate bool OnReturnButtonClickAction();
    public class ComButtonWithCd : MonoBehaviour
    {

        private Action _onButtonClickAction = null;
        private OnReturnButtonClickAction _onReturnButtonClickAction = null;
        private float _curInterval;

        private string _defaultStr;
        private string _countDownTimeStrFormat;

        [SerializeField] private Button button;
        [SerializeField] private UIGray buttonGray;
        [SerializeField] private float buttonCdTime = 0.5f;
        [SerializeField] private float buttonCdInterval = 0.1f;

        [Space(25)] [HeaderAttribute("CountDownTimeLabel")] [Space(15)]
        [SerializeField] private Text countDownTimeLabel;

        private void Awake()
        {
            BindEvents();
        }

        private void BindEvents()
        {
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnButtonClick);
            }
        }
        private void OnDestroy()
        {
            ResetData();
            UnBindEvents();
        }

        private void UnBindEvents()
        {
            if (button != null)
                button.onClick.RemoveAllListeners();
        }

        public void SetButtonListener(Action onClick)
        {
            _onButtonClickAction = onClick;
        }

        public void ResetListener()
        {
            _onButtonClickAction = null;
            _onReturnButtonClickAction = null;
        }

        public void ResetButtonListener()
        {
            _onButtonClickAction = null;
        }

        public void SetReturnButtonListener(OnReturnButtonClickAction returnButtonClickAction)
        {
            _onReturnButtonClickAction = returnButtonClickAction;
        }

        public void ResetReturnButtonListener()
        {
            _onReturnButtonClickAction = null;
        }

        private void ResetData()
        {
            _curInterval = 0.0f;
            _onButtonClickAction = null;
            _onReturnButtonClickAction = null;
        }

        private void OnButtonClick()
        {
            if (_onButtonClickAction != null)
                _onButtonClickAction();

            if (_onReturnButtonClickAction != null)
            {
                var flag = _onReturnButtonClickAction();
                //中间断掉
                if (flag == false)
                    return;
            }

            SetButtonTimeLimit(buttonCdTime);
        }

        public void SetButtonTimeLimit(float cdTime)
        {
            if (cdTime <= 0)
                return;

            //重置为不可点击
            UpdateButtonState(false);
            _curInterval = cdTime;
            //开启协程
            StartCoroutine(StartCountDown());
        }

        //开启倒计时
        private IEnumerator StartCountDown()
        {
            while (_curInterval > 0.0f)
            {
                //至少为0.1f
                if (buttonCdInterval <= 0.1f)
                {
                    _curInterval -= 0.1f;
                }
                else
                {
                    _curInterval -= buttonCdInterval;
                }
                SetCountDownTimeLabel();
                yield return new WaitForSeconds(0.1f);
            }

            UpdateButtonState(true);
            ResetCountDownTimeLabel();
            yield break;
        }


        public void UpdateButtonState(bool flag)
        {
            if (button != null)
                button.interactable = flag;

            if (buttonGray != null)
                buttonGray.enabled = !flag;
        }

        #region CountDownTimeStr
        public void SetCountDownTimeDescription(string defaultStr = null, string countDownTimeFormat = null)
        {
            _defaultStr = defaultStr;
            _countDownTimeStrFormat = countDownTimeFormat;
        }

        private void SetCountDownTimeLabel()
        {
            if (countDownTimeLabel == null)
                return;

            if (string.IsNullOrEmpty(_countDownTimeStrFormat) == true)
                return;

            var curCountDownTimeStr = string.Format(_countDownTimeStrFormat, (int) _curInterval + 1);
            countDownTimeLabel.text = curCountDownTimeStr;
        }

        private void ResetCountDownTimeLabel()
        {
            if (countDownTimeLabel == null)
                return;

            if (string.IsNullOrEmpty(_defaultStr) == true)
                return;

            countDownTimeLabel.text = _defaultStr;
        }


        #endregion



        public void Reset()
        {
            StopCountDown();
            UpdateButtonState(true);
            ResetCountDownTimeLabel();
        }

        public void StopCountDown()
        {
            StopAllCoroutines();
        }

        public float GetButtonCdTime()
        {
            return buttonCdTime;
        }

    }
}