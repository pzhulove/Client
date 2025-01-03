using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameClient
{
    public class SpringChallengeActivity : LimitTimeCommonActivity
    {
        private readonly LimitTimeActivityCheckComponent mCheckComponent = new LimitTimeActivityCheckComponent();
        public override bool IsHaveRedPoint()
        {
            return !mCheckComponent.IsChecked();
        }
        public override void Show(Transform root)
        {
            base.Show(root);
            mCheckComponent.Checked(this);
        }
        protected override string _GetPrefabPath()
        {
            string prefabPath = "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/SpringChallengeActivity";
            if (mDataModel != null && !string.IsNullOrEmpty(mDataModel.ActivityPrefafPath))
            {
                prefabPath = mDataModel.ActivityPrefafPath;
                return prefabPath;
            }
            return prefabPath;
        }

        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/SpringChallengeItem";
        }
    }
}
