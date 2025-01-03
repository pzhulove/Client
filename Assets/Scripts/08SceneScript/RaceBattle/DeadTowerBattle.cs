using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;

using Network;
using Protocol;
using DG.Tweening;

/// <summary>
/// Battle类
/// </summary>
namespace GameClient
{
    /// <summary>
    /// 
    /// </summary>
    public class ActivityBattle : PVEBattle
    {
        protected IDungeonFinish mFinishFrame;

        public ActivityBattle(BattleType type, eDungeonMode mode, int id) : base (type, mode, id)
        {

        }
       
        protected override void _onAreaClear(BeEvent.BeEventParam args)
        {

        }

        protected override void _onPlayerCancelReborn(BattlePlayer player)
        {
#if !LOGIC_SERVER
            ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
#endif
        }

        protected override void _onPostStart()
        {
#if !LOGIC_SERVER
		    if (ItemDataManager.GetInstance().IsPackageFull())
            {
                SystemNotifyManager.SystemNotify(1033);
            }  
#endif
        }

        protected void _openFinishFrame<T>() where T : ClientFrame
        {
#if !LOGIC_SERVER
            T frame = ClientSystemManager.instance.OpenFrame<T>() as T;
            if (null != frame)
            {
                mFinishFrame = frame as IDungeonFinish;
            }
#endif
        }

        private void _setName()
        {
#if !LOGIC_SERVER
            if (null != mFinishFrame)
            {
                mFinishFrame.SetName(mDungeonManager.GetDungeonDataManager().table.Name);
            }
#endif
        }

        protected void _setFinish(bool finish)
        {
#if !LOGIC_SERVER
            _setName();

            if (null != mFinishFrame)
            {
                mFinishFrame.SetFinish(finish);
            }
#endif
        }

        protected void _setDrop(ComItemList.Items[] items)
        {
#if !LOGIC_SERVER
            if (null != mFinishFrame)
            {
                mFinishFrame.SetDrops(items);
            }
#endif
        }

        protected void _setExp(ulong exp)
        {
#if !LOGIC_SERVER
            if (null != mFinishFrame)
            {
                mFinishFrame.SetExp(exp);
            }
#endif
        }

        protected ComItemList.Items[] _convertComItemList(SceneDungeonDropItem[] items)
        {
            List<ComItemList.Items> list = new List<ComItemList.Items>();
#if !LOGIC_SERVER
            for (int i = 0; i < items.Length; ++i)
            {
                list.Add(new ComItemList.Items()
                {
                    count = items[i].num,
                    id = (int)items[i].typeId,
                    type = ComItemList.eItemType.Custom,
                    equipType = (EEquipType)items[i].equipType
                });
            }
#endif
            return list.ToArray();

        }

        protected IEnumerator _endDropIter(int multi, GameClient.DungeonDeadTowerFinishFrame frame, bool isNorth)
        {
            if (!_isNeedSendNet())
            {
                frame.SetComItemList(null);
                yield break;
            }

            var msg = new MessageEvents();
            var req = new SceneDungeonEndDropReq();
            var res = new SceneDungeonEndDropRes();

            req.multi = (byte)multi;

            if (!isNorth)
            {
                req = null;
            }

            yield return (_sendMsgWithResend<SceneDungeonEndDropReq, SceneDungeonEndDropRes>(ServerType.GATE_SERVER, msg, req, res, true, 10));

            if (msg.IsAllMessageReceived())
            {
                if (res.multi == 0)
                {
                    Logger.LogError("fail to get the resutle drop item");

                    _setDrop(new ComItemList.Items[0]);
                    frame.SetComItemList(null);
                    yield break;
                }

                frame.SetComItemList(_convertComItemList(res.items));
            }
        }


    }

    public class DeadTowerBattle : ActivityBattle
    {
        private const int kRaceEndSplit = 5;

        private int currentIndex = 0;
        private List<int> mDialogIDArray = new List<int>();
        private List<int> mDialogIDAfterArray = new List<int>();

        private bool mHasSendRaceEnd = false;

        public DeadTowerBattle(BattleType type, eDungeonMode mode, int id) : base(type, mode, id)
        {
            mHasSendRaceEnd = false;
        }


        private void _sendRaceEnd()
        {
            if (_isNeedSendNet() && !mHasSendRaceEnd)
            {
                mHasSendRaceEnd = true;
                var req = _getDungeonRaceEndReqWithCount(kRaceEndSplit);
                NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
                Logger.LogProcessFormat("[战斗消息] 发送结算消息 {0}", ObjectDumper.Dump(req));
            }
        }

#region 重写虚函数们儿

		void SetFloorName()
		{
			var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIDeadTower>();
			if (battleUI != null)
                battleUI.SetFloor(currentIndex+1);
		}

		void SetPlayer(bool recover = false)
		{
            List<BattlePlayer> allPlayers = mDungeonPlayers.GetAllPlayers();

            //过层回血
            for (int i = 0; i < allPlayers.Count; ++i)
            {
                BattlePlayer player = allPlayers[i];
                if (null == player || null == player.playerActor)
                {
                    continue;
                }

                BeActor actor = player.playerActor;

                if (recover)
                {
                    actor.attribute.PostInit();
                    actor.m_pkGeActor.ResetHPBar();
                    actor.ResetSkillCoolDown();
                }

                actor.buffController.RemoveDebuff();
            }
        }
			
        protected override void _onCreateScene(BeEvent.BeEventParam args)
        {
			SetFloorName();
			SetPlayer();
        }

        private void _showCurrentTips()
        {
#if !LOGIC_SERVER
            SystemNotifyManager.SysDungeonAnimation(
                string.Format("{0} <color=#ffca14>{1}</color> 层", mDungeonManager.GetDungeonDataManager().table.Name, currentIndex+1
                )
            );
#endif
            
        }

        protected override void _onPostStart()
        {
            base._onPostStart();

#if !LOGIC_SERVER
            _showCurrentTips();
            
			SetFloorName();

			var dialog = TableManager.instance.GetTableItem<ProtoTable.TalkTable>(mDialogIDArray[currentIndex]);
			if (dialog != null)
			{
				ClientSystemManager.GetInstance().delayCaller.DelayCall(100, ()=>{
				//	Logger.LogErrorFormat("open dialog time:{0}", Time.realtimeSinceStartup);
					if (GetMode() != eDungeonMode.SyncFrame)
					{
						mDungeonManager.PauseFight(false);

						AddDialog(mDialogIDArray[currentIndex], () =>
							{
								mDungeonManager.ResumeFight(false);
								//mDungeonManager.StartFight();
							});
					}
				});
				

			}
#endif
        }

        private bool mIsProcessAreaClear = false;

        protected override void _onAreaClear(BeEvent.BeEventParam args)
        {
#if !LOGIC_SERVER
            if (!mIsProcessAreaClear)
            {
                mIsProcessAreaClear = true;
                GameFrameWork.instance.StartCoroutine(_processAreaClear());
            }
#else
#endif
        }

        private IEnumerator _processAreaClear()
        {
            yield return _sendDungeonReportDataIter();

            mIsProcessAreaClear = false;
            if (mDungeonManager != null && mDungeonPlayers != null)
            {
                var mCurBeScene = mDungeonManager.GetBeScene();
                var mainPlayer = mDungeonPlayers.GetMainPlayer();

                if (mCurBeScene != null && mainPlayer!=null)
                {
                    mCurBeScene.ForcePickUpDropItem(mainPlayer.playerActor);
                }
                //if (!_isNeedCalculateDrop())
                //{
                //    //mainPlayer.playerActor.TriggerEvent(BeEventType.onDeadTowerEnterNextLayer, null);                    //死亡之塔进入下一层的事件监听
                //}

                _sendDungeonClearAreaMonstersReq();

                if (mDungeonManager.GetDungeonDataManager().IsBossArea())
                {
#if !LOGIC_SERVER
                    GameFrameWork.instance.StartCoroutine(_openDeadTowerFinishFrame());
                    _fireSceneChangeAreaCmd();
#endif
                }
                else
                {
#if !LOGIC_SERVER
                    if (_isNeedCalculateDrop())
                    {
                        GameFrameWork.instance.StartCoroutine(_openDeadTowerFinishFrame());
                    }
                    else
                    {
                        //隐藏暂停按钮
                        var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPauseBtn>();
                        if (battleUI != null)
                        {
                            battleUI.HidePauseButton(true);
                        }
                        SystemNotifyManager.SysDungeonAnimation2("怪物已清除,即将进入下一层.");
                        PlaySound(5);

                        ClientSystemManager.GetInstance().delayCaller.DelayCall(2000, () =>
                        {
                            var dialog = TableManager.instance.GetTableItem<ProtoTable.TalkTable>(mDialogIDAfterArray[currentIndex]);
                            if (dialog != null)
                            {
                                // 打开对话
                                AddDialog(mDialogIDAfterArray[currentIndex], () =>
                                    {
                                        _fireSceneChangeAreaCmd();
                                    });
                            }
                            else
                            {
                                _fireSceneChangeAreaCmd();
                            }
                        });
                    }
#endif
                }
            }
        }

        private void _fireSceneChangeAreaCmd()
        {
#if !LOGIC_SERVER
            SceneChangeArea changeArea = new SceneChangeArea();
            FrameSync.instance.FireFrameCommand(changeArea);
#endif  
        }

        protected override void _onSceneAreaChange()
        {
            if (mDungeonManager.GetDungeonDataManager().IsBossArea())
            {
                mDungeonManager.FinishFight();
                _resetPlayerActor(false);
            }
            else
            {
                if (_isNeedCalculateDrop())
                {
                    //五层塔闯关成功
                    if(mDungeonManager.GetBeScene() != null)
                        mDungeonManager.GetBeScene().TriggerEventNew(BeEventSceneType.onDeadTowerPassFiveLayer);
                    SetPlayer(true);
                }
                else
                {
                    var allPlayers = mDungeonPlayers.GetAllPlayers();
                    //过层回血
                    for (int i = 0; i < allPlayers.Count; ++i)
                    {
                        if (null == allPlayers[i].playerActor)
                        {
                            continue;
                        }

						allPlayers[i].playerActor.buffController.TryAddBuff(35, 2000);
                        allPlayers[i].playerActor.TriggerEventNew(BeEventType.onDeadTowerEnterNextLayer);                    //死亡之塔进入下一层的事件监听
                    }
                }

                _changeAreaByIdx();
            }
        }

        protected override void _createPlayers()
        {
            base._createPlayers();

            var dungeonData  = mDungeonManager.GetDungeonDataManager();
            var players      = mDungeonPlayers.GetAllPlayers();
            for (int i = 0; i < players.Count; ++i)
            {
                players[i].playerActor.SetPosition(dungeonData.CurrentBirthPosition());
            }

			mDungeonManager.GetBeScene().InitFriendActor(dungeonData.CurrentBirthPosition());
        }

        protected override void _onPlayerDead(BattlePlayer player)
        {
#if !LOGIC_SERVER
		    var mainPlayer = mDungeonPlayers.GetMainPlayer();
            if (player == mainPlayer)
            {
                GameFrameWork.instance.StartCoroutine(_onDeadProcess());
            }  
#endif
        }

        private IEnumerator _onDeadProcess()
        {
            yield return _fireRaceEndOnLocalFrameIter();

            yield return _sendDungeonReportDataIter();

            _sendRaceEnd();

            // TODO 修改这里的流程！！！
            var frame = ClientSystemManager.instance.OpenFrame<DungeonDeadTowerFailFrame>() as IDungeonFinish;
            if (null != frame)
            {
                frame.SetLevel(currentIndex + 1);
            }
        }


        protected override void _onStart()
        {
            //_hiddenDungeonMap(true);
            currentIndex = mDungeonManager.GetDungeonDataManager().CurrentIndex();
            _setDialogArray();
            startTime = GetTimeStamp();
        }

        //protected override void _onEnd()
        //{
        //    _sendRaceEnd();
        //}

        protected override void _createDoors()
        {
            // keep empty
        }

        protected override void _onDoorStateChange(BeEvent.BeEventParam args)
        {
            // keep empty
        }


		protected override void PreparePreloadRes ()
		{
			PreloadManager.ClearCache();
            
			var dungeonDataManager = mDungeonManager.GetDungeonDataManager();
			if (null != dungeonDataManager)
			{
				CResPreloader.instance.AddRes(dungeonDataManager.PreloadPath(), false, 1, dungeonDataManager.Preload);
			}
            
			var players = mDungeonPlayers.GetAllPlayers();
            BeActionFrameMgr frameMgr = null;
            SkillFileListCache fileCache = null;
            if (mDungeonManager != null && mDungeonManager.GetBeScene() != null)
            {
                frameMgr = mDungeonManager.GetBeScene().ActionFrameMgr;
                fileCache = mDungeonManager.GetBeScene().SkillFileCache;
            }
            
            for (int i = 0; i < players.Count; ++i)
			{
				var actor = players[i].playerActor;
				if (actor != null)
				{
					GameClient.PreloadManager.PreloadActor(actor, frameMgr, fileCache);
				}
			}
            
			PreloadEnemies();
		}

#endregion

#region 私

        private bool _isNeedCalculateDrop()
        {
            return (currentIndex + 1) % kRaceEndSplit == 0;
        }

		private void ClearResource()
		{
#if !LOGIC_SERVER
            AssetGabageCollector.instance.ClearUnusedAsset(CResPreloader.instance.priorityGameObjectKeys);  
#endif
        }

        private void _changeAreaByIdx()
        {
            _changeAreaFade(800, 600, ()=>
            {
                if (mDungeonManager.GetDungeonDataManager().NextAreaByIndex(currentIndex + 1))
                {
					if (_isNeedCalculateDrop())
					{
						ClearResource();
					}

                    currentIndex++;

                    _createBase();
                    _createEntitys();
#if !LOGIC_SERVER
					PreloadMonster();
#endif
                    _onSceneStart();
                    mDungeonManager.StartFight();
                    
                    _sendSceneDungeonEnterNextAreaReq(mDungeonManager.GetDungeonDataManager().CurrentAreaID());
                    _sendDungeonRewardReq();
                }
            },
            ()=>{
                _showCurrentTips();
#if !LOGIC_SERVER
                //隐藏暂停按钮
                var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPauseBtn>();
                if (battleUI != null)
                {
                    battleUI.HidePauseButton(false);
                }
#endif
                //重新显示暂停按钮
            });
        }
    
        private void AddDialog(int id, UnityAction action)
        {
            var task = new GameClient.TaskDialogFrame.OnDialogOver();
            if (action != null)
            {
                task.AddListener(action);
            }
            GameClient.MissionManager.GetInstance().CreateDialogFrame(id, 0, task);
        }

        private void _setDialogArray(IList<string> area, List<int> array)
        {
            List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
            for (int i = 0; i < area.Count; ++i)
            {
                var kv = area[i].Split(':');

                if (kv.Length >= 2)
                {
                    int key = 0;
                    int value = 0;

                    if (int.TryParse(kv[0], out key) && int.TryParse(kv[1], out value))
                    {
                        list.Add(new KeyValuePair<int, int>(key, value));
                    }
                }
            }

            list.Sort((x, y) =>
                    {
                    return x.Key - y.Key;
                    });

            int j = 0;

            for (int i = 0; i < mDungeonManager.GetDungeonDataManager().asset.GetAreaConnectListLength(); ++i)
            {
                if (j < list.Count && i == list[j].Key)
                {
                    array.Add(list[j].Value);
                    j++;
                }
                else
                {
                    array.Add(0);
                }
            }
        }

        private void _setDialogArray()
        {
            var mDungeonConfigTable = mDungeonManager.GetDungeonDataManager().configTable;
            if (null != mDungeonConfigTable)
            {
                _setDialogArray(mDungeonConfigTable.AreaDialog, mDialogIDArray);
                _setDialogArray(mDungeonConfigTable.AreaAfterDialog, mDialogIDAfterArray);
            }
        }

        private void _sendDungeonClearAreaMonstersReq()
        {
            if (_isNeedSendNet())
            {
                var mainPlayer = mDungeonPlayers.GetMainPlayer().playerActor;

                var msg = new SceneDungeonClearAreaMonsters();
				msg.remainHp = (uint)mainPlayer.attribute.GetHP();
				msg.remainMp = (uint)mainPlayer.attribute.GetMP();
                // TODO 时间是从对话开始，清怪之后结束
                msg.usedTime = (uint)mDungeonStatistics.CurrentFightTime(false);
                msg.md5 = DungeonUtility.GetBattleEncryptMD5(string.Format("{0}{1}{2}", msg.usedTime, msg.remainHp, msg.remainMp));
                msg.lastFrame = mDungeonManager.GetDungeonDataManager().GetFinalFrameDataIndex();

                NetManager.instance.SendCommand(ServerType.GATE_SERVER, msg);

                Logger.LogProcessFormat("[战斗] 怪物清除消息 : {0}", ObjectDumper.Dump(msg));
            }
        }

        private IEnumerator _openDeadTowerFinishFrame()
        {
            _sendDungeonKillMonsterReq();

            ClientSystemManager.instance.OpenFrame<DungeonDeadTowerFinishFrame>();

            while (!ClientSystemManager.instance.IsFrameOpen<DungeonDeadTowerFinishFrame>())
            {
                yield return null;
            }

            var frame = ClientSystemManager.instance.GetFrame(typeof(DungeonDeadTowerFinishFrame)) as DungeonDeadTowerFinishFrame;

            if (frame == null)
            {
                yield break;
            }

            bool isBossArea = mDungeonManager.GetDungeonDataManager().IsBossArea();
            int time = mDungeonStatistics.LastFightTimeWithCount(false, kRaceEndSplit);

            int currentTop = CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_TOWER_TOP_FLOOR_TOTAL);
            int bestTime = CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_TOWER_USED_TIME_TOTAL);

            if (currentTop <= (currentIndex + 1))
            {
                bestTime = Mathf.Min(bestTime, time);
                frame.SetBestTime(bestTime);
                frame.SetIsNewRecord(time == bestTime);
            }
            else 
            {
                frame.SetIsNewRecord(false);
                frame.SetBestTime(-1);
            }

            frame.SetLevel(currentIndex + 1);
            frame.SetCurrentTime(time);
            frame.SetFinish(isBossArea);

            yield return (_endDropIter(1, frame, false));

            BattleDataManager.GetInstance().BattleInfo.dropCacheItemIds.Clear();

            bool isSend = false;
            if (isBossArea)
            {
                isSend = true;
                _sendRaceEnd();
                _sendDungeonRewardReq();
            }

            while (frame.state == DungeonDeadTowerFinishFrame.eFinishState.None)
            {
                yield return Yielders.EndOfFrame;
            }

            if (frame.state == DungeonDeadTowerFinishFrame.eFinishState.Continue)
            {
                if (!isBossArea)
                {
                    // TODO change the area with the id
                    _fireSceneChangeAreaCmd();
                }
            }
            else if (frame.state == DungeonDeadTowerFinishFrame.eFinishState.Back)
            {
                yield return _fireRaceEndOnLocalFrameIter();

                yield return _sendDungeonReportDataIter();

                if (!isSend)
                {
                    _sendRaceEnd();
                    _sendDungeonRewardReq();

                    yield return Yielders.EndOfFrame;
                }

                ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
            }
        }
#endregion
    }
}
