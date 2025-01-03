using System;
using System.Text;

namespace Mock.Protocol
{

	public class OpenJarResult : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 jarItemId;

		public UInt64 itemUid;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, jarItemId);
			BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref jarItemId);
			BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
		}


		#endregion

	}

}
