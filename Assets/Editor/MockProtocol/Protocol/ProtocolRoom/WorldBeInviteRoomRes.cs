using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  server->client 被邀请请求返回
	/// </summary>
	[AdvancedInspector.Descriptor(" server->client 被邀请请求返回", " server->client 被邀请请求返回")]
	public class WorldBeInviteRoomRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607828;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
		/// </summary>
		[AdvancedInspector.Descriptor(" 返回值", " 返回值")]
		public UInt32 result;
		/// <summary>
		///  房间信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 房间信息", " 房间信息")]
		public RoomInfo roomInfo = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			roomInfo.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			roomInfo.decode(buffer, ref pos_);
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
