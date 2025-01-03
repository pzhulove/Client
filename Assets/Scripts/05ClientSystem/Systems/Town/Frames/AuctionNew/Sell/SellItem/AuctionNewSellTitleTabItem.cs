using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public delegate void OnAuctionNewSellTabClick(ActionNewSellTabType sellTabType);

    //销售的页签
    public class AuctionNewSellTitleTabItem : MonoBehaviour
    {

        private bool _isSelected = false;
        private AuctionNewSellTabDataModel _sellTabDataModel = null;
        private OnAuctionNewSellTabClick _onSellTabClick = null;

        [SerializeField] private Text normalTabName;
        [SerializeField] private Text selectedTabName;
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
            _sellTabDataModel = null;
            _onSellTabClick = null;
        }

        private void OnDestroy()
        {
            if (toggle != null)
                toggle.onValueChanged.RemoveAllListeners();

            ResetData();
        }


        public void Init(AuctionNewSellTabDataModel sellTabDataModel,
            OnAuctionNewSellTabClick onAuctionNewSellTabClick,
            bool isSelected = false)
        {
            _sellTabDataModel = sellTabDataModel;
            if (_sellTabDataModel == null)
                return;

            _onSellTabClick = onAuctionNewSellTabClick;

            if (normalTabName != null)
            {
                if (string.IsNullOrEmpty(_sellTabDataModel.sellTabName) == false)
                    normalTabName.text = TR.Value(_sellTabDataModel.sellTabName);
            }

            if (selectedTabName != null)
            {
                if (string.IsNullOrEmpty(_sellTabDataModel.sellTabName) == false)
                    selectedTabName.text = TR.Value(_sellTabDataModel.sellTabName);
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
            if (_sellTabDataModel == null)
                return;

            //避免重复点击时，View重复更新
            if (_isSelected == value)
                return;
            _isSelected = value;

            if (value == true)
            {

                if (_onSellTabClick != null)
                {
                    _onSellTabClick(_sellTabDataModel.sellTabType);
                }
            }
        }

    }
}
