using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class ComMoneyRewardsInfoList : MonoBehaviour
    {
        public ComMoneyRewardsResultInfo[] results = new ComMoneyRewardsResultInfo[0];
        public ComMoneyRewardsResultInfo[] result4s = new ComMoneyRewardsResultInfo[0];
        public ComMoneyRewardsResultInfo[] result2s = new ComMoneyRewardsResultInfo[0];
        public Button[] buttons = new Button[0];
        public StateController[] btnWatchStatus = new StateController[0];
        public void OnClickWatchRecords()
        {
            //this is the last record who win 3 times

        }
    }
}