using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using System;

namespace GameClient
{
    class MoneyRewardsResultFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/MoneyRewards/MoneyRewardsResultFrame";
        }

        [UIControl("Time", typeof(ComTimeTick))]
        ComTimeTick comTime;
        [UIControl("Time/UINumber0", typeof(Text))]
        Text minutes;
        [UIControl("Time/UINumber1", typeof(Text))]
        Text seconds;
        [UIControl("", typeof(ComMoneyRewardsInfoList))]
        ComMoneyRewardsInfoList comRewardsInfoList;
        [UIControl("", typeof(StateController))]
        StateController comState;
        [UIControl("ChampAward", typeof(Text))]
        Text champAwards;
        string mNormal = "normal";
        string mUnStart = "unstart";
        string mDisable = "disable";
        string mRunning = "running";

        public static void CommandOpen(object argv)
        {
            if (!ClientSystemManager.GetInstance().IsFrameOpen<MoneyRewardsResultFrame>())
            {
                ClientSystemManager.GetInstance().OpenFrame<MoneyRewardsResultFrame>();
            }
        }
        
        protected sealed override void _OnOpenFrame()
        {
            _AddButton("Close", () => { frameMgr.CloseFrame(this); });

            if(null != comTime)
            {
                comTime.onTick += _OnTick;
            }

            _RegisterEvent();
            _UpdateTimeTick();
            _UpdateStage();
            _UpdateInfoList();
            _UpdateWatchInfos();
            _UpdateChampAwardsText();
        }

        void _RegisterEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsResultChanged, _OnMoneyRewardsResultChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsStatusChanged, _OnOnMoneyRewardsStatusChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsBattleInfoChanged, _OnMoneyRewardsBattleInfoChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsRcdStatusChanged, _OnMoneyRewardsRcdStatusChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsAwardsChanged, _OnMoneyRewardsAwardsChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsAwardListChanged, _OnMoneyRewardsAwardListChanged);
        }

        void _OnMoneyRewardsResultChanged(UIEvent uiEvent)
        {
            _UpdateInfoList();
        }

        void _OnOnMoneyRewardsStatusChanged(UIEvent uiEvent)
        {
            _UpdateStage();
            _UpdateTimeTick();
        }

        void _OnMoneyRewardsBattleInfoChanged(UIEvent uiEvent)
        {
            _UpdateWatchInfos();
        }

        void _OnMoneyRewardsRcdStatusChanged(UIEvent uiEvent)
        {
            _UpdateWatchInfos();
        }

        void _OnMoneyRewardsAwardsChanged(UIEvent uiEvent)
        {
            _UpdateChampAwardsText();
        }

        void _OnMoneyRewardsAwardListChanged(UIEvent uiEvent)
        {
            _UpdateChampAwardsText();
        }

        void _UnRegisterEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsResultChanged, _OnMoneyRewardsResultChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsStatusChanged, _OnOnMoneyRewardsStatusChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsBattleInfoChanged, _OnMoneyRewardsBattleInfoChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsRcdStatusChanged, _OnMoneyRewardsRcdStatusChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsAwardsChanged, _OnMoneyRewardsAwardsChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsAwardListChanged, _OnMoneyRewardsAwardListChanged);
        }

        void _UpdateStage()
        {
            if (null != comState)
            {
                switch (MoneyRewardsDataManager.GetInstance().eMoneyRewardsStatus)
                {
                    case MoneyRewardsStatus.MRS_INVALID:
                        {
                            comState.Key = "finish";
                        }
                        break;
                    case MoneyRewardsStatus.MRS_READY:
                        {
                            comState.Key = "ready";
                        }
                        break;
                    case MoneyRewardsStatus.MRS_8_RACE:
                        {
                            comState.Key = "8level";
                        }
                        break;
                    case MoneyRewardsStatus.MRS_PRE_4_RACE:
                        {
                            comState.Key = "4pre_level";
                        }
                        break;
                    case MoneyRewardsStatus.MRS_4_RACE:
                        {
                            comState.Key = "4level";
                        }
                        break;
                    case MoneyRewardsStatus.MRS_2_RACE:
                        {
                            comState.Key = "2level";
                        }
                        break;
                    case MoneyRewardsStatus.MRS_RACE:
                        {
                            comState.Key = "1level";
                        }
                        break;
                    case MoneyRewardsStatus.MRS_END:
                        {
                            comState.Key = "finish";
                        }
                        break;
                }
            }
        }

        [UIEventHandle("Go")]
        void _OnClickGoMail()
        {
            ClientSystemManager.instance.OpenFrame<MailNewFrame>(FrameLayer.Middle);
        }

        void _UpdateTimeTick()
        {
            if (null != comTime)
            {
                comTime.SetEndTime(MoneyRewardsDataManager.GetInstance().StatusEndTime);
            }
        }

        void PlayReplay(UInt64 a_raceID, bool normal = true)
        {
            string sessionID = a_raceID.ToString();

            Logger.LogProcessFormat("<color=#00ff00>play action[{0}] [start]</color>", a_raceID);

            if (TeamDataManager.GetInstance().HasTeam())
            {
                ShowNotify(ReplayErrorCode.HAS_TEAM);
                return;
            }

            if (ReplayServer.GetInstance().HasReplay(sessionID))
            {
                Logger.LogProcessFormat("<color=#00ff00>play action[{0}] [find rcd has existed !]</color>", a_raceID);
                var ret = ReplayServer.GetInstance().StartReplay(sessionID, ReplayPlayFrom.MONEY_REWARD);
                if (ret == ReplayErrorCode.SUCCEED)
                {
                    frameMgr.CloseFrame(this);
                    Logger.LogProcessFormat("<color=#00ff00>play action[{0}] [start play rcd succeed !]</color>", a_raceID);
                    SeasonDataManager.GetInstance().NotifyVideoPlayed(a_raceID);
                }
                else
                {
                    Logger.LogProcessFormat("<color=#ff0000>play action[{0}] [start play rcd failed !]</color>", a_raceID);
                    ReplayServer.GetInstance().Clear();
                }
                ShowNotify(ret);
            }
            else
            {
                Logger.LogProcessFormat("<color=#00ff00>play action[{0}] [find rcd has not existed yet!]</color>", a_raceID);

                if (normal)
                    ShowNotify(ReplayErrorCode.FILE_NOT_FOUND);
                else
                {
                    Logger.LogProcessFormat("<color=#00ff00>play action[{0}] [start download rcd !!]</color>", a_raceID);
                    StartDownloadReplayFile(a_raceID);
                }
            }
        }

        void ShowNotify(ReplayErrorCode code)
        {
            MoneyRewardsDataManager.ShowErrorNotify(code);
        }

        #region DOWNLOAD
        private UnityEngine.Coroutine mCurrentLoadServerCo = null;
        void StartDownloadReplayFile(UInt64 a_raceID)
        {
            string sessionID = a_raceID.ToString();

            if(!MoneyRewardsDataManager.GetInstance().isRcdInQueue(a_raceID))
            {
                MoneyRewardsDataManager.GetInstance().downloadRcd(a_raceID,(UInt64 raceId) =>
                {
                    string curSessionID = raceId.ToString();
                    Logger.LogProcessFormat("<color=#00ff00>play action[{0}] [download rcd succeed ! then start play!]</color>", a_raceID);
                    var ret = ReplayServer.GetInstance().StartReplay(curSessionID, ReplayPlayFrom.MONEY_REWARD);
                    if (ret == ReplayErrorCode.SUCCEED)
                    {
                        frameMgr.CloseFrame(this);

                        Logger.LogProcessFormat("<color=#00ff00>play action[{0}] [download rcd succeed ! play rcd succeed !]</color>", a_raceID);
                        SeasonDataManager.GetInstance().NotifyVideoPlayed(a_raceID);
                    }
                    else
                    {
                        Logger.LogProcessFormat("<color=#ff0000>play action[{0}] [download rcd succeed ! but play rcd failed !]</color>", a_raceID);
                        ReplayServer.GetInstance().Clear();
                    }
                    ShowNotify(ret);
                });
            }
        }
        #endregion
        void _OpenObserverRecord(CLPremiumLeagueBattle battle)
        {
            if (null == battle || null == battle.fighter1 || null == battle.fighter2)
            {
                return;
            }
            if (TeamDataManager.GetInstance().HasTeam())
            {
                ShowNotify(ReplayErrorCode.HAS_TEAM);
                return;
            }
            int iIndex = MoneyRewardsDataManager.GetInstance().getIndexByRoleId(PlayerBaseData.GetInstance().RoleID);
            bool bHasNext = false;
            MoneyRewardsDataManager.GetInstance().GetNextVsData(iIndex, MoneyRewardsDataManager.GetInstance().Status, ref bHasNext);
            if (bHasNext)
            {
                SystemNotifyManager.SystemNotify(9019);
                return;
            }
            if (Network.NetManager.instance.ConnectToRelayServer(battle.relayAddr.ip, battle.relayAddr.port, ClientApplication.playerinfo.accid, 4000))
            {
                if (Network.NetManager.instance != null)
                {
                    RelaySvrObserveReq req = new RelaySvrObserveReq
                    {
                        accid = ClientApplication.playerinfo.accid,
                        roleId = PlayerBaseData.GetInstance().RoleID,
                        raceId = battle.raceId,
                        startFrame = 0
                    };
                    Network.NetManager.instance.SendCommand<RelaySvrObserveReq>(Network.ServerType.RELAY_SERVER, req);
                    if (ReplayServer.GetInstance() != null)
                    {
                        ReplayServer.GetInstance().LiveShowServerAddr = battle.relayAddr;
                    }
                }
            }

            if (frameMgr != null)
                frameMgr.CloseFrame(this);

        }
        void _WatchRecord(CLPremiumLeagueBattle battle)
        {
            if(null != battle && null != battle.fighter1 && null != battle.fighter2)
            {
                int iIndex = MoneyRewardsDataManager.GetInstance().getIndexByRoleId(PlayerBaseData.GetInstance().RoleID);
                bool bHasNext = false;
                MoneyRewardsDataManager.GetInstance().GetNextVsData(iIndex, MoneyRewardsDataManager.GetInstance().Status, ref bHasNext);
                if (bHasNext)
                {
                    SystemNotifyManager.SystemNotify(9019);
                    return;
                }
                    
                Logger.LogProcessFormat("watch record raceid = {0} p1={1} p2={2}", battle.raceId, battle.fighter1.name, battle.fighter2.name);
                PlayReplay(battle.raceId,false);
            }
            else
            {
                Logger.LogProcessFormat("watch record error , value is not invalid !!!");
            }
        }

        public enum WatchBattleErrorCode
        {
            WBEC_OK = 0,
            WBEC_NO_RECORD = 1,
            WBEC_IN_FIGHTER,
            WBEC_UN_START,
        }

        void _PopWatchBattleErrMsg(WatchBattleErrorCode code)
        {
            if (code == WatchBattleErrorCode.WBEC_NO_RECORD)
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("money_rewards_watch_battle_error_no_record"));
            else if (code == WatchBattleErrorCode.WBEC_IN_FIGHTER)
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("money_rewards_watch_battle_error_in_fighter"));
            else if (code == WatchBattleErrorCode.WBEC_UN_START)
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("money_rewards_watch_battle_error_un_start"));
        }

        void _UpdateInfoList()
        {
            if(null != comRewardsInfoList)
            {
                for (int i = 0; i < comRewardsInfoList.results.Length; ++i)
                {
                    var result = comRewardsInfoList.results[i];
                    if(null != result)
                    {
                        if(i >= 0 && i < MoneyRewardsDataManager.GetInstance().ResultDatas.Length)
                        {
                            result.SetValue(MoneyRewardsDataManager.GetInstance().ResultDatas[i]);
                        }
                    }
                }

                //set champion
                if(comRewardsInfoList.results.Length == 9)
                {
                    ComMoneyRewardsResultInfo champion = comRewardsInfoList.results[comRewardsInfoList.results.Length - 1];
                    if(null != champion)
                    {
                        champion.SetValue(MoneyRewardsDataManager.GetInstance().getChampionData);
                    }
                }

                int[] watchMaps = new int[8] { 0, 7, 3, 4, 2, 5, 1, 6 };

                //set 4
                for (int i = 0; i < comRewardsInfoList.result4s.Length; ++i)
                {
                    var result = comRewardsInfoList.result4s[i];
                    if (null != result)
                    {
                        int idx0 = watchMaps[2 * i];
                        int idx1 = watchMaps[2 * i + 1];
                        var left = MoneyRewardsDataManager.GetInstance().ResultDatas[idx0];
                        var right = MoneyRewardsDataManager.GetInstance().ResultDatas[idx1];
                        ComMoneyRewardsResultData curValue = null;
                        int lWinTimes = (null == left) ? 0 : left.winTimes;
                        int rWinTimes = (null == right) ? 0 : right.winTimes;
                        if(lWinTimes > 0)
                        {
                            curValue = left;
                        }
                        else if(rWinTimes > 0)
                        {
                            curValue = right;
                        }
                        else
                        {
                            curValue = null;
                        }
                        result.SetValue(curValue);
                    }
                }
                //set 2
                for (int i = 0; i < comRewardsInfoList.result2s.Length; ++i)
                {
                    var result = comRewardsInfoList.result2s[i];
                    if (null != result)
                    {
                        ComMoneyRewardsResultData curValue = null;
                        for (int j = 0; j < watchMaps.Length / 2; ++j)
                        {
                            int idx = 4 * i + j;
                            var res = MoneyRewardsDataManager.GetInstance().ResultDatas[watchMaps[idx]];
                            if(null != res && res.winTimes >= 2)
                            {
                                curValue = res;
                            }
                        }
                        result.SetValue(curValue);
                    }
                }
            }
        }

        void _UpdateChampAwardsText()
        {
            if(null != champAwards)
            {
                int iValue = MoneyRewardsDataManager.GetInstance().ChampAward;
                champAwards.text = TR.Value("money_rewards_champ_awards_text", iValue);
            }
        }

        void _UpdateWatchInfos()
        {
            if(true)
            {
                //set watch 4
                int[] watchMaps = new int[8] { 0, 7, 3, 4, 2, 5, 1, 6 };
                for (int i = 0; i < watchMaps.Length / 2; ++i)
                {
                    if (i < comRewardsInfoList.buttons.Length)
                    {
                        int idx0 = watchMaps[2 * i];
                        int idx1 = watchMaps[2 * i + 1];
                        Button btnWatch = comRewardsInfoList.buttons[i];
                        if (null != btnWatch)
                        {
                            btnWatch.onClick.RemoveAllListeners();
                            btnWatch.CustomActive(true);
                        }

                        StateController comState = comRewardsInfoList.btnWatchStatus[i];
                        CLPremiumLeagueBattle battleInfo = null;
                        var left = MoneyRewardsDataManager.GetInstance().ResultDatas[idx0];
                        var right = MoneyRewardsDataManager.GetInstance().ResultDatas[idx1];

                        if (null != left && null != right)
                        {
                            battleInfo = MoneyRewardsDataManager.GetInstance().GetRelationBattleInfo(left.recordId, PremiumLeagueStatus.PLS_FINAL_EIGHT);
                        }
                        
                        if (null != comState)
                        {
                            switch (MoneyRewardsDataManager.GetInstance().Status)
                            {
                                case PremiumLeagueStatus.PLS_INIT:
                                case PremiumLeagueStatus.PLS_ENROLL:
                                case PremiumLeagueStatus.PLS_PRELIMINAY:
                                case PremiumLeagueStatus.PLS_FINAL_EIGHT_PREPARE:
                                    {
                                        comState.Key = mUnStart;
                                        if (null != btnWatch)
                                        {
                                            btnWatch.onClick.AddListener(() =>
                                            {
                                                _PopWatchBattleErrMsg(WatchBattleErrorCode.WBEC_UN_START);
                                            });
                                        }
                                    }
                                    break;
                                case PremiumLeagueStatus.PLS_FINAL_EIGHT:
                                case PremiumLeagueStatus.PLS_FINAL_FOUR:
                                case PremiumLeagueStatus.PLS_FINAL:
                                case PremiumLeagueStatus.PLS_FINAL_WAIT_CLEAR:
                                    {
                                        if (null == battleInfo)
                                        {
                                            comState.Key = mDisable;
                                            if (null != btnWatch)
                                            {
                                                btnWatch.onClick.AddListener(() =>
                                                {
                                                    _PopWatchBattleErrMsg(WatchBattleErrorCode.WBEC_NO_RECORD);
                                                });
                                            }
                                        }
                                        else if (battleInfo.isEnd != 0)
                                        {
                                            comState.Key = mNormal;
                                            if (null != btnWatch)
                                            {
                                                btnWatch.onClick.AddListener(() =>
                                                {
                                                    _WatchRecord(battleInfo);
                                                });
                                            }
                                        }
                                        else
                                        {
                                            comState.Key = mRunning;
                                            if (null != btnWatch)
                                            {
                                                btnWatch.onClick.AddListener(() =>
                                                {
                                                    _OpenObserverRecord(battleInfo);
                                                    //_PopWatchBattleErrMsg(WatchBattleErrorCode.WBEC_IN_FIGHTER);
                                                });
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

            if(true)
            {
                //set watch 2
                int[] watchMaps = new int[8] { 0, 7, 3, 4, 2, 5, 1, 6 };
                for (int i = 0; i < watchMaps.Length / 4; ++i)
                {
                    if (4 + i < comRewardsInfoList.buttons.Length)
                    {
                        Button btnWatch = comRewardsInfoList.buttons[4 + i];
                        if (null != btnWatch)
                        {
                            btnWatch.onClick.RemoveAllListeners();
                            btnWatch.CustomActive(true);
                        }
                        StateController comState = comRewardsInfoList.btnWatchStatus[4 + i];

                        CLPremiumLeagueBattle battleInfo = null;
                        for (int j = 0; j < 4; ++j)
                        {
                            int idx = watchMaps[4 * i + j];
                            var data = MoneyRewardsDataManager.GetInstance().ResultDatas[idx];
                            if(null == data)
                            {
                                continue;
                            }
                            battleInfo = MoneyRewardsDataManager.GetInstance().GetRelationBattleInfo(data.recordId, PremiumLeagueStatus.PLS_FINAL_FOUR);
                            if(null != battleInfo)
                            {
                                break;
                            }
                        }

                        if (null != comState)
                        {
                            switch (MoneyRewardsDataManager.GetInstance().Status)
                            {
                                case PremiumLeagueStatus.PLS_INIT:
                                case PremiumLeagueStatus.PLS_ENROLL:
                                case PremiumLeagueStatus.PLS_PRELIMINAY:
                                case PremiumLeagueStatus.PLS_FINAL_EIGHT_PREPARE:
                                case PremiumLeagueStatus.PLS_FINAL_EIGHT:
                                    {
                                        comState.Key = mUnStart;
                                        if(null != btnWatch)
                                        {
                                            btnWatch.onClick.AddListener(() =>
                                            {
                                                _PopWatchBattleErrMsg(WatchBattleErrorCode.WBEC_UN_START);
                                            });
                                        }
                                    }
                                    break;
                                case PremiumLeagueStatus.PLS_FINAL_FOUR:
                                case PremiumLeagueStatus.PLS_FINAL:
                                case PremiumLeagueStatus.PLS_FINAL_WAIT_CLEAR:
                                    {
                                        if (null == battleInfo)
                                        {
                                            comState.Key = mDisable;
                                            if (null != btnWatch)
                                            {
                                                btnWatch.onClick.AddListener(() =>
                                                {
                                                    _PopWatchBattleErrMsg(WatchBattleErrorCode.WBEC_NO_RECORD);
                                                });
                                            }
                                        }
                                        else if (battleInfo.isEnd != 0)
                                        {
                                            comState.Key = mNormal;
                                            if (null != btnWatch)
                                            {
                                                btnWatch.onClick.AddListener(() =>
                                                {
                                                    _WatchRecord(battleInfo);
                                                });
                                            }
                                        }
                                        else
                                        {
                                            comState.Key = mRunning;
                                            if (null != btnWatch)
                                            {
                                                btnWatch.onClick.AddListener(() =>
                                                {
                                                    _OpenObserverRecord(battleInfo);
                                                    // _PopWatchBattleErrMsg(WatchBattleErrorCode.WBEC_IN_FIGHTER);
                                                });
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

            //set champion watch
            if (comRewardsInfoList.buttons.Length > 0)
            {
                Button btnWatch = comRewardsInfoList.buttons[comRewardsInfoList.buttons.Length - 1];
                if (null != btnWatch)
                {
                    btnWatch.onClick.RemoveAllListeners();
                    btnWatch.CustomActive(true);
                }

                var battleInfo = MoneyRewardsDataManager.GetInstance().GetChampionRelationInfo();
                StateController comState = comRewardsInfoList.btnWatchStatus[comRewardsInfoList.buttons.Length - 1];

                if (null != comState)
                {
                    switch (MoneyRewardsDataManager.GetInstance().Status)
                    {
                        case PremiumLeagueStatus.PLS_INIT:
                        case PremiumLeagueStatus.PLS_ENROLL:
                        case PremiumLeagueStatus.PLS_PRELIMINAY:
                        case PremiumLeagueStatus.PLS_FINAL_EIGHT_PREPARE:
                        case PremiumLeagueStatus.PLS_FINAL_EIGHT:
                        case PremiumLeagueStatus.PLS_FINAL_FOUR:
                            {
                                comState.Key = mUnStart;
                                if (null != btnWatch)
                                {
                                    btnWatch.onClick.AddListener(() =>
                                    {
                                        _PopWatchBattleErrMsg(WatchBattleErrorCode.WBEC_UN_START);
                                    });
                                }
                            }
                            break;
                        case PremiumLeagueStatus.PLS_FINAL:
                        case PremiumLeagueStatus.PLS_FINAL_WAIT_CLEAR:
                            {
                                if (null == battleInfo)
                                {
                                    comState.Key = mDisable;
                                    if (null != btnWatch)
                                    {
                                        btnWatch.onClick.AddListener(() =>
                                        {
                                            _PopWatchBattleErrMsg(WatchBattleErrorCode.WBEC_NO_RECORD);
                                        });
                                    }
                                }
                                else if (battleInfo.isEnd != 0)
                                {
                                    comState.Key = mNormal;
                                    if (null != btnWatch)
                                    {
                                        btnWatch.onClick.AddListener(() =>
                                        {
                                            _WatchRecord(battleInfo);
                                        });
                                    }
                                }
                                else
                                {
                                    comState.Key = mRunning;
                                    if (null != btnWatch)
                                    {
                                        btnWatch.onClick.AddListener(() =>
                                        {
                                            _OpenObserverRecord(battleInfo);
                                            //  _PopWatchBattleErrMsg(WatchBattleErrorCode.WBEC_IN_FIGHTER);
                                        });
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        void _OnTick(uint time)
        {
            uint sec = time % 60;
            uint min = (time / 60) % 60;
            uint hour = (time / 3600) % 24;
            if (null != minutes)
            {
                minutes.text = string.Format("{0:D2}", min);
            }
            if(null != seconds)
            {
                seconds.text = string.Format("{0:D2}", sec);
            }
        }

        protected override void _OnCloseFrame()
        {
            if (null != comTime)
            {
                comTime.onTick -= _OnTick;
            }
            _UnRegisterEvent();
        }
    }
}