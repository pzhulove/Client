using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  团队状态通知
	/// </summary>
	[AdvancedInspector.Descriptor(" 团队状态通知", " 团队状态通知")]
	public class TeamCopyTeamStatusNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100057;
		public UInt32 Sequence;

		public UInt32 teamId;

		public UInt32 teamStatus;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, teamId);
			BaseDLL.encode_uint32(buffer, ref pos_, teamStatus);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref teamStatus);
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
