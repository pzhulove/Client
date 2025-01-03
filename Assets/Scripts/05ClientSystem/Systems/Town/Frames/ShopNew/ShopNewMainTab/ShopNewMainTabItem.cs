using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    //商店中MainTab的类型：分为商店类型和道具类型
    public enum ShopNewMainTabType
    {
        None = -1,
        ItemType = 0,        //商品类型
        ShopType = 1,        //商店类型
    }

    public class ShopNewMainTabData
    {
        public ShopNewMainTabType MainTabType;      //类型
        public string Name;                         //名字
        public int Index;                           //大类型中对应的小数值

        public bool IsClicked;                      //是否点击过，主要是针对商店类型的Type，第一次点击的时候，可能需要向服务器请求数据

        public ShopNewFilterData FirstFilterData;
        public ShopNewFilterData SecondFilterData;
    }

    //第一个参数表示MainTab的索引
    //如果是商店类型，第三个参数为商店ID。如果是商品类型第三个参数需要转化为(ShopTable.SubType)index;
    public delegate void OnShopMainTabClickCallBack(int mainTabIndex, ShopNewMainTabData shopNewMainTabData);


    public class ShopNewMainTabItem : MonoBehaviour
    {

        private int _mainTabIndex = 0;

        private OnShopMainTabClickCallBack _onToggleValueChanged = null;

        private bool _isSelected = false;
        private ShopNewMainTabData _shopNewMainTabData = null;

        [SerializeField]
        private Text nameText;

        [SerializeField] private Text selectedNameText;
        [SerializeField]
        private Toggle toggle;

        private void Awake()
        {
            _isSelected = false;
            _mainTabIndex = 0;
            _shopNewMainTabData = null;
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

        public void InitData(int index,
            ShopNewMainTabData shopNewMainTabData,
            OnShopMainTabClickCallBack toggleValueChanged = null,
            bool isSelected = false)
        {
            _mainTabIndex = index;
            _isSelected = false;
            _shopNewMainTabData = shopNewMainTabData;
            if (_shopNewMainTabData == null)
                return;

            if (nameText != null)
            {
                nameText.text = _shopNewMainTabData.Name;
            }

            if (selectedNameText != null)
            {
                selectedNameText.text = _shopNewMainTabData.Name;
            }

            _onToggleValueChanged = toggleValueChanged;

            if (isSelected == true)
            {
                if (toggle != null)
                {
                    toggle.isOn = true;
                }
            }
        }

        private void OnTabClicked(bool value)
        {
            if (_shopNewMainTabData == null)
            {
                Logger.LogErrorFormat("SHopNewMainTabItem OnTabClicked and tabData is null");
                return;
            }
            
            //避免重复点击时，View重复更新
            if (_isSelected == value)
                return;
            _isSelected = value;

            if (value == true)
            {
                if (_onToggleValueChanged != null)
                {
                    _onToggleValueChanged(_mainTabIndex, _shopNewMainTabData);
                }
            }
        }
    }
}
