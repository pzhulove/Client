using System;
using System.Collections.Generic;
///////删除linq
using System.Text;

namespace GameClient
{
    [System.Serializable]
    public enum EUIEventID
    {
        Invalid = -1,

        // DungeonRward(这里前面不要再加新的枚举，by wangbo)
        DungeonOnFight,
        DungeonRewardFinish,
        DungeonAreaChanged,

        OnConfirmToFashionMerge = 101,//FIXED ENUM VALUE DO NOT EDIT THIS VALUE(这里前面不要再加新的枚举，by shenshaojun)

        // PlayerMain
        JobIDChanged, // 转职（特指完成转职任务）
        JobIDReset,// 非专职（特指其他玩法引起的职业选择导致的职业id的改变,比如吃鸡） 
        PlayChangeJobEffect, //播放转职/切换职业特效    
        GoldChanged,
        BindGoldChanged,
        TicketChanged,
        BindTicketChanged,
        GoldJarScoreChanged,
        MagicJarScoreChanged,
        AliveCoinChanged,
        FatigueChanged,
        LevelChanged,
        SpChanged,
        NameChanged,
        ExpChanged,
        WarriorSoulChanged,
        FollowPetChanged,
        TraditionPkWaitingRoom,
        AwakeChanged,
        MoveSpeedChanged,
        RedPointChanged,
        PlayerDataBaseUpdated,
        PlayerDataGuildUpdated,
        MonthCardChanged,
        PlayerVipLvChanged,
        CounterChanged,
        NewMailNotify,
        PlayerDataSeasonUpdated,
        TipsAniStart,
        TipsAniEnd,
        PetChanged,
        ChangeJobSelectDialog,
        AvatarChanged,

        OnIsShowFashionWeapon, //是否选中展示时装武器

        /// <summary>
        /// 初始化宠物列表
        /// </summary>
        PetInfoInited,

        // NewbieGuide
        InitNewbieGuideBootData,
        EndNewbieGuideCover,
        CheckAllNewbieGuide,
        CurGuideStart,
        CurGuideFinish,
        HpChanged,
        AddNewMission,
        FinishTalkDialog,
        TraceBegin,
        TraceEnd,

        //FinancialPlan
        FinancialPlanBuyRes,
        FinancialPlanReceivedRes,
        FinancialPlanLevelSync,
        FinancialPlanRedPointTips,
        FinancialPlanButtonUpdateByLogin,
        FinancialPlanButtonUpdateByLevel,
        FinancialPlanButtonUpdateBySceneChanged,

        //攻城怪物
        SyncAttackCityMonsterAdd,       //同步增加怪物
        SyncAttackCityMonsterDel,    //同步删除怪物
        SyncAttackCityMonsterList,      //同步怪物列表
        SyncAttackCityMonsterUpdate,

        // Skill
        SkillListChanged,
        SkillBarChanged,
        SkillSolutionChanged,
        UpdateCurUseSkillBar,
        SkillLvUpNoticeUpdate,
        SkillPlanPageUnlock,
        BattlePreviewCreateFinish,

        // skillnew
        OnSelectSkillPage,          //选择技能页
        CloseSkillConfigFrame,      //关闭技能也
        SkillLearnedLevelChanged,
        OnChooseSkill,

        // Buff
        BuffListChanged,
        BuffRemoved,
        BuffAdded,

        /// <summary>
        /// 战斗内buff Cancel
        /// Removed一定Cancel但Cancel不一定Removed
        /// </summary>
        BattleBuffCancel,
        /// <summary>
        /// 战斗内,删除
        /// </summary>
        BattleBuffRemoved,
       
        /// <summary>
        /// 战斗内,添加
        /// </summary>
        BattleBuffAdded,
        /// <summary>
        /// 战斗输出更新
        /// </summary>
        BattleDataUpdate,

        // Item
        ItemUseSuccess,
        ItemSellSuccess,
        ItemStoreSuccess,
        ItemTakeSuccess,
        ItemStrengthenSuccess,
        ItemStrengthenFail,
        ItemSingleUpdate,
        ItemQualityChanged,
        ItemNewStateChanged,
        ItemNotifyGet,
        ItemNotifyRemoved,
        ItemCountChanged,
        ItemsAttrChanged,
        TimeLessItemsChanged,
        ItemRenewalSuccess,
        SwitchEquipSuccess,
        ContinueProcessStart,
        ContinueProcessFinish,
        ContinueProcessReset,
        ItemPropertyChanged,

        // Package
        PackageTypeChanged,
        PackageFull,
        PackageNotFull,
        OnlyUpdateItemList,
        PackageSwitch2OneGroup,
        // Storage
        StorageLevelUp,

        // Shop
        ShopBuyGoodsSuccess,
        ShopRefreshSuccess,
        ShopMainFrameClose,
        ShopGuildFrameClose,
        ShopMallFrameClose,
        ShopRefreshTimesChanged,
        OnCloseShopFrame,
        GoodsRecommend,
        ChangeNum,
        ChangeNum2,
        VirtualInputNumberChange,
        VirtualInputEnsure,
        updateFashionTab,
        AccountShopUpdate,
        AccountShopItemUpdata,
        SpeicialItemUpdate,
        AccountSpecialItemUpdate,
        PlayerMallPointUpdate,      //商城积分
        AccountShopReqFailed,

        // Auction
        AuctionSearchLimitChanged,
        AuctionFreezeRemind,

        ActorShowTabChanged,

        //ShopNew
        ShopNewBuyGoodsSuccess,
        ShopNewRequestChildrenShopDataSucceed,          //请求子商店的刷新数据的返回

        // BattleMain
        ClientBattleMainFadeInFadeOut,
        DisplayMissionTips,

        // Acvitity
        ActivityUpdate,
        ActivityAdd,
        ActivityDelete,
        ActivityNoticeUpdate,
        ActivitySpecialRedPointNotify,

        /// 限时活动
        ActivityTabsInfoUpdate,//活动页签数据更新
        ActivityLimitTimeTaskUpdate,       //限时活动任务状态改变
        ActivityLimitTimeUpdate,           //限时活动状态改变
        ActivityLimitTimeToggleChange,      //限时活动切换到特定活动id的页签去。

        ActivityLimitTimeTaskDataUpdate,       //限时活动任务数据更新
        ActivityLimitTimeDataUpdate,           //限时活动数据更新
        ActivtiyLimitTimeAccounterNumUpdate,  //限时活动账号使用次数更新

        // 七日活动
        SevenDaysActivityUpdate,

		// 赌马活动
	    HorseGamblingStateUpdate,//活动状态改变
		HorseGamblingGameStateUpdate,//活动每局状态改变
		HorseGamblingUpdate,//活动主界面数据刷新
		HorseGamblingGameHistoryUpdate,//获取比赛历史
	    HorseGamblingRankListUpdate,//获取射手排行榜
	    HorseGamblingOddsUpdate,//更新赔率信息
	    HorseGamblingShooterHistoryUpdate,//更新射手战绩信息
	    HorseGamblingShooterStakeUpdate,//更新射手押注数据
	    HorseGamblingShooterStakeResp,//押注返回
	    HorseGamblingBuyBulletResponse,//购买子弹返回
		/// <summary>
		/// 活动副本
		/// </summary>
		ActivityDungeonUpdate,
        /// <summary>
        /// 堕落深渊活动状态更新
        /// </summary>
        ActivityDungeonStateUpdate,
        WelfActivityRedPoint,//福利界面疲劳找回，奖励找回红点特殊处理

        /// <summary>
        /// boss活动
        /// </summary>
        BossExchangeUpdate,
        BossKillActivityExist,
        UpdateBossActivityState,
        UpdateBossActivityData,

        /// <summary>
        /// 活动副本 死亡之塔 扫荡完成更新
        /// </summary>
        ActivityDungeonDeadTowerWipeEnd,

        /// <summary>
        /// 夺宝活动
        /// </summary>
        TreasureLotteryBuyResp,
        TreasureLotterySyncActivity,
        TreasureLotterySyncMyLottery,
        TreasureLotterySyncHistory,
        TreasureLotterySyncDraw,
        TreasureLotteryStatusChange,
        TreasureLotteryShowHistory,     //跳转到当天榜单页面
        TreasureLotteryShowActivity,    //跳转到夺宝中页面
        // FuncUnlock
        NewFuncUnlock,
        NextFuncOpen,
        NewAreaUnlock,
        UpdateUnlockFunc,
        NewFuncFrameOpen,
        NewFuncFrameClose,
        NpcRelationMissionChanged,
        //帐号 功能 解锁
        NewAccountFuncUnlock,

        // menu
        MenuIdChanged,
        TaskNpcIdChanged,
        OnAddNewTask,
        OnCompleteTask,
        OnGiveUpTask,
        MissionIDChanged,
        TaskTraceDirection,
        HideGuideArrow,
        ShowGuideArrow,

        MissionAccepted,
        MissionUpdated,
        MissionSync,        //任务同步

        FinishedTaskListSync, // 已经完成的任务id同步

        // taskDialog
        Dlg2TaskId,
        DlgCallBack,

        // Scene
        InitializeTownSystem,
        SceneChangedFinish,
        BattleInitFinished,
        SceneJumpFinished,
        SystemLoadingCompelete,
        SwitchToMainScene,
        TownSceneInited,

        //SceneChange
        SceneChangedLoadingFinish,          //场景切换时，loading结束

        // System
        SystemChanged,

        // OpenFrame
        FrameOpen,
        FadeInOver,
        SwitchToMission,
        GuankaFrameOpen,
        DungeonRewardFrameOpen,
        TaskDialogFrameOpen,
        ChangeJobSelectFrameOpen,
        ChangeJobFinishFrameOpen,
        VipFrameOpen,
        VipPrivilegeFrameOpen,
        FirstPayFrameOpen,
        SecondPayFrameOpen,
        BlueCircleChange,

        //DialogSecondVew
        OpenDialogSecondView,

        //Notify Open Frame
        NotifyOpenWelfareFrame,

        // CloseFrame
        FrameClose,
        MiddleFrameClose,
        FadeOutOver,
        MailFrameClose,
        DungeonRewardFrameClose,
        MissionRewardFrameClose,
        UplevelFrameClose,
        OutPkWaitingScene,
        WelfareFrameClose,
        SkillPlanClose,
        GuildMainFrameClose,
        VipFrameClose,
        VipPrivilegeFrameClose,
        FirstPayFrameClose,
        SecondPayFrameClose,

        // pk
        PkMatchScoreChanged,
        PkCoinChanged,
        PkCurWinSteak,
        PkMatchStartSuccess,
        PkMatchCancelSuccess,
        PkMatchFailed,
        PkMatchCancelFailed,
        PkEndData,
        pkGuideStart,
        pkGuideEnd,
        PKMatched,

        // Pk3v3Cross
        OnUpdatePk3v3CrossRankScoreList,
        Pk3v3CrossUpdateMyTeamFrame,
        Pk3v3CrossPkAwardInfoUpdate,
        PK3V3CrossButton,
        SecurityLockApplyStateButton,

        // pk2v2cross
        PK2V2CrossButton,
        PK2V2CrossStatusUpdate,
        OnUpdatePk2v2CrossRankScoreList,
        Pk2v2CrossPkAwardInfoUpdate,
        Pk2v2CrossBeginMatch,
        Pk2v2CrossCancelMatch,
        Pk2v2CrossBeginMatchRes,
        Pk2v2CrossCancelMatchRes,

        // allframeevent
        SceneLoadFinish,

        /// <summary>
        /// 解锁新的关卡难度
        /// </summary>
        DungeonUnlockDiff,
        DungeonPlayerUseSkill,

        /// <summary>
        /// 下一个地下城
        /// </summary>
        ChapterNextDungeon,
        SelectEnterDungeon,

        /// <summary>
        /// 角色加载进度改版通知
        /// </summary>
        DungeonPlayerLoadProgressChanged,

        /// <summary>
        /// 地下城评价改变
        /// </summary>
        DungeonScoreChanged,

        /// <summary>
        /// 地下城buff药配置
        /// </summary>
        DungeonPotionSetChanged,

        DungeonQuickBuyPotionSuccess,
        DungeonQuickBuyPotionFail,

        /// <summary>
        /// 选择服务器
        /// </summary>
        ServerListSelectChanged,

        /// <summary>
        /// 用户取消等待
        /// </summary>
        ServerQueueCancel,

        /// <summary>
        /// 登录失败，连接失败，返回错误
        /// </summary>
        ServerLoginFail,

        /// <summary>
        /// 登录失败，连接失败，返回错误
        /// </summary>
        ServerLoginFailWithServerConnect,

        /// <summary>
        /// 开始登录流程
        /// </summary>
        ServerLoginStart,

        /// <summary>
        /// 等待队列
        /// </summary>
        ServerLoginQueueWait,

        /// <summary>
        /// 成功登录 
        /// </summary>
        ServerLoginSuccess,

        // 断开连接
        DisConnect,

        /// <summary>
        /// 关卡选择界面关卡详情关闭
        /// </summary>
        ChapterNormalHalfFrameOpen,
        ChapterNormalHalfFrameClose,
        
        //
        SetDefaultSelectedID,
        RoleInfoUpdate,

        //
        FunctionFrameUpdate,

        //
        PurchaseCommanUpdate,

        // Team
        TeamCreateSuccess,
        TeamAddMemberSuccess,
        TeamJoinSuccess,
        TeamRemoveMemberSuccess,
        TeamChangeLeaderSuccess,
        TeamPosStateChanged,
        TeamMemberStateChanged,
        TeamListRequestSuccess,
        TeamListRequestSuccessForTeamMainUI,
        TeamChapterIDSelect,
        TeamDungeonIDSelect,
        TeamPasswardChanged,
        TeamInfoUpdateSuccess,
        TeamNotifyChat,
        TeamNotifyChatMsg,
        TeamMatchStartSuccess,
        TeamMatchCancelSuccess,
        TeamNewInviteNoticeUpdate,
        TeamListUpdateByHard,
        TeamTimeChanged,
        TeamGlobalJoined,   //加入队伍了  //OnVoiceChatTeamJoin
        TeamGlobalLeaved,    //离开队伍    //OnVoiceChatTeamLeave

        OnDecomposeChanged,
        OnStrengthChanged,
        DecomposeFinished,
        OnSpecailStrenghthenStart,
        OnSpecailStrenghthenCanceled,
        OnSpecailStrenghthenFailed,
        OnStrengthenError,
        OnUsePerfectWashRoll,

        // chat
        OnRecievChat,

        //relation
        OnRecievRecommendFriend,
        OnRefreshFriendList,
        OnRefreshInviteList,
        
        OnShowFriendSecMenu,
        OnShowFriendChat,
        OnRecvPrivateChat,
        OnRecvQueryPlayer,
        OnQueryIntervalChanged,
        OnQueryEnd,
        OnPrivateChat,
        OnUpdatePrivate,
        OnDelPrivate,
        OnPrivateRdChanged,
        OnRelationChanged,
        FriendRequestNoticeUpdate,
        OnPlayerOnLineStatusChanged,
        OnUpdatePayText,
        OnDonateAllSended,
        OnSendFriendGift,//好友单独送礼

        //Equipment
        OnEquiptedNew,
        // battle
        BattlePlayerDead,
        BattlePlayerAlive,
        BattlePlayerBack,
        BattlePlayerLeave,
        BattlePlayerInfoChange,
        BattleFrameSyncEnd,

        // common
        OnCloseMenu,

        // Menu
        UpdateActorShowMenu,

        // Enchantments
        OnMergeSuccess,
        OnAddMagicSuccess,

        OnOneKeyMergeSuccess,   //附魔卡一键合成

        // Bxy
        OnBxyMergeSuccess,
        // Sinan
        OnSinanSuccess,
        // EquipJicheng
        OnEquipJichengSuccess,

        //Sarah
        OnAddSarahSuccess,

        // map
        MapMoveToNPC,
        MapMoveToScene,
		//levelGift
		LeftSlip,
		RightSlip,
        // player
        PlayerMoveStateChanged,

        EnterDungeon,

        // DeadTower
        OnDeadTowerWipeoutTimeChange,

        OnCountValueChange,
        SetChatTab,

        //BeginContineStrengthen;
        BeginContineStrengthen,
        EndContineStrengthen,
        IntterruptContineStrengthen,
        StrengthenContinue,
        StrengthenDelay,
        StrengthenCanceled,
        ClosePotionSetFrame,

        // Guild
        GuildListUpdated,
        GuildRequestJoinSuccess,
        GuildRequestJoinAllSuccess,
        GuildCreateSuccess,
        GuildMembersUpdated,
        GuildRequestersUpdated,
        GuildNewRequester,
        GuildKickMemberSuccess,
        GuildLeaveGuildSuccess,
        GuildProcessRequesterSuccess,
        GuildChangeDutySuccess,
        GuildSendMailSuccess,
        GuildChangeDeclarationSuccess,
        GuildChangeNoticeSuccess,
        GuildChangeNameSuccess,
        GuildRequestDismissSuccess,
        GuildRequestCancelDismissSuccess,
        GuildCloseMainFrame,
        GuildOpenMemberFrame,
        GuildOpenBuildingFrame,
        GuildOpenShopFrame,
        GuildOpenShopRefreshConsumeItem,
        GuildOpenStorageFrame,
        GuildOpenManorFrame,
        GuildOpenCrossManorFrame,
        GuildOpenRedPacketFrame,
        GuildUpgradeBuildingSuccess,
        GuildRequestDonateLogSuccess,
        GuildDonateSuccess,
        GuildExchangeSuccess,
        GuildSkillLevelupSuccess,
        GuildHasDismissed,
        GuildLeaderUpdated,
        GuildWorshipSuccess,
        GuildAddTableMember,
        GuildRemoveTableMember,
        GuildJoinTableSuccess,
        GuildTableFinished,
        GuildInspireSuccess,
        GuildBaseInfoUpdated,
        GuildSignupSuccess,
        GuildManorInfoUpdated,
        GuildManorOwnerUpdated,
        GuildBattleRecordSync,
        GuildBattleRewardGetSuccess,
        GuildBattleStateChanged,
        GuildBattleRanklistChanged,
        GuildSelfRankChanged,
        GuildInviteNoticeUpdate,
        GuildAttackCityInfoUpdate,
        GuildInspireInfoUpdate,
        GuildLotteryResultRes,
        GuildTownStatueUpdate,
        GuildJoinLvUpdate,
        GuildReceiveMergerd,//是否有公会兼并自己
        RefuseAllReceive,//拒绝全部的申请
        GuildReceiveMergedList,// 返回本公会收到的兼并申请列表
        AgreeMerger,           //同意兼并       
        RequestGuildMergerSucess,//申请兼并成功
        CanMergerdGuildMemberUpdate,//刷新可兼并的公会成员的数量
        AcceptOtherGuildAgree,//接受到对方同意你的兼并请求
        GuildGlobalKickedOut,       //被踢出公会     //OnVoiceChatGuildLeave
        GuildGlobalJoined,          //加入公会了     //OnVoiceChatGuildJoin

        // 帮会副本
        GuildDungeonDamageRank,
        GuildDungeonUpdateActivityData,
        GuildDungeonUpdateDungeonMapInfo,
        GuildOpenGuildDungeonTreasureChests,
        GuildDungeonUpdateDungeonBufInfo,
        GuildDungeonSyncActivityState,
        GuildGuardStatueUpdate,
        GuildDungeonShowFireworks,
        GuildDungeonSetBossDiff,
        GuildBossHPRefresh,

        BattleDoubleBossTips,

        DungeonRebornFail,
        DungeonRebornSuccess,

        //RedPacket
        RedPacketSendSuccess,
        RedPacketOpenSuccess,
        RedPacketCheckSuccess,
        RedPacketGet,
        RedPacketDelete,

        //BudoMsg
        BudoRewardReturn,
        OnFashionMergeNotify,
        OnFashionMergeRedCounChanged,
        OnFashionMergeSwich,

		//pay
		OnPayResultNotify,
        OnPayRewardReceived,

        //DailyCharge
        DailyChargeResultNotify,
        DailyChargeRedPointChanged,
        DailyChargeCounterChanged,

        MagicJarUseSuccess,
        MagicJarUseFail,
        UpdatePayData,

        // Ranklist
        RanklistUpdated,

        // EquipForge
        EquipForgeSuccess,
        EquipForgeFail,

        //FrameCallBack
        FrameCloseCallBack,

        // WaitMessage
        WaitMessageReopen,
        RoleRecoveryUpdate,
        RoleDeleteOk,

        //time
        ServerTimeChanged,

        //RoleId
        RoleIdChanged,
        RoleChatDirtyChanged,
        PopChatMsg,
        OnChatFrameStatusChanged,

        //PKRank
        PKSelfLevelUpdated,
        PKRankChanged,
        SeasonStarted,
        PKMyRecordUpdated,
        PKPeakRecordUpdated,

        onSDKLoginSuccess,
        onSDKLogoutSuccess,

        JarOpenRecordUpdate,

        //world_chat_horn
        WordChatHorn,

        //relation_frame_new
        RelationTabChanged,
        RelationAddRecommendFriendMsgSended,
        MainRelationTabChanged,
        FriendComMenuRemoveList,//好友界面菜单移除列表事件枚举
        //jar
        JarFreeTimeChanged,

        // pet
        PetSlotChanged,
        PetItemsInfoUpdate,
        PetFeedSuccess,
        PetPropertyReselect,
        EatPetSuccess,
        PlayActiveFeedPetAction,
        FollowPetSatietyChanged,
        UpdatePetFoodNum,

        /// <summary>
        /// 点击宠物喂养
        /// </summary>
        PetGoldFeedClick,

        /// <summary>
        /// 点击宠物页签
        /// </summary>
        PetTabClick,

        OnEnchantCardSelected,
        OnSarakCardSelected,

        // limit time gift pack in mall
        HasLimitTimeGiftToBuy,
        OnLimitTimeGiftViewRefresh,
        OnLimitTimeGiftDataRefresh,
        OnLimitTimeGiftChecked,
        GetGiftData,// 获得礼包数据

		OnMallFrameClosed,
		
        //MonthCardUpdate
        OnMonthCardUpdate,
		
		//SDK Bind Mobile Phone
        SDKBindPhoneFinished,
		 //LimitTimeActivity Show or Hide
        ShowLimitTimeActivityBtn,
        InitLimitTimeActivityView,
		RefreshLimitTimeActivityIcon,
        AutoEnterToRoleSelect,
        ControlFashionFrame,
        RefreshActivityLimitTimeBtn,

        //GUILDSTORAGE
        OnGuildHouseItemAdd,
        OnGuildHouseItemUpdate,
        OnGuildHouseItemRemoved,
        OnGuildHouseItemStoreRet,
        OnGuildHouseItemDeleteRet,
        OnOpenGuildHouseMain,
        OnGuildSotrageOperationRecordsChanged,

        //OnlineService
        MakeShowOnlineService,
        OnRecOnlineServiceSign,
        OnRecOnlineServiceNewNote,
        OnRecVipOnlineService,
        OnWebViewLoadStart,
        OnWebVieewLoadFinish,
        OnWebViewLoadError,

        //TAPSystem
        OnShowAskTeacherMenu,
        OnAskTeacherMsgSended,
        OnQueryTeacherChanged,
        OnSearchedTeacherListChanged,
        OnSearchedPupilListChanged,
        OnApplyPupilListChanged,
        OnApplyTeacherListChanged,//new
        OnShowPupilRealMenu,
        OnShowPupilApplyMenu,
        OnAnnouncementChanged,
        OnGetPupilSettingChanged,
        OnWanApplyedPupilChanged,
        OnNewPupilApplyRecieved,
        OnNewTeacherApplyRecieved,//new
        OnShowTeacherRealMenu,
        OnSelectedAOPPlyerChanged,
        OnPayWordsChanged,
        OnPayListChanged,
        OnAddOnPaySettingChanged,
        OnPayRequestTimesChanged,
        OnDonateSelecteItemChanged,
        OnRefreshClassmateDic,

        OnMyPupilMissionUpdate,
        OnTAPLearningUpdate,
        OnPupilDataUpdate,
        OnTeacherDataUpdate,
        OnUpdateTAPPublishFrame,
        OnReSelectTAPToggle,
        OnTAPStartTalk,
        OnTAPPublishMissionSuccess,
        OnTAPGraduationScoreChange,
        OnTAPTeacherValueChange,
        OnTAPApplyToggleRedPointUpdate,
        OnTAPOpenSearchFrame,
        OnTAPGraduationSuccess,
        OnTAPReportTeacherSuccess,
        OnTapPupilReportRedPoint,
        OnTapTeacherSubmitRedPoint,

        OnInviteTextChanged,
        /// <summary>
        /// 上下宠物刷新背包面板属性
        /// </summary>
        OnRefreshPackageProperty,
        OnMoneyRewardsStatusChanged,
        OnMoneyRewardsResultChanged,
        OnMoneyRewardsPoolsMoneyChanged,
        OnMoneyRewardsSelfResultChanged,
        OnMoneyRewardsPlayerCountChanged,
        OnMoneyRewardsAwardsChanged,
        OnMoneyRewardsRankListChanged,
        OnMoneyRewardsRecordsChanged,
        OnMoneyRewardsTrySecondMatch,
        OnMoneyRewardsBattleInfoChanged,
        OnMoneyRewardsAwardListChanged,
        OnMoneyRewardsRcdStatusChanged,
        OnMoneyRewardsAddPartyTimesChanged,

        OnSelectedMergeTitleChanged,
        /// <summary>
        /// 播放解锁功能动画
        /// </summary>
        OnPlayerFunctionUnlockAnimation,

        UpdateFreeVersion,

        /// <summary>
        /// 3v3或得到结算消息
        /// </summary>
        PK3V3GetRaceEndResult,

        PK2V2CrossScoreGetRaceEndResult, // 2v2跨服乱斗积分赛得到结算消息

        /// <summary>
        /// 3v3一轮结束的时候
        /// </summary>
        PK3V3GetRoundEndResult,

        /// <summary>
        /// 开始准备倒计时
        /// </summary>
        PK3V3StartRedyFightCount,

        /// <summary>
        /// 开始进行第一轮投票出战
        /// </summary>
        PK3V3StartVoteForFight,

        /// <summary>
        /// 匹配玩家投票竞选出战
        /// </summary>
        PK3V3VoteForFightStatusChanged,

        /// <summary>
        /// 匹配玩家投票竞选结束，等待动画
        /// </summary>
        PK3V3FinishVoteForFight,

        //pk3v3
        Pk3v3RoomInfoUpdate,
        Pk3v3RoomSimpleInfoUpdate,
        Pk3v3RoomSlotInfoUpdate,
        Pk3v3InviteRoomListUpdate,
        Pk3v3KickOut,
        Pk3v3BeginMatch,
        Pk3v3CancelMatch,
        Pk3v3BeginMatchRes,
        Pk3v3CancelMatchRes,
        Pk3v3RefuseBeginMatch,
        Pk3v3PlayerLeave,
        Set3v3RoomName,
        Set3v3RoomPassword,
        Pk3v3VoteEnterBattle,
        Pk3v3RefreshRoomList,
        Pk3v3ChangePosition,

        OnNormalFashionModeChanged,
        OnNormalFashionSelected,
        OnFashionSpecialMerged,
        OnFashionSpecialFly,
        OnFashionAutoEquip,
        OnFashionFastItemBuyFinished,       //购买FastItem
        OnFashionTicketBuyFinished,
        OnFashionNormalItemSelected,        //选择NormalItem

        //AuctionBuyFrame
        RefreshAuctionBuyFrameInfo,

        
		//VOICE SDK STATUS START

        VoiceMicNotAuth,                     //麦权限
        VoiceChatReset,                      //录音和发送状态重置
        VoiceChatRecordStart,
        VoiceChatRecordEnd,
        VoiceChatRecordCancel,
        VoiceChatRecordFailed,
        VoiceChatRecordVolumeChanged,
        VoiceChatSendStart,
        VoiceChatSendEnd,
        VoiceChatSendCancel,
        VoiceChatSendFailed,

        VoiceChatReadySendCancel,                //准备取消发送
        VoiceChatShowSendCancel,                 //提示可以取消发送

        VoiceTalkMicSwitch,                      //实时语音按钮状态变化
        VoiceTalkPlayerSwitch,
        VoiceTalkLimitAllNotSpeak,                //全体禁言
        VoiceTalkMicClosedByOther,                //被禁言  
        VoiceTalkChannelChanged,                  //实时说话频道切换
        VoiceTalkOtherSpeakInChannel,             //其他人在频道里讲话时 展示动画
        VoiceTalkJoinChannelAndMicChanged,                     //加入实时语音频道切换麦状态成功
        VoiceTalkLeaveAllChannel,

		//VOICE SDK STATUS END

        OnDayChargeChanged,
        OnAchievementGroupSubTabChanged,
        OnAchievementGroupSubTabChangedRepeated,
        OnAchievementSecondMenuTabChanged,
        OnAchievementMaskPropertyChanged,
        OnShareAchievementItem,
        OnAchievementScoreChanged,
        OnAchievementComplete,
        OnAchievementOver,
        RefreshLimitTimeState,
        UpdatePackageTabRedPoint,
        UpdatePackageGrids,

        // 装备回收
        EquipRecoveryPriceReqSuccess,
        EquipSubmitSuccess,
        EquipRedeemSuccess,
        EquipReturnFail,
        EquipUpgradeSuccess,
        EquipRecoveryUpdateTime,
        EquipJarListUpdate,
        EquipRecivertDeleteItem,
        EquipSubmitScore,
        EquipUpgradeFail,
        EquipDonatePackageUpdate,
        //主界面动画
        RightLowerBubblePlayAnimation,
        RightLowerBubbleStopAnimation,
        Count,
        LockStatuNewMessageNumber,//世界频道锁屏状态新消息事件
        SetNoteNameSuccess,//设置备注名称成功

        onLiveShowPursueModeChange, //直播模式下追帧的变换
        OnSyncWorldMallQueryItems,            //获得同步的商品
        OnSyncMallBatchBuySucceed,            //时装商城，批量购买成功
        OnSyncWorldMallBuySucceed,            //道具商城和限时商城，单个购买成功
        OnReceiveSceneMallFashionLimitedSuitStatusRes,      //时装商城的限时道具
        GetCommendShopItemSucc, //推荐页面获取商店成功
        GetCommendAccountShopItemSucc, //推荐页面获取账号商店成功
        GetCommendMallItemSucc, //推荐页面获取商城成功

		//RandomTreasure
        OnChangeTreasureDigMap,//切换地图
        OnChangeTreasureDigSelectMap,//大地图中点击切换地图
        OnOpenTreasureDigMap,//服务器同步后，打开地图
        OnTreasureDigMapOpen,//地图开启后
        OnTreasureDigMapClose,//地图关闭后
        OnWatchTreasureDigSite,//宝箱查看
        OnWatchRefreshDigSite,//宝箱查看刷新
        OnOpenTreasureDigSite,
        OnTreasureDigSiteChanged,//地图挖宝点更新
        OnTreasureMapDigReset,//地图点重置
        OnTreasureMapPlayerNumSync,
        OnTreasureAtlasInfoSync,//大地图数据同步
        OnTreasureRaffleStart,//挖宝抽奖开始
        OnTreasureRaffleEnd,
        OnTreasureRecordInfoChanged,//挖宝记录更新
        OnTreasureRecordInfoSync,//挖宝记录同步
        OnTreasureAtlasOpen,//大地图（包含所有地图界面）打开
        OnTreasureAtlasClose,
		OnTreasureItemBuyRes,//宝箱道具购买返回
        OnTreasureFuncSwitch,//宝箱功能开关通知
		OnTresureItemCountChanged, //宝箱道具数量改变
        OnSyncWorldBuyPetSucceed, // 宠物推送界面购买宠物成功

        // 安全锁
        RefreshSecurityLockDataUI, // 刷新安全锁状态
        RefreshVerifyPwdErrorCount, // 刷新密码验证错误次数
        VanityBonusAnimationEnd,//虚空加成动画播放完毕
        VanityBonusBuffPos,//虚空加成BUff位置

        //药品配置
        BuffDrugSettingSubmit,//药品配置完成回调

        //宝珠摘除
        OnSelectPickBeadExpendItem,
        BeadPickSuccess,//宝珠摘除成功
		
		//StrengthenTicketMerge
        OnStrengthenTicketMergeStateUpdate,           //强化券合成活动状态改变
        OnStrengthenTicketMergeSelectType,             //强化券合成选择类型
        OnStrengthenTicketMergeSelectTicket,          //强化券合成选择的券
        OnStrengthenTicketFuseAddTicket,              //强化券融合添加选择的券
        OnStrengthenTicketFuseRemoveTicket,           //强化券融合移除选择的券
        OnStrengthenTicketMergeSuccess,               //强化券合成成功
        OnStrengthenTicketMergeFailed,                //强化券合成失败
        OnStrengthenTicketFuseSuccess,                //强化券融合成功
        OnStrengthenTicketFuseFailed,                 //强化券融合失败
        OnStrengthenTicketFuseCalPercent,             //强化券融合预计概率生成
        OnStrengthenTicketMergeSelectReset,           //强化券合成 选中第一个
        OnStrengthenTicketStartMerge,                 //开始请求合成
        OnStrengthenTicketMallBuySuccess,             //强化合成器 商城购买成功
        OnStrengthenTicketFreshView,                  //每次合成成功的时候刷新界面          
        //BeadUpgrade
        OnSelectExpendBeadItem,
        OnBeadUpgradeSuccess,//宝珠升级成功

        //AdventureTeam
        OnAdventureTeamFuncChanged,                  //冒险队功能状态改变
        //AdventureTeam Red Point
        OnAdventureTeamLevelUp,                      //监听冒险队等级变化
		OnAdventureTeamCollectionInfoChanged,		 //角色收藏 有新角色被激活
        OnAdventureTeamExpeditionAwardChanged,       //远征有奖励可领取
        OnAdventureTeamBaseInfoFrameOpen,            //基础界面打开
        OnAdventureTeamRenameSucc,
        OnAdventureTeamRenameCardBuySucc,

        OnAdventureTeamExpeditionMapInfoChanged,     //远征地图信息变化 仅用于刷新远征地图界面
        OnAdventureTeamExpeditionRolesChanged,       //佣兵远征角色 变化 仅用于打开选择佣兵远征界面
        OnAdventureTeamExpeditionMiniMapChanged,     //佣兵团小地图信息变化 用于刷新佣兵团小地图变化
        OnAdventureTeamExpeditionRolesSelected,      //佣兵团选择角色界面关闭后回调
        OnAdventureTeamExpeditionDispatch,           //佣兵团派遣回调
        OnAdventureTeamExpeddtionCancel,             //佣兵团取消派遣回调
        OnAdventureTeamExpeditionGetReward,          //佣兵团领取奖励回调
        OnAdventureTeamExpeditionTimeChanged,        //佣兵团远征时间更改回调
        OnAdventureTeamExpeditionIDChanged,          //佣兵团远征地图id更改回调
        OnAdventureTeamExpeditionTimerFinish,        //倒计时完成回调

        OnAdventureTeamBlessCrystalCountChanged,     //佣兵团Count变化
        OnAdventureTeamBlessCrystalExpChanged,
        OnAdventureTeamBountyCountChanged,
        OnAdventureTeamInheritBlessCountChanged,
        OnAdventureTeamInheritBlessExpChanged,

        OnAdventureTeamBaseInfoRes,                  //佣兵团请求数据回调成功
        OnAdventureTeamBlessCrystalInfoRes,
        OnAdventureTeamInheritBlessInfoRes,
        OnAdventureTeamCollectionInfoRes,

        OnAdventureTeamWeeklyTaskChange,            //佣兵团周长任务变化，刷新ui
        OnAdventureTeamWeeklyTaskStatusChanged,     //周常任务状态变化 刷新红点

        NotifyShowAdventureTeamUnlockAnim,            //佣兵团城镇解锁动画
        NotifyShowAdventurePassSeasonUnlockAnim,      //冒险通行证城镇解锁动画

        OnAdventureTeamExpeditionResultFrameClose,

        OnItemInPackageRemovedMessage,              //背包中的道具删除
        OnItemInPackageAddedMessage,                //背包中的数据增加一个

        //拍卖行
        OnAuctionNewReceiveItemNumResSucceed,           //查询在线数量
        OnAuctionNewReceiveItemDetailDataResSucceed,    //查询在线详情
        OnAuctionNewBuyShelfResSucceed,                 //购买栏位      
        OnAuctionNewReceiveSelfListResSucceed,          //请求自己上架的列表成功
        OnAuctionNewNotifyRefreshToRequestDetailItems,                  //刷新在售的详细信息      
        OnAuctionNewGetTreasureTransactionRecordSucceed,            //获得珍品交易纪律
        OnAuctionNewWorldQueryItemPriceResSucceed,              //查询待上架物品的价格
        OnAuctionNewReceiveNoticeReqSucceed,            //收到关注消息的返回
        OnAuctionNewWorldQueryItemPriceListResSucceed,          //查询上架物品价格的列表
        OnAuctionNewWorldQueryItemTransListResSucceed,          //查询最近交易的列表
        OnAuctionNewWorldQueryMagicCardOnSaleResSucceed,        //查询附魔卡相关信息

        OnAuctionNewSelectMagicCardStrengthenLevel,         //选择某个强化等级

        OnAuctionNewFrameClosed,                        //关闭拍卖行系统

        BossMissionCompleteFrameClose,
        OnUploadFileSucc,//上传录像成功
        OnUploadFileClose,//录像窗口关闭
        
        //神器罐子活动
        ArtifactJarDataUpdate,
        ArtifactDailyRewardUpdate,
        ArtifactDailyRecordUpdate,
        
        /// <summary>
        /// 请求回购列表返回成功
        /// </summary>
        BlackMarketMerchanRetSuccess,
        /// <summary>
        /// 黑市商人回购商品刷新
        /// </summary>
        BlackMarketMerchantItemUpdate,
        /// <summary>
        /// 黑市商人类型创建NPC
        /// </summary>
        SyncBlackMarketMerchantNPCType,

        //挑战关卡
        OnChallengeChapterFrameClose,       //关卡详情界面关闭
        OnChallengeChapterBeginChange,      //关卡开始改变
        OnChallengeChapterFinishChange,     //关卡结束改变

        //挑战模式界面中组队的聊天信息
        OnChallengeTeamChatDataUpdate,      //挑战模式中组队的聊天数据更新
        OnChallengeDungeonRewardUpdate,     //挑战关卡的数据进行更新

        //装备升级成功
        OnEquipUpgradeSuccess,

        OnUpdateAvatar,

        //充值推送
        TopUpPushButoonOpen,
        TopUpPushButtonClose,
        TopUpPushBuySuccess,

        // 吃鸡
        UpdateChijiPrepareScenePlayerNum,
        ChijiHpChanged,
        ChijiMpChanged,
        ChijiBattleStageChanged,
        ChijiPlayerDead,
        ChijiPkReady,
        ChijiPkReadyFinish,
        ChijiPKButtonChange,
        NearItemsChanged,
        PoisonStatChange,
        PoisonNextStage,
        UpdateChijiNpcData,
        ExchangeSuccess,
        StartOpenChijiItem,
        FinishOpenChijiItem,
        CancelOpenChijiItem,
        OpenChijiSkillChooseFrame,
        ChijiBestRank,
        TreasureMapSizeChange,
        PickUpLoserItem,

        // 公会拍卖
        OnGuildDungeonAuctionStateUpdate,       // 公会地下城拍卖状态更新
        OnGuildDungeonWorldAuctionStateUpdate,  // 公会地下城世界拍卖状态更新
        OnGuildDungeonAuctionItemsUpdate,       // 公会地下城拍卖数据刷新  
        OnGuildDungeonAuctionAddNewItem,        // 公会地下城拍卖增加新的拍卖品（世界拍卖或者公会拍卖增加新的拍卖数据）
        //通用键盘的输入
        OnCommonKeyBoardInput,                  //通用键盘的输入事件

        //tips功能按钮点击“更多“按钮事件
        OnMoreAndMoreBtnHandle,

        //邮件
        OnReadMailResSuccess, // 读取邮件成功
        UpdateMailStatus,     //邮件更新状态
        OnMailDeleteSuccess,//邮件删除成功


        OnGuildEmblemLevelUp,                   // 公会徽记升级成功
        OnOpenGuildEmblemLevelPage,             // 打开公会徽记页签
		OnUpdateGuildEmblemLvUpEntry,           // 刷新公会徽记tab按钮(入口)
        OnUpdateGuildEmblemLvUpRedPoint,        // 刷新公会徽记升级红点
        OnOpenGuildBenefitsPage,                // 打开公会福利页签
        OnOpenGuildActivityPage,                // 打开公会活动页签
        //头像框
        HeadPortraitFrameChange, //头像框改变
        UseHeadPortraitFrameSuccess, //使用头像框成功
        HeadPortraitFrameNotify,     //头像框通知
        HeadPortraitItemStateChanged, //头像框状态变化
        //头衔
        TitleDataUpdate,//头衔数据更新
        TitleNameUpdate,//自己头衔名字更新
        TitleGuidUpdate,//自己的头衔的guid更新
        TitleModeUpdate,//头衔界面里面的模型刷新
        TitleBookCloseFrame,//关闭称号铺界面的事件
        //周签到
        OnReceiveGasWeekSignInRecordRes,        //收到周签到的记录
        OnSyncSceneWeekSignInNotify,            //同步周签到的周数和领取的周数
        OnSyncSceneWeekSignBoxNotify,           //同步周签到的开箱子的奖励
        OnActivityWeekSignInRedPointChanged,        //活动周签到的红点提示
        OnNewPlayerWeekSignInRedPointChanged,       //新人周签到的红点提示

        //开罐
        OnBoxOpenFinished,              //罐子打开完成
        // 公会红包
        OnUpdateGuildRedPacketRecord,   // 公会红包记录更新
        OnSelectGuildRedPackType,       // 选择某个公会红包类型
        OnUpdateGuildRedPacketSpecInfo, // 刷新公会红包特殊信息数据
        //附魔卡升级返回
        OnEnchantmentCardUpgradeRetun,
        OnRemoveExpendEnchantmentCard,//清除材料附魔卡
		 //月卡翻牌奖励寄存
        OnMonthCardRewardUpdate,                //奖励刷新时
        OnMonthCardRewardAccquired,             //奖励领取后
        OnMonthCardRewardRedPointReset,         //奖励红点重置
        OnMonthCardRewardCDUpdate,              //奖励倒计时刷新
        OnEquipOrDropSkill,//装备或者弃用技能

        //团本相关
        OnReceiveTeamDuplicationPlayerInformationNotify,        //团本角色数据同步
        OnReceiveTeamDuplicationTeamListRes,            //收到队伍的请求
        OnReceiveTeamDuplicationRefreshTeamListMessage,     //队伍刷新的请求
        OnReceiveTeamDuplicationTeamLeaderRefuseJoinInMessage,      //队长拒绝申请
        OnReceiveTeamDuplicationJoinTeamInCdTimeMessage,            //处在CD时间
        OnReceiveTeamDuplicationDismissMessage,          //团本解散的消息
        OnReceiveTeamDuplicationBuildTeamSuccessMessage,        //团本创建成功(创建团本或者加入团本)
        OnReceiveTeamDuplicationTeamDataMessage,             //自己团本数据改变消息
        OnReceiveTeamDuplicationTeamStatusNotifyMessage,          //自己团本状态改变的消息
        OnReceiveTeamDuplicationTeamDetailDataMessage,       //队伍详情信息
        OnReceiveTeamDuplicationQuitTeamMessage,        //退出团本的消息
        OnReceiveTeamDuplicationCaptainNotifyMessage,   //小队的数据发生改变

        OnReceiveTeamDuplicationOwnerNewTeamInviteMessage,      //存在新的邀请团本（邀请他人入团），展示红点的消息
        OnReceiveTeamDuplicationOwnerNewRequesterMessage,        //存在新申请者（申请入团），展示红点的消息

        OnReceiveTeamDuplicationTeamMateListMessage,        //找队友列表改变
        OnReceiveTeamDuplicationTeamInviteListMessage,      //团本邀请者信息
        OnReceiveTeamDuplicationTeamInviteChoiceMessage,    //团本选择后的消息,主要是删除

        OnReceiveTeamDuplicationRequesterListMessage,       //申请入团者的列表信息
        OnTeamDuplicationForceQuitTeamByDragMessage,              //拖动过程中强制退出队伍
        OnReceiveTeamDuplicationAutoAgreeGoldMessage,           //金主自动同意入团

        OnReceiveTeamDuplicationFightStartVoteAgreeMessage,    //团本投票开战同意通知
        OnReceiveTeamDuplicationStartBattleRefuseMessage,   //团本开战拒绝的通知

        OnReceiveTeamDuplicationFightEndVoteFlagMessage,        //团本战斗结束投票标志的通知
        OnReceiveTeamDuplicationFightEndVoteAgreeMessage,       //团本战斗结束投票同意的通知
        OnReceiveTeamDuplicationFightEndVoteRefuseMessage,      //团本战斗结束投票拒绝的通知
        OnReceiveTeamDuplicationFightEndVoteResultSucceedMessage,    //团本战斗结束投票成功的通知

        OnReceiveTeamDuplicationFightStageNotifyMessage,    //团本阶段数据更新的通知
        OnReceiveTeamDuplicationFightPointNotifyMessage,    //团本据点数据更新的通知
        OnReceiveTeamDuplicationFightGoalNotifyMessage,     //团本小队目标和团本目标数据更新的通知
        OnReceiveTeamDuplicationFightCaptainGoalChangeMessage,      //团本中小队目标ID改变的通知

        OnReceiveTeamDuplicationPreFightPointDataUpdateMessage,     //前置据点更新，更新相应据点的解锁状态
        OnReceiveTeamDuplicationFightPointUnlockRateMessage,        //团本据点解锁的比例通知
        OnReceiveTeamDuplicationFightPointBossDataMessage,         //boss数据(阶段和血量)的通知
        OnReceiveTeamDuplicationFightPointEnergyAccumulationTimeMessage,        //能量蓄积据点时间更新的通知

        OnReceiveTeamDuplicationFightStageRewardEndTimeMessage,     //阶段翻牌的时间

        OnReceiveTeamDuplicationInBattleMessage,            //都同意开战之后处在开战状态的消息

        OnReceiveTeamDuplicationFightStageRewardNotify,    //阶段奖励的信息
        
        OnTeamDuplicationFightStageBeginMessage,            //阶段开始
        OnTeamDuplicationFightStageEndMessage,              //阶段结束提示完成
        OnReceiveTeamDuplicationFightStageEndNotifyMessage, //阶段结束的提示
        OnReceiveTeamDuplicationFightStageEndShowFinishMessage,     //阶段完成奖励展示的通知

        OnReceiveTeamDuplicationStageEndDescriptionCloseMessage,      //阶段完成描述关闭
        OnReceiveTeamDuplicationMiddleStageRewardCloseMessage,              //阶段奖励展示关闭
        OnReceiveTeamDuplicationFinalResultCloseMessage,              //最终结束展示关闭

        OnReceiveTeamDuplicationChatContentMessage,         //聊天消息
        OnReceiveTeamDuplicationServerFuncSwitchChangeMessage,      //团本开关的消息

        OnReceiveTeamDuplicationPlayerExpireTimeMessage,        //团本角色终止的消息
        OnReceiveTeamDuplicationCloseRelationFrame,             //团本场景中关闭相应的界面


        //增幅
        OnBreathEquipActivationSuccess,//气息装备激活成功
        OnBreathEquipClearSuccess,     //气息装备清除成功
        OnRedMarkEquipChangedSuccess,  //红字装备转化成功
        OnEquipmentListNoEquipment,             //装备列表为空
        OnEquipmentListHasEquip,       //有装备

        OnRefreshEquipmentList,

        //每日必做
        OnDailyTodoFuncStateUpdate,
        OnDailyTodoFuncPlayAnimEnd,

        //公平竞技场
        OnUpdateFairDuelEntryState,//显示或者隐藏公平竞技场入口按钮

        //激化
        ItemGrowthFail,
        OnSpecailGrowthFailed,//激化券失败
        ItemGrowthSuccess,    //激化成功

        // 月签到(新版)
        OnUpdateMonthlySignInCountInfo, // 刷新签到次数相关信息
        OnUpdateMonthlySignInItemInfo, // 刷新每日签到信息
        OnUpdateAccumulativeSignInItemInfo, // 刷新累计签到相关信息
        OnMonthlySignInRedPointReset,           //新版月签到红点

        // 终极试炼
        RefreshDungeonBufSuccess,    // 刷新关卡效果buf成功
        RefreshInspireBufSuccess,    // 属性鼓舞buf成功        
        OnSendQueryMallItemInfo, // 查询商城道具信息
        OnQueryMallItenInfoSuccess,//查询商城道具信息成功

        DeadLineReminderChanged, //时限到期提示

        RefreshChatData, // 刷新聊天频道数据
        DungeonChatMsgDataUpdate, //刷新战斗内聊天数据
        DungeonChatInputFieldOpen, //战斗快捷聊天输入面板打开
        DungeonChatInputFieldClose,//战斗快捷聊天输入面板关闭

        SelectFashionEquipToDecompose, // 选择时装进行分解
        CloseFashionEquipDecompose,     // 关闭时装分解界面

        //铭文镶嵌、合成
        OnSelectedInscriptionHole, //选中铭文孔
        OnInscriptionHoleOpenHoleSuccess,//铭文孔开孔成功
        RefreshInscriptionEquipmentList, //更新铭文装备列表
        OnInscriptionMosaicSuccess,      //铭文镶嵌成功
        OnCloseSynthesisIncriptionChanged, //取消合成铭文
        OnIncriptionSynthesisSuccess,      //铭文合成成功
		OnItemEquipInscriptionSucceed,      //铭文镶嵌成功
		

        // 冒险者通行证
        UpdateAventurePassStatus, // 刷新通行证相关数据
        UpdateAventurePassExpPackStatus, // 刷新经验包状态
        OnUpdateAdventureCoin,//冒险币更新
        AdventureUnlockKing,//解锁王者版
        UpdateAventurePassButtonRedPoint, // 刷新通行证入口按钮上的红点
        BuySkillPage2Sucess,//购买技能页2成功
        UpdateGameOptions, // 游戏选项值更新 
        OnUpateRollItem,//刷新roll点后的界面
        OnRollItemEnd,//roll完后的结果
        BuyAdventureLevelSucc, //购买通行证等级成功

        OnUpdateEquipmentScore,//装备评分更新

        StopCloseCommonNewMessageBoxView, // 阻止关闭CommonNewMessageBoxView

        OnUpdateIntegrationChallengeScore,//积分挑战活动
        
        OnEffectHide,//特效隐藏
        OnEffectShow,//特效显示


        //装备方案
        OnReceiveEquipPlanSyncMessage,          //装备方案的数据同步
        OnReceiveEquipPlanSwitchMessage,        //装备方案改变的消息
        OnReceiveEquipPlanItemEndTimeMessage,       //装备方案中存在道具过期被删除的消息

        //荣誉系统
        OnReceiveHonorSystemResMessage,         //荣誉系统消息的返回
        OnReceiveHonorSystemRedPointUpdateMessage,    //荣誉系统小红点的消息

        OnTreasureConversionSuccessed,//宝珠转换成功

        OnEpicEquipmentConversionSuccessed,//史诗装备转化成功

        //吃鸡商店
        OnReceiveSceneShopRefreshSucceed,    //吃鸡商店刷新成功
        OnReceiveSceneShopQuerySucceed,       //吃鸡商店查询公共
        OnReceiveSceneShopItemBuySucceed,       //吃鸡商店中商品购买成功
        //勇士招募
        WarriorRecruitQueryTaskSuccessed,//查询任务成功
        WarriorRecruitQueryIdentitySuccessed,//查询身份成功
        WarriorRecruitBindInviteCodeSuccessed,//绑定邀请码成功
        WarriorRecruitReceiveRewardSuccessed,//领取奖励成功
        WarriorRecruitQueryHireAlreadyBindSuccessed,//查询其他服是否绑定成功

        //仓库
        OnReceiveStorageChangeNameMessage,      //修改名字成功
        OnReceiveStorageUnlockMessage,          //解锁消息
        
        //ItemTipFrame打开和关闭的消息
        OnReceiveItemTipFrameOpenMessage,
        OnReceiveItemTipFrameCloseMessage,

       
        //限时团购活动
        OnGASWholeBargainResSuccessed,

        RefreshControlState,
        OnBeginDrag,
        OnEndDrag,

        OnGuildEventListRes,//公会日志

        // 城镇buf
        OnUpdateTownBuf, // 城镇buf刷新
        
        InputSettingBattleProNameChange,

        //地图自动寻路
        OnSceneMapAutoMoveBeginMessage,
        
        OnEnterDungeonArea,
        OnClearDungeonArea,
        OnDungeonBossKilled,
    }
}
