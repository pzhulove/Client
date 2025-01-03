using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// µÀ¾ß¼Û¸ñÇé¿ö·µ»Ø
	/// </summary>
	[AdvancedInspector.Descriptor("µÀ¾ß¼Û¸ñÇé¿ö·µ»Ø", "µÀ¾ß¼Û¸ñÇé¿ö·µ»Ø")]
	public class WorldAuctionQueryItemPricesRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603923;
		public UInt32 Sequence;
		/// <summary>
		///  ÅÄÂôÐÐÀàÐÍ
		/// </summary>
		[AdvancedInspector.Descriptor(" ÅÄÂôÐÐÀàÐÍ", " ÅÄÂôÐÐÀàÐÍ")]
		public byte type;
		/// <summary>
		///  ÎïÆ·ÀàÐÍid
		/// </summary>
		[AdvancedInspector.Descriptor(" ÎïÆ·ÀàÐÍid", " ÎïÆ·ÀàÐÍid")]
		public UInt32 itemTypeId;
		/// <summary>
		///  ÎïÆ·Ç¿»¯µÈ¼¶
		/// </summary>
		[AdvancedInspector.Descriptor(" ÎïÆ·Ç¿»¯µÈ¼¶", " ÎïÆ·Ç¿»¯µÈ¼¶")]
		public UInt32 strengthen;
		/// <summary>
		///  ½üÆÚÆ½¾ù½»Ò×¼Û¸ñ
		/// </summary>
		[AdvancedInspector.Descriptor(" ½üÆÚÆ½¾ù½»Ò×¼Û¸ñ", " ½üÆÚÆ½¾ù½»Ò×¼Û¸ñ")]
		public UInt32 averagePrice;
		/// <summary>
		///  Ä¿Ç°ÔÚÊÛµÄ¼Û¸ñ×îµÍµÄÍ¬ÑùµÀ¾ß
		/// </summary>
		[AdvancedInspector.Descriptor(" Ä¿Ç°ÔÚÊÛµÄ¼Û¸ñ×îµÍµÄÍ¬ÑùµÀ¾ß", " Ä¿Ç°ÔÚÊÛµÄ¼Û¸ñ×îµÍµÄÍ¬ÑùµÀ¾ß")]
		public AuctionBaseInfo[] actionItems = null;
		/// <summary>
		///  ½üÆÚ¿É¼ûÆ½¾ù½»Ò×¼Û¸ñ(·ÇÕäÆ·)
		/// </summary>
		[AdvancedInspector.Descriptor(" ½üÆÚ¿É¼ûÆ½¾ù½»Ò×¼Û¸ñ(·ÇÕäÆ·)", " ½üÆÚ¿É¼ûÆ½¾ù½»Ò×¼Û¸ñ(·ÇÕäÆ·)")]
		public UInt32 visAverPrice;
		/// <summary>
		///  æœ€å°ä»·æ ?
		/// </summary>
		[AdvancedInspector.Descriptor(" æœ€å°ä»·æ ?", " æœ€å°ä»·æ ?")]
		public UInt32 minPrice;
		/// <summary>
		///  æœ€å¤§ä»·æ ?
		/// </summary>
		[AdvancedInspector.Descriptor(" æœ€å¤§ä»·æ ?", " æœ€å¤§ä»·æ ?")]
		public UInt32 maxPrice;
		/// <summary>
		///  ÍÆ¼ö¼Û¸ñ
		/// </summary>
		[AdvancedInspector.Descriptor(" ÍÆ¼ö¼Û¸ñ", " ÍÆ¼ö¼Û¸ñ")]
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
