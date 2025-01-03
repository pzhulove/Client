using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  拍卖行信息返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 拍卖行信息返回", " 拍卖行信息返回")]
	public class WorldGuildAuctionItemRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608514;
		public UInt32 Sequence;
		/// <summary>
		///  拍卖类型(GuildAuctionType)
		/// </summary>
		[AdvancedInspector.Descriptor(" 拍卖类型(GuildAuctionType)", " 拍卖类型(GuildAuctionType)")]
		public UInt32 type;
		/// <summary>
		///  拍卖物品列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 拍卖物品列表", " 拍卖物品列表")]
		public GuildAuctionItem[] auctionItemList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, type);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)auctionItemList.Length);
			for(int i = 0; i < auctionItemList.Length; i++)
			{
				auctionItemList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref type);
			UInt16 auctionItemListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref auctionItemListCnt);
			auctionItemList = new GuildAuctionItem[auctionItemListCnt];
			for(int i = 0; i < auctionItemList.Length; i++)
			{
				auctionItemList[i] = new GuildAuctionItem();
				auctionItemList[i].decode(buffer, ref pos_);
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
