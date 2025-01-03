using System;
using Protocol;
using Network;
using ProtoTable;
using System.Collections.Generic;
///////删除linq
using ActivityLimitTime;
using UnityEngine;


namespace GameClient
{
    public class WeekSignInDataManager : DataManager<WeekSignInDataManager>
    {

        public const string NewPlayerTaskAwardBoxImagePath = "UI/Image/Icon/Icon_Jar/item_jar_01.png:item_jar_01";
        public const string NewPlayerTaskAwardBoxEffectPath = "UIFlatten/Prefabs/Jar/EffUI_xiuzhenguan02";
        public const string ActivityTaskAwardBoxImagePath = "UI/Image/Icon/Icon_Jar/item_jar_05.png:item_jar_05";
        public const string ActivityTaskAwardBoxEffectPath = "UIFlatten/Prefabs/Jar/EffUI_xiuzhenguan06";

        public const string ActivityWeekSignInStr = "ActivityWeekSignIn";
        public const string NewPlayerWeekSignInStr = "NewPlayerWeekSignIn";

        public const int WeekSignInConfigId = 9380;                    //周签到对应的活动配置ID
        public const uint NewPlayerWeekSignInOpActTypeId = 5400;          //新人周签运营活动类型,同时对应活动模板表的ID
        public const uint ActivityWeekSignInOpActTypeId = 5500;           //活动周签运营活动类型

        public const int NewPlayerWeekSignInTipDesId = 10018;      //新人周签到通用提示表的ID
        public const int ActivityWeekSignInTipDesId = 10019;       //活动周签到通用提示表的ID

        private OpActivityData _newPlayerWeekSignInOpActivityData;
        private OpActivityData _activityWeekSignInOpActivityData;

        //预览奖励相关的数据
        private List<WeekSignInPreviewAwardDataModel> _newPlayerPreviewAwardDataModelList =
            new List<WeekSignInPreviewAwardDataModel>();
        private List<WeekSignInPreviewAwardDataModel> _activityPreviewAwardDataModelList =
            new List<WeekSignInPreviewAwardDataModel>();

        //奖励相关的数据
        private WeekSignInAwardDataModel _newPlayerWeekSignInAwardDataModel = new WeekSignInAwardDataModel();
        private WeekSignInAwardDataModel _activityWeekSignInAwardDataModel = new WeekSignInAwardDataModel();

        //奖励记录相关的数据
        private List<WeekSignRecord> _newPlayerWeekSignInRecordList = new List<WeekSignRecord>();
        private List<WeekSignRecord> _activityWeekSignInRecordList = new List<WeekSignRecord>();


        public override void Initialize()
        {
            BindNetEvents();
        }

        public override void Clear()
        {
            ClearData();
            UnBindNetEvents();
        }

        private void ClearData()
        {
            _newPlayerPreviewAwardDataModelList.Clear();
            _activityPreviewAwardDataModelList.Clear();

            if (_newPlayerWeekSignInAwardDataModel != null)
            {
                _newPlayerWeekSignInAwardDataModel.Reset();
            }

            if (_activityWeekSignInAwardDataModel != null)
            {
                _activityWeekSignInAwardDataModel.Reset();
            }

            _newPlayerWeekSignInRecordList.Clear();
            _activityWeekSignInRecordList.Clear();

            _newPlayerWeekSignInOpActivityData = null;
            _activityWeekSignInOpActivityData = null;
        }


        private void BindNetEvents()
        {
            //抽奖记录的请求
            NetProcess.AddMsgHandler(GASWeekSignRecordRes.MsgID, OnReceiveGASWeekSignRecordRes);

            //抽箱子的奖励
            NetProcess.AddMsgHandler(SceneWeekSignBoxNotify.MsgID, OnSyncSceneWeekSignBoxNotify);

            //累计周数和周签到
            NetProcess.AddMsgHandler(SceneWeekSignNotify.MsgID, OnSyncSceneWeekSignNotify);
            NetProcess.AddMsgHandler(SceneWeekSignRewardRes.MsgID, OnReceiveSceneWeekSignRewardRes);

            //运营活动的状态和数据
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityLimitTimeUpdate, OnActivityLimitTimeStateUpdate);
            UIEventSystem.GetInstance()
                .RegisterEventHandler(EUIEventID.ActivityLimitTimeDataUpdate, OnActivityLimitTimeDataUpdate);

            //运营活动任务的状态和数据
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityLimitTimeTaskUpdate,
                OnReceiveActivityLimitTimeTaskUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityLimitTimeTaskDataUpdate,
                OnReceiveActivityLimitTimeTaskDataUpdate);
        }

        private void UnBindNetEvents()
        {
            NetProcess.RemoveMsgHandler(GASWeekSignRecordRes.MsgID, OnReceiveGASWeekSignRecordRes);
            NetProcess.RemoveMsgHandler(SceneWeekSignBoxNotify.MsgID, OnSyncSceneWeekSignBoxNotify);

            NetProcess.RemoveMsgHandler(SceneWeekSignNotify.MsgID, OnSyncSceneWeekSignNotify);
            NetProcess.RemoveMsgHandler(SceneWeekSignRewardRes.MsgID, OnReceiveSceneWeekSignRewardRes);

            //运营活动的状态
            UIEventSystem.GetInstance()
                .UnRegisterEventHandler(EUIEventID.ActivityLimitTimeUpdate, OnActivityLimitTimeStateUpdate);

            //运营活动的数据
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityLimitTimeDataUpdate,
                OnActivityLimitTimeDataUpdate);

            //运营活动任务的状态
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityLimitTimeTaskUpdate,
                OnReceiveActivityLimitTimeTaskUpdate);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityLimitTimeTaskDataUpdate,
                OnReceiveActivityLimitTimeTaskDataUpdate);
        }

        //根据WeekSignInType得到ActType
        public uint GetOpActTypeByWeekSignInType(WeekSignInType weekSignInType)
        {
            if (weekSignInType == WeekSignInType.NewPlayerWeekSignIn)
            {
                return NewPlayerWeekSignInOpActTypeId;
            }
            return ActivityWeekSignInOpActTypeId;
        }


        #region ActivityAndTaskUIEvent
        //运营活动的任务数据发生改变
        private void OnReceiveActivityLimitTimeTaskDataUpdate(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var data = (LimitTimeActivityTaskUpdateData)uiEvent.Param1;
            ReceiveActivityLimitTimeTaskInfoUpdate(data);
        }

        //运营活动的任务状态发生改变
        private void OnReceiveActivityLimitTimeTaskUpdate(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var data = (LimitTimeActivityTaskUpdateData)uiEvent.Param1;
            ReceiveActivityLimitTimeTaskInfoUpdate(data);
        }

        //更新红点信息
        private void ReceiveActivityLimitTimeTaskInfoUpdate(LimitTimeActivityTaskUpdateData data)
        {
            if (data == null)
                return;

            //判断是否为周签到运营活动的数据
            var weekSignInOpActData = GetWeekSignInOpActivityDataByWeekSignInType(WeekSignInType.NewPlayerWeekSignIn);
            //是否为新人周签到的运营任务
            if (weekSignInOpActData != null && weekSignInOpActData.dataId == data.ActivityId)
            {
                //更新红点
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnNewPlayerWeekSignInRedPointChanged);
                return;
            }

            //是否为活动周签到的运营活动
            weekSignInOpActData = GetWeekSignInOpActivityDataByWeekSignInType(WeekSignInType.ActivityWeekSignIn);
            if (weekSignInOpActData != null && weekSignInOpActData.dataId == data.ActivityId)
            {
                //更新红点
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnActivityWeekSignInRedPointChanged);
                return;
            }
        }

        //运营活动的状态发生改变
        private void OnActivityLimitTimeStateUpdate(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;
            
            var activity = (IActivity) uiEvent.Param1;
            if (activity == null)
                return;

            var activityId =  activity.GetId();
            OnActivityLimitTimeInfoUpdate(activityId);
        }

        //运营活动的数据发生改变
        private void OnActivityLimitTimeDataUpdate(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            uint activityId = (uint)uiEvent.Param1;
            OnActivityLimitTimeInfoUpdate(activityId);
        }

        private void OnActivityLimitTimeInfoUpdate(uint activityId)
        {
            //判断是否为周签到的活动更新，如果是的话，更新新人周签到和活动周签到的缓存
            var weekSignInOpActData = GetWeekSignInOpActivityDataByWeekSignInType(WeekSignInType.NewPlayerWeekSignIn);
            //是否为新人周签到的运营活动
            if (weekSignInOpActData != null && weekSignInOpActData.dataId == activityId)
            {
                //更新新人周签到的缓存
                _newPlayerWeekSignInOpActivityData = ActivityDataManager.GetInstance().GetActiveDataFromType(
                    ActivityLimitTimeFactory.EActivityType.OAT_WEEK_SIGN_NEW_PLAYER);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnNewPlayerWeekSignInRedPointChanged);
                return;
            }

            //是否为活动周签到的运营活动
            weekSignInOpActData = GetWeekSignInOpActivityDataByWeekSignInType(WeekSignInType.ActivityWeekSignIn);
            if (weekSignInOpActData != null && weekSignInOpActData.dataId == activityId)
            {
                _activityWeekSignInOpActivityData = ActivityDataManager.GetInstance().GetActiveDataFromType(
                    ActivityLimitTimeFactory.EActivityType.OAT_WEEK_SIGN_ACTIVITY);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnActivityWeekSignInRedPointChanged);
                return;
            }
        }

        #endregion

        #region SignBox
        //同步宝箱数据
        private void OnSyncSceneWeekSignBoxNotify(MsgDATA msgData)
        {
            if (msgData == null)
            {
                Logger.LogErrorFormat("OnSyncSceneWeekSignNotice MsgData is null");
                return;
            }

            //周签到活动中的已签到周数和领取周数发生变化
            var sceneWeekSignBoxNotify = new SceneWeekSignBoxNotify();
            sceneWeekSignBoxNotify.decode(msgData.bytes);

            if (sceneWeekSignBoxNotify.opActType == NewPlayerWeekSignInOpActTypeId)
            {
                _newPlayerWeekSignInAwardDataModel.WeekSignInType = WeekSignInType.NewPlayerWeekSignIn;
                _newPlayerWeekSignInAwardDataModel.WeekSignInBoxItemList.Clear();
                for (var i = 0; i < sceneWeekSignBoxNotify.boxData.Length; i++)
                {
                    _newPlayerWeekSignInAwardDataModel.WeekSignInBoxItemList.Add(sceneWeekSignBoxNotify.boxData[i]);
                }
                
                UIEventSystem.GetInstance()
                    .SendUIEvent(EUIEventID.OnSyncSceneWeekSignBoxNotify, WeekSignInType.NewPlayerWeekSignIn);
            }
            else if (sceneWeekSignBoxNotify.opActType == ActivityWeekSignInOpActTypeId)
            {
                _activityWeekSignInAwardDataModel.WeekSignInType = WeekSignInType.ActivityWeekSignIn;
                _activityWeekSignInAwardDataModel.WeekSignInBoxItemList.Clear();
                for (var i = 0; i < sceneWeekSignBoxNotify.boxData.Length; i++)
                {
                    _activityWeekSignInAwardDataModel.WeekSignInBoxItemList.Add(sceneWeekSignBoxNotify.boxData[i]);
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSyncSceneWeekSignBoxNotify,
                    WeekSignInType.ActivityWeekSignIn);
            }
        }

        //得到活动任务的奖励
        public WeekSignBox GetWeekSignBoxByWeekSignInType(WeekSignInType weekSignInType, int opActTaskId)
        {
            var weekSignInAwardDataModel = GetWeekSignInAwardDataModelByWeekSignInType(weekSignInType);
            if (weekSignInAwardDataModel == null || weekSignInAwardDataModel.WeekSignInBoxItemList == null
                || weekSignInAwardDataModel.WeekSignInBoxItemList.Count <= 0)
                return null;

            for (var i = 0; i < weekSignInAwardDataModel.WeekSignInBoxItemList.Count; i++)
            {
                if (weekSignInAwardDataModel.WeekSignInBoxItemList[i].opActId == opActTaskId)
                    return weekSignInAwardDataModel.WeekSignInBoxItemList[i];
            }

            return null;
        }

        #endregion

        #region WeekSignInTotalWeekData
        //同步周数和已领取的周数
        private void OnSyncSceneWeekSignNotify(MsgDATA msgData)
        {

            if (msgData == null)
            {
                Logger.LogErrorFormat("OnSyncSceneWeekSignNotice MsgData is null");
                return;
            }

            //周签到活动中的已签到周数和领取周数发生变化

            SceneWeekSignNotify sceneWeekSignNotify = new SceneWeekSignNotify();
            sceneWeekSignNotify.decode(msgData.bytes);

            if (sceneWeekSignNotify.opActType == NewPlayerWeekSignInOpActTypeId)
            {
                _newPlayerWeekSignInAwardDataModel.WeekSignInType = WeekSignInType.NewPlayerWeekSignIn;
                _newPlayerWeekSignInAwardDataModel.AlreadySignInWeek = sceneWeekSignNotify.signWeekSum;
                _newPlayerWeekSignInAwardDataModel.AlreadyReceiveWeekList.Clear();
                for (var i = 0; i < sceneWeekSignNotify.yetWeek.Length; i++)
                {
                    _newPlayerWeekSignInAwardDataModel.AlreadyReceiveWeekList.Add(sceneWeekSignNotify.yetWeek[i]);
                }

                UIEventSystem.GetInstance()
                    .SendUIEvent(EUIEventID.OnSyncSceneWeekSignInNotify, WeekSignInType.NewPlayerWeekSignIn);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnNewPlayerWeekSignInRedPointChanged);
            }
            else if(sceneWeekSignNotify.opActType == ActivityWeekSignInOpActTypeId)
            {
                _activityWeekSignInAwardDataModel.WeekSignInType = WeekSignInType.ActivityWeekSignIn;
                _activityWeekSignInAwardDataModel.AlreadySignInWeek = sceneWeekSignNotify.signWeekSum;
                _activityWeekSignInAwardDataModel.AlreadyReceiveWeekList.Clear();
                for (var i = 0; i < sceneWeekSignNotify.yetWeek.Length; i++)
                {
                    _activityWeekSignInAwardDataModel.AlreadyReceiveWeekList.Add(sceneWeekSignNotify.yetWeek[i]);
                }
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSyncSceneWeekSignInNotify,
                    WeekSignInType.ActivityWeekSignIn);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnActivityWeekSignInRedPointChanged);
            }
        }

        //领取周签到奖励的返回
        private void OnReceiveSceneWeekSignRewardRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            SceneWeekSignRewardRes res = new SceneWeekSignRewardRes();
            res.decode(msgData.bytes);

            if (res.retCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int) res.retCode);
            }
        }

        //发送领取周签到的奖励
        public void SendSceneWeekSignRewardReq(uint opActType, uint weekId)
        {
            SceneWeekSignRewardReq req = new SceneWeekSignRewardReq();
            req.opActType = opActType;
            req.weekID = weekId;

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }
        #endregion

        #region WeekSignInRecord
        //奖励记录的相关内容
        public void SendGASWeekSignRecordReq(WeekSignInType weekSignInType)
        {
            GASWeekSignRecordReq req = new GASWeekSignRecordReq();
            var opActTypeId = GetOpActTypeByWeekSignInType(weekSignInType);
            req.opActType = opActTypeId;

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        private void OnReceiveGASWeekSignRecordRes(MsgDATA msgData)
        {
            GASWeekSignRecordRes res = new GASWeekSignRecordRes();
            res.decode(msgData.bytes);

            if (res.opActType == NewPlayerWeekSignInOpActTypeId)
            {
                _newPlayerWeekSignInRecordList.Clear();

                if (res.record != null && res.record.Length > 0)
                {
                    for (var i = 0; i < res.record.Length; i++)
                        _newPlayerWeekSignInRecordList.Add(res.record[i]);

                    //按照创建时间排序，最近创建的放在第一的位置
                    if (_newPlayerWeekSignInRecordList != null && _newPlayerWeekSignInRecordList.Count > 1)
                    {
                        _newPlayerWeekSignInRecordList.Sort((x, y) => -x.createTime.CompareTo(y.createTime));
                    }
                }
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveGasWeekSignInRecordRes, WeekSignInType.NewPlayerWeekSignIn);
            }
            else if (res.opActType == ActivityWeekSignInOpActTypeId)
            {
                _activityWeekSignInRecordList.Clear();

                if (res.record != null && res.record.Length > 0)
                {
                    for (var i = 0; i < res.record.Length; i++)
                        _activityWeekSignInRecordList.Add(res.record[i]);

                    //按照创建时间排序，最近创建的放在第一的位置
                    if (_activityWeekSignInRecordList != null && _activityWeekSignInRecordList.Count > 1)
                    {
                        _activityWeekSignInRecordList.Sort((x, y) => -x.createTime.CompareTo(y.createTime));
                    }
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveGasWeekSignInRecordRes,
                    WeekSignInType.ActivityWeekSignIn);
            }
        }

        public List<WeekSignRecord> GetWeekSignInRecordListByWeekSignInType(WeekSignInType weekSignInType)
        {
            if (weekSignInType == WeekSignInType.NewPlayerWeekSignIn)
            {
                return _newPlayerWeekSignInRecordList;
            }
            else if (weekSignInType == WeekSignInType.ActivityWeekSignIn)
            {
                return _activityWeekSignInRecordList;
            }

            return null;
        }

        //重置
        public void ResetWeekSignInRecord()
        {
            _activityWeekSignInRecordList.Clear();
            _newPlayerWeekSignInRecordList.Clear();
        }

        #endregion

        //得到预览的奖励
        public List<WeekSignInPreviewAwardDataModel> GetPreviewAwardItemDataModelListByWeekSignInType(
            WeekSignInType weekSignInType)
        {
            var curPreviewAwardDataModelList = _activityPreviewAwardDataModelList;
            if (weekSignInType == WeekSignInType.NewPlayerWeekSignIn)
            {
                curPreviewAwardDataModelList = _newPlayerPreviewAwardDataModelList;
            }

            if (curPreviewAwardDataModelList.Count > 0)
                return curPreviewAwardDataModelList;
            else
            {
                //进行缓存
                var opActType = GetOpActTypeByWeekSignInType(weekSignInType);
                var weekSignTables = TableManager.GetInstance().GetTable<WeekSignTable>();
                var iter = weekSignTables.GetEnumerator();
                while (iter.MoveNext())
                {
                    var curWeekSignTable = iter.Current.Value as WeekSignTable;
                    if (curWeekSignTable != null 
                        && curWeekSignTable.opActType == opActType
                        && curWeekSignTable.Preview == 1)       //Preview 为 0， 预览奖励不显示
                    {
                        var curPreviewAwardDataModel = new WeekSignInPreviewAwardDataModel()
                        {
                            OpActType = curWeekSignTable.opActType,
                            ItemId = curWeekSignTable.rewardId,
                            ItemNumber = curWeekSignTable.rewardNum,
                            IsSpecialAward = curWeekSignTable.isBigReward == 1 ? true : false,
                            SortId = curWeekSignTable.sortId,
                        };
                        curPreviewAwardDataModelList.Add(curPreviewAwardDataModel);
                    }
                }

                if (curPreviewAwardDataModelList.Count > 0)
                    curPreviewAwardDataModelList.Sort((x, y) => x.SortId.CompareTo(y.SortId));

                return curPreviewAwardDataModelList;
            }
        }

        //对应类型的奖励数据
        public WeekSignInAwardDataModel GetWeekSignInAwardDataModelByWeekSignInType(
            WeekSignInType weekSignInType)
        {

            if (weekSignInType == WeekSignInType.NewPlayerWeekSignIn)
            {
                if (_newPlayerWeekSignInAwardDataModel.WeekSignInType == WeekSignInType.None)
                    _newPlayerWeekSignInAwardDataModel.WeekSignInType = WeekSignInType.NewPlayerWeekSignIn;

                if (_newPlayerWeekSignInAwardDataModel.WeekSignSumTableList == null
                    || _newPlayerWeekSignInAwardDataModel.WeekSignSumTableList.Count <= 0)
                    _newPlayerWeekSignInAwardDataModel.WeekSignSumTableList =
                        WeekSignInUtility.GetWeekSignSumTableListByWeekSignSumTablesInType(WeekSignInType.NewPlayerWeekSignIn);

                return _newPlayerWeekSignInAwardDataModel;
            }
            else if (weekSignInType == WeekSignInType.ActivityWeekSignIn)
            {
                if (_activityWeekSignInAwardDataModel.WeekSignInType == WeekSignInType.None)
                    _activityWeekSignInAwardDataModel.WeekSignInType = WeekSignInType.ActivityWeekSignIn;

                if (_activityWeekSignInAwardDataModel.WeekSignSumTableList == null
                    || _activityWeekSignInAwardDataModel.WeekSignSumTableList.Count <= 0)
                    _activityWeekSignInAwardDataModel.WeekSignSumTableList =
                        WeekSignInUtility.GetWeekSignSumTableListByWeekSignSumTablesInType(WeekSignInType.ActivityWeekSignIn);

                return _activityWeekSignInAwardDataModel;
            }

            return null;
        }

        //周签到的活动数据
        public OpActivityData GetWeekSignInOpActivityDataByWeekSignInType(WeekSignInType weekSignInType)
        {
            if (weekSignInType == WeekSignInType.NewPlayerWeekSignIn)
            {
                if (_newPlayerWeekSignInOpActivityData == null)
                    _newPlayerWeekSignInOpActivityData = ActivityDataManager.GetInstance().GetActiveDataFromType(
                        ActivityLimitTimeFactory.EActivityType.OAT_WEEK_SIGN_NEW_PLAYER);

                return _newPlayerWeekSignInOpActivityData;
            }
            else
            {
                if (_activityWeekSignInOpActivityData == null)
                    _activityWeekSignInOpActivityData = ActivityDataManager.GetInstance().GetActiveDataFromType(
                        ActivityLimitTimeFactory.EActivityType.OAT_WEEK_SIGN_ACTIVITY);

                return _activityWeekSignInOpActivityData;
            }
        }

        //时间戳转化为具体的时间：1979年01月22日
        public string GetWeekSignInTimeLabelByWeekSignInType(WeekSignInType weekSignInType)
        {
            var opActivityData = GetWeekSignInOpActivityDataByWeekSignInType(weekSignInType);

            if (opActivityData == null)
                return "";

            var beginTimeStr = Function.GetOneData((int)opActivityData.startTime);
            var endTimeStr = Function.GetOneData((int) opActivityData.endTime);

            if(weekSignInType == WeekSignInType.NewPlayerWeekSignIn)
                return TR.Value("week_sing_in_new_player_time", beginTimeStr, endTimeStr);

            return TR.Value("week_sing_in_activity_time", beginTimeStr, endTimeStr);
        }

        //任务完成，领取任务的奖励
        public void OnSendRequestOnTakeActTask(uint activityDataId, uint taskDataId)
        {
            ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.RequestOnTakeActTask(activityDataId, taskDataId);
        }
       
    }
}
