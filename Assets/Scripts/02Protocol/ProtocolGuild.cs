using System;
using System.Text;

namespace Protocol
{
	/// <summary>
	///  公会职务
	/// </summary>
	public enum GuildPost
	{
		/// <summary>
		///  无效值
		/// </summary>
		GUILD_INVALID = 0,
		/// <summary>
		///  普通成员
		/// </summary>
		GUILD_POST_NORMAL = 1,
		/// <summary>
		///  精英
		/// </summary>
		GUILD_POST_ELITE = 2,
		/// <summary>
		///  长老
		/// </summary>
		GUILD_POST_ELDER = 11,
		/// <summary>
		///  副会长
		/// </summary>
		GUILD_POST_ASSISTANT = 12,
		/// <summary>
		///  会长
		/// </summary>
		GUILD_POST_LEADER = 13,
	}

	/// <summary>
	///  公会属性
	/// </summary>
	public enum GuildAttr
	{
		/// <summary>
		///  无效属性
		/// </summary>
		GA_INVALID = 0,
		/// <summary>
		///  名字	string	
		/// </summary>
		GA_NAME = 1,
		/// <summary>
		///  等级	UInt8	
		/// </summary>
		GA_LEVEL = 2,
		/// <summary>
		///  宣言 string
		/// </summary>
		GA_DECLARATION = 3,
		/// <summary>
		///  部落资金 Int32
		/// </summary>
		GA_FUND = 4,
		/// <summary>
		///  公告 string
		/// </summary>
		GA_ANNOUNCEMENT = 5,
		/// <summary>
		///  公会建筑 GuildBuilding
		/// </summary>
		GA_BUILDING = 6,
		/// <summary>
		///  解散时间 UInt32
		/// </summary>
		GA_DISMISS_TIME = 7,
		/// <summary>
		///  成员数量 UInt16
		/// </summary>
		GA_MEMBER_NUM = 8,
		/// <summary>
		///  会长名字 string
		/// </summary>
		GA_LEADER_NAME = 9,
		/// <summary>
		///  报名领地ID UInt8
		/// </summary>
		GA_ENROLL_TERRID = 10,
		/// <summary>
		///  公会战分数 UInt32
		/// </summary>
		GA_BATTLE_SCORE = 11,
		/// <summary>
		///  公会占领领地 UInt8
		/// </summary>
		GA_OCCUPY_TERRID = 12,
		/// <summary>
		///  公会战鼓舞次数 UInt8
		/// </summary>
		GA_INSPIRE = 13,
		/// <summary>
		///  公会战胜利抽奖几率 UInt8
		/// </summary>
		GA_WIN_PROBABILITY = 14,
		/// <summary>
		///  公会战失败抽奖几率 UInt8
		/// </summary>
		GA_LOSE_PROBABILITY = 15,
		/// <summary>
		///  公会战仓库放入物品 UInt8
		/// </summary>
		GA_STORAGE_ADD_POST = 16,
		/// <summary>
		///  公会战仓库删除物品 UInt8
		/// </summary>
		GA_STORAGE_DEL_POST = 17,
		/// <summary>
		///  公会占领跨服领地 UInt8
		/// </summary>
		GA_OCCUPY_CROSS_TERRID = 18,
		/// <summary>
		///  工会历史占领领地 UInt8
		/// </summary>
		GA_HISTORY_TERRID = 19,
		/// <summary>
		///  加入公会等级 UInt32
		/// </summary>
		GA_JOIN_LEVEL = 20,
		/// <summary>
		///  公会副本难度 UInt32
		/// </summary>
		GA_DUNGEON_TYPE = 21,
		/// <summary>
		///  上周增加的繁荣度 UInt32
		/// </summary>
		GA_LAST_WEEK_ADD_FUND = 22,
		/// <summary>
		///  本周增加的繁荣度 UInt32
		/// </summary>
		GA_THIS_WEEK_ADD_FUND = 23,
	}

	/// <summary>
	/// 公会战类型
	/// </summary>
	public enum GuildBattleType
	{
		/// <summary>
		///  无效
		/// </summary>
		GBT_INVALID = 0,
		/// <summary>
		///  普通
		/// </summary>
		GBT_NORMAL = 1,
		/// <summary>
		///  宣战
		/// </summary>
		GBT_CHALLENGE = 2,
		/// <summary>
		/// 跨服
		/// </summary>
		GBT_CROSS = 3,
		GBT_MAX = 4,
	}

	/// <summary>
	///  工会战领地类型
	/// </summary>
	public enum GuildTerritoryType
	{
		/// <summary>
		///  无效
		/// </summary>
		GTT_INVALID = 0,
		/// <summary>
		/// 普通
		/// </summary>
		GTT_NORMAL = 1,
		/// <summary>
		/// 跨服
		/// </summary>
		GTT_CROSS = 2,
		GTT_MAX = 3,
	}

	/// <summary>
	///  公会战状态
	/// </summary>
	public enum GuildBattleStatus
	{
		/// <summary>
		///  无
		/// </summary>
		GBS_INVALID = 0,
		/// <summary>
		///  报名
		/// </summary>
		GBS_ENROLL = 1,
		/// <summary>
		///  准备
		/// </summary>
		GBS_PREPARE = 2,
		/// <summary>
		///  战斗
		/// </summary>
		GBS_BATTLE = 3,
		/// <summary>
		///  领奖
		/// </summary>
		GBS_REWARD = 4,
		GBS_MAX = 5,
	}

	/// <summary>
	///  公会建筑类型
	/// </summary>
	public enum GuildBuildingType
	{
		/// <summary>
		///  主城
		/// </summary>
		MAIN = 0,
		/// <summary>
		///  商店
		/// </summary>
		SHOP = 1,
		/// <summary>
		///  圆桌会议
		/// </summary>
		TABLE = 2,
		/// <summary>
		///  地下城
		/// </summary>
		DUNGEON = 3,
		/// <summary>
		///  雕像
		/// </summary>
		STATUE = 4,
		/// <summary>
		///  战争坊
		/// </summary>
		BATTLE = 5,
		/// <summary>
		///  福利社
		/// </summary>
		WELFARE = 6,
		/// <summary>
		///  荣耀殿堂
		/// </summary>
		HONOUR = 7,
		/// <summary>
		///  征战祭祀
		/// </summary>
		FETE = 8,
	}

	/// <summary>
	///  公会操作类型
	/// </summary>
	public enum GuildOperation
	{
		/// <summary>
		///  修改公会宣言
		/// </summary>
		MODIFY_DECLAR = 0,
		/// <summary>
		///  修改公会名
		/// </summary>
		MODIFY_NAME = 1,
		/// <summary>
		///  修改公会公告
		/// </summary>
		MODIFY_ANNOUNCE = 2,
		/// <summary>
		///  发送公会邮件
		/// </summary>
		SEND_MAIL = 3,
		/// <summary>
		///  升级建筑
		/// </summary>
		UPGRADE_BUILDING = 4,
		/// <summary>
		///  捐献
		/// </summary>
		DONATE = 5,
		/// <summary>
		///  兑换
		/// </summary>
		EXCHANGE = 6,
		/// <summary>
		///  升级技能
		/// </summary>
		UPGRADE_SKILL = 7,
		/// <summary>
		///  解散工会
		/// </summary>
		DISMISS = 8,
		/// <summary>
		///  取消解散工会
		/// </summary>
		CANCEL_DISMISS = 9,
		/// <summary>
		///  膜拜
		/// </summary>
		ORZ = 10,
		/// <summary>
		///  圆桌会议
		/// </summary>
		TABLE = 11,
		/// <summary>
		///  自费红包
		/// </summary>
		PAY_REDPACKET = 12,
	}

	/// <summary>
	///  捐献
	/// </summary>
	public enum GuildDonateType
	{
		/// <summary>
		///  金币捐献
		/// </summary>
		GOLD = 0,
		/// <summary>
		///  点劵捐献
		/// </summary>
		POINT = 1,
	}

	/// <summary>
	///  膜拜类型
	/// </summary>
	public enum GuildOrzType
	{
		/// <summary>
		///  普通膜拜
		/// </summary>
		GUILD_ORZ_LOW = 0,
		/// <summary>
		///  中级膜拜
		/// </summary>
		GUILD_ORZ_MID = 1,
		/// <summary>
		///  高级膜拜
		/// </summary>
		GUILD_ORZ_HIGH = 2,
	}

	/// <summary>
	///  雕像类型
	/// </summary>
	public enum FigureStatueType
	{
		FST_INVALID = 0,
		FST_GUILD = 1,
		FST_GUILD_ASSISTANT = 2,
		FST_GUILD_ASSISTANT_TWO = 3,
		/// <summary>
		///  公会地下城雕像
		/// </summary>
		FST_GUILD_DUNGEON_FIRST = 4,
		FST_GUILD_DUNGEON_SECOND = 5,
		FST_GUILD_DUNGEON_THIRD = 6,
	}

	/// <summary>
	///  公会仓库设置类型
	/// </summary>
	public enum GuildStorageSetting
	{
		GUILD_POST_INVALID = 0,
		/// <summary>
		///  胜利抽奖几率
		/// </summary>
		GSS_WIN_PROBABILITY = 1,
		/// <summary>
		///  失败抽奖几率
		/// </summary>
		GSS_LOSE_PROBABILITY = 2,
		/// <summary>
		///  仓库增加权限
		/// </summary>
		GSS_STORAGE_ADD_POST = 3,
		/// <summary>
		///  仓库删除权限
		/// </summary>
		GSS_STORAGE_DEL_POST = 4,
		GSS_MAX = 5,
	}

	/// <summary>
	///  公会成员抽奖状态
	/// </summary>
	public enum GuildBattleLotteryStatus
	{
		/// <summary>
		///  无效
		/// </summary>
		GBLS_INVALID = 0,
		/// <summary>
		///  不能抽奖
		/// </summary>
		GBLS_NOT = 1,
		/// <summary>
		///  可以抽奖
		/// </summary>
		GBLS_CAN = 2,
		/// <summary>
		///  已经抽奖
		/// </summary>
		GBLS_FIN = 3,
		GBLS_MAX = 4,
	}

	public enum GuildStorageOpType
	{
		GSOT_NONE = 0,
		/// <summary>
		///  获得
		/// </summary>
		GSOT_GET = 1,
		/// <summary>
		///  存入
		/// </summary>
		GSOT_PUT = 2,
		/// <summary>
		///  购买并存入
		/// </summary>
		GSOT_BUYPUT = 3,
	}

	/// <summary>
	/// *****************公会地下城*************************************
	/// </summary>
	/// <summary>
	///  公会地下城状态
	/// </summary>
	public enum GuildDungeonStatus
	{
		/// <summary>
		///  结束关闭
		/// </summary>
		GUILD_DUNGEON_END = 0,
		/// <summary>
		///  准备
		/// </summary>
		GUILD_DUNGEON_PREPARE = 1,
		/// <summary>
		///  开始
		/// </summary>
		GUILD_DUNGEON_START = 2,
		/// <summary>
		///  发奖
		/// </summary>
		GUILD_DUNGEON_REWARD = 3,
	}

	/// <summary>
	///  公会地下城难度等级
	/// </summary>
	public enum GuildDungeonLvl
	{
		/// <summary>
		///  初级
		/// </summary>
		GUILD_DUNGEON_LOW = 1,
		/// <summary>
		///  中级
		/// </summary>
		GUILD_DUNGEON_MID = 2,
		/// <summary>
		///  高级
		/// </summary>
		GUILD_DUNGEON_HIGUH = 3,
	}

	public enum GuildAuctionType
	{
		/// <summary>
		///  无效值
		/// </summary>
		G_AUCTION_INVALID = 0,
		/// <summary>
		///  公会拍
		/// </summary>
		G_AUCTION_GUILD = 1,
		/// <summary>
		///  世界拍
		/// </summary>
		G_AUCTION_WORLD = 2,
	}

	public enum GuildAuctionItemState
	{
		/// <summary>
		///  无效值
		/// </summary>
		GAI_STATE_INVALID = 0,
		/// <summary>
		///  拍卖准备
		/// </summary>
		GAI_STATE_PREPARE = 1,
		/// <summary>
		///  拍卖中
		/// </summary>
		GAI_STATE_NORMAL = 2,
		/// <summary>
		///  成交
		/// </summary>
		GAI_STATE_DEAL = 3,
		/// <summary>
		///  流拍
		/// </summary>
		GAI_STATE_ABORATION = 4,
	}

	/// <summary>
	///  公会信息
	/// </summary>
	public class GuildEntry : Protocol.IProtocolStream
	{
		/// <summary>
		///  id
		/// </summary>
		public UInt64 id;
		/// <summary>
		///  name
		/// </summary>
		public string name;
		/// <summary>
		///  公会等级
		/// </summary>
		public byte level;
		/// <summary>
		///  公会人数
		/// </summary>
		public byte memberNum;
		/// <summary>
		///  会长名字
		/// </summary>
		public string leaderName;
		/// <summary>
		///  宣言
		/// </summary>
		public string declaration;
		/// <summary>
		///  是否已经申请
		/// </summary>
		public byte isRequested;
		/// <summary>
		///  跨服领地
		/// </summary>
		public byte occupyCrossTerrId;
		/// <summary>
		///  本服领地
		/// </summary>
		public byte occupyTerrId;
		/// <summary>
		///  入会等级
		/// </summary>
		public UInt32 joinLevel;
		/// <summary>
		///  会长id
		/// </summary>
		public UInt64 leaderId;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, memberNum);
				byte[] leaderNameBytes = StringHelper.StringToUTF8Bytes(leaderName);
				BaseDLL.encode_string(buffer, ref pos_, leaderNameBytes, (UInt16)(buffer.Length - pos_));
				byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
				BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, isRequested);
				BaseDLL.encode_int8(buffer, ref pos_, occupyCrossTerrId);
				BaseDLL.encode_int8(buffer, ref pos_, occupyTerrId);
				BaseDLL.encode_uint32(buffer, ref pos_, joinLevel);
				BaseDLL.encode_uint64(buffer, ref pos_, leaderId);
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
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
				BaseDLL.decode_int8(buffer, ref pos_, ref memberNum);
				UInt16 leaderNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref leaderNameLen);
				byte[] leaderNameBytes = new byte[leaderNameLen];
				for(int i = 0; i < leaderNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref leaderNameBytes[i]);
				}
				leaderName = StringHelper.BytesToString(leaderNameBytes);
				UInt16 declarationLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref declarationLen);
				byte[] declarationBytes = new byte[declarationLen];
				for(int i = 0; i < declarationLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref declarationBytes[i]);
				}
				declaration = StringHelper.BytesToString(declarationBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref isRequested);
				BaseDLL.decode_int8(buffer, ref pos_, ref occupyCrossTerrId);
				BaseDLL.decode_int8(buffer, ref pos_, ref occupyTerrId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref joinLevel);
				BaseDLL.decode_uint64(buffer, ref pos_, ref leaderId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, memberNum);
				byte[] leaderNameBytes = StringHelper.StringToUTF8Bytes(leaderName);
				BaseDLL.encode_string(buffer, ref pos_, leaderNameBytes, (UInt16)(buffer.Length - pos_));
				byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
				BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, isRequested);
				BaseDLL.encode_int8(buffer, ref pos_, occupyCrossTerrId);
				BaseDLL.encode_int8(buffer, ref pos_, occupyTerrId);
				BaseDLL.encode_uint32(buffer, ref pos_, joinLevel);
				BaseDLL.encode_uint64(buffer, ref pos_, leaderId);
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
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
				BaseDLL.decode_int8(buffer, ref pos_, ref memberNum);
				UInt16 leaderNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref leaderNameLen);
				byte[] leaderNameBytes = new byte[leaderNameLen];
				for(int i = 0; i < leaderNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref leaderNameBytes[i]);
				}
				leaderName = StringHelper.BytesToString(leaderNameBytes);
				UInt16 declarationLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref declarationLen);
				byte[] declarationBytes = new byte[declarationLen];
				for(int i = 0; i < declarationLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref declarationBytes[i]);
				}
				declaration = StringHelper.BytesToString(declarationBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref isRequested);
				BaseDLL.decode_int8(buffer, ref pos_, ref occupyCrossTerrId);
				BaseDLL.decode_int8(buffer, ref pos_, ref occupyTerrId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref joinLevel);
				BaseDLL.decode_uint64(buffer, ref pos_, ref leaderId);
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
				// level
				_len += 1;
				// memberNum
				_len += 1;
				// leaderName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(leaderName);
					_len += 2 + _strBytes.Length;
				}
				// declaration
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(declaration);
					_len += 2 + _strBytes.Length;
				}
				// isRequested
				_len += 1;
				// occupyCrossTerrId
				_len += 1;
				// occupyTerrId
				_len += 1;
				// joinLevel
				_len += 4;
				// leaderId
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  公会成员
	/// </summary>
	public class GuildMemberEntry : Protocol.IProtocolStream
	{
		/// <summary>
		///  id
		/// </summary>
		public UInt64 id;
		/// <summary>
		///  name
		/// </summary>
		public string name;
		/// <summary>
		///  等级
		/// </summary>
		public UInt16 level;
		/// <summary>
		///  职业
		/// </summary>
		public byte occu;
		/// <summary>
		///  职务(对应枚举GuildPost)
		/// </summary>
		public byte post;
		/// <summary>
		///  历史贡献
		/// </summary>
		public UInt32 contribution;
		/// <summary>
		///  离线时间(0代表在线)
		/// </summary>
		public UInt32 logoutTime;
		/// <summary>
		///  活跃度
		/// </summary>
		public UInt32 activeDegree;
		/// <summary>
		/// vip等级
		/// </summary>
		public byte vipLevel;
		/// <summary>
		///  玩家段位
		/// </summary>
		public UInt32 seasonLevel;
		/// <summary>
		///  玩家标签信息
		/// </summary>
		public PlayerLabelInfo playerLabelInfo = new PlayerLabelInfo();

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_int8(buffer, ref pos_, post);
				BaseDLL.encode_uint32(buffer, ref pos_, contribution);
				BaseDLL.encode_uint32(buffer, ref pos_, logoutTime);
				BaseDLL.encode_uint32(buffer, ref pos_, activeDegree);
				BaseDLL.encode_int8(buffer, ref pos_, vipLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonLevel);
				playerLabelInfo.encode(buffer, ref pos_);
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
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_int8(buffer, ref pos_, ref post);
				BaseDLL.decode_uint32(buffer, ref pos_, ref contribution);
				BaseDLL.decode_uint32(buffer, ref pos_, ref logoutTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref activeDegree);
				BaseDLL.decode_int8(buffer, ref pos_, ref vipLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLevel);
				playerLabelInfo.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_int8(buffer, ref pos_, post);
				BaseDLL.encode_uint32(buffer, ref pos_, contribution);
				BaseDLL.encode_uint32(buffer, ref pos_, logoutTime);
				BaseDLL.encode_uint32(buffer, ref pos_, activeDegree);
				BaseDLL.encode_int8(buffer, ref pos_, vipLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonLevel);
				playerLabelInfo.encode(buffer, ref pos_);
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
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_int8(buffer, ref pos_, ref post);
				BaseDLL.decode_uint32(buffer, ref pos_, ref contribution);
				BaseDLL.decode_uint32(buffer, ref pos_, ref logoutTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref activeDegree);
				BaseDLL.decode_int8(buffer, ref pos_, ref vipLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLevel);
				playerLabelInfo.decode(buffer, ref pos_);
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
				// level
				_len += 2;
				// occu
				_len += 1;
				// post
				_len += 1;
				// contribution
				_len += 4;
				// logoutTime
				_len += 4;
				// activeDegree
				_len += 4;
				// vipLevel
				_len += 1;
				// seasonLevel
				_len += 4;
				// playerLabelInfo
				_len += playerLabelInfo.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  公会请求者信息
	/// </summary>
	public class GuildRequesterInfo : Protocol.IProtocolStream
	{
		/// <summary>
		/// id
		/// </summary>
		public UInt64 id;
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
		/// <summary>
		/// vip等级
		/// </summary>
		public byte vipLevel;
		/// <summary>
		/// 申请时间
		/// </summary>
		public UInt32 requestTime;
		/// <summary>
		///  玩家段位
		/// </summary>
		public UInt32 seasonLevel;
		/// <summary>
		/// 玩家标签信息
		/// </summary>
		public PlayerLabelInfo playerLabelInfo = new PlayerLabelInfo();

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_int8(buffer, ref pos_, vipLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, requestTime);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonLevel);
				playerLabelInfo.encode(buffer, ref pos_);
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
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_int8(buffer, ref pos_, ref vipLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref requestTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLevel);
				playerLabelInfo.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_int8(buffer, ref pos_, vipLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, requestTime);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonLevel);
				playerLabelInfo.encode(buffer, ref pos_);
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
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_int8(buffer, ref pos_, ref vipLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref requestTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLevel);
				playerLabelInfo.decode(buffer, ref pos_);
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
				// level
				_len += 2;
				// occu
				_len += 1;
				// vipLevel
				_len += 1;
				// requestTime
				_len += 4;
				// seasonLevel
				_len += 4;
				// playerLabelInfo
				_len += playerLabelInfo.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  公会建筑
	/// </summary>
	public class GuildBuilding : Protocol.IProtocolStream
	{
		/// <summary>
		///  建筑类型（对应枚举GuildBuildingType）
		/// </summary>
		public byte type;
		/// <summary>
		///  等级
		/// </summary>
		public byte level;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, level);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, level);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// level
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  圆桌会议成员信息
	/// </summary>
	public class GuildTableMember : Protocol.IProtocolStream
	{
		/// <summary>
		///  角色ID
		/// </summary>
		public UInt64 id;
		/// <summary>
		///  等级
		/// </summary>
		public UInt16 level;
		/// <summary>
		///  职业
		/// </summary>
		public byte occu;
		/// <summary>
		///  名字
		/// </summary>
		public string name;
		/// <summary>
		///  位置
		/// </summary>
		public byte seat;
		/// <summary>
		///  参与类型
		/// </summary>
		public byte type;
		/// <summary>
		///  玩家标签信息
		/// </summary>
		public PlayerLabelInfo playerLabelInfo = new PlayerLabelInfo();

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, seat);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				playerLabelInfo.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref seat);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				playerLabelInfo.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, seat);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				playerLabelInfo.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref seat);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				playerLabelInfo.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// level
				_len += 2;
				// occu
				_len += 1;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// seat
				_len += 1;
				// type
				_len += 1;
				// playerLabelInfo
				_len += playerLabelInfo.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  公会战成员
	/// </summary>
	public class GuildBattleMember : Protocol.IProtocolStream
	{
		/// <summary>
		///  ID
		/// </summary>
		public UInt64 id;
		/// <summary>
		/// 名字
		/// </summary>
		public string name;
		/// <summary>
		///  连胜数
		/// </summary>
		public byte winStreak;
		/// <summary>
		///  获得积分
		/// </summary>
		public UInt16 gotScore;
		/// <summary>
		///  总积分
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

	public class GuildBattleRecord : Protocol.IProtocolStream
	{
		public UInt32 index;
		/// <summary>
		///  胜利者
		/// </summary>
		public GuildBattleMember winner = new GuildBattleMember();
		/// <summary>
		///  失败者
		/// </summary>
		public GuildBattleMember loser = new GuildBattleMember();
		/// <summary>
		///  时间
		/// </summary>
		public UInt32 time;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, index);
				winner.encode(buffer, ref pos_);
				loser.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, time);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
				winner.decode(buffer, ref pos_);
				loser.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref time);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, index);
				winner.encode(buffer, ref pos_);
				loser.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, time);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
				winner.decode(buffer, ref pos_);
				loser.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref time);
			}

			public int getLen()
			{
				int _len = 0;
				// index
				_len += 4;
				// winner
				_len += winner.getLen();
				// loser
				_len += loser.getLen();
				// time
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class GuildTerritoryBaseInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  领地ID
		/// </summary>
		public byte terrId;
		/// <summary>
		///  服务器名称
		/// </summary>
		public string serverName;
		/// <summary>
		///  占领公会名称
		/// </summary>
		public string guildName;
		/// <summary>
		///  已经报名数量
		/// </summary>
		public UInt32 enrollSize;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, terrId);
				byte[] serverNameBytes = StringHelper.StringToUTF8Bytes(serverName);
				BaseDLL.encode_string(buffer, ref pos_, serverNameBytes, (UInt16)(buffer.Length - pos_));
				byte[] guildNameBytes = StringHelper.StringToUTF8Bytes(guildName);
				BaseDLL.encode_string(buffer, ref pos_, guildNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, enrollSize);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref terrId);
				UInt16 serverNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref serverNameLen);
				byte[] serverNameBytes = new byte[serverNameLen];
				for(int i = 0; i < serverNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref serverNameBytes[i]);
				}
				serverName = StringHelper.BytesToString(serverNameBytes);
				UInt16 guildNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guildNameLen);
				byte[] guildNameBytes = new byte[guildNameLen];
				for(int i = 0; i < guildNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref guildNameBytes[i]);
				}
				guildName = StringHelper.BytesToString(guildNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref enrollSize);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, terrId);
				byte[] serverNameBytes = StringHelper.StringToUTF8Bytes(serverName);
				BaseDLL.encode_string(buffer, ref pos_, serverNameBytes, (UInt16)(buffer.Length - pos_));
				byte[] guildNameBytes = StringHelper.StringToUTF8Bytes(guildName);
				BaseDLL.encode_string(buffer, ref pos_, guildNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, enrollSize);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref terrId);
				UInt16 serverNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref serverNameLen);
				byte[] serverNameBytes = new byte[serverNameLen];
				for(int i = 0; i < serverNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref serverNameBytes[i]);
				}
				serverName = StringHelper.BytesToString(serverNameBytes);
				UInt16 guildNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guildNameLen);
				byte[] guildNameBytes = new byte[guildNameLen];
				for(int i = 0; i < guildNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref guildNameBytes[i]);
				}
				guildName = StringHelper.BytesToString(guildNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref enrollSize);
			}

			public int getLen()
			{
				int _len = 0;
				// terrId
				_len += 1;
				// serverName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(serverName);
					_len += 2 + _strBytes.Length;
				}
				// guildName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(guildName);
					_len += 2 + _strBytes.Length;
				}
				// enrollSize
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class GuildBattleInspireInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  玩家ID
		/// </summary>
		public UInt64 playerId;
		/// <summary>
		///  玩家名字
		/// </summary>
		public string playerName;
		/// <summary>
		///  鼓舞次数
		/// </summary>
		public UInt32 inspireNum;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
				BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, inspireNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
				UInt16 playerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerNameLen);
				byte[] playerNameBytes = new byte[playerNameLen];
				for(int i = 0; i < playerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref playerNameBytes[i]);
				}
				playerName = StringHelper.BytesToString(playerNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref inspireNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
				BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, inspireNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
				UInt16 playerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerNameLen);
				byte[] playerNameBytes = new byte[playerNameLen];
				for(int i = 0; i < playerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref playerNameBytes[i]);
				}
				playerName = StringHelper.BytesToString(playerNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref inspireNum);
			}

			public int getLen()
			{
				int _len = 0;
				// playerId
				_len += 8;
				// playerName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(playerName);
					_len += 2 + _strBytes.Length;
				}
				// inspireNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  公会战相关信息
	/// </summary>
	public class GuildBattleBaseInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  报名领地ID
		/// </summary>
		public byte enrollTerrId;
		/// <summary>
		///  公会战积分
		/// </summary>
		public UInt32 guildBattleScore;
		/// <summary>
		///  已经占领的领地ID
		/// </summary>
		public byte occupyTerrId;
		/// <summary>
		///  已经占领的跨服领地ID
		/// </summary>
		public byte occupyCrossTerrId;
		/// <summary>
		///  历史占领的领地ID
		/// </summary>
		public byte historyTerrId;
		/// <summary>
		///  鼓舞次数
		/// </summary>
		public byte inspire;
		/// <summary>
		///  自己的公会战记录
		/// </summary>
		public GuildBattleRecord[] selfGuildBattleRecord = new GuildBattleRecord[0];
		/// <summary>
		///  领地信息
		/// </summary>
		public GuildTerritoryBaseInfo[] terrInfos = new GuildTerritoryBaseInfo[0];
		/// <summary>
		/// 公会战类型
		/// </summary>
		public byte guildBattleType;
		/// <summary>
		/// 公会战状态
		/// </summary>
		public byte guildBattleStatus;
		/// <summary>
		/// 公会战状态结束时间
		/// </summary>
		public UInt32 guildBattleStatusEndTime;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, enrollTerrId);
				BaseDLL.encode_uint32(buffer, ref pos_, guildBattleScore);
				BaseDLL.encode_int8(buffer, ref pos_, occupyTerrId);
				BaseDLL.encode_int8(buffer, ref pos_, occupyCrossTerrId);
				BaseDLL.encode_int8(buffer, ref pos_, historyTerrId);
				BaseDLL.encode_int8(buffer, ref pos_, inspire);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)selfGuildBattleRecord.Length);
				for(int i = 0; i < selfGuildBattleRecord.Length; i++)
				{
					selfGuildBattleRecord[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)terrInfos.Length);
				for(int i = 0; i < terrInfos.Length; i++)
				{
					terrInfos[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_int8(buffer, ref pos_, guildBattleType);
				BaseDLL.encode_int8(buffer, ref pos_, guildBattleStatus);
				BaseDLL.encode_uint32(buffer, ref pos_, guildBattleStatusEndTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref enrollTerrId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref guildBattleScore);
				BaseDLL.decode_int8(buffer, ref pos_, ref occupyTerrId);
				BaseDLL.decode_int8(buffer, ref pos_, ref occupyCrossTerrId);
				BaseDLL.decode_int8(buffer, ref pos_, ref historyTerrId);
				BaseDLL.decode_int8(buffer, ref pos_, ref inspire);
				UInt16 selfGuildBattleRecordCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref selfGuildBattleRecordCnt);
				selfGuildBattleRecord = new GuildBattleRecord[selfGuildBattleRecordCnt];
				for(int i = 0; i < selfGuildBattleRecord.Length; i++)
				{
					selfGuildBattleRecord[i] = new GuildBattleRecord();
					selfGuildBattleRecord[i].decode(buffer, ref pos_);
				}
				UInt16 terrInfosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref terrInfosCnt);
				terrInfos = new GuildTerritoryBaseInfo[terrInfosCnt];
				for(int i = 0; i < terrInfos.Length; i++)
				{
					terrInfos[i] = new GuildTerritoryBaseInfo();
					terrInfos[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_int8(buffer, ref pos_, ref guildBattleType);
				BaseDLL.decode_int8(buffer, ref pos_, ref guildBattleStatus);
				BaseDLL.decode_uint32(buffer, ref pos_, ref guildBattleStatusEndTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, enrollTerrId);
				BaseDLL.encode_uint32(buffer, ref pos_, guildBattleScore);
				BaseDLL.encode_int8(buffer, ref pos_, occupyTerrId);
				BaseDLL.encode_int8(buffer, ref pos_, occupyCrossTerrId);
				BaseDLL.encode_int8(buffer, ref pos_, historyTerrId);
				BaseDLL.encode_int8(buffer, ref pos_, inspire);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)selfGuildBattleRecord.Length);
				for(int i = 0; i < selfGuildBattleRecord.Length; i++)
				{
					selfGuildBattleRecord[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)terrInfos.Length);
				for(int i = 0; i < terrInfos.Length; i++)
				{
					terrInfos[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_int8(buffer, ref pos_, guildBattleType);
				BaseDLL.encode_int8(buffer, ref pos_, guildBattleStatus);
				BaseDLL.encode_uint32(buffer, ref pos_, guildBattleStatusEndTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref enrollTerrId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref guildBattleScore);
				BaseDLL.decode_int8(buffer, ref pos_, ref occupyTerrId);
				BaseDLL.decode_int8(buffer, ref pos_, ref occupyCrossTerrId);
				BaseDLL.decode_int8(buffer, ref pos_, ref historyTerrId);
				BaseDLL.decode_int8(buffer, ref pos_, ref inspire);
				UInt16 selfGuildBattleRecordCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref selfGuildBattleRecordCnt);
				selfGuildBattleRecord = new GuildBattleRecord[selfGuildBattleRecordCnt];
				for(int i = 0; i < selfGuildBattleRecord.Length; i++)
				{
					selfGuildBattleRecord[i] = new GuildBattleRecord();
					selfGuildBattleRecord[i].decode(buffer, ref pos_);
				}
				UInt16 terrInfosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref terrInfosCnt);
				terrInfos = new GuildTerritoryBaseInfo[terrInfosCnt];
				for(int i = 0; i < terrInfos.Length; i++)
				{
					terrInfos[i] = new GuildTerritoryBaseInfo();
					terrInfos[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_int8(buffer, ref pos_, ref guildBattleType);
				BaseDLL.decode_int8(buffer, ref pos_, ref guildBattleStatus);
				BaseDLL.decode_uint32(buffer, ref pos_, ref guildBattleStatusEndTime);
			}

			public int getLen()
			{
				int _len = 0;
				// enrollTerrId
				_len += 1;
				// guildBattleScore
				_len += 4;
				// occupyTerrId
				_len += 1;
				// occupyCrossTerrId
				_len += 1;
				// historyTerrId
				_len += 1;
				// inspire
				_len += 1;
				// selfGuildBattleRecord
				_len += 2;
				for(int j = 0; j < selfGuildBattleRecord.Length; j++)
				{
					_len += selfGuildBattleRecord[j].getLen();
				}
				// terrInfos
				_len += 2;
				for(int j = 0; j < terrInfos.Length; j++)
				{
					_len += terrInfos[j].getLen();
				}
				// guildBattleType
				_len += 1;
				// guildBattleStatus
				_len += 1;
				// guildBattleStatusEndTime
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  公会基础信息
	/// </summary>
	public class GuildBaseInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  公会ID
		/// </summary>
		public UInt64 id;
		/// <summary>
		///  公会名
		/// </summary>
		public string name;
		/// <summary>
		///  公会等级
		/// </summary>
		public byte level;
		/// <summary>
		///  公会资金
		/// </summary>
		public UInt32 fund;
		/// <summary>
		///  公会宣言
		/// </summary>
		public string declaration;
		/// <summary>
		///  公会公告
		/// </summary>
		public string announcement;
		/// <summary>
		///  解散时间
		/// </summary>
		public UInt32 dismissTime;
		/// <summary>
		///  成员数量
		/// </summary>
		public UInt16 memberNum;
		/// <summary>
		///  会长名字
		/// </summary>
		public string leaderName;
		/// <summary>
		///  公会战胜利抽奖几率
		/// </summary>
		public byte winProbability;
		/// <summary>
		///  公会战失败抽奖几率
		/// </summary>
		public byte loseProbability;
		/// <summary>
		///  公会仓库放入权限
		/// </summary>
		public byte storageAddPost;
		/// <summary>
		///  公会仓库放入权限
		/// </summary>
		public byte storageDelPost;
		/// <summary>
		///  建筑信息
		/// </summary>
		public GuildBuilding[] building = new GuildBuilding[0];
		/// <summary>
		///  有没有申请加入公会的人
		/// </summary>
		public byte hasRequester;
		/// <summary>
		///  圆桌会议成员信息
		/// </summary>
		public GuildTableMember[] tableMembers = new GuildTableMember[0];
		/// <summary>
		///  公会战相关信息
		/// </summary>
		public GuildBattleBaseInfo guildBattleInfo = new GuildBattleBaseInfo();
		/// <summary>
		///  入会限制等级
		/// </summary>
		public UInt32 joinLevel;
		/// <summary>
		///  徽记
		/// </summary>
		public UInt32 emblemLevel;
		/// <summary>
		///  公会副本难度
		/// </summary>
		public UInt32 dungeonType;
		/// <summary>
		/// 公会本周增加繁荣度
		/// </summary>
		public UInt32 weekAddedFund;
		/// <summary>
		/// 公会上周增加繁荣度
		/// </summary>
		public UInt32 lastWeekAddedFund;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, level);
				BaseDLL.encode_uint32(buffer, ref pos_, fund);
				byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
				BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
				byte[] announcementBytes = StringHelper.StringToUTF8Bytes(announcement);
				BaseDLL.encode_string(buffer, ref pos_, announcementBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, dismissTime);
				BaseDLL.encode_uint16(buffer, ref pos_, memberNum);
				byte[] leaderNameBytes = StringHelper.StringToUTF8Bytes(leaderName);
				BaseDLL.encode_string(buffer, ref pos_, leaderNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, winProbability);
				BaseDLL.encode_int8(buffer, ref pos_, loseProbability);
				BaseDLL.encode_int8(buffer, ref pos_, storageAddPost);
				BaseDLL.encode_int8(buffer, ref pos_, storageDelPost);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)building.Length);
				for(int i = 0; i < building.Length; i++)
				{
					building[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_int8(buffer, ref pos_, hasRequester);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)tableMembers.Length);
				for(int i = 0; i < tableMembers.Length; i++)
				{
					tableMembers[i].encode(buffer, ref pos_);
				}
				guildBattleInfo.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, joinLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, emblemLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonType);
				BaseDLL.encode_uint32(buffer, ref pos_, weekAddedFund);
				BaseDLL.encode_uint32(buffer, ref pos_, lastWeekAddedFund);
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
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
				BaseDLL.decode_uint32(buffer, ref pos_, ref fund);
				UInt16 declarationLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref declarationLen);
				byte[] declarationBytes = new byte[declarationLen];
				for(int i = 0; i < declarationLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref declarationBytes[i]);
				}
				declaration = StringHelper.BytesToString(declarationBytes);
				UInt16 announcementLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref announcementLen);
				byte[] announcementBytes = new byte[announcementLen];
				for(int i = 0; i < announcementLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref announcementBytes[i]);
				}
				announcement = StringHelper.BytesToString(announcementBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dismissTime);
				BaseDLL.decode_uint16(buffer, ref pos_, ref memberNum);
				UInt16 leaderNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref leaderNameLen);
				byte[] leaderNameBytes = new byte[leaderNameLen];
				for(int i = 0; i < leaderNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref leaderNameBytes[i]);
				}
				leaderName = StringHelper.BytesToString(leaderNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref winProbability);
				BaseDLL.decode_int8(buffer, ref pos_, ref loseProbability);
				BaseDLL.decode_int8(buffer, ref pos_, ref storageAddPost);
				BaseDLL.decode_int8(buffer, ref pos_, ref storageDelPost);
				UInt16 buildingCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref buildingCnt);
				building = new GuildBuilding[buildingCnt];
				for(int i = 0; i < building.Length; i++)
				{
					building[i] = new GuildBuilding();
					building[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_int8(buffer, ref pos_, ref hasRequester);
				UInt16 tableMembersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref tableMembersCnt);
				tableMembers = new GuildTableMember[tableMembersCnt];
				for(int i = 0; i < tableMembers.Length; i++)
				{
					tableMembers[i] = new GuildTableMember();
					tableMembers[i].decode(buffer, ref pos_);
				}
				guildBattleInfo.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref joinLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref emblemLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref weekAddedFund);
				BaseDLL.decode_uint32(buffer, ref pos_, ref lastWeekAddedFund);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, level);
				BaseDLL.encode_uint32(buffer, ref pos_, fund);
				byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
				BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
				byte[] announcementBytes = StringHelper.StringToUTF8Bytes(announcement);
				BaseDLL.encode_string(buffer, ref pos_, announcementBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, dismissTime);
				BaseDLL.encode_uint16(buffer, ref pos_, memberNum);
				byte[] leaderNameBytes = StringHelper.StringToUTF8Bytes(leaderName);
				BaseDLL.encode_string(buffer, ref pos_, leaderNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, winProbability);
				BaseDLL.encode_int8(buffer, ref pos_, loseProbability);
				BaseDLL.encode_int8(buffer, ref pos_, storageAddPost);
				BaseDLL.encode_int8(buffer, ref pos_, storageDelPost);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)building.Length);
				for(int i = 0; i < building.Length; i++)
				{
					building[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_int8(buffer, ref pos_, hasRequester);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)tableMembers.Length);
				for(int i = 0; i < tableMembers.Length; i++)
				{
					tableMembers[i].encode(buffer, ref pos_);
				}
				guildBattleInfo.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, joinLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, emblemLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonType);
				BaseDLL.encode_uint32(buffer, ref pos_, weekAddedFund);
				BaseDLL.encode_uint32(buffer, ref pos_, lastWeekAddedFund);
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
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
				BaseDLL.decode_uint32(buffer, ref pos_, ref fund);
				UInt16 declarationLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref declarationLen);
				byte[] declarationBytes = new byte[declarationLen];
				for(int i = 0; i < declarationLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref declarationBytes[i]);
				}
				declaration = StringHelper.BytesToString(declarationBytes);
				UInt16 announcementLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref announcementLen);
				byte[] announcementBytes = new byte[announcementLen];
				for(int i = 0; i < announcementLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref announcementBytes[i]);
				}
				announcement = StringHelper.BytesToString(announcementBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dismissTime);
				BaseDLL.decode_uint16(buffer, ref pos_, ref memberNum);
				UInt16 leaderNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref leaderNameLen);
				byte[] leaderNameBytes = new byte[leaderNameLen];
				for(int i = 0; i < leaderNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref leaderNameBytes[i]);
				}
				leaderName = StringHelper.BytesToString(leaderNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref winProbability);
				BaseDLL.decode_int8(buffer, ref pos_, ref loseProbability);
				BaseDLL.decode_int8(buffer, ref pos_, ref storageAddPost);
				BaseDLL.decode_int8(buffer, ref pos_, ref storageDelPost);
				UInt16 buildingCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref buildingCnt);
				building = new GuildBuilding[buildingCnt];
				for(int i = 0; i < building.Length; i++)
				{
					building[i] = new GuildBuilding();
					building[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_int8(buffer, ref pos_, ref hasRequester);
				UInt16 tableMembersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref tableMembersCnt);
				tableMembers = new GuildTableMember[tableMembersCnt];
				for(int i = 0; i < tableMembers.Length; i++)
				{
					tableMembers[i] = new GuildTableMember();
					tableMembers[i].decode(buffer, ref pos_);
				}
				guildBattleInfo.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref joinLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref emblemLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref weekAddedFund);
				BaseDLL.decode_uint32(buffer, ref pos_, ref lastWeekAddedFund);
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
				// level
				_len += 1;
				// fund
				_len += 4;
				// declaration
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(declaration);
					_len += 2 + _strBytes.Length;
				}
				// announcement
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(announcement);
					_len += 2 + _strBytes.Length;
				}
				// dismissTime
				_len += 4;
				// memberNum
				_len += 2;
				// leaderName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(leaderName);
					_len += 2 + _strBytes.Length;
				}
				// winProbability
				_len += 1;
				// loseProbability
				_len += 1;
				// storageAddPost
				_len += 1;
				// storageDelPost
				_len += 1;
				// building
				_len += 2;
				for(int j = 0; j < building.Length; j++)
				{
					_len += building[j].getLen();
				}
				// hasRequester
				_len += 1;
				// tableMembers
				_len += 2;
				for(int j = 0; j < tableMembers.Length; j++)
				{
					_len += tableMembers[j].getLen();
				}
				// guildBattleInfo
				_len += guildBattleInfo.getLen();
				// joinLevel
				_len += 4;
				// emblemLevel
				_len += 4;
				// dungeonType
				_len += 4;
				// weekAddedFund
				_len += 4;
				// lastWeekAddedFund
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  捐献日志
	/// </summary>
	public class GuildDonateLog : Protocol.IProtocolStream
	{
		/// <summary>
		///  id
		/// </summary>
		public UInt64 id;
		/// <summary>
		///  名字
		/// </summary>
		public string name;
		/// <summary>
		///  捐献类型（对应枚举GuildDonateType）
		/// </summary>
		public byte type;
		/// <summary>
		///  次数
		/// </summary>
		public byte num;
		/// <summary>
		///  获得贡献
		/// </summary>
		public UInt32 contri;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, num);
				BaseDLL.encode_uint32(buffer, ref pos_, contri);
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
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref num);
				BaseDLL.decode_uint32(buffer, ref pos_, ref contri);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, num);
				BaseDLL.encode_uint32(buffer, ref pos_, contri);
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
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref num);
				BaseDLL.decode_uint32(buffer, ref pos_, ref contri);
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
				// type
				_len += 1;
				// num
				_len += 1;
				// contri
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  公会会长信息
	/// </summary>
	public class GuildLeaderInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  ID
		/// </summary>
		public UInt64 id;
		/// <summary>
		///  名字
		/// </summary>
		public string name;
		/// <summary>
		///  职业
		/// </summary>
		public byte occu;
		/// <summary>
		///  外观
		/// </summary>
		public PlayerAvatar avatar = new PlayerAvatar();
		/// <summary>
		///  人气
		/// </summary>
		public UInt32 popularoty;
		/// <summary>
		///  玩家标签信息
		/// </summary>
		public PlayerLabelInfo playerLabelInfo = new PlayerLabelInfo();

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				avatar.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, popularoty);
				playerLabelInfo.encode(buffer, ref pos_);
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
				avatar.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref popularoty);
				playerLabelInfo.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				avatar.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, popularoty);
				playerLabelInfo.encode(buffer, ref pos_);
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
				avatar.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref popularoty);
				playerLabelInfo.decode(buffer, ref pos_);
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
				// avatar
				_len += avatar.getLen();
				// popularoty
				_len += 4;
				// playerLabelInfo
				_len += playerLabelInfo.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  雕像数据
	/// </summary>
	public class FigureStatueInfo : Protocol.IProtocolStream
	{
		public UInt32 accId;
		public UInt64 roleId;
		public string name;
		public byte occu;
		public PlayerAvatar avatar = new PlayerAvatar();
		public byte statueType;
		public string guildName;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, accId);
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				avatar.encode(buffer, ref pos_);
				BaseDLL.encode_int8(buffer, ref pos_, statueType);
				byte[] guildNameBytes = StringHelper.StringToUTF8Bytes(guildName);
				BaseDLL.encode_string(buffer, ref pos_, guildNameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref accId);
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
				avatar.decode(buffer, ref pos_);
				BaseDLL.decode_int8(buffer, ref pos_, ref statueType);
				UInt16 guildNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guildNameLen);
				byte[] guildNameBytes = new byte[guildNameLen];
				for(int i = 0; i < guildNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref guildNameBytes[i]);
				}
				guildName = StringHelper.BytesToString(guildNameBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, accId);
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				avatar.encode(buffer, ref pos_);
				BaseDLL.encode_int8(buffer, ref pos_, statueType);
				byte[] guildNameBytes = StringHelper.StringToUTF8Bytes(guildName);
				BaseDLL.encode_string(buffer, ref pos_, guildNameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref accId);
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
				avatar.decode(buffer, ref pos_);
				BaseDLL.decode_int8(buffer, ref pos_, ref statueType);
				UInt16 guildNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guildNameLen);
				byte[] guildNameBytes = new byte[guildNameLen];
				for(int i = 0; i < guildNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref guildNameBytes[i]);
				}
				guildName = StringHelper.BytesToString(guildNameBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// accId
				_len += 4;
				// roleId
				_len += 8;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// occu
				_len += 1;
				// avatar
				_len += avatar.getLen();
				// statueType
				_len += 1;
				// guildName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(guildName);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  工会站结束信息
	/// </summary>
	public class GuildBattleEndInfo : Protocol.IProtocolStream
	{
		public byte terrId;
		public string terrName;
		public UInt64 guildId;
		public string guildName;
		public string guildLeaderName;
		public string guildServerName;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, terrId);
				byte[] terrNameBytes = StringHelper.StringToUTF8Bytes(terrName);
				BaseDLL.encode_string(buffer, ref pos_, terrNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint64(buffer, ref pos_, guildId);
				byte[] guildNameBytes = StringHelper.StringToUTF8Bytes(guildName);
				BaseDLL.encode_string(buffer, ref pos_, guildNameBytes, (UInt16)(buffer.Length - pos_));
				byte[] guildLeaderNameBytes = StringHelper.StringToUTF8Bytes(guildLeaderName);
				BaseDLL.encode_string(buffer, ref pos_, guildLeaderNameBytes, (UInt16)(buffer.Length - pos_));
				byte[] guildServerNameBytes = StringHelper.StringToUTF8Bytes(guildServerName);
				BaseDLL.encode_string(buffer, ref pos_, guildServerNameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref terrId);
				UInt16 terrNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref terrNameLen);
				byte[] terrNameBytes = new byte[terrNameLen];
				for(int i = 0; i < terrNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref terrNameBytes[i]);
				}
				terrName = StringHelper.BytesToString(terrNameBytes);
				BaseDLL.decode_uint64(buffer, ref pos_, ref guildId);
				UInt16 guildNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guildNameLen);
				byte[] guildNameBytes = new byte[guildNameLen];
				for(int i = 0; i < guildNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref guildNameBytes[i]);
				}
				guildName = StringHelper.BytesToString(guildNameBytes);
				UInt16 guildLeaderNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guildLeaderNameLen);
				byte[] guildLeaderNameBytes = new byte[guildLeaderNameLen];
				for(int i = 0; i < guildLeaderNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref guildLeaderNameBytes[i]);
				}
				guildLeaderName = StringHelper.BytesToString(guildLeaderNameBytes);
				UInt16 guildServerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guildServerNameLen);
				byte[] guildServerNameBytes = new byte[guildServerNameLen];
				for(int i = 0; i < guildServerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref guildServerNameBytes[i]);
				}
				guildServerName = StringHelper.BytesToString(guildServerNameBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, terrId);
				byte[] terrNameBytes = StringHelper.StringToUTF8Bytes(terrName);
				BaseDLL.encode_string(buffer, ref pos_, terrNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint64(buffer, ref pos_, guildId);
				byte[] guildNameBytes = StringHelper.StringToUTF8Bytes(guildName);
				BaseDLL.encode_string(buffer, ref pos_, guildNameBytes, (UInt16)(buffer.Length - pos_));
				byte[] guildLeaderNameBytes = StringHelper.StringToUTF8Bytes(guildLeaderName);
				BaseDLL.encode_string(buffer, ref pos_, guildLeaderNameBytes, (UInt16)(buffer.Length - pos_));
				byte[] guildServerNameBytes = StringHelper.StringToUTF8Bytes(guildServerName);
				BaseDLL.encode_string(buffer, ref pos_, guildServerNameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref terrId);
				UInt16 terrNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref terrNameLen);
				byte[] terrNameBytes = new byte[terrNameLen];
				for(int i = 0; i < terrNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref terrNameBytes[i]);
				}
				terrName = StringHelper.BytesToString(terrNameBytes);
				BaseDLL.decode_uint64(buffer, ref pos_, ref guildId);
				UInt16 guildNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guildNameLen);
				byte[] guildNameBytes = new byte[guildNameLen];
				for(int i = 0; i < guildNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref guildNameBytes[i]);
				}
				guildName = StringHelper.BytesToString(guildNameBytes);
				UInt16 guildLeaderNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guildLeaderNameLen);
				byte[] guildLeaderNameBytes = new byte[guildLeaderNameLen];
				for(int i = 0; i < guildLeaderNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref guildLeaderNameBytes[i]);
				}
				guildLeaderName = StringHelper.BytesToString(guildLeaderNameBytes);
				UInt16 guildServerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guildServerNameLen);
				byte[] guildServerNameBytes = new byte[guildServerNameLen];
				for(int i = 0; i < guildServerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref guildServerNameBytes[i]);
				}
				guildServerName = StringHelper.BytesToString(guildServerNameBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// terrId
				_len += 1;
				// terrName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(terrName);
					_len += 2 + _strBytes.Length;
				}
				// guildId
				_len += 8;
				// guildName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(guildName);
					_len += 2 + _strBytes.Length;
				}
				// guildLeaderName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(guildLeaderName);
					_len += 2 + _strBytes.Length;
				}
				// guildServerName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(guildServerName);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	public class GuildStorageItemInfo : Protocol.IProtocolStream
	{
		public UInt64 uid;
		public UInt32 dataId;
		public UInt16 num;
		public byte str;
		public byte equipType;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
				BaseDLL.encode_int8(buffer, ref pos_, str);
				BaseDLL.encode_int8(buffer, ref pos_, equipType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
				BaseDLL.decode_int8(buffer, ref pos_, ref str);
				BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
				BaseDLL.encode_int8(buffer, ref pos_, str);
				BaseDLL.encode_int8(buffer, ref pos_, equipType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
				BaseDLL.decode_int8(buffer, ref pos_, ref str);
				BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
			}

			public int getLen()
			{
				int _len = 0;
				// uid
				_len += 8;
				// dataId
				_len += 4;
				// num
				_len += 2;
				// str
				_len += 1;
				// equipType
				_len += 1;
				return _len;
			}
		#endregion

	}

	public class GuildStorageDelItemInfo : Protocol.IProtocolStream
	{
		public UInt64 uid;
		public UInt16 num;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// uid
				_len += 8;
				// num
				_len += 2;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  仓库记录类型
	/// </summary>
	public class GuildStorageOpRecord : Protocol.IProtocolStream
	{
		public string name;
		public UInt32 opType;
		public GuildStorageItemInfo[] items = new GuildStorageItemInfo[0];
		public UInt32 time;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, opType);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, time);
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
				BaseDLL.decode_uint32(buffer, ref pos_, ref opType);
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new GuildStorageItemInfo[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new GuildStorageItemInfo();
					items[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref time);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, opType);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, time);
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
				BaseDLL.decode_uint32(buffer, ref pos_, ref opType);
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new GuildStorageItemInfo[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new GuildStorageItemInfo();
					items[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref time);
			}

			public int getLen()
			{
				int _len = 0;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// opType
				_len += 4;
				// items
				_len += 2;
				for(int j = 0; j < items.Length; j++)
				{
					_len += items[j].getLen();
				}
				// time
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class GuildEvent : Protocol.IProtocolStream
	{
		/// <summary>
		/// 事件信息
		/// </summary>
		public string eventInfo;
		/// <summary>
		/// 发生时间
		/// </summary>
		public UInt32 addTime;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] eventInfoBytes = StringHelper.StringToUTF8Bytes(eventInfo);
				BaseDLL.encode_string(buffer, ref pos_, eventInfoBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, addTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 eventInfoLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref eventInfoLen);
				byte[] eventInfoBytes = new byte[eventInfoLen];
				for(int i = 0; i < eventInfoLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref eventInfoBytes[i]);
				}
				eventInfo = StringHelper.BytesToString(eventInfoBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref addTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] eventInfoBytes = StringHelper.StringToUTF8Bytes(eventInfo);
				BaseDLL.encode_string(buffer, ref pos_, eventInfoBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, addTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 eventInfoLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref eventInfoLen);
				byte[] eventInfoBytes = new byte[eventInfoLen];
				for(int i = 0; i < eventInfoLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref eventInfoBytes[i]);
				}
				eventInfo = StringHelper.BytesToString(eventInfoBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref addTime);
			}

			public int getLen()
			{
				int _len = 0;
				// eventInfo
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(eventInfo);
					_len += 2 + _strBytes.Length;
				}
				// addTime
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  创建公会
	/// </summary>
	[Protocol]
	public class WorldGuildCreateReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601901;
		public UInt32 Sequence;
		/// <summary>
		/// 公会名
		/// </summary>
		public string name;
		/// <summary>
		/// 宣言
		/// </summary>
		public string declaration;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
				BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
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
				UInt16 declarationLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref declarationLen);
				byte[] declarationBytes = new byte[declarationLen];
				for(int i = 0; i < declarationLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref declarationBytes[i]);
				}
				declaration = StringHelper.BytesToString(declarationBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
				BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
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
				UInt16 declarationLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref declarationLen);
				byte[] declarationBytes = new byte[declarationLen];
				for(int i = 0; i < declarationLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref declarationBytes[i]);
				}
				declaration = StringHelper.BytesToString(declarationBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// declaration
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(declaration);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  创建公会返回
	/// </summary>
	[Protocol]
	public class WorldGuildCreateRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601902;
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
	///  离开公会
	/// </summary>
	[Protocol]
	public class WorldGuildLeaveReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601903;
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
	///  离开公会返回
	/// </summary>
	[Protocol]
	public class WorldGuildLeaveRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601904;
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
	///  加入公会
	/// </summary>
	[Protocol]
	public class WorldGuildJoinReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601905;
		public UInt32 Sequence;
		public UInt64 id;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  加入公会返回
	/// </summary>
	[Protocol]
	public class WorldJoinGuildRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601906;
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
	///  请求公会列表
	/// </summary>
	[Protocol]
	public class WorldGuildListReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601907;
		public UInt32 Sequence;
		/// <summary>
		///  开始位置 0开始
		/// </summary>
		public UInt16 start;
		/// <summary>
		///  数量
		/// </summary>
		public UInt16 num;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, start);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref start);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, start);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref start);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// start
				_len += 2;
				// num
				_len += 2;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  返回公会列表
	/// </summary>
	[Protocol]
	public class WorldGuildListRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601908;
		public UInt32 Sequence;
		/// <summary>
		/// 开始位置
		/// </summary>
		public UInt16 start;
		/// <summary>
		/// 总数
		/// </summary>
		public UInt16 totalnum;
		/// <summary>
		/// 部落列表
		/// </summary>
		public GuildEntry[] guilds = new GuildEntry[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, start);
				BaseDLL.encode_uint16(buffer, ref pos_, totalnum);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)guilds.Length);
				for(int i = 0; i < guilds.Length; i++)
				{
					guilds[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref start);
				BaseDLL.decode_uint16(buffer, ref pos_, ref totalnum);
				UInt16 guildsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guildsCnt);
				guilds = new GuildEntry[guildsCnt];
				for(int i = 0; i < guilds.Length; i++)
				{
					guilds[i] = new GuildEntry();
					guilds[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, start);
				BaseDLL.encode_uint16(buffer, ref pos_, totalnum);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)guilds.Length);
				for(int i = 0; i < guilds.Length; i++)
				{
					guilds[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref start);
				BaseDLL.decode_uint16(buffer, ref pos_, ref totalnum);
				UInt16 guildsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guildsCnt);
				guilds = new GuildEntry[guildsCnt];
				for(int i = 0; i < guilds.Length; i++)
				{
					guilds[i] = new GuildEntry();
					guilds[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// start
				_len += 2;
				// totalnum
				_len += 2;
				// guilds
				_len += 2;
				for(int j = 0; j < guilds.Length; j++)
				{
					_len += guilds[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求申请入公会的列表
	/// </summary>
	[Protocol]
	public class WorldGuildRequesterReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601909;
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
	///  返回申请入公会的列表
	/// </summary>
	[Protocol]
	public class WorldGuildRequesterRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601910;
		public UInt32 Sequence;
		/// <summary>
		///  申请人列表
		/// </summary>
		public GuildRequesterInfo[] requesters = new GuildRequesterInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)requesters.Length);
				for(int i = 0; i < requesters.Length; i++)
				{
					requesters[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 requestersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref requestersCnt);
				requesters = new GuildRequesterInfo[requestersCnt];
				for(int i = 0; i < requesters.Length; i++)
				{
					requesters[i] = new GuildRequesterInfo();
					requesters[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)requesters.Length);
				for(int i = 0; i < requesters.Length; i++)
				{
					requesters[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 requestersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref requestersCnt);
				requesters = new GuildRequesterInfo[requestersCnt];
				for(int i = 0; i < requesters.Length; i++)
				{
					requesters[i] = new GuildRequesterInfo();
					requesters[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// requesters
				_len += 2;
				for(int j = 0; j < requesters.Length; j++)
				{
					_len += requesters[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  通知新的入部落请求
	/// </summary>
	[Protocol]
	public class WorldGuildNewRequester : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601911;
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
	///  处理公会成员请求
	/// </summary>
	[Protocol]
	public class WorldGuildProcessRequester : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601912;
		public UInt32 Sequence;
		/// <summary>
		/// id(如果是0代表清空列表)
		/// </summary>
		public UInt64 id;
		/// <summary>
		/// 同意进入(0:不同意，1:同意)
		/// </summary>
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
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, agree);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref agree);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, agree);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref agree);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// agree
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  处理公会加入请求返回
	/// </summary>
	[Protocol]
	public class WorldGuildProcessRequesterRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601913;
		public UInt32 Sequence;
		public UInt32 result;
		/// <summary>
		///  新成员信息
		/// </summary>
		public GuildMemberEntry entry = new GuildMemberEntry();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				entry.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				entry.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				entry.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				entry.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// entry
				_len += entry.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  任命职位
	/// </summary>
	[Protocol]
	public class WorldGuildChangePostReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601914;
		public UInt32 Sequence;
		/// <summary>
		/// id
		/// </summary>
		public UInt64 id;
		/// <summary>
		/// 职位
		/// </summary>
		public byte post;
		/// <summary>
		/// 被替换的人
		/// </summary>
		public UInt64 replacerId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_int8(buffer, ref pos_, post);
				BaseDLL.encode_uint64(buffer, ref pos_, replacerId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref post);
				BaseDLL.decode_uint64(buffer, ref pos_, ref replacerId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, post);
				BaseDLL.encode_uint64(buffer, ref pos_, replacerId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref post);
				BaseDLL.decode_uint64(buffer, ref pos_, ref replacerId);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// post
				_len += 1;
				// replacerId
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  任命职位返回
	/// </summary>
	[Protocol]
	public class WorldGuildChangePostRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601915;
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
	///  踢人
	/// </summary>
	[Protocol]
	public class WorldGuildKick : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601916;
		public UInt32 Sequence;
		public UInt64 id;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  踢人返回
	/// </summary>
	[Protocol]
	public class WorldGuildKickRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601917;
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
	///  上线或新加入公会发送初始数据
	/// </summary>
	[Protocol]
	public class WorldGuildSyncInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601918;
		public UInt32 Sequence;
		/// <summary>
		///  公会基础信息
		/// </summary>
		public GuildBaseInfo info = new GuildBaseInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
	///  请求公会成员列表
	/// </summary>
	[Protocol]
	public class WorldGuildMemberListReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601919;
		public UInt32 Sequence;
		/// <summary>
		/// 0为查询本行会成员 别的值为查询别的行会成员，仅供公会兼并使用
		/// </summary>
		public UInt64 guildID;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guildID);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guildID);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guildID);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guildID);
			}

			public int getLen()
			{
				int _len = 0;
				// guildID
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  返回公会成员列表
	/// </summary>
	[Protocol]
	public class WorldGuildMemberListRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601920;
		public UInt32 Sequence;
		/// <summary>
		///  成员列表
		/// </summary>
		public GuildMemberEntry[] members = new GuildMemberEntry[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)members.Length);
				for(int i = 0; i < members.Length; i++)
				{
					members[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 membersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref membersCnt);
				members = new GuildMemberEntry[membersCnt];
				for(int i = 0; i < members.Length; i++)
				{
					members[i] = new GuildMemberEntry();
					members[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)members.Length);
				for(int i = 0; i < members.Length; i++)
				{
					members[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 membersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref membersCnt);
				members = new GuildMemberEntry[membersCnt];
				for(int i = 0; i < members.Length; i++)
				{
					members[i] = new GuildMemberEntry();
					members[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// members
				_len += 2;
				for(int j = 0; j < members.Length; j++)
				{
					_len += members[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  修改公会宣言
	/// </summary>
	[Protocol]
	public class WorldGuildModifyDeclaration : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601921;
		public UInt32 Sequence;
		public string declaration;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
				BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 declarationLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref declarationLen);
				byte[] declarationBytes = new byte[declarationLen];
				for(int i = 0; i < declarationLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref declarationBytes[i]);
				}
				declaration = StringHelper.BytesToString(declarationBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
				BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 declarationLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref declarationLen);
				byte[] declarationBytes = new byte[declarationLen];
				for(int i = 0; i < declarationLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref declarationBytes[i]);
				}
				declaration = StringHelper.BytesToString(declarationBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// declaration
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(declaration);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  修改公会名
	/// </summary>
	[Protocol]
	public class WorldGuildModifyName : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601922;
		public UInt32 Sequence;
		public string name;
		public UInt64 itemGUID;
		public UInt32 itemTableID;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint64(buffer, ref pos_, itemGUID);
				BaseDLL.encode_uint32(buffer, ref pos_, itemTableID);
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
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemGUID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTableID);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint64(buffer, ref pos_, itemGUID);
				BaseDLL.encode_uint32(buffer, ref pos_, itemTableID);
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
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemGUID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTableID);
			}

			public int getLen()
			{
				int _len = 0;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// itemGUID
				_len += 8;
				// itemTableID
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  修改公会公告
	/// </summary>
	[Protocol]
	public class WorldGuildModifyAnnouncement : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601923;
		public UInt32 Sequence;
		public string content;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] contentBytes = StringHelper.StringToUTF8Bytes(content);
				BaseDLL.encode_string(buffer, ref pos_, contentBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 contentLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref contentLen);
				byte[] contentBytes = new byte[contentLen];
				for(int i = 0; i < contentLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref contentBytes[i]);
				}
				content = StringHelper.BytesToString(contentBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] contentBytes = StringHelper.StringToUTF8Bytes(content);
				BaseDLL.encode_string(buffer, ref pos_, contentBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 contentLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref contentLen);
				byte[] contentBytes = new byte[contentLen];
				for(int i = 0; i < contentLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref contentBytes[i]);
				}
				content = StringHelper.BytesToString(contentBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// content
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(content);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  发送公会邮件
	/// </summary>
	[Protocol]
	public class WorldGuildSendMail : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601924;
		public UInt32 Sequence;
		public string content;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] contentBytes = StringHelper.StringToUTF8Bytes(content);
				BaseDLL.encode_string(buffer, ref pos_, contentBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 contentLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref contentLen);
				byte[] contentBytes = new byte[contentLen];
				for(int i = 0; i < contentLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref contentBytes[i]);
				}
				content = StringHelper.BytesToString(contentBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] contentBytes = StringHelper.StringToUTF8Bytes(content);
				BaseDLL.encode_string(buffer, ref pos_, contentBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 contentLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref contentLen);
				byte[] contentBytes = new byte[contentLen];
				for(int i = 0; i < contentLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref contentBytes[i]);
				}
				content = StringHelper.BytesToString(contentBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// content
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(content);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  同步公会修改信息(使用流的方式同步)
	/// </summary>
	[Protocol]
	public class WorldGuildSyncStreamInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601925;
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
	///  帮会通用操作返回
	/// </summary>
	[Protocol]
	public class WorldGuildOperRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601926;
		public UInt32 Sequence;
		/// <summary>
		///  操作类型（对应枚举GuildOperation）
		/// </summary>
		public byte type;
		/// <summary>
		///  结果
		/// </summary>
		public UInt32 result;
		/// <summary>
		///  参数1
		/// </summary>
		public UInt32 param;
		/// <summary>
		///  参数2
		/// </summary>
		public UInt64 param2;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, param);
				BaseDLL.encode_uint64(buffer, ref pos_, param2);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param);
				BaseDLL.decode_uint64(buffer, ref pos_, ref param2);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, param);
				BaseDLL.encode_uint64(buffer, ref pos_, param2);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param);
				BaseDLL.decode_uint64(buffer, ref pos_, ref param2);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// result
				_len += 4;
				// param
				_len += 4;
				// param2
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  升级建筑
	/// </summary>
	[Protocol]
	public class WorldGuildUpgradeBuilding : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601927;
		public UInt32 Sequence;
		/// <summary>
		///  建筑类型（对应枚举GuildBuildingType）
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

	/// <summary>
	///  请求捐赠
	/// </summary>
	[Protocol]
	public class WorldGuildDonateReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601928;
		public UInt32 Sequence;
		/// <summary>
		///  捐赠类型（对应枚举GuildDonateType）
		/// </summary>
		public byte type;
		/// <summary>
		///  次数
		/// </summary>
		public byte num;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_int8(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// num
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求捐赠日志
	/// </summary>
	[Protocol]
	public class WorldGuildDonateLogReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601929;
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
	///  返回捐赠日志
	/// </summary>
	[Protocol]
	public class WorldGuildDonateLogRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601930;
		public UInt32 Sequence;
		public GuildDonateLog[] logs = new GuildDonateLog[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)logs.Length);
				for(int i = 0; i < logs.Length; i++)
				{
					logs[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 logsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref logsCnt);
				logs = new GuildDonateLog[logsCnt];
				for(int i = 0; i < logs.Length; i++)
				{
					logs[i] = new GuildDonateLog();
					logs[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)logs.Length);
				for(int i = 0; i < logs.Length; i++)
				{
					logs[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 logsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref logsCnt);
				logs = new GuildDonateLog[logsCnt];
				for(int i = 0; i < logs.Length; i++)
				{
					logs[i] = new GuildDonateLog();
					logs[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// logs
				_len += 2;
				for(int j = 0; j < logs.Length; j++)
				{
					_len += logs[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  升级技能
	/// </summary>
	[Protocol]
	public class WorldGuildUpgradeSkill : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601931;
		public UInt32 Sequence;
		/// <summary>
		///  技能id
		/// </summary>
		public UInt16 skillId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, skillId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref skillId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, skillId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref skillId);
			}

			public int getLen()
			{
				int _len = 0;
				// skillId
				_len += 2;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  解散公会
	/// </summary>
	[Protocol]
	public class WorldGuildDismissReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601932;
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
	///  取消解散公会
	/// </summary>
	[Protocol]
	public class WorldGuildCancelDismissReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601933;
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
	///  请求会长信息
	/// </summary>
	[Protocol]
	public class WorldGuildLeaderInfoReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601934;
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
	///  返回会长信息
	/// </summary>
	[Protocol]
	public class WorldGuildLeaderInfoRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601935;
		public UInt32 Sequence;
		public GuildLeaderInfo info = new GuildLeaderInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
	///  请求膜拜
	/// </summary>
	[Protocol]
	public class WorldGuildOrzReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601936;
		public UInt32 Sequence;
		/// <summary>
		///  膜拜类型，对应枚举（GuildOrzType）
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

	/// <summary>
	///  请求加入圆桌会议
	/// </summary>
	[Protocol]
	public class WorldGuildTableJoinReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601937;
		public UInt32 Sequence;
		/// <summary>
		///  位置
		/// </summary>
		public byte seat;
		/// <summary>
		///  是不是协助
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
				BaseDLL.encode_int8(buffer, ref pos_, seat);
				BaseDLL.encode_int8(buffer, ref pos_, type);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref seat);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, seat);
				BaseDLL.encode_int8(buffer, ref pos_, type);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref seat);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
			}

			public int getLen()
			{
				int _len = 0;
				// seat
				_len += 1;
				// type
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  通知客户端有新的圆桌会议成员
	/// </summary>
	[Protocol]
	public class WorldGuildTableNewMember : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601938;
		public UInt32 Sequence;
		public GuildTableMember member = new GuildTableMember();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				member.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				member.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				member.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				member.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// member
				_len += member.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  通知客户端删除圆桌会议成员
	/// </summary>
	[Protocol]
	public class WorldGuildTableDelMember : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601939;
		public UInt32 Sequence;
		public UInt64 id;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  通知客户端的圆桌会议完成
	/// </summary>
	[Protocol]
	public class WorldGuildTableFinish : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601940;
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
	///  请求发自费红包
	/// </summary>
	[Protocol]
	public class WorldGuildPayRedPacketReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601941;
		public UInt32 Sequence;
		/// <summary>
		///  来源
		/// </summary>
		public UInt16 reason;
		/// <summary>
		///  名字
		/// </summary>
		public string name;
		/// <summary>
		///  数量
		/// </summary>
		public byte num;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, reason);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref reason);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, reason);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref reason);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// reason
				_len += 2;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// num
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  公会兑换
	/// </summary>
	[Protocol]
	public class SceneGuildExchangeReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501901;
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
	///  请求公会战报名
	/// </summary>
	[Protocol]
	public class WorldGuildBattleReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601942;
		public UInt32 Sequence;
		public byte terrId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, terrId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref terrId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, terrId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref terrId);
			}

			public int getLen()
			{
				int _len = 0;
				// terrId
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求公会战返回
	/// </summary>
	[Protocol]
	public class WorldGuildBattleRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601943;
		public UInt32 Sequence;
		public UInt32 result;
		public byte terrId;
		public UInt32 enrollSize;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_int8(buffer, ref pos_, terrId);
				BaseDLL.encode_uint32(buffer, ref pos_, enrollSize);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_int8(buffer, ref pos_, ref terrId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref enrollSize);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_int8(buffer, ref pos_, terrId);
				BaseDLL.encode_uint32(buffer, ref pos_, enrollSize);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_int8(buffer, ref pos_, ref terrId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref enrollSize);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// terrId
				_len += 1;
				// enrollSize
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求鼓舞
	/// </summary>
	[Protocol]
	public class WorldGuildBattleInspireReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601944;
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
	///  鼓舞返回
	/// </summary>
	[Protocol]
	public class WorldGuildBattleInspireRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601945;
		public UInt32 Sequence;
		public UInt32 result;
		public byte inspire;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_int8(buffer, ref pos_, inspire);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_int8(buffer, ref pos_, ref inspire);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_int8(buffer, ref pos_, inspire);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_int8(buffer, ref pos_, ref inspire);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// inspire
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求领取奖励
	/// </summary>
	[Protocol]
	public class WorldGuildBattleReceiveReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601946;
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

	/// <summary>
	///  领取奖励返回
	/// </summary>
	[Protocol]
	public class WorldGuildBattleReceiveRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601947;
		public UInt32 Sequence;
		public UInt32 result;
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
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_int8(buffer, ref pos_, boxId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_int8(buffer, ref pos_, ref boxId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_int8(buffer, ref pos_, boxId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_int8(buffer, ref pos_, ref boxId);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// boxId
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求领地战斗记录
	/// </summary>
	[Protocol]
	public class WorldGuildBattleRecordReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601948;
		public UInt32 Sequence;
		public byte isSelf;
		public Int32 startIndex;
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
				BaseDLL.encode_int32(buffer, ref pos_, startIndex);
				BaseDLL.encode_uint32(buffer, ref pos_, count);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isSelf);
				BaseDLL.decode_int32(buffer, ref pos_, ref startIndex);
				BaseDLL.decode_uint32(buffer, ref pos_, ref count);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, isSelf);
				BaseDLL.encode_int32(buffer, ref pos_, startIndex);
				BaseDLL.encode_uint32(buffer, ref pos_, count);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isSelf);
				BaseDLL.decode_int32(buffer, ref pos_, ref startIndex);
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
	///  领地战斗记录返回
	/// </summary>
	[Protocol]
	public class WorldGuildBattleRecordRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601949;
		public UInt32 Sequence;
		public UInt32 result;
		public GuildBattleRecord[] records = new GuildBattleRecord[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)records.Length);
				for(int i = 0; i < records.Length; i++)
				{
					records[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 recordsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref recordsCnt);
				records = new GuildBattleRecord[recordsCnt];
				for(int i = 0; i < records.Length; i++)
				{
					records[i] = new GuildBattleRecord();
					records[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)records.Length);
				for(int i = 0; i < records.Length; i++)
				{
					records[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 recordsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref recordsCnt);
				records = new GuildBattleRecord[recordsCnt];
				for(int i = 0; i < records.Length; i++)
				{
					records[i] = new GuildBattleRecord();
					records[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// result
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
	///  领地战斗记录同步
	/// </summary>
	[Protocol]
	public class WorldGuildBattleRecordSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601950;
		public UInt32 Sequence;
		public GuildBattleRecord record = new GuildBattleRecord();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
	///  请求领地信息
	/// </summary>
	[Protocol]
	public class WorldGuildBattleTerritoryReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601951;
		public UInt32 Sequence;
		public byte terrId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, terrId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref terrId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, terrId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref terrId);
			}

			public int getLen()
			{
				int _len = 0;
				// terrId
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  返回领地信息
	/// </summary>
	[Protocol]
	public class WorldGuildBattleTerritoryRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601952;
		public UInt32 Sequence;
		public UInt32 result;
		public GuildTerritoryBaseInfo info = new GuildTerritoryBaseInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				info.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				info.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				info.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				info.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// info
				_len += info.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  单次战斗结束
	/// </summary>
	[Protocol]
	public class WorldGuildBattleRaceEnd : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601953;
		public UInt32 Sequence;
		public byte result;
		public UInt32 oldScore;
		public UInt32 newScore;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, oldScore);
				BaseDLL.encode_uint32(buffer, ref pos_, newScore);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newScore);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, oldScore);
				BaseDLL.encode_uint32(buffer, ref pos_, newScore);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref oldScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref newScore);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 1;
				// oldScore
				_len += 4;
				// newScore
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  公会战结束
	/// </summary>
	[Protocol]
	public class WorldGuildBattleEnd : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601954;
		public UInt32 Sequence;
		public GuildBattleEndInfo[] info = new GuildBattleEndInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)info.Length);
				for(int i = 0; i < info.Length; i++)
				{
					info[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 infoCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref infoCnt);
				info = new GuildBattleEndInfo[infoCnt];
				for(int i = 0; i < info.Length; i++)
				{
					info[i] = new GuildBattleEndInfo();
					info[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)info.Length);
				for(int i = 0; i < info.Length; i++)
				{
					info[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 infoCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref infoCnt);
				info = new GuildBattleEndInfo[infoCnt];
				for(int i = 0; i < info.Length; i++)
				{
					info[i] = new GuildBattleEndInfo();
					info[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// info
				_len += 2;
				for(int j = 0; j < info.Length; j++)
				{
					_len += info[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求自身公会排行
	/// </summary>
	[Protocol]
	public class WorldGuildBattleSelfSortListReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601955;
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
	///  请求自身公会排行响应
	/// </summary>
	[Protocol]
	public class WorldGuildBattleSelfSortListRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601956;
		public UInt32 Sequence;
		public UInt32 result;
		public UInt32 memberRanking;
		public UInt32 guildRanking;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint32(buffer, ref pos_, memberRanking);
				BaseDLL.encode_uint32(buffer, ref pos_, guildRanking);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref memberRanking);
				BaseDLL.decode_uint32(buffer, ref pos_, ref guildRanking);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, memberRanking);
				BaseDLL.encode_uint32(buffer, ref pos_, guildRanking);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref memberRanking);
				BaseDLL.decode_uint32(buffer, ref pos_, ref guildRanking);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// memberRanking
				_len += 4;
				// guildRanking
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  公会邀请通知，有别的玩家邀请加入公会
	/// </summary>
	[Protocol]
	public class WorldGuildInviteNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601957;
		public UInt32 Sequence;
		/// <summary>
		///  邀请者ID
		/// </summary>
		public UInt64 inviterId;
		/// <summary>
		///  邀请者名字
		/// </summary>
		public string inviterName;
		/// <summary>
		///  邀请者职业
		/// </summary>
		public byte inviterOccu;
		/// <summary>
		///  邀请者等级
		/// </summary>
		public UInt16 inviterLevel;
		/// <summary>
		///  邀请者VIP等级
		/// </summary>
		public byte inviterVipLevel;
		/// <summary>
		///  公会ID
		/// </summary>
		public UInt64 guildId;
		/// <summary>
		///  公会名
		/// </summary>
		public string guildName;
		/// <summary>
		///  玩家标签信息
		/// </summary>
		public PlayerLabelInfo playerLabelInfo = new PlayerLabelInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, inviterId);
				byte[] inviterNameBytes = StringHelper.StringToUTF8Bytes(inviterName);
				BaseDLL.encode_string(buffer, ref pos_, inviterNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, inviterOccu);
				BaseDLL.encode_uint16(buffer, ref pos_, inviterLevel);
				BaseDLL.encode_int8(buffer, ref pos_, inviterVipLevel);
				BaseDLL.encode_uint64(buffer, ref pos_, guildId);
				byte[] guildNameBytes = StringHelper.StringToUTF8Bytes(guildName);
				BaseDLL.encode_string(buffer, ref pos_, guildNameBytes, (UInt16)(buffer.Length - pos_));
				playerLabelInfo.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref inviterId);
				UInt16 inviterNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref inviterNameLen);
				byte[] inviterNameBytes = new byte[inviterNameLen];
				for(int i = 0; i < inviterNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref inviterNameBytes[i]);
				}
				inviterName = StringHelper.BytesToString(inviterNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref inviterOccu);
				BaseDLL.decode_uint16(buffer, ref pos_, ref inviterLevel);
				BaseDLL.decode_int8(buffer, ref pos_, ref inviterVipLevel);
				BaseDLL.decode_uint64(buffer, ref pos_, ref guildId);
				UInt16 guildNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guildNameLen);
				byte[] guildNameBytes = new byte[guildNameLen];
				for(int i = 0; i < guildNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref guildNameBytes[i]);
				}
				guildName = StringHelper.BytesToString(guildNameBytes);
				playerLabelInfo.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, inviterId);
				byte[] inviterNameBytes = StringHelper.StringToUTF8Bytes(inviterName);
				BaseDLL.encode_string(buffer, ref pos_, inviterNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, inviterOccu);
				BaseDLL.encode_uint16(buffer, ref pos_, inviterLevel);
				BaseDLL.encode_int8(buffer, ref pos_, inviterVipLevel);
				BaseDLL.encode_uint64(buffer, ref pos_, guildId);
				byte[] guildNameBytes = StringHelper.StringToUTF8Bytes(guildName);
				BaseDLL.encode_string(buffer, ref pos_, guildNameBytes, (UInt16)(buffer.Length - pos_));
				playerLabelInfo.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref inviterId);
				UInt16 inviterNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref inviterNameLen);
				byte[] inviterNameBytes = new byte[inviterNameLen];
				for(int i = 0; i < inviterNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref inviterNameBytes[i]);
				}
				inviterName = StringHelper.BytesToString(inviterNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref inviterOccu);
				BaseDLL.decode_uint16(buffer, ref pos_, ref inviterLevel);
				BaseDLL.decode_int8(buffer, ref pos_, ref inviterVipLevel);
				BaseDLL.decode_uint64(buffer, ref pos_, ref guildId);
				UInt16 guildNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guildNameLen);
				byte[] guildNameBytes = new byte[guildNameLen];
				for(int i = 0; i < guildNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref guildNameBytes[i]);
				}
				guildName = StringHelper.BytesToString(guildNameBytes);
				playerLabelInfo.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// inviterId
				_len += 8;
				// inviterName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(inviterName);
					_len += 2 + _strBytes.Length;
				}
				// inviterOccu
				_len += 1;
				// inviterLevel
				_len += 2;
				// inviterVipLevel
				_len += 1;
				// guildId
				_len += 8;
				// guildName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(guildName);
					_len += 2 + _strBytes.Length;
				}
				// playerLabelInfo
				_len += playerLabelInfo.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  同步公会战状态
	/// </summary>
	[Protocol]
	public class WorldGuildBattleStatusSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601958;
		public UInt32 Sequence;
		/// <summary>
		///  类型
		/// </summary>
		public byte type;
		/// <summary>
		///  状态
		/// </summary>
		public byte status;
		/// <summary>
		///  状态存在时间
		/// </summary>
		public UInt32 time;
		/// <summary>
		///  公会战结束信息
		/// </summary>
		public GuildBattleEndInfo[] endInfo = new GuildBattleEndInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_int8(buffer, ref pos_, status);
				BaseDLL.encode_uint32(buffer, ref pos_, time);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)endInfo.Length);
				for(int i = 0; i < endInfo.Length; i++)
				{
					endInfo[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
				BaseDLL.decode_uint32(buffer, ref pos_, ref time);
				UInt16 endInfoCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref endInfoCnt);
				endInfo = new GuildBattleEndInfo[endInfoCnt];
				for(int i = 0; i < endInfo.Length; i++)
				{
					endInfo[i] = new GuildBattleEndInfo();
					endInfo[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, status);
				BaseDLL.encode_uint32(buffer, ref pos_, time);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)endInfo.Length);
				for(int i = 0; i < endInfo.Length; i++)
				{
					endInfo[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
				BaseDLL.decode_uint32(buffer, ref pos_, ref time);
				UInt16 endInfoCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref endInfoCnt);
				endInfo = new GuildBattleEndInfo[endInfoCnt];
				for(int i = 0; i < endInfo.Length; i++)
				{
					endInfo[i] = new GuildBattleEndInfo();
					endInfo[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// status
				_len += 1;
				// time
				_len += 4;
				// endInfo
				_len += 2;
				for(int j = 0; j < endInfo.Length; j++)
				{
					_len += endInfo[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求公会宣战报名
	/// </summary>
	[Protocol]
	public class WorldGuildChallengeReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601959;
		public UInt32 Sequence;
		public byte terrId;
		public UInt32 itemId;
		public UInt32 itemNum;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, terrId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref terrId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, terrId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref terrId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemNum);
			}

			public int getLen()
			{
				int _len = 0;
				// terrId
				_len += 1;
				// itemId
				_len += 4;
				// itemNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  返回公会宣战报名
	/// </summary>
	[Protocol]
	public class WorldGuildChallengeRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601960;
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
	///  请求公会宣战信息
	/// </summary>
	[Protocol]
	public class WorldGuildChallengeInfoReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601961;
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
	///  公会宣战信息同步
	/// </summary>
	[Protocol]
	public class WorldGuildChallengeInfoSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601962;
		public UInt32 Sequence;
		public GuildTerritoryBaseInfo info = new GuildTerritoryBaseInfo();
		public UInt64 enrollGuildId;
		public string enrollGuildName;
		public string enrollGuildleaderName;
		public byte enrollGuildLevel;
		public UInt32 itemId;
		public UInt32 itemNum;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint64(buffer, ref pos_, enrollGuildId);
				byte[] enrollGuildNameBytes = StringHelper.StringToUTF8Bytes(enrollGuildName);
				BaseDLL.encode_string(buffer, ref pos_, enrollGuildNameBytes, (UInt16)(buffer.Length - pos_));
				byte[] enrollGuildleaderNameBytes = StringHelper.StringToUTF8Bytes(enrollGuildleaderName);
				BaseDLL.encode_string(buffer, ref pos_, enrollGuildleaderNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, enrollGuildLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				info.decode(buffer, ref pos_);
				BaseDLL.decode_uint64(buffer, ref pos_, ref enrollGuildId);
				UInt16 enrollGuildNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref enrollGuildNameLen);
				byte[] enrollGuildNameBytes = new byte[enrollGuildNameLen];
				for(int i = 0; i < enrollGuildNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref enrollGuildNameBytes[i]);
				}
				enrollGuildName = StringHelper.BytesToString(enrollGuildNameBytes);
				UInt16 enrollGuildleaderNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref enrollGuildleaderNameLen);
				byte[] enrollGuildleaderNameBytes = new byte[enrollGuildleaderNameLen];
				for(int i = 0; i < enrollGuildleaderNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref enrollGuildleaderNameBytes[i]);
				}
				enrollGuildleaderName = StringHelper.BytesToString(enrollGuildleaderNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref enrollGuildLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				info.encode(buffer, ref pos_);
				BaseDLL.encode_uint64(buffer, ref pos_, enrollGuildId);
				byte[] enrollGuildNameBytes = StringHelper.StringToUTF8Bytes(enrollGuildName);
				BaseDLL.encode_string(buffer, ref pos_, enrollGuildNameBytes, (UInt16)(buffer.Length - pos_));
				byte[] enrollGuildleaderNameBytes = StringHelper.StringToUTF8Bytes(enrollGuildleaderName);
				BaseDLL.encode_string(buffer, ref pos_, enrollGuildleaderNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, enrollGuildLevel);
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				info.decode(buffer, ref pos_);
				BaseDLL.decode_uint64(buffer, ref pos_, ref enrollGuildId);
				UInt16 enrollGuildNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref enrollGuildNameLen);
				byte[] enrollGuildNameBytes = new byte[enrollGuildNameLen];
				for(int i = 0; i < enrollGuildNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref enrollGuildNameBytes[i]);
				}
				enrollGuildName = StringHelper.BytesToString(enrollGuildNameBytes);
				UInt16 enrollGuildleaderNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref enrollGuildleaderNameLen);
				byte[] enrollGuildleaderNameBytes = new byte[enrollGuildleaderNameLen];
				for(int i = 0; i < enrollGuildleaderNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref enrollGuildleaderNameBytes[i]);
				}
				enrollGuildleaderName = StringHelper.BytesToString(enrollGuildleaderNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref enrollGuildLevel);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemNum);
			}

			public int getLen()
			{
				int _len = 0;
				// info
				_len += info.getLen();
				// enrollGuildId
				_len += 8;
				// enrollGuildName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(enrollGuildName);
					_len += 2 + _strBytes.Length;
				}
				// enrollGuildleaderName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(enrollGuildleaderName);
					_len += 2 + _strBytes.Length;
				}
				// enrollGuildLevel
				_len += 1;
				// itemId
				_len += 4;
				// itemNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求公会战鼓舞信息
	/// </summary>
	[Protocol]
	public class WorldGuildBattleInspireInfoReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601963;
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
	///  返回公会战鼓舞信息
	/// </summary>
	[Protocol]
	public class WorldGuildBattleInspireInfoRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601964;
		public UInt32 Sequence;
		public UInt32 result;
		/// <summary>
		///  领地ID
		/// </summary>
		public byte terrId;
		/// <summary>
		///  鼓舞信息
		/// </summary>
		public GuildBattleInspireInfo[] inspireInfos = new GuildBattleInspireInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_int8(buffer, ref pos_, terrId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)inspireInfos.Length);
				for(int i = 0; i < inspireInfos.Length; i++)
				{
					inspireInfos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_int8(buffer, ref pos_, ref terrId);
				UInt16 inspireInfosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref inspireInfosCnt);
				inspireInfos = new GuildBattleInspireInfo[inspireInfosCnt];
				for(int i = 0; i < inspireInfos.Length; i++)
				{
					inspireInfos[i] = new GuildBattleInspireInfo();
					inspireInfos[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_int8(buffer, ref pos_, terrId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)inspireInfos.Length);
				for(int i = 0; i < inspireInfos.Length; i++)
				{
					inspireInfos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_int8(buffer, ref pos_, ref terrId);
				UInt16 inspireInfosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref inspireInfosCnt);
				inspireInfos = new GuildBattleInspireInfo[inspireInfosCnt];
				for(int i = 0; i < inspireInfos.Length; i++)
				{
					inspireInfos[i] = new GuildBattleInspireInfo();
					inspireInfos[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// terrId
				_len += 1;
				// inspireInfos
				_len += 2;
				for(int j = 0; j < inspireInfos.Length; j++)
				{
					_len += inspireInfos[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求公会仓库设置
	/// </summary>
	[Protocol]
	public class WorldGuildStorageSettingReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601965;
		public UInt32 Sequence;
		public byte type;
		public UInt32 value;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
	///  返回公会仓库设置
	/// </summary>
	[Protocol]
	public class WorldGuildStorageSettingRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601966;
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
	///  请求公会战抽奖
	/// </summary>
	[Protocol]
	public class WorldGuildBattleLotteryReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601967;
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
	///  返回公会战抽奖
	/// </summary>
	[Protocol]
	public class WorldGuildBattleLotteryRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601968;
		public UInt32 Sequence;
		public UInt32 result;
		public UInt32 contribution;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint32(buffer, ref pos_, contribution);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref contribution);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, contribution);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref contribution);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// contribution
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求公会仓库列表
	/// </summary>
	[Protocol]
	public class WorldGuildStorageListReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601969;
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
	///  返回公会仓库列表
	/// </summary>
	[Protocol]
	public class WorldGuildStorageListRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601970;
		public UInt32 Sequence;
		public UInt32 result;
		public UInt32 maxSize;
		public GuildStorageItemInfo[] items = new GuildStorageItemInfo[0];
		public GuildStorageOpRecord[] itemRecords = new GuildStorageOpRecord[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint32(buffer, ref pos_, maxSize);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemRecords.Length);
				for(int i = 0; i < itemRecords.Length; i++)
				{
					itemRecords[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref maxSize);
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new GuildStorageItemInfo[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new GuildStorageItemInfo();
					items[i].decode(buffer, ref pos_);
				}
				UInt16 itemRecordsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemRecordsCnt);
				itemRecords = new GuildStorageOpRecord[itemRecordsCnt];
				for(int i = 0; i < itemRecords.Length; i++)
				{
					itemRecords[i] = new GuildStorageOpRecord();
					itemRecords[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, maxSize);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemRecords.Length);
				for(int i = 0; i < itemRecords.Length; i++)
				{
					itemRecords[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref maxSize);
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new GuildStorageItemInfo[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new GuildStorageItemInfo();
					items[i].decode(buffer, ref pos_);
				}
				UInt16 itemRecordsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemRecordsCnt);
				itemRecords = new GuildStorageOpRecord[itemRecordsCnt];
				for(int i = 0; i < itemRecords.Length; i++)
				{
					itemRecords[i] = new GuildStorageOpRecord();
					itemRecords[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// maxSize
				_len += 4;
				// items
				_len += 2;
				for(int j = 0; j < items.Length; j++)
				{
					_len += items[j].getLen();
				}
				// itemRecords
				_len += 2;
				for(int j = 0; j < itemRecords.Length; j++)
				{
					_len += itemRecords[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  同步仓库物品数据
	/// </summary>
	[Protocol]
	public class WorldGuildStorageItemSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601971;
		public UInt32 Sequence;
		public GuildStorageItemInfo[] items = new GuildStorageItemInfo[0];
		public GuildStorageOpRecord[] records = new GuildStorageOpRecord[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)records.Length);
				for(int i = 0; i < records.Length; i++)
				{
					records[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new GuildStorageItemInfo[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new GuildStorageItemInfo();
					items[i].decode(buffer, ref pos_);
				}
				UInt16 recordsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref recordsCnt);
				records = new GuildStorageOpRecord[recordsCnt];
				for(int i = 0; i < records.Length; i++)
				{
					records[i] = new GuildStorageOpRecord();
					records[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)records.Length);
				for(int i = 0; i < records.Length; i++)
				{
					records[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new GuildStorageItemInfo[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new GuildStorageItemInfo();
					items[i].decode(buffer, ref pos_);
				}
				UInt16 recordsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref recordsCnt);
				records = new GuildStorageOpRecord[recordsCnt];
				for(int i = 0; i < records.Length; i++)
				{
					records[i] = new GuildStorageOpRecord();
					records[i].decode(buffer, ref pos_);
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
	///  请求放入公会仓库
	/// </summary>
	[Protocol]
	public class WorldGuildAddStorageReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601972;
		public UInt32 Sequence;
		public GuildStorageItemInfo[] items = new GuildStorageItemInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				items = new GuildStorageItemInfo[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new GuildStorageItemInfo();
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
				items = new GuildStorageItemInfo[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new GuildStorageItemInfo();
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

	/// <summary>
	///  返回放入公会仓库
	/// </summary>
	[Protocol]
	public class WorldGuildAddStorageRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601973;
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
	///  请求删除公会仓库物品
	/// </summary>
	[Protocol]
	public class WorldGuildDelStorageReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601974;
		public UInt32 Sequence;
		public GuildStorageDelItemInfo[] items = new GuildStorageDelItemInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				items = new GuildStorageDelItemInfo[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new GuildStorageDelItemInfo();
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
				items = new GuildStorageDelItemInfo[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new GuildStorageDelItemInfo();
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

	/// <summary>
	///  返回删除公会仓库物品
	/// </summary>
	[Protocol]
	public class WorldGuildDelStorageRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601975;
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
	///  查看仓库物品详情
	/// </summary>
	[Protocol]
	public class WorldWatchGuildStorageItemReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601976;
		public UInt32 Sequence;
		public UInt64 uid;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
			}

			public int getLen()
			{
				int _len = 0;
				// uid
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  雕像同步
	/// </summary>
	[Protocol]
	public class WorldFigureStatueSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 600108;
		public UInt32 Sequence;
		public FigureStatueInfo[] figureStatue = new FigureStatueInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)figureStatue.Length);
				for(int i = 0; i < figureStatue.Length; i++)
				{
					figureStatue[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 figureStatueCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref figureStatueCnt);
				figureStatue = new FigureStatueInfo[figureStatueCnt];
				for(int i = 0; i < figureStatue.Length; i++)
				{
					figureStatue[i] = new FigureStatueInfo();
					figureStatue[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)figureStatue.Length);
				for(int i = 0; i < figureStatue.Length; i++)
				{
					figureStatue[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 figureStatueCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref figureStatueCnt);
				figureStatue = new FigureStatueInfo[figureStatueCnt];
				for(int i = 0; i < figureStatue.Length; i++)
				{
					figureStatue[i] = new FigureStatueInfo();
					figureStatue[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// figureStatue
				_len += 2;
				for(int j = 0; j < figureStatue.Length; j++)
				{
					_len += figureStatue[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  设置加入公会等级请求
	/// </summary>
	[Protocol]
	public class WorldGuildSetJoinLevelReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601988;
		public UInt32 Sequence;
		/// <summary>
		///  加入等级
		/// </summary>
		public UInt32 joinLevel;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, joinLevel);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref joinLevel);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, joinLevel);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref joinLevel);
			}

			public int getLen()
			{
				int _len = 0;
				// joinLevel
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  设置加入公会等级返回
	/// </summary>
	[Protocol]
	public class WorldGuildSetJoinLevelRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601989;
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
	///  公会徽记升级请求
	/// </summary>
	[Protocol]
	public class WorldGuildEmblemUpReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601990;
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
	///  公会徽记升级返回
	/// </summary>
	[Protocol]
	public class WorldGuildEmblemUpRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601991;
		public UInt32 Sequence;
		public UInt32 result;
		public UInt32 emblemLevel;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint32(buffer, ref pos_, emblemLevel);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref emblemLevel);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, emblemLevel);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref emblemLevel);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// emblemLevel
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  设置公会副本难度请求
	/// </summary>
	[Protocol]
	public class WorldGuildSetDungeonTypeReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601992;
		public UInt32 Sequence;
		public UInt32 dungeonType;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonType);
			}

			public int getLen()
			{
				int _len = 0;
				// dungeonType
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  设置公会副本难度返回
	/// </summary>
	[Protocol]
	public class WorldGuildSetDungeonTypeRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601993;
		public UInt32 Sequence;
		public UInt32 result;
		public UInt32 dungeonType;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonType);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// dungeonType
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  公会领地每日奖励请求
	/// </summary>
	[Protocol]
	public class WorldGuildGetTerrDayRewardReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601994;
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
	///  公会领地每日奖励返回
	/// </summary>
	[Protocol]
	public class WorldGuildGetTerrDayRewardRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601995;
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
	///  公会地下城信息请求
	/// </summary>
	[Protocol]
	public class WorldGuildDungeonInfoReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608501;
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
	///  boss血量信息
	/// </summary>
	public class GuildDungeonBossBlood : Protocol.IProtocolStream
	{
		/// <summary>
		///  地下城id
		/// </summary>
		public UInt32 dungeonId;
		/// <summary>
		///  剩余血量
		/// </summary>
		public UInt64 oddBlood;
		/// <summary>
		///  待验证血量
		/// </summary>
		public UInt64 verifyBlood;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
				BaseDLL.encode_uint64(buffer, ref pos_, oddBlood);
				BaseDLL.encode_uint64(buffer, ref pos_, verifyBlood);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref oddBlood);
				BaseDLL.decode_uint64(buffer, ref pos_, ref verifyBlood);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
				BaseDLL.encode_uint64(buffer, ref pos_, oddBlood);
				BaseDLL.encode_uint64(buffer, ref pos_, verifyBlood);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref oddBlood);
				BaseDLL.decode_uint64(buffer, ref pos_, ref verifyBlood);
			}

			public int getLen()
			{
				int _len = 0;
				// dungeonId
				_len += 4;
				// oddBlood
				_len += 8;
				// verifyBlood
				_len += 8;
				return _len;
			}
		#endregion

	}

	public class GuildDungeonClearGateTime : Protocol.IProtocolStream
	{
		/// <summary>
		///  公会名字
		/// </summary>
		public string guildName;
		/// <summary>
		///  通关用时
		/// </summary>
		public UInt64 spendTime;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] guildNameBytes = StringHelper.StringToUTF8Bytes(guildName);
				BaseDLL.encode_string(buffer, ref pos_, guildNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint64(buffer, ref pos_, spendTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 guildNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guildNameLen);
				byte[] guildNameBytes = new byte[guildNameLen];
				for(int i = 0; i < guildNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref guildNameBytes[i]);
				}
				guildName = StringHelper.BytesToString(guildNameBytes);
				BaseDLL.decode_uint64(buffer, ref pos_, ref spendTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] guildNameBytes = StringHelper.StringToUTF8Bytes(guildName);
				BaseDLL.encode_string(buffer, ref pos_, guildNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint64(buffer, ref pos_, spendTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 guildNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guildNameLen);
				byte[] guildNameBytes = new byte[guildNameLen];
				for(int i = 0; i < guildNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref guildNameBytes[i]);
				}
				guildName = StringHelper.BytesToString(guildNameBytes);
				BaseDLL.decode_uint64(buffer, ref pos_, ref spendTime);
			}

			public int getLen()
			{
				int _len = 0;
				// guildName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(guildName);
					_len += 2 + _strBytes.Length;
				}
				// spendTime
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  公会地下城信息返回
	/// </summary>
	[Protocol]
	public class WorldGuildDungeonInfoRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608502;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
		/// </summary>
		public UInt32 result;
		/// <summary>
		///  地下城状态
		/// </summary>
		public UInt32 dungeonStatus;
		/// <summary>
		///  状态结束的时间戳
		/// </summary>
		public UInt32 statusEndStamp;
		/// <summary>
		///  是否已经领取奖励(1.领取 0.未领取)
		/// </summary>
		public UInt32 isReward;
		/// <summary>
		///  boss血量信息
		/// </summary>
		public GuildDungeonBossBlood[] bossBlood = new GuildDungeonBossBlood[0];
		/// <summary>
		///  通关用时
		/// </summary>
		public GuildDungeonClearGateTime[] clearGateTime = new GuildDungeonClearGateTime[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonStatus);
				BaseDLL.encode_uint32(buffer, ref pos_, statusEndStamp);
				BaseDLL.encode_uint32(buffer, ref pos_, isReward);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)bossBlood.Length);
				for(int i = 0; i < bossBlood.Length; i++)
				{
					bossBlood[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)clearGateTime.Length);
				for(int i = 0; i < clearGateTime.Length; i++)
				{
					clearGateTime[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonStatus);
				BaseDLL.decode_uint32(buffer, ref pos_, ref statusEndStamp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isReward);
				UInt16 bossBloodCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref bossBloodCnt);
				bossBlood = new GuildDungeonBossBlood[bossBloodCnt];
				for(int i = 0; i < bossBlood.Length; i++)
				{
					bossBlood[i] = new GuildDungeonBossBlood();
					bossBlood[i].decode(buffer, ref pos_);
				}
				UInt16 clearGateTimeCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref clearGateTimeCnt);
				clearGateTime = new GuildDungeonClearGateTime[clearGateTimeCnt];
				for(int i = 0; i < clearGateTime.Length; i++)
				{
					clearGateTime[i] = new GuildDungeonClearGateTime();
					clearGateTime[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonStatus);
				BaseDLL.encode_uint32(buffer, ref pos_, statusEndStamp);
				BaseDLL.encode_uint32(buffer, ref pos_, isReward);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)bossBlood.Length);
				for(int i = 0; i < bossBlood.Length; i++)
				{
					bossBlood[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)clearGateTime.Length);
				for(int i = 0; i < clearGateTime.Length; i++)
				{
					clearGateTime[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonStatus);
				BaseDLL.decode_uint32(buffer, ref pos_, ref statusEndStamp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isReward);
				UInt16 bossBloodCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref bossBloodCnt);
				bossBlood = new GuildDungeonBossBlood[bossBloodCnt];
				for(int i = 0; i < bossBlood.Length; i++)
				{
					bossBlood[i] = new GuildDungeonBossBlood();
					bossBlood[i].decode(buffer, ref pos_);
				}
				UInt16 clearGateTimeCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref clearGateTimeCnt);
				clearGateTime = new GuildDungeonClearGateTime[clearGateTimeCnt];
				for(int i = 0; i < clearGateTime.Length; i++)
				{
					clearGateTime[i] = new GuildDungeonClearGateTime();
					clearGateTime[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// dungeonStatus
				_len += 4;
				// statusEndStamp
				_len += 4;
				// isReward
				_len += 4;
				// bossBlood
				_len += 2;
				for(int j = 0; j < bossBlood.Length; j++)
				{
					_len += bossBlood[j].getLen();
				}
				// clearGateTime
				_len += 2;
				for(int j = 0; j < clearGateTime.Length; j++)
				{
					_len += clearGateTime[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  公会地下城伤害排行请求
	/// </summary>
	[Protocol]
	public class WorldGuildDungeonDamageRankReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608503;
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

	public class GuildDungeonDamage : Protocol.IProtocolStream
	{
		/// <summary>
		///  排名
		/// </summary>
		public UInt32 rank;
		/// <summary>
		///  伤害值
		/// </summary>
		public UInt64 damageVal;
		/// <summary>
		///  玩家ID
		/// </summary>
		public UInt64 playerId;
		/// <summary>
		///  玩家名字
		/// </summary>
		public string playerName;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, rank);
				BaseDLL.encode_uint64(buffer, ref pos_, damageVal);
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
				BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref rank);
				BaseDLL.decode_uint64(buffer, ref pos_, ref damageVal);
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
				BaseDLL.encode_uint32(buffer, ref pos_, rank);
				BaseDLL.encode_uint64(buffer, ref pos_, damageVal);
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
				BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref rank);
				BaseDLL.decode_uint64(buffer, ref pos_, ref damageVal);
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
				// rank
				_len += 4;
				// damageVal
				_len += 8;
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

	/// <summary>
	///  公会地下城伤害排行返回
	/// </summary>
	[Protocol]
	public class WorldGuildDungeonDamageRankRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608504;
		public UInt32 Sequence;
		/// <summary>
		///  伤害列表
		/// </summary>
		public GuildDungeonDamage[] damageVec = new GuildDungeonDamage[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)damageVec.Length);
				for(int i = 0; i < damageVec.Length; i++)
				{
					damageVec[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 damageVecCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref damageVecCnt);
				damageVec = new GuildDungeonDamage[damageVecCnt];
				for(int i = 0; i < damageVec.Length; i++)
				{
					damageVec[i] = new GuildDungeonDamage();
					damageVec[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)damageVec.Length);
				for(int i = 0; i < damageVec.Length; i++)
				{
					damageVec[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 damageVecCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref damageVecCnt);
				damageVec = new GuildDungeonDamage[damageVecCnt];
				for(int i = 0; i < damageVec.Length; i++)
				{
					damageVec[i] = new GuildDungeonDamage();
					damageVec[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// damageVec
				_len += 2;
				for(int j = 0; j < damageVec.Length; j++)
				{
					_len += damageVec[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  公会地下城副本信息请求
	/// </summary>
	[Protocol]
	public class WorldGuildDungeonCopyReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608505;
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
	///  掉落的buff信息
	/// </summary>
	public class GuildDungeonBuff : Protocol.IProtocolStream
	{
		/// <summary>
		///  id
		/// </summary>
		public UInt32 buffId;
		/// <summary>
		///  等级
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

	public class GuildDungeonBattleRecord : Protocol.IProtocolStream
	{
		/// <summary>
		///  地下城id
		/// </summary>
		public UInt32 dungeonId;
		/// <summary>
		///  战斗次数
		/// </summary>
		public UInt32 battleCnt;
		/// <summary>
		///  剩余血量
		/// </summary>
		public UInt64 oddBlood;
		/// <summary>
		///  buff列表
		/// </summary>
		public GuildDungeonBuff[] buffVec = new GuildDungeonBuff[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
				BaseDLL.encode_uint32(buffer, ref pos_, battleCnt);
				BaseDLL.encode_uint64(buffer, ref pos_, oddBlood);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)buffVec.Length);
				for(int i = 0; i < buffVec.Length; i++)
				{
					buffVec[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleCnt);
				BaseDLL.decode_uint64(buffer, ref pos_, ref oddBlood);
				UInt16 buffVecCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref buffVecCnt);
				buffVec = new GuildDungeonBuff[buffVecCnt];
				for(int i = 0; i < buffVec.Length; i++)
				{
					buffVec[i] = new GuildDungeonBuff();
					buffVec[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
				BaseDLL.encode_uint32(buffer, ref pos_, battleCnt);
				BaseDLL.encode_uint64(buffer, ref pos_, oddBlood);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)buffVec.Length);
				for(int i = 0; i < buffVec.Length; i++)
				{
					buffVec[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref battleCnt);
				BaseDLL.decode_uint64(buffer, ref pos_, ref oddBlood);
				UInt16 buffVecCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref buffVecCnt);
				buffVec = new GuildDungeonBuff[buffVecCnt];
				for(int i = 0; i < buffVec.Length; i++)
				{
					buffVec[i] = new GuildDungeonBuff();
					buffVec[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// dungeonId
				_len += 4;
				// battleCnt
				_len += 4;
				// oddBlood
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

	/// <summary>
	///  公会地下城副本信息返回
	/// </summary>
	[Protocol]
	public class WorldGuildDungeonCopyRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608506;
		public UInt32 Sequence;
		/// <summary>
		///  战斗记录
		/// </summary>
		public GuildDungeonBattleRecord[] battleRecord = new GuildDungeonBattleRecord[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)battleRecord.Length);
				for(int i = 0; i < battleRecord.Length; i++)
				{
					battleRecord[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 battleRecordCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref battleRecordCnt);
				battleRecord = new GuildDungeonBattleRecord[battleRecordCnt];
				for(int i = 0; i < battleRecord.Length; i++)
				{
					battleRecord[i] = new GuildDungeonBattleRecord();
					battleRecord[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)battleRecord.Length);
				for(int i = 0; i < battleRecord.Length; i++)
				{
					battleRecord[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 battleRecordCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref battleRecordCnt);
				battleRecord = new GuildDungeonBattleRecord[battleRecordCnt];
				for(int i = 0; i < battleRecord.Length; i++)
				{
					battleRecord[i] = new GuildDungeonBattleRecord();
					battleRecord[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// battleRecord
				_len += 2;
				for(int j = 0; j < battleRecord.Length; j++)
				{
					_len += battleRecord[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  公会地下城抽奖请求
	/// </summary>
	[Protocol]
	public class WorldGuildDungeonLotteryReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608507;
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
	///  公会地下城奖励
	/// </summary>
	public class GuildDungeonLotteryItem : Protocol.IProtocolStream
	{
		public UInt32 itemId;
		public UInt32 itemNum;
		public UInt32 isHighVal;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemNum);
				BaseDLL.encode_uint32(buffer, ref pos_, isHighVal);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isHighVal);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemNum);
				BaseDLL.encode_uint32(buffer, ref pos_, isHighVal);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isHighVal);
			}

			public int getLen()
			{
				int _len = 0;
				// itemId
				_len += 4;
				// itemNum
				_len += 4;
				// isHighVal
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  公会地下城抽奖返回
	/// </summary>
	[Protocol]
	public class WorldGuildDungeonLotteryRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608508;
		public UInt32 Sequence;
		/// <summary>
		///  结果
		/// </summary>
		public UInt32 result;
		/// <summary>
		///  奖励
		/// </summary>
		public GuildDungeonLotteryItem[] lotteryItemVec = new GuildDungeonLotteryItem[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)lotteryItemVec.Length);
				for(int i = 0; i < lotteryItemVec.Length; i++)
				{
					lotteryItemVec[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 lotteryItemVecCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref lotteryItemVecCnt);
				lotteryItemVec = new GuildDungeonLotteryItem[lotteryItemVecCnt];
				for(int i = 0; i < lotteryItemVec.Length; i++)
				{
					lotteryItemVec[i] = new GuildDungeonLotteryItem();
					lotteryItemVec[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)lotteryItemVec.Length);
				for(int i = 0; i < lotteryItemVec.Length; i++)
				{
					lotteryItemVec[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 lotteryItemVecCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref lotteryItemVecCnt);
				lotteryItemVec = new GuildDungeonLotteryItem[lotteryItemVecCnt];
				for(int i = 0; i < lotteryItemVec.Length; i++)
				{
					lotteryItemVec[i] = new GuildDungeonLotteryItem();
					lotteryItemVec[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// lotteryItemVec
				_len += 2;
				for(int j = 0; j < lotteryItemVec.Length; j++)
				{
					_len += lotteryItemVec[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  公会地下城状态同步
	/// </summary>
	[Protocol]
	public class WorldGuildDungeonStatusSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608509;
		public UInt32 Sequence;
		/// <summary>
		///  状态
		/// </summary>
		public UInt32 dungeonStatus;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonStatus);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonStatus);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dungeonStatus);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonStatus);
			}

			public int getLen()
			{
				int _len = 0;
				// dungeonStatus
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  公会地下城雕像请求
	/// </summary>
	[Protocol]
	public class WorldGuildDungeonStatueReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608510;
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
	///  公会地下城雕像返回
	/// </summary>
	[Protocol]
	public class WorldGuildDungeonStatueRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608511;
		public UInt32 Sequence;
		public FigureStatueInfo[] figureStatue = new FigureStatueInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)figureStatue.Length);
				for(int i = 0; i < figureStatue.Length; i++)
				{
					figureStatue[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 figureStatueCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref figureStatueCnt);
				figureStatue = new FigureStatueInfo[figureStatueCnt];
				for(int i = 0; i < figureStatue.Length; i++)
				{
					figureStatue[i] = new FigureStatueInfo();
					figureStatue[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)figureStatue.Length);
				for(int i = 0; i < figureStatue.Length; i++)
				{
					figureStatue[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 figureStatueCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref figureStatueCnt);
				figureStatue = new FigureStatueInfo[figureStatueCnt];
				for(int i = 0; i < figureStatue.Length; i++)
				{
					figureStatue[i] = new FigureStatueInfo();
					figureStatue[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// figureStatue
				_len += 2;
				for(int j = 0; j < figureStatue.Length; j++)
				{
					_len += figureStatue[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  公会地下城boss死亡通知
	/// </summary>
	[Protocol]
	public class WorldGuildDungeonBossDeadNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608512;
		public UInt32 Sequence;
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
	///  公会地下城结束通知
	/// </summary>
	[Protocol]
	public class WorldGuildDungeonEndNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608520;
		public UInt32 Sequence;
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
	///  公会地下城boss剩余血量
	/// </summary>
	[Protocol]
	public class WorldGuildDungeonBossOddBlood : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608521;
		public UInt32 Sequence;
		public UInt64 bossOddBlood;
		public UInt64 bossTotalBlood;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, bossOddBlood);
				BaseDLL.encode_uint64(buffer, ref pos_, bossTotalBlood);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref bossOddBlood);
				BaseDLL.decode_uint64(buffer, ref pos_, ref bossTotalBlood);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, bossOddBlood);
				BaseDLL.encode_uint64(buffer, ref pos_, bossTotalBlood);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref bossOddBlood);
				BaseDLL.decode_uint64(buffer, ref pos_, ref bossTotalBlood);
			}

			public int getLen()
			{
				int _len = 0;
				// bossOddBlood
				_len += 8;
				// bossTotalBlood
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  拍卖行信息请求
	/// </summary>
	[Protocol]
	public class WorldGuildAuctionItemReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608513;
		public UInt32 Sequence;
		/// <summary>
		///  拍卖类型(GuildAuctionType)
		/// </summary>
		public UInt32 type;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, type);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref type);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, type);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref type);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class GuildAuctionItem : Protocol.IProtocolStream
	{
		/// <summary>
		///  guid
		/// </summary>
		public UInt64 guid;
		/// <summary>
		///  出价人
		/// </summary>
		public UInt64 bidRoleId;
		/// <summary>
		///  当前出价
		/// </summary>
		public UInt32 curPrice;
		/// <summary>
		///  竞拍价
		/// </summary>
		public UInt32 bidPrice;
		/// <summary>
		///  一口价
		/// </summary>
		public UInt32 fixPrice;
		/// <summary>
		///  拍卖结束时间
		/// </summary>
		public UInt32 endTime;
		/// <summary>
		///  状态(GuildAuctionItemState)
		/// </summary>
		public UInt32 state;
		/// <summary>
		///  拍卖物品
		/// </summary>
		public ItemReward[] itemList = new ItemReward[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint64(buffer, ref pos_, bidRoleId);
				BaseDLL.encode_uint32(buffer, ref pos_, curPrice);
				BaseDLL.encode_uint32(buffer, ref pos_, bidPrice);
				BaseDLL.encode_uint32(buffer, ref pos_, fixPrice);
				BaseDLL.encode_uint32(buffer, ref pos_, endTime);
				BaseDLL.encode_uint32(buffer, ref pos_, state);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemList.Length);
				for(int i = 0; i < itemList.Length; i++)
				{
					itemList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref bidRoleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref curPrice);
				BaseDLL.decode_uint32(buffer, ref pos_, ref bidPrice);
				BaseDLL.decode_uint32(buffer, ref pos_, ref fixPrice);
				BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref state);
				UInt16 itemListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemListCnt);
				itemList = new ItemReward[itemListCnt];
				for(int i = 0; i < itemList.Length; i++)
				{
					itemList[i] = new ItemReward();
					itemList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint64(buffer, ref pos_, bidRoleId);
				BaseDLL.encode_uint32(buffer, ref pos_, curPrice);
				BaseDLL.encode_uint32(buffer, ref pos_, bidPrice);
				BaseDLL.encode_uint32(buffer, ref pos_, fixPrice);
				BaseDLL.encode_uint32(buffer, ref pos_, endTime);
				BaseDLL.encode_uint32(buffer, ref pos_, state);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemList.Length);
				for(int i = 0; i < itemList.Length; i++)
				{
					itemList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref bidRoleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref curPrice);
				BaseDLL.decode_uint32(buffer, ref pos_, ref bidPrice);
				BaseDLL.decode_uint32(buffer, ref pos_, ref fixPrice);
				BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref state);
				UInt16 itemListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemListCnt);
				itemList = new ItemReward[itemListCnt];
				for(int i = 0; i < itemList.Length; i++)
				{
					itemList[i] = new ItemReward();
					itemList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// guid
				_len += 8;
				// bidRoleId
				_len += 8;
				// curPrice
				_len += 4;
				// bidPrice
				_len += 4;
				// fixPrice
				_len += 4;
				// endTime
				_len += 4;
				// state
				_len += 4;
				// itemList
				_len += 2;
				for(int j = 0; j < itemList.Length; j++)
				{
					_len += itemList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  拍卖行信息返回
	/// </summary>
	[Protocol]
	public class WorldGuildAuctionItemRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608514;
		public UInt32 Sequence;
		/// <summary>
		///  拍卖类型(GuildAuctionType)
		/// </summary>
		public UInt32 type;
		/// <summary>
		///  拍卖物品列表
		/// </summary>
		public GuildAuctionItem[] auctionItemList = new GuildAuctionItem[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, type);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)auctionItemList.Length);
				for(int i = 0; i < auctionItemList.Length; i++)
				{
					auctionItemList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref type);
				UInt16 auctionItemListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref auctionItemListCnt);
				auctionItemList = new GuildAuctionItem[auctionItemListCnt];
				for(int i = 0; i < auctionItemList.Length; i++)
				{
					auctionItemList[i] = new GuildAuctionItem();
					auctionItemList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, type);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)auctionItemList.Length);
				for(int i = 0; i < auctionItemList.Length; i++)
				{
					auctionItemList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref type);
				UInt16 auctionItemListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref auctionItemListCnt);
				auctionItemList = new GuildAuctionItem[auctionItemListCnt];
				for(int i = 0; i < auctionItemList.Length; i++)
				{
					auctionItemList[i] = new GuildAuctionItem();
					auctionItemList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 4;
				// auctionItemList
				_len += 2;
				for(int j = 0; j < auctionItemList.Length; j++)
				{
					_len += auctionItemList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  拍卖行出价请求
	/// </summary>
	[Protocol]
	public class WorldGuildAuctionBidReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608515;
		public UInt32 Sequence;
		/// <summary>
		///  guid
		/// </summary>
		public UInt64 guid;
		/// <summary>
		///  出价
		/// </summary>
		public UInt32 bidPrice;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, bidPrice);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref bidPrice);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, bidPrice);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref bidPrice);
			}

			public int getLen()
			{
				int _len = 0;
				// guid
				_len += 8;
				// bidPrice
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  拍卖行出价返回
	/// </summary>
	[Protocol]
	public class WorldGuildAuctionBidRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608516;
		public UInt32 Sequence;
		/// <summary>
		///  返回码
		/// </summary>
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
	///  拍卖行一口价请求
	/// </summary>
	[Protocol]
	public class WorldGuildAuctionFixReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608517;
		public UInt32 Sequence;
		/// <summary>
		///  guid
		/// </summary>
		public UInt64 guid;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
			}

			public int getLen()
			{
				int _len = 0;
				// guid
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  拍卖行一口价返回
	/// </summary>
	[Protocol]
	public class WorldGuildAuctionFixRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608518;
		public UInt32 Sequence;
		/// <summary>
		///  返回码
		/// </summary>
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
	///  拍卖行通知
	/// </summary>
	[Protocol]
	public class WorldGuildAuctionNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608519;
		public UInt32 Sequence;
		/// <summary>
		///  拍卖类型
		/// </summary>
		public UInt32 type;
		/// <summary>
		///  非0打开，0关闭
		/// </summary>
		public UInt32 isOpen;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, isOpen);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isOpen);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, isOpen);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isOpen);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 4;
				// isOpen
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world 查看可兼并的公会列表请求
	/// </summary>
	[Protocol]
	public class WorldGuildWatchCanMergerReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601977;
		public UInt32 Sequence;
		/// <summary>
		///  开始位置 0开始
		/// </summary>
		public UInt16 start;
		/// <summary>
		///  数量
		/// </summary>
		public UInt16 num;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, start);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref start);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, start);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref start);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// start
				_len += 2;
				// num
				_len += 2;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world -> client  查看可兼并的公会列表返回
	/// </summary>
	[Protocol]
	public class WorldGuildWatchCanMergerRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601978;
		public UInt32 Sequence;
		public UInt16 start;
		public UInt16 totalNum;
		public GuildEntry[] guilds = new GuildEntry[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, start);
				BaseDLL.encode_uint16(buffer, ref pos_, totalNum);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)guilds.Length);
				for(int i = 0; i < guilds.Length; i++)
				{
					guilds[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref start);
				BaseDLL.decode_uint16(buffer, ref pos_, ref totalNum);
				UInt16 guildsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guildsCnt);
				guilds = new GuildEntry[guildsCnt];
				for(int i = 0; i < guilds.Length; i++)
				{
					guilds[i] = new GuildEntry();
					guilds[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, start);
				BaseDLL.encode_uint16(buffer, ref pos_, totalNum);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)guilds.Length);
				for(int i = 0; i < guilds.Length; i++)
				{
					guilds[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref start);
				BaseDLL.decode_uint16(buffer, ref pos_, ref totalNum);
				UInt16 guildsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guildsCnt);
				guilds = new GuildEntry[guildsCnt];
				for(int i = 0; i < guilds.Length; i++)
				{
					guilds[i] = new GuildEntry();
					guilds[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// start
				_len += 2;
				// totalNum
				_len += 2;
				// guilds
				_len += 2;
				for(int j = 0; j < guilds.Length; j++)
				{
					_len += guilds[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client -> world 兼并操作，包括申请兼并，取消申请兼并请求
	/// </summary>
	[Protocol]
	public class WorldGuildMergerRequestOperatorReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601979;
		public UInt32 Sequence;
		public UInt64 guildId;
		/// <summary>
		/// 操作类型 0申请兼并 1取消申请兼并
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
				BaseDLL.encode_uint64(buffer, ref pos_, guildId);
				BaseDLL.encode_int8(buffer, ref pos_, opType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guildId);
				BaseDLL.decode_int8(buffer, ref pos_, ref opType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guildId);
				BaseDLL.encode_int8(buffer, ref pos_, opType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guildId);
				BaseDLL.decode_int8(buffer, ref pos_, ref opType);
			}

			public int getLen()
			{
				int _len = 0;
				// guildId
				_len += 8;
				// opType
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world -> client 兼并操作结果
	/// </summary>
	[Protocol]
	public class WorldGuildMergerRequestOperatorRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601980;
		public UInt32 Sequence;
		public UInt32 errorCode;
		/// <summary>
		/// 操作类型 0申请兼并 1取消申请兼并
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
				BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
				BaseDLL.encode_int8(buffer, ref pos_, opType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
				BaseDLL.decode_int8(buffer, ref pos_, ref opType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
				BaseDLL.encode_int8(buffer, ref pos_, opType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
				BaseDLL.decode_int8(buffer, ref pos_, ref opType);
			}

			public int getLen()
			{
				int _len = 0;
				// errorCode
				_len += 4;
				// opType
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client ->  world 查询兼并请求
	/// </summary>
	[Protocol]
	public class WorldGuildReceiveMergerRequestReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601981;
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
	/// world -> client  返回兼并请求
	/// </summary>
	[Protocol]
	public class WorldGuildReceiveMergerRequestRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601982;
		public UInt32 Sequence;
		/// <summary>
		/// 是否有兼并请求 0没有 1有
		/// </summary>
		public byte isHave;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, isHave);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isHave);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, isHave);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isHave);
			}

			public int getLen()
			{
				int _len = 0;
				// isHave
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client ->  world 查看本公会收到的兼并申请列表
	/// </summary>
	[Protocol]
	public class WorldGuildWatchHavedMergerRequestReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601983;
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
	/// world -> client  返回本公会收到的兼并申请列表
	/// </summary>
	[Protocol]
	public class WorldGuildWatchHavedMergerRequestRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601984;
		public UInt32 Sequence;
		public GuildEntry[] guilds = new GuildEntry[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)guilds.Length);
				for(int i = 0; i < guilds.Length; i++)
				{
					guilds[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 guildsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guildsCnt);
				guilds = new GuildEntry[guildsCnt];
				for(int i = 0; i < guilds.Length; i++)
				{
					guilds[i] = new GuildEntry();
					guilds[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)guilds.Length);
				for(int i = 0; i < guilds.Length; i++)
				{
					guilds[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 guildsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guildsCnt);
				guilds = new GuildEntry[guildsCnt];
				for(int i = 0; i < guilds.Length; i++)
				{
					guilds[i] = new GuildEntry();
					guilds[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// guilds
				_len += 2;
				for(int j = 0; j < guilds.Length; j++)
				{
					_len += guilds[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client ->  world 同意，拒绝公会兼并申请，取消已同意的申请
	/// </summary>
	[Protocol]
	public class WorldGuildAcceptOrRefuseOrCancleMergerRequestReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601985;
		public UInt32 Sequence;
		public UInt64 guildId;
		/// <summary>
		/// 操作类型 0同意 1拒绝 2取消 3清空请求
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
				BaseDLL.encode_uint64(buffer, ref pos_, guildId);
				BaseDLL.encode_int8(buffer, ref pos_, opType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guildId);
				BaseDLL.decode_int8(buffer, ref pos_, ref opType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guildId);
				BaseDLL.encode_int8(buffer, ref pos_, opType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guildId);
				BaseDLL.decode_int8(buffer, ref pos_, ref opType);
			}

			public int getLen()
			{
				int _len = 0;
				// guildId
				_len += 8;
				// opType
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world -> client  返回申请请求操作结果
	/// </summary>
	[Protocol]
	public class WorldGuildAcceptOrRefuseOrCancleMergerRequestRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601986;
		public UInt32 Sequence;
		public UInt32 errorCode;
		/// <summary>
		/// 操作类型 0同意 1拒绝 2取消 3清空请求
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
				BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
				BaseDLL.encode_int8(buffer, ref pos_, opType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
				BaseDLL.decode_int8(buffer, ref pos_, ref opType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
				BaseDLL.encode_int8(buffer, ref pos_, opType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
				BaseDLL.decode_int8(buffer, ref pos_, ref opType);
			}

			public int getLen()
			{
				int _len = 0;
				// errorCode
				_len += 4;
				// opType
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world -> client  同步公会兼并相关信息
	/// </summary>
	[Protocol]
	public class WorldGuildSyncMergerInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601987;
		public UInt32 Sequence;
		/// <summary>
		/// 公会繁荣状态 1解散 2低繁荣 3中繁荣 4高繁荣
		/// </summary>
		public byte prosperityStatus;
		/// <summary>
		/// 请求状态 0无请求 1已申请 2已接受
		/// </summary>
		public byte mergerRequsetStatus;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, prosperityStatus);
				BaseDLL.encode_int8(buffer, ref pos_, mergerRequsetStatus);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref prosperityStatus);
				BaseDLL.decode_int8(buffer, ref pos_, ref mergerRequsetStatus);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, prosperityStatus);
				BaseDLL.encode_int8(buffer, ref pos_, mergerRequsetStatus);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref prosperityStatus);
				BaseDLL.decode_int8(buffer, ref pos_, ref mergerRequsetStatus);
			}

			public int getLen()
			{
				int _len = 0;
				// prosperityStatus
				_len += 1;
				// mergerRequsetStatus
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world 获取公会日志列表
	/// </summary>
	[Protocol]
	public class WorldGuildEventListReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601996;
		public UInt32 Sequence;
		/// <summary>
		/// 上次拉取的时间戳,0为第一次拉取
		/// </summary>
		public UInt32 uptime;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, uptime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref uptime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, uptime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref uptime);
			}

			public int getLen()
			{
				int _len = 0;
				// uptime
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client 获取公会日志列表返回
	/// </summary>
	[Protocol]
	public class WorldGuildEventListRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601997;
		public UInt32 Sequence;
		/// <summary>
		/// 当前事件戳, 用于下次拉取参考
		/// </summary>
		public UInt32 uptime;
		/// <summary>
		/// 公会列表
		/// </summary>
		public GuildEvent[] guildEvents = new GuildEvent[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, uptime);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)guildEvents.Length);
				for(int i = 0; i < guildEvents.Length; i++)
				{
					guildEvents[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref uptime);
				UInt16 guildEventsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guildEventsCnt);
				guildEvents = new GuildEvent[guildEventsCnt];
				for(int i = 0; i < guildEvents.Length; i++)
				{
					guildEvents[i] = new GuildEvent();
					guildEvents[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, uptime);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)guildEvents.Length);
				for(int i = 0; i < guildEvents.Length; i++)
				{
					guildEvents[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref uptime);
				UInt16 guildEventsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref guildEventsCnt);
				guildEvents = new GuildEvent[guildEventsCnt];
				for(int i = 0; i < guildEvents.Length; i++)
				{
					guildEvents[i] = new GuildEvent();
					guildEvents[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// uptime
				_len += 4;
				// guildEvents
				_len += 2;
				for(int j = 0; j < guildEvents.Length; j++)
				{
					_len += guildEvents[j].getLen();
				}
				return _len;
			}
		#endregion

	}

}
