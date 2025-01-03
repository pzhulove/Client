using UnityEngine;
using System.Collections;

namespace GameClient
{
    class ComClickShowTips : MonoBehaviour
    {
        public int iTableID = 0;
        ItemData itemData = null;
        public void OnClickItemTips()
        {
            if(null == itemData)
            {
                itemData = ItemDataManager.CreateItemDataFromTable(iTableID);
            }

            ItemTipManager.GetInstance().ShowTip(itemData, null);
        }

        void OnDestroy()
        {
            itemData = null;
            iTableID = 0;
        }
    }
}