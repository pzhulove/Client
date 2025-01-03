using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  处理请求者返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 处理请求者返回", " 处理请求者返回")]
	public class WorldTeamProcessRequesterRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601641;
		public UInt32 Sequence;
		/// <summary>
		///  目标ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 目标ID", " 目标ID")]
		public UInt64 targetId;
		/// <summary>
		///  结果
		/// </summary>
		[AdvancedInspector.Descriptor(" 结果", " 结果")]
		public UInt32 result;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, targetId);
			BaseDLL.encode_uint32(buffer, ref pos_, result);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref targetId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
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
