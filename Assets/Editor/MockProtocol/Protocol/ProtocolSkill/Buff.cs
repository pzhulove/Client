using System;
using System.Text;

namespace Mock.Protocol
{

	public class Buff : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt64 uid;

		public UInt32 id;

		public UInt32 overlay;

		public UInt32 duration;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, uid);
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_uint32(buffer, ref pos_, overlay);
			BaseDLL.encode_uint32(buffer, ref pos_, duration);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref overlay);
			BaseDLL.decode_uint32(buffer, ref pos_, ref duration);
		}


		#endregion

	}

}
