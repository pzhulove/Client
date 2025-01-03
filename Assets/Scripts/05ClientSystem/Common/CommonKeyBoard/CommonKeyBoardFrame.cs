using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class CommonKeyBoardDataModel
    {
        public Vector3 Position = Vector3.zero;
        public ulong CurrentValue;
        public ulong MaxValue;          //最大值
    }

    public class CommonKeyBoardFrame : ClientFrame
    {

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Common/CommonKeyBoard/CommonKeyBoardFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            if (mCommonKeyBoardView != null)
            {
                CommonKeyBoardDataModel commonKeyBoardDataModel = null;
                if (userData != null)
                    commonKeyBoardDataModel = userData as CommonKeyBoardDataModel;
                mCommonKeyBoardView.InitView(commonKeyBoardDataModel);
            }
        }

        #region ExtraUIBind
        private CommonKeyBoardView mCommonKeyBoardView = null;

        protected override void _bindExUI()
        {
            mCommonKeyBoardView = mBind.GetCom<CommonKeyBoardView>("CommonKeyBoardView");
        }

        protected override void _unbindExUI()
        {
            mCommonKeyBoardView = null;
        }
        #endregion
        

    }

}
