using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  活动状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 活动状态", " 活动状态")]
	public enum StateType
	{
		/// <summary>
		///  结束
		/// </summary>
		[AdvancedInspector.Descriptor(" 结束", " 结束")]
		End = 0,
		/// <summary>
		///  进行中
		/// </summary>
		[AdvancedInspector.Descriptor(" 进行中", " 进行中")]
		Running = 1,
		/// <summary>
		///  准备中
		/// </summary>
		[AdvancedInspector.Descriptor(" 准备中", " 准备中")]
		Ready = 2,
	}

	/// <summary>
	///  通知类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 通知类型", " 通知类型")]
	public enum NotifyType
	{

		NT_NONE = 0,
		/// <summary>
		///  公会战
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战", " 公会战")]
		NT_GUILD_BATTLE = 1,
		/// <summary>
		///  武道大会 		
		/// </summary>
		[AdvancedInspector.Descriptor(" 武道大会 		", " 武道大会 		")]
		NT_BUDO = 2,
		/// <summary>
		/// 罐子开放				
		/// </summary>
		[AdvancedInspector.Descriptor("罐子开放				", "罐子开放				")]
		NT_JAR_OPEN = 3,
		/// <summary>
		/// 罐子折扣重置			
		/// </summary>
		[AdvancedInspector.Descriptor("罐子折扣重置			", "罐子折扣重置			")]
		NT_JAR_SALE_RESET = 4,
		/// <summary>
		/// 时限道具
		/// </summary>
		[AdvancedInspector.Descriptor("时限道具", "时限道具")]
		NT_TIME_ITEM = 5,
		/// <summary>
		/// 赏金联赛
		/// </summary>
		[AdvancedInspector.Descriptor("赏金联赛", "赏金联赛")]
		NT_MONEY_REWARDS = 6,
		/// <summary>
		///  魔罐积分清空
		/// </summary>
		[AdvancedInspector.Descriptor(" 魔罐积分清空", " 魔罐积分清空")]
		NT_MAGIC_INTEGRAL_EMPTYING = 7,
		/// <summary>
		///  公会副本
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会副本", " 公会副本")]
		NT_GUILD_DUNGEON = 8,
		/// <summary>
		///  月卡奖励暂存箱不足24h提示
		/// </summary>
		[AdvancedInspector.Descriptor(" 月卡奖励暂存箱不足24h提示", " 月卡奖励暂存箱不足24h提示")]
		NT_MONTH_CARD_REWARD_EXPIRE_24H = 9,
		/// <summary>
		/// 佣兵团赏金即将重置
		/// </summary>
		[AdvancedInspector.Descriptor("佣兵团赏金即将重置", "佣兵团赏金即将重置")]
		NT_ADVENTURE_TEAM_BOUNTY_EMPTYING = 10,
		/// <summary>
		/// 佣兵团成长药剂即将重置
		/// </summary>
		[AdvancedInspector.Descriptor("佣兵团成长药剂即将重置", "佣兵团成长药剂即将重置")]
		NT_ADVENTURE_TEAM_INHERIT_BLESS_EMPTYING = 11,
		/// <summary>
		/// 冒险通行证代币重置
		/// </summary>
		[AdvancedInspector.Descriptor("冒险通行证代币重置", "冒险通行证代币重置")]
		NT_ADVENTURE_PASS_CARD_COIN_EMPTYING = 12,

		NT_MAX = 13,
	}

}
