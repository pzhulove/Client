using System;
using System.Text;

namespace Mock.Protocol
{

	public class ActivityInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		/// 状态，0 结束, 1 进行中，2 准备中
		/// </summary>
		[AdvancedInspector.Descriptor("状态，0 结束, 1 进行中，2 准备中", "状态，0 结束, 1 进行中，2 准备中")]
		public byte state;

		public UInt32 id;
		/// <summary>
		/// 活动名
		/// </summary>
		[AdvancedInspector.Descriptor("活动名", "活动名")]
		public string name;
		/// <summary>
		/// 需要等级
		/// </summary>
		[AdvancedInspector.Descriptor("需要等级", "需要等级")]
		public UInt16 level;
		/// <summary>
		/// 准备时间
		/// </summary>
		[AdvancedInspector.Descriptor("准备时间", "准备时间")]
		public UInt32 preTime;
		/// <summary>
		/// 开始时间
		/// </summary>
		[AdvancedInspector.Descriptor("开始时间", "开始时间")]
		public UInt32 startTime;
		/// <summary>
		/// 到期时间
		/// </summary>
		[AdvancedInspector.Descriptor("到期时间", "到期时间")]
		public UInt32 dueTime;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, state);
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint16(buffer, ref pos_, level);
			BaseDLL.encode_uint32(buffer, ref pos_, preTime);
			BaseDLL.encode_uint32(buffer, ref pos_, startTime);
			BaseDLL.encode_uint32(buffer, ref pos_, dueTime);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref state);
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_uint16(buffer, ref pos_, ref level);
			BaseDLL.decode_uint32(buffer, ref pos_, ref preTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref dueTime);
		}


		#endregion

	}

}
