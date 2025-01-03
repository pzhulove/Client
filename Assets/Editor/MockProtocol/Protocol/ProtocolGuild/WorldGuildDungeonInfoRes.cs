using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  公会地下城信息返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会地下城信息返回", " 公会地下城信息返回")]
	public class WorldGuildDungeonInfoRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608502;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
		/// </summary>
		[AdvancedInspector.Descriptor(" 返回值", " 返回值")]
		public UInt32 result;
		/// <summary>
		///  地下城状态
		/// </summary>
		[AdvancedInspector.Descriptor(" 地下城状态", " 地下城状态")]
		public UInt32 dungeonStatus;
		/// <summary>
		///  状态结束的时间戳
		/// </summary>
		[AdvancedInspector.Descriptor(" 状态结束的时间戳", " 状态结束的时间戳")]
		public UInt32 statusEndStamp;
		/// <summary>
		///  是否已经领取奖励(1.领取 0.未领取)
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否已经领取奖励(1.领取 0.未领取)", " 是否已经领取奖励(1.领取 0.未领取)")]
		public UInt32 isReward;
		/// <summary>
		///  boss血量信息
		/// </summary>
		[AdvancedInspector.Descriptor(" boss血量信息", " boss血量信息")]
		public GuildDungeonBossBlood[] bossBlood = null;
		/// <summary>
		///  通关用时
		/// </summary>
		[AdvancedInspector.Descriptor(" 通关用时", " 通关用时")]
		public GuildDungeonClearGateTime[] clearGateTime = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
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
