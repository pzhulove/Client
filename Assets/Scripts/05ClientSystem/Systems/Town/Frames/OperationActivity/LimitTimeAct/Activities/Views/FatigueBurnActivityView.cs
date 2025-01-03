using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class FatigueBurnActivityView : LimitTimeActivityViewCommon
    {
        public ActivityItemBase.OnActivityItemClick<int> OnOpenClick { set; private get; }

        public override void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            base.Init(model, onItemClick);
        }
    }
}
