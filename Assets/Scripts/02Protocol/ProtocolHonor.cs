using System;
using System.Text;

namespace Protocol
{
	/// <summary>
	///  
	/// </summary>
	public enum HONOR_DATE_TYPE
	{
		/// <summary>
		///  总计
		/// </summary>
		HONOR_DATE_TYPE_TOTAL = 1,
		/// <summary>
		///  今天
		/// </summary>
		HONOR_DATE_TYPE_TODAY = 2,
		/// <summary>
		///  昨天
		/// </summary>
		HONOR_DATE_TYPE_YESTERDAY = 3,
		/// <summary>
		///  本周
		/// </summary>
		HONOR_DATE_TYPE_THIS_WEEK = 4,
		/// <summary>
		///  上周
		/// </summary>
		HONOR_DATE_TYPE_LAST_WEEK = 5,
		HONOR_DATE_TYPE_MAX = 6,
	}

	/// <summary>
	///  荣誉请求
	/// </summary>
	[Protocol]
	public class SceneHonorReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 509901;
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
	///  pvp次数统计
	/// </summary>
	public class PvpStatistics : Protocol.IProtocolStream
	{
		/// <summary>
		///  pvp类型
		/// </summary>
		public UInt32 pvpType;
		/// <summary>
		///  次数
		/// </summary>
		public UInt32 pvpCnt;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, pvpType);
				BaseDLL.encode_uint32(buffer, ref pos_, pvpCnt);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref pvpType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref pvpCnt);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, pvpType);
				BaseDLL.encode_uint32(buffer, ref pos_, pvpCnt);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref pvpType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref pvpCnt);
			}

			public int getLen()
			{
				int _len = 0;
				// pvpType
				_len += 4;
				// pvpCnt
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  历史荣誉
	/// </summary>
	public class HistoryHonorInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  日期类型
		/// </summary>
		public UInt32 dateType;
		/// <summary>
		///  荣誉总计
		/// </summary>
		public UInt32 totalHonor;
		/// <summary>
		///  pvp计数列表
		/// </summary>
		public PvpStatistics[] pvpStatisticsList = new PvpStatistics[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dateType);
				BaseDLL.encode_uint32(buffer, ref pos_, totalHonor);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)pvpStatisticsList.Length);
				for(int i = 0; i < pvpStatisticsList.Length; i++)
				{
					pvpStatisticsList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dateType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalHonor);
				UInt16 pvpStatisticsListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref pvpStatisticsListCnt);
				pvpStatisticsList = new PvpStatistics[pvpStatisticsListCnt];
				for(int i = 0; i < pvpStatisticsList.Length; i++)
				{
					pvpStatisticsList[i] = new PvpStatistics();
					pvpStatisticsList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dateType);
				BaseDLL.encode_uint32(buffer, ref pos_, totalHonor);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)pvpStatisticsList.Length);
				for(int i = 0; i < pvpStatisticsList.Length; i++)
				{
					pvpStatisticsList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dateType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalHonor);
				UInt16 pvpStatisticsListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref pvpStatisticsListCnt);
				pvpStatisticsList = new PvpStatistics[pvpStatisticsListCnt];
				for(int i = 0; i < pvpStatisticsList.Length; i++)
				{
					pvpStatisticsList[i] = new PvpStatistics();
					pvpStatisticsList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// dateType
				_len += 4;
				// totalHonor
				_len += 4;
				// pvpStatisticsList
				_len += 2;
				for(int j = 0; j < pvpStatisticsList.Length; j++)
				{
					_len += pvpStatisticsList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  荣誉返回
	/// </summary>
	[Protocol]
	public class SceneHonorRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 509902;
		public UInt32 Sequence;
		/// <summary>
		///  荣誉等级
		/// </summary>
		public UInt32 honorLvl;
		/// <summary>
		///  荣誉经验
		/// </summary>
		public UInt32 honorExp;
		/// <summary>
		///  上周排名
		/// </summary>
		public UInt32 lastWeekRank;
		/// <summary>
		///  历史排名
		/// </summary>
		public UInt32 historyRank;
		/// <summary>
		///  最高荣誉等级
		/// </summary>
		public UInt32 highestHonorLvl;
		/// <summary>
		///  是否使用保障卡
		/// </summary>
		public UInt32 isUseCard;
		/// <summary>
		///  排名结算时间
		/// </summary>
		public UInt32 rankTime;
		/// <summary>
		///  历史荣誉信息
		/// </summary>
		public HistoryHonorInfo[] historyHonorInfoList = new HistoryHonorInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, honorLvl);
				BaseDLL.encode_uint32(buffer, ref pos_, honorExp);
				BaseDLL.encode_uint32(buffer, ref pos_, lastWeekRank);
				BaseDLL.encode_uint32(buffer, ref pos_, historyRank);
				BaseDLL.encode_uint32(buffer, ref pos_, highestHonorLvl);
				BaseDLL.encode_uint32(buffer, ref pos_, isUseCard);
				BaseDLL.encode_uint32(buffer, ref pos_, rankTime);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)historyHonorInfoList.Length);
				for(int i = 0; i < historyHonorInfoList.Length; i++)
				{
					historyHonorInfoList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref honorLvl);
				BaseDLL.decode_uint32(buffer, ref pos_, ref honorExp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref lastWeekRank);
				BaseDLL.decode_uint32(buffer, ref pos_, ref historyRank);
				BaseDLL.decode_uint32(buffer, ref pos_, ref highestHonorLvl);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isUseCard);
				BaseDLL.decode_uint32(buffer, ref pos_, ref rankTime);
				UInt16 historyHonorInfoListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref historyHonorInfoListCnt);
				historyHonorInfoList = new HistoryHonorInfo[historyHonorInfoListCnt];
				for(int i = 0; i < historyHonorInfoList.Length; i++)
				{
					historyHonorInfoList[i] = new HistoryHonorInfo();
					historyHonorInfoList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, honorLvl);
				BaseDLL.encode_uint32(buffer, ref pos_, honorExp);
				BaseDLL.encode_uint32(buffer, ref pos_, lastWeekRank);
				BaseDLL.encode_uint32(buffer, ref pos_, historyRank);
				BaseDLL.encode_uint32(buffer, ref pos_, highestHonorLvl);
				BaseDLL.encode_uint32(buffer, ref pos_, isUseCard);
				BaseDLL.encode_uint32(buffer, ref pos_, rankTime);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)historyHonorInfoList.Length);
				for(int i = 0; i < historyHonorInfoList.Length; i++)
				{
					historyHonorInfoList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref honorLvl);
				BaseDLL.decode_uint32(buffer, ref pos_, ref honorExp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref lastWeekRank);
				BaseDLL.decode_uint32(buffer, ref pos_, ref historyRank);
				BaseDLL.decode_uint32(buffer, ref pos_, ref highestHonorLvl);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isUseCard);
				BaseDLL.decode_uint32(buffer, ref pos_, ref rankTime);
				UInt16 historyHonorInfoListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref historyHonorInfoListCnt);
				historyHonorInfoList = new HistoryHonorInfo[historyHonorInfoListCnt];
				for(int i = 0; i < historyHonorInfoList.Length; i++)
				{
					historyHonorInfoList[i] = new HistoryHonorInfo();
					historyHonorInfoList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// honorLvl
				_len += 4;
				// honorExp
				_len += 4;
				// lastWeekRank
				_len += 4;
				// historyRank
				_len += 4;
				// highestHonorLvl
				_len += 4;
				// isUseCard
				_len += 4;
				// rankTime
				_len += 4;
				// historyHonorInfoList
				_len += 2;
				for(int j = 0; j < historyHonorInfoList.Length; j++)
				{
					_len += historyHonorInfoList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  荣誉小红点
	/// </summary>
	[Protocol]
	public class SceneHonorRedPoint : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 509903;
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
