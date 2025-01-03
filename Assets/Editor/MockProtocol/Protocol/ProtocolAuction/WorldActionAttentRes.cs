using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world->client ��ע����
	/// </summary>
	[AdvancedInspector.Descriptor("world->client ��ע����", "world->client ��ע����")]
	public class WorldActionAttentRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603930;
		public UInt32 Sequence;
		/// <summary>
		/// ���
		/// </summary>
		[AdvancedInspector.Descriptor("���", "���")]
		public UInt32 code;
		/// <summary>
		/// ��������Ʒ��Ϣ
		/// </summary>
		[AdvancedInspector.Descriptor("��������Ʒ��Ϣ", "��������Ʒ��Ϣ")]
		public AuctionBaseInfo aution = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			aution.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			aution.decode(buffer, ref pos_);
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
