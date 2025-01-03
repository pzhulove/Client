using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 头衔风格
	/// </summary>
	/// <summary>
	/// 穿戴的称谓信息
	/// </summary>
	[AdvancedInspector.Descriptor("穿戴的称谓信息", "穿戴的称谓信息")]
	public class PlayerWearedTitleInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 titleId;
		/// <summary>
		/// 表id
		/// </summary>
		[AdvancedInspector.Descriptor("表id", "表id")]
		public byte style;
		/// <summary>
		/// 风格
		/// </summary>
		[AdvancedInspector.Descriptor("风格", "风格")]
		public string name;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, titleId);
			BaseDLL.encode_int8(buffer, ref pos_, style);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref titleId);
			BaseDLL.decode_int8(buffer, ref pos_, ref style);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
		}


		#endregion

	}

}
