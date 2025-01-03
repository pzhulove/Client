using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  �ͽ�����״̬
	/// </summary>
	[AdvancedInspector.Descriptor(" �ͽ�����״̬", " �ͽ�����״̬")]
	public enum PremiumLeagueStatus
	{
		/// <summary>
		///  ��ʼ״̬
		/// </summary>
		[AdvancedInspector.Descriptor(" ��ʼ״̬", " ��ʼ״̬")]
		PLS_INIT = 0,
		/// <summary>
		///  ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ����", " ����")]
		PLS_ENROLL = 1,
		/// <summary>
		///  Ԥ��
		/// </summary>
		[AdvancedInspector.Descriptor(" Ԥ��", " Ԥ��")]
		PLS_PRELIMINAY = 2,
		/// <summary>
		///  ��ǿ׼��
		/// </summary>
		[AdvancedInspector.Descriptor(" ��ǿ׼��", " ��ǿ׼��")]
		PLS_FINAL_EIGHT_PREPARE = 3,
		/// <summary>
		///  ��ǿ��
		/// </summary>
		[AdvancedInspector.Descriptor(" ��ǿ��", " ��ǿ��")]
		PLS_FINAL_EIGHT = 4,
		/// <summary>
		///  ��ǿ��
		/// </summary>
		[AdvancedInspector.Descriptor(" ��ǿ��", " ��ǿ��")]
		PLS_FINAL_FOUR = 5,
		/// <summary>
		///  ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ����", " ����")]
		PLS_FINAL = 6,
		/// <summary>
		///  ���������ȴ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ���������ȴ����", " ���������ȴ����")]
		PLS_FINAL_WAIT_CLEAR = 7,
	}

	/// <summary>
	///  �ͽ�������������
	/// </summary>
	[AdvancedInspector.Descriptor(" �ͽ�������������", " �ͽ�������������")]
	public enum PremiumLeagueRewardType
	{
		/// <summary>
		///  ��1���Ľ���
		/// </summary>
		[AdvancedInspector.Descriptor(" ��1���Ľ���", " ��1���Ľ���")]
		PL_REWARD_NO_1 = 0,
		/// <summary>
		///  ��2���Ľ���
		/// </summary>
		[AdvancedInspector.Descriptor(" ��2���Ľ���", " ��2���Ľ���")]
		PL_REWARD_NO_2 = 1,
		/// <summary>
		///  3-4���Ľ���
		/// </summary>
		[AdvancedInspector.Descriptor(" 3-4���Ľ���", " 3-4���Ľ���")]
		PL_REWARD_NO_3_4 = 2,
		/// <summary>
		///  5-8���Ľ���
		/// </summary>
		[AdvancedInspector.Descriptor(" 5-8���Ľ���", " 5-8���Ľ���")]
		PL_REWARD_NO_5_8 = 3,
		/// <summary>
		///  9-20���Ľ���
		/// </summary>
		[AdvancedInspector.Descriptor(" 9-20���Ľ���", " 9-20���Ľ���")]
		PL_REWARD_NO_9_20 = 4,
	}

}
