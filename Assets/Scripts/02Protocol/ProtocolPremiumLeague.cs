using System;
using System.Text;

namespace Protocol
{
	/// <summary>
	///  �ͽ�����״̬
	/// </summary>
	public enum PremiumLeagueStatus
	{
		/// <summary>
		///  ��ʼ״̬
		/// </summary>
		PLS_INIT = 0,
		/// <summary>
		///  ����
		/// </summary>
		PLS_ENROLL = 1,
		/// <summary>
		///  Ԥ��
		/// </summary>
		PLS_PRELIMINAY = 2,
		/// <summary>
		///  ��ǿ׼��
		/// </summary>
		PLS_FINAL_EIGHT_PREPARE = 3,
		/// <summary>
		///  ��ǿ��
		/// </summary>
		PLS_FINAL_EIGHT = 4,
		/// <summary>
		///  ��ǿ��
		/// </summary>
		PLS_FINAL_FOUR = 5,
		/// <summary>
		///  ����
		/// </summary>
		PLS_FINAL = 6,
		/// <summary>
		///  ���������ȴ����
		/// </summary>
		PLS_FINAL_WAIT_CLEAR = 7,
	}

	/// <summary>
	///  �ͽ�������������
	/// </summary>
	public enum PremiumLeagueRewardType
	{
		/// <summary>
		///  ��1���Ľ���
		/// </summary>
		PL_REWARD_NO_1 = 0,
		/// <summary>
		///  ��2���Ľ���
		/// </summary>
		PL_REWARD_NO_2 = 1,
		/// <summary>
		///  3-4���Ľ���
		/// </summary>
		PL_REWARD_NO_3_4 = 2,
		/// <summary>
		///  5-8���Ľ���
		/// </summary>
		PL_REWARD_NO_5_8 = 3,
		/// <summary>
		///  9-20���Ľ���
		/// </summary>
		PL_REWARD_NO_9_20 = 4,
	}

	/// <summary>
	///  �ͽ�����״̬��Ϣ
	/// </summary>
	public class PremiumLeagueStatusInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  ״̬����ӦPremiumLeagueStatus��
		/// </summary>
		public byte status;
		/// <summary>
		///  ��ʼʱ��
		/// </summary>
		public UInt32 startTime;
		/// <summary>
		///  ����ʱ��
		/// </summary>
		public UInt32 endTime;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, status);
				BaseDLL.encode_uint32(buffer, ref pos_, startTime);
				BaseDLL.encode_uint32(buffer, ref pos_, endTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
				BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, status);
				BaseDLL.encode_uint32(buffer, ref pos_, startTime);
				BaseDLL.encode_uint32(buffer, ref pos_, endTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
				BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
			}

			public int getLen()
			{
				int _len = 0;
				// status
				_len += 1;
				// startTime
				_len += 4;
				// endTime
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  ��̭�������Ϣ
	/// </summary>
	public class PremiumLeagueBattleGamer : Protocol.IProtocolStream
	{
		/// <summary>
		///  ��ɫID
		/// </summary>
		public UInt64 roleId;
		/// <summary>
		///  ����
		/// </summary>
		public string name;
		/// <summary>
		///  ְҵ
		/// </summary>
		public byte occu;
		/// <summary>
		///  ����
		/// </summary>
		public UInt32 ranking;
		/// <summary>
		///  ʤ����
		/// </summary>
		public UInt32 winNum;
		/// <summary>
		///  �Ƿ��Ѿ�����
		/// </summary>
		public byte isLose;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_uint32(buffer, ref pos_, ranking);
				BaseDLL.encode_uint32(buffer, ref pos_, winNum);
				BaseDLL.encode_int8(buffer, ref pos_, isLose);
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
				BaseDLL.decode_uint32(buffer, ref pos_, ref ranking);
				BaseDLL.decode_uint32(buffer, ref pos_, ref winNum);
				BaseDLL.decode_int8(buffer, ref pos_, ref isLose);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_uint32(buffer, ref pos_, ranking);
				BaseDLL.encode_uint32(buffer, ref pos_, winNum);
				BaseDLL.encode_int8(buffer, ref pos_, isLose);
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
				BaseDLL.decode_uint32(buffer, ref pos_, ref ranking);
				BaseDLL.decode_uint32(buffer, ref pos_, ref winNum);
				BaseDLL.decode_int8(buffer, ref pos_, ref isLose);
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
				// ranking
				_len += 4;
				// winNum
				_len += 4;
				// isLose
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  �Լ����ͽ�������Ϣ
	/// </summary>
	public class PremiumLeagueSelfInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  ʤ����
		/// </summary>
		public UInt32 winNum;
		/// <summary>
		///  ����
		/// </summary>
		public UInt32 score;
		/// <summary>
		///  ����
		/// </summary>
		public UInt32 ranking;
		/// <summary>
		///  ��������
		/// </summary>
		public UInt32 enrollCount;
		/// <summary>
		///  ������
		/// </summary>
		public UInt32 loseNum;
		/// <summary>
		///  Ԥѡ����õĽ���
		/// </summary>
		public UInt32 preliminayRewardNum;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, winNum);
				BaseDLL.encode_uint32(buffer, ref pos_, score);
				BaseDLL.encode_uint32(buffer, ref pos_, ranking);
				BaseDLL.encode_uint32(buffer, ref pos_, enrollCount);
				BaseDLL.encode_uint32(buffer, ref pos_, loseNum);
				BaseDLL.encode_uint32(buffer, ref pos_, preliminayRewardNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref winNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref score);
				BaseDLL.decode_uint32(buffer, ref pos_, ref ranking);
				BaseDLL.decode_uint32(buffer, ref pos_, ref enrollCount);
				BaseDLL.decode_uint32(buffer, ref pos_, ref loseNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref preliminayRewardNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, winNum);
				BaseDLL.encode_uint32(buffer, ref pos_, score);
				BaseDLL.encode_uint32(buffer, ref pos_, ranking);
				BaseDLL.encode_uint32(buffer, ref pos_, enrollCount);
				BaseDLL.encode_uint32(buffer, ref pos_, loseNum);
				BaseDLL.encode_uint32(buffer, ref pos_, preliminayRewardNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref winNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref score);
				BaseDLL.decode_uint32(buffer, ref pos_, ref ranking);
				BaseDLL.decode_uint32(buffer, ref pos_, ref enrollCount);
				BaseDLL.decode_uint32(buffer, ref pos_, ref loseNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref preliminayRewardNum);
			}

			public int getLen()
			{
				int _len = 0;
				// winNum
				_len += 4;
				// score
				_len += 4;
				// ranking
				_len += 4;
				// enrollCount
				_len += 4;
				// loseNum
				_len += 4;
				// preliminayRewardNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  ս����¼��Ա
	/// </summary>
	public class PremiumLeagueRecordFighter : Protocol.IProtocolStream
	{
		/// <summary>
		///  id
		/// </summary>
		public UInt64 id;
		/// <summary>
		///  ����
		/// </summary>
		public string name;
		/// <summary>
		///  ��:ս��ǰ����ʤ�� Ӯ:ս������ʤ
		/// </summary>
		public byte winStreak;
		/// <summary>
		///  ��û���
		/// </summary>
		public UInt16 gotScore;
		/// <summary>
		///  �ܻ���
		/// </summary>
		public UInt16 totalScore;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, winStreak);
				BaseDLL.encode_uint16(buffer, ref pos_, gotScore);
				BaseDLL.encode_uint16(buffer, ref pos_, totalScore);
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
				BaseDLL.decode_int8(buffer, ref pos_, ref winStreak);
				BaseDLL.decode_uint16(buffer, ref pos_, ref gotScore);
				BaseDLL.decode_uint16(buffer, ref pos_, ref totalScore);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, winStreak);
				BaseDLL.encode_uint16(buffer, ref pos_, gotScore);
				BaseDLL.encode_uint16(buffer, ref pos_, totalScore);
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
				BaseDLL.decode_int8(buffer, ref pos_, ref winStreak);
				BaseDLL.decode_uint16(buffer, ref pos_, ref gotScore);
				BaseDLL.decode_uint16(buffer, ref pos_, ref totalScore);
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
				// winStreak
				_len += 1;
				// gotScore
				_len += 2;
				// totalScore
				_len += 2;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  ս����¼
	/// </summary>
	public class PremiumLeagueRecordEntry : Protocol.IProtocolStream
	{
		/// <summary>
		///  ���
		/// </summary>
		public UInt32 index;
		/// <summary>
		///  ʱ��
		/// </summary>
		public UInt32 time;
		/// <summary>
		///  ʤ����
		/// </summary>
		public PremiumLeagueRecordFighter winner = new PremiumLeagueRecordFighter();
		/// <summary>
		///  ʧ����
		/// </summary>
		public PremiumLeagueRecordFighter loser = new PremiumLeagueRecordFighter();

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, index);
				BaseDLL.encode_uint32(buffer, ref pos_, time);
				winner.encode(buffer, ref pos_);
				loser.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
				BaseDLL.decode_uint32(buffer, ref pos_, ref time);
				winner.decode(buffer, ref pos_);
				loser.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, index);
				BaseDLL.encode_uint32(buffer, ref pos_, time);
				winner.encode(buffer, ref pos_);
				loser.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
				BaseDLL.decode_uint32(buffer, ref pos_, ref time);
				winner.decode(buffer, ref pos_);
				loser.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// index
				_len += 4;
				// time
				_len += 4;
				// winner
				_len += winner.getLen();
				// loser
				_len += loser.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  �ͽ�������̭����Ա��Ϣ
	/// </summary>
	public class PremiumLeagueBattleFighter : Protocol.IProtocolStream
	{
		/// <summary>
		///  ��ɫID
		/// </summary>
		public UInt64 id;
		/// <summary>
		///  ����
		/// </summary>
		public string name;
		/// <summary>
		///  ְҵ
		/// </summary>
		public byte occu;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occu);
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
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occu);
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
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  �ͽ�������̭��
	/// </summary>
	public class CLPremiumLeagueBattle : Protocol.IProtocolStream
	{
		/// <summary>
		///  ����ID
		/// </summary>
		public UInt64 raceId;
		/// <summary>
		///  ��������
		/// </summary>
		public byte type;
		/// <summary>
		///  ��Ա1
		/// </summary>
		public PremiumLeagueBattleFighter fighter1 = new PremiumLeagueBattleFighter();
		/// <summary>
		///  ��Ա2
		/// </summary>
		public PremiumLeagueBattleFighter fighter2 = new PremiumLeagueBattleFighter();
		/// <summary>
		///  �Ƿ��Ѿ�������
		/// </summary>
		public byte isEnd;
		/// <summary>
		///  ʤ��ID
		/// </summary>
		public UInt64 winnerId;
		/// <summary>
		///  relay��ַ
		/// </summary>
		public SockAddr relayAddr = new SockAddr();

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, raceId);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				fighter1.encode(buffer, ref pos_);
				fighter2.encode(buffer, ref pos_);
				BaseDLL.encode_int8(buffer, ref pos_, isEnd);
				BaseDLL.encode_uint64(buffer, ref pos_, winnerId);
				relayAddr.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref raceId);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				fighter1.decode(buffer, ref pos_);
				fighter2.decode(buffer, ref pos_);
				BaseDLL.decode_int8(buffer, ref pos_, ref isEnd);
				BaseDLL.decode_uint64(buffer, ref pos_, ref winnerId);
				relayAddr.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, raceId);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				fighter1.encode(buffer, ref pos_);
				fighter2.encode(buffer, ref pos_);
				BaseDLL.encode_int8(buffer, ref pos_, isEnd);
				BaseDLL.encode_uint64(buffer, ref pos_, winnerId);
				relayAddr.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref raceId);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				fighter1.decode(buffer, ref pos_);
				fighter2.decode(buffer, ref pos_);
				BaseDLL.decode_int8(buffer, ref pos_, ref isEnd);
				BaseDLL.decode_uint64(buffer, ref pos_, ref winnerId);
				relayAddr.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// raceId
				_len += 8;
				// type
				_len += 1;
				// fighter1
				_len += fighter1.getLen();
				// fighter2
				_len += fighter2.getLen();
				// isEnd
				_len += 1;
				// winnerId
				_len += 8;
				// relayAddr
				_len += relayAddr.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  ͬ���ͽ�����״̬
	/// </summary>
	[Protocol]
	public class WorldPremiumLeagueSyncStatus : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607701;
		public UInt32 Sequence;
		public PremiumLeagueStatusInfo info = new PremiumLeagueStatusInfo();

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
	///  �����ͽ���������
	/// </summary>
	[Protocol]
	public class WorldPremiumLeagueRewardPoolReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607702;
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
	///  �����ͽ���������
	/// </summary>
	[Protocol]
	public class WorldPremiumLeagueRewardPoolRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607703;
		public UInt32 Sequence;
		/// <summary>
		///  ��������
		/// </summary>
		public UInt32 enrollPlayerNum;
		/// <summary>
		///  ������
		/// </summary>
		public UInt32 money;
		/// <summary>
		///  ����������
		/// </summary>
		public UInt32[] rewards = new UInt32[5];

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
				BaseDLL.encode_uint32(buffer, ref pos_, enrollPlayerNum);
				BaseDLL.encode_uint32(buffer, ref pos_, money);
				for(int i = 0; i < rewards.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, rewards[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref enrollPlayerNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref money);
				for(int i = 0; i < rewards.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref rewards[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, enrollPlayerNum);
				BaseDLL.encode_uint32(buffer, ref pos_, money);
				for(int i = 0; i < rewards.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, rewards[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref enrollPlayerNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref money);
				for(int i = 0; i < rewards.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref rewards[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// enrollPlayerNum
				_len += 4;
				// money
				_len += 4;
				// rewards
				_len += 4 * rewards.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  �ͽ�������������
	/// </summary>
	[Protocol]
	public class WorldPremiumLeagueEnrollReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607704;
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
	///  �ͽ�������������
	/// </summary>
	[Protocol]
	public class WorldPremiumLeagueEnrollRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607705;
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
	///  ��ʼ����̭������б�
	/// </summary>
	[Protocol]
	public class WorldPremiumLeagueBattleGamerInit : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607706;
		public UInt32 Sequence;
		public PremiumLeagueBattleGamer[] gamers = new PremiumLeagueBattleGamer[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)gamers.Length);
				for(int i = 0; i < gamers.Length; i++)
				{
					gamers[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 gamersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref gamersCnt);
				gamers = new PremiumLeagueBattleGamer[gamersCnt];
				for(int i = 0; i < gamers.Length; i++)
				{
					gamers[i] = new PremiumLeagueBattleGamer();
					gamers[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)gamers.Length);
				for(int i = 0; i < gamers.Length; i++)
				{
					gamers[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 gamersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref gamersCnt);
				gamers = new PremiumLeagueBattleGamer[gamersCnt];
				for(int i = 0; i < gamers.Length; i++)
				{
					gamers[i] = new PremiumLeagueBattleGamer();
					gamers[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// gamers
				_len += 2;
				for(int j = 0; j < gamers.Length; j++)
				{
					_len += gamers[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  ������̭�������Ϣ
	/// </summary>
	[Protocol]
	public class WorldPremiumLeagueBattleGamerUpdate : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607707;
		public UInt32 Sequence;
		public UInt64 roleId;
		public UInt32 winNum;
		public byte isLose;

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
				BaseDLL.encode_uint32(buffer, ref pos_, winNum);
				BaseDLL.encode_int8(buffer, ref pos_, isLose);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref winNum);
				BaseDLL.decode_int8(buffer, ref pos_, ref isLose);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint32(buffer, ref pos_, winNum);
				BaseDLL.encode_int8(buffer, ref pos_, isLose);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref winNum);
				BaseDLL.decode_int8(buffer, ref pos_, ref isLose);
			}

			public int getLen()
			{
				int _len = 0;
				// roleId
				_len += 8;
				// winNum
				_len += 4;
				// isLose
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  ������̭�������Ϣ
	/// </summary>
	[Protocol]
	public class WorldPremiumLeagueSelfInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607708;
		public UInt32 Sequence;
		public PremiumLeagueSelfInfo info = new PremiumLeagueSelfInfo();

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
	///  �����ͽ�����ս����¼
	/// </summary>
	[Protocol]
	public class WorldPremiumLeagueBattleRecordReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607709;
		public UInt32 Sequence;
		public byte isSelf;
		public UInt32 startIndex;
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
				BaseDLL.encode_int8(buffer, ref pos_, isSelf);
				BaseDLL.encode_uint32(buffer, ref pos_, startIndex);
				BaseDLL.encode_uint32(buffer, ref pos_, count);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isSelf);
				BaseDLL.decode_uint32(buffer, ref pos_, ref startIndex);
				BaseDLL.decode_uint32(buffer, ref pos_, ref count);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, isSelf);
				BaseDLL.encode_uint32(buffer, ref pos_, startIndex);
				BaseDLL.encode_uint32(buffer, ref pos_, count);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isSelf);
				BaseDLL.decode_uint32(buffer, ref pos_, ref startIndex);
				BaseDLL.decode_uint32(buffer, ref pos_, ref count);
			}

			public int getLen()
			{
				int _len = 0;
				// isSelf
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
	///  �����ͽ�����ս����¼
	/// </summary>
	[Protocol]
	public class WorldPremiumLeagueBattleRecordRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607710;
		public UInt32 Sequence;
		public UInt32 totalCount;
		public PremiumLeagueRecordEntry[] records = new PremiumLeagueRecordEntry[0];

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
				BaseDLL.encode_uint32(buffer, ref pos_, totalCount);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)records.Length);
				for(int i = 0; i < records.Length; i++)
				{
					records[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalCount);
				UInt16 recordsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref recordsCnt);
				records = new PremiumLeagueRecordEntry[recordsCnt];
				for(int i = 0; i < records.Length; i++)
				{
					records[i] = new PremiumLeagueRecordEntry();
					records[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, totalCount);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)records.Length);
				for(int i = 0; i < records.Length; i++)
				{
					records[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalCount);
				UInt16 recordsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref recordsCnt);
				records = new PremiumLeagueRecordEntry[recordsCnt];
				for(int i = 0; i < records.Length; i++)
				{
					records[i] = new PremiumLeagueRecordEntry();
					records[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// totalCount
				_len += 4;
				// records
				_len += 2;
				for(int j = 0; j < records.Length; j++)
				{
					_len += records[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  ͬ���ͽ�����ս����¼
	/// </summary>
	[Protocol]
	public class WorldPremiumLeagueBattleRecordSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607711;
		public UInt32 Sequence;
		public PremiumLeagueRecordEntry record = new PremiumLeagueRecordEntry();

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
				record.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				record.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				record.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				record.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// record
				_len += record.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  �ͽ���������
	/// </summary>
	[Protocol]
	public class WorldPremiumLeagueRaceEnd : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607712;
		public UInt32 Sequence;
		/// <summary>
		///  �ǲ���Ԥѡ��
		/// </summary>
		public byte isPreliminay;
		/// <summary>
		///  ս�����
		/// </summary>
		public byte result;
		/// <summary>
		///  ԭ�л���
		/// </summary>
		public UInt32 oldScore;
		/// <summary>
		///  �»���
		/// </summary>
		public UInt32 newScore;
		/// <summary>
		///  ��������
		/// </summary>
		public UInt32 preliminayRewardNum;
		/// <summary>
		///  �������
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
				BaseDLL.encode_int8(buffer, ref pos_, isPreliminay);
				BaseDLL.encode_int8(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, oldScore);
				BaseDLL.encode_uint32(buffer, ref pos_, newScore);
				BaseDLL.encode_uint32(buffer, ref pos_, preliminayRewardNum);
				BaseDLL.encode_uint32(buffer, ref pos_, getHonor);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isPreliminay);
				BaseDLL.decode_int8(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref preliminayRewardNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref getHonor);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, isPreliminay);
				BaseDLL.encode_int8(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, oldScore);
				BaseDLL.encode_uint32(buffer, ref pos_, newScore);
				BaseDLL.encode_uint32(buffer, ref pos_, preliminayRewardNum);
				BaseDLL.encode_uint32(buffer, ref pos_, getHonor);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isPreliminay);
				BaseDLL.decode_int8(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref preliminayRewardNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref getHonor);
			}

			public int getLen()
			{
				int _len = 0;
				// isPreliminay
				_len += 1;
				// result
				_len += 1;
				// oldScore
				_len += 4;
				// newScore
				_len += 4;
				// preliminayRewardNum
				_len += 4;
				// getHonor
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  ��ʼ����̭����ս�б�
	/// </summary>
	[Protocol]
	public class WorldPremiumLeagueBattleInfoInit : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607713;
		public UInt32 Sequence;
		public CLPremiumLeagueBattle[] battles = new CLPremiumLeagueBattle[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)battles.Length);
				for(int i = 0; i < battles.Length; i++)
				{
					battles[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 battlesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref battlesCnt);
				battles = new CLPremiumLeagueBattle[battlesCnt];
				for(int i = 0; i < battles.Length; i++)
				{
					battles[i] = new CLPremiumLeagueBattle();
					battles[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)battles.Length);
				for(int i = 0; i < battles.Length; i++)
				{
					battles[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 battlesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref battlesCnt);
				battles = new CLPremiumLeagueBattle[battlesCnt];
				for(int i = 0; i < battles.Length; i++)
				{
					battles[i] = new CLPremiumLeagueBattle();
					battles[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// battles
				_len += 2;
				for(int j = 0; j < battles.Length; j++)
				{
					_len += battles[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  ������̭����ս��Ϣ
	/// </summary>
	[Protocol]
	public class WorldPremiumLeagueBattleInfoUpdate : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607714;
		public UInt32 Sequence;
		public CLPremiumLeagueBattle battle = new CLPremiumLeagueBattle();

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
				battle.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				battle.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				battle.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				battle.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// battle
				_len += battle.getLen();
				return _len;
			}
		#endregion

	}

}
