using System.Collections.Generic;
using Network;
using Protocol;
using ProtoTable;


namespace GameClient
{


    //荣誉系统数据管理器
    public class HonorSystemDataManager : DataManager<HonorSystemDataManager>
    {

        public const int HonorSystemUnLockLevel = 30;           //荣誉系统解锁的等级
        public const int DefaultHonorSystemLevel = 1000;        //默认的荣誉系统等级（0级对应的数据）

        //三者是确定的
        public const int NormalHonorProtectCardId = 330000252;      //荣誉保障卡
        public const int HighHonorProtectCardId = 330000253;        //高级荣誉保障卡
        public const EPackageType ProtectCardItemPackageType = EPackageType.Material;       //保障卡的背包类型


        public uint PlayerHonorLevel;           //角色的荣誉等级
        public uint PlayerHonorExp;             //角色等级经验(总的经验)
        public uint PlayerLastWeekRank;         //角色上周排名
        public uint PlayerHistoryRank;          //角色历史排名(上上周排名)
        public uint PlayerHighestHonorLevel;    //角色最高荣誉等级

        public uint FinishTimeStamp;             //本周结算的时间戳
        public bool IsAlreadyUseProtectCard;     //是否已经使用使用了保障卡

        //角色的荣誉信息
        public List<PlayerHistoryHonorInfo> PlayerHistoryHonorInfoList = new List<PlayerHistoryHonorInfo>();

        public Dictionary<int, HonorPlayerTable> HonorPlayerDictionary = new Dictionary<int, HonorPlayerTable>();

        public int HonorHistoryActivityNumber = 4;          //历史活动第一次最多显示的数量

        public ulong HonorSystemInfoReqTimes = 0;             //请求次数
        public HistoryHonorInfo[] LastHonorSystemInfo = null;   //上次荣誉系统的信息

        public bool IsShowRedPointFlag = false;         //由服务器通知是否下发小红点

        //吃鸡和决斗场
        //荣誉最大值
        public int PkHonorExpMaxValue = 0;
        //本周的counter字符串
        public string PkHonorExpCounterStr = "pk_season_1v1_honor";     //本周当前数值的Counter字段

        public int ChiJiHonorExpMaxValue = 0;
        public string ChiJiHonorExpCounterStr = "chi_ji_honor";

        #region Initialize
        public override void Initialize()
        {
            BindNetEvents();
        }

        public override void Clear()
        {
            UnBindNetEvents();
            ClearData();
        }

        private void ClearData()
        {
            ResetPlayerHistoryHonorInfo();
            HonorSystemInfoReqTimes = 0;
            LastHonorSystemInfo = null;
            IsShowRedPointFlag = false;

            FinishTimeStamp = 0;
            IsAlreadyUseProtectCard = false;

            ChiJiHonorExpMaxValue = 0;
            PkHonorExpMaxValue = 0;
        }

        private void ResetPlayerHistoryHonorInfo()
        {
            PlayerHonorExp = 0;
            PlayerHonorLevel = 0;
            PlayerLastWeekRank = 0;
            PlayerHistoryRank = 0;
            PlayerHighestHonorLevel = 0;

            if (PlayerHistoryHonorInfoList != null && PlayerHistoryHonorInfoList.Count > 0)
            {
                for (var i = 0; i < PlayerHistoryHonorInfoList.Count; i++)
                {
                    var playerHistoryHonorInfo = PlayerHistoryHonorInfoList[i];
                    if (playerHistoryHonorInfo != null
                        && playerHistoryHonorInfo.PvpNumberStatisticsList != null)
                    {
                        playerHistoryHonorInfo.PvpNumberStatisticsList.Clear();
                    }
                }
                PlayerHistoryHonorInfoList.Clear();
            }

            HonorPlayerDictionary.Clear();
        }

        private void BindNetEvents()
        {
            NetProcess.AddMsgHandler(SceneHonorRes.MsgID, OnReceiveSceneHonorRes);
            NetProcess.AddMsgHandler(SceneHonorRedPoint.MsgID, OnReceiveSceneHonorRedPoint);
        }

        private void UnBindNetEvents()
        {
            NetProcess.RemoveMsgHandler(SceneHonorRes.MsgID, OnReceiveSceneHonorRes);
            NetProcess.RemoveMsgHandler(SceneHonorRedPoint.MsgID, OnReceiveSceneHonorRedPoint);
        }

        #endregion

        //发送荣誉请求
        public void OnSendSceneHonorReq()
        {
            SceneHonorReq sceneHonorReq = new SceneHonorReq();

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, sceneHonorReq);

            HonorSystemInfoReqTimes += 1;
        }

        private void OnReceiveSceneHonorRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            ResetPlayerHistoryHonorInfo();

            SceneHonorRes sceneHonorRes = new SceneHonorRes();
            sceneHonorRes.decode(msgData.bytes);

            //角色的荣誉数据
            PlayerHonorLevel = sceneHonorRes.honorLvl;
            PlayerHonorExp = sceneHonorRes.honorExp;
            PlayerLastWeekRank = sceneHonorRes.lastWeekRank;
            PlayerHistoryRank = sceneHonorRes.historyRank;
            PlayerHighestHonorLevel = sceneHonorRes.highestHonorLvl;

            FinishTimeStamp = sceneHonorRes.rankTime;
            IsAlreadyUseProtectCard = sceneHonorRes.isUseCard == 1 ? true : false;

            //客户端没有主动请求过，则表示是在角色登录的时候，服务器直接同步过来
            if (HonorSystemInfoReqTimes == 0)
            {
                LastHonorSystemInfo = sceneHonorRes.historyHonorInfoList;
                return;
            }

            //客户端主动请求过协议

            //今日和昨日
            int todayDay = TimeUtility.GetTodayTimeInWeekDay();
            int yesterdayDay = TimeUtility.GetYesterdayTimeInWeekDay();

            //创建荣誉的数据模型
            var todayPlayerHistoryHonorInfo =
                HonorSystemUtility.CreatePlayerHistoryHonorInfoByDateType(HONOR_DATE_TYPE.HONOR_DATE_TYPE_TODAY);
            var yesterdayPlayerHistoryHonorInfo 
                = HonorSystemUtility.CreatePlayerHistoryHonorInfoByDateType(HONOR_DATE_TYPE.HONOR_DATE_TYPE_YESTERDAY);
            var totalPlayerHistoryHonorInfo =
                HonorSystemUtility.CreatePlayerHistoryHonorInfoByDateType(HONOR_DATE_TYPE.HONOR_DATE_TYPE_TOTAL);
            var thisWeekPlayerHistoryHonorInfo =
                HonorSystemUtility.CreatePlayerHistoryHonorInfoByDateType(HONOR_DATE_TYPE.HONOR_DATE_TYPE_THIS_WEEK);
            var lastWeekPlayerHistoryHonorInfo =
                HonorSystemUtility.CreatePlayerHistoryHonorInfoByDateType(HONOR_DATE_TYPE.HONOR_DATE_TYPE_LAST_WEEK);

            //遍历表格数据，添加不同类型的荣誉数据
            var honorPlayerTables = TableManager.GetInstance().GetTable<HonorPlayerTable>();
            if (honorPlayerTables != null)
            {
                var iter = honorPlayerTables.GetEnumerator();
                while (iter.MoveNext())
                {
                    var honorPlayerTable = iter.Current.Value as HonorPlayerTable;
                    if(honorPlayerTable == null)
                        continue;
                    //不参与展示
                    if(honorPlayerTable.Sort == 0)
                        continue;
                    
                    //今日展示
                    if (HonorSystemUtility.IsPvpShowInWeekDay(todayDay, honorPlayerTable) == true)
                        HonorSystemUtility.AddPvpNumberStatisticsInPlayerHistoryHonorInfo(todayPlayerHistoryHonorInfo,
                            honorPlayerTable);

                    //昨日展示
                    if (HonorSystemUtility.IsPvpShowInWeekDay(yesterdayDay, honorPlayerTable) == true)
                        HonorSystemUtility.AddPvpNumberStatisticsInPlayerHistoryHonorInfo(yesterdayPlayerHistoryHonorInfo,
                            honorPlayerTable);

                    //总计，这周，上周
                    HonorSystemUtility.AddPvpNumberStatisticsInPlayerHistoryHonorInfo(totalPlayerHistoryHonorInfo,
                        honorPlayerTable);
                    HonorSystemUtility.AddPvpNumberStatisticsInPlayerHistoryHonorInfo(thisWeekPlayerHistoryHonorInfo,
                        honorPlayerTable);
                    HonorSystemUtility.AddPvpNumberStatisticsInPlayerHistoryHonorInfo(lastWeekPlayerHistoryHonorInfo,
                        honorPlayerTable);
                }
            }

            //将服务器同步的数据进行更新（荣誉总计和活动次数）
            if (sceneHonorRes.historyHonorInfoList != null && sceneHonorRes.historyHonorInfoList.Length > 0)
            {
                for (var i = 0; i < sceneHonorRes.historyHonorInfoList.Length; i++)
                {
                    var historyHonorInfo = sceneHonorRes.historyHonorInfoList[i];
                    if(historyHonorInfo == null)
                        continue;

                    //同步到对应的数据模型中
                    switch ((HONOR_DATE_TYPE) historyHonorInfo.dateType)
                    {
                        case HONOR_DATE_TYPE.HONOR_DATE_TYPE_TODAY:
                            HonorSystemUtility.UpdatePlayerHistoryInfo(todayPlayerHistoryHonorInfo,
                                historyHonorInfo);
                            break;
                        case HONOR_DATE_TYPE.HONOR_DATE_TYPE_YESTERDAY:
                            HonorSystemUtility.UpdatePlayerHistoryInfo(yesterdayPlayerHistoryHonorInfo,
                                historyHonorInfo);
                            break;
                        case HONOR_DATE_TYPE.HONOR_DATE_TYPE_TOTAL:
                            HonorSystemUtility.UpdatePlayerHistoryInfo(totalPlayerHistoryHonorInfo,
                                historyHonorInfo);
                            break;
                        case HONOR_DATE_TYPE.HONOR_DATE_TYPE_THIS_WEEK:
                            HonorSystemUtility.UpdatePlayerHistoryInfo(thisWeekPlayerHistoryHonorInfo,
                                historyHonorInfo);
                            break;
                        case HONOR_DATE_TYPE.HONOR_DATE_TYPE_LAST_WEEK:
                            HonorSystemUtility.UpdatePlayerHistoryInfo(lastWeekPlayerHistoryHonorInfo,
                                historyHonorInfo);
                            break;
                        default:
                            break;
                    }
                }
            }

            //更新总计数据的新标志
            HistoryHonorInfo lastTotalHonorInfo = null;
            if (LastHonorSystemInfo != null && LastHonorSystemInfo.Length > 0)
            {
                for (var i = 0; i < LastHonorSystemInfo.Length; i++)
                {
                    var curHistoryHonorInfo = LastHonorSystemInfo[i];
                    if(curHistoryHonorInfo == null)
                        continue;

                    if ((HONOR_DATE_TYPE) curHistoryHonorInfo.dateType
                        == HONOR_DATE_TYPE.HONOR_DATE_TYPE_TOTAL)
                    {
                        lastTotalHonorInfo = curHistoryHonorInfo;
                        break;
                    }
                }
            }
            HonorSystemUtility.UpdatePlayerNewFlagInHistoryInfo(totalPlayerHistoryHonorInfo,
                lastTotalHonorInfo);


            //不同日期类型的荣誉数据，保存并排序
            if (PlayerHistoryHonorInfoList == null)
                PlayerHistoryHonorInfoList = new List<PlayerHistoryHonorInfo>();
            PlayerHistoryHonorInfoList.Clear();

            PlayerHistoryHonorInfoList.Add(todayPlayerHistoryHonorInfo);
            PlayerHistoryHonorInfoList.Add(yesterdayPlayerHistoryHonorInfo);
            PlayerHistoryHonorInfoList.Add(totalPlayerHistoryHonorInfo);
            PlayerHistoryHonorInfoList.Add(thisWeekPlayerHistoryHonorInfo);
            PlayerHistoryHonorInfoList.Add(lastWeekPlayerHistoryHonorInfo);

            for (var i = 0; i < PlayerHistoryHonorInfoList.Count; i++)
            {
                var playerHistoryHonorInfo = PlayerHistoryHonorInfoList[i];
                if(playerHistoryHonorInfo == null)
                    continue;

                if (playerHistoryHonorInfo.PvpNumberStatisticsList == null
                    || playerHistoryHonorInfo.PvpNumberStatisticsList.Count <= 1)
                    continue;

                playerHistoryHonorInfo.PvpNumberStatisticsList.Sort((x, y) =>
                    x.PvpSort.CompareTo(y.PvpSort));
            }

            //保存本次的消息数据，
            LastHonorSystemInfo = sceneHonorRes.historyHonorInfoList;
            //发送ui消息,更新界面
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveHonorSystemResMessage);
        }

        private void OnReceiveSceneHonorRedPoint(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            SceneHonorRedPoint sceneHonorRedPoint = new SceneHonorRedPoint();
            sceneHonorRedPoint.decode(msgData.bytes);

            //设置展示红点的标志
            IsShowRedPointFlag = true;
            
            //发送红点更新的消息
            HonorSystemUtility.SendHonorSystemRedPointUpdateMessage();
        }

        //缓存并获得HonorPlayerTable的数据
        public HonorPlayerTable GetHonorPlayerTable(int honorPvpType)
        {

            if (HonorPlayerDictionary.ContainsKey(honorPvpType) == false)
            {
                var curHonorPlayerTable = TableManager.GetInstance().GetTableItem<HonorPlayerTable>(honorPvpType);
                if (curHonorPlayerTable != null)
                    HonorPlayerDictionary[honorPvpType] = curHonorPlayerTable;
            }

            HonorPlayerTable honorPlayerTable = null;
            HonorPlayerDictionary.TryGetValue(honorPvpType, out honorPlayerTable);
            return honorPlayerTable;
        }


    }
}
