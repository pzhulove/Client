using System;
using System.Collections.Generic;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class StorageSelectItem : MonoBehaviour
    {

        private int _itemIndex;
        private int _totalOwnerItemNumber;
        private OnStorageSelectItemClick _onStorageSelectItemClick;
        private EPackageType _curPackageType = EPackageType.RoleStorage;

        [Space(10)] [HeaderAttribute("Content")] [Space(10)]
        [SerializeField] private Button ItemSelectButton;

        [SerializeField] private Text itemNameLabel;
        [SerializeField] private Text itemNameLabelSelect;
        [SerializeField] private GameObject itemLockImage;
        [SerializeField] private Image itemButtonImage;
        [SerializeField] private CanvasGroup itemButtonCanvasSelect;

        private void Awake()
        {
            if (ItemSelectButton != null)
            {
                ItemSelectButton.onClick.RemoveAllListeners();
                ItemSelectButton.onClick.AddListener(OnItemSelectButtonClick);
            }
        }

        private void OnDestroy()
        {
            if(ItemSelectButton != null)
                ItemSelectButton.onClick.RemoveAllListeners();

            _itemIndex = 0;
            _totalOwnerItemNumber = 0;
            _onStorageSelectItemClick = null;
        }

        public void InitItem(int itemIndex,
            OnStorageSelectItemClick onRoleStorageSelectItemClick, EPackageType ePackageType = EPackageType.RoleStorage)
        {
            _itemIndex = itemIndex;
            _onStorageSelectItemClick = onRoleStorageSelectItemClick;
            _curPackageType = ePackageType;

            //OnUpdateItem(ownerItemNumber);
        }

        public void OnUpdateItem(int ownerItemNumber)
        {
            _totalOwnerItemNumber = ownerItemNumber;
            UpdateSelectItem();
        }

        public void OnUpdateItemName()
        {
            UpdateSelectItem();
        }

        public void UpdateSelectItem()
        {
            int curSelectIndex = 0;
            if (_curPackageType == EPackageType.RoleStorage)
            {
                curSelectIndex = StorageDataManager.GetInstance().GetRoleStorageCurrentSelectedIndex();
            }
            else
            {
                curSelectIndex = StorageDataManager.GetInstance().GetAccountStorageCurrentSelectedIndex();
            }
            itemButtonCanvasSelect.CustomActive(curSelectIndex == _itemIndex);

            //未解锁
            if (_itemIndex > _totalOwnerItemNumber)
            {
                CommonUtility.UpdateGameObjectVisible(itemLockImage, true);
                if (itemButtonImage != null)
                {
                    //透明度60%
                    itemButtonImage.color = new Color(255.0f,255.0f,255.0f, 0.6f);
                }
                CommonUtility.UpdateTextVisible(itemNameLabel, false);
            }
            else
            {
                //解锁
                CommonUtility.UpdateGameObjectVisible(itemLockImage, false);
                if (itemButtonImage != null)
                {
                    //透明度60%
                    itemButtonImage.color = new Color(255.0f, 255.0f, 255.0f, 1.0f);
                }

                CommonUtility.UpdateTextVisible(itemNameLabel, true);

                //item的名字
                var itemNameStr = StorageUtility.GetStorageNameByStorageIndex(_itemIndex, _curPackageType);
                itemNameLabel.SafeSetText(itemNameStr);
                itemNameLabelSelect.SafeSetText(itemNameStr);
            }
        }

        private void OnItemSelectButtonClick()
        {
            //未解锁
            if (_itemIndex > _totalOwnerItemNumber)
            {
                StorageUnLockCostDataModel storageUnLockCostDataModel = StorageUtility.GetStorageUnLockCostDataModel(
                    _totalOwnerItemNumber, _curPackageType);

                if (storageUnLockCostDataModel == null)
                    return;

                StorageUtility.OnOpenStorageUnLockTipFrame(storageUnLockCostDataModel, _curPackageType);

            }
            else
            {
                //解锁
                if (_onStorageSelectItemClick != null)
                {
                    _onStorageSelectItemClick(_itemIndex);
                }
            }
        }

        public int GetItemIndex()
        {
            return _itemIndex;
        }

    }
}