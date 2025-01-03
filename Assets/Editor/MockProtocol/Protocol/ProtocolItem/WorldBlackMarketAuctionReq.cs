using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 请求竞拍
	/// </summary>
	[AdvancedInspector.Descriptor("请求竞拍", "请求竞拍")]
	public class WorldBlackMarketAuctionReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 609004;
		public UInt32 Sequence;
		/// <summary>
		/// 竞拍item项guid
		/// </summary>
		[AdvancedInspector.Descriptor("竞拍item项guid", "竞拍item项guid")]
		public UInt64 auction_guid;
		/// <summary>
		/// 装备id
		/// </summary>
		[AdvancedInspector.Descriptor("装备id", "装备id")]
		public UInt64 item_guid;
		/// <summary>
		/// 竞拍价格
		/// </summary>
		[AdvancedInspector.Descriptor("竞拍价格", "竞拍价格")]
		public UInt32 auction_price;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, auction_guid);
			BaseDLL.encode_uint64(buffer, ref pos_, item_guid);
			BaseDLL.encode_uint32(buffer, ref pos_, auction_price);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref auction_guid);
			BaseDLL.decode_uint64(buffer, ref pos_, ref item_guid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref auction_price);
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
