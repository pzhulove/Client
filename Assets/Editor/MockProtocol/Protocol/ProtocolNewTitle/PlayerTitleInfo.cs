using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 称谓信息
	/// </summary>
	[AdvancedInspector.Descriptor("称谓信息", "称谓信息")]
	public class PlayerTitleInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt64 guid;
		/// <summary>
		/// 唯一id
		/// </summary>
		[AdvancedInspector.Descriptor("唯一id", "唯一id")]
		public UInt32 createTime;
		/// <summary>
		/// 创建时间(获取时间)
		/// </summary>
		[AdvancedInspector.Descriptor("创建时间(获取时间)", "创建时间(获取时间)")]
		public UInt32 titleId;
		/// <summary>
		/// 表id
		/// </summary>
		[AdvancedInspector.Descriptor("表id", "表id")]
		public byte type;
		/// <summary>
		/// 类型
		/// </summary>
		[AdvancedInspector.Descriptor("类型", "类型")]
		public UInt32 duetime;
		/// <summary>
		/// 到期时间,0永久
		/// </summary>
		[AdvancedInspector.Descriptor("到期时间,0永久", "到期时间,0永久")]
		public string name;
		/// <summary>
		/// 头衔名称
		/// </summary>
		[AdvancedInspector.Descriptor("头衔名称", "头衔名称")]
		public byte style;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, guid);
			BaseDLL.encode_uint32(buffer, ref pos_, createTime);
			BaseDLL.encode_uint32(buffer, ref pos_, titleId);
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint32(buffer, ref pos_, duetime);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, style);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref createTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref titleId);
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint32(buffer, ref pos_, ref duetime);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref style);
		}


		#endregion

	}

}
