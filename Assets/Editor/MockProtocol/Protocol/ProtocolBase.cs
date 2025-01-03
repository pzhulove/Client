using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求类型", " 请求类型")]
	public enum RequestType
	{
		/// <summary>
		///  邀请组队
		/// </summary>
		[AdvancedInspector.Descriptor(" 邀请组队", " 邀请组队")]
		InviteTeam = 1,
		/// <summary>
		///  根据玩家ID请求入队
		/// </summary>
		[AdvancedInspector.Descriptor(" 根据玩家ID请求入队", " 根据玩家ID请求入队")]
		JoinTeam = 2,
		/// <summary>
		///  请求加好友
		/// </summary>
		[AdvancedInspector.Descriptor(" 请求加好友", " 请求加好友")]
		RequestFriend = 3,
		/// <summary>
		///  请求拜师
		/// </summary>
		[AdvancedInspector.Descriptor(" 请求拜师", " 请求拜师")]
		RequestMaster = 4,
		/// <summary>
		///  请求收徒
		/// </summary>
		[AdvancedInspector.Descriptor(" 请求收徒", " 请求收徒")]
		RequestDisciple = 5,
		/// <summary>
		///  根据队伍ID加入队伍
		/// </summary>
		[AdvancedInspector.Descriptor(" 根据队伍ID加入队伍", " 根据队伍ID加入队伍")]
		JoinTeamByTeamID = 21,
		/// <summary>
		///  请求通过名字加好友
		/// </summary>
		[AdvancedInspector.Descriptor(" 请求通过名字加好友", " 请求通过名字加好友")]
		RequestFriendByName = 29,
		/// <summary>
		///  挑战
		/// </summary>
		[AdvancedInspector.Descriptor(" 挑战", " 挑战")]
		Request_Challenge_PK = 30,
		/// <summary>
		///  邀请公会
		/// </summary>
		[AdvancedInspector.Descriptor(" 邀请公会", " 邀请公会")]
		InviteJoinGuild = 31,
		/// <summary>
		/// 公平竞技场邀请
		/// </summary>
		[AdvancedInspector.Descriptor("公平竞技场邀请", "公平竞技场邀请")]
		Request_Equal_PK = 32,
	}

	/// <summary>
	///  刷新类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 刷新类型", " 刷新类型")]
	public enum RefreshType
	{
		/// <summary>
		///  不刷新
		/// </summary>
		[AdvancedInspector.Descriptor(" 不刷新", " 不刷新")]
		REFRESH_TYPE_NONE = 0,
		/// <summary>
		///  每月刷新
		/// </summary>
		[AdvancedInspector.Descriptor(" 每月刷新", " 每月刷新")]
		REFRESH_TYPE_PER_MONTH = 1,
		/// <summary>
		///  每周刷新
		/// </summary>
		[AdvancedInspector.Descriptor(" 每周刷新", " 每周刷新")]
		REFRESH_TYPE_PER_WEEK = 2,
		/// <summary>
		///  每日刷新
		/// </summary>
		[AdvancedInspector.Descriptor(" 每日刷新", " 每日刷新")]
		REFRESH_TYPE_PER_DAY = 3,
	}

	/// <summary>
	///  新手引导选择标志
	/// </summary>
	[AdvancedInspector.Descriptor(" 新手引导选择标志", " 新手引导选择标志")]
	public enum NoviceGuideChooseFlag
	{
		/// <summary>
		///  初态
		/// </summary>
		[AdvancedInspector.Descriptor(" 初态", " 初态")]
		NGCF_INIT = 0,
		/// <summary>
		///  弹出选择
		/// </summary>
		[AdvancedInspector.Descriptor(" 弹出选择", " 弹出选择")]
		NGCF_POPUP = 1,
		/// <summary>
		///  选择跳过引导
		/// </summary>
		[AdvancedInspector.Descriptor(" 选择跳过引导", " 选择跳过引导")]
		NGCF_PASS = 2,
		/// <summary>
		///  选择不跳过
		/// </summary>
		[AdvancedInspector.Descriptor(" 选择不跳过", " 选择不跳过")]
		NGCF_NOT_PASS = 3,
	}

}
