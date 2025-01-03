using GameClient;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class AccumulateClearanceView : LimitTimeActivityViewCommon
    {
        private int mTotalNum = 2;
        [SerializeField]
        private Text mTodayCount = null;
        public override void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            base.Init(model, onItemClick);
            int curNum = CountDataManager.GetInstance().GetCount(CounterKeys.ACT_DUNGEON);
            if(curNum>mTotalNum)
            {
                curNum = mTotalNum;
            }
            mTodayCount.SafeSetText(string.Format("{0}/{1}", curNum, mTotalNum));
        }
    }
}
