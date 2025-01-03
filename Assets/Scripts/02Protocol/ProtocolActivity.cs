using System;
using System.Text;

namespace Protocol
{
	/// <summary>
	///  活动状态
	/// </summary>
	public enum StateType
	{
		/// <summary>
		///  结束
		/// </summary>
		End = 0,
		/// <summary>
		///  进行中
		/// </summary>
		Running = 1,
		/// <summary>
		///  准备中
		/// </summary>
		Ready = 2,
	}

	/// <summary>
	///  通知类型
	/// </summary>
	public enum NotifyType
	{
		NT_NONE = 0,
		/// <summary>
		///  公会战
		/// </summary>
		NT_GUILD_BATTLE = 1,
		/// <summary>
		///  武道大会 		
		/// </summary>
		NT_BUDO = 2,
		/// <summary>
		/// 罐子开放				
		/// </summary>
		NT_JAR_OPEN = 3,
		/// <summary>
		/// 罐子折扣重置			
		/// </summary>
		NT_JAR_SALE_RESET = 4,
		/// <summary>
		/// 时限道具
		/// </summary>
		NT_TIME_ITEM = 5,
		/// <summary>
		/// 赏金联赛
		/// </summary>
		NT_MONEY_REWARDS = 6,
		/// <summary>
		///  魔罐积分清空
		/// </summary>
		NT_MAGIC_INTEGRAL_EMPTYING = 7,
		/// <summary>
		///  公会副本
		/// </summary>
		NT_GUILD_DUNGEON = 8,
		/// <summary>
		///  月卡奖励暂存箱不足24h提示
		/// </summary>
		NT_MONTH_CARD_REWARD_EXPIRE_24H = 9,
		/// <summary>
		/// 佣兵团赏金即将重置
		/// </summary>
		NT_ADVENTURE_TEAM_BOUNTY_EMPTYING = 10,
		/// <summary>
		/// 佣兵团成长药剂即将重置
		/// </summary>
		NT_ADVENTURE_TEAM_INHERIT_BLESS_EMPTYING = 11,
		/// <summary>
		/// 冒险通行证代币重置
		/// </summary>
		NT_ADVENTURE_PASS_CARD_COIN_EMPTYING = 12,
		NT_MAX = 13,
	}

	public class ActivityInfo : Protocol.IProtocolStream
	{
		/// <summary>
		/// 状态，0 结束, 1 进行中，2 准备中
		/// </summary>
		public byte state;
		public UInt32 id;
		/// <summary>
		/// 活动名
		/// </summary>
		public string name;
		/// <summary>
		/// 需要等级
		/// </summary>
		public UInt16 level;
		/// <summary>
		/// 准备时间
		/// </summary>
		public UInt32 preTime;
		/// <summary>
		/// 开始时间
		/// </summary>
		public UInt32 startTime;
		/// <summary>
		/// 到期时间
		/// </summary>
		public UInt32 dueTime;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, state);
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_uint32(buffer, ref pos_, preTime);
				BaseDLL.encode_uint32(buffer, ref pos_, startTime);
				BaseDLL.encode_uint32(buffer, ref pos_, dueTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref state);
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_uint32(buffer, ref pos_, ref preTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dueTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, state);
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_uint32(buffer, ref pos_, preTime);
				BaseDLL.encode_uint32(buffer, ref pos_, startTime);
				BaseDLL.encode_uint32(buffer, ref pos_, dueTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref state);
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_uint32(buffer, ref pos_, ref preTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dueTime);
			}

			public int getLen()
			{
				int _len = 0;
				// state
				_len += 1;
				// id
				_len += 4;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// level
				_len += 2;
				// preTime
				_len += 4;
				// startTime
				_len += 4;
				// dueTime
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class TaskPair : Protocol.IProtocolStream
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

	public class DropItem : Protocol.IProtocolStream
	{
		public UInt32 itemId;
		public UInt32 num;
		public byte strenth;
		/// <summary>
		///  装备类型(对应枚举EquipType)
		/// </summary>
		public byte equipType;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
				BaseDLL.encode_int8(buffer, ref pos_, strenth);
				BaseDLL.encode_int8(buffer, ref pos_, equipType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
				BaseDLL.decode_int8(buffer, ref pos_, ref strenth);
				BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
				BaseDLL.encode_int8(buffer, ref pos_, strenth);
				BaseDLL.encode_int8(buffer, ref pos_, equipType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
				BaseDLL.decode_int8(buffer, ref pos_, ref strenth);
				BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
			}

			public int getLen()
			{
				int _len = 0;
				// itemId
				_len += 4;
				// num
				_len += 4;
				// strenth
				_len += 1;
				// equipType
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  活动怪物信息
	/// </summary>
	public class ActivityMonsterInfo : Protocol.IProtocolStream
	{
		public string name;
		public byte activity;
		public UInt32 id;
		/// <summary>
		///  刷怪点类型(DEntityType)
		/// </summary>
		public byte pointType;
		public UInt32 startTime;
		public UInt32 endTime;
		public UInt32 remainNum;
		public UInt32 nextRollStartTime;
		public DropItem[] drops = new DropItem[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, activity);
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, pointType);
				BaseDLL.encode_uint32(buffer, ref pos_, startTime);
				BaseDLL.encode_uint32(buffer, ref pos_, endTime);
				BaseDLL.encode_uint32(buffer, ref pos_, remainNum);
				BaseDLL.encode_uint32(buffer, ref pos_, nextRollStartTime);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)drops.Length);
				for(int i = 0; i < drops.Length; i++)
				{
					drops[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref activity);
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref pointType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref remainNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref nextRollStartTime);
				UInt16 dropsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref dropsCnt);
				drops = new DropItem[dropsCnt];
				for(int i = 0; i < drops.Length; i++)
				{
					drops[i] = new DropItem();
					drops[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, activity);
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, pointType);
				BaseDLL.encode_uint32(buffer, ref pos_, startTime);
				BaseDLL.encode_uint32(buffer, ref pos_, endTime);
				BaseDLL.encode_uint32(buffer, ref pos_, remainNum);
				BaseDLL.encode_uint32(buffer, ref pos_, nextRollStartTime);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)drops.Length);
				for(int i = 0; i < drops.Length; i++)
				{
					drops[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref activity);
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref pointType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref remainNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref nextRollStartTime);
				UInt16 dropsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref dropsCnt);
				drops = new DropItem[dropsCnt];
				for(int i = 0; i < drops.Length; i++)
				{
					drops[i] = new DropItem();
					drops[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// activity
				_len += 1;
				// id
				_len += 4;
				// pointType
				_len += 1;
				// startTime
				_len += 4;
				// endTime
				_len += 4;
				// remainNum
				_len += 4;
				// nextRollStartTime
				_len += 4;
				// drops
				_len += 2;
				for(int j = 0; j < drops.Length; j++)
				{
					_len += drops[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  活动页签信息
	/// </summary>
	public class ActivityTabInfo : Protocol.IProtocolStream
	{
		public UInt32 id;
		public string name;
		public UInt32[] actIds = new UInt32[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)actIds.Length);
				for(int i = 0; i < actIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, actIds[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				UInt16 actIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref actIdsCnt);
				actIds = new UInt32[actIdsCnt];
				for(int i = 0; i < actIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref actIds[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)actIds.Length);
				for(int i = 0; i < actIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, actIds[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				UInt16 actIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref actIdsCnt);
				actIds = new UInt32[actIdsCnt];
				for(int i = 0; i < actIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref actIds[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// actIds
				_len += 2 + 4 * actIds.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 同步客户端活动状态
	/// </summary>
	[Protocol]
	public class WorldNotifyClientActivity : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602901;
		public UInt32 Sequence;
		public byte type;
		/// <summary>
		/// 0.结束，1.开始，2.准备
		/// </summary>
		public UInt32 id;
		public string name;
		public UInt16 level;
		public UInt32 preTime;
		public UInt32 startTime;
		/// <summary>
		/// 开始时间
		/// </summary>
		public UInt32 dueTime;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_uint32(buffer, ref pos_, preTime);
				BaseDLL.encode_uint32(buffer, ref pos_, startTime);
				BaseDLL.encode_uint32(buffer, ref pos_, dueTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_uint32(buffer, ref pos_, ref preTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dueTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_uint32(buffer, ref pos_, preTime);
				BaseDLL.encode_uint32(buffer, ref pos_, startTime);
				BaseDLL.encode_uint32(buffer, ref pos_, dueTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_uint32(buffer, ref pos_, ref preTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dueTime);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// id
				_len += 4;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// level
				_len += 2;
				// preTime
				_len += 4;
				// startTime
				_len += 4;
				// dueTime
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 截止时间
	/// </summary>
	/// <summary>
	///  server->client 同步活动数据
	/// </summary>
	[Protocol]
	public class SceneSyncClientActivities : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501136;
		public UInt32 Sequence;
		public ActivityInfo[] activities = new ActivityInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)activities.Length);
				for(int i = 0; i < activities.Length; i++)
				{
					activities[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 activitiesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref activitiesCnt);
				activities = new ActivityInfo[activitiesCnt];
				for(int i = 0; i < activities.Length; i++)
				{
					activities[i] = new ActivityInfo();
					activities[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)activities.Length);
				for(int i = 0; i < activities.Length; i++)
				{
					activities[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 activitiesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref activitiesCnt);
				activities = new ActivityInfo[activitiesCnt];
				for(int i = 0; i < activities.Length; i++)
				{
					activities[i] = new ActivityInfo();
					activities[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// activities
				_len += 2;
				for(int j = 0; j < activities.Length; j++)
				{
					_len += activities[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 同步活动任务状态
	/// </summary>
	[Protocol]
	public class SceneNotifyActiveTaskStatus : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501127;
		public UInt32 Sequence;
		public UInt32 taskId;
		public byte status;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_int8(buffer, ref pos_, status);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
				BaseDLL.encode_int8(buffer, ref pos_, status);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
			}

			public int getLen()
			{
				int _len = 0;
				// taskId
				_len += 4;
				// status
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 同步活动任务变量更新
	/// </summary>
	[Protocol]
	public class SceneNotifyActiveTaskVar : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501128;
		public UInt32 Sequence;
		public UInt32 taskId;
		public string key;
		public string val;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				byte[] keyBytes = StringHelper.StringToUTF8Bytes(key);
				BaseDLL.encode_string(buffer, ref pos_, keyBytes, (UInt16)(buffer.Length - pos_));
				byte[] valBytes = StringHelper.StringToUTF8Bytes(val);
				BaseDLL.encode_string(buffer, ref pos_, valBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
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
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
				byte[] keyBytes = StringHelper.StringToUTF8Bytes(key);
				BaseDLL.encode_string(buffer, ref pos_, keyBytes, (UInt16)(buffer.Length - pos_));
				byte[] valBytes = StringHelper.StringToUTF8Bytes(val);
				BaseDLL.encode_string(buffer, ref pos_, valBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
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
				// taskId
				_len += 4;
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
	/// 同步活动任务列表
	/// </summary>
	[Protocol]
	public class SceneSyncActiveTaskList : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501129;
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
	/// 提交活动任务
	/// </summary>
	[Protocol]
	public class SceneActiveTaskSubmit : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501130;
		public UInt32 Sequence;
		public UInt32 taskId;
		public UInt32 param1;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint32(buffer, ref pos_, param1);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param1);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
				BaseDLL.encode_uint32(buffer, ref pos_, param1);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param1);
			}

			public int getLen()
			{
				int _len = 0;
				// taskId
				_len += 4;
				// param1
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 补签
	/// </summary>
	[Protocol]
	public class SceneActiveTaskSubmitRp : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501131;
		public UInt32 Sequence;
		public UInt32[] taskId = new UInt32[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)taskId.Length);
				for(int i = 0; i < taskId.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, taskId[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 taskIdCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref taskIdCnt);
				taskId = new UInt32[taskIdCnt];
				for(int i = 0; i < taskId.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref taskId[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)taskId.Length);
				for(int i = 0; i < taskId.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, taskId[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 taskIdCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref taskIdCnt);
				taskId = new UInt32[taskIdCnt];
				for(int i = 0; i < taskId.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref taskId[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// taskId
				_len += 2 + 4 * taskId.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 新自然月签到
	/// </summary>
	[Protocol]
	public class SceneNewSignIn : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501162;
		public UInt32 Sequence;
		public byte day;
		public byte isAll;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, day);
				BaseDLL.encode_int8(buffer, ref pos_, isAll);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref day);
				BaseDLL.decode_int8(buffer, ref pos_, ref isAll);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, day);
				BaseDLL.encode_int8(buffer, ref pos_, isAll);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref day);
				BaseDLL.decode_int8(buffer, ref pos_, ref isAll);
			}

			public int getLen()
			{
				int _len = 0;
				// day
				_len += 1;
				// isAll
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 新自然月签到/补签结果返回
	/// </summary>
	[Protocol]
	public class SceneNewSignRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501163;
		public UInt32 Sequence;
		public UInt32 errorCode;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
			}

			public int getLen()
			{
				int _len = 0;
				// errorCode
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 查询新月签到情况
	/// </summary>
	[Protocol]
	public class SceneNewSignInQuery : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501164;
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
	/// 查询新月签到情况返回
	/// </summary>
	[Protocol]
	public class SceneNewSignInQueryRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501165;
		public UInt32 Sequence;
		/// <summary>
		/// 签到记录按位存储
		/// </summary>
		public UInt32 signFlag;
		/// <summary>
		/// 已领取累计奖励的累计天数按位存储
		/// </summary>
		public UInt32 collectFlag;
		/// <summary>
		/// 剩余收费补签次数
		/// </summary>
		public byte noFree;
		/// <summary>
		/// 剩余免费补签次数
		/// </summary>
		public byte free;
		/// <summary>
		/// 剩余活跃度补签次数
		/// </summary>
		public byte activite;
		/// <summary>
		/// 活跃度已补签次数
		/// </summary>
		public byte activiteCount;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, signFlag);
				BaseDLL.encode_uint32(buffer, ref pos_, collectFlag);
				BaseDLL.encode_int8(buffer, ref pos_, noFree);
				BaseDLL.encode_int8(buffer, ref pos_, free);
				BaseDLL.encode_int8(buffer, ref pos_, activite);
				BaseDLL.encode_int8(buffer, ref pos_, activiteCount);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref signFlag);
				BaseDLL.decode_uint32(buffer, ref pos_, ref collectFlag);
				BaseDLL.decode_int8(buffer, ref pos_, ref noFree);
				BaseDLL.decode_int8(buffer, ref pos_, ref free);
				BaseDLL.decode_int8(buffer, ref pos_, ref activite);
				BaseDLL.decode_int8(buffer, ref pos_, ref activiteCount);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, signFlag);
				BaseDLL.encode_uint32(buffer, ref pos_, collectFlag);
				BaseDLL.encode_int8(buffer, ref pos_, noFree);
				BaseDLL.encode_int8(buffer, ref pos_, free);
				BaseDLL.encode_int8(buffer, ref pos_, activite);
				BaseDLL.encode_int8(buffer, ref pos_, activiteCount);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref signFlag);
				BaseDLL.decode_uint32(buffer, ref pos_, ref collectFlag);
				BaseDLL.decode_int8(buffer, ref pos_, ref noFree);
				BaseDLL.decode_int8(buffer, ref pos_, ref free);
				BaseDLL.decode_int8(buffer, ref pos_, ref activite);
				BaseDLL.decode_int8(buffer, ref pos_, ref activiteCount);
			}

			public int getLen()
			{
				int _len = 0;
				// signFlag
				_len += 4;
				// collectFlag
				_len += 4;
				// noFree
				_len += 1;
				// free
				_len += 1;
				// activite
				_len += 1;
				// activiteCount
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 领取累计签到奖励
	/// </summary>
	[Protocol]
	public class SceneNewSignInCollect : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501166;
		public UInt32 Sequence;
		public byte day;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, day);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref day);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, day);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref day);
			}

			public int getLen()
			{
				int _len = 0;
				// day
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 领取累计签到奖励返回
	/// </summary>
	[Protocol]
	public class SceneNewSignInCollectRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501167;
		public UInt32 Sequence;
		public UInt32 errorCode;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
			}

			public int getLen()
			{
				int _len = 0;
				// errorCode
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 查询七日活动剩余时间
	/// </summary>
	[Protocol]
	public class SceneActiveRestTimeReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501138;
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
	/// 查询七日活动剩余时间返回
	/// </summary>
	[Protocol]
	public class SceneActiveRestTimeRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501137;
		public UInt32 Sequence;
		public UInt32 time1;
		public UInt32 time2;
		public UInt32 time3;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, time1);
				BaseDLL.encode_uint32(buffer, ref pos_, time2);
				BaseDLL.encode_uint32(buffer, ref pos_, time3);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref time1);
				BaseDLL.decode_uint32(buffer, ref pos_, ref time2);
				BaseDLL.decode_uint32(buffer, ref pos_, ref time3);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, time1);
				BaseDLL.encode_uint32(buffer, ref pos_, time2);
				BaseDLL.encode_uint32(buffer, ref pos_, time3);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref time1);
				BaseDLL.decode_uint32(buffer, ref pos_, ref time2);
				BaseDLL.decode_uint32(buffer, ref pos_, ref time3);
			}

			public int getLen()
			{
				int _len = 0;
				// time1
				_len += 4;
				// time2
				_len += 4;
				// time3
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 更新阶段礼包状态
	/// </summary>
	[Protocol]
	public class SceneSyncPhaseGift : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501141;
		public UInt32 Sequence;
		public UInt32 giftId;
		public byte canTake;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_int8(buffer, ref pos_, canTake);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref giftId);
				BaseDLL.decode_int8(buffer, ref pos_, ref canTake);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, giftId);
				BaseDLL.encode_int8(buffer, ref pos_, canTake);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref giftId);
				BaseDLL.decode_int8(buffer, ref pos_, ref canTake);
			}

			public int getLen()
			{
				int _len = 0;
				// giftId
				_len += 4;
				// canTake
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 1.可领取, 0.未完成
	/// </summary>
	/// <summary>
	/// 领取阶段礼包
	/// </summary>
	[Protocol]
	public class SceneTakePhaseGift : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501142;
		public UInt32 Sequence;
		public UInt32 giftId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref giftId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, giftId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref giftId);
			}

			public int getLen()
			{
				int _len = 0;
				// giftId
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class NotifyInfo : Protocol.IProtocolStream
	{
		public UInt32 type;
		/// <summary>
		/// 通知类型
		/// </summary>
		public UInt64 param;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, param);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref param);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, param);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref param);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 4;
				// param
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 通知参数
	/// </summary>
	/// <summary>
	/// 初始化通知列表
	/// </summary>
	[Protocol]
	public class SceneInitNotifyList : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501153;
		public UInt32 Sequence;
		public NotifyInfo[] notifys = new NotifyInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)notifys.Length);
				for(int i = 0; i < notifys.Length; i++)
				{
					notifys[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 notifysCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref notifysCnt);
				notifys = new NotifyInfo[notifysCnt];
				for(int i = 0; i < notifys.Length; i++)
				{
					notifys[i] = new NotifyInfo();
					notifys[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)notifys.Length);
				for(int i = 0; i < notifys.Length; i++)
				{
					notifys[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 notifysCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref notifysCnt);
				notifys = new NotifyInfo[notifysCnt];
				for(int i = 0; i < notifys.Length; i++)
				{
					notifys[i] = new NotifyInfo();
					notifys[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// notifys
				_len += 2;
				for(int j = 0; j < notifys.Length; j++)
				{
					_len += notifys[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 更新通知列表
	/// </summary>
	[Protocol]
	public class SceneUpdateNotifyList : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501154;
		public UInt32 Sequence;
		public NotifyInfo notify = new NotifyInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				notify.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				notify.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				notify.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				notify.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// notify
				_len += notify.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 请求删除通知
	/// </summary>
	[Protocol]
	public class SceneDeleteNotifyList : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501155;
		public UInt32 Sequence;
		public NotifyInfo notify = new NotifyInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				notify.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				notify.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				notify.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				notify.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// notify
				_len += notify.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求活动怪物信息
	/// </summary>
	[Protocol]
	public class WorldActivityMonsterReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607404;
		public UInt32 Sequence;
		public UInt32 activityId;
		public UInt32[] ids = new UInt32[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, activityId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)ids.Length);
				for(int i = 0; i < ids.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, ids[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref activityId);
				UInt16 idsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref idsCnt);
				ids = new UInt32[idsCnt];
				for(int i = 0; i < ids.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref ids[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, activityId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)ids.Length);
				for(int i = 0; i < ids.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, ids[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref activityId);
				UInt16 idsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref idsCnt);
				ids = new UInt32[idsCnt];
				for(int i = 0; i < ids.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref ids[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// activityId
				_len += 4;
				// ids
				_len += 2 + 4 * ids.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  返回活动怪物信息
	/// </summary>
	[Protocol]
	public class WorldActivityMonsterRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607405;
		public UInt32 Sequence;
		public UInt32 activityId;
		public ActivityMonsterInfo[] monsters = new ActivityMonsterInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, activityId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)monsters.Length);
				for(int i = 0; i < monsters.Length; i++)
				{
					monsters[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref activityId);
				UInt16 monstersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref monstersCnt);
				monsters = new ActivityMonsterInfo[monstersCnt];
				for(int i = 0; i < monsters.Length; i++)
				{
					monsters[i] = new ActivityMonsterInfo();
					monsters[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, activityId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)monsters.Length);
				for(int i = 0; i < monsters.Length; i++)
				{
					monsters[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref activityId);
				UInt16 monstersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref monstersCnt);
				monsters = new ActivityMonsterInfo[monstersCnt];
				for(int i = 0; i < monsters.Length; i++)
				{
					monsters[i] = new ActivityMonsterInfo();
					monsters[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// activityId
				_len += 4;
				// monsters
				_len += 2;
				for(int j = 0; j < monsters.Length; j++)
				{
					_len += monsters[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  活动页签信息同步
	/// </summary>
	[Protocol]
	public class WorldActivityTabsInfoSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607407;
		public UInt32 Sequence;
		public ActivityTabInfo[] tabsInfo = new ActivityTabInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)tabsInfo.Length);
				for(int i = 0; i < tabsInfo.Length; i++)
				{
					tabsInfo[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 tabsInfoCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref tabsInfoCnt);
				tabsInfo = new ActivityTabInfo[tabsInfoCnt];
				for(int i = 0; i < tabsInfo.Length; i++)
				{
					tabsInfo[i] = new ActivityTabInfo();
					tabsInfo[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)tabsInfo.Length);
				for(int i = 0; i < tabsInfo.Length; i++)
				{
					tabsInfo[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 tabsInfoCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref tabsInfoCnt);
				tabsInfo = new ActivityTabInfo[tabsInfoCnt];
				for(int i = 0; i < tabsInfo.Length; i++)
				{
					tabsInfo[i] = new ActivityTabInfo();
					tabsInfo[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// tabsInfo
				_len += 2;
				for(int j = 0; j < tabsInfo.Length; j++)
				{
					_len += tabsInfo[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 挑战者积分查询
	/// </summary>
	[Protocol]
	public class SceneChallengeScoreReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507414;
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
	/// 挑战者积分返回
	/// </summary>
	[Protocol]
	public class SceneChallengeScoreRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507415;
		public UInt32 Sequence;
		public UInt32 score;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, score);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref score);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, score);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref score);
			}

			public int getLen()
			{
				int _len = 0;
				// score
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  全民抢购最终折扣通知
	/// </summary>
	[Protocol]
	public class GASWholeBargainDiscountSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 707406;
		public UInt32 Sequence;
		/// <summary>
		///  折扣百分比
		/// </summary>
		public UInt32 discount;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, discount);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref discount);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, discount);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref discount);
			}

			public int getLen()
			{
				int _len = 0;
				// discount
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 全民抢购数据请求
	/// </summary>
	[Protocol]
	public class GASWholeBargainReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 707407;
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
	/// 全民抢购数据返回
	/// </summary>
	[Protocol]
	public class GASWholeBargainRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 707408;
		public UInt32 Sequence;
		/// <summary>
		///  玩家参与次数
		/// </summary>
		public UInt32 playerJoinNum;
		/// <summary>
		///  参与次数
		/// </summary>
		public UInt32 joinNum;
		/// <summary>
		///  最大次数
		/// </summary>
		public UInt32 maxNum;
		/// <summary>
		///  折扣
		/// </summary>
		public UInt32 discount;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, playerJoinNum);
				BaseDLL.encode_uint32(buffer, ref pos_, joinNum);
				BaseDLL.encode_uint32(buffer, ref pos_, maxNum);
				BaseDLL.encode_uint32(buffer, ref pos_, discount);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerJoinNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref joinNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref maxNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref discount);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, playerJoinNum);
				BaseDLL.encode_uint32(buffer, ref pos_, joinNum);
				BaseDLL.encode_uint32(buffer, ref pos_, maxNum);
				BaseDLL.encode_uint32(buffer, ref pos_, discount);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerJoinNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref joinNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref maxNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref discount);
			}

			public int getLen()
			{
				int _len = 0;
				// playerJoinNum
				_len += 4;
				// joinNum
				_len += 4;
				// maxNum
				_len += 4;
				// discount
				_len += 4;
				return _len;
			}
		#endregion

	}

}
