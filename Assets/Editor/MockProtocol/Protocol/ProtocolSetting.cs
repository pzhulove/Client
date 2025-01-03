using System;
using System.Text;

namespace Mock.Protocol
{

	public enum ServiceType
	{
		/// <summary>
		///  无效值
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效值", " 无效值")]
		SERVICE_INVALID = 0,
		/// <summary>
		///  赛季匹配
		/// </summary>
		[AdvancedInspector.Descriptor(" 赛季匹配", " 赛季匹配")]
		SERVICE_1V1_SEASON = 1,
		/// <summary>
		///  工会战报名
		/// </summary>
		[AdvancedInspector.Descriptor(" 工会战报名", " 工会战报名")]
		SERVICE_GUILD_BATTLE_ENROLL = 2,
		/// <summary>
		///  工会战
		/// </summary>
		[AdvancedInspector.Descriptor(" 工会战", " 工会战")]
		SERVICE_GUILD_BATTLE = 3,
		/// <summary>
		///  自动战斗
		/// </summary>
		[AdvancedInspector.Descriptor(" 自动战斗", " 自动战斗")]
		SERVICE_AUTO_BATTLE = 4,
		/// <summary>
		///  场景等级限制
		/// </summary>
		[AdvancedInspector.Descriptor(" 场景等级限制", " 场景等级限制")]
		SERVICE_SCENE_LEVEL_LIMIT = 5,
		/// <summary>
		///  装备品级调整
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备品级调整", " 装备品级调整")]
		EQUIP_SET_QUALITY_LV = 6,
		/// <summary>
		///  时装选属性
		/// </summary>
		[AdvancedInspector.Descriptor(" 时装选属性", " 时装选属性")]
		FASHION_SEL_ATTR = 7,
		/// <summary>
		///  金罐积分
		/// </summary>
		[AdvancedInspector.Descriptor(" 金罐积分", " 金罐积分")]
		GOLD_JAR_POINT = 8,
		/// <summary>
		///  魔罐积分
		/// </summary>
		[AdvancedInspector.Descriptor(" 魔罐积分", " 魔罐积分")]
		MAGIC_JAR_POINT = 9,
		/// <summary>
		///  圆桌会议
		/// </summary>
		[AdvancedInspector.Descriptor(" 圆桌会议", " 圆桌会议")]
		SERVICE_GUILD_TABLE = 20,
		/// <summary>
		///  公会技能
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会技能", " 公会技能")]
		SERVICE_GUILD_SKILL = 21,
		/// <summary>
		///  公会捐赠
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会捐赠", " 公会捐赠")]
		SERVICE_GUILD_DONATE = 22,
		/// <summary>
		///  公会膜拜
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会膜拜", " 公会膜拜")]
		SERVICE_GUILD_ORZ = 23,
		/// <summary>
		///  公会红包
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会红包", " 公会红包")]
		SERVICE_GUILD_RED_PACKET = 24,
		/// <summary>
		///  跨服公会战
		/// </summary>
		[AdvancedInspector.Descriptor(" 跨服公会战", " 跨服公会战")]
		SERVICE_GUILD_CROSS_BATTLE = 25,
		/// <summary>
		/// SDK小米切支付开关
		/// </summary>
		[AdvancedInspector.Descriptor("SDK小米切支付开关", "SDK小米切支付开关")]
		SERVICE_SDK_XIAOMI_CHANGE_CHARGE = 30,
		/// <summary>
		/// 帐号转移公告截图功能开关
		/// </summary>
		[AdvancedInspector.Descriptor("帐号转移公告截图功能开关", "帐号转移公告截图功能开关")]
		SERVICE_CONVERT_ACC_SCREENSHOT = 31,
		/// <summary>
		/// 货币重置快捷提示客户端检查开关
		/// </summary>
		[AdvancedInspector.Descriptor("货币重置快捷提示客户端检查开关", "货币重置快捷提示客户端检查开关")]
		SERVICE_CURRENCY_DEADLINE_CHECK = 33,
		/// <summary>
		///  随从系统
		/// </summary>
		[AdvancedInspector.Descriptor(" 随从系统", " 随从系统")]
		SERVICE_RETINUE = 40,
		/// <summary>
		///  次元石系统
		/// </summary>
		[AdvancedInspector.Descriptor(" 次元石系统", " 次元石系统")]
		SERVICE_WARP_STONE = 60,
		/// <summary>
		///  房间
		/// </summary>
		[AdvancedInspector.Descriptor(" 房间", " 房间")]
		SERVICE_ROOM = 70,
		/// <summary>
		///  语音
		/// </summary>
		[AdvancedInspector.Descriptor(" 语音", " 语音")]
		SERVICE_VOICE_NORMAL = 80,
		/// <summary>
		///  实时语音
		/// </summary>
		[AdvancedInspector.Descriptor(" 实时语音", " 实时语音")]
		SERVICE_VOICE_REAL_TIME = 81,
		/// <summary>
		///  vip认证
		/// </summary>
		[AdvancedInspector.Descriptor(" vip认证", " vip认证")]
		SERVICE_VIP_AUTH = 82,
		/// <summary>
		///  在线客服
		/// </summary>
		[AdvancedInspector.Descriptor(" 在线客服", " 在线客服")]
		SERVICE_ONLINE_SERVICE = 83,
		/// <summary>
		///  每日充值福利
		/// </summary>
		[AdvancedInspector.Descriptor(" 每日充值福利", " 每日充值福利")]
		SERVEICE_DAY_CHARGE_WELFARE = 91,
		/// <summary>
		/// 追帧模式开启
		/// </summary>
		[AdvancedInspector.Descriptor("追帧模式开启", "追帧模式开启")]
		SERVICE_CHASING_MODE = 92,
		/// <summary>
		///  随机宝箱
		/// </summary>
		[AdvancedInspector.Descriptor(" 随机宝箱", " 随机宝箱")]
		SERVICE_DIG = 100,
		/// <summary>
		///  安全锁
		/// </summary>
		[AdvancedInspector.Descriptor(" 安全锁", " 安全锁")]
		SERVICE_SECURITY_LOCK = 111,
		/// <summary>
		///  3v3乱斗
		/// </summary>
		[AdvancedInspector.Descriptor(" 3v3乱斗", " 3v3乱斗")]
		SERVICE_3v3_TUMBLE = 112,
		/// <summary>
		///  OPPO社区
		/// </summary>
		[AdvancedInspector.Descriptor(" OPPO社区", " OPPO社区")]
		SERVICE_OPPO_COMMUNITY = 120,
		/// <summary>
		///  VIVO社区
		/// </summary>
		[AdvancedInspector.Descriptor(" VIVO社区", " VIVO社区")]
		SERVICE_VIVO_COMMUNITY = 121,
		/// <summary>
		///  强化券
		/// </summary>
		[AdvancedInspector.Descriptor(" 强化券", " 强化券")]
		SERVICE_STRENGTHEN_TICKET_MERGE = 130,
		/// <summary>
		/// 公会副本
		/// </summary>
		[AdvancedInspector.Descriptor("公会副本", "公会副本")]
		SERVICE_GUILD_DUNGEON = 145,
		/// <summary>
		/// 时装合成
		/// </summary>
		[AdvancedInspector.Descriptor("时装合成", "时装合成")]
		SERVICE_FASHION_MERGO = 150,
		/// <summary>
		///  拍卖行珍品开关
		/// </summary>
		[AdvancedInspector.Descriptor(" 拍卖行珍品开关", " 拍卖行珍品开关")]
		SERVICE_AUCTION_TREAS = 210,
		/// <summary>
		///  拍卖行翻页开关
		/// </summary>
		[AdvancedInspector.Descriptor(" 拍卖行翻页开关", " 拍卖行翻页开关")]
		SERVICE_AUCTION_PAGE = 211,
		/// <summary>
		///  拍卖行交易冷却时间开关
		/// </summary>
		[AdvancedInspector.Descriptor(" 拍卖行交易冷却时间开关", " 拍卖行交易冷却时间开关")]
		SERVICE_AUCTION_COOLTIME = 212,
		/// <summary>
		/// 冒险队(佣兵团)
		/// </summary>
		[AdvancedInspector.Descriptor("冒险队(佣兵团)", "冒险队(佣兵团)")]
		SERVICE_ADVENTURE_TEAM = 215,
		/// <summary>
		/// 新回归活动
		/// </summary>
		[AdvancedInspector.Descriptor("新回归活动", "新回归活动")]
		SERVICE_NEW_RETURN = 216,
		/// <summary>
		/// 冒险队(佣兵团)排行榜
		/// </summary>
		[AdvancedInspector.Descriptor("冒险队(佣兵团)排行榜", "冒险队(佣兵团)排行榜")]
		SERVICE_ADVENTURE_TEAM_SORTLIST = 217,
		/// <summary>
		/// 周常深渊失败门票返还
		/// </summary>
		[AdvancedInspector.Descriptor("周常深渊失败门票返还", "周常深渊失败门票返还")]
		SERVICE_WEEK_HELL_FAIL_RETURN_TICKETS = 221,
		/// <summary>
		/// 公会副本奖励截图
		/// </summary>
		[AdvancedInspector.Descriptor("公会副本奖励截图", "公会副本奖励截图")]
		SERVICE_GUILD_DUNGEON_SCREEN_SHOT = 224,
		/// <summary>
		/// 单局结算ID判断
		/// </summary>
		[AdvancedInspector.Descriptor("单局结算ID判断", "单局结算ID判断")]
		SERVICE_RACE_ID_CHECK = 225,
		/// <summary>
		/// 每日必做
		/// </summary>
		[AdvancedInspector.Descriptor("每日必做", "每日必做")]
		SERVICE_DAILY_TODO = 226,
		/// <summary>
		/// 当删除账号玩家时检查玩家ID
		/// </summary>
		[AdvancedInspector.Descriptor("当删除账号玩家时检查玩家ID", "当删除账号玩家时检查玩家ID")]
		SERVICE_CHECK_ROLEID_WHEN_REMOVE_ACCOUNT_PLAYER = 227,
		/// <summary>
		/// 装备强化保护开关
		/// </summary>
		[AdvancedInspector.Descriptor("装备强化保护开关", "装备强化保护开关")]
		SERVICE_EQUIP_STRENGTHEN_PROTECT = 228,
		/// <summary>
		/// 装备强化装备分解失败移除装备开关
		/// </summary>
		[AdvancedInspector.Descriptor("装备强化装备分解失败移除装备开关", "装备强化装备分解失败移除装备开关")]
		SERVICE_EQUIP_STRENG_DESC_FAIL_REMOVE = 229,
		/// <summary>
		/// 辅助装备开关
		/// </summary>
		[AdvancedInspector.Descriptor("辅助装备开关", "辅助装备开关")]
		SERVICE_ASSIST_EQUIP = 230,
		/// <summary>
		/// 公会合并开关
		/// </summary>
		[AdvancedInspector.Descriptor("公会合并开关", "公会合并开关")]
		SERVICR_GUILDMERGER = 231,
		/// <summary>
		/// 终极试炼爬塔
		/// </summary>
		[AdvancedInspector.Descriptor("终极试炼爬塔", "终极试炼爬塔")]
		SERVICE_ZJSL_TOWER = 232,
		/// <summary>
		/// 团本
		/// </summary>
		[AdvancedInspector.Descriptor("团本", "团本")]
		SERVICE_TEAM_COPY = 233,
		/// <summary>
		///  飞升药接受活动同步开关
		/// </summary>
		[AdvancedInspector.Descriptor(" 飞升药接受活动同步开关", " 飞升药接受活动同步开关")]
		SERVICE_FLY_UP = 234,
		/// <summary>
		///  日志埋点通过游戏服发送
		/// </summary>
		[AdvancedInspector.Descriptor(" 日志埋点通过游戏服发送", " 日志埋点通过游戏服发送")]
		SERVICE_NEW_CLIENT_LOG = 235,
		/// <summary>
		///  新的结算面板
		/// </summary>
		[AdvancedInspector.Descriptor(" 新的结算面板", " 新的结算面板")]
		SERVICE_NEW_RACE_END_EXP = 236,
		/// <summary>
		/// 技能页开关
		/// </summary>
		[AdvancedInspector.Descriptor("技能页开关", "技能页开关")]
		SERVICE_SKILL_PAGE = 237,
		/// <summary>
		/// 装备方案开关
		/// </summary>
		[AdvancedInspector.Descriptor("装备方案开关", "装备方案开关")]
		SERVICE_EQUIP_SCHEME = 238,
		/// <summary>
		/// 金币寄售
		/// </summary>
		[AdvancedInspector.Descriptor("金币寄售", "金币寄售")]
		SERVICE_GOLD_CONSIGNMENT = 239,
	}


	public enum GameSetType
	{
		/// <summary>
		///  好友邀请
		/// </summary>
		[AdvancedInspector.Descriptor(" 好友邀请", " 好友邀请")]
		GST_FRIEND_INVATE = 1,
		/// <summary>
		///  隐私设置
		/// </summary>
		[AdvancedInspector.Descriptor(" 隐私设置", " 隐私设置")]
		GST_SECRET = 2,
	}


	public enum SecretSetType
	{
		/// <summary>
		///  公会邀请
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会邀请", " 公会邀请")]
		SST_GUILD_INVATE = 1,
		/// <summary>
		///  组队邀请
		/// </summary>
		[AdvancedInspector.Descriptor(" 组队邀请", " 组队邀请")]
		SST_TEAM_INVATE = 2,
		/// <summary>
		///  决斗邀请
		/// </summary>
		[AdvancedInspector.Descriptor(" 决斗邀请", " 决斗邀请")]
		SST_DUEL_INVATE = 4,
	}

	/// <summary>
	///  保存选项
	/// </summary>
	[AdvancedInspector.Descriptor(" 保存选项", " 保存选项")]
	public enum SaveOptionMask
	{
		/// <summary>
		///  是否不消耗精力(精英地下城)
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否不消耗精力(精英地下城)", " 是否不消耗精力(精英地下城)")]
		SOM_NOT_COUSUME_EBERGY = 1,
	}

}
