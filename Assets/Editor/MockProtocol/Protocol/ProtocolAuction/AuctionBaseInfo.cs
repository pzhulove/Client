using System;
using System.Text;

namespace Mock.Protocol
{

	public class AuctionBaseInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  Ψһid
		/// </summary>
		[AdvancedInspector.Descriptor(" Ψһid", " Ψһid")]
		public UInt64 guid;

		public UInt32 price;

		public byte pricetype;

		public UInt32 num;

		public UInt32 itemTypeId;

		public UInt32 strengthed;

		public UInt32 itemScore;

		public UInt32 publicEndTime;

		public byte isTreas;

		public UInt64 owner;

		public byte attent;

		public UInt32 attentNum;

		public UInt32 duetime;

		public UInt32 onSaleTime;

		public UInt32 beadbuffid;

		public byte isAgainOnsale;

		public byte equipType;

		public byte enhanceType;

		public UInt32 itemDueTime;

		public UInt32 enhanceNum;

		public UInt32 itemTransNum;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, guid);
			BaseDLL.encode_uint32(buffer, ref pos_, price);
			BaseDLL.encode_int8(buffer, ref pos_, pricetype);
			BaseDLL.encode_uint32(buffer, ref pos_, num);
			BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
			BaseDLL.encode_uint32(buffer, ref pos_, strengthed);
			BaseDLL.encode_uint32(buffer, ref pos_, itemScore);
			BaseDLL.encode_uint32(buffer, ref pos_, publicEndTime);
			BaseDLL.encode_int8(buffer, ref pos_, isTreas);
			BaseDLL.encode_uint64(buffer, ref pos_, owner);
			BaseDLL.encode_int8(buffer, ref pos_, attent);
			BaseDLL.encode_uint32(buffer, ref pos_, attentNum);
			BaseDLL.encode_uint32(buffer, ref pos_, duetime);
			BaseDLL.encode_uint32(buffer, ref pos_, onSaleTime);
			BaseDLL.encode_uint32(buffer, ref pos_, beadbuffid);
			BaseDLL.encode_int8(buffer, ref pos_, isAgainOnsale);
			BaseDLL.encode_int8(buffer, ref pos_, equipType);
			BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
			BaseDLL.encode_uint32(buffer, ref pos_, itemDueTime);
			BaseDLL.encode_uint32(buffer, ref pos_, enhanceNum);
			BaseDLL.encode_uint32(buffer, ref pos_, itemTransNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref price);
			BaseDLL.decode_int8(buffer, ref pos_, ref pricetype);
			BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref strengthed);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemScore);
			BaseDLL.decode_uint32(buffer, ref pos_, ref publicEndTime);
			BaseDLL.decode_int8(buffer, ref pos_, ref isTreas);
			BaseDLL.decode_uint64(buffer, ref pos_, ref owner);
			BaseDLL.decode_int8(buffer, ref pos_, ref attent);
			BaseDLL.decode_uint32(buffer, ref pos_, ref attentNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref duetime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref onSaleTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref beadbuffid);
			BaseDLL.decode_int8(buffer, ref pos_, ref isAgainOnsale);
			BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
			BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemDueTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref enhanceNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemTransNum);
		}


		#endregion

	}

}
