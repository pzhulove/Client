using System;
using System.Text;

namespace Mock.Protocol
{

	public enum RedPacketType
	{
		/// <summary>
		///  ������
		/// </summary>
		[AdvancedInspector.Descriptor(" ������", " ������")]
		GUILD = 1,
		/// <summary>
		///  ������
		/// </summary>
		[AdvancedInspector.Descriptor(" ������", " ������")]
		NEW_YEAR = 2,
	}

	/// <summary>
	///  ���״̬
	/// </summary>
	[AdvancedInspector.Descriptor(" ���״̬", " ���״̬")]
	public enum RedPacketStatus
	{
		/// <summary>
		///  ��ʼ״̬
		/// </summary>
		[AdvancedInspector.Descriptor(" ��ʼ״̬", " ��ʼ״̬")]
		INIT = 0,
		/// <summary>
		///  δ���
		/// </summary>
		[AdvancedInspector.Descriptor(" δ���", " δ���")]
		UNSATISFY = 1,
		/// <summary>
		///  �ȴ�������ȡ���
		/// </summary>
		[AdvancedInspector.Descriptor(" �ȴ�������ȡ���", " �ȴ�������ȡ���")]
		WAIT_RECEIVE = 2,
		/// <summary>
		///  ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ����", " ����")]
		RECEIVED = 3,
		/// <summary>
		///  �ѱ�����
		/// </summary>
		[AdvancedInspector.Descriptor(" �ѱ�����", " �ѱ�����")]
		EMPTY = 4,
		/// <summary>
		///  �ɴݻ�
		/// </summary>
		[AdvancedInspector.Descriptor(" �ɴݻ�", " �ɴݻ�")]
		DESTORY = 5,
	}

}
