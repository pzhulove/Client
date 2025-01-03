using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 取消竞拍请求
	/// </summary>
	[AdvancedInspector.Descriptor("取消竞拍请求", "取消竞拍请求")]
	public class WorldBlackMarketAuctionCancelReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 609006;
		public UInt32 Sequence;
		/// <summary>
		/// 竞拍item项guid
		/// </summary>
		[AdvancedInspector.Descriptor("竞拍item项guid", "竞拍item项guid")]
		public UInt64 auction_guid;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, auction_guid);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref auction_guid);
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
