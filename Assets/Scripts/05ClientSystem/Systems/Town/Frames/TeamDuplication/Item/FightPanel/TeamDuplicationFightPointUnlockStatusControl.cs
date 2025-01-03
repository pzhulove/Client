using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;
using Scripts.UI;

namespace GameClient
{

    //据点解锁状态控制器
    public class TeamDuplicationFightPointUnlockStatusControl : MonoBehaviour
    {

        [Space(10)]
        [HeaderAttribute("Cover")]
        [Space(5)]
        [SerializeField] private Text unlockRateLabel;

        [SerializeField] private Image unlockSliderImage;

        //据点解锁比例进行更新
        public void UpdateUnlockRate(int unlockRate)
        {
            if (unlockRate < 0)
                return;

            //描述
            if (unlockRateLabel != null)
            {
                var unlockRateStr = string.Format(TR.Value("team_duplication_fight_point_unlock_rate_format"),
                    unlockRate);

                unlockRateLabel.text = unlockRateStr;
            }

            ////图片
            //if (unlockSliderImage != null)
            //{
            //    var leftRate = 100 - unlockRate;
            //    if (leftRate < 0)
            //        leftRate = 0;
            //    else if (leftRate > 100)
            //    {
            //        leftRate = 100;
            //    }

            //    unlockSliderImage.fillAmount = (float) leftRate / 100.0f;
            //}

        }
    }
}
