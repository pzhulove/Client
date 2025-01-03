using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Protocol;
using ProtoTable;
using Network;
using System.Diagnostics;
using System.Reflection;
using ActivityLimitTime;

namespace GameClient
{
    /// <summary>
    /// 关卡详情工具类
    ///
    /// 用于查询关卡是否开放
    /// 用于查询关卡战斗类型
    /// </summary>
    public class ChapterUtility
    {
        public const int kDeadTowerTopLevel = 70;
        public const int kDeadTowerLevelSplit = 5;
        public const int kDeadTowerLevelCount = kDeadTowerTopLevel / kDeadTowerLevelSplit;

        private static DungeonID mDungeonID = new DungeonID(0);
        private static Battle.DungeonOpenInfo mSearchOp = new Battle.DungeonOpenInfo();
        private const int kMaxDiff = 4;

        public static BattleType GetBattleType(int id)
        {
            DungeonTable node = TableManager.instance.GetTableItem<DungeonTable>(id);
            if (null != node)
            {
                switch (node.SubType)
                {
                    case DungeonTable.eSubType.S_TEAM_BOSS:
                    case DungeonTable.eSubType.S_NORMAL:
                        return BattleType.Dungeon;
                    case DungeonTable.eSubType.S_YUANGU:
                        return BattleType.YuanGu;
                    case DungeonTable.eSubType.S_HELL:
                        return BattleType.Hell;
                    case DungeonTable.eSubType.S_NIUTOUGUAI:
                        return BattleType.Mou;
                    case DungeonTable.eSubType.S_JINBI:
                        return BattleType.GoldRush;
                    case DungeonTable.eSubType.S_NANBUXIGU:
                        // return BattleType.North;
                        return BattleType.Dungeon;
                    case DungeonTable.eSubType.S_SIWANGZHITA:
                        return BattleType.DeadTown;
                    case DungeonTable.eSubType.S_NEWBIEGUIDE:
                        return BattleType.NewbieGuide;
                    case DungeonTable.eSubType.S_PK:
                        return BattleType.MutiPlayer;
                    case DungeonTable.eSubType.S_GUILDPK:
                        return BattleType.GuildPVP;
                    case DungeonTable.eSubType.S_MONEYREWARDS_PVP:
                        return BattleType.MoneyRewardsPVP;
                    case DungeonTable.eSubType.S_WUDAOHUI:
                        return BattleType.ChampionMatch;
                    case DungeonTable.eSubType.S_GUILD_DUNGEON:
                        return BattleType.GuildPVE;
                    case DungeonTable.eSubType.S_COMBOTRAINING:
                        return BattleType.TrainingSkillCombo;
                    case DungeonTable.eSubType.S_FINALTEST_PVE:
                        return BattleType.FinalTestBattle;
                    case DungeonTable.eSubType.S_RAID_DUNGEON:
                        return BattleType.RaidPVE;
                    case DungeonTable.eSubType.S_TREASUREMAP:
                        return BattleType.TreasureMap;
                }
            }

            return BattleType.Dungeon;
        }

        /// <summary>
        /// 获得普通关卡的剧情列表
        /// </summary>
        /// <param name="dungeonID"></param>
        /// <returns></returns>
        public static IList<int> PreconditionIDList(int dungeonID)
        {
            mDungeonID.dungeonID = dungeonID;

            int normalID = mDungeonID.dungeonIDWithOutDiff;
            
            DungeonTable preItem = TableManager.instance.GetTableItem<DungeonTable>(normalID);

            if (preItem != null)
            {
                return preItem.storyDungeonIDs;
            }
            else
            {
                Logger.LogError("Dungeon Table : no item with " + normalID);
            }

            return new List<int>();
        }

        /// <summary>
        /// 查询对应地下城的任务是否为完成
        /// 此处只有进行中的任务列表，没有已完成的任务列表
        /// </summary>
        /// <param name="dungeonID"></param>
        /// <returns></returns>
        public static bool GetDungeonMissionState(int dungeonID)
        {
            MissionManager.SingleMissionInfo info = MissionManager.GetInstance().GetMissionInfoByDungeonID(dungeonID);

            if (null != info)
            {
                switch ((TaskStatus)info.status)
                {
                    case TaskStatus.TASK_UNFINISH:
                        return true;
                }
            }

            return false;
        }

        public static bool HasMissionByDungeonID(int dungeonID)
        {
            return GetMissionDungeonDiff(dungeonID) >= 0;
        }

        public static int GetMissionDungeonDiff(int dungeonID)
        {
            DungeonID id = new DungeonID(dungeonID);

            int tophard = GetDungeonTopHard(dungeonID);

            for (int i = 0; i <= tophard; ++i)
            {
                id.diffID = i;

                if (GetDungeonMissionState(id.dungeonID))
                {
                    return i;
                }

            }

            return -1;
        }


        public static uint GetMissionIDByDungeonID(int dungoenID)
        {
            MissionManager.SingleMissionInfo value = MissionManager.GetInstance().GetMissionInfoByDungeonID(dungoenID);

            if (null != value)
            {
                return value.taskID;
            }

            return 0;
        }

        public static string GetDungeonMissionInfo(int dungeonID)
        {
            {
                MissionManager.SingleMissionInfo value = MissionManager.GetInstance().GetMissionInfoByDungeonID(dungeonID);

                return MissionManager.GetInstance().GetMissionName(value.taskID) + MissionManager.GetInstance().GetMissionNameAppendBystatus(value.status,value.missionItem.ID);

            }

            return string.Empty;
        }

        public static int GetReadyDungeonID(int dungeonID, int hard = 0 /* 该参数已经无效 */)
        {
            //hard %= kMaxDiffculteCount;
            mDungeonID.dungeonID = dungeonID;

            int diff     = mDungeonID.diffID;
            int normalID = mDungeonID.dungeonIDWithOutDiff;

            int readyDungeonID = GetDungeonID2Enter(normalID);
            if (readyDungeonID < 0)
            {
                //Logger.LogError("real dungeon id is invalid");
                return -1;
            }

            if (readyDungeonID == normalID)
            {
                mDungeonID.dungeonID = dungeonID;
                mDungeonID.diffID    = diff;

                readyDungeonID       = mDungeonID.dungeonID;
            }

            Logger.LogProcessFormat("[chapterutility] 原有ID {0} 真正ID {1}", dungeonID, readyDungeonID);
            
            DungeonTable item = TableManager.instance.GetTableItem<DungeonTable>(readyDungeonID);
            if (item != null)
            {
                return readyDungeonID;
            }

            // Logger.LogError("Dungeon Table : no item with " + readyDungeonID);
            return -1;
        }

        /// <summary>
        /// 获取实际最大难度值
        /// 如果只有解锁难度0，则无法查找到
        /// </summary>
        public static int GetDungeonRealTopHard(int dungeonID)
        {
            mDungeonID.dungeonID = dungeonID;

            List<Battle.DungeonHardInfo> hardList =  BattleDataManager.GetInstance().ChapterInfo.allHardInfo;

            Battle.DungeonHardInfo hard = hardList.Find(x=>{ return x.id == mDungeonID.dungeonIDWithOutDiff; });
            if (null != hard)
            {
                return hard.unlockedHardType;
            }

            return -1;
        }

        /// <summary>
        /// 获得表中最大的难度
        /// </summary>
        /// <param name="dungeonID"></param>
        /// <returns></returns>
        public static int GetDungeonTopHard(int dungeonID)
        {
            mDungeonID.dungeonID = dungeonID;

            for (int i = kMaxDiff - 1; i >= 0; --i)
            {
                mDungeonID.diffID = i;

                DungeonTable item = TableManager.instance.GetTableItem<DungeonTable>(mDungeonID.dungeonID);
                if (null != item)
                {
                    return i;
                }
            }

            return -1; 
        }

		public static bool CanDungeonOpenAutoFight(int dungeonID)
		{
			DungeonTable item = TableManager.instance.GetTableItem<DungeonTable>(dungeonID);
			if (item != null)
			{
				return item.OpenAutoFight > 0;
			}

			return false;
		}

        public static string GetHardString(int diff)
        {
            diff %= kMaxDiff;

            if (diff == 0) return "普通";
            else if (diff == 1) return "冒险";
            else if (diff == 2) return "勇士";
            else if (diff == 3) return "王者";

            return "";
        }

        public static string GetHardColorString(int diff)
        {
            diff %= kMaxDiff;

            if (diff == 0)      return "#ffffff";
            else if (diff == 1) return "#00bfff";
            else if (diff == 2) return "#bf00ff";
            else if (diff == 3) return "#ff00bf";

            return "#ffffff";
        }

        /// <summary>
        /// 组队的地下城id
        /// </summary>
        private static int[] mTeamDungeonIds = null;

        private static void _loadTeamDungeonIds()
        {
            if (null != mTeamDungeonIds)
            {
                return;
            }

            List<int> teamDungeonIds = new List<int>();

            var teamDungeonData = TableManager.instance.GetTable<TeamDungeonTable>();

            var dataIter = teamDungeonData.GetEnumerator();

            while (dataIter.MoveNext())
            {
                TeamDungeonTable data = dataIter.Current.Value as TeamDungeonTable;

                if (null != data)
                {
                    if (data.DungeonID > 0)
                    {
                        teamDungeonIds.Add(data.DungeonID);
                    }
                }
            }

            mTeamDungeonIds = teamDungeonIds.ToArray();
        }

        public static bool IsTeamDungeonID(int dungeonID)
        { 
            _loadTeamDungeonIds();

            if (null == mTeamDungeonIds)
            {
                return false;
            }

            for (int i = 0; i < mTeamDungeonIds.Length; i++)
            {
                if (dungeonID == mTeamDungeonIds[i])
                {
                    return true;
                }
            }

            return false;
        }
        private static int[] mPVEChapterIds = null;

        private static void _loadPVEChapterIds()
        {
            if (null != mPVEChapterIds)
            {
                return;
            }

            List<int> chapterList = new List<int>();

            var chapterTableData = TableManager.GetInstance().GetTable<ChapterTable>();

            var iter = chapterTableData.GetEnumerator();

            while (iter.MoveNext())
            {
                ChapterTable tb = iter.Current.Value as ChapterTable;

                chapterList.Add(tb.ID);
            }

            mPVEChapterIds = chapterList.ToArray();
        }

        public static bool IsPVEChapter(int chapterId)
        {
            _loadPVEChapterIds();

            if (null == mPVEChapterIds)
            {
                return false;
            }

            for (int i = 0; i < mPVEChapterIds.Length; i++)
            {
                if (mPVEChapterIds[i] == chapterId)
                {
                    return true;
                }
            }
               
            return false;
        }

        /// <summary>
        /// 根据难度，获得最新的地下城ID
        /// </summary>
        public static int GetLastedDungeonIDByDiff(int diff)
        {
            if (diff >= 0 && diff < 4)
            {
                List<Battle.DungeonOpenInfo> openList = BattleDataManager.GetInstance().ChapterInfo.openedList;

                for (int i = openList.Count - 1; i >= 0; --i)
                {
                    int id = openList[i].id;

                    DungeonID did = new DungeonID(id);

                    if (IsPVEChapter(did.chapterID))
                    {
                        int curtopHard = GetDungeonRealTopHard(id);
                        if (curtopHard >= diff)
                        {
                            did.diffID = diff;
                            return did.dungeonID;
                        }
                    }
                }
            }

            return -1;
        }

        public static int GetDungeonID2Enter(int dungeonID)
        {
           IList<int> preconditionList = PreconditionIDList(dungeonID);
            List<Battle.DungeonOpenInfo> openList = BattleDataManager.GetInstance().ChapterInfo.openedList;

            mSearchOp.id = dungeonID;

            int ix = openList.BinarySearch(mSearchOp);
            if (ix >= 0)
            {
                // 这里做一个容错，策划会调整线上任务关卡的映射关系，导致一些历史遗留问题，已通关的关卡对应的前哨站的任务如果还在，那么继续显示前哨站
                if(preconditionList != null && preconditionList.Count > 0)
                {
                    List<MissionManager.SingleMissionInfo> missions = MissionManager.GetInstance().GetAllTaskByType(MissionTable.eTaskType.TT_MAIN);
                    if(missions != null && missions.Count > 0)
                    {
                        for(int i = 0; i < missions.Count; i++)
                        {
                            if(missions[i].status == (int)TaskStatus.TASK_UNFINISH)
                            {
                                MissionTable MissionItem = TableManager.instance.GetTableItem<MissionTable>((int)missions[i].taskID);
                                if(MissionItem != null && MissionItem.MapID != 0)
                                {
                                    bool bFind = false;

                                    for(int j = 0; j < preconditionList.Count; j++)
                                    {
                                        if(preconditionList[j] == MissionItem.MapID)
                                        {
                                            bFind = true;
                                            break;
                                        }
                                    }

                                    if(bFind)
                                    {
                                        return MissionItem.MapID;
                                    }
                                }
                            }
                        }
                    }
                }

                return dungeonID;
            }

            for (int i = preconditionList.Count - 1; i >= 0; --i)
            {
                int item = preconditionList[i];
                if (item <= 0)
                {
                    continue;
                }

                mSearchOp.id = item;

                int idx = openList.BinarySearch(mSearchOp);
                if (idx >= 0)
                {
                    return item;
                }
            }

            return -1;
        }

        public static DungeonScore GetDungeonBestScore(int id)
        {
            Battle.DungeonInfo info = _isInAllInfo(id);

            if (null != info)
            {
                return (DungeonScore)info.bestScore;
            }

            return DungeonScore.C;
        }

        /// <summary>
        /// 关卡开启列表，包含难度
        /// </summary>
        private static Battle.DungeonInfo _isInAllInfo(int dungeonID)
        {
            List<Battle.DungeonInfo> allinfo = BattleDataManager.GetInstance().ChapterInfo.allInfo;
            Battle.DungeonInfo findItem = allinfo.Find(x => { return x.id == dungeonID; });
            return findItem;
        }

        /// <summary>
        /// 关卡开放列表，不含难度
        /// </summary>
        private static Battle.DungeonOpenInfo _isOpen(int dungeonID)
        {
            mDungeonID.dungeonID = dungeonID;
            List<Battle.DungeonOpenInfo> allOpen = BattleDataManager.GetInstance().ChapterInfo.openedList;
            Battle.DungeonOpenInfo fopen = allOpen.Find(x => { return x.id == mDungeonID.dungeonIDWithOutDiff; });
            return fopen;
        }

        /// <summary>
        /// dh
        /// </summary>
        /// <param name="dungeonID"></param>
        /// <returns></returns>
        public static bool IsCanComsumeFatigue(int dungeonID)
        {
            DungeonTable tab = TableManager.instance.GetTableItem<DungeonTable>(dungeonID);
            if (null != tab && tab.CostFatiguePerArea > PlayerBaseData.GetInstance().fatigue)
            {
                return false;
            }

            return true;
        }

        public static bool OpenComsumeFatigueAddFrame(int dungeonID)
        {
            if (!IsCanComsumeFatigue(dungeonID))
            {
                ClientSystemManager.instance.CloseFrame<ComsumeFatigueAddFrame>();
                ClientSystemManager.instance.OpenFrame<ComsumeFatigueAddFrame>();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 具体关卡是否可以进
        /// 不会计算其剧情关卡
        /// </summary>
        public static bool GetDungeonCanEnter(int id, bool withTips = true, bool checkTicket = true, bool withTicketGetLink = true)
        {
            DungeonTable tab = TableManager.instance.GetTableItem<DungeonTable>(id);
            if (null != tab)
            {
                if (tab.MinLevel > PlayerBaseData.GetInstance().Level)
                {
                    if (withTips)
                        SystemNotifyManager.SystemNotify(5007, (object)tab.MinLevel);
                    return false;
                }

                if (checkTicket && tab.TicketID > 0)
                {
                    ItemTable itemtable = TableManager.instance.GetTableItem<ItemTable>(tab.TicketID);
                    int ticketNum= tab.TicketNum;
                    int minusNum = 0;
                    if (tab.SubType==DungeonTable.eSubType.S_HELL_ENTRY)//深渊
                    {
                        if (ActivityDataManager.GetInstance().GettAnniverTaskIsFinish(EAnniverBuffPrayType.HellTicketMinus))
                        {
                            if (ActivityDataManager.GetInstance().IsLeftChallengeTimes(EAnniverBuffPrayType.HellTicketMinus, CounterKeys.DUNGEON_HELL_TIMES))
                            {
                                minusNum = ActivityDataManager.GetInstance().GetAnniverTaskValue(EAnniverBuffPrayType.HellTicketMinus);
                                ticketNum -= minusNum;
                            }

                        }
                    }
                    else if(tab.SubType==DungeonTable.eSubType.S_YUANGU)//远古
                    {
                        if (ActivityDataManager.GetInstance().GettAnniverTaskIsFinish(EAnniverBuffPrayType.YuanGUTicketMinus))
                        {
                            if (ActivityDataManager.GetInstance().IsLeftChallengeTimes(EAnniverBuffPrayType.HellTicketMinus, CounterKeys.DUNGEON_YUANGU_TIMES))
                            {
                                minusNum = ActivityDataManager.GetInstance().GetAnniverTaskValue(EAnniverBuffPrayType.YuanGUTicketMinus);
                                ticketNum -= minusNum;
                            }
                        }
                    }
                    if(ticketNum<0)
                    {
                        Logger.LogError(string.Format("消耗的深渊票数量小于0，周年祈福buff减少的票数为：{0}",minusNum));
                    }
                    if (null != itemtable)
                    {
                        int cnt = ItemDataManager.GetInstance().GetOwnedItemCount(tab.TicketID);

                        if (cnt < ticketNum)
                        {
                            if (withTips)
                            {
                                SystemNotifyManager.SystemNotify(5009, (object)itemtable.Name, (object)ticketNum);
                                if (withTicketGetLink)
                                {
                                    //add by mjx on 170821 for limitTime gift in mall
                                    if (tab.SubType == DungeonTable.eSubType.S_YUANGU)
                                    {
                                        ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.TryShowMallGift(MallGiftPackActivateCond.NO_ANCIENT_TICKET, _onOpenYuanGuLinkFrame);
                                    }
                                    else if (tab.SubType == DungeonTable.eSubType.S_HELL_ENTRY)
                                    {
                                        ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.TryShowMallGift(MallGiftPackActivateCond.NO_HELL_TICKET, _onOpenHellLinkFrame);
                                    }
                                    else if (tab.SubType == DungeonTable.eSubType.S_WEEK_HELL_ENTRY)
                                    {
                                        //周常深渊票不足的时候，打开界面
                                        OpenWeekHellCostLinkFrame(tab.TicketID);
                                    }
                                }
                            }
                            return false;
                        }
                    }
                }
            }

            ComChapterDungeonUnit.eState st = GetDungeonState(id);

            if (st == ComChapterDungeonUnit.eState.Locked)
            {
                if (withTips)
                    SystemNotifyManager.SystemNotify(1009);
                return false;
            }

            return true;
        }

        //打开周常深渊票不足的界面
        private static void OpenWeekHellCostLinkFrame(int itemId)
        {
            ItemComeLink.OnLink(itemId, 0, false, null, false, true);
        }

        private static void _onOpenYuanGuLinkFrame()
        {
            Logger.LogProcessFormat("打开远古获取途径");
            ItemComeLink.OnLink(200000003, 0, false, null, false, true);
        }

        private static void _onOpenHellLinkFrame()
        {
            Logger.LogProcessFormat("打开深渊获取途径");
            ItemComeLink.OnLink(200000004, 0, false, null, false, true);
        }

        /// <summary>
        /// 获得关卡的状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ComChapterDungeonUnit.eState GetDungeonState(int id)
        {
            if (id > 0)
            {
                mDungeonID.dungeonID = id;

                int idnodiff = mDungeonID.dungeonIDWithOutDiff;
                int diff = mDungeonID.diffID;
                bool isPre = mDungeonID.prestoryID > 0;

                if (null != _isInAllInfo(id))
                {
                    return isPre ? ComChapterDungeonUnit.eState.LockPassed : ComChapterDungeonUnit.eState.Passed;
                }
                else if (null != _isOpen(idnodiff))
                {
                    int topHard = GetDungeonRealTopHard(idnodiff);

                    if (diff == 0 || topHard >= diff)
                    {
                        return ComChapterDungeonUnit.eState.Unlock;
                    }
                }
                else 
                {
                    return ComChapterDungeonUnit.eState.Locked;
                }
            }
            return ComChapterDungeonUnit.eState.Locked;
        }

        public static bool IsHell(int dungeonID)
        {
            List<Battle.DungeonOpenInfo> openList = BattleDataManager.GetInstance().ChapterInfo.openedList;
            mSearchOp.id = dungeonID;

            int ix = openList.BinarySearch(mSearchOp);
            if (ix >= 0)
            {
                return openList[ix].isHell;
            }

            return false;
        }

        public static KeyValuePair<int, int> NextDungeonIDWithMission(int dungeonID)
        {
            mDungeonID.dungeonID = dungeonID;

            int diff      = mDungeonID.diffID;
            int idnodiff  = mDungeonID.dungeonIDWithOutDiff;
            int idnopre   = mDungeonID.dungeonIDWithOutPrestory;

            int nextId    = -1;
            int missionId = -1;

            if (idnodiff != idnopre)
            {
                DungeonTable normalData = TableManager.instance.GetTableItem<DungeonTable>(idnopre);
                if (null != normalData)
                {
                    IList<int> list = normalData.storyDungeonIDs;
                    int index = list.IndexOf(idnodiff);
                    int maxIndex = list.Count - 1;

                    //missionId = normalData.PreTaskID;

                    if (maxIndex >= 0 && index >= 0)
                    {
                        if (index == maxIndex)
                        {
                            nextId = idnopre + diff;
                        }
                        else
                        {
                            nextId = list[index + 1] + diff;
                        }
                    }

                    DungeonTable newData = TableManager.instance.GetTableItem<DungeonTable>(nextId);
                    if (null != newData)
                    {
                        missionId = newData.storyTaskID;
                    }
                    else
                    {
                        Logger.LogErrorFormat("the nextID can't find in DugeonTable with ID {0}", nextId);
                    }
                }
            }

            Logger.LogProcessFormat("get next dungeon id {0}, with mission id {1}", nextId, missionId);

            return new KeyValuePair<int, int>(nextId, missionId);
        }

        public static ComChapterDungeonUnit.eMissionType GetMissionType(int missionID)
        {
            MissionTable missionTable = TableManager.instance.GetTableItem<MissionTable>(missionID);
            if (null != missionTable)
            {
                switch (missionTable.TaskType)
                {
    
                    case MissionTable.eTaskType.TT_MAIN:
                    case MissionTable.eTaskType.TT_CHANGEJOB:
                        return ComChapterDungeonUnit.eMissionType.Main;
                    case MissionTable.eTaskType.TT_BRANCH:
                        return ComChapterDungeonUnit.eMissionType.Other;
                    default:
                        return ComChapterDungeonUnit.eMissionType.None;
                }
            }

            return ComChapterDungeonUnit.eMissionType.None;
        }

        public static IClientFrame OpenChapterFrameByID(int dungeonID, GameObject root = null)
        {
            Logger.LogProcessFormat("打开界面 : {0}", dungeonID);

            ChapterBaseFrame.sDungeonID = dungeonID;

            DungeonID id = new DungeonID(dungeonID);

            DungeonUIConfigTable item = TableManager.instance.GetTableItem<DungeonUIConfigTable>(id.dungeonIDWithOutDiff);

            IClientFrame openFrame = null;

            if (null != item)
            {
                System.Type type = TypeTable.GetType(item.ClassName);
                if (null != type)
                {
                    openFrame = ClientSystemManager.instance.OpenFrame(type, root);
                }
                else
                {
                    Logger.LogErrorFormat("can't find {0}", item.ClassName);
                }
            }
            else
            {
                Logger.LogErrorFormat("ChapterNormalFrame不存在,已经被删掉了,请找王博.id:{0},id.dungeonIDWithOutDiff:{1}",id, id.dungeonIDWithOutDiff);

                // ChapterNormalFrame经过确认没有地方在用了。就先把预制体和代码删掉，如果发现还有用，到时候再加回来 by Wangbo 2020.07.22
                //openFrame = ClientSystemManager.instance.OpenFrame(typeof(ChapterNormalFrame), root);
            }

            _enterChapterInfoStat(dungeonID);

            return openFrame;
        }

        public static KeyValuePair<int, int> GetChapterRewardByChapterIdx(int chid)
        {
            List<MissionManager.SingleMissionInfo> allMission = ChapterUtility.FilterMissionInfoByChapterIdx(MissionTable.eSubType.Dungeon_Mission, chid);

            if (allMission == null)
            {
                Logger.LogErrorFormat("[关卡宝箱] allMission == null, childindex = {0}", chid);
                return new KeyValuePair<int, int>();
            }

            int sum    = allMission.Count;
            int finish = 0;

            for (int i = 0; i < allMission.Count; ++i)
            {
                if (allMission[i].status == (int)Protocol.TaskStatus.TASK_OVER)
                {
                    finish++;
                }
            }

            return new KeyValuePair<int, int>(finish, sum);
        }

        /// <summary>
        /// 这个是返回的当前s的进度
        /// </summary>
        /// <param name="chid"></param>
        /// <returns></returns>
        public static KeyValuePair<int, int> GetChapterRewardByChapterIdxNew(int chid)
        {
            List<MissionManager.SingleMissionInfo> allMission = ChapterUtility.FilterMissionInfoByChapterIdx(MissionTable.eSubType.Dungeon_Mission, chid);

            int sum = allMission.Count * 3;
            int finish = 0;

            for (int i = 0; i < allMission.Count; ++i)
            {
                MissionManager.SingleMissionInfo cur = allMission[i];
                uint curTaskID = cur.taskID;
                string tempScore = GameClient.MissionManager.GetInstance().GetMissionValueByKey(curTaskID, "score");//读当前分数
                string tempParam = GameClient.MissionManager.GetInstance().GetMissionValueByKey(curTaskID, "parameter");//读当前分数
                int score = -1;
                int param = -1;
                int.TryParse(tempScore, out score);
                int.TryParse(tempParam, out param);
                if(score != 0)
                {
                    switch (score)
                    {
                        case (int)Protocol.DungeonScore.SSS:
                            finish += 3;
                            break;
                        case (int)Protocol.DungeonScore.SS:
                            finish += 2;
                            break;
                        case (int)Protocol.DungeonScore.S:
                            finish += 1;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch(param)
                    {
                        case 1:
                            finish += 3;
                            break;
                        default:
                            break;
                    }
                }
            }

            return new KeyValuePair<int, int>(finish, sum);
        }

        public static int GetChapterCanGetByChapterIdx(int chid)
        {
            List<MissionManager.SingleMissionInfo> allMission = ChapterUtility.FilterMissionInfoByChapterIdx(MissionTable.eSubType.Dungeon_Mission, chid);

            if (allMission == null)
            {
                return 0;
            }

            int sum = allMission.Count;
            int finish = 0;

            for (int i = 0; i < allMission.Count; ++i)
            {
                if (allMission[i].status == (int)Protocol.TaskStatus.TASK_FINISHED)
                {
                    finish++;
                }
            }
            return finish;
            //return new KeyValuePair<int, int>(finish, sum);
        }

        public static List<MissionManager.SingleMissionInfo> FilterMissionInfoByChapterIdx(MissionTable.eSubType subType, int chid)
        {
            //int chid = ChapterSelectFrame.GetCurrentSelectChapter();
            if (chid <= 0)
            {
                return null;
            }

            List<MissionManager.SingleMissionInfo> allReward = MissionManager.GetInstance().GetAllTaskByType(MissionTable.eTaskType.TT_ACHIEVEMENT, new MissionTable.eSubType[] {subType});

            if(allReward != null && allReward.Count <= 0)
            {
                Logger.LogErrorFormat("[关卡宝箱] 总章节奖励未配置,subType = {0}, chidx = {1}", subType, chid);
            }

            allReward.RemoveAll(x=>
            {
                int v = -1;
                if (int.TryParse(x.missionItem.MissionParam, out v))
                {
                    return v != chid;
                }
                else 
                {
                    return true;
                }
            });

            return allReward;
        }

        public static List<MissionManager.SingleMissionInfo> FilterMissionInfoByCurChapter(MissionTable.eSubType subType)
        {
            return FilterMissionInfoByChapterIdx(subType, ChapterSelectFrame.GetCurrentSelectChapter());
        }


        public static bool IsChapterCanGetChapterReward(int chid)
        {
            if (chid <= 0)
            {
                return false;
            }

            List<MissionManager.SingleMissionInfo> allReward = MissionManager.GetInstance().GetAllTaskByType(MissionTable.eTaskType.TT_ACHIEVEMENT, new MissionTable.eSubType[] {MissionTable.eSubType.Dungeon_Chest, MissionTable.eSubType.Dungeon_Mission});

            allReward.RemoveAll(x=>
            {
                int v = -1;
                if (int.TryParse(x.missionItem.MissionParam, out v))
                {
                    if ((v == chid) && (x.status == (byte)Protocol.TaskStatus.TASK_FINISHED))
                    {
                        return false;
                    }
                }

                return true;
            });

            return allReward.Count > 0;
        }

        /// <summary>
        /// 章节是否开放
        /// </summary>
        public static bool IsChapterOpen(int chapterID)
        {
            DungeonID dungeonID = new DungeonID(0);

            List<Battle.DungeonInfo> allinfo = BattleDataManager.GetInstance().ChapterInfo.allInfo;
            Battle.DungeonInfo findItem = allinfo.Find(x => 
            { 
                dungeonID.dungeonID = x.id;
                return dungeonID.chapterID == chapterID; 
            });

            if (null != findItem)
            {
                return true;
            }

            List<Battle.DungeonOpenInfo> allOpen = BattleDataManager.GetInstance().ChapterInfo.openedList;
            Battle.DungeonOpenInfo fopen = allOpen.Find(x => 
            { 
                dungeonID.dungeonID = x.id;
                return dungeonID.chapterID == chapterID; 
            });

            if (null != fopen)
            {
                return true;
            }

            return false;
        }

        private static Dictionary<int, int> mCitySceneID2ChapterIdx = new Dictionary<int, int>();

        public static bool IsChapterOpenBySceneID(int sceneID)
        {
            int dungeonId = _getChapterIndexBySceneID(sceneID);

			if (dungeonId < 0)   
            {
                return false;
            }

			int realid = GetDungeonID2Enter (dungeonId);
			return GetDungeonCanEnter(realid, false, false);
        }
        /// <summary>
        /// 判读精英地下城是否解锁
        /// </summary>
        /// <param name="sceneID"></param>
        /// <returns></returns>
        public static bool IsEliteChapterOpenBySceneId(int sceneID)
        {
            bool result = false;
            CitySceneTable table = TableManager.instance.GetTableItem<CitySceneTable>(sceneID);

            if (null == table)
            {
                return result;
            }
            bool isFind = false;
            for (int i = 0; i < table.ChapterData.Count; i++)
            {
                DChapterData data = AssetLoader.instance.LoadRes(table.ChapterData[i], typeof(DChapterData)).obj as DChapterData;

                if (null == data)
                {
                    return result;
                }

                for (int j = 0; j < data.chapterList.Length; j++)
                {
                    int dungeonID = data.chapterList[j].dungeonID;
                    if (TeamUtility.IsEliteDungeonID(dungeonID))
                    {
                        ComChapterDungeonUnit.eState eState = GetDungeonState(dungeonID);
                        if (eState != ComChapterDungeonUnit.eState.Locked)
                        {
                            result = true;
                            isFind = true;
                            break;
                        }
                    }
                }
                if(isFind)
                {
                    break;
                }
            }
            return result;
        }
        private static int _getChapterIndexBySceneID(int sceneID)
        {
            if (mCitySceneID2ChapterIdx.ContainsKey(sceneID))
            {
                return mCitySceneID2ChapterIdx[sceneID];
            }

            CitySceneTable table = TableManager.instance.GetTableItem<CitySceneTable>(sceneID);

            if (null == table)
            {
                return -1;
            }

            DChapterData data = AssetLoader.instance.LoadRes(table.ChapterData[0], typeof(DChapterData)).obj as DChapterData;

            if (null ==  data)
            {
                return -1;
            }

            if (data.chapterList.Length > 0)
            {
                DungeonID id = new DungeonID(data.chapterList[0].dungeonID);

				mCitySceneID2ChapterIdx.Add(sceneID, id.dungeonIDWithOutDiff);

				return id.dungeonIDWithOutDiff;
            }

            return -1;
        }

       
        public static int[] GetAllChapterRewardChapters()
        {
            List<MissionManager.SingleMissionInfo> allReward = MissionManager.GetInstance().GetAllTaskByType(MissionTable.eTaskType.TT_ACHIEVEMENT, new MissionTable.eSubType[] {MissionTable.eSubType.Dungeon_Chest});

            List<int> allChapters = new List<int>();

            for (int i = 0; i < allReward.Count; ++i)
            {
                int v = -1;
                if (int.TryParse(allReward[i].missionItem.MissionParam, out v))
                {
                    allChapters.Add(v);
                }
            }

            allChapters.Sort();

            return allChapters.ToArray();
        }

        public static bool IsChapterCanGetChapterRewards()
        {
            int[] chapters = GetAllChapterRewardChapters();

            for (int i = 0; i < chapters.Length; ++i)
            {
                if (IsChapterCanGetChapterReward(chapters[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private static void _enterChapterInfoStat(int id)
        {
            mDungeonID.dungeonID = id;
            GameStatisticManager.instance.DoStatLevelChoose(StatLevelChooseType.CHOOSE_LEVEL, ChapterSelectFrame.sSceneID, mDungeonID.dungeonID, mDungeonID.diffID); 
        }


        private static Dictionary<int, int> mHellEnter = null;

        public static bool HellIsPass(int dungeonId)
        {
            _updateDungeonHellSplit();

            DungeonTable data = TableManager.instance.GetTableItem<DungeonTable>(dungeonId);

            if (data.SubType == DungeonTable.eSubType.S_HELL_ENTRY)
            {
                return GetDungeonState(dungeonId) == ComChapterDungeonUnit.eState.Passed;
            }

            if (data.SubType == DungeonTable.eSubType.S_HELL)
            {
                return GetDungeonState(_getHellEnterId(dungeonId)) == ComChapterDungeonUnit.eState.Passed;
            }

            return false;
        }

        /// <summary>
        /// 获得原始地下城id
        /// 如果是深渊返回深渊入口
        /// </summary>
        /// <param name="dungeonId"></param>
        /// <returns></returns>
        public static int GetOriginDungeonId(int dungeonId)
        {
            _updateDungeonHellSplit();

            DungeonTable data = TableManager.instance.GetTableItem<DungeonTable>(dungeonId);

            if (null == data)
            {
                return -1;
            }

            if (data.SubType == DungeonTable.eSubType.S_WEEK_HELL)
            {
                return data.ownerEntryId;
            }

            if (data.SubType != DungeonTable.eSubType.S_HELL)
            {
                return dungeonId;
            }

            return _getHellEnterId(dungeonId);
        }

        private static void _updateDungeonHellSplit()
        {
            if (null != mHellEnter)
            {
                return;
            }

            mHellEnter = new Dictionary<int, int>();

            var dict = TableManager.instance.GetTable<DungeonTable>();

            var keyIter = dict.Keys.GetEnumerator();

            DungeonID did = new DungeonID(0);

            while (keyIter.MoveNext())
            {
                int id = keyIter.Current;
                DungeonTable data = dict[id] as DungeonTable;

                if (null != data)
                {
                    if (data.SubType == DungeonTable.eSubType.S_HELL_ENTRY)
                    {
                        if (!mHellEnter.ContainsKey(data.HellSplitLevel))
                        {
                            did.dungeonID = data.ID;
                            mHellEnter.Add(data.HellSplitLevel, did.dungeonIDWithOutDiff);
                        }
                    }
                }
            }
        }

        private static int _getHellEnterId(int dungeonId)
        {
            DungeonID did = new DungeonID(dungeonId);

            int diff = did.diffID;

            DungeonTable data = TableManager.instance.GetTableItem<DungeonTable>(dungeonId);

            if (null == data)
            {
                return 0;
            }

            if (!mHellEnter.ContainsKey(data.HellSplitLevel))
            {
                return 0;
            }

            did.dungeonID = mHellEnter[data.HellSplitLevel];

            did.diffID = diff;

            return did.dungeonID;
        }
    }
}
