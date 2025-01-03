using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  捐献日志
	/// </summary>
	[AdvancedInspector.Descriptor(" 捐献日志", " 捐献日志")]
	public class GuildDonateLog : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  id
		/// </summary>
		[AdvancedInspector.Descriptor(" id", " id")]
		public UInt64 id;
		/// <summary>
		///  名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 名字", " 名字")]
		public string name;
		/// <summary>
		///  捐献类型（对应枚举GuildDonateType）
		/// </summary>
		[AdvancedInspector.Descriptor(" 捐献类型（对应枚举GuildDonateType）", " 捐献类型（对应枚举GuildDonateType）")]
		public byte type;
		/// <summary>
		///  次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 次数", " 次数")]
		public byte num;
		/// <summary>
		///  获得贡献
		/// </summary>
		[AdvancedInspector.Descriptor(" 获得贡献", " 获得贡献")]
		public UInt32 contri;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_int8(buffer, ref pos_, num);
			BaseDLL.encode_uint32(buffer, ref pos_, contri);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_int8(buffer, ref pos_, ref num);
			BaseDLL.decode_uint32(buffer, ref pos_, ref contri);
		}


		#endregion

	}

}
