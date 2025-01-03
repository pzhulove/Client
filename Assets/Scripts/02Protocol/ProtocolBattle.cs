using System;
using System.Text;

namespace Protocol
{
	[Protocol]
	public class SceneBattleUseItemReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508901;
		public UInt32 Sequence;
		public UInt64 uid;
		public byte useAll;
		public UInt32 param1;
		public UInt32 param2;
		public UInt32 battleID;

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
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_int8(buffer, ref pos_, useAll);
				BaseDLL.encode_uint32(buffer, ref pos_, param1);
				BaseDLL.encode_uint32(buffer, ref pos_, param2);
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_int8(buffer, ref pos_, ref useAll);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param1);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param2);
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_int8(buffer, ref pos_, useAll);
				BaseDLL.encode_uint32(buffer, ref pos_, param1);
				BaseDLL.encode_uint32(buffer, ref pos_, param2);
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_int8(buffer, ref pos_, ref useAll);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param1);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param2);
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
			}

			public int getLen()
			{
				int _len = 0;
				// uid
				_len += 8;
				// useAll
				_len += 1;
				// param1
				_len += 4;
				// param2
				_len += 4;
				// battleID
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneBattleUseItemRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508902;
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

	[Protocol]
	public class SceneBattleChangeSkillsReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508903;
		public UInt32 Sequence;
		public byte configType;
		public ChangeSkill[] skills = new ChangeSkill[0];

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
				BaseDLL.encode_int8(buffer, ref pos_, configType);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)skills.Length);
				for(int i = 0; i < skills.Length; i++)
				{
					skills[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref configType);
				UInt16 skillsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref skillsCnt);
				skills = new ChangeSkill[skillsCnt];
				for(int i = 0; i < skills.Length; i++)
				{
					skills[i] = new ChangeSkill();
					skills[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, configType);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)skills.Length);
				for(int i = 0; i < skills.Length; i++)
				{
					skills[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref configType);
				UInt16 skillsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref skillsCnt);
				skills = new ChangeSkill[skillsCnt];
				for(int i = 0; i < skills.Length; i++)
				{
					skills[i] = new ChangeSkill();
					skills[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// configType
				_len += 1;
				// skills
				_len += 2;
				for(int j = 0; j < skills.Length; j++)
				{
					_len += skills[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneBattleChangeSkillsRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508904;
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
	///  战场报名请求
	/// </summary>
	[Protocol]
	public class BattleEnrollReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 2200005;
		public UInt32 Sequence;
		/// <summary>
		///  0取消报名，非0报名
		/// </summary>
		public UInt32 isMatch;
		public UInt32 accId;
		public UInt64 roleId;
		public string playerName;
		public byte playerOccu;

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
				BaseDLL.encode_uint32(buffer, ref pos_, isMatch);
				BaseDLL.encode_uint32(buffer, ref pos_, accId);
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
				BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, playerOccu);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref isMatch);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				UInt16 playerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerNameLen);
				byte[] playerNameBytes = new byte[playerNameLen];
				for(int i = 0; i < playerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref playerNameBytes[i]);
				}
				playerName = StringHelper.BytesToString(playerNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref playerOccu);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, isMatch);
				BaseDLL.encode_uint32(buffer, ref pos_, accId);
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
				BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, playerOccu);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref isMatch);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				UInt16 playerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerNameLen);
				byte[] playerNameBytes = new byte[playerNameLen];
				for(int i = 0; i < playerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref playerNameBytes[i]);
				}
				playerName = StringHelper.BytesToString(playerNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref playerOccu);
			}

			public int getLen()
			{
				int _len = 0;
				// isMatch
				_len += 4;
				// accId
				_len += 4;
				// roleId
				_len += 8;
				// playerName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(playerName);
					_len += 2 + _strBytes.Length;
				}
				// playerOccu
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  战场报名返回
	/// </summary>
	[Protocol]
	public class BattleEnrollRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 2200006;
		public UInt32 Sequence;
		/// <summary>
		///  0取消报名，非0报名
		/// </summary>
		public UInt32 isMatch;
		/// <summary>
		///  返回值
		/// </summary>
		public UInt32 retCode;

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
				BaseDLL.encode_uint32(buffer, ref pos_, isMatch);
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref isMatch);
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, isMatch);
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref isMatch);
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public int getLen()
			{
				int _len = 0;
				// isMatch
				_len += 4;
				// retCode
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class BattlePkSomeOneReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 2200007;
		public UInt32 Sequence;
		public UInt64 roleId;
		public UInt64 dstId;
		public UInt32 battleID;
		public UInt32 dungeonID;

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
				BaseDLL.encode_uint64(buffer, ref pos_, dstId);
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonID);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref dstId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonID);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint64(buffer, ref pos_, dstId);
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonID);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref dstId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonID);
			}

			public int getLen()
			{
				int _len = 0;
				// roleId
				_len += 8;
				// dstId
				_len += 8;
				// battleID
				_len += 4;
				// dungeonID
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class BattlePkSomeOneRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 2200008;
		public UInt32 Sequence;
		public UInt32 retCode;
		public UInt64 roleId;
		public UInt64 dstId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint64(buffer, ref pos_, dstId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref dstId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint64(buffer, ref pos_, dstId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref dstId);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// roleId
				_len += 8;
				// dstId
				_len += 8;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class BattleLockSomeOneReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 2200013;
		public UInt32 Sequence;
		public UInt64 roleId;
		public UInt64 dstId;
		public UInt32 battleID;

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
				BaseDLL.encode_uint64(buffer, ref pos_, dstId);
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref dstId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint64(buffer, ref pos_, dstId);
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref dstId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
			}

			public int getLen()
			{
				int _len = 0;
				// roleId
				_len += 8;
				// dstId
				_len += 8;
				// battleID
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class BattleLockSomeOneRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 2200014;
		public UInt32 Sequence;
		public UInt32 retCode;
		public UInt64 roleId;
		public UInt64 dstId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint64(buffer, ref pos_, dstId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref dstId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint64(buffer, ref pos_, dstId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref dstId);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// roleId
				_len += 8;
				// dstId
				_len += 8;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneBattleBalanceEnd : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508905;
		public UInt32 Sequence;
		public UInt64 roleId;
		public UInt32 rank;
		public UInt32 playerNum;
		public UInt32 kills;
		public UInt32 survivalTime;
		public UInt32 score;
		public UInt32 battleID;
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
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint32(buffer, ref pos_, rank);
				BaseDLL.encode_uint32(buffer, ref pos_, playerNum);
				BaseDLL.encode_uint32(buffer, ref pos_, kills);
				BaseDLL.encode_uint32(buffer, ref pos_, survivalTime);
				BaseDLL.encode_uint32(buffer, ref pos_, score);
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint32(buffer, ref pos_, getHonor);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref rank);
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref kills);
				BaseDLL.decode_uint32(buffer, ref pos_, ref survivalTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref score);
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref getHonor);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint32(buffer, ref pos_, rank);
				BaseDLL.encode_uint32(buffer, ref pos_, playerNum);
				BaseDLL.encode_uint32(buffer, ref pos_, kills);
				BaseDLL.encode_uint32(buffer, ref pos_, survivalTime);
				BaseDLL.encode_uint32(buffer, ref pos_, score);
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint32(buffer, ref pos_, getHonor);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref rank);
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref kills);
				BaseDLL.decode_uint32(buffer, ref pos_, ref survivalTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref score);
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref getHonor);
			}

			public int getLen()
			{
				int _len = 0;
				// roleId
				_len += 8;
				// rank
				_len += 4;
				// playerNum
				_len += 4;
				// kills
				_len += 4;
				// survivalTime
				_len += 4;
				// score
				_len += 4;
				// battleID
				_len += 4;
				// getHonor
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class PlayerSubject : Protocol.IProtocolStream
	{
		public UInt32 accId;
		public UInt64 playerId;
		public string playerName;
		public byte occu;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, accId);
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
				BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occu);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref accId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
				UInt16 playerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerNameLen);
				byte[] playerNameBytes = new byte[playerNameLen];
				for(int i = 0; i < playerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref playerNameBytes[i]);
				}
				playerName = StringHelper.BytesToString(playerNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, accId);
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
				BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occu);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref accId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
				UInt16 playerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerNameLen);
				byte[] playerNameBytes = new byte[playerNameLen];
				for(int i = 0; i < playerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref playerNameBytes[i]);
				}
				playerName = StringHelper.BytesToString(playerNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
			}

			public int getLen()
			{
				int _len = 0;
				// accId
				_len += 4;
				// playerId
				_len += 8;
				// playerName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(playerName);
					_len += 2 + _strBytes.Length;
				}
				// occu
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneBattleMatchSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508906;
		public UInt32 Sequence;
		public UInt32 battleID;
		public UInt32 suvivalNum;
		public PlayerSubject[] players = new PlayerSubject[0];
		public UInt32 sceneNodeID;

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
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint32(buffer, ref pos_, suvivalNum);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)players.Length);
				for(int i = 0; i < players.Length; i++)
				{
					players[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, sceneNodeID);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref suvivalNum);
				UInt16 playersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playersCnt);
				players = new PlayerSubject[playersCnt];
				for(int i = 0; i < players.Length; i++)
				{
					players[i] = new PlayerSubject();
					players[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref sceneNodeID);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint32(buffer, ref pos_, suvivalNum);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)players.Length);
				for(int i = 0; i < players.Length; i++)
				{
					players[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, sceneNodeID);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref suvivalNum);
				UInt16 playersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playersCnt);
				players = new PlayerSubject[playersCnt];
				for(int i = 0; i < players.Length; i++)
				{
					players[i] = new PlayerSubject();
					players[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref sceneNodeID);
			}

			public int getLen()
			{
				int _len = 0;
				// battleID
				_len += 4;
				// suvivalNum
				_len += 4;
				// players
				_len += 2;
				for(int j = 0; j < players.Length; j++)
				{
					_len += players[j].getLen();
				}
				// sceneNodeID
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 吃鸡准备场景的人数
	/// </summary>
	[Protocol]
	public class BattleNotifyPrepareInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 2200009;
		public UInt32 Sequence;
		public UInt32 playerNum;
		public UInt32 totalNum;

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
				BaseDLL.encode_uint32(buffer, ref pos_, playerNum);
				BaseDLL.encode_uint32(buffer, ref pos_, totalNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, playerNum);
				BaseDLL.encode_uint32(buffer, ref pos_, totalNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalNum);
			}

			public int getLen()
			{
				int _len = 0;
				// playerNum
				_len += 4;
				// totalNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 吃鸡战斗场景的人数
	/// </summary>
	[Protocol]
	public class BattleNotifyBattleInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 2200010;
		public UInt32 Sequence;
		public UInt32 battleID;
		public UInt32 playerNum;

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
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint32(buffer, ref pos_, playerNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint32(buffer, ref pos_, playerNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerNum);
			}

			public int getLen()
			{
				int _len = 0;
				// battleID
				_len += 4;
				// playerNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class BattleServerSpecifyPkRobot : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 2200011;
		public UInt32 Sequence;
		public byte hard;
		public byte occu;
		public byte ai;

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
				BaseDLL.encode_int8(buffer, ref pos_, hard);
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_int8(buffer, ref pos_, ai);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref hard);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_int8(buffer, ref pos_, ref ai);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, hard);
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_int8(buffer, ref pos_, ai);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref hard);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_int8(buffer, ref pos_, ref ai);
			}

			public int getLen()
			{
				int _len = 0;
				// hard
				_len += 1;
				// occu
				_len += 1;
				// ai
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class BattleServerAddPkRobot : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 2200012;
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
	/// 同步毒圈
	/// </summary>
	[Protocol]
	public class SceneBattleSyncPoisonRing : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508910;
		public UInt32 Sequence;
		public UInt32 battleID;
		public UInt32 x;
		public UInt32 y;
		public UInt32 radius;
		public UInt32 beginTimestamp;
		public UInt32 interval;
		public UInt32 x1;
		public UInt32 y1;
		public UInt32 radius1;

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
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint32(buffer, ref pos_, x);
				BaseDLL.encode_uint32(buffer, ref pos_, y);
				BaseDLL.encode_uint32(buffer, ref pos_, radius);
				BaseDLL.encode_uint32(buffer, ref pos_, beginTimestamp);
				BaseDLL.encode_uint32(buffer, ref pos_, interval);
				BaseDLL.encode_uint32(buffer, ref pos_, x1);
				BaseDLL.encode_uint32(buffer, ref pos_, y1);
				BaseDLL.encode_uint32(buffer, ref pos_, radius1);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref x);
				BaseDLL.decode_uint32(buffer, ref pos_, ref y);
				BaseDLL.decode_uint32(buffer, ref pos_, ref radius);
				BaseDLL.decode_uint32(buffer, ref pos_, ref beginTimestamp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref interval);
				BaseDLL.decode_uint32(buffer, ref pos_, ref x1);
				BaseDLL.decode_uint32(buffer, ref pos_, ref y1);
				BaseDLL.decode_uint32(buffer, ref pos_, ref radius1);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint32(buffer, ref pos_, x);
				BaseDLL.encode_uint32(buffer, ref pos_, y);
				BaseDLL.encode_uint32(buffer, ref pos_, radius);
				BaseDLL.encode_uint32(buffer, ref pos_, beginTimestamp);
				BaseDLL.encode_uint32(buffer, ref pos_, interval);
				BaseDLL.encode_uint32(buffer, ref pos_, x1);
				BaseDLL.encode_uint32(buffer, ref pos_, y1);
				BaseDLL.encode_uint32(buffer, ref pos_, radius1);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref x);
				BaseDLL.decode_uint32(buffer, ref pos_, ref y);
				BaseDLL.decode_uint32(buffer, ref pos_, ref radius);
				BaseDLL.decode_uint32(buffer, ref pos_, ref beginTimestamp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref interval);
				BaseDLL.decode_uint32(buffer, ref pos_, ref x1);
				BaseDLL.decode_uint32(buffer, ref pos_, ref y1);
				BaseDLL.decode_uint32(buffer, ref pos_, ref radius1);
			}

			public int getLen()
			{
				int _len = 0;
				// battleID
				_len += 4;
				// x
				_len += 4;
				// y
				_len += 4;
				// radius
				_len += 4;
				// beginTimestamp
				_len += 4;
				// interval
				_len += 4;
				// x1
				_len += 4;
				// y1
				_len += 4;
				// radius1
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneBattlePickUpSpoilsReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508911;
		public UInt32 Sequence;
		public UInt32 battleID;
		public UInt64 playerID;
		public UInt64 itemGuid;

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
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint64(buffer, ref pos_, playerID);
				BaseDLL.encode_uint64(buffer, ref pos_, itemGuid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerID);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemGuid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint64(buffer, ref pos_, playerID);
				BaseDLL.encode_uint64(buffer, ref pos_, itemGuid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerID);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemGuid);
			}

			public int getLen()
			{
				int _len = 0;
				// battleID
				_len += 4;
				// playerID
				_len += 8;
				// itemGuid
				_len += 8;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneBattlePickUpSpoilsRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508912;
		public UInt32 Sequence;
		public UInt32 retCode;
		public UInt64 itemGuid;

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
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint64(buffer, ref pos_, itemGuid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemGuid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint64(buffer, ref pos_, itemGuid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemGuid);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// itemGuid
				_len += 8;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneBattleSelectOccuReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508913;
		public UInt32 Sequence;
		public byte occu;

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
				BaseDLL.encode_int8(buffer, ref pos_, occu);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, occu);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
			}

			public int getLen()
			{
				int _len = 0;
				// occu
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneBattleSelectOccuRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508914;
		public UInt32 Sequence;
		public UInt32 retCode;

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
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class DetailItem : Protocol.IProtocolStream
	{
		public UInt64 guid;
		public UInt32 itemTypeId;
		public UInt32 num;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// guid
				_len += 8;
				// itemTypeId
				_len += 4;
				// num
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneBattleNotifySpoilsItem : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508915;
		public UInt32 Sequence;
		public UInt32 battleID;
		public UInt64 playerID;
		public DetailItem[] detailItems = new DetailItem[0];

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
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint64(buffer, ref pos_, playerID);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)detailItems.Length);
				for(int i = 0; i < detailItems.Length; i++)
				{
					detailItems[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerID);
				UInt16 detailItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref detailItemsCnt);
				detailItems = new DetailItem[detailItemsCnt];
				for(int i = 0; i < detailItems.Length; i++)
				{
					detailItems[i] = new DetailItem();
					detailItems[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint64(buffer, ref pos_, playerID);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)detailItems.Length);
				for(int i = 0; i < detailItems.Length; i++)
				{
					detailItems[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerID);
				UInt16 detailItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref detailItemsCnt);
				detailItems = new DetailItem[detailItemsCnt];
				for(int i = 0; i < detailItems.Length; i++)
				{
					detailItems[i] = new DetailItem();
					detailItems[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// battleID
				_len += 4;
				// playerID
				_len += 8;
				// detailItems
				_len += 2;
				for(int j = 0; j < detailItems.Length; j++)
				{
					_len += detailItems[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneBattleNotifyWaveInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508918;
		public UInt32 Sequence;
		public UInt32 battleID;
		public UInt32 waveID;

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
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint32(buffer, ref pos_, waveID);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref waveID);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint32(buffer, ref pos_, waveID);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref waveID);
			}

			public int getLen()
			{
				int _len = 0;
				// battleID
				_len += 4;
				// waveID
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneBattleThrowSomeoneItemReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508920;
		public UInt32 Sequence;
		public UInt32 battleID;
		public UInt64 targetID;
		public UInt64 itemGuid;

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
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint64(buffer, ref pos_, targetID);
				BaseDLL.encode_uint64(buffer, ref pos_, itemGuid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint64(buffer, ref pos_, ref targetID);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemGuid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint64(buffer, ref pos_, targetID);
				BaseDLL.encode_uint64(buffer, ref pos_, itemGuid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint64(buffer, ref pos_, ref targetID);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemGuid);
			}

			public int getLen()
			{
				int _len = 0;
				// battleID
				_len += 4;
				// targetID
				_len += 8;
				// itemGuid
				_len += 8;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneBattleThrowSomeoneItemRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508921;
		public UInt32 Sequence;
		public UInt32 retCode;
		public UInt64 attackID;
		public UInt64 targetID;
		public UInt64 itemGuid;
		public UInt32 itemDataID;
		public string targetName;
		public UInt32 param;

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
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint64(buffer, ref pos_, attackID);
				BaseDLL.encode_uint64(buffer, ref pos_, targetID);
				BaseDLL.encode_uint64(buffer, ref pos_, itemGuid);
				BaseDLL.encode_uint32(buffer, ref pos_, itemDataID);
				byte[] targetNameBytes = StringHelper.StringToUTF8Bytes(targetName);
				BaseDLL.encode_string(buffer, ref pos_, targetNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, param);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint64(buffer, ref pos_, ref attackID);
				BaseDLL.decode_uint64(buffer, ref pos_, ref targetID);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemGuid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemDataID);
				UInt16 targetNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref targetNameLen);
				byte[] targetNameBytes = new byte[targetNameLen];
				for(int i = 0; i < targetNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref targetNameBytes[i]);
				}
				targetName = StringHelper.BytesToString(targetNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint64(buffer, ref pos_, attackID);
				BaseDLL.encode_uint64(buffer, ref pos_, targetID);
				BaseDLL.encode_uint64(buffer, ref pos_, itemGuid);
				BaseDLL.encode_uint32(buffer, ref pos_, itemDataID);
				byte[] targetNameBytes = StringHelper.StringToUTF8Bytes(targetName);
				BaseDLL.encode_string(buffer, ref pos_, targetNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, param);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint64(buffer, ref pos_, ref attackID);
				BaseDLL.decode_uint64(buffer, ref pos_, ref targetID);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemGuid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemDataID);
				UInt16 targetNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref targetNameLen);
				byte[] targetNameBytes = new byte[targetNameLen];
				for(int i = 0; i < targetNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref targetNameBytes[i]);
				}
				targetName = StringHelper.BytesToString(targetNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// attackID
				_len += 8;
				// targetID
				_len += 8;
				// itemGuid
				_len += 8;
				// itemDataID
				_len += 4;
				// targetName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(targetName);
					_len += 2 + _strBytes.Length;
				}
				// param
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneBattleNotifySomeoneDead : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508919;
		public UInt32 Sequence;
		public UInt32 battleID;
		public UInt64 playerID;
		public UInt64 killerID;
		public UInt32 reason;
		public UInt32 kills;
		public UInt32 suvivalNum;

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
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint64(buffer, ref pos_, playerID);
				BaseDLL.encode_uint64(buffer, ref pos_, killerID);
				BaseDLL.encode_uint32(buffer, ref pos_, reason);
				BaseDLL.encode_uint32(buffer, ref pos_, kills);
				BaseDLL.encode_uint32(buffer, ref pos_, suvivalNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerID);
				BaseDLL.decode_uint64(buffer, ref pos_, ref killerID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref reason);
				BaseDLL.decode_uint32(buffer, ref pos_, ref kills);
				BaseDLL.decode_uint32(buffer, ref pos_, ref suvivalNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint64(buffer, ref pos_, playerID);
				BaseDLL.encode_uint64(buffer, ref pos_, killerID);
				BaseDLL.encode_uint32(buffer, ref pos_, reason);
				BaseDLL.encode_uint32(buffer, ref pos_, kills);
				BaseDLL.encode_uint32(buffer, ref pos_, suvivalNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerID);
				BaseDLL.decode_uint64(buffer, ref pos_, ref killerID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref reason);
				BaseDLL.decode_uint32(buffer, ref pos_, ref kills);
				BaseDLL.decode_uint32(buffer, ref pos_, ref suvivalNum);
			}

			public int getLen()
			{
				int _len = 0;
				// battleID
				_len += 4;
				// playerID
				_len += 8;
				// killerID
				_len += 8;
				// reason
				_len += 4;
				// kills
				_len += 4;
				// suvivalNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  删除物品请求
	/// </summary>
	[Protocol]
	public class SceneBattleDelItemReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508923;
		public UInt32 Sequence;
		public UInt64 itemGuid;

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
				BaseDLL.encode_uint64(buffer, ref pos_, itemGuid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemGuid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemGuid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemGuid);
			}

			public int getLen()
			{
				int _len = 0;
				// itemGuid
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  删除物品返回
	/// </summary>
	[Protocol]
	public class SceneBattleDelItemRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508924;
		public UInt32 Sequence;
		public UInt32 retCode;

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
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneBattleEnterBattleReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508925;
		public UInt32 Sequence;
		public UInt32 battleID;

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
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
			}

			public int getLen()
			{
				int _len = 0;
				// battleID
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneBattleEnterBattleRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508926;
		public UInt32 Sequence;
		public UInt32 battleID;
		public UInt32 retCode;

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
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public int getLen()
			{
				int _len = 0;
				// battleID
				_len += 4;
				// retCode
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class BattleNpc : Protocol.IProtocolStream
	{
		public UInt64 npcGuid;
		/// <summary>
		///  npcId
		/// </summary>
		public UInt32 npcId;
		/// <summary>
		///  1刷出，0清除
		/// </summary>
		public UInt32 opType;
		/// <summary>
		///  位置
		/// </summary>
		public ScenePosition pos = new ScenePosition();

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, npcGuid);
				BaseDLL.encode_uint32(buffer, ref pos_, npcId);
				BaseDLL.encode_uint32(buffer, ref pos_, opType);
				pos.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref npcGuid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref npcId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref opType);
				pos.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, npcGuid);
				BaseDLL.encode_uint32(buffer, ref pos_, npcId);
				BaseDLL.encode_uint32(buffer, ref pos_, opType);
				pos.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref npcGuid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref npcId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref opType);
				pos.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// npcGuid
				_len += 8;
				// npcId
				_len += 4;
				// opType
				_len += 4;
				// pos
				_len += pos.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  刷出NPC
	/// </summary>
	[Protocol]
	public class SceneBattleNpcNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508929;
		public UInt32 Sequence;
		public BattleNpc[] battleNpcList = new BattleNpc[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)battleNpcList.Length);
				for(int i = 0; i < battleNpcList.Length; i++)
				{
					battleNpcList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 battleNpcListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref battleNpcListCnt);
				battleNpcList = new BattleNpc[battleNpcListCnt];
				for(int i = 0; i < battleNpcList.Length; i++)
				{
					battleNpcList[i] = new BattleNpc();
					battleNpcList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)battleNpcList.Length);
				for(int i = 0; i < battleNpcList.Length; i++)
				{
					battleNpcList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 battleNpcListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref battleNpcListCnt);
				battleNpcList = new BattleNpc[battleNpcListCnt];
				for(int i = 0; i < battleNpcList.Length; i++)
				{
					battleNpcList[i] = new BattleNpc();
					battleNpcList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// battleNpcList
				_len += 2;
				for(int j = 0; j < battleNpcList.Length; j++)
				{
					_len += battleNpcList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  与NPC交易请求
	/// </summary>
	[Protocol]
	public class SceneBattleNpcTradeReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508930;
		public UInt32 Sequence;
		public UInt64 npcGuid;
		public UInt32 npcId;
		public UInt64[] paramVec = new UInt64[0];

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
				BaseDLL.encode_uint64(buffer, ref pos_, npcGuid);
				BaseDLL.encode_uint32(buffer, ref pos_, npcId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)paramVec.Length);
				for(int i = 0; i < paramVec.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, paramVec[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref npcGuid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref npcId);
				UInt16 paramVecCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref paramVecCnt);
				paramVec = new UInt64[paramVecCnt];
				for(int i = 0; i < paramVec.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref paramVec[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, npcGuid);
				BaseDLL.encode_uint32(buffer, ref pos_, npcId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)paramVec.Length);
				for(int i = 0; i < paramVec.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, paramVec[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref npcGuid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref npcId);
				UInt16 paramVecCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref paramVecCnt);
				paramVec = new UInt64[paramVecCnt];
				for(int i = 0; i < paramVec.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref paramVec[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// npcGuid
				_len += 8;
				// npcId
				_len += 4;
				// paramVec
				_len += 2 + 8 * paramVec.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  与NPC交易返回
	/// </summary>
	[Protocol]
	public class SceneBattleNpcTradeRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508931;
		public UInt32 Sequence;
		public UInt32 retCode;
		public UInt32 npcId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint32(buffer, ref pos_, npcId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref npcId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint32(buffer, ref pos_, npcId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref npcId);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// npcId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  吃鸡通知踩中陷阱
	/// </summary>
	[Protocol]
	public class SceneBattleNotifyBeTraped : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508932;
		public UInt32 Sequence;
		public UInt32 battleID;
		public UInt64 playerID;
		public UInt64 ownerID;
		public UInt32 x;
		public UInt32 y;

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
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint64(buffer, ref pos_, playerID);
				BaseDLL.encode_uint64(buffer, ref pos_, ownerID);
				BaseDLL.encode_uint32(buffer, ref pos_, x);
				BaseDLL.encode_uint32(buffer, ref pos_, y);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerID);
				BaseDLL.decode_uint64(buffer, ref pos_, ref ownerID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref x);
				BaseDLL.decode_uint32(buffer, ref pos_, ref y);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint64(buffer, ref pos_, playerID);
				BaseDLL.encode_uint64(buffer, ref pos_, ownerID);
				BaseDLL.encode_uint32(buffer, ref pos_, x);
				BaseDLL.encode_uint32(buffer, ref pos_, y);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerID);
				BaseDLL.decode_uint64(buffer, ref pos_, ref ownerID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref x);
				BaseDLL.decode_uint32(buffer, ref pos_, ref y);
			}

			public int getLen()
			{
				int _len = 0;
				// battleID
				_len += 4;
				// playerID
				_len += 8;
				// ownerID
				_len += 8;
				// x
				_len += 4;
				// y
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  吃鸡放置陷阱请求
	/// </summary>
	[Protocol]
	public class SceneBattlePlaceTrapsReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508933;
		public UInt32 Sequence;
		public UInt64 itemGuid;
		public UInt32 itemCount;
		public UInt32 x;
		public UInt32 y;

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
				BaseDLL.encode_uint64(buffer, ref pos_, itemGuid);
				BaseDLL.encode_uint32(buffer, ref pos_, itemCount);
				BaseDLL.encode_uint32(buffer, ref pos_, x);
				BaseDLL.encode_uint32(buffer, ref pos_, y);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemGuid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemCount);
				BaseDLL.decode_uint32(buffer, ref pos_, ref x);
				BaseDLL.decode_uint32(buffer, ref pos_, ref y);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemGuid);
				BaseDLL.encode_uint32(buffer, ref pos_, itemCount);
				BaseDLL.encode_uint32(buffer, ref pos_, x);
				BaseDLL.encode_uint32(buffer, ref pos_, y);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemGuid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemCount);
				BaseDLL.decode_uint32(buffer, ref pos_, ref x);
				BaseDLL.decode_uint32(buffer, ref pos_, ref y);
			}

			public int getLen()
			{
				int _len = 0;
				// itemGuid
				_len += 8;
				// itemCount
				_len += 4;
				// x
				_len += 4;
				// y
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  吃鸡放置陷阱返回
	/// </summary>
	[Protocol]
	public class SceneBattlePlaceTrapsRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508934;
		public UInt32 Sequence;
		public UInt32 retCode;

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
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  吃鸡开宝箱请求
	/// </summary>
	[Protocol]
	public class SceneBattleOpenBoxReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508935;
		public UInt32 Sequence;
		public UInt64 itemGuid;
		/// <summary>
		///  参数 1：打开；2：取消；3：拾取
		/// </summary>
		public UInt32 param;

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
				BaseDLL.encode_uint64(buffer, ref pos_, itemGuid);
				BaseDLL.encode_uint32(buffer, ref pos_, param);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemGuid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemGuid);
				BaseDLL.encode_uint32(buffer, ref pos_, param);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemGuid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param);
			}

			public int getLen()
			{
				int _len = 0;
				// itemGuid
				_len += 8;
				// param
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  吃鸡开宝箱返回
	/// </summary>
	[Protocol]
	public class SceneBattleOpenBoxRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508936;
		public UInt32 Sequence;
		public UInt64 itemGuid;
		/// <summary>
		///  参数 1：打开；2：取消；3：拾取
		/// </summary>
		public UInt32 param;
		public UInt32 retCode;
		public UInt32 openTime;

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
				BaseDLL.encode_uint64(buffer, ref pos_, itemGuid);
				BaseDLL.encode_uint32(buffer, ref pos_, param);
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint32(buffer, ref pos_, openTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemGuid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param);
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref openTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemGuid);
				BaseDLL.encode_uint32(buffer, ref pos_, param);
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint32(buffer, ref pos_, openTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemGuid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param);
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref openTime);
			}

			public int getLen()
			{
				int _len = 0;
				// itemGuid
				_len += 8;
				// param
				_len += 4;
				// retCode
				_len += 4;
				// openTime
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  最佳排名通知
	/// </summary>
	[Protocol]
	public class BattleNotifyBestRank : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508937;
		public UInt32 Sequence;
		public UInt32 rank;

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
				BaseDLL.encode_uint32(buffer, ref pos_, rank);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref rank);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, rank);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref rank);
			}

			public int getLen()
			{
				int _len = 0;
				// rank
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class ChiJiSkill : Protocol.IProtocolStream
	{
		public UInt32 skillId;
		public UInt32 skillLvl;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, skillId);
				BaseDLL.encode_uint32(buffer, ref pos_, skillLvl);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref skillId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref skillLvl);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, skillId);
				BaseDLL.encode_uint32(buffer, ref pos_, skillLvl);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref skillId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref skillLvl);
			}

			public int getLen()
			{
				int _len = 0;
				// skillId
				_len += 4;
				// skillLvl
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  下发技能列表
	/// </summary>
	[Protocol]
	public class BattleSkillChoiceListNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508939;
		public UInt32 Sequence;
		public ChiJiSkill[] skillList = new ChiJiSkill[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)skillList.Length);
				for(int i = 0; i < skillList.Length; i++)
				{
					skillList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 skillListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref skillListCnt);
				skillList = new ChiJiSkill[skillListCnt];
				for(int i = 0; i < skillList.Length; i++)
				{
					skillList[i] = new ChiJiSkill();
					skillList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)skillList.Length);
				for(int i = 0; i < skillList.Length; i++)
				{
					skillList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 skillListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref skillListCnt);
				skillList = new ChiJiSkill[skillListCnt];
				for(int i = 0; i < skillList.Length; i++)
				{
					skillList[i] = new ChiJiSkill();
					skillList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// skillList
				_len += 2;
				for(int j = 0; j < skillList.Length; j++)
				{
					_len += skillList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  选择技能请求
	/// </summary>
	[Protocol]
	public class BattleChoiceSkillReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508940;
		public UInt32 Sequence;
		public UInt32 skillId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, skillId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref skillId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, skillId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref skillId);
			}

			public int getLen()
			{
				int _len = 0;
				// skillId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  选择技能返回
	/// </summary>
	[Protocol]
	public class BattleChoiceSkillRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508941;
		public UInt32 Sequence;
		public UInt32 retCode;
		public UInt32 skillId;
		public UInt32 skillLvl;

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
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint32(buffer, ref pos_, skillId);
				BaseDLL.encode_uint32(buffer, ref pos_, skillLvl);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref skillId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref skillLvl);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint32(buffer, ref pos_, skillId);
				BaseDLL.encode_uint32(buffer, ref pos_, skillLvl);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref skillId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref skillLvl);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// skillId
				_len += 4;
				// skillLvl
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  免战选项
	/// </summary>
	[Protocol]
	public class SceneBattleNoWarOption : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508942;
		public UInt32 Sequence;
		public UInt64 enemyRoleId;
		public string enemyName;

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
				BaseDLL.encode_uint64(buffer, ref pos_, enemyRoleId);
				byte[] enemyNameBytes = StringHelper.StringToUTF8Bytes(enemyName);
				BaseDLL.encode_string(buffer, ref pos_, enemyNameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref enemyRoleId);
				UInt16 enemyNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref enemyNameLen);
				byte[] enemyNameBytes = new byte[enemyNameLen];
				for(int i = 0; i < enemyNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref enemyNameBytes[i]);
				}
				enemyName = StringHelper.BytesToString(enemyNameBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, enemyRoleId);
				byte[] enemyNameBytes = StringHelper.StringToUTF8Bytes(enemyName);
				BaseDLL.encode_string(buffer, ref pos_, enemyNameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref enemyRoleId);
				UInt16 enemyNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref enemyNameLen);
				byte[] enemyNameBytes = new byte[enemyNameLen];
				for(int i = 0; i < enemyNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref enemyNameBytes[i]);
				}
				enemyName = StringHelper.BytesToString(enemyNameBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// enemyRoleId
				_len += 8;
				// enemyName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(enemyName);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  免战选择请求
	/// </summary>
	[Protocol]
	public class SceneBattleNoWarChoiceReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508943;
		public UInt32 Sequence;
		public UInt32 isNoWar;

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
				BaseDLL.encode_uint32(buffer, ref pos_, isNoWar);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref isNoWar);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, isNoWar);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref isNoWar);
			}

			public int getLen()
			{
				int _len = 0;
				// isNoWar
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  免战选择返回
	/// </summary>
	[Protocol]
	public class SceneBattleNoWarChoiceRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508944;
		public UInt32 Sequence;
		public UInt32 retCode;

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
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  免战通知
	/// </summary>
	[Protocol]
	public class SceneBattleNoWarNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508945;
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
	///  免战等待通知
	/// </summary>
	[Protocol]
	public class SceneBattleNoWarWait : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508946;
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
	///  职业列表请求
	/// </summary>
	[Protocol]
	public class SceneBattleOccuListReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508947;
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
	///  职业列表返回
	/// </summary>
	[Protocol]
	public class SceneBattleOccuListRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508948;
		public UInt32 Sequence;
		public UInt32[] occuList = new UInt32[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)occuList.Length);
				for(int i = 0; i < occuList.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, occuList[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 occuListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref occuListCnt);
				occuList = new UInt32[occuListCnt];
				for(int i = 0; i < occuList.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref occuList[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)occuList.Length);
				for(int i = 0; i < occuList.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, occuList[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 occuListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref occuListCnt);
				occuList = new UInt32[occuListCnt];
				for(int i = 0; i < occuList.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref occuList[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// occuList
				_len += 2 + 4 * occuList.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  通知吃鸡杂项数据
	/// </summary>
	[Protocol]
	public class BattleNotifyChijiMisc : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 2200022;
		public UInt32 Sequence;
		/// <summary>
		///  移动包发送间隔(ms)
		/// </summary>
		public UInt32 moveIntervalMs;

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
				BaseDLL.encode_uint32(buffer, ref pos_, moveIntervalMs);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref moveIntervalMs);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, moveIntervalMs);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref moveIntervalMs);
			}

			public int getLen()
			{
				int _len = 0;
				// moveIntervalMs
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  下发装备选择列表
	/// </summary>
	[Protocol]
	public class BattleEquipChoiceListNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508949;
		public UInt32 Sequence;
		public UInt32[] equipList = new UInt32[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)equipList.Length);
				for(int i = 0; i < equipList.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, equipList[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 equipListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref equipListCnt);
				equipList = new UInt32[equipListCnt];
				for(int i = 0; i < equipList.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref equipList[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)equipList.Length);
				for(int i = 0; i < equipList.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, equipList[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 equipListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref equipListCnt);
				equipList = new UInt32[equipListCnt];
				for(int i = 0; i < equipList.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref equipList[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// equipList
				_len += 2 + 4 * equipList.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  选择装备请求
	/// </summary>
	[Protocol]
	public class BattleChoiceEquipReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508950;
		public UInt32 Sequence;
		public UInt32 equipId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, equipId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref equipId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, equipId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref equipId);
			}

			public int getLen()
			{
				int _len = 0;
				// equipId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  选择装备返回
	/// </summary>
	[Protocol]
	public class BattleChoiceEquipRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508951;
		public UInt32 Sequence;
		public UInt32 retCode;
		public UInt32 equipId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint32(buffer, ref pos_, equipId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref equipId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint32(buffer, ref pos_, equipId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref equipId);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// equipId
				_len += 4;
				return _len;
			}
		#endregion

	}

}
