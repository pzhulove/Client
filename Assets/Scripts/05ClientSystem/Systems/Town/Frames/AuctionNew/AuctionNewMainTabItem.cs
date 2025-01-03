using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public delegate void OnAuctionMainTabClicked(AuctionNewMainTabType mainTabType);

    public class AuctionNewMainTabItem : MonoBehaviour
    {

        private bool _isSelected = false;
        private AuctionNewMainTabDataModel _mainTabDataModel = null;
        private OnAuctionMainTabClicked _onMainTabClicked = null;

        [SerializeField] private Text selectedTabName;
        [SerializeField] private Text normalTabName;            //未选中的名字
        [SerializeField] private Toggle toggle;

        private void Awake()
        {
            if (toggle != null)
            {
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener(OnTabClicked);
            }
        }

        private void ResetData()
        {
            _isSelected = false;
            _mainTabDataModel = null;
            _onMainTabClicked = null;
        }

        private void OnDestroy()
        {
            if (toggle != null)
                toggle.onValueChanged.RemoveAllListeners();

            ResetData();
        }


        public void Init(AuctionNewMainTabDataModel mainTabDataModel, 
            OnAuctionMainTabClicked onAuctionMainTabClicked,
            bool isSelected = false)
        {
            //首先数据的重置
            ResetData();

            _mainTabDataModel = mainTabDataModel;
            if (_mainTabDataModel == null)
                return;

            _onMainTabClicked = onAuctionMainTabClicked;

            if (selectedTabName != null)
            {
                if (string.IsNullOrEmpty(_mainTabDataModel.mainTabName) == false)
                    selectedTabName.text = TR.Value(_mainTabDataModel.mainTabName);
            }

            if (normalTabName != null)
            {
                if (string.IsNullOrEmpty(_mainTabDataModel.mainTabName) == false)
                    normalTabName.text = TR.Value(_mainTabDataModel.mainTabName);
            }

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
            if (_mainTabDataModel == null)
                return;

            //避免重复点击时，View重复更新
            if (_isSelected == value)
                return;
            _isSelected = value;

            if (value == true)
            {
                if (_onMainTabClicked != null)
                {
                    _onMainTabClicked(_mainTabDataModel.mainTabType);
                }
            }
        }

    }
}
