using Protocol;
using UnityEngine;

namespace GameClient
{
    public sealed class ChangeFashionExchangeActivity : LimitTimeCommonActivity
    {
        public override void Init(uint activityId)
        {
            base.Init(activityId);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
        }

        public override void Dispose()
        {
            base.Dispose();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
        }

        public override void Show(Transform root)
        {
            base.Show(root);
            var tempView = mView as ChangeFashionExchangeView;
            if(tempView != null)
            {
                tempView.setGetScoreCallBack(GetScore);
            }
        }

        private void GetScore(int id)
        {
            var dataItem = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(id);
            if (dataItem != null)
            {
                ItemComeLink.OnLink(id, 0, false, null, false, dataItem.bNeedJump > 0);
            }
        }

        protected override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/ChangeFashionExchangeActivity";
        }

        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/ChangeFashionExchangeItem";
        }

        private void _OnCountValueChanged(UIEvent uiEvent)
        {
            if (mView != null)
                mView.UpdateData(mDataModel);
        }

        protected override void _OnItemClick(int taskId, int param,ulong param2)
        {
            if (mDataModel == null || mDataModel.TaskDatas == null)
            {
                return;
            }
            if (mDataModel.State == OpActivityState.OAS_PREPARE)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("activity_havenot_open_tips"));
                return;
            }
            for (int i = 0; i < mDataModel.TaskDatas.Count; ++i)
            {
                if (mDataModel.TaskDatas[i].DataId == taskId)
                {
                    if (mDataModel.TaskDatas[i].State == OpActTaskState.OATS_FINISHED)
                    {
                        base._OnItemClick(taskId, 0,0);
                    }
                    else if (mDataModel.TaskDatas[i].State == OpActTaskState.OATS_UNFINISH)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("change_fashion_activity_cannot_exchange_tips"));
                    }
                    break;
                }
            }
        }
    }
}