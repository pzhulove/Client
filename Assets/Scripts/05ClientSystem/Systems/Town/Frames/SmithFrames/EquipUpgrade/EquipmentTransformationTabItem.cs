using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public delegate void OnEquipmentTransformationTabClick(EquipmentTransformationTabData  equipmentTransformationTabData);
    public class EquipmentTransformationTabItem : MonoBehaviour
    {
        [SerializeField] private Text mTabName;
        [SerializeField] private Text mSelectedTabName;
        [SerializeField] private Image mNormalIcon;
        [SerializeField] private Image mCheckIcon;
        [SerializeField] private Toggle mTabToggle;

        private EquipmentTransformationTabData equipmentTransformationTabData = null;
        private bool isSelected = false;
        private OnEquipmentTransformationTabClick onEquipmentTransformationTabClick = null;

        private GameObject mTabContentView = null;
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
            equipmentTransformationTabData = null;
            isSelected = false;
            onEquipmentTransformationTabClick = null;
            mTabContentView = null;
            mLinkData = null;
        }

        public void InitTabItem(EquipmentTransformationTabData equipmentTransformationTabData,
            bool isSelected,
            SmithShopNewLinkData linkData,
            OnEquipmentTransformationTabClick onEquipmentTransformationTabClick)
        {
            if (equipmentTransformationTabData == null)
            {
                return;
            }

            ResetData();

            this.equipmentTransformationTabData = equipmentTransformationTabData;
            mLinkData = linkData;
            this.onEquipmentTransformationTabClick = onEquipmentTransformationTabClick;

            if (!string.IsNullOrEmpty(equipmentTransformationTabData.normalIconPath))
            {
                if (mNormalIcon != null)
                    ETCImageLoader.LoadSprite(ref mNormalIcon, equipmentTransformationTabData.normalIconPath);
            }

            if (!string.IsNullOrEmpty(equipmentTransformationTabData.checkIconPath))
            {
                if (mCheckIcon != null)
                    ETCImageLoader.LoadSprite(ref mCheckIcon, equipmentTransformationTabData.checkIconPath);
            }

            SetTabName(equipmentTransformationTabData.tabName);

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
            if (equipmentTransformationTabData == null)
            {
                return;
            }

            if (isSelected == value) return;
            isSelected = value;

            if (value == true)
            {
                if (equipmentTransformationTabData.content != null)
                {
                    equipmentTransformationTabData.content.CustomActive(true);
                }

                if (mTabContentView == null)
                {
                    LoadContentView();
                }
                else
                {
                    var mView = mTabContentView.GetComponent<ISmithShopNewView>();
                    if (mView != null)
                    {
                        mView.OnEnableView();
                    }
                }

                if (onEquipmentTransformationTabClick != null)
                {
                    onEquipmentTransformationTabClick.Invoke(equipmentTransformationTabData);
                }
            }
            else
            {
                if (equipmentTransformationTabData.content != null)
                {
                    equipmentTransformationTabData.content.CustomActive(false);
                }

                var mView = mTabContentView.GetComponent<ISmithShopNewView>();
                if (mView != null)
                {
                    mView.OnDisableView();
                }
            }
        }

        private void LoadContentView()
        {
            var uiPrefabWrapper = equipmentTransformationTabData.content.GetComponent<UIPrefabWrapper>();
            if (uiPrefabWrapper != null)
            {
                GameObject uiPrefab = uiPrefabWrapper.LoadUIPrefab();
                if (uiPrefab != null)
                {
                    uiPrefab.transform.SetParent(equipmentTransformationTabData.content.transform, false);
                    mTabContentView = uiPrefab;
                }
            }

            if (mTabContentView != null)
            {
                var mView = mTabContentView.GetComponent<ISmithShopNewView>();
                if (mView != null)
                {
                    mView.InitView(mLinkData);
                }
            }
        }

        public void OnEnabelTabItem(bool value = true)
        {
            if (isSelected == true)
            {
                if (value)
                {
                    if (equipmentTransformationTabData != null)
                    {
                        equipmentTransformationTabData.content.CustomActive(true);
                    }

                    var mView = mTabContentView.GetComponent<ISmithShopNewView>();
                    if (mView != null)
                    {
                        mView.OnEnableView();
                    }

                    if (onEquipmentTransformationTabClick != null)
                    {
                        onEquipmentTransformationTabClick.Invoke(equipmentTransformationTabData);
                    }
                }
                else
                {
                    if (equipmentTransformationTabData != null)
                    {
                        equipmentTransformationTabData.content.CustomActive(false);
                    }

                    var mView = mTabContentView.GetComponent<ISmithShopNewView>();
                    if (mView != null)
                    {
                        mView.OnDisableView();
                    }
                }
            }
        }
    }
}

