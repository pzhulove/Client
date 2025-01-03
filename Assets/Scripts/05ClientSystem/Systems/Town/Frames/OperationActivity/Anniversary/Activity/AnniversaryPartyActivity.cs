using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameClient
{
    public class AnniversaryPartyActivity :LimitTimeCommonActivity
    {

        private readonly LimitTimeActivityCheckComponent mCheckComponent = new LimitTimeActivityCheckComponent();
        protected sealed override string _GetPrefabPath()
        {
            if (mDataModel != null && !string.IsNullOrEmpty(mDataModel.ActivityPrefafPath))
            {
                return mDataModel.ActivityPrefafPath;
            }
            return "UIFlatten/Prefabs/OperateActivity/Anniversary/Acitivity/AnniversaryPartyActivity";
        }
        protected override string _GetItemPrefabPath()
        {
            return string.Empty;
        }
        public sealed override void Show(Transform root)
        {
            base.Show(root);
            mCheckComponent.Checked(this);
        }

        public sealed override bool IsHaveRedPoint()
        {
            return !mCheckComponent.IsChecked();
        }

    }
}
