using System;
using System.Text;

namespace Mock.Protocol
{

	public enum TaskPublishType
	{

		TASK_PUBLISH_NPC = 0,

		TASK_PUBLISH_UI = 1,

		TASK_PUBLISH_CITY = 2,
	}


	public enum TaskSubmitType
	{

		TASK_SUBMIT_AUTO = 0,

		TASK_SUBMIT_NPC = 1,

		TASK_SUBMIT_UI = 2,

		TASK_SUBMIT_RIGHTNOW = 3,
	}


	public enum TaskStatus
	{

		TASK_INIT = 0,

		TASK_UNFINISH = 1,

		TASK_FINISHED = 2,

		TASK_FAILED = 3,

		TASK_SUBMITTING = 4,

		TASK_OVER = 5,
	}


	public enum DeleteTaskReason
	{

		DELETE_TASK_REASON_SUBMIT = 1,

		DELETE_TASK_REASON_ABANDON = 2,

		DELETE_TASK_REASON_SYSTEM = 3,

		DELETE_TASK_REASON_OTHER = 4,
	}


	public enum MasterSysReceiveDailyTaskRewardType
	{

		MSRDTR_MASTER = 1,
		/// <summary>
		/// 师傅领取
		/// </summary>
		[AdvancedInspector.Descriptor("师傅领取", "师傅领取")]
		MARDTR_DISCIPLE = 2,
	}

	/// <summary>
	/// 徒弟领取
	/// </summary>
	[AdvancedInspector.Descriptor("徒弟领取", "徒弟领取")]
	public enum MasterAcademicRewardRecvState
	{

		MARRS_NOTRECVED = 0,
		/// <summary>
		/// 没有领取
		/// </summary>
		[AdvancedInspector.Descriptor("没有领取", "没有领取")]
		MARRS_RECVED = 1,
	}

	/// <summary>
	/// 已经领取
	/// </summary>
	[AdvancedInspector.Descriptor("已经领取", "已经领取")]
	public enum MasterDailyTaskState
	{

		MDTST_UNPUBLISHED = 0,
		/// <summary>
		/// 未发布
		/// </summary>
		[AdvancedInspector.Descriptor("未发布", "未发布")]
		MDTST_UNREPORTED = 1,
		/// <summary>
		/// 已发布，未汇报
		/// </summary>
		[AdvancedInspector.Descriptor("已发布，未汇报", "已发布，未汇报")]
		MDTST_UNRECEIVED = 2,
		/// <summary>
		/// 已汇报，可领取
		/// </summary>
		[AdvancedInspector.Descriptor("已汇报，可领取", "已汇报，可领取")]
		MDTST_RECEIVEED = 3,
		/// <summary>
		/// 已汇报，已领取
		/// </summary>
		[AdvancedInspector.Descriptor("已汇报，已领取", "已汇报，已领取")]
		MDTST_UNPUB_UNRECE = 4,
	}

	/// <summary>
	/// 未发布，可领取 师傅才可能有这个状态
	/// </summary>
	[AdvancedInspector.Descriptor("未发布，可领取 师傅才可能有这个状态", "未发布，可领取 师傅才可能有这个状态")]
	public enum AdventureTeamTaskDifficult
	{

		C = 0,

		B = 1,

		A = 2,

		S = 3,

		SS = 4,

		SSS = 5,
	}

}
