using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class GrowthExpendItem : MonoBehaviour
    {
        [SerializeField]private GameObject mItemParent;
        [SerializeField]private Text mName;
        [SerializeField]private Text mItemDesc;
        [SerializeField] private GameObject mCheckRoot;

        private ComItemNew mComItem;
        private ItemData itemData;
        public ItemData ItemData
        {
            get { return itemData; }
        }

        public void OnItemVisiable(ItemData data)
        {
            if (data == null)
            {
                return;
            }

            itemData = data;

            if (mComItem == null)
            {
                mComItem = ComItemManager.CreateNew(mItemParent);
            }

            mComItem.Setup(itemData, Utility.OnItemClicked);

            mName.text = itemData.GetColorName();

            if (itemData.Description.Length >= 30)
            {
                string desc = itemData.Description.Substring(0, 30);
                desc += "......";
                mItemDesc.text = desc;
            }
            else
            {
                mItemDesc.text = itemData.Description;
            }
        }

        public void OnChangedDisplay(bool value)
        {
            if(mCheckRoot != null)
            {
                mCheckRoot.CustomActive(value);
            }
        }

        private void OnDestroy()
        {
            mComItem = null;
            itemData = null;
        }
    }
}