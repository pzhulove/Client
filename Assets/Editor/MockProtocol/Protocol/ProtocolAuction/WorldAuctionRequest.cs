using System;
using System.Text;

namespace Mock.Protocol
{

	public class WorldAuctionRequest : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603906;
		public UInt32 Sequence;
		/// <summary>
		///  拍卖类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 拍卖类型", " 拍卖类型")]
		public byte type;
		/// <summary>
		///  拍卖道具id
		/// </summary>
		[AdvancedInspector.Descriptor(" 拍卖道具id", " 拍卖道具id")]
		public UInt64 id;
		/// <summary>
		///  拍卖道具类型id
		/// </summary>
		[AdvancedInspector.Descriptor(" 拍卖道具类型id", " 拍卖道具类型id")]
		public UInt32 typeId;
		/// <summary>
		///  数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 数量", " 数量")]
		public UInt32 num;
		/// <summary>
		///  价格
		/// </summary>
		[AdvancedInspector.Descriptor(" 价格", " 价格")]
		public UInt32 price;
		/// <summary>
		///  持续时间(AuctionSellDuration)
		/// </summary>
		[AdvancedInspector.Descriptor(" 持续时间(AuctionSellDuration)", " 持续时间(AuctionSellDuration)")]
		public byte duration;
		/// <summary>
		///  强化等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 强化等级", " 强化等级")]
		public byte strength;
		/// <summary>
		///  宝珠buffid
		/// </summary>
		[AdvancedInspector.Descriptor(" 宝珠buffid", " 宝珠buffid")]
		public UInt32 beadbuffId;
		/// <summary>
		///  是否重新上架
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否重新上架", " 是否重新上架")]
		public byte isAgain;
		/// <summary>
		///  重新上架guid
		/// </summary>
		[AdvancedInspector.Descriptor(" 重新上架guid", " 重新上架guid")]
		public UInt64 auctionGuid;
		/// <summary>
		///  红字装备增幅类型 无效0/力量1/智力2/体力3/精神4
		/// </summary>
		[AdvancedInspector.Descriptor(" 红字装备增幅类型 无效0/力量1/智力2/体力3/精神4", " 红字装备增幅类型 无效0/力量1/智力2/体力3/精神4")]
		public byte enhanceType;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			BaseDLL.encode_uint32(buffer, ref pos_, typeId);
			BaseDLL.encode_uint32(buffer, ref pos_, num);
			BaseDLL.encode_uint32(buffer, ref pos_, price);
			BaseDLL.encode_int8(buffer, ref pos_, duration);
			BaseDLL.encode_int8(buffer, ref pos_, strength);
			BaseDLL.encode_uint32(buffer, ref pos_, beadbuffId);
			BaseDLL.encode_int8(buffer, ref pos_, isAgain);
			BaseDLL.encode_uint64(buffer, ref pos_, auctionGuid);
			BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref typeId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			BaseDLL.decode_uint32(buffer, ref pos_, ref price);
			BaseDLL.decode_int8(buffer, ref pos_, ref duration);
			BaseDLL.decode_int8(buffer, ref pos_, ref strength);
			BaseDLL.decode_uint32(buffer, ref pos_, ref beadbuffId);
			BaseDLL.decode_int8(buffer, ref pos_, ref isAgain);
			BaseDLL.decode_uint64(buffer, ref pos_, ref auctionGuid);
			BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
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
