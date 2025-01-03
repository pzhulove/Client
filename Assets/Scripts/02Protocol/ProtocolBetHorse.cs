using System;
using System.Text;

namespace Protocol
{
	public enum WeatherType
	{
		WEATHER_RAIN = 1,
		/// <summary>
		///  雨天
		/// </summary>
		WEATHER_SUNNY = 2,
		/// <summary>
		///  晴天
		/// </summary>
		WEATHER_FOGGY = 3,
	}

	/// <summary>
	///  雾天
	/// </summary>
	public enum ShooterStatusType
	{
		SHOOTER_STATUS_UNKNOWN = 0,
		/// <summary>
		/// 未知
		/// </summary>

		SHOOTER_STATUS_EXCELLENT = 1,
		/// <summary>
		///  优秀(红色)
		/// </summary>
		SHOOTER_STATUS_GOOD = 2,
		/// <summary>
		///  良好(黄色)
		/// </summary>
		SHOOTER_STATUS_INSTABLE = 3,
		/// <summary>
		///  不稳定(蓝色)
		/// </summary>
		SHOOTER_STATUS_COMMONLY = 4,
	}

	/// <summary>
	///  表现平平(绿色)
	/// </summary>
	public enum BetHorsePhaseType
	{
		PHASE_TYPE_READY = 1,
		/// <summary>
		///  准备
		/// </summary>
		PHASE_TYPE_STAKE = 2,
		/// <summary>
		///  押注阶段(1-90分钟)
		/// </summary>
		PHASE_TYPE_ADJUST = 3,
		/// <summary>
		///  调整阶段(91-100分钟)
		/// </summary>
		PHASE_TYPE_RESULT = 4,
		/// <summary>
		///  结果阶段(101-120分钟)
		/// </summary>
		PHASE_TYPE_DAY_END = 5,
	}

	/// <summary>
	///  今日比赛结束
	/// </summary>
	public class MapInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  地图id
		/// </summary>
		public UInt32 id;
		/// <summary>
		///  地形
		/// </summary>
		public UInt32 terrain;
		/// <summary>
		///  胜利射手id(0未结束)
		/// </summary>
		public UInt32 winShooterId;
		/// <summary>
		///  地图上的射手
		/// </summary>
		public UInt32[] shooter = new UInt32[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, terrain);
				BaseDLL.encode_uint32(buffer, ref pos_, winShooterId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)shooter.Length);
				for(int i = 0; i < shooter.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, shooter[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref terrain);
				BaseDLL.decode_uint32(buffer, ref pos_, ref winShooterId);
				UInt16 shooterCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref shooterCnt);
				shooter = new UInt32[shooterCnt];
				for(int i = 0; i < shooter.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref shooter[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, terrain);
				BaseDLL.encode_uint32(buffer, ref pos_, winShooterId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)shooter.Length);
				for(int i = 0; i < shooter.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, shooter[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref terrain);
				BaseDLL.decode_uint32(buffer, ref pos_, ref winShooterId);
				UInt16 shooterCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref shooterCnt);
				shooter = new UInt32[shooterCnt];
				for(int i = 0; i < shooter.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref shooter[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// terrain
				_len += 4;
				// winShooterId
				_len += 4;
				// shooter
				_len += 2 + 4 * shooter.Length;
				return _len;
			}
		#endregion

	}

	public class ShooterInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  射手id(唯一id)
		/// </summary>
		public UInt32 id;
		/// <summary>
		///  dataid
		/// </summary>
		public UInt32 dataid;
		/// <summary>
		///  状态(ShooterStatusType)
		/// </summary>
		public UInt32 status;
		/// <summary>
		///  赔率
		/// </summary>
		public UInt32 odds;
		/// <summary>
		///  胜率
		/// </summary>
		public UInt32 winRate;
		/// <summary>
		///  吃鸡数(夺冠次数)
		/// </summary>
		public UInt32 champion;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, dataid);
				BaseDLL.encode_uint32(buffer, ref pos_, status);
				BaseDLL.encode_uint32(buffer, ref pos_, odds);
				BaseDLL.encode_uint32(buffer, ref pos_, winRate);
				BaseDLL.encode_uint32(buffer, ref pos_, champion);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref status);
				BaseDLL.decode_uint32(buffer, ref pos_, ref odds);
				BaseDLL.decode_uint32(buffer, ref pos_, ref winRate);
				BaseDLL.decode_uint32(buffer, ref pos_, ref champion);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, dataid);
				BaseDLL.encode_uint32(buffer, ref pos_, status);
				BaseDLL.encode_uint32(buffer, ref pos_, odds);
				BaseDLL.encode_uint32(buffer, ref pos_, winRate);
				BaseDLL.encode_uint32(buffer, ref pos_, champion);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref status);
				BaseDLL.decode_uint32(buffer, ref pos_, ref odds);
				BaseDLL.decode_uint32(buffer, ref pos_, ref winRate);
				BaseDLL.decode_uint32(buffer, ref pos_, ref champion);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// dataid
				_len += 4;
				// status
				_len += 4;
				// odds
				_len += 4;
				// winRate
				_len += 4;
				// champion
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  赌马主界面请求
	/// </summary>
	[Protocol]
	public class BetHorseReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 708301;
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
	public class BetHorseRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 708302;
		public UInt32 Sequence;
		/// <summary>
		///  当前天气(WeatherType)
		/// </summary>
		public UInt32 weather;
		/// <summary>
		///  神秘射手
		/// </summary>
		public UInt32 mysteryShooter;
		/// <summary>
		///  赌马阶段
		/// </summary>
		public UInt32 phase;
		/// <summary>
		///  时间戳
		/// </summary>
		public UInt32 stamp;
		/// <summary>
		///  射手列表
		/// </summary>
		public ShooterInfo[] shooterList = new ShooterInfo[0];
		/// <summary>
		///  地图列表
		/// </summary>
		public MapInfo[] mapList = new MapInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, weather);
				BaseDLL.encode_uint32(buffer, ref pos_, mysteryShooter);
				BaseDLL.encode_uint32(buffer, ref pos_, phase);
				BaseDLL.encode_uint32(buffer, ref pos_, stamp);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)shooterList.Length);
				for(int i = 0; i < shooterList.Length; i++)
				{
					shooterList[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mapList.Length);
				for(int i = 0; i < mapList.Length; i++)
				{
					mapList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref weather);
				BaseDLL.decode_uint32(buffer, ref pos_, ref mysteryShooter);
				BaseDLL.decode_uint32(buffer, ref pos_, ref phase);
				BaseDLL.decode_uint32(buffer, ref pos_, ref stamp);
				UInt16 shooterListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref shooterListCnt);
				shooterList = new ShooterInfo[shooterListCnt];
				for(int i = 0; i < shooterList.Length; i++)
				{
					shooterList[i] = new ShooterInfo();
					shooterList[i].decode(buffer, ref pos_);
				}
				UInt16 mapListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref mapListCnt);
				mapList = new MapInfo[mapListCnt];
				for(int i = 0; i < mapList.Length; i++)
				{
					mapList[i] = new MapInfo();
					mapList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, weather);
				BaseDLL.encode_uint32(buffer, ref pos_, mysteryShooter);
				BaseDLL.encode_uint32(buffer, ref pos_, phase);
				BaseDLL.encode_uint32(buffer, ref pos_, stamp);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)shooterList.Length);
				for(int i = 0; i < shooterList.Length; i++)
				{
					shooterList[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mapList.Length);
				for(int i = 0; i < mapList.Length; i++)
				{
					mapList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref weather);
				BaseDLL.decode_uint32(buffer, ref pos_, ref mysteryShooter);
				BaseDLL.decode_uint32(buffer, ref pos_, ref phase);
				BaseDLL.decode_uint32(buffer, ref pos_, ref stamp);
				UInt16 shooterListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref shooterListCnt);
				shooterList = new ShooterInfo[shooterListCnt];
				for(int i = 0; i < shooterList.Length; i++)
				{
					shooterList[i] = new ShooterInfo();
					shooterList[i].decode(buffer, ref pos_);
				}
				UInt16 mapListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref mapListCnt);
				mapList = new MapInfo[mapListCnt];
				for(int i = 0; i < mapList.Length; i++)
				{
					mapList[i] = new MapInfo();
					mapList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// weather
				_len += 4;
				// mysteryShooter
				_len += 4;
				// phase
				_len += 4;
				// stamp
				_len += 4;
				// shooterList
				_len += 2;
				for(int j = 0; j < shooterList.Length; j++)
				{
					_len += shooterList[j].getLen();
				}
				// mapList
				_len += 2;
				for(int j = 0; j < mapList.Length; j++)
				{
					_len += mapList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  *****************************************************
	/// </summary>
	/// <summary>
	///  请求射手赔率
	/// </summary>
	[Protocol]
	public class ShooterOddsReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 708303;
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

	public class OddsRateInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  射手id
		/// </summary>
		public UInt32 id;
		/// <summary>
		///  赔率
		/// </summary>
		public UInt32 odds;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, odds);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref odds);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, odds);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref odds);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// odds
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class ShooterOddsRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 708304;
		public UInt32 Sequence;
		/// <summary>
		///  赔率列表
		/// </summary>
		public OddsRateInfo[] oddsList = new OddsRateInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)oddsList.Length);
				for(int i = 0; i < oddsList.Length; i++)
				{
					oddsList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 oddsListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref oddsListCnt);
				oddsList = new OddsRateInfo[oddsListCnt];
				for(int i = 0; i < oddsList.Length; i++)
				{
					oddsList[i] = new OddsRateInfo();
					oddsList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)oddsList.Length);
				for(int i = 0; i < oddsList.Length; i++)
				{
					oddsList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 oddsListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref oddsListCnt);
				oddsList = new OddsRateInfo[oddsListCnt];
				for(int i = 0; i < oddsList.Length; i++)
				{
					oddsList[i] = new OddsRateInfo();
					oddsList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// oddsList
				_len += 2;
				for(int j = 0; j < oddsList.Length; j++)
				{
					_len += oddsList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  ************************************************************
	/// </summary>
	/// <summary>
	///  射手的历史战绩
	/// </summary>
	[Protocol]
	public class ShooterHistoryRecordReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 708305;
		public UInt32 Sequence;
		/// <summary>
		///  射手id
		/// </summary>
		public UInt32 id;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class ShooterRecord : Protocol.IProtocolStream
	{
		/// <summary>
		///  场次
		/// </summary>
		public UInt32 coutrId;
		/// <summary>
		///  自己的赔率
		/// </summary>
		public UInt32 odds;
		/// <summary>
		///  胜负结果
		/// </summary>
		public UInt32 result;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, coutrId);
				BaseDLL.encode_uint32(buffer, ref pos_, odds);
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref coutrId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref odds);
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, coutrId);
				BaseDLL.encode_uint32(buffer, ref pos_, odds);
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref coutrId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref odds);
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// coutrId
				_len += 4;
				// odds
				_len += 4;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class ShooterHistoryRecordRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 708306;
		public UInt32 Sequence;
		/// <summary>
		///  射手id
		/// </summary>
		public UInt32 id;
		/// <summary>
		///  历史战绩列表
		/// </summary>
		public ShooterRecord[] recordList = new ShooterRecord[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)recordList.Length);
				for(int i = 0; i < recordList.Length; i++)
				{
					recordList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				UInt16 recordListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref recordListCnt);
				recordList = new ShooterRecord[recordListCnt];
				for(int i = 0; i < recordList.Length; i++)
				{
					recordList[i] = new ShooterRecord();
					recordList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)recordList.Length);
				for(int i = 0; i < recordList.Length; i++)
				{
					recordList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				UInt16 recordListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref recordListCnt);
				recordList = new ShooterRecord[recordListCnt];
				for(int i = 0; i < recordList.Length; i++)
				{
					recordList[i] = new ShooterRecord();
					recordList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// recordList
				_len += 2;
				for(int j = 0; j < recordList.Length; j++)
				{
					_len += recordList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// *****************************************************************
	/// </summary>
	/// <summary>
	///  玩家押注
	/// </summary>
	[Protocol]
	public class stakeReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 708307;
		public UInt32 Sequence;
		/// <summary>
		///  押注的射手
		/// </summary>
		public UInt32 id;
		/// <summary>
		///  押注子弹数量
		/// </summary>
		public UInt32 num;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// num
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class stakeRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 708308;
		public UInt32 Sequence;
		/// <summary>
		///  押注结果
		/// </summary>
		public UInt32 ret;
		/// <summary>
		///  押注的射手
		/// </summary>
		public UInt32 id;
		/// <summary>
		///  押注子弹数量
		/// </summary>
		public UInt32 num;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, ret);
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, ret);
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// ret
				_len += 4;
				// id
				_len += 4;
				// num
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  *************************************************************
	/// </summary>
	/// <summary>
	///  阶段同步
	/// </summary>
	[Protocol]
	public class BetHorsePhaseSycn : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 708309;
		public UInt32 Sequence;
		/// <summary>
		///  赌马阶段同步(BetHorsePhaseType)
		/// </summary>
		public UInt32 phaseSycn;
		/// <summary>
		///  时间戳
		/// </summary>
		public UInt32 stamp;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, phaseSycn);
				BaseDLL.encode_uint32(buffer, ref pos_, stamp);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref phaseSycn);
				BaseDLL.decode_uint32(buffer, ref pos_, ref stamp);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, phaseSycn);
				BaseDLL.encode_uint32(buffer, ref pos_, stamp);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref phaseSycn);
				BaseDLL.decode_uint32(buffer, ref pos_, ref stamp);
			}

			public int getLen()
			{
				int _len = 0;
				// phaseSycn
				_len += 4;
				// stamp
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  ***********************************************
	/// </summary>
	/// <summary>
	///  玩家支援记录
	/// </summary>
	[Protocol]
	public class StakeRecordReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 708310;
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

	public class StakeRecord : Protocol.IProtocolStream
	{
		/// <summary>
		///  场次id
		/// </summary>
		public UInt32 courtId;
		/// <summary>
		///  押注射手
		/// </summary>
		public UInt32 stakeShooter;
		/// <summary>
		///  赔率
		/// </summary>
		public UInt32 odds;
		/// <summary>
		///  支援数量
		/// </summary>
		public UInt32 stakeNum;
		/// <summary>
		///  盈利
		/// </summary>
		public Int32 profit;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, courtId);
				BaseDLL.encode_uint32(buffer, ref pos_, stakeShooter);
				BaseDLL.encode_uint32(buffer, ref pos_, odds);
				BaseDLL.encode_uint32(buffer, ref pos_, stakeNum);
				BaseDLL.encode_int32(buffer, ref pos_, profit);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref courtId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref stakeShooter);
				BaseDLL.decode_uint32(buffer, ref pos_, ref odds);
				BaseDLL.decode_uint32(buffer, ref pos_, ref stakeNum);
				BaseDLL.decode_int32(buffer, ref pos_, ref profit);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, courtId);
				BaseDLL.encode_uint32(buffer, ref pos_, stakeShooter);
				BaseDLL.encode_uint32(buffer, ref pos_, odds);
				BaseDLL.encode_uint32(buffer, ref pos_, stakeNum);
				BaseDLL.encode_int32(buffer, ref pos_, profit);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref courtId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref stakeShooter);
				BaseDLL.decode_uint32(buffer, ref pos_, ref odds);
				BaseDLL.decode_uint32(buffer, ref pos_, ref stakeNum);
				BaseDLL.decode_int32(buffer, ref pos_, ref profit);
			}

			public int getLen()
			{
				int _len = 0;
				// courtId
				_len += 4;
				// stakeShooter
				_len += 4;
				// odds
				_len += 4;
				// stakeNum
				_len += 4;
				// profit
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class StakeRecordRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 708311;
		public UInt32 Sequence;
		/// <summary>
		///  押注历史列表
		/// </summary>
		public StakeRecord[] StakeRecordList = new StakeRecord[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)StakeRecordList.Length);
				for(int i = 0; i < StakeRecordList.Length; i++)
				{
					StakeRecordList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 StakeRecordListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref StakeRecordListCnt);
				StakeRecordList = new StakeRecord[StakeRecordListCnt];
				for(int i = 0; i < StakeRecordList.Length; i++)
				{
					StakeRecordList[i] = new StakeRecord();
					StakeRecordList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)StakeRecordList.Length);
				for(int i = 0; i < StakeRecordList.Length; i++)
				{
					StakeRecordList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 StakeRecordListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref StakeRecordListCnt);
				StakeRecordList = new StakeRecord[StakeRecordListCnt];
				for(int i = 0; i < StakeRecordList.Length; i++)
				{
					StakeRecordList[i] = new StakeRecord();
					StakeRecordList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// StakeRecordList
				_len += 2;
				for(int j = 0; j < StakeRecordList.Length; j++)
				{
					_len += StakeRecordList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// ************************************************
	/// </summary>
	/// <summary>
	///  比赛历史
	/// </summary>
	[Protocol]
	public class BattleRecordReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 708312;
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

	public class BattleRecord : Protocol.IProtocolStream
	{
		/// <summary>
		///  场次id
		/// </summary>
		public UInt32 courtId;
		/// <summary>
		///  冠军射手
		/// </summary>
		public UInt32 champion;
		/// <summary>
		///  赔率
		/// </summary>
		public UInt32 odds;
		/// <summary>
		///  最大奖金
		/// </summary>
		public UInt32 maxProfit;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, courtId);
				BaseDLL.encode_uint32(buffer, ref pos_, champion);
				BaseDLL.encode_uint32(buffer, ref pos_, odds);
				BaseDLL.encode_uint32(buffer, ref pos_, maxProfit);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref courtId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref champion);
				BaseDLL.decode_uint32(buffer, ref pos_, ref odds);
				BaseDLL.decode_uint32(buffer, ref pos_, ref maxProfit);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, courtId);
				BaseDLL.encode_uint32(buffer, ref pos_, champion);
				BaseDLL.encode_uint32(buffer, ref pos_, odds);
				BaseDLL.encode_uint32(buffer, ref pos_, maxProfit);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref courtId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref champion);
				BaseDLL.decode_uint32(buffer, ref pos_, ref odds);
				BaseDLL.decode_uint32(buffer, ref pos_, ref maxProfit);
			}

			public int getLen()
			{
				int _len = 0;
				// courtId
				_len += 4;
				// champion
				_len += 4;
				// odds
				_len += 4;
				// maxProfit
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class BattleRecordRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 708313;
		public UInt32 Sequence;
		/// <summary>
		///  比赛历史记录列表
		/// </summary>
		public BattleRecord[] BattleRecordList = new BattleRecord[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)BattleRecordList.Length);
				for(int i = 0; i < BattleRecordList.Length; i++)
				{
					BattleRecordList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 BattleRecordListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref BattleRecordListCnt);
				BattleRecordList = new BattleRecord[BattleRecordListCnt];
				for(int i = 0; i < BattleRecordList.Length; i++)
				{
					BattleRecordList[i] = new BattleRecord();
					BattleRecordList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)BattleRecordList.Length);
				for(int i = 0; i < BattleRecordList.Length; i++)
				{
					BattleRecordList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 BattleRecordListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref BattleRecordListCnt);
				BattleRecordList = new BattleRecord[BattleRecordListCnt];
				for(int i = 0; i < BattleRecordList.Length; i++)
				{
					BattleRecordList[i] = new BattleRecord();
					BattleRecordList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// BattleRecordList
				_len += 2;
				for(int j = 0; j < BattleRecordList.Length; j++)
				{
					_len += BattleRecordList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  ********************************************
	/// </summary>
	/// <summary>
	///  选手排名
	/// </summary>
	[Protocol]
	public class shooterRankReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 708314;
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

	public class shooterRankInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  选手id
		/// </summary>
		public UInt32 shooterId;
		/// <summary>
		///  参赛次数
		/// </summary>
		public UInt32 battleNum;
		/// <summary>
		///  胜率
		/// </summary>
		public UInt32 winRate;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, shooterId);
				BaseDLL.encode_uint32(buffer, ref pos_, battleNum);
				BaseDLL.encode_uint32(buffer, ref pos_, winRate);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref shooterId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref winRate);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, shooterId);
				BaseDLL.encode_uint32(buffer, ref pos_, battleNum);
				BaseDLL.encode_uint32(buffer, ref pos_, winRate);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref shooterId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref winRate);
			}

			public int getLen()
			{
				int _len = 0;
				// shooterId
				_len += 4;
				// battleNum
				_len += 4;
				// winRate
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class shooterRankRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 708315;
		public UInt32 Sequence;
		/// <summary>
		///  排行列表
		/// </summary>
		public shooterRankInfo[] shooterRankList = new shooterRankInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)shooterRankList.Length);
				for(int i = 0; i < shooterRankList.Length; i++)
				{
					shooterRankList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 shooterRankListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref shooterRankListCnt);
				shooterRankList = new shooterRankInfo[shooterRankListCnt];
				for(int i = 0; i < shooterRankList.Length; i++)
				{
					shooterRankList[i] = new shooterRankInfo();
					shooterRankList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)shooterRankList.Length);
				for(int i = 0; i < shooterRankList.Length; i++)
				{
					shooterRankList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 shooterRankListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref shooterRankListCnt);
				shooterRankList = new shooterRankInfo[shooterRankListCnt];
				for(int i = 0; i < shooterRankList.Length; i++)
				{
					shooterRankList[i] = new shooterRankInfo();
					shooterRankList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// shooterRankList
				_len += 2;
				for(int j = 0; j < shooterRankList.Length; j++)
				{
					_len += shooterRankList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

}
