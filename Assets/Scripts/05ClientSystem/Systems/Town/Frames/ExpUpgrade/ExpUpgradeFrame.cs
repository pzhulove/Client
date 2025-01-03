using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ExpUpgradeData
    {
        public float CurExpValue = 0.0f;
        public float MaxExpValue = 0.0f;
        public float AddExpValue = 0.0f;
    }

    public class ExpUpgradeFrame : ClientFrame
    {

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/ExpUpgrade/ExpUpgrade";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();
            BindEvents();

            if (mExpUpgrade != null)
            {
                var expUpgradeData = userData as ExpUpgradeData;
                mExpUpgrade.InitExpUpgradeData(expUpgradeData);
            }
        }

        protected override void _OnCloseFrame()
        {
            base._OnCloseFrame();
            UnBindEvents();
        }

        private void BindEvents()
        {
        }

        private void UnBindEvents()
        {

        }

        #region ExtraUIBind
        private ExpUpgradeView mExpUpgrade = null;

        protected override void _bindExUI()
        {
            mExpUpgrade = mBind.GetCom<ExpUpgradeView>("ExpUpgrade");
            mExpUpgrade.OnButtonCloseCallBack = CloseFrame;
            mExpUpgrade.OnTimeIntervalCloseCallBack = CloseFrame;
        }

        protected override void _unbindExUI()
        {
            mExpUpgrade.OnButtonCloseCallBack = null;
            mExpUpgrade.OnTimeIntervalCloseCallBack = null;
            mExpUpgrade = null;
        }
        #endregion
        
        private void CloseFrame()
        {
            Close();
        }
    }
}