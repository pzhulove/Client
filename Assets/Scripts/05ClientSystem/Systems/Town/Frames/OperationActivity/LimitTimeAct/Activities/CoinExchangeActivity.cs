using Protocol;
using UnityEngine;

namespace GameClient
{
    public sealed class CoinExchangeActivity : LimitTimeCommonActivity
    {
        private readonly LimitTimeActivityCheckComponent mCheckComponet = new LimitTimeActivityCheckComponent();
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
            mCheckComponet.Checked(this);
        }
        public override bool IsHaveRedPoint()
        {
            bool result = false;
            if (mDataModel.TaskDatas == null || mDataModel.State != OpActivityState.OAS_IN)
                result= false;

            for (int i = 0; i < mDataModel.TaskDatas.Count; ++i)
            {
                if (mDataModel.TaskDatas[i].State == OpActTaskState.OATS_FINISHED)
                {
                    result= true;
                }
            }

           if(result)
            {
                return !mCheckComponet.IsChecked();
            }
            else
            {
                return false;
            }
          
        }
        protected override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/CoinExchangeActivity";
        }

        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/CoinExchangeItem";
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
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("activity_cannot_exchange_tips2"));
                    }
                    break;
                }
            }
        }
    }
}