using System;
using System.Text;

namespace Protocol
{
	/// <summary>
	///  同门关系类型
	/// </summary>
	public enum MasterSectRelationType
	{
		MSRELATION_MASTER = 1,
		/// <summary>
		/// 师傅
		/// </summary>
		MSRELATION_BROTHER = 2,
		/// <summary>
		/// 师兄弟
		/// </summary>
		MSRELATION_DISCIPLE = 3,
		/// <summary>
		/// 徒弟
		/// </summary>
		MSRELATION_MAX = 4,
	}

	public enum RelationFindType
	{
		/// <summary>
		/// 推荐好友
		/// </summary>
		Friend = 1,
		/// <summary>
		/// 推荐组队
		/// </summary>
		Team = 2,
		/// <summary>
		/// 推荐师傅
		/// </summary>
		Master = 3,
		/// <summary>
		/// 推荐徒弟
		/// </summary>
		Disciple = 4,
		/// <summary>
		/// 推荐房间玩家
		/// </summary>
		Room = 5,
	}

	/// <summary>
	/// 查询玩家类别
	/// </summary>
	public enum QueryPlayerType
	{
		/// <summary>
		///  本服
		/// </summary>
		QPT_CUR_SERVER = 0,
		/// <summary>
		///  团本
		/// </summary>
		QPT_TEAM_COPY = 1,
	}

	public enum RecvDiscipleState
	{
		Recv = 1,
		/// <summary>
		///  接受
		/// </summary>
		UnRecv = 2,
	}

	public enum ActiveTimeType
	{
		/// <summary>
		/// 24小时活跃
		/// </summary>
		AllDay = 1,
		/// <summary>
		/// 白天
		/// </summary>
		Day = 2,
		/// <summary>
		/// 晚上
		/// </summary>
		Night = 3,
	}

	public enum MasterType
	{
		/// <summary>
		/// 实力强悍型
		/// </summary>
		StrengthValiant = 1,
		/// <summary>
		/// 认真负责型
		/// </summary>
		Responsible = 2,
		/// <summary>
		/// 聊天社交型
		/// </summary>
		ChitChat = 3,
	}

	public enum RelationAnnounceType
	{
		/// <summary>
		/// 师傅
		/// </summary>
		Master = 1,
		/// <summary>
		/// 徒弟
		/// </summary>
		Disciple = 2,
	}

	/// <summary>
	/// 同步关系数据
	/// </summary>
	/// <summary>
	/// 格式 type(UInt8) + id(ObjID_t) + data
	/// </summary>
	[Protocol]
	public class WorldSyncRelationData : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601707;
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
	/// 上线同步关系列表
	/// </summary>
	/// <summary>
	/// datalist格式: type(UInt8) + ObjID_t + isOnline(UInt8) + data + .. + 0(ObjID_t)
	/// </summary>
	[Protocol]
	public class WorldSyncRelationList : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601708;
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
	/// 新关系同步
	/// </summary>
	[Protocol]
	public class WorldNotifyNewRelation : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601705;
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
	/// 删除关系同步
	/// </summary>
	[Protocol]
	public class WorldNotifyDelRelation : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601706;
		public UInt32 Sequence;
		public byte type;
		public UInt64 id;

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
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// id
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 查询推荐关系列表
	/// </summary>
	[Protocol]
	public class WorldRelationFindPlayersReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601709;
		public UInt32 Sequence;
		public byte type;
		/// <summary>
		///  类型(RelationFindType)
		/// </summary>
		public string name;

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
				BaseDLL.encode_int8(buffer, ref pos_, type);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  查找的名字
	/// </summary>
	/// <summary>
	/// 推荐关系结构
	/// </summary>
	public class QuickFriendInfo : Protocol.IProtocolStream
	{
		/// <summary>
		/// 玩家id
		/// </summary>
		public UInt64 playerId;
		/// <summary>
		/// 姓名
		/// </summary>
		public string name;
		/// <summary>
		/// 职业
		/// </summary>
		public byte occu;
		/// <summary>
		/// 性别
		/// </summary>
		public UInt32 seasonLv;
		/// <summary>
		/// 等级
		/// </summary>
		public UInt16 level;
		/// <summary>
		/// vip等级
		/// </summary>
		public byte vipLv;
		/// <summary>
		/// 师傅公告
		/// </summary>
		public string masterNote;
		/// <summary>
		/// 外观信息
		/// </summary>
		public PlayerAvatar avatar = new PlayerAvatar();
		/// <summary>
		/// 在线时间类型
		/// </summary>
		public byte activeTimeType;
		/// <summary>
		/// 师傅类型
		/// </summary>
		public byte masterType;
		/// <summary>
		/// 地区id
		/// </summary>
		public byte regionId;
		/// <summary>
		/// 宣言
		/// </summary>
		public string declaration;
		/// <summary>
		/// 玩家标签信息
		/// </summary>
		public PlayerLabelInfo playerLabelInfo = new PlayerLabelInfo();

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonLv);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, vipLv);
				byte[] masterNoteBytes = StringHelper.StringToUTF8Bytes(masterNote);
				BaseDLL.encode_string(buffer, ref pos_, masterNoteBytes, (UInt16)(buffer.Length - pos_));
				avatar.encode(buffer, ref pos_);
				BaseDLL.encode_int8(buffer, ref pos_, activeTimeType);
				BaseDLL.encode_int8(buffer, ref pos_, masterType);
				BaseDLL.encode_int8(buffer, ref pos_, regionId);
				byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
				BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
				playerLabelInfo.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLv);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_int8(buffer, ref pos_, ref vipLv);
				UInt16 masterNoteLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref masterNoteLen);
				byte[] masterNoteBytes = new byte[masterNoteLen];
				for(int i = 0; i < masterNoteLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref masterNoteBytes[i]);
				}
				masterNote = StringHelper.BytesToString(masterNoteBytes);
				avatar.decode(buffer, ref pos_);
				BaseDLL.decode_int8(buffer, ref pos_, ref activeTimeType);
				BaseDLL.decode_int8(buffer, ref pos_, ref masterType);
				BaseDLL.decode_int8(buffer, ref pos_, ref regionId);
				UInt16 declarationLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref declarationLen);
				byte[] declarationBytes = new byte[declarationLen];
				for(int i = 0; i < declarationLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref declarationBytes[i]);
				}
				declaration = StringHelper.BytesToString(declarationBytes);
				playerLabelInfo.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonLv);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, vipLv);
				byte[] masterNoteBytes = StringHelper.StringToUTF8Bytes(masterNote);
				BaseDLL.encode_string(buffer, ref pos_, masterNoteBytes, (UInt16)(buffer.Length - pos_));
				avatar.encode(buffer, ref pos_);
				BaseDLL.encode_int8(buffer, ref pos_, activeTimeType);
				BaseDLL.encode_int8(buffer, ref pos_, masterType);
				BaseDLL.encode_int8(buffer, ref pos_, regionId);
				byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
				BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
				playerLabelInfo.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLv);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_int8(buffer, ref pos_, ref vipLv);
				UInt16 masterNoteLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref masterNoteLen);
				byte[] masterNoteBytes = new byte[masterNoteLen];
				for(int i = 0; i < masterNoteLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref masterNoteBytes[i]);
				}
				masterNote = StringHelper.BytesToString(masterNoteBytes);
				avatar.decode(buffer, ref pos_);
				BaseDLL.decode_int8(buffer, ref pos_, ref activeTimeType);
				BaseDLL.decode_int8(buffer, ref pos_, ref masterType);
				BaseDLL.decode_int8(buffer, ref pos_, ref regionId);
				UInt16 declarationLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref declarationLen);
				byte[] declarationBytes = new byte[declarationLen];
				for(int i = 0; i < declarationLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref declarationBytes[i]);
				}
				declaration = StringHelper.BytesToString(declarationBytes);
				playerLabelInfo.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// playerId
				_len += 8;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// occu
				_len += 1;
				// seasonLv
				_len += 4;
				// level
				_len += 2;
				// vipLv
				_len += 1;
				// masterNote
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(masterNote);
					_len += 2 + _strBytes.Length;
				}
				// avatar
				_len += avatar.getLen();
				// activeTimeType
				_len += 1;
				// masterType
				_len += 1;
				// regionId
				_len += 1;
				// declaration
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(declaration);
					_len += 2 + _strBytes.Length;
				}
				// playerLabelInfo
				_len += playerLabelInfo.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 查询推荐关系列表
	/// </summary>
	[Protocol]
	public class WorldRelationFindPlayersRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601710;
		public UInt32 Sequence;
		public byte type;
		public QuickFriendInfo[] friendList = new QuickFriendInfo[0];

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
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)friendList.Length);
				for(int i = 0; i < friendList.Length; i++)
				{
					friendList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				UInt16 friendListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref friendListCnt);
				friendList = new QuickFriendInfo[friendListCnt];
				for(int i = 0; i < friendList.Length; i++)
				{
					friendList[i] = new QuickFriendInfo();
					friendList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)friendList.Length);
				for(int i = 0; i < friendList.Length; i++)
				{
					friendList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				UInt16 friendListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref friendListCnt);
				friendList = new QuickFriendInfo[friendListCnt];
				for(int i = 0; i < friendList.Length; i++)
				{
					friendList[i] = new QuickFriendInfo();
					friendList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// friendList
				_len += 2;
				for(int j = 0; j < friendList.Length; j++)
				{
					_len += friendList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 删除关系
	/// </summary>
	[Protocol]
	public class WorldRemoveRelation : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601704;
		public UInt32 Sequence;
		public byte type;
		public UInt64 uid;

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
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// uid
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  查询玩家信息（可根据角色ID和名字查询，优先使用角色ID）
	/// </summary>
	[Protocol]
	public class WorldQueryPlayerReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601701;
		public UInt32 Sequence;
		/// <summary>
		///  角色ID
		/// </summary>
		public UInt64 roleId;
		/// <summary>
		///  查询类型(QueryPlayerType)
		/// </summary>
		public UInt32 queryType;
		/// <summary>
		/// zone(跨服查询需填)
		/// </summary>
		public UInt32 zoneId;
		/// <summary>
		///  名字
		/// </summary>
		public string name;

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
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint32(buffer, ref pos_, queryType);
				BaseDLL.encode_uint32(buffer, ref pos_, zoneId);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref queryType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref zoneId);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint32(buffer, ref pos_, queryType);
				BaseDLL.encode_uint32(buffer, ref pos_, zoneId);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref queryType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref zoneId);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// roleId
				_len += 8;
				// queryType
				_len += 4;
				// zoneId
				_len += 4;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  查询玩家详细信息（可根据角色ID和名字查询，优先使用角色ID）
	/// </summary>
	[Protocol]
	public class WorldQueryPlayerDetailsReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601726;
		public UInt32 Sequence;
		/// <summary>
		///  角色ID
		/// </summary>
		public UInt64 roleId;
		/// <summary>
		///  查询类型(QueryPlayerType)
		/// </summary>
		public UInt32 queryType;
		/// <summary>
		/// zone(跨服查询需填)
		/// </summary>
		public UInt32 zoneId;
		/// <summary>
		///  名字
		/// </summary>
		public string name;

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
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint32(buffer, ref pos_, queryType);
				BaseDLL.encode_uint32(buffer, ref pos_, zoneId);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref queryType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref zoneId);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint32(buffer, ref pos_, queryType);
				BaseDLL.encode_uint32(buffer, ref pos_, zoneId);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref queryType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref zoneId);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// roleId
				_len += 8;
				// queryType
				_len += 4;
				// zoneId
				_len += 4;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  物品基本信息
	/// </summary>
	public class ItemBaseInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  唯一ID
		/// </summary>
		public UInt64 id;
		/// <summary>
		///  类型ID
		/// </summary>
		public UInt32 typeId;
		/// <summary>
		///  位置
		/// </summary>
		public UInt32 pos;
		/// <summary>
		///  强化
		/// </summary>
		public byte strengthen;
		/// <summary>
		///  装备类型
		/// </summary>
		public byte equipType;
		/// <summary>
		/// 增幅路线
		/// </summary>
		public byte enhanceType;
		/// <summary>
		///  装备评分
		/// </summary>
		public UInt32 equipScore;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, typeId);
				BaseDLL.encode_uint32(buffer, ref pos_, pos);
				BaseDLL.encode_int8(buffer, ref pos_, strengthen);
				BaseDLL.encode_int8(buffer, ref pos_, equipType);
				BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
				BaseDLL.encode_uint32(buffer, ref pos_, equipScore);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref typeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref pos);
				BaseDLL.decode_int8(buffer, ref pos_, ref strengthen);
				BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
				BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref equipScore);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, typeId);
				BaseDLL.encode_uint32(buffer, ref pos_, pos);
				BaseDLL.encode_int8(buffer, ref pos_, strengthen);
				BaseDLL.encode_int8(buffer, ref pos_, equipType);
				BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
				BaseDLL.encode_uint32(buffer, ref pos_, equipScore);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref typeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref pos);
				BaseDLL.decode_int8(buffer, ref pos_, ref strengthen);
				BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
				BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref equipScore);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// typeId
				_len += 4;
				// pos
				_len += 4;
				// strengthen
				_len += 1;
				// equipType
				_len += 1;
				// enhanceType
				_len += 1;
				// equipScore
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  Pk信息
	/// </summary>
	public class PkStatisticInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  胜场数
		/// </summary>
		public UInt32 totalWinNum;
		/// <summary>
		///  负场数
		/// </summary>
		public UInt32 totalLoseNum;
		/// <summary>
		///  总场数
		/// </summary>
		public UInt32 totalNum;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, totalWinNum);
				BaseDLL.encode_uint32(buffer, ref pos_, totalLoseNum);
				BaseDLL.encode_uint32(buffer, ref pos_, totalNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalWinNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalLoseNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, totalWinNum);
				BaseDLL.encode_uint32(buffer, ref pos_, totalLoseNum);
				BaseDLL.encode_uint32(buffer, ref pos_, totalNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalWinNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalLoseNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalNum);
			}

			public int getLen()
			{
				int _len = 0;
				// totalWinNum
				_len += 4;
				// totalLoseNum
				_len += 4;
				// totalNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  公会称号
	/// </summary>
	public class GuildTitle : Protocol.IProtocolStream
	{
		/// <summary>
		///  公会名
		/// </summary>
		public string name;
		/// <summary>
		///  职务
		/// </summary>
		public byte post;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, post);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref post);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, post);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref post);
			}

			public int getLen()
			{
				int _len = 0;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// post
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  查看玩家的信息
	/// </summary>
	public class PlayerWatchInfo : Protocol.IProtocolStream
	{
		public UInt64 id;
		public string name;
		public byte occu;
		public UInt16 level;
		public ItemBaseInfo[] equips = new ItemBaseInfo[0];
		public ItemBaseInfo[] fashionEquips = new ItemBaseInfo[0];
		public RetinueInfo retinue = new RetinueInfo();
		public PkStatisticInfo pkInfo = new PkStatisticInfo();
		public UInt32 pkValue;
		public UInt32 matchScore;
		/// <summary>
		///  vip等级
		/// </summary>
		public byte vipLevel;
		/// <summary>
		///  公会称号
		/// </summary>
		public GuildTitle guildTitle = new GuildTitle();
		/// <summary>
		///  赛季段位等级
		/// </summary>
		public UInt32 seasonLevel;
		/// <summary>
		///  赛季段位星级
		/// </summary>
		public UInt32 seasonStar;
		/// <summary>
		///  宠物
		/// </summary>
		public PetBaseInfo[] pets = new PetBaseInfo[0];
		/// <summary>
		///  外观
		/// </summary>
		public PlayerAvatar avatar = new PlayerAvatar();
		/// <summary>
		///  在线时间类型
		/// </summary>
		public byte activeTimeType;
		/// <summary>
		///  师傅类型
		/// </summary>
		public byte masterType;
		/// <summary>
		///  地区id
		/// </summary>
		public byte regionId;
		/// <summary>
		///  宣言
		/// </summary>
		public string declaration;
		/// <summary>
		/// 玩家标签信息
		/// </summary>
		public PlayerLabelInfo playerLabelInfo = new PlayerLabelInfo();
		/// <summary>
		/// 冒险队名
		/// </summary>
		public string adventureTeamName;
		/// <summary>
		/// 冒险队评级
		/// </summary>
		public string adventureTeamGrade;
		/// <summary>
		/// 冒险队排名
		/// </summary>
		public UInt32 adventureTeamRanking;
		/// <summary>
		/// 徽记
		/// </summary>
		public UInt32 emblemLevel;
		/// <summary>
		///  全身装备评分
		/// </summary>
		public UInt32 totalEquipScore;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)equips.Length);
				for(int i = 0; i < equips.Length; i++)
				{
					equips[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)fashionEquips.Length);
				for(int i = 0; i < fashionEquips.Length; i++)
				{
					fashionEquips[i].encode(buffer, ref pos_);
				}
				retinue.encode(buffer, ref pos_);
				pkInfo.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, pkValue);
				BaseDLL.encode_uint32(buffer, ref pos_, matchScore);
				BaseDLL.encode_int8(buffer, ref pos_, vipLevel);
				guildTitle.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonStar);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)pets.Length);
				for(int i = 0; i < pets.Length; i++)
				{
					pets[i].encode(buffer, ref pos_);
				}
				avatar.encode(buffer, ref pos_);
				BaseDLL.encode_int8(buffer, ref pos_, activeTimeType);
				BaseDLL.encode_int8(buffer, ref pos_, masterType);
				BaseDLL.encode_int8(buffer, ref pos_, regionId);
				byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
				BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
				playerLabelInfo.encode(buffer, ref pos_);
				byte[] adventureTeamNameBytes = StringHelper.StringToUTF8Bytes(adventureTeamName);
				BaseDLL.encode_string(buffer, ref pos_, adventureTeamNameBytes, (UInt16)(buffer.Length - pos_));
				byte[] adventureTeamGradeBytes = StringHelper.StringToUTF8Bytes(adventureTeamGrade);
				BaseDLL.encode_string(buffer, ref pos_, adventureTeamGradeBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, adventureTeamRanking);
				BaseDLL.encode_uint32(buffer, ref pos_, emblemLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, totalEquipScore);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				UInt16 equipsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref equipsCnt);
				equips = new ItemBaseInfo[equipsCnt];
				for(int i = 0; i < equips.Length; i++)
				{
					equips[i] = new ItemBaseInfo();
					equips[i].decode(buffer, ref pos_);
				}
				UInt16 fashionEquipsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref fashionEquipsCnt);
				fashionEquips = new ItemBaseInfo[fashionEquipsCnt];
				for(int i = 0; i < fashionEquips.Length; i++)
				{
					fashionEquips[i] = new ItemBaseInfo();
					fashionEquips[i].decode(buffer, ref pos_);
				}
				retinue.decode(buffer, ref pos_);
				pkInfo.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref pkValue);
				BaseDLL.decode_uint32(buffer, ref pos_, ref matchScore);
				BaseDLL.decode_int8(buffer, ref pos_, ref vipLevel);
				guildTitle.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonStar);
				UInt16 petsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref petsCnt);
				pets = new PetBaseInfo[petsCnt];
				for(int i = 0; i < pets.Length; i++)
				{
					pets[i] = new PetBaseInfo();
					pets[i].decode(buffer, ref pos_);
				}
				avatar.decode(buffer, ref pos_);
				BaseDLL.decode_int8(buffer, ref pos_, ref activeTimeType);
				BaseDLL.decode_int8(buffer, ref pos_, ref masterType);
				BaseDLL.decode_int8(buffer, ref pos_, ref regionId);
				UInt16 declarationLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref declarationLen);
				byte[] declarationBytes = new byte[declarationLen];
				for(int i = 0; i < declarationLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref declarationBytes[i]);
				}
				declaration = StringHelper.BytesToString(declarationBytes);
				playerLabelInfo.decode(buffer, ref pos_);
				UInt16 adventureTeamNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref adventureTeamNameLen);
				byte[] adventureTeamNameBytes = new byte[adventureTeamNameLen];
				for(int i = 0; i < adventureTeamNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref adventureTeamNameBytes[i]);
				}
				adventureTeamName = StringHelper.BytesToString(adventureTeamNameBytes);
				UInt16 adventureTeamGradeLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref adventureTeamGradeLen);
				byte[] adventureTeamGradeBytes = new byte[adventureTeamGradeLen];
				for(int i = 0; i < adventureTeamGradeLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref adventureTeamGradeBytes[i]);
				}
				adventureTeamGrade = StringHelper.BytesToString(adventureTeamGradeBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref adventureTeamRanking);
				BaseDLL.decode_uint32(buffer, ref pos_, ref emblemLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalEquipScore);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)equips.Length);
				for(int i = 0; i < equips.Length; i++)
				{
					equips[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)fashionEquips.Length);
				for(int i = 0; i < fashionEquips.Length; i++)
				{
					fashionEquips[i].encode(buffer, ref pos_);
				}
				retinue.encode(buffer, ref pos_);
				pkInfo.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, pkValue);
				BaseDLL.encode_uint32(buffer, ref pos_, matchScore);
				BaseDLL.encode_int8(buffer, ref pos_, vipLevel);
				guildTitle.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonStar);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)pets.Length);
				for(int i = 0; i < pets.Length; i++)
				{
					pets[i].encode(buffer, ref pos_);
				}
				avatar.encode(buffer, ref pos_);
				BaseDLL.encode_int8(buffer, ref pos_, activeTimeType);
				BaseDLL.encode_int8(buffer, ref pos_, masterType);
				BaseDLL.encode_int8(buffer, ref pos_, regionId);
				byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
				BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
				playerLabelInfo.encode(buffer, ref pos_);
				byte[] adventureTeamNameBytes = StringHelper.StringToUTF8Bytes(adventureTeamName);
				BaseDLL.encode_string(buffer, ref pos_, adventureTeamNameBytes, (UInt16)(buffer.Length - pos_));
				byte[] adventureTeamGradeBytes = StringHelper.StringToUTF8Bytes(adventureTeamGrade);
				BaseDLL.encode_string(buffer, ref pos_, adventureTeamGradeBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, adventureTeamRanking);
				BaseDLL.encode_uint32(buffer, ref pos_, emblemLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, totalEquipScore);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				UInt16 equipsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref equipsCnt);
				equips = new ItemBaseInfo[equipsCnt];
				for(int i = 0; i < equips.Length; i++)
				{
					equips[i] = new ItemBaseInfo();
					equips[i].decode(buffer, ref pos_);
				}
				UInt16 fashionEquipsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref fashionEquipsCnt);
				fashionEquips = new ItemBaseInfo[fashionEquipsCnt];
				for(int i = 0; i < fashionEquips.Length; i++)
				{
					fashionEquips[i] = new ItemBaseInfo();
					fashionEquips[i].decode(buffer, ref pos_);
				}
				retinue.decode(buffer, ref pos_);
				pkInfo.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref pkValue);
				BaseDLL.decode_uint32(buffer, ref pos_, ref matchScore);
				BaseDLL.decode_int8(buffer, ref pos_, ref vipLevel);
				guildTitle.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonStar);
				UInt16 petsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref petsCnt);
				pets = new PetBaseInfo[petsCnt];
				for(int i = 0; i < pets.Length; i++)
				{
					pets[i] = new PetBaseInfo();
					pets[i].decode(buffer, ref pos_);
				}
				avatar.decode(buffer, ref pos_);
				BaseDLL.decode_int8(buffer, ref pos_, ref activeTimeType);
				BaseDLL.decode_int8(buffer, ref pos_, ref masterType);
				BaseDLL.decode_int8(buffer, ref pos_, ref regionId);
				UInt16 declarationLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref declarationLen);
				byte[] declarationBytes = new byte[declarationLen];
				for(int i = 0; i < declarationLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref declarationBytes[i]);
				}
				declaration = StringHelper.BytesToString(declarationBytes);
				playerLabelInfo.decode(buffer, ref pos_);
				UInt16 adventureTeamNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref adventureTeamNameLen);
				byte[] adventureTeamNameBytes = new byte[adventureTeamNameLen];
				for(int i = 0; i < adventureTeamNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref adventureTeamNameBytes[i]);
				}
				adventureTeamName = StringHelper.BytesToString(adventureTeamNameBytes);
				UInt16 adventureTeamGradeLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref adventureTeamGradeLen);
				byte[] adventureTeamGradeBytes = new byte[adventureTeamGradeLen];
				for(int i = 0; i < adventureTeamGradeLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref adventureTeamGradeBytes[i]);
				}
				adventureTeamGrade = StringHelper.BytesToString(adventureTeamGradeBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref adventureTeamRanking);
				BaseDLL.decode_uint32(buffer, ref pos_, ref emblemLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalEquipScore);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// occu
				_len += 1;
				// level
				_len += 2;
				// equips
				_len += 2;
				for(int j = 0; j < equips.Length; j++)
				{
					_len += equips[j].getLen();
				}
				// fashionEquips
				_len += 2;
				for(int j = 0; j < fashionEquips.Length; j++)
				{
					_len += fashionEquips[j].getLen();
				}
				// retinue
				_len += retinue.getLen();
				// pkInfo
				_len += pkInfo.getLen();
				// pkValue
				_len += 4;
				// matchScore
				_len += 4;
				// vipLevel
				_len += 1;
				// guildTitle
				_len += guildTitle.getLen();
				// seasonLevel
				_len += 4;
				// seasonStar
				_len += 4;
				// pets
				_len += 2;
				for(int j = 0; j < pets.Length; j++)
				{
					_len += pets[j].getLen();
				}
				// avatar
				_len += avatar.getLen();
				// activeTimeType
				_len += 1;
				// masterType
				_len += 1;
				// regionId
				_len += 1;
				// declaration
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(declaration);
					_len += 2 + _strBytes.Length;
				}
				// playerLabelInfo
				_len += playerLabelInfo.getLen();
				// adventureTeamName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(adventureTeamName);
					_len += 2 + _strBytes.Length;
				}
				// adventureTeamGrade
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(adventureTeamGrade);
					_len += 2 + _strBytes.Length;
				}
				// adventureTeamRanking
				_len += 4;
				// emblemLevel
				_len += 4;
				// totalEquipScore
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  返回玩家信息
	/// </summary>
	[Protocol]
	public class WorldQueryPlayerRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601702;
		public UInt32 Sequence;
		/// <summary>
		///  查询类型(QueryPlayerType)
		/// </summary>
		public UInt32 queryType;
		/// <summary>
		/// zone(跨服查询需填)
		/// </summary>
		public UInt32 zoneId;
		public PlayerWatchInfo info = new PlayerWatchInfo();

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
				BaseDLL.encode_uint32(buffer, ref pos_, queryType);
				BaseDLL.encode_uint32(buffer, ref pos_, zoneId);
				info.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref queryType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref zoneId);
				info.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, queryType);
				BaseDLL.encode_uint32(buffer, ref pos_, zoneId);
				info.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref queryType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref zoneId);
				info.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// queryType
				_len += 4;
				// zoneId
				_len += 4;
				// info
				_len += info.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  返回玩家信息
	/// </summary>
	[Protocol]
	public class WorldQueryPlayerDetailsRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601727;
		public UInt32 Sequence;
		public RacePlayerInfo info = new RacePlayerInfo();

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
	/// 好友赠送
	/// </summary>
	[Protocol]
	public class WorldRelationPresentGiveReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601711;
		public UInt32 Sequence;
		public UInt64 friendUID;

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
				BaseDLL.encode_uint64(buffer, ref pos_, friendUID);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref friendUID);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, friendUID);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref friendUID);
			}

			public int getLen()
			{
				int _len = 0;
				// friendUID
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 同步上下线状态
	/// </summary>
	[Protocol]
	public class WorldSyncOnOffline : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601713;
		public UInt32 Sequence;
		public UInt64 id;
		public byte isOnline;

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
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, isOnline);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref isOnline);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, isOnline);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref isOnline);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// isOnline
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 更新关系
	/// </summary>
	[Protocol]
	public class WorldUpdateRelation : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601712;
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
	/// 加黑名单
	/// </summary>
	[Protocol]
	public class WorldAddToBlackList : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601703;
		public UInt32 Sequence;
		public UInt64 tarUid;

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
				BaseDLL.encode_uint64(buffer, ref pos_, tarUid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref tarUid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, tarUid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref tarUid);
			}

			public int getLen()
			{
				int _len = 0;
				// tarUid
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  玩家在线状态
	/// </summary>
	public class PlayerOnline : Protocol.IProtocolStream
	{
		public UInt64 uid;
		public byte online;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_int8(buffer, ref pos_, online);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_int8(buffer, ref pos_, ref online);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_int8(buffer, ref pos_, online);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_int8(buffer, ref pos_, ref online);
			}

			public int getLen()
			{
				int _len = 0;
				// uid
				_len += 8;
				// online
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// clt->svr更新聊天玩家的在线信息
	/// </summary>
	[Protocol]
	public class WorldUpdatePlayerOnlineReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601714;
		public UInt32 Sequence;
		public UInt64[] uids = new UInt64[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)uids.Length);
				for(int i = 0; i < uids.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, uids[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 uidsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref uidsCnt);
				uids = new UInt64[uidsCnt];
				for(int i = 0; i < uids.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref uids[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)uids.Length);
				for(int i = 0; i < uids.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, uids[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 uidsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref uidsCnt);
				uids = new UInt64[uidsCnt];
				for(int i = 0; i < uids.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref uids[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// uids
				_len += 2 + 8 * uids.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// svr->clt更新聊天玩家的在线信息
	/// </summary>
	[Protocol]
	public class WorldUpdatePlayerOnlineRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601715;
		public UInt32 Sequence;
		public PlayerOnline[] playerStates = new PlayerOnline[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)playerStates.Length);
				for(int i = 0; i < playerStates.Length; i++)
				{
					playerStates[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 playerStatesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerStatesCnt);
				playerStates = new PlayerOnline[playerStatesCnt];
				for(int i = 0; i < playerStates.Length; i++)
				{
					playerStates[i] = new PlayerOnline();
					playerStates[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)playerStates.Length);
				for(int i = 0; i < playerStates.Length; i++)
				{
					playerStates[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 playerStatesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerStatesCnt);
				playerStates = new PlayerOnline[playerStatesCnt];
				for(int i = 0; i < playerStates.Length; i++)
				{
					playerStates[i] = new PlayerOnline();
					playerStates[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// playerStates
				_len += 2;
				for(int j = 0; j < playerStates.Length; j++)
				{
					_len += playerStates[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 设置玩家师傅公告
	/// </summary>
	[Protocol]
	public class SetMasterNote : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601716;
		public UInt32 Sequence;
		public string note;

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
				byte[] noteBytes = StringHelper.StringToUTF8Bytes(note);
				BaseDLL.encode_string(buffer, ref pos_, noteBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 noteLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref noteLen);
				byte[] noteBytes = new byte[noteLen];
				for(int i = 0; i < noteLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref noteBytes[i]);
				}
				note = StringHelper.BytesToString(noteBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] noteBytes = StringHelper.StringToUTF8Bytes(note);
				BaseDLL.encode_string(buffer, ref pos_, noteBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 noteLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref noteLen);
				byte[] noteBytes = new byte[noteLen];
				for(int i = 0; i < noteLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref noteBytes[i]);
				}
				note = StringHelper.BytesToString(noteBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// note
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(note);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 设置玩家师傅公告返回
	/// </summary>
	[Protocol]
	public class SetMasterNoteRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601717;
		public UInt32 Sequence;
		public UInt32 code;
		public string note;

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
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				byte[] noteBytes = StringHelper.StringToUTF8Bytes(note);
				BaseDLL.encode_string(buffer, ref pos_, noteBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				UInt16 noteLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref noteLen);
				byte[] noteBytes = new byte[noteLen];
				for(int i = 0; i < noteLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref noteBytes[i]);
				}
				note = StringHelper.BytesToString(noteBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				byte[] noteBytes = StringHelper.StringToUTF8Bytes(note);
				BaseDLL.encode_string(buffer, ref pos_, noteBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				UInt16 noteLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref noteLen);
				byte[] noteBytes = new byte[noteLen];
				for(int i = 0; i < noteLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref noteBytes[i]);
				}
				note = StringHelper.BytesToString(noteBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// note
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(note);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  不接受
	/// </summary>
	/// <summary>
	/// 设置玩家收徒状态
	/// </summary>
	[Protocol]
	public class SetRecvDiscipleState : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601718;
		public UInt32 Sequence;
		public byte state;

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
				BaseDLL.encode_int8(buffer, ref pos_, state);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref state);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, state);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref state);
			}

			public int getLen()
			{
				int _len = 0;
				// state
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// RecvDiscipleState
	/// </summary>
	/// <summary>
	/// 设置玩家收徒状态返回
	/// </summary>
	[Protocol]
	public class SetRecvDiscipleStateRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601719;
		public UInt32 Sequence;
		public UInt32 code;
		public byte state;

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
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_int8(buffer, ref pos_, state);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_int8(buffer, ref pos_, ref state);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_int8(buffer, ref pos_, state);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_int8(buffer, ref pos_, ref state);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// state
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 请求师傅设置信息
	/// </summary>
	[Protocol]
	public class QueryMasterSettingReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601720;
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
	/// 师傅设置信息返回
	/// </summary>
	[Protocol]
	public class QueryMasterSettingRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601721;
		public UInt32 Sequence;
		public string masternote;
		/// <summary>
		/// 师傅公告
		/// </summary>
		public byte isRecv;

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
				byte[] masternoteBytes = StringHelper.StringToUTF8Bytes(masternote);
				BaseDLL.encode_string(buffer, ref pos_, masternoteBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, isRecv);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 masternoteLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref masternoteLen);
				byte[] masternoteBytes = new byte[masternoteLen];
				for(int i = 0; i < masternoteLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref masternoteBytes[i]);
				}
				masternote = StringHelper.BytesToString(masternoteBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref isRecv);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] masternoteBytes = StringHelper.StringToUTF8Bytes(masternote);
				BaseDLL.encode_string(buffer, ref pos_, masternoteBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, isRecv);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 masternoteLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref masternoteLen);
				byte[] masternoteBytes = new byte[masternoteLen];
				for(int i = 0; i < masternoteLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref masternoteBytes[i]);
				}
				masternote = StringHelper.BytesToString(masternoteBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref isRecv);
			}

			public int getLen()
			{
				int _len = 0;
				// masternote
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(masternote);
					_len += 2 + _strBytes.Length;
				}
				// isRecv
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 是否收徒
	/// </summary>
	/// <summary>
	/// 师傅赠送装备
	/// </summary>
	[Protocol]
	public class MasterGiveEquip : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501701;
		public UInt32 Sequence;
		public UInt64[] itemUids = new UInt64[0];
		/// <summary>
		/// 装备uid
		/// </summary>
		public UInt64 discipleId;

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemUids.Length);
				for(int i = 0; i < itemUids.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, itemUids[i]);
				}
				BaseDLL.encode_uint64(buffer, ref pos_, discipleId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 itemUidsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemUidsCnt);
				itemUids = new UInt64[itemUidsCnt];
				for(int i = 0; i < itemUids.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref itemUids[i]);
				}
				BaseDLL.decode_uint64(buffer, ref pos_, ref discipleId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemUids.Length);
				for(int i = 0; i < itemUids.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, itemUids[i]);
				}
				BaseDLL.encode_uint64(buffer, ref pos_, discipleId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 itemUidsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemUidsCnt);
				itemUids = new UInt64[itemUidsCnt];
				for(int i = 0; i < itemUids.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref itemUids[i]);
				}
				BaseDLL.decode_uint64(buffer, ref pos_, ref discipleId);
			}

			public int getLen()
			{
				int _len = 0;
				// itemUids
				_len += 2 + 8 * itemUids.Length;
				// discipleId
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 徒弟uid
	/// </summary>
	/// <summary>
	/// 请求代付
	/// </summary>
	[Protocol]
	public class AddonPayReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601722;
		public UInt32 Sequence;
		public UInt32 goodId;
		public UInt64 tarId;
		public string tarName;
		public byte tarOccu;
		public UInt32 tarLevel;
		public string words;

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
				BaseDLL.encode_uint32(buffer, ref pos_, goodId);
				BaseDLL.encode_uint64(buffer, ref pos_, tarId);
				byte[] tarNameBytes = StringHelper.StringToUTF8Bytes(tarName);
				BaseDLL.encode_string(buffer, ref pos_, tarNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, tarOccu);
				BaseDLL.encode_uint32(buffer, ref pos_, tarLevel);
				byte[] wordsBytes = StringHelper.StringToUTF8Bytes(words);
				BaseDLL.encode_string(buffer, ref pos_, wordsBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref goodId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref tarId);
				UInt16 tarNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref tarNameLen);
				byte[] tarNameBytes = new byte[tarNameLen];
				for(int i = 0; i < tarNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref tarNameBytes[i]);
				}
				tarName = StringHelper.BytesToString(tarNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref tarOccu);
				BaseDLL.decode_uint32(buffer, ref pos_, ref tarLevel);
				UInt16 wordsLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref wordsLen);
				byte[] wordsBytes = new byte[wordsLen];
				for(int i = 0; i < wordsLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref wordsBytes[i]);
				}
				words = StringHelper.BytesToString(wordsBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, goodId);
				BaseDLL.encode_uint64(buffer, ref pos_, tarId);
				byte[] tarNameBytes = StringHelper.StringToUTF8Bytes(tarName);
				BaseDLL.encode_string(buffer, ref pos_, tarNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, tarOccu);
				BaseDLL.encode_uint32(buffer, ref pos_, tarLevel);
				byte[] wordsBytes = StringHelper.StringToUTF8Bytes(words);
				BaseDLL.encode_string(buffer, ref pos_, wordsBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref goodId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref tarId);
				UInt16 tarNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref tarNameLen);
				byte[] tarNameBytes = new byte[tarNameLen];
				for(int i = 0; i < tarNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref tarNameBytes[i]);
				}
				tarName = StringHelper.BytesToString(tarNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref tarOccu);
				BaseDLL.decode_uint32(buffer, ref pos_, ref tarLevel);
				UInt16 wordsLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref wordsLen);
				byte[] wordsBytes = new byte[wordsLen];
				for(int i = 0; i < wordsLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref wordsBytes[i]);
				}
				words = StringHelper.BytesToString(wordsBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// goodId
				_len += 4;
				// tarId
				_len += 8;
				// tarName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(tarName);
					_len += 2 + _strBytes.Length;
				}
				// tarOccu
				_len += 1;
				// tarLevel
				_len += 4;
				// words
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(words);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 回复代付
	/// </summary>
	[Protocol]
	public class ReplyAddonPay : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601725;
		public UInt32 Sequence;
		public UInt64 id;
		public byte agree;

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
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, agree);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref agree);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, agree);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref agree);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// agree
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class AddonPayData : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501708;
		public UInt32 Sequence;
		public UInt64 id;
		public byte type;
		/// <summary>
		/// 0.自己找人付 1.别人找自己付
		/// </summary>
		public byte relationType;
		/// <summary>
		/// 社会关系
		/// </summary>
		public string name;
		public byte occu;
		public UInt32 level;
		public UInt32 overdueTime;
		public byte status;
		/// <summary>
		/// 0.未响应 1.同意 2.拒绝
		/// </summary>
		public UInt32 payItemId;
		public UInt32 payItemNum;
		public string words;

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
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, relationType);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_uint32(buffer, ref pos_, level);
				BaseDLL.encode_uint32(buffer, ref pos_, overdueTime);
				BaseDLL.encode_int8(buffer, ref pos_, status);
				BaseDLL.encode_uint32(buffer, ref pos_, payItemId);
				BaseDLL.encode_uint32(buffer, ref pos_, payItemNum);
				byte[] wordsBytes = StringHelper.StringToUTF8Bytes(words);
				BaseDLL.encode_string(buffer, ref pos_, wordsBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref relationType);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_uint32(buffer, ref pos_, ref level);
				BaseDLL.decode_uint32(buffer, ref pos_, ref overdueTime);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
				BaseDLL.decode_uint32(buffer, ref pos_, ref payItemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref payItemNum);
				UInt16 wordsLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref wordsLen);
				byte[] wordsBytes = new byte[wordsLen];
				for(int i = 0; i < wordsLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref wordsBytes[i]);
				}
				words = StringHelper.BytesToString(wordsBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, relationType);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_uint32(buffer, ref pos_, level);
				BaseDLL.encode_uint32(buffer, ref pos_, overdueTime);
				BaseDLL.encode_int8(buffer, ref pos_, status);
				BaseDLL.encode_uint32(buffer, ref pos_, payItemId);
				BaseDLL.encode_uint32(buffer, ref pos_, payItemNum);
				byte[] wordsBytes = StringHelper.StringToUTF8Bytes(words);
				BaseDLL.encode_string(buffer, ref pos_, wordsBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref relationType);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_uint32(buffer, ref pos_, ref level);
				BaseDLL.decode_uint32(buffer, ref pos_, ref overdueTime);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
				BaseDLL.decode_uint32(buffer, ref pos_, ref payItemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref payItemNum);
				UInt16 wordsLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref wordsLen);
				byte[] wordsBytes = new byte[wordsLen];
				for(int i = 0; i < wordsLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref wordsBytes[i]);
				}
				words = StringHelper.BytesToString(wordsBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// type
				_len += 1;
				// relationType
				_len += 1;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// occu
				_len += 1;
				// level
				_len += 4;
				// overdueTime
				_len += 4;
				// status
				_len += 1;
				// payItemId
				_len += 4;
				// payItemNum
				_len += 4;
				// words
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(words);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 初始化同步代付列表
	/// </summary>
	[Protocol]
	public class AddonPayList : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601724;
		public UInt32 Sequence;
		public AddonPayData[] data = new AddonPayData[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)data.Length);
				for(int i = 0; i < data.Length; i++)
				{
					data[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 dataCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref dataCnt);
				data = new AddonPayData[dataCnt];
				for(int i = 0; i < data.Length; i++)
				{
					data[i] = new AddonPayData();
					data[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)data.Length);
				for(int i = 0; i < data.Length; i++)
				{
					data[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 dataCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref dataCnt);
				data = new AddonPayData[dataCnt];
				for(int i = 0; i < data.Length; i++)
				{
					data[i] = new AddonPayData();
					data[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// data
				_len += 2;
				for(int j = 0; j < data.Length; j++)
				{
					_len += data[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 增加代付
	/// </summary>
	[Protocol]
	public class AddPayData : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601723;
		public UInt32 Sequence;
		public AddonPayData data = new AddonPayData();

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
				data.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				data.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				data.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				data.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// data
				_len += data.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 删除代付
	/// </summary>
	[Protocol]
	public class DelPayData : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501711;
		public UInt32 Sequence;
		public UInt64 id;

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
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 设置代付开关请求
	/// </summary>
	[Protocol]
	public class AddonPaySwitchReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501713;
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
	/// 设置代付开关返回
	/// </summary>
	[Protocol]
	public class SetAddonPaySwitchRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501714;
		public UInt32 Sequence;
		public byte isOn;

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
				BaseDLL.encode_int8(buffer, ref pos_, isOn);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isOn);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, isOn);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isOn);
			}

			public int getLen()
			{
				int _len = 0;
				// isOn
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 代付开关请求
	/// </summary>
	[Protocol]
	public class QueryAddonPaySwitch : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501715;
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
	/// 查询代付留言
	/// </summary>
	[Protocol]
	public class QueryAddonPayMsg : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501716;
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
	/// 查询代付留言返回
	/// </summary>
	[Protocol]
	public class QueryAddonPayMsgRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501717;
		public UInt32 Sequence;
		public string words;

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
				byte[] wordsBytes = StringHelper.StringToUTF8Bytes(words);
				BaseDLL.encode_string(buffer, ref pos_, wordsBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 wordsLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref wordsLen);
				byte[] wordsBytes = new byte[wordsLen];
				for(int i = 0; i < wordsLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref wordsBytes[i]);
				}
				words = StringHelper.BytesToString(wordsBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] wordsBytes = StringHelper.StringToUTF8Bytes(words);
				BaseDLL.encode_string(buffer, ref pos_, wordsBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 wordsLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref wordsLen);
				byte[] wordsBytes = new byte[wordsLen];
				for(int i = 0; i < wordsLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref wordsBytes[i]);
				}
				words = StringHelper.BytesToString(wordsBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// words
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(words);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  汇报自己作弊了
	/// </summary>
	[Protocol]
	public class WorldRelationReportCheat : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601736;
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
	/// client->world 设置徒弟问卷调查
	/// </summary>
	[Protocol]
	public class WorldSetDiscipleQuestionnaireReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601739;
		public UInt32 Sequence;
		/// <summary>
		/// 活跃时间类型
		/// </summary>
		public byte activeTimeType;
		/// <summary>
		/// 类型(ActiveTimeType)
		/// </summary>
		/// <summary>
		/// 希望类型师傅
		/// </summary>
		public byte masterType;
		/// <summary>
		/// 类型(MasterType)
		/// </summary>
		/// <summary>
		/// 所在地区id
		/// </summary>
		public byte regionId;
		/// <summary>
		/// 宣言
		/// </summary>
		public string declaration;

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
				BaseDLL.encode_int8(buffer, ref pos_, activeTimeType);
				BaseDLL.encode_int8(buffer, ref pos_, masterType);
				BaseDLL.encode_int8(buffer, ref pos_, regionId);
				byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
				BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref activeTimeType);
				BaseDLL.decode_int8(buffer, ref pos_, ref masterType);
				BaseDLL.decode_int8(buffer, ref pos_, ref regionId);
				UInt16 declarationLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref declarationLen);
				byte[] declarationBytes = new byte[declarationLen];
				for(int i = 0; i < declarationLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref declarationBytes[i]);
				}
				declaration = StringHelper.BytesToString(declarationBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, activeTimeType);
				BaseDLL.encode_int8(buffer, ref pos_, masterType);
				BaseDLL.encode_int8(buffer, ref pos_, regionId);
				byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
				BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref activeTimeType);
				BaseDLL.decode_int8(buffer, ref pos_, ref masterType);
				BaseDLL.decode_int8(buffer, ref pos_, ref regionId);
				UInt16 declarationLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref declarationLen);
				byte[] declarationBytes = new byte[declarationLen];
				for(int i = 0; i < declarationLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref declarationBytes[i]);
				}
				declaration = StringHelper.BytesToString(declarationBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// activeTimeType
				_len += 1;
				// masterType
				_len += 1;
				// regionId
				_len += 1;
				// declaration
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(declaration);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client 设置徒弟问卷返回
	/// </summary>
	[Protocol]
	public class WorldSetDiscipleQuestionnaireRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601740;
		public UInt32 Sequence;
		/// <summary>
		/// 返回码
		/// </summary>
		public UInt32 code;

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
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world 设置师傅问卷
	/// </summary>
	[Protocol]
	public class WorldSetMasterQuestionnaireReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601741;
		public UInt32 Sequence;
		/// <summary>
		/// 活跃时间类型
		/// </summary>
		public byte activeTimeType;
		/// <summary>
		/// 我是什么类型师傅
		/// </summary>
		public byte masterType;
		/// <summary>
		/// 所在地区id
		/// </summary>
		public byte regionId;
		/// <summary>
		/// 宣言
		/// </summary>
		public string declaration;

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
				BaseDLL.encode_int8(buffer, ref pos_, activeTimeType);
				BaseDLL.encode_int8(buffer, ref pos_, masterType);
				BaseDLL.encode_int8(buffer, ref pos_, regionId);
				byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
				BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref activeTimeType);
				BaseDLL.decode_int8(buffer, ref pos_, ref masterType);
				BaseDLL.decode_int8(buffer, ref pos_, ref regionId);
				UInt16 declarationLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref declarationLen);
				byte[] declarationBytes = new byte[declarationLen];
				for(int i = 0; i < declarationLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref declarationBytes[i]);
				}
				declaration = StringHelper.BytesToString(declarationBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, activeTimeType);
				BaseDLL.encode_int8(buffer, ref pos_, masterType);
				BaseDLL.encode_int8(buffer, ref pos_, regionId);
				byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
				BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref activeTimeType);
				BaseDLL.decode_int8(buffer, ref pos_, ref masterType);
				BaseDLL.decode_int8(buffer, ref pos_, ref regionId);
				UInt16 declarationLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref declarationLen);
				byte[] declarationBytes = new byte[declarationLen];
				for(int i = 0; i < declarationLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref declarationBytes[i]);
				}
				declaration = StringHelper.BytesToString(declarationBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// activeTimeType
				_len += 1;
				// masterType
				_len += 1;
				// regionId
				_len += 1;
				// declaration
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(declaration);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client 设置师傅问卷返回
	/// </summary>
	[Protocol]
	public class WorldSetMasterQuestionnaireRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601742;
		public UInt32 Sequence;
		/// <summary>
		/// 返回码
		/// </summary>
		public UInt32 code;

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
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client 通知新同门关系
	/// </summary>
	/// <summary>
	/// 格式 id(ObjID_t) + data
	/// </summary>
	[Protocol]
	public class WorldNotifyNewMasterSectRelation : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601743;
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
	/// world->client 同步同门关系数据
	/// </summary>
	/// <summary>
	/// 格式  id(ObjID_t) + data
	/// </summary>
	[Protocol]
	public class WorldSyncMasterSectRelationData : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601744;
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
	/// world->client 上线同步同门关系列表
	/// </summary>
	/// <summary>
	/// datalist格式: ObjID_t + isOnline(UInt8) + data + .. + 0(ObjID_t)
	/// </summary>
	[Protocol]
	public class WorldSyncMasterSectRelationList : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601745;
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
	/// client->world 更新同门关系
	/// </summary>
	[Protocol]
	public class WorldUpdateMasterSectRelationReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601746;
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
	/// world->client 删除同门关系同步
	/// </summary>
	[Protocol]
	public class WorldNotifyDelMasterSectRelation : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601747;
		public UInt32 Sequence;
		public byte type;
		public UInt64 id;

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
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// id
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client 同步问卷调查
	/// </summary>
	[Protocol]
	public class WorldSyncRelationQuestionnaire : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601748;
		public UInt32 Sequence;
		public byte activeTimeType;
		public byte masterType;
		public byte regionId;
		public string declaration;

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
				BaseDLL.encode_int8(buffer, ref pos_, activeTimeType);
				BaseDLL.encode_int8(buffer, ref pos_, masterType);
				BaseDLL.encode_int8(buffer, ref pos_, regionId);
				byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
				BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref activeTimeType);
				BaseDLL.decode_int8(buffer, ref pos_, ref masterType);
				BaseDLL.decode_int8(buffer, ref pos_, ref regionId);
				UInt16 declarationLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref declarationLen);
				byte[] declarationBytes = new byte[declarationLen];
				for(int i = 0; i < declarationLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref declarationBytes[i]);
				}
				declaration = StringHelper.BytesToString(declarationBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, activeTimeType);
				BaseDLL.encode_int8(buffer, ref pos_, masterType);
				BaseDLL.encode_int8(buffer, ref pos_, regionId);
				byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
				BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref activeTimeType);
				BaseDLL.decode_int8(buffer, ref pos_, ref masterType);
				BaseDLL.decode_int8(buffer, ref pos_, ref regionId);
				UInt16 declarationLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref declarationLen);
				byte[] declarationBytes = new byte[declarationLen];
				for(int i = 0; i < declarationLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref declarationBytes[i]);
				}
				declaration = StringHelper.BytesToString(declarationBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// activeTimeType
				_len += 1;
				// masterType
				_len += 1;
				// regionId
				_len += 1;
				// declaration
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(declaration);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world 请求徒弟出师
	/// </summary>
	[Protocol]
	public class WorldDiscipleFinishSchoolReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601749;
		public UInt32 Sequence;
		public UInt64 discipleId;

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
				BaseDLL.encode_uint64(buffer, ref pos_, discipleId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref discipleId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, discipleId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref discipleId);
			}

			public int getLen()
			{
				int _len = 0;
				// discipleId
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client 请求徒弟出师返回
	/// </summary>
	[Protocol]
	public class WorldDiscipleFinishSchoolRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601750;
		public UInt32 Sequence;
		public UInt32 code;

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
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client  同步师徒拜师收徒惩罚时间戳
	/// </summary>
	[Protocol]
	public class WorldSyncMasterDisciplePunishTime : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601751;
		public UInt32 Sequence;
		public UInt64 apprenticMasterPunishTime;
		public UInt64 recruitDisciplePunishTime;

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
				BaseDLL.encode_uint64(buffer, ref pos_, apprenticMasterPunishTime);
				BaseDLL.encode_uint64(buffer, ref pos_, recruitDisciplePunishTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref apprenticMasterPunishTime);
				BaseDLL.decode_uint64(buffer, ref pos_, ref recruitDisciplePunishTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, apprenticMasterPunishTime);
				BaseDLL.encode_uint64(buffer, ref pos_, recruitDisciplePunishTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref apprenticMasterPunishTime);
				BaseDLL.decode_uint64(buffer, ref pos_, ref recruitDisciplePunishTime);
			}

			public int getLen()
			{
				int _len = 0;
				// apprenticMasterPunishTime
				_len += 8;
				// recruitDisciplePunishTime
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world	 自动出师请求
	/// </summary>
	[Protocol]
	public class WorldAutomaticFinishSchoolReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601772;
		public UInt32 Sequence;
		public UInt64 targetId;

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
				BaseDLL.encode_uint64(buffer, ref pos_, targetId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref targetId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, targetId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref targetId);
			}

			public int getLen()
			{
				int _len = 0;
				// targetId
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 目标角色id
	/// </summary>
	/// <summary>
	/// world->client	 自动出师返回
	/// </summary>
	[Protocol]
	public class WorldAutomaticFinishSchoolRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601773;
		public UInt32 Sequence;
		public UInt32 code;

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
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world  设置玩家备注请求
	/// </summary>
	[Protocol]
	public class WorldSetPlayerRemarkReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601737;
		public UInt32 Sequence;
		public UInt64 roleId;
		public string remark;

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
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				byte[] remarkBytes = StringHelper.StringToUTF8Bytes(remark);
				BaseDLL.encode_string(buffer, ref pos_, remarkBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				UInt16 remarkLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref remarkLen);
				byte[] remarkBytes = new byte[remarkLen];
				for(int i = 0; i < remarkLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref remarkBytes[i]);
				}
				remark = StringHelper.BytesToString(remarkBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				byte[] remarkBytes = StringHelper.StringToUTF8Bytes(remark);
				BaseDLL.encode_string(buffer, ref pos_, remarkBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				UInt16 remarkLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref remarkLen);
				byte[] remarkBytes = new byte[remarkLen];
				for(int i = 0; i < remarkLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref remarkBytes[i]);
				}
				remark = StringHelper.BytesToString(remarkBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// roleId
				_len += 8;
				// remark
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(remark);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client  设置玩家备注返回
	/// </summary>
	[Protocol]
	public class WorldSetPlayerRemarkRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601738;
		public UInt32 Sequence;
		public UInt32 code;

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
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client 好友赠送返回
	/// </summary>
	[Protocol]
	public class WorldRelationPresentGiveRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601775;
		public UInt32 Sequence;
		public UInt32 code;
		public UInt64 friendID;

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
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint64(buffer, ref pos_, friendID);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint64(buffer, ref pos_, ref friendID);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint64(buffer, ref pos_, friendID);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint64(buffer, ref pos_, ref friendID);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// friendID
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world  师徒公告
	/// </summary>
	[Protocol]
	public class WorldRelationAnnounceReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601776;
		public UInt32 Sequence;
		public UInt32 type;

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
				BaseDLL.encode_uint32(buffer, ref pos_, type);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref type);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, type);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref type);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 类型(RelationAnnounceType)
	/// </summary>
	/// <summary>
	/// world->client 通知出师事件
	/// </summary>
	[Protocol]
	public class WorldNotifyFinSchEvent : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601777;
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

}
