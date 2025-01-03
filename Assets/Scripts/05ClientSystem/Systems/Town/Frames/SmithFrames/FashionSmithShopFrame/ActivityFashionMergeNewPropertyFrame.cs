using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class ActivityFashionMergeNewPropertyFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/FashionSmithShop/ActivityFashionMergeNewPropertyFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            if (mFashionMergeNewPropertyView != null)
            {
                mFashionMergeNewPropertyView.InitData();
            }
        }

        #region ExtraUIBind
        private FashionMergeNewPropertyView mFashionMergeNewPropertyView = null;

        protected override void _bindExUI()
        {
            mFashionMergeNewPropertyView = mBind.GetCom<FashionMergeNewPropertyView>("FashionMergeNewPropertyView");
        }

        protected override void _unbindExUI()
        {
            mFashionMergeNewPropertyView = null;
        }
        #endregion

    }
}