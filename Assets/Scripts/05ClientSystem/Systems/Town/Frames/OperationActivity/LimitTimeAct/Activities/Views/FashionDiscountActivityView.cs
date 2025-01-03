using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public sealed class FashionDiscountActivityView : FashionActivityView
    {
        protected override void UpdateTime()
        {
            mTextTime.SafeSetText(string.Format(TR.Value("activity_discount_fashion_time"),Function.GetDateTime((int) mModel.StartTime, (int)mModel.EndTime)));
        }
    }
}
