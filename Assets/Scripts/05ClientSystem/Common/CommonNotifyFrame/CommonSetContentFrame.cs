using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GameClient
{

    public class CommonSetContentDataModel
    {
        public string TitleStr;
        public string DefaultEmptyStr;
        public string DefaultContentStr;
        public int MaxWordNumber;

        public UnityAction<string> OnOkClicked;
    }
    
    public class CommonSetContentFrame : ClientFrame
    {

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Common/CommonFrame/CommonSetContentFrame";
        }


        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();

            var commonSetContentDataModel = userData as CommonSetContentDataModel;
            if (mCommonSetContentView != null)
                mCommonSetContentView.Init(commonSetContentDataModel);
        }

        #region ExtraUIBind
        private CommonSetContentView mCommonSetContentView = null;

        protected override void _bindExUI()
        {
            mCommonSetContentView = mBind.GetCom<CommonSetContentView>("CommonSetContentView");
        }

        protected override void _unbindExUI()
        {
            mCommonSetContentView = null;
        }
        #endregion

    }
}
