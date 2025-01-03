using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameClient
{
    public class SpringFestivalDungeonActivity : LimitTimeCommonActivity
    {
        private readonly LimitTimeActivityCheckComponent mCheckComponent = new LimitTimeActivityCheckComponent();
        public sealed override void Show(Transform root)
        {
            base.Show(root);
            mCheckComponent.Checked(this);
        }

        public sealed override bool IsHaveRedPoint()
        {
            return !mCheckComponent.IsChecked();
        }
        protected override string _GetPrefabPath()
        {
            string prefabPath = "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/SpringFestivalDungeonActivity";
            if (mDataModel != null && !string.IsNullOrEmpty(mDataModel.ActivityPrefafPath))
            {
                prefabPath = mDataModel.ActivityPrefafPath;
                return prefabPath;
            }
            return prefabPath;
        }

      
    }
}
