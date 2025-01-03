using System;
using System.Collections.Generic;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;

namespace GameClient
{

    public class HonorPreHistoryController : MonoBehaviour
    {


        [Space(5)]
        [HeaderAttribute("PreHistoryItem")]
        [Space(5)]
        [SerializeField] private HonorPreHistoryItem todayItem;
        [SerializeField] private HonorPreHistoryItem thisWeekItem;
        [SerializeField] private HonorPreHistoryItem lastWeekItem;
        
        public void InitHonorPreHistoryController()
        {
            //昨日统计
            if (todayItem != null)
            {
                CommonUtility.UpdateGameObjectVisible(todayItem.gameObject, true);
                todayItem.InitItem(HONOR_DATE_TYPE.HONOR_DATE_TYPE_TODAY);
            }

            //本周统计
            if (thisWeekItem != null)
            {
                CommonUtility.UpdateGameObjectVisible(thisWeekItem.gameObject, true);
                thisWeekItem.InitItem(HONOR_DATE_TYPE.HONOR_DATE_TYPE_THIS_WEEK);
            }

            //上周统计
            if (lastWeekItem != null)
            {
                CommonUtility.UpdateGameObjectVisible(lastWeekItem.gameObject, true);
                lastWeekItem.InitItem(HONOR_DATE_TYPE.HONOR_DATE_TYPE_LAST_WEEK);
            }
        }
    }
}