using System;
using System.Collections.Generic;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class StorageSelectView : MonoBehaviour
    {

        [Space(10)] [HeaderAttribute("Bg")] [Space(10)]
        [SerializeField] private GameObject firstBg;
        [SerializeField] private GameObject secondBg;
        [SerializeField] private GameObject thirdBg;

        [Space(20)]
        [HeaderAttribute("SelectItem")]
        [Space(10)]
        [SerializeField]
        private List<StorageSelectItem> roleStorageSelectItemList = new List<StorageSelectItem>();

        private EPackageType mCurPackageType = EPackageType.Storage;
        

        private void Awake()
        {

        }

        private void OnDestroy()
        {
        }

        //初始化RoleDropDownView
        public void InitStorageDropDownView(OnStorageSelectItemClick onStorageSelectItemClick, EPackageType ePackageType = EPackageType.Storage)
        {
            //初始化 SelectItem
            if (roleStorageSelectItemList != null && roleStorageSelectItemList.Count > 0)
            {
                var itemCount = roleStorageSelectItemList.Count;
                for (var i = 0; i < itemCount; i++)
                {
                    var selectItem = roleStorageSelectItemList[i];
                    if (selectItem == null)
                        continue;

                    var itemIndex = i + 1;
                    selectItem.InitItem(itemIndex, onStorageSelectItemClick, ePackageType);
                }
            }

            mCurPackageType = ePackageType;

            //更新
            OnUpdateRoleStorageDropDownView();            
        }

        //整体更新RoleDropDownView
        public void OnUpdateRoleStorageDropDownView()
        {
            int totalOwnerNumber = 0;
            if (mCurPackageType == EPackageType.Storage)
            {
                totalOwnerNumber = StorageDataManager.GetInstance().GetAccountStorageOwnerStorageNumber();
            }
            else if (mCurPackageType == EPackageType.RoleStorage)
            {
                totalOwnerNumber = StorageDataManager.GetInstance().GetRoleStorageOwnerStorageNumber();
            }
            UpdateRoleStorageDropDownBg(totalOwnerNumber);

            if (roleStorageSelectItemList == null || roleStorageSelectItemList.Count <= 0)
                return;

            var needShowItemNumber = StorageUtility.GetNeedShowItemNumber(totalOwnerNumber);

            var itemCount = roleStorageSelectItemList.Count;

            for (var i = 0; i < itemCount; i++)
            {
                var selectItem = roleStorageSelectItemList[i];
                if (selectItem == null)
                    continue;

                if (i < needShowItemNumber)
                {
                    CommonUtility.UpdateGameObjectVisible(selectItem.gameObject, true);
                    selectItem.OnUpdateItem(totalOwnerNumber);
                }
                else
                {
                    CommonUtility.UpdateGameObjectVisible(selectItem.gameObject, false);
                }
            }
        }

        public void OnUpdateStorageSelect(int itemPreIndex, int itemNextIndex)
        {
            if (roleStorageSelectItemList != null && roleStorageSelectItemList.Count > 0)
            {
                for (var i = 0; i < roleStorageSelectItemList.Count; i++)
                {
                    var selectItem = roleStorageSelectItemList[i];
                    if (selectItem == null)
                    {
                        continue;
                    }

                    if (selectItem.GetItemIndex() != itemPreIndex && selectItem.GetItemIndex() != itemNextIndex)
                    {
                        continue;
                    }

                    selectItem.UpdateSelectItem();
                }
            }
        }
        
        //更新名字
        public void OnUpdateRoleStorageDropDownName(int itemIndex)
        {
            if (roleStorageSelectItemList != null && roleStorageSelectItemList.Count > 0)
            {
                for (var i = 0; i < roleStorageSelectItemList.Count; i++)
                {
                    var selectItem = roleStorageSelectItemList[i];
                    if(selectItem == null)
                        continue;

                    if(selectItem.GetItemIndex() != itemIndex)
                        continue;

                    selectItem.OnUpdateItemName();
                }
            }
        }


        private void UpdateRoleStorageDropDownBg(int totalNumber)
        {
            CommonUtility.UpdateGameObjectVisible(firstBg, false);
            CommonUtility.UpdateGameObjectVisible(secondBg, false);
            CommonUtility.UpdateGameObjectVisible(thirdBg, false);

            var needShowLineNumber = StorageUtility.GetNeedShowLineNumber(totalNumber);

            if (needShowLineNumber == 1)
            {
                CommonUtility.UpdateGameObjectVisible(firstBg, true);
            }
            else if (needShowLineNumber == 2)
            {
                CommonUtility.UpdateGameObjectVisible(secondBg, true);
            }
            else
            {
                CommonUtility.UpdateGameObjectVisible(thirdBg, true);
            }
        }
    }
}