using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  公会战相关信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会战相关信息", " 公会战相关信息")]
	public class GuildBattleBaseInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  报名领地ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 报名领地ID", " 报名领地ID")]
		public byte enrollTerrId;
		/// <summary>
		///  公会战积分
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战积分", " 公会战积分")]
		public UInt32 guildBattleScore;
		/// <summary>
		///  已经占领的领地ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 已经占领的领地ID", " 已经占领的领地ID")]
		public byte occupyTerrId;
		/// <summary>
		///  已经占领的跨服领地ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 已经占领的跨服领地ID", " 已经占领的跨服领地ID")]
		public byte occupyCrossTerrId;
		/// <summary>
		///  历史占领的领地ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 历史占领的领地ID", " 历史占领的领地ID")]
		public byte historyTerrId;
		/// <summary>
		///  鼓舞次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 鼓舞次数", " 鼓舞次数")]
		public byte inspire;
		/// <summary>
		///  自己的公会战记录
		/// </summary>
		[AdvancedInspector.Descriptor(" 自己的公会战记录", " 自己的公会战记录")]
		public GuildBattleRecord[] selfGuildBattleRecord = null;
		/// <summary>
		///  领地信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 领地信息", " 领地信息")]
		public GuildTerritoryBaseInfo[] terrInfos = null;
		/// <summary>
		/// 公会战类型
		/// </summary>
		[AdvancedInspector.Descriptor("公会战类型", "公会战类型")]
		public byte guildBattleType;
		/// <summary>
		/// 公会战状态
		/// </summary>
		[AdvancedInspector.Descriptor("公会战状态", "公会战状态")]
		public byte guildBattleStatus;
		/// <summary>
		/// 公会战状态结束时间
		/// </summary>
		[AdvancedInspector.Descriptor("公会战状态结束时间", "公会战状态结束时间")]
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


		#endregion

	}

}
