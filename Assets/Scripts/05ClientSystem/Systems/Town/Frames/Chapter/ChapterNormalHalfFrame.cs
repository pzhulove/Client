using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Protocol;
using Network;
using ProtoTable;
using System;
using ActivityLimitTime;

namespace GameClient
{
    public class ChapterNormalHalfFrame : ChapterBaseFrame
    {
        private bool mBisFlag = false; //判断疲劳燃烧是否显示
        private ActivityLimitTime.ActivityLimitTimeData data = null;  //疲劳燃烧活动数据
        private ActivityLimitTime.ActivityLimitTimeDetailData mData = null; //疲劳燃烧活动开启的子任务数据
        private bool mFatigueCombustionTimeIsOpen = false;//是否正在倒计时
        private Text mTime; //疲劳燃烧时间Text文本
        private int mFatigueCombustionTime = -1;//用于疲劳燃烧结束时间戳
        private uint mAreaIndex = 0;


        bool bToggleInit = false; // 用来解决toggle在界面打开的时候会调用一次回调的问题

        //周年派对地下城id集合
        private List<int> mAniversaryPartyDungeonIdList = new List<int>() { 6511000,6512000 };

        private int mSpringFestivalDungeonId = 6521000;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chapter/Normal/ChapterNormalHalf.prefab";
        }

#region ExtraUIBind

        private GameObject mlevelChallengeTimesRoot = null;
        private Text mRebornLimitNumberValue = null;
        private Text mLevelChallengeTimesNumber = null;
        private GameObject mLevelResistValueRoot = null;
        private Text mLevelResistValueLabel = null;
        private Text mLevelResistValueNumber = null;
        private Text mOwnerResistValueLabel = null;
        private Text mOwnerResistValueNumber = null;
        private ComCommonChapterInfo mChapterInfo = null;
        private Button mTeamStart = null;
        private Button mNormalStart = null;
        private UIGray mStartButton = null;
        private UIGray mTeamButton = null;
        private GameObject mHellroot0 = null;
        private GameObject mHellroot1 = null;
        private GameObject mYg0 = null;
        private GameObject mYg1 = null;
        private GameObject mBosschallengeRoot = null;
        private GameObject mComsumeRoot = null;
        private GameObject mStartRoot = null;
        private GameObject mBindTicketRoot = null;
        private GameObject mTicketRoot = null;
        private LinkParse mMissionContent = null;
        private GameObject mFatigueRoot = null;
        private Text mMissionInfo = null;
        private GameObject mMissionInfoRoot = null;
        private ComChapterDungeonUnit mDungeonUnitInfo = null;
        private Button mOnSelectLeftButton = null;
        private Button mOnSelectRightButton = null;
        private Button mOnReward = null;
        private GameObject mOnRewardRed = null;
        private Text mChapterRewardCount = null;
        private Text mChapterRewardSum = null;
        private Button mOnClose = null;
        private GameObject mMask = null;
        private GameObject[] mHards = new GameObject[4];
        private GameObject mRedPoint = null;
        private Text mRedPointSum = null;
        private GameObject mRewardComplete = null;
        private GameObject mFatigueCombustionRoot = null;
        private Button mGroupDrug;
        private Toggle mCheckDrug;
        private Button mDropButton = null;
        private GameObject mDropButtonEffect = null;
        private GameObject mDropProgress = null;
        private Button mStrategySkillsBtn = null;
        private GameObject StartContinueRoot = null;
        private Button StartContinue = null;
        private Text mNormalDiffTxt;
        private Button mDropDetailBtn;
        private GameObject mNotCostFatigue = null;
        private Toggle mBtNotCostFatigue = null;
        private GameObject mEliteDungeonTipRoot = null;

        //春节地下城
        private GameObject mSpringFestivalRoot = null;
        private Text mRoleNumTxt;
        private Text mAccountNumTxt;
        private GameObject mStartBtnEffectGo;
        private GameObject mMoneiesGo;

        private ComPlayAniList mComPlayAniList;

        protected override void _bindExUI()
        {
            mlevelChallengeTimesRoot = mBind.GetGameObject("ChallengeTimesRoot");
            mLevelChallengeTimesNumber = mBind.GetCom<Text>("ChallengeTimesValue");
            mRebornLimitNumberValue = mBind.GetCom<Text>("RebornLimitNumberValue");
            mLevelResistValueRoot = mBind.GetGameObject("LevelResistValueRoot");
            mLevelResistValueLabel = mBind.GetCom<Text>("LevelResistValueLabel");
            mLevelResistValueNumber = mBind.GetCom<Text>("LevelResistValueNumber");
            mOwnerResistValueLabel = mBind.GetCom<Text>("OwnerResistValueLabel");
            mOwnerResistValueNumber = mBind.GetCom<Text>("OwnerResistValueNumber");
            mChapterInfo = mBind.GetCom<ComCommonChapterInfo>("chapterInfo");
            mTeamStart = mBind.GetCom<Button>("teamStart");
            mTeamStart.onClick.AddListener(_onTeamStartButtonClick);
            mNormalStart = mBind.GetCom<Button>("normalStart");
            mNormalStart.onClick.AddListener(_onNormalStartButtonClick);
            mStartButton = mBind.GetCom<UIGray>("startButton");
            mTeamButton = mBind.GetCom<UIGray>("teamButton");
            mHellroot0 = mBind.GetGameObject("hellroot0");
            mHellroot1 = mBind.GetGameObject("hellroot1");
            mYg0 = mBind.GetGameObject("yg0");
            mYg1 = mBind.GetGameObject("yg1");
            mBosschallengeRoot = mBind.GetGameObject("bosschallengeRoot");
            mComsumeRoot = mBind.GetGameObject("comsumeRoot");
            mStartRoot = mBind.GetGameObject("startRoot");
            mBindTicketRoot = mBind.GetGameObject("bindTicketRoot");
            mTicketRoot = mBind.GetGameObject("ticketRoot");
            mMissionContent = mBind.GetCom<LinkParse>("missionContent");
            mFatigueRoot = mBind.GetGameObject("fatigueRoot");
            mMissionInfo = mBind.GetCom<Text>("missionInfo");
            mMissionInfoRoot = mBind.GetGameObject("missionInfoRoot");
            mDungeonUnitInfo = mBind.GetCom<ComChapterDungeonUnit>("dungeonUnitInfo");
            mOnSelectLeftButton = mBind.GetCom<Button>("onSelectLeftButton");
            mOnSelectLeftButton.onClick.AddListener(_onOnSelectLeftButtonButtonClick);
            mOnSelectRightButton = mBind.GetCom<Button>("onSelectRightButton");
            mOnSelectRightButton.onClick.AddListener(_onOnSelectRightButtonButtonClick);
            mOnReward = mBind.GetCom<Button>("onReward");
            mOnReward.onClick.AddListener(_onOnRewardButtonClick);
            mOnRewardRed = mBind.GetGameObject("onRewardRed");
            mChapterRewardCount = mBind.GetCom<Text>("chapterRewardCount");
            mChapterRewardSum = mBind.GetCom<Text>("chapterRewardSum");
            mOnClose = mBind.GetCom<Button>("onClose");
            mOnClose.onClick.AddListener(_onOnCloseButtonClick);
            mMask = mBind.GetGameObject("mask");

            mHards[0] = mBind.GetGameObject("hard0");
            mHards[1] = mBind.GetGameObject("hard1");
            mHards[2] = mBind.GetGameObject("hard2");
            mHards[3] = mBind.GetGameObject("hard3");

            mRedPoint = mBind.GetGameObject("RedPoint");
            mRedPointSum = mBind.GetCom<Text>("RedPointSum");
            mRewardComplete = mBind.GetGameObject("RewardComplete");
            mFatigueCombustionRoot = mBind.GetGameObject("FatigueCombustionRoot");

            mGroupDrug = mBind.GetCom<Button>("GroupDrug");
            mGroupDrug.onClick.AddListener(_onSetDrugBtnClick);
            mCheckDrug = mBind.GetCom<Toggle>("CheckDrug");
            mCheckDrug.onValueChanged.AddListener(delegate {
                _onCheckDrugToggleChanged();
            });
            
            mDropButton = mBind.GetCom<Button>("DropButton");
            if (null != mDropButton)
            {
                mDropButton.onClick.AddListener(_onDropButtonClick);
            }
            mDropButtonEffect = mBind.GetGameObject("DropButtonEffect");
            mDropProgress = mBind.GetGameObject("DropProgress");
            mStrategySkillsBtn = mBind.GetCom<Button>("StrategySkills");
            if (mStrategySkillsBtn != null)
            {
                mStrategySkillsBtn.onClick.AddListener(_onmStrategySkillsBtnClick);
            }
            StartContinueRoot = mBind.GetGameObject("StartContinueRoot");
            StartContinue = mBind.GetCom<Button>("StartContinue");
            StartContinue.SafeSetOnClickListener(_onNormalStartButtonClick);

            mNormalDiffTxt = mBind.GetCom<Text>("NormallDiff");

            mDropDetailBtn = mBind.GetCom<Button>("DropDetailBtn");
            mDropDetailBtn.SafeAddOnClickListener(_OnDropDetailBtnClick);

            mNotCostFatigue = mBind.GetGameObject("NotCostFatigue");
            mBtNotCostFatigue = mBind.GetCom<Toggle>("btNotCostFatigue");
            mBtNotCostFatigue.SafeSetOnValueChangedListener((value) => 
            {
                if (bToggleInit)
                {
                    bToggleInit = false;
                    return;
                }

                if (value)
                {
                    if (TeamDataManager.GetInstance().GetMemberNum() == 0)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("elite_dungeon_can_not_set_toggle_state_with_one_player"));
                        mBtNotCostFatigue.SafeSetToggleOnState(false);
                    }
                    else
                    {
                        LoginToggleMsgBoxOKCancelFrame.TryShowMsgBox(LoginToggleMsgBoxOKCancelFrame.LoginToggleMsgType.NotCostFatigue,
                        TR.Value("elite_dungeon_not_cost_fatigu_have_no_award"),
                        () =>
                        {
                            TeamDataManager.GetInstance().SendSceneSaveOptionsReq(SaveOptionMask.SOM_NOT_COUSUME_EBERGY, true);
                        },
                        () =>
                        {
                            mBtNotCostFatigue.SafeSetToggleOnState(false);
                        });
                    }
                }
                else
                {            
                    TeamDataManager.GetInstance().SendSceneSaveOptionsReq(SaveOptionMask.SOM_NOT_COUSUME_EBERGY, false);
                }
            });

            mEliteDungeonTipRoot = mBind.GetGameObject("EliteDungeonTipRoot");

            mSpringFestivalRoot = mBind.GetGameObject("SpringFestivalDungeonRoot");
            mRoleNumTxt = mBind.GetCom<Text>("RoleNumTxt");
            mAccountNumTxt = mBind.GetCom<Text>("AccountNumTxt");
            mStartBtnEffectGo = mBind.GetGameObject("EffUI_qixijiemian_lingquanniu");
            mMoneiesGo = mBind.GetGameObject("Moneies");

            mComPlayAniList = mBind.GetCom<ComPlayAniList>("ChapterNormalHalf");
        }

     

        protected override void _unbindExUI()
        {
            mlevelChallengeTimesRoot = null;
            mRebornLimitNumberValue = null;
            mLevelChallengeTimesNumber = null;
            mLevelResistValueRoot = null;
            mLevelResistValueLabel = null;
            mLevelResistValueNumber = null;
            mOwnerResistValueLabel = null;
            mOwnerResistValueLabel = null;
            mChapterInfo = null;
            mTeamStart.onClick.RemoveListener(_onTeamStartButtonClick);
            mTeamStart = null;
            mNormalStart.onClick.RemoveListener(_onNormalStartButtonClick);
            mNormalStart = null;
            mStartButton = null;
            mTeamButton = null;
            mHellroot0 = null;
            mHellroot1 = null;
            mYg0 = null;
            mYg1 = null;
            mBosschallengeRoot = null;
            mComsumeRoot = null;
            mStartRoot = null;
            mBindTicketRoot = null;
            mTicketRoot = null;
            mMissionContent = null;
            mFatigueRoot = null;
            mMissionInfo = null;
            mMissionInfoRoot = null;
            mDungeonUnitInfo = null;
            mOnSelectLeftButton.onClick.RemoveListener(_onOnSelectLeftButtonButtonClick);
            mOnSelectLeftButton = null;
            mOnSelectRightButton.onClick.RemoveListener(_onOnSelectRightButtonButtonClick);
            mOnSelectRightButton = null;
            mOnReward.onClick.RemoveListener(_onOnRewardButtonClick);
            mOnReward = null;
            mOnRewardRed = null;
            mChapterRewardCount = null;
            mChapterRewardSum = null;
            mOnClose.onClick.RemoveListener(_onOnCloseButtonClick);
            mOnClose = null;
            mMask = null;

            mHards[0] = null;
            mHards[1] = null;
            mHards[2] = null;
            mHards[3] = null;

            mRedPoint = null;
            mRedPointSum = null;
            mRewardComplete = null;
            mFatigueCombustionRoot = null;

            mGroupDrug.onClick.RemoveListener(_onSetDrugBtnClick);
            mCheckDrug.onValueChanged.RemoveAllListeners(); 
            mGroupDrug = null;
            mCheckDrug = null;
            
            if (null != mDropButton)
            {
                mDropButton.onClick.RemoveListener(_onDropButtonClick);
            }
            mDropButton = null;
            mDropButtonEffect = null;
            mDropProgress = null;

            if (mStrategySkillsBtn != null)
            {
                mStrategySkillsBtn.onClick.RemoveListener(_onmStrategySkillsBtnClick);
            }
            mStrategySkillsBtn = null;
            StartContinueRoot = null;
            StartContinue = null;

            mNormalDiffTxt = null;

            mDropDetailBtn.SafeRemoveOnClickListener(_OnDropDetailBtnClick);
            mDropDetailBtn = null;

            mNotCostFatigue = null;   
            mBtNotCostFatigue = null;
            mEliteDungeonTipRoot = null;

            mSpringFestivalRoot =  null;
            mRoleNumTxt =  null;
            mAccountNumTxt = null;
            mStartBtnEffectGo = null;
            mMoneiesGo = null;
            mComPlayAniList = null;
        }

        private void _bindEvents()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BuffDrugSettingSubmit, _onBuffDrugSettingSubmit);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CounterChanged, _onUpdateChapterDropProgress);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UpdateGameOptions, _onUpdateGameOptions);


            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivtiyLimitTimeAccounterNumUpdate,_OnUpdateAccounterNum);
        }

      
        private void _unBindEvents()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BuffDrugSettingSubmit, _onBuffDrugSettingSubmit);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CounterChanged, _onUpdateChapterDropProgress);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UpdateGameOptions, _onUpdateGameOptions);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivtiyLimitTimeAccounterNumUpdate, _OnUpdateAccounterNum);
        }

        private void _onBuffDrugSettingSubmit(UIEvent ui)
        {
            if(null == mCheckDrug)
            {
                return;
            }
            mCheckDrug.isOn = true;
            if(null != mChapterInfo)
            {
                mChapterInfo.SetBuffDrugsInfo(mDungeonTable.BuffDrugConfig);
                mChapterInfo.UpDateCost(mCheckDrug.isOn, mDungeonID);
            }
        }

        private void _onUpdateChapterDropProgress(UIEvent ui)
        {
            if (mDungeonID == null)
                return;

            UpdateLevelChallengeTimes(mDungeonID.dungeonID);
            UpdateDropProgress(mDungeonID.dungeonID);
        }

        private void _onUpdateGameOptions(UIEvent ui)
        {
            _UpdateFatigueConsume();
            bToggleInit = true;
            UpdateEliteNotCostFatigueUI();
        }

        #endregion

        #region Callback

        private void _onSetDrugBtnClick()
        {
            if (null != mChapterInfoDrugs)
            {
                var dungeonId = ChapterBaseFrame.sDungeonID;
                ClientSystemManager.instance.OpenFrame<ChapterDrugSettingFrame>(GameClient.FrameLayer.Middle, dungeonId);
            }
        }

        private void _onCheckDrugToggleChanged()
        {
            if(null != mCheckDrug)
            {
                if (mCheckDrug.isOn)
                {
                    ChapterBuffDrugManager.GetInstance().ResetBuffDrugsFromLocal(mDungeonTable.BuffDrugConfig);
                }
                else
                {
                    ChapterBuffDrugManager.GetInstance().ResetAllMarkedBuffDrugs();
                }
            }
            if(null != mChapterInfo)
            {
                mChapterInfo.UpDateCost(mCheckDrug.isOn, mDungeonID);
            }
        }

        private void _onTeamStartButtonClick()
        {
            /* put your code in here */
            if (!_getTeamBattleIsLock())
            {
                _onTeamButton();
            }
            else if (_isTeamBattleLevelLimte())
            {


            }
            else 
            {
                SystemNotifyManager.SystemNotify(3050);
            }
        }

        private void _onNormalStartButtonClick()
        {
            /* put your code in here */

            if(TeamUtility.IsEliteDungeonID(mDungeonID.dungeonID))
            {
                if (TeamDataManager.GetInstance().HasTeam())
                {
                    _onStartButton();
                }
                else
                {
                    LoginToggleMsgBoxOKCancelFrame.TryShowMsgBox(LoginToggleMsgBoxOKCancelFrame.LoginToggleMsgType.EnterEliteDungeonTip,
                        TR.Value("elite_dungeon_need_team"),
                        () =>
                        {
                            _onStartButton();
                        },
                        () =>
                        {
                            _onTeamButton();
                        },
                        TR.Value("elite_dungeon_need_team_ok_content"),
                        TR.Value("elite_dungeon_need_team_cancel_content"),
                        true);
                }
            }
            else
            {
                _onStartButton();
            }
        }

        private void _onHelpButtonClick()
        {
            /* put your code in here */

            //ClientSystemManager.instance.OpenFrame<HelpFrame>(GameClient.FrameLayer.TopMost
        }

        private void _onOnSelectLeftButtonButtonClick()
        {
            /* put your code in here */

            _onLeft();
        }
        private void _onOnSelectRightButtonButtonClick()
        {
            /* put your code in here */

            _onRight();
        }

        private void _onOnRewardButtonClick()
        {
            /* put your code in here */

            if(sDungeonID==mSpringFestivalDungeonId)
            {
                ClientSystemManager.GetInstance().OpenFrame<LimitTimeActivityFrame>(FrameLayer.Middle, 22880);
            }
            else
            {
                if (ChapterSelectFrame.IsCurrentSelectChapterShowReward())
                {

                    int curChapterIdx = ChapterSelectFrame.GetCurrentSelectChapter();
                    ClientSystemManager.instance.OpenFrame<ChapterRewardFrame>(FrameLayer.Middle, curChapterIdx);
                }
            }
          
        }

        private void _onDropButtonClick()
        {
            var frame = ClientSystemManager.instance.OpenFrame<ChapterDropProgressFrame>() as ChapterDropProgressFrame;
            frame.SetData(mDungeonID.dungeonID, mAreaIndex);
        }

        private void _onOnCloseButtonClick()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChapterNormalHalfFrameClose); 
        }

        private void _onmStrategySkillsBtnClick()
        {
            var mDungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(mDungeonID.dungeonIDWithOutDiff);
            if (mDungeonTable == null)
            {
                return;
            }

            ClientSystemManager.GetInstance().OpenFrame<CheckPointHelpFrame>(FrameLayer.Middle, mDungeonTable.PlayingDescription);
        }
#endregion


        protected sealed override void _loadBg()
        {
            // keep empty
        }

        protected sealed override void _OnOpenFrame()
        {
            bToggleInit = true;

            base._OnOpenFrame();
            _bindEvents();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChapterNormalHalfFrameOpen);

            if (mComPlayAniList != null)
            {
                if (userData != null && userData is bool && (bool)userData)     //如果是点左右按钮切换关卡，则不播放动画，直接打开界面则播放
                {
                    mComPlayAniList.FinishAnis(0);
                }
                else
                {
                    mComPlayAniList.PlayAnis(0);
                }
            }

            mTeamStart.CustomActive(Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Team));
            ChapterBuffDrugManager.GetInstance().ResetAllMarkedBuffDrugs();
            if (ChapterBuffDrugManager.GetInstance().IsBuffDrugToggleOn())
            {
                mCheckDrug.isOn = true;
                ChapterBuffDrugManager.GetInstance().ResetBuffDrugsFromLocal(mDungeonTable.BuffDrugConfig);
            }
            if (null != mChapterInfo) 
            {
                mChapterInfo.UpDateCost(mCheckDrug.isOn, mDungeonID);
            }

            _updateRewardRedPoint();
            _updateDungeonRewardStatus();

            MissionManager.GetInstance().onUpdateMission += _onUpdateMission;
            if (ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager != null)
                ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.AddSyncTaskDataChangeListener(_OnTaskChange);
            _updateLeftRightSwitchButtonStatus();

            SetMask(false);

            mFatigueCombustionRoot.CustomActive(false);
            if (BIsShowFatigueCombustionGameobject())
            {
                _InitFatigueCombustionGameObject(mFatigueCombustionRoot);
            }

            InitStrategySkillsBtn();
            SetAnniversayDungeonData();

            UpdateEliteNotCostFatigueUI();
            SetSpringFestivalData();
        }


        private void InitStrategySkillsBtn()
        {
            bool isFlag = StrategySkillsBtnIsShow();
            mStrategySkillsBtn.CustomActive(isFlag);
        }

        /// <summary>
        /// 攻略技巧按钮是否显示
        /// </summary>
        /// <returns></returns>
        private bool StrategySkillsBtnIsShow()
        {
            var mDungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(mDungeonID.dungeonIDWithOutDiff);
            if (mDungeonTable == null)
            {
                return false;
            }
            
            if (mDungeonTable.PlayingDescription == "" || mDungeonTable.PlayingDescription == null)
            {
                return false;
            }

            return true;
        }

        private bool BIsShowFatigueCombustionGameobject()
        {
            var mDungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(mDungeonID.dungeonID);
            if (mDungeonTable != null)
            {
                // 精英地下城不显示精力燃烧相关UI
                if(mDungeonTable.ThreeType == DungeonTable.eThreeType.T_T_TEAM_ELITE)
                {
                    return false;
                }

                if (mDungeonTable.SubType == DungeonTable.eSubType.S_NORMAL ||
                    mDungeonTable.SubType == DungeonTable.eSubType.S_WUDAOHUI)
                {
                    return true;
                }
            }

            return false;
        }

        private void _updateLeftRightSwitchButtonStatus()
        {
            ChapterSelectFrame chapterSelect = ClientSystemManager.instance.GetFrame(typeof(ChapterSelectFrame)) as ChapterSelectFrame;

            if (null != chapterSelect)
            {
                if (!chapterSelect.IsCanSelectLeftDungeon())
                {
                    mOnSelectLeftButton.CustomActive(false);
                    //UIGray gray = mOnSelectLeftButton.gameObject.SafeAddComponent<UIGray>(false);
                    //gray.enabled = true;
                }

                if (!chapterSelect.IsCanSelectRightDungeon())
                {
                    mOnSelectRightButton.CustomActive(false);
                    //UIGray gray = mOnSelectRightButton.gameObject.SafeAddComponent<UIGray>(false);
                    //gray.enabled = true;
                }
            }
        }

        protected sealed override void _OnCloseFrame()
        {
            bToggleInit = false;

            _unBindEvents();
            MissionManager.GetInstance().onUpdateMission -= _onUpdateMission;
            if (ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager != null)
                ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.RemoveSyncTaskDataChangeListener(_OnTaskChange);
            if (mCheckDrug.isOn)
            {
                ChapterBuffDrugManager.GetInstance().SetBuffDrugToggleState(true);
            }
            else
            {
                ChapterBuffDrugManager.GetInstance().SetBuffDrugToggleState(false);
            }
        }
        public sealed override bool IsNeedUpdate()
        {
            return true;
        }
        protected sealed override void _OnUpdate(float timeElapsed)
        {
            if (BIsShowFatigueCombustionGameobject())
            {
                _SetFatigueCombustionTime();
            }
        }
        private void _onUpdateMission(UInt32 taskID)
        {
            _updateRewardRedPoint();
        }

        private void _updateRewardRedPoint()
        {
            int curChapterIdx = ChapterSelectFrame.GetCurrentSelectChapter();

            bool isShow = ChapterUtility.IsChapterCanGetChapterReward(curChapterIdx);
            mOnRewardRed.CustomActive(isShow);

            KeyValuePair<int, int> rate = ChapterUtility.GetChapterRewardByChapterIdx(curChapterIdx);
            int CanGetSum = ChapterUtility.GetChapterCanGetByChapterIdx(curChapterIdx);

            //宝箱的四种状态
            if (CanGetSum == 0 && rate.Key == 0)
            {
                mOnRewardRed.CustomActive(false);
                mRewardComplete.CustomActive(false);
            }
            else if (CanGetSum != 0)
            {
                mOnRewardRed.CustomActive(false);
                mRedPoint.CustomActive(true);
                _updateRedPointNum(CanGetSum);
                mRewardComplete.CustomActive(false);
            }
            else if (CanGetSum == 0 && rate.Key > 0 && isShow)
            {
                mOnRewardRed.CustomActive(true);
                mRedPoint.CustomActive(false);
                mRewardComplete.CustomActive(false);
            }
            else if (CanGetSum == 0 && rate.Key == rate.Value && !isShow)
            {
                mOnRewardRed.CustomActive(false);
                mRedPoint.CustomActive(false);
                mRewardComplete.CustomActive(true);
            }
            else
            {
                mOnRewardRed.CustomActive(false);
                mRedPoint.CustomActive(false);
                mRewardComplete.CustomActive(false);
            }


            mChapterRewardCount.text = rate.Key.ToString();
            mChapterRewardSum.text = rate.Value.ToString();

            if(sDungeonID==mSpringFestivalDungeonId)
            {
                mOnRewardRed.CustomActive(false);
                mRedPoint.CustomActive(false);
                mRewardComplete.CustomActive(false);
            }
        }

        private void _updateDungeonRewardStatus()
        {
            mOnReward.gameObject.SetActive(ChapterSelectFrame.IsCurrentSelectChapterShowReward());
        }

        private void _updateRedPointNum(int RedPointSum)
        {
            mRedPointSum.text = RedPointSum.ToString();
        }

        private void _updateDefaultDiffSelected()
        {
            mDungeonID.dungeonID = sDungeonID;
			int diff = ChapterUtility.GetMissionDungeonDiff (mDungeonID.dungeonID);
			if (diff >= 0) 
			{
				mDungeonID.diffID = diff;
			}

            for (int i = 0; i < 4; ++i)
            {
                mHards[i].SetActive(diff == i);
            }

			sDungeonID = mDungeonID.dungeonID;

            mChapterInfoDiffculte.SetDiffculte(mDungeonID.diffID, mDungeonID.dungeonID);
        }

        protected override void _loadLeftPanel()
        {
            GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.EndNewbieGuideCover);         
            if (null != mChapterInfo)
            {
                var com = mChapterInfo;
                mChapterInfoCommon    = com;
                mChapterInfoDiffculte = com;
                mChapterInfoDrops     = com;
                mChapterPassReward    = com;
                mChapterScore         = com;
                mChapterProcess       = com;
                mChapterInfoDrugs     = com;
                mChapterDungeonMap    = com;
                mChapterNodeState     = com;
                mChapterConsume       = com;
                mChapterMask          = com;
                //mChapterMonsterInfo   = com;
                mChapterInfo.SetBuffDrugsInfo(mDungeonTable.BuffDrugConfig);
            }

            _updateDefaultDiffSelected();

            _updateTeamBattleLockState();


            List<eChapterNodeState> nodeState = new List<eChapterNodeState>();
            List<int> nodeLimitLevel = new List<int>();

            List<Protocol.DungeonScore> nodeScore = new List<Protocol.DungeonScore>();
            List<string> nodeRL = new List<string>();

            int curTopHard = ChapterUtility.GetDungeonTopHard(mDungeonID.dungeonIDWithOutDiff);

            DungeonID id = new DungeonID(mDungeonID.dungeonID);
            for (int i = 0; i <= curTopHard; ++i)
            {
                id.diffID = i;

                nodeScore.Add(ChapterUtility.GetDungeonBestScore(id.dungeonID));

                var table = TableManager.instance.GetTableItem<DungeonTable>(id.dungeonID);
                if (null != table)
                {
                    nodeRL.Add(table.RecommendLevel);
                    nodeLimitLevel.Add(table.MinLevel);
                }
                else 
                {
                    nodeRL.Add("");
                    nodeLimitLevel.Add(0);
                }

                ComChapterDungeonUnit.eState state = ChapterUtility.GetDungeonState(id.dungeonID);
                switch (state)
                {
                    case ComChapterDungeonUnit.eState.Locked:
                        nodeState.Add(eChapterNodeState.Lock);
                        break;
                    case ComChapterDungeonUnit.eState.LockPassed:
                    case ComChapterDungeonUnit.eState.Passed:
                        nodeState.Add(eChapterNodeState.Passed);
                        break;
                    case ComChapterDungeonUnit.eState.Unlock:
                        if (PlayerBaseData.GetInstance().Level >= nodeLimitLevel[i]) 
                        {
                            nodeState.Add(eChapterNodeState.Unlock);
                        } 
                        else 
                        { 
                            nodeState.Add(eChapterNodeState.LockLevel); 
                        }
                        break;
                }
            }

            for (int i = curTopHard +1; i < 4; ++i)
            {
                nodeState.Add(eChapterNodeState.Miss);
            }

            if (null != mChapterInfoCommon)
            {
                mChapterInfoCommon.SetRecommnedLevel(nodeRL.ToArray());
            }

            if (null != mChapterNodeState)
            {
                mChapterNodeState.SetChapterScore(nodeScore.ToArray());
                mChapterNodeState.SetChapterState(nodeState.ToArray(), nodeLimitLevel.ToArray());
            }
            
            //if (null != mChapterInfoDrugs)
            //{
            //    mChapterInfoDrugs.SetBuffDrugs(mDungeonTable.BuffDrugConfig);
            //}

            //if(NewbieGuideManager.GetInstance().IsGuidingControl() && !NewbieGuideManager.GetInstance().IsGuidingTask(NewbieGuideTable.eNewbieGuideTask.GuanKaGuide))
            //{
            //    //_OnCloseFrame();
            //}
            //
            //
            _updateDungeonMissionInfo();
            _updateDungeonInfo();
            if (null != mChapterMask)
            {
                mChapterMask.SetChapterMask(id.dungeonID);
            }
        }

        private void _updateDungeonMissionInfo()
        {
            mMissionInfoRoot.SetActive(false);

            if (ChapterUtility.GetDungeonMissionState(mDungeonID.dungeonID))
            {
                int missionID = (int)ChapterUtility.GetMissionIDByDungeonID(mDungeonID.dungeonID);

                mMissionInfoRoot.SetActive(true);

                mMissionInfo.text = ChapterUtility.GetDungeonMissionInfo(mDungeonID.dungeonID);
				mDungeonUnitInfo.SetType(ChapterUtility.GetMissionType(missionID));
                mMissionContent.SetText(Utility.ParseMissionText(missionID, true), true);
            }
        }

        private void _updateDungeonInfo()
        {
            if (null != mDungeonTable)
            {
                //Sprite sp = AssetLoader.instance.LoadRes(mDungeonTable.TumbPath, typeof(Sprite)).obj as Sprite;
                //mDungeonUnitInfo.SetBackgroud(sp);
                mDungeonUnitInfo.SetBackgroud(mDungeonTable.TumbPath);

                // Sprite chsp = AssetLoader.instance.LoadRes(mDungeonTable.TumbChPath, typeof(Sprite)).obj as Sprite;
                // mDungeonUnitInfo.SetCharactorSprite(chsp);
                mDungeonUnitInfo.SetCharactorSprite(mDungeonTable.TumbChPath);

                mDungeonUnitInfo.SetDungeonID(mDungeonTable.ID);
                mDungeonUnitInfo.SetName(mDungeonTable.Name, mDungeonTable.RecommendLevel);
                mDungeonUnitInfo.SetState(ChapterUtility.GetDungeonState(mDungeonID.dungeonID));

                mChapterMask.SetBarState(mDungeonID.diffID);
                if (mDungeonID.prestoryID > 0)
                {
                    mDungeonUnitInfo.SetDungeonType(ComChapterDungeonUnit.eDungeonType.Prestory);
                }
                else
                {
                    mDungeonUnitInfo.SetDungeonType(ComChapterDungeonUnit.eDungeonType.Normal);
                }

                ChapterSelectFrame chapterSelectFrame = ClientSystemManager.instance.GetFrame(typeof(ChapterSelectFrame)) as ChapterSelectFrame;
                if (chapterSelectFrame != null)
                {
                    if(chapterSelectFrame._GetChapterIndex() == 31)
                    {
                        mDungeonUnitInfo.SetEffect("Effects/UI/Prefab/EffUI_Yijie/Prefab/Eff_UI_YiJie_fangjian");
                    }
                }
            }
        }

        protected override void _loadRightPanel()
        {
            // keep empty
        }

        protected override void _updateDiffculteInfo()
        {
            base._updateDiffculteInfo();

            mChapterInfoDiffculte.SetDiffculteCallback(_onDiffSelected);
            mDungeonID.dungeonID = sDungeonID;
            _onDiffSelected(mDungeonID.diffID);
        }


        /// <summary>
        /// 组队是否解锁
        /// </summary>
        private bool _getTeamBattleIsLock()
        {
            ComChapterDungeonUnit.eState state = ChapterUtility.GetDungeonState(mDungeonID.dungeonID);
            bool isLock = state == ComChapterDungeonUnit.eState.Locked;

            return !ChapterUtility.IsTeamDungeonID(mDungeonID.dungeonID) || isLock || _isTeamBattleLevelLimte();
        }

        private bool _isTeamBattleLevelLimte()
        {
            return !Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Team);
        }

        private void _updateTeamBattleLockState()
        {
            mTeamButton.enabled = _getTeamBattleIsLock();
        }

        private void _setConsumeMode(DungeonTable.eSubType subType)
        {
            //mYg0.CustomActive(false);
            //mYg1.CustomActive(false);
            //mHellroot0.CustomActive(false);
            //mHellroot1.CustomActive(false);
            mStartRoot.SetActive(true);
            mBosschallengeRoot.SetActive(false);
            mComsumeRoot.SetActive(true);
            //mTicketRoot.SetActive(true);
            //mBindTicketRoot.SetActive(true);
            //mFatigueRoot.SetActive(true);

            switch (subType)
            {
                case DungeonTable.eSubType.S_HELL:
                case DungeonTable.eSubType.S_HELL_ENTRY:
                case DungeonTable.eSubType.S_ANNIVERSARY_HARD:
                    //mHellroot0.CustomActive(true);
                    //mHellroot1.CustomActive(true);
                    //mTicketRoot.SetActive(false);
                    //mBindTicketRoot.SetActive(false);
                    break;
                case DungeonTable.eSubType.S_YUANGU:
                    //mYg0.CustomActive(true);
                    //mYg1.CustomActive(true);
                    //mTicketRoot.SetActive(false);
                    //mBindTicketRoot.SetActive(false);
                    break;
                case DungeonTable.eSubType.S_TEAM_BOSS:
                    mBosschallengeRoot.SetActive(true);
                    //mFatigueRoot.SetActive(false);
                    mComsumeRoot.SetActive(false);
                    mStartRoot.SetActive(false);
                    break;
                case DungeonTable.eSubType.S_TREASUREMAP:
                    mComsumeRoot.SetActive(false);
                    break;
                default:
                    break;
            }
        }

        private void _onDiffSelected(int idx)
        {
            Logger.LogProcessFormat("[NormalFrame]选择难度 {0}", idx);

            mDungeonID.diffID = idx;
            int id = mDungeonID.dungeonID;

            _loadTableData();
            _updateScore();
            _updateTeamBattleLockState();
            _updateDungeonMissionInfo();
            _updateDungeonInfo();
            

            var state = ChapterUtility.GetDungeonState(id);

            bool isLock = state == ComChapterDungeonUnit.eState.Locked;
            mChapterInfoDiffculte.SetLock(isLock);
            mChapterInfoDiffculte.SetDiffculte(mChapterInfoDiffculte.GetDiffculte(),mDungeonID.dungeonID);

            mStartButton.enabled = isLock;

            //int topHard = ChapterUtility.GetDungeonTopHard(id);
            //for (int i = 0; i <= topHard; ++i)
            //{
            //    mChapterInfoDiffculte.SetActiveDiffculteByIdx(i, i == idx);
            //}

            bool isPass = state == ComChapterDungeonUnit.eState.Passed;
            mChapterScore.SetPassed(isPass);
            if (isPass)
            {
                mChapterScore.SetBestScore(ChapterUtility.GetDungeonBestScore(id));
            }

            _setConsumeMode(mDungeonTable.SubType);

            if (null != mChapterConsume)
            {
                mChapterConsume.SetFatigueConsume(
                        mDungeonTable.CostFatiguePerArea * mDungeonTable.MostCostStamina,
                        mDungeonTable.SubType == ProtoTable.DungeonTable.eSubType.S_HELL_ENTRY, mDungeonTable.ID);

                var data = TableManager.instance.GetTableItem<ItemTable>(mDungeonTable.TicketID);

                if (null != data)
                {
                    // Sprite sp = AssetLoader.instance.LoadRes(data.Icon, typeof(Sprite)).obj as Sprite;
                    // mChapterConsume.SetHellConsume(data.Name, mDungeonTable.TicketNum, sp, mDungeonTable.SubType == DungeonTable.eSubType.S_HELL);
                    mChapterConsume.SetHellConsume(data.Name, mDungeonTable.TicketNum, data.Icon, mDungeonTable.SubType == DungeonTable.eSubType.S_HELL);
                }
                else 
                {
                    mChapterConsume.SetHellConsume("", 0, null, mDungeonTable.SubType == DungeonTable.eSubType.S_HELL);
                }
            }

            if (null != mChapterInfoDrops)
            {
                mChapterInfoDrops.SetDropList(mDungeonTable.DropItems,mDungeonTable.ID);
            }

            UpdateLevelResistValue(id);
            UpdateLevelChallengeTimes(id);
            UpdateDropProgress(id);  
            // 异界关卡 正在挑战的关卡显示 继续挑战按钮 功能和开始挑战是一样的          
            if (IsYiJieDungeon(mDungeonID.dungeonID))
            {
                bool isInChallenge = IsCurrentDungeonInChallenge();
                bool bShow = !isLock && isInChallenge;
                StartContinueRoot.CustomActive(bShow);
                mStartRoot.CustomActive(!bShow);
            }
        }

        //更新关卡的抗魔值
        private void UpdateLevelResistValue(int dungeonId)
        {
            var dungeonResistValue = DungeonUtility.GetDungeonResistMagicValueById(dungeonId);
            if (dungeonResistValue <= 0)
            {
                //关卡不存在抗魔值，不显示，直接返回
                mLevelResistValueRoot.gameObject.CustomActive(false);
            }
            else
            {
                //关卡存在抗魔值，显示抗魔值信息，以及根据情况显示抗魔值不足的提示

                mLevelResistValueRoot.gameObject.CustomActive(true);
                mLevelResistValueNumber.text = dungeonResistValue.ToString();

                //总的抗魔值
                var ownerMagicValue = DungeonUtility.GetDungeonMainPlayerResistMagicValue();
                //buff增加的抗魔值
                var magicValueByBuff = BeUtility.GetMainPlayerResistAddByBuff();
                //不存在buff加成的抗魔值
                if (magicValueByBuff == 0)
                {
                    if (dungeonResistValue > ownerMagicValue)
                    {
                        mOwnerResistValueNumber.text = string.Format(TR.Value("resist_magic_value_less"),
                            ownerMagicValue);
                    }
                    else
                    {
                        mOwnerResistValueNumber.text = string.Format(TR.Value("resist_magic_value_normal"),
                            ownerMagicValue);
                    }
                }
                else
                {
                    //存在buff加成的抗魔值
                    var baseMagicValue = ownerMagicValue - magicValueByBuff;
                    if (dungeonResistValue > baseMagicValue)
                    {
                        mOwnerResistValueNumber.text = string.Format(TR.Value("resist_magic_value_add_buff_less"),
                            baseMagicValue,magicValueByBuff);
                    }
                    else
                    {
                        mOwnerResistValueNumber.text = string.Format(TR.Value("resist_magic_value_add_buff_normal"),
                            baseMagicValue, magicValueByBuff);
                    }
                }

                //只有组队的时候才可能显示侵蚀抗性不足的提示
                if (TeamDataManager.GetInstance().HasTeam() == true)
                {
                    DungeonUtility.ShowResistMagicValueTips(dungeonResistValue);
                }

                //if (dungeonResistValue > ownerMagicValue)
                //{
                //    //自身的抗魔值低于关卡抗魔值，直接提示(无论组队与否）
                //    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("resist_magic_less_tip"));
                //    return;
                //}
                //else
                //{
                //    //自身抗魔值大于关卡抗魔值，检测队伍中是否存在抗魔值较低的队员，如果存在，提示
                //    DungeonUtility.ShowResistMagicValueTips(dungeonResistValue);
                //    return;
                //}
            }
        }

        private int _getLeftTimes()
        {
            var finishedTimes = DungeonUtility.GetDungeonDailyFinishedTimes(mDungeonID.dungeonID);
            var dailyMaxTime = DungeonUtility.GetDungeonDailyMaxTimes(mDungeonID.dungeonID);
            var leftTimes = dailyMaxTime - finishedTimes;
            if (leftTimes < 0)
                leftTimes = 0;

            return leftTimes;
        }

        private void UpdateLevelChallengeTimes(int dungeonId)
        {
            if(mlevelChallengeTimesRoot == null)
                return;

            var dungeonDailyBaseTimes = DungeonUtility.GetDungeonDailyBaseTimes(dungeonId);
            if (dungeonDailyBaseTimes <= 0)
            {
                mlevelChallengeTimesRoot.CustomActive(false);
            }
            else
            {
                mlevelChallengeTimesRoot.CustomActive(true);
                if (mLevelChallengeTimesNumber != null)
                {
                    var leftTimes = _getLeftTimes();
                    mLevelChallengeTimesNumber.text = string.Format(TR.Value("resist_magic_challenge_times"),
                        leftTimes, dungeonDailyBaseTimes);
                }

                if (mRebornLimitNumberValue != null)
                {
                    mRebornLimitNumberValue.text = DungeonUtility.GetDungeonRebornNumber(dungeonId);
                }
            }
            if(sDungeonID==mSpringFestivalDungeonId)
            {
                mlevelChallengeTimesRoot.CustomActive(false);
            }
        }

        void UpdateEliteNotCostFatigueUI()
        {
            mNotCostFatigue.CustomActive(TeamUtility.IsEliteDungeonID(mDungeonID.dungeonID));
            mBtNotCostFatigue.SafeSetToggleOnState(TeamDataManager.GetInstance().IsNotCostFatigueInEliteDungeon);
            bToggleInit = false;
            mBtNotCostFatigue.SafeSetGray(TeamDataManager.GetInstance().GetMemberNum() == 0, false);

            mEliteDungeonTipRoot.CustomActive(TeamUtility.IsEliteDungeonID(mDungeonID.dungeonID));
        }

        private void UpdateDropProgress(int dungeonId)
        {
            if (mDropProgress == null)
                return;

            var dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonId);
            if (dungeonTable == null)
                return;

            if (dungeonTable.SubType != DungeonTable.eSubType.S_DEVILDDOM)
            {
                mDropProgress.CustomActive(false);
            }
            else
            {
                mDropProgress.CustomActive(true);
                GameFrameWork.instance.StartCoroutine(_onWorldDungeonGetAreaIndex());
            }
        }
        
        private IEnumerator _onWorldDungeonGetAreaIndex()
        {
            var req = new WorldDungeonGetAreaIndexReq();
            req.dungeonId = (uint)mDungeonID.dungeonID;
            var res = new WorldDungeonGetAreaIndexRes();
            var msg = new MessageEvents();

            yield return MessageUtility.Wait(ServerType.GATE_SERVER, msg, req, res);

            if (msg.IsAllMessageReceived())
            {
                mAreaIndex = res.areaIndex >> 1;
                mDropButtonEffect.CustomActive(mAreaIndex > 0);
            }
        }

        private void _onTeamButton()
        {
            var mDiff = 0;
            if (null != mChapterInfoDiffculte)
            {
                mDiff = mChapterInfoDiffculte.GetDiffculte();
            }

            mDungeonID.dungeonID = sDungeonID;
            mDungeonID.diffID = mDiff;

            Utility.OpenTeamFrame(mDungeonID.dungeonID);
        }

        public static bool IsYiJieDungeon(int dungeonID)
        {
            var mDungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonID);
            return (mDungeonTable != null && mDungeonTable.SubType == DungeonTable.eSubType.S_DEVILDDOM);
        }
        public static bool IsCurrentDungeonInChallenge()
        {
            DungeonID dungeonID = new DungeonID(sDungeonID);
            if (dungeonID != null)
            {
                dungeonID.diffID = 0;
                bool isInChallenge = ChapterSelectFrame.IsInChallenge(dungeonID.dungeonID);
                return isInChallenge;
            }
            return false;
        }
        private void _onStartButton()
        {
            if (mDropProgress != null && mDropProgress.activeSelf && _getLeftTimes() <= 0)
            {
                do
                {
                    if (IsYiJieDungeon(mDungeonID.dungeonID) && IsCurrentDungeonInChallenge())
                    {
                        break;
                    }
                _usePassItem();
                return;
                }
                while (false);                
            }

            if (TeamDataManager.GetInstance().HasTeam())
            {
                var mDiff = 0;
                if (null != mChapterInfoDiffculte)
                {
                    mDiff = mChapterInfoDiffculte.GetDiffculte();
                }

                mDungeonID.dungeonID = sDungeonID;
                mDungeonID.diffID = mDiff;

                if (TeamDataManager.GetInstance().IsTeamLeader())
                {
                    // 是否是组队副本
                    int iTeamDungeonTableID = 0;
                    if(!Utility.CheckIsTeamDungeon(mDungeonID.dungeonID, ref iTeamDungeonTableID))
                    {
                        SystemNotifyManager.SystemNotify(1106);
                        return;
                    }

                    // 各种条件判断
                    if (!Utility.CheckTeamEnterDungeonCondition(iTeamDungeonTableID))
                    {
                        return;
                    }
                }
                else
                {
                    SystemNotifyManager.SystemNotify(1105);
                    return;
                }              
            }

            #region LevelResistMagicValue
            //关卡中存在侵蚀抗性，并且自身或者队员的侵蚀抗性不足，显示提示弹窗。否则不显示           
            var isShowResistMagicTip = false;
            var resistMagicTipContent = string.Empty;
            isShowResistMagicTip =
                DungeonUtility.IsShowDungeonResistMagicValueTip(mDungeonID.dungeonID, ref resistMagicTipContent);

            //显示抗魔值不足的提示
            if (isShowResistMagicTip == true)
            {
                var state = ChapterUtility.GetDungeonState(mDungeonID.dungeonID);
                var isLock = state == ComChapterDungeonUnit.eState.Locked;

                //关卡锁住不显示，关卡解锁，则显示提示弹框，并结束
                if (isLock == false)
                {
                    SystemNotifyManager.SysNotifyMsgBoxCancelOk(resistMagicTipContent, null,
                        MessageBoxOKCallBack);
                    return;
                }
            }
            #endregion

			//显示配置技能的提示框
            var isShowSkillConfigTipFrame =
                SkillDataManager.GetInstance().IsShowSkillTreeFrameTipBySkillConfig(MessageBoxOKCallBack);
            if(isShowSkillConfigTipFrame == true)
                return;

            ChapterUtility.OpenComsumeFatigueAddFrame(sDungeonID);

            GameFrameWork.instance.StartCoroutine(_commonStart());
        }

        private void MessageBoxOKCallBack()
        {
            ChapterUtility.OpenComsumeFatigueAddFrame(sDungeonID);
            GameFrameWork.instance.StartCoroutine(_commonStart());
        }

        private void _onLeft()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChapterNextDungeon, true);
        }

        private void _onRight()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChapterNextDungeon, false);
        }

        private void _usePassItem()
        {
            bool useFlag = false;
            int[] itemIdArray = new int[] { 800000798, 330000200, 330000194 };//虚空通行证
            for (int i = 0; i < itemIdArray.Length; i++)
            {
                var itemId = itemIdArray[i];
                if (ItemDataManager.GetInstance().GetOwnedItemCount(itemId) >= 1)
                {
                    var item = ItemDataManager.GetInstance().GetItemByTableID(itemId);
                    if (item != null)
                    {
                        if (item.GetCurrentRemainUseTime() <= 0)
                            useFlag = true;
                        else
                        {
                            SystemNotifyManager.SysNotifyMsgBoxOkCancel(TR.Value("drop_progress_challenge_times_not_enough_has_item"), () =>
                            {
                                ItemDataManager.GetInstance().UseItemWithoutDoubleCheck(item);
                            });
                            return;
                        }
                    }
                }
            }
            if (useFlag)
                SystemNotifyManager.SystemNotify(1226);
            else
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("guild_redpacket_has_no_cost_time"));
        }

        public void SetMask(bool enabled)
        {
            if(mMask!=null)
            {
                mMask.SetActive(enabled);
            }
          
        }
        
        /// <summary>
        /// 周年派对地下城设置按钮等位置
        /// </summary>
        private void SetAnniversayDungeonData()
        {
            if(mAniversaryPartyDungeonIdList[1]==sDungeonID)
            {
                // mStartButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(240, 64);
                // StartContinueRoot.GetComponent<RectTransform>().anchoredPosition = new Vector2(240, 64);
                // mTeamButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-240,64);
                // mComsumeRoot.GetComponent<RectTransform>().anchoredPosition = new Vector2(326.5f, - 153);
                // mlevelChallengeTimesRoot.GetComponent<RectTransform>().anchoredPosition = new Vector2(276f, -188);

            }
            // else if (mAniversaryPartyDungeonIdList[0] == sDungeonID)
            // {
            //     mStartButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(240, 64);
            //     StartContinueRoot.GetComponent<RectTransform>().anchoredPosition = new Vector2(240, 64);
            //     mTeamButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-240, 64);
            //     mComsumeRoot.GetComponent<RectTransform>().anchoredPosition = new Vector2(326.5f, -163);
            // }
            if(mAniversaryPartyDungeonIdList.Contains(sDungeonID))
            {
                mOnReward.CustomActive(false);
                mNormalDiffTxt.SafeSetText(TR.Value("AnniversaryParty_Normall_Diff_Des"));//唉。。。。
                mDropDetailBtn.CustomActive(true);
            }
            else
            {
                mDropDetailBtn.CustomActive(false);
            }
        }
        #region 春节地下城相关内容
        /// <summary>
        /// 设置春节地下城相关信息
        /// </summary>
        private void SetSpringFestivalData()
        {
            if(sDungeonID==mSpringFestivalDungeonId)
            {
                mSpringFestivalRoot.CustomActive(true);
                mNormalDiffTxt.SafeSetText(TR.Value("AnniversaryParty_Normall_Diff_Des"));
                mStartButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-2, 25);
                ActivityDataManager.GetInstance().SendSceneOpActivityGetCounterReq((int)mDungeonTable.SubType);


                mTeamStart.CustomActive(false);
                mComsumeRoot.CustomActive(false);
                mEliteDungeonTipRoot.CustomActive(false);
                mNotCostFatigue.CustomActive(false);
                mlevelChallengeTimesRoot.CustomActive(false);
                mOnSelectLeftButton.CustomActive(false);
                mOnSelectRightButton.CustomActive(false);
                mMoneiesGo.CustomActive(false);

                mStartButton.CustomActive(false);
            }
            else
            {
                mSpringFestivalRoot.CustomActive(false);
            }
        }

        private void _OnUpdateAccounterNum(UIEvent uiEvent)
        {
            if ((uint)uiEvent.Param1 == (int)mDungeonTable.SubType)
            {
                int roleLeftNum = DungeonUtility.GetDungeonDailyLeftTimes(sDungeonID);
                int maxRoleNum = DungeonUtility.GetDungeonDailyMaxTimes(sDungeonID);

                int accountLeftNum = 0;
                int totalAccountNum = 0;
                int accountHaveNum = (int)ActivityDataManager.GetInstance().GetActivityConunter((int)mDungeonTable.SubType);
                DungeonTimesTable dungeonTimesTable= TableManager.GetInstance().GetTableItem<DungeonTimesTable>((int)mDungeonTable.SubType);
                if(dungeonTimesTable!=null)
                {
                    totalAccountNum = dungeonTimesTable.AccountDailyTimesLimit;
                }
                else
                {
                    Logger.LogErrorFormat(string.Format("加载地下城次数表为null id=", (int)mDungeonTable.SubType));
                }
                if (accountHaveNum >= totalAccountNum)
                {
                    accountHaveNum = totalAccountNum;

                }
              
                accountLeftNum = totalAccountNum - accountHaveNum;
                //角色限制剩余的次数取账号限制和角色限制的最小值
                if(accountLeftNum<= roleLeftNum)
                {
                    roleLeftNum = accountLeftNum;
                }

                if(accountLeftNum<=0)
                {
                    accountLeftNum = 0;
                    mNormalStart.interactable = false;
                    mStartButton.enabled = true;
                    mStartBtnEffectGo.CustomActive(false);
                }
                else
                {
                    if (roleLeftNum <= 0)
                    {
                        roleLeftNum = 0;
                        mNormalStart.interactable = false;
                        mStartButton.enabled = true;
                        mStartBtnEffectGo.CustomActive(false);
                    }
                    else
                    {
                        mNormalStart.interactable = true;
                        mStartButton.enabled = false;
                        mStartBtnEffectGo.CustomActive(true);
                    }
                }
                mStartButton.CustomActive(true);
                mAccountNumTxt.SafeSetText(string.Format(TR.Value("AccountChallengeTimers_Tip", accountLeftNum, totalAccountNum)));
                mRoleNumTxt.SafeSetText(TR.Value("RoleChallengeTimers_Tip", roleLeftNum, maxRoleNum));
            }
        }
        
        #endregion
        /// <summary>
        /// 掉落详情点击
        /// </summary>
        private void _OnDropDetailBtnClick()
        {
            ChallengeUtility.OnOpenChallengeDropDetailFrame(sDungeonID);
        }
        #region 精力燃烧
        /// <summary>
        /// 初始化疲劳燃烧是否显示
        /// </summary>
        private void _InitFatigueCombustionGameObject(GameObject mFatigueCombustionRoot)
        {
            ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.FindFatigueCombustionActivityIsOpen(ref mBisFlag, ref data);

            if (mBisFlag == true && data != null)
            {
                mFatigueCombustionRoot.CustomActive(true);

                _InitFatigueCombustionInfo(mFatigueCombustionRoot, data);

            }
            else
            {
                mFatigueCombustionRoot.CustomActive(false);
            }

        }

        /// <summary>
        /// 初始化疲劳燃烧信息
        /// </summary>
        /// <param name="go"></param>
        /// <param name="activityData"></param>
        private void _InitFatigueCombustionInfo(GameObject go, ActivityLimitTime.ActivityLimitTimeData activityData)
        {
            ComCommonBind mBind = go.GetComponent<ComCommonBind>();
            if (mBind == null)
            {
                return;
            }
            var activityId = activityData.DataId;
            Text mTime = mBind.GetCom<Text>("Time");
            Button mOpen = mBind.GetCom<Button>("Open");
            Button mStop = mBind.GetCom<Button>("Stop");
            GameObject mOrdinaryName = mBind.GetGameObject("OrdinaryName");
            GameObject mSeniorName = mBind.GetGameObject("SeniorName");
            SetButtonGrayCD mCDGray = mBind.GetCom<SetButtonGrayCD>("CDGray");
            mOrdinaryName.CustomActive(false);
            mSeniorName.CustomActive(false);

            for (int i = 0; i < activityData.activityDetailDataList.Count; i++)
            {
                if (activityData.activityDetailDataList[i].ActivityDetailState == ActivityLimitTime.ActivityTaskState.Init||
                    activityData.activityDetailDataList[i].ActivityDetailState == ActivityLimitTime.ActivityTaskState.UnFinish)
                {
                    continue;
                }

                mData = activityData.activityDetailDataList[i];

                var mTaskId = mData.DataId;

                string mStrID = mTaskId.ToString();
                string mStr = mStrID.Substring(mStrID.Length - 1);
                int mIndex = 0;

                if (int.TryParse(mStr, out mIndex))
                {
                    if (mIndex == 1)
                    {
                        mOrdinaryName.CustomActive(true);
                        mSeniorName.CustomActive(false);
                    }
                    else
                    {
                        mSeniorName.CustomActive(true);
                        mOrdinaryName.CustomActive(false);
                    }
                }

                mOpen.onClick.RemoveAllListeners();
                mOpen.onClick.AddListener(() =>
                {
                    ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.RequestOnTakeActTask(activityId, mTaskId);
                    mCDGray.StartGrayCD();
                });

                mStop.onClick.RemoveAllListeners();
                mStop.onClick.AddListener(() =>
                {
                    ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.RequestOnTakeActTask(activityId, mTaskId);
                    mCDGray.StartGrayCD();
                });

                _UpdateFatigueCombustionData(go, mData);

                if (activityData.activityDetailDataList[i].ActivityDetailState != ActivityLimitTime.ActivityTaskState.Failed)
                return;
            }


        }

        /// <summary>
        /// 更新疲劳燃烧数据状态
        /// </summary>
        /// <param name="go"></param>
        /// <param name="activityData"></param>
        private void _UpdateFatigueCombustionData(GameObject go, ActivityLimitTime.ActivityLimitTimeDetailData activityData,bool isInit = true)
        {
            if (go == null || activityData == null)
            {
                return;
            }

            var mBind = go.GetComponent<ComCommonBind>();
            if (mBind == null)
            {
                return;
            }

            mTime = mBind.GetCom<Text>("Time");
            Button mOpen = mBind.GetCom<Button>("Open");
            Button mStop = mBind.GetCom<Button>("Stop");
            mOpen.CustomActive(false);
            mStop.CustomActive(false);
            //是否在燃烧
            mFatigueCombustionTimeIsOpen = false;
            switch (activityData.ActivityDetailState)
            {
                case ActivityLimitTime.ActivityTaskState.Init:
                case ActivityLimitTime.ActivityTaskState.UnFinish:
                    //mOpen.CustomActive(true);
                    //mTime.text = Function.GetLastsTimeStr(mData.DoneNum);
                    if (mChapterInfo != null)
                    {
                        mChapterInfo.mComChapterInfoDrop.RefreshFaFatigueCombustionBuff();
                    }

                    _UpdateFatigueConsume();
                    break;
                case ActivityLimitTime.ActivityTaskState.Finished:
                    mStop.CustomActive(true);
                    mFatigueCombustionTimeIsOpen = true;
                    mFatigueCombustionTime = mData.DoneNum;
                    if (null != mChapterInfoDrops)
                    {
                        mChapterInfoDrops.SetDropList(mDungeonTable.DropItems, mDungeonTable.ID);
                    }
                    _UpdateFatigueConsume();
                    break;
                case ActivityLimitTime.ActivityTaskState.Failed:
                    mTime.text = Function.GetLastsTimeStr(mData.DoneNum);
                    mOpen.CustomActive(true);

                    if (isInit)
                    {
                        if (null != mChapterInfoDrops)
                        {
                            mChapterInfoDrops.SetDropList(mDungeonTable.DropItems, mDungeonTable.ID);
                        }
                    }
                    else
                    {
                        if (mChapterInfo != null)
                        {
                            mChapterInfo.mComChapterInfoDrop.RefreshFaFatigueCombustionBuff();
                        }
                    }

                    _UpdateFatigueConsume();
                    break;
            }
        }

        private void _UpdateFatigueConsume()
        {
            if (null != mChapterConsume)
            {
                mChapterConsume.SetFatigueConsume(
                        mDungeonTable.CostFatiguePerArea * mDungeonTable.MostCostStamina,
                        mDungeonTable.SubType == ProtoTable.DungeonTable.eSubType.S_HELL_ENTRY, mDungeonTable.ID);
            }
        }

        /// <summary>
        /// 设置疲劳燃烧的时间
        /// </summary>
        private void _SetFatigueCombustionTime()
        {
            if (mFatigueCombustionTimeIsOpen && mTime != null)
            {
                if (mFatigueCombustionTime - (int)TimeManager.GetInstance().GetServerTime() > 0)
                {
                    mTime.text = Function.GetLastsTimeStr(mFatigueCombustionTime - (int)TimeManager.GetInstance().GetServerTime());
                }
                else
                {
                    mFatigueCombustionRoot.CustomActive(false);
                }
            }
        }

        /// <summary>
        /// 疲劳燃烧活动任务变化
        /// </summary>
        private void _OnTaskChange()
        {
            _UpdateFatigueCombustionData(mFatigueCombustionRoot, mData,false);
        }
        #endregion
    }
}
