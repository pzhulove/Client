using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class OPPOGrowthHaoLiRewardItem : MonoBehaviour
    {
        [SerializeField]private GameObject mItemRoot;

        private ComItem comItem = null;
        public void OnItemVisiable(AwardItemData awardItemData)
        {
            if (awardItemData == null)
            {
                return;
            }

            ItemData itemData = ItemDataManager.CreateItemDataFromTable(awardItemData.ID);
            itemData.Count = awardItemData.Num;
            if (comItem == null)
            {
                comItem = ComItemManager.Create(mItemRoot);
            }

            comItem.Setup(itemData, Utility.OnItemClicked);
        }

        private void OnDestroy()
        {
            comItem = null;
        }
    }
}

