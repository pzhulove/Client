using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;

namespace GameClient
{

    //动画类型
    public enum DoTweenAnimationFadeType
    {
        FadeOut = 0,
        FadeIn = 1,
    }

    //团队的类型：团队or小队
    public enum TeamDuplicationTeamCaptainType
    {
        None = 0,
        Team = 1,             //团队
        Captain = 2,           //小队
    }

    public enum TeamDuplicationPlayerType
    {
        None = 0,
        TeamLeader = 1, //团长
        Captain = 2, //队长
        Player = 3, //队员
    }

    //创建团队的数据模型
    public class TeamDuplicationTeamBuildDataModel
    {
        public uint TeamDifficultyLevel;
        public int TeamModelType;

        //是否重新设置装备评分
        public bool IsResetEquipmentScore;
        public int OwnerEquipmentScore;
    }

    //团队中个人的数据
    public class TeamDuplicationPlayerInformationDataModel
    {
        //由服务器同步
        public int DayAlreadyFightNumber;          //今日已经挑战的次数
        public int WeekAlreadyFightNumber;         //每周已经挑战的次数
        public bool IsCanCreateGold;        //是否可以创建金团，
        //可以免费退的次数
        public int DayFreeQuitNumber;       //每日免费退出的次数
        public int WeekFreeQuitNumber;      //每周免费退出的次数

        public int DayTotalFightNumber;          //今日可以挑战的总次数
        public int WeekTotalFightNumber;    //每周可以挑战的总次数

        public int HardLevelAlreadyFightNumber;     //噩梦已经挑战次数
        public int HardLevelTotalFightNumber;       //噩梦总的挑战次数

        public int CommonLevelPassNumber;           //普通难度通关次数
        public int UnLockCommonLevelTotalNumber;          //解锁噩梦难度需要通关的总次数

        public bool TicketIsEnough;         //门票是否足够

        //已经开放的难度
        public List<uint> AlreadyOpenDifficultyList = new List<uint>();
    }

    //查看其他人的权限
    public enum TeamDuplicationPermissionType
    {
        None = 0,
        PermissionTeamLeader = 1, //团长权限
        PermissionCaptain = 2, //队长权限
        PermissionPlayer = 3, //队员权限
    }

    //战斗目标的类型
    public enum TeamDuplicationFightGoalType
    {
        None = 0,
        CaptainGoal = 1,             //小队目标
        TeamDuplicationGoal = 2,           //团本目标
        FightPointDescription = 3,             //据点描述
    }

    //团队的数据模型
    public class TeamDuplicationTeamDataModel
    {
        public int TeamId;
        public string TeamName;
        public int TotalCommission;
        public int BonusCommission;
        public bool AutoAgreeGold;          //自动同意金主入团的标志，只有团长有效
        public int TeamModel;               //团本的类型
        public TeamCopyTeamStatus TeamStatus;       //团本的状态
        public uint TeamDifficultyLevel;        //团本的难度
        public int TeamEquipScore;          //团本的装备评分

        //小队的列表
        public List<TeamDuplicationCaptainDataModel> CaptainDataModelList = new List<TeamDuplicationCaptainDataModel>();

    }

    //小队的数据模型
    public class TeamDuplicationCaptainDataModel
    {
        public int CaptainId;      //小队的ID
        public List<TeamDuplicationPlayerDataModel> PlayerList = new List<TeamDuplicationPlayerDataModel>();
        public int CaptainStatus;
    }

    public class TeamDuplicationTeamRoomDataModel
    {
        public int TeamId;      //队伍ID
    }

    //团本中角色的数据模型
    public class TeamDuplicationPlayerDataModel
    {
        public ulong Guid;          //guid
        public string Name;         //名字
        public int Level;
        public int ProfessionId;    //职业ID
        public int PlayerAwakeState;    //觉醒状态
        public int HeadFrameId;     //头像框的ID
        public bool IsTeamLeader;               //是否为团长
        public bool IsCaptain;              //是否为队长
        public bool IsGoldOwner;    //是否为金主
        public int TeamIndex;       //属于第几小队
        public int TicketNumber;    //金币数量
        public int EquipmentScore;  //装备评分
        public int SeatId;          //座位ID
        public bool TicketIsEnough;         //门票是否足够
        public int ZoneId;              //服务器ID
        public ulong ExpireTime;        //过期时间
    }

    public enum TeamDuplicationRequesterType
    {
        None = -1,
        Normal = 0,
        Friend = 1,
        Guild = 2,      //工会
    }

    //申请者的数据模型
    public class TeamDuplicationRequesterDataModel
    {
        public ulong PlayerGuid;          //guid;
        public string Name;         //名字
        public int Level;           //等级
        public int ProfessionId;    //职业ID
        public int PlayerAwakeState;    //觉醒状态
        public int EquipmentScore;  //装备评分
        public byte RequesterType;  //请求者类型，好友，工会等
        public bool IsGold;         //是否为金主
        public bool IsFriend;       //是否为好友
        public ulong GuildId;       //工会Id;
        public bool IsGuild;        //是否为工会
        public int ZoneId;          //服务器的ID
    }


    //查看权限的两个角色
    public class TeamDuplicationPermissionDataModel
    {
        //自己角色的数据
        public TeamDuplicationPlayerDataModel OwnerPlayerDataModel;
        //选择角色的数据
        public TeamDuplicationPlayerDataModel SelectedPlayerDataModel;
        
        //点击按钮的屏幕坐标
        public Vector2 ClickScreenPosition;
    }


    //队伍属性的类型
    public enum TeamDuplicationTeamPropertyType
    {
        None = -1,
        EquipmentScoreType = 1,     //装备评分类型
        GoldValueType = 2,          //佣金类型
    }

    //队伍属性的数据结构
    public class TeamDuplicationTeamPropertyDataModel
    {
        public TeamCopyTeamModel TeamType;
        public TeamDuplicationTeamPropertyType TeamPropertyType;
        public int Value;
    }


    //队伍的数据模型
    public class TeamDuplicationTeamListDataModel
    {
        public uint TeamId;         //队伍ID
        public TeamCopyTeamModel TeamType;
        public int GoldValue;
        public string TeamName;
        public uint TeamHardLevel;      //团队的难度等级
        public int TeamNumber;
        public int EquipmentScore;
        public int TroopStatus;
        public int RequestType;
    }

    //队伍邀请者数据模型
    public class TeamDuplicationTeamInviteDataModel
    {
        public uint TeamId;
        public TeamCopyTeamModel TeamType;           //团本类型
        public string TeamLeaderName;               //团长名字
        public int TeamLeaderProfessionId;          //团长职业ID
        public int HeadFrameId;     //头像框的ID
        public int TeamLeaderAwakeState;            //团长的觉醒状态
        public int TeamLeaderLevel;                 //团长等级
        public uint TeamDifficultyLevel;        //团本的难度
    }

    public enum TeamDuplicationFightPointStatusType
    {
        None = -1,
        UnLock,             //未解锁
        Challenge,          //可挑战
        Selected,           //选中状态
        Reborn,             //重生状态
        Emergency,          //紧急状态
        Killed,             //击杀状态

        Num,
    }

    //战斗面板的据点数据模型
    public class TeamDuplicationFightPointDataModel
    {
        public int FightPointId;                        //据点ID
        public TeamCopyFieldStatus FightPointStatusType;        //据点状态
        public List<uint> FightPointTeamList;                //小队数量
        public int FightPointLeftFightNumber;                   //剩余挑战次数
        public int FightPointRebornTime;                    //重生的时间
        public int FightPointEnergyAccumulationStartTime;       //据点能量蓄积开始时间

        //从团本据点表中读取
        //位置
        public int FightPointPosition;
        //总共挑战次数
        public int FightPointTotalFightNumber;
    }

    //团本目标的数据模型
    public class TeamDuplicationFightGoalDataModel
    {
        //目标ID
        public int FightGoalId;

        //目标详情
        public List<TeamDuplicationFightGoalDetailDataModel> FightGoalDetailDataModelList =
            new List<TeamDuplicationFightGoalDetailDataModel>();
    }

    //战斗目标的详情
    public class TeamDuplicationFightGoalDetailDataModel
    {
        //据点ID
        public int FightPointId;
        
        //当前次数
        public int FightPointCurrentNumber;

        //总次数
        public int FightPointTotalNumber;
    }

    //战斗据点位置
    [Serializable]
    public class TeamDuplicationFightPointItemWithPosition
    {
        public GameObject FightPointItemRoot;
        public int PositionIndex;
    }

    //小队难度的配置
    public class TeamDuplicationTeamDifficultyConfigDataModel
    {
        public TeamCopyGrade Difficulty;      //难度
        public int TeamId;          //小队Id
    }

    //阶段奖励的数据模型
    public class TeamDuplicationFightStageRewardDataModel
    {
        public ulong PlayerGuid;
        public string PlayerName;
        public int RewardId;    //奖励的ID
        public int RewardNum;   //奖励的数量
        public int RewardIndex; //奖励的索引
        public bool IsGoldReward;       //是否为金奖
        public TeamCopyFlopLimit IsLimit;             //是否限制：0：不限制，1：今天领完，2：本周领完
    }

    //阶段描述的数据
    public class TeamDuplicationFightStageDescriptionDataModel
    {
        public int StageNumber;
        public bool IsBegin;
    }

    //阶段奖励的位置
    [Serializable]
    public class TeamDuplicationFinalStageRewardItemWithPosition
    {
        public GameObject RewardItemRoot;
        public int PositionIndex;
    }

    //游戏的结果
    public enum TeamDuplicationGameResultType
    {
        None = -1,
        Failed = 0,             //失败
        Success = 1,            //胜利
    }

    //小队和小队的总评分
    public class TeamDuplicationCaptainEquipmentScore
    {
        public int CaptainId;               //小队Id
        public int TotalEquipmentScore;     //小队角色总评分 
    }

    public enum TeamDuplicationFightVoteType
    {
        None = -1,
        FightStartVote,         //战斗开始投票
        FightEndVote,           //战斗结束投票
    }


    //处在能量蓄积中的据点相关数据
    public class TeamDuplicationFightPointEnergyAccumulationDataModel
    {
        public bool IsBeginEnergyAccumulated;       //开始蓄积的标志
        public int EnergyAccumulationStartTime;
        public float TimeUpdateInterval;

        //据点回复相关数据
        public int FightPointEnergyAccumulation5;
        public int FightPointEnergyAccumulation30;
        public int FightPointEnergyAccumulation50;
        public int FightPointEnergyAccumulation80;
        public int FightPointEnergyAccumulation100;
    }

}
