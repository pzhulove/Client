using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AdventureTeamMainTabItem : MonoBehaviour
    {
        private bool _isSelected = false;
        private AdventureTeamMainTabDataModel _mainTabDataModel = null;
        private GameObject _mainTabContentView = null;

        [SerializeField] private Text mainTabName;
        [SerializeField] private Toggle toggle;       
        [SerializeField] private GameObject redPoint;

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
            _mainTabContentView = null;
        }

        private void OnDestroy()
        {
            if (toggle != null)
                toggle.onValueChanged.RemoveAllListeners();

            ResetData();
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

                if (_mainTabDataModel.contentRoot != null)
                {
                    _mainTabDataModel.contentRoot.CustomActive(true);

                    if (_mainTabContentView == null)
                    {
                        LoadContentView();
                    }
                    else
                    {
                        var teamBaseView = _mainTabContentView.GetComponent<AdventureTeamContentBaseView>();
                        if (teamBaseView != null)
                        {
                            teamBaseView.OnEnableView();
                        }
                    }
                }
            }
            else
            {
                if (_mainTabDataModel.contentRoot != null)
                {
                    _mainTabDataModel.contentRoot.CustomActive(false);

                    if (_mainTabContentView != null)
                    {
                        var teamBaseView = _mainTabContentView.GetComponent<AdventureTeamContentBaseView>();
                        if (teamBaseView != null)
                        {
                            teamBaseView.OnDisableView();
                        }
                    }
                }
            }
        }

        private void LoadContentView()
        {
            var uiPrefabWrapper = _mainTabDataModel.contentRoot.GetComponent<UIPrefabWrapper>();
            if (uiPrefabWrapper != null)
            {
                var uiPrefab = uiPrefabWrapper.LoadUIPrefab();
                if (uiPrefab != null)
                {
                    Utility.AttachTo(uiPrefab, _mainTabDataModel.contentRoot);
                    //uiPrefab.transform.SetParent(_mainTabDataModel.contentRoot.transform, false);
                    _mainTabContentView = uiPrefab;
                }
            }

            if (_mainTabContentView != null)
            {
                var teamBaseView = _mainTabContentView.GetComponent<AdventureTeamContentBaseView>();
                if(teamBaseView != null)
                    teamBaseView.InitData();
            }
        }

        #region PUBLIC METHOD
        public void Init(AdventureTeamMainTabDataModel mainTabDataModel, bool isSelected = false)
        {
            //首先数据的重置
            ResetData();

            _mainTabDataModel = mainTabDataModel;
            if (_mainTabDataModel == null)
                return;

            if (mainTabName != null)
            {
                mainTabName.text = _mainTabDataModel.mainTabName;
            }

            if (isSelected == true)
            {
                if (toggle != null)
                {
                    toggle.isOn = true;
                }
            }
        }

        public void SetRedPointEnable(bool bEnable)
        {
            redPoint.CustomActive(bEnable);
        }

        public AdventureTeamMainTabType GetTabType()
        {
            if (null != _mainTabDataModel)
            {
                return _mainTabDataModel.mainTabType;
            }
            return AdventureTeamMainTabType.None;
        }

        public bool IsTabSelected()
        {
            return _isSelected;
        }

        public void SetTabSelect()
        {
            if (toggle && !toggle.isOn)
            {
                toggle.isOn = true;
            }
        }

        #endregion
    }
}
