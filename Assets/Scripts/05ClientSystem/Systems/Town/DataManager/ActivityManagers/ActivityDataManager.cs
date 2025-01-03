using Protocol;
using Network;
using ProtoTable;
using System.Collections.Generic;
using System;
using ActivityLimitTime;
using LimitTimeGift;
using System.Collections;

namespace GameClient
{
    public class LimitTimeActivityTaskUpdateData
    {
        public LimitTimeActivityTaskUpdateData(int activityId, int taskId)
        {
            ActivityId = activityId;
            TaskId = taskId;
        }

        public int ActivityId { get; private set; }
        public int TaskId { get; private set; }
    }

    public class AcivtityCounterRes
    {
        /// <summary>
        ///  计数id
        /// </summary>
        public UInt32 CounterId;
        /// <summary>
        ///  计数值
        /// </summary>
        public UInt32 CounterValue;

        public AcivtityCounterRes(UInt32 counterID, UInt32 counterValue)
        {
            CounterId = counterID;
            CounterValue = counterValue;
        }
    }
    public class ActivityDataManager : DataManager<ActivityDataManager>
    {
        //限时活动数据
        readonly Dictionary<uint, OpActivityData> mLimitTimeActivityDatas = new Dictionary<uint, OpActivityData>();
        //限时活动的任务数据
        readonly Dictionary<uint, OpActTask> mLimitTimeTaskDatas = new Dictionary<uint, OpActTask>();
        //Boss活动数据
        readonly Dictionary<uint, ActivityInfo> mBossActivityDatas = new Dictionary<uint, ActivityInfo>();
        //Boss兑换活动任务数据 key为活动表中配置的id
        readonly Dictionary<uint, SceneNotifyActiveTaskVar> mBossTaskDatas = new Dictionary<uint, SceneNotifyActiveTaskVar>();
        //Boss兑换活动任务数据, key为taskId
        readonly Dictionary<uint, SceneNotifyActiveTaskStatus> mBossTaskStatusDatas = new Dictionary<uint, SceneNotifyActiveTaskStatus>();
        //Boss击杀活动怪物数据 key是activityId
        readonly Dictionary<uint, WorldActivityMonsterRes> mBossKillMonsterDatas = new Dictionary<uint, WorldActivityMonsterRes>();
        //限时礼包活动数据 外面的key是商城商品类型MallType, 里面的key是商品id(即是活动id)
        readonly Dictionary<int, Dictionary<uint, MallItemInfo>> mGiftPackDatas = new Dictionary<int, Dictionary<uint, MallItemInfo>>();
        //活动页签表数据(服务器下发下来的)
        private readonly Dictionary<int, ActivityTabInfo> mActivityTabTables = new Dictionary<int, ActivityTabInfo>();

        //KEY活动ID或者任务id  Value：保存对应的counterId和counterValue 运营活动
        readonly Dictionary<UInt32, AcivtityCounterRes> mLimitTimeActivityCounterResDic = new Dictionary<UInt32, AcivtityCounterRes>();
        //KEY活动ID或者任务id  Value：保存对应的counterId和counterValue Boss活动
        readonly Dictionary<UInt32, AcivtityCounterRes> mBossActivityCounterResDic = new Dictionary<uint, AcivtityCounterRes>();
        //KEY是地下城的子类型 Value：保存对应的counterId和counterValue Boss活动
        readonly Dictionary<UInt32, AcivtityCounterRes> mDungeonCounterResDic = new Dictionary<uint, AcivtityCounterRes>();

        /// <summary>
        /// 限时团购最终折扣
        /// </summary>
        public static uint LimitTimeGroupBuyDiscount = 100;

        ///获取页签表中的大页签信息（包含所有该页签下的活动）
        public ActivityTabInfo GetActivityTabInfo(int filterId)
        {
            if (mActivityTabTables.ContainsKey(filterId))
            {
                return mActivityTabTables[filterId];
            }
            return null;
        }

        //获取页签表信息
        public Dictionary<int, ActivityTabInfo> GetTabInfos()
        {
            return mActivityTabTables;

        }

        //根据活动id获取对应的大页签id
        public int GetFilterIdByActivityId(int activityId)
        {
            int firstId = -1;
            foreach (var data in mActivityTabTables)
            {
                if (firstId == -1)
                {
                    firstId = data.Key;
                }
                var tabData = data.Value as ActivityTabInfo;
                if (tabData != null)
                {
                    for (int i = 0; i < tabData.actIds.Length; ++i)
                    {
                        if (tabData.actIds[i] == activityId)
                        {
                            return data.Key;
                        }
                    }
                }
            }
            return firstId;
        }

        //获取限时活动数据 该数据是协议发过来的数据
        public OpActivityData GetLimitTimeActivityData(uint activityId)
        {
            if (mLimitTimeActivityDatas.ContainsKey(activityId))
            {
                return mLimitTimeActivityDatas[activityId];
            }
            return null;
        }

        //获取任务数据
        public OpActTask GetLimitTimeTaskData(uint taskId)
        {
            if (mLimitTimeTaskDatas.ContainsKey(taskId))
            {
                return mLimitTimeTaskDatas[taskId];
            }
            return null;
        }

        //获取任务描述
        public string GetTaskDesByTaskId(uint taskId, uint activityId)
        {
            if (mLimitTimeActivityDatas.ContainsKey(activityId))
            {
                var activityData = mLimitTimeActivityDatas[activityId];
                if (activityData == null)
                    return null;

                var skillDescArray = activityData.taskDesc.Split('|');
                for (var i = 0; i < activityData.tasks.Length; ++i)
                {
                    //var task = ActivityDataManager.GetInstance().GetLimitTimeTaskData(activityData.tasks[i].dataid);
                    if (activityData.tasks[i].dataid == taskId)
                    {
                        if (i < skillDescArray.Length)
                        {
                            return skillDescArray[i];
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            return null;
        }

        //获取对应类型的礼包列表 用于不同购买活动
        public List<MallItemInfo> GetGiftPackInfos(MallTypeTable.eMallType mallType)
        {
            if (mGiftPackDatas.ContainsKey((int)mallType))
            {
                return new List<MallItemInfo>(mGiftPackDatas[(int)mallType].Values);
            }

            return null;
        }

        //是否包含某 限时购买 活动
        public bool IsContainGiftActivity(MallTypeTable.eMallType mallType, uint activityId)
        {
            if (mGiftPackDatas.ContainsKey((int)mallType))
            {
                return mGiftPackDatas[(int)mallType].ContainsKey(activityId);
            }

            return false;
        }

        //获取boss活动
        public ActivityInfo GetBossActivityData(uint activityId)
        {
            if (mBossActivityDatas.ContainsKey(activityId))
            {
                return mBossActivityDatas[activityId];
            }

            return null;
        }


        //获取到boss活动的任务
        public SceneNotifyActiveTaskVar GetBossTaskData(uint taskId)
        {
            if (mBossTaskDatas.ContainsKey(taskId))
            {
                return mBossTaskDatas[taskId];
            }

            return null;
        }

        //获取boss活动任务的状态
        public SceneNotifyActiveTaskStatus GetBossTaskStatusData(uint taskId)
        {
            if (mBossTaskStatusDatas.ContainsKey(taskId))
            {
                return mBossTaskStatusDatas[taskId];
            }

            return null;
        }

        //获取活动怪物的数据
        public WorldActivityMonsterRes GetBossKillMonsterData(uint activityId)
        {
            if (mBossKillMonsterDatas.ContainsKey(activityId))
            {
                return mBossKillMonsterDatas[activityId];
            }

            return null;
        }

        //获取某类型的商品id数据
        public MallItemInfo GetGiftPackData(ProtoTable.MallTypeTable.eMallType mallType, uint mallItemId)
        {
            if (mGiftPackDatas.ContainsKey((int)mallType) && mGiftPackDatas[(int)mallType].ContainsKey(mallItemId))
            {
                return mGiftPackDatas[(int)mallType][mallItemId];
            }

            return null;
        }

        //购买商城道具请求（没有引用？
        public void BuyGift(uint mallItemTableID)
        {
            WorldMallBuy req = new WorldMallBuy();

            req.itemId = mallItemTableID;
            req.num = 1;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
            WaitNetMessageManager.GetInstance().Wait<WorldMallBuyRet>(ret =>
            {
                if (ret.mallitemid == mallItemTableID)
                {
                    //发送UI事件
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnFashionTicketBuyFinished);
                }
            }, false);
        }

        //初始化
        public override void Initialize()
        {
            Clear();
            _BindNetMsg();
            //精力燃烧自动关闭
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FatigueChanged,_OnFatigueChanged);
            monthSignInAwards.Clear();
            monthSignCollectAwards.Clear();
            {
                Dictionary<int, object> dicts = TableManager.instance.GetTable<MonthSignAward>();
                if (dicts != null)
                {
                    var iter = dicts.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        MonthSignAward adt = iter.Current.Value as MonthSignAward;
                        if (adt == null)
                        {
                            continue;
                        }
                        Dictionary<int, MonthSignAward> dic = null;
                        monthSignInAwards.TryGetValue(adt.Month, out dic);
                        if (dic == null)
                        {
                            monthSignInAwards.Add(adt.Month, new Dictionary<int, MonthSignAward>());
                            dic = monthSignInAwards[adt.Month];
                        }
                        if (dic == null)
                        {
                            continue;
                        }
                        if (!dic.ContainsKey(adt.Day))
                        {
                            dic.Add(adt.Day, adt);
                        }
                        else
                        {
                            dic[adt.Day] = adt;
                        }
                    }
                }
            }
            {
                Dictionary<int, object> dicts = TableManager.instance.GetTable<MonthSignCollectAward>();
                if (dicts != null)
                {
                    var iter = dicts.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        MonthSignCollectAward adt = iter.Current.Value as MonthSignCollectAward;
                        if (adt == null)
                        {
                            continue;
                        }
                        monthSignCollectAwards.SafeAdd(adt.ID, adt);
                    }
                }
            }
            HasPopUpGetInspireBufFrame = false;
            NotPopUpRefreshBufMsgBox = false;
            InitLimiteBargainShowTable();
            InitWholeBargainDiscountTable();
        }
        void _OnFatigueChanged(UIEvent uiEvent)
        {
            // TODO 精力小于10时要关闭精力燃烧
            // if (PlayerBaseData.GetInstance().fatigue <= Utility.GetClientIntValue(ClientConstValueTable.eKey.CLOSE_FATIGUE_ACTIVITY_LIMIT_VALUE, 10))
            // {
            //     var curData = ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.GetCurUseFatigueData();
            //     if (null == curData)
            //         return;
            // }
        }
        public override void Clear()
        {
            _UnBindNetMsg();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FatigueChanged, _OnFatigueChanged);
            if (mLimitTimeActivityDatas != null)
            {
                mLimitTimeActivityDatas.Clear();
            }

            if (mLimitTimeTaskDatas != null)
            {
                mLimitTimeTaskDatas.Clear();
            }

            if (mBossActivityDatas != null)
            {
                mBossActivityDatas.Clear();
            }

            if (mBossTaskDatas != null)
            {
                mBossTaskDatas.Clear();
            }

            if (mBossKillMonsterDatas != null)
            {
                mBossKillMonsterDatas.Clear();
            }

            if (mGiftPackDatas != null)
            {
                mGiftPackDatas.Clear();
            }

            if (mBossTaskStatusDatas != null)
            {
                mBossTaskStatusDatas.Clear();
            }

            if (mActivityTabTables != null)
            {
                mActivityTabTables.Clear();
            }
            if (mLimitTimeActivityCounterResDic != null)
            {
                mLimitTimeActivityCounterResDic.Clear();

            }
            if(mBossActivityCounterResDic!=null)
            {
                mBossActivityCounterResDic.Clear();
            }

            if(mDungeonCounterResDic!=null)
            {
                mDungeonCounterResDic.Clear();
            }
            if (monthlySignInItemDatas != null)
            {
                monthlySignInItemDatas.Clear();
            }
            if (accumulativeSignInItemDatas != null)
            {
                accumulativeSignInItemDatas.Clear();
            }

            if (mPreviewDict != null)
            {
                mPreviewDict.Clear();
            }

            if (mDiscountDataList != null)
            {
                mDiscountDataList.Clear();
            }

            HasPopUpGetInspireBufFrame = false;
            NotPopUpRefreshBufMsgBox = false;

            LimitTimeGroupBuyDiscount = 100;
        }

        void _BindNetMsg()
        {
            NetProcess.AddMsgHandler(SceneOpActivityGetCounterRes.MsgID, _OnSysncLimitTimeAccountNum);//账号次数
            NetProcess.AddMsgHandler(WorldActivityTabsInfoSync.MsgID, _OnSyncActivityTabsInfo);//同步活动页签数据 增量的方式
            _BindBossActivityMsg();
            _BindLimitTimeActivityMsg();
            _BindGiftPackActivityMsg();
            NetProcess.AddMsgHandler(SceneNewSignInQueryRet.MsgID, _OnSceneNewSignInQueryRet);
            NetProcess.AddMsgHandler(SceneNewSignRet.MsgID, _OnSceneNewSignRet);
            NetProcess.AddMsgHandler(SceneNewSignInCollectRet.MsgID, _OnSceneNewSignInCollectRet);
            NetProcess.AddMsgHandler(SceneDungeonZjslRefreshBuffRes.MsgID, _OnSceneDungeonZjslRefreshBuffRes);

            NetProcess.AddMsgHandler(SceneChallengeScoreRet.MsgID, _OnChallengeScoreRet);

           //植树节活动
            BindArborDayNetMessages();

            //限时团购活动
            RegisterASWholeBargainNet(); 
        }

       

        void _BindGiftPackActivityMsg()
        {
            NetProcess.AddMsgHandler(WorldMallQueryItemRet.MsgID, _OnSyncLimitTimeGiftData);//请求限时礼包数据的返回协议
            NetProcess.AddMsgHandler(WorldMallBuyRet.MsgID, _OnGiftBuyRes);//购买返回
            NetProcess.AddMsgHandler(SyncWorldMallGiftPackActivityState.MsgID, _OnSyncLimitTimeAct);
        }

        void _BindBossActivityMsg()
        {
            NetProcess.AddMsgHandler(SceneSyncClientActivities.MsgID, _OnSyncBossActivities);//同步boss活动数据
            NetProcess.AddMsgHandler(SceneNotifyActiveTaskStatus.MsgID, _OnRecvBossActivityTaskStateChange);//boss兑换活动任务状态数据
            NetProcess.AddMsgHandler(SceneNotifyActiveTaskVar.MsgID, _OnSyncBossActivityTaskData);//boss兑换活动任务数据
            NetProcess.AddMsgHandler(WorldActivityMonsterRes.MsgID, _OnRecvBossActivityMonsterInfo);//boss击杀活动怪物数据
            NetProcess.AddMsgHandler(WorldNotifyClientActivity.MsgID, _OnRecvBossActivityStateChange);//boss活动状态更新
        }

        void _BindLimitTimeActivityMsg()
        {
            NetProcess.AddMsgHandler(SyncOpActivityDatas.MsgID, _OnSyncLimitTimeActivity);//同步活动数据
            NetProcess.AddMsgHandler(SyncOpActivityTasks.MsgID, _OnSyncLimitTimeActivityTasks);//同步活动任务数据
            NetProcess.AddMsgHandler(SyncOpActivityTaskChange.MsgID, _OnSyncLimitTimeActivityTaskChange);//任务状态改变
            NetProcess.AddMsgHandler(SyncOpActivityStateChange.MsgID, _OnSyncLimitTimeActivityStateChange);//活动状态改变



        }

        void _UnBindNetMsg()
        {
            NetProcess.RemoveMsgHandler(SyncOpActivityTasks.MsgID, _OnSyncLimitTimeActivityTasks);
            NetProcess.RemoveMsgHandler(WorldActivityTabsInfoSync.MsgID, _OnSyncActivityTabsInfo);
            _UnBindBossActivityMsg();
            _UnBindLimitTimeActivityMsg();
            _UnBindGiftPackActivityMsg();
            NetProcess.RemoveMsgHandler(SceneNewSignInQueryRet.MsgID, _OnSceneNewSignInQueryRet);
            NetProcess.RemoveMsgHandler(SceneNewSignRet.MsgID, _OnSceneNewSignRet);
            NetProcess.RemoveMsgHandler(SceneNewSignInCollectRet.MsgID, _OnSceneNewSignInCollectRet);
            NetProcess.RemoveMsgHandler(SceneDungeonZjslRefreshBuffRes.MsgID, _OnSceneDungeonZjslRefreshBuffRes);

            NetProcess.RemoveMsgHandler(SceneChallengeScoreRet.MsgID, _OnChallengeScoreRet);

            UnBindArborDayNetMessages();
            UnRegisterASWholeBargainNet();
        }

        void _UnBindGiftPackActivityMsg()
        {
            NetProcess.RemoveMsgHandler(WorldMallQueryItemRet.MsgID, _OnSyncLimitTimeGiftData);//请求限时礼包数据的返回协议
            NetProcess.RemoveMsgHandler(WorldMallBuyRet.MsgID, _OnGiftBuyRes);//购买返回
            NetProcess.RemoveMsgHandler(SyncWorldMallGiftPackActivityState.MsgID, _OnSyncLimitTimeAct);
        }

        void _UnBindLimitTimeActivityMsg()
        {
            NetProcess.RemoveMsgHandler(SyncOpActivityDatas.MsgID, _OnSyncLimitTimeActivity);
            NetProcess.RemoveMsgHandler(SyncOpActivityTaskChange.MsgID, _OnSyncLimitTimeActivityTaskChange);
            NetProcess.RemoveMsgHandler(SyncOpActivityStateChange.MsgID, _OnSyncLimitTimeActivityStateChange);

            NetProcess.RemoveMsgHandler(SceneOpActivityAcceptTaskRes.MsgID, _OnSysncLimitTimeAccountNum);//账号次数
        }

        void _UnBindBossActivityMsg()
        {
            NetProcess.RemoveMsgHandler(SceneSyncClientActivities.MsgID, _OnSyncBossActivities);
            NetProcess.RemoveMsgHandler(SceneNotifyActiveTaskStatus.MsgID, _OnRecvBossActivityTaskStateChange);
            NetProcess.RemoveMsgHandler(SceneNotifyActiveTaskVar.MsgID, _OnSyncBossActivityTaskData);
            NetProcess.RemoveMsgHandler(WorldActivityMonsterRes.MsgID, _OnRecvBossActivityMonsterInfo);
            NetProcess.RemoveMsgHandler(WorldNotifyClientActivity.MsgID, _OnRecvBossActivityStateChange);
        }

        //把活动页签表的数据下发下来
        void _OnSyncActivityTabsInfo(MsgDATA data)
        {
            WorldActivityTabsInfoSync kRecv = new WorldActivityTabsInfoSync();
            kRecv.decode(data.bytes);
            if (kRecv.tabsInfo != null)
            {
                for (int i = 0; i < kRecv.tabsInfo.Length; ++i)
                {
                    if (mActivityTabTables.ContainsKey((int)kRecv.tabsInfo[i].id))
                    {
                        mActivityTabTables.Remove((int)kRecv.tabsInfo[i].id);
                    }
                    mActivityTabTables.Add((int)kRecv.tabsInfo[i].id, kRecv.tabsInfo[i]);
                }
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityTabsInfoUpdate);
            }
        }

		#region 限时礼包相关

        public void RequestMallGiftData(MallTypeTable.eMallType mallType)
        {
            WorldMallQueryItemReq req = new WorldMallQueryItemReq();
            req.type = (byte)mallType;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 请求限时活动的商城礼包
        /// </summary>
        public void RequestMallGiftData()
        {
            var tableData = TableManager.GetInstance().GetTable<MallTypeTable>();
            if (tableData != null)
            {
                var enumerator = tableData.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    var data = (MallTypeTable)enumerator.Current.Value;
                    if (data.IsForActivity == 1)
                    {
                        WorldMallQueryItemReq req = new WorldMallQueryItemReq();
                        req.type = (byte)data.MallType;
                        NetManager netMgr = NetManager.Instance();
                        netMgr.SendCommand(ServerType.GATE_SERVER, req);
                    }
                }
            }
        }

#if TEST_DELETE_ACTIVITY
        public void TestDeleteGifPackActivity()
        {
            mGiftPackDatas[(int)MallTypeTable.eMallType.SN_ACTIVITY_GIFT].Clear();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeDataUpdate, (uint)79000);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeDataUpdate, (uint)79001);
        }
#endif

        //限时购买活动的商品数据
        void _OnSyncLimitTimeGiftData(MsgDATA msg)
        {
            WorldMallQueryItemRet res = new WorldMallQueryItemRet();
            res.decode(msg.bytes);
            //只存商城礼包和商城活动礼包、超级新服礼包数据,老兵回归礼包，国庆礼包。。。。。如果不想继续加枚举 需要新加一个表来配置
            var tableData = TableManager.GetInstance().GetTable<MallTypeTable>();
            bool isNeedProcess = false;
            if (tableData != null)
            {
                var enumerator = tableData.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    var data = (MallTypeTable)enumerator.Current.Value;
                    if ((byte)data.MallType == res.type)
                    {
                        isNeedProcess = true;
                        break;
                    }
                }
            }

            if (isNeedProcess && res.items != null)
            {
                if (!mGiftPackDatas.ContainsKey(res.type))
                {
                    mGiftPackDatas.Add(res.type, new Dictionary<uint, MallItemInfo>());
                }
                List<MallItemInfo> oldList = new List<MallItemInfo>(mGiftPackDatas[res.type].Values);
                mGiftPackDatas[res.type].Clear();

                for (int i = 0; i < res.items.Length; ++i)
                {
                    mGiftPackDatas[res.type][res.items[i].id] = res.items[i];
                }

                //商城活动礼包是发送活动更新消息
                if (res.type == (byte)MallTypeTable.eMallType.SN_ACTIVITY_GIFT)
                {
                    for (int i = 0; i < res.items.Length; ++i)
                    {
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeDataUpdate, res.items[i].id);
                    }

                    //如果活动结束了，新的数据里就没有这个活动了，所以要保存老的数据，遍历查找如果不在新的活动里，发送更新消息
                    for (int i = 0; i < oldList.Count; ++i)
                    {
                        if (!mGiftPackDatas[res.type].ContainsKey(oldList[i].id))
                        {
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeDataUpdate, oldList[i].id);
                        }
                    }
                }
                //商城礼包是发送任务更新消息
                else
                {
                    var activityId = _GetActivityGiftPackActivityId((MallTypeTable.eMallType)res.type);
                    if (activityId != 0)
                    {
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeDataUpdate, activityId);
                    }
                    //                    else
                    //                    {
                    //                        //TODO 本地生成活动
                    //#if UNITY_EDITOR
                    //                        Logger.LogError("检查运营活动表，找不到模板id为5000，扩展参数为" + res.type + "(对应商城表中MallType(分类名称)) 的活动。检查是否未配置活动或者活动扩展参数配置错误");
                    //#endif
                    //                    }
                }
            }
        }

        uint _GetActivityGiftPackActivityId(MallTypeTable.eMallType mallType)
        {
            foreach (var activity in mLimitTimeActivityDatas.Values)
            {
                if (activity.tmpType == (uint) OpActivityTmpType.OAT_LIMIT_TIME_GIFT_PACK && (uint)mallType == activity.parm||
                    activity.tmpType == (uint)ActivityLimitTimeFactory.EActivityType.OAT_FLYING_GIFTPACK_ACTIVITY && (uint)mallType == activity.parm)
                {
                    return activity.dataId;
                }
            }

            return 0;
        }

        /// <summary>
        /// 得到虚空加成活动id  
        /// </summary>
        /// <returns></returns>
        public uint GetActivityVanityBonusActivityId()
        {
            return _GetAdditionBuffActivityId(EadditionBuffType.XuKong);
        }

        /// <summary>
        /// 得到混沌加成活动id
        /// </summary>
        /// <param name="eadditionBuffType"></param>
        /// <returns></returns>
        public uint GetActivityChaosAdditionID()
        {
            return _GetAdditionBuffActivityId(EadditionBuffType.HunDun);
        }

        /// <summary>
        /// 得到加成活动id（现在有虚空加成和混沌加成）
        /// </summary>
        /// <returns></returns>
        private uint _GetAdditionBuffActivityId(EadditionBuffType eadditionBuffType)
        {
            foreach (var activity in mLimitTimeActivityDatas.Values)
            {
                if (activity.tmpType == (uint)OpActivityTmpType.OAT_DUNGEON_RANDOM_BUFF&&activity.parm==(uint)eadditionBuffType)//混沌加成和虚空加成活动用的同一个模板，这里要多添加一个判断
                {
                    return activity.dataId;
                }
            }

            return 0;
        }

        /// <summary>
        /// 得到黑市商人活动数据
        /// </summary>
        /// <returns></returns>
        public OpActivityData _GetBlackMarketMerchantOpActivityData()
        {
            foreach (var activity in mLimitTimeActivityDatas.Values)
            {
                if (activity.tmpType != (uint)OpActivityTmpType.OAT_BLACK_MARKET_SHOP)
                {
                    continue;
                }

                if (activity.state != (int)OpActivityState.OAS_IN)
                {
                    continue;
                }

                return activity;
            }

            return null;
        }

        /// <summary>
        /// 检查时装合成活动是否开启
        /// </summary>
        /// <returns></returns>
        public bool CheckFashionSynthesisActivityIsOpen()
        {
            foreach (var activity in mLimitTimeActivityDatas.Values)
            {
                if (activity.tmpType != (uint)OpActivityTmpType.OAT_FLYUP_GIFT)
                {
                    continue;
                }

                if (activity.parm2.Length <= 0)
                {
                    continue;
                }

                if (activity.parm2[0] != (uint)EGiftPackActivityType.FashionSynthesis)
                {
                    continue;
                }

                if (activity.state != (int)OpActivityState.OAS_IN)
                {
                    continue;
                }

                if (activity.parm3.Length <= 0)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 检查掉落翻倍活动是否开启
        /// </summary>
        /// <returns></returns>
        public bool CheckDropToDoubleActivityIsOpen(int taskId,ref int discount)
        {
            bool isDiscount = false;
            OpActivityData opActivityData = null;
            foreach (var activity in mLimitTimeActivityDatas.Values)
            {
                if (activity.tmpType != (uint)OpActivityTmpType.OAT_EXCHANE_DISCOUNT)
                {
                    continue;
                }

                if (activity.state != (int)OpActivityState.OAS_IN)
                {
                    continue;
                }

                opActivityData = activity;
            }

            if (opActivityData != null)
            {
                if (opActivityData.tasks.Length > 0)
                {
                    OpActTaskData opActTaskData = opActivityData.tasks[0];
                    OpActTask opActTask = GetLimitTimeTaskData(opActTaskData.dataid);
                    if (opActTask != null)
                    {
                        if (opActTask.state != (byte)TaskStatus.TASK_UNFINISH)
                        {
                            return isDiscount;
                        }
                        else
                        {
                            if (((IList)(opActTaskData.variables)).Contains((uint)taskId))
                            {
                                isDiscount = true;
                                if (opActTaskData.variables2.Length > 0)
                                {
                                    discount = (int)opActTaskData.variables2[0];
                                }
                            }
                        }
                    }
                }
            }

            return isDiscount;
        }
        
        void _OnGiftBuyRes(MsgDATA msg)
        {
            WorldMallBuyRet res = new WorldMallBuyRet();
            res.decode(msg.bytes);

            MallItemInfo item = _GetGiftPackById(res.mallitemid);
            if (item != null)
            {
                //商城活动礼包发送活动更新消息
                if (item.type == (byte)MallTypeTable.eMallType.SN_ACTIVITY_GIFT)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeDataUpdate, item.id);
                }
                //商城礼包发送任务更新消息
                else
                {
                    var activityId = _GetActivityGiftPackActivityId((MallTypeTable.eMallType)item.type);
                    if (activityId != 0)
                    {
                        LimitTimeActivityTaskUpdateData updateData = new LimitTimeActivityTaskUpdateData((int)activityId, (int)item.id);
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeTaskDataUpdate, updateData);
                    }
                }
            }
        }

        //TODO 现在是否需要此协议
        void _OnSyncLimitTimeAct(MsgDATA msg)
        {
            SyncWorldMallGiftPackActivityState res = new SyncWorldMallGiftPackActivityState();
            res.decode(msg.bytes);
        }

        private WorldMallQueryItemReq _ReadReqInfoFromTable(int tableId)
        {
            WorldMallQueryItemReq req = null;
            var mallTypeTable = TableManager.GetInstance().GetTableItem<MallTypeTable>(tableId);
            if (mallTypeTable != null)
            {
                req = new WorldMallQueryItemReq();
                req.type = (byte)mallTypeTable.MallType;
                var subTypeList = mallTypeTable.MallSubType;
                if (subTypeList != null && subTypeList.Count > 0)//|| subTypeList.Count == 0)
                {
                    req.subType = 0;
                    if (subTypeList[0] == 0)
                    {
                        req.subType = 0;
                    }
                }
                req.occu = (byte)mallTypeTable.ClassifyJob;
                req.moneyType = (byte)mallTypeTable.MoneyID;
                //Logger.LogError("ReadReqInfoFromTable  = " + req.type + " " + req.subType + " " + req.occu + " " + req.moneyType);
            }
            return req;
        }

        MallItemInfo _GetGiftPackById(uint mallitemid)
        {
            foreach (var datas in mGiftPackDatas.Values)
            {
                if (datas.ContainsKey(mallitemid))
                {
                    return datas[mallitemid];
                }
            }

            return null;
        }
        #endregion

        #region 限时活动相关

        public int GetActiveIdFromType(ActivityLimitTimeFactory.EActivityType type)
        {
            var enumerator = mLimitTimeActivityDatas.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Value.tmpType == (uint)type)
                {
                    return (int)enumerator.Current.Key;
                }
            }
            return 0;
        }
        public OpActivityData GetActiveDataFromType(ActivityLimitTimeFactory.EActivityType type)
        {
            var activityId = GetActiveIdFromType(type);
            var activityData = GetLimitTimeActivityData((uint)activityId);
            return activityData;
        }
        /// <summary>
        /// 得到不是初始化状态所有活动任务的ID
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<uint> GetHaveRecivedTaskID(ActivityLimitTimeFactory.EActivityType type)
        {
            List<uint> taskIDList = new List<uint>();
            OpActivityData activityData = GetActiveDataFromType(type);
            if (activityData != null)
            {
                if (activityData.tasks != null)
                {
                    for (int i = 0; i < activityData.tasks.Length; i++)
                    {
                        OpActTaskData taskData = activityData.tasks[i];
                        if (taskData == null) continue;
                        OpActTask actTask = GetLimitTimeTaskData(taskData.dataid);
                        if (actTask == null) continue;
                        if ((OpActTaskState)actTask.state != OpActTaskState.OATS_INIT)
                        {
                            taskIDList.Add(taskData.dataid);
                        }
                    }
                }
            }
            return taskIDList;
            
        }

        /// <summary>
        /// 判断周年祈福活动任务是否存在(特殊)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool GettAnniverTaskIsFinish(EAnniverBuffPrayType buffType)
        {
            bool result = false;
            OpActTaskData taskData = GetAnniverFinishTaskData(ActivityLimitTimeFactory.EActivityType.OAT_SECOND_ANNIVERSARY_PRAY, buffType);
            if (taskData != null)
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        ///周年祈福的深渊和远古的挑战次数是否用完
        /// </summary>
        /// <returns></returns>
        public bool IsLeftChallengeTimes(EAnniverBuffPrayType buffType,string counterKey)
        {
            bool result = false;
            OpActTaskData taskData = GetAnniverFinishTaskData(ActivityLimitTimeFactory.EActivityType.OAT_SECOND_ANNIVERSARY_PRAY, buffType);

            if(taskData!=null)
            {
                int curNum = CountDataManager.GetInstance().GetCount(counterKey);
                if(taskData.variables2!=null&&taskData.variables2.Length>1)
                {
                    int totalNum = (int)taskData.variables2[0];
                    result = curNum <totalNum;
                }
             
            }
            return result;
        }
        /// <summary>
        /// 得到周年祈福任务的数据
        /// </summary>
        /// <param name="activityType"></param>
        /// <param name="buffType"></param>
        /// <returns></returns>
        public int GetAnniverTaskValue(EAnniverBuffPrayType buffType)
        {
            uint value = 0;
            OpActTaskData taskData = GetAnniverFinishTaskData(ActivityLimitTimeFactory.EActivityType.OAT_SECOND_ANNIVERSARY_PRAY, buffType);
            if (taskData != null && taskData.variables2 != null)
            {
                if (taskData.variables2.Length >= 2)
                {
                    value = taskData.variables2[1];
                }else if(taskData.variables2.Length>=1)
                {
                    value = taskData.variables2[0];
                }
               
            }
            return (int)value;
        }
       
        private OpActTaskData GetAnniverFinishTaskData(ActivityLimitTimeFactory.EActivityType activityType, EAnniverBuffPrayType buffType)
        {
           
            OpActivityData activityData = GetActiveDataFromType(activityType);
            if (activityData != null)
            {
                if (activityData.tasks != null)
                {
                    for (int i = 0; i < activityData.tasks.Length; i++)
                    {
                        OpActTaskData taskData = activityData.tasks[i];
                        if (taskData == null) continue;
                        OpActTask actTask = GetLimitTimeTaskData(taskData.dataid);
                        if (actTask == null) continue;
                        if ((OpActTaskState)actTask.state == OpActTaskState.OATS_FINISHED&& (EAnniverBuffPrayType)taskData.completeNum == buffType)
                        {
                            return taskData;
                        }
                    }
                }
            }
            return null;
        }

		//请求活动任务操作 类似提交任务、交换道具
        public void RequestOnTakeActTask(UInt32 activityDataId, UInt32 taskDataId , ulong tempParam = 0)
        {
            TakeOpActTaskReq taskReq = new TakeOpActTaskReq();
            taskReq.activityDataId = activityDataId;
            taskReq.taskDataId = taskDataId;
            if (tempParam != 0)
            {
                taskReq.param = tempParam;
            }
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, taskReq);
        }

        public void RequestActivityTaskInfo(UInt32 activityDataId)
        {
            SceneOpActivityTaskInfoReq Req = new SceneOpActivityTaskInfoReq();
            Req.opActId = activityDataId;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, Req);
        }

        /// <summary>
        /// 登录时 服务器同步 本地限时活动 全部静态数据
        /// </summary>
        /// <param name="msg"></param>
        private void _OnSyncLimitTimeActivity(MsgDATA msg)
        {
            int pos = 0;
            SyncOpActivityDatas syncActivityDatas = new SyncOpActivityDatas();
            syncActivityDatas.decode(msg.bytes, ref pos);

            if (syncActivityDatas.datas == null)
                return;

            //以下代码为当两个打折活动同时存在的时候只显示节日的打折活动
            bool haveBuyFasionActivity = false;
            for (int i = 0; i < syncActivityDatas.datas.Length; i++)
            {
                if (syncActivityDatas.datas[i].tmpType == (int)OpActivityTmpType.OAT_BUY_FASHION &&
                    syncActivityDatas.datas[i].state == (int)OpActivityState.OAS_IN)
                {
                    haveBuyFasionActivity = true;
                    break;
                }
            }

            for (int i = 0; i < syncActivityDatas.datas.Length; ++i)
            {
                var syncData = syncActivityDatas.datas[i];
                //绑定手机活动、赌马、夺宝活动、强化券合成、强化祈福 不记录 （强化券合成、强化祈福在StrengthenTicketMergeDataManager处理
                if (syncData == null || syncData.tmpType == (uint)OpActivityTmpType.OAT_BIND_PHONE || syncData.tmpType == (uint)OpActivityTmpType.OAT_GAMBING || syncData.tmpType == (uint)OpActivityTmpType.OAT_BET_HORSE
                    || syncData.tmpType == (uint)OpActivityTmpType.OAT_STRENGTHEN_TICKET_SYNTHESIS || syncData.tmpType == (uint)3100)
                {
                    continue;
                }

                //当存在时装活动时候 屏蔽新服商城时装打折活动
                if (haveBuyFasionActivity && syncActivityDatas.datas[i].tmpType == (int)OpActivityTmpType.OAT_MALL_DISCOUNT_FOR_NEW_SERVER)
                {
                    continue;
                }
               
                mLimitTimeActivityDatas[syncActivityDatas.datas[i].dataId] = syncActivityDatas.datas[i];
              
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeDataUpdate, syncActivityDatas.datas[i].dataId);
            
                //请求活动任务的限领次数
                for (int j = 0; j < syncData.tasks.Length; j++)
                {
                    var taskData=  syncData.tasks[j];
                    if (taskData == null) return;
                    if (taskData.accountDailySubmitLimit > 0)
                    {
                        ActivityDataManager.GetInstance().SendSceneOpActivityGetCounterReq((int)taskData.dataid, ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_DAILY_SUBMIT_TASK);
                    }
                    if (taskData.accountTotalSubmitLimit > 0)
                    {
                        ActivityDataManager.GetInstance().SendSceneOpActivityGetCounterReq((int)taskData.dataid, ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_TOTAL_SUBMIT_TASK);
                    }

                    if(taskData.accountWeeklySubmitLimit > 0)
                        ActivityDataManager.GetInstance().SendSceneOpActivityGetCounterReq((int)taskData.dataid, ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_WEEKLY_SUBMIT_TASK);
                }
            }

        }

        //同步活动任务
        private void _OnSyncLimitTimeActivityTasks(MsgDATA msg)
        {
            int pos = 0;
            SyncOpActivityTasks data = new SyncOpActivityTasks();
            data.decode(msg.bytes, ref pos);
            if (data.tasks != null && data.tasks.Length > 0)
            {
                for (int i = 0; i < data.tasks.Length; ++i)
                {
                    mLimitTimeTaskDatas[data.tasks[i].dataId] = data.tasks[i];
                    //发送活动任务更新消息
                    LimitTimeActivityTaskUpdateData updateData = new LimitTimeActivityTaskUpdateData(_GetLimitTimeActivityIdByTaskId(data.tasks[i].dataId), (int)data.tasks[i].dataId);
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeTaskDataUpdate, updateData);

                }
            }
        }

        //同步活动任务状态和数据的改变
        private void _OnSyncLimitTimeActivityTaskChange(MsgDATA msg)
        {
            int pos = 0;
            SyncOpActivityTaskChange data = new SyncOpActivityTaskChange();
            data.decode(msg.bytes, ref pos);
            if (data.tasks != null && data.tasks.Length > 0)
            {
                for (int i = 0; i < data.tasks.Length; ++i)
                {
                    var task = data.tasks[i];
                    mLimitTimeTaskDatas[task.dataId] = task;
                    //发送活动任务更新消息
                    LimitTimeActivityTaskUpdateData updateData = new LimitTimeActivityTaskUpdateData(_GetLimitTimeActivityIdByTaskId(task.dataId), (int)task.dataId);
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeTaskDataUpdate, updateData);
                }
            }
        }

        //同步活动状态改变
        private void _OnSyncLimitTimeActivityStateChange(MsgDATA msg)
        {
            int pos = 0;
            SyncOpActivityStateChange data = new SyncOpActivityStateChange();
            data.decode(msg.bytes, ref pos);

            if (data.data != null)
            {
                mLimitTimeActivityDatas[data.data.dataId] = data.data;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeDataUpdate, data.data.dataId);
                if (mLimitTimeActivityDatas.ContainsKey(data.data.dataId) && data.data.state == (byte)OpActivityState.OAS_END)
                {
                    _RemoveLimitTimeTasksByActivityId(data.data.dataId);
                    mLimitTimeActivityDatas.Remove(data.data.dataId);
                }
            }
        }
        //获取账号次数
        private void _OnSysncLimitTimeAccountNum(MsgDATA msg)
        {
            int pos = 0;
            SceneOpActivityGetCounterRes data = new SceneOpActivityGetCounterRes();
            data.decode(msg.bytes, ref pos);
            if (data != null)
            {
                if (data.counterId == (uint)ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_DAILY_SUBMIT_TASK ||
                    data.counterId == (uint)ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_TOTAL_SUBMIT_TASK ||
                    data.counterId == (uint)ActivityLimitTimeFactory.EActivityCounterType.OAT_SUMMER_WEEKLY_VACATION ||
                    data.counterId == (uint)ActivityLimitTimeFactory.EActivityCounterType.QAT_SUMMER_DAILY_CHARGE ||
                    data.counterId == (uint)ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_WEEKLY_SUBMIT_TASK)
                {
                    if (!mLimitTimeActivityCounterResDic.ContainsKey(data.opActId))
                    {
                        mLimitTimeActivityCounterResDic.Add(data.opActId, new AcivtityCounterRes(data.counterId, data.counterValue));
                    }
                    else
                    {

                        mLimitTimeActivityCounterResDic[data.opActId].CounterValue = data.counterValue;
                    }
                }else if(data.counterId==(uint)ActivityLimitTimeFactory.EActivityCounterType.OP_ITEM_ACTIVITY_BEHAVIOR_TOTAL_SUBMIT_TASK)
                {
                    if (!mBossActivityCounterResDic.ContainsKey(data.opActId))
                    {
                        mBossActivityCounterResDic.Add(data.opActId, new AcivtityCounterRes(data.counterId, data.counterValue));
                    }
                    else
                    {

                        mBossActivityCounterResDic[data.opActId].CounterValue = data.counterValue;
                    }
                }else if(data.counterId==(uint)ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_DAILY_DUNGEON_COUNT)
                {
                    if (!mDungeonCounterResDic.ContainsKey(data.opActId))
                    {
                        mDungeonCounterResDic.Add(data.opActId, new AcivtityCounterRes(data.counterId, data.counterValue));
                    }
                    else
                    {

                        mDungeonCounterResDic[data.opActId].CounterValue = data.counterValue;
                    }
                }
                 UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivtiyLimitTimeAccounterNumUpdate, data.opActId, data.counterId, (int)data.counterValue);
                //更新活动数据
                 UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeDataUpdate, data.opActId);
                //发送活动任务更新消息
                LimitTimeActivityTaskUpdateData updateData = new LimitTimeActivityTaskUpdateData(_GetLimitTimeActivityIdByTaskId(data.opActId), (int)data.opActId);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeTaskDataUpdate, updateData);
            }
        }


        /// <summary>
        /// 发送账号次数请求 活动相关 --运营活动
        /// </summary>
        /// <param name="eActivityType"></param>
        /// <param name="counterId"></param>
        public void SendSceneOpActivityGetCounterReq(ActivityLimitTimeFactory.EActivityType eActivityType, ActivityLimitTimeFactory.EActivityCounterType eActivityCounterType)
        {
            SceneOpActivityGetCounterReq req = new SceneOpActivityGetCounterReq();
            //活动ID
            req.opActId = (UInt32)ActivityDataManager.GetInstance().GetActiveIdFromType(eActivityType);
            req.counterId = (UInt32)eActivityCounterType;
            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        ///  发送账号次数请求 活动相关 --地下城
        /// </summary>
        /// <param name="dungeonSubType">地下城子类型</param>
        public void SendSceneOpActivityGetCounterReq(int dungeonSubType)
        {
            SceneOpActivityGetCounterReq req = new SceneOpActivityGetCounterReq();
            //地下城子类型
            req.opActId = (UInt32)dungeonSubType;
            req.counterId =(UInt32)ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_DAILY_DUNGEON_COUNT;
            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 发送账号次数请求 任务相关   --运营活动
        /// </summary>
        /// <param name="eActivityType"></param>
        /// <param name="counterId"></param>
        public void SendSceneOpActivityGetCounterReq(int taskID, ActivityLimitTimeFactory.EActivityCounterType eActivityCounterType)
        {
            SceneOpActivityGetCounterReq req = new SceneOpActivityGetCounterReq();
            //活动ID
            req.opActId = (UInt32)taskID;
            req.counterId = (UInt32)eActivityCounterType;
            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }
        /// <summary>
        /// 得到账号的次数 活动相关
        /// </summary>
        /// <returns></returns>
        public UInt32 GetActivityConunter(ActivityLimitTimeFactory.EActivityType eActivityType, ActivityLimitTimeFactory.EActivityCounterType eActivityCounterType)
        {
            UInt32 ret = 0;
            AcivtityCounterRes acivtityCounterRes = null;
            int actId = ActivityDataManager.GetInstance().GetActiveIdFromType(eActivityType);
            if (eActivityCounterType == ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_DAILY_SUBMIT_TASK ||
                eActivityCounterType == ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_TOTAL_SUBMIT_TASK ||
                eActivityCounterType == ActivityLimitTimeFactory.EActivityCounterType.OAT_SUMMER_WEEKLY_VACATION ||
                 eActivityCounterType == ActivityLimitTimeFactory.EActivityCounterType.QAT_SUMMER_DAILY_CHARGE)
            {
                if (mLimitTimeActivityCounterResDic.TryGetValue((UInt32)actId, out acivtityCounterRes))
                {
                    if (acivtityCounterRes.CounterId == (UInt32)eActivityCounterType)
                    {
                        ret = acivtityCounterRes.CounterValue;
                    }
                }
            }else if(eActivityCounterType == ActivityLimitTimeFactory.EActivityCounterType.OP_ITEM_ACTIVITY_BEHAVIOR_TOTAL_SUBMIT_TASK)
            {
                if (mBossActivityCounterResDic.TryGetValue((UInt32)actId, out acivtityCounterRes))
                {
                    if (acivtityCounterRes.CounterId == (UInt32)eActivityCounterType)
                    {
                        ret = acivtityCounterRes.CounterValue;
                    }
                }
            }
             return ret;
        }

        /// <summary>
        /// 得到账的次数 地下城相关
        /// </summary>
        /// <returns></returns>
        public UInt32 GetActivityConunter(int dungeonSubType)
        {
            UInt32 ret = 0;
            AcivtityCounterRes acivtityCounterRes = null;
            if (mDungeonCounterResDic.TryGetValue((UInt32)dungeonSubType, out acivtityCounterRes))
            {
                if (acivtityCounterRes.CounterId == (UInt32)ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_DAILY_DUNGEON_COUNT)
                {
                    ret = acivtityCounterRes.CounterValue;
                }
            }
            return ret;
        }

        /// <summary>
        /// 得到账号的次数 任务相关
        /// </summary>
        /// <returns></returns>
        public UInt32 GetActivityConunter(uint taskID, ActivityLimitTimeFactory.EActivityCounterType eActivityCounterType)
        {
            UInt32 ret = 0;
            AcivtityCounterRes acivtityCounterRes = null;
            int actId = (int)taskID;
            if (eActivityCounterType == ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_DAILY_SUBMIT_TASK ||
               eActivityCounterType == ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_TOTAL_SUBMIT_TASK ||
               eActivityCounterType == ActivityLimitTimeFactory.EActivityCounterType.OAT_SUMMER_WEEKLY_VACATION ||
                eActivityCounterType == ActivityLimitTimeFactory.EActivityCounterType.QAT_SUMMER_DAILY_CHARGE ||
                eActivityCounterType == ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_WEEKLY_SUBMIT_TASK)
            {
                if (mLimitTimeActivityCounterResDic.TryGetValue((UInt32)actId, out acivtityCounterRes))
                {
                    if (acivtityCounterRes.CounterId == (UInt32)eActivityCounterType)
                    {
                        ret = acivtityCounterRes.CounterValue;
                    }
                }
            } else if (eActivityCounterType == ActivityLimitTimeFactory.EActivityCounterType.OP_ITEM_ACTIVITY_BEHAVIOR_TOTAL_SUBMIT_TASK)
            {
                if (mBossActivityCounterResDic.TryGetValue((UInt32)actId, out acivtityCounterRes))
                {
                    if (acivtityCounterRes.CounterId == (UInt32)eActivityCounterType)
                    {
                        ret = acivtityCounterRes.CounterValue;
                    }
                }
            }
            return ret;

        }

        /// <summary>
        /// 是否显示折扣活动相关内容
        /// </summary>
        /// <returns></returns>
        public bool IsShowFirstDiscountDes(uint mallItemId)
        {
            var firstDiscountActivityData = ActivityDataManager.GetInstance().GetActiveDataFromType(ActivityLimitTimeFactory.EActivityType.OAT_LIMITTIMEFIRSTDISCOUNTACTIIVITY);
            if (firstDiscountActivityData != null && firstDiscountActivityData.state == (int)ActivityState.Start)//活动开始了
            {

                bool isHaveBuy = AccountShopDataManager.GetInstance().GetAccountSpecialItemNum(AccountCounterType.ACC_NEW_SERVER_GIFT_DISCOUNT) > 0;
                if(isHaveBuy)
                {
                    return false;
                }
                if (firstDiscountActivityData.parm2!=null)
                {
                    uint[] canDisocountMallItems = firstDiscountActivityData.parm2;//可以打折的商品id
                    for (int i = 0; i < canDisocountMallItems.Length; i++)
                    {
                        if(canDisocountMallItems[i]==mallItemId)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        //通过限时活动id移除活动任务数据
        private void _RemoveLimitTimeTasksByActivityId(uint activityId)
        {
            if (mLimitTimeActivityDatas.ContainsKey(activityId))
            {
                for (int i = 0; i < mLimitTimeActivityDatas[activityId].tasks.Length; ++i)
                {
                    mLimitTimeTaskDatas.Remove(mLimitTimeActivityDatas[activityId].tasks[i].dataid);
                }
            }
        }

        //限时活动通过任务id查找活动id
        public int _GetLimitTimeActivityIdByTaskId(uint taskId)
        {
            List<uint> activityIdList = new List<uint>(mLimitTimeActivityDatas.Keys);
            for (int i = 0; i < activityIdList.Count; ++i)
            {
                var activity = mLimitTimeActivityDatas[activityIdList[i]];
                for (int j = 0; j < activity.tasks.Length; ++j)
                {
                    if (activity.tasks[j].dataid == taskId)
                    {
                        return (int)activity.dataId;
                    }
                }
            }

            return 0;
        }

        #endregion

        #region Boss活动相关

        public void SendSubmitBossExchangeTask(int taskId)
        {
            SceneActiveTaskSubmit kSend = new SceneActiveTaskSubmit();
            kSend.taskId = (uint)taskId;
            kSend.param1 = 0;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);
        }

        /// <summary>
        /// 请求前往击杀boss页签下的数据
        /// </summary>
        public void RequestBossKillData(int activityId)
        {
            WorldActivityMonsterReq req = new WorldActivityMonsterReq();

            var activeMainTableData = TableManager.GetInstance().GetTableItem<ActiveMainTable>(activityId);
            if (activeMainTableData == null)
            {
                return;
            }
            string[] BossId = activeMainTableData.BossId.Split(',');
            uint[] BossActivityId = new uint[BossId.Length];
            for (int i = 0; i < BossId.Length; i++)
            {
                uint result = 0;
                uint.TryParse(BossId[i], out result);
                BossActivityId[i] = result;
            }
            req.ids = BossActivityId;
            req.activityId = (uint)activityId;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        //登录时同步Boss活动数据
        void _OnSyncBossActivities(MsgDATA data)
        {
            SceneSyncClientActivities activities = new SceneSyncClientActivities();
            activities.decode(data.bytes);

            //Logger.LogProcessFormat("OnRecvWorldSyncClientActivities {0}", ObjectDumper.Dump(activities));
            for (int i = 0; i < activities.activities.Length; ++i)
            {
                var activeMainTableData = TableManager.GetInstance().GetTableItem<ActiveMainTable>((int)activities.activities[i].id);
                if (activeMainTableData == null)
                {
                    continue;
                }

                if (activeMainTableData.ActivityType == ActiveMainTable.eActivityType.ExchangeActivity || activeMainTableData.ActivityType == ActiveMainTable.eActivityType.KillBossActivity || activeMainTableData.ActivityType == ActiveMainTable.eActivityType.QuestActivity)
                {

                    //如果是boss击杀活动，则请求怪物数据
                    if (activeMainTableData.ActivityType == ActiveMainTable.eActivityType.KillBossActivity)
                    {
                        RequestBossKillData((int)activities.activities[i].id);
                    }

                    if (!mBossActivityDatas.ContainsKey(activities.activities[i].id))
                    {
                        this.mBossActivityDatas.Add(activities.activities[i].id, activities.activities[i]);
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeDataUpdate, activities.activities[i].id);
                    }

                }
            }
        }

        //返回活动怪物信息
        void _OnRecvBossActivityMonsterInfo(MsgDATA data)
        {
            WorldActivityMonsterRes kRecv = new WorldActivityMonsterRes();
            kRecv.decode(data.bytes);
            if (kRecv.monsters == null || kRecv.monsters.Length <= 0)
            {
                return;
            }

            uint activityId = kRecv.activityId;
            if (activityId > 0)
            {
                mBossKillMonsterDatas[activityId] = kRecv;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeDataUpdate, activityId);
            }
        }

        //Boss兑换活动任务状态更新的时候同步
        void _OnRecvBossActivityTaskStateChange(MsgDATA data)
        {
            SceneNotifyActiveTaskStatus kRecv = new SceneNotifyActiveTaskStatus();
            kRecv.decode(data.bytes);

            mBossTaskStatusDatas[kRecv.taskId] = kRecv;

            //发送活动任务更新消息
            LimitTimeActivityTaskUpdateData updateData = new LimitTimeActivityTaskUpdateData(_GetBossExchangeActivityIdByTaskId((int)kRecv.taskId), (int)kRecv.taskId);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeTaskDataUpdate, updateData);
        }

        //剩余兑换次数发生改变时候刷新
        void _OnSyncBossActivityTaskData(MsgDATA data)
        {
            SceneNotifyActiveTaskVar kRecv = new SceneNotifyActiveTaskVar();
            kRecv.decode(data.bytes);

            mBossTaskDatas[kRecv.taskId] = kRecv;
            LimitTimeActivityTaskUpdateData updateData = new LimitTimeActivityTaskUpdateData(_GetBossExchangeActivityIdByTaskId((int)kRecv.taskId), (int)kRecv.taskId);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeTaskDataUpdate, updateData);
        }

        //服务器返回boss活动改变的协议
        void _OnRecvBossActivityStateChange(MsgDATA data)
        {
            WorldNotifyClientActivity kRecv = new WorldNotifyClientActivity();
            kRecv.decode(data.bytes);
            var activeMainTableData = TableManager.GetInstance().GetTableItem<ActiveMainTable>((int)kRecv.id);
            if (activeMainTableData == null)
            {
                return;
            }

            if (activeMainTableData.ActivityType == ActiveMainTable.eActivityType.ExchangeActivity || activeMainTableData.ActivityType == ActiveMainTable.eActivityType.KillBossActivity || activeMainTableData.ActivityType == ActiveMainTable.eActivityType.QuestActivity)
            {
                ActivityInfo activityInfo = new ActivityInfo
                {
                    state = kRecv.type,
                    id = kRecv.id,
                    level = kRecv.level,
                    name = kRecv.name,
                    preTime = kRecv.preTime,
                    startTime = kRecv.startTime,
                    dueTime = kRecv.dueTime
                };
                mBossActivityDatas[kRecv.id] = activityInfo;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeDataUpdate, kRecv.id);

                if (kRecv.type == (byte)StateType.End)//活动结束
                {
                    mBossActivityDatas.Remove(kRecv.id);
                    //移除所有任务数据
                    if (activeMainTableData.ActivityType == ActiveMainTable.eActivityType.ExchangeActivity || activeMainTableData.ActivityType == ActiveMainTable.eActivityType.QuestActivity || activeMainTableData.ActivityType == ActiveMainTable.eActivityType.QuestActivity)
                    {
                        _RemoveBossExchangeTasksByActivityId(kRecv.id);
                    }
                    else
                    {
                        _RemoveBossKillTasksByActivityId(kRecv.id);
                    }
                }
            }
        }
#if TEST_DELETE_ACTIVITY
        public void TestDeleteBossActivity()
        {
            ActivityInfo activityInfo1 = new ActivityInfo
            {
                state = 0,
                id = 18300,
                level = 1,
                name = "18300",
                preTime = 0,
                startTime = 0,
                dueTime = 0
            };

            ActivityInfo activityInfo2 = new ActivityInfo
            {
                state = 0,
                id = 19300,
                level = 1,
                name = "19300",
                preTime = 0,
                startTime = 0,
                dueTime = 0
            };
            mBossActivityDatas[18300] = activityInfo1;
            mBossActivityDatas[19300] = activityInfo2;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeDataUpdate, (uint)18300);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeDataUpdate, (uint)19300);
        }

        public void TestDeleteSummaryActivities()
        {
            TestDeleteGifPackActivity();
            TestDeleteDeleteWaterMelon();
            TestDeleteBossActivity();
        }

        void TestDeleteDeleteWaterMelon()
        {
            mLimitTimeActivityDatas[1090].state = (byte) OpActivityState.OAS_END;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeDataUpdate, (uint)1090);
        }
#endif
        //移除所有Boss兑换活动任务数据
        void _RemoveBossExchangeTasksByActivityId(uint activityId)
        {
            List<uint> taskIdList = new List<uint>(mBossTaskDatas.Keys);
            for (int i = 0; i < taskIdList.Count; ++i)
            {
                if (_GetBossExchangeActivityIdByTaskId((int)taskIdList[i]) == activityId)
                {
                    mBossTaskDatas.Remove(taskIdList[i]);
                }
            }

            List<uint> taskStatusList = new List<uint>(mBossTaskStatusDatas.Keys);
            for (int i = 0; i < taskStatusList.Count; ++i)
            {
                if (_GetBossExchangeActivityIdByTaskId((int)taskStatusList[i]) == activityId)
                {
                    mBossTaskStatusDatas.Remove(taskStatusList[i]);
                }
            }
        }

        //移除所有Boss击杀活动任务数据
        void _RemoveBossKillTasksByActivityId(uint activityId)
        {
            List<uint> taskIdList = new List<uint>(mBossKillMonsterDatas.Keys);
            for (int i = 0; i < taskIdList.Count; ++i)
            {
                if (_GetBossKillActivityIdByTaskId((uint)taskIdList[i]) == activityId)
                {
                    mBossKillMonsterDatas.Remove(taskIdList[i]);
                }
            }
        }

        /// <summary>
        /// 通过任务id来找到对应的boss兑换活动的id
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        int _GetBossExchangeActivityIdByTaskId(int taskId)
        {
            var activeTableAllData = TableManager.GetInstance().GetTable<ActiveTable>();
            var enumerator = activeTableAllData.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var activeTableData = enumerator.Current.Value as ActiveTable;
                if (activeTableData.ID == taskId)
                {
                    return activeTableData.TemplateID;
                }
            }
            return 0;
        }

        /// <summary>
        /// 通过ActivityMonsterInfo里的id来查找对应的Boss击杀活动的id
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        uint _GetBossKillActivityIdByTaskId(uint taskId)
        {
            foreach (var data in mBossKillMonsterDatas)
            {
                for (int i = 0; i < data.Value.monsters.Length; ++i)
                {
                    if (taskId == data.Value.monsters[i].id)
                    {
                        return data.Key;
                    }
                }
            }
            return 0;
        }
        /// <summary>
        /// 请求Boss账号数据
        /// </summary>
        /// <param name="data"></param>
        public void OnRequsetBossAccountData(BossExchangeTaskModel data)
        {
            if (data.AccountTotalSubmitLimit > 0)
            {
                ActivityDataManager.GetInstance().SendSceneOpActivityGetCounterReq((int)data.Id, ActivityLimitTimeFactory.EActivityCounterType.OP_ITEM_ACTIVITY_BEHAVIOR_TOTAL_SUBMIT_TASK);
            }

        }
        /// <summary>
        ///请求活动账号请求的回调
        /// </summary>
        public  void RegisterBossAccountData(ClientEventSystem.UIEventHandler eventHandler)
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivtiyLimitTimeAccounterNumUpdate, eventHandler);
        }
        ///注销活动账号请求的回调
        /// </summary>
        public  void UnRegisterBossAccountData(ClientEventSystem.UIEventHandler eventHandler)
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivtiyLimitTimeAccounterNumUpdate, eventHandler);
        }
        #endregion
        #region 终极试炼
        public class UltimateChallengeFloorData
        {
            public int floor = 0;
            public int tableID = 0;
        }
        public int GetUltimateChallengeHPPercent()
        {
            const int max = 100000;
            int count = CountDataManager.GetInstance().GetCount("zjsl_player_hp");
            if(count == 0 || count > max)
            {
                count = max;
            }

            float value = (float)count / 1000.0f;
            if(value < 1.0f)
            {
                value = 1.0f;
            }

            return (int)value;
        }
        public int GetUltimateChallengeMPPercent()
        {
            int count = CountDataManager.GetInstance().GetCount("zjsl_player_mp");
            if (count == 0 || count > 1000)
            {
                count = 1000;
            }

            float value = (float)count / 10.0f;
            if(value < 1.0f)
            {
                value = 1.0f;
            }

            return (int)value;
        }
        public int GetUltimateChallengeMaxFloorRecord()
        {
            int count = CountDataManager.GetInstance().GetCount("zjsl_top_floor_total");           
            return count;  
        }
        public int GetUltimateChallengeLeftEnterCount()
        {
            int count = CountDataManager.GetInstance().GetCount("zjsl_dungeon_times");
            return GetUltimateChallengeMaxEnterCount() - count;
        }
        public int GetUltimateChallengeMaxEnterCount()
        {
            return Utility.GetSystemValueFromTable(SystemValueTable.eType2.SVT_ZJSL_DUNGEON_TIMES_DAILY);
        }
        public int GetUltimateChallengeLeftCount()
        {
            int count = CountDataManager.GetInstance().GetCount("zjsl_challenge_times");
            return GetUltimateChallengeMaxCount() - count;
        }
        public int GetUltimateChallengeMaxCount()
        {
            return 1;
        }
        public int GetUltimateChallengeDungeonBufID()
        {
            int id = CountDataManager.GetInstance().GetCount("zjsl_dungeon_buff");
            return id;
        }
        public int GetUltimateChallengeDungeonBufLv()
        {
            return 1;
        }
        public int GetUltimateChallengeInspireBufID()
        {
            int id = CountDataManager.GetInstance().GetCount("zjsl_inspire_buff");
            return id;
        }
        public bool HasPopUpGetInspireBufFrame { get; set; }
        public int GetUltimateChallengeInspireBufLv()
        {
            return 1;
        }
        public int GetUltimateChallengeTodayStartFloor()
        {
            int count = CountDataManager.GetInstance().GetCount("zjsl_top_floor") + 1;
            return count;
        }

        // 关卡效果生效起始层数
        public int GetUltimateChallengeDungeonBufBeginFloor()
        {
            int floor = CountDataManager.GetInstance().GetCount("zjsl_dungeon_buff_floor");
            return floor;    
        }

        // 关卡效果持续层数
        public int GetUltimateChallengeDungeonBufDurationFloor(int bufID)
        {
            Dictionary<int, object> dicts = TableManager.instance.GetTable<UltimateChallengeBuffTable>();
            if (dicts != null)
            {
                var iter = dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    UltimateChallengeBuffTable adt = iter.Current.Value as UltimateChallengeBuffTable;
                    if (adt == null)
                    {
                        continue;
                    }
                    if(adt.buffID == bufID)
                    {
                        return adt.sustain;
                    }
                }
            }
            return 0;
        }

        // 每日开放层数
        public int GetUltimateChallengeDungeonDailyOpenFloor()
        {
            return Utility.GetSystemValueFromTable(SystemValueTable.eType2.SVT_ZJSL_TOWER_FLOOR_OPEN_DAILY);
        }
        public bool NotPopUpRefreshBufMsgBox { get; set; }
        void _OnSceneDungeonZjslRefreshBuffRes(MsgDATA msg)
        {
            SceneDungeonZjslRefreshBuffRes res = new SceneDungeonZjslRefreshBuffRes();
            res.decode(msg.bytes);
            if (res.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.result);
                return;
            }
            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("refresh_buf_success"));

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RefreshDungeonBufSuccess);
        }
        public void SendSceneDungeonZjslRefreshBuffReq(int dungeonID,bool useItem = true)
        {
            SceneDungeonZjslRefreshBuffReq kSend = new SceneDungeonZjslRefreshBuffReq();
            kSend.dungeonId = (uint)dungeonID;
            kSend.useItem = useItem ? (uint)1 : 0;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);
        }
        #endregion
        #region 月签到
        public class MonthlySignInItemData
        {
            public AwardItemData awardItemData = new AwardItemData();
            public bool signIned = false;
            public int day = 1;
        }
        public class AccumulativeSignInItemData
        {
            public AwardItemData awardItemData = new AwardItemData();
            public bool hasGotAward = false;
            public int accumulativeDay = 3;
        }
        public class MonthlySignInCountInfo
        {
            public byte noFree;
            public byte free;
            public byte activite;
            public byte accumulatedActivite; // 通过活跃度获得的累计补签次数
        }
        List<MonthlySignInItemData> monthlySignInItemDatas = new List<MonthlySignInItemData>();
        List<AccumulativeSignInItemData> accumulativeSignInItemDatas = new List<AccumulativeSignInItemData>();
        MonthlySignInCountInfo monthlySignInCountInfo = new MonthlySignInCountInfo();
        public List<MonthlySignInItemData> GetMonthlySignInItemDatas()
        {
            return monthlySignInItemDatas;
        }
        public List<AccumulativeSignInItemData> GetAccumulativeSignInItemDatas()
        {
            return accumulativeSignInItemDatas;
        }
        public string GetVipAddUpText(int month, int day)
        {
            if (monthSignInAwards == null)
            {
                return "";
            }
            if (!monthSignInAwards.ContainsKey(month))
            {
                return "";
            }
            Dictionary<int, MonthSignAward> dic = monthSignInAwards[month];
            if (dic == null)
            {
                return "";
            }
            if (!dic.ContainsKey(day))
            {
                return "";
            }
            MonthSignAward monthSignAward = dic[day];
            if (monthSignAward == null)
            {
                return "";
            }
            if (monthSignAward.VIPDouble > Utility.GetSystemValueFromTable(SystemValueTable.eType.SVT_VIPLEVEL_MAX))
            {
                return "";
            }
            return TR.Value("vip_double", monthSignAward.VIPDouble);
        }
        public AwardItemData GetAwardItemData(int month, int day)
        {
            if (monthSignInAwards == null)
            {
                return null;
            }
            if (!monthSignInAwards.ContainsKey(month))
            {
                return null;
            }
            Dictionary<int, MonthSignAward> dic = monthSignInAwards[month];
            if (dic == null)
            {
                return null;
            }
            if (!dic.ContainsKey(day))
            {
                return null;
            }
            MonthSignAward monthSignAward = dic[day];
            if (monthSignAward == null)
            {
                return null;
            }
            AwardItemData awardItemData = new AwardItemData();
            if (awardItemData == null)
            {
                return null;
            }
            awardItemData.ID = monthSignAward.ItemID;
            awardItemData.Num = monthSignAward.ItemNum;
            return awardItemData;
        }
        public int GetHasSignInCount()
        {
            int count = 0;
            if (monthlySignInItemDatas != null)
            {
                for (int i = 0; i < monthlySignInItemDatas.Count; i++)
                {
                    MonthlySignInItemData monthlySignInItemData = monthlySignInItemDatas[i];
                    if (monthlySignInItemData == null)
                    {
                        continue;
                    }
                    if (monthlySignInItemData.signIned)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        public bool IsShowSignInRedPoint()
        {
            var isShowRedPoint = ActivityDataManager.GetInstance().CanSignInToday() || ActivityDataManager.GetInstance().CanFillCheckWithFreeCount() || ActivityDataManager.GetInstance().CanGetSignCollectAward();
            return isShowRedPoint;
        }

        // 今天可以签到
        public bool CanSignInToday()
        {
            if (monthlySignInItemDatas == null)
            {
                return false;
            }
            DateTime dtNow = Function.ConvertIntDateTime(TimeManager.GetInstance().GetServerDoubleTime());
            if(dtNow.Hour < 6)
            {
                dtNow = dtNow.AddDays(-1);
            }
            for (int i = 0; i < monthlySignInItemDatas.Count; i++)
            {
                MonthlySignInItemData monthlySignInItemData = monthlySignInItemDatas[i];
                if (monthlySignInItemData == null)
                {
                    continue;
                }
                if (monthlySignInItemData.day == dtNow.Day)
                {
                    return !monthlySignInItemData.signIned;
                }
            }
            return false;
        }
        public int GetFillCheckCount()
        {
            if (monthlySignInCountInfo == null)
            {
                return 0;
            }
            return monthlySignInCountInfo.noFree + monthlySignInCountInfo.free + monthlySignInCountInfo.activite;
        }

        // 可以免费补签
        public bool CanFillCheckWithFreeCount()
        {
            if (monthlySignInCountInfo == null)
            {
                return false;
            }
            if (monthlySignInCountInfo.free + monthlySignInCountInfo.activite == 0)
            {
                return false;
            }
            if (monthlySignInItemDatas == null)
            {
                return false;
            }
            int count = 0;
            DateTime dtNow = Function.ConvertIntDateTime(TimeManager.GetInstance().GetServerDoubleTime());
            if(dtNow.Hour < 6)
            {
                dtNow = dtNow.AddDays(-1);
            }
            for (int i = 0; i < monthlySignInItemDatas.Count; i++)
            {
                MonthlySignInItemData monthlySignInItemData = monthlySignInItemDatas[i];
                if (monthlySignInItemData == null)
                {
                    continue;
                }
                if (monthlySignInItemData.day >= dtNow.Day)
                {
                    break;
                }
                if (!monthlySignInItemData.signIned)
                {
                    count++;
                }
            }
            return count > 0;
        }

        // 可以领取累计签到奖励
        public bool CanGetSignCollectAward()
        {
            if (accumulativeSignInItemDatas == null)
            {
                return false;
            }
            int signInCount = ActivityDataManager.GetInstance().GetHasSignInCount();
            for (int i = 0; i < accumulativeSignInItemDatas.Count; i++)
            {
                ActivityDataManager.AccumulativeSignInItemData accumulativeSignInItemData = accumulativeSignInItemDatas[i];
                if (accumulativeSignInItemData == null)
                {
                    continue;
                }
                if (signInCount >= accumulativeSignInItemData.accumulativeDay && !accumulativeSignInItemData.hasGotAward)
                {
                    return true;
                }
            }
            return false;
        }
        public const int MONTH_SIGN_IN_CONFIG_ID = 9380; //活动模板表 ActivityTypeID
        public static int GetMonthDayNum(int year, int month)
        {
            int days = DateTime.DaysInMonth(year, month);
            return days;
        }
        public MonthlySignInCountInfo GetMonthlySignInCountInfo()
        {
            return monthlySignInCountInfo;
        }
        Dictionary<int, Dictionary<int, MonthSignAward>> monthSignInAwards = new Dictionary<int, Dictionary<int, MonthSignAward>>();
        Dictionary<int, MonthSignCollectAward> monthSignCollectAwards = new Dictionary<int, MonthSignCollectAward>();
        void _OnSceneNewSignInQueryRet(MsgDATA msg)
        {
            SceneNewSignInQueryRet res = new SceneNewSignInQueryRet();
            res.decode(msg.bytes);
            if (monthlySignInCountInfo != null)
            {
                monthlySignInCountInfo.noFree = res.noFree;
                monthlySignInCountInfo.free = res.free;
                monthlySignInCountInfo.activite = res.activite;
                monthlySignInCountInfo.accumulatedActivite = res.activiteCount;
            }
            if (monthlySignInItemDatas != null)
            {
                monthlySignInItemDatas.Clear();
                DateTime dateTime = Function.ConvertIntDateTime(TimeManager.GetInstance().GetServerDoubleTime());
                if(dateTime.Hour < 6)
                {
                    dateTime = dateTime.AddDays(-1);
                }
                int dayNum = GetMonthDayNum(dateTime.Year, dateTime.Month);
                BitArray ba = new BitArray(BitConverter.GetBytes(res.signFlag));
                for (int i = 0; i < ba.Length && i < dayNum; i++)
                {
                    MonthlySignInItemData monthlySignInItemData = new MonthlySignInItemData();
                    if (monthlySignInItemData != null)
                    {
                        monthlySignInItemData.day = i + 1;
                        monthlySignInItemData.signIned = ba.Get(i + 1); // 这里注意下 第0位不用，第一位表示第一天 第二位表示第二天 以此类推
                        monthlySignInItemData.awardItemData = GetAwardItemData(dateTime.Month, monthlySignInItemData.day);
                    }
                    monthlySignInItemDatas.Add(monthlySignInItemData);
                }
            }
            if (accumulativeSignInItemDatas != null)
            {
                accumulativeSignInItemDatas.Clear();
                DateTime dateTime = Function.ConvertIntDateTime(TimeManager.GetInstance().GetServerDoubleTime());
                int dayNum = GetMonthDayNum(dateTime.Year, dateTime.Month);
                BitArray ba = new BitArray(BitConverter.GetBytes(res.collectFlag));
                Dictionary<int, MonthSignCollectAward> dicts = monthSignCollectAwards;
                if (dicts != null)
                {
                    var iter = dicts.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        MonthSignCollectAward adt = iter.Current.Value as MonthSignCollectAward;
                        if (adt == null)
                        {
                            continue;
                        }
                        AccumulativeSignInItemData accumulativeSignInItemData = new AccumulativeSignInItemData();
                        if (accumulativeSignInItemData != null)
                        {
                            accumulativeSignInItemData.accumulativeDay = adt.ID;
                            accumulativeSignInItemData.hasGotAward = ba.Get(adt.ID);
                            accumulativeSignInItemData.awardItemData = new AwardItemData();
                            if (accumulativeSignInItemData.awardItemData != null)
                            {
                                accumulativeSignInItemData.awardItemData.ID = adt.ItemID;
                                accumulativeSignInItemData.awardItemData.Num = adt.ItemNum;
                            }
                        }
                        accumulativeSignInItemDatas.Add(accumulativeSignInItemData);
                    }
                }
            }

            ClientSystemManager.GetInstance().CloseFrame<SignFrame>();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUpdateMonthlySignInCountInfo);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUpdateMonthlySignInItemInfo);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUpdateAccumulativeSignInItemInfo);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMonthlySignInRedPointReset);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.WelfActivityRedPoint);
        }
        void _OnSceneNewSignRet(MsgDATA msg)
        {
            SceneNewSignRet res = new SceneNewSignRet();
            res.decode(msg.bytes);
            if (res.errorCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.errorCode);
                return;
            }
        }
        void _OnSceneNewSignInCollectRet(MsgDATA msg)
        {
            SceneNewSignInCollectRet res = new SceneNewSignInCollectRet();
            res.decode(msg.bytes);
            if (res.errorCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.errorCode);
                return;
            }
        }
        public void SendMonthlySignInQuery()
        {
            SceneNewSignInQuery kSend = new SceneNewSignInQuery();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);
        }
        public void SendMonthlySignIn(int day, bool isAll = false)
        {
            SceneNewSignIn kSend = new SceneNewSignIn();
            kSend.day = (byte)day;
            kSend.isAll = (byte)(isAll ? 1 : 0);
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);
        }
        public void SendGetAccumulativeSignInAward(int day)
        {
            SceneNewSignInCollect kSend = new SceneNewSignInCollect();
            kSend.day = (byte)day;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);
        }
        #endregion

        /// <summary>
        /// 申请挑战者积分
        /// </summary>
        public void RequestChallengeScore()
        {
            SceneChallengeScoreReq kSend = new SceneChallengeScoreReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);
            
        }
        /// <summary>
        /// 获取挑战者积分
        /// </summary>
        /// <param name="obj"></param>
        private void _OnChallengeScoreRet(MsgDATA msg)
        {
            SceneChallengeScoreRet res = new SceneChallengeScoreRet();
            res.decode(msg.bytes);
            PlayerBaseData.GetInstance().ChanllengeScore = (int)res.score;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUpdateIntegrationChallengeScore);
        }

        #region ArborDay

        private void BindArborDayNetMessages()
        {

            NetProcess.AddMsgHandler(SceneActivePlantRes.MsgID, OnReceiveSceneActivePlantRes);
            NetProcess.AddMsgHandler(SceneActivePlantAppraRes.MsgID, OnReceiveSceneActivePlantAppraRes);
        }

        private void UnBindArborDayNetMessages()
        {
            NetProcess.RemoveMsgHandler(SceneActivePlantRes.MsgID, OnReceiveSceneActivePlantRes);
            NetProcess.RemoveMsgHandler(SceneActivePlantAppraRes.MsgID, OnReceiveSceneActivePlantAppraRes);
        }

        //种植树木的歇息
        public void OnSendSceneActivePlantReq()
        {
            SceneActivePlantReq sceneActivePlantReq = new SceneActivePlantReq();

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, sceneActivePlantReq);
        }

        private void OnReceiveSceneActivePlantRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            SceneActivePlantRes sceneActivePlantRes = new SceneActivePlantRes();
            sceneActivePlantRes.decode(msgData.bytes);

            //植树错误码
            if (sceneActivePlantRes.retCode != (uint) ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int) sceneActivePlantRes.retCode);
            }
        }

        //鉴定树木
        public void OnSendSceneActivePlantAppraReq()
        {
            SceneActivePlantAppraReq plantAppraReq = new SceneActivePlantAppraReq();

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, plantAppraReq);
        }


        private void OnReceiveSceneActivePlantAppraRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            SceneActivePlantAppraRes plantAppraRes = new SceneActivePlantAppraRes();
            plantAppraRes.decode(msgData.bytes);

            if (plantAppraRes.retCode != (uint) ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int) plantAppraRes.retCode);
            }
        }
        #endregion


        #region 全民抢购活动

        public enum LimitTimeGroupBuyPreviewType
        {
            None = 0,
            MayDay,         //五一
            GoblinChamber,  //地精商会
            GoblinChamberNew //地精商会新活动
        }

        public class LimitTimeGroupBuyDiscountData
        {
            public int joinNum;
            public int discount;
        }

        /// <summary>
        /// 限时团购展示数据
        /// </summary>
        public class LimitTimeGroupBuyPreviewDataModel
        {
            public LimitTimeGroupBuyPreviewType type;
            public int itemId;
            public int goblinCoin;
            public int price;
        }

        public static Dictionary<LimitTimeGroupBuyPreviewType, List<LimitTimeGroupBuyPreviewDataModel>> mPreviewDict = new Dictionary<LimitTimeGroupBuyPreviewType, List<LimitTimeGroupBuyPreviewDataModel>>();

        public static List<LimitTimeGroupBuyDiscountData> mDiscountDataList = new List<LimitTimeGroupBuyDiscountData>();
        public static List<LimitTimeGroupBuyPreviewDataModel> GetLimitTimeGroupBuyPrevieDataList(LimitTimeGroupBuyPreviewType type)
        {
            List<LimitTimeGroupBuyPreviewDataModel> list = new List<LimitTimeGroupBuyPreviewDataModel>();

            if (mPreviewDict == null)
            {
                return list;
            }

            if (mPreviewDict.TryGetValue(type,out list))
            {
               
            }

            return list;
        }

        private void InitLimiteBargainShowTable()
        {
            if (mPreviewDict == null)
            {
                mPreviewDict = new Dictionary<LimitTimeGroupBuyPreviewType, List<LimitTimeGroupBuyPreviewDataModel>>();
            }

            mPreviewDict.Clear();

            var enumerator = TableManager.GetInstance().GetTable<LimiteBargainShowTable>().GetEnumerator();
            while (enumerator.MoveNext())
            {
                LimiteBargainShowTable table = enumerator.Current.Value as LimiteBargainShowTable;
                if (table == null) continue;

                if (!mPreviewDict.ContainsKey((LimitTimeGroupBuyPreviewType)table.ShowType))
                {
                    mPreviewDict.Add((LimitTimeGroupBuyPreviewType)table.ShowType, new List<LimitTimeGroupBuyPreviewDataModel>());
                }

                LimitTimeGroupBuyPreviewDataModel model = new LimitTimeGroupBuyPreviewDataModel();
                model.type = (LimitTimeGroupBuyPreviewType)table.ShowType;
                model.itemId = table.ShowItem;
                model.goblinCoin = table.GoblinCoins;
                model.price = table.Price;

                mPreviewDict[(LimitTimeGroupBuyPreviewType)table.ShowType].Add(model);
            }
        }

        private void InitWholeBargainDiscountTable()
        {
            if (mDiscountDataList == null)
            {
                mDiscountDataList = new List<LimitTimeGroupBuyDiscountData>();
            }

            mDiscountDataList.Clear();

            var enumerator = TableManager.GetInstance().GetTable<WholeBargainDiscountTable>().GetEnumerator();
            while (enumerator.MoveNext())
            {
                WholeBargainDiscountTable table = enumerator.Current.Value as WholeBargainDiscountTable;
                if (table == null)
                {
                    continue;
                }

                LimitTimeGroupBuyDiscountData data = new LimitTimeGroupBuyDiscountData();
                data.joinNum = table.JoinNum;
                data.discount = table.Discount;

                mDiscountDataList.Add(data);
            }
        }

        private void RegisterASWholeBargainNet()
        {
            NetProcess.AddMsgHandler(GASWholeBargainRes.MsgID, OnReceiveGASWholeBargainRes);
            NetProcess.AddMsgHandler(GASWholeBargainDiscountSync.MsgID, OnReceiveGASWholeBargainDiscountSync);
        }

        private void UnRegisterASWholeBargainNet()
        {
            NetProcess.RemoveMsgHandler(GASWholeBargainRes.MsgID, OnReceiveGASWholeBargainRes);
            NetProcess.RemoveMsgHandler(GASWholeBargainDiscountSync.MsgID, OnReceiveGASWholeBargainDiscountSync);
        }

        private void OnReceiveGASWholeBargainRes(MsgDATA msg)
        {
            GASWholeBargainRes res = new GASWholeBargainRes();
            res.decode(msg.bytes);

            LimitTimeGroupBuyDataModel model = new LimitTimeGroupBuyDataModel();
            model.joinNum = (int)res.joinNum;
            model.maxNum = (int)res.maxNum;
            model.playerJoinNum = (int)res.playerJoinNum;
            model.discount = (int)res.discount;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnGASWholeBargainResSuccessed, model);
        }

        private void OnReceiveGASWholeBargainDiscountSync(MsgDATA msg)
        {
            GASWholeBargainDiscountSync res = new GASWholeBargainDiscountSync();
            res.decode(msg.bytes);

            LimitTimeGroupBuyDiscount = res.discount == 0 ? 100 : res.discount;
        }

        /// <summary>
        /// 全民抢购数据请求
        /// </summary>
        public void OnSendGASWholeBargainReq()
        {
            GASWholeBargainReq req = new GASWholeBargainReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }


        /// <summary>
        /// 检查全民团购活动是否开启
        /// </summary>
        /// <returns></returns>
        public bool CheckGroupPurchaseActivityIsOpen()
        {
            foreach (var activity in mLimitTimeActivityDatas.Values)
            {
                if (activity.tmpType != (uint)OpActivityTmpType.OAT_WHOLE_BARGAIN_SHOP)
                {
                    continue;
                }
                
                if (activity.state != (int)OpActivityState.OAS_IN)
                {
                    continue;
                }
                
                return true;
            }

            return false;
        }
        #endregion

    }
}