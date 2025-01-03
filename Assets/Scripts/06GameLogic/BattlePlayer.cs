using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;   

using GameClient;
using Protocol;
using ProtoTable;
using System.Text;

public class BattlePlayer
{
    public const byte kFullLoadRate = 100;

    public enum EState
    {
        None,
        //Loading,
        Normal = 0,
        Dead,
    }

    public enum eDungeonPlayerTeamType
    {
        /// <summary>
        /// 左边
        /// </summary>
        eTeamRed,

        /// <summary>
        /// 右边
        /// </summary>
        eTeamBlue,
    }

    public enum eNetState
    {
        None,
        /// <summary>
        /// 掉线
        /// </summary>
        Offline,

        /// <summary>
        /// 在线
        /// </summary>
        Online,

        /// <summary>
        /// 退出, 不回来了
        /// </summary>
        Quit,
    }

    public enum eNetQuality
    {
        /// <summary>
        /// 无
        /// </summary>
        Off  = 0,

        /// <summary>
        /// 差
        /// </summary>
        Bad  = 1,

        /// <summary>
        /// 中
        /// </summary>
        Good = 2,

        /// <summary>
        /// 好 
        /// </summary>
        Best = 3
    }

    protected eNetQuality mNetQuality = eNetQuality.Best;

    public eNetQuality netQuality
    {
        get 
        {
            return mNetQuality;
        }

        set 
        {
            mNetQuality = value;
        }
    }

    protected eNetState mNetState = eNetState.None;

    public eNetState netState
    {
        get 
        {
            return mNetState;
        }

        set 
        {
            mNetState = value;
        }
    }


    protected EState mLastState = EState.None;

    public EState lastState 
    {
        get 
        {
            return mLastState;
        }
    }

    protected EState mState = EState.Normal;

    public EState state 
    {
        get 
        {
            return mState;
        }

        set 
        {
            mLastState = mState;
            mState = value;
        }
    }

    public void onPlayerLeave()
    {
        netState = eNetState.Offline;
    }

    public void onPlayerBack()
    {
        netState = eNetState.Online;
    }

    public void onPlayerQuit()
    {
        //netState = eNetState.Quit;
    }

    public bool IsPlayerMonthCard()
    {
        if (null != playerInfo)
        {
            return 0 != playerInfo.monthcard;
        }

        return false;
    }

    public bool IsPlayerLoadFinish()
    {
        //if (state == EState.Loading)
        //{
        //
        return loadRate >= kFullLoadRate;
        //}
        //return true;
    }

    private byte mLoadRate = 0;

    public byte loadRate
    {
        get
        {
            return mLoadRate;
        }

        set
        {
            if (mLoadRate < value)
            {
                mLoadRate = value;

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonPlayerLoadProgressChanged);
            }
        }
    }

    private bool mIsAutoFight = false;

    public bool isAutoFight
    {
        get
        { 
            return mIsAutoFight;
        }

        set
        {
            Logger.LogProcessFormat("[战斗玩家] {0}, {1}, 自动战斗是否开启 {2} -> {3}", GetPlayerSeat(), GetPlayerName(), mIsAutoFight, value);

            mIsAutoFight = value;
        }
    }

    public RacePlayerInfo playerInfo;
    public BeActor playerActor;
    public PlayerStatistics statistics = new PlayerStatistics();

    private eDungeonPlayerTeamType mTeamType = eDungeonPlayerTeamType.eTeamBlue;

    public eDungeonPlayerTeamType teamType
    {
        get 
        {
            return mTeamType;
        }

        set
        { 
            Logger.LogProcessFormat("[战斗玩家] {0}, {1}, mTeamType {2} -> {3}", GetPlayerSeat(), GetPlayerName(), mTeamType, value);
            mTeamType = value; 
        }
    }


    public enum eFightingStatus
    {
        /// <summary>
        /// 未上
        /// </summary>
        None,

        /// <summary>
        /// 正在战斗
        /// </summary>
        Fighting,

        /// <summary>
        /// 已经下场
        /// </summary>
        Dead,
    }

    private eFightingStatus mFightingStatus = eFightingStatus.None;

    public eFightingStatus fightingStatus
    {
        set 
        {
            Logger.LogProcessFormat("[战斗玩家] {0}, {1}, fightingStatus {2} -> {3}", GetPlayerSeat(), GetPlayerName(), mFightingStatus, value);
            mFightingStatus = value;
        }

        get 
        {
            return mFightingStatus;
        }
    }

    private bool mIsFighting = false;

    public bool isFighting 
    {
        get 
        {
            return mIsFighting;
        }

        set
        {
            Logger.LogProcessFormat("[战斗玩家] {0}, {1}, IsFighting {2} -> {3}", GetPlayerSeat(), GetPlayerName(), mIsFighting, value);
            mIsFighting = value;
        }
    }

    public bool hasFighted
    {
        get 
        {
            return GetLastPKResult() != PKResult.INVALID;
        }
    }

    public bool isPassedInRound
    {
        get
        {
            PKResult result = GetLastPKResult();

            return PKResult.DRAW == result || PKResult.LOSE == result;
        }
    }

    public PKResult GetLastPKResult()
    {
        if (null == statistics)
        {
            return PKResult.INVALID;
        }

        return statistics.GetLastPKResult();
    }

    public int GetPKResultRank()
    {
        if (null == statistics)
        {
            return 1;
        }

        return statistics.raceResultRank;
    }

    public void SetPKResultRank(int rank)
    {
        if (null == statistics)
        {
            Logger.LogErrorFormat("[战斗玩家] 设置排名失败 {0}", rank);
            return ;
        }

        statistics.raceResultRank = rank;
    }

    public int GetKillMatchPlayerCount()
    {
        if (null == statistics)
        {
            return 0;
        }
        if (BattleDataManager.GetInstance().PkRaceType == RaceType.PK_3V3_Melee || BattleDataManager.GetInstance().PkRaceType == RaceType.PK_2V2_Melee)
        {
            return statistics.killPlayers;
        }
        return statistics.GetKillMatchPlayerCount();
    }

    

    public int GetLastPKRoundIndex()
    {
        if (null == statistics)
        {
            return -1;
        }

        return statistics.GetLastPKRoundIndex();
    }

    public UInt32 GetRaceEndResultSeasonLevel()
    {
        if (null == statistics)
        {
            return 0;
        }

        if (null == statistics.raceResult)
        {
            return 0;
        }

        if (null == statistics.raceResult.beforeRaceResult)
        {
            return 0;
        }

        return statistics.raceResult.beforeRaceResult.seasonLevel;
    }

    public UInt32 GetRaceEndResultScoreWarContriScore()
    {
        if (null == statistics)
        {
            return UInt32.MaxValue;
        }

        if (null == statistics.raceResult)
        {
            return UInt32.MaxValue;
        }

        if (null == statistics.raceResult.beforeRaceResult)
        {
            return UInt32.MaxValue;
        }

        return statistics.raceResult.beforeRaceResult.scoreWarContriScore;
    }

    public UInt32 GetRaceEndResultScoreWarBaseScore()
    {
        if (null == statistics)
        {
            return UInt32.MaxValue;
        }

        if (null == statistics.raceResult)
        {
            return UInt32.MaxValue;
        }

        if (null == statistics.raceResult.beforeRaceResult)
        {
            return UInt32.MaxValue;
        }

        return statistics.raceResult.beforeRaceResult.scoreWarBaseScore;
    }

    public UInt32 GetRaceEndResultAddGlory()
    {
        if (null == statistics)
        {
            return 0;
        }
        if (null == statistics.raceResult)
        {
            return 0;
        }
        if (null == statistics.raceResult.beforeRaceResult)
        {
            return 0;
        }
        return statistics.raceResult.beforeRaceResult.changeGlory;
    }
    public void AddPKResult(int roundIndex, byte seat, PKResult result, ePVPBattleEndType endType, uint selfHpRate)
    {
        Logger.LogProcessFormat("[战斗玩家] {0}, {1}, AddPKResult Round {2}, {3} -> {4}, HPRate {5}", GetPlayerSeat(), GetPlayerName(), roundIndex, GetLastPKResult(), result, endType, selfHpRate);
        statistics.AddPKResult(roundIndex, seat, result, endType, selfHpRate);
    }

    public uint GetLastHurtedRate()
    {
        return statistics.GetLastHurtedRate();
    }

    private uint mPKAllDamageRate = 0;
    private int mLastRound = -1;

    public void AddDamageData(byte targetSeat, uint rate)
    {
        if (mLastRound >= statistics.GetLastPKRoundIndex())
        {
            return;
        }

        mLastRound = statistics.GetLastPKRoundIndex();

        mPKAllDamageRate += rate;
        Logger.LogProcessFormat("[战斗玩家] {0}, {1}, AddDamageData TargetSet {2}, +{3} => {4}", GetPlayerSeat(), GetPlayerName(), targetSeat, rate, mPKAllDamageRate);
    }

    public uint GetPKAllDamageRate()
    {
        return mPKAllDamageRate;
    }

    public bool ConvertSceneRoomMatchPkRaceEnd2LocalBattlePlayer(Protocol.SceneRoomMatchPkRaceEnd endInfo)
    {
        if (null == endInfo)
        {
            Logger.LogErrorFormat("[战斗玩家] 比赛结果为空");
            return false;
        }

        if (null == statistics)
        {
            Logger.LogErrorFormat("[战斗玩家] 统计数据为空");
            return false;
        }

        if (!IsLocalPlayer())
        {
            Logger.LogErrorFormat("[战斗玩家] 不为本地玩家");
            return false;
        }

        PlayerStatistics.MatchRacePKResult result = statistics.raceResult;

        result.addPkCoinFromRace                  = endInfo.addPkCoinFromRace;
        result.addPkCoinFromActivity              = endInfo.addPkCoinFromActivity;
        result.totalPkCoinFromRace                = endInfo.totalPkCoinFromRace;
        result.totalPkCoinFromActivity            = endInfo.totalPkCoinFromActivity;
        result.changeSeasonExp                    = endInfo.changeSeasonExp;
        result.isInPvPActivity                    = endInfo.isInPvPActivity;
        result.raceResult                         = (PKResult)endInfo.result;

        if (null == result.beforeRaceResult)
        {
            result.beforeRaceResult = new PlayerStatistics.MatchRacePlayerResult();
        }

        result.beforeRaceResult.matchScore  = endInfo.oldMatchScore;
        result.beforeRaceResult.pkCoin      = endInfo.oldPkCoin;
        result.beforeRaceResult.pkValue     = endInfo.oldPkValue;
        result.beforeRaceResult.seasonExp   = endInfo.oldSeasonExp;
        result.beforeRaceResult.seasonLevel = endInfo.oldSeasonLevel;
        result.beforeRaceResult.seasonStar  = endInfo.oldSeasonStar;
        result.beforeRaceResult.changeGlory = endInfo.getHonor;

        if (null == result.afterRaceResult)
        {
            result.afterRaceResult = new PlayerStatistics.MatchRacePlayerResult();
        }

        result.afterRaceResult.matchScore  = endInfo.newMatchScore;
        // TODO pkCoin = oldPkCoin + add?
        /*result.afterRaceResult.pkCoin      = endInfo.newPkCoin;*/
        result.afterRaceResult.pkValue     = endInfo.newPkValue;
        result.afterRaceResult.seasonExp   = endInfo.newSeasonExp;
        result.afterRaceResult.seasonLevel = endInfo.newSeasonLevel;
        result.afterRaceResult.seasonStar  = endInfo.newSeasonStar;

        return true;
    }

    public bool ConvertRoomSlotBattleEndInfo2BattlePlayer(Protocol.RoomSlotBattleEndInfo slotInfo)
    {
        if (null == slotInfo)
        {
            Logger.LogErrorFormat("[战斗玩家] 玩家座位结果为空");
            return false;
        }

        if (null == statistics)
        {
            Logger.LogErrorFormat("[战斗玩家] 数据为空");
            return false;
        }

        if (slotInfo.seat != GetPlayerSeat())
        {
            Logger.LogErrorFormat("[战斗玩家] 玩家座位号错误");
            return false;
        }

        PlayerStatistics.MatchRacePKResult result = statistics.raceResult;

        result.raceResult                         = (PKResult)slotInfo.resultFlag;
        result.roomId                             = slotInfo.roomId;
        result.roomType                           = (RoomType)slotInfo.roomType;

        if (null == result.beforeRaceResult)
        {
            result.beforeRaceResult = new PlayerStatistics.MatchRacePlayerResult();
        }

        result.beforeRaceResult.seasonLevel = slotInfo.seasonLevel;
        result.beforeRaceResult.seasonExp   = slotInfo.seasonExp;
        result.beforeRaceResult.seasonStar  = slotInfo.seasonStar;
        result.beforeRaceResult.scoreWarContriScore = slotInfo.scoreWarContriScore;
        result.beforeRaceResult.scoreWarBaseScore = slotInfo.scoreWarBaseScore;
        result.beforeRaceResult.changeGlory = slotInfo.getHonor;

        return true;
    }

    private bool mIsVote = false;

    public bool isVote 
    {
        get
        {
            return mIsVote;
        }

        set 
        {
            Logger.LogProcessFormat("[战斗玩家] {0}, {1}, Vote {2} -> {3}", GetPlayerSeat(), GetPlayerName(), mIsVote, value);
            mIsVote = value;
        }
    }

    public RoomType GetMatchRoomType()
    {
        if (null == statistics)
        {
            return RoomType.ROOM_TYPE_INVALID;
        }

        if (null == statistics.raceResult)
        {
            return RoomType.ROOM_TYPE_INVALID;
        }

        return statistics.raceResult.roomType;
    }

    public byte GetPlayerSeat()
    {
        if (!IsDataValidBattlePlayer(this))
        {
            return byte.MaxValue;
        }

        return playerInfo.seat;
    }

    public string GetPlayerName()
    {
        if (!IsDataValidBattlePlayer(this))
        {
            return string.Empty;
        }

        return playerInfo.name;
    }

    public string GetPlayerServerName()
    {
        if (!IsDataValidBattlePlayer(this))
        {
            return string.Empty;
        }

        return playerInfo.serverName;
    }

    public bool IsTeamBlue()
    {
        return !IsTeamRed();
    }

    public bool IsTeamRed()
    {
        return eDungeonPlayerTeamType.eTeamRed == teamType;
    }

    public bool IsLocalPlayer()
    {
#if LOGIC_SERVER
        return false;
#else
        return IsDataValidBattlePlayer(this) && (playerInfo.accid == ClientApplication.playerinfo.accid);
#endif
    }

    public int GetPlayerCamp()
    {
        return IsTeamRed() ? (int)ProtoTable.UnitTable.eCamp.C_HERO : (int)ProtoTable.UnitTable.eCamp.C_ENEMY;
    }

    public string GetBattlePlayerInfo()
    {
        if (!IsDataValidBattlePlayer(this))
        {
            return "[玩家] ****无效对象***";
        }
        
        return string.Format("\n[玩家基本信息] {0} 座位号:{1} 账号ID:{2} 角色ID:{3}\n等级:{4} 职业:{5} HP:{6}，MP:{7}\n工会名字:{8}\n匹配积分:{9}, pk积分:{10}\n赛季属性:{11}, 赛季星级:{12}, 赛季等级:{13}\n月卡:{14}\n队伍类型:{15}, 战斗状态:{16}，是否在战斗：{17}, 是否已经战斗过:{18}, 最近一场战斗结果:{19}, 最近一场战斗场次:{20}\n", 
            /* 0  - 3  */ playerInfo.name, playerInfo.seat, playerInfo.accid, playerInfo.roleId,
            /* 4  - 7  */ playerInfo.level, playerInfo.occupation, playerInfo.remainHp, playerInfo.remainMp,
            /* 8  - 8  */ playerInfo.guildName,
            /* 9  - 10 */ playerInfo.matchScore, playerInfo.pkValue,
            /* 11 - 13 */ playerInfo.seasonAttr, playerInfo.seasonStar, playerInfo.seasonLevel,
            /* 14 - 14 */ playerInfo.monthcard,
            /* 15 - 20 */ teamType, fightingStatus, isFighting, hasFighted, GetLastPKResult(), GetLastPKRoundIndex()
        );
    }


#region 静态帮助函数

    

    public static eDungeonPlayerTeamType GetTargetTeamType(eDungeonPlayerTeamType type)
    {
        switch (type)
        {
            case eDungeonPlayerTeamType.eTeamBlue:
                return eDungeonPlayerTeamType.eTeamRed;
            case eDungeonPlayerTeamType.eTeamRed:
                return eDungeonPlayerTeamType.eTeamBlue;
            default:
                Logger.LogErrorFormat("[战斗] 错误类型 {0}", type);
                return eDungeonPlayerTeamType.eTeamRed;
        }
    }

    public static bool IsDataValidBattlePlayer(BattlePlayer player)
    {
        if (null == player)
        {
            //Logger.LogError("[战斗] player 为空");
            return false;
        }

        if (null == player.playerInfo)
        {
            //Logger.LogError("[战斗] player.playerInfo 为空");
            return false; 
        }

        return true;
    }

    public static Dictionary<int, int> GetSkillInfo(RacePlayerInfo info)
    {
        Dictionary<int, int> skillInfo = new Dictionary<int, int>();
        for (int i = 0; i < info.skills.Length; ++i)
        {
            var skill = info.skills[i];
            if (!skillInfo.ContainsKey(skill.id))
                skillInfo.Add(skill.id, skill.level);
            else
                Logger.LogErrorFormat("alreay has skill:{0} level:{1} this level:{2}", skill.id, skillInfo[skill.id], skill.level);
        }

        return skillInfo;
    }

    public static Dictionary<int, int> GetSkillSlotMap(RacePlayerInfo info)
    {
        Dictionary<int, int> slotMap = new Dictionary<int, int>();
        for (int i = 0; i < info.skills.Length; ++i)
        {
            var skill = info.skills[i];
            int key = skill.slot + 3;
            if (!slotMap.ContainsKey(key))
                slotMap.Add(skill.slot + 3, skill.id);
        }

        return slotMap;
    }

    public static List<int> GetSkillList(RacePlayerInfo info)
    {
        List<int> slotMap =new List<int>();
        for (int i = 0; i < info.skills.Length; ++i)
        {
            slotMap.Add(info.skills[i].id);
        }

        return slotMap;
    }

	public static int GetTitleID(RacePlayerInfo player)
	{
		int titleID = 0;
		for (int i = 0; i < player.equips.Length; ++i)
		{
			RaceEquip equip = player.equips[i];

            //第一场战斗和技能连招的玩家装备 PK练习场木桩的装备 不是服务器下发的 所有不进行过滤  
            if (BattleMain.battleType != BattleType.NewbieGuide && BattleMain.battleType != BattleType.TrainingSkillCombo && BattleMain.battleType != BattleType.Training)
            {
                if (Array.IndexOf(player.wearingEqIds, equip.uid) < 0)
                {
                    continue;
                }
            }

            var data = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)equip.id);
			if (data != null)
			{
				if (data.SubType == ItemTable.eSubType.TITLE)
				{
					titleID = (int)equip.id;
					break;
				}
			}
		}

		return titleID;
	}

    /// <summary>
    /// 获取当前穿戴方案中的装备
    /// </summary>
    public static List<ItemProperty> GetEquips(RacePlayerInfo player,bool isPVP)
    {
        List<ItemProperty> property = new List<ItemProperty>();
        for (int i = 0; i < player.equips.Length; ++i)
        {
            RaceEquip equip = player.equips[i];

            //第一场战斗和技能连招的玩家装备 PK练习场木桩的装备 不是服务器下发的 所有不进行过滤  
            if(BattleMain.battleType != BattleType.NewbieGuide && BattleMain.battleType != BattleType.TrainingSkillCombo && BattleMain.battleType != BattleType.Training)
            {
                if (Array.IndexOf(player.wearingEqIds, equip.uid) < 0)
                {
                    continue;
                }
            }
            
            ItemProperty itemProperty = GetItemProperty(equip,isPVP);
            if (itemProperty != null)
            {
                property.Add(itemProperty);
            }
            else
            {
                Logger.LogErrorFormat("带入的装备{0}无法找到", equip.id);
            }
        }

        return property;
    }

    /// <summary>
    /// 根据服务器下发的装备转成客户端用的装备数据
    /// </summary>
    public static ItemProperty GetEquip(RaceEquip equip, bool isPvp)
    {
        ItemProperty itemProperty = GetItemProperty(equip, isPvp);
        if (itemProperty == null)
        {
            Logger.LogErrorFormat("带入的装备{0}无法找到", equip.id);
        }
        return itemProperty;
    }

    public static Dictionary<int, string> GetAvatar(PlayerAvatar avatar)
    {
        Dictionary<int, string> fashions = new Dictionary<int, string>();
       // bool oldFashion = false;
       // string headWearPath = string.Empty;

        var tmpEquipItemIds = BeUtility.CopyVector(avatar.equipItemIds);
        BeUtility.DealWithFashion(tmpEquipItemIds);

        for (int i = 0; i < tmpEquipItemIds.Length; ++i)
        {
            var equipData = TableManager.GetInstance().GetTableItem<ItemTable>((int)tmpEquipItemIds[i]);
            if (equipData == null)
            {
                continue;
            }

            bool bChange = false;
           
            if (equipData.SubType >= ItemTable.eSubType.FASHION_HAIR && equipData.SubType <= ItemTable.eSubType.FASHION_EPAULET)
            {
                bChange = true;
            }
            else if(equipData.SubType >= ItemTable.eSubType.HEAD && equipData.SubType <= ItemTable.eSubType.BOOT)
            {
                if(BattleDataManager.GetInstance().PkRaceType == RaceType.ChiJi)
                {
                    bChange = true;
                }
            }

            if(!bChange)
            {
                continue;
            }

            var resData = TableManager.GetInstance().GetTableItem<ResTable>(equipData.ResID);
            if (resData != null)
            {
                // if(equipData.SubType == ProtoTable.ItemTable.eSubType.FASHION_HEAD)
                // {
                //     if (!resData.newFashion)
                //     {
                //         oldFashion = true;
                //     }
                // }
                // if (equipData.SubType == ProtoTable.ItemTable.eSubType.FASHION_EPAULET)
                // {
                //     headWearPath = resData.ModelPath;
                //     continue;
                // }
                fashions.Add((int)equipData.SubType, resData.ModelPath);
            }
        }
        // if (!oldFashion && !string.IsNullOrEmpty(headWearPath))
        // {
        //     fashions.Add((int)ProtoTable.ItemTable.eSubType.FASHION_EPAULET, headWearPath);
        // }

        return fashions;
    }

    public static Dictionary<int, string> GetAvatar(RacePlayerInfo player)
    {
        return GetAvatar(player.avatar);
        
    }


    public static List<ItemProperty> GetSideEquips(RacePlayerInfo player,bool isPVP)
    {
        List<ItemProperty> property = new List<ItemProperty>();
        for (int i = 0; i < player.secondWeapons.Length; ++i)
        {
            RaceEquip equip = player.secondWeapons[i];

            ItemProperty itemProperty = GetItemProperty(equip, isPVP);
            if (itemProperty != null)
                property.Add(itemProperty);
            else
            {
                Logger.LogErrorFormat("带入的装备{0}无法找到", equip.id);
            }
        }

        return property;
    }

    private static ItemProperty GetItemProperty(RaceEquip equip,bool isPVP)
    {
        ItemData item = ItemDataManager.CreateItemDataFromTable((int)equip.id);

        if (item != null)
        {
            item.isPVP = isPVP;
            item.BaseProp.props[(int)EEquipProp.PhysicsAttack] = (int)equip.phyAtk;
            item.BaseProp.props[(int)EEquipProp.MagicAttack] = (int)equip.magAtk;
            item.BaseProp.props[(int)EEquipProp.PhysicsDefense] = (int)equip.phydef;
            item.BaseProp.props[(int)EEquipProp.MagicDefense] = (int)equip.magdef;
            // by ckm
            if(item.SubType != (int)ItemTable.eSubType.ST_BXY_EQUIP)
            {
                item.BaseProp.props[(int)EEquipProp.Strenth] = (int)equip.strenth;
                item.BaseProp.props[(int)EEquipProp.Stamina] = (int)equip.stamina;
                item.BaseProp.props[(int)EEquipProp.Intellect] = (int)equip.intellect;
                item.BaseProp.props[(int)EEquipProp.Spirit] = (int)equip.spirit;
            }
            else
            {
                if(equip.strenth != 0)
                {
                    item.AddAttachBxyBuffIID(item.BaseProp, 0, (int)equip.strenth);
                }
                if(equip.intellect != 0)
                {
                    item.AddAttachBxyBuffIID(item.BaseProp, 0, (int)equip.intellect);
                }
                if(equip.spirit != 0)
                {
                    item.AddAttachBxyBuffIID(item.BaseProp, 0, (int)equip.spirit);
                }
                if(equip.stamina != 0)
                {
                    item.AddAttachBxyBuffIID(item.BaseProp, 0, (int)equip.stamina);
                }
            }
            //强化
            item.BaseProp.props[(int)EEquipProp.IgnoreMagicAttack] = (int)equip.disMagAtk;
            item.BaseProp.props[(int)EEquipProp.IgnorePhysicsAttack] = (int)equip.disphyAtk;
            item.BaseProp.props[(int)EEquipProp.IngoreIndependence] = (int)equip.indpendAtkStreng;
            item.BaseProp.props[(int)EEquipProp.IgnoreMagicDefense] = (int)equip.dismagdef;
            item.BaseProp.props[(int)EEquipProp.IgnorePhysicsDefense] = (int)equip.disphydef;

            item.BaseProp.props[(int)EEquipProp.IgnorePhysicsDefenseRate] = (int)equip.phyDefPercent;
            item.BaseProp.props[(int)EEquipProp.IgnoreMagicDefenseRate] = (int)equip.magDefPercent;
            item.BaseProp.props[(int)EEquipProp.Independence] = (int)equip.independAtk;

            //装备增幅
            if (equip.equipType == (byte)EEquipType.ET_REDMARK)
            {
                if (equip.enhanceType == (byte)EGrowthAttrType.GAT_STRENGTH)
                {
                    item.BaseProp.props[(int)EEquipProp.Strenth] += (int)equip.enhanceNum * GlobalLogic.VALUE_1000;
                }
                else if (equip.enhanceType == (byte)EGrowthAttrType.GAT_INTELLIGENCE)
                {
                    item.BaseProp.props[(int)EEquipProp.Intellect] += (int)equip.enhanceNum * GlobalLogic.VALUE_1000;
                }
                else if (equip.enhanceType == (byte)EGrowthAttrType.GAT_STAMINA)
                {
                    item.BaseProp.props[(int)EEquipProp.Stamina] += (int)equip.enhanceNum * GlobalLogic.VALUE_1000;
                }
                else if (equip.enhanceType == (byte)EGrowthAttrType.GAT_SPIRIT)
                {
                    item.BaseProp.props[(int)EEquipProp.Spirit] += (int)equip.enhanceNum * GlobalLogic.VALUE_1000;
                }
            }

            if (equip.strPropEx.Length > 0)
            {
                //属强
                for (int j = 1; j < item.BaseProp.magicElementsAttack.Length; ++j)
                    item.BaseProp.magicElementsAttack[j] = equip.strPropEx[j - 1];
            }

            if (equip.defPropEx.Length > 0)
            {
                //属抗
                for (int j = 1; j < item.BaseProp.magicElementsDefence.Length; ++j)
                    item.BaseProp.magicElementsDefence[j] = equip.defPropEx[j - 1];
            }

            //异抗
            item.BaseProp.props[(int)EEquipProp.AbormalResist] = equip.abnormalResistsTotal;

            if (equip.abnormalResists.Length > 0)
            {
                for (int j = 0; j < Global.ABNORMAL_COUNT; ++j)
                    item.BaseProp.abnormalResists[j] = equip.abnormalResists[j];
            }
            var table = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType3.SVT_Independent);
            if (equip.fashionAttrId > 0)
            {
                EquipProp equipProp = EquipProp.CreateFromTable((int)equip.fashionAttrId);
                if (isPVP)
                {
                    if (equipProp != null)
                    {
                        if (table != null)
                        {
                            int factor = table.Value;
                            var logicFactor = new VFactor(factor, GlobalLogic.VALUE_1000);
                            equipProp.props[(int)EEquipProp.Independence] = equipProp.props[(int)EEquipProp.Independence] * logicFactor;
                        }
                    }
                }
                item.BaseProp = equipProp;
            }

            if (item.Type == ItemTable.eType.FASHION)
            {
                EquipProp equipProp = EquipProp.CreateFromTable(item.FashionBaseAttributeID);
                if(isPVP)
                {
                    if (equipProp != null)
                    {
                        if (table != null)
                        {
                            int factor = table.Value;
                            var logicFactor = new VFactor(factor, GlobalLogic.VALUE_1000);
                            equipProp.props[(int)EEquipProp.Independence] = equipProp.props[(int)EEquipProp.Independence] * logicFactor;
                        }
                    }
                }
                if (equipProp != null)
                {
                    if (item.BaseProp != null)
                    {
                        item.BaseProp += equipProp;
                    }
                    else
                    {
                        item.BaseProp = equipProp;
                    }
                }
            }

            for (int j = 0; j < equip.properties.Length; ++j)
            {
                var pro = equip.properties[j];
                EServerProp prop = (EServerProp)pro.type;
                MapEnum mapEnum = Utility.GetEnumAttribute<EServerProp, MapEnum>(prop);
                if (mapEnum != null)
                {
                    item.RandamProp.props[(int)mapEnum.Prop] = (int)pro.value;
                }
            }

            if (equip.magicCard > 0)
            {
                if (item.mPrecEnchantmentCard != null)
                {
                    item.mPrecEnchantmentCard = new PrecEnchantmentCard
                    {
                        iEnchantmentCardID = (int)equip.magicCard,
                        iEnchantmentCardLevel = (int)equip.magicCardLv,
                    };
                }
            }

            if (equip.mountPrecBeads != null && equip.mountPrecBeads.Length > 0)
            {
                item.PrecBeadBattle = BeUtility.SwitchPrecBead(equip.mountPrecBeads);
            }
            if (!isPVP && equip.inscriptionIds != null && equip.inscriptionIds.Length > 0)
            {
                item.InscriptionHoles = BeUtility.SwitchInscriptHoleData(equip.inscriptionIds);
            }
            item.StrengthenLevel = equip.strengthen;

            ItemProperty ip = item.GetBattleProperty();
            ip.itemID = (int)equip.id;
            ip.guid = equip.uid;
            return ip;
        }
        return null;
    }

    public static List<Battle.DungeonBuff> GetBuffList(RacePlayerInfo player)
    {
        if (null == player || null == player.buffs)
        {
            return null;
        }

        var list = new List<Battle.DungeonBuff>();

        for (int i = 0; i < player.buffs.Length; ++i)
        {
            var buff = player.buffs[i];

            var buffData = TableManager.instance.GetTableItem<ProtoTable.BuffTable>((int)buff.id);
            if (buffData != null)
            {
                Battle.DungeonBuff.eBuffDurationType buffDurationType = Battle.DungeonBuff.eBuffDurationType.Battle;
                if (buffData.durationType == (int)Battle.DungeonBuff.eBuffDurationType.OnlyUseInBattle)
                    buffDurationType = Battle.DungeonBuff.eBuffDurationType.OnlyUseInBattle;
                list.Add(new Battle.DungeonBuff()
                {
                    id = (int)buff.id,
                    overlay = (int)buff.overlayNums,
                    lefttime = (float)buff.duration,
                    duration = (float)buff.duration,
                    // TODO 这里的类型最好由服务端发送过来
                    type = buffDurationType
                });
            }
        }

        return list;
    }

    public static int GetWeaponStrengthenLevel(RacePlayerInfo player)
    {
        if (player == null)
            return 0;

        for (int i = 0; i < player.equips.Length; ++i)
        {
            var equip = player.equips[i];
            var itemData = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)equip.id);
            if (itemData != null && (EEquipWearSlotType)itemData.SubType == EEquipWearSlotType.EquipWeapon)
            {
                return equip.strengthen;
            }
        }

        //TODO
        return 1;
    }

	public static List<BuffInfoData> GetRankBuff(RacePlayerInfo player)
	{
		if (player == null)
			return null;

		List<BuffInfoData> buffInfos = new List<BuffInfoData>();
		var buffList = SeasonDataManager.GetSeasonAttrBuffIDs((int)player.seasonAttr);
		if (buffList != null)
		{
			for(int i=0; i<buffList.Count; ++i)
			{
				if (buffList[i] > 0)
				{
					BuffInfoData buffInfo = new BuffInfoData();
					buffInfo.buffID = buffList[i];
					buffInfos.Add(buffInfo);
				}
			}
		}

		return buffInfos;
	}

    public static int GetCrsytalNum(RacePlayerInfo player)
    {
        int ret = 0;
        if (player.raceItems.Length > 0)
        {
            for(int i=0; i<player.raceItems.Length; ++i)
            {
                if (player.raceItems[i].id == Global.CRYSTAL_ITEM_ID)
                {
                    ret += player.raceItems[i].num;
                }
            }
        }

        return ret;
    }

	public static PetData GetPetData(RacePlayerInfo player,bool isPvp)
	{
		PetData petData = new PetData ();

		List<BuffInfoData> list = new List<BuffInfoData> ();
		for(int i=0; i<player.pets.Length; ++i)
		{
			var data = player.pets [i];
			var petTableData = TableManager.GetInstance ().GetTableItem<ProtoTable.PetTable> ((int)data.dataId);
			if (petTableData == null)
				continue;

			//天生技能
			if (petTableData.InnateSkill > 0)
			{
				BeUtility.AddBuffFromSkill(petTableData.InnateSkill, data.level, list, isPvp);
			}

			//主宠物
			if (petTableData.PetType == PetTable.ePetType.PT_ATTACK)
			{
				petData.id = petTableData.MonsterID;//(int)data.dataId;
                if(petTableData.Skills.Length <= (int)data.skillIndex || (int)data.skillIndex  < 0)
                {
                    Logger.LogErrorFormat("player accid {0} roleid {1} serverName {2} GetPetData petid {3} skill is out of Range {4}", player.accid, player.roleId,player.serverName, data.dataId, data.skillIndex);

                    data.skillIndex = 0;//此处为无可奈何的处理，正式服理论上不会出现该问题
                }
                if (petTableData.Skills.Length <= data.skillIndex)
                {
                    petData.skillID = 0;
                }
                else
                {
                    petData.skillID = petTableData.Skills[data.skillIndex];
                }
				petData.hunger = data.hunger;
				petData.level = data.level;
			}
			//其他宠物
			else {
				//int skillID = petTableData.Skills[data.skillIndex];

				int skillID = PetDataManager.GetPetSkillIDByJob((int)data.dataId, (int)player.occupation, (int)data.skillIndex);

				//Logger.LogErrorFormat("pet skill id:{0}", skillID);

				BeUtility.AddBuffFromSkill(skillID, data.level, list, isPvp);
			}
		}

		petData.buffs = list;

		return petData;
	}


    public static void DebugPrint(RacePlayerInfo player)
    {
        if (player == null)
            return;

        StringBuilder str = StringBuilderCache.Acquire(2000);
        str.AppendFormat("<color=red>[robot]</color> name:{0} level:{1} ai:{2}\n", player.name, player.level, player.robotAIType);
        for (int i = 0; i < player.skills.Length; ++i)
        {
            str.AppendFormat("skill {0}: L{1}\n", player.skills[i].id, player.skills[i].level);
        }
        for (int i = 0; i < player.equips.Length; ++i)
        {
            str.AppendFormat("equip:{0}\n", player.equips[i].id);
        }

        Logger.LogWarningFormat(str.ToString());
    }
#endregion

    #region ItemUse

    private RaceItem _findRaceItemById(int id)
    {
        RaceItem raceItem = null;

        for (int i = 0; i < playerInfo.raceItems.Length; ++i)
        {
            raceItem = playerInfo.raceItems[i];

            if (null != raceItem && raceItem.id == id)
            {
                return raceItem;
            }
        }

        return null;
    }

    private RaceItem _addRaceItemById(int id)
    {
        RaceItem item = _findRaceItemById(id);

        if (null == item)
        {
            List<RaceItem> raceItems = new List<RaceItem>(playerInfo.raceItems);

            item = new RaceItem();
            item.id = (uint)id;
            item.num = 0;
            raceItems.Add(item);

            playerInfo.raceItems = raceItems.ToArray();
        }

        return item;
    }

    private int _getItemIDByRaceType(Battle.DungeonItem.eType type)
    {
        ProtoTable.ItemConfigTable itemconfig = TableManager.instance.GetTableItem<ProtoTable.ItemConfigTable>((int)type);

        if (null != itemconfig)
        {
            return (itemconfig.ItemID);
        }

        return 0;
    }


    private RaceItem _findRaceItem(Battle.DungeonItem.eType type)
    {
        return _findRaceItemById(_getItemIDByRaceType(type));
    }

    public bool CanUseItem(Battle.DungeonItem.eType type, ushort num = 1)
    {
        return CanUseItemById(_getItemIDByRaceType(type), num);
    }

    public bool CanUseItemById(int id, ushort num = 1)
    {
        if (null == playerInfo || null == playerInfo.raceItems)
        {
            return false;
        }

        RaceItem raceItem = _findRaceItemById(id);
        if (null == raceItem)
        {
            return false;
        }

        return null != raceItem && raceItem.num >= num;
    }

    public int AddItemById(int id, ushort num)
    {
        if (null == playerInfo || null == playerInfo.raceItems)
        {
            return 0;
        }

        RaceItem raceItem = _addRaceItemById(id);
        if (null == raceItem)
        {
            return 0;
        }

        raceItem.num += num;

        Logger.LogErrorFormat("[BattlePlayer] 添加数量 {0}, {1}", num, raceItem.num);

        return (int)raceItem.num;
    }

    public int UseItemById(int id, ushort num = 1)
    {
        if (null == playerActor)
        {
            return 0;
        }

        if (null == playerInfo || null == playerInfo.raceItems)
        {
            return 0;
        }

        RaceItem raceItem = _findRaceItemById(id);

        if (null != raceItem)
        {
            if (raceItem.num >= num)
            {
                raceItem.num -= num;

                if (num > 0)
                {
                    var cmd = new UseItemFrameCommand();

                    cmd.itemID = raceItem.id;
                    cmd.itemNum = num;

                    statistics.UseItem((int)raceItem.id, num);

                    FrameSync.instance.FireFrameCommand(cmd, true);
                }

                return raceItem.num;
            }
        }

        return 0;
    }

    public int UseItem(Battle.DungeonItem.eType type, ushort num = 1)
    {
        return UseItemById(_getItemIDByRaceType(type), num);
    }

    #endregion
}
