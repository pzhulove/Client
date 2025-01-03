using UnityEngine;

namespace GameClient
{
    public sealed class FatigueForBuffActivity : LimitTimeCommonActivity
    {
        private readonly LimitTimeActivityCheckComponent mCheckComponent = new LimitTimeActivityCheckComponent();

        public override void Init(uint activityId)
        {
            base.Init(activityId);
            if (mDataModel != null)
            {
                mDataModel.SortTaskByState();
            }

        }

        public override void Show(Transform root)
        {
            base.Show(root);

            mCheckComponent.Checked(this);
        }

        public override bool IsHaveRedPoint()
        {
            return !mCheckComponent.IsChecked();
        }

        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/FatigueForBuffItem";
        }

        protected sealed override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/FatigueForBuffActivity.prefab";
        }

    }
}