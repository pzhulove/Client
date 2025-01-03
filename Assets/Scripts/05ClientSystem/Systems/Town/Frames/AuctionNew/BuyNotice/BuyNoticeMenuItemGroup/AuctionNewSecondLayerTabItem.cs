using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AuctionNewSecondLayerTabItem : MonoBehaviour
    {

        private bool _isSelected = false;
        private AuctionNewMenuTabDataModel _firstLayerTabDataModel = null;
        private AuctionNewMenuTabDataModel _secondLayerTabDataModel = null;
        private OnSecondLayerTabToggleClick _onSecondLayerTabToggleClick = null;


        [Space(10)]
        [HeaderAttribute("SecondLayerTab")]
        [Space(5)]
        [SerializeField] private Text tabName;

        [SerializeField] private Text selectedTabName;
        [SerializeField] private Toggle tabToggle;

        private void Awake()
        {
            if (tabToggle != null)
            {
                tabToggle.onValueChanged.RemoveAllListeners();
                tabToggle.onValueChanged.AddListener(OnTabClicked);
            }
        }

        private void OnDestroy()
        {
            if (tabToggle != null)
                tabToggle.onValueChanged.RemoveAllListeners();

            ResetData();
        }

        private void ResetData()
        {
            _isSelected = false;
            _firstLayerTabDataModel = null;
            _secondLayerTabDataModel = null;
            _onSecondLayerTabToggleClick = null;
        }

        public void InitTabItem(AuctionNewMenuTabDataModel firstLayerTabDataModel,
            AuctionNewMenuTabDataModel secondLayerTabDataModel,
            bool isSelected = false,
            OnSecondLayerTabToggleClick onSecondLayerTabToggleClick = null)
        {

            //InitData
            //首先数据的重置
            ResetData();

            _firstLayerTabDataModel = firstLayerTabDataModel;
            _secondLayerTabDataModel = secondLayerTabDataModel;
            _onSecondLayerTabToggleClick = onSecondLayerTabToggleClick;

            if (_secondLayerTabDataModel == null)
                return;
            if (_secondLayerTabDataModel.AuctionNewFrameTable == null)
                return;

            //InitView
            if (tabName != null)
                tabName.text = _secondLayerTabDataModel.AuctionNewFrameTable.Name;

            if (selectedTabName != null)
                selectedTabName.text = _secondLayerTabDataModel.AuctionNewFrameTable.Name;

            if (isSelected == true)
            {
                if (tabToggle != null)
                {
                    tabToggle.isOn = true;
                }
            }
        }


        private void OnTabClicked(bool value)
        {
            if (_secondLayerTabDataModel == null || _secondLayerTabDataModel.AuctionNewFrameTable == null)
                return;

            ////避免重复点击时，View重复更新
            //if (_isSelected == value)
            //    return;
            _isSelected = value;

            if (value == true)
            {
                if (_onSecondLayerTabToggleClick != null)
                {
                    _onSecondLayerTabToggleClick(_firstLayerTabDataModel, _secondLayerTabDataModel);
                }
            }        
        }

        //private void OnEnable()
        //{
        //    if (_isSelected == true)
        //    {
        //        if (_onSecondLayerTabToggleClick != null)
        //            _onSecondLayerTabToggleClick(_firstLayerTabDataModel, _menuTabDataModel);
        //    }
        //}

        public void OnEnabelTabItem()
        {
            if (_isSelected == true)
            {
                if (_onSecondLayerTabToggleClick != null)
                {
                    _onSecondLayerTabToggleClick(_firstLayerTabDataModel, _secondLayerTabDataModel);
                }
            }
        }

    }
}
