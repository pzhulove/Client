using System;
using System.Text;

namespace Mock.Protocol
{

	public class ItemInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt64 uid;

		public UInt32 num;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, uid);
			BaseDLL.encode_uint32(buffer, ref pos_, num);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref num);
		}


		#endregion

	}

}
