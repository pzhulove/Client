using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class ComAchievementAwardPlayFrameConfig : MonoBehaviour
    {
        public GameObject[] goParents = new GameObject[0];
        List<ComItem> comItems = new List<ComItem>(4);
        public ComEffectPrcess[] comEffectProcess = new ComEffectPrcess[0];

        public void SetAwards(int iId)
        {
            var levelItem = TableManager.GetInstance().GetTableItem<ProtoTable.AchievementLevelInfoTable>(iId);
            if(null == levelItem)
            {
                return;
            }

            if(levelItem.AwardID.Count != levelItem.AwardCount.Count)
            {
                return;
            }

            int awardCount = levelItem.AwardID.Count;
            if (comItems.Count == 0)
            {
                for (int i = 0; i < goParents.Length; ++i)
                {
                    comItems.Add(ComItemManager.Create(goParents[i]));
                }
            }

            for(int i = 0; i < goParents.Length; ++i)
            {
                goParents[i].CustomActive(false);
                if(null != comItems && i < comItems.Count && i < awardCount)
                {
                    var itemData = ItemDataManager.CreateItemDataFromTable(levelItem.AwardID[i]);
                    if(null != itemData)
                    {
                        itemData.Count = levelItem.AwardCount[i];
                    }
                    if(null != comItems[i])
                    comItems[i].Setup(itemData, null);
                    if(i < comEffectProcess.Length)
                    {
                        comEffectProcess[i].Play();
                    }
                }
            }
        }

        public void DestroyComItems()
        {
            for (int i = 0; i < comItems.Count; ++i)
            {
                if (null != comItems[i])
                    ComItemManager.Destroy(comItems[i]);
            }
            comItems.Clear();
        }

        void OnDestroy()
        {
            DestroyComItems();
        }
    }
}