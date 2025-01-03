using System;
using System.Text;

namespace Mock.Protocol
{

	public class AccountCounter : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  计数类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 计数类型", " 计数类型")]
		public UInt32 type;
		/// <summary>
		///  数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 数量", " 数量")]
		public UInt64 num;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, type);
			BaseDLL.encode_uint64(buffer, ref pos_, num);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref type);
			BaseDLL.decode_uint64(buffer, ref pos_, ref num);
		}


		#endregion

	}

}
