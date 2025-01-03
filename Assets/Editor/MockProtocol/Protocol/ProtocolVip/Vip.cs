using System;
using System.Text;

namespace Mock.Protocol
{

	public class Vip : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public byte level;

		public UInt32 exp;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, level);
			BaseDLL.encode_uint32(buffer, ref pos_, exp);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref level);
			BaseDLL.decode_uint32(buffer, ref pos_, ref exp);
		}


		#endregion

	}

}
