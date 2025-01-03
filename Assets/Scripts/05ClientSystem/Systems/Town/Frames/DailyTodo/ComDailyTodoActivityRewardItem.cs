using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

namespace GameClient
{
    public class ComDailyTodoActivityRewardItem : MonoBehaviour
    {        
        public bool rewardItemNeedClick = true;

        private ComItem rewardItem;
        private ItemData itemData;

        private void OnDestroy()
        {
            itemData = null;
            UnInit();
        }

        public void Init(int rewardItemId)
        {
            rewardItem = ComItemManager.Create(this.gameObject);
            itemData = ItemDataManager.CreateItemDataFromTable(rewardItemId);
            if (rewardItemNeedClick)
            {
                rewardItem.Setup(itemData, Utility.OnItemClicked);
            }
            else
            {
                rewardItem.Setup(itemData, null);
            }
        }

        public void UnInit()
        {
            if (rewardItem != null)
            {
                ComItemManager.Destroy(rewardItem);
                rewardItem = null;
            }
        }
    }
}
