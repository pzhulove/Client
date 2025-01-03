using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  同门关系类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 同门关系类型", " 同门关系类型")]
	public enum MasterSectRelationType
	{

		MSRELATION_MASTER = 1,
		/// <summary>
		/// 师傅
		/// </summary>
		[AdvancedInspector.Descriptor("师傅", "师傅")]
		MSRELATION_BROTHER = 2,
		/// <summary>
		/// 师兄弟
		/// </summary>
		[AdvancedInspector.Descriptor("师兄弟", "师兄弟")]
		MSRELATION_DISCIPLE = 3,
		/// <summary>
		/// 徒弟
		/// </summary>
		[AdvancedInspector.Descriptor("徒弟", "徒弟")]
		MSRELATION_MAX = 4,
	}


	public enum RelationFindType
	{
		/// <summary>
		/// 推荐好友
		/// </summary>
		[AdvancedInspector.Descriptor("推荐好友", "推荐好友")]
		Friend = 1,
		/// <summary>
		/// 推荐组队
		/// </summary>
		[AdvancedInspector.Descriptor("推荐组队", "推荐组队")]
		Team = 2,
		/// <summary>
		/// 推荐师傅
		/// </summary>
		[AdvancedInspector.Descriptor("推荐师傅", "推荐师傅")]
		Master = 3,
		/// <summary>
		/// 推荐徒弟
		/// </summary>
		[AdvancedInspector.Descriptor("推荐徒弟", "推荐徒弟")]
		Disciple = 4,
		/// <summary>
		/// 推荐房间玩家
		/// </summary>
		[AdvancedInspector.Descriptor("推荐房间玩家", "推荐房间玩家")]
		Room = 5,
	}

	/// <summary>
	/// 查询玩家类别
	/// </summary>
	[AdvancedInspector.Descriptor("查询玩家类别", "查询玩家类别")]
	public enum QueryPlayerType
	{
		/// <summary>
		///  本服
		/// </summary>
		[AdvancedInspector.Descriptor(" 本服", " 本服")]
		QPT_CUR_SERVER = 0,
		/// <summary>
		///  团本
		/// </summary>
		[AdvancedInspector.Descriptor(" 团本", " 团本")]
		QPT_TEAM_COPY = 1,
		/// <summary>
		///  战场地牢
		/// </summary>
		[AdvancedInspector.Descriptor(" 战场地牢", " 战场地牢")]
		QPT_BATTLE_LOST = 2,
		/// <summary>
		/// 冠军赛
		/// </summary>
		[AdvancedInspector.Descriptor("冠军赛", "冠军赛")]
		QPT_CHAMPION = 3,
	}


	public enum RecvDiscipleState
	{

		Recv = 1,
		/// <summary>
		///  接受
		/// </summary>
		[AdvancedInspector.Descriptor(" 接受", " 接受")]
		UnRecv = 2,
	}


	public enum ActiveTimeType
	{
		/// <summary>
		/// 24小时活跃
		/// </summary>
		[AdvancedInspector.Descriptor("24小时活跃", "24小时活跃")]
		AllDay = 1,
		/// <summary>
		/// 白天
		/// </summary>
		[AdvancedInspector.Descriptor("白天", "白天")]
		Day = 2,
		/// <summary>
		/// 晚上
		/// </summary>
		[AdvancedInspector.Descriptor("晚上", "晚上")]
		Night = 3,
	}


	public enum MasterType
	{
		/// <summary>
		/// 实力强悍型
		/// </summary>
		[AdvancedInspector.Descriptor("实力强悍型", "实力强悍型")]
		StrengthValiant = 1,
		/// <summary>
		/// 认真负责型
		/// </summary>
		[AdvancedInspector.Descriptor("认真负责型", "认真负责型")]
		Responsible = 2,
		/// <summary>
		/// 聊天社交型
		/// </summary>
		[AdvancedInspector.Descriptor("聊天社交型", "聊天社交型")]
		ChitChat = 3,
	}


	public enum RelationAnnounceType
	{
		/// <summary>
		/// 师傅
		/// </summary>
		[AdvancedInspector.Descriptor("师傅", "师傅")]
		Master = 1,
		/// <summary>
		/// 徒弟
		/// </summary>
		[AdvancedInspector.Descriptor("徒弟", "徒弟")]
		Disciple = 2,
	}

}
