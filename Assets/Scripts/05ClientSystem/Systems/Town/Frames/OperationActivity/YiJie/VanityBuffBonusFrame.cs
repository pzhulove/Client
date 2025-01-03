using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using ActivityLimitTime;
using Protocol;
using ProtoTable;
using Network;
using System;
///////É¾³ýlinq

namespace GameClient
{
    public class VanityBuffBonusModel
    {
        public Vector3 pos;
        public string iconPath;
        public string des;
    }
    public class VanityBuffBonusFrame : ClientFrame
    {
        private const string prefabPath = "UIFlatten/Prefabs/OperateActivity/YiJie/VanityBuffBonusFrame";
        VanityBuffBonusModel data = null;
        #region ExtraUIBind
        private VanityBuffBonusView mVanityBuffBonusView = null;

        protected sealed override void _bindExUI()
        {
            mVanityBuffBonusView = mBind.GetCom<VanityBuffBonusView>("VanityBuffBonusView");
        }

        protected sealed override void _unbindExUI()
        {
            mVanityBuffBonusView = null;
        }
        #endregion

        public sealed override string GetPrefabPath()
        {
            return prefabPath;
        }
        
        protected sealed override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            _InitView();
        }

        protected sealed override void _OnCloseFrame()
        {
            base._OnCloseFrame();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VanityBonusAnimationEnd);
        }

        public void _InitView()
        {
            data = userData as VanityBuffBonusModel;
            
            if (data != null)
            {
                mVanityBuffBonusView.Init(data);
                mVanityBuffBonusView.PlayAnimation();
            }
        }

        void _OnCloseClick()
        {
            Close();
        }
        
    }
}
