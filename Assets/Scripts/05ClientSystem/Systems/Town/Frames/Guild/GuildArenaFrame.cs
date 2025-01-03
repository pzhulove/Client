using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using UnityEngine.Assertions;
using Protocol;
using Network;
using System.Collections;

namespace GameClient
{
    public class GuildArenaData
    {
        public CitySceneTable.eSceneSubType SceneSubType = CitySceneTable.eSceneSubType.NULL;
        public int CurrentSceneID = 0;
        public int TargetTownSceneID = 0;

        public void Clear()
        {
            SceneSubType = CitySceneTable.eSceneSubType.NULL;
            CurrentSceneID = 0;
            TargetTownSceneID = 0;
        }
    }

    class GuildArenaFrame : ClientFrame
    {
        ComTalk m_miniTalk;
        GuildArenaData guildArenaData = new GuildArenaData();
        const int activityUICloseDelaySecond = 30 * 60;
        const float fReqGuildActivityDataInterval = 5.0f;
        const float fireworksTime = 30.0f;
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildArenaFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            BindUIEvent();
            guildArenaData = userData as GuildArenaData;
            InitComTalk();
            _OnUpdateActivityData(null);
            GuildDataManager.GuildDungeonActivityData data = GuildDataManager.GetInstance().GetGuildDungeonActivityData();
            if(data != null && data.nActivityState != (int)GuildDungeonStatus.GUILD_DUNGEON_END && GuildDataManager.CheckActivityLimit())
            {
                GuildDataManager.GetInstance().SendWorldGuildDungeonInfoReq();
                GuildDataManager.GetInstance().SendWorldGuildDungeonCopyReq();
            }
            GuildDataManager.GetInstance().SendWorldGuildDungeonStatueReq();
            InvokeMethod.InvokeInterval(this,
                fReqGuildActivityDataInterval,
                fReqGuildActivityDataInterval,
                99999999.0f,
                null,
                () =>
                {
//                     GuildDataManager.GuildDungeonActivityData activityData = GuildDataManager.GetInstance().GetGuildDungeonActivityData();
//                     if (activityData != null && activityData.nActivityState != (int)GuildDungeonStatus.GUILD_DUNGEON_END && GuildDataManager.CheckActivityLimit())
//                     {
//                         GuildDataManager.GetInstance().SendWorldGuildDungeonInfoReq();
//                         GuildDataManager.GetInstance().SendWorldGuildDungeonCopyReq();
//                     }
                }
           , null);
            _OnShowFireworks(null);
            _OnGuildDungeonAuctionStateUpdate(null);
            TryOpenGuildDungeonBossKillRankListFrame();
        }

        protected IEnumerator OpenTeamMainFrame()
        {
            yield return Yielders.EndOfFrame;
            yield return Yielders.EndOfFrame;
            if(TeamDataManager.GetInstance().HasTeam())
            {
                ClientSystemManager.GetInstance().OpenFrame<TeamMainFrame>(FrameLayer.Bottom);
                ClientSystemManager.GetInstance().GetFrame(typeof(TeamMainFrame)).GetFrame().CustomActive(true);
            }
            else
            {
                ClientSystemManager.GetInstance().OpenFrame<TeamMainMenuFrame>(FrameLayer.Bottom);
                ClientSystemManager.GetInstance().GetFrame(typeof(TeamMainMenuFrame)).GetFrame().CustomActive(true);
            }
        }
        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildDungeonUpdateActivityData, _OnUpdateActivityData);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildOpenGuildDungeonTreasureChests, _OnOpenGuildDungeonTreasureChests);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildDungeonSyncActivityState, _OnSyncActivityState);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildDungeonShowFireworks, _OnShowFireworks);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnGuildDungeonAuctionStateUpdate, _OnGuildDungeonAuctionStateUpdate);
        }
        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildDungeonUpdateActivityData, _OnUpdateActivityData);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildOpenGuildDungeonTreasureChests, _OnOpenGuildDungeonTreasureChests);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildDungeonSyncActivityState, _OnSyncActivityState);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildDungeonShowFireworks, _OnShowFireworks);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnGuildDungeonAuctionStateUpdate, _OnGuildDungeonAuctionStateUpdate);
        }
        public override bool IsNeedUpdate()
        {
            return true;
        }
        protected override void _OnUpdate(float timeElapsed)
        {
            if (TeamDataManager.GetInstance().HasTeam())
            {
                if(!ClientSystemManager.GetInstance().IsFrameOpen<TeamMainFrame>())
                {
                    ClientSystemManager.GetInstance().OpenFrame<TeamMainFrame>(FrameLayer.Bottom);                   
                    ClientSystemManager.GetInstance().GetFrame(typeof(TeamMainFrame)).GetFrame().CustomActive(true);
                }
                else if(ClientSystemManager.GetInstance().IsFrameHidden(typeof(TeamMainFrame)))
                {                   
                    ClientSystemManager.GetInstance().GetFrame(typeof(TeamMainFrame)).GetFrame().CustomActive(true);
                } 
            }
            else
            {
                if (!ClientSystemManager.GetInstance().IsFrameOpen<TeamMainMenuFrame>())
                {
                    ClientSystemManager.GetInstance().OpenFrame<TeamMainMenuFrame>(FrameLayer.Bottom);
                    ClientSystemManager.GetInstance().GetFrame(typeof(TeamMainMenuFrame)).GetFrame().CustomActive(true);
                }
                else if (ClientSystemManager.GetInstance().IsFrameHidden(typeof(TeamMainMenuFrame)))
                {
                    ClientSystemManager.GetInstance().GetFrame(typeof(TeamMainMenuFrame)).GetFrame().CustomActive(true);
                }
            }
            GuildDataManager.GuildDungeonActivityData data = GuildDataManager.GetInstance().GetGuildDungeonActivityData();
            if (data != null)
            {
                if (ClientSystemManager.GetInstance().IsFrameOpen<TeamMainMenuFrame>())
                {                    
                    ClientSystemManager.GetInstance().GetFrame(typeof(TeamMainMenuFrame)).GetFrame().CustomActive(data.nActivityState != (int)GuildDungeonStatus.GUILD_DUNGEON_END);
                }
                else if (ClientSystemManager.GetInstance().IsFrameOpen(typeof(TeamMainFrame)))
                {
                    ClientSystemManager.GetInstance().GetFrame(typeof(TeamMainFrame)).GetFrame().CustomActive(data.nActivityState != (int)GuildDungeonStatus.GUILD_DUNGEON_END);
                }
            }
            if(!GuildDataManager.CheckActivityLimit())
            {
                if (ClientSystemManager.GetInstance().IsFrameOpen<TeamMainMenuFrame>())
                {
                    ClientSystemManager.GetInstance().GetFrame(typeof(TeamMainMenuFrame)).GetFrame().CustomActive(false);
                }
                else if (ClientSystemManager.GetInstance().IsFrameOpen(typeof(TeamMainFrame)))
                {
                    ClientSystemManager.GetInstance().GetFrame(typeof(TeamMainFrame)).GetFrame().CustomActive(false);
                }
            }
            UpdateStateInfo();
        }
        protected sealed override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SwitchToMission);
            InvokeMethod.RmoveInvokeIntervalCall(this);
            InvokeMethod.RemoveInvokeCall(this);
            UnBindUIEvent();
           if (m_miniTalk != null)
           {
                ComTalk.Recycle();
                m_miniTalk = null;
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<ClientSystemTownFrame>())
            {
                ClientSystemTownFrame Townframe = ClientSystemManager.GetInstance().GetFrame(typeof(ClientSystemTownFrame)) as ClientSystemTownFrame;
                Townframe.SetForbidFadeIn(false);
            }
            ClientSystemManager.GetInstance().CloseFrame<GuildDungeonBossKillRankListFrame>();
        }

        void _RegisterUIEvent()
        {
           
        }

        void _UnRegisterUIEvent()
        {
           
        }

        void _OnUpdateActivityData(UIEvent uiEvent)
        {
            GuildDataManager.GuildDungeonActivityData data = GuildDataManager.GetInstance().GetGuildDungeonActivityData();
            if(data != null && data.nActivityState == (int)GuildDungeonStatus.GUILD_DUNGEON_END)
            {
                ShowAcitivityUIs(false);
                return;
            }
            if(!GuildDataManager.CheckActivityLimit())
            {
                ShowAcitivityUIs(false);
                return;
            }
            ShowAcitivityUIs(true);
            UpdateKillInfo();
            UpdateStateInfo();
        }
        void _OnSyncActivityState(UIEvent uiEvent)
        {
            uint nOldState = (uint)uiEvent.Param1;
            uint nNewState = (uint)uiEvent.Param2;
            if(nNewState == (int)GuildDungeonStatus.GUILD_DUNGEON_START || nNewState == (int)GuildDungeonStatus.GUILD_DUNGEON_REWARD)
            {
                ClientSystemManager.GetInstance().OpenFrame<GuildDungeonBossKillRankListFrame>(FrameLayer.Bottom);
            }
            else
            {
                ClientSystemManager.GetInstance().CloseFrame<GuildDungeonBossKillRankListFrame>();
            }
            if(nOldState == (int)GuildDungeonStatus.GUILD_DUNGEON_START || nNewState == (int)GuildDungeonStatus.GUILD_DUNGEON_REWARD)
            {
                InvokeMethod.Invoke(this, activityUICloseDelaySecond,() => 
                {
                    ShowAcitivityUIs(false);
                });             
            }
            if(nNewState == (int)GuildDungeonStatus.GUILD_DUNGEON_END)
            {
                ShowAcitivityUIs(false);
            }
            else
            {
                ShowAcitivityUIs(true);
            }
            if (!GuildDataManager.CheckActivityLimit())
            {
                ShowAcitivityUIs(false);
                return;
            }
            return;
        }
        void _OnOpenGuildDungeonTreasureChests(UIEvent uiEvent)
        {
            List<GuildDungeonActivityChestItem> items = uiEvent.Param1 as List<GuildDungeonActivityChestItem>;
            if(items == null)
            {
                return;
            }
            ClientSystemManager.GetInstance().OpenFrame<ShowGuildDungeonChestItemsFrame>(FrameLayer.Middle, items);
            return;
        }
        void InitComTalk()
        {
            m_miniTalk = ComTalk.Create(mTalkParent);
        }
		#region ExtraUIBind
		private GameObject mTalkParent = null;
		private Button mSkill = null;
		private Button mPackge = null;
		private Button mGuild = null;
		private Button mFriend = null;
		private Button mBtReturnToTown = null;
        private GameObject Rank_Award = null;
        private Button btRank = null;
        private Button btAward = null;
        private Text txtKillInfo = null;
        private Slider process = null;
        private Text txtStateInfo = null;
        private Button btEnter = null;
        private Button btOpen = null;
        private GameObject stateInfo = null;
        private GameObject bossKillInfo = null;
        private Button btnbossKillInfo = null;
        private GameObject yanhuaRoot = null;
        private GameObject mediumBossKillInfo = null;
        private Button btnMediumBossKillInfo = null;
        private Image verifyBlood = null;
        private Button guildDungeonAuction = null;
		
        MediumGuildDungeonMini Medium0 = null;
        MediumGuildDungeonMini Medium1 = null;
        MediumGuildDungeonMini Medium2 = null;
		protected override void _bindExUI()
		{
			mTalkParent = mBind.GetGameObject("TalkParent");
			mSkill = mBind.GetCom<Button>("skill");
			if (null != mSkill)
			{
				mSkill.onClick.AddListener(_onSkillButtonClick);
			}
			mPackge = mBind.GetCom<Button>("packge");
			if (null != mPackge)
			{
				mPackge.onClick.AddListener(_onPackgeButtonClick);
			}
			mGuild = mBind.GetCom<Button>("guild");
			if (null != mGuild)
			{
				mGuild.onClick.AddListener(_onGuildButtonClick);
			}
			mFriend = mBind.GetCom<Button>("friend");
			if (null != mFriend)
			{
				mFriend.onClick.AddListener(_onFriendButtonClick);
			}
			mBtReturnToTown = mBind.GetCom<Button>("btReturnToTown");
			if (null != mBtReturnToTown)
			{
				mBtReturnToTown.onClick.AddListener(_onBtReturnToTownButtonClick);
			}
            Rank_Award = mBind.GetGameObject("Rank_Award");
            btRank = mBind.GetCom<Button>("btRank");
            if(btRank != null)
            {
                btRank.onClick.RemoveAllListeners();
                btRank.onClick.AddListener(() => 
                {
                    ClientSystemManager.GetInstance().OpenFrame<GuildDungeonDamageRankListFrame>();
                });
            }
            btAward = mBind.GetCom<Button>("btAward");
            if(btAward != null)
            {
                btAward.onClick.RemoveAllListeners();
                btAward.onClick.AddListener(() => 
                {
                    //ClientSystemManager.GetInstance().OpenFrame<GuildDungeonAwardShowFrame>();
                });
            }
            txtKillInfo = mBind.GetCom<Text>("txtKillInfo");
            process = mBind.GetCom<Slider>("process");
            txtStateInfo = mBind.GetCom<Text>("txtStateInfo");
            btEnter = mBind.GetCom<Button>("btEnter");
            if(btEnter != null)
            {
                btEnter.onClick.RemoveAllListeners();
                btEnter.onClick.AddListener(() => 
                {
                });
            }
            btOpen = mBind.GetCom<Button>("btOpen");
            if(btOpen != null)
            {
                btOpen.onClick.RemoveAllListeners();
                btOpen.onClick.AddListener(() => 
                {
                    GuildDataManager.GetInstance().TryGetGuildDungeonActivityChestAward();
                });
            }
            stateInfo = mBind.GetGameObject("stateInfo");
            bossKillInfo = mBind.GetGameObject("bossKillInfo");
            mediumBossKillInfo = mBind.GetGameObject("mediumBossKillInfo");
            btnbossKillInfo = mBind.GetCom<Button>("btnbossKillInfo");
            if(btnbossKillInfo != null)
            {
                btnbossKillInfo.onClick.RemoveAllListeners();
                btnbossKillInfo.onClick.AddListener(() => 
                {
                    MoveToGuildDungeonGate();
                });
            }
            btnMediumBossKillInfo = mBind.GetCom<Button>("btnMediumBossKillInfo");
            if (btnMediumBossKillInfo != null)
            {
                btnMediumBossKillInfo.onClick.RemoveAllListeners();
                btnMediumBossKillInfo.onClick.AddListener(() =>
                {
                    MoveToGuildDungeonGate();
                });
            }

            yanhuaRoot = mBind.GetGameObject("yanhuaRoot");
            Medium0 = mBind.GetCom<MediumGuildDungeonMini>("Medium0");
            Medium1 = mBind.GetCom<MediumGuildDungeonMini>("Medium1");
            Medium2 = mBind.GetCom<MediumGuildDungeonMini>("Medium2");
            verifyBlood = mBind.GetCom<Image>("verifyBlood");
            guildDungeonAuction = mBind.GetCom<Button>("guildDungeonAuction");
            guildDungeonAuction.SafeRemoveAllListener();
            guildDungeonAuction.SafeAddOnClickListener(() => 
            {
                ClientSystemManager.GetInstance().OpenFrame<GuildDungeonAuctionFrame>(FrameLayer.Middle, GuildDungeonAuctionFrame.FrameType.GuildAuction);
            });
		}
		
		protected override void _unbindExUI()
		{
			mTalkParent = null;
			if (null != mSkill)
			{
				mSkill.onClick.RemoveListener(_onSkillButtonClick);
			}
			mSkill = null;
			if (null != mPackge)
			{
				mPackge.onClick.RemoveListener(_onPackgeButtonClick);
			}
			mPackge = null;
			if (null != mGuild)
			{
				mGuild.onClick.RemoveListener(_onGuildButtonClick);
			}
			mGuild = null;
			if (null != mFriend)
			{
				mFriend.onClick.RemoveListener(_onFriendButtonClick);
			}
			mFriend = null;
			if (null != mBtReturnToTown)
			{
				mBtReturnToTown.onClick.RemoveListener(_onBtReturnToTownButtonClick);
			}
			mBtReturnToTown = null;
            Rank_Award = null;
            btRank = null;
            btAward = null;
            txtKillInfo = null;
            process = null;
            txtStateInfo = null;
            btEnter = null;
            stateInfo = null;
            bossKillInfo = null;
            btnbossKillInfo = null;
            yanhuaRoot = null;
            mediumBossKillInfo = null;
            btnMediumBossKillInfo = null;
            Medium0 = null;
            Medium1 = null;
            Medium2 = null;
            verifyBlood = null;
            guildDungeonAuction = null;
		}
		#endregion

        #region Callback
        private void _onBtReturnToTownButtonClick()
        {
//             if(TeamDataManager.GetInstance().HasTeam())
//             {
//                 SystemNotifyManager.SysNotifyFloatingEffect("当前处于组队状态，无法退出场景");
//                 return;
//             }

            SwitchSceneToTown();
        }
        private void _onSkillButtonClick()
        {
            frameMgr.OpenFrame<SkillFrame>(FrameLayer.Middle);
        }
        private void _onPackgeButtonClick()
        {
            frameMgr.OpenFrame<PackageNewFrame>(FrameLayer.Middle);
        }
        private void _onGuildButtonClick()
        {
            frameMgr.OpenFrame<GuildMainFrame>(FrameLayer.Middle);
        }
        private void _onFriendButtonClick()
        {
            RelationFrameNew.CommandOpen();
        }
        #endregion
        void MoveToGuildDungeonGate()
        {
            if (GuildDataManager.CheckActivityLimit() && GuildDataManager.GetInstance().GetGuildDungeonActivityStatus() == GuildDungeonStatus.GUILD_DUNGEON_START)
            {
                ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                if (systemTown != null && systemTown.MainPlayer != null)
                {
                    systemTown.MainPlayer.CommandMoveToScene(GuildDataManager.nGuildDungeonMapScenceID);
                    return;
                }
            }
            return;
        }
        void UpdateKillInfo()
        {
            GuildDataManager.GuildDungeonActivityData data = GuildDataManager.GetInstance().GetGuildDungeonActivityData();
            if (data != null)
            {
                if (txtKillInfo != null)
                {
                    txtKillInfo.text = string.Format("{0}/{1}", data.nBossOddHp, data.nBossMaxHp);
                }
                if(process != null && data.nBossMaxHp > 0)
                {
                    process.value = (float)data.nBossOddHp / (float)data.nBossMaxHp;                   
                }
                if(verifyBlood != null && data.nBossMaxHp > 0)
                {
                    verifyBlood.fillAmount = (float)data.nVerifyBlood / (float)data.nBossMaxHp;
            } 
            }
            List<MediumGuildDungeonMini> mediums = new List<MediumGuildDungeonMini>();
            mediums.Add(Medium0);
            mediums.Add(Medium1);
            mediums.Add(Medium2);
            for (int i = 0; i < data.mediumDungeonDamgeInfos.Count; i++)
            {
                if (i < mediums.Count)
                {
                    MediumGuildDungeonMini medium = mediums[i];
                    if (medium != null)
                    {
                        medium.SetUp(data.mediumDungeonDamgeInfos[i]);
                    }
                }
            }
        }
        void GetLeftTime(uint nEndTime, uint nNowTime, ref int nLeftDay, ref int nLeftHour, ref int nLeftMin, ref int nLeftSec)
        {
            nLeftDay = 0;
            nLeftHour = 0;
            nLeftMin = 0;
            nLeftSec = 0;
            if (nEndTime <= nNowTime)
            {
                return;
            }
            uint LeftTime = nEndTime - nNowTime;
            uint Day = LeftTime / (24 * 60 * 60);
            LeftTime -= Day * 24 * 60 * 60;
            uint Hour = LeftTime / (60 * 60);
            LeftTime -= Hour * 60 * 60;
            uint Minute = LeftTime / 60;
            LeftTime -= Minute * 60;
            uint Second = LeftTime;
            nLeftDay = (int)Day;
            nLeftHour = (int)Hour;
            nLeftMin = (int)Minute;
            nLeftSec = (int)Second;
            return;
        }        
        void ShowAcitivityUIs(bool bShow)
        {
            Rank_Award.CustomActive(bShow);
            stateInfo.CustomActive(bShow);
            bossKillInfo.CustomActive(bShow);
            mediumBossKillInfo.CustomActive(bShow);
            GuildDataManager.GuildDungeonActivityData data = GuildDataManager.GetInstance().GetGuildDungeonActivityData();
            if (data != null)
            {
                if (bShow)
                {
                    bool bIsBossDungeonOpen = true;
                    for (int i = 0; i < data.mediumDungeonDamgeInfos.Count; i++)
                    {
                        if (data.mediumDungeonDamgeInfos[i].nOddHp > 0)
                        {
                            bIsBossDungeonOpen = false;
                            break;
                        }
                    }
                    bossKillInfo.CustomActive(bIsBossDungeonOpen);
                    mediumBossKillInfo.CustomActive(!bIsBossDungeonOpen);

                    // 准备阶段不显示两种血条 不显示排行版
                    if (data.nActivityState == (int)GuildDungeonStatus.GUILD_DUNGEON_PREPARE)
                    {
                        bossKillInfo.CustomActive(false);
                        mediumBossKillInfo.CustomActive(false);
                        Rank_Award.CustomActive(false);
                    }
                }
            }
            if (bShow)
            {
            }
            else
            {
            }
        }
        void _OnShowFireworks(UIEvent uiEvent)
        {
            if (GuildDataManager.GetInstance().IsShowFireworks)
            {
                GuildDataManager.GetInstance().IsShowFireworks = false;
                GuildDataManager.GuildDungeonActivityData data = GuildDataManager.GetInstance().GetGuildDungeonActivityData();
                if (data != null && GuildDataManager.CheckActivityLimit() && data.nActivityState != (int)GuildDungeonStatus.GUILD_DUNGEON_END)
                {
                    yanhuaRoot.CustomActive(true);
                    InvokeMethod.Invoke(this, fireworksTime, () =>
                    {
                        yanhuaRoot.CustomActive(false);
                    });
                }               
            }
            else
            {
                yanhuaRoot.CustomActive(false);
            }
        }
        void UpdateStateInfo()
        {
            GuildDataManager.GuildDungeonActivityData data = GuildDataManager.GetInstance().GetGuildDungeonActivityData();
            if (data != null)
            {
                if (txtStateInfo != null)
                {
                    int nLeftDay = 0;
                    int nLeftHour = 0;
                    int nLeftMin = 0;
                    int nLeftSec = 0;
                    GetLeftTime(data.nActivityStateEndTime, TimeManager.GetInstance().GetServerTime(), ref nLeftDay, ref nLeftHour, ref nLeftMin, ref nLeftSec);                  
                    if (data.nActivityState == (int)GuildDungeonStatus.GUILD_DUNGEON_END)
                    {
                        txtStateInfo.text = string.Format("地下城已关闭");
                    }
                    else if(data.nActivityState == (int)GuildDungeonStatus.GUILD_DUNGEON_PREPARE)
                    {
                        txtStateInfo.text = string.Format("地下城即将开启: {0}:{1}:{2}", string.Format("{0:00}", nLeftHour), string.Format("{0:00}", nLeftMin), string.Format("{0:00}", nLeftSec));
                    }
                    else if(data.nActivityState == (int)GuildDungeonStatus.GUILD_DUNGEON_START)
                    {
                        txtStateInfo.text = string.Format("地下城持续时间: {0}:{1}:{2}", string.Format("{0:00}", nLeftHour), string.Format("{0:00}", nLeftMin), string.Format("{0:00}", nLeftSec));
                        if(data.nBossOddHp == 0)
                        {
                            txtStateInfo.text = "Boss已被击杀";
                        }
                    }
                    else if(data.nActivityState == (int)GuildDungeonStatus.GUILD_DUNGEON_REWARD)
                    {
                        txtStateInfo.text = string.Format("地下城已关闭");
                    }                    
                }
            }               
        }
        void _OnGuildDungeonAuctionStateUpdate(UIEvent uiEvent)
        {
            guildDungeonAuction.CustomActive(GuildDataManager.GetInstance().IsGuildAuctionOpen);
        }

        void SwitchSceneToTown()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            GameFrameWork.instance.StartCoroutine(systemTown._NetSyncChangeScene(new SceneParams
            {
                currSceneID = guildArenaData.CurrentSceneID,
                currDoorID = 0,
                targetSceneID = guildArenaData.TargetTownSceneID,
                targetDoorID = 0,
            }, true));

            frameMgr.CloseFrame(this);
        }
        void TryOpenGuildDungeonBossKillRankListFrame()
        {
            GuildDungeonStatus status = GuildDataManager.GetInstance().GetGuildDungeonActivityStatus();
            if (GuildDataManager.CheckActivityLimit() && (status == GuildDungeonStatus.GUILD_DUNGEON_START || status == GuildDungeonStatus.GUILD_DUNGEON_REWARD))
            {
                ClientSystemManager.GetInstance().OpenFrame<GuildDungeonBossKillRankListFrame>(FrameLayer.Bottom);
            }
            return;
        }
    }
}
