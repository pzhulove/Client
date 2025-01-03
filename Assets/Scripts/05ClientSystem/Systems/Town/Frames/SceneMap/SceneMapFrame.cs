using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SceneMapFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SceneMap/SceneMapFrame";
        }

        protected override void _OnOpenFrame()
        {

            base._OnOpenFrame();

            if (mSceneMapView != null)
            {
                mSceneMapView.Init();
            }
        }

        #region ExtraUIBind
        private SceneMapView mSceneMapView = null;

        protected override void _bindExUI()
        {
            mSceneMapView = mBind.GetCom<SceneMapView>("SceneMapView");
        }

        protected override void _unbindExUI()
        {
            mSceneMapView = null;
        }
        #endregion
        

    }
}
