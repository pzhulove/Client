using GameClient;
using Protocol;
namespace GameClient
{
    public enum EDailyActivityType
    {
        Normal=0,//普通
        Anniversary=1,//周年登录
        LanternFestivalWork = 2,//元宵开工
        TuanBenSupport = 3, //团本扶持
    }
    public sealed class DailyRewardActivity : LimitTimeCommonActivity
    {

        private OpActivityData mData;
        public override void Init(uint activityId)
        {
            mData = ActivityDataManager.GetInstance().GetLimitTimeActivityData(activityId);
            if (mData != null)
            {
                mDataModel = new LimitTimeActivityModel(mData, _GetItemPrefabPath());
            }
        }
        protected sealed override string _GetItemPrefabPath()
        {
          
            string itemPath = "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/DailyRewardItem";
          
            if (mData != null && mData.parm2 != null && mData.parm2.Length >=1)
            {
                switch (mData.parm2[0])
                {
                    case (uint)EDailyActivityType.Anniversary:
                        itemPath = "UIFlatten/Prefabs/OperateActivity/Anniversary/Item/AnniversaryLoginItem";
                        break;
                    case (uint)EDailyActivityType.LanternFestivalWork:
                        itemPath = "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/LanternFestivalWorkItem";
                        break;
                }
            }
            return itemPath;
        }
        protected sealed override string _GetPrefabPath()
        {
           
            string prefabPath = "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/DailyRewardActivityNew";
            if (mData != null && !string.IsNullOrEmpty(mData.prefabPath))
            {
                prefabPath = mData.prefabPath;
                return prefabPath;
            }
            if (mData != null && mData.parm2 != null && mData.parm2.Length >=1)
            {
                switch (mData.parm2[0])
                {
                    case (uint)EDailyActivityType.Anniversary:
                        prefabPath = "UIFlatten/Prefabs/OperateActivity/Anniversary/Acitivity/AnniversaryLoginActivity";
                        break;
                    case (uint)EDailyActivityType.LanternFestivalWork:
                        prefabPath = "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/LanternFestivalWorkActivity";
                        break;
                    case (uint)EDailyActivityType.TuanBenSupport:
                        prefabPath = "UIFlatten/Prefabs/OperateActivity/Anniversary/Acitivity/TuanBenSupportActivity";
                        break;
                }
            }

            return prefabPath;
        }
    }
}