using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComMoneyRewardsEnterSetting : MonoBehaviour
    {
        public string fmtString;
        public Text hint;
        public Text champAwards;

        public void UpdateHint()
        {
            if(null != hint)
            {
                var itemID = MoneyRewardsDataManager.GetInstance().EnrollItemID;
                var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(itemID);
                string itemName = string.Empty;
                if(null != itemData)
                {
                    itemName = itemData.GetColorName();
                }
                hint.text = string.Format(fmtString, itemName, MoneyRewardsDataManager.GetInstance().EnrollCount);
            }
        }

        public void UpdateChampAwards()
        {
            if(null != champAwards)
            {
                champAwards.text = MoneyRewardsDataManager.GetInstance().ChampAward.ToString();
            }
        }
    }
}