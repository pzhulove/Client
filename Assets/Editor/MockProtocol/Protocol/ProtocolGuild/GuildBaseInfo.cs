using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  公会基础信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会基础信息", " 公会基础信息")]
	public class GuildBaseInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  公会ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会ID", " 公会ID")]
		public UInt64 id;
		/// <summary>
		///  公会名
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会名", " 公会名")]
		public string name;
		/// <summary>
		///  公会等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会等级", " 公会等级")]
		public byte level;
		/// <summary>
		///  公会资金
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会资金", " 公会资金")]
		public UInt32 fund;
		/// <summary>
		///  公会宣言
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会宣言", " 公会宣言")]
		public string declaration;
		/// <summary>
		///  公会公告
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会公告", " 公会公告")]
		public string announcement;
		/// <summary>
		///  解散时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 解散时间", " 解散时间")]
		public UInt32 dismissTime;
		/// <summary>
		///  成员数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 成员数量", " 成员数量")]
		public UInt16 memberNum;
		/// <summary>
		///  会长名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 会长名字", " 会长名字")]
		public string leaderName;
		/// <summary>
		///  公会战胜利抽奖几率
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战胜利抽奖几率", " 公会战胜利抽奖几率")]
		public byte winProbability;
		/// <summary>
		///  公会战失败抽奖几率
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战失败抽奖几率", " 公会战失败抽奖几率")]
		public byte loseProbability;
		/// <summary>
		///  公会仓库放入权限
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会仓库放入权限", " 公会仓库放入权限")]
		public byte storageAddPost;
		/// <summary>
		///  公会仓库放入权限
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会仓库放入权限", " 公会仓库放入权限")]
		public byte storageDelPost;
		/// <summary>
		///  建筑信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 建筑信息", " 建筑信息")]
		public GuildBuilding[] building = null;
		/// <summary>
		///  有没有申请加入公会的人
		/// </summary>
		[AdvancedInspector.Descriptor(" 有没有申请加入公会的人", " 有没有申请加入公会的人")]
		public byte hasRequester;
		/// <summary>
		///  圆桌会议成员信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 圆桌会议成员信息", " 圆桌会议成员信息")]
		public GuildTableMember[] tableMembers = null;
		/// <summary>
		///  公会战相关信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战相关信息", " 公会战相关信息")]
		public GuildBattleBaseInfo guildBattleInfo = null;
		/// <summary>
		///  入会限制等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 入会限制等级", " 入会限制等级")]
		public UInt32 joinLevel;
		/// <summary>
		///  徽记
		/// </summary>
		[AdvancedInspector.Descriptor(" 徽记", " 徽记")]
		public UInt32 emblemLevel;
		/// <summary>
		///  公会副本难度
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会副本难度", " 公会副本难度")]
		public UInt32 dungeonType;
		/// <summary>
		/// 公会本周增加繁荣度
		/// </summary>
		[AdvancedInspector.Descriptor("公会本周增加繁荣度", "公会本周增加繁荣度")]
		public UInt32 weekAddedFund;
		/// <summary>
		/// 公会上周增加繁荣度
		/// </summary>
		[AdvancedInspector.Descriptor("公会上周增加繁荣度", "公会上周增加繁荣度")]
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


		#endregion

	}

}
