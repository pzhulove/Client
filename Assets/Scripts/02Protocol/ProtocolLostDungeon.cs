using System;
using System.Text;

namespace Protocol
{
	/// <summary>
	/// 迷失地牢状态
	/// </summary>
	public enum LostDungeonState
	{
		LDS_CLOSE = 0,
		/// <summary>
		/// 关闭中
		/// </summary>
		LDS_PROGRESSING = 1,
		/// <summary>
		/// 进行中
		/// </summary>
		LDS_END_CAN_REWARD = 2,
	}

	/// <summary>
	/// 结束可领取奖励
	/// </summary>
	public enum LostDungeonPlayerBattleSt
	{
		LDPBT_NORMAL = 0,
		/// <summary>
		/// 非战斗
		/// </summary>
		LDPBT_BATTLE_PVE = 1,
		/// <summary>
		/// PVE战斗中
		/// </summary>
		LDPBT_BATTLE_PVP = 2,
	}

	public enum LostDungeonNodeState
	{
		LDNS_NONE = 0,
		LDNS_LOCK = 1,
		/// <summary>
		/// 封锁
		/// </summary>
		LDNS_CLOSE = 2,
		/// <summary>
		/// 关闭
		/// </summary>
		LDNS_OPEN = 3,
		/// <summary>
		/// 打开
		/// </summary>
		LDNS_HALF_OPEN = 4,
	}

	/// <summary>
	/// 半开
	/// </summary>
	public enum LostDungeonFloorState
	{
		LDFS_NONE = 0,
		/// <summary>
		/// 
		/// </summary>
		LDFS_LOCK = 1,
		/// <summary>
		/// 封锁
		/// </summary>
		LDFS_UNLOCK_UNPASS = 2,
		/// <summary>
		/// 解锁未通关
		/// </summary>
		LDFS_UNLOCK_PASS = 3,
	}

	/// <summary>
	/// 已通关
	/// </summary>
	public enum LostDungeonBoxState
	{
		LDBXS_NONE = 0,
		LDBXS_UNOPENED = 1,
		/// <summary>
		/// 未开过
		/// </summary>
		LDBXS_OPENED = 2,
	}

	/// <summary>
	/// 打开过
	/// </summary>
	/// <summary>
	/// 地下城队伍战斗状态
	/// </summary>
	public enum LostTeamBattleSt
	{
		LDTBS_NORMAL = 0,
		/// <summary>
		/// 非战斗状态
		/// </summary>
		LDTBS_MATCH = 1,
		/// <summary>
		/// 匹配中,（不一定成功）
		/// </summary>
		LDTBS_BATTLE = 2,
		/// <summary>
		/// 战斗状态
		/// </summary>
		LDTBS_MAX = 3,
	}

	/// <summary>
	/// 地下城调整模式
	/// </summary>
	public enum LostDungChangeMode
	{
		LDCM_SINGLE = 1,
		/// <summary>
		/// 单人
		/// </summary>
		LDCM_TEAM = 2,
	}

	/// <summary>
	/// 组队
	/// </summary>
	/// <summary>
	///  同步队伍信息类型
	/// </summary>
	public enum SyncDungeonEnterTeamInfoType
	{
		SDETIT_NONE = 0,
		SDETIT_ENTER_SYNC = 1,
		/// <summary>
		/// 进入时候同步
		/// </summary>
		SDETIT_ADD = 2,
		/// <summary>
		/// 添加队员
		/// </summary>
		SDETIT_LEAVE = 3,
		/// <summary>
		/// 队员离开
		/// </summary>
		SDETIT_UPDATE = 4,
		/// <summary>
		/// 刷新队员信息
		/// </summary>
		SDETIT_CHG_MODE = 5,
		/// <summary>
		/// 刷新挑战模式
		/// </summary>
		SDETIT_TEAM_STATE = 6,
		/// <summary>
		/// 刷新队伍状态
		/// </summary>
		SDETIT_MAX = 7,
	}

	public enum LostDungeonBattleReasult
	{
		LDBR_NONE = 0,
		LDBR_PASS = 1,
		/// <summary>
		/// 通关
		/// </summary>
		LDBR_FAIL = 2,
		/// <summary>
		/// 失败
		/// </summary>
		LDBR_OVER = 3,
	}

	/// <summary>
	/// PVP战斗中
	/// </summary>
	/// <summary>
	///  挑战迷失地牢请求
	/// </summary>
	[Protocol]
	public class LostDungeonChallengeReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510001;
		public UInt32 Sequence;
		/// <summary>
		///  层数
		/// </summary>
		public UInt32 floor;
		/// <summary>
		///  难度
		/// </summary>
		public byte hardType;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, floor);
				BaseDLL.encode_int8(buffer, ref pos_, hardType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref floor);
				BaseDLL.decode_int8(buffer, ref pos_, ref hardType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, floor);
				BaseDLL.encode_int8(buffer, ref pos_, hardType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref floor);
				BaseDLL.decode_int8(buffer, ref pos_, ref hardType);
			}

			public int getLen()
			{
				int _len = 0;
				// floor
				_len += 4;
				// hardType
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  挑战迷失地牢返回
	/// </summary>
	[Protocol]
	public class LostDungeonChallengeRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510002;
		public UInt32 Sequence;
		/// <summary>
		/// 结果
		/// </summary>
		public UInt32 code;
		/// <summary>
		/// 匹配的战场id
		/// </summary>
		public UInt32 battleId;
		/// <summary>
		/// 匹配的战场的表id
		/// </summary>
		public UInt32 battleDataId;
		/// <summary>
		/// 匹配的战场的场景id
		/// </summary>
		public UInt32 sceneId;
		/// <summary>
		/// 层数
		/// </summary>
		public UInt32 floor;
		/// <summary>
		///  难度
		/// </summary>
		public byte hardType;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint32(buffer, ref pos_, battleId);
				BaseDLL.encode_uint32(buffer, ref pos_, battleDataId);
				BaseDLL.encode_uint32(buffer, ref pos_, sceneId);
				BaseDLL.encode_uint32(buffer, ref pos_, floor);
				BaseDLL.encode_int8(buffer, ref pos_, hardType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleDataId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref sceneId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref floor);
				BaseDLL.decode_int8(buffer, ref pos_, ref hardType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint32(buffer, ref pos_, battleId);
				BaseDLL.encode_uint32(buffer, ref pos_, battleDataId);
				BaseDLL.encode_uint32(buffer, ref pos_, sceneId);
				BaseDLL.encode_uint32(buffer, ref pos_, floor);
				BaseDLL.encode_int8(buffer, ref pos_, hardType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleDataId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref sceneId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref floor);
				BaseDLL.decode_int8(buffer, ref pos_, ref hardType);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// battleId
				_len += 4;
				// battleDataId
				_len += 4;
				// sceneId
				_len += 4;
				// floor
				_len += 4;
				// hardType
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 楼层节点
	/// </summary>
	public class LostDungeonNode : Protocol.IProtocolStream
	{
		/// <summary>
		/// 节点id
		/// </summary>
		public UInt32 id;
		/// <summary>
		/// 宝箱id(宝箱的话)
		/// </summary>
		public UInt32 treasChestId;
		/// <summary>
		/// 状态(枚举LostDungeonNodeState战场门, LostDungeonBoxState(宝箱))
		/// </summary>
		public byte state;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, treasChestId);
				BaseDLL.encode_int8(buffer, ref pos_, state);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref treasChestId);
				BaseDLL.decode_int8(buffer, ref pos_, ref state);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, treasChestId);
				BaseDLL.encode_int8(buffer, ref pos_, state);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref treasChestId);
				BaseDLL.decode_int8(buffer, ref pos_, ref state);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// treasChestId
				_len += 4;
				// state
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 楼层数据
	/// </summary>
	public class LostDungeonFloorData : Protocol.IProtocolStream
	{
		/// <summary>
		/// 第几层
		/// </summary>
		public UInt32 floor;
		/// <summary>
		/// 生成的节点
		/// </summary>
		public LostDungeonNode[] nodes = new LostDungeonNode[0];
		/// <summary>
		/// 楼层状态(枚举LostDungeonFloorState)
		/// </summary>
		public byte state;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, floor);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)nodes.Length);
				for(int i = 0; i < nodes.Length; i++)
				{
					nodes[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_int8(buffer, ref pos_, state);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref floor);
				UInt16 nodesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nodesCnt);
				nodes = new LostDungeonNode[nodesCnt];
				for(int i = 0; i < nodes.Length; i++)
				{
					nodes[i] = new LostDungeonNode();
					nodes[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_int8(buffer, ref pos_, ref state);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, floor);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)nodes.Length);
				for(int i = 0; i < nodes.Length; i++)
				{
					nodes[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_int8(buffer, ref pos_, state);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref floor);
				UInt16 nodesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nodesCnt);
				nodes = new LostDungeonNode[nodesCnt];
				for(int i = 0; i < nodes.Length; i++)
				{
					nodes[i] = new LostDungeonNode();
					nodes[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_int8(buffer, ref pos_, ref state);
			}

			public int getLen()
			{
				int _len = 0;
				// floor
				_len += 4;
				// nodes
				_len += 2;
				for(int j = 0; j < nodes.Length; j++)
				{
					_len += nodes[j].getLen();
				}
				// state
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  开启迷失地牢请求
	/// </summary>
	[Protocol]
	public class LostDungeonOpenReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510003;
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
	/// 开启迷失地牢返回
	/// </summary>
	[Protocol]
	public class LostDungeonOpenRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510004;
		public UInt32 Sequence;
		/// <summary>
		/// 结果
		/// </summary>
		public UInt32 code;
		/// <summary>
		/// 楼层数据
		/// </summary>
		public LostDungeonFloorData[] floorDatas = new LostDungeonFloorData[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)floorDatas.Length);
				for(int i = 0; i < floorDatas.Length; i++)
				{
					floorDatas[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				UInt16 floorDatasCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref floorDatasCnt);
				floorDatas = new LostDungeonFloorData[floorDatasCnt];
				for(int i = 0; i < floorDatas.Length; i++)
				{
					floorDatas[i] = new LostDungeonFloorData();
					floorDatas[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)floorDatas.Length);
				for(int i = 0; i < floorDatas.Length; i++)
				{
					floorDatas[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				UInt16 floorDatasCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref floorDatasCnt);
				floorDatas = new LostDungeonFloorData[floorDatasCnt];
				for(int i = 0; i < floorDatas.Length; i++)
				{
					floorDatas[i] = new LostDungeonFloorData();
					floorDatas[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// floorDatas
				_len += 2;
				for(int j = 0; j < floorDatas.Length; j++)
				{
					_len += floorDatas[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->battle 迷失地牢战场PK请求
	/// </summary>
	[Protocol]
	public class SceneLostDungeonPkReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510011;
		public UInt32 Sequence;
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
				BaseDLL.encode_uint64(buffer, ref pos_, dstId);
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonID);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref dstId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonID);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, dstId);
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonID);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref dstId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonID);
			}

			public int getLen()
			{
				int _len = 0;
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

	/// <summary>
	///  battle->client 战场PK返回
	/// </summary>
	[Protocol]
	public class SceneLostDungeonPkRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510012;
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

	/// <summary>
	///  scene->client 任务列表请求
	/// </summary>
	[Protocol]
	public class SceneLostDungeonTaskListReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510013;
		public UInt32 Sequence;
		/// <summary>
		///  难度
		/// </summary>
		public UInt32 hardType;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, hardType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref hardType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, hardType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref hardType);
			}

			public int getLen()
			{
				int _len = 0;
				// hardType
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  scene->client 任务列表返回
	/// </summary>
	[Protocol]
	public class SceneLostDungeonTaskListNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510005;
		public UInt32 Sequence;
		public UInt32[] taskList = new UInt32[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)taskList.Length);
				for(int i = 0; i < taskList.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, taskList[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 taskListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref taskListCnt);
				taskList = new UInt32[taskListCnt];
				for(int i = 0; i < taskList.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref taskList[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)taskList.Length);
				for(int i = 0; i < taskList.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, taskList[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 taskListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref taskListCnt);
				taskList = new UInt32[taskListCnt];
				for(int i = 0; i < taskList.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref taskList[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// taskList
				_len += 2 + 4 * taskList.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->scene 选择任务请求
	/// </summary>
	[Protocol]
	public class SceneLostDungeonChoiceTaskReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510006;
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

	/// <summary>
	///  scene->client 选择任务返回
	/// </summary>
	[Protocol]
	public class SceneLostDungeonChoiceTaskRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510007;
		public UInt32 Sequence;
		public UInt32 retCode;
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
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// taskId
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class LostDungeonTaskVar : Protocol.IProtocolStream
	{
		public string key;
		public string val;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] keyBytes = StringHelper.StringToUTF8Bytes(key);
				BaseDLL.encode_string(buffer, ref pos_, keyBytes, (UInt16)(buffer.Length - pos_));
				byte[] valBytes = StringHelper.StringToUTF8Bytes(val);
				BaseDLL.encode_string(buffer, ref pos_, valBytes, (UInt16)(buffer.Length - pos_));
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
				UInt16 valLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref valLen);
				byte[] valBytes = new byte[valLen];
				for(int i = 0; i < valLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref valBytes[i]);
				}
				val = StringHelper.BytesToString(valBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] keyBytes = StringHelper.StringToUTF8Bytes(key);
				BaseDLL.encode_string(buffer, ref pos_, keyBytes, (UInt16)(buffer.Length - pos_));
				byte[] valBytes = StringHelper.StringToUTF8Bytes(val);
				BaseDLL.encode_string(buffer, ref pos_, valBytes, (UInt16)(buffer.Length - pos_));
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
				UInt16 valLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref valLen);
				byte[] valBytes = new byte[valLen];
				for(int i = 0; i < valLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref valBytes[i]);
				}
				val = StringHelper.BytesToString(valBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// key
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(key);
					_len += 2 + _strBytes.Length;
				}
				// val
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(val);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  scene->client 任务数据通知
	/// </summary>
	[Protocol]
	public class SceneLostDungeonTaskDataNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510014;
		public UInt32 Sequence;
		public UInt32 taskId;
		public UInt32 status;
		public LostDungeonTaskVar[] ldTaskVar = new LostDungeonTaskVar[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint32(buffer, ref pos_, status);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)ldTaskVar.Length);
				for(int i = 0; i < ldTaskVar.Length; i++)
				{
					ldTaskVar[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref status);
				UInt16 ldTaskVarCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref ldTaskVarCnt);
				ldTaskVar = new LostDungeonTaskVar[ldTaskVarCnt];
				for(int i = 0; i < ldTaskVar.Length; i++)
				{
					ldTaskVar[i] = new LostDungeonTaskVar();
					ldTaskVar[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
				BaseDLL.encode_uint32(buffer, ref pos_, status);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)ldTaskVar.Length);
				for(int i = 0; i < ldTaskVar.Length; i++)
				{
					ldTaskVar[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref status);
				UInt16 ldTaskVarCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref ldTaskVarCnt);
				ldTaskVar = new LostDungeonTaskVar[ldTaskVarCnt];
				for(int i = 0; i < ldTaskVar.Length; i++)
				{
					ldTaskVar[i] = new LostDungeonTaskVar();
					ldTaskVar[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// taskId
				_len += 4;
				// status
				_len += 4;
				// ldTaskVar
				_len += 2;
				for(int j = 0; j < ldTaskVar.Length; j++)
				{
					_len += ldTaskVar[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  scene->client 任务结果通知
	/// </summary>
	[Protocol]
	public class SceneLostDungeonTaskResultNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510015;
		public UInt32 Sequence;
		public UInt32 taskId;
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
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// taskId
				_len += 4;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->scene  选择地下城挑战模式请求
	/// </summary>
	[Protocol]
	public class SceneLostDungeonSwitchChageModeReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510008;
		public UInt32 Sequence;
		public byte chageMode;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, chageMode);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref chageMode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, chageMode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref chageMode);
			}

			public int getLen()
			{
				int _len = 0;
				// chageMode
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// scene->client 选择地下城挑战模式返回
	/// </summary>
	[Protocol]
	public class SceneLostDungeonSwitchChageModeRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510009;
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
	/// 队伍信息
	/// </summary>
	public class LostDungTeamInfo : Protocol.IProtocolStream
	{
		/// <summary>
		/// 队伍id
		/// </summary>
		public UInt32 teamId;
		/// <summary>
		/// 队伍索引[1-4]
		/// </summary>
		public byte index;
		/// <summary>
		/// 战斗状态枚举LostTeamBattleSt
		/// </summary>
		public byte battleState;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
				BaseDLL.encode_int8(buffer, ref pos_, index);
				BaseDLL.encode_int8(buffer, ref pos_, battleState);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_int8(buffer, ref pos_, ref index);
				BaseDLL.decode_int8(buffer, ref pos_, ref battleState);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
				BaseDLL.encode_int8(buffer, ref pos_, index);
				BaseDLL.encode_int8(buffer, ref pos_, battleState);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_int8(buffer, ref pos_, ref index);
				BaseDLL.decode_int8(buffer, ref pos_, ref battleState);
			}

			public int getLen()
			{
				int _len = 0;
				// teamId
				_len += 4;
				// index
				_len += 1;
				// battleState
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 队员信息
	/// </summary>
	public class LostDungTeamMember : Protocol.IProtocolStream
	{
		/// <summary>
		/// 角色id
		/// </summary>
		public UInt64 roleId;
		/// <summary>
		/// 队伍id
		/// </summary>
		public UInt32 teamId;
		/// <summary>
		/// 队伍中位置
		/// </summary>
		public byte pos;
		/// <summary>
		/// 名字
		/// </summary>
		public string name;
		/// <summary>
		/// 等级
		/// </summary>
		public UInt16 level;
		/// <summary>
		/// 职业
		/// </summary>
		public byte occu;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
				BaseDLL.encode_int8(buffer, ref pos_, pos);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, occu);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_int8(buffer, ref pos_, ref pos);
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
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
				BaseDLL.encode_int8(buffer, ref pos_, pos);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, occu);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
				BaseDLL.decode_int8(buffer, ref pos_, ref pos);
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
			}

			public int getLen()
			{
				int _len = 0;
				// roleId
				_len += 8;
				// teamId
				_len += 4;
				// pos
				_len += 1;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// level
				_len += 2;
				// occu
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// scene->client 同步地下城组队信息
	/// </summary>
	[Protocol]
	public class SceneLostDungeonSyncDungeonTeamInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510010;
		public UInt32 Sequence;
		public byte type;
		/// <summary>
		/// 枚举LostDungChangeMode
		/// </summary>
		public byte challengeMode;
		public LostDungTeamMember[] teamMemebers = new LostDungTeamMember[0];
		public LostDungTeamMember addMember = new LostDungTeamMember();
		public UInt64 leavePlayerId;
		public LostDungTeamInfo[] teamInfos = new LostDungTeamInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_int8(buffer, ref pos_, challengeMode);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)teamMemebers.Length);
				for(int i = 0; i < teamMemebers.Length; i++)
				{
					teamMemebers[i].encode(buffer, ref pos_);
				}
				addMember.encode(buffer, ref pos_);
				BaseDLL.encode_uint64(buffer, ref pos_, leavePlayerId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)teamInfos.Length);
				for(int i = 0; i < teamInfos.Length; i++)
				{
					teamInfos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref challengeMode);
				UInt16 teamMemebersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref teamMemebersCnt);
				teamMemebers = new LostDungTeamMember[teamMemebersCnt];
				for(int i = 0; i < teamMemebers.Length; i++)
				{
					teamMemebers[i] = new LostDungTeamMember();
					teamMemebers[i].decode(buffer, ref pos_);
				}
				addMember.decode(buffer, ref pos_);
				BaseDLL.decode_uint64(buffer, ref pos_, ref leavePlayerId);
				UInt16 teamInfosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref teamInfosCnt);
				teamInfos = new LostDungTeamInfo[teamInfosCnt];
				for(int i = 0; i < teamInfos.Length; i++)
				{
					teamInfos[i] = new LostDungTeamInfo();
					teamInfos[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, challengeMode);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)teamMemebers.Length);
				for(int i = 0; i < teamMemebers.Length; i++)
				{
					teamMemebers[i].encode(buffer, ref pos_);
				}
				addMember.encode(buffer, ref pos_);
				BaseDLL.encode_uint64(buffer, ref pos_, leavePlayerId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)teamInfos.Length);
				for(int i = 0; i < teamInfos.Length; i++)
				{
					teamInfos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref challengeMode);
				UInt16 teamMemebersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref teamMemebersCnt);
				teamMemebers = new LostDungTeamMember[teamMemebersCnt];
				for(int i = 0; i < teamMemebers.Length; i++)
				{
					teamMemebers[i] = new LostDungTeamMember();
					teamMemebers[i].decode(buffer, ref pos_);
				}
				addMember.decode(buffer, ref pos_);
				BaseDLL.decode_uint64(buffer, ref pos_, ref leavePlayerId);
				UInt16 teamInfosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref teamInfosCnt);
				teamInfos = new LostDungTeamInfo[teamInfosCnt];
				for(int i = 0; i < teamInfos.Length; i++)
				{
					teamInfos[i] = new LostDungTeamInfo();
					teamInfos[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// challengeMode
				_len += 1;
				// teamMemebers
				_len += 2;
				for(int j = 0; j < teamMemebers.Length; j++)
				{
					_len += teamMemebers[j].getLen();
				}
				// addMember
				_len += addMember.getLen();
				// leavePlayerId
				_len += 8;
				// teamInfos
				_len += 2;
				for(int j = 0; j < teamInfos.Length; j++)
				{
					_len += teamInfos[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// battle->client 通知开始地下城投票
	/// </summary>
	[Protocol]
	public class BattleTeamRaceVoteNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 2210002;
		public UInt32 Sequence;
		public UInt32 dungeonId;
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
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
				BaseDLL.encode_uint32(buffer, ref pos_, teamId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
			}

			public int getLen()
			{
				int _len = 0;
				// dungeonId
				_len += 4;
				// teamId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->battle  玩家上报投票选项
	/// </summary>
	[Protocol]
	public class BattleTeamReportVoteChoice : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 2210003;
		public UInt32 Sequence;
		public UInt64 roleId;
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
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_int8(buffer, ref pos_, agree);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_int8(buffer, ref pos_, ref agree);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_int8(buffer, ref pos_, agree);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_int8(buffer, ref pos_, ref agree);
			}

			public int getLen()
			{
				int _len = 0;
				// roleId
				_len += 8;
				// agree
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// battle->client 广播玩家玩家投票选项
	/// </summary>
	[Protocol]
	public class BattleTeamVoteChoiceNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 2210004;
		public UInt32 Sequence;
		public UInt64 roleId;
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
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_int8(buffer, ref pos_, agree);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_int8(buffer, ref pos_, ref agree);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_int8(buffer, ref pos_, agree);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_int8(buffer, ref pos_, ref agree);
			}

			public int getLen()
			{
				int _len = 0;
				// roleId
				_len += 8;
				// agree
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->scene 地牢开宝箱请求
	/// </summary>
	[Protocol]
	public class LostDungeonOpenBoxReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510016;
		public UInt32 Sequence;
		public UInt32 floor;
		public UInt32 boxId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, floor);
				BaseDLL.encode_uint32(buffer, ref pos_, boxId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref floor);
				BaseDLL.decode_uint32(buffer, ref pos_, ref boxId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, floor);
				BaseDLL.encode_uint32(buffer, ref pos_, boxId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref floor);
				BaseDLL.decode_uint32(buffer, ref pos_, ref boxId);
			}

			public int getLen()
			{
				int _len = 0;
				// floor
				_len += 4;
				// boxId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// scene->client 地牢开宝箱返回
	/// </summary>
	[Protocol]
	public class LostDungeonOpenBoxRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510017;
		public UInt32 Sequence;
		public UInt32 code;
		public ItemReward[] itemVec = new ItemReward[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemVec.Length);
				for(int i = 0; i < itemVec.Length; i++)
				{
					itemVec[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				UInt16 itemVecCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemVecCnt);
				itemVec = new ItemReward[itemVecCnt];
				for(int i = 0; i < itemVec.Length; i++)
				{
					itemVec[i] = new ItemReward();
					itemVec[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemVec.Length);
				for(int i = 0; i < itemVec.Length; i++)
				{
					itemVec[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				UInt16 itemVecCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemVecCnt);
				itemVec = new ItemReward[itemVecCnt];
				for(int i = 0; i < itemVec.Length; i++)
				{
					itemVec[i] = new ItemReward();
					itemVec[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// itemVec
				_len += 2;
				for(int j = 0; j < itemVec.Length; j++)
				{
					_len += itemVec[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// scene->client 同步楼层数据 
	/// </summary>
	[Protocol]
	public class LostDungeonSyncFloor : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510018;
		public UInt32 Sequence;
		public LostDungeonFloorData[] floorData = new LostDungeonFloorData[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)floorData.Length);
				for(int i = 0; i < floorData.Length; i++)
				{
					floorData[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 floorDataCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref floorDataCnt);
				floorData = new LostDungeonFloorData[floorDataCnt];
				for(int i = 0; i < floorData.Length; i++)
				{
					floorData[i] = new LostDungeonFloorData();
					floorData[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)floorData.Length);
				for(int i = 0; i < floorData.Length; i++)
				{
					floorData[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 floorDataCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref floorDataCnt);
				floorData = new LostDungeonFloorData[floorDataCnt];
				for(int i = 0; i < floorData.Length; i++)
				{
					floorData[i] = new LostDungeonFloorData();
					floorData[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// floorData
				_len += 2;
				for(int j = 0; j < floorData.Length; j++)
				{
					_len += floorData[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  scene->client 同步资源列表
	/// </summary>
	[Protocol]
	public class SceneLostDungeonSyncResourcesList : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510019;
		public UInt32 Sequence;
		public UInt32 battleID;
		public SceneItemInfo[] infoes = new SceneItemInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)infoes.Length);
				for(int i = 0; i < infoes.Length; i++)
				{
					infoes[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				UInt16 infoesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref infoesCnt);
				infoes = new SceneItemInfo[infoesCnt];
				for(int i = 0; i < infoes.Length; i++)
				{
					infoes[i] = new SceneItemInfo();
					infoes[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)infoes.Length);
				for(int i = 0; i < infoes.Length; i++)
				{
					infoes[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				UInt16 infoesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref infoesCnt);
				infoes = new SceneItemInfo[infoesCnt];
				for(int i = 0; i < infoes.Length; i++)
				{
					infoes[i] = new SceneItemInfo();
					infoes[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// battleID
				_len += 4;
				// infoes
				_len += 2;
				for(int j = 0; j < infoes.Length; j++)
				{
					_len += infoes[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  scene->client 新增资源
	/// </summary>
	[Protocol]
	public class SceneLostDungeonResourceAdd : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510020;
		public UInt32 Sequence;
		public UInt32 battleID;
		public SceneItemInfo[] data = new SceneItemInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)data.Length);
				for(int i = 0; i < data.Length; i++)
				{
					data[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				UInt16 dataCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref dataCnt);
				data = new SceneItemInfo[dataCnt];
				for(int i = 0; i < data.Length; i++)
				{
					data[i] = new SceneItemInfo();
					data[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)data.Length);
				for(int i = 0; i < data.Length; i++)
				{
					data[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				UInt16 dataCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref dataCnt);
				data = new SceneItemInfo[dataCnt];
				for(int i = 0; i < data.Length; i++)
				{
					data[i] = new SceneItemInfo();
					data[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// battleID
				_len += 4;
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
	///  scene->client 删除资源
	/// </summary>
	[Protocol]
	public class SceneLostDungeonResourceDel : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510021;
		public UInt32 Sequence;
		public UInt32 battleID;
		/// <summary>
		///  item guid列表
		/// </summary>
		public UInt64[] guids = new UInt64[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)guids.Length);
				for(int i = 0; i < guids.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, guids[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				UInt16 guidsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guidsCnt);
				guids = new UInt64[guidsCnt];
				for(int i = 0; i < guids.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref guids[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)guids.Length);
				for(int i = 0; i < guids.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, guids[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				UInt16 guidsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guidsCnt);
				guids = new UInt64[guidsCnt];
				for(int i = 0; i < guids.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref guids[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// battleID
				_len += 4;
				// guids
				_len += 2 + 8 * guids.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 宝箱选择道具请求
	/// </summary>
	[Protocol]
	public class SceneLostDungeonBoxChoiceItemReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510022;
		public UInt32 Sequence;
		public UInt32 itemId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
			}

			public int getLen()
			{
				int _len = 0;
				// itemId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 宝箱选择道具返回
	/// </summary>
	[Protocol]
	public class SceneLostDungeonBoxChoiceItemRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510023;
		public UInt32 Sequence;
		public UInt32 retCode;
		public ItemReward item = new ItemReward();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				item.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				item.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				item.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				item.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// item
				_len += item.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 结束
	/// </summary>
	/// <summary>
	/// scene->client 通知客户端爬塔楼层通关
	/// </summary>
	[Protocol]
	public class SceneLostDungeonSettleFlooorNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510024;
		public UInt32 Sequence;
		public UInt32 floor;
		public byte battleResult;
		public UInt32 addScore;
		public UInt32 decScore;
		public UInt32 score;
		public UInt32 againItemId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, floor);
				BaseDLL.encode_int8(buffer, ref pos_, battleResult);
				BaseDLL.encode_uint32(buffer, ref pos_, addScore);
				BaseDLL.encode_uint32(buffer, ref pos_, decScore);
				BaseDLL.encode_uint32(buffer, ref pos_, score);
				BaseDLL.encode_uint32(buffer, ref pos_, againItemId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref floor);
				BaseDLL.decode_int8(buffer, ref pos_, ref battleResult);
				BaseDLL.decode_uint32(buffer, ref pos_, ref addScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref decScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref score);
				BaseDLL.decode_uint32(buffer, ref pos_, ref againItemId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, floor);
				BaseDLL.encode_int8(buffer, ref pos_, battleResult);
				BaseDLL.encode_uint32(buffer, ref pos_, addScore);
				BaseDLL.encode_uint32(buffer, ref pos_, decScore);
				BaseDLL.encode_uint32(buffer, ref pos_, score);
				BaseDLL.encode_uint32(buffer, ref pos_, againItemId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref floor);
				BaseDLL.decode_int8(buffer, ref pos_, ref battleResult);
				BaseDLL.decode_uint32(buffer, ref pos_, ref addScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref decScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref score);
				BaseDLL.decode_uint32(buffer, ref pos_, ref againItemId);
			}

			public int getLen()
			{
				int _len = 0;
				// floor
				_len += 4;
				// battleResult
				_len += 1;
				// addScore
				_len += 4;
				// decScore
				_len += 4;
				// score
				_len += 4;
				// againItemId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->scene 爬塔领取奖励
	/// </summary>
	[Protocol]
	public class SceneLostDungeonGetRewardReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510025;
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
	/// scene->client 爬塔领取奖励返回
	/// </summary>
	[Protocol]
	public class SceneLostDungeonGetRewardRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510026;
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
	/// client->scene 放弃爬塔请求
	/// </summary>
	[Protocol]
	public class SceneLostDungeonGiveUpReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510027;
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
	/// scene->client 放弃爬塔返回
	/// </summary>
	[Protocol]
	public class SceneLostDungeonGiveUpRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510028;
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
	/// scene->client 同步迷失战场数据
	/// </summary>
	[Protocol]
	public class BattleLostDungSyncBattleData : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510029;
		public UInt32 Sequence;
		public UInt32 battleId;
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
				BaseDLL.encode_uint32(buffer, ref pos_, battleId);
				BaseDLL.encode_uint32(buffer, ref pos_, playerNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, battleId);
				BaseDLL.encode_uint32(buffer, ref pos_, playerNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerNum);
			}

			public int getLen()
			{
				int _len = 0;
				// battleId
				_len += 4;
				// playerNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->battleScene  查看玩家情报
	/// </summary>
	[Protocol]
	public class BattleLostDungSeeIntellReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510030;
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
	/// battleScene->client  查看玩家情报返回
	/// </summary>
	[Protocol]
	public class BattleLostDungSeeIntellRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510031;
		public UInt32 Sequence;
		public UInt32 code;
		public UInt64 playerId;
		public ItemReward[] artifacts = new ItemReward[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)artifacts.Length);
				for(int i = 0; i < artifacts.Length; i++)
				{
					artifacts[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
				UInt16 artifactsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref artifactsCnt);
				artifacts = new ItemReward[artifactsCnt];
				for(int i = 0; i < artifacts.Length; i++)
				{
					artifacts[i] = new ItemReward();
					artifacts[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)artifacts.Length);
				for(int i = 0; i < artifacts.Length; i++)
				{
					artifacts[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
				UInt16 artifactsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref artifactsCnt);
				artifacts = new ItemReward[artifactsCnt];
				for(int i = 0; i < artifacts.Length; i++)
				{
					artifacts[i] = new ItemReward();
					artifacts[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// playerId
				_len += 8;
				// artifacts
				_len += 2;
				for(int j = 0; j < artifacts.Length; j++)
				{
					_len += artifacts[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// battleScene->client  敌人是否在地下城
	/// </summary>
	[Protocol]
	public class SceneLostDungeonEnemyInDungeon : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510032;
		public UInt32 Sequence;
		/// <summary>
		///  敌方id
		/// </summary>
		public UInt64 enemyId;
		/// <summary>
		///  地下城id
		/// </summary>
		public UInt32 dungeonId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, enemyId);
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref enemyId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, enemyId);
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref enemyId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
			}

			public int getLen()
			{
				int _len = 0;
				// enemyId
				_len += 8;
				// dungeonId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// battleScene->client  通知客户端离开战场
	/// </summary>
	[Protocol]
	public class SceneNotifyClientLeaveBattle : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 2210011;
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
	/// client->scene  查询击杀目标坐标请求
	/// </summary>
	[Protocol]
	public class SceneQueryKillTargetPosReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510033;
		public UInt32 Sequence;
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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)playerIds.Length);
				for(int i = 0; i < playerIds.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, playerIds[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)playerIds.Length);
				for(int i = 0; i < playerIds.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, playerIds[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
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
				// playerIds
				_len += 2 + 8 * playerIds.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 地牢玩家坐标信息
	/// </summary>
	public class LostDungeonPlayerPos : Protocol.IProtocolStream
	{
		/// <summary>
		/// 玩家id
		/// </summary>
		public UInt64 playerId;
		/// <summary>
		/// 场景id
		/// </summary>
		public UInt32 sceneId;
		/// <summary>
		/// x坐标
		/// </summary>
		public UInt32 posX;
		/// <summary>
		/// y坐标
		/// </summary>
		public UInt32 posY;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				BaseDLL.encode_uint32(buffer, ref pos_, sceneId);
				BaseDLL.encode_uint32(buffer, ref pos_, posX);
				BaseDLL.encode_uint32(buffer, ref pos_, posY);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref sceneId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref posX);
				BaseDLL.decode_uint32(buffer, ref pos_, ref posY);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				BaseDLL.encode_uint32(buffer, ref pos_, sceneId);
				BaseDLL.encode_uint32(buffer, ref pos_, posX);
				BaseDLL.encode_uint32(buffer, ref pos_, posY);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref sceneId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref posX);
				BaseDLL.decode_uint32(buffer, ref pos_, ref posY);
			}

			public int getLen()
			{
				int _len = 0;
				// playerId
				_len += 8;
				// sceneId
				_len += 4;
				// posX
				_len += 4;
				// posY
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// scene->client  查询击杀目标坐标返回
	/// </summary>
	[Protocol]
	public class SceneQueryKillTargetPosRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 510034;
		public UInt32 Sequence;
		public LostDungeonPlayerPos[] playerPos = new LostDungeonPlayerPos[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)playerPos.Length);
				for(int i = 0; i < playerPos.Length; i++)
				{
					playerPos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 playerPosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerPosCnt);
				playerPos = new LostDungeonPlayerPos[playerPosCnt];
				for(int i = 0; i < playerPos.Length; i++)
				{
					playerPos[i] = new LostDungeonPlayerPos();
					playerPos[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)playerPos.Length);
				for(int i = 0; i < playerPos.Length; i++)
				{
					playerPos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 playerPosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerPosCnt);
				playerPos = new LostDungeonPlayerPos[playerPosCnt];
				for(int i = 0; i < playerPos.Length; i++)
				{
					playerPos[i] = new LostDungeonPlayerPos();
					playerPos[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// playerPos
				_len += 2;
				for(int j = 0; j < playerPos.Length; j++)
				{
					_len += playerPos[j].getLen();
				}
				return _len;
			}
		#endregion

	}

}
