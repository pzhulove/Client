using System;
using System.Text;

namespace Protocol
{
	/// <summary>
	///  神器罐子折扣抽取状态
	/// </summary>
	public enum ArtifactJarDiscountExtractStatus
	{
		/// <summary>
		///  不可抽取
		/// </summary>
		AJDES_INVALID = 0,
		/// <summary>
		///  可抽取
		/// </summary>
		AJDES_IN = 1,
		/// <summary>
		///  已抽取
		/// </summary>
		AJDES_OVER = 2,
	}

	public enum OpActivityTmpType
	{
		OAT_NONE = 0,
		/// <summary>
		/// 每日单笔充值
		/// </summary>
		OAT_DAY_SINGLE_CHARGE = 1,
		/// <summary>
		/// 每日累计充值
		/// </summary>
		OAT_DAY_TATOL_CHARGE = 2,
		/// <summary>
		/// 累计充值
		/// </summary>
		OAT_TATOL_CHARGE = 3,
		/// <summary>
		/// 单笔充值	
		/// </summary>
		OAT_SINGLE_CHARGE = 4,
		/// <summary>
		/// 连续充值
		/// </summary>
		OAT_COMBO_CHARGE = 5,
		/// <summary>
		/// 每日累计消耗道具
		/// </summary>
		OAT_DAY_COST_ITEM = 6,
		/// <summary>
		/// 累计消耗道具
		/// </summary>
		OAT_COST_ITEM = 7,
		/// <summary>
		/// 每日购买指定商城礼包
		/// </summary>
		OAT_DAY_BUY_GIFTPACK = 8,
		/// <summary>
		/// 购买指定商城礼包
		/// </summary>
		OAT_BUY_GIFTPACK = 9,
		/// <summary>
		/// 每日登陆
		/// </summary>
		OAT_DAY_LOGIN = 10,
		/// <summary>
		/// 累计登陆天数
		/// </summary>
		OAT_LOGIN_DAYNUM = 11,
		/// <summary>
		/// 每日累计在线
		/// </summary>
		OAT_DAY_ONLINE_TIME = 12,
		/// <summary>
		/// 总累计在线
		/// </summary>
		OAT_ONLINE_TIME = 13,
		/// <summary>
		/// 每日累计完成关卡
		/// </summary>
		OAT_DAY_COMPLETE_DUNG = 14,
		/// <summary>
		/// 累计完成关卡
		/// </summary>
		OAT_COMPLETE_DUNG = 15,
		/// <summary>
		/// 手机绑定
		/// </summary>
		OAT_BIND_PHONE = 16,
		/// <summary>
		/// 时装
		/// </summary>
		OAT_BUY_FASHION = 17,
		/// <summary>
		/// 老的新服冲击赛
		/// </summary>
		OAT_LEVEL_FIGHTING = 18,
		/// <summary>
		/// 新服商城时装打折
		/// </summary>
		OAT_MALL_DISCOUNT_FOR_NEW_SERVER = 1000,
		/// <summary>
		/// 新服冲级赛竞争阶段
		/// </summary>
		OAT_LEVEL_FIGHTING_FOR_NEW_SERVER = 1001,
		/// <summary>
		/// 新服冲级赛公示阶段
		/// </summary>
		OAT_LEVEL_SHOW_FOR_NEW_SERVER = 1002,
		/// <summary>
		///  地下城掉落活动
		/// </summary>
		OAT_DUNGEON_DROP_ACTIVITY = 1100,
		/// <summary>
		///  地下城结算经验加成
		/// </summary>
		OAT_DUNGEON_EXP_ADDITION = 1200,
		/// <summary>
		///  决斗币奖励
		/// </summary>
		OAT_PVP_PK_COIN = 1300,
		/// <summary>
		///  预约职业活动
		/// </summary>
		OAT_APPOINTMENT_OCCU = 1400,
		/// <summary>
		///  深渊票消耗得抽奖
		/// </summary>
		OAT_HELL_TICKET_FOR_DRAW_PRIZE = 1500,
		/// <summary>
		///  疲劳消耗得BUFF
		/// </summary>
		OAT_FATIGUE_FOR_BUFF = 1600,
		/// <summary>
		///  疲劳消耗得代币
		/// </summary>
		OAT_FATIGUE_FOR_TOKEN_COIN = 1700,
		/// <summary>
		///  疲劳燃烧
		/// </summary>
		OAT_FATIGUE_BURNING = 1800,
		/// <summary>
		///  夺宝活动
		/// </summary>
		OAT_GAMBING = 1900,
		/// <summary>
		///  每日奖励
		/// </summary>
		OAT_DAILY_REWARD = 2000,
		/// <summary>
		///  七夕鹊桥
		/// </summary>
		OAT_MAGPIE_BRIDGE = 2100,
		/// <summary>
		///  月卡活动
		/// </summary>
		OAT_MONTH_CARD = 2200,
		/// <summary>
		///  限时礼包
		/// </summary>
		OAT_LIMIT_TIME_GIFT_PACK = 5000,
		/// <summary>
		///  赌马
		/// </summary>
		OAT_BET_HORSE = 5100,
		/// <summary>
		///  buff加成活动
		/// </summary>
		OAT_BUFF_ADDITION = 2300,
		/// <summary>
		///  地下城掉落倍率增加
		/// </summary>
		OAT_DUNGEON_DROP_RATE_ADDITION = 2400,
		/// <summary>
		///  百变换装
		/// </summary>
		OAT_CHANGE_FASHION_MERGE = 2500,
		/// <summary>
		///  绝版兑换
		/// </summary>
		OAT_CHANGE_FASHION_EXCHANGE = 2600,
		/// <summary>
		///  换装福利
		/// </summary>
		OAT_CHANGE_FASHION_WELFARE = 2700,
		/// <summary>
		///  地下城随机buff活动
		/// </summary>
		OAT_DUNGEON_RANDOM_BUFF = 2800,
		/// <summary>
		///  地下城通关得奖励
		/// </summary>
		OAT_DUNGEON_CLEAR_GET_REWARD = 2900,
		/// <summary>
		///  换装卷购买
		/// </summary>
		OAT_CHANGE_FASHION_PURCHASE = 3000,
		/// <summary>
		///  换装节活动
		/// </summary>
		OAT_CHANGE_FASHION_ACT = 3300,
		/// <summary>
		///  每日buff活动
		/// </summary>
		OAT_DAILY_BUFF = 3100,
		/// <summary>
		///  强化券合成活动
		/// </summary>
		OAT_STRENGTHEN_TICKET_SYNTHESIS = 3200,
		/// <summary>
		///  商城购买获得活动
		/// </summary>
		OAT_MALL_BUY_GOT = 3400,
		/// <summary>
		///  神器罐子活动
		/// </summary>
		OAT_ARTIFACT_JAR = 3500,
		/// <summary>
		///  罐子抽奖活动
		/// </summary>
		OAT_JAR_DRAW_LOTTERY = 3600,
		/// <summary>
		///  限时深渊活动
		/// </summary>
		OAT_LIMIT_TIME_HELL = 3700,
		/// <summary>
		///  绑金商店活动
		/// </summary>
		OAT_MALL_BINDINGGOLD = 3900,
		/// <summary>
		///  黑市商人活动
		/// </summary>
		OAT_BLACK_MARKET_SHOP = 4000,
		/// <summary>
		///  周常深渊
		/// </summary>
		OAT_WEEK_DEEP = 4600,
		/// <summary>
		///  购买赠送
		/// </summary>
		OAT_BUY_PRRSENT = 4700,
		/// <summary>
		///  累计登录领奖
		/// </summary>
		OAT_CUMULATE_LOGIN_REWARD = 4800,
		/// <summary>
		///  累计通关地下城天数领奖
		/// </summary>
		OAT_CUMULATEPASS_DUNGEON_REWARD = 4900,
		/// <summary>
		///  神器罐展示活动
		/// </summary>
		OAT_ARTIFACT_JAR_SHOW = 5000,
		/// <summary>
		///  飞升礼包
		/// </summary>
		OAT_FLYUP_GIFT = 5900,
		/// <summary>
		///  团本扶持
		/// </summary>
		OAT_TEAM_COPY_SUPPORT = 6000,
		/// <summary>
		///  累计在线
		/// </summary>
		OAT_CUMULATE_ONLINE = 6100,
		/// <summary>
		///  周年派对活动
		/// </summary>
		OAT_ZHOUNIAN_PAIDUI = 6200,
		/// <summary>
		///  万圣节南瓜头
		/// </summary>
		OAT_HALLOWEEN_PUMPKIN_HELMET = 6300,
		/// <summary>
		///  元旦2020
		/// </summary>
		OAT_NEW_YEAR_2020 = 6400,
		/// <summary>
		///  点券消费返利
		/// </summary>
		OAT_MONEY_CONSUME_REBATE = 6500,
		/// <summary>
		///  挑战者俱乐部	
		/// </summary>
		OAT_CHALLENGE_HUB = 6600,
		/// <summary>
		///  兑换折扣活动 
		/// </summary>
		OAT_EXCHANE_DISCOUNT = 6700,
		/// <summary>
		///  新服礼包折扣
		/// </summary>
		OAT_NEW_SERVER_GIFT_DISCOUNT = 6800,
		/// <summary>
		///  春节红包领取活动
		/// </summary>
		OAT_SPRING_FESTIVAL_RED_PACKET_RECV = 6900,
		/// <summary>
		///  春节地下城
		/// </summary>
		OAT_SPRING_FESTIVAL_DUNGEON = 7000,
		/// <summary>
		///  春节红包雨
		/// </summary>
		OAT_SPRING_FESTIVAL_RED_PACKET_RAIN = 7100,
		/// <summary>
		/// 在线好礼
		/// </summary>
		OAT_ONLINE_GIFT = 7600,
		/// <summary>
		/// 植树大挑战	
		/// </summary>
		OAT_PLANT_TREE = 7700,
		/// <summary>
		/// 全民砍价折扣
		/// </summary>
		OAT_WHOLE_BARGAIN_DISCOUNT = 50001,
		/// <summary>
		/// 全民砍价购买
		/// </summary>
		OAT_WHOLE_BARGAIN_SHOP = 50002,
	}

	public enum OpActivityTag
	{
		OAT_NONE = 0,
		OAT_NEW = 1,
	}

	public enum OpActivityCircleType
	{
		AT_DAILY = 0,
		/// <summary>
		///  每日活动
		/// </summary>
		AT_ONCE = 1,
		/// <summary>
		///  一次性活动
		/// </summary>
		AT_WEEK = 2,
	}

	/// <summary>
	///  每周活动
	/// </summary>
	public enum OpActivityState
	{
		OAS_END = 0,
		/// <summary>
		/// 活动结束
		/// </summary>
		OAS_IN = 1,
		/// <summary>
		/// 活动开始
		/// </summary>
		OAS_PREPARE = 2,
	}

	public enum OpActTaskState
	{
		OATS_INIT = 0,
		/// <summary>
		/// 初始状态
		/// </summary>
		OATS_UNFINISH = 1,
		/// <summary>
		/// 已经接任务
		/// </summary>
		OATS_FINISHED = 2,
		/// <summary>
		/// 已完成，未提交
		/// </summary>
		OATS_FAILED = 3,
		/// <summary>
		/// 失败
		/// </summary>
		OATS_SUBMITTING = 4,
		/// <summary>
		/// 正在提交中（已完成并且正在提交中)
		/// </summary>
		OATS_OVER = 5,
	}

	/// <summary>
	/// 植树大挑战活动状态
	/// </summary>
	public enum PlantOpActSate
	{
		POPS_NONE = 0,
		/// <summary>
		/// 未种植
		/// </summary>
		POPS_PLANTING = 1,
		/// <summary>
		/// 成长中
		/// </summary>
		POPS_CAN_APP = 2,
		/// <summary>
		/// 可鉴定
		/// </summary>
		POPS_ALLGET = 3,
	}

	[Protocol]
	public class SceneCdkReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607401;
		public UInt32 Sequence;
		public string cdk;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] cdkBytes = StringHelper.StringToUTF8Bytes(cdk);
				BaseDLL.encode_string(buffer, ref pos_, cdkBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 cdkLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref cdkLen);
				byte[] cdkBytes = new byte[cdkLen];
				for(int i = 0; i < cdkLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref cdkBytes[i]);
				}
				cdk = StringHelper.BytesToString(cdkBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] cdkBytes = StringHelper.StringToUTF8Bytes(cdk);
				BaseDLL.encode_string(buffer, ref pos_, cdkBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 cdkLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref cdkLen);
				byte[] cdkBytes = new byte[cdkLen];
				for(int i = 0; i < cdkLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref cdkBytes[i]);
				}
				cdk = StringHelper.BytesToString(cdkBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// cdk
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(cdk);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneCdkRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501152;
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
	/// 活动准备
	/// </summary>
	public class OpTaskReward : Protocol.IProtocolStream
	{
		public UInt32 id;
		public UInt32 num;
		public byte qualityLv;
		public byte strenth;
		/// <summary>
		/// 拍卖行交易冷却时间
		/// </summary>
		public UInt32 auctionCoolTimeStamp;
		/// <summary>
		/// 装备类型
		/// </summary>
		public byte equipType;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
				BaseDLL.encode_int8(buffer, ref pos_, qualityLv);
				BaseDLL.encode_int8(buffer, ref pos_, strenth);
				BaseDLL.encode_uint32(buffer, ref pos_, auctionCoolTimeStamp);
				BaseDLL.encode_int8(buffer, ref pos_, equipType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
				BaseDLL.decode_int8(buffer, ref pos_, ref qualityLv);
				BaseDLL.decode_int8(buffer, ref pos_, ref strenth);
				BaseDLL.decode_uint32(buffer, ref pos_, ref auctionCoolTimeStamp);
				BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
				BaseDLL.encode_int8(buffer, ref pos_, qualityLv);
				BaseDLL.encode_int8(buffer, ref pos_, strenth);
				BaseDLL.encode_uint32(buffer, ref pos_, auctionCoolTimeStamp);
				BaseDLL.encode_int8(buffer, ref pos_, equipType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
				BaseDLL.decode_int8(buffer, ref pos_, ref qualityLv);
				BaseDLL.decode_int8(buffer, ref pos_, ref strenth);
				BaseDLL.decode_uint32(buffer, ref pos_, ref auctionCoolTimeStamp);
				BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// num
				_len += 4;
				// qualityLv
				_len += 1;
				// strenth
				_len += 1;
				// auctionCoolTimeStamp
				_len += 4;
				// equipType
				_len += 1;
				return _len;
			}
		#endregion

	}

	public class OpActTaskData : Protocol.IProtocolStream
	{
		public UInt32 dataid;
		/// <summary>
		/// dataId
		/// </summary>
		public UInt32 completeNum;
		/// <summary>
		/// 完成数量
		/// </summary>
		public OpTaskReward[] rewards = new OpTaskReward[0];
		/// <summary>
		/// 奖励组
		/// </summary>
		public UInt32[] variables = new UInt32[0];
		/// <summary>
		/// 变量组
		/// </summary>
		public UInt32[] variables2 = new UInt32[0];
		/// <summary>
		/// 变量组2
		/// </summary>
		public CounterItem[] counters = new CounterItem[0];
		/// <summary>
		/// counter组
		/// </summary>
		/// <summary>
		///  任务名
		/// </summary>
		public string taskName;
		public string[] varProgressName = new string[0];
		/// <summary>
		/// 任务变量进度名
		/// </summary>
		/// <summary>
		/// 开启等级限制(玩家等级)
		/// </summary>
		public UInt16 playerLevelLimit;
		/// <summary>
		///  账户每日领奖限制次数
		/// </summary>
		public UInt32 accountDailySubmitLimit;
		/// <summary>
		///  账户总领奖限制次数
		/// </summary>
		public UInt32 accountTotalSubmitLimit;
		/// <summary>
		///  重置类型
		/// </summary>
		public UInt32 resetType;
		/// <summary>
		///  新增3项
		/// </summary>
		public UInt32 cantAccept;
		public UInt32 eventType;
		public UInt32 subType;
		/// <summary>
		///  账户每周领奖限制次数
		/// </summary>
		public UInt32 accountWeeklySubmitLimit;
		/// <summary>
		///  账号任务
		/// </summary>
		public UInt32 accountTask;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dataid);
				BaseDLL.encode_uint32(buffer, ref pos_, completeNum);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)rewards.Length);
				for(int i = 0; i < rewards.Length; i++)
				{
					rewards[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)variables.Length);
				for(int i = 0; i < variables.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, variables[i]);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)variables2.Length);
				for(int i = 0; i < variables2.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, variables2[i]);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)counters.Length);
				for(int i = 0; i < counters.Length; i++)
				{
					counters[i].encode(buffer, ref pos_);
				}
				byte[] taskNameBytes = StringHelper.StringToUTF8Bytes(taskName);
				BaseDLL.encode_string(buffer, ref pos_, taskNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)varProgressName.Length);
				for(int i = 0; i < varProgressName.Length; i++)
				{
					byte[] varProgressNameBytes = StringHelper.StringToUTF8Bytes(varProgressName[i]);
					BaseDLL.encode_string(buffer, ref pos_, varProgressNameBytes, (UInt16)(buffer.Length - pos_));
				}
				BaseDLL.encode_uint16(buffer, ref pos_, playerLevelLimit);
				BaseDLL.encode_uint32(buffer, ref pos_, accountDailySubmitLimit);
				BaseDLL.encode_uint32(buffer, ref pos_, accountTotalSubmitLimit);
				BaseDLL.encode_uint32(buffer, ref pos_, resetType);
				BaseDLL.encode_uint32(buffer, ref pos_, cantAccept);
				BaseDLL.encode_uint32(buffer, ref pos_, eventType);
				BaseDLL.encode_uint32(buffer, ref pos_, subType);
				BaseDLL.encode_uint32(buffer, ref pos_, accountWeeklySubmitLimit);
				BaseDLL.encode_uint32(buffer, ref pos_, accountTask);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref completeNum);
				UInt16 rewardsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref rewardsCnt);
				rewards = new OpTaskReward[rewardsCnt];
				for(int i = 0; i < rewards.Length; i++)
				{
					rewards[i] = new OpTaskReward();
					rewards[i].decode(buffer, ref pos_);
				}
				UInt16 variablesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref variablesCnt);
				variables = new UInt32[variablesCnt];
				for(int i = 0; i < variables.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref variables[i]);
				}
				UInt16 variables2Cnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref variables2Cnt);
				variables2 = new UInt32[variables2Cnt];
				for(int i = 0; i < variables2.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref variables2[i]);
				}
				UInt16 countersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref countersCnt);
				counters = new CounterItem[countersCnt];
				for(int i = 0; i < counters.Length; i++)
				{
					counters[i] = new CounterItem();
					counters[i].decode(buffer, ref pos_);
				}
				UInt16 taskNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref taskNameLen);
				byte[] taskNameBytes = new byte[taskNameLen];
				for(int i = 0; i < taskNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref taskNameBytes[i]);
				}
				taskName = StringHelper.BytesToString(taskNameBytes);
				UInt16 varProgressNameCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref varProgressNameCnt);
				varProgressName = new string[varProgressNameCnt];
				for(int i = 0; i < varProgressName.Length; i++)
				{
					UInt16 varProgressNameLen = 0;
					BaseDLL.decode_uint16(buffer, ref pos_, ref varProgressNameLen);
					byte[] varProgressNameBytes = new byte[varProgressNameLen];
					for(int j = 0; j < varProgressNameLen; j++)
					{
						BaseDLL.decode_int8(buffer, ref pos_, ref varProgressNameBytes[j]);
					}
					varProgressName[i] = StringHelper.BytesToString(varProgressNameBytes);
				}
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerLevelLimit);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accountDailySubmitLimit);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accountTotalSubmitLimit);
				BaseDLL.decode_uint32(buffer, ref pos_, ref resetType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref cantAccept);
				BaseDLL.decode_uint32(buffer, ref pos_, ref eventType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref subType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accountWeeklySubmitLimit);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accountTask);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dataid);
				BaseDLL.encode_uint32(buffer, ref pos_, completeNum);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)rewards.Length);
				for(int i = 0; i < rewards.Length; i++)
				{
					rewards[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)variables.Length);
				for(int i = 0; i < variables.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, variables[i]);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)variables2.Length);
				for(int i = 0; i < variables2.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, variables2[i]);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)counters.Length);
				for(int i = 0; i < counters.Length; i++)
				{
					counters[i].encode(buffer, ref pos_);
				}
				byte[] taskNameBytes = StringHelper.StringToUTF8Bytes(taskName);
				BaseDLL.encode_string(buffer, ref pos_, taskNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)varProgressName.Length);
				for(int i = 0; i < varProgressName.Length; i++)
				{
					byte[] varProgressNameBytes = StringHelper.StringToUTF8Bytes(varProgressName[i]);
					BaseDLL.encode_string(buffer, ref pos_, varProgressNameBytes, (UInt16)(buffer.Length - pos_));
				}
				BaseDLL.encode_uint16(buffer, ref pos_, playerLevelLimit);
				BaseDLL.encode_uint32(buffer, ref pos_, accountDailySubmitLimit);
				BaseDLL.encode_uint32(buffer, ref pos_, accountTotalSubmitLimit);
				BaseDLL.encode_uint32(buffer, ref pos_, resetType);
				BaseDLL.encode_uint32(buffer, ref pos_, cantAccept);
				BaseDLL.encode_uint32(buffer, ref pos_, eventType);
				BaseDLL.encode_uint32(buffer, ref pos_, subType);
				BaseDLL.encode_uint32(buffer, ref pos_, accountWeeklySubmitLimit);
				BaseDLL.encode_uint32(buffer, ref pos_, accountTask);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref completeNum);
				UInt16 rewardsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref rewardsCnt);
				rewards = new OpTaskReward[rewardsCnt];
				for(int i = 0; i < rewards.Length; i++)
				{
					rewards[i] = new OpTaskReward();
					rewards[i].decode(buffer, ref pos_);
				}
				UInt16 variablesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref variablesCnt);
				variables = new UInt32[variablesCnt];
				for(int i = 0; i < variables.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref variables[i]);
				}
				UInt16 variables2Cnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref variables2Cnt);
				variables2 = new UInt32[variables2Cnt];
				for(int i = 0; i < variables2.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref variables2[i]);
				}
				UInt16 countersCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref countersCnt);
				counters = new CounterItem[countersCnt];
				for(int i = 0; i < counters.Length; i++)
				{
					counters[i] = new CounterItem();
					counters[i].decode(buffer, ref pos_);
				}
				UInt16 taskNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref taskNameLen);
				byte[] taskNameBytes = new byte[taskNameLen];
				for(int i = 0; i < taskNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref taskNameBytes[i]);
				}
				taskName = StringHelper.BytesToString(taskNameBytes);
				UInt16 varProgressNameCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref varProgressNameCnt);
				varProgressName = new string[varProgressNameCnt];
				for(int i = 0; i < varProgressName.Length; i++)
				{
					UInt16 varProgressNameLen = 0;
					BaseDLL.decode_uint16(buffer, ref pos_, ref varProgressNameLen);
					byte[] varProgressNameBytes = new byte[varProgressNameLen];
					for(int j = 0; j < varProgressNameLen; j++)
					{
						BaseDLL.decode_int8(buffer, ref pos_, ref varProgressNameBytes[j]);
					}
					varProgressName[i] = StringHelper.BytesToString(varProgressNameBytes);
				}
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerLevelLimit);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accountDailySubmitLimit);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accountTotalSubmitLimit);
				BaseDLL.decode_uint32(buffer, ref pos_, ref resetType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref cantAccept);
				BaseDLL.decode_uint32(buffer, ref pos_, ref eventType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref subType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accountWeeklySubmitLimit);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accountTask);
			}

			public int getLen()
			{
				int _len = 0;
				// dataid
				_len += 4;
				// completeNum
				_len += 4;
				// rewards
				_len += 2;
				for(int j = 0; j < rewards.Length; j++)
				{
					_len += rewards[j].getLen();
				}
				// variables
				_len += 2 + 4 * variables.Length;
				// variables2
				_len += 2 + 4 * variables2.Length;
				// counters
				_len += 2;
				for(int j = 0; j < counters.Length; j++)
				{
					_len += counters[j].getLen();
				}
				// taskName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(taskName);
					_len += 2 + _strBytes.Length;
				}
				// varProgressName
				_len += 2;
				for(int j = 0; j < varProgressName.Length; j++)
				{
					{
						byte[] _strBytes = StringHelper.StringToUTF8Bytes(varProgressName[j]);
						_len += 2 + _strBytes.Length;
					}
				}
				// playerLevelLimit
				_len += 2;
				// accountDailySubmitLimit
				_len += 4;
				// accountTotalSubmitLimit
				_len += 4;
				// resetType
				_len += 4;
				// cantAccept
				_len += 4;
				// eventType
				_len += 4;
				// subType
				_len += 4;
				// accountWeeklySubmitLimit
				_len += 4;
				// accountTask
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class OpActivityData : Protocol.IProtocolStream
	{
		public UInt32 dataId;
		/// <summary>
		/// id
		/// </summary>
		public byte state;
		/// <summary>
		/// 状态OpActivityState
		/// </summary>
		public UInt32 tmpType;
		/// <summary>
		/// 模板类型OpActivityTmpType
		/// </summary>
		public string name;
		/// <summary>
		/// 活动名
		/// </summary>
		public byte tag;
		/// <summary>
		/// 活动标签OpActivityTag
		/// </summary>
		public UInt32 prepareTime;
		/// <summary>
		/// 准备时间
		/// </summary>
		public UInt32 startTime;
		/// <summary>
		/// 开始时间
		/// </summary>
		public UInt32 endTime;
		/// <summary>
		/// 结束时间
		/// </summary>
		public string desc;
		/// <summary>
		/// 描述
		/// </summary>
		public string ruleDesc;
		/// <summary>
		/// 规则描述OpActivityCircleType
		/// </summary>
		public byte circleType;
		/// <summary>
		/// 循环类型
		/// </summary>
		public OpActTaskData[] tasks = new OpActTaskData[0];
		/// <summary>
		/// 任务信息
		/// </summary>
		public string taskDesc;
		/// <summary>
		/// 任务描述
		/// </summary>
		/// <summary>
		/// 扩展参数
		/// </summary>
		public UInt32 parm;
		/// <summary>
		/// 扩展参数2
		/// </summary>
		public UInt32[] parm2 = new UInt32[0];
		/// <summary>
		/// 开启等级限制(玩家等级)
		/// </summary>
		public UInt16 playerLevelLimit;
		/// <summary>
		/// logo描述
		/// </summary>
		public string logoDesc;
		/// <summary>
		/// 活动相关count参数
		/// </summary>
		public string countParam;
		/// <summary>
		/// 扩展参数3
		/// </summary>
		public UInt32[] parm3 = new UInt32[0];
		/// <summary>
		/// 活动预制体路径
		/// </summary>
		public string prefabPath;
		/// <summary>
		/// 宣传图路径
		/// </summary>
		public string logoPath;
		/// <summary>
		/// 字符串参数
		/// </summary>
		public string[] strParams = new string[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
				BaseDLL.encode_int8(buffer, ref pos_, state);
				BaseDLL.encode_uint32(buffer, ref pos_, tmpType);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, tag);
				BaseDLL.encode_uint32(buffer, ref pos_, prepareTime);
				BaseDLL.encode_uint32(buffer, ref pos_, startTime);
				BaseDLL.encode_uint32(buffer, ref pos_, endTime);
				byte[] descBytes = StringHelper.StringToUTF8Bytes(desc);
				BaseDLL.encode_string(buffer, ref pos_, descBytes, (UInt16)(buffer.Length - pos_));
				byte[] ruleDescBytes = StringHelper.StringToUTF8Bytes(ruleDesc);
				BaseDLL.encode_string(buffer, ref pos_, ruleDescBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, circleType);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)tasks.Length);
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i].encode(buffer, ref pos_);
				}
				byte[] taskDescBytes = StringHelper.StringToUTF8Bytes(taskDesc);
				BaseDLL.encode_string(buffer, ref pos_, taskDescBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, parm);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)parm2.Length);
				for(int i = 0; i < parm2.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, parm2[i]);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, playerLevelLimit);
				byte[] logoDescBytes = StringHelper.StringToUTF8Bytes(logoDesc);
				BaseDLL.encode_string(buffer, ref pos_, logoDescBytes, (UInt16)(buffer.Length - pos_));
				byte[] countParamBytes = StringHelper.StringToUTF8Bytes(countParam);
				BaseDLL.encode_string(buffer, ref pos_, countParamBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)parm3.Length);
				for(int i = 0; i < parm3.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, parm3[i]);
				}
				byte[] prefabPathBytes = StringHelper.StringToUTF8Bytes(prefabPath);
				BaseDLL.encode_string(buffer, ref pos_, prefabPathBytes, (UInt16)(buffer.Length - pos_));
				byte[] logoPathBytes = StringHelper.StringToUTF8Bytes(logoPath);
				BaseDLL.encode_string(buffer, ref pos_, logoPathBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)strParams.Length);
				for(int i = 0; i < strParams.Length; i++)
				{
					byte[] strParamsBytes = StringHelper.StringToUTF8Bytes(strParams[i]);
					BaseDLL.encode_string(buffer, ref pos_, strParamsBytes, (UInt16)(buffer.Length - pos_));
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
				BaseDLL.decode_int8(buffer, ref pos_, ref state);
				BaseDLL.decode_uint32(buffer, ref pos_, ref tmpType);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref tag);
				BaseDLL.decode_uint32(buffer, ref pos_, ref prepareTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
				UInt16 descLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref descLen);
				byte[] descBytes = new byte[descLen];
				for(int i = 0; i < descLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref descBytes[i]);
				}
				desc = StringHelper.BytesToString(descBytes);
				UInt16 ruleDescLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref ruleDescLen);
				byte[] ruleDescBytes = new byte[ruleDescLen];
				for(int i = 0; i < ruleDescLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref ruleDescBytes[i]);
				}
				ruleDesc = StringHelper.BytesToString(ruleDescBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref circleType);
				UInt16 tasksCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref tasksCnt);
				tasks = new OpActTaskData[tasksCnt];
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i] = new OpActTaskData();
					tasks[i].decode(buffer, ref pos_);
				}
				UInt16 taskDescLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref taskDescLen);
				byte[] taskDescBytes = new byte[taskDescLen];
				for(int i = 0; i < taskDescLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref taskDescBytes[i]);
				}
				taskDesc = StringHelper.BytesToString(taskDescBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref parm);
				UInt16 parm2Cnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref parm2Cnt);
				parm2 = new UInt32[parm2Cnt];
				for(int i = 0; i < parm2.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref parm2[i]);
				}
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerLevelLimit);
				UInt16 logoDescLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref logoDescLen);
				byte[] logoDescBytes = new byte[logoDescLen];
				for(int i = 0; i < logoDescLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref logoDescBytes[i]);
				}
				logoDesc = StringHelper.BytesToString(logoDescBytes);
				UInt16 countParamLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref countParamLen);
				byte[] countParamBytes = new byte[countParamLen];
				for(int i = 0; i < countParamLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref countParamBytes[i]);
				}
				countParam = StringHelper.BytesToString(countParamBytes);
				UInt16 parm3Cnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref parm3Cnt);
				parm3 = new UInt32[parm3Cnt];
				for(int i = 0; i < parm3.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref parm3[i]);
				}
				UInt16 prefabPathLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref prefabPathLen);
				byte[] prefabPathBytes = new byte[prefabPathLen];
				for(int i = 0; i < prefabPathLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref prefabPathBytes[i]);
				}
				prefabPath = StringHelper.BytesToString(prefabPathBytes);
				UInt16 logoPathLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref logoPathLen);
				byte[] logoPathBytes = new byte[logoPathLen];
				for(int i = 0; i < logoPathLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref logoPathBytes[i]);
				}
				logoPath = StringHelper.BytesToString(logoPathBytes);
				UInt16 strParamsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref strParamsCnt);
				strParams = new string[strParamsCnt];
				for(int i = 0; i < strParams.Length; i++)
				{
					UInt16 strParamsLen = 0;
					BaseDLL.decode_uint16(buffer, ref pos_, ref strParamsLen);
					byte[] strParamsBytes = new byte[strParamsLen];
					for(int j = 0; j < strParamsLen; j++)
					{
						BaseDLL.decode_int8(buffer, ref pos_, ref strParamsBytes[j]);
					}
					strParams[i] = StringHelper.BytesToString(strParamsBytes);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
				BaseDLL.encode_int8(buffer, ref pos_, state);
				BaseDLL.encode_uint32(buffer, ref pos_, tmpType);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, tag);
				BaseDLL.encode_uint32(buffer, ref pos_, prepareTime);
				BaseDLL.encode_uint32(buffer, ref pos_, startTime);
				BaseDLL.encode_uint32(buffer, ref pos_, endTime);
				byte[] descBytes = StringHelper.StringToUTF8Bytes(desc);
				BaseDLL.encode_string(buffer, ref pos_, descBytes, (UInt16)(buffer.Length - pos_));
				byte[] ruleDescBytes = StringHelper.StringToUTF8Bytes(ruleDesc);
				BaseDLL.encode_string(buffer, ref pos_, ruleDescBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, circleType);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)tasks.Length);
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i].encode(buffer, ref pos_);
				}
				byte[] taskDescBytes = StringHelper.StringToUTF8Bytes(taskDesc);
				BaseDLL.encode_string(buffer, ref pos_, taskDescBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, parm);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)parm2.Length);
				for(int i = 0; i < parm2.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, parm2[i]);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, playerLevelLimit);
				byte[] logoDescBytes = StringHelper.StringToUTF8Bytes(logoDesc);
				BaseDLL.encode_string(buffer, ref pos_, logoDescBytes, (UInt16)(buffer.Length - pos_));
				byte[] countParamBytes = StringHelper.StringToUTF8Bytes(countParam);
				BaseDLL.encode_string(buffer, ref pos_, countParamBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)parm3.Length);
				for(int i = 0; i < parm3.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, parm3[i]);
				}
				byte[] prefabPathBytes = StringHelper.StringToUTF8Bytes(prefabPath);
				BaseDLL.encode_string(buffer, ref pos_, prefabPathBytes, (UInt16)(buffer.Length - pos_));
				byte[] logoPathBytes = StringHelper.StringToUTF8Bytes(logoPath);
				BaseDLL.encode_string(buffer, ref pos_, logoPathBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)strParams.Length);
				for(int i = 0; i < strParams.Length; i++)
				{
					byte[] strParamsBytes = StringHelper.StringToUTF8Bytes(strParams[i]);
					BaseDLL.encode_string(buffer, ref pos_, strParamsBytes, (UInt16)(buffer.Length - pos_));
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
				BaseDLL.decode_int8(buffer, ref pos_, ref state);
				BaseDLL.decode_uint32(buffer, ref pos_, ref tmpType);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref tag);
				BaseDLL.decode_uint32(buffer, ref pos_, ref prepareTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
				UInt16 descLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref descLen);
				byte[] descBytes = new byte[descLen];
				for(int i = 0; i < descLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref descBytes[i]);
				}
				desc = StringHelper.BytesToString(descBytes);
				UInt16 ruleDescLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref ruleDescLen);
				byte[] ruleDescBytes = new byte[ruleDescLen];
				for(int i = 0; i < ruleDescLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref ruleDescBytes[i]);
				}
				ruleDesc = StringHelper.BytesToString(ruleDescBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref circleType);
				UInt16 tasksCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref tasksCnt);
				tasks = new OpActTaskData[tasksCnt];
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i] = new OpActTaskData();
					tasks[i].decode(buffer, ref pos_);
				}
				UInt16 taskDescLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref taskDescLen);
				byte[] taskDescBytes = new byte[taskDescLen];
				for(int i = 0; i < taskDescLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref taskDescBytes[i]);
				}
				taskDesc = StringHelper.BytesToString(taskDescBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref parm);
				UInt16 parm2Cnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref parm2Cnt);
				parm2 = new UInt32[parm2Cnt];
				for(int i = 0; i < parm2.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref parm2[i]);
				}
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerLevelLimit);
				UInt16 logoDescLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref logoDescLen);
				byte[] logoDescBytes = new byte[logoDescLen];
				for(int i = 0; i < logoDescLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref logoDescBytes[i]);
				}
				logoDesc = StringHelper.BytesToString(logoDescBytes);
				UInt16 countParamLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref countParamLen);
				byte[] countParamBytes = new byte[countParamLen];
				for(int i = 0; i < countParamLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref countParamBytes[i]);
				}
				countParam = StringHelper.BytesToString(countParamBytes);
				UInt16 parm3Cnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref parm3Cnt);
				parm3 = new UInt32[parm3Cnt];
				for(int i = 0; i < parm3.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref parm3[i]);
				}
				UInt16 prefabPathLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref prefabPathLen);
				byte[] prefabPathBytes = new byte[prefabPathLen];
				for(int i = 0; i < prefabPathLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref prefabPathBytes[i]);
				}
				prefabPath = StringHelper.BytesToString(prefabPathBytes);
				UInt16 logoPathLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref logoPathLen);
				byte[] logoPathBytes = new byte[logoPathLen];
				for(int i = 0; i < logoPathLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref logoPathBytes[i]);
				}
				logoPath = StringHelper.BytesToString(logoPathBytes);
				UInt16 strParamsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref strParamsCnt);
				strParams = new string[strParamsCnt];
				for(int i = 0; i < strParams.Length; i++)
				{
					UInt16 strParamsLen = 0;
					BaseDLL.decode_uint16(buffer, ref pos_, ref strParamsLen);
					byte[] strParamsBytes = new byte[strParamsLen];
					for(int j = 0; j < strParamsLen; j++)
					{
						BaseDLL.decode_int8(buffer, ref pos_, ref strParamsBytes[j]);
					}
					strParams[i] = StringHelper.BytesToString(strParamsBytes);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// dataId
				_len += 4;
				// state
				_len += 1;
				// tmpType
				_len += 4;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// tag
				_len += 1;
				// prepareTime
				_len += 4;
				// startTime
				_len += 4;
				// endTime
				_len += 4;
				// desc
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(desc);
					_len += 2 + _strBytes.Length;
				}
				// ruleDesc
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(ruleDesc);
					_len += 2 + _strBytes.Length;
				}
				// circleType
				_len += 1;
				// tasks
				_len += 2;
				for(int j = 0; j < tasks.Length; j++)
				{
					_len += tasks[j].getLen();
				}
				// taskDesc
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(taskDesc);
					_len += 2 + _strBytes.Length;
				}
				// parm
				_len += 4;
				// parm2
				_len += 2 + 4 * parm2.Length;
				// playerLevelLimit
				_len += 2;
				// logoDesc
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(logoDesc);
					_len += 2 + _strBytes.Length;
				}
				// countParam
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(countParam);
					_len += 2 + _strBytes.Length;
				}
				// parm3
				_len += 2 + 4 * parm3.Length;
				// prefabPath
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(prefabPath);
					_len += 2 + _strBytes.Length;
				}
				// logoPath
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(logoPath);
					_len += 2 + _strBytes.Length;
				}
				// strParams
				_len += 2;
				for(int j = 0; j < strParams.Length; j++)
				{
					{
						byte[] _strBytes = StringHelper.StringToUTF8Bytes(strParams[j]);
						_len += 2 + _strBytes.Length;
					}
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 已完成,已提交
	/// </summary>
	public class OpActTaskParam : Protocol.IProtocolStream
	{
		public string key;
		public UInt32 value;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] keyBytes = StringHelper.StringToUTF8Bytes(key);
				BaseDLL.encode_string(buffer, ref pos_, keyBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, value);
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
				BaseDLL.decode_uint32(buffer, ref pos_, ref value);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] keyBytes = StringHelper.StringToUTF8Bytes(key);
				BaseDLL.encode_string(buffer, ref pos_, keyBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, value);
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
				BaseDLL.decode_uint32(buffer, ref pos_, ref value);
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
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 运营活动任务数据
	/// </summary>
	public class OpActTask : Protocol.IProtocolStream
	{
		public UInt32 dataId;
		/// <summary>
		/// id
		/// </summary>
		public UInt32 curNum;
		/// <summary>
		/// 当前数量
		/// </summary>
		public byte state;
		/// <summary>
		/// 状态OpActTaskState
		/// </summary>
		public OpActTaskParam[] parms = new OpActTaskParam[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
				BaseDLL.encode_uint32(buffer, ref pos_, curNum);
				BaseDLL.encode_int8(buffer, ref pos_, state);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)parms.Length);
				for(int i = 0; i < parms.Length; i++)
				{
					parms[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref curNum);
				BaseDLL.decode_int8(buffer, ref pos_, ref state);
				UInt16 parmsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref parmsCnt);
				parms = new OpActTaskParam[parmsCnt];
				for(int i = 0; i < parms.Length; i++)
				{
					parms[i] = new OpActTaskParam();
					parms[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
				BaseDLL.encode_uint32(buffer, ref pos_, curNum);
				BaseDLL.encode_int8(buffer, ref pos_, state);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)parms.Length);
				for(int i = 0; i < parms.Length; i++)
				{
					parms[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref curNum);
				BaseDLL.decode_int8(buffer, ref pos_, ref state);
				UInt16 parmsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref parmsCnt);
				parms = new OpActTaskParam[parmsCnt];
				for(int i = 0; i < parms.Length; i++)
				{
					parms[i] = new OpActTaskParam();
					parms[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// dataId
				_len += 4;
				// curNum
				_len += 4;
				// state
				_len += 1;
				// parms
				_len += 2;
				for(int j = 0; j < parms.Length; j++)
				{
					_len += parms[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 任务状态变量
	/// </summary>
	/// <summary>
	/// 同步运营活动data
	/// </summary>
	[Protocol]
	public class SyncOpActivityDatas : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501145;
		public UInt32 Sequence;
		public OpActivityData[] datas = new OpActivityData[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)datas.Length);
				for(int i = 0; i < datas.Length; i++)
				{
					datas[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 datasCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref datasCnt);
				datas = new OpActivityData[datasCnt];
				for(int i = 0; i < datas.Length; i++)
				{
					datas[i] = new OpActivityData();
					datas[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)datas.Length);
				for(int i = 0; i < datas.Length; i++)
				{
					datas[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 datasCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref datasCnt);
				datas = new OpActivityData[datasCnt];
				for(int i = 0; i < datas.Length; i++)
				{
					datas[i] = new OpActivityData();
					datas[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// datas
				_len += 2;
				for(int j = 0; j < datas.Length; j++)
				{
					_len += datas[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 同步运营活动任务
	/// </summary>
	[Protocol]
	public class SyncOpActivityTasks : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501146;
		public UInt32 Sequence;
		public OpActTask[] tasks = new OpActTask[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				tasks = new OpActTask[tasksCnt];
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i] = new OpActTask();
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
				tasks = new OpActTask[tasksCnt];
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i] = new OpActTask();
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
	/// 同步运营活动任务变化
	/// </summary>
	[Protocol]
	public class SyncOpActivityTaskChange : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501147;
		public UInt32 Sequence;
		public OpActTask[] tasks = new OpActTask[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
				tasks = new OpActTask[tasksCnt];
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i] = new OpActTask();
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
				tasks = new OpActTask[tasksCnt];
				for(int i = 0; i < tasks.Length; i++)
				{
					tasks[i] = new OpActTask();
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
	/// 同步运营活动状态变化
	/// </summary>
	[Protocol]
	public class SyncOpActivityStateChange : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501149;
		public UInt32 Sequence;
		public OpActivityData data = new OpActivityData();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
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
	/// 领取运营活动任务奖励
	/// </summary>
	[Protocol]
	public class TakeOpActTaskReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501148;
		public UInt32 Sequence;
		public UInt32 activityDataId;
		public UInt32 taskDataId;
		public UInt64 param;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, activityDataId);
				BaseDLL.encode_uint32(buffer, ref pos_, taskDataId);
				BaseDLL.encode_uint64(buffer, ref pos_, param);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref activityDataId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskDataId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref param);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, activityDataId);
				BaseDLL.encode_uint32(buffer, ref pos_, taskDataId);
				BaseDLL.encode_uint64(buffer, ref pos_, param);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref activityDataId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskDataId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref param);
			}

			public int getLen()
			{
				int _len = 0;
				// activityDataId
				_len += 4;
				// taskDataId
				_len += 4;
				// param
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 运营活动任务信息请求
	/// </summary>
	[Protocol]
	public class SceneOpActivityTaskInfoReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501158;
		public UInt32 Sequence;
		public UInt32 opActId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
			}

			public int getLen()
			{
				int _len = 0;
				// opActId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 活动服务器不存在
	/// </summary>
	[Protocol]
	public class GASNonExistent : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 700001;
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
	/// 运营活动变量同步
	/// </summary>
	[Protocol]
	public class SceneOpActivityVarSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507401;
		public UInt32 Sequence;
		/// <summary>
		///  运营活动id
		/// </summary>
		public UInt32 opActId;
		/// <summary>
		///  变量key
		/// </summary>
		public string key;
		/// <summary>
		///  变量value
		/// </summary>
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
				BaseDLL.encode_uint32(buffer, ref pos_, opActId);
				byte[] keyBytes = StringHelper.StringToUTF8Bytes(key);
				BaseDLL.encode_string(buffer, ref pos_, keyBytes, (UInt16)(buffer.Length - pos_));
				byte[] valueBytes = StringHelper.StringToUTF8Bytes(value);
				BaseDLL.encode_string(buffer, ref pos_, valueBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
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
				BaseDLL.encode_uint32(buffer, ref pos_, opActId);
				byte[] keyBytes = StringHelper.StringToUTF8Bytes(key);
				BaseDLL.encode_string(buffer, ref pos_, keyBytes, (UInt16)(buffer.Length - pos_));
				byte[] valueBytes = StringHelper.StringToUTF8Bytes(value);
				BaseDLL.encode_string(buffer, ref pos_, valueBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
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
				// opActId
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

	/// <summary>
	/// 神器罐子折扣信息请求
	/// </summary>
	[Protocol]
	public class SceneArtifactJarDiscountInfoReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507402;
		public UInt32 Sequence;
		/// <summary>
		///  运营活动id
		/// </summary>
		public UInt32 opActId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
			}

			public int getLen()
			{
				int _len = 0;
				// opActId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 神器罐子折扣信息同步
	/// </summary>
	[Protocol]
	public class SceneArtifactJarDiscountInfoSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507403;
		public UInt32 Sequence;
		/// <summary>
		///  运营活动id
		/// </summary>
		public UInt32 opActId;
		/// <summary>
		///  抽取折扣状态(ArtifactJarDiscountExtractStatus)
		/// </summary>
		public byte extractDiscountStatus;
		/// <summary>
		///  折扣率
		/// </summary>
		public UInt32 discountRate;
		/// <summary>
		///  折扣生效次数
		/// </summary>
		public UInt32 discountEffectTimes;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActId);
				BaseDLL.encode_int8(buffer, ref pos_, extractDiscountStatus);
				BaseDLL.encode_uint32(buffer, ref pos_, discountRate);
				BaseDLL.encode_uint32(buffer, ref pos_, discountEffectTimes);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
				BaseDLL.decode_int8(buffer, ref pos_, ref extractDiscountStatus);
				BaseDLL.decode_uint32(buffer, ref pos_, ref discountRate);
				BaseDLL.decode_uint32(buffer, ref pos_, ref discountEffectTimes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActId);
				BaseDLL.encode_int8(buffer, ref pos_, extractDiscountStatus);
				BaseDLL.encode_uint32(buffer, ref pos_, discountRate);
				BaseDLL.encode_uint32(buffer, ref pos_, discountEffectTimes);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
				BaseDLL.decode_int8(buffer, ref pos_, ref extractDiscountStatus);
				BaseDLL.decode_uint32(buffer, ref pos_, ref discountRate);
				BaseDLL.decode_uint32(buffer, ref pos_, ref discountEffectTimes);
			}

			public int getLen()
			{
				int _len = 0;
				// opActId
				_len += 4;
				// extractDiscountStatus
				_len += 1;
				// discountRate
				_len += 4;
				// discountEffectTimes
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 神器罐子折扣抽取请求
	/// </summary>
	[Protocol]
	public class SceneArtifactJarExtractDiscountReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507404;
		public UInt32 Sequence;
		/// <summary>
		///  运营活动id
		/// </summary>
		public UInt32 opActId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
			}

			public int getLen()
			{
				int _len = 0;
				// opActId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 神器罐子折扣抽取返回
	/// </summary>
	[Protocol]
	public class SceneArtifactJarExtractDiscountRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507405;
		public UInt32 Sequence;
		/// <summary>
		///  运营活动id
		/// </summary>
		public UInt32 opActId;
		/// <summary>
		///  错误码
		/// </summary>
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
				BaseDLL.encode_uint32(buffer, ref pos_, opActId);
				BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActId);
				BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
			}

			public int getLen()
			{
				int _len = 0;
				// opActId
				_len += 4;
				// errorCode
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  周签到数据下发
	/// </summary>
	[Protocol]
	public class SceneWeekSignNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507406;
		public UInt32 Sequence;
		/// <summary>
		///  活动类型
		/// </summary>
		public UInt32 opActType;
		/// <summary>
		///  已签到周数
		/// </summary>
		public UInt32 signWeekSum;
		/// <summary>
		///  已经领取奖励的周
		/// </summary>
		public UInt32[] yetWeek = new UInt32[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActType);
				BaseDLL.encode_uint32(buffer, ref pos_, signWeekSum);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)yetWeek.Length);
				for(int i = 0; i < yetWeek.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, yetWeek[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref signWeekSum);
				UInt16 yetWeekCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref yetWeekCnt);
				yetWeek = new UInt32[yetWeekCnt];
				for(int i = 0; i < yetWeek.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref yetWeek[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActType);
				BaseDLL.encode_uint32(buffer, ref pos_, signWeekSum);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)yetWeek.Length);
				for(int i = 0; i < yetWeek.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, yetWeek[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref signWeekSum);
				UInt16 yetWeekCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref yetWeekCnt);
				yetWeek = new UInt32[yetWeekCnt];
				for(int i = 0; i < yetWeek.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref yetWeek[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// opActType
				_len += 4;
				// signWeekSum
				_len += 4;
				// yetWeek
				_len += 2 + 4 * yetWeek.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  周签到宝箱数据
	/// </summary>
	public class WeekSignBox : Protocol.IProtocolStream
	{
		/// <summary>
		///  活动Id
		/// </summary>
		public UInt32 opActId;
		/// <summary>
		///  奖励列表
		/// </summary>
		public ItemReward[] itemVec = new ItemReward[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemVec.Length);
				for(int i = 0; i < itemVec.Length; i++)
				{
					itemVec[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
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
				BaseDLL.encode_uint32(buffer, ref pos_, opActId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemVec.Length);
				for(int i = 0; i < itemVec.Length; i++)
				{
					itemVec[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
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
				// opActId
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
	///  周签到宝箱数据下发
	/// </summary>
	[Protocol]
	public class SceneWeekSignBoxNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507407;
		public UInt32 Sequence;
		/// <summary>
		///  活动类型
		/// </summary>
		public UInt32 opActType;
		/// <summary>
		///  宝箱数据
		/// </summary>
		public WeekSignBox[] boxData = new WeekSignBox[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActType);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)boxData.Length);
				for(int i = 0; i < boxData.Length; i++)
				{
					boxData[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActType);
				UInt16 boxDataCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref boxDataCnt);
				boxData = new WeekSignBox[boxDataCnt];
				for(int i = 0; i < boxData.Length; i++)
				{
					boxData[i] = new WeekSignBox();
					boxData[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActType);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)boxData.Length);
				for(int i = 0; i < boxData.Length; i++)
				{
					boxData[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActType);
				UInt16 boxDataCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref boxDataCnt);
				boxData = new WeekSignBox[boxDataCnt];
				for(int i = 0; i < boxData.Length; i++)
				{
					boxData[i] = new WeekSignBox();
					boxData[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// opActType
				_len += 4;
				// boxData
				_len += 2;
				for(int j = 0; j < boxData.Length; j++)
				{
					_len += boxData[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  周签到奖励领取请求
	/// </summary>
	[Protocol]
	public class SceneWeekSignRewardReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507408;
		public UInt32 Sequence;
		/// <summary>
		///  活动类型
		/// </summary>
		public UInt32 opActType;
		/// <summary>
		///  第几周
		/// </summary>
		public UInt32 weekID;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActType);
				BaseDLL.encode_uint32(buffer, ref pos_, weekID);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref weekID);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActType);
				BaseDLL.encode_uint32(buffer, ref pos_, weekID);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref weekID);
			}

			public int getLen()
			{
				int _len = 0;
				// opActType
				_len += 4;
				// weekID
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  周签到奖励领取返回
	/// </summary>
	[Protocol]
	public class SceneWeekSignRewardRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507409;
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
	///  周签到活动抽奖记录请求
	/// </summary>
	[Protocol]
	public class GASWeekSignRecordReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 707401;
		public UInt32 Sequence;
		/// <summary>
		///  活动类型
		/// </summary>
		public UInt32 opActType;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActType);
			}

			public int getLen()
			{
				int _len = 0;
				// opActType
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class WeekSignRecord : Protocol.IProtocolStream
	{
		/// <summary>
		///  服务器名字
		/// </summary>
		public string serverName;
		/// <summary>
		///  玩家名字
		/// </summary>
		public string roleName;
		/// <summary>
		///  道具ID
		/// </summary>
		public UInt32 itemId;
		/// <summary>
		///  道具数量
		/// </summary>
		public UInt32 itemNum;
		/// <summary>
		///  创建时间
		/// </summary>
		public UInt32 createTime;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] serverNameBytes = StringHelper.StringToUTF8Bytes(serverName);
				BaseDLL.encode_string(buffer, ref pos_, serverNameBytes, (UInt16)(buffer.Length - pos_));
				byte[] roleNameBytes = StringHelper.StringToUTF8Bytes(roleName);
				BaseDLL.encode_string(buffer, ref pos_, roleNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemNum);
				BaseDLL.encode_uint32(buffer, ref pos_, createTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 serverNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref serverNameLen);
				byte[] serverNameBytes = new byte[serverNameLen];
				for(int i = 0; i < serverNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref serverNameBytes[i]);
				}
				serverName = StringHelper.BytesToString(serverNameBytes);
				UInt16 roleNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref roleNameLen);
				byte[] roleNameBytes = new byte[roleNameLen];
				for(int i = 0; i < roleNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref roleNameBytes[i]);
				}
				roleName = StringHelper.BytesToString(roleNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref createTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] serverNameBytes = StringHelper.StringToUTF8Bytes(serverName);
				BaseDLL.encode_string(buffer, ref pos_, serverNameBytes, (UInt16)(buffer.Length - pos_));
				byte[] roleNameBytes = StringHelper.StringToUTF8Bytes(roleName);
				BaseDLL.encode_string(buffer, ref pos_, roleNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemNum);
				BaseDLL.encode_uint32(buffer, ref pos_, createTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 serverNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref serverNameLen);
				byte[] serverNameBytes = new byte[serverNameLen];
				for(int i = 0; i < serverNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref serverNameBytes[i]);
				}
				serverName = StringHelper.BytesToString(serverNameBytes);
				UInt16 roleNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref roleNameLen);
				byte[] roleNameBytes = new byte[roleNameLen];
				for(int i = 0; i < roleNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref roleNameBytes[i]);
				}
				roleName = StringHelper.BytesToString(roleNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref createTime);
			}

			public int getLen()
			{
				int _len = 0;
				// serverName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(serverName);
					_len += 2 + _strBytes.Length;
				}
				// roleName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(roleName);
					_len += 2 + _strBytes.Length;
				}
				// itemId
				_len += 4;
				// itemNum
				_len += 4;
				// createTime
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  周签到活动抽奖记录返回
	/// </summary>
	[Protocol]
	public class GASWeekSignRecordRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 707402;
		public UInt32 Sequence;
		/// <summary>
		///  活动类型
		/// </summary>
		public UInt32 opActType;
		/// <summary>
		///  抽奖记录
		/// </summary>
		public WeekSignRecord[] record = new WeekSignRecord[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActType);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)record.Length);
				for(int i = 0; i < record.Length; i++)
				{
					record[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActType);
				UInt16 recordCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref recordCnt);
				record = new WeekSignRecord[recordCnt];
				for(int i = 0; i < record.Length; i++)
				{
					record[i] = new WeekSignRecord();
					record[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActType);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)record.Length);
				for(int i = 0; i < record.Length; i++)
				{
					record[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActType);
				UInt16 recordCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref recordCnt);
				record = new WeekSignRecord[recordCnt];
				for(int i = 0; i < record.Length; i++)
				{
					record[i] = new WeekSignRecord();
					record[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// opActType
				_len += 4;
				// record
				_len += 2;
				for(int j = 0; j < record.Length; j++)
				{
					_len += record[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  运营活动接任务请求
	/// </summary>
	[Protocol]
	public class SceneOpActivityAcceptTaskReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507410;
		public UInt32 Sequence;
		/// <summary>
		///  运营活动id
		/// </summary>
		public UInt32 opActId;
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
				BaseDLL.encode_uint32(buffer, ref pos_, opActId);
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActId);
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
			}

			public int getLen()
			{
				int _len = 0;
				// opActId
				_len += 4;
				// taskId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  运营活动接任务返回
	/// </summary>
	[Protocol]
	public class SceneOpActivityAcceptTaskRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507411;
		public UInt32 Sequence;
		/// <summary>
		///  运营活动id
		/// </summary>
		public UInt32 opActId;
		/// <summary>
		///  任务id
		/// </summary>
		public UInt32 taskId;
		/// <summary>
		///  返回值
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
				BaseDLL.encode_uint32(buffer, ref pos_, opActId);
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActId);
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public int getLen()
			{
				int _len = 0;
				// opActId
				_len += 4;
				// taskId
				_len += 4;
				// retCode
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  运营活动接任务请求
	/// </summary>
	[Protocol]
	public class SceneOpActivityGetCounterReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507412;
		public UInt32 Sequence;
		/// <summary>
		///  运营活动id
		/// </summary>
		public UInt32 opActId;
		/// <summary>
		///  计数id
		/// </summary>
		public UInt32 counterId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActId);
				BaseDLL.encode_uint32(buffer, ref pos_, counterId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref counterId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActId);
				BaseDLL.encode_uint32(buffer, ref pos_, counterId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref counterId);
			}

			public int getLen()
			{
				int _len = 0;
				// opActId
				_len += 4;
				// counterId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  运营活动接任务返回
	/// </summary>
	[Protocol]
	public class SceneOpActivityGetCounterRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507413;
		public UInt32 Sequence;
		/// <summary>
		///  运营活动id
		/// </summary>
		public UInt32 opActId;
		/// <summary>
		///  计数id
		/// </summary>
		public UInt32 counterId;
		/// <summary>
		///  计数值
		/// </summary>
		public UInt32 counterValue;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActId);
				BaseDLL.encode_uint32(buffer, ref pos_, counterId);
				BaseDLL.encode_uint32(buffer, ref pos_, counterValue);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref counterId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref counterValue);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opActId);
				BaseDLL.encode_uint32(buffer, ref pos_, counterId);
				BaseDLL.encode_uint32(buffer, ref pos_, counterValue);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref counterId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref counterValue);
			}

			public int getLen()
			{
				int _len = 0;
				// opActId
				_len += 4;
				// counterId
				_len += 4;
				// counterValue
				_len += 4;
				return _len;
			}
		#endregion

	}

}
