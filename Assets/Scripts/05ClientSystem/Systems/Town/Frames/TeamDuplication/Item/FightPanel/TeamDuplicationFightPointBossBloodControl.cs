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

    //boss据点的血量控制
    public class TeamDuplicationFightPointBossBloodControl : MonoBehaviour
    {

        [Space(10)]
        [HeaderAttribute("Cover")]
        [Space(5)]
        [SerializeField] private Image bossBloodCoverImage;

        //boss血量更新
        public void UpdateBossBloodRate(int bloodRate)
        {
            if (bossBloodCoverImage == null)
                return;

            if (bloodRate < 0)
                return;

            var rate = (float)bloodRate / 100.0f;

            bossBloodCoverImage.fillAmount = rate;
        }

    }
}
