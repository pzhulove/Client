using System;
using System.Text;

namespace Mock.Protocol
{

	public class TaskPair : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public string key;

		public string value;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] keyBytes = StringHelper.StringToUTF8Bytes(key);
			BaseDLL.encode_string(buffer, ref pos_, keyBytes, (UInt16)(buffer.Length - pos_));
			byte[] valueBytes = StringHelper.StringToUTF8Bytes(value);
			BaseDLL.encode_string(buffer, ref pos_, valueBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 keyLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref keyLen);
			byte[] keyBytes = new byte[keyLen];
			for(int i = 0; i < keyLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref keyBytes[i]);
			}
			key = StringHelper.BytesToString(keyBytes);
			UInt16 valueLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref valueLen);
			byte[] valueBytes = new byte[valueLen];
			for(int i = 0; i < valueLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref valueBytes[i]);
			}
			value = StringHelper.BytesToString(valueBytes);
		}


		#endregion

	}

}
