using System;
using Protocol;
using Network;
using ProtoTable;
using System.Collections.Generic;
using System.Diagnostics;


namespace GameClient
{
    public class TeamDuplicationDataManager : DataManager<TeamDuplicationDataManager>
    {

        //团本相关的音效ID
        //攻坚面板上紧急状态的音效
        public const int TeamDuplicationAudioUrgentStateId = 5064;
        //胜利失败和结束的音效
        public const int TeamDuplicationAudioGameResultId = 5065;

        //攻坚场景中商店ID
        public const int TeamDuplicationShopId = 31;

        //战斗成功和战斗失败默认展示时间
        public readonly int GameResultShowTime = 4;

        //战斗开始倒计时的时间
        public readonly int FightCountDownShowTime = 3;          //战斗开始展示的倒计时：3,2,1
        public readonly int FightCountDownTotalTime = 6;         //界面总共显示时间

        //关卡描述开始和结束的展示时间
        public readonly float TeamDuplicationStageBeginIntervalTime = 2.0f;
        public readonly float TeamDuplicationStageEndIntervalTime = 4.0f;

        public readonly int TeamDuplicationUnLockLevel = 40;			//解锁等级

        public readonly int TeamDuplicationBuildSceneId = 6035;         //团本组队场景ID，对应地图城镇表中的ID
        public readonly int TeamDuplicationFightSceneId = 6036;         //团本攻坚场景ID；

        public readonly int TeamDuplicationCaptainNumber = 4;      //小队的数量
        public readonly int TeamDuplicationPlayerNumberInCaptain = 3;      //每个小队中成员的数量
        public readonly int TeamDuplicationTotalPlayerNumberInTeam = 12;       //团队的总人数

        //战前配置类型
        public TeamCopyPlanModel FightSettingConfigPlanModel = TeamCopyPlanModel.TEAM_COPY_PLAN_MODEL_INVALID;
        private List<TeamDuplicationTeamDifficultyConfigDataModel> _teamConfigDataModelList =
            new List<TeamDuplicationTeamDifficultyConfigDataModel>();

        //重连的场景ID
        public int TeamDuplicationReconnectSceneId = 0;
        public ulong TeamDuplicationReconnectExpireTime = 0;  //重连结束时间

        //战斗开始投票相关
        public int StartBattleVoteIntervalTime { get; private set; }
        public int StartBattleVoteEndTime { get; private set; }
        public bool IsInStartBattleVotingTime { get; private set; }     //是否处在投票阶段
        private List<ulong> _fightStartVoteAgreeList = new List<ulong>();
        public bool FightVoteAgreeBySwitchFightScene = false;       //切换到战斗场景，并同意投票（首先切换到战斗场景，之后同意)

        //战斗结束投票相关
        //战斗结束投票的标志，true表示存在按钮，只有团长有用
        public bool TeamDuplicationFightEndVoteFlag = false;
        //战斗结束投票相关数据
        public int FightEndVoteIntervalTime;
        public int FightEndVoteEndTime;
        public List<ulong> FightEndVoteAgreeList = new List<ulong>();
        public List<ulong> FightEndVoteRefuseList = new List<ulong>();

        //是否为金主,应该由服务器决定
        public bool IsTeamDuplicationGoldOwner = false;

        private TeamDuplicationPlayerInformationDataModel _playerInformationDataModel;    //自己的相关数据

        //团长是否已经展示了提示
        public bool IsAlreadyShowPositionAdjustTip = false;

        //是否拥有团队
        public bool IsTeamDuplicationOwnerTeam { get; private set; }

        //团队的数据结构
        private TeamDuplicationTeamDataModel _teamDataModel = new TeamDuplicationTeamDataModel();
        public TeamDuplicationTeamDataModel OtherTeamDataModel = new TeamDuplicationTeamDataModel();

        //队伍列表的数据, 总列表和当前页的列表
        private List<TeamDuplicationTeamListDataModel> _teamListDataModelList = new List<TeamDuplicationTeamListDataModel>();
        public int TeamListCurrentPage = 1;     //队伍列表当前页数
        public int TeamListTotalPage = 1;       //队伍列表总的页数
        private Dictionary<int, int> _teamRequestJoinInEndTimeDic = new Dictionary<int, int>();

        //队伍请求者的数据列表
        private List<TeamDuplicationRequesterDataModel> _teamRequesterDataModelList =
            new List<TeamDuplicationRequesterDataModel>();
        //团本队友数据列表
        private List<TeamDuplicationRequesterDataModel> _teamFriendDataModelList =
            new List<TeamDuplicationRequesterDataModel>();

        //新的申请者标志
        public bool IsTeamDuplicationOwnerNewRequester = false;
        //新的邀请者标志
        public bool IsTeamDuplicationOwnerNewInvite = false;

        //团本邀请列表数据
        public List<TeamDuplicationTeamInviteDataModel> TeamInviteDataModelList { get; private set; }

        //攻坚面板的数据
        //攻坚阶段Id
        public int TeamDuplicationFightStageId { get; private set; }
        //攻坚结束时间，在开始战斗同步之后获得数据
        public uint TeamDuplicationFightFinishTime { get; private set; }
        //小队目标的数据
        public TeamDuplicationFightGoalDataModel TeamDuplicationCaptainFightGoalDataModel { get; private set; }
        //团本目标的数据
        public TeamDuplicationFightGoalDataModel TeamDuplicationTeamFightGoalDataModel { get; private set; }
        //攻坚据点的数据
        public List<TeamDuplicationFightPointDataModel> TeamDuplicationFightPointDataModelList { get; private set; }

        //阶段翻牌的倒计时间
        public readonly int TeamDuplicationFightStageRewardTime = 3;
        //最终翻牌的强制结束时间
        public readonly uint TeamDuplicationFinalStageRewardTime = 10;
        //阶段翻牌的奖励
        public List<TeamDuplicationFightStageRewardDataModel> TeamDuplicationFightStageRewardDataModelList { get;
            private set;
        }

        //团本是否需要展示失败的结果
        public bool IsNeedShowGameFailResult = false;
        public bool IsAlreadyReceiveFinalReward = false;

        //团本完成的阶段ID;
        public int TeamDuplicationEndStageId = 0;

        //翻牌持续时间
        public const float TeamDuplicationRewardItemActionDuringTime = 0.5f;

        //时间为0，表示正处在在线状态；时间大约0，表示正处在离线状态，到了对应的时间戳，角色将被踢除
        public Dictionary<ulong, ulong> TeamDuplicationPlayerExpireTimeDic = new Dictionary<ulong, ulong>();

        public bool IsEnterTeamDuplicationBuildSceneFromTown = false;           //从城镇通过前往，进入组队场景
        public bool IsAlreadyShowTicketIsNotEnoughTip = false;                  //门票不足的提示

        //据点解锁比例
        public Dictionary<int, int> TeamDuplicationFightPointUnLockRateDictionary = new Dictionary<int, int>();
        //boss据点相关
        public int BossPhase;                       //boss据点的阶段
        public int BossBloodRate;                   //boss血量（0-100）

        //据点能量蓄积状态相关的数据
        public TeamDuplicationFightPointEnergyAccumulationDataModel FightPointEnergyAccumulationRelationDataModel;

        public TeamDuplicationDataManager()
        {
            TeamDuplicationFightStageId = 0;
            TeamDuplicationFightFinishTime = 0;
            TeamDuplicationCaptainFightGoalDataModel = new TeamDuplicationFightGoalDataModel();
            TeamDuplicationTeamFightGoalDataModel = new TeamDuplicationFightGoalDataModel();
            TeamDuplicationFightPointDataModelList = new List<TeamDuplicationFightPointDataModel>();

            TeamDuplicationFightStageRewardDataModelList = new List<TeamDuplicationFightStageRewardDataModel>();

            IsTeamDuplicationOwnerTeam = false;

            StartBattleVoteIntervalTime = 0;
            StartBattleVoteEndTime = 0;
            IsInStartBattleVotingTime = false;
            TeamInviteDataModelList = new List<TeamDuplicationTeamInviteDataModel>();            
        }

        public override void Initialize()
        {
            BindNetEvents();
        }

        private void BindNetEvents()
        {
            //团本同步自己相关数据
            NetProcess.AddMsgHandler(TeamCopyPlayerInfoNotify.MsgID, OnReceiveTeamCopyPlayerInfoNotify);

            //团本同步
            NetProcess.AddMsgHandler(TeamCopyTeamUpdate.MsgID, OnReceiveTeamCopyTeamUpdate);

            //团本重连的同步
            NetProcess.AddMsgHandler(TeamCopyReconnectNotify.MsgID, OnReceiveTeamCopyReconnectNotify);

            //角色掉线的同步
            NetProcess.AddMsgHandler(TeamCopyPlayerExpireNotify.MsgID, OnReceiveTeamCopyPlayerExpireNotify);

            //团本创建
            NetProcess.AddMsgHandler(TeamCopyCreateTeamRes.MsgID, OnReceiveTeamCopyCreateTeamRes);

            //修改团本装备评分
            NetProcess.AddMsgHandler(TeamCopyModifyEquipScoreRes.MsgID, OnReceiveTeamCopyModifyEquipScoreRes);

            //团本数据同步
            NetProcess.AddMsgHandler(TeamCopyTeamDataRes.MsgID, OnReceiveTeamCopyTeamDataRes);
            //团本状态的同步
            NetProcess.AddMsgHandler(TeamCopyTeamStatusNotify.MsgID, OnReceiveTeamCopyTeamStatusNotify);
            //团本小队数据的同步
            NetProcess.AddMsgHandler(TeamCopySquadNotify.MsgID, OnReceiveTeamCopySquadNotify);

            //团本任命和提出
            NetProcess.AddMsgHandler(TeamCopyKickRes.MsgID, OnReceiveTeamCopyKickRes);
            NetProcess.AddMsgHandler(TeamCopyAppointmentRes.MsgID, OnReceiveTeamCopyAppointmentRes);
            NetProcess.AddMsgHandler(TeamCopyChangeSeatRes.MsgID, OnReceiveTeamCopyChangeSeatRes);
            NetProcess.AddMsgHandler(TeamCopyAppointmentNotify.MsgID, OnReceiveTeamCopyAppointmentNotify);

            //团本列表
            NetProcess.AddMsgHandler(TeamCopyTeamListRes.MsgID, OnReceiveTeamCopyTeamListRes);
            //团本详情信息
            NetProcess.AddMsgHandler(TeamCopyTeamDetailRes.MsgID, OnReceiveTeamCopyTeamDetailRes);

            //团本申请
            NetProcess.AddMsgHandler(TeamCopyTeamApplyRes.MsgID, OnReceiveTeamCopyTeamApplyRes);
            //团本邀请者的通知
            NetProcess.AddMsgHandler(TeamCopyApplyNotify.MsgID, OnReceiveTeamCopyApplyNotify);
            //团本申请拒绝
            NetProcess.AddMsgHandler(TeamCopyApplyRefuseNotify.MsgID, OnReceiveTeamCopyApplyRefuseNotify);

            //团本成员加入和离开的广播
            NetProcess.AddMsgHandler(TeamCopyMemberNotify.MsgID, OnReceiveTeamCopyMemberNotify);

            //金主自动入团的协议
            NetProcess.AddMsgHandler(TeamCopyAutoAgreeGoldRes.MsgID, OnReceiveTeamCopyAutoAgreeGoldRes);

            //团本入队申请者列表
            NetProcess.AddMsgHandler(TeamCopyTeamApplyListRes.MsgID, OnReceiveTeamCopyTeamApplyListRes);

            //团本查找好友的列表
            NetProcess.AddMsgHandler(TeamCopyFindTeamMateRes.MsgID, OnReceiveTeamCopyFindTeamMateRes);

            //团本邀请好友的通知
            NetProcess.AddMsgHandler(TeamCopyInviteNotify.MsgID, OnReceiveTeamCopyInviteNotify);

            //团本邀请列表
            NetProcess.AddMsgHandler(TeamCopyInviteListRes.MsgID, OnReceiveTeamCopyInviteListRes);

            //团本邀请选择
            NetProcess.AddMsgHandler(TeamCopyInviteChoiceRes.MsgID, OnReceiveTeamCopyInviteChoiceRes);

            //团本的招募
            NetProcess.AddMsgHandler(TeamCopyRecruitRes.MsgID, OnReceiveTeamRecruitRes);
            //团本超链接加入
            NetProcess.AddMsgHandler(TeamCopyLinkJoinRes.MsgID, OnReceiveTeamCopyLinkJoinRes);

            //团本同意or拒绝
            NetProcess.AddMsgHandler(TeamCopyTeamApplyReplyRes.MsgID, OnReceiveTeamCopyTeamApplyReplyRes);

            //退出团本
            NetProcess.AddMsgHandler(TeamCopyTeamQuitRes.MsgID, OnReceiveTeamCopyTeamQuitRes);

            //团本开战返回
            NetProcess.AddMsgHandler(TeamCopyStartBattleRes.MsgID, OnReceiveTeamCopyStartBattleRes);
            //团本开始投票同步
            NetProcess.AddMsgHandler(TeamCopyStartBattleNotify.MsgID, OnReceiveTeamCopyStartBattleNotify);
            //团本投票同步
            NetProcess.AddMsgHandler(TeamCopyVoteNotify.MsgID, OnReceiveTeamCopyVoteNotify);
            //团本投票结束
            NetProcess.AddMsgHandler(TeamCopyVoteFinish.MsgID, OnReceiveTeamCopyVoteFinish);

            //团本战斗结束投票的标志
            NetProcess.AddMsgHandler(TeamCopyForceEndFlag.MsgID, OnReceiveTeamCopyForceEndFlag);
            //团本战斗结束投票的返回
            NetProcess.AddMsgHandler(TeamCopyForceEndRes.MsgID, OnReceiveTeamCopyForceEndRes);
            //团本战斗结束投票开始的广播
            NetProcess.AddMsgHandler(TeamCopyForceEndVoteNotify.MsgID, OnReceiveTeamCopyEndVoteNotify);
            //团本战斗结束有人投票的结果
            NetProcess.AddMsgHandler(TeamCopyForceEndMemberVote.MsgID, OnReceiveTeamCopyForceEndMemberVoteNotify);
            //团本战斗结束投票的结果通知
            NetProcess.AddMsgHandler(TeamCopyForceEndVoteResult.MsgID, OnReceiveTeamCopyForceEndVoteResultNotify);

            //攻坚面板相关
            //团本阶段同步
            NetProcess.AddMsgHandler(TeamCopyStageNotify.MsgID, OnReceiveTeamCopyStageNotify);
            //团本据点数据更新
            NetProcess.AddMsgHandler(TeamCopyFieldNotify.MsgID,
                OnReceiveTeamCopyFieldNotify);
            //团本某个据点完成
            NetProcess.AddMsgHandler(TeamCopyFieldStatusNotify.MsgID,
                OnReceiveTeamCopyFieldStatusNotify);
            //团本据点解锁比例的通知
            NetProcess.AddMsgHandler(TeamCopyFieldUnlockRate.MsgID, OnReceiveTeamCopyFieldUnlockRate);
            
            //团本目标数据更新
            NetProcess.AddMsgHandler(TeamCopyTargetNotify.MsgID,
                OnReceiveTeamCopyTargetNotify);


            //开始挑战
            NetProcess.AddMsgHandler(TeamCopyStartChallengeRes.MsgID,
                OnReceiveTeamCopyStartChallengeRes);

            //阶段翻牌的奖励
            NetProcess.AddMsgHandler(TeamCopyStageFlopRes.MsgID,
                OnReceiveTeamCopyStageFlopRes);

            //阶段挑战结束
            NetProcess.AddMsgHandler(TeamCopyStageEnd.MsgID, OnReceiveTeamCopyStageEndNotify);

            //团本开关
            ServerSceneFuncSwitchManager.GetInstance().AddServerFuncSwitchListener(ServiceType.SERVICE_TEAM_COPY,
                OnServerFuncSwitch);
        }

        private void UnBindNetEvents()
        {
            NetProcess.RemoveMsgHandler(TeamCopyPlayerInfoNotify.MsgID, OnReceiveTeamCopyPlayerInfoNotify);
            NetProcess.RemoveMsgHandler(TeamCopyTeamUpdate.MsgID, OnReceiveTeamCopyTeamUpdate);
            NetProcess.RemoveMsgHandler(TeamCopyReconnectNotify.MsgID, OnReceiveTeamCopyReconnectNotify);
            NetProcess.RemoveMsgHandler(TeamCopyPlayerExpireNotify.MsgID, OnReceiveTeamCopyPlayerExpireNotify);

            NetProcess.RemoveMsgHandler(TeamCopyCreateTeamRes.MsgID, OnReceiveTeamCopyCreateTeamRes);
            NetProcess.RemoveMsgHandler(TeamCopyModifyEquipScoreRes.MsgID, OnReceiveTeamCopyModifyEquipScoreRes);
            NetProcess.RemoveMsgHandler(TeamCopyTeamDataRes.MsgID, OnReceiveTeamCopyTeamDataRes);
            NetProcess.RemoveMsgHandler(TeamCopyTeamStatusNotify.MsgID, OnReceiveTeamCopyTeamStatusNotify);
            NetProcess.RemoveMsgHandler(TeamCopySquadNotify.MsgID, OnReceiveTeamCopySquadNotify);

            NetProcess.RemoveMsgHandler(TeamCopyKickRes.MsgID, OnReceiveTeamCopyKickRes);
            NetProcess.RemoveMsgHandler(TeamCopyAppointmentRes.MsgID, OnReceiveTeamCopyAppointmentRes);
            NetProcess.RemoveMsgHandler(TeamCopyChangeSeatRes.MsgID, OnReceiveTeamCopyChangeSeatRes);
            NetProcess.RemoveMsgHandler(TeamCopyAppointmentNotify.MsgID, OnReceiveTeamCopyAppointmentNotify);

            NetProcess.RemoveMsgHandler(TeamCopyTeamListRes.MsgID, OnReceiveTeamCopyTeamListRes);
            NetProcess.RemoveMsgHandler(TeamCopyTeamDetailRes.MsgID, OnReceiveTeamCopyTeamDetailRes);

            NetProcess.RemoveMsgHandler(TeamCopyTeamApplyRes.MsgID, OnReceiveTeamCopyTeamApplyRes);
            NetProcess.RemoveMsgHandler(TeamCopyApplyNotify.MsgID, OnReceiveTeamCopyApplyNotify);
            NetProcess.RemoveMsgHandler(TeamCopyApplyRefuseNotify.MsgID, OnReceiveTeamCopyApplyRefuseNotify);
            NetProcess.RemoveMsgHandler(TeamCopyTeamApplyListRes.MsgID, OnReceiveTeamCopyTeamApplyListRes);
            NetProcess.RemoveMsgHandler(TeamCopyMemberNotify.MsgID, OnReceiveTeamCopyMemberNotify);

            NetProcess.RemoveMsgHandler(TeamCopyAutoAgreeGoldRes.MsgID, OnReceiveTeamCopyAutoAgreeGoldRes);

            NetProcess.RemoveMsgHandler(TeamCopyFindTeamMateRes.MsgID, OnReceiveTeamCopyFindTeamMateRes);
            NetProcess.RemoveMsgHandler(TeamCopyInviteNotify.MsgID, OnReceiveTeamCopyInviteNotify);

            NetProcess.RemoveMsgHandler(TeamCopyInviteListRes.MsgID, OnReceiveTeamCopyInviteListRes);
            NetProcess.RemoveMsgHandler(TeamCopyInviteChoiceRes.MsgID, OnReceiveTeamCopyInviteChoiceRes);
            NetProcess.RemoveMsgHandler(TeamCopyRecruitRes.MsgID, OnReceiveTeamRecruitRes);
            NetProcess.RemoveMsgHandler(TeamCopyLinkJoinRes.MsgID, OnReceiveTeamCopyLinkJoinRes);

            NetProcess.RemoveMsgHandler(TeamCopyTeamApplyReplyRes.MsgID, OnReceiveTeamCopyTeamApplyReplyRes);

            NetProcess.RemoveMsgHandler(TeamCopyTeamQuitRes.MsgID, OnReceiveTeamCopyTeamQuitRes);

            NetProcess.RemoveMsgHandler(TeamCopyStartBattleRes.MsgID, OnReceiveTeamCopyStartBattleRes);
            NetProcess.RemoveMsgHandler(TeamCopyStartBattleNotify.MsgID, OnReceiveTeamCopyStartBattleNotify);
            NetProcess.RemoveMsgHandler(TeamCopyVoteNotify.MsgID, OnReceiveTeamCopyVoteNotify);
            NetProcess.RemoveMsgHandler(TeamCopyVoteFinish.MsgID, OnReceiveTeamCopyVoteFinish);

            NetProcess.RemoveMsgHandler(TeamCopyForceEndFlag.MsgID, OnReceiveTeamCopyForceEndFlag);
            NetProcess.RemoveMsgHandler(TeamCopyForceEndRes.MsgID, OnReceiveTeamCopyForceEndRes);
            NetProcess.RemoveMsgHandler(TeamCopyForceEndVoteNotify.MsgID, OnReceiveTeamCopyEndVoteNotify);
            NetProcess.RemoveMsgHandler(TeamCopyForceEndMemberVote.MsgID, OnReceiveTeamCopyForceEndMemberVoteNotify);
            NetProcess.RemoveMsgHandler(TeamCopyForceEndVoteResult.MsgID, OnReceiveTeamCopyForceEndVoteResultNotify);


            NetProcess.RemoveMsgHandler(TeamCopyStageNotify.MsgID, OnReceiveTeamCopyStageNotify);
            NetProcess.RemoveMsgHandler(TeamCopyFieldNotify.MsgID, OnReceiveTeamCopyFieldNotify);
            NetProcess.RemoveMsgHandler(TeamCopyTargetNotify.MsgID, OnReceiveTeamCopyTargetNotify);
            NetProcess.RemoveMsgHandler(TeamCopyFieldStatusNotify.MsgID,
                OnReceiveTeamCopyFieldStatusNotify);
            NetProcess.RemoveMsgHandler(TeamCopyFieldUnlockRate.MsgID, OnReceiveTeamCopyFieldUnlockRate);

            NetProcess.RemoveMsgHandler(TeamCopyStartChallengeRes.MsgID,
                OnReceiveTeamCopyStartChallengeRes);

            NetProcess.RemoveMsgHandler(TeamCopyStageFlopRes.MsgID,
                OnReceiveTeamCopyStageFlopRes);

            NetProcess.RemoveMsgHandler(TeamCopyStageEnd.MsgID, OnReceiveTeamCopyStageEndNotify);

            ServerSceneFuncSwitchManager.GetInstance().RemoveServerFuncSwitchListener(ServiceType.SERVICE_TEAM_COPY,
                OnServerFuncSwitch);
        }

        public override void Clear()
        {
            ClearData();
            UnBindNetEvents();

            //全局
            IsAlreadyShowTicketIsNotEnoughTip = false;
            ResetPlayerInformationDataModel();
        }

        public void ClearData()
        {
            IsTeamDuplicationOwnerTeam = false;
            ResetTeamDataModel();
            IsAlreadyReceiveFinalReward = false;

            IsTeamDuplicationGoldOwner = false;

            ResetTeamListDataModelList();

            ResetTeamDuplicationFightPanelData();

            FightSettingConfigPlanModel = TeamCopyPlanModel.TEAM_COPY_PLAN_MODEL_INVALID;
            ResetTeamConfigDataModelList();

            ResetTeamRequesterDataModelList();
            ResetTeamRequestJoinInEndTimeDic();

            ResetTeamFriendDataModelList();

            ResetFightStageRewardDataModelList();
            //战斗开始投票
            ResetFightStartVoteData();
            //战斗结束投票
            ResetFightEndVoteData();
            TeamDuplicationFightEndVoteFlag = false;

            IsAlreadyShowPositionAdjustTip = false;

            ResetTeamDuplicationReconnectData();

            ResetTeamDuplicationTeamInviteDataModelList();

            IsTeamDuplicationOwnerNewInvite = false;
            IsTeamDuplicationOwnerNewRequester = false;
            TeamDuplicationEndStageId = 0;

            IsNeedShowGameFailResult = false;

            TeamDuplicationPlayerExpireTimeDic.Clear();

            IsEnterTeamDuplicationBuildSceneFromTown = false;

            TeamDuplicationFightPointUnLockRateDictionary.Clear();
            BossPhase = 0;
            BossBloodRate = 0;
        }

        #region TeamDuplicationReconnectNotify
        private void OnReceiveTeamCopyReconnectNotify(MsgDATA msgData)
        {
            TeamCopyReconnectNotify teamCopyReconnectNotify = new TeamCopyReconnectNotify();
            teamCopyReconnectNotify.decode(msgData.bytes);

            var sceneId = (int) teamCopyReconnectNotify.sceneId;

            var citySceneTable = TableManager.GetInstance().GetTableItem<CitySceneTable>(sceneId);
            if (citySceneTable == null)
                return;

            if (citySceneTable.SceneSubType != CitySceneTable.eSceneSubType.TeamDuplicationBuid
               && citySceneTable.SceneSubType != CitySceneTable.eSceneSubType.TeamDuplicationFight)
                return;

            TeamDuplicationReconnectSceneId = sceneId;
            TeamDuplicationReconnectExpireTime = teamCopyReconnectNotify.expireTime;
        }

        //收到团本队员的离线时间
        private void OnReceiveTeamCopyPlayerExpireNotify(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyPlayerExpireNotify teamCopyPlayerExpireNotify = new TeamCopyPlayerExpireNotify();
            teamCopyPlayerExpireNotify.decode(msgData.bytes);

            var playerId = teamCopyPlayerExpireNotify.playerId;
            var playerExpireTime = teamCopyPlayerExpireNotify.expireTime;

            TeamDuplicationPlayerExpireTimeDic[playerId] = playerExpireTime;

            //队员离线时间进行缓存
            TeamDuplicationUtility.UpdateTeamDuplicationPlayerExpireTimeByGuid(playerId,
                playerExpireTime);
            
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationPlayerExpireTimeMessage,
                playerId, playerExpireTime);
        }

        public void ResetTeamDuplicationReconnectData()
        {
            TeamDuplicationReconnectSceneId = 0;
            TeamDuplicationReconnectExpireTime = 0;
        }

        #endregion

        #region TeamDuplicationTeamDataInfo
        //团队相关

        //团队中自己的相关数据
        private void OnReceiveTeamCopyPlayerInfoNotify(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyPlayerInfoNotify teamCopyPlayerInfoNotify = new TeamCopyPlayerInfoNotify();
            teamCopyPlayerInfoNotify.decode(msgData.bytes);

            //创建角色
            if (_playerInformationDataModel == null)
                _playerInformationDataModel = TeamDuplicationUtility.CreateTeamDuplicationPlayerInformationDataModel();

            //每天完成和总的次数
            _playerInformationDataModel.DayAlreadyFightNumber = (int) teamCopyPlayerInfoNotify.dayNum;
            _playerInformationDataModel.DayTotalFightNumber = (int) teamCopyPlayerInfoNotify.dayTotalNum;

            //每周完成和总的次数
            _playerInformationDataModel.WeekAlreadyFightNumber = (int) teamCopyPlayerInfoNotify.weekNum;
            _playerInformationDataModel.WeekTotalFightNumber = (int) teamCopyPlayerInfoNotify.weekTotalNum;

            //噩梦挑战次数
            _playerInformationDataModel.HardLevelAlreadyFightNumber = (int) teamCopyPlayerInfoNotify.diffWeekNum;
            _playerInformationDataModel.HardLevelTotalFightNumber = (int) teamCopyPlayerInfoNotify.diffWeekTotalNum;

            //普通难度的通关次数和解锁的总次数
            _playerInformationDataModel.CommonLevelPassNumber = (int) teamCopyPlayerInfoNotify.commonGradePassNum;
            _playerInformationDataModel.UnLockCommonLevelTotalNumber =
                (int) teamCopyPlayerInfoNotify.unlockDiffGradeCommonNum;

            //1:可以创建金团；0：不可以创建金团
            _playerInformationDataModel.IsCanCreateGold = teamCopyPlayerInfoNotify.isCreateGold == 1;

            //周和日免费退出的次数
            _playerInformationDataModel.DayFreeQuitNumber = (int) teamCopyPlayerInfoNotify.dayFreeQuitNum;
            _playerInformationDataModel.WeekFreeQuitNumber = (int) teamCopyPlayerInfoNotify.weekFreeQuitNum;

            //门票是否足够 1: 表示足够，0：表示门票不足
            _playerInformationDataModel.TicketIsEnough = teamCopyPlayerInfoNotify.ticketIsEnough == 1 ? true : false;

            //添加已经解锁的难度
            _playerInformationDataModel.AlreadyOpenDifficultyList.Clear();
            for (var i = 0; i < teamCopyPlayerInfoNotify.yetOpenGradeList.Length; i++)
            {
                _playerInformationDataModel.AlreadyOpenDifficultyList.Add(teamCopyPlayerInfoNotify.yetOpenGradeList[i]);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationPlayerInformationNotify);
        }

        //玩家的团本数据
        public TeamDuplicationPlayerInformationDataModel GetPlayerInformationDataModel()
        {
            if (_playerInformationDataModel == null)
                _playerInformationDataModel = TeamDuplicationUtility.CreateTeamDuplicationPlayerInformationDataModel();

            return _playerInformationDataModel;
        }

        //重置玩家的团本数据
        private void ResetPlayerInformationDataModel()
        {
            if (_playerInformationDataModel != null)
            {
                if(_playerInformationDataModel.AlreadyOpenDifficultyList != null)
                    _playerInformationDataModel.AlreadyOpenDifficultyList.Clear();
            }

            _playerInformationDataModel = null;
        }


        //团队数据更新：解散或者ID更新
        private void OnReceiveTeamCopyTeamUpdate(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyTeamUpdate teamUpdate = new TeamCopyTeamUpdate();
            teamUpdate.decode(msgData.bytes);

            //队伍解散
            if (teamUpdate.opType == 1)
            {
                ResetTeamDuplicationDataByQuitTeam();

                //队伍解散消息
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationDismissMessage);
            }
            else
            {
                IsTeamDuplicationOwnerTeam = true;
            }
        }

        //创建团队
        public void OnSendTeamCopyCreateTeamReq(uint teamModel,
            uint equipmentScore,
            uint goldValueNumber,
            uint teamDifficultyType = 0)
        {
            //0:挑战团队；1：金团团队

            var createTeamReq = new TeamCopyCreateTeamReq();
            createTeamReq.teamModel = teamModel;
            createTeamReq.equipScore = equipmentScore;
            createTeamReq.param = goldValueNumber;

            //难度，默认为1，普通难度
            createTeamReq.teamGrade = teamDifficultyType;
            if (teamDifficultyType == 0)
                createTeamReq.teamGrade = 1;

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, createTeamReq);
        }

        private void OnReceiveTeamCopyCreateTeamRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyCreateTeamRes createTemRes = new TeamCopyCreateTeamRes();
            createTemRes.decode(msgData.bytes);

            if (createTemRes.retCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)createTemRes.retCode);
            }
        }

        //修改团本的装备评分
        public void OnSendTeamCopyModifyEquipScoreReq(int teamEquipScore)
        {
            TeamCopyModifyEquipScoreReq teamCopyModifyEquipScoreEquip = new TeamCopyModifyEquipScoreReq();
            teamCopyModifyEquipScoreEquip.equipScore = (uint) teamEquipScore;

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, teamCopyModifyEquipScoreEquip);
        }

        //修改团本装备评分的返回
        private void OnReceiveTeamCopyModifyEquipScoreRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyModifyEquipScoreRes teamCopyModifyEquipScoreRes = new TeamCopyModifyEquipScoreRes();
            teamCopyModifyEquipScoreRes.decode(msgData.bytes);

            if (teamCopyModifyEquipScoreRes.retCode != (uint) ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int) teamCopyModifyEquipScoreRes.retCode);
            }
        }

        //请求团队的数据
        public void OnSendTeamCopyTeamDataReq(int teamId)
        {
            TeamCopyTeamDataReq teamDataReq = new TeamCopyTeamDataReq();
            teamDataReq.teamId = (uint)teamId;

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, teamDataReq);
        }

        //团队数据下发成功
        private void OnReceiveTeamCopyTeamDataRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyTeamDataRes teamDataRes = new TeamCopyTeamDataRes();
            teamDataRes.decode(msgData.bytes);

            if (teamDataRes.teamId <= 0)
                return;

            //自己团队
            IsTeamDuplicationOwnerTeam = true;

            if (_teamDataModel != null)
            {
                //创建了团本
                if (_teamDataModel.TeamId == 0 || _teamDataModel.TeamId != teamDataRes.teamId)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationBuildTeamSuccessMessage);
                }
            }

            ResetTeamDataModel();

            TeamDuplicationUtility.UpdateTeamDataModelByTeamCopyTeamDataRes(_teamDataModel,
                teamDataRes);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationTeamDataMessage);
        }

        //团本状态更新
        private void OnReceiveTeamCopyTeamStatusNotify(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyTeamStatusNotify teamCopyTeamStatusNotify = new TeamCopyTeamStatusNotify();
            teamCopyTeamStatusNotify.decode(msgData.bytes);

            if (teamCopyTeamStatusNotify.teamId <= 0)
                return;

            //没有团本
            if (IsTeamDuplicationOwnerTeam == false)
                return;

            //团本的ID不一致
            if (_teamDataModel == null || _teamDataModel.TeamId != teamCopyTeamStatusNotify.teamId)
                return;

            _teamDataModel.TeamStatus = (TeamCopyTeamStatus) teamCopyTeamStatusNotify.teamStatus;
            if (_teamDataModel.TeamStatus == TeamCopyTeamStatus.TEAM_COPY_TEAM_STATUS_FAILED)
            {
                IsNeedShowGameFailResult = true;
            }
            else
            {
                IsNeedShowGameFailResult = false;
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationTeamStatusNotifyMessage);
        }

        //请求团本详情数据
        public void OnSendTeamDetailReq(int teamId)
        {
            TeamCopyTeamDetailReq teamCopyTeamDetailReq = new TeamCopyTeamDetailReq();
            teamCopyTeamDetailReq.teamId = (uint) teamId;

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, teamCopyTeamDetailReq);
        }

        private void OnReceiveTeamCopyTeamDetailRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyTeamDetailRes teamCopyTeamDetailRes = new TeamCopyTeamDetailRes();
            teamCopyTeamDetailRes.decode(msgData.bytes);

            if (teamCopyTeamDetailRes.teamId <= 0)
                return;

            ResetOtherTeamDataModel();
            TeamDuplicationUtility.UpdateTeamDataModelByTeamCopyTeamDetailRes(OtherTeamDataModel,
                teamCopyTeamDetailRes);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationTeamDetailDataMessage,
                (int)teamCopyTeamDetailRes.teamId);
        }

        //小队数据同步
        private void OnReceiveTeamCopySquadNotify(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            var teamCopySquadNotify = new TeamCopySquadNotify();
            teamCopySquadNotify.decode(msgData.bytes);
            //不存在队伍
            if (IsTeamDuplicationOwnerTeam == false)
                return;

            var captainDataModel = TeamDuplicationUtility.GetTeamDuplicationCaptainDataModelByCaptainId(
                (int) teamCopySquadNotify.squadId);

            if (captainDataModel == null)
                return;

            captainDataModel.CaptainStatus = (int) teamCopySquadNotify.squadStatus;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationCaptainNotifyMessage,
                (int)teamCopySquadNotify.squadId);
        }

        //退出队伍
        public void OnSendTeamCopyTeamQuitReq()
        {
            ////不存在团队，不用退出
            if (IsTeamDuplicationOwnerTeam == false)
                return;

            TeamCopyTeamQuitReq teamQuitReq = new TeamCopyTeamQuitReq();

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, teamQuitReq);
        }

        private void OnReceiveTeamCopyTeamQuitRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyTeamQuitRes teamQuitRes = new TeamCopyTeamQuitRes();
            teamQuitRes.decode(msgData.bytes);

            if (teamQuitRes.retCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)teamQuitRes.retCode);
                return;
            }

            //退出队伍之后，情况相关的数据
            ResetTeamDuplicationDataByQuitTeam();

            //队伍解散消息
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationQuitTeamMessage);
        }

        //得到所有小队的数据信息
        public List<TeamDuplicationCaptainDataModel> GetTeamDuplicationCaptainDataModelList()
        {
            if (_teamDataModel == null || _teamDataModel.CaptainDataModelList == null)
                return null;

            return _teamDataModel.CaptainDataModelList;
        }

        //得到团本的整体数据
        public TeamDuplicationTeamDataModel GetTeamDuplicationTeamDataModel()
        {
            return _teamDataModel;
        }

        #endregion

        #region TeamDuplicationTeamList
        //团队列表相关内容

        //请求团队列表
        public void OnSendTeamCopyTeamListReq(int pageNumber,
            int teamDuplicationTeamModel = 0)
        {
            TeamCopyTeamListReq teamListReq = new TeamCopyTeamListReq();
            teamListReq.pageNum = (uint)pageNumber;
            teamListReq.teamModel = (uint)teamDuplicationTeamModel;

            var netMgr = NetManager.Instance();
            if (netMgr != null)
            {
                netMgr.SendCommand(ServerType.GATE_SERVER, teamListReq);
            }
        }

        private void OnReceiveTeamCopyTeamListRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyTeamListRes teamListRes = new TeamCopyTeamListRes();
            teamListRes.decode(msgData.bytes);

            TeamListCurrentPage = (int)teamListRes.curPage;
            TeamListTotalPage = (int)teamListRes.totalPageNum;

            _teamListDataModelList.Clear();
            if (teamListRes.teamList != null && teamListRes.teamList.Length > 0)
            {
                for (var i = 0; i < teamListRes.teamList.Length; i++)
                {
                    var teamCopyTeamProperty = teamListRes.teamList[i];
                    if (teamCopyTeamProperty != null)
                    {
                        var teamDataModel = new TeamDuplicationTeamListDataModel();
                        teamDataModel.TeamId = teamCopyTeamProperty.teamId;
                        teamDataModel.TeamType = (TeamCopyTeamModel)teamCopyTeamProperty.teamModel;
                        teamDataModel.GoldValue = (int)teamCopyTeamProperty.commission;
                        teamDataModel.TeamName = teamCopyTeamProperty.teamName;
                        teamDataModel.TeamHardLevel = teamCopyTeamProperty.teamGrade;
                        teamDataModel.TeamNumber = (int)teamCopyTeamProperty.memberNum;
                        teamDataModel.EquipmentScore = (int)teamCopyTeamProperty.equipScore;
                        teamDataModel.TroopStatus = (int)teamCopyTeamProperty.status;

                        _teamListDataModelList.Add(teamDataModel);
                    }
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationTeamListRes);

        }

        //成员加入或者离开的广播通知
        private void OnReceiveTeamCopyMemberNotify(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            var teamCopyMemberNotify = new TeamCopyMemberNotify();
            teamCopyMemberNotify.decode(msgData.bytes);

            if (teamCopyMemberNotify.memberName == null)
                return;

            //0：加入，1：离开
            var contentStr = string.Format(TR.Value("team_duplication_leave_team"), teamCopyMemberNotify.memberName);
            if (teamCopyMemberNotify.flag == 0)
                contentStr = string.Format(TR.Value("team_duplication_join_in_team"), teamCopyMemberNotify.memberName);

            SystemNotifyManager.SysNotifyFloatingEffect(contentStr);

        }

        //自动同意金主入团
        public void OnSendTeamCopyAutoAgreeGoldReq(bool isAutoAgree)
        {
            TeamCopyAutoAgreeGoldReq teamCopyAutoAgreeGoldReq = new TeamCopyAutoAgreeGoldReq();
            //  (0：不同意，1：同意)
            teamCopyAutoAgreeGoldReq.isAutoAgree = 0;
            if (isAutoAgree == true)
                teamCopyAutoAgreeGoldReq.isAutoAgree = 1;

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, teamCopyAutoAgreeGoldReq);
        }

        private void OnReceiveTeamCopyAutoAgreeGoldRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyAutoAgreeGoldRes teamCopyAutoAgreeGoldRes = new TeamCopyAutoAgreeGoldRes();
            teamCopyAutoAgreeGoldRes.decode(msgData.bytes);

            if (teamCopyAutoAgreeGoldRes.retCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)teamCopyAutoAgreeGoldRes.retCode);
                return;
            }

            if (_teamDataModel == null)
                return;

            _teamDataModel.AutoAgreeGold = teamCopyAutoAgreeGoldRes.isAutoAgree != 0;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationAutoAgreeGoldMessage);
        }

        //申请团队入队
        public void OnSendTeamCopyTeamApplyReq(int teamId)
        {
            var teamApplyReq = new TeamCopyTeamApplyReq();
            teamApplyReq.teamId = (uint)teamId;

            //0不是金主，1：金主
            teamApplyReq.isGold = 0;
            if (IsTeamDuplicationGoldOwner == true)
                teamApplyReq.isGold = 1;

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, teamApplyReq);
        }

        //团队成员申请者的返回
        private void OnReceiveTeamCopyTeamApplyRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyTeamApplyRes teamApplyRes = new TeamCopyTeamApplyRes();
            teamApplyRes.decode(msgData.bytes);

            //申请返回，没有成功,存在错误码，根据错误码进行相应的操作
            if (teamApplyRes.retCode != (uint)ProtoErrorCode.SUCCESS)
            {
                if (teamApplyRes.expireTime > 0)
                {
                    //处在CD时间，不可以申请，按钮倒计时
                    //飘字
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_can_not_join_in_team"));
                    //设置teamID的CD时间
                    var teamId = (int) teamApplyRes.teamId;
                    SetTeamRequestJoinInEndTime(teamId, (int) teamApplyRes.expireTime);
                    //发送UI事件
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationJoinTeamInCdTimeMessage,
                        teamId);
                }
                else
                {
                    //可以申请，但是没有申请成功。飘字，并刷新
                    SystemNotifyManager.SystemNotify((int)teamApplyRes.retCode);
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationRefreshTeamListMessage);
                }
            }
            else
            {
                //申请成功提示
                //已申请加入团队，等待团长同意
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_request_team_join_in"));

                //申请成功，需要刷新一下界面
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationRefreshTeamListMessage);
            }
        }

        //存在新的申请者的通知
        private void OnReceiveTeamCopyApplyNotify(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyApplyNotify teamCopyApplyNotify = new TeamCopyApplyNotify();
            teamCopyApplyNotify.decode(msgData.bytes);

            //1表示有人申请，0表示没有人申请
            IsTeamDuplicationOwnerNewRequester = teamCopyApplyNotify.IsHasApply == 1 ? true : false;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationOwnerNewRequesterMessage);
        }

        //团长拒绝通知
        private void OnReceiveTeamCopyApplyRefuseNotify(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyApplyRefuseNotify teamCopyApplyRefuseNotify = new TeamCopyApplyRefuseNotify();
            teamCopyApplyRefuseNotify.decode(msgData.bytes);

            if (string.IsNullOrEmpty(teamCopyApplyRefuseNotify.chiefName) == true)
                return;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationTeamLeaderRefuseJoinInMessage,
                teamCopyApplyRefuseNotify.chiefName);
        }

        //是否解锁了金团模式
        public bool IsTeamDuplicationUnLockGoldType()
        {
            //需要各种条件
            return true;
        }
        #endregion

        #region TeamDuplicationTeamRequesterList

        //申请队伍列表列表

        //发送申请者列表的请求
        public void OnSendTeamCopyTeamApplyListReq()
        {
            TeamCopyTeamApplyListReq teamApplyListReq = new TeamCopyTeamApplyListReq();

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, teamApplyListReq);
        }

        public void OnReceiveTeamCopyTeamApplyListRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyTeamApplyListRes teamApplyListRes = new TeamCopyTeamApplyListRes();
            teamApplyListRes.decode(msgData.bytes);

            //数据重置
            ResetTeamRequesterDataModelList();

            //数据添加
            if (teamApplyListRes.applyList != null && teamApplyListRes.applyList.Length > 0)
            {
                for (var i = 0; i < teamApplyListRes.applyList.Length; i++)
                {
                    var applyProperty = teamApplyListRes.applyList[i];
                    if (applyProperty != null)
                    {
                        TeamDuplicationRequesterDataModel requesterDataModel = new TeamDuplicationRequesterDataModel();
                        TeamDuplicationUtility.UpdateTeamDuplicationRequesterDataModel(applyProperty,
                            requesterDataModel);
                        _teamRequesterDataModelList.Add(requesterDataModel);
                    }
                }
            }

            //数据排序
            TeamDuplicationUtility.SortTeamDuplicationRequesterDataModelList(_teamRequesterDataModelList);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationRequesterListMessage);
        }

        //申请团队相关内容(同意入团或者拒绝入团)
        public void OnSendTeamCopyTeamApplyReplyReq(bool isAgree, List<ulong> playerIdList)
        {
            TeamCopyTeamApplyReplyReq applyReplyReq = new TeamCopyTeamApplyReplyReq();

            applyReplyReq.isAgree = 0;
            if (isAgree == true)
                applyReplyReq.isAgree = 1;

            if (playerIdList != null && playerIdList.Count > 0)
            {
                applyReplyReq.playerIds = playerIdList.ToArray();
            }

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, applyReplyReq);
        }

        private void OnReceiveTeamCopyTeamApplyReplyRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyTeamApplyReplyRes applyReplyRes = new TeamCopyTeamApplyReplyRes();
            applyReplyRes.decode(msgData.bytes);

            //提示错误信息
            if (applyReplyRes.retCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)applyReplyRes.retCode);
            }

            //团长拒绝，不弹飘字
            ////拒绝添加提示
            //if (applyReplyRes.isAgree == 0)
            //{
            //    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_team_troop_refuse_request"));
            //}

            if (applyReplyRes.playerIds.Length == 0)
                return;

            //处理申请列表的数据，并更新界面
            for (var i = 0; i < applyReplyRes.playerIds.Length; i++)
            {
                var curPlayerId = applyReplyRes.playerIds[i];

                if (_teamRequesterDataModelList != null && _teamRequesterDataModelList.Count > 0)
                {
                    //遍历申请列表
                    for (var j = 0; j < _teamRequesterDataModelList.Count; j++)
                    {
                        var curRequesterDataModel = _teamRequesterDataModelList[j];
                        if (curRequesterDataModel.PlayerGuid == curPlayerId)
                        {
                            _teamRequesterDataModelList.RemoveAt(j);
                            break;
                        }
                    }
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationRequesterListMessage);
        }

        public List<TeamDuplicationRequesterDataModel> GetRequesterDataModelList()
        {
            return _teamRequesterDataModelList;
        }

        //申请者的列表
        public void ResetTeamRequesterDataModelList()
        {
            if (_teamRequesterDataModelList != null)
                _teamRequesterDataModelList.Clear();
        }

        //队伍申请加入的截止时间
        public void ResetTeamRequestJoinInEndTimeDic()
        {
            if (_teamRequestJoinInEndTimeDic != null)
                _teamRequestJoinInEndTimeDic.Clear();
        }

        public int GetTeamRequestJoinInEndTime(int teamId)
        {
            var joinInTime = 0;
            if (_teamRequestJoinInEndTimeDic == null)
                return 0;

            if (_teamRequestJoinInEndTimeDic.ContainsKey(teamId) == true)
                joinInTime = _teamRequestJoinInEndTimeDic[teamId];

            return joinInTime;
        }

        public void SetTeamRequestJoinInEndTime(int teamId, int joinInTime)
        {
            if (_teamRequestJoinInEndTimeDic == null)
                return;

            _teamRequestJoinInEndTimeDic[teamId] = joinInTime;
        }

        #endregion

        #region TeamDuplicationFindTeamMate
        //找队友相关内容

        //发送队友的消息
        public void OnSendTeamCopyFindTeamMateReq()
        {
            TeamCopyFindTeamMateReq teamMateReq = new TeamCopyFindTeamMateReq();

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, teamMateReq);
        }

        private void OnReceiveTeamCopyFindTeamMateRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyFindTeamMateRes teamMateRes = new TeamCopyFindTeamMateRes();
            teamMateRes.decode(msgData.bytes);

            //数据重置
            ResetTeamFriendDataModelList();

            //数据添加
            if (teamMateRes.playerList != null && teamMateRes.playerList.Length > 0)
            {
                for (var i = 0; i < teamMateRes.playerList.Length; i++)
                {
                    var applyProperty = teamMateRes.playerList[i];
                    if (applyProperty != null)
                    {
                        TeamDuplicationRequesterDataModel friendDataModel = new TeamDuplicationRequesterDataModel();
                        TeamDuplicationUtility.UpdateTeamDuplicationRequesterDataModel(applyProperty,
                            friendDataModel);
                        _teamFriendDataModelList.Add(friendDataModel);
                    }
                }
            }

            //数据排序
            TeamDuplicationUtility.SortTeamDuplicationRequesterDataModelList(_teamFriendDataModelList);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationTeamMateListMessage);
        }

        //邀请好友
        public void OnSendTeamCopyInvitePlayer(List<ulong> invitePlayerIdList)
        {
            TeamCopyInvitePlayer teamCopyInvitePlayer = new TeamCopyInvitePlayer();

            if (invitePlayerIdList != null && invitePlayerIdList.Count > 0)
                teamCopyInvitePlayer.inviteList = invitePlayerIdList.ToArray();

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, teamCopyInvitePlayer);

        }

        private void ResetTeamFriendDataModelList()
        {
            if (_teamFriendDataModelList != null)
                _teamFriendDataModelList.Clear();
        }

        public List<TeamDuplicationRequesterDataModel> GetTeamFriendDataModelList()
        {
            return _teamFriendDataModelList;
        }

        #endregion

        #region TeamDuplicationTeamInvite

        //是否拥有邀请者的通知
        private void OnReceiveTeamCopyInviteNotify(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyInviteNotify teamCopyInviteNotify = new TeamCopyInviteNotify();
            teamCopyInviteNotify.decode(msgData.bytes);

            IsTeamDuplicationOwnerNewInvite = true;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationOwnerNewTeamInviteMessage);
        }

        //发送团队邀请者请求
        public void OnSendTeamCopyInviteListReq()
        {
            TeamCopyInviteListReq teamCopyInviteListReq = new TeamCopyInviteListReq();

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, teamCopyInviteListReq);
        }

        private void OnReceiveTeamCopyInviteListRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyInviteListRes teamCopyInviteListRes = new TeamCopyInviteListRes();
            teamCopyInviteListRes.decode(msgData.bytes);

            TeamInviteDataModelList.Clear();

            if (teamCopyInviteListRes.inviteList != null && teamCopyInviteListRes.inviteList.Length > 0)
            {
                for (var i = 0; i < teamCopyInviteListRes.inviteList.Length; i++)
                {
                    TCInviteInfo tcInviteInfo = teamCopyInviteListRes.inviteList[i];
                    TeamDuplicationTeamInviteDataModel teamInviteDataModel = new TeamDuplicationTeamInviteDataModel();
                    TeamDuplicationUtility.UpdateTeamDuplicationTeamInviteDataModel(tcInviteInfo,
                        teamInviteDataModel);
                    TeamInviteDataModelList.Add(teamInviteDataModel);
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationTeamInviteListMessage);
        }

        //邀请选择
        public void OnSendTeamCopyInviteChoiceReq(bool isAgree, List<uint> teamIdList)
        {
            TeamCopyInviteChoiceReq teamCopyInviteChoiceReq = new TeamCopyInviteChoiceReq();
            teamCopyInviteChoiceReq.isAgree = 0;
            if (isAgree == true)
                teamCopyInviteChoiceReq.isAgree = 1;

            teamCopyInviteChoiceReq.teamId = teamIdList.ToArray();

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, teamCopyInviteChoiceReq);
        }

        //邀请选择返回
        private void OnReceiveTeamCopyInviteChoiceRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyInviteChoiceRes teamCopyInviteChoiceRes = new TeamCopyInviteChoiceRes();
            teamCopyInviteChoiceRes.decode(msgData.bytes);

            //返回错误码
            if (teamCopyInviteChoiceRes.retCode != (uint) ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int) teamCopyInviteChoiceRes.retCode);
                return;
            }

            //拒绝的teamId，需要删除相关数据
            if (teamCopyInviteChoiceRes.teamId != null && teamCopyInviteChoiceRes.teamId.Length > 0)
            {
                //删除对应的团队数据
                for (var i = 0; i < teamCopyInviteChoiceRes.teamId.Length; i++)
                {
                    var deleteTeamId = teamCopyInviteChoiceRes.teamId[i];
                    if (TeamInviteDataModelList != null && TeamInviteDataModelList.Count > 0)
                    {
                        for (var j = 0; j < TeamInviteDataModelList.Count; j++)
                        {
                            var teamInviteDataModel = TeamInviteDataModelList[j];

                            if(teamInviteDataModel == null)
                                continue;

                            //删除对应的某个队伍
                            if (teamInviteDataModel.TeamId == deleteTeamId)
                            {
                                TeamInviteDataModelList.RemoveAt(j);
                                break;
                            }
                        }
                    }
                }

                //拒绝飘字
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_build_team_refuse_invite"));

                //刷新界面
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationTeamInviteChoiceMessage);
            }
        }

        //发送招募消息
        public void OnSendTeamRecruitReq()
        {
            TeamCopyRecruitReq teamCopyRecruitReq = new TeamCopyRecruitReq();

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, teamCopyRecruitReq);
        }

        //招募消息的返回
        private void OnReceiveTeamRecruitRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyRecruitRes teamCopyRecruitRes = new TeamCopyRecruitRes();
            teamCopyRecruitRes.decode(msgData.bytes);

            if (teamCopyRecruitRes.retCode != (uint) ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int) teamCopyRecruitRes.retCode);
                return;
            }

            //招募消息返回
            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_quick_chat_message_send"));
        }

        //发送超链接加入团本的消息
        public void OnSendTeamCopyLinkJoinReq(int teamId)
        {
            TeamCopyLinkJoinReq teamCopyLinkJoinReq = new TeamCopyLinkJoinReq();
            teamCopyLinkJoinReq.teamId = (uint)teamId;

            //0：不是金主，1：金主
            teamCopyLinkJoinReq.isGold = 0;
            if (IsTeamDuplicationGoldOwner == true)
                teamCopyLinkJoinReq.isGold = 1;

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, teamCopyLinkJoinReq);
        }

        //超链接加入团本的返回
        private void OnReceiveTeamCopyLinkJoinRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyLinkJoinRes teamCopyLinkJoinRes = new TeamCopyLinkJoinRes();
            teamCopyLinkJoinRes.decode(msgData.bytes);

            if (teamCopyLinkJoinRes.retCode != (uint) ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int) teamCopyLinkJoinRes.retCode);
                return;
            }
        }

        public void ResetTeamDuplicationTeamInviteDataModelList()
        {
            if(TeamInviteDataModelList != null)
                TeamInviteDataModelList.Clear();
        }

        #endregion

        #region TeamDuplicationTeamRoom

        //成为团长或者队长的广播
        public void OnReceiveTeamCopyAppointmentNotify(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyAppointmentNotify teamCopyAppointmentNotify = new TeamCopyAppointmentNotify();
            teamCopyAppointmentNotify.decode(msgData.bytes);

            if (teamCopyAppointmentNotify.post == (uint) TeamCopyPost.TEAM_COPY_POST_CHIEF)
            {
                //任命为团长
                var notifyContentStr = string.Format(TR.Value("team_duplication_appoint_player_to_team_leader"),
                    teamCopyAppointmentNotify.name);
                SystemNotifyManager.SysNotifyFloatingEffect(notifyContentStr);
            }
            else if (teamCopyAppointmentNotify.post == (uint) TeamCopyPost.TEAM_COPY_POST_CAPTAIN)
            {
                //任命为队长
                var notifyContentStr = string.Format(TR.Value("team_duplication_appoint_player_to_captain"),
                    teamCopyAppointmentNotify.name);
                SystemNotifyManager.SysNotifyFloatingEffect(notifyContentStr);
            }
        }

        //踢出某个角色
        public void OnSendTeamCopyKickReq(ulong playerId)
        {
            var teamCopyKickReq = new TeamCopyKickReq();
            teamCopyKickReq.playerId = playerId;

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, teamCopyKickReq);
        }

        private void OnReceiveTeamCopyKickRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            var teamCopyKickRes = new TeamCopyKickRes();
            teamCopyKickRes.decode(msgData.bytes);

            if (teamCopyKickRes.retCode != (uint) ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int) teamCopyKickRes.retCode);
                return;
            }
        }

        //对某个角色进行任命
        public void OnSendTeamCopyAppointmentReq(ulong playerId, int post)
        {
            var teamCopyAppointmentReq = new TeamCopyAppointmentReq();
            teamCopyAppointmentReq.playerId = playerId;
            teamCopyAppointmentReq.post = (uint) post;

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, teamCopyAppointmentReq);
        }

        private void OnReceiveTeamCopyAppointmentRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            var teamCopyAppointmentRes = new TeamCopyAppointmentRes();
            teamCopyAppointmentRes.decode(msgData.bytes);

            if (teamCopyAppointmentRes.retCode != (uint) ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int) teamCopyAppointmentRes.retCode);
                return;
            }
        }

        //交换角色的位置
        public void OnSendTeamCopyChangeSeatReq(int srcSeat, int destSeat)
        {
            var teamCopyChangeSeatReq = new TeamCopyChangeSeatReq();
            teamCopyChangeSeatReq.srcSeat = (uint)srcSeat;
            teamCopyChangeSeatReq.destSeat = (uint)destSeat;

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, teamCopyChangeSeatReq);
        }

        private void OnReceiveTeamCopyChangeSeatRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyChangeSeatRes teamCopyChangeSeatRes = new TeamCopyChangeSeatRes();
            teamCopyChangeSeatRes.decode(msgData.bytes);

            if (teamCopyChangeSeatRes.retCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)teamCopyChangeSeatRes.retCode);
            }
        }

        #endregion

        #region TeamDuplicationFightVote

        //开战投票相关

        //团长开战投票
        public void OnSendTeamCopyStartBattleReq(TeamCopyPlanModel teamCopyPlanModel)
        {
            var startBattleReq = new TeamCopyStartBattleReq();

            //没有配置，默认选择引导模式
            if (teamCopyPlanModel == TeamCopyPlanModel.TEAM_COPY_PLAN_MODEL_INVALID)
            {
                //设置为引导模式
                startBattleReq.planModel = (uint) TeamCopyPlanModel.TEAM_COPY_PLAN_MODEL_GUIDE;
                //小队难度设置为默认数值
                TeamDuplicationUtility.SetTeamDuplicationDefaultCaptainDifficultySetting();

                //引导模式添加小队配置
                if (_teamConfigDataModelList != null
                    && _teamConfigDataModelList.Count > 0)
                {
                    var teamDifficultyConfigCount = _teamConfigDataModelList.Count;
                    startBattleReq.battlePlanList = new TeamCopyBattlePlan[teamDifficultyConfigCount];

                    for (var i = 0; i < teamDifficultyConfigCount; i++)
                    {
                        var teamDifficultyConfigDataModel = _teamConfigDataModelList[i];
                        if (teamDifficultyConfigDataModel == null)
                            continue;

                        TeamCopyBattlePlan battlePlan = new TeamCopyBattlePlan();
                        battlePlan.difficulty = (uint)teamDifficultyConfigDataModel.Difficulty;
                        battlePlan.squadId = (uint)teamDifficultyConfigDataModel.TeamId;
                        startBattleReq.battlePlanList[i] = battlePlan;
                    }
                }
            }
            else
            {
                startBattleReq.planModel = (uint)teamCopyPlanModel;

                //引导模式添加小队配置
                if (teamCopyPlanModel == TeamCopyPlanModel.TEAM_COPY_PLAN_MODEL_GUIDE
                    && _teamConfigDataModelList != null
                    && _teamConfigDataModelList.Count > 0)
                {
                    var teamDifficultyConfigCount = _teamConfigDataModelList.Count;
                    startBattleReq.battlePlanList = new TeamCopyBattlePlan[teamDifficultyConfigCount];

                    for (var i = 0; i < teamDifficultyConfigCount; i++)
                    {
                        var teamDifficultyConfigDataModel = _teamConfigDataModelList[i];
                        if (teamDifficultyConfigDataModel == null)
                            continue;

                        TeamCopyBattlePlan battlePlan = new TeamCopyBattlePlan();
                        battlePlan.difficulty = (uint)teamDifficultyConfigDataModel.Difficulty;
                        battlePlan.squadId = (uint)teamDifficultyConfigDataModel.TeamId;
                        startBattleReq.battlePlanList[i] = battlePlan;
                    }
                }
            }

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, startBattleReq);
        }

        //团长开战的返回消息，主要处理一些错误信息
        private void OnReceiveTeamCopyStartBattleRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            var startBattleRes = new TeamCopyStartBattleRes();
            startBattleRes.decode(msgData.bytes);

            var commonTipDes = TableManager.GetInstance().GetTableItem<CommonTipsDesc>((int) startBattleRes.retCode);
            if (commonTipDes == null || string.IsNullOrEmpty(commonTipDes.Descs) == true)
                return;

            var showContentStr = string.Format(commonTipDes.Descs, startBattleRes.roleName);
            SystemNotifyManager.SysNotifyFloatingEffect(showContentStr);
        }

        //开战投票同步给各个玩家
        private void OnReceiveTeamCopyStartBattleNotify(MsgDATA msgData)
        {

            if (msgData == null)
                return;

            var startBattleNotify = new TeamCopyStartBattleNotify();
            startBattleNotify.decode(msgData.bytes);

            ResetFightStartVoteData();

            //开战信息
            StartBattleVoteIntervalTime = (int) startBattleNotify.voteDurationTime;
            StartBattleVoteEndTime = (int) startBattleNotify.voteEndTime;
            IsInStartBattleVotingTime = true;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationCloseRelationFrame);
            //团本开战投票
            TeamDuplicationUtility.OnOpenTeamDuplicationFightStartVoteFrame();
        }

        //开战的投票
        public void OnSendTeamCopyStartBattleVote(bool isAgree)
        {
            var startBattleVote = new TeamCopyStartBattleVote();

            //1 同意开战；0 拒绝开战
            startBattleVote.vote = 1;
            if (isAgree == false)
                startBattleVote.vote = 0;

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, startBattleVote);
        }

        //投票通知
        private void OnReceiveTeamCopyVoteNotify(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            var teamCopyVoteNotify = new TeamCopyVoteNotify();
            teamCopyVoteNotify.decode(msgData.bytes);

            if (teamCopyVoteNotify.vote == 0)
            {
                //拒绝
                var teamDuplicationPlayerDataModel = TeamDuplicationUtility.GetPlayerDataModelByGuid(
                    teamCopyVoteNotify.roleId);
                if (teamDuplicationPlayerDataModel != null &&
                    string.IsNullOrEmpty(teamDuplicationPlayerDataModel.Name) == false)
                {
                    var contentStr = string.Format(TR.Value("team_duplication_player_refuse_start_battle"),
                        teamDuplicationPlayerDataModel.Name);
                    SystemNotifyManager.SysNotifyFloatingEffect(contentStr);
                }

                //重置数据
                ResetFightStartVoteData();

                //关闭界面
                TeamDuplicationUtility.OnCloseTeamDuplicationFightStartVoteFrame();
            }
            else
            {
                //某个玩家同意, 添加到ID
                AddPlayerGuidToFightStartVoteAgreeList(teamCopyVoteNotify.roleId);

                //同意，发送消息，并更新UI
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationFightStartVoteAgreeMessage,
                    teamCopyVoteNotify.roleId);
            }
        }

        //投票结束，所有人都同意，开战
        private void OnReceiveTeamCopyVoteFinish(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyVoteFinish teamCopyVoteFinish = new TeamCopyVoteFinish();
            teamCopyVoteFinish.decode(msgData.bytes);

            ResetFightStartVoteData();
            TeamDuplicationUtility.OnCloseTeamDuplicationFightStartVoteFrame();

            if (teamCopyVoteFinish.result == 0)
            {
                //投票成功，都同意开战
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationInBattleMessage);
            }
            else
            {
                //投票失败，存在成员拒绝开战
                var playerNameStr = "";
                if (teamCopyVoteFinish.notVotePlayer != null && teamCopyVoteFinish.notVotePlayer.Length > 0)
                {
                    for (var i = 0; i < teamCopyVoteFinish.notVotePlayer.Length; i++)
                    {
                        var notPlayer = teamCopyVoteFinish.notVotePlayer[i];
                        playerNameStr = playerNameStr + notPlayer + ".";
                    }
                }

                if (string.IsNullOrEmpty(playerNameStr) == true)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(
                        TR.Value("team_duplication_start_battle_by_player_refuse"));
                }
                else
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(string.Format(
                        TR.Value("team_duplication_start_battle_by_player_not_enough"),
                        playerNameStr));
                }
            }
        }

        //同意开战的角色列表
        public List<ulong> GetFightStartVoteAgreeList()
        {
            return _fightStartVoteAgreeList;
        }

        //添加同意开战的角色ID
        private void AddPlayerGuidToFightStartVoteAgreeList(ulong guid)
        {
            if (guid == 0)
                return;

            if (_fightStartVoteAgreeList == null)
                _fightStartVoteAgreeList = new List<ulong>();

            bool isFind = false;
            for (var i = 0; i < _fightStartVoteAgreeList.Count; i++)
            {
                if (_fightStartVoteAgreeList[i] == guid)
                {
                    isFind = true;
                    break;
                }
            }

            if (isFind == false)
                _fightStartVoteAgreeList.Add(guid);
        }

        //重置开战投票的数据
        private void ResetFightStartVoteData()
        {
            if(_fightStartVoteAgreeList != null)
                _fightStartVoteAgreeList.Clear();

            StartBattleVoteEndTime = 0;
            StartBattleVoteIntervalTime = 0;
            FightVoteAgreeBySwitchFightScene = false;
            IsInStartBattleVotingTime = false;
        }

        //战斗结束投票相关
        //可以强制投票结束的标志
        private void OnReceiveTeamCopyForceEndFlag(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyForceEndFlag teamCopyForceEndFlag = new TeamCopyForceEndFlag();
            teamCopyForceEndFlag.decode(msgData.bytes);

            //可以强制投票结束
            TeamDuplicationFightEndVoteFlag = true;

            //发送一个消息,展示按钮
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationFightEndVoteFlagMessage);
        }

        //发送战斗结束投票的消息
        public void OnSendTeamCopyForceEndReq()
        {
            var teamCopyForceEndReq = new TeamCopyForceEndReq();

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, teamCopyForceEndReq);
        }

        //战斗结束投票的返回
        public void OnReceiveTeamCopyForceEndRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyForceEndRes teamCopyForceEndRes = new TeamCopyForceEndRes();
            teamCopyForceEndRes.decode(msgData.bytes);

            if ((ProtoErrorCode)teamCopyForceEndRes.retCode != ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)teamCopyForceEndRes.retCode);
            }
        }

        //战斗结束投票开始的广播
        public void OnReceiveTeamCopyEndVoteNotify(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            ResetFightEndVoteData();

            var teamCopyForceEndVoteNotify = new TeamCopyForceEndVoteNotify();
            teamCopyForceEndVoteNotify.decode(msgData.bytes);

            FightEndVoteIntervalTime = (int)teamCopyForceEndVoteNotify.voteDurationTime;
            FightEndVoteEndTime = (int)teamCopyForceEndVoteNotify.voteEndTime;

            //不存在团本
            if (TeamDuplicationUtility.IsTeamDuplicationOwnerTeam() == false)
                return;

            //没有处在攻坚场景
            if (TeamDuplicationUtility.IsTeamDuplicationInFightScene() == false)
                return;

            if (TeamDuplicationUtility.IsTeamDuplicationInFightingStatus() == false)
                return;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationCloseRelationFrame);
            //只有在攻坚场景中打开界面
            TeamDuplicationUtility.OnOpenTeamDuplicationFightEndVoteFrame();
        }

        //自己投票
        public void OnSendTeamCopyForceEndVoteReq(bool isAgree)
        {
            var teamCopyForceEndVoteReq = new TeamCopyForceEndVoteReq();
            //0拒绝，非0同意
            teamCopyForceEndVoteReq.vote = 1;       //默认同意
            if (isAgree == false)
                teamCopyForceEndVoteReq.vote = 0;

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, teamCopyForceEndVoteReq);
        }

        //同步有人投票的结果
        public void OnReceiveTeamCopyForceEndMemberVoteNotify(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            var teamCopyForceEndMemberVote = new TeamCopyForceEndMemberVote();
            teamCopyForceEndMemberVote.decode(msgData.bytes);

            var roleGuid = teamCopyForceEndMemberVote.roleId;
            var vote = teamCopyForceEndMemberVote.vote;

            if (vote == 0)
            {
                //拒绝
                AddPlayerGuidToFightEndVoteRefuseList(roleGuid);

                //发送消息
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationFightEndVoteRefuseMessage,
                    roleGuid);
            }
            else
            {
                //同意战斗结束
                AddPlayerGuidToFightEndVoteAgreeList(roleGuid);

                //发送消息，更新UI
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationFightEndVoteAgreeMessage,
                    roleGuid);
            }
        }

        //投票结束的通知
        public void OnReceiveTeamCopyForceEndVoteResultNotify(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            var teamCopyForceEndVoteResult = new TeamCopyForceEndVoteResult();
            teamCopyForceEndVoteResult.decode(msgData.bytes);

            TeamDuplicationUtility.OnCloseTeamDuplicationFightEndVoteFrame();
            ResetFightEndVoteData();

            if (teamCopyForceEndVoteResult.result == 0)
            {
                //投票成功

                TeamDuplicationFightEndVoteFlag = false;

                //发送消息，投票结束
                UIEventSystem.GetInstance()
                    .SendUIEvent(EUIEventID.OnReceiveTeamDuplicationFightEndVoteResultSucceedMessage);
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_fight_end_vote_succeed"));

            }
            else
            {
                //投票失败
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_fight_end_vote_fail"));
            }
        }

        //添加拒绝战斗结束的角色Id
        private void AddPlayerGuidToFightEndVoteRefuseList(ulong guid)
        {
            if (guid == 0)
                return;

            if (FightEndVoteRefuseList == null)
                FightEndVoteRefuseList = new List<ulong>();

            var isFind = false;
            for (var i = 0; i < FightEndVoteRefuseList.Count; i++)
            {
                if (FightEndVoteRefuseList[i] == guid)
                {
                    isFind = true;
                    break;
                }
            }

            if (isFind == false)
                FightEndVoteRefuseList.Add(guid);

        }

        //添加同意战斗结束的角色ID
        private void AddPlayerGuidToFightEndVoteAgreeList(ulong guid)
        {
            if (guid == 0)
                return;

            if (FightEndVoteAgreeList == null)
                FightEndVoteAgreeList = new List<ulong>();

            var isFind = false;
            for (var i = 0; i < FightEndVoteAgreeList.Count; i++)
            {
                if (FightEndVoteAgreeList[i] == guid)
                {
                    isFind = true;
                    break;
                }
            }

            if (isFind == false)
                FightEndVoteAgreeList.Add(guid);
        }

        //结束投票数据重置
        private void ResetFightEndVoteData()
        {
            if (FightEndVoteAgreeList != null)
                FightEndVoteAgreeList.Clear();

            if(FightEndVoteRefuseList != null)
                FightEndVoteRefuseList.Clear();

            FightEndVoteEndTime = 0;
            FightEndVoteIntervalTime = 0;
        }
        #endregion

        #region TeamDuplicationFightPanelData

        //团本阶段数据同步
        private void OnReceiveTeamCopyStageNotify(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyStageNotify teamCopyStageNotify = new TeamCopyStageNotify();
            teamCopyStageNotify.decode(msgData.bytes);

            ResetTeamDuplicationFightPanelData();

            UpdateTeamDuplicationFightPanelData(teamCopyStageNotify);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationFightStageNotifyMessage);
        }

        //团本阶段数据更新
        private void UpdateTeamDuplicationFightPanelData(TeamCopyStageNotify teamCopyStageNotify)
        {
            if (teamCopyStageNotify == null)
                return;

            TeamDuplicationFightStageId = (int) teamCopyStageNotify.stageId;
            TeamDuplicationFightFinishTime = teamCopyStageNotify.gameOverTime;

            if (TeamDuplicationCaptainFightGoalDataModel == null)
                TeamDuplicationCaptainFightGoalDataModel = new TeamDuplicationFightGoalDataModel();
            TeamDuplicationUtility.UpdateTeamDuplicationFightGoalDataModel(
                TeamDuplicationCaptainFightGoalDataModel,
                teamCopyStageNotify.squadTarget);

            if (TeamDuplicationTeamFightGoalDataModel == null)
                TeamDuplicationTeamFightGoalDataModel = new TeamDuplicationFightGoalDataModel();
            TeamDuplicationUtility.UpdateTeamDuplicationFightGoalDataModel(TeamDuplicationTeamFightGoalDataModel,
                teamCopyStageNotify.teamTarget);

            if (TeamDuplicationFightPointDataModelList == null)
                TeamDuplicationFightPointDataModelList = new List<TeamDuplicationFightPointDataModel>();
            TeamDuplicationFightPointDataModelList.Clear();

            for (var i = 0; i < teamCopyStageNotify.feildList.Length; i++)
            {
                var teamCopyField = teamCopyStageNotify.feildList[i];
                if (teamCopyField != null)
                {
                    TeamDuplicationFightPointDataModel fightPointDataModel = new TeamDuplicationFightPointDataModel();

                    TeamDuplicationUtility.UpdateTeamDuplicationFightPointDataModel(fightPointDataModel,
                        teamCopyField);

                    TeamDuplicationFightPointDataModelList.Add(fightPointDataModel);
                }
            }

            UpdateFightPointEnergyAccumulationRelationDataModel();
        }

        //团本据点数据更新
        private void OnReceiveTeamCopyFieldNotify(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyFieldNotify teamCopyFieldNotify = new TeamCopyFieldNotify();
            teamCopyFieldNotify.decode(msgData.bytes);

            if (teamCopyFieldNotify.feildList == null
                || teamCopyFieldNotify.feildList.Length <= 0)
                return;

            if (TeamDuplicationFightPointDataModelList == null)
                TeamDuplicationFightPointDataModelList = new List<TeamDuplicationFightPointDataModel>();

            for (var i = 0; i < teamCopyFieldNotify.feildList.Length; i++)
            {
                var teamCopyField = teamCopyFieldNotify.feildList[i];
                if(teamCopyField == null)
                    continue;

                bool isFind = false;
                for (var j = 0; j < TeamDuplicationFightPointDataModelList.Count; j++)
                {
                    var curFightPointDataModel = TeamDuplicationFightPointDataModelList[j];
                    if(curFightPointDataModel == null)
                        continue;

                    if (curFightPointDataModel.FightPointId == (int) teamCopyField.feildId)
                    {
                        isFind = true;
                        
                        //判断据点的状态是否由解锁变为非解锁
                        if (TeamDuplicationUtility.IsFightPointFinishUnlocking(
                            curFightPointDataModel.FightPointStatusType,
                            (TeamCopyFieldStatus) teamCopyField.state) == true)
                        {
                            //解锁飘字
                            TeamDuplicationUtility.ShowUnLockFightPointNameTips((int)teamCopyField.feildId);
                            //对据点解锁中的进度进行更新
                            ResetTeamDuplicationFightPointUnlockRateByFightPointId((int) teamCopyField.feildId);
                        }

                        //更新据点的数据
                        TeamDuplicationUtility.UpdateTeamDuplicationFightPointDataModel(
                            curFightPointDataModel,
                            teamCopyField);

                        break;
                    }
                }

                //没有找到，新出现的据点
                if (isFind == false)
                {
                    TeamDuplicationFightPointDataModel fightPointDataModel = new TeamDuplicationFightPointDataModel();

                    TeamDuplicationUtility.UpdateTeamDuplicationFightPointDataModel(fightPointDataModel,
                        teamCopyField);

                    TeamDuplicationFightPointDataModelList.Add(fightPointDataModel);

                    //新的据点处在非解锁状态中,进行飘字提示
                    if (fightPointDataModel.FightPointStatusType !=
                        TeamCopyFieldStatus.TEAM_COPY_FIELD_STATUS_UNLOCKING)
                    {
                        TeamDuplicationUtility.ShowUnLockFightPointNameTips((int)teamCopyField.feildId);
                    }
                }
            }
            
            UpdateFightPointEnergyAccumulationRelationDataModel();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationFightPointNotifyMessage);
        }

        //能量蓄积据点相关数据进行更新
        private void UpdateFightPointEnergyAccumulationRelationDataModel()
        {
            //查找能量蓄积状态的据点
            TeamDuplicationFightPointDataModel energyAccumulationFightPoint = null;
            if (TeamDuplicationFightPointDataModelList != null && TeamDuplicationFightPointDataModelList.Count > 0)
            {
                for (var i = 0; i < TeamDuplicationFightPointDataModelList.Count; i++)
                {
                    var curFightPointDataModel = TeamDuplicationFightPointDataModelList[i];

                    if (curFightPointDataModel == null)
                        continue;
                    if (curFightPointDataModel.FightPointStatusType
                        != TeamCopyFieldStatus.TEAM_COPY_FIELD_STATUS_ENERGY_REVIVE)
                        continue;

                    //处在能量蓄积状态的据点
                    energyAccumulationFightPoint = curFightPointDataModel;
                    break;
                }
            }

            //没有找到能量蓄积的据点
            if (energyAccumulationFightPoint == null)
            {
                FightPointEnergyAccumulationRelationDataModel = null;
            }
            else
            {
                //能量蓄积相关数据结构不存在，创建新的
                if (FightPointEnergyAccumulationRelationDataModel == null)
                {
                    FightPointEnergyAccumulationRelationDataModel =
                        TeamDuplicationUtility.GetEnergyAccumulationFightPointDataModel();
                    if (FightPointEnergyAccumulationRelationDataModel != null)
                    {
                        FightPointEnergyAccumulationRelationDataModel.IsBeginEnergyAccumulated = true;
                        FightPointEnergyAccumulationRelationDataModel.EnergyAccumulationStartTime =
                            energyAccumulationFightPoint.FightPointEnergyAccumulationStartTime;
                    }
                }
                else
                {
                    //如果能量蓄积开始时间改变，进行时间的更新
                    if (energyAccumulationFightPoint.FightPointEnergyAccumulationStartTime
                        != FightPointEnergyAccumulationRelationDataModel.EnergyAccumulationStartTime)
                    {
                        FightPointEnergyAccumulationRelationDataModel.EnergyAccumulationStartTime =
                            energyAccumulationFightPoint.FightPointEnergyAccumulationStartTime;
                        FightPointEnergyAccumulationRelationDataModel.IsBeginEnergyAccumulated = true;
                        FightPointEnergyAccumulationRelationDataModel.TimeUpdateInterval = 0.0f;
                    }
                }
            }

        }

        //团本据点状态更新， 进行飘字提示
        private void OnReceiveTeamCopyFieldStatusNotify(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyFieldStatusNotify teamCopyFieldStatusNotify = new TeamCopyFieldStatusNotify();
            teamCopyFieldStatusNotify.decode(msgData.bytes);

            if (teamCopyFieldStatusNotify.fieldId <= 0)
                return;

            //非歼灭状态，直接返回
            if ((TeamCopyFieldStatus)teamCopyFieldStatusNotify.fieldStatus !=
                TeamCopyFieldStatus.TEAM_COPY_FIELD_STATUS_DEFEAT)
            {
                return;
            }

            //据点被歼灭进行飘字提示
            var teamCopyFieldTable = TableManager.GetInstance().GetTableItem<TeamCopyFieldTable>(
                (int)teamCopyFieldStatusNotify
                    .fieldId);

            if (teamCopyFieldTable == null)
                return;

            var showContentStr = string.Format(TR.Value("team_duplication_fight_point_finished"),
                teamCopyFieldTable.Name);

            SystemNotifyManager.SysNotifyFloatingEffect(showContentStr);
        }

        //团本目标更新
        private void OnReceiveTeamCopyTargetNotify(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyTargetNotify teamCopyTargetNotify = new TeamCopyTargetNotify();
            teamCopyTargetNotify.decode(msgData.bytes);

            if (TeamDuplicationCaptainFightGoalDataModel == null)
                TeamDuplicationCaptainFightGoalDataModel = new TeamDuplicationFightGoalDataModel();

            //小队目标是否改变
            var isCaptainGoalIdChange = TeamDuplicationCaptainFightGoalDataModel != null
                                         && TeamDuplicationCaptainFightGoalDataModel.FightGoalId
                                         != teamCopyTargetNotify.squadTarget.targetId;

            //小队目标数据更新
            TeamDuplicationUtility.UpdateTeamDuplicationFightGoalDataModel(TeamDuplicationCaptainFightGoalDataModel,
                teamCopyTargetNotify.squadTarget);

            //小队目标ID改变，发送UI事件
            if (isCaptainGoalIdChange == true)
            {
                UIEventSystem.GetInstance()
                    .SendUIEvent(EUIEventID.OnReceiveTeamDuplicationFightCaptainGoalChangeMessage);
            }

            //团队目标数据更改
            if (TeamDuplicationTeamFightGoalDataModel == null)
                TeamDuplicationTeamFightGoalDataModel = new TeamDuplicationFightGoalDataModel();
            TeamDuplicationUtility.UpdateTeamDuplicationFightGoalDataModel(TeamDuplicationTeamFightGoalDataModel,
                teamCopyTargetNotify.teamTarget);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationFightGoalNotifyMessage);
        }

        //团本据点解锁的刷新
        private void OnReceiveTeamCopyFieldUnlockRate(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyFieldUnlockRate teamCopyFieldUnLockRate = new TeamCopyFieldUnlockRate();
            teamCopyFieldUnLockRate.decode(msgData.bytes);

            int fightPointId = (int) teamCopyFieldUnLockRate.fieldId;
            int unlockRate = (int) teamCopyFieldUnLockRate.unlockRate;

            //没有创建，存在改变
            TeamDuplicationFightPointUnLockRateDictionary[fightPointId] = unlockRate;

            //对应的boss衍生据点和boss相关的血量
            BossPhase = (int)teamCopyFieldUnLockRate.bossPhase;
            BossBloodRate = (int)teamCopyFieldUnLockRate.bossBloodRate;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationFightPointUnlockRateMessage, fightPointId);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationFightPointBossDataMessage);

        }


        //当某个据点已经完成解锁之后，对据点的数据进行重置
        public void ResetTeamDuplicationFightPointUnlockRateByFightPointId(int fightPointId)
        {
            if (fightPointId <= 0)
                return;

            if (TeamDuplicationFightPointUnLockRateDictionary == null
                || TeamDuplicationFightPointUnLockRateDictionary.Count <= 0)
                return;

            if (TeamDuplicationFightPointUnLockRateDictionary.ContainsKey(fightPointId) == false)
                return;
            
            //解锁Rate重置为0；
            TeamDuplicationFightPointUnLockRateDictionary[fightPointId] = 0;
        }

        //某个据点的解锁进度
        public int GetTeamDuplicationFightPointUnlockRateByFightPointId(int fightPointId)
        {
            if (fightPointId <= 0)
                return 0;
            if (TeamDuplicationFightPointUnLockRateDictionary == null ||
                TeamDuplicationFightPointUnLockRateDictionary.Count <= 0)
                return 0;

            if (TeamDuplicationFightPointUnLockRateDictionary.ContainsKey(fightPointId) == false)
                return 0;

            return TeamDuplicationFightPointUnLockRateDictionary[fightPointId];
        }

        //得到根据前置据点得到当前据点的解锁情况
        public int GetTeamDuplicationFightPointUnlockRateByPreFightPointId(int preFightPointId)
        {
            if (TeamDuplicationFightPointDataModelList == null 
                || TeamDuplicationFightPointDataModelList.Count <= 0)
                return 0;

            for (var i = 0; i < TeamDuplicationFightPointDataModelList.Count; i++)
            {
                var fightPointDataModel = TeamDuplicationFightPointDataModelList[i];
                if(fightPointDataModel == null)
                    continue;

                if(fightPointDataModel.FightPointId != preFightPointId)
                    continue;

                //完成次数
                var finishFightNumber = (fightPointDataModel.FightPointTotalFightNumber -
                              fightPointDataModel.FightPointLeftFightNumber);
                //完成比率
                var unlockRate = 0;
                if(fightPointDataModel.FightPointTotalFightNumber != 0)
                    unlockRate = finishFightNumber * (100 / fightPointDataModel.FightPointTotalFightNumber);

                return unlockRate;
            }

            return 0;
        }
        
        private void ResetTeamDuplicationFightPanelData()
        {
            TeamDuplicationFightStageId = 0;
            TeamDuplicationFightFinishTime = 0;
            if(TeamDuplicationFightPointDataModelList != null)
                TeamDuplicationFightPointDataModelList.Clear();

            if (TeamDuplicationCaptainFightGoalDataModel != null)
            {
                TeamDuplicationCaptainFightGoalDataModel.FightGoalId = 0;
                if(TeamDuplicationCaptainFightGoalDataModel.FightGoalDetailDataModelList != null)
                    TeamDuplicationCaptainFightGoalDataModel.FightGoalDetailDataModelList.Clear();
            }

            if (TeamDuplicationTeamFightGoalDataModel != null)
            {
                TeamDuplicationTeamFightGoalDataModel.FightGoalId = 0;
                if(TeamDuplicationTeamFightGoalDataModel.FightGoalDetailDataModelList != null)
                    TeamDuplicationTeamFightGoalDataModel.FightGoalDetailDataModelList.Clear();
            }

            //据点蓄积相关数据
            FightPointEnergyAccumulationRelationDataModel = null;
        }
        
        #endregion

        #region TeamDuplicationFightStartChallenge
        //进入游戏开始挑战

        //开始挑战
        //据点Id
        public void OnSendTeamCopyStartChallengeReq(int fightPointId)
        {
            TeamCopyStartChallengeReq startChallengeReq = new TeamCopyStartChallengeReq();

            startChallengeReq.fieldId = (uint)fightPointId;
            
            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, startChallengeReq);

        }

        private void OnReceiveTeamCopyStartChallengeRes(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyStartChallengeRes startChallengeRes = new TeamCopyStartChallengeRes();
            startChallengeRes.decode(msgData.bytes);

            if (startChallengeRes.retCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)startChallengeRes.retCode);
                return;
            }
        }

        //阶段结束进行提示
        private void OnReceiveTeamCopyStageEndNotify(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            TeamCopyStageEnd teamCopyStageEnd = new TeamCopyStageEnd();
            teamCopyStageEnd.decode(msgData.bytes);

            //记录战斗战斗阶段完成Id
            TeamDuplicationEndStageId = (int) teamCopyStageEnd.stageId;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationFightStageEndNotifyMessage);
        }


        #endregion

        #region TeamDuplicationFightStageReward

        //发送阶段奖励翻牌的协议
        public void OnSendTeamCopyTargetFlopReq(int stageId)
        {
            TeamCopyStageFlopReq targetFlopReq = new TeamCopyStageFlopReq();
            //阶段和牌的序号
            targetFlopReq.stageId = (uint) stageId;

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, targetFlopReq);
        }

        //阶段翻牌的返回
        private void OnReceiveTeamCopyStageFlopRes(MsgDATA msgData)
        {

            if (msgData == null)
                return;

            TeamCopyStageFlopRes teamCopyStageFlopRes = new TeamCopyStageFlopRes();
            teamCopyStageFlopRes.decode(msgData.bytes);

            if (teamCopyStageFlopRes.flopList == null
                || teamCopyStageFlopRes.flopList.Length <= 0)
                return;

            var stageId = (int) teamCopyStageFlopRes.stageId;

            TeamDuplicationFightStageRewardDataModelList.Clear();

            for (var i = 0; i < teamCopyStageFlopRes.flopList.Length; i++)
            {
                var teamCopyFlop = teamCopyStageFlopRes.flopList[i];

                if(teamCopyFlop == null)
                    continue;

                TeamDuplicationFightStageRewardDataModel fightStageRewardDataModel =
                    new TeamDuplicationFightStageRewardDataModel();
                TeamDuplicationUtility.UpdateFightStageRewardDataModel(teamCopyFlop,
                    fightStageRewardDataModel);
                TeamDuplicationFightStageRewardDataModelList.Add(fightStageRewardDataModel);

            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationFightStageRewardNotify,
                stageId);
        }

        //重置阶段奖励
        public void ResetFightStageRewardDataModelList()
        {
            if(TeamDuplicationFightStageRewardDataModelList != null)
                TeamDuplicationFightStageRewardDataModelList.Clear();
        }

        #endregion

        #region TeamDuplicationServerFuncSwitch
        //Server开关更新
        private void OnServerFuncSwitch(ServerSceneFuncSwitch funcSwitch)
        {
            //非团本开关
            if (funcSwitch.sType != ServiceType.SERVICE_TEAM_COPY)
                return;
            //团本开关，发送团本开关更新的消息
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationServerFuncSwitchChangeMessage);
        }
        #endregion

        #region Helper

        //得到小队难度的配置
        //共四个小队
        public List<TeamDuplicationTeamDifficultyConfigDataModel> GetTeamConfigDataModelList()
        {
            return _teamConfigDataModelList;
        }

        //初始化小队难度
        public void InitTeamConfigDataModelList()
        {
            if (_teamConfigDataModelList == null)
                return;
            _teamConfigDataModelList.Clear();

            for (var i = 0; i < TeamDuplicationCaptainNumber; i++)
            {
                TeamDuplicationTeamDifficultyConfigDataModel teamConfigDataModel =
                    new TeamDuplicationTeamDifficultyConfigDataModel();
                teamConfigDataModel.Difficulty = (TeamCopyGrade)(i + 1);
                teamConfigDataModel.TeamId = 0;
                _teamConfigDataModelList.Add(teamConfigDataModel);
            }
        }

        public bool IsTeamDuplicationTeamDifficultyConfigNotSet()
        {
            if (_teamConfigDataModelList == null || _teamConfigDataModelList.Count <= 0)
                return true;

            return false;
        }

        //保存难度
        public void UpdateTeamConfigDataModelList(
            List<TeamDuplicationTeamDifficultyConfigDataModel> saveTeamConfigDataModelList)
        {
            if (saveTeamConfigDataModelList == null || _teamConfigDataModelList == null
                                                    || saveTeamConfigDataModelList.Count <= 0
                                                    || _teamConfigDataModelList.Count <= 0)
                return;

            var saveTeamConfigNumber = saveTeamConfigDataModelList.Count;
            var curTeamConfigNumber = _teamConfigDataModelList.Count;

            for (var i = 0; i < saveTeamConfigNumber && i < curTeamConfigNumber; i++)
            {
                var saveTeamConfigDataModel = saveTeamConfigDataModelList[i];
                var teamConfigDataModel = _teamConfigDataModelList[i];
                if (teamConfigDataModel != null && saveTeamConfigDataModel != null)
                {
                    teamConfigDataModel.Difficulty = saveTeamConfigDataModel.Difficulty;
                    teamConfigDataModel.TeamId = saveTeamConfigDataModel.TeamId;
                }
            }
        }

        //重置难度配置
        public void ResetTeamConfigDataModelList()
        {
            if (_teamConfigDataModelList != null && _teamConfigDataModelList.Count > 0)
                _teamConfigDataModelList.Clear();
        }

        //队伍列表的数据
        public List<TeamDuplicationTeamListDataModel> GetTeamDuplicationTeamListDataModelList()
        {
            return _teamListDataModelList;
        }

        #endregion

        #region ClearData

        //退出团本，清空相关的数据
        private void ResetTeamDuplicationDataByQuitTeam()
        {
            IsTeamDuplicationOwnerTeam = false;
            IsAlreadyReceiveFinalReward = false;

            ResetTeamDataModel();
            ResetTeamDuplicationFightPanelData();

            FightSettingConfigPlanModel = TeamCopyPlanModel.TEAM_COPY_PLAN_MODEL_INVALID;
            ResetTeamConfigDataModelList();

            ResetFightStartVoteData();
            ResetFightEndVoteData();
            TeamDuplicationFightEndVoteFlag = false;

            IsAlreadyShowPositionAdjustTip = false;
            ResetTeamDuplicationReconnectData();
            IsTeamDuplicationOwnerNewRequester = false;
            TeamDuplicationEndStageId = 0;
            IsNeedShowGameFailResult = false;
            TeamDuplicationPlayerExpireTimeDic.Clear();

            TeamDuplicationFightPointUnLockRateDictionary.Clear();
            BossPhase = 0;
            BossBloodRate = 0;
        }

        //重置其他队伍数据
        public void ResetOtherTeamDataModel()
        {
            if (OtherTeamDataModel == null)
                return;

            OtherTeamDataModel.TeamId = 0;
            OtherTeamDataModel.TeamName = "";
            OtherTeamDataModel.TotalCommission = 0;
            OtherTeamDataModel.BonusCommission = 0;

            if (OtherTeamDataModel != null && OtherTeamDataModel.CaptainDataModelList != null)
            {
                for (var i = 0; i < OtherTeamDataModel.CaptainDataModelList.Count; i++)
                {
                    var troopDataModel = OtherTeamDataModel.CaptainDataModelList[i];
                    if (troopDataModel != null && troopDataModel.PlayerList != null)
                    {
                        troopDataModel.PlayerList.Clear();
                    }
                }
                OtherTeamDataModel.CaptainDataModelList.Clear();
            }
        }

        //清空团本数据
        private void ResetTeamDataModel()
        {
            if (_teamDataModel == null)
                return;

            _teamDataModel.TeamId = 0;
            _teamDataModel.TeamName = "";
            _teamDataModel.TotalCommission = 0;
            _teamDataModel.BonusCommission = 0;

            if (_teamDataModel != null && _teamDataModel.CaptainDataModelList != null)
            {
                for (var i = 0; i < _teamDataModel.CaptainDataModelList.Count; i++)
                {
                    var troopDataModel = _teamDataModel.CaptainDataModelList[i];
                    if (troopDataModel != null && troopDataModel.PlayerList != null)
                    {
                        troopDataModel.PlayerList.Clear();
                    }
                }
                _teamDataModel.CaptainDataModelList.Clear();
            }
        }

        private void ResetTeamListDataModelList()
        {
            if (_teamListDataModelList != null)
                _teamListDataModelList.Clear();
        }
        #endregion

        #region Update
        public override void Update(float time)
        {
            UpdateFightPointEnergyAccumulationStatus(time);
        }

        //能量蓄积状态的据点相关数据进行更新
        private void UpdateFightPointEnergyAccumulationStatus(float time)
        {
            //不存在能量蓄积的据点
            if (FightPointEnergyAccumulationRelationDataModel == null
                || FightPointEnergyAccumulationRelationDataModel.EnergyAccumulationStartTime <= 0)
                return;

            //能量蓄积已经完成，达到了100%，直接返回，不在刷新
            if (FightPointEnergyAccumulationRelationDataModel.IsBeginEnergyAccumulated == false)
                return;

            FightPointEnergyAccumulationRelationDataModel.TimeUpdateInterval += time;
            //1s，时间刷新
            if (FightPointEnergyAccumulationRelationDataModel.TimeUpdateInterval >= 1.0f)
            {
                //timeInterval 重置
                FightPointEnergyAccumulationRelationDataModel.TimeUpdateInterval = 0;

                var intervalTime = (int)TimeManager.GetInstance().GetServerTime() -
                                FightPointEnergyAccumulationRelationDataModel.EnergyAccumulationStartTime;

                //能量蓄积，超过100%，不再刷新 (避免重复刷新)
                if (intervalTime >
                    (FightPointEnergyAccumulationRelationDataModel.FightPointEnergyAccumulation100 + 1))
                {
                    FightPointEnergyAccumulationRelationDataModel.IsBeginEnergyAccumulated = false;
                }

                //只有城镇的团本场景中，才飘字和刷新，不在城镇，直接返回
                bool isInTeamDuplicationScene = TeamDuplicationUtility.IsInTeamDuplicationScene();
                if (isInTeamDuplicationScene == false)
                    return;


                //不同阶段进行飘字
                if (intervalTime == FightPointEnergyAccumulationRelationDataModel.FightPointEnergyAccumulation30)
                {
                    //飘字 30%
                    SystemNotifyManager.SysNotifyFloatingEffect(
                        TR.Value("team_duplication_fight_point_energy_accumulation_thirty_percent"));
                }
                else if (intervalTime == FightPointEnergyAccumulationRelationDataModel.FightPointEnergyAccumulation50)
                {
                    //飘字 50%
                    SystemNotifyManager.SysNotifyFloatingEffect(
                        TR.Value("team_duplication_fight_point_energy_accumulation_fifty_percent"));
                }
                else if (intervalTime == FightPointEnergyAccumulationRelationDataModel.FightPointEnergyAccumulation80)
                {
                    //飘字 80%
                    SystemNotifyManager.SysNotifyFloatingEffect(
                        TR.Value("team_duplication_fight_point_energy_accumulation_eighty_percent"));
                }
                else if (intervalTime == FightPointEnergyAccumulationRelationDataModel.FightPointEnergyAccumulation100)
                {
                    //飘字 100%
                    SystemNotifyManager.SysNotifyFloatingEffect(
                        TR.Value("team_duplication_fight_point_energy_accumulation_one_hundred_percent"));
                }

                UIEventSystem.GetInstance()
                    .SendUIEvent(EUIEventID.OnReceiveTeamDuplicationFightPointEnergyAccumulationTimeMessage);
            }
        }
        #endregion
    }
}
