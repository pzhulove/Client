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
		/// ʦ����ȡ
		/// </summary>
		[AdvancedInspector.Descriptor("ʦ����ȡ", "ʦ����ȡ")]
		MARDTR_DISCIPLE = 2,
	}

	/// <summary>
	/// ͽ����ȡ
	/// </summary>
	[AdvancedInspector.Descriptor("ͽ����ȡ", "ͽ����ȡ")]
	public enum MasterAcademicRewardRecvState
	{

		MARRS_NOTRECVED = 0,
		/// <summary>
		/// û����ȡ
		/// </summary>
		[AdvancedInspector.Descriptor("û����ȡ", "û����ȡ")]
		MARRS_RECVED = 1,
	}

	/// <summary>
	/// �Ѿ���ȡ
	/// </summary>
	[AdvancedInspector.Descriptor("�Ѿ���ȡ", "�Ѿ���ȡ")]
	public enum MasterDailyTaskState
	{

		MDTST_UNPUBLISHED = 0,
		/// <summary>
		/// δ����
		/// </summary>
		[AdvancedInspector.Descriptor("δ����", "δ����")]
		MDTST_UNREPORTED = 1,
		/// <summary>
		/// �ѷ�����δ�㱨
		/// </summary>
		[AdvancedInspector.Descriptor("�ѷ�����δ�㱨", "�ѷ�����δ�㱨")]
		MDTST_UNRECEIVED = 2,
		/// <summary>
		/// �ѻ㱨������ȡ
		/// </summary>
		[AdvancedInspector.Descriptor("�ѻ㱨������ȡ", "�ѻ㱨������ȡ")]
		MDTST_RECEIVEED = 3,
		/// <summary>
		/// �ѻ㱨������ȡ
		/// </summary>
		[AdvancedInspector.Descriptor("�ѻ㱨������ȡ", "�ѻ㱨������ȡ")]
		MDTST_UNPUB_UNRECE = 4,
	}

	/// <summary>
	/// δ����������ȡ ʦ���ſ��������״̬
	/// </summary>
	[AdvancedInspector.Descriptor("δ����������ȡ ʦ���ſ��������״̬", "δ����������ȡ ʦ���ſ��������״̬")]
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
