using Protocol;
using ProtoTable;
using UnityEngine;

namespace GameClient
{
    public sealed class DungeonDropActivity : LimitTimeCommonActivity
    {
        private OpActivityData mData;
        protected override void _OnItemClick(int taskId, int param,ulong param2)
        {
            int id = -1;

            for (int i = 0; i < mDataModel.TaskDatas.Count; ++i)
            {
                if (mDataModel.TaskDatas[i].DataId == taskId)
                {
                    id = (int)mDataModel.TaskDatas[i].ParamNums[0];
                }
            }

            if (id == -1)
            {
                return;
            }

            var mAcquiredMethodTable = TableManager.GetInstance().GetTableItem<AcquiredMethodTable>(id);
            if (mAcquiredMethodTable == null)
            {
                return;
            }
            string mLinkInfo = mAcquiredMethodTable.LinkInfo;
            ActiveManager.GetInstance().OnClickLinkInfo(mLinkInfo);

            if (ClientSystemManager.GetInstance().IsFrameOpen<LimitTimeActivityFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<LimitTimeActivityFrame>();
            }
        }
        public override void Init(uint activityId)
        {
            base.Init(activityId);
            mData = ActivityDataManager.GetInstance().GetLimitTimeActivityData(activityId);
        }
        protected override string _GetPrefabPath()
        {
            if(mData!=null)
            {
                string path = mData.prefabPath;
                if (!string.IsNullOrEmpty(path))
                {
                    return path;
                }
            }
           
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/CommonHaveExtraItemActivity";
        }

        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/DungeonDropItem";
        }
    }
}