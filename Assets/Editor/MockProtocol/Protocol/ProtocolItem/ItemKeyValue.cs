using System;
using System.Text;

namespace Mock.Protocol
{

	public class ItemKeyValue : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 key;

		public Int32 value;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, key);
			BaseDLL.encode_int32(buffer, ref pos_, value);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref key);
			BaseDLL.decode_int32(buffer, ref pos_, ref value);
		}


		#endregion

	}

}
