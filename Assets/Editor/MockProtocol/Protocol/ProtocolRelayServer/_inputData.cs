using System;
using System.Text;

namespace Mock.Protocol
{

	public class _inputData : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 sendTime;

		public UInt32 data1;

		public UInt32 data2;

		public UInt32 data3;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, sendTime);
			BaseDLL.encode_uint32(buffer, ref pos_, data1);
			BaseDLL.encode_uint32(buffer, ref pos_, data2);
			BaseDLL.encode_uint32(buffer, ref pos_, data3);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref sendTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref data1);
			BaseDLL.decode_uint32(buffer, ref pos_, ref data2);
			BaseDLL.decode_uint32(buffer, ref pos_, ref data3);
		}


		#endregion

	}

}
