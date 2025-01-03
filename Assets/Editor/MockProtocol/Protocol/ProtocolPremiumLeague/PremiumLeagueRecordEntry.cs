using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  ս����¼
	/// </summary>
	[AdvancedInspector.Descriptor(" ս����¼", " ս����¼")]
	public class PremiumLeagueRecordEntry : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  ���
		/// </summary>
		[AdvancedInspector.Descriptor(" ���", " ���")]
		public UInt32 index;
		/// <summary>
		///  ʱ��
		/// </summary>
		[AdvancedInspector.Descriptor(" ʱ��", " ʱ��")]
		public UInt32 time;
		/// <summary>
		///  ʤ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ʤ����", " ʤ����")]
		public PremiumLeagueRecordFighter winner = null;
		/// <summary>
		///  ʧ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ʧ����", " ʧ����")]
		public PremiumLeagueRecordFighter loser = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, index);
			BaseDLL.encode_uint32(buffer, ref pos_, time);
			winner.encode(buffer, ref pos_);
			loser.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref index);
			BaseDLL.decode_uint32(buffer, ref pos_, ref time);
			winner.decode(buffer, ref pos_);
			loser.decode(buffer, ref pos_);
		}


		#endregion

	}

}
