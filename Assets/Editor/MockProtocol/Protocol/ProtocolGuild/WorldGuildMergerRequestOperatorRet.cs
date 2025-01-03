using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world -> client 兼并操作结果
	/// </summary>
	[AdvancedInspector.Descriptor("world -> client 兼并操作结果", "world -> client 兼并操作结果")]
	public class WorldGuildMergerRequestOperatorRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601980;
		public UInt32 Sequence;

		public UInt32 errorCode;
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
			BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
			BaseDLL.encode_int8(buffer, ref pos_, opType);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
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
