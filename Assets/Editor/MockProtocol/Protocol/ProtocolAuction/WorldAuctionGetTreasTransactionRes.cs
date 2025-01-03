using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world->client 拉取珍品交易记录返回
	/// </summary>
	[AdvancedInspector.Descriptor("world->client 拉取珍品交易记录返回", "world->client 拉取珍品交易记录返回")]
	public class WorldAuctionGetTreasTransactionRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603925;
		public UInt32 Sequence;
		/// <summary>
		/// 寄售列表
		/// </summary>
		[AdvancedInspector.Descriptor("寄售列表", "寄售列表")]
		public AuctionTransaction[] sales = null;
		/// <summary>
		/// 购买列表
		/// </summary>
		[AdvancedInspector.Descriptor("购买列表", "购买列表")]
		public AuctionTransaction[] buys = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)sales.Length);
			for(int i = 0; i < sales.Length; i++)
			{
				sales[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)buys.Length);
			for(int i = 0; i < buys.Length; i++)
			{
				buys[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 salesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref salesCnt);
			sales = new AuctionTransaction[salesCnt];
			for(int i = 0; i < sales.Length; i++)
			{
				sales[i] = new AuctionTransaction();
				sales[i].decode(buffer, ref pos_);
			}
			UInt16 buysCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref buysCnt);
			buys = new AuctionTransaction[buysCnt];
			for(int i = 0; i < buys.Length; i++)
			{
				buys[i] = new AuctionTransaction();
				buys[i].decode(buffer, ref pos_);
			}
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
