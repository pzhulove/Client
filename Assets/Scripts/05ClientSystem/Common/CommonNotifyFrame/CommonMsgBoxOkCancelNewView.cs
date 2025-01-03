using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public delegate void OnCommonMsgBoxToggleClick(bool value);

    public class CommonMsgBoxOkCancelNewView : MonoBehaviour
    {

        private CommonMsgBoxOkCancelNewParamData _commonMsgBoxOkCancelNewParamData = null;

        private bool _isShowNotify = false;
        private OnCommonMsgBoxToggleClick _onCommonMsgBoxToggleClick;
        private Action _onLeftButtonClickCallBack;
        private Action _onRightButtonClickCallBack;

        private bool _isMiddleButton = false;
        private Action _onMiddleButtonClickCallBack;

        [Space(10)]
        [HeaderAttribute("Content")]
        [SerializeField] private Text contentLabelOne;      //区别于是否存在Notify，只是位置的不同
        [SerializeField] private Text contentLabelTwo;
        [SerializeField] private GameObject notifyToggleRoot;
        [SerializeField] private Toggle notifyToggle;

        [Space(10)]
        [HeaderAttribute("Button")]
        [SerializeField] private Button leftButton;
        [SerializeField] private Text leftButtonText;
        [SerializeField] private Button rightButton;
        [SerializeField] private Text rightButtonText;
        [SerializeField] private Button middleButton;
        [SerializeField] private Text middleButtonText;

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

            if (middleButton != null)
            {
                middleButton.onClick.RemoveAllListeners();
                middleButton.onClick.AddListener(OnMiddleButtonClick);
            }
        }

        private void UnBindEvents()
        {
            if(notifyToggle != null)
                notifyToggle.onValueChanged.RemoveAllListeners();

            if(leftButton != null)
                leftButton.onClick.RemoveAllListeners();

            if(rightButton != null)
                rightButton.onClick.RemoveAllListeners();
            
            if(middleButton != null)
                middleButton.onClick.RemoveAllListeners();
        }

        private void ClearData()
        {
            _onCommonMsgBoxToggleClick = null;
            _onLeftButtonClickCallBack = null;
            _onRightButtonClickCallBack = null;
            _isShowNotify = false;
            _commonMsgBoxOkCancelNewParamData = null;

            _isMiddleButton = false;
            _onMiddleButtonClickCallBack = null;

        }

        public void InitData(CommonMsgBoxOkCancelNewParamData paramData)
        {
            _commonMsgBoxOkCancelNewParamData = paramData;

            if (_commonMsgBoxOkCancelNewParamData == null)
            {
                OnCloseFrame();
                return;
            }

            _onCommonMsgBoxToggleClick = _commonMsgBoxOkCancelNewParamData.OnCommonMsgBoxToggleClick;
            _onLeftButtonClickCallBack = _commonMsgBoxOkCancelNewParamData.OnLeftButtonClickCallBack;
            _onRightButtonClickCallBack = _commonMsgBoxOkCancelNewParamData.OnRightButtonClickCallBack;
            _isShowNotify = _commonMsgBoxOkCancelNewParamData.IsShowNotify;

            _onMiddleButtonClickCallBack = _commonMsgBoxOkCancelNewParamData.OnMiddleButtonClickCallBack;
            _isMiddleButton = _commonMsgBoxOkCancelNewParamData.IsMiddleButton;

            InitView();

            if (_commonMsgBoxOkCancelNewParamData.IsDefaultCheck == true)
            {
                if (notifyToggle != null)
                {
                    notifyToggle.isOn = true;
                }
            }
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

            if (_isShowNotify == true)
            {
                if (contentLabelOne != null)
                {
                    contentLabelOne.text = _commonMsgBoxOkCancelNewParamData.ContentLabel;
                    contentLabelOne.gameObject.CustomActive(true);
                    if (_commonMsgBoxOkCancelNewParamData.ContentTextAnchor != TextAnchor.MiddleCenter)
                        contentLabelOne.alignment = _commonMsgBoxOkCancelNewParamData.ContentTextAnchor;
                }

                if (contentLabelTwo != null)
                {
                    contentLabelTwo.gameObject.CustomActive(false);
                }
            }
            else
            {
                if (contentLabelOne != null)
                    contentLabelOne.gameObject.CustomActive(false);

                if (contentLabelTwo != null)
                {
                    contentLabelTwo.text = _commonMsgBoxOkCancelNewParamData.ContentLabel;
                    contentLabelTwo.gameObject.CustomActive(true);
                    if (_commonMsgBoxOkCancelNewParamData.ContentTextAnchor != TextAnchor.MiddleCenter)
                        contentLabelTwo.alignment = _commonMsgBoxOkCancelNewParamData.ContentTextAnchor;
                }
            }


        }

        private void InitButton()
        {
            //显示两个按钮
            if (_isMiddleButton == false)
            {
                if (middleButton != null)
                {
                    middleButton.gameObject.CustomActive(false);
                }

                if (leftButton != null)
                {
                    leftButton.gameObject.CustomActive(true);

                    if (leftButtonText != null)
                        leftButtonText.text = _commonMsgBoxOkCancelNewParamData.LeftButtonText;
                }

                if (rightButton != null)
                {
                    rightButton.gameObject.CustomActive(true);

                    if (rightButtonText != null)
                        rightButtonText.text = _commonMsgBoxOkCancelNewParamData.RightButtonText;
                }
            }
            else
            {
                //显示一个按钮
                if (leftButton != null)
                    leftButton.gameObject.CustomActive(false);
                if (rightButton != null)
                    rightButton.gameObject.CustomActive(false);

                if (middleButton != null)
                {
                    middleButton.gameObject.CustomActive(true);

                    if (middleButtonText != null)
                        middleButtonText.text = _commonMsgBoxOkCancelNewParamData.MiddleButtonText;
                }
            }

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

        private void OnMiddleButtonClick()
        {
            OnCloseFrame();

            if (_onMiddleButtonClickCallBack != null)
            {
                _onMiddleButtonClickCallBack();
            }
        }

        private void OnCloseFrame()
        {
            ClientSystemManager.GetInstance().CloseFrame<CommonMsgBoxOkCancelNewFrame>();
        }

    }

}