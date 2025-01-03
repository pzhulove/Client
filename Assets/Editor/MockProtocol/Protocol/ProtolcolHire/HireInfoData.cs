using System;
using System.Text;

namespace Mock.Protocol
{

	public class HireInfoData : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 taskID;

		public UInt32 cnt;

		public byte status;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, taskID);
			BaseDLL.encode_uint32(buffer, ref pos_, cnt);
			BaseDLL.encode_int8(buffer, ref pos_, status);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref taskID);
			BaseDLL.decode_uint32(buffer, ref pos_, ref cnt);
			BaseDLL.decode_int8(buffer, ref pos_, ref status);
		}


		#endregion

	}

}
