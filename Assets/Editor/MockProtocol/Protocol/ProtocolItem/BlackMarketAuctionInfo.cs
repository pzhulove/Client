using System;
using System.Text;

namespace Mock.Protocol
{

	public class BlackMarketAuctionInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  唯一id
		/// </summary>
		[AdvancedInspector.Descriptor(" 唯一id", " 唯一id")]
		public UInt64 guid;
		/// <summary>
		///  回购物品id
		/// </summary>
		[AdvancedInspector.Descriptor(" 回购物品id", " 回购物品id")]
		public UInt32 back_buy_item_id;
		/// <summary>
		///  回购类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 回购类型", " 回购类型")]
		public byte back_buy_type;
		/// <summary>
		///  固定收购价格
		/// </summary>
		[AdvancedInspector.Descriptor(" 固定收购价格", " 固定收购价格")]
		public UInt32 price;
		/// <summary>
		///  竞拍开始时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 竞拍开始时间", " 竞拍开始时间")]
		public UInt32 begin_time;
		/// <summary>
		///  竞拍结束时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 竞拍结束时间", " 竞拍结束时间")]
		public UInt32 end_time;
		/// <summary>
		///  推荐价格
		/// </summary>
		[AdvancedInspector.Descriptor(" 推荐价格", " 推荐价格")]
		public UInt32 recommend_price;
		/// <summary>
		///  价格下限
		/// </summary>
		[AdvancedInspector.Descriptor(" 价格下限", " 价格下限")]
		public UInt32 price_lower_limit;
		/// <summary>
		///  价格上限
		/// </summary>
		[AdvancedInspector.Descriptor(" 价格上限", " 价格上限")]
		public UInt32 price_upper_limit;
		/// <summary>
		///  状态(BlackMarketAuctionState)
		/// </summary>
		[AdvancedInspector.Descriptor(" 状态(BlackMarketAuctionState)", " 状态(BlackMarketAuctionState)")]
		public byte state;
		/// <summary>
		///  竞拍角色id
		/// </summary>
		[AdvancedInspector.Descriptor(" 竞拍角色id", " 竞拍角色id")]
		public UInt64 auctioner_guid;
		/// <summary>
		///  竞拍者名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 竞拍者名字", " 竞拍者名字")]
		public string auctioner_name;
		/// <summary>
		///  报价
		/// </summary>
		[AdvancedInspector.Descriptor(" 报价", " 报价")]
		public UInt32 auction_price;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, guid);
			BaseDLL.encode_uint32(buffer, ref pos_, back_buy_item_id);
			BaseDLL.encode_int8(buffer, ref pos_, back_buy_type);
			BaseDLL.encode_uint32(buffer, ref pos_, price);
			BaseDLL.encode_uint32(buffer, ref pos_, begin_time);
			BaseDLL.encode_uint32(buffer, ref pos_, end_time);
			BaseDLL.encode_uint32(buffer, ref pos_, recommend_price);
			BaseDLL.encode_uint32(buffer, ref pos_, price_lower_limit);
			BaseDLL.encode_uint32(buffer, ref pos_, price_upper_limit);
			BaseDLL.encode_int8(buffer, ref pos_, state);
			BaseDLL.encode_uint64(buffer, ref pos_, auctioner_guid);
			byte[] auctioner_nameBytes = StringHelper.StringToUTF8Bytes(auctioner_name);
			BaseDLL.encode_string(buffer, ref pos_, auctioner_nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, auction_price);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref back_buy_item_id);
			BaseDLL.decode_int8(buffer, ref pos_, ref back_buy_type);
			BaseDLL.decode_uint32(buffer, ref pos_, ref price);
			BaseDLL.decode_uint32(buffer, ref pos_, ref begin_time);
			BaseDLL.decode_uint32(buffer, ref pos_, ref end_time);
			BaseDLL.decode_uint32(buffer, ref pos_, ref recommend_price);
			BaseDLL.decode_uint32(buffer, ref pos_, ref price_lower_limit);
			BaseDLL.decode_uint32(buffer, ref pos_, ref price_upper_limit);
			BaseDLL.decode_int8(buffer, ref pos_, ref state);
			BaseDLL.decode_uint64(buffer, ref pos_, ref auctioner_guid);
			UInt16 auctioner_nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref auctioner_nameLen);
			byte[] auctioner_nameBytes = new byte[auctioner_nameLen];
			for(int i = 0; i < auctioner_nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref auctioner_nameBytes[i]);
			}
			auctioner_name = StringHelper.BytesToString(auctioner_nameBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref auction_price);
		}


		#endregion

	}

}
