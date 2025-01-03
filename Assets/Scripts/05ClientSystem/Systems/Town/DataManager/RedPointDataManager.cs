using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using ActivityLimitTime;
using Protocol;
using Network;
using EJarType = ProtoTable.JarBonus.eType;
using ProtoTable;

namespace GameClient
{
    public enum ERedPoint
    {
        Invalid = -1,
        GuildRequester,
        GuildShop,

/*========================================================================================
        // 强烈注意，每个系统功能的枚举值都加了自己的数值区间！！！！！！
        // 新系统的枚举都加在最后 by wangbo 2019.10.17
  ========================================================================================*/

        ServerRedPointCount = 32,  // <<<<----------------------------------------------

        GuildExchange,
        Jar,
        MagicJar,
        GoldJar,
        MagicJar_Lv55,  //55级罐子红点
        PackageMain,    // 主界面背包红点

        // guild
        GuildMain = 100,      // 主界面公会红点 <<<<----------------------------------------------

        GuildBase,      // 公会基础
        GuildBuilding,  // 公会建筑
        GuildMember,    // 公会成员列表
        GuildManor,     // 公会领地
        GuildCrossManor, // 跨服公会战领地
        GuildRedPacket, //公会红包
        GuildBuildingManager,  // 公会建筑管理
        GuildEmblem,    // 公会徽记        
        GuildSetBossDiff,       // 设置boss难度

        GuildMainCity,
        GuildWelfare,
        GuildStatue,
        GuildSkill,
        GuildTable,

        GuildBattleSignUp,    // 公会战报名
        GuildBattleEnter,    // 公会战进入场景
        
        GuildMerger,//公会合并
        GuildTerrDayReward, // 公会领地每日奖励

        // end guild

        Skill = 200, // 技能界面  <<<<----------------------------------------------
        DailyProve, // 每日试炼
        TeamNewRequester, // 申请组队

        EquipForge,     // 装备打造入口红点
        Institute,

        /// <summary>
        /// 关卡宝箱
        /// </summary>
        ChapterReward,

        /// <summary>
        /// 活动副本
        /// </summary>
        ActivityDungeon = 400,  // <<<<----------------------------------------------
        ActivityLimitTime,//限时活动

        /// <summary>
        /// SDK 手机绑定icon
        /// </summary>
        SDKBindPhone = 500, // <<<<----------------------------------------------

        /// <summary>
        /// 装备回收的红点
        /// </summary>
        EquipRecovery,

        /// <summary>
        /// 佣兵团
        /// </summary>
        AdventureTeam,
    

        Count,
    }

    class RedPointDataManager : DataManager<RedPointDataManager>
    {
        public delegate bool CheckRedPoint();
        bool[] m_arrRedPoints = new bool[(int)ERedPoint.Count];
        CheckRedPoint[] m_arrDelCheckRedPoints = new CheckRedPoint[(int)ERedPoint.Count];       // 本地维护的红点，检测函数
        bool m_bRedPointsInited = false;

        List<ERedPoint> m_arrEventBuffer = new List<ERedPoint>();

        public sealed override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.RedPointDataManager;
        }

        public sealed override void Initialize()
        {
            if (m_bRedPointsInited == false)
            {
                m_arrDelCheckRedPoints[(int)ERedPoint.GuildRequester] = null;
                m_arrDelCheckRedPoints[(int)ERedPoint.GuildShop] = null;
                m_arrDelCheckRedPoints[(int)ERedPoint.GuildExchange] = GuildDataManager.GetInstance().CheckExchangeRedPoint;
                m_arrDelCheckRedPoints[(int)ERedPoint.MagicJar] = () => { return JarDataManager.GetInstance().CheckRedPoint(EJarType.MagicJar); };
                m_arrDelCheckRedPoints[(int)ERedPoint.GoldJar] = () => { return JarDataManager.GetInstance().CheckRedPoint(EJarType.GoldJar); };
                m_arrDelCheckRedPoints[(int)ERedPoint.MagicJar_Lv55] = () => { return JarDataManager.GetInstance().CheckRedPoint(EJarType.MagicJar_Lv55) && Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.MagicJarLv55); };
                m_arrDelCheckRedPoints[(int)ERedPoint.Jar] = () =>
                {
                    return HasRedPoint(ERedPoint.MagicJar) ||
                         HasRedPoint(ERedPoint.GoldJar) || 
                         HasRedPoint(ERedPoint.MagicJar_Lv55);
                };
                m_arrDelCheckRedPoints[(int)ERedPoint.GuildBattleEnter] = GuildDataManager.GetInstance().CheckGuildBattleEnterRedPoint;
                m_arrDelCheckRedPoints[(int)ERedPoint.GuildBattleSignUp] = GuildDataManager.GetInstance().CheckGuildBattleSignUpRedPoint;

                m_arrDelCheckRedPoints[(int)ERedPoint.GuildMain] = () =>
                {
                    if (GuildDataManager.GetInstance().HasSelfGuild())
                    {
                        bool bAcitivityOpen = false;
                        GuildDataManager.GuildDungeonActivityData data = GuildDataManager.GetInstance().GetGuildDungeonActivityData();
                        if(data != null)
                        {
                            bAcitivityOpen = GuildDataManager.CheckActivityLimit() && (data.nActivityState != (int)GuildDungeonStatus.GUILD_DUNGEON_END);
                        }
                        return HasRedPoint(ERedPoint.GuildBase) ||
                            HasRedPoint(ERedPoint.GuildBuilding) ||
                            HasRedPoint(ERedPoint.GuildMember) ||
                            HasRedPoint(ERedPoint.GuildMerger)||
                            HasRedPoint(ERedPoint.GuildManor) ||
                            //HasRedPoint(ERedPoint.GuildShop) || 
                            HasRedPoint(ERedPoint.GuildCrossManor) ||
                            //HasRedPoint(ERedPoint.GuildRedPacket) || 
                            HasRedPoint(ERedPoint.GuildTerrDayReward) ||
                            bAcitivityOpen;
                    }
                    return false;
                };

                m_arrDelCheckRedPoints[(int)ERedPoint.GuildBase] = () =>
                {
                    return false;
                };

                m_arrDelCheckRedPoints[(int)ERedPoint.GuildBuilding] = () =>
                {
                    return HasRedPoint(ERedPoint.GuildMainCity)
                    || HasRedPoint(ERedPoint.GuildBuildingManager)                       
                    || HasRedPoint(ERedPoint.GuildEmblem)
                    || HasRedPoint(ERedPoint.GuildSetBossDiff);             
                };

                m_arrDelCheckRedPoints[(int)ERedPoint.GuildBuildingManager] = () =>
                {
                    return GuildDataManager.GetInstance().CanLvUpBulilding();                   
                };
                m_arrDelCheckRedPoints[(int)ERedPoint.GuildEmblem] = () =>
                {
                    return GuildDataManager.GetInstance().CanActiveEmblem() || GuildDataManager.GetInstance().CanLvUpEmblem();                  
                };
                m_arrDelCheckRedPoints[(int)ERedPoint.GuildSetBossDiff] = () =>
                {
                    return GuildDataManager.GetInstance().CanSetBossDiff();
                };

                m_arrDelCheckRedPoints[(int)ERedPoint.GuildMember] = () =>
                {
                    return HasRedPoint(ERedPoint.GuildRequester) && GuildDataManager.GetInstance().HasPermission(EGuildPermission.ProcessRequester);
                };

                m_arrDelCheckRedPoints[(int)ERedPoint.GuildManor] = () =>
                {
                    return GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_NORMAL && 
                          (HasRedPoint(ERedPoint.GuildBattleSignUp) || HasRedPoint(ERedPoint.GuildBattleEnter));
                };

                m_arrDelCheckRedPoints[(int)ERedPoint.GuildCrossManor] = () =>
                {
                    return GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS && 
                           (HasRedPoint(ERedPoint.GuildBattleSignUp) || HasRedPoint(ERedPoint.GuildBattleEnter));
                };

                m_arrDelCheckRedPoints[(int)ERedPoint.GuildMainCity] = () => { return false; };
                m_arrDelCheckRedPoints[(int)ERedPoint.GuildWelfare] = () => { return HasRedPoint(ERedPoint.GuildExchange); };
                m_arrDelCheckRedPoints[(int)ERedPoint.GuildStatue] = () =>
                {
                    return GuildDataManager.GetInstance().GetRemainWorshipTimes() > 0;
                };
                m_arrDelCheckRedPoints[(int)ERedPoint.GuildSkill] = () => { return false; };
                m_arrDelCheckRedPoints[(int)ERedPoint.GuildTable] = () =>
                {
                    if (GuildDataManager.GetInstance().HasJoinedTable() == false &&
                    GuildDataManager.GetInstance().GetTableType() == ProtoTable.GuildRoundtableTable.eType.First)
                    {
                        return true;
                    }
                    return false;
                };
//                 m_arrDelCheckRedPoints[(int)ERedPoint.GuildRedPacket] = () => {
//                     return RedPackDataManager.GetInstance().HasRedPacketToOpen(RedPacketType.GUILD);
//                 };

                m_arrDelCheckRedPoints[(int)ERedPoint.Institute] = () => {
                    return MissionManager.GetInstance().HasRedPoint();
                };

                m_arrDelCheckRedPoints[(int)ERedPoint.Skill] = SkillDataManager.GetInstance().HasSkillLvCanUp;
                m_arrDelCheckRedPoints[(int)ERedPoint.DailyProve] = Utility.IsShowDailyProveRedPoint;
                m_arrDelCheckRedPoints[(int)ERedPoint.TeamNewRequester] = TeamDataManager.GetInstance().HasNewRequester;

                m_arrDelCheckRedPoints[(int)ERedPoint.ChapterReward] = ChapterUtility.IsChapterCanGetChapterRewards;

                m_arrDelCheckRedPoints[(int)ERedPoint.PackageMain] = () =>
                {
                    return ItemDataManager.GetInstance().IsPackageHasNew(EPackageType.Equip) ||
                    ItemDataManager.GetInstance().IsPackageHasNew(EPackageType.Fashion)
                        || ItemDataManager.GetInstance().IsPackageHasNew(EPackageType.Consumable)|| 
                        ItemDataManager.GetInstance().IsPackageHasNew(EPackageType.Title) ||
                        ItemDataManager.GetInstance().IsPackageHasNew(EPackageType.Inscription) ||
                        ItemDataManager.GetInstance().IsPackageHasNew(EPackageType.Bxy) ||
                        ItemDataManager.GetInstance().IsPackageHasNew(EPackageType.Sinan) ||
                        PetDataManager.GetInstance().IsNeedShowOnUsePetsRedPoint()
                        || PetDataManager.GetInstance().IsNeedShowPetEggRedPoint()
                        || PetDataManager.GetInstance().IsNeedShowPetRedPoint()
                        || HonorSystemUtility.IsShowHonorSystemRedPoint();
                };

                m_arrDelCheckRedPoints[(int)ERedPoint.EquipForge] = EquipForgeDataManager.GetInstance().CheckRedPoint;

                m_arrDelCheckRedPoints[(int)ERedPoint.ActivityDungeon] = ()=>{
                    return ActivityDungeonDataManager.GetInstance().IsShowRedByActivityType(ProtoTable.ActivityDungeonTable.eActivityType.TimeLimit)
                        || ActivityDungeonDataManager.GetInstance().IsShowRedByActivityType(ProtoTable.ActivityDungeonTable.eActivityType.Daily)
                        || MissionDailyFrame.CheckRedPoint();
                };

                //added by mjx on 170814  for sdk bind phone icon btn in main town frame
                m_arrDelCheckRedPoints[(int)ERedPoint.SDKBindPhone] = () =>
                {
                    return MobileBind.MobileBindManager.GetInstance().HasBindMobileAwardToReceive();
                };

                m_arrDelCheckRedPoints[(int) ERedPoint.ActivityLimitTime] = ActivityManager.GetInstance().IsHaveAnyRedPoint;

                m_arrDelCheckRedPoints[(int)ERedPoint.EquipRecovery] = () =>
                {
                    return EquipRecoveryDataManager.GetInstance().HaveRedPoint();
                };

                m_arrDelCheckRedPoints[(int)ERedPoint.AdventureTeam] = () =>
                {
                    return AdventureTeamDataManager.GetInstance().HasRedPointShow();
                };
                m_arrDelCheckRedPoints[(int)ERedPoint.GuildMerger] = null;
                m_bRedPointsInited = true;
            }
        }

        public sealed override void Clear()
        {
            if (m_bRedPointsInited)
            {
                m_arrRedPoints = new bool[(int)ERedPoint.Count];
                m_arrDelCheckRedPoints = new CheckRedPoint[(int)ERedPoint.Count];
                m_bRedPointsInited = false;
            }

            m_arrEventBuffer.Clear();
        }

        public void Update(float a_fTime)
        {
            for (int i = 0; i < m_arrEventBuffer.Count; ++i)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RedPointChanged, m_arrEventBuffer[i]);
            }
            m_arrEventBuffer.Clear();
        }

        public void UpdateRedPoints(uint a_nServerData)
        {
            m_arrRedPoints[(int)ERedPoint.GuildRequester] = (a_nServerData & (1 << (int)RedPointFlag.GUILD_REQUESTER)) != 0;
            m_arrRedPoints[(int)ERedPoint.GuildShop] = (a_nServerData & (1 << (int)RedPointFlag.GUILD_SHOP)) != 0;
            m_arrRedPoints[(int)ERedPoint.GuildMerger] = (a_nServerData & (1 << (int)RedPointFlag.GUILD_MERGER)) != 0;
            m_arrRedPoints[(int)ERedPoint.GuildTerrDayReward] = (a_nServerData & (1 << (int)RedPointFlag.GUILD_BATTLE_TERR_DAY_REWARD)) != 0;

            NotifyRedPointChanged();
        }

        public bool HasRedPoint(ERedPoint a_eType)
        {
            if (a_eType > ERedPoint.Invalid && a_eType < ERedPoint.Count)
            {
                if (m_arrDelCheckRedPoints[(int)a_eType] != null)
                {
                    m_arrRedPoints[(int)a_eType] = m_arrDelCheckRedPoints[(int)a_eType].Invoke();
                }
                return m_arrRedPoints[(int)a_eType];
            }
            return false;
        }

        public void ClearRedPoint(ERedPoint a_eType)
        {
            if (a_eType > ERedPoint.Invalid && a_eType < ERedPoint.Count)
            {
                if (a_eType < ERedPoint.ServerRedPointCount)
                {
                    SceneClearRedPoint msg = new SceneClearRedPoint();
                    msg.flag = (uint)a_eType;
                    NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);
                }

                if (m_arrRedPoints[(int)a_eType])
                {
                    m_arrRedPoints[(int)a_eType] = false;
                    NotifyRedPointChanged();
                }
            }
        }

        public void NotifyRedPointChanged(ERedPoint redPointType = ERedPoint.Invalid)
        {
            if (m_arrEventBuffer.Contains(redPointType) == false)
            {
                m_arrEventBuffer.Add(redPointType);
            }
        }
    }
}
