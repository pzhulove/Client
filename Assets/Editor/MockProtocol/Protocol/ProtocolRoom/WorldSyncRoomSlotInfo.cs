using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  server->client 同步房间玩家信息
	/// </summary>
	[AdvancedInspector.Descriptor(" server->client 同步房间玩家信息", " server->client 同步房间玩家信息")]
	public class WorldSyncRoomSlotInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607803;
		public UInt32 Sequence;

		public RoomSlotInfo slotInfo = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			slotInfo.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			slotInfo.decode(buffer, ref pos_);
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
