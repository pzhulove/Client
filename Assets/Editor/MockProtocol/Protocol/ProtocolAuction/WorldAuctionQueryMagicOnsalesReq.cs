using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->world 查询拍卖行指定附魔卡各等级数量请求
	/// </summary>
	[AdvancedInspector.Descriptor("client->world 查询拍卖行指定附魔卡各等级数量请求", "client->world 查询拍卖行指定附魔卡各等级数量请求")]
	public class WorldAuctionQueryMagicOnsalesReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603933;
		public UInt32 Sequence;

		public UInt32 itemTypeId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
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
