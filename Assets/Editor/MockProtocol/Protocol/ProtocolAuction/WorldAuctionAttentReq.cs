using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->world ��ע
	/// </summary>
	[AdvancedInspector.Descriptor("client->world ��ע", "client->world ��ע")]
	public class WorldAuctionAttentReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603929;
		public UInt32 Sequence;
		/// <summary>
		/// ��������Ʒid
		/// </summary>
		[AdvancedInspector.Descriptor("��������Ʒid", "��������Ʒid")]
		public UInt64 autionGuid;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, autionGuid);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref autionGuid);
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
