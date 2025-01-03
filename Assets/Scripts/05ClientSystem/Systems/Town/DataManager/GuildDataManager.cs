using System;
using System.Collections.Generic;
using System.Text;
using Network;
using Protocol;
using System.ComponentModel;
using ProtoTable;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Reflection;

namespace GameClient
{
    using UIItemData = AwardItemData;

    /// <summary>
    ///公会兼并请求状态
    /// </summary>
    public enum EMergerRequestType
    {
        None=0,
        HaveSend=1, //发送
        HaveAccept =2,//接受
    }
    /// <summary>
    /// 公会的繁荣状态
    /// </summary>
    public enum EGuildProsperityState
    {
        None=0,
        Dismiss=1,//解散
        Low=2,    //低
        Mid=3,    //中
        Heigh=4   //高
    }
    /// <summary>
    /// 申请兼并类型
    /// </summary>
    public enum EMergerOpType
    {
        Apply=0,
        Cancel=1
    }
    public enum EGuildDuty
    {
        Invalid = 0,

        [Description("guild_duty_normal")]
        Normal,

        [Description("guild_duty_elite")]
        Elite,

        [Description("guild_duty_elder")]
        Elder,

        [Description("guild_duty_assistant")]
        Assistant,

        [Description("guild_duty_leader")]
        Leader,
    }

    public enum GuildBattleOpenType
    {
        GBOT_INVALID = 0, //  无效
        GBOT_NORMAL_CHALLENGE = 1,   // 普通公会战或攻城战界面
        GBOT_CROSS = 2, // 跨服公会战界面
        GBOT_GUILD_SCENE_MAIN = 3, // 公会战场景主界面
    }

    /// <summary>
    /// 公会权限
    /// 请不要修改权限枚举对应的值,也不要穿插着添加新的枚举值
    /// </summary>
    enum EGuildPermission
    {
        Invalid = 0,

        SetDutyNormal = 1 << EGuildDuty.Normal,           // 设置职位为普通
        SetDutyElder = 1 << EGuildDuty.Elder,            // 设置职位为长老
        SetDutyAssistant = 1 << EGuildDuty.Assistant,        // 设置职位为副会长
        SetDutyLeader = 1 << EGuildDuty.Leader,           // 设置职位为会长

        KickMember = 1 << 6,                                 // 踢出成员

        ProcessRequester = 1 << 12,                                // 审批请求者
        ChangeDeclaration = 1 << 13,                                // 修改宣言
        ChangeNotice = 1 << 14,                                // 修改公会公告
        ChangeName = 1 << 15,                                // 修改公会名
        SendMail = 1 << 16,                                // 公会邮件
        Dismiss = 1 << 17,                                // 解散帮会
        UpgradeBuilding = 1 << 18,                                // 升级建筑
        StartGuildBattle = 1 << 19,                                // 开启公会战
        StartGuildAttackCity = 1 << 20,                            // 开启攻城战
        StartGuildCrossBattle = 1 << 21,                    // 开启跨服公会战
        ChangeJoinLv = 1 << 22,                             // 修改入会等级限制
        SetGuildDungeonBossDiff = 1 << 23,                  // 设置公会副本boss难度
        GuildMeger=1<<24,                                    //公会合并

        // 不同职务的权限
        /// <summary>
        ///  普通成员
        /// </summary>
        NormalMask = Invalid,

        /// <summary>
        ///  精英
        /// </summary>
        EliteMask = Invalid,

        /// <summary>
        ///  长老
        /// </summary>
        ElderMask = KickMember | SendMail | ProcessRequester | ChangeJoinLv,

        /// <summary>
        ///  副会长
        /// </summary>
        AssistantMask = ElderMask | ChangeDeclaration | ChangeNotice | StartGuildBattle | SetDutyElder | SetDutyNormal | StartGuildAttackCity | StartGuildCrossBattle | ChangeJoinLv | SetGuildDungeonBossDiff,

        /// <summary>
        ///  会长
        /// </summary>
        LeaderMask = AssistantMask | UpgradeBuilding | ChangeName | Dismiss | SetDutyAssistant | SetDutyLeader | ChangeJoinLv | SetGuildDungeonBossDiff| GuildMeger,

    };

    enum EGuildBattleState
    {
        /// <summary>
        /// 无效
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// 报名
        /// </summary>
        Signup = 1,

        /// <summary>
        /// 准备中
        /// </summary>
        Preparing = 2,

        /// <summary>
        /// 战斗中
        /// </summary>
        Firing = 3,

        /// <summary>
        /// 抽奖阶段
        /// </summary>
        LuckyDraw = 4,
    }

    /// <summary>
    /// 公会基础数据
    /// </summary>
    class GuildData
    {
        public ulong uGUID;             // ID
        public string strName;          // 名称
        public int nLevel;              // 等级
        public int nMemberCount;        // 成员数
        public int nMemberMaxCount;     // 最大成员数量
        public string strDeclaration;   // 宣言（对外）
        public string strLeaderName;    // 会长名字
        public bool bHasApplied;        // 是否已经申请过（表示申请过入会或者是有被合并的请求）
        public string remark;           // 好友备注名称
        public ulong leaderID;          // 会长玩家id
        public int occupyCrossTerrId;   // 跨服领地id  
        public int occupyTerrId;        // 本服领地id  
        public uint joinLevel;           // 入会等级
    }

    /// <summary>
    /// 公会成员数据
    /// </summary>
    class GuildMemberData
    {
        public ulong uGUID;             // guid
        public int nJobID;              // 职业ID
        public string strName;          // 名字
        public int nLevel;              // 等级
        public EGuildDuty eGuildDuty;   // 公会职务
        public int nContribution;       // 贡献
        public uint uOffLineTime;       // 离线时间戳
        public uint uActiveDegree;      // 活跃度
        public string remark;           //备注名称
        public PlayerLabelInfo playerLabelInfo = new PlayerLabelInfo();
        public uint vipLevel;           // VIP等级
        public uint seasonLevel;        // 玩家段位
    }

    class GuildLeaderData : GuildMemberData
    {
        public uint nPopularity;            // 会长雕像人气
        public PlayerAvatar avatorInfo;     // 形象信息
    }

    /// <summary>
    /// 申请者数据
    /// </summary>
    class GuildRequesterData
    {
        public ulong uGUID;             // guid
        public int nJobID;              // 职业ID
        public string strName;          // 名字
        public int nLevel;              // 等级
        public string remark;           //备注名称
    }

    class GuildBuildingData
    {
        public delegate bool CheckRedPoint();

        public GuildBuildingType eType;     // 类型
        public int nLevel;                  // 等级
        public int nMaxLevel;               // 最大等级
        public int nUnlockMaincityLevel;    // 解锁等级上限需要的主城的等级
        public int nUpgradeCost;            // 升级消耗
    }

    class GuildDonateData
    {
        public ulong uGUID;             // 捐献者GUID
        public string strName;          // 捐献者名字
        public GuildDonateType eType;   // 捐献类型
        public int nTimes;              // 捐献次数
        public int nFund;               // 增加的公会资金
    }

    class GuildSkillData
    {
        public int nSkillID;
        public int nLevel;
        public int nCurrentMaxLevel;
        public int nFinalMaxLevel;
        public string strName;
        public string strIcon;
        public int nGoldCost;
        public int nContributionCost;
        public string strCurrentEffect;
        public string strNextEffect;
        public int nNextNeedBuildingLevel;
    }

    /// <summary>
    /// 我的公会数据
    /// </summary>
    class GuildMyData
    {
        public ulong uGUID;                             // ID
        public string strName;                          // 名称
        public int nLevel;                              // 等级
        public int nMemberCount;                        // 成员数量
        public int nMemberMaxCount;                     // 最大成员数量
        public int nFund;                               // 资金
        public string strDeclaration;                   // 宣言（对外）
        public string strNotice;                        // 公告（对内）
        public uint nDismissTime;                       // 解散时间戳
        public uint nExchangeCoolTime;                  // 福利社兑换冷却完成时间戳
        public int nTargetManorID;                      // 报名领地ID
        public int nTargetCrossManorID;                 // 报名跨服公会战领地ID
        public int nBattleScore;                        // 公会战分数
        public int nSelfManorID;                        // 公会占领领地
        public int nSelfCrossManorID;                   // 公会占领的跨服领地
        public int nSelfHistoryManorID;                 // 公会在活动期间内曾经占领过的领地
        public int nInspire;                            // 公会战鼓舞次数
		public int nWinProbability;                     // 公会战胜利抽奖几率
		public int nLoseProbability;                    // 公会战失败抽奖几率
		public int nStorageAddPost;                     // 公会战仓库放入物品
		public int nStorageDelPost;                     // 公会战仓库删除物品
        public uint nJoinLevel;                         // 公会入会等级限制
        public uint emblemLevel;                        // 公会徽记等级
        public uint dungeonType;                        // 公会副本难度 取值 1 2 3 。。。。
        public byte mergerRequestType;                  //公会兼并请求状态 UInt8 无0 发送1 接受2    
        public byte prosperity;                         // 公会繁荣状态 UInt8 无0 解散1 低2 中等3 高4
        public uint lastWeekFunValue;                   //上周增加的资金
        public uint thisWeekFunValue;                   ///这周增加的资金

		public GuildLeaderData leaderData = new GuildLeaderData();                          // 会长数据
        public List<GuildMemberData> arrMembers = new List<GuildMemberData>();              // 成员
        public List<GuildRequesterData> arrRequesters = new List<GuildRequesterData>();     // 申请者
        public GuildTableMember[] arrTableMembers = new GuildTableMember[7];                // 圆桌会议成员
        public Dictionary<GuildBuildingType, GuildBuildingData> dictBuildings = new Dictionary<GuildBuildingType, GuildBuildingData>();   // 公会建筑
    }

    public class GuildAttackCityData
    {
        public GuildTerritoryBaseInfo info = new GuildTerritoryBaseInfo();
        public UInt64 enrollGuildId;
        public string enrollGuildName;
        public byte enrollGuildLevel;
        public string enrollGuildleaderName;
        public UInt32 itemId;
        public UInt32 itemNum;

        public void ClearData()
        {
            info.terrId = 0;
            info.guildName = "";
            info.enrollSize = 0;
            enrollGuildId = 0;
            enrollGuildName = "";
            enrollGuildLevel = 0;
            enrollGuildleaderName = "";
            itemId = 0;
            itemNum = 0;
        }
    }

    public class GuildDungeonActivityChestItem
    {
        public ItemData itemData;
        public bool isHighValue;
    }
    class GuildDataManager : DataManager<GuildDataManager>
    {
        public class DungeonDamageRankInfo
        {
            public uint nRank;
            public string strPlayerName;
            public ulong nDamage;
            public ulong nPlayerID;
        }
        public class BossDungeonDamageInfo
        {
            public ulong nBossID;
            public ulong nDungeonID;
        }
        public class GuildDungeonClearGateInfo
        {
            public string guildName;
            public ulong spendTime;
        }
        public class MediumDungeonDamageInfo
        { 
            public ulong nDungeonID;
            public ulong nMaxHp;
            public ulong nOddHp;
            public ulong nVerifyBlood;
        }
        public class JuniorDungeonDamageInfo
        {
            public ulong nDungeonID;
            public ulong nKillCount;    
        }
        public class BuffAddUpInfo
        {
            public ulong nDungeonID;        
            public ulong nBuffID;
            public ulong nBuffLv;
        }
        public class GuildDungeonActivityData
        {
            public ulong nBossOddHp;
            public ulong nBossMaxHp;
            public ulong nVerifyBlood;
            public List<JuniorDungeonDamageInfo> juniorDungeonDamgeInfos = new List<JuniorDungeonDamageInfo>();
            public List<MediumDungeonDamageInfo> mediumDungeonDamgeInfos = new List<MediumDungeonDamageInfo>();
            public List<BossDungeonDamageInfo> bossDungeonDamageInfos = new List<BossDungeonDamageInfo>();
            public List<BuffAddUpInfo> buffAddUpInfos = new List<BuffAddUpInfo>();
            public List<GuildDungeonClearGateInfo> guildDungeonClearGateInfos = new List<GuildDungeonClearGateInfo>();
            public uint nActivityState;
            public uint nActivityStateEndTime;
            public bool bGetReward;
        } 
        public enum AuctionItemState
        {
            Prepare,        // 准备中
            InAuction,      // 竞拍中
            SoldOut,        // 已售出
            AbortiveAuction // 流拍
        }
        public class GuildAuctionItemData
        {
            public ulong guid;                          // 拍卖数据的唯一id
            public UIItemData itemData0 = new UIItemData();                // 道具0
            public UIItemData itemData1 = new UIItemData();                // 道具1
            public ulong statusEndStamp;                // 结束时间戳
            public AuctionItemState auctionItemState;   // 道具状态
            public ulong buyNowPrice;                   // 一口价
            public ulong curbiddingPrice;               // 当前竞拍价
            public ulong nextBiddingPrice;              // 下一次竞拍价
            public ulong bidRoleId;                     // 最新出价人
        }
        Dictionary<int, List<GuildSkillTable>> m_dictSkillTableData = new Dictionary<int, List<GuildSkillTable>>();
        const string m_strExchangeCoolTime = "guild_exchange_cold";

        GuildAttackCityData m_AttackCityData = new GuildAttackCityData();
        List<GuildBattleInspireInfo> m_GuildInspireList = new List<GuildBattleInspireInfo>();
        ItemData LotteryItem = null;

        List<FigureStatueInfo> TownStatueList = new List<FigureStatueInfo>();

        List<FigureStatueInfo> GuildGuardStatueList = new List<FigureStatueInfo>();
        GuildBuildingType m_eBuidingType = new GuildBuildingType();
        int UpBuildingNeedTicket = 0;
        private const string kKeyPrefix = "battle_buff_";
        List<DungeonDamageRankInfo> m_DungeonRankInfoList = new List<DungeonDamageRankInfo>();
        public List<DungeonDamageRankInfo> GetDungeonRankInfoList()
        {
            return m_DungeonRankInfoList;
        }
        public const int nGuildDungeonMapScenceID = 6031; // 公会地下城场景id
        public const int nGuildAreanScenceID = 6090; // 公会场景id
        const float fReqGuildActivityDataInterval = 5.0f;
        int iIntervalCallID = 0;

        Dictionary<int, GuildDungeonLvlTable> m_GuildDungeonID2LvTable = null; // 公会地下城id-->公会地下城等级表

        GuildDungeonActivityData m_GuildDungeonActivityData = new GuildDungeonActivityData();
        List<GuildAuctionItemData> guildAuctionItemDatasForGuildAuction = null;
        List<GuildAuctionItemData> guildAuctionItemDatasForWorldAuction = null;
        public bool IsGuildAuctionOpen // 公会拍卖是否开启了
        {
            get;
            private set;
        }
        public bool IsGuildWorldAuctionOpen // 公会世界拍卖是否开启了
        {
            get;
            private set;
        }
        public bool HaveNewWorldAuctonItem // 有新的世界拍卖品来了
        {
            get;
            set;
        }
        public bool HaveNewGuildAuctonItem // 有新的公会拍卖品来了
        {
            get;
            set;
        }
        public List<GuildAuctionItemData> GetGuildAuctionItemDatasForGuildAuction()
        {
            return guildAuctionItemDatasForGuildAuction;
        }
        public List<GuildAuctionItemData> GetGuildAuctionItemDatasForWorldAuction()
        {
            return guildAuctionItemDatasForWorldAuction;
        }
        string m_kSavePath = "GuildDungeonOpen.json";
        string jsonText = null;
        public GuildDungeonActivityData GetGuildDungeonActivityData()
        {
            return m_GuildDungeonActivityData;
        }
        DungeonDamageRankInfo m_MyDungeonDamageRankInfo = new DungeonDamageRankInfo();
        public DungeonDamageRankInfo GetMyDungeonDamageRankInfo()
        {
            return m_MyDungeonDamageRankInfo;
        }

        Dictionary<int, int> guildDungeonID2TeamDungeonID = null;

        /// <summary>
        ///  翻牌翻倍标志
        /// </summary>
        public byte chestDoubleFlag = 0;

        public GuildMyData myGuild { get; private set; }
        private Dictionary<GuildBuildingType, GuildBuildInfoTable> guildBuildType2Talbe = new Dictionary<GuildBuildingType, GuildBuildInfoTable>();

        /// <summary>
        /// 点劵捐献一次需要的数量
        /// </summary>
        public int donatePointCost { get; private set; }

        /// <summary>
        /// 金币捐献一次需要的数量
        /// </summary>
        public int donateGoldCost { get; private set; }

        /// <summary>
        /// 捐献一次点劵获得的贡献
        /// </summary>
        public int donatePointGet { get; private set; }

        /// <summary>
        /// 捐献一次金币获得的贡献
        /// </summary>
        public int donateGoldGet { get; private set; }

        /// <summary>
        /// 福利社兑换一次消耗的贡献
        /// </summary>
        public int exchangeCost { get; private set; }

        public bool canJoinAllGuild { get; private set; }

        // 最大可以设置的入会等级
        public static int iJoinGuildMaxLevel = 60;
        private int nEmblemSkillID = 0; // 公会徽记对应的技能id

         /// <summary>
         /// 可兼并公会的成员
         /// </summary>
         public List<GuildMemberData> CanMergerdGuildMembers= new List<GuildMemberData>();
      
        /// <summary>
        /// 自己的公会是否有被其他公会兼并的请求
        /// </summary>
        public bool IsHaveMergedRequest = false;

        /// <summary>
        /// 是否同意了其他公会的兼并
        /// </summary>
        public bool IsAgreeMergerRequest = false;
        /// <summary>
        /// 服务器的开服时间戳
        /// </summary>
        private ulong mServerStartGameTime = 0;

        /// <summary>
        /// 公会战次数不够的时候，是否跳过tip，直接购买
        /// </summary>
        public bool IsJumpTipWhenStartGuildBattle = false;

        /// <summary>
        /// 跨服公会战次数不够的时候，是否跳过tip，直接购买
        /// </summary>
        public bool IsJumpTipWhenStartGuildBattleCorssServer = false;
        public int GetEmblemSkillID()
        {
            return nEmblemSkillID;
        }
        public int GetEmblemLv()
        {
            GuildMyData guildMyData = GuildDataManager.GetInstance().myGuild;
            if (guildMyData == null)
            {
                return -1;
            }
            return (int)guildMyData.emblemLevel;
        }
        // 获取公户副本难度等级(每次公会副本开启之前进行设置)
        public int GetGuildDungeonDiffType()
        {
            GuildMyData guildMyData = GuildDataManager.GetInstance().myGuild;
            if (guildMyData == null)
            {
                return 0;
            }
            return (int)guildMyData.dungeonType;       
        }
        public string GetEmblemNamePath(int iEmblemLv)
        {
            GuildEmblemTable guildEmblemTable = TableManager.GetInstance().GetTableItem<GuildEmblemTable>(iEmblemLv);
            if(guildEmblemTable != null)
            {
                return guildEmblemTable.namePath;
            }
            return "";
        }
        public string GetEmblemIconPath(int iEmblemLv)
        {
            GuildEmblemTable guildEmblemTable = TableManager.GetInstance().GetTableItem<GuildEmblemTable>(iEmblemLv);
            if (guildEmblemTable != null)
            {
                return guildEmblemTable.iconPath;
            }
            return "";
        }
        int nMaxEmblemLv = 10;
        public int GetMaxEmblemLv()
        {
            return nMaxEmblemLv;
        }
        public int GetEmblemLvUpGuildLvLimit()
        {          
            var table = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_GUILD_EMBLEM_GUILD_LEVEL);
            if(table == null)
            {
                return 0;
            }
            return table.Value;
        }
        public int GetEmblemLvUpPlayerLvLimit()
        {            
            var table = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_GUILD_EMBLEM_ROLE_LEVEL);
            if (table == null)
            {
                return 0;
            }
            return table.Value;
        }
        public int GetEmblemLvUpHonourLvLimit()
        {        
            var table = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_GUILD_EMBLEM_HONOUR_LEVEL);
            if (table == null)
            {
                return 0;
            }
            return table.Value;
        }
        public void GetEmblemLvUpCost(int iEmblemLv, ref int[] itemIDs, ref int[] itemNums)
        {
            if (iEmblemLv <= 0 || iEmblemLv > GetMaxEmblemLv())
            {
                return;
            }
            GuildEmblemTable guildEmblemTable = TableManager.GetInstance().GetTableItem<GuildEmblemTable>(iEmblemLv);
            if (guildEmblemTable == null)
            {
                return;
            }
            itemIDs = new int[guildEmblemTable.costIdLength];
            itemNums = new int[guildEmblemTable.costNumLength];
            if (itemIDs == null || itemNums == null)
            {
                return;
            }
            for(int i = 0;i < itemIDs.Length;i++)
            {
                itemIDs[i] = guildEmblemTable.costId[i];
            }
            for(int i = 0;i < itemNums.Length;i++)
            {
                itemNums[i] = guildEmblemTable.costNum[i];
            }
            return;
        }
        public bool IsCostEnoughToLvUpEmblem(ref List<int> notEnoughItemIDs)
        {
            int[] materials = null;
            int[] material_counts = null;
            notEnoughItemIDs = new List<int>();
            if(notEnoughItemIDs == null)
            {
                return false;
            }
            int nEmblemLv = GuildDataManager.GetInstance().GetEmblemLv();
            GuildDataManager.GetInstance().GetEmblemLvUpCost(nEmblemLv + 1, ref materials, ref material_counts);
            if(materials == null || material_counts == null)
            {
                return false;
            }
            int iCount = Math.Min(materials.Length, material_counts.Length);
            for(int i = 0;i < iCount;i++)
            {
                if(ItemDataManager.GetInstance().GetOwnedItemCount(materials[i]) < material_counts[i])
                {
                    notEnoughItemIDs.Add(materials[i]);
                }
            }
            return notEnoughItemIDs.Count == 0;
        }
        public int GetEmblemNeedHonourLv(int nEmblemLv)
        {
            GuildEmblemTable emblemTable = TableManager.GetInstance().GetTableItem<GuildEmblemTable>(nEmblemLv);
            if(emblemTable == null)
            {
                return 0;
            }
            return emblemTable.needHonourLevel;
        }

        public int winPower { get; private set; }
        public int losePower { get; private set; }
        public GuildPost contributePower { get; private set; }
        public GuildPost clearPower { get; private set; }
        public delegate void OnGuildPowerChanged(PowerSettingType ePowerSettingType, int iPowerValue);
        public OnGuildPowerChanged onGuildPowerChanged;
        public void SendChangeGuildSettingPower(PowerSettingType ePowerSettingType, int iPowerValue)
        {
            if (ePowerSettingType > PowerSettingType.PST_INVALID && ePowerSettingType < PowerSettingType.PST_COUNT)
            {
                switch (ePowerSettingType)
                {
                    case PowerSettingType.PST_WIN_POWER:
                        {
                            WorldGuildStorageSettingReq kSend = new WorldGuildStorageSettingReq();
                            kSend.type = (byte)GuildStorageSetting.GSS_WIN_PROBABILITY;
                            kSend.value = (uint)iPowerValue;
                            //Logger.LogErrorFormat("[guild_power]setting win power {0} {1}", ePowerSettingType, iPowerValue);
                            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);
                        }
                        break;
                    case PowerSettingType.PST_LOSE_POWER:
                        {
                            WorldGuildStorageSettingReq kSend = new WorldGuildStorageSettingReq();
                            kSend.type = (byte)GuildStorageSetting.GSS_LOSE_PROBABILITY;
                            kSend.value = (uint)iPowerValue;
                            //Logger.LogErrorFormat("[guild_power]setting lose power {0} {1}", ePowerSettingType, iPowerValue);
                            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);
                        }
                        break;
                    case PowerSettingType.PST_CONTRIBUTE_POWER:
                        {
                            WorldGuildStorageSettingReq kSend = new WorldGuildStorageSettingReq();
                            kSend.type = (byte)GuildStorageSetting.GSS_STORAGE_ADD_POST;
                            kSend.value = (uint)iPowerValue;
                            //Logger.LogErrorFormat("[guild_power]setting storage_add power {0} {1}", ePowerSettingType, (EGuildDuty)iPowerValue);
                            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);
                        }
                        break;
                    case PowerSettingType.PST_DELETE_POWER:
                        {
                            WorldGuildStorageSettingReq kSend = new WorldGuildStorageSettingReq();
                            kSend.type = (byte)GuildStorageSetting.GSS_STORAGE_DEL_POST;
                            kSend.value = (uint)iPowerValue;
                            //Logger.LogErrorFormat("[guild_power]setting storage_clear power {0} {1}", ePowerSettingType, (EGuildDuty)iPowerValue);
                            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);
                        }
                        break;
                }
            }
            else
            {
                Logger.LogErrorFormat("setting ePowerSettingType = {0} ERROR!!!!!", ePowerSettingType);
            }
        }

        //[MessageHandle(WorldGuildStorageSettingRes.MsgID)]
        void _OnRecvWorldGuildStorageSettingRes(MsgDATA msgData)
        {
            WorldGuildStorageSettingRes msgRet = new WorldGuildStorageSettingRes();
            msgRet.decode(msgData.bytes);

            if(msgRet.result != 0)
            {
                SystemNotifyManager.SystemNotify((int)msgRet.result);
            }
        }

        void _OnStorageSettingSync(GuildStorageSetting eSetting,int iPowerValue)
        {
            PowerSettingType ePowerSettingType = PowerSettingType.PST_COUNT;
            switch (eSetting)
            {
                case GuildStorageSetting.GSS_WIN_PROBABILITY:
                    {
                        ePowerSettingType = PowerSettingType.PST_WIN_POWER;
                        winPower = iPowerValue;
                        Logger.LogProcessFormat("[guild_power] sync {0} {1}", ePowerSettingType, iPowerValue);
                    }
                    break;
                case GuildStorageSetting.GSS_LOSE_PROBABILITY:
                    {
                        ePowerSettingType = PowerSettingType.PST_LOSE_POWER;
                        losePower = iPowerValue;
                    }
                    break;
                case GuildStorageSetting.GSS_STORAGE_ADD_POST:
                    {
                        ePowerSettingType = PowerSettingType.PST_CONTRIBUTE_POWER;
                        contributePower = (GuildPost)iPowerValue;
                    }
                    break;
                case GuildStorageSetting.GSS_STORAGE_DEL_POST:
                    {
                        ePowerSettingType = PowerSettingType.PST_DELETE_POWER;
                        clearPower = (GuildPost)iPowerValue;
                    }
                    break;
            }

            if(ePowerSettingType != PowerSettingType.PST_COUNT)
            {
                if (null != onGuildPowerChanged)
                {
                    onGuildPowerChanged(ePowerSettingType, iPowerValue);
                }
            }
        }

        public bool queried
        {
            get; set;
        }
        List<ItemData> storageDatas = new List<ItemData>();
        public List<ItemData> storeDatas
        {
            get
            {
                return storageDatas;
            }
        }
        public int storeageCapacity
        {
            get;
            set;
        }

        public int translateWinPowerIndex(int iPowerValue)
        {
            int iIndex = -1;
            for(int i = 0; i < winPowerSetting.Count; ++i)
            {
                if(winPowerSetting[i] == iPowerValue)
                {
                    iIndex = i;
                    break;
                }
            }
            return iIndex;
        }

        public int translateLosePowerIndex(int iPowerValue)
        {
            int iIndex = -1;
            for (int i = 0; i < losePowerSetting.Count; ++i)
            {
                if (losePowerSetting[i] == iPowerValue)
                {
                    iIndex = i;
                    break;
                }
            }
            return iIndex;
        }

        public static List<int> winPowerSetting = new List<int> { 0, 30, 50, 80, 100 };
        public static List<int> losePowerSetting = new List<int> { 0, 30, 50, 80, 100 };
        public static int getWinPowerByIndex(int iIndex)
        {
            if(iIndex >= 0 && iIndex < winPowerSetting.Count)
            {
                return winPowerSetting[iIndex];
            }
            return winPowerSetting[0];
        }
        public static int getLosePowerByIndex(int iIndex)
        {
            if (iIndex >= 0 && iIndex < losePowerSetting.Count)
            {
                return losePowerSetting[iIndex];
            }
            return losePowerSetting[0];
        }

        //[MessageHandle(WorldGuildStorageItemSync.MsgID)]
        void _OnSorageItemUpdated(MsgDATA msgData)
        {
            WorldGuildStorageItemSync recv = new WorldGuildStorageItemSync();
            recv.decode(msgData.bytes);

            //Logger.LogErrorFormat("WorldGuildStorageItemSync");

            for(int i = 0; i < recv.records.Length; ++i)
            {
                AddRecord(recv.records[i]);
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnGuildSotrageOperationRecordsChanged);

            for(int i = 0; i < recv.items.Length; ++i)
            {
                var item = recv.items[i];

                var find = storageDatas.Find(x =>
                {
                    return x.GUID == item.uid;
                });

                if (null == find)
                {
                    if (item.num > 0)
                    {
                        var itemData = ItemDataManager.CreateItemDataFromTable((int)item.dataId);
                        if (null != itemData)
                        {
                            itemData.Count = item.num;
                            itemData.GUID = item.uid;
                            itemData.StrengthenLevel = item.str;
                            itemData.EquipType = (EEquipType)item.equipType;
                            storageDatas.Add(itemData);
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnGuildHouseItemAdd, itemData);
                        }
                    }
                }
                else
                {
                    if (item.num == 0)
                    {
                        storageDatas.Remove(find);
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnGuildHouseItemRemoved, find);
                    }
                    else
                    {
                        find.Count = item.num;
                        find.StrengthenLevel = item.str;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnGuildHouseItemUpdate, find);
                    }
                }
            }
        }

        public void SendStoreItems(GuildStorageItemInfo[] items)
        {
            //Logger.LogErrorFormat("send add item to store house!!!");
            if (null != items && items.Length > 0)
            {
                WorldGuildAddStorageReq msg = new WorldGuildAddStorageReq();
                msg.items = items;

                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);
            }
        }

        //[MessageHandle(WorldGuildAddStorageRes.MsgID)]
        void _OnRecvWorldGuildAddStorageRes(MsgDATA msgData)
        {
            WorldGuildAddStorageRes msgRet = new WorldGuildAddStorageRes();
            msgRet.decode(msgData.bytes);

            if (0 != msgRet.result)
            {
                SystemNotifyManager.SystemNotify((int)msgRet.result);
            }
            else
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_item_add_ok"));
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnGuildHouseItemStoreRet);
        }

        public void SendDeleteStorageItems(GuildStorageDelItemInfo[] items)
        {
            //Logger.LogErrorFormat("send del item from store house!!!");
            if(null != items && items.Length > 0)
            {
                WorldGuildDelStorageReq msg = new WorldGuildDelStorageReq();
                msg.items = items;

                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);
            }
        }

        //[MessageHandle(WorldGuildDelStorageRes.MsgID)]
        void _OnRecvWorldGuildDelStorageRes(MsgDATA msgData)
        {
            WorldGuildDelStorageRes msgRet = new WorldGuildDelStorageRes();
            msgRet.decode(msgData.bytes);

            if (0 != msgRet.result)
            {
                SystemNotifyManager.SystemNotify((int)msgRet.result);
            }
            else
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_item_clear_ok"));
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnGuildHouseItemDeleteRet);
        }

        List<object> records = new List<object>();
        public List<object> GuildStorageOperationRecords
        {
            get
            {
                return records;
            }

            private set
            {
                records = value;
            }
        }

        void _OnRecordsChanged()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnGuildSotrageOperationRecordsChanged);
        }

        public void AddRecord(GuildStorageOpRecord record)
        {
            string desc = string.Empty;
            switch ((GuildStorageOpType)record.opType)
            {
                case GuildStorageOpType.GSOT_BUYPUT:
                    {
                        desc = TR.Value("guild_op_desc_3");
                    }
                    break;
                case GuildStorageOpType.GSOT_PUT:
                    {
                        desc = TR.Value("guild_op_desc_2");
                    }
                    break;
                case GuildStorageOpType.GSOT_GET:
                    {
                        desc = TR.Value("guild_op_desc_1");
                    }
                    break;
            }
            string timeDesc = _GetTimeDesc(record);

            if(!string.IsNullOrEmpty(desc) && null != record.items)
            {
                for (int i = 0, iStart = 0; i < record.items.Length; i += 5)
                {
                    int iEnd = IntMath.Min(iStart + 5, record.items.Length);
                    string itemDescs = _GetItemDescs(record,iStart, iEnd);
                    iStart = iEnd;

                    if (!string.IsNullOrEmpty(desc) && !string.IsNullOrEmpty(itemDescs))
                    {
                        SotrageOperateRecordData data = new SotrageOperateRecordData();
                        data.record = record;
                        data.measured = false;
                        data.value = TR.Value("guild_op_desc", timeDesc, data.record.name, desc, itemDescs);
                        records.Add(data);
                    }
                }
            }
            else
            {
                Logger.LogErrorFormat("desc is empty!!!", desc);
            }
        }

        string _GetTimeDesc(GuildStorageOpRecord record)
        {
            string timeDesc = string.Empty;
            if(null != record)
            {
                return Utility.ToUtcTime2Local(record.time).ToString(TR.Value("guild_record_date_string"), Utility.cultureInfo);
            }
            return timeDesc;
        }

        string _GetItemDescs(GuildStorageOpRecord record,int iStart,int iEnd)
        {
            string desc = string.Empty;
            if(null != record)
            {
                for(int i = iStart; i < record.items.Length && i < iEnd; ++i)
                {
                    var itemInfo = record.items[i];
                    if(null != itemInfo && itemInfo.num > 0)
                    {
                        var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)itemInfo.dataId);
                        if(null == item)
                        {
                            continue;
                        }
                        var qualityInfo = ItemData.GetQualityInfo(item.Color,item.Color2 == 1);
                        if(null == qualityInfo)
                        {
                            continue;
                        }

                        string content = "{" + string.Format("I {0} {1} {2}",0, itemInfo.dataId, itemInfo.str) + "}";

                        if (string.IsNullOrEmpty(desc))
                        {
                            desc += string.Format("{0}*{1}", TR.Value("common_color_text", qualityInfo.ColStr, content), itemInfo.num);
                        }
                        else
                        {
                            desc += string.Format(",{0}*{1}", TR.Value("common_color_text", qualityInfo.ColStr, content), itemInfo.num);
                        }
                    }
                }
            }
            return desc;
        }

        public List<WorldGuildInviteNotify> GuildInviteList = new List<WorldGuildInviteNotify>();

        public Dictionary<int, List<GuildSkillTable>> dictSkillTableData { get { return m_dictSkillTableData; } }

        public sealed override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.GuildDataManager;
        }

        public sealed override void Initialize()
        {
            RegisterNetHandler();
            _RegisterUIEvent();
            _InitGuildSkillData();
            donatePointCost = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_GUILD_DONATE_POINT_COST).Value;
            donateGoldCost = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_GUILD_DONATE_GOLD_COST).Value;
            donatePointGet = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_GUILD_DONATE_POINT_CONTRI).Value;
            donateGoldGet = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_GUILD_DONATE_GOLD_CONTRI).Value;
            exchangeCost = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_GUILD_EXCHANGE_COST).Value;
            canJoinAllGuild = true;
            winPower = 0;
            losePower = 0;
            contributePower = GuildPost.GUILD_POST_ELDER;
            clearPower = GuildPost.GUILD_POST_ASSISTANT;
            storageDatas.Clear();
            storeageCapacity = 50;
            queried = false;
            records.Clear();
            IsShowFireworks = false;
            ActivityNotifyUIOpenCount = 0;

            m_GuildDungeonActivityData = new GuildDungeonActivityData();
            m_MyDungeonDamageRankInfo = new DungeonDamageRankInfo();
            m_DungeonRankInfoList = new List<DungeonDamageRankInfo>();

            guildDungeonID2TeamDungeonID = new Dictionary<int, int>();
            if(guildDungeonID2TeamDungeonID != null)
            {
                guildDungeonID2TeamDungeonID.Clear();
              
                Dictionary<int, object> dicts = TableManager.instance.GetTable<TeamDungeonTable>();
                if (dicts != null)
                {
                    var iter = dicts.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        TeamDungeonTable adt = iter.Current.Value as TeamDungeonTable;
                        if (adt == null)
                        {
                            continue;
                        }

                        if(adt.DungeonID > 0)
                        {                            
                            guildDungeonID2TeamDungeonID[adt.DungeonID] = adt.ID;                       
                        }                        
                    }
                }       
            }

            {
                m_GuildDungeonID2LvTable = new Dictionary<int, GuildDungeonLvlTable>();
                if(m_GuildDungeonID2LvTable != null)
                {
                    Dictionary<int, object> dicts = TableManager.instance.GetTable<GuildDungeonLvlTable>();
                    if (dicts != null)
                    {
                        var iter = dicts.GetEnumerator();
                        while (iter.MoveNext())
                        {
                            GuildDungeonLvlTable adt = iter.Current.Value as GuildDungeonLvlTable;
                            if (adt == null)
                            {
                                continue;
                            }

                            m_GuildDungeonID2LvTable.SafeAdd(adt.DungeonId, adt);
                        }
                    }
                }
            }

            PlayerBaseData.GetInstance().onLevelChanged -= OnLevelChanged;
            PlayerBaseData.GetInstance().onLevelChanged += OnLevelChanged;           
            PlayerBaseData.GetInstance().onMoneyChanged -= _OnMoneyChanged;
            PlayerBaseData.GetInstance().onMoneyChanged += _OnMoneyChanged;
            ItemDataManager.GetInstance().onAddNewItem -= _OnAddNewItem;
            ItemDataManager.GetInstance().onAddNewItem += _OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem -= OnUpdateItem;
            ItemDataManager.GetInstance().onUpdateItem += OnUpdateItem;
            guildAuctionItemDatasForGuildAuction = new List<GuildAuctionItemData>();
            guildAuctionItemDatasForWorldAuction = new List<GuildAuctionItemData>();
            IsGuildAuctionOpen = false;
            IsGuildWorldAuctionOpen = false;
            HaveNewWorldAuctonItem = false;
            HaveNewGuildAuctonItem = false;
            InvokeMethod.RemoveInvokeCall(OpenNotifyUI);
            nEmblemSkillID = 0;
            var systemValueTable = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_GUILD_EMBLEM_SKILL_PVE);
            if (systemValueTable != null)
            {
                nEmblemSkillID = systemValueTable.Value;
            }
            {
                nMaxEmblemLv = 0;
                Dictionary<int, object> dicts = TableManager.instance.GetTable<GuildEmblemTable>();
                if (dicts != null)
                {
                    var iter = dicts.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        GuildEmblemTable adt = iter.Current.Value as GuildEmblemTable;
                        if (adt == null)
                        {
                            continue;
                        }
                        if(adt.ID > nMaxEmblemLv)
                        {
                            nMaxEmblemLv = adt.ID;
                        }
                    }
                }
            }
            {
                guildBuildType2Talbe = new Dictionary<GuildBuildingType, GuildBuildInfoTable>();
                if(guildBuildType2Talbe != null)
                {
                    Dictionary<int, object> dicts = TableManager.instance.GetTable<GuildBuildInfoTable>();
                    if (dicts != null)
                    {
                        var iter = dicts.GetEnumerator();
                        while (iter.MoveNext())
                        {
                            GuildBuildInfoTable adt = iter.Current.Value as GuildBuildInfoTable;
                            if (adt == null)
                            {
                                continue;
                            }
                            guildBuildType2Talbe[(GuildBuildingType)adt.buildType] = adt;
                        }
                    }
                }
            }
            checkedLvUpBulilding = false;
            checkedLvUpEmblem = false;
            checkedSetBossDiff = false;
        }

        void _OnMoneyChanged(PlayerBaseData.MoneyBinderType eMoneyBinderType)
        {
            UpdateGuildEmblemRedPoint();
        }
        void _OnAddNewItem(List<Item> items)
        {
            UpdateGuildEmblemRedPoint();
        }
        void OnUpdateItem(List<Item> items)
        {
            UpdateGuildEmblemRedPoint();
        }
        public sealed override void Clear()
        {
            InvokeMethod.TaskManager.GetInstance().RmoveInvokeIntervalCall(iIntervalCallID);

            UnRegisterNetHandler();
            _UnregisterUIEvent();
            _ClearGuildSkillData();
            _ClearMyGuild();

            chestDoubleFlag = 0;
            donatePointCost = 0;
            donateGoldCost = 0;
            donatePointGet = 0;
            donateGoldGet = 0;
            exchangeCost = 0;
            canJoinAllGuild = true;
            winPower = 0;
            losePower = 0;
            contributePower = GuildPost.GUILD_POST_ELDER;
            clearPower = GuildPost.GUILD_POST_ASSISTANT;
            storageDatas.Clear();
            storeageCapacity = 50;
            queried = false;
            records.Clear();

            m_AttackCityData.ClearData();
            m_GuildInspireList.Clear();

            if(LotteryItem != null)
            {
                LotteryItem = null;
            }

            m_bHasLotteryed = true;

            if(TownStatueList != null)
            {
                TownStatueList.Clear();
            }

            if(GuildGuardStatueList != null)
            {
                GuildGuardStatueList.Clear();
            }
            if(m_DungeonRankInfoList != null)
            {
                m_DungeonRankInfoList.Clear();
            }
            UpBuildingNeedTicket = 0;
            guildBuildType2Talbe = null;

            checkedLvUpBulilding = false;
            checkedLvUpEmblem = false;
            checkedSetBossDiff = false;

            PlayerBaseData.GetInstance().GuildEmblemLv = 0;

            IsHaveMergedRequest = false;
            IsAgreeMergerRequest = false;
            CanMergerdGuildMembers.Clear();
            mServerStartGameTime = 0;
            IsJumpTipWhenStartGuildBattle = false;
            IsJumpTipWhenStartGuildBattleCorssServer = false;
        }
        public override void Update(float a_fTime)
        {
        }
        public override void OnApplicationStart()
        {
            FileArchiveAccessor.LoadFileInPersistentFileArchive(m_kSavePath, out jsonText);
            if (jsonText == null)
            {
                FileArchiveAccessor.SaveFileInPersistentFileArchive(m_kSavePath, "");
                jsonText = "";
                return;
            }
            return;
        }

        public bool CheckExchangeRedPoint()
        {
            uint nTimeCool = GuildDataManager.GetInstance().myGuild.nExchangeCoolTime;
            uint nCurrentTime = TimeManager.GetInstance().GetServerTime();

            int nMaxTime = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_GUILD_EXCHANGE_MAX_TIME).Value;
            int nUsedTime = CountDataManager.GetInstance().GetCount("guild_exchange");
            int nRemainTime = nMaxTime - nUsedTime;

            return nTimeCool <= nCurrentTime && nRemainTime > 0;
        }

        

        public void RequestGuildList(int a_nStartIndex, int a_nCount)
        {
            WorldGuildListReq msg = new WorldGuildListReq();
            msg.start = (ushort)a_nStartIndex;
            msg.num = (ushort)a_nCount;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<WorldGuildListRes>(msgRet =>
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildListUpdated, msgRet);
            });
        }

        public void CreateGuild(string a_strName, string a_strDeclaration)
        {
            if (HasSelfGuild())
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_already_have_guild"));
                return;
            }

            WorldGuildCreateReq msg = new WorldGuildCreateReq();
            msg.name = a_strName;
            msg.declaration = a_strDeclaration;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<WorldGuildCreateRes>(msgRet =>
            {
                if (msgRet == null)
                {
                    return;
                }

                if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.result);
                }
                else
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_creat_success"));
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildCreateSuccess);
                }
            });
        }

        public static void AcceptJoinGuild(string param)
        {
            if (string.IsNullOrEmpty(param))
            {
                return;
            }

            ulong guid = 0;
            if (!ulong.TryParse(param, out guid))
            {
                return;
            }

            GuildDataManager.GetInstance().RequestJoinGuild(guid);
        }

        public void RequestJoinGuild(ulong a_uGuildID)
        {
            if (HasSelfGuild())
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_already_have_guild"));
                return;
            }

            WorldGuildJoinReq msg = new WorldGuildJoinReq();
            msg.id = a_uGuildID;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<WorldJoinGuildRes>(msgRet =>
            {
                if (msgRet == null)
                {
                    return;
                }

                if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.result);
                }
                else
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_application"));
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildRequestJoinSuccess, a_uGuildID);
                }
            });
        }

        public void RequestJoinAllGuild()
        {
            if (HasSelfGuild())
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_already_have_guild"));
                return;
            }

            if (canJoinAllGuild == false)
            {
                return;
            }

            WorldGuildJoinReq msg = new WorldGuildJoinReq();
            msg.id = 0;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<WorldJoinGuildRes>(msgRet =>
            {
                if (msgRet == null)
                {
                    return;
                }

                if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.result);
                }
                else
                {
                    canJoinAllGuild = false;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildRequestJoinAllSuccess);
                }
            });
        }

        public void RequestGuildMembers()
        {
            if (_CheckSelfGuild())
            {
                WorldGuildMemberListReq msg = new WorldGuildMemberListReq();
                msg.guildID = 0;
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

                WaitNetMessageManager.GetInstance().Wait<WorldGuildMemberListRes>(msgRet =>
                {
                    myGuild.arrMembers.Clear();

                    for (int i = 0; i < msgRet.members.Length; ++i)
                    {
                        GuildMemberEntry source = msgRet.members[i];
                        GuildMemberData target = new GuildMemberData();
                        target.uGUID = source.id;
                        target.strName = source.name;
                        target.nLevel = source.level;
                        target.nJobID = source.occu;
                        target.vipLevel = source.vipLevel;
                        target.seasonLevel = source.seasonLevel;
                        target.eGuildDuty = GetClientDuty(source.post);
                        target.nContribution = (int)source.contribution;
                        target.uOffLineTime = source.logoutTime;
                        target.uActiveDegree = source.activeDegree;
                        target.playerLabelInfo = source.playerLabelInfo;
                        RelationData relationData = null;
                        RelationDataManager.GetInstance().FindPlayerIsRelation(source.id, ref relationData);
                        if (relationData != null)
                        {
                            if (relationData.remark != null && relationData.remark != "")
                            {
                                target.remark = relationData.remark;
                            }
                        }

                        myGuild.arrMembers.Add(target);
                    }

                    int iIndex = myGuild.arrMembers.FindIndex(value => { return value.uGUID == PlayerBaseData.GetInstance().RoleID; });
                    if (iIndex > 0 && iIndex < myGuild.arrMembers.Count)
                    {
                        GuildMemberData selfdata = myGuild.arrMembers[iIndex];

                        myGuild.arrMembers[iIndex] = myGuild.arrMembers[0];
                        myGuild.arrMembers[0] = selfdata;
                    }

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildMembersUpdated);
                });
            }
        }

        public void RequestGuildRequesters()
        {
            if (_CheckSelfGuild())
            {
                WorldGuildRequesterReq msg = new WorldGuildRequesterReq();
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

                WaitNetMessageManager.GetInstance().Wait<WorldGuildRequesterRes>(msgRet =>
                {
                    if (myGuild.arrRequesters == null)
                    {
                        myGuild.arrRequesters = new List<GuildRequesterData>();
                    }
                    else
                    {
                        myGuild.arrRequesters.Clear();
                    }

                    for (int i = 0; i < msgRet.requesters.Length; ++i)
                    {
                        GuildRequesterInfo source = msgRet.requesters[i];
                        GuildRequesterData target = new GuildRequesterData();
                        target.uGUID = source.id;
                        target.strName = source.name;
                        target.nLevel = source.level;
                        target.nJobID = source.occu;
                        RelationData relationData = null;
                        RelationDataManager.GetInstance().FindPlayerIsRelation(source.id, ref relationData);
                        if (relationData != null)
                        {
                            if (relationData.remark != null && relationData.remark != "")
                            {
                                target.remark = relationData.remark;
                            }
                        }
                        myGuild.arrRequesters.Add(target);
                    }

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildRequestersUpdated);
                });
            }
        }

        public void ProcessRequester(ulong a_uGUID, bool a_bAgree)
        {
            if (_CheckSelfGuild())
            {
                if (_CheckPermission(EGuildPermission.ProcessRequester))
                {
                    WorldGuildProcessRequester msg = new WorldGuildProcessRequester();
                    msg.id = a_uGUID;
                    msg.agree = a_bAgree ? (byte)1 : (byte)0;
                    NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

                    WaitNetMessageManager.GetInstance().Wait<WorldGuildProcessRequesterRes>(msgRet =>
                    {
                        if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                        {
                            SystemNotifyManager.SystemNotify((int)msgRet.result);
                        }
                        else
                        {
                            if (a_uGUID == 0)
                            {
                                _ClearRequester();
                            }
                            else
                            {
                                _RemoveRequester(a_uGUID);
                                if (a_bAgree)
                                {
                                    _AddMember(msgRet.entry);

                                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_agree_requester_success", msgRet.entry.name));
                                }
                            }
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildProcessRequesterSuccess, a_uGUID);
                        }
                    });
                }
            }
        }

        public void ChangeMemberDuty(ulong a_uMemberGUID, EGuildDuty a_eDuty, ulong a_uReplaceMemberGUID)
        {
            if (_CheckSelfGuild())
            {
                GuildMemberData member = _FindMember(a_uMemberGUID);
                if (member == null)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_can_not_find_member"));
                    return;
                }

                EGuildPermission ePermission = (EGuildPermission)(1 << (int)a_eDuty);
                if (_CheckPermission(ePermission, member.eGuildDuty))
                {
                    WorldGuildChangePostReq msg = new WorldGuildChangePostReq();
                    msg.id = a_uMemberGUID;
                    msg.post = GetServerDuty(a_eDuty);
                    msg.replacerId = a_uReplaceMemberGUID;
                    NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

                    WaitNetMessageManager.GetInstance().Wait<WorldGuildChangePostRes>(msgRet =>
                    {
                        if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                        {
                            SystemNotifyManager.SystemNotify((int)msgRet.result);
                        }
                        else
                        {
                            if (member != null)
                            {
                                member.eGuildDuty = a_eDuty;

                                if (a_uReplaceMemberGUID > 0)
                                {
                                    GuildMemberData replaceMember = _FindMember(a_uReplaceMemberGUID);
                                    if (replaceMember != null)
                                    {
                                        replaceMember.eGuildDuty = EGuildDuty.Normal;
                                    }
                                }

                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildChangeDutySuccess, a_uMemberGUID, a_uReplaceMemberGUID);
                            }
                        }
                    });
                }
            }
        }

        public void DismissGuild()
        {
            if (_CheckSelfGuild())
            {
                if (_CheckPermission(EGuildPermission.Dismiss))
                {
                    WorldGuildDismissReq msg = new WorldGuildDismissReq();
                    NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

                    WaitNetMessageManager.GetInstance().Wait<WorldGuildOperRes>(msgRet =>
                    {
                        if (msgRet.type == (byte)GuildOperation.DISMISS)
                        {
                            if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                            {
                                SystemNotifyManager.SystemNotify((int)msgRet.result);
                            }
                            else
                            {
                                myGuild.nDismissTime = msgRet.param;
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildRequestDismissSuccess);
                            }
                        }
                    });
                }
            }
        }

        public void CancelDismissGuild()
        {
            if (_CheckSelfGuild())
            {
                if (_CheckPermission(EGuildPermission.Dismiss))
                {
                    WorldGuildCancelDismissReq msg = new WorldGuildCancelDismissReq();
                    NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

                    WaitNetMessageManager.GetInstance().Wait<WorldGuildOperRes>(msgRet =>
                    {
                        if (msgRet.type == (byte)GuildOperation.CANCEL_DISMISS)
                        {
                            if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                            {
                                SystemNotifyManager.SystemNotify((int)msgRet.result);
                            }
                            else
                            {
                                myGuild.nDismissTime = 0;
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildRequestCancelDismissSuccess);
                            }
                        }
                    });
                }
            }
        }

        public void LeaveGuild()
        {
            if (_CheckSelfGuild())
            {
                WorldGuildLeaveReq msg = new WorldGuildLeaveReq();
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

                WaitNetMessageManager.GetInstance().Wait<WorldGuildLeaveRes>(msgRet =>
                {
                    if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                    {
                        SystemNotifyManager.SystemNotify((int)msgRet.result);
                    }
                    else
                    {
                        _ClearMyGuild();
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildLeaveGuildSuccess);
                    }
                });
            }
        }

        public void KickMember(ulong a_uMemberGUID)
        {
            if (_CheckSelfGuild())
            {
                GuildMemberData member = _FindMember(a_uMemberGUID);
                if (member == null)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_can_not_find_member"));
                    return;
                }

                if (_CheckPermission(EGuildPermission.KickMember, member.eGuildDuty))
                {
                    WorldGuildKick msg = new WorldGuildKick();
                    msg.id = a_uMemberGUID;
                    NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

                    WaitNetMessageManager.GetInstance().Wait<WorldGuildKickRes>(msgRet =>
                    {
                        if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                        {
                            SystemNotifyManager.SystemNotify((int)msgRet.result);
                        }
                        else
                        {
                            _RemoveMember(a_uMemberGUID);
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildKickMemberSuccess, a_uMemberGUID);
                        }
                    });
                }
            }
        }

        public void SendMail(string a_strContent)
        {
            if (_CheckSelfGuild())
            {
                if (_CheckPermission(EGuildPermission.SendMail))
                {
                    WorldGuildSendMail msg = new WorldGuildSendMail();
                    msg.content = a_strContent;
                    NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

                    WaitNetMessageManager.GetInstance().Wait<WorldGuildOperRes>(msgRet =>
                    {
                        if (msgRet == null)
                        {
                            return;
                        }

                        if (msgRet.type == (byte)GuildOperation.SEND_MAIL)
                        {
                            if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                            {
                                SystemNotifyManager.SystemNotify((int)msgRet.result);
                            }
                            else
                            {
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildSendMailSuccess);
                                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_sende-mail_success"));
                            }
                        }
                    });
                }
            }
        }

        public void ChangeDeclaration(string a_strContent)
        {
            if (_CheckSelfGuild())
            {
                if (_CheckPermission(EGuildPermission.ChangeDeclaration))
                {
                    WorldGuildModifyDeclaration msg = new WorldGuildModifyDeclaration();
                    msg.declaration = a_strContent;
                    NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

                    WaitNetMessageManager.GetInstance().Wait<WorldGuildOperRes>(msgRet =>
                    {
                        if (msgRet == null)
                        {
                            return;
                        }

                        if (msgRet.type == (byte)GuildOperation.MODIFY_DECLAR)
                        {
                            if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                            {
                                SystemNotifyManager.SystemNotify((int)msgRet.result);
                            }
                            else
                            {
                                myGuild.strDeclaration = a_strContent;
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildChangeDeclarationSuccess);
                            }
                        }
                    });
                }
            }
        }

        public void ChangeNotice(string a_strContent)
        {
            if (_CheckSelfGuild())
            {
                if (_CheckPermission(EGuildPermission.ChangeNotice))
                {
                    WorldGuildModifyAnnouncement msg = new WorldGuildModifyAnnouncement();
                    msg.content = a_strContent;
                    NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

                    WaitNetMessageManager.GetInstance().Wait<WorldGuildOperRes>(msgRet =>
                    {
                        if (msgRet == null)
                        {
                            return;
                        }

                        if (msgRet.type == (byte)GuildOperation.MODIFY_ANNOUNCE)
                        {
                            if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                            {
                                SystemNotifyManager.SystemNotify((int)msgRet.result);
                            }
                            else
                            {
                                myGuild.strNotice = a_strContent;
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildChangeNoticeSuccess);
                            }
                        }
                    });
                }
            }
        }

        public void ChangeName(uint a_ItemtableID, ulong a_itemGUID, string a_strContent)
        {
            if (_CheckSelfGuild())
            {
                if (_CheckPermission(EGuildPermission.ChangeName))
                {
                    WorldGuildModifyName msg = new WorldGuildModifyName();
                    msg.itemTableID = a_ItemtableID;
                    msg.itemGUID = a_itemGUID;
                    msg.name = a_strContent;
                    NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

                    WaitNetMessageManager.GetInstance().Wait<WorldGuildOperRes>(msgRet =>
                    {
                        if (msgRet == null)
                        {
                            return;
                        }

                        if (msgRet.type == (byte)GuildOperation.MODIFY_NAME)
                        {
                            if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                            {
                                SystemNotifyManager.SystemNotify((int)msgRet.result);
                            }
                            else
                            {
                                myGuild.strName = a_strContent;
                                ClientSystemManager.GetInstance().CloseFrame<GuildCommonModifyFrame>();
                                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_change_name_success"));
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildChangeNameSuccess);
                            }
                        }
                    });
                }
            }
        }

        public void UpgradeBuilding(GuildBuildingType a_eType, int costFund)
        {
            GuildBuildingData data = GetBuildingData(a_eType);
            if(data == null)
            {
                return;
            }

            if (!_CheckSelfGuild())
                    {
                return;                
            }

            if (!_CheckPermission(EGuildPermission.UpgradeBuilding))
                        {
                return;               
                    }

            m_eBuidingType = a_eType;         

                        SendUpBuildingReq();
        }

        void OnClickSendUpBuildingLv()
        {
            CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();

            costInfo.nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT);
            costInfo.nCount = UpBuildingNeedTicket;

            CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
            {
                SendUpBuildingReq();
            });        
        }
        void UpdateGuildBuildingRedPoint()
        {
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.GuildMain);
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.GuildBuilding);
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.GuildBuildingManager);
        }
        void UpdateGuildEmblemRedPoint()
        {
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.GuildMain);
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.GuildBuilding);
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.GuildEmblem);
        }
        void UpdateGuildSetBossDiffRedPoint()
        {
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.GuildMain);
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.GuildBuilding);
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.GuildSetBossDiff);
        }

        public void SendUpBuildingReq()
        {
            WorldGuildUpgradeBuilding msg = new WorldGuildUpgradeBuilding();
            msg.type = (byte)m_eBuidingType;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            int nLevel = 0;
            int nFund = 0;
            if (myGuild.dictBuildings.ContainsKey(m_eBuidingType))
            {
                nLevel = myGuild.dictBuildings[m_eBuidingType].nLevel + 1;
                nFund = myGuild.nFund - myGuild.dictBuildings[m_eBuidingType].nUpgradeCost;
            }

            WaitNetMessageManager.GetInstance().Wait<WorldGuildOperRes>(msgRet =>
            {
                if (msgRet.type == (byte)GuildOperation.UPGRADE_BUILDING)
                {
                    if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                    {
                        SystemNotifyManager.SystemNotify((int)msgRet.result);
                    }
                    else
                    {
                        if (myGuild.dictBuildings.ContainsKey(m_eBuidingType))
                        {
                            myGuild.dictBuildings[m_eBuidingType].nLevel = nLevel;
                            myGuild.nFund = nFund;
                            UpdateGuildBuildingRedPoint();
                            _UpdateBuildingData();
                            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("guild_build_lv_up_success", GetBuildingName(m_eBuidingType)));
                        }
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildUpgradeBuildingSuccess);
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUpdateGuildEmblemLvUpEntry);
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUpdateGuildEmblemLvUpRedPoint);                        
                    }
                }
            });
        }

        public void RequsetDonateLog()
        {
            if (_CheckSelfGuild())
            {
                WorldGuildDonateLogReq msg = new WorldGuildDonateLogReq();
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

                WaitNetMessageManager.GetInstance().Wait<WorldGuildDonateLogRes>(msgRet =>
                {
                    List<GuildDonateData> arrDonateData = new List<GuildDonateData>();
                    for (int i = 0; i < msgRet.logs.Length; ++i)
                    {
                        GuildDonateData data = new GuildDonateData();
                        GuildDonateLog source = msgRet.logs[i];
                        data.uGUID = source.id;
                        data.strName = source.name;
                        data.eType = (GuildDonateType)source.type;
                        data.nTimes = source.num;
                        data.nFund = (int)source.contri;
                        arrDonateData.Add(data);
                    }

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildRequestDonateLogSuccess, arrDonateData);
                });
            }
        }

        public void Donate(GuildDonateType a_eType, int a_nTimes)
        {
            if (_CheckSelfGuild())
            {
                WorldGuildDonateReq msg = new WorldGuildDonateReq();
                msg.type = (byte)a_eType;
                msg.num = (byte)a_nTimes;
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

                int nFund = myGuild.nFund;
                int nContribution = PlayerBaseData.GetInstance().guildContribution;
                ulong uGold = PlayerBaseData.GetInstance().Gold;
                ulong uBindGold = PlayerBaseData.GetInstance().BindGold;
                ulong uTicket = PlayerBaseData.GetInstance().Ticket;
                ulong uBindTicket = PlayerBaseData.GetInstance().BindTicket;
                uint nDonateGoldTimes = 0;
                uint nDonateTicketTimes = 0;

                if (a_eType == GuildDonateType.GOLD)
                {
                    int nGet = donateGoldGet * a_nTimes;
                    ulong uCost = (ulong)(donateGoldCost * a_nTimes);
                    nFund += nGet;
                    nContribution += nGet;
                    if (uBindGold >= uCost)
                    {
                        uBindGold -= uCost;
                    }
                    else
                    {
                        uGold -= (uCost - uBindGold);
                        uBindGold = 0;
                    }

                    nDonateGoldTimes = (uint)CountDataManager.GetInstance().GetCount("guild_donate_gold") + (uint)a_nTimes;
                }
                else if (a_eType == GuildDonateType.POINT)
                {
                    int nGet = donatePointGet * a_nTimes;
                    ulong uCost = (ulong)(donatePointCost * a_nTimes);
                    nFund += nGet;
                    nContribution += nGet;
                    if (uBindTicket >= uCost)
                    {
                        uBindTicket -= uCost;
                    }
                    else
                    {
                        uTicket -= (uCost - uBindTicket);
                        uBindTicket = 0;
                    }

                    nDonateTicketTimes = (uint)CountDataManager.GetInstance().GetCount("guild_donate_point") + (uint)a_nTimes;
                }

                WaitNetMessageManager.GetInstance().Wait<WorldGuildOperRes>(msgRet =>
                {
                    if (msgRet.type == (byte)GuildOperation.DONATE)
                    {
                        if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                        {
                            SystemNotifyManager.SystemNotify((int)msgRet.result);
                        }
                        else
                        {
                            GuildDonateData data = new GuildDonateData();
                            data.uGUID = PlayerBaseData.GetInstance().RoleID;
                            data.strName = PlayerBaseData.GetInstance().Name;
                            data.eType = a_eType;
                            data.nTimes = a_nTimes;

                            myGuild.nFund = nFund;
                            PlayerBaseData.GetInstance().guildContribution = nContribution;
                            PlayerBaseData.GetInstance().BindGold = uBindGold;
                            PlayerBaseData.GetInstance().Gold = uGold;
                            PlayerBaseData.GetInstance().Ticket = uTicket;
                            PlayerBaseData.GetInstance().BindTicket = uBindTicket;

                            if (a_eType == GuildDonateType.GOLD)
                            {
                                data.nFund = donateGoldGet * a_nTimes;
                                CountDataManager.GetInstance().SetCount("guild_donate_gold", nDonateGoldTimes);
                                Logger.LogProcessFormat("donate gold receive -> count:{0}", nDonateGoldTimes);
                            }
                            else if (a_eType == GuildDonateType.POINT)
                            {
                                data.nFund = donatePointGet * a_nTimes;
                                CountDataManager.GetInstance().SetCount("guild_donate_point", nDonateTicketTimes);
                                Logger.LogProcessFormat("donate point receive -> count:{0}", nDonateTicketTimes);
                            }

                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildDonateSuccess, data);
                        }
                    }
                });
            }
        }

        public void Exchange()
        {
            if (_CheckSelfGuild())
            {
                SceneGuildExchangeReq msg = new SceneGuildExchangeReq();
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

                uint nTimes = (uint)CountDataManager.GetInstance().GetCount("guild_exchange") + 1;

                WaitNetMessageManager.GetInstance().Wait<WorldGuildOperRes>(msgRet =>
                {
                    if (msgRet.type == (byte)GuildOperation.EXCHANGE)
                    {
                        if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                        {
                            SystemNotifyManager.SystemNotify((int)msgRet.result);
                        }
                        else
                        {
                            CountDataManager.GetInstance().SetCount("guild_exchange", nTimes);
                            myGuild.nExchangeCoolTime = msgRet.param;
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildExchangeSuccess);
                        }
                    }
                });
            }
        }

        public void Worship(GuildOrzType a_eType)
        {
            if (_CheckSelfGuild())
            {
                WorldGuildOrzReq msg = new WorldGuildOrzReq();
                msg.type = (byte)a_eType;
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

                uint nTimes = (uint)CountDataManager.GetInstance().GetCount("guild_orz") + 1;

                WaitNetMessageManager.GetInstance().Wait<WorldGuildOperRes>(msgRet =>
                {
                    if (msgRet.type == (byte)GuildOperation.ORZ)
                    {
                        if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                        {
                            SystemNotifyManager.SystemNotify((int)msgRet.result);
                        }
                        else
                        {
                            CountDataManager.GetInstance().SetCount("guild_orz", nTimes);
                            myGuild.leaderData.nPopularity++;
                            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.GuildStatue);
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildWorshipSuccess, a_eType);
                        }
                    }
                });
            }
        }

        public void RequestLeaderStatue()
        {
            if (_CheckSelfGuild())
            {
                WorldGuildLeaderInfoReq msg = new WorldGuildLeaderInfoReq();
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

                WaitNetMessageManager.GetInstance().Wait<WorldGuildLeaderInfoRes>(msgRet =>
                {
                    myGuild.leaderData.strName = msgRet.info.name;
                    myGuild.leaderData.nPopularity = msgRet.info.popularoty;
                    myGuild.leaderData.uGUID = msgRet.info.id;
                    myGuild.leaderData.nJobID = msgRet.info.occu;
                    myGuild.leaderData.avatorInfo = msgRet.info.avatar;

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildLeaderUpdated);
                });
            }
        }

        public void JoinTable(int a_nPos)
        {
            if (_CheckSelfGuild())
            {
                if (HasJoinedTable())
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_table_already_joined"));
                    return;
                }

                GuildRoundtableTable.eType eType = GetTableType();

                WorldGuildTableJoinReq msg = new WorldGuildTableJoinReq();
                msg.seat = (byte)a_nPos;
                msg.type = (byte)(eType);
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

                uint nTimes = 0;
                if (eType == GuildRoundtableTable.eType.First)
                {
                    nTimes = (uint)CountDataManager.GetInstance().GetCount("guild_table") + 1;
                }
                else if (eType == GuildRoundtableTable.eType.Help)
                {
                    nTimes = (uint)CountDataManager.GetInstance().GetCount("guild_table_help") + 1;
                }

                WaitNetMessageManager.GetInstance().Wait<WorldGuildOperRes>(msgRet =>
                {
                    if (msgRet.type == (byte)GuildOperation.TABLE)
                    {
                        if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                        {
                            SystemNotifyManager.SystemNotify((int)msgRet.result);
                        }
                        else
                        {
                            if (eType == GuildRoundtableTable.eType.First)
                            {
                                CountDataManager.GetInstance().SetCount("guild_table", nTimes);
                            }
                            else if (eType == GuildRoundtableTable.eType.Help)
                            {
                                CountDataManager.GetInstance().SetCount("guild_table_help", nTimes);
                            }

                            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_table_join_success"));

                            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.GuildTable);
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildJoinTableSuccess, a_nPos);
                        }
                    }
                });
            }
        }

        public void BuyAndSendRedPacket(int a_nID, int a_nNum, string a_strName)
        {
            if (_CheckSelfGuild())
            {
                WorldGuildPayRedPacketReq msg = new WorldGuildPayRedPacketReq();
                msg.reason = (ushort)a_nID;
                msg.num = (byte)a_nNum;
                msg.name = a_strName;
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

                uint nTimes = (uint)CountDataManager.GetInstance().GetCount("guild_pay_rp") + 1;

                WaitNetMessageManager.GetInstance().Wait<WorldGuildOperRes>(msgRet =>
                {
                    if (msgRet.type == (byte)GuildOperation.PAY_REDPACKET)
                    {
                        if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                        {
                            SystemNotifyManager.SystemNotify((int)msgRet.result);
                        }
                        else
                        {
                            CountDataManager.GetInstance().SetCount("guild_pay_rp", nTimes);
                            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPacketSendSuccess, msgRet.param2);
                        }
                    }
                });
            }
        }

        public GuildRoundtableTable.eType GetTableType()
        {
            GuildRoundtableTable.eType eType;
            if (_GetFirstRemainTimes() > 0)
            {
                eType = GuildRoundtableTable.eType.First;
            }
            else if (_GetHelpRemainTimes() > 0)
            {
                eType = GuildRoundtableTable.eType.Help;
            }
            else
            {
                eType = GuildRoundtableTable.eType.FreeHelp;
            }

            return eType;
        }

        public int GetRemainWorshipTimes()
        {
            int nUsed = CountDataManager.GetInstance().GetCount("guild_orz");
            int nMax = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_GUILD_WORSHIP_TIME).Value;
            int nRemain = nMax - nUsed;
            if (nRemain < 0)
            {
                nRemain = 0;
            }
            return nRemain;
        }

        public bool HasPermission(EGuildPermission a_ePermission, EGuildDuty a_eSourceDuty = EGuildDuty.Invalid)
        {
            if (myGuild == null)
            {
                return false;
            }
            else
            {
                EGuildPermission ePermission = a_ePermission;
                if (
                    ePermission == EGuildPermission.SetDutyNormal ||
                    ePermission == EGuildPermission.SetDutyElder ||
                    ePermission == EGuildPermission.SetDutyAssistant ||
                    ePermission == EGuildPermission.SetDutyLeader
                    )
                {
                    if (PlayerBaseData.GetInstance().eGuildDuty < a_eSourceDuty)
                    {
                        ePermission = EGuildPermission.Invalid;
                    }
                }
                else if (ePermission == EGuildPermission.KickMember)
                {
                    if (PlayerBaseData.GetInstance().eGuildDuty <= a_eSourceDuty)
                    {
                        ePermission = EGuildPermission.Invalid;
                    }
                }

                EGuildPermission eMask = EGuildPermission.Invalid;
                switch (PlayerBaseData.GetInstance().eGuildDuty)
                {
                    case EGuildDuty.Invalid:
                        {
                            eMask = EGuildPermission.Invalid;
                            break;
                        }
                    case EGuildDuty.Normal:
                        {
                            eMask = EGuildPermission.NormalMask;
                            break;
                        }
                    case EGuildDuty.Elite:
                        {
                            eMask = EGuildPermission.EliteMask;
                            break;
                        }
                    case EGuildDuty.Elder:
                        {
                            eMask = EGuildPermission.ElderMask;
                            break;
                        }
                    case EGuildDuty.Assistant:
                        {
                            eMask = EGuildPermission.AssistantMask;
                            break;
                        }
                    case EGuildDuty.Leader:
                        {
                            eMask = EGuildPermission.LeaderMask;
                            break;
                        }
                }

                return (eMask & ePermission) != 0;
            }
        }

        public bool HasSelfGuild()
        {
            return myGuild != null;
        }

        public bool HasJoinedTable()
        {
            if (HasSelfGuild())
            {
                for (int i = 0; i < myGuild.arrTableMembers.Length; ++i)
                {
                    if (myGuild.arrTableMembers[i] != null)
                    {
                        if (myGuild.arrTableMembers[i].id == PlayerBaseData.GetInstance().RoleID)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 职位是否满了
        /// </summary>
        /// <param name="a_eDuty"></param>
        /// <returns></returns>
        public bool IsDutyFull(EGuildDuty a_eDuty)
        {
            if (HasSelfGuild() == false)
            {
                return false;
            }

            if (a_eDuty == EGuildDuty.Leader)
            {
                return true;
            }
            else if (a_eDuty == EGuildDuty.Assistant)
            {
                int assistantNum = 0;
                for (int i = 0; i < myGuild.arrMembers.Count; ++i)
                {
                    if (myGuild.arrMembers[i].eGuildDuty == EGuildDuty.Assistant)
                    {
                        assistantNum++;
                    }
                }
                if (assistantNum == 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (a_eDuty == EGuildDuty.Elder)
            {
                int nCount = 0;
                for (int i = 0; i < myGuild.arrMembers.Count; ++i)
                {
                    if (myGuild.arrMembers[i].eGuildDuty == EGuildDuty.Elder)
                    {
                        nCount++;
                    }
                }
                return nCount >= 4;
            }
            else
            {
                return false;
            }
        }

        public List<GuildMemberData> GetMembers()
        {
            return myGuild.arrMembers;
        }

        public List<GuildMemberData> GetMembersByDuty(EGuildDuty a_eDuty)
        {
            List<GuildMemberData> arrMembers = new List<GuildMemberData>();
            if (HasSelfGuild())
            {
                for (int i = 0; i < myGuild.arrMembers.Count; ++i)
                {
                    if (myGuild.arrMembers[i].eGuildDuty == a_eDuty)
                    {
                        arrMembers.Add(myGuild.arrMembers[i]);
                    }
                }
            }
            return arrMembers;
        }

        public byte GetServerDuty(EGuildDuty a_eDuty)
        {
            GuildPost ePost = GuildPost.GUILD_INVALID;
            switch (a_eDuty)
            {
                case EGuildDuty.Invalid:
                    ePost = GuildPost.GUILD_INVALID;
                    break;
                case EGuildDuty.Normal:
                    ePost = GuildPost.GUILD_POST_NORMAL;
                    break;
                case EGuildDuty.Elite:
                    ePost = GuildPost.GUILD_POST_ELITE;
                    break;
                case EGuildDuty.Elder:
                    ePost = GuildPost.GUILD_POST_ELDER;
                    break;
                case EGuildDuty.Assistant:
                    ePost = GuildPost.GUILD_POST_ASSISTANT;
                    break;
                case EGuildDuty.Leader:
                    ePost = GuildPost.GUILD_POST_LEADER;
                    break;
            }
            return (byte)ePost;
        }

        public EGuildDuty GetClientDuty(byte a_nDuty)
        {
            GuildPost ePost = (GuildPost)a_nDuty;
            switch (ePost)
            {
                case GuildPost.GUILD_INVALID:
                    return EGuildDuty.Invalid;
                case GuildPost.GUILD_POST_NORMAL:
                    return EGuildDuty.Normal;
                case GuildPost.GUILD_POST_ELITE:
                    return EGuildDuty.Elite;
                case GuildPost.GUILD_POST_ELDER:
                    return EGuildDuty.Elder;
                case GuildPost.GUILD_POST_ASSISTANT:
                    return EGuildDuty.Assistant;
                case GuildPost.GUILD_POST_LEADER:
                    return EGuildDuty.Leader;
                default: return EGuildDuty.Invalid;
            }
        }

        public int GetBuildingLevel(GuildBuildingType a_eType)
        {
            if (HasSelfGuild())
            {
                GuildBuildingData data = null;
                myGuild.dictBuildings.TryGetValue(a_eType, out data);
                if (data != null)
                {
                    return data.nLevel;
                }
            }
            return 0;
        }

        public string GetBuildingName(GuildBuildingType guildBuildingType)
        {
            GuildBuildInfoTable guildBuildInfoTable = GetGuildBuildInfoTable(guildBuildingType);
            if(guildBuildInfoTable != null)
            {
                return guildBuildInfoTable.buildName;
            }
            return "";
        }
        public GuildBuildingData GetBuildingData(GuildBuildingType a_eType)
        {
            if (HasSelfGuild())
            {
                GuildBuildingData data = null;
                myGuild.dictBuildings.TryGetValue(a_eType, out data);
                return data;
            }
            return null;
        }

        // 获取建筑解锁等级
        // 目前只有荣耀殿堂和征战祭祀有解锁等级
        // add by qxy 2019-04-08
        public int GetUnLockBuildingNeedMainCityLv(GuildBuildingType guildBuildingType)
        {
            if(guildBuildingType == GuildBuildingType.HONOUR)
            {
                return GetEmblemLvUpGuildLvLimit();
            }
            else if(guildBuildingType == GuildBuildingType.FETE)
            {
                return GetGuildDungeonActivityGuildLvLimit();
            }
            return 0;
        }
        public GuildBuildInfoTable GetGuildBuildInfoTable(GuildBuildingType guildBuildingType)
        {
            if(guildBuildType2Talbe == null)
            {
                return null;
            }
            GuildBuildInfoTable table = null;
            guildBuildType2Talbe.TryGetValue(guildBuildingType, out table);        
            return table;          
        }
        public string GetMyGuildName()
        {
            if (HasSelfGuild())
            {
                return myGuild.strName;
            }
            return string.Empty;
        }
        public int GetMyGuildFund()
        {
            if (HasSelfGuild())
            {
                return myGuild.nFund;
            }
            return 0;
        }
        public int GetGuildLv()
        {
            if(myGuild != null)
            {
                return myGuild.nLevel;
            }
            return 0;
        }

		public void InviteJoinGuild(ulong roleID)
		{
			SceneRequest req = new SceneRequest();

			req.type = (byte)RequestType.InviteJoinGuild;
			req.target = roleID;
			req.targetName = "";
			req.param = 0;

			NetManager netMgr = NetManager.Instance();
			netMgr.SendCommand(ServerType.GATE_SERVER, req);
		}


        GuildMemberData _FindMember(ulong a_uMemberGUID)
        {
            if (myGuild != null && myGuild.arrMembers != null)
            {
                return myGuild.arrMembers.Find(value => { return value.uGUID == a_uMemberGUID; });
            }
            return null;
        }

        void _AddMember(GuildMemberEntry a_sourceData)
        {
            if (myGuild != null && a_sourceData != null)
            {
                if (myGuild.arrMembers.Find(value => { return value.uGUID == a_sourceData.id; }) == null)
                {
                    GuildMemberData target = new GuildMemberData();
                    target.uGUID = a_sourceData.id;
                    target.strName = a_sourceData.name;
                    target.nLevel = a_sourceData.level;
                    target.nJobID = a_sourceData.occu;
                    target.vipLevel = a_sourceData.vipLevel;
                    target.seasonLevel = a_sourceData.seasonLevel;
                    target.eGuildDuty = GetClientDuty(a_sourceData.post);
                    target.nContribution = (int)a_sourceData.contribution;
                    target.uOffLineTime = a_sourceData.logoutTime;
                    target.uActiveDegree = a_sourceData.activeDegree;

                    myGuild.arrMembers.Add(target);
                }
            }
        }

        void _RemoveMember(ulong a_uMemberGUID)
        {
            if (myGuild != null && myGuild.arrMembers != null)
            {
                int nIndex = myGuild.arrMembers.FindIndex((member) => { return member.uGUID == a_uMemberGUID; });
                if (nIndex >= 0 && nIndex < myGuild.arrMembers.Count)
                {
                    myGuild.arrMembers.RemoveAt(nIndex);
                }
            }
        }

        void _RemoveRequester(ulong a_uRequesterGUID)
        {
            if (myGuild != null && myGuild.arrRequesters != null)
            {
                int nIndex = myGuild.arrRequesters.FindIndex(value => { return value.uGUID == a_uRequesterGUID; });
                if (nIndex >= 0 && nIndex < myGuild.arrRequesters.Count)
                {
                    myGuild.arrRequesters.RemoveAt(nIndex);
                }
            }
        }

        void _ClearRequester()
        {
            if (myGuild != null && myGuild.arrRequesters != null)
            {
                myGuild.arrRequesters.Clear();
            }
        }

        bool _CheckSelfGuild(bool a_bHas = true)
        {
            if (a_bHas)
            {
                if (HasSelfGuild() == false)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_have_no_guild"));
                    return false;
                }
            }
            else
            {
                if (HasSelfGuild())
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_already_have_guild"));
                    return false;
                }
            }
            return true;
        }

        bool _CheckPermission(EGuildPermission a_ePermission, EGuildDuty a_eSourceDuty = EGuildDuty.Invalid)
        {
            if (HasPermission(a_ePermission, a_eSourceDuty) == false)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_no_permission_to_operate"));
                return false;
            }
            return true;
        }

        void _UpdateBuildingData()
        {
            {
                GuildBuildingData data = myGuild.dictBuildings[GuildBuildingType.MAIN];
                data.nMaxLevel = 10;
                data.nUnlockMaincityLevel = -1;
                GuildBuildingTable table = TableManager.GetInstance().GetTableItem<GuildBuildingTable>(data.nLevel + 1);
                if (table != null)
                {
                    data.nUpgradeCost = table.MainCost;
                }
            }

            int nGuildLevel = myGuild.dictBuildings[GuildBuildingType.MAIN].nLevel;
            GuildTable tableGuild = TableManager.GetInstance().GetTableItem<GuildTable>(nGuildLevel);
            if (tableGuild != null)
            {
                {
                    GuildBuildingData data = myGuild.dictBuildings[GuildBuildingType.SHOP];
                    data.nMaxLevel = tableGuild.ShopLevel;
                    
                    int ntempGuildLevel = nGuildLevel;
                    GuildTable tempGuildTable = null;
                    while (true)
                    {
                        ntempGuildLevel++;
                        tempGuildTable = TableManager.GetInstance().GetTableItem<GuildTable>(ntempGuildLevel);

                        if (tempGuildTable != null)
                        {
                            if (tempGuildTable.ShopLevel > data.nMaxLevel)
                            {
                                data.nUnlockMaincityLevel = ntempGuildLevel;
                                break;
                            }
                        }
                        else
                        {
                            data.nUnlockMaincityLevel = -1;
                            break;
                        }
                    }

                    GuildBuildingTable table = TableManager.GetInstance().GetTableItem<GuildBuildingTable>(data.nLevel + 1);
                    if (table != null)
                    {
                        data.nUpgradeCost = table.ShopCost;
                    }
                }
                {
                    GuildBuildingData data = myGuild.dictBuildings[GuildBuildingType.HONOUR];
                    data.nMaxLevel = tableGuild.HonourLevel;
                    int ntempGuildLevel = nGuildLevel;
                    GuildTable tempGuildTable = null;
                    while (true)
                    {
                        ntempGuildLevel++;
                        tempGuildTable = TableManager.GetInstance().GetTableItem<GuildTable>(ntempGuildLevel);
                        if (tempGuildTable != null)
                        {
                            if (tempGuildTable.HonourLevel > data.nMaxLevel)
                            {
                                data.nUnlockMaincityLevel = ntempGuildLevel;
                                break;
                            }
                        }
                        else
                        {
                            data.nUnlockMaincityLevel = -1;
                            break;
                        }
                    }
                    GuildBuildingTable table = TableManager.GetInstance().GetTableItem<GuildBuildingTable>(data.nLevel + 1);
                    if (table != null)
                    {
                        data.nUpgradeCost = table.HonourCost;
                    }
                }
                {
                    GuildBuildingData data = myGuild.dictBuildings[GuildBuildingType.FETE];
                    data.nMaxLevel = tableGuild.FeteLevel;
                    int ntempGuildLevel = nGuildLevel;
                    GuildTable tempGuildTable = null;
                    while (true)
                    {
                        ntempGuildLevel++;
                        tempGuildTable = TableManager.GetInstance().GetTableItem<GuildTable>(ntempGuildLevel);
                        if (tempGuildTable != null)
                        {
                            if (tempGuildTable.FeteLevel > data.nMaxLevel)
                            {
                                data.nUnlockMaincityLevel = ntempGuildLevel;
                                break;
                            }
                        }
                        else
                        {
                            data.nUnlockMaincityLevel = -1;
                            break;
                        }
                    }
                    GuildBuildingTable table = TableManager.GetInstance().GetTableItem<GuildBuildingTable>(data.nLevel + 1);
                    if (table != null)
                    {
                        data.nUpgradeCost = table.FeteCost;
                    }
                }

                {
                    GuildBuildingData data = myGuild.dictBuildings[GuildBuildingType.TABLE];
                    data.nMaxLevel = tableGuild.TableLevel;

                    int ntempGuildLevel = nGuildLevel;
                    GuildTable tempGuildTable = null;
                    while (true)
                    {
                        ntempGuildLevel++;
                        tempGuildTable = TableManager.GetInstance().GetTableItem<GuildTable>(ntempGuildLevel);

                        if (tempGuildTable != null)
                        {
                            if (tempGuildTable.TableLevel > data.nMaxLevel)
                            {
                                data.nUnlockMaincityLevel = ntempGuildLevel;
                                break;
                            }
                        }
                        else
                        {
                            data.nUnlockMaincityLevel = -1;
                            break;
                        }
                    }

                    GuildBuildingTable table = TableManager.GetInstance().GetTableItem<GuildBuildingTable>(data.nLevel + 1);
                    if (table != null)
                    {
                        data.nUpgradeCost = table.TableCost;
                    }
                }

                {
                    GuildBuildingData data = myGuild.dictBuildings[GuildBuildingType.DUNGEON];
                    data.nMaxLevel = tableGuild.DungeonLevel;

                    int ntempGuildLevel = nGuildLevel;
                    GuildTable tempGuildTable = null;
                    while (true)
                    {
                        ntempGuildLevel++;
                        tempGuildTable = TableManager.GetInstance().GetTableItem<GuildTable>(ntempGuildLevel);

                        if (tempGuildTable != null)
                        {
                            if (tempGuildTable.DungeonLevel > data.nMaxLevel)
                            {
                                data.nUnlockMaincityLevel = ntempGuildLevel;
                                break;
                            }
                        }
                        else
                        {
                            data.nUnlockMaincityLevel = -1;
                            break;
                        }
                    }

                    GuildBuildingTable table = TableManager.GetInstance().GetTableItem<GuildBuildingTable>(data.nLevel + 1);
                    if (table != null)
                    {
                        data.nUpgradeCost = table.DungeonCost;
                    }
                }

                {
                    GuildBuildingData data = myGuild.dictBuildings[GuildBuildingType.STATUE];
                    data.nMaxLevel = tableGuild.StatueLevel;

                    int ntempGuildLevel = nGuildLevel;
                    GuildTable tempGuildTable = null;
                    while (true)
                    {
                        ntempGuildLevel++;
                        tempGuildTable = TableManager.GetInstance().GetTableItem<GuildTable>(ntempGuildLevel);

                        if (tempGuildTable != null)
                        {
                            if (tempGuildTable.StatueLevel > data.nMaxLevel)
                            {
                                data.nUnlockMaincityLevel = ntempGuildLevel;
                                break;
                            }
                        }
                        else
                        {
                            data.nUnlockMaincityLevel = -1;
                            break;
                        }
                    }

                    GuildBuildingTable table = TableManager.GetInstance().GetTableItem<GuildBuildingTable>(data.nLevel + 1);
                    if (table != null)
                    {
                        data.nUpgradeCost = table.StatueCost;
                    }
                }

                {
                    GuildBuildingData data = myGuild.dictBuildings[GuildBuildingType.BATTLE];
                    data.nMaxLevel = tableGuild.BattleLevel;

                    int ntempGuildLevel = nGuildLevel;
                    GuildTable tempGuildTable = null;
                    while (true)
                    {
                        ntempGuildLevel++;
                        tempGuildTable = TableManager.GetInstance().GetTableItem<GuildTable>(ntempGuildLevel);

                        if (tempGuildTable != null)
                        {
                            if (tempGuildTable.BattleLevel > data.nMaxLevel)
                            {
                                data.nUnlockMaincityLevel = ntempGuildLevel;
                                break;
                            }
                        }
                        else
                        {
                            data.nUnlockMaincityLevel = -1;
                            break;
                        }
                    }

                    GuildBuildingTable table = TableManager.GetInstance().GetTableItem<GuildBuildingTable>(data.nLevel + 1);
                    if (table != null)
                    {
                        data.nUpgradeCost = table.BattleCost;
                    }
                }

                {
                    GuildBuildingData data = myGuild.dictBuildings[GuildBuildingType.WELFARE];
                    data.nMaxLevel = tableGuild.WelfareLevel;

                    int ntempGuildLevel = nGuildLevel;
                    GuildTable tempGuildTable = null;
                    while (true)
                    {
                        ntempGuildLevel++;
                        tempGuildTable = TableManager.GetInstance().GetTableItem<GuildTable>(ntempGuildLevel);

                        if (tempGuildTable != null)
                        {
                            if (tempGuildTable.WelfareLevel > data.nMaxLevel)
                            {
                                data.nUnlockMaincityLevel = ntempGuildLevel;
                                break;
                            }
                        }
                        else
                        {
                            data.nUnlockMaincityLevel = -1;
                            break;
                        }
                    }

                    GuildBuildingTable table = TableManager.GetInstance().GetTableItem<GuildBuildingTable>(data.nLevel + 1);
                    if (table != null)
                    {
                        data.nUpgradeCost = table.WelfareCost;
                    }
                }
            }
            else
            {
                Logger.LogErrorFormat(string.Format("公会表找不到ID:{0}的条目", nGuildLevel));
            }
            UpdateGuildBuildingRedPoint();
            UpdateGuildEmblemRedPoint();
            UpdateGuildSetBossDiffRedPoint();
        }

        void _InitGuildSkillData()
        {
            _ClearGuildSkillData();

            var iter = TableManager.GetInstance().GetTable<GuildSkillTable>().GetEnumerator();
            while (iter.MoveNext())
            {
                GuildSkillTable table = iter.Current.Value as GuildSkillTable;
                if (m_dictSkillTableData.ContainsKey(table.SkillID) == false)
                {
                    m_dictSkillTableData.Add(table.SkillID, new List<GuildSkillTable>());
                }
                m_dictSkillTableData[table.SkillID].Add(table);
            }
        }

        void _ClearGuildSkillData()
        {
            m_dictSkillTableData.Clear();
        }

        int _GetFirstRemainTimes()
        {
            int nMax = TableManager.GetInstance().GetTableItem<ProtoTable.GuildRoundtableTable>((int)ProtoTable.GuildRoundtableTable.eType.First).TimesLimit;
            int nTimes = nMax - CountDataManager.GetInstance().GetCount("guild_table");
            if (nTimes < 0)
            {
                nTimes = 0;
            }
            return nTimes;
        }

        int _GetHelpRemainTimes()
        {
            int nMax = TableManager.GetInstance().GetTableItem<ProtoTable.GuildRoundtableTable>((int)ProtoTable.GuildRoundtableTable.eType.Help).TimesLimit;
            int nTimes = nMax - CountDataManager.GetInstance().GetCount("guild_table_help");
            if (nTimes < 0)
            {
                nTimes = 0;
            }
            return nTimes;
        }

        void RegisterNetHandler()
        {
            NetProcess.AddMsgHandler(WorldGuildSyncInfo.MsgID, _OnInitMyGuild);
            NetProcess.AddMsgHandler(WorldGuildInviteNotify.MsgID, _OnInviteJoinGuild);
            NetProcess.AddMsgHandler(WorldFigureStatueSync.MsgID, _OnTownStatueRes);
            NetProcess.AddMsgHandler(WorldGuildStorageSettingRes.MsgID, _OnRecvWorldGuildStorageSettingRes);
            NetProcess.AddMsgHandler(WorldGuildStorageItemSync.MsgID, _OnSorageItemUpdated);
            NetProcess.AddMsgHandler(WorldGuildAddStorageRes.MsgID, _OnRecvWorldGuildAddStorageRes);
            NetProcess.AddMsgHandler(WorldGuildDelStorageRes.MsgID, _OnRecvWorldGuildDelStorageRes);
            NetProcess.AddMsgHandler(WorldGuildAuctionNotify.MsgID, _OnWorldGuildAuctionNotify);
            NetProcess.AddMsgHandler(WorldGuildAuctionItemRes.MsgID, _OnWorldGuildAuctionItemRes);
            NetProcess.AddMsgHandler(WorldGuildAuctionFixRes.MsgID, _OnWorldGuildAuctionFixRes);
            NetProcess.AddMsgHandler(WorldGuildAuctionBidRes.MsgID, _OnWorldGuildAuctionBidRes);
            NetProcess.AddMsgHandler(WorldGuildGetTerrDayRewardRes.MsgID, _OnWorldGuildGetTerrDayRewardRes);

            NetProcess.AddMsgHandler(WorldNotifyGameStartTime.MsgID, _OnSyncWordStartTime);
        }

        void UnRegisterNetHandler()
        {
            NetProcess.RemoveMsgHandler(WorldGuildSyncInfo.MsgID, _OnInitMyGuild);
            NetProcess.RemoveMsgHandler(WorldGuildInviteNotify.MsgID, _OnInviteJoinGuild);
            NetProcess.RemoveMsgHandler(WorldFigureStatueSync.MsgID, _OnTownStatueRes);
            NetProcess.RemoveMsgHandler(WorldGuildStorageSettingRes.MsgID, _OnRecvWorldGuildStorageSettingRes);
            NetProcess.RemoveMsgHandler(WorldGuildStorageItemSync.MsgID, _OnSorageItemUpdated);
            NetProcess.RemoveMsgHandler(WorldGuildAddStorageRes.MsgID, _OnRecvWorldGuildAddStorageRes);
            NetProcess.RemoveMsgHandler(WorldGuildDelStorageRes.MsgID, _OnRecvWorldGuildDelStorageRes);
            NetProcess.RemoveMsgHandler(WorldGuildAuctionNotify.MsgID, _OnWorldGuildAuctionNotify);
            NetProcess.RemoveMsgHandler(WorldGuildAuctionItemRes.MsgID, _OnWorldGuildAuctionItemRes);
            NetProcess.RemoveMsgHandler(WorldGuildAuctionFixRes.MsgID, _OnWorldGuildAuctionFixRes);
            NetProcess.RemoveMsgHandler(WorldGuildAuctionBidRes.MsgID, _OnWorldGuildAuctionBidRes);
            NetProcess.RemoveMsgHandler(WorldGuildGetTerrDayRewardRes.MsgID, _OnWorldGuildGetTerrDayRewardRes);

            NetProcess.RemoveMsgHandler(WorldNotifyGameStartTime.MsgID, _OnSyncWordStartTime);
        }

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PlayerDataGuildUpdated, _OnPlayerDataGuildUpdated);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountChanged);
        }

        void _UnregisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PlayerDataGuildUpdated, _OnPlayerDataGuildUpdated);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountChanged);
        }

        void _OnPlayerDataGuildUpdated(UIEvent a_event)
        {
            if (a_event.Param1 == null)
            {
                return;
            }

            if (a_event.Param1.GetType() != typeof(SceneObjectAttr))
            {
                return;
            }

            SceneObjectAttr attr = (SceneObjectAttr)(a_event.Param1);
            if (attr == SceneObjectAttr.SOA_GUILD_POST)
            {
                if (PlayerBaseData.GetInstance().eGuildDuty == EGuildDuty.Invalid)
                {
                    _ClearMyGuild();
                    //SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_has_dismissed"));
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildHasDismissed);
                }
                else
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildChangeDutySuccess);
                }                
            }
        }

        void _OnCountChanged(UIEvent a_event)
        {
            if (HasSelfGuild())
            {
                string strKey = a_event.Param1 as string;
                if (strKey == m_strExchangeCoolTime)
                {
                    myGuild.nExchangeCoolTime = (uint)CountDataManager.GetInstance().GetCount(m_strExchangeCoolTime);
                }
            }
        }

        void _BindSelfGuildNetMsg()
        {
            NetProcess.AddMsgHandler(WorldGuildSyncStreamInfo.MsgID, _OnNetSyncMyGuild);
            NetProcess.AddMsgHandler(WorldGuildTableNewMember.MsgID, _OnNetAddTableMember);
            NetProcess.AddMsgHandler(WorldGuildTableDelMember.MsgID, _OnNetRemoveTableMember);
            NetProcess.AddMsgHandler(WorldGuildTableFinish.MsgID, _OnNetNotifyTableFinished);
            NetProcess.AddMsgHandler(WorldGuildBattleRecordSync.MsgID, _OnNetSyncBattleRecord);
            NetProcess.AddMsgHandler(WorldGuildBattleRecordRes.MsgID, _OnNetGuildBattleRecordRes);
            NetProcess.AddMsgHandler(WorldGuildChallengeRes.MsgID, _OnGuildChallengeRes);
            NetProcess.AddMsgHandler(WorldGuildBattleInspireInfoRes.MsgID, _OnGuildBattleInspireRes);
            NetProcess.AddMsgHandler(WorldGuildBattleLotteryRes.MsgID, _OnGuildBattleLotteryRes);

            NetProcess.AddMsgHandler(WorldGuildBattleStatusSync.MsgID, _OnNetSyncGuildBattleState);
            NetProcess.AddMsgHandler(WorldGuildChallengeInfoSync.MsgID, _OnNetSyncGuildAttackCityRes);

            NetProcess.AddMsgHandler(WorldMatchStartRes.MsgID, _onStartBattle);
            NetProcess.AddMsgHandler(WorldMatchCancelRes.MsgID, _onCancelBattle);
            NetProcess.AddMsgHandler(WorldGuildDungeonCopyRes.MsgID, _OnWorldGuildDungeonCopyRes);
            NetProcess.AddMsgHandler(WorldGuildDungeonDamageRankRes.MsgID, _OnWorldGuildDungeonDamageRankRes);
            NetProcess.AddMsgHandler(WorldGuildDungeonInfoRes.MsgID, _OnWorldGuildDungeonInfoRes);
            NetProcess.AddMsgHandler(WorldGuildDungeonLotteryRes.MsgID, _OnWorldGuildDungeonLotteryRes);
            NetProcess.AddMsgHandler(WorldGuildDungeonStatusSync.MsgID, _OnWorldGuildDungeonStatusSync);
            NetProcess.AddMsgHandler(WorldGuildDungeonStatueRes.MsgID, _OnWorldGuildDungeonStatueRes);
            NetProcess.AddMsgHandler(WorldGuildDungeonBossDeadNotify.MsgID, _OnWorldGuildDungeonBossDeadNotify);
            NetProcess.AddMsgHandler(WorldGuildSetJoinLevelRes.MsgID, _OnWorldGuildSetJoinLevelRes);
            
            NetProcess.AddMsgHandler(WorldGuildEmblemUpRes.MsgID, _OnWorldGuildEmblemUpRes);
            NetProcess.AddMsgHandler(WorldGuildSetDungeonTypeRes.MsgID, _OnWorldGuildSetDungeonTypeRes);

            //公会兼并
            NetProcess.AddMsgHandler(WorldGuildWatchCanMergerRet.MsgID, _OnCanMergerGuildListRes);
            NetProcess.AddMsgHandler(WorldGuildMergerRequestOperatorRet.MsgID, _OnGuildMergerRequestOperatorRes);
            NetProcess.AddMsgHandler(WorldGuildReceiveMergerRequestRet.MsgID, _OnGuildReceiveMergerRequestRes);
            NetProcess.AddMsgHandler(WorldGuildWatchHavedMergerRequestRet.MsgID, _OnGuildWatchHavedMergerRequestRes);
            NetProcess.AddMsgHandler(WorldGuildAcceptOrRefuseOrCancleMergerRequestRet.MsgID, _OnGuildAcceptOrRefuseOrCancleMergerRequestRes);
            NetProcess.AddMsgHandler(WorldGuildMemberListRes.MsgID, _OnCanMergerdGuildMembersRes);
            NetProcess.AddMsgHandler(WorldGuildSyncMergerInfo.MsgID, _OnGuildSyncMergerInfo);

            NetProcess.AddMsgHandler(WorldGuildEventListRes.MsgID, _OnGuildEventListRes);
          
        }

      
        void _UnBindSelfGuildNetMsg()
        {
            NetProcess.RemoveMsgHandler(WorldGuildSyncStreamInfo.MsgID, _OnNetSyncMyGuild);
            NetProcess.RemoveMsgHandler(WorldGuildTableNewMember.MsgID, _OnNetAddTableMember);
            NetProcess.RemoveMsgHandler(WorldGuildTableDelMember.MsgID, _OnNetRemoveTableMember);
            NetProcess.RemoveMsgHandler(WorldGuildTableFinish.MsgID, _OnNetNotifyTableFinished);
            NetProcess.RemoveMsgHandler(WorldGuildBattleRecordSync.MsgID, _OnNetSyncBattleRecord);
            NetProcess.RemoveMsgHandler(WorldGuildBattleRecordRes.MsgID, _OnNetGuildBattleRecordRes);
            NetProcess.RemoveMsgHandler(WorldGuildChallengeRes.MsgID, _OnGuildChallengeRes);
            NetProcess.RemoveMsgHandler(WorldGuildBattleInspireInfoRes.MsgID, _OnGuildBattleInspireRes);
            NetProcess.RemoveMsgHandler(WorldGuildBattleLotteryRes.MsgID, _OnGuildBattleLotteryRes);

            NetProcess.RemoveMsgHandler(WorldGuildBattleStatusSync.MsgID, _OnNetSyncGuildBattleState);
            NetProcess.RemoveMsgHandler(WorldGuildChallengeInfoSync.MsgID, _OnNetSyncGuildAttackCityRes);

            NetProcess.RemoveMsgHandler(WorldMatchStartRes.MsgID, _onStartBattle);
            NetProcess.RemoveMsgHandler(WorldMatchCancelRes.MsgID, _onCancelBattle);
            NetProcess.RemoveMsgHandler(WorldGuildDungeonCopyRes.MsgID, _OnWorldGuildDungeonCopyRes);
            NetProcess.RemoveMsgHandler(WorldGuildDungeonDamageRankRes.MsgID, _OnWorldGuildDungeonDamageRankRes);
            NetProcess.RemoveMsgHandler(WorldGuildDungeonInfoRes.MsgID, _OnWorldGuildDungeonInfoRes);
            NetProcess.RemoveMsgHandler(WorldGuildDungeonLotteryRes.MsgID, _OnWorldGuildDungeonLotteryRes);
            NetProcess.RemoveMsgHandler(WorldGuildDungeonStatusSync.MsgID, _OnWorldGuildDungeonStatusSync);
            NetProcess.RemoveMsgHandler(WorldGuildDungeonStatueRes.MsgID, _OnWorldGuildDungeonStatueRes);
            NetProcess.RemoveMsgHandler(WorldGuildDungeonBossDeadNotify.MsgID, _OnWorldGuildDungeonBossDeadNotify);
            NetProcess.RemoveMsgHandler(WorldGuildSetJoinLevelRes.MsgID, _OnWorldGuildSetJoinLevelRes);

            
			
			
			
			
            
            NetProcess.RemoveMsgHandler(WorldGuildEmblemUpRes.MsgID, _OnWorldGuildEmblemUpRes);
            NetProcess.RemoveMsgHandler(WorldGuildSetDungeonTypeRes.MsgID, _OnWorldGuildSetDungeonTypeRes);

            //公会兼并
            NetProcess.RemoveMsgHandler(WorldGuildWatchCanMergerRet.MsgID, _OnCanMergerGuildListRes);
            NetProcess.RemoveMsgHandler(WorldGuildMergerRequestOperatorRet.MsgID, _OnGuildMergerRequestOperatorRes);
            NetProcess.RemoveMsgHandler(WorldGuildReceiveMergerRequestRet.MsgID, _OnGuildReceiveMergerRequestRes);
            NetProcess.RemoveMsgHandler(WorldGuildWatchHavedMergerRequestRet.MsgID, _OnGuildWatchHavedMergerRequestRes);
            NetProcess.RemoveMsgHandler(WorldGuildAcceptOrRefuseOrCancleMergerRequestRet.MsgID, _OnGuildAcceptOrRefuseOrCancleMergerRequestRes);
            NetProcess.RemoveMsgHandler(WorldGuildMemberListRes.MsgID, _OnCanMergerdGuildMembersRes);
            NetProcess.RemoveMsgHandler(WorldGuildSyncMergerInfo.MsgID, _OnGuildSyncMergerInfo);

            NetProcess.RemoveMsgHandler(WorldGuildEventListRes.MsgID, _OnGuildEventListRes);

        }

        private void _onStartBattle(MsgDATA a_msgData)
        {
            if (null == a_msgData)
            {
                return ;
            }

            WorldMatchStartRes msgRet = new WorldMatchStartRes();
            msgRet.decode(a_msgData.bytes);

            if (msgRet == null)
            {
                return;
            }

            if (msgRet.result != 0)
            {
                if (msgRet.result == (int)ProtoErrorCode.PREMIUM_LEAGUE_PRELIMINAY_ALREADY_LOSE)
                {
                    //none
                }
                else
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.result);
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchFailed);
                return;
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchStartSuccess);
        }

        private void _onCancelBattle(MsgDATA a_msgData)
        {
            if (null == a_msgData)
            {
                return ;
            }

            WorldMatchCancelRes msgRet = new WorldMatchCancelRes();
            msgRet.decode(a_msgData.bytes);

            if (msgRet == null)
            {
                return;
            }

            if (msgRet.result != 0)
            {
                SystemNotifyManager.SystemNotify((int)msgRet.result);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchCancelFailed);
                return;
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchCancelSuccess);
        }


        void _InitMyGuild(MsgDATA a_msgData)
        {
            if (a_msgData != null)
            {
                WorldGuildSyncInfo msgSync = new WorldGuildSyncInfo();
                msgSync.decode(a_msgData.bytes);

                if (msgSync.info.id <= 0)
                {
                    return;
                }

                myGuild = new GuildMyData();
                myGuild.uGUID = msgSync.info.id;
                myGuild.strName = msgSync.info.name;
                myGuild.nLevel = msgSync.info.level;
                myGuild.nJoinLevel = msgSync.info.joinLevel;
                myGuild.emblemLevel = msgSync.info.emblemLevel;
                myGuild.dungeonType = msgSync.info.dungeonType;
                OnGuildLevelChanged(msgSync.info.level, myGuild.nLevel);
                myGuild.nMemberCount = msgSync.info.memberNum;
                myGuild.nMemberMaxCount = TableManager.GetInstance().GetTableItem<GuildTable>(myGuild.nLevel).MemberNum;
                myGuild.nFund = (int)msgSync.info.fund;
                UpdateGuildBuildingRedPoint();
                UpdateGuildEmblemRedPoint();
                myGuild.strDeclaration = msgSync.info.declaration;
                myGuild.strNotice = msgSync.info.announcement;
                myGuild.nDismissTime = msgSync.info.dismissTime;
                myGuild.nExchangeCoolTime = (uint)CountDataManager.GetInstance().GetCount(m_strExchangeCoolTime);
                myGuild.leaderData.strName = msgSync.info.leaderName;
                myGuild.leaderData.eGuildDuty = EGuildDuty.Leader;
                myGuild.nWinProbability = msgSync.info.winProbability;
                _OnStorageSettingSync(GuildStorageSetting.GSS_WIN_PROBABILITY, msgSync.info.winProbability);
                myGuild.nLoseProbability = msgSync.info.loseProbability;
                _OnStorageSettingSync(GuildStorageSetting.GSS_LOSE_PROBABILITY, msgSync.info.loseProbability);
                myGuild.nStorageAddPost = msgSync.info.storageAddPost;
                _OnStorageSettingSync(GuildStorageSetting.GSS_STORAGE_ADD_POST, msgSync.info.storageAddPost);
                myGuild.nStorageDelPost = msgSync.info.storageDelPost;
                _OnStorageSettingSync(GuildStorageSetting.GSS_STORAGE_DEL_POST, msgSync.info.storageDelPost);
                myGuild.lastWeekFunValue = msgSync.info.lastWeekAddedFund;
                myGuild.thisWeekFunValue = msgSync.info.weekAddedFund;
                GuildInviteList.Clear();
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildInviteNoticeUpdate);

                #region init table members
                for (int i = 0; i < myGuild.arrTableMembers.Length; ++i)
                {
                    myGuild.arrTableMembers[i] = null;
                }
                for (int i = 0; i < msgSync.info.tableMembers.Length; ++i)
                {
                    GuildTableMember member = msgSync.info.tableMembers[i];
                    if (member.seat >= 0 && member.seat < myGuild.arrTableMembers.Length)
                    {
                        myGuild.arrTableMembers[member.seat] = member;
                    }
                }
                #endregion

                #region init building data
                for (int i = 0; i < msgSync.info.building.Length; ++i)
                {
                    GuildBuilding source = msgSync.info.building[i];
                    GuildBuildingData target = new GuildBuildingData();
                    target.eType = (GuildBuildingType)source.type;
                    target.nLevel = source.level;
                    myGuild.dictBuildings.Add(target.eType, target);
                }

                _UpdateBuildingData();
                #endregion

                _InitGuildBattle(msgSync.info.guildBattleInfo);

                _BindSelfGuildNetMsg();
            }
        }

        void _ClearMyGuild()
        {
            myGuild = null;
            GuildInviteList.Clear();

            _ClearGuildBattle();
            _UnBindSelfGuildNetMsg();
        }

        void _OnNetSyncMyGuild(MsgDATA a_msgData)
        {
            if (a_msgData != null && myGuild != null)
            {
                int pos = 0;

                while (true)
                {
                    byte type = 0;
                    BaseDLL.decode_int8(a_msgData.bytes, ref pos, ref type);
                    if (type == 0)
                    {
                        break;
                    }

					if (type == (byte)GuildAttr.GA_NAME)
					{
						UInt16 strLen = 0;
						BaseDLL.decode_uint16(a_msgData.bytes, ref pos, ref strLen);
						byte[] strBytes = new byte[strLen];
						for (int i = 0; i < strLen; i++)
						{
							BaseDLL.decode_int8(a_msgData.bytes, ref pos, ref strBytes[i]);
						}

						myGuild.strName = Encoding.UTF8.GetString(strBytes);
					}
					else if (type == (byte)GuildAttr.GA_LEVEL)
					{
						byte data = 0;
						BaseDLL.decode_int8(a_msgData.bytes, ref pos, ref data);
						myGuild.nLevel = data;
                        OnGuildLevelChanged(data, myGuild.nLevel);
						myGuild.nMemberMaxCount = TableManager.GetInstance().GetTableItem<GuildTable>(myGuild.nLevel).MemberNum;
					}
					else if (type == (byte)GuildAttr.GA_DECLARATION)
					{
						UInt16 strLen = 0;
						BaseDLL.decode_uint16(a_msgData.bytes, ref pos, ref strLen);
						byte[] strBytes = new byte[strLen];
						for (int i = 0; i < strLen; i++)
						{
							BaseDLL.decode_int8(a_msgData.bytes, ref pos, ref strBytes[i]);
						}

						myGuild.strDeclaration = Encoding.UTF8.GetString(strBytes);
					}
					else if (type == (byte)GuildAttr.GA_FUND)
					{
						BaseDLL.decode_int32(a_msgData.bytes, ref pos, ref myGuild.nFund);
                        UpdateGuildBuildingRedPoint();
					}
					else if (type == (byte)GuildAttr.GA_ANNOUNCEMENT)
					{
						UInt16 strLen = 0;
						BaseDLL.decode_uint16(a_msgData.bytes, ref pos, ref strLen);
						byte[] strBytes = new byte[strLen];
						for (int i = 0; i < strLen; i++)
						{
							BaseDLL.decode_int8(a_msgData.bytes, ref pos, ref strBytes[i]);
						}

						myGuild.strNotice = Encoding.UTF8.GetString(strBytes);
					}
					else if (type == (byte)GuildAttr.GA_MEMBER_NUM)
					{
						ushort count = 0;
						BaseDLL.decode_uint16(a_msgData.bytes, ref pos, ref count);
						myGuild.nMemberCount = count;
					}
					else if (type == (byte)GuildAttr.GA_DISMISS_TIME)
					{
						BaseDLL.decode_uint32(a_msgData.bytes, ref pos, ref myGuild.nDismissTime);
					}
					else if (type == (byte)GuildAttr.GA_LEADER_NAME)
					{
						UInt16 strLen = 0;
						BaseDLL.decode_uint16(a_msgData.bytes, ref pos, ref strLen);
						byte[] strBytes = new byte[strLen];
						for (int i = 0; i < strLen; i++)
						{
							BaseDLL.decode_int8(a_msgData.bytes, ref pos, ref strBytes[i]);
						}

						myGuild.leaderData.strName = Encoding.UTF8.GetString(strBytes);
					}
					else if (type == (byte)GuildAttr.GA_BUILDING)
					{
						byte size = 0;
						BaseDLL.decode_int8(a_msgData.bytes, ref pos, ref size);
						for (int i = 0; i < size; ++i)
						{
							byte buildingType = 0;
							byte level = 0;
							BaseDLL.decode_int8(a_msgData.bytes, ref pos, ref buildingType);
							BaseDLL.decode_int8(a_msgData.bytes, ref pos, ref level);

							GuildBuildingType eType = (GuildBuildingType)buildingType;
							if (myGuild.dictBuildings.ContainsKey(eType))
							{
								myGuild.dictBuildings[eType].nLevel = level;
							}
						}
                        UpdateGuildEmblemRedPoint();
                        UpdateGuildSetBossDiffRedPoint();
					}
					else if (type == (byte)GuildAttr.GA_ENROLL_TERRID)
					{
						byte data = 0;
						BaseDLL.decode_int8(a_msgData.bytes, ref pos, ref data);

                        if(m_guildBattleType == GuildBattleType.GBT_CROSS)
                        {
                            myGuild.nTargetCrossManorID = data;
                        }
                        else
                        {
                            myGuild.nTargetManorID = data;
                        }	
					}
					else if (type == (byte)GuildAttr.GA_BATTLE_SCORE)
					{
						BaseDLL.decode_int32(a_msgData.bytes, ref pos, ref myGuild.nBattleScore);
					}
					else if (type == (byte)GuildAttr.GA_OCCUPY_TERRID)
					{
						byte data = 0;
						BaseDLL.decode_int8(a_msgData.bytes, ref pos, ref data);
						myGuild.nSelfManorID = data;
					}
                    else if(type == (byte)GuildAttr.GA_OCCUPY_CROSS_TERRID)
                    {
                        byte data = 0;
                        BaseDLL.decode_int8(a_msgData.bytes, ref pos, ref data);
                        myGuild.nSelfCrossManorID = data;
                    }
                    else if(type == (byte)GuildAttr.GA_HISTORY_TERRID)
                    {
                        byte data = 0;
                        BaseDLL.decode_int8(a_msgData.bytes, ref pos, ref data);
                        myGuild.nSelfHistoryManorID = data;
                    }
                    else if (type == (byte)GuildAttr.GA_JOIN_LEVEL)
                    {
                        UInt32 data = 0;
                        BaseDLL.decode_uint32(a_msgData.bytes, ref pos, ref data);
                        myGuild.nJoinLevel = data;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildJoinLvUpdate);
                    }
                    else if (type == (byte)GuildAttr.GA_DUNGEON_TYPE)
                    {
                        UInt32 data = 0;
                        BaseDLL.decode_uint32(a_msgData.bytes, ref pos, ref data);
                        myGuild.dungeonType = data;

                        _UpdateBuildingData();
                    }
					else if (type == (byte)GuildAttr.GA_INSPIRE)
					{
						byte data = 0;
						BaseDLL.decode_int8(a_msgData.bytes, ref pos, ref data);
						myGuild.nInspire = data;
					}
					else if (type == (byte)GuildAttr.GA_WIN_PROBABILITY)
					{
						byte data = 0;
						BaseDLL.decode_int8(a_msgData.bytes, ref pos, ref data);
						myGuild.nWinProbability = data;
                        _OnStorageSettingSync(GuildStorageSetting.GSS_WIN_PROBABILITY,data);
					}
					else if (type == (byte)GuildAttr.GA_LOSE_PROBABILITY)
					{
						byte data = 0;
						BaseDLL.decode_int8(a_msgData.bytes, ref pos, ref data);
						myGuild.nLoseProbability = data;
                        _OnStorageSettingSync(GuildStorageSetting.GSS_LOSE_PROBABILITY, data);
                    }
					else if (type == (byte)GuildAttr.GA_STORAGE_ADD_POST)
					{
						byte data = 0;
						BaseDLL.decode_int8(a_msgData.bytes, ref pos, ref data);
						myGuild.nStorageAddPost = data;
                        _OnStorageSettingSync(GuildStorageSetting.GSS_STORAGE_ADD_POST, data);
                    }
					else if (type == (byte)GuildAttr.GA_STORAGE_DEL_POST)
					{
						byte data = 0;
						BaseDLL.decode_int8(a_msgData.bytes, ref pos, ref data);
						myGuild.nStorageDelPost = data;
                        _OnStorageSettingSync(GuildStorageSetting.GSS_STORAGE_DEL_POST, data);
                    }
                    else if (type == (byte)GuildAttr.GA_LAST_WEEK_ADD_FUND)
                    {
                        UInt32 data = 0;
                        BaseDLL.decode_uint32(a_msgData.bytes, ref pos, ref data);
                        myGuild.lastWeekFunValue = data;
                    }
                    else if (type == (byte)GuildAttr.GA_THIS_WEEK_ADD_FUND)
                    {
                        UInt32 data = 0;
                        BaseDLL.decode_uint32(a_msgData.bytes, ref pos, ref data);
                        myGuild.thisWeekFunValue = data;
                    }

                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildBaseInfoUpdated);
            }
        }

        void _OnNetAddTableMember(MsgDATA a_msgData)
        {
            WorldGuildTableNewMember msgRet = new WorldGuildTableNewMember();
            msgRet.decode(a_msgData.bytes);

            int nIndex = msgRet.member.seat;
            if (nIndex >= 0 && nIndex < myGuild.arrTableMembers.Length)
            {
                myGuild.arrTableMembers[nIndex] = msgRet.member;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildAddTableMember, nIndex);
            }
        }

        void _OnNetRemoveTableMember(MsgDATA a_msgData)
        {
            WorldGuildTableDelMember msgRet = new WorldGuildTableDelMember();
            msgRet.decode(a_msgData.bytes);

            for (int i = 0; i < myGuild.arrTableMembers.Length; ++i)
            {
                if (myGuild.arrTableMembers[i].id == msgRet.id)
                {
                    myGuild.arrTableMembers[i] = null;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildRemoveTableMember, i);
                    break;
                }
            }
        }

        void _OnNetNotifyTableFinished(MsgDATA a_msgData)
        {
            for (int i = 0; i < myGuild.arrTableMembers.Length; ++i)
            {
                myGuild.arrTableMembers[i] = null;
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildTableFinished);
        }


        //[EnterGameMessageHandle(WorldGuildSyncInfo.MsgID)]
        //[MessageHandle(WorldGuildSyncInfo.MsgID)]
        void _OnInitMyGuild(MsgDATA a_data)
        {
            if (a_data != null)
            {
                _InitMyGuild(a_data);
            }
            else
            {
                _ClearMyGuild();
            }
        }

        //[MessageHandle(WorldGuildInviteNotify.MsgID)]
        void _OnInviteJoinGuild(MsgDATA a_data)
        {
            WorldGuildInviteNotify notify = new WorldGuildInviteNotify();
            notify.decode(a_data.bytes);

            if (HasSelfGuild())
            {
                return;
            }

            bool bFind = false;
            bool bNeedNotice = false;

            for (int i = 0; i < GuildInviteList.Count; i++)
            {
                if (GuildInviteList[i].inviterId == notify.inviterId)
                {
                    if (GuildInviteList[i].guildId != notify.guildId)
                    {
                        bNeedNotice = true;
                    }

                    GuildInviteList[i] = notify;
                    bFind = true;

                    break;
                }
            }

            if (!bFind)
            {
                GuildInviteList.Add(notify);
                bNeedNotice = true;
            }

            if (bNeedNotice)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildInviteNoticeUpdate);
            }

            AddInviteGuildNotify(notify);
        }

        public void AddInviteGuildNotify(WorldGuildInviteNotify notify)
        {
            RelationData inviteFriend = new RelationData();
            inviteFriend.uid = notify.inviterId;
            inviteFriend.name = notify.inviterName;
            inviteFriend.level = notify.inviterLevel;
            inviteFriend.occu = notify.inviterOccu;
            inviteFriend.vipLv = notify.inviterVipLevel;
            inviteFriend.type = (byte)RelationType.RELATION_STRANGER;

            ChatManager.GetInstance().AddAskForPupilInvite(inviteFriend, TR.Value("tap_invite_Guild", notify.guildName, notify.guildId));
        }

        //[MessageHandle(WorldFigureStatueSync.MsgID)]
        void _OnTownStatueRes(MsgDATA a_data)
        {
            WorldFigureStatueSync res = new WorldFigureStatueSync();
            res.decode(a_data.bytes);

            TownStatueList.Clear();

            for(int i = 0; i < res.figureStatue.Length; i++)
            {
                TownStatueList.Add(res.figureStatue[i]);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildTownStatueUpdate);
        }

        #region guild battle
        List<GuildBattleRecord> m_arrSelfBattleRecords = new List<GuildBattleRecord>();
        List<GuildBattleRecord> m_arrBattleRecords = new List<GuildBattleRecord>();
        int m_nLatestRecordID = -1;
        bool m_bGuildBattleSignUp = false;
        bool m_bGuildBattleEnter = false;

        GuildBattleType m_guildBattleType = GuildBattleType.GBT_INVALID;
        EGuildBattleState m_guildBattleState = EGuildBattleState.Invalid;
        uint m_nStateEndTime = 0;
        GuildTerritoryBaseInfo[] m_arrGuildManorInfos = null;
        bool m_bBattleNotifyInited = false;
        bool m_bHasLotteryed = true;

        public bool isBattleNotifyInited
        {
            get { return m_bBattleNotifyInited; }
            set { m_bBattleNotifyInited = value; }
        }

        public GuildTerritoryBaseInfo GetGuildTerritoryBaseInfo(int a_nID)
        {
            if (m_arrGuildManorInfos != null)
            {
                for (int i = 0; i < m_arrGuildManorInfos.Length; ++i)
                {
                    if (m_arrGuildManorInfos[i].terrId == a_nID)
                    {
                        return m_arrGuildManorInfos[i];
                    }
                }
            }
            return null;
        }

        public string GetManorOwner(int a_nID)
        {
            if (m_arrGuildManorInfos != null)
            {
                for (int i = 0; i < m_arrGuildManorInfos.Length; ++i)
                {
                    if (m_arrGuildManorInfos[i].terrId == a_nID)
                    {
                        return m_arrGuildManorInfos[i].guildName;
                    }
                }
            }
            return string.Empty;
        }

        public bool IsSameGuild(ulong guildID)
        {
            return null != myGuild && myGuild.uGUID == guildID;
        }

        public bool IsInBattlePrepareScene()
        {
            ClientSystemTown town = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (town != null && GuildDataManager.GetInstance().HasSelfGuild())
            {
                int nManorID = GuildDataManager.GetInstance().myGuild.nTargetManorID;
                GuildTerritoryTable manorTable = TableManager.GetInstance().GetTableItem<GuildTerritoryTable>(nManorID);
                if (manorTable != null)
                {
                    if (town.CurrentSceneID == manorTable.SceneID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public int GetLatestBattleRecordID()
        {
            return m_nLatestRecordID;
        }

        public int GetGuildBattleWinTimes()
        {
            int nCount = 0;
            for (int i = 0; i < m_arrSelfBattleRecords.Count; ++i)
            {
                if (m_arrSelfBattleRecords[i].winner.id == PlayerBaseData.GetInstance().RoleID)
                {
                    nCount++;
                }
            }
            return nCount;
        }

        public int GetGuildBattleFailTimes()
        {
            int nCount = 0;
            for (int i = 0; i < m_arrSelfBattleRecords.Count; ++i)
            {
                if (m_arrSelfBattleRecords[i].loser.id == PlayerBaseData.GetInstance().RoleID)
                {
                    nCount++;
                }
            }
            return nCount;
        }

        public EGuildBattleState GetGuildBattleState()
        {
            return m_guildBattleState;
        }

        public GuildBattleType GetGuildBattleType()
        {
            return m_guildBattleType;
        }

        public uint GetGuildBattleStateEndTime()
        {
            return m_nStateEndTime;
        }

        /// <summary>
        /// 公会战，是否有目标领地（已报名）
        /// </summary>
        /// <returns></returns>
        public bool HasSelfManor()
        {
            if (HasSelfGuild())
            {
                return myGuild.nSelfManorID > 0;
            }
            return false;
        }

        public bool HasSelfCrossManor()
        {
            if (HasSelfGuild())
            {
                return myGuild.nSelfCrossManorID > 0;
            }
            return false;
        }

        /// <summary>
        /// 公会战，是否有目标领地（已报名）
        /// </summary>
        /// <returns></returns>
        public bool HasTargetManor()
        {
            if (HasSelfGuild())
            {
                return (myGuild.nTargetManorID > 0 || myGuild.nTargetCrossManorID > 0);
            }
            return false;
        }

        public string GetTargetManorName()
        {
            if (HasTargetManor())
            {
                int TargetManorID = myGuild.nTargetManorID;

                if(m_guildBattleType == GuildBattleType.GBT_CROSS)
                {
                    TargetManorID = myGuild.nTargetCrossManorID;
                }

                GuildTerritoryTable tableData = TableManager.GetInstance().GetTableItem<GuildTerritoryTable>(TargetManorID);
                if (tableData == null)
                {
                    Logger.LogErrorFormat("加载公会领地表错误，id{0}不存在", TargetManorID);
                    return string.Empty;
                }

                return tableData.Name;
            }
            return string.Empty;
        }

        public static int GetJoinGuildMinLv()
        {
            var table = TableManager.GetInstance().GetTableItem<FunctionUnLock>((int)FunctionUnLock.eFuncType.Guild);
            if (table != null)
            {
                return table.FinishLevel;
            }

            return 18;
        }

        public static int GetJoinGuildMaxLevel()
        {
            var table = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_GUILD_SET_MAX_JOIN_LEVEL);
            if(table != null)
            {
                return table.Value;
            }
            return iJoinGuildMaxLevel;
        }
        public void BattleInspire()
        {
            if (_CheckSelfGuild())
            {
                WorldGuildBattleInspireReq msg = new WorldGuildBattleInspireReq();
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

                WaitNetMessageManager.GetInstance().Wait<WorldGuildBattleInspireRes>(msgRet =>
                {
                    if (msgRet == null)
                    {
                        return;
                    }

                    // 公会有可能中途解散了，所以需要再检查一次
                    if (_CheckSelfGuild())
                    {
                        if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                        {
                            SystemNotifyManager.SystemNotify((int)msgRet.result);
                        }
                        else
                        {
                            myGuild.nInspire = msgRet.inspire;
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildInspireSuccess);
                        }
                    }
                });
            }
        }

        public void BattleSignup(int a_nManorID)
        {
            if (!_CheckSelfGuild())
            {
                return;
            }

            if(m_guildBattleType == GuildBattleType.GBT_CROSS)
            {
                if (!_CheckPermission(EGuildPermission.StartGuildCrossBattle))
                {
                    return;
                }
            }
            else
            {
                if (!_CheckPermission(EGuildPermission.StartGuildBattle))
                {
                    return;
                }
            }

            WorldGuildBattleReq msg = new WorldGuildBattleReq();
            msg.terrId = (byte)a_nManorID;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<WorldGuildBattleRes>(msgRet =>
            {
                if (msgRet == null)
                {
                    return;
                }

                // 公会有可能中途解散了，所以需要再检查一次
                if (_CheckSelfGuild())
                {
                    if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                    {
                        SystemNotifyManager.SystemNotify((int)msgRet.result);
                    }
                    else
                    {
                        if (m_guildBattleType == GuildBattleType.GBT_CROSS)
                        {
                            myGuild.nTargetCrossManorID = a_nManorID;
                        }
                        else
                        {
                            myGuild.nTargetManorID = a_nManorID;
                        }

                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildSignupSuccess);
                    }
                }
            },false);
        }

        public void RequestManorInfo(int a_nManorID)
        {
            if (_CheckSelfGuild())
            {
                WorldGuildBattleTerritoryReq msg = new WorldGuildBattleTerritoryReq();
                msg.terrId = (byte)a_nManorID;
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

                WaitNetMessageManager.GetInstance().Wait<WorldGuildBattleTerritoryRes>(msgRet =>
                {
                    if (msgRet == null)
                    {
                        return;
                    }

                    // 公会有可能中途解散了，所以需要再检查一次
                    if (_CheckSelfGuild())
                    {
                        if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                        {
                            SystemNotifyManager.SystemNotify((int)msgRet.result);
                        }
                        else
                        {
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildManorInfoUpdated, msgRet.info);
                        }
                    }
                });
            }
        }

        public List<GuildBattleRecord> GetBattleRecords()
        {
            if (_CheckSelfGuild())
            {
                return m_arrBattleRecords;
            }
            return null;
        }

        public List<GuildBattleRecord> GetSelfBattleRecords()
        {
            return m_arrSelfBattleRecords;
        }

        public void RequestGetBattleReward(int a_nBoxID)
        {
            WorldGuildBattleReceiveReq msg = new WorldGuildBattleReceiveReq();
            msg.boxId = (byte)a_nBoxID;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<WorldGuildBattleReceiveRes>(msgRet =>
            {
                if (msgRet == null)
                {
                    return;
                }

                if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.result);
                }
                else
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildBattleRewardGetSuccess, msgRet.boxId);
                }
            });
        }

        public void RequestRanklist(SortListType a_type, int a_nStart = 0, int a_nCount = 100)
        {
            WorldSortListReq msg = new WorldSortListReq();
            msg.type = SortListDecoder.MakeSortListType(a_type);
            msg.start = (ushort)a_nStart;
            msg.num = (ushort)a_nCount;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait(WorldSortListRet.MsgID, msgRet =>
            {
                if (msgRet == null)
                {
                    return;
                }

                int pos = 0;
                BaseSortList sortList = SortListDecoder.Decode(msgRet.bytes, ref pos, msgRet.bytes.Length);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildBattleRanklistChanged, sortList);
            }, false);
        }

        public void RequestGuildManorWeekRanklist()
        {
            WorldSortListReq msg = new WorldSortListReq();
            msg.type = SortListDecoder.MakeSortListType(SortListType.SORTLIST_GUILD_BATTLE_WEEK_SCORE);
            msg.start = (ushort)0;
            msg.num = (ushort)20;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);
            WaitNetMessageManager.GetInstance().Wait(WorldSortListRet.MsgID, msgRet =>
            {
                if (msgRet == null)
                {
                    return;
                }
                int pos = 0;
                BaseSortList sortList = SortListDecoder.Decode(msgRet.bytes, ref pos, msgRet.bytes.Length);                
                ClientSystemManager.GetInstance().OpenFrame<GuildManorRankListFrame>(FrameLayer.Middle, sortList);        
            }, false);
        }
        public void RequestSelfRanklist()
        {
            WorldGuildBattleSelfSortListReq msg = new WorldGuildBattleSelfSortListReq();
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<WorldGuildBattleSelfSortListRes>(msgRet =>
            {
                if (msgRet == null)
                {
                    return;
                }

                if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.result);
                }
                else
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildSelfRankChanged, msgRet.memberRanking, msgRet.guildRanking);
                }
            });
        }

        public void StartBattle()
        {
            if(m_guildBattleType == GuildBattleType.GBT_CROSS)
            {
                if (ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_GUILD_CROSS_BATTLE))
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("跨服公会战系统目前已关闭");
                    return;
                }
            }

            WorldMatchStartReq req = new WorldMatchStartReq();
            req.type = (byte)PkType.PK_Guild;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);

            //WaitNetMessageManager.GetInstance().Wait<WorldMatchStartRes>(msgRet =>
            //{
            //    if (msgRet == null)
            //    {
            //        return;
            //    }

            //    if (msgRet.result != 0)
            //    {
            //        SystemNotifyManager.SystemNotify((int)msgRet.result);

            //        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchFailed);
            //        return;
            //    }

            //    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchStartSuccess);
            //});
        }

        public void CancelStartBattle()
        {
            WorldMatchCancelReq req = new WorldMatchCancelReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);

            //WaitNetMessageManager.GetInstance().Wait<WorldMatchCancelRes>(msgRet =>
            //{
            //    if (msgRet == null)
            //    {
            //        return;
            //    }

            //    if (msgRet.result != 0)
            //    {
            //        SystemNotifyManager.SystemNotify((int)msgRet.result);
            //        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchCancelFailed);
            //        return;
            //    }

            //    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchCancelSuccess);
            //});
        }

        public bool CheckGuildBattleSignUpRedPoint()
        {
            return m_bGuildBattleSignUp;
        }

        public void SetGuildBattleSignUpRedPoint(bool a_bHas)
        {
            if (a_bHas != m_bGuildBattleSignUp)
            {
                m_bGuildBattleSignUp = a_bHas;
                RedPointDataManager.GetInstance().NotifyRedPointChanged();
            }
        }

        public bool CheckGuildBattleEnterRedPoint()
        {
            return m_bGuildBattleEnter;
        }

        public void SetGuildBattleEnterRedPoint(bool a_bHas)
        {
            if (a_bHas != m_bGuildBattleEnter)
            {
                m_bGuildBattleEnter = a_bHas;
                RedPointDataManager.GetInstance().NotifyRedPointChanged();
            }
        }

        void _InitGuildBattle(GuildBattleBaseInfo a_data)
        {
            _InitGuildBattleRecord(a_data.selfGuildBattleRecord);

            Assert.IsNotNull(myGuild);

            EGuildBattleState eOldState = m_guildBattleState;
            m_guildBattleState = (EGuildBattleState)a_data.guildBattleStatus;
            m_guildBattleType = (GuildBattleType)a_data.guildBattleType;
            m_nStateEndTime = a_data.guildBattleStatusEndTime;

            myGuild.nInspire = a_data.inspire;
            myGuild.nSelfManorID = a_data.occupyTerrId;
            myGuild.nSelfCrossManorID = a_data.occupyCrossTerrId;
            myGuild.nSelfHistoryManorID = a_data.historyTerrId;

            if(m_guildBattleType == GuildBattleType.GBT_CROSS)
            {
                myGuild.nTargetCrossManorID = a_data.enrollTerrId;
            }
            else
            {
                myGuild.nTargetManorID = a_data.enrollTerrId;
            }
            
            myGuild.nBattleScore = (int)a_data.guildBattleScore;
            m_arrGuildManorInfos = a_data.terrInfos;

            if (m_guildBattleState == EGuildBattleState.Invalid)
            {
                myGuild.nTargetManorID = 0;
                myGuild.nTargetCrossManorID = 0;
                myGuild.nInspire = 0;
            }

            _SetupGuildBattleRedPoint();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildBattleStateChanged, eOldState, m_guildBattleState, null);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityDungeonUpdate);
        }

        void _ClearGuildBattle()
        {
            _ClearGuildBattleRecord();
            
            m_bGuildBattleSignUp = false;
            m_bGuildBattleEnter = false;
            m_arrGuildManorInfos = null;

            EGuildBattleState eOldState = m_guildBattleState;
            m_guildBattleState = EGuildBattleState.Invalid;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildBattleStateChanged, eOldState, m_guildBattleState, null);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityDungeonUpdate);
        }

        void _InitGuildBattleRecord(GuildBattleRecord[] a_selfBattleRecords)
        {
            _ClearGuildBattleRecord();

            if (a_selfBattleRecords != null)
            {
                m_arrSelfBattleRecords.AddRange(a_selfBattleRecords);
                m_arrBattleRecords.AddRange(a_selfBattleRecords);
            }
        }

        void _ClearGuildBattleRecord()
        {
            m_nLatestRecordID = -1;
            m_arrBattleRecords.Clear();
            m_arrSelfBattleRecords.Clear();
        }

        void _SetupGuildBattleRedPoint()
        {
            if (m_guildBattleState == EGuildBattleState.Signup)
            {
                SetGuildBattleSignUpRedPoint(true);
                SetGuildBattleEnterRedPoint(false);
            }
            else if (m_guildBattleState == EGuildBattleState.Preparing || m_guildBattleState == EGuildBattleState.Firing)
            {
                SetGuildBattleSignUpRedPoint(false);
                if (HasTargetManor())
                {
                    SetGuildBattleEnterRedPoint(true);
                }
            }
            else
            {
                SetGuildBattleSignUpRedPoint(false);
                SetGuildBattleEnterRedPoint(false);
            }
        }

        void _OnNetSyncGuildBattleState(MsgDATA a_msgData)
        {
            if (HasSelfGuild() == false)
            {
                return;
            }


            WorldGuildBattleStatusSync msgData = new WorldGuildBattleStatusSync();
            msgData.decode(a_msgData.bytes);

            EGuildBattleState eOldState = m_guildBattleState;
            m_guildBattleState = (EGuildBattleState)msgData.status;
            m_guildBattleType = (GuildBattleType)msgData.type;
            m_nStateEndTime = msgData.time;

            _SetupGuildBattleRedPoint();

            #region chat message
            {
                if (m_guildBattleState == EGuildBattleState.Preparing)
                {
                    if (HasTargetManor())
                    {
                        string strContent = "";

                        if (GetGuildBattleType() == GuildBattleType.GBT_NORMAL)
                        {
                            strContent = TR.Value("guild_chat_notify_signup_success", GetTargetManorName());
                        }
                        else if(GetGuildBattleType() == GuildBattleType.GBT_CROSS)
                        {
                            strContent = TR.Value("guild_chat_notify_signup_success", GetTargetManorName());
                        }
                        else
                        {
                            strContent = TR.Value("guild_chat_notify_attackcity_signup_success", GetTargetManorName());
                        }

                        strContent = strContent.Replace('[', '{');
                        strContent = strContent.Replace(']', '}');
                        ChatManager.GetInstance().AddLocalGuildChatData(strContent);
                    }
                }
                else if (m_guildBattleState == EGuildBattleState.Firing)
                {
                    if (GuildDataManager.GetInstance().HasTargetManor())
                    {
                        string strContent = TR.Value("guild_chat_notify_battle_start", GetTargetManorName());
                        strContent = strContent.Replace('[', '{');
                        strContent = strContent.Replace(']', '}');
                        ChatManager.GetInstance().AddLocalGuildChatData(strContent);
                    }
                }
                else if (eOldState == EGuildBattleState.Firing && m_guildBattleState == EGuildBattleState.Invalid)
                {
                    int iTargetManorID = myGuild.nTargetManorID;

                    if(m_guildBattleType == GuildBattleType.GBT_CROSS)
                    {
                        iTargetManorID = myGuild.nTargetCrossManorID;
                    }

                    for (int i = 0; i < msgData.endInfo.Length; ++i)
                    {
                        GuildBattleEndInfo info = msgData.endInfo[i];

                        if (info.terrId == iTargetManorID)
                        {
                            if (info.guildName == myGuild.strName)
                            {
                                ChatManager.GetInstance().AddLocalGuildChatData(TR.Value("guild_chat_notify_battle_win", GetTargetManorName(), GetTargetManorName()));
                            }
                            else
                            {
                                ChatManager.GetInstance().AddLocalGuildChatData(TR.Value("guild_chat_notify_battle_lose", GetTargetManorName(), info.guildName, GetTargetManorName()));
                            }

                            break;
                        }
                    }
                }
            }
            #endregion


            // 公会战进入结束状态
            if (eOldState == EGuildBattleState.Firing && m_guildBattleState == EGuildBattleState.LuckyDraw)
            {
                // 更新领地占领信息
                for (int i = 0; i < msgData.endInfo.Length; ++i)
                {
                    GuildBattleEndInfo info = msgData.endInfo[i];

                    for (int j = 0; j < m_arrGuildManorInfos.Length; ++j)
                    {
                        if (m_arrGuildManorInfos[j].terrId == info.terrId)
                        {
                            m_arrGuildManorInfos[j].guildName = info.guildName;
                            m_arrGuildManorInfos[j].serverName = info.guildServerName;

                            break;
                        }
                    }
                }

                // 清空报名信息，鼓舞信息
                myGuild.nTargetManorID = 0;
                myGuild.nTargetCrossManorID = 0;
                myGuild.nInspire = 0;
                m_GuildInspireList.Clear();

                // 添加新消息提示
                CitySceneTable.eSceneSubType subType;
                if (ClientSystemTown.GetCurrentSceneSubType(out subType) && (subType == CitySceneTable.eSceneSubType.GuildBattle || subType == CitySceneTable.eSceneSubType.CrossGuildBattle))
                {
                    NewMessageNoticeManager.GetInstance().AddNewMessageNoticeWhenNoExist("GuildBattleEnd", null, data =>
                    {
                        ClientSystemManager.GetInstance().OpenFrame<GuildBattleResultFrame>(FrameLayer.Middle, msgData.endInfo);
                        NewMessageNoticeManager.GetInstance().RemoveNewMessageNotice(data);
                    });
                }
            }
            else
            {
                _ClearGuildBattleRecord();
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildBattleStateChanged, eOldState, m_guildBattleState, msgData.endInfo);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityDungeonUpdate);
        }

        void _OnNetSyncGuildAttackCityRes(MsgDATA a_msgData)
        {
            WorldGuildChallengeInfoSync res = new WorldGuildChallengeInfoSync();
            res.decode(a_msgData.bytes);

            m_AttackCityData.info = res.info;

            m_AttackCityData.enrollGuildId = res.enrollGuildId;
            m_AttackCityData.enrollGuildName = res.enrollGuildName;
            m_AttackCityData.enrollGuildLevel = res.enrollGuildLevel;
            m_AttackCityData.enrollGuildleaderName = res.enrollGuildleaderName;
            m_AttackCityData.itemId = res.itemId;
            m_AttackCityData.itemNum = res.itemNum;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildAttackCityInfoUpdate);
        }

        void _OnNetSyncBattleRecord(MsgDATA a_msgData)
        {
            if (m_guildBattleState != EGuildBattleState.Firing)
            {
                return;
            }

            WorldGuildBattleRecordSync msgData = new WorldGuildBattleRecordSync();
            msgData.decode(a_msgData.bytes);

            if ((int)msgData.record.index > m_nLatestRecordID)
            {
                m_nLatestRecordID = (int)msgData.record.index;
            }

            m_arrBattleRecords.Add(msgData.record);

            if (msgData.record.loser.id == PlayerBaseData.GetInstance().RoleID ||
                msgData.record.winner.id == PlayerBaseData.GetInstance().RoleID)
            {
                m_arrSelfBattleRecords.Add(msgData.record);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildBattleRecordSync, msgData.record);
        }

        void _OnNetGuildBattleRecordRes(MsgDATA a_msgData)
        {
            WorldGuildBattleRecordRes res = new WorldGuildBattleRecordRes();
            res.decode(a_msgData.bytes);

            if (res.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.result);
                return;
            }

            _InitGuildBattleRecord(res.records);
        }

        void _OnGuildChallengeRes(MsgDATA a_msgData)
        {
            WorldGuildChallengeRes res = new WorldGuildChallengeRes();
            res.decode(a_msgData.bytes);

            if (res.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.result);
                return;
            }

            SystemNotifyManager.SysNotifyFloatingEffect("宣战成功!");
        }

        void _OnGuildBattleInspireRes(MsgDATA a_msgData)
        {
            WorldGuildBattleInspireInfoRes res = new WorldGuildBattleInspireInfoRes();
            res.decode(a_msgData.bytes);

            if (res.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.result);
                return;
            }

            m_GuildInspireList.Clear();

            int iTerriID = res.terrId;

            for(int i = 0; i < res.inspireInfos.Length; i++)
            {
                m_GuildInspireList.Add(res.inspireInfos[i]);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildInspireInfoUpdate);
        }

        void _OnGuildBattleLotteryRes(MsgDATA a_msgData)
        {
            WorldGuildBattleLotteryRes res = new WorldGuildBattleLotteryRes();

            int nPos = 0;
            res.decode(a_msgData.bytes, ref nPos);

            if (res.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.result);
                return;
            }

            if(res.contribution > 0)
            {
                // 构造帮贡的ItemData,帮贡ID写死
                LotteryItem = ItemDataManager.CreateItemDataFromTable(600000009);

                if (LotteryItem == null)
                {
                    SystemNotifyManager.SysNotifyMsgBoxOK("Get Contribution is null in [_OnGuildBattleLotteryRes]");
                    Logger.LogError("Get Contribution is null in [_OnGuildBattleLotteryRes]");
                    return;
                }

                LotteryItem.Count = (int)res.contribution;
            }
            else
            {
                List<Item> items = ItemDecoder.Decode(a_msgData.bytes, ref nPos, a_msgData.bytes.Length);
                if (items != null && items.Count > 0)
                {
                    LotteryItem = ItemDataManager.GetInstance().CreateItemDataFromNet(items[0]);

                    if (LotteryItem == null)
                    {
                        SystemNotifyManager.SysNotifyMsgBoxOK("Get item is null in [_OnGuildBattleLotteryRes]");
                        Logger.LogError("Get item is null in [_OnGuildBattleLotteryRes]");
                        return;
                    }
                }
                else
                {
                    SystemNotifyManager.SysNotifyMsgBoxOK("Get nothing in [_OnGuildBattleLotteryRes]");
                    Logger.LogError("Get nothing in [_OnGuildBattleLotteryRes]");
                    return;
                }
            }

            /*
            if(items != null && items.Count > 0)
            {
                LotteryItem = ItemDataManager.GetInstance().CreateItemDataFromNet(items[0]);

                if (LotteryItem == null)
                {
                    SystemNotifyManager.SysNotifyMsgBoxOK("Get item is null in [_OnGuildBattleLotteryRes]");
                    Logger.LogError("Get item is null in [_OnGuildBattleLotteryRes]");
                    return;
                }
            }
            else if(res.contribution > 0)
            {
                // 构造帮贡的ItemData,帮贡ID写死
                LotteryItem = ItemDataManager.CreateItemDataFromTable(600000009);

                if (LotteryItem == null)
                {
                    SystemNotifyManager.SysNotifyMsgBoxOK("Get Contribution is null in [_OnGuildBattleLotteryRes]");
                    Logger.LogError("Get Contribution is null in [_OnGuildBattleLotteryRes]");
                    return;
                }

                LotteryItem.Count = (int)res.contribution;
            }
            
            else
            {
                SystemNotifyManager.SysNotifyMsgBoxOK("Get nothing in [_OnGuildBattleLotteryRes]");
                Logger.LogError("Get nothing in [_OnGuildBattleLotteryRes]");
                return;
            }
            */

            m_bHasLotteryed = true;

            ShowItemsFrameData NeedData = GenerateJarAnimationNeedData();
            ClientSystemManager.GetInstance().OpenFrame<GuildLotteryFrame>(FrameLayer.Middle, NeedData);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildLotteryResultRes);
        }
        #endregion

        public ShowItemsFrameData GenerateJarAnimationNeedData()
        {
            ShowItemsFrameData NeedData = new ShowItemsFrameData();

            NeedData.data = new JarData();
            NeedData.data.arrRealBonusItems = new List<ItemSimpleData>();
            NeedData.items = new List<JarBonus>();

            NeedData.data.eType = ProtoTable.JarBonus.eType.MagicJar;
            NeedData.data.nID = 801;
            NeedData.data.strName = "公会仓库魔罐";
            NeedData.data.strJarImagePath = "UI/Image/Icon/Icon_Jar/item_jar_01.png:item_jar_01";
            NeedData.data.strJarModelPath = "UIFlatten/Prefabs/Jar/EffUI_xiuzhenguan02";

            // 把公会仓库里的道具作为奖池滚动显示
            if(storageDatas != null && storageDatas.Count > 0)
            {
                for(int i = 0; i < storageDatas.Count; i++)
                {
                    ItemSimpleData simpledata = new ItemSimpleData();

                    simpledata.ItemID = storageDatas[i].TableID;
                    simpledata.Count = storageDatas[i].Count;
                    simpledata.Name = storageDatas[i].Name;

                    NeedData.data.arrRealBonusItems.Add(simpledata);
                }                     
            }

            // 但是公会仓库有可能屁东西都没有,而抽奖动画又一定要有内容可展示,所以这里加进去一些假的滚动展示内容(目前只是加了一些不同数值的帮贡)
            int iShowNum = 3;

            if (storageDatas != null && storageDatas.Count > 2)
            {
                iShowNum = 1;
            }

            for (int i = 0; i < iShowNum; i++)
            {
                ItemData lotterys = ItemDataManager.CreateItemDataFromTable(600000009);
                ItemSimpleData simpledata = new ItemSimpleData();

                simpledata.ItemID = 600000009;
                simpledata.Count = (i + 1) * 100;
                simpledata.Name = lotterys.Name + ((i + 1) * 100).ToString();

                NeedData.data.arrRealBonusItems.Add(simpledata);
            }

            // 然后还要构造最终实际"得到"的数据,第一个是"购买"数据(公会战的抽奖动画是用不上的,但是这个数据要有),第二个才是真正抽到的道具
            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    NeedData.items.Add(new JarBonus());
                }
                else
                {
                    if(LotteryItem == null)
                    {
                        Logger.LogError("LotteryItem is null when play animation");
                    }

                    JarBonus item1 = new JarBonus();
                    item1.item = LotteryItem;

                    NeedData.items.Add(item1);
                }
            }

            return NeedData;
        }

        public void SetDataExchange(ref GuildBattleInspireInfo recive, GuildBattleInspireInfo give)
        {
            recive.playerId = give.playerId;
            recive.playerName = give.playerName;
            recive.inspireNum = give.inspireNum;
        }

        public void UpdateInspireInfo(ref Text mInspireLevel, ref Text mCurAttr, ref GameObject mInspireMax, ref Button mInspire, ref Image mInspireIcon, ref Text mInspireCount,ref ComButtonEnbale mEnableInspire, GuildBattleOpenType guildopentype = GuildBattleOpenType.GBOT_INVALID)
        {
            if(myGuild == null)
            {
                Logger.LogError("myGuild is null");
                return;
            }

            int nCurrentLevel = myGuild.nInspire;
            int nNextLevel = nCurrentLevel + 1;

            Assert.IsTrue(nCurrentLevel >= 0);

            if(mInspireLevel != null)
            {
                mInspireLevel.text = nCurrentLevel.ToString() + "级";
            }

            if(mCurAttr != null)
            {
                mCurAttr.text = _GetInspireAttrDesc(nCurrentLevel);
            }

            GuildInspireTable table = TableManager.GetInstance().GetTableItem<GuildInspireTable>(nNextLevel);
            if (table == null)
            {
                if(mInspireMax != null && mInspire != null)
                {
                    mInspireMax.CustomActive(true);
                    mInspire.gameObject.CustomActive(false);
                }
            }
            else
            {
                if(mInspireMax != null && mInspire != null)
                {
                    mInspireMax.CustomActive(false);
                    mInspire.gameObject.CustomActive(true);
                }

                if (table.ConsumeItem.Count != 1 || table.CrossConsumeItem.Count != 1)
                {
                    Logger.LogError("【公会鼓舞】消耗的道具种类数量错误！！");
                }

                Assert.IsTrue(table.ConsumeItem.Count == 1 || table.CrossConsumeItem.Count == 1);

                string[] values = {"", ""};

                if(guildopentype == GuildBattleOpenType.GBOT_CROSS)
                {
                    values = table.CrossConsumeItem[0].Split('_');
                }
                else if(guildopentype == GuildBattleOpenType.GBOT_NORMAL_CHALLENGE)
                {
                    values = table.ConsumeItem[0].Split('_');
                }
                else
                {
                    if(m_guildBattleType == GuildBattleType.GBT_CROSS)
                    {
                        values = table.CrossConsumeItem[0].Split('_');
                    }
                    else
                    {
                        values = table.ConsumeItem[0].Split('_');
                    }
                }

                int id = 0;

                if(int.TryParse(values[0], out id))
                {
                    ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(id);

                    // mInspireIcon.sprite = AssetLoader.GetInstance().LoadRes(itemData.Icon, typeof(Sprite)).obj as Sprite;
                    if(mInspireIcon != null)
                    {
                        ETCImageLoader.LoadSprite(ref mInspireIcon, itemData.Icon);
                    }

                    if(mInspireCount != null)
                    {
                        mInspireCount.text = values[1];
                    }

                    bool bEnable = true;

                    if (!HasTargetManor())
                    {
                        bEnable = false;
                    }

                    if (GetGuildBattleType() == GuildBattleType.GBT_NORMAL || GetGuildBattleType() == GuildBattleType.GBT_CROSS)
                    {
                        if (GetGuildBattleState() < EGuildBattleState.Signup || GetGuildBattleState() > EGuildBattleState.Preparing)
                        {
                            bEnable = false;
                        }
                    }
                    else
                    {
                        if (GetGuildBattleState() != EGuildBattleState.Preparing)
                        {
                            bEnable = false;
                        }
                    }

                    if (mEnableInspire != null)
                    {
                        if (guildopentype == GuildBattleOpenType.GBOT_CROSS)
                        {
                            mEnableInspire.SetEnable(m_guildBattleType == GuildBattleType.GBT_CROSS && bEnable);
                        }
                        else if (guildopentype == GuildBattleOpenType.GBOT_NORMAL_CHALLENGE)
                        {
                            mEnableInspire.SetEnable((m_guildBattleType == GuildBattleType.GBT_NORMAL || m_guildBattleType == GuildBattleType.GBT_CHALLENGE) && bEnable);
                        }
                        else
                        {
                            mEnableInspire.SetEnable(bEnable);
                        }
                    }                    
                }
            }
        }

        public GuildAttackCityData GetAttackCityData()
        {
            return m_AttackCityData;
        }

        public List<GuildBattleInspireInfo> GetGuildBattleInspireInfoList()
        {
            return m_GuildInspireList;
        }

        public List<FigureStatueInfo> GetTownStatueInfo()
        {
            return TownStatueList;
        }

        public List<FigureStatueInfo> GetGuildGuardStatueInfo()
        {
            return GuildGuardStatueList;
        }
        public bool HasGuildBattleLotteryed()
        {
            return m_bHasLotteryed;
        }

        public bool IsInGuildBattle()
        {
            return (m_guildBattleState >= EGuildBattleState.Preparing && m_guildBattleState <= EGuildBattleState.Firing && m_guildBattleType != GuildBattleType.GBT_CROSS);
        }

        public void SetLotteryState(byte state)
        {
            if((GuildBattleLotteryStatus)state == GuildBattleLotteryStatus.GBLS_CAN)
            {
                m_bHasLotteryed = false;
            }
            else
            {
                m_bHasLotteryed = true;
            }
        }

        public void SendGuildBattleLotteryReq()
        {
            if(m_guildBattleState != EGuildBattleState.LuckyDraw)
            {
                return;
            }

            WorldGuildBattleLotteryReq req = new WorldGuildBattleLotteryReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        void _GetInspireCost(out int a_nTableID, out int a_nCount)
        {
            int nCurrentLevel = GuildDataManager.GetInstance().myGuild.nInspire;
            int nNextLevel = nCurrentLevel + 1;
            Assert.IsTrue(nCurrentLevel >= 0);

            GuildInspireTable table = TableManager.GetInstance().GetTableItem<GuildInspireTable>(nNextLevel);
            if (table != null)
            {
                if(m_guildBattleType == GuildBattleType.GBT_CROSS)
                {
                    Assert.IsTrue(table.CrossConsumeItem.Count == 1);
                    string[] values = table.CrossConsumeItem[0].Split('_');
                    a_nTableID = int.Parse(values[0]);
                    a_nCount = int.Parse(values[1]);
                }
                else
                {
                    Assert.IsTrue(table.ConsumeItem.Count == 1);
                    string[] values = table.ConsumeItem[0].Split('_');
                    a_nTableID = int.Parse(values[0]);
                    a_nCount = int.Parse(values[1]);
                }
            }
            else
            {
                a_nTableID = 0;
                a_nCount = 0;
            }
        }

        string _GetInspireAttrDesc(int a_nLevel)
        {
            if (a_nLevel <= 0)
            {
                return TR.Value("guild_manor_inspire_none");
            }

            GuildInspireTable table = TableManager.GetInstance().GetTableItem<GuildInspireTable>(a_nLevel);
            if (table == null)
            {
                return TR.Value("guild_manor_inspire_max");
            }
            else
            {
                if (GuildDataManager.GetInstance().GetBuildingLevel(GuildBuildingType.MAIN) < table.NeedGuildLevel)
                {
                    return TR.Value("guild_manor_inspire_max");
                }
            }

            string str = string.Empty;
            int nSkillID = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_GUILD_INSPIRE_SKILL_ID).Value;
            List<string> values = SkillDataManager.GetInstance().GetSkillDesList(nSkillID, (byte)a_nLevel);
            for (int i = 0; i < values.Count; ++i)
            {
                if (i == 0)
                {
                    str = values[i];
                }
                else
                {
                    str += "\n";
                    str += values[i];
                }
            }

            return str;
        }

        public void SendInspire()
        {
            if (GetGuildBattleType() == GuildBattleType.GBT_NORMAL || GetGuildBattleType() == GuildBattleType.GBT_CROSS)
            {
                if (GetGuildBattleState() < EGuildBattleState.Signup || GetGuildBattleState() > EGuildBattleState.Preparing)
                {
                    SystemNotifyManager.SystemNotify(2314014);
                    return;
                }
            }
            else
            {
                if (GetGuildBattleState() != EGuildBattleState.Preparing)
                {
                    SystemNotifyManager.SystemNotify(2314039);
                    return;
                }
            }

            CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();
            _GetInspireCost(out costInfo.nMoneyID, out costInfo.nCount);

            CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
            {
                if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
                {
                    return;
                }

                BattleInspire();
            });
        }

        public void SwitchToGuildScene()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<GuildMainFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<GuildMainFrame>();
            }

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            CitySceneTable TownTableData = TableManager.instance.GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (TownTableData == null)
            {
                return;
            }

          if (ClientSystemManager.GetInstance().IsFrameOpen<ClientSystemTownFrame>())
          {
                ClientSystemTownFrame frame = ClientSystemManager.GetInstance().GetFrame(typeof(ClientSystemTownFrame)) as ClientSystemTownFrame;
                frame.SetForbidFadeIn(true);
          }

            GameFrameWork.instance.StartCoroutine(systemTown._NetSyncChangeScene(
                  new SceneParams
                  {
                      currSceneID = systemTown.CurrentSceneID,
                      currDoorID = 0,
                      targetSceneID = nGuildAreanScenceID,
                      targetDoorID = 0,
                  }));
        }
        public static string GetTerrName(int terrID)
        {
            GuildTerritoryTable tableData = TableManager.GetInstance().GetTableItem<GuildTerritoryTable>(terrID);
            if (tableData != null)
            {
                return tableData.Name;
            }
            return "无";
        }
        #region GuildDungeonMethod

        private GuildDungeonLvlTable GetGuildDungeonLvlTable(int nDungeonID)
        {
            if(m_GuildDungeonID2LvTable == null)
            {
                return null;
            }

            if(!m_GuildDungeonID2LvTable.ContainsKey(nDungeonID))
            {
                return null;
            }

            GuildDungeonLvlTable temp = null;
            temp = m_GuildDungeonID2LvTable[nDungeonID];
            if(temp == null)
            {
                return null;
            }

            return temp;
        }

        public string GetGuildDungeonName(uint nDungeonID)
        {
            GuildDungeonLvlTable table = GetGuildDungeonLvlTable((int)nDungeonID);
            if(table != null)
            {
                return table.dungeonName;
            }

            return "";
        }

        public static int GetDungeonTypeByDungeonID(int iDungeonID)
        {
            GuildDungeonLvlTable table = GuildDataManager.GetInstance().GetGuildDungeonLvlTable(iDungeonID);
            if (table != null)
            {
                return table.dungeonType;
            }

            return 0;
        }

        // 根据征战祭祀等级获取公会副本等级
        private static int GetDungeonTypeByFeteLv(int iFeteLv)
        {
            int nType = 0;

            Dictionary<int, object> dicts = TableManager.instance.GetTable<GuildDungeonTypeTable>();
            if (dicts != null)
            {
                var iter = dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    GuildDungeonTypeTable adt = iter.Current.Value as GuildDungeonTypeTable;
                    if (adt == null)
                    {
                        continue;
                    }

                    if (adt.buildLvl >= iFeteLv)
                    {
                        nType = adt.dungeonType;
                        break;
                    } 
                }
            }

            return nType;
        }

        // 根据征战祭祀等级判断是否能进入某个地下城
        // 征战祭祀等级影响公会副本难度等级 add by qxy 2019-03-28
        public static bool CheckGuildDungeonCanEnter(int iFeteLv, int iDungeonID)
        {
            return GetDungeonTypeByFeteLv(iFeteLv) >= GetDungeonTypeByDungeonID(iDungeonID);
        }
        public bool checkedLvUpBulilding = false;

        // 有建筑可以升级
        public bool CanLvUpBulilding()
        {
            if(checkedLvUpBulilding)
            {
                return false;
            }
            if(myGuild == null)
            {
                return false;
            }
            if (myGuild.dictBuildings == null)
            {
                return false;
            }

            // 没有升级权限 呵呵了
            if(!HasPermission(EGuildPermission.UpgradeBuilding))
            {
                return false;
            }

            // 公会资金满足 建筑需要的主城等级满足（主城建筑无此条件）
            var iter = GuildDataManager.GetInstance().myGuild.dictBuildings.GetEnumerator();
            while (iter.MoveNext())
            {
                GuildBuildingData guildBuildingDataTemp = iter.Current.Value as GuildBuildingData;
                if(guildBuildingDataTemp == null)
                {
                    continue;
                }
                GuildBuildInfoTable guildBuildInfoTable = GetGuildBuildInfoTable(iter.Current.Key);
                if(guildBuildInfoTable == null)
                {
                    continue;
                }
                if(!guildBuildInfoTable.isOpen)
                {
                    continue;
                }
                if(guildBuildingDataTemp.nLevel >= guildBuildingDataTemp.nMaxLevel)
                {
                    continue;
                }
                if (GuildDataManager.GetInstance().GetMyGuildFund() < guildBuildingDataTemp.nUpgradeCost)
                {
                    continue;
                }                     
                return true;
            }            
            return false;
        }

        public bool checkedLvUpEmblem = false;
        // 达到激活徽记条件
        public bool CanActiveEmblem()
        {
            if(checkedLvUpEmblem)
            {
                return false;
            }
            // 当前等级为0 且公会等级达到 且玩家等级达到 且建筑等级达到 且材料足够

            List<int> notEnoughItemIDs = null;
            return (GetEmblemLv() == 0 
                && GetGuildLv() >= GetEmblemLvUpGuildLvLimit() 
                && PlayerBaseData.GetInstance().Level >= GetEmblemLvUpPlayerLvLimit()
                && GetBuildingLevel(GuildBuildingType.HONOUR) >= GetEmblemLvUpHonourLvLimit() 
                && GuildDataManager.GetInstance().IsCostEnoughToLvUpEmblem(ref notEnoughItemIDs));
        }
        // 达到升级徽记条件
        public bool CanLvUpEmblem()
        {
            if(checkedLvUpEmblem)
            {
                return false;
            }
            // 当前等级为1到最高级中间 建筑等级达到 材料足够
            List<int> notEnoughItemIDs = null;            
            return (GetEmblemLv() >= 1 && GetEmblemLv() < GetMaxEmblemLv() 
                && GuildDataManager.GetInstance().GetBuildingLevel(GuildBuildingType.HONOUR) >= GuildDataManager.GetInstance().GetEmblemNeedHonourLv(GetEmblemLv() + 1)
                && GuildDataManager.GetInstance().IsCostEnoughToLvUpEmblem(ref notEnoughItemIDs));
        }
        public static int currentMaxBuildLv = 2; // 现阶段战争祭坛等级上限
        public bool checkedSetBossDiff = false;
        int GetCanSelectBossDiffCount()
        {
            int iCount = 0;
            Dictionary<int, object> dicts = TableManager.instance.GetTable<GuildDungeonTypeTable>();
            if (dicts != null)
            {
                var iter = dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    GuildDungeonTypeTable adt = iter.Current.Value as GuildDungeonTypeTable;
                    if (adt == null)
                    {
                        continue;
                    }
                    GuildDungeonTypeTable dungeonTypeTable = adt;
                    bool bTypeNotOpen = dungeonTypeTable.buildLvl > GuildDataManager.currentMaxBuildLv; // 该难度是否 未开放
                    if (bTypeNotOpen) // 若解锁需要的战争祭坛等级＞现阶段战争祭坛等级上限，解锁提示显示：敬请期待
                    {
                        continue;
                    }
                    bool isTypeUnLocked = GuildDataManager.GetInstance().GetBuildingLevel(GuildBuildingType.FETE) >= dungeonTypeTable.buildLvl;
                    if (isTypeUnLocked == false)
                    {
                        continue;
                    }
                    iCount++;
                }
            }
            return iCount;
        }

        // 可以设置boss难度
        public bool CanSetBossDiff()
        {
            if(checkedSetBossDiff)
            {
                return false;
            }
            return HasPermission(EGuildPermission.SetGuildDungeonBossDiff)
                && GetGuildLv() >= GetGuildDungeonActivityGuildLvLimit()
                && GetCanSelectBossDiffCount() >= 1;
        }

        public int GetGuildDungeonDiffLevel(uint nDungeonID)
        {
            GuildDungeonLvlTable table = GetGuildDungeonLvlTable((int)nDungeonID);
            if (table != null)
            {
                return table.dungeonLvl;
            }

            return 0;
        }

        public ulong GetGuildDungeonBossMaxHp(uint nDungeonID)
        {
            ulong nMaxHp = 0;
            string maxHp = "";
            GuildDungeonLvlTable table = GetGuildDungeonLvlTable((int)nDungeonID);
            if (table != null)
            {
                maxHp = table.bossBlood;
            }

            ulong.TryParse(maxHp, out nMaxHp);

            return nMaxHp;           
        }
        public int GetGuildDungeonLv(uint nDungeonID)
        {
            DungeonTable tableItem = TableManager.GetInstance().GetTableItem<DungeonTable>((int)nDungeonID);
            if (tableItem != null)
            {
                return tableItem.Level;
            }
            return 0;
        }
        public string GetGuildDungeonIconPath(uint nDungeonID)
        {
            GuildDungeonLvlTable table = GetGuildDungeonLvlTable((int)nDungeonID);
            if (table != null)
            {
                return table.iconPath;
            }

            return "";
        }
        public string GetGuildDungeonMiniIconPath(uint nDungeonID)
        {
            GuildDungeonLvlTable table = GetGuildDungeonLvlTable((int)nDungeonID);
            if (table != null)
            {
                return table.miniIconPath;
            }
            return "";
        }
        public string GetGuildDungeonPlayingDesc(uint nDungeonID)
        {
            GuildDungeonLvlTable table = GetGuildDungeonLvlTable((int)nDungeonID);
            if (table != null)
            {
                return table.playingDesc;
            }
            return "";
        }
        public string GetGuildDungeonWeaknessDesc(uint nDungeonID)
        {
            GuildDungeonLvlTable table = GetGuildDungeonLvlTable((int)nDungeonID);
            if (table != null)
            {
                return table.weaknessDesc;
            }
            return "";
        }
        public string GetGuildDungeonRecommendDesc(uint nDungeonID)
        {
            GuildDungeonLvlTable table = GetGuildDungeonLvlTable((int)nDungeonID);
            if (table != null)
            {
                return table.recommendDesc;
            }
            return "";
        }
        private static int _getCellValue(Type type, string filed, BuffTable buf, int lv)
        {
            try
            {
                PropertyInfo obj = type.GetProperty(filed, BindingFlags.Public | BindingFlags.Instance);
                object gv = obj.GetValue(buf, null);
                if (gv is ProtoTable.UnionCell)
                {
                    var cv = gv as ProtoTable.UnionCell;
                    if (null != cv)
                    {
                        return TableManager.GetValueFromUnionCell(cv, lv);
                    }
                }
                else if (gv is int)
                {
                    return (int)gv;
                }
            }
            catch
            {
            }
            return -1;
        }
        private static int _getFinalValue(BuffDrugConfigInfoTable.eValueType type, int val)
        {
            switch (type)
            {
                case BuffDrugConfigInfoTable.eValueType.Rate1000:
                    return val / 10;
            }
            return val;
        }
        private static int GetGuildDungeonBufMaxLv(int iBufID)
        {
            int nMaxLv = 0;
            Dictionary<int, object> dicts = TableManager.instance.GetTable<ItemBuffTable>();
            if (dicts != null)
            {
                var iter = dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    ItemBuffTable adt = iter.Current.Value as ItemBuffTable;
                    if (adt == null)
                    {
                        continue;
                    }
                    if (adt.buffId != iBufID)
                    {
                        continue;
                    }
                    nMaxLv = adt.buffMaxLvl;
                    break;
                }
            }
            return nMaxLv;
        }
        public static string GetBuffAddUpInfo(int iBufID, int iBufLv)
        {
            string txtInfo = "";
            var tableBuf = TableManager.instance.GetTableItem<ProtoTable.BuffTable>(iBufID);
            if (tableBuf == null)
            {
                return "";
            }
            var buffType = (BuffType)tableBuf.Type;
            var dataType = tableBuf.GetType();
            var buffs = TableManager.instance.GetTable<BuffDrugConfigInfoTable>();
            var iter = buffs.GetEnumerator();
            while (iter.MoveNext())
            {
                BuffDrugConfigInfoTable config = iter.Current.Value as BuffDrugConfigInfoTable;
                if (null != config)
                {
                    int v = _getCellValue(dataType, config.Filed, tableBuf, iBufLv);
                    int va = _getFinalValue(config.ValueType, v);
                    if (va > 0)
                    {
                        txtInfo = string.Format(TR.Value(config.Filed), va);
                    }
                }
            }
            if(iBufLv == GetGuildDungeonBufMaxLv(iBufID))
            {
                txtInfo += "(Max)";
            }
            return txtInfo;
        }
        private List<int> GetOpenedGuildDungeonIDList()
        {
            List<int> openIDs = new List<int>();
            List<int> openJuniorIDs = new List<int>();
            List<int> openMediumIDs = new List<int>();
            List<int> openBossIDs = new List<int>();
            List<int> tempIDs = new List<int>();
            if (m_GuildDungeonActivityData != null && m_GuildDungeonActivityData.juniorDungeonDamgeInfos != null)
            {
                for (int i = 0; i < m_GuildDungeonActivityData.juniorDungeonDamgeInfos.Count; i++)
                {
                    openIDs.Add((int)m_GuildDungeonActivityData.juniorDungeonDamgeInfos[i].nDungeonID);
                }
                if (m_GuildDungeonActivityData != null && m_GuildDungeonActivityData.mediumDungeonDamgeInfos != null)
                {                 
                    for (int i = 0; i < m_GuildDungeonActivityData.mediumDungeonDamgeInfos.Count; i++)
                    {
                        openIDs.Add((int)m_GuildDungeonActivityData.mediumDungeonDamgeInfos[i].nDungeonID);
                    }
                    int iNum = 0;
                    for(int i = 0;i < m_GuildDungeonActivityData.mediumDungeonDamgeInfos.Count;i++)
                    {
                        if(m_GuildDungeonActivityData.mediumDungeonDamgeInfos[i].nOddHp == 0)
                        {
                            iNum++;
                        }
                    }
                    if(iNum == m_GuildDungeonActivityData.mediumDungeonDamgeInfos.Count)
                    {
                        if(m_GuildDungeonActivityData != null && m_GuildDungeonActivityData.bossDungeonDamageInfos.Count > 0)
                        {
                            openIDs.Add((int)m_GuildDungeonActivityData.bossDungeonDamageInfos[0].nDungeonID);
                        }
                    }
                }
            }
            return openIDs;
        }
        public bool IsGuildDungeonMap(int nDungeonID)
        {
            DungeonTable table = TableManager.GetInstance().GetTableItem<DungeonTable>(nDungeonID);
            if(table == null)
            {
                return false;
            }
            return table.SubType == DungeonTable.eSubType.S_GUILD_DUNGEON;
        }
        public bool IsGuildDungeonOpen(int nDungeonID)
        {
            List<int> openIDs = GetOpenedGuildDungeonIDList();
            if(openIDs == null)
            {
                return false;
            }
            return openIDs.Contains(nDungeonID);
        }      
        public bool IsShowChestModel()
        {
            if(!CheckActivityLimit())
            {
                return false;
            }
            if(m_GuildDungeonActivityData == null)
            {
                return false;
            }
            if(m_GuildDungeonActivityData.nActivityState == (int)GuildDungeonStatus.GUILD_DUNGEON_END)
            {
                return false;
            }
            if (m_GuildDungeonActivityData.nActivityState == (int)GuildDungeonStatus.GUILD_DUNGEON_PREPARE)
            {
                return false;
            }
            if(m_GuildDungeonActivityData.nBossOddHp != 0)
            {
                return false;
            }
            return true;
        }
        public void TryGetGuildDungeonActivityChestAward()
        {
            GuildDataManager.GuildDungeonActivityData data = GuildDataManager.GetInstance().GetGuildDungeonActivityData();
            if (data != null)
            {
                if (data.bGetReward)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("guildDungeonHaveGetReward"));
                }
                else
                {
                    GuildDataManager.GetInstance().SendWorldGuildDungeonLotteryReq();
                }
            }
            return;
        }
        public static bool CheckActivityLimit()
        {
            if(GuildDataManager.GetInstance().myGuild == null)
            {
                return false;
            }
            if (ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_GUILD_DUNGEON))
            {
                return false;
            }
            int iLimitGuildLv = GetGuildDungeonActivityGuildLvLimit();
            int iLimitPlayerLevel = GetGuildDungeonActivityPlayerLvLimit();           
            if (GuildDataManager.GetInstance().myGuild.nLevel < iLimitGuildLv || PlayerBaseData.GetInstance().Level < iLimitPlayerLevel)
            {
                return false;
            }
            return true;
        }
        public GuildDungeonStatus GetGuildDungeonActivityStatus()
        {
            if(m_GuildDungeonActivityData == null)
            {
                return GuildDungeonStatus.GUILD_DUNGEON_END;
            }
            return (GuildDungeonStatus)m_GuildDungeonActivityData.nActivityState;
        }
        public bool IsGuildDungeonActivityOpen()
        {
            GuildDungeonStatus status = GuildDataManager.GetInstance().GetGuildDungeonActivityStatus();
            if (CheckActivityLimit() && (status == GuildDungeonStatus.GUILD_DUNGEON_PREPARE
                || status == GuildDungeonStatus.GUILD_DUNGEON_START
                || status == GuildDungeonStatus.GUILD_DUNGEON_REWARD))
            {
                return true;
            }
            return false;
        }
        public static int GetGuildDungeonActivityPlayerLvLimit()
        {
            var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_GUILD_DUNGEON_PLAYER_LEVEL_LIMIT);
            if (SystemValueTableData != null)
            {
                return SystemValueTableData.Value;
            }
            return 1;
        }
        public static int GetGuildDungeonActivityGuildLvLimit()
        {
            var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_GUILD_DUNGEON_GUILD_LEVEL_LIMIT);
            if (SystemValueTableData != null)
            {
                return SystemValueTableData.Value;
            }
            return 1;
        }
        private void OnLevelChanged(int iPreLv, int iCurLv)
        {
            int iOpenLv = GetGuildDungeonActivityPlayerLvLimit();
            if (iOpenLv > 0 && iPreLv < iOpenLv && iCurLv >= iOpenLv)
            {                
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityDungeonUpdate);              
            }            
            UpdateGuildEmblemRedPoint();
        }
        private void OnGuildLevelChanged(int iPreLv, int iCurLv)
        {
            int iOpenLv = GetGuildDungeonActivityGuildLvLimit();
            if (iOpenLv > 0 && iPreLv < iOpenLv && iCurLv >= iOpenLv)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityDungeonUpdate);
            }
            UpdateGuildEmblemRedPoint();
            UpdateGuildSetBossDiffRedPoint();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUpdateGuildEmblemLvUpEntry);
            // 公会等级升级其实就是公会主城升级了
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildUpgradeBuildingSuccess);       
        }
        private void OnUpdateActivityNotice(GuildDungeonStatus oldState,GuildDungeonStatus newState)
        {
            NotifyInfo NoticeData = new NotifyInfo
            {
                type = (uint)NotifyType.NT_GUILD_DUNGEON
            };
            if (oldState == GuildDungeonStatus.GUILD_DUNGEON_END && newState != GuildDungeonStatus.GUILD_DUNGEON_END)
            {
                ActivityNoticeDataManager.GetInstance().AddActivityNotice(NoticeData);
                DeadLineReminderDataManager.GetInstance().AddActivityNotice(NoticeData);
            }
            else if(oldState != GuildDungeonStatus.GUILD_DUNGEON_END && newState == GuildDungeonStatus.GUILD_DUNGEON_END)
            {
                ActivityNoticeDataManager.GetInstance().DeleteActivityNotice(NoticeData);
                DeadLineReminderDataManager.GetInstance().DeleteActivityNotice(NoticeData);
            }
        }
        public bool IsShowFireworks
        {
            get;
            set;
        }

        private int ActivityNotifyUIOpenCount
        {
            get;
            set;
        }
        private void OpenNotifyUI()
        {
            ClientSystemManager.GetInstance().OpenFrame<GuildDungeonActivityOpenNotifyFrame>();
        }
        public static bool IsGuildTeamDungeonID(int iTeamDungeonID)
        {
            TeamDungeonTable table = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(iTeamDungeonID);
            return table != null && GetInstance().IsGuildDungeonMap(table.DungeonID);
        }

        public static bool IsGuildDungeonMapScence()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (systemTown != null && systemTown.CurrentSceneID == nGuildDungeonMapScenceID)
            {
                return true;
            }

            return false;
        }

        public static bool IsInGuildAreanScence()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (systemTown != null)
            {
                CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                if (scenedata != null)
                {
                    if (scenedata.SceneSubType == CitySceneTable.eSceneSubType.Guild)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static void FliterTeamDungeonID(ref List<int> Fliter, ref Dictionary<int,List<int>> SecFliterDict)
        {
            List<int> guildTeamDungeonID = new List<int>();
            for (int i = 0; i < Fliter.Count; i++)
            {
                if (SecFliterDict.ContainsKey(Fliter[i]))
                {
                    List<int> ids = SecFliterDict[Fliter[i]];
                    for (int j = 0; j < ids.Count; j++)
                    {
                        TeamDungeonTable teamDungeonTable = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(ids[j]);
                        if (teamDungeonTable == null)
                        {
                            continue;
                        }

                        DungeonTable dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(teamDungeonTable.DungeonID);
                        if (dungeonTable == null)
                        {
                            continue;
                        }

                        if (dungeonTable.SubType == DungeonTable.eSubType.S_GUILD_DUNGEON)
                        {
                            guildTeamDungeonID.Add(Fliter[i]);
                            break;
                        }
                    }
                }
            }

            // 公会场景和公会地图场景只保留公会副本一级菜单
            if (IsInGuildAreanScence() || IsGuildDungeonMapScence())
            {
                Fliter.Clear();
                Fliter.AddRange(guildTeamDungeonID);

                // 把公会副本二级菜单替换为服务器发过来的id列表
                for (int i = 0; i < Fliter.Count; i++)
                {
                    if (SecFliterDict.ContainsKey(Fliter[i]))
                    {
                        SecFliterDict[Fliter[i]] = GuildDataManager.GetInstance().GetGuildDungeonIDList();
                    }
                }
            }
            else  // 非公会场景要剔除掉公会副本一级菜单
            {
                // 剔除掉公会副本一级菜单ID
                for (int i = 0; i < Fliter.Count;)
                {
                    if (guildTeamDungeonID.Contains(Fliter[i]))
                    {
                        Fliter.RemoveAt(i);
                        continue;
                    }

                    i++;
                }

                // 清除二级菜单中的公会副本id数据
                for (int i = 0; i < guildTeamDungeonID.Count; i++)
                {
                    if (SecFliterDict.ContainsKey(guildTeamDungeonID[i]))
                    {
                        SecFliterDict.Remove(guildTeamDungeonID[i]);
                    }
                }
            }

            return;
        }

        public List<int> GetGuildDungeonIDList()
        {
            List<int> ids = new List<int>();
            if(m_GuildDungeonActivityData != null)
            {
                if(m_GuildDungeonActivityData.juniorDungeonDamgeInfos != null)
                {
                    for (int i = 0; i < m_GuildDungeonActivityData.juniorDungeonDamgeInfos.Count; i++)
                    {
                        int id = (int)m_GuildDungeonActivityData.juniorDungeonDamgeInfos[i].nDungeonID;
                        if (guildDungeonID2TeamDungeonID != null && guildDungeonID2TeamDungeonID.ContainsKey(id))
                        {
                            ids.Add(guildDungeonID2TeamDungeonID[id]);
                        }                        
                    }
                }

                if(m_GuildDungeonActivityData.mediumDungeonDamgeInfos != null)
                {
                    for(int i = 0;i < m_GuildDungeonActivityData.mediumDungeonDamgeInfos.Count;i++)
                    {
                        int id = (int)m_GuildDungeonActivityData.mediumDungeonDamgeInfos[i].nDungeonID;
                        if (guildDungeonID2TeamDungeonID != null && guildDungeonID2TeamDungeonID.ContainsKey(id))
                        {
                            ids.Add(guildDungeonID2TeamDungeonID[id]);
                        }                        
                    }
                }

                if(m_GuildDungeonActivityData.bossDungeonDamageInfos != null)
                {
                    for(int i = 0;i < m_GuildDungeonActivityData.bossDungeonDamageInfos.Count;i++)
                    {
                        int id = (int)m_GuildDungeonActivityData.bossDungeonDamageInfos[i].nDungeonID;
                        if(guildDungeonID2TeamDungeonID != null && guildDungeonID2TeamDungeonID.ContainsKey(id))
                        {
                            ids.Add(guildDungeonID2TeamDungeonID[id]);
                        }                        
                    }
                }
                
            }
            return ids;
        }

      
        
                

                // 活动关闭了要关闭定时请求
        void RequestActivityData()
                        {
                            // 只有在公会场景和地图选择场景才请求数据
                            if(IsInGuildAreanScence() || IsGuildDungeonMapScence())
                            {
                                GuildDataManager.GuildDungeonActivityData activityData = GuildDataManager.GetInstance().GetGuildDungeonActivityData();
                                if (activityData != null && activityData.nActivityState != (int)GuildDungeonStatus.GUILD_DUNGEON_END && GuildDataManager.CheckActivityLimit())
                                {
                                    GuildDataManager.GetInstance().SendWorldGuildDungeonInfoReq();
                                    GuildDataManager.GetInstance().SendWorldGuildDungeonCopyReq();
                                }
                            }                           
            return;
        }       
        public bool IsIDOpened(UInt64 id)
        {
            if (string.IsNullOrEmpty(jsonText))
            {
                return false;
            }
            LitJson.JsonData data = LitJson.JsonMapper.ToObject(jsonText);
            if (data == null)
            {
                return false;
            }
            bool value = false;
            string keyName = id.ToString();
            if (data.ContainsKey(keyName) && data[keyName].IsBoolean)
            {
                return (bool)data[keyName];
            }
            return false;     
        }
        public void ClearIDOpened(UInt64 id)
        {
            LitJson.JsonData data = LitJson.JsonMapper.ToObject(jsonText);
            if (data == null)
            {
                return;
            }
            data[id.ToString()] = false;
            try
            {
                jsonText = data.ToJson();
                if (!string.IsNullOrEmpty(jsonText))
                {
                    FileArchiveAccessor.SaveFileInPersistentFileArchive(m_kSavePath, jsonText);
                }
            }
            catch (System.Exception e)
            {
                Logger.LogError(e.ToString());
            }
            return;
        }
        public void SetIDOpened(UInt64 id)
        {
            LitJson.JsonData data = LitJson.JsonMapper.ToObject(jsonText);
            if (data == null)
            {
                return;
            }
            data[id.ToString()] = true;
            try
            {
                jsonText = data.ToJson();
                if (!string.IsNullOrEmpty(jsonText))
                {
                    FileArchiveAccessor.SaveFileInPersistentFileArchive(m_kSavePath, jsonText);
                }
            }
            catch (System.Exception e)
            {
                Logger.LogError(e.ToString());
            }
            return;
        }
        #endregion
        #region GuildRedPacket

        // 获取每日发送红包最大次数
        public int GetDailySendRedPacketMaxCount()
        {
            return Utility.GetSystemValueFromTable(SystemValueTable.eType.SVT_GUILD_BUY_REDPACKET_TIME);          
        }
        // 获取每日发送红包剩余次数
        public int GetDailySendRedPacketLeftCount()
        {
            int nCostTime = CountDataManager.GetInstance().GetCount("guild_pay_rp");
            int iLeftCount = GetDailySendRedPacketMaxCount() - nCostTime;
            if(iLeftCount <= 0)
            {
                return 0;
            }
            return iLeftCount;
        }
        #endregion
        #region GuildDungeonSendMsg
        public void SendWorldGuildDungeonDamageRankReq()
        {
            WorldGuildDungeonDamageRankReq req = new WorldGuildDungeonDamageRankReq();
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }
        public void SendWorldGuildDungeonInfoReq()
        {
            WorldGuildDungeonInfoReq req = new WorldGuildDungeonInfoReq();
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }
        public void SendWorldGuildDungeonCopyReq()
        {
            WorldGuildDungeonCopyReq req = new WorldGuildDungeonCopyReq();
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }
        public void SendWorldGuildDungeonLotteryReq()
        {
            WorldGuildDungeonLotteryReq req = new WorldGuildDungeonLotteryReq();
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }
        public void SendWorldGuildDungeonStatueReq()
        {
            WorldGuildDungeonStatueReq req = new WorldGuildDungeonStatueReq();
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }
        public void SendWorldGuildSetJoinLevelReq(uint joinLv)
        {
            WorldGuildSetJoinLevelReq req = new WorldGuildSetJoinLevelReq();
            NetManager netMgr = NetManager.Instance();
            req.joinLevel = joinLv;
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }
        public void SendWorldGuildSetDungeonTypeReq(uint iDiffType)
        {
            WorldGuildSetDungeonTypeReq req = new WorldGuildSetDungeonTypeReq();
            NetManager netMgr = NetManager.Instance();
            req.dungeonType = iDiffType;
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }
        #endregion
        #region GuildAuctionSendMsg  
        public void SendWorldGuildAuctionItemReq(GuildAuctionType guildAuctionType)
        {
            WorldGuildAuctionItemReq req = new WorldGuildAuctionItemReq();
            NetManager netMgr = NetManager.Instance();
            req.type = (uint)guildAuctionType;
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }
        public void SendWorldGuildAuctionFixReq(ulong itemGUID)
        {
            WorldGuildAuctionFixReq req = new WorldGuildAuctionFixReq();
            NetManager netMgr = NetManager.Instance();
            req.guid = itemGUID;
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }
        public void SendWorldGuildAuctionBidReq(ulong itemGUID,ulong price)
        {
            WorldGuildAuctionBidReq req = new WorldGuildAuctionBidReq();
            NetManager netMgr = NetManager.Instance();
            req.guid = itemGUID;
            req.bidPrice = (uint)price;
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }
        public void SendWorldGuildGetTerrDayRewardReq()
        {
            WorldGuildGetTerrDayRewardReq req = new WorldGuildGetTerrDayRewardReq();
            NetManager netMgr = NetManager.Instance();      
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }
        #endregion
        #region GuildEmblemMsg
        public void SendWorldGuildEmblemUpReq()
        {
            WorldGuildEmblemUpReq req = new WorldGuildEmblemUpReq();
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }
        #endregion
        #region GuildAuctionMsgProc
        private void _OnWorldGuildAuctionItemRes(MsgDATA msg)
        {
            WorldGuildAuctionItemRes msgData = new WorldGuildAuctionItemRes();
            msgData.decode(msg.bytes);
            bool bAuctionOpen = false;
            Action<List<GuildAuctionItemData>,GuildAuctionItem[]> SetItemDatas = (auctionItemDatas, auctionItemList) => 
            {
                if (auctionItemDatas == null || auctionItemList == null)
                {
                    return;                    
                }
                auctionItemDatas.Clear();
                for (int i = 0; i < auctionItemList.Length; i++)
                {
                    GuildAuctionItemData data = new GuildAuctionItemData();
                    GuildAuctionItem guildAuctionItem = auctionItemList[i];
                    if (data == null || guildAuctionItem == null)
                    {
                        continue;
                    }
                    data.guid = guildAuctionItem.guid;
                    if (guildAuctionItem.itemList != null && guildAuctionItem.itemList.Length >= 1)
                    {
                        data.itemData0.ID = (int)(guildAuctionItem.itemList[0].id);
                        data.itemData0.Num = (int)(guildAuctionItem.itemList[0].num);                      
                    }
                    if (guildAuctionItem.itemList != null && guildAuctionItem.itemList.Length >= 2)
                    {
                        data.itemData1.ID = (int)(guildAuctionItem.itemList[1].id);
                        data.itemData1.Num = (int)(guildAuctionItem.itemList[1].num);
                    }
                    data.curbiddingPrice = guildAuctionItem.curPrice;
                    data.nextBiddingPrice = guildAuctionItem.bidPrice;
                    data.buyNowPrice = guildAuctionItem.fixPrice;
                    data.statusEndStamp = guildAuctionItem.endTime;
                    data.bidRoleId = guildAuctionItem.bidRoleId;
                    GuildAuctionItemState state = (GuildAuctionItemState)guildAuctionItem.state;
                    if(state == GuildAuctionItemState.GAI_STATE_PREPARE)
                    {
                        data.auctionItemState = AuctionItemState.Prepare;
                    }
                    else if (state == GuildAuctionItemState.GAI_STATE_NORMAL)
                    {
                        data.auctionItemState = AuctionItemState.InAuction;
                    }
                    else if (state == GuildAuctionItemState.GAI_STATE_DEAL)
                    {
                        data.auctionItemState = AuctionItemState.SoldOut;
                    }
                    else if (state == GuildAuctionItemState.GAI_STATE_ABORATION)
                    {
                        data.auctionItemState = AuctionItemState.AbortiveAuction;
                    }
                    // 只要有一个道具时正在拍卖中，则认为拍卖活动正在进行中
                    if(state == GuildAuctionItemState.GAI_STATE_NORMAL)
                    {
                        bAuctionOpen = true;
                    }
                    auctionItemDatas.Add(data);
                }
                return;
            };
            if(msgData.type == (uint)GuildAuctionType.G_AUCTION_GUILD)
            {
                SetItemDatas(guildAuctionItemDatasForGuildAuction, msgData.auctionItemList);
                guildAuctionItemDatasForGuildAuction.Sort(SortGuildAuctionItemData);
                if(!ClientSystemManager.GetInstance().IsFrameOpen<GuildDungeonAuctionFrame>())
                {
                    // 这里去判断下是否全部流拍或者已售出，是的话打开世界拍卖页签
                    bool bHasValidGuildAuctionItem = false; // 是否还有有效的拍卖品
                    for (int i = 0; i < guildAuctionItemDatasForGuildAuction.Count; i++)
                    {
                        GuildAuctionItemData guildAuctionItemData = guildAuctionItemDatasForGuildAuction[i];
                        if (guildAuctionItemData == null)
                        {
                            continue;
                        }
                        // 某个拍卖品的状态既不是流拍也不是已售出，则认为还有有效的拍卖品
                        if (guildAuctionItemData.auctionItemState != AuctionItemState.AbortiveAuction && guildAuctionItemData.auctionItemState != AuctionItemState.SoldOut)
                        {
                            bHasValidGuildAuctionItem = true;
                            break;
                        }
                    }
                    if (bHasValidGuildAuctionItem)
                    {
                        ClientSystemManager.GetInstance().OpenFrame<GuildDungeonAuctionFrame>(FrameLayer.Middle, GuildDungeonAuctionFrame.FrameType.GuildAuction);
                    }
                    else
                    {
                        ClientSystemManager.GetInstance().OpenFrame<GuildDungeonAuctionFrame>(FrameLayer.Middle, GuildDungeonAuctionFrame.FrameType.WorldAuction);
                    }
                }
                //IsGuildAuctionOpen = bAuctionOpen;
            }
            else if(msgData.type == (uint)GuildAuctionType.G_AUCTION_WORLD)
            {
                SetItemDatas(guildAuctionItemDatasForWorldAuction, msgData.auctionItemList);
                guildAuctionItemDatasForWorldAuction.Sort(SortGuildAuctionItemData);
                //IsGuildWorldAuctionOpen = bAuctionOpen;
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnGuildDungeonAuctionItemsUpdate,(GuildAuctionType)msgData.type);
            return;
        }
        static List<AuctionItemState> states = new List<AuctionItemState>() {AuctionItemState.InAuction,AuctionItemState.Prepare,AuctionItemState.AbortiveAuction,AuctionItemState.SoldOut };
        int SortGuildAuctionItemData(GuildAuctionItemData x, GuildAuctionItemData y)
        {
            if(x == null || y == null)
            {
                return 0;
            }
            if(states == null)
            {
                return 0;
            }
            int iIndex1 = states.FindIndex(state => { return state == x.auctionItemState; });
            int iIndex2 = states.FindIndex(state => { return state == y.auctionItemState; });
            return iIndex1.CompareTo(iIndex2);           
        }
        private void _OnWorldGuildAuctionNotify(MsgDATA msg)
        {
            WorldGuildAuctionNotify msgData = new WorldGuildAuctionNotify();
            msgData.decode(msg.bytes);
            if(msgData.type == (uint)GuildAuctionType.G_AUCTION_GUILD)
            {
                IsGuildAuctionOpen = (msgData.isOpen != 0);

                // 拍卖结束了就直接关闭相关拍卖界面吧
                if (!IsGuildAuctionOpen)
                {
                    ClientSystemManager.GetInstance().CloseFrame<GuildDungeonAuctionFrame>();
                }

                HaveNewGuildAuctonItem = msgData.isOpen != 0; // 每次有新的拍卖品时服务器下发一次此消息，且msgData.isOpen不为0
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnGuildDungeonAuctionAddNewItem);
            }
            else if(msgData.type == (uint)GuildAuctionType.G_AUCTION_WORLD)
            {
                IsGuildWorldAuctionOpen = (msgData.isOpen != 0);

                // 拍卖结束了就直接关闭相关拍卖界面吧
                if (!IsGuildWorldAuctionOpen)
                {
                    ClientSystemManager.GetInstance().CloseFrame<GuildDungeonAuctionFrame>();
                }
                HaveNewWorldAuctonItem = msgData.isOpen != 0; // 每次有新的拍卖品时服务器下发一次此消息，且msgData.isOpen不为0
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnGuildDungeonAuctionAddNewItem);
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnGuildDungeonAuctionStateUpdate);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnGuildDungeonWorldAuctionStateUpdate);
            return;
        }
        private void _OnWorldGuildAuctionFixRes(MsgDATA msg)
        {
            WorldGuildAuctionFixRes msgData = new WorldGuildAuctionFixRes();
            msgData.decode(msg.bytes);
            if (msgData.retCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.retCode);
                return;
            }
            GuildDungeonAuctionFrame.RequestGuildAuctionItem();
            return;
        }
        private void _OnWorldGuildAuctionBidRes(MsgDATA msg)
        {
            WorldGuildAuctionBidRes msgData = new WorldGuildAuctionBidRes();
            msgData.decode(msg.bytes);
            if (msgData.retCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.retCode);
                if (msgData.retCode == (uint)ProtoErrorCode.GUILD_AUCTION_BID_BE_OVER) // 拍卖价格有更新需要重新出价
                {
                    GuildDungeonAuctionFrame.RequestGuildAuctionItem();
                }
                return;
            }

            // 竞价成功后重新请求数据，用于刷新UI
            GuildDungeonAuctionFrame.RequestGuildAuctionItem();
            return;
        }
        #endregion
        #region GuildEmblemUpMsgProc
        private void _OnWorldGuildEmblemUpRes(MsgDATA msg)
        {
            WorldGuildEmblemUpRes msgData = new WorldGuildEmblemUpRes();
            msgData.decode(msg.bytes);
            if (msgData.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
                return;
            }            
            if(myGuild != null)
            {
                myGuild.emblemLevel = msgData.emblemLevel;
            }
            UpdateGuildEmblemRedPoint();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnGuildEmblemLevelUp);
            ClientSystemManager.GetInstance().OpenFrame<GuildEmblemLvUpResultFrame>();
            return;
        }
        #endregion
        #region GuildDungeonMsgProc
        private void _OnWorldGuildDungeonDamageRankRes(MsgDATA msg)
        {
            WorldGuildDungeonDamageRankRes msgData = new WorldGuildDungeonDamageRankRes();
            msgData.decode(msg.bytes);
            
            if(m_DungeonRankInfoList != null)
            {
                m_DungeonRankInfoList.Clear();
                for (int i = 0; i < msgData.damageVec.Length; i++)
                {
                    DungeonDamageRankInfo data = new DungeonDamageRankInfo();
                    data.nRank = msgData.damageVec[i].rank;
                    data.nPlayerID = msgData.damageVec[i].playerId;
                    data.nDamage = msgData.damageVec[i].damageVal;
                    data.strPlayerName = msgData.damageVec[i].playerName;
                    m_DungeonRankInfoList.Add(data);
                    if(m_MyDungeonDamageRankInfo != null && data.nPlayerID == PlayerBaseData.GetInstance().RoleID)
                    {
                        m_MyDungeonDamageRankInfo.nRank = data.nRank;
                        m_MyDungeonDamageRankInfo.nPlayerID = data.nPlayerID;
                        m_MyDungeonDamageRankInfo.nDamage = data.nDamage;
                        m_MyDungeonDamageRankInfo.strPlayerName = data.strPlayerName;
                    }
                }
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildDungeonDamageRank);
            return;
        }
        private void _OnWorldGuildDungeonInfoRes(MsgDATA msg)
        {
            WorldGuildDungeonInfoRes msgData = new WorldGuildDungeonInfoRes();
            msgData.decode(msg.bytes);
            if (msgData.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
                return;
            }
            if (m_GuildDungeonActivityData != null)
            {
                uint nOldState = m_GuildDungeonActivityData.nActivityState;
                uint nNewState = msgData.dungeonStatus;
                ulong iOldOddHp = m_GuildDungeonActivityData.nBossOddHp;
                if(msgData.bossBlood.Length == 4) // 个数为3则表示是三个中级boss的血量信息
                {
                    if (m_GuildDungeonActivityData.mediumDungeonDamgeInfos != null)
                    {
                        m_GuildDungeonActivityData.mediumDungeonDamgeInfos.Clear();
                    }
                    for (int i = 0; i < msgData.bossBlood.Length; i++)
                    {
                        GuildDungeonBossBlood bloodInfo = msgData.bossBlood[i];
                        int dungeonLvl = GetGuildDungeonDiffLevel(bloodInfo.dungeonId);
                        if (dungeonLvl == (int)GuildDungeonLvl.GUILD_DUNGEON_HIGUH)
                        {
                            m_GuildDungeonActivityData.nBossMaxHp = GetGuildDungeonBossMaxHp(bloodInfo.dungeonId);
                            m_GuildDungeonActivityData.nBossOddHp = bloodInfo.oddBlood;
                            m_GuildDungeonActivityData.nVerifyBlood = bloodInfo.verifyBlood;
                        }
                        else if (dungeonLvl == (int)GuildDungeonLvl.GUILD_DUNGEON_MID)
                        {
                            MediumDungeonDamageInfo data = new MediumDungeonDamageInfo();
                            data.nDungeonID = bloodInfo.dungeonId;
                            data.nMaxHp = (ulong)GetGuildDungeonBossMaxHp(bloodInfo.dungeonId);
                            data.nOddHp = bloodInfo.oddBlood;
                            data.nVerifyBlood = bloodInfo.verifyBlood;                         
                            m_GuildDungeonActivityData.mediumDungeonDamgeInfos.Add(data);
                        }
                    }
                }
                else
                {
                    Debug.LogErrorFormat("boss血量信息长度非法，应该为4，当前长度为{0}",msgData.bossBlood.Length);
                }                
               if(msgData.clearGateTime != null && m_GuildDungeonActivityData.guildDungeonClearGateInfos != null)
                {
                    m_GuildDungeonActivityData.guildDungeonClearGateInfos.Clear();
                    for (int i = 0; i < msgData.clearGateTime.Length; i++)
                    {
                        GuildDungeonClearGateInfo info = new GuildDungeonClearGateInfo();
                        info.guildName = msgData.clearGateTime[i].guildName;
                        info.spendTime = msgData.clearGateTime[i].spendTime;
                        m_GuildDungeonActivityData.guildDungeonClearGateInfos.Add(info);
                    }                    
                    m_GuildDungeonActivityData.guildDungeonClearGateInfos.Sort(delegate(GuildDungeonClearGateInfo info1, GuildDungeonClearGateInfo info2)
                    {
                        if(info1.spendTime == info2.spendTime)
                        {
                            return 0;
                        }
                        return info1.spendTime < info2.spendTime ? -1 : 1;
                    });
                }
                m_GuildDungeonActivityData.nActivityState = msgData.dungeonStatus;
                m_GuildDungeonActivityData.nActivityStateEndTime = msgData.statusEndStamp;
                m_GuildDungeonActivityData.bGetReward = msgData.isReward == 1;
                OnUpdateActivityNotice((GuildDungeonStatus)nOldState,(GuildDungeonStatus)nNewState);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityDungeonUpdate);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPointChanged, ERedPoint.GuildMain);
                {
                    if(iOldOddHp != 0 && m_GuildDungeonActivityData.nBossOddHp == 0)
                    {
                        bool bIsInGuildArean = IsInGuildAreanScence();                       
                        IsShowFireworks = true;
                        if(bIsInGuildArean)
                        {                            
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildDungeonShowFireworks);                        
                        }                       
                    }
                }
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildDungeonUpdateActivityData);            
        }
        private void _OnWorldGuildDungeonCopyRes(MsgDATA msg)
        {
            WorldGuildDungeonCopyRes msgData = new WorldGuildDungeonCopyRes();
            msgData.decode(msg.bytes);
            if (m_GuildDungeonActivityData != null)
            {
                if(m_GuildDungeonActivityData.juniorDungeonDamgeInfos != null)
                {
                    m_GuildDungeonActivityData.juniorDungeonDamgeInfos.Clear();
                }
                if(m_GuildDungeonActivityData.mediumDungeonDamgeInfos != null)
                {
                    m_GuildDungeonActivityData.mediumDungeonDamgeInfos.Clear();
                }
                if(m_GuildDungeonActivityData.bossDungeonDamageInfos != null)
                {
                    m_GuildDungeonActivityData.bossDungeonDamageInfos.Clear();
                }
                if(m_GuildDungeonActivityData.buffAddUpInfos != null)
                {
                    m_GuildDungeonActivityData.buffAddUpInfos.Clear();
                }
                for(int i = 0;i < msgData.battleRecord.Length;i++)
                {
                    GuildDungeonBattleRecord record = msgData.battleRecord[i];
                    int dungeonLvl = GetGuildDungeonDiffLevel(record.dungeonId);
                    if (dungeonLvl == (int)GuildDungeonLvl.GUILD_DUNGEON_LOW)
                    {
                        JuniorDungeonDamageInfo data = new JuniorDungeonDamageInfo();
                        data.nDungeonID = record.dungeonId;
                        data.nKillCount = record.battleCnt;                  
                        m_GuildDungeonActivityData.juniorDungeonDamgeInfos.Add(data);
                    }
                    else if(dungeonLvl == (int)GuildDungeonLvl.GUILD_DUNGEON_MID)
                    {
                        MediumDungeonDamageInfo data = new MediumDungeonDamageInfo();
                        data.nDungeonID = record.dungeonId;
                        data.nMaxHp = (ulong)GetGuildDungeonBossMaxHp(record.dungeonId);
                        data.nOddHp = record.oddBlood;
                        m_GuildDungeonActivityData.mediumDungeonDamgeInfos.Add(data);
                    }
                    else if(dungeonLvl == (int)GuildDungeonLvl.GUILD_DUNGEON_HIGUH)
                    {
                        BossDungeonDamageInfo data = new BossDungeonDamageInfo();                     
                        data.nDungeonID = record.dungeonId;
                        m_GuildDungeonActivityData.bossDungeonDamageInfos.Add(data);
                    }
                    for(int j = 0;j < record.buffVec.Length;j++)
                    {
                        GuildDungeonBuff buf = record.buffVec[j];
                        BuffAddUpInfo data = new BuffAddUpInfo();                  
                        data.nDungeonID = record.dungeonId;
                        data.nBuffLv = buf.buffLvl;
                        data.nBuffID = buf.buffId;
                        m_GuildDungeonActivityData.buffAddUpInfos.Add(data);
                    }
                }
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildDungeonUpdateDungeonMapInfo);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildDungeonUpdateDungeonBufInfo);
        }
        private void _OnWorldGuildDungeonLotteryRes(MsgDATA msg)
        {
            WorldGuildDungeonLotteryRes msgData = new WorldGuildDungeonLotteryRes();
            msgData.decode(msg.bytes);
            if (msgData.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
                return;
            }
            List<GuildDungeonActivityChestItem> items = new List<GuildDungeonActivityChestItem>();
            if(items != null)
            {
                for (int i = 0; i < msgData.lotteryItemVec.Length; i++)
                {
                    ItemData itemData = ItemDataManager.CreateItemDataFromTable((int)msgData.lotteryItemVec[i].itemId);
                    if (itemData != null)
                    {
                        itemData.Count = (int)msgData.lotteryItemVec[i].itemNum;
                        GuildDungeonActivityChestItem item = new GuildDungeonActivityChestItem();
                        item.itemData = itemData;
                        item.isHighValue = msgData.lotteryItemVec[i].isHighVal > 0;
                        items.Add(item);
                    }
                }
            }           
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildOpenGuildDungeonTreasureChests,items);
        }
        private void _OnWorldGuildDungeonBossDeadNotify(MsgDATA msg)
        {
            WorldGuildDungeonBossDeadNotify msgData = new WorldGuildDungeonBossDeadNotify();
            msgData.decode(msg.bytes);
            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("guildBossKilledNotify", GetGuildDungeonName(msgData.dungeonId)));
            return;
        }
        private void _OnWorldGuildDungeonStatusSync(MsgDATA msg)
        {
            WorldGuildDungeonStatusSync msgData = new WorldGuildDungeonStatusSync();
            msgData.decode(msg.bytes);
            if(m_GuildDungeonActivityData != null)
            {
                uint nOldState = m_GuildDungeonActivityData.nActivityState;
                uint nNewState = msgData.dungeonStatus;
                m_GuildDungeonActivityData.nActivityState = nNewState;
                OnUpdateActivityNotice((GuildDungeonStatus)nOldState, (GuildDungeonStatus)nNewState);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildDungeonSyncActivityState, nOldState, nNewState);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityDungeonUpdate);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPointChanged, ERedPoint.GuildMain);
                if(nNewState == (int)GuildDungeonStatus.GUILD_DUNGEON_END)
                {
                    InvokeMethod.TaskManager.GetInstance().RmoveInvokeIntervalCall(iIntervalCallID);
                }
                else // 活动开启的时候才启动定时请求活动数据
                {
                    RequestActivityData();
                    InvokeMethod.TaskManager.GetInstance().RmoveInvokeIntervalCall(iIntervalCallID);
                    iIntervalCallID = InvokeMethod.TaskManager.GetInstance().InvokeIntervalCall(this,
                        Time.time,
                        fReqGuildActivityDataInterval,                
                        fReqGuildActivityDataInterval,                
                        99999999.0f,                
                        null,                
                        RequestActivityData,
                        null);
                }

                // 只有地下城开启状态播放动画
                if(nNewState == (int)GuildDungeonStatus.GUILD_DUNGEON_START)
                {
                    if(!IsIDOpened(ClientApplication.playerinfo.accid))
                    {
                        SetIDOpened(ClientApplication.playerinfo.accid);
                        ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
                        if (systemTown != null)
                        {
                            InvokeMethod.Invoke(2.0f, OpenNotifyUI);
                        }
                    }
                }
                else
                {
                    ClearIDOpened(ClientApplication.playerinfo.accid);
                }
            }

            return;
        }

        private void _OnWorldGuildDungeonStatueRes(MsgDATA msg)
        {
            WorldGuildDungeonStatueRes msgData = new WorldGuildDungeonStatueRes();
            msgData.decode(msg.bytes);
            GuildGuardStatueList.Clear();
            for (int i = 0; i < msgData.figureStatue.Length; i++)
            {
                GuildGuardStatueList.Add(msgData.figureStatue[i]);
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildGuardStatueUpdate);
            return;
        }
        private void _OnWorldGuildSetJoinLevelRes(MsgDATA msg)
        {
            WorldGuildSetJoinLevelRes msgData = new WorldGuildSetJoinLevelRes();
            msgData.decode(msg.bytes);
            if (msgData.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
                return;
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildJoinLvUpdate);
            return;
        }
        private void _OnWorldGuildSetDungeonTypeRes(MsgDATA msg)
        {
            WorldGuildSetDungeonTypeRes msgData = new WorldGuildSetDungeonTypeRes();
            msgData.decode(msg.bytes);
            if (msgData.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
                return;
            }
            if(myGuild != null)
            {
                myGuild.dungeonType = msgData.dungeonType;
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildDungeonSetBossDiff);
            return;
        }
        #endregion

        private void _OnWorldGuildGetTerrDayRewardRes(MsgDATA msg)
        {
            WorldGuildGetTerrDayRewardRes msgData = new WorldGuildGetTerrDayRewardRes();
            msgData.decode(msg.bytes);
            if (msgData.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.result);
                return;
            }
            return;
        }
        #region 公会兼并


        /// <summary>
        /// 自己的公会成员数量是否满足兼并条件
        /// </summary>
        /// <returns></returns>
        public bool GuildMemberIsEnoughMegrge()
        {
            if (myGuild != null)
            {
                return myGuild.nMemberCount <= myGuild.nMemberMaxCount * 1 / 2;
            }
            return false;
        }
        /// <summary>
        /// 是否超过开服14天
        /// </summary>
        /// <returns></returns>
        public bool IsCanGuildMerger()
        {
            bool result = false;
            ulong time = TimeManager.GetInstance().GetServerTime() - mServerStartGameTime;
            if (time >= 14 * 24 * 60 * 60)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 请求可以兼并的公会列表
        /// </summary>
        /// <param name="a_nStartIndex"></param>
        /// <param name="a_nCount"></param>
        public void RequestCanMergerdGuildList(int a_nStartIndex, int a_nCount)
        {
            WorldGuildWatchCanMergerReq msg = new WorldGuildWatchCanMergerReq();
            msg.start = (ushort)a_nStartIndex;
            msg.num = (ushort)a_nCount;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

        }
        /// <summary>
        /// 请求可以兼并的公会列表的返回
        /// </summary>
        /// <param name="msgDATA"></param>
        private void _OnCanMergerGuildListRes(MsgDATA msgDATA)
        {
            WorldGuildWatchCanMergerRet res = new WorldGuildWatchCanMergerRet();
            int pos = 0;
            res.decode(msgDATA.bytes, ref pos);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildListUpdated, res);
        }
        /// <summary>
        /// 申请可以兼并的公会成员
        /// </summary>
        public void RequestCanMergerdGuildMembers(ulong guildID)
        {
            WorldGuildMemberListReq msg = new WorldGuildMemberListReq();
            msg.guildID = guildID;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

        }
        /// <summary>
        /// 申请可以兼并的公会成员的返回
        /// </summary>
        private void _OnCanMergerdGuildMembersRes(MsgDATA msgDATA)
        {
            WorldGuildMemberListRes res = new WorldGuildMemberListRes();
            int pos = 0;
            res.decode(msgDATA.bytes, ref pos);
            CanMergerdGuildMembers.Clear();

            for (int i = 0; i < res.members.Length; ++i)
            {
                GuildMemberEntry source = res.members[i];
                GuildMemberData target = new GuildMemberData();
                target.uGUID = source.id;
                target.strName = source.name;
                target.nLevel = source.level;
                target.nJobID = source.occu;
                target.vipLevel = source.vipLevel;
                target.seasonLevel = source.seasonLevel;
                target.eGuildDuty = GetClientDuty(source.post);
                target.nContribution = (int)source.contribution;
                target.uOffLineTime = source.logoutTime;
                target.uActiveDegree = source.activeDegree;
                target.playerLabelInfo = source.playerLabelInfo;
                RelationData relationData = null;
                RelationDataManager.GetInstance().FindPlayerIsRelation(source.id, ref relationData);
                if (relationData != null)
                {
                    if (relationData.remark != null && relationData.remark != "")
                    {
                        target.remark = relationData.remark;
                    }
                }

                CanMergerdGuildMembers.Add(target);
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.CanMergerdGuildMemberUpdate);
        }

        /// <summary>
        /// 公会兼并操作
        /// </summary>
        /// <param name="guildId">公会id</param>
        /// <param name="type">操作类型 0申请兼并 1取消申请兼并</param>
        public void RequestGuildMergeOp(ulong guildId, byte type)
        {
            WorldGuildMergerRequestOperatorReq msg = new WorldGuildMergerRequestOperatorReq();
            msg.guildId = guildId;
            msg.opType = type;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

        }
        /// <summary>
        /// 公会兼并操作的返回
        /// </summary>
        /// <param name="obj"></param>
        private void _OnGuildMergerRequestOperatorRes(MsgDATA msgDATA)
        {
            WorldGuildMergerRequestOperatorRet res = new WorldGuildMergerRequestOperatorRet();
            int pos = 0;
            res.decode(msgDATA.bytes, ref pos);
            if (res.errorCode == (uint)ProtoErrorCode.SUCCESS)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RequestGuildMergerSucess, res.opType);
            }
            else if (res.errorCode == (uint)ProtoErrorCode.GUILD_MERGER_REQUEST_EXISTS)
            {
                SystemNotifyManager.SystemNotify((int)res.errorCode);
            }else if(res.errorCode == (uint)ProtoErrorCode.GUILD_MERGER_TIME_SHORT)
            {
                var NotifyData = TableManager.GetInstance().GetTableItem<CommonTipsDesc>((int)res.errorCode);
                var table = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_GUILD_MERGER_SPACE_DAY);
                string des = "";
                if (NotifyData!=null&&table!=null)
                {
                    des = string.Format(NotifyData.Descs, table.Value);
                }
                SystemNotifyManager.SysNotifyFloatingEffect(des);
            }
        }


        /// <summary>
        /// 查询是否有兼并自己公会的请求
        /// </summary>
        public void RequestGuildReceiveMergeRequest()
        {
            WorldGuildReceiveMergerRequestReq msg = new WorldGuildReceiveMergerRequestReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

        }
        /// <summary>
        /// 查询是否有兼并自己公会的请求的返回
        /// </summary>
        /// <param name="obj"></param>
        private void _OnGuildReceiveMergerRequestRes(MsgDATA msgDATA)
        {
            WorldGuildReceiveMergerRequestRet res = new WorldGuildReceiveMergerRequestRet();
            int pos = 0;
            res.decode(msgDATA.bytes, ref pos);
            if (res.isHave == 0)
            {
                IsHaveMergedRequest = false;
            }
            else if (res.isHave == 1)
            {
                IsHaveMergedRequest = true;
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildReceiveMergerd);

        }


        /// <summary>
        /// 查看本公会收到的兼并申请列表
        /// </summary>
        public void RequestGuildHaveMgergeRequest()
        {
            WorldGuildWatchHavedMergerRequestReq msg = new WorldGuildWatchHavedMergerRequestReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

        }
        /// <summary>
        /// 查看本公会收到的兼并申请列表的返回
        /// </summary>
        /// <param name="obj"></param>
        private void _OnGuildWatchHavedMergerRequestRes(MsgDATA msgDATA)
        {
            WorldGuildWatchHavedMergerRequestRet res = new WorldGuildWatchHavedMergerRequestRet();
            int pos = 0;
            res.decode(msgDATA.bytes, ref pos);
            if(res.guilds!=null&&res.guilds.Length>0)
            {
                for (int i = 0; i < res.guilds.Length; i++)
                {
                    if(res.guilds[i].isRequested!=0)
                    {
                        IsAgreeMergerRequest = true;
                    }
                }
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildReceiveMergedList, res.guilds);
        }


        /// <summary>
        /// 拒绝公会兼并申请，取消已同意的申请
        /// </summary>
        /// <param name="opType">0同意 1拒绝 2取消 3清空请求</param>
        public void AcceptOrRefuseOrCancelMergeRequest(byte opType, UInt64 guildId)
        {
            WorldGuildAcceptOrRefuseOrCancleMergerRequestReq msg = new WorldGuildAcceptOrRefuseOrCancleMergerRequestReq();
            msg.opType = opType;
            msg.guildId = guildId;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);
        }
        /// <summary>
        /// 拒绝公会兼并申请，取消已同意的申请的返回
        /// </summary>
        private void _OnGuildAcceptOrRefuseOrCancleMergerRequestRes(MsgDATA msgDATA)
        {
            WorldGuildAcceptOrRefuseOrCancleMergerRequestRet res = new WorldGuildAcceptOrRefuseOrCancleMergerRequestRet();
            int pos = 0;
            res.decode(msgDATA.bytes, ref pos);
            if (res.errorCode == (uint)ProtoErrorCode.SUCCESS)
            {
                RequestGuildHaveMgergeRequest();
                if (res.opType == 0)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.AgreeMerger);
                }
                else if (res.opType == 3)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RefuseAllReceive);
                }
            }
            else if (res.errorCode == (uint)ProtoErrorCode.GUILD_MERGER_REQUEST_ONE|| res.errorCode == (uint)ProtoErrorCode.GUILD_MERGER_INVAILD)
            {
                SystemNotifyManager.SystemNotify((int)res.errorCode);
            }
        }

        /// <summary>
        /// 同步公会兼并相关信息 
        /// 公会繁荣状态(1解散 2低繁荣 3中繁荣 4高繁荣)
        /// 请求状态(0无请求 1已申请 2已接受)
        /// </summary>
        /// <param name="obj"></param>
        private void _OnGuildSyncMergerInfo(MsgDATA msgDATA)
        {
            WorldGuildSyncMergerInfo res = new WorldGuildSyncMergerInfo();
            int pos = 0;
            res.decode(msgDATA.bytes, ref pos);

            myGuild.prosperity = res.prosperityStatus;
            myGuild.mergerRequestType = res.mergerRequsetStatus;
           
        }
        /// <summary>
        ///  同步客户端开服时间
        /// </summary>
        /// <param name="msgDATA"></param>
        private void _OnSyncWordStartTime(MsgDATA msgDATA)
        {
            WorldNotifyGameStartTime res = new WorldNotifyGameStartTime();
            int pos = 0;
            res.decode(msgDATA.bytes, ref pos);
            mServerStartGameTime = res.time;
        }

        /// <summary>
        /// 发送同步公会日志的请求
        /// </summary>
        public void RequsetGuildEvent(uint upTime)
        {
            WorldGuildEventListReq msg = new WorldGuildEventListReq();
            msg.uptime = upTime;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);
        }

        /// <summary>
        /// 同步公会日志
        /// </summary>
        /// <param name="obj"></param>
        private void _OnGuildEventListRes(MsgDATA ms)
        {
            WorldGuildEventListRes res = new WorldGuildEventListRes();
            int pos = 0;
            res.decode(ms.bytes, ref pos);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnGuildEventListRes, res);
        }

        #endregion
    }
}
