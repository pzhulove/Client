using System;
using System.Text;

namespace Mock.Protocol
{

	public enum TeamCopyTeamModel
	{
		/// <summary>
		///  默认全部模式
		/// </summary>
		[AdvancedInspector.Descriptor(" 默认全部模式", " 默认全部模式")]
		TEAM_COPY_TEAM_MODEL_DEFAULT = 0,
		/// <summary>
		///  挑战模式
		/// </summary>
		[AdvancedInspector.Descriptor(" 挑战模式", " 挑战模式")]
		TEAM_COPY_TEAM_MODEL_CHALLENGE = 1,
		/// <summary>
		///  金团模式
		/// </summary>
		[AdvancedInspector.Descriptor(" 金团模式", " 金团模式")]
		TEAM_COPY_TEAM_MODEL_GOLD = 2,
	}

	/// <summary>
	///  据点状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 据点状态", " 据点状态")]
	public enum TeamCopyFieldStatus
	{
		/// <summary>
		///  无效状态
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效状态", " 无效状态")]
		TEAM_COPY_FIELD_STATUS_INVALID = 0,
		/// <summary>
		///  初始状态
		/// </summary>
		[AdvancedInspector.Descriptor(" 初始状态", " 初始状态")]
		TEAM_COPY_FIELD_STATUS_INIT = 1,
		/// <summary>
		///  重生
		/// </summary>
		[AdvancedInspector.Descriptor(" 重生", " 重生")]
		TEAM_COPY_FIELD_STATUS_REBORN = 2,
		/// <summary>
		///  歼灭
		/// </summary>
		[AdvancedInspector.Descriptor(" 歼灭", " 歼灭")]
		TEAM_COPY_FIELD_STATUS_DEFEAT = 3,
		/// <summary>
		///  紧急
		/// </summary>
		[AdvancedInspector.Descriptor(" 紧急", " 紧急")]
		TEAM_COPY_FIELD_STATUS_URGENT = 4,
		/// <summary>
		///  解锁中
		/// </summary>
		[AdvancedInspector.Descriptor(" 解锁中", " 解锁中")]
		TEAM_COPY_FIELD_STATUS_UNLOCKING = 5,
		/// <summary>
		///  能量恢复中
		/// </summary>
		[AdvancedInspector.Descriptor(" 能量恢复中", " 能量恢复中")]
		TEAM_COPY_FIELD_STATUS_ENERGY_REVIVE = 6,
	}

	/// <summary>
	///  战前配置模式
	/// </summary>
	[AdvancedInspector.Descriptor(" 战前配置模式", " 战前配置模式")]
	public enum TeamCopyPlanModel
	{
		/// <summary>
		///  无效
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效", " 无效")]
		TEAM_COPY_PLAN_MODEL_INVALID = 0,
		/// <summary>
		///  自由模式
		/// </summary>
		[AdvancedInspector.Descriptor(" 自由模式", " 自由模式")]
		TEAM_COPY_PLAN_MODEL_FREE = 1,
		/// <summary>
		///  引导模式
		/// </summary>
		[AdvancedInspector.Descriptor(" 引导模式", " 引导模式")]
		TEAM_COPY_PLAN_MODEL_GUIDE = 2,
	}

	/// <summary>
	///  小队难度
	/// </summary>
	[AdvancedInspector.Descriptor(" 小队难度", " 小队难度")]
	public enum TeamCopyGrade
	{
		/// <summary>
		///  团队难度
		/// </summary>
		[AdvancedInspector.Descriptor(" 团队难度", " 团队难度")]
		TEAM_COPY_GRADE_TEAM = 0,
		/// <summary>
		///  A难度
		/// </summary>
		[AdvancedInspector.Descriptor(" A难度", " A难度")]
		TEAM_COPY_GRADE_A = 1,
		/// <summary>
		///  B难度
		/// </summary>
		[AdvancedInspector.Descriptor(" B难度", " B难度")]
		TEAM_COPY_GRADE_B = 2,
		/// <summary>
		///  C难度
		/// </summary>
		[AdvancedInspector.Descriptor(" C难度", " C难度")]
		TEAM_COPY_GRADE_C = 3,
		/// <summary>
		///  D难度
		/// </summary>
		[AdvancedInspector.Descriptor(" D难度", " D难度")]
		TEAM_COPY_GRADE_D = 4,
	}

	/// <summary>
	///  阶段
	/// </summary>
	[AdvancedInspector.Descriptor(" 阶段", " 阶段")]
	public enum TeamCopyStage
	{
		/// <summary>
		///  空阶段
		/// </summary>
		[AdvancedInspector.Descriptor(" 空阶段", " 空阶段")]
		TEAM_COPY_STAGE_NULL = 0,
		/// <summary>
		///  第一阶段
		/// </summary>
		[AdvancedInspector.Descriptor(" 第一阶段", " 第一阶段")]
		TEAM_COPY_STAGE_ONE = 1,
		/// <summary>
		///  第二阶段
		/// </summary>
		[AdvancedInspector.Descriptor(" 第二阶段", " 第二阶段")]
		TEAM_COPY_STAGE_TWO = 2,
		/// <summary>
		///  最终阶段
		/// </summary>
		[AdvancedInspector.Descriptor(" 最终阶段", " 最终阶段")]
		TEAM_COPY_STAGE_FINAL = 3,
	}

	/// <summary>
	///  目标
	/// </summary>
	[AdvancedInspector.Descriptor(" 目标", " 目标")]
	public enum TeamCopyTargetType
	{
		/// <summary>
		///  团队目标
		/// </summary>
		[AdvancedInspector.Descriptor(" 团队目标", " 团队目标")]
		TEAM_COPY_TARGET_TYPE_TEAM = 1,
		/// <summary>
		///  小队目标
		/// </summary>
		[AdvancedInspector.Descriptor(" 小队目标", " 小队目标")]
		TEAM_COPY_TARGET_TYPE_SQUAD = 2,
	}

	/// <summary>
	///  队伍状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 队伍状态", " 队伍状态")]
	public enum TeamCopyTeamStatus
	{
		/// <summary>
		///  备战
		/// </summary>
		[AdvancedInspector.Descriptor(" 备战", " 备战")]
		TEAM_COPY_TEAM_STATUS_PARPARE = 0,
		/// <summary>
		///  战斗
		/// </summary>
		[AdvancedInspector.Descriptor(" 战斗", " 战斗")]
		TEAM_COPY_TEAM_STATUS_BATTLE = 1,
		/// <summary>
		///  胜利
		/// </summary>
		[AdvancedInspector.Descriptor(" 胜利", " 胜利")]
		TEAM_COPY_TEAM_STATUS_VICTORY = 2,
		/// <summary>
		///  失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 失败", " 失败")]
		TEAM_COPY_TEAM_STATUS_FAILED = 3,
	}

	/// <summary>
	///  小队状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 小队状态", " 小队状态")]
	public enum TeamCopySquadStatus
	{
		/// <summary>
		///  待命中
		/// </summary>
		[AdvancedInspector.Descriptor(" 待命中", " 待命中")]
		TEAM_COPY_SQUAD_STATUS_INIT = 0,
		/// <summary>
		///  备战中
		/// </summary>
		[AdvancedInspector.Descriptor(" 备战中", " 备战中")]
		TEAM_COPY_SQUAD_STATUS_PREPARE = 1,
		/// <summary>
		///  战斗中
		/// </summary>
		[AdvancedInspector.Descriptor(" 战斗中", " 战斗中")]
		TEAM_COPY_SQUAD_STATUS_BATTLE = 2,
	}

	/// <summary>
	///  职位(按位操作)
	/// </summary>
	[AdvancedInspector.Descriptor(" 职位(按位操作)", " 职位(按位操作)")]
	public enum TeamCopyPost
	{
		/// <summary>
		///  队员
		/// </summary>
		[AdvancedInspector.Descriptor(" 队员", " 队员")]
		TEAM_COPY_POST_NORMAL = 1,
		/// <summary>
		///  金主
		/// </summary>
		[AdvancedInspector.Descriptor(" 金主", " 金主")]
		TEAM_COPY_POST_GOLD = 2,
		/// <summary>
		///  队长
		/// </summary>
		[AdvancedInspector.Descriptor(" 队长", " 队长")]
		TEAM_COPY_POST_CAPTAIN = 4,
		/// <summary>
		///  团长
		/// </summary>
		[AdvancedInspector.Descriptor(" 团长", " 团长")]
		TEAM_COPY_POST_CHIEF = 8,
	}

	/// <summary>
	///  翻牌限制类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 翻牌限制类型", " 翻牌限制类型")]
	public enum TeamCopyFlopLimit
	{
		/// <summary>
		///  不限制
		/// </summary>
		[AdvancedInspector.Descriptor(" 不限制", " 不限制")]
		TEAM_COPY_FLOP_LIMIT_NULL = 0,
		/// <summary>
		///  日限制
		/// </summary>
		[AdvancedInspector.Descriptor(" 日限制", " 日限制")]
		TEAM_COPY_FLOP_LIMIT_DAY = 1,
		/// <summary>
		///  周限制
		/// </summary>
		[AdvancedInspector.Descriptor(" 周限制", " 周限制")]
		TEAM_COPY_FLOP_LIMIT_WEEK = 2,
		/// <summary>
		///  通关限制
		/// </summary>
		[AdvancedInspector.Descriptor(" 通关限制", " 通关限制")]
		TEAM_COPY_FLOP_LIMIT_PASS_GATE = 3,
	}

	/// <summary>
	///  难度
	/// </summary>
	[AdvancedInspector.Descriptor(" 难度", " 难度")]
	public enum TeamCopyTeamGrade
	{
		/// <summary>
		///  普通难度
		/// </summary>
		[AdvancedInspector.Descriptor(" 普通难度", " 普通难度")]
		TEAM_COPY_TEAM_GRADE_COMMON = 1,
		/// <summary>
		///  困难难度
		/// </summary>
		[AdvancedInspector.Descriptor(" 困难难度", " 困难难度")]
		TEAM_COPY_TEAM_GRADE_DIFF = 2,
	}

}
