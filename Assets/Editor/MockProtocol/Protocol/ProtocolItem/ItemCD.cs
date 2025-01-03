using System;
using System.Text;

namespace Mock.Protocol
{

	public class ItemCD : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public byte groupid;

		public UInt32 endtime;

		public UInt32 maxtime;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, groupid);
			BaseDLL.encode_uint32(buffer, ref pos_, endtime);
			BaseDLL.encode_uint32(buffer, ref pos_, maxtime);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref groupid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref endtime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref maxtime);
		}


		#endregion

	}

}
