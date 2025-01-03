using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 房间位置信息
	/// </summary>
	[AdvancedInspector.Descriptor("房间位置信息", "房间位置信息")]
	public class RoomSlotInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public byte group;

		public byte index;

		public UInt64 playerId;

		public string playerName;

		public UInt16 playerLevel;

		public UInt32 playerSeasonLevel;

		public byte playerVipLevel;

		public byte playerOccu;

		public byte playerAwake;

		public PlayerAvatar avatar = null;

		public byte status;

		public byte readyStatus;
		/// <summary>
		/// 玩家标签信息
		/// </summary>
		[AdvancedInspector.Descriptor("玩家标签信息", "玩家标签信息")]
		public PlayerLabelInfo playerLabelInfo = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, group);
			BaseDLL.encode_int8(buffer, ref pos_, index);
			BaseDLL.encode_uint64(buffer, ref pos_, playerId);
			byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
			BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint16(buffer, ref pos_, playerLevel);
			BaseDLL.encode_uint32(buffer, ref pos_, playerSeasonLevel);
			BaseDLL.encode_int8(buffer, ref pos_, playerVipLevel);
			BaseDLL.encode_int8(buffer, ref pos_, playerOccu);
			BaseDLL.encode_int8(buffer, ref pos_, playerAwake);
			avatar.encode(buffer, ref pos_);
			BaseDLL.encode_int8(buffer, ref pos_, status);
			BaseDLL.encode_int8(buffer, ref pos_, readyStatus);
			playerLabelInfo.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref group);
			BaseDLL.decode_int8(buffer, ref pos_, ref index);
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
			BaseDLL.decode_uint32(buffer, ref pos_, ref playerSeasonLevel);
			BaseDLL.decode_int8(buffer, ref pos_, ref playerVipLevel);
			BaseDLL.decode_int8(buffer, ref pos_, ref playerOccu);
			BaseDLL.decode_int8(buffer, ref pos_, ref playerAwake);
			avatar.decode(buffer, ref pos_);
			BaseDLL.decode_int8(buffer, ref pos_, ref status);
			BaseDLL.decode_int8(buffer, ref pos_, ref readyStatus);
			playerLabelInfo.decode(buffer, ref pos_);
		}


		#endregion

	}

}
