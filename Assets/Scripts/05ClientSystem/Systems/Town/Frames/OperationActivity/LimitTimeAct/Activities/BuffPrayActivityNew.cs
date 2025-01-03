using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameClient
{
  
    public class BuffPrayActivityNew : LimitTimeCommonActivity
    {
        private readonly LimitTimeActivityCheckComponent mCheckComponent = new LimitTimeActivityCheckComponent();
        protected sealed override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/BuffPrayActivityNew";
        }

        public sealed override void Show(Transform root)
        {
            base.Show(root);
            mCheckComponent.Checked(this);
        }

        public sealed override void UpdateData()
        {
            base.UpdateData();
        }
        public sealed override bool IsHaveRedPoint()
        {
            return !mCheckComponent.IsChecked();
        }

    }
}
