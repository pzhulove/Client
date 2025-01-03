//using System;
//using System.Collections.Generic;
//using System.Collections;
/////////删除linq
//using System.Text;
//using Network;
//using Protocol;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using System.Reflection;
//using ActivityLimitTime;
//using ProtoTable;

//using DG.Tweening;
//using UnityEngine.EventSystems;

//namespace GameClient
//{
//    public partial class ClientSystemBattle : ClientSystem
//    {
//#region ExtraUIBind
//        private ComCountScript m_countScirpt = null;
//        private GameObject mTimerRoot = null;
//        private GameObject mLeftPKRoot = null;
//        private GameObject mRightPKRoot = null;
//        private static SimpleTimer mTimerController = null;
//        private GameObject mAutoFightRootObj = null;
//        private GameObject mChatRoot = null;
//        private GameObject mProfessionInfo = null;
//        private GameObject mPkResult = null;
//        private GameObject mBossBarRoot = null;
//		private Button mBtnWatchLeft = null;
//		private Button mBtnWatchRight = null;
//        private Button mPvp3v3Button = null;
//        private GameObject mPvp3v3RightPendingRoot = null;
//        private GameObject mPvp3v3LeftPendingRoot = null;
//        private GameObject mPvp3v3FightOn = null;
//        private GameObject mPvp3v3FightOff = null;
//        private GameObject mPvp3v3TipsRoot = null; //mBind.GetGameObject("pvp3v3TipsRoot");
//        private Button mPvp3v3MicBtn = null;
//        private Image mPvp3v3MicBtnBg = null;
//        private Image mPvp3v3MicBtnClose = null;
//        private Button mPvp3v3PlayerBtn = null;
//        private Image mPvp3v3PlayerBtnBg = null;
//        private Image mPvp3v3PlayerBtnClose = null;
//        private ComTimeLimitButton mPvp3v3TimeLimitButton = null;
//        private GameObject mTeamVoiceBtnGo = null;
//        private VoiceInputBtn mTeamVoiceBtn = null;
//        private Button mBtnChangeWeapon = null;
//        private Image weaponIcon = null;
//        private Image weaponBack = null;

//        private CDCoolDowm cdCoolDowm = null;

//        private GameObject comboItemContainer;
//        private GameObject skillComboGuide;
//        private GameObject skillComboItem;
//        private GameObject magicGuide01;
//        private GameObject magicGuide02;
//        private GameObject normalGuide;
//        private ComDungeonScore mFightingTimeRoot = null;
//        private GameObject mVanityBonusBuffConent = null;
//        private Image mVanityBonusBuffIcon = null;
//        private GameObject mRebornInfoRoot = null;
//        private Text mRebornDesc = null;
//		private Text mRobotInfo = null;
//        private RectTransform mSwitchEquips = null;
//        private CDCoolDowm mSwitchEquipCD = null;
//        private Image mSwitchEquipIcon = null;
//        private Button mSwitchEquipsBtn = null;

//        protected sealed override void _bindExUI()
//        {
//            m_countScirpt     = mBind.GetCom<ComCountScript>("_countScirpt");
//            mTimerRoot        = mBind.GetGameObject("timerRoot");
//            mLeftPKRoot       = mBind.GetGameObject("leftPKRoot");
//            mRightPKRoot      = mBind.GetGameObject("rightPKRoot");
//#if !SERVER_LOGIC 

//            mTimerController  = mBind.GetCom<SimpleTimer>("timerController");

// #else
//            mTimerController = new SimpleTimer();
// #endif

//            mAutoFightRootObj = mBind.GetGameObject("autoFightRootObj");
//            mChatRoot         = mBind.GetGameObject("chatRoot");
//            mProfessionInfo = mBind.GetGameObject("ProfessionInfo");
//            mPkResult         = mBind.GetGameObject("pkResult");
//            mBossBarRoot      = mBind.GetGameObject("bossBarRoot");

//			pingText = mBind.GetCom<Text>("pingText");

//			mBtnWatchLeft = mBind.GetCom<Button>("btnWatchLeft");
//			mBtnWatchLeft.onClick.AddListener(_onBtnWatchLeftButtonClick);
//            mBtnWatchLeft.CustomActive(false);
//            mBtnWatchRight = mBind.GetCom<Button>("btnWatchRight");
//			mBtnWatchRight.onClick.AddListener(_onBtnWatchRightButtonClick);
//            mBtnWatchRight.CustomActive(true);


//            mPvp3v3Button = mBind.GetCom<Button>("pvp3v3Button");
//            mPvp3v3Button.onClick.AddListener(_onPvp3v3ButtonButtonClick);
//            mPvp3v3RightPendingRoot = mBind.GetGameObject("pvp3v3RightPendingRoot");
//            mPvp3v3LeftPendingRoot = mBind.GetGameObject("pvp3v3LeftPendingRoot");

//            mPvp3v3FightOn = mBind.GetGameObject("pvp3v3FightOn");
//            mPvp3v3FightOff = mBind.GetGameObject("pvp3v3FightOff");
//            mPvp3v3TipsRoot = mBind.GetGameObject("pvp3v3TipsRoot");
//            mPvp3v3TimeLimitButton = mBind.GetCom<ComTimeLimitButton>("pvp3v3TimeLimitButton");

//            mPvp3v3MicBtn = mBind.GetCom<Button>("pvp3v3MicBtn");
//            mPvp3v3MicBtn.onClick.AddListener(_onPvp3v3MicBtnButtonClick);
//            mPvp3v3MicBtnBg = mBind.GetCom<Image>("pvp3v3MicBtnBg");
//            mPvp3v3MicBtnClose = mBind.GetCom<Image>("pvp3v3MicBtnClose");
//            mPvp3v3PlayerBtn = mBind.GetCom<Button>("pvp3v3PlayerBtn");
//            mPvp3v3PlayerBtn.onClick.AddListener(_onPvp3v3PlayerBtnButtonClick);
//            mPvp3v3PlayerBtnBg = mBind.GetCom<Image>("pvp3v3PlayerBtnBg");
//            mPvp3v3PlayerBtnClose = mBind.GetCom<Image>("pvp3v3PlayerBtnClose");
//            mTeamVoiceBtnGo = mBind.GetGameObject("teamVoiceBtnGo");
//            mTeamVoiceBtn = mBind.GetCom<VoiceInputBtn>("teamVoiceBtn");
//            weaponIcon = mBind.GetCom<Image>("weaponIcon");
//            weaponBack = mBind.GetCom<Image>("weaponBack");
//            cdCoolDowm = mBind.GetCom<CDCoolDowm>("cdCoolDown");
//            mBtnChangeWeapon = mBind.GetCom<Button>("btnChangeWeapon");
//            mBtnChangeWeapon.onClick.AddListener(_onBtnChangeWeaponButtonClick);
//            mSwitchEquipIcon = mBind.GetCom<Image>("SwitchEquipIcon");

//            comboItemContainer = mBind.GetGameObject("comboItemContainer");
//            skillComboItem = mBind.GetGameObject("skillComboItem");
//            magicGuide01 = mBind.GetGameObject("magicGuide01");
//            magicGuide02 = mBind.GetGameObject("magicGuide02");
//            normalGuide = mBind.GetGameObject("normalGuide");
//            skillComboGuide = mBind.GetGameObject("skillComboGuide");
//            mFightingTimeRoot = mBind.GetCom<ComDungeonScore>("FightingTimeRoot");
//            mVanityBonusBuffConent = mBind.GetGameObject("VanityBonusBuffConent");
//            mVanityBonusBuffIcon = mBind.GetCom<Image>("VanityBonusBuffIcon");
//            mRebornInfoRoot = mBind.GetGameObject("RebornInfo");
//            mSwitchEquips = mBind.GetCom<RectTransform>("SwitchEquips");
//            mSwitchEquipsBtn = mBind.GetCom<Button>("SwitchEquipsBtn");
//            mSwitchEquipsBtn.onClick.AddListener(_onSwitchEquipsBtnButtonClick);
//            mSwitchEquipCD = mBind.GetCom<CDCoolDowm>("SwitchEquipCD");
//            mRebornDesc = mBind.GetCom<Text>("RebornDesc");
//#if ROBOT_TEST
//            mRobotInfo = mBind.GetCom<Text>("RobotInfo");
//            if(!mRobotInfo.IsNull())
//            {
//                mRobotInfo.CustomActive(true);
//            }
//#endif
//        }

//        protected sealed override void _unbindExUI()
//        {
//			pingText = null;
//            m_countScirpt = null;
//            mTimerRoot = null;
//            mLeftPKRoot = null;
//            mRightPKRoot = null;
//            mTimerController = null;
//            mAutoFightRootObj = null;
//            mChatRoot = null;
//            mProfessionInfo = null;
//            mBossBarRoot = null;
//			mBtnWatchLeft.onClick.RemoveListener(_onBtnWatchLeftButtonClick);
//			mBtnWatchLeft = null;
//			mBtnWatchRight.onClick.RemoveListener(_onBtnWatchRightButtonClick);
//			mBtnWatchRight = null;
//            mPvp3v3Button.onClick.RemoveListener(_onPvp3v3ButtonButtonClick);
//            mPvp3v3Button = null;

//            mPvp3v3RightPendingRoot = null;
//            mPvp3v3LeftPendingRoot = null;

//            mPvp3v3FightOn  = null;
//            mPvp3v3FightOff = null;
//            mPvp3v3TipsRoot = null;
//            mPvp3v3TimeLimitButton = null;

//            mPvp3v3MicBtn.onClick.RemoveListener(_onPvp3v3MicBtnButtonClick);
//            mPvp3v3MicBtn = null;
//            mPvp3v3MicBtnBg = null;
//            mPvp3v3MicBtnClose = null;
//            mPvp3v3PlayerBtn.onClick.RemoveListener(_onPvp3v3PlayerBtnButtonClick);
//            mPvp3v3PlayerBtn = null;
//            mPvp3v3PlayerBtnBg = null;
//            mPvp3v3PlayerBtnClose = null;
//            mTeamVoiceBtnGo = null;
//            mTeamVoiceBtn = null;

//            mBtnChangeWeapon.onClick.RemoveListener(_onBtnChangeWeaponButtonClick);
//            mBtnChangeWeapon = null;
//            mSwitchEquipIcon = null;
//            mFightingTimeRoot = null;
//            mVanityBonusBuffConent = null;
//            mVanityBonusBuffIcon = null;
//            mRebornInfoRoot = null;
//            mSwitchEquips = null;
//            mSwitchEquipsBtn.onClick.RemoveListener(_onSwitchEquipsBtnButtonClick);
//            mSwitchEquipsBtn = null;
//            mSwitchEquipCD = null;
//            mRebornDesc = null;
//#if ROBOT_TEST
//            mRobotInfo = null;
//#endif

//        }
//        #endregion
//        public void SetRebornDescOpen(bool isActive)
//        {
//            if (mRebornInfoRoot != null)
//            {
//                mRebornInfoRoot.CustomActive(isActive);
//            }
//        }
//        public void RefreshRebornCount(int leftCount, int maxCount)
//        {
//            if (mRebornDesc != null)
//            {
//                mRebornDesc.text = string.Format("队伍复活次数 : {0}/{1}", leftCount, maxCount);
//            }
//        }

//        private void _onBtnWatchLeftButtonClick()
//		{
//			/* put your code in here */
//			mBtnWatchLeft.CustomActive(false);
//			mBtnWatchRight.CustomActive(true);

//			if (ReplayServer.GetInstance().IsReplay())
//				ReplayServer.GetInstance().SwitchWatchPlayer(true);

//		}
//		private void _onBtnWatchRightButtonClick()
//		{
//			/* put your code in here */
//			mBtnWatchRight.CustomActive(false);
//			mBtnWatchLeft.CustomActive(true);

//			if (ReplayServer.GetInstance().IsReplay())
//				ReplayServer.GetInstance().SwitchWatchPlayer(false);
//		}

//        private void _onPvp3v3ButtonButtonClick()
//        {
//            /* put your code in here */
//            if (null == BattleMain.instance)
//                return;
//            if (ReplayServer.GetInstance().IsReplay())
//                return;
//            BattlePlayer mainPlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer();

//            if (null == mainPlayer)
//            {
//                return ;
//            }

//            MatchRoundVote cmd = new MatchRoundVote
//            {
//                isVote = !mainPlayer.isVote
//            };
//            FrameSync.instance.FireFrameCommand(cmd);

//            Logger.LogProcessFormat("[战斗] 战斗中投票 是否要出战呢 : {0}", cmd.isVote);

//            _update3v3ApplyFightStatus();
//        }

//        private void _onPvp3v3MicBtnButtonClick()
//        {
//            /* put your code in here */
//            if (ReplayServer.GetInstance().IsReplay())
//                return;
//            OnVoiceSDKMicClick();
//        }
//        private void _onPvp3v3PlayerBtnButtonClick()
//        {
//            /* put your code in here */
//            if (ReplayServer.GetInstance().IsReplay())
//                return;
//            OnVoiceSDKPlayerClick();
//        }

//        private void _onBtnChangeWeaponButtonClick()
//        {
//            try
//            {
//                if (ReplayServer.GetInstance().IsReplay())
//                    return;

//                mBtnChangeWeapon.transform.parent.DOScale(1.3f, 0.1f).OnComplete(() =>
//                {
//                    mBtnChangeWeapon.transform.parent.DOScale(1.2f, 0.1f);
//                });
//                BeActor actor = BattleMain.instance.GetLocalPlayer().playerActor;

//                if (cdCoolDowm != null && cdCoolDowm.surplusTime > 0)
//                {
//                    SystemNotifyManager.SysNotifyFloatingEffect("切换武器冷却中");
//                    return;
//                }

//                if (actor != null && actor.isSpecialMonster)
//                {
//                    SystemNotifyManager.SysNotifyFloatingEffect("当前状态不可切换");
//                    return;
//                }

//                if (actor.IsCastingSkill())
//                {
//                    BeSkill skill = actor.GetCurrentSkill();
//                    if (skill != null)
//                    {
//                        if (skill.canSwitchWeapon)
//                        {
//                            ChangeWeapon(0);
//                        }
//                        else
//                        {
//                            SystemNotifyManager.SysNotifyFloatingEffect("该技能释放过程中无法更换武器");
//                            //  actor.m_pkGeActor.CreateHeadText(HitTextType.SKILL_CANNOTUSE, "UI/Font/new_font/pic_incd.png");
//                        }
//                    }

//                }
//                else
//                {
//                    ChangeWeapon(0);
//                }
//                AudioManager.instance.PlaySound(102);

//                GameStatisticManager.GetInstance().DoStartUIButton("BattleChangeWeapon");
//            }
//            catch(Exception e)
//            {
//                Logger.LogErrorFormat("_onBtnChangeWeaponButtonClick:{0}",e.Message);
//            }
//        }

//        //[UIControl("Text")]
//        static Text pingText;
//        [UIControl("TestInfo")]
//        Text testInfo;

//        [UIObject("DungeonMap/CountRoot/Button")]
//        GameObject mbuttonObject;

//        [UIObject("ArrowLeft")]
//        GameObject mArrowLeft;

//        [UIObject("ArrowRight")]
//        GameObject mArrowRight;

//        [UIObject("ArrowLeftGo")]
//        GameObject mArrowLeftGo;

//        [UIObject("ArrowRightGo")]
//        GameObject mArrowRightGo;

//        public static SimpleTimer TimerController { get { return mTimerController; } }

//        [UIObject("BlindMask")]
//        GameObject mBlindMask;

//        [UIControl("TipsBar", typeof(Image))]
//        private Image mTipsBar;

//        [UIControl("Exp", typeof(ComExpBar))]
//        private ComExpBar mExpBar;

//        [UIObject("PlayerInfos")]
//        GameObject mPlayerSelfInfo;
//        public GameObject PlayerSelfInfoRoot { get { return mPlayerSelfInfo; } }
//        public GameObject PlayerOtherInfoRoot { get { return mPlayerSelfInfo; } }
//        public GameObject PlayerPKLeftRoot { get { return mLeftPKRoot; } }
//        public GameObject PlayerPKRightRoot { get { return mRightPKRoot; } }

//        public GameObject MonsterBossRoot { get { return mBossBarRoot; } }

//        public GameObject Pvp3v3LeftHpBarRoot { get { return mBind.GetGameObject("pvp3v3TestHpLeftBarRoot"); } }

//        public GameObject Pvp3v3RightHpBarRoot { get { return mBind.GetGameObject("pvp3v3TestHpRightBarRoot"); ; } }


//        [UIControl("DungeonMap/RScore", typeof(ComDungeonScore))]
//        private ComDungeonScore mDungeonScore;

//        [UIObject("DungeonMap/TextRoot")]
//        GameObject goTextRoot;

//        [UIControl("DeadTower/Level", typeof(Text))]
//        Text textDeadTowerLevel;

//        [UIControl("ChampionMatch/Name", typeof(Text))]
//        Text textChampionMatch;

//        [UIControl("MuscleShift/Text", typeof(Text))]
//        Text textMuscleShift;

//        [UIControl("MuscleShift/CD", typeof(Image))]
//        Image imageMuscleShift;

//        [UIObject("MuscleShift/Icon")]
//        GameObject iconMuscleShift;

//        [UIControl("AutoFight", typeof(Toggle))]
//        Toggle autoFight;

//        #region PVP
//        [UIObject("PVPReplay")]
//        GameObject goReplay;

//        [UIControl("PVPReplay/buttonReturn", typeof(Button))]
//        Button btnReplayReturn;

//        [UIControl("PVPReplay/buttonPlaySpeed", typeof(Button))]
//        Button btnReplaySpeed;

//        [UIControl("PVPReplay/buttonPlaySpeed/Text", typeof(Text))]
//        Text txtSpeedDesc;

//        [UIControl("PVPReplay/buttonPause", typeof(Button))]
//        Button btnReplayPause;

//        [UIControl("PVPReplay/buttonResume", typeof(Button))]
//        Button btnReplayResume;

//        [UIObject("PVPTrain")]
//        GameObject goTrain;

//        [UIControl("PVPTrain/buttonReturn", typeof(Button))]
//        Button btnTrainReturn;

//        [UIControl("PVPTrain/buttonReset", typeof(Button))]
//        Button btnTrainReset;
//		#endregion


//        [UIControl("RedPacket")]
//        Button btnRedPacket;

//        [UIControl("RedPacket/Count")]
//        Text labRedPacketCount;

//        #region 测试功能

//        [UIObject("tmpButton1")]
//        GameObject goTmpBun1;

//        [UIObject("tmpButton2")]
//        GameObject goTmpBun2;

//        [UIEventHandle("tmpButton1")]
//        void TmpSetPosition()
//        {
//            //var players = BattleMain.instance.GetPlayerManager().GetAllPlayers();
//            //players[1].playerActor.SetPosition(players[0].playerActor.GetPosition());

//			//ShowPkResult(PKResult.WIN);
//        }

//        [UIEventHandle("tmpButton2")]
//        void TmpSetPosition2()
//        {
//            //var players = BattleMain.instance.GetPlayerManager().GetAllPlayers();
//            //players[2].playerActor.SetPosition(players[0].playerActor.GetPosition());
//			//ShowPkResult(PKResult.LOSE);
//        }

//#region 战斗场景 实时语音测试模块

//        [UIObject("DungeonMap/CountRoot/realMircoBtn")]
//        GameObject realMircoBtn;

//        [UIControl("DungeonMap/CountRoot/realMircoBtn/onOffImage",typeof(Image))]
//        Image onOffImage;

//        [UIObject("DungeonMap/CountRoot/realPlayerBtn")]
//        GameObject realPlayerBtn;

//        [UIControl("DungeonMap/CountRoot/realPlayerBtn/openCloseImage",typeof(Image))]
//        Image openCloseImage;



//        [UIEventHandle("DungeonMap/CountRoot/realMircoBtn")]
//        void RealMircoBtn()
//        {
//            //if (isFirstInRealMicBtn)
//            //{
//            //    isFirstInRealMicBtn = false;
//            //    return;
//            //}
//            OnVoiceSDKMicClick();
//        }

//        [UIEventHandle("DungeonMap/CountRoot/realPlayerBtn")]
//        void RealPlayerBtn()
//        {
//            //if (isFirstInRealPlayerBtn)
//            //{
//            //    isFirstInRealPlayerBtn = false;
//            //    return;
//            //}
//            OnVoiceSDKPlayerClick();
//        }

//        #region 武器替换
//        private void InitWeaponChange()
//        {
//            if (mBtnChangeWeapon == null||ReplayServer.GetInstance().IsReplay())
//                return;


//            var player = BattleMain.instance.GetPlayerManager().GetMainPlayer();
//            if (player != null)
//            {
//                if (player.playerActor == null) return;

//                if (player.playerActor.GetEntityData().CanChangeWeapon())
//                {
//                    if (BattleMain.battleType == BattleType.PVP3V3Battle)
//                    {
//                        mBtnChangeWeapon.transform.parent.gameObject.CustomActive(player.isFighting);
//                    }
//                    else
//                    {
//                        mBtnChangeWeapon.transform.parent.gameObject.CustomActive(true);
//                    }
//                    //bool isPVP = BattleMain.IsModePvP(BattleMain.battleType);
//                    //mBtnChangeWeapon.transform.parent.rectTransform().anchoredPosition = isPVP ? new Vector2(-85, 427) : new Vector2(-121, 574);
//                    ChangeWeaponIcon(player.playerActor.GetEntityData().GetBackupEquipItemID());
//                }
//            }
//        }

//        public void SetWeaponState(bool flag)
//        {
//            if (mBtnChangeWeapon == null || ReplayServer.GetInstance().IsReplay())
//                return;
//            SetSwitchEquipBtnState(flag);
//            var player = BattleMain.instance.GetPlayerManager().GetMainPlayer();
//            if (player != null)
//            {
//                if (player.playerActor == null) return;
//                if (flag)
//                {
//                    if (player.playerActor.GetEntityData().CanChangeWeapon())
//                    {
//                        if (BattleMain.battleType == BattleType.PVP3V3Battle)
//                        {
//                            mBtnChangeWeapon.transform.parent.gameObject.CustomActive(player.isFighting);
//                        }
//                        else
//                        {
//                            mBtnChangeWeapon.transform.parent.gameObject.CustomActive(true);
//                        }
//                    }

//                }
//                else
//                {
//                    mBtnChangeWeapon.transform.parent.gameObject.CustomActive(false);
//                }
//            }
//        }

//        public void ChangeWeaponIcon(int id)
//        {
//            if (mBtnChangeWeapon == null || id == 0)
//                return;
//            var data = ItemDataManager.GetInstance().GetItemByTableID(id);
//            if (data == null)
//                return;
//            ETCImageLoader.LoadSprite(ref weaponBack, data.GetQualityInfo().Background);
//            ETCImageLoader.LoadSprite(ref weaponIcon, data.Icon);
//        }

//        public void StartChangeWeaponCD(BeActor actor)
//        {
//            float surpusTime = Global.Settings.switchWeaponTime;
//            float reduceCD = 0;
//            Mechanism81 mechanism = actor.GetMechanism(5072) as Mechanism81;
//            if(mechanism!=null)
//                reduceCD = mechanism.changeWeaponCD.f;

//            Mechanism81 weaponMechanism = actor.GetMechanism(158) as Mechanism81;
//            if (weaponMechanism != null)
//                reduceCD += weaponMechanism.changeWeaponCD.f;

//            if (mechanism != null && cdCoolDowm != null)
//            {
//                cdCoolDowm.StartCD(surpusTime*(1+reduceCD));
//            }
//            else
//            {
//                cdCoolDowm.StartCD(Global.Settings.switchWeaponTime);
//            }
//        }

//        private void ChangeWeapon(int index)
//        {
//            var actor = BattleMain.instance.GetPlayerManager().GetMainPlayer();
//            if (null != actor && null != actor.playerActor)
//            {
//                StartChangeWeaponCD(actor.playerActor);
//                ChangeWeaponCommand cmd = new ChangeWeaponCommand();
//                cmd.weaponIndex = index;

//                FrameSync.instance.FireFrameCommand(cmd);
//            }
//        }

//        //重置切换武器CD
//        public void ResetChangeWeaponCD()
//        {
//            cdCoolDowm.ResetCD();
//        }

//        #endregion

//        // 入口 
//        private void TryInitVoiceChat(UIEvent uiEvent)
//        {
//            InitVoiceSDKBtns();
//            if (PluginManager.instance.OpenTalkRealVocie == false)
//                return;
//            InitVoiceModule();
//        }

//        private void UnInitVoiceButtons()
//        {
//            if (PluginManager.instance.OpenTalkRealVocie == false)
//                return;
//            if (!_isVoiceOpenInTeam() && !_isVoiceOpenIn3v3())
//                return;

//            //if (SDKVoiceManager.GetInstance().lastRealTalkVoiceScene == RealTalkVoiceScene.None)
//            //{
//                SDKVoiceManager.GetInstance().CloseRealMic();
//                SDKVoiceManager.GetInstance().CloseReaPlayer();
//            //}

//            SDKVoiceManager.GetInstance().RecoverGameVolumnInTalkVoice();
//            SDKVoiceManager.GetInstance().LeaveAllChannel();
//            SDKVoiceManager.GetInstance().RemoveRealVoiceHandler(OnJoinChannelSucc,
//                                                    OnVoiceSDKMicOn,
//                                                    OnVoiceSDKPlayerOn);
//        }

//        private void InitVoiceModule()
//        {
//            bool isVoiceOpenInTeam = _isVoiceOpenInTeam();

//            if (realMircoBtn)
//            {
//                realMircoBtn.CustomActive(isVoiceOpenInTeam);
//            }
//            if (realPlayerBtn)
//            {
//                realPlayerBtn.CustomActive(isVoiceOpenInTeam);
//            }

//            bool isVoiceOpen3v3 = _isVoiceOpenIn3v3();
//            if (mPvp3v3MicBtn != null)
//            {
//                mPvp3v3MicBtn.gameObject.CustomActive(isVoiceOpen3v3);
//            }
//            if (mPvp3v3PlayerBtn != null)
//            {
//                mPvp3v3PlayerBtn.gameObject.CustomActive(isVoiceOpen3v3);
//            }

//           if (!isVoiceOpenInTeam && !isVoiceOpen3v3)
//                return;

//            SDKVoiceManager.GetInstance().AddRealVoiceHandler(OnJoinChannelSucc,
//                                                    OnVoiceSDKMicOn,
//                                                    OnVoiceSDKPlayerOn);
//            string channelId = TryGetVoiceSDKChannalId();
//            if (string.IsNullOrEmpty(channelId))
//            {
//                Logger.LogProcessFormat("3v3 room create voice channel id is empty!");
//                return;
//            }
//            SDKVoiceManager.GetInstance().JoinChannel(channelId, PlayerBaseData.GetInstance().RoleID+"",
//                ClientApplication.playerinfo.openuid + "", ClientApplication.playerinfo.token + "");
//        }

//        private bool _isVoiceOpenInTeam()
//        {
//            if (null == BattleMain.instance)
//                return false;

//            if (BattleMain.instance.GetPlayerManager() == null || BattleMain.instance.GetPlayerManager().GetAllPlayers() == null)
//                return false;
//            bool bMoreThanOnePlayer = BattleMain.instance.GetPlayerManager ().GetAllPlayers ().Count > 1;
//            bool bModeInTeam         = BattleMain.IsTeamMode(BattleMain.battleType,BattleMain.mode);

//            if (PluginManager.GetInstance().OpenTalkRealInTeam == false && bModeInTeam)
//                return false;

//            return bModeInTeam && bMoreThanOnePlayer;
//        }

//        private bool _isVoiceOpenIn3v3()
//        {
//            if (null == BattleMain.instance)
//                return false;
//            if (ReplayServer.GetInstance().IsReplay())
//                return false;
//            if (BattleMain.instance.GetPlayerManager() == null)
//                return false;
//            bool bMoreThanOnePlayer = BattleMain.instance.GetPlayerManager().GetMainPlayerTeamPlayerCount() > 1;
//            bool bMode3v3         = BattleMain.IsModePVP3V3(BattleMain.battleType);

//            if (PluginManager.GetInstance().OpenTalkRealIn3v3Pvp == false && bMode3v3)
//                return false;

//            return bMoreThanOnePlayer && bMode3v3;
//        }

//        private void InitVoiceSDKBtns()
//        {
//            if (realMircoBtn)
//            {
//                realMircoBtn.CustomActive(false);
//            }
//            if (realPlayerBtn)
//            {
//                realPlayerBtn.CustomActive(false);
//            }

//            if (mPvp3v3MicBtn != null)
//            {
//                mPvp3v3MicBtn.gameObject.CustomActive(false);
//            }
//            if (mPvp3v3PlayerBtn != null)
//            {
//                mPvp3v3PlayerBtn.gameObject.CustomActive(false);
//            }
//        }

//        private string TryGetVoiceSDKChannalId()
//        {
//            uint groupType = 0;
//            if (BattleMain.IsModePVP3V3(BattleMain.battleType))
//            {
//                if (null != BattleMain.instance)
//                    groupType = (uint)BattleMain.instance.GetPlayerManager().GetMainPlayer().teamType;
//                else
//                    Logger.LogError("null == BattleMain.instance!!!!");
//            }
//            ulong channelId = ClientApplication.playerinfo.session * 10 + groupType;
//            return channelId + "";
//        }

//        void OnJoinChannelSucc()
//        {
//            SDKVoiceManager.GetInstance().ResetRealTalkVoiceParams();
//            SDKVoiceManager.GetInstance().CloseRealMic();
//            SDKVoiceManager.GetInstance().OpenRealPlayer();
//        }

//        void OnVoiceSDKMicClick()
//        {
//            //bool isMobileNetEnable = SDKVoiceManager.GetInstance().GetMobileNetworkEnabled();
//            //if (!isMobileNetEnable)
//            //{
//            //    return;
//            //}
//            if (SDKVoiceManager.GetInstance().IsRecordVoiceEnabled == false)
//            {
//                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("voice_sdk_record_not_enabled"));
//                return;
//            }
//            SDKVoiceManager.GetInstance().ControlRealVoiceMic();
//        }

//        void OnVoiceSDKPlayerClick()
//        {
//            //bool isMobileNetEnable = SDKVoiceManager.GetInstance().GetMobileNetworkEnabled();
//            //if (!isMobileNetEnable)
//            //{
//            //    return;
//            //}
//            if (SDKVoiceManager.GetInstance().IsPlayVoiceEnabled == false)
//            {
//                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("voice_sdk_play_not_enabled"));
//                return;
//            }
//            SDKVoiceManager.GetInstance().ControlRealVociePlayer();
//        }

//        void OnVoiceSDKMicOn(bool isOn)
//        {
//            //Logger.LogErrorFormat("SystemBattle - OnVoiceSDKMicOn - invoke {0}", isOn);

//            ChangeMicBtnStatus(isOn);
//            if (isOn)
//            {
//                SDKVoiceManager.GetInstance().CutGameVolumnInTalkVoice();
//            }
//        }

//        void OnVoiceSDKPlayerOn(bool isOn)
//        {
//            //Logger.LogErrorFormat("SystemBattle - OnVoiceSDKPlayerOn - invoke {0}",isOn);

//            ChangePlayerBtnStatus(isOn);
//            //SDKVoiceManager.GetInstance().ControlGameMusicVolumn(GetVoiceDeviceIsOn());
//        }

//        private bool GetVoiceDeviceIsOn()
//        {
//            bool isMicOn = SDKVoiceManager.GetInstance().IsTalkRealMicOn();
//            bool isPlayerOn = SDKVoiceManager.GetInstance().IsTalkRealPlayerOn();
//            bool isMicOrPlayerOn = isMicOn || isPlayerOn;
//            return isMicOrPlayerOn;
//        }

//        private void ChangeMicBtnStatus(bool isMicOn)
//        {
//            if(onOffImage != null)
//                onOffImage.gameObject.CustomActive(!isMicOn);


//            if (BattleMain.IsModePVP3V3(BattleMain.battleType))
//            {
//                if (mPvp3v3MicBtnClose != null)
//                {
//                    mPvp3v3MicBtnClose.gameObject.CustomActive(!isMicOn);
//                }
//                if (mPvp3v3MicBtnBg != null)
//                {
//                    mPvp3v3MicBtnBg.enabled = isMicOn;
//                }
//            }
//        }

//        private void ChangePlayerBtnStatus(bool isPlayerOpen)
//        {
//            if(openCloseImage != null)
//                openCloseImage.gameObject.CustomActive(!isPlayerOpen);


//            if (BattleMain.IsModePVP3V3(BattleMain.battleType))
//            {
//                if (mPvp3v3PlayerBtnClose != null)
//                {
//                    mPvp3v3PlayerBtnClose.gameObject.CustomActive(!isPlayerOpen);
//                }
//                if (mPvp3v3PlayerBtnBg != null)
//                {
//                    mPvp3v3PlayerBtnBg.enabled = isPlayerOpen;
//                }
//            }
//        }

//        #region 组队录音聊天

//        private VoiceChatModule voiceChatModule;

//        void InitTeamChatVoice()
//        {
//            SetVoiceInputBtnShow(false);

//            if (!PluginManager.instance.OpenChatVoiceInGloabl)
//                return;

//            if (IsInTeamMode() == false)
//            {
//                return;
//            }

//            SetVoiceInputBtnShow(true);

//            voiceChatModule = SDKVoiceManager.GetInstance().VoiceChatModule;
//            if (voiceChatModule != null)
//            {
//                voiceChatModule.BindRoot(mTeamVoiceBtnGo, VoiceInputType.ComtalkTeam);
//            }

//            if(mTeamVoiceBtnGo != null)
//            {
//                ComIntervalGroup.GetInstance().Register(this, (int)ChatType.CT_TEAM, Utility.FindComponent<ComFunctionInterval>(mTeamVoiceBtnGo, "VoiceSend"));
//            }
//        }

//        void UnInitTeamChatVoice()
//        {
//            if (!PluginManager.instance.OpenChatVoiceInGloabl)
//                return;

//            if (IsInTeamMode() == false)
//            {
//                return;
//            }

//            if (voiceChatModule != null)
//            {
//                voiceChatModule.UnBindRoot(VoiceInputType.ComtalkTeam, mTeamVoiceBtn);
//            }
//            ComIntervalGroup.GetInstance().UnRegister(this);
//        }

//        void SetVoiceInputBtnShow(bool isShow)
//        {
//            if (mTeamVoiceBtnGo)
//            {
//                mTeamVoiceBtnGo.CustomActive(isShow);
//            }
//            if (mTeamVoiceBtn)
//            {
//                mTeamVoiceBtn.gameObject.CustomActive(isShow);
//            }
//        }

//        bool IsInTeamMode()
//        {
//            if (null == BattleMain.instance)
//                return false;

//            if (BattleMain.instance.GetPlayerManager() == null || BattleMain.instance.GetPlayerManager().GetAllPlayers() == null)
//                return false;
//            bool bMoreThanOnePlayer = BattleMain.instance.GetPlayerManager().GetAllPlayers().Count > 1;
//            bool bModeInTeam = BattleMain.IsTeamMode(BattleMain.battleType, BattleMain.mode);
//            bool bMode3v3 = BattleMain.IsModePVP3V3(BattleMain.battleType);

//            if (bMode3v3)
//                return false;

//            return bModeInTeam && bMoreThanOnePlayer;
//        }

//        #endregion

//#endregion
//        #endregion

//        bool canTrainReset = true;

//        GameObject autoFightEffect = null;

//        UIGray muscleShiftGray;

//        ulong previousExp = 0;
//        int previousLevel = 0;

//        public BeActionManager simpleActionManager = new BeActionManager();
//        public DebugBattleStatisCompnent comDebugBattleStatis = new DebugBattleStatisCompnent();
//        public ShowHitComponent comShowHit = new ShowHitComponent();

//        private List<ClientFrame> childFrameList = new List<ClientFrame>(); //添加到战斗UI界面的子页面列表

//		public void ShowPkResult(PKResult result)
//		{
//            if (mPkResult == null) return;
//            mPkResult.CustomActive(true);

//			ComCommonBind pkBind = mPkResult.GetComponent<ComCommonBind>();

//            if (null == pkBind)
//            {
//                return ;
//            }

//            GameObject winObj = pkBind.GetGameObject("objWin");
//            GameObject loseObj = pkBind.GetGameObject("objLost");
//            GameObject drawObj = pkBind.GetGameObject("objDual");

//            winObj.CustomActive(false);
//            loseObj.CustomActive(false);
//            drawObj.CustomActive(false);

//            switch (result)
//            {
//                case PKResult.DRAW:
//                    drawObj.CustomActive(true);
//                    break;
//                case PKResult.LOSE:
//                    loseObj.CustomActive(true);
//                    break;
//                case PKResult.WIN:
//                    winObj.CustomActive(true);
//                    break;
//            }
//		}

//        public void HiddenPkResult()
//        {
//            if (null == mPkResult)
//            {
//                return ;
//            }

//            mPkResult.CustomActive(false);
//        }

//        public void ShowPVP3V3Tips()
//        {
//            if (null == mPvp3v3TipsRoot)
//            {
//                return ;
//            }

//            mPvp3v3TipsRoot.CustomActive(true);
//        }

//        public void HiddenPVP3V3Tips()
//        {
//            if (null == mPvp3v3TipsRoot)
//            {
//                return ;
//            }

//            mPvp3v3TipsRoot.CustomActive(false);
//        }

//        public void ShowLevelTip(int tip)
//        {
//            CloseLevelTip();

//            var data = TableManager.GetInstance().GetTableItem<ProtoTable.MonsterSpeech>(tip);
//            if (data != null)
//            {
//                NewbieGuideBattleTipsFrame frame = ClientSystemManager.GetInstance().OpenFrame<NewbieGuideBattleTipsFrame>(FrameLayer.Top) as NewbieGuideBattleTipsFrame;
//                frame.SetTipsText(data.Speech);
//            }
//        }

//        public void CloseLevelTip()
//        {
//            if (ClientSystemManager.GetInstance().IsFrameOpen<NewbieGuideBattleTipsFrame>())
//                ClientSystemManager.GetInstance().CloseFrame<NewbieGuideBattleTipsFrame>();
//        }

//        public void ShowDigit(bool flag)
//        {
//            if(m_countScirpt != null)
//                m_countScirpt.Show(flag);
//        }

//        public void SetCountDigit(int second)
//        {
//            if(m_countScirpt != null)
//                m_countScirpt.SetMTimeImage(second);
//        }
//#region 自动战斗
//        public void SetAutoFightVisible(bool flag)
//        {
//            if (autoFightEffect != null)
//            {
//                if (flag && autoFight != null && autoFight.isOn)
//                    autoFightEffect.CustomActive(flag);
//                else
//                    autoFightEffect.CustomActive(flag);
//            }
//        }

//        public void SetAutoFight(bool auto)
//        {
//            if (true/* || BattleMain.instance != null && BattleMain.battleType == BattleType.Dungeon*/)
//            {
//                if(null != BattleMain.instance)
//                {
// 					var actor = BattleMain.instance.GetPlayerManager().GetMainPlayer();
//                    if (null != actor && null != actor.playerActor)
//                    {
//                        AutoFightCommand cmd = new AutoFightCommand
//	                    {
//	                        openAutoFight = auto,
//	                        seat = actor.playerInfo.seat
//	                    };
//						FrameSync.instance.FireFrameCommand(cmd);
//                    }                    
//                }
//            }
//        }

//        public void LoadAutoFightConfig()
//        {
//			if (!Global.Settings.forceUseAutoFight)
//			{
//				if (!SwitchFunctionUtility.IsOpen(12) && BattleMain.IsModeMultiplayer(BattleMain.mode))
//					return ;
//			}
			
//            if (autoFight != null)
//            {
//                var savedAutoFight = PlayerLocalSetting.GetValue("AutoFight");
//                if (savedAutoFight != null)
//                {
//                    autoFight.isOn = (bool)savedAutoFight;

//                    if (autoFight.isOn)
//                    {
//                        ClientSystemManager.instance.delayCaller.DelayCall(10, () =>
//                                {
//                                SetAutoFight(false);
//                                StartCheckRestoreAutoFight();
//                                });
//                    }
//                    else
//                    {
//                        SetAutoFight(false);
//                    }
//                }
//            }
//        }

//        public void SaveAutoFightConfig(bool flag)
//        {
//            if (autoFight != null)
//            {
//                PlayerLocalSetting.SetValue("AutoFight", flag);
//                PlayerLocalSetting.SaveConfig();
//            }
//        }

//        public bool CanLevelOpenAutoFight()
//        {
//            return Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.AutoFight);
//        }

//        public bool CanOpenAutoFight()
//        {
//			#if DEBUG_SETTING
//            if (Global.Settings.forceUseAutoFight)
//                return true;

//            if (Global.Settings.startSystem == EClientSystem.Battle)
//                return true;
//			#endif

//			if (!CanLevelOpenAutoFight())
//				return false;

//            if (Global.Settings.canUseAutoFightFirstPass)
//			{
//				if (BattleMain.battleType == BattleType.Hell)
//					return ChapterUtility.HellIsPass((int)BattleDataManager.GetInstance().BattleInfo.dungeonId);
//				else
//					return ChapterUtility.GetDungeonState(BattleDataManager.GetInstance().BattleInfo.dungeonId)  == ComChapterDungeonUnit.eState.Passed;
//			}

//			return true;
//			//return CanLevelOpenAutoFight();//PlayerBaseData.GetInstance().Level >= Utility.GetFuncUnlockLevel(ProtoTable.FunctionUnLock.eFuncType.AutoFight);
//        }

//        public bool CanShowOpenAutoFight()
//        {
//			#if DEBUG_SETTING
//            if (Global.Settings.startSystem == EClientSystem.Battle)
//                return true;

//            if (Global.Settings.forceUseAutoFight)
//                return true;
//			#endif

//            if (NewbieGuideManager.GetInstance().GetNextGuide(NewbieGuideTable.eNewbieGuideType.NGT_WEAK) == NewbieGuideTable.eNewbieGuideTask.AutoFightGuide)
//            {
//                return false;
//            }

//            return Global.Settings.canUseAutoFight && ChapterUtility.CanDungeonOpenAutoFight(BattleDataManager.GetInstance().BattleInfo.dungeonId);
//        }

//        public void InitAutoFight()
//        {
//            if (!CanShowOpenAutoFight())
//                return;

//            if (!CanOpenAutoFight())
//            {
//                if (autoFight != null)
//                {
//                    autoFight.gameObject.CustomActive(true);
//                    autoFight.gameObject.SafeAddComponent<UIGray>();
//                    autoFight.onValueChanged.AddListener((bool choose) =>
//                    {
//                        string strNotify = "";

//                        if (!CanLevelOpenAutoFight())
//                        {
//                            strNotify = string.Format("{0}级开启自动战斗", Utility.GetFunctionUnlockLevel(FunctionUnLock.eFuncType.AutoFight));
//                        }
//                        else
//                        {
//                            strNotify = string.Format("首次通关后开启自动战斗");
//                        }

//                        SystemNotifyManager.SysNotifyTextAnimation(strNotify);
//                    });
//                }
                
//                return;
//            }

//            if (autoFight != null)
//            {
//                StopCheckRestoreAutoFight();

//                autoFight.gameObject.CustomActive(true);

//                autoFight.onValueChanged.AddListener((bool choose) => {
//					StopCheckRestoreAutoFight ();
//					SetAutoFight (choose);

//					if (choose) {
//						if (autoFightEffect == null) {
//							string res = "Effects/Scene_effects/EffectUI/EffUI_Autofight";
//							autoFightEffect = CGameObjectPool.instance.GetGameObject (res, enResourceType.UIPrefab, (uint)GameObjectPoolFlag.None);
//							Utility.AttachTo (autoFightEffect, autoFight.gameObject);
//						}
//					} else {
//						DeinitAutoFight ();
//					}

//					SaveAutoFightConfig (choose);
//				});

//                if (Global.Settings.loadAutoFight)
//                    LoadAutoFightConfig();


//                var player = BattleMain.instance.GetPlayerManager().GetMainPlayer().playerActor;
//                if (null != player)
//                {
//                    player.RegisterEvent(BeEventType.onMoveJoystick, (object[] args) =>
//                            {
//                                if (autoFight.isOn)
//                                    SetAutoFight(false);
//                            });

//                    player.RegisterEvent(BeEventType.onStopMoveJoystick, (object[] args) =>
//                            {
//                                if (autoFight.isOn)
//                                    StartCheckRestoreAutoFight();
//                            });

//                    player.RegisterEvent(BeEventType.onPassedDoor, (args) =>
//                    {
//                        if (autoFight.isOn)
//                        {
//                            StartCheckRestoreAutoFight();
//                        }
//                    });
//                    player.RegisterEvent(BeEventType.onStateChange, (object[] args) =>
//                    {
//                        ActionState state = (ActionState)args[0];
//                        if (state == ActionState.AS_IDLE && !player.IsInMoveDirection() && autoFight.isOn && player.pauseAI)
//                        {
//                            StartCheckRestoreAutoFight();
//                        }
//                    });
//                }
//            }
//        }

//        public void DeinitAutoFight()
//        {
//            if (autoFight != null && autoFightEffect != null)
//            {
//                CGameObjectPool.instance.RecycleGameObject(autoFightEffect);
//                autoFightEffect = null;
//            }
//        }

//        int autoFightAcc = 0;
//        int AUTOFIGHT_RESTORE_INTERVAL = 2000;
//        bool checkRestoreAutoFight = false;

//        public void StartCheckRestoreAutoFight()
//        {
//            checkRestoreAutoFight = true;
//            autoFightAcc = 0;
//        }

//        public void StopCheckRestoreAutoFight()
//        {
//            if (!checkRestoreAutoFight)
//                return;

//            checkRestoreAutoFight = false;
//        }

//        public void UpdateCheckRestoreAutoFight(int delta)
//        {
//            if (checkRestoreAutoFight)
//            {
//                autoFightAcc += delta;
//                if (autoFightAcc >= AUTOFIGHT_RESTORE_INTERVAL)
//                {
//                    StopCheckRestoreAutoFight();
//                    if (null == autoFight)
//                    {
//                        return;
//                    }

//                    BattlePlayer localPlayer = null;
//                    if(BattleMain.instance != null)
//                        localPlayer = BattleMain.instance.GetLocalPlayer();

//                    if (null == localPlayer ||
//                        null == localPlayer.playerActor)
//                    {
//                        return;
//                    }

//					if (autoFight.isOn && !localPlayer.playerActor.IsInMoveDirection())
//                    {
//                        SetAutoFight(true);                       
//                    }
//                }
//            }
//        }
//#endregion

//        public void InitForTrain()
//        {
//			canTrainReset = true;
//            if (goTrain != null)
//                goTrain.CustomActive(true);

//            StartTimer();

//            if (btnTrainReturn != null)
//            {
//                btnTrainReturn.onClick.AddListener(() =>
//                        {
//                            BeUtility.ResetCamera();
//                            ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
//                        });
//            }

//            if (btnTrainReset != null)
//            {
//                btnTrainReset.onClick.AddListener(() =>
//                        {
//                            var battle = BattleMain.instance.GetBattle() as TrainingBatte2;
//                            if (battle != null)
//                            {
//                                if (!canTrainReset)
//                                    return;
//                                if (BattleMain.instance.GetPlayerManager() == null) return;
//                                var node = BattleMain.instance.GetPlayerManager().GetMainPlayer();
//                                if (node != null)
//                                {
//                                    BeActor actor = node.playerActor;
//                                    BeSkill currSkill = actor.GetCurrentSkill();
//                                    if (currSkill != null)
//                                    {
//                                        currSkill.joystickMode = SkillJoystickMode.FREE;
//                                        currSkill.RemoveJoystickEffect();
//                                        currSkill.EndJoystick();
//                                        currSkill.Cancel();
//                                    }
//                                }
//                                canTrainReset = false;
//                                battle.RecreatePlayers();
//                                if (mTimerController != null)
//                                    mTimerController.Reset();
//                                var gray = btnTrainReset.GetComponent<UIGray>();
//                                if (gray != null)
//                                    gray.enabled = true;
//                                ClientSystemManager.GetInstance().delayCaller.DelayCall(5000, () =>
//                                {
//                                    canTrainReset = true;
//                                    if (btnTrainReset == null) return;
//                                    btnTrainReset.enabled = true;
//                                    if (gray != null)
//                                        gray.enabled = false;
//                                });
//                                BeUtility.ResetCamera();
//                                ResetCombo();
//                            }
//                            AudioManager.instance.StopAll(AudioType.AudioEffect);

//                        });
//            }
//        }

//        protected void ResetCombo()
//        {
//            if (Combo != null)
//            {
//                Combo.StopFeed();
//            }
//        }

//        public void InitForReplay()
//        {
//            if (!ReplayServer.GetInstance().IsReplay())
//                return;

//            if (goReplay != null)
//                goReplay.CustomActive(true);

//            if (btnReplayReturn != null)
//            {
//                btnReplayReturn.onClick.AddListener(() =>
//                        {
//                            if (ReplayServer.GetInstance().IsLiveShow())
//                            {
//                                if (Network.NetManager.instance != null)
//                                    Network.NetManager.instance.Disconnect(ServerType.RELAY_SERVER);
//                            }
//                            ReplayServer.GetInstance().Stop(false,"InitForReplay");
//                            ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
//                        });
//            }
//            if (btnReplaySpeed != null)
//            {
//                if (ReplayServer.GetInstance().IsLiveShow())
//                {
//                    txtSpeedDesc.text = "同步进度";
//                }
//                else
//                {
//                    txtSpeedDesc.text = string.Format("速度×{0}", ReplayServer.GetInstance().timeScaler);
//                }
//                btnReplaySpeed.onClick.AddListener(() =>
//                {
//                    if (ReplayServer.GetInstance().IsLiveShow())
//                    {
//                        ReplayServer.GetInstance().StartPersue();
//                        btnReplaySpeed.gameObject.SetActive(false);
//                    }
//                    else
//                    {
//                        ReplayServer.GetInstance().ScaleTime();
//                        txtSpeedDesc.text = string.Format("速度×{0}", ReplayServer.GetInstance().timeScaler);
//                    }
//                });
//            }

//            if (btnReplayResume != null && btnReplayPause != null)
//            {


//                if (ReplayServer.GetInstance().IsLiveShow())
//                {
//                    btnReplayResume.gameObject.CustomActive(false);
//                    btnReplayPause.gameObject.CustomActive(false);
//                }
//                else
//                {
//                    btnReplayPause.gameObject.CustomActive(true);
//                }
//                btnReplayPause.onClick.AddListener(() =>
//                        {
//                        btnReplayResume.gameObject.CustomActive(true);
//                        btnReplayPause.gameObject.CustomActive(false);

//                        ReplayServer.GetInstance().Pause();
//                        });

//                btnReplayResume.onClick.AddListener(() =>
//                        {
//                        btnReplayResume.gameObject.CustomActive(false);
//                        btnReplayPause.gameObject.CustomActive(true);

//                        ReplayServer.GetInstance().Resume();
//                        });

//                btnReplayResume.gameObject.CustomActive(false);
//            }
//		}
//		public void SetReplayVisible(bool flag)
//		{
//			if (goReplay != null)
//				goReplay.CustomActive(flag);
//		}

//        public void InitForDeadTower()
//        {
//            if (goTextRoot != null)
//                goTextRoot.CustomActive(false);
//            if (textDeadTowerLevel != null)
//            {
//                textDeadTowerLevel.transform.parent.gameObject.SetActive(true);
//            }
//        }

//        public void SetFloor(int floor)
//        {
//            if (textDeadTowerLevel != null)
//                textDeadTowerLevel.text = floor.ToString();
//        }

//        public void InitForChampionMatch()
//        {
//            if (goTextRoot != null)
//                goTextRoot.CustomActive(false);
//            if (textChampionMatch != null)
//                textChampionMatch.transform.parent.gameObject.SetActive(true);
//        }

//        public void SetChampionMatchName(string name)
//        {
//            if (textChampionMatch != null)
//                textChampionMatch.text = name;
//        }

//        public void SetMuscleShiftActive(bool flag)
//        {
//            if (textMuscleShift != null)
//                textMuscleShift.transform.parent.gameObject.SetActive(flag);
//        }

//        public void SetMuscleShiftCount(int count)
//        {
//            if (textMuscleShift != null)
//                textMuscleShift.text = string.Format("X{0}", count);
//            if (iconMuscleShift != null && muscleShiftGray == null)
//                muscleShiftGray = iconMuscleShift.GetComponent<UIGray>();
//            if (muscleShiftGray != null)
//                muscleShiftGray.enabled = count <= 0;
//        }

//        public void SetMuscleShiftCD(float per)
//        {
//            if (imageMuscleShift != null)
//                imageMuscleShift.fillAmount = per;
//        }

//        public sealed override string GetMainUIPrefabName()
//        {
//            return "UIFlatten/Prefabs/BattleUI/ClientSystemBattleMain";
//        }

//        protected sealed override void _OnMainFrameOpen()
//        {
//            //ClientSystemManager.instance.OpenFrame<DungeonTipListFrame>();
//        }

//        bool mBVanityBonusBuff = false;

//        bool mBChaosAdditionBuff = false;

//        void OpenVanityBuffBonusFrame()
//        {
//            if (BattleMain.instance.GetDungeonManager().GetDungeonDataManager().IsYiJieCheckPoint() && ActivityManager.GetInstance().GetVanityBonusActivityIsShow())
//            {
//                ClientSystemManager.instance.OpenFrame<VanityBuffBonusFrame>(FrameLayer.High,ActivityManager.GetInstance().GetVanityBuffBonusModel());
//            }
//        }
//       void OpenChaosAdditionFrame()
//        {
//            if (BattleMain.instance.GetDungeonManager().GetDungeonDataManager().IsHunDunCheckPoint() && ActivityManager.GetInstance().GetChaosAdditionActivityIsShow())
//            {
//                ClientSystemManager.instance.OpenFrame<VanityBuffBonusFrame>(FrameLayer.High, ActivityManager.GetInstance().GetVanityBuffBonusModel());
//            }
//        }
//        [UIEventHandle("DungeonMap/CountRoot/Button")]
//        void OnPauseButtonClick()
//        {
//            ClientSystemManager.instance.OpenFrame<PauseFrame>(FrameLayer.Middle);

//            if (!BattleMain.IsModeMultiplayer(BattleMain.mode))
//                BattleMain.instance.GetDungeonManager().PauseFight();
//        }

//        protected sealed override string _GetLevelName()
//        {
//            return "DnH";
//        }

//        public void HidePauseButton(bool hide = true)
//        {
//            if(mbuttonObject != null)
//                mbuttonObject.SetActive(!hide);
//        }

//        private void _updateExpBar(bool force)
//        {
//            if (!force)
//            {
//                int addExp = (int)(PlayerBaseData.GetInstance().Exp - previousExp);
//                previousExp = PlayerBaseData.GetInstance().Exp;

//                var node = BattleMain.instance.GetPlayerManager().GetMainPlayer();
//                if (node != null)
//                {
//                    BeActor actor = node.playerActor;
//                    if (actor != null && actor.isLocalActor && actor.m_pkGeActor != null && addExp > 0)
//                        actor.m_pkGeActor.CreateHeadText(HitTextType.GET_EXP, addExp);
//                }
//            }
//            if(mExpBar != null)
//                mExpBar.SetExp(PlayerBaseData.GetInstance().Exp, force, exp =>
//                    {
//                    return TableManager.instance.GetCurRoleExp(exp);
//                    });
//        }

//        private void _setExpBarVisible(bool visible)
//        {
//            if(!mExpBar.IsNull())
//                mExpBar.gameObject.SetActive(visible);
//        }


//        private GameObject objCanvasComboText;
//        private ComboCount comboCountComp;

//        [UIControl("DungeonMap", typeof(ComDungeonMap))]
//        private ComDungeonMap mDungeonMapCom;

//        public ComDungeonMap dungeonMapCom
//        {
//            get
//            {
//                return mDungeonMapCom;
//            }
//        }

//        public ComboCount Combo
//        {
//            get
//            {
//                return comboCountComp;
//            }
//        }

//        private void _unloadMapUI()
//        {
//            if (mDungeonScore != null)
//            {
//                GameObject.Destroy(mDungeonScore.gameObject);
//                mDungeonScore = null;
//            }

//            if (mDungeonMapCom != null)
//            {
//                GameObject.Destroy(mDungeonMapCom.gameObject);
//                mDungeonMapCom = null;
//            }

//            if (mFightingTimeRoot != null)
//            {
//                GameObject.Destroy(mFightingTimeRoot.gameObject);
//                mFightingTimeRoot = null;
//            }
//        }

//        private void _loadComboUI()
//        {
//            if (objCanvasComboText == null)
//            {
//                objCanvasComboText = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/BattleUI/ComboText");
//                Utility.AttachTo(objCanvasComboText, ClientSystemManager.instance.BottomLayer);
//                comboCountComp = objCanvasComboText.GetComponent<ComboCount>();
//            }
//        }

//        private void _unloadComboUI()
//        {
//            if (objCanvasComboText != null)
//            {
//                GameObject.Destroy(objCanvasComboText);
//                objCanvasComboText = null;
//            }

//            comboCountComp = null;
//        }

//        GameObject objFlashWhite = null;

//        public void ShowFlashWhite()
//        {
//            if (objFlashWhite != null)
//            {
//                GameObject.Destroy(objFlashWhite);
//                objFlashWhite = null;
//            }

//            if (objFlashWhite == null)
//            {
//                objFlashWhite = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/Common/Common_flashWhite");
//                Utility.AttachTo(objFlashWhite, _mainFrame);
//            }
//        }

//        void _DisplayArraw(GameObject go,bool isShow,bool useActive = false)
//        {
//            if (useActive)
//                go.SetActive(isShow);
//            else
//            {
//                if (!go.activeSelf)
//                    go.SetActive(true);

//                RectTransform rcTrans = go.transform as RectTransform;
//                rcTrans.localScale = isShow ? new Vector3(0.8f, 0.8f, 0.8f) : new Vector3(0.01f, 0.01f, 0.01f);
//            }
//        }

//        public void ShowArrow(bool show = true, float angle = 0, bool right = true, bool withGo = false)
//        {
//            if (mArrowLeft != null)
//            {
//                if (withGo)
//                {
//                    _DisplayArraw(mArrowLeftGo, show);
//                    _DisplayArraw(mArrowRightGo, show);
//                    //mArrowLeftGo.SetActive(show);
//                    //mArrowRightGo.SetActive(show);
//                }
//                else
//                {
//                    _DisplayArraw(mArrowLeft, show);
//                    _DisplayArraw(mArrowRight, show);
//                    //mArrowLeft.SetActive(show);
//                    //mArrowRight.SetActive(show);
//                }

//                if (show)
//                {
//                    RectTransform rt = null;

//                    if (right)
//                    {
//                        rt = mArrowRight.transform.transform as RectTransform;
//                        if (withGo)
//                        {
//                            //mArrowLeftGo.SetActive(false);
//                            _DisplayArraw(mArrowLeftGo, false);
//                        }
//                        else
//                        {
//                            //mArrowLeft.SetActive(false);
//                            _DisplayArraw(mArrowLeft, false);
//                        }
//                    }
//                    else
//                    {
//                        rt = mArrowLeft.transform.transform as RectTransform;
//                        if (withGo)
//                        {
//                            //mArrowRightGo.SetActive(false);
//                            _DisplayArraw(mArrowRightGo, false);
//                        }
//                        else
//                        {
//                            //mArrowRight.SetActive(false);
//                            _DisplayArraw(mArrowRight, false);
//                        }
//                    }
//                    float percent = Mathf.Clamp(angle + 0.17f, 0, 1f);
//                    if (right)
//                    {
//                        rt.anchorMin = new Vector2(1f, percent);
//                        rt.anchorMax = new Vector2(1f, percent);
//                    }
//                    else
//                    {
//                        rt.anchorMin = new Vector2(0f, percent);
//                        rt.anchorMax = new Vector2(0f, percent);
//                    }
//                }
//            }
//        }



//#region Debug
//        private GameObject goDebug = null;
//        private void _loadDebugUI()
//        {
//            if (goDebug == null)
//            {
//                goDebug = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/Common/UIInput");
//                var rect = goDebug.GetComponent<RectTransform>();

//                Utility.AttachTo(goDebug, ClientSystemManager.instance.BottomLayer, true);

//                if (rect != null)
//                {
//                    rect.SetAsFirstSibling();
//                }
//            }
//        }

//        private void _unloadDebugUI()
//        {
//            if (goDebug != null)
//            {
//                GameObject.Destroy(goDebug);
//                goDebug = null;
//            }
//        }
//#endregion



//#region DeadTips

//        [UIObject("Canxue")]
//        private GameObject mDeadTips;

//        public void ShowDeadTips(bool isShow)
//        {
//			if (null != mDeadTips)
//            {
//				mDeadTips.CustomActive(isShow);
//            }
//        }
//        #endregion

//        private void _updatePrechangeJobSkillButton()
//        {
//            if (BattleMain.battleType == BattleType.NewbieGuide)
//            {
//                return ;
//            }

//            bool isShow = PlayerBaseData.GetInstance().Level > 1;

//            JobTable jobTable = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().PreChangeJobTableID);
//            if (jobTable == null)
//            {
//                if (PlayerBaseData.GetInstance().Level < 15 )
//                {
//                    Logger.LogErrorFormat("预先转职职业 {0}", PlayerBaseData.GetInstance().PreChangeJobTableID);
//                }
//                return;
//            }

//            if (null == InputManager.instance)
//            {
//                Logger.LogErrorFormat("预先转职职业 按钮为空空");
//                return;
//            }

//            ETCButton preChangeJob = InputManager.instance.GetETCButton(jobTable.ProJobSkills);
//            if (null != preChangeJob)
//            {
//                preChangeJob.gameObject.CustomActive(isShow);
//            }
//        }

//        private void _loadBattleUI()
//        {
//            GameFrameWork.instance.SwithTouchInput();

//            _loadComboUI();
//            _loadDebugUI();

//            ShowDeadTips(false);

//            if (Global.Settings.showBattleInfoPanel)
//            {
//                comDebugBattleStatis._loadBattleStatisticsUI();
//            }

//            if (mArrowLeft != null)
//            {
//                mArrowLeft.SetActive(false);
//                mArrowRight.SetActive(false);
//            }

//            if (mBlindMask != null)
//            {
//                SetBlindMask(false);
//            }


//            previousExp = PlayerBaseData.GetInstance().Exp;
//            previousLevel = PlayerBaseData.GetInstance().Level;


//            _UpdateRedPacket();
//            //if (mBind == null)
//            //	mBind = _mainFrame.GetComponent<ComCommonBind>();

//            //m_countScirpt = mBind.GetCom<ComCountScript>("count3");
//            //m_countScirpt.StopCount();
//            if(m_countScirpt != null)
//                m_countScirpt.StopCount();
//        }

//        void _UpdateRedPacket()
//        {
//            if (BattleMain.IsModePvP(BattleMain.battleType) == false)
//            {
//                int nCount = RedPackDataManager.GetInstance().GetWaitOpenCount();
//                if (btnRedPacket != null)
//                {
//                    btnRedPacket.gameObject.SetActive(nCount > 0/* && RedPackDataManager.GetInstance().CheckNewYearActivityOpen()*/);
//                }

//                if (labRedPacketCount != null)
//                {
//                    labRedPacketCount.gameObject.SetActive(nCount > 1/* && RedPackDataManager.GetInstance().CheckNewYearActivityOpen()*/);
//                    labRedPacketCount.text = nCount.ToString();
//                }
//            }
//            else
//            {
//                if (btnRedPacket != null)
//                {
//                    btnRedPacket.gameObject.SetActive(false);
//                }

//                if (labRedPacketCount != null)
//                {
//                    labRedPacketCount.gameObject.SetActive(false);
//                }
//            }
//        }

//        public void SetBlindMask(bool visible)
//        {
//            if (mBlindMask == null) return;
//            if (mBlindMask.activeSelf != visible)
//            {
//                mBlindMask.SetActive(visible);
//            }
//        }

//        public void SetBlindMaskPosition(Vector2 pos)
//        {
//            if (mBlindMask != null)
//            {
//                RectTransform rt = mBlindMask.transform as RectTransform;
//                rt.anchorMin = new Vector2(pos.x / Screen.width, pos.y / Screen.height);
//                rt.anchorMax = new Vector2(pos.x / Screen.width, pos.y / Screen.height);
//            }
//        }

//        private void _unloadBattleUI()
//        {
//            _unloadComboUI();
//            _unloadDebugUI();
//            //_unloadMapUI();

//            GameFrameWork.instance.SwithTouchInput(false);

//            DeinitAutoFight();
//        }

//        public void SetDungeonMapActive(bool active)
//        {
//            if (mDungeonMapCom != null)
//            {
//                mDungeonMapCom.gameObject.SetActive(active);
//            }
//        }

//        /// <summary>
//        /// 战斗开始携程收集
//        /// </summary>
//        /// <param name="enter"></param>
//        public sealed override void GetEnterCoroutine(AddCoroutine enter)
//        {
//            base.GetEnterCoroutine(enter);

//			#if DEBUG_SETTING
//            if (Global.Settings.startSystem == EClientSystem.Battle)
//            {
//                if (Global.Settings.isLocalDungeon)
//                {
//#if LOCAL_RECORD_FILE_REPLAY
//					EquipMasterDataManager.GetInstance().Initialize();

//					string path = Global.Settings.scenePath;

//					byte[] buff = System.IO.File.ReadAllBytes (path);
//                    //Logger.LogForReplay("[RECORD]Start DeSerialiaction:{0} buffLen:{1}", path, buff.Length);

//					ReplayFile replayFile = new ReplayFile ();


//                    int pos = 0;
//                    replayFile.decode(buff,ref pos);

//                    string content = ObjectDumper.Dump(replayFile);
//                    System.IO.File.WriteAllText(path+"_RelayFile_log.txt", content);

//                    //if(replayFile.header.raceType == 255)
//                    //{
//                    //    dungeonInfo = new SceneDungeonStartRes();
//                    //    dungeonInfo.decode(buff);
//                    //}
		

//                    SceneDungeonStartRes rep = new SceneDungeonStartRes();
//					rep.decode(buff, ref pos);

//                    content = ObjectDumper.Dump(rep);
//                    System.IO.File.WriteAllText(path + "_SceneDungeonStartRes_log.txt", content);

//                    BattleDataManager.GetInstance().ClearBatlte();
//                    BattleDataManager.GetInstance().ConvertDungeonBattleInfo(rep);


//                    _setRacePlayers(rep.players);
//					ClientApplication.playerinfo.session = rep.session;

//                    Logger.LogErrorFormat("ClientApplication.playerinfo.session = {0},[GetEnterCoroutine]", ClientApplication.playerinfo.session);

//					ClientApplication.relayServer.ip = rep.addr.ip;
//					ClientApplication.relayServer.port = rep.addr.port;
//					ClientApplication.playerinfo.accid = rep.players[0].accid;

//                    eDungeonMode mode = eDungeonMode.None;
//                    if (rep.session == 0)
//                    {
//                        mode = eDungeonMode.LocalFrame;
//                    }
//                    else
//                    {
//                        mode = eDungeonMode.SyncFrame;
//                    }

//                    //session = rep.session;

//                    BattleType type = ChapterUtility.GetBattleType((int)rep.dungeonId);
//                    //BattleMain.OpenBattle(type, mode, (int)rep.dungeonId);
//					BattleMain.OpenBattle(type, eDungeonMode.RecordFrame, (int)rep.dungeonId, string.Empty);
//					BaseBattle battle = BattleMain.instance.GetBattle () as BaseBattle;
//                    //var battle = BattleFactory.CreateBattle(type, mode, (int)rep.dungeonId);
//                    //record = battle.recordServer;
//                    battle.recordServer.sessionID = rep.session.ToString();
//                    battle.recordServer.StartRecord(type, mode, rep.session.ToString(), true, true);

//                    battle.SetBattleFlag(rep.battleFlag);

//                    battle.SetDungeonClearInfo(rep.clearedDungeonIds);
//                    var raidBattle = battle as RaidBattle;
//                    if(raidBattle!= null)
//                    {
//                    for (int i = 0; i < rep.clearedDungeonIds.Length; i++)
//                    {
//                            raidBattle.DungeonDestroyNotify((int)rep.clearedDungeonIds[i]);
//                        }
//                    }

//                    var guildBattle = battle as GuildPVEBattle;
//                    if (guildBattle != null && rep.guildDungeonInfo != null)
//                    {
//                        guildBattle.SetBossInfo(rep.guildDungeonInfo.bossOddBlood,rep.guildDungeonInfo.bossTotalBlood);
//                        guildBattle.SetBuffInfo(rep.guildDungeonInfo.buffVec);
//                    }

//                      FinalTestBattle finalBattle = battle as FinalTestBattle;
//                if (finalBattle != null && rep.zjslDungeonInfo!=null)
//                {
//                    finalBattle.SetBossInfo(rep.zjslDungeonInfo.boss1ID, rep.zjslDungeonInfo.boss1RemainHp, rep.zjslDungeonInfo.boss2ID, rep.zjslDungeonInfo.boss2RemainHp);
//                    finalBattle.SetBuffInfo(rep.zjslDungeonInfo.buffVec);
//                }
//                    FrameSync.instance.SetRelayFile(replayFile);
//                    //battle.StartLogicServer(this);
//#else
//                    // local model
//                    BattleType type = ChapterUtility.GetBattleType(Global.Settings.localDungeonID);
//                    BattleMain.OpenBattle(type, eDungeonMode.Test, Global.Settings.localDungeonID, string.Empty);
//#endif
//                }
//                else
//                {
//                    BattleMain.OpenBattle(BattleType.Single, eDungeonMode.None, 0, string.Empty);
//                    FrameSync.instance.StartSingleFrame();
//                }
//            }
//#endif

//                    enter(_3stepStart);
//        }

//		private void _setRacePlayers(RacePlayerInfo[] players)
//		{
//			BattleDataManager.GetInstance().PlayerInfo = players;
//			ClientApplication.racePlayerInfo = players;
//			for (int i = 0; i < ClientApplication.racePlayerInfo.Length; ++i)
//			{
//				var current = ClientApplication.racePlayerInfo[i];
//				if (current.accid == ClientApplication.playerinfo.accid)
//				{
//					ClientApplication.playerinfo.seat = current.seat;
//				}
//			}
//		}

//        private IEnumerator _errorHandle(eEnumError type, string msg)
//        {
//            Logger.LogErrorFormat("[战斗] {0} {1}", type, msg);

//            OnSystemError();

//            yield break;
//        }

//        //private IEnumerator _commonEndProcess(IASyncOperation op)
//        //{
//        //    op.SetProgress(1.0f);
//        //    op.SetProgressInfo("Loading...");
//        //    yield break;
//        //}

//        private IEnumerator _3stepStart(IASyncOperation op)
//        {
//            ThreeStepProcess _3step = new ThreeStepProcess(
//                    "BattleStart",
//                    ClientSystemManager.instance.enumeratorManager,
//                    BattleMain.instance.Start(op));

//            _3step.SetErrorProcessHandle(_errorHandle);

//            return _3step;
//        }

//        //public override void OnStart(SystemContent systemContent)
//        //{
//        //    
//        //}

//        //临时使用，用于打开组队面板
//        public static bool bNeedOpenTeamFrame = false;

//        public sealed override void OnEnter()
//        {
//            base.OnEnter();

//			if (Global.Settings.debugNewAutofightAI)
//				behaviac.Workspace.Instance.TryInit ();

//            if (Global.Settings.displayHUD)
//                HUDInfoViewer.instance.Init();

//            if (null != simpleActionManager)
//                simpleActionManager.Init();

//            if (state == eClientSystemState.onError)
//            {
//                //ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
//                return ;
//            }

//            if(BattleMain.IsTeamMode(BattleMain.battleType,BattleMain.mode))
//            {
//                bNeedOpenTeamFrame = true;
//            }

//            DungeonRebornFrame.isLocal = false;

//            _loadBattleUI();
//            _bindDungeonEvent();

//            _updateExpBar(true);

//            if (Global.Settings.startSystem == EClientSystem.Battle)
//            {
//                DungeonRebornFrame.isLocal = true;

//                if (!Global.Settings.isLocalDungeon)
//                {
//                    _unloadMapUI();
//                }
//            }
//            else
//            {
//                if (BattleMain.instance.battleState == BattleState.End)
//                {
//                    if(BattleMain.battleType != BattleType.ChijiPVP)
//                    {
//                        ClientSystemManager.instance.OpenFrame<PKBattleResultFrame>(FrameLayer.Middle);
//                    }
//                }
//            }
		   
//		    //Add Voice Button Init
//            InitVoiceSDKBtns();
//            InitTeamChatVoice();

//            _hiddenFunctionByBattleType(BattleMain.battleType);

//            _setDungeonItem();

//            _bindUIEvent();
//            _bindDungeonScore();

//#if DEBUG_SETTING
//            if (Global.Settings.isDebug)
//            {
//                if (Global.Settings.testPlayerNum > 1)
//                {
//                    for (int i = Global.Settings.testPlayerNum; i > 1; --i)
//                    {
//                        if (i == 3)
//                            goTmpBun2.CustomActive(true);
//                        else if (i == 2)
//                            goTmpBun1.CustomActive(true);
//                    }
//                }
//            }
//#endif
//            //add by mjx on 170821 for limitTimeGift when first dead
//            if (ActivityLimitTimeCombineManager.GetInstance().GiftDataManager != null)
//                ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.RegisterPlayerDead();

//            //重置录像状态
//            if (!ReplayServer.GetInstance().IsReplay())
//                ReplayServer.GetInstance().SetLastPlaying(false);
//            //PVP模式调整UI3DRoot
//            if (BattleMain.instance != null)
//            {
//                if (BattleMain.IsModePvP(BattleMain.battleType))
//                {
//                    SetUI3DRoot(true);
//                }
//                else
//                {
//                    SetUI3DRoot(false);
//                }
//            }

//            InitVanityBonusBuff();
//            InitHunDunBuff();
//            _updatePrechangeJobSkillButton();

//            EngineConfig.asyncLoadAnimRuntimeSwitch = true;
//            if (BattleMain.battleType == BattleType.ScufflePVP)
//            {
//                GameStatisticManager.instance.dataStatistics.Init();
//            }
//            InitSwitchEquip();
//#if ROBOT_TEST
//            if (BattleMain.IsModeMultiplayer(BattleMain.mode))
//            {
//                SetAutoFight(true);
//                autoFight.isOn = true;
//            }
//            AutoFightTestDataManager.instance.RecordBattleData(true);
//#endif
//        }


//		public void ShowTraceAnimation()
//		{
//			if (ClientSystemManager.instance.IsFrameOpen<MissionDungenFrame>())
//			{
//				var frame = ClientSystemManager.instance.GetFrame(typeof(MissionDungenFrame));
//				if (frame != null)
//				{
//					MissionDungenFrame missionFrame = (frame as MissionDungenFrame);
//					missionFrame.Move(true);

//					ClientSystemManager.instance.delayCaller.DelayCall(3000, ()=>{
//						missionFrame.Move(false);
//					});
//				}
//			}
//		}

//        public sealed override void OnStart(SystemContent systemContent)
//        {
//            var localPlayer = BattleMain.instance.GetLocalPlayer();

//            if (state == eClientSystemState.onError)
//            {
//                ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
//                ClientReconnectManager.instance.Clear();
//            }
//            else
//            {
//                var battle = BattleMain.instance.GetBattle() as BaseBattle;

//                if (battle != null)
//                {
//                    battle.PostStart();
//                    SetRebornDescOpen(battle.IsRebornCountLimit());
//                    RefreshRebornCount(battle.GetLeftRebornCount(), battle.GetMaxRebornCount());
//                }

//                ClientSystemManager.instance.delayCaller.DelayCall(100, () =>
//                {
//                        bool hasDialog = MissionManager.GetInstance().TryOpenTaskGuideInBattle();
//						if (!hasDialog)
//							ShowTraceAnimation();
//                });
//            }

            
//            _bindGetLocalPlayerHPValueChanged();
//            _bindGetLocalPlayerMPValueChanged();
//        }       

//        private bool _bindGetLocalPlayerHPValueChanged()
//        {
//            if (null == BattleMain.instance)
//            {
//                return false;
//            }

//            BattlePlayer player = BattleMain.instance.GetLocalPlayer();

//            if (!BattlePlayer.IsDataValidBattlePlayer(player))
//            {
//                return false;
//            }

//            if (null == player.playerActor)
//            {
//                return false;
//            }

//            player.playerActor.RegisterEvent(BeEventType.onHPChange, (object[] args3) =>
//            {
//                if (null == player)
//                {
//                    return ;
//                }

//                var actor = player.playerActor;

//                if (null == mComDrugTipsBar)
//                {
//                    return ;
//                }

//                if ( mComDrugTipsBar.gameObject.activeSelf)
//                {
//                    GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.HpChanged);
//                    mComDrugTipsBar.UseHpDrug(player.isAutoFight);
//                }
//            });

//            return true;
//        }
//        private bool _bindGetLocalPlayerMPValueChanged()
//        {
//            if (null == BattleMain.instance)
//            {
//                return false;
//            }
//            BattlePlayer player = BattleMain.instance.GetLocalPlayer();
//            if (!BattlePlayer.IsDataValidBattlePlayer(player))
//            {
//                return false;
//            }
//            if (null == player.playerActor)
//                    {
//                return false;
//            }
//            player.playerActor.RegisterEvent(BeEventType.onMPChange, (object[] args3) =>
//            {
//                if (null == player)
//                {
//                    return;
//                }
//                var actor = player.playerActor;
//                if (null == mComDrugTipsBar)
//                {
//                    return;
//                    }
//                if ( mComDrugTipsBar.gameObject.activeSelf)
//                {
//                    mComDrugTipsBar.UseMPDrug(player.isAutoFight);
//                }
//            });

//            return true;
//        }

//        public sealed override void OnExit()
//        {
//#if ROBOT_TEST
//            AutoFightTestDataManager.instance.RecordBattleData();
//#endif
//            SavePveBattleResult();
//            simpleActionManager.Deinit();
            
//            _unloadBattleUI();
//            _unBindUIEvent();
//            _unbindDungeonEvent();

//            _tryPushFrameBeforeBattleMainClose();

//            mIsInit3v3AllPendingCharactor = false;

//			//leave voice room
//			UnInitVoiceButtons();
//            UnInitTeamChatVoice();
//            var system = ClientSystemManager.GetInstance().TargetSystem as ClientSystemBattle;
//            SaveSkillDamageData();
//            if (BattleMain.instance != null)
//            {
//                if (system != null)
//                {
//                    BattleMain.CloseBattle(false);
//                }
//                else
//                {
//                    BattleMain.CloseBattle();
//                }
//            }
            
//            if (ReplayServer.GetInstance() != null)
//            {
//                ReplayServer.GetInstance().EndReplay(false,"SystemBattleExit");
//            }
//            ExceptionManager.GetInstance().PrintLogToFile(true);

//            ClientSystemManager.GetInstance().Clear3DUIRoot();
//            comShowHit.Clear();

//            EngineConfig.asyncLoadAnimRuntimeSwitch = false;
//            CloseChildFrameList();
//        }

//        private void SavePveBattleResult()
//        {
//#if !LOGIC_SERVER
//            if (BattleMain.instance == null)
//                return;
//            var baseBattle = BattleMain.instance.GetBattle() as BaseBattle;
//            if (baseBattle == null)
//                return;
//            PlayerLocalSetting.SetValue("BattleResult", (int)baseBattle.PveBattleResult);
//            PlayerLocalSetting.SaveConfig();
//#endif
//        }

//        private void _hiddenTimer()
//        {
//            if (null != mTimerController)
//            {
//                mTimerController.SetVisible(false);
//            }
//        }

//        private void _hiddenDropGold()
//        {
//            if (null != mDungeonMapCom)
//            {
//                mDungeonMapCom.mTextBoxRoot.SetActive(false);
//                mDungeonMapCom.mTextGoldRoot.SetActive(false);
//            }
//        }

//        private void _hiddenFunctionByBattleType(BattleType type)
//        {
//			if (comShowHit != null)
//				comShowHit.RefreshGraphicSetting ();

//            if(!mChatRoot.IsNull())
//                mChatRoot.SetActive(BattleMain.IsTeamMode(BattleMain.battleType,BattleMain.mode));
//            if(!mPvp3v3TipsRoot.IsNull())
//                mPvp3v3TipsRoot.CustomActive(false);

//            if (pingText != null)
//                pingText.gameObject.CustomActive(false);

//            switch (type)
//            {
//                case BattleType.Single:
//                    _hiddenTimer();
//                    _setExpBarVisible(false);
//                    break;
//                case BattleType.Dungeon:
//                case BattleType.YuanGu:
//                case BattleType.Hell:
//                    _hiddenTimer();
//                    //if(Global.Settings.forceUseAutoFight || type == BattleType.Dungeon) 
//                    InitAutoFight();
//                    break;
//                case BattleType.TreasureMap:
//                    _hiddenTimer();
//                    if (!mDungeonScore.IsNull() && !mDungeonScore.gameObject.IsNull())
//                        mDungeonScore.gameObject.CustomActive(false);
//                    InitAutoFight();
//                    if (!mFightingTimeRoot.IsNull() && !mFightingTimeRoot.gameObject.IsNull())
//                    {
//                        mFightingTimeRoot.gameObject.CustomActive(false);
//                    }
//                    break;
//                case BattleType.ChampionMatch:
//                    _hiddenTimer();
//                    InitForChampionMatch();
//                    InitAutoFight();
//                    break;
//                case BattleType.GuildPVE:
//                    {
//                        _hiddenTimer();
//                        GuildDungeonLvlTable table = null;
//                        if (BattleMain.instance != null)
//                        {
//                            var battle = BattleMain.instance.GetBattle() as GuildPVEBattle;
//                            if (battle != null && battle.ValidTable != null)
//                            {
//                                table = battle.ValidTable;
//                                if (table.dungeonLvl > 1)
//                                {
//                                    if(mDungeonScore != null)
//                                        mDungeonScore.gameObject.CustomActive(false);
//                                    if (mFightingTimeRoot)
//                                    {
//                                        mFightingTimeRoot.gameObject.CustomActive(false);
//                                    }
//                                }

//                            }
//                        }
//                        InitAutoFight();
//                    }
//                    break;
//                case BattleType.Mou:
//                case BattleType.North:
//                case BattleType.GoldRush:
               
//                    _hiddenTimer();
//                    if(mDungeonScore != null)
//                        mDungeonScore.gameObject.CustomActive(false);
//                    InitAutoFight();
//                    if (mFightingTimeRoot)
//                    {
//                        mFightingTimeRoot.gameObject.CustomActive(false);
//                    }
//                    break;
//                case BattleType.DeadTown:
//                case BattleType.FinalTestBattle:
//                    _hiddenTimer();
//                    _hiddenDropGold();
//                    if(mDungeonScore != null)
//                        mDungeonScore.gameObject.CustomActive(false);
//                    InitForDeadTower();
//                    InitAutoFight();
//                    if (mFightingTimeRoot)
//                    {
//                        mFightingTimeRoot.gameObject.CustomActive(false);
//                    }
//                    break;
//                case BattleType.MutiPlayer:
//                case BattleType.GuildPVP:
//                case BattleType.MoneyRewardsPVP:
//                case BattleType.PVP3V3Battle:
//                case BattleType.ScufflePVP:
//                case BattleType.ChijiPVP:
//                    _setExpBarVisible(false);
//                    _unloadMapUI();
//                    HidePauseButton();
//                    _hiddenDropGold();
//                    InitForReplay();
//#if DEBUG_SETTING
//                    if (Global.Settings.forceUseAutoFight)
//                        InitAutoFight();
//#endif
//                    break;
//                case BattleType.Training:
//                    _setExpBarVisible(false);
//                    _unloadMapUI();
//                    HidePauseButton();
//                    _hiddenDropGold();
//                    InitForTrain();
//                    break;
//                case BattleType.NewbieGuide:
//                case BattleType.TrainingSkillCombo:
//                    if(!mAutoFightRootObj.IsNull())
//                        mAutoFightRootObj.SetActive(false);
//                    if(!mChatRoot.IsNull())
//                        mChatRoot.SetActive(false);
//                    _hiddenDropGold();
//                    _hiddenTimer();
//                    _setExpBarVisible(false);
//                    HidePauseButton();
//                    if(!mComDrugTipsBar.IsNull())
//                        mComDrugTipsBar.SetRootActive(false);
//                    if(mDungeonScore != null)
//                        mDungeonScore.gameObject.SetActive(false);
//                    if (mFightingTimeRoot)
//                    {
//                        mFightingTimeRoot.gameObject.CustomActive(false);
//                    }
//                    break;
//                case BattleType.TrainingPVE:
//                    _setExpBarVisible(false);
//                    _unloadMapUI();
//                    _hiddenDropGold();
//                    InitTrainingPveBattle();
//                    break;
//                case BattleType.RaidPVE:
//                    InitAutoFight();
//                    _hiddenTimer();
//                    if(mDungeonScore != null)
//                        mDungeonScore.gameObject.SetActive(false);
//                    break;

//            }

//            InitWeaponChange();

//            if (null != mPvp3v3Button)
//            {
//                if(!mPvp3v3TimeLimitButton.IsNull())
//                    mPvp3v3TimeLimitButton.ResetCount();

//                mPvp3v3Button.gameObject.CustomActive(BattleMain.IsModePVP3V3(type) && !ReplayServer.instance.IsReplay());

//                //BattlePlayer mainPlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer();
//                //if (null != mainPlayer)
//                //{

//                //    RectTransform rect = mPvp3v3Button.GetComponent<RectTransform>();
//                //    if (null != rect)
//                //    {
//                //        int dir = mainPlayer.IsTeamRed() ? -1 : 1;

//                //        Vector3 pos = rect.localPosition;

//                //        rect.localPosition = new Vector3(dir * Mathf.Abs(pos.x), pos.y, pos.z);
//                //    }
//                //}
//            }
//        }

//#region Event

//        protected void _bindDungeonEvent()
//        {
//            if (BattleMain.instance == null)
//            {
//                return;


//            }

//            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BattleDoubleBossTips, _tipDoubleBoss);

//            switch (BattleMain.battleType)
//            {
//                case BattleType.DeadTown:
//                case BattleType.YuanGu:
//                case BattleType.Dungeon:
//                case BattleType.ChampionMatch:
//                case BattleType.GuildPVE:
//                case BattleType.FinalTestBattle:
//                    if (BattleMain.mode != eDungeonMode.Test)
//                    {
//                        MissionManager.GetInstance().TriggerDungenBegin();
//                        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonRewardFinish, _chapterFinish);
//                    }
//                    break;
//            }

//            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK3V3StartVoteForFight, TryInitVoiceChat);
//            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.onLiveShowPursueModeChange, _onLiveShowPursueModeChange);
//            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonRebornSuccess, _onPlayerReborn);
//        }
//        private void _onPlayerReborn(UIEvent ui)
//        {
//            if (BattleMain.instance == null)
//            {
//                return;
//            }
//            var battle = BattleMain.instance.GetBattle();
//            if (battle != null && battle.IsRebornCountLimit())
//            {
//                RefreshRebornCount(battle.GetLeftRebornCount(), battle.GetMaxRebornCount());
//            }
//        }
//        private void _onLiveShowPursueModeChange(UIEvent ui)
//        {
//            if (ReplayServer.GetInstance().IsLiveShow())
//            {
//                if (!ReplayServer.GetInstance().isInPersueMode)
//                {
//                    btnReplaySpeed.gameObject.SetActive(true);
//                }
//            }

//        }
//        private void _tipDoubleBoss(UIEvent ui)
//        {
//            int delayInMS = (int)ui.Param1;
//            int tipId = (int)ui.Param2;
//            CommonTipsDesc tb =  TableManager.instance.GetTableItem<CommonTipsDesc>(tipId);

//            Logger.LogProcessFormat("[双boss提示] {0}", delayInMS);

//            if (null != tb)
//            {
//                SystemNotifyManager.SysDungeonSkillTip(tb.Descs, delayInMS / 1000.0f);
//            }
//        }

//        private void _chapterFinish(UIEvent uiEvent)
//        {
//            if (BattleMain.mode != eDungeonMode.Test)
//            {
//                Logger.Log("chapter finish callback");
//                MissionManager.GetInstance().TriggerDungenEnd();
//            }
//        }

//        protected void _unbindDungeonEvent()
//        {
//            if (BattleMain.instance == null)
//                return;

//            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BattleDoubleBossTips, _tipDoubleBoss);

//            switch (BattleMain.battleType)
//            {
//                case BattleType.DeadTown:
//                case BattleType.YuanGu:
//                case BattleType.Dungeon:
//                case BattleType.ChampionMatch:
//                case BattleType.GuildPVE:
//                case BattleType.FinalTestBattle:
//                    if (BattleMain.mode != eDungeonMode.Test)
//                    {
//                        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonRewardFinish, _chapterFinish);
//                    }
//                    break;
//            }
//            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.onLiveShowPursueModeChange, _onLiveShowPursueModeChange);
//            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK3V3StartVoteForFight, TryInitVoiceChat);
//            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonRebornSuccess, _onPlayerReborn);
//        }

//        protected void _bindDungeonScore()
//        {
//            if (null != mDungeonScore)
//            {

//                mDungeonScore.Init();

//                mDungeonScore.infos[0].SetScoreCallback(
//                        () => { return BattleMain.instance.GetDungeonStatistics().HitCountScore(); });

//                mDungeonScore.infos[1].SetScoreCallback(
//                        () => { return BattleMain.instance.GetDungeonStatistics().AllFightTimeScore(true); });

//                mDungeonScore.infos[1].SetTimeLimiteCallback(
//                        () => { return BattleMain.instance.GetDungeonStatistics().FightTimeSplit(1); });

//                mDungeonScore.infos[2].SetScoreCallback(
//                        () => { return BattleMain.instance.GetDungeonStatistics().RebornCountScore(); });

//                mDungeonScore.infos[0].SetCallback(
//                        () => { return BattleMain.instance.GetDungeonStatistics().AllHitCount(); });

//                mDungeonScore.infos[1].SetCallback(
//                        () => { return BattleMain.instance.GetDungeonStatistics().AllFightTime(true); });

//                mDungeonScore.infos[2].SetCallback(
//                        () => { return BattleMain.instance.GetDungeonStatistics().AllRebornCount(); });
//            }

            
//            if (null != mDungeonScore)
//            {
//                mDungeonScore.onFadeChanged = (State) => {
//                    bool bComScoreOpend = (State == ComDungeonScore.eState.Open);
//                    if (mFightingTimeRoot)
//                    {
//                        mFightingTimeRoot.gameObject.CustomActive(!bComScoreOpend);
//                    }
//                };
//            }

//            if (null != mFightingTimeRoot && null != mFightingTimeRoot.infos && mFightingTimeRoot.infos.Length > 0)
//            {
//                mFightingTimeRoot.infos[0].SetCallback(
//                    () => { return BattleMain.instance.GetDungeonStatistics().AllFightTime(true); });
//            }            
//        }

//        protected void _bindUIEvent()
//        {
//            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.LevelChanged, _onLevelChanged);
//            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ExpChanged, _onExpChanged);
//            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PKRankChanged, _onPkRankChanged);
//            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonScoreChanged, _onScoreUpdate);
//            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonUnlockDiff, _onDiffHardUpdate);
//            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPacketOpenSuccess, _OnRedPacketOpenSuccess);
//            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPacketGet, _OnNewRedPacketGet);
//            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPacketDelete, _OnDeleteRedPacket);
//            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CurGuideStart, _OnGuideStart);
//            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CurGuideFinish, _OnGuideFinish);
//            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BattleFrameSyncEnd, _OnBattleFrameSyncEnd);
//            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK3V3StartRedyFightCount, _onPK3V3StartRedyFightCount);
//            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK3V3VoteForFightStatusChanged, _onPK3V3StartRedyFightCount);
//            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK3V3GetRoundEndResult, _onPK3V3GetRoundEndResult);
//            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.VanityBonusAnimationEnd, onUpdateVanityBonusIconStatus);
//            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SystemChanged, _OnSystemSwitchFinished);
//            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRollItemEnd, _OnRollItemEnd);
//            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TreasureMapSizeChange, _OnTreasureMapSizeChanged);
//        }

//        protected void _unBindUIEvent()
//        {
//            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.LevelChanged, _onLevelChanged);
//            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ExpChanged, _onExpChanged);
//            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PKRankChanged, _onPkRankChanged);
//            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonScoreChanged, _onScoreUpdate);
//            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonUnlockDiff, _onDiffHardUpdate);
//            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPacketOpenSuccess, _OnRedPacketOpenSuccess);
//            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPacketGet, _OnNewRedPacketGet);
//            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPacketDelete, _OnDeleteRedPacket);
//            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CurGuideStart, _OnGuideStart);
//            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CurGuideFinish, _OnGuideFinish);
//            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BattleFrameSyncEnd, _OnBattleFrameSyncEnd);
//            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK3V3StartRedyFightCount, _onPK3V3StartRedyFightCount);
//            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK3V3VoteForFightStatusChanged, _onPK3V3StartRedyFightCount);
//            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK3V3GetRoundEndResult, _onPK3V3GetRoundEndResult);
//            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.VanityBonusAnimationEnd, onUpdateVanityBonusIconStatus);
//            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SystemChanged, _OnSystemSwitchFinished);
//            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRollItemEnd, _OnRollItemEnd);
//            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TreasureMapSizeChange, _OnTreasureMapSizeChanged);
//        }


//        private class PVP3V3Unit
//        {
//            public byte                      seat;
//            public ComPVP3V3PendingCharactor com;
//        }

//        List<PVP3V3Unit> mPVP3V3Units = new List<PVP3V3Unit>();

//        private PVP3V3Unit _getPVP3V3Unit(BattlePlayer player)
//        {
//            if (!BattlePlayer.IsDataValidBattlePlayer(player))
//            {
//                return null;
//            }

//            for (int i = 0; i < mPVP3V3Units.Count; ++i)
//            {
//                if (mPVP3V3Units[i].seat == player.GetPlayerSeat())
//                {
//                    return mPVP3V3Units[i];   
//                }
//            }

//            return null;
//        }

//        private PVP3V3Unit _addPVP3V3Unit(BattlePlayer player)
//        {
//            if (!BattlePlayer.IsDataValidBattlePlayer(player))
//            {
//                return null;
//            }

//            GameObject charactor = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/Battle/Bars/Charactor/PVP3V3/3v3PendingCharactor");

//            if (null == charactor)
//            {
//                return null;
//            }

//            if (player.IsTeamRed())
//            {
//                Utility.AttachTo(charactor, mPvp3v3LeftPendingRoot);
//            }
//            else
//            {
//                Utility.AttachTo(charactor, mPvp3v3RightPendingRoot);
//            }

//            PVP3V3Unit unit = new PVP3V3Unit
//            {
//                seat = player.GetPlayerSeat(),
//                com = charactor.GetComponent<ComPVP3V3PendingCharactor>()
//            };
//            unit.com.InitWithSeat(unit.seat);

//            mPVP3V3Units.Add(unit);
           
//            return unit;
//        }

//        private void _clearPVP3V3Units()
//        {
//            for (int i = 0; i < mPVP3V3Units.Count; ++i)
//            {
//                mPVP3V3Units[i].seat = byte.MaxValue;
//                mPVP3V3Units[i].com  = null;
//            }

//            mPVP3V3Units.Clear();
//        }

//        private void _onPK3V3StartRedyFightCount(UIEvent ui)
//        {
//            _update3v3AllPendingCharactor();
//            _update3v3UIVisible();
//            _update3v3ApplyFightStatus();
//        }

//        private void _onPK3V3GetRoundEndResult(UIEvent ui)
//        {
//            if (null != mPvp3v3TimeLimitButton)
//            {
//                mPvp3v3TimeLimitButton.ResetCount();
//            }

//            if (null == mPvp3v3Button)
//            {
//                return ;
//            }

//            mPvp3v3Button.gameObject.CustomActive(false);
//        }

//        private bool mIsInit3v3AllPendingCharactor = false;

//        private void onUpdateVanityBonusIconStatus(UIEvent ui)
//        {

//            if (ClientSystemManager.GetInstance().IsFrameOpen<VanityBuffBonusFrame>())
//            {
//                ClientSystemManager.GetInstance().CloseFrame<VanityBuffBonusFrame>();
//            }

//            if (mVanityBonusBuffConent != null)
//            {
//                mVanityBonusBuffConent.CustomActive(true);
//            }

//        }

//        private void _OnSystemSwitchFinished(UIEvent uiEvent)
//        {
//            if(ChijiDataManager.GetInstance().GuardForPkEndData && BattleMain.battleType == BattleType.ChijiPVP)
//            {
//                ChijiDataManager.GetInstance().GuardForPkEndData = false;

//                Logger.LogErrorFormat("吃鸡结算异常测试----PKResult = {0},[_OnSystemSwitchFinished]", ChijiDataManager.GetInstance().PkEndData.result);

//                // 失败退出到城镇要清掉吃鸡相关数据
//                ChijiDataManager.GetInstance().ResponseBattleEnd();
//            }
//        }
//        private void _OnRollItemEnd(UIEvent uiEvent)
//        {
//            //为防止底层的其他流程(如断线，等待时间超长) 打断正常的结算流程做一个保护
//            if (ClientSystemManager.instance.IsFrameOpen<DungeonRollFrame>())
//            {
//                ClientSystemManager.instance.CloseFrame<DungeonRollFrame>();
//                ClientSystemManager.instance.OpenFrame<DungeonRollResultFrame>(FrameLayer.Middle, uiEvent.Param1);
//            }
//        }
//        private void _OnTreasureMapSizeChanged(UIEvent uiEvent)
//        {
//            var param = uiEvent.Param1 as TreasureDungeonMap.UITreasureEventParam;
//            if (param != null && !dungeonMapCom.IsNull())
//            {
//                dungeonMapCom.ResizeMap(param.width, param.height);
//            }
//        }

//        private void _init3v3AllPendingCharactor()
//        {
//            if (mIsInit3v3AllPendingCharactor)
//            {
//                return; 
//            }

//            mIsInit3v3AllPendingCharactor = true;

//            List<BattlePlayer> allPlayers = BattleMain.instance.GetPlayerManager().GetAllPlayers();

//            for (int i = 0; i < allPlayers.Count; ++i)
//            {
//                BattlePlayer player = allPlayers[i];
//                _addPVP3V3Unit(player);
//            }
//        }
       
//        private void _update3v3AllPendingCharactor()
//        {
//            _init3v3AllPendingCharactor();

//            for (int i = 0; i < mPVP3V3Units.Count; ++i)
//            {
//                PVP3V3Unit unit = mPVP3V3Units[i];
//                if (null != unit && null != unit.com)
//                {
//                    Logger.LogProcessFormat("[战斗] 更新HUD信息 玩家 {0} ", unit.seat);

//                    unit.com.UpdateInfo();

//                    BattlePlayer player = BattleMain.instance.GetPlayerManager().GetPlayerBySeat(unit.seat);


//                    if (BattlePlayer.IsDataValidBattlePlayer(player))
//                    {
//                        Logger.LogProcessFormat("[战斗] 更新HUD显示隐藏 玩家 {0} {1}  是否战斗 {2} ", player.GetPlayerName(), player.GetPlayerSeat(), player.isFighting);
//                        unit.com.gameObject.CustomActive(!player.isFighting);
//                    }
//                }
//            }
//        }

//        private void _update3v3ApplyFightStatus()
//        {
//            BattlePlayer player = BattleMain.instance.GetPlayerManager().GetMainPlayer();

//            if (!BattlePlayer.IsDataValidBattlePlayer(player))
//            {
//                return;
//            }

//            if (!player.hasFighted)
//            {
//                if (player.isVote)
//                {
//                    mPvp3v3FightOn.CustomActive(true);
//                    mPvp3v3FightOff.CustomActive(false);
//                }
//                else
//                {
//                    mPvp3v3FightOn.CustomActive(false);
//                    mPvp3v3FightOff.CustomActive(true);
//                }
//            }
//            else 
//            {
//                _update3v3UIVisible();
//            }
//        }

//        private void _update3v3UIVisible()
//        {
//            BattlePlayer player = BattleMain.instance.GetPlayerManager().GetMainPlayer();

//            if (!BattlePlayer.IsDataValidBattlePlayer(player))
//            {
//                return;
//            }

//            if (null == mPvp3v3Button)
//            {
//                return ;
//            }

//            bool isShow = true;

//            if (player.isFighting)
//            {
//                isShow = false;
//            }

//            if (player.isPassedInRound)
//            {
//                isShow = false;
//            }
//            InitWeaponChange();
//            mPvp3v3Button.gameObject.CustomActive(isShow && !ReplayServer.instance.IsReplay());
//        }

//        private void _onDiffHardUpdate(UIEvent ui)
//        {
//            int id = (int)ui.Param1;
//            int hard = (int)ui.Param2;

//            var item = TableManager.instance.GetTableItem<ProtoTable.DungeonTable>(id);
//            if (null != item)
//            {
//                SystemNotifyManager.SystemNotify(6002,
//                        item.Name,
//                        ChapterUtility.GetHardString(hard)
//                        );
//            }
//        }

//        private void _onExpChanged(UIEvent uiEvent)
//        {
//            if (null != BattleMain.instance)
//            {
//                switch (BattleMain.battleType)
//                {
//                    case BattleType.Mou:
//                    case BattleType.North:
//                    case BattleType.DeadTown:
//                    case BattleType.Dungeon:
//                    case BattleType.Hell:
//                    case BattleType.YuanGu:
//                    case BattleType.GoldRush:
//                    case BattleType.ChampionMatch:
//                    case BattleType.GuildPVE:
//                        _updateExpBar(false);
//                        break;
//                }
//            }
//        }

//        protected void _onLevelChanged(UIEvent uiEvent)
//        {
//            if (null != BattleMain.instance)
//            {
//                switch (BattleMain.battleType)
//                {
//                    case BattleType.Mou:
//                    case BattleType.North:
//                    case BattleType.DeadTown:
//                    case BattleType.Dungeon:
//                    case BattleType.Hell:
//                    case BattleType.YuanGu:
//                    case BattleType.GoldRush:
//                    case BattleType.ChampionMatch:
//                    case BattleType.GuildPVE:
//                    case BattleType.FinalTestBattle:
//                        var mainPlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer();
//                        if (mainPlayer != null && mainPlayer.playerActor != null && mainPlayer.playerActor.m_pkGeActor != null)
//                        {
//                            int levelChanged = PlayerBaseData.GetInstance().Level - previousLevel;
//                            previousLevel = PlayerBaseData.GetInstance().Level;

//                            LevelChangeCommand cmd = new LevelChangeCommand
//                            {
//                                newLevel = (int)PlayerBaseData.GetInstance().Level
//                            };
//                            FrameSync.instance.FireFrameCommand(cmd);
//                        }

//                        break;
//                }
//            }

//            _updatePrechangeJobSkillButton();
//        }
//        protected void _onPkRankChanged(UIEvent uiEvent)
//        {
//            if (null != BattleMain.instance)
//            {
//                switch (BattleMain.battleType)
//                {
//                    case BattleType.Mou:
//                    case BattleType.North:
//                    case BattleType.DeadTown:
//                    case BattleType.Dungeon:
//                    case BattleType.ChampionMatch:  
//                    case BattleType.GuildPVE:
//                    case BattleType.FinalTestBattle:
//                        var mainPlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer();
//                        if (mainPlayer != null && mainPlayer.playerActor != null && mainPlayer.playerActor.m_pkGeActor != null)
//                        {
//                            mainPlayer.playerActor.m_pkGeActor.UpdatePkRank(SeasonDataManager.GetInstance().seasonLevel, SeasonDataManager.GetInstance().seasonStar);
//                        }
//                        break;
//                }
//            }
//        }

//        protected void _OnRedPacketOpenSuccess(UIEvent a_event)
//        {
//            if (BattleMain.IsModePvP(BattleMain.battleType))
//            {
//                if (ClientSystemManager.instance.IsFrameOpen<OpenRedPacketFrame>() == false)
//                {
//                    ClientSystemManager.instance.OpenFrame<OpenRedPacketFrame>(FrameLayer.Middle, a_event.Param1);
//                }
//            }

//            _UpdateRedPacket();
//        }

//        protected void _OnNewRedPacketGet(UIEvent a_event)
//        {
//            _UpdateRedPacket();
//        }

//        protected void _OnDeleteRedPacket(UIEvent a_event)
//        {
//            _UpdateRedPacket();
//        }

//        protected void _OnGuideStart(UIEvent a_event)
//        {
//            NewbieGuideTable.eNewbieGuideTask eGuideTask = (NewbieGuideTable.eNewbieGuideTask)a_event.Param1;

//            if (eGuideTask == NewbieGuideTable.eNewbieGuideTask.AutoFightGuide)
//            {
//                autoFight.gameObject.CustomActive(false);
//            }
//#if !SERVER_LOGIC
//            BaseBattle battle = BattleMain.instance.GetBattle() as BaseBattle;
//            if (battle != null)
//            {
//                BeDungeon data = battle.dungeonManager as BeDungeon;
//                if(eGuideTask == NewbieGuideTable.eNewbieGuideTask.AbyssGuide2||eGuideTask == NewbieGuideTable.eNewbieGuideTask.AbyssGuide3)
//                SkillComboControl.instance.StartHellGuide(data);
//            }
//#endif
//        }

//        protected void _OnGuideFinish(UIEvent a_event)
//        {
//            NewbieGuideTable.eNewbieGuideTask eGuideTask = (NewbieGuideTable.eNewbieGuideTask)a_event.Param1;

//            if (eGuideTask == NewbieGuideTable.eNewbieGuideTask.AutoFightGuide)
//            {
//				InitAutoFight();
//            }
//        }
//#endregion

//        void _OnBattleFrameSyncEnd(UIEvent a_event)
//        {
//            StopTimer();
//        }

//        private void _onScoreUpdate(UIEvent uiEvent)
//        {
//            if (null != mDungeonScore 
//                && null != BattleMain.instance 
//                && BattleMain.instance.GetDungeonManager()!=null
//                && BattleMain.instance.GetDungeonManager().GetBeScene()!=null
//                && !BattleMain.instance.GetDungeonManager().GetBeScene().IsBossDead())
//            {
//                DungeonScore realScore = BattleMain.instance.GetDungeonStatistics().FinalDungeonScore();
//                mDungeonScore.SetScore(realScore);

//                if (mFightingTimeRoot)
//                {
//                    mFightingTimeRoot.SetScore(realScore);
//                }
//            }
//        }

//        public void InitVanityBonusBuff()
//        {
//            if (BattleMain.instance.GetDungeonManager().GetDungeonDataManager().IsYiJieCheckPoint() && ActivityManager.GetInstance().GetVanityBonusActivityIsShow())
//            {
//                for (int i = 0; i < PlayerBaseData.GetInstance().buffList.Count; i++)
//                {
//                    var buffId = PlayerBaseData.GetInstance().buffList[i].id;
//                    var buffTable = TableManager.GetInstance().GetTableItem<BuffTable>(buffId);
//                    if (buffTable != null)
//                    {
//                        if (buffTable.ApplyDungeon.Length > 0)
//                        {
//                            int num = 0;
//                            string[] strs = buffTable.ApplyDungeon.Split('|');
//                            for (int j = 0; j < strs.Length; j++)
//                            {
//                                if (int.TryParse(strs[j], out num))
//                                {
//                                    if (num != 17)
//                                    {
//                                        continue;
//                                    }

//                                    mVanityBonusBuffIcon.sprite = AssetLoader.instance.LoadRes(buffTable.Icon, typeof(Sprite)).obj as Sprite;
//                                    var pos = mVanityBonusBuffConent.GetComponent<RectTransform>().position;
//                                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VanityBonusBuffPos, pos, buffTable);
//                                    mBVanityBonusBuff = true;
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// 混沌地下城buff
//        /// </summary>
//        public void InitHunDunBuff()
//        {
//            if (BattleMain.instance.GetDungeonManager().GetDungeonDataManager().IsHunDunCheckPoint() && ActivityManager.GetInstance().GetChaosAdditionActivityIsShow())
//            {
//                for (int i = 0; i < PlayerBaseData.GetInstance().buffList.Count; i++)
//                {
//                    var buffId = PlayerBaseData.GetInstance().buffList[i].id;
//                    var buffTable = TableManager.GetInstance().GetTableItem<BuffTable>(buffId);
//                    if (buffTable != null)
//                    {
//                        if (buffTable.ApplyDungeon.Length > 0)
//                        {
//                            int num = 0;
//                            string[] strs = buffTable.ApplyDungeon.Split('|');
//                            for (int j = 0; j < strs.Length; j++)
//                            {
//                                if (int.TryParse(strs[j], out num))
//                                {
//                                    if (num ==(int)DungeonTable.eSubType.S_WEEK_HELL || num == (int)DungeonTable.eSubType.S_WEEK_HELL_PER || num == (int)DungeonTable.eSubType.S_WEEK_HELL_ENTRY)
//                                    {
//                                        mVanityBonusBuffIcon.sprite = AssetLoader.instance.LoadRes(buffTable.Icon, typeof(Sprite)).obj as Sprite;
//                                        var pos = mVanityBonusBuffConent.GetComponent<RectTransform>().position;
//                                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VanityBonusBuffPos, pos, buffTable);
//                                        mBChaosAdditionBuff = true;
//                                    }

                                 
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        private void _tryPushFrameBeforeBattleMainClose()
//        {
//            if (null == BattleMain.instance)
//            {
//                return;
//            }
			
//			//修炼场退出后不打开活动关卡页面	
//            if (BattleMain.battleType == BattleType.TrainingPVE)
//            {
//                if(InstituteFrame.IsEnterFromYanWUYuan)
//                {
//                    ClientSystemManager.instance.Push2FrameStack(new OpenClientFrameStackCmd(typeof(InstituteFrame)));
//                }
//                return;
//            }
          

//            int dungeonID = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;
//            dungeonID = ChapterUtility.GetOriginDungeonId(dungeonID);

//            var table = TableManager.instance.GetTableItem<ProtoTable.DungeonTable>(dungeonID);
//            if (null != table)
//            {
//#if !LOGIC_SERVER
//                // 终极试炼关卡结算失败后会弹出一个获得buf的界面
//                if (BattleMain.battleType == BattleType.FinalTestBattle)
//                {
//                    var baseBattle = BattleMain.instance.GetBattle() as BaseBattle;
//                    if (baseBattle != null)
//                    {
//                        if (baseBattle.PveBattleResult == BattleResult.Fail && ActivityDataManager.GetInstance().GetUltimateChallengeInspireBufID() > 0)
//                        {
//                            ClientSystemManager.instance.Push2FrameStack(new OpenClientFrameStackCmd(typeof(GetUltimateChallengeBufTipFrame)));
//                        }
//                    }
                    
//                    ClientSystemManager.instance.Push2FrameStack(new OpenClientFrameStackCmd(typeof(UltimateChallengeFrame)));
//                }
//#endif

//                if (!TeamDataManager.GetInstance().HasTeam())
//                {
//                    //深渊和远古，进入到挑战关卡
//                    if (table.SubType == DungeonTable.eSubType.S_YUANGU
//                        || table.SubType == DungeonTable.eSubType.S_HELL_ENTRY
//                        || table.SubType == DungeonTable.eSubType.S_LIMIT_TIME_HELL
//                        || table.SubType == DungeonTable.eSubType.S_LIMIT_TIME__FREE_HELL
//                        || table.SubType == DungeonTable.eSubType.S_WEEK_HELL
//                        || table.SubType == DungeonTable.eSubType.S_WEEK_HELL_ENTRY
//                        || table.SubType == DungeonTable.eSubType.S_WEEK_HELL_PER)
//                    {
//                        ChallengeUtility.OnOpenClientFrameStackCmd(dungeonID, table);
//                    }
//                    else
//                    {
//                        if (table.SubType != ProtoTable.DungeonTable.eSubType.S_NORMAL &&
//                            table.SubType != ProtoTable.DungeonTable.eSubType.S_WUDAOHUI &&
//                            table.SubType != ProtoTable.DungeonTable.eSubType.S_NEWBIEGUIDE &&
//                            table.SubType != ProtoTable.DungeonTable.eSubType.S_PK &&
//                            table.SubType != ProtoTable.DungeonTable.eSubType.S_COMBOTRAINING && 
//                            table.SubType != ProtoTable.DungeonTable.eSubType.S_CITYMONSTER &&
//                            table.SubType != ProtoTable.DungeonTable.eSubType.S_DEVILDDOM &&
//                            table.SubType != ProtoTable.DungeonTable.eSubType.S_GUILD_DUNGEON&&
//                            table.SubType != ProtoTable.DungeonTable.eSubType.S_RAID_DUNGEON &&
//                            table.SubType != ProtoTable.DungeonTable.eSubType.S_ANNIVERSARY_HARD &&
//                            table.SubType != ProtoTable.DungeonTable.eSubType.S_ANNIVERSARY_NORMAL && 
//                            table.SubType != ProtoTable.DungeonTable.eSubType.S_TREASUREMAP)
//                        {
//                            ActiveParams data = new ActiveParams
//                            {
//                                param0 = (ulong)dungeonID
//                            };
//                            ClientSystemManager.instance.Push2FrameStack(new OpenClientFrameStackCmd(typeof(ActivityDungeonFrame), data));
//                        }

//                    }
//                }

//                if (BattleMain.battleType == BattleType.TrainingSkillCombo || BattleMain.battleType == BattleType.Training)
//                {
//                    ClientSystemManager.instance.Push2FrameStack(new OpenClientFrameStackCmd(typeof(InstituteFrame)));
//                }
//            }
//        }

//        private float mTimeTick = 0;
//        private const float kUITick = 0.5f;
//        protected void _baseUITick(float delta)
//        {
//            mTimeTick += delta;
//            if (mTimeTick > kUITick)
//            {
//                mTimeTick -= kUITick;
//                _onUIUpdate();
//            }
//        }

//        protected void _onUIUpdate()
//        {
//            _onScoreUpdate(null);
//        }
        
//        float timer = 0;
//        float mBChaosAdditionTimer = 0;
//        protected sealed override void _OnUpdate(float timeElapsed)
//        {

//            bool isDungoneDataValid = false;
//            if (BattleMain.instance != null &&
//                BattleMain.instance.GetDungeonManager() != null &&
//                BattleMain.instance.GetDungeonManager().GetDungeonDataManager() != null)
//            {
//                isDungoneDataValid = true;
//            }
//            if (mBVanityBonusBuff)
//            {
//                timer += timeElapsed;
//                if (timer >= 0.3f)
//                {

//                    if(isDungoneDataValid && 
//                        BattleMain.instance.GetDungeonManager().GetDungeonDataManager().IsYiJieCheckPoint())
//                    {
//                        OpenVanityBuffBonusFrame();
//                    }
                    
//                    mBVanityBonusBuff = false;
//                    timer = 0;
//                }
//            }

//            if(mBChaosAdditionBuff)
//            {
//                mBChaosAdditionTimer += timeElapsed;
//                if (mBChaosAdditionTimer >= 0.3f)
//                {
//                    if (isDungoneDataValid &&
//                        BattleMain.instance.GetDungeonManager().GetDungeonDataManager().IsHunDunCheckPoint())
//                    {
//                        OpenChaosAdditionFrame();
//                    }
//                    mBChaosAdditionBuff = false;
//                    mBChaosAdditionTimer = 0;
//                }
//            }

//			int deltaTime = (int)(timeElapsed * GlobalLogic.VALUE_1000);
//            _baseUITick(timeElapsed);
//            comShowHit.Update(deltaTime);
//            UpdateCheckRestoreAutoFight(deltaTime);
//			simpleActionManager.Update(deltaTime);

////#if DEBUG_SETTING && UNITY_EDITOR
////            if (Input.GetKeyDown(KeyCode.Y))
////            {
////                var actor = BattleMain.instance.GetPlayerManager().GetMainPlayer().playerActor;
////                if (actor != null)
////                {
////                    actor.Locomote(new BeStateData((int)ActionState.AS_HURT, 0, 0, 0, 0, 300), true);
////                }
////            }
////#endif

//            {
//                if (BattleMain.instance != null)
//                {
//#if !LOGIC_SERVER
//                    if (RecordServer.GetInstance().IsReplayRecord())
//						RecordServer.GetInstance().Update(deltaTime);
//#endif
//                    if (ReplayServer.GetInstance().IsReplay())
//						ReplayServer.GetInstance().Update(deltaTime);
//                    BattleMain.instance.Update();

//                    if (BattleMain.IsModeMultiplayer(BattleMain.mode))
//                    {
//#if DEBUG_SETTING
//						if (Global.Settings.isDebug)
//                        	_onUpdateRandCount();
//#endif

//						UpdateCheckFPS(timeElapsed);
//                    }
//#if ROBOT_TEST
//                    //UpdateDungeonRobot();
//#endif

//                    if (BattleMain.IsShowPing(BattleMain.battleType))
//                    {
//                        _onUpdatePing();

//                        BattlePlayer target = BattleMain.instance.GetLocalTargetPlayer();
//                        if (null != target && null != target.playerActor)
//                        {
//                            if (BattleMain.IsModePvP(BattleMain.battleType))
//                            {
//                                if (!target.playerActor.stateController.IsInvisible())
//                                    BattleMain.instance.Main.TraceActor(target.playerActor);
//                            }
//                            else
//                            {
//                                BattleMain.instance.Main.TraceActor(target.playerActor);
//                            }
//                        }
//                    }
//                    else
//                    {
//                        if (BattleMain.battleType == BattleType.ScufflePVP)
//                        {
//                            _onUpdatePing();
//                        }
//                    }
//                }
//            }
//        }

//		protected float fpsAcc = 0;
//		protected void UpdateCheckFPS(float delta)
//		{
//			fpsAcc += delta;
//			if (fpsAcc >= ComponentFPS.instance.watchFrames )
//			{
//				fpsAcc -= ComponentFPS.instance.watchFrames;
//				GeGraphicSetting.instance.CheckBattleFPS();
//			}
//        }
        
//#if ROBOT_TEST
//        //float m_robotTimeStamp = 0;
//        //protected void UpdateDungeonRobot()
//        //{
//        //    if (mRobotInfo.IsNull())return;
//        //    var curTime = Time.realtimeSinceStartup;
//        //    float durTime = curTime - m_robotTimeStamp;
//        //    if(durTime > 1)
//        //    {
//        //        m_robotTimeStamp = curTime;
//        //        int ping = NetManager.instance.GetPingToRelayServer();
//        //        float memconsume = GC.GetTotalMemory(false) / (1024.0f * 1024);
//        //        if (memconsume > AutoFightTestDataManager.instance.MaxMemoryInBattle)
//        //        {
//        //            AutoFightTestDataManager.instance.MaxMemoryInBattle = memconsume;
//        //        }
//        //        var execCmdPerf = FrameSync.instance.execCmdPerf;
//        //        var recvCmdPerf = FrameSync.instance.recvCmdPerf;
//        //        string framePerfText1 = BeUtility.Format("Exec: Recent:{0} Average:{1} Max:{2}\n", execCmdPerf.recentDelay, execCmdPerf.averageDelay, execCmdPerf.maxDelay);
//        //        string framePerfText2 = BeUtility.Format("Recv: Recent:{0} Average:{1} Max:{2}\n", recvCmdPerf.recentDelay, recvCmdPerf.averageDelay, recvCmdPerf.maxDelay);
//        //        string framePerfText3 = BeUtility.Format("Udp: Recent:{0} Average:{1} Max:{2}\n", UdpClient.perf.recentDelay, UdpClient.perf.averageDelay, UdpClient.perf.maxDelay);
//        //        string info = BeUtility.Format("Ping :{0} Mem :{1}M\n", ping, memconsume.ToString("0.0"));
//        //        mRobotInfo.text = BeUtility.Format("{0}{1}{2}{3}", info, framePerfText1, framePerfText2, framePerfText3);
//        //    }
//        //}
//#endif

//protected float time = 0;
//        private void _onUpdatePing()
//        {
//            float delta = Time.realtimeSinceStartup - time;

//            if (delta > 1.0f)
//            {
//                int iPing = NetManager.instance.GetPingToRelayServer();
//                string strPing = "";
//                string strPingNum = "" + iPing + "ms";
//                if (iPing < 100)
//                {
//                    strPing = strPingNum;
//                }
//                else if (iPing < 200)
//                {
//                    strPing = "<color=yellow>" + strPingNum + "</color>";
//                }
//                else
//                {
//                    strPing = "<color=red>" + strPingNum + "</color>";
//                }

//                if (pingText != null)
//                {
//#if UNITY_EDITOR
//                    if (FrameSync.instance != null )
//                    {
//                        strPing += string.Format(" ExecCmd: {0} RecvCmd: {1}\n UdpRecv: {2} JetterDelay: {3}\n Drift: {4}", FrameSync.instance.execCmdPerf.recentDelay,
//                                        FrameSync.instance.recvCmdPerf.recentDelay, UdpClient.perf.recentDelay, FrameSync.instance.avgFrameDelay, Global.Settings.drift);
//                    }
//                    pingText.text = "Ping: " + strPing + " FPS: " + ComponentFPS.instance.GetFPS();
//#endif
//                }

//#if UNITY_EDITOR
//                var execCmdPerf = FrameSync.instance.execCmdPerf;
//                var recvCmdPerf = FrameSync.instance.recvCmdPerf;
//                string framePerfText1 = string.Format("Exec: Recent:{0} Average:{1} Max:{2}\n", execCmdPerf.recentDelay, execCmdPerf.averageDelay, execCmdPerf.maxDelay);                
//                string framePerfText2 = string.Format("Recv: Recent:{0} Average:{1} Max:{2}\n", recvCmdPerf.recentDelay, recvCmdPerf.averageDelay, recvCmdPerf.maxDelay);
//                string framePerfText3 = string.Format("Udp: Recent:{0} Average:{1} Max:{2}\n", UdpClient.perf.recentDelay, UdpClient.perf.averageDelay, UdpClient.perf.maxDelay);
//                if (pingText != null)
//                    pingText.text = pingText.text + "\n" + framePerfText1 + framePerfText2 + framePerfText3;
//#endif
//                if (BattleMain.battleType == BattleType.ScufflePVP)
//                {
//                    GameStatisticManager.instance.dataStatistics.CollectPingStatistic(iPing);
//                    GameStatisticManager.instance.dataStatistics.CollectFpsStatistic(ComponentFPS.instance.GetFPS());
//                }
//                time = Time.realtimeSinceStartup;
//            }
//        }

//        uint savedRandCallNum = 0;
//        private void _onUpdateRandCount()
//        {
            
//            if (pingText != null && BattleMain.instance.FrameRandom.callNum != savedRandCallNum)
//            {
//                savedRandCallNum = BattleMain.instance.FrameRandom.callNum;
//                pingText.CustomActive(true);

//                  pingText.text = "Rand CallNum:" + savedRandCallNum;
//                //pingText.text = string.Format("Rand CallNum: {0} Chasing Mode {1} UpdateCount {2}", savedRandCallNum/*, FrameSync.instance.IsInChasingMode, FrameSync.SyncCurFrameUpdate*/);
//            }
//        }
//        public void SetTipsPercent(float percent)
//        {
//            mTipsBar.enabled = true;
//            mTipsBar.fillAmount = percent;
//        }

//        public static void StartTimer(int countdown = 0)
//        {
//            if (mTimerController != null)
//            {
//                if (countdown > 0)
//                    mTimerController.SetCountdown(countdown);

//                mTimerController.StartTimer();
//            }

//            //if (null != pingText)
//            //{
//            //    pingText.gameObject.CustomActive(true);
//            //}
//        }
//        public static void StopTimer()
//        {
//            if (mTimerController != null)
//            {
//                mTimerController.StopTimer();
//            }
//        }

//        public static bool IsTimeUp()
//        {
//            if (mTimerController != null)
//            {
//                return mTimerController.IsTimeUp();
//            }
//            else
//            {
//                return true;
//            }
//        }

//        public static void UpdateTimer(int delta)
//        {
//            if (mTimerController != null)
//            {
//                mTimerController.UpdateTimer(delta);
//            }
//        }

//#region DungeonDrugTips
//        [UIControl("DungeonDrugTips", typeof(ComDrugTipsBar))]
//        private ComDrugTipsBar mComDrugTipsBar;

//        public bool IsDrugVisible()
//        {
//            if (mComDrugTipsBar != null && mComDrugTipsBar.gameObject != null)
//                return mComDrugTipsBar.gameObject.activeSelf;
//            return false;
//        }
//        public void SetDrugVisible(bool flag)
//        {
//            if (mComDrugTipsBar != null)
//                mComDrugTipsBar.CustomActive(flag);
//        }

//        private void _setDungeonItem()
//        {
//            if (mComDrugTipsBar == null)
//                return;
//            mComDrugTipsBar.Init();

//            switch (BattleMain.battleType)
//            {
//                case BattleType.Single:
//                case BattleType.MutiPlayer:
//                case BattleType.GuildPVP:
//                case BattleType.MoneyRewardsPVP:
//                case BattleType.Training:
//                case BattleType.DeadTown:
//                case BattleType.PVP3V3Battle:            
//                case BattleType.ScufflePVP:
//                case BattleType.TrainingPVE:
//                case BattleType.ChijiPVP:
//                case BattleType.FinalTestBattle:
//                    mComDrugTipsBar.SetRootActive(false);
//                    break;
//                case BattleType.Dungeon:               
//                case BattleType.GuildPVE:
//                case BattleType.ChampionMatch:
//                    mComDrugTipsBar.SetRootActive(Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.BattleDrugs));
//                    break;
//            }
//        }

//		public void UseDrug()
//		{
//            //Logger.LogProcessFormat("[PotionSet] UseDrug");

//			//if (PlayerBaseData.GetInstance().VipLevel < 1)
//			//	return;

//			//if (mComDrugTipsBar != null)
//			//	mComDrugTipsBar.UseDefaultDrug();
//		}
//#endregion

//        //protected override void _OnDisconnect(ServerType type)
//        //{
//        //    if (type == ServerType.GATE_SERVER)
//        //    {
//        //        GameFrameWork.instance.StartCoroutine(ClientSystemManager.instance.ReconnectGateServer());
//        //    }
//        //    else if (type == ServerType.RELAY_SERVER)
//        //    {
//        //        if (BattleMain.instance != null &&  
//        //            BattleMain.mode == eDungeonMode.SyncFrame &&
//        //            BattleMain.instance.battleState == BattleState.Start)
//        //        {
//        //            GameFrameWork.instance.StartCoroutine(ClientSystemManager.instance.ReconnectRelayServer());
//        //        }
//        //        else
//        //        {
//        //            OnReconnect();
//        //        }
//        //    }
//        //}

//        private void RemoveClearTip()
//        {
//            GameObject AnimationObj = Utility.FindGameObject(ClientSystemManager.instance.HighLayer, "CommonSysDungeonAnimation2", false);
//            if (AnimationObj != null)
//            {
//                GameObject.Destroy(AnimationObj);
//            }
//        }

//        public void ShowBossWarning()
//        {
//            RemoveClearTip();

//            GameObject TipAnimation = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/BattleUI/TipBossAnimation");
//            TipAnimation.name = "TipBossAnimation";
//            AutoCloseBattle close = TipAnimation.GetComponent<AutoCloseBattle>();
//            if (close != null)
//                close.SetCloseTime(2f);

//            Utility.AttachTo(TipAnimation, ClientSystemManager.instance.HighLayer);

//            AudioManager.instance.PlaySound(6);
//        }

//        public void ShowTreasureBossWarning()
//        {
//            GameObject TipAnimation = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/BattleUI/TipBossAnimation");
//            TipAnimation.name = "TipBossAnimation";
//            AutoCloseBattle close = TipAnimation.GetComponent<AutoCloseBattle>();
//            if (close != null)
//                close.SetCloseTime(2f);
//            var comBind = TipAnimation.GetComponent<ComCommonBind>();
//            if (comBind == null) return;
//            var origin = comBind.GetGameObject("JinGao");
//            if (!origin.IsNull())
//            {
//                origin.CustomActive(false);
//            }
//            var originBg = comBind.GetGameObject("Jingao_add");
//            if (!originBg.IsNull())
//            {
//                originBg.CustomActive(false);
//            }
//            var newly = comBind.GetGameObject("Jingao_1");
//            if (!newly.IsNull())
//            {
//                newly.CustomActive(true);
//            }
//            var newlyBg = comBind.GetGameObject("Jingao_add_1");
//            if (!newlyBg.IsNull())
//            {
//                newlyBg.CustomActive(true);
//            }
//            Utility.AttachTo(TipAnimation, ClientSystemManager.instance.HighLayer);

//            AudioManager.instance.PlaySound(6);
//        }

//        //设置特殊子弹的UI显示
//        public void SetSilverBulletNum(int skillId, int num, SpecialBulletType type)
//        {
//            BattleProfessionFrame frame = CreateBattleProfessionFrame();
//            if (frame != null)
//                frame.SetSilverBulletNum(skillId, num, type);
//        }

//        //设置Buff数量
//        public void SetBuffNum(int num)
//        {
//            BattleProfessionFrame frame = CreateBattleProfessionFrame();
//            if (frame != null)
//                frame.SetBuffNum(num);
//        }
//        //设置Buff数量(力法combobuff)
//        public void SetComboBuffNum(int num, float progress)
//        {
//            BattleProfessionFrame frame = CreateBattleProfessionFrame();
//            if (frame != null)
//                frame.SetComboBuffNum(num, progress);
//        }

//        //设置技能使用次数
//        public void SetSkillUseCount(int skillId, int num, string path)
//        {
//            num = num < 0 ? 0 : num;
//            BattleProfessionFrame frame = CreateBattleProfessionFrame();
//            if (frame != null)
//                frame.SetSkillUseCount(skillId, num, path);
//        }

//        /// <summary>
//        /// 设置圣骑士觉醒被动BuffUI显示
//        /// </summary>
//        public void SetShengQiBeiDongBuff(int index, int curNum, int maxNum)
//        {
//            BattleProfessionFrame frame = CreateBattleProfessionFrame();
//            if (frame != null)
//                frame.SetShengQiBeiDongBuff(index, curNum, maxNum);
//        }

//        public void InitNvDaQiangEnergyBar(int n)
//        {
//            BattleProfessionFrame frame = CreateBattleProfessionFrame();
//            if (frame != null)
//                frame.InitNvDaQiangEnergyBar(n);
//        }

//        public void SetNvDaQiangEnergyBar(int times)
//        {
//            BattleProfessionFrame frame = CreateBattleProfessionFrame();
//            if (frame != null)
//                frame.SetNvDaQiangEnergyBar(times);
//        }

//        //创建战斗信息界面
//        private BattleProfessionFrame CreateBattleProfessionFrame()
//        {
//            BattleProfessionFrame frame = ClientSystemManager.instance.GetFrame(typeof(BattleProfessionFrame)) as BattleProfessionFrame;
//            if (frame == null)
//            {
//                frame = ClientSystemManager.instance.OpenFrame<BattleProfessionFrame>() as BattleProfessionFrame;
//                Utility.AttachTo(frame.GetFrame(), mProfessionInfo);
//                if(!childFrameList.Contains(frame))
//                    childFrameList.Add(frame);
//            }
//            return frame;
//        }

//        protected string m_DamageBarPath = "UIFlatten/Prefabs/BattleUI/DamageValueBar";
//        protected GameObject m_DamageBar = null;
//        public void ShowDamageBar(bool isShow)
//        {
//            if (m_DamageBar == null)
//            {
//                m_DamageBar = AssetLoader.instance.LoadResAsGameObject(m_DamageBarPath);               
//            }
//            if (m_DamageBar != null && mBind != null)
//            {
//                Utility.AttachTo(m_DamageBar, mBind.gameObject);
//                m_DamageBar.CustomActive(isShow);
//            }
//        }

//        //显示伤害值进度条
//        public void ChangeDamageData(float current, float max)
//        {
//            ComCommonBind bind = m_DamageBar.GetComponent<ComCommonBind>();
//            bind.GetCom<Slider>("Slider").value = current / max;
//            string currStr = (current / 10000).ToString("#0.0");
//            string maxStr = (max / 10000).ToString("#0.0");
//            string value = string.Format("{0}W/{1}W", currStr, maxStr);
//            bind.GetCom<Text>("DamageValue").text = value;
//        }

//        //倒计时
//        public void ChangeCountDown(int countTime)
//        {
//            ComCommonBind bind = m_DamageBar.GetComponent<ComCommonBind>();
//            bind.GetCom<Text>("CountDown").text = countTime.ToString();
//        }

//        //播放角色出场动画
//        public void PlayCharacterAni(bool faceLeft,string prefabPath,int removeDelay)
//        {
//            GameObject characterAni = AssetLoader.instance.LoadResAsGameObject(prefabPath);
//            if (characterAni != null)
//            {
//                Utility.AttachTo(characterAni, ClientSystemManager.instance.GetLayer(FrameLayer.TopMost));
//                ComCommonBind aniBind = characterAni.GetComponent<ComCommonBind>();
//                if (aniBind != null)
//                {
//                    aniBind.GetCom<Image>("Left").CustomActive(!faceLeft);
//                    aniBind.GetCom<Image>("Right").CustomActive(faceLeft);
//                }
//                ClientSystemManager.instance.delayCaller.DelayCall(removeDelay, () =>
//                {
//                    GameObject.Destroy(characterAni);
//                });
//            }
//        }

//        [UIEventHandle("RedPacket")]
//        void _OnOpenRedPacket()
//        {
//            RedPacketBaseEntry redPacket = RedPackDataManager.GetInstance().GetFirstRedPacketToOpen();
//            if (redPacket != null)
//            {
//                RedPackDataManager.GetInstance().OpenRedPacket(redPacket.id);
//            }
//        }

//        //根据战斗场景类型 设置3DRoot
//        protected void SetUI3DRoot(bool isPvp)
//        {
//#if !LOGIC_SERVER
//            if (ClientSystemManager.GetInstance() != null)
//            {
//                RectTransform ui3drootRect = ClientSystemManager.GetInstance().Layer3DRoot.GetComponent<RectTransform>();
//                Vector3 orignalPos = ui3drootRect.localPosition;
//                ui3drootRect.localPosition = isPvp ? new Vector3(orignalPos.x, orignalPos.y, -6.5f) : new Vector3(orignalPos.x, orignalPos.y, -1f);
//            }
//#endif
//        }

//        //[UIEventHandle("DungeonMap/CountRoot/Chat")]
//        //void _onButtonChat()
//        //{
//        //    //ClientSystemManager.instance.OpenFrame<DungeonTeamChatFrame>();
//        //    //ChatManager.GetInstance().OpenTeamChatFrame();
//        //}

//        public void HideFightingTime()
//        {
//            if (mFightingTimeRoot == null)
//                return;
//            mFightingTimeRoot.CustomActive(false);
//        }

//#region PVE修炼场相关
//        //初始化修炼场UI数据
//        protected void InitTrainingPveBattle()
//        {
//#if !LOGIC_SERVER
//            if (BattleMain.instance == null)
//                return;
//            _hiddenTimer();
//            var battle =  BattleMain.instance.GetBattle() as TrainingPVEBattle;
//            if (battle == null)
//                return;
//            battle.Init();
//#endif
//        }

//        //保存技能伤害数据
//        protected void SaveSkillDamageData()
//        {
//#if !LOGIC_SERVER
//            if (BattleMain.instance == null
//                || BattleMain.instance.GetPlayerManager() == null
//                || BattleMain.instance.GetPlayerManager().GetMainPlayer() == null)
//                return;
//            if (BattleMain.IsModePvP(BattleMain.battleType))
//                return;
//            BeActor actor = BattleMain.instance.GetPlayerManager().GetMainPlayer().playerActor;
//            string dungeonName = BeUtility.GetDungeonName();
//            if (actor == null)
//                return;
//            if(actor.skillDamageManager != null)
//                actor.skillDamageManager.SaveSkillDamageData(actor, dungeonName);
//#endif
//        }
//#endregion

//        //关闭所有挂载的子页面
//        private void CloseChildFrameList()
//        {
//#if !LOGIC_SERVER
//            for (int i=0;i< childFrameList.Count; i++)
//            {
//                if(childFrameList[i] != null)
//                    childFrameList[i].Close();
//            }
//            childFrameList.Clear();
//#endif
//        }
//    }
//}
