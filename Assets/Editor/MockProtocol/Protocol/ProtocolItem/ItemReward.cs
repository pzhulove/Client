using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 附魔卡等级
	/// </summary>
	[AdvancedInspector.Descriptor("附魔卡等级", "附魔卡等级")]
	public class ItemReward : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 id;

		public UInt32 num;

		public byte qualityLv;
		/// <summary>
		///  强化等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 强化等级", " 强化等级")]
		public byte strength;

		public UInt32 auctionCoolTimeStamp;
		/// <summary>
		///  装备类型，对应枚举EquipType
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备类型，对应枚举EquipType", " 装备类型，对应枚举EquipType")]
		public byte equipType;
		/// <summary>
		/// 拍卖行交易次数
		/// </summary>
		[AdvancedInspector.Descriptor("拍卖行交易次数", "拍卖行交易次数")]
		public UInt32 auctionTransTimes;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_uint32(buffer, ref pos_, num);
			BaseDLL.encode_int8(buffer, ref pos_, qualityLv);
			BaseDLL.encode_int8(buffer, ref pos_, strength);
			BaseDLL.encode_uint32(buffer, ref pos_, auctionCoolTimeStamp);
			BaseDLL.encode_int8(buffer, ref pos_, equipType);
			BaseDLL.encode_uint32(buffer, ref pos_, auctionTransTimes);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			BaseDLL.decode_int8(buffer, ref pos_, ref qualityLv);
			BaseDLL.decode_int8(buffer, ref pos_, ref strength);
			BaseDLL.decode_uint32(buffer, ref pos_, ref auctionCoolTimeStamp);
			BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref auctionTransTimes);
		}


		#endregion

	}

}
