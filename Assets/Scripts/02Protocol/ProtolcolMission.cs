using System;
using System.Text;

namespace Protocol
{
	public enum TaskPublishType
	{
		TASK_PUBLISH_NPC = 0,
		TASK_PUBLISH_UI = 1,
		TASK_PUBLISH_CITY = 2,
	}

	public enum TaskSubmitType
	{
		TASK_SUBMIT_AUTO = 0,
		TASK_SUBMIT_NPC = 1,
		TASK_SUBMIT_UI = 2,
		TASK_SUBMIT_RIGHTNOW = 3,
	}

	public enum TaskStatus
	{
		TASK_INIT = 0,
		TASK_UNFINISH = 1,
		TASK_FINISHED = 2,
		TASK_FAILED = 3,
		TASK_SUBMITTING = 4,
		TASK_OVER = 5,
	}

	public enum DeleteTaskReason
	{
		DELETE_TASK_REASON_SUBMIT = 1,
		DELETE_TASK_REASON_ABANDON = 2,
		DELETE_TASK_REASON_SYSTEM = 3,
		DELETE_TASK_REASON_OTHER = 4,
	}

	public enum MasterSysReceiveDailyTaskRewardType
	{
		MSRDTR_MASTER = 1,
		/// <summary>
		/// 师傅领取
		/// </summary>
		MARDTR_DISCIPLE = 2,
	}

	/// <summary>
	/// 徒弟领取
	/// </summary>
	public enum MasterAcademicRewardRecvState
	{
		MARRS_NOTRECVED = 0,
		/// <summary>
		/// 没有领取
		/// </summary>
		MARRS_RECVED = 1,
	}

	/// <summary>
	/// 已经领取
	/// </summary>
	public enum MasterDailyTaskState
	{
		MDTST_UNPUBLISHED = 0,
		/// <summary>
		/// 未发布
		/// </summary>
		MDTST_UNREPORTED = 1,
		/// <summary>
		/// 已发布，未汇报
		/// </summary>
		MDTST_UNRECEIVED = 2,
		/// <summary>
		/// 已汇报，可领取
		/// </summary>
		MDTST_RECEIVEED = 3,
		/// <summary>
		/// 已汇报，已领取
		/// </summary>
		MDTST_UNPUB_UNRECE = 4,
	}

	/// <summary>
	/// 未发布，可领取 师傅才可能有这个状态
	/// </summary>
	public enum MasterReportStatus
	{
		/// <summary>
		///  无效
		/// </summary>
		MASTER_REPORT_STATUS_INVALID = 0,
		/// <summary>
		///  徒弟已经上报
		/// </summary>
		MASTER_REPORT_STATUS_REPORT = 1,
		/// <summary>
		///  师傅已经提交
		/// </summary>
		MASTER_REPORT_STATUS_SUBMIT = 2,
	}

	public enum AdventureTeamTaskDifficult
	{
		C = 0,
		B = 1,
		A = 2,
		S = 3,
		SS = 4,
		SSS = 5,
	}

	[Protocol]
	public class SceneAcceptTaskReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501103;
		public UInt32 Sequence;
		public byte acceptType;
		public UInt32 npcID;
		public UInt32 taskID;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, acceptType);
				BaseDLL.encode_uint32(buffer, ref pos_, npcID);
				BaseDLL.encode_uint32(buffer, ref pos_, taskID);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref acceptType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref npcID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskID);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, acceptType);
				BaseDLL.encode_uint32(buffer, ref pos_, npcID);
				BaseDLL.encode_uint32(buffer, ref pos_, taskID);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref acceptType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref npcID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskID);
			}

			public int getLen()
			{
				int _len = 0;
				// acceptType
				_len += 1;
				// npcID
				_len += 4;
				// taskID
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSubmitTaskReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501104;
		public UInt32 Sequence;
		public byte submitType;
		public UInt32 npcID;
		public UInt32 taskID;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, submitType);
				BaseDLL.encode_uint32(buffer, ref pos_, npcID);
				BaseDLL.encode_uint32(buffer, ref pos_, taskID);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref submitType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref npcID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskID);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, submitType);
				BaseDLL.encode_uint32(buffer, ref pos_, npcID);
				BaseDLL.encode_uint32(buffer, ref pos_, taskID);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref submitType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref npcID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskID);
			}

			public int getLen()
			{
				int _len = 0;
				// submitType
				_len += 1;
				// npcID
				_len += 4;
				// taskID
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneAbandonTaskReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501105;
		public UInt32 Sequence;
		public UInt32 taskID;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskID);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskID);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskID);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskID);
			}

			public int getLen()
			{
				int _len = 0;
				// taskID
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class MissionPair : Protocol.IProtocolStream
	{
		public string key;
		public string value;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] keyBytes = StringHelper.StringToUTF8Bytes(key);
				BaseDLL.encode_string(buffer, ref pos_, keyBytes, (UInt16)(buffer.Length - pos_));
				byte[] valueBytes = StringHelper.StringToUTF8Bytes(value);
				BaseDLL.encode_string(buffer, ref pos_, valueBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 keyLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref keyLen);
				byte[] keyBytes = new byte[keyLen];
				for(int i = 0; i < keyLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref keyBytes[i]);
				}
				key = StringHelper.BytesToString(keyBytes);
				UInt16 valueLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref valueLen);
				byte[] valueBytes = new byte[valueLen];
				for(int i = 0; i < valueLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref valueBytes[i]);
				}
				value = StringHelper.BytesToString(valueBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] keyBytes = StringHelper.StringToUTF8Bytes(key);
				BaseDLL.encode_string(buffer, ref pos_, keyBytes, (UInt16)(buffer.Length - pos_));
				byte[] valueBytes = StringHelper.StringToUTF8Bytes(value);
				BaseDLL.encode_string(buffer, ref pos_, valueBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 keyLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref keyLen);
				byte[] keyBytes = new byte[keyLen];
				for(int i = 0; i < keyLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref keyBytes[i]);
				}
				key = StringHelper.BytesToString(keyBytes);
				UInt16 valueLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref valueLen);
				byte[] valueBytes = new byte[valueLen];
				for(int i = 0; i < valueLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref valueBytes[i]);
				}
				value = StringHelper.BytesToString(valueBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// key
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(key);
					_len += 2 + _strBytes.Length;
				}
				// value
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(value);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	public class MissionInfo : Protocol.IProtocolStream
	{
		public UInt32 taskID;
		public byte status;
		public MissionPair[] akMissionPairs = new MissionPair[0];
		public UInt32 finTime;
		public byte submitCount;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskID);
				BaseDLL.encode_int8(buffer, ref pos_, status);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)akMissionPairs.Length);
				for(int i = 0; i < akMissionPairs.Length; i++)
				{
					akMissionPairs[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, finTime);
				BaseDLL.encode_int8(buffer, ref pos_, submitCount);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskID);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
				UInt16 akMissionPairsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref akMissionPairsCnt);
				akMissionPairs = new MissionPair[akMissionPairsCnt];
				for(int i = 0; i < akMissionPairs.Length; i++)
				{
					akMissionPairs[i] = new MissionPair();
					akMissionPairs[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref finTime);
				BaseDLL.decode_int8(buffer, ref pos_, ref submitCount);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskID);
				BaseDLL.encode_int8(buffer, ref pos_, status);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)akMissionPairs.Length);
				for(int i = 0; i < akMissionPairs.Length; i++)
				{
					akMissionPairs[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, finTime);
				BaseDLL.encode_int8(buffer, ref pos_, submitCount);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskID);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
				UInt16 akMissionPairsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref akMissionPairsCnt);
				akMissionPairs = new MissionPair[akMissionPairsCnt];
				for(int i = 0; i < akMissionPairs.Length; i++)
				{
					akMissionPairs[i] = new MissionPair();
					akMissionPairs[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref finTime);
				BaseDLL.decode_int8(buffer, ref pos_, ref submitCount);
			}

			public int getLen()
			{
				int _len = 0;
				// taskID
				_len += 4;
				// status
				_len += 1;
				// akMissionPairs
				_len += 2;
				for(int j = 0; j < akMissionPairs.Length; j++)
				{
					_len += akMissionPairs[j].getLen();
				}
				// finTime
				_len += 4;
				// submitCount
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneTaskListRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501106;
		public UInt32 Sequence;
		public MissionInfo[] tasks = new MissionInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)tasks.Length);
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 tasksCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref tasksCnt);
				tasks = new MissionInfo[tasksCnt];
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i] = new MissionInfo();
					tasks[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)tasks.Length);
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 tasksCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref tasksCnt);
				tasks = new MissionInfo[tasksCnt];
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i] = new MissionInfo();
					tasks[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// tasks
				_len += 2;
				for(int j = 0; j < tasks.Length; j++)
				{
					_len += tasks[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneNotifyNewTaskRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501107;
		public UInt32 Sequence;
		public MissionInfo taskInfo = new MissionInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				taskInfo.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				taskInfo.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				taskInfo.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				taskInfo.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// taskInfo
				_len += taskInfo.getLen();
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneNotifyDeleteTaskRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501108;
		public UInt32 Sequence;
		public UInt32 taskID;
		public byte reasion;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskID);
				BaseDLL.encode_int8(buffer, ref pos_, reasion);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskID);
				BaseDLL.decode_int8(buffer, ref pos_, ref reasion);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskID);
				BaseDLL.encode_int8(buffer, ref pos_, reasion);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskID);
				BaseDLL.decode_int8(buffer, ref pos_, ref reasion);
			}

			public int getLen()
			{
				int _len = 0;
				// taskID
				_len += 4;
				// reasion
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneNotifyTaskStatusRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501109;
		public UInt32 Sequence;
		public UInt32 taskID;
		public byte status;
		public UInt32 finTime;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskID);
				BaseDLL.encode_int8(buffer, ref pos_, status);
				BaseDLL.encode_uint32(buffer, ref pos_, finTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskID);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
				BaseDLL.decode_uint32(buffer, ref pos_, ref finTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskID);
				BaseDLL.encode_int8(buffer, ref pos_, status);
				BaseDLL.encode_uint32(buffer, ref pos_, finTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskID);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
				BaseDLL.decode_uint32(buffer, ref pos_, ref finTime);
			}

			public int getLen()
			{
				int _len = 0;
				// taskID
				_len += 4;
				// status
				_len += 1;
				// finTime
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneNotifyTaskVarRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501110;
		public UInt32 Sequence;
		public UInt32 taskID;
		public string key;
		public string value;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskID);
				byte[] keyBytes = StringHelper.StringToUTF8Bytes(key);
				BaseDLL.encode_string(buffer, ref pos_, keyBytes, (UInt16)(buffer.Length - pos_));
				byte[] valueBytes = StringHelper.StringToUTF8Bytes(value);
				BaseDLL.encode_string(buffer, ref pos_, valueBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskID);
				UInt16 keyLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref keyLen);
				byte[] keyBytes = new byte[keyLen];
				for(int i = 0; i < keyLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref keyBytes[i]);
				}
				key = StringHelper.BytesToString(keyBytes);
				UInt16 valueLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref valueLen);
				byte[] valueBytes = new byte[valueLen];
				for(int i = 0; i < valueLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref valueBytes[i]);
				}
				value = StringHelper.BytesToString(valueBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskID);
				byte[] keyBytes = StringHelper.StringToUTF8Bytes(key);
				BaseDLL.encode_string(buffer, ref pos_, keyBytes, (UInt16)(buffer.Length - pos_));
				byte[] valueBytes = StringHelper.StringToUTF8Bytes(value);
				BaseDLL.encode_string(buffer, ref pos_, valueBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskID);
				UInt16 keyLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref keyLen);
				byte[] keyBytes = new byte[keyLen];
				for(int i = 0; i < keyLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref keyBytes[i]);
				}
				key = StringHelper.BytesToString(keyBytes);
				UInt16 valueLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref valueLen);
				byte[] valueBytes = new byte[valueLen];
				for(int i = 0; i < valueLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref valueBytes[i]);
				}
				value = StringHelper.BytesToString(valueBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// taskID
				_len += 4;
				// key
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(key);
					_len += 2 + _strBytes.Length;
				}
				// value
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(value);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSubmitDailyTask : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501124;
		public UInt32 Sequence;
		public UInt32 taskId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
			}

			public int getLen()
			{
				int _len = 0;
				// taskId
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneDailyTaskList : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501123;
		public UInt32 Sequence;
		public MissionInfo[] tasks = new MissionInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)tasks.Length);
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 tasksCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref tasksCnt);
				tasks = new MissionInfo[tasksCnt];
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i] = new MissionInfo();
					tasks[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)tasks.Length);
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 tasksCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref tasksCnt);
				tasks = new MissionInfo[tasksCnt];
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i] = new MissionInfo();
					tasks[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// tasks
				_len += 2;
				for(int j = 0; j < tasks.Length; j++)
				{
					_len += tasks[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSubmitAchievementTask : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501126;
		public UInt32 Sequence;
		public UInt32 taskId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
			}

			public int getLen()
			{
				int _len = 0;
				// taskId
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneAchievementTaskList : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501125;
		public UInt32 Sequence;
		public MissionInfo[] tasks = new MissionInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)tasks.Length);
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 tasksCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref tasksCnt);
				tasks = new MissionInfo[tasksCnt];
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i] = new MissionInfo();
					tasks[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)tasks.Length);
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 tasksCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref tasksCnt);
				tasks = new MissionInfo[tasksCnt];
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i] = new MissionInfo();
					tasks[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// tasks
				_len += 2;
				for(int j = 0; j < tasks.Length; j++)
				{
					_len += tasks[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSubmitAllDailyTask : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501132;
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
	public class SceneSetTaskItemReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501133;
		public UInt32 Sequence;
		public UInt32 taskId;
		public UInt64[] itemIds = new UInt64[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemIds.Length);
				for(int i = 0; i < itemIds.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, itemIds[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
				UInt16 itemIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemIdsCnt);
				itemIds = new UInt64[itemIdsCnt];
				for(int i = 0; i < itemIds.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref itemIds[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemIds.Length);
				for(int i = 0; i < itemIds.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, itemIds[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
				UInt16 itemIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemIdsCnt);
				itemIds = new UInt64[itemIdsCnt];
				for(int i = 0; i < itemIds.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref itemIds[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// taskId
				_len += 4;
				// itemIds
				_len += 2 + 8 * itemIds.Length;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSetTaskItemRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501134;
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

	[Protocol]
	public class SceneRefreshCycleTask : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501135;
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
	public class SceneDailyScoreRewardReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501139;
		public UInt32 Sequence;
		public byte boxId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, boxId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref boxId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, boxId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref boxId);
			}

			public int getLen()
			{
				int _len = 0;
				// boxId
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneDailyScoreRewardRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501140;
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

	[Protocol]
	public class SceneLegendTaskListRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501114;
		public UInt32 Sequence;
		public MissionInfo[] tasks = new MissionInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)tasks.Length);
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 tasksCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref tasksCnt);
				tasks = new MissionInfo[tasksCnt];
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i] = new MissionInfo();
					tasks[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)tasks.Length);
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 tasksCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref tasksCnt);
				tasks = new MissionInfo[tasksCnt];
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i] = new MissionInfo();
					tasks[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// tasks
				_len += 2;
				for(int j = 0; j < tasks.Length; j++)
				{
					_len += tasks[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSubmitLegendTask : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501115;
		public UInt32 Sequence;
		public UInt32 taskId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
			}

			public int getLen()
			{
				int _len = 0;
				// taskId
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneResetTaskSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501116;
		public UInt32 Sequence;
		public MissionInfo taskInfo = new MissionInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				taskInfo.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				taskInfo.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				taskInfo.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				taskInfo.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// taskInfo
				_len += taskInfo.getLen();
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneFinishedTaskList : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501117;
		public UInt32 Sequence;
		public UInt32[] taskIds = new UInt32[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)taskIds.Length);
				for(int i = 0; i < taskIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, taskIds[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 taskIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref taskIdsCnt);
				taskIds = new UInt32[taskIdsCnt];
				for(int i = 0; i < taskIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref taskIds[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)taskIds.Length);
				for(int i = 0; i < taskIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, taskIds[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 taskIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref taskIdsCnt);
				taskIds = new UInt32[taskIdsCnt];
				for(int i = 0; i < taskIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref taskIds[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// taskIds
				_len += 2 + 4 * taskIds.Length;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneAchievementScoreRewardReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501156;
		public UInt32 Sequence;
		public UInt32 rewardId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, rewardId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref rewardId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, rewardId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref rewardId);
			}

			public int getLen()
			{
				int _len = 0;
				// rewardId
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneAchievementScoreRewardRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501157;
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

	[Protocol]
	public class SceneSyncMasterDailyTaskList : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501159;
		public UInt32 Sequence;
		public MissionInfo[] tasks = new MissionInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)tasks.Length);
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 tasksCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref tasksCnt);
				tasks = new MissionInfo[tasksCnt];
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i] = new MissionInfo();
					tasks[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)tasks.Length);
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 tasksCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref tasksCnt);
				tasks = new MissionInfo[tasksCnt];
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i] = new MissionInfo();
					tasks[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// tasks
				_len += 2;
				for(int j = 0; j < tasks.Length; j++)
				{
					_len += tasks[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSyncMasterAcademicTaskList : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501160;
		public UInt32 Sequence;
		public MissionInfo[] tasks = new MissionInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)tasks.Length);
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 tasksCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref tasksCnt);
				tasks = new MissionInfo[tasksCnt];
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i] = new MissionInfo();
					tasks[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)tasks.Length);
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 tasksCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref tasksCnt);
				tasks = new MissionInfo[tasksCnt];
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i] = new MissionInfo();
					tasks[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// tasks
				_len += 2;
				for(int j = 0; j < tasks.Length; j++)
				{
					_len += tasks[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSyncMasterTaskList : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501161;
		public UInt32 Sequence;
		public MissionInfo[] tasks = new MissionInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)tasks.Length);
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 tasksCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref tasksCnt);
				tasks = new MissionInfo[tasksCnt];
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i] = new MissionInfo();
					tasks[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)tasks.Length);
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 tasksCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref tasksCnt);
				tasks = new MissionInfo[tasksCnt];
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i] = new MissionInfo();
					tasks[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// tasks
				_len += 2;
				for(int j = 0; j < tasks.Length; j++)
				{
					_len += tasks[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world 发布师门日常任务请求
	/// </summary>
	[Protocol]
	public class WorldPublishMasterDialyTaskReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601752;
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
	/// world->client 发布师门日常任务返回
	/// </summary>
	[Protocol]
	public class WorldPublishMasterDialyTaskRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601753;
		public UInt32 Sequence;
		public UInt32 code;
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
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint64(buffer, ref pos_, discipleId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint64(buffer, ref pos_, ref discipleId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint64(buffer, ref pos_, discipleId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint64(buffer, ref pos_, ref discipleId);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// discipleId
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world  获取徒弟师门任务数据请求
	/// </summary>
	[Protocol]
	public class WorldGetDiscipleMasterTasksReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601756;
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

	public class MasterTaskShareData : Protocol.IProtocolStream
	{
		public UInt32 academicTotalGrowth;
		public UInt32 masterDailyTaskGrowth;
		public UInt32 masterAcademicTaskGrowth;
		public UInt32 masterUplevelGrowth;
		public UInt32 masterGiveEquipGrowth;
		public UInt32 masterGiveGiftGrowth;
		public UInt32 masterTeamClearDungeonGrowth;
		public UInt32 goodTeachValue;
		public MissionInfo[] dailyTasks = new MissionInfo[0];
		public MissionInfo[] academicTasks = new MissionInfo[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, academicTotalGrowth);
				BaseDLL.encode_uint32(buffer, ref pos_, masterDailyTaskGrowth);
				BaseDLL.encode_uint32(buffer, ref pos_, masterAcademicTaskGrowth);
				BaseDLL.encode_uint32(buffer, ref pos_, masterUplevelGrowth);
				BaseDLL.encode_uint32(buffer, ref pos_, masterGiveEquipGrowth);
				BaseDLL.encode_uint32(buffer, ref pos_, masterGiveGiftGrowth);
				BaseDLL.encode_uint32(buffer, ref pos_, masterTeamClearDungeonGrowth);
				BaseDLL.encode_uint32(buffer, ref pos_, goodTeachValue);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)dailyTasks.Length);
				for(int i = 0; i < dailyTasks.Length; i++)
				{
					dailyTasks[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)academicTasks.Length);
				for(int i = 0; i < academicTasks.Length; i++)
				{
					academicTasks[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref academicTotalGrowth);
				BaseDLL.decode_uint32(buffer, ref pos_, ref masterDailyTaskGrowth);
				BaseDLL.decode_uint32(buffer, ref pos_, ref masterAcademicTaskGrowth);
				BaseDLL.decode_uint32(buffer, ref pos_, ref masterUplevelGrowth);
				BaseDLL.decode_uint32(buffer, ref pos_, ref masterGiveEquipGrowth);
				BaseDLL.decode_uint32(buffer, ref pos_, ref masterGiveGiftGrowth);
				BaseDLL.decode_uint32(buffer, ref pos_, ref masterTeamClearDungeonGrowth);
				BaseDLL.decode_uint32(buffer, ref pos_, ref goodTeachValue);
				UInt16 dailyTasksCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref dailyTasksCnt);
				dailyTasks = new MissionInfo[dailyTasksCnt];
				for(int i = 0; i < dailyTasks.Length; i++)
				{
					dailyTasks[i] = new MissionInfo();
					dailyTasks[i].decode(buffer, ref pos_);
				}
				UInt16 academicTasksCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref academicTasksCnt);
				academicTasks = new MissionInfo[academicTasksCnt];
				for(int i = 0; i < academicTasks.Length; i++)
				{
					academicTasks[i] = new MissionInfo();
					academicTasks[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, academicTotalGrowth);
				BaseDLL.encode_uint32(buffer, ref pos_, masterDailyTaskGrowth);
				BaseDLL.encode_uint32(buffer, ref pos_, masterAcademicTaskGrowth);
				BaseDLL.encode_uint32(buffer, ref pos_, masterUplevelGrowth);
				BaseDLL.encode_uint32(buffer, ref pos_, masterGiveEquipGrowth);
				BaseDLL.encode_uint32(buffer, ref pos_, masterGiveGiftGrowth);
				BaseDLL.encode_uint32(buffer, ref pos_, masterTeamClearDungeonGrowth);
				BaseDLL.encode_uint32(buffer, ref pos_, goodTeachValue);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)dailyTasks.Length);
				for(int i = 0; i < dailyTasks.Length; i++)
				{
					dailyTasks[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)academicTasks.Length);
				for(int i = 0; i < academicTasks.Length; i++)
				{
					academicTasks[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref academicTotalGrowth);
				BaseDLL.decode_uint32(buffer, ref pos_, ref masterDailyTaskGrowth);
				BaseDLL.decode_uint32(buffer, ref pos_, ref masterAcademicTaskGrowth);
				BaseDLL.decode_uint32(buffer, ref pos_, ref masterUplevelGrowth);
				BaseDLL.decode_uint32(buffer, ref pos_, ref masterGiveEquipGrowth);
				BaseDLL.decode_uint32(buffer, ref pos_, ref masterGiveGiftGrowth);
				BaseDLL.decode_uint32(buffer, ref pos_, ref masterTeamClearDungeonGrowth);
				BaseDLL.decode_uint32(buffer, ref pos_, ref goodTeachValue);
				UInt16 dailyTasksCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref dailyTasksCnt);
				dailyTasks = new MissionInfo[dailyTasksCnt];
				for(int i = 0; i < dailyTasks.Length; i++)
				{
					dailyTasks[i] = new MissionInfo();
					dailyTasks[i].decode(buffer, ref pos_);
				}
				UInt16 academicTasksCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref academicTasksCnt);
				academicTasks = new MissionInfo[academicTasksCnt];
				for(int i = 0; i < academicTasks.Length; i++)
				{
					academicTasks[i] = new MissionInfo();
					academicTasks[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// academicTotalGrowth
				_len += 4;
				// masterDailyTaskGrowth
				_len += 4;
				// masterAcademicTaskGrowth
				_len += 4;
				// masterUplevelGrowth
				_len += 4;
				// masterGiveEquipGrowth
				_len += 4;
				// masterGiveGiftGrowth
				_len += 4;
				// masterTeamClearDungeonGrowth
				_len += 4;
				// goodTeachValue
				_len += 4;
				// dailyTasks
				_len += 2;
				for(int j = 0; j < dailyTasks.Length; j++)
				{
					_len += dailyTasks[j].getLen();
				}
				// academicTasks
				_len += 2;
				for(int j = 0; j < academicTasks.Length; j++)
				{
					_len += academicTasks[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client  获取徒弟师门任务数据返回	
	/// </summary>
	[Protocol]
	public class WorldGetDiscipleMasterTasksRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601757;
		public UInt32 Sequence;
		public UInt32 code;
		public UInt64 discipleId;
		public MasterTaskShareData masterTaskShareData = new MasterTaskShareData();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint64(buffer, ref pos_, discipleId);
				masterTaskShareData.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint64(buffer, ref pos_, ref discipleId);
				masterTaskShareData.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint64(buffer, ref pos_, discipleId);
				masterTaskShareData.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint64(buffer, ref pos_, ref discipleId);
				masterTaskShareData.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// discipleId
				_len += 8;
				// masterTaskShareData
				_len += masterTaskShareData.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world 徒弟汇报师门任务完成请求
	/// </summary>
	[Protocol]
	public class WorldReportMasterDailyTaskReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601761;
		public UInt32 Sequence;
		public UInt64 masterId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, masterId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref masterId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, masterId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref masterId);
			}

			public int getLen()
			{
				int _len = 0;
				// masterId
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client 徒弟汇报师门任务完成返回
	/// </summary>
	[Protocol]
	public class WorldReportMasterDailyTaskRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601762;
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
	/// client->world  领取师门日常任务完成奖励请求
	/// </summary>
	[Protocol]
	public class WorldReceiveMasterDailyTaskRewardReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601765;
		public UInt32 Sequence;
		public byte type;
		public UInt64 discipeleId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint64(buffer, ref pos_, discipeleId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref discipeleId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, discipeleId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref discipeleId);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// discipeleId
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 师傅领取的时候 要填徒弟id
	/// </summary>
	/// <summary>
	/// world->client  领取师门日常任务完成奖励返回
	/// </summary>
	[Protocol]
	public class WorldReceiveMasterDailyTaskRewardRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601766;
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
	/// client->world 徒弟领取师门成长奖励请求
	/// </summary>
	[Protocol]
	public class WorldReceiveMasterAcademicTaskRewardReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601767;
		public UInt32 Sequence;
		public UInt32 giftDataId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, giftDataId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref giftDataId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, giftDataId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref giftDataId);
			}

			public int getLen()
			{
				int _len = 0;
				// giftDataId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client 徒弟领取师门成长奖励返回
	/// </summary>
	[Protocol]
	public class WorldReceiveMasterAcademicTaskRewardRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601768;
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
	/// world->client 出师成功奖励展示
	/// </summary>
	[Protocol]
	public class WorldNotifyFinshSchoolReward : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601771;
		public UInt32 Sequence;
		public UInt32 giftId;
		public UInt64 masterId;
		public UInt64 discipleId;
		public string masterName;
		public string discipleName;
		public UInt32 discipleGrowth;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, giftId);
				BaseDLL.encode_uint64(buffer, ref pos_, masterId);
				BaseDLL.encode_uint64(buffer, ref pos_, discipleId);
				byte[] masterNameBytes = StringHelper.StringToUTF8Bytes(masterName);
				BaseDLL.encode_string(buffer, ref pos_, masterNameBytes, (UInt16)(buffer.Length - pos_));
				byte[] discipleNameBytes = StringHelper.StringToUTF8Bytes(discipleName);
				BaseDLL.encode_string(buffer, ref pos_, discipleNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, discipleGrowth);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref giftId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref masterId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref discipleId);
				UInt16 masterNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref masterNameLen);
				byte[] masterNameBytes = new byte[masterNameLen];
				for(int i = 0; i < masterNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref masterNameBytes[i]);
				}
				masterName = StringHelper.BytesToString(masterNameBytes);
				UInt16 discipleNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref discipleNameLen);
				byte[] discipleNameBytes = new byte[discipleNameLen];
				for(int i = 0; i < discipleNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref discipleNameBytes[i]);
				}
				discipleName = StringHelper.BytesToString(discipleNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref discipleGrowth);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, giftId);
				BaseDLL.encode_uint64(buffer, ref pos_, masterId);
				BaseDLL.encode_uint64(buffer, ref pos_, discipleId);
				byte[] masterNameBytes = StringHelper.StringToUTF8Bytes(masterName);
				BaseDLL.encode_string(buffer, ref pos_, masterNameBytes, (UInt16)(buffer.Length - pos_));
				byte[] discipleNameBytes = StringHelper.StringToUTF8Bytes(discipleName);
				BaseDLL.encode_string(buffer, ref pos_, discipleNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, discipleGrowth);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref giftId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref masterId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref discipleId);
				UInt16 masterNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref masterNameLen);
				byte[] masterNameBytes = new byte[masterNameLen];
				for(int i = 0; i < masterNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref masterNameBytes[i]);
				}
				masterName = StringHelper.BytesToString(masterNameBytes);
				UInt16 discipleNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref discipleNameLen);
				byte[] discipleNameBytes = new byte[discipleNameLen];
				for(int i = 0; i < discipleNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref discipleNameBytes[i]);
				}
				discipleName = StringHelper.BytesToString(discipleNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref discipleGrowth);
			}

			public int getLen()
			{
				int _len = 0;
				// giftId
				_len += 4;
				// masterId
				_len += 8;
				// discipleId
				_len += 8;
				// masterName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(masterName);
					_len += 2 + _strBytes.Length;
				}
				// discipleName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(discipleName);
					_len += 2 + _strBytes.Length;
				}
				// discipleGrowth
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world 徒弟汇报师门任务完成请求
	/// </summary>
	[Protocol]
	public class WorldReportMasterTaskReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601799;
		public UInt32 Sequence;
		public UInt64 masterId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, masterId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref masterId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, masterId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref masterId);
			}

			public int getLen()
			{
				int _len = 0;
				// masterId
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client 徒弟汇报师门任务完成返回
	/// </summary>
	[Protocol]
	public class WorldReportMasterTaskRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601800;
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
	/// client->world 师傅领取师门任务请求
	/// </summary>
	[Protocol]
	public class WorldSubmitMasterTaskReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601801;
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
	///  world->client 师傅领取师门任务返回
	/// </summary>
	[Protocol]
	public class WorldSubmitMasterTaskRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601802;
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
	///  world->client 通知师傅有徒弟的任务刷新
	/// </summary>
	[Protocol]
	public class WorldUpdateMasterTaskSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601803;
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
	/// client->world 提交一个账号任务
	/// </summary>
	[Protocol]
	public class WorldSubmitAccountTask : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601103;
		public UInt32 Sequence;
		/// <summary>
		///  任务id
		/// </summary>
		public UInt32 taskId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
			}

			public int getLen()
			{
				int _len = 0;
				// taskId
				_len += 4;
				return _len;
			}
		#endregion

	}

}
