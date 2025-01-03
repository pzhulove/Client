using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->server 被邀请请求
	/// </summary>
	[AdvancedInspector.Descriptor(" client->server 被邀请请求", " client->server 被邀请请求")]
	public class WorldBeInviteRoomReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607827;
		public UInt32 Sequence;
		/// <summary>
		///  房间号
		/// </summary>
		[AdvancedInspector.Descriptor(" 房间号", " 房间号")]
		public UInt32 roomId;
		/// <summary>
		///  邀请玩家ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 邀请玩家ID", " 邀请玩家ID")]
		public UInt64 invitePlayerId;
		/// <summary>
		///  是否接受
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否接受", " 是否接受")]
		public byte isAccept;
		/// <summary>
		///  队伍
		/// </summary>
		[AdvancedInspector.Descriptor(" 队伍", " 队伍")]
		public byte slotGroup;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, roomId);
			BaseDLL.encode_uint64(buffer, ref pos_, invitePlayerId);
			BaseDLL.encode_int8(buffer, ref pos_, isAccept);
			BaseDLL.encode_int8(buffer, ref pos_, slotGroup);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref roomId);
			BaseDLL.decode_uint64(buffer, ref pos_, ref invitePlayerId);
			BaseDLL.decode_int8(buffer, ref pos_, ref isAccept);
			BaseDLL.decode_int8(buffer, ref pos_, ref slotGroup);
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
