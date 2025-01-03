using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ProtoTable;
using Protocol;

namespace GameClient
{
    /// <summary>
    /// pve相关UI
    /// </summary>
    public class BattleUIPve : BattleUIBase
    {
        public BattleUIPve() : base() { }

        #region ExtraUIBind
        private GameObject mAutoFightEffect = null;
        private Toggle mToggleAutoFight = null;
        private Image mTipsBar = null;
        private ComCountScript mCountScirpt = null;
        private Image mVanityBonusBuffIcon = null;
        private GameObject mVanityBonusBuffConent = null;
        private GameObject mDeadTips = null;
        private ComBattleExpBar mExpBar = null;
        private GameObject mRebornInfoRoot = null;
        private Text mRebornDesc = null;
        private ButtonEx mNextRoom = null;
        private Image mAutoFightIcon = null;

        protected override void _bindExUI()
        {
            mAutoFightEffect = mBind.GetGameObject("AutoFightEffect");
            mToggleAutoFight = mBind.GetCom<Toggle>("ToggleAutoFight");
            mTipsBar = mBind.GetCom<Image>("TipsBar");
            mCountScirpt = mBind.GetCom<ComCountScript>("CountScirpt");
            mVanityBonusBuffIcon = mBind.GetCom<Image>("VanityBonusBuffIcon");
            mVanityBonusBuffConent = mBind.GetGameObject("VanityBonusBuffConent");
            mDeadTips = mBind.GetGameObject("DeadTips");
            mExpBar = mBind.GetCom<ComBattleExpBar>("ExpBar");
            mRebornInfoRoot = mBind.GetGameObject("RebornInfoRoot");
            mRebornDesc = mBind.GetCom<Text>("RebornDesc");
            mNextRoom = mBind.GetCom<ButtonEx>("NextRoom");
            mAutoFightIcon = mBind.GetCom<Image>("AutoFightIcon");
        }

        protected override void _unbindExUI()
        {
            mAutoFightEffect = null;
            mToggleAutoFight = null;
            mTipsBar = null;
            mCountScirpt = null;
            mVanityBonusBuffIcon = null;
            mVanityBonusBuffConent = null;
            mDeadTips = null;
            mExpBar = null;
            mRebornInfoRoot = null;
            mRebornDesc = null;
            mNextRoom = null;
            mAutoFightIcon = null;
        }
        #endregion

        private bool mBChaosAdditionBuff = false;
        private bool mBVanityBonusBuff = false;
        private ulong previousExp;

        protected override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/BattleUIComponent/BattleUIPve";
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            previousExp = PlayerBaseData.GetInstance().Exp;
            _updateExpBar(true);
            InitVanityBonusBuff();
            InitHunDunBuff();
            ShowDeadTips(false);
            if (mCountScirpt != null)
                mCountScirpt.StopCount();
            InitAutoFight();

            mNextRoom.onClick.AddListener(() => {
                if(BattleMain.instance != null)
                {
                    var battle = BattleMain.instance.GetBattle() as PVEBattle;
                    if(battle != null)
                    {
                        battle.ForceChangeRoom(true);
                    }
                }
            });

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ExpChanged, _onExpChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonRebornSuccess, _onPlayerReborn);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.LevelChanged, _onLevelChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonUnlockDiff, _onDiffHardUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CurGuideStart, _OnGuideStart);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CurGuideFinish, _OnGuideFinish);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.VanityBonusAnimationEnd, onUpdateVanityBonusIconStatus);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRollItemEnd, _OnRollItemEnd);
        }

        protected override void OnStart()
        {
            base.OnStart();
            SetRebornDescOpen();
            RefreshRebornCount();
        }

        protected override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void OnUpdate(float timeElapsed)
        {
            base.OnUpdate(timeElapsed);
            int deltaTime = (int)(timeElapsed * GlobalLogic.VALUE_1000);
            UpdateCheckRestoreAutoFight(deltaTime);
        }

        protected override void OnExit()
        {
            base.OnExit();
            DeinitAutoFight();

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ExpChanged, _onExpChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonRebornSuccess, _onPlayerReborn);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.LevelChanged, _onLevelChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonUnlockDiff, _onDiffHardUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CurGuideStart, _OnGuideStart);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CurGuideFinish, _OnGuideFinish);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.VanityBonusAnimationEnd, onUpdateVanityBonusIconStatus);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRollItemEnd, _OnRollItemEnd);
        }

        public void ShowDigit(bool flag)
        {
            if (mCountScirpt != null)
                mCountScirpt.Show(flag);
        }

        public void SetCountDigit(int second)
        {
            if (mCountScirpt != null)
                mCountScirpt.SetMTimeImage(second);
        }

        public void SetRebornDescOpen()
        {
            if (BattleMain.instance == null) return;
            var _baseBattle = BattleMain.instance.GetBattle();
            if (_baseBattle == null) return;
            bool isActive = _baseBattle.IsRebornCountLimit();
            if (mRebornInfoRoot == null) return;
            mRebornInfoRoot.CustomActive(isActive);
        }

        public void RefreshRebornCount()
        {
            if (BattleMain.instance == null) return;
            var _baseBattle = BattleMain.instance.GetBattle();
            if (_baseBattle == null) return;
            int leftCount = _baseBattle.GetLeftRebornCount();
            int maxCount = _baseBattle.GetMaxRebornCount();
            if (mRebornDesc == null)
                return;
            mRebornDesc.text = string.Format("队伍复活次数 : {0}/{1}", leftCount, maxCount);
        }

        /// <summary>
        /// 显示死亡Tips
        /// </summary>
        /// <param name="isShow">为True时表示显示</param>
        public void ShowDeadTips(bool isShow)
        {
            if (mDeadTips == null) return;
            mDeadTips.CustomActive(isShow);
        }

        public void SetTipsPercent(float percent)
        {
            mTipsBar.enabled = true;
            mTipsBar.fillAmount = percent;
        }

        private void RemoveClearTip()
        {
            GameObject AnimationObj = Utility.FindGameObject(ClientSystemManager.instance.HighLayer, "CommonSysDungeonAnimation2", false);
            if (AnimationObj != null)
            {
                GameObject.Destroy(AnimationObj);
            }
        }

        public void ShowBossWarning()
        {
            RemoveClearTip();

            GameObject TipAnimation = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/BattleUI/TipBossAnimation");
            TipAnimation.name = "TipBossAnimation";
            AutoCloseBattle close = TipAnimation.GetComponent<AutoCloseBattle>();
            if (close != null)
                close.SetCloseTime(2f);

            Utility.AttachTo(TipAnimation, ClientSystemManager.instance.HighLayer);

            AudioManager.instance.PlaySound(6);
        }

        public void ShowTraceAnimation()
        {
            if (ClientSystemManager.instance.IsFrameOpen<MissionDungenFrame>())
            {
                var frame = ClientSystemManager.instance.GetFrame(typeof(MissionDungenFrame));
                if (frame != null)
                {
                    MissionDungenFrame missionFrame = (frame as MissionDungenFrame);
                    missionFrame.Move(true);

                    ClientSystemManager.instance.delayCaller.DelayCall(3000, () =>
                    {
                        missionFrame.Move(false);
                    });
                }
            }
        }

        public void CloseLevelTip()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<NewbieGuideBattleTipsFrame>())
                ClientSystemManager.GetInstance().CloseFrame<NewbieGuideBattleTipsFrame>();
        }

        public void ShowLevelTip(int tip)
        {
            CloseLevelTip();

            var data = TableManager.GetInstance().GetTableItem<ProtoTable.MonsterSpeech>(tip);
            if (data != null)
            {
                NewbieGuideBattleTipsFrame frame = ClientSystemManager.GetInstance().OpenFrame<NewbieGuideBattleTipsFrame>(FrameLayer.Top) as NewbieGuideBattleTipsFrame;
                frame.SetTipsText(data.Speech);
            }
        }

        public void ShowTreasureBossWarning()
        {
            GameObject TipAnimation = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/BattleUI/TipBossAnimation");
            TipAnimation.name = "TipBossAnimation";
            AutoCloseBattle close = TipAnimation.GetComponent<AutoCloseBattle>();
            if (close != null)
                close.SetCloseTime(2f);
            var comBind = TipAnimation.GetComponent<ComCommonBind>();
            if (comBind == null) return;
            var origin = comBind.GetGameObject("JinGao");
            if (!origin.IsNull())
            {
                origin.CustomActive(false);
            }
            var originBg = comBind.GetGameObject("Jingao_add");
            if (!originBg.IsNull())
            {
                originBg.CustomActive(false);
            }
            var newly = comBind.GetGameObject("Jingao_1");
            if (!newly.IsNull())
            {
                newly.CustomActive(true);
            }
            var newlyBg = comBind.GetGameObject("Jingao_add_1");
            if (!newlyBg.IsNull())
            {
                newlyBg.CustomActive(true);
            }
            Utility.AttachTo(TipAnimation, ClientSystemManager.instance.HighLayer);

            AudioManager.instance.PlaySound(6);
        }

        public void InitVanityBonusBuff()
        {
            if (BattleMain.instance.GetDungeonManager().GetDungeonDataManager().IsYiJieCheckPoint() && ActivityManager.GetInstance().GetVanityBonusActivityIsShow())
            {
                for (int i = 0; i < PlayerBaseData.GetInstance().buffList.Count; i++)
                {
                    var buffId = PlayerBaseData.GetInstance().buffList[i].id;
                    var buffTable = TableManager.GetInstance().GetTableItem<BuffTable>(buffId);
                    if (buffTable != null)
                    {
                        if (buffTable.ApplyDungeon.Length > 0)
                        {
                            int num = 0;
                            string[] strs = buffTable.ApplyDungeon.Split('|');
                            for (int j = 0; j < strs.Length; j++)
                            {
                                if (int.TryParse(strs[j], out num))
                                {
                                    if (num != 17)
                                    {
                                        continue;
                                    }

                                    mVanityBonusBuffIcon.sprite = AssetLoader.instance.LoadRes(buffTable.Icon, typeof(Sprite)).obj as Sprite;
                                    var pos = mVanityBonusBuffConent.GetComponent<RectTransform>().position;
                                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VanityBonusBuffPos, pos, buffTable);
                                    mBVanityBonusBuff = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 混沌地下城buff
        /// </summary>
        public void InitHunDunBuff()
        {
            if (BattleMain.instance.GetDungeonManager().GetDungeonDataManager().IsHunDunCheckPoint() && ActivityManager.GetInstance().GetChaosAdditionActivityIsShow())
            {
                for (int i = 0; i < PlayerBaseData.GetInstance().buffList.Count; i++)
                {
                    var buffId = PlayerBaseData.GetInstance().buffList[i].id;
                    var buffTable = TableManager.GetInstance().GetTableItem<BuffTable>(buffId);
                    if (buffTable != null)
                    {
                        if (buffTable.ApplyDungeon.Length > 0)
                        {
                            int num = 0;
                            string[] strs = buffTable.ApplyDungeon.Split('|');
                            for (int j = 0; j < strs.Length; j++)
                            {
                                if (int.TryParse(strs[j], out num))
                                {
                                    if (num == (int)DungeonTable.eSubType.S_WEEK_HELL || num == (int)DungeonTable.eSubType.S_WEEK_HELL_PER || num == (int)DungeonTable.eSubType.S_WEEK_HELL_ENTRY)
                                    {
                                        mVanityBonusBuffIcon.sprite = AssetLoader.instance.LoadRes(buffTable.Icon, typeof(Sprite)).obj as Sprite;
                                        var pos = mVanityBonusBuffConent.GetComponent<RectTransform>().position;
                                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VanityBonusBuffPos, pos, buffTable);
                                        mBChaosAdditionBuff = true;
                                    }


                                }
                            }
                        }
                    }
                }
            }
        }

        public void SetAutoFight(bool auto)
        {
            if (null == BattleMain.instance) return;
            var actor = BattleMain.instance.GetPlayerManager().GetMainPlayer();
            if (null != actor && null != actor.playerActor)
            {
                AutoFightCommand cmd = new AutoFightCommand
                {
                    openAutoFight = auto,
                    seat = actor.playerInfo.seat
                };
                FrameSync.instance.FireFrameCommand(cmd);
            }
        }

        public void LoadAutoFightConfig()
        {
            if (!Global.Settings.forceUseAutoFight)
            {
                if (!SwitchFunctionUtility.IsOpen(12) && BattleMain.IsModeMultiplayer(BattleMain.mode))
                    return;
            }

            if (mToggleAutoFight != null)
            {
                var savedAutoFight = PlayerLocalSetting.GetValue("AutoFight");
                if (savedAutoFight != null)
                {
                    mToggleAutoFight.isOn = (bool)savedAutoFight;

                    if (mToggleAutoFight.isOn)
                    {
                        ClientSystemManager.instance.delayCaller.DelayCall(10, () =>
                        {
                            SetAutoFight(false);
                            StartCheckRestoreAutoFight();
                        });
                    }
                    else
                    {
                        SetAutoFight(false);
                    }
                }
            }
        }

        public void SetAutoFightIsOn(bool isOn)
        {
            if (mToggleAutoFight == null)
                return;
            mToggleAutoFight.isOn = isOn;
        }

        public void SaveAutoFightConfig(bool flag)
        {
            if (mToggleAutoFight != null)
            {
                PlayerLocalSetting.SetValue("AutoFight", flag);
                PlayerLocalSetting.SaveConfig();
            }
        }

        public bool CanLevelOpenAutoFight()
        {
            return Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.AutoFight);
        }

        public bool CanOpenAutoFight()
        {
#if DEBUG_SETTING
            if (Global.Settings.startSystem == EClientSystem.Battle)
                return true;
#endif

#if DEBUG_FIGHT || UNITY_EDITOR
            if (Global.Settings.forceUseAutoFight)
                return true;
#endif

            if (!CanLevelOpenAutoFight())
                return false;

            if (Global.Settings.canUseAutoFightFirstPass)
            {
                if (BattleMain.battleType == BattleType.Hell)
                    return ChapterUtility.HellIsPass((int)BattleDataManager.GetInstance().BattleInfo.dungeonId);
                else
                    return ChapterUtility.GetDungeonState(BattleDataManager.GetInstance().BattleInfo.dungeonId) == ComChapterDungeonUnit.eState.Passed;
            }

            return true;
            //return CanLevelOpenAutoFight();//PlayerBaseData.GetInstance().Level >= Utility.GetFuncUnlockLevel(ProtoTable.FunctionUnLock.eFuncType.AutoFight);
        }

        public bool CanShowOpenAutoFight()
        {
#if DEBUG_SETTING
            if (Global.Settings.startSystem == EClientSystem.Battle)
                return true;
#endif

#if DEBUG_FIGHT || UNITY_EDITOR
            if (Global.Settings.forceUseAutoFight)
                return true;
#endif

            if (NewbieGuideManager.GetInstance().GetNextGuide(NewbieGuideTable.eNewbieGuideType.NGT_WEAK) == NewbieGuideTable.eNewbieGuideTask.AutoFightGuide)
            {
                return false;
            }

            return Global.Settings.canUseAutoFight && ChapterUtility.CanDungeonOpenAutoFight(BattleDataManager.GetInstance().BattleInfo.dungeonId);
        }

        public void InitAutoFight()
        {
            //ETCImageLoader.LoadSprite(ref mAutoFightIcon,"UI/Image/NewPacked/NewBattle/Battle_Pve.png:Battle_Pve_Btn_Zidong_02");
            if (!CanShowOpenAutoFight())
                return;

            if (!CanOpenAutoFight())
            {
                if (mToggleAutoFight != null)
                {
                    mToggleAutoFight.gameObject.CustomActive(true);
                    mToggleAutoFight.gameObject.SafeAddComponent<UIGray>();
                    //ETCImageLoader.LoadSprite(ref mAutoFightIcon,"UI/Image/NewPacked/NewBattle/Battle_Pve.png:Battle_Pve_Btn_Zidong_01");
                    mToggleAutoFight.onValueChanged.AddListener((bool choose) =>
                    {
                        string strNotify = "";

                        if (!CanLevelOpenAutoFight())
                        {
                            strNotify = string.Format("{0}级开启自动战斗", Utility.GetFunctionUnlockLevel(FunctionUnLock.eFuncType.AutoFight));
                        }
                        else
                        {
                            strNotify = string.Format("首次通关后开启自动战斗");
                        }

                        SystemNotifyManager.SysNotifyTextAnimation(strNotify);
                    });
                }

                return;
            }

            if (mToggleAutoFight != null)
            {
                StopCheckRestoreAutoFight();

                mToggleAutoFight.gameObject.CustomActive(true);

                mToggleAutoFight.onValueChanged.AddListener((bool choose) =>
                {
                    StopCheckRestoreAutoFight();
                    SetAutoFight(choose);
                    
                    mAutoFightEffect.CustomActive(choose);

                    SaveAutoFightConfig(choose);
                });

                if (Global.Settings.loadAutoFight)
                    LoadAutoFightConfig();


                var player = BattleMain.instance.GetPlayerManager().GetMainPlayer().playerActor;
                if (null != player)
                {
                    player.RegisterEventNew(BeEventType.onMoveJoystick, (args) =>
                    {
                        if (mToggleAutoFight.isOn)
                            SetAutoFight(false);
                    });

                    player.RegisterEventNew(BeEventType.onStopMoveJoystick, (args) =>
                    {
                        if (mToggleAutoFight.isOn)
                            StartCheckRestoreAutoFight();
                    });

                    player.RegisterEventNew(BeEventType.onPassedDoor, (args) =>
                    {
                        if (mToggleAutoFight.isOn)
                        {
                            StartCheckRestoreAutoFight();
                        }
                    });
                    player.RegisterEventNew(BeEventType.onStateChange, (param) =>
                    {
                        ActionState state = (ActionState)param.m_Int;
                        if (state == ActionState.AS_IDLE && !player.IsInMoveDirection() && mToggleAutoFight.isOn && player.pauseAI)
                        {
                            StartCheckRestoreAutoFight();
                        }
                    });
                }
            }
        }

        public void DeinitAutoFight()
        {
            mAutoFightEffect = null;
        }

        int autoFightAcc = 0;
        int AUTOFIGHT_RESTORE_INTERVAL = 2000;
        bool checkRestoreAutoFight = false;

        public void StartCheckRestoreAutoFight()
        {
            checkRestoreAutoFight = true;
            autoFightAcc = 0;
        }

        public void StopCheckRestoreAutoFight()
        {
            if (!checkRestoreAutoFight)
                return;

            checkRestoreAutoFight = false;
        }

        public void UpdateCheckRestoreAutoFight(int delta)
        {
            if (checkRestoreAutoFight)
            {
                autoFightAcc += delta;
                if (autoFightAcc >= AUTOFIGHT_RESTORE_INTERVAL)
                {
                    StopCheckRestoreAutoFight();
                    if (null == mToggleAutoFight)
                    {
                        return;
                    }

                    BattlePlayer localPlayer = null;
                    if (BattleMain.instance != null)
                        localPlayer = BattleMain.instance.GetLocalPlayer();

                    if (null == localPlayer ||
                        null == localPlayer.playerActor)
                    {
                        return;
                    }

                    if (mToggleAutoFight.isOn && !localPlayer.playerActor.IsInMoveDirection())
                    {
                        SetAutoFight(true);
                    }
                }
            }
        }

        protected void _OnGuideFinish(UIEvent a_event)
        {
            NewbieGuideTable.eNewbieGuideTask eGuideTask = (NewbieGuideTable.eNewbieGuideTask)a_event.Param1;

            if (eGuideTask == NewbieGuideTable.eNewbieGuideTask.AutoFightGuide)
            {
                InitAutoFight();
            }
        }

        private void _updateExpBar(bool force)
        {
            if (!force)
            {
                int addExp = (int)(PlayerBaseData.GetInstance().Exp - previousExp);
                previousExp = PlayerBaseData.GetInstance().Exp;

                var node = BattleMain.instance.GetPlayerManager().GetMainPlayer();
                if (node != null)
                {
                    BeActor actor = node.playerActor;
                    if (actor != null && actor.isLocalActor && actor.m_pkGeActor != null && addExp > 0)
                        actor.m_pkGeActor.CreateHeadText(HitTextType.GET_EXP, addExp);
                }
            }
            if (mExpBar != null)
                mExpBar.SetExp(PlayerBaseData.GetInstance().Exp, force, exp =>
                {
                    return TableManager.instance.GetCurRoleExp(exp);
                });
        }

        private void _onExpChanged(UIEvent uiEvent)
        {
            if (null != BattleMain.instance)
            {
                switch (BattleMain.battleType)
                {
                    case BattleType.Mou:
                    case BattleType.North:
                    case BattleType.DeadTown:
                    case BattleType.Dungeon:
                    case BattleType.Hell:
                    case BattleType.YuanGu:
                    case BattleType.GoldRush:
                    case BattleType.ChampionMatch:
                    case BattleType.GuildPVE:
                        _updateExpBar(false);
                        break;
                }
            }
        }


        private void _onPlayerReborn(UIEvent ui)
        {
            if (BattleMain.instance == null)
            {
                return;
            }
            var battle = BattleMain.instance.GetBattle();
            if (battle != null && battle.IsRebornCountLimit())
            {
                RefreshRebornCount(battle.GetLeftRebornCount(), battle.GetMaxRebornCount());
            }
        }

        public void RefreshRebornCount(int leftCount, int maxCount)
        {
            if (mRebornDesc != null)
            {
                mRebornDesc.text = string.Format("队伍复活次数 : {0}/{1}", leftCount, maxCount);
            }
        }

        protected void _onLevelChanged(UIEvent uiEvent)
        {
            if (null != BattleMain.instance)
            {
                switch (BattleMain.battleType)
                {
                    case BattleType.Mou:
                    case BattleType.North:
                    case BattleType.DeadTown:
                    case BattleType.Dungeon:
                    case BattleType.Hell:
                    case BattleType.YuanGu:
                    case BattleType.GoldRush:
                    case BattleType.ChampionMatch:
                    case BattleType.GuildPVE:
                    case BattleType.FinalTestBattle:
                        var mainPlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer();
                        if (mainPlayer != null && mainPlayer.playerActor != null && mainPlayer.playerActor.m_pkGeActor != null)
                        {
                            LevelChangeCommand cmd = new LevelChangeCommand
                            {
                                newLevel = (int)PlayerBaseData.GetInstance().Level
                            };
                            FrameSync.instance.FireFrameCommand(cmd);
                        }

                        break;
                }
            }

            _updatePrechangeJobSkillButton();
        }

        private void _updatePrechangeJobSkillButton()
        {
            if (BattleMain.battleType == BattleType.NewbieGuide)
            {
                return;
            }

            bool isShow = PlayerBaseData.GetInstance().Level > 1;

            JobTable jobTable = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().PreChangeJobTableID);
            if (jobTable == null)
            {
                if (PlayerBaseData.GetInstance().Level < 15)
                {
                    Logger.LogErrorFormat("预先转职职业 {0}", PlayerBaseData.GetInstance().PreChangeJobTableID);
                }
                return;
            }

            if (null == InputManager.instance)
            {
                Logger.LogErrorFormat("预先转职职业 按钮为空空");
                return;
            }

            ETCButton preChangeJob = InputManager.instance.GetETCButton(jobTable.ProJobSkills);
            if (null != preChangeJob)
            {
                preChangeJob.gameObject.CustomActive(isShow);
            }
        }

        private void _onDiffHardUpdate(UIEvent ui)
        {
            int id = (int)ui.Param1;
            int hard = (int)ui.Param2;

            var item = TableManager.instance.GetTableItem<ProtoTable.DungeonTable>(id);
            if (null != item)
            {
                SystemNotifyManager.SystemNotify(6002,
                        item.Name,
                        ChapterUtility.GetHardString(hard)
                        );
            }
        }

        protected void _OnGuideStart(UIEvent a_event)
        {
            NewbieGuideTable.eNewbieGuideTask eGuideTask = (NewbieGuideTable.eNewbieGuideTask)a_event.Param1;

            if (eGuideTask == NewbieGuideTable.eNewbieGuideTask.AutoFightGuide)
            {
                mToggleAutoFight.gameObject.CustomActive(false);
            }
#if !SERVER_LOGIC
            BaseBattle battle = BattleMain.instance.GetBattle() as BaseBattle;
            if (battle != null)
            {
                BeDungeon data = battle.dungeonManager as BeDungeon;
                if (eGuideTask == NewbieGuideTable.eNewbieGuideTask.AbyssGuide2 || eGuideTask == NewbieGuideTable.eNewbieGuideTask.AbyssGuide3)
                    SkillComboControl.instance.StartHellGuide(data);
            }
#endif
        }

        private void onUpdateVanityBonusIconStatus(UIEvent ui)
        {

            if (ClientSystemManager.GetInstance().IsFrameOpen<VanityBuffBonusFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<VanityBuffBonusFrame>();
            }

            if (mVanityBonusBuffConent != null)
            {
                mVanityBonusBuffConent.CustomActive(true);
            }
        }

        private void _OnRollItemEnd(UIEvent uiEvent)
        {
            //为防止底层的其他流程(如断线，等待时间超长) 打断正常的结算流程做一个保护
            if (ClientSystemManager.instance.IsFrameOpen<DungeonRollFrame>())
            {
                ClientSystemManager.instance.CloseFrame<DungeonRollFrame>();
                ClientSystemManager.instance.OpenFrame<DungeonRollResultFrame>(FrameLayer.Middle, uiEvent.Param1);
            }
        }
    }
}