using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  道具位置
	/// </summary>
	[AdvancedInspector.Descriptor(" 道具位置", " 道具位置")]
	public class ItemPos : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  包裹类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 包裹类型", " 包裹类型")]
		public byte type;
		/// <summary>
		///  格子索引
		/// </summary>
		[AdvancedInspector.Descriptor(" 格子索引", " 格子索引")]
		public UInt32 index;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint32(buffer, ref pos_, index);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint32(buffer, ref pos_, ref index);
		}


		#endregion

	}

}
