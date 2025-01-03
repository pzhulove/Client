using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;

using Network;
using Protocol;
using DG.Tweening;
using ProtoTable;

namespace GameClient
{
    [LoggerModel("Chapter")]
    public class PVEBattle : BaseBattle
    {
        protected int thisRoomMonsterCreatedCount;

        private enum eHellProcessState
        {
            onHellNone,

            onHellInit,

            onHellTipsInit,
            onHellTips,

            onWaitHellProcessStart,
            onHellProcessStart,

            onHellProcessBattleFight,
            onHellProcessBattleClear,

            onHellProcessFinish,
            onHellProcessReportFinish,
        }

        private DungeonHellGuideFrame mHellTipsFrame = null;

        private eHellProcessState mHellState = eHellProcessState.onHellNone;
        private BeObject mCurrentHellObject = null;
        private bool mHasHellPlayBGM = false;
        private bool mStarted = false;

        private const int kWaitHellTipsTime = 6;
        private int mWaitHellTipsTimeCounter = kWaitHellTipsTime;
        private int mWaitOneSeconedCounter = 1000;
        private int mHellFinishDurTime = -1;
        private bool mHellOpen = false;
        private bool eggRoomOpen = true;
        private void _resetHellData()
        {
            mHellState = eHellProcessState.onHellNone;
            mCurrentHellObject = null;
            mHasHellPlayBGM = false;
            mWaitHellTipsTimeCounter = kWaitHellTipsTime;
            mWaitOneSeconedCounter = 1000;
        }

        public PVEBattle(BattleType type, eDungeonMode mode, int id) : base(type, mode, id)
        { 
            //加速单机模式进入
            if (Global.Settings.startSystem == GameClient.EClientSystem.Battle)
            {
                NeedPreload = false;
            }
        }

        #region 小地图
        public void OpenHellClose()
        {
            mHellOpen = true;
            if (this.recordServer != null && recordServer.IsProcessRecord())
            {
                this.recordServer.RecordProcess("Open Hell Close");
                this.recordServer.Mark(0xb2452746);
                // Mark:0xb2452746 [SCENE]Open Hell Close
            }
        }

        private void _updateDungeonMap()
        {
#if !LOGIC_SERVER
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIDungeonMap>();
            if (battleUI != null
                && mDungeonManager!=null
                && mDungeonManager.GetDungeonDataManager()!=null)
            {
                battleUI.dungeonMapCom.SetDungeonData(mDungeonManager.GetDungeonDataManager().asset, mDungeonManager.GetDungeonDataManager().GetNextAreaIndex);

                int originID = mDungeonManager.GetDungeonDataManager().FindDataConnectIDByAreaID(BattleDataManager.GetInstance().BattleInfo.dungeonHealInfo.areaId);

                battleUI.dungeonMapCom.SetHell(BattleDataManager.GetInstance().BattleInfo.dungeonHealInfo.mode,
                        originID);
            }
#endif
        }

        protected void _updateDungeonState(bool isOpen)
        {
#if !LOGIC_SERVER
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIDungeonMap>();
            if (battleUI != null)
            {
                Logger.LogProcessFormat("update dungeon map state with state {0}", isOpen);

                //curSystem.SetDungeonMapActive(true);

                var item = mDungeonManager.GetDungeonDataManager().CurrentDataConnect();
                if (item == null)
                {
                    return;
                }

                if(battleUI.dungeonMapCom != null)
                {
                    if (mDungeonManager.GetDungeonDataManager().table.SubType == DungeonTable.eSubType.S_DEVILDDOM)
                        battleUI.dungeonMapCom.SetDropProgress(areaIndex);

                    battleUI.dungeonMapCom.SetStartPosition(item.GetPositionX(), item.GetPositionY());

                    if (isOpen)
                    {
                        battleUI.dungeonMapCom.SetOpenPosition(item.GetPositionX(), item.GetPositionY());
                    }
                    battleUI.dungeonMapCom.SetViewPortData(item.GetPositionX(), item.GetPositionY());
                } 
            }
#endif
        }

        /// <summary>
        /// 只是小地图UI显示
        /// </summary>
        /// <param name="flag"></param>
        public void SetEggRoom(bool flag)
        {
#if !LOGIC_SERVER
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIDungeonMap>();
            if (battleUI != null && battleUI.dungeonMapCom != null)
                battleUI.dungeonMapCom.SetEggRoomState(flag);
#endif
        }

        protected void _hiddenDungeonMap(bool isShow)
        {
#if !LOGIC_SERVER
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIDungeonMap>();
            if (null != battleUI)
            {
                battleUI.SetDungeonMapActive(isShow);
            }
#endif
        }

        protected uint areaIndex;
        protected void _sendDungeonDropProgressReq()
        {
#if !LOGIC_SERVER
            if (_isNeedSendNet() && mDungeonManager.GetDungeonDataManager().table.SubType == DungeonTable.eSubType.S_DEVILDDOM)
            {
                GameFrameWork.instance.StartCoroutine(_onWorldDungeonGetAreaIndex());
            }
#endif
        }

        private IEnumerator _onWorldDungeonGetAreaIndex()
        {
            if (mDungeonManager == null)
                yield break;

            var req = new WorldDungeonGetAreaIndexReq();
            req.dungeonId = (uint)mDungeonManager.GetDungeonDataManager().battleInfo.dungeonId;
            var res = new WorldDungeonGetAreaIndexRes();
            var msg = new MessageEvents();

            yield return MessageUtility.Wait(ServerType.GATE_SERVER, msg, req, res);

            if (msg.IsAllMessageReceived())
            {
                areaIndex = res.areaIndex >> 1;
                _updateDungeonState(false);
            }
        }

        #endregion

        #region DungeonKillMonsterReq
        protected void _sendDungeonKillMonsterReq()
        {
#if !LOGIC_SERVER
            if (_isNeedSendNet())
            {
                mDungeonManager.GetDungeonDataManager().SendKillMonsters();
            }
#endif
        }

        protected void _resendNoVertifyDungeonKillMonsters()
        {
#if !LOGIC_SERVER
            if (_isNeedSendNet() && mDungeonManager.GetDungeonDataManager().HasNoVertifyKilledMonsters())
            {
                mDungeonManager.GetDungeonDataManager().SendNoVertifyKilledMonsters();
            }
#endif
        }
        #endregion

        #region SceneDungeonRaceEnd
        protected virtual SceneDungeonRaceEndReq _getDungeonRaceEndReq()
        {
            var mainPlayer = mDungeonPlayers.GetMainPlayer();
            var msg = new SceneDungeonRaceEndReq
            {
                beHitCount = (ushort)mDungeonStatistics.HitCount(mainPlayer.playerInfo.seat),
                usedTime = (uint)mDungeonStatistics.AllFightTime(true),
                score = (byte)mDungeonStatistics.FinalDungeonScore(),
                maxDamage = mDungeonStatistics.GetAllMaxHurtDamage(),
                skillId = mDungeonStatistics.GetAllMaxHurtSkillID(),
                param = mDungeonStatistics.GetAllMaxHurtID(),
                totalDamage = mDungeonStatistics.GetAllHurtDamage(),
                lastFrame = mDungeonManager.GetDungeonDataManager().GetFinalFrameDataIndex()
            };
            msg.md5 = DungeonUtility.GetBattleEncryptMD5(string.Format("{0}{1}{2}", msg.score, msg.beHitCount, msg.usedTime));
#if !LOGIC_SERVER
            if (recordServer != null && recordServer.IsProcessRecord())
            {
                recordServer.RecordProcess("[BATTLE]Race End _getDungeonRaceEndReq lastFrame {0}", msg.lastFrame);
                RecordServer.GetInstance().MarkInt(0x8779809, (int)msg.lastFrame);
                // Mark:0x8779809 [BATTLE]Race End _getDungeonRaceEndReq lastFrame {0}
            }

#endif

            return msg;
        }

        protected SceneDungeonRaceEndReq _getDungeonRaceEndReqWithCount(int count)
        {
            if (mDungeonPlayers == null) return null;
            var mainPlayer = mDungeonPlayers.GetMainPlayer();
            if (mainPlayer == null) return null;
            var msg = new SceneDungeonRaceEndReq
            {
                beHitCount = (ushort)mDungeonStatistics.HitCount(mainPlayer.playerInfo.seat),
                usedTime = (uint)mDungeonStatistics.LastFightTimeWithCount(true, count),
                score = (byte)mDungeonStatistics.FinalDungeonScore(),
                maxDamage = mDungeonStatistics.GetAllMaxHurtDamage(),
                skillId = mDungeonStatistics.GetAllMaxHurtSkillID(),
                param = mDungeonStatistics.GetAllMaxHurtID(),
                totalDamage = mDungeonStatistics.GetAllHurtDamage(),
                lastFrame = mDungeonManager.GetDungeonDataManager().GetFinalFrameDataIndex()
            };
            msg.md5 = DungeonUtility.GetBattleEncryptMD5(string.Format("{0}{1}{2}", msg.score, msg.beHitCount, msg.usedTime));
#if !LOGIC_SERVER
            if (recordServer != null && recordServer.IsProcessRecord())
            {
                recordServer.RecordProcess("[BATTLE]Race End _getDungeonRaceEndReqWithCount lastFrame {0}", msg.lastFrame);
                RecordServer.GetInstance().MarkInt(0x8779810, (int)msg.lastFrame);
                // Mark:0x8779810 [BATTLE]Race End _getDungeonRaceEndReqWithCount lastFrame {0}
            }

#endif

            return msg;
        }

        //单人关卡结算
        private IEnumerator _sendDungeonRaceEndReqIter(bool modifyScore = false, Protocol.DungeonScore score = Protocol.DungeonScore.C)
        {
            yield return _fireRaceEndOnLocalFrameIter();
            yield return _sendDungeonReportDataIter();

            var msgEvent = new MessageEvents();
            var res = new SceneDungeonRaceEndRes();
            var req = _getDungeonRaceEndReq();

            if (modifyScore)
            {
                req.score = (byte)score;
                req.md5 = DungeonUtility.GetBattleEncryptMD5(string.Format("{0}{1}{2}", req.score, req.beHitCount, req.usedTime));
            }

            //Logger.LogErrorFormat("[单人关卡] 发送结算");
            if (mDungeonManager.GetDungeonDataManager() != null)
            {
                mDungeonManager.GetDungeonDataManager().LockBattleEnd();
            }


            yield return _sendMsgWithResend<SceneDungeonRaceEndReq, SceneDungeonRaceEndRes>(ServerType.GATE_SERVER, msgEvent, req, res, true, 3, 3, 33);

            //收到结算
            if (msgEvent.IsAllMessageReceived())
            {
                //Logger.LogErrorFormat("[单人关卡] 收到结算");
                _onSceneDungeonRaceEndRes(res);
            }
            else
            {
                ClientSystemManager.instance.QuitToLoginSystem(9991);
                //Logger.LogErrorFormat("[单人关卡] 没有收到结算的消息");
            }
        }

        protected IEnumerator _sendDungeonReportDataIter()
        {
            if (GetMode() == eDungeonMode.LocalFrame)
            {
                if (mDungeonManager == null)
                    yield break;
                if (mDungeonManager.GetDungeonDataManager() == null)
                    yield break;

                mDungeonManager.GetDungeonDataManager().PushFinalFrameData();

                mDungeonManager.GetDungeonDataManager().SendWorldDungeonReportFrame();

                yield return null;

                while (!mDungeonManager.GetDungeonDataManager().IsAllReportDataServerRecived())
                {
                    yield return null;
                }

                mDungeonManager.GetDungeonDataManager().UnlockUpdateCheck();
            }
        }

        private RelaySvrDungeonRaceEndReq _getDungeonRaceEndTeamReq()
        {

            RelaySvrDungeonRaceEndReq msg = new RelaySvrDungeonRaceEndReq();
            //msg.roleId = PlayerBaseData.GetInstance().RoleID;
#if !LOGIC_SERVER
            msg.raceEndInfo.sessionId = ClientApplication.playerinfo.session;
#else
            msg.raceEndInfo.sessionId = this.recordServer != null ? Convert.ToUInt64(recordServer.sessionID) : 0UL;
#endif
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
                    score = (byte)mDungeonStatistics.FinalDungeonScore(),
                    beHitCount = (ushort)mDungeonStatistics.HitCount(source.seat)
                };
                target.md5 = DungeonUtility.GetBattleEncryptMD5(string.Format("{0}{1}{2}", target.score, target.beHitCount, msg.raceEndInfo.usedTime));
                msg.raceEndInfo.infoes[i] = target;
            }

            return msg;
        }

        private IEnumerator _sendDungeonTeamRaceEndReqIter(bool modifyScore = false, Protocol.DungeonScore score = Protocol.DungeonScore.C)
        {
#if !LOGIC_SERVER
            var msgEvent = new MessageEvents();
            var res = new SceneDungeonRaceEndRes();
            var req = _getDungeonRaceEndTeamReq();

            if (modifyScore)
            {
                for (int i = 0; i < req.raceEndInfo.infoes.Length; ++i)
                {
                    DungeonPlayerRaceEndInfo info = req.raceEndInfo.infoes[i];

                    info.score = (byte)score;
                    info.md5 = DungeonUtility.GetBattleEncryptMD5(string.Format("{0}{1}{2}", info.score, info.beHitCount, req.raceEndInfo.usedTime));
                }
            }

            BattleMain.instance.WaitForResult();

            if (SwitchFunctionUtility.IsOpen(51))
            {
                yield return MessageUtility.Wait<RelaySvrDungeonRaceEndReq, SceneDungeonRaceEndRes>(ServerType.RELAY_SERVER, msgEvent, req, res, false, 60.0f);

                if (msgEvent.IsAllMessageReceived())
                {
                    _onSceneDungeonRaceEndRes(res);
                }
                else
                {
                    ClientSystemManager.instance.QuitToLoginSystem(9991);
                    yield break;
                }
            }
            else
            {
                yield return MessageUtility.Wait<RelaySvrDungeonRaceEndReq, SceneDungeonRaceEndRes>(ServerType.RELAY_SERVER, msgEvent, req, res, false);

                if (msgEvent.IsAllMessageReceived())
                {
                    _onSceneDungeonRaceEndRes(res);
                }
            }
#else
            yield break;
#endif
        }
#if !LOGIC_SERVER
        private Coroutine mDungeonRaceEndReqCoroutine = null;
#endif

        public void SendDungeonRaceEnd()
        {
            _sendDungeonRaceEndReq();
        }
        protected void _sendDungeonRaceEndReq(bool dead = false)
        {
#if !LOGIC_SERVER
            isNormalFinsh = !dead;
            if (_isNeedSendNet())
            {
                if (ClientSystemManager.instance.IsFrameOpen<DungeonRebornFrame>())
                {
                    ClientSystemManager.instance.CloseFrame<DungeonRebornFrame>();
                }

                if (eDungeonMode.SyncFrame == GetMode())
                {
                    //
                    //如果取消复活，并且当前没有活着的人就结束战斗
                    if (!dead)
                        mDungeonRaceEndReqCoroutine = GameFrameWork.instance.StartCoroutine(_sendDungeonTeamRaceEndReqIter());
                    else
                    {
                        if (mDungeonPlayers.IsAllPlayerDead())
                        {
                            GameClient.ClientReconnectManager.instance.canReconnectRelay = false;
                            mDungeonRaceEndReqCoroutine = GameFrameWork.instance.StartCoroutine(_sendDungeonTeamRaceEndReqIter());
                        }
                        else
                        {
                            //
                            //ClientSystemManager.instance.CloseFrame<DungeonRebornFrame>();
                        }
                    }
                }
                else
                {
                    GameFrameWork.instance.StartCoroutine(_sendDungeonRaceEndReqIter());
                }
            }
            else
            {
                SceneDungeonRaceEndRes res = new SceneDungeonRaceEndRes
                {
                    result = 0
                };
                _onSceneDungeonRaceEndRes(res);
            }

            ClearBgm();

            _playDungeonFinish();
#endif
        }

        protected void _sendDungeonRaceEndFail(DungeonScore score = DungeonScore.C)
        {
#if !LOGIC_SERVER
            if (eDungeonMode.SyncFrame == GetMode())
            {
                mDungeonRaceEndReqCoroutine = GameFrameWork.instance.StartCoroutine(_sendDungeonTeamRaceEndReqIter(true, score));
            }
            else
            {
				GameFrameWork.instance.StartCoroutine(_sendDungeonRaceEndReqIter(true, score));
            }
#endif
        }

        /// <summary>
        /// 已经获得的道具列表
        /// </summary>
        /// <returns></returns>
        protected List<ComItemList.Items> _getPickedDropItems()
        {
            List<Battle.DungeonDropItem> allGots = BattleDataManager.GetInstance().BattleInfo.dropCacheItemIds;
            List<ComItemList.Items> list = new List<ComItemList.Items>();

            for (int i = 0; i < allGots.Count; ++i)
            {
                ComItemList.Items item = list.Find(x =>
                {
                    return x.id == (int)allGots[i].typeId;
                });

                if (null != item)
                {
                    item.count += (uint)allGots[i].num;
                }
                else
                {
                    list.Add(new ComItemList.Items()
                    {
                        id = allGots[i].typeId,
                        count = (uint)allGots[i].num,
                        type = ComItemList.eItemType.Custom,
                        strenthLevel = allGots[i].strenthLevel,
                        equipType = allGots[i].equipType
                    });
                }
            }

            return list;
        }

        protected List<ComItemList.Items> _getTeamRewardItem(SceneDungeonRaceEndRes res)
        {
            List<ComItemList.Items> list = new List<ComItemList.Items>();

            if (null != res.teamReward && res.teamReward.id > 0)
            {
                ComItemList.Items item = new ComItemList.Items
                {
                    count = res.teamReward.num,
                    id = (int)res.teamReward.id,
                    type = ComItemList.eItemType.Custom,
                    flag = ComItemList.eItemExtraFlag.ExtraReward,
                    strenthLevel = res.teamReward.strength,
                    equipType = (EEquipType)res.teamReward.equipType
                };
                list.Insert(0, item);
            }
            if (null != res.veteranReturnReward && res.veteranReturnReward.id > 0)
            {
                ComItemList.Items item = new ComItemList.Items
                {
                    count = res.veteranReturnReward.num,
                    id = (int)res.veteranReturnReward.id,
                    type = ComItemList.eItemType.Custom,
                    flag = ComItemList.eItemExtraFlag.ExtraReward,
                    strenthLevel = res.veteranReturnReward.strength,
                    equipType = (EEquipType)res.veteranReturnReward.equipType
                };
                list.Insert(0, item);
            }
            return list;
        }

        protected List<ComItemList.Items> _getRaceEndDropItem()
        {

            List<ComItemList.Items> list = new List<ComItemList.Items>();
            if (mDungeonManager == null || mDungeonManager.GetDungeonDataManager() == null)
            {
                return list;
            }
            List<Battle.DungeonDropItem> drops = mDungeonManager.GetDungeonDataManager().GetRaceEndDrops();
            for (int i = 0; i < drops.Count; ++i)
            {
                ComItemList.Items item = new ComItemList.Items
                {
                    count = (uint)drops[i].num,
                    id = (int)drops[i].typeId,
                    type = ComItemList.eItemType.Custom,
                    strenthLevel = drops[i].strenthLevel,
                    equipType = drops[i].equipType
                };

                list.Add(item);
            }

            return list;
        }

        protected List<ComItemList.Items> _getAllRewardItems(SceneDungeonRaceEndRes res)
        {
            List<ComItemList.Items> list = _getTeamRewardItem(res);
            List<ComItemList.Items> raceend = _getRaceEndDropItem();
            List<ComItemList.Items> droplist = _getPickedDropItems();
            if (BattleDataManager.GetInstance() != null && BattleDataManager.GetInstance().mSelfRollItems.Count > 0)
            {
                list.AddRange(BattleDataManager.GetInstance().mSelfRollItems);
            }
            list.AddRange(raceend);
            list.AddRange(droplist);

            return list;
        }

        private void _openDungeonFinishFrame(SceneDungeonRaceEndRes res)
        {
#if !LOGIC_SERVER
            //存储战斗结果
            PveBattleResult = (DungeonScore)res.score == DungeonScore.C ? BattleResult.Fail : BattleResult.Success;
            var frame = GameClient.ClientSystemManager.instance.OpenFrame<GameClient.DungeonFinishFrame>(GameClient.FrameLayer.Middle) as GameClient.DungeonFinishFrame;
            frame.SetData(res);
            frame.SetDrops(_getAllRewardItems(res).ToArray());
#endif
        }

        private IEnumerator _waitForFrameClose(Type type)
        {
#if !LOGIC_SERVER
            yield return Yielders.EndOfFrame;

            while (ClientSystemManager.instance.IsFrameOpen(type))
            {
                yield return Yielders.EndOfFrame;
            }
#else
            yield break;
#endif
        }

        protected IEnumerator _requestRaceEndDrops(int multi)
        {
#if !LOGIC_SERVER
            if (!_isNeedSendNet())
            {
                yield break;
            }

            var msg = new MessageEvents();
            var req = new SceneDungeonEndDropReq();
            var res = new SceneDungeonEndDropRes();

            req.multi = (byte)multi;

            yield return (_sendMsgWithResend<SceneDungeonEndDropReq, SceneDungeonEndDropRes>(ServerType.GATE_SERVER, msg, req, res, true, 10));

            if (msg.IsAllMessageReceived())
            {
            }
#else
            yield break;
#endif
        }


        private void DoSpecialGCClear(bool clearKey = false)
        {
            //对某些特殊的关卡 在过门的时候添加GC
            if (mDungeonManager.GetDungeonDataManager().table != null && mDungeonManager.GetDungeonDataManager().table.NeedForceGC)
            {
                // Logger.LogErrorFormat("did:{0} 执行GC!!!!", did);
                if (clearKey)
                    AssetGabageCollector.instance.ClearUnusedAsset(CResPreloader.instance.priorityGameObjectKeys);
                else
                    AssetGabageCollector.instance.ClearUnusedAsset();
            }
        }

        private IEnumerator _finishProcess(SceneDungeonRaceEndRes res)
        {
#if !LOGIC_SERVER 
            if (0 != res.hasRaceEndDrop)
            {
                yield return _requestRaceEndDrops(res.raceEndDropBaseMulti);
            }

            if (null == BattleMain.instance)
            {
                yield break;
            }

            if (mDungeonManager != null && mDungeonManager.GetDungeonDataManager() != null &&
                mDungeonManager.GetDungeonDataManager().table != null)
            {
                Vec3 pos = mDungeonManager.GetBeScene().GeGDeadBossPosition();
                mDungeonManager.GetBeScene().DropItems(mDungeonManager.GetDungeonDataManager().GetRaceEndDrops(), new VInt3(pos),false);
                yield return Yielders.GetWaitForSeconds(1.5f);/// new WaitForSeconds(1.5f);
            }
            if (null == BattleMain.instance)
            {
                yield break;
            }
            bool isItemRoll = false;
            bool isSingleItemRoll = false;
            isSingleItemRoll = res.rollSingleReward != null && res.rollSingleReward.Length > 0;
            isItemRoll = res.rollReward != null && res.rollReward.Length > 0;
            //开启roll点流程
            if (isItemRoll)
            {
                Vec3 pos = mDungeonManager.GetBeScene().GeGDeadBossPosition();
                List<BeRegionDropItem> dropItems = new List<BeRegionDropItem>();
                mDungeonManager.GetBeScene().DropItems(BattleDataManager.GetInstance().mAllRollRewardItems, new VInt3(pos), false, false, dropItems);
                ClientSystemManager.instance.OpenFrame<DungeonRollFrame>(FrameLayer.Middle);
                yield return (_waitForFrameClose(typeof(DungeonRollFrame)));
                if (ClientSystemManager.instance.IsFrameOpen<DungeonRollResultFrame>())
                {
                    float waitTime = 5.0f;
                    var systemValueItem = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>(632);
                    if (systemValueItem != null)
                    {
                        waitTime = Convert.ToSingle(systemValueItem.Value);
                    }
                    yield return Yielders.GetWaitForSeconds(waitTime);
                    for (int i = 0; i < dropItems.Count; i++)
                    {
                        if (dropItems[i] != null)
                        {
                            dropItems[i].RemoveAll();
                        }
                    }
                    ClientSystemManager.instance.CloseFrame<DungeonRollResultFrame>();
                }
            }
            else if (isSingleItemRoll)
            {
                Vec3 pos = mDungeonManager.GetBeScene().GeGDeadBossPosition();
                mDungeonManager.GetBeScene().DropItems(BattleDataManager.GetInstance().mAllRollRewardItems, new VInt3(pos), false, true);
            }

            _openDungeonFinishFrame(res);

            yield return (_waitForFrameClose(typeof(DungeonFinishFrame)));

            if (null == BattleMain.instance)
            {
                yield break;
            }

            if (0 != res.hasRaceEndChest)
            {
                if (null == BattleMain.instance)
                {
                    yield break;
                }
                var battle = this as GuildPVEBattle; //公会战没有翻牌
                if (battle == null)
                {
                    RecordServer.instance.PushReconnectCmd("openDungeonRewardFrame");
                    ClientSystemManager.instance.OpenFrame<DungeonRewardFrame>(FrameLayer.Middle);
                    yield return (_waitForFrameClose(typeof(DungeonRewardFrame)));
                }
            }

            if (null == BattleMain.instance)
            {
                yield break;
            }

            if (mDungeonManager == null || mDungeonManager.GetDungeonDataManager()==null) yield break;

            int id = dungeonManager.GetDungeonDataManager().id.dungeonID;
            DungeonTable tableData = TableManager.GetInstance().GetTableItem<DungeonTable>(id);
            if (tableData != null && isNormalFinsh &&
                ChapterUtility.PreconditionIDList(id).Count != 0 &&
                isChapterNoPassed && tableData.SubType != DungeonTable.eSubType.S_WEEK_HELL)
            {
                ClientSystemManager.instance.OpenFrame<BossMissionCompletePromptFrame>(FrameLayer.Middle);

                yield return (_waitForFrameClose(typeof(BossMissionCompletePromptFrame)));
            }
            if (null == BattleMain.instance)
            {
                yield break;
            }

            ClientSystemManager.instance.OpenFrame<DungeonMenuFrame>();

            yield return Yielders.EndOfFrame;
#else 
            yield break;
#endif

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonRewardFinish);
        }

        protected virtual void _onSceneDungeonRaceEndRes(SceneDungeonRaceEndRes res)
        {
#if !LOGIC_SERVER
            ClientReconnectManager.instance.canReconnectRelay = false;
            RecordServer.instance.PushReconnectCmd(string.Format("_onSceneDungeonRaceEndRes : {0} Begin-----------", res.result));
            Logger.LogProcessFormat("[PVE战斗] 收到结算消息 {0}", ObjectDumper.Dump(res));

            if (recordServer != null && recordServer.IsProcessRecord())
            {
                recordServer.RecordProcess("[BATTLE]Race End");
                recordServer.Mark(0xb2452741);
                // Mark:0xb2452741 [BATTLE]Race End
            }

            if (res.result == 0)
            {
                //if (null != mDungeonManager && null != mDungeonManager.GetDungeonDataManager() && mDungeonManager.GetDungeonDataManager().IsBossArea())
                //{
                //    BattlePlayer player = mDungeonPlayers.GetMainPlayer();

                //    if (null != player)
                //    {
                //        var actor = player.playerActor;

                //        if (actor != null && !actor.IsDead())
                //        {
                //            BeScene beScene = mDungeonManager.GetBeScene();

                //            if (null != beScene)
                //            {
                //                beScene.ForcePickUpDropItem(actor);
                //            }
                //        }
                //    }
                //}
                BattleDataManager.GetInstance().ConvertDungeonBattleEndInfo(res);
                var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPauseBtn>();
                if (battleUI != null)
                {
                    battleUI.HidePauseButton();
                }
                GameFrameWork.instance.StartCoroutine(_finishProcess(res));

                //     if (eDungeonMode.SyncFrame == GetMode())
                //                 {
                //                     //BattleMain.instance.End();
                // 
                // 					if (RecordServer.GetInstance().IsReplayRecord())
                // 					{
                // 						RecordServer.GetInstance().EndRecord();
                // 					}
                //                 }

                // TODO 这里的逻辑移出去
                if ((byte)Protocol.DungeonScore.C == res.score)
                {
                    Logger.LogProcessFormat("[战斗] 失败");
                    DevelopGuidanceManager.GetInstance().SignalGuidanceEntrace();
                }
            }
            else
            {
                Logger.LogErrorCode(res.result);
            }
            RecordServer.instance.PushReconnectCmd(string.Format("_onSceneDungeonRaceEndRes : {0} End-----------", res.result));
#endif
        }
        #endregion

        #region SceneDungeonReward
        protected void _sendDungeonRewardReq()
        {
#if !LOGIC_SERVER
            if (mDungeonManager == null)
                return;
            if (mDungeonManager.GetDungeonDataManager() == null)
                return;
            _resendNoVertifyDungeonRewards();

            if (_isNeedSendNet())
            {
                mDungeonManager.GetDungeonDataManager().SendPickedDrops();
            }
#endif

        }

        protected void _sendDungeonBossRewardReq()
        {
#if !LOGIC_SERVER
            if (mDungeonManager == null)
                return;
            if (mDungeonManager.GetDungeonDataManager() == null)
                return;
            if (_isNeedSendNet())
            {
                mDungeonManager.GetDungeonDataManager().SendBossDrops();
            }

            {
                Vec3 pos = mDungeonManager.GetBeScene().GeGDeadBossPosition();
                mDungeonManager.GetBeScene().DropItems(mDungeonManager.GetDungeonDataManager().battleInfo.bossDropItems, new VInt3(pos));
            }
#endif
        }

        protected void _resendNoVertifyDungeonRewards()
        {
#if !LOGIC_SERVER
            if (mDungeonManager == null)
                return;
            if (mDungeonManager.GetDungeonDataManager() == null)
                return;
            if (_isNeedSendNet() && mDungeonManager.GetDungeonDataManager().HasNoVertifyDrops())
            {
                mDungeonManager.GetDungeonDataManager().SendNoVertifyDrops();
            }
#endif
        }
        #endregion

        #region SceneDungeonEnterNextArea
        protected void _sendSceneDungeonEnterNextAreaReq(int nextAreaID)
        {
#if !LOGIC_SERVER
            if (_isNeedSendNet())
            {
                mDungeonManager.GetDungeonDataManager().SendSceneDungeonAreaChange(nextAreaID);
            }
#endif
        }
        #endregion

        #region 虚函数
        protected override void _onCreateScene(BeEvent.BeEventParam args)
        {
            _updateDungeonState(false);
        }

        protected override void _onDoorStateChange(BeEvent.BeEventParam args)
        {
            _updateDungeonState(args.m_Bool);
        }

        private int killedMonsterCount = 0;
        protected sealed override void _onMonsterDead(BeEvent.BeEventParam args)
        {
            //Logger.LogErrorFormat("onMonsterDead");
            _sendDungeonRewardReq();
            killedMonsterCount++;
        }
        protected sealed override void _onEggDead(BeEvent.BeEventParam args)
        {
            _sendDungeonRewardReq();
        }

        public override void OnCriticalElementDisappear()
        {
            if (mDungeonManager == null)
            {
                Logger.LogErrorFormat("OnCriticalElementDisappear occur data error!!");
                return;
            }
            if (!mDungeonManager.IsFinishFight())
            {
#if !LOGIC_SERVER
                _sendDungeonRaceEndFail();
#else
                var req = _getDungeonRaceEndTeamReq();
                LogicServer.ReportRaceEndToLogicServer(req);
#endif
                mDungeonManager.FinishFight();
            }

        }

        protected sealed override void _onMonsterRemoved(BeEvent.BeEventParam args)
        {
            //Logger.LogErrorFormat("onMonsterRemoved");
            _sendDungeonKillMonsterReq();
            _sendDungeonRewardReq();
        }

        protected override void _onAreaClear(BeEvent.BeEventParam args)
        {
#if !LOGIC_SERVER
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPve>();
            if (battleUI != null)
                battleUI.CloseLevelTip();
#endif

            if (recordServer != null && recordServer.IsProcessRecord())
            {
                recordServer.RecordProcess("[SCENE]_onAreaClear");
                recordServer.Mark(0x8779900);
                // Mark:0x8779900 [SCENE]_onAreaClear
            }

            if (mDungeonManager.GetDungeonDataManager().IsHellArea())
            {
                Battle.DungeonHellInfo info = mDungeonManager.GetDungeonDataManager().CurrentHellDestructs();
                if (null != info && info.state != Battle.eDungeonHellState.End)
                {
                    return;
                }
            }

            if (mDungeonManager.GetDungeonDataManager().IsBossArea())
            {

                _CheckFightEnd();

                /*
                if (recordServer.IsReplayRecord())
				{
					if (recordServer.IsProcessRecord())
					{
						FrameSync.instance.SetDungeonMode(eDungeonMode.LocalFrame);
						//mDungeonManager.GetBeScene().state = BeSceneState.onFight;
					}
						
					recordServer.EndRecord();
				}
                */


                // TODO 重置玩家信息
                //_resetPlayerActor(false);
                //_hiddenDungeonMap(false);
            }
            else
            {
#if !LOGIC_SERVER
                if (this.thisRoomMonsterCreatedCount > 0)
                {
                    SystemNotifyManager.SystemNotify(6000);
                    PlaySound(5);
                }

                if (_rejectEnterBossAreaInHellMode())
                {
                    mDungeonManager.GetBeScene().SetBossTransportDoorEffectState(false);
                }

                var index = mDungeonManager.GetDungeonDataManager().CurrentIndex();
                areaIndex = (uint)(1 << index) | areaIndex;
                _updateDungeonState(true);
#endif
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnClearDungeonArea);
        }

        private bool _rejectEnterBossAreaInHellMode()
        {
            return GetBattleType() == BattleType.Hell &&
                mDungeonManager.GetDungeonDataManager().IsBossAreaNearby() &&
                0 == mDungeonManager.GetDungeonDataManager().IsHellAreaVisited();
        }

        bool isNormalFinsh = false;
        bool isChapterNoPassed = false;
        private int startFatigue = 0;
        protected override void _onStart()
        {
            startFatigue = PlayerBaseData.GetInstance().fatigue;
            startTime = GetTimeStamp();
            GetChapterPassed();
            _updateDungeonMap();
            _sendDungeonDropProgressReq();
        }

        private void GetChapterPassed()
        {
            isChapterNoPassed = ComCommonChapterInfo._isAllDiffNoScores(dungeonManager.GetDungeonDataManager().id.dungeonID);
        }


        protected override void _onEnd()
        {
            upLoadBattleInfo();
            _resetHellData();

            mStarted = false;
#if !LOGIC_SERVER
            if(mDungeonRaceEndReqCoroutine != null)
            {
                GameFrameWork.instance.StopCoroutine(mDungeonRaceEndReqCoroutine);
            }
#endif
        }

        private void upLoadBattleInfo()
        {
            DungeonDataManager mDungeonData = mDungeonManager.GetDungeonDataManager();
            var players = mDungeonPlayers.GetAllPlayers();
            var dungonId = mDungeonData.id.dungeonID;
            var dungeonMode = mDungeonData.dungeonMode;
            int playersCount = players.Count;
            bool isTeamState = playersCount > 1;
            List<UInt64> ridList = new List<UInt64>();
            string rid = "";
            string rname = "";
            long totalDamage = 0;
            for (int i = 0; i < players.Count; i++)
            {
                var battlePlayer = players[i];
                var racePlayer = battlePlayer.playerInfo;
                ridList.Add(racePlayer.roleId);
                bool isLocalActor = racePlayer.accid == ClientApplication.playerinfo.accid;
                if(isLocalActor)
                {
                    rid = racePlayer.roleId.ToString();
                    rname = racePlayer.name;
                    BeEntity entity = battlePlayer.playerActor.GetTopOwner(battlePlayer.playerActor);
                    if(entity == null || entity.GetEntityData() == null || entity.GetEntityData().battleData == null)
                    {
                        totalDamage = 0;
                    } 
                    else
                    {
                        totalDamage = entity.GetEntityData().battleData.GetTotalDamage();
                    }
                }
            }
            ridList.Sort();

            string rids = "";
            for (int i = 0; i < ridList.Count; i++)
            {
                if(string.IsNullOrEmpty(rids)) 
                {
                    rids = ridList[i].ToString();
                } 
                else 
                {
                    rids = rids + "|" + ridList[i].ToString();
                }
            }

            string iF = "";
            
            if (mDungeonManager.IsFinishFight() && !isDungeonFail)
            {
                iF = "1";
            } 
            else 
            {
                iF = "0";
            }
            int cosumedFatigue = 0;
            if (isEntryDoor)
            {
                cosumedFatigue = startFatigue - PlayerBaseData.GetInstance().fatigue + 1;
            } 
            else
            {
                cosumedFatigue = 1;
            }
            long endTime =  GetTimeStamp();
            long diffTime = endTime - startTime;
            Logger.LogError("isDungeonFail = " + isDungeonFail);
            string param =  "?z=" + players[0].playerInfo.zoneId + "&d=" + dungonId.ToString().Trim()
                            + "&a=" + ClientApplication.playerinfo.accid.ToString().Trim() + "&dmg=" + totalDamage.ToString().Trim()
                            + "&r=" + rid.Trim() + "&km=" + killedMonsterCount.ToString().Trim()
                            + "&at=" + Global.USERNAME + "&rn=" + rname + "&rs=" + rids.Trim()
                            + "&cf=" + cosumedFatigue.ToString().Trim() + "&if=" + iF.Trim()
                            + "&df=" + diffTime.ToString().Trim();
            Logger.LogError("param = " + param);
            GameFrameWork.instance.StartCoroutine(_upLoadBattleInfo(param));
            isDungeonFail = false;
            startTime = 0;
        }

        private IEnumerator _upLoadBattleInfo(string param)
        {
            BaseWaitHttpRequest dungeonWt = new BaseWaitHttpRequest();

            dungeonWt.url = Global.UPLOAD_PLAYER_DUNGEON_INFO_ADDRESS + param;
            
            yield return dungeonWt;
            yield break;
        }

        protected override void _onPostStart()
        {

#if !LOGIC_SERVER
            if (GetMode() != eDungeonMode.Test)
            {

                if (ItemDataManager.GetInstance().IsPackageFull())
                {
                    SystemNotifyManager.SystemNotify(1033);
                }
            }
            if (mDungeonManager == null 
                || mDungeonManager.GetDungeonDataManager() == null
                || mDungeonManager.GetBeScene() == null 
                || mDungeonManager.GetBeScene().sceneData == null) return;
            string diff = ChapterUtility.GetHardString(mDungeonManager.GetDungeonDataManager().id.diffID);
            if (mDungeonManager.GetDungeonDataManager().table.SubType == ProtoTable.DungeonTable.eSubType.S_CITYMONSTER ||
                mDungeonManager.GetDungeonDataManager().table.SubType == ProtoTable.DungeonTable.eSubType.S_GUILD_DUNGEON)
                diff = "";
            else if (mDungeonManager.GetDungeonDataManager().table.SubType == ProtoTable.DungeonTable.eSubType.S_LIMIT_TIME_HELL ||
                    mDungeonManager.GetDungeonDataManager().table.SubType == ProtoTable.DungeonTable.eSubType.S_LIMIT_TIME__FREE_HELL||
                    mDungeonManager.GetDungeonDataManager().table.SubType == ProtoTable.DungeonTable.eSubType.S_WEEK_HELL ||
                    mDungeonManager.GetDungeonDataManager().table.SubType == ProtoTable.DungeonTable.eSubType.S_WEEK_HELL_PER ||
                    mDungeonManager.GetDungeonDataManager().table.SubType == ProtoTable.DungeonTable.eSubType.S_ANNIVERSARY_HARD ||
                    mDungeonManager.GetDungeonDataManager().table.SubType == ProtoTable.DungeonTable.eSubType.S_ANNIVERSARY_NORMAL)
            {
                diff = ChapterUtility.GetHardString(3);
            }
            else if (mDungeonManager.GetDungeonDataManager().IsHardRaid)
            {
                diff = "噩梦";
            }

            SystemNotifyManager.SystemNotify(6001,
                mDungeonManager.GetDungeonDataManager().table.Name,
                diff
            );

            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPve>();
            if (battleUI != null)
                battleUI.ShowLevelTip(mDungeonManager.GetBeScene().sceneData.GetTipsID());
            mStarted = true;

            if (GetBattleType() == BattleType.Hell)
            {
                GameFrameWork.instance.StartCoroutine(_hellEnterTips());
            }
            ShowAlienLand();
#endif

            var mBeScene = mDungeonManager.GetBeScene();
            if (mBeScene != null)
            {
                mBeScene.RegisterEventNew(BeEventSceneType.onBossDead, (args) => {

                    var players = mDungeonPlayers.GetAllPlayers();
                    for (int i = 0; i < players.Count; ++i)
                    {
                        var actor = players[i].playerActor;
                        if (actor != null && !actor.IsDead())
                            actor.SetAutoFight(false);
                        if (actor != null && actor.IsDead() && actor.GetRecordServer().IsProcessRecord())
                        {
                            actor.GetRecordServer().RecordProcess("this actor is dead");
                            actor.GetRecordServer().Mark(0xb4422741);
                            // Mark:0xb4422741 this actor is dead
                        }
                    }
                    /*
					if (system != null)
					{
						system.SetAutoFightVisible(false);
					}*/

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnDungeonBossKilled);
                });
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnEnterDungeonArea);
        }

        private void _showActivityMonsterTips()
        {
#if !LOGIC_SERVER
            var mDungeonData = mDungeonManager.GetDungeonDataManager();
            var monsters = mDungeonData.CurrentMonsters();

            for (int i = 0; i < monsters.Count; ++i)
            {
                Battle.DungeonMonster monsterUnit = monsters[i];

                if (null != monsterUnit)
                {
                    string monsterName = string.Empty;
                    MonsterID monsterID = new MonsterID
                    {
                        resID = monsterUnit.typeId,
                        monsterLevel = 0
                    };

                    ProtoTable.UnitTable unit = TableManager.instance.GetTableItem<ProtoTable.UnitTable>(monsterID.resID);

                    if (null != unit)
                    {
                        monsterName = unit.Name;
                    }

                    if (!string.IsNullOrEmpty(monsterName))
                    {
                        switch (monsterUnit.monsterType)
                        {
                            case DEntityType.ACTIVITY_BOSS_POS:
                                SystemNotifyManager.SystemNotify(1281, monsterName);
                                break;
                            case DEntityType.ACTIVITY_ELITE_POS:
                                SystemNotifyManager.SystemNotify(1280, monsterName);
                                break;
                        }
                    }
                }
            }
#endif
        }

        protected void ShowAlienLand()
        {
#if !LOGIC_SERVER
            // 异界管卡需要判断每个房间的boss击杀次数是否用完,用完就给个提示
            DungeonDataManager manager = mDungeonManager.GetDungeonDataManager();
            if (manager == null)
            {
                return;
            }

            Battle.BattleInfo info = manager.battleInfo;
            if (info == null)
            {
                return;
            }

            DungeonTable tableData = TableManager.GetInstance().GetTableItem<DungeonTable>(info.dungeonId);
            if (tableData == null)
            {
                return;
            }

            if (tableData.SubType != DungeonTable.eSubType.S_DEVILDDOM)
            {
                return;
            }

            if (info.dropOverMonster == null)
            {
                return;
            }

            List<Battle.DungeonArea> areas = info.areas;
            if(areas == null)
            {
                return;
            }

            for(int i = 0; i < areas.Count; i++)
            {
                if(areas[i].id != manager.CurrentAreaID())
                {
                    continue;
                }

                IList<Battle.DungeonMonster> monsters = areas[i].FirstMonsterList;
                if(monsters == null)
                {
                    return;
                }

                for (int j = 0; j < monsters.Count; j++)
                {
                    var findValue = info.dropOverMonster.Find(value => { return value == monsters[j].id; });

                    if (findValue != 0)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("resist_magic_challenge_reward_over"));
                        break;
                    }

                }

                break;
            }
#endif
        }

        protected sealed override void _createRegions()
        {
            var mBeScene = mDungeonManager.GetBeScene();
            var mDungeonData = mDungeonManager.GetDungeonDataManager();
            var regions = mDungeonData.CurrentRegions();

            if (null != mBeScene)
            {
                mBeScene.CreateRegions(_doorTriggerAllPlayers, regions);
            }
        }


        protected sealed override void _createEntitys()
        {
            base._createEntitys();

            // 玩家朝向问题
            var mBeScene = mDungeonManager.GetBeScene();
            if (null != mBeScene)
            {
                BeActor monster = mBeScene.FindAPendingMonster();
                if (null != monster)
                {
                    bool faceFlag = monster.GetFace();
                    var players = mDungeonPlayers.GetAllPlayers();
                    if (null != players)
                    {
                        int countPlayer = players.Count;
                        for (int i = 0; i < countPlayer; i++)
                        {
                            BeActor curActor = players[i].playerActor;
                            if (null != curActor)
                            {
                                bool faceLeft = (monster.GetPosition().x - curActor.GetPosition().x) > 0 ? false : true;
                                curActor.SetFace(faceLeft);
                            }
                        }
                    }
                }
            }

#if !LOGIC_SERVER
            if (mStarted)
            {
                var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPve>();
                if (battleUI != null)
                {
                    battleUI.ShowLevelTip(mDungeonManager.GetBeScene().sceneData.GetTipsID());
                }
            }
#endif
        }

        protected sealed override void _createHealDestruct()
        {
            var beScene = mDungeonManager.GetBeScene();
            var dungeonData = mDungeonManager.GetDungeonDataManager();
            var info = dungeonData.CurrentHellDestructs();

            _resetHellData();

            if (null != info)
            {
                mCurrentHellObject = beScene.CreateHellDestruct(info);
                mHellState = eHellProcessState.onHellInit;
            }
        }

        protected override void _createMonsters()
        {
            var mBeScene = mDungeonManager.GetBeScene();
            var mDungeonData = mDungeonManager.GetDungeonDataManager();
            var monsters = mDungeonData.CurrentMonsters();
            var monsterCreatedCount = mBeScene.CreateMonsterList(monsters, mDungeonData.IsBossArea(), mDungeonData.GetBirthPosition());
            this.thisRoomMonsterCreatedCount = monsterCreatedCount;
        }

        protected sealed override void _createDestructs()
        {
            var mBeScene = mDungeonManager.GetBeScene();
            var mDungeonData = mDungeonManager.GetDungeonDataManager();
            var destructs = mDungeonData.CurrentDestructs();
            mBeScene.CreateDestructList2(destructs);
        }

        #region Door
        private List<BattlePlayer> mCachBeActor = new List<BattlePlayer>();
        protected List<BattlePlayer> _doorTriggerAllPlayers()
        {
            if (null != mDungeonPlayers)
            {
                var players = mDungeonPlayers.GetAllPlayers();

                mCachBeActor.Clear();

                for (int i = 0; i < players.Count; ++i)
                {
                    mCachBeActor.Add(players[i]);
                }
            }

            return mCachBeActor;
        }

        protected void _changeAreaFade(uint fadein, uint fadeout, DungeonFadeCallback cb, DungeonFadeCallback finishcb = null)
        {
            mDungeonManager.OpenFade(
                () =>
                {
                    if (FrameSync.instance != null)
                    {
                        // 在这里重置一些东西
                        FrameSync.instance.ResetMove();

                        // 禁用输入
                        FrameSync.instance.isFire = false;
                    }
                    
                    _resetPlayerActor(false);
                    InputManager.isForceLock = true;
                },
                cb,
                () =>
                {
                    if (null != finishcb)
                    {
                        finishcb.Invoke();
                    }
                    if (FrameSync.instance != null)
                    {
                        // 恢复输入
                        FrameSync.instance.isFire = true;
                    }
                    InputManager.isForceLock = false;
#if !LOGIC_SERVER
                    var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUICommon>();
                    if (battleUI != null && battleUI.comShowHit != null)
                    {
                        battleUI.comShowHit.ClearHitNumber();
                    }
#endif

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnEnterDungeonArea);
                },
                fadein,
                fadeout
            );

        }

        bool forceChangeRoom = false;
        public void ForceChangeRoom(bool force = false)
        {
            var scene = mDungeonManager.GetBeScene();
            if (scene == null || scene.IsBossSceneClear())
            {
                return;
            }

            if (scene.state < BeSceneState.onClear)
            {
                return;
            }

            if (forceChangeRoom && !force)
            {
                return;
            }
            forceChangeRoom = true;

            {
#if !LOGIC_SERVER
                startPassDoor = (int)Time.realtimeSinceStartup;
#endif

                var beScene = mDungeonManager.GetBeScene();
                if (null != beScene)
                {
#if !SERVER_LOGIC

                    var actor = mDungeonPlayers.GetMainPlayer().playerActor;
                    if (actor != null && !actor.IsDead())
                        beScene.ForcePickUpDropItem(actor);

#endif
                }

                {
                    {
                        _changeAreaFade(600, 300,
                        () =>
                        {
                        //int playerCount = mDungeonPlayers.GetAllPlayers().Count;
                        //if (GetMode() == eDungeonMode.SyncFrame && playerCount >= 1)

                        {
                            //if (mDungeonManager.GetDungeonDataManager().IsHellArea() && SwitchFunctionUtility.IsOpen(8))
                            //	AssetGabageCollector.instance.ClearUnusedAsset();
                            //else
#if !LOGIC_SERVER
                            //内存大于2G并且是高配就不GC
                            if (!PluginManager.instance.IsLargeMemoryDevice() || !GeGraphicSetting.instance.IsHighLevel())
                                {

                                    AssetGabageCollector.instance.ClearUnusedAsset(CResPreloader.instance.priorityGameObjectKeys);
                                }

                                else
                                {
                                // 
                                //                                 if (mDungeonManager.GetDungeonDataManager().IsNextAreaBoss())
                                //                                 {
                                //                                     Logger.LogErrorFormat("深渊 执行GC!!!!");
                                //                                     AssetGabageCollector.instance.ClearUnusedAsset(CResPreloader.instance.priorityGameObjectKeys);
                                //                                 }
                                //                                 else
                                DoSpecialGCClear(true);
                                }
                                if (RecordServer.instance != null)
                                    RecordServer.instance.FlushProcess();
#endif
                        }
                            bool isFinishFight = false;
                        //针对于团本联动 服务器发送其他团队结算达到自己胜利条件后，自己已经结算的判断保护
                        if (BattleType.RaidPVE == this.GetBattleType())
                            {
                                isFinishFight = mDungeonManager.IsFinishFight();
                            }

                            var players = mDungeonPlayers.GetAllPlayers();
                            for (int i = 0; i < players.Count; ++i)
                            {
                                players[i].playerActor.TriggerEventNew(BeEventType.OnBeforePassDoor);
                            }

                            TMBattleAssetLoadRecord.instance.AssetLoadFlag = BattleAssetLoadFlag.PassingDoor;

#if STAT_EXTRA_INFO
                        bool nextAreaVisited = mDungeonManager.GetDungeonDataManager().IsNextAreaVisited(door.GetDoortype());
#endif


                            if (mDungeonManager.GetDungeonDataManager().NextAreaByIndex(0, true))
                            {
                                for (int i = 0; i < players.Count; ++i)
                                {
                                    players[i].playerActor.TriggerEventNew(BeEventType.onStartPassDoor);
                                }

#if !LOGIC_SERVER
                            SystemNotifyManager.ClearDungeonSkillTip();
#endif

                                _createBase();                
                                _createEntitys();
#if !LOGIC_SERVER
                            if (Global.PRELOAD_MODE != PreloadMode.ALL)
                                    PreloadMonster();
#endif
                            _onSceneStart();
                                mDungeonManager.StartFight(isFinishFight);


#if !LOGIC_SERVER
                            _sendSceneDungeonEnterNextAreaReq(mDungeonManager.GetDungeonDataManager().CurrentAreaID());
                                _sendDungeonRewardReq();
#endif

#if STAT_EXTRA_INFO
#if !LOGIC_SERVER
	                            if (!nextAreaVisited)
	                            {
	                                var passDoorDuration = (int)Time.realtimeSinceStartup - startPassDoor;
	                                //Logger.LogErrorFormat("passDoorDuration:{0}", passDoorDuration);
	                                GameStatisticManager.GetInstance().DoStatSmallPackageInfo(GameStatisticManager.ExtraInfo.PASSDOOR_DUNGEON, passDoorDuration.ToString());
	                            }           
#endif
#endif

                        }
                            else
                            {
                            // TOOD change area error
                        }
                        },
                        () => {

                            if (mLevelMgr != null)
                            {
                                mLevelMgr.PassedDoor();
                            }

                            TMBattleAssetLoadRecord.instance.AssetLoadFlag = BattleAssetLoadFlag.Fighting;

                            var players = mDungeonPlayers.GetAllPlayers();
                            for (int i = 0; i < players.Count; ++i)
                            {
                                players[i].playerActor.ShowTransport(false);
                                players[i].playerActor.TriggerEventNew(BeEventType.onPassedDoor);

                                if (players[i].playerActor.pet != null)
                                {
                                    players[i].playerActor.SetPetAlongside();
                                }

                                var beActor = players[i].playerActor;



                                if (beActor.aiManager != null && beActor.aiManager.isAutoFight)
                                {
                                    beActor.aiManager.StopCurrentCommand();
                                    if (beActor != null && beActor.IsProcessRecord())
                                    {
                                        beActor.GetRecordServer().RecordProcess("[AI]PID:{0} StopCurrentComand", beActor.GetPID());
                                        beActor.GetRecordServer().MarkInt(0x8779812, beActor.GetPID());
                                        // Mark:0x8779812 [AI]PID:{0} StopCurrentComand
                                    }
                                }

                                if (beActor != null)
                                {
                                    beActor.SetPosition(mDungeonManager.GetDungeonDataManager().CurrentBirthPosition());
                                }
                            }

                            _showActivityMonsterTips();
                            ShowAlienLand();                   

                            forceChangeRoom = false;
                        });
                    }
                }
            }
        }

        private int startPassDoor = 0;
        private bool _doorCallback(ISceneRegionInfoData info, BeRegionTarget target)
        {
#if !LOGIC_SERVER
            startPassDoor = (int)Time.realtimeSinceStartup;
#endif

            bool ret = true;

            var beScene = mDungeonManager.GetBeScene();
            if (null != beScene)
            {
#if !SERVER_LOGIC

                var actor = mDungeonPlayers.GetMainPlayer().playerActor;
                if (actor != null && !actor.IsDead())
                    beScene.ForcePickUpDropItem(actor);

#endif

            }

            ISceneTransportDoorData door = info as ISceneTransportDoorData;
            if (null != door)
            {
                if (mHellState == eHellProcessState.onHellProcessFinish || mHellState == eHellProcessState.onHellProcessReportFinish)
                {
                    ret = false;
                }
                else if (_rejectEnterBossAreaInHellMode() &&
                    mDungeonManager.GetDungeonDataManager().IsNextAreaBoss(door.GetDoortype()))
                {
#if !LOGIC_SERVER
                    SystemNotifyManager.SystemNotify(5018);
#endif
                    ret = false;
                }
                else if (mDungeonManager.GetDungeonDataManager().IsYiJieCheckPoint() && mDungeonManager.GetDungeonDataManager().IsNextAreaBoss(door.GetDoortype()) && !eggRoomOpen)
                {
                    ret = false;
                }

                else
                {
                    _changeAreaFade(600, 300,
                    () =>
                    {
                        //int playerCount = mDungeonPlayers.GetAllPlayers().Count;
                        //if (GetMode() == eDungeonMode.SyncFrame && playerCount >= 1)

                        {
                            //if (mDungeonManager.GetDungeonDataManager().IsHellArea() && SwitchFunctionUtility.IsOpen(8))
                            //	AssetGabageCollector.instance.ClearUnusedAsset();
                            //else
#if !LOGIC_SERVER
                            //内存大于2G并且是高配就不GC
                            if (!PluginManager.instance.IsLargeMemoryDevice() || !GeGraphicSetting.instance.IsHighLevel())
                            {

                                AssetGabageCollector.instance.ClearUnusedAsset(CResPreloader.instance.priorityGameObjectKeys);
                            }

                            else
                            {
                                // 
                                //                                 if (mDungeonManager.GetDungeonDataManager().IsNextAreaBoss())
                                //                                 {
                                //                                     Logger.LogErrorFormat("深渊 执行GC!!!!");
                                //                                     AssetGabageCollector.instance.ClearUnusedAsset(CResPreloader.instance.priorityGameObjectKeys);
                                //                                 }
                                //                                 else
                                DoSpecialGCClear(true);
                            }
                            if (RecordServer.instance != null)
                                RecordServer.instance.FlushProcess();
#endif
                        }
                        bool isFinishFight = false;
                        //针对于团本联动 服务器发送其他团队结算达到自己胜利条件后，自己已经结算的判断保护
                        if(BattleType.RaidPVE == this.GetBattleType())
                        {
                            isFinishFight = mDungeonManager.IsFinishFight();
                        }
                        
                        var players = mDungeonPlayers.GetAllPlayers();
                        for (int i = 0; i < players.Count; ++i)
                        {
                            players[i].playerActor.TriggerEventNew(BeEventType.OnBeforePassDoor);
                        }

#if !LOGIC_SERVER
                        TMBattleAssetLoadRecord.instance.AssetLoadFlag = BattleAssetLoadFlag.PassingDoor;
#endif

#if STAT_EXTRA_INFO
                        bool nextAreaVisited = mDungeonManager.GetDungeonDataManager().IsNextAreaVisited(door.GetDoortype());
#endif
                        if (mDungeonManager.GetDungeonDataManager().NextArea(door.GetDoortype()))
                        {
                            for (int i = 0; i < players.Count; ++i)
                            {
                                players[i].playerActor.TriggerEventNew(BeEventType.onStartPassDoor);
                            }

#if !LOGIC_SERVER
                            SystemNotifyManager.ClearDungeonSkillTip();
#endif

                            if (recordServer != null && recordServer.IsProcessRecord())
                            {
                                recordServer.RecordProcess("[BATTLE]_doorCallback FROM {0} TO {1} area:{2}",
                                    door.GetDoortype(), door.GetNextdoortype(), mDungeonManager.GetDungeonDataManager().CurrentScenePath());
                                recordServer.MarkString(0x8779811, door.GetDoortype().ToString(), door.GetNextdoortype().ToString(), mDungeonManager.GetDungeonDataManager().CurrentScenePath());
                                // Mark:0x8779811 [BATTLE]_doorCallback FROM {0} TO {1} area:{2}
                            }
                            _createBase();
                            _createEntitys();
#if !LOGIC_SERVER
                            if (Global.PRELOAD_MODE != PreloadMode.ALL)
                                PreloadMonster();
#endif
                            _onSceneStart();
                            mDungeonManager.StartFight(isFinishFight);


#if !LOGIC_SERVER
                            _sendSceneDungeonEnterNextAreaReq(mDungeonManager.GetDungeonDataManager().CurrentAreaID());
                            _sendDungeonRewardReq();
#endif

#if STAT_EXTRA_INFO
	#if !LOGIC_SERVER
	                            if (!nextAreaVisited)
	                            {
	                                var passDoorDuration = (int)Time.realtimeSinceStartup - startPassDoor;
	                                //Logger.LogErrorFormat("passDoorDuration:{0}", passDoorDuration);
	                                GameStatisticManager.GetInstance().DoStatSmallPackageInfo(GameStatisticManager.ExtraInfo.PASSDOOR_DUNGEON, passDoorDuration.ToString());
	                            }           
	#endif 
#endif

                        }
                        else
                        {
                            // TOOD change area error
                        }
                    },
                    () => {

                        if (mLevelMgr != null)
                        {
                            mLevelMgr.PassedDoor();
                        }

#if !LOGIC_SERVER
                        TMBattleAssetLoadRecord.instance.AssetLoadFlag = BattleAssetLoadFlag.Fighting;
#endif

                        var players = mDungeonPlayers.GetAllPlayers();
                        for (int i = 0; i < players.Count; ++i)
                        {
                            players[i].playerActor.ShowTransport(false);
                            players[i].playerActor.TriggerEventNew(BeEventType.onPassedDoor);

                            if (players[i].playerActor.pet != null)
                            {
                                players[i].playerActor.SetPetAlongside();
                            }

                            var beActor = players[i].playerActor;



                            if (beActor.aiManager != null && beActor.aiManager.isAutoFight)
                            {
                                beActor.aiManager.StopCurrentCommand();
                                if (beActor != null && beActor.IsProcessRecord())
                                {
                                    beActor.GetRecordServer().RecordProcess("[AI]PID:{0} StopCurrentComand", beActor.GetPID());
                                    beActor.GetRecordServer().MarkInt(0x8779812, beActor.GetPID());
                                    // Mark:0x8779812 [AI]PID:{0} StopCurrentComand
                                }
                            }

                        }
                            isEntryDoor = true;
							_showActivityMonsterTips();
                            ShowAlienLand();  
                    });
                }
            }

            return ret;
        }

        private bool isEntryDoor = false;

        protected sealed override bool _isBattleLoadFinish()
        {
            bool loadFinish = true;
            var players = mDungeonPlayers.GetAllPlayers();
            if (null != players)
            {
                for (int i = 0; i < players.Count; ++i)
                {
                    var currentBattlePlayer = players[i];
                    if (null != currentBattlePlayer)
                    {
                        BeActor beActor = currentBattlePlayer.playerActor;
                        if (null != beActor && null != beActor.m_pkGeActor)
                            loadFinish = loadFinish ? beActor.m_pkGeActor.IsAvatarLoadFinished() : loadFinish;
                    }
                }
            }

            return loadFinish;
        }

        protected void _resetPlayerActor(bool force = true)
        {
            var players = mDungeonPlayers.GetAllPlayers();

            for (int i = 0; i < players.Count; ++i)
            {
                var beActor = players[i].playerActor;
                if (null != beActor)
                {
                    if (force)
                    {
                        beActor.sgSwitchStates(new BeStateData((int)ActionState.AS_IDLE));
                    }
                    else
                    {
                        beActor.sgPushState(new BeStateData((int)ActionState.AS_IDLE));
                    }
                    
                    beActor.Reset();
                    beActor.SetAttackButtonState(ButtonState.RELEASE);
                }
            }
        }

        protected override void _createDoors()
        {
            var mBeScene = mDungeonManager.GetBeScene();
            var mDungeonData = mDungeonManager.GetDungeonDataManager();
            var doors = mDungeonData.CurrentDoors();

            var chanceDoorType = mBeScene.GetChanceDoorType();


            for (int i = 0; i < doors.Count; ++i)
            {
                if (null != doors[i].door)
                {
                    /*					if (!BattleMain.instance.HasBattleFlag(BattleFlag.NPC_PROTECT) || 
                                            (BattleMain.instance.HasBattleFlag(BattleFlag.NPC_PROTECT) && doors[i].doorType == chanceDoorType))*/
                    mBeScene.AddTransportDoor(doors[i].door.GetRegionInfo(), _doorTriggerAllPlayers, _doorCallback, doors[i].isconnectwithboss, doors[i].isvisited, doors[i].doorType,doors[i].isEggDoor, doors[i].materialPath);
                }
            }
        }
        #endregion

        #region HellProcess
        private IEnumerator _hellEnterTips()
        {
#if !LOGIC_SERVER
            ClientSystemManager.instance.CloseFrame<DungeonHellTipsFrame>();

            DungeonHellTipsFrame frame = ClientSystemManager.instance.OpenFrame<DungeonHellTipsFrame>(FrameLayer.TopMost) as DungeonHellTipsFrame;
            frame.SetHellType(BattleDataManager.GetInstance().BattleInfo.dungeonHealInfo.mode);

            yield return Yielders.GetWaitForSeconds(3.2f);

            ClientSystemManager.instance.CloseFrame<DungeonHellTipsFrame>();
#endif
            yield break;
        }
        #endregion


        public void SetAccompanyInfo(BattlePlayer battlePlayer)
        {
            if (true ||
                GetMode() == eDungeonMode.LocalFrame ||
                GetMode() == eDungeonMode.Test)
            {

                var racePlayer = battlePlayer.playerInfo;
                for (int i = 0; i < racePlayer.retinues.Length; ++i)
                {
                    if (racePlayer.retinues[i].isMain > 0)
                    {
//                         var retinueData = TableManager.GetInstance().GetTableItem<ProtoTable.FollowPetTable>((int)(racePlayer.retinues[i].dataId));
//                         if (retinueData != null)
//                         {
//                             AccompanyData adata = new AccompanyData
//                             {
//                                 id = retinueData.MonsterID,
//                                 skillID = retinueData.Skills[0],
//                                 level = racePlayer.retinues[i].level
//                             };
// 
//                             battlePlayer.playerActor.SetAccompanyData(adata);
//                         }
                        break;
                    }
                }

                /*var data = RetinueManager.GetInstance().GetRetineData(RetinueSlotType.RST_MAIN);
                if (data != null)
                {
                    var mCurBeScene = mDungeonManager.GetBeScene();
					var accompanyData = data.item;
                    if (accompanyData != null)
                    {
                        // TODO 这里默认把随从的血条放在了头像血条边上的位置
                        //summon.m_pkGeActor.CreateHPBarCharactor(1);

						AccompanyData adata = new AccompanyData();
						adata.id = accompanyData.MonsterID;
						adata.skillID = data.kMainSkill.ID;
						adata.level = data.iLevel;

						actor.SetAccompanyData(adata);
                    }
                }*/
            }
        }

        protected override void _createPlayers()
        {
            var mBeScene = mDungeonManager.GetBeScene();
            var mDungeonData = mDungeonManager.GetDungeonDataManager();
            var players = mDungeonPlayers.GetAllPlayers();


            #region get pos
            VInt3 bornPos = mDungeonData.GetBirthPosition();
            VInt3[] poses = new VInt3[5];
            poses[0] = bornPos;

            int count = 1;

            for (int i = 1; i <= players.Count - 1; ++i)
            {
                var tmp = bornPos;
                tmp.x += BeAIManager.DIR_VALUE2[i - 1, 0] * VInt.one.i;
                tmp.y += BeAIManager.DIR_VALUE2[i - 1, 1] * VInt.one.i;

                if (!mBeScene.IsInBlockPlayer(tmp))
                {
                    poses[count++] = tmp;
                }
            }

            for (int i = count; i <= players.Count - 1; ++i)
                poses[i] = bornPos;
            #endregion

            for (int i = 0; i < players.Count; ++i)
            {
                var battlePlayer = players[i];



                if (battlePlayer.playerActor == null)
                {
                    var racePlayer = battlePlayer.playerInfo;

                    var petData = BattlePlayer.GetPetData(racePlayer,false);

                    bool isLocalActor = racePlayer.accid == ClientApplication.playerinfo.accid;
                    bool isShowFashionWeapon = racePlayer.avatar.isShoWeapon == 1 ? true : false;
                    bool isAIRobot = racePlayer.robotAIType > 0 ? true : false;

                    var actor = mBeScene.CreateCharacter(
                        isLocalActor,
                        racePlayer.occupation,
                        racePlayer.level,
                        (int)ProtoTable.UnitTable.eCamp.C_HERO,
                        BattlePlayer.GetSkillInfo(racePlayer),
                        BattlePlayer.GetEquips(racePlayer,false),
                        BattlePlayer.GetBuffList(racePlayer),
                        racePlayer.seat,
                        racePlayer.name,
                        BattlePlayer.GetWeaponStrengthenLevel(racePlayer),
                        BattlePlayer.GetRankBuff(racePlayer),
                        petData,
                        BattlePlayer.GetSideEquips(racePlayer,false),
                        BattlePlayer.GetAvatar(racePlayer),
                        isShowFashionWeapon,
                        isAIRobot
                    );

                    actor.InitChangeEquipData(racePlayer.equips, racePlayer.equipScheme);
                    actor.SetScale(VInt.Float2VIntValue(Global.Settings.charScale));
                    if (actor.GetEntityData() != null)
                        actor.GetEntityData().SetCrystalNum(BattlePlayer.GetCrsytalNum(racePlayer));

                    battlePlayer.playerActor = actor;

                    actor.skillController.skillSlotMap = BattlePlayer.GetSkillSlotMap(racePlayer);
                    actor.SetPosition(mDungeonData.GetBirthPosition(), true);
                    actor.isMainActor = true;
                    actor.UseProtect();
                    actor.m_iRemoveTime = Int32.MaxValue;

                    if (null != actor.m_pkGeActor)
                    {
                        actor.m_pkGeActor.AddSimpleShadow(Vector3.one);
                    }

#if !LOGIC_SERVER
                    actor.m_pkGeActor.AddTittleComponent(BattlePlayer.GetTitleID(racePlayer), racePlayer.name, 0, "", racePlayer.level, (int)racePlayer.seasonLevel, PlayerInfoColor.LEVEL_PLAYER);
                    if (racePlayer.accid == ClientApplication.playerinfo.accid)
                        actor.m_pkGeActor.CreateInfoBar(racePlayer.name, PlayerInfoColor.TOWN_PLAYER, racePlayer.level);
                    else
                        actor.m_pkGeActor.CreateInfoBar(racePlayer.name, PlayerInfoColor.LEVEL_PLAYER, racePlayer.level);
#endif


                    if (racePlayer.accid == ClientApplication.playerinfo.accid)
                    {
#if !LOGIC_SERVER
                        mDungeonManager.GetGeScene().AttachCameraTo(actor.m_pkGeActor);
#endif


                        //初始化伤害统计数据
                        actor.skillDamageManager.InitData(mBeScene);
                        
                        actor.isLocalActor = true;
                        actor.UseActorData();
                        actor.m_pkGeActor.CreateFootIndicator("Effects/Common/Sfx/Jiaodi/Prefab/Eff_jiaodidingwei_guo");
                    }
                    else {

#if !LOGIC_SERVER
                        //画质设置：低配不显示友军特效
                        if (GeGraphicSetting.instance.IsLowLevel())
                        {
                            var effectManager = actor.m_pkGeActor.GetEffectManager();
                            if (effectManager != null)
                            {
                                effectManager.useCube = true;
                            }
                        }
#endif
                    }

                    // TODO battle Player Dead
                    actor.RegisterEventNew(BeEventType.onAfterDead, arsg =>
                    {
                        if (battlePlayer.state != BattlePlayer.EState.Dead)
                        {
                            _onPlayerDead(battlePlayer);
                        }


#if !LOGIC_SERVER
                        GameStatisticManager.instance.DoStatInBattleEx(StatInBattleType.PLAYER_DEAD,
                                mDungeonManager.GetDungeonDataManager().id.dungeonID,
                                mDungeonManager.GetDungeonDataManager().CurrentAreaID(),
                                string.Format("{0}, {1}", battlePlayer.playerInfo.roleId, battlePlayer.statistics.data.deadCount));
#endif

                    });

                    actor.RegisterEventNew(BeEventType.onDeadProtectEnd,args=>
                    {
#if !LOGIC_SERVER
                        RecordServer.instance.PushReconnectCmd(string.Format("onDeadProtectEnd {0}", battlePlayer.playerActor.GetPID()));
#endif
                        _CheckFightEnd();
                    });

                    actor.RegisterEventNew(BeEventType.onDead, arsg =>
                    {
                        if (battlePlayer.state != BattlePlayer.EState.Dead)
                        {
#if !LOGIC_SERVER
                            //主角自己 并且是异界关卡
                            if (battlePlayer.IsLocalPlayer() &&
                                mDungeonManager != null &&
                                mDungeonManager.GetDungeonDataManager() != null &&
                                mDungeonManager.GetDungeonDataManager().isDeViILDdOM() && 
                                GameStatisticManager.GetInstance() != null)
                            {
                                GameStatisticManager.GetInstance().DoStartAnotherWorldDie(mDungeonManager.GetDungeonDataManager().CurrentAreaID());
                            }
                            RecordServer.instance.PushReconnectCmd(string.Format("BeEventType.onDead {0}", battlePlayer.playerActor.GetPID()));
#endif
                            bool isAllPlayerDead = true;
                            int loopIndex = 0;
                            for (loopIndex = 0; loopIndex < players.Count; loopIndex++)
                            {
                                var player = players[loopIndex];
                                if (!player.playerActor.IsDead())
                                {
                                    isAllPlayerDead = false;
                                    break;
                                }
                            }
                           
                            if (isAllPlayerDead)
                            {
                                for (loopIndex = 0; loopIndex < players.Count; loopIndex++)
                                {
                                    var player = players[loopIndex];
                                    player.playerActor.StartDeadProtect();
                                }
                            }
                        }
                    });

                    actor.RegisterEventNew(BeEventType.onHit, args => {
                    //actor.RegisterEvent(BeEventType.onHit, args => {
                    if (args != null)
                    {
                        var e = args.m_Obj as BeEntity;
                        if (e != null && e.m_iCamp != battlePlayer.playerActor.m_iCamp)
                        {
                            _onPlayerHit(battlePlayer);
                        }
                    }
                    else
                    {
                        _onPlayerHit(battlePlayer);
                    }
                    });

                    actor.RegisterEventNew(BeEventType.onReborn, args => {
                        _onPlayerReborn(battlePlayer);


#if !LOGIC_SERVER
                        GameStatisticManager.instance.DoStatInBattleEx(StatInBattleType.PLAYER_REBORN,
                                mDungeonManager.GetDungeonDataManager().id.dungeonID,
                                mDungeonManager.GetDungeonDataManager().CurrentAreaID(),
                                string.Format("{0}, {1}", battlePlayer.playerInfo.roleId, battlePlayer.statistics.data.rebornCount));
#endif

                    });

                    actor.RegisterEventNew(BeEventType.onCastSkill, args =>
                    {
                        int skillId = args.m_Int;

                        _onPlayerUseSkill(battlePlayer, skillId);

                        /*                        GameStatisticManager.instance.DoStatInBattleEx(StatInBattleType.PLAYER_USESKILL,
                                                        mDungeonManager.GetDungeonDataManager().id.dungeonID,
                                                        mDungeonManager.GetDungeonDataManager().CurrentAreaID(),
                                                        string.Format("{0}", skillId));*/

                    });

                    SetAccompanyInfo(battlePlayer);

                    if (petData != null)
                        actor.SetPetData(petData);
                    actor.CreateFollowMonster();

                    //actor.InitAutoFight();
                    InitAutoFight(actor);
                    ChangeActorAttribute(actor);
                    //actor.forceRunMode = false;
                    actor.SetForceRunMode(false);
#if DEBUG_SETTING
                    if (Global.Settings.isDebug)
                    {
                        if (Global.Settings.playerHP > 0)
                        {
                            //actor.GetEntityData().battleData.hp = Global.Settings.playerHP;
                            //actor.GetEntityData().battleData.maxHp = Global.Settings.playerHP;
                            actor.GetEntityData().SetHP(Global.Settings.playerHP);
                            actor.GetEntityData().SetMaxHP(Global.Settings.playerHP);
                            actor.m_pkGeActor.ResetHPBar();
                        }
                    }
#endif
                }
                else
                {
                    // set transport birth position

                    battlePlayer.playerActor.ResetMoveCmd();
                    if (battlePlayer.playerActor.actionManager != null)
                        battlePlayer.playerActor.actionManager.StopAll();

                }

                battlePlayer.playerActor.SetPosition(poses[i], true);

                mBeScene.InitFriendActor(mDungeonData.GetBirthPosition());
            }
        }
        #endregion

        protected void InitAutoFight(BeActor actor)
        {
            var jobData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(actor.attribute.professtion);
            if (jobData != null)
            {
                actor.InitAutoFight(jobData.AIConfig1, jobData.AIConfig2, jobData.AIConfig3);
            }
        }

        protected virtual void ChangeActorAttribute(BeActor actor)
        {

        }

        #region 虚函们
        protected virtual void _onPlayerHit(BattlePlayer player)
        {
            player.statistics.OnHit();
        }

        protected virtual void _onPlayerReborn(BattlePlayer player)
        {
            if (recordServer != null && recordServer.IsProcessRecord())
            {
                recordServer.RecordProcess("[BATTLE]mid:{0} player reborn", player.playerActor.m_iID);
                recordServer.MarkInt(0x8779813, player.playerActor.m_iID);
                // Mark:0x8779813 [BATTLE]mid:{0} player reborn
            }
            var players = mDungeonPlayers.GetAllPlayers();
            for (int loopIndex = 0; loopIndex < players.Count; loopIndex++)
            {
                var curPlayer = players[loopIndex];
                curPlayer.playerActor.EndDeadProtect();
            }
           
#if !LOGIC_SERVER
            byte seat = player.playerInfo.seat;
            byte mainPlayerSeat = mDungeonPlayers.GetMainPlayer().playerInfo.seat;


            if (BattleMain.IsModeMultiplayer(GetMode()) && seat == mainPlayerSeat)
            {
                mDungeonManager.GetGeScene().AttachCameraTo(player.playerActor.m_pkGeActor);
            }
#endif

            /*			if (seat == ClientApplication.playerinfo.seat && BattleMain.instance.HasBattleFlag(BattleFlag.NPC_PROTECT))
                        {
                            mDungeonManager.ResumeFight();
                        }*/

            player.state = BattlePlayer.EState.Normal;
            player.statistics.Reborn();


#if !LOGIC_SERVER

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonRebornSuccess, player.playerInfo.seat);

            if (mainPlayerSeat == seat)
            {
                if (ClientSystemManager.instance.IsFrameOpen<DungeonRebornFrame>())
                {
                    ClientSystemManager.instance.CloseFrame<DungeonRebornFrame>();
                }
            }
#endif
            _CheckFightEnd();
        }

        protected virtual void _onPlayerDead(BattlePlayer player)
        {
            if (player != null)
            {
                if (recordServer != null && recordServer.IsProcessRecord())
                {
                    recordServer.RecordProcess("[BATTLE]mid:{0} player dead", player.playerActor.m_iID);
                    recordServer.MarkInt(0x8779814, player.playerActor.m_iID);
                    // Mark:0x8779814 [BATTLE]mid:{0} player dead
                }

                _playDungeonDead();

                player.state = BattlePlayer.EState.Dead;
                player.statistics.Dead();
                byte seat = player.playerInfo.seat;
#if !LOGIC_SERVER
                byte mainPlayerSeat = mDungeonPlayers.GetMainPlayer().playerInfo.seat;
                if (seat == mainPlayerSeat)
                {
                    if (BattleMain.IsModeMultiplayer(GetMode()))
                    {
                        BattlePlayer alivePlayer = mDungeonPlayers.GetFirstAlivePlayer();
                        if (null != alivePlayer)
                        {
                            mDungeonManager.GetGeScene().AttachCameraTo(alivePlayer.playerActor.m_pkGeActor);
                        }
                    }
                    else
                    {
                        /*					if (BattleMain.instance.HasBattleFlag(BattleFlag.NPC_PROTECT))
                                            {
                                                mDungeonManager.PauseFight();
                                            }*/
                    }
                }

                if (mDungeonPlayers.IsAllPlayerDead() || seat == mainPlayerSeat)
                {
                    _startPlayerDeadProcess(player);
                }
#endif
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BattlePlayerDead);
        }

        private UnityEngine.Coroutine mDeadProcess = null;

        private void _stopPlayerDeadProcess()
        {
#if !LOGIC_SERVER
            if (null != mDeadProcess)
            {
                GameFrameWork.instance.StopCoroutine(mDeadProcess);
                mDeadProcess = null;
            }
#endif
        }
        private void _startPlayerDeadProcess(BattlePlayer player)
        {
#if !LOGIC_SERVER
            _stopPlayerDeadProcess();

            if (!mDungeonManager.IsFinishFight())
            {
                mDeadProcess = GameFrameWork.instance.StartCoroutine(_playerDeadProcess(player));
            }
#endif
        }

        protected override void _onPlayerLeave(BattlePlayer player)
        {
            if (player != null)
            {
                player.netState = BattlePlayer.eNetState.Offline;

                if (recordServer != null && recordServer.IsProcessRecord())
                {
                    recordServer.RecordProcess("[BATTLE]PID:{0} playerName:{1} _onPlayerLeave", player.playerActor.m_iID, player.playerInfo.name);
                    recordServer.MarkString(0x8779815, player.playerActor.m_iID.ToString(), player.playerInfo.name);
                    // Mark:0x8779815 [BATTLE]PID:{0} playerName:{1} _onPlayerLeave
                }
            }
            if (mDungeonPlayers != null && mDungeonPlayers.IsAllPlayerDead())
            {
                _startPlayerDeadProcess(player);
            }
        }

        protected sealed override void _onPlayerBack(BattlePlayer player)
        {
            if (player != null)
            {
                player.netState = BattlePlayer.eNetState.Online;

                //player.playerActor.ReturnFromBattle();
                if (recordServer != null && recordServer.IsProcessRecord())
                {
                    recordServer.RecordProcess("[BATTLE]PID:{0} playerName:{1} _onPlayerBack", player.playerActor.m_iID, player.playerInfo.name);
                    recordServer.MarkString(0x8779816, player.playerActor.m_iID.ToString(), player.playerInfo.name);
                    // Mark:0x8779816 [BATTLE]PID:{0} playerName:{1} _onPlayerBack
                }
            }
        }

        protected virtual void _onPlayerUseSkill(BattlePlayer player, int skill)
        {
        }

        protected virtual void _onPlayerCancelReborn(BattlePlayer player)
        {
            if (player != null)
            {
                player.playerActor.EndDeadProtect();
            }
#if !LOGIC_SERVER
            RecordServer.instance.PushReconnectCmd(string.Format("_onPlayerCancelReborn {0}", player.playerActor.GetPID()));
#endif
            _CheckFightEnd();
        }

        private IEnumerator _playerDeadProcess(BattlePlayer player)
        {
#if !LOGIC_SERVER
            if(player != null && player.IsLocalPlayer())
            {
                if (ClientSystemManager.instance.IsFrameOpen<DungeonRebornFrame>())
                {
                    ClientSystemManager.instance.CloseFrame<DungeonRebornFrame>();
                }

                ClientSystemManager.instance.OpenFrame<DungeonRebornFrame>(FrameLayer.Middle);
            }

            yield return Yielders.EndOfFrame;
#endif

            while (DungeonRebornFrame.sState == DungeonRebornFrame.eState.None)
            {
                yield return Yielders.EndOfFrame;
            }

            if (DungeonRebornFrame.sState == DungeonRebornFrame.eState.Cancel)
            {
                _onPlayerCancelReborn(player);
#if !LOGIC_SERVER
                if (ClientSystemManager.instance.IsFrameOpen<DungeonRebornFrame>())
                {
                    ClientSystemManager.instance.CloseFrame<DungeonRebornFrame>();
                }
#endif
            }
        }

		public void CreateNPC(int npcID)
		{
			//var npc = mDungeonManager.GetBeScene().CreateMonster(npcID, false, null, 0, (int)ProtoTable.UnitTable.eCamp.C_HERO);
			//BattleMain.instance.SetBattleFlag(BattleFlag.NPC_PROTECT);

			var player = mDungeonPlayers.GetMainPlayer();
			BeActor npc = null;
			object[] summoned = new object[1];

            MonsterIDData idData = new MonsterIDData(npcID);
            if (idData.level <= 0)
                idData.level = 1;

            if (player.playerActor.DoSummon(npcID, idData.level, ProtoTable.EffectTable.eSummonPosType.ORIGIN, null, 1, 0, 0, 0, 0, false, 0, 0, null, SummonDisplayType.NONE, summoned))
			{
                /*
				if (summoned[0] != null)
				{
					npc = (BeActor)summoned[0];
					npc.aiManager.aiType = BeAIManager.AIType.NOATTACK;

					npc.RegisterEvent(BeEventType.onDead, arsg=>{
						//护送NPC死亡，直接关卡失败
						_sendDungeonRaceEndFail();
					});
				}*/
			}

		}

        public  override void FrameUpdate(int delta)
        {
            base.FrameUpdate(delta);
            _updateHellProcess(delta);
        }
        #endregion
        protected void _CheckFightEnd()
        {
            if (mDungeonManager == null || mDungeonPlayers == null)
            {
               // _sendDungeonRaceEndReq(true);
                return;
            }
            if (mDungeonManager.IsFinishFight()) return;
            
            List<BattlePlayer> players = mDungeonPlayers.GetAllPlayers();
            bool isAllPlayerDead = true;
#if !LOGIC_SERVER
            byte mainPlayerSeat = mDungeonPlayers.GetMainPlayer().playerInfo.seat;
#endif
            bool isAllEnemyDead = mDungeonManager.GetBeScene().isAllEnemyDead();
            for (int i = 0; i < players.Count; i++)
            {
                var player = players[i];
                if (!player.playerActor.IsDead() ||
                    player.playerActor.IsInDeadProtect)
                {
                    isAllPlayerDead = false;
                    break;
                }
            }
#if !LOGIC_SERVER
            RecordServer.instance.PushReconnectCmd(string.Format("CheckFightEnd isAllEnemyDead : {0} isAllPlayerDead: {1} IsBossArea :{2}", isAllEnemyDead, isAllPlayerDead, mDungeonManager.GetDungeonDataManager().IsBossArea()));
#endif
            if (isAllEnemyDead && mDungeonManager.GetDungeonDataManager().IsBossArea())
            {
#if !LOGIC_SERVER
                _sendDungeonKillMonsterReq();
                _sendDungeonRewardReq();
                _sendDungeonBossRewardReq();
                _sendDungeonRaceEndReq();              
#else
                 var req = _getDungeonRaceEndTeamReq();
                LogicServer.ReportRaceEndToLogicServer(req);
#endif

            }
            else if(isAllPlayerDead)
            {
#if !LOGIC_SERVER
                _sendDungeonRaceEndReq(true);
#else
                var req = _getDungeonRaceEndTeamReq();
                LogicServer.ReportRaceEndToLogicServer(req);
#endif
            }
            if (/*!isAllPlayerDead &&*/ isAllEnemyDead && mDungeonManager.GetDungeonDataManager().IsBossArea() || isAllPlayerDead)
            {
                mDungeonManager.FinishFight();
#if !LOGIC_SERVER
                if (mDungeonManager.GetDungeonDataManager().isDeViILDdOM())
                    SendGameStatictise();
#endif
            }

        }

        private void SendGameStatictise()
        {
            List<BattlePlayer> players = mDungeonPlayers.GetAllPlayers();
            for (int i = 0; i < players.Count; i++)
            {
                BeActor actor = players[i].playerActor;
                if (actor != null && actor.isLocalActor)
                {
                    int areaID = mDungeonManager.GetDungeonDataManager().CurrentAreaID();
                    Mechanism2004 mechanism = actor.GetMechanism(5300) as Mechanism2004;
                    if (mechanism != null)
                    {
                        GameStatisticManager.instance.SendBatrayCount(mechanism.betrayCnt.ToString(), string.Format("running_{0}", areaID));
                        GameStatisticManager.instance.SendBatrayCount(mechanism.betrayTotal.ToString(), string.Format("end_{0}", dungeonManager.GetDungeonDataManager().id.dungeonID));
                    }
                }
            }
        }
#region 深渊流程
        private void _updateHellProcess(int delta)
        {
            DungeonDataManager     dungeonData = mDungeonManager.GetDungeonDataManager();

            if (null == dungeonData)
            {
                return;
            }

            BeScene                beScene 	   = mDungeonManager.GetBeScene();

            if (null == beScene)
            {
                return ;
            }


            Battle.DungeonHellInfo info        = dungeonData.CurrentHellDestructs();


            if (null == info || info.mode == DungeonHellMode.Null || info.waveInfos.Count < 3)
            {
                return;
            }

            if (null == mCurrentHellObject)
            {
                return;
            }

            switch (mHellState)
            {
                case eHellProcessState.onHellInit:
                    {
                        mCurrentHellObject.SetCanBeAttacked(false);

                        mHasHellPlayBGM = false;

                        if (info.mode == DungeonHellMode.Hard)
                        {
                            mHasHellPlayBGM = PushBgm(dungeonData.table.HellDamnHardBGMPath);
                        }
                        else
                        {
                            mHasHellPlayBGM = PushBgm(dungeonData.table.HellHardBGMPath);
                        }

                        _setHellInfoState(info, Battle.eDungeonHellState.Ready);
                        mCurrentHellObject.currentStage = 0;
                        mCurrentHellObject.sgSwitchStates(new BeStateData((int)ActionState.AS_IDLE));

                        mHellState = eHellProcessState.onHellProcessBattleClear;
                    }
                    break;
                case eHellProcessState.onHellProcessStart:
                    {
                        switch (info.state)
                        {
                            case Battle.eDungeonHellState.Ready:
                                _setHellInfoState(info, Battle.eDungeonHellState.Start);
                                mHellState = eHellProcessState.onHellTipsInit;
                                break;
                            case Battle.eDungeonHellState.Start:
                                break;
                            case Battle.eDungeonHellState.Monster1:
                                mCurrentHellObject.currentStage = 2;
                                mCurrentHellObject.sgSwitchStates(new BeStateData((int)ActionState.AS_IDLE));
                                mHellState = eHellProcessState.onHellTipsInit;
                                break;
                            case Battle.eDungeonHellState.Monster2Pre:
                                {
                                    mHellState = eHellProcessState.onHellTipsInit;
                                }
                                break;
                            case Battle.eDungeonHellState.Monster2:

                                info.state = Battle.eDungeonHellState.End;

                                beScene.state = BeSceneState.onBulletTime;
                                beScene.DropItems(info.dropItems, mCurrentHellObject.GetPosition());
                                mCurrentHellObject.ForceDoDead();

                                if (mHasHellPlayBGM)
                                {
                                    PopBgm();
                                }

                                _sendDungeonRewardReq();

                                beScene.state = BeSceneState.onFight;

#if !LOGIC_SERVER
                                CResPreloader.instance.RemovePriorityKeys(CResPreloader.PreloadTag.HELL);
#endif
                                if (mHellOpen && GetMode() == eDungeonMode.SyncFrame)
                                {
                                    mHellState = eHellProcessState.onHellProcessFinish;
                                    SystemNotifyManager.SystemNotify(5020);
                                    mHellFinishDurTime = 0;
                                }
                                else
                                {
                                    mHellFinishDurTime = -1;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case eHellProcessState.onHellTipsInit:
                    {
                        mWaitHellTipsTimeCounter = kWaitHellTipsTime;
                        string currentTipsDesc = string.Empty;
                        switch (info.state)
                        {
                            case Battle.eDungeonHellState.Start:
                                {
                                    ProtoTable.CommonTipsDesc tab = TableManager.instance.GetTableItem<ProtoTable.CommonTipsDesc>(5015);
                                    currentTipsDesc = tab.Descs;
                                }
                                break;
                            case Battle.eDungeonHellState.Monster1:
                                if (info.mode == DungeonHellMode.Hard)
                                {
                                    ProtoTable.CommonTipsDesc tab = TableManager.instance.GetTableItem<ProtoTable.CommonTipsDesc>(5016);
                                    currentTipsDesc = tab.Descs;
                                }
                                else
                                {
                                    ProtoTable.CommonTipsDesc tab = TableManager.instance.GetTableItem<ProtoTable.CommonTipsDesc>(5017);
                                    currentTipsDesc = tab.Descs;
                                }
                                break;
                            case Battle.eDungeonHellState.Monster2Pre:
                                {
                                    ProtoTable.CommonTipsDesc tab = TableManager.instance.GetTableItem<ProtoTable.CommonTipsDesc>(5017);
                                    currentTipsDesc = tab.Descs;
                                }
                                break;
                        }
                       
                        if (string.IsNullOrEmpty(currentTipsDesc))
                        {
                            mHellState = eHellProcessState.onHellProcessFinish;
                        }
                        else
                        {
                            _setHellTips(currentTipsDesc);
                            mHellState = eHellProcessState.onHellTips;
                        }
                    }
                    break;
                case eHellProcessState.onHellTips:
                    {
                        mWaitOneSeconedCounter -= delta;
                        if (mWaitOneSeconedCounter < 0)
                        {
                            mWaitOneSeconedCounter += 1000;
                            mWaitHellTipsTimeCounter--;

                            if (mWaitHellTipsTimeCounter > 0)
                            {
                                _setHellTipsLeftCount(mWaitHellTipsTimeCounter);
                            }
                            else
                            {
                                _closeHellTips();

                                switch (info.state)
                                {
                                    case Battle.eDungeonHellState.Start:
                                        {
                                            mCurrentHellObject.currentStage = 1;
                                            mCurrentHellObject.sgSwitchStates(new BeStateData((int)ActionState.AS_IDLE));

                                            if (_dropHellWaveMonster(info.waveInfos[0], mCurrentHellObject.GetPosition()))
                                            {
                                                _setHellInfoState(info, Battle.eDungeonHellState.Monster1);

                                                mHellState = eHellProcessState.onHellProcessBattleFight;
                                            }
                                            else
                                            {
                                                mHellState = eHellProcessState.onHellProcessBattleClear;
                                            }
                                        }
                                        break;
                                    case Battle.eDungeonHellState.Monster1:
                                        {
                                            if (info.mode == DungeonHellMode.Hard)
                                            {
                                                if (_dropHellWaveMonster(info.waveInfos[1], mCurrentHellObject.GetPosition()))
                                                {
                                                    _setHellInfoState(info, Battle.eDungeonHellState.Monster2Pre);
                                                    mHellState = eHellProcessState.onHellProcessBattleFight;
                                                }
                                                else
                                                {
                                                    mHellState = eHellProcessState.onHellProcessBattleClear;
                                                }
                                            }
                                            else
                                            {
                                                info.state = Battle.eDungeonHellState.Monster2;

                                                if (_dropHellWaveMonster(info.waveInfos[2], mCurrentHellObject.GetPosition()))
                                                {
                                                    mHellState = eHellProcessState.onHellProcessBattleFight;
                                                }
                                                else
                                                {
                                                    mHellState = eHellProcessState.onHellProcessBattleClear;
                                                }
                                            }
                                        }
                                        break;
                                    case Battle.eDungeonHellState.Monster2Pre:
                                        {
                                            info.state = Battle.eDungeonHellState.Monster2;

                                            if (_dropHellWaveMonster(info.waveInfos[2], mCurrentHellObject.GetPosition()))
                                            {
                                                mHellState = eHellProcessState.onHellProcessBattleFight;
                                            }
                                            else
                                            {
                                                mHellState = eHellProcessState.onHellProcessBattleClear;
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    break;
                case eHellProcessState.onHellProcessBattleFight:
                    { 
                        if (beScene.state == BeSceneState.onFight)
                        {
                            mHellState = eHellProcessState.onHellProcessBattleClear;
                        }
                    }
                    break;
                case eHellProcessState.onHellProcessBattleClear:
                    {
                        if (beScene.state == BeSceneState.onClear)
                        {
                            mHellState = eHellProcessState.onHellProcessStart;
                        }
                    }
                    break;
                case eHellProcessState.onHellProcessFinish:
                    {
                        if (mHellFinishDurTime >= 0)
                        {
                            mHellFinishDurTime += delta;
                            if (mHellFinishDurTime >= 8000)
                            {
                                mHellState = eHellProcessState.onHellProcessReportFinish;
#if !LOGIC_SERVER
                                if (RecordServer.GetInstance() != null && RecordServer.GetInstance().IsReplayRecord())
                                {
                                    RecordServer.GetInstance().EndRecord("HellEnd");
                                }
                                BeUtility.ResetCamera();
                                if (NewbieGuideManager.instance != null)
                                    NewbieGuideManager.instance.CleanWeakGuide();
                                if (ClientSystemManager.instance != null && ClientSystemManager.instance.delayCaller != null)
                                {
                                    ClientSystemManager.instance.delayCaller.DelayCall(10,()=>{ ClientSystemManager.instance.SwitchSystem<ClientSystemTown>(); });
                                }
#endif
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

        }

        private bool _dropHellWaveMonster(Battle.DungeonHellWaveInfo wave, VInt3 pos)
        {
            if (wave.monsters.Count > 0)
            {
                for (int i = 0; i < wave.monsters.Count; ++i)
                {
                    var fx = FrameRandom.InRange(-2 * IntMath.kIntDen, (2+1) * IntMath.kIntDen);
                    var fy = FrameRandom.InRange(-2 * IntMath.kIntDen, (2+1) * IntMath.kIntDen);

                    VInt3 npos = new VInt3(pos.x+fx, pos.y+fy, pos.z);

					npos = BeAIManager.FindStandPositionNew(npos, mDungeonManager.GetBeScene(), false,false,40);

                    var monster = mDungeonManager.GetBeScene().CreateRemoteMonster(wave.monsters[i], npos, VFactor.one);
                    if (monster != null)
                    {
                        if (monster.GetRecordServer().IsProcessRecord())
                        {
                            monster.GetRecordServer().RecordProcess("[CreateHellPos] create hell monster {0} {1} pos{2} newpos{3}", monster.GetName(), monster.m_iID, pos, npos);
                            monster.GetRecordServer().Mark(0x8789801, new int[]
                                                                      {
                                                                            monster.m_iID, pos.x,pos.y,pos.z, npos.x,npos.y,npos.z
                                                                      }, monster.GetName());
                        }
                        monster.StartAI(null);
                    }
					
                }

                return true;
            }

            return false;
        }

        private bool _setHellInfoState(Battle.DungeonHellInfo info, Battle.eDungeonHellState state)
        {
            if ((int)info.state <= (int)state)
            {
                Logger.LogProcessFormat("[hell] 改变状态 {0} => {1}", info.state, state);
                info.state = state;
                return true;
            }

            return false;
        }

        private DungeonHellGuideFrame _setHellTips(string text)
        {
#if !LOGIC_SERVER
            ClientSystemManager.instance.CloseFrame<DungeonHellGuideFrame>();
            mHellTipsFrame = ClientSystemManager.instance.OpenFrame<DungeonHellGuideFrame>(FrameLayer.Middle) as DungeonHellGuideFrame;
            mHellTipsFrame.SetDescription(text);

            return mHellTipsFrame;
#else
            return null;
#endif
        }

        private void _setHellTipsLeftCount(int count)
        {
#if !LOGIC_SERVER
            if (null != mHellTipsFrame)
            {
                mHellTipsFrame.SetLeftCount(count);
            }
#endif
        }

        private void _closeHellTips()
        {
#if !LOGIC_SERVER
            if (null != mHellTipsFrame)
            {
                ClientSystemManager.instance.CloseFrame(mHellTipsFrame);
            }
            mHellTipsFrame = null;
#endif
        }


#endregion

        /// <summary>
        /// 发送战斗结束消息
        /// </summary>
        /// <param name="isSuccess"></param>
        public void SendLimitDungeonFightEnd(bool isSuccess)
        {
#if !LOGIC_SERVER
            if (isSuccess)
            {
                _sendDungeonKillMonsterReq();
                _sendDungeonRewardReq();
                _sendDungeonBossRewardReq();
                _sendDungeonRaceEndReq();
            }
            else
            {
                _sendDungeonRaceEndFail();
            }
#else
                var req = _getDungeonRaceEndTeamReq();
                LogicServer.ReportRaceEndToLogicServer(req);
#endif
            mDungeonManager.FinishFight();
        }
    }
}
