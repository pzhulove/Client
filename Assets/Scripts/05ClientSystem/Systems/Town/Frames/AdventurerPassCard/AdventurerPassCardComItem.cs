using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AdventurerPassCardComItem : MonoBehaviour
    {
        [SerializeField] private Image mImgIcon;
        [SerializeField] private Image mImgQuality;
        [SerializeField] private Text mTextCount;
        private ItemData mItemData;
        public void OnInit(ItemData item)
        {
            mItemData = item;
            _ShowItemInfo();
        }

        private void _ShowItemInfo()
        {
            mImgIcon.SafeSetImage(mItemData.Icon);
            mImgQuality.SafeSetImage(GameUtility.Item.GetItemQualityBg(mItemData.Quality));
            if (mItemData.Count > 1)
            {
               mTextCount.SafeSetText(mItemData.Count.ToString());
            }
            else
            {
               mTextCount.SafeSetText(string.Empty);
            }
        }

        public void OnClickShowTip()
        {
            ItemData compareItem = _GetCompareItem(mItemData);
            if (compareItem != null)
            {
                ItemTipManager.GetInstance().ShowTipWithCompareItem(mItemData, compareItem, null);
            }
            else
            {
                ItemTipManager.GetInstance().ShowTip(mItemData, null, TextAnchor.MiddleLeft);
            }
        }
        ItemData _GetCompareItem(ItemData item)
        {
            ItemData compareItem = null;
            if (item != null && item.WillCanEquip())
            {
                List<ulong> guids = null;
                if (item.PackageType == EPackageType.Equip)
                {
                    guids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
                }
                else if (item.PackageType == EPackageType.Fashion)
                {
                    guids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearFashion);
                }
                if (guids != null)
                {
                    for (int i = 0; i < guids.Count; ++i)
                    {
                        ItemData tempItem = ItemDataManager.GetInstance().GetItem(guids[i]);
                        if (
                            tempItem != null &&
                            tempItem.GUID != item.GUID &&
                            tempItem.IsWearSoltEqual(item)
                            )
                        {
                            compareItem = tempItem;
                            break;
                        }
                    }
                }
            }
            return compareItem;
        }
    }
}
