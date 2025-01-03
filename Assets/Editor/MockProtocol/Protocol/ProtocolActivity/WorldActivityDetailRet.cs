using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 请求活动详情返回
	/// </summary>
	[AdvancedInspector.Descriptor("请求活动详情返回", "请求活动详情返回")]
	public class WorldActivityDetailRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 602905;
		public UInt32 Sequence;

		public UInt32 code;

		public UInt32 id;

		public string name;

		public UInt16 level;

		public UInt32 preTime;

		public UInt32 startTime;

		public UInt32 dueTime;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
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
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
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

		public UInt32 GetSequence()
		{
			return Sequence;
		}

		public void SetSequence(UInt32 sequence)
		{
			Sequence = sequence;
		}

		#endregion

	}

}
