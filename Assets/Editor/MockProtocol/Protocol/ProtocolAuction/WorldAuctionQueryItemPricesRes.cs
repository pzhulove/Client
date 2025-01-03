using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 道具价格情况返回
	/// </summary>
	[AdvancedInspector.Descriptor("道具价格情况返回", "道具价格情况返回")]
	public class WorldAuctionQueryItemPricesRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603923;
		public UInt32 Sequence;
		/// <summary>
		///  拍卖行类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 拍卖行类型", " 拍卖行类型")]
		public byte type;
		/// <summary>
		///  物品类型id
		/// </summary>
		[AdvancedInspector.Descriptor(" 物品类型id", " 物品类型id")]
		public UInt32 itemTypeId;
		/// <summary>
		///  物品强化等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 物品强化等级", " 物品强化等级")]
		public UInt32 strengthen;
		/// <summary>
		///  近期平均交易价格
		/// </summary>
		[AdvancedInspector.Descriptor(" 近期平均交易价格", " 近期平均交易价格")]
		public UInt32 averagePrice;
		/// <summary>
		///  目前在售的价格最低的同样道具
		/// </summary>
		[AdvancedInspector.Descriptor(" 目前在售的价格最低的同样道具", " 目前在售的价格最低的同样道具")]
		public AuctionBaseInfo[] actionItems = null;
		/// <summary>
		///  近期可见平均交易价格(非珍品)
		/// </summary>
		[AdvancedInspector.Descriptor(" 近期可见平均交易价格(非珍品)", " 近期可见平均交易价格(非珍品)")]
		public UInt32 visAverPrice;
		/// <summary>
		///  鏈�灏忎环鏍?
		/// </summary>
		[AdvancedInspector.Descriptor(" 鏈�灏忎环鏍?", " 鏈�灏忎环鏍?")]
		public UInt32 minPrice;
		/// <summary>
		///  鏈�澶т环鏍?
		/// </summary>
		[AdvancedInspector.Descriptor(" 鏈�澶т环鏍?", " 鏈�澶т环鏍?")]
		public UInt32 maxPrice;
		/// <summary>
		///  推荐价格
		/// </summary>
		[AdvancedInspector.Descriptor(" 推荐价格", " 推荐价格")]
		public UInt32 recommendPrice;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
			BaseDLL.encode_uint32(buffer, ref pos_, strengthen);
			BaseDLL.encode_uint32(buffer, ref pos_, averagePrice);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)actionItems.Length);
			for(int i = 0; i < actionItems.Length; i++)
			{
				actionItems[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint32(buffer, ref pos_, visAverPrice);
			BaseDLL.encode_uint32(buffer, ref pos_, minPrice);
			BaseDLL.encode_uint32(buffer, ref pos_, maxPrice);
			BaseDLL.encode_uint32(buffer, ref pos_, recommendPrice);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref strengthen);
			BaseDLL.decode_uint32(buffer, ref pos_, ref averagePrice);
			UInt16 actionItemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref actionItemsCnt);
			actionItems = new AuctionBaseInfo[actionItemsCnt];
			for(int i = 0; i < actionItems.Length; i++)
			{
				actionItems[i] = new AuctionBaseInfo();
				actionItems[i].decode(buffer, ref pos_);
			}
			BaseDLL.decode_uint32(buffer, ref pos_, ref visAverPrice);
			BaseDLL.decode_uint32(buffer, ref pos_, ref minPrice);
			BaseDLL.decode_uint32(buffer, ref pos_, ref maxPrice);
			BaseDLL.decode_uint32(buffer, ref pos_, ref recommendPrice);
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
