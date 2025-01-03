using System;
using System.Text;

namespace Protocol
{
	/// <summary>
	///  远征状态
	/// </summary>
	public enum ExpeditionStatus
	{
		/// <summary>
		///  准备出发
		/// </summary>
		EXPEDITION_STATUS_PREPARE = 0,
		/// <summary>
		///  远征中
		/// </summary>
		EXPEDITION_STATUS_IN = 1,
		/// <summary>
		///  远征完毕
		/// </summary>
		EXPEDITION_STATUS_OVER = 2,
	}

	/// <summary>
	///  冒险队(佣兵团)信息
	/// </summary>
	public class AdventureTeamInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  冒险队队名
		/// </summary>
		public string adventureTeamName;
		/// <summary>
		///  冒险队等级
		/// </summary>
		public UInt16 adventureTeamLevel;
		/// <summary>
		///  冒险队经验
		/// </summary>
		public UInt64 adventureTeamExp;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] adventureTeamNameBytes = StringHelper.StringToUTF8Bytes(adventureTeamName);
				BaseDLL.encode_string(buffer, ref pos_, adventureTeamNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, adventureTeamLevel);
				BaseDLL.encode_uint64(buffer, ref pos_, adventureTeamExp);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 adventureTeamNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref adventureTeamNameLen);
				byte[] adventureTeamNameBytes = new byte[adventureTeamNameLen];
				for(int i = 0; i < adventureTeamNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref adventureTeamNameBytes[i]);
				}
				adventureTeamName = StringHelper.BytesToString(adventureTeamNameBytes);
				BaseDLL.decode_uint16(buffer, ref pos_, ref adventureTeamLevel);
				BaseDLL.decode_uint64(buffer, ref pos_, ref adventureTeamExp);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] adventureTeamNameBytes = StringHelper.StringToUTF8Bytes(adventureTeamName);
				BaseDLL.encode_string(buffer, ref pos_, adventureTeamNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, adventureTeamLevel);
				BaseDLL.encode_uint64(buffer, ref pos_, adventureTeamExp);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 adventureTeamNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref adventureTeamNameLen);
				byte[] adventureTeamNameBytes = new byte[adventureTeamNameLen];
				for(int i = 0; i < adventureTeamNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref adventureTeamNameBytes[i]);
				}
				adventureTeamName = StringHelper.BytesToString(adventureTeamNameBytes);
				BaseDLL.decode_uint16(buffer, ref pos_, ref adventureTeamLevel);
				BaseDLL.decode_uint64(buffer, ref pos_, ref adventureTeamExp);
			}

			public int getLen()
			{
				int _len = 0;
				// adventureTeamName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(adventureTeamName);
					_len += 2 + _strBytes.Length;
				}
				// adventureTeamLevel
				_len += 2;
				// adventureTeamExp
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  冒险队(佣兵团)扩展信息
	/// </summary>
	public class AdventureTeamExtraInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  冒险队队名
		/// </summary>
		public string adventureTeamName;
		/// <summary>
		///  冒险队等级
		/// </summary>
		public UInt16 adventureTeamLevel;
		/// <summary>
		///  冒险队经验
		/// </summary>
		public UInt64 adventureTeamExp;
		/// <summary>
		///  冒险队评级
		/// </summary>
		public string adventureTeamGrade;
		/// <summary>
		///  冒险队排名
		/// </summary>
		public UInt32 adventureTeamRanking;
		/// <summary>
		///  拥有角色栏位数
		/// </summary>
		public UInt32 adventureTeamOwnRoleFileds;
		/// <summary>
		///  冒险团角色总评分
		/// </summary>
		public UInt32 adventureTeamRoleTotalScore;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] adventureTeamNameBytes = StringHelper.StringToUTF8Bytes(adventureTeamName);
				BaseDLL.encode_string(buffer, ref pos_, adventureTeamNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, adventureTeamLevel);
				BaseDLL.encode_uint64(buffer, ref pos_, adventureTeamExp);
				byte[] adventureTeamGradeBytes = StringHelper.StringToUTF8Bytes(adventureTeamGrade);
				BaseDLL.encode_string(buffer, ref pos_, adventureTeamGradeBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, adventureTeamRanking);
				BaseDLL.encode_uint32(buffer, ref pos_, adventureTeamOwnRoleFileds);
				BaseDLL.encode_uint32(buffer, ref pos_, adventureTeamRoleTotalScore);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 adventureTeamNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref adventureTeamNameLen);
				byte[] adventureTeamNameBytes = new byte[adventureTeamNameLen];
				for(int i = 0; i < adventureTeamNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref adventureTeamNameBytes[i]);
				}
				adventureTeamName = StringHelper.BytesToString(adventureTeamNameBytes);
				BaseDLL.decode_uint16(buffer, ref pos_, ref adventureTeamLevel);
				BaseDLL.decode_uint64(buffer, ref pos_, ref adventureTeamExp);
				UInt16 adventureTeamGradeLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref adventureTeamGradeLen);
				byte[] adventureTeamGradeBytes = new byte[adventureTeamGradeLen];
				for(int i = 0; i < adventureTeamGradeLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref adventureTeamGradeBytes[i]);
				}
				adventureTeamGrade = StringHelper.BytesToString(adventureTeamGradeBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref adventureTeamRanking);
				BaseDLL.decode_uint32(buffer, ref pos_, ref adventureTeamOwnRoleFileds);
				BaseDLL.decode_uint32(buffer, ref pos_, ref adventureTeamRoleTotalScore);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] adventureTeamNameBytes = StringHelper.StringToUTF8Bytes(adventureTeamName);
				BaseDLL.encode_string(buffer, ref pos_, adventureTeamNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, adventureTeamLevel);
				BaseDLL.encode_uint64(buffer, ref pos_, adventureTeamExp);
				byte[] adventureTeamGradeBytes = StringHelper.StringToUTF8Bytes(adventureTeamGrade);
				BaseDLL.encode_string(buffer, ref pos_, adventureTeamGradeBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, adventureTeamRanking);
				BaseDLL.encode_uint32(buffer, ref pos_, adventureTeamOwnRoleFileds);
				BaseDLL.encode_uint32(buffer, ref pos_, adventureTeamRoleTotalScore);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 adventureTeamNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref adventureTeamNameLen);
				byte[] adventureTeamNameBytes = new byte[adventureTeamNameLen];
				for(int i = 0; i < adventureTeamNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref adventureTeamNameBytes[i]);
				}
				adventureTeamName = StringHelper.BytesToString(adventureTeamNameBytes);
				BaseDLL.decode_uint16(buffer, ref pos_, ref adventureTeamLevel);
				BaseDLL.decode_uint64(buffer, ref pos_, ref adventureTeamExp);
				UInt16 adventureTeamGradeLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref adventureTeamGradeLen);
				byte[] adventureTeamGradeBytes = new byte[adventureTeamGradeLen];
				for(int i = 0; i < adventureTeamGradeLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref adventureTeamGradeBytes[i]);
				}
				adventureTeamGrade = StringHelper.BytesToString(adventureTeamGradeBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref adventureTeamRanking);
				BaseDLL.decode_uint32(buffer, ref pos_, ref adventureTeamOwnRoleFileds);
				BaseDLL.decode_uint32(buffer, ref pos_, ref adventureTeamRoleTotalScore);
			}

			public int getLen()
			{
				int _len = 0;
				// adventureTeamName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(adventureTeamName);
					_len += 2 + _strBytes.Length;
				}
				// adventureTeamLevel
				_len += 2;
				// adventureTeamExp
				_len += 8;
				// adventureTeamGrade
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(adventureTeamGrade);
					_len += 2 + _strBytes.Length;
				}
				// adventureTeamRanking
				_len += 4;
				// adventureTeamOwnRoleFileds
				_len += 4;
				// adventureTeamRoleTotalScore
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  远征队员信息
	/// </summary>
	public class ExpeditionMemberInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  角色id
		/// </summary>
		public UInt64 roleid;
		/// <summary>
		///  角色名
		/// </summary>
		public string name;
		/// <summary>
		///  角色等级
		/// </summary>
		public UInt16 level;
		/// <summary>
		///  角色职业
		/// </summary>
		public byte occu;
		/// <summary>
		///  角色外观
		/// </summary>
		public PlayerAvatar avatar = new PlayerAvatar();
		/// <summary>
		///  远征地图id
		/// </summary>
		public byte expeditionMapId;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleid);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				avatar.encode(buffer, ref pos_);
				BaseDLL.encode_int8(buffer, ref pos_, expeditionMapId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleid);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				avatar.decode(buffer, ref pos_);
				BaseDLL.decode_int8(buffer, ref pos_, ref expeditionMapId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleid);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				avatar.encode(buffer, ref pos_);
				BaseDLL.encode_int8(buffer, ref pos_, expeditionMapId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleid);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				avatar.decode(buffer, ref pos_);
				BaseDLL.decode_int8(buffer, ref pos_, ref expeditionMapId);
			}

			public int getLen()
			{
				int _len = 0;
				// roleid
				_len += 8;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// level
				_len += 2;
				// occu
				_len += 1;
				// avatar
				_len += avatar.getLen();
				// expeditionMapId
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  远征地图基本信息
	/// </summary>
	public class ExpeditionMapBaseInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  地图id
		/// </summary>
		public byte mapId;
		/// <summary>
		///  远征状态(对应枚举ExpeditionStatus)
		/// </summary>
		public byte expeditionStatus;
		/// <summary>
		///  远征队员数
		/// </summary>
		public UInt16 memberNum;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, mapId);
				BaseDLL.encode_int8(buffer, ref pos_, expeditionStatus);
				BaseDLL.encode_uint16(buffer, ref pos_, memberNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref mapId);
				BaseDLL.decode_int8(buffer, ref pos_, ref expeditionStatus);
				BaseDLL.decode_uint16(buffer, ref pos_, ref memberNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, mapId);
				BaseDLL.encode_int8(buffer, ref pos_, expeditionStatus);
				BaseDLL.encode_uint16(buffer, ref pos_, memberNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref mapId);
				BaseDLL.decode_int8(buffer, ref pos_, ref expeditionStatus);
				BaseDLL.decode_uint16(buffer, ref pos_, ref memberNum);
			}

			public int getLen()
			{
				int _len = 0;
				// mapId
				_len += 1;
				// expeditionStatus
				_len += 1;
				// memberNum
				_len += 2;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 同步冒险队信息
	/// </summary>
	[Protocol]
	public class AdventureTeamInfoSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 308601;
		public UInt32 Sequence;
		/// <summary>
		///  冒险队信息
		/// </summary>
		public AdventureTeamExtraInfo info = new AdventureTeamExtraInfo();

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
	///  client->world 冒险队队名修改请求
	/// </summary>
	[Protocol]
	public class WorldAdventureTeamRenameReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608601;
		public UInt32 Sequence;
		/// <summary>
		///  要修改的新名字
		/// </summary>
		public string newName;
		/// <summary>
		///  需要消耗的道具
		/// </summary>
		public UInt64 costItemUId;
		public UInt32 costItemDataId;

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
				byte[] newNameBytes = StringHelper.StringToUTF8Bytes(newName);
				BaseDLL.encode_string(buffer, ref pos_, newNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint64(buffer, ref pos_, costItemUId);
				BaseDLL.encode_uint32(buffer, ref pos_, costItemDataId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 newNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref newNameLen);
				byte[] newNameBytes = new byte[newNameLen];
				for(int i = 0; i < newNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref newNameBytes[i]);
				}
				newName = StringHelper.BytesToString(newNameBytes);
				BaseDLL.decode_uint64(buffer, ref pos_, ref costItemUId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref costItemDataId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] newNameBytes = StringHelper.StringToUTF8Bytes(newName);
				BaseDLL.encode_string(buffer, ref pos_, newNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint64(buffer, ref pos_, costItemUId);
				BaseDLL.encode_uint32(buffer, ref pos_, costItemDataId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 newNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref newNameLen);
				byte[] newNameBytes = new byte[newNameLen];
				for(int i = 0; i < newNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref newNameBytes[i]);
				}
				newName = StringHelper.BytesToString(newNameBytes);
				BaseDLL.decode_uint64(buffer, ref pos_, ref costItemUId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref costItemDataId);
			}

			public int getLen()
			{
				int _len = 0;
				// newName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(newName);
					_len += 2 + _strBytes.Length;
				}
				// costItemUId
				_len += 8;
				// costItemDataId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  world->client 冒险队队名修改返回
	/// </summary>
	[Protocol]
	public class WorldAdventureTeamRenameRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608602;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		public UInt32 resCode;
		/// <summary>
		///  要修改的新名字
		/// </summary>
		public string newName;

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
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				byte[] newNameBytes = StringHelper.StringToUTF8Bytes(newName);
				BaseDLL.encode_string(buffer, ref pos_, newNameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				UInt16 newNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref newNameLen);
				byte[] newNameBytes = new byte[newNameLen];
				for(int i = 0; i < newNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref newNameBytes[i]);
				}
				newName = StringHelper.BytesToString(newNameBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				byte[] newNameBytes = StringHelper.StringToUTF8Bytes(newName);
				BaseDLL.encode_string(buffer, ref pos_, newNameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				UInt16 newNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref newNameLen);
				byte[] newNameBytes = new byte[newNameLen];
				for(int i = 0; i < newNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref newNameBytes[i]);
				}
				newName = StringHelper.BytesToString(newNameBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// resCode
				_len += 4;
				// newName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(newName);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->world 赐福水晶信息请求
	/// </summary>
	[Protocol]
	public class WorldBlessCrystalInfoReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608603;
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
	///  world->client 赐福水晶信息返回
	/// </summary>
	[Protocol]
	public class WorldBlessCrystalInfoRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608604;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		public UInt32 resCode;
		/// <summary>
		///  拥有的赐福水晶数量
		/// </summary>
		public UInt32 ownBlessCrystalNum;
		/// <summary>
		///  拥有的赐福水晶经验
		/// </summary>
		public UInt64 ownBlessCrystalExp;

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
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				BaseDLL.encode_uint32(buffer, ref pos_, ownBlessCrystalNum);
				BaseDLL.encode_uint64(buffer, ref pos_, ownBlessCrystalExp);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref ownBlessCrystalNum);
				BaseDLL.decode_uint64(buffer, ref pos_, ref ownBlessCrystalExp);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				BaseDLL.encode_uint32(buffer, ref pos_, ownBlessCrystalNum);
				BaseDLL.encode_uint64(buffer, ref pos_, ownBlessCrystalExp);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref ownBlessCrystalNum);
				BaseDLL.decode_uint64(buffer, ref pos_, ref ownBlessCrystalExp);
			}

			public int getLen()
			{
				int _len = 0;
				// resCode
				_len += 4;
				// ownBlessCrystalNum
				_len += 4;
				// ownBlessCrystalExp
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->world 传承祝福信息请求
	/// </summary>
	[Protocol]
	public class WorldInheritBlessInfoReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608605;
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
	///  world->client 传承祝福信息返回
	/// </summary>
	[Protocol]
	public class WorldInheritBlessInfoRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608606;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		public UInt32 resCode;
		/// <summary>
		///  拥有的传承祝福数量
		/// </summary>
		public UInt32 ownInheritBlessNum;
		/// <summary>
		///  拥有的传承祝福经验
		/// </summary>
		public UInt64 ownInheritBlessExp;

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
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				BaseDLL.encode_uint32(buffer, ref pos_, ownInheritBlessNum);
				BaseDLL.encode_uint64(buffer, ref pos_, ownInheritBlessExp);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref ownInheritBlessNum);
				BaseDLL.decode_uint64(buffer, ref pos_, ref ownInheritBlessExp);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				BaseDLL.encode_uint32(buffer, ref pos_, ownInheritBlessNum);
				BaseDLL.encode_uint64(buffer, ref pos_, ownInheritBlessExp);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref ownInheritBlessNum);
				BaseDLL.decode_uint64(buffer, ref pos_, ref ownInheritBlessExp);
			}

			public int getLen()
			{
				int _len = 0;
				// resCode
				_len += 4;
				// ownInheritBlessNum
				_len += 4;
				// ownInheritBlessExp
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->world 传承经验请求
	/// </summary>
	[Protocol]
	public class WorldInheritExpReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608607;
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
	///  world->client 传承经验返回
	/// </summary>
	[Protocol]
	public class WorldInheritExpRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608608;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		public UInt32 resCode;
		/// <summary>
		///  拥有的传承祝福数量
		/// </summary>
		public UInt32 ownInheritBlessNum;
		/// <summary>
		///  拥有的传承祝福经验
		/// </summary>
		public UInt64 ownInheritBlessExp;

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
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				BaseDLL.encode_uint32(buffer, ref pos_, ownInheritBlessNum);
				BaseDLL.encode_uint64(buffer, ref pos_, ownInheritBlessExp);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref ownInheritBlessNum);
				BaseDLL.decode_uint64(buffer, ref pos_, ref ownInheritBlessExp);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				BaseDLL.encode_uint32(buffer, ref pos_, ownInheritBlessNum);
				BaseDLL.encode_uint64(buffer, ref pos_, ownInheritBlessExp);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref ownInheritBlessNum);
				BaseDLL.decode_uint64(buffer, ref pos_, ref ownInheritBlessExp);
			}

			public int getLen()
			{
				int _len = 0;
				// resCode
				_len += 4;
				// ownInheritBlessNum
				_len += 4;
				// ownInheritBlessExp
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->world 冒险队扩展信息请求
	/// </summary>
	[Protocol]
	public class WorldAdventureTeamExtraInfoReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608609;
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
	///  world->client 冒险队扩展信息返回
	/// </summary>
	[Protocol]
	public class WorldAdventureTeamExtraInfoRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608610;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		public UInt32 resCode;
		public AdventureTeamExtraInfo extraInfo = new AdventureTeamExtraInfo();

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
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				extraInfo.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				extraInfo.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				extraInfo.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				extraInfo.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// resCode
				_len += 4;
				// extraInfo
				_len += extraInfo.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->world 远征地图查询请求
	/// </summary>
	[Protocol]
	public class WorldQueryExpeditionMapReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608611;
		public UInt32 Sequence;
		/// <summary>
		///  地图id
		/// </summary>
		public byte mapId;

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
				BaseDLL.encode_int8(buffer, ref pos_, mapId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref mapId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, mapId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref mapId);
			}

			public int getLen()
			{
				int _len = 0;
				// mapId
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  world->client 远征地图查询返回
	/// </summary>
	[Protocol]
	public class WorldQueryExpeditionMapRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608612;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		public UInt32 resCode;
		/// <summary>
		///  地图id
		/// </summary>
		public byte mapId;
		/// <summary>
		///  远征状态(对应枚举ExpeditionStatus)
		/// </summary>
		public byte expeditionStatus;
		/// <summary>
		///  远征持续时间(小时)
		/// </summary>
		public UInt32 durationOfExpedition;
		/// <summary>
		///  远征结束时间
		/// </summary>
		public UInt32 endTimeOfExpedition;
		/// <summary>
		///  远征队员信息
		/// </summary>
		public ExpeditionMemberInfo[] members = new ExpeditionMemberInfo[0];

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
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				BaseDLL.encode_int8(buffer, ref pos_, mapId);
				BaseDLL.encode_int8(buffer, ref pos_, expeditionStatus);
				BaseDLL.encode_uint32(buffer, ref pos_, durationOfExpedition);
				BaseDLL.encode_uint32(buffer, ref pos_, endTimeOfExpedition);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)members.Length);
				for(int i = 0; i < members.Length; i++)
				{
					members[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				BaseDLL.decode_int8(buffer, ref pos_, ref mapId);
				BaseDLL.decode_int8(buffer, ref pos_, ref expeditionStatus);
				BaseDLL.decode_uint32(buffer, ref pos_, ref durationOfExpedition);
				BaseDLL.decode_uint32(buffer, ref pos_, ref endTimeOfExpedition);
				UInt16 membersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref membersCnt);
				members = new ExpeditionMemberInfo[membersCnt];
				for(int i = 0; i < members.Length; i++)
				{
					members[i] = new ExpeditionMemberInfo();
					members[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				BaseDLL.encode_int8(buffer, ref pos_, mapId);
				BaseDLL.encode_int8(buffer, ref pos_, expeditionStatus);
				BaseDLL.encode_uint32(buffer, ref pos_, durationOfExpedition);
				BaseDLL.encode_uint32(buffer, ref pos_, endTimeOfExpedition);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)members.Length);
				for(int i = 0; i < members.Length; i++)
				{
					members[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				BaseDLL.decode_int8(buffer, ref pos_, ref mapId);
				BaseDLL.decode_int8(buffer, ref pos_, ref expeditionStatus);
				BaseDLL.decode_uint32(buffer, ref pos_, ref durationOfExpedition);
				BaseDLL.decode_uint32(buffer, ref pos_, ref endTimeOfExpedition);
				UInt16 membersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref membersCnt);
				members = new ExpeditionMemberInfo[membersCnt];
				for(int i = 0; i < members.Length; i++)
				{
					members[i] = new ExpeditionMemberInfo();
					members[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// resCode
				_len += 4;
				// mapId
				_len += 1;
				// expeditionStatus
				_len += 1;
				// durationOfExpedition
				_len += 4;
				// endTimeOfExpedition
				_len += 4;
				// members
				_len += 2;
				for(int j = 0; j < members.Length; j++)
				{
					_len += members[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->world 可选远征角色查询请求
	/// </summary>
	[Protocol]
	public class WorldQueryOptionalExpeditionRolesReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608613;
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
	///  world->client 可选远征角色查询返回
	/// </summary>
	[Protocol]
	public class WorldQueryOptionalExpeditionRolesRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608614;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		public UInt32 resCode;
		/// <summary>
		///  可选远征角色列表
		/// </summary>
		public ExpeditionMemberInfo[] roles = new ExpeditionMemberInfo[0];

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
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)roles.Length);
				for(int i = 0; i < roles.Length; i++)
				{
					roles[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				UInt16 rolesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref rolesCnt);
				roles = new ExpeditionMemberInfo[rolesCnt];
				for(int i = 0; i < roles.Length; i++)
				{
					roles[i] = new ExpeditionMemberInfo();
					roles[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)roles.Length);
				for(int i = 0; i < roles.Length; i++)
				{
					roles[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				UInt16 rolesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref rolesCnt);
				roles = new ExpeditionMemberInfo[rolesCnt];
				for(int i = 0; i < roles.Length; i++)
				{
					roles[i] = new ExpeditionMemberInfo();
					roles[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// resCode
				_len += 4;
				// roles
				_len += 2;
				for(int j = 0; j < roles.Length; j++)
				{
					_len += roles[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->world 派遣远征队请求
	/// </summary>
	[Protocol]
	public class WorldDispatchExpeditionTeamReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608615;
		public UInt32 Sequence;
		/// <summary>
		///  地图id
		/// </summary>
		public byte mapId;
		/// <summary>
		///  远征成员(角色id列表)
		/// </summary>
		public UInt64[] members = new UInt64[0];
		/// <summary>
		///  远征时间(持续时间 小时)
		/// </summary>
		public UInt32 housOfduration;

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
				BaseDLL.encode_int8(buffer, ref pos_, mapId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)members.Length);
				for(int i = 0; i < members.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, members[i]);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, housOfduration);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref mapId);
				UInt16 membersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref membersCnt);
				members = new UInt64[membersCnt];
				for(int i = 0; i < members.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref members[i]);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref housOfduration);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, mapId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)members.Length);
				for(int i = 0; i < members.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, members[i]);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, housOfduration);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref mapId);
				UInt16 membersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref membersCnt);
				members = new UInt64[membersCnt];
				for(int i = 0; i < members.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref members[i]);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref housOfduration);
			}

			public int getLen()
			{
				int _len = 0;
				// mapId
				_len += 1;
				// members
				_len += 2 + 8 * members.Length;
				// housOfduration
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  world->client 派遣远征队返回
	/// </summary>
	[Protocol]
	public class WorldDispatchExpeditionTeamRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608616;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		public UInt32 resCode;
		/// <summary>
		///  地图id
		/// </summary>
		public byte mapId;
		/// <summary>
		///  远征状态(对应枚举ExpeditionStatus)
		/// </summary>
		public byte expeditionStatus;
		/// <summary>
		///  远征持续时间(小时)
		/// </summary>
		public UInt32 durationOfExpedition;
		/// <summary>
		///  远征结束时间
		/// </summary>
		public UInt32 endTimeOfExpedition;
		/// <summary>
		///  远征队员信息
		/// </summary>
		public ExpeditionMemberInfo[] members = new ExpeditionMemberInfo[0];

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
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				BaseDLL.encode_int8(buffer, ref pos_, mapId);
				BaseDLL.encode_int8(buffer, ref pos_, expeditionStatus);
				BaseDLL.encode_uint32(buffer, ref pos_, durationOfExpedition);
				BaseDLL.encode_uint32(buffer, ref pos_, endTimeOfExpedition);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)members.Length);
				for(int i = 0; i < members.Length; i++)
				{
					members[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				BaseDLL.decode_int8(buffer, ref pos_, ref mapId);
				BaseDLL.decode_int8(buffer, ref pos_, ref expeditionStatus);
				BaseDLL.decode_uint32(buffer, ref pos_, ref durationOfExpedition);
				BaseDLL.decode_uint32(buffer, ref pos_, ref endTimeOfExpedition);
				UInt16 membersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref membersCnt);
				members = new ExpeditionMemberInfo[membersCnt];
				for(int i = 0; i < members.Length; i++)
				{
					members[i] = new ExpeditionMemberInfo();
					members[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				BaseDLL.encode_int8(buffer, ref pos_, mapId);
				BaseDLL.encode_int8(buffer, ref pos_, expeditionStatus);
				BaseDLL.encode_uint32(buffer, ref pos_, durationOfExpedition);
				BaseDLL.encode_uint32(buffer, ref pos_, endTimeOfExpedition);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)members.Length);
				for(int i = 0; i < members.Length; i++)
				{
					members[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				BaseDLL.decode_int8(buffer, ref pos_, ref mapId);
				BaseDLL.decode_int8(buffer, ref pos_, ref expeditionStatus);
				BaseDLL.decode_uint32(buffer, ref pos_, ref durationOfExpedition);
				BaseDLL.decode_uint32(buffer, ref pos_, ref endTimeOfExpedition);
				UInt16 membersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref membersCnt);
				members = new ExpeditionMemberInfo[membersCnt];
				for(int i = 0; i < members.Length; i++)
				{
					members[i] = new ExpeditionMemberInfo();
					members[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// resCode
				_len += 4;
				// mapId
				_len += 1;
				// expeditionStatus
				_len += 1;
				// durationOfExpedition
				_len += 4;
				// endTimeOfExpedition
				_len += 4;
				// members
				_len += 2;
				for(int j = 0; j < members.Length; j++)
				{
					_len += members[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->world 取消远征请求
	/// </summary>
	[Protocol]
	public class WorldCancelExpeditionReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608617;
		public UInt32 Sequence;
		/// <summary>
		///  地图id
		/// </summary>
		public byte mapId;

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
				BaseDLL.encode_int8(buffer, ref pos_, mapId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref mapId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, mapId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref mapId);
			}

			public int getLen()
			{
				int _len = 0;
				// mapId
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  world->client 取消远征返回
	/// </summary>
	[Protocol]
	public class WorldCancelExpeditionRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608618;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		public UInt32 resCode;
		/// <summary>
		///  地图id
		/// </summary>
		public byte mapId;
		/// <summary>
		///  远征状态(对应枚举ExpeditionStatus)
		/// </summary>
		public byte expeditionStatus;

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
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				BaseDLL.encode_int8(buffer, ref pos_, mapId);
				BaseDLL.encode_int8(buffer, ref pos_, expeditionStatus);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				BaseDLL.decode_int8(buffer, ref pos_, ref mapId);
				BaseDLL.decode_int8(buffer, ref pos_, ref expeditionStatus);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				BaseDLL.encode_int8(buffer, ref pos_, mapId);
				BaseDLL.encode_int8(buffer, ref pos_, expeditionStatus);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				BaseDLL.decode_int8(buffer, ref pos_, ref mapId);
				BaseDLL.decode_int8(buffer, ref pos_, ref expeditionStatus);
			}

			public int getLen()
			{
				int _len = 0;
				// resCode
				_len += 4;
				// mapId
				_len += 1;
				// expeditionStatus
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->world 远征奖励领取请求
	/// </summary>
	[Protocol]
	public class WorldGetExpeditionRewardsReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608619;
		public UInt32 Sequence;
		/// <summary>
		///  地图id
		/// </summary>
		public byte mapId;

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
				BaseDLL.encode_int8(buffer, ref pos_, mapId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref mapId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, mapId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref mapId);
			}

			public int getLen()
			{
				int _len = 0;
				// mapId
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  world->client 远征奖励领取返回
	/// </summary>
	[Protocol]
	public class WorldGetExpeditionRewardsRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608620;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		public UInt32 resCode;
		/// <summary>
		///  地图id
		/// </summary>
		public byte mapId;
		/// <summary>
		///  远征状态(对应枚举ExpeditionStatus)
		/// </summary>
		public byte expeditionStatus;

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
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				BaseDLL.encode_int8(buffer, ref pos_, mapId);
				BaseDLL.encode_int8(buffer, ref pos_, expeditionStatus);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				BaseDLL.decode_int8(buffer, ref pos_, ref mapId);
				BaseDLL.decode_int8(buffer, ref pos_, ref expeditionStatus);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				BaseDLL.encode_int8(buffer, ref pos_, mapId);
				BaseDLL.encode_int8(buffer, ref pos_, expeditionStatus);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				BaseDLL.decode_int8(buffer, ref pos_, ref mapId);
				BaseDLL.decode_int8(buffer, ref pos_, ref expeditionStatus);
			}

			public int getLen()
			{
				int _len = 0;
				// resCode
				_len += 4;
				// mapId
				_len += 1;
				// expeditionStatus
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->world 查询全部远征地图请求
	/// </summary>
	[Protocol]
	public class WorldQueryAllExpeditionMapsReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608621;
		public UInt32 Sequence;
		/// <summary>
		///  地图id集
		/// </summary>
		public byte[] mapIds = new byte[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mapIds.Length);
				for(int i = 0; i < mapIds.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, mapIds[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 mapIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref mapIdsCnt);
				mapIds = new byte[mapIdsCnt];
				for(int i = 0; i < mapIds.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref mapIds[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mapIds.Length);
				for(int i = 0; i < mapIds.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, mapIds[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 mapIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref mapIdsCnt);
				mapIds = new byte[mapIdsCnt];
				for(int i = 0; i < mapIds.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref mapIds[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// mapIds
				_len += 2 + 1 * mapIds.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  world->client 查询全部远征地图返回
	/// </summary>
	[Protocol]
	public class WorldQueryAllExpeditionMapsRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608622;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		public UInt32 resCode;
		/// <summary>
		///  地图基本信息集
		/// </summary>
		public ExpeditionMapBaseInfo[] mapBaseInfos = new ExpeditionMapBaseInfo[0];

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
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mapBaseInfos.Length);
				for(int i = 0; i < mapBaseInfos.Length; i++)
				{
					mapBaseInfos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				UInt16 mapBaseInfosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref mapBaseInfosCnt);
				mapBaseInfos = new ExpeditionMapBaseInfo[mapBaseInfosCnt];
				for(int i = 0; i < mapBaseInfos.Length; i++)
				{
					mapBaseInfos[i] = new ExpeditionMapBaseInfo();
					mapBaseInfos[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mapBaseInfos.Length);
				for(int i = 0; i < mapBaseInfos.Length; i++)
				{
					mapBaseInfos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				UInt16 mapBaseInfosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref mapBaseInfosCnt);
				mapBaseInfos = new ExpeditionMapBaseInfo[mapBaseInfosCnt];
				for(int i = 0; i < mapBaseInfos.Length; i++)
				{
					mapBaseInfos[i] = new ExpeditionMapBaseInfo();
					mapBaseInfos[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// resCode
				_len += 4;
				// mapBaseInfos
				_len += 2;
				for(int j = 0; j < mapBaseInfos.Length; j++)
				{
					_len += mapBaseInfos[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->world 请求查询拥有的职业
	/// </summary>
	[Protocol]
	public class WorldQueryOwnOccupationsReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608623;
		public UInt32 Sequence;
		/// <summary>
		///  基础职业
		/// </summary>
		public byte[] baseOccus = new byte[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)baseOccus.Length);
				for(int i = 0; i < baseOccus.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, baseOccus[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 baseOccusCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref baseOccusCnt);
				baseOccus = new byte[baseOccusCnt];
				for(int i = 0; i < baseOccus.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref baseOccus[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)baseOccus.Length);
				for(int i = 0; i < baseOccus.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, baseOccus[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 baseOccusCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref baseOccusCnt);
				baseOccus = new byte[baseOccusCnt];
				for(int i = 0; i < baseOccus.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref baseOccus[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// baseOccus
				_len += 2 + 1 * baseOccus.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  world->client 查询拥有的职业返回
	/// </summary>
	[Protocol]
	public class WorldQueryOwnOccupationsRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608624;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		public UInt32 resCode;
		/// <summary>
		///  职业集
		/// </summary>
		public byte[] occus = new byte[0];

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
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)occus.Length);
				for(int i = 0; i < occus.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, occus[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				UInt16 occusCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref occusCnt);
				occus = new byte[occusCnt];
				for(int i = 0; i < occus.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref occus[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)occus.Length);
				for(int i = 0; i < occus.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, occus[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				UInt16 occusCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref occusCnt);
				occus = new byte[occusCnt];
				for(int i = 0; i < occus.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref occus[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// resCode
				_len += 4;
				// occus
				_len += 2 + 1 * occus.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  world->client 同步拥有的新职业
	/// </summary>
	[Protocol]
	public class WorldQueryOwnOccupationsSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608625;
		public UInt32 Sequence;
		/// <summary>
		///  拥有的新职业
		/// </summary>
		public byte[] ownNewOccus = new byte[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)ownNewOccus.Length);
				for(int i = 0; i < ownNewOccus.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, ownNewOccus[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 ownNewOccusCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref ownNewOccusCnt);
				ownNewOccus = new byte[ownNewOccusCnt];
				for(int i = 0; i < ownNewOccus.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref ownNewOccus[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)ownNewOccus.Length);
				for(int i = 0; i < ownNewOccus.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, ownNewOccus[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 ownNewOccusCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref ownNewOccusCnt);
				ownNewOccus = new byte[ownNewOccusCnt];
				for(int i = 0; i < ownNewOccus.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref ownNewOccus[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// ownNewOccus
				_len += 2 + 1 * ownNewOccus.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->world 请求清除解锁的新职业
	/// </summary>
	[Protocol]
	public class WorldRemoveUnlockedNewOccupationsReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608626;
		public UInt32 Sequence;
		/// <summary>
		///  新职业
		/// </summary>
		public byte[] newOccus = new byte[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)newOccus.Length);
				for(int i = 0; i < newOccus.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, newOccus[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 newOccusCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref newOccusCnt);
				newOccus = new byte[newOccusCnt];
				for(int i = 0; i < newOccus.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref newOccus[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)newOccus.Length);
				for(int i = 0; i < newOccus.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, newOccus[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 newOccusCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref newOccusCnt);
				newOccus = new byte[newOccusCnt];
				for(int i = 0; i < newOccus.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref newOccus[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// newOccus
				_len += 2 + 1 * newOccus.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  world->client 同步全部远征地图信息
	/// </summary>
	[Protocol]
	public class WorldAllExpeditionMapsSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608628;
		public UInt32 Sequence;
		public ExpeditionMapBaseInfo[] mapBaseInfos = new ExpeditionMapBaseInfo[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mapBaseInfos.Length);
				for(int i = 0; i < mapBaseInfos.Length; i++)
				{
					mapBaseInfos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 mapBaseInfosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref mapBaseInfosCnt);
				mapBaseInfos = new ExpeditionMapBaseInfo[mapBaseInfosCnt];
				for(int i = 0; i < mapBaseInfos.Length; i++)
				{
					mapBaseInfos[i] = new ExpeditionMapBaseInfo();
					mapBaseInfos[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mapBaseInfos.Length);
				for(int i = 0; i < mapBaseInfos.Length; i++)
				{
					mapBaseInfos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 mapBaseInfosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref mapBaseInfosCnt);
				mapBaseInfos = new ExpeditionMapBaseInfo[mapBaseInfosCnt];
				for(int i = 0; i < mapBaseInfos.Length; i++)
				{
					mapBaseInfos[i] = new ExpeditionMapBaseInfo();
					mapBaseInfos[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// mapBaseInfos
				_len += 2;
				for(int j = 0; j < mapBaseInfos.Length; j++)
				{
					_len += mapBaseInfos[j].getLen();
				}
				return _len;
			}
		#endregion

	}

}
