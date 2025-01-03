using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  寄存物品
	/// </summary>
	[AdvancedInspector.Descriptor(" 寄存物品", " 寄存物品")]
	public class AuctionMagicOnsale : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 等级", " 等级")]
		public byte strength;
		/// <summary>
		///  数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 数量", " 数量")]
		public UInt32 num;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, strength);
			BaseDLL.encode_uint32(buffer, ref pos_, num);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref strength);
			BaseDLL.decode_uint32(buffer, ref pos_, ref num);
		}


		#endregion

	}

}
