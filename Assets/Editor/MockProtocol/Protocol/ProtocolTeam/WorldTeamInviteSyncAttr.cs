using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  同步队伍属性给被邀请者
	/// </summary>
	[AdvancedInspector.Descriptor(" 同步队伍属性给被邀请者", " 同步队伍属性给被邀请者")]
	public class WorldTeamInviteSyncAttr : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601657;
		public UInt32 Sequence;
		/// <summary>
		///  队伍ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 队伍ID", " 队伍ID")]
		public Int32 teamId;
		/// <summary>
		///  队伍目标
		/// </summary>
		[AdvancedInspector.Descriptor(" 队伍目标", " 队伍目标")]
		public Int32 target;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int32(buffer, ref pos_, teamId);
			BaseDLL.encode_int32(buffer, ref pos_, target);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int32(buffer, ref pos_, ref teamId);
			BaseDLL.decode_int32(buffer, ref pos_, ref target);
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
