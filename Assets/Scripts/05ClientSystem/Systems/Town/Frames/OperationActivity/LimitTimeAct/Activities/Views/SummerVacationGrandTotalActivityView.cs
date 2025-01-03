using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameClient
{
    public class SummerVacationGrandTotalActivityView : LimitTimeActivityViewCommon
    {

        public override void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            base.Init(model, onItemClick);
         
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
