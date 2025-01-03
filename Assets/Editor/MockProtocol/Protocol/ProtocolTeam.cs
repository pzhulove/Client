using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  队伍目标类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 队伍目标类型", " 队伍目标类型")]
	public enum TeamTargetType
	{
		/// <summary>
		///  地下城
		/// </summary>
		[AdvancedInspector.Descriptor(" 地下城", " 地下城")]
		Dungeon = 0,
	}

	/// <summary>
	///  队员属性
	/// </summary>
	[AdvancedInspector.Descriptor(" 队员属性", " 队员属性")]
	public enum TeamMemberProperty
	{
		/// <summary>
		///  等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 等级", " 等级")]
		Level = 0,
		/// <summary>
		///  公会ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会ID", " 公会ID")]
		GuildID = 1,
		/// <summary>
		///  剩余次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 剩余次数", " 剩余次数")]
		RemainTimes = 2,
		/// <summary>
		///  职业
		/// </summary>
		[AdvancedInspector.Descriptor(" 职业", " 职业")]
		Occu = 3,
		/// <summary>
		///  状态
		/// </summary>
		[AdvancedInspector.Descriptor(" 状态", " 状态")]
		StatusMask = 4,
		/// <summary>
		///  vip等级
		/// </summary>
		[AdvancedInspector.Descriptor(" vip等级", " vip等级")]
		VipLevel = 5,
		/// <summary>
		///  抗磨值
		/// </summary>
		[AdvancedInspector.Descriptor(" 抗磨值", " 抗磨值")]
		ResistMagic = 6,
		/// <summary>
		/// 玩家标签信息
		/// </summary>
		[AdvancedInspector.Descriptor("玩家标签信息", "玩家标签信息")]
		PlayerLabelInfo = 7,
	}

	/// <summary>
	///  成员状态掩码
	/// </summary>
	[AdvancedInspector.Descriptor(" 成员状态掩码", " 成员状态掩码")]
	public enum TeamMemberStatusMask
	{
		/// <summary>
		///  是否在线
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否在线", " 是否在线")]
		Online = 1,
		/// <summary>
		///  准备
		/// </summary>
		[AdvancedInspector.Descriptor(" 准备", " 准备")]
		Ready = 2,
		/// <summary>
		///  助战
		/// </summary>
		[AdvancedInspector.Descriptor(" 助战", " 助战")]
		Assist = 4,
		/// <summary>
		///  是否在战斗中
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否在战斗中", " 是否在战斗中")]
		Racing = 8,
	}

	/// <summary>
	///  队伍选项
	/// </summary>
	[AdvancedInspector.Descriptor(" 队伍选项", " 队伍选项")]
	public enum TeamOption
	{
		/// <summary>
		///  深渊自动关闭
		/// </summary>
		[AdvancedInspector.Descriptor(" 深渊自动关闭", " 深渊自动关闭")]
		HellAutoClose = 1,
	}

	/// <summary>
	///  队伍选项修改类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 队伍选项修改类型", " 队伍选项修改类型")]
	public enum TeamOptionOperType
	{
		/// <summary>
		///  目标
		/// </summary>
		[AdvancedInspector.Descriptor(" 目标", " 目标")]
		Target = 0,
		/// <summary>
		///  自动同意
		/// </summary>
		[AdvancedInspector.Descriptor(" 自动同意", " 自动同意")]
		AutoAgree = 1,
		/// <summary>
		///  深渊自动关闭功能
		/// </summary>
		[AdvancedInspector.Descriptor(" 深渊自动关闭功能", " 深渊自动关闭功能")]
		HellAutoClose = 2,
	}

}
