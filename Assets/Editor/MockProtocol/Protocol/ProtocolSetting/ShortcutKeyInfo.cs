using System;
using System.Text;

namespace Mock.Protocol
{

	public class ShortcutKeyInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  设置类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 设置类型", " 设置类型")]
		public UInt32 setType;
		/// <summary>
		///  设置值（客户端自定义的长字符串格式，最大1000个字节）
		/// </summary>
		[AdvancedInspector.Descriptor(" 设置值（客户端自定义的长字符串格式，最大1000个字节）", " 设置值（客户端自定义的长字符串格式，最大1000个字节）")]
		public string setValue;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, setType);
			byte[] setValueBytes = StringHelper.StringToUTF8Bytes(setValue);
			BaseDLL.encode_string(buffer, ref pos_, setValueBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref setType);
			UInt16 setValueLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref setValueLen);
			byte[] setValueBytes = new byte[setValueLen];
			for(int i = 0; i < setValueLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref setValueBytes[i]);
			}
			setValue = StringHelper.BytesToString(setValueBytes);
		}


		#endregion

	}

}
