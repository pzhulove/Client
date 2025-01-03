using System;
using System.Text;

namespace Protocol
{
	/// <summary>
	///  录像评价
	/// </summary>
	public enum ReplayEvaluation
	{
		Invalid = 0,
	}

	/// <summary>
	///  录像列表类型
	/// </summary>
	public enum ReplayListType
	{
		/// <summary>
		///  无效值
		/// </summary>
		Invalid = 0,
		/// <summary>
		///  自己对战记录
		/// </summary>
		Self = 1,
		/// <summary>
		///  高手对决
		/// </summary>
		Master = 2,
		/// <summary>
		///  收藏
		/// </summary>
		Collection = 3,
	}

	/// <summary>
	///  PK结果
	/// </summary>
	public enum PkRaceResult
	{
		/// <summary>
		///  无效
		/// </summary>
		Invalid = 0,
		/// <summary>
		///  胜利
		/// </summary>
		Win = 1,
		/// <summary>
		///  失败
		/// </summary>
		Lose = 2,
		/// <summary>
		///  平局
		/// </summary>
		Dogfall = 3,
	}

	/// <summary>
	///  录像玩家信息
	/// </summary>
	public class ReplayFighterInfo : Protocol.IProtocolStream
	{
		public UInt64 roleId;
		public string name;
		public byte occu;
		public UInt16 level;
		public byte pos;
		/// <summary>
		///  赛季段位
		/// </summary>
		public UInt32 seasonLevel;
		/// <summary>
		///  赛季星星
		/// </summary>
		public byte seasonStars;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, pos);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonLevel);
				BaseDLL.encode_int8(buffer, ref pos_, seasonStars);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
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
				BaseDLL.decode_int8(buffer, ref pos_, ref pos);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLevel);
				BaseDLL.decode_int8(buffer, ref pos_, ref seasonStars);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, pos);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonLevel);
				BaseDLL.encode_int8(buffer, ref pos_, seasonStars);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
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
				BaseDLL.decode_int8(buffer, ref pos_, ref pos);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLevel);
				BaseDLL.decode_int8(buffer, ref pos_, ref seasonStars);
			}

			public int getLen()
			{
				int _len = 0;
				// roleId
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
				// pos
				_len += 1;
				// seasonLevel
				_len += 4;
				// seasonStars
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  录像信息
	/// </summary>
	public class ReplayInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  版本号
		/// </summary>
		public UInt32 version;
		/// <summary>
		///  比赛ID
		/// </summary>
		public UInt64 raceId;
		/// <summary>
		///  pk类型（对应枚举PkType）
		/// </summary>
		public byte type;
		/// <summary>
		///  评价(对应枚举ReplayEvaluation)
		/// </summary>
		public byte evaluation;
		/// <summary>
		///  记录时间
		/// </summary>
		public UInt32 recordTime;
		/// <summary>
		///  战斗结果（第一个人的结果,对应枚举PkRaceResult）
		/// </summary>
		public byte result;
		/// <summary>
		///  战斗信息
		/// </summary>
		public ReplayFighterInfo[] fighters = new ReplayFighterInfo[0];
		/// <summary>
		///  观看次数
		/// </summary>
		public UInt32 viewNum;
		/// <summary>
		///  分数
		/// </summary>
		public UInt32 score;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, version);
				BaseDLL.encode_uint64(buffer, ref pos_, raceId);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, evaluation);
				BaseDLL.encode_uint32(buffer, ref pos_, recordTime);
				BaseDLL.encode_int8(buffer, ref pos_, result);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)fighters.Length);
				for(int i = 0; i < fighters.Length; i++)
				{
					fighters[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, viewNum);
				BaseDLL.encode_uint32(buffer, ref pos_, score);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref version);
				BaseDLL.decode_uint64(buffer, ref pos_, ref raceId);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref evaluation);
				BaseDLL.decode_uint32(buffer, ref pos_, ref recordTime);
				BaseDLL.decode_int8(buffer, ref pos_, ref result);
				UInt16 fightersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref fightersCnt);
				fighters = new ReplayFighterInfo[fightersCnt];
				for(int i = 0; i < fighters.Length; i++)
				{
					fighters[i] = new ReplayFighterInfo();
					fighters[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref viewNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref score);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, version);
				BaseDLL.encode_uint64(buffer, ref pos_, raceId);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, evaluation);
				BaseDLL.encode_uint32(buffer, ref pos_, recordTime);
				BaseDLL.encode_int8(buffer, ref pos_, result);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)fighters.Length);
				for(int i = 0; i < fighters.Length; i++)
				{
					fighters[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, viewNum);
				BaseDLL.encode_uint32(buffer, ref pos_, score);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref version);
				BaseDLL.decode_uint64(buffer, ref pos_, ref raceId);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref evaluation);
				BaseDLL.decode_uint32(buffer, ref pos_, ref recordTime);
				BaseDLL.decode_int8(buffer, ref pos_, ref result);
				UInt16 fightersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref fightersCnt);
				fighters = new ReplayFighterInfo[fightersCnt];
				for(int i = 0; i < fighters.Length; i++)
				{
					fighters[i] = new ReplayFighterInfo();
					fighters[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref viewNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref score);
			}

			public int getLen()
			{
				int _len = 0;
				// version
				_len += 4;
				// raceId
				_len += 8;
				// type
				_len += 1;
				// evaluation
				_len += 1;
				// recordTime
				_len += 4;
				// result
				_len += 1;
				// fighters
				_len += 2;
				for(int j = 0; j < fighters.Length; j++)
				{
					_len += fighters[j].getLen();
				}
				// viewNum
				_len += 4;
				// score
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求对战记录
	/// </summary>
	[Protocol]
	public class SceneReplayListReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507501;
		public UInt32 Sequence;
		/// <summary>
		///  录像列表类型（对应枚举ReplayListType）
		/// </summary>
		public byte type;

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
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  返回对战记录
	/// </summary>
	[Protocol]
	public class SceneReplayListRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507502;
		public UInt32 Sequence;
		/// <summary>
		///  录像列表类型（对应枚举ReplayListType）
		/// </summary>
		public byte type;
		/// <summary>
		///  所有录像简介
		/// </summary>
		public ReplayInfo[] replays = new ReplayInfo[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)replays.Length);
				for(int i = 0; i < replays.Length; i++)
				{
					replays[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				UInt16 replaysCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref replaysCnt);
				replays = new ReplayInfo[replaysCnt];
				for(int i = 0; i < replays.Length; i++)
				{
					replays[i] = new ReplayInfo();
					replays[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)replays.Length);
				for(int i = 0; i < replays.Length; i++)
				{
					replays[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				UInt16 replaysCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref replaysCnt);
				replays = new ReplayInfo[replaysCnt];
				for(int i = 0; i < replays.Length; i++)
				{
					replays[i] = new ReplayInfo();
					replays[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// replays
				_len += 2;
				for(int j = 0; j < replays.Length; j++)
				{
					_len += replays[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  看了一场录像
	/// </summary>
	[Protocol]
	public class SceneReplayView : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507503;
		public UInt32 Sequence;
		public UInt64 raceid;

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
				BaseDLL.encode_uint64(buffer, ref pos_, raceid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref raceid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, raceid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref raceid);
			}

			public int getLen()
			{
				int _len = 0;
				// raceid
				_len += 8;
				return _len;
			}
		#endregion

	}

	public class ReplayHeader : Protocol.IProtocolStream
	{
		public UInt32 magicCode;
		public UInt32 version;
		public UInt32 startTime;
		public UInt64 sessionId;
		public byte raceType;
		public UInt32 pkDungeonId;
		public UInt32 duration;
		public RacePlayerInfo[] players = new RacePlayerInfo[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, magicCode);
				BaseDLL.encode_uint32(buffer, ref pos_, version);
				BaseDLL.encode_uint32(buffer, ref pos_, startTime);
				BaseDLL.encode_uint64(buffer, ref pos_, sessionId);
				BaseDLL.encode_int8(buffer, ref pos_, raceType);
				BaseDLL.encode_uint32(buffer, ref pos_, pkDungeonId);
				BaseDLL.encode_uint32(buffer, ref pos_, duration);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)players.Length);
				for(int i = 0; i < players.Length; i++)
				{
					players[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref magicCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref version);
				BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
				BaseDLL.decode_uint64(buffer, ref pos_, ref sessionId);
				BaseDLL.decode_int8(buffer, ref pos_, ref raceType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref pkDungeonId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref duration);
				UInt16 playersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playersCnt);
				players = new RacePlayerInfo[playersCnt];
				for(int i = 0; i < players.Length; i++)
				{
					players[i] = new RacePlayerInfo();
					players[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, magicCode);
				BaseDLL.encode_uint32(buffer, ref pos_, version);
				BaseDLL.encode_uint32(buffer, ref pos_, startTime);
				BaseDLL.encode_uint64(buffer, ref pos_, sessionId);
				BaseDLL.encode_int8(buffer, ref pos_, raceType);
				BaseDLL.encode_uint32(buffer, ref pos_, pkDungeonId);
				BaseDLL.encode_uint32(buffer, ref pos_, duration);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)players.Length);
				for(int i = 0; i < players.Length; i++)
				{
					players[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref magicCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref version);
				BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
				BaseDLL.decode_uint64(buffer, ref pos_, ref sessionId);
				BaseDLL.decode_int8(buffer, ref pos_, ref raceType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref pkDungeonId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref duration);
				UInt16 playersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playersCnt);
				players = new RacePlayerInfo[playersCnt];
				for(int i = 0; i < players.Length; i++)
				{
					players[i] = new RacePlayerInfo();
					players[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// magicCode
				_len += 4;
				// version
				_len += 4;
				// startTime
				_len += 4;
				// sessionId
				_len += 8;
				// raceType
				_len += 1;
				// pkDungeonId
				_len += 4;
				// duration
				_len += 4;
				// players
				_len += 2;
				for(int j = 0; j < players.Length; j++)
				{
					_len += players[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	public class ReplayRaceResult : Protocol.IProtocolStream
	{
		public byte pos;
		public byte result;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, pos);
				BaseDLL.encode_int8(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref pos);
				BaseDLL.decode_int8(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, pos);
				BaseDLL.encode_int8(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref pos);
				BaseDLL.decode_int8(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// pos
				_len += 1;
				// result
				_len += 1;
				return _len;
			}
		#endregion

	}

	public class ReplayFile : Protocol.IProtocolStream
	{
		public ReplayHeader header = new ReplayHeader();
		public Frame[] frames = new Frame[0];
		public ReplayRaceResult[] results = new ReplayRaceResult[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				header.encode(buffer, ref pos_);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)frames.Length);
				for(int i = 0; i < frames.Length; i++)
				{
					frames[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)results.Length);
				for(int i = 0; i < results.Length; i++)
				{
					results[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				header.decode(buffer, ref pos_);
				UInt16 framesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref framesCnt);
				frames = new Frame[framesCnt];
				for(int i = 0; i < frames.Length; i++)
				{
					frames[i] = new Frame();
					frames[i].decode(buffer, ref pos_);
				}
				UInt16 resultsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref resultsCnt);
				results = new ReplayRaceResult[resultsCnt];
				for(int i = 0; i < results.Length; i++)
				{
					results[i] = new ReplayRaceResult();
					results[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				header.encode(buffer, ref pos_);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)frames.Length);
				for(int i = 0; i < frames.Length; i++)
				{
					frames[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)results.Length);
				for(int i = 0; i < results.Length; i++)
				{
					results[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				header.decode(buffer, ref pos_);
				UInt16 framesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref framesCnt);
				frames = new Frame[framesCnt];
				for(int i = 0; i < frames.Length; i++)
				{
					frames[i] = new Frame();
					frames[i].decode(buffer, ref pos_);
				}
				UInt16 resultsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref resultsCnt);
				results = new ReplayRaceResult[resultsCnt];
				for(int i = 0; i < results.Length; i++)
				{
					results[i] = new ReplayRaceResult();
					results[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// header
				_len += header.getLen();
				// frames
				_len += 2;
				for(int j = 0; j < frames.Length; j++)
				{
					_len += frames[j].getLen();
				}
				// results
				_len += 2;
				for(int j = 0; j < results.Length; j++)
				{
					_len += results[j].getLen();
				}
				return _len;
			}
		#endregion

	}

}
