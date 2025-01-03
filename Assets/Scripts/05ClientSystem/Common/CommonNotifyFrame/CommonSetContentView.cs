using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class CommonSetContentView : MonoBehaviour
    {

        private string _titleStr;
        private string _defaultEmptyStr;
        private int _maxWorldNumber;
        private UnityAction<string> _onOkClick;

        private string _inputContentStr;


        private CommonSetContentDataModel _setContentDataModel;

        [SerializeField] private Text titleLabel;

        [Space(10)] [HeaderAttribute("Input")] [Space(10)]
        [SerializeField] private Text inputWorldNumberLabel;

        [SerializeField] private Text contentPlaceHolderLabel;
        [SerializeField] private InputField contentInputField;

        [Space(10)] [HeaderAttribute("Button")] [Space(10)]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button okButton;

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
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseButtonClick);
            }

            if (okButton != null)
            {
                okButton.onClick.RemoveAllListeners();
                okButton.onClick.AddListener(OnOkButtonClick);
            }

            if (contentInputField != null)
            {
                contentInputField.onValueChanged.RemoveAllListeners();
                contentInputField.onValueChanged.AddListener(OnContentInputFieldValueChanged);
            }
        }

        private void UnBindEvents()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveAllListeners();

            if (okButton != null)
                okButton.onClick.RemoveAllListeners();

            if (contentInputField != null)
                contentInputField.onValueChanged.RemoveAllListeners();
        }

        private void ClearData()
        {
            _setContentDataModel = null;
            _onOkClick = null;
        }

        public void Init(CommonSetContentDataModel setContentDataModel)
        {
            _setContentDataModel = setContentDataModel;

            if (_setContentDataModel == null)
                return;

            _titleStr = _setContentDataModel.TitleStr;
            _defaultEmptyStr = setContentDataModel.DefaultEmptyStr;
            _maxWorldNumber = _setContentDataModel.MaxWordNumber;
            _onOkClick = _setContentDataModel.OnOkClicked;

            _inputContentStr = _setContentDataModel.DefaultContentStr;

            InitView();
        }

        private void InitView()
        {
            if (titleLabel != null)
                titleLabel.text = _titleStr;

            //默认的内容
            if (contentPlaceHolderLabel != null)
                contentPlaceHolderLabel.text = _defaultEmptyStr;

            if (contentInputField != null)
                contentInputField.text = _inputContentStr;

            UpdateInputWorldNumberLabel();

        }

        #region BindEvents

        private void OnOkButtonClick()
        {
            if (string.IsNullOrEmpty(_inputContentStr) == true)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("Common_Set_Content_With_None_Word"));
            }
            else
            {
                if (_inputContentStr.Length <= 0)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("Common_Set_Content_With_None_Word"));
                }
                else if (_inputContentStr.Length > _maxWorldNumber)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("Common_Set_Content_With_Exceed_Max_Word"));
                }
                else
                {
                    if (_onOkClick != null)
                        _onOkClick(_inputContentStr);
                }
            }
        }

        private void OnContentInputFieldValueChanged(string valueStr)
        {
            _inputContentStr = valueStr;

            UpdateInputWorldNumberLabel();
        }

        private void UpdateInputWorldNumberLabel()
        {
            var inputContentNumber = 0;
            if (string.IsNullOrEmpty(_inputContentStr) == false)
                inputContentNumber = _inputContentStr.Length;

            if (inputWorldNumberLabel != null)
            {
                var numberStr = TR.Value("Common_Two_Number_Format_One",
                    inputContentNumber,
                    _maxWorldNumber);
                inputWorldNumberLabel.text = numberStr;
            }
        }

        private void OnCloseButtonClick()
        {
            ClientSystemManager.GetInstance().CloseFrame<CommonSetContentFrame>();
        }
        #endregion


    }

}