using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  返回拍卖行道具数量
	/// </summary>
	[AdvancedInspector.Descriptor(" 返回拍卖行道具数量", " 返回拍卖行道具数量")]
	public class WorldAuctionItemNumRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603921;
		public UInt32 Sequence;
		/// <summary>
		///  商品状态(AuctionGoodState)
		/// </summary>
		[AdvancedInspector.Descriptor(" 商品状态(AuctionGoodState)", " 商品状态(AuctionGoodState)")]
		public byte goodState;

		public AuctionItemBaseInfo[] items = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, goodState);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
			for(int i = 0; i < items.Length; i++)
			{
				items[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref goodState);
			UInt16 itemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
			items = new AuctionItemBaseInfo[itemsCnt];
			for(int i = 0; i < items.Length; i++)
			{
				items[i] = new AuctionItemBaseInfo();
				items[i].decode(buffer, ref pos_);
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
