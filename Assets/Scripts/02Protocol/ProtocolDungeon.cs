using System;
using System.Text;

namespace Protocol
{
	public enum DungeonScore
	{
		C = 0,
		B = 1,
		A = 2,
		S = 3,
		SS = 4,
		SSS = 5,
	}

	/// <summary>
	///  深渊模式
	/// </summary>
	public enum DungeonHellMode
	{
		/// <summary>
		///  无
		/// </summary>
		Null = 0,
		/// <summary>
		///  普通
		/// </summary>
		Normal = 1,
		/// <summary>
		///  困难
		/// </summary>
		Hard = 2,
	}

	/// <summary>
	///  疲劳燃烧类型
	/// </summary>
	public enum FatigueBurnType
	{
		/// <summary>
		///  无
		/// </summary>
		FBT_NONE = 0,
		/// <summary>
		///  普通
		/// </summary>
		FBT_COMMON = 1,
		/// <summary>
		///  高级
		/// </summary>
		FBT_ADVANCED = 2,
	}

	/// <summary>
	///  各种经验加成类型
	/// </summary>
	public enum DungeonAdditionType
	{
		/// <summary>
		///  经验药水
		/// </summary>
		EXP_BUFF = 0,
		/// <summary>
		///  VIP经验加成
		/// </summary>
		EXP_VIP = 1,
		/// <summary>
		///  评价经验加成
		/// </summary>
		EXP_SCORE = 2,
		/// <summary>
		///  难度经验加成
		/// </summary>
		EXP_HARD = 3,
		/// <summary>
		///  公会技能经验加成
		/// </summary>
		EXP_GUILD_SKILL = 4,
		/// <summary>
		///  VIP金币加成
		/// </summary>
		GOLD_VIP = 5,
		/// <summary>
		///  TAP经验加成
		/// </summary>
		EXP_TAP = 6,
		/// <summary>
		///  好友经验加成
		/// </summary>
		EXP_FRIEND = 7,
		/// <summary>
		///  冒险队经验加成
		/// </summary>
		DUNGEON_EXP_ADD_ADVENTURE_TEAM = 8,
		/// <summary>
		///  基础经验
		/// </summary>
		DUNGEON_EXP_BASE = 9,
	}

	public enum RollOpTypeEnum
	{
		/// <summary>
		///  无效
		/// </summary>
		RIE_INVALID = 0,
		/// <summary>
		///  需要
		/// </summary>
		RIE_NEED = 1,
		/// <summary>
		///  谦让
		/// </summary>
		RIE_MODEST = 2,
	}

	public enum DungeonChestType
	{
		/// <summary>
		///  普通宝箱
		/// </summary>
		Normal = 0,
		/// <summary>
		///  Vip宝箱
		/// </summary>
		Vip = 1,
		/// <summary>
		///  付费宝箱
		/// </summary>
		Pay = 2,
	}

	public class SceneDungeonInfo : Protocol.IProtocolStream
	{
		public UInt32 id;
		public byte bestScore;
		public UInt32 bestRecordTime;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, bestScore);
				BaseDLL.encode_uint32(buffer, ref pos_, bestRecordTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref bestScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref bestRecordTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, bestScore);
				BaseDLL.encode_uint32(buffer, ref pos_, bestRecordTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref bestScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref bestRecordTime);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// bestScore
				_len += 1;
				// bestRecordTime
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class DungeonAdditionInfo : Protocol.IProtocolStream
	{
		public UInt32[] addition = new UInt32[10];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				for(int i = 0; i < addition.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, addition[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				for(int i = 0; i < addition.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref addition[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				for(int i = 0; i < addition.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, addition[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				for(int i = 0; i < addition.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref addition[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// addition
				_len += 4 * addition.Length;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneDungeonInit : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506801;
		public UInt32 Sequence;
		public SceneDungeonInfo[] allInfo = new SceneDungeonInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)allInfo.Length);
				for(int i = 0; i < allInfo.Length; i++)
				{
					allInfo[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 allInfoCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref allInfoCnt);
				allInfo = new SceneDungeonInfo[allInfoCnt];
				for(int i = 0; i < allInfo.Length; i++)
				{
					allInfo[i] = new SceneDungeonInfo();
					allInfo[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)allInfo.Length);
				for(int i = 0; i < allInfo.Length; i++)
				{
					allInfo[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 allInfoCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref allInfoCnt);
				allInfo = new SceneDungeonInfo[allInfoCnt];
				for(int i = 0; i < allInfo.Length; i++)
				{
					allInfo[i] = new SceneDungeonInfo();
					allInfo[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// allInfo
				_len += 2;
				for(int j = 0; j < allInfo.Length; j++)
				{
					_len += allInfo[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneDungeonUpdate : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506802;
		public UInt32 Sequence;
		public SceneDungeonInfo info = new SceneDungeonInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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

	public class SceneDungeonHardInfo : Protocol.IProtocolStream
	{
		public UInt32 id;
		public byte unlockedHardType;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, unlockedHardType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref unlockedHardType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, unlockedHardType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref unlockedHardType);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// unlockedHardType
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  地下城静态数值
	/// </summary>
	public class DungeonStaticValue : Protocol.IProtocolStream
	{
		public Int32[] values = new Int32[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)values.Length);
				for(int i = 0; i < values.Length; i++)
				{
					BaseDLL.encode_int32(buffer, ref pos_, values[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 valuesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref valuesCnt);
				values = new Int32[valuesCnt];
				for(int i = 0; i < values.Length; i++)
				{
					BaseDLL.decode_int32(buffer, ref pos_, ref values[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)values.Length);
				for(int i = 0; i < values.Length; i++)
				{
					BaseDLL.encode_int32(buffer, ref pos_, values[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 valuesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref valuesCnt);
				values = new Int32[valuesCnt];
				for(int i = 0; i < values.Length; i++)
				{
					BaseDLL.decode_int32(buffer, ref pos_, ref values[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// values
				_len += 2 + 4 * values.Length;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneDungeonHardInit : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506803;
		public UInt32 Sequence;
		public SceneDungeonHardInfo[] allInfo = new SceneDungeonHardInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)allInfo.Length);
				for(int i = 0; i < allInfo.Length; i++)
				{
					allInfo[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 allInfoCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref allInfoCnt);
				allInfo = new SceneDungeonHardInfo[allInfoCnt];
				for(int i = 0; i < allInfo.Length; i++)
				{
					allInfo[i] = new SceneDungeonHardInfo();
					allInfo[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)allInfo.Length);
				for(int i = 0; i < allInfo.Length; i++)
				{
					allInfo[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 allInfoCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref allInfoCnt);
				allInfo = new SceneDungeonHardInfo[allInfoCnt];
				for(int i = 0; i < allInfo.Length; i++)
				{
					allInfo[i] = new SceneDungeonHardInfo();
					allInfo[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// allInfo
				_len += 2;
				for(int j = 0; j < allInfo.Length; j++)
				{
					_len += allInfo[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneDungeonHardUpdate : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506804;
		public UInt32 Sequence;
		public SceneDungeonHardInfo info = new SceneDungeonHardInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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

	[Protocol]
	public class SceneDungeonStartReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506805;
		public UInt32 Sequence;
		public UInt32 dungeonId;
		/// <summary>
		///  要使用的药水
		/// </summary>
		public UInt32[] buffDrugs = new UInt32[0];
		/// <summary>
		///  是否重新开始
		/// </summary>
		public byte isRestart;
		/// <summary>
		///  如果是城镇怪物、填入城镇怪物ID
		/// </summary>
		public UInt64 cityMonsterId;
		/// <summary>
		///  如果是未央地图、填入司南ID
		/// </summary>
		public UInt64 sinanId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)buffDrugs.Length);
				for(int i = 0; i < buffDrugs.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, buffDrugs[i]);
				}
				BaseDLL.encode_int8(buffer, ref pos_, isRestart);
				BaseDLL.encode_uint64(buffer, ref pos_, cityMonsterId);
				BaseDLL.encode_uint64(buffer, ref pos_, sinanId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
				UInt16 buffDrugsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref buffDrugsCnt);
				buffDrugs = new UInt32[buffDrugsCnt];
				for(int i = 0; i < buffDrugs.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref buffDrugs[i]);
				}
				BaseDLL.decode_int8(buffer, ref pos_, ref isRestart);
				BaseDLL.decode_uint64(buffer, ref pos_, ref cityMonsterId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref sinanId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)buffDrugs.Length);
				for(int i = 0; i < buffDrugs.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, buffDrugs[i]);
				}
				BaseDLL.encode_int8(buffer, ref pos_, isRestart);
				BaseDLL.encode_uint64(buffer, ref pos_, cityMonsterId);
				BaseDLL.encode_uint64(buffer, ref pos_, sinanId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
				UInt16 buffDrugsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref buffDrugsCnt);
				buffDrugs = new UInt32[buffDrugsCnt];
				for(int i = 0; i < buffDrugs.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref buffDrugs[i]);
				}
				BaseDLL.decode_int8(buffer, ref pos_, ref isRestart);
				BaseDLL.decode_uint64(buffer, ref pos_, ref cityMonsterId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref sinanId);
			}

			public int getLen()
			{
				int _len = 0;
				// dungeonId
				_len += 4;
				// buffDrugs
				_len += 2 + 4 * buffDrugs.Length;
				// isRestart
				_len += 1;
				// cityMonsterId
				_len += 8;
				// sinanId
				_len += 8;
				return _len;
			}
		#endregion

	}

	public class SceneDungeonDropItem : Protocol.IProtocolStream
	{
		public UInt32 id;
		public UInt32 typeId;
		public UInt32 num;
		public byte isDouble;
		public byte strenth;
		/// <summary>
		///  装备类型，对应枚举EquipType
		/// </summary>
		public byte equipType;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, typeId);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
				BaseDLL.encode_int8(buffer, ref pos_, isDouble);
				BaseDLL.encode_int8(buffer, ref pos_, strenth);
				BaseDLL.encode_int8(buffer, ref pos_, equipType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref typeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
				BaseDLL.decode_int8(buffer, ref pos_, ref isDouble);
				BaseDLL.decode_int8(buffer, ref pos_, ref strenth);
				BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, typeId);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
				BaseDLL.encode_int8(buffer, ref pos_, isDouble);
				BaseDLL.encode_int8(buffer, ref pos_, strenth);
				BaseDLL.encode_int8(buffer, ref pos_, equipType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref typeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
				BaseDLL.decode_int8(buffer, ref pos_, ref isDouble);
				BaseDLL.decode_int8(buffer, ref pos_, ref strenth);
				BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// typeId
				_len += 4;
				// num
				_len += 4;
				// isDouble
				_len += 1;
				// strenth
				_len += 1;
				// equipType
				_len += 1;
				return _len;
			}
		#endregion

	}

	public class SceneDungeonMonster : Protocol.IProtocolStream
	{
		public UInt32 id;
		public UInt32 pointId;
		public UInt32 typeId;
		public SceneDungeonDropItem[] dropItems = new SceneDungeonDropItem[0];
		public byte[] prefixes = new byte[0];
		public UInt32 summonerId;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, pointId);
				BaseDLL.encode_uint32(buffer, ref pos_, typeId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)dropItems.Length);
				for(int i = 0; i < dropItems.Length; i++)
				{
					dropItems[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)prefixes.Length);
				for(int i = 0; i < prefixes.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, prefixes[i]);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, summonerId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref pointId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref typeId);
				UInt16 dropItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref dropItemsCnt);
				dropItems = new SceneDungeonDropItem[dropItemsCnt];
				for(int i = 0; i < dropItems.Length; i++)
				{
					dropItems[i] = new SceneDungeonDropItem();
					dropItems[i].decode(buffer, ref pos_);
				}
				UInt16 prefixesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref prefixesCnt);
				prefixes = new byte[prefixesCnt];
				for(int i = 0; i < prefixes.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref prefixes[i]);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref summonerId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, pointId);
				BaseDLL.encode_uint32(buffer, ref pos_, typeId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)dropItems.Length);
				for(int i = 0; i < dropItems.Length; i++)
				{
					dropItems[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)prefixes.Length);
				for(int i = 0; i < prefixes.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, prefixes[i]);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, summonerId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref pointId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref typeId);
				UInt16 dropItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref dropItemsCnt);
				dropItems = new SceneDungeonDropItem[dropItemsCnt];
				for(int i = 0; i < dropItems.Length; i++)
				{
					dropItems[i] = new SceneDungeonDropItem();
					dropItems[i].decode(buffer, ref pos_);
				}
				UInt16 prefixesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref prefixesCnt);
				prefixes = new byte[prefixesCnt];
				for(int i = 0; i < prefixes.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref prefixes[i]);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref summonerId);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// pointId
				_len += 4;
				// typeId
				_len += 4;
				// dropItems
				_len += 2;
				for(int j = 0; j < dropItems.Length; j++)
				{
					_len += dropItems[j].getLen();
				}
				// prefixes
				_len += 2 + 1 * prefixes.Length;
				// summonerId
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class SceneDungeonArea : Protocol.IProtocolStream
	{
		public UInt32 id;
		public SceneDungeonMonster[] monsters = new SceneDungeonMonster[0];
		public SceneDungeonMonster[] destructs = new SceneDungeonMonster[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)monsters.Length);
				for(int i = 0; i < monsters.Length; i++)
				{
					monsters[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)destructs.Length);
				for(int i = 0; i < destructs.Length; i++)
				{
					destructs[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				UInt16 monstersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref monstersCnt);
				monsters = new SceneDungeonMonster[monstersCnt];
				for(int i = 0; i < monsters.Length; i++)
				{
					monsters[i] = new SceneDungeonMonster();
					monsters[i].decode(buffer, ref pos_);
				}
				UInt16 destructsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref destructsCnt);
				destructs = new SceneDungeonMonster[destructsCnt];
				for(int i = 0; i < destructs.Length; i++)
				{
					destructs[i] = new SceneDungeonMonster();
					destructs[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)monsters.Length);
				for(int i = 0; i < monsters.Length; i++)
				{
					monsters[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)destructs.Length);
				for(int i = 0; i < destructs.Length; i++)
				{
					destructs[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				UInt16 monstersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref monstersCnt);
				monsters = new SceneDungeonMonster[monstersCnt];
				for(int i = 0; i < monsters.Length; i++)
				{
					monsters[i] = new SceneDungeonMonster();
					monsters[i].decode(buffer, ref pos_);
				}
				UInt16 destructsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref destructsCnt);
				destructs = new SceneDungeonMonster[destructsCnt];
				for(int i = 0; i < destructs.Length; i++)
				{
					destructs[i] = new SceneDungeonMonster();
					destructs[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// monsters
				_len += 2;
				for(int j = 0; j < monsters.Length; j++)
				{
					_len += monsters[j].getLen();
				}
				// destructs
				_len += 2;
				for(int j = 0; j < destructs.Length; j++)
				{
					_len += destructs[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  深渊波次信息
	/// </summary>
	public class DungeonHellWaveInfo : Protocol.IProtocolStream
	{
		public byte wave;
		public SceneDungeonMonster[] monsters = new SceneDungeonMonster[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, wave);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)monsters.Length);
				for(int i = 0; i < monsters.Length; i++)
				{
					monsters[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref wave);
				UInt16 monstersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref monstersCnt);
				monsters = new SceneDungeonMonster[monstersCnt];
				for(int i = 0; i < monsters.Length; i++)
				{
					monsters[i] = new SceneDungeonMonster();
					monsters[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, wave);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)monsters.Length);
				for(int i = 0; i < monsters.Length; i++)
				{
					monsters[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref wave);
				UInt16 monstersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref monstersCnt);
				monsters = new SceneDungeonMonster[monstersCnt];
				for(int i = 0; i < monsters.Length; i++)
				{
					monsters[i] = new SceneDungeonMonster();
					monsters[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// wave
				_len += 1;
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
	///  深渊信息
	/// </summary>
	public class DungeonHellInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  模式，对应枚举（DungeonHellMode）
		/// </summary>
		public byte mode;
		/// <summary>
		///  所在区域
		/// </summary>
		public UInt32 areaId;
		/// <summary>
		///  波次信息
		/// </summary>
		public DungeonHellWaveInfo[] waveInfoes = new DungeonHellWaveInfo[0];
		/// <summary>
		///  掉落
		/// </summary>
		public SceneDungeonDropItem[] dropItems = new SceneDungeonDropItem[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, mode);
				BaseDLL.encode_uint32(buffer, ref pos_, areaId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)waveInfoes.Length);
				for(int i = 0; i < waveInfoes.Length; i++)
				{
					waveInfoes[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)dropItems.Length);
				for(int i = 0; i < dropItems.Length; i++)
				{
					dropItems[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref mode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref areaId);
				UInt16 waveInfoesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref waveInfoesCnt);
				waveInfoes = new DungeonHellWaveInfo[waveInfoesCnt];
				for(int i = 0; i < waveInfoes.Length; i++)
				{
					waveInfoes[i] = new DungeonHellWaveInfo();
					waveInfoes[i].decode(buffer, ref pos_);
				}
				UInt16 dropItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref dropItemsCnt);
				dropItems = new SceneDungeonDropItem[dropItemsCnt];
				for(int i = 0; i < dropItems.Length; i++)
				{
					dropItems[i] = new SceneDungeonDropItem();
					dropItems[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, mode);
				BaseDLL.encode_uint32(buffer, ref pos_, areaId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)waveInfoes.Length);
				for(int i = 0; i < waveInfoes.Length; i++)
				{
					waveInfoes[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)dropItems.Length);
				for(int i = 0; i < dropItems.Length; i++)
				{
					dropItems[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref mode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref areaId);
				UInt16 waveInfoesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref waveInfoesCnt);
				waveInfoes = new DungeonHellWaveInfo[waveInfoesCnt];
				for(int i = 0; i < waveInfoes.Length; i++)
				{
					waveInfoes[i] = new DungeonHellWaveInfo();
					waveInfoes[i].decode(buffer, ref pos_);
				}
				UInt16 dropItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref dropItemsCnt);
				dropItems = new SceneDungeonDropItem[dropItemsCnt];
				for(int i = 0; i < dropItems.Length; i++)
				{
					dropItems[i] = new SceneDungeonDropItem();
					dropItems[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// mode
				_len += 1;
				// areaId
				_len += 4;
				// waveInfoes
				_len += 2;
				for(int j = 0; j < waveInfoes.Length; j++)
				{
					_len += waveInfoes[j].getLen();
				}
				// dropItems
				_len += 2;
				for(int j = 0; j < dropItems.Length; j++)
				{
					_len += dropItems[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	public class DungeonBuff : Protocol.IProtocolStream
	{
		/// <summary>
		///  buffid
		/// </summary>
		public UInt32 buffId;
		/// <summary>
		///  buff等级
		/// </summary>
		public UInt32 buffLvl;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, buffId);
				BaseDLL.encode_uint32(buffer, ref pos_, buffLvl);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref buffId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref buffLvl);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, buffId);
				BaseDLL.encode_uint32(buffer, ref pos_, buffLvl);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref buffId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref buffLvl);
			}

			public int getLen()
			{
				int _len = 0;
				// buffId
				_len += 4;
				// buffLvl
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class GuildDungeonInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  boss剩余血量
		/// </summary>
		public UInt64 bossOddBlood;
		/// <summary>
		///  boss总血量
		/// </summary>
		public UInt64 bossTotalBlood;
		/// <summary>
		///  加成buff
		/// </summary>
		public DungeonBuff[] buffVec = new DungeonBuff[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, bossOddBlood);
				BaseDLL.encode_uint64(buffer, ref pos_, bossTotalBlood);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)buffVec.Length);
				for(int i = 0; i < buffVec.Length; i++)
				{
					buffVec[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref bossOddBlood);
				BaseDLL.decode_uint64(buffer, ref pos_, ref bossTotalBlood);
				UInt16 buffVecCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref buffVecCnt);
				buffVec = new DungeonBuff[buffVecCnt];
				for(int i = 0; i < buffVec.Length; i++)
				{
					buffVec[i] = new DungeonBuff();
					buffVec[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, bossOddBlood);
				BaseDLL.encode_uint64(buffer, ref pos_, bossTotalBlood);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)buffVec.Length);
				for(int i = 0; i < buffVec.Length; i++)
				{
					buffVec[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref bossOddBlood);
				BaseDLL.decode_uint64(buffer, ref pos_, ref bossTotalBlood);
				UInt16 buffVecCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref buffVecCnt);
				buffVec = new DungeonBuff[buffVecCnt];
				for(int i = 0; i < buffVec.Length; i++)
				{
					buffVec[i] = new DungeonBuff();
					buffVec[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// bossOddBlood
				_len += 8;
				// bossTotalBlood
				_len += 8;
				// buffVec
				_len += 2;
				for(int j = 0; j < buffVec.Length; j++)
				{
					_len += buffVec[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	public class ZjslDungeonBuff : Protocol.IProtocolStream
	{
		/// <summary>
		///  buffid
		/// </summary>
		public UInt32 buffId;
		/// <summary>
		///  buff等级
		/// </summary>
		public UInt32 buffLvl;
		/// <summary>
		///  buff对象，1：玩家；2：怪物
		/// </summary>
		public UInt32 buffTarget;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, buffId);
				BaseDLL.encode_uint32(buffer, ref pos_, buffLvl);
				BaseDLL.encode_uint32(buffer, ref pos_, buffTarget);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref buffId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref buffLvl);
				BaseDLL.decode_uint32(buffer, ref pos_, ref buffTarget);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, buffId);
				BaseDLL.encode_uint32(buffer, ref pos_, buffLvl);
				BaseDLL.encode_uint32(buffer, ref pos_, buffTarget);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref buffId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref buffLvl);
				BaseDLL.decode_uint32(buffer, ref pos_, ref buffTarget);
			}

			public int getLen()
			{
				int _len = 0;
				// buffId
				_len += 4;
				// buffLvl
				_len += 4;
				// buffTarget
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class ZjslDungeonInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  BOSS1的ID
		/// </summary>
		public UInt32 boss1ID;
		/// <summary>
		///  BOSS2的ID
		/// </summary>
		public UInt32 boss2ID;
		/// <summary>
		///  BOSS1的剩余血量
		/// </summary>
		public UInt32 boss1RemainHp;
		/// <summary>
		///  BOSS2的剩
		/// </summary>
		public UInt32 boss2RemainHp;
		/// <summary>
		///  加成buff
		/// </summary>
		public ZjslDungeonBuff[] buffVec = new ZjslDungeonBuff[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, boss1ID);
				BaseDLL.encode_uint32(buffer, ref pos_, boss2ID);
				BaseDLL.encode_uint32(buffer, ref pos_, boss1RemainHp);
				BaseDLL.encode_uint32(buffer, ref pos_, boss2RemainHp);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)buffVec.Length);
				for(int i = 0; i < buffVec.Length; i++)
				{
					buffVec[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref boss1ID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref boss2ID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref boss1RemainHp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref boss2RemainHp);
				UInt16 buffVecCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref buffVecCnt);
				buffVec = new ZjslDungeonBuff[buffVecCnt];
				for(int i = 0; i < buffVec.Length; i++)
				{
					buffVec[i] = new ZjslDungeonBuff();
					buffVec[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, boss1ID);
				BaseDLL.encode_uint32(buffer, ref pos_, boss2ID);
				BaseDLL.encode_uint32(buffer, ref pos_, boss1RemainHp);
				BaseDLL.encode_uint32(buffer, ref pos_, boss2RemainHp);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)buffVec.Length);
				for(int i = 0; i < buffVec.Length; i++)
				{
					buffVec[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref boss1ID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref boss2ID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref boss1RemainHp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref boss2RemainHp);
				UInt16 buffVecCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref buffVecCnt);
				buffVec = new ZjslDungeonBuff[buffVecCnt];
				for(int i = 0; i < buffVec.Length; i++)
				{
					buffVec[i] = new ZjslDungeonBuff();
					buffVec[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// boss1ID
				_len += 4;
				// boss2ID
				_len += 4;
				// boss1RemainHp
				_len += 4;
				// boss2RemainHp
				_len += 4;
				// buffVec
				_len += 2;
				for(int j = 0; j < buffVec.Length; j++)
				{
					_len += buffVec[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	public class RollItemInfo : Protocol.IProtocolStream
	{
		/// <summary>
		/// roll物品索引
		/// </summary>
		public byte rollIndex;
		/// <summary>
		/// 掉落物品
		/// </summary>
		public DropItem dropItem = new DropItem();

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, rollIndex);
				dropItem.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref rollIndex);
				dropItem.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, rollIndex);
				dropItem.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref rollIndex);
				dropItem.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// rollIndex
				_len += 1;
				// dropItem
				_len += dropItem.getLen();
				return _len;
			}
		#endregion

	}

	public class RollDropResultItem : Protocol.IProtocolStream
	{
		/// <summary>
		/// roll物品索引
		/// </summary>
		public byte rollIndex;
		/// <summary>
		/// 请求类型
		/// </summary>
		public byte opType;
		/// <summary>
		/// roll点
		/// </summary>
		public UInt32 point;
		/// <summary>
		/// 玩家id
		/// </summary>
		public UInt64 playerId;
		/// <summary>
		/// 玩家名字
		/// </summary>
		public string playerName;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, rollIndex);
				BaseDLL.encode_int8(buffer, ref pos_, opType);
				BaseDLL.encode_uint32(buffer, ref pos_, point);
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
				BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref rollIndex);
				BaseDLL.decode_int8(buffer, ref pos_, ref opType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref point);
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
				UInt16 playerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerNameLen);
				byte[] playerNameBytes = new byte[playerNameLen];
				for(int i = 0; i < playerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref playerNameBytes[i]);
				}
				playerName = StringHelper.BytesToString(playerNameBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, rollIndex);
				BaseDLL.encode_int8(buffer, ref pos_, opType);
				BaseDLL.encode_uint32(buffer, ref pos_, point);
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
				BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref rollIndex);
				BaseDLL.decode_int8(buffer, ref pos_, ref opType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref point);
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
				UInt16 playerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerNameLen);
				byte[] playerNameBytes = new byte[playerNameLen];
				for(int i = 0; i < playerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref playerNameBytes[i]);
				}
				playerName = StringHelper.BytesToString(playerNameBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// rollIndex
				_len += 1;
				// opType
				_len += 1;
				// point
				_len += 4;
				// playerId
				_len += 8;
				// playerName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(playerName);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneDungeonStartRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506806;
		public UInt32 Sequence;
		/// <summary>
		///  是否接着上一次保存的状态
		/// </summary>
		public byte isCointnue;
		public UInt32 hp;
		public UInt32 result;
		public UInt32 key1;
		public SceneDungeonArea[] areas = new SceneDungeonArea[0];
		/// <summary>
		///  深渊信息
		/// </summary>
		public DungeonHellInfo hellInfo = new DungeonHellInfo();
		public UInt32 mp;
		public UInt32 key2;
		/// <summary>
		/// 登录RelayServer的session
		/// </summary>
		public UInt64 session;
		/// <summary>
		///  RelayServer地址
		/// </summary>
		public SockAddr addr = new SockAddr();
		/// <summary>
		///  所有玩家信息
		/// </summary>
		public RacePlayerInfo[] players = new RacePlayerInfo[0];
		public UInt32 key3;
		/// <summary>
		///  是否开放自动战斗
		/// </summary>
		public byte openAutoBattle;
		public UInt32 dungeonId;
		public UInt32 startAreaId;
		public UInt32 key4;
		/// <summary>
		///  boss掉落
		/// </summary>
		public SceneDungeonDropItem[] bossDropItems = new SceneDungeonDropItem[0];
		/// <summary>
		///  是否记录日志
		/// </summary>
		public byte isRecordLog;
		/// <summary>
		///  是否记录录像
		/// </summary>
		public byte isRecordReplay;
		/// <summary>
		///  掉落次数用完的怪
		/// </summary>
		public UInt32[] dropOverMonster = new UInt32[0];
		/// <summary>
		///  公会地下城信息
		/// </summary>
		public GuildDungeonInfo guildDungeonInfo = new GuildDungeonInfo();
		/// <summary>
		///  深渊是否自动结束(打完柱子就退出)
		/// </summary>
		public byte hellAutoClose;
		/// <summary>
		///  战斗标记(位掩码)
		/// </summary>
		public UInt32 battleFlag;
		/// <summary>
		///  终极试炼信息(只在终极试炼地下城使用)
		/// </summary>
		public ZjslDungeonInfo zjslDungeonInfo = new ZjslDungeonInfo();
		/// <summary>
		///  已通关地下城id
		/// </summary>
		public UInt32[] clearedDungeonIds = new UInt32[0];
		/// <summary>
		///  md5
		/// </summary>
		public byte[] md5 = new byte[16];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, isCointnue);
				BaseDLL.encode_uint32(buffer, ref pos_, hp);
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, key1);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)areas.Length);
				for(int i = 0; i < areas.Length; i++)
				{
					areas[i].encode(buffer, ref pos_);
				}
				hellInfo.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, mp);
				BaseDLL.encode_uint32(buffer, ref pos_, key2);
				BaseDLL.encode_uint64(buffer, ref pos_, session);
				addr.encode(buffer, ref pos_);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)players.Length);
				for(int i = 0; i < players.Length; i++)
				{
					players[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, key3);
				BaseDLL.encode_int8(buffer, ref pos_, openAutoBattle);
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
				BaseDLL.encode_uint32(buffer, ref pos_, startAreaId);
				BaseDLL.encode_uint32(buffer, ref pos_, key4);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)bossDropItems.Length);
				for(int i = 0; i < bossDropItems.Length; i++)
				{
					bossDropItems[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_int8(buffer, ref pos_, isRecordLog);
				BaseDLL.encode_int8(buffer, ref pos_, isRecordReplay);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)dropOverMonster.Length);
				for(int i = 0; i < dropOverMonster.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, dropOverMonster[i]);
				}
				guildDungeonInfo.encode(buffer, ref pos_);
				BaseDLL.encode_int8(buffer, ref pos_, hellAutoClose);
				BaseDLL.encode_uint32(buffer, ref pos_, battleFlag);
				zjslDungeonInfo.encode(buffer, ref pos_);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)clearedDungeonIds.Length);
				for(int i = 0; i < clearedDungeonIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, clearedDungeonIds[i]);
				}
				for(int i = 0; i < md5.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, md5[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isCointnue);
				BaseDLL.decode_uint32(buffer, ref pos_, ref hp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref key1);
				UInt16 areasCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref areasCnt);
				areas = new SceneDungeonArea[areasCnt];
				for(int i = 0; i < areas.Length; i++)
				{
					areas[i] = new SceneDungeonArea();
					areas[i].decode(buffer, ref pos_);
				}
				hellInfo.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref mp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref key2);
				BaseDLL.decode_uint64(buffer, ref pos_, ref session);
				addr.decode(buffer, ref pos_);
				UInt16 playersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playersCnt);
				players = new RacePlayerInfo[playersCnt];
				for(int i = 0; i < players.Length; i++)
				{
					players[i] = new RacePlayerInfo();
					players[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref key3);
				BaseDLL.decode_int8(buffer, ref pos_, ref openAutoBattle);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref startAreaId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref key4);
				UInt16 bossDropItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref bossDropItemsCnt);
				bossDropItems = new SceneDungeonDropItem[bossDropItemsCnt];
				for(int i = 0; i < bossDropItems.Length; i++)
				{
					bossDropItems[i] = new SceneDungeonDropItem();
					bossDropItems[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_int8(buffer, ref pos_, ref isRecordLog);
				BaseDLL.decode_int8(buffer, ref pos_, ref isRecordReplay);
				UInt16 dropOverMonsterCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref dropOverMonsterCnt);
				dropOverMonster = new UInt32[dropOverMonsterCnt];
				for(int i = 0; i < dropOverMonster.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref dropOverMonster[i]);
				}
				guildDungeonInfo.decode(buffer, ref pos_);
				BaseDLL.decode_int8(buffer, ref pos_, ref hellAutoClose);
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleFlag);
				zjslDungeonInfo.decode(buffer, ref pos_);
				UInt16 clearedDungeonIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref clearedDungeonIdsCnt);
				clearedDungeonIds = new UInt32[clearedDungeonIdsCnt];
				for(int i = 0; i < clearedDungeonIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref clearedDungeonIds[i]);
				}
				for(int i = 0; i < md5.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref md5[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, isCointnue);
				BaseDLL.encode_uint32(buffer, ref pos_, hp);
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, key1);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)areas.Length);
				for(int i = 0; i < areas.Length; i++)
				{
					areas[i].encode(buffer, ref pos_);
				}
				hellInfo.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, mp);
				BaseDLL.encode_uint32(buffer, ref pos_, key2);
				BaseDLL.encode_uint64(buffer, ref pos_, session);
				addr.encode(buffer, ref pos_);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)players.Length);
				for(int i = 0; i < players.Length; i++)
				{
					players[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, key3);
				BaseDLL.encode_int8(buffer, ref pos_, openAutoBattle);
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
				BaseDLL.encode_uint32(buffer, ref pos_, startAreaId);
				BaseDLL.encode_uint32(buffer, ref pos_, key4);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)bossDropItems.Length);
				for(int i = 0; i < bossDropItems.Length; i++)
				{
					bossDropItems[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_int8(buffer, ref pos_, isRecordLog);
				BaseDLL.encode_int8(buffer, ref pos_, isRecordReplay);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)dropOverMonster.Length);
				for(int i = 0; i < dropOverMonster.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, dropOverMonster[i]);
				}
				guildDungeonInfo.encode(buffer, ref pos_);
				BaseDLL.encode_int8(buffer, ref pos_, hellAutoClose);
				BaseDLL.encode_uint32(buffer, ref pos_, battleFlag);
				zjslDungeonInfo.encode(buffer, ref pos_);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)clearedDungeonIds.Length);
				for(int i = 0; i < clearedDungeonIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, clearedDungeonIds[i]);
				}
				for(int i = 0; i < md5.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, md5[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isCointnue);
				BaseDLL.decode_uint32(buffer, ref pos_, ref hp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref key1);
				UInt16 areasCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref areasCnt);
				areas = new SceneDungeonArea[areasCnt];
				for(int i = 0; i < areas.Length; i++)
				{
					areas[i] = new SceneDungeonArea();
					areas[i].decode(buffer, ref pos_);
				}
				hellInfo.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref mp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref key2);
				BaseDLL.decode_uint64(buffer, ref pos_, ref session);
				addr.decode(buffer, ref pos_);
				UInt16 playersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playersCnt);
				players = new RacePlayerInfo[playersCnt];
				for(int i = 0; i < players.Length; i++)
				{
					players[i] = new RacePlayerInfo();
					players[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref key3);
				BaseDLL.decode_int8(buffer, ref pos_, ref openAutoBattle);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref startAreaId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref key4);
				UInt16 bossDropItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref bossDropItemsCnt);
				bossDropItems = new SceneDungeonDropItem[bossDropItemsCnt];
				for(int i = 0; i < bossDropItems.Length; i++)
				{
					bossDropItems[i] = new SceneDungeonDropItem();
					bossDropItems[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_int8(buffer, ref pos_, ref isRecordLog);
				BaseDLL.decode_int8(buffer, ref pos_, ref isRecordReplay);
				UInt16 dropOverMonsterCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref dropOverMonsterCnt);
				dropOverMonster = new UInt32[dropOverMonsterCnt];
				for(int i = 0; i < dropOverMonster.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref dropOverMonster[i]);
				}
				guildDungeonInfo.decode(buffer, ref pos_);
				BaseDLL.decode_int8(buffer, ref pos_, ref hellAutoClose);
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleFlag);
				zjslDungeonInfo.decode(buffer, ref pos_);
				UInt16 clearedDungeonIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref clearedDungeonIdsCnt);
				clearedDungeonIds = new UInt32[clearedDungeonIdsCnt];
				for(int i = 0; i < clearedDungeonIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref clearedDungeonIds[i]);
				}
				for(int i = 0; i < md5.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref md5[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// isCointnue
				_len += 1;
				// hp
				_len += 4;
				// result
				_len += 4;
				// key1
				_len += 4;
				// areas
				_len += 2;
				for(int j = 0; j < areas.Length; j++)
				{
					_len += areas[j].getLen();
				}
				// hellInfo
				_len += hellInfo.getLen();
				// mp
				_len += 4;
				// key2
				_len += 4;
				// session
				_len += 8;
				// addr
				_len += addr.getLen();
				// players
				_len += 2;
				for(int j = 0; j < players.Length; j++)
				{
					_len += players[j].getLen();
				}
				// key3
				_len += 4;
				// openAutoBattle
				_len += 1;
				// dungeonId
				_len += 4;
				// startAreaId
				_len += 4;
				// key4
				_len += 4;
				// bossDropItems
				_len += 2;
				for(int j = 0; j < bossDropItems.Length; j++)
				{
					_len += bossDropItems[j].getLen();
				}
				// isRecordLog
				_len += 1;
				// isRecordReplay
				_len += 1;
				// dropOverMonster
				_len += 2 + 4 * dropOverMonster.Length;
				// guildDungeonInfo
				_len += guildDungeonInfo.getLen();
				// hellAutoClose
				_len += 1;
				// battleFlag
				_len += 4;
				// zjslDungeonInfo
				_len += zjslDungeonInfo.getLen();
				// clearedDungeonIds
				_len += 2 + 4 * clearedDungeonIds.Length;
				// md5
				_len += 1 * md5.Length;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneDungeonAddMonsterDropItemNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506815;
		public UInt32 Sequence;
		public UInt32 monsterId;
		public SceneDungeonDropItem[] dropItems = new SceneDungeonDropItem[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, monsterId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)dropItems.Length);
				for(int i = 0; i < dropItems.Length; i++)
				{
					dropItems[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref monsterId);
				UInt16 dropItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref dropItemsCnt);
				dropItems = new SceneDungeonDropItem[dropItemsCnt];
				for(int i = 0; i < dropItems.Length; i++)
				{
					dropItems[i] = new SceneDungeonDropItem();
					dropItems[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, monsterId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)dropItems.Length);
				for(int i = 0; i < dropItems.Length; i++)
				{
					dropItems[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref monsterId);
				UInt16 dropItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref dropItemsCnt);
				dropItems = new SceneDungeonDropItem[dropItemsCnt];
				for(int i = 0; i < dropItems.Length; i++)
				{
					dropItems[i] = new SceneDungeonDropItem();
					dropItems[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// monsterId
				_len += 4;
				// dropItems
				_len += 2;
				for(int j = 0; j < dropItems.Length; j++)
				{
					_len += dropItems[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneDungeonEnterNextAreaReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506807;
		public UInt32 Sequence;
		public UInt32 areaId;
		public UInt32 lastFrame;
		public DungeonStaticValue staticVal = new DungeonStaticValue();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, areaId);
				BaseDLL.encode_uint32(buffer, ref pos_, lastFrame);
				staticVal.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref areaId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref lastFrame);
				staticVal.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, areaId);
				BaseDLL.encode_uint32(buffer, ref pos_, lastFrame);
				staticVal.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref areaId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref lastFrame);
				staticVal.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// areaId
				_len += 4;
				// lastFrame
				_len += 4;
				// staticVal
				_len += staticVal.getLen();
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneDungeonEnterNextAreaRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506808;
		public UInt32 Sequence;
		public UInt32 areaId;
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
				BaseDLL.encode_uint32(buffer, ref pos_, areaId);
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref areaId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, areaId);
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref areaId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// areaId
				_len += 4;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneDungeonRewardReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506809;
		public UInt32 Sequence;
		public UInt32 lastFrame;
		public UInt32[] dropItemIds = new UInt32[0];
		public byte[] md5 = new byte[16];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, lastFrame);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)dropItemIds.Length);
				for(int i = 0; i < dropItemIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, dropItemIds[i]);
				}
				for(int i = 0; i < md5.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, md5[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref lastFrame);
				UInt16 dropItemIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref dropItemIdsCnt);
				dropItemIds = new UInt32[dropItemIdsCnt];
				for(int i = 0; i < dropItemIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref dropItemIds[i]);
				}
				for(int i = 0; i < md5.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref md5[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, lastFrame);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)dropItemIds.Length);
				for(int i = 0; i < dropItemIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, dropItemIds[i]);
				}
				for(int i = 0; i < md5.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, md5[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref lastFrame);
				UInt16 dropItemIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref dropItemIdsCnt);
				dropItemIds = new UInt32[dropItemIdsCnt];
				for(int i = 0; i < dropItemIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref dropItemIds[i]);
				}
				for(int i = 0; i < md5.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref md5[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// lastFrame
				_len += 4;
				// dropItemIds
				_len += 2 + 4 * dropItemIds.Length;
				// md5
				_len += 1 * md5.Length;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneDungeonRewardRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506810;
		public UInt32 Sequence;
		public UInt32[] pickedItems = new UInt32[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)pickedItems.Length);
				for(int i = 0; i < pickedItems.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, pickedItems[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 pickedItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref pickedItemsCnt);
				pickedItems = new UInt32[pickedItemsCnt];
				for(int i = 0; i < pickedItems.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref pickedItems[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)pickedItems.Length);
				for(int i = 0; i < pickedItems.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, pickedItems[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 pickedItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref pickedItemsCnt);
				pickedItems = new UInt32[pickedItemsCnt];
				for(int i = 0; i < pickedItems.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref pickedItems[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// pickedItems
				_len += 2 + 4 * pickedItems.Length;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneDungeonRaceEndReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506811;
		public UInt32 Sequence;
		public byte score;
		public byte[] md5 = new byte[16];
		public UInt32 usedTime;
		public UInt16 beHitCount;
		/// <summary>
		///  最大伤害
		/// </summary>
		public UInt32 maxDamage;
		/// <summary>
		///  造成最大伤害的技能
		/// </summary>
		public UInt32 skillId;
		/// <summary>
		///  透传信息（随便用）
		/// </summary>
		public UInt32 param;
		/// <summary>
		///  总伤害
		/// </summary>
		public UInt64 totalDamage;
		/// <summary>
		///  最后一帧
		/// </summary>
		public UInt32 lastFrame;
		/// <summary>
		///  对boss的伤害
		/// </summary>
		public UInt64 bossDamage;
		/// <summary>
		///  对boss的伤害比例
		/// </summary>
		public UInt32 bossDamageRatio;
		/// <summary>
		///  玩家剩余血量
		/// </summary>
		public UInt32 playerRemainHp;
		/// <summary>
		///  玩家剩余蓝量
		/// </summary>
		public UInt32 playerRemainMp;
		/// <summary>
		///  BOSS1的ID
		/// </summary>
		public UInt32 boss1ID;
		/// <summary>
		///  BOSS2的ID
		/// </summary>
		public UInt32 boss2ID;
		/// <summary>
		///  BOSS1的剩余血量
		/// </summary>
		public UInt32 boss1RemainHp;
		/// <summary>
		///  BOSS2的剩余血量
		/// </summary>
		public UInt32 boss2RemainHp;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, score);
				for(int i = 0; i < md5.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, md5[i]);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, usedTime);
				BaseDLL.encode_uint16(buffer, ref pos_, beHitCount);
				BaseDLL.encode_uint32(buffer, ref pos_, maxDamage);
				BaseDLL.encode_uint32(buffer, ref pos_, skillId);
				BaseDLL.encode_uint32(buffer, ref pos_, param);
				BaseDLL.encode_uint64(buffer, ref pos_, totalDamage);
				BaseDLL.encode_uint32(buffer, ref pos_, lastFrame);
				BaseDLL.encode_uint64(buffer, ref pos_, bossDamage);
				BaseDLL.encode_uint32(buffer, ref pos_, bossDamageRatio);
				BaseDLL.encode_uint32(buffer, ref pos_, playerRemainHp);
				BaseDLL.encode_uint32(buffer, ref pos_, playerRemainMp);
				BaseDLL.encode_uint32(buffer, ref pos_, boss1ID);
				BaseDLL.encode_uint32(buffer, ref pos_, boss2ID);
				BaseDLL.encode_uint32(buffer, ref pos_, boss1RemainHp);
				BaseDLL.encode_uint32(buffer, ref pos_, boss2RemainHp);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref score);
				for(int i = 0; i < md5.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref md5[i]);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref usedTime);
				BaseDLL.decode_uint16(buffer, ref pos_, ref beHitCount);
				BaseDLL.decode_uint32(buffer, ref pos_, ref maxDamage);
				BaseDLL.decode_uint32(buffer, ref pos_, ref skillId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param);
				BaseDLL.decode_uint64(buffer, ref pos_, ref totalDamage);
				BaseDLL.decode_uint32(buffer, ref pos_, ref lastFrame);
				BaseDLL.decode_uint64(buffer, ref pos_, ref bossDamage);
				BaseDLL.decode_uint32(buffer, ref pos_, ref bossDamageRatio);
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerRemainHp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerRemainMp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref boss1ID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref boss2ID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref boss1RemainHp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref boss2RemainHp);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, score);
				for(int i = 0; i < md5.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, md5[i]);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, usedTime);
				BaseDLL.encode_uint16(buffer, ref pos_, beHitCount);
				BaseDLL.encode_uint32(buffer, ref pos_, maxDamage);
				BaseDLL.encode_uint32(buffer, ref pos_, skillId);
				BaseDLL.encode_uint32(buffer, ref pos_, param);
				BaseDLL.encode_uint64(buffer, ref pos_, totalDamage);
				BaseDLL.encode_uint32(buffer, ref pos_, lastFrame);
				BaseDLL.encode_uint64(buffer, ref pos_, bossDamage);
				BaseDLL.encode_uint32(buffer, ref pos_, bossDamageRatio);
				BaseDLL.encode_uint32(buffer, ref pos_, playerRemainHp);
				BaseDLL.encode_uint32(buffer, ref pos_, playerRemainMp);
				BaseDLL.encode_uint32(buffer, ref pos_, boss1ID);
				BaseDLL.encode_uint32(buffer, ref pos_, boss2ID);
				BaseDLL.encode_uint32(buffer, ref pos_, boss1RemainHp);
				BaseDLL.encode_uint32(buffer, ref pos_, boss2RemainHp);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref score);
				for(int i = 0; i < md5.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref md5[i]);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref usedTime);
				BaseDLL.decode_uint16(buffer, ref pos_, ref beHitCount);
				BaseDLL.decode_uint32(buffer, ref pos_, ref maxDamage);
				BaseDLL.decode_uint32(buffer, ref pos_, ref skillId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param);
				BaseDLL.decode_uint64(buffer, ref pos_, ref totalDamage);
				BaseDLL.decode_uint32(buffer, ref pos_, ref lastFrame);
				BaseDLL.decode_uint64(buffer, ref pos_, ref bossDamage);
				BaseDLL.decode_uint32(buffer, ref pos_, ref bossDamageRatio);
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerRemainHp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerRemainMp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref boss1ID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref boss2ID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref boss1RemainHp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref boss2RemainHp);
			}

			public int getLen()
			{
				int _len = 0;
				// score
				_len += 1;
				// md5
				_len += 1 * md5.Length;
				// usedTime
				_len += 4;
				// beHitCount
				_len += 2;
				// maxDamage
				_len += 4;
				// skillId
				_len += 4;
				// param
				_len += 4;
				// totalDamage
				_len += 8;
				// lastFrame
				_len += 4;
				// bossDamage
				_len += 8;
				// bossDamageRatio
				_len += 4;
				// playerRemainHp
				_len += 4;
				// playerRemainMp
				_len += 4;
				// boss1ID
				_len += 4;
				// boss2ID
				_len += 4;
				// boss1RemainHp
				_len += 4;
				// boss2RemainHp
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneDungeonRaceEndRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506812;
		public UInt32 Sequence;
		public UInt32 result;
		public byte score;
		public UInt32 usedTime;
		public UInt32 killMonsterTotalExp;
		public UInt32 raceEndExp;
		public byte hasRaceEndDrop;
		public byte raceEndDropBaseMulti;
		public DungeonAdditionInfo addition = new DungeonAdditionInfo();
		public ItemReward teamReward = new ItemReward();
		/// <summary>
		///  有没有结算翻牌
		/// </summary>
		public byte hasRaceEndChest;
		/// <summary>
		///  月卡结算金币奖励
		/// </summary>
		public UInt32 monthcartGoldReward;
		/// <summary>
		///  金币称号金币奖励
		/// </summary>
		public UInt32 goldTitleGoldReward;
		/// <summary>
		///  BOSS伤害金币奖励
		/// </summary>
		public UInt32 bossDamageGoldReward;
		/// <summary>
		///  疲劳燃烧类型
		/// </summary>
		public byte fatigueBurnType;
		/// <summary>
		///  翻牌翻倍标志
		/// </summary>
		public byte chestDoubleFlag;
		/// <summary>
		///  回归掉落奖励
		/// </summary>
		public ItemReward veteranReturnReward = new ItemReward();
		/// <summary>
		///  roLl掉落
		/// </summary>
		public RollItemInfo[] rollReward = new RollItemInfo[0];
		/// <summary>
		///  roll单人获取掉落情况
		/// </summary>
		public ItemReward[] rollSingleReward = new ItemReward[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_int8(buffer, ref pos_, score);
				BaseDLL.encode_uint32(buffer, ref pos_, usedTime);
				BaseDLL.encode_uint32(buffer, ref pos_, killMonsterTotalExp);
				BaseDLL.encode_uint32(buffer, ref pos_, raceEndExp);
				BaseDLL.encode_int8(buffer, ref pos_, hasRaceEndDrop);
				BaseDLL.encode_int8(buffer, ref pos_, raceEndDropBaseMulti);
				addition.encode(buffer, ref pos_);
				teamReward.encode(buffer, ref pos_);
				BaseDLL.encode_int8(buffer, ref pos_, hasRaceEndChest);
				BaseDLL.encode_uint32(buffer, ref pos_, monthcartGoldReward);
				BaseDLL.encode_uint32(buffer, ref pos_, goldTitleGoldReward);
				BaseDLL.encode_uint32(buffer, ref pos_, bossDamageGoldReward);
				BaseDLL.encode_int8(buffer, ref pos_, fatigueBurnType);
				BaseDLL.encode_int8(buffer, ref pos_, chestDoubleFlag);
				veteranReturnReward.encode(buffer, ref pos_);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)rollReward.Length);
				for(int i = 0; i < rollReward.Length; i++)
				{
					rollReward[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)rollSingleReward.Length);
				for(int i = 0; i < rollSingleReward.Length; i++)
				{
					rollSingleReward[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_int8(buffer, ref pos_, ref score);
				BaseDLL.decode_uint32(buffer, ref pos_, ref usedTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref killMonsterTotalExp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref raceEndExp);
				BaseDLL.decode_int8(buffer, ref pos_, ref hasRaceEndDrop);
				BaseDLL.decode_int8(buffer, ref pos_, ref raceEndDropBaseMulti);
				addition.decode(buffer, ref pos_);
				teamReward.decode(buffer, ref pos_);
				BaseDLL.decode_int8(buffer, ref pos_, ref hasRaceEndChest);
				BaseDLL.decode_uint32(buffer, ref pos_, ref monthcartGoldReward);
				BaseDLL.decode_uint32(buffer, ref pos_, ref goldTitleGoldReward);
				BaseDLL.decode_uint32(buffer, ref pos_, ref bossDamageGoldReward);
				BaseDLL.decode_int8(buffer, ref pos_, ref fatigueBurnType);
				BaseDLL.decode_int8(buffer, ref pos_, ref chestDoubleFlag);
				veteranReturnReward.decode(buffer, ref pos_);
				UInt16 rollRewardCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref rollRewardCnt);
				rollReward = new RollItemInfo[rollRewardCnt];
				for(int i = 0; i < rollReward.Length; i++)
				{
					rollReward[i] = new RollItemInfo();
					rollReward[i].decode(buffer, ref pos_);
				}
				UInt16 rollSingleRewardCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref rollSingleRewardCnt);
				rollSingleReward = new ItemReward[rollSingleRewardCnt];
				for(int i = 0; i < rollSingleReward.Length; i++)
				{
					rollSingleReward[i] = new ItemReward();
					rollSingleReward[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_int8(buffer, ref pos_, score);
				BaseDLL.encode_uint32(buffer, ref pos_, usedTime);
				BaseDLL.encode_uint32(buffer, ref pos_, killMonsterTotalExp);
				BaseDLL.encode_uint32(buffer, ref pos_, raceEndExp);
				BaseDLL.encode_int8(buffer, ref pos_, hasRaceEndDrop);
				BaseDLL.encode_int8(buffer, ref pos_, raceEndDropBaseMulti);
				addition.encode(buffer, ref pos_);
				teamReward.encode(buffer, ref pos_);
				BaseDLL.encode_int8(buffer, ref pos_, hasRaceEndChest);
				BaseDLL.encode_uint32(buffer, ref pos_, monthcartGoldReward);
				BaseDLL.encode_uint32(buffer, ref pos_, goldTitleGoldReward);
				BaseDLL.encode_uint32(buffer, ref pos_, bossDamageGoldReward);
				BaseDLL.encode_int8(buffer, ref pos_, fatigueBurnType);
				BaseDLL.encode_int8(buffer, ref pos_, chestDoubleFlag);
				veteranReturnReward.encode(buffer, ref pos_);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)rollReward.Length);
				for(int i = 0; i < rollReward.Length; i++)
				{
					rollReward[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)rollSingleReward.Length);
				for(int i = 0; i < rollSingleReward.Length; i++)
				{
					rollSingleReward[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_int8(buffer, ref pos_, ref score);
				BaseDLL.decode_uint32(buffer, ref pos_, ref usedTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref killMonsterTotalExp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref raceEndExp);
				BaseDLL.decode_int8(buffer, ref pos_, ref hasRaceEndDrop);
				BaseDLL.decode_int8(buffer, ref pos_, ref raceEndDropBaseMulti);
				addition.decode(buffer, ref pos_);
				teamReward.decode(buffer, ref pos_);
				BaseDLL.decode_int8(buffer, ref pos_, ref hasRaceEndChest);
				BaseDLL.decode_uint32(buffer, ref pos_, ref monthcartGoldReward);
				BaseDLL.decode_uint32(buffer, ref pos_, ref goldTitleGoldReward);
				BaseDLL.decode_uint32(buffer, ref pos_, ref bossDamageGoldReward);
				BaseDLL.decode_int8(buffer, ref pos_, ref fatigueBurnType);
				BaseDLL.decode_int8(buffer, ref pos_, ref chestDoubleFlag);
				veteranReturnReward.decode(buffer, ref pos_);
				UInt16 rollRewardCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref rollRewardCnt);
				rollReward = new RollItemInfo[rollRewardCnt];
				for(int i = 0; i < rollReward.Length; i++)
				{
					rollReward[i] = new RollItemInfo();
					rollReward[i].decode(buffer, ref pos_);
				}
				UInt16 rollSingleRewardCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref rollSingleRewardCnt);
				rollSingleReward = new ItemReward[rollSingleRewardCnt];
				for(int i = 0; i < rollSingleReward.Length; i++)
				{
					rollSingleReward[i] = new ItemReward();
					rollSingleReward[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// score
				_len += 1;
				// usedTime
				_len += 4;
				// killMonsterTotalExp
				_len += 4;
				// raceEndExp
				_len += 4;
				// hasRaceEndDrop
				_len += 1;
				// raceEndDropBaseMulti
				_len += 1;
				// addition
				_len += addition.getLen();
				// teamReward
				_len += teamReward.getLen();
				// hasRaceEndChest
				_len += 1;
				// monthcartGoldReward
				_len += 4;
				// goldTitleGoldReward
				_len += 4;
				// bossDamageGoldReward
				_len += 4;
				// fatigueBurnType
				_len += 1;
				// chestDoubleFlag
				_len += 1;
				// veteranReturnReward
				_len += veteranReturnReward.getLen();
				// rollReward
				_len += 2;
				for(int j = 0; j < rollReward.Length; j++)
				{
					_len += rollReward[j].getLen();
				}
				// rollSingleReward
				_len += 2;
				for(int j = 0; j < rollSingleReward.Length; j++)
				{
					_len += rollSingleReward[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneDungeonChestNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506816;
		public UInt32 Sequence;
		/// <summary>
		///  宝箱付费货币类型
		/// </summary>
		public UInt32 payChestCostItemId;
		/// <summary>
		///  宝箱付费货币数量
		/// </summary>
		public UInt32 payChestCost;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, payChestCostItemId);
				BaseDLL.encode_uint32(buffer, ref pos_, payChestCost);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref payChestCostItemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref payChestCost);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, payChestCostItemId);
				BaseDLL.encode_uint32(buffer, ref pos_, payChestCost);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref payChestCostItemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref payChestCost);
			}

			public int getLen()
			{
				int _len = 0;
				// payChestCostItemId
				_len += 4;
				// payChestCost
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneDungeonOpenChestReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506813;
		public UInt32 Sequence;
		public byte pos;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, pos);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref pos);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, pos);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref pos);
			}

			public int getLen()
			{
				int _len = 0;
				// pos
				_len += 1;
				return _len;
			}
		#endregion

	}

	public class DungeonChest : Protocol.IProtocolStream
	{
		public UInt32 itemId;
		public UInt32 num;
		public byte type;
		public UInt32 goldReward;
		public byte isRareControl;
		public byte strenth;
		/// <summary>
		///  装备类型，对应枚举EquipType
		/// </summary>
		public byte equipType;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, goldReward);
				BaseDLL.encode_int8(buffer, ref pos_, isRareControl);
				BaseDLL.encode_int8(buffer, ref pos_, strenth);
				BaseDLL.encode_int8(buffer, ref pos_, equipType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref goldReward);
				BaseDLL.decode_int8(buffer, ref pos_, ref isRareControl);
				BaseDLL.decode_int8(buffer, ref pos_, ref strenth);
				BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, goldReward);
				BaseDLL.encode_int8(buffer, ref pos_, isRareControl);
				BaseDLL.encode_int8(buffer, ref pos_, strenth);
				BaseDLL.encode_int8(buffer, ref pos_, equipType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref goldReward);
				BaseDLL.decode_int8(buffer, ref pos_, ref isRareControl);
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
				// type
				_len += 1;
				// goldReward
				_len += 4;
				// isRareControl
				_len += 1;
				// strenth
				_len += 1;
				// equipType
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneDungeonOpenChestRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506814;
		public UInt32 Sequence;
		public UInt64 owner;
		public byte pos;
		public DungeonChest chest = new DungeonChest();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, owner);
				BaseDLL.encode_int8(buffer, ref pos_, pos);
				chest.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref owner);
				BaseDLL.decode_int8(buffer, ref pos_, ref pos);
				chest.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, owner);
				BaseDLL.encode_int8(buffer, ref pos_, pos);
				chest.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref owner);
				BaseDLL.decode_int8(buffer, ref pos_, ref pos);
				chest.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// owner
				_len += 8;
				// pos
				_len += 1;
				// chest
				_len += chest.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求复活
	/// </summary>
	[Protocol]
	public class SceneDungeonReviveReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506817;
		public UInt32 Sequence;
		/// <summary>
		///  要复活的目标
		/// </summary>
		public UInt64 targetId;
		/// <summary>
		///  每一次复活都有一个ID
		/// </summary>
		public UInt32 reviveId;
		/// <summary>
		///  消耗的复活币数量
		/// </summary>
		public UInt32 reviveCoinNum;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, targetId);
				BaseDLL.encode_uint32(buffer, ref pos_, reviveId);
				BaseDLL.encode_uint32(buffer, ref pos_, reviveCoinNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref targetId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref reviveId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref reviveCoinNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, targetId);
				BaseDLL.encode_uint32(buffer, ref pos_, reviveId);
				BaseDLL.encode_uint32(buffer, ref pos_, reviveCoinNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref targetId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref reviveId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref reviveCoinNum);
			}

			public int getLen()
			{
				int _len = 0;
				// targetId
				_len += 8;
				// reviveId
				_len += 4;
				// reviveCoinNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneDungeonReviveRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506818;
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
	/// 玩家杀死怪物通知
	/// </summary>
	[Protocol]
	public class SceneDungeonKillMonsterReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506819;
		public UInt32 Sequence;
		public UInt32[] monsterIds = new UInt32[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)monsterIds.Length);
				for(int i = 0; i < monsterIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, monsterIds[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 monsterIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref monsterIdsCnt);
				monsterIds = new UInt32[monsterIdsCnt];
				for(int i = 0; i < monsterIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref monsterIds[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)monsterIds.Length);
				for(int i = 0; i < monsterIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, monsterIds[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 monsterIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref monsterIdsCnt);
				monsterIds = new UInt32[monsterIdsCnt];
				for(int i = 0; i < monsterIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref monsterIds[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// monsterIds
				_len += 2 + 4 * monsterIds.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 玩家杀死怪物返回
	/// </summary>
	[Protocol]
	public class SceneDungeonKillMonsterRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506820;
		public UInt32 Sequence;
		public UInt32[] monsterIds = new UInt32[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)monsterIds.Length);
				for(int i = 0; i < monsterIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, monsterIds[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 monsterIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref monsterIdsCnt);
				monsterIds = new UInt32[monsterIdsCnt];
				for(int i = 0; i < monsterIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref monsterIds[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)monsterIds.Length);
				for(int i = 0; i < monsterIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, monsterIds[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 monsterIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref monsterIdsCnt);
				monsterIds = new UInt32[monsterIdsCnt];
				for(int i = 0; i < monsterIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref monsterIds[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// monsterIds
				_len += 2 + 4 * monsterIds.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 玩家杀死怪物返回
	/// </summary>
	[Protocol]
	public class SceneDungeonClearAreaMonsters : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506821;
		public UInt32 Sequence;
		/// <summary>
		///  使用时间(ms)
		/// </summary>
		public UInt32 usedTime;
		/// <summary>
		///  剩余蓝量
		/// </summary>
		public UInt32 remainMp;
		public byte[] md5 = new byte[16];
		/// <summary>
		///  剩余血量
		/// </summary>
		public UInt32 remainHp;
		/// <summary>
		///  最后一帧
		/// </summary>
		public UInt32 lastFrame;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, usedTime);
				BaseDLL.encode_uint32(buffer, ref pos_, remainMp);
				for(int i = 0; i < md5.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, md5[i]);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, remainHp);
				BaseDLL.encode_uint32(buffer, ref pos_, lastFrame);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref usedTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref remainMp);
				for(int i = 0; i < md5.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref md5[i]);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref remainHp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref lastFrame);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, usedTime);
				BaseDLL.encode_uint32(buffer, ref pos_, remainMp);
				for(int i = 0; i < md5.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, md5[i]);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, remainHp);
				BaseDLL.encode_uint32(buffer, ref pos_, lastFrame);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref usedTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref remainMp);
				for(int i = 0; i < md5.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref md5[i]);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref remainHp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref lastFrame);
			}

			public int getLen()
			{
				int _len = 0;
				// usedTime
				_len += 4;
				// remainMp
				_len += 4;
				// md5
				_len += 1 * md5.Length;
				// remainHp
				_len += 4;
				// lastFrame
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  地下城开放信息
	/// </summary>
	public class DungeonOpenInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  地下城ID
		/// </summary>
		public UInt32 id;
		/// <summary>
		///  是否开放深渊模式(1:开放，0:不开放)
		/// </summary>
		public byte hellMode;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, hellMode);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref hellMode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, hellMode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref hellMode);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// hellMode
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 同步新开放的地下城列表
	/// </summary>
	[Protocol]
	public class SceneDungeonSyncNewOpenedList : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506822;
		public UInt32 Sequence;
		/// <summary>
		/// 新开放的地下城列表
		/// </summary>
		public DungeonOpenInfo[] newOpenedList = new DungeonOpenInfo[0];
		/// <summary>
		/// 新关闭掉的地下城列表
		/// </summary>
		public UInt32[] newClosedList = new UInt32[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)newOpenedList.Length);
				for(int i = 0; i < newOpenedList.Length; i++)
				{
					newOpenedList[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)newClosedList.Length);
				for(int i = 0; i < newClosedList.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, newClosedList[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 newOpenedListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref newOpenedListCnt);
				newOpenedList = new DungeonOpenInfo[newOpenedListCnt];
				for(int i = 0; i < newOpenedList.Length; i++)
				{
					newOpenedList[i] = new DungeonOpenInfo();
					newOpenedList[i].decode(buffer, ref pos_);
				}
				UInt16 newClosedListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref newClosedListCnt);
				newClosedList = new UInt32[newClosedListCnt];
				for(int i = 0; i < newClosedList.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref newClosedList[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)newOpenedList.Length);
				for(int i = 0; i < newOpenedList.Length; i++)
				{
					newOpenedList[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)newClosedList.Length);
				for(int i = 0; i < newClosedList.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, newClosedList[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 newOpenedListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref newOpenedListCnt);
				newOpenedList = new DungeonOpenInfo[newOpenedListCnt];
				for(int i = 0; i < newOpenedList.Length; i++)
				{
					newOpenedList[i] = new DungeonOpenInfo();
					newOpenedList[i].decode(buffer, ref pos_);
				}
				UInt16 newClosedListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref newClosedListCnt);
				newClosedList = new UInt32[newClosedListCnt];
				for(int i = 0; i < newClosedList.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref newClosedList[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// newOpenedList
				_len += 2;
				for(int j = 0; j < newOpenedList.Length; j++)
				{
					_len += newOpenedList[j].getLen();
				}
				// newClosedList
				_len += 2 + 4 * newClosedList.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求结算掉落
	/// </summary>
	[Protocol]
	public class SceneDungeonEndDropReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506823;
		public UInt32 Sequence;
		/// <summary>
		///  倍率
		/// </summary>
		public byte multi;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, multi);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref multi);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, multi);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref multi);
			}

			public int getLen()
			{
				int _len = 0;
				// multi
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  返回结算掉落
	/// </summary>
	[Protocol]
	public class SceneDungeonEndDropRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506824;
		public UInt32 Sequence;
		/// <summary>
		///  总倍率（0代表获取失败）
		/// </summary>
		public byte multi;
		/// <summary>
		///  掉落物品
		/// </summary>
		public SceneDungeonDropItem[] items = new SceneDungeonDropItem[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, multi);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref multi);
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new SceneDungeonDropItem[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new SceneDungeonDropItem();
					items[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, multi);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref multi);
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new SceneDungeonDropItem[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new SceneDungeonDropItem();
					items[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// multi
				_len += 1;
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
	///  请求扫荡死亡之塔
	/// </summary>
	[Protocol]
	public class SceneTowerWipeoutReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507201;
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
	///  扫荡死亡之塔返回
	/// </summary>
	[Protocol]
	public class SceneTowerWipeoutRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507202;
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
	///  请求死亡之塔扫荡奖励
	/// </summary>
	[Protocol]
	public class SceneTowerWipeoutResultReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507203;
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
	///  死亡之塔扫荡奖励返回
	/// </summary>
	[Protocol]
	public class SceneTowerWipeoutResultRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507204;
		public UInt32 Sequence;
		public UInt32 result;
		public SceneDungeonDropItem[] items = new SceneDungeonDropItem[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new SceneDungeonDropItem[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new SceneDungeonDropItem();
					items[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new SceneDungeonDropItem[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new SceneDungeonDropItem();
					items[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// result
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
	///  请求死亡之塔扫荡奖励（指定层数）
	/// </summary>
	[Protocol]
	public class SceneTowerWipeoutQueryResultReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507211;
		public UInt32 Sequence;
		/// <summary>
		/// 起始层数
		/// </summary>
		public UInt32 beginFloor;
		/// <summary>
		/// 结束层数
		/// </summary>
		public UInt32 endFloor;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, beginFloor);
				BaseDLL.encode_uint32(buffer, ref pos_, endFloor);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref beginFloor);
				BaseDLL.decode_uint32(buffer, ref pos_, ref endFloor);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, beginFloor);
				BaseDLL.encode_uint32(buffer, ref pos_, endFloor);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref beginFloor);
				BaseDLL.decode_uint32(buffer, ref pos_, ref endFloor);
			}

			public int getLen()
			{
				int _len = 0;
				// beginFloor
				_len += 4;
				// endFloor
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  死亡之塔扫荡奖励返回
	/// </summary>
	[Protocol]
	public class SceneTowerWipeoutQueryResultRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507212;
		public UInt32 Sequence;
		public UInt32 result;
		public SceneDungeonDropItem[] items = new SceneDungeonDropItem[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new SceneDungeonDropItem[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new SceneDungeonDropItem();
					items[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new SceneDungeonDropItem[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new SceneDungeonDropItem();
					items[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// result
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
	///  请求快速完成死亡之塔扫荡
	/// </summary>
	[Protocol]
	public class SceneTowerWipeoutQuickFinishReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507205;
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
	///  快速完成死亡之塔返回
	/// </summary>
	[Protocol]
	public class SceneTowerWipeoutQuickFinishRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507206;
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
	///  请求重置死亡之塔
	/// </summary>
	[Protocol]
	public class SceneTowerResetReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507207;
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
	///  重置死亡之塔返回
	/// </summary>
	[Protocol]
	public class SceneTowerResetRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507208;
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
	///  请求死亡之塔首通奖励
	/// </summary>
	[Protocol]
	public class SceneTowerFloorAwardReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507209;
		public UInt32 Sequence;
		public UInt32 floor;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref floor);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, floor);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref floor);
			}

			public int getLen()
			{
				int _len = 0;
				// floor
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  领取死亡之塔首通奖励返回
	/// </summary>
	[Protocol]
	public class SceneTowerFloorAwardRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507210;
		public UInt32 Sequence;
		public UInt32 result;
		public SceneDungeonDropItem[] items = new SceneDungeonDropItem[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new SceneDungeonDropItem[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new SceneDungeonDropItem();
					items[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new SceneDungeonDropItem[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new SceneDungeonDropItem();
					items[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// result
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
	///  请求购买地下城次数
	/// </summary>
	[Protocol]
	public class SceneDungeonBuyTimesReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506831;
		public UInt32 Sequence;
		public byte subType;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, subType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref subType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, subType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref subType);
			}

			public int getLen()
			{
				int _len = 0;
				// subType
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  购买地下城次数返回
	/// </summary>
	[Protocol]
	public class SceneDungeonBuyTimesRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506832;
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
	///  请求重置地下城房间索引
	/// </summary>
	[Protocol]
	public class SceneDungeonResetAreaIndexReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506833;
		public UInt32 Sequence;
		/// <summary>
		///  地下城ID
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
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
			}

			public int getLen()
			{
				int _len = 0;
				// dungeonId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  重置地下城房间索引返回
	/// </summary>
	[Protocol]
	public class SceneDungeonResetAreaIndexRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506834;
		public UInt32 Sequence;
		/// <summary>
		///  地下城ID
		/// </summary>
		public UInt32 dungeonId;
		/// <summary>
		///  房间索引
		/// </summary>
		public UInt32 areaIndex;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint32(buffer, ref pos_, areaIndex);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref areaIndex);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
				BaseDLL.encode_uint32(buffer, ref pos_, areaIndex);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref areaIndex);
			}

			public int getLen()
			{
				int _len = 0;
				// dungeonId
				_len += 4;
				// areaIndex
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  通知服务器进入比赛了
	/// </summary>
	[Protocol]
	public class WorldDungeonEnterRaceReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 606809;
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
	///  服务器返回玩家进入比赛
	/// </summary>
	[Protocol]
	public class WorldDungeonEnterRaceRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 606810;
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
	///  玩家上报帧数据
	/// </summary>
	[Protocol]
	public class WorldDungeonReportFrameReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 606811;
		public UInt32 Sequence;
		/// <summary>
		///  操作帧
		/// </summary>
		public Frame[] frames = new Frame[0];
		/// <summary>
		///  随机数
		/// </summary>
		public FrameChecksum[] checksums = new FrameChecksum[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)frames.Length);
				for(int i = 0; i < frames.Length; i++)
				{
					frames[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)checksums.Length);
				for(int i = 0; i < checksums.Length; i++)
				{
					checksums[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 framesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref framesCnt);
				frames = new Frame[framesCnt];
				for(int i = 0; i < frames.Length; i++)
				{
					frames[i] = new Frame();
					frames[i].decode(buffer, ref pos_);
				}
				UInt16 checksumsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref checksumsCnt);
				checksums = new FrameChecksum[checksumsCnt];
				for(int i = 0; i < checksums.Length; i++)
				{
					checksums[i] = new FrameChecksum();
					checksums[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)frames.Length);
				for(int i = 0; i < frames.Length; i++)
				{
					frames[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)checksums.Length);
				for(int i = 0; i < checksums.Length; i++)
				{
					checksums[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 framesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref framesCnt);
				frames = new Frame[framesCnt];
				for(int i = 0; i < frames.Length; i++)
				{
					frames[i] = new Frame();
					frames[i].decode(buffer, ref pos_);
				}
				UInt16 checksumsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref checksumsCnt);
				checksums = new FrameChecksum[checksumsCnt];
				for(int i = 0; i < checksums.Length; i++)
				{
					checksums[i] = new FrameChecksum();
					checksums[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// frames
				_len += 2;
				for(int j = 0; j < frames.Length; j++)
				{
					_len += frames[j].getLen();
				}
				// checksums
				_len += 2;
				for(int j = 0; j < checksums.Length; j++)
				{
					_len += checksums[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求地下城房间索引
	/// </summary>
	[Protocol]
	public class WorldDungeonGetAreaIndexReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 606815;
		public UInt32 Sequence;
		/// <summary>
		///  地下城ID
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
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
			}

			public int getLen()
			{
				int _len = 0;
				// dungeonId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  地下城房间索引返回
	/// </summary>
	[Protocol]
	public class WorldDungeonGetAreaIndexRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 606816;
		public UInt32 Sequence;
		/// <summary>
		///  地下城ID
		/// </summary>
		public UInt32 dungeonId;
		/// <summary>
		///  房间索引
		/// </summary>
		public UInt32 areaIndex;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint32(buffer, ref pos_, areaIndex);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref areaIndex);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
				BaseDLL.encode_uint32(buffer, ref pos_, areaIndex);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref areaIndex);
			}

			public int getLen()
			{
				int _len = 0;
				// dungeonId
				_len += 4;
				// areaIndex
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  返回玩家上报帧结果
	/// </summary>
	[Protocol]
	public class WorldDungeonReportFrameRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 606812;
		public UInt32 Sequence;
		/// <summary>
		///  收到的最后一帧
		/// </summary>
		public UInt32 lastFrame;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, lastFrame);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref lastFrame);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, lastFrame);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref lastFrame);
			}

			public int getLen()
			{
				int _len = 0;
				// lastFrame
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  终极试炼地下城刷新BUFF请求
	/// </summary>
	[Protocol]
	public class SceneDungeonZjslRefreshBuffReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506835;
		public UInt32 Sequence;
		/// <summary>
		///  地下城ID
		/// </summary>
		public UInt32 dungeonId;
		/// <summary>
		///  使用刷新票
		/// </summary>
		public UInt32 useItem;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint32(buffer, ref pos_, useItem);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref useItem);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
				BaseDLL.encode_uint32(buffer, ref pos_, useItem);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref useItem);
			}

			public int getLen()
			{
				int _len = 0;
				// dungeonId
				_len += 4;
				// useItem
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  终极试炼地下城刷新BUFF返回
	/// </summary>
	[Protocol]
	public class SceneDungeonZjslRefreshBuffRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506836;
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
	///  client->world	公共掉落roll请求
	/// </summary>
	[Protocol]
	public class WorldDungeonRollItemReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 606817;
		public UInt32 Sequence;
		/// <summary>
		/// 掉落索引
		/// </summary>
		public byte dropIndex;
		/// <summary>
		/// 请求类型
		/// </summary>
		public byte opType;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, dropIndex);
				BaseDLL.encode_int8(buffer, ref pos_, opType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref dropIndex);
				BaseDLL.decode_int8(buffer, ref pos_, ref opType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, dropIndex);
				BaseDLL.encode_int8(buffer, ref pos_, opType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref dropIndex);
				BaseDLL.decode_int8(buffer, ref pos_, ref opType);
			}

			public int getLen()
			{
				int _len = 0;
				// dropIndex
				_len += 1;
				// opType
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  world->client	公共掉落roll请求返回
	/// </summary>
	[Protocol]
	public class WorldDungeonRollItemRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 606818;
		public UInt32 Sequence;
		/// <summary>
		/// 错误码
		/// </summary>
		public UInt32 result;
		/// <summary>
		/// 掉落索引
		/// </summary>
		public byte dropIndex;
		/// <summary>
		/// 请求类型
		/// </summary>
		public byte opType;
		/// <summary>
		/// roll点数
		/// </summary>
		public UInt32 point;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_int8(buffer, ref pos_, dropIndex);
				BaseDLL.encode_int8(buffer, ref pos_, opType);
				BaseDLL.encode_uint32(buffer, ref pos_, point);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_int8(buffer, ref pos_, ref dropIndex);
				BaseDLL.decode_int8(buffer, ref pos_, ref opType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref point);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_int8(buffer, ref pos_, dropIndex);
				BaseDLL.encode_int8(buffer, ref pos_, opType);
				BaseDLL.encode_uint32(buffer, ref pos_, point);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_int8(buffer, ref pos_, ref dropIndex);
				BaseDLL.decode_int8(buffer, ref pos_, ref opType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref point);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// dropIndex
				_len += 1;
				// opType
				_len += 1;
				// point
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  world->client 公共掉落roll结算信息
	/// </summary>
	[Protocol]
	public class WorldDungeonRollItemResult : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 606819;
		public UInt32 Sequence;
		/// <summary>
		/// roll信息
		/// </summary>
		public RollDropResultItem[] items = new RollDropResultItem[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new RollDropResultItem[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new RollDropResultItem();
					items[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new RollDropResultItem[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new RollDropResultItem();
					items[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
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

}
