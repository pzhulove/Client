using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class WeekSignInAwardRecordFrame : ClientFrame
    {

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SignIn/WeekSignInAwardRecordFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            if (mWeekSignInAwardRecordView != null)
            {
                var weekSingInType = (int)userData;
                mWeekSignInAwardRecordView.InitView(weekSingInType);
            }
        }

        #region ExtraUIBind
        private WeekSignInAwardRecordView mWeekSignInAwardRecordView = null;

        protected override void _bindExUI()
        {
            mWeekSignInAwardRecordView = mBind.GetCom<WeekSignInAwardRecordView>("WeekSignInAwardRecordView");
        }

        protected override void _unbindExUI()
        {
            mWeekSignInAwardRecordView = null;
        }
        #endregion
        

    }

}
