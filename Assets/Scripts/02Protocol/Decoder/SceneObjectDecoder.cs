using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using System.Reflection;

namespace Protocol
{
    // 场景object的属性枚举
    public enum SceneObjectAttr
    {
        SOA_INVALID,    //无效
        SOA_NAME,       //名字	string
        SOA_LEVEL,      //等级	UInt16
        SOA_EXP,        //经验	UInt64
        SOA_BLESSEXP,   //祝福经验	UInt32
        SOA_SEX,        //性别	UInt8
        SOA_OCCU,       //职业	UInt8
        SOA_PKMODE,     //pk模式	UInt8
        SOA_PKCOLOR,    //pk颜色	UInt8
        SOA_VIPLVL,     //vip等级   UInt8
        SOA_QQVIPINFO,  //qq平台特权信息 QQVipInfo
        SOA_EVIL,       //罪恶	UInt16;
        SOA_ATTACK,     //攻击	UInt32
        SOA_DEFENSE,    //防御	UInt32
        SOA_HIT,        //命中	UInt8
        SOA_DODGE,      //闪避	UInt8
        SOA_THUMP,      //暴击	UInt32
        SOA_HOLDOFF,    //抵挡	UInt32
        SOA_THUMPEFFECT,    //暴击效果 UInt32
        SOA_CHAOSATTACK,    //混沌攻击 UInt32
        SOA_CHAOSDEFENSE,   //混沌防御 UInt32
        SOA_SWIM,           //眩晕	   UInt32
        SOA_SPEEDDOWN,      //减速     UInt32
        SOA_IMMOBILE,       //定身	   UInt32
        SOA_SILENT,         //沉默	   UInt32
        SOA_SWIM_DEF,       //眩晕抗性	UInt32
        SOA_SPEEDDOWN_DEF,  //减速抗性	UInt32
        SOA_IMMOBILE_DEF,   //定身抗性	UInt32
        SOA_SILENT_DEF,     //沉默抗性	UInt32
        SOA_HP,             //生命值	UInt32
        SOA_MAXHP,          //最大生命值	UInt32
        SOA_POWER,          //战斗力	UInt32
        SOA_MOVESPEED,      //移动速度	UInt16
        SOA_POSITION,       //当前位置	CLPosition, UInt16+UInt16
        SOA_DIR,            //方向		UInt8
        SOA_VEHICLEID,      //车id		ObjID_t
        SOA_SKILLS,         //技能列表  只发送给自身 格式:id(UInt16)  + level(UInt8) ... 0(UInt16)
        SOA_BUFFS,          //buff列表 同步给自身和可见玩家 格式:id(UInt16) + value(Int32) + time(UInt32) ... 0(UInt16)
        SOA_ITEMCD,         //道具cd	只同步给自身 格式:cd组(UInt8) + cd时间(UInt32) ... 0(UInt8)
        SOA_GOLD,         //金币	UInt32
        SOA_BINDGOLD,       //绑定金币 UInt32
        SOA_POINT,           //点券 UInt32
        SOA_BINDPOINT,      //绑定点券 UInt32
        SOA_SPIRITSTONE,    //灵石 UInt32
        SOA_HONOR,         //荣誉 UInt32
        SOA_PACKSIZE,       //随身普通包裹大小 UInt32
        SOA_STORAGESIZE,    //仓库大小		   UInt8
        SOA_EQUIPS,         //周围可见的装备  衣服(UInt32) + 装备(UInt32)
        SOA_SUITEFFECT,     //套装效果 UInt32 |全套强化等级UInt4|全套6级宝石数UInt12(宝石等级UInt4 + 宝石个数UInt8)|全套品质UInt8|全套随机属性星级UInt8|
        SOA_BUFFCONTROL,    //buff控制标识	UInt16(BuffControlFlag组合)
        SOA_DATAID,         //数据项id UInt32
        SOA_STATUS,         //状态     UInt8   
        SOA_NPCTASKSTATUS,  //npc任务状态 UInt8 NpcTaskStatus
        SOA_SKILLBAR,       //技能栏  只发送自身 格式: pos(UInt8 0开始) + id(UInt16) ... 0xFF(UInt8)
        SOA_GAMEOPTIONS,    //游戏选项
        SOA_SUBNAME,        //子名字 string
        SOA_TEAMMASTER,     //是否队长	UInt8 0表示不是 1表示是队长
        SOA_TEAMSIZE,       //队伍大小  UInt8 0表示没队伍
        SOA_TRIBEID,        //所属部落id	UInt64
        SOA_TRIBEPOST,      //部落职务	UInt8
        SOA_ITEMBIND,       //道具绑定  UInt8
        SOA_ITEMQUALITY,    //道具品质	UInt8
        SOA_ITEMNUM,        //道具数量	UInt16
        SOA_TRAPID,         //陷阱id UInt32
        SOA_MOUNTID,        //当前骑乘的坐骑 id(UInt32)
        SOA_BATTLEGROUP,    //战场阵营	UInt8
        SOA_OWNERID,        //道具、召唤物等所有者ID UInt64
        SOA_SUMMONAIMODE,   //召唤物ai模式	UInt8	
        SOA_TEAMID,         //道具所属队伍ID UInt32
        SOA_AUTOHOOK,       //自动挂机信息，只在场景服务器同步
        SOA_ESCORTERID,     //护送者id ObjID_t
        SOA_TITLEMASK,      //称号掩码 16长度的字节数组
        SOA_TITLE,          //当前使用称号  UInt32
        SOA_COMPLETETIME,   //建筑物NPC的建成时间 UInt32
        SOA_DEATHOWNER,     //死亡归属者 ObjID_t
        SOA_POWERDETAIL,    //战斗力详细信息 只发送给自身，格式：id(UInt8) + 值(UInt32) ... 0(UInt8)
        SOA_UPDATEBULLETIN, //更新版本号 UInt32
        SOA_FUNCNOTIFY,     //预告功能 char[8]
        SOA_STORY,          //当前剧情 UInt16
        SOA_ACTIONFLAG,     //行为标记 UInt32
        SOA_FUNCFLAG,       //功能开启标志 char[8]
        SOA_LOVERNAME,      //伴侣名字 string
        SOA_SRCZONEID,      //原区号	 UInt16
        SOA_PETDATAID,      //宠物数据id UInt32
        S0A_PETTRANSLIST,   //宠物幻化列表 char[25]
        SOA_TRANSID,        //幻化id UInt32
        SOA_DAYCHARGENUM,      //玉石   UInt32
        SOA_GATHERSOULVAL,  //聚魂值 UInt32
        SOA_SOULPACK,       //兽魂背包 PetSoul[12]
        SOA_ZONEID,         //区号	UInt32
        SOA_TRANSFIGUREID,  //当前变身ID		UInt32
        SOA_SCENE_POS,      //场景中的位置 ScenePos, UInt32+UInt32
        SOA_SCENE_DIR,      //场景中的方向 UInt16(360度)
        SOA_SP,             //SP	UInt32
        SOA_AWAKEN,         //觉醒  UInt8
        SOA_ITEMBAR,        //物品栏  只发送自身 格式: pos(UInt8 0开始) + id(UInt32) ... 0(UInt8)
        SOA_PKVALUE,        //决斗场积分  UInt32
        SOA_FATIGUE,        //疲劳值  UInt16
        SOA_WARRIOR_SOUL,   //勇者之魂   UInt32
        SOA_MATCH_SCORE,    //匹配积分 UInt32
        SOA_WARP_STONE,     //次元石	格式:  id(UInt32) + level(UInt8) + energy(UInt32) ... 0(UInt32)
        SOA_COUNTER,        //各种计数 CounterMgr
        SOA_FOLLOWPET,      //随从 UInt32
        SOA_PK_COIN,        //决斗币 UInt32
        SOA_AVATAR,         //外观 PlayerAvatar
        SOA_ALIVE_COIN,     //复活币
        SOA_VIP,            //VIP
        SOA_CREATE_TIME,    //创建时间
        SOA_NEW_BOOT,       //新手引导
        SOA_TOWER_WIPEOUT_END_TIME,	//死亡之塔扫荡结束时间 UInt32
        SOA_GUILD_POST,	    //帮会职务 UInt8
        SOA_GUILD_CONTRI,	//帮贡 UInt32
        SOA_GUILD_SHOP,		//公会商店ID UInt32
        SOA_REDPOINT,       //红点 UInt32
        SOA_GUILD_NAME,     //公会名
        SOA_GUILD_BATTLE_NUMBER,    //公会战次数	UInt32
        SOA_GUILD_BATTLE_SCORE,     //公会战积分	UInt32
        SOA_GUILD_BATTLE_REWARD,	//公会战奖励	
        SOA_DAILY_TASK_SOURCE,          //每日任务积分
        SOA_DAILY_TASK_REWARD_MASK,     //每日任务积分奖励
        SOA_WUDAO_STATUS,           //武道大会状态
        SOA_MONTH_CARD_EXPIRE_TIME,	//月卡到期时间 UInt32
        SOA_CUSTOM_DATA,	        //自定义数据 UInt32
        SOA_SEASON_LEVEL,           //赛季段位 UInt32
        SOA_SEASON_STAR,            //赛季星级 UInt32
        SOA_SEASON_UPLEVEL_RECORD,	//赛季晋级赛记录	PkRaceResult + ... + 0(UInt8)
        SOA_SEASON_ATTR,            //赛季属性id    UInt8
        SOA_SEASON_KING_MARK,       //王者印记  UInt16
        SOA_AUCTION_REFRESH_TIME,   //上一次刷新拍卖行时间
        SOA_AUCTION_ADDITION_BOOTH, //拍卖行额外栏位
        SOA_GOLDJAR_POINT,          //金罐积分
        SOA_MAGJAR_POINT,           //魔罐积分
        SOA_BOOT_FLAG,              //弱引导
        SOA_SEASON_EXP,             //赛季经验	UInt32
        SOA_PET_FOLLOW,             //跟随的宠物 UInt32
		SOA_POTION_SET,				//
		SOA_PRE_OCCU,               //预转职职业 UInt8
		SOA_GUILD_BATTLE_LOTTERY_STATUS,    //公会战抽奖状态 UInt8
		SOA_PVP_SKILLS,                     //PVP技能列表  只发送给自身 格式:id(UInt16)  + level(UInt8) ... 0(UInt16)
		SOA_PVP_SKILLBAR,                   //技能栏  只发送自身 格式: pos(UInt8 0开始) + id(UInt16) ... 0xFF(UInt8)
		SOA_PVP_SP,                     //PVPSP	UInt32
        SOA_ACHIEVEMENT_SCORE,      //成就积分 UInt32
        SOA_ACHIEVEMENT_TASK_REWARD_MASK, //成就积分奖励
        SOA_WEAPON_BAR,                     //武器栏
        SOA_APPOINTMENT_OCCU,               //是否是预约角色
		SOA_PACKAGE_SIZE_ADDITION,  //包裹容量加成
		SOA_MONEY_MANAGE_STATUS,                   //理财管理状态
		SOA_MONEY_MANAGE_REWARD_MASK,       //理财管理奖励
		SOA_SCORE_WAR_SCORE,                //积分赛积分
		SOA_SCORE_WAR_BATTLE_COUNT,         //积分赛战斗次数
		SOA_SCORE_WAR_REWARD_MASK,           //积分赛参与奖

		SOA_SCORE_WAR_WIN_BATTLE_COUNT,     //积分赛胜利次数
		SOA_ACADEMIC_TOTAL_GROWTH_VALUE,	//徒弟学业成长值
		SOA_MASTER_DAILYTASK_GROWTH,		//师门日常任务成长总值
		SOA_MASTER_ACADEMICTASK_GROWTH,	    //师门学业成长总值
		SOA_MASTER_UPLEVEL_GROWTH,			//师门升级成长总值
		SOA_MASTER_GIVEEQUIP_GROWTH,		//师门赠送装备成长总值
		SOA_MASTER_GIVEGIFT_GROWTH,			//师门赠送礼包成长总值
        SOA_MASTER_TEAMCLEARDUNGON_GROWTH,  //师徒组队通过地下城成长值
        SOA_MASTER_GOODTEACHER_VALUE,       //良师值
		SOA_ROOM_ID,						//房间ID
		SOA_SHOW_FASHION_WEAPON,			//显示时装武器
		SOA_WEAPON_LEASE_TICKETS,			//武器租赁好运符
		SOA_GAME_SET,						//游戏设置
        SOA_NOVICE_GUIDE_CHOOSE_FLAG, 		//新手引导选择标志
		SOA_ADVENTURE_TEAM_NAME,            //佣兵团名称
		SOA_HEAD_FRAME,						//头像框
		SOA_NEW_TITLE_NAME,					//穿戴头衔名
		SOA_NEW_TITLE_GUID,					//穿戴头衔GUID
        SOA_CHIJI_HP,                       //吃鸡HP
        SOA_CHIJI_MP,                       //吃鸡MP
		SOA_PACKAGE_TYPE,					//包裹类型的大小	
        SOA_GUILD_EMBLEM,					//公会徽记等级
        SOA_OPPO_VIP_LEVEL,                 //oppo vip等级
        SOA_CHIJI_SCORE,                    //吃鸡积分
        SOA_EQUAL_PVP_SKILLS,				//公平竞技场PVP技能列表
	    SOA_EQUAL_PVP_SKILLBAR,				//公平竞技场技能栏
	    SOA_EQUAL_PVP_SP,					//公平竞技场SP	UInt32
		SOA_ACCOUNT_ACHIEVEMENT_SCORE,		//账号成就积分 
        SOA_MALL_POINT,                     //商城积分 UInt32
        SOA_TOTAL_EQUIP_SCORE,              //全身装备评分 UInt32
        SOA_ADVENTURE_COIN,                 //冒险币
        SOA_STORAGE_OPEN_NUM,			    //仓库解锁数量 UInt8
    };


    //武道大会状态
	public enum WudaoStatus
    {
        // 初始状态（未参加）
        WUDAO_STATUS_INIT,
        // 进行中
        WUDAO_STATUS_PLAYING,
        // 已完成
        WUDAO_STATUS_COMPLETE,
    };

    
    // oppo琥珀等级
    public enum OppoAmberLevel
    {
        // 普通用户
        OAL_NONE = 0,
        // 绿珀1星
        OAL_GREEN_ONE,
        // 绿珀2星
        OAL_GREEN_TWO,
        // 绿珀3星
        OAL_GREEN_THREE,
        // 蓝珀1星
        OAL_BLUE_ONE,
        // 蓝珀2星
        OAL_BLUE_TWO,
        // 蓝珀3星
        OAL_BLUE_THREE,
        // 金珀1星
        OAL_GOLD_ONE,
        // 金珀2星
        OAL_GOLD_TWO,
        // 金珀3星
        OAL_GOLD_THREE,
        // 华贵红珀
        OAL_RED,
        // 至尊紫珀
        OAL_PURPLE,
    };
    public class ObjectPos
    {
        public UInt16 x, y;
    }

	public class QQVipInfo
    {
        //标识位
        public byte flag;
        //黄蓝钻等级
        public byte level;
        //3366等级
        public byte lv3366;
    }

	public class CounterInfo
    {
        public string name;
        public UInt32 value;
    }

	public class FatigueInfo
    {
        public UInt16 fatigue;
        public UInt16 maxFatigue;
    }

    public class MaskProperty
    {
        public MaskProperty(UInt32 size)
        {
            maskSize = size;
            byteSize = ((size - 1) / 8 + 1);
            flags = new byte[byteSize];
        }

        public bool CheckMask(UInt32 offset)
        {
            if (offset >= maskSize) return false;
            int index = (int)(offset / 8);
            int bitoffset = (int)(offset % 8);
            return ((flags[index]) & (1 << bitoffset)) != 0;
        }

        public UInt32 maskSize;
        public UInt32 byteSize;
        public byte[] flags;
    }

    public class FuncMaskProperty : MaskProperty
    {
        public FuncMaskProperty() : base(128) { }
    }

    public class BootMaskProperty : MaskProperty
    {
        public BootMaskProperty() : base(128) { }
    }

    public class GuildBattleMaskProperty : MaskProperty
    {
        public GuildBattleMaskProperty() : base(64) { }
    }

    public class DailyTaskMaskProperty : MaskProperty
    {
        public DailyTaskMaskProperty() : base(64) { }
    }

	public class AchievementMaskProperty : MaskProperty
	{
		public AchievementMaskProperty() : base(64) { }
	}

	public class MoneyManageMaskProperty : MaskProperty
	{
		public MoneyManageMaskProperty() : base(64) { }
	}

	public class ScoreWarMaskProperty : MaskProperty
	{
		public ScoreWarMaskProperty() : base(64) { }
	}

	public class SceneObject : StreamObject
    {
        public UInt64 id;
        public byte isNew;
        public UInt32 sceneId;
        public byte type;

        [SProperty((int)SceneObjectAttr.SOA_ZONEID)]
        public UInt32 zoneId;

        [SProperty((int)SceneObjectAttr.SOA_NAME)]
        public string name;

        [SProperty((int)SceneObjectAttr.SOA_LEVEL)]
        public UInt16 level;

		[SProperty((int)SceneObjectAttr.SOA_EXP)]
		public UInt64 exp;

        [SProperty((int)SceneObjectAttr.SOA_SEX)]
        public byte sex;

        [SProperty((int)SceneObjectAttr.SOA_OCCU)]
        public byte occu;

        [SProperty((int)SceneObjectAttr.SOA_QQVIPINFO)]
        public QQVipInfo qqinfo;

        [SProperty((int)SceneObjectAttr.SOA_HP)]
        public UInt32 hp;

        [SProperty((int)SceneObjectAttr.SOA_MAXHP)]
        public UInt32 maxHp;

        [SProperty((int)SceneObjectAttr.SOA_MOVESPEED)]
        public UInt16 moveSpeed;

        [SProperty((int)SceneObjectAttr.SOA_POSITION)]
        public ObjectPos __pos;   // nouse

        [SProperty((int)SceneObjectAttr.SOA_DIR)]
        public UInt16 __dir;

        [SProperty((int)SceneObjectAttr.SOA_SKILLS)]
        public SkillMgr skillMgr;

        [SProperty((int)SceneObjectAttr.SOA_BUFFS)]
        public List<Battle.DungeonBuff> buffList;

		[SProperty((int)SceneObjectAttr.SOA_GOLD)]
		public UInt32 gold;

		[SProperty((int)SceneObjectAttr.SOA_BINDGOLD)]
		public UInt32 bindGold;

        [SProperty((int)SceneObjectAttr.SOA_POINT)]
        public UInt32 point;

        [SProperty((int)SceneObjectAttr.SOA_BINDPOINT)]
        public UInt32 bindPoint;

        [SProperty((int)SceneObjectAttr.SOA_STATUS)]
        public byte state;

		[SProperty((int)SceneObjectAttr.SOA_SKILLBAR)]
		public SkillBars skillBars;

        [SProperty((int)SceneObjectAttr.SOA_SCENE_POS)]
        public ScenePosition pos;

		[SProperty((int)SceneObjectAttr.SOA_SCENE_DIR)]
		public SceneDir dir;

		[SProperty((int)SceneObjectAttr.SOA_SP)]
		public Sp sp;

		[SProperty((int)SceneObjectAttr.SOA_AWAKEN)]
		public byte awaken;

		[SProperty((int)SceneObjectAttr.SOA_HONOR)]
        public UInt32 honor;

        [SProperty((int)SceneObjectAttr.SOA_PACKSIZE)]
        public UInt32 packSize;

        [SProperty((int)SceneObjectAttr.SOA_PKVALUE)]
        public UInt32 pkValue;

		[SProperty((int)SceneObjectAttr.SOA_FATIGUE)]
		public FatigueInfo fatigue;

        [SProperty((int)SceneObjectAttr.SOA_STORAGESIZE)]
        public byte storageSize;

        [SProperty((int)SceneObjectAttr.SOA_TITLE)]
        public UInt32 title;

        [SProperty((int)SceneObjectAttr.SOA_DAYCHARGENUM)]
        public UInt32 dayChargeNum;

        [SProperty((int)SceneObjectAttr.SOA_ITEMCD)]
        public List<ItemCD> itemCd;

        [SProperty((int)SceneObjectAttr.SOA_WARRIOR_SOUL)]
        public UInt32 warriorSoul;

        [SProperty((int)SceneObjectAttr.SOA_MATCH_SCORE)]
        public UInt32 matchScore;

        [SProperty((int)SceneObjectAttr.SOA_WARP_STONE)]
        public List<WarpStoneInfo> warpStone;

        [SProperty((int)SceneObjectAttr.SOA_COUNTER)]
        public Dictionary<string, CounterInfo> counterMgr;

        [SProperty((int)SceneObjectAttr.SOA_FOLLOWPET)]
        public SceneRetinue sceneRetinue;

        [SProperty((int)SceneObjectAttr.SOA_PK_COIN)]
        public UInt32 pkCoin;

        [SProperty((int)SceneObjectAttr.SOA_FUNCNOTIFY)]
        public FuncMaskProperty funcNotify;

        [SProperty((int)SceneObjectAttr.SOA_FUNCFLAG)]
        public FuncMaskProperty funcFlag;

        [SProperty((int)SceneObjectAttr.SOA_AVATAR)]
        public PlayerAvatar avatar;

        [SProperty((int)SceneObjectAttr.SOA_ALIVE_COIN)]
        public UInt32 aliveCoin;

        [SProperty((int)SceneObjectAttr.SOA_VIP)]
        public Vip vip;

        [SProperty((int)SceneObjectAttr.SOA_CREATE_TIME)]
        public UInt32 createTime;

        [SProperty((int)SceneObjectAttr.SOA_NEW_BOOT)]
        public UInt32 newBoot;

        [SProperty((int)SceneObjectAttr.SOA_TOWER_WIPEOUT_END_TIME)]
        public UInt32 deathTowerWipeoutEndTime;

        [SProperty((int)SceneObjectAttr.SOA_GUILD_POST)]
        public byte guildPost;

        [SProperty((int)SceneObjectAttr.SOA_GUILD_CONTRI)]
        public UInt32 guildContri;

        [SProperty((int)SceneObjectAttr.SOA_GUILD_SHOP)]
        public byte guildShopId;

        [SProperty((int)SceneObjectAttr.SOA_REDPOINT)]
        public UInt32 redPoint;

        [SProperty((int)SceneObjectAttr.SOA_GUILD_NAME)]
        public string guildName;

        [SProperty((int)SceneObjectAttr.SOA_GUILD_BATTLE_NUMBER)]
        public UInt32 guildBattleNumber;

        [SProperty((int)SceneObjectAttr.SOA_GUILD_BATTLE_SCORE)]
        public UInt32 guildBattleScore;

        [SProperty((int)SceneObjectAttr.SOA_GUILD_BATTLE_REWARD)]
        public GuildBattleMaskProperty guildBattleMask;

        [SProperty((int)SceneObjectAttr.SOA_DAILY_TASK_SOURCE)]
        public UInt32 datilyTaskScore;

        [SProperty((int)SceneObjectAttr.SOA_DAILY_TASK_REWARD_MASK)]
        public DailyTaskMaskProperty dailyTaskMask;

        [SProperty((int)SceneObjectAttr.SOA_WUDAO_STATUS)]
        public byte wudaoStatus;

        [SProperty((int)SceneObjectAttr.SOA_MONTH_CARD_EXPIRE_TIME)]
        public uint monthCardExpireTime;

        [SProperty((int)SceneObjectAttr.SOA_CUSTOM_DATA)]
        public uint customData;

        [SProperty((int)SceneObjectAttr.SOA_SEASON_LEVEL)]
        public uint seasonLevel;

        [SProperty((int)SceneObjectAttr.SOA_SEASON_STAR)]
        public uint seasonStar;

        [SProperty((int)SceneObjectAttr.SOA_SEASON_UPLEVEL_RECORD)]
        public List<byte> seasonUplevelRecord;

        [SProperty((int)SceneObjectAttr.SOA_SEASON_ATTR)]
        public byte seasonAttr;

        [SProperty((int)SceneObjectAttr.SOA_SEASON_KING_MARK)]
        public UInt16 seasonKingMarkCount;
        [SProperty((int)SceneObjectAttr.SOA_AUCTION_REFRESH_TIME)]
        public UInt32 auctionRefreshTime;

        [SProperty((int)SceneObjectAttr.SOA_AUCTION_ADDITION_BOOTH)]
        public UInt32 auctionAdditionBooth;

        [SProperty((int)SceneObjectAttr.SOA_BOOT_FLAG)]
        public BootMaskProperty bootFlag;

        [SProperty((int)SceneObjectAttr.SOA_GOLDJAR_POINT)]
        public UInt32 goldJarPoint;

        [SProperty((int)SceneObjectAttr.SOA_MAGJAR_POINT)]
        public UInt32 magJarPoint;

        [SProperty((int)SceneObjectAttr.SOA_SEASON_EXP)]
        public UInt32 seasonExp;

        [SProperty((int)SceneObjectAttr.SOA_PET_FOLLOW)]
        public UInt32 followPetDataId;

		[SProperty((int)SceneObjectAttr.SOA_POTION_SET)]
        public List<UInt32> potionSets;

		[SProperty((int)SceneObjectAttr.SOA_PRE_OCCU)]
		public byte preOccu;

		[SProperty((int)SceneObjectAttr.SOA_GUILD_BATTLE_LOTTERY_STATUS)]
		public byte guildBattleLotteryStatus;

		[SProperty((int)SceneObjectAttr.SOA_PVP_SKILLS)]
        public SkillMgr pvpSkillMgr;

        [SProperty((int)SceneObjectAttr.SOA_PVP_SKILLBAR)]
		public SkillBars pvpSkillBars;

		[SProperty((int)SceneObjectAttr.SOA_PVP_SP)]
		public Sp pvpSp;

		[SProperty((int)SceneObjectAttr.SOA_EQUAL_PVP_SKILLS)]
		public SkillMgr equalPvpSkillMgr;

		[SProperty((int)SceneObjectAttr.SOA_EQUAL_PVP_SKILLBAR)]
		public SkillBars equalPvpSkillBars;

		[SProperty((int)SceneObjectAttr.SOA_EQUAL_PVP_SP)]
		public UInt32 equalPvpSp;

		[SProperty((int)SceneObjectAttr.SOA_ACHIEVEMENT_SCORE)]
		public UInt32 achievementScore;

		[SProperty((int)SceneObjectAttr.SOA_ACHIEVEMENT_TASK_REWARD_MASK)]
		public AchievementMaskProperty achievementMask;

		[SProperty((int)SceneObjectAttr.SOA_WEAPON_BAR)]
		public List<UInt64> weaponBar;

		[SProperty((int)SceneObjectAttr.SOA_PACKAGE_SIZE_ADDITION)]
		public List<UInt32> packSizeAddition;
		
        [SProperty((int)SceneObjectAttr.SOA_APPOINTMENT_OCCU)]
        public byte appointmentOccu;

		

		[SProperty((int)SceneObjectAttr.SOA_MONEY_MANAGE_STATUS)]
		public byte moneyManageStatus;

		[SProperty((int)SceneObjectAttr.SOA_MONEY_MANAGE_REWARD_MASK)]
		public MoneyManageMaskProperty moneyManageRewardMask;

		[SProperty((int)SceneObjectAttr.SOA_SCORE_WAR_SCORE)]
		public UInt32 scoreWarScore;

		[SProperty((int)SceneObjectAttr.SOA_SCORE_WAR_BATTLE_COUNT)]
		public byte scoreWarBattleCount;

		[SProperty((int)SceneObjectAttr.SOA_SCORE_WAR_REWARD_MASK)]
		public ScoreWarMaskProperty scoreWarRewardMask;

		[SProperty((int)SceneObjectAttr.SOA_SCORE_WAR_WIN_BATTLE_COUNT)]
		public byte scoreWarWinBattleCount;

		[SProperty((int)SceneObjectAttr.SOA_ACADEMIC_TOTAL_GROWTH_VALUE)]
        public UInt32 academicTotalGrowthValue;
		
		[SProperty((int)SceneObjectAttr.SOA_MASTER_DAILYTASK_GROWTH)]
        public UInt32 masterDailyTaskGrowth;
		
		[SProperty((int)SceneObjectAttr.SOA_MASTER_ACADEMICTASK_GROWTH)]
        public UInt32 masterAcademicTaskGrowth;
		
		[SProperty((int)SceneObjectAttr.SOA_MASTER_UPLEVEL_GROWTH)]
        public UInt32 masterUplevelGrowth;
		
		[SProperty((int)SceneObjectAttr.SOA_MASTER_GIVEEQUIP_GROWTH)]
        public UInt32 masterGiveEquipGrowth;
		
		[SProperty((int)SceneObjectAttr.SOA_MASTER_GIVEGIFT_GROWTH)]
        public UInt32 masterGiveGiftGrowth;

        [SProperty((int)SceneObjectAttr.SOA_MASTER_TEAMCLEARDUNGON_GROWTH)]
        public UInt32 masterTeamClearDungeonGrowth;

        [SProperty((int)SceneObjectAttr.SOA_MASTER_GOODTEACHER_VALUE)]
        public UInt32 goodTeacherValue;

		[SProperty((int)SceneObjectAttr.SOA_SHOW_FASHION_WEAPON)]
        public byte showFashionWeapon;
		
		[SProperty((int)SceneObjectAttr.SOA_WEAPON_LEASE_TICKETS)]
		 public UInt32 weaponLeaseTickets;

        [SProperty((int)SceneObjectAttr.SOA_GAME_SET)]
        public string gameSets;

        [SProperty((int)SceneObjectAttr.SOA_NOVICE_GUIDE_CHOOSE_FLAG)]
        public byte noviceGuideChooseFlag;

        [SProperty((int)SceneObjectAttr.SOA_ADVENTURE_TEAM_NAME)]
        public string adventureTeamName;
		
		[SProperty((int)SceneObjectAttr.SOA_HEAD_FRAME)]
        public UInt32 headFrame;
		
		[SProperty((int)SceneObjectAttr.SOA_NEW_TITLE_NAME)]
        public PlayerWearedTitleInfo wearedTitleInfo;
		
		[SProperty((int)SceneObjectAttr.SOA_NEW_TITLE_GUID)]
		public UInt64 newTitleGuid;

        [SProperty((int)SceneObjectAttr.SOA_CHIJI_HP)]
        public Int32 chijiHp;

        [SProperty((int)SceneObjectAttr.SOA_CHIJI_MP)]
        public Int32 chijiMp;

		[SProperty((int)SceneObjectAttr.SOA_PACKAGE_TYPE)]
        public string packageTypeStr;

        [SProperty((int)SceneObjectAttr.SOA_GUILD_EMBLEM)]
        public UInt32 guildEmblemLvl;
        
        [SProperty((int)SceneObjectAttr.SOA_OPPO_VIP_LEVEL)]
        public byte oppoVipLevel;

        [SProperty((int)SceneObjectAttr.SOA_CHIJI_SCORE)]
        public UInt32 chijiScore;
		
		[SProperty((int)SceneObjectAttr.SOA_ACCOUNT_ACHIEVEMENT_SCORE)]
		public UInt32 accountAchievementScore;

        [SProperty((int)SceneObjectAttr.SOA_MALL_POINT)]
        public UInt32 mallPoint;

        [SProperty((int)SceneObjectAttr.SOA_TOTAL_EQUIP_SCORE)]
        public UInt32 equipScore;

        [SProperty((int)SceneObjectAttr.SOA_ADVENTURE_COIN)]
        public UInt32 adventureCoin;
        
        [SProperty((int)SceneObjectAttr.SOA_STORAGE_OPEN_NUM)]
        public byte storageOpenNum; 


        [SProperty((int)SceneObjectAttr.SOA_GAMEOPTIONS)]
        public UInt32 gameOptions;

    }

	public class SceneObjectDecoder : StreamObjectDecoder<SceneObject>
    {
        public static Dictionary<ulong, SceneObject> DecodeSyncSceneObjectMsg(byte[] buffer, ref int pos, int length)
        {
            Dictionary<ulong, SceneObject> sceneObjs = new Dictionary<ulong, SceneObject>();

            byte append = 0;
            BaseDLL.decode_int8(buffer, ref pos, ref append);
            UInt32 sceneId = 0;
            BaseDLL.decode_uint32(buffer, ref pos, ref sceneId);

            while (length - pos > 8)
            {
                SceneObject sceneObj = new SceneObject();
                sceneObj.sceneId = sceneId;
                if(!DecodeSceneObject(ref sceneObj, buffer, ref pos, length))
                {
                    Logger.LogErrorFormat("decode syn scene object msg failed");
                    return null;
                }

                sceneObjs.Add(sceneObj.id,sceneObj);
            }

            return sceneObjs;
        }

        public static bool DecodeSyncSelfObject(ref SceneObject self, byte[] buffer, ref int pos, int length)
        {
            return DecodePropertys(ref self, buffer, ref pos, length);
        }

        public static bool DecodeSceneObject(ref SceneObject sceneObj, byte[] buffer, ref int pos, int length)
        {
            InitFieldDict();

            // 8字节的id
            if(length - pos < 9)
            {
                return false;
            }

            BaseDLL.decode_uint64(buffer, ref pos, ref sceneObj.id);
            BaseDLL.decode_int8(buffer, ref pos, ref sceneObj.type);

            return DecodePropertys(ref sceneObj, buffer, ref pos, length);
        }

        public static SceneObject DecodeSelfSceneObject(byte[] buffer, ref int pos, int length)
        {
            SceneObject sceneObj = new SceneObject();
            DecodePropertys(ref sceneObj, buffer, ref pos, length);
            return sceneObj;
        }
    }
}
