using System;
using System.Text;

namespace Mock.Protocol
{

	public class PkOccuRecord : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public byte occu;

		public UInt32 winNum;

		public UInt32 loseNum;

		public UInt32 totalNum;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, occu);
			BaseDLL.encode_uint32(buffer, ref pos_, winNum);
			BaseDLL.encode_uint32(buffer, ref pos_, loseNum);
			BaseDLL.encode_uint32(buffer, ref pos_, totalNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref occu);
			BaseDLL.decode_uint32(buffer, ref pos_, ref winNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref loseNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref totalNum);
		}


		#endregion

	}

}
