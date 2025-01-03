using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// Counter项
	/// </summary>
	[AdvancedInspector.Descriptor("Counter项", "Counter项")]
	public class CounterItem : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		/// 货币id
		/// </summary>
		[AdvancedInspector.Descriptor("货币id", "货币id")]
		public UInt32 currencyId;
		/// <summary>
		/// 名字
		/// </summary>
		[AdvancedInspector.Descriptor("名字", "名字")]
		public string name;
		/// <summary>
		/// 值
		/// </summary>
		[AdvancedInspector.Descriptor("值", "值")]
		public UInt32 value;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, currencyId);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, value);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref currencyId);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref value);
		}


		#endregion

	}

}
