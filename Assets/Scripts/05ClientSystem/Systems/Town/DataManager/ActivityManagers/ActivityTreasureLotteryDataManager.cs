using System.Collections.Generic;
using DataModel;
using Network;
using Protocol;
using ProtoTable;

namespace GameClient
{
    public interface IActivityTreasureLotteryDataMananger
    {
        int GetModelAmount<T>();
        string GetRemainTime();
        ETreasureLotterState GetState();

        //根据表里的商品id获取数组下标id
        int GetModelIndexByLotteryId<T>(int lotteryId);

        T GetModel<T>(int id) where T : IActivityTreasureLotteryModelBase;

        bool IsHadData();
    }

    public enum ETreasureLotterState
    {
        Close,
        Open,
        Prepare
    }


    public class ActivityTreasureLotteryDataManager : DataManager<ActivityTreasureLotteryDataManager>, IActivityTreasureLotteryDataMananger
    {
        private enum EDataType
        {
            Activity,
            MyLottery,
            History,
            Count
        }

        private const int DEFAULT_LIST_CAPCITY = 8;
        private List<ActivityTreasureLotteryModel> mActivityModelList = new List<ActivityTreasureLotteryModel>(DEFAULT_LIST_CAPCITY);
        private List<ActivityTreasureLotteryMyLotteryModel> mMyLotteryModelList = new List<ActivityTreasureLotteryMyLotteryModel>(DEFAULT_LIST_CAPCITY);
        private List<ActivityTreasureLotteryHistoryModel> mHistroyModelList = new List<ActivityTreasureLotteryHistoryModel>(DEFAULT_LIST_CAPCITY);
        private Queue<ActivityTreasureLotteryDrawModel> _drawLotteryQueue = new Queue<ActivityTreasureLotteryDrawModel>(DEFAULT_LIST_CAPCITY);
        private ETreasureLotterState _state = ETreasureLotterState.Close;
        private uint mTime;
        private uint[] mLastRequestTime;
        private ushort mUnlockLevel = ushort.MaxValue;

        public string GetRemainTime()
        {
            return Function.GetLastsTimeStr(this.mTime - TimeManager.GetInstance().GetServerTime());
        }

        public ETreasureLotterState GetState()
        {
            if (this._state == ETreasureLotterState.Open || this._state == ETreasureLotterState.Prepare)
            {
                if (PlayerBaseData.GetInstance().Level < this.mUnlockLevel)
                {
                    return ETreasureLotterState.Close;
                }
            }

            return this._state;
        }

        public IActivityTreasureLotteryDrawModel DequeueDrawLottery()
        {
            if (this._drawLotteryQueue == null || this._drawLotteryQueue.Count <= 0)
            {
                return null;
            }

            return this._drawLotteryQueue.Dequeue();
        }

        public int GetDrawLotteryCount()
        {
            return this._drawLotteryQueue.Count;
        }

#if ACTIVITY_TEST
        public void TestInit()
        {
            _state = ETreasureLotterState.Open;
            _time = 3600 + TimeManager.GetInstance().GetServerTime();
            TestCreateActivityModel();
            TestCreateMyLotteryModel();
            TestCreateHistoryModel();
        }

        void TestCreateActivityModel()
        {
            var table = TableManager.GetInstance().GetTable<ProtoTable.ItemTable>();
            UnityEngine.Random.InitState(DateTime.Now.Millisecond);
            List<object> list = new List<object>(table.Values);
            int count = list.Count > 100 ? 100 : list.Count;
            int currencyType = (int)ProtoTable.ItemTable.eSubType.WARRIOR_SOUL;
            mActivityModelList.Add(new ActivityTreasureLotteryModel(110210001, 100, 100, 100, 100, GambingItemStatus.GIS_PREPARE, 10, ItemDataManager.GetInstance().GetMoneyIDByType((ProtoTable.ItemTable.eSubType.BindPOINT)), 10, 1, 0));
            mActivityModelList.Add(new ActivityTreasureLotteryModel(155411002, 100, 100, 0, 0, GambingItemStatus.GIS_SOLD_OUE, 10, ItemDataManager.GetInstance().GetMoneyIDByType((ProtoTable.ItemTable.eSubType.POINT)), 0, 2, 1));
            for (int i = 0; i < 7; ++i)
            {
                ProtoTable.ItemTable item = list[i] as ProtoTable.ItemTable;
                var numberPerGroup = UnityEngine.Random.Range(0, 1000);
                var totalGroup = UnityEngine.Random.Range(0, 100);
                uint leftNum = (uint)UnityEngine.Random.Range(0, 1000);
                var leftGroupNum = UnityEngine.Random.Range(0, 100);
                var boughtNum = UnityEngine.Random.Range(0, 100);
                mActivityModelList.Add(new ActivityTreasureLotteryModel(item.ID, numberPerGroup, totalGroup, leftNum, leftGroupNum, GambingItemStatus.GIS_SELLING, 10, ItemDataManager.GetInstance().GetMoneyIDByType((ProtoTable.ItemTable.eSubType)currencyType), boughtNum, (ushort)i, (uint)i + 2));
                if (++currencyType > (int)ProtoTable.ItemTable.eSubType.BindPOINT)
                {
                    currencyType = (int)ProtoTable.ItemTable.eSubType.WARRIOR_SOUL;
                }
            }
            mActivityModelList.Sort(CompareActivityModel);
        }

        void TestCreateMyLotteryModel()
        {
            var table = TableManager.GetInstance().GetTable<ProtoTable.ItemTable>();
            List<object> list = new List<object>(table.Values);
            int currencyType = (int)ProtoTable.ItemTable.eSubType.WARRIOR_SOUL;
            for (int i = 0; i < 20; ++i)
            {
                ProtoTable.ItemTable item = list[i] as ProtoTable.ItemTable;
                var myInvestment = UnityEngine.Random.Range(0, 1000);
                var totalGroup = UnityEngine.Random.Range(0, 100);
                int leftNum = UnityEngine.Random.Range(0, 1000);
                var leftGroupNum = UnityEngine.Random.Range(0, 100);
                var boughtNum = UnityEngine.Random.Range(0, 100);
                var totalNum = UnityEngine.Random.Range(100, 1000);
                int moneyId = ItemDataManager.GetInstance().GetMoneyIDByType((ProtoTable.ItemTable.eSubType)currencyType);
                mMyLotteryModelList.Add(new ActivityTreasureLotteryMyLotteryModel(item.ID, boughtNum, leftGroupNum, true, leftNum, myInvestment, totalNum, moneyId, 100, "获胜者", 10));
                if (++currencyType > (int)ProtoTable.ItemTable.eSubType.BindPOINT)
                {
                    currencyType = (int)ProtoTable.ItemTable.eSubType.WARRIOR_SOUL;
                }
            }
        }

        void TestCreateHistoryModel()
        {
            var table = TableManager.GetInstance().GetTable<ProtoTable.ItemTable>();
            List<object> list = new List<object>(table.Values);
            int currencyType = (int)ProtoTable.ItemTable.eSubType.WARRIOR_SOUL;
            for (int i = 0; i < 20; ++i)
            {
                ProtoTable.ItemTable item = list[i] as ProtoTable.ItemTable;
                var myInvestment = UnityEngine.Random.Range(500, 1000);
                var roleId = UnityEngine.Random.Range(0, 100);
                var leftGroupNum = UnityEngine.Random.Range(0, 100);
                int moneyId = ItemDataManager.GetInstance().GetMoneyIDByType((ProtoTable.ItemTable.eSubType)currencyType);
                GambingGroupRecordData[] playerList = new GambingGroupRecordData[5];
                playerList[0] = new GambingGroupRecordData();
                playerList[0].gainerId = PlayerBaseData.GetInstance().RoleID;
                playerList[0].gainerName = PlayerBaseData.GetInstance().Name;
                playerList[0].groupId = 1;
                playerList[0].investCurrencyId = (uint)moneyId;
                playerList[0].investCurrencyNum = (uint)myInvestment;
                for (int j = 1; j < 5; ++j)
                {
                    playerList[j] = new GambingGroupRecordData();
                    playerList[j].gainerId = (uint)roleId;
                    playerList[j].gainerName = "其他玩家";
                    playerList[j].groupId = (ushort)leftGroupNum;
                    playerList[j].investCurrencyId = (uint)moneyId;
                    var invest = UnityEngine.Random.Range(500, 1000);
                    playerList[j].investCurrencyNum = (uint)invest;
                }
                bool isZero = UnityEngine.Random.Range(0, 1) == 0;
                int soldTime = 0;
                if (!isZero)
                {
                    soldTime = (int)(TimeManager.GetInstance().GetServerTime() - UnityEngine.Random.Range(0, 3600));
                }
                mHistroyModelList.Add(new ActivityTreasureLotteryHistoryModel(item.ID, true, playerList, soldTime, i));
                if (++currencyType > (int)ProtoTable.ItemTable.eSubType.BindPOINT)
                {
                    currencyType = (int)ProtoTable.ItemTable.eSubType.WARRIOR_SOUL;
                }
            }
        }
#endif

        public T GetModel<T>(int id) where T : IActivityTreasureLotteryModelBase
        {
            if (typeof(T) == typeof(IActivityTreasureLotteryModel))
            {
                return (T) GetActivityModel(id);
            }

            if (typeof(T) == typeof(IActivityTreasureLotteryMyLotteryModel))
            {
                return (T) GetMyLotteryModel(id);
            }

            if (typeof(T) == typeof(IActivityTreasureLotteryHistoryModel))
            {
                return (T) GetHistoryModel(id);
            }

            //error
            return default(T);
        }

        public bool IsHadData()
        {
            return this.mActivityModelList != null && this.mActivityModelList.Count > 0;
        }

        public int GetItemIdByLotteryId(int lotteryId)
        {
            if (this.mActivityModelList == null)
            {
                return 0;
            }

            for (var i = 0; i < this.mActivityModelList.Count; ++i)
            {
                if (this.mActivityModelList[i].LotteryId == lotteryId)
                {
                    return this.mActivityModelList[i].ItemId;
                }
            }

            return 0;
        }

        public int GetModelIndexByLotteryId<T>(int lotteryId)
        {
            if (typeof(T) == typeof(IActivityTreasureLotteryModel))
            {
                return GetActivityModelIndexByLotteryId(lotteryId);
            }

            if (typeof(T) == typeof(IActivityTreasureLotteryMyLotteryModel))
            {
                return GetMyLotteryModelIndexByLotteryId(lotteryId);
            }

            if (typeof(T) == typeof(IActivityTreasureLotteryHistoryModel))
            {
                return GetHistoryModelIndexByLotteryId(lotteryId);
            }

            return 0;
        }

        public int GetModelAmount<T>()
        {
            if (typeof(T) == typeof(IActivityTreasureLotteryModel))
            {
                return GetActivityModelAmount();
            }

            if (typeof(T) == typeof(IActivityTreasureLotteryMyLotteryModel))
            {
                return GetMyLotteryModelAmount();
            }

            if (typeof(T) == typeof(IActivityTreasureLotteryHistoryModel))
            {
                return GetHistoryModelAmount();
            }

            return 0;
        }

        public void BuyLottery(int id, uint buyCount, bool isBuyAll)
        {
            if (id < 0 || this.mActivityModelList == null || id >= this.mActivityModelList.Count)
            {
                Logger.LogError("mActivityModelList is null");
                return;
            }

            var req = new PayingGambleReq
            {
                gambingItemId = this.mActivityModelList[id].LotteryId,
                groupId = this.mActivityModelList[id].GroupId,
                investCopies = buyCount,
                bBuyAll = (byte) (isBuyAll ? 1 : 0)
            };
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        public void GetLotteryItemList(bool isImmediately = false)
        {
            if (GetState() == ETreasureLotterState.Close)
            {
                return;
            }
            var syncInterval = TableManager.GetInstance().GetTableItem<SystemValueTable>((int) SystemValueTable.eType2.SVT_GAMBING_REFRESH_REQUEST_RATE).Value;
            var serverTime = TimeManager.GetInstance().GetServerTime();
            if (!isImmediately && serverTime - this.mLastRequestTime[(int) EDataType.Activity] < syncInterval)
            {
                return;
            }

            this.mLastRequestTime[(int) EDataType.Activity] = serverTime;
            var req = new GambingItemQueryReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        public void GetMyLotteryItemList(bool isImmediately = false)
        {
            int syncInterval = TableManager.GetInstance().GetTableItem<SystemValueTable>((int) SystemValueTable.eType2.SVT_GAMBING_REFRESH_REQUEST_RATE).Value;
            uint serverTime = TimeManager.GetInstance().GetServerTime();

            if(this.mLastRequestTime == null || (int)EDataType.MyLottery >= this.mLastRequestTime.Length)
            {
                return;
            }

            if (!isImmediately && serverTime - this.mLastRequestTime[(int) EDataType.MyLottery] < syncInterval)
            {
                return;
            }

            this.mLastRequestTime[(int) EDataType.MyLottery] = serverTime;
            var req = new GambingMineQueryReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        public void GetHistroyItemList(bool isImmediately = false)
        {
            int syncInterval = TableManager.GetInstance().GetTableItem<SystemValueTable>((int) SystemValueTable.eType2.SVT_GAMBING_REFRESH_REQUEST_RATE).Value;
            uint serverTime = TimeManager.GetInstance().GetServerTime();

            if (this.mLastRequestTime == null || (int)EDataType.History >= this.mLastRequestTime.Length)
            {
                return;
            }

            if (isImmediately || serverTime - this.mLastRequestTime[(int) EDataType.History] >= syncInterval)
            {
                this.mLastRequestTime[(int) EDataType.History] = serverTime;
                var req = new GambingRecordQueryReq();
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
            }
        }

        public override void Initialize()
        {
            this.mLastRequestTime = new uint[(int) EDataType.Count];
#if ACTIVITY_TEST
            TestInit();
#endif

            NetProcess.AddMsgHandler(PayingGambleRes.MsgID, OnBuyLotteryResp);
            NetProcess.AddMsgHandler(GambingItemQueryRes.MsgID, OnGetLotteryItemList);
            NetProcess.AddMsgHandler(GambingMineQueryRes.MsgID, OnGetMyLotteryInfo);
            NetProcess.AddMsgHandler(GambingRecordQueryRes.MsgID, OnGetHistory);
            NetProcess.AddMsgHandler(GambingLotterySync.MsgID, OnSyncDrawLotteryResp);
            NetProcess.AddMsgHandler(SyncOpActivityDatas.MsgID, OnSyncActivities);
            NetProcess.AddMsgHandler(SyncOpActivityStateChange.MsgID, OnSyncActivityStateChange);
        }

        public override void Clear()
        {
            this._state = ETreasureLotterState.Close;
            this.mTime = 0;
            this.mLastRequestTime = null;
            this.mUnlockLevel = ushort.MaxValue;
            this.mActivityModelList.Clear();
            this.mMyLotteryModelList.Clear();
            this.mHistroyModelList.Clear();
            this._drawLotteryQueue.Clear();
            NetProcess.RemoveMsgHandler(PayingGambleRes.MsgID, OnBuyLotteryResp);
            NetProcess.RemoveMsgHandler(GambingItemQueryRes.MsgID, OnGetLotteryItemList);
            NetProcess.RemoveMsgHandler(GambingMineQueryRes.MsgID, OnGetMyLotteryInfo);
            NetProcess.RemoveMsgHandler(GambingRecordQueryRes.MsgID, OnGetHistory);
            NetProcess.RemoveMsgHandler(GambingLotterySync.MsgID, OnSyncDrawLotteryResp);
            NetProcess.RemoveMsgHandler(SyncOpActivityDatas.MsgID, OnSyncActivities);
            NetProcess.RemoveMsgHandler(SyncOpActivityStateChange.MsgID, OnSyncActivityStateChange);
            PlayerBaseData.GetInstance().onLevelChanged -= OnLevelChanged;
        }

        /// <summary>
        ///     根据数组下标获取夺宝数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private IActivityTreasureLotteryModel GetActivityModel(int id)
        {
            if (id < 0 || this.mActivityModelList == null || id >= this.mActivityModelList.Count)
            {
                return default(ActivityTreasureLotteryModel);
            }

            return this.mActivityModelList[id];
        }


        private IActivityTreasureLotteryMyLotteryModel GetMyLotteryModel(int id)
        {
            if (id < 0 || this.mMyLotteryModelList == null || id >= this.mMyLotteryModelList.Count)
            {
                return default(ActivityTreasureLotteryMyLotteryModel);
            }

            return this.mMyLotteryModelList[id];
        }

        private IActivityTreasureLotteryHistoryModel GetHistoryModel(int id)
        {
            if (id < 0 || this.mHistroyModelList == null || id >= this.mHistroyModelList.Count)
            {
                return default(ActivityTreasureLotteryHistoryModel);
            }

            return this.mHistroyModelList[id];
        }

        private void OnSyncActivities(MsgDATA data)
        {
            var resp = new SyncOpActivityDatas();
            resp.decode(data.bytes);
            for (var i = 0; i < resp.datas.Length; ++i)
            {
                if (resp.datas[i].tmpType == (int) OpActivityTmpType.OAT_GAMBING)
                {
                    InitData(resp.datas[i].state, resp.datas[i].startTime, resp.datas[i].endTime, resp.datas[i].playerLevelLimit);
                    break;
                }
            }
        }

        private void InitData(byte state, uint startTime, uint endTime, ushort unlockLevel)
        {
            this._state = (ETreasureLotterState) state;
            this.mUnlockLevel = unlockLevel;
            switch (this._state)
            {
                case ETreasureLotterState.Prepare:
                    this.mTime = startTime;
                    break;
                case ETreasureLotterState.Open:
                    this.mTime = endTime;
                    break;
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TreasureLotteryStatusChange);

            if (this.mUnlockLevel > PlayerBaseData.GetInstance().Level)
            {
                PlayerBaseData.GetInstance().onLevelChanged -= OnLevelChanged;
                PlayerBaseData.GetInstance().onLevelChanged += OnLevelChanged;
            }
        }

        private void OnLevelChanged(int iPreLv, int iCurLv)
        {
            if (iCurLv >= this.mUnlockLevel)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TreasureLotteryStatusChange);
                PlayerBaseData.GetInstance().onLevelChanged -= OnLevelChanged;
            }
        }

        private void OnSyncActivityStateChange(MsgDATA data)
        {
            var resp = new SyncOpActivityStateChange();
            resp.decode(data.bytes);
            if (resp.data.tmpType == (int) OpActivityTmpType.OAT_GAMBING)
            {
                InitData(resp.data.state, resp.data.startTime, resp.data.endTime, resp.data.playerLevelLimit);
            }
        }

        private void OnBuyLotteryResp(MsgDATA data)
        {
            var pos = 0;
            var resp = new PayingGambleRes();
            resp.decode(data.bytes, ref pos);
            if (resp.retCode == 0 && resp.itemInfo != null)
            {
                UpdateLotteryItem(resp.itemInfo);
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TreasureLotteryBuyResp, resp);
        }

        private void UpdateLotteryItem(GambingItemInfo item)
        {
            if (this.mActivityModelList == null)
            {
                return;
            }

            for (var i = 0; i < this.mActivityModelList.Count; ++i)
            {
                if (this.mActivityModelList[i].LotteryId == item.gambingItemId)
                {
                    this.mActivityModelList[i] = new ActivityTreasureLotteryModel(item);
                    this.mActivityModelList.Sort(CompareActivityModel);
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TreasureLotterySyncActivity);
                    break;
                }
            }
        }

        private void OnGetLotteryItemList(MsgDATA data)
        {
            var pos = 0;
            var resp = new GambingItemQueryRes();
            resp.decode(data.bytes, ref pos);
            if (this.mActivityModelList == null)
            {
                this.mActivityModelList = new List<ActivityTreasureLotteryModel>(DEFAULT_LIST_CAPCITY);
            }

            this.mActivityModelList.Clear();
            for (var i = 0; i < resp.gambingItems.Length; ++i)
            {
                this.mActivityModelList.Add(new ActivityTreasureLotteryModel(resp.gambingItems[i]));
            }

            this.mActivityModelList.Sort(CompareActivityModel);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TreasureLotterySyncActivity);
        }

        private void OnGetMyLotteryInfo(MsgDATA data)
        {
            var pos = 0;
            var resp = new GambingMineQueryRes();
            resp.decode(data.bytes, ref pos);
            if (this.mMyLotteryModelList == null)
            {
                this.mMyLotteryModelList = new List<ActivityTreasureLotteryMyLotteryModel>(DEFAULT_LIST_CAPCITY);
            }

            this.mMyLotteryModelList.Clear();
            for (var i = 0; i < resp.mineGambingInfo.Length; ++i)
            {
                this.mMyLotteryModelList.Add(new ActivityTreasureLotteryMyLotteryModel(resp.mineGambingInfo[i]));
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TreasureLotterySyncMyLottery);
        }

        private void OnGetHistory(MsgDATA data)
        {
            var pos = 0;
            var resp = new GambingRecordQueryRes();
            resp.decode(data.bytes, ref pos);
            if (this.mHistroyModelList == null)
            {
                this.mHistroyModelList = new List<ActivityTreasureLotteryHistoryModel>(DEFAULT_LIST_CAPCITY);
            }

            this.mHistroyModelList.Clear();
            for (var i = 0; i < resp.gambingRecordDatas.Length; ++i)
            {
                this.mHistroyModelList.Add(new ActivityTreasureLotteryHistoryModel(resp.gambingRecordDatas[i]));
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TreasureLotterySyncHistory);
        }

        private void OnSyncDrawLotteryResp(MsgDATA data)
        {
            var info = new GambingLotterySync();
            info.decode(data.bytes);
            if (this._drawLotteryQueue == null)
            {
                this._drawLotteryQueue = new Queue<ActivityTreasureLotteryDrawModel>(DEFAULT_LIST_CAPCITY);
            }

            this._drawLotteryQueue.Enqueue(new ActivityTreasureLotteryDrawModel(info.gainersGambingInfo, info.participantsGambingInfo));
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TreasureLotterySyncDraw);
        }


        private int GetActivityModelAmount()
        {
            return this.mActivityModelList == null ? 0 : this.mActivityModelList.Count;
        }

        private int GetMyLotteryModelAmount()
        {
            return this.mMyLotteryModelList == null ? 0 : this.mMyLotteryModelList.Count;
        }

        private int GetHistoryModelAmount()
        {
            return this.mHistroyModelList == null ? 0 : this.mHistroyModelList.Count;
        }

        private int GetActivityModelIndexByLotteryId(int lotteryId)
        {
            if (this.mActivityModelList == null)
            {
                return 0;
            }

            for (var i = 0; i < this.mActivityModelList.Count; ++i)
            {
                if (this.mActivityModelList[i].LotteryId == lotteryId)
                {
                    return i;
                }
            }

            return 0;
        }

        private int GetMyLotteryModelIndexByLotteryId(int lotteryId)
        {
            if (this.mMyLotteryModelList == null)
            {
                return 0;
            }

            for (var i = 0; i < this.mMyLotteryModelList.Count; ++i)
            {
                if (this.mMyLotteryModelList[i].LotteryId == lotteryId)
                {
                    return i;
                }
            }

            return 0;
        }

        private int GetHistoryModelIndexByLotteryId(int lotteryId)
        {
            if (this.mHistroyModelList == null)
            {
                return 0;
            }

            for (var i = 0; i < this.mHistroyModelList.Count; ++i)
            {
                if (this.mHistroyModelList[i].LotteryId == lotteryId)
                {
                    return i;
                }
            }

            return 0;
        }

        private int CompareActivityModel(ActivityTreasureLotteryModel a, ActivityTreasureLotteryModel b)
        {
            if (a == null || b == null)
            {
                Logger.LogError("Param is null");
                return 0;
            }

            if (a.State != b.State)
            {
                if (a.State == GambingItemStatus.GIS_SOLD_OUE || a.State == GambingItemStatus.GIS_LOTTERY)
                {
                    return 1;
                }

                if (b.State == GambingItemStatus.GIS_SOLD_OUE || b.State == GambingItemStatus.GIS_LOTTERY)
                {
                    return -1;
                }
            }

            if (a.SortId < b.SortId)
            {
                return -1;
            }

            return a.SortId > b.SortId ? 1 : 0;
        }
    }
}