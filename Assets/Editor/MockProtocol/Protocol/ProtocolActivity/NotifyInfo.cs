using System;
using System.Text;

namespace Mock.Protocol
{

	public class NotifyInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 type;
		/// <summary>
		/// 通知类型
		/// </summary>
		[AdvancedInspector.Descriptor("通知类型", "通知类型")]
		public UInt64 param;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, type);
			BaseDLL.encode_uint64(buffer, ref pos_, param);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref type);
			BaseDLL.decode_uint64(buffer, ref pos_, ref param);
		}


		#endregion

	}

}
