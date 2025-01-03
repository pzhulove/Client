using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 开罐记录数据
	/// </summary>
	[AdvancedInspector.Descriptor("开罐记录数据", "开罐记录数据")]
	public class OpenJarRecord : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public string name;

		public UInt32 itemId;

		public UInt32 num;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, itemId);
			BaseDLL.encode_uint32(buffer, ref pos_, num);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref num);
		}


		#endregion

	}

}
