using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComMyPkRecord : MonoBehaviour
    {
        public Image imgLeftSeasonIcon;
        public Text labLeftName;
        public Text labLeftJob;
        public Text labLeftSeasonName;

        public Image imgRightSeasonIcon;
        public Text labRightName;
        public Text labRightJob;
        public Text labRightSeasonName;

        public Image imgPkResult;

        public Text labCreateTime;
        public Button btnStore;
        public Button btnShare;
        public Button btnPlay;
        public Button btnUpLoad;
        public Text[] playerNames;

        public Image resultBg = null;
    }
}
