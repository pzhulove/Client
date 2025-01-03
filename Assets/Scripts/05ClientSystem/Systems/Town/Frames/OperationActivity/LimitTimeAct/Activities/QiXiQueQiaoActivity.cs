using Protocol;
using ProtoTable;

namespace GameClient
{
    public enum ActivityType
    {
        AT_QIXIQUEQIAO = 1,//七夕鹊桥活动
        AT_CHRISTMASSNNOWMAN = 2,//圣诞雪人活动
        AT_LANTERNFESTIVAL = 3, //挂灯笼大挑战活动
        AT_APRILFOOLSDAY = 4,   //愚人节活动
        AT_DRAGONBOATFESTIVAL = 5, //端午节活动
        AT_QIXINEW=6,//新的七夕鹊桥活动
        AT_ZHONGQIU= 7//中秋节-活跃活动
    }
    public sealed class QiXiQueQiaoActivity : LimitTimeCommonActivity
    {
        OpActivityData data;
        public override void Init(uint activityId)
        {
            data = ActivityDataManager.GetInstance().GetLimitTimeActivityData(activityId);
            var opActivityTable = TableManager.GetInstance().GetTableItem<OpActivityTable>((int)activityId);
            if (data != null && opActivityTable != null)
            {
                mDataModel = new LimitTimeActivityModel(data, _GetItemPrefabPath(), opActivityTable.BgPath);
            }
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
        }

        public override void Dispose()
        {
            base.Dispose();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
            data = null;
        }

        protected override string _GetItemPrefabPath()
        {
            string sItemPrefabPath = "";
            switch ((int)data.parm)
            {
                case (int)ActivityType.AT_QIXINEW:
                case (int)ActivityType.AT_ZHONGQIU:
                    sItemPrefabPath = "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/QiXiQueQiaoItem";
                    break;
                case (int)ActivityType.AT_CHRISTMASSNNOWMAN:
                    sItemPrefabPath = "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/ChristmasSnowmanItem";
                    break;
                case (int)ActivityType.AT_LANTERNFESTIVAL:
                case (int)ActivityType.AT_APRILFOOLSDAY:
                case (int)ActivityType.AT_DRAGONBOATFESTIVAL:
                    sItemPrefabPath = "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/LanternFestivalItem";
                    break;
                default:
                    break;
            }
            return sItemPrefabPath;
        }

        protected override string _GetPrefabPath()
        {
            string sPrefabPath = "";
            switch ((int)mDataModel.Param)
            {
                case (int)ActivityType.AT_QIXINEW:
                    sPrefabPath = "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/QiXiQueQiaoActivity";
                    break;
                case (int)ActivityType.AT_ZHONGQIU:
                    sPrefabPath = "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/QiXiQueQiaoActivity2";
                    break;
                case (int)ActivityType.AT_CHRISTMASSNNOWMAN:
                    sPrefabPath = "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/ChristmasSnowmanActivity";
                    break;
                case (int)ActivityType.AT_LANTERNFESTIVAL:
                    sPrefabPath = "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/LanternFestivalActivity";
                    break;
                case (int)ActivityType.AT_APRILFOOLSDAY:
                case (int)ActivityType.AT_DRAGONBOATFESTIVAL:
                    sPrefabPath = "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/AprilFoolsDayActivity";
                    break;
                default:
                    break;
            }
            return sPrefabPath;
        }

        void _OnCountValueChanged(UIEvent uiEvent)
        {
            if (mView != null)
            {
                mView.UpdateData(mDataModel);
            }
        }
    }
}