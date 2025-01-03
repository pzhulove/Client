using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public delegate void OnMallTabClick(MallNewMainTabDataModel _mainTabDataModel);
    public class MallNewMainTabItem : MonoBehaviour
    {
        // private const string normalRechargeImagePath = "UI/Image/Packed/p_UI_Common00.png:UI_Tongyong_Anniu_Quren_Xuanzhong_01";
        // private const string selectedRechargeImagePath = "UI/Image/Packed/p_UI_Common00.png:UI_Tongyong_Shuxiang_Yeqian_Xuanzhong_02";
        // private const string normalImagePath = "UI/Image/Packed/p_UI_Common00.png:UI_Tongyong_Shuxiang_Yeqian_Changtai_01";
        // private const string selectedImagePath = "UI/Image/Packed/p_UI_Common00.png:UI_Tongyong_Shuxiang_Yeqian_Xuanzhong_01";
        public static MallNewMainTabItem selectItem;
        private bool mIsSelected = false;
        private MallNewMainTabDataModel mMainTabDataModel = null;
        private MallNewBaseView mContentView = null;

        [SerializeField] private Text mainTabName;
        [SerializeField] private Toggle mToggle;
        [SerializeField] private GameObject hot;
        // [SerializeField] private Image normalImage;
        // [SerializeField] private Image selectedImage;
        // [SerializeField] private GameObject tabUiEffectRoot;

        private OnMallTabClick mOnMallTabClick;
        private void Awake()
        {
            InitData();
            // if (mToggle != null)
            // {
            //     mToggle.onValueChanged.RemoveAllListeners();
            //     mToggle.onValueChanged.AddListener(OnTabClicked);
            // }
        }

        private void InitData()
        {
            mIsSelected = false;
            mMainTabDataModel = null;
            mContentView = null;
        }

        private void OnDestroy()
        {
            // if(mToggle != null)
            //     mToggle.onValueChanged.RemoveAllListeners();
            mMainTabDataModel = null;
            mContentView = null;
            mOnMallTabClick = null;
        }

        //初始化
        public void Init(MallNewMainTabDataModel mainTabDataModel, OnMallTabClick onMallTabClick, bool isSelected = false)
        {
            mMainTabDataModel = mainTabDataModel;
            mOnMallTabClick = onMallTabClick;
            if (mMainTabDataModel == null)
                return;

            if (mainTabName != null)
                mainTabName.text = mMainTabDataModel.mainTabName;

            if (isSelected && mToggle != null)
                mToggle.isOn = true;

            InitSpecialMainTabItem();

        }
        //充值商城页签显示特效
        // public void SetTabUiEffect(GameObject tabUiEffect)
        // {
        //     if (tabUiEffectRoot != null)
        //     {
        //         tabUiEffectRoot.CustomActive(true);
        //         if (tabUiEffect != null)
        //         {
        //             tabUiEffect.CustomActive(true);
        //             tabUiEffect.transform.SetParent(tabUiEffectRoot.transform, false);
        //             tabUiEffect.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        //         }
        //     }
        // }

        // public void HideTabUiEffect()
        // {
        //     if (tabUiEffectRoot != null)
        //     {
        //         tabUiEffectRoot.CustomActive(false);
        //     }
        // }

        //初始化特殊Tab的信息
        private void InitSpecialMainTabItem()
        {
            //限时商城Tab：限时热门图标
            if (mMainTabDataModel.mainTabType == MallNewType.LimitTimeMall)
            {
                if (hot != null)
                    hot.CustomActive(true);
            }
            else
            {
                if (hot != null)
                    hot.CustomActive(false);
            }

            //充值商城Tab：正常和选中Image显示特殊的Image
            // if (mMainTabDataModel.mainTabType == MallNewType.ReChargeMall)
            // {
            //     if (normalImage != null)
            //     {
            //         ETCImageLoader.LoadSprite(ref normalImage, normalRechargeImagePath);
            //     }

            //     if (selectedImage != null)
            //     {
            //         ETCImageLoader.LoadSprite(ref selectedImage, selectedRechargeImagePath);
            //     }
            // }
            // //其他商城 恢复正常
            // else
            // {
            //     if (normalImage != null)
            //     {
            //         ETCImageLoader.LoadSprite(ref normalImage, normalImagePath);
            //     }

            //     if (selectedImage != null)
            //     {
            //         ETCImageLoader.LoadSprite(ref selectedImage, selectedImagePath);
            //     }
            // }
        }

        public void SetOffToggle()
        {
            if (null != mToggle)
            {
                mToggle.group.allowSwitchOff = true;
                mToggle.isOn = false;
            }
        }

        //toggle 点击事件
        [SerializeField] private int mNormalTextSize;
        [SerializeField] private int mSelectTextSize;
        public void OnTabClicked(bool value)
        {
            mToggle.group.allowSwitchOff = false;
            if(mMainTabDataModel == null)
                return;
            mainTabName.fontSize = value ? mSelectTextSize : mNormalTextSize;
            //避免重复点击时，View重复更新
            if(mIsSelected == value)
                return;
            mIsSelected = value;
            if (value)
            {
                selectItem = this;
                MallNewFrame.CloseMallPayFrame();
                if (mMainTabDataModel.contentRoot != null)
                {
                    mMainTabDataModel.contentRoot.CustomActive(true);
                    if (null == mContentView)
                    {
                        LoadContentView();
                    }
                    else
                    {
                        mContentView.OnEnableView();
                    }
                }
                //添加埋点
                if (mMainTabDataModel.mainTabType == MallNewType.LimitTimeMall)
                {
                    Utility.DoStartFrameOperation("MallNewFrame", "LimitTimeMallBtn");
                }
                else if (mMainTabDataModel.mainTabType == MallNewType.ExChangeMall)
                {
                    Utility.DoStartFrameOperation("MallNewFrame", "ExChangeMallBtn");
                }
                else if (mMainTabDataModel.mainTabType == MallNewType.PropertyMall)
                {
                    Utility.DoStartFrameOperation("MallNewFrame", "PropertyMallBtn");
                }
            }
            else
            {
                if (mMainTabDataModel.contentRoot != null)
                {
                    mMainTabDataModel.contentRoot.CustomActive(false);
                }
            }

            if (mOnMallTabClick != null)
            {
                mOnMallTabClick.Invoke(mMainTabDataModel);
            }
        }

        //加载view的预制体
        private void LoadContentView()
        {
            var uiPrefabWrapper = mMainTabDataModel.contentRoot.GetComponent<UIPrefabWrapper>();
            if (uiPrefabWrapper != null)
            {
                var uiPrefab = uiPrefabWrapper.LoadUIPrefab();
                if (uiPrefab != null)
                {
                    uiPrefab.transform.SetParent(mMainTabDataModel.contentRoot.transform,false);
                }

                if (uiPrefab != null)
                {
                    mContentView = uiPrefab.GetComponent<MallNewBaseView>();
                    if (mContentView != null)
                    {
                        //只初始化一次
                        mContentView.InitData(MallNewFrame.DefaultIndex, MallNewFrame.SecondIndex, MallNewFrame.ThirdIndex);
                        MallNewFrame.DefaultIndex = 0;
                        MallNewFrame.SecondIndex = 0;
                        MallNewFrame.ThirdIndex = 0;
                    }
                }
            }
        }

    }
}
