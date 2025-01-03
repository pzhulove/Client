using System;
using System.Text;

namespace Protocol
{
	/// <summary>
	///  战斗类型
	/// </summary>
	public enum RaceType
	{
		/// <summary>
		///  关卡
		/// </summary>
		Dungeon = 0,
		/// <summary>
		///  PK
		/// </summary>
		PK = 1,
		/// <summary>
		///  公会战
		/// </summary>
		GuildBattle = 2,
		/// <summary>
		///  赏金联赛预选赛
		/// </summary>
		PremiumLeaguePreliminay = 3,
		/// <summary>
		///  赏金联赛淘汰赛
		/// </summary>
		PremiumLeagueBattle = 4,
		/// <summary>
		///  3v3
		/// </summary>
		PK_3V3 = 5,
		/// <summary>
		///  3v3积分赛
		/// </summary>
		ScoreWar = 6,
		/// <summary>
		///  3v3乱斗
		/// </summary>
		PK_3V3_Melee = 7,
		/// <summary>
		///  吃鸡
		/// </summary>
		ChiJi = 8,
		/// <summary>
		/// 公平竞技场
		/// </summary>
		PK_EQUAL_1V1 = 9,
		/// <summary>
		///  2V2乱斗活动
		/// </summary>
		PK_2V2_Melee = 11,
	}

	/// <summary>
	/// 装备附带属性
	/// </summary>
	public enum EquipExProp
	{
		EEP_LIGHT = 0,
		/// <summary>
		/// 光属性
		/// </summary>
		EEP_FIRE = 1,
		/// <summary>
		/// 火属性
		/// </summary>
		EEP_ICE = 2,
		/// <summary>
		/// 冰属性
		/// </summary>
		EEP_DARK = 3,
		/// <summary>
		/// 暗属性
		/// </summary>
		EEP_MAX = 4,
	}

	/// <summary>
	/// 装备异常抗性
	/// </summary>
	public enum EquipAbnormalResist
	{
		EAR_FLASH = 0,
		/// <summary>
		/// 感电
		/// </summary>
		EAR_BLEEDING = 1,
		/// <summary>
		/// 出血
		/// </summary>
		EAR_BURN = 2,
		/// <summary>
		/// 灼烧
		/// </summary>
		EAR_POISON = 3,
		/// <summary>
		/// 中毒
		/// </summary>
		EAR_BLIND = 4,
		/// <summary>
		/// 失明
		/// </summary>
		EAR_STUN = 5,
		/// <summary>
		/// 晕眩
		/// </summary>
		EAR_STONE = 6,
		/// <summary>
		/// 石化
		/// </summary>
		EAR_FROZEN = 7,
		/// <summary>
		/// 冰冻
		/// </summary>
		EAR_SLEEP = 8,
		/// <summary>
		/// 睡眠
		/// </summary>
		EAR_CONFUNSE = 9,
		/// <summary>
		/// 混乱
		/// </summary>
		EAR_STRAIN = 10,
		/// <summary>
		/// 束缚
		/// </summary>
		EAR_SPEED_DOWN = 11,
		/// <summary>
		/// 减速
		/// </summary>
		EAR_CURSE = 12,
		/// <summary>
		/// 诅咒
		/// </summary>
		EAR_MAX = 13,
	}

	/// <summary>
	///  好友状态
	/// </summary>
	public enum FriendMatchStatus
	{
		/// <summary>
		///  空闲
		/// </summary>
		Idle = 0,
		/// <summary>
		///  忙碌
		/// </summary>
		Busy = 1,
		/// <summary>
		///  下线
		/// </summary>
		Offlie = 2,
	}

	/// <summary>
	///  赛季状态
	/// </summary>
	public enum SeasonPlayStatus
	{
		SPS_INVALID = 0,
		SPS_NEW = 1,
		SPS_NEW_ATTR = 2,
	}

	[Protocol]
	public class WorldMatchStartReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506701;
		public UInt32 Sequence;
		/// <summary>
		///  匹配类型，对应MatchType
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

	[Protocol]
	public class WorldMatchStartRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 606702;
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
	public class WorldMatchCancelReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506702;
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
	public class WorldMatchCancelRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 606703;
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

	public class RaceSkillInfo : Protocol.IProtocolStream
	{
		public UInt16 id;
		public byte level;
		public UInt32 talentId;
		public byte slot;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, level);
				BaseDLL.encode_uint32(buffer, ref pos_, talentId);
				BaseDLL.encode_int8(buffer, ref pos_, slot);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
				BaseDLL.decode_uint32(buffer, ref pos_, ref talentId);
				BaseDLL.decode_int8(buffer, ref pos_, ref slot);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, level);
				BaseDLL.encode_uint32(buffer, ref pos_, talentId);
				BaseDLL.encode_int8(buffer, ref pos_, slot);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
				BaseDLL.decode_uint32(buffer, ref pos_, ref talentId);
				BaseDLL.decode_int8(buffer, ref pos_, ref slot);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 2;
				// level
				_len += 1;
				// talentId
				_len += 4;
				// slot
				_len += 1;
				return _len;
			}
		#endregion

	}

	public class RaceItemRandProperty : Protocol.IProtocolStream
	{
		public byte type;
		public UInt32 value;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, value);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref value);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, value);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref value);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// value
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 镶嵌宝珠信息
	/// </summary>
	public class RacePrecBead : Protocol.IProtocolStream
	{
		public UInt32 preciousBeadId;
		public UInt32 buffId;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, preciousBeadId);
				BaseDLL.encode_uint32(buffer, ref pos_, buffId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref preciousBeadId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref buffId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, preciousBeadId);
				BaseDLL.encode_uint32(buffer, ref pos_, buffId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref preciousBeadId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref buffId);
			}

			public int getLen()
			{
				int _len = 0;
				// preciousBeadId
				_len += 4;
				// buffId
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class RaceEquip : Protocol.IProtocolStream
	{
		public UInt64 uid;
		public UInt32 id;
		public UInt32 pos;
		public UInt32 phyAtk;
		public UInt32 magAtk;
		public UInt32 phydef;
		public UInt32 magdef;
		public UInt32 strenth;
		public UInt32 stamina;
		public UInt32 intellect;
		public UInt32 spirit;
		public RaceItemRandProperty[] properties = new RaceItemRandProperty[0];
		public UInt32 magicCard;
		public UInt32 disphyAtk;
		public UInt32 disMagAtk;
		public UInt32 disphydef;
		public UInt32 dismagdef;
		public byte strengthen;
		public UInt32 fashionAttrId;
		public UInt32 phyDefPercent;
		public UInt32 magDefPercent;
		public UInt32 preciousBeadId;
		public byte isTimeLimit;
		public Int32[] atkPropEx = new Int32[0];
		/// <summary>
		///  攻击附带属性EquipExProp
		/// </summary>
		public Int32[] strPropEx = new Int32[0];
		/// <summary>
		///  属强EquipExProp
		/// </summary>
		public Int32[] defPropEx = new Int32[0];
		/// <summary>
		///  属抗EquipExProp
		/// </summary>
		public Int32 abnormalResistsTotal;
		public Int32[] abnormalResists = new Int32[0];
		/// <summary>
		///  异抗EquipAbnormalResist
		/// </summary>
		public RacePrecBead[] mountPrecBeads = new RacePrecBead[0];
		public byte magicCardLv;
		/// <summary>
		/// 附魔卡等级
		/// </summary>
		public byte equipType;
		/// <summary>
		/// 装备类型:普通0/带气息1/红字2
		/// </summary>
		public byte enhanceType;
		/// <summary>
		/// 红字装备增幅类型:力量0/智力1/体力2/精神3
		/// </summary>
		public UInt32 enhanceNum;
		/// <summary>
		/// 红字装备增幅数值
		/// </summary>
		public UInt32[] inscriptionIds = new UInt32[0];
		/// <summary>
		/// 铭文
		/// </summary>
		public UInt32 independAtk;
		/// <summary>
		/// 独立攻击力
		/// </summary>
		public UInt32 indpendAtkStreng;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, pos);
				BaseDLL.encode_uint32(buffer, ref pos_, phyAtk);
				BaseDLL.encode_uint32(buffer, ref pos_, magAtk);
				BaseDLL.encode_uint32(buffer, ref pos_, phydef);
				BaseDLL.encode_uint32(buffer, ref pos_, magdef);
				BaseDLL.encode_uint32(buffer, ref pos_, strenth);
				BaseDLL.encode_uint32(buffer, ref pos_, stamina);
				BaseDLL.encode_uint32(buffer, ref pos_, intellect);
				BaseDLL.encode_uint32(buffer, ref pos_, spirit);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)properties.Length);
				for(int i = 0; i < properties.Length; i++)
				{
					properties[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, magicCard);
				BaseDLL.encode_uint32(buffer, ref pos_, disphyAtk);
				BaseDLL.encode_uint32(buffer, ref pos_, disMagAtk);
				BaseDLL.encode_uint32(buffer, ref pos_, disphydef);
				BaseDLL.encode_uint32(buffer, ref pos_, dismagdef);
				BaseDLL.encode_int8(buffer, ref pos_, strengthen);
				BaseDLL.encode_uint32(buffer, ref pos_, fashionAttrId);
				BaseDLL.encode_uint32(buffer, ref pos_, phyDefPercent);
				BaseDLL.encode_uint32(buffer, ref pos_, magDefPercent);
				BaseDLL.encode_uint32(buffer, ref pos_, preciousBeadId);
				BaseDLL.encode_int8(buffer, ref pos_, isTimeLimit);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)atkPropEx.Length);
				for(int i = 0; i < atkPropEx.Length; i++)
				{
					BaseDLL.encode_int32(buffer, ref pos_, atkPropEx[i]);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)strPropEx.Length);
				for(int i = 0; i < strPropEx.Length; i++)
				{
					BaseDLL.encode_int32(buffer, ref pos_, strPropEx[i]);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)defPropEx.Length);
				for(int i = 0; i < defPropEx.Length; i++)
				{
					BaseDLL.encode_int32(buffer, ref pos_, defPropEx[i]);
				}
				BaseDLL.encode_int32(buffer, ref pos_, abnormalResistsTotal);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)abnormalResists.Length);
				for(int i = 0; i < abnormalResists.Length; i++)
				{
					BaseDLL.encode_int32(buffer, ref pos_, abnormalResists[i]);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mountPrecBeads.Length);
				for(int i = 0; i < mountPrecBeads.Length; i++)
				{
					mountPrecBeads[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_int8(buffer, ref pos_, magicCardLv);
				BaseDLL.encode_int8(buffer, ref pos_, equipType);
				BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
				BaseDLL.encode_uint32(buffer, ref pos_, enhanceNum);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)inscriptionIds.Length);
				for(int i = 0; i < inscriptionIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, inscriptionIds[i]);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, independAtk);
				BaseDLL.encode_uint32(buffer, ref pos_, indpendAtkStreng);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref pos);
				BaseDLL.decode_uint32(buffer, ref pos_, ref phyAtk);
				BaseDLL.decode_uint32(buffer, ref pos_, ref magAtk);
				BaseDLL.decode_uint32(buffer, ref pos_, ref phydef);
				BaseDLL.decode_uint32(buffer, ref pos_, ref magdef);
				BaseDLL.decode_uint32(buffer, ref pos_, ref strenth);
				BaseDLL.decode_uint32(buffer, ref pos_, ref stamina);
				BaseDLL.decode_uint32(buffer, ref pos_, ref intellect);
				BaseDLL.decode_uint32(buffer, ref pos_, ref spirit);
				UInt16 propertiesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref propertiesCnt);
				properties = new RaceItemRandProperty[propertiesCnt];
				for(int i = 0; i < properties.Length; i++)
				{
					properties[i] = new RaceItemRandProperty();
					properties[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref magicCard);
				BaseDLL.decode_uint32(buffer, ref pos_, ref disphyAtk);
				BaseDLL.decode_uint32(buffer, ref pos_, ref disMagAtk);
				BaseDLL.decode_uint32(buffer, ref pos_, ref disphydef);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dismagdef);
				BaseDLL.decode_int8(buffer, ref pos_, ref strengthen);
				BaseDLL.decode_uint32(buffer, ref pos_, ref fashionAttrId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref phyDefPercent);
				BaseDLL.decode_uint32(buffer, ref pos_, ref magDefPercent);
				BaseDLL.decode_uint32(buffer, ref pos_, ref preciousBeadId);
				BaseDLL.decode_int8(buffer, ref pos_, ref isTimeLimit);
				UInt16 atkPropExCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref atkPropExCnt);
				atkPropEx = new Int32[atkPropExCnt];
				for(int i = 0; i < atkPropEx.Length; i++)
				{
					BaseDLL.decode_int32(buffer, ref pos_, ref atkPropEx[i]);
				}
				UInt16 strPropExCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref strPropExCnt);
				strPropEx = new Int32[strPropExCnt];
				for(int i = 0; i < strPropEx.Length; i++)
				{
					BaseDLL.decode_int32(buffer, ref pos_, ref strPropEx[i]);
				}
				UInt16 defPropExCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref defPropExCnt);
				defPropEx = new Int32[defPropExCnt];
				for(int i = 0; i < defPropEx.Length; i++)
				{
					BaseDLL.decode_int32(buffer, ref pos_, ref defPropEx[i]);
				}
				BaseDLL.decode_int32(buffer, ref pos_, ref abnormalResistsTotal);
				UInt16 abnormalResistsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref abnormalResistsCnt);
				abnormalResists = new Int32[abnormalResistsCnt];
				for(int i = 0; i < abnormalResists.Length; i++)
				{
					BaseDLL.decode_int32(buffer, ref pos_, ref abnormalResists[i]);
				}
				UInt16 mountPrecBeadsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref mountPrecBeadsCnt);
				mountPrecBeads = new RacePrecBead[mountPrecBeadsCnt];
				for(int i = 0; i < mountPrecBeads.Length; i++)
				{
					mountPrecBeads[i] = new RacePrecBead();
					mountPrecBeads[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_int8(buffer, ref pos_, ref magicCardLv);
				BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
				BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref enhanceNum);
				UInt16 inscriptionIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref inscriptionIdsCnt);
				inscriptionIds = new UInt32[inscriptionIdsCnt];
				for(int i = 0; i < inscriptionIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref inscriptionIds[i]);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref independAtk);
				BaseDLL.decode_uint32(buffer, ref pos_, ref indpendAtkStreng);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, pos);
				BaseDLL.encode_uint32(buffer, ref pos_, phyAtk);
				BaseDLL.encode_uint32(buffer, ref pos_, magAtk);
				BaseDLL.encode_uint32(buffer, ref pos_, phydef);
				BaseDLL.encode_uint32(buffer, ref pos_, magdef);
				BaseDLL.encode_uint32(buffer, ref pos_, strenth);
				BaseDLL.encode_uint32(buffer, ref pos_, stamina);
				BaseDLL.encode_uint32(buffer, ref pos_, intellect);
				BaseDLL.encode_uint32(buffer, ref pos_, spirit);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)properties.Length);
				for(int i = 0; i < properties.Length; i++)
				{
					properties[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, magicCard);
				BaseDLL.encode_uint32(buffer, ref pos_, disphyAtk);
				BaseDLL.encode_uint32(buffer, ref pos_, disMagAtk);
				BaseDLL.encode_uint32(buffer, ref pos_, disphydef);
				BaseDLL.encode_uint32(buffer, ref pos_, dismagdef);
				BaseDLL.encode_int8(buffer, ref pos_, strengthen);
				BaseDLL.encode_uint32(buffer, ref pos_, fashionAttrId);
				BaseDLL.encode_uint32(buffer, ref pos_, phyDefPercent);
				BaseDLL.encode_uint32(buffer, ref pos_, magDefPercent);
				BaseDLL.encode_uint32(buffer, ref pos_, preciousBeadId);
				BaseDLL.encode_int8(buffer, ref pos_, isTimeLimit);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)atkPropEx.Length);
				for(int i = 0; i < atkPropEx.Length; i++)
				{
					BaseDLL.encode_int32(buffer, ref pos_, atkPropEx[i]);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)strPropEx.Length);
				for(int i = 0; i < strPropEx.Length; i++)
				{
					BaseDLL.encode_int32(buffer, ref pos_, strPropEx[i]);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)defPropEx.Length);
				for(int i = 0; i < defPropEx.Length; i++)
				{
					BaseDLL.encode_int32(buffer, ref pos_, defPropEx[i]);
				}
				BaseDLL.encode_int32(buffer, ref pos_, abnormalResistsTotal);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)abnormalResists.Length);
				for(int i = 0; i < abnormalResists.Length; i++)
				{
					BaseDLL.encode_int32(buffer, ref pos_, abnormalResists[i]);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mountPrecBeads.Length);
				for(int i = 0; i < mountPrecBeads.Length; i++)
				{
					mountPrecBeads[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_int8(buffer, ref pos_, magicCardLv);
				BaseDLL.encode_int8(buffer, ref pos_, equipType);
				BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
				BaseDLL.encode_uint32(buffer, ref pos_, enhanceNum);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)inscriptionIds.Length);
				for(int i = 0; i < inscriptionIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, inscriptionIds[i]);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, independAtk);
				BaseDLL.encode_uint32(buffer, ref pos_, indpendAtkStreng);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref pos);
				BaseDLL.decode_uint32(buffer, ref pos_, ref phyAtk);
				BaseDLL.decode_uint32(buffer, ref pos_, ref magAtk);
				BaseDLL.decode_uint32(buffer, ref pos_, ref phydef);
				BaseDLL.decode_uint32(buffer, ref pos_, ref magdef);
				BaseDLL.decode_uint32(buffer, ref pos_, ref strenth);
				BaseDLL.decode_uint32(buffer, ref pos_, ref stamina);
				BaseDLL.decode_uint32(buffer, ref pos_, ref intellect);
				BaseDLL.decode_uint32(buffer, ref pos_, ref spirit);
				UInt16 propertiesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref propertiesCnt);
				properties = new RaceItemRandProperty[propertiesCnt];
				for(int i = 0; i < properties.Length; i++)
				{
					properties[i] = new RaceItemRandProperty();
					properties[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref magicCard);
				BaseDLL.decode_uint32(buffer, ref pos_, ref disphyAtk);
				BaseDLL.decode_uint32(buffer, ref pos_, ref disMagAtk);
				BaseDLL.decode_uint32(buffer, ref pos_, ref disphydef);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dismagdef);
				BaseDLL.decode_int8(buffer, ref pos_, ref strengthen);
				BaseDLL.decode_uint32(buffer, ref pos_, ref fashionAttrId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref phyDefPercent);
				BaseDLL.decode_uint32(buffer, ref pos_, ref magDefPercent);
				BaseDLL.decode_uint32(buffer, ref pos_, ref preciousBeadId);
				BaseDLL.decode_int8(buffer, ref pos_, ref isTimeLimit);
				UInt16 atkPropExCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref atkPropExCnt);
				atkPropEx = new Int32[atkPropExCnt];
				for(int i = 0; i < atkPropEx.Length; i++)
				{
					BaseDLL.decode_int32(buffer, ref pos_, ref atkPropEx[i]);
				}
				UInt16 strPropExCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref strPropExCnt);
				strPropEx = new Int32[strPropExCnt];
				for(int i = 0; i < strPropEx.Length; i++)
				{
					BaseDLL.decode_int32(buffer, ref pos_, ref strPropEx[i]);
				}
				UInt16 defPropExCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref defPropExCnt);
				defPropEx = new Int32[defPropExCnt];
				for(int i = 0; i < defPropEx.Length; i++)
				{
					BaseDLL.decode_int32(buffer, ref pos_, ref defPropEx[i]);
				}
				BaseDLL.decode_int32(buffer, ref pos_, ref abnormalResistsTotal);
				UInt16 abnormalResistsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref abnormalResistsCnt);
				abnormalResists = new Int32[abnormalResistsCnt];
				for(int i = 0; i < abnormalResists.Length; i++)
				{
					BaseDLL.decode_int32(buffer, ref pos_, ref abnormalResists[i]);
				}
				UInt16 mountPrecBeadsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref mountPrecBeadsCnt);
				mountPrecBeads = new RacePrecBead[mountPrecBeadsCnt];
				for(int i = 0; i < mountPrecBeads.Length; i++)
				{
					mountPrecBeads[i] = new RacePrecBead();
					mountPrecBeads[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_int8(buffer, ref pos_, ref magicCardLv);
				BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
				BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref enhanceNum);
				UInt16 inscriptionIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref inscriptionIdsCnt);
				inscriptionIds = new UInt32[inscriptionIdsCnt];
				for(int i = 0; i < inscriptionIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref inscriptionIds[i]);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref independAtk);
				BaseDLL.decode_uint32(buffer, ref pos_, ref indpendAtkStreng);
			}

			public int getLen()
			{
				int _len = 0;
				// uid
				_len += 8;
				// id
				_len += 4;
				// pos
				_len += 4;
				// phyAtk
				_len += 4;
				// magAtk
				_len += 4;
				// phydef
				_len += 4;
				// magdef
				_len += 4;
				// strenth
				_len += 4;
				// stamina
				_len += 4;
				// intellect
				_len += 4;
				// spirit
				_len += 4;
				// properties
				_len += 2;
				for(int j = 0; j < properties.Length; j++)
				{
					_len += properties[j].getLen();
				}
				// magicCard
				_len += 4;
				// disphyAtk
				_len += 4;
				// disMagAtk
				_len += 4;
				// disphydef
				_len += 4;
				// dismagdef
				_len += 4;
				// strengthen
				_len += 1;
				// fashionAttrId
				_len += 4;
				// phyDefPercent
				_len += 4;
				// magDefPercent
				_len += 4;
				// preciousBeadId
				_len += 4;
				// isTimeLimit
				_len += 1;
				// atkPropEx
				_len += 2 + 4 * atkPropEx.Length;
				// strPropEx
				_len += 2 + 4 * strPropEx.Length;
				// defPropEx
				_len += 2 + 4 * defPropEx.Length;
				// abnormalResistsTotal
				_len += 4;
				// abnormalResists
				_len += 2 + 4 * abnormalResists.Length;
				// mountPrecBeads
				_len += 2;
				for(int j = 0; j < mountPrecBeads.Length; j++)
				{
					_len += mountPrecBeads[j].getLen();
				}
				// magicCardLv
				_len += 1;
				// equipType
				_len += 1;
				// enhanceType
				_len += 1;
				// enhanceNum
				_len += 4;
				// inscriptionIds
				_len += 2 + 4 * inscriptionIds.Length;
				// independAtk
				_len += 4;
				// indpendAtkStreng
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 独立攻击力强化加成
	/// </summary>
	public class RaceItem : Protocol.IProtocolStream
	{
		public UInt32 id;
		public UInt16 num;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// num
				_len += 2;
				return _len;
			}
		#endregion

	}

	public class RaceBuffInfo : Protocol.IProtocolStream
	{
		public UInt32 id;
		public UInt32 overlayNums;
		public UInt64 startTime;
		public UInt32 duration;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, overlayNums);
				BaseDLL.encode_uint64(buffer, ref pos_, startTime);
				BaseDLL.encode_uint32(buffer, ref pos_, duration);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref overlayNums);
				BaseDLL.decode_uint64(buffer, ref pos_, ref startTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref duration);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, overlayNums);
				BaseDLL.encode_uint64(buffer, ref pos_, startTime);
				BaseDLL.encode_uint32(buffer, ref pos_, duration);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref overlayNums);
				BaseDLL.decode_uint64(buffer, ref pos_, ref startTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref duration);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// overlayNums
				_len += 4;
				// startTime
				_len += 8;
				// duration
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class RaceWarpStone : Protocol.IProtocolStream
	{
		public UInt32 id;
		public byte level;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, level);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, level);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// level
				_len += 1;
				return _len;
			}
		#endregion

	}

	public class RaceRetinue : Protocol.IProtocolStream
	{
		public UInt32 dataId;
		public byte level;
		public byte star;
		public byte isMain;
		public UInt32[] buffIds = new UInt32[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
				BaseDLL.encode_int8(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, star);
				BaseDLL.encode_int8(buffer, ref pos_, isMain);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)buffIds.Length);
				for(int i = 0; i < buffIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, buffIds[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
				BaseDLL.decode_int8(buffer, ref pos_, ref star);
				BaseDLL.decode_int8(buffer, ref pos_, ref isMain);
				UInt16 buffIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref buffIdsCnt);
				buffIds = new UInt32[buffIdsCnt];
				for(int i = 0; i < buffIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref buffIds[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
				BaseDLL.encode_int8(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, star);
				BaseDLL.encode_int8(buffer, ref pos_, isMain);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)buffIds.Length);
				for(int i = 0; i < buffIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, buffIds[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
				BaseDLL.decode_int8(buffer, ref pos_, ref star);
				BaseDLL.decode_int8(buffer, ref pos_, ref isMain);
				UInt16 buffIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref buffIdsCnt);
				buffIds = new UInt32[buffIdsCnt];
				for(int i = 0; i < buffIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref buffIds[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// dataId
				_len += 4;
				// level
				_len += 1;
				// star
				_len += 1;
				// isMain
				_len += 1;
				// buffIds
				_len += 2 + 4 * buffIds.Length;
				return _len;
			}
		#endregion

	}

	public class RacePet : Protocol.IProtocolStream
	{
		public UInt32 dataId;
		public UInt16 level;
		public UInt16 hunger;
		public byte skillIndex;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_uint16(buffer, ref pos_, hunger);
				BaseDLL.encode_int8(buffer, ref pos_, skillIndex);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_uint16(buffer, ref pos_, ref hunger);
				BaseDLL.decode_int8(buffer, ref pos_, ref skillIndex);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_uint16(buffer, ref pos_, hunger);
				BaseDLL.encode_int8(buffer, ref pos_, skillIndex);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_uint16(buffer, ref pos_, ref hunger);
				BaseDLL.decode_int8(buffer, ref pos_, ref skillIndex);
			}

			public int getLen()
			{
				int _len = 0;
				// dataId
				_len += 4;
				// level
				_len += 2;
				// hunger
				_len += 2;
				// skillIndex
				_len += 1;
				return _len;
			}
		#endregion

	}

	public class RaceEquipScheme : Protocol.IProtocolStream
	{
		public byte type;
		public UInt32 id;
		public byte isWear;
		public UInt64[] equips = new UInt64[0];
		public UInt64 title;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, isWear);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)equips.Length);
				for(int i = 0; i < equips.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, equips[i]);
				}
				BaseDLL.encode_uint64(buffer, ref pos_, title);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref isWear);
				UInt16 equipsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref equipsCnt);
				equips = new UInt64[equipsCnt];
				for(int i = 0; i < equips.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref equips[i]);
				}
				BaseDLL.decode_uint64(buffer, ref pos_, ref title);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, isWear);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)equips.Length);
				for(int i = 0; i < equips.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, equips[i]);
				}
				BaseDLL.encode_uint64(buffer, ref pos_, title);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref isWear);
				UInt16 equipsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref equipsCnt);
				equips = new UInt64[equipsCnt];
				for(int i = 0; i < equips.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref equips[i]);
				}
				BaseDLL.decode_uint64(buffer, ref pos_, ref title);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// id
				_len += 4;
				// isWear
				_len += 1;
				// equips
				_len += 2 + 8 * equips.Length;
				// title
				_len += 8;
				return _len;
			}
		#endregion

	}

	public class RacePlayerInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  服务器ID
		/// </summary>
		public UInt32 zoneId;
		/// <summary>
		///  服务器名
		/// </summary>
		public string serverName;
		/// <summary>
		///  ai难度，0代表无效值，说明不是机器人
		/// </summary>
		public byte robotAIType;
		/// <summary>
		///  机器人难度（0代表满血）
		/// </summary>
		public byte robotHard;
		public UInt64 roleId;
		public UInt32 accid;
		public string name;
		public string guildName;
		public byte occupation;
		public UInt16 level;
		public UInt32 pkValue;
		public UInt32 matchScore;
		public byte seat;
		public UInt32 remainHp;
		public UInt32 remainMp;
		public UInt32 seasonLevel;
		public UInt32 seasonStar;
		public byte seasonAttr;
		public byte monthcard;
		public RaceSkillInfo[] skills = new RaceSkillInfo[0];
		public RaceEquip[] equips = new RaceEquip[0];
		public RaceItem[] raceItems = new RaceItem[0];
		public RaceBuffInfo[] buffs = new RaceBuffInfo[0];
		public RaceWarpStone[] warpStones = new RaceWarpStone[0];
		public RaceRetinue[] retinues = new RaceRetinue[0];
		public RacePet[] pets = new RacePet[0];
		public UInt32[] potionPos = new UInt32[0];
		public RaceEquip[] secondWeapons = new RaceEquip[0];
		public PlayerAvatar avatar = new PlayerAvatar();
		/// <summary>
		/// 玩家标签信息
		/// </summary>
		public PlayerLabelInfo playerLabelInfo = new PlayerLabelInfo();
		/// <summary>
		/// 装备方案
		/// </summary>
		public RaceEquipScheme[] equipScheme = new RaceEquipScheme[0];
		/// <summary>
		/// 当前穿戴装备,时装id
		/// </summary>
		public UInt64[] wearingEqIds = new UInt64[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, zoneId);
				byte[] serverNameBytes = StringHelper.StringToUTF8Bytes(serverName);
				BaseDLL.encode_string(buffer, ref pos_, serverNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, robotAIType);
				BaseDLL.encode_int8(buffer, ref pos_, robotHard);
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint32(buffer, ref pos_, accid);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				byte[] guildNameBytes = StringHelper.StringToUTF8Bytes(guildName);
				BaseDLL.encode_string(buffer, ref pos_, guildNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occupation);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_uint32(buffer, ref pos_, pkValue);
				BaseDLL.encode_uint32(buffer, ref pos_, matchScore);
				BaseDLL.encode_int8(buffer, ref pos_, seat);
				BaseDLL.encode_uint32(buffer, ref pos_, remainHp);
				BaseDLL.encode_uint32(buffer, ref pos_, remainMp);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonStar);
				BaseDLL.encode_int8(buffer, ref pos_, seasonAttr);
				BaseDLL.encode_int8(buffer, ref pos_, monthcard);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)skills.Length);
				for(int i = 0; i < skills.Length; i++)
				{
					skills[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)equips.Length);
				for(int i = 0; i < equips.Length; i++)
				{
					equips[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)raceItems.Length);
				for(int i = 0; i < raceItems.Length; i++)
				{
					raceItems[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)buffs.Length);
				for(int i = 0; i < buffs.Length; i++)
				{
					buffs[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)warpStones.Length);
				for(int i = 0; i < warpStones.Length; i++)
				{
					warpStones[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)retinues.Length);
				for(int i = 0; i < retinues.Length; i++)
				{
					retinues[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)pets.Length);
				for(int i = 0; i < pets.Length; i++)
				{
					pets[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)potionPos.Length);
				for(int i = 0; i < potionPos.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, potionPos[i]);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)secondWeapons.Length);
				for(int i = 0; i < secondWeapons.Length; i++)
				{
					secondWeapons[i].encode(buffer, ref pos_);
				}
				avatar.encode(buffer, ref pos_);
				playerLabelInfo.encode(buffer, ref pos_);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)equipScheme.Length);
				for(int i = 0; i < equipScheme.Length; i++)
				{
					equipScheme[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)wearingEqIds.Length);
				for(int i = 0; i < wearingEqIds.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, wearingEqIds[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref zoneId);
				UInt16 serverNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref serverNameLen);
				byte[] serverNameBytes = new byte[serverNameLen];
				for(int i = 0; i < serverNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref serverNameBytes[i]);
				}
				serverName = StringHelper.BytesToString(serverNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref robotAIType);
				BaseDLL.decode_int8(buffer, ref pos_, ref robotHard);
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accid);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				UInt16 guildNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guildNameLen);
				byte[] guildNameBytes = new byte[guildNameLen];
				for(int i = 0; i < guildNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref guildNameBytes[i]);
				}
				guildName = StringHelper.BytesToString(guildNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref occupation);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_uint32(buffer, ref pos_, ref pkValue);
				BaseDLL.decode_uint32(buffer, ref pos_, ref matchScore);
				BaseDLL.decode_int8(buffer, ref pos_, ref seat);
				BaseDLL.decode_uint32(buffer, ref pos_, ref remainHp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref remainMp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonStar);
				BaseDLL.decode_int8(buffer, ref pos_, ref seasonAttr);
				BaseDLL.decode_int8(buffer, ref pos_, ref monthcard);
				UInt16 skillsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref skillsCnt);
				skills = new RaceSkillInfo[skillsCnt];
				for(int i = 0; i < skills.Length; i++)
				{
					skills[i] = new RaceSkillInfo();
					skills[i].decode(buffer, ref pos_);
				}
				UInt16 equipsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref equipsCnt);
				equips = new RaceEquip[equipsCnt];
				for(int i = 0; i < equips.Length; i++)
				{
					equips[i] = new RaceEquip();
					equips[i].decode(buffer, ref pos_);
				}
				UInt16 raceItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref raceItemsCnt);
				raceItems = new RaceItem[raceItemsCnt];
				for(int i = 0; i < raceItems.Length; i++)
				{
					raceItems[i] = new RaceItem();
					raceItems[i].decode(buffer, ref pos_);
				}
				UInt16 buffsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref buffsCnt);
				buffs = new RaceBuffInfo[buffsCnt];
				for(int i = 0; i < buffs.Length; i++)
				{
					buffs[i] = new RaceBuffInfo();
					buffs[i].decode(buffer, ref pos_);
				}
				UInt16 warpStonesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref warpStonesCnt);
				warpStones = new RaceWarpStone[warpStonesCnt];
				for(int i = 0; i < warpStones.Length; i++)
				{
					warpStones[i] = new RaceWarpStone();
					warpStones[i].decode(buffer, ref pos_);
				}
				UInt16 retinuesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref retinuesCnt);
				retinues = new RaceRetinue[retinuesCnt];
				for(int i = 0; i < retinues.Length; i++)
				{
					retinues[i] = new RaceRetinue();
					retinues[i].decode(buffer, ref pos_);
				}
				UInt16 petsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref petsCnt);
				pets = new RacePet[petsCnt];
				for(int i = 0; i < pets.Length; i++)
				{
					pets[i] = new RacePet();
					pets[i].decode(buffer, ref pos_);
				}
				UInt16 potionPosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref potionPosCnt);
				potionPos = new UInt32[potionPosCnt];
				for(int i = 0; i < potionPos.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref potionPos[i]);
				}
				UInt16 secondWeaponsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref secondWeaponsCnt);
				secondWeapons = new RaceEquip[secondWeaponsCnt];
				for(int i = 0; i < secondWeapons.Length; i++)
				{
					secondWeapons[i] = new RaceEquip();
					secondWeapons[i].decode(buffer, ref pos_);
				}
				avatar.decode(buffer, ref pos_);
				playerLabelInfo.decode(buffer, ref pos_);
				UInt16 equipSchemeCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref equipSchemeCnt);
				equipScheme = new RaceEquipScheme[equipSchemeCnt];
				for(int i = 0; i < equipScheme.Length; i++)
				{
					equipScheme[i] = new RaceEquipScheme();
					equipScheme[i].decode(buffer, ref pos_);
				}
				UInt16 wearingEqIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref wearingEqIdsCnt);
				wearingEqIds = new UInt64[wearingEqIdsCnt];
				for(int i = 0; i < wearingEqIds.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref wearingEqIds[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, zoneId);
				byte[] serverNameBytes = StringHelper.StringToUTF8Bytes(serverName);
				BaseDLL.encode_string(buffer, ref pos_, serverNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, robotAIType);
				BaseDLL.encode_int8(buffer, ref pos_, robotHard);
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint32(buffer, ref pos_, accid);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				byte[] guildNameBytes = StringHelper.StringToUTF8Bytes(guildName);
				BaseDLL.encode_string(buffer, ref pos_, guildNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occupation);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_uint32(buffer, ref pos_, pkValue);
				BaseDLL.encode_uint32(buffer, ref pos_, matchScore);
				BaseDLL.encode_int8(buffer, ref pos_, seat);
				BaseDLL.encode_uint32(buffer, ref pos_, remainHp);
				BaseDLL.encode_uint32(buffer, ref pos_, remainMp);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonStar);
				BaseDLL.encode_int8(buffer, ref pos_, seasonAttr);
				BaseDLL.encode_int8(buffer, ref pos_, monthcard);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)skills.Length);
				for(int i = 0; i < skills.Length; i++)
				{
					skills[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)equips.Length);
				for(int i = 0; i < equips.Length; i++)
				{
					equips[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)raceItems.Length);
				for(int i = 0; i < raceItems.Length; i++)
				{
					raceItems[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)buffs.Length);
				for(int i = 0; i < buffs.Length; i++)
				{
					buffs[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)warpStones.Length);
				for(int i = 0; i < warpStones.Length; i++)
				{
					warpStones[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)retinues.Length);
				for(int i = 0; i < retinues.Length; i++)
				{
					retinues[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)pets.Length);
				for(int i = 0; i < pets.Length; i++)
				{
					pets[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)potionPos.Length);
				for(int i = 0; i < potionPos.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, potionPos[i]);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)secondWeapons.Length);
				for(int i = 0; i < secondWeapons.Length; i++)
				{
					secondWeapons[i].encode(buffer, ref pos_);
				}
				avatar.encode(buffer, ref pos_);
				playerLabelInfo.encode(buffer, ref pos_);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)equipScheme.Length);
				for(int i = 0; i < equipScheme.Length; i++)
				{
					equipScheme[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)wearingEqIds.Length);
				for(int i = 0; i < wearingEqIds.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, wearingEqIds[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref zoneId);
				UInt16 serverNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref serverNameLen);
				byte[] serverNameBytes = new byte[serverNameLen];
				for(int i = 0; i < serverNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref serverNameBytes[i]);
				}
				serverName = StringHelper.BytesToString(serverNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref robotAIType);
				BaseDLL.decode_int8(buffer, ref pos_, ref robotHard);
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accid);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				UInt16 guildNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guildNameLen);
				byte[] guildNameBytes = new byte[guildNameLen];
				for(int i = 0; i < guildNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref guildNameBytes[i]);
				}
				guildName = StringHelper.BytesToString(guildNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref occupation);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_uint32(buffer, ref pos_, ref pkValue);
				BaseDLL.decode_uint32(buffer, ref pos_, ref matchScore);
				BaseDLL.decode_int8(buffer, ref pos_, ref seat);
				BaseDLL.decode_uint32(buffer, ref pos_, ref remainHp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref remainMp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonStar);
				BaseDLL.decode_int8(buffer, ref pos_, ref seasonAttr);
				BaseDLL.decode_int8(buffer, ref pos_, ref monthcard);
				UInt16 skillsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref skillsCnt);
				skills = new RaceSkillInfo[skillsCnt];
				for(int i = 0; i < skills.Length; i++)
				{
					skills[i] = new RaceSkillInfo();
					skills[i].decode(buffer, ref pos_);
				}
				UInt16 equipsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref equipsCnt);
				equips = new RaceEquip[equipsCnt];
				for(int i = 0; i < equips.Length; i++)
				{
					equips[i] = new RaceEquip();
					equips[i].decode(buffer, ref pos_);
				}
				UInt16 raceItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref raceItemsCnt);
				raceItems = new RaceItem[raceItemsCnt];
				for(int i = 0; i < raceItems.Length; i++)
				{
					raceItems[i] = new RaceItem();
					raceItems[i].decode(buffer, ref pos_);
				}
				UInt16 buffsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref buffsCnt);
				buffs = new RaceBuffInfo[buffsCnt];
				for(int i = 0; i < buffs.Length; i++)
				{
					buffs[i] = new RaceBuffInfo();
					buffs[i].decode(buffer, ref pos_);
				}
				UInt16 warpStonesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref warpStonesCnt);
				warpStones = new RaceWarpStone[warpStonesCnt];
				for(int i = 0; i < warpStones.Length; i++)
				{
					warpStones[i] = new RaceWarpStone();
					warpStones[i].decode(buffer, ref pos_);
				}
				UInt16 retinuesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref retinuesCnt);
				retinues = new RaceRetinue[retinuesCnt];
				for(int i = 0; i < retinues.Length; i++)
				{
					retinues[i] = new RaceRetinue();
					retinues[i].decode(buffer, ref pos_);
				}
				UInt16 petsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref petsCnt);
				pets = new RacePet[petsCnt];
				for(int i = 0; i < pets.Length; i++)
				{
					pets[i] = new RacePet();
					pets[i].decode(buffer, ref pos_);
				}
				UInt16 potionPosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref potionPosCnt);
				potionPos = new UInt32[potionPosCnt];
				for(int i = 0; i < potionPos.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref potionPos[i]);
				}
				UInt16 secondWeaponsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref secondWeaponsCnt);
				secondWeapons = new RaceEquip[secondWeaponsCnt];
				for(int i = 0; i < secondWeapons.Length; i++)
				{
					secondWeapons[i] = new RaceEquip();
					secondWeapons[i].decode(buffer, ref pos_);
				}
				avatar.decode(buffer, ref pos_);
				playerLabelInfo.decode(buffer, ref pos_);
				UInt16 equipSchemeCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref equipSchemeCnt);
				equipScheme = new RaceEquipScheme[equipSchemeCnt];
				for(int i = 0; i < equipScheme.Length; i++)
				{
					equipScheme[i] = new RaceEquipScheme();
					equipScheme[i].decode(buffer, ref pos_);
				}
				UInt16 wearingEqIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref wearingEqIdsCnt);
				wearingEqIds = new UInt64[wearingEqIdsCnt];
				for(int i = 0; i < wearingEqIds.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref wearingEqIds[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// zoneId
				_len += 4;
				// serverName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(serverName);
					_len += 2 + _strBytes.Length;
				}
				// robotAIType
				_len += 1;
				// robotHard
				_len += 1;
				// roleId
				_len += 8;
				// accid
				_len += 4;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// guildName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(guildName);
					_len += 2 + _strBytes.Length;
				}
				// occupation
				_len += 1;
				// level
				_len += 2;
				// pkValue
				_len += 4;
				// matchScore
				_len += 4;
				// seat
				_len += 1;
				// remainHp
				_len += 4;
				// remainMp
				_len += 4;
				// seasonLevel
				_len += 4;
				// seasonStar
				_len += 4;
				// seasonAttr
				_len += 1;
				// monthcard
				_len += 1;
				// skills
				_len += 2;
				for(int j = 0; j < skills.Length; j++)
				{
					_len += skills[j].getLen();
				}
				// equips
				_len += 2;
				for(int j = 0; j < equips.Length; j++)
				{
					_len += equips[j].getLen();
				}
				// raceItems
				_len += 2;
				for(int j = 0; j < raceItems.Length; j++)
				{
					_len += raceItems[j].getLen();
				}
				// buffs
				_len += 2;
				for(int j = 0; j < buffs.Length; j++)
				{
					_len += buffs[j].getLen();
				}
				// warpStones
				_len += 2;
				for(int j = 0; j < warpStones.Length; j++)
				{
					_len += warpStones[j].getLen();
				}
				// retinues
				_len += 2;
				for(int j = 0; j < retinues.Length; j++)
				{
					_len += retinues[j].getLen();
				}
				// pets
				_len += 2;
				for(int j = 0; j < pets.Length; j++)
				{
					_len += pets[j].getLen();
				}
				// potionPos
				_len += 2 + 4 * potionPos.Length;
				// secondWeapons
				_len += 2;
				for(int j = 0; j < secondWeapons.Length; j++)
				{
					_len += secondWeapons[j].getLen();
				}
				// avatar
				_len += avatar.getLen();
				// playerLabelInfo
				_len += playerLabelInfo.getLen();
				// equipScheme
				_len += 2;
				for(int j = 0; j < equipScheme.Length; j++)
				{
					_len += equipScheme[j].getLen();
				}
				// wearingEqIds
				_len += 2 + 8 * wearingEqIds.Length;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldNotifyRaceStart : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 606701;
		public UInt32 Sequence;
		public UInt64 roleId;
		public UInt64 sessionId;
		public SockAddr addr = new SockAddr();
		/// <summary>
		///  对应枚举（RaceType）
		/// </summary>
		public byte raceType;
		public UInt32 dungeonId;
		public RacePlayerInfo[] players = new RacePlayerInfo[0];
		/// <summary>
		///  是否记录日志
		/// </summary>
		public byte isRecordLog;
		/// <summary>
		///  是否记录录像
		/// </summary>
		public byte isRecordReplay;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint64(buffer, ref pos_, sessionId);
				addr.encode(buffer, ref pos_);
				BaseDLL.encode_int8(buffer, ref pos_, raceType);
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)players.Length);
				for(int i = 0; i < players.Length; i++)
				{
					players[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_int8(buffer, ref pos_, isRecordLog);
				BaseDLL.encode_int8(buffer, ref pos_, isRecordReplay);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref sessionId);
				addr.decode(buffer, ref pos_);
				BaseDLL.decode_int8(buffer, ref pos_, ref raceType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
				UInt16 playersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playersCnt);
				players = new RacePlayerInfo[playersCnt];
				for(int i = 0; i < players.Length; i++)
				{
					players[i] = new RacePlayerInfo();
					players[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_int8(buffer, ref pos_, ref isRecordLog);
				BaseDLL.decode_int8(buffer, ref pos_, ref isRecordReplay);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint64(buffer, ref pos_, sessionId);
				addr.encode(buffer, ref pos_);
				BaseDLL.encode_int8(buffer, ref pos_, raceType);
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)players.Length);
				for(int i = 0; i < players.Length; i++)
				{
					players[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_int8(buffer, ref pos_, isRecordLog);
				BaseDLL.encode_int8(buffer, ref pos_, isRecordReplay);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref sessionId);
				addr.decode(buffer, ref pos_);
				BaseDLL.decode_int8(buffer, ref pos_, ref raceType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
				UInt16 playersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playersCnt);
				players = new RacePlayerInfo[playersCnt];
				for(int i = 0; i < players.Length; i++)
				{
					players[i] = new RacePlayerInfo();
					players[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_int8(buffer, ref pos_, ref isRecordLog);
				BaseDLL.decode_int8(buffer, ref pos_, ref isRecordReplay);
			}

			public int getLen()
			{
				int _len = 0;
				// roleId
				_len += 8;
				// sessionId
				_len += 8;
				// addr
				_len += addr.getLen();
				// raceType
				_len += 1;
				// dungeonId
				_len += 4;
				// players
				_len += 2;
				for(int j = 0; j < players.Length; j++)
				{
					_len += players[j].getLen();
				}
				// isRecordLog
				_len += 1;
				// isRecordReplay
				_len += 1;
				return _len;
			}
		#endregion

	}

	public class PkOccuRecord : Protocol.IProtocolStream
	{
		public byte occu;
		public UInt32 winNum;
		public UInt32 loseNum;
		public UInt32 totalNum;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_uint32(buffer, ref pos_, winNum);
				BaseDLL.encode_uint32(buffer, ref pos_, loseNum);
				BaseDLL.encode_uint32(buffer, ref pos_, totalNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_uint32(buffer, ref pos_, ref winNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref loseNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_uint32(buffer, ref pos_, winNum);
				BaseDLL.encode_uint32(buffer, ref pos_, loseNum);
				BaseDLL.encode_uint32(buffer, ref pos_, totalNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_uint32(buffer, ref pos_, ref winNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref loseNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalNum);
			}

			public int getLen()
			{
				int _len = 0;
				// occu
				_len += 1;
				// winNum
				_len += 4;
				// loseNum
				_len += 4;
				// totalNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSyncPkStatisticInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506703;
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
	public class SceneSyncPkStatisticProperty : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506704;
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
	///  结算
	/// </summary>
	[Protocol]
	public class SceneMatchPkRaceEnd : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506705;
		public UInt32 Sequence;
		/// <summary>
		///  PK类型，对应枚举(PkType)
		/// </summary>
		public byte pkType;
		public byte result;
		public UInt32 oldPkValue;
		public UInt32 newPkValue;
		public UInt32 oldMatchScore;
		public UInt32 newMatchScore;
		/// <summary>
		///  初始决斗币数量
		/// </summary>
		public UInt32 oldPkCoin;
		/// <summary>
		///  战斗获得的决斗币
		/// </summary>
		public UInt32 addPkCoinFromRace;
		/// <summary>
		///  今日战斗获得的全部决斗币
		/// </summary>
		public UInt32 totalPkCoinFromRace;
		/// <summary>
		///  是否在PVP活动期间
		/// </summary>
		public byte isInPvPActivity;
		/// <summary>
		///  活动额外获得的决斗币
		/// </summary>
		public UInt32 addPkCoinFromActivity;
		/// <summary>
		///  今日活动获得的全部决斗币
		/// </summary>
		public UInt32 totalPkCoinFromActivity;
		/// <summary>
		///  原段位
		/// </summary>
		public UInt32 oldSeasonLevel;
		/// <summary>
		///  现段位
		/// </summary>
		public UInt32 newSeasonLevel;
		/// <summary>
		///  原星
		/// </summary>
		public UInt32 oldSeasonStar;
		/// <summary>
		///  现星
		/// </summary>
		public UInt32 newSeasonStar;
		/// <summary>
		///  原经验
		/// </summary>
		public UInt32 oldSeasonExp;
		/// <summary>
		///  现经验
		/// </summary>
		public UInt32 newSeasonExp;
		/// <summary>
		///  改变的经验
		/// </summary>
		public Int32 changeSeasonExp;
		/// <summary>
		///  获得荣誉
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
				BaseDLL.encode_int8(buffer, ref pos_, pkType);
				BaseDLL.encode_int8(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, oldPkValue);
				BaseDLL.encode_uint32(buffer, ref pos_, newPkValue);
				BaseDLL.encode_uint32(buffer, ref pos_, oldMatchScore);
				BaseDLL.encode_uint32(buffer, ref pos_, newMatchScore);
				BaseDLL.encode_uint32(buffer, ref pos_, oldPkCoin);
				BaseDLL.encode_uint32(buffer, ref pos_, addPkCoinFromRace);
				BaseDLL.encode_uint32(buffer, ref pos_, totalPkCoinFromRace);
				BaseDLL.encode_int8(buffer, ref pos_, isInPvPActivity);
				BaseDLL.encode_uint32(buffer, ref pos_, addPkCoinFromActivity);
				BaseDLL.encode_uint32(buffer, ref pos_, totalPkCoinFromActivity);
				BaseDLL.encode_uint32(buffer, ref pos_, oldSeasonLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, newSeasonLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, oldSeasonStar);
				BaseDLL.encode_uint32(buffer, ref pos_, newSeasonStar);
				BaseDLL.encode_uint32(buffer, ref pos_, oldSeasonExp);
				BaseDLL.encode_uint32(buffer, ref pos_, newSeasonExp);
				BaseDLL.encode_int32(buffer, ref pos_, changeSeasonExp);
				BaseDLL.encode_uint32(buffer, ref pos_, getHonor);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref pkType);
				BaseDLL.decode_int8(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldPkValue);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newPkValue);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldMatchScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newMatchScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldPkCoin);
				BaseDLL.decode_uint32(buffer, ref pos_, ref addPkCoinFromRace);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalPkCoinFromRace);
				BaseDLL.decode_int8(buffer, ref pos_, ref isInPvPActivity);
				BaseDLL.decode_uint32(buffer, ref pos_, ref addPkCoinFromActivity);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalPkCoinFromActivity);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldSeasonLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newSeasonLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldSeasonStar);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newSeasonStar);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldSeasonExp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newSeasonExp);
				BaseDLL.decode_int32(buffer, ref pos_, ref changeSeasonExp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref getHonor);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, pkType);
				BaseDLL.encode_int8(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, oldPkValue);
				BaseDLL.encode_uint32(buffer, ref pos_, newPkValue);
				BaseDLL.encode_uint32(buffer, ref pos_, oldMatchScore);
				BaseDLL.encode_uint32(buffer, ref pos_, newMatchScore);
				BaseDLL.encode_uint32(buffer, ref pos_, oldPkCoin);
				BaseDLL.encode_uint32(buffer, ref pos_, addPkCoinFromRace);
				BaseDLL.encode_uint32(buffer, ref pos_, totalPkCoinFromRace);
				BaseDLL.encode_int8(buffer, ref pos_, isInPvPActivity);
				BaseDLL.encode_uint32(buffer, ref pos_, addPkCoinFromActivity);
				BaseDLL.encode_uint32(buffer, ref pos_, totalPkCoinFromActivity);
				BaseDLL.encode_uint32(buffer, ref pos_, oldSeasonLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, newSeasonLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, oldSeasonStar);
				BaseDLL.encode_uint32(buffer, ref pos_, newSeasonStar);
				BaseDLL.encode_uint32(buffer, ref pos_, oldSeasonExp);
				BaseDLL.encode_uint32(buffer, ref pos_, newSeasonExp);
				BaseDLL.encode_int32(buffer, ref pos_, changeSeasonExp);
				BaseDLL.encode_uint32(buffer, ref pos_, getHonor);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref pkType);
				BaseDLL.decode_int8(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldPkValue);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newPkValue);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldMatchScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newMatchScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldPkCoin);
				BaseDLL.decode_uint32(buffer, ref pos_, ref addPkCoinFromRace);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalPkCoinFromRace);
				BaseDLL.decode_int8(buffer, ref pos_, ref isInPvPActivity);
				BaseDLL.decode_uint32(buffer, ref pos_, ref addPkCoinFromActivity);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalPkCoinFromActivity);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldSeasonLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newSeasonLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldSeasonStar);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newSeasonStar);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldSeasonExp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newSeasonExp);
				BaseDLL.decode_int32(buffer, ref pos_, ref changeSeasonExp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref getHonor);
			}

			public int getLen()
			{
				int _len = 0;
				// pkType
				_len += 1;
				// result
				_len += 1;
				// oldPkValue
				_len += 4;
				// newPkValue
				_len += 4;
				// oldMatchScore
				_len += 4;
				// newMatchScore
				_len += 4;
				// oldPkCoin
				_len += 4;
				// addPkCoinFromRace
				_len += 4;
				// totalPkCoinFromRace
				_len += 4;
				// isInPvPActivity
				_len += 1;
				// addPkCoinFromActivity
				_len += 4;
				// totalPkCoinFromActivity
				_len += 4;
				// oldSeasonLevel
				_len += 4;
				// newSeasonLevel
				_len += 4;
				// oldSeasonStar
				_len += 4;
				// newSeasonStar
				_len += 4;
				// oldSeasonExp
				_len += 4;
				// newSeasonExp
				_len += 4;
				// changeSeasonExp
				_len += 4;
				// getHonor
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求参加武道大会
	/// </summary>
	[Protocol]
	public class SceneWudaoJoinReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506706;
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
	///  参加武道大会返回
	/// </summary>
	[Protocol]
	public class SceneWudaoJoinRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506707;
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
	///  请求领取武道大会奖励
	/// </summary>
	[Protocol]
	public class SceneWudaoRewardReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506708;
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
	///  领取武道大会奖励返回
	/// </summary>
	[Protocol]
	public class SceneWudaoRewardRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506709;
		public UInt32 Sequence;
		public UInt32 result;
		public ItemReward[] getItems = new ItemReward[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)getItems.Length);
				for(int i = 0; i < getItems.Length; i++)
				{
					getItems[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 getItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref getItemsCnt);
				getItems = new ItemReward[getItemsCnt];
				for(int i = 0; i < getItems.Length; i++)
				{
					getItems[i] = new ItemReward();
					getItems[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)getItems.Length);
				for(int i = 0; i < getItems.Length; i++)
				{
					getItems[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 getItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref getItemsCnt);
				getItems = new ItemReward[getItemsCnt];
				for(int i = 0; i < getItems.Length; i++)
				{
					getItems[i] = new ItemReward();
					getItems[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// getItems
				_len += 2;
				for(int j = 0; j < getItems.Length; j++)
				{
					_len += getItems[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  好友状态信息
	/// </summary>
	public class FriendMatchStatusInfo : Protocol.IProtocolStream
	{
		public UInt64 roleId;
		/// <summary>
		///  状态，对应枚举（FriendMatchStatus）
		/// </summary>
		public byte status;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_int8(buffer, ref pos_, status);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_int8(buffer, ref pos_, status);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
			}

			public int getLen()
			{
				int _len = 0;
				// roleId
				_len += 8;
				// status
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求查询好友状态
	/// </summary>
	[Protocol]
	public class WorldMatchQueryFriendStatusReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 606706;
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
	///  查询好友状态返回
	/// </summary>
	[Protocol]
	public class WorldMatchQueryFriendStatusRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 606707;
		public UInt32 Sequence;
		public FriendMatchStatusInfo[] infoes = new FriendMatchStatusInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				infoes = new FriendMatchStatusInfo[infoesCnt];
				for(int i = 0; i < infoes.Length; i++)
				{
					infoes[i] = new FriendMatchStatusInfo();
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
				infoes = new FriendMatchStatusInfo[infoesCnt];
				for(int i = 0; i < infoes.Length; i++)
				{
					infoes[i] = new FriendMatchStatusInfo();
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
	/// 通知段位信息
	/// </summary>
	[Protocol]
	public class SceneSyncSeasonLevel : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506711;
		public UInt32 Sequence;
		public UInt32 oldSeasonLevel;
		public UInt32 oldSeasonStar;
		public UInt32 seasonLevel;
		public UInt32 seasonStar;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, oldSeasonLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, oldSeasonStar);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonStar);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldSeasonLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldSeasonStar);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonStar);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, oldSeasonLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, oldSeasonStar);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonStar);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldSeasonLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldSeasonStar);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonStar);
			}

			public int getLen()
			{
				int _len = 0;
				// oldSeasonLevel
				_len += 4;
				// oldSeasonStar
				_len += 4;
				// seasonLevel
				_len += 4;
				// seasonStar
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 客户端通知赛季播放状态
	/// </summary>
	[Protocol]
	public class SceneSeasonPlayStatus : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506712;
		public UInt32 Sequence;
		public UInt32 seasonId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, seasonId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, seasonId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonId);
			}

			public int getLen()
			{
				int _len = 0;
				// seasonId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 通知客户端赛季信息
	/// </summary>
	[Protocol]
	public class SceneSyncSeasonInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506713;
		public UInt32 Sequence;
		public UInt32 seasonId;
		public UInt32 endTime;
		public UInt32 seasonAttrEndTime;
		public UInt32 seasonAttrLevel;
		public byte seasonAttr;
		public UInt32 seasonLevel;
		public UInt32 seasonStar;
		public UInt32 seasonExp;
		public byte seasonStatus;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, seasonId);
				BaseDLL.encode_uint32(buffer, ref pos_, endTime);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonAttrEndTime);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonAttrLevel);
				BaseDLL.encode_int8(buffer, ref pos_, seasonAttr);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonStar);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonExp);
				BaseDLL.encode_int8(buffer, ref pos_, seasonStatus);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonAttrEndTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonAttrLevel);
				BaseDLL.decode_int8(buffer, ref pos_, ref seasonAttr);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonStar);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonExp);
				BaseDLL.decode_int8(buffer, ref pos_, ref seasonStatus);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, seasonId);
				BaseDLL.encode_uint32(buffer, ref pos_, endTime);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonAttrEndTime);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonAttrLevel);
				BaseDLL.encode_int8(buffer, ref pos_, seasonAttr);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonStar);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonExp);
				BaseDLL.encode_int8(buffer, ref pos_, seasonStatus);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonAttrEndTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonAttrLevel);
				BaseDLL.decode_int8(buffer, ref pos_, ref seasonAttr);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonStar);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonExp);
				BaseDLL.decode_int8(buffer, ref pos_, ref seasonStatus);
			}

			public int getLen()
			{
				int _len = 0;
				// seasonId
				_len += 4;
				// endTime
				_len += 4;
				// seasonAttrEndTime
				_len += 4;
				// seasonAttrLevel
				_len += 4;
				// seasonAttr
				_len += 1;
				// seasonLevel
				_len += 4;
				// seasonStar
				_len += 4;
				// seasonExp
				_len += 4;
				// seasonStatus
				_len += 1;
				return _len;
			}
		#endregion

	}

}
