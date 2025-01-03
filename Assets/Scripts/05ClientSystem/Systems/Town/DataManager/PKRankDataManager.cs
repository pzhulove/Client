using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Network;
using Protocol;
using ProtoTable;
using UnityEngine.Assertions;

namespace GameClient
{
    enum EPromotionState
    {
        Invalid,
        Promoting,
        Successed,
        Failed,
    }

    class PromotionInfo
    {
        public EPromotionState eState;

        /// <summary>
        /// 需要赢得次数
        /// </summary>
        public int nTargetWinCount;

        /// <summary>
        /// 总次数
        /// </summary>
        public int nTotalCount;

        /// <summary>
        /// 当前胜利次数
        /// </summary>
        public int nCurrentWinCount;

        /// <summary>
        /// 当前失败次数
        /// </summary>
        public int nCurrentLoseCount;

        /// <summary>
        /// 记录战斗胜败
        /// </summary>
        public List<byte> arrRecords;

        /// <summary>
        /// 晋级成功的段位
        /// </summary>
        public int nNextSeasonLevel;
    }

    class SeasonLevel
    {
        /// <summary>
        /// 赛季段位表格数据
        /// </summary>
        public SeasonLevelTable levelTable;

        /// <summary>
        /// 段位属性数据
        /// </summary>
        public SeasonAttrTable attrTable;

        /// <summary>
        /// 格式化后的属性
        /// </summary>
        public string strFormatAttr;
    }

    class SeasonDataManager : DataManager<SeasonDataManager>
    {
        /// <summary>
        /// 赛季索引
        /// </summary>
        public int seasonID { get; private set; }

        /// <summary>
        /// 赛季结束时间戳
        /// </summary>
        public int seasonEndTime { get; private set; }

        /// <summary>
        /// 赛季属性所对应的赛季ID
        /// </summary>
        public int seasonAttrMappedSeasonID { get; set; }

        /// <summary>
        /// 赛季属性ID
        /// </summary>
        public int seasonAttrID { get; set; }

        /// <summary>
        /// 赛季属性更新时间戳
        /// </summary>
        public int seasonAttrEndTime { get; set; }

        

        /// <summary>
        /// 升段战胜败记录
        /// </summary>
        public List<byte> seasonUplevelRecords { get; set; }

        /// <summary>
        /// 段位ID
        /// </summary>
        public int seasonLevel { get; set; }

        /// <summary>
        /// 老的段位ID
        /// </summary>
        public int seasonOldLevel { get; set; }

        public int seasonStar { get; set; }

        public int seasonOldStar { get; set; }

        public int seasonExp { get; set; }

        public int seasonOldExp { get; set; }

        /// <summary>
        /// 赛季段位排名
        /// </summary>
        public int seasonRank { get; private set; }

        SeasonPlayStatus m_eSeasonPlayStatus = SeasonPlayStatus.SPS_INVALID;
        List<SeasonLevel> m_arrSeasonRewards = new List<SeasonLevel>();
        List<SeasonLevel> m_arrSeasonAttrs = new List<SeasonLevel>();

        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.PkRankData;
        }

        public override void Initialize()
        {
            seasonID = 1;
            seasonEndTime = 0;
            seasonLevel = 0;
            seasonOldLevel = 0;
            seasonStar = 0;
            seasonOldStar = 0;
            seasonExp = 0;
            seasonOldExp = 0;
            seasonRank = -1;
            seasonAttrMappedSeasonID = 0;
            seasonAttrID = 0;
            seasonAttrEndTime = 0;
            _InitSeasonRewards();
            _InitSeasonAttrs();

            _BindNetMsg();
            _BindGameEvent();
        }

        public override void Clear()
        {
            seasonID = 1;
            seasonEndTime = 0;
            seasonLevel = 0;
            seasonOldLevel = 0;
            seasonStar = 0;
            seasonOldStar = 0;
            seasonExp = 0;
            seasonOldExp = 0;
            seasonRank = -1;
            seasonAttrMappedSeasonID = 0;
            seasonAttrID = 0;
            seasonAttrEndTime = 0;
            _ClearSeasonRewards();
            _ClearSeasonAttrs();

            _UnBindNetMsg();
            _UnBindGameEvent();
        }

        public int GetMaxRankID()
        {
            // TODO
            return 65000;
        }

        public int GetMinRankID()
        {
            // TODO
            return 14501;
        }

        public void GetPreLevel(int a_nLevel, int a_nStar, int a_nExp, out int a_nPreLevel, out int a_nPreStar, out int a_nPreExp)
        {
            int nOffset = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>(
                (int)ProtoTable.SystemValueTable.eType.SVT_SEASON_WIN_STREAK_EXP).Value;
            a_nPreLevel = a_nLevel;
            a_nPreStar = a_nStar;
            while (a_nExp < nOffset)
            {
                SeasonLevelTable tableData = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(a_nPreLevel);
                if (tableData == null)
                {
                    Logger.LogErrorFormat("赛季段位表 找不到ID：{0}", a_nPreLevel);
                    break;
                }

                if (a_nLevel == GetMaxRankID() && a_nStar > 1)
                {
                    a_nPreLevel = a_nLevel;
                    a_nPreStar = a_nStar - 1;
                }
                else
                {
                    a_nPreLevel = tableData.PreId;
                    a_nPreStar = 0;
                }

                tableData = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(a_nPreLevel);
                if (tableData == null)
                {
                    Logger.LogErrorFormat("赛季段位表 找不到ID：{0}", a_nPreLevel);
                    break;
                }

                a_nExp += tableData.MaxExp;
            }

            a_nPreExp = a_nExp - nOffset;
        }

        public string GetRankName(int a_nRankID, bool a_bWithSubName = true)
        {
            SeasonLevelTable data = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(a_nRankID);
            if (data != null)
            {
                if (data.ID >= GetMaxRankID())
                {
                    return TR.Value(string.Format("pk_rank_main{0}", (int)data.MainLevel));
                }
                else
                {
                    if (a_bWithSubName)
                    {
                        return string.Format("{0}{1}",
                            TR.Value(string.Format("pk_rank_main{0}", (int)data.MainLevel)),
                            TR.Value(string.Format("pk_rank_sub{0}", (int)data.SmallLevel))
                        );
                    }
                    else
                    {
                        return TR.Value(string.Format("pk_rank_main{0}", (int)data.MainLevel));
                    }
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetSimpleRankName(int a_nRankID)
        {
            SeasonLevelTable data = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(a_nRankID);
            if (data != null)
            {
                return TR.Value(string.Format("pk_rank_simple{0}", (int)data.MainLevel));
            }
            else
            {
                return string.Empty;
            }
        }

        public bool IsBattleFailReduceRank(int a_nRankID)
        {
            SeasonLevelTable data = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(a_nRankID);
            if (data != null)
            {
                return data.IsFailLevelReduce == 1;
            }
            else
            {
                return false;
            }
        }

        public PromotionInfo GetPromotionInfo(int a_nRankID, PKResult a_eResult = PKResult.INVALID)
        {
            PromotionInfo info = new PromotionInfo();
            SeasonLevelTable data = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(a_nRankID);
            if (data != null && data.IsPromotion == 1)
            {
                string[] arrValues = data.PromotionRule.Split(',');
                Assert.IsTrue(arrValues.Length >= 2);
                info.nNextSeasonLevel = data.AfterId;
                info.nTargetWinCount = int.Parse(arrValues[0]);
                info.nTotalCount = int.Parse(arrValues[1]);
                info.nCurrentLoseCount = 0;
                info.nCurrentWinCount = 0;

                info.arrRecords = new List<byte>();
                if (seasonUplevelRecords != null)
                {
                    for (int i = 0; i < seasonUplevelRecords.Count; ++i)
                    {
                        info.arrRecords.Add(seasonUplevelRecords[i]);
                    }
                }
                if (a_eResult != PKResult.INVALID)
                {
                    info.arrRecords.Add((byte)a_eResult);
                }

                for (int i = 0; i < info.arrRecords.Count; ++i)
                {
                    if (info.arrRecords[i] == (byte)PKResult.WIN)
                    {
                        info.nCurrentWinCount++;
                    }
                    else if (info.arrRecords[i] == (byte)PKResult.LOSE || info.arrRecords[i] == (byte)PKResult.DRAW)
                    {
                        info.nCurrentLoseCount++;
                    }
                }

                if (info.nCurrentWinCount >= info.nTargetWinCount)
                {
                    info.eState = EPromotionState.Successed;
                }
                else if (info.nCurrentLoseCount > info.nTotalCount - info.nTargetWinCount)
                {
                    info.eState = EPromotionState.Failed;
                }
                else
                {
                    info.eState = EPromotionState.Promoting;
                }
            }
            else
            {
                info.eState = EPromotionState.Invalid;
            }

            return info;
        }

        public bool IsLevelValid(int a_nSeasonLevel)
        {
            return TableManager.GetInstance().GetTableItem<SeasonLevelTable>(a_nSeasonLevel) != null;
        }

        public string GetMainSeasonLevelIcon(int a_nSeasonLevel)
        {
            SeasonLevelTable table = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(a_nSeasonLevel);
            if (table != null)
            {
                return table.Icon;
            }
            return string.Empty;
        }

        public string GetMainSeasonLevelSmallIcon(int a_nSeasonLevel)
        {
            SeasonLevelTable table = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(a_nSeasonLevel);
            if (table != null)
            {
                return table.SmallIcon;
            }
            return string.Empty;
        }

        public string GetSubSeasonLevelIcon(int a_nSeasonLevel)
        {
            SeasonLevelTable table = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(a_nSeasonLevel);
            if (table != null)
            {
                return table.SubLevelIcon;
            }
            return string.Empty;
        }

        public static IList<int>  GetSeasonAttrBuffIDs(int a_nSeasonAttrID)
        {
			SeasonAttrTable tableData = TableManager.GetInstance().GetTableItem<SeasonAttrTable>(a_nSeasonAttrID);
            if (tableData != null)
            {
                return tableData.BuffIDs;
            }
            else
            {
                return null;
            }
        }

        public List<SeasonLevel> GetSeasonRewards()
        {
            return m_arrSeasonRewards;
        }

        public List<SeasonLevel> GetSeasonAttrs()
        {
            return m_arrSeasonAttrs;
        }

        public SeasonLevel GetSeasonAttr(int a_nSeasonAttrID)
        {
            for (int i = 0; i < m_arrSeasonAttrs.Count; ++i)
            {
                if (m_arrSeasonAttrs[i].attrTable.ID == a_nSeasonAttrID)
                {
                    return m_arrSeasonAttrs[i];
                }
            }
            return null;
        }

        public bool IsMainLevelSame(int a_leftSeasonID, int a_rightSeasonID)
        {
            SeasonLevelTable tableDataLeft = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(a_leftSeasonID);
            SeasonLevelTable tableDataRight = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(a_rightSeasonID);
            if (tableDataLeft != null && tableDataRight != null && tableDataRight.MainLevel == tableDataLeft.MainLevel)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetNextAttrID()
        {
            if (GetMaxRankID() == seasonLevel)
            {
                if (seasonRank >= 1 && seasonRank <= 10)
                {
                    SeasonLevelTable temp = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(seasonLevel);
                    return temp.AttrID + 1;
                }
            }

            SeasonLevelTable levelTable = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(seasonLevel);
            return levelTable.AttrID;
        }

        public void RequestSelfPKRank()
        {
            WorldSortListSelfInfoReq msg = new WorldSortListSelfInfoReq();
            msg.type = SortListDecoder.MakeSortListType(SortListType.SORTLIST_1V1_SEASON);
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<WorldSortListSelfInfoRet>(msgRet =>
            {
                seasonRank = (int)msgRet.ranking;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PKSelfLevelUpdated, msgRet.ranking);
            }, false);
        }

        public void RequsetMyPkRecord()
        {
            SceneReplayListReq msg = new SceneReplayListReq();
            msg.type = (byte)ReplayListType.Self;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<SceneReplayListRes>(msgRet =>
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PKMyRecordUpdated, msgRet);
            });
        }

        public void RequsetPeakPkRecord()
        {
            SceneReplayListReq msg = new SceneReplayListReq();
            msg.type = (byte)ReplayListType.Master;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<SceneReplayListRes>(msgRet =>
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PKPeakRecordUpdated, msgRet);
            });
        }

        public void NotifyVideoPlayed(UInt64 a_raceID)
        {
            SceneReplayView msg = new SceneReplayView();
            msg.raceid = a_raceID;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);
        }

        void _BindNetMsg()
        {
            NetProcess.AddMsgHandler(SceneSyncSeasonInfo.MsgID, _OnNetSeasonStart);
            NetProcess.AddMsgHandler(SceneMatchPkRaceEnd.MsgID, _OnNetPKEnd);
        }

        void _UnBindNetMsg()
        {
            NetProcess.RemoveMsgHandler(SceneSyncSeasonInfo.MsgID, _OnNetSeasonStart);
            NetProcess.RemoveMsgHandler(SceneMatchPkRaceEnd.MsgID, _OnNetPKEnd);
        }

        void _BindGameEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SystemChanged, _OnSystemChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PlayerDataSeasonUpdated, _OnPlayerDataSeasonChanged);
        }

        void _UnBindGameEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SystemChanged, _OnSystemChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PlayerDataSeasonUpdated, _OnPlayerDataSeasonChanged);
        }

        void _OnNetSeasonStart(MsgDATA a_msgData)
        {
            SceneSyncSeasonInfo msgSeason = new SceneSyncSeasonInfo();
            msgSeason.decode(a_msgData.bytes);

            seasonLevel = (int)msgSeason.seasonLevel;
            seasonStar = (int)msgSeason.seasonStar;
            seasonExp = (int)msgSeason.seasonExp;
            m_eSeasonPlayStatus = (SeasonPlayStatus)msgSeason.seasonStatus;

            seasonID = (int)msgSeason.seasonId;
            seasonEndTime = (int)msgSeason.endTime;

            seasonAttrMappedSeasonID = (int)msgSeason.seasonAttrLevel;
            seasonAttrID = (int)msgSeason.seasonAttr;
            seasonAttrEndTime = (int)msgSeason.seasonAttrEndTime;

            _TryShowSeasonStart();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SeasonStarted);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PKRankChanged);
        }

        void _OnNetPKEnd(MsgDATA a_msgData)
        {
            SceneMatchPkRaceEnd ret = new SceneMatchPkRaceEnd();
            ret.decode(a_msgData.bytes);

            seasonLevel = (int)ret.newSeasonLevel;
            seasonOldLevel = (int)ret.oldSeasonLevel;
            seasonStar = (int)ret.newSeasonStar;
            seasonOldStar = (int)ret.oldSeasonStar;
            seasonExp = (int)ret.newSeasonExp;
            seasonOldExp = (int)ret.oldSeasonExp;
        }

        void _OnSystemChanged(UIEvent a_event)
        {
            _TryShowSeasonStart();
        }

        void _OnPlayerDataSeasonChanged(UIEvent a_event)
        {
            SceneObjectAttr prop = (SceneObjectAttr)a_event.Param1;
            if (
                prop == SceneObjectAttr.SOA_SEASON_LEVEL ||
                prop == SceneObjectAttr.SOA_SEASON_STAR ||
                prop == SceneObjectAttr.SOA_SEASON_EXP
                )
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PKRankChanged);
            }
        }


        void _TryShowSeasonStart()
        {
            if (ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown != null &&
                Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Duel) && 
                m_eSeasonPlayStatus > SeasonPlayStatus.SPS_INVALID)
            {
#if UNITY_EDITOR || ROBOT_TEST
                //添加跳过弹窗的开关
                if (!Global.Settings.CloseLoginPushFrame)
                {
#endif
                    if (m_eSeasonPlayStatus == SeasonPlayStatus.SPS_NEW)
                    {
                        if (ClientSystemManager.GetInstance().IsFrameOpen<PKSeasonStartFrame>())
                        {
                            ClientSystemManager.GetInstance().CloseFrame<PKSeasonStartFrame>();
                        }
                        ClientSystemManager.GetInstance().OpenFrame<PKSeasonStartFrame>();
                    }
                    else if (m_eSeasonPlayStatus == SeasonPlayStatus.SPS_NEW_ATTR)
                    {
                        if (ClientSystemManager.GetInstance().IsFrameOpen<PKSeasonAttrChangeFrame>())
                        {
                            ClientSystemManager.GetInstance().CloseFrame<PKSeasonAttrChangeFrame>();
                        }
                        ClientSystemManager.GetInstance().OpenFrame<PKSeasonAttrChangeFrame>();
                    }
#if UNITY_EDITOR || ROBOT_TEST
                }
#endif            

                m_eSeasonPlayStatus = SeasonPlayStatus.SPS_INVALID;

                SceneSeasonPlayStatus msg = new SceneSeasonPlayStatus();
                msg.seasonId = (uint)seasonID;
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);
            }
        }

        void _InitSeasonRewards()
        {
            m_arrSeasonRewards.Clear();
            var iter = TableManager.GetInstance().GetTable<SeasonLevelTable>().GetEnumerator();
            while (iter.MoveNext())
            {
                SeasonLevelTable table = iter.Current.Value as SeasonLevelTable;
                bool bContain = false;
                for (int i = 0; i < m_arrSeasonRewards.Count; ++i)
                {
                    if (m_arrSeasonRewards[i].levelTable.MainLevel == table.MainLevel)
                    {
                        bContain = true;
                        break;
                    }
                }
                if (bContain == false)
                {
                    SeasonLevel seasonLevel = new SeasonLevel();
                    seasonLevel.levelTable = table;
                    seasonLevel.attrTable = null;
                    seasonLevel.strFormatAttr = string.Empty;
                    m_arrSeasonRewards.Add(seasonLevel);
                }
            }
        }

        void _ClearSeasonRewards()
        {
            m_arrSeasonRewards.Clear();
        }

        void _InitSeasonAttrs()
        {
            m_arrSeasonAttrs.Clear();

            var iter = TableManager.GetInstance().GetTable<SeasonLevelTable>().GetEnumerator();
            while (iter.MoveNext())
            {
                SeasonLevelTable table = iter.Current.Value as SeasonLevelTable;
                bool bContain = false;
                for (int i = 0; i < m_arrSeasonAttrs.Count; ++i)
                {
                    if (m_arrSeasonAttrs[i].levelTable.MainLevel == table.MainLevel)
                    {
                        bContain = true;
                        break;
                    }
                }
                if (bContain == false)
                {
                    SeasonLevel seasonLevel = new SeasonLevel();
                    seasonLevel.levelTable = table;
                    seasonLevel.attrTable = TableManager.GetInstance().GetTableItem<SeasonAttrTable>(table.AttrID);
                    seasonLevel.strFormatAttr = string.Empty;
                    for (int i = 0; i < seasonLevel.attrTable.Descs.Count; ++i)
                    {
                        if (i > 0)
                        {
                            if (i % 4 == 0)
                            {
                                seasonLevel.strFormatAttr += '\n';
                            }
                            else
                            {
                                seasonLevel.strFormatAttr += ' ';
                            }
                        }
                        seasonLevel.strFormatAttr += seasonLevel.attrTable.Descs[i];
                    }

                    m_arrSeasonAttrs.Add(seasonLevel);
                }
            }

            {
                SeasonLevel seasonLevel = new SeasonLevel();
                seasonLevel.levelTable = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(GetMaxRankID());
                seasonLevel.attrTable = TableManager.GetInstance().GetTableItem<SeasonAttrTable>(seasonLevel.levelTable.AttrID + 1);
                seasonLevel.strFormatAttr = string.Empty;
                for (int i = 0; i < seasonLevel.attrTable.Descs.Count; ++i)
                {
                    if (i > 0)
                    {
                        if (i % 4 == 0)
                        {
                            seasonLevel.strFormatAttr += '\n';
                        }
                        else
                        {
                            seasonLevel.strFormatAttr += "  ";
                        }
                    }
                    seasonLevel.strFormatAttr += seasonLevel.attrTable.Descs[i];
                }

                m_arrSeasonAttrs.Add(seasonLevel);
            }

        }

        void _ClearSeasonAttrs()
        {
            m_arrSeasonAttrs.Clear();
        }
    }
}
