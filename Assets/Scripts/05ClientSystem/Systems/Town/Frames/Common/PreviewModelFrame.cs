using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class PreViewDataModel
    {
        public bool isCreatItem;
        public List<PreViewItemData> preViewItemList;

        public PreViewDataModel()
        {
            isCreatItem = false;
            preViewItemList = new List<PreViewItemData>();
        }
    }

    public class PreViewItemData
    {
        public int itemId;
        public int activityId;
    }

    public class PreviewModelFrame : ClientFrame
    {
        PreViewDataModel preViewDataModel = null;
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Common/PreviewModelFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            if (userData != null)
            {
                if (preViewDataModel == null)
                {
                    preViewDataModel = userData as PreViewDataModel;
                }
            }
            
            if (mPreviewModelView != null)
            {
                mPreviewModelView.InitView(preViewDataModel);
            }
        }

        protected sealed override void _OnCloseFrame()
        {
            preViewDataModel = null;
        }

        #region ExtraUIBind
        private PreviewModelView mPreviewModelView = null;

        protected sealed override void _bindExUI()
        {
            mPreviewModelView = mBind.GetCom<PreviewModelView>("PreviewModelView");
        }

        protected sealed override void _unbindExUI()
        {
            mPreviewModelView = null;
        }
        #endregion
    }
}

