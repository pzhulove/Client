using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#region USING ACTIVITY
using Protocol;
using ProtoTable;
using ActivityLimitTime;
using System;
#endregion

namespace GameClient
{
    public class BuffPrayActivity : LimitTimeCommonActivity
    {
        private readonly LimitTimeActivityCheckComponent mCheckComponent = new LimitTimeActivityCheckComponent();
        #region Model Params

        #endregion

        #region View Params

        #endregion

        #region PRIVATE METHODS

        protected sealed override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/BuffPrayActivity";
        }

        public sealed override void Show(Transform root)
        {
            base.Show(root);
            mCheckComponent.Checked(this);
        }

        public sealed override void UpdateData()
        {
            base.UpdateData();
        }
        public sealed override bool IsHaveRedPoint()
        {
            return !mCheckComponent.IsChecked();
        }
        #endregion

        #region  PUBLIC METHODS

        
        #endregion
    }
}