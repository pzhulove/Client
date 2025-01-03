using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client -> world 兼并操作，包括申请兼并，取消申请兼并请求
	/// </summary>
	[AdvancedInspector.Descriptor("client -> world 兼并操作，包括申请兼并，取消申请兼并请求", "client -> world 兼并操作，包括申请兼并，取消申请兼并请求")]
	public class WorldGuildMergerRequestOperatorReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601979;
		public UInt32 Sequence;

		public UInt64 guildId;
		/// <summary>
		/// 操作类型 0申请兼并 1取消申请兼并
		/// </summary>
		[AdvancedInspector.Descriptor("操作类型 0申请兼并 1取消申请兼并", "操作类型 0申请兼并 1取消申请兼并")]
		public byte opType;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, guildId);
			BaseDLL.encode_int8(buffer, ref pos_, opType);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref guildId);
			BaseDLL.decode_int8(buffer, ref pos_, ref opType);
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
