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

    //据点中挑战的次数
    public class TeamDuplicationFightPointNumberItem : MonoBehaviour
    {

        [Space(10)]
        [HeaderAttribute("Cover")]
        [Space(5)]
        [SerializeField] private Image fightNumberCover;


        public void Init(bool isFinished)
        {
            CommonUtility.UpdateImageVisible(fightNumberCover, !isFinished);
        }

    }
}
