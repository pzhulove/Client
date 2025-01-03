using System;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class DailyRewardLevelItem : MonoBehaviour
    {
        [SerializeField] private Text mTextLv;
        [SerializeField] private ComUIListScript mLvItemList;

        public void OnInit(DailyLevelRewardData dailyLevelRewardData)
        {
            mTextLv.SafeSetText(TR.Value("activity_daily_reward_level_desp", dailyLevelRewardData.minLv ,dailyLevelRewardData.maxLv));
            if (null != mLvItemList)
            {
                mLvItemList.Initialize();
                mLvItemList.onItemVisiable = (item) =>{
                    if (null == item)
                        return;
                    var script = item.GetComponent<ComItemNew>();
                    ItemData data = ItemDataManager.CreateItemDataFromTable(dailyLevelRewardData.itemIdList[item.m_index]);
                    data.Count = dailyLevelRewardData.itemNumList[item.m_index];
                    script.Setup(data, null, true);
                };
            }
        }
    }
}
