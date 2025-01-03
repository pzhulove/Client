using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using Network;
using ProtoTable;

namespace GameClient
{
    public class RandomTreasureMapModel
    {
        public int mapId = -1;
        public int beInPlayerNum = 0;
        public List<RandomTreasureMapDigSiteModel> mapTotalDigSites = new List<RandomTreasureMapDigSiteModel>();
        public int goldSiteNum = 0;
        public int silverSiteNum = 0;
        public DigMapTable localMapData = null;

        public RandomTreasureMapModel()
        {
            Clear();
        }

        public void Clear()
        {
            mapId = -1;
            beInPlayerNum = 0;
            goldSiteNum = 0;
            silverSiteNum = 0;
            if (mapTotalDigSites != null)
            {
                mapTotalDigSites.Clear();
            }
            localMapData = null;
        }
    }

    public class RandomTreasureMapDigSiteModel
    {
        public int index = -1;                                                  //挖宝点在列表里的序号
        public int mapId = -1;
        public DigType type = DigType.DIG_INVALID;                              //无效点表示不可挖的点（是否需要显示？？？）
        public DigStatus status = DigStatus.DIG_STATUS_INVALID;                 //注意 OPEN状态表示已经被开过了
        public uint refreshTime;
        public uint changeStatusTime;
        public ItemSimpleData openItem;                                         //当前挖宝点已经被打开的 道具
        public List<ItemSimpleData> itemSDatas = new List<ItemSimpleData>();    //当前挖宝点的宝藏道具

        public RandomTreasureMapDigSiteModel()
        {
            Clear();
        }

        public void Clear()
        {
            index = -1;
            mapId = -1;
            type = DigType.DIG_INVALID;
            status = DigStatus.DIG_STATUS_INVALID;
            refreshTime = 0;
            changeStatusTime = 0;
            openItem = null;
            if (itemSDatas != null)
            {
                itemSDatas.Clear();
            }
        }
    }

    public class RandomTreasureMapRecordModel
    {
        public int mapId;
        public string mapName;
        public int digIndex = -1;
        public DigType digType = DigType.DIG_INVALID;
        public ulong playerId;
        public string playerName;
        public ItemSimpleData itemSData;

        public RandomTreasureMapRecordModel()
        {
            Clear();
        }

        public void Clear()
        {
            mapId = -1;
            mapName = "";
            digIndex = -1;
            digType = DigType.DIG_INVALID;
            playerId = 0;
            playerName = "";
            itemSData = null;
        }
    }

    public class RandomTreasureUIEvent : UIEvent
    {
        public RandomTreasureUIEvent(int timeStamp)
        {
            this.excuteTimeStmp = timeStamp;
        }
        public int excuteTimeStmp = 0;
    }

    /// <summary>
    /// 随机宝箱管理
    /// 
    /// 道具：月卡特权道具
    /// </summary>
    public class RandomTreasureDataManager : DataManager<RandomTreasureDataManager>
    {
        #region Model Params        

        private int mGoldenShovelCount = 0;                                     //铲子数目
        private int mSilverShovelCount = 0;

        private ProtoTable.ItemTable.eColor mFilterRecordByQuality = ItemTable.eColor.PURPLE;           //显示记录 最低道具品级

        private Dictionary<int, RandomTreasureMapModel> mTotalMapModelDic = new Dictionary<int, RandomTreasureMapModel>();
        private List<RandomTreasureMapRecordModel> mTotalMapRecordList = new List<RandomTreasureMapRecordModel>();

        private List<RandomTreasureUIEvent> mDelayRefreshUIEventList = new List<RandomTreasureUIEvent>();
        private float mUpdateTimer = 0f;
        private float mUpdateInterval = 5f;

        private int gold_Treasure_Item_Id = 330000193;
        public int Gold_Treasure_Item_Id 
        {
            get { return gold_Treasure_Item_Id; }        
        }
        private int silver_Treasure_Item_Id = 330000192;
        public int Silver_Treasure_Item_Id
        {
            get { return silver_Treasure_Item_Id; }
        }

        private ItemSimpleData goldRaffleMustGetItem;
        public ItemSimpleData GoldRaffleMustGetItem
        {
            get{ return goldRaffleMustGetItem; }
        }

        private ItemSimpleData silverRaffleMustGetItem;
        public ItemSimpleData SilverRaffleMustGetItem
        {
            get { return silverRaffleMustGetItem; }
        }

        //是否跳过 银宝箱 抽奖动画  （本次登录有效）
        private bool bSilverRaffleSkipAnim = false;
        public bool BSilverRaffleSkipAnim
        {
            get { return bSilverRaffleSkipAnim; }
            set { bSilverRaffleSkipAnim = value; }
        }

        #endregion
        
        #region PRIVATE METHODS

        public override void Initialize()
        {
            _InitLocalMapData();
            _BindEvent();
        }

        public override void Clear()
        {
            ClearRandomTreasureData();
            _UnBindEvent();
        }

        public override void Update(float a_fTime)
        {
            mUpdateTimer += a_fTime;
            if (mUpdateTimer > mUpdateInterval)
            {
                _InvokePreUIEventInDelayList();
                mUpdateTimer = 0f;
            }
        }

        private void _InvokePreUIEventInDelayList()
        {
            if (mDelayRefreshUIEventList != null && mDelayRefreshUIEventList.Count > 0)
            {
                var uiEvent = mDelayRefreshUIEventList[0];
                if (uiEvent != null)
                {
                    UIEventSystem.GetInstance().SendUIEvent(uiEvent);
                }
                mDelayRefreshUIEventList.RemoveAt(0);
                Logger.LogProcessFormat("[RandomTreasureDataManager] - Update : SendUIEvent : {0}", uiEvent.EventID.ToString());
            }                
        }

        private void _BindEvent()
        {
            NetProcess.AddMsgHandler(WorldDigMapOpenRes.MsgID, _OnReqOpenDigMapRes);
            NetProcess.AddMsgHandler(WorldDigWatchRes.MsgID, _OnReqWatchDigSiteRes);
            NetProcess.AddMsgHandler(WorldDigOpenRes.MsgID, _OnReqOpenDigSiteRes);
            NetProcess.AddMsgHandler(WorldDigMapInfoRes.MsgID, _OnReqMapInfoRes);
            NetProcess.AddMsgHandler(WorldDigRecordsRes.MsgID, _OnReqDigRecordRes);

            ServerSceneFuncSwitchManager.GetInstance().AddServerFuncSwitchListener(ServiceType.SERVICE_DIG, _OnServerSwitchFunc);
        }

        private void _UnBindEvent()
        {
            NetProcess.RemoveMsgHandler(WorldDigMapOpenRes.MsgID, _OnReqOpenDigMapRes);
            NetProcess.RemoveMsgHandler(WorldDigWatchRes.MsgID, _OnReqWatchDigSiteRes);
            NetProcess.RemoveMsgHandler(WorldDigOpenRes.MsgID, _OnReqOpenDigSiteRes);
            NetProcess.RemoveMsgHandler(WorldDigMapInfoRes.MsgID, _OnReqMapInfoRes);
            NetProcess.RemoveMsgHandler(WorldDigRecordsRes.MsgID, _OnReqDigRecordRes);

            ServerSceneFuncSwitchManager.GetInstance().RemoveServerFuncSwitchListener(ServiceType.SERVICE_DIG, _OnServerSwitchFunc);
        }

        private void _InitLocalMapData()
        {
            if (mTotalMapModelDic == null)
            {
                mTotalMapModelDic = new Dictionary<int, RandomTreasureMapModel>();
            }
            else
            {
                mTotalMapModelDic.Clear();
            }
            var digMapTable = TableManager.GetInstance().GetTable<DigMapTable>();
            if (digMapTable == null)
            {
                Logger.LogErrorFormat("[RandomTreasureDataManager] - _InitLocalMapData not find digMapTable");
                return;
            }
            var enumerator = digMapTable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current.Value as DigMapTable;
                RandomTreasureMapModel mapModel = new RandomTreasureMapModel();
                mapModel.mapId = item.ID;
                mapModel.localMapData = item;
                if (mTotalMapModelDic.ContainsKey(item.ID))
                {
                    mTotalMapModelDic[item.ID].localMapData = item;
                }
                else
                {
                    mTotalMapModelDic.Add(item.ID, mapModel);
                }
            }

            string interval_tr = TR.Value("random_treasure_ui_event_delay_interval");
            if (float.TryParse(interval_tr, out mUpdateInterval)) { }

            var sysTable_1 = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_DIG_GOLD_BUY_ITEM_ID);
            var sysTable_2 = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_DIG_GOLD_BUY_ITEM_NUM);
            if (sysTable_1 != null && sysTable_2 != null)
            {
                 string itemName = ItemDataManager.GetInstance().GetOwnedItemName(sysTable_1.Value);
                 goldRaffleMustGetItem = new ItemSimpleData(sysTable_1.Value, sysTable_2.Value);
                 goldRaffleMustGetItem.Name = itemName;
            }

            var sysTable_3 = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_DIG_SILVER_BUY_ITEM_ID);
            var sysTable_4 = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_DIG_SILVER_BUY_ITEM_NUM);
            if (sysTable_3 != null && sysTable_4 != null)
            {
                string itemName = ItemDataManager.GetInstance().GetOwnedItemName(sysTable_3.Value);
                silverRaffleMustGetItem = new ItemSimpleData(sysTable_3.Value, sysTable_4.Value);
                silverRaffleMustGetItem.Name = itemName;
            }

            var sysTable_5 = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_DIG_GOLD_ITEM_ID);
            if (sysTable_5 != null)
            {
                gold_Treasure_Item_Id = sysTable_5.Value;
            }

            var sysTable_6 = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_DIG_SILVER_ITEM_ID);
            if (sysTable_6 != null)
            {
                silver_Treasure_Item_Id = sysTable_6.Value;
            }
        }

        private void ClearRandomTreasureData()
        {
            if (mTotalMapModelDic != null)
            {
                var enumerator = mTotalMapModelDic.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var model = enumerator.Current.Value as RandomTreasureMapModel;
                    if(model != null)
                    {
                        model.Clear();
                    }
                }
                mTotalMapModelDic.Clear();
            }

            if (mTotalMapRecordList != null)
            {
                for (int i = 0; i < mTotalMapRecordList.Count; i++)
                {
                    var record = mTotalMapRecordList[i];
                    if (record != null)
                    {
                        record.Clear();
                    }
                }
                mTotalMapRecordList.Clear();
            }

            if (mDelayRefreshUIEventList != null)
            {
                mDelayRefreshUIEventList.Clear();
            }

            goldRaffleMustGetItem = null;
            silverRaffleMustGetItem = null;

            mUpdateTimer = 0f;

            bSilverRaffleSkipAnim = false;
        }

        private void _OnServerSwitchFunc(ServerSceneFuncSwitch funcSwitch)
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTreasureFuncSwitch, funcSwitch.sIsOpen);
        }

        #region EventCB

        private void _OnSyncChangedDigInfo(MsgDATA data)
        {
            WorldDigInfoSync syncChangedDigInfo = new WorldDigInfoSync();
            syncChangedDigInfo.decode(data.bytes);
            int netMapId = (int)syncChangedDigInfo.mapId;
            if (mTotalMapModelDic == null)
            {
                Logger.LogError("[RandomTreasureDataManager] - _OnSyncChangedDigInfo mTotalMapModelDic is null");
                return;
            }
            if (!mTotalMapModelDic.ContainsKey(netMapId))
            {
                Logger.LogErrorFormat("[RandomTreasureDataManager] - _OnSyncChangedDigInfo local map data is not equal to net data : {0}", netMapId);
                return;
            }
            RandomTreasureMapModel localMap = mTotalMapModelDic[netMapId];
            if (localMap == null || localMap.mapTotalDigSites == null)
            {
                Logger.LogErrorFormat("[RandomTreasureDataManager] - _OnSyncChangedDigInfo mTotalMapModelDic key {0} , obj is null", netMapId);
                return;
            }
            var changedDigInfo = syncChangedDigInfo.info;
            if (changedDigInfo == null)
            {
                Logger.LogErrorFormat("[RandomTreasureDataManager] - _OnSyncChangedDigInfo ServerData is ERROR");
                return;
            }
            RandomTreasureMapDigSiteModel digSiteModel = null;
            for (int i = 0; i < localMap.mapTotalDigSites.Count; i++)
            {
                digSiteModel = localMap.mapTotalDigSites[i];
                if (digSiteModel == null)
                {
                    continue;
                }
                if (digSiteModel.index == changedDigInfo.index)
                {
                    _DigSiteNetDataToLocalData(digSiteModel, changedDigInfo, netMapId);
                    break;
                }
            }

            Logger.LogProcessFormat("[RandomTreasureDataManager] - _OnSyncChangedDigInfo OnTreasureDigSiteChanged");
            if (digSiteModel != null)
            {
                int timeStamp =(int)TimeManager.GetInstance().GetServerTime();
                RandomTreasureUIEvent uiEvent = new RandomTreasureUIEvent(timeStamp);
                uiEvent.EventID = EUIEventID.OnTreasureDigSiteChanged;
                uiEvent.Param1 = digSiteModel;

                if (mDelayRefreshUIEventList != null)
                {
                    mDelayRefreshUIEventList.Add(uiEvent);
                }
                //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTreasureDigSiteChanged, digSiteModel);
            }
        }

        private void _OnSyncResetDigInfos(MsgDATA data)
        {
            WorldDigRefreshSync syncRefreshDigInfos = new WorldDigRefreshSync();
            syncRefreshDigInfos.decode(data.bytes);
            int netMapId = (int)syncRefreshDigInfos.mapId;
            if (mTotalMapModelDic == null)
            {
                Logger.LogError("[RandomTreasureDataManager] - _OnSyncResetDigInfos mTotalMapModelDic is null");
                return;
            }
            if (!mTotalMapModelDic.ContainsKey(netMapId))
            {
                Logger.LogErrorFormat("[RandomTreasureDataManager] - _OnSyncResetDigInfos local map data is not equal to net data : {0}", netMapId);
                return;
            }
            RandomTreasureMapModel localMap = mTotalMapModelDic[netMapId];
            if (localMap == null)
            {
                Logger.LogErrorFormat("[RandomTreasureDataManager] - _OnSyncResetDigInfos mTotalMapModelDic key {0} , obj is null", netMapId);
                return;
            }
            if (localMap.mapTotalDigSites == null)
            {
                localMap.mapTotalDigSites = new List<RandomTreasureMapDigSiteModel>();
            }
            else
            {
                localMap.mapTotalDigSites.Clear();
            }
            for (int i = 0; i < syncRefreshDigInfos.infos.Length; i++)
            {
                var digInfo = syncRefreshDigInfos.infos[i];
                var mapSite = new RandomTreasureMapDigSiteModel();
                _DigSiteNetDataToLocalData(mapSite, digInfo, netMapId);
                localMap.mapTotalDigSites.Add(mapSite);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTreasureMapDigReset, localMap);

            Logger.LogProcessFormat("[RandomTreasureDataManager] - _OnSyncResetDigInfos UIEvent OnTreasureMapDigReset and changeMap id is " + localMap.mapId);
        }

        private void _OnReqOpenDigMapRes(MsgDATA data)
        {
            WorldDigMapOpenRes waitReqOpenDigMapRes = new WorldDigMapOpenRes();
            waitReqOpenDigMapRes.decode(data.bytes);

            Logger.LogProcessFormat("[RandomTreasureDataManager] - _OnReqOpenDigMapRes ret result is " + waitReqOpenDigMapRes.result);

            if(waitReqOpenDigMapRes.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)waitReqOpenDigMapRes.result);
                return;
            }
            int netMapId = (int)waitReqOpenDigMapRes.mapId;
            if (mTotalMapModelDic == null)
            {
                Logger.LogError("[RandomTreasureDataManager] - _OnReqOpenDigMapRes mTotalMapModelDic is null");
                return;
            }
            if (!mTotalMapModelDic.ContainsKey(netMapId))
            {
                Logger.LogErrorFormat("[RandomTreasureDataManager] - _OnReqOpenDigMapRes local map data is not equal to net data : {0}",netMapId);
                return;
            }
            RandomTreasureMapModel localMap = mTotalMapModelDic[netMapId];
            if (localMap == null)
            {
                Logger.LogErrorFormat("[RandomTreasureDataManager] - _OnReqOpenDigMapRes mTotalMapModelDic key {0} , obj is null", netMapId);
                return;
            }
            if (localMap.mapTotalDigSites == null)
            {
                localMap.mapTotalDigSites = new List<RandomTreasureMapDigSiteModel>();
            }
            else
            {
                localMap.mapTotalDigSites.Clear();
            }
            for (int i = 0; i < waitReqOpenDigMapRes.infos.Length; i++)
            {
                var digInfo = waitReqOpenDigMapRes.infos[i];
                var mapSite = new RandomTreasureMapDigSiteModel();
                _DigSiteNetDataToLocalData(mapSite, digInfo, netMapId);
                localMap.mapTotalDigSites.Add(mapSite);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnOpenTreasureDigMap, localMap);
        }

        private void _OnReqWatchDigSiteRes(MsgDATA data)
        {
            WorldDigWatchRes waitReqWatchDigRes = new WorldDigWatchRes();
            waitReqWatchDigRes.decode(data.bytes);

            Logger.LogProcessFormat("[RandomTreasureDataManager] - _OnReqWatchDigSiteRes ret result is " + waitReqWatchDigRes.result);

            if (waitReqWatchDigRes.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)waitReqWatchDigRes.result);
                return;
            }
            int netMapId = (int)waitReqWatchDigRes.mapId;
            if (mTotalMapModelDic == null)
            {
                Logger.LogError("[RandomTreasureDataManager] - _OnReqWatchDigSiteRes mTotalMapModelDic is null");
                return;
            }
            if (!mTotalMapModelDic.ContainsKey(netMapId))
            {
                Logger.LogErrorFormat("[RandomTreasureDataManager] - _OnReqWatchDigSiteRes local map data is not equal to net data : {0}", netMapId);
                return;
            }
            RandomTreasureMapModel localMap = mTotalMapModelDic[netMapId];
            if (localMap == null || localMap.mapTotalDigSites == null)
            {
                Logger.LogErrorFormat("[RandomTreasureDataManager] - _OnReqWatchDigSiteRes mTotalMapModelDic key {0} , obj is null", netMapId);
                return;
            }
            var watchDigDetailInfo = waitReqWatchDigRes.info;
            if(watchDigDetailInfo == null || watchDigDetailInfo.simpleInfo == null || 
               watchDigDetailInfo.digItems == null || waitReqWatchDigRes.index != watchDigDetailInfo.simpleInfo.index)
            {
                Logger.LogErrorFormat("[RandomTreasureDataManager] - _OnReqWatchDigSiteRes ServerData is ERROR");
                return;
            }
            RandomTreasureMapDigSiteModel digSiteModel = null;
            for (int i = 0; i < localMap.mapTotalDigSites.Count; i++)
            {
                digSiteModel = localMap.mapTotalDigSites[i];
                if (digSiteModel == null)
                {
                    continue;
                }
                if (digSiteModel.index == waitReqWatchDigRes.index)
                {
                    _DigSiteNetDataToLocalData(digSiteModel, watchDigDetailInfo, netMapId);
                    break;
                }
            }
            if (digSiteModel != null)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnWatchTreasureDigSite, digSiteModel);
            }
        }

        private void _OnReqOpenDigSiteRes(MsgDATA data)
        {
            WorldDigOpenRes waitDigOpenRes = new WorldDigOpenRes();
            waitDigOpenRes.decode(data.bytes);

            Logger.LogProcessFormat("[RandomTreasureDataManager] - _OnReqOpenDigSiteRes ret result is " + waitDigOpenRes.result);

            if (waitDigOpenRes.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)waitDigOpenRes.result);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTreasureRaffleEnd, false, false);
                
                return;
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnOpenTreasureDigSite, (int)waitDigOpenRes.itemIndex, (int)waitDigOpenRes.itemId, (int)waitDigOpenRes.itemNum);
        }

        private void _DigSiteNetDataToLocalData(RandomTreasureMapDigSiteModel localDigSite, DigInfo netDigBaseSite, int mapId)
        {
            if (localDigSite == null)
            {
                return;
            }
            if (netDigBaseSite == null)
            {
                return;
            }
            //将协议返回的值都进行赋值 ！！！
            localDigSite.mapId = mapId;

            localDigSite.index = (int)netDigBaseSite.index;
            localDigSite.type = (DigType)netDigBaseSite.type;
            localDigSite.status = (DigStatus)netDigBaseSite.status;
            localDigSite.refreshTime = netDigBaseSite.refreshTime;
            localDigSite.changeStatusTime = netDigBaseSite.changeStatusTime;
            ItemSimpleData sData = _GetItemSimpleDataByItemId((int)netDigBaseSite.openItemId, (int)netDigBaseSite.openItemNum);
            localDigSite.openItem = sData;
        }

        private void _DigSiteNetDataToLocalData(RandomTreasureMapDigSiteModel localDigSite,  DigDetailInfo netDigDetailSite, int mapId)
        {
            if (localDigSite == null)
            {
                return;
            }
            if (netDigDetailSite == null)
            {
                return;
            }
            if (netDigDetailSite.simpleInfo == null)
            {
                return;
            }
            //将协议返回的值都进行赋值 ！！！
            localDigSite.mapId = mapId;

            localDigSite.type = (DigType)netDigDetailSite.simpleInfo.type;
            localDigSite.status = (DigStatus)netDigDetailSite.simpleInfo.status;
            localDigSite.refreshTime = netDigDetailSite.simpleInfo.refreshTime;
            localDigSite.changeStatusTime = netDigDetailSite.simpleInfo.changeStatusTime;

            localDigSite.openItem = _GetItemSimpleDataByItemId((int)netDigDetailSite.simpleInfo.openItemId, (int)netDigDetailSite.simpleInfo.openItemNum);
            DigItemInfo[] digItemInfos = netDigDetailSite.digItems;
            if (digItemInfos == null)
            {
                return;
            }
            List<ItemSimpleData> tempItemSDatas = new List<ItemSimpleData>();
            for (int i = 0; i < digItemInfos.Length; i++)
            {
                var digInfo = digItemInfos[i];
                if (digInfo == null)
                {
                    continue;
                }
                ItemSimpleData sData = _GetItemSimpleDataByItemId((int)digInfo.itemId,(int)digInfo.itemNum);
                tempItemSDatas.Add(sData);
            }
            localDigSite.itemSDatas = tempItemSDatas;
        }

        private ItemSimpleData _GetItemSimpleDataByItemId(int itemId, int itemCount = 1)
        {
            ItemSimpleData sData = new ItemSimpleData(itemId, itemCount);
            ProtoTable.ItemTable tableData = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(itemId);
            if (tableData == null)
            {
                return sData;
            }
            sData.Name = tableData.Name;
            return sData;
        }

        private void _OnSyncMapPlayerNum(MsgDATA data)
        {
            WorldDigPlayerSizeSync syncPlayerNum = new WorldDigPlayerSizeSync();
            syncPlayerNum.decode(data.bytes);
            int netMapId = (int)syncPlayerNum.mapId;
            if (mTotalMapModelDic == null)
            {
                Logger.LogError("[RandomTreasureDataManager] - _OnSyncMapPlayerNum mTotalMapModelDic is null");
                return;
            }
            if (!mTotalMapModelDic.ContainsKey(netMapId))
            {
                Logger.LogErrorFormat("[RandomTreasureDataManager] - _OnSyncMapPlayerNum local map data is not equal to net data : {0}", netMapId);
                return;
            }
            RandomTreasureMapModel localMap = mTotalMapModelDic[netMapId];
            if (localMap == null)
            {
                Logger.LogErrorFormat("[RandomTreasureDataManager] - _OnSyncMapPlayerNum mTotalMapModelDic key {0} , obj is null", netMapId);
                return;
            }
            localMap.beInPlayerNum = (int)syncPlayerNum.playerSize;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTreasureMapPlayerNumSync, localMap);

            Logger.LogProcessFormat("[RandomTreasureDataManager] - _OnSyncMapPlayerNum UIEvent OnTreasureMapPlayerNumSync and changeMap id is " + localMap.mapId);
        }

        private void _OnReqMapInfoRes(MsgDATA data)
        {
            WorldDigMapInfoRes syncMapInfo = new WorldDigMapInfoRes();
            syncMapInfo.decode(data.bytes);

            Logger.LogProcessFormat("[RandomTreasureDataManager] - _OnReqMapInfoRes ret result is " + syncMapInfo.result);

            if (syncMapInfo.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)syncMapInfo.result);
                return;
            }
            if (mTotalMapModelDic == null)
            {
                Logger.LogError("[RandomTreasureDataManager] - _OnReqMapInfoRes mTotalMapModelDic is null");
                return;
            }

            var digMapInfos = syncMapInfo.digMapInfos;
            if (digMapInfos == null || digMapInfos.Length == 0)
            {
                Logger.LogError("[RandomTreasureDataManager] - _OnReqMapInfoRes server digMapInfos is null");
                return;
            }
            for (int i = 0; i < digMapInfos.Length; i++)
            {
                DigMapInfo mapInfo = digMapInfos[i];
                if (mapInfo == null)
                {
                    continue;
                }
                int mapId = (int)mapInfo.mapId;
                int goldDigNum = (int)mapInfo.goldDigSize;
                int silverDigNum = (int)mapInfo.silverDigSize;
                RandomTreasureMapModel model = mTotalMapModelDic[mapId];
                if (model != null)
                {
                    model.goldSiteNum = goldDigNum;
                    model.silverSiteNum = silverDigNum;
                }
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTreasureAtlasInfoSync);
        }

        /// <summary>
        /// 新增记录 不清本地缓存
        /// </summary>
        /// <param name="data"></param>
        private void _OnSyncDigRecordInfo(MsgDATA data)
        {
            WorldDigRecordInfoSync recordInfoSync = new WorldDigRecordInfoSync();
            recordInfoSync.decode(data.bytes);

            Logger.LogProcessFormat("[RandomTreasureDataManager] - _OnSyncDigRecordInfo WorldDigRecordInfoSync !!!");

            var netRecordInfo = recordInfoSync.info;
            if (mTotalMapRecordList == null)
            {
                return;
            }
            if(netRecordInfo == null)
            {
                return;
            }
            RandomTreasureMapRecordModel recordModel = new RandomTreasureMapRecordModel();
            recordModel.mapId = (int)netRecordInfo.mapId;
            if (mTotalMapModelDic != null && mTotalMapModelDic.ContainsKey(recordModel.mapId))
            {
                var mapModel = mTotalMapModelDic[recordModel.mapId];
                if (mapModel != null && mapModel.localMapData != null)
                {
                    recordModel.mapName = mapModel.localMapData.Name;
                }
            }
            recordModel.digIndex = (int)netRecordInfo.digIndex;
            recordModel.digType = (DigType)netRecordInfo.type;
            recordModel.playerId = netRecordInfo.playerId;
            recordModel.playerName = netRecordInfo.playerName;
            if (_NeedFilterRecordInfoByItemQuality((int)netRecordInfo.itemId))
            {
                return;
            }
            recordModel.itemSData = _GetItemSimpleDataByItemId((int)netRecordInfo.itemId, (int)netRecordInfo.itemNum);
            mTotalMapRecordList.Add(recordModel);

            Logger.LogProcessFormat("[RandomTreasureDataManager] - _OnSyncDigRecordInfo OnTreasureRecordInfoChanged");

            int timeStamp = (int)TimeManager.GetInstance().GetServerTime();
            RandomTreasureUIEvent uiEvent = new RandomTreasureUIEvent(timeStamp);
            uiEvent.EventID = EUIEventID.OnTreasureRecordInfoChanged;
            if(mDelayRefreshUIEventList != null)
            {
                mDelayRefreshUIEventList.Add(uiEvent);
            }
            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTreasureRecordInfoChanged);
        }

        /// <summary>
        /// 同步当前全部记录 需要重新清本地缓存 
        /// </summary>
        /// <param name="data"></param>
        private void _OnReqDigRecordRes(MsgDATA data)
        {
            WorldDigRecordsRes recordInfoRes = new WorldDigRecordsRes();
            recordInfoRes.decode(data.bytes);
            var netRecordInfos = recordInfoRes.infos;
            if (mTotalMapRecordList == null)
            {
                return;
            }
            //同步所有记录
            mTotalMapRecordList.Clear();
            if (netRecordInfos == null)
            {
                return;
            }
            for (int i = 0; i < netRecordInfos.Length; i++)
            {
                var netRecordInfo = netRecordInfos[i];
                if (netRecordInfos == null)
                {
                    continue;
                }
                RandomTreasureMapRecordModel recordModel = new RandomTreasureMapRecordModel();
                recordModel.mapId = (int)netRecordInfo.mapId;
                if (mTotalMapModelDic != null && mTotalMapModelDic.ContainsKey(recordModel.mapId))
                {
                    var mapModel = mTotalMapModelDic[recordModel.mapId];
                    if (mapModel != null && mapModel.localMapData != null)
                    {
                        recordModel.mapName = mapModel.localMapData.Name;
                    }
                }
                recordModel.digIndex = (int)netRecordInfo.digIndex;
                recordModel.digType = (DigType)netRecordInfo.type;
                recordModel.playerId = netRecordInfo.playerId;
                recordModel.playerName = netRecordInfo.playerName;
                if (_NeedFilterRecordInfoByItemQuality((int)netRecordInfo.itemId))
                {
                    continue;
                }
                recordModel.itemSData = _GetItemSimpleDataByItemId((int)netRecordInfo.itemId, (int)netRecordInfo.itemNum);
                mTotalMapRecordList.Add(recordModel);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTreasureRecordInfoSync);

            Logger.LogProcessFormat("[RandomTreasureDataManager] - _OnReqDigRecordRes UIEvent OnTreasureRecordInfoSync");
        }

        private void _OnMallBuyRes(MsgDATA data)
        {
            WorldMallBuyRet mallBuyRes = new WorldMallBuyRet();
            mallBuyRes.decode(data.bytes);

            if (mallBuyRes.code != (uint)ProtoErrorCode.SUCCESS)
            {
                //统一在ShopNewDataManager处理
                // SystemNotifyManager.SystemNotify((int)mallBuyRes.code);
            }
            else
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTreasureItemBuyRes);
            }
        }

        /// <summary>
        /// <=指定品质 过滤   
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
  
        private bool _NeedFilterRecordInfoByItemQuality(int itemId)
        {                   
            ItemData tempData = ItemDataManager.CreateItemDataFromTable(itemId);
            if ((int)tempData.Quality <= (int)mFilterRecordByQuality)
            {
                return true;
            }
            return false;
        }

        public bool _NeedFilterRecordInfoByItemQuality(ItemData itemData)
        {
            if (itemData == null)
            {
                return true;
            }
            if ((int)itemData.Quality <= (int)mFilterRecordByQuality)
            {
                return true;
            }
            return false;
        }

        #endregion

        #endregion

        #region  PUBLIC METHODS

        public Dictionary<int, RandomTreasureMapModel> GetTotalMapModelDic()
        {
            return mTotalMapModelDic;
        }

        public List<RandomTreasureMapModel> GetTotalMapModelList()
        {
            if (mTotalMapModelDic == null)
            {
                return null;
            }
            List<RandomTreasureMapModel> mapModelList = new List<RandomTreasureMapModel>();
            var enumerator = mTotalMapModelDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var model = enumerator.Current.Value as RandomTreasureMapModel;
                if (model != null)
                {
                    mapModelList.Add(model);
                }
            }
            return mapModelList;
        }

        public List<RandomTreasureMapModel> GetFilterMapModelList(RandomTreasureMapModel currMapModel)
        {
            if (mTotalMapModelDic == null)
            {
                return null;
            }
            List<RandomTreasureMapModel> mapModelList = new List<RandomTreasureMapModel>();
            var enumerator = mTotalMapModelDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var model = enumerator.Current.Value as RandomTreasureMapModel;
                if (model != null)
                {
                    if (currMapModel != null && model.mapId == currMapModel.mapId)
                    {
                        //过滤掉了
                        continue;
                    }
                    mapModelList.Add(model);
                }
            }
            return mapModelList;
        }

        /// <summary>
        /// 获取指定宝箱类型的地图数据
        /// </summary>
        /// <param name="type">指定宝箱类型</param>
        /// <param name="needFiler">是否需要过滤</param>
        /// <param name="currMapModel"> 是否包含当前地图数据 </param>
        /// <returns></returns>
        public List<RandomTreasureMapModel> GetTreasureTypeMapModelList(Protocol.DigType type, bool needFiler = false , RandomTreasureMapModel currMapModel = null)
        {
            if (mTotalMapModelDic == null)
            {
                return null;
            }
            List<RandomTreasureMapModel> mapModelList = new List<RandomTreasureMapModel>();
            var enumerator = mTotalMapModelDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var model = enumerator.Current.Value as RandomTreasureMapModel;
                if (model == null)
                {
                    continue;
                }
                //过滤
                if (needFiler && currMapModel != null && currMapModel.mapId == model.mapId)
                {
                    continue;
                }
                switch (type)
                {
                    case DigType.DIG_GLOD:
                        if (model.goldSiteNum > 0)
                        {
                            mapModelList.Add(model);
                        }
                        break;
                    case DigType.DIG_SILVER:
                        if (model.silverSiteNum > 0)
                        {
                            mapModelList.Add(model);
                        }
                        break;
                }
            }
            return mapModelList;
        }

        public int GetFirstMapId()
        {
            int firstMapId = 0;
            if (mTotalMapModelDic == null)
            {
                Logger.LogError("[RandomTreasureDataManager] - GetMapModelByMapId , mTotalMapModelDic is null");
                return firstMapId;
            }

            var enumerator = mTotalMapModelDic.Keys.GetEnumerator();
            while (enumerator.MoveNext())
            {
                int mapId = enumerator.Current;
                return mapId;
            }
            return firstMapId;
        }

        public RandomTreasureMapModel GetMapModelByMapId(int mapId)
        {
            if (mTotalMapModelDic == null)
            {
                Logger.LogError("[RandomTreasureDataManager] - GetMapModelByMapId , mTotalMapModelDic is null");
                return null;
            }
            RandomTreasureMapModel mapModel = null;
            mTotalMapModelDic.TryGetValue(mapId, out mapModel);
            return mapModel;
        }

        public RandomTreasureMapDigSiteModel GetMapSiteModelByMapIdAndSiteIndex(int mapId, int mapSiteIndex)
        {
            if (mTotalMapModelDic == null)
            {
                Logger.LogError("[RandomTreasureDataManager] - GetMapSiteModelByMapIdAndSiteIndex , mTotalMapModelDic is null");
                return null;
            }
            RandomTreasureMapDigSiteModel mapSiteModel = null;
            RandomTreasureMapModel mapModel = null;
            mTotalMapModelDic.TryGetValue(mapId, out mapModel);
            if (mapModel != null)
            {
                var siteModels = mapModel.mapTotalDigSites;
                if (siteModels == null)
                {
                    return mapSiteModel;
                }
                for (int i = 0; i < siteModels.Count; i++)
                {
                    if (siteModels[i].index == mapSiteIndex)
                    {
                        return siteModels[i];
                    }
                }
            }
            return mapSiteModel;
        }

        public List<RandomTreasureMapRecordModel> GetMapRecordInfoList()
        {
            return mTotalMapRecordList;
        }

        public void AddNetEventListener()
        {
            NetProcess.AddMsgHandler(WorldDigInfoSync.MsgID, _OnSyncChangedDigInfo);
            NetProcess.AddMsgHandler(WorldDigRefreshSync.MsgID, _OnSyncResetDigInfos);
            NetProcess.AddMsgHandler(WorldDigPlayerSizeSync.MsgID, _OnSyncMapPlayerNum);
            NetProcess.AddMsgHandler(WorldDigRecordInfoSync.MsgID, _OnSyncDigRecordInfo);
            NetProcess.AddMsgHandler(WorldMallBuyRet.MsgID, _OnMallBuyRes);
        }

        public void RemoveNetEventListener()
        {
            NetProcess.RemoveMsgHandler(WorldDigInfoSync.MsgID, _OnSyncChangedDigInfo);
            NetProcess.RemoveMsgHandler(WorldDigRefreshSync.MsgID, _OnSyncResetDigInfos);
            NetProcess.RemoveMsgHandler(WorldDigPlayerSizeSync.MsgID, _OnSyncMapPlayerNum);
            NetProcess.RemoveMsgHandler(WorldDigRecordInfoSync.MsgID, _OnSyncDigRecordInfo);
            NetProcess.RemoveMsgHandler(WorldMallBuyRet.MsgID, _OnMallBuyRes);
        }

        public void ReqOpenFirstTreasureMap()
        {
            int firstMapId = GetFirstMapId();
            if (firstMapId < 1)
            {
                Logger.LogErrorFormat("[RandomTreasureDataManager] - ReqOpenFirstTreasureMap firstId < 1", firstMapId);
                return;
            }
            WorldDigMapOpenReq msg = new WorldDigMapOpenReq();
            msg.mapId = (uint)firstMapId;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            Logger.LogProcessFormat("[RandomTreasureDataManager] - ReqOpenFirstTreasureMap map id is " + firstMapId);
        }

        public void ReqOpenTreasureMap(int mapId)
        {
            if (GetMapModelByMapId(mapId) == null)
            {
                Logger.LogErrorFormat("[RandomTreasureDataManager] - ReqOpenTreasureMap data is null, mapid is {0}", mapId);
                return;
            }
            WorldDigMapOpenReq msg = new WorldDigMapOpenReq();
            msg.mapId = (uint)mapId;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            Logger.LogProcessFormat("[RandomTreasureDataManager] - ReqOpenTreasureMap map id is " + mapId);
        }

        public void ReqOpenTreasureMap(RandomTreasureMapModel mapModel)
        {
            if (mapModel == null)
            {
                Logger.LogError("[RandomTreasureDataManager] - ReqOpenTreasureMap data is null");
                return;
            }
            WorldDigMapOpenReq msg = new WorldDigMapOpenReq();
            msg.mapId = (uint)mapModel.mapId;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            Logger.LogProcessFormat("[RandomTreasureDataManager] - RandomTreasureMapModel map id is " + mapModel.mapId);
        }

        public void ReqCloseTreasureMap(RandomTreasureMapModel mapModel)
        {
            if (mapModel == null)
            {
                Logger.LogError("[RandomTreasureDataManager] - ReqCloseTreasureMap data is null");
                return;
            }
            WorldDigMapCloseReq msg = new WorldDigMapCloseReq();
            msg.mapId = (uint)mapModel.mapId;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            Logger.LogProcessFormat("[RandomTreasureDataManager] - ReqCloseTreasureMap !!! mapId is " + mapModel.mapId);

            WaitNetMessageManager.GetInstance().Wait<WorldDigMapCloseRes>(msgRet =>
            {
                if (msgRet == null)
                {
                    return;
                }
                if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.result);
                }
                else
                {
                    Logger.LogProcessFormat("[RandomTreasureDataManager] - ReqCloseTreasureMap msgRet.result = " + msgRet.result);
                }
            });
        }

        public void ReqWatchTreasureSite(RandomTreasureMapDigSiteModel digSite)
        {
            if (digSite == null)
            {
                Logger.LogError("[RandomTreasureDataManager] - ReqWatchTreasureSite data is null");
                return;
            }
            WorldDigWatchReq msg = new WorldDigWatchReq();
            msg.mapId = (uint)digSite.mapId;
            msg.index = (uint)digSite.index;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            Logger.LogProcessFormat("[RandomTreasureDataManager] - ReqWatchTreasureSite !!! digSite index is " + digSite.index + ", mapId is " + digSite.mapId);
        }

        public void ReqOpenTreasureDigSite(RandomTreasureMapDigSiteModel digSite)
        {
            if (digSite == null)
            {
                Logger.LogError("[RandomTreasureDataManager] - ReqDigTreasureSite data is null");
                return;
            }
            WorldDigOpenReq msg = new WorldDigOpenReq();
            msg.mapId = (uint)digSite.mapId;
            msg.index = (uint)digSite.index;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            Logger.LogProcessFormat("[RandomTreasureDataManager] - ReqOpenTreasureDigSite !!! digSite index is " + digSite.index + ", mapId is " + digSite.mapId);
        }

        public void ReqTotalAtlasInfo()
        {
            WorldDigMapInfoReq msg = new WorldDigMapInfoReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            Logger.LogProcessFormat("[RandomTreasureDataManager] - ReqTotalAtlasInfo !!! ");
        }

        public void ReqMapRecordInfo()
        {
            WorldDigRecordsReq msg = new WorldDigRecordsReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            Logger.LogProcessFormat("[RandomTreasureDataManager] - ReqMapRecordInfo !!! ");
        }

        public void ReqFastMallBuy(int itemId)
        {
            WorldGetMallItemByItemIdReq msg = new WorldGetMallItemByItemIdReq();
            msg.itemId = (uint)itemId;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<WorldGetMallItemByItemIdRes>(msgRet =>
            {
                var mallItemInfo = msgRet.mallItem;
                ClientSystemManager.GetInstance().OpenFrame<MallBuyFrame>(FrameLayer.Middle, mallItemInfo);
            }, false);
        }

        public void SystemNotifyOnGetItem(ItemSimpleData rewardItem)
        {
            if (rewardItem != null)
            {
                string str = string.Format("{0} * {1}", rewardItem.Name, rewardItem.Count);
                SystemNotifyManager.SysNotifyFloatingEffect(str, ProtoTable.CommonTipsDesc.eShowMode.SI_QUEUE, rewardItem.ItemID);
            }
        }

        public bool CheckGetItemDataNeedNotifyRecord(ItemData itemData)
        {
            bool needNotifyRecord = false;
            needNotifyRecord = _NeedFilterRecordInfoByItemQuality(itemData) == false ;
            return needNotifyRecord;
        }

        public void SystemNotifyOnGetItem(ItemData rewardItem)
        {
            if (rewardItem != null)
            {
                string str = string.Format("{0} * {1}", rewardItem.Name, rewardItem.Count);
                SystemNotifyManager.SysNotifyFloatingEffect(str, ProtoTable.CommonTipsDesc.eShowMode.SI_QUEUE, rewardItem.TableID);
            }
        }

        /// <summary>
        /// 功能是否需要开启或者关闭
        /// </summary>
        /// <returns></returns>
        public bool IsServerSwitchFuncOn()
        {
            bool bServerSwitchOn = ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_DIG) != true;
            return bServerSwitchOn;
        }

        public void OpenRandomTreasureFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<RandomTreasureFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<RandomTreasureFrame>();
            }
            ClientSystemManager.GetInstance().OpenFrame<RandomTreasureFrame>(FrameLayer.Middle);
        }
    
        #endregion
    }
}