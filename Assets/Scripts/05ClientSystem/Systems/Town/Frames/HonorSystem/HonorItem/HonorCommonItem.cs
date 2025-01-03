using System;
using System.Collections.Generic;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class HonorCommonItem : MonoBehaviour
    {
        [Space(5)]
        [HeaderAttribute("Title")]
        [SerializeField] private Text activityNameText = null;
        [SerializeField] private Text activityNumberText = null;

        public void InitItem(PvpNumberStatistics pvpNumberStatistics)
        {
            if (pvpNumberStatistics == null)
                return;

            if (activityNameText != null)
                activityNameText.text = pvpNumberStatistics.PvpName;

            if (activityNumberText != null)
                activityNumberText.text = pvpNumberStatistics.PvpCount.ToString();
        }

    }
}