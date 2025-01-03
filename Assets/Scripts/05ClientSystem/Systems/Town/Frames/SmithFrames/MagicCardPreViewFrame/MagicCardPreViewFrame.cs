using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;

namespace GameClient
{
    public class MagicCardPreViewFrame : ClientFrame
    {
        #region ExtraUIBind
        private MagicCardPreViewView mMagicCardPreView = null;

        protected sealed override void _bindExUI()
        {
            mMagicCardPreView = mBind.GetCom<MagicCardPreViewView>("MagicCardPreView");
        }

        protected sealed override void _unbindExUI()
        {
            mMagicCardPreView = null;
        }
        #endregion

        List<ItemData> mData = null;
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/FunctionPrefab/MagicCardPreViewFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            mData = userData as List<ItemData>;
            if (mData != null)
            {
                mData.Sort((x, y) =>
                {
                    return (int)x.Quality - (int)y.Quality > 0 ? -1 : 1;
                });

                if (mMagicCardPreView != null)
                {
                    mMagicCardPreView.InitView(mData);
                }
            }
        }

        protected sealed override void _OnCloseFrame()
        {
            if (mMagicCardPreView != null)
            {
                mMagicCardPreView.UnInitView();
            }
            mData = null;
        }
    }
}

