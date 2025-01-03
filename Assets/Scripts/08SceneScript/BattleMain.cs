using UnityEngine;
using System.Collections.Generic;
using Protocol;
using Network;
using System;
using System.Collections;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
//using BehaviorDesigner.Runtime;
using GameClient;

public enum BattleState
{
    Null = 0,
    Start,
    WaitResult,
    End
}

public enum BattleType
{
    None  = -1,
    /// <summary>
    /// 自由练习场
    /// </summary>
    Single = 0,
    /// <summary>
    /// PK模式
    /// </summary>
    MutiPlayer,
    /// <summary>
    /// 普通关卡模式
    /// </summary>
    Dungeon,		
    /// <summary>
    /// 死亡之塔
    /// </summary>
    DeadTown,
    /// <summary>
    /// 牛头怪乐园
    /// </summary>
    Mou,
    /// <summary>
    /// 南部溪谷
    /// </summary>
    North,
    /// <summary>
    /// 新手引导
    /// </summary>
    NewbieGuide,
    /// <summary>
    /// pk打木桩, 自由练习
    /// </summary>
	Training,

    /// <summary>
    /// 连招练习
    /// </summary>
	TrainingSkillCombo,
    /// <summary>
    /// 深渊模式
    /// </summary>
	Hell,
    /// <summary>
    /// 远古模式
    /// </summary>
    YuanGu,
    /// <summary>
    /// 金币关卡
    /// </summary>
    GoldRush,
    /// <summary>
    /// 公会领地战
    /// </summary>
    GuildPVP,
    /// <summary>
    /// 赏金联赛PVP
    /// </summary>
    MoneyRewardsPVP,
    /// <summary>
    /// PVP 3v3
    /// </summary>
    PVP3V3Battle,
    /// <summary>
    /// 擂台赛关卡
    /// </summary>
    ChampionMatch,

    /// <summary>
    /// 混战模式
    /// </summary>
    ScufflePVP,
    /// <summary>
    /// 公会副本
    /// </summary>
    GuildPVE,
    /// <summary>
    /// PVE修炼场
    /// </summary>
    TrainingPVE,
    /// <summary>
    /// 吃鸡PVP
    /// </summary>
    ChijiPVP,
    /// <summary>
    /// 终极试炼
    /// </summary>
    FinalTestBattle,
    /// <summary>
    /// 团队副本
    /// </summary>
    RaidPVE,
    /// <summary>
    /// 春节活动藏宝图副本
    /// </summary>
    TreasureMap,
    /// <summary>
    /// 爬塔战场PVE
    /// </summary>
    BattlegroundPVE,
    /// <summary>
    /// 爬塔战场PVP
    /// </summary>
    BattlegroundPVP,
    Demo,
    /// <summary>
    /// 按键设置战斗
    /// </summary>
    InputSetting,

    /// <summary>
    /// 转职试炼
    /// </summary>
    ChangeOccu,
}

public enum BattleFlag
{
	None = 0,
	//NPC_PROTECT = 1<<0,
}

/// <summary>
/// 使用这个来创建
/// </summary>
public class BattleFactory
{
    private static void _prepareTrainigData()
    {
        RacePlayerInfo info = new RacePlayerInfo
        {
            accid = ClientApplication.playerinfo.accid,
            roleId = PlayerBaseData.GetInstance().RoleID,
            level = PlayerBaseData.GetInstance().Level,
            name = PlayerBaseData.GetInstance().Name,
            seat = 0,
            occupation = (byte)PlayerBaseData.GetInstance().JobTableID
        };

        {
            var list = new List<RaceSkillInfo>();
            var kv = TableManager.instance.GetSkillInfoByPid(PlayerBaseData.GetInstance().JobTableID);
            var iter = kv.GetEnumerator();
            while (iter.MoveNext())
            {
                list.Add(new RaceSkillInfo()
                        {
                        id = (ushort)iter.Current.Key,
                        level = (byte)iter.Current.Value,
                        slot = 0,
                        });
            }
            info.skills = list.ToArray();
        }

        BattleDataManager.GetInstance().PlayerInfo = new RacePlayerInfo[1] { info };

        _prepareSingleArea();
    }

	private static void PrepareTrainingData2()
	{
        #region 玩家
        RacePlayerInfo info = new RacePlayerInfo
        {
            accid = ClientApplication.playerinfo.accid,
            roleId = PlayerBaseData.GetInstance().RoleID,
            level = PlayerBaseData.GetInstance().Level,
            name = PlayerBaseData.GetInstance().Name,
            seat = 0,
            occupation = (byte)PlayerBaseData.GetInstance().JobTableID
        };

        var list = new List<RaceSkillInfo>();
		var skillBar = SkillDataManager.GetInstance().GetPvpSkillBar();
        for (int i=0; i<skillBar.Count; ++i)
		{
			var equipedSkill = skillBar[i];
			Skill skill = SkillDataManager.GetInstance().GetSkillInfoById(equipedSkill.id, true);

			list.Add(new RaceSkillInfo()
				{
					id = (ushort)equipedSkill.id,
					level = (byte)skill.level,
					slot = equipedSkill.slot,
				});
		}
		info.skills = list.ToArray();

        /*
		var skillList = SkillDataManager.GetInstance().skillList;
		for(int i=0; i<skillList.Count; ++i)
		{
			var skill = skillList[i];
			if (skillList.Find(s =>{ return s.id == skill.id;}) == null)
			{
				list.Add(new RaceSkillInfo()
					{
						id = (ushort)skill.id,
						level = (byte)skill.level,
						slot = 0,
					});
			}
			info.skills = list.ToArray();
		}*/
        #endregion

        #region 木桩
        RacePlayerInfo info2 = new RacePlayerInfo();
        ProtoTable.AIConfigTable aiConfigData = TableManager.instance.GetTableItem<ProtoTable.AIConfigTable>(Global.Settings.trainingAIConfigId);
        if (Global.Settings.isTrainingAIOpen && aiConfigData != null)
        {
            info2.occupation = (byte)aiConfigData.JobID;
            info2.robotAIType = (byte)aiConfigData.AIType;
            info2.name = "决斗场机器人";

            //加载技能
            ProtoTable.RobotConfigTable robotData = TableManager.instance.GetTableItem<ProtoTable.RobotConfigTable>(Global.Settings.trainingRobotId);
            if (robotData != null && robotData.Skills != null)
            {
                list.Clear();
                for(int i=0;i< robotData.SkillsLength; i++)
                {
                    RaceSkillInfo skillInfo = new RaceSkillInfo();
                    skillInfo.id = (ushort)robotData.Skills[i];
                    skillInfo.level = 1;
                    skillInfo.slot = (byte)(i + 3);
                    list.Add(skillInfo);
                }
                info2.skills = list.ToArray();

                //练习场机器人添加装备
                if (aiConfigData != null)
                {
                    var list2 = new List<RaceEquip>();
                    for (int i = 0; i < aiConfigData.EquipsLength; i++)
                    {
                        RaceEquip raceEquip = new RaceEquip();
                        raceEquip.id = (uint)aiConfigData.Equips[i];
                        list2.Add(raceEquip);
                    }
                    info2.equips = list2.ToArray();
                }
            }
        }
        else
        {
            info2.occupation = 10;
            info2.robotAIType = 0;
            info2.name = "陪练魂剑士";
            int[] equipsData = new int[] { 135470002, 135471002, 135472002, 135473002, 135474002, 135490001, 135491002, 135492003 };
            List<RaceEquip> equips = BeUtility.GetEquips(equipsData);
            info2.equips = equips.ToArray();
        }
        info2.level = 50;
        info2.seat = 1;

        #endregion

        BattleDataManager.GetInstance().PlayerInfo = new RacePlayerInfo[2] { info , info2};
	}

    private static void _prepareSingleArea()
    {
        BattleDataManager.GetInstance().BattleInfo.startAreaId = 1;

        var area = new Battle.DungeonArea
        {
            id = 1
        };
        BattleDataManager.GetInstance().BattleInfo.areas = new List<Battle.DungeonArea>
        {
            area
        };
    }

    //加载PVE修炼场的玩家数据
    private static void PrepareTrainingPVE()
    {
        RacePlayerInfo info = PrepareMainPlayerData(false);
        BattleDataManager.GetInstance().PlayerInfo = new RacePlayerInfo[1] { info };
    }

    //加载主玩家数据
    private static RacePlayerInfo PrepareMainPlayerData(bool isPvp = true)
    {
        RacePlayerInfo info = new RacePlayerInfo
        {
            accid = ClientApplication.playerinfo.accid,
            roleId = PlayerBaseData.GetInstance().RoleID,
            level = PlayerBaseData.GetInstance().Level,
            name = PlayerBaseData.GetInstance().Name,
            seat = 0,
            occupation = (byte)PlayerBaseData.GetInstance().JobTableID
        };

        var list = new List<RaceSkillInfo>();
        List<SkillBarGrid> skillBar = new List<SkillBarGrid>();
        if (isPvp)
            skillBar = SkillDataManager.GetInstance().GetPvpSkillBar();
        else
            skillBar = SkillDataManager.GetInstance().GetPveSkillBar();
        for (int i = 0; i < skillBar.Count; ++i)
        {
            var equipedSkill = skillBar[i];
            Skill skill = SkillDataManager.GetInstance().GetSkillInfoById(equipedSkill.id, isPvp);

            if(skill != null)
            {
                list.Add(new RaceSkillInfo()
                {
                    id = (ushort)equipedSkill.id,
                    level = (byte)skill.level,
                    slot = equipedSkill.slot,
                });
            }
            else
            {
                Logger.LogErrorFormat("技能配置方案有技能id={0},但是已学习的技能列表里没有,这种情况不应该出现,大概率是服务器下发数据有问题", equipedSkill.id);
            }
        }
        info.skills = list.ToArray();
        return info;
    }

    private static void PrepareDemoBattle()
    {
        RacePlayerInfo info = PreparePlayerDataForDemoBattle(false);
        BattleDataManager.GetInstance().PlayerInfo = new RacePlayerInfo[1] { info };
    }
    private static RacePlayerInfo PreparePlayerDataForDemoBattle(bool isPvp = true, byte occuId = 0)
    {
        RacePlayerInfo info = new RacePlayerInfo
        {
            accid = ClientApplication.playerinfo.accid,
            roleId = PlayerBaseData.GetInstance().RoleID,
            level = 60,
            name = "",
            seat = 0,
            occupation = (byte)PlayerBaseData.GetInstance().JobTableID
        };

        if(occuId > 0)
        {
            info.occupation = occuId;
        }

        var list = new List<RaceSkillInfo>();
        var kv = TableManager.instance.GetSkillInfoByPid(PlayerBaseData.GetInstance().JobTableID);
        var iter = kv.GetEnumerator();
        while (iter.MoveNext())
        {
            var cur = iter.Current;

            var skillTable = TableManager.instance.GetTableItem<ProtoTable.SkillTable>(cur.Key);
            
            if(skillTable == null)
                continue;
            
            var category = skillTable.SkillCategory;
            if (category == 2 || category == 3 || category == 4)
            {
                list.Add(new RaceSkillInfo()
                {
                    id = (ushort)iter.Current.Key,
                    level = (byte)iter.Current.Value,
                    slot = 0,
                });    
            }
        }
        info.skills = list.ToArray();
        return info;
    }

    private static void PrepareInputSetting()
    {
        RacePlayerInfo info = PreparePlayerDataForInputSetting(false);
        BattleDataManager.GetInstance().PlayerInfo = new RacePlayerInfo[1] { info };
    }
    
    private static RacePlayerInfo PreparePlayerDataForInputSetting(bool isPvp = true)
    {
        RacePlayerInfo info = new RacePlayerInfo
        {
            accid = ClientApplication.playerinfo.accid,
            roleId = PlayerBaseData.GetInstance().RoleID,
            level = PlayerBaseData.GetInstance().Level,
            name = PlayerBaseData.GetInstance().Name,
            seat = 0,
            occupation = (byte)PlayerBaseData.GetInstance().JobTableID,
            potionPos = PlayerBaseData.GetInstance().potionSets.ToArray()
        };

        info.raceItems = new RaceItem[info.potionPos.Length];
        for(int i = 0; i < info.raceItems.Length; i++)
        {
            info.raceItems[i] = new RaceItem();
            var potionId = info.potionPos[i];
            info.raceItems[i].id = potionId;
            info.raceItems[i].num = (UInt16)ItemDataManager.GetInstance().GetItemCount((int)potionId);
        }

        var list = new List<RaceSkillInfo>();
        List<SkillBarGrid> skillBar = new List<SkillBarGrid>();
        if (isPvp)
            skillBar = SkillDataManager.GetInstance().GetPvpSkillBar();
        else
            skillBar = SkillDataManager.GetInstance().GetPveSkillBar();
        for (int i = 0; i < skillBar.Count; ++i)
        {
            var equipedSkill = skillBar[i];
            Skill skill = SkillDataManager.GetInstance().GetSkillInfoById(equipedSkill.id, isPvp);

            if(skill != null)
            {
                list.Add(new RaceSkillInfo()
                {
                    id = (ushort)equipedSkill.id,
                    level = (byte)skill.level,
                    slot = equipedSkill.slot,
                });
            }
            else
            {
                Logger.LogErrorFormat("技能配置方案有技能id={0},但是已学习的技能列表里没有,这种情况不应该出现,大概率是服务器下发数据有问题", equipedSkill.id);
            }
        }
        info.skills = list.ToArray();
        return info;
    }

    public static void PrepareChangeOccuBattle(byte occuId)
    {
        RacePlayerInfo info = PreparePlayerDataForDemoBattle(false, occuId);
        BattleDataManager.GetInstance().PlayerInfo = new RacePlayerInfo[1] { info };
    }

    public static BaseBattle CreateBattle(BattleType type, eDungeonMode mode = eDungeonMode.LocalFrame, int id = 0)
    {
        switch(type)
        {
            case BattleType.Hell:
            case BattleType.YuanGu:
            case BattleType.Dungeon:
                return new PVEBattle(type, mode, id);
            case BattleType.FinalTestBattle:
                return new FinalTestBattle(type, mode, id);
            case BattleType.ChampionMatch:
                return new ChampionMatchBattle(type, mode, id);
            case BattleType.MutiPlayer:
                _prepareSingleArea();
                return new PVPBattle(type, eDungeonMode.SyncFrame, id);
            case BattleType.ChijiPVP:
                _prepareSingleArea();
                return new ChiJiPVPBattle(type, eDungeonMode.SyncFrame, id);
            case BattleType.Single:
                _prepareTrainigData();
                return new TrainingBatte(type, mode, 0);
            case BattleType.DeadTown:
                return new DeadTowerBattle(type, mode, id);
            case BattleType.GoldRush:
                return new GoldRushBattle(type, mode, id);
            case BattleType.Mou:
                return new MouBattle(type, mode, id);
            case BattleType.North:
                return new NorthBattle(type, mode, id);
            case BattleType.NewbieGuide:
                //_prepareTrainigData();
                return new NewbieGuideBattle(type, eDungeonMode.Test, 1);
			case BattleType.Training:
				_prepareSingleArea();
				PrepareTrainingData2();
				return new TrainingBatte2(type, eDungeonMode.LocalFrame, 10);
            case BattleType.TrainingSkillCombo:
                return new TrainingSkillComboBattle(type, mode, id);
            case BattleType.GuildPVP:
                _prepareSingleArea();
                return new GuildPVPBattle(type, eDungeonMode.SyncFrame, id);
            case BattleType.MoneyRewardsPVP:
                _prepareSingleArea();
                return new MoneyRewardsPVPBattle(type, eDungeonMode.SyncFrame, id);
            case BattleType.PVP3V3Battle:
                _prepareSingleArea();
                return new PVP3V3Battle(type, eDungeonMode.SyncFrame, id);
            case BattleType.ScufflePVP:
                _prepareSingleArea();
                return new PVPScuffleBattle(type, eDungeonMode.SyncFrame, id);
            case BattleType.GuildPVE:
                return new GuildPVEBattle(type, mode, id);
            case BattleType.RaidPVE:
                return new RaidBattle(type, mode, id);
            case BattleType.TrainingPVE:
                _prepareSingleArea();
                PrepareTrainingPVE();
                return new TrainingPVEBattle(type, mode, 60);
            case BattleType.TreasureMap:
                return new TreasureMapBattle(type, mode, id);
            case BattleType.Demo:
                _prepareSingleArea();
                PrepareDemoBattle();
                return new DemoBattle(type, mode, 60);
            case BattleType.InputSetting:
                _prepareSingleArea();
                PrepareInputSetting();
                return new InputSettingBattle(type, eDungeonMode.LocalFrame, 10);
            case BattleType.ChangeOccu:
                _prepareSingleArea();
                return new ChangeOccuBattle(type, eDungeonMode.LocalFrame, id);
            default:
                Logger.LogErrorFormat("没有处理 eDungeonMode {0} 的战斗", type);
                return null;
        }
    }
}

[LoggerModel("Chapter")]
public class BattleMain
{
    //!USE FOR GAME CLIENT,NOT IN LOGICSERVER
    protected IDungeonPlayerDataManager mDungeonPlayers;
    protected IDungeonManager           mDungeonManager;
    protected IDungeonStatistics        mDungeonStatistics;
    protected IDungeonPreloadAssets     mDungeonPreloadAsset;
    protected IBattle                   mBattle;
    protected IDungeonAudio             mDungeonAudio;

    protected FrameRandomImp            mFrameRandom;

	protected uint 						mBattleFlag = 0;


    public FrameRandomImp              FrameRandom{
        get {
            return mFrameRandom;
        }
    }

	public IBattle GetBattle()
	{
		return mBattle;
	}

    public IDungeonStatistics GetDungeonStatistics()
    {
        return mDungeonStatistics;
    }

    public IDungeonManager GetDungeonManager()
    {
        return mDungeonManager;
    }

    public IDungeonPlayerDataManager GetPlayerManager()
    {
        return mDungeonPlayers;
    }

    public static BattleMain instance
    {
        get
        {
            if (null != toDestroyBattle)
            {
                //Logger.LogErrorFormat("[战斗] 战斗中数据设置前搞事情");
            }

            return battleMain;
        }
    }

    protected BeScene            mMain;
    /// <summary>
    /// 这个地方先保持这个名字，防止有问题
    /// </summary>
    public BeScene Main
    {
        get
        {
            // TODO 这里不能这样搞，要改
            if (null != mDungeonManager)
            {
                return mDungeonManager.GetBeScene();
            }

            return mMain;
        }
    }

    protected BattleState        mBattleState;
    public BattleState battleState
    {
        get { return mBattleState;  }
        set { mBattleState = value;  }
    }

    protected static BattleType         mBattleType;
    public static BattleType battleType
    {
        get { return mBattleType; }
        set { mBattleType = value; }
    }

    public static bool IsLastNewbieGuideBattle()
    {
        return mBattleType == BattleType.NewbieGuide;
    }

	public static bool CheckLastBattleMode(BattleType type)
	{
		return mBattleType == type;
	}


    public static eDungeonMode mode
    {
        get
        {
            if (instance == null)
            {
                return eDungeonMode.None;
            }

            if (null != instance.mBattle)
            {
                return instance.mBattle.GetMode();
            }

            return eDungeonMode.None;
        }
    }

    /// <summary>
    /// 上一个待清除的
    /// </summary>
    protected static BattleMain toDestroyBattle;

    /// <summary>
    /// 当前的
    /// </summary>
    protected static BattleMain battleMain;

    private BattleMain(BattleType type)
    {
        mBattleType = type;
    }

    public static BattleMain OpenBattle(BattleType type, eDungeonMode mode, int id,string sessionid)
    {
        Logger.LogProcessFormat("[battle] 开启战斗 {0}, 模式 {1}, 地下城ID {2}", type, mode, id);

        if (null != battleMain)
        {
            toDestroyBattle = battleMain;
            var preBattle = toDestroyBattle.GetBattle() as BaseBattle;
            if(preBattle != null && preBattle.recordServer != null)
                preBattle.recordServer.EndRecord("openNewBattle");
        }
        battleMain = new BattleMain(type);
        //BeScene.ClearSID();
        GeActorEx.ClearStatic();
        var battle = BattleFactory.CreateBattle(type, mode, id);
        battleMain._initBattle(battle);

#if !LOGIC_SERVER
		if ( !ReplayServer.GetInstance().IsReplay() 
#if LOCAL_RECORD_FILE_REPLAY
        || mode != eDungeonMode.RecordFrame
#endif
        )
        {
        	battleMain._setStat(id);
        }
#endif

        return battleMain;
    }

    private void _setStat(int id)
    {
        var buffList = mDungeonPlayers.GetMainPlayer().playerInfo.buffs;

        List<int> buff = new List<int>();

        for (int i = 0; i < buffList.Length; ++i)
        {
            buff.Add((int)buffList[i].id);
        }

        DungeonID did = new DungeonID(id);


#if !LOGIC_SERVER
        GameStatisticManager.instance.DoStatLevelChoose(StatLevelChooseType.ENTER_LEVEL,
                ChapterSelectFrame.sSceneID,
                did.dungeonID,
                did.diffID,
                buff);
#endif

    }

    private void _initBattle(BaseBattle battle)
    {
        mBattle              = battle;
        mDungeonPlayers      = battle.dungeonPlayerManager;
        mDungeonManager      = battle.dungeonManager;
        mDungeonStatistics   = battle.dungeonStatistics;
        mDungeonPreloadAsset = battle.dungeonManager.GetDungeonDataManager();
        mDungeonAudio        = battle;
        mFrameRandom         = battle.FrameRandom;
        
        Logger.LogProcessFormat("[battle] 初始化完成");
    }

    private void _uninitBattle()
    {
        mBattle              = null;
        mDungeonPlayers      = null;
        mDungeonManager      = null;
        mDungeonStatistics   = null;
        mDungeonPreloadAsset = null;
        mDungeonAudio        = null;

        mMain                = null;

        Logger.LogProcessFormat("[battle] 反初始化完成");
    }

	public static bool IsNeedRecordPVP(BattleType type)
	{
        return type == BattleType.MutiPlayer || type == BattleType.GuildPVP || type == BattleType.MoneyRewardsPVP || type == BattleType.ChijiPVP || type == BattleType.BattlegroundPVP;
	}

	public static bool IsModePvP(BattleType type)
	{
        return type == BattleType.MutiPlayer || type == BattleType.Training || type == BattleType.GuildPVP || type == BattleType.MoneyRewardsPVP || type == BattleType.PVP3V3Battle || type ==BattleType.ScufflePVP || type == BattleType.ChijiPVP || type == BattleType.BattlegroundPVP;
    }

    public static bool IsPKTowerPve(BattleType type)
    {
        return type == BattleType.BattlegroundPVE;
    }

    public static bool IsModeChiji(BattleType type)
    {
        return type == BattleType.ChijiPVP;
    }

    public static bool IsChiji()
    {
        return BattleDataManager.GetInstance().PkRaceType == RaceType.ChiJi;
    }
    /// <summary>
    /// 是否是公平竞技场
    /// </summary>
    /// <returns></returns>
    public static bool IsFairDuel()
    {
        return BattleDataManager.GetInstance().PkRaceType == RaceType.PK_EQUAL_1V1;
    }

    //是否是PVE修炼场模式
    public static bool IsModePveTraining(BattleType type)
    {
        return type == BattleType.TrainingPVE;
    }

	public static bool IsModeMultiplayer(eDungeonMode mode)
	{
        return mode == eDungeonMode.SyncFrame;
	}

	public static bool IsModeTrain(BattleType type)
	{
        return type == BattleType.Training || type == BattleType.TrainingPVE;
	}

    public static bool IsPKTower(BattleType type)
    {
        return type == BattleType.BattlegroundPVE || type == BattleType.BattlegroundPVP;
    }

    public static bool IsTeamMode(BattleType type,eDungeonMode mode)
    {
        return !IsModePvP(type) && IsModeMultiplayer(mode);
    }

    public static bool IsModePVP3V3(BattleType type)
    {
        return type == BattleType.PVP3V3Battle;
    }

    public static bool IsCanAccompany(BattleType type)
    {

        return type != BattleType.MutiPlayer &&
            type != BattleType.GuildPVP &&
            type != BattleType.Training &&
            type != BattleType.PVP3V3Battle &&
            type != BattleType.MoneyRewardsPVP &&
            type != BattleType.ScufflePVP &&
            type != BattleType.ChijiPVP && type == BattleType.BattlegroundPVP;

    }

    public static bool IsProtectFloat(BattleType type)
    {
        return type == BattleType.MutiPlayer ||
            type == BattleType.GuildPVP ||
            type == BattleType.Training ||
            type == BattleType.PVP3V3Battle ||
            type == BattleType.MoneyRewardsPVP ||
            type == BattleType.ScufflePVP ||
            type == BattleType.ChijiPVP ||
            type == BattleType.BattlegroundPVP;
    }

    public static bool IsProtectGround(BattleType type)
    {
        return type == BattleType.MutiPlayer ||
            type == BattleType.GuildPVP ||
            type == BattleType.Training ||
            type == BattleType.PVP3V3Battle ||
            type == BattleType.MoneyRewardsPVP ||
            type == BattleType.ScufflePVP ||
            type == BattleType.ChijiPVP ||
            type == BattleType.BattlegroundPVP;
    }

    public static bool IsProtectStand(BattleType type)
    {
        return type == BattleType.MutiPlayer ||
            type == BattleType.GuildPVP ||
            type == BattleType.Training ||
            type == BattleType.PVP3V3Battle ||
            type == BattleType.MoneyRewardsPVP ||
            type == BattleType.ScufflePVP ||
            type == BattleType.ChijiPVP ||
            type == BattleType.BattlegroundPVP;
    }

    public static bool IsShowPing(BattleType type)
    {
        return type == BattleType.MutiPlayer ||
            type == BattleType.GuildPVP ||
            type == BattleType.Training ||
            type == BattleType.PVP3V3Battle ||
            type == BattleType.MoneyRewardsPVP ||
            type == BattleType.ScufflePVP;
    }

    public static void CloseBattle(bool needEndRecord = true)
    {
        if (toDestroyBattle != null)
        {
            toDestroyBattle._unload(needEndRecord);
            toDestroyBattle = null;
        }
        else if (battleMain != null)
        {
            battleMain._unload();
            battleMain = null;
        }
    }

    public BattlePlayer GetLocalPlayer(UInt64 id = 0)
    {
        if (mDungeonPlayers != null)
        {
            return mDungeonPlayers.GetMainPlayer();
        }

        return null;
    }

    /// <summary>
    /// 在PlayerManager里面具体实现
    /// 这里只提供一个快捷访问的接口
    /// </summary>
    /// <returns></returns>
    public BattlePlayer GetLocalTargetPlayer()
    {
        if (mDungeonPlayers != null)
        {
            var players = mDungeonPlayers.GetAllPlayers();
            var mainPlayer = mDungeonPlayers.GetMainPlayer();
            for (int i = 0; i < players.Count; ++i)
            {
                if (players[i] != mainPlayer)
                {
                    return players[i];
                }
            }
        }

        return null;
    }

    private void _unload(bool needEndRecord = true)
    {
        if (null != mBattle)
        {
            mBattle.End(needEndRecord);
        }

        _uninitBattle();
    }
    
    public IEnumerator Start(IASyncOperation op)
    {
        battleState = BattleState.Start;

        op.SetProgress(0.0f);

        if (null != mBattle)
        {
            yield return mBattle.Start(op);
        }
    }

    public void WaitForResult()
    {
        battleState = BattleState.WaitResult;

        if (null != mDungeonAudio)
        {
            mDungeonAudio.ClearBgm();
        }
    }

    public void End()
    {
        battleState = BattleState.End;

		if (ReplayServer.GetInstance().IsReplay())
			ReplayServer.GetInstance().EndReplay(true,"BattleMainEnd");
    }

    public void Update () 
    {
#if ROBOT_TEST
        try
        {
#endif
            int delta = (int) (Time.deltaTime * GlobalLogic.VALUE_1000);

            //if (battleState != BattleState.End)
            {
                FrameSync.instance.UpdateFrame();
            }

            if (mBattle != null)
            {
                mBattle.Update(delta);
            }
#if ROBOT_TEST
        }
        catch (Exception e)
        {
            var battle = mBattle as BaseBattle;
            if (null != battle && null != battle.recordServer)
            {
                battle.recordServer.RecordProcess(e.ToString());
            }
        }
#endif
    }

    protected static Dictionary<int,object> m_ChijiEffectMapTableDic;
    protected static Dictionary<int, object> m_ChijiSkillMapTableDic;
    /// <summary>
    /// 是否是吃鸡模式下 需要替换触发效果ID
    /// </summary>
    public static bool IsChijiNeedReplaceHurtId(int hurtId,BattleType type)
    {
        if (type != BattleType.ChijiPVP)
            return false;
        if (m_ChijiEffectMapTableDic == null)
            m_ChijiEffectMapTableDic = TableManager.instance.GetTable<ProtoTable.ChijiEffectMapTable>(); 
        if(!m_ChijiEffectMapTableDic.ContainsKey(hurtId))
            return false;
        return true;
    }

    /// <summary>
    /// 是否是吃鸡需要替换的技能ID
    /// </summary>
    public static bool IsChijiNeedReplaceSkillId(int skillId, BattleType type)
    {
        if (type != BattleType.ChijiPVP)
            return false;
        if (m_ChijiSkillMapTableDic == null)
            m_ChijiSkillMapTableDic = TableManager.instance.GetTable<ProtoTable.ChijiSkillMapTable>();
        if (!m_ChijiSkillMapTableDic.ContainsKey(skillId))
            return false;
        return true;
    }
}
