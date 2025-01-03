using System;
using System.Text;

namespace Mock.Protocol
{

	public enum DungeonScore
	{

		C = 0,

		B = 1,

		A = 2,

		S = 3,

		SS = 4,

		SSS = 5,
	}

	/// <summary>
	///  深渊模式
	/// </summary>
	[AdvancedInspector.Descriptor(" 深渊模式", " 深渊模式")]
	public enum DungeonHellMode
	{
		/// <summary>
		///  无
		/// </summary>
		[AdvancedInspector.Descriptor(" 无", " 无")]
		Null = 0,
		/// <summary>
		///  普通
		/// </summary>
		[AdvancedInspector.Descriptor(" 普通", " 普通")]
		Normal = 1,
		/// <summary>
		///  困难
		/// </summary>
		[AdvancedInspector.Descriptor(" 困难", " 困难")]
		Hard = 2,
		/// <summary>
		///  终极困难
		/// </summary>
		[AdvancedInspector.Descriptor(" 终极困难", " 终极困难")]
		Hard_Ultimate = 3,
	}

	/// <summary>
	///  疲劳燃烧类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 疲劳燃烧类型", " 疲劳燃烧类型")]
	public enum FatigueBurnType
	{
		/// <summary>
		///  无
		/// </summary>
		[AdvancedInspector.Descriptor(" 无", " 无")]
		FBT_NONE = 0,
		/// <summary>
		///  普通
		/// </summary>
		[AdvancedInspector.Descriptor(" 普通", " 普通")]
		FBT_COMMON = 1,
		/// <summary>
		///  高级
		/// </summary>
		[AdvancedInspector.Descriptor(" 高级", " 高级")]
		FBT_ADVANCED = 2,
	}

	/// <summary>
	///  各种经验加成类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 各种经验加成类型", " 各种经验加成类型")]
	public enum DungeonAdditionType
	{
		/// <summary>
		///  经验药水
		/// </summary>
		[AdvancedInspector.Descriptor(" 经验药水", " 经验药水")]
		EXP_BUFF = 0,
		/// <summary>
		///  VIP经验加成
		/// </summary>
		[AdvancedInspector.Descriptor(" VIP经验加成", " VIP经验加成")]
		EXP_VIP = 1,
		/// <summary>
		///  评价经验加成
		/// </summary>
		[AdvancedInspector.Descriptor(" 评价经验加成", " 评价经验加成")]
		EXP_SCORE = 2,
		/// <summary>
		///  难度经验加成
		/// </summary>
		[AdvancedInspector.Descriptor(" 难度经验加成", " 难度经验加成")]
		EXP_HARD = 3,
		/// <summary>
		///  公会技能经验加成
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会技能经验加成", " 公会技能经验加成")]
		EXP_GUILD_SKILL = 4,
		/// <summary>
		///  VIP金币加成
		/// </summary>
		[AdvancedInspector.Descriptor(" VIP金币加成", " VIP金币加成")]
		GOLD_VIP = 5,
		/// <summary>
		///  TAP经验加成
		/// </summary>
		[AdvancedInspector.Descriptor(" TAP经验加成", " TAP经验加成")]
		EXP_TAP = 6,
		/// <summary>
		///  好友经验加成
		/// </summary>
		[AdvancedInspector.Descriptor(" 好友经验加成", " 好友经验加成")]
		EXP_FRIEND = 7,
		/// <summary>
		///  冒险队经验加成
		/// </summary>
		[AdvancedInspector.Descriptor(" 冒险队经验加成", " 冒险队经验加成")]
		DUNGEON_EXP_ADD_ADVENTURE_TEAM = 8,
		/// <summary>
		///  基础经验
		/// </summary>
		[AdvancedInspector.Descriptor(" 基础经验", " 基础经验")]
		DUNGEON_EXP_BASE = 9,
	}


	public enum RollOpTypeEnum
	{
		/// <summary>
		///  无效
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效", " 无效")]
		RIE_INVALID = 0,
		/// <summary>
		///  需要
		/// </summary>
		[AdvancedInspector.Descriptor(" 需要", " 需要")]
		RIE_NEED = 1,
		/// <summary>
		///  谦让
		/// </summary>
		[AdvancedInspector.Descriptor(" 谦让", " 谦让")]
		RIE_MODEST = 2,
	}


	public enum DungeonChestType
	{
		/// <summary>
		///  普通宝箱
		/// </summary>
		[AdvancedInspector.Descriptor(" 普通宝箱", " 普通宝箱")]
		Normal = 0,
		/// <summary>
		///  Vip宝箱
		/// </summary>
		[AdvancedInspector.Descriptor(" Vip宝箱", " Vip宝箱")]
		Vip = 1,
		/// <summary>
		///  付费宝箱
		/// </summary>
		[AdvancedInspector.Descriptor(" 付费宝箱", " 付费宝箱")]
		Pay = 2,
	}

}
