using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SmithShopNewFirstTabItem : MonoBehaviour
    {
        [SerializeField] private Text mTabName;
        [SerializeField] private Text mSelectedTabName;
        [SerializeField] private Toggle mTabToggle;

        private bool isSelected = false;
        private SmithShopNewMainTabDataModel mMainTabDataModel = null;
        private bool isOwnerChildren = false;//是否存在子节点
        private bool isInitChildrenTab = false;//子页签是否进行了初始化

        private OnFirstTabToggleClick mOnFirstTabToggleClick;
        private OnSecondTabToggleClick mOnSecondTabToggleClick;
        private int mDefaultSecondTabIndex = 0;

        private List<SecondTabDataModel> mSecondTabDataModelList;

        private GameObject mTabContentView = null;
        private StrengthenGrowthView mStrengthenGrowthView = null;
        private SmithShopNewLinkData mLinkData = null;
        private List<SmithShopNewSecondTabItem> mSecondTabItemList = new List<SmithShopNewSecondTabItem>();
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
            isSelected = false;
            mMainTabDataModel = null;
            isOwnerChildren = false;//是否存在子节点
            isInitChildrenTab = false;//子页签是否进行了初始化
            mOnFirstTabToggleClick = null;
            mOnSecondTabToggleClick = null;
            mDefaultSecondTabIndex = 0;
            mSecondTabDataModelList = null;
            mTabContentView = null;
            mStrengthenGrowthView = null;
            mLinkData = null;
            mSecondTabItemList.Clear();
        }

        public void InitTabItem(SmithShopNewMainTabDataModel mainTabDataModel,
            List<SecondTabDataModel> secondTabDataModelList,
            bool isSelected,
            StrengthenGrowthView view,
            SmithShopNewLinkData linkData,
            int defaultSecondTabIndex = 0,
            OnFirstTabToggleClick onFirstTabToggleClick = null,
            OnSecondTabToggleClick onSecondTabToggleClick = null)
        {
            ResetData();

            mMainTabDataModel = mainTabDataModel;
            mSecondTabDataModelList = secondTabDataModelList;
            mDefaultSecondTabIndex = defaultSecondTabIndex;
            mStrengthenGrowthView = view;
            mLinkData = linkData;

            mOnFirstTabToggleClick = onFirstTabToggleClick;
            mOnSecondTabToggleClick = onSecondTabToggleClick;

            if (mMainTabDataModel == null)
            {
                return;
            }

            isOwnerChildren = false;
            if (mMainTabDataModel.isSunTAB == true)
            {
                isOwnerChildren = true;
            }

            if (mTabName != null)
            {
                mTabName.text = mMainTabDataModel.tabName;
            }

            if (mSelectedTabName != null)
            {
                mSelectedTabName.text = mMainTabDataModel.tabName;
            }
            
            if (isSelected == true)
            {
                if (mTabToggle != null)
                {
                    mTabToggle.isOn = true;
                }
            }
        }
        
        private void OnTabToggleClick(bool value)
        {
            if (mMainTabDataModel == null)
            {
                return;
            }

            if (isSelected == value)
            {
                return;
            }

            isSelected = value;
            
            if (value == true)
            {
                SmithShopNewFrameView.SetLastSelectItem(mMainTabDataModel.tabType);

                if (mMainTabDataModel.content != null)
                {
                    mMainTabDataModel.content.CustomActive(true);
                }

                if (mTabContentView == null)
                {
                    LoadContentView();
                }
                else
                {
                    if (mMainTabDataModel.tabType != SmithShopNewTabType.SSNTT_STRENGTHEN &&
                        mMainTabDataModel.tabType != SmithShopNewTabType.SSNTT_GROWTH)
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

                if (mOnFirstTabToggleClick != null)
                {
                    mOnFirstTabToggleClick.Invoke(mMainTabDataModel);
                }
            }
            else
            {
                if (mMainTabDataModel.content != null)
                {
                    mMainTabDataModel.content.CustomActive(false);
                }

                if (mMainTabDataModel.tabType != SmithShopNewTabType.SSNTT_STRENGTHEN &&
                    mMainTabDataModel.tabType != SmithShopNewTabType.SSNTT_GROWTH)
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

                SmithShopNewFrameView.mLastSelectedItemData = SmithShopNewFrameView.GetLastSelectItem(mMainTabDataModel.tabType);
            }
        }

        private void LoadContentView()
        {
            if (mMainTabDataModel == null || mMainTabDataModel.content == null)
            {
                return;
            }

            var uiPrefabWrapper = mMainTabDataModel.content.GetComponent<UIPrefabWrapper>();
            if (uiPrefabWrapper != null)
            {
                GameObject uiPrefab = uiPrefabWrapper.LoadUIPrefab();
                if (uiPrefab != null)
                {
                    uiPrefab.transform.SetParent(mMainTabDataModel.content.transform, false);
                    mTabContentView = uiPrefab;
                }
            }

            if (mTabContentView != null)
            {
                //如果等于强化
                if (mMainTabDataModel.tabType == SmithShopNewTabType.SSNTT_STRENGTHEN)
                {
                    var mView = mTabContentView.GetComponent<StrengthGrowthBaseView>();
                    if (mView != null)
                    {
                        mView.IniteData(mLinkData, StrengthenGrowthType.SGT_Strengthen, mStrengthenGrowthView);
                    }
                }
                else if (mMainTabDataModel.tabType == SmithShopNewTabType.SSNTT_GROWTH)
                {
                    var mView = mTabContentView.GetComponent<StrengthGrowthBaseView>();
                    if (mView != null)
                    {
                        mView.IniteData(mLinkData, StrengthenGrowthType.SGT_Gtowth, mStrengthenGrowthView);
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
    }
}