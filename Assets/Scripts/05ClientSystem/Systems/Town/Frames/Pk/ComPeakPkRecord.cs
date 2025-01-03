using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComPeakPkRecord : MonoBehaviour
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
        public Text labPlayTime;
        public Text labScore;
        public Button btnStore;
        public Button btnShare;
        public Button btnPlay;
        public Button btnUpload;

        public Image resultBg = null;
    }
}
