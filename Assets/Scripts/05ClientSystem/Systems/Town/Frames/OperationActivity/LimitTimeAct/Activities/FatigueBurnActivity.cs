using Protocol;
using ProtoTable;
using UnityEngine;

namespace GameClient
{
    public sealed class FatigueBurnActivity : LimitTimeCommonActivity
    {
        protected override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/FatigueBurnActivity";
        }

        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/FatigueBurnItem";
        }

        protected override void _OnItemClick(int taskId, int param,ulong param2)
        {
            //0是开启按钮，其他是处理冻结和解冻
            if (param == 0)
            {
                if (mDataModel != null && mDataModel.TaskDatas != null)
                {
                    for (int i = 0; i < mDataModel.TaskDatas.Count; ++i)
                    {
                        var task = mDataModel.TaskDatas[i];
                        if (task.DataId == taskId)
                        {
                            //判断状态，没有在燃烧则判断钱够不够，够的话开启燃烧
                            if (task.State != OpActTaskState.OATS_FINISHED)
                            {
                                var paramNums = task.ParamNums;
                                if (paramNums != null && paramNums.Count > 2)
                                {
                                    int itemId = (int)paramNums[0];
                                    var tableData = TableManager.GetInstance().GetTableItem<ItemTable>(itemId);
                                    int ordinaryItemCount = (int)paramNums[1];
                                    string ordinaryItemName = tableData.Name;
                                    string notifyCont = string.Format(TR.Value("activity_fatigue_burning_buy_text"), ordinaryItemCount, ordinaryItemName);
                                    SystemNotifyManager.SysNotifyMsgBoxOkCancel(notifyCont, () =>
                                    {
                                        CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();
                                        costInfo.nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(tableData.SubType);
                                        ItemTipManager.GetInstance().CloseAll();
                                        costInfo.nCount = ordinaryItemCount;
                                        CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
                                        {
                                            base._OnItemClick(taskId, param,0);
                                        });

                                    });
                                }
                            }
                            else
                            {
                                SystemNotifyManager.SystemNotify(9054);
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                base._OnItemClick(taskId, param,0);
            }

        }

    }
}