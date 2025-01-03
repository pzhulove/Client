using System;
using System.Text;

namespace Protocol
{
	public enum TeamCopyTeamModel
	{
		/// <summary>
		///  默认全部模式
		/// </summary>
		TEAM_COPY_TEAM_MODEL_DEFAULT = 0,
		/// <summary>
		///  挑战模式
		/// </summary>
		TEAM_COPY_TEAM_MODEL_CHALLENGE = 1,
		/// <summary>
		///  金团模式
		/// </summary>
		TEAM_COPY_TEAM_MODEL_GOLD = 2,
	}

	/// <summary>
	///  据点状态
	/// </summary>
	public enum TeamCopyFieldStatus
	{
		/// <summary>
		///  无效状态
		/// </summary>
		TEAM_COPY_FIELD_STATUS_INVALID = 0,
		/// <summary>
		///  初始状态
		/// </summary>
		TEAM_COPY_FIELD_STATUS_INIT = 1,
		/// <summary>
		///  重生
		/// </summary>
		TEAM_COPY_FIELD_STATUS_REBORN = 2,
		/// <summary>
		///  歼灭
		/// </summary>
		TEAM_COPY_FIELD_STATUS_DEFEAT = 3,
		/// <summary>
		///  紧急
		/// </summary>
		TEAM_COPY_FIELD_STATUS_URGENT = 4,
		/// <summary>
		///  解锁中
		/// </summary>
		TEAM_COPY_FIELD_STATUS_UNLOCKING = 5,
		/// <summary>
		///  能量恢复中
		/// </summary>
		TEAM_COPY_FIELD_STATUS_ENERGY_REVIVE = 6,
	}

	/// <summary>
	///  战前配置模式
	/// </summary>
	public enum TeamCopyPlanModel
	{
		/// <summary>
		///  无效
		/// </summary>
		TEAM_COPY_PLAN_MODEL_INVALID = 0,
		/// <summary>
		///  自由模式
		/// </summary>
		TEAM_COPY_PLAN_MODEL_FREE = 1,
		/// <summary>
		///  引导模式
		/// </summary>
		TEAM_COPY_PLAN_MODEL_GUIDE = 2,
	}

	/// <summary>
	///  小队难度
	/// </summary>
	public enum TeamCopyGrade
	{
		/// <summary>
		///  团队难度
		/// </summary>
		TEAM_COPY_GRADE_TEAM = 0,
		/// <summary>
		///  A难度
		/// </summary>
		TEAM_COPY_GRADE_A = 1,
		/// <summary>
		///  B难度
		/// </summary>
		TEAM_COPY_GRADE_B = 2,
		/// <summary>
		///  C难度
		/// </summary>
		TEAM_COPY_GRADE_C = 3,
		/// <summary>
		///  D难度
		/// </summary>
		TEAM_COPY_GRADE_D = 4,
	}

	/// <summary>
	///  阶段
	/// </summary>
	public enum TeamCopyStage
	{
		/// <summary>
		///  空阶段
		/// </summary>
		TEAM_COPY_STAGE_NULL = 0,
		/// <summary>
		///  第一阶段
		/// </summary>
		TEAM_COPY_STAGE_ONE = 1,
		/// <summary>
		///  第二阶段
		/// </summary>
		TEAM_COPY_STAGE_TWO = 2,
		/// <summary>
		///  最终阶段
		/// </summary>
		TEAM_COPY_STAGE_FINAL = 3,
	}

	/// <summary>
	///  目标
	/// </summary>
	public enum TeamCopyTargetType
	{
		/// <summary>
		///  团队目标
		/// </summary>
		TEAM_COPY_TARGET_TYPE_TEAM = 1,
		/// <summary>
		///  小队目标
		/// </summary>
		TEAM_COPY_TARGET_TYPE_SQUAD = 2,
	}

	/// <summary>
	///  队伍状态
	/// </summary>
	public enum TeamCopyTeamStatus
	{
		/// <summary>
		///  备战
		/// </summary>
		TEAM_COPY_TEAM_STATUS_PARPARE = 0,
		/// <summary>
		///  战斗
		/// </summary>
		TEAM_COPY_TEAM_STATUS_BATTLE = 1,
		/// <summary>
		///  胜利
		/// </summary>
		TEAM_COPY_TEAM_STATUS_VICTORY = 2,
		/// <summary>
		///  失败
		/// </summary>
		TEAM_COPY_TEAM_STATUS_FAILED = 3,
	}

	/// <summary>
	///  小队状态
	/// </summary>
	public enum TeamCopySquadStatus
	{
		/// <summary>
		///  待命中
		/// </summary>
		TEAM_COPY_SQUAD_STATUS_INIT = 0,
		/// <summary>
		///  备战中
		/// </summary>
		TEAM_COPY_SQUAD_STATUS_PREPARE = 1,
		/// <summary>
		///  战斗中
		/// </summary>
		TEAM_COPY_SQUAD_STATUS_BATTLE = 2,
	}

	/// <summary>
	///  职位(按位操作)
	/// </summary>
	public enum TeamCopyPost
	{
		/// <summary>
		///  队员
		/// </summary>
		TEAM_COPY_POST_NORMAL = 1,
		/// <summary>
		///  金主
		/// </summary>
		TEAM_COPY_POST_GOLD = 2,
		/// <summary>
		///  队长
		/// </summary>
		TEAM_COPY_POST_CAPTAIN = 4,
		/// <summary>
		///  团长
		/// </summary>
		TEAM_COPY_POST_CHIEF = 8,
	}

	/// <summary>
	///  翻牌限制类型
	/// </summary>
	public enum TeamCopyFlopLimit
	{
		/// <summary>
		///  不限制
		/// </summary>
		TEAM_COPY_FLOP_LIMIT_NULL = 0,
		/// <summary>
		///  日限制
		/// </summary>
		TEAM_COPY_FLOP_LIMIT_DAY = 1,
		/// <summary>
		///  周限制
		/// </summary>
		TEAM_COPY_FLOP_LIMIT_WEEK = 2,
		/// <summary>
		///  通关限制
		/// </summary>
		TEAM_COPY_FLOP_LIMIT_PASS_GATE = 3,
	}

	/// <summary>
	///  难度
	/// </summary>
	public enum TeamCopyTeamGrade
	{
		/// <summary>
		///  普通难度
		/// </summary>
		TEAM_COPY_TEAM_GRADE_COMMON = 1,
		/// <summary>
		///  困难难度
		/// </summary>
		TEAM_COPY_TEAM_GRADE_DIFF = 2,
	}

	/// <summary>
	///  创建团队请求
	/// </summary>
	[Protocol]
	public class TeamCopyCreateTeamReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100003;
		public UInt32 Sequence;
		/// <summary>
		///  难度
		/// </summary>
		public UInt32 teamGrade;
		/// <summary>
		///  模式
		/// </summary>
		public UInt32 teamModel;
		/// <summary>
		///  装备评分
		/// </summary>
		public UInt32 equipScore;
		/// <summary>
		///  param(佣金)
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
				BaseDLL.encode_uint32(buffer, ref pos_, teamGrade);
				BaseDLL.encode_uint32(buffer, ref pos_, teamModel);
				BaseDLL.encode_uint32(buffer, ref pos_, equipScore);
				BaseDLL.encode_uint32(buffer, ref pos_, param);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamGrade);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamModel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref equipScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, teamGrade);
				BaseDLL.encode_uint32(buffer, ref pos_, teamModel);
				BaseDLL.encode_uint32(buffer, ref pos_, equipScore);
				BaseDLL.encode_uint32(buffer, ref pos_, param);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamGrade);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamModel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref equipScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param);
			}

			public int getLen()
			{
				int _len = 0;
				// teamGrade
				_len += 4;
				// teamModel
				_len += 4;
				// equipScore
				_len += 4;
				// param
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  创建团队返回
	/// </summary>
	[Protocol]
	public class TeamCopyCreateTeamRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100004;
		public UInt32 Sequence;
		/// <summary>
		///  团队Id
		/// </summary>
		public UInt32 teamId;
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
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public int getLen()
			{
				int _len = 0;
				// teamId
				_len += 4;
				// retCode
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  团队数据请求
	/// </summary>
	[Protocol]
	public class TeamCopyTeamDataReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100005;
		public UInt32 Sequence;
		/// <summary>
		///  团队Id
		/// </summary>
		public UInt32 teamId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
			}

			public int getLen()
			{
				int _len = 0;
				// teamId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  团本成员信息
	/// </summary>
	public class TeamCopyMember : Protocol.IProtocolStream
	{
		/// <summary>
		///  玩家ID
		/// </summary>
		public UInt64 playerId;
		/// <summary>
		///  玩家名字
		/// </summary>
		public string playerName;
		/// <summary>
		///  玩家职业
		/// </summary>
		public UInt32 playerOccu;
		/// <summary>
		///  觉醒
		/// </summary>
		public UInt32 playerAwaken;
		/// <summary>
		///  玩家等级
		/// </summary>
		public UInt32 playerLvl;
		/// <summary>
		///  职位
		/// </summary>
		public UInt32 post;
		/// <summary>
		///  装备评分
		/// </summary>
		public UInt32 equipScore;
		/// <summary>
		///  座位
		/// </summary>
		public UInt32 seatId;
		/// <summary>
		///  门票是否足够
		/// </summary>
		public UInt32 ticketIsEnough;
		/// <summary>
		///  zone
		/// </summary>
		public UInt32 zoneId;
		/// <summary>
		///  过期时间
		/// </summary>
		public UInt64 expireTime;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
				BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, playerOccu);
				BaseDLL.encode_uint32(buffer, ref pos_, playerAwaken);
				BaseDLL.encode_uint32(buffer, ref pos_, playerLvl);
				BaseDLL.encode_uint32(buffer, ref pos_, post);
				BaseDLL.encode_uint32(buffer, ref pos_, equipScore);
				BaseDLL.encode_uint32(buffer, ref pos_, seatId);
				BaseDLL.encode_uint32(buffer, ref pos_, ticketIsEnough);
				BaseDLL.encode_uint32(buffer, ref pos_, zoneId);
				BaseDLL.encode_uint64(buffer, ref pos_, expireTime);
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
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerOccu);
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerAwaken);
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerLvl);
				BaseDLL.decode_uint32(buffer, ref pos_, ref post);
				BaseDLL.decode_uint32(buffer, ref pos_, ref equipScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seatId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref ticketIsEnough);
				BaseDLL.decode_uint32(buffer, ref pos_, ref zoneId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref expireTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
				BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, playerOccu);
				BaseDLL.encode_uint32(buffer, ref pos_, playerAwaken);
				BaseDLL.encode_uint32(buffer, ref pos_, playerLvl);
				BaseDLL.encode_uint32(buffer, ref pos_, post);
				BaseDLL.encode_uint32(buffer, ref pos_, equipScore);
				BaseDLL.encode_uint32(buffer, ref pos_, seatId);
				BaseDLL.encode_uint32(buffer, ref pos_, ticketIsEnough);
				BaseDLL.encode_uint32(buffer, ref pos_, zoneId);
				BaseDLL.encode_uint64(buffer, ref pos_, expireTime);
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
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerOccu);
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerAwaken);
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerLvl);
				BaseDLL.decode_uint32(buffer, ref pos_, ref post);
				BaseDLL.decode_uint32(buffer, ref pos_, ref equipScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seatId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref ticketIsEnough);
				BaseDLL.decode_uint32(buffer, ref pos_, ref zoneId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref expireTime);
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
				// playerOccu
				_len += 4;
				// playerAwaken
				_len += 4;
				// playerLvl
				_len += 4;
				// post
				_len += 4;
				// equipScore
				_len += 4;
				// seatId
				_len += 4;
				// ticketIsEnough
				_len += 4;
				// zoneId
				_len += 4;
				// expireTime
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  小队数据
	/// </summary>
	public class SquadData : Protocol.IProtocolStream
	{
		/// <summary>
		///  小队ID
		/// </summary>
		public UInt32 squadId;
		/// <summary>
		///  小队状态
		/// </summary>
		public UInt32 squadStatus;
		/// <summary>
		///  成员列表
		/// </summary>
		public TeamCopyMember[] teamMemberList = new TeamCopyMember[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, squadId);
				BaseDLL.encode_uint32(buffer, ref pos_, squadStatus);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)teamMemberList.Length);
				for(int i = 0; i < teamMemberList.Length; i++)
				{
					teamMemberList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref squadId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref squadStatus);
				UInt16 teamMemberListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref teamMemberListCnt);
				teamMemberList = new TeamCopyMember[teamMemberListCnt];
				for(int i = 0; i < teamMemberList.Length; i++)
				{
					teamMemberList[i] = new TeamCopyMember();
					teamMemberList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, squadId);
				BaseDLL.encode_uint32(buffer, ref pos_, squadStatus);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)teamMemberList.Length);
				for(int i = 0; i < teamMemberList.Length; i++)
				{
					teamMemberList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref squadId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref squadStatus);
				UInt16 teamMemberListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref teamMemberListCnt);
				teamMemberList = new TeamCopyMember[teamMemberListCnt];
				for(int i = 0; i < teamMemberList.Length; i++)
				{
					teamMemberList[i] = new TeamCopyMember();
					teamMemberList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// squadId
				_len += 4;
				// squadStatus
				_len += 4;
				// teamMemberList
				_len += 2;
				for(int j = 0; j < teamMemberList.Length; j++)
				{
					_len += teamMemberList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  团队数据返回
	/// </summary>
	[Protocol]
	public class TeamCopyTeamDataRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100006;
		public UInt32 Sequence;
		/// <summary>
		///  队伍ID
		/// </summary>
		public UInt32 teamId;
		/// <summary>
		///  装备评分
		/// </summary>
		public UInt32 equipScore;
		/// <summary>
		///  状态
		/// </summary>
		public UInt32 status;
		/// <summary>
		///  团队名字
		/// </summary>
		public string teamName;
		/// <summary>
		///  佣金总数
		/// </summary>
		public UInt32 totalCommission;
		/// <summary>
		///  分成佣金
		/// </summary>
		public UInt32 bonusCommission;
		/// <summary>
		///  自动同意金主入团(0：不同意，1：同意)
		/// </summary>
		public UInt32 autoAgreeGold;
		/// <summary>
		///  团本模式
		/// </summary>
		public UInt32 teamModel;
		/// <summary>
		///  难度
		/// </summary>
		public UInt32 teamGrade;
		/// <summary>
		///  小队列表
		/// </summary>
		public SquadData[] squadList = new SquadData[0];

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
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
				BaseDLL.encode_uint32(buffer, ref pos_, equipScore);
				BaseDLL.encode_uint32(buffer, ref pos_, status);
				byte[] teamNameBytes = StringHelper.StringToUTF8Bytes(teamName);
				BaseDLL.encode_string(buffer, ref pos_, teamNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, totalCommission);
				BaseDLL.encode_uint32(buffer, ref pos_, bonusCommission);
				BaseDLL.encode_uint32(buffer, ref pos_, autoAgreeGold);
				BaseDLL.encode_uint32(buffer, ref pos_, teamModel);
				BaseDLL.encode_uint32(buffer, ref pos_, teamGrade);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)squadList.Length);
				for(int i = 0; i < squadList.Length; i++)
				{
					squadList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref equipScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref status);
				UInt16 teamNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref teamNameLen);
				byte[] teamNameBytes = new byte[teamNameLen];
				for(int i = 0; i < teamNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref teamNameBytes[i]);
				}
				teamName = StringHelper.BytesToString(teamNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalCommission);
				BaseDLL.decode_uint32(buffer, ref pos_, ref bonusCommission);
				BaseDLL.decode_uint32(buffer, ref pos_, ref autoAgreeGold);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamModel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamGrade);
				UInt16 squadListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref squadListCnt);
				squadList = new SquadData[squadListCnt];
				for(int i = 0; i < squadList.Length; i++)
				{
					squadList[i] = new SquadData();
					squadList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
				BaseDLL.encode_uint32(buffer, ref pos_, equipScore);
				BaseDLL.encode_uint32(buffer, ref pos_, status);
				byte[] teamNameBytes = StringHelper.StringToUTF8Bytes(teamName);
				BaseDLL.encode_string(buffer, ref pos_, teamNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, totalCommission);
				BaseDLL.encode_uint32(buffer, ref pos_, bonusCommission);
				BaseDLL.encode_uint32(buffer, ref pos_, autoAgreeGold);
				BaseDLL.encode_uint32(buffer, ref pos_, teamModel);
				BaseDLL.encode_uint32(buffer, ref pos_, teamGrade);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)squadList.Length);
				for(int i = 0; i < squadList.Length; i++)
				{
					squadList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref equipScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref status);
				UInt16 teamNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref teamNameLen);
				byte[] teamNameBytes = new byte[teamNameLen];
				for(int i = 0; i < teamNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref teamNameBytes[i]);
				}
				teamName = StringHelper.BytesToString(teamNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalCommission);
				BaseDLL.decode_uint32(buffer, ref pos_, ref bonusCommission);
				BaseDLL.decode_uint32(buffer, ref pos_, ref autoAgreeGold);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamModel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamGrade);
				UInt16 squadListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref squadListCnt);
				squadList = new SquadData[squadListCnt];
				for(int i = 0; i < squadList.Length; i++)
				{
					squadList[i] = new SquadData();
					squadList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// teamId
				_len += 4;
				// equipScore
				_len += 4;
				// status
				_len += 4;
				// teamName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(teamName);
					_len += 2 + _strBytes.Length;
				}
				// totalCommission
				_len += 4;
				// bonusCommission
				_len += 4;
				// autoAgreeGold
				_len += 4;
				// teamModel
				_len += 4;
				// teamGrade
				_len += 4;
				// squadList
				_len += 2;
				for(int j = 0; j < squadList.Length; j++)
				{
					_len += squadList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  团队列表请求
	/// </summary>
	[Protocol]
	public class TeamCopyTeamListReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100007;
		public UInt32 Sequence;
		/// <summary>
		///  队伍模式
		/// </summary>
		public UInt32 teamModel;
		/// <summary>
		///  请求页
		/// </summary>
		public UInt32 pageNum;

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
				BaseDLL.encode_uint32(buffer, ref pos_, teamModel);
				BaseDLL.encode_uint32(buffer, ref pos_, pageNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamModel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref pageNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, teamModel);
				BaseDLL.encode_uint32(buffer, ref pos_, pageNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamModel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref pageNum);
			}

			public int getLen()
			{
				int _len = 0;
				// teamModel
				_len += 4;
				// pageNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  团队面板属性
	/// </summary>
	public class TeamCopyTeamProperty : Protocol.IProtocolStream
	{
		/// <summary>
		///  id
		/// </summary>
		public UInt32 teamId;
		/// <summary>
		///  模式
		/// </summary>
		public UInt32 teamModel;
		/// <summary>
		///  佣金
		/// </summary>
		public UInt32 commission;
		/// <summary>
		///  名字
		/// </summary>
		public string teamName;
		/// <summary>
		///  难度
		/// </summary>
		public UInt32 teamGrade;
		/// <summary>
		///  人数
		/// </summary>
		public UInt32 memberNum;
		/// <summary>
		///  装备评分
		/// </summary>
		public UInt32 equipScore;
		/// <summary>
		///  状态
		/// </summary>
		public UInt32 status;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
				BaseDLL.encode_uint32(buffer, ref pos_, teamModel);
				BaseDLL.encode_uint32(buffer, ref pos_, commission);
				byte[] teamNameBytes = StringHelper.StringToUTF8Bytes(teamName);
				BaseDLL.encode_string(buffer, ref pos_, teamNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, teamGrade);
				BaseDLL.encode_uint32(buffer, ref pos_, memberNum);
				BaseDLL.encode_uint32(buffer, ref pos_, equipScore);
				BaseDLL.encode_uint32(buffer, ref pos_, status);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamModel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref commission);
				UInt16 teamNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref teamNameLen);
				byte[] teamNameBytes = new byte[teamNameLen];
				for(int i = 0; i < teamNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref teamNameBytes[i]);
				}
				teamName = StringHelper.BytesToString(teamNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamGrade);
				BaseDLL.decode_uint32(buffer, ref pos_, ref memberNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref equipScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref status);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
				BaseDLL.encode_uint32(buffer, ref pos_, teamModel);
				BaseDLL.encode_uint32(buffer, ref pos_, commission);
				byte[] teamNameBytes = StringHelper.StringToUTF8Bytes(teamName);
				BaseDLL.encode_string(buffer, ref pos_, teamNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, teamGrade);
				BaseDLL.encode_uint32(buffer, ref pos_, memberNum);
				BaseDLL.encode_uint32(buffer, ref pos_, equipScore);
				BaseDLL.encode_uint32(buffer, ref pos_, status);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamModel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref commission);
				UInt16 teamNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref teamNameLen);
				byte[] teamNameBytes = new byte[teamNameLen];
				for(int i = 0; i < teamNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref teamNameBytes[i]);
				}
				teamName = StringHelper.BytesToString(teamNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamGrade);
				BaseDLL.decode_uint32(buffer, ref pos_, ref memberNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref equipScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref status);
			}

			public int getLen()
			{
				int _len = 0;
				// teamId
				_len += 4;
				// teamModel
				_len += 4;
				// commission
				_len += 4;
				// teamName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(teamName);
					_len += 2 + _strBytes.Length;
				}
				// teamGrade
				_len += 4;
				// memberNum
				_len += 4;
				// equipScore
				_len += 4;
				// status
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  团队列表返回
	/// </summary>
	[Protocol]
	public class TeamCopyTeamListRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100008;
		public UInt32 Sequence;
		/// <summary>
		///  当前页
		/// </summary>
		public UInt32 curPage;
		/// <summary>
		///  总页数
		/// </summary>
		public UInt32 totalPageNum;
		/// <summary>
		///  团队列表
		/// </summary>
		public TeamCopyTeamProperty[] teamList = new TeamCopyTeamProperty[0];

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
				BaseDLL.encode_uint32(buffer, ref pos_, curPage);
				BaseDLL.encode_uint32(buffer, ref pos_, totalPageNum);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)teamList.Length);
				for(int i = 0; i < teamList.Length; i++)
				{
					teamList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref curPage);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalPageNum);
				UInt16 teamListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref teamListCnt);
				teamList = new TeamCopyTeamProperty[teamListCnt];
				for(int i = 0; i < teamList.Length; i++)
				{
					teamList[i] = new TeamCopyTeamProperty();
					teamList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, curPage);
				BaseDLL.encode_uint32(buffer, ref pos_, totalPageNum);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)teamList.Length);
				for(int i = 0; i < teamList.Length; i++)
				{
					teamList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref curPage);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalPageNum);
				UInt16 teamListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref teamListCnt);
				teamList = new TeamCopyTeamProperty[teamListCnt];
				for(int i = 0; i < teamList.Length; i++)
				{
					teamList[i] = new TeamCopyTeamProperty();
					teamList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// curPage
				_len += 4;
				// totalPageNum
				_len += 4;
				// teamList
				_len += 2;
				for(int j = 0; j < teamList.Length; j++)
				{
					_len += teamList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  申请入团请求
	/// </summary>
	[Protocol]
	public class TeamCopyTeamApplyReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100009;
		public UInt32 Sequence;
		/// <summary>
		///  队伍ID
		/// </summary>
		public UInt32 teamId;
		/// <summary>
		///  是否金主(非0是金主)
		/// </summary>
		public UInt32 isGold;

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
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
				BaseDLL.encode_uint32(buffer, ref pos_, isGold);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isGold);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
				BaseDLL.encode_uint32(buffer, ref pos_, isGold);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isGold);
			}

			public int getLen()
			{
				int _len = 0;
				// teamId
				_len += 4;
				// isGold
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  申请入团返回
	/// </summary>
	[Protocol]
	public class TeamCopyTeamApplyRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100010;
		public UInt32 Sequence;
		public UInt32 retCode;
		public UInt32 teamId;
		public UInt64 expireTime;

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
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
				BaseDLL.encode_uint64(buffer, ref pos_, expireTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref expireTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
				BaseDLL.encode_uint64(buffer, ref pos_, expireTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref expireTime);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// teamId
				_len += 4;
				// expireTime
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  退出团队请求
	/// </summary>
	[Protocol]
	public class TeamCopyTeamQuitReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100011;
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
	///  退出团队返回
	/// </summary>
	[Protocol]
	public class TeamCopyTeamQuitRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100012;
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

	public class TeamCopyBattlePlan : Protocol.IProtocolStream
	{
		/// <summary>
		///  难度
		/// </summary>
		public UInt32 difficulty;
		/// <summary>
		///  小队ID
		/// </summary>
		public UInt32 squadId;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, difficulty);
				BaseDLL.encode_uint32(buffer, ref pos_, squadId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref difficulty);
				BaseDLL.decode_uint32(buffer, ref pos_, ref squadId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, difficulty);
				BaseDLL.encode_uint32(buffer, ref pos_, squadId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref difficulty);
				BaseDLL.decode_uint32(buffer, ref pos_, ref squadId);
			}

			public int getLen()
			{
				int _len = 0;
				// difficulty
				_len += 4;
				// squadId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  开战请求
	/// </summary>
	[Protocol]
	public class TeamCopyStartBattleReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100013;
		public UInt32 Sequence;
		public UInt32 planModel;
		public TeamCopyBattlePlan[] battlePlanList = new TeamCopyBattlePlan[0];

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
				BaseDLL.encode_uint32(buffer, ref pos_, planModel);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)battlePlanList.Length);
				for(int i = 0; i < battlePlanList.Length; i++)
				{
					battlePlanList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref planModel);
				UInt16 battlePlanListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref battlePlanListCnt);
				battlePlanList = new TeamCopyBattlePlan[battlePlanListCnt];
				for(int i = 0; i < battlePlanList.Length; i++)
				{
					battlePlanList[i] = new TeamCopyBattlePlan();
					battlePlanList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, planModel);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)battlePlanList.Length);
				for(int i = 0; i < battlePlanList.Length; i++)
				{
					battlePlanList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref planModel);
				UInt16 battlePlanListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref battlePlanListCnt);
				battlePlanList = new TeamCopyBattlePlan[battlePlanListCnt];
				for(int i = 0; i < battlePlanList.Length; i++)
				{
					battlePlanList[i] = new TeamCopyBattlePlan();
					battlePlanList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// planModel
				_len += 4;
				// battlePlanList
				_len += 2;
				for(int j = 0; j < battlePlanList.Length; j++)
				{
					_len += battlePlanList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  开战返回
	/// </summary>
	[Protocol]
	public class TeamCopyStartBattleRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100014;
		public UInt32 Sequence;
		public string roleName;
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
				byte[] roleNameBytes = StringHelper.StringToUTF8Bytes(roleName);
				BaseDLL.encode_string(buffer, ref pos_, roleNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 roleNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref roleNameLen);
				byte[] roleNameBytes = new byte[roleNameLen];
				for(int i = 0; i < roleNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref roleNameBytes[i]);
				}
				roleName = StringHelper.BytesToString(roleNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] roleNameBytes = StringHelper.StringToUTF8Bytes(roleName);
				BaseDLL.encode_string(buffer, ref pos_, roleNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 roleNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref roleNameLen);
				byte[] roleNameBytes = new byte[roleNameLen];
				for(int i = 0; i < roleNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref roleNameBytes[i]);
				}
				roleName = StringHelper.BytesToString(roleNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public int getLen()
			{
				int _len = 0;
				// roleName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(roleName);
					_len += 2 + _strBytes.Length;
				}
				// retCode
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  开战通知
	/// </summary>
	[Protocol]
	public class TeamCopyStartBattleNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100015;
		public UInt32 Sequence;
		/// <summary>
		///  投票持续时间
		/// </summary>
		public UInt32 voteDurationTime;
		/// <summary>
		///  投票截止时间
		/// </summary>
		public UInt32 voteEndTime;

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
				BaseDLL.encode_uint32(buffer, ref pos_, voteDurationTime);
				BaseDLL.encode_uint32(buffer, ref pos_, voteEndTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref voteDurationTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref voteEndTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, voteDurationTime);
				BaseDLL.encode_uint32(buffer, ref pos_, voteEndTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref voteDurationTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref voteEndTime);
			}

			public int getLen()
			{
				int _len = 0;
				// voteDurationTime
				_len += 4;
				// voteEndTime
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  开战投票
	/// </summary>
	[Protocol]
	public class TeamCopyStartBattleVote : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100016;
		public UInt32 Sequence;
		/// <summary>
		///  非0同意，0拒绝
		/// </summary>
		public UInt32 vote;

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
				BaseDLL.encode_uint32(buffer, ref pos_, vote);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref vote);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, vote);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref vote);
			}

			public int getLen()
			{
				int _len = 0;
				// vote
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  投票通知
	/// </summary>
	[Protocol]
	public class TeamCopyVoteNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100017;
		public UInt32 Sequence;
		/// <summary>
		///  角色id
		/// </summary>
		public UInt64 roleId;
		/// <summary>
		///  非0同意，0拒绝
		/// </summary>
		public UInt32 vote;

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
				BaseDLL.encode_uint32(buffer, ref pos_, vote);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref vote);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint32(buffer, ref pos_, vote);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref vote);
			}

			public int getLen()
			{
				int _len = 0;
				// roleId
				_len += 8;
				// vote
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  投票完成
	/// </summary>
	[Protocol]
	public class TeamCopyVoteFinish : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100018;
		public UInt32 Sequence;
		/// <summary>
		///  结果(0代表成功，非0代表失败)
		/// </summary>
		public UInt32 result;
		/// <summary>
		///  未投票玩家
		/// </summary>
		public string[] notVotePlayer = new string[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)notVotePlayer.Length);
				for(int i = 0; i < notVotePlayer.Length; i++)
				{
					byte[] notVotePlayerBytes = StringHelper.StringToUTF8Bytes(notVotePlayer[i]);
					BaseDLL.encode_string(buffer, ref pos_, notVotePlayerBytes, (UInt16)(buffer.Length - pos_));
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 notVotePlayerCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref notVotePlayerCnt);
				notVotePlayer = new string[notVotePlayerCnt];
				for(int i = 0; i < notVotePlayer.Length; i++)
				{
					UInt16 notVotePlayerLen = 0;
					BaseDLL.decode_uint16(buffer, ref pos_, ref notVotePlayerLen);
					byte[] notVotePlayerBytes = new byte[notVotePlayerLen];
					for(int j = 0; j < notVotePlayerLen; j++)
					{
						BaseDLL.decode_int8(buffer, ref pos_, ref notVotePlayerBytes[j]);
					}
					notVotePlayer[i] = StringHelper.BytesToString(notVotePlayerBytes);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)notVotePlayer.Length);
				for(int i = 0; i < notVotePlayer.Length; i++)
				{
					byte[] notVotePlayerBytes = StringHelper.StringToUTF8Bytes(notVotePlayer[i]);
					BaseDLL.encode_string(buffer, ref pos_, notVotePlayerBytes, (UInt16)(buffer.Length - pos_));
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 notVotePlayerCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref notVotePlayerCnt);
				notVotePlayer = new string[notVotePlayerCnt];
				for(int i = 0; i < notVotePlayer.Length; i++)
				{
					UInt16 notVotePlayerLen = 0;
					BaseDLL.decode_uint16(buffer, ref pos_, ref notVotePlayerLen);
					byte[] notVotePlayerBytes = new byte[notVotePlayerLen];
					for(int j = 0; j < notVotePlayerLen; j++)
					{
						BaseDLL.decode_int8(buffer, ref pos_, ref notVotePlayerBytes[j]);
					}
					notVotePlayer[i] = StringHelper.BytesToString(notVotePlayerBytes);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// notVotePlayer
				_len += 2;
				for(int j = 0; j < notVotePlayer.Length; j++)
				{
					{
						byte[] _strBytes = StringHelper.StringToUTF8Bytes(notVotePlayer[j]);
						_len += 2 + _strBytes.Length;
					}
				}
				return _len;
			}
		#endregion

	}

	public class TeamCopyTargetDetail : Protocol.IProtocolStream
	{
		/// <summary>
		///  据点id
		/// </summary>
		public UInt32 fieldId;
		/// <summary>
		///  当前次数
		/// </summary>
		public UInt32 curNum;
		/// <summary>
		///  总次数
		/// </summary>
		public UInt32 totalNum;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, fieldId);
				BaseDLL.encode_uint32(buffer, ref pos_, curNum);
				BaseDLL.encode_uint32(buffer, ref pos_, totalNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref fieldId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref curNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, fieldId);
				BaseDLL.encode_uint32(buffer, ref pos_, curNum);
				BaseDLL.encode_uint32(buffer, ref pos_, totalNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref fieldId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref curNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalNum);
			}

			public int getLen()
			{
				int _len = 0;
				// fieldId
				_len += 4;
				// curNum
				_len += 4;
				// totalNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  目标
	/// </summary>
	public class TeamCopyTarget : Protocol.IProtocolStream
	{
		/// <summary>
		///  目标id
		/// </summary>
		public UInt32 targetId;
		/// <summary>
		///  目标详情
		/// </summary>
		public TeamCopyTargetDetail[] targetDetailList = new TeamCopyTargetDetail[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, targetId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)targetDetailList.Length);
				for(int i = 0; i < targetDetailList.Length; i++)
				{
					targetDetailList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref targetId);
				UInt16 targetDetailListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref targetDetailListCnt);
				targetDetailList = new TeamCopyTargetDetail[targetDetailListCnt];
				for(int i = 0; i < targetDetailList.Length; i++)
				{
					targetDetailList[i] = new TeamCopyTargetDetail();
					targetDetailList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, targetId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)targetDetailList.Length);
				for(int i = 0; i < targetDetailList.Length; i++)
				{
					targetDetailList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref targetId);
				UInt16 targetDetailListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref targetDetailListCnt);
				targetDetailList = new TeamCopyTargetDetail[targetDetailListCnt];
				for(int i = 0; i < targetDetailList.Length; i++)
				{
					targetDetailList[i] = new TeamCopyTargetDetail();
					targetDetailList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// targetId
				_len += 4;
				// targetDetailList
				_len += 2;
				for(int j = 0; j < targetDetailList.Length; j++)
				{
					_len += targetDetailList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  据点
	/// </summary>
	public class TeamCopyFeild : Protocol.IProtocolStream
	{
		/// <summary>
		///  据点id
		/// </summary>
		public UInt32 feildId;
		/// <summary>
		///  剩余次数
		/// </summary>
		public UInt32 oddNum;
		/// <summary>
		///  状态
		/// </summary>
		public UInt32 state;
		/// <summary>
		///  重生时间
		/// </summary>
		public UInt32 rebornTime;
		/// <summary>
		///  能量恢复时间点
		/// </summary>
		public UInt32 energyReviveTime;
		/// <summary>
		///  攻打小队列表
		/// </summary>
		public UInt32[] attackSquadList = new UInt32[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, feildId);
				BaseDLL.encode_uint32(buffer, ref pos_, oddNum);
				BaseDLL.encode_uint32(buffer, ref pos_, state);
				BaseDLL.encode_uint32(buffer, ref pos_, rebornTime);
				BaseDLL.encode_uint32(buffer, ref pos_, energyReviveTime);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)attackSquadList.Length);
				for(int i = 0; i < attackSquadList.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, attackSquadList[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref feildId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oddNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref state);
				BaseDLL.decode_uint32(buffer, ref pos_, ref rebornTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref energyReviveTime);
				UInt16 attackSquadListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref attackSquadListCnt);
				attackSquadList = new UInt32[attackSquadListCnt];
				for(int i = 0; i < attackSquadList.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref attackSquadList[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, feildId);
				BaseDLL.encode_uint32(buffer, ref pos_, oddNum);
				BaseDLL.encode_uint32(buffer, ref pos_, state);
				BaseDLL.encode_uint32(buffer, ref pos_, rebornTime);
				BaseDLL.encode_uint32(buffer, ref pos_, energyReviveTime);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)attackSquadList.Length);
				for(int i = 0; i < attackSquadList.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, attackSquadList[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref feildId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oddNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref state);
				BaseDLL.decode_uint32(buffer, ref pos_, ref rebornTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref energyReviveTime);
				UInt16 attackSquadListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref attackSquadListCnt);
				attackSquadList = new UInt32[attackSquadListCnt];
				for(int i = 0; i < attackSquadList.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref attackSquadList[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// feildId
				_len += 4;
				// oddNum
				_len += 4;
				// state
				_len += 4;
				// rebornTime
				_len += 4;
				// energyReviveTime
				_len += 4;
				// attackSquadList
				_len += 2 + 4 * attackSquadList.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  阶段通知
	/// </summary>
	[Protocol]
	public class TeamCopyStageNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100019;
		public UInt32 Sequence;
		/// <summary>
		///  阶段id
		/// </summary>
		public UInt32 stageId;
		/// <summary>
		///  游戏结束时间
		/// </summary>
		public UInt32 gameOverTime;
		/// <summary>
		///  小队目标
		/// </summary>
		public TeamCopyTarget squadTarget = new TeamCopyTarget();
		/// <summary>
		///  团队目标
		/// </summary>
		public TeamCopyTarget teamTarget = new TeamCopyTarget();
		/// <summary>
		///  据点列表
		/// </summary>
		public TeamCopyFeild[] feildList = new TeamCopyFeild[0];

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
				BaseDLL.encode_uint32(buffer, ref pos_, stageId);
				BaseDLL.encode_uint32(buffer, ref pos_, gameOverTime);
				squadTarget.encode(buffer, ref pos_);
				teamTarget.encode(buffer, ref pos_);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)feildList.Length);
				for(int i = 0; i < feildList.Length; i++)
				{
					feildList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref stageId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref gameOverTime);
				squadTarget.decode(buffer, ref pos_);
				teamTarget.decode(buffer, ref pos_);
				UInt16 feildListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref feildListCnt);
				feildList = new TeamCopyFeild[feildListCnt];
				for(int i = 0; i < feildList.Length; i++)
				{
					feildList[i] = new TeamCopyFeild();
					feildList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, stageId);
				BaseDLL.encode_uint32(buffer, ref pos_, gameOverTime);
				squadTarget.encode(buffer, ref pos_);
				teamTarget.encode(buffer, ref pos_);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)feildList.Length);
				for(int i = 0; i < feildList.Length; i++)
				{
					feildList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref stageId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref gameOverTime);
				squadTarget.decode(buffer, ref pos_);
				teamTarget.decode(buffer, ref pos_);
				UInt16 feildListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref feildListCnt);
				feildList = new TeamCopyFeild[feildListCnt];
				for(int i = 0; i < feildList.Length; i++)
				{
					feildList[i] = new TeamCopyFeild();
					feildList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// stageId
				_len += 4;
				// gameOverTime
				_len += 4;
				// squadTarget
				_len += squadTarget.getLen();
				// teamTarget
				_len += teamTarget.getLen();
				// feildList
				_len += 2;
				for(int j = 0; j < feildList.Length; j++)
				{
					_len += feildList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  据点通知
	/// </summary>
	[Protocol]
	public class TeamCopyFieldNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100020;
		public UInt32 Sequence;
		/// <summary>
		///  据点列表(已存在的据点更新，新据点增加)
		/// </summary>
		public TeamCopyFeild[] feildList = new TeamCopyFeild[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)feildList.Length);
				for(int i = 0; i < feildList.Length; i++)
				{
					feildList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 feildListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref feildListCnt);
				feildList = new TeamCopyFeild[feildListCnt];
				for(int i = 0; i < feildList.Length; i++)
				{
					feildList[i] = new TeamCopyFeild();
					feildList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)feildList.Length);
				for(int i = 0; i < feildList.Length; i++)
				{
					feildList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 feildListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref feildListCnt);
				feildList = new TeamCopyFeild[feildListCnt];
				for(int i = 0; i < feildList.Length; i++)
				{
					feildList[i] = new TeamCopyFeild();
					feildList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// feildList
				_len += 2;
				for(int j = 0; j < feildList.Length; j++)
				{
					_len += feildList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  目标通知
	/// </summary>
	[Protocol]
	public class TeamCopyTargetNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100021;
		public UInt32 Sequence;
		/// <summary>
		///  小队目标
		/// </summary>
		public TeamCopyTarget squadTarget = new TeamCopyTarget();
		/// <summary>
		///  团队目标
		/// </summary>
		public TeamCopyTarget teamTarget = new TeamCopyTarget();

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
				squadTarget.encode(buffer, ref pos_);
				teamTarget.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				squadTarget.decode(buffer, ref pos_);
				teamTarget.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				squadTarget.encode(buffer, ref pos_);
				teamTarget.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				squadTarget.decode(buffer, ref pos_);
				teamTarget.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// squadTarget
				_len += squadTarget.getLen();
				// teamTarget
				_len += teamTarget.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  阶段结束
	/// </summary>
	[Protocol]
	public class TeamCopyStageEnd : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100022;
		public UInt32 Sequence;
		/// <summary>
		///  阶段id
		/// </summary>
		public UInt32 stageId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, stageId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref stageId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, stageId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref stageId);
			}

			public int getLen()
			{
				int _len = 0;
				// stageId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求申请列表
	/// </summary>
	[Protocol]
	public class TeamCopyTeamApplyListReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100023;
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

	public class TeamCopyApplyProperty : Protocol.IProtocolStream
	{
		/// <summary>
		///  玩家ID
		/// </summary>
		public UInt64 playerId;
		/// <summary>
		///  玩家名字
		/// </summary>
		public string playerName;
		/// <summary>
		///  玩家职业
		/// </summary>
		public UInt32 playerOccu;
		/// <summary>
		///  觉醒
		/// </summary>
		public UInt32 playerAwaken;
		/// <summary>
		///  等级
		/// </summary>
		public UInt32 playerLevel;
		/// <summary>
		///  装备评分
		/// </summary>
		public UInt32 equipScore;
		/// <summary>
		///  是否金主(非0是金主)
		/// </summary>
		public UInt32 isGold;
		/// <summary>
		///  公会id
		/// </summary>
		public UInt64 guildId;
		/// <summary>
		///  区服id
		/// </summary>
		public UInt32 zoneId;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
				BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, playerOccu);
				BaseDLL.encode_uint32(buffer, ref pos_, playerAwaken);
				BaseDLL.encode_uint32(buffer, ref pos_, playerLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, equipScore);
				BaseDLL.encode_uint32(buffer, ref pos_, isGold);
				BaseDLL.encode_uint64(buffer, ref pos_, guildId);
				BaseDLL.encode_uint32(buffer, ref pos_, zoneId);
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
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerOccu);
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerAwaken);
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref equipScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isGold);
				BaseDLL.decode_uint64(buffer, ref pos_, ref guildId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref zoneId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
				BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, playerOccu);
				BaseDLL.encode_uint32(buffer, ref pos_, playerAwaken);
				BaseDLL.encode_uint32(buffer, ref pos_, playerLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, equipScore);
				BaseDLL.encode_uint32(buffer, ref pos_, isGold);
				BaseDLL.encode_uint64(buffer, ref pos_, guildId);
				BaseDLL.encode_uint32(buffer, ref pos_, zoneId);
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
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerOccu);
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerAwaken);
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref equipScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isGold);
				BaseDLL.decode_uint64(buffer, ref pos_, ref guildId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref zoneId);
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
				// playerOccu
				_len += 4;
				// playerAwaken
				_len += 4;
				// playerLevel
				_len += 4;
				// equipScore
				_len += 4;
				// isGold
				_len += 4;
				// guildId
				_len += 8;
				// zoneId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  返回申请列表
	/// </summary>
	[Protocol]
	public class TeamCopyTeamApplyListRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100024;
		public UInt32 Sequence;
		/// <summary>
		///  申请列表
		/// </summary>
		public TeamCopyApplyProperty[] applyList = new TeamCopyApplyProperty[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)applyList.Length);
				for(int i = 0; i < applyList.Length; i++)
				{
					applyList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 applyListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref applyListCnt);
				applyList = new TeamCopyApplyProperty[applyListCnt];
				for(int i = 0; i < applyList.Length; i++)
				{
					applyList[i] = new TeamCopyApplyProperty();
					applyList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)applyList.Length);
				for(int i = 0; i < applyList.Length; i++)
				{
					applyList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 applyListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref applyListCnt);
				applyList = new TeamCopyApplyProperty[applyListCnt];
				for(int i = 0; i < applyList.Length; i++)
				{
					applyList[i] = new TeamCopyApplyProperty();
					applyList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// applyList
				_len += 2;
				for(int j = 0; j < applyList.Length; j++)
				{
					_len += applyList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  申请处理
	/// </summary>
	[Protocol]
	public class TeamCopyTeamApplyReplyReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100025;
		public UInt32 Sequence;
		/// <summary>
		///  非0同意，0拒绝
		/// </summary>
		public UInt32 isAgree;
		/// <summary>
		///  玩家列表
		/// </summary>
		public UInt64[] playerIds = new UInt64[0];

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
				BaseDLL.encode_uint32(buffer, ref pos_, isAgree);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)playerIds.Length);
				for(int i = 0; i < playerIds.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, playerIds[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref isAgree);
				UInt16 playerIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerIdsCnt);
				playerIds = new UInt64[playerIdsCnt];
				for(int i = 0; i < playerIds.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref playerIds[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, isAgree);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)playerIds.Length);
				for(int i = 0; i < playerIds.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, playerIds[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref isAgree);
				UInt16 playerIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerIdsCnt);
				playerIds = new UInt64[playerIdsCnt];
				for(int i = 0; i < playerIds.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref playerIds[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// isAgree
				_len += 4;
				// playerIds
				_len += 2 + 8 * playerIds.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  申请处理返回
	/// </summary>
	[Protocol]
	public class TeamCopyTeamApplyReplyRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100026;
		public UInt32 Sequence;
		/// <summary>
		///  返回码
		/// </summary>
		public UInt32 retCode;
		/// <summary>
		///  非0同意，0拒绝
		/// </summary>
		public UInt32 isAgree;
		/// <summary>
		///  玩家列表
		/// </summary>
		public UInt64[] playerIds = new UInt64[0];

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
				BaseDLL.encode_uint32(buffer, ref pos_, isAgree);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)playerIds.Length);
				for(int i = 0; i < playerIds.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, playerIds[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isAgree);
				UInt16 playerIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerIdsCnt);
				playerIds = new UInt64[playerIdsCnt];
				for(int i = 0; i < playerIds.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref playerIds[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint32(buffer, ref pos_, isAgree);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)playerIds.Length);
				for(int i = 0; i < playerIds.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, playerIds[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isAgree);
				UInt16 playerIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerIdsCnt);
				playerIds = new UInt64[playerIdsCnt];
				for(int i = 0; i < playerIds.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref playerIds[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// isAgree
				_len += 4;
				// playerIds
				_len += 2 + 8 * playerIds.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  开始挑战
	/// </summary>
	[Protocol]
	public class TeamCopyStartChallengeReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100027;
		public UInt32 Sequence;
		/// <summary>
		///  据点id
		/// </summary>
		public UInt32 fieldId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, fieldId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref fieldId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, fieldId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref fieldId);
			}

			public int getLen()
			{
				int _len = 0;
				// fieldId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  开始挑战
	/// </summary>
	[Protocol]
	public class TeamCopyStartChallengeRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100028;
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
	///  找队友
	/// </summary>
	[Protocol]
	public class TeamCopyFindTeamMateReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100029;
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

	[Protocol]
	public class TeamCopyFindTeamMateRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100030;
		public UInt32 Sequence;
		/// <summary>
		///  玩家
		/// </summary>
		public TeamCopyApplyProperty[] playerList = new TeamCopyApplyProperty[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)playerList.Length);
				for(int i = 0; i < playerList.Length; i++)
				{
					playerList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 playerListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerListCnt);
				playerList = new TeamCopyApplyProperty[playerListCnt];
				for(int i = 0; i < playerList.Length; i++)
				{
					playerList[i] = new TeamCopyApplyProperty();
					playerList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)playerList.Length);
				for(int i = 0; i < playerList.Length; i++)
				{
					playerList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 playerListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerListCnt);
				playerList = new TeamCopyApplyProperty[playerListCnt];
				for(int i = 0; i < playerList.Length; i++)
				{
					playerList[i] = new TeamCopyApplyProperty();
					playerList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// playerList
				_len += 2;
				for(int j = 0; j < playerList.Length; j++)
				{
					_len += playerList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  邀请
	/// </summary>
	[Protocol]
	public class TeamCopyInvitePlayer : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100031;
		public UInt32 Sequence;
		/// <summary>
		///  邀请列表
		/// </summary>
		public UInt64[] inviteList = new UInt64[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)inviteList.Length);
				for(int i = 0; i < inviteList.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, inviteList[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 inviteListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref inviteListCnt);
				inviteList = new UInt64[inviteListCnt];
				for(int i = 0; i < inviteList.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref inviteList[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)inviteList.Length);
				for(int i = 0; i < inviteList.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, inviteList[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 inviteListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref inviteListCnt);
				inviteList = new UInt64[inviteListCnt];
				for(int i = 0; i < inviteList.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref inviteList[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// inviteList
				_len += 2 + 8 * inviteList.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  邀请通知
	/// </summary>
	[Protocol]
	public class TeamCopyInviteNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100032;
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
	///  请求翻牌
	/// </summary>
	[Protocol]
	public class TeamCopyStageFlopReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100035;
		public UInt32 Sequence;
		/// <summary>
		///  阶段id
		/// </summary>
		public UInt32 stageId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, stageId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref stageId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, stageId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref stageId);
			}

			public int getLen()
			{
				int _len = 0;
				// stageId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  翻牌奖励
	/// </summary>
	public class TeamCopyFlop : Protocol.IProtocolStream
	{
		/// <summary>
		///  玩家name
		/// </summary>
		public string playerName;
		/// <summary>
		///  玩家id
		/// </summary>
		public UInt64 playerId;
		/// <summary>
		///  奖励id
		/// </summary>
		public UInt32 rewardId;
		/// <summary>
		///  奖励数量
		/// </summary>
		public UInt32 rewardNum;
		/// <summary>
		///  序号
		/// </summary>
		public UInt32 number;
		/// <summary>
		///  是否金牌
		/// </summary>
		public UInt32 goldFlop;
		/// <summary>
		///  是否限制(TeamCopyFlopLimit)
		/// </summary>
		public UInt32 isLimit;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
				BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				BaseDLL.encode_uint32(buffer, ref pos_, rewardId);
				BaseDLL.encode_uint32(buffer, ref pos_, rewardNum);
				BaseDLL.encode_uint32(buffer, ref pos_, number);
				BaseDLL.encode_uint32(buffer, ref pos_, goldFlop);
				BaseDLL.encode_uint32(buffer, ref pos_, isLimit);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 playerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerNameLen);
				byte[] playerNameBytes = new byte[playerNameLen];
				for(int i = 0; i < playerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref playerNameBytes[i]);
				}
				playerName = StringHelper.BytesToString(playerNameBytes);
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref rewardId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref rewardNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref number);
				BaseDLL.decode_uint32(buffer, ref pos_, ref goldFlop);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isLimit);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
				BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				BaseDLL.encode_uint32(buffer, ref pos_, rewardId);
				BaseDLL.encode_uint32(buffer, ref pos_, rewardNum);
				BaseDLL.encode_uint32(buffer, ref pos_, number);
				BaseDLL.encode_uint32(buffer, ref pos_, goldFlop);
				BaseDLL.encode_uint32(buffer, ref pos_, isLimit);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 playerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerNameLen);
				byte[] playerNameBytes = new byte[playerNameLen];
				for(int i = 0; i < playerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref playerNameBytes[i]);
				}
				playerName = StringHelper.BytesToString(playerNameBytes);
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref rewardId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref rewardNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref number);
				BaseDLL.decode_uint32(buffer, ref pos_, ref goldFlop);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isLimit);
			}

			public int getLen()
			{
				int _len = 0;
				// playerName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(playerName);
					_len += 2 + _strBytes.Length;
				}
				// playerId
				_len += 8;
				// rewardId
				_len += 4;
				// rewardNum
				_len += 4;
				// number
				_len += 4;
				// goldFlop
				_len += 4;
				// isLimit
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  翻牌
	/// </summary>
	[Protocol]
	public class TeamCopyStageFlopRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100036;
		public UInt32 Sequence;
		/// <summary>
		///  阶段id
		/// </summary>
		public UInt32 stageId;
		/// <summary>
		///  翻牌列表
		/// </summary>
		public TeamCopyFlop[] flopList = new TeamCopyFlop[0];

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
				BaseDLL.encode_uint32(buffer, ref pos_, stageId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)flopList.Length);
				for(int i = 0; i < flopList.Length; i++)
				{
					flopList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref stageId);
				UInt16 flopListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref flopListCnt);
				flopList = new TeamCopyFlop[flopListCnt];
				for(int i = 0; i < flopList.Length; i++)
				{
					flopList[i] = new TeamCopyFlop();
					flopList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, stageId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)flopList.Length);
				for(int i = 0; i < flopList.Length; i++)
				{
					flopList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref stageId);
				UInt16 flopListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref flopListCnt);
				flopList = new TeamCopyFlop[flopListCnt];
				for(int i = 0; i < flopList.Length; i++)
				{
					flopList[i] = new TeamCopyFlop();
					flopList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// stageId
				_len += 4;
				// flopList
				_len += 2;
				for(int j = 0; j < flopList.Length; j++)
				{
					_len += flopList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  小队通知
	/// </summary>
	[Protocol]
	public class TeamCopySquadNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100038;
		public UInt32 Sequence;
		/// <summary>
		///  小队id
		/// </summary>
		public UInt32 squadId;
		/// <summary>
		///  小队状态
		/// </summary>
		public UInt32 squadStatus;

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
				BaseDLL.encode_uint32(buffer, ref pos_, squadId);
				BaseDLL.encode_uint32(buffer, ref pos_, squadStatus);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref squadId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref squadStatus);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, squadId);
				BaseDLL.encode_uint32(buffer, ref pos_, squadStatus);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref squadId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref squadStatus);
			}

			public int getLen()
			{
				int _len = 0;
				// squadId
				_len += 4;
				// squadStatus
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  更新通知
	/// </summary>
	[Protocol]
	public class TeamCopyTeamUpdate : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100039;
		public UInt32 Sequence;
		/// <summary>
		///  1.解散
		/// </summary>
		public UInt32 opType;
		public UInt32 teamId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, opType);
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opType);
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
			}

			public int getLen()
			{
				int _len = 0;
				// opType
				_len += 4;
				// teamId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  团本重连通知
	/// </summary>
	[Protocol]
	public class TeamCopyReconnectNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100040;
		public UInt32 Sequence;
		/// <summary>
		///  重连返回场景
		/// </summary>
		public UInt32 sceneId;
		/// <summary>
		///  玩家过期时间
		/// </summary>
		public UInt64 expireTime;

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
				BaseDLL.encode_uint32(buffer, ref pos_, sceneId);
				BaseDLL.encode_uint64(buffer, ref pos_, expireTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref sceneId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref expireTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, sceneId);
				BaseDLL.encode_uint64(buffer, ref pos_, expireTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref sceneId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref expireTime);
			}

			public int getLen()
			{
				int _len = 0;
				// sceneId
				_len += 4;
				// expireTime
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  换座位
	/// </summary>
	[Protocol]
	public class TeamCopyChangeSeatReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100041;
		public UInt32 Sequence;
		public UInt32 srcSeat;
		public UInt32 destSeat;

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
				BaseDLL.encode_uint32(buffer, ref pos_, srcSeat);
				BaseDLL.encode_uint32(buffer, ref pos_, destSeat);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref srcSeat);
				BaseDLL.decode_uint32(buffer, ref pos_, ref destSeat);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, srcSeat);
				BaseDLL.encode_uint32(buffer, ref pos_, destSeat);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref srcSeat);
				BaseDLL.decode_uint32(buffer, ref pos_, ref destSeat);
			}

			public int getLen()
			{
				int _len = 0;
				// srcSeat
				_len += 4;
				// destSeat
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class TeamCopyChangeSeatRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100042;
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
	///  队伍详情
	/// </summary>
	[Protocol]
	public class TeamCopyTeamDetailReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100043;
		public UInt32 Sequence;
		public UInt32 teamId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
			}

			public int getLen()
			{
				int _len = 0;
				// teamId
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class TeamCopyTeamDetailRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100044;
		public UInt32 Sequence;
		/// <summary>
		///  队伍ID
		/// </summary>
		public UInt32 teamId;
		/// <summary>
		///  队伍模式
		/// </summary>
		public UInt32 teamModel;
		/// <summary>
		///  团队名字
		/// </summary>
		public string teamName;
		/// <summary>
		///  佣金总数
		/// </summary>
		public UInt32 totalCommission;
		/// <summary>
		///  分成佣金
		/// </summary>
		public UInt32 bonusCommission;
		/// <summary>
		///  小队列表
		/// </summary>
		public SquadData[] squadList = new SquadData[0];

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
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
				BaseDLL.encode_uint32(buffer, ref pos_, teamModel);
				byte[] teamNameBytes = StringHelper.StringToUTF8Bytes(teamName);
				BaseDLL.encode_string(buffer, ref pos_, teamNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, totalCommission);
				BaseDLL.encode_uint32(buffer, ref pos_, bonusCommission);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)squadList.Length);
				for(int i = 0; i < squadList.Length; i++)
				{
					squadList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamModel);
				UInt16 teamNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref teamNameLen);
				byte[] teamNameBytes = new byte[teamNameLen];
				for(int i = 0; i < teamNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref teamNameBytes[i]);
				}
				teamName = StringHelper.BytesToString(teamNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalCommission);
				BaseDLL.decode_uint32(buffer, ref pos_, ref bonusCommission);
				UInt16 squadListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref squadListCnt);
				squadList = new SquadData[squadListCnt];
				for(int i = 0; i < squadList.Length; i++)
				{
					squadList[i] = new SquadData();
					squadList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
				BaseDLL.encode_uint32(buffer, ref pos_, teamModel);
				byte[] teamNameBytes = StringHelper.StringToUTF8Bytes(teamName);
				BaseDLL.encode_string(buffer, ref pos_, teamNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, totalCommission);
				BaseDLL.encode_uint32(buffer, ref pos_, bonusCommission);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)squadList.Length);
				for(int i = 0; i < squadList.Length; i++)
				{
					squadList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamModel);
				UInt16 teamNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref teamNameLen);
				byte[] teamNameBytes = new byte[teamNameLen];
				for(int i = 0; i < teamNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref teamNameBytes[i]);
				}
				teamName = StringHelper.BytesToString(teamNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalCommission);
				BaseDLL.decode_uint32(buffer, ref pos_, ref bonusCommission);
				UInt16 squadListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref squadListCnt);
				squadList = new SquadData[squadListCnt];
				for(int i = 0; i < squadList.Length; i++)
				{
					squadList[i] = new SquadData();
					squadList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// teamId
				_len += 4;
				// teamModel
				_len += 4;
				// teamName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(teamName);
					_len += 2 + _strBytes.Length;
				}
				// totalCommission
				_len += 4;
				// bonusCommission
				_len += 4;
				// squadList
				_len += 2;
				for(int j = 0; j < squadList.Length; j++)
				{
					_len += squadList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  踢出
	/// </summary>
	[Protocol]
	public class TeamCopyKickReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100045;
		public UInt32 Sequence;
		/// <summary>
		///  踢出的玩家
		/// </summary>
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

	[Protocol]
	public class TeamCopyKickRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100046;
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
	///  任命
	/// </summary>
	[Protocol]
	public class TeamCopyAppointmentReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100047;
		public UInt32 Sequence;
		/// <summary>
		///  任命玩家
		/// </summary>
		public UInt64 playerId;
		/// <summary>
		///  职位
		/// </summary>
		public UInt32 post;

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
				BaseDLL.encode_uint32(buffer, ref pos_, post);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref post);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				BaseDLL.encode_uint32(buffer, ref pos_, post);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref post);
			}

			public int getLen()
			{
				int _len = 0;
				// playerId
				_len += 8;
				// post
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class TeamCopyAppointmentRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100048;
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
	///  队员通知
	/// </summary>
	[Protocol]
	public class TeamCopyMemberNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100049;
		public UInt32 Sequence;
		/// <summary>
		///  队员名字
		/// </summary>
		public string memberName;
		/// <summary>
		///  标志(0：加入，1：离开)
		/// </summary>
		public UInt32 flag;

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
				byte[] memberNameBytes = StringHelper.StringToUTF8Bytes(memberName);
				BaseDLL.encode_string(buffer, ref pos_, memberNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, flag);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 memberNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref memberNameLen);
				byte[] memberNameBytes = new byte[memberNameLen];
				for(int i = 0; i < memberNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref memberNameBytes[i]);
				}
				memberName = StringHelper.BytesToString(memberNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref flag);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] memberNameBytes = StringHelper.StringToUTF8Bytes(memberName);
				BaseDLL.encode_string(buffer, ref pos_, memberNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, flag);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 memberNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref memberNameLen);
				byte[] memberNameBytes = new byte[memberNameLen];
				for(int i = 0; i < memberNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref memberNameBytes[i]);
				}
				memberName = StringHelper.BytesToString(memberNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref flag);
			}

			public int getLen()
			{
				int _len = 0;
				// memberName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(memberName);
					_len += 2 + _strBytes.Length;
				}
				// flag
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  自动同意金主
	/// </summary>
	[Protocol]
	public class TeamCopyAutoAgreeGoldReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100050;
		public UInt32 Sequence;
		/// <summary>
		///  (0：不同意，1：同意)
		/// </summary>
		public UInt32 isAutoAgree;

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
				BaseDLL.encode_uint32(buffer, ref pos_, isAutoAgree);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref isAutoAgree);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, isAutoAgree);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref isAutoAgree);
			}

			public int getLen()
			{
				int _len = 0;
				// isAutoAgree
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class TeamCopyAutoAgreeGoldRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100051;
		public UInt32 Sequence;
		public UInt32 retCode;
		public UInt32 isAutoAgree;

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
				BaseDLL.encode_uint32(buffer, ref pos_, isAutoAgree);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isAutoAgree);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint32(buffer, ref pos_, isAutoAgree);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isAutoAgree);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// isAutoAgree
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  邀请列表
	/// </summary>
	[Protocol]
	public class TeamCopyInviteListReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100052;
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

	public class TCInviteInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  队伍ID
		/// </summary>
		public UInt32 teamId;
		/// <summary>
		///  队伍模式
		/// </summary>
		public UInt32 teamModel;
		/// <summary>
		///  队伍难度
		/// </summary>
		public UInt32 teamGrade;
		/// <summary>
		///  名字
		/// </summary>
		public string name;
		/// <summary>
		///  职业
		/// </summary>
		public UInt32 occu;
		/// <summary>
		///  觉醒
		/// </summary>
		public UInt32 awaken;
		/// <summary>
		///  等级
		/// </summary>
		public UInt32 level;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
				BaseDLL.encode_uint32(buffer, ref pos_, teamModel);
				BaseDLL.encode_uint32(buffer, ref pos_, teamGrade);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, occu);
				BaseDLL.encode_uint32(buffer, ref pos_, awaken);
				BaseDLL.encode_uint32(buffer, ref pos_, level);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamModel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamGrade);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref occu);
				BaseDLL.decode_uint32(buffer, ref pos_, ref awaken);
				BaseDLL.decode_uint32(buffer, ref pos_, ref level);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
				BaseDLL.encode_uint32(buffer, ref pos_, teamModel);
				BaseDLL.encode_uint32(buffer, ref pos_, teamGrade);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, occu);
				BaseDLL.encode_uint32(buffer, ref pos_, awaken);
				BaseDLL.encode_uint32(buffer, ref pos_, level);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamModel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamGrade);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref occu);
				BaseDLL.decode_uint32(buffer, ref pos_, ref awaken);
				BaseDLL.decode_uint32(buffer, ref pos_, ref level);
			}

			public int getLen()
			{
				int _len = 0;
				// teamId
				_len += 4;
				// teamModel
				_len += 4;
				// teamGrade
				_len += 4;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// occu
				_len += 4;
				// awaken
				_len += 4;
				// level
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class TeamCopyInviteListRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100053;
		public UInt32 Sequence;
		public TCInviteInfo[] inviteList = new TCInviteInfo[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)inviteList.Length);
				for(int i = 0; i < inviteList.Length; i++)
				{
					inviteList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 inviteListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref inviteListCnt);
				inviteList = new TCInviteInfo[inviteListCnt];
				for(int i = 0; i < inviteList.Length; i++)
				{
					inviteList[i] = new TCInviteInfo();
					inviteList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)inviteList.Length);
				for(int i = 0; i < inviteList.Length; i++)
				{
					inviteList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 inviteListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref inviteListCnt);
				inviteList = new TCInviteInfo[inviteListCnt];
				for(int i = 0; i < inviteList.Length; i++)
				{
					inviteList[i] = new TCInviteInfo();
					inviteList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// inviteList
				_len += 2;
				for(int j = 0; j < inviteList.Length; j++)
				{
					_len += inviteList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  邀请选择
	/// </summary>
	[Protocol]
	public class TeamCopyInviteChoiceReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100054;
		public UInt32 Sequence;
		/// <summary>
		///  0拒绝，非0同意
		/// </summary>
		public UInt32 isAgree;
		/// <summary>
		///  队伍Id
		/// </summary>
		public UInt32[] teamId = new UInt32[0];

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
				BaseDLL.encode_uint32(buffer, ref pos_, isAgree);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)teamId.Length);
				for(int i = 0; i < teamId.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, teamId[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref isAgree);
				UInt16 teamIdCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref teamIdCnt);
				teamId = new UInt32[teamIdCnt];
				for(int i = 0; i < teamId.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref teamId[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, isAgree);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)teamId.Length);
				for(int i = 0; i < teamId.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, teamId[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref isAgree);
				UInt16 teamIdCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref teamIdCnt);
				teamId = new UInt32[teamIdCnt];
				for(int i = 0; i < teamId.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref teamId[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// isAgree
				_len += 4;
				// teamId
				_len += 2 + 4 * teamId.Length;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class TeamCopyInviteChoiceRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100055;
		public UInt32 Sequence;
		public UInt32 retCode;
		public UInt32[] teamId = new UInt32[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)teamId.Length);
				for(int i = 0; i < teamId.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, teamId[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				UInt16 teamIdCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref teamIdCnt);
				teamId = new UInt32[teamIdCnt];
				for(int i = 0; i < teamId.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref teamId[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)teamId.Length);
				for(int i = 0; i < teamId.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, teamId[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				UInt16 teamIdCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref teamIdCnt);
				teamId = new UInt32[teamIdCnt];
				for(int i = 0; i < teamId.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref teamId[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// teamId
				_len += 2 + 4 * teamId.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  任命通知
	/// </summary>
	[Protocol]
	public class TeamCopyAppointmentNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100056;
		public UInt32 Sequence;
		public UInt32 post;
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
				BaseDLL.encode_uint32(buffer, ref pos_, post);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref post);
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
				BaseDLL.encode_uint32(buffer, ref pos_, post);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref post);
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
				// post
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
	///  团队状态通知
	/// </summary>
	[Protocol]
	public class TeamCopyTeamStatusNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100057;
		public UInt32 Sequence;
		public UInt32 teamId;
		public UInt32 teamStatus;

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
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
				BaseDLL.encode_uint32(buffer, ref pos_, teamStatus);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamStatus);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
				BaseDLL.encode_uint32(buffer, ref pos_, teamStatus);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamStatus);
			}

			public int getLen()
			{
				int _len = 0;
				// teamId
				_len += 4;
				// teamStatus
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  申请通知
	/// </summary>
	[Protocol]
	public class TeamCopyApplyNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100058;
		public UInt32 Sequence;
		public UInt32 IsHasApply;

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
				BaseDLL.encode_uint32(buffer, ref pos_, IsHasApply);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref IsHasApply);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, IsHasApply);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref IsHasApply);
			}

			public int getLen()
			{
				int _len = 0;
				// IsHasApply
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  玩家的团本数据
	/// </summary>
	[Protocol]
	public class TeamCopyPlayerInfoNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100059;
		public UInt32 Sequence;
		/// <summary>
		///  普通日次数
		/// </summary>
		public UInt32 dayNum;
		/// <summary>
		///  普通日总次数
		/// </summary>
		public UInt32 dayTotalNum;
		/// <summary>
		///  普通周次数
		/// </summary>
		public UInt32 weekNum;
		/// <summary>
		///  普通周总次数
		/// </summary>
		public UInt32 weekTotalNum;
		/// <summary>
		///  噩梦周次数
		/// </summary>
		public UInt32 diffWeekNum;
		/// <summary>
		///  噩梦周总次数
		/// </summary>
		public UInt32 diffWeekTotalNum;
		/// <summary>
		///  是否可以创建金团
		/// </summary>
		public UInt32 isCreateGold;
		/// <summary>
		///  日免费退出次数
		/// </summary>
		public UInt32 dayFreeQuitNum;
		/// <summary>
		///  周免费退出次数
		/// </summary>
		public UInt32 weekFreeQuitNum;
		/// <summary>
		///  门票是否足够
		/// </summary>
		public UInt32 ticketIsEnough;
		/// <summary>
		///  普通难度通关次数
		/// </summary>
		public UInt32 commonGradePassNum;
		/// <summary>
		///  解锁噩梦需要的普通难度次数
		/// </summary>
		public UInt32 unlockDiffGradeCommonNum;
		/// <summary>
		///  已开的难度
		/// </summary>
		public UInt32[] yetOpenGradeList = new UInt32[0];

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
				BaseDLL.encode_uint32(buffer, ref pos_, dayNum);
				BaseDLL.encode_uint32(buffer, ref pos_, dayTotalNum);
				BaseDLL.encode_uint32(buffer, ref pos_, weekNum);
				BaseDLL.encode_uint32(buffer, ref pos_, weekTotalNum);
				BaseDLL.encode_uint32(buffer, ref pos_, diffWeekNum);
				BaseDLL.encode_uint32(buffer, ref pos_, diffWeekTotalNum);
				BaseDLL.encode_uint32(buffer, ref pos_, isCreateGold);
				BaseDLL.encode_uint32(buffer, ref pos_, dayFreeQuitNum);
				BaseDLL.encode_uint32(buffer, ref pos_, weekFreeQuitNum);
				BaseDLL.encode_uint32(buffer, ref pos_, ticketIsEnough);
				BaseDLL.encode_uint32(buffer, ref pos_, commonGradePassNum);
				BaseDLL.encode_uint32(buffer, ref pos_, unlockDiffGradeCommonNum);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)yetOpenGradeList.Length);
				for(int i = 0; i < yetOpenGradeList.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, yetOpenGradeList[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dayNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dayTotalNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref weekNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref weekTotalNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref diffWeekNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref diffWeekTotalNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isCreateGold);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dayFreeQuitNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref weekFreeQuitNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref ticketIsEnough);
				BaseDLL.decode_uint32(buffer, ref pos_, ref commonGradePassNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref unlockDiffGradeCommonNum);
				UInt16 yetOpenGradeListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref yetOpenGradeListCnt);
				yetOpenGradeList = new UInt32[yetOpenGradeListCnt];
				for(int i = 0; i < yetOpenGradeList.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref yetOpenGradeList[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dayNum);
				BaseDLL.encode_uint32(buffer, ref pos_, dayTotalNum);
				BaseDLL.encode_uint32(buffer, ref pos_, weekNum);
				BaseDLL.encode_uint32(buffer, ref pos_, weekTotalNum);
				BaseDLL.encode_uint32(buffer, ref pos_, diffWeekNum);
				BaseDLL.encode_uint32(buffer, ref pos_, diffWeekTotalNum);
				BaseDLL.encode_uint32(buffer, ref pos_, isCreateGold);
				BaseDLL.encode_uint32(buffer, ref pos_, dayFreeQuitNum);
				BaseDLL.encode_uint32(buffer, ref pos_, weekFreeQuitNum);
				BaseDLL.encode_uint32(buffer, ref pos_, ticketIsEnough);
				BaseDLL.encode_uint32(buffer, ref pos_, commonGradePassNum);
				BaseDLL.encode_uint32(buffer, ref pos_, unlockDiffGradeCommonNum);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)yetOpenGradeList.Length);
				for(int i = 0; i < yetOpenGradeList.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, yetOpenGradeList[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dayNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dayTotalNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref weekNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref weekTotalNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref diffWeekNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref diffWeekTotalNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isCreateGold);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dayFreeQuitNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref weekFreeQuitNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref ticketIsEnough);
				BaseDLL.decode_uint32(buffer, ref pos_, ref commonGradePassNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref unlockDiffGradeCommonNum);
				UInt16 yetOpenGradeListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref yetOpenGradeListCnt);
				yetOpenGradeList = new UInt32[yetOpenGradeListCnt];
				for(int i = 0; i < yetOpenGradeList.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref yetOpenGradeList[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// dayNum
				_len += 4;
				// dayTotalNum
				_len += 4;
				// weekNum
				_len += 4;
				// weekTotalNum
				_len += 4;
				// diffWeekNum
				_len += 4;
				// diffWeekTotalNum
				_len += 4;
				// isCreateGold
				_len += 4;
				// dayFreeQuitNum
				_len += 4;
				// weekFreeQuitNum
				_len += 4;
				// ticketIsEnough
				_len += 4;
				// commonGradePassNum
				_len += 4;
				// unlockDiffGradeCommonNum
				_len += 4;
				// yetOpenGradeList
				_len += 2 + 4 * yetOpenGradeList.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  招募
	/// </summary>
	[Protocol]
	public class TeamCopyRecruitReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100060;
		public UInt32 Sequence;
		/// <summary>
		///  队伍模式
		/// </summary>
		public UInt32 teamModel;
		/// <summary>
		///  队伍Id
		/// </summary>
		public UInt32 teamId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, teamModel);
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamModel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, teamModel);
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamModel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
			}

			public int getLen()
			{
				int _len = 0;
				// teamModel
				_len += 4;
				// teamId
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class TeamCopyRecruitRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100061;
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
	///  链接加入
	/// </summary>
	[Protocol]
	public class TeamCopyLinkJoinReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100062;
		public UInt32 Sequence;
		/// <summary>
		///  队伍Id
		/// </summary>
		public UInt32 teamId;
		/// <summary>
		///  是否金主(非0是金主)
		/// </summary>
		public UInt32 isGold;

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
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
				BaseDLL.encode_uint32(buffer, ref pos_, isGold);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isGold);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
				BaseDLL.encode_uint32(buffer, ref pos_, isGold);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isGold);
			}

			public int getLen()
			{
				int _len = 0;
				// teamId
				_len += 4;
				// isGold
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class TeamCopyLinkJoinRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100063;
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
	///  据点状态通知
	/// </summary>
	[Protocol]
	public class TeamCopyFieldStatusNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100064;
		public UInt32 Sequence;
		/// <summary>
		///  据点id
		/// </summary>
		public UInt32 fieldId;
		/// <summary>
		///  据点状态
		/// </summary>
		public UInt32 fieldStatus;

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
				BaseDLL.encode_uint32(buffer, ref pos_, fieldId);
				BaseDLL.encode_uint32(buffer, ref pos_, fieldStatus);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref fieldId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref fieldStatus);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, fieldId);
				BaseDLL.encode_uint32(buffer, ref pos_, fieldStatus);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref fieldId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref fieldStatus);
			}

			public int getLen()
			{
				int _len = 0;
				// fieldId
				_len += 4;
				// fieldStatus
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  申请被拒绝通知
	/// </summary>
	[Protocol]
	public class TeamCopyApplyRefuseNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100065;
		public UInt32 Sequence;
		public string chiefName;

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
				byte[] chiefNameBytes = StringHelper.StringToUTF8Bytes(chiefName);
				BaseDLL.encode_string(buffer, ref pos_, chiefNameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 chiefNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref chiefNameLen);
				byte[] chiefNameBytes = new byte[chiefNameLen];
				for(int i = 0; i < chiefNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref chiefNameBytes[i]);
				}
				chiefName = StringHelper.BytesToString(chiefNameBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] chiefNameBytes = StringHelper.StringToUTF8Bytes(chiefName);
				BaseDLL.encode_string(buffer, ref pos_, chiefNameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 chiefNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref chiefNameLen);
				byte[] chiefNameBytes = new byte[chiefNameLen];
				for(int i = 0; i < chiefNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref chiefNameBytes[i]);
				}
				chiefName = StringHelper.BytesToString(chiefNameBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// chiefName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(chiefName);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  玩家离线通知
	/// </summary>
	[Protocol]
	public class TeamCopyPlayerExpireNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100066;
		public UInt32 Sequence;
		public UInt64 playerId;
		public UInt64 expireTime;

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
				BaseDLL.encode_uint64(buffer, ref pos_, expireTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref expireTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				BaseDLL.encode_uint64(buffer, ref pos_, expireTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref expireTime);
			}

			public int getLen()
			{
				int _len = 0;
				// playerId
				_len += 8;
				// expireTime
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  据点解锁比例通知
	/// </summary>
	[Protocol]
	public class TeamCopyFieldUnlockRate : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100067;
		public UInt32 Sequence;
		/// <summary>
		///  boss阶段
		/// </summary>
		public UInt32 bossPhase;
		/// <summary>
		///  boss血量比例
		/// </summary>
		public UInt32 bossBloodRate;
		/// <summary>
		///  据点id
		/// </summary>
		public UInt32 fieldId;
		/// <summary>
		///  比例
		/// </summary>
		public UInt32 unlockRate;

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
				BaseDLL.encode_uint32(buffer, ref pos_, bossPhase);
				BaseDLL.encode_uint32(buffer, ref pos_, bossBloodRate);
				BaseDLL.encode_uint32(buffer, ref pos_, fieldId);
				BaseDLL.encode_uint32(buffer, ref pos_, unlockRate);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref bossPhase);
				BaseDLL.decode_uint32(buffer, ref pos_, ref bossBloodRate);
				BaseDLL.decode_uint32(buffer, ref pos_, ref fieldId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref unlockRate);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, bossPhase);
				BaseDLL.encode_uint32(buffer, ref pos_, bossBloodRate);
				BaseDLL.encode_uint32(buffer, ref pos_, fieldId);
				BaseDLL.encode_uint32(buffer, ref pos_, unlockRate);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref bossPhase);
				BaseDLL.decode_uint32(buffer, ref pos_, ref bossBloodRate);
				BaseDLL.decode_uint32(buffer, ref pos_, ref fieldId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref unlockRate);
			}

			public int getLen()
			{
				int _len = 0;
				// bossPhase
				_len += 4;
				// bossBloodRate
				_len += 4;
				// fieldId
				_len += 4;
				// unlockRate
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  客户端上报阶段boss信息
	/// </summary>
	[Protocol]
	public class TeamCopyPhaseBossInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100068;
		public UInt32 Sequence;
		/// <summary>
		///  比赛Id
		/// </summary>
		public UInt64 raceId;
		/// <summary>
		///  角色Id
		/// </summary>
		public UInt64 roleId;
		/// <summary>
		///  当前帧
		/// </summary>
		public UInt32 curFrame;
		/// <summary>
		///  阶段
		/// </summary>
		public UInt32 phase;
		/// <summary>
		///  boss血量百分比
		/// </summary>
		public UInt32 bossBloodRate;

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
				BaseDLL.encode_uint64(buffer, ref pos_, raceId);
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint32(buffer, ref pos_, curFrame);
				BaseDLL.encode_uint32(buffer, ref pos_, phase);
				BaseDLL.encode_uint32(buffer, ref pos_, bossBloodRate);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref raceId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref curFrame);
				BaseDLL.decode_uint32(buffer, ref pos_, ref phase);
				BaseDLL.decode_uint32(buffer, ref pos_, ref bossBloodRate);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, raceId);
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint32(buffer, ref pos_, curFrame);
				BaseDLL.encode_uint32(buffer, ref pos_, phase);
				BaseDLL.encode_uint32(buffer, ref pos_, bossBloodRate);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref raceId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref curFrame);
				BaseDLL.decode_uint32(buffer, ref pos_, ref phase);
				BaseDLL.decode_uint32(buffer, ref pos_, ref bossBloodRate);
			}

			public int getLen()
			{
				int _len = 0;
				// raceId
				_len += 8;
				// roleId
				_len += 8;
				// curFrame
				_len += 4;
				// phase
				_len += 4;
				// bossBloodRate
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  强制结束标记
	/// </summary>
	[Protocol]
	public class TeamCopyForceEndFlag : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100069;
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
	///  强制投票结束
	/// </summary>
	[Protocol]
	public class TeamCopyForceEndReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100070;
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

	[Protocol]
	public class TeamCopyForceEndRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100071;
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
	///  强制结束投票通知
	/// </summary>
	[Protocol]
	public class TeamCopyForceEndVoteNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100072;
		public UInt32 Sequence;
		/// <summary>
		///  投票持续时间
		/// </summary>
		public UInt32 voteDurationTime;
		/// <summary>
		///  投票截止时间
		/// </summary>
		public UInt32 voteEndTime;

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
				BaseDLL.encode_uint32(buffer, ref pos_, voteDurationTime);
				BaseDLL.encode_uint32(buffer, ref pos_, voteEndTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref voteDurationTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref voteEndTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, voteDurationTime);
				BaseDLL.encode_uint32(buffer, ref pos_, voteEndTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref voteDurationTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref voteEndTime);
			}

			public int getLen()
			{
				int _len = 0;
				// voteDurationTime
				_len += 4;
				// voteEndTime
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  投票
	/// </summary>
	[Protocol]
	public class TeamCopyForceEndVoteReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100073;
		public UInt32 Sequence;
		/// <summary>
		///  非0同意，0拒绝
		/// </summary>
		public UInt32 vote;

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
				BaseDLL.encode_uint32(buffer, ref pos_, vote);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref vote);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, vote);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref vote);
			}

			public int getLen()
			{
				int _len = 0;
				// vote
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  有人投票
	/// </summary>
	[Protocol]
	public class TeamCopyForceEndMemberVote : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100074;
		public UInt32 Sequence;
		public UInt64 roleId;
		/// <summary>
		///  非0同意，0拒绝
		/// </summary>
		public UInt32 vote;

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
				BaseDLL.encode_uint32(buffer, ref pos_, vote);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref vote);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint32(buffer, ref pos_, vote);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref vote);
			}

			public int getLen()
			{
				int _len = 0;
				// roleId
				_len += 8;
				// vote
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  强制投票结果
	/// </summary>
	[Protocol]
	public class TeamCopyForceEndVoteResult : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100075;
		public UInt32 Sequence;
		/// <summary>
		///  返回结果(0成功，非0失败)
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
	///  修改队伍装备评分请求
	/// </summary>
	[Protocol]
	public class TeamCopyModifyEquipScoreReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100076;
		public UInt32 Sequence;
		public UInt32 equipScore;

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
				BaseDLL.encode_uint32(buffer, ref pos_, equipScore);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref equipScore);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, equipScore);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref equipScore);
			}

			public int getLen()
			{
				int _len = 0;
				// equipScore
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  修改队伍装备评分返回
	/// </summary>
	[Protocol]
	public class TeamCopyModifyEquipScoreRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 1100077;
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

}
