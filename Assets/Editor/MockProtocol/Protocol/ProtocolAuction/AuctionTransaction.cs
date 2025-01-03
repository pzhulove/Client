using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 拍卖行交易记录
	/// </summary>
	[AdvancedInspector.Descriptor("拍卖行交易记录", "拍卖行交易记录")]
	public class AuctionTransaction : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public byte type;

		public UInt32 item_id;

		public UInt32 item_num;

		public UInt32 item_score;

		public UInt32 unit_price;

		public UInt32 verify_end_time;

		public byte enhance_type;

		public byte strength;

		public byte qualitylv;

		public UInt32 trans_time;

		public UInt32 beadbuffId;

		public UInt32 mountBeadId;

		public UInt32 mountBeadBuffId;

		public UInt32 mountMagicCardId;

		public byte mountMagicCardLv;

		public byte equipType;

		public UInt32 enhanceNum;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint32(buffer, ref pos_, item_id);
			BaseDLL.encode_uint32(buffer, ref pos_, item_num);
			BaseDLL.encode_uint32(buffer, ref pos_, item_score);
			BaseDLL.encode_uint32(buffer, ref pos_, unit_price);
			BaseDLL.encode_uint32(buffer, ref pos_, verify_end_time);
			BaseDLL.encode_int8(buffer, ref pos_, enhance_type);
			BaseDLL.encode_int8(buffer, ref pos_, strength);
			BaseDLL.encode_int8(buffer, ref pos_, qualitylv);
			BaseDLL.encode_uint32(buffer, ref pos_, trans_time);
			BaseDLL.encode_uint32(buffer, ref pos_, beadbuffId);
			BaseDLL.encode_uint32(buffer, ref pos_, mountBeadId);
			BaseDLL.encode_uint32(buffer, ref pos_, mountBeadBuffId);
			BaseDLL.encode_uint32(buffer, ref pos_, mountMagicCardId);
			BaseDLL.encode_int8(buffer, ref pos_, mountMagicCardLv);
			BaseDLL.encode_int8(buffer, ref pos_, equipType);
			BaseDLL.encode_uint32(buffer, ref pos_, enhanceNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint32(buffer, ref pos_, ref item_id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref item_num);
			BaseDLL.decode_uint32(buffer, ref pos_, ref item_score);
			BaseDLL.decode_uint32(buffer, ref pos_, ref unit_price);
			BaseDLL.decode_uint32(buffer, ref pos_, ref verify_end_time);
			BaseDLL.decode_int8(buffer, ref pos_, ref enhance_type);
			BaseDLL.decode_int8(buffer, ref pos_, ref strength);
			BaseDLL.decode_int8(buffer, ref pos_, ref qualitylv);
			BaseDLL.decode_uint32(buffer, ref pos_, ref trans_time);
			BaseDLL.decode_uint32(buffer, ref pos_, ref beadbuffId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref mountBeadId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref mountBeadBuffId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref mountMagicCardId);
			BaseDLL.decode_int8(buffer, ref pos_, ref mountMagicCardLv);
			BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref enhanceNum);
		}


		#endregion

	}

}
