using Protocol;
using ProtoTable;

namespace GameClient
{
    public sealed class ChangeFashionSpecialExchangeActivity : LimitTimeCommonActivity
    {
        public override void Init(uint activityId)
        {
            base.Init(activityId);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        protected override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/ChangeFashionSpecialExchangeActivity";
        }

        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/ChangeFashionSpecialExchangeItem";
        }
        
        protected override void _OnItemClick(int taskId, int param,ulong param2)
        {
            if (mDataModel == null || mDataModel.TaskDatas == null)
            {
                return;
            }

            if(mDataModel.State  == OpActivityState.OAS_PREPARE)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("activity_havenot_open_tips"));
                return;
            }
            if (param2 == 0)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("change_fashion_activity_havenot_fashion_tips"));
                return;
            }

            for (int i = 0; i < mDataModel.TaskDatas.Count; ++i)
            {
                if (mDataModel.TaskDatas[i].DataId == taskId)
                {
                    if (mDataModel.TaskDatas[i].State == OpActTaskState.OATS_FINISHED)
                    {
                        ItemData itemData = ItemDataManager.GetInstance().GetItem(param2);
                        if(itemData == null)
                        {
                            return;
                        }
                        var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(itemData.TableID);
                        if(itemTableData == null)
                        {
                            return;
                        }
                        if(itemTableData.SuitID > 100107)
                        {
                            string notifyCont = TR.Value("change_fashion_activity_fashion_expensive_tips");
                            SystemNotifyManager.SysNotifyMsgBoxOkCancel(notifyCont, () =>
                            {
                                ActivityDataManager.GetInstance().RequestOnTakeActTask(mDataModel.Id, (uint)taskId, param2);
                            });
                        }
                        else
                        {
                            ActivityDataManager.GetInstance().RequestOnTakeActTask(mDataModel.Id, (uint)taskId, param2);
                        }
                        //if (mDataModel.TaskDatas[i].AwardDataList[0].id == param2)
                        //{
                        //    string notifyCont = TR.Value("change_fashion_activity_fashion_expensive_tips");
                        //    SystemNotifyManager.SysNotifyMsgBoxOkCancel(notifyCont, () =>
                        //    {
                        //        ActivityDataManager.GetInstance().RequestOnTakeActTask(mDataModel.Id, (uint)taskId, param2);
                        //    });
                        //}
                        //else
                        //{
                        //    ActivityDataManager.GetInstance().RequestOnTakeActTask(mDataModel.Id, (uint)taskId, param2);
                        //}
                    }
                    else if (mDataModel.TaskDatas[i].State == OpActTaskState.OATS_UNFINISH)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("change_special_fashion_activity_cannot_exchange_tips"));
                    }
                    break;
                }
            }
        }
    }
}