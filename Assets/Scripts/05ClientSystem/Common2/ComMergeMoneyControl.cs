using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameClient
{
    class ComMergeMoneyControl : MonoBehaviour
    {
        public enum CMMC
        {
            CMMC_ENOUGH = 0,
            CMMC_NOT_ENOUGH,
        }

        string key_enough = "enough";
        string key_notenough = "not_enough";

        public StateController stateControl;
        public GameObject goItemParent;
        public Text Name;
        public Text count;
        ComItem comItem;

        void OnDestroy()
        {
            if(null != comItem)
            {
                comItem.Setup(null, null);
                ComItemManager.Destroy(comItem);
                comItem = null;
            }
            goItemParent = null;
        }

        public void SetState(CMMC eCMMC)
        {
            switch(eCMMC)
            {
                case CMMC.CMMC_ENOUGH:
                    {
                        if(null != stateControl)
                        {
                            stateControl.Key = key_enough;
                        }
                    }
                    break;
                case CMMC.CMMC_NOT_ENOUGH:
                    {
                        if(null != stateControl)
                        {
                            stateControl.Key = key_notenough;
                        }
                    }
                    break;
            }
        }

        public void SetCost(int moneyId,int moneyCount)
        {
            if(null == comItem && null != goItemParent)
            {
                comItem = ComItemManager.Create(goItemParent);
            }

            ItemData itemData = ItemDataManager.CreateItemDataFromTable(moneyId);
            if (null != itemData)
            {
                itemData.Count = 1;
            }

            if (null != comItem)
            {
                comItem.Setup(itemData, null);
            }

            if(null != Name)
            {
                if(null != itemData)
                    Name.text = itemData.Name;
                int iTotalCount = ItemDataManager.GetInstance().GetOwnedItemCount(moneyId, true);
                int iNeedCount = moneyCount;
                bool bEnough = !(iTotalCount < iNeedCount && iNeedCount >= 0);
                count.color = bEnough ? Color.white : Color.red;
                count.text = iNeedCount.ToString();
            }
        }
    }
}