using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;

namespace GameClient
{
    public class RandomPropertiesFrame : ClientFrame
    {
        List<int> mDatas = null;
        #region ExtraUIBind
        private RandomPropertiesView mRandomPropertiesView = null;

        protected sealed override void _bindExUI()
        {
            mRandomPropertiesView = mBind.GetCom<RandomPropertiesView>("RandomPropertiesView");
        }

        protected sealed override void _unbindExUI()
        {
            mRandomPropertiesView = null;
        }
        #endregion
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/RandomPropertiesFrame/RandomPropertiesFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            base._OnOpenFrame();
            mDatas = userData as List<int>;
            if (mDatas != null)
            {
                mRandomPropertiesView.Initialize(this,mDatas);
            }
        }
        
        protected sealed override void _OnCloseFrame()
        {
            base._OnCloseFrame();
            mRandomPropertiesView.UnInitialize();
        }
    }
}

