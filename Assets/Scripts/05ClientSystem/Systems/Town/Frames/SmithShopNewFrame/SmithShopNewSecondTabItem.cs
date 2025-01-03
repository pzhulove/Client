using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SmithShopNewSecondTabItem : MonoBehaviour
    {
        [SerializeField] private Text mTabName;
        [SerializeField] private Text mSelectedTabName;
        [SerializeField] private Toggle mTabToggle;

        private SmithShopNewMainTabDataModel mMainTabDataModel = null;
        private SecondTabDataModel mSecondTabDataModel = null;
        private bool isSelected = false;
        private OnSecondTabToggleClick mOnSecondTabToggleClick = null;

        private GameObject mTabContentView = null;
        private StrengthenGrowthView mStrengthenGrowthView = null;
        private SmithShopNewLinkData mLinkData = null;
        private void Awake()
        {
            if (mTabToggle != null)
            {
                mTabToggle.onValueChanged.RemoveAllListeners();
                mTabToggle.onValueChanged.AddListener(OnTabToggleClick);
            }
        }

        private void OnDestroy()
        {
            ResetData();
        }

        private void ResetData()
        {
            mMainTabDataModel = null;
            mSecondTabDataModel = null;
            isSelected = false;
            mOnSecondTabToggleClick = null;
            mTabContentView = null;
            mLinkData = null;
        }

        public void InitTabItem(SmithShopNewMainTabDataModel mainTabDataModel,
            SecondTabDataModel secondTabDataModel,
            bool isSelected,
            StrengthenGrowthView view,
            SmithShopNewLinkData linkData,
            OnSecondTabToggleClick onSecondTabToggleClick)
        {
            ResetData();

            mMainTabDataModel = mainTabDataModel;
            mSecondTabDataModel = secondTabDataModel;
            mStrengthenGrowthView = view;
            mLinkData = linkData;
            mOnSecondTabToggleClick = onSecondTabToggleClick;

            if (mSecondTabDataModel == null || mMainTabDataModel == null)
            {
                return;
            }

            SetTabName(mSecondTabDataModel.name);

            if (isSelected == true)
            {
                if (mTabToggle != null)
                {
                    mTabToggle.isOn = true;
                }
            }
        }

        private void SetTabName(string name)
        {
            if (mTabName != null)
            {
                mTabName.text = name;
            }

            if (mSelectedTabName != null)
            {
                mSelectedTabName.text = name;
            }
        }

        private void OnTabToggleClick(bool value)
        {
            if (mSecondTabDataModel == null)
            {
                return;
            }

            if (isSelected == value) return;
            isSelected = value;

            if (value == true)
            {
                if (mSecondTabDataModel.content != null)
                {
                    mSecondTabDataModel.content.CustomActive(true);
                }

                if (mTabContentView == null)
                {
                    LoadContentView();
                }
                else
                {
                    if (mMainTabDataModel.tabType != SmithShopNewTabType.SSNTT_GROWTH)
                    {
                        var mView = mTabContentView.GetComponent<ISmithShopNewView>();
                        if (mView != null)
                        {
                            mView.OnEnableView();
                        }
                    }
                    else
                    {
                        var mView = mTabContentView.GetComponent<StrengthGrowthBaseView>();
                        if (mView != null)
                        {
                            mView.OnEnableView();
                        }
                    }
                }

                if (mOnSecondTabToggleClick != null)
                {
                    mOnSecondTabToggleClick.Invoke(mMainTabDataModel, mSecondTabDataModel);
                }
            }
            else
            {
                if (mSecondTabDataModel.content != null)
                {
                    mSecondTabDataModel.content.CustomActive(false);
                }

                if (mMainTabDataModel.tabType != SmithShopNewTabType.SSNTT_GROWTH)
                {
                    var mView = mTabContentView.GetComponent<ISmithShopNewView>();
                    if (mView != null)
                    {
                        mView.OnDisableView();
                    }
                }
                else
                {
                    var mView = mTabContentView.GetComponent<StrengthGrowthBaseView>();
                    if (mView != null)
                    {
                        mView.OnDisableView();
                    }
                }
            }
        }

        private void LoadContentView()
        {
            var uiPrefabWrapper = mSecondTabDataModel.content.GetComponent<UIPrefabWrapper>();
            if (uiPrefabWrapper != null)
            {
                GameObject uiPrefab = uiPrefabWrapper.LoadUIPrefab();
                if (uiPrefab != null)
                {
                    uiPrefab.transform.SetParent(mSecondTabDataModel.content.transform, false);
                    mTabContentView = uiPrefab;
                }
            }

            if (mTabContentView != null)
            {
                //如果等于激化
                if (mMainTabDataModel.tabType == SmithShopNewTabType.SSNTT_GROWTH)
                {
                    var mView = mTabContentView.GetComponent<StrengthGrowthBaseView>();

                    StrengthenGrowthType type = StrengthenGrowthType.SGT_NONE;

                    if (mSecondTabDataModel.index == (int)GrowthSubTabType.GSTT_GROWTH)
                    {
                        type = StrengthenGrowthType.SGT_Gtowth;
                    }
                    else if (mSecondTabDataModel.index == (int)GrowthSubTabType.GSTT_CLEAT)
                    {
                        type = StrengthenGrowthType.SGT_Clear;
                    }
                    else if (mSecondTabDataModel.index == (int)GrowthSubTabType.GSTT_ACTIVATE)
                    {
                        type = StrengthenGrowthType.SGT_Activate;
                    }
                    else if (mSecondTabDataModel.index == (int)GrowthSubTabType.GSTT_CHANGE)
                    {
                        type = StrengthenGrowthType.SGT_Change;
                    }

                    if (mView != null)
                    {
                        mView.IniteData(mLinkData, type, mStrengthenGrowthView);
                    }
                }
                else
                {
                    var mView = mTabContentView.GetComponent<ISmithShopNewView>();
                    if (mView != null)
                    {
                        mView.InitView(mLinkData);
                    }
                }
            }
        }

        public void OnEnabelTabItem(bool value = true)
        {
            if (isSelected == true)
            {
                if (value)
                {
                    if (mSecondTabDataModel != null)
                    {
                        mSecondTabDataModel.content.CustomActive(true);
                    }

                    if (mMainTabDataModel.tabType != SmithShopNewTabType.SSNTT_GROWTH)
                    {
                        var mView = mTabContentView.GetComponent<ISmithShopNewView>();
                        if (mView != null)
                        {
                            mView.OnEnableView();
                        }
                    }
                    else
                    {
                        var mView = mTabContentView.GetComponent<StrengthGrowthBaseView>();
                        if (mView != null)
                        {
                            mView.OnEnableView();
                        }
                    }

                    if (mOnSecondTabToggleClick != null)
                    {
                        mOnSecondTabToggleClick.Invoke(mMainTabDataModel, mSecondTabDataModel);
                    }
                }
                else
                {
                    if (mSecondTabDataModel != null)
                    {
                        mSecondTabDataModel.content.CustomActive(false);
                    }

                    if (mMainTabDataModel.tabType != SmithShopNewTabType.SSNTT_GROWTH)
                    {
                        var mView = mTabContentView.GetComponent<ISmithShopNewView>();
                        if (mView != null)
                        {
                            mView.OnDisableView();
                        }
                    }
                    else
                    {
                        var mView = mTabContentView.GetComponent<StrengthGrowthBaseView>();
                        if (mView != null)
                        {
                            mView.OnDisableView();
                        }
                    }
                }
            }
        }
    }
}