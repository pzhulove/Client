using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class FatigueForBuffActivityView : LimitTimeActivityViewCommon
    {
        [SerializeField]private GameObject[] mItemRoots = new GameObject[0];
        [SerializeField]private Text mTextTime;
        //[SerializeField]private Text mTextRule;
        //[SerializeField]private Text mTextTotalProgrees;
        //[SerializeField]private Slider mImageProgresss;
       
        public override sealed void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            if (model == null)
            {
                Logger.LogError("LimitTimeActivityModel data is empty");
                return;
            }

            mOnItemClick = onItemClick;
            _InitItems(model);
            _UpdateTitleInfo(model);
        }

        void _UpdateTitleInfo(ILimitTimeActivityModel model)
        {
            mTextTime.SafeSetText(string.Format("{0}", Function.GetTimeWithoutYearNoZero((int)model.StartTime, (int)model.EndTime)));
            //mTextRule.SafeSetText(model.RuleDesc);
        }

    }
}
