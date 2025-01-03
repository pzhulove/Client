using System;
using System.Text;

namespace Mock.Protocol
{

	public class GuildAuctionItem : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  guid
		/// </summary>
		[AdvancedInspector.Descriptor(" guid", " guid")]
		public UInt64 guid;
		/// <summary>
		///  出价人
		/// </summary>
		[AdvancedInspector.Descriptor(" 出价人", " 出价人")]
		public UInt64 bidRoleId;
		/// <summary>
		///  当前出价
		/// </summary>
		[AdvancedInspector.Descriptor(" 当前出价", " 当前出价")]
		public UInt32 curPrice;
		/// <summary>
		///  竞拍价
		/// </summary>
		[AdvancedInspector.Descriptor(" 竞拍价", " 竞拍价")]
		public UInt32 bidPrice;
		/// <summary>
		///  一口价
		/// </summary>
		[AdvancedInspector.Descriptor(" 一口价", " 一口价")]
		public UInt32 fixPrice;
		/// <summary>
		///  拍卖结束时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 拍卖结束时间", " 拍卖结束时间")]
		public UInt32 endTime;
		/// <summary>
		///  状态(GuildAuctionItemState)
		/// </summary>
		[AdvancedInspector.Descriptor(" 状态(GuildAuctionItemState)", " 状态(GuildAuctionItemState)")]
		public UInt32 state;
		/// <summary>
		///  拍卖物品
		/// </summary>
		[AdvancedInspector.Descriptor(" 拍卖物品", " 拍卖物品")]
		public ItemReward[] itemList = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, guid);
			BaseDLL.encode_uint64(buffer, ref pos_, bidRoleId);
			BaseDLL.encode_uint32(buffer, ref pos_, curPrice);
			BaseDLL.encode_uint32(buffer, ref pos_, bidPrice);
			BaseDLL.encode_uint32(buffer, ref pos_, fixPrice);
			BaseDLL.encode_uint32(buffer, ref pos_, endTime);
			BaseDLL.encode_uint32(buffer, ref pos_, state);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemList.Length);
			for(int i = 0; i < itemList.Length; i++)
			{
				itemList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
			BaseDLL.decode_uint64(buffer, ref pos_, ref bidRoleId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref curPrice);
			BaseDLL.decode_uint32(buffer, ref pos_, ref bidPrice);
			BaseDLL.decode_uint32(buffer, ref pos_, ref fixPrice);
			BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref state);
			UInt16 itemListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref itemListCnt);
			itemList = new ItemReward[itemListCnt];
			for(int i = 0; i < itemList.Length; i++)
			{
				itemList[i] = new ItemReward();
				itemList[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
