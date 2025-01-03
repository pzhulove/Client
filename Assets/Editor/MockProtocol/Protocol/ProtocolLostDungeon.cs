using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 迷失地牢状态
	/// </summary>
	[AdvancedInspector.Descriptor("迷失地牢状态", "迷失地牢状态")]
	public enum LostDungeonState
	{

		LDS_CLOSE = 0,
		/// <summary>
		/// 关闭中
		/// </summary>
		[AdvancedInspector.Descriptor("关闭中", "关闭中")]
		LDS_PROGRESSING = 1,
		/// <summary>
		/// 进行中
		/// </summary>
		[AdvancedInspector.Descriptor("进行中", "进行中")]
		LDS_END_CAN_REWARD = 2,
	}

	/// <summary>
	/// 结束可领取奖励
	/// </summary>
	[AdvancedInspector.Descriptor("结束可领取奖励", "结束可领取奖励")]
	public enum LostDungeonPlayerBattleSt
	{

		LDPBT_NORMAL = 0,
		/// <summary>
		/// 非战斗
		/// </summary>
		[AdvancedInspector.Descriptor("非战斗", "非战斗")]
		LDPBT_BATTLE_PVE = 1,
		/// <summary>
		/// PVE战斗中
		/// </summary>
		[AdvancedInspector.Descriptor("PVE战斗中", "PVE战斗中")]
		LDPBT_BATTLE_PVP = 2,
	}


	public enum LostDungeonNodeState
	{

		LDNS_NONE = 0,

		LDNS_LOCK = 1,
		/// <summary>
		/// 封锁
		/// </summary>
		[AdvancedInspector.Descriptor("封锁", "封锁")]
		LDNS_CLOSE = 2,
		/// <summary>
		/// 关闭
		/// </summary>
		[AdvancedInspector.Descriptor("关闭", "关闭")]
		LDNS_OPEN = 3,
		/// <summary>
		/// 打开
		/// </summary>
		[AdvancedInspector.Descriptor("打开", "打开")]
		LDNS_HALF_OPEN = 4,
	}

	/// <summary>
	/// 半开
	/// </summary>
	[AdvancedInspector.Descriptor("半开", "半开")]
	public enum LostDungeonFloorState
	{

		LDFS_NONE = 0,
		/// <summary>
		/// 
		/// </summary>
		[AdvancedInspector.Descriptor("", "")]
		LDFS_LOCK = 1,
		/// <summary>
		/// 封锁
		/// </summary>
		[AdvancedInspector.Descriptor("封锁", "封锁")]
		LDFS_UNLOCK_UNPASS = 2,
		/// <summary>
		/// 解锁未通关
		/// </summary>
		[AdvancedInspector.Descriptor("解锁未通关", "解锁未通关")]
		LDFS_UNLOCK_PASS = 3,
	}

	/// <summary>
	/// 已通关
	/// </summary>
	[AdvancedInspector.Descriptor("已通关", "已通关")]
	public enum LostDungeonBoxState
	{

		LDBXS_NONE = 0,

		LDBXS_UNOPENED = 1,
		/// <summary>
		/// 未开过
		/// </summary>
		[AdvancedInspector.Descriptor("未开过", "未开过")]
		LDBXS_OPENED = 2,
	}

	/// <summary>
	/// 打开过
	/// </summary>
	/// <summary>
	/// 地下城队伍战斗状态
	/// </summary>
	[AdvancedInspector.Descriptor("地下城队伍战斗状态", "地下城队伍战斗状态")]
	public enum LostTeamBattleSt
	{

		LDTBS_NORMAL = 0,
		/// <summary>
		/// 非战斗状态
		/// </summary>
		[AdvancedInspector.Descriptor("非战斗状态", "非战斗状态")]
		LDTBS_MATCH = 1,
		/// <summary>
		/// 匹配中,（不一定成功）
		/// </summary>
		[AdvancedInspector.Descriptor("匹配中,（不一定成功）", "匹配中,（不一定成功）")]
		LDTBS_BATTLE = 2,
		/// <summary>
		/// 战斗状态
		/// </summary>
		[AdvancedInspector.Descriptor("战斗状态", "战斗状态")]
		LDTBS_MAX = 3,
	}

	/// <summary>
	/// 地下城调整模式
	/// </summary>
	[AdvancedInspector.Descriptor("地下城调整模式", "地下城调整模式")]
	public enum LostDungChangeMode
	{

		LDCM_SINGLE = 1,
		/// <summary>
		/// 单人
		/// </summary>
		[AdvancedInspector.Descriptor("单人", "单人")]
		LDCM_TEAM = 2,
	}

	/// <summary>
	/// 组队
	/// </summary>
	/// <summary>
	///  同步队伍信息类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 同步队伍信息类型", " 同步队伍信息类型")]
	public enum SyncDungeonEnterTeamInfoType
	{

		SDETIT_NONE = 0,

		SDETIT_ENTER_SYNC = 1,
		/// <summary>
		/// 进入时候同步
		/// </summary>
		[AdvancedInspector.Descriptor("进入时候同步", "进入时候同步")]
		SDETIT_ADD = 2,
		/// <summary>
		/// 添加队员
		/// </summary>
		[AdvancedInspector.Descriptor("添加队员", "添加队员")]
		SDETIT_LEAVE = 3,
		/// <summary>
		/// 队员离开
		/// </summary>
		[AdvancedInspector.Descriptor("队员离开", "队员离开")]
		SDETIT_UPDATE = 4,
		/// <summary>
		/// 刷新队员信息
		/// </summary>
		[AdvancedInspector.Descriptor("刷新队员信息", "刷新队员信息")]
		SDETIT_CHG_MODE = 5,
		/// <summary>
		/// 刷新挑战模式
		/// </summary>
		[AdvancedInspector.Descriptor("刷新挑战模式", "刷新挑战模式")]
		SDETIT_TEAM_STATE = 6,
		/// <summary>
		/// 刷新队伍状态
		/// </summary>
		[AdvancedInspector.Descriptor("刷新队伍状态", "刷新队伍状态")]
		SDETIT_MAX = 7,
	}


	public enum LostDungeonBattleReasult
	{

		LDBR_NONE = 0,

		LDBR_PASS = 1,
		/// <summary>
		/// 通关
		/// </summary>
		[AdvancedInspector.Descriptor("通关", "通关")]
		LDBR_FAIL = 2,
		/// <summary>
		/// 失败
		/// </summary>
		[AdvancedInspector.Descriptor("失败", "失败")]
		LDBR_OVER = 3,
	}

}
