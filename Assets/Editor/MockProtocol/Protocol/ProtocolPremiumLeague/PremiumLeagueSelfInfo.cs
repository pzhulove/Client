using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  �Լ����ͽ�������Ϣ
	/// </summary>
	[AdvancedInspector.Descriptor(" �Լ����ͽ�������Ϣ", " �Լ����ͽ�������Ϣ")]
	public class PremiumLeagueSelfInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  ʤ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ʤ����", " ʤ����")]
		public UInt32 winNum;
		/// <summary>
		///  ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ����", " ����")]
		public UInt32 score;
		/// <summary>
		///  ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ����", " ����")]
		public UInt32 ranking;
		/// <summary>
		///  ��������
		/// </summary>
		[AdvancedInspector.Descriptor(" ��������", " ��������")]
		public UInt32 enrollCount;
		/// <summary>
		///  ������
		/// </summary>
		[AdvancedInspector.Descriptor(" ������", " ������")]
		public UInt32 loseNum;
		/// <summary>
		///  Ԥѡ����õĽ���
		/// </summary>
		[AdvancedInspector.Descriptor(" Ԥѡ����õĽ���", " Ԥѡ����õĽ���")]
		public UInt32 preliminayRewardNum;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, winNum);
			BaseDLL.encode_uint32(buffer, ref pos_, score);
			BaseDLL.encode_uint32(buffer, ref pos_, ranking);
			BaseDLL.encode_uint32(buffer, ref pos_, enrollCount);
			BaseDLL.encode_uint32(buffer, ref pos_, loseNum);
			BaseDLL.encode_uint32(buffer, ref pos_, preliminayRewardNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref winNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref score);
			BaseDLL.decode_uint32(buffer, ref pos_, ref ranking);
			BaseDLL.decode_uint32(buffer, ref pos_, ref enrollCount);
			BaseDLL.decode_uint32(buffer, ref pos_, ref loseNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref preliminayRewardNum);
		}


		#endregion

	}

}
