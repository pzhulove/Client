using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// scene->client �쳣���׼�¼����
	/// </summary>
	[AdvancedInspector.Descriptor("scene->client �쳣���׼�¼����", "scene->client �쳣���׼�¼����")]
	public class SceneAbnormalTransactionRecordRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 503907;
		public UInt32 Sequence;
		/// <summary>
		///  ���������
		/// </summary>
		[AdvancedInspector.Descriptor(" ���������", " ���������")]
		public UInt32 frozenMoneyType;
		/// <summary>
		///  ������
		/// </summary>
		[AdvancedInspector.Descriptor(" ������", " ������")]
		public UInt32 frozenAmount;
		/// <summary>
		///  �쳣����ʱ��
		/// </summary>
		[AdvancedInspector.Descriptor(" �쳣����ʱ��", " �쳣����ʱ��")]
		public UInt32 abnormalTransactionTime;
		/// <summary>
		///  ������
		/// </summary>
		[AdvancedInspector.Descriptor(" ������", " ������")]
		public UInt32 backDeadline;
		/// <summary>
		///  �ѷ������
		/// </summary>
		[AdvancedInspector.Descriptor(" �ѷ������", " �ѷ������")]
		public UInt32 backAmount;
		/// <summary>
		///  �ѷ�����
		/// </summary>
		[AdvancedInspector.Descriptor(" �ѷ�����", " �ѷ�����")]
		public UInt32 backDays;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, frozenMoneyType);
			BaseDLL.encode_uint32(buffer, ref pos_, frozenAmount);
			BaseDLL.encode_uint32(buffer, ref pos_, abnormalTransactionTime);
			BaseDLL.encode_uint32(buffer, ref pos_, backDeadline);
			BaseDLL.encode_uint32(buffer, ref pos_, backAmount);
			BaseDLL.encode_uint32(buffer, ref pos_, backDays);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref frozenMoneyType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref frozenAmount);
			BaseDLL.decode_uint32(buffer, ref pos_, ref abnormalTransactionTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref backDeadline);
			BaseDLL.decode_uint32(buffer, ref pos_, ref backAmount);
			BaseDLL.decode_uint32(buffer, ref pos_, ref backDays);
		}

		public UInt32 GetSequence()
		{
			return Sequence;
		}

		public void SetSequence(UInt32 sequence)
		{
			Sequence = sequence;
		}

		#endregion

	}

}
