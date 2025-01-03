using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{

    public delegate void OnCommonKeyBoardNumberClicked(KeyBoardNumberType keyBoardNumberType);

    public class CommonKeyBoardNumberItem : MonoBehaviour
    {

        private const string ItemNameBase = "Key_Number_";
        private KeyBoardNumberType _keyBoardNumberType;
        private OnCommonKeyBoardNumberClicked _onCommonKeyBoardNumberClicked;

        [Space(10)] [HeaderAttribute("Button")] [Space(10)] [SerializeField]
        private Button numberButton;

        [SerializeField] private Text numberValue;
        
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
            if (numberButton != null)
            {
                numberButton.onClick.RemoveAllListeners();
                numberButton.onClick.AddListener(OnKeyBoardNumberClicked);
            }
        }

        private void UnBindEvents()
        {
            if (numberButton != null)
                numberButton.onClick.RemoveAllListeners();

        }

        private void ClearData()
        {
            _keyBoardNumberType = KeyBoardNumberType.None;
            _onCommonKeyBoardNumberClicked = null;
        }

        public void InitItem(KeyBoardNumberType keyBoardNumberType,
            OnCommonKeyBoardNumberClicked onCommonKeyBoardNumberClicked)
        {
            _keyBoardNumberType = keyBoardNumberType;
            _onCommonKeyBoardNumberClicked = onCommonKeyBoardNumberClicked;

            if (_keyBoardNumberType >= KeyBoardNumberType.Zero && _keyBoardNumberType <= KeyBoardNumberType.Nine)
            {
                if (numberValue != null)
                    numberValue.text = ((int) _keyBoardNumberType).ToString();
            }

            //预制体名字
            gameObject.name = ItemNameBase + _keyBoardNumberType.ToString();
        }

        private void OnKeyBoardNumberClicked()
        {
            if (_onCommonKeyBoardNumberClicked != null)
                _onCommonKeyBoardNumberClicked(_keyBoardNumberType);
        }

    }
}
