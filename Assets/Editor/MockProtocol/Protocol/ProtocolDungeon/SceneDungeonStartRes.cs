using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneDungeonStartRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 506806;
		public UInt32 Sequence;
		/// <summary>
		///  是否接着上一次保存的状态
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否接着上一次保存的状态", " 是否接着上一次保存的状态")]
		public byte isCointnue;

		public UInt32 hp;

		public UInt32 result;

		public UInt32 key1;

		public SceneDungeonArea[] areas = null;
		/// <summary>
		///  深渊信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 深渊信息", " 深渊信息")]
		public DungeonHellInfo hellInfo = null;

		public UInt32 mp;

		public UInt32 key2;
		/// <summary>
		/// 登录RelayServer的session
		/// </summary>
		[AdvancedInspector.Descriptor("登录RelayServer的session", "登录RelayServer的session")]
		public UInt64 session;
		/// <summary>
		///  RelayServer地址
		/// </summary>
		[AdvancedInspector.Descriptor(" RelayServer地址", " RelayServer地址")]
		public SockAddr addr = null;
		/// <summary>
		///  所有玩家信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 所有玩家信息", " 所有玩家信息")]
		public RacePlayerInfo[] players = null;

		public UInt32 key3;
		/// <summary>
		///  是否开放自动战斗
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否开放自动战斗", " 是否开放自动战斗")]
		public byte openAutoBattle;

		public UInt32 dungeonId;

		public UInt32 startAreaId;

		public UInt32 key4;
		/// <summary>
		///  boss掉落
		/// </summary>
		[AdvancedInspector.Descriptor(" boss掉落", " boss掉落")]
		public SceneDungeonDropItem[] bossDropItems = null;
		/// <summary>
		///  是否记录日志
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否记录日志", " 是否记录日志")]
		public byte isRecordLog;
		/// <summary>
		///  是否记录录像
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否记录录像", " 是否记录录像")]
		public byte isRecordReplay;
		/// <summary>
		///  掉落次数用完的怪
		/// </summary>
		[AdvancedInspector.Descriptor(" 掉落次数用完的怪", " 掉落次数用完的怪")]
		public UInt32[] dropOverMonster = new UInt32[0];
		/// <summary>
		///  公会地下城信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会地下城信息", " 公会地下城信息")]
		public GuildDungeonInfo guildDungeonInfo = null;
		/// <summary>
		///  深渊是否自动结束(打完柱子就退出)
		/// </summary>
		[AdvancedInspector.Descriptor(" 深渊是否自动结束(打完柱子就退出)", " 深渊是否自动结束(打完柱子就退出)")]
		public byte hellAutoClose;
		/// <summary>
		///  战斗标记(位掩码)
		/// </summary>
		[AdvancedInspector.Descriptor(" 战斗标记(位掩码)", " 战斗标记(位掩码)")]
		public UInt32 battleFlag;
		/// <summary>
		///  终极试炼信息(只在终极试炼地下城使用)
		/// </summary>
		[AdvancedInspector.Descriptor(" 终极试炼信息(只在终极试炼地下城使用)", " 终极试炼信息(只在终极试炼地下城使用)")]
		public ZjslDungeonInfo zjslDungeonInfo = null;
		/// <summary>
		///  已通关地下城id
		/// </summary>
		[AdvancedInspector.Descriptor(" 已通关地下城id", " 已通关地下城id")]
		public UInt32[] clearedDungeonIds = new UInt32[0];
		/// <summary>
		///  md5
		/// </summary>
		[AdvancedInspector.Descriptor(" md5", " md5")]
		public byte[] md5 = new byte[16];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
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

		public UInt32 GetSequence()
		{
			return Sequence;
		}

		public void SetSequence(UInt32 sequence)
		{
			Sequence = sequence;
		}

		#endregion

	}

}
