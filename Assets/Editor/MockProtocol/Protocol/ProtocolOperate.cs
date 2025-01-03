using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  神器罐子折扣抽取状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 神器罐子折扣抽取状态", " 神器罐子折扣抽取状态")]
	public enum ArtifactJarDiscountExtractStatus
	{
		/// <summary>
		///  不可抽取
		/// </summary>
		[AdvancedInspector.Descriptor(" 不可抽取", " 不可抽取")]
		AJDES_INVALID = 0,
		/// <summary>
		///  可抽取
		/// </summary>
		[AdvancedInspector.Descriptor(" 可抽取", " 可抽取")]
		AJDES_IN = 1,
		/// <summary>
		///  已抽取
		/// </summary>
		[AdvancedInspector.Descriptor(" 已抽取", " 已抽取")]
		AJDES_OVER = 2,
	}


	public enum OpActivityTmpType
	{

		OAT_NONE = 0,
		/// <summary>
		/// 每日单笔充值
		/// </summary>
		[AdvancedInspector.Descriptor("每日单笔充值", "每日单笔充值")]
		OAT_DAY_SINGLE_CHARGE = 1,
		/// <summary>
		/// 每日累计充值
		/// </summary>
		[AdvancedInspector.Descriptor("每日累计充值", "每日累计充值")]
		OAT_DAY_TATOL_CHARGE = 2,
		/// <summary>
		/// 累计充值
		/// </summary>
		[AdvancedInspector.Descriptor("累计充值", "累计充值")]
		OAT_TATOL_CHARGE = 3,
		/// <summary>
		/// 单笔充值	
		/// </summary>
		[AdvancedInspector.Descriptor("单笔充值	", "单笔充值	")]
		OAT_SINGLE_CHARGE = 4,
		/// <summary>
		/// 连续充值
		/// </summary>
		[AdvancedInspector.Descriptor("连续充值", "连续充值")]
		OAT_COMBO_CHARGE = 5,
		/// <summary>
		/// 每日累计消耗道具
		/// </summary>
		[AdvancedInspector.Descriptor("每日累计消耗道具", "每日累计消耗道具")]
		OAT_DAY_COST_ITEM = 6,
		/// <summary>
		/// 累计消耗道具
		/// </summary>
		[AdvancedInspector.Descriptor("累计消耗道具", "累计消耗道具")]
		OAT_COST_ITEM = 7,
		/// <summary>
		/// 每日购买指定商城礼包
		/// </summary>
		[AdvancedInspector.Descriptor("每日购买指定商城礼包", "每日购买指定商城礼包")]
		OAT_DAY_BUY_GIFTPACK = 8,
		/// <summary>
		/// 购买指定商城礼包
		/// </summary>
		[AdvancedInspector.Descriptor("购买指定商城礼包", "购买指定商城礼包")]
		OAT_BUY_GIFTPACK = 9,
		/// <summary>
		/// 每日登陆
		/// </summary>
		[AdvancedInspector.Descriptor("每日登陆", "每日登陆")]
		OAT_DAY_LOGIN = 10,
		/// <summary>
		/// 累计登陆天数
		/// </summary>
		[AdvancedInspector.Descriptor("累计登陆天数", "累计登陆天数")]
		OAT_LOGIN_DAYNUM = 11,
		/// <summary>
		/// 每日累计在线
		/// </summary>
		[AdvancedInspector.Descriptor("每日累计在线", "每日累计在线")]
		OAT_DAY_ONLINE_TIME = 12,
		/// <summary>
		/// 总累计在线
		/// </summary>
		[AdvancedInspector.Descriptor("总累计在线", "总累计在线")]
		OAT_ONLINE_TIME = 13,
		/// <summary>
		/// 每日累计完成关卡
		/// </summary>
		[AdvancedInspector.Descriptor("每日累计完成关卡", "每日累计完成关卡")]
		OAT_DAY_COMPLETE_DUNG = 14,
		/// <summary>
		/// 累计完成关卡
		/// </summary>
		[AdvancedInspector.Descriptor("累计完成关卡", "累计完成关卡")]
		OAT_COMPLETE_DUNG = 15,
		/// <summary>
		/// 手机绑定
		/// </summary>
		[AdvancedInspector.Descriptor("手机绑定", "手机绑定")]
		OAT_BIND_PHONE = 16,
		/// <summary>
		/// 时装
		/// </summary>
		[AdvancedInspector.Descriptor("时装", "时装")]
		OAT_BUY_FASHION = 17,
		/// <summary>
		/// 老的新服冲击赛
		/// </summary>
		[AdvancedInspector.Descriptor("老的新服冲击赛", "老的新服冲击赛")]
		OAT_LEVEL_FIGHTING = 18,
		/// <summary>
		/// 新服商城时装打折
		/// </summary>
		[AdvancedInspector.Descriptor("新服商城时装打折", "新服商城时装打折")]
		OAT_MALL_DISCOUNT_FOR_NEW_SERVER = 1000,
		/// <summary>
		/// 新服冲级赛竞争阶段
		/// </summary>
		[AdvancedInspector.Descriptor("新服冲级赛竞争阶段", "新服冲级赛竞争阶段")]
		OAT_LEVEL_FIGHTING_FOR_NEW_SERVER = 1001,
		/// <summary>
		/// 新服冲级赛公示阶段
		/// </summary>
		[AdvancedInspector.Descriptor("新服冲级赛公示阶段", "新服冲级赛公示阶段")]
		OAT_LEVEL_SHOW_FOR_NEW_SERVER = 1002,
		/// <summary>
		///  地下城掉落活动
		/// </summary>
		[AdvancedInspector.Descriptor(" 地下城掉落活动", " 地下城掉落活动")]
		OAT_DUNGEON_DROP_ACTIVITY = 1100,
		/// <summary>
		///  地下城结算经验加成
		/// </summary>
		[AdvancedInspector.Descriptor(" 地下城结算经验加成", " 地下城结算经验加成")]
		OAT_DUNGEON_EXP_ADDITION = 1200,
		/// <summary>
		///  决斗币奖励
		/// </summary>
		[AdvancedInspector.Descriptor(" 决斗币奖励", " 决斗币奖励")]
		OAT_PVP_PK_COIN = 1300,
		/// <summary>
		///  预约职业活动
		/// </summary>
		[AdvancedInspector.Descriptor(" 预约职业活动", " 预约职业活动")]
		OAT_APPOINTMENT_OCCU = 1400,
		/// <summary>
		///  深渊票消耗得抽奖
		/// </summary>
		[AdvancedInspector.Descriptor(" 深渊票消耗得抽奖", " 深渊票消耗得抽奖")]
		OAT_HELL_TICKET_FOR_DRAW_PRIZE = 1500,
		/// <summary>
		///  疲劳消耗得BUFF
		/// </summary>
		[AdvancedInspector.Descriptor(" 疲劳消耗得BUFF", " 疲劳消耗得BUFF")]
		OAT_FATIGUE_FOR_BUFF = 1600,
		/// <summary>
		///  疲劳消耗得代币
		/// </summary>
		[AdvancedInspector.Descriptor(" 疲劳消耗得代币", " 疲劳消耗得代币")]
		OAT_FATIGUE_FOR_TOKEN_COIN = 1700,
		/// <summary>
		///  疲劳燃烧
		/// </summary>
		[AdvancedInspector.Descriptor(" 疲劳燃烧", " 疲劳燃烧")]
		OAT_FATIGUE_BURNING = 1800,
		/// <summary>
		///  夺宝活动
		/// </summary>
		[AdvancedInspector.Descriptor(" 夺宝活动", " 夺宝活动")]
		OAT_GAMBING = 1900,
		/// <summary>
		///  每日奖励
		/// </summary>
		[AdvancedInspector.Descriptor(" 每日奖励", " 每日奖励")]
		OAT_DAILY_REWARD = 2000,
		/// <summary>
		///  七夕鹊桥
		/// </summary>
		[AdvancedInspector.Descriptor(" 七夕鹊桥", " 七夕鹊桥")]
		OAT_MAGPIE_BRIDGE = 2100,
		/// <summary>
		///  月卡活动
		/// </summary>
		[AdvancedInspector.Descriptor(" 月卡活动", " 月卡活动")]
		OAT_MONTH_CARD = 2200,
		/// <summary>
		///  限时礼包
		/// </summary>
		[AdvancedInspector.Descriptor(" 限时礼包", " 限时礼包")]
		OAT_LIMIT_TIME_GIFT_PACK = 5000,
		/// <summary>
		///  赌马
		/// </summary>
		[AdvancedInspector.Descriptor(" 赌马", " 赌马")]
		OAT_BET_HORSE = 5100,
		/// <summary>
		///  buff加成活动
		/// </summary>
		[AdvancedInspector.Descriptor(" buff加成活动", " buff加成活动")]
		OAT_BUFF_ADDITION = 2300,
		/// <summary>
		///  地下城掉落倍率增加
		/// </summary>
		[AdvancedInspector.Descriptor(" 地下城掉落倍率增加", " 地下城掉落倍率增加")]
		OAT_DUNGEON_DROP_RATE_ADDITION = 2400,
		/// <summary>
		///  百变换装
		/// </summary>
		[AdvancedInspector.Descriptor(" 百变换装", " 百变换装")]
		OAT_CHANGE_FASHION_MERGE = 2500,
		/// <summary>
		///  绝版兑换
		/// </summary>
		[AdvancedInspector.Descriptor(" 绝版兑换", " 绝版兑换")]
		OAT_CHANGE_FASHION_EXCHANGE = 2600,
		/// <summary>
		///  换装福利
		/// </summary>
		[AdvancedInspector.Descriptor(" 换装福利", " 换装福利")]
		OAT_CHANGE_FASHION_WELFARE = 2700,
		/// <summary>
		///  地下城随机buff活动
		/// </summary>
		[AdvancedInspector.Descriptor(" 地下城随机buff活动", " 地下城随机buff活动")]
		OAT_DUNGEON_RANDOM_BUFF = 2800,
		/// <summary>
		///  地下城通关得奖励
		/// </summary>
		[AdvancedInspector.Descriptor(" 地下城通关得奖励", " 地下城通关得奖励")]
		OAT_DUNGEON_CLEAR_GET_REWARD = 2900,
		/// <summary>
		///  换装卷购买
		/// </summary>
		[AdvancedInspector.Descriptor(" 换装卷购买", " 换装卷购买")]
		OAT_CHANGE_FASHION_PURCHASE = 3000,
		/// <summary>
		///  换装节活动
		/// </summary>
		[AdvancedInspector.Descriptor(" 换装节活动", " 换装节活动")]
		OAT_CHANGE_FASHION_ACT = 3300,
		/// <summary>
		///  每日buff活动
		/// </summary>
		[AdvancedInspector.Descriptor(" 每日buff活动", " 每日buff活动")]
		OAT_DAILY_BUFF = 3100,
		/// <summary>
		///  强化券合成活动
		/// </summary>
		[AdvancedInspector.Descriptor(" 强化券合成活动", " 强化券合成活动")]
		OAT_STRENGTHEN_TICKET_SYNTHESIS = 3200,
		/// <summary>
		///  商城购买获得活动
		/// </summary>
		[AdvancedInspector.Descriptor(" 商城购买获得活动", " 商城购买获得活动")]
		OAT_MALL_BUY_GOT = 3400,
		/// <summary>
		///  神器罐子活动
		/// </summary>
		[AdvancedInspector.Descriptor(" 神器罐子活动", " 神器罐子活动")]
		OAT_ARTIFACT_JAR = 3500,
		/// <summary>
		///  罐子抽奖活动
		/// </summary>
		[AdvancedInspector.Descriptor(" 罐子抽奖活动", " 罐子抽奖活动")]
		OAT_JAR_DRAW_LOTTERY = 3600,
		/// <summary>
		///  限时深渊活动
		/// </summary>
		[AdvancedInspector.Descriptor(" 限时深渊活动", " 限时深渊活动")]
		OAT_LIMIT_TIME_HELL = 3700,
		/// <summary>
		///  绑金商店活动
		/// </summary>
		[AdvancedInspector.Descriptor(" 绑金商店活动", " 绑金商店活动")]
		OAT_MALL_BINDINGGOLD = 3900,
		/// <summary>
		///  黑市商人活动
		/// </summary>
		[AdvancedInspector.Descriptor(" 黑市商人活动", " 黑市商人活动")]
		OAT_BLACK_MARKET_SHOP = 4000,
		/// <summary>
		///  周常深渊
		/// </summary>
		[AdvancedInspector.Descriptor(" 周常深渊", " 周常深渊")]
		OAT_WEEK_DEEP = 4600,
		/// <summary>
		///  购买赠送
		/// </summary>
		[AdvancedInspector.Descriptor(" 购买赠送", " 购买赠送")]
		OAT_BUY_PRRSENT = 4700,
		/// <summary>
		///  累计登录领奖
		/// </summary>
		[AdvancedInspector.Descriptor(" 累计登录领奖", " 累计登录领奖")]
		OAT_CUMULATE_LOGIN_REWARD = 4800,
		/// <summary>
		///  累计通关地下城天数领奖
		/// </summary>
		[AdvancedInspector.Descriptor(" 累计通关地下城天数领奖", " 累计通关地下城天数领奖")]
		OAT_CUMULATEPASS_DUNGEON_REWARD = 4900,
		/// <summary>
		///  神器罐展示活动
		/// </summary>
		[AdvancedInspector.Descriptor(" 神器罐展示活动", " 神器罐展示活动")]
		OAT_ARTIFACT_JAR_SHOW = 5000,
		/// <summary>
		///  飞升礼包
		/// </summary>
		[AdvancedInspector.Descriptor(" 飞升礼包", " 飞升礼包")]
		OAT_FLYUP_GIFT = 5900,
		/// <summary>
		///  团本扶持
		/// </summary>
		[AdvancedInspector.Descriptor(" 团本扶持", " 团本扶持")]
		OAT_TEAM_COPY_SUPPORT = 6000,
		/// <summary>
		///  累计在线
		/// </summary>
		[AdvancedInspector.Descriptor(" 累计在线", " 累计在线")]
		OAT_CUMULATE_ONLINE = 6100,
		/// <summary>
		///  周年派对活动
		/// </summary>
		[AdvancedInspector.Descriptor(" 周年派对活动", " 周年派对活动")]
		OAT_ZHOUNIAN_PAIDUI = 6200,
		/// <summary>
		///  万圣节南瓜头
		/// </summary>
		[AdvancedInspector.Descriptor(" 万圣节南瓜头", " 万圣节南瓜头")]
		OAT_HALLOWEEN_PUMPKIN_HELMET = 6300,
		/// <summary>
		///  元旦2020
		/// </summary>
		[AdvancedInspector.Descriptor(" 元旦2020", " 元旦2020")]
		OAT_NEW_YEAR_2020 = 6400,
		/// <summary>
		///  点券消费返利
		/// </summary>
		[AdvancedInspector.Descriptor(" 点券消费返利", " 点券消费返利")]
		OAT_MONEY_CONSUME_REBATE = 6500,
		/// <summary>
		///  挑战者俱乐部	
		/// </summary>
		[AdvancedInspector.Descriptor(" 挑战者俱乐部	", " 挑战者俱乐部	")]
		OAT_CHALLENGE_HUB = 6600,
		/// <summary>
		///  兑换折扣活动 
		/// </summary>
		[AdvancedInspector.Descriptor(" 兑换折扣活动 ", " 兑换折扣活动 ")]
		OAT_EXCHANE_DISCOUNT = 6700,
		/// <summary>
		///  新服礼包折扣
		/// </summary>
		[AdvancedInspector.Descriptor(" 新服礼包折扣", " 新服礼包折扣")]
		OAT_NEW_SERVER_GIFT_DISCOUNT = 6800,
		/// <summary>
		///  春节红包领取活动
		/// </summary>
		[AdvancedInspector.Descriptor(" 春节红包领取活动", " 春节红包领取活动")]
		OAT_SPRING_FESTIVAL_RED_PACKET_RECV = 6900,
		/// <summary>
		///  春节地下城
		/// </summary>
		[AdvancedInspector.Descriptor(" 春节地下城", " 春节地下城")]
		OAT_SPRING_FESTIVAL_DUNGEON = 7000,
		/// <summary>
		///  春节红包雨
		/// </summary>
		[AdvancedInspector.Descriptor(" 春节红包雨", " 春节红包雨")]
		OAT_SPRING_FESTIVAL_RED_PACKET_RAIN = 7100,
		/// <summary>
		/// 在线好礼
		/// </summary>
		[AdvancedInspector.Descriptor("在线好礼", "在线好礼")]
		OAT_ONLINE_GIFT = 7600,
		/// <summary>
		/// 植树大挑战	
		/// </summary>
		[AdvancedInspector.Descriptor("植树大挑战	", "植树大挑战	")]
		OAT_PLANT_TREE = 7700,
		/// <summary>
		/// 地精币回馈大礼
		/// </summary>
		[AdvancedInspector.Descriptor("地精币回馈大礼", "地精币回馈大礼")]
		OAT_GNOME_COIN_GIFT = 7900,
		/// <summary>
		/// 全民砍价折扣
		/// </summary>
		[AdvancedInspector.Descriptor("全民砍价折扣", "全民砍价折扣")]
		OAT_WHOLE_BARGAIN_DISCOUNT = 50001,
		/// <summary>
		/// 全民砍价购买
		/// </summary>
		[AdvancedInspector.Descriptor("全民砍价购买", "全民砍价购买")]
		OAT_WHOLE_BARGAIN_SHOP = 50002,
		/// <summary>
		/// 充值消费活动
		/// </summary>
		[AdvancedInspector.Descriptor("充值消费活动", "充值消费活动")]
		OAT_RECHARGE_CONSUME_REBATE = 50003,
		/// <summary>
		/// 完成pk
		/// </summary>
		[AdvancedInspector.Descriptor("完成pk", "完成pk")]
		OAT_COMPLETE_PK = 50004,
		/// <summary>
		/// 冠军赛礼包
		/// </summary>
		[AdvancedInspector.Descriptor("冠军赛礼包", "冠军赛礼包")]
		OAT_CHAMPION_GIFT = 50005,
		/// <summary>
		/// 普通领奖
		/// </summary>
		[AdvancedInspector.Descriptor("普通领奖", "普通领奖")]
		OAT_COMMON_AWARD = 50006,
		/// <summary>
		/// 每日挑战
		/// </summary>
		[AdvancedInspector.Descriptor("每日挑战", "每日挑战")]
		OAT_DAILY_CHALLENGE = 50007,
		/// <summary>
		/// 帐号在线领奖
		/// </summary>
		[AdvancedInspector.Descriptor("帐号在线领奖", "帐号在线领奖")]
		OAT_ACCOUNT_ONLINE = 50008,
	}


	public enum OpActivityTag
	{

		OAT_NONE = 0,

		OAT_NEW = 1,
	}


	public enum OpActivityCircleType
	{

		AT_DAILY = 0,
		/// <summary>
		///  每日活动
		/// </summary>
		[AdvancedInspector.Descriptor(" 每日活动", " 每日活动")]
		AT_ONCE = 1,
		/// <summary>
		///  一次性活动
		/// </summary>
		[AdvancedInspector.Descriptor(" 一次性活动", " 一次性活动")]
		AT_WEEK = 2,
	}

	/// <summary>
	///  每周活动
	/// </summary>
	[AdvancedInspector.Descriptor(" 每周活动", " 每周活动")]
	public enum OpActivityState
	{

		OAS_END = 0,
		/// <summary>
		/// 活动结束
		/// </summary>
		[AdvancedInspector.Descriptor("活动结束", "活动结束")]
		OAS_IN = 1,
		/// <summary>
		/// 活动开始
		/// </summary>
		[AdvancedInspector.Descriptor("活动开始", "活动开始")]
		OAS_PREPARE = 2,
	}


	public enum OpActTaskState
	{

		OATS_INIT = 0,
		/// <summary>
		/// 初始状态
		/// </summary>
		[AdvancedInspector.Descriptor("初始状态", "初始状态")]
		OATS_UNFINISH = 1,
		/// <summary>
		/// 已经接任务
		/// </summary>
		[AdvancedInspector.Descriptor("已经接任务", "已经接任务")]
		OATS_FINISHED = 2,
		/// <summary>
		/// 已完成，未提交
		/// </summary>
		[AdvancedInspector.Descriptor("已完成，未提交", "已完成，未提交")]
		OATS_FAILED = 3,
		/// <summary>
		/// 失败
		/// </summary>
		[AdvancedInspector.Descriptor("失败", "失败")]
		OATS_SUBMITTING = 4,
		/// <summary>
		/// 正在提交中（已完成并且正在提交中)
		/// </summary>
		[AdvancedInspector.Descriptor("正在提交中（已完成并且正在提交中)", "正在提交中（已完成并且正在提交中)")]
		OATS_OVER = 5,
	}

	/// <summary>
	/// 植树大挑战活动状态
	/// </summary>
	[AdvancedInspector.Descriptor("植树大挑战活动状态", "植树大挑战活动状态")]
	public enum PlantOpActSate
	{

		POPS_NONE = 0,
		/// <summary>
		/// 未种植
		/// </summary>
		[AdvancedInspector.Descriptor("未种植", "未种植")]
		POPS_PLANTING = 1,
		/// <summary>
		/// 成长中
		/// </summary>
		[AdvancedInspector.Descriptor("成长中", "成长中")]
		POPS_CAN_APP = 2,
		/// <summary>
		/// 可鉴定
		/// </summary>
		[AdvancedInspector.Descriptor("可鉴定", "可鉴定")]
		POPS_ALLGET = 3,
	}

}
