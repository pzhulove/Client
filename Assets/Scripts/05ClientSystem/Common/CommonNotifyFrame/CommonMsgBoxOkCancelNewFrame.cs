using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class CommonMsgBoxOkCancelNewParamData
    {
        //content
        public string ContentLabel;
        //content对其的方式， 预制体是MiddleCenter对齐，具体可由外部传入
        public TextAnchor ContentTextAnchor = TextAnchor.MiddleCenter;

        //Toggle
        public bool IsShowNotify;
        public bool IsDefaultCheck;//表示是否默认勾选
        public OnCommonMsgBoxToggleClick OnCommonMsgBoxToggleClick;

        //button
        public string LeftButtonText;
        public Action OnLeftButtonClickCallBack;
        public string RightButtonText;
        public Action OnRightButtonClickCallBack;

        public string MiddleButtonText;
        public Action OnMiddleButtonClickCallBack;
        public bool IsMiddleButton;
    }

    public class CommonMsgBoxOkCancelNewFrame : ClientFrame
    {

       
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/CommonSystemNotify/CommonMsgBoxOKCancelNewFrame";
        }

        
        protected override void _OnOpenFrame()
        {
           
            base._OnOpenFrame();

            var commonMsgBoxOkCancelParamData = userData as CommonMsgBoxOkCancelNewParamData;

            if (mCommonMsgBoxOkCancelNewView != null)
                mCommonMsgBoxOkCancelNewView.InitData(commonMsgBoxOkCancelParamData);

        }

        #region ExtraUIBind
        private CommonMsgBoxOkCancelNewView mCommonMsgBoxOkCancelNewView = null;

        protected override void _bindExUI()
        {
            mCommonMsgBoxOkCancelNewView = mBind.GetCom<CommonMsgBoxOkCancelNewView>("CommonMsgBoxOkCancelNewView");
        }

        protected override void _unbindExUI()
        {
            mCommonMsgBoxOkCancelNewView = null;
        }
        #endregion
    }
}
