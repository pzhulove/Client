using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// server->client 通知被踢出玩家
	/// </summary>
	[AdvancedInspector.Descriptor("server->client 通知被踢出玩家", "server->client 通知被踢出玩家")]
	public class WorldSyncRoomKickOutInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607805;
		public UInt32 Sequence;
		/// <summary>
		/// 踢出原因
		/// </summary>
		[AdvancedInspector.Descriptor("踢出原因", "踢出原因")]
		public UInt32 reason;
		/// <summary>
		/// 踢出玩家id
		/// </summary>
		[AdvancedInspector.Descriptor("踢出玩家id", "踢出玩家id")]
		public UInt64 kickPlayerId;
		/// <summary>
		/// 踢出玩家名字
		/// </summary>
		[AdvancedInspector.Descriptor("踢出玩家名字", "踢出玩家名字")]
		public string kickPlayerName;
		/// <summary>
		/// 房间id
		/// </summary>
		[AdvancedInspector.Descriptor("房间id", "房间id")]
		public UInt32 roomId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, reason);
			BaseDLL.encode_uint64(buffer, ref pos_, kickPlayerId);
			byte[] kickPlayerNameBytes = StringHelper.StringToUTF8Bytes(kickPlayerName);
			BaseDLL.encode_string(buffer, ref pos_, kickPlayerNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, roomId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref reason);
			BaseDLL.decode_uint64(buffer, ref pos_, ref kickPlayerId);
			UInt16 kickPlayerNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref kickPlayerNameLen);
			byte[] kickPlayerNameBytes = new byte[kickPlayerNameLen];
			for(int i = 0; i < kickPlayerNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref kickPlayerNameBytes[i]);
			}
			kickPlayerName = StringHelper.BytesToString(kickPlayerNameBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref roomId);
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
