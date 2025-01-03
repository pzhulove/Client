using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GameClient
{
    public class ExpUpgradeView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text curExpText;
        [SerializeField] private Text upgradeExpText;
        [SerializeField] private Text closeTipText;
        [SerializeField] private Slider expSlider;

        [SerializeField] private float showTimeInterval;    //frame显示时间
        [SerializeField] private int addExpFrame;           //经验增加动画持续的帧率

        private float _curExpValue = 100;
        private float _addExpValue = 300;
        private float _curTotalExpValue = 500;
        private float _addExpInterval = 0;

        private UnityAction _onTimeIntervalCloseCallBack;
        public UnityAction OnTimeIntervalCloseCallBack
        {
            get { return _onTimeIntervalCloseCallBack; }
            set
            {
                if (value != null)
                {
                    _onTimeIntervalCloseCallBack = value;
                }
            }
        }

        [SerializeField] private Button closeButton;
        private UnityAction _onButtonCloseCallBack;
        public UnityAction OnButtonCloseCallBack
        {
            set
            {
                if (closeButton != null)
                {
                    if (_onButtonCloseCallBack != null)
                    {
                        closeButton.onClick.RemoveListener(_onButtonCloseCallBack);
                    }

                    if (value != null)
                    {
                        _onButtonCloseCallBack = value;
                        closeButton.onClick.AddListener(_onButtonCloseCallBack);
                    }
                }
            }
        }
        
        private void Awake()
        {
            titleText.text = TR.Value("exp_upgrade_title");
            closeTipText.text = TR.Value("exp_upgrade_close_tip");
        }

        public void InitExpUpgradeData(ExpUpgradeData expUpgradeData)
        {
            _curExpValue = expUpgradeData.CurExpValue;
            _curTotalExpValue = expUpgradeData.MaxExpValue;
            _addExpValue = expUpgradeData.AddExpValue;

            curExpText.text = GetExpValueStr(_curExpValue, _curTotalExpValue);
            upgradeExpText.text = GetExpValueStr(_addExpValue, _curTotalExpValue);

            //经验值
            expSlider.maxValue = (float)_curTotalExpValue;
            expSlider.value = (float)_curExpValue;

            //默认动画播放帧率为30帧，或者配置决定
            if (addExpFrame <= 1)
            {
                addExpFrame = 30;
            }
            _addExpInterval = _addExpValue / (float)addExpFrame;

            //frame持续时间 至少为2.0f
            if (showTimeInterval < 2.0f)
            {
                showTimeInterval = 2.0f;
            }
        }

        private void Update()
        {
            //经验值增加
            if (addExpFrame >= 0)
            {
                if (addExpFrame == 0)
                {
                    expSlider.value = _curExpValue + _addExpValue;
                    addExpFrame = -1;
                }
                else
                {
                    expSlider.value = expSlider.value + _addExpInterval;
                    addExpFrame -= 1;
                }
            }

            //显示时间,之后关闭
            showTimeInterval -= Time.deltaTime;
            if (showTimeInterval <= 0.0f)
            {
                if (_onTimeIntervalCloseCallBack != null)
                {
                    _onTimeIntervalCloseCallBack();
                }
            }
        }

        private void OnDisable()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
            }
        }

        //helper
        private string GetExpValueStr(float value, float totalValue)
        {
            var curValue = value / totalValue;
            var curValueInt = (int)(curValue * 100);
            return string.Format("{0}%", curValueInt);
        }

    }
}
