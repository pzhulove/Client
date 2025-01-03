using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class GrowthStampsItem : MonoBehaviour
    {
        [SerializeField]private GameObject mItemParent;
        [SerializeField]private Text mName;
        [SerializeField]private Toggle mToggle;
        [SerializeField]private GameObject mCheckMark;

        private ComItemNew mComItem;
        public void OnItemVisiable(ItemData itemData,UnityEngine.Events.UnityAction<ItemData> OnToggleClick)
        {
            if (itemData == null)
            {
                return;
            }
            if (mComItem == null)
            {
                mComItem = ComItemManager.CreateNew(mItemParent);
            }

            mComItem.Setup(itemData, null);

            if (mName != null)
            {
                mName.text = itemData.GetColorName();
            }

            if (mToggle != null)
            {
                mToggle.onValueChanged.RemoveAllListeners();
                mToggle.onValueChanged.AddListener((bool value) => 
                {
                    if (value)
                    {
                        OnToggleClick(itemData);
                    }

                    mCheckMark.CustomActive(value);
                });
            }
        }

        public void ItemChangeDisplay()
        {
            mCheckMark.CustomActive(false);
        }

        private void OnDestroy()
        {
            mComItem = null;
        }
    }
}