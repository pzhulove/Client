using System;
using System.Text;

namespace Protocol
{
	public enum RoomType
	{
		/// <summary>
		/// 无效
		/// </summary>
		ROOM_TYPE_INVALID = 0,
		/// <summary>
		/// 3V3自由模式
		/// </summary>
		ROOM_TYPE_THREE_FREE = 1,
		/// <summary>
		/// 3V3匹配模式
		/// </summary>
		ROOM_TYPE_THREE_MATCH = 2,
		/// <summary>
		/// 3V3积分赛
		/// </summary>
		ROOM_TYPE_THREE_SCORE_WAR = 3,
		/// <summary>
		/// 乱斗模式
		/// </summary>
		ROOM_TYPE_MELEE = 4,
		ROOM_TYPE_MAX = 5,
	}

	public enum RoomStatus
	{
		/// <summary>
		/// 无效
		/// </summary>
		ROOM_STATUS_INVALID = 0,
		/// <summary>
		/// 打开状态
		/// </summary>
		ROOM_STATUS_OPEN = 1,
		/// <summary>
		/// 战斗准备
		/// </summary>
		ROOM_STATUS_READY = 2,
		/// <summary>
		/// 匹配阶段
		/// </summary>
		ROOM_STATUS_MATCH = 3,
		/// <summary>
		/// 战斗状态
		/// </summary>
		ROOM_STATUS_BATTLE = 4,
		ROOM_STATUS_NUM = 5,
	}

	public enum RoomSlotStatus
	{
		ROOM_SLOT_STATUS_INVALID = 0,
		/// <summary>
		/// 打开
		/// </summary>
		ROOM_SLOT_STATUS_OPEN = 1,
		/// <summary>
		/// 关闭
		/// </summary>
		ROOM_SLOT_STATUS_CLOSE = 2,
		/// <summary>
		/// 等待
		/// </summary>
		ROOM_SLOT_STATUS_WAIT = 3,
		/// <summary>
		/// 离线
		/// </summary>
		ROOM_SLOT_STATUS_OFFLINE = 4,
		ROOM_SLOT_STATUS_NUM = 5,
	}

	public enum RoomSlotGroup
	{
		/// <summary>
		/// 无效
		/// </summary>
		ROOM_SLOT_GROUP_INVALID = 0,
		/// <summary>
		/// 红队
		/// </summary>
		ROOM_SLOT_GROUP_RED = 1,
		/// <summary>
		/// 蓝队
		/// </summary>
		ROOM_SLOT_GROUP_BLUE = 2,
		ROOM_SLOT_GROUP_NUM = 3,
	}

	public enum RoomQuitReason
	{
		/// <summary>
		/// 无效
		/// </summary>
		ROOM_QUIT_REASON_INVALID = 0,
		/// <summary>
		/// 自己退出
		/// </summary>
		ROOM_QUIT_SELF = 1,
		/// <summary>
		/// 被房主踢出
		/// </summary>
		ROOM_QUIT_OWNER_KICK_OUT = 2,
		/// <summary>
		/// 解散
		/// </summary>
		ROOM_QUIT_DISMISS = 3,
		ROOM_QUIT_NUM = 4,
	}

	public enum RoomSlotReadyStatus
	{
		ROOM_SLOT_READY_STATUS_INVALID = 0,
		/// <summary>
		/// 接受
		/// </summary>
		ROOM_SLOT_READY_STATUS_ACCEPT = 1,
		/// <summary>
		/// 拒绝
		/// </summary>
		ROOM_SLOT_READY_STATUS_REFUSE = 2,
		/// <summary>
		/// 超时
		/// </summary>
		ROOM_SLOT_READY_STATUS_TIMEOUT = 3,
		ROOM_SLOT_READY_STATUS_NUM = 4,
	}

	public enum RoomSwapResult
	{
		ROOM_SWAP_RESULT_INVALID = 0,
		/// <summary>
		/// 接受
		/// </summary>
		ROOM_SWAP_RESULT_ACCEPT = 1,
		/// <summary>
		/// 拒绝
		/// </summary>
		ROOM_SWAP_RESULT_REFUSE = 2,
		/// <summary>
		/// 超时
		/// </summary>
		ROOM_SWAP_RESULT_TIMEOUT = 3,
		/// <summary>
		/// 取消
		/// </summary>
		ROOM_SWAP_RESULT_CANCEL = 4,
	}

	/// <summary>
	/// 房间位置信息
	/// </summary>
	public class RoomSlotInfo : Protocol.IProtocolStream
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
		public PlayerAvatar avatar = new PlayerAvatar();
		public byte status;
		public byte readyStatus;
		/// <summary>
		/// 玩家标签信息
		/// </summary>
		public PlayerLabelInfo playerLabelInfo = new PlayerLabelInfo();

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

			public void encode(MapViewStream buffer, ref int pos_)
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

			public void decode(MapViewStream buffer, ref int pos_)
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

			public int getLen()
			{
				int _len = 0;
				// group
				_len += 1;
				// index
				_len += 1;
				// playerId
				_len += 8;
				// playerName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(playerName);
					_len += 2 + _strBytes.Length;
				}
				// playerLevel
				_len += 2;
				// playerSeasonLevel
				_len += 4;
				// playerVipLevel
				_len += 1;
				// playerOccu
				_len += 1;
				// playerAwake
				_len += 1;
				// avatar
				_len += avatar.getLen();
				// status
				_len += 1;
				// readyStatus
				_len += 1;
				// playerLabelInfo
				_len += playerLabelInfo.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 房间简单信息
	/// </summary>
	public class RoomSimpleInfo : Protocol.IProtocolStream
	{
		public UInt32 id;
		public string name;
		public byte roomStatus;
		public byte roomType;
		public byte isLimitPlayerLevel;
		public UInt16 limitPlayerLevel;
		public byte isLimitPlayerSeasonLevel;
		public UInt32 limitPlayerSeasonLevel;
		public byte playerSize;
		public byte playerMaxSize;
		public byte isPassword;
		public UInt64 ownerId;
		public byte ownerOccu;
		public UInt32 ownerSeasonLevel;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, roomStatus);
				BaseDLL.encode_int8(buffer, ref pos_, roomType);
				BaseDLL.encode_int8(buffer, ref pos_, isLimitPlayerLevel);
				BaseDLL.encode_uint16(buffer, ref pos_, limitPlayerLevel);
				BaseDLL.encode_int8(buffer, ref pos_, isLimitPlayerSeasonLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, limitPlayerSeasonLevel);
				BaseDLL.encode_int8(buffer, ref pos_, playerSize);
				BaseDLL.encode_int8(buffer, ref pos_, playerMaxSize);
				BaseDLL.encode_int8(buffer, ref pos_, isPassword);
				BaseDLL.encode_uint64(buffer, ref pos_, ownerId);
				BaseDLL.encode_int8(buffer, ref pos_, ownerOccu);
				BaseDLL.encode_uint32(buffer, ref pos_, ownerSeasonLevel);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref roomStatus);
				BaseDLL.decode_int8(buffer, ref pos_, ref roomType);
				BaseDLL.decode_int8(buffer, ref pos_, ref isLimitPlayerLevel);
				BaseDLL.decode_uint16(buffer, ref pos_, ref limitPlayerLevel);
				BaseDLL.decode_int8(buffer, ref pos_, ref isLimitPlayerSeasonLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref limitPlayerSeasonLevel);
				BaseDLL.decode_int8(buffer, ref pos_, ref playerSize);
				BaseDLL.decode_int8(buffer, ref pos_, ref playerMaxSize);
				BaseDLL.decode_int8(buffer, ref pos_, ref isPassword);
				BaseDLL.decode_uint64(buffer, ref pos_, ref ownerId);
				BaseDLL.decode_int8(buffer, ref pos_, ref ownerOccu);
				BaseDLL.decode_uint32(buffer, ref pos_, ref ownerSeasonLevel);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, roomStatus);
				BaseDLL.encode_int8(buffer, ref pos_, roomType);
				BaseDLL.encode_int8(buffer, ref pos_, isLimitPlayerLevel);
				BaseDLL.encode_uint16(buffer, ref pos_, limitPlayerLevel);
				BaseDLL.encode_int8(buffer, ref pos_, isLimitPlayerSeasonLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, limitPlayerSeasonLevel);
				BaseDLL.encode_int8(buffer, ref pos_, playerSize);
				BaseDLL.encode_int8(buffer, ref pos_, playerMaxSize);
				BaseDLL.encode_int8(buffer, ref pos_, isPassword);
				BaseDLL.encode_uint64(buffer, ref pos_, ownerId);
				BaseDLL.encode_int8(buffer, ref pos_, ownerOccu);
				BaseDLL.encode_uint32(buffer, ref pos_, ownerSeasonLevel);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref roomStatus);
				BaseDLL.decode_int8(buffer, ref pos_, ref roomType);
				BaseDLL.decode_int8(buffer, ref pos_, ref isLimitPlayerLevel);
				BaseDLL.decode_uint16(buffer, ref pos_, ref limitPlayerLevel);
				BaseDLL.decode_int8(buffer, ref pos_, ref isLimitPlayerSeasonLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref limitPlayerSeasonLevel);
				BaseDLL.decode_int8(buffer, ref pos_, ref playerSize);
				BaseDLL.decode_int8(buffer, ref pos_, ref playerMaxSize);
				BaseDLL.decode_int8(buffer, ref pos_, ref isPassword);
				BaseDLL.decode_uint64(buffer, ref pos_, ref ownerId);
				BaseDLL.decode_int8(buffer, ref pos_, ref ownerOccu);
				BaseDLL.decode_uint32(buffer, ref pos_, ref ownerSeasonLevel);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// roomStatus
				_len += 1;
				// roomType
				_len += 1;
				// isLimitPlayerLevel
				_len += 1;
				// limitPlayerLevel
				_len += 2;
				// isLimitPlayerSeasonLevel
				_len += 1;
				// limitPlayerSeasonLevel
				_len += 4;
				// playerSize
				_len += 1;
				// playerMaxSize
				_len += 1;
				// isPassword
				_len += 1;
				// ownerId
				_len += 8;
				// ownerOccu
				_len += 1;
				// ownerSeasonLevel
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 房间信息
	/// </summary>
	public class RoomInfo : Protocol.IProtocolStream
	{
		public RoomSimpleInfo roomSimpleInfo = new RoomSimpleInfo();
		public RoomSlotInfo[] roomSlotInfos = new RoomSlotInfo[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				roomSimpleInfo.encode(buffer, ref pos_);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)roomSlotInfos.Length);
				for(int i = 0; i < roomSlotInfos.Length; i++)
				{
					roomSlotInfos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				roomSimpleInfo.decode(buffer, ref pos_);
				UInt16 roomSlotInfosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref roomSlotInfosCnt);
				roomSlotInfos = new RoomSlotInfo[roomSlotInfosCnt];
				for(int i = 0; i < roomSlotInfos.Length; i++)
				{
					roomSlotInfos[i] = new RoomSlotInfo();
					roomSlotInfos[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				roomSimpleInfo.encode(buffer, ref pos_);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)roomSlotInfos.Length);
				for(int i = 0; i < roomSlotInfos.Length; i++)
				{
					roomSlotInfos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				roomSimpleInfo.decode(buffer, ref pos_);
				UInt16 roomSlotInfosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref roomSlotInfosCnt);
				roomSlotInfos = new RoomSlotInfo[roomSlotInfosCnt];
				for(int i = 0; i < roomSlotInfos.Length; i++)
				{
					roomSlotInfos[i] = new RoomSlotInfo();
					roomSlotInfos[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// roomSimpleInfo
				_len += roomSimpleInfo.getLen();
				// roomSlotInfos
				_len += 2;
				for(int j = 0; j < roomSlotInfos.Length; j++)
				{
					_len += roomSlotInfos[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 房间列表信息
	/// </summary>
	public class RoomListInfo : Protocol.IProtocolStream
	{
		public UInt32 startIndex;
		public UInt32 total;
		public RoomSimpleInfo[] rooms = new RoomSimpleInfo[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, startIndex);
				BaseDLL.encode_uint32(buffer, ref pos_, total);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)rooms.Length);
				for(int i = 0; i < rooms.Length; i++)
				{
					rooms[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref startIndex);
				BaseDLL.decode_uint32(buffer, ref pos_, ref total);
				UInt16 roomsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref roomsCnt);
				rooms = new RoomSimpleInfo[roomsCnt];
				for(int i = 0; i < rooms.Length; i++)
				{
					rooms[i] = new RoomSimpleInfo();
					rooms[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, startIndex);
				BaseDLL.encode_uint32(buffer, ref pos_, total);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)rooms.Length);
				for(int i = 0; i < rooms.Length; i++)
				{
					rooms[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref startIndex);
				BaseDLL.decode_uint32(buffer, ref pos_, ref total);
				UInt16 roomsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref roomsCnt);
				rooms = new RoomSimpleInfo[roomsCnt];
				for(int i = 0; i < rooms.Length; i++)
				{
					rooms[i] = new RoomSimpleInfo();
					rooms[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// startIndex
				_len += 4;
				// total
				_len += 4;
				// rooms
				_len += 2;
				for(int j = 0; j < rooms.Length; j++)
				{
					_len += rooms[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 结算位置信息
	/// </summary>
	public class RoomSlotBattleEndInfo : Protocol.IProtocolStream
	{
		public byte resultFlag;
		public UInt32 roomId;
		public byte roomType;
		public UInt64 roleId;
		public byte seat;
		public UInt32 seasonLevel;
		public UInt32 seasonStar;
		public UInt32 seasonExp;
		public UInt32 scoreWarBaseScore;
		public UInt32 scoreWarContriScore;
		public UInt32 getHonor;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, resultFlag);
				BaseDLL.encode_uint32(buffer, ref pos_, roomId);
				BaseDLL.encode_int8(buffer, ref pos_, roomType);
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_int8(buffer, ref pos_, seat);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonStar);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonExp);
				BaseDLL.encode_uint32(buffer, ref pos_, scoreWarBaseScore);
				BaseDLL.encode_uint32(buffer, ref pos_, scoreWarContriScore);
				BaseDLL.encode_uint32(buffer, ref pos_, getHonor);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref resultFlag);
				BaseDLL.decode_uint32(buffer, ref pos_, ref roomId);
				BaseDLL.decode_int8(buffer, ref pos_, ref roomType);
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_int8(buffer, ref pos_, ref seat);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonStar);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonExp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref scoreWarBaseScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref scoreWarContriScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref getHonor);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, resultFlag);
				BaseDLL.encode_uint32(buffer, ref pos_, roomId);
				BaseDLL.encode_int8(buffer, ref pos_, roomType);
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_int8(buffer, ref pos_, seat);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonStar);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonExp);
				BaseDLL.encode_uint32(buffer, ref pos_, scoreWarBaseScore);
				BaseDLL.encode_uint32(buffer, ref pos_, scoreWarContriScore);
				BaseDLL.encode_uint32(buffer, ref pos_, getHonor);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref resultFlag);
				BaseDLL.decode_uint32(buffer, ref pos_, ref roomId);
				BaseDLL.decode_int8(buffer, ref pos_, ref roomType);
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_int8(buffer, ref pos_, ref seat);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonStar);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonExp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref scoreWarBaseScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref scoreWarContriScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref getHonor);
			}

			public int getLen()
			{
				int _len = 0;
				// resultFlag
				_len += 1;
				// roomId
				_len += 4;
				// roomType
				_len += 1;
				// roleId
				_len += 8;
				// seat
				_len += 1;
				// seasonLevel
				_len += 4;
				// seasonStar
				_len += 4;
				// seasonExp
				_len += 4;
				// scoreWarBaseScore
				_len += 4;
				// scoreWarContriScore
				_len += 4;
				// getHonor
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 同步房间信息
	/// </summary>
	[Protocol]
	public class WorldSyncRoomInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607801;
		public UInt32 Sequence;
		public RoomInfo info = new RoomInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				info.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				info.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				info.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				info.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// info
				_len += info.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 同步房间简单信息
	/// </summary>
	[Protocol]
	public class WorldSyncRoomSimpleInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607802;
		public UInt32 Sequence;
		public RoomSimpleInfo info = new RoomSimpleInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				info.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				info.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				info.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				info.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// info
				_len += info.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 同步房间玩家信息
	/// </summary>
	[Protocol]
	public class WorldSyncRoomSlotInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607803;
		public UInt32 Sequence;
		public RoomSlotInfo slotInfo = new RoomSlotInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				slotInfo.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				slotInfo.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				slotInfo.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				slotInfo.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// slotInfo
				_len += slotInfo.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// server->client 通知被邀请玩家 邀请信息
	/// </summary>
	[Protocol]
	public class WorldSyncRoomInviteInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607804;
		public UInt32 Sequence;
		/// <summary>
		/// 房间号
		/// </summary>
		public UInt32 roomId;
		/// <summary>
		/// 房间名
		/// </summary>
		public string roomName;
		/// <summary>
		/// 房间类型
		/// </summary>
		public byte roomType;
		/// <summary>
		/// 邀请者ID
		/// </summary>
		public UInt64 inviterId;
		/// <summary>
		/// 邀请者名字
		/// </summary>
		public string inviterName;
		/// <summary>
		/// 邀请者职业
		/// </summary>
		public byte inviterOccu;
		/// <summary>
		/// 邀请者觉醒
		/// </summary>
		public byte inviterAwaken;
		/// <summary>
		/// 邀请者等级
		/// </summary>
		public UInt16 inviterLevel;
		/// <summary>
		/// 房间人数
		/// </summary>
		public UInt32 playerSize;
		/// <summary>
		/// 房间最大人数
		/// </summary>
		public UInt32 playerMaxSize;
		/// <summary>
		/// 队伍
		/// </summary>
		public byte slotGroup;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, roomId);
				byte[] roomNameBytes = StringHelper.StringToUTF8Bytes(roomName);
				BaseDLL.encode_string(buffer, ref pos_, roomNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, roomType);
				BaseDLL.encode_uint64(buffer, ref pos_, inviterId);
				byte[] inviterNameBytes = StringHelper.StringToUTF8Bytes(inviterName);
				BaseDLL.encode_string(buffer, ref pos_, inviterNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, inviterOccu);
				BaseDLL.encode_int8(buffer, ref pos_, inviterAwaken);
				BaseDLL.encode_uint16(buffer, ref pos_, inviterLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, playerSize);
				BaseDLL.encode_uint32(buffer, ref pos_, playerMaxSize);
				BaseDLL.encode_int8(buffer, ref pos_, slotGroup);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref roomId);
				UInt16 roomNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref roomNameLen);
				byte[] roomNameBytes = new byte[roomNameLen];
				for(int i = 0; i < roomNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref roomNameBytes[i]);
				}
				roomName = StringHelper.BytesToString(roomNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref roomType);
				BaseDLL.decode_uint64(buffer, ref pos_, ref inviterId);
				UInt16 inviterNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref inviterNameLen);
				byte[] inviterNameBytes = new byte[inviterNameLen];
				for(int i = 0; i < inviterNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref inviterNameBytes[i]);
				}
				inviterName = StringHelper.BytesToString(inviterNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref inviterOccu);
				BaseDLL.decode_int8(buffer, ref pos_, ref inviterAwaken);
				BaseDLL.decode_uint16(buffer, ref pos_, ref inviterLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerSize);
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerMaxSize);
				BaseDLL.decode_int8(buffer, ref pos_, ref slotGroup);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, roomId);
				byte[] roomNameBytes = StringHelper.StringToUTF8Bytes(roomName);
				BaseDLL.encode_string(buffer, ref pos_, roomNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, roomType);
				BaseDLL.encode_uint64(buffer, ref pos_, inviterId);
				byte[] inviterNameBytes = StringHelper.StringToUTF8Bytes(inviterName);
				BaseDLL.encode_string(buffer, ref pos_, inviterNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, inviterOccu);
				BaseDLL.encode_int8(buffer, ref pos_, inviterAwaken);
				BaseDLL.encode_uint16(buffer, ref pos_, inviterLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, playerSize);
				BaseDLL.encode_uint32(buffer, ref pos_, playerMaxSize);
				BaseDLL.encode_int8(buffer, ref pos_, slotGroup);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref roomId);
				UInt16 roomNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref roomNameLen);
				byte[] roomNameBytes = new byte[roomNameLen];
				for(int i = 0; i < roomNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref roomNameBytes[i]);
				}
				roomName = StringHelper.BytesToString(roomNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref roomType);
				BaseDLL.decode_uint64(buffer, ref pos_, ref inviterId);
				UInt16 inviterNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref inviterNameLen);
				byte[] inviterNameBytes = new byte[inviterNameLen];
				for(int i = 0; i < inviterNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref inviterNameBytes[i]);
				}
				inviterName = StringHelper.BytesToString(inviterNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref inviterOccu);
				BaseDLL.decode_int8(buffer, ref pos_, ref inviterAwaken);
				BaseDLL.decode_uint16(buffer, ref pos_, ref inviterLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerSize);
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerMaxSize);
				BaseDLL.decode_int8(buffer, ref pos_, ref slotGroup);
			}

			public int getLen()
			{
				int _len = 0;
				// roomId
				_len += 4;
				// roomName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(roomName);
					_len += 2 + _strBytes.Length;
				}
				// roomType
				_len += 1;
				// inviterId
				_len += 8;
				// inviterName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(inviterName);
					_len += 2 + _strBytes.Length;
				}
				// inviterOccu
				_len += 1;
				// inviterAwaken
				_len += 1;
				// inviterLevel
				_len += 2;
				// playerSize
				_len += 4;
				// playerMaxSize
				_len += 4;
				// slotGroup
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// server->client 通知被踢出玩家
	/// </summary>
	[Protocol]
	public class WorldSyncRoomKickOutInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607805;
		public UInt32 Sequence;
		/// <summary>
		/// 踢出原因
		/// </summary>
		public UInt32 reason;
		/// <summary>
		/// 踢出玩家id
		/// </summary>
		public UInt64 kickPlayerId;
		/// <summary>
		/// 踢出玩家名字
		/// </summary>
		public string kickPlayerName;
		/// <summary>
		/// 房间id
		/// </summary>
		public UInt32 roomId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
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

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, reason);
				BaseDLL.encode_uint64(buffer, ref pos_, kickPlayerId);
				byte[] kickPlayerNameBytes = StringHelper.StringToUTF8Bytes(kickPlayerName);
				BaseDLL.encode_string(buffer, ref pos_, kickPlayerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, roomId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
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

			public int getLen()
			{
				int _len = 0;
				// reason
				_len += 4;
				// kickPlayerId
				_len += 8;
				// kickPlayerName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(kickPlayerName);
					_len += 2 + _strBytes.Length;
				}
				// roomId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// server->client 通知邀请者,被邀请玩家的返回
	/// </summary>
	[Protocol]
	public class WorldSyncRoomBeInviteInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607806;
		public UInt32 Sequence;
		/// <summary>
		/// 玩家ID
		/// </summary>
		public UInt64 playerId;
		/// <summary>
		/// 是否接受
		/// </summary>
		public byte isAccept;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
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

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				BaseDLL.encode_int8(buffer, ref pos_, isAccept);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
				BaseDLL.decode_int8(buffer, ref pos_, ref isAccept);
			}

			public int getLen()
			{
				int _len = 0;
				// playerId
				_len += 8;
				// isAccept
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 通知玩家交换位置信息
	/// </summary>
	[Protocol]
	public class WorldSyncRoomSwapSlotInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
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

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
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

			public void encode(MapViewStream buffer, ref int pos_)
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

			public void decode(MapViewStream buffer, ref int pos_)
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

			public int getLen()
			{
				int _len = 0;
				// playerId
				_len += 8;
				// playerName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(playerName);
					_len += 2 + _strBytes.Length;
				}
				// playerLevel
				_len += 2;
				// playerOccu
				_len += 1;
				// playerAwaken
				_len += 1;
				// slotGroup
				_len += 1;
				// slotIndex
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///   server->client 通知玩家交换位置返回信息
	/// </summary>
	[Protocol]
	public class WorldSyncRoomSwapResultInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607808;
		public UInt32 Sequence;
		public byte result;
		public UInt64 playerId;
		public string playerName;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, result);
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
				BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref result);
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
				UInt16 playerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerNameLen);
				byte[] playerNameBytes = new byte[playerNameLen];
				for(int i = 0; i < playerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref playerNameBytes[i]);
				}
				playerName = StringHelper.BytesToString(playerNameBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, result);
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
				BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref result);
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
				UInt16 playerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerNameLen);
				byte[] playerNameBytes = new byte[playerNameLen];
				for(int i = 0; i < playerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref playerNameBytes[i]);
				}
				playerName = StringHelper.BytesToString(playerNameBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 1;
				// playerId
				_len += 8;
				// playerName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(playerName);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 通知玩家密码信息
	/// </summary>
	[Protocol]
	public class WorldSyncRoomPasswordInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607809;
		public UInt32 Sequence;
		public UInt32 roomId;
		public string password;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, roomId);
				byte[] passwordBytes = StringHelper.StringToUTF8Bytes(password);
				BaseDLL.encode_string(buffer, ref pos_, passwordBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref roomId);
				UInt16 passwordLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref passwordLen);
				byte[] passwordBytes = new byte[passwordLen];
				for(int i = 0; i < passwordLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref passwordBytes[i]);
				}
				password = StringHelper.BytesToString(passwordBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, roomId);
				byte[] passwordBytes = StringHelper.StringToUTF8Bytes(password);
				BaseDLL.encode_string(buffer, ref pos_, passwordBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref roomId);
				UInt16 passwordLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref passwordLen);
				byte[] passwordBytes = new byte[passwordLen];
				for(int i = 0; i < passwordLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref passwordBytes[i]);
				}
				password = StringHelper.BytesToString(passwordBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// roomId
				_len += 4;
				// password
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(password);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneRoomMatchPkRaceEnd : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507802;
		public UInt32 Sequence;
		/// <summary>
		///  PK类型，对应枚举(PkType)
		/// </summary>
		public byte pkType;
		public UInt64 raceId;
		public byte result;
		public UInt32 oldPkValue;
		public UInt32 newPkValue;
		public UInt32 oldMatchScore;
		public UInt32 newMatchScore;
		/// <summary>
		///  初始决斗币数量
		/// </summary>
		public UInt32 oldPkCoin;
		/// <summary>
		///  战斗获得的决斗币
		/// </summary>
		public UInt32 addPkCoinFromRace;
		/// <summary>
		///  今日战斗获得的全部决斗币
		/// </summary>
		public UInt32 totalPkCoinFromRace;
		/// <summary>
		///  是否在PVP活动期间
		/// </summary>
		public byte isInPvPActivity;
		/// <summary>
		///  活动额外获得的决斗币
		/// </summary>
		public UInt32 addPkCoinFromActivity;
		/// <summary>
		///  今日活动获得的全部决斗币
		/// </summary>
		public UInt32 totalPkCoinFromActivity;
		/// <summary>
		///  原段位
		/// </summary>
		public UInt32 oldSeasonLevel;
		/// <summary>
		///  现段位
		/// </summary>
		public UInt32 newSeasonLevel;
		/// <summary>
		/// 原星
		/// </summary>
		public UInt32 oldSeasonStar;
		/// <summary>
		/// 现星
		/// </summary>
		public UInt32 newSeasonStar;
		/// <summary>
		/// 原经验
		/// </summary>
		public UInt32 oldSeasonExp;
		/// <summary>
		/// 现经验
		/// </summary>
		public UInt32 newSeasonExp;
		/// <summary>
		/// 改变的经验
		/// </summary>
		public Int32 changeSeasonExp;
		/// <summary>
		/// 队伍信息
		/// </summary>
		public RoomSlotBattleEndInfo[] slotInfos = new RoomSlotBattleEndInfo[0];
		/// <summary>
		/// 获得荣誉
		/// </summary>
		public UInt32 getHonor;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, pkType);
				BaseDLL.encode_uint64(buffer, ref pos_, raceId);
				BaseDLL.encode_int8(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, oldPkValue);
				BaseDLL.encode_uint32(buffer, ref pos_, newPkValue);
				BaseDLL.encode_uint32(buffer, ref pos_, oldMatchScore);
				BaseDLL.encode_uint32(buffer, ref pos_, newMatchScore);
				BaseDLL.encode_uint32(buffer, ref pos_, oldPkCoin);
				BaseDLL.encode_uint32(buffer, ref pos_, addPkCoinFromRace);
				BaseDLL.encode_uint32(buffer, ref pos_, totalPkCoinFromRace);
				BaseDLL.encode_int8(buffer, ref pos_, isInPvPActivity);
				BaseDLL.encode_uint32(buffer, ref pos_, addPkCoinFromActivity);
				BaseDLL.encode_uint32(buffer, ref pos_, totalPkCoinFromActivity);
				BaseDLL.encode_uint32(buffer, ref pos_, oldSeasonLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, newSeasonLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, oldSeasonStar);
				BaseDLL.encode_uint32(buffer, ref pos_, newSeasonStar);
				BaseDLL.encode_uint32(buffer, ref pos_, oldSeasonExp);
				BaseDLL.encode_uint32(buffer, ref pos_, newSeasonExp);
				BaseDLL.encode_int32(buffer, ref pos_, changeSeasonExp);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)slotInfos.Length);
				for(int i = 0; i < slotInfos.Length; i++)
				{
					slotInfos[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, getHonor);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref pkType);
				BaseDLL.decode_uint64(buffer, ref pos_, ref raceId);
				BaseDLL.decode_int8(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldPkValue);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newPkValue);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldMatchScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newMatchScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldPkCoin);
				BaseDLL.decode_uint32(buffer, ref pos_, ref addPkCoinFromRace);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalPkCoinFromRace);
				BaseDLL.decode_int8(buffer, ref pos_, ref isInPvPActivity);
				BaseDLL.decode_uint32(buffer, ref pos_, ref addPkCoinFromActivity);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalPkCoinFromActivity);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldSeasonLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newSeasonLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldSeasonStar);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newSeasonStar);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldSeasonExp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newSeasonExp);
				BaseDLL.decode_int32(buffer, ref pos_, ref changeSeasonExp);
				UInt16 slotInfosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref slotInfosCnt);
				slotInfos = new RoomSlotBattleEndInfo[slotInfosCnt];
				for(int i = 0; i < slotInfos.Length; i++)
				{
					slotInfos[i] = new RoomSlotBattleEndInfo();
					slotInfos[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref getHonor);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, pkType);
				BaseDLL.encode_uint64(buffer, ref pos_, raceId);
				BaseDLL.encode_int8(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, oldPkValue);
				BaseDLL.encode_uint32(buffer, ref pos_, newPkValue);
				BaseDLL.encode_uint32(buffer, ref pos_, oldMatchScore);
				BaseDLL.encode_uint32(buffer, ref pos_, newMatchScore);
				BaseDLL.encode_uint32(buffer, ref pos_, oldPkCoin);
				BaseDLL.encode_uint32(buffer, ref pos_, addPkCoinFromRace);
				BaseDLL.encode_uint32(buffer, ref pos_, totalPkCoinFromRace);
				BaseDLL.encode_int8(buffer, ref pos_, isInPvPActivity);
				BaseDLL.encode_uint32(buffer, ref pos_, addPkCoinFromActivity);
				BaseDLL.encode_uint32(buffer, ref pos_, totalPkCoinFromActivity);
				BaseDLL.encode_uint32(buffer, ref pos_, oldSeasonLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, newSeasonLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, oldSeasonStar);
				BaseDLL.encode_uint32(buffer, ref pos_, newSeasonStar);
				BaseDLL.encode_uint32(buffer, ref pos_, oldSeasonExp);
				BaseDLL.encode_uint32(buffer, ref pos_, newSeasonExp);
				BaseDLL.encode_int32(buffer, ref pos_, changeSeasonExp);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)slotInfos.Length);
				for(int i = 0; i < slotInfos.Length; i++)
				{
					slotInfos[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, getHonor);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref pkType);
				BaseDLL.decode_uint64(buffer, ref pos_, ref raceId);
				BaseDLL.decode_int8(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldPkValue);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newPkValue);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldMatchScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newMatchScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldPkCoin);
				BaseDLL.decode_uint32(buffer, ref pos_, ref addPkCoinFromRace);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalPkCoinFromRace);
				BaseDLL.decode_int8(buffer, ref pos_, ref isInPvPActivity);
				BaseDLL.decode_uint32(buffer, ref pos_, ref addPkCoinFromActivity);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalPkCoinFromActivity);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldSeasonLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newSeasonLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldSeasonStar);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newSeasonStar);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldSeasonExp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newSeasonExp);
				BaseDLL.decode_int32(buffer, ref pos_, ref changeSeasonExp);
				UInt16 slotInfosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref slotInfosCnt);
				slotInfos = new RoomSlotBattleEndInfo[slotInfosCnt];
				for(int i = 0; i < slotInfos.Length; i++)
				{
					slotInfos[i] = new RoomSlotBattleEndInfo();
					slotInfos[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref getHonor);
			}

			public int getLen()
			{
				int _len = 0;
				// pkType
				_len += 1;
				// raceId
				_len += 8;
				// result
				_len += 1;
				// oldPkValue
				_len += 4;
				// newPkValue
				_len += 4;
				// oldMatchScore
				_len += 4;
				// newMatchScore
				_len += 4;
				// oldPkCoin
				_len += 4;
				// addPkCoinFromRace
				_len += 4;
				// totalPkCoinFromRace
				_len += 4;
				// isInPvPActivity
				_len += 1;
				// addPkCoinFromActivity
				_len += 4;
				// totalPkCoinFromActivity
				_len += 4;
				// oldSeasonLevel
				_len += 4;
				// newSeasonLevel
				_len += 4;
				// oldSeasonStar
				_len += 4;
				// newSeasonStar
				_len += 4;
				// oldSeasonExp
				_len += 4;
				// newSeasonExp
				_len += 4;
				// changeSeasonExp
				_len += 4;
				// slotInfos
				_len += 2;
				for(int j = 0; j < slotInfos.Length; j++)
				{
					_len += slotInfos[j].getLen();
				}
				// getHonor
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->server 请求房间列表
	/// </summary>
	[Protocol]
	public class WorldRoomListReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607811;
		public UInt32 Sequence;
		/// <summary>
		///  玩家等级
		/// </summary>
		public UInt16 limitPlayerLevel;
		/// <summary>
		///  玩家段位
		/// </summary>
		public UInt32 limitPlayerSeasonLevel;
		/// <summary>
		///  房间状态
		/// </summary>
		public byte roomStatus;
		/// <summary>
		///  房间类型
		/// </summary>
		public byte roomType;
		/// <summary>
		///  是否有密码
		/// </summary>
		public byte isPassword;
		/// <summary>
		///  开始位置
		/// </summary>
		public UInt32 startIndex;
		/// <summary>
		///  个数
		/// </summary>
		public UInt32 count;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, limitPlayerLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, limitPlayerSeasonLevel);
				BaseDLL.encode_int8(buffer, ref pos_, roomStatus);
				BaseDLL.encode_int8(buffer, ref pos_, roomType);
				BaseDLL.encode_int8(buffer, ref pos_, isPassword);
				BaseDLL.encode_uint32(buffer, ref pos_, startIndex);
				BaseDLL.encode_uint32(buffer, ref pos_, count);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref limitPlayerLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref limitPlayerSeasonLevel);
				BaseDLL.decode_int8(buffer, ref pos_, ref roomStatus);
				BaseDLL.decode_int8(buffer, ref pos_, ref roomType);
				BaseDLL.decode_int8(buffer, ref pos_, ref isPassword);
				BaseDLL.decode_uint32(buffer, ref pos_, ref startIndex);
				BaseDLL.decode_uint32(buffer, ref pos_, ref count);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, limitPlayerLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, limitPlayerSeasonLevel);
				BaseDLL.encode_int8(buffer, ref pos_, roomStatus);
				BaseDLL.encode_int8(buffer, ref pos_, roomType);
				BaseDLL.encode_int8(buffer, ref pos_, isPassword);
				BaseDLL.encode_uint32(buffer, ref pos_, startIndex);
				BaseDLL.encode_uint32(buffer, ref pos_, count);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref limitPlayerLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref limitPlayerSeasonLevel);
				BaseDLL.decode_int8(buffer, ref pos_, ref roomStatus);
				BaseDLL.decode_int8(buffer, ref pos_, ref roomType);
				BaseDLL.decode_int8(buffer, ref pos_, ref isPassword);
				BaseDLL.decode_uint32(buffer, ref pos_, ref startIndex);
				BaseDLL.decode_uint32(buffer, ref pos_, ref count);
			}

			public int getLen()
			{
				int _len = 0;
				// limitPlayerLevel
				_len += 2;
				// limitPlayerSeasonLevel
				_len += 4;
				// roomStatus
				_len += 1;
				// roomType
				_len += 1;
				// isPassword
				_len += 1;
				// startIndex
				_len += 4;
				// count
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 请求房间列表返回
	/// </summary>
	[Protocol]
	public class WorldRoomListRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607812;
		public UInt32 Sequence;
		public UInt32 result;
		public RoomListInfo roomList = new RoomListInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				roomList.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				roomList.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				roomList.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				roomList.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// roomList
				_len += roomList.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->server 请求创建或更新房间
	/// </summary>
	[Protocol]
	public class WorldUpdateRoomReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607813;
		public UInt32 Sequence;
		/// <summary>
		/// 房间号
		/// </summary>
		public UInt32 roomId;
		/// <summary>
		/// 房间类型
		/// </summary>
		public byte roomType;
		/// <summary>
		/// 房间名
		/// </summary>
		public string name;
		/// <summary>
		/// 房间密码
		/// </summary>
		public string password;
		/// <summary>
		/// 是否启用房间限制等级
		/// </summary>
		public byte isLimitPlayerLevel;
		/// <summary>
		/// 房间限制等级
		/// </summary>
		public UInt16 limitPlayerLevel;
		/// <summary>
		/// 是否启用房间限制段位
		/// </summary>
		public byte isLimitPlayerSeasonLevel;
		/// <summary>
		/// 房间限制段位
		/// </summary>
		public UInt32 limitPlayerSeasonLevel;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, roomId);
				BaseDLL.encode_int8(buffer, ref pos_, roomType);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				byte[] passwordBytes = StringHelper.StringToUTF8Bytes(password);
				BaseDLL.encode_string(buffer, ref pos_, passwordBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, isLimitPlayerLevel);
				BaseDLL.encode_uint16(buffer, ref pos_, limitPlayerLevel);
				BaseDLL.encode_int8(buffer, ref pos_, isLimitPlayerSeasonLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, limitPlayerSeasonLevel);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref roomId);
				BaseDLL.decode_int8(buffer, ref pos_, ref roomType);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				UInt16 passwordLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref passwordLen);
				byte[] passwordBytes = new byte[passwordLen];
				for(int i = 0; i < passwordLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref passwordBytes[i]);
				}
				password = StringHelper.BytesToString(passwordBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref isLimitPlayerLevel);
				BaseDLL.decode_uint16(buffer, ref pos_, ref limitPlayerLevel);
				BaseDLL.decode_int8(buffer, ref pos_, ref isLimitPlayerSeasonLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref limitPlayerSeasonLevel);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, roomId);
				BaseDLL.encode_int8(buffer, ref pos_, roomType);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				byte[] passwordBytes = StringHelper.StringToUTF8Bytes(password);
				BaseDLL.encode_string(buffer, ref pos_, passwordBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, isLimitPlayerLevel);
				BaseDLL.encode_uint16(buffer, ref pos_, limitPlayerLevel);
				BaseDLL.encode_int8(buffer, ref pos_, isLimitPlayerSeasonLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, limitPlayerSeasonLevel);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref roomId);
				BaseDLL.decode_int8(buffer, ref pos_, ref roomType);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				UInt16 passwordLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref passwordLen);
				byte[] passwordBytes = new byte[passwordLen];
				for(int i = 0; i < passwordLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref passwordBytes[i]);
				}
				password = StringHelper.BytesToString(passwordBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref isLimitPlayerLevel);
				BaseDLL.decode_uint16(buffer, ref pos_, ref limitPlayerLevel);
				BaseDLL.decode_int8(buffer, ref pos_, ref isLimitPlayerSeasonLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref limitPlayerSeasonLevel);
			}

			public int getLen()
			{
				int _len = 0;
				// roomId
				_len += 4;
				// roomType
				_len += 1;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// password
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(password);
					_len += 2 + _strBytes.Length;
				}
				// isLimitPlayerLevel
				_len += 1;
				// limitPlayerLevel
				_len += 2;
				// isLimitPlayerSeasonLevel
				_len += 1;
				// limitPlayerSeasonLevel
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 请求创建或更新房间返回
	/// </summary>
	[Protocol]
	public class WorldUpdateRoomRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607814;
		public UInt32 Sequence;
		/// <summary>
		/// 返回值
		/// </summary>
		public UInt32 result;
		/// <summary>
		/// 房间信息
		/// </summary>
		public RoomInfo info = new RoomInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				info.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				info.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				info.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				info.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// info
				_len += info.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->server 请求进入房间
	/// </summary>
	[Protocol]
	public class WorldJoinRoomReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607815;
		public UInt32 Sequence;
		/// <summary>
		///  房间号
		/// </summary>
		public UInt32 roomId;
		/// <summary>
		/// 房间类型
		/// </summary>
		public byte roomType;
		/// <summary>
		/// 密码
		/// </summary>
		public string password;
		/// <summary>
		/// 创建时间
		/// </summary>
		public UInt32 createTime;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, roomId);
				BaseDLL.encode_int8(buffer, ref pos_, roomType);
				byte[] passwordBytes = StringHelper.StringToUTF8Bytes(password);
				BaseDLL.encode_string(buffer, ref pos_, passwordBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, createTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref roomId);
				BaseDLL.decode_int8(buffer, ref pos_, ref roomType);
				UInt16 passwordLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref passwordLen);
				byte[] passwordBytes = new byte[passwordLen];
				for(int i = 0; i < passwordLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref passwordBytes[i]);
				}
				password = StringHelper.BytesToString(passwordBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref createTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, roomId);
				BaseDLL.encode_int8(buffer, ref pos_, roomType);
				byte[] passwordBytes = StringHelper.StringToUTF8Bytes(password);
				BaseDLL.encode_string(buffer, ref pos_, passwordBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, createTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref roomId);
				BaseDLL.decode_int8(buffer, ref pos_, ref roomType);
				UInt16 passwordLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref passwordLen);
				byte[] passwordBytes = new byte[passwordLen];
				for(int i = 0; i < passwordLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref passwordBytes[i]);
				}
				password = StringHelper.BytesToString(passwordBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref createTime);
			}

			public int getLen()
			{
				int _len = 0;
				// roomId
				_len += 4;
				// roomType
				_len += 1;
				// password
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(password);
					_len += 2 + _strBytes.Length;
				}
				// createTime
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 请求进入房间返回
	/// </summary>
	[Protocol]
	public class WorldJoinRoomRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607816;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
		/// </summary>
		public UInt32 result;
		/// <summary>
		/// 房间信息
		/// </summary>
		public RoomInfo info = new RoomInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				info.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				info.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				info.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				info.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// info
				_len += info.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->server 请求退出房间
	/// </summary>
	[Protocol]
	public class WorldQuitRoomReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607817;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 请求退出房间返回
	/// </summary>
	[Protocol]
	public class WorldQuitRoomRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607818;
		public UInt32 Sequence;
		/// <summary>
		/// 返回值
		/// </summary>
		public UInt32 result;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->server 请求踢出房间
	/// </summary>
	[Protocol]
	public class WorldKickOutRoomReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607819;
		public UInt32 Sequence;
		public UInt64 playerId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
			}

			public int getLen()
			{
				int _len = 0;
				// playerId
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 请求踢出房间返回
	/// </summary>
	[Protocol]
	public class WorldKickOutRoomRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607820;
		public UInt32 Sequence;
		public UInt32 result;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->server 请求邀请进入房间
	/// </summary>
	[Protocol]
	public class WorldInviteJoinRoomReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607823;
		public UInt32 Sequence;
		public UInt64 playerId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
			}

			public int getLen()
			{
				int _len = 0;
				// playerId
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 请求邀请进入房间返回
	/// </summary>
	[Protocol]
	public class WorldInviteJoinRoomRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607824;
		public UInt32 Sequence;
		public UInt32 result;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->server 请求转让房主
	/// </summary>
	[Protocol]
	public class WorldChangeRoomOwnerReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607825;
		public UInt32 Sequence;
		public UInt64 playerId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
			}

			public int getLen()
			{
				int _len = 0;
				// playerId
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 请求转让房主返回
	/// </summary>
	[Protocol]
	public class WorldChangeRoomOwnerRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607826;
		public UInt32 Sequence;
		public UInt32 result;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->server 被邀请请求
	/// </summary>
	[Protocol]
	public class WorldBeInviteRoomReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607827;
		public UInt32 Sequence;
		/// <summary>
		///  房间号
		/// </summary>
		public UInt32 roomId;
		/// <summary>
		///  邀请玩家ID
		/// </summary>
		public UInt64 invitePlayerId;
		/// <summary>
		///  是否接受
		/// </summary>
		public byte isAccept;
		/// <summary>
		///  队伍
		/// </summary>
		public byte slotGroup;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
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

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, roomId);
				BaseDLL.encode_uint64(buffer, ref pos_, invitePlayerId);
				BaseDLL.encode_int8(buffer, ref pos_, isAccept);
				BaseDLL.encode_int8(buffer, ref pos_, slotGroup);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref roomId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref invitePlayerId);
				BaseDLL.decode_int8(buffer, ref pos_, ref isAccept);
				BaseDLL.decode_int8(buffer, ref pos_, ref slotGroup);
			}

			public int getLen()
			{
				int _len = 0;
				// roomId
				_len += 4;
				// invitePlayerId
				_len += 8;
				// isAccept
				_len += 1;
				// slotGroup
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 被邀请请求返回
	/// </summary>
	[Protocol]
	public class WorldBeInviteRoomRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607828;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
		/// </summary>
		public UInt32 result;
		/// <summary>
		///  房间信息
		/// </summary>
		public RoomInfo roomInfo = new RoomInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
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

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				roomInfo.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				roomInfo.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// roomInfo
				_len += roomInfo.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->server 请求关闭打开位置
	/// </summary>
	[Protocol]
	public class WorldRoomCloseSlotReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607829;
		public UInt32 Sequence;
		/// <summary>
		/// 队伍
		/// </summary>
		public byte slotGroup;
		/// <summary>
		/// 位置
		/// </summary>
		public byte index;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, slotGroup);
				BaseDLL.encode_int8(buffer, ref pos_, index);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref slotGroup);
				BaseDLL.decode_int8(buffer, ref pos_, ref index);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, slotGroup);
				BaseDLL.encode_int8(buffer, ref pos_, index);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref slotGroup);
				BaseDLL.decode_int8(buffer, ref pos_, ref index);
			}

			public int getLen()
			{
				int _len = 0;
				// slotGroup
				_len += 1;
				// index
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///   server->client 请求关闭打开位置返回
	/// </summary>
	[Protocol]
	public class WorldRoomCloseSlotRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607830;
		public UInt32 Sequence;
		public UInt32 result;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->server 请求房间交换位置
	/// </summary>
	[Protocol]
	public class WorldRoomSwapSlotReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607831;
		public UInt32 Sequence;
		public UInt32 roomId;
		public byte slotGroup;
		public byte index;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, roomId);
				BaseDLL.encode_int8(buffer, ref pos_, slotGroup);
				BaseDLL.encode_int8(buffer, ref pos_, index);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref roomId);
				BaseDLL.decode_int8(buffer, ref pos_, ref slotGroup);
				BaseDLL.decode_int8(buffer, ref pos_, ref index);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, roomId);
				BaseDLL.encode_int8(buffer, ref pos_, slotGroup);
				BaseDLL.encode_int8(buffer, ref pos_, index);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref roomId);
				BaseDLL.decode_int8(buffer, ref pos_, ref slotGroup);
				BaseDLL.decode_int8(buffer, ref pos_, ref index);
			}

			public int getLen()
			{
				int _len = 0;
				// roomId
				_len += 4;
				// slotGroup
				_len += 1;
				// index
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 请求房间交换位置返回
	/// </summary>
	[Protocol]
	public class WorldRoomSwapSlotRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607832;
		public UInt32 Sequence;
		public UInt32 result;
		public UInt64 playerId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// playerId
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->server 请求响应房间交换位置
	/// </summary>
	[Protocol]
	public class WorldRoomResponseSwapSlotReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607833;
		public UInt32 Sequence;
		public byte isAccept;
		public byte slotGroup;
		public byte slotIndex;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, isAccept);
				BaseDLL.encode_int8(buffer, ref pos_, slotGroup);
				BaseDLL.encode_int8(buffer, ref pos_, slotIndex);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isAccept);
				BaseDLL.decode_int8(buffer, ref pos_, ref slotGroup);
				BaseDLL.decode_int8(buffer, ref pos_, ref slotIndex);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, isAccept);
				BaseDLL.encode_int8(buffer, ref pos_, slotGroup);
				BaseDLL.encode_int8(buffer, ref pos_, slotIndex);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isAccept);
				BaseDLL.decode_int8(buffer, ref pos_, ref slotGroup);
				BaseDLL.decode_int8(buffer, ref pos_, ref slotIndex);
			}

			public int getLen()
			{
				int _len = 0;
				// isAccept
				_len += 1;
				// slotGroup
				_len += 1;
				// slotIndex
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 请求响应房间交换位置返回
	/// </summary>
	[Protocol]
	public class WorldRoomResponseSwapSlotRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607834;
		public UInt32 Sequence;
		public UInt32 result;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->server 请求匹配战斗
	/// </summary>
	[Protocol]
	public class WorldRoomBattleStartReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607835;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 请求匹配战斗返回
	/// </summary>
	[Protocol]
	public class WorldRoomBattleStartRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607836;
		public UInt32 Sequence;
		public UInt32 result;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->server 请求取消匹配战斗
	/// </summary>
	[Protocol]
	public class WorldRoomBattleCancelReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607837;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 请求取消匹配战斗返回
	/// </summary>
	[Protocol]
	public class WorldRoomBattleCancelRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607838;
		public UInt32 Sequence;
		public UInt32 result;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->server 请求战斗准备
	/// </summary>
	[Protocol]
	public class WorldRoomBattleReadyReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607839;
		public UInt32 Sequence;
		public byte slotStatus;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, slotStatus);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref slotStatus);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, slotStatus);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref slotStatus);
			}

			public int getLen()
			{
				int _len = 0;
				// slotStatus
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 请求战斗准备返回
	/// </summary>
	[Protocol]
	public class WorldRoomBattleReadyRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607840;
		public UInt32 Sequence;
		public UInt32 result;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->server 发送邀请信息
	/// </summary>
	[Protocol]
	public class WorldRoomSendInviteLinkReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607841;
		public UInt32 Sequence;
		public UInt32 roomId;
		public byte channel;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, roomId);
				BaseDLL.encode_int8(buffer, ref pos_, channel);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref roomId);
				BaseDLL.decode_int8(buffer, ref pos_, ref channel);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, roomId);
				BaseDLL.encode_int8(buffer, ref pos_, channel);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref roomId);
				BaseDLL.decode_int8(buffer, ref pos_, ref channel);
			}

			public int getLen()
			{
				int _len = 0;
				// roomId
				_len += 4;
				// channel
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 发送邀请信息返回
	/// </summary>
	[Protocol]
	public class WorldRoomSendInviteLinkRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607842;
		public UInt32 Sequence;
		public UInt32 result;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

}
