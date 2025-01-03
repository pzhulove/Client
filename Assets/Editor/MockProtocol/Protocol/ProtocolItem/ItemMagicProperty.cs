using System;
using System.Text;

namespace Mock.Protocol
{

	public class ItemMagicProperty : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public byte type;
		/// <summary>
		/// 1.随机属性，2.buff
		/// </summary>
		[AdvancedInspector.Descriptor("1.随机属性，2.buff", "1.随机属性，2.buff")]
		public UInt32 param1;
		/// <summary>
		/// 随机属性: 属性id，buff:buffid
		/// </summary>
		[AdvancedInspector.Descriptor("随机属性: 属性id，buff:buffid", "随机属性: 属性id，buff:buffid")]
		public UInt32 param2;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint32(buffer, ref pos_, param1);
			BaseDLL.encode_uint32(buffer, ref pos_, param2);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint32(buffer, ref pos_, ref param1);
			BaseDLL.decode_uint32(buffer, ref pos_, ref param2);
		}


		#endregion

	}

}
