using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class InscriptionItemElement : MonoBehaviour
    {
        [SerializeField] private GameObject mItemParent;
        [SerializeField] private GameObject mCheckMark;
        [SerializeField] private Text mInscriptionName;
        [SerializeField] private Text mInscriptionAttr;

        private ItemData currentItemData;
        public ItemData CurrentItemData
        {
            get { return currentItemData; }
            set { currentItemData = value; }
        }
        private ComItemNew mComItem;
        public void OnItemVisiable(ItemData itemData)
        {
            currentItemData = itemData;

            if (mComItem == null)
            {
                mComItem = ComItemManager.CreateNew(mItemParent);
            }

            mComItem.Setup(currentItemData, Utility.OnItemClicked);

            if (mInscriptionName != null)
            {
                mInscriptionName.text = currentItemData.GetColorName();
            }

            if (mInscriptionAttr != null)
            {
                mInscriptionAttr.text = InscriptionMosaicDataManager.GetInstance().GetInscriptionAttributesDesc(currentItemData.TableID);
            }
        }

        public void OnItemChangeDisplay(bool isSelected)
        {
            if (mCheckMark != null)
            {
                mCheckMark.CustomActive(isSelected);
            }
        }

        private void OnDestroy()
        {
            currentItemData = null;
            mComItem = null;
        }
    }
}

