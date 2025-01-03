using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  商城购买获得物信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 商城购买获得物信息", " 商城购买获得物信息")]
	public class MallBuyGotInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  购买获得物类型(MallBuyGotType)
		/// </summary>
		[AdvancedInspector.Descriptor(" 购买获得物类型(MallBuyGotType)", " 购买获得物类型(MallBuyGotType)")]
		public byte buyGotType;
		/// <summary>
		///  购买获得物对应的道具id
		/// </summary>
		[AdvancedInspector.Descriptor(" 购买获得物对应的道具id", " 购买获得物对应的道具id")]
		public UInt32 itemDataId;
		/// <summary>
		///  购买获得物数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 购买获得物数量", " 购买获得物数量")]
		public UInt32 buyGotNum;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, buyGotType);
			BaseDLL.encode_uint32(buffer, ref pos_, itemDataId);
			BaseDLL.encode_uint32(buffer, ref pos_, buyGotNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref buyGotType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemDataId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref buyGotNum);
		}


		#endregion

	}

}
