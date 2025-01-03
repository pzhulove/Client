using Protocol;
using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    /// <summary>
    /// 限时玩法按钮整合
    /// </summary>
    public class TimePlayButton : MonoBehaviour
    {
        [SerializeField]private Button mGuildBattleBtn;
        [SerializeField]private Button mBattleMasterBtn;
        [SerializeField]private Button mRewardBattleMasterBtn;
        [SerializeField]private Button mFinalEightVSTableBtn;
        [SerializeField]private Button mGuildDungeonBtn;
        [SerializeField]private Button mCrossServeGuildBattleBtn;
        [SerializeField]private Button m3V3PointRaceBtn;
        [SerializeField]private Button mLevelPlayingFieldBtn;
        [SerializeField] private Button m2V2PointRaceBtn;

        private void Awake()
        {
            BindUIEvent();
            BindButtonEvent();
        }

        private void OnDestroy()
        {
            UnBindUIEvent();
            UnBindButtonEvent();
        }

        private void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildBattleStateChanged, GuildBattleStateChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsStatusChanged, OnMoneyRewardsStatusChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK3V3CrossButton, Update3V3PointRaceBtn);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUpdateFairDuelEntryState, OnUpdateFairDuelEntryState);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityDungeonUpdate, OnActivityDungeonUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK2V2CrossButton, Update2V2PointRaceBtn);

            BudoManager.GetInstance().onBudoInfoChanged += OnBudoInfoChanged;
        }

        private void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildBattleStateChanged, GuildBattleStateChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsStatusChanged, OnMoneyRewardsStatusChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK3V3CrossButton, Update3V3PointRaceBtn);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUpdateFairDuelEntryState, OnUpdateFairDuelEntryState);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityDungeonUpdate, OnActivityDungeonUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK2V2CrossButton, Update2V2PointRaceBtn);

            BudoManager.GetInstance().onBudoInfoChanged -= OnBudoInfoChanged;
        }

        private void BindButtonEvent()
        {
            if (mGuildBattleBtn != null)
            {
                mGuildBattleBtn.onClick.RemoveAllListeners();
                mGuildBattleBtn.onClick.AddListener(OnGuildBattleBtnClick);
            }

            if (mBattleMasterBtn != null)
            {
                mBattleMasterBtn.onClick.RemoveAllListeners();
                mBattleMasterBtn.onClick.AddListener(OnBattleMasterBtnClick);
            }

            if (mRewardBattleMasterBtn != null)
            {
                mRewardBattleMasterBtn.onClick.RemoveAllListeners();
                mRewardBattleMasterBtn.onClick.AddListener(OnRewardBattleMasterBtnClick);
            }

            if (mFinalEightVSTableBtn != null)
            {
                mFinalEightVSTableBtn.onClick.RemoveAllListeners();
                mFinalEightVSTableBtn.onClick.AddListener(OnFinalEightVSTableBtnClick);
            }

            if (m3V3PointRaceBtn != null)
            {
                m3V3PointRaceBtn.onClick.RemoveAllListeners();
                m3V3PointRaceBtn.onClick.AddListener(On3V3PointRaceBtnClick);
            }

            if (mGuildDungeonBtn != null)
            {
                mGuildDungeonBtn.onClick.RemoveAllListeners();
                mGuildDungeonBtn.onClick.AddListener(OnGuildDungeonBtnClick);
            }

            if (mCrossServeGuildBattleBtn != null)
            {
                mCrossServeGuildBattleBtn.onClick.RemoveAllListeners();
                mCrossServeGuildBattleBtn.onClick.AddListener(OnCrossServeGuildBattleBtnClick);
            }

            if (mLevelPlayingFieldBtn != null)
            {
                mLevelPlayingFieldBtn.onClick.RemoveAllListeners();
                mLevelPlayingFieldBtn.onClick.AddListener(OnLevelPlayingFieldBtnClick);
            }

            m2V2PointRaceBtn.SafeSetOnClickListener(() => 
            {
                ClientSystemManager.GetInstance().OpenFrame<JoinPk2v2CrossFrame>();
            });
        }

        private void UnBindButtonEvent()
        {
            if (mGuildBattleBtn != null)
            {
                mGuildBattleBtn.onClick.RemoveListener(OnGuildBattleBtnClick);
            }

            if (mBattleMasterBtn != null)
            {
                mBattleMasterBtn.onClick.RemoveListener(OnBattleMasterBtnClick);
            }

            if (mRewardBattleMasterBtn != null)
            {
                mRewardBattleMasterBtn.onClick.RemoveListener(OnRewardBattleMasterBtnClick);
            }

            if (mFinalEightVSTableBtn != null)
            {
                mFinalEightVSTableBtn.onClick.RemoveListener(OnFinalEightVSTableBtnClick);
            }

            if (m3V3PointRaceBtn != null)
            {
                m3V3PointRaceBtn.onClick.RemoveListener(On3V3PointRaceBtnClick);
            }

            if (mGuildDungeonBtn != null)
            {
                mGuildDungeonBtn.onClick.RemoveListener(OnGuildDungeonBtnClick);
            }

            if (mCrossServeGuildBattleBtn != null)
            {
                mCrossServeGuildBattleBtn.onClick.RemoveListener(OnCrossServeGuildBattleBtnClick);
            }

            if (mLevelPlayingFieldBtn != null)
            {
                mLevelPlayingFieldBtn.onClick.RemoveListener(OnLevelPlayingFieldBtnClick);
            }

            m2V2PointRaceBtn = null;
        }

        public void InitializeMainUI()
        {
            UpdateGuildBattleBtn();
            UpdateBattleMasterBtn();
            UpdateRewardBattleMasterBtn();
            UpdateFinalEightVSTableBtn();
            UpdateGuildDungeonBtn();
            UpdateCrossServeGuildBattleBtn();

            {
                UIEvent uiEvent = new UIEvent();
                uiEvent.Param1 = (byte)Pk3v3CrossDataManager.GetInstance().Get3v3CrossWarStatus();
                Update3V3PointRaceBtn(uiEvent);
            }

            {
                UIEvent uiEvent = new UIEvent();
                uiEvent.Param1 = (byte)Pk2v2CrossDataManager.GetInstance().Get2v2CrossWarStatus();
                Update2V2PointRaceBtn(uiEvent);
            }

            UpdateLevelPlayingFieldBtn();
        }

        /// <summary>
        /// 公会战按钮
        /// </summary>
        private void UpdateGuildBattleBtn()
        {
            mGuildBattleBtn.CustomActive(GuildDataManager.GetInstance().IsInGuildBattle());
        }

        /// <summary>
        /// 武道大会
        /// </summary>
        private void UpdateBattleMasterBtn()
        {
            var activityDungeonData = ActivityDungeonDataManager.GetInstance().GetSubByActivityDungeonID(8);
            if (activityDungeonData == null)
            {
                return;
            }

            //活动状态判断
            if (activityDungeonData.activityInfo != null)
            {
                var actState = activityDungeonData.activityInfo.state;
                mBattleMasterBtn.CustomActive(actState == eActivityDungeonState.Start);
            }
        }

        /// <summary>
        /// 赏金武道大会
        /// </summary>
        private void UpdateRewardBattleMasterBtn()
        {
            var activityDungeonData = ActivityDungeonDataManager.GetInstance().GetSubByActivityDungeonID(20);
            if (activityDungeonData == null)
            {
                return;
            }

            //活动状态判断
            if (activityDungeonData.activityInfo != null)
            {
                var actState = activityDungeonData.activityInfo.state;
                mRewardBattleMasterBtn.CustomActive(actState == eActivityDungeonState.Start);
            }
        }

        /// <summary>
        /// 八强对战表
        /// </summary>
        private void UpdateFinalEightVSTableBtn()
        {
            mFinalEightVSTableBtn.CustomActive(MoneyRewardsDataManager.GetInstance().Status > PremiumLeagueStatus.PLS_PRELIMINAY);
        }

        /// <summary>
        /// 公会地下城
        /// </summary>
        private void UpdateGuildDungeonBtn()
        {
            mGuildDungeonBtn.CustomActive(ActivityDungeonFrame.GetGuildDungeonActivityState() == eActivityDungeonState.Start);
        }

        /// <summary>
        /// 跨服公会站
        /// </summary>
        private void UpdateCrossServeGuildBattleBtn()
        {
            mCrossServeGuildBattleBtn.CustomActive(ActivityDungeonFrame.GetGuildCrossBattleActivityState() == eActivityDungeonState.Start);
        }

        /// <summary>
        /// 公平竞技场
        /// </summary>
        private void UpdateLevelPlayingFieldBtn()
        {
            mLevelPlayingFieldBtn.CustomActive(FairDuelDataManager.GetInstance().IsShowFairDuelEnterBtn());
        }

        /// <summary>
        /// 3v3积分赛
        /// </summary>
        /// <param name="uiEvent"></param>
        private void Update3V3PointRaceBtn(UIEvent uiEvent)
        {
            var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_SCORE_WAR_LEVEL);
            if (SystemValueTableData != null && PlayerBaseData.GetInstance().Level < SystemValueTableData.Value)
            {
                m3V3PointRaceBtn.CustomActive(false);
                return;
            }

            byte status = (byte)uiEvent.Param1;

            bool isFlag = false;
            if (status >= (byte)ScoreWarStatus.SWS_PREPARE && status < (byte)ScoreWarStatus.SWS_WAIT_END)
            {
                isFlag = true;
            }
            else
            {
                isFlag = false;
            }

            m3V3PointRaceBtn.CustomActive(isFlag);
        }

        private void Update2V2PointRaceBtn(UIEvent uiEvent)
        {
            var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType3.SVT_2V2_SCORE_WAR_LEVEL);
            if (SystemValueTableData != null && PlayerBaseData.GetInstance().Level < SystemValueTableData.Value)
            {
                m2V2PointRaceBtn.CustomActive(false);
                return;
            }

            byte status = (byte)uiEvent.Param1;

            bool isFlag = false;
            if (status >= (byte)ScoreWar2V2Status.SWS_2V2_PREPARE && status < (byte)ScoreWar2V2Status.SWS_2V2_WAIT_END)
            {
                isFlag = true;
            }
            else
            {
                isFlag = false;
            }

            m2V2PointRaceBtn.CustomActive(isFlag);
        }

        private void OnUpdateFairDuelEntryState(UIEvent uiEvent)
        {
            bool isShow = (bool)uiEvent.Param1;
            mLevelPlayingFieldBtn.CustomActive(isShow);
        }

        private void OnActivityDungeonUpdate(UIEvent uiEvent)
        {
            UpdateGuildDungeonBtn();
            UpdateCrossServeGuildBattleBtn();
            UpdateRewardBattleMasterBtn();
            UpdateBattleMasterBtn();
        }
        
        private void GuildBattleStateChanged(UIEvent uiEvent)
        {
            UpdateGuildBattleBtn();
        }

        private void OnMoneyRewardsStatusChanged(UIEvent uiEvent)
        {
            UpdateRewardBattleMasterBtn();
            UpdateFinalEightVSTableBtn();
        }

        private void OnBudoInfoChanged()
        {
            UpdateBattleMasterBtn();
        }

        #region BindButtonEvent

        private void OnGuildBattleBtnClick()
        {
            if (GuildDataManager.GetInstance().myGuild != null)
            {
                EGuildBattleState state = GuildDataManager.GetInstance().GetGuildBattleState();

                if (state >= EGuildBattleState.Preparing && state <= EGuildBattleState.Firing)
                {
                    if (GuildDataManager.GetInstance().HasTargetManor())
                    {
                        if (GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS)
                        {
                            ClientSystemManager.GetInstance().OpenFrame<GuildCrossManorFrame>();
                        }
                        else
                        {
                            ClientSystemManager.GetInstance().OpenFrame<GuildManorFrame>();
                        }
                    }
                    else
                    {
                        if (GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS)
                        {
                            GuildMainFrame.OpenLinkFrame("10");
                        }
                        else
                        {
                            GuildMainFrame.OpenLinkFrame("8");
                        }
                    }

                    return;
                }
            }
            else
            {
                ClientSystemManager.GetInstance().OpenFrame<GuildListFrame>(FrameLayer.Middle);
            }
        }

        private void OnBattleMasterBtnClick()
        {
            if (!Utility.EnterBudo())
            {
                return;
            }
        }

        private void OnRewardBattleMasterBtnClick()
        {
            MoneyRewardsEnterFrame.CommandOpen(null);
        }

        private void OnFinalEightVSTableBtnClick()
        {
            var type = typeof(GameClient.ClientFrame).Assembly.GetType("GameClient.MoneyRewardsResultFrame");
            if (null == type)
            {
                return;
            }

            var methodInfo = type.GetMethod("CommandOpen");
            if (null == methodInfo)
            {
                return;
            }

            methodInfo.Invoke(null, new object[] { null });
        }

        private void On3V3PointRaceBtnClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<JoinPK3v3CrossFrame>(FrameLayer.Middle);
        }

        private void OnGuildDungeonBtnClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<GuildMainFrame>();
        }

        private void OnCrossServeGuildBattleBtnClick()
        {
            GuildMainFrame.OpenLinkFrame("14");
        }

        private void OnLevelPlayingFieldBtnClick()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null) return;

            PkWaitingRoomData roomData = new PkWaitingRoomData
            {
                CurrentSceneID = systemTown.CurrentSceneID,
                TargetTownSceneID = 6033,
                SceneSubType = CitySceneTable.eSceneSubType.FairDuelPrepare
            };

            ClientSystemManager.GetInstance().OpenFrame<FairDuelEntranceFrame>(FrameLayer.Middle, roomData);
        }

        #endregion
    }
}