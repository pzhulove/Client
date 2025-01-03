using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameClient
{
    public enum EAnniverBuffPrayType
    {
        ExpAdd=1,
        HellTicketMinus=2,
        YuanGUTicketMinus=3,
        XuKongChallengeNumAdd=4,
        DropItem=5,
        

    }
    public class AnniversaryBuffPrayActivity : LimitTimeCommonActivity
    {
        private readonly LimitTimeActivityCheckComponent mCheckComponent = new LimitTimeActivityCheckComponent();

        protected sealed override string _GetPrefabPath()
        {
            if(mDataModel!=null&&!string.IsNullOrEmpty(mDataModel.ActivityPrefafPath))
            {
                return mDataModel.ActivityPrefafPath;
            }
            return "UIFlatten/Prefabs/OperateActivity/Anniversary/Acitivity/AnniversaryBuffPrayActivity";
        }
        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/Anniversary/Item/AnniversaryBuffPrayItem";
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