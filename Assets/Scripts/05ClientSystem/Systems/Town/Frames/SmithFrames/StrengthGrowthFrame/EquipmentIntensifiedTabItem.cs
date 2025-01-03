using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public delegate void OnEquipmentIntensifiedTabClick(EquipmentIntensifiedTabData equipmentIntensifiedTabData);
    public class EquipmentIntensifiedTabItem : MonoBehaviour
    {
        [SerializeField] private Text mTabName;
        [SerializeField] private Text mSelectedTabName;
        [SerializeField] private Image mNormalIcon;
        [SerializeField] private Image mCheckIcon;
        [SerializeField] private Toggle mTabToggle;

        private EquipmentIntensifiedTabData equipmentIntensifiedTabData = null;
        private bool isSelected = false;
        private OnEquipmentIntensifiedTabClick  onEquipmentIntensifiedTabClick = null;

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
            equipmentIntensifiedTabData = null;
            isSelected = false;
            onEquipmentIntensifiedTabClick = null;
            mTabContentView = null;
            mLinkData = null;
        }

        public void InitTabItem(EquipmentIntensifiedTabData equipmentIntensifiedTabData,
            bool isSelected,
            StrengthenGrowthView view,
            SmithShopNewLinkData linkData,
            OnEquipmentIntensifiedTabClick  onEquipmentIntensifiedTabClick)
        {
            ResetData();
            
            this.equipmentIntensifiedTabData = equipmentIntensifiedTabData;
            mStrengthenGrowthView = view;
            mLinkData = linkData;
            this.onEquipmentIntensifiedTabClick = onEquipmentIntensifiedTabClick;

            if (equipmentIntensifiedTabData == null)
            {
                return;
            }

            if(!string.IsNullOrEmpty(equipmentIntensifiedTabData.normalIconPath))
            {
                if (mNormalIcon != null)
                    ETCImageLoader.LoadSprite(ref mNormalIcon, equipmentIntensifiedTabData.normalIconPath);
            }

            if (!string.IsNullOrEmpty(equipmentIntensifiedTabData.checkIconPath))
            {
                if (mCheckIcon != null)
                    ETCImageLoader.LoadSprite(ref mCheckIcon, equipmentIntensifiedTabData.checkIconPath);
            }

            SetTabName(equipmentIntensifiedTabData.tabName);

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
            if (equipmentIntensifiedTabData == null)
            {
                return;
            }

            if (isSelected == value) return;
            isSelected = value;

            if (value == true)
            {
                if (equipmentIntensifiedTabData.content != null)
                {
                    equipmentIntensifiedTabData.content.CustomActive(true);
                }

                if (mTabContentView == null)
                {
                    LoadContentView();
                }
                else
                {
                    var mView = mTabContentView.GetComponent<StrengthGrowthBaseView>();
                    if (mView != null)
                    {
                        mView.OnEnableView();
                    }
                }

                if (onEquipmentIntensifiedTabClick != null)
                {
                    onEquipmentIntensifiedTabClick.Invoke(equipmentIntensifiedTabData);
                }
            }
            else
            {
                if (equipmentIntensifiedTabData.content != null)
                {
                    equipmentIntensifiedTabData.content.CustomActive(false);
                }

                var mView = mTabContentView.GetComponent<StrengthGrowthBaseView>();
                if (mView != null)
                {
                    mView.OnDisableView();
                }
            }
        }

        private void LoadContentView()
        {
            var uiPrefabWrapper = equipmentIntensifiedTabData.content.GetComponent<UIPrefabWrapper>();
            if (uiPrefabWrapper != null)
            {
                GameObject uiPrefab = uiPrefabWrapper.LoadUIPrefab();
                if (uiPrefab != null)
                {
                    uiPrefab.transform.SetParent(equipmentIntensifiedTabData.content.transform, false);
                    mTabContentView = uiPrefab;
                }
            }

            if (mTabContentView != null)
            {
                var mView = mTabContentView.GetComponent<StrengthGrowthBaseView>();

                StrengthenGrowthType type = StrengthenGrowthType.SGT_NONE;

                if (equipmentIntensifiedTabData.id == (int)GrowthSubTabType.GSTT_GROWTH)
                {
                    type = StrengthenGrowthType.SGT_Gtowth;
                }
                else if (equipmentIntensifiedTabData.id == (int)GrowthSubTabType.GSTT_CLEAT)
                {
                    type = StrengthenGrowthType.SGT_Clear;
                }
                else if (equipmentIntensifiedTabData.id == (int)GrowthSubTabType.GSTT_ACTIVATE)
                {
                    type = StrengthenGrowthType.SGT_Activate;
                }
                else if (equipmentIntensifiedTabData.id == (int)GrowthSubTabType.GSTT_CHANGE)
                {
                    type = StrengthenGrowthType.SGT_Change;
                }

                if (mView != null)
                {
                    mView.IniteData(mLinkData, type, mStrengthenGrowthView);
                }
            }
        }

        public void OnEnabelTabItem(bool value = true)
        {
            if (isSelected == true)
            {
                if (value)
                {
                    if (equipmentIntensifiedTabData != null)
                    {
                        equipmentIntensifiedTabData.content.CustomActive(true);
                    }

                    var mView = mTabContentView.GetComponent<StrengthGrowthBaseView>();
                    if (mView != null)
                    {
                        mView.OnEnableView();
                    }

                    if (onEquipmentIntensifiedTabClick != null)
                    {
                        onEquipmentIntensifiedTabClick.Invoke(equipmentIntensifiedTabData);
                    }
                }
                else
                {
                    if (equipmentIntensifiedTabData != null)
                    {
                        equipmentIntensifiedTabData.content.CustomActive(false);
                    }

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

