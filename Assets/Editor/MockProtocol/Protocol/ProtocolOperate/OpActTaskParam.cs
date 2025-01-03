using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 已完成,已提交
	/// </summary>
	[AdvancedInspector.Descriptor("已完成,已提交", "已完成,已提交")]
	public class OpActTaskParam : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public string key;

		public UInt32 value;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] keyBytes = StringHelper.StringToUTF8Bytes(key);
			BaseDLL.encode_string(buffer, ref pos_, keyBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, value);
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
			BaseDLL.decode_uint32(buffer, ref pos_, ref value);
		}


		#endregion

	}

}
