using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 置换次数
	/// </summary>
	/// <summary>
	///  铭文镶嵌孔
	/// </summary>
	[AdvancedInspector.Descriptor(" 铭文镶嵌孔", " 铭文镶嵌孔")]
	public class InscriptionMountHole : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 index;
		/// <summary>
		/// 孔索引
		/// </summary>
		[AdvancedInspector.Descriptor("孔索引", "孔索引")]
		public UInt32 type;
		/// <summary>
		/// 孔类型
		/// </summary>
		[AdvancedInspector.Descriptor("孔类型", "孔类型")]
		public UInt32 inscriptionId;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, index);
			BaseDLL.encode_uint32(buffer, ref pos_, type);
			BaseDLL.encode_uint32(buffer, ref pos_, inscriptionId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref index);
			BaseDLL.decode_uint32(buffer, ref pos_, ref type);
			BaseDLL.decode_uint32(buffer, ref pos_, ref inscriptionId);
		}


		#endregion

	}

}
