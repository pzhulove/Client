using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class MaterialSynthesisItem : MonoBehaviour
    {
        [SerializeField]private GameObject mItemParent;
        [SerializeField]private GameObject mCheckMarkRoot;
        [SerializeField]private Text mName;
        [SerializeField]private Text mCheckName;

        private ComItemNew mComItem;
        public MaterialsSynthesisData mMaterialsSynthesisData { get; set; }
        public void OnItemVisiable(MaterialsSynthesisData data)
        {
            if (data == null)
            {
                return;
            }

            mMaterialsSynthesisData = data;

            ItemData itemData = ItemDataManager.CreateItemDataFromTable(data.tableID);
            if (itemData == null)
            {
                return;
            }

            if (mComItem == null)
            {
                mComItem = ComItemManager.CreateNew(mItemParent);
            }

            mComItem.Setup(itemData, Utility.OnItemClicked);

            mName.text = mCheckName.text = itemData.GetColorName();
        }

        public void OnOnItemChangeDisplayDelegate(bool bSelected)
        {
            mCheckMarkRoot.CustomActive(bSelected);
        }

        private void OnDestroy()
        {
            mComItem = null;
            mMaterialsSynthesisData = null;
        }
    }
}