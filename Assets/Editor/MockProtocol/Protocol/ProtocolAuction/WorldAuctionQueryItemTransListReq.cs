using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->world 查询道具拍卖行交易记录请求
	/// </summary>
	[AdvancedInspector.Descriptor("client->world 查询道具拍卖行交易记录请求", "client->world 查询道具拍卖行交易记录请求")]
	public class WorldAuctionQueryItemTransListReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603935;
		public UInt32 Sequence;

		public UInt32 itemTypeId;

		public UInt32 strengthen;

		public byte enhanceType;

		public UInt32 beadBuffId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
			BaseDLL.encode_uint32(buffer, ref pos_, strengthen);
			BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
			BaseDLL.encode_uint32(buffer, ref pos_, beadBuffId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref strengthen);
			BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref beadBuffId);
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
