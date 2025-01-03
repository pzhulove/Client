using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  �ͽ�������̭��
	/// </summary>
	[AdvancedInspector.Descriptor(" �ͽ�������̭��", " �ͽ�������̭��")]
	public class CLPremiumLeagueBattle : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  ����ID
		/// </summary>
		[AdvancedInspector.Descriptor(" ����ID", " ����ID")]
		public UInt64 raceId;
		/// <summary>
		///  ��������
		/// </summary>
		[AdvancedInspector.Descriptor(" ��������", " ��������")]
		public byte type;
		/// <summary>
		///  ��Ա1
		/// </summary>
		[AdvancedInspector.Descriptor(" ��Ա1", " ��Ա1")]
		public PremiumLeagueBattleFighter fighter1 = null;
		/// <summary>
		///  ��Ա2
		/// </summary>
		[AdvancedInspector.Descriptor(" ��Ա2", " ��Ա2")]
		public PremiumLeagueBattleFighter fighter2 = null;
		/// <summary>
		///  �Ƿ��Ѿ�������
		/// </summary>
		[AdvancedInspector.Descriptor(" �Ƿ��Ѿ�������", " �Ƿ��Ѿ�������")]
		public byte isEnd;
		/// <summary>
		///  ʤ��ID
		/// </summary>
		[AdvancedInspector.Descriptor(" ʤ��ID", " ʤ��ID")]
		public UInt64 winnerId;
		/// <summary>
		///  relay��ַ
		/// </summary>
		[AdvancedInspector.Descriptor(" relay��ַ", " relay��ַ")]
		public SockAddr relayAddr = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, raceId);
			BaseDLL.encode_int8(buffer, ref pos_, type);
			fighter1.encode(buffer, ref pos_);
			fighter2.encode(buffer, ref pos_);
			BaseDLL.encode_int8(buffer, ref pos_, isEnd);
			BaseDLL.encode_uint64(buffer, ref pos_, winnerId);
			relayAddr.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref raceId);
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			fighter1.decode(buffer, ref pos_);
			fighter2.decode(buffer, ref pos_);
			BaseDLL.decode_int8(buffer, ref pos_, ref isEnd);
			BaseDLL.decode_uint64(buffer, ref pos_, ref winnerId);
			relayAddr.decode(buffer, ref pos_);
		}


		#endregion

	}

}
