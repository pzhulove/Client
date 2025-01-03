using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
///////删除linq
using ProtoTable;
using DG.Tweening;
using ActivityLimitTime;

namespace GameClient
{
    public enum BubbleShowType
    {
        Guild,
        Skill,
        EquipHandBook,
    }

    class ClientSystemTownFrame : ClientFrame
    {
        private bool mIsStopMoveFunction = false;
        private bool mLastJoyStickFizzyCheck = false;
        private Vector2 mLastJoyStickPosition = Vector2.zero;

        #region data
        float m_sin60 = 0.8660254f;
        float m_sin45 = 0.7071067f;
        float m_sin30 = 0.5f;

        //右下角的Tip(技能，图鉴）是否展示
        //public static bool IsShowingRightDownTip = false;
        public static bool IsShowSkillTips = false;
        public static bool IsShowGuildTips = false;
        public static bool IsShowEquipHandBookTips = false;
        static bool bGuardForNotify = false;
        float DayOnlineInterval = 0.0f;
        float PrivateCustomBubbleTime = 0.0f;

        bool isTownSceneLoadFinish = false; // 城镇场景是否加载完毕
        InputManager _inputManager;
        //TraceFrame m_kTraceFrame = new TraceFrame();
        ComTalk m_miniTalk;
        int ChangeJobSelectID = 0;   
        public const int huawei = 21100;
        public const int oppo = 21200;
        public const int vivo = 21300;
        public const int xiaomi = 21400;
        public const int meizu = 21500;
        public const string unLockEffect = "Effects/UI/Prefab/EffUI_xinxiaoxi";
        List<GameObject> unlockEffectList = new List<GameObject>();
        public const string unLockClickEffect = "Effects/Scene_effects/EffectUI/EffUI_ICON";
        List<GameObject> unLockClickEffects = new List<GameObject>();
        private bool bNeedSwitchToChijiPrepare = false; 
        [UIControl("left", typeof(ComFunction))]
        ComFunction comFuntion;

        #endregion

        #region init
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/MainFrameTown/MainTownFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            // OpenFrame只是做了一些界面控件初始化的一些功能,实际的内容初始化都是等待SceneLoadFinish后执行的
            base._OnOpenFrame();

            InitData();
            InitJoystick();
            InitTalk();
            InitFilters();
            UpdateMapTitle();
            
            if (null != comFuntion)
            {
                comFuntion.Initialize();
            }

            BindUIEvent();
            InitTAPGraduationEffect();
            isTownSceneLoadFinish = false;

            // 不管前面有没有清掉吃鸡技能数据，现在进入城镇的时候强制清一次，
            // 那么再进入pve或pvp的时候都不应该再出现吃鸡技能了，
            // 如果万一读的是吃鸡技能那也应该是进入战斗的以后看到的是空的技能栏，而不是看到上一把吃鸡带出来的技能
            SkillDataManager.GetInstance().ClearChijiSkill();
			CheckItemCountInAutoFightTest();
        }

        protected sealed override void _OnCloseFrame()
        {
            UnBindUIEvent();
            UnloadInput();
            ClearData();
            InitTAPGraduationEffect();


            if (null != comFuntion)
            {
                comFuntion.UnInitialize();
                comFuntion = null;
            }

            base._OnCloseFrame();
            InvokeMethod.RemovePerFrameCall(this);
            unlockEffectList.Clear();
            _ClearAdventureTeamFuncUnlockCoroutine();
            _ClearAdventurePassSeasonFuncUnlockCoroutine();

            // 主界面关闭了 应该把相关状态变量重置掉 add by qxy 2019-07-10
            IsShowSkillTips = false;
            IsShowGuildTips = false;
            IsShowEquipHandBookTips = false;
            isTownSceneLoadFinish = false;
            InvokeMethod.RemoveInvokeCall(PlayAdventurePassSeasonFuncUnlockAnim);
        }

        protected void ClearData()
        {
            mLastJoyStickFizzyCheck = false;
            mLastJoyStickPosition = Vector2.zero;
            mIsStopMoveFunction = false;

            DayOnlineInterval = 0.0f;
            PrivateCustomBubbleTime = 0.0f;

            if(_inputManager != null)
            {
                _inputManager = null;
            }

            if (m_miniTalk != null)
            {
                ComTalk.Recycle();
                m_miniTalk = null;
            }

            ChangeJobSelectID = 0;
            bNeedSwitchToChijiPrepare = false;
            
            if (unLockClickEffects!=null)
            {
                foreach (var item in unLockClickEffects)
                {
                    if (item != null)
                    {
                        GameObject.Destroy(item);
                    }
                }  
                unLockClickEffects.Clear();
            }
            
        }

        public void SceneLoadFinish()
        {
            OnSceneLoadFinish();

        }

        protected sealed override void OnSceneLoadFinish()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                Logger.LogErrorFormat("排查主界面角色信息未初始化原因1");
                return;
            }

            CitySceneTable TownTableData = TableManager.instance.GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (TownTableData == null)
            {
                Logger.LogErrorFormat("排查主界面角色信息未初始化原因2--CurrentSceneID = {0}", systemTown.CurrentSceneID);
                return;
            }

            systemTown.AddVoiceListenerForAllNpc();
            ClientSystemManager.GetInstance().bIsInPkWaitingRoom = false;

            if (TownTableData.SceneType == CitySceneTable.eSceneType.PK_PREPARE)
            {
                if (TownTableData.SceneSubType == CitySceneTable.eSceneSubType.TRADITION)
                {
                    UpdatePkWaitingRoom(TownTableData);
                }
                else if (TownTableData.SceneSubType == CitySceneTable.eSceneSubType.BUDO)
                {
                    UpdateBudo(TownTableData);
                }
                else if (TownTableData.SceneSubType == CitySceneTable.eSceneSubType.MoneyRewards)
                {
                    MoneyRewardsMainFrame.CommandOpen(new MoneyRewardsMainFrameData
                    {
                        citySceneItem = TownTableData,
                        CurrentSceneID = TownTableData.ID,
                        TargetTownSceneID = TownTableData.BirthCity,
                    });
                }
                else if (TownTableData.SceneSubType == CitySceneTable.eSceneSubType.GuildBattle || TownTableData.SceneSubType == CitySceneTable.eSceneSubType.CrossGuildBattle)
                {
                    UpdateGuildBattle();
                }
                else if(TownTableData.SceneSubType == CitySceneTable.eSceneSubType.Pk3v3)
                {
                    UpdatePk3v3WaitingRoom(TownTableData);
                }
                else if(TownTableData.SceneSubType == CitySceneTable.eSceneSubType.CrossPk3v3)
                {
                    UpdatePk3v3CrossWaitingRoom(TownTableData);
                }else if(TownTableData.SceneSubType==CitySceneTable.eSceneSubType.FairDuelPrepare)
                {
                    UpdateFairBattleWaitingRoom(TownTableData);
                    mIsCanShowGiftFrame = false;
                }
                else if (TownTableData.SceneSubType == CitySceneTable.eSceneSubType.Melee2v2Cross)
                {
                    UpdatePk2v2CrossWaitingRoom(TownTableData);
                }

                InitializeMainUI();
            }
			else if (TownTableData.SceneType == CitySceneTable.eSceneType.TEAMDUPLICATION)
            {
                if (TownTableData.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationBuid)
                {
                    UpdateTeamDuplicationBuildFrame(TownTableData);
                }
                else if (TownTableData.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationFight)
                {
                    UpdateTeamDuplicationFightFrame(TownTableData);
                }

                mIsCanShowGiftFrame = false;
                InitializeMainUI();
            }
            else if (TownTableData.SceneType == CitySceneTable.eSceneType.NORMAL && 
                     TownTableData.SceneSubType == CitySceneTable.eSceneSubType.Guild)
            {
                UpdateGuildArenaFrame(TownTableData);
                InitializeMainUI();
            }
            else if(TownTableData.SceneType == CitySceneTable.eSceneType.BATTLEPEPARE)
            {
                bNeedSwitchToChijiPrepare = true;
            }
            else
            {
                SetForbidFadeIn(false);
                FadeInSpecial(true);

                InitializeMainUI();

                if (ClientSystemManager.GetInstance().PreSystemType == typeof(ClientSystemLogin))
                {
#if UNITY_EDITOR || ROBOT_TEST
                    //添加跳过弹窗的开关
                    if (!Global.Settings.CloseLoginPushFrame)
                    {
#endif 
                        AdsPush.LoginPushManager.GetInstance().Callback = _StartOpenFollowingQueue;
                        AdsPush.LoginPushManager.GetInstance().TryOpenLoginPushFrame();
#if UNITY_EDITOR || ROBOT_TEST
                    }
#endif
                }

                if (ClientSystemManager.GetInstance().PreSystemType == typeof(ClientSystemBattle))
                {
#if APPLE_STORE
                    if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(IOSFuncSwitchTable.eType.LIMITTIME_GIFT) == false)
                    {
#endif
                        //add by mjx on 170827 for push vip frame to up fatigue
                        MonthCardTipManager.instance.TryOpenMonthCardTipFrameByCond(PlayerBaseData.GetInstance().RoleID);
#if APPLE_STORE
                    }
#endif
                }
            }

            _OnMailListReq();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SkillLvUpNoticeUpdate);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.FinancialPlanButtonUpdateByLogin);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SecurityLockApplyStateButton);

            // 刷新公会拍卖和公会世界拍卖入口icon
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnGuildDungeonAuctionStateUpdate);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnGuildDungeonWorldAuctionStateUpdate);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnGuildDungeonAuctionAddNewItem); // 刷新公会拍卖红点
            Utility.SetInitTownSystemState();

            //获取限时活动界面礼包信息
            ActivityManager.GetInstance().RequestGiftDatas();
            
            //有新的触发礼包 让malldatamanager去获取最新的触发礼包
            MallNewDataManager.GetInstance().GetTriggerGiftMallList();

            //语音功能登录
            if (ClientSystemManager.GetInstance().PreSystemType == typeof(ClientSystemLogin))
            {
                VoiceSDK.SDKVoiceManager.GetInstance().LoginVoice();
            }

            if (ClientSystemManager.GetInstance().PreSystemType == typeof(ClientSystemLogin))
            {
                if (!ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_SECURITY_LOCK))
                {
                    SecurityLockDataManager.GetInstance().SendWorldSecurityLockDataReq();
                }                    
            }

            if (ChijiDataManager.GetInstance().SwitchingPrepareToTown)
            {
                PlayerBaseData.GetInstance().bLevelUpChange = false;
                ChijiDataManager.GetInstance().SwitchingPrepareToTown = false;
            }
            mIsCanShowGiftFrame = true;
            //宠物推送
            if (ActivityLimitTimeCombineManager.GetInstance().GiftDataManager != null)
            {
                if (ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.PetPushFrameIsOpen)
                {
                    ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.OpenLimitTimePetGiftFrame(ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.GetPetPushItemInfo());
                    ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.PetPushFrameIsOpen = false;
                }
            }

            TopUpPushDataManager.GetInstance().SendWorldGetRechargePushItemsReq();            

            //请求刷新月卡翻牌奖励数据
            MonthCardRewardLockersDataManager.GetInstance().ReqMonthCardRewardLockersItems();

            //佣兵远征 请求地图状态
            if (AdventureTeamDataManager.GetInstance().BFuncOpened)
            {
                AdventureTeamDataManager.GetInstance().ReqExpeditionAllMapInfo();
                AdventureTeamDataManager.GetInstance().ReqGetAllExpeditionMaps();                
            }

            //刷新每日必做本地数据状态
            DailyTodoDataManager.GetInstance().ReqDailyTodoFunctionState();
            GuildDataManager.GetInstance().RequestGuildReceiveMergeRequest();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TimeLessItemsChanged);
            ActivityDataManager.GetInstance().SendMonthlySignInQuery();
            // 检查货币重置
            if (AdsPush.LoginPushManager.GetInstance().IsFirstLogin())
            {
                DeadLineReminderDataManager.GetInstance().CheckCurrencyDeadlineStatus();
            }
            AdventurerPassCardDataManager.GetInstance().SendWorldAventurePassStatusReq();   // 请求冒险者通行证数据
            AdventurerPassCardDataManager.GetInstance().SendWorldAventurePassExpPackReq(0); // 查询下经验包领取状态

            WarriorRecruitDataManager.GetInstance().SendWorldQueryHireCoinReq();//请求招募硬币
            WarriorRecruitDataManager.GetInstance().SendWorldQueryHirePushReq(0);//查询勇士招募是否推送过了

            SkillDataManager.GetInstance().UpdateSkillLevelAddInfo();

            isTownSceneLoadFinish = true;
        }

        public void SetJoystickAfterSetting()
        {
            if (_inputManager == null)
                return;

            if (_inputManager.joystickMode != SettingManager.GetInstance().GetJoystickMode())
                ReloadJoystick();

        }

        void InitData()
        {
            // 这里可以加一些表格数据的初始化，比如需要拿系统数值表的一些数据，可以在这里加代码
            mLastJoyStickPosition = Vector2.zero;
            mLastJoyStickFizzyCheck = false;
            mIsStopMoveFunction = false;
        }

        void InitJoystick()
        {
            _inputManager = new InputManager();
            _inputManager.LoadJoystick(SettingManager.GetInstance().GetJoystickMode());
            
            GameObject joyStick = _inputManager.GetJoyStick();
            if (joyStick != null)
            {
                Utility.AttachTo(joyStick, GameClient.ClientSystemManager.instance.BottomLayer);
                joyStick.transform.SetAsFirstSibling();
            }
            
            _inputManager.SetJoyStickMoveCallback(_OnJoyStickMove);
            _inputManager.SetJoyStickMoveEndCallback(_OnJoyStickStop);
        }

        public void StopMainPlayerMoveAndStopFizzyCheck()
        {
            _OnJoyStickStop();
            mLastJoyStickFizzyCheck = false;
            mIsStopMoveFunction = true;
        }

        public void StartFizzyCheckAndResumeJoystickMove()
        {
            mIsStopMoveFunction = false;
            mLastJoyStickFizzyCheck = true;
        }

        void ReloadJoystick()
        {
            UnloadInput();
            InitJoystick();
        }

        void InitTalk()
        {
            m_miniTalk = ComTalk.Create(mTalkRoot);
        }

        void InitFilters()
        {
            string[] contents = new string[(int)ChatType.CT_MAX_WORDS]
            {
                MainUIIconPath.talkTabs + "/TabC",
                "",
                "",
                MainUIIconPath.talkTabs + "/TabB",
                MainUIIconPath.talkTabs + "/TabA",
                MainUIIconPath.talkTabs + "/TabD",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
            };

            for (int i = 0; i < contents.Length; ++i)
            {
                if (!string.IsNullOrEmpty(contents[i]))
                {
                    Toggle toggle = Utility.FindComponent<Toggle>(frame, contents[i]);

                    toggle.isOn = SystemConfigManager.GetInstance().IsChatToggleOn((ChatType)i);
                    ChatType eChatType = (ChatType)i;
                    GameObject goCheckMark = Utility.FindChild(toggle.gameObject, "CheckMark");
                    goCheckMark.CustomActive(toggle.isOn);

                    toggle.onValueChanged.AddListener((bool bValue) =>
                    {
                        SystemConfigManager.GetInstance().SetChatToggle(eChatType, bValue);
                        goCheckMark.CustomActive(bValue);
                    });
                }
            }
        }

        void InitializeMainUI()
        {
            UpdateShowButton(); 
            UpdateUplevelGiftText();
            UpdateDayOnlineGiftText();
            UpdateDuelLockTip();
            UpdateDuelOrChangeJobBtn();
            UpdateChallengeButton();
            InitRedPoint();

            EGuildBattleState state = GuildDataManager.GetInstance().GetGuildBattleState();
            UpdateGuildBattle(state, state);

            UpdatePay(false);
            _InitMagicJar();

            UpdateMallGiftNotice();

            InitMainTownFrameCommonButtonControl();

            //added by mjx on 170814
            UpdateSDKBindPhoneBtn();
			InitLimitTimeActivity();
           	InitHaveGoodsRecommend();

            //夺宝活动UI显示(活动按钮、开奖提示)
            InitTreasureLotteryActivityUI();

	        InitHorseGambling();

            _InitRandomTreasure();
  
			_InitStrengthenTicketMerge();
     
            UpdateShowChannelRankListButton();  
            UpdateNewYearRedPackButton();
            UpdateChijiButton();
            UpdateTreasureConnvertButton();
            UpdateAdventurePassCardButton();
            _ShowAdventurePassCardEndTip();
            //展示触发礼包
            _ShowTriggerGift();
            if (GeGraphicSetting.instance.isModified)
            {
                SystemNotifyManager.SystemNotify(8523);
                GeGraphicSetting.instance.isModified = false;
            }
            _RefreshHaveLevelPermenentBtn();

            UpdateTopRightState(PlayerBaseData.GetInstance().IsExpand);

            if (mTimePlayBtn != null)
            {
                mTimePlayBtn.InitializeMainUI();
            }

            // 刷新城镇buf按钮状态
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUpdateTownBuf);

#if APPLE_STORE
			//add by mjx for appstore 
            TryHideRankListAtFirst();
#endif

#if MG_TEST || MG_TEST2
            PluginManager.GetInstance().SetBuglyVerIdInfo("Town");           
#endif
        }

        private void InitMainTownFrameCommonButtonControl()
        {
            if (mMainTownFrameCommonButtonControl != null)
                mMainTownFrameCommonButtonControl.UpdateMainTownFrameCommonButtonControl();
        }

        void InitRedPoint()
        {
            if (ItemDataManager.GetInstance().IsPackageFull())
            {
                mPackageFull.gameObject.SetActive(true);
                mPackageAnim.DORestart();
            }
            else
            {
                mPackageFull.gameObject.SetActive(false);
                mPackageAnim.DOPause();
                mPackageAnim.gameObject.transform.localRotation = Quaternion.identity;
            }        

            mPackageRedPoint.gameObject.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.PackageMain));
            //mDuelRedPoint.gameObject.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.DailyProve));
            mGuildRedPoint.gameObject.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildMain));
            mSkillRedPoint.gameObject.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.Skill));
            
            mActivityLimitTimeRedPoint.CustomActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.ActivityLimitTime));
            mJarRedPoint.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.Jar));            

            //mChapterReawrdRedPoint.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.ChapterReward));         

            UpdateMissionRedPoint();
            UpdateRedPacket();

            _updateDailyRedPoint();

            //             m_objMagicJarRedPoint.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.MagicJar));
            //             m_objGoldJarRedPoint.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GoldJar));

        }

        void UpdateShowChannelRankListButton()
        {
            mRankallRoot.CustomActive(_isChanneRankBtn());
        }      

        void UpdateNewYearRedPackButton()
        {
            mRedPackRankListObj.CustomActive(RedPackDataManager.GetInstance().CheckNewYearActivityOpen());
        }

        private void UpdateChijiButton()
        {
            mChiji.CustomActive(ChijiDataManager.GetInstance().MainFrameChijiButtonIsShow());
        }

        private void UpdateTreasureConnvertButton()
        {
            mTreasureConversion.CustomActive(BeadCardManager.GetInstance().CheckTreasureConvertActivityOpon());
        }

        private void UpdateAdventurePassCardButton()
        {       
            adventurerPassCard.CustomActive(AdventurerPassCardDataManager.GetInstance().CardLv > 0);
        }

        //判断是否显示通行证倒计时
        private void _ShowAdventurePassCardEndTip()
        {
            if (AdventurerPassCardDataManager.GetInstance().CardLv > 0)// && Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.AdventurePassSeason))
            {
                int leftTime = AdventurerPassCardDataManager.GetInstance().GetSeasonLeftTime();
                int tipMaxDay = Utility.GetClientIntValue(ClientConstValueTable.eKey.ADVENTURE_PASS_CARD_END_TIP_TIME, 5);
                int seasonId = (int)AdventurerPassCardDataManager.GetInstance().SeasonID;
                int prefsDay = PlayerPrefsManager.GetInstance().GetAccTypeKeyIntValue(PlayerPrefsManager.PlayerPrefsKeyType.AdventurePassCardEndeTip, seasonId);
                if (leftTime > 0 && leftTime <= tipMaxDay * 24 * 3600 && prefsDay != (leftTime / (24 * 3600) + 1))
                {
                    PlayerPrefsManager.GetInstance().SetAccTypeKeyIntValue(PlayerPrefsManager.PlayerPrefsKeyType.AdventurePassCardEndeTip, (leftTime / (24 * 3600) + 1), seasonId);
                    ClientSystemManager.GetInstance().OpenFrame<AdventurerPassCardEndTipFrame>();
                }
            }
        }
        private void _updateDailyRedPoint()
        {
            mDailyRedPoint.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.ActivityDungeon));

            mDailyRedPointCount.text = _getDailyRedPointCountString();
        }

        private string _getDailyRedPointCountString()
        {
            int cnt = 0;

            cnt += MissionDailyFrame.GetRedPointCount();
            cnt += ActivityDungeonDataManager.GetInstance().GetRedCountByActivityType(ActivityDungeonTable.eActivityType.TimeLimit);
            cnt += ActivityDungeonDataManager.GetInstance().GetRedCountByActivityType(ActivityDungeonTable.eActivityType.Daily);
            return cnt.ToString();
        }

        //private void onUpdatePayText(UIEvent iEvent)
        //{
        //    _onUpdatePaytext();
        //}
        //private void _onUpdatePaytext()
        //{
        //    if (PayManager.GetInstance().HasSecondPay())
        //    {
        //        mpayText.text = "首充";
                
        //    }
        //    else
        //    {
        //        mpayText.text = "充值";
        //        mFirstRecharge.gameObject.CustomActive(false);
        //    }
        //}
#endregion

#region event
        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SceneChangedFinish, OnSceneChangedFinish);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SwitchToMainScene, OnSwitchToMainScene);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SystemChanged, OnSystemSwitchFinished);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPointChanged, OnRedPointChanged);      
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.LevelChanged, OnLevelChanged);             
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.JobIDChanged, OnJobIDChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.JobIDReset, OnJobIDReset);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UpdateUnlockFunc, OnUpdateUnlockFunc);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.NewFuncUnlock, OnNewFuncUnlock);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.NewAccountFuncUnlock, OnNewAccountFuncUnlock);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.NextFuncOpen, OnNextFuncOpen);    
  
        
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PackageFull, OnPackageFullUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PackageNotFull, OnPackageFullUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPacketOpenSuccess, OnRedPacketOpenSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPacketGet, OnNewRedPacketGet);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPacketDelete, OnDeleteRedPacket);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnPayResultNotify, OnPay);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildBattleStateChanged, OnGuildBattleStateChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.WelfareFrameClose, OnUpdateUplevelGift);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityUpdate, OnActivityUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UplevelFrameClose, OnSkillLearnTips);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FadeOutOver, OnFadeOutOver);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMonthCardUpdate, OnMonthCardUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChangeJobSelectDialog, OnChangeJobSelectDialog);
			//added by mjx on 170814
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SDKBindPhoneFinished, OnSDKBindPhoneFinished);
			UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ShowLimitTimeActivityBtn, ShowLimitTimeActivity);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuankaFrameOpen, OnGunakaFrameOpen);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildMainFrameClose, OnCloseGuildMainFrame);
 
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUpdatePayText, onUpdatePayText);                   
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GoodsRecommend, OnPrivateOrderingNoticeUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CurGuideStart, OnNewbieGuideStart);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityLimitTimeUpdate, OnUpdateActivityLimitTimeState);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityLimitTimeTaskUpdate, OnUpdateActivityLimitTimeState);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TreasureLotterySyncDraw, OnUpdateActivityTreasureLottery);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TreasureLotteryStatusChange, OnUpdateActivityTreasureLottery);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RightLowerBubblePlayAnimation, mRightLowerBubblePlayAnimation);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnPlayerFunctionUnlockAnimation, OnPlayerFunctionUnlockAnimation);
	        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.HorseGamblingStateUpdate, OnUpdateHorseGambling);
      
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTreasureFuncSwitch, _OnRandomTreasureFuncChange);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTAPGraduationSuccess, _OnTAPGraduationSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnStrengthenTicketMergeStateUpdate, _OnStrengthTicketMergeStateUpdate);
          
          
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildDungeonSyncActivityState, _OnSyncActivityState);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TopUpPushButoonOpen, _OnTopUpPushButoonOpen);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TopUpPushButtonClose, _OnTopUpPushButoonClose);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnGuildDungeonAuctionStateUpdate, _OnGuildDungeonAuctionStateUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnGuildDungeonWorldAuctionStateUpdate, _OnGuildDungeonWorldAuctionStateUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnGuildDungeonAuctionAddNewItem, _OnGuildDungeonAuctionAddNewItem);       

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.NotifyShowAdventureTeamUnlockAnim, _OnNotifyShowAdventureTeamUnlockAnim);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.NotifyShowAdventurePassSeasonUnlockAnim, _OnNotifyShowAdventurePassSeasonUnlockAnim);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.NotifyOpenWelfareFrame, _OnNotifyOpenWelfareFrame);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UpdateAventurePassStatus, _OnUpdateAventurePassStatus);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UpdateAventurePassButtonRedPoint, _OnUpdateAventurePassButtonRedPoint);

            //商城如果获取触发礼包 需要显示出来
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSyncWorldMallQueryItems, _OnSetTriggerMallBtn);

 

            onFadeInEnd += FuncUnlockNotify;
            onFadeInEnd += OpenComTalk;
            SystemConfigManager.GetInstance().onChatFilterChanged += OnChatFilterChanged;

            ItemDataManager.GetInstance().onAddNewItem += OnAddNewItem;

            MissionManager.GetInstance().onAddNewMission += OnAddNewMission;
            MissionManager.GetInstance().onUpdateMission += OnUpdateMission;
            MissionManager.GetInstance().onDeleteMission += OnDeleteMission;
            MissionManager.GetInstance().missionChangedDelegate += OnMissionChanged;
            MissionManager.GetInstance().onChestIdsChanged += _OnChestIdsChanged;

            ActiveManager.GetInstance().onActivityUpdate += _OnActivityUpdate;
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SceneChangedFinish, OnSceneChangedFinish);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SwitchToMainScene, OnSwitchToMainScene);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SystemChanged, OnSystemSwitchFinished);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPointChanged, OnRedPointChanged);   
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.LevelChanged, OnLevelChanged);      
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.JobIDChanged, OnJobIDChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.JobIDReset, OnJobIDReset);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UpdateUnlockFunc, OnUpdateUnlockFunc);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.NewFuncUnlock, OnNewFuncUnlock);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.NewAccountFuncUnlock, OnNewAccountFuncUnlock);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.NextFuncOpen, OnNextFuncOpen);       
         

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PackageFull, OnPackageFullUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PackageNotFull, OnPackageFullUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPacketOpenSuccess, OnRedPacketOpenSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPacketGet, OnNewRedPacketGet);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPacketDelete, OnDeleteRedPacket);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnPayResultNotify, OnPay);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildBattleStateChanged, OnGuildBattleStateChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.WelfareFrameClose, OnUpdateUplevelGift);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityUpdate, OnActivityUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UplevelFrameClose, OnSkillLearnTips);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FadeOutOver, OnFadeOutOver);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMonthCardUpdate,OnMonthCardUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ChangeJobSelectDialog, OnChangeJobSelectDialog);
			  //added by mjx on 170814
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SDKBindPhoneFinished,OnSDKBindPhoneFinished);
			UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ShowLimitTimeActivityBtn, ShowLimitTimeActivity);
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RefreshLimitTimeActivityIcon, UpdateLimitTimeActIcon);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuankaFrameOpen, OnGunakaFrameOpen);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildMainFrameClose, OnCloseGuildMainFrame);
   
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUpdatePayText, onUpdatePayText);                       
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GoodsRecommend, OnPrivateOrderingNoticeUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CurGuideStart, OnNewbieGuideStart);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityLimitTimeUpdate, OnUpdateActivityLimitTimeState);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityLimitTimeTaskUpdate, OnUpdateActivityLimitTimeState);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TreasureLotterySyncDraw, OnUpdateActivityTreasureLottery);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TreasureLotteryStatusChange, OnUpdateActivityTreasureLottery);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RightLowerBubblePlayAnimation, mRightLowerBubblePlayAnimation);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnPlayerFunctionUnlockAnimation, OnPlayerFunctionUnlockAnimation);
	        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.HorseGamblingStateUpdate, OnUpdateHorseGambling);
     
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTreasureFuncSwitch, _OnRandomTreasureFuncChange);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTAPGraduationSuccess, _OnTAPGraduationSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnStrengthenTicketMergeStateUpdate, _OnStrengthTicketMergeStateUpdate);
       
         
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildDungeonSyncActivityState, _OnSyncActivityState);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TopUpPushButoonOpen, _OnTopUpPushButoonOpen);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TopUpPushButtonClose, _OnTopUpPushButoonClose);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnGuildDungeonAuctionStateUpdate, _OnGuildDungeonAuctionStateUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnGuildDungeonWorldAuctionStateUpdate, _OnGuildDungeonWorldAuctionStateUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnGuildDungeonAuctionAddNewItem, _OnGuildDungeonAuctionAddNewItem);        

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.NotifyShowAdventureTeamUnlockAnim, _OnNotifyShowAdventureTeamUnlockAnim);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.NotifyShowAdventurePassSeasonUnlockAnim, _OnNotifyShowAdventurePassSeasonUnlockAnim);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.NotifyOpenWelfareFrame, _OnNotifyOpenWelfareFrame);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UpdateAventurePassStatus, _OnUpdateAventurePassStatus);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UpdateAventurePassButtonRedPoint, _OnUpdateAventurePassButtonRedPoint);     
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSyncWorldMallQueryItems, _OnSetTriggerMallBtn);

            onFadeInEnd -= FuncUnlockNotify;
            onFadeInEnd -= OpenComTalk;

            SystemConfigManager.GetInstance().onChatFilterChanged -= OnChatFilterChanged;
            ItemDataManager.GetInstance().onAddNewItem -= OnAddNewItem;

            MissionManager.GetInstance().onAddNewMission -= OnAddNewMission;
            MissionManager.GetInstance().onUpdateMission -= OnUpdateMission;
            MissionManager.GetInstance().onDeleteMission -= OnDeleteMission;
            MissionManager.GetInstance().missionChangedDelegate -= OnMissionChanged;
            MissionManager.GetInstance().onChestIdsChanged -= _OnChestIdsChanged;

            ActiveManager.GetInstance().onActivityUpdate -= _OnActivityUpdate;
        }

        void UnloadInput()
        {
            if (_inputManager != null)
            {
                _inputManager.Unload();
                _inputManager = null;
            }
        }

		void ReloadInput()
		{
			UnloadInput();
			InitJoystick();
		}

        void OnSceneChangedFinish(UIEvent iEvent)
        {
            UpdateRedPacket();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.NewMailNotify);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityNoticeUpdate);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamNewInviteNoticeUpdate);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SkillLvUpNoticeUpdate);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.FriendRequestNoticeUpdate);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildInviteNoticeUpdate);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.FinancialPlanButtonUpdateBySceneChanged);
            var state = (byte)Pk3v3CrossDataManager.GetInstance().Get3v3CrossWarStatus();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PK3V3CrossButton, state);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.HasLimitTimeGiftToBuy);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3InviteRoomListUpdate,(byte)RoomType.ROOM_TYPE_THREE_FREE);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3InviteRoomListUpdate, (byte)RoomType.ROOM_TYPE_THREE_SCORE_WAR);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMoneyRewardsStatusChanged);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TimeLessItemsChanged);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SecurityLockApplyStateButton);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationOwnerNewRequesterMessage);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationOwnerNewTeamInviteMessage);

            //刷新成就界面红点
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAchievementScoreChanged, PlayerBaseData.GetInstance().AchievementScore,
                PlayerBaseData.GetInstance().AchievementScore);

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown != null)
            {
                CitySceneTable sceneData = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                if (sceneData == null)
                {
                    return;
                }

                //TODO
                // 城镇与城镇之间的切场景,从单局退出到城镇,以及从登陆进入到城镇也都会触发的
                VoiceSDK.SDKVoiceManager.GetInstance().LeaveChatRoom(ChatType.CT_NORMAL);
                
                if (sceneData.SceneType == CitySceneTable.eSceneType.NORMAL || sceneData.SceneType == CitySceneTable.eSceneType.PK_PREPARE)
                {
                    VoiceSDK.SDKVoiceManager.GetInstance().JoinChatRoom(ChatType.CT_NORMAL);
                }
            }

            UpdateMapTitle();
        }

        void OnSwitchToMainScene(UIEvent iEvent)
        {
            UpdateGuildEffect();
            UpdateGuildTips();
            UpdateMapTitle();
        }

        void OnSystemSwitchFinished(UIEvent iEvent)
        {
            if(bNeedSwitchToChijiPrepare)
            {
                bNeedSwitchToChijiPrepare = false;

                ChijiDataManager.GetInstance().SwitchingTownToPrepare = true;
                Utility.SwitchToChijiWaittingRoom();
            }

            UpdateMapTitle();
        }

        void OnRedPointChanged(UIEvent a_event)
        {
            ERedPoint redPointType = (ERedPoint)a_event.Param1;

            if (redPointType == ERedPoint.ChapterReward)
            {
                //mChapterReawrdRedPoint.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.ChapterReward));
            }
            else if (redPointType == ERedPoint.Skill)
            {
                mSkillRedPoint.gameObject.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.Skill));
            }
            else if (redPointType == ERedPoint.ActivityDungeon)
            {
                _updateDailyRedPoint();
            }                  
            else if(redPointType >= ERedPoint.GuildMain && redPointType < ERedPoint.Skill)
            {
                mGuildRedPoint.gameObject.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildMain));
            }
            else if(redPointType == ERedPoint.PackageMain)
            {
                mPackageRedPoint.gameObject.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.PackageMain));
            }            
            else if(redPointType == ERedPoint.Jar)
            {
                mJarRedPoint.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.Jar));
            }
            else
            {
                if (mSDKBindPhoneBtnRedPoint)
                    mSDKBindPhoneBtnRedPoint.CustomActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.SDKBindPhone));
            }
        }        

        void OnLevelChanged(UIEvent uiEvent)
        {
            ClientSystemTown currentSystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;

            if(currentSystem == null)
            {
                return;
            }

            if (currentSystem.MainPlayer != null)
            {
                currentSystem.MainPlayer.SetPlayerRoleLv(PlayerBaseData.GetInstance().Level);
            }

            // 吃鸡只处理上面的等级显示，其他一概不处理,否则一个1级角色去玩吃鸡升到60级，把所有功能给解锁了，返回主城又没有重新隐藏
            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(currentSystem.CurrentSceneID);
            if (scenedata == null)
            {
                return;
            }

            if (scenedata.SceneSubType == CitySceneTable.eSceneSubType.Battle || scenedata.SceneSubType == CitySceneTable.eSceneSubType.BattlePrepare)
            {
                return;
            }

            UpdateDuelLockTip();
            UpdateDuelOrChangeJobBtn();
            UpdateChallengeButton();
            _InitMagicJar();
            UpdateShowChannelRankListButton();
            UpdateNewYearRedPackButton();
            UpdateTreasureConnvertButton();
            //ClientSystemTown._OpenChangeJobTip();
            //ClientSystemTown._OpenAwakeTip();
            UpdateSDKBindPhoneBtn();
            InitLimitTimeActivity();
            _UpdateRandomTreasure();
            InitHorseGambling();
			_UpdateStrengthenTicketMerge();
            //升级
            SDKInterface.Instance.UpdateRoleInfo(
                2, ClientApplication.adminServer.id, ClientApplication.adminServer.name,
                PlayerBaseData.GetInstance().RoleID.ToString(), 
                PlayerBaseData.GetInstance().Name,
                PlayerBaseData.GetInstance().JobTableID, PlayerBaseData.GetInstance().Level, PlayerBaseData.GetInstance().VipLevel,
                (int)PlayerBaseData.GetInstance().Ticket);

            //英雄等级改变的时候，需要更新背包的红点
            mPackageRedPoint.gameObject.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.PackageMain));

            if (PlayerBaseData.GetInstance().Level >= 60)
            {
                ActivityManager.GetInstance().UpdateFlyingGiftPackActivity(5004);
            }
            int limitLv = 0;
            int.TryParse(TR.Value("ConsumeRebateLimitPlayerGrade"),out limitLv);
            if(PlayerBaseData.GetInstance().Level>= limitLv)
            {
                ActivityManager.GetInstance().UpdateConsumeRebateActivity(1540);
            }

            int limitlevel = 0;
            int.TryParse(TR.Value("SpringFestivalRedEnvelopeRainLimitPlayerGrade"), out limitlevel);
            if (PlayerBaseData.GetInstance().Level >= limitlevel)
            {
                ActivityManager.GetInstance().UpdateSpringFestivalRedEnvelopeRainActivity(1585);
            }
            
            if (PlayerBaseData.GetInstance().Level < 5)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(string.Format(TR.Value("congratulations_lv_up_to"),PlayerBaseData.GetInstance().Level));
            }
        }
       
        void OnJobIDChanged(UIEvent uiEvent)
        {
            _OnShowChangeJobDialog();
            UpdateDuelOrChangeJobBtn();
        }

        void OnJobIDReset(UIEvent uiEvent)
        {
            
        }

        void OnUpdateUnlockFunc(UIEvent iEvent)
        {
            //UpdateShowButton();
        }

        void OnNewFuncUnlock(UIEvent iEvent)
        {
            byte iTableID = (byte)iEvent.Param1;

            FunctionUnLock FunctionUnLockData = TableManager.GetInstance().GetTableItem<FunctionUnLock>((int)iTableID);

            if(FunctionUnLockData == null)
            {
                return;
            }

            if (FunctionUnLockData.OpenArea == 1)
            {
                //是否打开该解锁功能折叠区域.
                if (FunctionUnLockData.LocationType == FunctionUnLock.eLocationType.BottomRightExpand)
                {
                    if (ClientSystemManager.instance.IsFrameOpen<BottomRightCornerExpandFrame>())
                    {
                        ClientSystemManager.instance.CloseFrame<BottomRightCornerExpandFrame>();
                    }
                    else
                    {
                        ClientSystemManager.instance.OpenFrame<BottomRightCornerExpandFrame>();
                    }
                }

                if (FunctionUnLockData.LocationType == FunctionUnLock.eLocationType.TopLeftExpand)
                {
                    if(ClientSystemManager.instance.IsFrameOpen<TopLeftCornerExpandFrame>())
                    {
                        ClientSystemManager.instance.CloseFrame<TopLeftCornerExpandFrame>();
                    }
                    else
                    {
                        ClientSystemManager.instance.OpenFrame<TopLeftCornerExpandFrame>();
                    }
                }
            }
            
            if (FunctionUnLockData.TargetBtnPos == "" || FunctionUnLockData.TargetBtnPos == "-")
            {
                return;
            }

            GameObject buttonObj = Utility.FindGameObject(frame, FunctionUnLockData.TargetBtnPos);
            if (buttonObj == null)
            {
                return;
            }

            if (FunctionUnLockData.ExpandType == FunctionUnLock.eExpandType.ET_TopRight)
            {
                ComTopButtonExpandController control = GetComTopButtonExpandController();
                if (control == null)
                {
                    return;
                }

                if (!control.IsExpand())
                {
                    control.MainButtonState();
                    return;
                }
            }

            GameObject go = AssetLoader.instance.LoadResAsGameObject(unLockEffect);
            if (go != null)
            {
                Utility.AttachTo(go, buttonObj);
                //unlockEffectList.Add(go);
            }

            //添加新解锁功能特效,点击删除.
            if (FunctionUnLockData.ShowOpenEffect == 1)
            {
                AddUnlockClickEffect(buttonObj);    
            }
            
            if (FunctionUnLockData.FuncType == FunctionUnLock.eFuncType.FashionMerge )
            {
                if (ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_FASHION_MERGO) == false)
                {
                    buttonObj.SetActive(true);
                }
                else
                {
                    buttonObj.SetActive(false);
                }
            }
            else
            {
                buttonObj.SetActive(true);
            }

#if APPLE_STORE
			 //add by mjx for ios appstore
             if (CheckIsFunctionClose(FunctionUnLockData))
             {
                 buttonObj.SetActive(false);
                 return;
			 }
			 buttonObj.SetActive(true);
#endif

            {
                ComTopButtonExpandController control = GetComTopButtonExpandController();
                if (control != null)
                {
                    control.UpdateShowHideBtnState();
                    return;
                }
            }
        }

        void AddUnlockClickEffect(GameObject buttonObj)
        {
            var com = buttonObj.GetComponent<ComAttachEffect>();
            if (com == null)
            {
                buttonObj.AddComponent<ComAttachEffect>();
                var comEffect = buttonObj.GetComponent<ComAttachEffect>();
                if (comEffect != null)
                {
                    comEffect.effectPath = unLockClickEffect;
                    var effectObj = comEffect.AddEffect();
                    if (effectObj != null)
                    {
                        unLockClickEffects.Add(effectObj);
                    }
                }
            }
        }

        void OnPlayerFunctionUnlockAnimation(UIEvent iEvent)
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<FunctionUnlockFrame>())
            {
                return;
            }

            if (!Global.Settings.isAnimationInto)
            {
                return;
            }

            // 如果是使用秘药飞升升级的，那么就不弹各种升级表现了
            if(PlayerBaseData.GetInstance().IsFlyUpState)
            {
                return;
            }

            GameFrameWork.instance.StartCoroutine(_NewFuncUnlockPlayStart());
        }
        

        bool SevenDaysButtonIsShow()
        {
#if APPLE_STORE
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.SEVEN_AWARDS))
            {
                return false;
            }
#endif
            return SevendaysDataManager.GetInstance().IsSevenDaysActiveOpen();

            
        }
        IEnumerator _NewFuncUnlockPlayStart()
        {
            if (PlayerBaseData.GetInstance().NewUnlockFuncList.Count <= 0)
            {
                yield break;
            }

            FunctionUnLock FunctionUnLockData = TableManager.GetInstance().GetTableItem<FunctionUnLock>(PlayerBaseData.GetInstance().NewUnlockFuncList[0]);

            if (FunctionUnLockData.FuncType == FunctionUnLock.eFuncType.ActivitySevenDays)
            {
                if (!(Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.ActivitySevenDays) && SevenDaysButtonIsShow()))
                {
                    PlayerBaseData.GetInstance().NewUnlockFuncList.RemoveAt(0);
                    GameFrameWork.instance.StartCoroutine(_NewFuncUnlockPlayStart());
                    yield break;
                }
            }

            if (FunctionUnLockData.bPlayAnim == 0)
            {
                PlayerBaseData.GetInstance().NewUnlockFuncList.RemoveAt(0);
                GameFrameWork.instance.StartCoroutine(_NewFuncUnlockPlayStart());
                yield break;
            }

            if (FunctionUnLockData.TargetBtnPos == "" || FunctionUnLockData.TargetBtnPos == "-")
            {
                PlayerBaseData.GetInstance().NewUnlockFuncList.RemoveAt(0);
                GameFrameWork.instance.StartCoroutine(_NewFuncUnlockPlayStart());
                yield break;
            }

            GameObject buttonObj = Utility.FindGameObject(frame, FunctionUnLockData.TargetBtnPos);
            if (buttonObj == null)
            {
                PlayerBaseData.GetInstance().NewUnlockFuncList.RemoveAt(0);
                GameFrameWork.instance.StartCoroutine(_NewFuncUnlockPlayStart());
                yield break;
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.NewFuncFrameOpen);

            if (FunctionUnLockData.bShowBtn == 0)
            {
                Image[] images = buttonObj.GetComponentsInChildren<Image>(true);
                for (int i = 0; i < images.Length; i++)
                {
                    Color clo = images[i].color;
                    clo.a = 0;
                    images[i].color = clo;
                }

                Text[] texts = buttonObj.GetComponentsInChildren<Text>();
                for (int i = 0; i < texts.Length; i++)
                {
                    texts[i].enabled = false;
                }
            }

            buttonObj.SetActive(true);

            yield return new WaitForEndOfFrame();

            UnlockData data = new UnlockData();
            data.FuncUnlockID = PlayerBaseData.GetInstance().NewUnlockFuncList[0];
            data.pos = buttonObj.transform.position;

            FunctionUnlockFrame UnlockFrame = ClientSystemManager.GetInstance().OpenFrame<FunctionUnlockFrame>(FrameLayer.Top, data) as FunctionUnlockFrame;
            UnlockFrame.ResPlayEnd = _NewFuncUnlockPlayEnd;
        }

        void _NewFuncUnlockPlayEnd()
        {
            if (PlayerBaseData.GetInstance().NewUnlockFuncList.Count <= 0)
            {
                return;
            }

            FunctionUnLock FunctionUnLockData = TableManager.GetInstance().GetTableItem<FunctionUnLock>(PlayerBaseData.GetInstance().NewUnlockFuncList[0]);

            if (FunctionUnLockData.TargetBtnPos == "" || FunctionUnLockData.TargetBtnPos == "-")
            {
                PlayerBaseData.GetInstance().NewUnlockFuncList.RemoveAt(0);
                return;
            }

            GameObject buttonObj = Utility.FindGameObject(frame, FunctionUnLockData.TargetBtnPos);
            if (buttonObj == null)
            {
                PlayerBaseData.GetInstance().NewUnlockFuncList.RemoveAt(0);
                return;
            }

            Image[] images = buttonObj.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i].name == buttonObj.name)
                {
                    continue;
                }

                Color clo = images[i].color;
                clo.a = 255;
                images[i].color = clo;
            }

            Text[] texts = buttonObj.GetComponentsInChildren<Text>(true);
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].enabled = true;
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<FunctionUnlockFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<FunctionUnlockFrame>();
            }

            PlayerBaseData.GetInstance().NewUnlockFuncList.RemoveAt(0);

            GameFrameWork.instance.StartCoroutine(_NewFuncUnlockPlayStart());
        }

        void OnNextFuncOpen(UIEvent uiEvent)
        {
            
            var nextOpenList = PlayerBaseData.GetInstance().NextUnlockFunc;
            

            if (null == nextOpenList || nextOpenList.Count <= 0)
            {
                mNextopen.gameObject.SetActive(false);
                return;
            }
            
            FunctionUnLock FunctionUnLockData = TableManager.GetInstance().GetTableItem<FunctionUnLock>(nextOpenList[0]);

            if (FunctionUnLockData == null)
            {
                mNextopen.gameObject.SetActive(false);
                return;
            }

            mNextopen.gameObject.SetActive(true);

            mNextOpenName.text = FunctionUnLockData.Name;
            mNextOpenLv.text = string.Format("{0}级", FunctionUnLockData.FinishLevel);           

            if (FunctionUnLockData.IconPath != "" && FunctionUnLockData.IconPath != "-")
            {
#if APPLE_STORE
                //add by mjx for ios appstore
                if (CheckIsFunctionClose(FunctionUnLockData))
                {
                    mNextopen.gameObject.CustomActive(false);
                    return;
                }
#endif
                Sprite Icon = AssetLoader.instance.LoadRes(FunctionUnLockData.IconPath, typeof(Sprite)).obj as Sprite;

                if (Icon != null)
                {
                    // mNextOpenIcon.sprite = Icon;
                    ETCImageLoader.LoadSprite(ref mNextOpenIcon, FunctionUnLockData.IconPath);

                    mNextopen.onClick.RemoveAllListeners();
                    mNextopen.onClick.AddListener(() =>
                    {
                        if (ClientSystemManager.instance.IsFrameOpen<NextOpenShowFrame>())
                        {
                            ClientSystemManager.instance.CloseFrame<NextOpenShowFrame>();
                        }

                        ClientSystemManager.instance.OpenFrame<NextOpenShowFrame>(FrameLayer.Middle);
                    });
                }
            }
        } 

       

  

        void _OnTAPGraduationSuccess(UIEvent uiEvent)
        {
            if (mTAPGraduationEffUI != null)
            {
                mTAPGraduationEffUI.CustomActive(true);
                StartCoroutine(StopEffect());
            }
        }
        IEnumerator StopEffect()
        {
            if(mTAPGraduationEffUI != null)
            {
                var time = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)ProtoTable.SystemValueTable.eType2.SVT_TAPEFFUI_TIME).Value;
                float timeFloat = time * 1.0f / 1000;
                yield return new WaitForSeconds(timeFloat);
                mTAPGraduationEffUI.CustomActive(false);
            }
        }

        void InitTAPGraduationEffect()
        {
            if(mTAPGraduationEffUI != null)
            {
                mTAPGraduationEffUI.CustomActive(false);
            }
        }

        void OnPackageFullUpdate(UIEvent a_event)
        {
            if (ItemDataManager.GetInstance().IsPackageFull())
            {
                mPackageFull.gameObject.SetActive(true);
                mPackageAnim.DORestart();
            }
            else
            {
                mPackageFull.gameObject.SetActive(false);
                mPackageAnim.DOPause();
                mPackageAnim.gameObject.transform.localRotation = Quaternion.identity;
            }
        }

        void OnRedPacketOpenSuccess(UIEvent a_event)
        {
//             if (frameMgr.IsFrameOpen<GuildOpenRedPacketFrame>() == false)
//             {
//                 frameMgr.OpenFrame<GuildOpenRedPacketFrame>(FrameLayer.Middle, a_event.Param1);
//             }

            UpdateRedPacket();
        }

        void OnNewRedPacketGet(UIEvent a_event)
        {
            UpdateRedPacket();
        }

        void OnDeleteRedPacket(UIEvent a_event)
        {
            UpdateRedPacket();
        }

        public void OnPay(UIEvent iEvent)
        {
            bool needOpenWindow = ((string)iEvent.Param1) != "10";
            UpdatePay(needOpenWindow);
        }

        void OnGuildBattleStateChanged(UIEvent iEvent)
        {
            UpdateGuildBattle((EGuildBattleState)iEvent.Param1, (EGuildBattleState)iEvent.Param2);
        }

        void OnUpdateUplevelGift(UIEvent iEvent)
        {
            UpdateUplevelGiftText();
            UpdateDayOnlineGiftText();

            // 先判断公会tips，再判断技能tips
            UpdateGuildEffect();
            UpdateGuildTips();
            UpdateSkillTipsState();

            if (EquipHandbookDataManager.GetInstance().OnLoginFlag)
            {
                OnSceneUpdateEquipHandBookTips();

                if (JarDataManager.GetInstance().ShowJarTips())
                {
                    _showJarTip();
                }

                EquipHandbookDataManager.GetInstance().OnLoginFlag = false;
            }

            //通知福利界面关闭完成
            FollowingOpenQueueManager.GetInstance().NotifyCurrentOrderClosed();
        }

        void OnActivityUpdate(UIEvent a_event)
        {
            UpdateNewYearRedPackButton();
            UpdateTreasureConnvertButton();
            _InitMagicJar();

            var activityId = (uint)a_event.Param1;
            var chijiActivityIdList = ChijiDataManager.GetInstance().ChijiActivityIDs.ToList<int>();
            if (chijiActivityIdList!= null && chijiActivityIdList.Contains((int)activityId))
            {
                UpdateChijiButton();
            }
        }

        void OnSkillLearnTips(UIEvent a_event)
        {
            UpdateSkillLearnTips();
            UpdateEquipHandBookTips();
        }

        void OnFadeOutOver(UIEvent a_event)
        {
            if((string)a_event.Param1 != "ClientSystemTownFrame")
            {
                return;
            }

            _SetSkillTipActive(false);
			_SetGuildTipActive(false);
            _SetEquipHandBookTipActive(false);

            ClientSystemManager.instance.CloseFrame<TopLeftCornerExpandFrame>();
            ClientSystemManager.instance.CloseFrame<BottomRightCornerExpandFrame>();
        }
		
		  //added by mjx on 170814
        void OnSDKBindPhoneFinished(UIEvent uiEvent)
        {
            try
            {
                bool isShowed = (bool)uiEvent.Param1;

                if (MobileBind.MobileBindManager.GetInstance().IsMobileBindFuncEnable() == false)
                {
                    isShowed = false;
                }

                if (mSDKBindPhoneBtn)
                {
                    mSDKBindPhoneBtn.gameObject.CustomActive(isShowed);
                }
            }
            catch (Exception e)
            {
                Logger.LogError("bind phone send notify param is error :"+e.ToString());
            }
        }

        void OnMonthCardUpdate(UIEvent a_event)
        {
            //add by mjx on 170828 for month card tips
            MonthCardTipManager.instance.SetTrueConfig(PlayerBaseData.GetInstance().RoleID);

            //月卡状态改变 尝试刷新 红点
            MonthCardRewardLockersDataManager.GetInstance().RefreshRedPoint();
        }

        void OnChangeJobSelectDialog(UIEvent a_event)
        {
            ChangeJobSelectID = (int)a_event.Param1;

            var PreJobData = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().PreChangeJobTableID);

            if (PreJobData != null && PreJobData.PreJobDialogID.Count >= 2)
            {
                if (ChangeJobSelectID == PlayerBaseData.GetInstance().PreChangeJobTableID)
                {
                    MissionManager.GetInstance().CreateDialogFrame(PreJobData.PreJobDialogID[0], 0);
                }
                else
                {
                    MissionManager.GetInstance().CreateDialogFrame(PreJobData.PreJobDialogID[1], 0, new TaskDialogFrame.OnDialogOver().AddListener(_OpenChangeJobSelNextDialog));
                }
            }
        }

        void OnCloseGuildMainFrame(UIEvent a_event)
        {
            UpdateGuildEffect();
            UpdateGuildTips();
        }

        void _OpenChangeJobSelNextDialog()
        {
            var CurJobData = TableManager.GetInstance().GetTableItem<JobTable>(ChangeJobSelectID);

            if (CurJobData != null && CurJobData.PreJobDialogID.Count >= 2)
            {
                InvokeMethod.Invoke(this, 1.0f, () =>
                {
                    MissionManager.GetInstance().CreateDialogFrame(CurJobData.PreJobDialogID[0], 0);
                });
            }
        }

        void FuncUnlockNotify()
        {
            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.NewFuncUnlock);
        }

        void OpenComTalk()
        {
            if (m_state == EFrameState.FadeIn || m_state == EFrameState.Open)
            {
                InitTalk();
            }
        }

        void OnChatFilterChanged(List<bool> chatFilters)
        {
            if(ComTalk.ms_comTalk == m_miniTalk)
            {
                m_miniTalk.OnChatFilterChanged(chatFilters);
            }
        }

        void OnAddNewItem(List<Item> items)
        {
            for (int i = 0; i < items.Count; ++i)
            {
//                 var itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
            }
        }

        void OnAddNewMission(UInt32 taskID)
        {
            UpdateMissionRedPoint();

            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.ChapterReward);
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.ActivityDungeon);

            MissionManager.SingleMissionInfo missioninfo = null;

            if (MissionManager.GetInstance().IsChangeJobMainMission((int)taskID, ref missioninfo))
            {
                // ClientSystemTown._OpenChangeJobTip();
            }
        }

        void OnUpdateMission(UInt32 taskID)
        {
            MissionManager.SingleMissionInfo skillinfo = null;

            if (MissionManager.GetInstance().IsChangeJobMainMission((int)taskID, ref skillinfo))
            {
                // ClientSystemTown._OpenChangeJobTip();

                if (skillinfo != null && skillinfo.status == (int)TaskStatus.TASK_FINISHED)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.JobIDChanged); // 转职
                }
            }

            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.ChapterReward);
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.ActivityDungeon);

            UpdateMissionRedPoint();
        }

        void OnDeleteMission(UInt32 taskID)
        {
            if (MissionManager.GetInstance().IsFinishingAwakeMission((int)taskID))
            {
                ClientSystemTown currentSystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;

                if (currentSystem != null)
                {
                    if (currentSystem.MainPlayer != null)
                    {
                        currentSystem.MainPlayer.SetPlayerAwakeState(true);
                    }

                    PlayerBaseData.GetInstance().bNeedShowAwakeFrame = true;

                    if (!ClientSystemManager.GetInstance().IsFrameOpen<LevelUpNotify>())
                    {
                        ClientSystemManager.GetInstance().OpenFrame<AwakeFrame>(FrameLayer.Middle);
                    }
                }
            }

            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.ChapterReward);
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.ActivityDungeon);

            UpdateMissionRedPoint();
        }

        void OnMissionChanged()
        {
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.ChapterReward);
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.ActivityDungeon);

            UpdateMissionRedPoint();
        }

        void _OnChestIdsChanged()
        {
            _updateDailyRedPoint();
        }

        void _OnActivityUpdate(ActiveManager.ActivityData data, ActiveManager.ActivityUpdateType EActivityUpdateType)
        {
            UpdateDayOnlineGiftText();
            UpdateUplevelGiftText();       
            UpdateShowChannelRankListButton();
            UpdateNewYearRedPackButton();
            UpdateTreasureConnvertButton();
        }

        /// <summary>
        /// 
        /// </summary>
        void _OnDumpAssetInfo()
        {
            List<string> info = new List<string>();
            AssetPackageManager.instance.DumpAssetPackageInfo(ref info);
            string infoDump = "";
            for (int i = 0, icnt = info.Count; i < icnt; ++i)
            {
                infoDump += info[i];
                infoDump += "\n";
            }
            Debug.LogError(infoDump);

            infoDump = "";
            info.Clear();
            //AssetDesc.DumpAssetDescReg(ref info);
            for (int i = 0, icnt = info.Count; i < icnt; ++i)
            {
                infoDump += info[i];
                infoDump += "\n";
            }

            Debug.LogError(infoDump);
        }

#endregion

#region updateinfo
        void UpdatePkWaitingRoom(CitySceneTable TownTableData)
        {
            ClientSystemManager.GetInstance().bIsInPkWaitingRoom = true;
            SetForbidFadeIn(true);

            if (!ClientSystemManager.GetInstance().IsFrameOpen<PkWaitingRoom>())
            {
                PkWaitingRoomData RoomData = new PkWaitingRoomData
                {
                    SceneSubType = TownTableData.SceneSubType,
                    CurrentSceneID = TownTableData.ID,
                    TargetTownSceneID = TownTableData.BirthCity
                };

                ClientSystemManager.GetInstance().OpenFrame<PkWaitingRoom>(FrameLayer.Bottom, RoomData);
            }
        }

        void UpdateBudo(CitySceneTable TownTableData)
        {
            BudoArenaFrameData data = new BudoArenaFrameData
            {
                CurrentSceneID = TownTableData.ID,
                TargetTownSceneID = TownTableData.BirthCity
            };

            BudoArenaFrame.Open(data);
        }

        void UpdateGuildBattle()
        {
            ClientSystemManager.GetInstance().OpenFrame<GuildBattleFrame>(FrameLayer.Bottom);
        }

        void UpdatePk3v3WaitingRoom(CitySceneTable TownTableData)
        {
            SetForbidFadeIn(true);

            if (!ClientSystemManager.GetInstance().IsFrameOpen<Pk3v3WaitingRoom>())
            {
                PkWaitingRoomData RoomData = new PkWaitingRoomData
                {
                    SceneSubType = TownTableData.SceneSubType,
                    CurrentSceneID = TownTableData.ID,
                    TargetTownSceneID = TownTableData.BirthCity
                };

                ClientSystemManager.GetInstance().OpenFrame<Pk3v3WaitingRoom>(FrameLayer.Bottom, RoomData);
            }
        }

        void UpdatePk3v3CrossWaitingRoom(CitySceneTable TownTableData)
        {
            SetForbidFadeIn(true);

            if (!ClientSystemManager.GetInstance().IsFrameOpen<Pk3v3CrossWaitingRoom>())
            {
                PkWaitingRoomData RoomData = new PkWaitingRoomData
                {
                    SceneSubType = TownTableData.SceneSubType,
                    CurrentSceneID = TownTableData.ID,
                    TargetTownSceneID = TownTableData.BirthCity
                };

                ClientSystemManager.GetInstance().OpenFrame<Pk3v3CrossWaitingRoom>(FrameLayer.Bottom, RoomData);

                ClientSystemManager.GetInstance().OpenFrame<Pk3v3CrossTeamMainFrame>(FrameLayer.Bottom);
                ClientSystemManager.GetInstance().GetFrame(typeof(Pk3v3CrossTeamMainFrame)).GetFrame().CustomActive(true);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3CrossUpdateMyTeamFrame);

                ClientSystemManager.GetInstance().OpenFrame<Pk3v3CrossTeamMainMenuFrame>(FrameLayer.Bottom);
                ClientSystemManager.GetInstance().GetFrame(typeof(Pk3v3CrossTeamMainMenuFrame)).GetFrame().CustomActive(true);

                if (Pk3v3CrossDataManager.GetInstance().HasTeam())
                {
                    ClientSystemManager.GetInstance().OpenFrame<Pk3v3CrossMyTeamFrame>();
                    ClientSystemManager.GetInstance().GetFrame(typeof(Pk3v3CrossMyTeamFrame)).GetFrame().CustomActive(true);
                }

                //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PK3V3CrossButton, "HideJoin3v3CrossBtn");
            }
        }

        void UpdatePk2v2CrossWaitingRoom(CitySceneTable TownTableData)
        {
            SetForbidFadeIn(true);
            if (!ClientSystemManager.GetInstance().IsFrameOpen<Pk2v2CrossWaitingRoomFrame>())
            {
                PkWaitingRoomData RoomData = new PkWaitingRoomData
                {
                    SceneSubType = TownTableData.SceneSubType,
                    CurrentSceneID = TownTableData.ID,
                    TargetTownSceneID = TownTableData.BirthCity
                };
                ClientSystemManager.GetInstance().OpenFrame<Pk2v2CrossWaitingRoomFrame>(FrameLayer.Bottom, RoomData);                
            }
        }
        void UpdateFairBattleWaitingRoom(CitySceneTable TownTableData)
        {
            SetForbidFadeIn(true);
            if (!ClientSystemManager.GetInstance().IsFrameOpen<FairDuelWaitingRoomFrame>())
            {
                var FairDueliRoomData = new FairDueliRoomData
                {
                    SceneSubType = TownTableData.SceneSubType,
                    CurrentSceneID = TownTableData.ID,
                    TargetTownSceneID = TownTableData.TraditionSceneID,
                };
                ClientSystemManager.GetInstance().OpenFrame<FairDuelWaitingRoomFrame>(FrameLayer.Bottom, FairDueliRoomData);
            }
        }

        void UpdateGuildArenaFrame(CitySceneTable TownTableData)
        {
            SetForbidFadeIn(true);
            if (!ClientSystemManager.GetInstance().IsFrameOpen<GuildArenaFrame>())
            {
                var guildAeenaData = new GuildArenaData
                {
                    SceneSubType = TownTableData.SceneSubType,
                    CurrentSceneID = TownTableData.ID,
                    TargetTownSceneID = TownTableData.TraditionSceneID,
                };
                ClientSystemManager.GetInstance().OpenFrame<GuildArenaFrame>(FrameLayer.Bottom, guildAeenaData);
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildDungeonShowFireworks);
        }
		
		 private void UpdateTeamDuplicationBuildFrame(CitySceneTable townTableData)
        {
            SetForbidFadeIn(true);
            if(ClientSystemManager.GetInstance().IsFrameOpen<TeamDuplicationMainBuildFrame>() == false)
                TeamDuplicationUtility.OnOpenTeamDuplicationMainBuildFrame();
        }

        private void UpdateTeamDuplicationFightFrame(CitySceneTable townTableData)
        {
            SetForbidFadeIn(true);
            if(ClientSystemManager.GetInstance().IsFrameOpen<TeamDuplicationMainFightFrame>() == false)
                TeamDuplicationUtility.OnOpenTeamDuplicationMainFightFrame();
        }     

        void UpdateRedPacket()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (scenedata == null)
            {
                return;
            }

            if (scenedata.SceneType == CitySceneTable.eSceneType.NORMAL ||
                scenedata.SceneType == CitySceneTable.eSceneType.SINGLE ||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TRADITION || 
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.Pk3v3)
            {
                int nCount = RedPackDataManager.GetInstance().GetWaitOpenCount();
                mLabRedPacketCount.text = nCount.ToString();

                mRedPacket.gameObject.CustomActive(nCount > 0/* && RedPackDataManager.GetInstance().CheckNewYearActivityOpen()*/);
                mLabRedPacketCount.gameObject.CustomActive(nCount > 1/* && RedPackDataManager.GetInstance().CheckNewYearActivityOpen()*/);     
            }
            else
            {
                mRedPacket.gameObject.CustomActive(false);
                mLabRedPacketCount.gameObject.CustomActive(false);
            }
        }
       
        void UpdateUplevelGiftText()
        {

            ComTopButtonExpandController control = GetComTopButtonExpandController();
            if (control == null)
            {
                return;
            }

            bool isFuncUnLock = Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.Welfare);
            mUpLevelGiftObj.SetActive(HaveLevelGift() && control.IsExpand() && isFuncUnLock);
        }

        bool HaveLevelGift()
        {
            var UplevelGiftdata = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_LEVELUPGIFT_LIMIT);
            if (UplevelGiftdata == null)
            {
                return false;
            }
            int iLimitLv = UplevelGiftdata.Value;

            var activeData = ActiveManager.GetInstance().GetActiveData(4000);
            if (activeData == null)
            {
                return false;
            }

            var acts = activeData.akChildItems;

            if (acts == null)
            {
                
                return false;
            }

            int iCanReceiveIdx = -1;
            int iUnFinishIdx = -1;

            Utility.CalShowUplevelGiftIndex(activeData, ref iCanReceiveIdx, ref iUnFinishIdx);

            int iIndex = 0;
            if (iCanReceiveIdx != -1)
            {
                iIndex = iCanReceiveIdx;
            }
            else if (iUnFinishIdx != -1)
            {
                iIndex = iUnFinishIdx;
            }
            else
            {
                return false;
            }

            if (activeData.akChildItems[iIndex].activeItem.LevelLimit <= iLimitLv)
            {
                mUplevelGiftText.text = string.Format("{0}级礼包", activeData.akChildItems[iIndex].activeItem.LevelLimit);
                return true;
            }
            else
            {
                return false;
            }
        }

        void UpdateDayOnlineGiftText()
        {
            bool isFuncUnLock = Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Welfare);
            mOnLineGift.CustomActive(HaveOnlineGift() && isFuncUnLock);
        }
        bool HaveOnlineGift()
        {
            var activeData = ActiveManager.GetInstance().GetActiveData(5000);
            if (activeData == null)
            {
                return false;
            }

            var acts = activeData.akChildItems;

            if (acts == null)
            {
                return false;
            }

            int iCanReceiveIdx = -1;
            int iUnFinishIdx = -1;

            Utility.CalShowUplevelGiftIndex(activeData, ref iCanReceiveIdx, ref iUnFinishIdx);

            int iIndex = 0;
            if (iCanReceiveIdx != -1)
            {
                iIndex = iCanReceiveIdx;
            }
            else if (iUnFinishIdx != -1)
            {
                iIndex = iUnFinishIdx;
            }
            else
            {
                return false;
            }

            if (activeData.akChildItems[iIndex].activeItem.Param0 == "")
            {
                return false;
            }

            int iNeedDayOnlineTime = int.Parse(activeData.akChildItems[iIndex].activeItem.Param0);
            int iDayOnlineTime = Utility.GetDayOnLineTime();
          
            if (iDayOnlineTime < iNeedDayOnlineTime * 60)
            {
                mOnlIneGiftText.SafeSetText(Function.GetLastsTimeStr(iNeedDayOnlineTime * 60 - iDayOnlineTime, true));
            }
            else
            {
                mOnlIneGiftText.text = "可领取";
            }

            return true;
        }

        void UpdateDuelOrChangeJobBtn()
        {
            if (PlayerBaseData.GetInstance().Level <= 15 && IsJobChangeAfter())
            {
                mDuel.gameObject.CustomActive(false);
            }
            else
            {
                if (Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Duel))
                    mDuel.gameObject.CustomActive(true);
                else
                {
                    mDuel.gameObject.CustomActive(false);
                }
            }
        }

        void UpdateChallengeButton()
        {
            if (mChallenge != null)
            {
                if (PlayerBaseData.GetInstance().Level < ChallengeDataManager.GetInstance().ChallengeOpenLevel)
                {
                    mChallenge.gameObject.CustomActive(false);
                }
                else
                {
                    mChallenge.gameObject.CustomActive(true);
                }
            }
        }

        void UpdateDuelLockTip()
        {
            if(Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Duel))
            {
                mDuelTipRoot.SetActive(false);
            }
            else
            {
                mDuelTipRoot.SetActive(true);
            }
        }

        void UpdateSkillLearnTips()
        {
            bool isSkillBarFull = (!SkillDataManager.GetInstance().IsSkillBarFull(SkillConfigType.SKILL_CONFIG_PVE) || !SkillDataManager.GetInstance().IsSkillBarFull(SkillConfigType.SKILL_CONFIG_PVP));
            if (( SkillDataManager.GetInstance().IsShowSkillButton() == true
                  && (isSkillBarFull || SkillDataManager.GetInstance().HasSkillLvCanUp())))
            {
                if (isSkillBarFull)
                {
                    //mBeStrongTipsText.text = TR.Value("skill point03");
                }
                else
                {
                    //mBeStrongTipsText.text = TR.Value("skill point02");
                }

                mRightLowerBubbleShowOrder.AddAnimation(BubbleShowType.Skill);

                IsShowSkillTips = true;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TipsAniStart);
                
            }
            else
            {
                _SetSkillTipActive(false);
            }
            
        }

        void OnSceneUpdateEquipHandBookTips()
        {
//             bool bIsHintEquipmentGuide = EquipHandbookDataManager.GetInstance().BIsHintEquipmentGuide() && Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.EquipHandBook); 
// 
//             if (bIsHintEquipmentGuide)
//             {
//                 mEquipHandBookTipsText.text = TR.Value("equiphandbookhint");
//                 mRightLowerBubbleShowOrder.AddAnimation(BubbleShowType.EquipHandBook);
// 
//                 IsShowEquipHandBookTips = true;
//                 UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TipsAniStart);
//             }
//             else
//             {
//                 _SetEquipHandBookTipActive(false);
//             }
        }

        void UpdateEquipHandBookTips()
        {
//             bool bIsHintEquipmentGuide = EquipHandbookDataManager.GetInstance().bIsHintEquipmentGuide();
//             if (bIsHintEquipmentGuide)
//             {
//                 mEquipHandBookTipsText.text = TR.Value("equiphandbookhint");
//                 mRightLowerBubbleShowOrder.AddAnimation(BubbleShowType.EquipHandBook);
// 
//                 IsShowEquipHandBookTips = true;
//                 UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TipsAniStart);
//             }
//             else
//             {
//                 _SetEquipHandBookTipActive(false);
//             }
        }

        void UpdateGuildTips()
        {
            EGuildBattleState state = GuildDataManager.GetInstance().GetGuildBattleState();

            if (state < EGuildBattleState.Signup || state > EGuildBattleState.LuckyDraw)
            {
                _SetGuildTipActive(false);
                return;
            }

            if(state == EGuildBattleState.Signup)
            {
                if(GuildDataManager.GetInstance().GetGuildBattleType() != GuildBattleType.GBT_CHALLENGE)
                {
                    _SetGuildTipActive(false);
                    return;
                }
            }

            if (state >= EGuildBattleState.Preparing && state <= EGuildBattleState.Firing)
            {
                if (!GuildDataManager.GetInstance().HasTargetManor())
                {
                    _SetGuildTipActive(false);
                    return;
                }
            }

            if (state == EGuildBattleState.LuckyDraw)
            {
                if(GuildDataManager.GetInstance().HasGuildBattleLotteryed())
                {
                    _SetGuildTipActive(false);
                    return;
                }
            }

            _SetSkillTipActive(false);

            mRightLowerBubbleShowOrder.AddAnimation(BubbleShowType.Guild);

            if (state == EGuildBattleState.Signup)
            {
                mGuildTipsText.text = TR.Value("guild_battle_challenge");
            }
            else if (state == EGuildBattleState.Preparing)
            {
                mGuildTipsText.text = TR.Value("guild_battle_prepare");
            }
            else if (state == EGuildBattleState.Firing)
            {
                mGuildTipsText.text = TR.Value("guild_battle_fire");
            }
            else
            {
                mGuildTipsText.text = TR.Value("guild_battle_lottery");
            }
            
        }


        // 公会战或者公会副本活动期间显示特效
        // add by qxy 2019-01-23
        void UpdateGuildEffect()
        {
            if(mGuildBattleEffect == null)
            {
                return;
            }

            bool bGuildDungeonActivityOpen = GuildDataManager.GetInstance().IsGuildDungeonActivityOpen();          
            EGuildBattleState CurState = GuildDataManager.GetInstance().GetGuildBattleState();
            bool bGuildBattleActivityOpen = false;
            if (CurState >= EGuildBattleState.Preparing 
                && CurState <= EGuildBattleState.Firing 
                && GuildDataManager.GetInstance().HasTargetManor())
            {
                bGuildBattleActivityOpen = true;
            }
            if (bGuildBattleActivityOpen || bGuildDungeonActivityOpen)
            {
                mGuildBattleEffect.gameObject.CustomActive(true);
                mGuildBattleEffect.gameObject.CustomActive(true);
            }
            else
            {
                mGuildBattleEffect.gameObject.CustomActive(false);
                mGuildBattleEffect.gameObject.CustomActive(false);
            }
        }

        void OnSkillTipsAniComplete()
        {
            IsShowSkillTips = false;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TipsAniEnd);
        }

        void OnGuildTipsAniComPlete()
        {
            IsShowGuildTips = false;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TipsAniEnd);
        }

        void OnEquipHandBookAniComPlete()
        {
            IsShowEquipHandBookTips = false;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TipsAniEnd);
        }

        void UpdateShowButton()
        {
            //注： 如果是帐号绑定功能 在这里跳过 有专门的帐号解锁通知 和 各自功能的单独的解锁判断 ！！！
            var FunctionUnLockData = TableManager.GetInstance().GetTable<FunctionUnLock>();
            var enumerator = FunctionUnLockData.GetEnumerator();

            while (enumerator.MoveNext())
            {
                FunctionUnLock FunctionUnLockItem = enumerator.Current.Value as FunctionUnLock;

                if (FunctionUnLockItem.TargetBtnPos == "" || FunctionUnLockItem.TargetBtnPos == "-" || FunctionUnLockItem.bShowBtn == 1)
                {
                    continue;
                }
               
                // 冒险者通行证解锁按钮是账号绑定，这里需要跳过
                if(FunctionUnLockItem.FuncType == FunctionUnLock.eFuncType.AdventurePassSeason)
                {
                    continue;
                }

                GameObject buttonObj = Utility.FindGameObject(frame, FunctionUnLockItem.TargetBtnPos);
                if (buttonObj == null)
                {
                    continue;
                }

                bool bFind = false;
                for (int i = 0; i < PlayerBaseData.GetInstance().UnlockFuncList.Count; i++)
                {
                    if (FunctionUnLockItem.ID != PlayerBaseData.GetInstance().UnlockFuncList[i])
                    {
                        continue;
                    }

                    bFind = true;
                    break;
                }

                if(FunctionUnLockItem.FuncType == FunctionUnLock.eFuncType.Duel)
                {
                    bFind = true;
                }
#if APPLE_STORE
                //add by mjx for ios appstore
                if (CheckIsFunctionClose(FunctionUnLockItem))
                {
                     bFind = false;     
                }
#endif

                if (bFind)
                {
                    buttonObj.SetActive(true);
                }
                else
                {
                    buttonObj.SetActive(false);
                }
            }
        }

        public void UpdatePay(bool needOpenWindow = true)
        {
            if ((PayManager.GetInstance().HasFirstPay() || PayManager.GetInstance().HasConsumePay()) && Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.FirstReChargeActivity))
            {
                mFirstRecharge.gameObject.SetActive(true);
                mFirstRechargeRedPoint.gameObject.SetActive(PayManager.GetInstance().CanGetRewards());
            }
            else
            {
                mFirstRecharge.gameObject.SetActive(false);
            }

            if (needOpenWindow)
            {
                if (PayManager.GetInstance().HasNewActivityFinish())
                {
                    _onFirstRechargeButtonClick();
                }                    
            }
        }

        void UpdateGuildBattle(EGuildBattleState a_eOldState, EGuildBattleState a_eNewState)
        {
            NotifyInfo NoticeData = new NotifyInfo
            {
                type = (uint)NotifyType.NT_GUILD_BATTLE
            };

            if (a_eNewState == EGuildBattleState.Preparing || a_eNewState == EGuildBattleState.Firing)
            {
                ActivityNoticeDataManager.GetInstance().AddActivityNotice(NoticeData);
                DeadLineReminderDataManager.GetInstance().AddActivityNotice(NoticeData);
            }
            else
            {
                ActivityNoticeDataManager.GetInstance().DeleteActivityNotice(NoticeData);
                DeadLineReminderDataManager.GetInstance().DeleteActivityNotice(NoticeData);
            }

            CitySceneTable.eSceneSubType subType;
            ClientSystemTown.GetCurrentSceneSubType(out subType);

            if (subType != CitySceneTable.eSceneSubType.GuildBattle && subType != CitySceneTable.eSceneSubType.CrossGuildBattle)
            {
                if (GuildDataManager.GetInstance().isBattleNotifyInited == false || a_eOldState != a_eNewState)
                {
                    if (a_eNewState == EGuildBattleState.Preparing || a_eNewState == EGuildBattleState.Firing)
                    {
//                         NewMessageNoticeManager.GetInstance().AddNewMessageNoticeWhenNoExist("GuildBattle", null, data =>
//                         {
//                             Utility.EnterGuildBattle();
//                             NewMessageNoticeManager.GetInstance().RemoveNewMessageNotice(data);
//                         });
                    }

                    GuildDataManager.GetInstance().isBattleNotifyInited = true;
                }
            }
            else
            {
                GuildDataManager.GetInstance().isBattleNotifyInited = true;
            }

            // 公会图标加特效
            UpdateGuildEffect();

            // 气泡提示
            UpdateGuildTips();
        }

        void UpdateMissionRedPoint()
        {
            if (frame == null)
            {
                //Logger.LogError ("frame is null 客户端流程有问题!!!");
                return;
            }

            int iUnlockLevel = 0;
            var FuncUnlockdata = TableManager.GetInstance().GetTableItem<FunctionUnLock>((int)FunctionUnLock.eFuncType.Achievement);
            if (FuncUnlockdata != null)
            {
                iUnlockLevel = FuncUnlockdata.FinishLevel;
            }
            bool bAchievementUnlocked = PlayerBaseData.GetInstance().Level >= iUnlockLevel;

            iUnlockLevel = 0;
            FuncUnlockdata = TableManager.GetInstance().GetTableItem<FunctionUnLock>((int)FunctionUnLock.eFuncType.DailyTask);
            if (FuncUnlockdata != null)
            {
                iUnlockLevel = FuncUnlockdata.FinishLevel;
            }
            bool bDailyUnlocked = PlayerBaseData.GetInstance().Level >= iUnlockLevel;

            bool bHasFinishDaily = false;
            bool bHasFinishAchievent = false;
            bool bHasFinishNormal = false;
            bool bHasFinishMission = false;
            var values = MissionManager.GetInstance().taskGroup.Values.ToList();
            for (int i = 0; i < values.Count; ++i)
            {
                if (values[i] == null || values[i].missionItem == null)
                {
                    continue;
                }

                if(values[i].status != (int)Protocol.TaskStatus.TASK_FINISHED)
                {
                    continue;
                }

                if(!bHasFinishNormal)
                {
                    if(values[i].missionItem.TaskType == MissionTable.eTaskType.TT_BRANCH ||
                        values[i].missionItem.TaskType == MissionTable.eTaskType.TT_MAIN ||
                        values[i].missionItem.TaskType == MissionTable.eTaskType.TT_CYCLE)
                    {
                        bHasFinishNormal = true;
                    }
                }

                if(!bHasFinishDaily)
                {
                    if(bDailyUnlocked && values[i].missionItem.TaskType == MissionTable.eTaskType.TT_DIALY
                    && values[i].missionItem.SubType == MissionTable.eSubType.Daily_Task)
                    {
                        bHasFinishDaily = true;
                    }
                }

                if(!bHasFinishAchievent)
                {
                    if(bAchievementUnlocked && values[i].missionItem.TaskType == MissionTable.eTaskType.TT_ACHIEVEMENT && values[i].missionItem.SubType ==
                    MissionTable.eSubType.Daily_Null)
                    {
                        bHasFinishAchievent = true;
                    }
                }
            }
            bHasFinishMission = bHasFinishAchievent || bHasFinishNormal;
            //mMissionRedPoint.gameObject.SetActive(bHasFinishMission);
            //mDailyMissionRedPoint.gameObject.SetActive(bHasFinishDaily);

            return;
        }

        void UpdateSkillTipsState()
        {
            if (SkillDataManager.GetInstance().bNoticeSkillLvUp)
            {
                UpdateSkillLearnTips();
                SkillDataManager.GetInstance().bNoticeSkillLvUp = false;
            }
        }
        /// <summary>
        /// 是否可以弹出礼包Frame
        /// </summary>
        private bool mIsCanShowGiftFrame = true;
        //add by mjx for 商城时限礼包通知购买
        void UpdateMallGiftNotice()
        {
            if (ChijiDataManager.GetInstance().SwitchingPrepareToTown)
            {
                return;
            }
            if (!mIsCanShowGiftFrame) return;
            if (ActivityLimitTimeCombineManager.GetInstance() != null
               && ActivityLimitTimeCombineManager.GetInstance().GiftDataManager != null)
            {
                ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.SendReqLimitGiftData();
            }

            if(LimitTimeGift.LimitTimeGiftFrameManager.GetInstance() != null)
            {
                LimitTimeGift.LimitTimeGiftFrameManager.GetInstance().WaitToShowLimitTimeGiftFrame();
            }
        }
#endregion

#region update
        public sealed override bool IsNeedUpdate()
        {
            return true;
        }

        protected sealed override void _OnUpdate(float timeElapsed)
        {
			if (_inputManager != null)
			{
				_inputManager.SingleUpdate(0);
			}

            CheckLevelUp();       

            DayOnlineInterval += timeElapsed;

            if(DayOnlineInterval > 1.0f)
            {
                DayOnlineInterval = 0.0f;
                UpdateDayOnlineGiftText();
                UpdateMainPlayerLevel();
            }

            if (mPrivateCustomBubbles != null && mPrivateCustomBubbles.gameObject.activeInHierarchy)
            {
                if (mPrivateCustomBubbles.gameObject.activeSelf == true)
                {
                    mPrivateCustomBubbles.gameObject.CustomActive(false);
                }
                PrivateCustomBubbleTime += timeElapsed;
                if (PrivateCustomBubbleTime >= 60.0f)
                {
                    mPrivateCustomBubbles.gameObject.CustomActive(false);

                    StartCoroutine(mPrivateCustomShowOrHide(300.0f));

                    PrivateCustomBubbleTime = 0;
                }
                
            }

			UpdateCheckFPS(timeElapsed);

            OnlineServiceManager.GetInstance().UpdateReqOfflineInfos(timeElapsed);
        }
        
        /// <summary>
        /// 更新主角的Level(城镇左上角和主角title的level）,1s中更新一次，
        /// 避免数据同步不一致。
        /// </summary>
        private void UpdateMainPlayerLevel()
        { 
            var currentSystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (currentSystem != null && currentSystem.MainPlayer != null)
            {
                if (currentSystem.MainPlayer.GraphicActor != null)
                {
                    currentSystem.MainPlayer.GraphicActor.OnLevelChanged(PlayerBaseData.GetInstance().Level);
                }             
            }
        }
#endregion

		protected float fpsAcc = 0;
		protected void UpdateCheckFPS(float delta)
		{
			fpsAcc += delta;
			if (fpsAcc >= ComponentFPS.instance.watchFrames)
			{
				fpsAcc -= ComponentFPS.instance.watchFrames;
				GeGraphicSetting.instance.CheckTownFPS();

/*				if (GeGraphicSetting.instance.needPromoted)
				{
					ShowGraphicTip();
				}*/
			}
		}

		void ShowGraphicTip()
		{
			if (mGraphicTips.gameObject.activeSelf)
				return;

			mGraphicTipsText.text = TR.Value("graphic_set_tip");

			DOTweenAnimation[] dotween1 = mGraphicTips.GetComponents<DOTweenAnimation>();
			DOTweenAnimation[] dotween2 = mGraphicTipsText.GetComponents<DOTweenAnimation>();

			for (int i = 0; i < dotween1.Length; i++)
			{
				dotween1[i].DORestart();
				dotween2[i].DORestart();

				dotween1[i].onComplete.RemoveAllListeners();

				if(i == 2)
				{
					dotween1[i].onComplete.AddListener(_onGraphicTipsButtonClick);
				}
			}

			mGraphicTips.gameObject.CustomActive(true);
		}

        IEnumerator mPrivateCustomShowOrHide(float waitTime)
        {
            
            yield return Yielders.GetWaitForSeconds(waitTime);

            mPrivateCustomBubbles.gameObject.CustomActive(false);
           
        }

#region protected interface
        protected void _OnMailListReq()
        {
            MailDataManager.OnSendMailListReq();
        }

        // 请求邮件列表返回
        [MessageHandle(WorldMailListRet.MsgID)]
        protected void _OnWorldMailListRes(MsgDATA msg)
        {
            WorldMailListRet res = new WorldMailListRet();
            res.decode(msg.bytes);

            Logger.Log("[MailSystem] Recived WorldMailListRet.. \n");

            // 接收到邮件数据
            PlayerBaseData.GetInstance().mails.mailList.Clear();
            PlayerBaseData.GetInstance().mails.rewardMailList.Clear();
            for (int i = 0; i < res.mails.Length; i++)
            {

                if ((MailType)res.mails[i].type == MailType.MAIL_TYPE_GM )
                {
                    PlayerBaseData.GetInstance().mails.mailList.Add(res.mails[i]);
                }
                else
                {
                    PlayerBaseData.GetInstance().mails.rewardMailList.Add(res.mails[i]);
                }
            }

            PlayerBaseData.GetInstance().mails.SortMailList();
            PlayerBaseData.GetInstance().mails.UpdateOneKeyNum();

            PlayerBaseData.GetInstance().mails.SortRewardMailList();
            PlayerBaseData.GetInstance().mails.UpdateOneKeyRewardNum();

            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.NewMailNotify);
        }

        //开始打开之后的一系列功能和界面的队列
        void _StartOpenFollowingQueue()
        {
            // 接下来的界面 通过这里打开
            FollowingOpenQueueManager.GetInstance().StartOpenFollowingQueue();
        }

        void _TryOpenActiveFrame(FollowingOpenTrigger trigger)
        {
            if (ClientSystemManager.GetInstance().PreSystemType != typeof(ClientSystemLogin))
            {
                return;
            }

            //以线上现象得到  福利界面的弹出 是每个角色每日弹一次的 
            //2019年5月8日 
            if (!AdsPush.LoginPushManager.GetInstance().IsFirstLogin())
            {
                return;
            }

            int[] aiTemplateID = new int[] { 8100, 3000 };
            int[] aiOpenLevel = new int[] { 9, 9 };

            bool bShowActiveFrame = false;
            for (int i = 0; i < aiTemplateID.Length; ++i)
            {
                var activeData = ActiveManager.GetInstance().GetActiveData(aiTemplateID[i]);
                if (activeData == null)
                {
                    continue;
                }

                if (PlayerBaseData.GetInstance().Level < aiOpenLevel[i])
                {
                    continue;
                }

                var find = activeData.akChildItems.Find(x =>
                {
                    return x.status == 2;
                });

                if (find != null)
                {
                    ActiveManager.GetInstance().OpenActiveFrame(activeData.mainItem.ActiveTypeID, aiTemplateID[i]);
                    bShowActiveFrame = true;
                    break;
                }
            }

            if (!bShowActiveFrame)
            {
                // 先判断公会tips，再判断技能tips
                UpdateGuildEffect();
                UpdateGuildTips();
                UpdateSkillTipsState();
            }
            else
            {
                if (trigger != null)
                {
                    trigger.triggerType = FollowingOpenTriggerType.Normal;
                }
            }
        }

        void CheckLevelUp()
        {
            if(!PlayerBaseData.GetInstance().bLevelUpChange || bGuardForNotify)
            {
                return;
            }

            if(!ClientSystem.IsCurrentSystemStart())
            {
                return;
            }

            if(PlayerBaseData.GetInstance().Level < 5)
            {
                return;
            }

            if(NewbieGuideManager.GetInstance().IsGuidingControl())
            {
                return;
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<TaskDialogFrame>() || ClientSystemManager.GetInstance().IsFrameOpen<AwakeFrame>())
            {
                return;
            }

            bGuardForNotify = true;
            InvokeMethod.Invoke(LeanTween.instance.LevelUpNotifyDelay, () =>
            {
                PlayerBaseData.GetInstance().bLevelUpChange = false;
                bGuardForNotify = false;
                
                if (PlayerBaseData.GetInstance().Level >= 5)
                {
                    frameMgr.OpenFrame<LevelUpNotify>(FrameLayer.Middle);
                }
                else
                {
                    UIEvent uiEvent = new UIEvent
                    {
                        EventID = EUIEventID.CheckAllNewbieGuide
                    };
                    GlobalEventSystem.GetInstance().SendUIEvent(uiEvent);
                }
            });
        }       

        void _OnShowChangeJobDialog()
        {
            ClientSystemTown clientSystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (clientSystem == null)
            {
                return;
            }

            _JobChangeSuccessFrame();
        }

        void _JobChangeSuccessFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<ChangeJobFinish>())
            {
                ClientSystemManager.GetInstance().CloseFrame<ChangeJobFinish>();
            }

            ClientSystemGameBattle batlleTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
            if(batlleTown != null)
            {
                return;
            }
            GameFrameWork.instance.StartCoroutine(_OpenCHangeJobFinish());
            // ClientSystemManager.GetInstance().OpenFrame<ChangeJobFinish>(FrameLayer.Middle);
        }

        private IEnumerator _OpenCHangeJobFinish()
        {
            yield return new WaitForSeconds(0.5f);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PlayChangeJobEffect);
            int time = Utility.GetClientIntValue(ClientConstValueTable.eKey.CHANGE_JOB_FINISH_DELAY_TIME, 1000);
            yield return new WaitForSeconds(((float)time)/1000f);
            ClientSystemManager.GetInstance().OpenFrame<ChangeJobFinish>(FrameLayer.Middle);
        }

        void _SetSkillTipActive(bool bActive)
        {
            //mBeStrongTips.CustomActive(bActive);
        }

        void _SetEquipHandBookTipActive(bool bActive)
        {
//             if (bActive)
//             {
//                 mEquipHandBookTips.gameObject.CustomActive(true);
//             }
//             else
//             {
//                 mEquipHandBookTips.gameObject.CustomActive(false);
//             }
        }

        void _setJarTipsActive(bool bActive)
        {

#if APPLE_STORE
            //屏蔽罐子提示
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.LIMITTIME_JAR))
            {
                mJarTips.gameObject.CustomActive(false);
				return;
            }
#endif

            if (bActive)
            {
                mJarTips.gameObject.CustomActive(true);
            }
            else
            {
                mJarTips.gameObject.CustomActive(false);
            }
        }

        void _SetGuildTipActive(bool bActive)
        {
            if (bActive)
            {
                if (mGuildTips != null)
                    mGuildTips.gameObject.CustomActive(true);
            }
            else
            {
                if (mGuildTips != null)
                    mGuildTips.gameObject.CustomActive(false);
            }
        }

        void _OnJoyStickStop()
        {
            mIsStopMoveFunction = false;
            mLastJoyStickFizzyCheck = false;

            ClientSystemTown cursystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;

            if (cursystem != null && cursystem.MainPlayer != null)
            {
                if (cursystem.MainPlayer.MoveState == BeTownPlayerMain.EMoveState.AutoMoving)
                {
                    return;
                }

                cursystem.MainPlayer.CommandStopMove();
            }
        }

        void _OnJoyStickMove(Vector2 pos)
        {
            ClientSystemTown cursystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (cursystem.MainPlayer == null)
            {
                return;
            }

            if (pos == Vector2.zero)
            {
                return;
            }

            if (mLastJoyStickFizzyCheck)
            {
                Vector2 distance = mLastJoyStickPosition - pos;

                if (distance.magnitude < 0.438f)
                {
                    return ;
                }

                mLastJoyStickFizzyCheck = false;
            }

            mLastJoyStickPosition = pos;

            if (mIsStopMoveFunction)
            {
                return;
            }

            Vector2 dir = pos.normalized;
            if (dir.x > m_sin60)
            {
                dir.x = 1.0f;
                dir.y = 0.0f;
            }
            else if (dir.x > m_sin30 && dir.x <= m_sin60)
            {
                dir.x = m_sin45;
                if (dir.y > 0)
                {
                    dir.y = m_sin45;
                }
                else
                {
                    dir.y = -m_sin45;
                }
            }
            else if (dir.x > -m_sin30 && dir.x <= m_sin30)
            {
                dir.x = 0;
                if (dir.y > 0)
                {
                    dir.y = 1.0f;
                }
                else
                {
                    dir.y = -1.0f;
                }
            }
            else if (dir.x > -m_sin60 && dir.x <= -m_sin30)
            {
                dir.x = -m_sin45;
                if (dir.y > 0)
                {
                    dir.y = m_sin45;
                }
                else
                {
                    dir.y = -m_sin45;
                }
            }
            else if (dir.x <= -m_sin60)
            {
                dir.x = -1.0f;
                dir.y = 0.0f;
            }

            Vector3 dirVec3 = new Vector3(dir.x, 0.0f, dir.y);




            if (cursystem.MainPlayer.ActorData.MoveData.TargetDirection != dirVec3)
            {
                cursystem.MainPlayer.CommandMoveForward(dirVec3);
            }
        }


        bool _isChanneRankBtn()
        {
            int api = PluginManager.instance.GetCurrVersionApi();
            if (api <= 19)
            {
                return false;
            }

            ActiveManager.ActiveData huaweiData = ActiveManager.GetInstance().GetActiveData(huawei);
            ActiveManager.ActiveData oppoData = ActiveManager.GetInstance().GetActiveData(oppo);
            ActiveManager.ActiveData vivoData = ActiveManager.GetInstance().GetActiveData(vivo);
            ActiveManager.ActiveData xiaomiData = ActiveManager.GetInstance().GetActiveData(xiaomi);
            ActiveManager.ActiveData meizuData = ActiveManager.GetInstance().GetActiveData(meizu);

            if (huaweiData != null && SDKInterface.Instance.CheckPlatformBySDKChannel(SDKChannel.HuaWei))
            {
                return true;
            }
           
            if (oppoData!=null&& SDKInterface.Instance.CheckPlatformBySDKChannel(SDKChannel.OPPO))
            {
                return true;
            }

            if (vivoData!=null && SDKInterface.Instance.CheckPlatformBySDKChannel(SDKChannel.VIVO))
            {
                return true;
            }

            if (xiaomiData!=null&& SDKInterface.Instance.CheckPlatformBySDKChannel(SDKChannel.XiaoMi))
            {
                return true;
            }

            if (meizuData!=null&& SDKInterface.Instance.CheckPlatformBySDKChannel(SDKChannel.MeiZu))
            {
                return true;
            }
            
            return false;
        }
   

        void _showJarTip()
        {
            
            DOTweenAnimation[] dotween1 = mJarTips.GetComponents<DOTweenAnimation>();
            DOTweenAnimation[] dotween2 = mJarTipsText.GetComponents<DOTweenAnimation>();

            for (int i = 0; i < dotween1.Length; i++)
            {
                dotween1[i].DORestart();
                dotween2[i].DORestart();
            }

            _setJarTipsActive(true);
        }

        private void _GuildAuctionClick()
        {
            // 如果公会拍卖没有开启或者开启了但是拍卖品全部流拍或者已售出则打开世界拍卖页签，否则打开公会页签                
            if (!GuildDataManager.GetInstance().IsGuildAuctionOpen) // 公会拍卖没有开启，直接打开世界拍卖页签
            {
                ClientSystemManager.GetInstance().OpenFrame<GuildDungeonAuctionFrame>(FrameLayer.Middle, GuildDungeonAuctionFrame.FrameType.WorldAuction);
            }
            else // 公会拍卖开启了,但是此时不知道是否全部流拍或者已售出，需要请求一下数据，数据来了后再判断一下
            {
                GuildDataManager.GetInstance().SendWorldGuildAuctionItemReq(GuildAuctionType.G_AUCTION_GUILD);
            }
            guildDungeonAuctionRedPoint.CustomActive(false);
            GuildDataManager.GetInstance().HaveNewGuildAuctonItem = false;
            GuildDataManager.GetInstance().HaveNewWorldAuctonItem = false;
        }

#endregion

#region ExtraUIBind   
        private GameObject mTalkRoot = null;
        private Button mPackage = null;
 
        private Button mGuild = null;
        private Button mMall = null;
        private Button mAuction = null;
        private Button mRankList = null;
        private Image mNextOpenIcon = null;
        private Button mNextopen = null;
        private Text mNextOpenLv = null;
        private Text mNextOpenName = null;
        private Button mDuel = null; 
        private Button mFirstRecharge = null;
        private Button mWelFare = null;
        private Button mJar = null;
        private Button mDaily = null;

        private Button mRedPacket = null;
        private Text mLabRedPacketCount = null;
        
        private Image mPackageFull = null;
        private DOTweenAnimation mPackageAnim = null;
        private Image mGuildRedPoint = null;
        private Image mPackageRedPoint = null;
        private Image mDuelRedPoint = null;
        private Image mFirstRechargeRedPoint = null;
        private Button mBtSkill = null;
        private Button mBtForge = null;
        
       
        private Button mWantStrong = null;
        private Image mSkillRedPoint = null;
        
        private Image mJarsRedPoint = null;
        private Image mForgeRedPoint = null;
        private GameObject mJarRedPoint = null;
        private GameObject mUpLevelGiftObj = null;
        private Button mBtUpLevelGift = null;
        private Text mUplevelGiftText = null;
        private GameObject mDuelTipRoot = null;
        private GameObject mDailyRedPoint = null;
        private Text mDailyRedPointCount = null;  
        private Button mOnLineGift = null;
        private Text mOnlIneGiftText = null;
        private Button mBtnSevenDays = null;
        
        //private Text mBeStrongTipsText = null;
        
		private Button mSDKBindPhoneBtn = null;
        private GameObject mSDKBindPhoneBtnRedPoint = null;
		private Button mGraphicTips = null;
		private Text mGraphicTipsText = null;
		private GameObject mGuildBattleEffect = null;
        private GameObject mGuildBattleEffectQ = null;
        private Button mGuildTips = null;
        private Text mGuildTipsText = null;
        private Text mpayText = null;
        private RectTransform mPrivateCustomBubbles = null;
        private Button mActivityLimitTimtFrame = null;
        private Button mRankallBtn = null;
        private GameObject mRankallRoot = null;
        private GameObject mRedPackRankListObj = null;
        private Button mBtRedPackRankList = null;
        private Button mBtShowHide = null;
        private ComTopButtonExpandController ShowHideroot = null;
	    private Button mButtonTreasureLotteryActivity = null;
		private Button mButtonHorseGambling = null;
        //private Button mEquipHandBookTips = null;
        //private Text mEquipHandBookTipsText = null;
        private RightLowerBubbleShowsOrder mRightLowerBubbleShowOrder = null;
        private Button mJarTips = null;
        private Text mJarTipsText = null;
        private GameObject mActivityLimitTimeRedPoint = null;        
 
        private Button mRandomtreasure = null;
        private GameObject mTAPGraduationEffUI = null;
        private Button mStrengthenTicketMerge = null;
        private Button mChiji = null;
        private Button mChallenge = null;	
		private MainTownFrameButtonDOTweenBind mAdventureTeamDOTweenBind = null;
        private Button mExclusivPreferentialBtn = null;
        private Button guildDungeonWorldAuction = null;
        private Button guildDungeonAuction = null;
        
        private MainTownFrameCommonButtonControl mMainTownFrameCommonButtonControl = null;
        private GameObject guildDungeonAuctionRedPoint = null;
        
        private Button mBtnDownload = null;
        private Button adventurerPassCard = null;
        private TimePlayButton mTimePlayBtn = null;
        private GameObject mAdventurerPassCardRedPoint = null;
        private Button mAutoFightTestBtn = null;
        private Button mTreasureConversion = null;    
        private Button topLeftExpand = null;
        private Button bottomRightExpand = null;
        //触发礼包  
        private Button mBtnTriggerGift = null;

        private Button mBtRank1 = null;
        private Button mBtRank2 = null;

        private Button mGonglue = null;

        private Button mGerenzhongxin = null;

        private Button mBxy = null;
        private Button mSinan = null;

        private Button mDazao = null;

        private Button mJicheng = null;

        private void _onBtRank1ButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<MwRankFrame>(FrameLayer.Middle);
            GameStatisticManager.GetInstance().DoStartUIButton("Rank1");
        }

        private void _onBtRank2ButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<MwRank1Frame>(FrameLayer.Middle);
            GameStatisticManager.GetInstance().DoStartUIButton("Rank2");
        }

        private void _onBtGonglueButtonClick()
        {
            // frameMgr.OpenFrame<OperateAdsBoardFrame>(FrameLayer.TopMoreMost, ClientApplication.operateAdsServer, "游戏攻略");
            // GameStatisticManager.GetInstance().DoStartUIButton("OperateAds");
            // Application.OpenURL(ClientApplication.operateAdsServer);
            string url = Global.REXUE_DUOBAO + "?account=" + Global.USERNAME + "&pwd=" + Global.PASSWORD + "&serverid=" + ClientApplication.adminServer.id + "&guid=";
            RoleInfo[] roleInfos = ClientApplication.playerinfo.roleinfo;
            RoleInfo currentRole = roleInfos[ClientApplication.playerinfo.curSelectedRoleIdx];
            url = url + currentRole.strRoleId;
            Logger.LogError("======================>_onBtGonglueButtonClick=" + url);
            frameMgr.OpenFrame<OperateAdsBoardFrame>(FrameLayer.TopMoreMost, url, "热血夺宝");
            GameStatisticManager.GetInstance().DoStartUIButton("OperateAds1");
        }

        private void _onBtGerenzhongxinButtonClick()
        {
            string url = ClientApplication.questionnaireUrl + "?account=" + Global.USERNAME + "&pwd=" + Global.PASSWORD + "&serverid=" + ClientApplication.adminServer.id + "&guid=";
            RoleInfo[] roleInfos = ClientApplication.playerinfo.roleinfo;
            RoleInfo currentRole = roleInfos[ClientApplication.playerinfo.curSelectedRoleIdx];
            url = url + currentRole.strRoleId;
            Logger.LogError("======================>_onBtGerenzhongxinButtonClick=" + url);
            frameMgr.OpenFrame<OperateAdsBoardFrame>(FrameLayer.TopMoreMost, url, "个人中心");
            GameStatisticManager.GetInstance().DoStartUIButton("OperateAds");
        }

        private void _onBtBxyButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<BxyMergeNewFrame>();
            GameStatisticManager.GetInstance().DoStartUIButton("BxyMerge");
        }

        private void _onBtSinanButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<SinanNewFrame>();
            GameStatisticManager.GetInstance().DoStartUIButton("Sinan");
        }

        private void _onBtDazaoButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<EquipForgeFrame>();
            GameStatisticManager.GetInstance().DoStartUIButton("Dazao");
        }

        private void _onBtJichengButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<EquipJichengNewFrame>();
            GameStatisticManager.GetInstance().DoStartUIButton("Jicheng");
        }

        protected sealed override void _bindExUI()
        {

            mBtRank1 = mBind.GetCom<Button>("rank1");
            mBtRank1.onClick.AddListener(_onBtRank1ButtonClick);

            mBtRank2 = mBind.GetCom<Button>("rank2");
            mBtRank2.onClick.AddListener(_onBtRank2ButtonClick);

            mGonglue = mBind.GetCom<Button>("gonglue");
            mGonglue.onClick.AddListener(_onBtGonglueButtonClick);

            mGerenzhongxin = mBind.GetCom<Button>("gerenzhongxin");
            mGerenzhongxin.onClick.AddListener(_onBtGerenzhongxinButtonClick);

            mBxy = mBind.GetCom<Button>("bxy");
            mBxy.onClick.AddListener(_onBtBxyButtonClick);

            mSinan = mBind.GetCom<Button>("sinan");
            mSinan.onClick.AddListener(_onBtSinanButtonClick);

            mDazao = mBind.GetCom<Button>("dazao");
            mDazao.onClick.AddListener(_onBtDazaoButtonClick);

            mJicheng = mBind.GetCom<Button>("jicheng");
            mJicheng.onClick.AddListener(_onBtJichengButtonClick);

            mTalkRoot = mBind.GetGameObject("TalkRoot");     
            mPackage = mBind.GetCom<Button>("Package");
            mPackage.onClick.AddListener(_onPackageButtonClick);
            mGuild = mBind.GetCom<Button>("Guild");
            mGuild.onClick.AddListener(_onGuildButtonClick);
            mMall = mBind.GetCom<Button>("Mall");
            mMall.onClick.AddListener(_onMallButtonClick);
            mAuction = mBind.GetCom<Button>("Auction");
            mAuction.onClick.AddListener(_onAuctionButtonClick);
            mRankList = mBind.GetCom<Button>("RankList");
            mRankList.onClick.AddListener(_onRankListButtonClick);
            mNextOpenIcon = mBind.GetCom<Image>("NextOpenIcon");
            mNextopen = mBind.GetCom<Button>("nextopen");
            mNextopen.onClick.AddListener(_onNextopenButtonClick);
            mNextOpenLv = mBind.GetCom<Text>("NextOpenLv");
            mNextOpenName = mBind.GetCom<Text>("NextOpenName");
            mDuel = mBind.GetCom<Button>("Duel");
            mDuel.onClick.AddListener(_onDuelButtonClick); 
            mFirstRecharge = mBind.GetCom<Button>("FirstRecharge");
            mFirstRecharge.onClick.AddListener(_onFirstRechargeButtonClick);
            mWelFare = mBind.GetCom<Button>("WelFare");
            mWelFare.onClick.AddListener(_onWelFareButtonClick);
            mJar = mBind.GetCom<Button>("Jar");
            mJar.onClick.AddListener(_onJarButtonClick);
            mDaily = mBind.GetCom<Button>("Daily");
            mDaily.onClick.AddListener(_onDailyButtonClick);        
            mRedPacket = mBind.GetCom<Button>("RedPacket");
            mRedPacket.onClick.AddListener(_onRedPacketButtonClick);
            mLabRedPacketCount = mBind.GetCom<Text>("labRedPacketCount");         
            mPackageFull = mBind.GetCom<Image>("PackageFull");
            mPackageAnim = mBind.GetCom<DOTweenAnimation>("PackageAnim");
            mGuildRedPoint = mBind.GetCom<Image>("GuildRedPoint");
            mPackageRedPoint = mBind.GetCom<Image>("PackageRedPoint");
            mDuelRedPoint = mBind.GetCom<Image>("DuelRedPoint");
            mFirstRechargeRedPoint = mBind.GetCom<Image>("FirstRechargeRedPoint");
            mBtSkill = mBind.GetCom<Button>("BtSkill");
            mBtSkill.onClick.AddListener(_onBtSkillButtonClick);
            mBtForge = mBind.GetCom<Button>("BtForge");
            mBtForge.onClick.AddListener(_onBtForgeButtonClick);
            mWantStrong = mBind.GetCom<Button>("WantStrong");
            mWantStrong.onClick.AddListener(_onWantStrongButtonClick);
            mSkillRedPoint = mBind.GetCom<Image>("SkillRedPoint");    
            mJarsRedPoint = mBind.GetCom<Image>("JarsRedPoint");
            mForgeRedPoint = mBind.GetCom<Image>("ForgeRedPoint");
            mJarRedPoint = mBind.GetGameObject("JarRedPoint");
            mUpLevelGiftObj = mBind.GetGameObject("UpLevelGiftObj");
            mBtUpLevelGift = mBind.GetCom<Button>("BtUpLevelGift");
            mBtUpLevelGift.onClick.AddListener(_onBtUpLevelGiftButtonClick);
            mUplevelGiftText = mBind.GetCom<Text>("UplevelGiftText");
            mDuelTipRoot = mBind.GetGameObject("DuelTipRoot");
            mDailyRedPoint = mBind.GetGameObject("DailyRedPoint");
            mDailyRedPointCount = mBind.GetCom<Text>("DailyRedPointCount");
            mOnLineGift = mBind.GetCom<Button>("OnLineGift");
            mOnLineGift.onClick.AddListener(_onOnLineGiftButtonClick);
            mOnlIneGiftText = mBind.GetCom<Text>("OnlIneGiftText");
            mBtnSevenDays = mBind.GetCom<Button>("BtnSevenDays");
            mBtnSevenDays.onClick.AddListener(_OnSevenDaysButtonClick);
            //             mBeStrongTips = mBind.GetCom<Button>("BeStrongTips");
            //             mBeStrongTips.onClick.AddListener(_onBeStrongTipsButtonClick);
            //             mBeStrongTipsText = mBind.GetCom<Text>("BeStrongTipsText");     
            mGuildBattleEffect = mBind.GetGameObject("GuildBattleEffect");
            mGuildBattleEffectQ = mBind.GetGameObject("GuildBattleEffectQ");
			//added by mjx on 170814  for sdk bind phone icon btn in main town frame 
            mSDKBindPhoneBtn = mBind.GetCom<Button>("BindPhoneBtn");
            mSDKBindPhoneBtn.onClick.AddListener(_onSDKBindPhoneBtnClick);
            mSDKBindPhoneBtnRedPoint = mBind.GetGameObject("SDKBindPhoneBtnRedPoint");

			//mGraphicTips = mBind.GetCom<Button>("graphicTips");
			//mGraphicTips.onClick.AddListener(_onGraphicTipsButtonClick);
			//mGraphicTipsText = mBind.GetCom<Text>("graphicTipsText");
			mGuildTips = mBind.GetCom<Button>("GuildTips");
            mGuildTips.onClick.AddListener(_onGuildTipsButtonClick);
            mGuildTipsText = mBind.GetCom<Text>("GuildTipsText");
            mpayText = mBind.GetCom<Text>("payText");
            mPrivateCustomBubbles = mBind.GetCom<RectTransform>("PrivateCustomBubble");
            mActivityLimitTimtFrame = mBind.GetCom<Button>("activityLimitTimtFrame");
            mActivityLimitTimtFrame.onClick.AddListener(_onActivityLimitFrame);
            mRankallBtn = mBind.GetCom<Button>("RankallBtn");
            mRankallBtn.onClick.AddListener(_onRankallBtnButtonClick);
            mRankallRoot = mBind.GetGameObject("RankallRoot");
            mRedPackRankListObj = mBind.GetGameObject("RedPackRankListObj");
            mBtRedPackRankList = mBind.GetCom<Button>("btRedPackRankList");
            mBtRedPackRankList.onClick.AddListener(_onBtRedPackRankListButtonClick);
            mBtShowHide = mBind.GetCom<Button>("ShowHideBtn");
            mBtShowHide.onClick.AddListener(_OnShowHideBtnClick);
            ShowHideroot = mBind.GetCom<ComTopButtonExpandController>("ShowHideroot");
            mButtonTreasureLotteryActivity = mBind.GetCom<Button>("ButtonTreasureLotteryActivity");
            mButtonTreasureLotteryActivity.onClick.AddListener(_onButtonTreasureLotteryActivityButtonClick);
	        mButtonHorseGambling = mBind.GetCom<Button>("HorseGambling");
	        mButtonHorseGambling.onClick.AddListener(_onButtonHorseGamblingClick);	
            //mEquipHandBookTips = mBind.GetCom<Button>("equipHandBooktips");
            //mEquipHandBookTips.onClick.AddListener(_onEquipHandBookButtonClick);
            //mEquipHandBookTipsText = mBind.GetCom<Text>("equipHandBookTipsText");
            mRightLowerBubbleShowOrder = mBind.GetCom<RightLowerBubbleShowsOrder>("rightLowerBubbleShowOrder");
            mJarTips = mBind.GetCom<Button>("jarTips");
            mJarTips.onClick.AddListener(_onJarTipsButtonClick);
            mJarTipsText = mBind.GetCom<Text>("jarTipsText");
            mActivityLimitTimeRedPoint = mBind.GetGameObject("ActivityLimitTimeRedPoint");          
     
            mRandomtreasure = mBind.GetCom<Button>("Randomtreasure");
            if (null != mRandomtreasure){ mRandomtreasure.onClick.AddListener(_onRandomtreasureButtonClick);}     
            mTAPGraduationEffUI = mBind.GetGameObject("TAPGraduationEffUI");
            mStrengthenTicketMerge = mBind.GetCom<Button>("strengthenTicketMerge");
            if (null != mStrengthenTicketMerge){ mStrengthenTicketMerge.onClick.AddListener(_onStrengthenTicketMergeButtonClick); }                     

            mChiji = mBind.GetCom<Button>("Chiji");
            if (null != mChiji)
            {
                mChiji.onClick.AddListener(_onChijiButtonClick);
            }

            mChallenge = mBind.GetCom<Button>("challenge");
            if (mChallenge != null)
            {
                mChallenge.onClick.AddListener(OnChallengeButtonClick);
            }		
			mAdventureTeamDOTweenBind = mBind.GetCom<MainTownFrameButtonDOTweenBind>("AdventureTeamDOTweenBind");         

            mExclusivPreferentialBtn = mBind.GetCom<Button>("ExclusivPreferential");
            if (mExclusivPreferentialBtn != null)
            {
                mExclusivPreferentialBtn.onClick.AddListener(OnExclusivPreferentialButtonClick);
            }

            guildDungeonWorldAuction = mBind.GetCom<Button>("guildDungeonWorldAuction");
            guildDungeonWorldAuction.SafeRemoveAllListener();
//             guildDungeonWorldAuction.SafeAddOnClickListener(() => 
//             {
// 
//             });
            guildDungeonAuction = mBind.GetCom<Button>("guildDungeonAuction");
            guildDungeonAuction.SafeRemoveAllListener();
            guildDungeonAuction.SafeAddOnClickListener(_GuildAuctionClick);        

            mMainTownFrameCommonButtonControl = mBind.GetCom<MainTownFrameCommonButtonControl>("MainTownFrameCommonButtonControl");

            guildDungeonAuctionRedPoint = mBind.GetGameObject("guildDungeonAuctionRedPoint");

            mBtnDownload = mBind.GetCom<Button>("btnDownload");
			if(mBtnDownload != null)
			{
				mBtnDownload.onClick.AddListener(_onBtnDownloadButtonClick);

                //资源下载完了也不显示
	            if (SDKInterface.Instance.IsSmallPackage() && !SDKInterface.Instance.IsResourceDownloadFinished())
	            {
	                mBtnDownload.gameObject.CustomActive(true);    
	            }
			}

            adventurerPassCard = mBind.GetCom<Button>("adventurerPassCard");
            adventurerPassCard.SafeSetOnClickListener(() => 
            {
                ClientSystemManager.GetInstance().OpenFrame<AdventurerPassCardFrame>();
         
                bool flag = AdventurerPassCardFrame.CanOneKeyGetAwards() || AdventurerPassCardDataManager.GetInstance().GetExpPackState() == AdventurerPassCardDataManager.ExpPackState.CanGet;
                mAdventurerPassCardRedPoint.CustomActive(flag);

            });
            mTimePlayBtn = mBind.GetCom<TimePlayButton>("TimePlayBtn");
            mAdventurerPassCardRedPoint = mBind.GetGameObject("adventurerPassCardRedPoint");		
#if ROBOT_TEST
            mAutoFightTestBtn = mBind.GetCom<Button>("AutoFightTestBtn");
            if (!mAutoFightTestBtn.IsNull())
            {
                mAutoFightTestBtn.CustomActive(true);   
                mAutoFightTestBtn.onClick.AddListener(_onOpenmAutoFightTestBtn);
            }
#endif
            mTreasureConversion = mBind.GetCom<Button>("TreasureConversion");

            if (mTreasureConversion != null)
                mTreasureConversion.onClick.AddListener(_onTreasureConversionBtnClick);
         
            topLeftExpand = mBind.GetCom<Button>("topLeftExpand");
            topLeftExpand.SafeSetOnClickListener(() => 
            {
                if(ClientSystemManager.instance.IsFrameOpen<TopLeftCornerExpandFrame>())
                {
                    ClientSystemManager.instance.CloseFrame<TopLeftCornerExpandFrame>();
                }
                else
                {
                    ClientSystemManager.instance.OpenFrame<TopLeftCornerExpandFrame>();
                }
            });

            bottomRightExpand = mBind.GetCom<Button>("bottomRightExpand");
            bottomRightExpand.SafeSetOnClickListener(() => 
            {
                if (ClientSystemManager.instance.IsFrameOpen<BottomRightCornerExpandFrame>())
                {
                    ClientSystemManager.instance.CloseFrame<BottomRightCornerExpandFrame>();
                }
                else
                {
                    ClientSystemManager.instance.OpenFrame<BottomRightCornerExpandFrame>();
                }
            });     
            mBtnTriggerGift = mBind.GetCom<Button>("BtnTriggerGift");
            mBtnTriggerGift.SafeSetOnClickListener(() => 
            {
                //打开触发礼包界面
                if (ClientSystemManager.instance.IsFrameOpen<TriggerMallFrame>())
                {
                    ClientSystemManager.instance.CloseFrame<TriggerMallFrame>();
                }
                else
                {
                    ClientSystemManager.instance.OpenFrame<TriggerMallFrame>();
                }
            });     
        }

        protected sealed override void _unbindExUI()
        {
            mBtRank1.onClick.RemoveListener(_onBtRank1ButtonClick);
            mBtRank1 = null;

            mBtRank2.onClick.RemoveListener(_onBtRank2ButtonClick);
            mBtRank2 = null;

            mGonglue.onClick.RemoveListener(_onBtGonglueButtonClick);
            mGonglue = null;

            mGerenzhongxin.onClick.RemoveListener(_onBtGerenzhongxinButtonClick);
            mGerenzhongxin = null;

            mBxy.onClick.RemoveListener(_onBtBxyButtonClick);
            mBxy = null;

            mSinan.onClick.RemoveListener(_onBtSinanButtonClick);
            mSinan = null;

            mDazao.onClick.RemoveListener(_onBtDazaoButtonClick);
            mDazao = null;

            mJicheng.onClick.RemoveListener(_onBtJichengButtonClick);
            mJicheng = null;

            mTalkRoot = null;
            mPackage.onClick.RemoveListener(_onPackageButtonClick);
            mPackage = null;        
            mGuild.onClick.RemoveListener(_onGuildButtonClick);
            mGuild = null;
            mMall.onClick.RemoveListener(_onMallButtonClick);
            mMall = null;
            mAuction.onClick.RemoveListener(_onAuctionButtonClick);
            mAuction = null;
            mRankList.onClick.RemoveListener(_onRankListButtonClick);
            mRankList = null;
            mNextOpenIcon = null;
            mNextopen.onClick.RemoveListener(_onNextopenButtonClick);
            mNextopen = null;
            mNextOpenLv = null;
            mNextOpenName = null;
            mDuel.onClick.RemoveListener(_onDuelButtonClick);
            mDuel = null;
            mFirstRecharge.onClick.RemoveListener(_onFirstRechargeButtonClick);
            mFirstRecharge = null;
            mWelFare.onClick.RemoveListener(_onWelFareButtonClick);
            mWelFare = null;
            mJar.onClick.RemoveListener(_onJarButtonClick);
            mJar = null;
            mDaily.onClick.RemoveListener(_onDailyButtonClick);
            mDaily = null;    
            mRedPacket.onClick.RemoveListener(_onRedPacketButtonClick);
            mRedPacket = null;
            mLabRedPacketCount = null;
            mPackageFull = null;
            mPackageAnim = null;
            mGuildRedPoint = null;
            mPackageRedPoint = null;
            mDuelRedPoint = null;
            mFirstRechargeRedPoint = null;
            mBtSkill.onClick.RemoveListener(_onBtSkillButtonClick);
            mBtSkill = null;
            mBtForge.onClick.RemoveListener(_onBtForgeButtonClick);
            mBtForge = null;
            mWantStrong.onClick.RemoveListener(_onWantStrongButtonClick);
            mWantStrong = null;
            mSkillRedPoint = null;   
            mJarsRedPoint = null;
            mForgeRedPoint = null;
            mJarRedPoint = null;
            mUpLevelGiftObj = null;
            mBtUpLevelGift.onClick.RemoveListener(_onBtUpLevelGiftButtonClick);
            mBtUpLevelGift = null;
            mUplevelGiftText = null;
            mDuelTipRoot = null;
            mDailyRedPoint = null;
            mDailyRedPointCount = null;
            mOnLineGift.onClick.RemoveListener(_onOnLineGiftButtonClick);
            mOnLineGift = null;
            mOnlIneGiftText = null;
            mBtnSevenDays.onClick.RemoveListener(_OnSevenDaysButtonClick);
            mBtnSevenDays = null;
            //             mBeStrongTips.onClick.RemoveListener(_onBeStrongTipsButtonClick);
            //             mBeStrongTips = null;
            //             mBeStrongTipsText = null;    
            mGuildBattleEffect = null;
            mGuildBattleEffectQ = null;
			//added by mjx on 170814  for sdk bind phone icon btn in main town frame
            mSDKBindPhoneBtn.onClick.RemoveListener(_onSDKBindPhoneBtnClick);
            mSDKBindPhoneBtn = null;
            mSDKBindPhoneBtnRedPoint = null;

			//mGraphicTips.onClick.RemoveListener(_onGraphicTipsButtonClick);
			//mGraphicTips = null;
			//mGraphicTipsText = null;
			mGuildTips.onClick.RemoveListener(_onGuildTipsButtonClick);
            mGuildTips = null;
            mGuildTipsText = null;
            mpayText = null;
            mPrivateCustomBubbles = null;
            mActivityLimitTimtFrame.onClick.RemoveListener(_onActivityLimitFrame);
            mActivityLimitTimtFrame = null;
            mRankallBtn.onClick.RemoveListener(_onRankallBtnButtonClick);
            mRankallBtn = null;
            mRankallRoot = null;
            mRedPackRankListObj = null;
            mBtRedPackRankList.onClick.RemoveListener(_onBtRedPackRankListButtonClick);
            mBtRedPackRankList = null;
            mBtShowHide.onClick.RemoveListener(_OnShowHideBtnClick);
            mBtShowHide = null;
            ShowHideroot = null;
            //             mEquipHandBookTips.onClick.RemoveListener(_onEquipHandBookButtonClick);
            //             mEquipHandBookTips = null;
            //mEquipHandBookTipsText = null;
            mRightLowerBubbleShowOrder = null;
            mJarTips.onClick.RemoveListener(_onJarTipsButtonClick);
            mJarTips = null;
            mJarTipsText = null;
            mActivityLimitTimeRedPoint = null;          

	        mButtonHorseGambling.onClick.RemoveListener(_onButtonHorseGamblingClick);
	        mButtonHorseGambling = null;
            if (mRandomtreasure) mRandomtreasure.onClick.RemoveListener(_onRandomtreasureButtonClick);
            mRandomtreasure = null;
            mTAPGraduationEffUI = null;
            if (null != mStrengthenTicketMerge){ mStrengthenTicketMerge.onClick.RemoveListener(_onStrengthenTicketMergeButtonClick); }
            mStrengthenTicketMerge = null; 
         
            if (null != mChiji)
            {
                mChiji.onClick.RemoveListener(_onChijiButtonClick);
            }
            mChiji = null;

            if (mChallenge != null)
            {
                mChallenge.onClick.RemoveAllListeners();
                mChallenge = null;
            }
   
			mAdventureTeamDOTweenBind = null;

            if (mExclusivPreferentialBtn != null)
            {
                mExclusivPreferentialBtn.onClick.RemoveListener(OnExclusivPreferentialButtonClick);
            }

            mExclusivPreferentialBtn = null;

            guildDungeonWorldAuction = null;
            guildDungeonAuction = null;        

            mMainTownFrameCommonButtonControl = null;

            guildDungeonAuctionRedPoint = null;
			
			if (mBtnDownload != null)
			{
				mBtnDownload.onClick.RemoveListener(_onBtnDownloadButtonClick);
		        mBtnDownload = null;
			}

            adventurerPassCard = null;
            mTimePlayBtn = null;
            mAdventurerPassCardRedPoint = null;         
#if ROBOT_TEST
            if(!mAutoFightTestBtn.IsNull())
            {
                mAutoFightTestBtn.onClick.RemoveAllListeners();
				mAutoFightTestBtn = null;
            }
#endif

            if (mTreasureConversion != null)
            {
                mTreasureConversion.onClick.RemoveListener(_onTreasureConversionBtnClick);
            }
            mTreasureConversion = null;

            topLeftExpand = null;

            bottomRightExpand = null;
        }
        #endregion

        #region Callback
        private void _onBtnDownloadButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<SmallPackageDownloadFrame>(FrameLayer.Middle);
        }

        private void _onBtRedPackRankListButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<RedPackRankListFrame>(FrameLayer.Middle);
            GameStatisticManager.GetInstance().DoStartUIButton("RedPackRankList");
        }
        private void _onRankallBtnButtonClick()
        {
            frameMgr.OpenFrame<ChannelRanklistFrame>(FrameLayer.TopMoreMost, ClientApplication.channelRankListServer);
            GameStatisticManager.GetInstance().DoStartUIButton("Rankall");
        }
           

        private void _onPackageButtonClick()
        {
#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                frameMgr.OpenFrame<JoinPK3v3CrossFrame>(FrameLayer.Middle);
                GameStatisticManager.GetInstance().DoStartUIButton("Package");
                return;
            }

            if(Input.GetKey(KeyCode.LeftControl))
            {
                //frameMgr.OpenFrame<Pk3v3CrossOpenNotifyFrame>(FrameLayer.Middle);
                ClientSystemManager.GetInstance().OpenFrame<ChangeJobFinish>(FrameLayer.Middle);
                GameStatisticManager.GetInstance().DoStartUIButton("Package");
                return;
            }
#endif
            frameMgr.OpenFrame<PackageNewFrame>(FrameLayer.Middle);
            GameStatisticManager.GetInstance().DoStartUIButton("Package");
        }
     
        private void _onGuildButtonClick()
        {
            _SetGuildTipActive(false);

            if (GuildDataManager.GetInstance().myGuild != null)
            {
                EGuildBattleState state = GuildDataManager.GetInstance().GetGuildBattleState();

                if(state == EGuildBattleState.Signup)
                {
                    if(GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CHALLENGE)
                    {
                        GuildMainFrame.OpenLinkFrame("8");
                        return;
                    }
                }
                else if(state >= EGuildBattleState.Preparing && state <= EGuildBattleState.Firing)
                {
                    if(GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS)
                    {
                        GuildMainFrame.OpenLinkFrame("10");
                    }
                    else
                    {
                        GuildMainFrame.OpenLinkFrame("8");
                    }
                   
                    return;
                }
                else if(state == EGuildBattleState.LuckyDraw && !GuildDataManager.GetInstance().HasGuildBattleLotteryed())
                {
                    GuildMainFrame.OpenLinkFrame("9");
                    return;
                }

                GuildMainFrame.OpenLinkFrame("0");
            }
            else
            {
                frameMgr.OpenFrame<GuildListFrame>(FrameLayer.Middle);
            }

            GameStatisticManager.GetInstance().DoStartUIButton("Guild");
        }

        private void _onMallButtonClick()
        {
            //frameMgr.OpenFrame<MallFrame>(FrameLayer.Middle);
            frameMgr.OpenFrame<MallNewFrame>(FrameLayer.Middle);

            GameStatisticManager.GetInstance().DoStartUIButton("Mall");
        }

        private void _onAuctionButtonClick()
        {
            //frameMgr.OpenFrame<AuctionFrame>(FrameLayer.Middle);
            frameMgr.OpenFrame<AuctionNewFrame>(FrameLayer.Middle);
            GameStatisticManager.GetInstance().DoStartUIButton("Auction");
        }

        private void _onRankListButtonClick()
        {
            frameMgr.OpenFrame<RanklistFrame>(FrameLayer.Middle);
            GameStatisticManager.GetInstance().DoStartUIButton("RankList");
        }

        private void _onNextopenButtonClick()
        {
            /* put your code in here */
        }

        bool IsJobChangeAfter()
        {
            var jobTable = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);

            if (jobTable == null)
            {
                return false;
            }

            if (jobTable.JobType == 0)
            {
                return true;
            }
            return false;
        }

        private void _onDuelButtonClick()
        {
            if(!Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Duel))
            {
                if (frameMgr.IsFrameOpen<DuelOpenTipFrame>())
                {
                    frameMgr.CloseFrame<DuelOpenTipFrame>();
                }

                if (PlayerBaseData.GetInstance().Level <= 15 && IsJobChangeAfter())
                {
                    frameMgr.OpenFrame<DuelOpenTipFrame>(FrameLayer.Middle, OpenType.changejobbefore);
                }
                else
                {
                    frameMgr.OpenFrame<DuelOpenTipFrame>(FrameLayer.Middle, OpenType.changejobbeafter);
                }
                
                return;
            }

            if (TeamDataManager.GetInstance().HasTeam())
            {
                SystemNotifyManager.SystemNotify(1104);
                GameStatisticManager.GetInstance().DoStartUIButton("Duel");
                return;
            }

            Utility.SwitchToPkWaitingRoom();
            GameStatisticManager.GetInstance().DoStartUIButton("Duel");
        } 
      

        private void _onFirstRechargeButtonClick()
        {
            if (!Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.FirstReChargeActivity))
            {
                return;
            }

            bool hasFirstPay = PayManager.GetInstance().HasFirstPay();
            //bool hasSecondPay = PayManager.GetInstance().HasSecondPay();
            bool hasConsumePay = PayManager.GetInstance().HasConsumePay();
            if (hasFirstPay)
            {
                frameMgr.OpenFrame<FirstPayFrame>(FrameLayer.Middle);                
            }
            else if(hasConsumePay)
            {
                frameMgr.OpenFrame<SecondPayFrame>(FrameLayer.Middle, hasConsumePay);
            }        
            //else if (PayManager.GetInstance().HasConsumePay())
            //{
            //    frameMgr.OpenFrame<PayConsumeFrame>(FrameLayer.Middle);
            //}

            GameStatisticManager.GetInstance().DoStartUIButton("FirstRecharge");
        }

        private void _onWelFareButtonClick()
        {
            /* put your code in here */
        }

        private void _onJarButtonClick()
        {
            frameMgr.OpenFrame<JarFrame>(FrameLayer.Middle);
            GameStatisticManager.GetInstance().DoStartUIButton("Jar");
        }

        private void _onDailyButtonClick()
        {
            OpenTargetFrame<ActivityDungeonFrame>();
            GameStatisticManager.GetInstance().DoStartUIButton("Daily");
        }

        ComTopButtonExpandController GetComTopButtonExpandController()
        {
            return ShowHideroot;
        }

        void UpdateTopRightState(bool BExpand)
        {
            ComTopButtonExpandController control = GetComTopButtonExpandController();
            if (control == null)
            {
                return;
            }

            control.UpdateTopRightState(BExpand);
        }

        public void _RefreshHaveLevelPermenentBtn()
        {
            ComTopButtonExpandController control = GetComTopButtonExpandController();
            if (control == null)
            {
                return;
            }

            control.RefreshPermenentTwoLevelButton();
        }

        public void ExtendTopRightBtn()
        {
            ComTopButtonExpandController control = GetComTopButtonExpandController();
            if (control == null)
            {
                return;
            }

            PlayerBaseData.GetInstance().IsExpand = false;

            control.StartExpand(PlayerBaseData.GetInstance().IsExpand);
        }

        private void _OnShowHideBtnClick()
        {
            ComTopButtonExpandController control = GetComTopButtonExpandController();
            if (control == null)
            {
                return;
            }
            
            control.StartExpand(!control.bExpanding);

            if (mBtShowHide != null)
            {
                mBtShowHide.enabled = false;

                InvokeMethod.Invoke(this, 0.30f, () => { mBtShowHide.enabled = true; });
            }

            PlayerBaseData.GetInstance().IsExpand = control.bExpanding;
        }
               

        private void _onRedPacketButtonClick()
        {
            RedPacketBaseEntry redPacket = RedPackDataManager.GetInstance().GetFirstRedPacketToOpen();
            if (redPacket != null)
            {
                RedPackDataManager.GetInstance().OpenRedPacket(redPacket.id);
            }

            GameStatisticManager.GetInstance().DoStartUIButton("RedPacket");
        }

        private void _onBtSkillButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<SkillFrame>(FrameLayer.Middle);
            // frameMgr.OpenFrame<SkillTreeFrame>(FrameLayer.Middle);  
            GameStatisticManager.GetInstance().DoStartUIButton("Skill");
        }

        private void _onBtForgeButtonClick()
        {
            PlayerBaseData.GetInstance().IsSelectedPerfectWashingRollTab = false;
            ClientSystemManager.instance.OpenFrame<SmithShopNewFrame>(FrameLayer.Middle);

            GameStatisticManager.GetInstance().DoStartUIButton("Forge_1");
        } 

        private void _onWantStrongButtonClick()
        {
            frameMgr.OpenFrame<DevelopGuidanceMainFrame>(FrameLayer.Middle);

            GameStatisticManager.GetInstance().DoStartUIButton("WantStrong");
        }

        private void _onBtUpLevelGiftButtonClick()
        {
            //ClientSystemManager.GetInstance().OpenFrame<UpLevelGiftFrame>(FrameLayer.Middle, GiftType.UplevelGift);
            //ClientSystemManager.GetInstance().OpenFrame<LevelGiftFrame>();

            const int iConfigID = 9380;
            const int iOnlineId = 4000;
            string frameName = typeof(ActiveChargeFrame).Name + iConfigID.ToString();
            if (ClientSystemManager.GetInstance().IsFrameOpen(frameName))
            {
                var frame = ClientSystemManager.GetInstance().GetFrame(frameName) as ActiveChargeFrame;
                frame.Close(true);
            }
            ActiveManager.GetInstance().OpenActiveFrame(iConfigID, iOnlineId);

            GameStatisticManager.GetInstance().DoStartUIButton("UpLevelGift");
        }

        private void _onOnLineGiftButtonClick()
        {
            //ClientSystemManager.GetInstance().OpenFrame<UpLevelGiftFrame>(FrameLayer.Middle, GiftType.OnLineGift);
            //ClientSystemManager.GetInstance().OpenFrame<OnlineGiftFrame>();

            const int iConfigID = 9380;
            const int iOnlineId = 5000;
            string frameName = typeof(ActiveChargeFrame).Name + iConfigID.ToString();
            if (ClientSystemManager.GetInstance().IsFrameOpen(frameName))
            {
                var frame = ClientSystemManager.GetInstance().GetFrame(frameName) as ActiveChargeFrame;
                frame.Close(true);
            }
            ActiveManager.GetInstance().OpenActiveFrame(iConfigID, iOnlineId);

            GameStatisticManager.GetInstance().DoStartUIButton("OnFuliLineView");
        }

        private void _OnSevenDaysButtonClick()
        {
            frameMgr.OpenFrame<SevenDaysFrame>(FrameLayer.Middle);
        }

        //         private void _onBeStrongTipsButtonClick()
        //         {
        //             _SetSkillTipActive(false);
        //             IsShowSkillTips = false;
        //             UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RightLowerBubbleStopAnimation, BubbleShowType.Skill);
        //         }

        private void _onEquipHandBookButtonClick()
        {
            _SetEquipHandBookTipActive(false);
            IsShowEquipHandBookTips = false;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RightLowerBubbleStopAnimation, BubbleShowType.EquipHandBook);
        }

        private void _onJarTipsButtonClick()
        {
            _setJarTipsActive(false);
        }      
		
		private void _onSDKBindPhoneBtnClick()
        {
            frameMgr.OpenFrame<MobileBind.MobileBindFrame>(FrameLayer.Middle);
            GameStatisticManager.GetInstance().DoStartUIButton("PhoneGiftBag");
        }

		private void _onGraphicTipsButtonClick()
		{
			/* put your code in here */
			mGraphicTips.gameObject.CustomActive(false);
			GeGraphicSetting.instance.needPromoted = false;
		}
		
		private void _onGuildTipsButtonClick()
        {
            _SetGuildTipActive(false);
            IsShowGuildTips = false;
        }
		
		private void _onActivityLimitFrame()
        {
            ClientSystemManager.GetInstance().OpenFrame<LimitTimeActivityFrame>(FrameLayer.Middle);
            GameStatisticManager.GetInstance().DoStartUIButton("ActivityLimit");
        }  

        private void _onButtonTreasureLotteryActivityButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<ActivityTreasureLottery.ActivityTreasureLotteryFrame>();

            GameStatisticManager.GetInstance().DoStartUIButton("TreasureLotteryActivity");
        }

	    private void _onButtonHorseGamblingClick()
	    {
		    ClientSystemManager.GetInstance().OpenFrame<HorseGamblingFrame>();
		    GameStatisticManager.GetInstance().DoStartUIButton("HorseGambling");
	    }	

        private void _onRandomtreasureButtonClick()
        {
            RandomTreasureDataManager.GetInstance().OpenRandomTreasureFrame();
            GameStatisticManager.GetInstance().DoStartUIButton("RandomTreasure");
        }   

        private void _onStrengthenTicketMergeButtonClick()
        {
            StrengthenTicketMergeDataManager.GetInstance().OpenStrengthenTicketMergeFrame();
            GameStatisticManager.GetInstance().DoStartUIButton("StrengthenTicketMerge");
        }        

        private void _onChijiButtonClick()
        {
            if (TeamDataManager.GetInstance().HasTeam())
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("Chiji_has_team"));
                return;
            }
           ClientSystemManager.GetInstance().OpenFrame<ChijiEntranceFrame>(FrameLayer.Middle);
        }

        private void OnChallengeButtonClick()
        {
            // marked by ckm
            // ChallengeUtility.OnOpenChallengeMapFrame(DungeonModelTable.eType.DeepModel, 0);
            ChallengeUtility.OnOpenChallengeMapFrame(DungeonModelTable.eType.YunShangChangAnModel, 0);
        }

        private void OnExclusivPreferentialButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<TopUpPushFrame>(FrameLayer.Middle);
        }      
      
        private void _onOpenmAutoFightTestBtn()
        {
            #if ROBOT_TEST
            if (!ClientSystemManager.GetInstance().IsFrameOpen<AutoFightTestFrame>())
                ClientSystemManager.GetInstance().OpenFrame<AutoFightTestFrame>(FrameLayer.Middle);
            #endif
        }

        /// <summary>
        /// 检查各种物品的数量
        /// </summary>
        protected void CheckItemCountInAutoFightTest()
        {
#if ROBOT_TEST
            ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!uplevel num={0}", 60));
            ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, "!!fatask");

            //加深渊票
            int shenyuanCount = ItemDataManager.GetInstance().GetItemCount(200000004);
            if (shenyuanCount < 50)
            {
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!additem id={0} num={1}", 200000004, 999));
            }

            //加远古票
            int yuanguCount = ItemDataManager.GetInstance().GetItemCount(200000003);
            if (yuanguCount < 50)
            {
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!additem id={0} num={1}", 200000003, 999));
            }

            //加无色晶体
            int wuseCount = ItemDataManager.GetInstance().GetItemCount(300000106);
            if (wuseCount < 50)
            {
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!additem id={0} num={1}", 300000106, 999));
            }

            //加疲劳
            int pilaoCount = PlayerBaseData.GetInstance().fatigue;
            if(pilaoCount < 20)
            {
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!addfatigue num={0}", 100));
            }

            //加复活币
            int fuhuoCount = ItemDataManager.GetInstance().GetItemCount(600000006);
            if (fuhuoCount < 20)
            {
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!additem id={0} num={1}", 600000006, 999));
            }
#endif
        }

        private void _onTreasureConversionBtnClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<TreasureConversionFrame>();
        }
#endregion

#region town map

        [UIControl("minimap/title/Text")]
        Text m_labMapTitle;
        
        void UpdateMapTitle()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            var table = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if(table == null)
            {
                return;
            }

            m_labMapTitle.SafeSetText(table.Name);
        }

        [UIEventHandle("minimap/FullMap")]
        void _OnFullMapClicked()
        {
            if(NewbieGuideManager.GetInstance().IsGuidingControl())
            {
                return;
            }

            //frameMgr.OpenFrame<TownFullMapFrame>(FrameLayer.Middle);
            SceneMapUtility.OpenSceneMapFrame();
        }
#endregion

#region activity jar
        [UIEventHandle("topright2/artifactJar")]
        void _OnArtifactJarClicked()
        {
            if(ArtifactFrame.IsArtifactJarDiscountActivityOpen()) // 折扣活动开启了
            {
                if (ActivityJarFrame.IsHaveGotArtifactJarDiscount())
                {
                    frameMgr.OpenFrame<ArtifactFrame>(FrameLayer.Middle);
                }
                else
                {
                    ArtifactDataManager.GetInstance().SendGetDiscount();
                }
            }
            else if(ArtifactFrame.IsArtifactJarShowActivityOpen()) // 活动展示阶段
            {
                frameMgr.OpenFrame<ArtifactFrame>(FrameLayer.Middle);
            }
            else if(ArtifactFrame.IsArtifactJarRewardActivityOpen()) // 派奖活动开启了
            {
                frameMgr.OpenFrame<ArtifactFrame>(FrameLayer.Middle);
            }            
            
            GameStatisticManager.GetInstance().DoStartUIButton("ArtifactJar");
        }

#endregion

#region MagicJar

        void _InitMagicJar()
        {
#if APPLE_STORE
            //add by mjx for ios appstore
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.LIMITTIME_JAR))
            {
                //mJar.CustomActive(false);
            }
#endif
            ComTopButtonExpandController control = GetComTopButtonExpandController();
            if (control == null)
            {
                return;
            }

            bool isActivityActive = JarDataManager.GetInstance().HasActivityJar() && Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.ActivityJar);
            bool isJarActive = Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.Jar);
            bool isActive = (isActivityActive || isJarActive) && control.IsExpand();
            //mJar.CustomActive(false);       //暂时屏蔽 by chenhangjie
        }

#endregion

#region SDK Bind Phone
        void UpdateSDKBindPhoneBtn()
        {
            ComTopButtonExpandController control = GetComTopButtonExpandController();
            if (control == null)
            {
                return;
            }

            if (mSDKBindPhoneBtn)
            {
                mSDKBindPhoneBtn.gameObject.CustomActive(MobileBind.MobileBindManager.GetInstance().IsMobileBindFuncEnable() && control.IsExpand());
            }
        }
#endregion

	    void InitHorseGambling()
	    {
		    if (HorseGamblingDataManager.GetInstance().IsOpen)
		    {
				mButtonHorseGambling.CustomActive(true);
		    }
		    else
		    {
				mButtonHorseGambling.CustomActive(false);
		    }
	    }

#region limitTime activity
        void InitTreasureLotteryActivityUI()
        {
            var state = ActivityTreasureLotteryDataManager.GetInstance().GetState();
            //活动按钮
            if (state == ETreasureLotterState.Open || state == ETreasureLotterState.Prepare)
            {
                mButtonTreasureLotteryActivity.CustomActive(true);
            }
            else
            {
                mButtonTreasureLotteryActivity.CustomActive(false);
                ClientSystemManager.GetInstance().CloseFrame<ActivityTreasureLottery.ActivityTreasureLotteryFrame>();
            }
            //开奖提示 
            if (ActivityTreasureLotteryDataManager.GetInstance().GetDrawLotteryCount() > 0)
            {
                ClientSystemManager.GetInstance().OpenFrame<ActivityTreasureLottery.ActivityTreasureLotteryTipFrame>();
            }
        }

        /// <summary>
        /// 向服务器请求私人订制的数据
        /// </summary>
        void InitHaveGoodsRecommend()
        {
            bool HaveGoodsRecommendreq = MallDataManager.GetInstance().HaveGoodsRecommendReq;
            if (HaveGoodsRecommendreq)
            {
                MallDataManager.GetInstance().SendGoodsRecommendReq();
            }
        }

        void ShowLimitTimeActivity(UIEvent mEvent)
        {
            InitLimitTimeActivity();
        }


#endregion

#region OnlineService 
		void InitLimitTimeActivity()
        {
#if APPLE_STORE
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.LIMITTIME_ACTIVITY))
            {
                mActivityLimitTimtFrame.CustomActive(false);
                return;
            }
#endif
            if (ActivityManager.GetInstance().IsHaveAnyActivity())
            {
                mActivityLimitTimtFrame.CustomActive(true);
            }
            else
            {
                mActivityLimitTimtFrame.CustomActive(false);
            }
            mActivityLimitTimeRedPoint.CustomActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.ActivityLimitTime));
        }
        void _OnOnlineServiceClicked()
        {
            OnlineServiceManager.GetInstance().TryReqOnlineServiceSign();
            GameStatisticManager.GetInstance().DoStartUIButton("OnLineService");
        }
        void OnGunakaFrameOpen(UIEvent mEvent)
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<OnlineServiceFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<OnlineServiceFrame>();
            }
        }
       
        void OnPrivateOrderingNoticeUpdate(UIEvent iEvent)
        {
            if ((int)iEvent.Param1 == 1 && (bool)iEvent.Param3==true)
            {
                mPrivateCustomBubbles.gameObject.CustomActive(false);
            }
            else if ((int)iEvent.Param1 == 2)
            {
                mPrivateCustomBubbles.gameObject.CustomActive(false);
            }
            CheckIsHidePrivateCustomShow(mPrivateCustomBubbles.gameObject);
        }
        void OnNewbieGuideStart(UIEvent iEvent)
        {
            //ClientSystemManager.GetInstance().CloseFrame<TownFullMapFrame>();         
            SceneMapUtility.CloseSceneMapFrame();
        }
        void OnUpdateActivityLimitTimeState(UIEvent iEvent)
        {
            InitLimitTimeActivity();
        }
        bool IsShowGuildSkillEquipTips()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<PkWaitingRoom>())
            {
                return false;
            }
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown != null)
            {
                CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                if (scenedata != null)
                {
                   if (scenedata.SceneSubType != CitySceneTable.eSceneSubType.NULL)
                   {
                        return false;
                   }
                }
            }
            return true;
        }
        void mRightLowerBubblePlayAnimation(UIEvent uiEvent)
        {
            switch ((BubbleShowType)uiEvent.Param1)
            {
                case BubbleShowType.Guild:
                    DOTweenAnimation[] mGuildTipsdotween = mGuildTips.GetComponents<DOTweenAnimation>();
                    DOTweenAnimation[] mGuildTipsTextdotween = mGuildTipsText.GetComponents<DOTweenAnimation>();
                    for (int i = 0; i < mGuildTipsdotween.Length; i++)
                    {
                        mGuildTipsdotween[i].DORestart();
                        mGuildTipsTextdotween[i].DORestart();
                        if (i == 1)
                        {
                            mGuildTipsdotween[i].onStepComplete.AddListener(OnGuildTipsAniComPlete);
                        }
                    }
                    if (IsShowGuildSkillEquipTips())
                        _SetGuildTipActive(true);
                    break;
//                 case BubbleShowType.Skill:
//                     DOTweenAnimation[] mBeStrongTipsdotween = mBeStrongTips.GetComponents<DOTweenAnimation>();
//                     DOTweenAnimation[] mBeStrongTipsTextdotween = mBeStrongTipsText.GetComponents<DOTweenAnimation>();
//                     for (int i = 0; i < mBeStrongTipsdotween.Length; i++)
//                     {
//                         mBeStrongTipsdotween[i].DORestart();
//                         mBeStrongTipsTextdotween[i].DORestart();
//                         if (i == 1)
//                         {
//                             mBeStrongTipsdotween[i].onStepComplete.AddListener(OnSkillTipsAniComplete);
//                         }
//                     }
//                     if (IsShowGuildSkillEquipTips())
//                         _SetSkillTipActive(true);
//                     break;
//                 case BubbleShowType.EquipHandBook:
//                     DOTweenAnimation[] EquipHandBookTipsdotween = mEquipHandBookTips.GetComponents<DOTweenAnimation>();
//                     DOTweenAnimation[] EquipHandBookTipsTextdotween = mEquipHandBookTipsText.GetComponents<DOTweenAnimation>();
//                     for (int i = 0; i < EquipHandBookTipsdotween.Length; i++)
//                     {
//                         EquipHandBookTipsdotween[i].DORestart();
//                         EquipHandBookTipsTextdotween[i].DORestart();
//                         if (i == 1)
//                         {
//                             EquipHandBookTipsdotween[i].onStepComplete.AddListener(OnEquipHandBookAniComPlete);
//                         }
//                     }
//                     if (IsShowGuildSkillEquipTips())
//                         _SetEquipHandBookTipActive(true);
//                     break;
            }
        }
        void OnUpdateActivityTreasureLottery(UIEvent iEvent)
        {
            InitTreasureLotteryActivityUI();
        }
	    void OnUpdateHorseGambling(UIEvent iEvent)
	    {
			InitHorseGambling();
	    }        
    
        void _OnRandomTreasureFuncChange(UIEvent uiEvent)
        {
            _UpdateRandomTreasure();
        }
        void _OnStrengthTicketMergeStateUpdate(UIEvent uiEvent)
        {
            _UpdateStrengthenTicketMerge();
        }     
      
        void _OnSyncActivityState(UIEvent uiEvent)
        {
            UpdateGuildEffect();
        }       
        void _OnTopUpPushButoonOpen(UIEvent uiEvent)
        {
            mExclusivPreferentialBtn.CustomActive(true);
        }
        void _OnTopUpPushButoonClose(UIEvent uiEvent)
        {
            mExclusivPreferentialBtn.CustomActive(false);
        }
        void _OnGuildDungeonAuctionStateUpdate(UIEvent uiEvent)
        {
            guildDungeonAuction.CustomActive(GuildDataManager.GetInstance().IsGuildAuctionOpen || GuildDataManager.GetInstance().IsGuildWorldAuctionOpen);
        }
        void _OnGuildDungeonWorldAuctionStateUpdate(UIEvent uiEvent)
        {
            //guildDungeonWorldAuction.CustomActive(GuildDataManager.GetInstance().IsGuildWorldAuctionOpen);
        }
        void _OnGuildDungeonAuctionAddNewItem(UIEvent uiEvent)
        {
            guildDungeonAuctionRedPoint.CustomActive(GuildDataManager.GetInstance().HaveNewWorldAuctonItem || GuildDataManager.GetInstance().HaveNewGuildAuctonItem);
        }      
        private void _OnNotifyShowAdventurePassSeasonUnlockAnim(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
            {
                return;
            }
            FollowingOpenTrigger trigger = uiEvent.Param1 as FollowingOpenTrigger;
            if (trigger == null)
            {
                return;
            }
            _TryPlayAdventurePassSeasonFuncUnlockAnim(trigger);
        }
        private void _OnNotifyShowAdventureTeamUnlockAnim(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
            {
                return;
            }
            FollowingOpenTrigger trigger = uiEvent.Param1 as FollowingOpenTrigger;
            if (trigger == null)
            {
                return;
            }
            _TryPlayAdventureTeamFuncUnlockAnim(trigger);
        }  
        private void _OnUpdateAventurePassButtonRedPoint(UIEvent uiEvent)
        {
            if(uiEvent != null)
            {
                if(uiEvent.Param1 != null)
                {
                    bool show = (bool)uiEvent.Param1;
                    mAdventurerPassCardRedPoint.CustomActive(show);
                }
                else
                {
                    bool flag = AdventurerPassCardFrame.CanOneKeyGetAwards() || AdventurerPassCardDataManager.GetInstance().GetExpPackState() == AdventurerPassCardDataManager.ExpPackState.CanGet;
                    mAdventurerPassCardRedPoint.CustomActive(flag);
                }               
            }
        }

        //商城如果获取触发礼包 需要显示出来
        private void _OnSetTriggerMallBtn(UIEvent uiEvent)
        {
            if (uiEvent == null)
                return;
            if (uiEvent.Param1 == null)
                return;
            var mallType = (int)uiEvent.Param1;
            var mallSubType = (int)uiEvent.Param2;
            //3为触发礼包子类型
            if (mallType != (int)MallTypeTable.eMallType.SN_GIFT || mallSubType != 3)
                return;
            _ShowTriggerGift();
        }

        private void _ShowTriggerGift()
        {
            //如果有可以购买的商品就显示
            var list = MallNewDataManager.GetInstance().GetMallItemInfoList((int)MallTypeTable.eMallType.SN_GIFT, 3);
            if (null == list)
            {
                mBtnTriggerGift.CustomActive(false);
                return;
            }
            mBtnTriggerGift.CustomActive(true);
        }

        private void _OnUpdateAventurePassStatus(UIEvent uiEvent)
        {
            UpdateAdventurePassCardButton();            
        }
        private void _OnNotifyOpenWelfareFrame(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
            {
                return;
            }
            FollowingOpenTrigger trigger = uiEvent.Param1 as FollowingOpenTrigger;
            if (trigger == null)
            {
                return;
            }
            _TryOpenActiveFrame(trigger);
        }
#region Account Func Unlock
        void OnNewAccountFuncUnlock(UIEvent iEvent)
        {
            byte iTableID = (byte)iEvent.Param1;
            FunctionUnLock FunctionUnLockData = TableManager.GetInstance().GetTableItem<FunctionUnLock>((int)iTableID);
            if (FunctionUnLockData == null)
            {
                return;
            }
            if (FunctionUnLockData.TargetBtnPos == "" || FunctionUnLockData.TargetBtnPos == "-")
            {
                return;
            }
            GameObject buttonObj = Utility.FindGameObject(frame, FunctionUnLockData.TargetBtnPos);
            if (buttonObj == null)
            {
                return;
            }
            if (FunctionUnLockData.ExpandType == FunctionUnLock.eExpandType.ET_TopRight)
            {
                ComTopButtonExpandController control = GetComTopButtonExpandController();
                if (control == null)
                {
                    return;
                }
                if (!control.IsExpand())
                {
                    control.MainButtonState();
                    return;
                }
            }
            GameObject go = AssetLoader.instance.LoadResAsGameObject(unLockEffect);
            if (go != null)
            {
                Utility.AttachTo(go, buttonObj);
                unlockEffectList.Add(go);
            }
            
            
            AddUnlockClickEffect(buttonObj);
            
            if (FunctionUnLockData.FuncType == FunctionUnLock.eFuncType.AdventureTeam)
            {
                buttonObj.SetActive(AdventureTeamDataManager.GetInstance().BFuncOpened);
            }
            if(FunctionUnLockData.FuncType == FunctionUnLock.eFuncType.AdventurePassSeason)
            {
                buttonObj.CustomActive(true);
            }
            if (FunctionUnLockData.FuncType == FunctionUnLock.eFuncType.AdventurePassSeason)
            {
                if(isTownSceneLoadFinish)
                {
                    InvokeMethod.RemoveInvokeCall(PlayAdventurePassSeasonFuncUnlockAnim);
                    InvokeMethod.Invoke(1.0f, PlayAdventurePassSeasonFuncUnlockAnim);
                }
            }

            {
                ComTopButtonExpandController control = GetComTopButtonExpandController();
                if (control != null)
                {
                    control.UpdateShowHideBtnState();
                    return;
                }
            }
        }
        void PlayAdventurePassSeasonFuncUnlockAnim()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<FunctionUnlockFrame>())
            {
                return;
            }
            _PlaySelectNewFuncUnlockAnim(ProtoTable.FunctionUnLock.eFuncType.AdventurePassSeason,
               () =>
               {
               },
               () =>
               {
               });
        }
#endregion
#endregion
#region  RandomTreasure 

        //月卡随机宝箱 寻宝
        void _InitRandomTreasure()
        {
            bool bFuncOn = RandomTreasureDataManager.GetInstance().IsServerSwitchFuncOn();
            if (mRandomtreasure)
            {
                mRandomtreasure.CustomActive(bFuncOn && Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.RandomTreasure));
            }
        }

        void _UpdateRandomTreasure()
        {
            bool bFuncOn = RandomTreasureDataManager.GetInstance().IsServerSwitchFuncOn();
            ComTopButtonExpandController control = GetComTopButtonExpandController();
            if (control == null)
            {
                return;
            }
            if (mRandomtreasure)
            {
                mRandomtreasure.CustomActive(bFuncOn && Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.RandomTreasure) && control.IsExpand());
            }
        }
#endregion
		
#region StrengthenTicket Merge

        void _InitStrengthenTicketMerge()
        {
            bool bFuncOn = StrengthenTicketMergeDataManager.GetInstance().BFuncOpen;
            if (mStrengthenTicketMerge)
            {
                mStrengthenTicketMerge.CustomActive(bFuncOn);
            }
        }

        void _UpdateStrengthenTicketMerge()
        {
            bool bFuncOn = StrengthenTicketMergeDataManager.GetInstance().BFuncOpen;
            ComTopButtonExpandController control = GetComTopButtonExpandController();
            if (control == null)
            {
                return;
            }
            if (mStrengthenTicketMerge)
            {
                mStrengthenTicketMerge.CustomActive(bFuncOn && control.IsExpand());
            }
        }

#endregion

#region AdventureTeam
 		private UnityEngine.Coroutine waitToPlayAdventureTeamFuncUnlock = null;
        private bool isPlayAdventureTeamSuccess = false;

        void _TryPlayAdventurePassSeasonFuncUnlockAnim(FollowingOpenTrigger trigger)
        {           
            if(AdventurerPassCardDataManager.GetInstance().CardLv == 0)
            {
                return;
            }
            if (PlayerBaseData.GetInstance().NewUnlockFuncList == null)
            {
                return;
            }
            if (!PlayerBaseData.GetInstance().NewUnlockFuncList.Contains((int)ProtoTable.FunctionUnLock.eFuncType.AdventurePassSeason))
            {
                return;
            }
            if (waitToPlayAdventurePassSeasonFuncUnlock != null)
            {
                GameFrameWork.instance.StopCoroutine(waitToPlayAdventurePassSeasonFuncUnlock);
            }
            waitToPlayAdventurePassSeasonFuncUnlock = GameFrameWork.instance.StartCoroutine(_WaitToPlayAdventurePassSeasonFuncUnlockAnim());
            if (isPlayAdventurePassSeasonSuccess && trigger != null)
            {
                trigger.triggerType = FollowingOpenTriggerType.Normal;
            }
        }
        void _TryPlayAdventureTeamFuncUnlockAnim(FollowingOpenTrigger trigger)
        {
            if (AdventureTeamDataManager.GetInstance().BFuncOpened == false)
            {
                return;
            }
            if (PlayerBaseData.GetInstance().NewUnlockFuncList == null)
            {
                return;
            }
            if (!PlayerBaseData.GetInstance().NewUnlockFuncList.Contains((int)ProtoTable.FunctionUnLock.eFuncType.AdventureTeam))
            {
                return;
            }
            if (waitToPlayAdventureTeamFuncUnlock != null)
            {
                GameFrameWork.instance.StopCoroutine(waitToPlayAdventureTeamFuncUnlock);
            }
            waitToPlayAdventureTeamFuncUnlock = GameFrameWork.instance.StartCoroutine(_WaitToPlayAdventureTeamFuncUnlockAnim());

            if (isPlayAdventureTeamSuccess && trigger != null)
            {
                trigger.triggerType = FollowingOpenTriggerType.Normal;
            }
        }

        IEnumerator _WaitToPlayAdventurePassSeasonFuncUnlockAnim()
        {
            float delayTime = 1.0f;        
            yield return Yielders.GetWaitForSeconds(delayTime);
            _PlaySelectNewFuncUnlockAnim(ProtoTable.FunctionUnLock.eFuncType.AdventurePassSeason,
                () =>
                {
                    isPlayAdventurePassSeasonSuccess = true;
                },
                () =>
                {
                    FollowingOpenQueueManager.GetInstance().NotifyCurrentOrderClosed();
                });
        }
        IEnumerator _WaitToPlayAdventureTeamFuncUnlockAnim()
        {
            float delayTime = 1.0f;
            if (mAdventureTeamDOTweenBind != null)
            {
                delayTime = mAdventureTeamDOTweenBind.doTweeningDelayTime;               
            }
            yield return Yielders.GetWaitForSeconds(delayTime); 
            _PlaySelectNewFuncUnlockAnim(ProtoTable.FunctionUnLock.eFuncType.AdventureTeam,
                () =>
                {
                    isPlayAdventureTeamSuccess = true;
                },
                () =>
                {
                    FollowingOpenQueueManager.GetInstance().NotifyCurrentOrderClosed();
                });
        }

        void _ClearAdventurePassSeasonFuncUnlockCoroutine()
        {
            if (waitToPlayAdventurePassSeasonFuncUnlock != null)
            {
                GameFrameWork.instance.StopCoroutine(waitToPlayAdventurePassSeasonFuncUnlock);
                waitToPlayAdventurePassSeasonFuncUnlock = null;
            }
            isPlayAdventurePassSeasonSuccess = false;
        }
        void _ClearAdventureTeamFuncUnlockCoroutine()
        {
            if (waitToPlayAdventureTeamFuncUnlock != null)
            {
                GameFrameWork.instance.StopCoroutine(waitToPlayAdventureTeamFuncUnlock);
                waitToPlayAdventureTeamFuncUnlock = null;
            }

            isPlayAdventureTeamSuccess = false;
        }

#endregion
        private UnityEngine.Coroutine waitToPlayAdventurePassSeasonFuncUnlock = null;
        private bool isPlayAdventurePassSeasonSuccess = false;

#region Play Func Unlock One Anim

        void _PlaySelectNewFuncUnlockAnim(ProtoTable.FunctionUnLock.eFuncType funcType, System.Action onPlayStartHandler = null, System.Action onPlayEndHandler = null)
        {
            if (PlayerBaseData.GetInstance().NewUnlockFuncList == null)
            {
                return;
            }

            if (!PlayerBaseData.GetInstance().NewUnlockFuncList.Contains((int)funcType))
            {
                return;
            }

            FunctionUnLock FunctionUnLockData = TableManager.GetInstance().GetTableItem<FunctionUnLock>((int)funcType);

            if (FunctionUnLockData.bPlayAnim == 0)
            {
                return;
            }

            if (FunctionUnLockData.TargetBtnPos == "" || FunctionUnLockData.TargetBtnPos == "-")
            {
                return;
            }

            GameObject buttonObj = Utility.FindGameObject(frame, FunctionUnLockData.TargetBtnPos);
            if (buttonObj == null)
            {
                return;
            }

            //if (FunctionUnLockData.bShowBtn == 0)
            //{
            //    Image[] images = buttonObj.GetComponentsInChildren<Image>(true);
            //    for (int i = 0; i < images.Length; i++)
            //    {
            //        Color clo = images[i].color;
            //        clo.a = 0;
            //        images[i].color = clo;
            //    }

            //    Text[] texts = buttonObj.GetComponentsInChildren<Text>();
            //    for (int i = 0; i < texts.Length; i++)
            //    {
            //        texts[i].enabled = false;
            //    }
            //}

            //buttonObj.CustomActive(true);
            // UIEventSystem.GetInstance().SendUIEvent(EUIEventID.NewFuncFrameOpen);

            UnlockData data = new UnlockData();
            data.FuncUnlockID = (int)funcType;
            data.pos = buttonObj.transform.position;

            FunctionUnlockFrame UnlockFrame = ClientSystemManager.GetInstance().OpenFrame<FunctionUnlockFrame>(FrameLayer.Top, data) as FunctionUnlockFrame;

            if (UnlockFrame != null && onPlayStartHandler != null)
            {
				 onPlayStartHandler();
            }

            UnlockFrame.ResPlayEnd = () =>
            {
                if (ClientSystemManager.GetInstance().IsFrameOpen<FunctionUnlockFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<FunctionUnlockFrame>();
                }

                if (onPlayEndHandler != null)
                {
                    onPlayEndHandler();
                }
            };

            PlayerBaseData.GetInstance().NewUnlockFuncList.Remove((int)funcType);
        }

#endregion

#region Log send
//         [UIEventHandle("topleft2/SendLog")]
//         void _OnSendLogClicked()
//         {
//             int kkk = 0;
//         }
#endregion
		
#region IOSFuncSwitch

        bool CheckIsFunctionClose(FunctionUnLock funcData)
        {
            if (funcData == null)
                return false;
            int sevenAward = 32;
            if (funcData.ID == sevenAward)
            {
                if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.SEVEN_AWARDS))
                {
                    return true;
                }
            }
            if (funcData.FuncType == FunctionUnLock.eFuncType.ActivityJar)
            {
                if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.LIMITTIME_JAR))
                {
                    return true;
                }
            }
            else if (funcData.FuncType == FunctionUnLock.eFuncType.Ranklist)
            {
                if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.SHARE_TEXT_CHANGE))
                {
                    return true;
                }
            }
            else if (funcData.FuncType == FunctionUnLock.eFuncType.ActivityLimitTime)
            {
                if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.SHARE_TEXT_CHANGE))
                {
                    return true;
                }
            }
            else if (funcData.FuncType == FunctionUnLock.eFuncType.FestivalActivity)
            {
                if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.LIMITTIME_ACTIVITY))
                {
                    return true;
                }
            }
            return false;
        }
		
        void TryHideRankListAtFirst()
        {
            //add by mjx for appstore 
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.SHARE_TEXT_CHANGE))
            {
                if (mRankList)
                {
                    mRankList.gameObject.CustomActive(false);
                }
            }
        }
       
        //隐藏气泡是否成功
        bool CheckIsHidePrivateCustomShow(GameObject privateCustomBubbles)
        {
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.LIMITTIME_GIFT))
            {
                if (privateCustomBubbles)
                {
                    privateCustomBubbles.CustomActive(false);
                    return true;
                }
            }
            return false;
        }
#endregion
    }
}
