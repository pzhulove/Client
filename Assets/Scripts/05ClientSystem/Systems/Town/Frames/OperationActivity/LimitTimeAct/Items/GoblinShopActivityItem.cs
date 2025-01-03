using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class GoblinShopActivityItem : MonoBehaviour
    {
        [SerializeField]
        private GameObject mItemRoot;
        [SerializeField]
        private Text mNameTxt;

        public void Init(int id,Vector2 size)
        {
            ItemData itemData = ItemDataManager.CreateItemDataFromTable(id);
            ComItem comItem = ComItemManager.Create(mItemRoot);
            if (itemData != null && comItem != null)
            {
                comItem.GetComponent<RectTransform>().sizeDelta = size;
                comItem.Setup(itemData, Utility.OnItemClicked);
                mNameTxt.SafeSetText(itemData.Name);
            }
        }

    }
}
