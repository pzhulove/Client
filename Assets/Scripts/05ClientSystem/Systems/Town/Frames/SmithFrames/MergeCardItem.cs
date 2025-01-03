using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public delegate void OnEmptyClick(bool isCardA);
    public class MergeCardItem : MonoBehaviour
    {
        [SerializeField] private GameObject mCardRoot;
        [SerializeField] private GameObject mItemParent;
        [SerializeField] private Text mCardName;
        [SerializeField] private Text mCardAttr;
        [SerializeField] private Text mCardLevel;

        [SerializeField] private bool IsCardA;

        private OnEmptyClick mOnEmptyClick;
        private ComItemNew mComItem;
      
        private void OnDestroy()
        {
            mOnEmptyClick = null;
            mComItem = null;
        }

        public void InitMergeCardItem(OnEmptyClick onEmptyClick)
        {
            mOnEmptyClick = onEmptyClick;

            if (mComItem == null)
            {
                mComItem = ComItemManager.CreateNew(mItemParent);
            }

            mComItem.Setup(null, null);
        }

        public void UpdateMergeCardItem(ItemData itemData)
        {
            if (itemData == null)
            {
                return;
            }

            var cardItemData = ItemDataManager.CreateItemDataFromTable(itemData.TableID);
            if (cardItemData == null)
            {
                return;
            }

            cardItemData.mPrecEnchantmentCard.iEnchantmentCardLevel = itemData.mPrecEnchantmentCard.iEnchantmentCardLevel;

            mCardRoot.CustomActive(true);

            if (mComItem != null)
            {
                mComItem.Setup(cardItemData, Utility.OnItemClicked);
            }

            if (mCardName != null)
            {
                mCardName.text = cardItemData.GetColorName();
            }

            if (mCardAttr != null)
            {
                mCardAttr.text = EnchantmentsCardManager.GetInstance().GetEnchantmentCardAttributesDesc(cardItemData.TableID, cardItemData.mPrecEnchantmentCard.iEnchantmentCardLevel);
            }

            if (mCardLevel != null)
            {
                if (cardItemData.mPrecEnchantmentCard.iEnchantmentCardLevel > 0)
                {
                    mCardLevel.text = string.Format("+{0}", cardItemData.mPrecEnchantmentCard.iEnchantmentCardLevel);
                }
                else
                {
                    mCardLevel.text = string.Empty;
                }
            }
        }

        public void OnEmptyClick()
        {
            Reset();

            if (mOnEmptyClick != null)
            {
                mOnEmptyClick.Invoke(IsCardA);
            }
        }

        public void Reset()
        {
            mCardRoot.CustomActive(false);

            if (mComItem != null)
            {
                mComItem.Setup(null, null);
            }

            if (mCardName != null)
            {
                mCardName.text = string.Empty;
            }

            if (mCardAttr != null)
            {
                mCardAttr.text = string.Empty;
            }

            if (mCardLevel != null)
            {
                mCardLevel.text = string.Empty;
            }
        }
    }
}