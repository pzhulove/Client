using System;
using System.Text;

namespace Mock.Protocol
{

	public enum SceneObjectStatus
	{

		SOS_STAND = 0,

		SOS_WALK = 2,
	}

	/// <summary>
	///  红点标记
	/// </summary>
	[AdvancedInspector.Descriptor(" 红点标记", " 红点标记")]
	public enum RedPointFlag
	{
		/// <summary>
		///  公会请求者
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会请求者", " 公会请求者")]
		GUILD_REQUESTER = 0,
		/// <summary>
		///  公会商店
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会商店", " 公会商店")]
		GUILD_SHOP = 1,
		/// <summary>
		///  公会兼并
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会兼并", " 公会兼并")]
		GUILD_MERGER = 4,
		/// <summary>
		///  公会战领地每日奖励
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战领地每日奖励", " 公会战领地每日奖励")]
		GUILD_BATTLE_TERR_DAY_REWARD = 5,
	}

	/// <summary>
	///  场景obj类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 场景obj类型", " 场景obj类型")]
	public enum SceneObjectType
	{
		/// <summary>
		///  城镇怪物
		/// </summary>
		[AdvancedInspector.Descriptor(" 城镇怪物", " 城镇怪物")]
		SOT_CITYMONSTER = 9,
	}

	/// <summary>
	///  小怪类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 小怪类型", " 小怪类型")]
	public enum CityMonsterType
	{
		/// <summary>
		///  
		/// </summary>
		[AdvancedInspector.Descriptor(" ", " ")]
		CITY_MONSTER_INVALID = 0,
		/// <summary>
		/// 活动
		/// </summary>
		[AdvancedInspector.Descriptor("活动", "活动")]
		CITY_MONSTER_ACTIVITY = 1,
		/// <summary>
		/// 任务
		/// </summary>
		[AdvancedInspector.Descriptor("任务", "任务")]
		CITY_MONSTER_TASK = 2,
		/// <summary>
		/// 爬塔
		/// </summary>
		[AdvancedInspector.Descriptor("爬塔", "爬塔")]
		CITY_MONSTER_LOST_DUNGEON = 3,
	}

	/// <summary>
	///  获得经验原因
	/// </summary>
	[AdvancedInspector.Descriptor(" 获得经验原因", " 获得经验原因")]
	public enum PlayerIncExpReason
	{

		PIER_INVALID = 0,
		/// <summary>
		///  使用经验丸(经验是绝对值)
		/// </summary>
		[AdvancedInspector.Descriptor(" 使用经验丸(经验是绝对值)", " 使用经验丸(经验是绝对值)")]
		PIER_EXP_PILL_VALUE = 1,
		/// <summary>
		///  使用经验丸(经验是百分比)
		/// </summary>
		[AdvancedInspector.Descriptor(" 使用经验丸(经验是百分比)", " 使用经验丸(经验是百分比)")]
		PIER_EXP_PILL_PERCENT = 2,
	}

}
