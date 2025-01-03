using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComSeasonReward : MonoBehaviour
    {
        public Image imgSeasonIcon;
        public Text labSeasonName;
        public GameObject objReward0;
        public GameObject objReward1;
        public GameObject objReward2;
        public GameObject objReward3;
        public GameObject objReward4;
        public GameObject objReward5;
        public GameObject objMySeasonLevel;


        [HideInInspector]
        public List<ComItem> arrComItems = new List<ComItem>();
    }
}
