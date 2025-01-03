using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class RaidBattle : PVEBattle
    {
        int mCurRebornCount = 0;
        int mMaxRebornCount = 0;
        bool mIsTeamWin = false;
        protected bool[] ServerNotifyArr = new bool[10];
        protected Dictionary<int, int> mDungeonRecoverProcess = new Dictionary<int, int>();//保存团本关卡其他进度
        public RaidBattle(BattleType type, eDungeonMode mode, int id) : base(type, mode, id)
        {
            if (dungeonManager == null ||
               dungeonManager.GetDungeonDataManager() == null ||
               dungeonManager.GetDungeonDataManager().table == null)
                return;
            mMaxRebornCount = dungeonManager.GetDungeonDataManager().table.TotalRebornCount;
            mCurRebornCount = mMaxRebornCount;
        }
        public override bool IsRebornCountLimit()  //是否副本有总复活数量限制
        {
            return mMaxRebornCount > 0;
        }
        public override int GetLeftRebornCount()  //剩余总复活数量
        {
            return mCurRebornCount;
        }
        public override int GetMaxRebornCount()
        {
            return mMaxRebornCount;
        }

        protected override void _onReborn(BattlePlayer player)
        {
            mCurRebornCount--;
        }
        protected override void _onSceneDungeonRaceEndRes(SceneDungeonRaceEndRes res)
        {
#if !LOGIC_SERVER
            ClientReconnectManager.instance.canReconnectRelay = false;

            Logger.LogProcessFormat("[PVE战斗] 收到结算消息 {0}", ObjectDumper.Dump(res));

            if (recordServer != null && recordServer.IsProcessRecord())
            {
                recordServer.RecordProcess("[BATTLE]Race End");
                recordServer.Mark(0xb8422742);
                // Mark:0xb8422742 [BATTLE]Race End
            }

            if (res.result == 0)
            {
                _finishProcess((DungeonScore)res.score);
            }
            else
            {
                Logger.LogErrorCode(res.result);
            }
#endif
        }

        //UI界面展示 无结算翻牌需求
        private void _finishProcess(DungeonScore score)
        {
            if (null == BattleMain.instance)
            {
                return;
            }
            if (!mIsTeamWin)
            {
                if (score != DungeonScore.C)
                {
                    string notifySucc = TR.Value("team_battle_result_succ");
                    SystemNotifyManager.SysNotifyFloatingEffect(notifySucc);
                }
                else
                {
                    isDungeonFail = true;
                    string notifyFailed = TR.Value("team_battle_result_failed");
                    SystemNotifyManager.SysNotifyFloatingEffect(notifyFailed);
                }
            }

            if (!ClientSystemManager.instance.IsFrameOpen<DungeonMenuFrame>())
            {
                ClientSystemManager.instance.OpenFrame<DungeonMenuFrame>();
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonRewardFinish);
        }

        protected override void _onTeamCopyRaceEnd()
        {
            if (mDungeonManager != null && !mDungeonManager.IsFinishFight())
            {
                mIsTeamWin = true;
#if !LOGIC_SERVER
                //当收到该帧命令后直接结算
                _sendDungeonRaceEndFail(DungeonScore.A);
#else 
                var req = _getRaceEndTeamReqForLogicServer(DungeonScore.A);
                if( req != null)
                {
                    LogicServer.ReportRaceEndToLogicServer(req);
                }
#endif
                if (mDungeonManager.GetBeScene() != null)
                {
                    mDungeonManager.GetBeScene().ClearAllMonsters();
                }
            }

        }

        /// <summary>
        /// 获取关卡进度
        /// </summary>
        /// <param name="dungeonId"></param>
        /// <returns></returns>
        public int GetDungeonRecoverProcess(int dungeonId)
        {
            int process;
            if (mDungeonRecoverProcess.TryGetValue(dungeonId, out process))
            {
                return process;
            }
            return 0;
        }
        /// <summary>
        /// 设置关卡进度
        /// </summary>
        /// <param name="dungeonId"></param>
        /// <param name="process"></param>
        public void SetDungeonRecoverProcess(int dungeonId, int process)
        {
            if (mDungeonRecoverProcess.ContainsKey(dungeonId))
            {
                mDungeonRecoverProcess[dungeonId] = process;
            }
            else
            {
                mDungeonRecoverProcess.Add(dungeonId, process);
            }
        }

        public void DungeonDestroyNotify(int dungeonId)
        {
            Logger.LogErrorFormat("收到团本关卡被歼灭的消息,关卡ID:{0}", dungeonId);
            if (dungeonId == 8000700)
            {
                ServerNotifyArr[(int)ServerNotifyMessageId.CthyllaSweetDream] = true;
            }
            else if (dungeonId == 8000800)
            {
                ServerNotifyArr[(int)ServerNotifyMessageId.CthyllaNightmare] = true;
            }
            else if (dungeonId == 8000400)
            {
                ServerNotifyArr[(int)ServerNotifyMessageId.TeamTailPass] = true;
            }
            else if (dungeonId == 8001700)
            {
                ServerNotifyArr[(int)ServerNotifyMessageId.HARDCthyllaSweetDream] = true;
            }
            else if (dungeonId == 8001800)
            {
                ServerNotifyArr[(int)ServerNotifyMessageId.HARDCthyllaNightmare] = true;
            }
            else if (dungeonId == 8001400)
            {
                ServerNotifyArr[(int)ServerNotifyMessageId.HARDTeamTailPass] = true;
            }
        }

        /// <summary>
        /// 检测服务器下发消息是否满足
        /// </summary>
        public bool CheckServerNotify(int id)
        {
            if (id <= 0)
                return false;
            if (id >= ServerNotifyArr.Length)
            {
                Logger.LogErrorFormat("消息ID错误:{0}", id);
                return false;
            }
            return ServerNotifyArr[id];
        }

        #region IOnExecCommand implementation
        public override void BeforeExecFrameCommand(byte seat, IFrameCommand cmd)
        {
        }

        public override void AfterExecFrameCommand(byte seat, IFrameCommand cmd)
        {
            base.AfterExecFrameCommand(seat, cmd);

            switch (cmd.GetID())
            {
                case FrameCommandID.TeamCopyRaceEnd:
                    {
                        _onTeamCopyRaceEnd();

                        if (mDungeonManager != null)
                        {
                            mDungeonManager.FinishFight();
                        }
#if !LOGIC_SERVER
                        //if (FrameSync.instance.IsInChasingMode)
                        //    FrameSync.instance.EndChasingMode();
#endif
                        if (!ReplayServer.GetInstance().IsLiveShow())
                        {
                            FrameSync.instance.SetDungeonMode(eDungeonMode.LocalFrame);
                        }


                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BattleFrameSyncEnd);
                    }
                    break;
            }
        }
        #endregion

#if LOGIC_SERVER
        protected RelaySvrDungeonRaceEndReq _getRaceEndTeamReqForLogicServer(DungeonScore score)
        {
            if (mDungeonManager == null || mDungeonManager.GetDungeonDataManager() == null ||
                mDungeonManager.GetDungeonDataManager().id == null || mDungeonStatistics == null ||
                mDungeonPlayers == null || mDungeonPlayers.GetAllPlayers() == null)
                return null;
            RelaySvrDungeonRaceEndReq msg = new RelaySvrDungeonRaceEndReq();
            msg.raceEndInfo.sessionId = recordServer != null ? System.Convert.ToUInt64(recordServer.sessionID) : 0UL;

            msg.raceEndInfo.dungeonId = (uint)mDungeonManager.GetDungeonDataManager().id.dungeonID;
            msg.raceEndInfo.usedTime = (uint)mDungeonStatistics.AllFightTime(true);

            var m_battlePlayers = mDungeonPlayers.GetAllPlayers();

            msg.raceEndInfo.infoes = new DungeonPlayerRaceEndInfo[m_battlePlayers.Count];

            for (int i = 0; i < m_battlePlayers.Count; ++i)
            {
                RacePlayerInfo source = m_battlePlayers[i].playerInfo;
                DungeonPlayerRaceEndInfo target = new DungeonPlayerRaceEndInfo
                {
                    roleId = source.roleId,
                    pos = source.seat,
                    score = (byte)score,
                    beHitCount = (ushort)mDungeonStatistics.HitCount(source.seat)
                };
                target.md5 = DungeonUtility.GetBattleEncryptMD5(string.Format("{0}{1}{2}", target.score, target.beHitCount, msg.raceEndInfo.usedTime));
                msg.raceEndInfo.infoes[i] = target;
            }

            return msg;
        }
#endif
    }
}

