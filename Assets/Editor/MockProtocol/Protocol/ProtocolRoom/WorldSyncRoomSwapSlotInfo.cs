using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  server->client 通知玩家交换位置信息
	/// </summary>
	[AdvancedInspector.Descriptor(" server->client 通知玩家交换位置信息", " server->client 通知玩家交换位置信息")]
	public class WorldSyncRoomSwapSlotInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607807;
		public UInt32 Sequence;

		public UInt64 playerId;

		public string playerName;

		public UInt16 playerLevel;

		public byte playerOccu;

		public byte playerAwaken;

		public byte slotGroup;

		public byte slotIndex;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, playerId);
			byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
			BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint16(buffer, ref pos_, playerLevel);
			BaseDLL.encode_int8(buffer, ref pos_, playerOccu);
			BaseDLL.encode_int8(buffer, ref pos_, playerAwaken);
			BaseDLL.encode_int8(buffer, ref pos_, slotGroup);
			BaseDLL.encode_int8(buffer, ref pos_, slotIndex);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
			UInt16 playerNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref playerNameLen);
			byte[] playerNameBytes = new byte[playerNameLen];
			for(int i = 0; i < playerNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref playerNameBytes[i]);
			}
			playerName = StringHelper.BytesToString(playerNameBytes);
			BaseDLL.decode_uint16(buffer, ref pos_, ref playerLevel);
			BaseDLL.decode_int8(buffer, ref pos_, ref playerOccu);
			BaseDLL.decode_int8(buffer, ref pos_, ref playerAwaken);
			BaseDLL.decode_int8(buffer, ref pos_, ref slotGroup);
			BaseDLL.decode_int8(buffer, ref pos_, ref slotIndex);
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
