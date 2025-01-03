using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    //时装商城衣服的Tab Type：分为套装和单品
    public enum FashionMallClothTabType
    {
        None = -1,
        Suit = 0,        //套装
        Single = 1,        //单品
        Weapon = 2,         //武器
        Number,
    }

    [Serializable]
    public class FashionMallClothTabData
    {
        public int Index;
        public int MallTableId;         //对应商城表中的ID
        public string Name;             //页签名字
        public FashionMallClothTabType ClothTabType;    //页签类型
    }

    public delegate void OnFashionMallClothTabValueChanged(int index, FashionMallClothTabType clothTabType, int mallTableId);


    public class FashionMallClothTabItem : MonoBehaviour
    {

        private OnFashionMallClothTabValueChanged _onToggleValueChanged = null;

        private bool _isSelected = false;
        private FashionMallClothTabData _clothTabData = null;

        [SerializeField]
        private Text nameText;
        [SerializeField]
        private Toggle toggle;

        [SerializeField] private Image specialImage;

        private void Awake()
        {
            _isSelected = false;
            _clothTabData = null;
            BindUiEventSystem();
        }

        private void BindUiEventSystem()
        {
            if (toggle != null)
            {
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener(OnTabClicked);
            }
        }

        private void OnDestroy()
        {
            UnBindUiEventSystem();
        }

        private void UnBindUiEventSystem()
        {
            if (toggle != null)
            {
                toggle.onValueChanged.RemoveAllListeners();
            }
        }

        public void InitData(FashionMallClothTabData clothTabData,
            OnFashionMallClothTabValueChanged toggleValueChanged = null,
            bool isSelected = false)
        {
            _isSelected = false;
            _clothTabData = clothTabData;
            if (_clothTabData == null)
                return;

            if (nameText != null)
            {
                nameText.text = _clothTabData.Name;
            }

            InitSpecialImage();

            _onToggleValueChanged = toggleValueChanged;

            if (isSelected == true)
            {
                if (toggle != null)
                {
                    toggle.isOn = true;
                }
            }
        }

        //Tab上特殊的图标
        private void InitSpecialImage()
        {
            if (specialImage != null)
            {
                if (_clothTabData.ClothTabType == FashionMallClothTabType.Weapon)
                {
                    specialImage.gameObject.CustomActive(true);
                }
                else
                {
                    specialImage.gameObject.CustomActive(false);
                }
            }
        }

        private void OnTabClicked(bool value)
        {
            if (_clothTabData == null)
                return;

            //避免重复点击时，View重复更新
            if (_isSelected == value)
                return;
            _isSelected = value;

            if (value == true)
            {
                if (_onToggleValueChanged != null)
                {
                    _onToggleValueChanged(_clothTabData.Index,
                        _clothTabData.ClothTabType,
                        _clothTabData.MallTableId);
                }
            }
        }
    }
}
