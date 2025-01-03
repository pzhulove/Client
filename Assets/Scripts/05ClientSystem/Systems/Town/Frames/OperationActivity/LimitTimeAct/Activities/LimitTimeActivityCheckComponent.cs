using System;
using UnityEngine;

namespace GameClient
{
    public class LimitTimeActivityCheckComponent : IDisposable
    {
        private bool mIsChecked = false;

        public void Checked(IActivity activity)
        {
            if (!mIsChecked)
            {
                mIsChecked = true;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeUpdate, activity);
            }
        }

        public bool IsChecked()
        {
            return mIsChecked;
        }

        public void Dispose()
        {
            mIsChecked = false;
        }
    }
}