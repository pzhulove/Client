using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using Network;

namespace GameClient
{
    public class MoneyRewardsMainFrameData
    {
        public ProtoTable.CitySceneTable citySceneItem;
        public int CurrentSceneID = 0;
        public int TargetTownSceneID = 0;
    };
    public class MoneyRewardsMainFrame : ClientFrame
    {
        public static void CommandOpen(object argv = null)
        {
            MoneyRewardsMainFrameData data = argv as MoneyRewardsMainFrameData;
            if(null == data)
            {
                data = new MoneyRewardsMainFrameData();
            }

            MoneyRewardsRankFrame._RequestRanklist((int)SortListType.SORTLIST_PREMIUM_LEAGUE, 0, 20,false);

            if (!ClientSystemManager.GetInstance().IsFrameOpen<MoneyRewardsMainFrame>())
            {
                ClientSystemManager.GetInstance().OpenFrame<MoneyRewardsMainFrame>(FrameLayer.Bottom, argv);
            }
        }

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/MoneyRewards/MoneyRewardsMainFrame";
        }

        [UIControl("AwardsPanel/c0", typeof(Text))]
        Text award_0;
        [UIControl("AwardsPanel/c1", typeof(Text))]
        Text award_1;
        [UIControl("AwardsPanel/c2", typeof(Text))]
        Text award_2;
        [UIControl("AwardsPanel/c3", typeof(Text))]
        Text award_3;
        [UIControl("AwardsPanel/c4", typeof(Text))]
        Text award_4;
        Text[] awards = new Text[5];

        [UIControl("PlayerCount", typeof(Text))]
        Text playerCount;

        [UIControl("ScorePanel/MyRate/MyRate", typeof(Text))]
        Text MyRate;
        [UIControl("ScorePanel/MyScore/MyScore", typeof(Text))]
        Text MyScore;
        [UIControl("ScorePanel/MaxScore/MaxScore ", typeof(Text))]
        Text MaxScore;
        [UIControl("ScorePanel/MyRank/MyRank", typeof(Text))]
        Text MyRank;

        [UIControl("Status", typeof(StateController))]
        StateController comStatus;

        [UIControl("Status/Layout", typeof(TimeRefresh))]
        TimeRefresh timer;

        [UIObject("BtnWatchResult")]
        GameObject goWatch;

        [UIControl("BtnStartMatch", typeof(StateController))]
        StateController comMatchState;

        [UIControl("", typeof(ComMoneyRewardsDataBinder))]
        ComMoneyRewardsDataBinder comMoneyRewardsDataBinder;

        string mUnStart = "unstart";
        string mNormal = "normal";
        string mClosed = "closed";
        string mNoRank = "no_rank";
        string mOnceOver = "once_over";
        string mTwoOver = "two_over";

        void _OnOnMoneyRewardsStatusChanged(UIEvent uiEvent)
        {
            _UpdateStatus();
            _UpdateMatchButtonStatus();
            _UpdateBtnMatchVisible();
            _UpdateVSPanel();
            _UpdateVSPanelStatusDesc();
            _UpdateVSPanelTimer();
            _UpdateVSPanelShow();
        }

        void _OnMoneyRewardsAwardsChanged(UIEvent uiEvent)
        {
            _UpdateAwardsText();
        }

        void _OnMoneyRewardsPlayerCountChanged(UIEvent uiEvent)
        {
            _UpdatePlayerCount();
        }

        void _OnMoneyRewardsTrySecondMatch(UIEvent uiEvent)
        {
            SystemNotifyManager.SystemNotify(7022, _OnConfirmSecondMatch, null, new object[] { MoneyRewardsDataManager.GetInstance().secondMatchCost, MoneyRewardsDataManager.GetInstance().SecondMoneyName });
        }

        void _OnMoneyRewardsAwardListChanged(UIEvent uiEvent)
        {
            _UpdateAwardsText();
        }

        void _OnMoneyRewardsSelfResultChanged(UIEvent uiEvent)
        {
            _UpdateRanks();
            _UpdateScore();
            _UpdateRate();
            _UpdateEachVsGetAwards();
        }

        void _OnRedPointChanged(UIEvent uiEvent)
        {
            ERedPoint redpointType = (ERedPoint)uiEvent.Param1;

            if (redpointType == ERedPoint.Skill)
            {
                btSkillRedPoint.gameObject.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.Skill));
            }
            else if (redpointType == ERedPoint.DailyProve)
            {
                //DailyProveRedPoint.gameObject.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.DailyProve));
            }
            else
            {
                btPackageRedPoint.gameObject.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.PackageMain));
            }
        }

        void _OnMoneyRewardsAddPartyTimesChanged(UIEvent uiEvent)
        {
            _UpdateMatchButtonStatus();
        }

        void _OnMoneyRewardsResultChanged(UIEvent uiEvent)
        {
            _UpdateVSPanel();
            _UpdateVSPanelTimer();
        }

        void _OnMoneyRewardsBattleInfoChanged(UIEvent uiEvent)
        {
            _UpdateVSPanel();
            _UpdateVSPanelTimer();
        }

        void _OnMoneyRewardsPoolsMoneyChanged(UIEvent uiEvent)
        {
            _UpdatePoolMoneys();
        }

        void _OnConfirmSecondMatch()
        {
            if(!MoneyRewardsDataManager.GetInstance().IsSecondMoneyEnough)
            {
                ItemComeLink.OnLink(MoneyRewardsDataManager.GetInstance().SecondMoneyID, MoneyRewardsDataManager.GetInstance().secondMatchCost, true);
                return;
            }

            MoneyRewardsDataManager.GetInstance().SendAddParty(()=>
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("money_rewards_fixed_desc14"));
            });
        }

        #region match
        GameObject goMask;
        GameObject goTimer;
        bool bIsWaiting = false;
        bool _isMatch = false;
        bool bIsMatch
        {
            set
            {
                _isMatch = value;
                _UpdateBtnMatchVisible();
            }
            get
            {
                return _isMatch;
            }
        }
        Button btnMatch;
        Button btnStopMatch;
        MatchCounterDown comCounterDown;
        GameObject goSeek = null;
        UIAudioProxy comAudio = null;
        GameObject _LoadSeekCom(GameObject goParent, string path = "UIFlatten/Prefabs/Pk/Seek")
        {
            if (goParent == null || string.IsNullOrEmpty(path))
            {
                return null;
            }

            GameObject goSeek = AssetLoader.instance.LoadRes(path, typeof(GameObject)).obj as GameObject;
            Utility.AttachTo(goSeek, goParent);
            comAudio = goSeek.GetComponent<UIAudioProxy>();
            if(null != comAudio)
            {
                comAudio.StopAudio(UIAudioProxy.AudioTigger.OnCall);
            }

            comCounterDown = Utility.FindComponent<MatchCounterDown>(goSeek, "CountDown");
            comCounterDown.enabled = false;

            return goSeek;
        }

        void _InitMatchInfos()
        {
            goMask = Utility.FindChild(frame, "FrontPage");
            goMask.CustomActive(false);
            goTimer = Utility.FindChild(frame, "MatchInfo");
            goTimer.CustomActive(false);
            goSeek = _LoadSeekCom(Utility.FindChild(frame, "MatchInfo"));
            btnMatch = Utility.FindComponent<Button>(frame, "BtnStartMatch");
            btnMatch.onClick.RemoveAllListeners();
            btnStopMatch = Utility.FindComponent<Button>(frame, "BtnStopMatch");
            btnStopMatch.onClick.AddListener(_OnClickStopMatch);
            btnStopMatch.CustomActive(false);
            bIsWaiting = false;
            bIsMatch = false;
        }

        void _OnClickMatch()
        {
            if (bIsMatch)
            {
                return;
            }

            if (!MoneyRewardsDataManager.GetInstance().CanMatch)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("money_rewards_match_stage_error"),ProtoTable.CommonTipsDesc.eShowMode.SI_UNIQUE);
                return;
            }

            if (TeamDataManager.GetInstance().HasTeam())
            {
                SystemNotifyManager.SystemNotify(1104);
                return;
            }
            if (bIsWaiting)
            {
                return;
            }
            bIsWaiting = true;
            bIsMatch = true;

            _BeginMatch();
            goTimer.CustomActive(true);
            comCounterDown.enabled = true;
            if (null != comAudio)
            {
                comAudio.StopAudio(UIAudioProxy.AudioTigger.OnCall);
                comAudio.TriggerAudio(8888);
            }
            MoneyRewardsDataManager.GetInstance().SendMatchParty();
        }

        void _TrySecondMatch()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMoneyRewardsTrySecondMatch);
        }

        void _OnClickStopMatch()
        {
            if (!bIsMatch)
            {
                return;
            }
            if (bIsWaiting)
            {
                return;
            }
            bIsWaiting = true;
            bIsMatch = false;

            btnStopMatch.CustomActive(false);
            goMask.CustomActive(true);

            WorldMatchCancelReq req = new WorldMatchCancelReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);

            WaitNetMessageManager.GetInstance().Wait<WorldMatchCancelRes>(msgRet =>
            {
                if (msgRet == null)
                {
                    return;
                }

                if (msgRet.result != 0)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.result);
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchCancelFailed);
                    return;
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchCancelSuccess);
            });
        }

        void _BeginMatch()
        {
            btnStopMatch.CustomActive(true);
            goMask.CustomActive(true);
        }

        void _OnActiveUpdate(UIEvent uiEvent)
        {
            uint nID = (uint)uiEvent.Param1;
            if (nID == MoneyRewardsDataManager.GetInstance().ActiveID)
            {
                if (ActiveManager.GetInstance().allActivities.ContainsKey((int)nID))
                {
                    var activity = ActiveManager.GetInstance().allActivities[(int)nID];
                    if (activity != null && activity.state == 0)
                    {
                        if (bIsMatch)
                        {
                            WorldMatchCancelReq req = new WorldMatchCancelReq();
                            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
                        }
                    }
                }
            }
        }

        void _OnMatchOk(UIEvent uiEvent)
        {
            bIsWaiting = false;
            bIsMatch = true;
            goMask.CustomActive(true);
        }

        void _OnMatchStop(UIEvent uiEvent)
        {
            bIsWaiting = false;
            bIsMatch = false;
            goMask.CustomActive(false);

            btnStopMatch.CustomActive(false);

            goTimer.CustomActive(false);
            comCounterDown.enabled = false;
            if (null != comAudio)
            {
                comAudio.StopAudio(UIAudioProxy.AudioTigger.OnCall);
            }
        }

        void _OnMatchCancelOk(UIEvent uiEvent)
        {
            bIsWaiting = false;
            bIsMatch = false;
            goMask.CustomActive(false);

            btnStopMatch.CustomActive(false);

            goTimer.CustomActive(false);
            comCounterDown.enabled = false;
            if (null != comAudio)
            {
                comAudio.StopAudio(UIAudioProxy.AudioTigger.OnCall);
            }
        }

        void _OnMatchCancelFailed(UIEvent uiEvent)
        {
            bIsWaiting = false;
            bIsMatch = true;
            goMask.CustomActive(true);

            btnStopMatch.CustomActive(true);
        }
        #endregion

        ComTalk comTalk = null;

        MoneyRewardsMainFrameData data = null;
        protected sealed override void _OnOpenFrame()
        {
            data = userData as MoneyRewardsMainFrameData;
            awards[0] = award_0;
            awards[1] = award_1;
            awards[2] = award_2;
            awards[3] = award_3;
            awards[4] = award_4;

            if (ReplayServer.GetInstance().IsLastPlaying())
            {
                ReplayServer.GetInstance().SetLastPlaying(false);
                MoneyRewardsResultFrame.CommandOpen(null);
            }

            _UpdateAwardsText();
            _UpdateRanks();
            _UpdateScore();
            _UpdateRate();
            _UpdatePlayerCount();
            _UpdateStatus();
            _UpdateVSPanel();
            _UpdateVSPanelStatusDesc();
            _AddButton("ScorePanel/WatchRank", _OnWatchRank);
            _AddButton("ScorePanel/WatchRecords", _OnWatchRecords);
            _AddButton("BtnWatchResult", () =>
            {
                MoneyRewardsResultFrame.CommandOpen(null);
            });
            _AddButton("btMenu", () =>
             {
                 ClientSystemManager.GetInstance().OpenFrame<PkMenuFrame>(FrameLayer.Middle);
             });
            _InitMatchInfos();
            InitRedPoint();
            _UpdateMatchButtonStatus();
            _UpdateBtnMatchVisible();
            _UpdateVSPanelTimer();
            _UpdateVSPanelShow();
            _UpdateEachVSMaxAwards();
            _UpdateEachVsGetAwards();
            _UpdateFixedVSAwards();
            _UpdatePoolMoneys();

            comTalk = ComTalk.Create(Utility.FindChild(frame, "TalkParent"));

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityUpdate, _OnActiveUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PkMatchStartSuccess, _OnMatchOk);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PkMatchFailed, _OnMatchStop);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PkMatchCancelSuccess, _OnMatchCancelOk);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PkMatchCancelFailed, _OnMatchCancelFailed);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsStatusChanged, _OnOnMoneyRewardsStatusChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsAwardsChanged, _OnMoneyRewardsAwardsChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsPlayerCountChanged, _OnMoneyRewardsPlayerCountChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsTrySecondMatch, _OnMoneyRewardsTrySecondMatch);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsAwardListChanged, _OnMoneyRewardsAwardListChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsSelfResultChanged, _OnMoneyRewardsSelfResultChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPointChanged, _OnRedPointChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsAddPartyTimesChanged, _OnMoneyRewardsAddPartyTimesChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsResultChanged, _OnMoneyRewardsResultChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsBattleInfoChanged, _OnMoneyRewardsBattleInfoChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsPoolsMoneyChanged, _OnMoneyRewardsPoolsMoneyChanged);

            InvokeMethod.Invoke(this, 1.0f, _UpdateRequestMySelfRank);
        }

        void _UpdateRequestMySelfRank()
        {
            MoneyRewardsRankFrame._RequestRanklist((int)SortListType.SORTLIST_PREMIUM_LEAGUE, 0, 1, false, _TryNextInvoke);
        }

        void _TryNextInvoke()
        {
            if(null != frame)
            {
                InvokeMethod.Invoke(this, 1.0f, _UpdateRequestMySelfRank);
            }
        }

        void _OnWatchRank()
        {
            MoneyRewardsRankFrame.CommandOpen(null);
        }

        void _OnWatchRecords()
        {
            MoneyRewardsRecordFrame.CommandOpen(null);
        }

        protected sealed override void _OnCloseFrame()
        {
            data = null;
            System.Array.Clear(awards, 0, awards.Length);
            _isMatch = false;

            if (btnMatch != null)
            {
                btnMatch.onClick.RemoveAllListeners();
                btnMatch = null;
            }
            if (btnStopMatch != null)
            {
                btnStopMatch.onClick.RemoveAllListeners();
                btnStopMatch = null;
            }

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PkMatchStartSuccess, _OnMatchOk);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PkMatchFailed, _OnMatchStop);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PkMatchCancelSuccess, _OnMatchCancelOk);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PkMatchCancelFailed, _OnMatchCancelFailed);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityUpdate, _OnActiveUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsStatusChanged, _OnOnMoneyRewardsStatusChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsAwardsChanged, _OnMoneyRewardsAwardsChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsPlayerCountChanged, _OnMoneyRewardsPlayerCountChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsTrySecondMatch, _OnMoneyRewardsTrySecondMatch);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsAwardListChanged, _OnMoneyRewardsAwardListChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsSelfResultChanged, _OnMoneyRewardsSelfResultChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPointChanged, _OnRedPointChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsAddPartyTimesChanged, _OnMoneyRewardsAddPartyTimesChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsResultChanged, _OnMoneyRewardsResultChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsBattleInfoChanged, _OnMoneyRewardsBattleInfoChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsPoolsMoneyChanged, _OnMoneyRewardsPoolsMoneyChanged);

            if (comTalk != null)
            {
                ComTalk.Recycle();
                comTalk = null;
            }

            InvokeMethod.RemoveInvokeCall(this);
            InvokeMethod.RmoveInvokeIntervalCall(mVsPanelObjLock);
            InvokeMethod.RemoveInvokeCall(mVsPanelObjLock);
        }

        void _UpdateAwardsText()
        {
            for(int i = 0; i < awards.Length; ++i)
            {
                if(null != awards[i])
                {
                    if(i < MoneyRewardsDataManager.GetInstance().awardsList.Count)
                    {
                        awards[i].text = MoneyRewardsDataManager.GetInstance().awardsList[i].ToString();
                    }
                    else
                    {
                        awards[i].text = 0.ToString();
                    }
                }
            }
        }

        void _UpdateRanks()
        {
            if(null != MyRank)
            {
                MyRank.text = TR.Value("money_rewards_my_rank", MoneyRewardsDataManager.GetInstance().Rank);
            }
        }

        void _UpdateScore()
        {
            if (null != MyScore)
            {
                MyScore.text = TR.Value("money_rewards_my_current_score", MoneyRewardsDataManager.GetInstance().Score);
            }

            if (null != MaxScore)
            {
                MaxScore.text = TR.Value("money_rewards_my_max_score", MoneyRewardsDataManager.GetInstance().MaxScore);
            }
        }

        void _UpdateRate()
        {
            if (null != MyRate)
            {
                MyRate.text = TR.Value("money_rewards_my_rate", MoneyRewardsDataManager.GetInstance().WinTimes, MoneyRewardsDataManager.GetInstance().LoseTime);
            }
        }

        void _UpdatePlayerCount()
        {
            if(null != playerCount)
            {
                playerCount.text = TR.Value("money_rewards_player_counts", MoneyRewardsDataManager.GetInstance().playerCount);
            }
        }

        void _UpdateStatus()
        {
            if(null != timer)
            {
                timer.Time = 0;
                timer.Initialize();
            }

            if(null != comStatus)
            {
                switch(MoneyRewardsDataManager.GetInstance().eMoneyRewardsStatus)
                {
                    case MoneyRewardsStatus.MRS_INVALID:
                        {
                            comStatus.Key = "finish";
                        }
                        break;
                    case MoneyRewardsStatus.MRS_READY:
                        {
                            comStatus.Key = "ready";
                        }
                        break;
                    case MoneyRewardsStatus.MRS_8_RACE:
                        {
                            comStatus.Key = "8level";
                        }
                        break;
                    case MoneyRewardsStatus.MRS_PRE_4_RACE:
                        {
                            comStatus.Key = "4pre_level";
                        }
                        break;
                    case MoneyRewardsStatus.MRS_4_RACE:
                        {
                            comStatus.Key = "4level";
                        }
                        break;
                    case MoneyRewardsStatus.MRS_2_RACE:
                        {
                            comStatus.Key = "2level";
                        }
                        break;
                    case MoneyRewardsStatus.MRS_RACE:
                        {
                            comStatus.Key = "1level";
                        }
                        break;
                    case MoneyRewardsStatus.MRS_END:
                        {
                            comStatus.Key = "finish";
                        }
                        break;
                }
            }

            uint delta = MoneyRewardsDataManager.GetInstance().StatusEndTime >= TimeManager.GetInstance().GetServerTime() ? MoneyRewardsDataManager.GetInstance().StatusEndTime - TimeManager.GetInstance().GetServerTime() : 0;
            timer.Time = delta;

            goWatch.CustomActive(MoneyRewardsDataManager.GetInstance().Status > PremiumLeagueStatus.PLS_PRELIMINAY);
        }

        object mVsPanelObjLock = new object();
        void _UpdateVSPanelTimer()
        {
            if(null != comMoneyRewardsDataBinder)
            {
                var localData = MoneyRewardsDataManager.GetInstance().GetLocalResultData();
                if (null == localData)
                {
                    InvokeMethod.RmoveInvokeIntervalCall(mVsPanelObjLock);
                    InvokeMethod.RemoveInvokeCall(mVsPanelObjLock);
                    comMoneyRewardsDataBinder.CloseCounter();
                }
                else
                {
                    int index = MoneyRewardsDataManager.GetInstance().getIndexByRoleId(localData.recordId);
                    bool bVSEnable = false;
                    var otherData = MoneyRewardsDataManager.GetInstance().GetNextVsData(index, MoneyRewardsDataManager.GetInstance().Status, ref bVSEnable);
                    if(!bVSEnable)
                    {
                        InvokeMethod.RmoveInvokeIntervalCall(mVsPanelObjLock);
                        InvokeMethod.RemoveInvokeCall(mVsPanelObjLock);
                        comMoneyRewardsDataBinder.CloseCounter();
                    }
                    else
                    {
                        InvokeMethod.RmoveInvokeIntervalCall(mVsPanelObjLock);
                        InvokeMethod.RemoveInvokeCall(mVsPanelObjLock);
                        _UpdateVSPanelTimerCallback();
                        InvokeMethod.InvokeInterval(mVsPanelObjLock,0.0f,1.0f,9999999.0f,null, _UpdateVSPanelTimerCallback,null);
                    }
                }
            }
        }

        void _UpdateVSPanelShow()
        {
            if (null != comMoneyRewardsDataBinder)
            {
                if(MoneyRewardsDataManager.GetInstance().Status <= PremiumLeagueStatus.PLS_PRELIMINAY)
                {
                    comMoneyRewardsDataBinder.SelectVSPanel(false);
                }
                else
                {
                    comMoneyRewardsDataBinder.SelectVSPanel(true);
                }
            }
        }

        void _UpdateEachVSMaxAwards()
        {
            if(null != comMoneyRewardsDataBinder)
            {
                comMoneyRewardsDataBinder.SetVSAwardsDesc();
            }
        }

        void _UpdateFixedVSAwards()
        {
            if(null != comMoneyRewardsDataBinder)
            {
                comMoneyRewardsDataBinder.SetVSEachFixedGetDesc();
            }
        }

        void _UpdateEachVsGetAwards()
        {
            if (null != comMoneyRewardsDataBinder)
            {
                comMoneyRewardsDataBinder.SetVSGetAwardsDesc();
            }
        }

        void _UpdatePoolMoneys()
        {
            if (null != comMoneyRewardsDataBinder)
            {
                comMoneyRewardsDataBinder.SetPoolAwards();
            }
        }

        void _OnHideVSPanelTimer()
        {
            if(null != comMoneyRewardsDataBinder)
            {
                comMoneyRewardsDataBinder.CloseCounter();
            }
        }
        void _UpdateVSPanelTimerCallback()
        {
            uint delta = MoneyRewardsDataManager.GetInstance().StatusEndTime >= TimeManager.GetInstance().GetServerTime() ? MoneyRewardsDataManager.GetInstance().StatusEndTime - TimeManager.GetInstance().GetServerTime() : 0;
            if (delta <= comMoneyRewardsDataBinder.triggerTime)
            {
                InvokeMethod.RmoveInvokeIntervalCall(mVsPanelObjLock);
                InvokeMethod.RemoveInvokeCall(mVsPanelObjLock);
                comMoneyRewardsDataBinder.StartCounter(delta);
                InvokeMethod.Invoke(mVsPanelObjLock, delta, _OnHideVSPanelTimer);
            }
        }

        void _UpdateVSPanel()
        {
            if(null != comMoneyRewardsDataBinder)
            {
                switch (MoneyRewardsDataManager.GetInstance().Status)
                {
                    case PremiumLeagueStatus.PLS_INIT:
                    case PremiumLeagueStatus.PLS_ENROLL:
                    case PremiumLeagueStatus.PLS_PRELIMINAY:
                        {
                            comMoneyRewardsDataBinder.SetVSEnable(false);
                            comMoneyRewardsDataBinder.RemoveWatchListener();
                            comMoneyRewardsDataBinder.SetWatchPlayerInfo(null);
                        }
                        break;
                    case PremiumLeagueStatus.PLS_FINAL_EIGHT_PREPARE:
                    case PremiumLeagueStatus.PLS_FINAL_EIGHT:
                    case PremiumLeagueStatus.PLS_FINAL_FOUR:
                    case PremiumLeagueStatus.PLS_FINAL:
                        {
                            var localData = MoneyRewardsDataManager.GetInstance().GetLocalResultData();
                            if(null == localData)
                            {
                                comMoneyRewardsDataBinder.SetVSEnable(false);
                                comMoneyRewardsDataBinder.RemoveWatchListener();
                                comMoneyRewardsDataBinder.SetWatchPlayerInfo(null);
                            }
                            else
                            {
                                int index = MoneyRewardsDataManager.GetInstance().getIndexByRoleId(localData.recordId);
                                bool bVSEnable = false;
                                var otherData = MoneyRewardsDataManager.GetInstance().GetNextVsData(index, MoneyRewardsDataManager.GetInstance().Status, ref bVSEnable);
                                comMoneyRewardsDataBinder.SetVSEnable(bVSEnable);
                                comMoneyRewardsDataBinder.RemoveWatchListener();
                                comMoneyRewardsDataBinder.SetWatchPlayerInfo(otherData);
                                if (bVSEnable)
                                {
                                    comMoneyRewardsDataBinder.SetPlayerEnable(0, null != localData);
                                    comMoneyRewardsDataBinder.SetPlayerEnable(1, null != otherData);
                                    comMoneyRewardsDataBinder.SetPlayerData(0, localData);
                                    comMoneyRewardsDataBinder.SetPlayerData(1, otherData);
                                    if (null != otherData)
                                    {
                                        comMoneyRewardsDataBinder.AddWatchListener();
                                    }
                                }
                            }
                        }
                        break;
                    case PremiumLeagueStatus.PLS_FINAL_WAIT_CLEAR:
                        {
                            comMoneyRewardsDataBinder.SetVSEnable(false);
                            comMoneyRewardsDataBinder.RemoveWatchListener();
                            comMoneyRewardsDataBinder.SetWatchPlayerInfo(null);
                        }
                        break;
                }
            }
        }
        void _UpdateVSPanelStatusDesc()
        {
            if (null != comMoneyRewardsDataBinder)
            {
                comMoneyRewardsDataBinder.UpdateStatus();
            }
        }

        void _UpdateMatchButtonStatus()
        {
            btnMatch.onClick.RemoveAllListeners();
            if (null != comMatchState)
            {
                switch(MoneyRewardsDataManager.GetInstance().Status)
                {
                    case PremiumLeagueStatus.PLS_INIT:
                        {
                            comMatchState.Key = mUnStart;
                            btnMatch.onClick.AddListener(() =>
                            {
                                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("money_rewards_fixed_desc4"), ProtoTable.CommonTipsDesc.eShowMode.SI_UNIQUE);
                            });
                        }
                        break;
                    case PremiumLeagueStatus.PLS_ENROLL:
                        {
                            comMatchState.Key = mUnStart;
                            btnMatch.onClick.AddListener(() =>
                            {
                                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("money_rewards_fixed_desc2"), ProtoTable.CommonTipsDesc.eShowMode.SI_UNIQUE);
                            });
                        }
                        break;
                    case PremiumLeagueStatus.PLS_PRELIMINAY:
                        {
                            if (MoneyRewardsDataManager.GetInstance().addPartyTimes <= 0)
                            {
                                comMatchState.Key = mNoRank;
                                btnMatch.onClick.AddListener(() =>
                                {
                                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("money_rewards_fixed_desc11"), ProtoTable.CommonTipsDesc.eShowMode.SI_UNIQUE);
                                });
                            }
                            else if (MoneyRewardsDataManager.GetInstance().addPartyTimes == 1)
                            {
                                if(MoneyRewardsDataManager.GetInstance().LoseTime <= 0)
                                {
                                    comMatchState.Key = mNormal;
                                    btnMatch.onClick.AddListener(_OnClickMatch);
                                }
                                else
                                {
                                    comMatchState.Key = mOnceOver;
                                    btnMatch.onClick.AddListener(_TrySecondMatch);
                                }
                            }
                            else if (MoneyRewardsDataManager.GetInstance().addPartyTimes == 2)
                            {
                                if (MoneyRewardsDataManager.GetInstance().LoseTime <= 0)
                                {
                                    comMatchState.Key = mNormal;
                                    btnMatch.onClick.AddListener(_OnClickMatch);
                                }
                                else
                                {
                                    comMatchState.Key = mTwoOver;
                                    btnMatch.onClick.AddListener(() =>
                                    {
                                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("money_rewards_fixed_desc12"), ProtoTable.CommonTipsDesc.eShowMode.SI_UNIQUE);
                                    });
                                }
                            }
                            else
                            {
                                comMatchState.Key = mTwoOver;
                                btnMatch.onClick.AddListener(() =>
                                {
                                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("money_rewards_fixed_desc12"), ProtoTable.CommonTipsDesc.eShowMode.SI_UNIQUE);
                                });
                            }
                        }
                        break;
                    case PremiumLeagueStatus.PLS_FINAL_EIGHT_PREPARE:
                        {
                            comMatchState.Key = mClosed;
                            btnMatch.onClick.AddListener(() =>
                            {
                                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("money_rewards_fixed_desc6"), ProtoTable.CommonTipsDesc.eShowMode.SI_UNIQUE);
                            });
                        }
                        break;
                    case PremiumLeagueStatus.PLS_FINAL_EIGHT:
                        {
                            comMatchState.Key = mClosed;
                            btnMatch.onClick.AddListener(() =>
                            {
                                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("money_rewards_fixed_desc7"), ProtoTable.CommonTipsDesc.eShowMode.SI_UNIQUE);
                            });
                        }
                        break;
                    case PremiumLeagueStatus.PLS_FINAL_FOUR:
                        {
                            comMatchState.Key = mClosed;
                            btnMatch.onClick.AddListener(() =>
                            {
                                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("money_rewards_fixed_desc8"), ProtoTable.CommonTipsDesc.eShowMode.SI_UNIQUE);
                            });
                        }
                        break;
                    case PremiumLeagueStatus.PLS_FINAL:
                        {
                            comMatchState.Key = mClosed;
                            btnMatch.onClick.AddListener(() =>
                            {
                                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("money_rewards_fixed_desc9"), ProtoTable.CommonTipsDesc.eShowMode.SI_UNIQUE);
                            });
                        }
                        break;
                    case PremiumLeagueStatus.PLS_FINAL_WAIT_CLEAR:
                        {
                            comMatchState.Key = mClosed;
                            btnMatch.onClick.AddListener(() =>
                            {
                                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("money_rewards_fixed_desc10"), ProtoTable.CommonTipsDesc.eShowMode.SI_UNIQUE);
                            });
                        }
                        break;
                }
            }
        }

        [UIEventHandle("btReturnToTown")]
        void OnClickReturnToTown()
        {
            if (bIsMatch)
            {
                SystemNotifyManager.SystemNotify(4004);
                return;
            }

            if(MoneyRewardsDataManager.GetInstance().Status == PremiumLeagueStatus.PLS_PRELIMINAY// ||
                /*MoneyRewardsDataManager.GetInstance().Status == PremiumLeagueStatus.PLS_FINAL_EIGHT_PREPARE*/)
            {
                var timeDesc = Utility.ToUtcTime2Local(MoneyRewardsDataManager.GetInstance().StatusEndTime).ToString("HH:mm", Utility.cultureInfo);

                if (MoneyRewardsDataManager.GetInstance().Rank <= 8)
                {
                    SystemNotifyManager.SystemNotify(7028, _ConfirmToReturn, null, new object[] { MoneyRewardsDataManager.GetInstance().Rank , timeDesc });
                    return;
                }
            }

            int iIndex = MoneyRewardsDataManager.GetInstance().getIndexByRoleId(PlayerBaseData.GetInstance().RoleID);
            bool bHasNext = false;
            MoneyRewardsDataManager.GetInstance().GetNextVsData(iIndex, MoneyRewardsDataManager.GetInstance().Status, ref bHasNext);
            if (bHasNext)
            {
                _DoUnLeaveHint();
                return;
            }

            _ConfirmToReturn();
        }

        void _UpdateBtnMatchVisible()
        {
            bool bVisible = MoneyRewardsDataManager.GetInstance().Status < PremiumLeagueStatus.PLS_FINAL_EIGHT_PREPARE && !_isMatch;
            btnMatch.CustomActive(bVisible);
        }

        void _DoUnLeaveHint()
        {
            if(null != comMoneyRewardsDataBinder)
            {
                comMoneyRewardsDataBinder.DoUnLeaveHint();
            }
        }

        void _ConfirmToReturn()
        {
            if (data == null)
            {
                return;
            }

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (systemTown == null)
            {
                Logger.LogError("Current System is not Town!!!");
                return;
            }

            ProtoTable.CitySceneTable TownTableData = TableManager.instance.GetTableItem<ProtoTable.CitySceneTable>(systemTown.CurrentSceneID);
            if (TownTableData == null)
            {
                return;
            }

            GameFrameWork.instance.StartCoroutine(systemTown._NetSyncChangeScene(new SceneParams
            {
                currSceneID = data.CurrentSceneID,
                currDoorID = 0,
                targetSceneID = data.TargetTownSceneID,
                targetDoorID = 0,
            }, true));

            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("btPackage")]
        void OnPackage()
        {
            ClientSystemManager.GetInstance().OpenFrame<PackageNewFrame>(FrameLayer.Middle);
        }

        [UIEventHandle("btSkill")]
        void OnSkill()
        {
            ClientSystemManager.GetInstance().OpenFrame<SkillFrame>(FrameLayer.Middle);
        }

        [UIControl("btPackage/RedPoint")]
        Image btPackageRedPoint;

        [UIControl("btSkill/RedPoint")]
        Image btSkillRedPoint;

        void InitRedPoint()
        {
            btPackageRedPoint.gameObject.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.PackageMain));
            btSkillRedPoint.gameObject.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.Skill));
            //DailyProveRedPoint.gameObject.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.DailyProve));
        }
    }
}