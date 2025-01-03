using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  通知客户端有新的圆桌会议成员
	/// </summary>
	[AdvancedInspector.Descriptor(" 通知客户端有新的圆桌会议成员", " 通知客户端有新的圆桌会议成员")]
	public class WorldGuildTableNewMember : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601938;
		public UInt32 Sequence;

		public GuildTableMember member = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			member.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			member.decode(buffer, ref pos_);
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
