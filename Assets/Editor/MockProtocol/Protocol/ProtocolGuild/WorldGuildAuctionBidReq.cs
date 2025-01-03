using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  拍卖行出价请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 拍卖行出价请求", " 拍卖行出价请求")]
	public class WorldGuildAuctionBidReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608515;
		public UInt32 Sequence;
		/// <summary>
		///  guid
		/// </summary>
		[AdvancedInspector.Descriptor(" guid", " guid")]
		public UInt64 guid;
		/// <summary>
		///  出价
		/// </summary>
		[AdvancedInspector.Descriptor(" 出价", " 出价")]
		public UInt32 bidPrice;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, guid);
			BaseDLL.encode_uint32(buffer, ref pos_, bidPrice);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref bidPrice);
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
