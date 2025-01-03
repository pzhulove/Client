using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AuctionNewMsgBoxOkCancelView : MonoBehaviour
    {

        private CommonMsgBoxOkCancelNewParamData _commonMsgBoxOkCancelNewParamData = null;

        private bool _isShowNotify = false;
        private OnCommonMsgBoxToggleClick _onCommonMsgBoxToggleClick;
        private Action _onLeftButtonClickCallBack;
        private Action _onRightButtonClickCallBack;

        [Space(10)]
        [HeaderAttribute("Content")]
        [SerializeField] private Text contentLabelOne;      //区别于是否存在Notify，只是位置的不同
        [SerializeField] private GameObject notifyToggleRoot;
        [SerializeField] private Toggle notifyToggle;

        [Space(10)]
        [HeaderAttribute("Button")]
        [SerializeField] private Button leftButton;
        [SerializeField] private Text leftButtonText;
        [SerializeField] private Button rightButton;
        [SerializeField] private Text rightButtonText;

        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }

        private void BindEvents()
        {
            if (notifyToggle != null)
            {
                notifyToggle.onValueChanged.RemoveAllListeners();
                notifyToggle.onValueChanged.AddListener(OnToggleClick);
            }

            if (leftButton != null)
            {
                leftButton.onClick.RemoveAllListeners();
                leftButton.onClick.AddListener(OnLeftButtonClick);
            }

            if (rightButton != null)
            {
                rightButton.onClick.RemoveAllListeners();
                rightButton.onClick.AddListener(OnRightButtonClick);
            }
        }

        private void UnBindEvents()
        {
            if (notifyToggle != null)
                notifyToggle.onValueChanged.RemoveAllListeners();

            if (leftButton != null)
                leftButton.onClick.RemoveAllListeners();

            if (rightButton != null)
                rightButton.onClick.RemoveAllListeners();
        }

        private void ClearData()
        {
            _onCommonMsgBoxToggleClick = null;
            _onLeftButtonClickCallBack = null;
            _onRightButtonClickCallBack = null;
            _isShowNotify = false;
            _commonMsgBoxOkCancelNewParamData = null;

        }

        public void InitData(CommonMsgBoxOkCancelNewParamData paramData)
        {
            _commonMsgBoxOkCancelNewParamData = paramData;

            if (_commonMsgBoxOkCancelNewParamData == null)
            {
                Logger.LogErrorFormat("AuctionNewMsgBoxOkCancelView InitData data is null");
                OnCloseFrame();
                return;
            }

            _onCommonMsgBoxToggleClick = _commonMsgBoxOkCancelNewParamData.OnCommonMsgBoxToggleClick;
            _onLeftButtonClickCallBack = _commonMsgBoxOkCancelNewParamData.OnLeftButtonClickCallBack;
            _onRightButtonClickCallBack = _commonMsgBoxOkCancelNewParamData.OnRightButtonClickCallBack;
            _isShowNotify = _commonMsgBoxOkCancelNewParamData.IsShowNotify;

            InitView();
        }

        private void InitView()
        {
            //初始化内容
            InitContent();
            //初始化按钮
            InitButton();
        }

        private void InitContent()
        {
            if (notifyToggleRoot != null)
            {
                notifyToggleRoot.gameObject.CustomActive(_isShowNotify);
            }

            if (contentLabelOne != null)
            {
                contentLabelOne.text = _commonMsgBoxOkCancelNewParamData.ContentLabel;
                contentLabelOne.gameObject.CustomActive(true);
            }
        }

        private void InitButton()
        {
            if (leftButtonText != null)
                leftButtonText.text = _commonMsgBoxOkCancelNewParamData.LeftButtonText;

            if (rightButtonText != null)
                rightButtonText.text = _commonMsgBoxOkCancelNewParamData.RightButtonText;            
        }

        private void OnToggleClick(bool value)
        {
            if (_onCommonMsgBoxToggleClick != null)
            {
                _onCommonMsgBoxToggleClick(value);
            }
        }

        private void OnLeftButtonClick()
        {
            OnCloseFrame();

            if (_onLeftButtonClickCallBack != null)
            {
                _onLeftButtonClickCallBack();
            }
        }

        private void OnRightButtonClick()
        {
            OnCloseFrame();

            if (_onRightButtonClickCallBack != null)
            {
                _onRightButtonClickCallBack();
            }
        }
        
        private void OnCloseFrame()
        {
            ClientSystemManager.GetInstance().CloseFrame<AuctionNewMsgBoxOkCancelFrame>();
        }

    }

}