using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  宝珠镶嵌孔类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 宝珠镶嵌孔类型", " 宝珠镶嵌孔类型")]
	public enum PreciousBeadMountHoleType
	{
		/// <summary>
		///  初始值
		/// </summary>
		[AdvancedInspector.Descriptor(" 初始值", " 初始值")]
		PBMHT_NONE = 0,
		/// <summary>
		///  普通
		/// </summary>
		[AdvancedInspector.Descriptor(" 普通", " 普通")]
		PBMHT_COMMON = 1,
		/// <summary>
		///  多彩
		/// </summary>
		[AdvancedInspector.Descriptor(" 多彩", " 多彩")]
		PBMHT_COLOUR = 2,
		/// <summary>
		///  最大值
		/// </summary>
		[AdvancedInspector.Descriptor(" 最大值", " 最大值")]
		PBMHT_MAX = 3,
	}

	/// <summary>
	///  商城购买获得物类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 商城购买获得物类型", " 商城购买获得物类型")]
	public enum MallBuyGotType
	{
		/// <summary>
		///  无
		/// </summary>
		[AdvancedInspector.Descriptor(" 无", " 无")]
		MALL_BUY_GOT_TYPE_NONE = 0,
		/// <summary>
		///  地精纪念币
		/// </summary>
		[AdvancedInspector.Descriptor(" 地精纪念币", " 地精纪念币")]
		MALL_BUY_GOT_TYPE_GNOME_COIN = 1,
	}

	/// <summary>
	///  我的夺宝状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 我的夺宝状态", " 我的夺宝状态")]
	public enum GambingMineStatus
	{
		/// <summary>
		///  初始值
		/// </summary>
		[AdvancedInspector.Descriptor(" 初始值", " 初始值")]
		GMS_INIT = 0,
		/// <summary>
		///  夺宝成功
		/// </summary>
		[AdvancedInspector.Descriptor(" 夺宝成功", " 夺宝成功")]
		GMS_SUCCESS = 1,
		/// <summary>
		///  夺宝失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 夺宝失败", " 夺宝失败")]
		GMS_FAILE = 2,
		/// <summary>
		///  等待揭晓
		/// </summary>
		[AdvancedInspector.Descriptor(" 等待揭晓", " 等待揭晓")]
		GMS_WAIT = 3,
	}

	/// <summary>
	/// 夺宝商品状态
	/// </summary>
	[AdvancedInspector.Descriptor("夺宝商品状态", "夺宝商品状态")]
	public enum GambingItemStatus
	{

		GIS_INVLID = 0,
		/// <summary>
		///  准备
		/// </summary>
		[AdvancedInspector.Descriptor(" 准备", " 准备")]
		GIS_PREPARE = 1,
		/// <summary>
		///  在售
		/// </summary>
		[AdvancedInspector.Descriptor(" 在售", " 在售")]
		GIS_SELLING = 2,
		/// <summary>
		///  售完
		/// </summary>
		[AdvancedInspector.Descriptor(" 售完", " 售完")]
		GIS_SOLD_OUE = 3,
		/// <summary>
		///  开奖
		/// </summary>
		[AdvancedInspector.Descriptor(" 开奖", " 开奖")]
		GIS_LOTTERY = 4,
		/// <summary>
		///  下架
		/// </summary>
		[AdvancedInspector.Descriptor(" 下架", " 下架")]
		GIS_OFF_SALE = 5,
	}

	/// <summary>
	/// 商城玩家绑定礼包激活条件
	/// </summary>
	[AdvancedInspector.Descriptor("商城玩家绑定礼包激活条件", "商城玩家绑定礼包激活条件")]
	public enum MallGiftPackActivateCond
	{
		/// <summary>
		/// 无
		/// </summary>
		[AdvancedInspector.Descriptor("无", "无")]
		INVALID = 0,
		/// <summary>
		/// 强化到10
		/// </summary>
		[AdvancedInspector.Descriptor("强化到10", "强化到10")]
		STRENGEN_TEN = 1,
		/// <summary>
		/// 强化资源不足	
		/// </summary>
		[AdvancedInspector.Descriptor("强化资源不足	", "强化资源不足	")]
		STRENGEN_NO_RESOURCE = 2,
		/// <summary>
		/// 品级调整箱不足
		/// </summary>
		[AdvancedInspector.Descriptor("品级调整箱不足", "品级调整箱不足")]
		NO_QUILTY_ADJUST_BOX = 3,
		/// <summary>
		/// 疲劳不足，且背包中无疲劳药水
		/// </summary>
		[AdvancedInspector.Descriptor("疲劳不足，且背包中无疲劳药水", "疲劳不足，且背包中无疲劳药水")]
		NO_FATIGUE = 4,
		/// <summary>
		/// 刷深渊门票不足	
		/// </summary>
		[AdvancedInspector.Descriptor("刷深渊门票不足	", "刷深渊门票不足	")]
		NO_HELL_TICKET = 5,
		/// <summary>
		/// 刷远古门票不足	
		/// </summary>
		[AdvancedInspector.Descriptor("刷远古门票不足	", "刷远古门票不足	")]
		NO_ANCIENT_TICKET = 6,
		/// <summary>
		/// 死亡
		/// </summary>
		[AdvancedInspector.Descriptor("死亡", "死亡")]
		DIE = 7,
		/// <summary>
		/// 强化装备碎掉,推送10级装备
		/// </summary>
		[AdvancedInspector.Descriptor("强化装备碎掉,推送10级装备", "强化装备碎掉,推送10级装备")]
		STRENGEN_BROKE_TEN = 8,
		/// <summary>
		/// 强化装备碎掉,推送15级装备
		/// </summary>
		[AdvancedInspector.Descriptor("强化装备碎掉,推送15级装备", "强化装备碎掉,推送15级装备")]
		STRENGEN_BROKE_FIFTEEN = 9,
		/// <summary>
		/// 强化装备碎掉,推送20级装备
		/// </summary>
		[AdvancedInspector.Descriptor("强化装备碎掉,推送20级装备", "强化装备碎掉,推送20级装备")]
		STRENGEN_BROKE_TWENTY = 10,
		/// <summary>
		/// 强化装备碎掉,推送25级装备
		/// </summary>
		[AdvancedInspector.Descriptor("强化装备碎掉,推送25级装备", "强化装备碎掉,推送25级装备")]
		STRENGEN_BROKE_TWENTY_FIVE = 11,
		/// <summary>
		/// 强化装备碎掉,推送30级装备
		/// </summary>
		[AdvancedInspector.Descriptor("强化装备碎掉,推送30级装备", "强化装备碎掉,推送30级装备")]
		STRENGEN_BROKE_THIRTY = 12,
		/// <summary>
		/// 强化装备碎掉,推送35级装备
		/// </summary>
		[AdvancedInspector.Descriptor("强化装备碎掉,推送35级装备", "强化装备碎掉,推送35级装备")]
		STRENGEN_BROKE_THIRTY_FIVE = 13,
		/// <summary>
		/// 强化装备碎掉,推送40级装备
		/// </summary>
		[AdvancedInspector.Descriptor("强化装备碎掉,推送40级装备", "强化装备碎掉,推送40级装备")]
		STRENGEN_BROKE_FORTY = 14,
		/// <summary>
		/// 强化装备碎掉,推送45级装备
		/// </summary>
		[AdvancedInspector.Descriptor("强化装备碎掉,推送45级装备", "强化装备碎掉,推送45级装备")]
		STRENGEN_BROKE_FORTY_FIVE = 15,
		/// <summary>
		/// 强化装备碎掉,推送50级装备
		/// </summary>
		[AdvancedInspector.Descriptor("强化装备碎掉,推送50级装备", "强化装备碎掉,推送50级装备")]
		STRENGEN_BROKE_FIFTY = 16,
	}

	/// <summary>
	/// 商城商品类型
	/// </summary>
	[AdvancedInspector.Descriptor("商城商品类型", "商城商品类型")]
	public enum MallGoodsType
	{
		/// <summary>
		/// 普通商品
		/// </summary>
		[AdvancedInspector.Descriptor("普通商品", "普通商品")]
		INVALID = 0,
		/// <summary>
		/// 人民币礼包：可每日刷新
		/// </summary>
		[AdvancedInspector.Descriptor("人民币礼包：可每日刷新", "人民币礼包：可每日刷新")]
		GIFT_DAILY_REFRESH = 1,
		/// <summary>
		/// 礼包：账号激活限制一次
		/// </summary>
		[AdvancedInspector.Descriptor("礼包：账号激活限制一次", "礼包：账号激活限制一次")]
		GIFT_ACTIVATE_ONCE = 2,
		/// <summary>
		/// 礼包：账号激活限制三次礼包
		/// </summary>
		[AdvancedInspector.Descriptor("礼包：账号激活限制三次礼包", "礼包：账号激活限制三次礼包")]
		GIFT_ACTIVATE_THREE_TIMES = 3,
		/// <summary>
		/// 普通商品：可多选一
		/// </summary>
		[AdvancedInspector.Descriptor("普通商品：可多选一", "普通商品：可多选一")]
		COMMON_CHOOSE_ONE = 4,
		/// <summary>
		/// 礼包：限时活动
		/// </summary>
		[AdvancedInspector.Descriptor("礼包：限时活动", "礼包：限时活动")]
		GIFT_ACTIVITY = 5,
		/// <summary>
		/// 礼包: 普通不刷新礼包
		/// </summary>
		[AdvancedInspector.Descriptor("礼包: 普通不刷新礼包", "礼包: 普通不刷新礼包")]
		GIFT_COMMON = 6,
		/// <summary>
		/// 礼包: 每日刷新
		/// </summary>
		[AdvancedInspector.Descriptor("礼包: 每日刷新", "礼包: 每日刷新")]
		GIFT_COMMON_DAILY_REFRESH = 7,
	}

	/// <summary>
	/// 商城礼包活动状态
	/// </summary>
	[AdvancedInspector.Descriptor("商城礼包活动状态", "商城礼包活动状态")]
	public enum MallGiftPackActivityState
	{
		/// <summary>
		/// 无效
		/// </summary>
		[AdvancedInspector.Descriptor("无效", "无效")]
		GPAS_INVALID = 0,
		/// <summary>
		/// 开放
		/// </summary>
		[AdvancedInspector.Descriptor("开放", "开放")]
		GPAS_OPEN = 1,
		/// <summary>
		/// 关闭
		/// </summary>
		[AdvancedInspector.Descriptor("关闭", "关闭")]
		GPAS_CLOSED = 2,
	}

	/// <summary>
	/// 私人订制触发状态
	/// </summary>
	[AdvancedInspector.Descriptor("私人订制触发状态", "私人订制触发状态")]
	public enum MallPersonalTailorActivateState
	{

		MPTAS_INVALID = 0,
		/// <summary>
		/// 已开启
		/// </summary>
		[AdvancedInspector.Descriptor("已开启", "已开启")]
		MPTAS_OPEN = 1,
		/// <summary>
		/// 已关闭
		/// </summary>
		[AdvancedInspector.Descriptor("已关闭", "已关闭")]
		MPTAS_CLOSED = 2,
		/// <summary>
		/// 玩家上线
		/// </summary>
		[AdvancedInspector.Descriptor("玩家上线", "玩家上线")]
		MPTAS_ONLINE = 3,
	}

	/// <summary>
	/// item key-value key枚举类型
	/// </summary>
	[AdvancedInspector.Descriptor("item key-value key枚举类型", "item key-value key枚举类型")]
	public enum ItemKeyValuesKeyType
	{

		IKVKT_NONE = 0,

		IKVKT_WBP_STR = 1,
		/// <summary>
		/// 力量
		/// </summary>
		[AdvancedInspector.Descriptor("力量", "力量")]
		IKVKT_WBP_STA = 2,
		/// <summary>
		/// 体力
		/// </summary>
		[AdvancedInspector.Descriptor("体力", "体力")]
		IKVKT_WBP_INT = 3,
		/// <summary>
		/// 智力
		/// </summary>
		[AdvancedInspector.Descriptor("智力", "智力")]
		IKVKT_WBP_SPR = 4,
		/// <summary>
		/// 精神
		/// </summary>
		[AdvancedInspector.Descriptor("精神", "精神")]
		IKVKT_WBP_HPMAX = 5,
		/// <summary>
		/// hp最大值
		/// </summary>
		[AdvancedInspector.Descriptor("hp最大值", "hp最大值")]
		IKVKT_WBP_MPMAX = 6,
		/// <summary>
		/// mp最大值
		/// </summary>
		[AdvancedInspector.Descriptor("mp最大值", "mp最大值")]
		IKVKT_WBP_HPREC = 7,
		/// <summary>
		/// hp恢复
		/// </summary>
		[AdvancedInspector.Descriptor("hp恢复", "hp恢复")]
		IKVKT_WBP_MPREC = 8,
		/// <summary>
		/// mp恢复
		/// </summary>
		[AdvancedInspector.Descriptor("mp恢复", "mp恢复")]
		IKVKT_WBP_HIT = 9,
		/// <summary>
		/// 命中率
		/// </summary>
		[AdvancedInspector.Descriptor("命中率", "命中率")]
		IKVKT_WBP_DEX = 10,
		/// <summary>
		/// 回避率
		/// </summary>
		[AdvancedInspector.Descriptor("回避率", "回避率")]
		IKVKT_WBP_PHYCRT = 11,
		/// <summary>
		/// 物理暴击
		/// </summary>
		[AdvancedInspector.Descriptor("物理暴击", "物理暴击")]
		IKVKT_WBP_MGCCRT = 12,
		/// <summary>
		/// 魔法暴击
		/// </summary>
		[AdvancedInspector.Descriptor("魔法暴击", "魔法暴击")]
		IKVKT_WBP_ATKSPD = 13,
		/// <summary>
		/// 攻速
		/// </summary>
		[AdvancedInspector.Descriptor("攻速", "攻速")]
		IKVKT_WBP_RDYSPD = 14,
		/// <summary>
		/// 施放速度
		/// </summary>
		[AdvancedInspector.Descriptor("施放速度", "施放速度")]
		IKVKT_WBP_MOVSPD = 15,
		/// <summary>
		/// 移动速度
		/// </summary>
		[AdvancedInspector.Descriptor("移动速度", "移动速度")]
		IKVKT_WBP_JUMP = 16,
		/// <summary>
		/// 跳跃力
		/// </summary>
		[AdvancedInspector.Descriptor("跳跃力", "跳跃力")]
		IKVKT_WBP_HITREC = 17,
		/// <summary>
		/// 硬直
		/// </summary>
		[AdvancedInspector.Descriptor("硬直", "硬直")]
		IKVKT_WBP_PHYATK = 18,
		/// <summary>
		/// 物攻
		/// </summary>
		[AdvancedInspector.Descriptor("物攻", "物攻")]
		IKVKT_WBP_MAGATK = 19,
		/// <summary>
		/// 法攻
		/// </summary>
		[AdvancedInspector.Descriptor("法攻", "法攻")]
		IKVKT_WBP_PHYDEF = 20,
		/// <summary>
		/// 物防
		/// </summary>
		[AdvancedInspector.Descriptor("物防", "物防")]
		IKVKT_WBP_MAGDEF = 21,
		/// <summary>
		/// 法防	
		/// </summary>
		[AdvancedInspector.Descriptor("法防	", "法防	")]
		IKVKT_WBP_LIGHT = 22,
		/// <summary>
		/// 光属性攻击
		/// </summary>
		[AdvancedInspector.Descriptor("光属性攻击", "光属性攻击")]
		IKVKT_WBP_FIRE = 23,
		/// <summary>
		/// 火属性攻击
		/// </summary>
		[AdvancedInspector.Descriptor("火属性攻击", "火属性攻击")]
		IKVKT_WBP_ICE = 24,
		/// <summary>
		/// 冰属性攻击
		/// </summary>
		[AdvancedInspector.Descriptor("冰属性攻击", "冰属性攻击")]
		IKVKT_WBP_DARK = 25,
		/// <summary>
		/// 暗属性攻击
		/// </summary>
		[AdvancedInspector.Descriptor("暗属性攻击", "暗属性攻击")]
		IKVKT_WBP_LIGHT_ATK = 26,
		/// <summary>
		/// 光属性强化
		/// </summary>
		[AdvancedInspector.Descriptor("光属性强化", "光属性强化")]
		IKVKT_WBP_FIRE_ATK = 27,
		/// <summary>
		/// 火属性强化
		/// </summary>
		[AdvancedInspector.Descriptor("火属性强化", "火属性强化")]
		IKVKT_WBP_ICE_ATK = 28,
		/// <summary>
		/// 冰属性强化
		/// </summary>
		[AdvancedInspector.Descriptor("冰属性强化", "冰属性强化")]
		IKVKT_WBP_DARK_ATK = 29,
		/// <summary>
		/// 暗属性强化
		/// </summary>
		[AdvancedInspector.Descriptor("暗属性强化", "暗属性强化")]
		IKVKT_WBP_LIGHT_DEF = 30,
		/// <summary>
		/// 光属性抗性
		/// </summary>
		[AdvancedInspector.Descriptor("光属性抗性", "光属性抗性")]
		IKVKT_WBP_FIRE_DEF = 31,
		/// <summary>
		/// 火属性抗性
		/// </summary>
		[AdvancedInspector.Descriptor("火属性抗性", "火属性抗性")]
		IKVKT_WBP_ICE_DEF = 32,
		/// <summary>
		/// 冰属性抗性
		/// </summary>
		[AdvancedInspector.Descriptor("冰属性抗性", "冰属性抗性")]
		IKVKT_WBP_DARK_DEF = 33,
		/// <summary>
		/// 暗属性抗性
		/// </summary>
		[AdvancedInspector.Descriptor("暗属性抗性", "暗属性抗性")]
		IKVKT_WBP_GDKX = 34,
		/// <summary>
		/// 感电抗性
		/// </summary>
		[AdvancedInspector.Descriptor("感电抗性", "感电抗性")]
		IKVKT_WBP_CXKX = 35,
		/// <summary>
		/// 出血抗性
		/// </summary>
		[AdvancedInspector.Descriptor("出血抗性", "出血抗性")]
		IKVKT_WBP_ZSKX = 36,
		/// <summary>
		/// 灼烧抗性
		/// </summary>
		[AdvancedInspector.Descriptor("灼烧抗性", "灼烧抗性")]
		IKVKT_WBP_ZDKX = 37,
		/// <summary>
		/// 中毒抗性
		/// </summary>
		[AdvancedInspector.Descriptor("中毒抗性", "中毒抗性")]
		IKVKT_WBP_SMKX = 38,
		/// <summary>
		/// 失明抗性
		/// </summary>
		[AdvancedInspector.Descriptor("失明抗性", "失明抗性")]
		IKVKT_WBP_XYKX = 39,
		/// <summary>
		/// 眩晕抗性
		/// </summary>
		[AdvancedInspector.Descriptor("眩晕抗性", "眩晕抗性")]
		IKVKT_WBP_SHKX = 40,
		/// <summary>
		/// 石化抗性
		/// </summary>
		[AdvancedInspector.Descriptor("石化抗性", "石化抗性")]
		IKVKT_WBP_BDKX = 41,
		/// <summary>
		/// 冰冻抗性
		/// </summary>
		[AdvancedInspector.Descriptor("冰冻抗性", "冰冻抗性")]
		IKVKT_WBP_SLKX = 42,
		/// <summary>
		/// 睡眠抗性
		/// </summary>
		[AdvancedInspector.Descriptor("睡眠抗性", "睡眠抗性")]
		IKVKT_WBP_HLKX = 43,
		/// <summary>
		/// 混乱抗性
		/// </summary>
		[AdvancedInspector.Descriptor("混乱抗性", "混乱抗性")]
		IKVKT_WBP_SFKX = 44,
		/// <summary>
		/// 束缚抗性
		/// </summary>
		[AdvancedInspector.Descriptor("束缚抗性", "束缚抗性")]
		IKVKT_WBP_JSKX = 45,
		/// <summary>
		/// 减速抗性
		/// </summary>
		[AdvancedInspector.Descriptor("减速抗性", "减速抗性")]
		IKVKT_WBP_ZZKX = 46,
		/// <summary>
		/// 诅咒抗性
		/// </summary>
		[AdvancedInspector.Descriptor("诅咒抗性", "诅咒抗性")]
		IKVKT_WBP_YKXZ = 47,
		/// <summary>
		/// 所有异常抗性
		/// </summary>
		[AdvancedInspector.Descriptor("所有异常抗性", "所有异常抗性")]
		IKVKT_WBP_INDEPEND_ATK = 48,
		/// <summary>
		/// 独立攻击力
		/// </summary>
		[AdvancedInspector.Descriptor("独立攻击力", "独立攻击力")]
		IKVKT_WBP_PhySkillMp = 49,
		/// <summary>
		/// 物理技能耗蓝变化
		/// </summary>
		[AdvancedInspector.Descriptor("物理技能耗蓝变化", "物理技能耗蓝变化")]
		IKVKT_WBP_PhySkillCd = 50,
		/// <summary>
		/// 物理技能CD变化
		/// </summary>
		[AdvancedInspector.Descriptor("物理技能CD变化", "物理技能CD变化")]
		IKVKT_WBP_MagSkillMp = 51,
		/// <summary>
		/// 魔法技能耗蓝变化
		/// </summary>
		[AdvancedInspector.Descriptor("魔法技能耗蓝变化", "魔法技能耗蓝变化")]
		IKVKT_WBP_MagSkillCd = 52,
		/// <summary>
		/// 魔法技能CD变化
		/// </summary>
		[AdvancedInspector.Descriptor("魔法技能CD变化", "魔法技能CD变化")]
		IKVKT_WBP_QSKX = 53,
		/// <summary>
		/// 侵蚀抗性
		/// </summary>
		[AdvancedInspector.Descriptor("侵蚀抗性", "侵蚀抗性")]
		IKVKT_WBP_BUFF_BEGIN = 100,
		/// <summary>
		/// 特殊属性（buff信息表ID）
		/// </summary>
		[AdvancedInspector.Descriptor("特殊属性（buff信息表ID）", "特殊属性（buff信息表ID）")]
		IKVKT_WBP_BUFF_END = 200,

		IKVKT_WBP_SPECIAL = 300,
		/// <summary>
		/// 装备特性类型
		/// </summary>
		[AdvancedInspector.Descriptor("装备特性类型", "装备特性类型")]
		IKVKT_WBP_BUILD_WEAPON = 301,
		/// <summary>
		/// 打造出的装备
		/// </summary>
		[AdvancedInspector.Descriptor("打造出的装备", "打造出的装备")]
		IKVKT_WBP_SPECIAL_DETAIL = 302,
		/// <summary>
		/// 装备特性数值
		/// </summary>
		[AdvancedInspector.Descriptor("装备特性数值", "装备特性数值")]
		IKVKT_WBP_ADDITION_SCORE = 303,
		/// <summary>
		/// 打造出武器的附加评分
		/// </summary>
		[AdvancedInspector.Descriptor("打造出武器的附加评分", "打造出武器的附加评分")]
		IKVKT_WBP_MAX = 304,

		IKVKT_AUCTION_TRANS_TIMES = 401,
		/// <summary>
		/// 拍卖行交易次数
		/// </summary>
		[AdvancedInspector.Descriptor("拍卖行交易次数", "拍卖行交易次数")]
		IKVKT_AUCTION_TRANS_TIMESTMP = 402,
		/// <summary>
		/// 拍卖行最近交易时间
		/// </summary>
		[AdvancedInspector.Descriptor("拍卖行最近交易时间", "拍卖行最近交易时间")]
		IKVKT_MAX = 403,
	}

	/// <summary>
	///  快速购买目标类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 快速购买目标类型", " 快速购买目标类型")]
	public enum QuickBuyTargetType
	{
		/// <summary>
		///  复活
		/// </summary>
		[AdvancedInspector.Descriptor(" 复活", " 复活")]
		QUICK_BUY_REVIVE = 0,
		/// <summary>
		///  购买道具
		/// </summary>
		[AdvancedInspector.Descriptor(" 购买道具", " 购买道具")]
		QUICK_BUY_ITEM = 1,
	}


	public enum FashionMergeResultType
	{

		FMRT_NORMAL = 1,

		FMRT_SPECIAL = 2,
	}


	public enum ItemCheckType
	{

		ICT_VALID = 0,
		/// <summary>
		///  有效
		/// </summary>
		[AdvancedInspector.Descriptor(" 有效", " 有效")]
		ICT_ABOUT_TO_EXPIRE = 1,
		/// <summary>
		///  快要到期
		/// </summary>
		[AdvancedInspector.Descriptor(" 快要到期", " 快要到期")]
		ICT_EXPIRE = 2,
		/// <summary>
		///  到期可续费
		/// </summary>
		[AdvancedInspector.Descriptor(" 到期可续费", " 到期可续费")]
		ICT_NEED_DELETE = 3,
	}


	public enum ItemLockType
	{
		/// <summary>
		///  道具没锁
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具没锁", " 道具没锁")]
		ILT_ITEM_UNLOCK = 0,
		/// <summary>
		///  道具锁
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具锁", " 道具锁")]
		ILT_ITEM_LOCK = 1,
		/// <summary>
		///  租赁锁	
		/// </summary>
		[AdvancedInspector.Descriptor(" 租赁锁	", " 租赁锁	")]
		ILT_LEASE_LOCK = 2,
		/// <summary>
		///  时装锁
		/// </summary>
		[AdvancedInspector.Descriptor(" 时装锁", " 时装锁")]
		ILT_FASHION_LOCK = 8,
	}

	/// <summary>
	/// 宝珠升级类型
	/// </summary>
	[AdvancedInspector.Descriptor("宝珠升级类型", "宝珠升级类型")]
	public enum UpgradePrecType
	{
		/// <summary>
		///  未镶嵌
		/// </summary>
		[AdvancedInspector.Descriptor(" 未镶嵌", " 未镶嵌")]
		UnMounted = 0,
		/// <summary>
		///  已镶嵌
		/// </summary>
		[AdvancedInspector.Descriptor(" 已镶嵌", " 已镶嵌")]
		Mounted = 1,
	}

	/// <summary>
	/// 黑市商人类型
	/// </summary>
	[AdvancedInspector.Descriptor("黑市商人类型", "黑市商人类型")]
	public enum BlackMarketType
	{

		BmtInvalid = 0,
		/// <summary>
		///  固定价格
		/// </summary>
		[AdvancedInspector.Descriptor(" 固定价格", " 固定价格")]
		BmtFixedPrice = 1,
		/// <summary>
		///  竞拍价格
		/// </summary>
		[AdvancedInspector.Descriptor(" 竞拍价格", " 竞拍价格")]
		BmtAuctionPrice = 2,
	}


	public enum BlackMarketAuctionState
	{

		BmaisInvalid = 0,
		/// <summary>
		/// 可以竞拍
		/// </summary>
		[AdvancedInspector.Descriptor("可以竞拍", "可以竞拍")]
		BmaisCanAuction = 1,
		/// <summary>
		/// 已申请
		/// </summary>
		[AdvancedInspector.Descriptor("已申请", "已申请")]
		BmaisApplyed = 2,
		/// <summary>
		/// 已交易
		/// </summary>
		[AdvancedInspector.Descriptor("已交易", "已交易")]
		BmaisTransed = 3,

		BmaisMax = 4,
	}


	public enum MagicCardCompOneKeyEndReason
	{

		MCCER_NONE = 0,
		/// <summary>
		/// 次数达到最大
		/// </summary>
		[AdvancedInspector.Descriptor("次数达到最大", "次数达到最大")]
		MCCER_TIMES_MAX = 1,
		/// <summary>
		/// 背包满
		/// </summary>
		[AdvancedInspector.Descriptor("背包满", "背包满")]
		MCCER_ITEMPACK_FULL = 2,
		/// <summary>
		/// 货币不足
		/// </summary>
		[AdvancedInspector.Descriptor("货币不足", "货币不足")]
		MCCER_MONEY_INSUFF = 3,

		MCCER_MONEY_MAX = 4,
	}

	/// <summary>
	///  商城购买获得物类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 商城购买获得物类型", " 商城购买获得物类型")]
	public enum EquipSchemeType
	{
		/// <summary>
		///  无
		/// </summary>
		[AdvancedInspector.Descriptor(" 无", " 无")]
		EQST_NONE = 0,
		/// <summary>
		/// 装备，称号
		/// </summary>
		[AdvancedInspector.Descriptor("装备，称号", "装备，称号")]
		EQST_EQUIP = 1,
	}

	/// <summary>
	///  装备转换类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 装备转换类型", " 装备转换类型")]
	public enum EquipConvertType
	{
		/// <summary>
		///  同套装
		/// </summary>
		[AdvancedInspector.Descriptor(" 同套装", " 同套装")]
		EQCT_SAME = 1,
		/// <summary>
		///  跨套装
		/// </summary>
		[AdvancedInspector.Descriptor(" 跨套装", " 跨套装")]
		EQCT_DIFF = 2,
	}

}
