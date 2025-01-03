using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  �ͽ�����״̬��Ϣ
	/// </summary>
	[AdvancedInspector.Descriptor(" �ͽ�����״̬��Ϣ", " �ͽ�����״̬��Ϣ")]
	public class PremiumLeagueStatusInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  ״̬����ӦPremiumLeagueStatus��
		/// </summary>
		[AdvancedInspector.Descriptor(" ״̬����ӦPremiumLeagueStatus��", " ״̬����ӦPremiumLeagueStatus��")]
		public byte status;
		/// <summary>
		///  ��ʼʱ��
		/// </summary>
		[AdvancedInspector.Descriptor(" ��ʼʱ��", " ��ʼʱ��")]
		public UInt32 startTime;
		/// <summary>
		///  ����ʱ��
		/// </summary>
		[AdvancedInspector.Descriptor(" ����ʱ��", " ����ʱ��")]
		public UInt32 endTime;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, status);
			BaseDLL.encode_uint32(buffer, ref pos_, startTime);
			BaseDLL.encode_uint32(buffer, ref pos_, endTime);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref status);
			BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
		}


		#endregion

	}

}
