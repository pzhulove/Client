using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 竞拍返回
	/// </summary>
	[AdvancedInspector.Descriptor("竞拍返回", "竞拍返回")]
	public class WorldBlackMarketAuctionRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 609005;
		public UInt32 Sequence;
		/// <summary>
		/// 结果
		/// </summary>
		[AdvancedInspector.Descriptor("结果", "结果")]
		public UInt32 code;
		/// <summary>
		/// 竞拍的商品
		/// </summary>
		[AdvancedInspector.Descriptor("竞拍的商品", "竞拍的商品")]
		public BlackMarketAuctionInfo item = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			item.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			item.decode(buffer, ref pos_);
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
