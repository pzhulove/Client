using Protocol;
using UnityEngine;

namespace GameClient
{
    public sealed class ArborDayActivity : LimitTimeCommonActivity
    {
        //树木种植状态的字符串
        private string _treePlantStateStr;

        public override void Init(uint activityId)
        {
            base.Init(activityId);

            InitArborDayActivityInfo();

            BindUiEvents();

        }

        private void InitArborDayActivityInfo()
        {
            if (mDataModel == null
                || mDataModel.StrParam == null
                || mDataModel.StrParam.Length != 4)
                return;

            _treePlantStateStr = mDataModel.StrParam[2];
        }

        public override void Dispose()
        {
            base.Dispose();

            UnBindUiEvents();
        }

        protected override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/ArborDayActivity";
        }

        //植树节活动是否可以种植树木
        public override bool IsHaveRedPoint()
        {
            if (mDataModel == null)
                return false;

            //活动状态未开启，不显示红点
            if (mDataModel.State != OpActivityState.OAS_IN)
                return false;

            //判断活动任务是否可以领取
            if (mDataModel.TaskDatas != null)
            {
                for (var i = 0; i < mDataModel.TaskDatas.Count; i++)
                {
                    var curTaskData = mDataModel.TaskDatas[i];
                    if (curTaskData == null)
                        continue;

                    //任务完成（可以领取）,显示红点
                    if (curTaskData.State == OpActTaskState.OATS_FINISHED)
                        return true;
                }
            }

            //字符串存在，不为null
            if (string.IsNullOrEmpty(_treePlantStateStr) == false)
            {
                var treePlantState = (PlantOpActSate)ArborDayUtility.GetCounterValueByCounterStr(_treePlantStateStr);
                //可以种植或者可以鉴定,显示红点,其他状态不现实红点
                if (treePlantState == PlantOpActSate.POPS_NONE
                    || treePlantState == PlantOpActSate.POPS_CAN_APP)
                    return true;
            }

            return false;
        }

        #region UIEvent
        private void BindUiEvents()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange,
                OnCounterValueChanged);
        }

        private void UnBindUiEvents()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange,
                OnCounterValueChanged);
        }

        private void OnCounterValueChanged(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            //树木状态字段不存在
            if (string.IsNullOrEmpty(_treePlantStateStr) == true)
                return;

            string countKey = (string)uiEvent.Param1;

            //是否为树木的状态
            if (string.Equals(countKey, _treePlantStateStr) == false)
                return;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeUpdate, this);
        }
        #endregion


    }
}