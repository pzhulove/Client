using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// server->client 通知邀请者,被邀请玩家的返回
	/// </summary>
	[AdvancedInspector.Descriptor("server->client 通知邀请者,被邀请玩家的返回", "server->client 通知邀请者,被邀请玩家的返回")]
	public class WorldSyncRoomBeInviteInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607806;
		public UInt32 Sequence;
		/// <summary>
		/// 玩家ID
		/// </summary>
		[AdvancedInspector.Descriptor("玩家ID", "玩家ID")]
		public UInt64 playerId;
		/// <summary>
		/// 是否接受
		/// </summary>
		[AdvancedInspector.Descriptor("是否接受", "是否接受")]
		public byte isAccept;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, playerId);
			BaseDLL.encode_int8(buffer, ref pos_, isAccept);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
			BaseDLL.decode_int8(buffer, ref pos_, ref isAccept);
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
