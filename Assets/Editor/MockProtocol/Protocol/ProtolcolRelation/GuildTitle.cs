using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  公会称号
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会称号", " 公会称号")]
	public class GuildTitle : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  公会名
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会名", " 公会名")]
		public string name;
		/// <summary>
		///  职务
		/// </summary>
		[AdvancedInspector.Descriptor(" 职务", " 职务")]
		public byte post;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, post);
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
			BaseDLL.decode_int8(buffer, ref pos_, ref post);
		}


		#endregion

	}

}
