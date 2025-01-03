using System;
using System.Text;

namespace Protocol
{
	public enum SceneObjectStatus
	{
		SOS_STAND = 0,
		SOS_WALK = 2,
	}

	/// <summary>
	///  红点标记
	/// </summary>
	public enum RedPointFlag
	{
		/// <summary>
		///  公会请求者
		/// </summary>
		GUILD_REQUESTER = 0,
		/// <summary>
		///  公会商店
		/// </summary>
		GUILD_SHOP = 1,
		/// <summary>
		///  公会兼并
		/// </summary>
		GUILD_MERGER = 4,
		/// <summary>
		///  公会战领地每日奖励
		/// </summary>
		GUILD_BATTLE_TERR_DAY_REWARD = 5,
	}

	/// <summary>
	///  场景obj类型
	/// </summary>
	public enum SceneObjectType
	{
		/// <summary>
		///  城镇怪物
		/// </summary>
		SOT_CITYMONSTER = 9,
	}

	public class ScenePosition : Protocol.IProtocolStream
	{
		public UInt32 x;
		public UInt32 y;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, x);
				BaseDLL.encode_uint32(buffer, ref pos_, y);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref x);
				BaseDLL.decode_uint32(buffer, ref pos_, ref y);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, x);
				BaseDLL.encode_uint32(buffer, ref pos_, y);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref x);
				BaseDLL.decode_uint32(buffer, ref pos_, ref y);
			}

			public int getLen()
			{
				int _len = 0;
				// x
				_len += 4;
				// y
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class SceneDir : Protocol.IProtocolStream
	{
		public Int16 x;
		public Int16 y;
		public byte faceRight;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int16(buffer, ref pos_, x);
				BaseDLL.encode_int16(buffer, ref pos_, y);
				BaseDLL.encode_int8(buffer, ref pos_, faceRight);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int16(buffer, ref pos_, ref x);
				BaseDLL.decode_int16(buffer, ref pos_, ref y);
				BaseDLL.decode_int8(buffer, ref pos_, ref faceRight);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int16(buffer, ref pos_, x);
				BaseDLL.encode_int16(buffer, ref pos_, y);
				BaseDLL.encode_int8(buffer, ref pos_, faceRight);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int16(buffer, ref pos_, ref x);
				BaseDLL.decode_int16(buffer, ref pos_, ref y);
				BaseDLL.decode_int8(buffer, ref pos_, ref faceRight);
			}

			public int getLen()
			{
				int _len = 0;
				// x
				_len += 2;
				// y
				_len += 2;
				// faceRight
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  npc坐标
	/// </summary>
	public class NpcPos : Protocol.IProtocolStream
	{
		public Int32 x;
		public Int32 y;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int32(buffer, ref pos_, x);
				BaseDLL.encode_int32(buffer, ref pos_, y);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int32(buffer, ref pos_, ref x);
				BaseDLL.decode_int32(buffer, ref pos_, ref y);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int32(buffer, ref pos_, x);
				BaseDLL.encode_int32(buffer, ref pos_, y);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int32(buffer, ref pos_, ref x);
				BaseDLL.decode_int32(buffer, ref pos_, ref y);
			}

			public int getLen()
			{
				int _len = 0;
				// x
				_len += 4;
				// y
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  城镇npc（目前就支持城镇怪物）
	/// </summary>
	public class SceneNpc : Protocol.IProtocolStream
	{
		/// <summary>
		///  唯一ID
		/// </summary>
		public UInt64 guid;
		/// <summary>
		///  npc类型，对应枚举SceneObjectType
		/// </summary>
		public byte type;
		/// <summary>
		///  功能类型（不同SceneObjectType有不同的含义）
		/// </summary>
		/// <summary>
		///  SOT_CITYMONSTER -> CityMonsterType
		/// </summary>
		public byte funcType;
		/// <summary>
		///  类型ID
		/// </summary>
		public UInt32 id;
		/// <summary>
		///  位置
		/// </summary>
		public NpcPos pos = new NpcPos();
		/// <summary>
		///  剩余次数
		/// </summary>
		public UInt32 remainTimes;
		/// <summary>
		///  总次数
		/// </summary>
		public UInt32 totalTimes;
		/// <summary>
		///  对应的地下城
		/// </summary>
		public UInt32 dungeonId;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, funcType);
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				pos.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, remainTimes);
				BaseDLL.encode_uint32(buffer, ref pos_, totalTimes);
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref funcType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				pos.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref remainTimes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalTimes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, funcType);
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				pos.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, remainTimes);
				BaseDLL.encode_uint32(buffer, ref pos_, totalTimes);
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref funcType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				pos.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref remainTimes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalTimes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
			}

			public int getLen()
			{
				int _len = 0;
				// guid
				_len += 8;
				// type
				_len += 1;
				// funcType
				_len += 1;
				// id
				_len += 4;
				// pos
				_len += pos.getLen();
				// remainTimes
				_len += 4;
				// totalTimes
				_len += 4;
				// dungeonId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  场景npc信息
	/// </summary>
	public class SceneNpcInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  场景ID
		/// </summary>
		public UInt32 sceneId;
		/// <summary>
		///  所有npc信息
		/// </summary>
		public SceneNpc[] npcs = new SceneNpc[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, sceneId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)npcs.Length);
				for(int i = 0; i < npcs.Length; i++)
				{
					npcs[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref sceneId);
				UInt16 npcsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref npcsCnt);
				npcs = new SceneNpc[npcsCnt];
				for(int i = 0; i < npcs.Length; i++)
				{
					npcs[i] = new SceneNpc();
					npcs[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, sceneId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)npcs.Length);
				for(int i = 0; i < npcs.Length; i++)
				{
					npcs[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref sceneId);
				UInt16 npcsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref npcsCnt);
				npcs = new SceneNpc[npcsCnt];
				for(int i = 0; i < npcs.Length; i++)
				{
					npcs[i] = new SceneNpc();
					npcs[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// sceneId
				_len += 4;
				// npcs
				_len += 2;
				for(int j = 0; j < npcs.Length; j++)
				{
					_len += npcs[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneNotifyEnterScene : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500303;
		public UInt32 Sequence;
		public UInt32 result;
		public UInt32 mapid;
		public string name;
		public ScenePosition pos = new ScenePosition();
		public SceneDir dir = new SceneDir();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint32(buffer, ref pos_, mapid);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				pos.encode(buffer, ref pos_);
				dir.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref mapid);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				pos.decode(buffer, ref pos_);
				dir.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, mapid);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				pos.encode(buffer, ref pos_);
				dir.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref mapid);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				pos.decode(buffer, ref pos_);
				dir.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// mapid
				_len += 4;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// pos
				_len += pos.getLen();
				// dir
				_len += dir.getLen();
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneMoveRequire : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500501;
		public UInt32 Sequence;
		/// <summary>
		///  客户端所在场景
		/// </summary>
		public UInt32 clientSceneId;
		public ScenePosition pos = new ScenePosition();
		public SceneDir dir = new SceneDir();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, clientSceneId);
				pos.encode(buffer, ref pos_);
				dir.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref clientSceneId);
				pos.decode(buffer, ref pos_);
				dir.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, clientSceneId);
				pos.encode(buffer, ref pos_);
				dir.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref clientSceneId);
				pos.decode(buffer, ref pos_);
				dir.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// clientSceneId
				_len += 4;
				// pos
				_len += pos.getLen();
				// dir
				_len += dir.getLen();
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSyncPlayerMove : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500502;
		public UInt32 Sequence;
		public UInt64 id;
		public ScenePosition pos = new ScenePosition();
		public SceneDir dir = new SceneDir();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				pos.encode(buffer, ref pos_);
				dir.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				pos.decode(buffer, ref pos_);
				dir.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				pos.encode(buffer, ref pos_);
				dir.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				pos.decode(buffer, ref pos_);
				dir.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// pos
				_len += pos.getLen();
				// dir
				_len += dir.getLen();
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSynSelf : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500601;
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
	public class SceneSyncSceneObject : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500602;
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
	public class SceneSyncObjectProperty : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500603;
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
	public class SceneDeleteSceneObject : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500604;
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
	public class SceneReturnToTown : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500517;
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
	public class ScenePlayerChangeSceneReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500503;
		public UInt32 Sequence;
		public UInt32 curDoorId;
		public UInt32 dstSceneId;
		public UInt32 dstDoorId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, curDoorId);
				BaseDLL.encode_uint32(buffer, ref pos_, dstSceneId);
				BaseDLL.encode_uint32(buffer, ref pos_, dstDoorId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref curDoorId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dstSceneId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dstDoorId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, curDoorId);
				BaseDLL.encode_uint32(buffer, ref pos_, dstSceneId);
				BaseDLL.encode_uint32(buffer, ref pos_, dstDoorId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref curDoorId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dstSceneId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dstDoorId);
			}

			public int getLen()
			{
				int _len = 0;
				// curDoorId
				_len += 4;
				// dstSceneId
				_len += 4;
				// dstDoorId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  清除公会红点
	/// </summary>
	[Protocol]
	public class SceneClearRedPoint : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500617;
		public UInt32 Sequence;
		/// <summary>
		///  红点类型（对应枚举RedPointFlag）
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
				BaseDLL.encode_uint32(buffer, ref pos_, flag);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref flag);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, flag);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref flag);
			}

			public int getLen()
			{
				int _len = 0;
				// flag
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  设置用户自定义字段
	/// </summary>
	[Protocol]
	public class SceneSetCustomData : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500620;
		public UInt32 Sequence;
		public UInt32 data;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, data);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref data);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, data);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref data);
			}

			public int getLen()
			{
				int _len = 0;
				// data
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  npc列表
	/// </summary>
	[Protocol]
	public class SceneNpcList : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500621;
		public UInt32 Sequence;
		/// <summary>
		///  所有场景的npc信息
		/// </summary>
		public SceneNpcInfo[] infoes = new SceneNpcInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)infoes.Length);
				for(int i = 0; i < infoes.Length; i++)
				{
					infoes[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 infoesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref infoesCnt);
				infoes = new SceneNpcInfo[infoesCnt];
				for(int i = 0; i < infoes.Length; i++)
				{
					infoes[i] = new SceneNpcInfo();
					infoes[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)infoes.Length);
				for(int i = 0; i < infoes.Length; i++)
				{
					infoes[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 infoesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref infoesCnt);
				infoes = new SceneNpcInfo[infoesCnt];
				for(int i = 0; i < infoes.Length; i++)
				{
					infoes[i] = new SceneNpcInfo();
					infoes[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
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
	///  新增npc
	/// </summary>
	[Protocol]
	public class SceneNpcAdd : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500622;
		public UInt32 Sequence;
		/// <summary>
		///  新增的npc
		/// </summary>
		public SceneNpcInfo[] data = new SceneNpcInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				data = new SceneNpcInfo[dataCnt];
				for(int i = 0; i < data.Length; i++)
				{
					data[i] = new SceneNpcInfo();
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
				data = new SceneNpcInfo[dataCnt];
				for(int i = 0; i < data.Length; i++)
				{
					data[i] = new SceneNpcInfo();
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
	///  删除npc
	/// </summary>
	[Protocol]
	public class SceneNpcDel : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500623;
		public UInt32 Sequence;
		/// <summary>
		///  npc guid列表
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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)guids.Length);
				for(int i = 0; i < guids.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, guids[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)guids.Length);
				for(int i = 0; i < guids.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, guids[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
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
				// guids
				_len += 2 + 8 * guids.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  修改npc状态
	/// </summary>
	[Protocol]
	public class SceneNpcUpdate : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500624;
		public UInt32 Sequence;
		/// <summary>
		///  npc最新信息
		/// </summary>
		public SceneNpcInfo data = new SceneNpcInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
	///  通知客户端loading情况
	/// </summary>
	[Protocol]
	public class SceneNotifyLoadingInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500117;
		public UInt32 Sequence;
		/// <summary>
		///  是否在loading中
		/// </summary>
		public byte isLoading;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, isLoading);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isLoading);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, isLoading);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isLoading);
			}

			public int getLen()
			{
				int _len = 0;
				// isLoading
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求客户端loading情况
	/// </summary>
	[Protocol]
	public class SceneQueryLoadingInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500118;
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
	///  场景物品坐标
	/// </summary>
	public class SceneItemPos : Protocol.IProtocolStream
	{
		public UInt32 x;
		public UInt32 y;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, x);
				BaseDLL.encode_uint32(buffer, ref pos_, y);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref x);
				BaseDLL.decode_uint32(buffer, ref pos_, ref y);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, x);
				BaseDLL.encode_uint32(buffer, ref pos_, y);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref x);
				BaseDLL.decode_uint32(buffer, ref pos_, ref y);
			}

			public int getLen()
			{
				int _len = 0;
				// x
				_len += 4;
				// y
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  场景物品
	/// </summary>
	public class SceneItem : Protocol.IProtocolStream
	{
		/// <summary>
		///  唯一ID
		/// </summary>
		public UInt64 guid;
		/// <summary>
		///  类型ID
		/// </summary>
		public UInt32 data_id;
		/// <summary>
		///  位置
		/// </summary>
		public SceneItemPos pos = new SceneItemPos();
		/// <summary>
		///  归属
		/// </summary>
		public UInt64 owner;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, data_id);
				pos.encode(buffer, ref pos_);
				BaseDLL.encode_uint64(buffer, ref pos_, owner);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref data_id);
				pos.decode(buffer, ref pos_);
				BaseDLL.decode_uint64(buffer, ref pos_, ref owner);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, data_id);
				pos.encode(buffer, ref pos_);
				BaseDLL.encode_uint64(buffer, ref pos_, owner);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref data_id);
				pos.decode(buffer, ref pos_);
				BaseDLL.decode_uint64(buffer, ref pos_, ref owner);
			}

			public int getLen()
			{
				int _len = 0;
				// guid
				_len += 8;
				// data_id
				_len += 4;
				// pos
				_len += pos.getLen();
				// owner
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  场景物品信息
	/// </summary>
	public class SceneItemInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  场景ID
		/// </summary>
		public UInt32 sceneId;
		/// <summary>
		///  所有item信息
		/// </summary>
		public SceneItem[] items = new SceneItem[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, sceneId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref sceneId);
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new SceneItem[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new SceneItem();
					items[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, sceneId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref sceneId);
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new SceneItem[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new SceneItem();
					items[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// sceneId
				_len += 4;
				// items
				_len += 2;
				for(int j = 0; j < items.Length; j++)
				{
					_len += items[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  场景item列表
	/// </summary>
	[Protocol]
	public class SceneItemList : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500625;
		public UInt32 Sequence;
		public UInt32 battleID;
		/// <summary>
		///  所有场景的物品信息
		/// </summary>
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
	///  新增item
	/// </summary>
	[Protocol]
	public class SceneItemAdd : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500626;
		public UInt32 Sequence;
		public UInt32 battleID;
		/// <summary>
		///  新增的item
		/// </summary>
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
	///  删除item
	/// </summary>
	[Protocol]
	public class SceneItemDel : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500627;
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
	///  修改item状态
	/// </summary>
	[Protocol]
	public class SceneItemUpdate : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500628;
		public UInt32 Sequence;
		public UInt32 battleID;
		/// <summary>
		///  item最新信息
		/// </summary>
		public SceneItemInfo data = new SceneItemInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				data.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				data.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				data.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				data.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// battleID
				_len += 4;
				// data
				_len += data.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  同步item列表
	/// </summary>
	[Protocol]
	public class SceneItemSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500629;
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
	public class SceneBattleBirthTransfer : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508916;
		public UInt32 Sequence;
		public UInt32 battleID;
		public UInt32 regionID;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint32(buffer, ref pos_, regionID);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref regionID);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint32(buffer, ref pos_, regionID);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref regionID);
			}

			public int getLen()
			{
				int _len = 0;
				// battleID
				_len += 4;
				// regionID
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneBattleBirthTransferNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508917;
		public UInt32 Sequence;
		public UInt32 battleID;
		public UInt64 playerID;
		public UInt32 birthPosX;
		public UInt32 birthPosY;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint32(buffer, ref pos_, birthPosX);
				BaseDLL.encode_uint32(buffer, ref pos_, birthPosY);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref birthPosX);
				BaseDLL.decode_uint32(buffer, ref pos_, ref birthPosY);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, battleID);
				BaseDLL.encode_uint64(buffer, ref pos_, playerID);
				BaseDLL.encode_uint32(buffer, ref pos_, birthPosX);
				BaseDLL.encode_uint32(buffer, ref pos_, birthPosY);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref birthPosX);
				BaseDLL.decode_uint32(buffer, ref pos_, ref birthPosY);
			}

			public int getLen()
			{
				int _len = 0;
				// battleID
				_len += 4;
				// playerID
				_len += 8;
				// birthPosX
				_len += 4;
				// birthPosY
				_len += 4;
				return _len;
			}
		#endregion

	}

}
