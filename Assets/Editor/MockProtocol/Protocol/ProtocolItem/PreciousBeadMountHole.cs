using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  宝珠镶嵌孔
	/// </summary>
	[AdvancedInspector.Descriptor(" 宝珠镶嵌孔", " 宝珠镶嵌孔")]
	public class PreciousBeadMountHole : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public byte index;
		/// <summary>
		/// 孔索引
		/// </summary>
		[AdvancedInspector.Descriptor("孔索引", "孔索引")]
		public byte type;
		/// <summary>
		/// 孔类型
		/// </summary>
		[AdvancedInspector.Descriptor("孔类型", "孔类型")]
		public UInt32 preciousBeadId;
		/// <summary>
		/// 镶嵌宝珠id
		/// </summary>
		[AdvancedInspector.Descriptor("镶嵌宝珠id", "镶嵌宝珠id")]
		public UInt32 buffId;
		/// <summary>
		/// 附加buff id
		/// </summary>
		[AdvancedInspector.Descriptor("附加buff id", "附加buff id")]
		public UInt32 auctionCoolTimeStamp;
		/// <summary>
		/// 宝珠拍卖行交易冷却时间戳(秒)
		/// </summary>
		[AdvancedInspector.Descriptor("宝珠拍卖行交易冷却时间戳(秒)", "宝珠拍卖行交易冷却时间戳(秒)")]
		public UInt32 extirpeCnt;
		/// <summary>
		/// 摘除次数
		/// </summary>
		[AdvancedInspector.Descriptor("摘除次数", "摘除次数")]
		public UInt32 replaceCnt;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, index);
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint32(buffer, ref pos_, preciousBeadId);
			BaseDLL.encode_uint32(buffer, ref pos_, buffId);
			BaseDLL.encode_uint32(buffer, ref pos_, auctionCoolTimeStamp);
			BaseDLL.encode_uint32(buffer, ref pos_, extirpeCnt);
			BaseDLL.encode_uint32(buffer, ref pos_, replaceCnt);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref index);
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint32(buffer, ref pos_, ref preciousBeadId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref buffId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref auctionCoolTimeStamp);
			BaseDLL.decode_uint32(buffer, ref pos_, ref extirpeCnt);
			BaseDLL.decode_uint32(buffer, ref pos_, ref replaceCnt);
		}


		#endregion

	}

}
