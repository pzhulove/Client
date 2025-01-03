using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  清除队伍邀请
	/// </summary>
	[AdvancedInspector.Descriptor(" 清除队伍邀请", " 清除队伍邀请")]
	public class WorldTeamInviteClearNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601656;
		public UInt32 Sequence;
		/// <summary>
		///  队伍ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 队伍ID", " 队伍ID")]
		public Int32 teamId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int32(buffer, ref pos_, teamId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int32(buffer, ref pos_, ref teamId);
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
