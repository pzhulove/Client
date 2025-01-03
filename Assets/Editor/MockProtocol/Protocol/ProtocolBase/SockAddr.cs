using System;
using System.Text;

namespace Mock.Protocol
{

	public class SockAddr : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public string ip;

		public UInt16 port;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] ipBytes = StringHelper.StringToUTF8Bytes(ip);
			BaseDLL.encode_string(buffer, ref pos_, ipBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint16(buffer, ref pos_, port);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 ipLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref ipLen);
			byte[] ipBytes = new byte[ipLen];
			for(int i = 0; i < ipLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref ipBytes[i]);
			}
			ip = StringHelper.BytesToString(ipBytes);
			BaseDLL.decode_uint16(buffer, ref pos_, ref port);
		}


		#endregion

	}

}
