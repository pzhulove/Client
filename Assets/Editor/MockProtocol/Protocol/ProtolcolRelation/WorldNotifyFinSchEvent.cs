using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 类型(RelationAnnounceType)
	/// </summary>
	/// <summary>
	/// world->client 通知出师事件
	/// </summary>
	[AdvancedInspector.Descriptor("world->client 通知出师事件", "world->client 通知出师事件")]
	public class WorldNotifyFinSchEvent : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601777;
		public UInt32 Sequence;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
		}

		public void decode(byte[] buffer, ref int pos_)
		{
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
