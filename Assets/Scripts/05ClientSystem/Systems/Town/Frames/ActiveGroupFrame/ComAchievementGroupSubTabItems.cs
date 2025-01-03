using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoTable;

namespace GameClient
{
    public class ComAchievementGroupSubTabItems : MonoBehaviour
    {
        public ComAchievementGroupSubTabItem[] subItems;

        public void SetFlags(AchievementGroupMainItemTable mainItem)
        {
            if(null != mainItem)
            {
                int count = mainItem.ChildTabs.Count;
                if(count == 1)
                {
                    gameObject.CustomActive(false);
                }
                else
                {
                    gameObject.CustomActive(true);
                    for(int i = 0; i < subItems.Length; ++i)
                    {
                        subItems[i].CustomActive(i < mainItem.ChildTabs.Count);
                        var subItem = subItems[i];
                        subItem.OnValueChanged(false);

                        if (i < mainItem.ChildTabs.Count)
                        {
                            subItem.OnItemVisible(mainItem.ChildTabs[i]);
                        }
                    }
                }
            }
            else
            {
                gameObject.CustomActive(false);
            }
        }
    }
}