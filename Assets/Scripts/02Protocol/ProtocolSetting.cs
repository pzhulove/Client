using System;
using System.Text;

namespace Protocol
{
	public enum ServiceType
	{
		/// <summary>
		///  无效值
		/// </summary>
		SERVICE_INVALID = 0,
		/// <summary>
		///  赛季匹配
		/// </summary>
		SERVICE_1V1_SEASON = 1,
		/// <summary>
		///  工会战报名
		/// </summary>
		SERVICE_GUILD_BATTLE_ENROLL = 2,
		/// <summary>
		///  工会战
		/// </summary>
		SERVICE_GUILD_BATTLE = 3,
		/// <summary>
		///  自动战斗
		/// </summary>
		SERVICE_AUTO_BATTLE = 4,
		/// <summary>
		///  场景等级限制
		/// </summary>
		SERVICE_SCENE_LEVEL_LIMIT = 5,
		/// <summary>
		///  装备品级调整
		/// </summary>
		EQUIP_SET_QUALITY_LV = 6,
		/// <summary>
		///  时装选属性
		/// </summary>
		FASHION_SEL_ATTR = 7,
		/// <summary>
		///  金罐积分
		/// </summary>
		GOLD_JAR_POINT = 8,
		/// <summary>
		///  魔罐积分
		/// </summary>
		MAGIC_JAR_POINT = 9,
		/// <summary>
		///  圆桌会议
		/// </summary>
		SERVICE_GUILD_TABLE = 20,
		/// <summary>
		///  公会技能
		/// </summary>
		SERVICE_GUILD_SKILL = 21,
		/// <summary>
		///  公会捐赠
		/// </summary>
		SERVICE_GUILD_DONATE = 22,
		/// <summary>
		///  公会膜拜
		/// </summary>
		SERVICE_GUILD_ORZ = 23,
		/// <summary>
		///  公会红包
		/// </summary>
		SERVICE_GUILD_RED_PACKET = 24,
		/// <summary>
		///  跨服公会战
		/// </summary>
		SERVICE_GUILD_CROSS_BATTLE = 25,
		/// <summary>
		/// SDK小米切支付开关
		/// </summary>
		SERVICE_SDK_XIAOMI_CHANGE_CHARGE = 30,
		/// <summary>
		/// 帐号转移公告截图功能开关
		/// </summary>
		SERVICE_CONVERT_ACC_SCREENSHOT = 31,
		/// <summary>
		/// 货币重置快捷提示客户端检查开关
		/// </summary>
		SERVICE_CURRENCY_DEADLINE_CHECK = 33,
		/// <summary>
		///  随从系统
		/// </summary>
		SERVICE_RETINUE = 40,
		/// <summary>
		///  次元石系统
		/// </summary>
		SERVICE_WARP_STONE = 60,
		/// <summary>
		///  房间
		/// </summary>
		SERVICE_ROOM = 70,
		/// <summary>
		///  语音
		/// </summary>
		SERVICE_VOICE_NORMAL = 80,
		/// <summary>
		///  实时语音
		/// </summary>
		SERVICE_VOICE_REAL_TIME = 81,
		/// <summary>
		///  vip认证
		/// </summary>
		SERVICE_VIP_AUTH = 82,
		/// <summary>
		///  在线客服
		/// </summary>
		SERVICE_ONLINE_SERVICE = 83,
		/// <summary>
		///  每日充值福利
		/// </summary>
		SERVEICE_DAY_CHARGE_WELFARE = 91,
		/// <summary>
		/// 追帧模式开启
		/// </summary>
		SERVICE_CHASING_MODE = 92,
		/// <summary>
		///  随机宝箱
		/// </summary>
		SERVICE_DIG = 100,
		/// <summary>
		///  安全锁
		/// </summary>
		SERVICE_SECURITY_LOCK = 111,
		/// <summary>
		///  3v3乱斗
		/// </summary>
		SERVICE_3v3_TUMBLE = 112,
		/// <summary>
		///  OPPO社区
		/// </summary>
		SERVICE_OPPO_COMMUNITY = 120,
		/// <summary>
		///  VIVO社区
		/// </summary>
		SERVICE_VIVO_COMMUNITY = 121,
		/// <summary>
		///  强化券
		/// </summary>
		SERVICE_STRENGTHEN_TICKET_MERGE = 130,
		/// <summary>
		/// 公会副本
		/// </summary>
		SERVICE_GUILD_DUNGEON = 145,
		/// <summary>
		/// 时装合成
		/// </summary>
		SERVICE_FASHION_MERGO = 150,
		/// <summary>
		///  拍卖行珍品开关
		/// </summary>
		SERVICE_AUCTION_TREAS = 210,
		/// <summary>
		///  拍卖行翻页开关
		/// </summary>
		SERVICE_AUCTION_PAGE = 211,
		/// <summary>
		///  拍卖行交易冷却时间开关
		/// </summary>
		SERVICE_AUCTION_COOLTIME = 212,
		/// <summary>
		/// 冒险队(佣兵团)
		/// </summary>
		SERVICE_ADVENTURE_TEAM = 215,
		/// <summary>
		/// 新回归活动
		/// </summary>
		SERVICE_NEW_RETURN = 216,
		/// <summary>
		/// 冒险队(佣兵团)排行榜
		/// </summary>
		SERVICE_ADVENTURE_TEAM_SORTLIST = 217,
		/// <summary>
		/// 周常深渊失败门票返还
		/// </summary>
		SERVICE_WEEK_HELL_FAIL_RETURN_TICKETS = 221,
		/// <summary>
		/// 公会副本奖励截图
		/// </summary>
		SERVICE_GUILD_DUNGEON_SCREEN_SHOT = 224,
		/// <summary>
		/// 单局结算ID判断
		/// </summary>
		SERVICE_RACE_ID_CHECK = 225,
		/// <summary>
		/// 每日必做
		/// </summary>
		SERVICE_DAILY_TODO = 226,
		/// <summary>
		/// 当删除账号玩家时检查玩家ID
		/// </summary>
		SERVICE_CHECK_ROLEID_WHEN_REMOVE_ACCOUNT_PLAYER = 227,
		/// <summary>
		/// 装备强化保护开关
		/// </summary>
		SERVICE_EQUIP_STRENGTHEN_PROTECT = 228,
		/// <summary>
		/// 装备强化装备分解失败移除装备开关
		/// </summary>
		SERVICE_EQUIP_STRENG_DESC_FAIL_REMOVE = 229,
		/// <summary>
		/// 辅助装备开关
		/// </summary>
		SERVICE_ASSIST_EQUIP = 230,
		/// <summary>
		///  公会合并开关
		/// </summary>
		SERVICR_GUILDMERGER = 231,
		/// <summary>
		///  终极试炼爬塔
		/// </summary>
		SERVICE_ZJSL_TOWER = 232,
		/// <summary>
		/// 团本
		/// </summary>
		SERVICE_TEAM_COPY = 233,
		/// <summary>
		///  飞升药接受活动同步开关
		/// </summary>
		SERVICE_FLY_UP = 234,
		/// <summary>
		///  日志埋点通过游戏服发送
		/// </summary>
		SERVICE_NEW_CLIENT_LOG = 235,
		/// <summary>
		///  新的结算面板
		/// </summary>
		SERVICE_NEW_RACE_END_EXP = 236,
		/// <summary>
		/// 技能页开关
		/// </summary>
		SERVICE_SKILL_PAGE = 237,
		/// <summary>
		/// 装备方案开关
		/// </summary>
		SERVICE_EQUIP_SCHEME = 238,
	}

	public enum GameSetType
	{
		/// <summary>
		///  好友邀请
		/// </summary>
		GST_FRIEND_INVATE = 1,
		/// <summary>
		///  隐私设置
		/// </summary>
		GST_SECRET = 2,
	}

	public enum SecretSetType
	{
		/// <summary>
		///  公会邀请
		/// </summary>
		SST_GUILD_INVATE = 1,
		/// <summary>
		///  组队邀请
		/// </summary>
		SST_TEAM_INVATE = 2,
		/// <summary>
		///  决斗邀请
		/// </summary>
		SST_DUEL_INVATE = 4,
	}

	/// <summary>
	///  保存选项
	/// </summary>
	public enum SaveOptionMask
	{
		/// <summary>
		///  是否不消耗精力(精英地下城)
		/// </summary>
		SOM_NOT_COUSUME_EBERGY = 1,
	}

	public class SkillBarGrid : Protocol.IProtocolStream
	{
		public byte slot;
		public UInt16 id;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, slot);
				BaseDLL.encode_uint16(buffer, ref pos_, id);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref slot);
				BaseDLL.decode_uint16(buffer, ref pos_, ref id);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, slot);
				BaseDLL.encode_uint16(buffer, ref pos_, id);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref slot);
				BaseDLL.decode_uint16(buffer, ref pos_, ref id);
			}

			public int getLen()
			{
				int _len = 0;
				// slot
				_len += 1;
				// id
				_len += 2;
				return _len;
			}
		#endregion

	}

	public class SkillBar : Protocol.IProtocolStream
	{
		public byte index;
		public SkillBarGrid[] grids = new SkillBarGrid[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, index);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)grids.Length);
				for(int i = 0; i < grids.Length; i++)
				{
					grids[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref index);
				UInt16 gridsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref gridsCnt);
				grids = new SkillBarGrid[gridsCnt];
				for(int i = 0; i < grids.Length; i++)
				{
					grids[i] = new SkillBarGrid();
					grids[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, index);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)grids.Length);
				for(int i = 0; i < grids.Length; i++)
				{
					grids[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref index);
				UInt16 gridsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref gridsCnt);
				grids = new SkillBarGrid[gridsCnt];
				for(int i = 0; i < grids.Length; i++)
				{
					grids[i] = new SkillBarGrid();
					grids[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// index
				_len += 1;
				// grids
				_len += 2;
				for(int j = 0; j < grids.Length; j++)
				{
					_len += grids[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	public class SkillBars : Protocol.IProtocolStream
	{
		public byte index;
		public SkillBar[] bar = new SkillBar[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, index);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)bar.Length);
				for(int i = 0; i < bar.Length; i++)
				{
					bar[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref index);
				UInt16 barCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref barCnt);
				bar = new SkillBar[barCnt];
				for(int i = 0; i < bar.Length; i++)
				{
					bar[i] = new SkillBar();
					bar[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, index);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)bar.Length);
				for(int i = 0; i < bar.Length; i++)
				{
					bar[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref index);
				UInt16 barCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref barCnt);
				bar = new SkillBar[barCnt];
				for(int i = 0; i < bar.Length; i++)
				{
					bar[i] = new SkillBar();
					bar[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// index
				_len += 1;
				// bar
				_len += 2;
				for(int j = 0; j < bar.Length; j++)
				{
					_len += bar[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneExchangeSkillBarReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501201;
		public UInt32 Sequence;
		public byte configType;
		public SkillBars skillBars = new SkillBars();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, configType);
				skillBars.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref configType);
				skillBars.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, configType);
				skillBars.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref configType);
				skillBars.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// configType
				_len += 1;
				// skillBars
				_len += skillBars.getLen();
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneExchangeSkillBarRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501202;
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
	/// 检查名字合法性请求
	/// </summary>
	[Protocol]
	public class SceneCheckChangeNameReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501216;
		public UInt32 Sequence;
		public string newName;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] newNameBytes = StringHelper.StringToUTF8Bytes(newName);
				BaseDLL.encode_string(buffer, ref pos_, newNameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 newNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref newNameLen);
				byte[] newNameBytes = new byte[newNameLen];
				for(int i = 0; i < newNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref newNameBytes[i]);
				}
				newName = StringHelper.BytesToString(newNameBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] newNameBytes = StringHelper.StringToUTF8Bytes(newName);
				BaseDLL.encode_string(buffer, ref pos_, newNameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 newNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref newNameLen);
				byte[] newNameBytes = new byte[newNameLen];
				for(int i = 0; i < newNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref newNameBytes[i]);
				}
				newName = StringHelper.BytesToString(newNameBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// newName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(newName);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 检查名字合法性返回
	/// </summary>
	[Protocol]
	public class SceneCheckChangeNameRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501217;
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
	/// 改名
	/// </summary>
	[Protocol]
	public class SceneChangeNameReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501206;
		public UInt32 Sequence;
		public UInt64 itemUid;
		public string newName;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
				byte[] newNameBytes = StringHelper.StringToUTF8Bytes(newName);
				BaseDLL.encode_string(buffer, ref pos_, newNameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
				UInt16 newNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref newNameLen);
				byte[] newNameBytes = new byte[newNameLen];
				for(int i = 0; i < newNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref newNameBytes[i]);
				}
				newName = StringHelper.BytesToString(newNameBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
				byte[] newNameBytes = StringHelper.StringToUTF8Bytes(newName);
				BaseDLL.encode_string(buffer, ref pos_, newNameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
				UInt16 newNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref newNameLen);
				byte[] newNameBytes = new byte[newNameLen];
				for(int i = 0; i < newNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref newNameBytes[i]);
				}
				newName = StringHelper.BytesToString(newNameBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// itemUid
				_len += 8;
				// newName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(newName);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 改名返回
	/// </summary>
	[Protocol]
	public class SceneChangeNameRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501218;
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

	[Protocol]
	public class SceneChangeOccu : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501212;
		public UInt32 Sequence;
		public byte occu;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, occu);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, occu);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
			}

			public int getLen()
			{
				int _len = 0;
				// occu
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSyncFuncUnlock : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501213;
		public UInt32 Sequence;
		public byte funcId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, funcId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref funcId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, funcId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref funcId);
			}

			public int getLen()
			{
				int _len = 0;
				// funcId
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  同步系统开关（登录时）
	/// </summary>
	[Protocol]
	public class SceneSyncServiceSwitch : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501214;
		public UInt32 Sequence;
		/// <summary>
		///  关掉的系统（对应枚举ServiceType）
		/// </summary>
		public UInt16[] closedServices = new UInt16[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)closedServices.Length);
				for(int i = 0; i < closedServices.Length; i++)
				{
					BaseDLL.encode_uint16(buffer, ref pos_, closedServices[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 closedServicesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref closedServicesCnt);
				closedServices = new UInt16[closedServicesCnt];
				for(int i = 0; i < closedServices.Length; i++)
				{
					BaseDLL.decode_uint16(buffer, ref pos_, ref closedServices[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)closedServices.Length);
				for(int i = 0; i < closedServices.Length; i++)
				{
					BaseDLL.encode_uint16(buffer, ref pos_, closedServices[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 closedServicesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref closedServicesCnt);
				closedServices = new UInt16[closedServicesCnt];
				for(int i = 0; i < closedServices.Length; i++)
				{
					BaseDLL.decode_uint16(buffer, ref pos_, ref closedServices[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// closedServices
				_len += 2 + 2 * closedServices.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  修改系统开关
	/// </summary>
	[Protocol]
	public class SceneUpdateServiceSwitch : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501215;
		public UInt32 Sequence;
		/// <summary>
		///  系统类型（对应枚举ServiceType）
		/// </summary>
		public UInt16 type;
		/// <summary>
		///  是否开放
		/// </summary>
		public byte open;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, open);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref open);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, open);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref open);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 2;
				// open
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  设置武器栏请求
	/// </summary>
	[Protocol]
	public class SceneSetWeaponBarReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501219;
		public UInt32 Sequence;
		public byte index;
		public UInt64 weaponId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, index);
				BaseDLL.encode_uint64(buffer, ref pos_, weaponId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref index);
				BaseDLL.decode_uint64(buffer, ref pos_, ref weaponId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, index);
				BaseDLL.encode_uint64(buffer, ref pos_, weaponId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref index);
				BaseDLL.decode_uint64(buffer, ref pos_, ref weaponId);
			}

			public int getLen()
			{
				int _len = 0;
				// index
				_len += 1;
				// weaponId
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  设置武器栏响应
	/// </summary>
	[Protocol]
	public class SceneSetWeaponBarRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501220;
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
	///  设置pvp技能配置请求
	/// </summary>
	[Protocol]
	public class SceneSetPvpSkillConfigReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501221;
		public UInt32 Sequence;
		public byte isUsePvpConfig;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, isUsePvpConfig);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isUsePvpConfig);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, isUsePvpConfig);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isUsePvpConfig);
			}

			public int getLen()
			{
				int _len = 0;
				// isUsePvpConfig
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  设置pvp技能配置响应
	/// </summary>
	[Protocol]
	public class SceneSetPvpSkillConfigRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501222;
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
	///  设置公平竞技技能配置请求 0表示查询设置情况 1表示第一次设置
	/// </summary>
	[Protocol]
	public class SceneSetEqualPvpSkillConfigReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501226;
		public UInt32 Sequence;
		public byte isSetedEqualPvPConfig;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, isSetedEqualPvPConfig);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isSetedEqualPvPConfig);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, isSetedEqualPvPConfig);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isSetedEqualPvPConfig);
			}

			public int getLen()
			{
				int _len = 0;
				// isSetedEqualPvPConfig
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  设置公平竞技技能配置响应 0表示未设置 1表示已设置
	/// </summary>
	[Protocol]
	public class SceneSetEqualPvpSkillConfigRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501227;
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
	///  设置游戏设置请求
	/// </summary>
	[Protocol]
	public class SceneGameSetReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501223;
		public UInt32 Sequence;
		/// <summary>
		///  设置类型
		/// </summary>
		public UInt32 gameSetType;
		/// <summary>
		///  设置值
		/// </summary>
		public string setValue;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, gameSetType);
				byte[] setValueBytes = StringHelper.StringToUTF8Bytes(setValue);
				BaseDLL.encode_string(buffer, ref pos_, setValueBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref gameSetType);
				UInt16 setValueLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref setValueLen);
				byte[] setValueBytes = new byte[setValueLen];
				for(int i = 0; i < setValueLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref setValueBytes[i]);
				}
				setValue = StringHelper.BytesToString(setValueBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, gameSetType);
				byte[] setValueBytes = StringHelper.StringToUTF8Bytes(setValue);
				BaseDLL.encode_string(buffer, ref pos_, setValueBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref gameSetType);
				UInt16 setValueLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref setValueLen);
				byte[] setValueBytes = new byte[setValueLen];
				for(int i = 0; i < setValueLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref setValueBytes[i]);
				}
				setValue = StringHelper.BytesToString(setValueBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// gameSetType
				_len += 4;
				// setValue
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(setValue);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  设置pvp技能配置响应
	/// </summary>
	[Protocol]
	public class SceneGameSetRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501224;
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
	///  world->client  通知功能解锁
	/// </summary>
	[Protocol]
	public class WorldSyncFuncUnlock : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601201;
		public UInt32 Sequence;
		public byte funcId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, funcId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref funcId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, funcId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref funcId);
			}

			public int getLen()
			{
				int _len = 0;
				// funcId
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->scene  通知举报
	/// </summary>
	[Protocol]
	public class SceneReportNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501228;
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
	///  client->scene 保存选项
	/// </summary>
	[Protocol]
	public class SceneSaveOptionsReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501205;
		public UInt32 Sequence;
		public UInt32 options;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, options);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref options);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, options);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref options);
			}

			public int getLen()
			{
				int _len = 0;
				// options
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class ShortcutKeyInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  设置类型
		/// </summary>
		public UInt32 setType;
		/// <summary>
		///  设置值（客户端自定义的长字符串格式，最大1000个字节）
		/// </summary>
		public string setValue;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, setType);
				byte[] setValueBytes = StringHelper.StringToUTF8Bytes(setValue);
				BaseDLL.encode_string(buffer, ref pos_, setValueBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref setType);
				UInt16 setValueLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref setValueLen);
				byte[] setValueBytes = new byte[setValueLen];
				for(int i = 0; i < setValueLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref setValueBytes[i]);
				}
				setValue = StringHelper.BytesToString(setValueBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, setType);
				byte[] setValueBytes = StringHelper.StringToUTF8Bytes(setValue);
				BaseDLL.encode_string(buffer, ref pos_, setValueBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref setType);
				UInt16 setValueLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref setValueLen);
				byte[] setValueBytes = new byte[setValueLen];
				for(int i = 0; i < setValueLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref setValueBytes[i]);
				}
				setValue = StringHelper.BytesToString(setValueBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// setType
				_len += 4;
				// setValue
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(setValue);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  游戏快捷键设置请求
	/// </summary>
	[Protocol]
	public class SceneShortcutKeySetReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501229;
		public UInt32 Sequence;
		public ShortcutKeyInfo info = new ShortcutKeyInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
	///  游戏快捷键设置返回
	/// </summary>
	[Protocol]
	public class SceneShortcutKeySetRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501230;
		public UInt32 Sequence;
		public UInt32 retCode;
		public ShortcutKeyInfo info = new ShortcutKeyInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				info.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				info.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				info.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				info.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// info
				_len += info.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  游戏快捷键设置同步
	/// </summary>
	[Protocol]
	public class SceneShortcutKeySetSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501231;
		public UInt32 Sequence;
		public ShortcutKeyInfo[] infos = new ShortcutKeyInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)infos.Length);
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 infosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref infosCnt);
				infos = new ShortcutKeyInfo[infosCnt];
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i] = new ShortcutKeyInfo();
					infos[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)infos.Length);
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 infosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref infosCnt);
				infos = new ShortcutKeyInfo[infosCnt];
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i] = new ShortcutKeyInfo();
					infos[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// infos
				_len += 2;
				for(int j = 0; j < infos.Length; j++)
				{
					_len += infos[j].getLen();
				}
				return _len;
			}
		#endregion

	}

}
