using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  强制结束投票通知
	/// </summary>
	[AdvancedInspector.Descriptor(" 强制结束投票通知", " 强制结束投票通知")]
	public class TeamCopyForceEndVoteNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100072;
		public UInt32 Sequence;
		/// <summary>
		///  投票持续时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 投票持续时间", " 投票持续时间")]
		public UInt32 voteDurationTime;
		/// <summary>
		///  投票截止时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 投票截止时间", " 投票截止时间")]
		public UInt32 voteEndTime;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, voteDurationTime);
			BaseDLL.encode_uint32(buffer, ref pos_, voteEndTime);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref voteDurationTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref voteEndTime);
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
