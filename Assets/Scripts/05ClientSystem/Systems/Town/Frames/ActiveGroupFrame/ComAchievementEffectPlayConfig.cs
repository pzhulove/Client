using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class ComAchievementEffectPlayConfig : MonoBehaviour
    {
        public List<ComAchievementEffectPlayItem> effectItems = new List<ComAchievementEffectPlayItem>(4);
        List<ComAchievementEffectPlayItem> mUsedItems = new List<ComAchievementEffectPlayItem>(4);
        public void Play()
        {
            int popId = MissionManager.GetInstance().PopAchievementItem();
            if (0 != popId)
            {
                ProtoTable.AchievementGroupSubItemTable subItem = TableManager.GetInstance().GetTableItem<ProtoTable.AchievementGroupSubItemTable>(popId);
                if(null == subItem)
                {
                    return;
                }

                ComAchievementEffectPlayItem usedItem = null;
                if (effectItems.Count > 0)
                {
                    usedItem = effectItems[0];
                    effectItems.RemoveAt(0);
                    mUsedItems.Add(usedItem);
                    usedItem.OnCreate();
                    usedItem.SetValue(popId);
                }
                else
                {
                    if (mUsedItems.Count > 0)
                    {
                        usedItem = mUsedItems[0];
                        mUsedItems.RemoveAt(0);
                        mUsedItems.Add(usedItem);
                        usedItem.OnCreate();
                        usedItem.SetValue(popId);
                    }
                }
            }
            else
            {
                if (mUsedItems.Count > 0)
                {
                    mUsedItems[0].OnRecycle();
                    effectItems.Add(mUsedItems[0]);
                    mUsedItems.RemoveAt(0);
                }
            }
        }
    }
}