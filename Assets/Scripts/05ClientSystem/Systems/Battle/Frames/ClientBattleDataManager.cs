using System.Collections.Generic;
using Protocol;
using Network;
using System;
using System.Text;
using System.Diagnostics;
using GameClient;

namespace Battle
{
    public class DungeonItem : IComparable<DungeonItem>
    {
        public enum eType
        {
            Invalid,

            SmallDrug               = 100,
            MediumDrug,
            BigDrug,
            LeiMiDrug,

            SmallColorlessCrystals  = 200,
            Glod                    = 300,
            Ticket,
            Exp,
            DeadCoin,
            MonsterCoin,
            RebornCoin,

            Lvl1HPDrug              = 400,
            Lvl2HPDrug,
            Lvl3HPDrug,
            Lvl4HPDrug,
            Lvl5HPDrug,
            Lvl6HPDrug,

            Lvl1MPDrug              = 500,
            Lvl2MPDrug,
            Lvl3MPDrug,
            Lvl4MPDrug,
            Lvl5MPDrug,
            Lvl6MPDrug,
        }

        private int mId;

        public int id
        {
            get
            {
                return mId;
            }

            set
            {
                mId = value;

                type = eType.Invalid;

                try
                {
                    type = (eType)TableManager.instance.GetItemConfigID(mId);
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                }
            }
        }

        public int num;

        public eType type
        {
            get; private set;
        }

        public int CompareTo(DungeonItem item)
        {
            return id - item.id;
        }
    }

    public class DungeonBuff : IComparable<DungeonBuff>
    {
        public enum eBuffDurationType
        {
            Town = 0,
            Battle = 1,
            OnlyUseInBattle = 2,            //只在战斗中生效
            SpecialTown = 3,                //特殊城镇Buff(给服务器做区分用)
        }

        // the id number from the server
        public ulong uid = 0;
        // buffid, index the BuffTable
        public int id = -1;
        public int overlay = -1;
        public float duration = -1.0f;

        public float lefttime = -1.0f;

        public eBuffDurationType type = eBuffDurationType.Town;

        public bool readymove = false;

        public int CompareTo(DungeonBuff buff)
        {
            if (id == buff.id)
            {
                if (uid > buff.uid)
                {
                    return 1;
                }
                else if (uid < buff.uid)
                {
                    return -1;
                }

                return 0;
            }

            return id - buff.id;
        }
    }

    public class DungeonDropItem : IComparable<DungeonDropItem>
    {
        public int id        = -1;
        public int typeId    = -1;
        public int num       = -1;

        /// <summary>
        /// 是否是双倍
        /// </summary>
        public bool isDouble = false;
        public int strenthLevel = -1;
        public EEquipType equipType;

        public int CompareTo(DungeonDropItem other)
        {
            return id - other.id;
        }
    }

    public class DungeonMonster : IComparable<DungeonMonster>
    {
        public int id = -1;
        /// <summary>
        /// 点ID
        /// </summary>
        public int pointId = -1;
		public CrypticInt32 typeId = -1;
        public List<DungeonDropItem> dropItems = new List<DungeonDropItem>();
        public List<int> prefixes = new List<int>();
        /// <summary>
        /// 父节点ID
        /// </summary>
        public int summonerId = -1;
        /// <summary>
        /// 该节点死亡之后需要召唤的怪物列表
        /// </summary>
        public List<DungeonMonster> summonerMonsters = null;

        public bool removed = false;

		/// <summary>
		/// 怪物类型
		/// </summary>
		public DEntityType monsterType = DEntityType.MONSTER;

        public int CompareTo(DungeonMonster other)
        {
            return id - other.id;
        }
    }

    public class DungeonHellWaveInfo : IComparable<DungeonHellWaveInfo>
    {
        public int wave;
        public List<DungeonMonster> monsters = new List<DungeonMonster>();

        public int CompareTo(DungeonHellWaveInfo other)
        {
            return wave - other.wave;
        }

        public DungeonHellWaveInfo Duplicate()
        {
            DungeonHellWaveInfo wave = new DungeonHellWaveInfo();
            return wave;
        }
    }

    public enum eDungeonHellState
    {
        None,
        Ready,
        Start,
        Monster1,
        Monster2Pre,
        Monster2,
        End,
    }

    public class DungeonHellInfo
    {
        public DungeonHellMode mode;
        public int areaId;
        public List<DungeonHellWaveInfo> waveInfos = new List<DungeonHellWaveInfo>();
        public List<DungeonDropItem> dropItems = new List<DungeonDropItem>();
        public bool isFinish = false;
        public eDungeonHellState state = eDungeonHellState.None;

        public DungeonHellInfo Duplicate()
        {
            DungeonHellInfo info = new DungeonHellInfo();

            info.mode      = mode;
            info.areaId    = areaId;
            info.waveInfos = new List<DungeonHellWaveInfo>(waveInfos);
            info.dropItems = new List<DungeonDropItem>(dropItems);
            info.isFinish  = isFinish;
            info.state     = state;

            return info;
        }
    }
    public class DungeonAreaMonsterSubGroup : IComparable<DungeonAreaMonsterSubGroup>
    {
        public DungeonAreaMonsterSubGroup(int id)
        {
            this.id = id;
        }

        public int id { get; private set; }

        public List<DungeonMonster> monsters = new List<DungeonMonster>();

        public int CompareTo(DungeonAreaMonsterSubGroup other)
        {
            return id - other.id;
        }
    }

    public class DungeonArea : IComparable<DungeonArea>
    {
        public int id = -1;
        /// <summary>
        /// 原始数据
        /// </summary>
        private List<DungeonMonster> monsters = new List<DungeonMonster>();

        /// <summary>
        /// 分组数据
        ///
        /// 从原始数据来
        /// </summary>
        private List<DungeonAreaMonsterSubGroup> monsterGroups = new List<DungeonAreaMonsterSubGroup>();
        public List<DungeonMonster> destructs = new List<DungeonMonster>();
		public List<int> regions = new List<int>();


        #region 原始数据接口

        /// <summary>
        /// 添加怪物原始数据
        /// </summary>
        public void AddMonster(DungeonMonster monster)
        {
            monsters.Add(monster);
        }

        /// <summary>
        /// 原始数据数目
        /// </summary>
        public int MonsterCount
        {
            get
            {
                return monsters.Count;
            }
        }

        /// <summary>
        /// 根据索引 获得原始数据
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public DungeonMonster GetMonsterAt(int idx)
        {
            if (idx < 0 || idx >= MonsterCount)
            {
                return null;
            }

            return monsters[idx];
        }
        #endregion


        #region 怪物分组


        public IList<DungeonMonster> FirstMonsterList
        {
            get
            {
                return GetMonsterListAt(0);
            }
        }

        public IList<DungeonMonster> GetMonsterListAt(int idx)
        {
            int cnt = MonsterListCount;
            if (idx >= cnt || idx < 0)
            {
                return Array.Empty<DungeonMonster>();
            }

            return monsterGroups[idx].monsters;
        }

        public int MonsterListCount
        {
            get
            {
                if (null == monsterGroups)
                {
                    return 0;
                }

                return monsterGroups.Count;
            }
        }

        public void AddSubGroupMonster(int subGroupID, DungeonMonster m)
        {
            var list = GetMonstersBySubGroupID(subGroupID);
            if (null == list)
            {
                DungeonAreaMonsterSubGroup subGroup = new DungeonAreaMonsterSubGroup(subGroupID);
                monsterGroups.Add(subGroup);
                list = subGroup.monsters;
            }

            if (null == list)
            {
                return;
            }

            list.Add(m);
        }

        public void SortMonsterSubGroup()
        {
            if (null == monsterGroups)
            {
                return;
            }

            monsterGroups.Sort();
        }

        private List<DungeonMonster> GetMonstersBySubGroupID(int subGroupID)
        {
            for (int i = 0; i < monsterGroups.Count; ++i)
            {
                if (monsterGroups[i].id == subGroupID)
                {
                    return monsterGroups[i].monsters;
                }
            }

            return null;
        }
        #endregion

        public int CompareTo(DungeonArea other)
        {
            return id - other.id;
        }

        public DungeonArea Duplicate()
        {
            DungeonArea dupArea = new DungeonArea();

            dupArea.id          = id;
            dupArea.monsters    = new List<DungeonMonster>(monsters);
            dupArea.destructs   = new List<DungeonMonster>(destructs);
			dupArea.regions     = new List<int>(regions);

            return dupArea;
        }
    }

    public class DungeonAddMonsterDropItem : IComparable<DungeonAddMonsterDropItem>
    {
        public int monsterId = -1;
        public List<DungeonDropItem> dropItems = new List<DungeonDropItem>();

        public int CompareTo(DungeonAddMonsterDropItem other)
        {
            return monsterId - other.monsterId;
        }
    }

    /// <summary>
    /// 翻牌的消耗数据
    /// </summary>
    public class DungeonChestInfo
    {
        /// <summary>
        /// 道具ID
        /// </summary>
        public uint itemId = 0;

        /// <summary>
        /// 数目
        /// </summary>
        public uint count  = 0;
    }

    public class BattleInfo
    {
#region 原有从协议转过来的数据
        /// <summary>
        /// 地下城ID
        /// </summary>
        public int dungeonId = -1;

        /// <summary>
        /// 验证秘钥3
        /// </summary>
        public UInt32 key3;

        /// <summary>
        /// 开始的AreaID
        /// </summary>
        public int startAreaId = -1;

        /// <summary>
        /// 开始游戏前的随机种子，用于深渊的房间随机等
        /// </summary>
        public uint randomSeed = 0x12345678;

        /// <summary>
        /// 当前所在房间的区域AreaID
        /// </summary>
        public int areaId = -1;

        /// <summary>
        /// 验证秘钥4
        /// </summary>
        public UInt32 key4;

        /// <summary>
        /// 最佳纪录
        /// </summary>
        public int bestRecordTime = -1;

        /// <summary>
        /// 最大连击数
        /// </summary>
        public int maxComboCount = -1;

        /// <summary>
        /// 所有区域的列表
        /// </summary>
        public List<DungeonArea> areas = new List<DungeonArea>();

        /// <summary>
        /// 附加的怪物掉落
        /// </summary>
        public List<DungeonAddMonsterDropItem> addMonsterDropItem = new List<DungeonAddMonsterDropItem>();

        /// <summary>
        /// 验证秘钥2
        /// </summary>
        public UInt32 key2;

        /// <summary>
        /// 深渊的信息
        /// </summary>
        public DungeonHellInfo dungeonHealInfo = new DungeonHellInfo();

        /// <summary>
        /// 所有的掉落列表
        /// </summary>
        public List<DungeonDropItem> dropItems = new List<DungeonDropItem>();

        /// <summary>
        /// 结算之后的BOSS掉落的物品
        /// </summary>
        public List<DungeonDropItem> bossDropItems = new List<DungeonDropItem>();

        /// <summary>
        /// 验证秘钥1
        /// </summary>
        public UInt32 key1;

        /// <summary>
        ///  掉落次数用完的怪
        /// </summary>
        public List<UInt32> dropOverMonster = new List<UInt32>();
        #endregion


        #region 缓存的数据
        /// <summary>
        /// 结算掉落的倍数
        /// </summary>
        public byte endRaceDropMulti;

        /// <summary>
        /// 所有掉落出来的东西
        /// </summary>
        public List<DungeonDropItem> dropCacheItemIds = new List<DungeonDropItem>();

        /// <summary>
        /// 从怪物掉出来的
        /// </summary>
        public List<int> dropItemIds = new List<int>();

        /// <summary>
        /// 
        /// </summary>
        public List<int> pickedItems = new List<int>();

        /// <summary>
        /// 杀死的怪物
        /// </summary>
        public List<int> killedMonsters = new List<int>();
       
        public void KillMonster(int id)
        {
            if (id < 0)
            {
                return;
            }

            int idx = killedMonsters.BinarySearch(id);
            if (idx < 0)
            {
                killedMonsters.Insert(~idx, id);
            }
            else
            {
                //Logger.LogErrorFormat("already contain with id {0}", id);
            }
        }
#endregion
    }

    public class DungeonInfo : IComparable<DungeonInfo>
    {
        public int id;
        public int bestScore;
        public int bestRecordTime;

        public int CompareTo(DungeonInfo other)
        {
            return id - other.id;
        }
    }

    public class DungeonHardInfo : IComparable<DungeonHardInfo>
    {
        public int id;
        public int unlockedHardType;

        public int CompareTo(DungeonHardInfo other)
        {
            return id - other.id;
        }
    }

    public class DungeonOpenInfo : IComparable<DungeonOpenInfo>
    {
        public int id;
        public bool isHell;

        public int CompareTo(DungeonOpenInfo other)
        {
            return id - other.id;
        }
    }

    public class ChapterInfo
    {
        public List<DungeonInfo>     allInfo     = new List<DungeonInfo>();
        public List<DungeonHardInfo> allHardInfo = new List<DungeonHardInfo>();

        public List<DungeonOpenInfo> openedList = new List<DungeonOpenInfo>();

        /// <summary>
        /// 地下城关卡是否开启
        /// </summary>
        /// <param name="dungeonID">地下城ID</param>
        /// <param name="isUnlock">是否解锁</param>
        /// <param name="topHard">最大难度</param>
        /// <returns>是否成功执行，即改地下城ID是否有效</returns>
        public bool IsOpen(int dungeonID, out bool isUnlock, out int topHard)
        {
            isUnlock = false;
            topHard = -1;

            var dungeonTable = TableManager.instance.GetTableItem<ProtoTable.DungeonTable>(dungeonID);
            if (null == dungeonTable)
            {
                Logger.LogWarningFormat("can't find item in DungeonTable with id {0}", dungeonID);
                return false;
            }

            // 去除最后一位，去除难度
            dungeonID = dungeonID / 10 * 10;

            DungeonOpenInfo mSearch = new DungeonOpenInfo() { id = dungeonID };
            isUnlock = openedList.BinarySearch(mSearch) >= 0;

            if (isUnlock)
            {
                topHard = GetTopHard(dungeonID);
            }

            return true;
        }

        /// <summary>
        /// 获得最大难度
        /// </summary>
        /// <param name="dungeonID">地下城ID</param>
        /// <returns>最大难度</returns>
        public int GetTopHard(int dungeonID)
        {
            var hardList = allHardInfo;
            var topHard = 0;

            var idx = hardList.BinarySearch(new Battle.DungeonHardInfo { id = dungeonID / 1000 * 1000 });

            if (idx >= 0)
            {
                topHard = hardList[idx].unlockedHardType;
            }

            return topHard;
        }
    }

    public class DungeonDoor
    {
        public bool isconnectwithboss       = false;
        public bool isvisited         		= false;
        public ISceneTransportDoorData door = null;
		public TransportDoorType doorType   = TransportDoorType.None;
        public bool isEggDoor = false;
        public string materialPath = "";
    }
}

namespace GameClient
{
    /// <summary>
    /// 战斗关卡相关数据
    /// </summary>
    [LoggerModel("GlobalBattleData")]
    public class BattleDataManager : DataManager<BattleDataManager>
    {
        public Battle.BattleInfo BattleInfo = new Battle.BattleInfo();
        public Battle.ChapterInfo ChapterInfo = new Battle.ChapterInfo();
        public RacePlayerInfo[] PlayerInfo = new RacePlayerInfo[0];
        public Battle.DungeonChestInfo chestInfo = new Battle.DungeonChestInfo();

        public RollItemInfo[] rollRewardItem = new RollItemInfo[0];  //结算完后所有的需要roll点的服务器数据
        public List<Battle.DungeonDropItem> mAllRollRewardItems = new List<Battle.DungeonDropItem>(); //生成在地图上待roll点的道具数据(单人roll点也放入里面)
        public List<ComItemList.Items> mSelfRollItems = new List<ComItemList.Items>(); //roll点后属于自己的道具 给结算界面使用

        private RaceType pkRaceType;

        public UInt64 originExp = 0;

        public override void Initialize()
        {
            RegisterNetHandler();
        }

        public override void Clear()
        {
            ChapterInfo = new Battle.ChapterInfo();
            PlayerInfo = new RacePlayerInfo[0];
           
            pkRaceType = RaceType.Dungeon;
            ClearBatlte();

            UnRegisterNetHandler();
        }

        void RegisterNetHandler()
        {
            NetProcess.AddMsgHandler(SceneDungeonSyncNewOpenedList.MsgID, _onSceneDungeonSyncNewOpenedList);
            NetProcess.AddMsgHandler(SceneDungeonInit.MsgID, _onSceneDungeonInit);
            NetProcess.AddMsgHandler(SceneDungeonHardInit.MsgID, _onSceneDungeonHardInit);
            NetProcess.AddMsgHandler(SceneDungeonHardUpdate.MsgID, _onSceneDungeonHardUpdate);
            NetProcess.AddMsgHandler(SceneDungeonUpdate.MsgID, _onSceneDungeonUpdate);
            NetProcess.AddMsgHandler(SceneDungeonAddMonsterDropItemNotify.MsgID, _onSceneDungeonAddMonsterDropItemNotify);
            NetProcess.AddMsgHandler(WorldDungeonRollItemRes.MsgID, _OnDungeonRollItemRes);
            NetProcess.AddMsgHandler(WorldDungeonRollItemResult.MsgID, _OnDungonRollItemStatistic);
        }

        void UnRegisterNetHandler()
        {
            NetProcess.RemoveMsgHandler(SceneDungeonSyncNewOpenedList.MsgID, _onSceneDungeonSyncNewOpenedList);
            NetProcess.RemoveMsgHandler(SceneDungeonInit.MsgID, _onSceneDungeonInit);
            NetProcess.RemoveMsgHandler(SceneDungeonHardInit.MsgID, _onSceneDungeonHardInit);
            NetProcess.RemoveMsgHandler(SceneDungeonHardUpdate.MsgID, _onSceneDungeonHardUpdate);
            NetProcess.RemoveMsgHandler(SceneDungeonUpdate.MsgID, _onSceneDungeonUpdate);
            NetProcess.RemoveMsgHandler(SceneDungeonAddMonsterDropItemNotify.MsgID, _onSceneDungeonAddMonsterDropItemNotify);
            NetProcess.RemoveMsgHandler(WorldDungeonRollItemRes.MsgID, _OnDungeonRollItemRes);
            NetProcess.RemoveMsgHandler(WorldDungeonRollItemResult.MsgID, _OnDungonRollItemStatistic);
        }
        private void _OnDungeonRollItemRes(MsgDATA msg)
        {
            WorldDungeonRollItemRes res = new WorldDungeonRollItemRes();
            res.decode(msg.bytes);
            if (res.result == 0)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUpateRollItem, res);
            }
        }
        public class ResultRollItem
        {
            public ItemData item = null;
            public RollItemInfo itemData = null;
            public List<RollDropResultItem> playerInfoes = new List<RollDropResultItem>();
        }
        private void _OnDungonRollItemStatistic(MsgDATA msg)
        {
            mSelfRollItems.Clear();
         //   Logger.LogError("_OnDungonRollItemStatistic");
            if (rollRewardItem == null) return;
            WorldDungeonRollItemResult res = new WorldDungeonRollItemResult();
            res.decode(msg.bytes);
            var resultInfo = res.items;
            if (resultInfo == null) return;
            List<ResultRollItem> resultList = new List<ResultRollItem>();
         //   Logger.LogErrorFormat("_OnDungonRollItemStatistic Len {0}", resultInfo.Length);
            //生成roll点以后的结果数据给 其他界面使用
            for (int i = 0; i < rollRewardItem.Length; i++)
            {
                var result = new ResultRollItem();
                resultList.Add(result);
                var curRollRewordItem = rollRewardItem[i];
                result.itemData = curRollRewordItem;
                var curDropItem = curRollRewordItem.dropItem;
                if (curDropItem != null)
                {
                    result.item = GameClient.ItemDataManager.CreateItemDataFromTable((int)curDropItem.itemId);
                }
                int maxScore = 0;
                int maxIndex = 0;
                for (int j = 0; j < resultInfo.Length; j++)
                {
                    var curResultInfo = resultInfo[j];
                    if (curRollRewordItem.rollIndex == curResultInfo.rollIndex)
                    {
                        result.playerInfoes.Add(curResultInfo);
                        if (maxScore < (int)curResultInfo.point)
                        {
                            maxScore = (int)curResultInfo.point;
                            maxIndex = j;
                        }
                    }
                }
             //   Logger.LogErrorFormat("RollItemResultList {0} {1} {2}", i, result.itemData != null ? result.itemData.rollIndex : -1, result.itemData.dropItem != null ? result.itemData.dropItem.itemId : 99999999);
                //for (int j = 0; j < result.playerInfoes.Count; j++)
                //{
                //    Logger.LogErrorFormat("playerName {3} opType {2} playerId {1} point {0} rollIndex {4}",
                //    result.playerInfoes[j].point,
                //    result.playerInfoes[j].playerId,
                //    result.playerInfoes[j].opType,
                //    result.playerInfoes[j].playerName,
                //    result.playerInfoes[j].rollIndex);
                //}

                if (maxIndex < resultInfo.Length)
                {
                    if (resultInfo[maxIndex].playerId == PlayerBaseData.GetInstance().RoleID)
                    {
                        if (curDropItem != null)
                        {
                            //生成属于自己的道具
                            mSelfRollItems.Add(new ComItemList.Items
                            {
                                count = (uint)curDropItem.num,
                                id = (int)curDropItem.itemId,
                                type = ComItemList.eItemType.Custom,
                                strenthLevel = curDropItem.strenth,
                                equipType = (EEquipType)curDropItem.equipType
                            });
                        }

                    }
                }
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRollItemEnd, resultList);
        }
        //[MessageHandle(SceneDungeonSyncNewOpenedList.MsgID)]
        private void _onSceneDungeonSyncNewOpenedList(MsgDATA data)
        {
            SceneDungeonSyncNewOpenedList rep = new SceneDungeonSyncNewOpenedList();
            rep.decode(data.bytes);

            Logger.LogProcessFormat(ObjectDumper.Dump(rep));
            BattleDataManager.GetInstance().DungeonOpenList(rep);
        }

        //[MessageHandle(SceneDungeonInit.MsgID)]
        private void _onSceneDungeonInit(MsgDATA data)
        {
            SceneDungeonInit rep = new SceneDungeonInit();
            rep.decode(data.bytes);

            Logger.LogProcessFormat(ObjectDumper.Dump(rep));
            BattleDataManager.GetInstance().DungeonInit(rep);
        }

       // [MessageHandle(SceneDungeonHardInit.MsgID)]
        private void _onSceneDungeonHardInit(MsgDATA data)
        {
            SceneDungeonHardInit rep = new SceneDungeonHardInit();
            rep.decode(data.bytes);

            Logger.LogProcessFormat(ObjectDumper.Dump(rep));
            BattleDataManager.GetInstance().DungeonHardInit(rep);
        }

        //[ProtocolHandle(typeof(SceneDungeonUpdate))]
        private void _onSceneDungeonUpdate(/*SceneDungeonUpdate rep*/MsgDATA data)
        {
            SceneDungeonUpdate rep = new SceneDungeonUpdate();
            rep.decode(data.bytes);

            Logger.LogProcessFormat(ObjectDumper.Dump(rep));
            BattleDataManager.GetInstance().DungeonUpdate(rep);
        }
       
        //[ProtocolHandle(typeof(SceneDungeonHardUpdate))]
        private void _onSceneDungeonHardUpdate(/*SceneDungeonHardUpdate rep*/MsgDATA data)
        {
            SceneDungeonHardUpdate rep = new SceneDungeonHardUpdate();
            rep.decode(data.bytes);

            Logger.LogProcessFormat(ObjectDumper.Dump(rep));

            BattleDataManager.GetInstance().DungeonHardUpdate(rep);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonUnlockDiff, (int)rep.info.id, (int)rep.info.unlockedHardType);
        }

        //[ProtocolHandle(typeof(SceneDungeonAddMonsterDropItemNotify))]
        private void _onSceneDungeonAddMonsterDropItemNotify(/*SceneDungeonAddMonsterDropItemNotify res*/MsgDATA data)
        {
            SceneDungeonAddMonsterDropItemNotify res = new SceneDungeonAddMonsterDropItemNotify();
            res.decode(data.bytes);

            Logger.Log(ObjectDumper.Dump(res));
            BattleDataManager.GetInstance().DungeonAddMonsterDropItemNotify(res);
        }

        public void ClearBatlte()
        {
            BattleInfo = new Battle.BattleInfo();
            chestInfo  = new Battle.DungeonChestInfo();
            rollRewardItem = new RollItemInfo[0];
            mAllRollRewardItems.Clear();
            mSelfRollItems.Clear();

        }

        #region Private Methmod
        private void _updateAllInfo(SceneDungeonInfo repInfo)
        {
            var chapterInfo = ChapterInfo.allInfo;

            var info = new Battle.DungeonInfo()
            {
                id = (int)repInfo.id,
                bestScore = repInfo.bestScore,
                bestRecordTime = (int)repInfo.bestRecordTime
            };

            int idx = chapterInfo.BinarySearch(info);
            if (idx < 0)
            {
                chapterInfo.Insert(~idx, info);
            }
            else
            {
                chapterInfo[idx] = info;
            }
        }

        private void _updateHardInfo(SceneDungeonHardInfo repInfo)
        {
            var chapterInfo = ChapterInfo.allHardInfo;

            var info = new Battle.DungeonHardInfo()
            {
                id = (int)repInfo.id,
                unlockedHardType = repInfo.unlockedHardType,
            };


            int idx = chapterInfo.BinarySearch(info);
            if (idx < 0)
            {
                chapterInfo.Insert(~idx, info);
            }
            else
            {
                chapterInfo[idx] = info;
            }
        }
        #endregion

        private Battle.DungeonOpenInfo sSearchOpenOp = new Battle.DungeonOpenInfo();

        public RaceType PkRaceType
        {
            get
            {
                return pkRaceType;
            }

            set
            {
                pkRaceType = value;
            }
        }

        public void DungeonOpenList(SceneDungeonSyncNewOpenedList rep)
        {
            for (int i = 0; i < rep.newOpenedList.Length; ++i)
            {
                var data = rep.newOpenedList[i];
                sSearchOpenOp.id = (int)data.id;

                int idx = ChapterInfo.openedList.BinarySearch(sSearchOpenOp);
                if (idx < 0)
                {
                    ChapterInfo.openedList.Insert(~idx, new Battle.DungeonOpenInfo()
                    {
                        id = (int)data.id,
                        isHell = data.hellMode == 1
                    });
                    Logger.LogProcessFormat("openList add item {0}", data);
                }
                else
                {
                    ChapterInfo.openedList[idx].isHell = data.hellMode == 1;
                }
            }

            for (int i = 0; i < rep.newClosedList.Length; ++i)
            {
                var data = (int)rep.newClosedList[i];
                ChapterInfo.openedList.RemoveAll(x=> { return x.id == data; });
                Logger.LogProcessFormat("openList remove item {0}", data);
            }

            Logger.LogProcessFormat("openList {0}", ObjectDumper.Dump(ChapterInfo.openedList));
        }

        public void DungeonInit(SceneDungeonInit rep)
        {
            for (int i = 0; i < rep.allInfo.Length; ++i)
            {
                _updateAllInfo(rep.allInfo[i]);
            }
        }

        public void DungeonUpdate(SceneDungeonUpdate rep)
        {
            _updateAllInfo(rep.info);
        }

        public void DungeonHardInit(SceneDungeonHardInit rep)
        {
            for (int i = 0; i < rep.allInfo.Length; ++i)
            {
                _updateHardInfo(rep.allInfo[i]);
            }
        }

        public void DungeonHardUpdate(SceneDungeonHardUpdate rep)
        {
            _updateHardInfo(rep.info);
        }

        private void _addMonsterDropItem(Battle.DungeonAddMonsterDropItem item)
        {
            var monsterList = BattleInfo.addMonsterDropItem;
            var idx = monsterList.BinarySearch(item);
            if (idx < 0)
            {
                monsterList.Insert(~idx, item);
            }
            else
            {
                var resDropList = item.dropItems;
                resDropList.GetEnumerator();
                var dropList = monsterList[idx].dropItems;
                for (int i = 0; i < resDropList.Count; ++i)
                {
                    dropList.Add(resDropList[i]);
                }
            }
        }

        public void DungeonAddMonsterDropItemNotify(SceneDungeonAddMonsterDropItemNotify notify)
        {
            var addDropItem = new Battle.DungeonAddMonsterDropItem()
            {
                monsterId = (int)notify.monsterId
            };

            for (int i = 0; i < notify.dropItems.Length; i++)
            {
                var notifyDropItem = notify.dropItems[i];
                addDropItem.dropItems.Add(new Battle.DungeonDropItem()
                {
                    id = (int)notifyDropItem.id,
                    typeId = (int)notifyDropItem.typeId,
                    num = (int)notifyDropItem.num,
                    isDouble = notifyDropItem.isDouble == 1,
                    strenthLevel = notifyDropItem.strenth
                });
            }

            _addMonsterDropItem(addDropItem);
        }

        #region Start

        private void _convertDungeonDropItem(List<Battle.DungeonDropItem> battleDrops, SceneDungeonDropItem[] protocolDrops)
        {
            if (null != battleDrops && null != protocolDrops)
            {
                for (int i = 0; i < protocolDrops.Length; ++i)
                {
                    SceneDungeonDropItem   dropItem       = protocolDrops[i];

                    Battle.DungeonDropItem battleDropItem = new Battle.DungeonDropItem()
                    {
                        id = (int)dropItem.id,
                        typeId = (int)dropItem.typeId,
                        num = (int)dropItem.num,
                        isDouble = dropItem.isDouble == 1,
                        strenthLevel = dropItem.strenth,
                        equipType = (EEquipType)dropItem.equipType
                    };

                    battleDrops.Add(battleDropItem);

                    if (null != BattleInfo.dropItems)
                    {
                        BattleInfo.dropItems.Add(battleDropItem);
                    }
                }
            }
        }

        private Battle.DungeonMonster _convertOneDungeonMonster(SceneDungeonMonster resMonster)
        {
            var monster = new Battle.DungeonMonster()
            {
                id         = (int)resMonster.id,
                pointId    = (int)resMonster.pointId,
                typeId     = (int)resMonster.typeId,
                summonerId = (int)resMonster.summonerId
            };

            if (null != resMonster.prefixes)
            {
                for (int k = 0; k < resMonster.prefixes.Length; ++k)
                {
                    monster.prefixes.Add(resMonster.prefixes[k]);
                }
            }

            _convertDungeonDropItem(monster.dropItems, resMonster.dropItems);

            return monster;
        }
        public void ConvertPKBattleInfo(WorldNotifyRaceStart res)
        {
            BattleInfo = new Battle.BattleInfo();
            BattleInfo.dungeonId = (int)res.dungeonId;
        }
        public void ConvertDungeonBattleEndInfo(SceneDungeonRaceEndRes res)
        {
            mSelfRollItems.Clear();
            mAllRollRewardItems.Clear();
            rollRewardItem = res.rollReward;
            if (rollRewardItem == null) return;
            //Logger.LogErrorFormat("ConvertDungeonBattleEndInfo rollRewardItem Len {0}", rollRewardItem.Length);
            for (int i = 0; i < rollRewardItem.Length; i++)
            {
                var curItem = rollRewardItem[i];
                if (curItem == null) continue;
                var curDropItem = curItem.dropItem;
                if (curDropItem == null) continue;
                Battle.DungeonDropItem bdrop = new Battle.DungeonDropItem();
                bdrop.id = -1;
                bdrop.num = (int)curDropItem.num;
                bdrop.typeId = (int)curDropItem.itemId;
                bdrop.isDouble = false;
                bdrop.strenthLevel = curDropItem.strenth;
                bdrop.equipType = (EEquipType)curDropItem.equipType;
                mAllRollRewardItems.Add(bdrop);
               // Logger.LogErrorFormat("rollRewardItem {0} {1} {2} {3}", i, curDropItem.itemId, curDropItem.equipType, curDropItem.strenth, curDropItem.num);
            }
            if (res.rollSingleReward == null) return;
          //  Logger.LogErrorFormat("ConvertDungeonBattleEndInfo rollSingleReward Len {0}", res.rollSingleReward.Length);
            for (int i = 0; i < res.rollSingleReward.Length; i++)
            {
                var curItem = res.rollSingleReward[i];
                if (curItem == null) continue;
                mAllRollRewardItems.Add(new Battle.DungeonDropItem
                {
                    id = -1,
                    num = (int)curItem.num,
                    typeId = (int)curItem.id,
                    isDouble = false,
                    strenthLevel = curItem.strength,
                    equipType = (EEquipType)curItem.equipType
                });

                mSelfRollItems.Add(new ComItemList.Items
                {
                    count = (uint)curItem.num,
                    id = (int)curItem.id,
                    type = ComItemList.eItemType.Custom,
                    strenthLevel = curItem.strength,
                    equipType = (EEquipType)curItem.equipType
                });
                //Logger.LogErrorFormat("rollSingleReward {0} {1} {2} {3}", i, curItem.id, curItem.equipType, curItem.strength, curItem.num);
            }
        }
        public void ConvertDungeonBattleInfo(SceneDungeonStartRes res)
        {
            BattleInfo             = new Battle.BattleInfo();
            BattleInfo.dungeonId   = (int)res.dungeonId;
            BattleInfo.startAreaId = (int)res.startAreaId;

            BattleInfo.key4        = res.key4;
            BattleInfo.key1        = res.key1;
            BattleInfo.key3        = res.key3;
            BattleInfo.key2        = res.key2;

            // boss掉落
            _convertDungeonDropItem(BattleInfo.bossDropItems, res.bossDropItems);

            var passedList = ChapterInfo.allInfo;
            for (int i = 0; i < passedList.Count; ++i)
            {
                if (passedList[i].id == res.dungeonId)
                {
                    BattleInfo.bestRecordTime = passedList[i].bestRecordTime;
                    break;
                }
            }

            var resAreaList = res.areas;
            var areaList    = BattleInfo.areas;

            for (int i = 0; i < resAreaList.Length; i++)
            {
                var resArea = resAreaList[i];
                var resMonsterList = resArea.monsters;
                var monsterList = new List<Battle.DungeonMonster>();
                var destructList = new List<Battle.DungeonMonster>();
                var area = new Battle.DungeonArea();

                for (int j = 0; j < resArea.destructs.Length; ++j)
                {
                    var monster = _convertOneDungeonMonster(resArea.destructs[j]);
                    destructList.Add(monster);
                }

                for (int j = 0; j < resMonsterList.Length; j++)
                {
                    var monster = _convertOneDungeonMonster(resMonsterList[j]);
                    area.AddMonster(monster);

                    // 可破坏物刷怪
                    if (monster.summonerId > 0)
                    {
                        var finditem = destructList.Find(x => { return x.id == monster.summonerId; });
                        if (null != finditem)
                        {
                            if (null == finditem.summonerMonsters)
                            {
                                finditem.summonerMonsters = new List<Battle.DungeonMonster>();
                            }

                            finditem.summonerMonsters.Add(monster);
                        }
                    }
                }

                area.id = (int)resArea.id;
                area.destructs = destructList;

                areaList.Add(area);
            }

            // 深渊刷怪
            DungeonHellInfo rhellInfo = res.hellInfo;
            Battle.DungeonHellInfo hellInfo = BattleInfo.dungeonHealInfo;
            hellInfo.mode = (DungeonHellMode)rhellInfo.mode;

            if (hellInfo.mode != DungeonHellMode.Null)
            {
                hellInfo.areaId = (int)rhellInfo.areaId;

                DungeonHellWaveInfo rwaveInfo;
                Battle.DungeonHellWaveInfo waveInfo;

                for (int i = 0; i < rhellInfo.waveInfoes.Length; ++i)
                {
                    rwaveInfo = rhellInfo.waveInfoes[i];

                    waveInfo = new Battle.DungeonHellWaveInfo();
                    waveInfo.wave = i;

                    for (int j = 0; j < rwaveInfo.monsters.Length; ++j)
                    {
                        waveInfo.monsters.Add(_convertOneDungeonMonster(rwaveInfo.monsters[j]));
                    }

                    hellInfo.waveInfos.Add(waveInfo);
                }

                _convertDungeonDropItem(hellInfo.dropItems, rhellInfo.dropItems);
                //for (int i = 0; i < rhellInfo.dropItems.Length; ++i)
                //{
                //    hellInfo.dropItems.Add(new Battle.DungeonDropItem()
                //    {
                //        id = (int)rhellInfo.dropItems[i].id,
                //        num = (int)rhellInfo.dropItems[i].num,
                //        typeId = (int)rhellInfo.dropItems[i].typeId,
                //    });
                //}
            }

            // 保存该关卡掉落用完的怪
            if (BattleInfo.dropOverMonster != null)
            {
                BattleInfo.dropOverMonster.Clear();

                if(res.dropOverMonster != null)
                {
                    for (int i = 0; i < res.dropOverMonster.Length; i++)
                    {
                        BattleInfo.dropOverMonster.Add(res.dropOverMonster[i]);
                    }
                }
            }

            Logger.LogProcessFormat(ObjectDumper.Dump(BattleInfo));
        }
        #endregion

        /// <summary>
        /// 获取本地玩家信息
        /// </summary>
        public RacePlayerInfo GetLocalPlayerInfo()
        {
            if (PlayerInfo == null) return null;
            for (int i = 0; i < PlayerInfo.Length; i++)
            {
                if (PlayerInfo[i].accid == ClientApplication.playerinfo.accid)
                {
                    return PlayerInfo[i];
                }
            }
            return null;
        }
    }
}
