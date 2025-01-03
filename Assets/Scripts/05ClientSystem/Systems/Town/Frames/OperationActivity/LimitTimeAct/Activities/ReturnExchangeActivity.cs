using Protocol;

namespace GameClient
{
    public sealed class ReturnExchangeActivity : LimitTimeCommonActivity
    {
        public override void Init(uint activityId)
        {
            base.Init(activityId);
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.AccountSpecialItemUpdate, _UpdateItemCount);
        }

        public override void Dispose()
        {
            base.Dispose();
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.AccountSpecialItemUpdate, _UpdateItemCount);
        }

        protected override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/ReturnExchangeActivity";
        }

        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/ReturnExchangeItem";
        }

        private void _OnCountValueChanged(UIEvent uiEvent)
        {
            if (mView != null)
                mView.UpdateData(mDataModel);
        }

        private void _UpdateItemCount(UIEvent uiEvent)
        {
            if(mView != null)
            {
                mView.UpdateData(mDataModel);
            }
        }

        protected override void _OnItemClick(int taskId, int param,ulong param2)
        {
            if (mDataModel == null || mDataModel.TaskDatas == null)
            {
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
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("activity_cannot_exchange_tips"));
                    }
                    break;
                }
            }
        }
    }
}