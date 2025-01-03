using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  战斗类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 战斗类型", " 战斗类型")]
	public enum RaceType
	{
		/// <summary>
		///  关卡
		/// </summary>
		[AdvancedInspector.Descriptor(" 关卡", " 关卡")]
		Dungeon = 0,
		/// <summary>
		///  PK
		/// </summary>
		[AdvancedInspector.Descriptor(" PK", " PK")]
		PK = 1,
		/// <summary>
		///  公会战
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战", " 公会战")]
		GuildBattle = 2,
		/// <summary>
		///  赏金联赛预选赛
		/// </summary>
		[AdvancedInspector.Descriptor(" 赏金联赛预选赛", " 赏金联赛预选赛")]
		PremiumLeaguePreliminay = 3,
		/// <summary>
		///  赏金联赛淘汰赛
		/// </summary>
		[AdvancedInspector.Descriptor(" 赏金联赛淘汰赛", " 赏金联赛淘汰赛")]
		PremiumLeagueBattle = 4,
		/// <summary>
		///  3v3
		/// </summary>
		[AdvancedInspector.Descriptor(" 3v3", " 3v3")]
		PK_3V3 = 5,
		/// <summary>
		///  3v3积分赛
		/// </summary>
		[AdvancedInspector.Descriptor(" 3v3积分赛", " 3v3积分赛")]
		ScoreWar = 6,
		/// <summary>
		///  3v3乱斗
		/// </summary>
		[AdvancedInspector.Descriptor(" 3v3乱斗", " 3v3乱斗")]
		PK_3V3_Melee = 7,
		/// <summary>
		///  吃鸡
		/// </summary>
		[AdvancedInspector.Descriptor(" 吃鸡", " 吃鸡")]
		ChiJi = 8,
		/// <summary>
		/// 公平竞技场
		/// </summary>
		[AdvancedInspector.Descriptor("公平竞技场", "公平竞技场")]
		PK_EQUAL_1V1 = 9,
		/// <summary>
		///  2V2乱斗活动
		/// </summary>
		[AdvancedInspector.Descriptor(" 2V2乱斗活动", " 2V2乱斗活动")]
		PK_2V2_Melee = 11,
		/// <summary>
		///  爬塔
		/// </summary>
		[AdvancedInspector.Descriptor(" 爬塔", " 爬塔")]
		PK_TOWER = 12,
		/// <summary>
		///  冠军赛
		/// </summary>
		[AdvancedInspector.Descriptor(" 冠军赛", " 冠军赛")]
		PK_Champion = 13,
	}

	/// <summary>
	/// 装备附带属性
	/// </summary>
	[AdvancedInspector.Descriptor("装备附带属性", "装备附带属性")]
	public enum EquipExProp
	{

		EEP_LIGHT = 0,
		/// <summary>
		/// 光属性
		/// </summary>
		[AdvancedInspector.Descriptor("光属性", "光属性")]
		EEP_FIRE = 1,
		/// <summary>
		/// 火属性
		/// </summary>
		[AdvancedInspector.Descriptor("火属性", "火属性")]
		EEP_ICE = 2,
		/// <summary>
		/// 冰属性
		/// </summary>
		[AdvancedInspector.Descriptor("冰属性", "冰属性")]
		EEP_DARK = 3,
		/// <summary>
		/// 暗属性
		/// </summary>
		[AdvancedInspector.Descriptor("暗属性", "暗属性")]
		EEP_MAX = 4,
	}

	/// <summary>
	/// 装备异常抗性
	/// </summary>
	[AdvancedInspector.Descriptor("装备异常抗性", "装备异常抗性")]
	public enum EquipAbnormalResist
	{

		EAR_FLASH = 0,
		/// <summary>
		/// 感电
		/// </summary>
		[AdvancedInspector.Descriptor("感电", "感电")]
		EAR_BLEEDING = 1,
		/// <summary>
		/// 出血
		/// </summary>
		[AdvancedInspector.Descriptor("出血", "出血")]
		EAR_BURN = 2,
		/// <summary>
		/// 灼烧
		/// </summary>
		[AdvancedInspector.Descriptor("灼烧", "灼烧")]
		EAR_POISON = 3,
		/// <summary>
		/// 中毒
		/// </summary>
		[AdvancedInspector.Descriptor("中毒", "中毒")]
		EAR_BLIND = 4,
		/// <summary>
		/// 失明
		/// </summary>
		[AdvancedInspector.Descriptor("失明", "失明")]
		EAR_STUN = 5,
		/// <summary>
		/// 晕眩
		/// </summary>
		[AdvancedInspector.Descriptor("晕眩", "晕眩")]
		EAR_STONE = 6,
		/// <summary>
		/// 石化
		/// </summary>
		[AdvancedInspector.Descriptor("石化", "石化")]
		EAR_FROZEN = 7,
		/// <summary>
		/// 冰冻
		/// </summary>
		[AdvancedInspector.Descriptor("冰冻", "冰冻")]
		EAR_SLEEP = 8,
		/// <summary>
		/// 睡眠
		/// </summary>
		[AdvancedInspector.Descriptor("睡眠", "睡眠")]
		EAR_CONFUNSE = 9,
		/// <summary>
		/// 混乱
		/// </summary>
		[AdvancedInspector.Descriptor("混乱", "混乱")]
		EAR_STRAIN = 10,
		/// <summary>
		/// 束缚
		/// </summary>
		[AdvancedInspector.Descriptor("束缚", "束缚")]
		EAR_SPEED_DOWN = 11,
		/// <summary>
		/// 减速
		/// </summary>
		[AdvancedInspector.Descriptor("减速", "减速")]
		EAR_CURSE = 12,
		/// <summary>
		/// 诅咒
		/// </summary>
		[AdvancedInspector.Descriptor("诅咒", "诅咒")]
		EAR_MAX = 13,
	}

	/// <summary>
	///  好友状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 好友状态", " 好友状态")]
	public enum FriendMatchStatus
	{
		/// <summary>
		///  空闲
		/// </summary>
		[AdvancedInspector.Descriptor(" 空闲", " 空闲")]
		Idle = 0,
		/// <summary>
		///  忙碌
		/// </summary>
		[AdvancedInspector.Descriptor(" 忙碌", " 忙碌")]
		Busy = 1,
		/// <summary>
		///  下线
		/// </summary>
		[AdvancedInspector.Descriptor(" 下线", " 下线")]
		Offlie = 2,
	}

	/// <summary>
	///  赛季状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 赛季状态", " 赛季状态")]
	public enum SeasonPlayStatus
	{

		SPS_INVALID = 0,

		SPS_NEW = 1,

		SPS_NEW_ATTR = 2,
	}

}
