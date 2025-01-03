using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 活动准备
	/// </summary>
	/// <summary>
	///  同ItemReward
	/// </summary>
	[AdvancedInspector.Descriptor(" 同ItemReward", " 同ItemReward")]
	public class OpTaskReward : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 id;

		public UInt32 num;

		public byte qualityLv;

		public byte strenth;
		/// <summary>
		/// 拍卖行交易冷却时间
		/// </summary>
		[AdvancedInspector.Descriptor("拍卖行交易冷却时间", "拍卖行交易冷却时间")]
		public UInt32 auctionCoolTimeStamp;
		/// <summary>
		/// 装备类型
		/// </summary>
		[AdvancedInspector.Descriptor("装备类型", "装备类型")]
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
			BaseDLL.encode_int8(buffer, ref pos_, strenth);
			BaseDLL.encode_uint32(buffer, ref pos_, auctionCoolTimeStamp);
			BaseDLL.encode_int8(buffer, ref pos_, equipType);
			BaseDLL.encode_uint32(buffer, ref pos_, auctionTransTimes);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			BaseDLL.decode_int8(buffer, ref pos_, ref qualityLv);
			BaseDLL.decode_int8(buffer, ref pos_, ref strenth);
			BaseDLL.decode_uint32(buffer, ref pos_, ref auctionCoolTimeStamp);
			BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref auctionTransTimes);
		}


		#endregion

	}

}
