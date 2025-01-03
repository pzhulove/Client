using System;
using System.Text;

namespace Mock.Protocol
{

	public enum RoomType
	{
		/// <summary>
		/// 无效
		/// </summary>
		[AdvancedInspector.Descriptor("无效", "无效")]
		ROOM_TYPE_INVALID = 0,
		/// <summary>
		/// 3V3自由模式
		/// </summary>
		[AdvancedInspector.Descriptor("3V3自由模式", "3V3自由模式")]
		ROOM_TYPE_THREE_FREE = 1,
		/// <summary>
		/// 3V3匹配模式
		/// </summary>
		[AdvancedInspector.Descriptor("3V3匹配模式", "3V3匹配模式")]
		ROOM_TYPE_THREE_MATCH = 2,
		/// <summary>
		/// 3V3积分赛
		/// </summary>
		[AdvancedInspector.Descriptor("3V3积分赛", "3V3积分赛")]
		ROOM_TYPE_THREE_SCORE_WAR = 3,
		/// <summary>
		/// 乱斗模式
		/// </summary>
		[AdvancedInspector.Descriptor("乱斗模式", "乱斗模式")]
		ROOM_TYPE_MELEE = 4,

		ROOM_TYPE_MAX = 5,
	}


	public enum RoomStatus
	{
		/// <summary>
		/// 无效
		/// </summary>
		[AdvancedInspector.Descriptor("无效", "无效")]
		ROOM_STATUS_INVALID = 0,
		/// <summary>
		/// 打开状态
		/// </summary>
		[AdvancedInspector.Descriptor("打开状态", "打开状态")]
		ROOM_STATUS_OPEN = 1,
		/// <summary>
		/// 战斗准备
		/// </summary>
		[AdvancedInspector.Descriptor("战斗准备", "战斗准备")]
		ROOM_STATUS_READY = 2,
		/// <summary>
		/// 匹配阶段
		/// </summary>
		[AdvancedInspector.Descriptor("匹配阶段", "匹配阶段")]
		ROOM_STATUS_MATCH = 3,
		/// <summary>
		/// 战斗状态
		/// </summary>
		[AdvancedInspector.Descriptor("战斗状态", "战斗状态")]
		ROOM_STATUS_BATTLE = 4,

		ROOM_STATUS_NUM = 5,
	}


	public enum RoomSlotStatus
	{

		ROOM_SLOT_STATUS_INVALID = 0,
		/// <summary>
		/// 打开
		/// </summary>
		[AdvancedInspector.Descriptor("打开", "打开")]
		ROOM_SLOT_STATUS_OPEN = 1,
		/// <summary>
		/// 关闭
		/// </summary>
		[AdvancedInspector.Descriptor("关闭", "关闭")]
		ROOM_SLOT_STATUS_CLOSE = 2,
		/// <summary>
		/// 等待
		/// </summary>
		[AdvancedInspector.Descriptor("等待", "等待")]
		ROOM_SLOT_STATUS_WAIT = 3,
		/// <summary>
		/// 离线
		/// </summary>
		[AdvancedInspector.Descriptor("离线", "离线")]
		ROOM_SLOT_STATUS_OFFLINE = 4,

		ROOM_SLOT_STATUS_NUM = 5,
	}


	public enum RoomSlotGroup
	{
		/// <summary>
		/// 无效
		/// </summary>
		[AdvancedInspector.Descriptor("无效", "无效")]
		ROOM_SLOT_GROUP_INVALID = 0,
		/// <summary>
		/// 红队
		/// </summary>
		[AdvancedInspector.Descriptor("红队", "红队")]
		ROOM_SLOT_GROUP_RED = 1,
		/// <summary>
		/// 蓝队
		/// </summary>
		[AdvancedInspector.Descriptor("蓝队", "蓝队")]
		ROOM_SLOT_GROUP_BLUE = 2,

		ROOM_SLOT_GROUP_NUM = 3,
	}


	public enum RoomQuitReason
	{
		/// <summary>
		/// 无效
		/// </summary>
		[AdvancedInspector.Descriptor("无效", "无效")]
		ROOM_QUIT_REASON_INVALID = 0,
		/// <summary>
		/// 自己退出
		/// </summary>
		[AdvancedInspector.Descriptor("自己退出", "自己退出")]
		ROOM_QUIT_SELF = 1,
		/// <summary>
		/// 被房主踢出
		/// </summary>
		[AdvancedInspector.Descriptor("被房主踢出", "被房主踢出")]
		ROOM_QUIT_OWNER_KICK_OUT = 2,
		/// <summary>
		/// 解散
		/// </summary>
		[AdvancedInspector.Descriptor("解散", "解散")]
		ROOM_QUIT_DISMISS = 3,

		ROOM_QUIT_NUM = 4,
	}


	public enum RoomSlotReadyStatus
	{

		ROOM_SLOT_READY_STATUS_INVALID = 0,
		/// <summary>
		/// 接受
		/// </summary>
		[AdvancedInspector.Descriptor("接受", "接受")]
		ROOM_SLOT_READY_STATUS_ACCEPT = 1,
		/// <summary>
		/// 拒绝
		/// </summary>
		[AdvancedInspector.Descriptor("拒绝", "拒绝")]
		ROOM_SLOT_READY_STATUS_REFUSE = 2,
		/// <summary>
		/// 超时
		/// </summary>
		[AdvancedInspector.Descriptor("超时", "超时")]
		ROOM_SLOT_READY_STATUS_TIMEOUT = 3,

		ROOM_SLOT_READY_STATUS_NUM = 4,
	}


	public enum RoomSwapResult
	{

		ROOM_SWAP_RESULT_INVALID = 0,
		/// <summary>
		/// 接受
		/// </summary>
		[AdvancedInspector.Descriptor("接受", "接受")]
		ROOM_SWAP_RESULT_ACCEPT = 1,
		/// <summary>
		/// 拒绝
		/// </summary>
		[AdvancedInspector.Descriptor("拒绝", "拒绝")]
		ROOM_SWAP_RESULT_REFUSE = 2,
		/// <summary>
		/// 超时
		/// </summary>
		[AdvancedInspector.Descriptor("超时", "超时")]
		ROOM_SWAP_RESULT_TIMEOUT = 3,
		/// <summary>
		/// 取消
		/// </summary>
		[AdvancedInspector.Descriptor("取消", "取消")]
		ROOM_SWAP_RESULT_CANCEL = 4,
	}

}
