using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Network;
using Protocol;
using ProtoTable;
using UnityEngine;
using SingleMissionInfo = GameClient.MissionManager.SingleMissionInfo;
///////删除linq

namespace GameClient
{
    #region ADVENTURE TEAM BASE INFO

    public class AdventureTeamRenameModel
    {
        public uint renameItemTableId;
        public ulong renameItemGUID;
        public string newNameStr;

        public AdventureTeamRenameModel()
        {
            Clear();
        }

        public void Clear()
        {
            renameItemGUID = 0;
            newNameStr = "";
        }
    }

    #endregion

    #region Collection

    public class CharacterCollectionModel
    {
        public int jobTableId;
        public bool isTransfer;         //是否是转职职业
        public bool isJobOpened;        //职业是否开放
        public string jobPhotoPath;
        public string jobNamePath;
        public bool needPlay;           //是否需要播放
        public bool isOwned;            //是否已拥有
    }

    #endregion

    #region BLESS SHOP

    public class BlessCrystalModel
    {
        public uint itemTableId;
        public string itemName;
        public string itemIconPath;
        public int currOwnCount;
        public int currNumMaximum;
        public ulong currOwnExp;
        public ulong currExpMaximum;
    }

    #endregion

    #region Inherit Bless

    public class InheritBlessModel
    {
        public uint ownInheritBlessNum;
        public uint inheritBlessMaxNum;
    }

    public class InheritExpModel
    {
        public ulong ownInheritBlessExp;
        public ulong inheritBlessMaxExp;
        public ulong inheritBlessUnitExp;
    }
    #endregion

    #region AdventureTeam Expedition
    public enum ExpeditionRewardCondition
    {
        REQUIRE_ANY_OCCU=0,
        REQUIRE_ANY_SAME_BASE_OCCU=1,
        REQUIRE_ANY_DIFF_BASE_OCCU=2,
        REQUIRE_ANY_DIFF_CHANGED_OCCU=3,
    }

    public struct ExpeditionReward
    {
        public int rolesNum;
        public ExpeditionRewardCondition rewardCondition;
        public string rewards;
        public ExpeditionReward(int num, ExpeditionRewardCondition condition, string reward)
        {
            rolesNum = num;
            rewardCondition = condition;
            rewards = reward;
        }
    }

    public class ExpeditionMapModel
    {
        public string mapName;
        public int playerLevelLimit;
        public int adventureTeamLevelLimit;
        public int rolesCapacity;//支持远征角色数量
        public string expeditionTime;
        public string mapImagePath;
        public string miniMapImagePath;
        public List<ExpeditionReward> rewardList;

        //额外数据 服务器数据
        public ExpeditionMapNetInfo mapNetInfo;

        public ExpeditionMapModel(byte id, string name,int playerLevel,int teamLevel, int rolesNum,string time,string map,string minimap)
        {
            mapName = name;
            playerLevelLimit = playerLevel;
            adventureTeamLevelLimit = teamLevel;
            rolesCapacity = rolesNum;
            expeditionTime = time;
            mapImagePath = map;
            miniMapImagePath = minimap;
            rewardList = new List<ExpeditionReward>();

            mapNetInfo = new ExpeditionMapNetInfo(id);
        }

        public void Clear()
        {
            if (rewardList != null)
            {
                rewardList.Clear();
            }
            if (mapNetInfo != null)
            {
                mapNetInfo.Clear();
            }
        }
    }

    /// <summary>
    /// 存储读表信息用于查询
    /// </summary>
    public class ExpeditionMapBaseInfo
    {
        //key == mapId
        public Dictionary<int,ExpeditionMapModel> expeditionMapDic;
        
        public ExpeditionMapBaseInfo()
        {
            expeditionMapDic = new Dictionary<int, ExpeditionMapModel>();
        }

        public void Clear()
        {
            if(null != expeditionMapDic)
            {
                var mapDicEnum = expeditionMapDic.GetEnumerator();
                while (mapDicEnum.MoveNext())
                {
                    var mapItem = mapDicEnum.Current.Value as ExpeditionMapModel;
                    if (mapItem == null)
                        continue;
                    mapItem.Clear();
                }
                expeditionMapDic.Clear();
            }
        }
    }

    /// <summary>
    /// 服务器当前地图信息，只维护当前应当显示的地图，切换地图，执行操作后更新内容
    /// </summary>
    public class ExpeditionMapNetInfo
    {
        public byte mapId;
        public ExpeditionStatus mapStatus;
        public uint durationOfExpedition;//远征持续时间 从服务器获取 本地修改
        public uint endTimeOfExpedition;
        public List<ExpeditionMemberInfo> roles;

        public ExpeditionMapNetInfo(byte id)
        {
            mapId = id;    //默认远征地图id = 1
            mapStatus = ExpeditionStatus.EXPEDITION_STATUS_PREPARE;
            durationOfExpedition = 0;
            endTimeOfExpedition = 0;
            roles = new List<ExpeditionMemberInfo>();
        }

        public ExpeditionMapNetInfo()
        {
            mapId = 1;
            mapStatus = ExpeditionStatus.EXPEDITION_STATUS_PREPARE;
            durationOfExpedition = 0;
            endTimeOfExpedition = 0;
            roles = new List<ExpeditionMemberInfo>();
        }

        public void Clear()
        {
            if (null != roles)
            {
                roles.Clear();
            }
        }
    }
    #endregion

    #region Bounty Shop

    public class BountyModle
    {
        public uint itemTableId;
        public string itemName;
        public string itemIconPath;
        public int currOwnCount;
    }

    #endregion

    /// <summary>
    /// 冒险队的数据管理器
    /// </summary>
    public class AdventureTeamDataManager : DataManager<AdventureTeamDataManager>
    {
#region Model Params

        private bool bLocalDataInited = false;
        private bool bNetInited = false;

        //注意 这个功能开关不只是进入城镇才有 在选角界面也需要加
        //注意 这个字段不能这样赋值 ！！！
        //private bool bFuncOpened;
        public bool BFuncOpened
        {
            get { 
                //return bFuncOpened; 
                return !ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_ADVENTURE_TEAM) && !IsAdventureTeamNameEmpty();
            }
        }

        #region Base Info

        private int adventureTeamLevelMinimun = 0;
        public int AdventureTeamLevelMinimum
        {
            get { return adventureTeamLevelMinimun; }
        }
        private int adventureTeamLevelMaximun = 10;
        public int AdventureTeamLevelMaximum
        {
            get { return adventureTeamLevelMaximun; }
        }

        private int renameLimitCharNum = 7;
        public int RenameLimitCharNum { get { return renameLimitCharNum; } }

        private Dictionary<int, ulong> mAdventureTeamUpLevelExpDic = new Dictionary<int, ulong>();

        private int mPlayerMaxLevel;
        public int PlayerMaxLevel
        {
            get
            {
                if (mPlayerMaxLevel == 0)
                {
                    var maxLevel = 60;
                    var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_PLAYER_MAX_LEVEL_LIMIT);
                    if (null != systemValue)
                    {
                        maxLevel = systemValue.Value;
                    }
                    mPlayerMaxLevel = maxLevel;
                }
                return mPlayerMaxLevel;
            }
        }


        private int mRenameCardTableId;
        public int RenameCardTableId
        {
            get
            {
                return mRenameCardTableId;
            }
        }
		
        private int weeklyTaskRefreshHour = 6;
        public int WeeklyTaskRefreshHour
        {
            get { return weeklyTaskRefreshHour; }
        }

        #endregion

        #region Character Collection

        //key => base job id
        private Dictionary<int, List<CharacterCollectionModel>> mCharacterCollectionDic = new Dictionary<int, List<CharacterCollectionModel>>();
        private List<CharacterCollectionModel> mTotalCharacterCollection = new List<CharacterCollectionModel>();
        private Dictionary<int, string> mBaseJobTableIdWithNameDic = new Dictionary<int, string>();

        #endregion

        #region Bless Shop

        private BlessCrystalModel blessCrystalModel;
        public BlessCrystalModel BlessCrystalModel
        {
            get
            {
                if (blessCrystalModel == null)
                {
                    blessCrystalModel = new BlessCrystalModel();
                }
                return blessCrystalModel;
            }
        }

        /*
         *  private BlessShopModel mBlessShopModel = new BlessShopModel();
        //key => shopId
        private Dictionary<int, AccountShopLocalItemInfoDicByTabType> mShopIdShopItemInfoDic = new Dictionary<int, AccountShopLocalItemInfoDicByTabType>();
        //key => shopItemId
        private Dictionary<int, AccountShopLocalItemInfo> mShopItemIdShopItemInfoDic = new Dictionary<int, AccountShopLocalItemInfo>();
        */

        #endregion

        #region Pass Bless

        private ItemTable mPassBlessItem;
        public ItemTable PassBlessItem
        {
            get
            {
                if (null == mPassBlessItem)
                {
                    var passBlessItemId = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_INHERIT_BLESS_ITEM_TABLE_ID);
                    if (null != passBlessItemId)
                    {
                        mPassBlessItem = TableManager.GetInstance().GetTableItem<ItemTable>(passBlessItemId.Value);
                    }
                }
                return mPassBlessItem;
            }
        }

        private InheritBlessModel inheritBlessModel;
        public InheritBlessModel InheritBlessModel
        {
            get
            {
                if (inheritBlessModel == null)
                {
                    inheritBlessModel = new InheritBlessModel();
                }
                return inheritBlessModel;
            }
        }

        private InheritExpModel inheritExpModel;
        public InheritExpModel InheritExpModel
        {
            get
            {
                if (inheritExpModel == null)
                {
                    inheritExpModel = new InheritExpModel();
                }
                return inheritExpModel;
            }
        }

        //界面数据 需要清空 保证本次有效！！！
        private InheritBlessModel uiTempInheritBlessModel;
        public InheritBlessModel UiTempInheritBlessModel { get { return uiTempInheritBlessModel; } }
        private InheritExpModel uiTempInheritExpModel;
        public InheritExpModel UiTempInheritExpModel { get { return uiTempInheritExpModel; } }

        #endregion 

        #region Bounty Shop

        private BountyModle bountyModel;
        public BountyModle BountyModel
        {
            get
            {
                if (bountyModel == null)
                {
                    bountyModel = new BountyModle();
                }
                return bountyModel;
            }
        }
        
        #endregion

        #region Expedition
        private ExpeditionMapBaseInfo mExpeditionMapBaseInfo = new ExpeditionMapBaseInfo();

        public ExpeditionMapBaseInfo ExpeditionMapBaseInfo
        {
            get
            {
                if(mExpeditionMapBaseInfo == null)
                {
                    mExpeditionMapBaseInfo = new ExpeditionMapBaseInfo();
                }
                return mExpeditionMapBaseInfo;
            }
        }


        //缓存数据 当前展示的远征地图 服务器返回数据
        //默认id 为 1
        private ExpeditionMapNetInfo mExpeditionMapNetInfo = new ExpeditionMapNetInfo(1);

        public ExpeditionMapNetInfo ExpeditionMapNetInfo
        {
            get
            {
                if (mExpeditionMapNetInfo == null)
                {
                    mExpeditionMapNetInfo = new ExpeditionMapNetInfo(1);
                }
                return mExpeditionMapNetInfo;
            }
        }

        //缓存协议接收的远征角色
        private ExpeditionMemberInfo[] mExpeditionRoles;

        //是否切换远征地图标志位，用于控制远征界面是否加载奖励数据
        private bool mIsChangedExpeditionMap = false;

        public bool IsChangedExpeditionMap
        {
            get
            {
                return mIsChangedExpeditionMap;
            }
        }
        //是否切换远征时间标志位
        private bool mIsChangedExpeditionTime = false;

        public bool IsChangeExpeditionTime
        {
            get
            {
                return mIsChangedExpeditionTime;
            }
            set
            {
                mIsChangedExpeditionTime = value;
            }
        }

        private bool mIsChangedExpeditionRoles = false;

        public bool IsChangedExpedtionRoles
        {
            get
            {
                return mIsChangedExpeditionRoles;
            }
            set
            {
                mIsChangedExpeditionRoles = value;
            }
        }

        private bool mCanGetReward = false;
        #endregion

        #region WeeklyTask
        List<SingleMissionInfo> m_ADTMissionList = new List<SingleMissionInfo>();

        public List<SingleMissionInfo> ADTMissionList
        {
            get
            {
                return m_ADTMissionList;
            }
        }

        int m_ADTMissionFinishMaxNum = 0;

        public int ADTMissionFinishMaxNum
        {
            get
            {
                return m_ADTMissionFinishMaxNum;
            }
        }

        bool hasWeeklyTaskReceived = false;
        #endregion

        #region TR

        private string tr_rename_content_empty = "";
        private string tr_rename_content_beyond_max = "";
        private string tr_rename_content_illegal = "";
        private string tr_rename_content_be_used = "";
        private string tr_rename_content_no_changed = "";
        private string tr_rename_content_failed = "";
        private string tr_rename_content_success = "";
        private string tr_rename_quick_buy_ask = "";

        private string tr_adventure_team_level_up_tip = "";

        private string tr_select_role_extend_succ = "";
        private string tr_select_role_field_reach_max = "";
        private string tr_select_role_field_not_use_total = "";
        private string tr_select_role_field_extend_failed = "";

        private string tr_collection_first_tab_name = "";

        private string tr_expedition_dispatch_succeed = "";
        private string tr_expedition_dispatch_fail = "";
        private string tr_expedition_dispatch_dup = "";
        private Dictionary<int, string> tr_expedition_requires = new Dictionary<int, string>();
        #endregion

        #region Extend Model Params

        public bool OnAdventureTeamLevelChangedFlag { get; set; }

        private bool hasWeeklyTaskCheckedToday = false;
        private bool onFirstCheckWeeklyTaskFlag = true;   //默认每天首次显示 并且查看后消失
        public bool  OnFirstCheckWeeklyTaskFlag
        {
            get {
                return onFirstCheckWeeklyTaskFlag && !hasWeeklyTaskCheckedToday;
            }
            set
            {
                if (!value)
                {
                    onFirstCheckWeeklyTaskFlag = value;
                    _NotifyWeeklyTaskStatusChanged();
                    if (!hasWeeklyTaskCheckedToday)
                    {
                        _SaveWeeklyTaskCheckTimestamp();
                        hasWeeklyTaskCheckedToday = _IsWeeklyTaskCheckedToday();
                    }
                }
            }
        }

        private bool onFirstCheckPassBlessFlag = true;      //默认每天首次显示 并且查看后消失
        public bool OnFirstCheckPassBlessFlag
        {
            get
            {
                return onFirstCheckPassBlessFlag;
            }
            set
            {
                if (!value)
                {
                    if (isPassBlessOwnCountAddup)
                    {
                        isPassBlessOwnCountAddup = false;       //不可使用成长药剂的角色，成长药剂每收集满1瓶，出现红点，查看后消失，当日不再出现
                        _NotifyPassBlessCountChanged();
                    }
                    if (isPassBlessCanUse)
                    {
                        onFirstCheckPassBlessFlag = value;
                        _NotifyPassBlessCountChanged();
                    }
                    else if (isPassBlessEnoughOnFirstLogin)
                    {
                        onFirstCheckPassBlessFlag = value;
                        _NotifyPassBlessCountChanged();
                        _SavePassBlessCheckTimestamp();
                        hasPassBlessCheckedToday = _IsPassBlessCheckedToday();
                    }
                }
            }
        }

        private bool isPassBlessOwnCountAddup = false;      //成长药剂数量是否累加
        private bool isPassBlessOwnCountInit = false;       //成长药剂数量初始化
        private bool isPassBlessCanUse = false;
        private bool isPassBlessEnoughOnFirstLogin = false;
        private bool hasPassBlessCheckedToday = false;

        #endregion

#endregion

#region PRIVATE METHODS

        #region Initialize
        public override void Initialize()
        {
            _BindEvents();
            _InitTR();
            _InitLocalData();
        }

        public override void Clear()
        {
            _UnBindEvents();
            _ClearLocalData();
            _ClearTR();
        }

        private void _BindEvents()
        {
            if (bNetInited == false)
            {
                NetProcess.AddMsgHandler(AdventureTeamInfoSync.MsgID, _OnSyncAdventureTeamInfo);
                NetProcess.AddMsgHandler(WorldAdventureTeamRenameRes.MsgID, _OnAdventureTeamRenameRes);
                NetProcess.AddMsgHandler(WorldExtensibleRoleFieldUnlockRes.MsgID, _OnExtensibleRoleFieldUnlockRes);

                NetProcess.AddMsgHandler(WorldBlessCrystalInfoRes.MsgID, _OnBlessCrystalInfoRes);
                NetProcess.AddMsgHandler(WorldInheritBlessInfoRes.MsgID, _OnPassBlessInfoRes);
                NetProcess.AddMsgHandler(WorldInheritExpRes.MsgID, _OnUsePassBlessExpRes);

                NetProcess.AddMsgHandler(WorldQueryExpeditionMapRes.MsgID, _OnExpeditionMapInfoRes);
                NetProcess.AddMsgHandler(WorldQueryOptionalExpeditionRolesRes.MsgID, _OnExpeditionRolesRes);
                NetProcess.AddMsgHandler(WorldDispatchExpeditionTeamRes.MsgID, _OnDispatchExpeditionTeamRes);
                NetProcess.AddMsgHandler(WorldCancelExpeditionRes.MsgID, _OnCancelExpeditionRes);
                NetProcess.AddMsgHandler(WorldGetExpeditionRewardsRes.MsgID, _OnGetExpeditionRewardsRes);
                NetProcess.AddMsgHandler(WorldQueryAllExpeditionMapsRes.MsgID, _OnGetAllExpeditionMaps);
                NetProcess.AddMsgHandler(WorldAllExpeditionMapsSync.MsgID, _OnOnceExpeditionDispatchFinish);

                NetProcess.AddMsgHandler(WorldAdventureTeamExtraInfoRes.MsgID, _OnAdventureTeamExtraInfoRes);

                NetProcess.AddMsgHandler(WorldAccountCounterNotify.MsgID, _OnWorldAccountCounterNotify);

                NetProcess.AddMsgHandler(WorldQueryOwnOccupationsRes.MsgID, _OnQueryOwnJobRes);
                NetProcess.AddMsgHandler(WorldQueryOwnOccupationsSync.MsgID, _OnSyncOwnNewJobs);

                NetProcess.AddMsgHandler(WorldMallBuyRet.MsgID, _OnWorldMallBuyRet);

                ServerSceneFuncSwitchManager.GetInstance().AddServerFuncSwitchListener(ServiceType.SERVICE_ADVENTURE_TEAM, _OnServerSwitchFunc);

                bNetInited = true;
            }
        }

        private void _UnBindEvents()
        {
            NetProcess.RemoveMsgHandler(AdventureTeamInfoSync.MsgID, _OnSyncAdventureTeamInfo);
            NetProcess.RemoveMsgHandler(WorldAdventureTeamRenameRes.MsgID, _OnAdventureTeamRenameRes);
            NetProcess.RemoveMsgHandler(WorldExtensibleRoleFieldUnlockRes.MsgID, _OnExtensibleRoleFieldUnlockRes);

            NetProcess.RemoveMsgHandler(WorldBlessCrystalInfoRes.MsgID, _OnBlessCrystalInfoRes);
            NetProcess.RemoveMsgHandler(WorldInheritBlessInfoRes.MsgID, _OnPassBlessInfoRes);
            NetProcess.RemoveMsgHandler(WorldInheritExpRes.MsgID, _OnUsePassBlessExpRes);

            NetProcess.RemoveMsgHandler(WorldQueryExpeditionMapRes.MsgID, _OnExpeditionMapInfoRes);
            NetProcess.RemoveMsgHandler(WorldQueryOptionalExpeditionRolesRes.MsgID, _OnExpeditionRolesRes);
            NetProcess.RemoveMsgHandler(WorldDispatchExpeditionTeamRes.MsgID, _OnDispatchExpeditionTeamRes);
            NetProcess.RemoveMsgHandler(WorldCancelExpeditionRes.MsgID, _OnCancelExpeditionRes);
            NetProcess.RemoveMsgHandler(WorldGetExpeditionRewardsRes.MsgID, _OnGetExpeditionRewardsRes);
            NetProcess.RemoveMsgHandler(WorldQueryAllExpeditionMapsRes.MsgID, _OnGetAllExpeditionMaps);
            NetProcess.RemoveMsgHandler(WorldAllExpeditionMapsSync.MsgID, _OnOnceExpeditionDispatchFinish);

            NetProcess.RemoveMsgHandler(WorldAdventureTeamExtraInfoRes.MsgID, _OnAdventureTeamExtraInfoRes);

            NetProcess.RemoveMsgHandler(WorldAccountCounterNotify.MsgID, _OnWorldAccountCounterNotify);

            NetProcess.RemoveMsgHandler(WorldQueryOwnOccupationsRes.MsgID, _OnQueryOwnJobRes);
            NetProcess.RemoveMsgHandler(WorldQueryOwnOccupationsSync.MsgID, _OnSyncOwnNewJobs);

            NetProcess.RemoveMsgHandler(WorldMallBuyRet.MsgID, _OnWorldMallBuyRet);

            ServerSceneFuncSwitchManager.GetInstance().RemoveServerFuncSwitchListener(ServiceType.SERVICE_ADVENTURE_TEAM, _OnServerSwitchFunc);
        }

        #endregion

        #region Base Data

        private void _InitTR()
        {
            tr_rename_content_empty = TR.Value("adventure_team_change_name_no_content");
            tr_rename_content_beyond_max = TR.Value("adventure_team_change_name_exceed_upper_limit");
            tr_rename_content_illegal = TR.Value("adventure_team_change_name_content_no_law");
            tr_rename_content_be_used = TR.Value("adventure_team_change_name_content_used");
            tr_rename_content_no_changed = TR.Value("adventure_team_change_name_content_same");
            tr_rename_content_failed = TR.Value("adventure_team_change_name_content_change_failed");
            tr_rename_content_success = TR.Value("adventure_team_change_name_content_change_succeed");
            tr_rename_quick_buy_ask = TR.Value("adventure_team_change_name_ask");

            tr_adventure_team_level_up_tip = TR.Value("adventure_team_level_up_succeed_tip");

            tr_select_role_extend_succ = TR.Value("select_role_extend_field_num_succ");
            tr_select_role_field_reach_max = TR.Value("select_role_field_reach_max");
            tr_select_role_field_not_use_total = TR.Value("select_role_field_not_use_total");
            tr_select_role_field_extend_failed = TR.Value("select_role_field_extend_failed");

            tr_collection_first_tab_name = TR.Value("adventure_team_collection_first_tab");

            tr_expedition_dispatch_succeed = TR.Value("adventure_team_expeidtion_dispatch_succeed_tips");
            tr_expedition_dispatch_fail = TR.Value("adventure_team_expeidtion_dispatch_fail_tips");
            tr_expedition_dispatch_dup = TR.Value("adventure_team_expeidtion_dispatch_dup_tips");
            tr_expedition_requires = new Dictionary<int, string>();
            tr_expedition_requires.Add((int)ExpeditionRewardCondition.REQUIRE_ANY_OCCU, TR.Value("adventure_team_expedition_REQUIRE_ANY_OCCU"));
            tr_expedition_requires.Add((int)ExpeditionRewardCondition.REQUIRE_ANY_SAME_BASE_OCCU, TR.Value("adventure_team_expedition_REQUIRE_ANY_SAME_BASE_OCCU"));
            tr_expedition_requires.Add((int)ExpeditionRewardCondition.REQUIRE_ANY_DIFF_BASE_OCCU, TR.Value("adventure_team_expedition_REQUIRE_ANY_DIFF_BASE_OCCU"));
            tr_expedition_requires.Add((int)ExpeditionRewardCondition.REQUIRE_ANY_DIFF_CHANGED_OCCU, TR.Value("adventure_team_expedition_REQUIRE_ANY_DIFF_CHANGED_OCCU"));
        }

        private void _ClearTR()
        {
            tr_rename_content_empty = "";
            tr_rename_content_beyond_max = "";
            tr_rename_content_illegal = "";
            tr_rename_content_be_used = "";
            tr_rename_content_no_changed = "";
            tr_rename_content_failed = "";
            tr_rename_content_success = "";
            tr_rename_quick_buy_ask = "";

            tr_adventure_team_level_up_tip = "";

            tr_select_role_extend_succ = "";
            tr_select_role_field_reach_max = "";
            tr_select_role_field_not_use_total = "";
            tr_select_role_field_extend_failed = "";

            tr_collection_first_tab_name = "";

            tr_expedition_dispatch_succeed = "";
            tr_expedition_dispatch_fail = "";
            tr_expedition_dispatch_dup = "";
            tr_expedition_requires.Clear();
        }

        private void _InitLocalData()
        {
            if (bLocalDataInited)
            {
                return;
            }

            bLocalDataInited = true;

            var guildNameMaxWordsTable = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_GUILD_NAME_MAX_WORDS);
            if (guildNameMaxWordsTable != null)
            {
                renameLimitCharNum = guildNameMaxWordsTable.Value;
            }

            //功能开关
            //bFuncOpened = !ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_ADVENTURE_TEAM) && !IsAdventureTeamNameEmpty();

            //不初始化  不能随意中断初始化流程  这儿中断了 其他地方也要监听是否开启  ！！！
            //if (bFuncOpened == false)
            //{
            //    return;
            //}

            var refreshHourSystemValueTable = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_DAILY_TODO_REFRESH_TIME);
            if (refreshHourSystemValueTable != null)
            {
                weeklyTaskRefreshHour = refreshHourSystemValueTable.Value;
            }

            _InitAdventureTeamTableData();

            _InitBlessCrystalModel();

            _InitBountyModel();

            _InitExpeditionMapLocalData();

            _InitPassBlessModel();

            _InitCollectionCharacterModel();

            _InitWeeklyTaskModel();
        }

        private void _ClearLocalData()
        {
            bLocalDataInited = false;
            bNetInited = false;
            //bFuncOpened = false;

            if(mAdventureTeamUpLevelExpDic != null)
            {
                mAdventureTeamUpLevelExpDic.Clear();
            }

            if (mCharacterCollectionDic != null)
            {
                var enumerator = mCharacterCollectionDic.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var model = enumerator.Current.Value as List<CharacterCollectionModel>;
                    if(model != null)
                    {
                        model.Clear();
                    }
                }
                mCharacterCollectionDic.Clear();
            }
            if (mTotalCharacterCollection != null)
            {
                mTotalCharacterCollection.Clear();
            }
            if (mBaseJobTableIdWithNameDic != null)
            {
                mBaseJobTableIdWithNameDic.Clear();
            }

            blessCrystalModel = null;
            bountyModel = null;

            inheritBlessModel = null;
            inheritExpModel = null;
            mPassBlessItem = null;
            isPassBlessOwnCountAddup = false;
            isPassBlessOwnCountInit = false;
            isPassBlessCanUse = false;
            isPassBlessEnoughOnFirstLogin = false;
            hasPassBlessCheckedToday = false;            
            hasWeeklyTaskCheckedToday = false;

            //需要清空 保证本次有效
            uiTempInheritBlessModel = null;
            uiTempInheritExpModel = null;

            OnAdventureTeamLevelChangedFlag = false;
            onFirstCheckWeeklyTaskFlag = true;
            onFirstCheckPassBlessFlag = true;

            if (mExpeditionMapBaseInfo != null)
            {
                mExpeditionMapBaseInfo.Clear();
            }
            mExpeditionRoles = null;
            mCanGetReward = false;
            if (mExpeditionMapNetInfo != null)
            {
                mExpeditionMapNetInfo.Clear();
            }

            _UnInitWeeklyTaskModel();
        }

        private void _InitAdventureTeamTableData()
        {
            if(mAdventureTeamUpLevelExpDic == null)
            {
                mAdventureTeamUpLevelExpDic = new Dictionary<int, ulong>();
            } else
            {
                mAdventureTeamUpLevelExpDic.Clear();
            }
            var aTeamTable = TableManager.GetInstance().GetTable<AdventureTeamTable>();
            if(aTeamTable == null)
            {
                return;
            }
            var enumerator = aTeamTable.GetEnumerator();
            int index = 0;
            while(enumerator.MoveNext())
            {
                var tableItem = enumerator.Current.Value as AdventureTeamTable;
                if(tableItem == null) continue;

                //exp
                ulong upLevelExp;
                if (ulong.TryParse(tableItem.Exp, out upLevelExp))
                {
                    mAdventureTeamUpLevelExpDic[tableItem.ID] =  upLevelExp;
                }

                //level
                if (index == 0)
                {
                    adventureTeamLevelMaximun = adventureTeamLevelMinimun = tableItem.ID;
                }
                if (tableItem.ID > adventureTeamLevelMaximun)
                {
                    adventureTeamLevelMaximun = tableItem.ID;
                }
                else if (tableItem.ID < adventureTeamLevelMinimun)
                {
                    adventureTeamLevelMinimun = tableItem.ID;
                }
                index++;
            }
        }

        private string _GetAdventureTeamTableIncomeDescByType(int adventureTeamLevel)
        {
            string desc = "";
            var teamTable = TableManager.GetInstance().GetTableItem<AdventureTeamTable>(adventureTeamLevel);
            if (teamTable == null)
            {
                return desc;
            }
            desc = teamTable.PropertyIncomeDesc;
            return desc;
        }

        /// <summary>
        /// 判断是否还有未使用的角色栏位
        /// 
        /// 如果角色被删除 但是roleinfo还是有角色的  所以需要判断 可用 角色数 
        /// </summary>
        /// <returns></returns>
        private bool _CheckHasUnUsedRoleFields()
        {
            bool hasUnUsed = false;

            if (RecoveryRoleCachedObject.HasOwnedRoles < RecoveryRoleCachedObject.EnabledRoleField)
            {
                hasUnUsed = true;
            }

            return hasUnUsed;
        }

        #endregion

        #region Local Data

        private void _InitBlessCrystalModel()
        {
            if (blessCrystalModel == null)
            {
                blessCrystalModel = new BlessCrystalModel();
            }
            var blessItemId = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_BLESS_CRYSTAL_ITEM_TABLE_ID);
            if (blessItemId != null)
            {
                blessCrystalModel.itemTableId = (uint)blessItemId.Value;
                var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(blessItemId.Value);
                if (itemTable != null)
                {
                    blessCrystalModel.itemName = itemTable.Name;
                    blessCrystalModel.itemIconPath = itemTable.Icon;
                }
            }
            var blessCrystalOwnMaxNum = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_BLESS_CRYSTAL_OWN_MAX_NUM);
            if (blessCrystalOwnMaxNum != null)
            {
                blessCrystalModel.currNumMaximum = blessCrystalOwnMaxNum.Value;
            }
            var blessCrystalOwnMaxExp = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_BLESS_CRYSTAL_OWN_MAX_EXP);
            if (blessCrystalOwnMaxExp != null)
            {
                blessCrystalModel.currExpMaximum = (ulong)blessCrystalOwnMaxExp.Value;
            }
        }

        private void _InitBountyModel()
        {
            if (bountyModel == null)
            {
                bountyModel = new BountyModle();
            }
            var bountyItemId = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_BLESS_BOUNTY_ITEM_TABLE_ID);
            if (bountyItemId != null)
            {
                bountyModel.itemTableId = (uint)bountyItemId.Value;
                var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(bountyItemId.Value);
                if (itemTable != null)
                {
                    bountyModel.itemName = itemTable.Name;
                    bountyModel.itemIconPath = itemTable.Icon;
                }
            }
        }

        private void _InitExpeditionMapLocalData()
        {
            if(null == mExpeditionMapBaseInfo)
            {
                mExpeditionMapBaseInfo = new ExpeditionMapBaseInfo();
            }

            var expeditionMapTable = TableManager.GetInstance().GetTable<ExpeditionMapTable>();
            if(null == expeditionMapTable)
            {
                return;
            }
            var enumerator = expeditionMapTable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var expeditionMapItem = enumerator.Current.Value as ExpeditionMapTable;
                if (expeditionMapItem == null) continue;
                mExpeditionMapBaseInfo.expeditionMapDic.Add(expeditionMapItem.ID,
                    new ExpeditionMapModel((byte)expeditionMapItem.ID, expeditionMapItem.MapName, expeditionMapItem.PlayerLevelLimit,
                    expeditionMapItem.AdventureTeamLevelLimit, expeditionMapItem.RolesCapacity,
                    expeditionMapItem.ExpeditionTime, expeditionMapItem.BackgroundPath, expeditionMapItem.MiniMapPath));
            }

            var expeditionRewardTable = TableManager.GetInstance().GetTable<ExpeditionMapRewardTable>();
            if(null == expeditionMapTable)
            {
                return;
            }
            var tempEnumrator = expeditionRewardTable.GetEnumerator();
            while(tempEnumrator.MoveNext())
            {
                var expeditionRewardItem = tempEnumrator.Current.Value as ExpeditionMapRewardTable;
                if (expeditionRewardItem == null) continue;
                ExpeditionRewardCondition tempCondition = ExpeditionRewardCondition.REQUIRE_ANY_OCCU;
                int rolesCount = 0;
                if (expeditionRewardItem.RequireAnyOccu != 0)
                {
                    rolesCount = expeditionRewardItem.RequireAnyOccu;
                }
                else if (expeditionRewardItem.RequireAnySameBaseOccu != 0)
                {
                    rolesCount = expeditionRewardItem.RequireAnySameBaseOccu;
                    tempCondition = ExpeditionRewardCondition.REQUIRE_ANY_SAME_BASE_OCCU;
                }
                else if (expeditionRewardItem.RequireAnyDiffBaseOccu != 0)
                {
                    rolesCount = expeditionRewardItem.RequireAnyDiffBaseOccu;
                    tempCondition = ExpeditionRewardCondition.REQUIRE_ANY_DIFF_BASE_OCCU;
                }
                else if (expeditionRewardItem.RequireAnyDiffChangedOccu != 0)
                {
                    rolesCount = expeditionRewardItem.RequireAnyDiffChangedOccu;
                    tempCondition = ExpeditionRewardCondition.REQUIRE_ANY_DIFF_CHANGED_OCCU;
                }
                ExpeditionReward tempReward = new ExpeditionReward(rolesCount, tempCondition, expeditionRewardItem.Rewards);
                mExpeditionMapBaseInfo.expeditionMapDic[expeditionRewardItem.ExpeditionMapId].rewardList.Add(tempReward);
            }

        }

        private void _InitPassBlessModel()
        {
            if (null == inheritBlessModel)
            {
                inheritBlessModel = new InheritBlessModel();
            }
            var passBlessOwnMaxNum = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_INHERIT_BLESS_OWN_MAX_NUM);
            if (passBlessOwnMaxNum != null)
            {
                inheritBlessModel.inheritBlessMaxNum = (uint)passBlessOwnMaxNum.Value;                
            }

            if (null == inheritExpModel)
            {
                inheritExpModel = new InheritExpModel();
            }
            var passBlessOwnMaxExp = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_INHERIT_BLESS_OWN_MAX_EXP);
            if (passBlessOwnMaxExp != null)
            {
                inheritExpModel.inheritBlessMaxExp = (uint)passBlessOwnMaxExp.Value;
            }
            var passBlessUseUnitExp = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_INHERIT_BLESS_UNIT_EXP_FOR_USE);
            if (passBlessUseUnitExp != null)
            {
                inheritExpModel.inheritBlessUnitExp = (uint)passBlessUseUnitExp.Value;
            }

            if (null == mPassBlessItem)
            {
                var passBlessItemId = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_INHERIT_BLESS_ITEM_TABLE_ID);
                if (null != passBlessItemId)
                {
                    mPassBlessItem = TableManager.GetInstance().GetTableItem<ItemTable>(passBlessItemId.Value);
                }               
            }

            hasPassBlessCheckedToday = _IsPassBlessCheckedToday();
        }

        private string _getExpeitionMapIdTime(int mapId)
        {
            string serverId = "";
            string accId = "";
            if (null != ClientApplication.playerinfo) 
            {
                serverId = ClientApplication.playerinfo.serverID.ToString();
                accId = ClientApplication.playerinfo.accid.ToString();
            }
            return TR.Value("adventure_team_expedition_mapid_time_setting", serverId, accId, mapId);
        }

        private void _SetExpeditionMapIdTime(int mapId, int expeditionDuration)
        {
            PlayerPrefs.SetInt(_getExpeitionMapIdTime(mapId), expeditionDuration);
        }

        private uint _GetExpeditionMapIdTime(int mapId)
        {
            return (uint)PlayerPrefs.GetInt(_getExpeitionMapIdTime(mapId), 1);
        }

        private void _InitCollectionCharacterModel()
        {
            //初始化默认页签
            if (mBaseJobTableIdWithNameDic == null)
            {
                mBaseJobTableIdWithNameDic = new Dictionary<int, string>();
            }
            mBaseJobTableIdWithNameDic.Add(0, tr_collection_first_tab_name);

            //初始化角色收藏
            if (mCharacterCollectionDic == null)
            {
                mCharacterCollectionDic = new Dictionary<int, List<CharacterCollectionModel>>();
            }
            if (mTotalCharacterCollection == null)
            {
                mTotalCharacterCollection = new List<CharacterCollectionModel>();
            }

            var jobTableDic = TableManager.GetInstance().GetTable<JobTable>();
            if (jobTableDic == null)
            {
                return;
            }
            List<CharacterCollectionModel> models = null;
            
            var enumerator = jobTableDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var jobTable = enumerator.Current.Value as JobTable;
                if (jobTable == null)
                    continue;
                CharacterCollectionModel model = new CharacterCollectionModel();
                model.jobTableId = jobTable.ID;
                model.isTransfer = jobTable.JobType == 1 ? true : false;
                model.isJobOpened = jobTable.Open == 1 ? true : false;

                //存储 打开的基础职业 到职业页签数据中
                if (!model.isTransfer && model.isJobOpened)
                {
                    int baseJobId = model.jobTableId;
                    if (!mBaseJobTableIdWithNameDic.ContainsKey(baseJobId))
                        mBaseJobTableIdWithNameDic.Add(baseJobId, jobTable.Name);
                    else
                        mBaseJobTableIdWithNameDic[baseJobId] = jobTable.Name;
                    continue;
                }

                //存储 全部转职职业（无论是否打开） 到收藏数据中
                //注意 key => 基础职业 id
                model.jobPhotoPath = jobTable.CharacterCollectionPhoto;
                model.jobNamePath = jobTable.CharacterCollectionArtLetting;

                //过滤掉开放职业的 但是没有配置资源的
                if (!model.isJobOpened || 
                    string.IsNullOrEmpty(model.jobPhotoPath) || string.IsNullOrEmpty(model.jobNamePath))
                {
                    continue;
                }
                if (mCharacterCollectionDic.TryGetValue(jobTable.prejob, out models))
                {
                    if (models == null)
                    {
                        models = new List<CharacterCollectionModel>();
                    }
                    models.Add(model);
                }
                else
                {
                    mCharacterCollectionDic.Add(jobTable.prejob, new List<CharacterCollectionModel>() { model });
                }
                //存到全部那儿
                mTotalCharacterCollection.Add(model);
            }

            //存一个敬请期待的空数据
            var fakeModel =  new CharacterCollectionModel(){
                jobTableId = 0,
                isTransfer = true,
                isJobOpened = false,
                jobPhotoPath = "",
                jobNamePath = "",
                needPlay = false,
                isOwned = false
            };
            mCharacterCollectionDic.Add(Int16.MaxValue, new List<CharacterCollectionModel>() { fakeModel });
            mTotalCharacterCollection.Add(fakeModel);
        }
        #endregion

#region Net Data Ret

        #region Base Info
        private void _OnSyncAdventureTeamInfo(MsgDATA data)
        {
            if (data == null)
            {
                return;
            }
            var syncInfo = new AdventureTeamInfoSync();
            syncInfo.decode(data.bytes);
            if(syncInfo.info != null && ClientApplication.playerinfo != null && ClientApplication.playerinfo.adventureTeamInfo != null)
            {
                //注意顺序
                int tempTeamLevel = ClientApplication.playerinfo.adventureTeamInfo.adventureTeamLevel;

                if (string.IsNullOrEmpty(ClientApplication.playerinfo.adventureTeamInfo.adventureTeamName) &&
                    string.IsNullOrEmpty(syncInfo.info.adventureTeamName) == false)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamFuncChanged);
                }

                ClientApplication.playerinfo.adventureTeamInfo = syncInfo.info;
                if (syncInfo.info.adventureTeamLevel != tempTeamLevel)
                {
                    _NotifyLevelUp();

                    //飘字
                    SystemNotifyManager.SysNotifyTextAnimation(string.Format(tr_adventure_team_level_up_tip, syncInfo.info.adventureTeamLevel));

                    OnAdventureTeamLevelChangedFlag = true;
                }
            }
        }

        private void _OnAdventureTeamRenameRes(MsgDATA data)
        {
            if (data == null)
            {
                return;
            }
            var renameRet = new WorldAdventureTeamRenameRes();
            renameRet.decode(data.bytes);
            switch (renameRet.resCode)
            {
                case (uint)ProtoErrorCode.SUCCESS:
                    if (ClientApplication.playerinfo != null && ClientApplication.playerinfo.adventureTeamInfo != null)
                    {
                        ClientApplication.playerinfo.adventureTeamInfo.adventureTeamName = renameRet.newName;
                        SystemNotifyManager.SysNotifyFloatingEffect(tr_rename_content_success);
                    }
                    TitleComponent.OnChangeAdventTeamName(0, renameRet.newName);

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamRenameSucc);

                    break;
                case (uint)ProtoErrorCode.ADVENTURE_TEAM_NAME_INVALID:
                    SystemNotifyManager.SysNotifyTextAnimation(tr_rename_content_illegal);
                    break;
                case (uint)ProtoErrorCode.ADVENTURE_TEAM_NAME_SAME:
                    SystemNotifyManager.SysNotifyTextAnimation(tr_rename_content_no_changed);
                    break;
                case (uint)ProtoErrorCode.ADVENTURE_TEAM_NAME_EXIST:
                    SystemNotifyManager.SysNotifyTextAnimation(tr_rename_content_be_used);
                    break;
                case (uint)ProtoErrorCode.ADVENTURE_TEAM_RENAME_FAILED:
                    SystemNotifyManager.SysNotifyTextAnimation(tr_rename_content_failed);
                    break;
                default:
                    break;
            }
        }

        private void _OnExtensibleRoleFieldUnlockRes(MsgDATA data)
        {
            if (data == null)
            {
                return;
            }
            WorldExtensibleRoleFieldUnlockRes res = new WorldExtensibleRoleFieldUnlockRes();
            res.decode(data.bytes);
            if (res.resCode == (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SysNotifyTextAnimation(tr_select_role_extend_succ);
                if (ClientApplication.playerinfo != null)
                {
                    ClientApplication.playerinfo.newUnLockExtendRoleFieldNum++;
                    ClientApplication.playerinfo.unLockedExtendRoleFieldNum++;
                }
            }
            else if (res.resCode == (uint)ProtoErrorCode.ENTERGAME_ROLE_FIELD_REACN_UPPER_LIMIT)
            {
                SystemNotifyManager.SysNotifyTextAnimation(tr_select_role_field_reach_max);
            }
            else if (res.resCode == (uint)ProtoErrorCode.ENTERGAME_ROLE_FIELD_NOT_ALL_USED)
            {
                SystemNotifyManager.SysNotifyTextAnimation(tr_select_role_field_not_use_total);
            }
            else if (res.resCode == (uint)ProtoErrorCode.ENTERGAME_EXTENSIBLE_ROLE_FIELD_UNLOCK_FAILED)
            {
                SystemNotifyManager.SysNotifyTextAnimation(tr_select_role_field_extend_failed);
            }   
        }

        private void _OnBlessCrystalInfoRes(MsgDATA data)
        {
            if (data == null) return;
            WorldBlessCrystalInfoRes res = new WorldBlessCrystalInfoRes();
            res.decode(data.bytes);
            if (res.resCode == (uint)ProtoErrorCode.SUCCESS)
            {
                if (blessCrystalModel != null)
                {
                    if (blessCrystalModel.currOwnCount != res.ownBlessCrystalNum)
                    {
                        blessCrystalModel.currOwnCount = (int)res.ownBlessCrystalNum;

                        _NotifyBlessCrystalCountChanged();
                    }
                    if (blessCrystalModel.currOwnExp != res.ownBlessCrystalExp)
                    {
                        blessCrystalModel.currOwnExp = res.ownBlessCrystalExp;
                    }

                    _DebugDataManagerLoggger("_OnBlessCrystalInfoRes", "OnAdventureTeamBlessShopReqBaseInfoSucc");
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamBlessCrystalInfoRes);
                }
            }
            else
            {

            }
        }

        private void _OnAdventureTeamExtraInfoRes(MsgDATA data)
        {
            if (data == null) return;
            WorldAdventureTeamExtraInfoRes res = new WorldAdventureTeamExtraInfoRes();
            res.decode(data.bytes);
            if (res.resCode == (uint)ProtoErrorCode.SUCCESS)
            {
                if (ClientApplication.playerinfo != null)
                {
                    ClientApplication.playerinfo.adventureTeamInfo = res.extraInfo;
                }
                _DebugDataManagerLoggger("_OnAdventureTeamExtraInfoRes", "OnAdventureTeamExtraInfo Succ");

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamBaseInfoRes);
            }
            else
            {

            }
        }

        /// <summary>
        /// 监听 服务器同步 帐号Count变化 
        /// </summary>
        /// <param name="msg"></param>
        private void _OnWorldAccountCounterNotify(MsgDATA msg)
        {
            WorldAccountCounterNotify msgData = new WorldAccountCounterNotify();
            msgData.decode(msg.bytes);
            if (msgData == null || msgData.accCounterList == null)
            {
                return;
            }
            for (int i = 0; i < msgData.accCounterList.Length; i++)
            {
                var counterType = (AccountCounterType)msgData.accCounterList[i].type;
                var counterNum = msgData.accCounterList[i].num;
                switch (counterType)
                {
                    case AccountCounterType.ACC_COUNTER_BLESS_CRYSTAL:
                        int tempBlessCrystalNum = (int)counterNum;
                        if (blessCrystalModel != null && blessCrystalModel.currOwnCount != tempBlessCrystalNum)
                        {
                            blessCrystalModel.currOwnCount = tempBlessCrystalNum;

                            _NotifyBlessCrystalCountChanged();
                        }
                        break;
                    case AccountCounterType.ACC_COUNTER_BLESS_CRYSTAL_EXP:
                        if (blessCrystalModel != null && blessCrystalModel.currOwnExp != counterNum)
                        {
                            blessCrystalModel.currOwnExp = counterNum;
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamBlessCrystalExpChanged);
                        }
                        break;
                    case AccountCounterType.ACC_COUNTER_BOUNTY:
                        int tempBountyNum = (int)counterNum;
                        if (bountyModel != null && bountyModel.currOwnCount != tempBountyNum)
                        {
                            bountyModel.currOwnCount = tempBountyNum;
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamBountyCountChanged);
                        }
                        break;
                    case AccountCounterType.ACC_COUNTER_INHERIT_BLESS:
                        uint tempInheritBlessNum = (uint)counterNum;
                        if (inheritBlessModel != null && inheritBlessModel.ownInheritBlessNum != tempInheritBlessNum)
                        {
                            int addCount = (int)tempInheritBlessNum - (int)inheritBlessModel.ownInheritBlessNum;
                            isPassBlessOwnCountAddup = addCount > 0 && isPassBlessOwnCountInit ? true : false;
                            inheritBlessModel.ownInheritBlessNum = (uint)counterNum;                            

                            _NotifyPassBlessCountChanged();
                        }
                        isPassBlessOwnCountInit = true;
                        break;
                    case AccountCounterType.ACC_COUNTER_INHERIT_BLESS_EXP:
                        if (inheritExpModel != null && inheritExpModel.ownInheritBlessExp != counterNum)
                        {
                            inheritExpModel.ownInheritBlessExp = counterNum;
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamInheritBlessExpChanged);
                        }
                        break;
                }
            }
        }

        private void _OnQueryOwnJobRes(MsgDATA data)
        {
            if (null == data) return;
            WorldQueryOwnOccupationsRes res = new WorldQueryOwnOccupationsRes();
            res.decode(data.bytes);
            if (res.resCode == (uint)ProtoErrorCode.SUCCESS)
            {
                _RestoreCharacterCollectionModels(res.occus, false);
				UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamCollectionInfoRes);
            }
            else
            { 
            }
        }

        private void _OnSyncOwnNewJobs(MsgDATA data)
        {
            if (null == data) return;
            WorldQueryOwnOccupationsSync res = new WorldQueryOwnOccupationsSync();
            res.decode(data.bytes);
            _RestoreCharacterCollectionModels(res.ownNewOccus, true);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamCollectionInfoRes);
        }

        private void _RestoreCharacterCollectionModels(byte[] resJobIds, bool isResJobIdNew)
        {
            if (mCharacterCollectionDic == null || mTotalCharacterCollection == null)
            {
                return;
            }
            List<CharacterCollectionModel> models = null;
            CharacterCollectionModel model = null;

            if (resJobIds == null) return;
            for (int i = 0; i < resJobIds.Length; i++)
            {
                int resJobId = (int)resJobIds[i];
                int baseJobId = _GetBaseJobTableIdByTransferJobId(resJobId);
                if (baseJobId <= 0)
                    continue;
                if (mCharacterCollectionDic.TryGetValue(baseJobId, out models))
                {
                    if (models == null)
                        continue;                    
                    for (int j = 0; j < models.Count; j++)
                    {
                        model = models[j];
                        if (model == null) continue;
                        if (model.jobTableId == resJobId)
                        {
                            //已拥有
                            model.isOwned = true;

                            //只能在返回新角色中设为有新激活的状态
                            if (isResJobIdNew)
                            {
                                ChangeSelectJobPlayStatus(model, true);
                            }
                            break;
                        }
                    }
                }
                
                //不用再存一份
                //for (int j = 0; j < mTotalCharacterCollection.Count; j++)
                //{
                //    model = mTotalCharacterCollection[j];
                //    if (model == null) continue;
                //    if (model.jobTableId == resJobId)
                //    {
                //        var activateStatus = isResJobIdNew ? CharacterCollectionActivateStatus.NewActivated : CharacterCollectionActivateStatus.Activated;
                //        ChangeSelectJobActivateStatus(model, activateStatus);
                //        break;
                //    }
                //}
            }
        }

        private int _GetBaseJobTableIdByTransferJobId(int transferJobId)
        {
            int baseJobId = 0;

            var jobTable = TableManager.GetInstance().GetTableItem<JobTable>(transferJobId);
            if (jobTable == null)
            {
                return baseJobId;
            }
            return jobTable.prejob;
        }


        private void _OnWorldMallBuyRet(MsgDATA msg)
        {
            if (msg == null)
                return;

            var worldMallBuyRet = new WorldMallBuyRet();
            worldMallBuyRet.decode(msg.bytes);

            //购买不成功
            if (worldMallBuyRet.code != (uint)ProtoErrorCode.SUCCESS)
            {
                //统一在ShopNewDataManager处理
                // SystemNotifyManager.SystemNotify((int)worldMallBuyRet.code);
            }
            else
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamRenameCardBuySucc, new ItemSimpleData() { ItemID = (int)worldMallBuyRet.mallitemid });
            }
        }

        #endregion

        #region Pass Bless
        private void _OnPassBlessInfoRes(MsgDATA data)
        {
            if (data == null) return;
            WorldInheritBlessInfoRes res = new WorldInheritBlessInfoRes();
            res.decode(data.bytes);
            if (res.resCode == (uint)ProtoErrorCode.SUCCESS)
            {
                if (null == inheritBlessModel)
                {
                    inheritBlessModel = new InheritBlessModel();
                }
                if (inheritBlessModel.ownInheritBlessNum != res.ownInheritBlessNum)
                {
                    inheritBlessModel.ownInheritBlessNum = res.ownInheritBlessNum;
                }

                if (null == inheritExpModel)
                {
                    inheritExpModel = new InheritExpModel();
                }
                if (inheritExpModel.ownInheritBlessExp != res.ownInheritBlessExp)
                {
                    inheritExpModel.ownInheritBlessExp = res.ownInheritBlessExp;
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamInheritBlessInfoRes);
            }
            else
            {

            }
        }

        private void _OnUsePassBlessExpRes(MsgDATA data)
        {
            if (data == null) return;
            WorldInheritExpRes res = new WorldInheritExpRes();
            res.decode(data.bytes);
            if (res.resCode == (uint)ProtoErrorCode.SUCCESS)
            {
                if (null != inheritBlessModel && inheritBlessModel.ownInheritBlessNum != res.ownInheritBlessNum)
                {
                    int addCount = (int)res.ownInheritBlessNum - (int)inheritBlessModel.ownInheritBlessNum;
                    isPassBlessOwnCountAddup = addCount > 0 ? true : false;
                    inheritBlessModel.ownInheritBlessNum = res.ownInheritBlessNum;

                    _NotifyPassBlessCountChanged();
                }
                if (null != inheritExpModel && inheritExpModel.ownInheritBlessExp != res.ownInheritBlessExp)
                {
                    inheritExpModel.ownInheritBlessExp = res.ownInheritBlessExp;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamInheritBlessExpChanged);
                }
            }
            else if (res.resCode == (uint)ProtoErrorCode.INHERIT_BLESS_USE_ERROR)
            {

            }
        }

        #endregion

        #region Expedition

        private void _OnExpeditionMapInfoRes(MsgDATA data)
        {
            if (data == null) return;
            WorldQueryExpeditionMapRes res = new WorldQueryExpeditionMapRes();
            res.decode(data.bytes);
            if (res.resCode == (uint)ProtoErrorCode.SUCCESS)
            {
                _RestoreExpeditionMapsNetInfo(res.mapId, res.expeditionStatus, res.durationOfExpedition, res.endTimeOfExpedition, res.members);

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamExpeditionMapInfoChanged, new ExpeditionMapNetInfo() { mapId = res.mapId });
            }
            else
            {

            }
        }

        private void _RestoreExpeditionMapsNetInfo(byte netMapId, byte netExpeditionStatus, uint netDurationOfExpedition, uint netEndTimeOfExpedition, ExpeditionMemberInfo[] netMembers)
        {
            var tempExpeditionMapNetInfo = new ExpeditionMapNetInfo();
            tempExpeditionMapNetInfo.mapId = netMapId;
            tempExpeditionMapNetInfo.mapStatus = (ExpeditionStatus)netExpeditionStatus;
            if (tempExpeditionMapNetInfo.mapStatus == ExpeditionStatus.EXPEDITION_STATUS_PREPARE)
            {
                tempExpeditionMapNetInfo.durationOfExpedition = _GetExpeditionMapIdTime(tempExpeditionMapNetInfo.mapId);
            }
            else
            {
                tempExpeditionMapNetInfo.durationOfExpedition = netDurationOfExpedition;
            }
            tempExpeditionMapNetInfo.endTimeOfExpedition = netEndTimeOfExpedition;

            mIsChangedExpeditionRoles = true;
            if (tempExpeditionMapNetInfo.roles == null)
            {
                tempExpeditionMapNetInfo.roles = new List<ExpeditionMemberInfo>();
            }
            else
            {
                tempExpeditionMapNetInfo.roles.Clear();
            }
            if (netMembers != null && netMembers.Length != 0)
            {
                for (int i = 0; i < netMembers.Length; i++)
                {
                    tempExpeditionMapNetInfo.roles.Add(netMembers[i]);
                }
            }           

            //刷新本地地图数据
            _UpdateLocalExpeditionMapsNetInfo(tempExpeditionMapNetInfo);

            //刷新当前展示地图数据
            if (mExpeditionMapNetInfo != null && tempExpeditionMapNetInfo.mapId == mExpeditionMapNetInfo.mapId)
            {
                if (mExpeditionMapBaseInfo.expeditionMapDic != null &&
                    mExpeditionMapBaseInfo.expeditionMapDic.ContainsKey(tempExpeditionMapNetInfo.mapId))
                {
                    var tempMapModel = mExpeditionMapBaseInfo.expeditionMapDic[tempExpeditionMapNetInfo.mapId];
                    if (tempMapModel != null)
                    {
                        var tempMapNetInfo = tempMapModel.mapNetInfo;
                        if (tempMapNetInfo != null)
                        {
                            if (mExpeditionMapNetInfo.durationOfExpedition != tempMapNetInfo.durationOfExpedition)
                            {
                                mIsChangedExpeditionTime = true;
                            }
                            else
                            {
                                mIsChangedExpeditionTime = false;
                            }
                            _UpdateExpeditionMapNetInfo(mExpeditionMapNetInfo, tempMapNetInfo);
                        }
                    }
                }
            }
        }

        private void _OnExpeditionRolesRes(MsgDATA data)
        {
            if (data == null) return;
            WorldQueryOptionalExpeditionRolesRes res = new WorldQueryOptionalExpeditionRolesRes();
            res.decode(data.bytes);
            if (res.resCode == (uint)ProtoErrorCode.SUCCESS)
            {
                mExpeditionRoles = res.roles;
                mIsChangedExpeditionMap = false;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamExpeditionRolesChanged);
            }
            else
            {

            }
        }

        private void _OnDispatchExpeditionTeamRes(MsgDATA data)
        {
            if (data == null) return;
            WorldDispatchExpeditionTeamRes res = new WorldDispatchExpeditionTeamRes();
            res.decode(data.bytes);
            if (res.resCode == (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(tr_expedition_dispatch_succeed);
                //if (mExpeditionMapNetInfo != null)
                //{
                //    _SetExpeditionMapIdTime(mExpeditionMapNetInfo.mapId, (int)mExpeditionMapNetInfo.durationOfExpedition);
                //}
                _SetExpeditionMapIdTime(res.mapId, (int)res.durationOfExpedition);
                _RestoreExpeditionMapsNetInfo(res.mapId, res.expeditionStatus, res.durationOfExpedition, res.endTimeOfExpedition, res.members);

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamExpeditionDispatch, new ExpeditionMapNetInfo() { mapId = res.mapId });
            }
            else if (res.resCode == 4000012)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(tr_expedition_dispatch_dup);
            }
        }
         
        private void _OnCancelExpeditionRes(MsgDATA data)
        {
            if (data == null) return;
            WorldCancelExpeditionRes res = new WorldCancelExpeditionRes();
            res.decode(data.bytes);
            if (res.resCode == (uint)ProtoErrorCode.SUCCESS)
            {
                _UpdateLocalExpeditionMapsBaseInfo(res.mapId, res.expeditionStatus);   //服务器返回的是 127   - 2019年12月13日

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamExpeddtionCancel, new ExpeditionMapNetInfo() { mapId = res.mapId });
            }
            else
            {

            }
        }

        private void _OnGetExpeditionRewardsRes(MsgDATA data)
        {
            if (data == null) return;
            WorldGetExpeditionRewardsRes res = new WorldGetExpeditionRewardsRes();
            res.decode(data.bytes);
            if (res.resCode == (uint)ProtoErrorCode.SUCCESS)
            {
                _UpdateLocalExpeditionMapsBaseInfo(res.mapId, res.expeditionStatus);

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamExpeditionGetReward, new ExpeditionMapNetInfo() { mapId = res.mapId });
            }
            else
            {

            }
        }

        private void _OnGetAllExpeditionMaps(MsgDATA data)
        {
            if (data == null) return;
            WorldQueryAllExpeditionMapsRes res = new WorldQueryAllExpeditionMapsRes();
            res.decode(data.bytes);
            if(res.resCode == (uint)ProtoErrorCode.SUCCESS)
            {               
                bool hasExpeditionMapOver = false;
                for (int i = 0; i < res.mapBaseInfos.Length; i++)
                {
                    _UpdateLocalExpeditionMapsBaseInfo(res.mapBaseInfos[i]);

                    if (!hasExpeditionMapOver && 
                        res.mapBaseInfos[i] != null && 
                        (ExpeditionStatus)res.mapBaseInfos[i].expeditionStatus == ExpeditionStatus.EXPEDITION_STATUS_OVER)
                    {
                        hasExpeditionMapOver = true;                        
                        
                        mCanGetReward = true;
                        _NotifyExpeditionHasRewardChanged();
                    }
                }
                if (!hasExpeditionMapOver)
                {
                    mCanGetReward = false;
                    _NotifyExpeditionHasRewardChanged();
                }
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamExpeditionMiniMapChanged, res.mapBaseInfos);
            }
            else
            {

            }
            
        }

        private void _OnOnceExpeditionDispatchFinish(MsgDATA data)
        {
            if (data == null) return;
            WorldAllExpeditionMapsSync res = new WorldAllExpeditionMapsSync();
            res.decode(data.bytes);
            if(res.mapBaseInfos != null)
            {
                bool hasExpeditionMapOver = false;
                for (int i = 0; i < res.mapBaseInfos.Length; i++)
                {
                    _UpdateLocalExpeditionMapsBaseInfo(res.mapBaseInfos[i]);

                    if (!hasExpeditionMapOver && 
                        res.mapBaseInfos[i] != null &&
                        (ExpeditionStatus)res.mapBaseInfos[i].expeditionStatus == ExpeditionStatus.EXPEDITION_STATUS_OVER)
                    {
                        hasExpeditionMapOver = true;

                        ReqGetAllExpeditionMaps();
                        if (mExpeditionMapNetInfo != null)
                        {
                            if (res.mapBaseInfos[i].mapId == mExpeditionMapNetInfo.mapId)
                            {
                                ReqExpeditionMapInfo(mExpeditionMapNetInfo.mapId);
                            }
                        }

                        mCanGetReward = true;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamExpeditionTimerFinish);
                        _NotifyExpeditionHasRewardChanged();
                    }
                }
                if (!hasExpeditionMapOver)
                {
                    mCanGetReward = false;
                    _NotifyExpeditionHasRewardChanged();
                }
            }
        }


        private void _UpdateLocalExpeditionMapsBaseInfo(Protocol.ExpeditionMapBaseInfo mapBaseInfo)
        {
            if (mapBaseInfo == null)
            {
                return;
            }
            _UpdateLocalExpeditionMapsBaseInfo(mapBaseInfo.mapId, mapBaseInfo.expeditionStatus);
        }

        private void _UpdateLocalExpeditionMapsBaseInfo(byte mapId, byte expeditionStatus)
        {
            if (mExpeditionMapBaseInfo == null || mExpeditionMapBaseInfo.expeditionMapDic == null)
            {
                return;
            }
            var mapBaseInfoDicEnum = mExpeditionMapBaseInfo.expeditionMapDic.GetEnumerator();
            while (mapBaseInfoDicEnum.MoveNext())
            {
                int _mapId = mapBaseInfoDicEnum.Current.Key;
                var mapModel = mapBaseInfoDicEnum.Current.Value as ExpeditionMapModel;

                if (_mapId == mapId && mapModel != null && mapModel.mapNetInfo != null)
                {
                    mapModel.mapNetInfo.mapStatus = (ExpeditionStatus)expeditionStatus;
                    if (mapModel.mapNetInfo.mapStatus == ExpeditionStatus.EXPEDITION_STATUS_IN ||
                        mapModel.mapNetInfo.mapStatus == ExpeditionStatus.EXPEDITION_STATUS_OVER) //远征中的地图 不清空
                        continue;
                    if (mapModel.mapNetInfo.roles != null && mapModel.mapNetInfo.roles.Count > 0)
                    {
                        for (int i = 0; i < mapModel.mapNetInfo.roles.Count; i++)
                        {
                            var role = mapModel.mapNetInfo.roles[i];
                            role.expeditionMapId = 0;
                        }

                        _ResetSelectedExpedtionRoleInfos(mapModel.mapNetInfo.roles);
                        mapModel.mapNetInfo.roles.Clear();
                    }                    
                }

            }
        }

        private void _UpdateLocalExpeditionMapsNetInfo(ExpeditionMapNetInfo mapNetInfo)
        {
            if (mapNetInfo == null)
            {
                return;
            }
            if (mExpeditionMapBaseInfo == null || mExpeditionMapBaseInfo.expeditionMapDic == null)
            {
                return;
            }
            var mapBaseInfoDicEnum = mExpeditionMapBaseInfo.expeditionMapDic.GetEnumerator();
            while (mapBaseInfoDicEnum.MoveNext())
            {
                int mapId = mapBaseInfoDicEnum.Current.Key;
                var mapModel = mapBaseInfoDicEnum.Current.Value as ExpeditionMapModel;

                if (mapId == mapNetInfo.mapId && mapModel != null && mapModel.mapNetInfo != null)
                {
                    _UpdateExpeditionMapNetInfo(mapModel.mapNetInfo, mapNetInfo);
                }
            }
        }

        private void _UpdateExpeditionMapNetInfo(ExpeditionMapNetInfo oldNetInfo, ExpeditionMapNetInfo newNetInfo)
        {
            if (oldNetInfo == null || newNetInfo == null)
            {
                return;
            }
            if (oldNetInfo.mapId != newNetInfo.mapId)
            {
                return;
            }
            oldNetInfo.mapStatus = newNetInfo.mapStatus;
            oldNetInfo.durationOfExpedition = newNetInfo.durationOfExpedition;
            oldNetInfo.endTimeOfExpedition = newNetInfo.endTimeOfExpedition;
            if (oldNetInfo.roles == null)
            {
                oldNetInfo.roles = new List<ExpeditionMemberInfo>();
            }
            else
            {
                oldNetInfo.roles.Clear();
            }
            if (newNetInfo.roles != null && newNetInfo.roles.Count > 0)
            {
                for (int i = 0; i < newNetInfo.roles.Count; i++)
                {
                    oldNetInfo.roles.Add(newNetInfo.roles[i]);
                }
            }
        }

        #endregion

        #region WeeklyTask

        bool _IsAdventureteamWeeklyMission(SingleMissionInfo value)
        {
            if (value == null || value.missionItem == null)
            {
                return false;
            }
            if (value.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TASK_ADVENTURE_TEAM_ACCOUNT_WEEKLY)
            {
                return true;
            }

            return false;
        }

        void OnAddAventureTeamWeeklyMission(UInt32 taskID)
        {
            if (!_IsAdventureteamWeeklyMission(MissionManager.GetInstance().GetMissionInfo(taskID)))
            {
                return;
            }
            m_ADTMissionList.Add(MissionManager.GetInstance().GetMissionInfo(taskID));
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamWeeklyTaskChange);
            UpdateWeeklyTaskRedPoint();
        }

        void OnUpdateAventureTeamWeeklyMission(UInt32 taskID)
        {
            if (!_IsAdventureteamWeeklyMission(MissionManager.GetInstance().GetMissionInfo(taskID)))
            {
                return;
            }
            if (m_ADTMissionList != null)
            {
                for (int i = 0; i < m_ADTMissionList.Count; ++i)
                {
                    if (m_ADTMissionList[i].taskID == taskID)
                    {
                        m_ADTMissionList[i] = MissionManager.GetInstance().GetMissionInfo(taskID);
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamWeeklyTaskChange);
                        UpdateWeeklyTaskRedPoint();
                        break;
                    }
                }
            }
        }

        void OnDeleteAventureTeamWeeklyMission(UInt32 taskID)
        {
            if (m_ADTMissionList != null)
            {
                for(int i= 0; i < m_ADTMissionList.Count; ++i)
                {
                    if(m_ADTMissionList[i].taskID == taskID)
                    {
                        m_ADTMissionList.RemoveAt(i);
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamWeeklyTaskChange);
                        UpdateWeeklyTaskRedPoint();
                        break;
                    }
                }
            }
            
        }

        void OnMissionListChanged()
        {
            _GetAdventureTeamMissions(ref m_ADTMissionList);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamWeeklyTaskChange);
            UpdateWeeklyTaskRedPoint();
        }

        private void _InitWeeklyTaskModel()
        {
            var systemValue = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_ADVENTURETEAM_WEEKLYMISSION_MAXNUM);

            if (systemValue != null)
            {
                m_ADTMissionFinishMaxNum = systemValue.Value;
            }

            _GetAdventureTeamMissions(ref m_ADTMissionList);
            UpdateWeeklyTaskRedPoint();

            MissionManager.GetInstance().missionChangedDelegate += OnMissionListChanged;
            MissionManager.GetInstance().onAddNewMission += OnAddAventureTeamWeeklyMission;
            MissionManager.GetInstance().onUpdateMission += OnUpdateAventureTeamWeeklyMission;
            MissionManager.GetInstance().onDeleteMission += OnDeleteAventureTeamWeeklyMission;

            hasWeeklyTaskCheckedToday = _IsWeeklyTaskCheckedToday();
        }

        private void _UnInitWeeklyTaskModel()
        {
            MissionManager.GetInstance().onDeleteMission -= OnDeleteAventureTeamWeeklyMission;
            MissionManager.GetInstance().onUpdateMission -= OnUpdateAventureTeamWeeklyMission;
            MissionManager.GetInstance().onAddNewMission -= OnAddAventureTeamWeeklyMission;
            MissionManager.GetInstance().missionChangedDelegate -= OnMissionListChanged;

            m_ADTMissionList.Clear();

            hasWeeklyTaskReceived = false;
        }

        void UpdateWeeklyTaskRedPoint()
        {
            int finishedNum = _GetFinishedWeeklyTaskNum();
            for (int i = 0; i < m_ADTMissionList.Count; ++i)
            {
                if(m_ADTMissionList[i].status == (int)Protocol.TaskStatus.TASK_FINISHED)
                {
                    if (finishedNum < ADTMissionFinishMaxNum) 
                    {
                        if (!hasWeeklyTaskReceived)
                        {
                            hasWeeklyTaskReceived = true;
                            _NotifyWeeklyTaskStatusChanged();
                        }                       
                        return;
                    }
                }
            }
            if (hasWeeklyTaskReceived)
            {               
                hasWeeklyTaskReceived = false;
                _NotifyWeeklyTaskStatusChanged();
            }
        }
        #endregion

        private void _OnServerSwitchFunc(ServerSceneFuncSwitch funcSwitch)
        {
            //bFuncOpened = funcSwitch.sIsOpen && !IsAdventureTeamNameEmpty();   
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamFuncChanged);
        }

        private bool IsAdventureTeamNameEmpty()
        {
            if(ClientApplication.playerinfo == null)
                return true;
            if(ClientApplication.playerinfo.adventureTeamInfo == null)
                return true;
            if(string.IsNullOrEmpty(ClientApplication.playerinfo.adventureTeamInfo.adventureTeamName))
                return true;

            return false;
        }

#endregion

       
#endregion

#region  PUBLIC METHODS

        #region Base Info
        public string GetAdventureTeamIncomeDescByLevel(int adventureTeamLevel)
        {
            string incomeDesc = _GetAdventureTeamTableIncomeDescByType(adventureTeamLevel); ;
            return incomeDesc;
        }

        public int GetAdventureTeamLevel()
        {
            int teamLevel = 1;
            if (ClientApplication.playerinfo != null && ClientApplication.playerinfo.adventureTeamInfo != null)
            {
                teamLevel = ClientApplication.playerinfo.adventureTeamInfo.adventureTeamLevel;
            }
            return teamLevel;
        }

        public string GetAdventureTeamName()
        {
            string teamName = PlayerBaseData.GetInstance().Name;
            if (ClientApplication.playerinfo != null && ClientApplication.playerinfo.adventureTeamInfo != null)
            {
                teamName = ClientApplication.playerinfo.adventureTeamInfo.adventureTeamName;
            }
            return teamName;
        }

        public string GetColorAdventureTeamName()
        {
            string name = GetAdventureTeamName();
            string grade = GetAdventureTeamGrade();
            return ChangeColorByGrade(name, grade);
        }

        public int GetAdventureTeamCurrExp()
        {
            int teamExp = 0;
            if (ClientApplication.playerinfo != null && ClientApplication.playerinfo.adventureTeamInfo != null)
            {
                teamExp = (int)ClientApplication.playerinfo.adventureTeamInfo.adventureTeamExp;
            }
            return teamExp;
        }

        public ulong GetAdventureTeamUpLevelExp()
        {
            ulong thisUpLevelExp = 0;
            int currLevel = GetAdventureTeamLevel();
            if (currLevel >= adventureTeamLevelMaximun)
            {
                return 0; //为了显示满级
            }
            if (mAdventureTeamUpLevelExpDic != null && mAdventureTeamUpLevelExpDic.ContainsKey(currLevel))
            {
                thisUpLevelExp = mAdventureTeamUpLevelExpDic[currLevel];
            }
            return thisUpLevelExp;
        }

        public KeyValuePair<ulong, ulong> GetAdventureTeamCurrExpWithUpLevelExp(ulong currExp)
        {
            return new KeyValuePair<ulong, ulong>(currExp, GetAdventureTeamUpLevelExp());
        }

        public KeyValuePair<ulong, ulong> GetBlessCrystalShopCurrExpWithMaxExp(ulong currExp)
        {
            ulong maxExp = currExp;
            if (blessCrystalModel != null)
            {
                maxExp = (ulong)blessCrystalModel.currExpMaximum;
            }
            return new KeyValuePair<ulong, ulong>(currExp, maxExp);
        }

        public ulong GetAdventureTeamBlessCrystalCount()
        {
            if (blessCrystalModel != null)
            {
                return (uint)blessCrystalModel.currOwnCount;
            }
            return 0;
        }

        public ulong GetAdventureTeamBountyCount()
        {
            if (bountyModel != null)
            {
                return (uint)bountyModel.currOwnCount;
            }
            return 0;
        }

        public ulong GetAdventureTeamPassBlessCount()
        {
            if (inheritBlessModel != null)
            {
                return inheritBlessModel.ownInheritBlessNum;
            }
            return 0;
        }

        public ulong GetAdventureTeamPassBlessExp()
        {
            if (inheritExpModel != null)
            {
                return inheritExpModel.ownInheritBlessExp;
            }
            return 0;
        }

        public ulong GetAdventureTeamPassBlessUnitExp()
        {
            if (inheritExpModel != null)
            {
                return inheritExpModel.inheritBlessUnitExp;
            }
            return 0;
        }

        /// <summary>
        /// 获取评级 描述
        /// </summary>
        /// <returns></returns>
        public string GetAdventureTeamGrade()
        {
            if (ClientApplication.playerinfo != null && ClientApplication.playerinfo.adventureTeamInfo != null)
            {
                return ClientApplication.playerinfo.adventureTeamInfo.adventureTeamGrade;
            }
            return "";
        }

        public string GetColorAdventureTeamGrade()
        {
            string grade = GetAdventureTeamGrade();
            return ChangeColorByGrade(grade, grade);
        }

        public int GetAdventureTeamGradeTableId()
        {
            string gradeStr = GetAdventureTeamGrade();
            AdventureTeamGradeTable.eGradeEnum gradeEnum = GetAdventureTeamGradeEnum(gradeStr);
            return (int)gradeEnum;
        }

        /// <summary>
        /// 获取 排行榜 
        /// </summary>
        /// <returns> 0 为未上榜 </returns>
        public uint GetAdventureTeamRank()
        {
            if (ClientApplication.playerinfo != null && ClientApplication.playerinfo.adventureTeamInfo != null)
            {
                return ClientApplication.playerinfo.adventureTeamInfo.adventureTeamRanking;
            }
            return 0;
        }

        /// <summary>
        /// 获取 总评分
        /// </summary>
        /// <returns></returns>
        public uint GetAdventureTeamScore()
        {
            if (ClientApplication.playerinfo != null && ClientApplication.playerinfo.adventureTeamInfo != null)
            {
                return ClientApplication.playerinfo.adventureTeamInfo.adventureTeamRoleTotalScore;
            }
            return 0;
        }

        public int GetAdventureTeamTitleTableIdByRanking(int ranking)
        {
            if (ranking <= 0)
            {
                return 0;
            }
            var table = TableManager.GetInstance().GetTable<AdventureTeamTitleTypeTable>();
            if (table == null)
            {
                return 0;
            }

            var enumerator = table.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var tableItem = enumerator.Current.Value as AdventureTeamTitleTypeTable;
                if (tableItem == null)
                    continue;
                if (tableItem.LimitType != AdventureTeamTitleTypeTable.eLimitType.Ranking)
                {
                    continue;
                }
                if (ranking >= tableItem.RankingRangeMin && ranking <= tableItem.RankingRangeMax)
                {
                    return tableItem.TitleTableID;
                }
            }
            return 0;
        }

        /// <summary>
        /// 根据排行榜排名获取头衔资源
        /// </summary>
        /// <param name="newTitleTableId"></param>
        /// <returns></returns>
        public string GetAdventureTeamTitleResPathByRanking(int ranking)
        {
            int titleTableId = GetAdventureTeamTitleTableIdByRanking(ranking);
            if (titleTableId <= 0)
            {
                return "";
            }
            var newTitleTable = TableManager.GetInstance().GetTableItem<NewTitleTable>(titleTableId);
            if (newTitleTable == null)
            {
                return "";
            }
            return newTitleTable.path;
        }

        #endregion

        #region Character Collection

        /// <summary>
        /// 获得对应基础职业的转职职业
        /// </summary>
        /// <param name="baseJobId">小于等于0时 查询全部转职职业</param>
        /// <returns></returns>
        public List<CharacterCollectionModel> GetCharacterCollectionModelsByBaseJobId(int baseJobId)
        {
            if (baseJobId <= 0)
            {
                return mTotalCharacterCollection;
            }

            List<CharacterCollectionModel> models = null;

            if (mCharacterCollectionDic == null)
            {
                return models;
            }

            if (mCharacterCollectionDic.TryGetValue(baseJobId, out models))
            {
                //升序
                models.Sort((x, y) => { return x.jobTableId.CompareTo(y.jobTableId); });
                return models;
            }

            //Test
            if (models != null  && models.Count > 0)
            {
                return new List<CharacterCollectionModel>() { models[0] };
            }

            return models;
        }

        public int[] GetTotalBaseJobTabIds()
        {
            int[] baseJobIds = null;
            if (mBaseJobTableIdWithNameDic == null || mBaseJobTableIdWithNameDic.Keys == null)
            {
                return baseJobIds;
            }
            baseJobIds = new int[mBaseJobTableIdWithNameDic.Keys.Count];
            var enumerator = mBaseJobTableIdWithNameDic.Keys.GetEnumerator();
            int index = 0;
            while (enumerator.MoveNext())
            {
                int baseJobId = enumerator.Current;
                baseJobIds[index] = baseJobId;
                index++;
            }
            return baseJobIds;
        }

        /// <summary>
        /// 获取基础职业的名称 包括"全部"职业 
        /// </summary>
        /// <param name="baseJobId">小于等于0时 查询全部转职职业</param>
        public string[] GetTotalBaseJobNames()
        {
            string[] baseJobNames = null;
            if (mBaseJobTableIdWithNameDic == null || mBaseJobTableIdWithNameDic.Values == null)
            {
                return baseJobNames;
            }
            baseJobNames = new string[mBaseJobTableIdWithNameDic.Values.Count];
            var enumerator = mBaseJobTableIdWithNameDic.Values.GetEnumerator();
            int index = 0;
            while (enumerator.MoveNext())
            {
                string baseJobName = enumerator.Current;
                baseJobNames[index] = baseJobName;
                index++;
            }
            return baseJobNames;
        }

        public bool[] GetTotalSubJobOwnStatus()
        {
            var baseJobIds = GetTotalBaseJobTabIds();
            if (baseJobIds == null || baseJobIds.Length == 0)
            {
                return null;
            }
            if (mCharacterCollectionDic == null || mCharacterCollectionDic.Keys.Count == 0)
            {
                return null;
            }
            bool[] ownStatuses = new bool[baseJobIds.Length];           //含有“全部”这个页签  这个页签需要前置！！！

            for (int i = baseJobIds.Length - 1; i > 0; i--)
            {
                int baseJobId = baseJobIds[i];
                List<CharacterCollectionModel> subJobModels = null;
                if (mCharacterCollectionDic.TryGetValue(baseJobId, out subJobModels))
                {
                    if (subJobModels == null) continue;
                    for (int j = 0; j < subJobModels.Count; j++)
                    {
                        var jobModel = subJobModels[j];
                        if (jobModel == null) continue;
                        if (jobModel.needPlay)
                        {
                            ownStatuses[i] = true;
                            break;
                        }
                    }
                }
            }

            //设置全部页签
            for (int i = ownStatuses.Length - 1; i > 0; i--)
            {
                if (ownStatuses[i])
                {
                    ownStatuses[0] = true;  //第一个为全部页签
                    break;
                }
            }

            return ownStatuses;
        }

        #endregion

        #region Expedition

        /// <summary>
        /// 返回列表有N个相同大职业
        /// </summary>
        /// <param name="occuList">职业id列表</param>
        /// <returns></returns>
        public int IsAnySameBaseOccu(int[] occuList)
        {
            //key jobId
            Dictionary<int, int> tempOccuDic = new Dictionary<int, int>();
            for(int i = 0; i < occuList.Length; i++)
            {
                var jobTableItem = TableManager.GetInstance().GetTableItem<JobTable>(occuList[i]);
                int jobId;
                if(jobTableItem.JobType == 1)
                {
                    jobId = jobTableItem.prejob;
                }
                else
                {
                    jobId = jobTableItem.ID;
                }
                if (tempOccuDic.ContainsKey(jobId))
                {
                    tempOccuDic[jobId]++;
                }
                else
                {
                    tempOccuDic.Add(jobId, 1);
                }
            }
            int rolesNum = 0;
            foreach(int i in tempOccuDic.Values)
            {
                if (i > rolesNum) rolesNum = i;
            }
            return rolesNum;
        }

        private List<ExpeditionMemberInfo> _GetMaxNumSameBaseOccuInfos(List<ExpeditionMemberInfo> roleInfos)
        {
            List<ExpeditionMemberInfo> maxNumSameBaseOccuRoles = new List<ExpeditionMemberInfo>();
            if (roleInfos == null || roleInfos.Count <= 0)
            {
                return maxNumSameBaseOccuRoles;
            }

            //key jobId
            Dictionary<int, List<ExpeditionMemberInfo>> tempOccuRolesDic = new Dictionary<int, List<ExpeditionMemberInfo>>();
            for (int i = 0; i < roleInfos.Count; i++)
            {
                var roleInfo = roleInfos[i];
                if (roleInfo == null)
                    continue;
                var jobTableItem = TableManager.GetInstance().GetTableItem<JobTable>(roleInfo.occu);
                if (jobTableItem == null)
                    continue;
                int jobId;
                if (jobTableItem.JobType == 1)
                {
                    jobId = jobTableItem.prejob;
                }
                else
                {
                    jobId = jobTableItem.ID;
                }
                if (tempOccuRolesDic.ContainsKey(jobId) && tempOccuRolesDic[jobId] != null)
                {
                    tempOccuRolesDic[jobId].Add(roleInfo);
                }
                else
                {
                    tempOccuRolesDic.Add(jobId, new List<ExpeditionMemberInfo>() { roleInfo });
                }
            }
            int rolesNum = 0, maxNumJobIdKey = 0;
            var dicEnum = tempOccuRolesDic.GetEnumerator();
            while (dicEnum.MoveNext())
            {               
                var dicValue = dicEnum.Current.Value as List<ExpeditionMemberInfo>;
                if (dicValue == null)
                    continue;
                if (dicValue.Count > rolesNum)
                {
                    maxNumJobIdKey = dicEnum.Current.Key;
                    rolesNum = dicValue.Count;                   
                }
            }

            if (tempOccuRolesDic.ContainsKey(maxNumJobIdKey))
            {
                maxNumSameBaseOccuRoles = tempOccuRolesDic[maxNumJobIdKey];
            }
            return maxNumSameBaseOccuRoles;
        }

        /// <summary>
        /// 返回列表有N个不同大职业
        /// </summary>
        /// <param name="occuList">职业id列表</param>
        /// <returns></returns>
        public int IsAnyDiffBaseOccu(int[] occuList)
        {
            int rolesNum = 0;
            List<int> tempOccuList = new List<int>();
            for (int i = 0; i < occuList.Length; i++)
            {
                var jobTableItem = TableManager.GetInstance().GetTableItem<JobTable>(occuList[i]);
                int jobId;
                if (jobTableItem.JobType == 1)
                {
                    jobId = jobTableItem.prejob;
                }
                else
                {
                    jobId = jobTableItem.ID;
                }
                if (!tempOccuList.Contains(jobId))
                {
                    tempOccuList.Add(jobId);
                    rolesNum++;
                }
            }
            return rolesNum;
        }

        private List<ExpeditionMemberInfo> _GetMaxNumDiffBaseOccuInfos(List<ExpeditionMemberInfo> roleInfos)
        {
            List<ExpeditionMemberInfo> maxNumDiffBaseOccuRoles = new List<ExpeditionMemberInfo>();
            List<int> tempOccuList = GamePool.ListPool<int>.Get();
            if (roleInfos == null || roleInfos.Count <= 0)
            {
                return maxNumDiffBaseOccuRoles;
            }

            for (int i = 0; i < roleInfos.Count; i++)
            {
                var roleInfo = roleInfos[i];
                if (roleInfo == null)
                    continue;
                var jobTableItem = TableManager.GetInstance().GetTableItem<JobTable>(roleInfo.occu);
                if (jobTableItem == null)
                    continue;
                int jobId;
                if (jobTableItem.JobType == 1)
                {
                    jobId = jobTableItem.prejob;
                }
                else
                {
                    jobId = jobTableItem.ID;
                }
                if (!tempOccuList.Contains(jobId))
                {
                    tempOccuList.Add(jobId);
                    maxNumDiffBaseOccuRoles.Add(roleInfo);
                }
            }
            GamePool.ListPool<int>.Release(tempOccuList);
            return maxNumDiffBaseOccuRoles;
        }

        /// <summary>
        /// 返回列表有N个不同的小职业
        /// </summary>
        /// <param name="occuList"></param>
        /// <returns></returns>
        public int IsAnyDiffChangedOccu(int[] occuList)
        {
            int rolesNum = 0;
            List<int> tempOccuList = new List<int>();
            for (int i = 0; i < occuList.Length; i++)
            {
                var jobTableItem = TableManager.GetInstance().GetTableItem<JobTable>(occuList[i]);
                if (jobTableItem.JobType == 0) continue;
                if (!tempOccuList.Contains(occuList[i])) 
                {
                    tempOccuList.Add(occuList[i]);
                    rolesNum++;
                }
            }
            return rolesNum;
        }

        private List<ExpeditionMemberInfo> _GetMaxNumDiffChangedOccuInfos(List<ExpeditionMemberInfo> roleInfos)
        {
            List<ExpeditionMemberInfo> maxNumDiffChangedOccuRoles = new List<ExpeditionMemberInfo>();
            List<int> tempOccuList = GamePool.ListPool<int>.Get();
            if (roleInfos == null || roleInfos.Count <= 0)
            {
                return maxNumDiffChangedOccuRoles;
            }

            for (int i = 0; i < roleInfos.Count; i++)
            {
                var roleInfo = roleInfos[i];
                if (roleInfo == null)
                    continue;
                var jobTableItem = TableManager.GetInstance().GetTableItem<JobTable>(roleInfo.occu);
                if (jobTableItem == null)
                    continue;
                if (jobTableItem.JobType == 0) continue;
                if (!tempOccuList.Contains(roleInfo.occu))
                {
                    tempOccuList.Add(roleInfo.occu);
                    maxNumDiffChangedOccuRoles.Add(roleInfo);
                }
            }
            GamePool.ListPool<int>.Release(tempOccuList);
            return maxNumDiffChangedOccuRoles;
        }

        public bool IsExpeditionMapEnable()
        {
            if (mExpeditionMapNetInfo == null)
            {
                return false;
            }
            if(mExpeditionMapNetInfo.mapStatus == ExpeditionStatus.EXPEDITION_STATUS_PREPARE)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取远征队成员人数
        /// </summary>
        public int GetExpeditionRolesNum()
        {
            int total = 0;
            for(int i = 0; i < ExpeditionMapNetInfo.roles.Count; i++)
            {
                if (ExpeditionMapNetInfo.roles[i] != null) total++;
            }
            return total;
        }

        /// <summary>
        /// 返回整理排序后的远征角色数值
        /// </summary>
        /// <returns></returns>
        public List<ExpeditionMemberInfo>[] GetExpeditionRolesList()
        {
            if (mExpeditionMapNetInfo == null)
            {
                return new List<ExpeditionMemberInfo>[0];
            }
            List<ExpeditionMemberInfo> enableRoles = new List<ExpeditionMemberInfo>();
            List<ExpeditionMemberInfo> lowLevelRoles = new List<ExpeditionMemberInfo>();
            List<ExpeditionMemberInfo> unenableRoles = new List<ExpeditionMemberInfo>();
            if (mExpeditionRoles == null) return null;
            for(int i = 0; i < mExpeditionRoles.Length; i++)
            {
                bool isSame = false;
                for(int j = 0; j < mExpeditionMapNetInfo.roles.Count; j++)
                {
                    if ( mExpeditionMapNetInfo.roles[j] != null && mExpeditionMapNetInfo.roles[j].roleid == mExpeditionRoles[i].roleid) 
                    {
                        isSame = true;
                        break;
                    }
                }
                if (isSame)
                {
                    continue;
                }
                else if (mExpeditionRoles[i].expeditionMapId != 0)
                {
                    unenableRoles.Add(mExpeditionRoles[i]);
                    continue;
                }
                else if (mExpeditionRoles[i].level <
                    mExpeditionMapBaseInfo.expeditionMapDic[mExpeditionMapNetInfo.mapId].playerLevelLimit)
                {
                    lowLevelRoles.Add(mExpeditionRoles[i]);
                }
                else
                {
                    enableRoles.Add(mExpeditionRoles[i]);
                }
            }
            return new List<ExpeditionMemberInfo>[] { enableRoles, lowLevelRoles, unenableRoles };
        }

        public bool RemoveExpeditionRole(ExpeditionMemberInfo info)
        {
            if (mExpeditionMapNetInfo == null)
            {
                return false;
            }
            for(int i = 0; i < mExpeditionMapNetInfo.roles.Count; i++)
            {
                if (mExpeditionMapNetInfo.roles[i] != null && mExpeditionMapNetInfo.roles[i].roleid == info.roleid) 
                {
                    mExpeditionMapNetInfo.roles.RemoveAt(i);
                    mIsChangedExpeditionRoles = true;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamExpeditionRolesSelected);
                    return true;
                }
            }
            return false;
        }

        public bool AddExpeditionRole(ExpeditionMemberInfo info)
        {
            if (mExpeditionMapNetInfo == null)
            {
                return false;
            }
            int capacity = mExpeditionMapBaseInfo.expeditionMapDic[mExpeditionMapNetInfo.mapId].rolesCapacity;
            if (mExpeditionMapNetInfo.roles.Count == capacity)
            {
                SystemNotifyManager.SysNotifyTextAnimation(string.Format(tr_expedition_dispatch_fail, capacity));
                return false;
            }
            for(int i = 0; i < mExpeditionMapNetInfo.roles.Count; i++)
            {
                if (mExpeditionMapNetInfo.roles[i] != null && mExpeditionMapNetInfo.roles[i].roleid == info.roleid) 
                {
                    return false;
                }
            }

            mExpeditionMapNetInfo.roles.Add(info);
            mIsChangedExpeditionRoles = true;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamExpeditionRolesSelected);
            return true;
        }

        public bool IsRolesInExpeditionList(ExpeditionMemberInfo info)
        {
            if (mExpeditionMapNetInfo == null)
            {
                return false;
            }
            for (int i = 0; i < mExpeditionMapNetInfo.roles.Count; i++)
            {
                if (mExpeditionMapNetInfo.roles[i] != null && mExpeditionMapNetInfo.roles[i].roleid == info.roleid)
                {
                    return true;
                }
            }
            return false;
        }

        public byte[] GetAllExpeditionMapsId()
        {
            List<byte> tempMapsId = new List<byte>();
            foreach (int key in mExpeditionMapBaseInfo.expeditionMapDic.Keys)
            {
                tempMapsId.Add(BitConverter.GetBytes(key)[0]);
            }
            return tempMapsId.ToArray();
        }

        public void SetEpxeditionTime(byte time, bool useOnekey = false)
        {
            if (useOnekey)
            {
                if (mExpeditionMapBaseInfo != null && mExpeditionMapBaseInfo.expeditionMapDic != null)
                {
                    var mapDicEnum = mExpeditionMapBaseInfo.expeditionMapDic.GetEnumerator();
                    while (mapDicEnum.MoveNext())
                    {
                        var mapModel = mapDicEnum.Current.Value as ExpeditionMapModel;
                        if (mapModel == null || mapModel.mapNetInfo == null)
                            continue;
                        if (mapModel.playerLevelLimit <= PlayerMaxLevel &&
                            mapModel.mapNetInfo.mapStatus == ExpeditionStatus.EXPEDITION_STATUS_PREPARE &&
                            mapModel.mapNetInfo.roles != null && mapModel.mapNetInfo.roles.Count > 0)
                        {
                            mapModel.mapNetInfo.durationOfExpedition = time;
                            if (mExpeditionMapNetInfo != null && mapModel.mapNetInfo.mapId == mExpeditionMapNetInfo.mapId)
                            {
                                _SetEpxeditionTime(time);
                            }                            
                        }
                    }
                }
            }
            else
            {
                _SetEpxeditionTime(time);
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamExpeditionTimeChanged);
        }

        private void _SetEpxeditionTime(byte time)
        {
            if (mExpeditionMapNetInfo == null)
            {
                return;
            }
            if (mExpeditionMapNetInfo.durationOfExpedition == time)
            {
                return;
            }
            mExpeditionMapNetInfo.durationOfExpedition = time;
            mIsChangedExpeditionTime = true;            
        }

        public void SetExpeditionMapId(byte id)
        {
            if(id > 0)
            {
                if (mExpeditionMapNetInfo != null)
                {
                    mExpeditionMapNetInfo.mapId = id;
                }
                mIsChangedExpeditionMap = true;
            }
        }

        public int ExpeditionRoleListCount()
        {
            int count = 0;
            //难道网速太慢了没收到协议就先点按钮引发bug？
            if(mExpeditionMapNetInfo == null || mExpeditionMapNetInfo.roles == null)
            {
                return count;
            }
            for(int i = 0; i < mExpeditionMapNetInfo.roles.Count; i++)
            {
                if(mExpeditionMapNetInfo.roles[i] != null)
                {
                    count++;
                }
            }
            return count;
        }

        public string TryGetExpeditionMapRewardConition( int flag)
        {
            string condition;
            tr_expedition_requires.TryGetValue(flag, out condition);
            if(condition == null)
            {
                condition = "";
            }
            return condition;
        }


        /// <summary>
        /// 尝试打开远征选角界面
        /// </summary>
        public void TryOpenExpeditionRoleSelectFrame(ExpeditionMemberInfo tempRoleInfo)
        {
            if (IsExpeditionMapEnable())
            {
                if (tempRoleInfo == null)
                {
                    if (IsChangedExpeditionMap)
                    {
                        //发送协议获取远征角色
                        ReqExpeditionRolesInfo();
                        return;
                    }
                    //直接打开选择角色界面
                    if (!ClientSystemManager.GetInstance().IsFrameOpen<AdventureTeamExpeditionCharacterSelectFrame>())
                    {
                        ClientSystemManager.GetInstance().OpenFrame<AdventureTeamExpeditionCharacterSelectFrame>();
                    }
                }
                else
                {
                    RemoveExpeditionRole(tempRoleInfo);
                }
            }
        }

        public List<ExpeditionMapModel> GetFinishedExpeditionMapModels()
        {
            List<ExpeditionMapModel> tempMapModels = new List<ExpeditionMapModel>();
            if (mExpeditionMapBaseInfo == null || mExpeditionMapBaseInfo.expeditionMapDic == null)
            {
                return tempMapModels;
            }
            var mapDicEnum = mExpeditionMapBaseInfo.expeditionMapDic.GetEnumerator();
            while (mapDicEnum.MoveNext())
            {
                var mapModel = mapDicEnum.Current.Value as ExpeditionMapModel;
                if (mapModel == null || mapModel.mapNetInfo == null)
                {
                    continue;
                }
                if (mapModel.playerLevelLimit <= PlayerMaxLevel &&
                    mapModel.mapNetInfo.mapStatus == ExpeditionStatus.EXPEDITION_STATUS_OVER &&
                    mapModel.mapNetInfo.roles != null && mapModel.mapNetInfo.roles.Count > 0)
                {
                    tempMapModels.Add(mapModel);
                }
            }
            return tempMapModels;
        }

        public List<byte> GetExpeditionTimeList(ExpeditionMapModel tempMapModel)
        {
            List<byte> tempTimeList = new List<byte>();
            if (tempMapModel == null || string.IsNullOrEmpty(tempMapModel.expeditionTime))
            {
                return tempTimeList;
            }
            string[] times = tempMapModel.expeditionTime.Split('|');
            if (times == null || times.Length <= 0)
            {
                return tempTimeList;
            }
            for (int i = 0; i < times.Length; i++)
            {
                int temp = 1;
                if (int.TryParse(times[i], out temp))
                {
                    tempTimeList.Add(BitConverter.GetBytes(temp)[0]);
                }
            }
            return tempTimeList;
        }

        public void ClearReadyExpeditionMapModels(List<ExpeditionMapModel> tempMapModels)
        {
            if (tempMapModels == null || tempMapModels.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < tempMapModels.Count; i++)
            {
                var mapModel = tempMapModels[i];
                if (mapModel == null || mapModel.mapNetInfo == null || mapModel.mapNetInfo.roles == null)
                    continue;
                if (mapModel.mapNetInfo.mapStatus == ExpeditionStatus.EXPEDITION_STATUS_IN ||
                        mapModel.mapNetInfo.mapStatus == ExpeditionStatus.EXPEDITION_STATUS_OVER) //远征中的地图 不清空
                    continue;
                for (int j = 0; j < mapModel.mapNetInfo.roles.Count; j++)
                {
                    var role = mapModel.mapNetInfo.roles[j];
                    role.expeditionMapId = 0;
                }

                _ResetSelectedExpedtionRoleInfos(mapModel.mapNetInfo.roles);
                mapModel.mapNetInfo.roles.Clear();
            }            
        }

        /// <summary>
        /// 根据传入远征角色信息 重置角色远征地图信息 清0
        /// </summary>
        /// <param name="selectedRoleInfos"></param>
        private void _ResetSelectedExpedtionRoleInfos(List<ExpeditionMemberInfo> toClearRoleInfos)
        {
            if (mExpeditionRoles == null || mExpeditionRoles.Length <= 0)
            {
                return;
            }
            if (toClearRoleInfos == null || toClearRoleInfos.Count <= 0)
            {
                return;
            }
            ExpeditionMemberInfo tempRoleInfo = null;
            ExpeditionMemberInfo tempSelectedRoleInfo = null;
            for (int i = 0; i < toClearRoleInfos.Count; i++)
            {
                tempSelectedRoleInfo = toClearRoleInfos[i];
                for (int j = 0; j < mExpeditionRoles.Length; j++)
                {
                    tempRoleInfo = mExpeditionRoles[j];
                    if (tempRoleInfo == null)
                        continue;
                    if (tempRoleInfo.roleid == tempSelectedRoleInfo.roleid)
                    {
                        tempRoleInfo.expeditionMapId = 0;
                        break;
                    }
                }
            }
        }

        public List<ExpeditionMapModel> GetReadyExpeditionMapModels()
        {
            List<ExpeditionMapModel> tempMapModels = new List<ExpeditionMapModel>();

            if (mExpeditionMapBaseInfo == null || mExpeditionMapBaseInfo.expeditionMapDic == null)
            {
                return tempMapModels;
            }
            var mapDicEnum = mExpeditionMapBaseInfo.expeditionMapDic.GetEnumerator();
            while (mapDicEnum.MoveNext())
            {
                var mapModel = mapDicEnum.Current.Value as ExpeditionMapModel;
                if (mapModel == null || mapModel.mapNetInfo == null)
                {
                    continue;
                }
                //筛选当前角色可用的地图
                if (mapModel.playerLevelLimit <= PlayerMaxLevel &&
                    mapModel.mapNetInfo.mapStatus == ExpeditionStatus.EXPEDITION_STATUS_PREPARE)
                {
                    tempMapModels.Add(mapModel);
                }
            }
            tempMapModels.Sort(
                (x, y) => -x.playerLevelLimit.CompareTo(y.playerLevelLimit)
                );

            if (tempMapModels.Count <= 0)
            {
                return tempMapModels;
            }
            if (mExpeditionRoles == null || mExpeditionRoles.Length <= 0)
            {
                return tempMapModels;
            }
            for (int i = 0; i < tempMapModels.Count; i++)
            {
                var mapModel = tempMapModels[i];
                if (mapModel == null)
                    continue;
                if (mapModel.rewardList == null || mapModel.rewardList.Count <= 0)
                    continue;
                int maxPlayerLevel = mapModel.playerLevelLimit;
                List<ExpeditionMemberInfo> enableLevelRoles = _GetExpeditionRolesByPlayerLevel(maxPlayerLevel);
                if (enableLevelRoles == null || enableLevelRoles.Count <= 0)
                    continue;
                List<ExpeditionMemberInfo> tempCondRoles = null;
                int finialRoleNum = 0;
                List<ExpeditionMemberInfo> finialRoles = null;
                for (int j = 0; j < mapModel.rewardList.Count; j++)
                {
                    var reward = mapModel.rewardList[j];
                    tempCondRoles = _GetExpeditionRewardCondMemberInfos(enableLevelRoles, reward);
                    _SetFinialExpeditionMemberInfos(ref finialRoleNum, ref finialRoles, tempCondRoles, reward.rolesNum);
                }
                if (finialRoles != null && finialRoles.Count > 0)
                {
                    if (mapModel.mapNetInfo.roles == null)
                    {
                        mapModel.mapNetInfo.roles = new List<ExpeditionMemberInfo>();
                    }
                    else
                    {
                        mapModel.mapNetInfo.roles.Clear();
                    }
                    for (int j = 0; j < finialRoles.Count; j++)
                    {
                        finialRoles[j].expeditionMapId = mapModel.mapNetInfo.mapId;
                        mapModel.mapNetInfo.roles.Add(finialRoles[j]);
                    }
                }
            }

            //移除没有派遣角色的地图  倒序删 ！
            for (int i = tempMapModels.Count - 1;  i >= 0; i--)
            {
                var mapModel = tempMapModels[i];
                if (mapModel == null || mapModel.mapNetInfo == null)
                {
                    tempMapModels.RemoveAt(i);
                    continue;
                }
                if (mapModel.mapNetInfo.roles == null || mapModel.mapNetInfo.roles.Count <= 0)
                {
                    tempMapModels.RemoveAt(i);
                    continue;
                }
            }

            return tempMapModels;
        }

        private List<ExpeditionMemberInfo> _GetExpeditionRewardCondMemberInfos(List<ExpeditionMemberInfo> enableLevelRoles, ExpeditionReward reward)
        {
            if (enableLevelRoles == null || enableLevelRoles.Count <= 0)
            {
                return null;
            }
            List<ExpeditionMemberInfo> tempCondRoles = null;
            if (reward.rewardCondition == ExpeditionRewardCondition.REQUIRE_ANY_OCCU)
            {
                tempCondRoles = enableLevelRoles;
            }
            else if (reward.rewardCondition == ExpeditionRewardCondition.REQUIRE_ANY_SAME_BASE_OCCU)
            {
                tempCondRoles = _GetMaxNumSameBaseOccuInfos(enableLevelRoles);
            }
            else if (reward.rewardCondition == ExpeditionRewardCondition.REQUIRE_ANY_DIFF_BASE_OCCU)
            {
                tempCondRoles = _GetMaxNumDiffBaseOccuInfos(enableLevelRoles);
            }
            else if (reward.rewardCondition == ExpeditionRewardCondition.REQUIRE_ANY_DIFF_CHANGED_OCCU)
            {
                tempCondRoles = _GetMaxNumDiffChangedOccuInfos(enableLevelRoles);
            }
            return tempCondRoles;
        }

        private void _SetFinialExpeditionMemberInfos(ref int finialRoleNum, ref List<ExpeditionMemberInfo> finialRoles, List<ExpeditionMemberInfo> tempCondRoles, int tempCondRoleNum)
        {
            if (tempCondRoles == null || tempCondRoles.Count <= 0)
            {
                return;
            }
            if (tempCondRoleNum <= tempCondRoles.Count)
            {
                if (finialRoleNum < tempCondRoleNum)
                {
                    finialRoleNum = tempCondRoleNum;
                    finialRoles = tempCondRoles.GetRange(0, finialRoleNum);
                }
            }
        }

        private List<ExpeditionMemberInfo> _GetExpeditionRolesByPlayerLevel(int playerLevel)
        {
            List<ExpeditionMemberInfo> enableRoles = new List<ExpeditionMemberInfo>();
            if (mExpeditionRoles == null || mExpeditionRoles.Length <= 0)
            {
                return enableRoles;
            }
            for (int i = 0; i < mExpeditionRoles.Length; i++)
            {
                var role = mExpeditionRoles[i];
                if (role == null)
                {
                    continue;
                }
                if (role.expeditionMapId != 0)
                {
                    continue;
                }
                if (role.level >= playerLevel)
                {
                    enableRoles.Add(mExpeditionRoles[i]);
                }
            }
            return enableRoles;
        }

        public int GetExpeditionRewardItemTotalCount(List<ExpeditionMapModel> tempMapModels, System.Action<string> SetRewardIconHandler = null)
        {
            if (tempMapModels == null || tempMapModels.Count <= 0)
            {
                return 0;
            }
            int totalRewardCount = 0;
            bool bSetIconFlag = false;
            for (int i = 0; i < tempMapModels.Count; i++)
            {
                var model = tempMapModels[i];
                if (model == null)
                    continue;
                if (model.rewardList == null || model.rewardList.Count <= 0)
                {
                    continue;
                }
                List<ExpeditionMemberInfo> tempConditionRoles = null;
                for (int j = 0; j < model.rewardList.Count; j++)
                {
                    var reward = model.rewardList[j];
                    if (model.mapNetInfo == null || model.mapNetInfo.roles == null)
                    {
                        continue;
                    }
                    tempConditionRoles = _GetExpeditionRewardCondMemberInfos(model.mapNetInfo.roles, reward);
                    if (tempConditionRoles == null || tempConditionRoles.Count < reward.rolesNum)               //判断条件漏掉了 奖励的满足条件
                    {
                        continue;
                    }
                    if (!string.IsNullOrEmpty(reward.rewards))
                    {
                        string[] tempRewards = reward.rewards.Split(':');
                        if (tempRewards != null && tempRewards.Length == 2)
                        {
                            int firstItemId, firstItemCount;
                            if (!bSetIconFlag && int.TryParse(tempRewards[0], out firstItemId))
                            {
                                ProtoTable.ItemTable itemTableItem = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(firstItemId);
                                if (itemTableItem != null && SetRewardIconHandler != null)
                                {
                                    SetRewardIconHandler(itemTableItem.Icon);
                                    bSetIconFlag = true;
                                }
                            }
                            if (int.TryParse(tempRewards[1], out firstItemCount))
                            {
                                firstItemCount *= (int)model.mapNetInfo.durationOfExpedition;
                                totalRewardCount += firstItemCount;
                            }
                        }
                    }
                }
            }
            return totalRewardCount;
        }

        public uint GetLastExpeditionMaxLevelMapDurationTime(List<ExpeditionMapModel> tempMapModels)
        {
            uint lastDurationTime = 0;
            if (tempMapModels == null || tempMapModels.Count <= 0)
            {
                return lastDurationTime;
            }
            int tempMapModelMaxLevel = 0;
            ExpeditionMapModel maxLevelMapModel = null;
            for (int i = 0; i < tempMapModels.Count; i++)
            {
                var model = tempMapModels[i];
                if (model == null)
                    continue;
                if (tempMapModelMaxLevel < model.playerLevelLimit)
                {
                    tempMapModelMaxLevel = model.playerLevelLimit;
                    maxLevelMapModel = model;
                }
            }
            if (maxLevelMapModel != null && maxLevelMapModel.mapNetInfo != null)
            {
                lastDurationTime = _GetExpeditionMapIdTime((int)maxLevelMapModel.mapNetInfo.mapId);
            }
            return lastDurationTime;
        }

        public uint GetLastExpeditionMaxMapDurationTime(List<ExpeditionMapModel> tempMapModels)
        {
            uint lastDurationTime = 0;
            if (tempMapModels == null || tempMapModels.Count <= 0)
            {
                return lastDurationTime;
            }
            uint tempMapModelMaxDuration = 0;
            ExpeditionMapModel maxDurationMapModel = null;
            for (int i = 0; i < tempMapModels.Count; i++)
            {
                var model = tempMapModels[i];
                if (model == null || model.mapNetInfo == null)
                    continue;
                if (tempMapModelMaxDuration < model.mapNetInfo.durationOfExpedition)
                {
                    tempMapModelMaxDuration = model.mapNetInfo.durationOfExpedition;
                    maxDurationMapModel = model;
                }
            }
            if (maxDurationMapModel != null && maxDurationMapModel.mapNetInfo != null)
            {
                lastDurationTime = _GetExpeditionMapIdTime((int)maxDurationMapModel.mapNetInfo.mapId);
            }
            return lastDurationTime;
        }

        #endregion

        #region Pass Bless

        public void ResetUiTempPassBlessModel()
        {
            if (uiTempInheritBlessModel == null)
            {
                uiTempInheritBlessModel = new InheritBlessModel();
            }
            if (inheritBlessModel != null)
            {
                uiTempInheritBlessModel.ownInheritBlessNum = inheritBlessModel.ownInheritBlessNum;
                uiTempInheritBlessModel.inheritBlessMaxNum = inheritBlessModel.inheritBlessMaxNum;
            }
            if (uiTempInheritExpModel == null)
            {
                uiTempInheritExpModel = new InheritExpModel();
            }
            if (inheritExpModel != null)
            {
                uiTempInheritExpModel.ownInheritBlessExp = inheritExpModel.ownInheritBlessExp;
                uiTempInheritExpModel.inheritBlessMaxExp = inheritExpModel.inheritBlessMaxExp;
                uiTempInheritExpModel.inheritBlessUnitExp = inheritExpModel.inheritBlessUnitExp;
            }
        }

        public int CheckNeedFlyExpTimes()
        {
            int needTime = 0;

            if (uiTempInheritBlessModel == null || inheritBlessModel == null)
            {
                return needTime;
            }

            if (uiTempInheritBlessModel.ownInheritBlessNum >= inheritBlessModel.ownInheritBlessNum)
            {
                //没存满
                return needTime;
            }
            else
            {
                //存满了一个以上
                needTime = (int)(inheritBlessModel.ownInheritBlessNum - uiTempInheritBlessModel.ownInheritBlessNum);
            }
            return needTime;
        }

        public void AddupOneExpTempNum()
        {
            if (uiTempInheritBlessModel == null)
            {
                uiTempInheritBlessModel = new InheritBlessModel();
            }
            uiTempInheritBlessModel.ownInheritBlessNum++;
        }

        #endregion

#region Net Data Req

        #region Base Info
        public void ReqChangeTeamName(AdventureTeamRenameModel model)
        {
            if (model == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(model.newNameStr))
            {
                SystemNotifyManager.SysNotifyTextAnimation(tr_rename_content_empty);
                return;
            }
            if (model.newNameStr.Length > renameLimitCharNum)
            {
                SystemNotifyManager.SysNotifyTextAnimation(tr_rename_content_beyond_max);
                return;
            }
            WorldAdventureTeamRenameReq renameReq = new WorldAdventureTeamRenameReq();
            renameReq.newName = model.newNameStr;
            renameReq.costItemUId = model.renameItemGUID;
            renameReq.costItemDataId = model.renameItemTableId;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, renameReq);
        }

        public void QuickMallBuyAndChangeTeamName(int buyCardNum = 1)
        {
            int mallitemId = 0;
            var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType2.SVT_ADVENTURE_RENAME_CARD_MALLITEM_TABLE_ID);
            if (null == systemValue)
            {
                return;
            }
            mallitemId = systemValue.Value;

            var mallItemTable = TableManager.GetInstance().GetTableItem<MallItemTable>(mallitemId);
            if (null == mallItemTable)
            {
                return;
            }
            mRenameCardTableId = mallItemTable.itemid;

            var quickBuyTable = TableManager.GetInstance().GetTableItem<QuickBuyTable>(mRenameCardTableId);
            if (quickBuyTable == null)
            {
                return;
            }
            ItemData costItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(quickBuyTable.CostItemID);
            if (costItemData != null)
            {
                string mContent = string.Format(tr_rename_quick_buy_ask, costItemData.GetColorName(), quickBuyTable.CostNum);

                int multiple = quickBuyTable.multiple;
                int endTime = 0;
                bool isTimer = false;
                MallItemMultipleIntegralData data = MallNewDataManager.GetInstance().CheckMallItemMultipleIntegral(quickBuyTable.ID);
                if (data != null)
                {
                    multiple += data.multiple;
                    endTime = data.endTime;
                }

                if (endTime > 0)
                {
                    isTimer = (endTime - TimeManager.GetInstance().GetServerTime()) > 0;
                }

                if (multiple > 0)
                {
                    int price = MallNewUtility.GetTicketConvertIntergalNumnber(quickBuyTable.CostNum) * multiple;
                    string str = string.Empty;
                    if (multiple <= 1)
                    {
                        str = TR.Value("mall_fast_buy_intergral_single_multiple_desc", price);
                    }
                    else
                    {
                        str = TR.Value("mall_fast_buy_intergral_many_multiple_desc", price, multiple,string.Empty);
                    }

                    if (isTimer == true)
                    {
                        str = TR.Value("mall_fast_buy_intergral_many_multiple_desc", price, multiple, TR.Value("mall_fast_buy_intergral_many_multiple_remain_time_desc", Function.SetShowTimeDay(endTime)));
                    }

                    mContent += str;
                }

                SystemNotifyManager.SysNotifyMsgBoxOkCancel(
                        mContent,
                        () =>
                        {
                            CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();
                            costInfo.nMoneyID = costItemData.PriceItemID;
                            costInfo.nCount = quickBuyTable.CostNum;
                            CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
                            {
                                if (multiple > 0)
                                {
                                    string content = string.Empty;
                                    //积分商城积分等于上限值
                                    if ((int)PlayerBaseData.GetInstance().IntergralMallTicket == MallNewUtility.GetIntergralMallTicketUpper() &&
                                         MallNewDataManager.GetInstance().bItemMallIntergralMallScoreIsEqual == false)
                                    {
                                        content = TR.Value("mall_buy_intergral_mall_score_equal_upper_value_desc");

                                        MallNewUtility.CommonIntergralMallPopupWindow(content, MallNewUtility.ItemMallIntergralMallScoreIsEqual, () => { ReqMallBuy(mallitemId, buyCardNum); });
                                    }
                                    else
                                    {
                                        int ticketConvertScoreNumber = MallNewUtility.GetTicketConvertIntergalNumnber(quickBuyTable.CostNum) * multiple;

                                        int allIntergralScore = (int)PlayerBaseData.GetInstance().IntergralMallTicket + ticketConvertScoreNumber;

                                        //购买道具后商城积分超出上限值
                                        if (allIntergralScore > MallNewUtility.GetIntergralMallTicketUpper() &&
                                           (int)PlayerBaseData.GetInstance().IntergralMallTicket != MallNewUtility.GetIntergralMallTicketUpper() &&
                                            MallNewDataManager.GetInstance().bItemMallIntergralMallScoreIsExceed == false)
                                        {
                                            content = TR.Value("mall_buy_intergral_mall_score_exceed_upper_value_desc",
                                                               (int)PlayerBaseData.GetInstance().IntergralMallTicket,
                                                               MallNewUtility.GetIntergralMallTicketUpper(),
                                                               MallNewUtility.GetIntergralMallTicketUpper() - (int)PlayerBaseData.GetInstance().IntergralMallTicket);

                                            MallNewUtility.CommonIntergralMallPopupWindow(content, MallNewUtility.ItemMallIntergralMallScoreIsExceed, () => { ReqMallBuy(mallitemId, buyCardNum); });
                                        }
                                        else
                                        {//未超出
                                            ReqMallBuy(mallitemId, buyCardNum);
                                        }
                                    }
                                }
                                else
                                {
                                    ReqMallBuy(mallitemId, buyCardNum);
                                }
                            });
                        }
               );
            }
        }

        public void ReqMallBuy(int mallItemId, int itemNum)
        {
            WorldMallBuy req = new WorldMallBuy();
            req.itemId = (uint)mallItemId;
            req.num = (UInt16)itemNum;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        public void ReqExtendRoleFieldUnlock(ulong costItemGUID, int costItemTableId)
        {
            //不需要判断了
            //if (_CheckHasUnUsedRoleFields())
            //{
            //    SystemNotifyManager.SysNotifyTextAnimation(tr_select_role_field_not_use_total);
            //    return;
            //}

            WorldExtensibleRoleFieldUnlockReq extendReq = new WorldExtensibleRoleFieldUnlockReq();
            extendReq.costItemUId = costItemGUID;
            extendReq.costItemDataId = (uint)costItemTableId;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, extendReq);
        }

        public void ReqBlessCrystalInfo()
        {
            WorldBlessCrystalInfoReq blessReq = new WorldBlessCrystalInfoReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, blessReq);
        }

        public void ReqAdventureTeamExtraInfo()
        {
            WorldAdventureTeamExtraInfoReq extraInfoReq = new WorldAdventureTeamExtraInfoReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, extraInfoReq);
        }

        #endregion

        #region Pass Bless

        /// <summary>
        ///  请求刷新传承祝福数据
        /// </summary>
        /// <param name="isOpenFrameFirstReq">是否是打开界面的第一次请求</param>
        public void ReqPassBlessInfo()
        {
            WorldInheritBlessInfoReq passBlessReq = new WorldInheritBlessInfoReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, passBlessReq);
        }

        public void ReqUsePassBlessExp()
        {
            WorldInheritExpReq passBlessExpReq = new WorldInheritExpReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, passBlessExpReq);
        }

        public bool IsEnableToUsePassBless()
        {
            var playerLevel = PlayerBaseData.GetInstance().Level;
            if (mPassBlessItem == null)
            {
                return false;
            }
            if (playerLevel < mPassBlessItem.NeedLevel ||
                playerLevel > mPassBlessItem.MaxLevel)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Expediton

        /// <summary>
        /// 根据地图id查询地图远征状态
        /// </summary>
        /// <param name="mapId">地图id</param>
        public void ReqExpeditionMapInfo(byte id)
        {
            WorldQueryExpeditionMapReq mapInfoReq = new WorldQueryExpeditionMapReq();
            mapInfoReq.mapId = id;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, mapInfoReq);
        }

        /// <summary>
        /// 查询可远征角色
        /// </summary>
        public void ReqExpeditionRolesInfo()
        {
            WorldQueryOptionalExpeditionRolesReq expeditionRolesInfo = new WorldQueryOptionalExpeditionRolesReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, expeditionRolesInfo);
        }

        /// <summary>
        /// 远征派遣请求
        /// </summary>
        /// <param name="mapId"></param>
        /// <param name="members"></param>
        /// <param name="time"></param>
        public void ReqDispatchExpeditionTeam()
        {
            ReqDispatchExpeditionTeam(mExpeditionMapNetInfo);
        }

        public void ReqDispatchExpeditionTeam(List<ExpeditionMapModel> mapModels)
        {
            if (mapModels != null && mapModels.Count > 0)
            {
                for (int i = 0; i < mapModels.Count; i++)
                {
                    var mapModel = mapModels[i];
                    if (mapModel == null || mapModel.mapNetInfo == null)
                        continue;
                    ReqDispatchExpeditionTeam(mapModel.mapNetInfo);
                }
            }
        }

        public void ReqDispatchExpeditionTeam(ExpeditionMapNetInfo mapNetInfo)
        {
            if (mapNetInfo == null)
            {
                return;
            }
            WorldDispatchExpeditionTeamReq dispatchExpeditionTeamReq = new WorldDispatchExpeditionTeamReq();
            dispatchExpeditionTeamReq.mapId = mapNetInfo.mapId;
            if (mapNetInfo.roles != null && mapNetInfo.roles.Count > 0)
            {
                ulong[] members = new ulong[mapNetInfo.roles.Count];
                for (int i = 0; i < mapNetInfo.roles.Count; i++)
                {
                    members[i] = mapNetInfo.roles[i].roleid;
                }
                dispatchExpeditionTeamReq.members = members;
            }
            dispatchExpeditionTeamReq.housOfduration = mapNetInfo.durationOfExpedition;
            if (mapNetInfo.mapStatus == ExpeditionStatus.EXPEDITION_STATUS_PREPARE)
            {
                mapNetInfo.mapStatus = ExpeditionStatus.EXPEDITION_STATUS_IN;
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, dispatchExpeditionTeamReq);
            }
        }

        /// <summary>
        /// 取消远征请求
        /// </summary>
        /// <param name="mapId"></param>
        public void ReqCancelExpeditionTeam()
        {
            if (ExpeditionMapNetInfo.mapStatus == ExpeditionStatus.EXPEDITION_STATUS_IN)
            {
                WorldCancelExpeditionReq cancelExpeditionTeamReq = new WorldCancelExpeditionReq();
                cancelExpeditionTeamReq.mapId = ExpeditionMapNetInfo.mapId;
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, cancelExpeditionTeamReq);
            }
        }

        /// <summary>
        /// 领取远征奖励
        /// </summary>
        /// <param name="mapId"></param>
        public void ReqGetExpeditionRewards()
        {
            if(ExpeditionMapNetInfo.mapStatus == ExpeditionStatus.EXPEDITION_STATUS_OVER)
            {
                WorldGetExpeditionRewardsReq getExpeditionRewardsReq = new WorldGetExpeditionRewardsReq();
                getExpeditionRewardsReq.mapId = ExpeditionMapNetInfo.mapId;
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, getExpeditionRewardsReq);
            }
        }

        public void ReqGetExpeditionRewards(byte mapId)
        {
            WorldGetExpeditionRewardsReq getExpeditionRewardsReq = new WorldGetExpeditionRewardsReq();
            getExpeditionRewardsReq.mapId = mapId;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, getExpeditionRewardsReq);
        }

        public void ReqGetAllExpeditionMaps()
        {
            WorldQueryAllExpeditionMapsReq getAllExpeditionMapsReq = new WorldQueryAllExpeditionMapsReq();
            getAllExpeditionMapsReq.mapIds = GetAllExpeditionMapsId();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, getAllExpeditionMapsReq);
        }

        public void ReqExpeditionAllMapInfo()
        {
            byte[] mapIds = GetAllExpeditionMapsId();
            if (mapIds != null && mapIds.Length > 0)
            {
                for (int i = 0; i < mapIds.Length; i++)
                {
                    ReqExpeditionMapInfo(mapIds[i]);
                }
            }
        }
#endregion

#region Collection 
        public void ReqOwnJobInfo(int[] baseJobIds)
        {
            if (baseJobIds == null || baseJobIds.Length == 0)
            {
                return;
            }
            WorldQueryOwnOccupationsReq req = new WorldQueryOwnOccupationsReq();
            byte[] reqBaseJobIds = new byte[baseJobIds.Length];
            for (int i = 0; i < baseJobIds.Length; i++)
            {
                reqBaseJobIds[i] = (byte)baseJobIds[i];
            }
            req.baseOccus = reqBaseJobIds;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        public void ReqClearActivatedJob(int[] transferJobIds)
        {
            if (transferJobIds == null || transferJobIds.Length == 0)
            {
                return;
            }
            WorldRemoveUnlockedNewOccupationsReq req = new WorldRemoveUnlockedNewOccupationsReq();
            byte[] reqJobIds = new byte[transferJobIds.Length];
            for (int i = 0; i < transferJobIds.Length; i++)
            {
                reqJobIds[i] = (byte)transferJobIds[i];
            }
            req.newOccus = reqJobIds;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 更改 选择开放的转职职业 的激活状态
        /// 
        /// 注意激活顺序已经限制 不能反向和间隔
        /// 激活过 <- 新激活 <- 未激活 
        /// </summary
        /// <param name="model"></param>
        /// <param name="bActivated"></param>
        public void ChangeSelectJobPlayStatus(CharacterCollectionModel model, bool toPlay)
        {
            if (!CheckIsSelectJobSatisfyConditions(model))
            {
                return;
            }
            model.needPlay = toPlay;

            _NotifyCharacterCollectionChanged();
        }

        /// <summary>
        /// 检查界面用的  准备激活的角色  是否满足一揽子条件 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool CheckIsSelectJobSatisfyConditions(CharacterCollectionModel model)
        {
            bool bSatisfied = false;

            if (model != null && model.isJobOpened && model.isTransfer && model.isOwned)
            {
                bSatisfied = true;
            }

            return bSatisfied;
        }

#endregion

#region WeeklyTask
        public void _GetAdventureTeamMissions(ref List<SingleMissionInfo> list)
        {
            list = MissionManager.GetInstance().taskGroup.Values.ToList();
            list.RemoveAll(x => {
                return !_IsAdventureteamWeeklyMission(x);
            });
        }

        public int _GetFinishedWeeklyTaskNum()
        {
            int total = 0;
            if(ADTMissionList != null)
            {
                for(int i = 0; i < ADTMissionList.Count; ++i)
                {
                    if(ADTMissionList[i].status == (int)Protocol.TaskStatus.TASK_OVER)
                    {
                        total++;
                    }
                }
            }
            return total;
        }
#endregion

#endregion

        public void OpenAdventureTeamInfoFrame(AdventureTeamMainTabType tabType = AdventureTeamMainTabType.BaseInformation)
        {
            AdventureTeamInformationFrame.OpenTabFrame(tabType);
        }

#region Red Point

        public bool HasRedPointShow()
        {
            if (!BFuncOpened)
            {
                return false;
            }

            bool isBaseInfoShow = IsBaseInfoTabRedPointShow();
            bool isCharacterColShow = IsCharacterCollectionTabRedPointShow();
            bool isCharacterExpShow = IsCharacterExpeditionTabRedPointShow();
            bool isPassBlessShow = IsPassingBlessTabRedPointShow();
            bool isWeeklyTaskShow = IsWeeklyTaskTabRedPointShow();

            return isBaseInfoShow || isCharacterColShow || isCharacterExpShow || isPassBlessShow || isWeeklyTaskShow;
        }

        public bool IsBaseInfoTabRedPointShow()
        {
            bool isBlessCrystalOwnEnough = _CheckBlessCrystalIsFull();
            return OnAdventureTeamLevelChangedFlag || isBlessCrystalOwnEnough;
        }

        public bool IsCharacterCollectionTabRedPointShow()
        {
            return _CheckNewCharacterNeedPlay();
        }

        public bool IsCharacterExpeditionTabRedPointShow()
        {
            return mCanGetReward;
        }

        public bool IsPassingBlessTabRedPointShow()
        {
            if (inheritBlessModel != null)
            {
                return _CheckPassBlessIsAvailable();
            }
            return false;
        }

        public bool IsWeeklyTaskTabRedPointShow()
        {
            bool checkWeeklyTaskStatus = _CheckWeeklyTaskCanReceiveWithinWeeklyLimit();
            if (checkWeeklyTaskStatus)
            {
                return true;
            }
            return false;
        }

        private bool _CheckBlessCrystalIsFull()
        {
            bool isBlessCrystalOwnFull = false;
            if (blessCrystalModel != null)
            {
                isBlessCrystalOwnFull = blessCrystalModel.currOwnCount >= blessCrystalModel.currNumMaximum;
            }
            return isBlessCrystalOwnFull;
        }

        private bool _CheckPassBlessIsAvailable()
        {
            uint inheritBlessNum = 0;
            ulong inheritBlessExp = 0;
            ulong inheritBlessUnitExp = 0;
            if (inheritBlessModel != null)
            {
                inheritBlessNum = inheritBlessModel.ownInheritBlessNum;
            }
            if (inheritExpModel != null)
            {
                inheritBlessExp = inheritExpModel.ownInheritBlessExp;
                inheritBlessUnitExp = inheritExpModel.inheritBlessUnitExp;
            }

            if (inheritBlessNum > 0 && isPassBlessOwnCountAddup)
            {
                return true;
            }

            if (IsEnableToUsePassBless())
            {
                //可使用成长药剂的角色，只要成长药剂可使用，出现小红点，本次登陆查看后消失
                if (inheritBlessNum > 0 || inheritBlessExp >= inheritBlessUnitExp)
                {
                    isPassBlessCanUse = true;
                    return onFirstCheckPassBlessFlag;
                }
            }
            else
            {
                //不可使用成长药剂的角色每天登陆，只要成长药剂瓶数≥1瓶，出现小红点，查看后消失，当日不再出现
                if (inheritBlessNum > 0 && !hasPassBlessCheckedToday)
                {
                    isPassBlessEnoughOnFirstLogin = true;
                    return onFirstCheckPassBlessFlag;
                }
            }
            return false;
        }

        private bool _IsPassBlessCheckedToday()
        {
            int localTime = PlayerPrefsManager.GetInstance().GetTypeKeyIntValue(PlayerPrefsManager.PlayerPrefsKeyType.ATPassBlessCheckTime);
            if (localTime > Function.GetRefreshHourTimeStamp(weeklyTaskRefreshHour))
            {
                return true;
            }
            return false;
        }

        private void _SavePassBlessCheckTimestamp()
        {
            int currentTimestamp = Function.GetCurrTimeStamp();
            PlayerPrefsManager.GetInstance().SetTypeKeyIntValue(PlayerPrefsManager.PlayerPrefsKeyType.ATPassBlessCheckTime, currentTimestamp);
        }

        private bool _IsWeeklyTaskCheckedToday()
        {
            int localTime = PlayerPrefsManager.GetInstance().GetTypeKeyIntValue(PlayerPrefsManager.PlayerPrefsKeyType.ATWeeklyTaskCheckTime);
            if (localTime > Function.GetRefreshHourTimeStamp(weeklyTaskRefreshHour))
            {
                return true;
            }
            return false;
        }

        private void _SaveWeeklyTaskCheckTimestamp()
        {
            int currentTimestamp = Function.GetCurrTimeStamp();
            PlayerPrefsManager.GetInstance().SetTypeKeyIntValue(PlayerPrefsManager.PlayerPrefsKeyType.ATWeeklyTaskCheckTime, currentTimestamp);
        }

        private bool _CheckNewCharacterNeedPlay()
        {
            if (mTotalCharacterCollection != null)
            {
                for (int i = 0; i < mTotalCharacterCollection.Count; i++)
                {
                    var model = mTotalCharacterCollection[i];
                    if (model == null) continue;
                    if (model.needPlay)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //有每周任务奖励可领取 或者    每天第一次登录 并且 第一次查看 并且 每周没有剩余完成次数 并且每天只提示一次   或者 每周任务还有剩余完成次数
        private bool _CheckWeeklyTaskCanReceiveWithinWeeklyLimit()
        {
            bool isTodayFirstLogin = AdsPush.LoginPushManager.GetInstance().IsFirstLogin();
            int finishedNum = _GetFinishedWeeklyTaskNum();
            int maxNum = ADTMissionFinishMaxNum;
            bool hasWeeklyTaskRemaingNum = maxNum - finishedNum > 0 ? true : false;

            return hasWeeklyTaskReceived
                || (OnFirstCheckWeeklyTaskFlag && hasWeeklyTaskRemaingNum) 
                || (isTodayFirstLogin && OnFirstCheckWeeklyTaskFlag && !hasWeeklyTaskRemaingNum);
        }

        private void _NotifyLevelUp()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamLevelUp);
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.AdventureTeam);
        }

        private void _NotifyCharacterCollectionChanged()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamCollectionInfoChanged);
            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamNewRoleCollectActive);
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.AdventureTeam);
        }

        private void _NotifyExpeditionHasRewardChanged()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamExpeditionAwardChanged);
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.AdventureTeam);
        }

        private void _NotifyBlessCrystalCountChanged()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamBlessCrystalCountChanged);
            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamBlessMedalFull);
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.AdventureTeam);
        }

        private void _NotifyPassBlessCountChanged()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamInheritBlessCountChanged);

            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamInheritBlessToUse);
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.AdventureTeam);
        }

        private void _NotifyWeeklyTaskStatusChanged()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamWeeklyTaskStatusChanged);
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.AdventureTeam);
        }

#endregion

#endregion

#region PUBLIC STATIC METHOD

        public static string ChangeColorByGrade(string name, string gradeString)
        {
            try
            {
                AdventureTeamGradeTable.eGradeEnum grade = GetAdventureTeamGradeEnum(gradeString);
                if (grade == AdventureTeamGradeTable.eGradeEnum.GradeEnum_None)
                {
                    return name;
                }
                return string.Format(GetAdventureNameColorByGrade(grade), name);
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("[AdventureTeamDataManager] - ChangeColorByGrade error ： {0}", e.ToString());
                return name;
            }
        }

        static string GetAdventureNameColorByGrade(AdventureTeamGradeTable.eGradeEnum grade)
        {
            var item = TableManager.GetInstance().GetTableItem<AdventureTeamGradeTable>((int)grade);
            if (item != null)
            {
                return item.NameColor;
            }
            else
            {
                return "{0}";
            }
        }

        static AdventureTeamGradeTable.eGradeEnum GetAdventureTeamGradeEnum(string gradeStr)
        {
            try
            {
                if (string.IsNullOrEmpty(gradeStr))
                {
                    return AdventureTeamGradeTable.eGradeEnum.GradeEnum_None;
                }
                AdventureTeamGradeTable.eGradeEnum gradeEnum =
                                (AdventureTeamGradeTable.eGradeEnum)Enum.Parse(typeof(AdventureTeamGradeTable.eGradeEnum), gradeStr);
                return gradeEnum;
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("[AdventureTeamDataManager] - GetAdventureTeamGradeEnum error ： {0}", e.ToString());
                return AdventureTeamGradeTable.eGradeEnum.GradeEnum_None;
            }
        }

#endregion

#region  TEST
        private void _DebugDataManagerLoggger(string methodName, string errorLog)
        {
#if UNITY_EDITOR
            //if (Global.Settings.isDebug)
            //{
            //    Logger.LogErrorFormat("[AdventureDataManager] - {0}, error: {1}", methodName, errorLog);
            //}
#endif
        }
#endregion
    }
}
