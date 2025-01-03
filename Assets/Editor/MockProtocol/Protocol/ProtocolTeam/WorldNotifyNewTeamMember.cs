using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  通知新成员加入
	/// </summary>
	[AdvancedInspector.Descriptor(" 通知新成员加入", " 通知新成员加入")]
	public class WorldNotifyNewTeamMember : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601602;
		public UInt32 Sequence;
		/// <summary>
		///  队员信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 队员信息", " 队员信息")]
		public TeammemberInfo info = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			info.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			info.decode(buffer, ref pos_);
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
